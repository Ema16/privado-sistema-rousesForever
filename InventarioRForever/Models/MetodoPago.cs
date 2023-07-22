using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class MetodoPago
{
    public int CodMetodoPago { get; set; }

    public string? MetodoPago1 { get; set; }

    public string? Observaciones { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
