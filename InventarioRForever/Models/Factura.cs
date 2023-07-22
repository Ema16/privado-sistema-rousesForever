using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Factura
{
    public int CodFactura { get; set; }

    public DateTime? FechaFactura { get; set; }

    public string? MetodoPago { get; set; }

    public int? ImporteTotal { get; set; }

    public int CodUsuario { get; set; }

    public virtual Usuario CodUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
