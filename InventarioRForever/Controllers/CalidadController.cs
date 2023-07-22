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
    public class CalidadController : Controller
    {
        private readonly InventarioRfContext _context;
        public int pageSize, skip, recordsTotal;

        public CalidadController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: Calidad
        public async Task<IActionResult> Index()
        {
              return _context.Calidads != null ? 
                          View(await _context.Calidads.ToListAsync()) :
                          Problem("Entity set 'InventarioRfContext.Calidads'  is null.");
        }

        // GET: Calidad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Calidads == null)
            {
                return NotFound();
            }

            var calidad = await _context.Calidads
                .FirstOrDefaultAsync(m => m.CodCalidad == id);
            if (calidad == null)
            {
                return NotFound();
            }

            return View(calidad);
        }

        // GET: Calidad/Create
        public IActionResult Create()
        {
			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}
			return View();
        }

        // POST: Calidad/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodCalidad,NombreCalidad")] Calidad calidad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(calidad);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Calidad Creada!";
				return RedirectToAction(nameof(Create));
            }
            return View(calidad);
        }

        // GET: Calidad/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Calidads == null)
            {
                return NotFound();
            }

            var calidad = await _context.Calidads.FindAsync(id);
            if (calidad == null)
            {
                return NotFound();
            }
            return View(calidad);
        }

        // POST: Calidad/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodCalidad,NombreCalidad")] Calidad calidad)
        {
            if (id != calidad.CodCalidad)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(calidad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalidadExists(calidad.CodCalidad))
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
            return View(calidad);
        }

        // GET: Calidad/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Calidads == null)
            {
                return NotFound();
            }

            var calidad = await _context.Calidads
                .FirstOrDefaultAsync(m => m.CodCalidad == id);
            if (calidad == null)
            {
                return NotFound();
            }

            return View(calidad);
        }

        // POST: Calidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Calidads == null)
            {
                return Problem("Entity set 'InventarioRfContext.Calidads'  is null.");
            }
            var calidad = await _context.Calidads.FindAsync(id);
            if (calidad != null)
            {
                _context.Calidads.Remove(calidad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CalidadExists(int id)
        {
          return (_context.Calidads?.Any(e => e.CodCalidad == id)).GetValueOrDefault();
        }

        //Devuelve la lista de calidades 
        [HttpPost]
        public ActionResult listadeCalidades()
        {
            List<Calidad> calidades = new List<Calidad>();

            try
            {
                recordsTotal = 0;

                IQueryable<Calidad> query = (from c in _context.Calidads
                                           select new Calidad
                                           {
                                               CodCalidad = c.CodCalidad,
                                               NombreCalidad = c.NombreCalidad,
                                           });

                recordsTotal = query.Count();
                calidades = query.ToList();

                return Json(new { recordsFiltered = recordsTotal, data = calidades });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Detalles de la calidad
        [HttpGet]
        public async Task<IActionResult> detalleCalidad(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle = await _context.Calidads
                .Include(m => m.Productos)
                .ThenInclude(m => m.CodInventarioNavigation)
                .FirstOrDefaultAsync(m => m.CodCalidad == id);

            if (detalle == null)
            {
                return NotFound();
            }

            return PartialView("Details", detalle);
        }
    }
}
