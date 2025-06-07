// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Hàm hiển thị thông báo toast
function showToast(message, type = 'success') {
    // Kiểm tra nếu bootstrap chưa được load
    if (typeof bootstrap === 'undefined') {
        console.warn('Bootstrap chưa được tải. Hiển thị alert thay thế.');
        alert(message);
        return;
    }

    try {
        // Kiểm tra nếu đã có toast container, nếu không thì tạo mới
        if (!document.querySelector('.toast-container')) {
            const toastContainer = document.createElement('div');
            toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            document.body.appendChild(toastContainer);
        }

        // Tạo ID duy nhất cho toast
        const id = 'toast-' + Date.now();
        
        // Xác định class dựa trên loại thông báo
        let bgClass;
        switch (type) {
            case 'success': bgClass = 'bg-success'; break;
            case 'error': bgClass = 'bg-danger'; break;
            case 'warning': bgClass = 'bg-warning'; break;
            case 'info': bgClass = 'bg-info'; break;
            default: bgClass = 'bg-primary';
        }
        
        // Tạo HTML cho toast
        const toastHTML = `
            <div id="${id}" class="toast align-items-center ${bgClass} text-white border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;
        
        // Thêm toast vào container
        document.querySelector('.toast-container').innerHTML += toastHTML;
        
        // Khởi tạo toast
        const toastElement = document.getElementById(id);
        const toast = new bootstrap.Toast(toastElement, { autohide: true, delay: 3000 });
        toast.show();
        
        // Xóa toast sau khi đã ẩn
        toastElement.addEventListener('hidden.bs.toast', function () {
            if (document.getElementById(id)) {
                toastElement.remove();
            }
        });
    } catch (error) {
        console.error('Không thể hiển thị toast:', error);
        alert(message);
    }
}

// Hiệu ứng hiển thị và ẩn thông báo
document.addEventListener('DOMContentLoaded', function() {
    // Hiển thị thông báo thành công
    const successMessage = document.getElementById('successMessage');
    if (successMessage) {
        showToast(successMessage.textContent, 'success');
        // Ẩn div gốc sau khi đã hiển thị toast
        successMessage.style.display = 'none';
    }
    
    // Hiển thị thông báo lỗi
    const errorMessage = document.getElementById('errorMessage');
    if (errorMessage) {
        showToast(errorMessage.textContent, 'error');
        // Ẩn div gốc sau khi đã hiển thị toast
        errorMessage.style.display = 'none';
    }
    
    // Hiệu ứng cho các card
    const cards = document.querySelectorAll('.card');
    cards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.classList.add('shadow-hover');
        });
        
        card.addEventListener('mouseleave', function() {
            this.classList.remove('shadow-hover');
        });
    });
    
    // Thêm active class cho menu hiện tại
    const currentLocation = location.pathname;
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    
    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentLocation) {
            link.classList.add('active');
            
            // Nếu link nằm trong dropdown, cũng highlight dropdown
            const dropdownParent = link.closest('.dropdown');
            if (dropdownParent) {
                const dropdownToggle = dropdownParent.querySelector('.dropdown-toggle');
                if (dropdownToggle) {
                    dropdownToggle.classList.add('active');
                }
            }
        }
    });
    
    // Hiệu ứng cho các nút
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('mousedown', function() {
            this.classList.add('btn-pressed');
        });
        
        button.addEventListener('mouseup', function() {
            this.classList.remove('btn-pressed');
        });
        
        button.addEventListener('mouseleave', function() {
            this.classList.remove('btn-pressed');
        });
    });
});

// Format tiền tệ
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', { 
        style: 'currency', 
        currency: 'VND',
        maximumFractionDigits: 0
    }).format(amount);
}

// Format ngày giờ
function formatDateTime(dateTimeString) {
    const date = new Date(dateTimeString);
    return new Intl.DateTimeFormat('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    }).format(date);
}

// Format ngày
function formatDate(dateString) {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    }).format(date);
}

// Hàm hiển thị modal xác nhận
function showConfirmModal(title, message, confirmCallback, cancelCallback = null) {
    // Kiểm tra và tạo modal nếu chưa tồn tại
    let confirmModal = document.getElementById('confirmModal');
    
    if (!confirmModal) {
        const modalHTML = `
            <div class="modal fade" id="confirmModal" tabindex="-1" aria-labelledby="confirmModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="confirmModalLabel">Xác nhận</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body" id="confirmModalBody">
                            Bạn có chắc chắn muốn thực hiện hành động này?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" id="confirmModalCancel">Hủy</button>
                            <button type="button" class="btn btn-primary" id="confirmModalConfirm">Xác nhận</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        document.body.insertAdjacentHTML('beforeend', modalHTML);
        confirmModal = document.getElementById('confirmModal');
    }
    
    // Cập nhật nội dung
    document.getElementById('confirmModalLabel').textContent = title;
    document.getElementById('confirmModalBody').textContent = message;
    
    // Modal object
    const modal = new bootstrap.Modal(confirmModal);
    
    // Xử lý sự kiện nút xác nhận
    const confirmBtn = document.getElementById('confirmModalConfirm');
    // Xóa tất cả event listeners cũ
    const newConfirmBtn = confirmBtn.cloneNode(true);
    confirmBtn.parentNode.replaceChild(newConfirmBtn, confirmBtn);
    
    // Thêm event listener mới
    newConfirmBtn.addEventListener('click', function() {
        if (typeof confirmCallback === 'function') {
            confirmCallback();
        }
        modal.hide();
    });
    
    // Xử lý sự kiện nút hủy
    const cancelBtn = document.getElementById('confirmModalCancel');
    // Xóa tất cả event listeners cũ
    const newCancelBtn = cancelBtn.cloneNode(true);
    cancelBtn.parentNode.replaceChild(newCancelBtn, cancelBtn);
    
    // Thêm event listener mới
    newCancelBtn.addEventListener('click', function() {
        if (typeof cancelCallback === 'function') {
            cancelCallback();
        }
    });
    
    // Hiển thị modal
    modal.show();
}

