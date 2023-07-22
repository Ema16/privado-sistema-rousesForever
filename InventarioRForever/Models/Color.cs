using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Color
{
    public int CodColor { get; set; }

    public string? NombreColor { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
