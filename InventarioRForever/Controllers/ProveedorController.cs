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
    public class ProveedorController : Controller
    {
        private readonly InventarioRfContext _context;
        public int pageSize, skip, recordsTotal;

        public ProveedorController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: Proveedor
        public async Task<IActionResult> Index()
        {
              return _context.Proveedors != null ? 
                          View(await _context.Proveedors.ToListAsync()) :
                          Problem("Entity set 'InventarioRfContext.Proveedors'  is null.");
        }

        // GET: Proveedor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Proveedors == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors
                .FirstOrDefaultAsync(m => m.CodProveedor == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // GET: Proveedor/Create
        public IActionResult Create()
        {
			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}

			return View();
        }

        // POST: Proveedor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodProveedor,Nombre,Direccion,Telefono")] Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(proveedor);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Proveedor creado con exito";

				return RedirectToAction(nameof(Create));
            }
            return View(proveedor);
        }

        // GET: Proveedor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Proveedors == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            return View(proveedor);
        }

        // POST: Proveedor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodProveedor,Nombre,Direccion,Telefono")] Proveedor proveedor)
        {
            if (id != proveedor.CodProveedor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedorExists(proveedor.CodProveedor))
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
            return View(proveedor);
        }

        // GET: Proveedor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Proveedors == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors
                .FirstOrDefaultAsync(m => m.CodProveedor == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: Proveedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Proveedors == null)
            {
                return Problem("Entity set 'InventarioRfContext.Proveedors'  is null.");
            }
            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor != null)
            {
                _context.Proveedors.Remove(proveedor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
          return (_context.Proveedors?.Any(e => e.CodProveedor == id)).GetValueOrDefault();
        }


        //Devuelve la lista de proveedores 
        [HttpPost]
        public ActionResult listadeProveedores()
        {
            List<Proveedor> proveedores = new List<Proveedor>();

            try
            {
                recordsTotal = 0;

                IQueryable<Proveedor> query = (from p in _context.Proveedors
                                              select new Proveedor
                                              {
                                                  CodProveedor = p.CodProveedor,
                                                  Nombre = p.Nombre,
                                                  Direccion = p.Direccion,
                                                  Telefono = p.Telefono,

                                              });

                recordsTotal = query.Count();
                proveedores = query.ToList();

                return Json(new { recordsFiltered = recordsTotal, data = proveedores });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Detalles de los proveedores
        [HttpGet]
        public async Task<IActionResult> detalleProveedor(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle = await _context.Proveedors
                .Include(m => m.RecepcionMercancia)
                .ThenInclude(m => m.CodMaterialNavigation)
                .FirstOrDefaultAsync(m => m.CodProveedor == id);

            if (detalle == null)
            {
                return NotFound();
            }

            return PartialView("Details", detalle);
        }
    }
}
