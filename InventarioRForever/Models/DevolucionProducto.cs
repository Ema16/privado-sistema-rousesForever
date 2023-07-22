using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace InventarioRForever.Models;

public partial class DevolucionProducto
{
    public int CodDevolucionVenta { get; set; }

    public DateTime? FechaDevolucion { get; set; }

    public string? Motivo { get; set; }

    public int? CantidadDevueltaProducto { get; set; }

    public int CodUsuario { get; set; }

    public int CodBodega { get; set; }

    public int CodVenta { get; set; }

    public int CodMovimiento { get; set; }

    [NotMapped]
    public string? NombreUsuario { get; set; }
    
    [NotMapped]
    public string? NombreBodega { get; set; }
    public virtual Bodega CodBodegaNavigation { get; set; } = null!;

    public virtual Movimiento CodMovimientoNavigation { get; set; } = null!;

    public virtual Usuario CodUsuarioNavigation { get; set; } = null!;

    public virtual Ventum CodVentaNavigation { get; set; } = null!;
}
