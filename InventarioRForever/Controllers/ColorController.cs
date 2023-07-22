using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioRForever.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace InventarioRForever.Controllers
{

    [Authorize(Roles = "Administrador")]
    public class ColorController : Controller
    {
        private readonly InventarioRfContext _context;
        public int pageSize, skip, recordsTotal;

        public ColorController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: Color
        public async Task<IActionResult> Index()
        {
              return _context.Colors != null ? 
                          View(await _context.Colors.ToListAsync()) :
                          Problem("Entity set 'InventarioRfContext.Colors'  is null.");
        }

        // GET: Color/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Colors == null)
            {
                return NotFound();
            }

            var color = await _context.Colors
                .FirstOrDefaultAsync(m => m.CodColor == id);
            if (color == null)
            {
                return NotFound();
            }

            return View(color);
        }

        // GET: Color/Create
        public IActionResult Create()
        {
			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}

			return View();
        }

        // POST: Color/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodColor,NombreColor")] Color color)
        {
            if (ModelState.IsValid)
            {
                _context.Add(color);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Color Creado!";

				return RedirectToAction(nameof(Create));
            }
            return View(color);
        }

        // GET: Color/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Colors == null)
            {
                return NotFound();
            }

            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return NotFound();
            }
            return View(color);
        }

        // POST: Color/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodColor,NombreColor")] Color color)
        {
            if (id != color.CodColor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(color);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColorExists(color.CodColor))
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
            return View(color);
        }

        // GET: Color/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Colors == null)
            {
                return NotFound();
            }

            var color = await _context.Colors
                .FirstOrDefaultAsync(m => m.CodColor == id);
            if (color == null)
            {
                return NotFound();
            }

            return View(color);
        }

        // POST: Color/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Colors == null)
            {
                return Problem("Entity set 'InventarioRfContext.Colors'  is null.");
            }
            var color = await _context.Colors.FindAsync(id);
            if (color != null)
            {
                _context.Colors.Remove(color);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ColorExists(int id)
        {
          return (_context.Colors?.Any(e => e.CodColor == id)).GetValueOrDefault();
        }

        //Devuelve la lista de colores 
        [HttpPost]
        public ActionResult listadeColores()
        {
            List<Color> colores = new List<Color>();

            try
            {
                recordsTotal = 0;

                IQueryable<Color> query = (from c in _context.Colors
                                            select new Color
                                            {
                                               CodColor = c.CodColor,
                                               NombreColor = c.NombreColor,
                                            });

                recordsTotal = query.Count();
                colores = query.ToList();

                return Json(new { recordsFiltered = recordsTotal, data = colores });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Detalles del color
        [HttpGet]
        public async Task<IActionResult> detalleColor(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle = await _context.Colors
                .Include(m => m.Productos)
                .ThenInclude(m => m.CodInventarioNavigation)
                .FirstOrDefaultAsync(m => m.CodColor == id);

            if (detalle == null)
            {
                return NotFound();
            }

            return PartialView("Details", detalle);
        }

    }
}
