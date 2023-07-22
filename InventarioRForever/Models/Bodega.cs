using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Bodega
{
    public int CodBodega { get; set; }

    public int? Isla { get; set; }

    public string? NombreBodega { get; set; }

    public string? Seccion { get; set; }

    public int? Nivel { get; set; }

    public int? Contenedor { get; set; }

    public int? Capacidad { get; set; }

    public int CodMovimiento { get; set; }

    public virtual Movimiento CodMovimientoNavigation { get; set; } = null!;

    public virtual ICollection<DevolucionProducto> DevolucionProductos { get; set; } = new List<DevolucionProducto>();
}
