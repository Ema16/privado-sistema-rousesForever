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
    public class SucursalController : Controller
    {
        private readonly InventarioRfContext _context;
		public int pageSize, skip, recordsTotal;
		public SucursalController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: Sucursal
        public async Task<IActionResult> Index()
        {
              return _context.Sucursals != null ? 
                          View(await _context.Sucursals.ToListAsync()) :
                          Problem("Entity set 'InventarioRfContext.Sucursals'  is null.");
        }

        // GET: Sucursal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sucursals == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursals
                .FirstOrDefaultAsync(m => m.CodSucursal == id);
            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }

        // GET: Sucursal/Create
        public IActionResult Create()
        {
			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}

			return View();
        }

        // POST: Sucursal/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodSucursal,NombreSucursal,Direccion,Municipio,Departamento,Telefono,Observacion")] Sucursal sucursal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sucursal);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Sucursal Creada!";

				return RedirectToAction(nameof(Create));
            }
            return View(sucursal);
        }

        // GET: Sucursal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sucursals == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursals.FindAsync(id);
            if (sucursal == null)
            {
                return NotFound();
            }
            return View(sucursal);
        }

        // POST: Sucursal/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodSucursal,NombreSucursal,Direccion,Municipio,Departamento,Telefono,Observacion")] Sucursal sucursal)
        {
            if (id != sucursal.CodSucursal)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sucursal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SucursalExists(sucursal.CodSucursal))
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
            return View(sucursal);
        }

        // GET: Sucursal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sucursals == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursals
                .FirstOrDefaultAsync(m => m.CodSucursal == id);
            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }

        // POST: Sucursal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sucursals == null)
            {
                return Problem("Entity set 'InventarioRfContext.Sucursals'  is null.");
            }
            var sucursal = await _context.Sucursals.FindAsync(id);
            if (sucursal != null)
            {
                _context.Sucursals.Remove(sucursal);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SucursalExists(int id)
        {
          return (_context.Sucursals?.Any(e => e.CodSucursal == id)).GetValueOrDefault();
        }


		//Devuelve la lista de sucursales 
		[HttpPost]
		public ActionResult listadeSucursales()
		{
			List<Sucursal> sucursals = new List<Sucursal>();

			try
			{
				recordsTotal = 0;

				IQueryable<Sucursal> query = (from s in _context.Sucursals
											   select new Sucursal
											   {
												   CodSucursal = s.CodSucursal,
                                                   NombreSucursal = s.NombreSucursal,
                                                   Direccion = s.Direccion,
                                                   Municipio = s.Municipio,
                                                   Departamento = s.Departamento,
                                                   Telefono = s.Telefono,
                                                   Observacion = s.Observacion,

											   });

				recordsTotal = query.Count();
				sucursals = query.ToList();

				return Json(new { recordsFiltered = recordsTotal, data = sucursals });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		//Detalles del pedido
		[HttpGet]
		public async Task<IActionResult> detalleSucursal(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var detalle = await _context.Sucursals
				.FirstOrDefaultAsync(m => m.CodSucursal == id);

			if (detalle == null)
			{
				return NotFound();
			}

			return PartialView("Details", detalle);
		}
	}
}
