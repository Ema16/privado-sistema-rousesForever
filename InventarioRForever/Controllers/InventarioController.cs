using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventarioRForever.Models;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace InventarioRForever.Controllers
{

	[Authorize(Roles = "Administrador,Ventas")]
	public class InventarioController : Controller
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

        public InventarioController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: Inventario
        public async Task<IActionResult> Index()
        {
              return _context.Inventarios != null ? 
                          View(await _context.Inventarios.ToListAsync()) :
                          Problem("Entity set 'InventarioRfContext.Inventarios'  is null.");
        }

        // GET: Inventario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(m => m.CodInventario == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // GET: Inventario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inventario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodInventario,Stock,StockDevuelto,FechaInventario")] Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventario);
        }

        // GET: Inventario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }
            return View(inventario);
        }

        // POST: Inventario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodInventario,Stock,StockDevuelto,FechaInventario")] Inventario inventario)
        {
            if (id != inventario.CodInventario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventarioExists(inventario.CodInventario))
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
            return View(inventario);
        }

        // GET: Inventario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inventarios == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(m => m.CodInventario == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // POST: Inventario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Inventarios == null)
            {
                return Problem("Entity set 'InventarioRfContext.Inventarios'  is null.");
            }
            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario != null)
            {
                _context.Inventarios.Remove(inventario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventarioExists(int id)
        {
          return (_context.Inventarios?.Any(e => e.CodInventario == id)).GetValueOrDefault();
        }


        [HttpPost]
        public ActionResult InventarioMaterial()
        {
            List<ViewModel> inventarios = new List<ViewModel>();

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

                IQueryable<ViewModel> query = (from e in _context.Inventarios
                                                join d in _context.Materials on e.CodInventario equals d.CodInventario
                                                select new ViewModel
                                                {
                                                    CodInventario = e.CodInventario,
                                                    Stock = e.Stock,
                                                    stockDevuelto = e.StockDevuelto,
                                                    FechaInventario = e.FechaInventario,
                                                    NombreMaterial = d.NombreMaterial,
                                                });
               
                if (searchValue != "")
                {
                     query = query.Where(d => d.NombreMaterial.Contains(searchValue));
                }


                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    //query = query.OrderBy(item => item.CodInventario);

                   //  query = query.OrderBy(sortColumn + " " +sortColumnDirection); //esto ya no funciona en net core 6
                }

                recordsTotal = query.Count();
                inventarios = query.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = inventarios });
            }
            catch (Exception ex)
            {
                throw;
            }

            
        }

        [HttpPost]
        public ActionResult InventarioProducto()
        {


            return View();
        }


        //Devuelve la cantidad de stock disponible para cada producto y cuanto queda
		[HttpPost]
		public ActionResult stockDisponible()
		{
			List<ViewModel> productos = new List<ViewModel>();

			try
			{
				recordsTotal = 0;

				IQueryable<ViewModel> query = (from e in _context.Inventarios
											   join d in _context.Productos on e.CodInventario equals d.CodInventario
                                               join ca in _context.Calidads on d.CodCalidad equals ca.CodCalidad
                                               join co in _context.Colors on d.CodColor equals co.CodColor
											   select new ViewModel
											   {
												   CodProducto = d.CodProducto,
												   NombreProducto = d.NombreProducto,
                                                   EstadoProducto = d.EstadoProducto,
                                                   NombreCalidad = ca.NombreCalidad,
                                                   NombreColor = co.NombreColor,
                                                   Stock = e.Stock,

											   });

				recordsTotal = query.Count();
				productos = query.ToList();

				return Json(new {recordsFiltered = recordsTotal, data = productos });
			}
			catch (Exception ex)
			{
				throw;
			}


		}

		//Totalida de Ventas
		[HttpPost]
		public ActionResult totalVentas()
		{
			List<Factura> totalidad = new List<Factura>();

			try
			{
				recordsTotal = 0;

				IQueryable<Factura> query = (from e in _context.Facturas
											   select new Factura
											   {
                                                   CodFactura = e.CodFactura,
                                                   FechaFactura = e.FechaFactura,
                                                   MetodoPago = e.MetodoPago,
                                                   ImporteTotal = e.ImporteTotal,
											   });

				recordsTotal = query.Count();
				totalidad = query.ToList();

				return Json(new { recordsFiltered = recordsTotal, data = totalidad });
			}
			catch (Exception ex)
			{
				throw;
			}


		}

        // Metodo GET para las ventas de los usuarios
        // Lista de ventas
        public IActionResult ventaClientes()
        {
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios.OrderByDescending(u => u.CodUsuario).Where(m => m.CodRolUsuario == 3), "CodUsuario", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ventaClientes([Bind("Fecha1,Fecha2,CodUsuario")] Reporte reporte)
        {
            List<Pedido> totalidad = new List<Pedido>();

            try
            {


                var detalle = await _context.Pedidos
                    .Include(m => m.CodUsuarioNavigation)
                    .Include(m => m.CodFacturaNavigation)
                    .Include(m => m.CodMetodoPagoNavigation)
                    .Where(m => m.FechaPedido >= reporte.Fecha1).Where(m => m.FechaPedido <= reporte.Fecha2).Where(m => m.CodUsuario==reporte.CodUsuario).ToListAsync();

                return new ViewAsPdf("reporteClientes", detalle)
                {
                    //FileName = "prueba.pdf",
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    PageSize = Rotativa.AspNetCore.Options.Size.Letter
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        // Metodo GET para las ventas realizadas
        // Lista de ventas
        public IActionResult Venta()
		{
			return View();
		}

		// Metodo GET para la totalidad de ventas
		// Lista de ventas
		public IActionResult totalidadVentas()
		{
			return View();
		}


        //Devuelve la lista de ventas de los usuarios 
        [HttpPost]
        public ActionResult listaVentasUsuarios()
        {
            List<Pedidos> ventas = new List<Pedidos>();

            try
            {
                recordsTotal = 0;

                IQueryable<Pedidos> query = (from b in _context.Pedidos
                                             join u in _context.Usuarios on b.CodUsuario equals u.CodUsuario
                                             join f in _context.Facturas on b.CodFactura equals f.CodFactura
                                             join m in _context.MetodoPagos on b.CodMetodoPago equals m.CodMetodoPago
                                             where u.CodRolUsuario == 3
                                             select new Pedidos
                                             {
                                                 CodPedido = b.CodPedido,
                                                 Nombre1 = u.Nombre1 + " " + u.Nombre2 + " " + u.OtrosNombres + " " + u.Apellido1 + " " + u.Apellido2,
                                                 FechaPedido = b.FechaPedido,
                                                 EstadoPedido = b.EstadoPedido,
                                                 MetodoPago1 = m.MetodoPago1,
                                                 ImporteTotal = f.ImporteTotal,

                                             }); 

                recordsTotal = query.Count();
                ventas = query.ToList();

                return Json(new { recordsFiltered = recordsTotal, data = ventas });
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //Vista del stock disponible y cuanto queda
        public IActionResult stockMaterial()
        {
            ViewData["CodMaterial"] = new SelectList(_context.Materials.OrderByDescending(u => u.CodMaterial), "CodMaterial", "NombreMaterial");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> stockMaterial([Bind("CodMaterial")] Reporte reporte)
        {

            try
            {
                if (reporte.CodMaterial == 0)
                {
                    var detalle = await _context.Materials
                    .Include(m => m.CodCategoriaNavigation)
                    .Include(m => m.CodTipoMaterialNavigation)
                    .Include(m => m.CodInventarioNavigation)
                    .ToListAsync();
                    return new ViewAsPdf("reporteMaterial", detalle)
                    {
                        //FileName = "prueba.pdf",
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        PageSize = Rotativa.AspNetCore.Options.Size.Letter
                    };
                }
                else
                {
                    var detalle = await _context.Materials
                    .Include(m => m.CodCategoriaNavigation)
                    .Include(m => m.CodTipoMaterialNavigation)
                    .Include(m => m.CodInventarioNavigation)
                    .Where(m => m.CodMaterial == reporte.CodMaterial).ToListAsync();
                    return new ViewAsPdf("reporteMaterial", detalle)
                    {
                        //FileName = "prueba.pdf",
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        PageSize = Rotativa.AspNetCore.Options.Size.Letter
                    };
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: Inventario/Create
        //Vista del stock disponible y cuanto queda
        public IActionResult Stock()
		{
            ViewData["CodProducto"] = new SelectList(_context.Productos.OrderByDescending(u => u.CodProducto), "CodProducto", "NombreProducto");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Stock([Bind("CodProducto")] Reporte reporte)
        {

            try
            {
                if (reporte.CodProducto==0)
                {
                    var detalle = await _context.Productos
                    .Include(m => m.CodColorNavigation)
                    .Include(m => m.CodCalidadNavigation)
                    .Include(m => m.CodCategoriaNavigation)
                    .Include(m => m.CodInventarioNavigation)
                    .ToListAsync();
                    return new ViewAsPdf("reporteStock", detalle)
                    {
                        //FileName = "prueba.pdf",
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        PageSize = Rotativa.AspNetCore.Options.Size.Letter
                    };
                }
                else
                {
                    var detalle = await _context.Productos
                    .Include(m => m.CodColorNavigation)
                    .Include(m => m.CodCalidadNavigation)
                    .Include(m => m.CodCategoriaNavigation)
                    .Include(m => m.CodInventarioNavigation)
                    .Where(m => m.CodProducto == reporte.CodProducto).ToListAsync();
                    return new ViewAsPdf("reporteStock", detalle)
                    {
                        //FileName = "prueba.pdf",
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        PageSize = Rotativa.AspNetCore.Options.Size.Letter
                    };
                }

                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Metodo GET que devuelve sobre las rosas en produccion por calidad y colores
        //como de igual manera vista que devulete el producto que se obtiene durante el día
        // GET: Inventario/Create
        public IActionResult Fabricacion()
        {
            return View();
        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> totalidadVentas([Bind("Fecha1,Fecha2")] Reporte reporte)
		{
			List<Factura> totalidad = new List<Factura>();

			try
			{

			/*	IQueryable<Factura> query = (from e in _context.Facturas
											 where e.FechaFactura >= reporte.Fecha1 && e.FechaFactura <= reporte.Fecha2
											 select new Factura
											 {
												 CodFactura = e.CodFactura,
												 FechaFactura = e.FechaFactura,
												 MetodoPago = e.MetodoPago,
												 ImporteTotal = e.ImporteTotal,
											 });

                totalidad = query.ToList();*/

                var detalle = await _context.Facturas
                    .Where(m => m.FechaFactura >= reporte.Fecha1).Where(m => m.FechaFactura <= reporte.Fecha2).ToListAsync();

				return new ViewAsPdf("generarReporte", detalle)
				{
				  //FileName = "prueba.pdf",
					PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
					PageSize = Rotativa.AspNetCore.Options.Size.Letter
				};
			}
			catch (Exception ex)
			{
				throw;
			}

			/*
			if (codigoPedido == null)
			{
				return NotFound();
			}
			var detalle = await _context.Pedidos
				.Include(p => p.CodUsuarioNavigation)
				.Include(p => p.CodFacturaNavigation)
				.Include(p => p.CodMetodoPagoNavigation)
				.Include(p => p.Venta)
				.ThenInclude(p => p.CodProductoNavigation)
				.FirstOrDefaultAsync(m => m.CodPedido == codigoPedido);


			if (detalle == null)
			{
				return NotFound();
			}

			return new ViewAsPdf("ImprimirFactura", detalle)
			{
				//  FileName = "prueba.pdf",
				PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
				PageSize = Rotativa.AspNetCore.Options.Size.Letter
			};*/
			return View();
		}


	}
}
