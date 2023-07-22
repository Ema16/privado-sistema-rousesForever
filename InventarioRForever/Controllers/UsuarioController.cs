using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioRForever.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace InventarioRForever.Controllers
{


	public class UsuarioController : Controller
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

        public UsuarioController(InventarioRfContext context)
        {
            _context = context;
        }

        // GET: Usuario
        //  [Authorize(Roles ="Administrador")]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            IndexHome datos = new IndexHome();

                var countAdmin = (from e in _context.Usuarios
                                  where e.CodRolUsuario == 1 || e.CodRolUsuario == 2
                                  select e).Count();

            var countClientes = (from e in _context.Usuarios
                              where e.CodRolUsuario == 3
                              select e).Count();

            var countProveedore = (from e in _context.Proveedors
                                 select e).Count();

            var countCategorias = (from e in _context.Categoria
                                   select e).Count();

            var countproductos = (from e in _context.Proveedors
                                   select e).Count();

            var countventas = (from e in _context.Pedidos
                                  select e).Count();

            datos.countAdmin = countAdmin;
            datos.countClientes = countClientes;
            datos.countProveedores = countProveedore;
            datos.countCategorias = countCategorias;
            datos.countProductos = countproductos;
            datos.countVentas = countventas;

            return View(datos);
            /* var inventarioRfContext = _context.Usuarios.Include(u => u.CodRolUsuarioNavigation);
             return View(await inventarioRfContext.ToListAsync());*/
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.CodRolUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.CodUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }


		//Cliente 1 para el modal
		//Registro del usuario
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> NuevoClienteModal()
        {
			return PartialView("NuevoCliente");
			
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador,Ventas")]
		public async Task<IActionResult> NuevoClienteModal([Bind("CodUsuario,Nombre1,Nombre2,OtrosNombres,Apellido1,Apellido2,Nit,Direccion,Contacto,Correo,Contrasenia,Estado,RecContrasenia,ActivarUsuario,CodRolUsuario,CodSucursal1")] Usuario usuario)
		{

			//Retornamos el registro del producto para ver a que inventario corresponde
			//var sucursal = _context.Sucursals.Where(m => m.CodSucursal == usuario.CodSucursal1).FirstOrDefault();
			if (usuario.CodRolUsuario == 0 || usuario.CodRolUsuario == null)
			{
				usuario.CodRolUsuario = 3;
			}

			if (!ModelState.IsValid)
			{
				// Omitir la validación obligatoria
				ModelState.Remove("CodRolUsuarioNavigation");
			}

			if (ModelState.IsValid)
			{

				//Buscar sucursal y asignarlo

				if (usuario.Contrasenia != null)
				{
					//usuario.Direccion = sucursal.NombreSucursal;
					usuario.Contrasenia = Crypto.Hash(usuario.Contrasenia);
				}
				usuario.Estado = "Activo";

				_context.Add(usuario);

				TempData["usuario"] = "Usuario Creado!";
				await _context.SaveChangesAsync();

				return RedirectToAction("Create", "Pedido");
			}
			//ViewData["CodRolUsuario"] = new SelectList(_context.RolUsuarios, "CodRolUsuario", "CodRolUsuario", usuario.CodRolUsuario);
			//return View(usuario);
			return RedirectToAction("Create", "Pedido");
		}


		// GET: Usuario/Create
		[Authorize(Roles = "Administrador")]
		public IActionResult Cliente()
		{
            if (TempData["mensaje"] != null)
            {
                ViewBag.mensaje = TempData["mensaje"].ToString();
            }
            return View();
		}

		// POST: Usuario/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Cliente([Bind("CodUsuario,Nombre1,Nombre2,OtrosNombres,Apellido1,Apellido2,Nit,Direccion,Contacto,Correo,Contrasenia,Estado,RecContrasenia,ActivarUsuario,CodRolUsuario,CodSucursal1")] Usuario usuario)
		{

            //Retornamos el registro del producto para ver a que inventario corresponde
            //var sucursal = _context.Sucursals.Where(m => m.CodSucursal == usuario.CodSucursal1).FirstOrDefault();
            if (usuario.CodRolUsuario==0 || usuario.CodRolUsuario==null)
            {
                usuario.CodRolUsuario = 3;
            }

			if (!ModelState.IsValid)
			{
				// Omitir la validación obligatoria
				ModelState.Remove("CodRolUsuarioNavigation");
			}

			if (ModelState.IsValid)
			{

				//Buscar sucursal y asignarlo

				if (usuario.Contrasenia != null)
				{
					//usuario.Direccion = sucursal.NombreSucursal;
					usuario.Contrasenia = Crypto.Hash(usuario.Contrasenia);
				}
				usuario.Estado = "Activo";

				_context.Add(usuario);
				await _context.SaveChangesAsync();

                TempData["mensaje"] = "Cliente creado!";

                return RedirectToAction(nameof(Cliente));
			}
			ViewData["CodRolUsuario"] = new SelectList(_context.RolUsuarios, "CodRolUsuario", "CodRolUsuario", usuario.CodRolUsuario);
			return View(usuario);
		}

		// GET: Usuario/Create
		[Authorize(Roles = "Administrador")]
		public IActionResult Create()
        {
            ViewData["CodRolUsuario"] = new SelectList(_context.RolUsuarios, "CodRolUsuario", "Rol");
			ViewData["CodSucursal"] = new SelectList(_context.Sucursals, "CodSucursal", "NombreSucursal");

			if (TempData["mensaje"] != null)
			{
				ViewBag.mensaje = TempData["mensaje"].ToString();
			}

			return View();
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Create([Bind("CodUsuario,Nombre1,Nombre2,OtrosNombres,Apellido1,Apellido2,Nit,Direccion,Contacto,Correo,Contrasenia,Estado,RecContrasenia,ActivarUsuario,CodRolUsuario,CodSucursal1")] Usuario usuario)
        {

			//Retornamos el registro del producto para ver a que inventario corresponde
			var sucursal = _context.Sucursals.Where(m => m.CodSucursal == usuario.CodSucursal1).FirstOrDefault();

			if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodRolUsuarioNavigation");
            }

            if (sucursal == null)
            {
                return NotFound();
            }
           
            if (ModelState.IsValid)
            {

                //Buscar sucursal y asignarlo

                if (usuario.Contrasenia!=null)
                {
					usuario.Direccion = sucursal.NombreSucursal;
					usuario.Contrasenia = Crypto.Hash(usuario.Contrasenia);
                }
                usuario.Estado = "Activo";

                _context.Add(usuario);
                await _context.SaveChangesAsync();

				TempData["mensaje"] = "Usuario Creado!";

				return RedirectToAction(nameof(Create));
            }
            ViewData["CodRolUsuario"] = new SelectList(_context.RolUsuarios, "CodRolUsuario", "CodRolUsuario", usuario.CodRolUsuario);
            return View(usuario);
        }

		// GET: Usuario/Edit/5
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["CodRolUsuario"] = new SelectList(_context.RolUsuarios, "CodRolUsuario", "CodRolUsuario", usuario.CodRolUsuario);
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Edit(int id, [Bind("CodUsuario,Nombre1,Nombre2,OtrosNombres,Apellido1,Apellido2,Nit,Direccion,Contacto,Correo,Contrasenia,Estado,RecContrasenia,ActivarUsuario,CodRolUsuario")] Usuario usuario)
        {
            if (id != usuario.CodUsuario)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // Omitir la validación obligatoria
                ModelState.Remove("CodRolUsuarioNavigation");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (usuario.Contrasenia!=null)
                    {
                        usuario.Contrasenia = Crypto.Hash(usuario.Contrasenia);
                    }
                    
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.CodUsuario))
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
            ViewData["CodRolUsuario"] = new SelectList(_context.RolUsuarios, "CodRolUsuario", "CodRolUsuario", usuario.CodRolUsuario);
            return View(usuario);
        }

		// GET: Usuario/Delete/5
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.CodRolUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.CodUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'InventarioRfContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
          return (_context.Usuarios?.Any(e => e.CodUsuario == id)).GetValueOrDefault();
        }


        //LOGIN
        [HttpGet]
        public ActionResult Login()
        {

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Login(string usr, string password)
        {

            if (usr == null)
            {
                return NotFound();
            }

            var usuario = _context.Usuarios.FirstOrDefault(m => m.Correo == usr);

            if (usuario != null)
            {
                if (string.Compare(Crypto.Hash(password),usuario.Contrasenia)==0)
                {
                    var rol = _context.RolUsuarios.FirstOrDefault(m => m.CodRolUsuario == usuario.CodRolUsuario);
                    var identify = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, usuario.Correo), new Claim (ClaimTypes.Role, rol.Rol),
                        new Claim(ClaimTypes.NameIdentifier, ""+usuario.CodRolUsuario)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var princial = new ClaimsPrincipal(identify);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,princial);

                    return RedirectToAction("Index","Usuario");
                }
                else
                {
                    TempData["autent"] = "Datos erróneos. Por favor, inténtelo nuevamente!";
                    return RedirectToAction("Index","Home");
                }
            }
            else
            {
                TempData["autent"] = "Datos erróneos. Por favor, inténtelo nuevamente!";
                return RedirectToAction("Index", "Home");
            }
        }


		//Metodo que devuelve la lista de clientes
		[HttpPost]
		[Authorize(Roles = "Administrador")]
		public ActionResult ListaClientes()
		{
			List<Usuario> usuarios = new List<Usuario>();

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

				IQueryable<Usuario> query = (from e in _context.Usuarios
											 join m in _context.RolUsuarios on e.CodRolUsuario equals m.CodRolUsuario
											 where e.CodRolUsuario == 3
											 select new Usuario
											 {
												 CodUsuario = e.CodUsuario,
												 Nombre1 = e.Nombre1,
												 Apellido1 = e.Apellido1,

											 });


				if (searchValue != "")
				{
					query = query.Where(d => d.Nombre1.Contains(searchValue));
				}


				if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
				{
					//	query = query.OrderBy(sortColumn + " " + sortColumnDirection);
				}

				recordsTotal = query.Count();
				usuarios = query.Skip(skip).Take(pageSize).ToList();

				return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = usuarios });


			}
			catch (Exception ex)
			{
				throw;
			}


		}



		//Metodo que devuelve la lista de administadores
		[HttpPost]
		[Authorize(Roles = "Administrador")]
		public ActionResult ListaAdministrador()
        {
            List<Usuario> usuarios = new List<Usuario>();

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

                IQueryable<Usuario> query = (from e in _context.Usuarios
                                             join m in _context.RolUsuarios on e.CodRolUsuario equals m.CodRolUsuario
                                             where e.CodRolUsuario == 1 || e.CodRolUsuario == 2
                                             select new Usuario
                                             {
                                                 CodUsuario = e.CodUsuario,
                                                 Nombre1 = e.Nombre1,
                                                 Apellido1 = e.Apellido1,

                                             });


				if (searchValue != "")
				{
					query = query.Where(d => d.Nombre1.Contains(searchValue));
				}


				if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
				{
				//	query = query.OrderBy(sortColumn + " " + sortColumnDirection);
				}

				recordsTotal = query.Count();
				usuarios = query.Skip(skip).Take(pageSize).ToList();

				return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = usuarios });


			}
			catch (Exception ex)
            {
                throw;
            }


        }

        //Registro del usuario
        [HttpGet]
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> DetalleUsuario(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle = await _context.Usuarios
                .Include(u => u.CodRolUsuarioNavigation)
                .Include(u => u.Pedidos)
                .ThenInclude(u => u.CodFacturaNavigation)
                .FirstOrDefaultAsync(m => m.CodUsuario == id);

            if (detalle == null)
            {
                return NotFound();
            }

            return PartialView("DetalleUsuario", detalle);
        }


		//Obtener datos de un producto
		[HttpGet]
		[Authorize(Roles = "Administrador,Ventas")]
		public IActionResult ObtenerDatosCliente(int codCliente)
		{
			// Obtén los datos de tu origen de datos (por ejemplo, una base de datos)
			List<Usuario> lista;
			lista = (from d in _context.Usuarios
					 where d.CodUsuario == codCliente
					 select d).ToList();

			// Devuelve los datos en formato JSON
			return Json(lista);
		}



		//LOGOUT
		[HttpGet]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AccessDenided()
        {
            return RedirectToAction("Index", "Usuario");
        }

        [HttpPost]
        [Authorize]
        public ActionResult Logout(string j)
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
