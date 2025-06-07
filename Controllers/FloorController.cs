using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class FloorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FloorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Floor
        public async Task<IActionResult> Index()
        {
            var floors = await _context.Floors
                .Include(f => f.Tables)
                .ToListAsync();
            return View(floors);
        }

        // GET: Floor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var floor = await _context.Floors
                .Include(f => f.Tables)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (floor == null)
            {
                return NotFound();
            }

            return View(floor);
        }

        // GET: Floor/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Floor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Description")] Floor floor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(floor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm tầng mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(floor);
        }

        // GET: Floor/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var floor = await _context.Floors.FindAsync(id);
            if (floor == null)
            {
                return NotFound();
            }
            return View(floor);
        }

        // POST: Floor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Floor floor)
        {
            if (id != floor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(floor);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật tầng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FloorExists(floor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(floor);
        }

        // GET: Floor/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var floor = await _context.Floors
                .Include(f => f.Tables)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (floor == null)
            {
                return NotFound();
            }

            return View(floor);
        }

        // POST: Floor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var floor = await _context.Floors
                .Include(f => f.Tables)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (floor == null)
            {
                return NotFound();
            }
            
            // Kiểm tra xem tầng có bàn nào không
            if (floor.Tables.Any())
            {
                ModelState.AddModelError(string.Empty, "Không thể xóa tầng này vì có bàn đang được sử dụng!");
                return View(floor);
            }

            _context.Floors.Remove(floor);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Xóa tầng thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool FloorExists(int id)
        {
            return _context.Floors.Any(e => e.Id == id);
        }
    }
} 