// Kiểm tra trạng thái đặt bàn và cập nhật UI tương ứng
function updateReservationStatus() {
    const statusElements = document.querySelectorAll('.status-indicator');
    
    statusElements.forEach(element => {
        const status = element.dataset.status?.toLowerCase();
        let statusClass = '';
        let icon = '';
        
        switch (status) {
            case 'pending':
                statusClass = 'status-pending';
                icon = '<i class="fas fa-clock me-1"></i>';
                break;
            case 'confirmed':
                statusClass = 'status-confirmed';
                icon = '<i class="fas fa-check-circle me-1"></i>';
                break;
            case 'completed':
                statusClass = 'status-completed';
                icon = '<i class="fas fa-check-double me-1"></i>';
                break;
            case 'cancelled':
                statusClass = 'status-cancelled';
                icon = '<i class="fas fa-times-circle me-1"></i>';
                break;
            default:
                statusClass = '';
                icon = '<i class="fas fa-question-circle me-1"></i>';
        }
        
        if (statusClass) {
            element.className = `status-badge ${statusClass}`;
            element.innerHTML = `${icon}${element.textContent}`;
        }
    });
}

// Hiệu ứng hover cho các phần tử bảng
function setupTableRowEffects() {
    const tableRows = document.querySelectorAll('.table-hover tbody tr');
    
    tableRows.forEach(row => {
        row.addEventListener('mouseenter', function() {
            this.classList.add('row-highlight');
        });
        
        row.addEventListener('mouseleave', function() {
            this.classList.remove('row-highlight');
        });
    });
}

// Khởi chạy các hàm khi trang được tải
document.addEventListener('DOMContentLoaded', function() {
    updateReservationStatus();
    setupTableRowEffects();
});
