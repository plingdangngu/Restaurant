using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RestaurantManagement.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        // GET: Order
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Index(string status = "All", string searchString = "", int page = 1)
        {
            var orders = await _orderService.GetAllOrdersAsync();
            
            // Filter by status if needed
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                orders = orders.Where(o => o.Status == status).ToList();
            }
            
            // Filter by search string if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => 
                    o.Id.ToString().Contains(searchString) ||
                    (o.Table != null && o.Table.Name.Contains(searchString)) ||
                    (o.Customer != null && o.Customer.Name.Contains(searchString)) ||
                    (o.Employee != null && o.Employee.FullName.Contains(searchString))
                ).ToList();
            }
            
            // Calculate total value
            decimal totalValue = orders.Sum(o => o.TotalAmount);
            
            // Prepare status list for dropdown
            var statusList = new List<string> { "All", "Pending", "Processing", "Completed", "Cancelled" };
            
            // Pagination
            int pageSize = 10;
            int totalItems = orders.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            // Adjust page if out of range
            page = Math.Max(1, Math.Min(page, totalPages));
            
            // Get items for current page
            var pagedOrders = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            // Set ViewData
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = Math.Max(1, totalPages);
            ViewData["Status"] = status;
            ViewData["SearchString"] = searchString;
            ViewData["StatusList"] = statusList;
            ViewData["TotalValue"] = totalValue;
            ViewData["IsAdminView"] = true;
            
            return View(pagedOrders);
        }

        // GET: Order/MyOrders
        [Authorize]
        public async Task<IActionResult> MyOrders(string status = "All", string searchString = "", int page = 1)
        {
            var currentUserId = _userManager.GetUserId(User);
            
            // Get all orders for the current user only
            var userOrders = await _orderService.GetUserOrdersAsync(currentUserId);
            
            // Filter by status if needed
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                userOrders = userOrders.Where(o => o.Status == status).ToList();
            }
            
            // Filter by search string if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                userOrders = userOrders.Where(o => 
                    o.Id.ToString().Contains(searchString) ||
                    (o.Table != null && o.Table.Name.Contains(searchString))
                ).ToList();
            }
            
            // Calculate total value
            decimal totalValue = userOrders.Sum(o => o.TotalAmount);
            
            // Prepare status list for dropdown
            var statusList = new List<string> { "All", "Pending", "Processing", "Completed", "Cancelled" };
            
            // Pagination
            int pageSize = 10;
            int totalItems = userOrders.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            // Adjust page if out of range
            page = Math.Max(1, Math.Min(page, totalPages));
            
            // Get items for current page
            var pagedOrders = userOrders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            // Set ViewData
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = Math.Max(1, totalPages);
            ViewData["Status"] = status;
            ViewData["SearchString"] = searchString;
            ViewData["StatusList"] = statusList;
            ViewData["TotalValue"] = totalValue;
            ViewData["IsAdminView"] = false;
            
            return View(pagedOrders);
        }

        // GET: Order/Details/5
        [Authorize] // Allow any authenticated user
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetOrderForDetailsAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner of the order or has admin/employee role
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            var isEmployee = User.IsInRole("Employee");
            
            if (order.EmployeeId != currentUserId && order.CustomerId != currentUserId && order.UserId != currentUserId && !isAdmin && !isEmployee)
            {
                return Forbid();
            }

            // Tính tổng tiền đơn hàng
            decimal totalAmount = 0;
            if (order.OrderItems != null && order.OrderItems.Any())
            {
                totalAmount = order.OrderItems.Sum(item => item.Price * item.Quantity);
            }
            
            // Truyền tổng tiền vào ViewData để hiển thị trên trang
            ViewData["TotalAmount"] = totalAmount;

            return View(order);
        }

        // GET: Order/Create
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create()
        {
            var viewModel = await _orderService.PrepareCreateViewModelAsync();
            
            // Kiểm tra nếu không có bàn trống
            if (viewModel.TablesSelectList == null || !viewModel.TablesSelectList.Any())
            {
                TempData["ErrorMessage"] = "Không có bàn trống nào khả dụng. Vui lòng kiểm tra lại trạng thái các bàn.";
                return RedirectToAction(nameof(MyOrders));
            }
            
            return View(viewModel);
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create(OrderCreateViewModel viewModel)
        {
            try
            {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var result = await _orderService.CreateOrderAsync(viewModel, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                        
                        if (User.IsInRole("Admin") || User.IsInRole("Employee"))
                    return RedirectToAction(nameof(Index));
                        else
                            return RedirectToAction(nameof(MyOrders));
                    }
                    else
                    {
                        ModelState.AddModelError("", result.Message);
                        TempData["ErrorMessage"] = result.Message;
                    }
                }
                else
                {
                    // Log model state errors
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["ErrorMessage"] = $"Lỗi xác thực: {errors}";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi không mong muốn: {ex.Message}";
                }

            // Nếu có lỗi, tải lại dữ liệu và trả về view
            viewModel.TablesSelectList = await _orderService.GetTablesSelectListAsync(viewModel.TableId);
            viewModel.Customers = await _orderService.GetCustomersSelectListAsync();
            return View(viewModel);
        }

        // GET: Order/Edit/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _orderService.PrepareEditViewModelAsync(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id, OrderEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _orderService.UpdateOrderAsync(viewModel);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    
                    if (User.IsInRole("Admin") || User.IsInRole("Employee"))
                    return RedirectToAction(nameof(Index));
                    else
                        return RedirectToAction(nameof(MyOrders));
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            return View(viewModel);
        }

        // GET: Order/Delete/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetOrderForDeleteAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;
                
            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            return RedirectToAction(nameof(Index));
            else
                return RedirectToAction(nameof(MyOrders));
        }

        // GET: Order/AddItem/5
        [Authorize]
        public async Task<IActionResult> AddItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetOrderByIdAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập: người dùng phải là chủ đơn hàng hoặc có quyền Admin/Employee
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            var isEmployee = User.IsInRole("Employee");
            
            if (order.CustomerId != currentUserId && order.UserId != currentUserId && !isAdmin && !isEmployee)
            {
                return Forbid();
            }

            // Kiểm tra trạng thái của đơn hàng
            if (order.Status == "Completed" || order.Status == "Canceled")
            {
                TempData["ErrorMessage"] = $"Không thể thêm món vào đơn hàng có trạng thái {order.Status}";
                return RedirectToAction("Details", new { id = id.Value });
            }

            // Kiểm tra trạng thái của bàn
            var table = await _orderService.GetTableByIdAsync(order.TableId);
            if (table == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin bàn";
                return RedirectToAction("Details", new { id = id.Value });
            }

            var viewModel = await _orderService.PrepareAddItemViewModelAsync(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }

            // Kiểm tra danh sách món ăn
            if (viewModel.MenuItems == null || !viewModel.MenuItems.Any())
            {
                TempData["ErrorMessage"] = "Không có món ăn nào khả dụng";
            }

            return View(viewModel);
        }

        // POST: Order/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddItem(AddItemViewModel viewModel)
        {
            // Loại bỏ validation của các trường không bắt buộc
            ModelState.Remove("MenuItems");
            ModelState.Remove("SpecialRequests");
            
            // Check if the current user is the owner of the order or has admin/employee role
            var order = await _orderService.GetOrderByIdAsync(viewModel.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập: người dùng phải là chủ đơn hàng hoặc có quyền Admin/Employee
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            var isEmployee = User.IsInRole("Employee");
            
            if (order.CustomerId != currentUserId && order.UserId != currentUserId && !isAdmin && !isEmployee)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var result = await _orderService.AddItemToOrderAsync(viewModel);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Details", new { id = viewModel.OrderId });
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            else
            {
                // Log mã lỗi
                var errorMessages = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = $"Lỗi xác thực: {errorMessages}";
            }

            // Khôi phục dữ liệu ViewModel đầy đủ nếu có lỗi
            var updatedViewModel = await _orderService.PrepareAddItemViewModelAsync(viewModel.OrderId);
            if (updatedViewModel != null)
            {
                updatedViewModel.Quantity = viewModel.Quantity > 0 ? viewModel.Quantity : 1;
                updatedViewModel.Notes = viewModel.Notes;
                updatedViewModel.MenuItemId = viewModel.MenuItemId;
                return View(updatedViewModel);
            }

            return View(viewModel);
        }

        // GET: Order/RemoveItem/5
        [Authorize]
        public async Task<IActionResult> RemoveItem(int? id, int? orderId)
        {
            if (id == null || orderId == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetOrderByIdAsync(orderId.Value);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập: người dùng phải là chủ đơn hàng hoặc có quyền Admin/Employee
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            var isEmployee = User.IsInRole("Employee");
            
            if (order.CustomerId != currentUserId && order.UserId != currentUserId && !isAdmin && !isEmployee)
            {
                return Forbid();
            }

            var result = await _orderService.RemoveItemFromOrderAsync(id.Value);
            
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;
                
            return RedirectToAction("Details", new { id = orderId });
        }

        // GET: Order/Payment/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Payment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetOrderForPaymentAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Payment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Payment(int id)
        {
            var result = await _orderService.CreatePaymentAsync(id);
            
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                
                if (User.IsInRole("Admin") || User.IsInRole("Employee"))
                return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction(nameof(MyOrders));
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                
                if (User.IsInRole("Admin") || User.IsInRole("Employee"))
                return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction(nameof(MyOrders));
            }
        }

        // GET: Order/ActiveOrders
        [Authorize(Roles = "Admin,Employee")]
        public async Task<JsonResult> ActiveOrders()
        {
            var activeOrders = await _orderService.GetActiveOrdersAsync();
            return Json(activeOrders);
        }

        // POST: Order/AddMultipleItems
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddMultipleItems(int orderId, string cartItems, string notes)
        {
            // Kiểm tra đơn hàng
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập: người dùng phải là chủ đơn hàng hoặc có quyền Admin/Employee
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            var isEmployee = User.IsInRole("Employee");
            
            if (order.CustomerId != currentUserId && order.UserId != currentUserId && !isAdmin && !isEmployee)
            {
                return Forbid();
            }

            // Kiểm tra trạng thái đơn hàng
            if (order.Status == "Completed" || order.Status == "Canceled")
            {
                TempData["ErrorMessage"] = $"Không thể thêm món vào đơn hàng có trạng thái {order.Status}";
                return RedirectToAction("Details", new { id = orderId });
            }

            try
            {
                // Chuyển đổi chuỗi JSON thành danh sách CartItemViewModel
                var cartItemsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartItems);
                
                if (cartItemsList == null || !cartItemsList.Any())
                {
                    TempData["ErrorMessage"] = "Không có món ăn nào để thêm vào đơn hàng";
                    return RedirectToAction("AddItem", new { id = orderId });
                }
                
                // Thêm từng món ăn vào đơn hàng
                int successCount = 0;
                List<string> errorMessages = new List<string>();

                foreach (var cartItem in cartItemsList)
                {
                    var addItemResult = await _orderService.AddItemToOrderAsync(new AddItemViewModel
                    {
                        OrderId = orderId,
                        MenuItemId = cartItem.MenuItemId,
                        Quantity = cartItem.Quantity,
                        Notes = cartItem.Notes ?? notes // Ưu tiên ghi chú riêng của món ăn, nếu không có thì dùng ghi chú chung
                    });

                    if (addItemResult.Success)
                    {
                        successCount++;
                    }
                    else
                    {
                        errorMessages.Add($"{cartItem.MenuItemName}: {addItemResult.Message}");
                    }
                }

                // Thông báo kết quả
                if (successCount > 0)
                {
                    TempData["SuccessMessage"] = $"Đã thêm {successCount} món vào đơn hàng thành công!";
                }

                if (errorMessages.Any())
                {
                    TempData["ErrorMessage"] = $"Có lỗi khi thêm một số món: {string.Join("; ", errorMessages)}";
                }

                return RedirectToAction("Details", new { id = orderId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xử lý đơn hàng: {ex.Message}";
                return RedirectToAction("AddItem", new { id = orderId });
            }
        }

        // GET: Order/UpdateStatus
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            // Validación básica
            if (string.IsNullOrEmpty(status))
            {
                TempData["ErrorMessage"] = "Trạng thái không hợp lệ";
                return RedirectToAction("Details", new { id });
            }

            // Validación de estado
            if (status != "Pending" && status != "Processing" && 
                status != "Completed" && status != "Cancelled")
            {
                TempData["ErrorMessage"] = "Trạng thái không được hỗ trợ";
                return RedirectToAction("Details", new { id });
            }

            // Actualización a través del servicio
            var result = await _orderService.ChangeOrderStatusAsync(id, status);
            
            if (result.Success)
                TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn hàng thành '{GetStatusDisplayName(status)}'";
            else
                TempData["ErrorMessage"] = result.Message;
                
            return RedirectToAction("Details", new { id });
        }

        // GET: Order/UpdateItemStatus
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateItemStatus(int id, string status, int? orderId = null)
        {
            // Validar estados válidos
            if (string.IsNullOrEmpty(status))
            {
                TempData["ErrorMessage"] = "Trạng thái không hợp lệ";
                return RedirectToAction("Details", new { id = orderId });
            }

            // Validar que el estado es permitido
            if (status != "Pending" && status != "Processing" && status != "Ready" && 
                status != "Served" && status != "Cancelled")
            {
                TempData["ErrorMessage"] = "Trạng thái không được hỗ trợ";
                return RedirectToAction("Details", new { id = orderId });
            }

            try
            {
                // Buscar el artículo
                var item = await _orderService.GetOrderItemAsync(id);
                if (item == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy món ăn";
                    return RedirectToAction("Details", new { id = orderId });
                }

                // Guardar el ID de la orden para redirección
                var orderIdToReturn = orderId ?? item.OrderId;

                // Actualizar el estado
                item.Status = status;
                var result = await _orderService.UpdateOrderItemAsync(item);

                if (result.Success)
                    TempData["SuccessMessage"] = $"Đã cập nhật trạng thái món '{item.MenuItem?.Name}' thành '{GetItemStatusDisplayName(status)}'";
                else
                    TempData["ErrorMessage"] = result.Message;

                return RedirectToAction("Details", new { id = orderIdToReturn });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Details", new { id = orderId });
            }
        }

        // Método auxiliar para obtener nombres de visualización en vietnamita
        private string GetStatusDisplayName(string status)
        {
            return status switch
            {
                "Pending" => "Chờ xử lý",
                "Processing" => "Đang xử lý", 
                "Completed" => "Hoàn thành",
                "Cancelled" => "Đã hủy",
                _ => status
            };
        }

        // Método auxiliar para obtener nombres de visualización de estados de elementos en vietnamita
        private string GetItemStatusDisplayName(string status)
        {
            return status switch
            {
                "Pending" => "Chờ xử lý",
                "Processing" => "Đang chế biến",
                "Ready" => "Sẵn sàng",
                "Served" => "Đã phục vụ",
                "Cancelled" => "Đã hủy",
                _ => status
            };
        }
    }
} 