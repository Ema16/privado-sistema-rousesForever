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
	public class DevolucionProductoController : Controller
    {
        private readonly InventarioRfContext _context;
        public int pageSize, skip, recordsTotal;

        public DevolucionProductoController(InventarioRfContext context)
        {
            _context = context;
        }

		// GET: DevolucionProducto
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Index()
        {
			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}

			var inventarioRfContext = _context.DevolucionProductos.Include(d => d.CodBodegaNavigation).Include(d => d.CodMovimientoNavigation).Include(d => d.CodUsuarioNavigation).Include(d => d.CodVentaNavigation);
            return View(await inventarioRfContext.ToListAsync());
        }

        // GET: DevolucionProducto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DevolucionProductos == null)
            {
                return NotFound();
            }

            var devolucionProducto = await _context.DevolucionProductos
                .Include(d => d.CodBodegaNavigation)
                .Include(d => d.CodMovimientoNavigation)
                .Include(d => d.CodUsuarioNavigation)
                .Include(d => d.CodVentaNavigation)
                .FirstOrDefaultAsync(m => m.CodDevolucionVenta == id);
            if (devolucionProducto == null)
            {
                return NotFound();
            }

            return View(devolucionProducto);
        }

		// GET: DevolucionProducto/Create
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult Create()
        {
            ViewData["CodBodega"] = new SelectList(_context.Bodegas, "CodBodega", "NombreBodega");
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento");
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios.Where(m => m.CodRolUsuario==3), "CodUsuario", "FullName");
            ViewData["CodVenta"] = new SelectList(_context.Venta, "CodVenta", "CodVenta");
            return View();
        }

        // POST: DevolucionProducto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Create([Bind("CodDevolucionVenta,FechaDevolucion,Motivo,CantidadDevueltaProducto,CodUsuario,CodBodega,CodVenta,CodMovimiento")] DevolucionProducto devolucionProducto)
        {
            DateTime fecha = DateTime.Now;

			if (!ModelState.IsValid)
			{
				// Omitir la validación obligatoria
				ModelState.Remove("CodBodegaNavigation");
				ModelState.Remove("CodMovimientoNavigation");
				ModelState.Remove("CodUsuarioNavigation");
				ModelState.Remove("CodVentaNavigation");
			}


			if (ModelState.IsValid)
            {
                Movimiento movimiento = new Movimiento();
                movimiento.CodTipoMovimiento = 7;
                movimiento.CantidadMovimiento = devolucionProducto.CantidadDevueltaProducto;
                movimiento.FechaMovimiento = fecha;
                _context.Add(movimiento);
                await _context.SaveChangesAsync();
                devolucionProducto.CodMovimiento = movimiento.CodMovimiento;

				_context.Add(devolucionProducto);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Devolucion producto!";

				return RedirectToAction(nameof(Index));
            }
            ViewData["CodBodega"] = new SelectList(_context.Bodegas, "CodBodega", "CodBodega", devolucionProducto.CodBodega);
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", devolucionProducto.CodMovimiento);
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios, "CodUsuario", "CodUsuario", devolucionProducto.CodUsuario);
            ViewData["CodVenta"] = new SelectList(_context.Venta, "CodVenta", "CodVenta", devolucionProducto.CodVenta);
            return View(devolucionProducto);
        }

		// GET: DevolucionProducto/Edit/5
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DevolucionProductos == null)
            {
                return NotFound();
            }

            var devolucionProducto = await _context.DevolucionProductos.FindAsync(id);
            if (devolucionProducto == null)
            {
                return NotFound();
            }
            ViewData["CodBodega"] = new SelectList(_context.Bodegas, "CodBodega", "CodBodega", devolucionProducto.CodBodega);
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", devolucionProducto.CodMovimiento);
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios, "CodUsuario", "CodUsuario", devolucionProducto.CodUsuario);
            ViewData["CodVenta"] = new SelectList(_context.Venta, "CodVenta", "CodVenta", devolucionProducto.CodVenta);
            return View(devolucionProducto);
        }

        // POST: DevolucionProducto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodDevolucionVenta,FechaDevolucion,Motivo,CantidadDevueltaProducto,CodUsuario,CodBodega,CodVenta,CodMovimiento")] DevolucionProducto devolucionProducto)
        {
            if (id != devolucionProducto.CodDevolucionVenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(devolucionProducto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DevolucionProductoExists(devolucionProducto.CodDevolucionVenta))
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
            ViewData["CodBodega"] = new SelectList(_context.Bodegas, "CodBodega", "CodBodega", devolucionProducto.CodBodega);
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", devolucionProducto.CodMovimiento);
            ViewData["CodUsuario"] = new SelectList(_context.Usuarios, "CodUsuario", "CodUsuario", devolucionProducto.CodUsuario);
            ViewData["CodVenta"] = new SelectList(_context.Venta, "CodVenta", "CodVenta", devolucionProducto.CodVenta);
            return View(devolucionProducto);
        }

        // GET: DevolucionProducto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DevolucionProductos == null)
            {
                return NotFound();
            }

            var devolucionProducto = await _context.DevolucionProductos
                .Include(d => d.CodBodegaNavigation)
                .Include(d => d.CodMovimientoNavigation)
                .Include(d => d.CodUsuarioNavigation)
                .Include(d => d.CodVentaNavigation)
                .FirstOrDefaultAsync(m => m.CodDevolucionVenta == id);
            if (devolucionProducto == null)
            {
                return NotFound();
            }

            return View(devolucionProducto);
        }

        // POST: DevolucionProducto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DevolucionProductos == null)
            {
                return Problem("Entity set 'InventarioRfContext.DevolucionProductos'  is null.");
            }
            var devolucionProducto = await _context.DevolucionProductos.FindAsync(id);
            if (devolucionProducto != null)
            {
                _context.DevolucionProductos.Remove(devolucionProducto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
		}

		//Obtener datos de un pedido de usuario
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult ObternerPedidosUsuario(int codUsuario)
		{
			// Retornamos pedidos que ya estan pagados

			List<Pedido> lista;
			lista = (from d in _context.Pedidos
					 where d.CodUsuario == codUsuario && d.EstadoPedido== "Pagado"
					 select d).ToList();

			// Devuelve los datos en formato JSON
			return Json(lista);
		}

		//Obtener datos productos de un pedido
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult ObternerDetallePedido(int codPedido)
		{
			// Retornamos pedidos que ya estan pagados

			List<Pedidos> lista;
			lista = (from v in _context.Venta
					 join p in _context.Productos on v.CodProducto equals p.CodProducto
					 where v.CodPedido == codPedido
					 select new Pedidos
					 {
						 CodVenta = v.CodVenta,
						 NombreProducto = p.NombreProducto,
                         Cantidad = v.Cantidad,
					 }).ToList();

			// Devuelve los datos en formato JSON
			return Json(lista);
		}

		private bool DevolucionProductoExists(int id)
        {
          return (_context.DevolucionProductos?.Any(e => e.CodDevolucionVenta == id)).GetValueOrDefault();
        }

        //Devuelve la lista de devoluciones de productos 
        [HttpPost]
        public ActionResult listaDevProducto()
        {
            List<DevolucionProducto> devolucionesP = new List<DevolucionProducto>();

            try
            {
                recordsTotal = 0;

                IQueryable<DevolucionProducto> query = (from d in _context.DevolucionProductos
                                                        join u in _context.Usuarios on d.CodUsuario equals u.CodUsuario
                                                        join b in _context.Bodegas on d.CodBodega equals b.CodBodega
                                                        select new DevolucionProducto
                                                        {
                                                            CodVenta = d.CodVenta,
                                                            FechaDevolucion = d.FechaDevolucion,
                                                            CantidadDevueltaProducto = d.CantidadDevueltaProducto,
                                                            NombreUsuario = u.Nombre1,
                                                            NombreBodega = b.NombreBodega,
                                                        });

                recordsTotal = query.Count();
                devolucionesP = query.ToList();

                return Json(new { recordsFiltered = recordsTotal, data = devolucionesP });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Detalles de la devolucion de producto
        [HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> detalleDevProducto(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle = await _context.Venta
                .Include(m => m.CodFacturaNavigation)
                .Include(m => m.CodPedidoNavigation)
                .Include(m => m.CodProductoNavigation)
                .Include(m => m.CodSucursalNavigation)
                .Include(m => m.DevolucionProductos)
                .ThenInclude(m => m.CodUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.CodVenta == id);

            if (detalle == null)
            {
                return NotFound();
            }

            return PartialView("Details", detalle);
        }

    }
}
