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
	public class ProductoController : Controller
    {
        private readonly InventarioRfContext _context;
		public int pageSize, skip, recordsTotal;

		public ProductoController(InventarioRfContext context)
        {
            _context = context;
        }

		// GET: Producto
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Index()
        {
            var inventarioRfContext = _context.Productos.Include(p => p.CodCalidadNavigation).Include(p => p.CodCategoriaNavigation).Include(p => p.CodColorNavigation).Include(p => p.CodInventarioNavigation).Include(p => p.CodMovimientoNavigation);
            return View(await inventarioRfContext.ToListAsync());
        }

        // GET: Producto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.CodCalidadNavigation)
                .Include(p => p.CodCategoriaNavigation)
                .Include(p => p.CodColorNavigation)
                .Include(p => p.CodInventarioNavigation)
                .Include(p => p.CodMovimientoNavigation)
                .FirstOrDefaultAsync(m => m.CodProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

		// GET: Producto/Create
		[Authorize(Roles = "Administrador")]
		public IActionResult Create()
        {

			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}
			ViewData["CodCalidad"] = new SelectList(_context.Calidads, "CodCalidad", "NombreCalidad");
            ViewData["CodCategoria"] = new SelectList(_context.Categoria, "CodCategoria", "NombreCategoria");
            ViewData["CodColor"] = new SelectList(_context.Colors, "CodColor", "NombreColor");
            ViewData["CodInventario"] = new SelectList(_context.Inventarios, "CodInventario", "CodInventario");
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento");
            return View();
        }

        // POST: Producto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Create([Bind("CodProducto,NombreProducto,EstadoProducto,Descripcion,PrecioCompra,PrecioVenta,CodColor,CodCalidad,CodInventario,CodCategoria,CodMovimiento")] Producto producto)
        {

            if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodCalidadNavigation");
                ModelState.Remove("CodCategoriaNavigation");
                ModelState.Remove("CodColorNavigation");
                ModelState.Remove("CodInventarioNavigation");
                ModelState.Remove("CodMovimientoNavigation");
            }

            if (ModelState.IsValid)
            {
				var color = _context.Colors.Where(m => m.CodColor == producto.CodColor).FirstOrDefault();
                var calidad = _context.Calidads.Where(m => m.CodCalidad == producto.CodCalidad).FirstOrDefault();

                if (color == null || calidad == null)
                {
                    return NotFound();
                }


				DateTime fecha = DateTime.Now;

                Inventario inventario = new Inventario();
                inventario.Stock = 0;
                inventario.FechaInventario = fecha;
                _context.Add(inventario);
                await _context.SaveChangesAsync();

                producto.CodInventario = inventario.CodInventario;

                Movimiento movimiento = new Movimiento();
                movimiento.CantidadMovimiento = 0;
                movimiento.StockMenosInventario = 0;
                movimiento.CantidadStockDisponible = 0;
                movimiento.FechaMovimiento = fecha;
                movimiento.CodTipoMovimiento = 1;
                _context.Add(movimiento);
                await _context.SaveChangesAsync();

                
                producto.CodMovimiento = movimiento.CodMovimiento;
                producto.NombreProducto = producto.NombreProducto + " " + color.NombreColor + " " + calidad.NombreCalidad;

                _context.Add(producto);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Producto creado!";

				return RedirectToAction(nameof(Create));
            }
            ViewData["CodCalidad"] = new SelectList(_context.Calidads, "CodCalidad", "CodCalidad", producto.CodCalidad);
            ViewData["CodCategoria"] = new SelectList(_context.Categoria, "CodCategoria", "CodCategoria", producto.CodCategoria);
            ViewData["CodColor"] = new SelectList(_context.Colors, "CodColor", "CodColor", producto.CodColor);
            ViewData["CodInventario"] = new SelectList(_context.Inventarios, "CodInventario", "CodInventario", producto.CodInventario);
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", producto.CodMovimiento);
            return View(producto);
        }

		// GET: Producto/Edit/5
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CodCalidad"] = new SelectList(_context.Calidads, "CodCalidad", "CodCalidad", producto.CodCalidad);
            ViewData["CodCategoria"] = new SelectList(_context.Categoria, "CodCategoria", "CodCategoria", producto.CodCategoria);
            ViewData["CodColor"] = new SelectList(_context.Colors, "CodColor", "CodColor", producto.CodColor);
            ViewData["CodInventario"] = new SelectList(_context.Inventarios, "CodInventario", "CodInventario", producto.CodInventario);
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", producto.CodMovimiento);
            return View(producto);
        }

        // POST: Producto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Edit(int id, [Bind("CodProducto,NombreProducto,EstadoProducto,Descripcion,PrecioCompra,PrecioVenta,CodColor,CodCalidad,CodInventario,CodCategoria,CodMovimiento")] Producto producto)
        {
            if (id != producto.CodProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.CodProducto))
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
            ViewData["CodCalidad"] = new SelectList(_context.Calidads, "CodCalidad", "CodCalidad", producto.CodCalidad);
            ViewData["CodCategoria"] = new SelectList(_context.Categoria, "CodCategoria", "CodCategoria", producto.CodCategoria);
            ViewData["CodColor"] = new SelectList(_context.Colors, "CodColor", "CodColor", producto.CodColor);
            ViewData["CodInventario"] = new SelectList(_context.Inventarios, "CodInventario", "CodInventario", producto.CodInventario);
            ViewData["CodMovimiento"] = new SelectList(_context.Movimientos, "CodMovimiento", "CodMovimiento", producto.CodMovimiento);
            return View(producto);
        }

        // GET: Producto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.CodCalidadNavigation)
                .Include(p => p.CodCategoriaNavigation)
                .Include(p => p.CodColorNavigation)
                .Include(p => p.CodInventarioNavigation)
                .Include(p => p.CodMovimientoNavigation)
                .FirstOrDefaultAsync(m => m.CodProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

		//Devolvemos los precios, descuentos y stock del producto
		//Detalles del pedido
		[Authorize(Roles = "Administrador,Ventas")]
		[HttpGet]
		public async Task<IActionResult> descuentosProducto(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var descuentos = await _context.Productos
                .Include(m => m.CodInventarioNavigation)
                .Include(m => m.CodColorNavigation)
                .Include(m => m.CodCalidadNavigation)
                .Include(m => m.Descuentos)
				.FirstOrDefaultAsync(m => m.CodProducto == id);

			if (descuentos == null)
			{
				return NotFound();
			}

			return PartialView("Descuentos", descuentos);
		}

		//Devuelve la lista de productos 
		[HttpPost]
		[Authorize(Roles = "Administrador")]
		public ActionResult listadeProductos()
		{
			List<Producto> productos = new List<Producto>();

			try
			{
				recordsTotal = 0;

				IQueryable<Producto> query = (from p in _context.Productos
											  select new Producto
											  {
												  CodProducto = p.CodProducto,
												  NombreProducto = p.NombreProducto,
												  PrecioCompra = p.PrecioCompra,
												  PrecioVenta = p.PrecioVenta,
											  });

				recordsTotal = query.Count();
				productos = query.ToList();

				return Json(new { recordsFiltered = recordsTotal, data = productos });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		//Detalles del producto
		[HttpGet]
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> detalleProducto(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var detalle = await _context.Productos
				.Include(m => m.CodCalidadNavigation)
				.Include(m => m.CodCategoriaNavigation)
				.Include(m => m.CodColorNavigation)
				.Include(m => m.CodInventarioNavigation)
				.Include(m => m.CodMovimientoNavigation)
				.Include(m => m.Descuentos)
				.Include(m => m.OrdenFabricacions)
				.ThenInclude(m => m.CodFabricaNavigation)
				.FirstOrDefaultAsync(m => m.CodProducto == id);

			if (detalle == null)
			{
				return NotFound();
			}

			return PartialView("Details", detalle);
		}


		// POST: Producto/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Productos == null)
            {
                return Problem("Entity set 'InventarioRfContext.Productos'  is null.");
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
          return (_context.Productos?.Any(e => e.CodProducto == id)).GetValueOrDefault();
        }
    }
}
