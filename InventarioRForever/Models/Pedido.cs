using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioRForever.Models;

public partial class Pedido
{
    public int CodPedido { get; set; }

    public DateTime? FechaPedido { get; set; }

    public string? EstadoPedido { get; set; }

    public int CodUsuario { get; set; }

    public int? CodFactura { get; set; }

    public int CodMetodoPago { get; set; }

    [NotMapped]
	public List<int>? Productos { get; set; }

	[NotMapped]
	public List<int>? Cantidades { get; set; }

	public virtual Factura? CodFacturaNavigation { get; set; }

    public virtual MetodoPago CodMetodoPagoNavigation { get; set; } = null!;

    public virtual Usuario CodUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
