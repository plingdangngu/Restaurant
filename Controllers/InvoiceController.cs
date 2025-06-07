using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvoiceController(IInvoiceService invoiceService, UserManager<ApplicationUser> userManager)
        {
            _invoiceService = invoiceService;
            _userManager = userManager;
        }

        // GET: Invoice
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, string searchString = "", int page = 1)
        {
            int pageSize = 10;
            
            // Set default date range if not provided (current month)
            if (!fromDate.HasValue)
            {
                fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            
            if (!toDate.HasValue)
            {
                toDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1); // End of today
            }
            
            // Get invoices for the date range
            var reportData = await _invoiceService.GetInvoiceReportAsync(fromDate, toDate);
            
            // Filter by search string if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                reportData = reportData.Where(i => 
                    i.TableName.ToLower().Contains(searchString) ||
                    i.UserName.ToLower().Contains(searchString) ||
                    i.InvoiceId.ToString().Contains(searchString)
                ).ToList();
            }
            
            // Calculate total count for pagination
            var totalCount = reportData.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            
            // Ensure page is within range
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
            
            // Get paged results
            var pagedData = reportData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            // Calculate total revenue
            decimal totalRevenue = reportData.Sum(i => i.Total);
            
            // Prepare view data
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");
            ViewData["SearchString"] = searchString;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["TotalRevenue"] = totalRevenue;
            
            return View(pagedData);
        }

        // GET: Invoice/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _invoiceService.PrepareInvoiceViewModelAsync(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // GET: Invoice/Print/5
        public async Task<IActionResult> Print(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _invoiceService.PrepareInvoiceViewModelAsync(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }

            // Set print layout (no header/footer)
            return View(viewModel);
        }

        // GET: Invoice/Revenue
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Revenue(DateTime? fromDate, DateTime? toDate, string groupBy = "day")
        {
            // Set default date range if not provided (current month)
            if (!fromDate.HasValue)
            {
                fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            
            if (!toDate.HasValue)
            {
                toDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1); // End of today
            }
            
            // Get invoices for the date range
            var invoices = await _invoiceService.GetInvoiceReportAsync(fromDate, toDate);
            
            // Group by selected interval (day, week, month)
            var revenueData = new List<RevenueReportItem>();
            
            switch (groupBy.ToLower())
            {
                case "day":
                    var dailyData = invoices
                        .GroupBy(i => i.PaymentDate.Date)
                        .Select(g => new
                        {
                            Date = g.Key,
                            TotalAmount = g.Sum(i => i.Total),
                            Count = g.Count()
                        })
                        .OrderBy(g => g.Date)
                        .ToList();
                        
                    foreach (var item in dailyData)
                    {
                        revenueData.Add(new RevenueReportItem
                        {
                            Period = item.Date.ToString("dd/MM/yyyy"),
                            TotalAmount = item.TotalAmount,
                            InvoiceCount = item.Count
                        });
                    }
                    break;
                    
                case "week":
                    var weeklyData = invoices
                        .GroupBy(i => new { Year = i.PaymentDate.Year, Week = GetWeekNumber(i.PaymentDate) })
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Week = g.Key.Week,
                            TotalAmount = g.Sum(i => i.Total),
                            Count = g.Count()
                        })
                        .OrderBy(g => g.Year)
                        .ThenBy(g => g.Week)
                        .ToList();
                        
                    foreach (var item in weeklyData)
                    {
                        revenueData.Add(new RevenueReportItem
                        {
                            Period = $"Tuần {item.Week}, {item.Year}",
                            TotalAmount = item.TotalAmount,
                            InvoiceCount = item.Count
                        });
                    }
                    break;
                    
                case "month":
                    var monthlyData = invoices
                        .GroupBy(i => new { Year = i.PaymentDate.Year, Month = i.PaymentDate.Month })
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            TotalAmount = g.Sum(i => i.Total),
                            Count = g.Count()
                        })
                        .OrderBy(g => g.Year)
                        .ThenBy(g => g.Month)
                        .ToList();
                        
                    foreach (var item in monthlyData)
                    {
                        revenueData.Add(new RevenueReportItem
                        {
                            Period = $"Tháng {item.Month}/{item.Year}",
                            TotalAmount = item.TotalAmount,
                            InvoiceCount = item.Count
                        });
                    }
                    break;
            }
            
            // Calculate total revenue
            decimal totalRevenue = invoices.Sum(i => i.Total);
            int totalInvoices = invoices.Count;
            
            // Prepare view data
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");
            ViewData["GroupBy"] = groupBy;
            ViewData["TotalRevenue"] = totalRevenue;
            ViewData["TotalInvoices"] = totalInvoices;
            
            return View(revenueData);
        }
        
        // Helper method to get week number
        private int GetWeekNumber(DateTime date)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("vi-VN");
            System.Globalization.Calendar calendar = cultureInfo.Calendar;
            
            return calendar.GetWeekOfYear(date, 
                System.Globalization.CalendarWeekRule.FirstDay, 
                DayOfWeek.Monday);
        }
        
        // Helper class for revenue report data
        public class RevenueReportItem
        {
            public string Period { get; set; }
            public decimal TotalAmount { get; set; }
            public int InvoiceCount { get; set; }
            public decimal AverageAmount => InvoiceCount > 0 ? Math.Round(TotalAmount / InvoiceCount, 2) : 0;
        }
    }
} 