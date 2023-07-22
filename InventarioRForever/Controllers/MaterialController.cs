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
	public class MaterialController : Controller
    {
        private readonly InventarioRfContext _context;
		public int pageSize, skip, recordsTotal;
		public MaterialController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: Material
        public async Task<IActionResult> Index()
        {
            var inventarioRfContext = _context.Materials.Include(m => m.CodCategoriaNavigation).Include(m => m.CodInventarioNavigation).Include(m => m.CodTipoMaterialNavigation);
            return View(await inventarioRfContext.ToListAsync());
        }

        // GET: Material/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Materials == null)
            {
                return NotFound();
            }

            var material = await _context.Materials
                .Include(m => m.CodCategoriaNavigation)
                .Include(m => m.CodInventarioNavigation)
                .Include(m => m.CodTipoMaterialNavigation)
                .FirstOrDefaultAsync(m => m.CodMaterial == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
		}

		// GET: Material/Create
		[Authorize(Roles = "Administrador")]
		public IActionResult Create()
        {

			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}
			ViewData["CodCategoria"] = new SelectList(_context.Categoria, "CodCategoria", "NombreCategoria");
            ViewData["CodInventario"] = new SelectList(_context.Inventarios, "CodInventario", "CodInventario");
            ViewData["CodTipoMaterial"] = new SelectList(_context.TipoMaterials, "CodTipoMaterial", "NombreTipoMaterial");
            return View();
        }

        // POST: Material/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Create([Bind("CodMaterial,NombreMaterial,CodCategoria,CodTipoMaterial,CodInventario")] Material material)
        {
            if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodInventarioNavigation");
                ModelState.Remove("CodTipoMaterialNavigation");
                ModelState.Remove("CodCategoriaNavigation");
            }

            if (ModelState.IsValid)
            {
				DateTime fecha = DateTime.Now;

				//Agregamos el registro al inventario del producto siendo este inicialmente 0
				Inventario inventario = new Inventario();
                inventario.Stock = 0;
                inventario.FechaInventario = fecha;

                _context.Add(inventario);
                await _context.SaveChangesAsync();
                material.CodInventario = inventario.CodInventario;
                //Agregamos el registro al inventario del producto siendo este inicialmente 0


                _context.Add(material);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Material Creada!";

				return RedirectToAction(nameof(Create));
            }
            ViewData["CodCategoria"] = new SelectList(_context.Categoria, "CodCategoria", "CodCategoria", material.CodCategoria);
            ViewData["CodInventario"] = new SelectList(_context.Inventarios, "CodInventario", "CodInventario", material.CodInventario);
            ViewData["CodTipoMaterial"] = new SelectList(_context.TipoMaterials, "CodTipoMaterial", "CodTipoMaterial", material.CodTipoMaterial);
            return View(material);
        }

        // GET: Material/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Materials == null)
            {
                return NotFound();
            }

            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            ViewData["CodCategoria"] = new SelectList(_context.Categoria, "CodCategoria", "CodCategoria", material.CodCategoria);
            ViewData["CodInventario"] = new SelectList(_context.Inventarios, "CodInventario", "CodInventario", material.CodInventario);
            ViewData["CodTipoMaterial"] = new SelectList(_context.TipoMaterials, "CodTipoMaterial", "CodTipoMaterial", material.CodTipoMaterial);
            return View(material);
        }

        // POST: Material/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodMaterial,NombreMaterial,CodCategoria,CodTipoMaterial,CodInventario")] Material material)
        {
            if (id != material.CodMaterial)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(material.CodMaterial))
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
            ViewData["CodCategoria"] = new SelectList(_context.Categoria, "CodCategoria", "CodCategoria", material.CodCategoria);
            ViewData["CodInventario"] = new SelectList(_context.Inventarios, "CodInventario", "CodInventario", material.CodInventario);
            ViewData["CodTipoMaterial"] = new SelectList(_context.TipoMaterials, "CodTipoMaterial", "CodTipoMaterial", material.CodTipoMaterial);
            return View(material);
        }

        // GET: Material/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Materials == null)
            {
                return NotFound();
            }

            var material = await _context.Materials
                .Include(m => m.CodCategoriaNavigation)
                .Include(m => m.CodInventarioNavigation)
                .Include(m => m.CodTipoMaterialNavigation)
                .FirstOrDefaultAsync(m => m.CodMaterial == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // POST: Material/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Materials == null)
            {
                return Problem("Entity set 'InventarioRfContext.Materials'  is null.");
            }
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialExists(int id)
        {
          return (_context.Materials?.Any(e => e.CodMaterial == id)).GetValueOrDefault();
        }

		//Devuelve la lista de Materiales 
		[HttpPost]
		[Authorize(Roles = "Administrador,Ventas")]
		public ActionResult listadeMateriales()
		{
			List<Material> materiales = new List<Material>();

			try
			{
				recordsTotal = 0;

				IQueryable<Material> query = (from m in _context.Materials
											  join i in _context.Inventarios on m.CodInventario equals i.CodInventario
											  select new Material
											{
												CodMaterial = m.CodMaterial,
                                                NombreMaterial = m.NombreMaterial,
                                                Stock = i.Stock,
											});

				recordsTotal = query.Count();
				materiales = query.ToList();

				return Json(new { recordsFiltered = recordsTotal, data = materiales });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		//Detalles de las fabricas
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> detalleMaterial(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var detalle = await _context.Materials
                .Include(m => m.CodCategoriaNavigation)
                .Include(m => m.CodTipoMaterialNavigation)
                .Include(m => m.CodInventarioNavigation)
                .Include(m => m.FabricacionMaterials)
                .ThenInclude (m => m.CodProductoFabricaNavigation)
                .ThenInclude( m => m.CodProductoNavigation)
                .Include(m => m.RecepcionMercancia)
                .ThenInclude(m => m.CodProveedorNavigation)
				.FirstOrDefaultAsync(m => m.CodMaterial == id);

			if (detalle == null)
			{
				return NotFound();
			}

			return PartialView("Details", detalle);
		}

	}
}
