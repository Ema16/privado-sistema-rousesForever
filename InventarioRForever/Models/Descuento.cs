using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioRForever.Models;

public partial class Descuento
{
    public int CodProductoDescuento { get; set; }

    public int? Descuento1 { get; set; }

    public DateTime? FechaDescuento { get; set; }

    public string? EstadoDescuento { get; set; }

    public int CodProducto { get; set; }

    [NotMapped]
    public string? NombreProducto { get; set; }

    public virtual Producto CodProductoNavigation { get; set; } = null!;
}
