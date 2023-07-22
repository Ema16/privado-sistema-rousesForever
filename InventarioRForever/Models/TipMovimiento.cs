using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class TipMovimiento
{
    public int CodTipoMovimiento { get; set; }

    public string? TipoMovimiento { get; set; }

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
