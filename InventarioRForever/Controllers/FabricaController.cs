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
    public class FabricaController : Controller
    {
        private readonly InventarioRfContext _context;
        public int pageSize, skip, recordsTotal;

        public FabricaController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: Fabrica
        public async Task<IActionResult> Index()
        {
              return _context.Fabricas != null ? 
                          View(await _context.Fabricas.ToListAsync()) :
                          Problem("Entity set 'InventarioRfContext.Fabricas'  is null.");
        }

        // GET: Fabrica/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fabricas == null)
            {
                return NotFound();
            }

            var fabrica = await _context.Fabricas
                .FirstOrDefaultAsync(m => m.CodFabrica == id);
            if (fabrica == null)
            {
                return NotFound();
            }

            return View(fabrica);
        }

        // GET: Fabrica/Create
        public IActionResult Create()
        {
			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}
			return View();
        }

        // POST: Fabrica/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodFabrica,NombreFabrica,Telefono,Direccion")] Fabrica fabrica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fabrica);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Fabrica Creada!";

				return RedirectToAction(nameof(Create));
            }
            return View(fabrica);
        }

        // GET: Fabrica/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fabricas == null)
            {
                return NotFound();
            }

            var fabrica = await _context.Fabricas.FindAsync(id);
            if (fabrica == null)
            {
                return NotFound();
            }
            return View(fabrica);
        }

        // POST: Fabrica/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodFabrica,NombreFabrica,Telefono,Direccion")] Fabrica fabrica)
        {
            if (id != fabrica.CodFabrica)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fabrica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FabricaExists(fabrica.CodFabrica))
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
            return View(fabrica);
        }

        // GET: Fabrica/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fabricas == null)
            {
                return NotFound();
            }

            var fabrica = await _context.Fabricas
                .FirstOrDefaultAsync(m => m.CodFabrica == id);
            if (fabrica == null)
            {
                return NotFound();
            }

            return View(fabrica);
        }

        // POST: Fabrica/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fabricas == null)
            {
                return Problem("Entity set 'InventarioRfContext.Fabricas'  is null.");
            }
            var fabrica = await _context.Fabricas.FindAsync(id);
            if (fabrica != null)
            {
                _context.Fabricas.Remove(fabrica);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FabricaExists(int id)
        {
          return (_context.Fabricas?.Any(e => e.CodFabrica == id)).GetValueOrDefault();
        }

        //Devuelve la lista de fabricas 
        [HttpPost]
        public ActionResult listadeFabricas()
        {
            List<Fabrica> fabricas = new List<Fabrica>();

            try
            {
                recordsTotal = 0;

                IQueryable<Fabrica> query = (from f in _context.Fabricas
                                               select new Fabrica
                                               {
                                                   CodFabrica = f.CodFabrica,
                                                   NombreFabrica = f.NombreFabrica,
                                                   Telefono = f.Telefono,
                                                   Direccion = f.Direccion,
                                               });

                recordsTotal = query.Count();
                fabricas = query.ToList();

                return Json(new { recordsFiltered = recordsTotal, data = fabricas });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Detalles de las fabricas
        [HttpGet]
        public async Task<IActionResult> detalleFabrica(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle = await _context.Fabricas
                .Include(m => m.OrdenFabricacions)
                .ThenInclude(m => m.CodProductoNavigation)
                .FirstOrDefaultAsync(m => m.CodFabrica == id);

            if (detalle == null)
            {
                return NotFound();
            }

            return PartialView("Details", detalle);
        }
    }
}
