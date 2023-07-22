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
    public class MetodoPagoController : Controller
    {
        private readonly InventarioRfContext _context;
        public int pageSize, skip, recordsTotal;
        public MetodoPagoController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: MetodoPago
        public async Task<IActionResult> Index()
        {
              return _context.MetodoPagos != null ? 
                          View(await _context.MetodoPagos.ToListAsync()) :
                          Problem("Entity set 'InventarioRfContext.MetodoPagos'  is null.");
        }

        // GET: MetodoPago/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MetodoPagos == null)
            {
                return NotFound();
            }

            var metodoPago = await _context.MetodoPagos
                .FirstOrDefaultAsync(m => m.CodMetodoPago == id);
            if (metodoPago == null)
            {
                return NotFound();
            }

            return View(metodoPago);
        }

        // GET: MetodoPago/Create
        public IActionResult Create()
        {
			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}
			return View();
        }

        // POST: MetodoPago/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodMetodoPago,MetodoPago1,Observaciones")] MetodoPago metodoPago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(metodoPago);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Metodo Creada!";

				return RedirectToAction(nameof(Create));
            }
            return View(metodoPago);
        }

        // GET: MetodoPago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MetodoPagos == null)
            {
                return NotFound();
            }

            var metodoPago = await _context.MetodoPagos.FindAsync(id);
            if (metodoPago == null)
            {
                return NotFound();
            }
            return View(metodoPago);
        }

        // POST: MetodoPago/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodMetodoPago,MetodoPago1,Observaciones")] MetodoPago metodoPago)
        {
            if (id != metodoPago.CodMetodoPago)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(metodoPago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MetodoPagoExists(metodoPago.CodMetodoPago))
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
            return View(metodoPago);
        }

        // GET: MetodoPago/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MetodoPagos == null)
            {
                return NotFound();
            }

            var metodoPago = await _context.MetodoPagos
                .FirstOrDefaultAsync(m => m.CodMetodoPago == id);
            if (metodoPago == null)
            {
                return NotFound();
            }

            return View(metodoPago);
        }

        // POST: MetodoPago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MetodoPagos == null)
            {
                return Problem("Entity set 'InventarioRfContext.MetodoPagos'  is null.");
            }
            var metodoPago = await _context.MetodoPagos.FindAsync(id);
            if (metodoPago != null)
            {
                _context.MetodoPagos.Remove(metodoPago);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MetodoPagoExists(int id)
        {
          return (_context.MetodoPagos?.Any(e => e.CodMetodoPago == id)).GetValueOrDefault();
        }


        //Devuelve la lista de metodos de pago 
        [HttpPost]
        public ActionResult listadeMetodosPago()
        {
            List<MetodoPago> metodosdepago = new List<MetodoPago>();

            try
            {
                recordsTotal = 0;

                IQueryable<MetodoPago> query = (from m in _context.MetodoPagos
                                            select new MetodoPago
                                            {
                                                CodMetodoPago = m.CodMetodoPago,
                                                MetodoPago1 = m.MetodoPago1,
                                                Observaciones = m.Observaciones,
                                            });

                recordsTotal = query.Count();
                metodosdepago = query.ToList();

                return Json(new { recordsFiltered = recordsTotal, data = metodosdepago });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Detalles de las fabricas
        [HttpGet]
        public async Task<IActionResult> detalleMetodosPago(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle = await _context.MetodoPagos
                .Include(m => m.Pedidos)
                .ThenInclude (m => m.CodFacturaNavigation)
                .FirstOrDefaultAsync(m => m.CodMetodoPago == id);

            if (detalle == null)
            {
                return NotFound();
            }

            return PartialView("Details", detalle);
        }
    }
}
