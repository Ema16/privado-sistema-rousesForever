using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Categorium
{
    public int CodCategoria { get; set; }

    public string? NombreCategoria { get; set; }

    public string? TipoCategoria { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
