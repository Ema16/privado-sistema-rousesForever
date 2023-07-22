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
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace InventarioRForever.Controllers
{
	[Authorize]
	public class PedidoController : Controller
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

		public PedidoController(InventarioRfContext context)
        {
            _context = context;
        }

		// GET: Pedido
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Index()
        {
            if (TempData["update"] != null)
            {
                ViewBag.update = TempData["update"].ToString();
            }
            var inventarioRfContext = _context.Pedidos.Include(p => p.CodFacturaNavigation).Include(p => p.CodMetodoPagoNavigation).Include(p => p.CodUsuarioNavigation);
            return View(await inventarioRfContext.ToListAsync());
        }

        // GET: Pedido/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.CodFacturaNavigation)
                .Include(p => p.CodMetodoPagoNavigation)
                .Include(p => p.CodUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.CodPedido == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

		// GET: Pedido/Create
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult Create()
        {
			if (TempData["pedido"] != null)
			{
				ViewBag.pedido = TempData["pedido"].ToString();
			}

			if (TempData["usuario"] != null)
			{
				ViewBag.usuario = TempData["usuario"].ToString();
			}


			ViewData["CodFactura"] = new SelectList(_context.Facturas, "CodFactura", "CodFactura");
            ViewData["CodMetodoPago"] = new SelectList(_context.MetodoPagos, "CodMetodoPago", "MetodoPago1");
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios.OrderByDescending(u => u.CodUsuario).Where(m => m.CodRolUsuario==3), "CodUsuario", "FullName");
            ViewData["CodProducto"] = new SelectList(_context.Productos, "CodProducto", "NombreProducto");
            return View();
        }

        // POST: Pedido/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Create([Bind("CodPedido,FechaPedido,EstadoPedido,CodUsuario,CodFactura,CodMetodoPago,Productos,Cantidades")] Pedido pedido)
        {
            int? importeParcial = 0;
            int? importeTotal = 0;
            int cont = 0;
            int cantidad = 0;

            if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodUsuarioNavigation");
                ModelState.Remove("CodMetodoPagoNavigation");
            }

            if (pedido.Productos == null)
            {
                return NotFound();
            }

			if (pedido.Cantidades == null)
			{
				return NotFound();
			}

			DateTime thisDay = DateTime.Now;

			
			foreach (var i in pedido.Productos)
            {
                cantidad = pedido.Cantidades[cont];
					//Retrornamos el registro del producto para ver a que inventario corresponde
					var producto = _context.Productos.Where(m => m.CodProducto == i).FirstOrDefault();

                   

					if (producto == null)
					{
						return NotFound();
					}
					//Calculos
					importeParcial = producto.PrecioVenta * cantidad;
					importeTotal = importeTotal + importeParcial;

					//El parametro de metodo_de_pago de la tabla Venta sera el total parcial, no creamos una tabla para eso

					//Retornamos el registro del inventario correspondiente al producto seleccionado en la venta
					var inventario = _context.Inventarios.Where(m => m.CodInventario == producto.CodInventario).FirstOrDefault();


					if (inventario == null)
					{
						return NotFound();
					}
				    var tempinv = inventario.Stock;

				if (inventario.Stock >= cantidad)
					{
						Movimiento movimiento = new Movimiento();
						movimiento.CantidadMovimiento = cantidad; //Pendiente de asignar la cantidad de la venta
						movimiento.StockMenosInventario = tempinv; //Stock anterior disponible


					    //Asignamos al nuevo inventario la cantidad menos el stock vendido
					    inventario.Stock = inventario.Stock - cantidad;
                        inventario.FechaInventario = thisDay;
						_context.Update(inventario);
						await _context.SaveChangesAsync();
						//***************************************************************

						movimiento.CantidadStockDisponible = inventario.Stock;
						movimiento.FechaMovimiento = thisDay;
						movimiento.CodTipoMovimiento = 4;
						_context.Add(movimiento);
						await _context.SaveChangesAsync();

                       
					   

					//Registramos los detalles del pedido (venta)
					pedido.Venta.Add(new Ventum { Cantidad = cantidad, MetodoDePago=importeParcial.ToString(), FechaPedido = thisDay, CodProducto = i, CodSucursal = 1, CodMovimiento = movimiento.CodMovimiento, CodTipoVenta = 1 });
						await _context.SaveChangesAsync();

					}
                cantidad = 0;
                cont++;
                

            }

            if (ModelState.IsValid)
            {
                //Hacer una validación que si no se ha pagado no se genera la factura
                if (pedido.CodMetodoPago!=1)
                {
                    var metodoP = _context.MetodoPagos.Where(m => m.CodMetodoPago == pedido.CodMetodoPago).FirstOrDefault();

                    if(metodoP == null)
                    {
                        return NotFound();
                    }

					Factura factura = new Factura();
					factura.FechaFactura = thisDay;
                    factura.MetodoPago = metodoP.MetodoPago1;
					factura.ImporteTotal = importeTotal;
					factura.CodUsuario = pedido.CodUsuario;
					_context.Add(factura);
					await _context.SaveChangesAsync();

					pedido.CodFactura = factura.CodFactura;
                    pedido.EstadoPedido = "Pagado";
                }
                else
                {
                    pedido.EstadoPedido = "Pendiente de pago";
                }

               
                _context.Add(pedido);
                await _context.SaveChangesAsync();

				TempData["pedido"] = "Pedido Creado!";

				return RedirectToAction(nameof(Create));
            }
            ViewData["CodFactura"] = new SelectList(_context.Facturas, "CodFactura", "CodFactura", pedido.CodFactura);
            ViewData["CodMetodoPago"] = new SelectList(_context.MetodoPagos, "CodMetodoPago", "CodMetodoPago", pedido.CodMetodoPago);
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios, "CodUsuario", "CodUsuario", pedido.CodUsuario);
            return View(pedido);
        }

		// GET: Pedido/Edit/5
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            ViewData["CodFactura"] = new SelectList(_context.Facturas, "CodFactura", "CodFactura", pedido.CodFactura);
            ViewData["CodMetodoPago"] = new SelectList(_context.MetodoPagos, "CodMetodoPago", "MetodoPago1", pedido.CodMetodoPago);
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios, "CodUsuario", "FullName", pedido.CodUsuario);
            return View(pedido);
        }

        // POST: Pedido/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Edit(int id, [Bind("CodPedido,FechaPedido,EstadoPedido,CodUsuario,CodFactura,CodMetodoPago")] Pedido pedido)
        {
			int? importeTotal = 0;

			if (id != pedido.CodPedido)
            {
                return NotFound();
            }

			if (!ModelState.IsValid)
			{
				// Omitir la validación obligatoria
				ModelState.Remove("CodUsuarioNavigation");
				ModelState.Remove("CodMetodoPagoNavigation");
			}

			if (ModelState.IsValid)
            {
                try
                {
                    if (pedido.CodMetodoPago!=1)
                    {
						//Retrornamos el registro del pedido para actualizar los datos
						var pedido1 = _context.Pedidos.Where(m => m.CodPedido == pedido.CodPedido).Include(m => m.Venta).FirstOrDefault();

						if (pedido1 == null)
						{
							return NotFound();
						}

						foreach (var i in pedido1.Venta)
						{
							if (i.MetodoDePago == null)
							{
								return NotFound();
							}
							importeTotal = importeTotal + int.Parse(i.MetodoDePago);
						}

                        var metodoP = _context.MetodoPagos.Where(m => m.CodMetodoPago == pedido.CodMetodoPago).FirstOrDefault();

                        if (metodoP == null)
                        {
                            return NotFound();
                        }

						DateTime fecha = DateTime.Now;

						Factura factura = new Factura();
						factura.FechaFactura = fecha;
						factura.MetodoPago = metodoP.MetodoPago1;
						factura.ImporteTotal = importeTotal;
						factura.CodUsuario = pedido1.CodUsuario;
						_context.Add(factura);
						await _context.SaveChangesAsync();


						pedido1.CodFactura = factura.CodFactura;
                        pedido1.CodMetodoPago = pedido.CodMetodoPago;
                        pedido1.EstadoPedido = "Pagado";
						_context.Update(pedido1);
						await _context.SaveChangesAsync();
					}
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.CodPedido))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["update"] = "Pedido Actualizado!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodFactura"] = new SelectList(_context.Facturas, "CodFactura", "CodFactura", pedido.CodFactura);
            ViewData["CodMetodoPago"] = new SelectList(_context.MetodoPagos, "CodMetodoPago", "CodMetodoPago", pedido.CodMetodoPago);
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios, "CodUsuario", "CodUsuario", pedido.CodUsuario);
            return View(pedido);
        }

        // GET: Pedido/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.CodFacturaNavigation)
                .Include(p => p.CodMetodoPagoNavigation)
                .Include(p => p.CodUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.CodPedido == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedido/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pedidos == null)
            {
                return Problem("Entity set 'InventarioRfContext.Pedidos'  is null.");
            }
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


		[Authorize(Roles = "Administrador,Ventas")]
		//Obtener datos de un producto
		[HttpGet]
		public IActionResult ObtenerDatosTabla(int idProducto)
		{
			// Obtén los datos de tu origen de datos (por ejemplo, una base de datos)
			List<Producto> lista;
             lista = (from d in _context.Productos
                      where d.CodProducto == idProducto
                      select d).ToList();

			// Devuelve los datos en formato JSON
			return Json(lista);
		}


		private bool PedidoExists(int id)
        {
          return (_context.Pedidos?.Any(e => e.CodPedido == id)).GetValueOrDefault();
        }

		//Metodo que devuelve la lista de administadores
		[HttpPost]
		[Authorize(Roles = "Administrador,Ventas")]
		public ActionResult listaPedidos()
		{
			List<Pedidos> pedidos = new List<Pedidos>();

			try
			{
				recordsTotal = 0;

				IQueryable<Pedidos> query = (from e in _context.Pedidos
											 join m in _context.Usuarios on e.CodUsuario equals m.CodUsuario
                                             join l in _context.MetodoPagos on e.CodMetodoPago equals l.CodMetodoPago
											 select new Pedidos
											 {
												 CodPedido = e.CodPedido,
                                                // FullName = m.FullName,
                                                 Nombre1 = m.Nombre1+" "+m.Nombre2+" "+ m.OtrosNombres+" "+m.Apellido1+" "+m.Apellido2,
                                                 FechaPedido = e.FechaPedido,
                                                 EstadoPedido = e.EstadoPedido,
                                                 Nit = m.Nit,
                                                 Direccion = m.Direccion,
                                                 MetodoPago1 = l.MetodoPago1,

											 });


				recordsTotal = query.Count();
				pedidos = query.ToList();

				return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = pedidos });


			}
			catch (Exception ex)
			{
				throw;
			}


		}

		//Devuelve la cantidad de ventas
		[HttpPost]
		[Authorize(Roles = "Administrador,Ventas")]
		public ActionResult ventasRealizadas()
		{
			List<Ventas> ventas = new List<Ventas>();

			try
			{
				recordsTotal = 0;

				IQueryable<Ventas> query = (from e in _context.Venta
											   join p in _context.Productos on e.CodProducto equals p.CodProducto
                                               join ca in _context.Calidads on p.CodCalidad equals ca.CodCalidad
                                               join co in _context.Colors   on p.CodColor equals co.CodColor
											   join s in _context.Sucursals on e.CodSucursal equals s.CodSucursal
											   select new Ventas
											   {
												   CodPedido = e.CodPedido,
                                                   NombreProducto = p.NombreProducto,
                                                   NombreColor = co.NombreColor,
												   NombreCalidad = ca.NombreCalidad,
                                                   Cantidad = e.Cantidad,
												   NombreSucursal = s.NombreSucursal,
												   FechaVenta = e.FechaPedido,

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



		//Detalles del pedido Json, este debemos de utilizar
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult pedidoJson(int idPedido)
		{
			/*List<Ventum> lista;
			lista = (from d in _context.Venta
					 where d.CodPedido == idPedido
					 select d).Include(m => m.CodProductoNavigation).ToList();
			var options = new JsonSerializerOptions
			{
				ReferenceHandler = ReferenceHandler.Preserve
			};

			var json = JsonSerializer.Serialize(lista, options);
			// Devuelve los datos en formato JSON
			return Json(json);*/
			
			// Obtén los datos de tu origen de datos (por ejemplo, una base de datos)
			List<Pedidos> lista;
			lista = (from p in _context.Pedidos
					 join v in _context.Venta on p.CodPedido equals v.CodPedido
					 join m in _context.Productos on v.CodProducto equals m.CodProducto
					 where p.CodPedido == idPedido
					 select new Pedidos
                     {
                        NombreProducto = m.NombreProducto,
                        precioVenta = m.PrecioVenta,
                        Cantidad = v.Cantidad,
						parcialTotal = v.MetodoDePago, 
                     }).ToList();

			// Devuelve los datos en formato JSON
            return Json(lista);
		}

		//Detalles del pedido
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> DetallePedido(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var detalle = await _context.Pedidos
				.Include(p => p.CodUsuarioNavigation)
                .Include(p => p.CodFacturaNavigation)
				.Include(p => p.CodMetodoPagoNavigation)
                .Include(p => p.Venta)
                .ThenInclude(p => p.CodProductoNavigation)
				.FirstOrDefaultAsync(m => m.CodPedido == id);

			if (detalle == null)
			{
				return NotFound();
			}

			return PartialView("Details", detalle);
		}


        //Imprimir Factura
        [AcceptVerbs("GET", "POST")]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> ImprimirFactura(int? codigoPedido)
		{
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
            };
		}

        //Imprimir Boleta
        [AcceptVerbs("GET", "POST")]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> ImprimirPedido(int? codigoPedido)
        {
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

            return new ViewAsPdf("ImprimirPedido", detalle)
            {
                //  FileName = "prueba.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.Letter
            };
        }

	}
}
