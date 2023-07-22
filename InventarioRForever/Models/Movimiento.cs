using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Movimiento
{
    public int CodMovimiento { get; set; }

    public int? CantidadMovimiento { get; set; }

    public int? StockMenosInventario { get; set; }

    public int? CantidadStockDisponible { get; set; }

    public DateTime? FechaMovimiento { get; set; }

    public string? Lote { get; set; }

    public DateTime? Caducidad { get; set; }

    public int? CodTipoMovimiento { get; set; }

    public virtual ICollection<Bodega> Bodegas { get; set; } = new List<Bodega>();

    public virtual TipMovimiento? CodTipoMovimientoNavigation { get; set; }

    public virtual ICollection<DevolucionMaterial> DevolucionMaterials { get; set; } = new List<DevolucionMaterial>();

    public virtual ICollection<DevolucionProducto> DevolucionProductos { get; set; } = new List<DevolucionProducto>();

    public virtual ICollection<OrdenFabricacion> OrdenFabricacions { get; set; } = new List<OrdenFabricacion>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
