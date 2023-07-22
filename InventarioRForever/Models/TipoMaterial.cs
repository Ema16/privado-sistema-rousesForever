using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class TipoMaterial
{
    public int CodTipoMaterial { get; set; }

    public string? NombreTipoMaterial { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
