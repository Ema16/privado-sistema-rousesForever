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

	[Authorize]
	public class DevolucionMaterialController : Controller
    {
        private readonly InventarioRfContext _context;
		public int pageSize, skip, recordsTotal;
		public DevolucionMaterialController(InventarioRfContext context)
        {
            _context = context;
        }

		// GET: DevolucionMaterial
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Index()
        {

			if (TempData["devolucion"] != null)
			{
				ViewBag.devolucion = TempData["devolucion"].ToString();
			}

			var inventarioRfContext = _context.DevolucionMaterials.Include(d => d.CodMovimientoNavigation).Include(d => d.CodProveedorMaterialNavigation).Include(d => d.CodUsuarioNavigation);
            return View(await inventarioRfContext.ToListAsync());
        }

        // GET: DevolucionMaterial/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DevolucionMaterials == null)
            {
                return NotFound();
            }

            var devolucionMaterial = await _context.DevolucionMaterials
                .Include(d => d.CodMovimientoNavigation)
                .Include(d => d.CodProveedorMaterialNavigation)
                .Include(d => d.CodUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.CodDevolucionMaterial == id);
            if (devolucionMaterial == null)
            {
                return NotFound();
            }

            return View(devolucionMaterial);
        }

		// GET: DevolucionMaterial/Create
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult Create()
        {
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento");
           // ViewData["CodProveedorMaterial"] = new SelectList(_context.RecepcionMercancia, "CodProveedorMaterial", "CantidadProducida");
			ViewData["CodMaterial"] = new SelectList(_context.Materials, "CodMaterial", "NombreMaterial");
			ViewData["CodUsuario"] = new SelectList(_context.Usuarios, "CodUsuario", "FullName");
            return View();
        }

        // POST: DevolucionMaterial/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Create([Bind("CodDevolucionMaterial,FechaDevolucion,Motivo,TipoDevolucion,CantidadDevueltaMaterial,CodUsuario,CodProveedorMaterial,CodMovimiento")] DevolucionMaterial devolucionMaterial)
        {
            DateTime fecha = DateTime.Now;

            if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodMovimientoNavigation");
                ModelState.Remove("CodProveedorMaterialNavigation");
                ModelState.Remove("CodUsuarioNavigation");
            }

            //Retornamos el registro de la recepcion de mercancia para saber a que material corresponde
            var recepcion = _context.RecepcionMercancia.Include(m => m.CodMaterialNavigation).Include(m => m.CodProveedorNavigation).FirstOrDefault(m => m.CodProveedorMaterial == devolucionMaterial.CodProveedorMaterial);

            if (recepcion == null)
            {
                return NotFound();
            }

            var inventario = _context.Inventarios.Where(m => m.CodInventario == recepcion.CodMaterial).FirstOrDefault();
            //Retornamos el registro del materia para ver a que inventario corresponde
            //var producto = _context.Materials.Where(m => m.CodMaterial == devolucionMaterial).FirstOrDefault();
            if (inventario == null)
            {
                return NotFound();
            }

            if (inventario.Stock>=devolucionMaterial.CantidadDevueltaMaterial) {

                devolucionMaterial.CodUsuario = 4;

                var invtemp = inventario.Stock;

                inventario.Stock = inventario.Stock - devolucionMaterial.CantidadDevueltaMaterial;
                inventario.FechaInventario = fecha;

                Movimiento movimiento = new Movimiento();
                movimiento.CantidadMovimiento = devolucionMaterial.CantidadDevueltaMaterial;
                movimiento.StockMenosInventario = invtemp; //StockMenosInventario es el stock disponible anterior

                _context.Update(inventario);
                await _context.SaveChangesAsync();

                
                movimiento.CantidadStockDisponible = inventario.Stock;
                movimiento.FechaMovimiento = fecha;
                movimiento.CodTipoMovimiento = 3;
                _context.Add(movimiento);
                await _context.SaveChangesAsync();

                devolucionMaterial.CodMovimiento = movimiento.CodMovimiento;

                if (ModelState.IsValid)
                {
                    _context.Add(devolucionMaterial);
                    await _context.SaveChangesAsync();

					TempData["devolucion"] = "Devolución creada!";

					return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", devolucionMaterial.CodMovimiento);
            ViewData["CodProveedorMaterial"] = new SelectList(_context.RecepcionMercancia, "CodProveedorMaterial", "CodProveedorMaterial", devolucionMaterial.CodProveedorMaterial);
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios, "CodUsuario", "CodUsuario", devolucionMaterial.CodUsuario);
            return View(devolucionMaterial);
        }

        // GET: DevolucionMaterial/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DevolucionMaterials == null)
            {
                return NotFound();
            }

            var devolucionMaterial = await _context.DevolucionMaterials.FindAsync(id);
            if (devolucionMaterial == null)
            {
                return NotFound();
            }
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", devolucionMaterial.CodMovimiento);
            ViewData["CodProveedorMaterial"] = new SelectList(_context.RecepcionMercancia, "CodProveedorMaterial", "CodProveedorMaterial", devolucionMaterial.CodProveedorMaterial);
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios, "CodUsuario", "CodUsuario", devolucionMaterial.CodUsuario);
            return View(devolucionMaterial);
        }

        // POST: DevolucionMaterial/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodDevolucionMaterial,FechaDevolucion,Motivo,TipoDevolucion,CantidadDevueltaMaterial,CodUsuario,CodProveedorMaterial,CodMovimiento")] DevolucionMaterial devolucionMaterial)
        {
            if (id != devolucionMaterial.CodDevolucionMaterial)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(devolucionMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DevolucionMaterialExists(devolucionMaterial.CodDevolucionMaterial))
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
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", devolucionMaterial.CodMovimiento);
            ViewData["CodProveedorMaterial"] = new SelectList(_context.RecepcionMercancia, "CodProveedorMaterial", "CodProveedorMaterial", devolucionMaterial.CodProveedorMaterial);
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios, "CodUsuario", "CodUsuario", devolucionMaterial.CodUsuario);
            return View(devolucionMaterial);
        }

        // GET: DevolucionMaterial/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DevolucionMaterials == null)
            {
                return NotFound();
            }

            var devolucionMaterial = await _context.DevolucionMaterials
                .Include(d => d.CodMovimientoNavigation)
                .Include(d => d.CodProveedorMaterialNavigation)
                .Include(d => d.CodUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.CodDevolucionMaterial == id);
            if (devolucionMaterial == null)
            {
                return NotFound();
            }

            return View(devolucionMaterial);
        }

        // POST: DevolucionMaterial/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DevolucionMaterials == null)
            {
                return Problem("Entity set 'InventarioRfContext.DevolucionMaterials'  is null.");
            }
            var devolucionMaterial = await _context.DevolucionMaterials.FindAsync(id);
            if (devolucionMaterial != null)
            {
                _context.DevolucionMaterials.Remove(devolucionMaterial);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


		//Obtener datos de un recepcion de mercancía
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult ObtenerDatosMercancia(int codMaterial)
		{
			// Obtén los datos de tu origen de datos (por ejemplo, una base de datos)
			List<listaMercancia> lista;
			lista = (from d in _context.Materials
					 join r in _context.RecepcionMercancia on d.CodMaterial equals r.CodMaterial
                     join i in _context.Inventarios on d.CodInventario equals i.CodInventario
					 where d.CodMaterial == codMaterial
					 select new listaMercancia
                     {
                         CodProveedorMaterial = r.CodProveedorMaterial,
                         CodMaterial = d.CodMaterial,
                         NombreMaterial = d.NombreMaterial,
                         CantidadProducida = r.CantidadProducida, //Fecha de recepción de mercancia
                         stock = i.Stock,
                     }).ToList();

			// Devuelve los datos en formato JSON
			return Json(lista);
		}

		private bool DevolucionMaterialExists(int id)
        {
          return (_context.DevolucionMaterials?.Any(e => e.CodDevolucionMaterial == id)).GetValueOrDefault();
        }

		//Devuelve la lista de devoluciones de materiales 
		[HttpPost]
		[Authorize(Roles = "Administrador,Ventas")]
		public ActionResult listaDevMateriales()
		{
            List<DevolucionMaterial> devolucionesM = new List<DevolucionMaterial>();

			try
			{
				recordsTotal = 0;

				IQueryable<DevolucionMaterial> query = (from d in _context.DevolucionMaterials
                                                        join r in _context.RecepcionMercancia on d.CodProveedorMaterial equals r.CodProveedorMaterial
                                                        join m in _context.Materials on r.CodMaterial equals m.CodMaterial
                                                        join p in _context.Proveedors on r.CodProveedor equals p.CodProveedor
											 select new DevolucionMaterial
											 {
												 CodDevolucionMaterial = d.CodDevolucionMaterial,
                                                 NombreMaterial = m.NombreMaterial,
                                                 FechaDevolucion = d.FechaDevolucion,
                                                 Motivo = d.Motivo,
                                                 TipoDevolucion = d.TipoDevolucion,
                                                 CantidadDevueltaMaterial = d.CantidadDevueltaMaterial,
                                                 NombreProveedor = p.Nombre,
											 });

				recordsTotal = query.Count();
				devolucionesM = query.ToList();

				return Json(new { recordsFiltered = recordsTotal, data = devolucionesM });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		//Detalles de la devolucion de material
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> detalleDevMaterial(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var detalle = await _context.DevolucionMaterials
                .Include(m => m.CodProveedorMaterialNavigation)
                .ThenInclude(m => m.CodMaterialNavigation)
                .Include(m => m.CodUsuarioNavigation)
				.FirstOrDefaultAsync(m => m.CodDevolucionMaterial == id);

			if (detalle == null)
			{
				return NotFound();
			}

			return PartialView("Details", detalle);
		}

	}
}
