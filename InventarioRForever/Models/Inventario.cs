using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Inventario
{
    public int CodInventario { get; set; }

    public int? Stock { get; set; }

    public int? StockDevuelto { get; set; }

    public DateTime? FechaInventario { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
