using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Calidad
{
    public int CodCalidad { get; set; }

    public string? NombreCalidad { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
