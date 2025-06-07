using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;
using RestaurantManagement.Extensions;
using RestaurantManagement.Repositories.Interfaces;
using RestaurantManagement.Services.Interfaces;
using RestaurantManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ITableRepository tableRepository,
            IMenuItemRepository menuItemRepository,
            IUserRepository userRepository,
            ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _menuItemRepository = menuItemRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<Order> GetOrderWithDetailsAsync(int id)
        {
            return await _orderRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<Order> GetOrderForDetailsAsync(int id)
        {
            return await _orderRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<OrderCreateViewModel> PrepareCreateViewModelAsync()
        {
            var viewModel = new OrderCreateViewModel
            {
                OrderDate = DateTime.Now,
                TablesSelectList = await GetTablesSelectListAsync(),
                Customers = await GetCustomersSelectListAsync()
            };
            
            return viewModel;
        }

        public async Task<SelectList> GetCustomersSelectListAsync()
        {
            // Lấy danh sách khách hàng từ customer repository
            var customers = await _customerRepository.GetAllAsync();
            
            // Chuyển đổi thành danh sách cho SelectList
            var selectListItems = customers.Select(c => new
            {
                Id = c.Id.ToString(), // Convert ID to string
                DisplayName = $"{c.Name} {(string.IsNullOrEmpty(c.Phone) ? "" : $"({c.Phone})")}"
            }).ToList();
            
            return new SelectList(selectListItems, "Id", "DisplayName");
        }

        public async Task<(bool Success, string Message, Order Order)> CreateOrderAsync(OrderCreateViewModel model, string userId)
        {
            if (model == null)
                return (false, "Model không hợp lệ", null);

            try
            {
                // Kiểm tra bàn tồn tại và đang trống
                var table = await _tableRepository.GetByIdAsync(model.TableId);
                if (table == null)
                    return (false, "Bàn không tồn tại", null);

                if (table.IsOccupied)
                    return (false, "Bàn đã được sử dụng", null);

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return (false, "Người dùng không hợp lệ", null);

                var order = new Order
                {
                    TableId = model.TableId,
                    UserId = userId,
                    OrderDate = model.OrderDate,
                    Status = "Pending",
                    Notes = model.Notes
                };

                await _orderRepository.AddAsync(order);

                // Cập nhật trạng thái bàn
                table.IsOccupied = true;
                _tableRepository.Update(table);

                await _orderRepository.SaveChangesAsync();

                return (true, "Tạo đơn hàng thành công!", order);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<OrderEditViewModel> PrepareEditViewModelAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return null;

            var viewModel = new OrderEditViewModel
            {
                Id = order.Id,
                TableId = order.TableId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Notes = order.Notes,
                Tables = await GetTablesSelectListAsync(order.TableId)
            };

            return viewModel;
        }

        public async Task<(bool Success, string Message)> UpdateOrderAsync(OrderEditViewModel model)
        {
            if (model == null)
                return (false, "Model không hợp lệ");

            try
            {
                var order = await _orderRepository.GetByIdAsync(model.Id);
                if (order == null)
                    return (false, "Không tìm thấy đơn hàng");

                // Nếu đổi bàn
                if (order.TableId != model.TableId)
                {
                    // Giải phóng bàn cũ
                    var oldTable = await _tableRepository.GetByIdAsync(order.TableId);
                    if (oldTable != null)
                    {
                        oldTable.IsOccupied = false;
                        _tableRepository.Update(oldTable);
                    }

                    // Cập nhật bàn mới
                    var newTable = await _tableRepository.GetByIdAsync(model.TableId);
                    if (newTable == null)
                        return (false, "Bàn mới không tồn tại");

                    if (newTable.IsOccupied)
                        return (false, "Bàn mới đã được sử dụng");

                    newTable.IsOccupied = true;
                    _tableRepository.Update(newTable);
                }

                order.TableId = model.TableId;
                order.OrderDate = model.OrderDate;
                order.Status = model.Status;
                order.Notes = model.Notes;

                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();

                return (true, "Cập nhật đơn hàng thành công!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderExistsAsync(model.Id))
                    return (false, "Không tìm thấy đơn hàng");
                else
                    throw;
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<Order> GetOrderForDeleteAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> DeleteOrderAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);

                if (order == null)
                    return (false, "Không tìm thấy đơn hàng");

                // Kiểm tra nếu đơn hàng đã thanh toán
                if (order.Status == "Completed")
                    return (false, "Không thể xóa đơn hàng đã hoàn thành");

                // Xóa tất cả các mục trong đơn hàng
                _orderRepository.RemoveRange(order.OrderItems);

                // Giải phóng bàn
                var table = await _tableRepository.GetByIdAsync(order.TableId);
                if (table != null)
                {
                    table.IsOccupied = false;
                    _tableRepository.Update(table);
                }

                _orderRepository.Remove(order);
                await _orderRepository.SaveChangesAsync();

                return (true, "Xóa đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<bool> OrderExistsAsync(int id)
        {
            return await _orderRepository.ExistsAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetPendingOrdersAsync()
        {
            return await _orderRepository.GetAllAsync(o => o.Status == "Pending" || o.Status == "Processing");
        }

        public async Task<List<Order>> GetOrdersByTableAsync(int tableId)
        {
            return await _orderRepository.GetAllAsync(o => o.TableId == tableId && (o.Status == "Pending" || o.Status == "Processing"));
        }

        public async Task<AddItemViewModel> PrepareAddItemViewModelAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return null;

            // Lấy danh sách món ăn có sẵn và nhóm theo danh mục
            var menuItems = await _menuItemRepository.GetActiveMenuItemsAsync();
            
            // Nếu không có món ăn nào khả dụng, trả về một ViewModel với danh sách trống
            if (menuItems == null || !menuItems.Any())
            {
                return new AddItemViewModel
                {
                    OrderId = orderId,
                    Quantity = 1,
                    MenuItems = new SelectList(new List<object>(), "Id", "DisplayName"),
                    MenuCategories = new Dictionary<string, List<MenuItem>>()
                };
            }

            // Nhóm món ăn theo danh mục
            var menuItemsByCategory = menuItems
                .GroupBy(m => m.Category?.Name ?? "Không phân loại")
                .ToDictionary(g => g.Key, g => g.ToList());

            // Chuyển đổi danh sách món ăn cho SelectList
            var selectListItems = menuItems
                .OrderBy(m => m.Category?.Name)
                .ThenBy(m => m.Name)
                .Select(m => new
                {
                    Id = m.Id,
                    DisplayName = $"{m.Name} ({m.Price:C0})"
                }).ToList();

            var viewModel = new AddItemViewModel
            {
                OrderId = orderId,
                Quantity = 1,
                MenuItems = new SelectList(selectListItems, "Id", "DisplayName"),
                MenuCategories = menuItemsByCategory
            };

            return viewModel;
        }

        public async Task<(bool Success, string Message)> AddItemToOrderAsync(AddItemViewModel model)
        {
            if (model == null)
                return (false, "Model không hợp lệ");

            try
            {
                var order = await _orderRepository.GetByIdAsync(model.OrderId);
                if (order == null)
                    return (false, "Không tìm thấy đơn hàng");

                // Kiểm tra trạng thái đơn hàng
                if (order.Status == "Completed" || order.Status == "Canceled")
                    return (false, $"Không thể thêm món vào đơn hàng có trạng thái {order.Status}");

                var menuItem = await _menuItemRepository.GetByIdAsync(model.MenuItemId);
                if (menuItem == null)
                    return (false, "Không tìm thấy món ăn");

                if (!menuItem.IsAvailable)
                    return (false, "Món ăn này hiện không được phục vụ");

                // Kiểm tra xem món này đã có trong đơn hàng chưa
                var existingItem = await _orderRepository.GetFirstOrDefaultAsync<OrderItem>(i => i.OrderId == model.OrderId && i.MenuItemId == model.MenuItemId);

                // Nếu đã có, tăng số lượng
                if (existingItem != null)
                {
                    existingItem.Quantity += model.Quantity;
                    existingItem.Price = menuItem.Price;
                    existingItem.Subtotal = existingItem.Price * existingItem.Quantity;
                    
                    // Notes là không bắt buộc
                    if (!string.IsNullOrEmpty(model.Notes))
                    {
                        existingItem.SpecialRequests = model.Notes;
                    }

                    _orderRepository.Update(existingItem);
                }
                else
                {
                    // Nếu chưa có, thêm mới
                    var orderItem = new OrderItem
                    {
                        OrderId = model.OrderId,
                        MenuItemId = model.MenuItemId,
                        Quantity = model.Quantity,
                        Price = menuItem.Price,
                        Subtotal = menuItem.Price * model.Quantity,
                        SpecialRequests = model.Notes // Notes là không bắt buộc
                    };

                    await _orderRepository.AddAsync(orderItem);
                }

                // Cập nhật trạng thái đơn hàng
                if (order.Status == "Pending")
                {
                    order.Status = "Processing";
                }
                
                // Cập nhật tổng tiền đơn hàng
                var orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(model.OrderId);
                order.TotalAmount = orderItems.Sum(i => i.Subtotal);
                if (existingItem != null)
                {
                    // Nếu chỉ cập nhật món hiện có, phải thêm phần chênh lệch từ món được cập nhật
                    order.TotalAmount += menuItem.Price * model.Quantity;
                }

                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();

                return (true, $"Đã thêm {menuItem.Name} vào đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                Console.WriteLine($"Lỗi khi thêm món vào đơn hàng: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                
                return (false, $"Lỗi khi thêm món: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> RemoveItemFromOrderAsync(int id)
        {
            try
            {
                var orderItem = await _orderRepository.GetFirstOrDefaultAsync<OrderItem>(i => i.Id == id);

                if (orderItem == null)
                    return (false, "Không tìm thấy mục đơn hàng");
                
                // Lấy đơn hàng để cập nhật tổng tiền
                var order = await _orderRepository.GetByIdAsync(orderItem.OrderId);
                if (order == null)
                    return (false, "Không tìm thấy đơn hàng");

                // Lưu lại giá trị để trừ khỏi tổng tiền
                decimal itemTotal = orderItem.Subtotal;

                _orderRepository.Remove(orderItem);

                // Kiểm tra nếu đơn hàng không còn mục nào
                var remainingItems = await _orderRepository.CountAsync<OrderItem>(i => i.OrderId == orderItem.OrderId && i.Id != id);

                if (remainingItems == 0)
                {
                    // Nếu không còn mục nào, đặt lại trạng thái đơn hàng và reset tổng tiền về 0
                    order.Status = "Pending";
                    order.TotalAmount = 0;
                }
                else
                {
                    // Cập nhật tổng tiền đơn hàng bằng cách trừ giá trị của món ăn bị xóa
                    order.TotalAmount -= itemTotal;
                }

                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();

                return (true, "Xóa món khỏi đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> AddOrderItemAsync(OrderItem item)
        {
            if (item == null)
                return (false, "Mục đơn hàng không hợp lệ");

            try
            {
                await _orderRepository.AddAsync(item);
                await _orderRepository.SaveChangesAsync();
                return (true, "Thêm món thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateOrderItemAsync(OrderItem item)
        {
            if (item == null)
                return (false, "Mục đơn hàng không hợp lệ");

            try
            {
                _orderRepository.Update(item);
                await _orderRepository.SaveChangesAsync();
                return (true, "Cập nhật món thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteOrderItemAsync(int itemId)
        {
            try
            {
                var orderItem = await _orderRepository.GetFirstOrDefaultAsync<OrderItem>(i => i.Id == itemId);
                if (orderItem == null)
                    return (false, "Không tìm thấy mục đơn hàng");

                _orderRepository.Remove(orderItem);
                await _orderRepository.SaveChangesAsync();
                return (true, "Xóa món thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            return await _orderRepository.GetAllAsync<OrderItem>(i => i.OrderId == orderId);
        }

        public async Task<Order> GetOrderForPaymentAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> ProcessPaymentAsync(int orderId, PaymentViewModel payment)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);

                if (order == null)
                    return (false, "Không tìm thấy đơn hàng");

                if (order.Status == "Completed")
                    return (false, "Đơn hàng này đã được thanh toán");

                // Tạo hóa đơn
                var invoice = new Invoice
                {
                    OrderId = orderId,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = payment.PaymentMethod,
                    Amount = await CalculateOrderTotalAsync(orderId),
                    DiscountAmount = payment.DiscountAmount,
                    TotalAmount = await CalculateOrderTotalAsync(orderId) - payment.DiscountAmount,
                    Status = "Paid"
                };

                _orderRepository.Add(invoice);

                // Cập nhật trạng thái đơn hàng
                order.Status = "Completed";
                _orderRepository.Update(order);

                // Giải phóng bàn
                var table = await _tableRepository.GetByIdAsync(order.TableId);
                if (table != null)
                {
                    table.IsOccupied = false;
                    _tableRepository.Update(table);
                }

                await _orderRepository.SaveChangesAsync();

                return (true, "Thanh toán đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, Invoice Invoice)> CreatePaymentAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);

                if (order == null)
                    return (false, "Không tìm thấy đơn hàng", null);

                if (order.Status == "Completed")
                    return (false, "Đơn hàng này đã được thanh toán", null);

                // Tạo hóa đơn
                var invoice = new Invoice
                {
                    OrderId = orderId,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "Cash",
                    Amount = await CalculateOrderTotalAsync(orderId),
                    Status = "Paid"
                };

                _orderRepository.Add(invoice);

                // Cập nhật trạng thái đơn hàng
                order.Status = "Completed";
                _orderRepository.Update(order);

                // Giải phóng bàn
                var table = await _tableRepository.GetByIdAsync(order.TableId);
                if (table != null)
                {
                    table.IsOccupied = false;
                    _tableRepository.Update(table);
                }

                await _orderRepository.SaveChangesAsync();

                return (true, "Thanh toán đơn hàng thành công!", invoice);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> ChangeOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return (false, "Không tìm thấy đơn hàng");

                order.Status = status;
                _orderRepository.Update(order);

                // Nếu trạng thái là hoàn thành, giải phóng bàn
                if (status == "Completed" || status == "Canceled")
                {
                    var table = await _tableRepository.GetByIdAsync(order.TableId);
                    if (table != null)
                    {
                        table.IsOccupied = false;
                        _tableRepository.Update(table);
                    }
                }

                await _orderRepository.SaveChangesAsync();

                return (true, "Cập nhật trạng thái đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<decimal> CalculateOrderTotalAsync(int orderId)
        {
            var orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(orderId);
            return orderItems.Sum(i => i.Subtotal);
        }

        public async Task<List<object>> GetActiveOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync(o => o.Status == "Pending" || o.Status == "Processing");
            
            var activeOrders = orders
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    TableName = o.Table?.Name,
                    UserName = o.User?.UserName,
                    ItemCount = o.OrderItems?.Count ?? 0,
                    Total = o.OrderItems?.Sum(i => i.Subtotal) ?? 0,
                    o.Status
                })
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return activeOrders.Cast<object>().ToList();
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Order>();
                
            return await _orderRepository.GetAllAsync(o => o.EmployeeId == userId);
        }

        public async Task<OrderViewModel> PrepareOrderViewModelAsync(int? orderId, int? tableId)
        {
            // Lấy danh sách các món ăn theo danh mục
            var menuItemsQuery = await _menuItemRepository.GetAllAsync(m => m.IsAvailable);
            var menuItems = await menuItemsQuery
                .Include(m => m.Category)
                .OrderBy(m => m.Category.DisplayOrder)
                .ThenBy(m => m.DisplayOrder)
                .ToListAsync();

            var menuItemsByCategory = menuItems
                .GroupBy(m => m.Category?.Name ?? "Không phân loại")
                .ToDictionary(g => g.Key, g => g.ToList());

            // Chuẩn bị thông tin đơn hàng
            Order order = null;
            List<OrderItem> orderItems = new List<OrderItem>();
            Table table = null;

            if (orderId.HasValue)
            {
                order = await _orderRepository.GetByIdWithDetailsAsync(orderId.Value);

                if (order != null)
                {
                    orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(order.Id);
                    table = await _tableRepository.GetByIdAsync(order.TableId);
                }
            }
            else if (tableId.HasValue)
            {
                table = await _tableRepository.GetByIdAsync(tableId.Value);

                // Tìm đơn hàng đang mở cho bàn này
                if (table != null)
                {
                    var orders = await _orderRepository.GetAllAsync(o => o.TableId == tableId.Value && 
                                   (o.Status == "Pending" || o.Status == "Processing"));
                        
                    order = orders.OrderByDescending(o => o.OrderDate).FirstOrDefault();

                    if (order != null)
                    {
                        orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(order.Id);
                    }
                }
            }
            
            // Chuyển đổi OrderItem thành OrderItemViewModel
            var orderItemViewModels = orderItems.Select(item => new OrderItemViewModel
            {
                Id = item.Id,
                OrderId = item.OrderId,
                MenuItemId = item.MenuItemId,
                ItemName = item.MenuItem?.Name ?? "",
                Quantity = item.Quantity,
                UnitPrice = item.Price,
                SpecialRequests = item.SpecialRequests
            }).ToList();

            // Lấy danh sách bàn trống
            var availableTables = await _tableRepository.GetAvailableTablesAsync();
            
            // Thêm bàn hiện tại nếu có
            if (table != null && !availableTables.Any(t => t.Id == table.Id))
            {
                var tablesWithCurrent = new List<Table>(availableTables);
                tablesWithCurrent.Add(table);
                availableTables = tablesWithCurrent.OrderBy(t => t.Floor.Name).ThenBy(t => t.Name).ToList();
            }

            var viewModel = new OrderViewModel
            {
                Id = order?.Id ?? 0,
                TableId = table?.Id ?? 0,
                TableName = table?.Name,
                CustomerId = order?.CustomerId,
                CustomerName = order?.Customer?.Name,
                EmployeeName = order?.User?.UserName,
                OrderDate = order?.OrderDate ?? DateTime.Now,
                Status = order?.Status ?? "Pending",
                Notes = order?.Notes,
                Total = orderItems.Sum(i => i.Subtotal),
                ItemCount = orderItems.Count,
                Items = orderItemViewModels,
                AvailableTables = availableTables
            };

            return viewModel;
        }

        public async Task<SelectList> GetTablesSelectListAsync(int? selectedTableId = null)
        {
            var tables = await _tableRepository.GetAvailableTablesAsync();

            // Chuyển đổi List<Table> thành danh sách cho SelectList
            var selectListItems = tables.Select(t => new
                {
                    Id = t.Id,
                    DisplayName = $"{t.Name} (Tầng {t.Floor.Name})"
            }).ToList();

            return new SelectList(selectListItems, "Id", "DisplayName", selectedTableId);
        }

        public async Task<SelectList> GetMenuItemsSelectListAsync()
        {
            var menuItems = await _menuItemRepository.GetActiveMenuItemsAsync();
            
            var selectListItems = menuItems
                .OrderBy(m => m.Name)
                .Select(m => new
                {
                    Id = m.Id,
                    DisplayName = $"{m.Name} ({m.Price:C0})"
                }).ToList();

            return new SelectList(selectListItems, "Id", "DisplayName");
        }

        public async Task<Table> GetTableByIdAsync(int tableId)
        {
            return await _tableRepository.GetByIdWithFloorAsync(tableId);
        }

        // Implementación del método para obtener un OrderItem por su ID
        public async Task<OrderItem> GetOrderItemAsync(int id)
        {
            return await _orderRepository.GetOrderItemByIdAsync(id);
        }
    }
} 