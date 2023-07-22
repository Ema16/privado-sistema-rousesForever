using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioRForever.Models;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace InventarioRForever.Controllers
{

	[Authorize]
	public class RecepcionMercanciaController : Controller
    {
		//Atributos para el JSON
		public string draw = "";
		public string start = "";
		public string length = "";
		public string sortColumn = "";
		public string sortColumnDir = "";
		public string searchValue = "";
		public int pageSize, skip, recordsTotal;
		//Atributos para el JSON

		private readonly InventarioRfContext _context;

        public RecepcionMercanciaController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: RecepcionMercancia
        public async Task<IActionResult> Index()
        {
            var inventarioRfContext = _context.RecepcionMercancia.Include(r => r.CodMaterialNavigation).Include(r => r.CodProveedorNavigation);
            return View(await inventarioRfContext.ToListAsync());
        }

        // GET: RecepcionMercancia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RecepcionMercancia == null)
            {
                return NotFound();
            }

            var recepcionMercancium = await _context.RecepcionMercancia
                .Include(r => r.CodMaterialNavigation)
                .Include(r => r.CodProveedorNavigation)
                .FirstOrDefaultAsync(m => m.CodProveedorMaterial == id);
            if (recepcionMercancium == null)
            {
                return NotFound();
            }

            return View(recepcionMercancium);
        }

		// GET: RecepcionMercancia/Create
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult Create()
        {
			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}

			ViewData["CodMaterial"] = new SelectList(_context.Materials, "CodMaterial", "NombreMaterial");
            ViewData["CodProveedor"] = new SelectList(_context.Proveedors, "CodProveedor", "Nombre");
            return View();
        }

        // POST: RecepcionMercancia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Create([Bind("CodProveedorMaterial,Cantidad,CantidadProducida,CodMaterial,CodProveedor")] RecepcionMercancium recepcionMercancium)
        {
            DateTime fecha = DateTime.Now;

            if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodMaterialNavigation");
                ModelState.Remove("CodProveedorNavigation");
            }

            var material = _context.Materials.Where(x => x.CodMaterial == recepcionMercancium.CodMaterial).FirstOrDefault();
            if (material == null)
            {
                /*TempData["Message"] = "Seleccione un material";
                if (TempData["Message"]!=null)
                {
					ViewBag.materialNull = TempData["Message"].ToString();
				}
				ViewData["CodMaterial"] = new SelectList(_context.Materials, "CodMaterial", "CodMaterial", recepcionMercancium.CodMaterial);
				ViewData["CodProveedor"] = new SelectList(_context.Proveedors, "CodProveedor", "CodProveedor", recepcionMercancium.CodProveedor);
				return View(recepcionMercancium);*/
                return NotFound();
			}
            var inventario = _context.Inventarios.Where(x => x.CodInventario == material.CodInventario).FirstOrDefault();
            if (inventario == null)
            {
                return NotFound();
            }

            inventario.Stock = recepcionMercancium.Cantidad + inventario.Stock;
            inventario.FechaInventario = fecha;

			_context.Update(inventario);
            await _context.SaveChangesAsync();

            if (ModelState.IsValid)
            {
                _context.Add(recepcionMercancium);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Material ingresado!";

				return RedirectToAction(nameof(Create));
            }
            ViewData["CodMaterial"] = new SelectList(_context.Materials, "CodMaterial", "CodMaterial", recepcionMercancium.CodMaterial);
            ViewData["CodProveedor"] = new SelectList(_context.Proveedors, "CodProveedor", "CodProveedor", recepcionMercancium.CodProveedor);
            return View(recepcionMercancium);
        }

        // GET: RecepcionMercancia/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RecepcionMercancia == null)
            {
                return NotFound();
            }

            var recepcionMercancium = await _context.RecepcionMercancia.FindAsync(id);
            if (recepcionMercancium == null)
            {
                return NotFound();
            }
            ViewData["CodMaterial"] = new SelectList(_context.Materials, "CodMaterial", "CodMaterial", recepcionMercancium.CodMaterial);
            ViewData["CodProveedor"] = new SelectList(_context.Proveedors, "CodProveedor", "CodProveedor", recepcionMercancium.CodProveedor);
            return View(recepcionMercancium);
        }

        // POST: RecepcionMercancia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodProveedorMaterial,Cantidad,CantidadProducida,CodMaterial,CodProveedor")] RecepcionMercancium recepcionMercancium)
        {
            if (id != recepcionMercancium.CodProveedorMaterial)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recepcionMercancium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecepcionMercanciumExists(recepcionMercancium.CodProveedorMaterial))
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
            ViewData["CodMaterial"] = new SelectList(_context.Materials, "CodMaterial", "CodMaterial", recepcionMercancium.CodMaterial);
            ViewData["CodProveedor"] = new SelectList(_context.Proveedors, "CodProveedor", "CodProveedor", recepcionMercancium.CodProveedor);
            return View(recepcionMercancium);
        }

        // GET: RecepcionMercancia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RecepcionMercancia == null)
            {
                return NotFound();
            }

            var recepcionMercancium = await _context.RecepcionMercancia
                .Include(r => r.CodMaterialNavigation)
                .Include(r => r.CodProveedorNavigation)
                .FirstOrDefaultAsync(m => m.CodProveedorMaterial == id);
            if (recepcionMercancium == null)
            {
                return NotFound();
            }

            return View(recepcionMercancium);
        }

        // POST: RecepcionMercancia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RecepcionMercancia == null)
            {
                return Problem("Entity set 'InventarioRfContext.RecepcionMercancia'  is null.");
            }
            var recepcionMercancium = await _context.RecepcionMercancia.FindAsync(id);
            if (recepcionMercancium != null)
            {
                _context.RecepcionMercancia.Remove(recepcionMercancium);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool RecepcionMercanciumExists(int id)
        {
          return (_context.RecepcionMercancia?.Any(e => e.CodProveedorMaterial == id)).GetValueOrDefault();
        }

		//Metodo que devuelve la lista de entradas de mercancía
		[HttpPost]
		[Authorize(Roles = "Administrador,Ventas")]
		public ActionResult listaEntradaMercancia()
		{
			List<listaMercancia> recepcion = new List<listaMercancia>();

			try
			{
				var draw = Request.Form["draw"].FirstOrDefault();
				var start = Request.Form["start"].FirstOrDefault();
				var length = Request.Form["length"].FirstOrDefault();
				var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
				var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
				var searchValue = Request.Form["search[value]"].FirstOrDefault();

				pageSize = length != null ? Convert.ToInt32(length) : 0;
				skip = start != null ? Convert.ToInt32(start) : 0;
				recordsTotal = 0;

				IQueryable<listaMercancia> query = (from e in _context.RecepcionMercancia
											 join m in _context.Materials on e.CodMaterial equals m.CodMaterial
											 join l in _context.Proveedors on e.CodProveedor equals l.CodProveedor
                                             join i in _context.TipoMaterials on m.CodTipoMaterial equals i.CodTipoMaterial
											 select new listaMercancia
											 {
												 CodProveedorMaterial = e.CodProveedorMaterial,
												 Cantidad = e.Cantidad,
                                                 CantidadProducida = e.CantidadProducida,
												 NombreMaterial = m.NombreMaterial,
                                                 NombreTipoMaterial = i.NombreTipoMaterial,
                                                 NombreProveedor = l.Nombre,
											 });


				if (searchValue != "")
				{
					query = query.Where(d => d.NombreMaterial.Contains(searchValue));
				}


				if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
				{
					//	query = query.OrderBy(sortColumn + " " + sortColumnDirection);
				}

				recordsTotal = query.Count();
				recepcion = query.Skip(skip).Take(pageSize).ToList();

				return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = recepcion });


			}
			catch (Exception ex)
			{
				throw;
			}


		}

		//Obtener datos de un entrada de mercancía
		//Detalles del pedido
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> detalleMercancia(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var detalle = await _context.RecepcionMercancia
                .Include(m => m.CodMaterialNavigation)
                .ThenInclude(m => m.CodInventarioNavigation)
                .Include(m => m.CodProveedorNavigation)
				.FirstOrDefaultAsync(m => m.CodProveedorMaterial == id);

			if (detalle == null)
			{
				return NotFound();
			}

			return PartialView("Details", detalle);
		}
	}
}
