using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class RecepcionMercancium
{
    public int CodProveedorMaterial { get; set; }

    public int? Cantidad { get; set; }

    public DateTime? CantidadProducida { get; set; }

    public int CodMaterial { get; set; }

    public int CodProveedor { get; set; }

    public virtual Material CodMaterialNavigation { get; set; } = null!;

    public virtual Proveedor CodProveedorNavigation { get; set; } = null!;

    public virtual ICollection<DevolucionMaterial> DevolucionMaterials { get; set; } = new List<DevolucionMaterial>();
}
