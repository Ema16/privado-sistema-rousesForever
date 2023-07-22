using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioRForever.Models;
using System.Drawing.Printing;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace InventarioRForever.Controllers
{
	[Authorize]
	public class OrdenFabricacionController : Controller
    {
        private readonly InventarioRfContext _context;


		//Atributos para el JSON
		public string draw = "";
		public string start = "";
		public string length = "";
		public string sortColumn = "";
		public string sortColumnDir = "";
		public string searchValue = "";
		public int pageSize, skip, recordsTotal;
		//Atributos para el JSON

		public OrdenFabricacionController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: OrdenFabricacion
        public async Task<IActionResult> Index()
        {
            var inventarioRfContext = _context.OrdenFabricacions.Include(o => o.CodFabricaNavigation).Include(o => o.CodMovimientoNavigation).Include(o => o.CodProductoNavigation);
            return View(await inventarioRfContext.ToListAsync());
        }

        // GET: OrdenFabricacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrdenFabricacions == null)
            {
                return NotFound();
            }

            var ordenFabricacion = await _context.OrdenFabricacions
                .Include(o => o.CodFabricaNavigation)
                .Include(o => o.CodMovimientoNavigation)
                .Include(o => o.CodProductoNavigation)
                .FirstOrDefaultAsync(m => m.CodProductoFabrica == id);
            if (ordenFabricacion == null)
            {
                return NotFound();
            }

            return View(ordenFabricacion);
        }

		// GET: OrdenFabricacion/Create
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult Create()
        {

			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}

			if (TempData["update"] != null)
			{
				ViewBag.update = TempData["update"].ToString();
			}

			ViewData["CodFabrica"] = new SelectList(_context.Fabricas, "CodFabrica", "NombreFabrica");
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento");
            ViewData["CodProducto"] = new SelectList(_context.Productos, "CodProducto", "NombreProducto");
            ViewData["CodMaterial"] = new SelectList(_context.Materials, "CodMaterial", "NombreMaterial");
            return View();
        }

        // POST: OrdenFabricacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Create([Bind("CodProductoFabrica,CantidadFabricacion,FechaFabricacion,EstadoFabricacion,EstadoIngresoInventario,Observacion,CodProducto,CodFabrica,CodMovimiento,Materiales,Cantidades")] OrdenFabricacion ordenFabricacion)
        {


			int cantidad = 0;
            int cont = 0;
			DateTime fecha = DateTime.Now;

            if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodFabricaNavigation");
                ModelState.Remove("CodMovimientoNavigation");
                ModelState.Remove("CodProductoNavigation");
            }

            ordenFabricacion.FechaFabricacion = fecha;

            if (ordenFabricacion.Materiales == null)
            {
                return NotFound();
            }
			if (ordenFabricacion.Cantidades == null)
			{
				return NotFound();
			}
			//Retornamos el registro del producto para ver a que inventario corresponde
			var producto = _context.Productos.Where(m => m.CodProducto == ordenFabricacion.CodProducto).FirstOrDefault();
            if (producto == null)
            {
                return NotFound();
            }
            //Retornamos el registro del inventario correspondiente al producto seleccionado en la entrada de mercancia
            var inventario = _context.Inventarios.Where(m => m.CodInventario == producto.CodInventario).FirstOrDefault();

            if (inventario == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
				var invtemp = inventario.Stock;
				Movimiento movimiento = new Movimiento();
                movimiento.CantidadMovimiento = invtemp;
                movimiento.StockMenosInventario = ordenFabricacion.CantidadFabricacion; 
                movimiento.CantidadStockDisponible = inventario.Stock; 

                if (ordenFabricacion.EstadoFabricacion == "Producida")
                {
					
					movimiento.StockMenosInventario = invtemp; //Stock anterior disponible
					movimiento.CantidadStockDisponible = ordenFabricacion.CantidadFabricacion + inventario.Stock; 
                    inventario.Stock = inventario.Stock + ordenFabricacion.CantidadFabricacion;
                    inventario.FechaInventario = fecha;

                    ordenFabricacion.EstadoIngresoInventario = "Ingresado";
                }
                
                movimiento.FechaMovimiento = fecha;
                movimiento.CodTipoMovimiento = 2;

                _context.Add(movimiento);
                await _context.SaveChangesAsync();

                ordenFabricacion.CodMovimiento = movimiento.CodMovimiento;

                _context.Add(ordenFabricacion);
                await _context.SaveChangesAsync();


                foreach (var n in ordenFabricacion.Materiales)
                {
                    //Cantidad de materia utilizado
                    cantidad = ordenFabricacion.Cantidades[cont];
                    //Retornamos el registro del material para ver a que inventario corresponde
                    var material = _context.Materials.Where(m => m.CodMaterial == n).FirstOrDefault();

					if (material == null)
					{
						return NotFound();
					}

					//Retornamos el registro del inventario correspondiente al producto seleccionado en la entrada de mercancia
					var inventarioM = _context.Inventarios.Where(m => m.CodInventario == material.CodInventario).FirstOrDefault();

                    if (inventarioM == null)
                    {
                        return NotFound();
                    }

                    inventarioM.Stock = inventarioM.Stock - cantidad;
                    inventarioM.FechaInventario = fecha;
                    _context.Update(inventarioM);
                    await _context.SaveChangesAsync();

					FabricacionMaterial fabricacionMaterial = new FabricacionMaterial();
                    fabricacionMaterial.CodProductoFabrica = ordenFabricacion.CodProductoFabrica;
                    fabricacionMaterial.CodMaterial = n;
                    fabricacionMaterial.CantidaMaterial = cantidad;
                    _context.Add(fabricacionMaterial);
                    await _context.SaveChangesAsync();
                    cont++;
                    cantidad = 0;
                }
				TempData["mensaje"] = "Orden creada!";
				return RedirectToAction(nameof(Create));
            }
            ViewData["CodFabrica"] = new SelectList(_context.Fabricas, "CodFabrica", "CodFabrica", ordenFabricacion.CodFabrica);
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", ordenFabricacion.CodMovimiento);
            ViewData["CodProducto"] = new SelectList(_context.Productos, "CodProducto", "CodProducto", ordenFabricacion.CodProducto);
            return View(ordenFabricacion);
        }

        //Imprimir Factura
        [AcceptVerbs("GET", "POST")]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> UpdateOrden(int? id)
        {
            DateTime fecha = DateTime.Now;

            try
            {
                //Retornamos el registro del producto para ver a que inventario corresponde
                var orden = _context.OrdenFabricacions.Where(m => m.CodProductoFabrica == id).FirstOrDefault();          

                if (orden == null)
                {
                    return NotFound();
                }

                orden.EstadoFabricacion = "Producida";
                orden.EstadoIngresoInventario = "Ingresado"; 

				_context.Update(orden);
                await _context.SaveChangesAsync();


                var producto = _context.Productos.Where(m => m.CodProducto == orden.CodProducto).FirstOrDefault();

                if (producto == null)
                {
                    return NotFound();
                }

                var inventario = _context.Inventarios.Where(m => m.CodInventario == producto.CodInventario).FirstOrDefault();

                if (inventario == null)
                {
                    return NotFound();
                }

                var movimiento = _context.Movimientos.Where(m => m.CodMovimiento == orden.CodMovimiento).FirstOrDefault();
               
                if (movimiento == null)
                {
                    return NotFound();
                }

                var invtemp = inventario.Stock;

                movimiento.StockMenosInventario = invtemp; //Stock anterior
                movimiento.CantidadStockDisponible = orden.CantidadFabricacion + inventario.Stock;
                movimiento.CantidadMovimiento = orden.CantidadFabricacion;
               // 

                movimiento.FechaMovimiento = fecha;
                _context.Update(movimiento);
                await _context.SaveChangesAsync();

                inventario.Stock = inventario.Stock + orden.CantidadFabricacion;
                inventario.FechaInventario = fecha;
                _context.Update(inventario);

				TempData["update"] = "Orden Actualizada!";

				await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                    throw;
            }


            return RedirectToAction(nameof(Create));
        }


        // GET: OrdenFabricacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrdenFabricacions == null)
            {
                return NotFound();
            }

            var ordenFabricacion = await _context.OrdenFabricacions.FindAsync(id);
            if (ordenFabricacion == null)
            {
                return NotFound();
            }
            ViewData["CodFabrica"] = new SelectList(_context.Fabricas, "CodFabrica", "CodFabrica", ordenFabricacion.CodFabrica);
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", ordenFabricacion.CodMovimiento);
            ViewData["CodProducto"] = new SelectList(_context.Productos, "CodProducto", "CodProducto", ordenFabricacion.CodProducto);
            return View(ordenFabricacion);
        }

        // POST: OrdenFabricacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Edit(int id, [Bind("CodProductoFabrica,CantidadFabricacion,FechaFabricacion,EstadoFabricacion,EstadoIngresoInventario,Observacion,CodProducto,CodFabrica,CodMovimiento")] OrdenFabricacion ordenFabricacion)
        {
            if (id != ordenFabricacion.CodProductoFabrica)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodFabricaNavigation");
                ModelState.Remove("CodMovimientoNavigation");
                ModelState.Remove("CodProductoNavigation");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime fecha = DateTime.Now;

                    //Retornamos el registro del producto para ver a que inventario corresponde
                    var producto = _context.Productos.Where(m => m.CodProducto == ordenFabricacion.CodProducto).FirstOrDefault();

                    if (producto == null)
                    {
                        return NotFound();
                    }

                    //Retornamos el registro del inventario correspondiente al producto seleccionado en la venta
                    var inventario = _context.Inventarios.Where(m => m.CodInventario == producto.CodInventario).FirstOrDefault();

                    if (inventario == null)
                    {
                        return NotFound();
                    }

                    var movimiento = _context.Movimientos.Where(m => m.CodMovimiento == ordenFabricacion.CodMovimiento).FirstOrDefault();
                    if (movimiento == null)
                    {
                        return NotFound();
                    }
                    movimiento.StockMenosInventario = ordenFabricacion.CantidadFabricacion + inventario.Stock;
                    movimiento.CantidadStockDisponible = ordenFabricacion.CantidadFabricacion + inventario.Stock;
                    _context.Update(movimiento);
                    await _context.SaveChangesAsync();

                    ordenFabricacion.EstadoFabricacion = "Producida";
                    ordenFabricacion.EstadoIngresoInventario = "Ingresado";

                    inventario.Stock = inventario.Stock + ordenFabricacion.CantidadFabricacion;
                    inventario.FechaInventario = fecha;
                    _context.Update(inventario);
                    await _context.SaveChangesAsync();

                    _context.Update(ordenFabricacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdenFabricacionExists(ordenFabricacion.CodProductoFabrica))
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
            ViewData["CodFabrica"] = new SelectList(_context.Fabricas, "CodFabrica", "CodFabrica", ordenFabricacion.CodFabrica);
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", ordenFabricacion.CodMovimiento);
            ViewData["CodProducto"] = new SelectList(_context.Productos, "CodProducto", "CodProducto", ordenFabricacion.CodProducto);
            return View(ordenFabricacion);
        }

        // GET: OrdenFabricacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrdenFabricacions == null)
            {
                return NotFound();
            }

            var ordenFabricacion = await _context.OrdenFabricacions
                .Include(o => o.CodFabricaNavigation)
                .Include(o => o.CodMovimientoNavigation)
                .Include(o => o.CodProductoNavigation)
                .FirstOrDefaultAsync(m => m.CodProductoFabrica == id);
            if (ordenFabricacion == null)
            {
                return NotFound();
            }

            return View(ordenFabricacion);
        }

        // POST: OrdenFabricacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrdenFabricacions == null)
            {
                return Problem("Entity set 'InventarioRfContext.OrdenFabricacions'  is null.");
            }
            var ordenFabricacion = await _context.OrdenFabricacions.FindAsync(id);
            if (ordenFabricacion != null)
            {
                _context.OrdenFabricacions.Remove(ordenFabricacion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

		//Obtener datos de un producto
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult ObtenerDatosTablaMaterial(int idMaterial)
		{
			// Obtén los datos de tu origen de datos (por ejemplo, una base de datos)
			List<listaMercancia> lista;
			lista = (from d in _context.Materials
                     join t in _context.TipoMaterials on d.CodTipoMaterial equals t.CodTipoMaterial
                     join i in _context.Inventarios on d.CodInventario equals i.CodInventario
					 where d.CodMaterial == idMaterial
					 select new listaMercancia
					 {
                         CodMaterial = d.CodMaterial,
						 NombreMaterial = d.NombreMaterial,
						 NombreTipoMaterial = t.NombreTipoMaterial,
                         stock = i.Stock
					 }).ToList();

			// Devuelve los datos en formato JSON
			return Json(lista);
		}




		//Metodo que devuelve la lista de fabricaciones por calidad y color
		[HttpPost]
		[Authorize(Roles = "Administrador,Ventas")]
		public ActionResult listaOrdenesFabricacion()
		{
			List<OrdenFabricacion> ordenFabricacions = new List<OrdenFabricacion>();

			try
			{
			/*	var draw = Request.Form["draw"].FirstOrDefault();
				var start = Request.Form["start"].FirstOrDefault();
				var length = Request.Form["length"].FirstOrDefault();
				var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
				var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
				var searchValue = Request.Form["search[value]"].FirstOrDefault();

				pageSize = length != null ? Convert.ToInt32(length) : 0;
				skip = start != null ? Convert.ToInt32(start) : 0;*/
				recordsTotal = 0;

				IQueryable<OrdenFabricacion> query = (from e in _context.OrdenFabricacions
											 join m in _context.Productos on e.CodProducto equals m.CodProducto
                                             join c in _context.Calidads on m.CodCalidad equals c.CodCalidad
                                             join co in _context.Colors on m.CodColor equals co.CodColor
											 select new OrdenFabricacion
											 {
												 CodProductoFabrica = e.CodProductoFabrica,
                                                 NombreProducto = m.NombreProducto,
                                                 FechaFabricacion = e.FechaFabricacion,
                                                 CantidadFabricacion = e.CantidadFabricacion,
                                                 EstadoFabricacion = e.EstadoFabricacion,
                                                 EstadoIngresoInventario = e.EstadoIngresoInventario,
                                                 NombreCalidad = c.NombreCalidad,
                                                 NombreColor = co.NombreColor,
											 });


				/*if (searchValue != "")
				{
					query = query.Where(d => d.NombreProducto.Contains(searchValue));
				}


				if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
				{
					//	query = query.OrderBy(sortColumn + " " + sortColumnDirection);
				}*/

				recordsTotal = query.Count();
				ordenFabricacions = query.ToList();

				return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = ordenFabricacions });


			}
			catch (Exception ex)
			{
				throw;
			}

		}

		//Registro del usuario
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> DetalleOrden(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

            var detalle = await _context.OrdenFabricacions
                .Include(m => m.CodProductoNavigation)
                .Include(m => m.CodFabricaNavigation)
                .Include(m => m.FabricacionMaterials)
                .ThenInclude (m => m.CodMaterialNavigation)
                .FirstOrDefaultAsync(m => m.CodProductoFabrica == id);


			if (detalle == null)
			{
				return NotFound();
			}

			return PartialView("Details", detalle);
		}

		private bool OrdenFabricacionExists(int id)
        {
          return (_context.OrdenFabricacions?.Any(e => e.CodProductoFabrica == id)).GetValueOrDefault();
        }
    }
}
