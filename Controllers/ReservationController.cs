using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RestaurantManagement.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationController(IReservationService reservationService, UserManager<ApplicationUser> userManager)
        {
            _reservationService = reservationService;
            _userManager = userManager;
        }

        // GET: Reservation
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Index()
        {
            var orders = await _reservationService.GetAllReservationsAsync();
            return View(orders);
        }

        // GET: Reservation/Details/5
        [Authorize] // Cho phép tất cả người dùng đã xác thực
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get current user
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            var isEmployee = User.IsInRole("Employee");
            
            // Nếu là Admin hoặc Employee, lấy thông tin đặt bàn mà không cần kiểm tra UserId
            Order reservation;
            if (isAdmin || isEmployee)
            {
                reservation = await _reservationService.GetReservationByIdAsync(id.Value);
            }
            else
            {
                // Nếu là khách hàng thông thường thì chỉ lấy đơn đặt bàn của chính họ
                reservation = await _reservationService.GetReservationByIdAsync(id.Value, userId);
            }

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservation/Create
        [Authorize]
        public async Task<IActionResult> Create(int? peopleCount)
        {
            var viewModel = await _reservationService.PrepareCreateModelAsync(peopleCount);
            return View(viewModel);
        }

        // POST: Reservation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(ReservationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Get current user
                var userId = _userManager.GetUserId(User);
                
                var result = await _reservationService.CreateReservationAsync(viewModel, userId);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.ErrorMessage;
                    return RedirectToAction(nameof(MyReservations));
                }
                else
                {
                    ModelState.AddModelError("TableId", result.ErrorMessage);
                }
            }
            
            // Reload available tables if model validation fails
            var availableTables = await _reservationService.GetAvailableTablesAsync(viewModel.PeopleCount);
            var tableList = new List<Table>();
            
            // Convert dynamic objects to Table objects for the view model
            foreach (dynamic tableObj in availableTables)
            {
                var table = new Table
                {
                    Id = tableObj.id,
                    Name = tableObj.name,
                    Capacity = tableObj.capacity,
                    Floor = new Floor { Name = tableObj.floor }
                };
                tableList.Add(table);
            }
            
            viewModel.AvailableTables = tableList;
            return View(viewModel);
        }

        // GET: Reservation/Edit/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _reservationService.PrepareEditModelAsync(id.Value);
                
            if (viewModel == null)
            {
                return NotFound();
            }
            
            ViewData["Tables"] = await _reservationService.GetTablesSelectListAsync(viewModel.TableId);
            ViewData["Customers"] = await _reservationService.GetCustomersSelectListAsync(viewModel.CustomerId);
            
            return View(viewModel);
        }
        
        // POST: Reservation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id, ReservationEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _reservationService.UpdateReservationAsync(viewModel);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage);
                }
            }
            
            ViewData["Tables"] = await _reservationService.GetTablesSelectListAsync(viewModel.TableId);
            ViewData["Customers"] = await _reservationService.GetCustomersSelectListAsync(viewModel.CustomerId);
            
            return View(viewModel);
        }

        // GET: Reservation/Delete/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _reservationService.GetReservationForDeleteAsync(id.Value);
                
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Reservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _reservationService.DeleteReservationAsync(id);
            TempData["SuccessMessage"] = "Xóa đặt bàn thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservation/ConfirmReservation/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ConfirmReservation(int id)
        {
            await _reservationService.ConfirmReservationAsync(id);
            TempData["SuccessMessage"] = "Đã xác nhận đặt bàn!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservation/Cancel/5
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            // Lấy thông tin user hiện tại
            var userId = _userManager.GetUserId(User);
            
            var result = await _reservationService.CancelReservationAsync(id, userId);
            
            if (result.Success)
                TempData["SuccessMessage"] = result.ErrorMessage;
            else
                TempData["ErrorMessage"] = result.ErrorMessage;
                
            return RedirectToAction(nameof(MyReservations));
        }

        // GET: Reservation/MyReservations
        [Authorize]
        public async Task<IActionResult> MyReservations()
        {
            // Get current user
            var userId = _userManager.GetUserId(User);
            var orders = await _reservationService.GetUserReservationsAsync(userId);
            return View(orders);
        }

        // Phương thức Ajax để lấy bàn phù hợp dựa trên số người
        [HttpGet]
        [Authorize]
        public async Task<JsonResult> GetAvailableTables(int peopleCount)
        {
            var availableTables = await _reservationService.GetAvailableTablesAsync(peopleCount);
            return Json(availableTables);
        }
    }
} 