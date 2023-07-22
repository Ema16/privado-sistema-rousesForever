using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Ventum
{
    public int CodVenta { get; set; }

    public int? Cantidad { get; set; }

    public DateTime? FechaPedido { get; set; }

    public string? MetodoDePago { get; set; }

    public int CodPedido { get; set; }

    public int CodProducto { get; set; }

    public int? CodFactura { get; set; }

    public int CodSucursal { get; set; }

    public int CodMovimiento { get; set; }

    public int CodTipoVenta { get; set; }

    public virtual Factura? CodFacturaNavigation { get; set; }

    public virtual Movimiento CodMovimientoNavigation { get; set; } = null!;

    public virtual Pedido CodPedidoNavigation { get; set; } = null!;

    public virtual Producto CodProductoNavigation { get; set; } = null!;

    public virtual Sucursal CodSucursalNavigation { get; set; } = null!;

    public virtual TipoVentum CodTipoVentaNavigation { get; set; } = null!;

    public virtual ICollection<DevolucionProducto> DevolucionProductos { get; set; } = new List<DevolucionProducto>();
}
