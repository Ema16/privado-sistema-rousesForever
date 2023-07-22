using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class TipoVentum
{
    public int CodTipoVenta { get; set; }

    public string? NombreTipoVenta { get; set; }

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
