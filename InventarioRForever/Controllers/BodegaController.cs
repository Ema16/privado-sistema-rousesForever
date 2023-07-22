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
    public class BodegaController : Controller
    {
        private readonly InventarioRfContext _context;
		public int pageSize, skip, recordsTotal;

		public BodegaController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: Bodega
        public async Task<IActionResult> Index()
        {
            var inventarioRfContext = _context.Bodegas.Include(b => b.CodMovimientoNavigation);
            return View(await inventarioRfContext.ToListAsync());
        }

        // GET: Bodega/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bodegas == null)
            {
                return NotFound();
            }

            var bodega = await _context.Bodegas
                .Include(b => b.CodMovimientoNavigation)
                .FirstOrDefaultAsync(m => m.CodBodega == id);
            if (bodega == null)
            {
                return NotFound();
            }

            return View(bodega);
        }

        // GET: Bodega/Create
        public IActionResult Create()
        {
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento");

			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}
			return View();
        }

        // POST: Bodega/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodBodega,Isla,NombreBodega,Seccion,Nivel,Contenedor,Capacidad,CodMovimiento")] Bodega bodega)
        {

            if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodMovimientoNavigation");
            }

            if (ModelState.IsValid)
            {

                //Error técnico registrar un movimiento de bodega que sea el id 1
                bodega.CodMovimiento = 1;
                _context.Add(bodega);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Bodega Creada!";

				return RedirectToAction(nameof(Create));
            }
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", bodega.CodMovimiento);
            return View(bodega);
        }

        // GET: Bodega/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bodegas == null)
            {
                return NotFound();
            }

            var bodega = await _context.Bodegas.FindAsync(id);
            if (bodega == null)
            {
                return NotFound();
            }
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", bodega.CodMovimiento);
            return View(bodega);
        }

        // POST: Bodega/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodBodega,Isla,NombreBodega,Seccion,Nivel,Contenedor,Capacidad,CodMovimiento")] Bodega bodega)
        {
            if (id != bodega.CodBodega)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bodega);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BodegaExists(bodega.CodBodega))
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
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", bodega.CodMovimiento);
            return View(bodega);
        }

        // GET: Bodega/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bodegas == null)
            {
                return NotFound();
            }

            var bodega = await _context.Bodegas
                .Include(b => b.CodMovimientoNavigation)
                .FirstOrDefaultAsync(m => m.CodBodega == id);
            if (bodega == null)
            {
                return NotFound();
            }

            return View(bodega);
        }

        // POST: Bodega/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bodegas == null)
            {
                return Problem("Entity set 'InventarioRfContext.Bodegas'  is null.");
            }
            var bodega = await _context.Bodegas.FindAsync(id);
            if (bodega != null)
            {
                _context.Bodegas.Remove(bodega);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BodegaExists(int id)
        {
          return (_context.Bodegas?.Any(e => e.CodBodega == id)).GetValueOrDefault();
        }


		//Devuelve la lista de Bodegas 
		[HttpPost]
		public ActionResult listadeBodegas()
		{
			List<Bodega> bodegas = new List<Bodega>();

			try
			{
				recordsTotal = 0;

				IQueryable<Bodega> query = (from b in _context.Bodegas
											 select new Bodega
											 {
												 CodBodega = b.CodBodega,
                                                 Isla = b.Isla,
                                                 NombreBodega = b.NombreBodega,
                                                 Seccion = b.Seccion,
                                                 Nivel = b.Nivel,
                                                 Contenedor = b.Contenedor,
                                                 Capacidad = b.Capacidad,
											 });

				recordsTotal = query.Count();
				bodegas = query.ToList();

				return Json(new { recordsFiltered = recordsTotal, data = bodegas });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		//Detalles de las fabricas
		[HttpGet]
		public async Task<IActionResult> detalleBodega(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var detalle = await _context.Bodegas
                .Include(m => m.DevolucionProductos)
                .ThenInclude(m => m.CodVentaNavigation)
                .ThenInclude(m => m.CodProductoNavigation)
				.FirstOrDefaultAsync(m => m.CodBodega == id);

			if (detalle == null)
			{
				return NotFound();
			}

			return PartialView("Details", detalle);
		}
	}
}
