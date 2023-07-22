using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioRForever.Models;

public partial class DevolucionMaterial
{
    public int CodDevolucionMaterial { get; set; }

    public DateTime? FechaDevolucion { get; set; }

    public string? Motivo { get; set; }

    public string? TipoDevolucion { get; set; }

    public int? CantidadDevueltaMaterial { get; set; }

    public int CodUsuario { get; set; }

    public int CodProveedorMaterial { get; set; }

    public int CodMovimiento { get; set; }

    [NotMapped]
	public int? CodMaterial { get; set; }

    [NotMapped]
    public string? NombreMaterial { get; set; }


    [NotMapped]
    public string? NombreProveedor { get; set; }

    public virtual Movimiento CodMovimientoNavigation { get; set; } = null!;

    public virtual RecepcionMercancium CodProveedorMaterialNavigation { get; set; } = null!;

    public virtual Usuario CodUsuarioNavigation { get; set; } = null!;
}
