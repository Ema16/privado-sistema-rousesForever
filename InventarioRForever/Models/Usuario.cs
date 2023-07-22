using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace InventarioRForever.Models;

public partial class Usuario
{
    public int CodUsuario { get; set; }

    public string? Nombre1 { get; set; }

    public string? Nombre2 { get; set; }

    public string? OtrosNombres { get; set; }

    public string? Apellido1 { get; set; }

    public string? Apellido2 { get; set; }

    public string? Nit { get; set; }

    public string? Direccion { get; set; }

    public string? Contacto { get; set; }

    public string? Correo { get; set; }

    public string? Contrasenia { get; set; }

    public string? Estado { get; set; }

    public string? RecContrasenia { get; set; }

    public string? ActivarUsuario { get; set; }

    public int CodRolUsuario { get; set; }

    [Display(Name = "Full Name Usuario")]
    public string FullName
    {
        get
        {
            return Nombre1 + " " + Nombre2 +" "+ OtrosNombres + " " + Apellido1 +" "+ Apellido2;
        }
    }

    [NotMapped]
	public int CodSucursal1 { get; set; }

	public virtual RolUsuario CodRolUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<DevolucionMaterial> DevolucionMaterials { get; set; } = new List<DevolucionMaterial>();

    public virtual ICollection<DevolucionProducto> DevolucionProductos { get; set; } = new List<DevolucionProducto>();

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
