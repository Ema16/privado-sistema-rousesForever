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
    public class DescuentoController : Controller
    {
        private readonly InventarioRfContext _context;
        public int pageSize, skip, recordsTotal;
        public DescuentoController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: Descuento
        public async Task<IActionResult> Index()
        {
            var inventarioRfContext = _context.Descuentos.Include(d => d.CodProductoNavigation);
            return View(await inventarioRfContext.ToListAsync());
        }

        // GET: Descuento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Descuentos == null)
            {
                return NotFound();
            }

            var descuento = await _context.Descuentos
                .Include(d => d.CodProductoNavigation)
                .FirstOrDefaultAsync(m => m.CodProductoDescuento == id);
            if (descuento == null)
            {
                return NotFound();
            }

            return View(descuento);
        }

        // GET: Descuento/Create
        public IActionResult Create()
        {
			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}
			ViewData["CodProducto"] = new SelectList(_context.Productos, "CodProducto", "NombreProducto");
            return View();
        }

        // POST: Descuento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodProductoDescuento,Descuento1,FechaDescuento,EstadoDescuento,CodProducto")] Descuento descuento)
        {

            if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodProductoNavigation");
            }

            if (ModelState.IsValid)
            {
                _context.Add(descuento);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Descuento Creado!";
				return RedirectToAction(nameof(Create));
            }
            ViewData["CodProducto"] = new SelectList(_context.Productos, "CodProducto", "CodProducto", descuento.CodProducto);
            return View(descuento);
        }

        // GET: Descuento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Descuentos == null)
            {
                return NotFound();
            }

            var descuento = await _context.Descuentos.FindAsync(id);
            if (descuento == null)
            {
                return NotFound();
            }
            ViewData["CodProducto"] = new SelectList(_context.Productos, "CodProducto", "CodProducto", descuento.CodProducto);
            return View(descuento);
        }

        // POST: Descuento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodProductoDescuento,Descuento1,FechaDescuento,EstadoDescuento,CodProducto")] Descuento descuento)
        {
            if (id != descuento.CodProductoDescuento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(descuento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DescuentoExists(descuento.CodProductoDescuento))
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
            ViewData["CodProducto"] = new SelectList(_context.Productos, "CodProducto", "CodProducto", descuento.CodProducto);
            return View(descuento);
        }

        // GET: Descuento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Descuentos == null)
            {
                return NotFound();
            }

            var descuento = await _context.Descuentos
                .Include(d => d.CodProductoNavigation)
                .FirstOrDefaultAsync(m => m.CodProductoDescuento == id);
            if (descuento == null)
            {
                return NotFound();
            }

            return View(descuento);
        }

        // POST: Descuento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Descuentos == null)
            {
                return Problem("Entity set 'InventarioRfContext.Descuentos'  is null.");
            }
            var descuento = await _context.Descuentos.FindAsync(id);
            if (descuento != null)
            {
                _context.Descuentos.Remove(descuento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DescuentoExists(int id)
        {
          return (_context.Descuentos?.Any(e => e.CodProductoDescuento == id)).GetValueOrDefault();
        }

        //Devuelve la lista de descuentos 
        [HttpPost]
        public ActionResult listadeDescuentos()
        {
            List<Descuento> calidades = new List<Descuento>();

            try
            {
                recordsTotal = 0;

                IQueryable<Descuento> query = (from d in _context.Descuentos
                                               join p in _context.Productos on d.CodProducto equals p.CodProducto
                                             select new Descuento
                                             {
                                                 CodProductoDescuento = d.CodProductoDescuento,
                                                 Descuento1 = d.Descuento1,
                                                 FechaDescuento = d.FechaDescuento,
                                                 EstadoDescuento = d.EstadoDescuento,
                                                 CodProducto = d.CodProducto,
                                                 NombreProducto = p.NombreProducto,
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

        //Detalles del descuento
        [HttpGet]
        public async Task<IActionResult> detalleDescuento(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle = await _context.Productos
                .Include(m => m.CodInventarioNavigation)
                .Include(m => m.Descuentos)
                .FirstOrDefaultAsync(m => m.CodProducto == id);

            if (detalle == null)
            {
                return NotFound();
            }

            return PartialView("Details", detalle);
        }


    }
}
