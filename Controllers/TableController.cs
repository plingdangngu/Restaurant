using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.ViewModels;
using RestaurantManagement.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace RestaurantManagement.Controllers
{
    public class TableController : Controller
    {
        private readonly ITableService _tableService;
        
        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }
        
        // GET: Table
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Index()
        {
            var tables = await _tableService.GetAllTablesAsync();
            return View(tables);
        }
        
        // GET: Table/Details/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _tableService.GetTableByIdAsync(id.Value);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }
        
        // GET: Table/Create
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create()
        {
            ViewData["FloorId"] = await _tableService.GetFloorsSelectListAsync();
            return View();
        }
        
        // POST: Table/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create([Bind("Id,Name,FloorId,Capacity,Status,IsOccupied,Notes")] Table table)
        {
            if (ModelState.IsValid)
            {
                var result = await _tableService.CreateTableAsync(table);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            
            ViewData["FloorId"] = await _tableService.GetFloorsSelectListAsync(table.FloorId);
            return View(table);
        }
        
        // GET: Table/Edit/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _tableService.GetTableByIdAsync(id.Value);
            if (table == null)
            {
                return NotFound();
            }
            
            ViewData["FloorId"] = await _tableService.GetFloorsSelectListAsync(table.FloorId);
            return View(table);
        }
        
        // POST: Table/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,FloorId,Capacity,Status,IsOccupied,Notes")] Table table)
        {
            if (id != table.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _tableService.UpdateTableAsync(table);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            
            ViewData["FloorId"] = await _tableService.GetFloorsSelectListAsync(table.FloorId);
            return View(table);
        }
        
        // GET: Table/Delete/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _tableService.GetTableForDeleteAsync(id.Value);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }
        
        // POST: Table/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _tableService.DeleteTableAsync(id);
            
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;
                
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Table/FloorView
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> FloorView()
        {
            var tablesByFloor = await _tableService.GetTablesGroupedByFloorAsync();
            return View(tablesByFloor);
        }
        
        // GET: Table/Available
        [AllowAnonymous]
        public async Task<IActionResult> Available(int? capacity)
        {
            var tables = await _tableService.GetAvailableTablesAsync(capacity);
            return View(tables);
        }
        
        // GET: Table/MoveTable/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> MoveTable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var table = await _tableService.GetTableByIdAsync(id.Value);
                
            if (table == null)
            {
                return NotFound();
            }
            
            // Lấy danh sách tầng và bàn trống
            var floors = await _tableService.GetFloorsAsync();
            var availableTables = await _tableService.GetAvailableTablesAsync(null);
                
            var viewModel = new MoveTableViewModel
            {
                SourceTableId = table.Id,
                SourceTableName = table.Name,
                SourceFloorName = table.Floor.Name
            };
            
            ViewData["TargetTableId"] = new SelectList(availableTables, "Id", "Name");
            
            return View(viewModel);
        }
        
        // POST: Table/MoveTable
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> MoveTable(MoveTableViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var sourceTable = await _tableService.GetTableByIdAsync(viewModel.SourceTableId);
                    
                var targetTable = await _tableService.GetTableByIdAsync(viewModel.TargetTableId);
                    
                if (sourceTable == null || targetTable == null)
                {
                    return NotFound();
                }
                
                // Kiểm tra bàn đích có đang trống không
                if (targetTable.IsOccupied)
                {
                    ModelState.AddModelError("TargetTableId", "Bàn đích đang được sử dụng!");
                    
                    var availableTables = await _tableService.GetAvailableTablesAsync(null);
                        
                    ViewData["TargetTableId"] = new SelectList(availableTables, "Id", "Name", viewModel.TargetTableId);
                    return View(viewModel);
                }
                
                // Chuyển tất cả đơn hàng sang bàn mới
                await _tableService.MoveOrdersAsync(viewModel.SourceTableId, viewModel.TargetTableId);
                
                // Cập nhật trạng thái bàn
                sourceTable.IsOccupied = false;
                targetTable.IsOccupied = true;
                
                await _tableService.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Đã chuyển bàn {sourceTable.Name} sang {targetTable.Name} thành công!";
                return RedirectToAction(nameof(Index));
            }
            
            var availableTablesForView = await _tableService.GetAvailableTablesAsync(null);
                
            ViewData["TargetTableId"] = new SelectList(availableTablesForView, "Id", "Name", viewModel.TargetTableId);
            return View(viewModel);
        }
        
        // GET: Table/AvailableTables
        // Cho phép khách hàng xem bàn trống
        [AllowAnonymous]
        public async Task<IActionResult> AvailableTables(int? floorId)
        {
            // Truy vấn tất cả các bàn 
            var tables = await _tableService.GetAllTablesAsync();
            
            // Nếu chọn tầng cụ thể
            if (floorId.HasValue)
            {
                tables = tables.Where(t => t.FloorId == floorId).ToList();
            }
            
            // Lấy các bàn còn trống
            var availableTables = tables
                .Where(t => t.Status == "Available" || t.Status == null)
                .OrderBy(t => t.Floor.Name)
                .ThenBy(t => t.Name)
                .ToList();
                
            // Lấy tất cả các tầng để hiển thị trong dropdown
            var floors = await _tableService.GetFloorsAsync();
                
            ViewData["Floors"] = floors;
            ViewData["SelectedFloorId"] = floorId;
            
            return View(availableTables);
        }
    }
} 