using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Producto
{
    public int CodProducto { get; set; }

    public string? NombreProducto { get; set; }

    public string? EstadoProducto { get; set; }

    public string? Descripcion { get; set; }

    public int? PrecioCompra { get; set; }

    public int? PrecioVenta { get; set; }

    public int CodColor { get; set; }

    public int CodCalidad { get; set; }

    public int CodInventario { get; set; }

    public int CodCategoria { get; set; }

    public int CodMovimiento { get; set; }

    public virtual Calidad CodCalidadNavigation { get; set; } = null!;

    public virtual Categorium CodCategoriaNavigation { get; set; } = null!;

    public virtual Color CodColorNavigation { get; set; } = null!;

    public virtual Inventario CodInventarioNavigation { get; set; } = null!;

    public virtual Movimiento CodMovimientoNavigation { get; set; } = null!;

    public virtual ICollection<Descuento> Descuentos { get; set; } = new List<Descuento>();

    public virtual ICollection<OrdenFabricacion> OrdenFabricacions { get; set; } = new List<OrdenFabricacion>();

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
