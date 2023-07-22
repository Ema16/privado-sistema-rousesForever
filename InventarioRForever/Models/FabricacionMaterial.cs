using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class FabricacionMaterial
{
    public int CodFabricacionMaterial { get; set; }

    public int CodProductoFabrica { get; set; }

    public int CodMaterial { get; set; }

    public int? CantidaMaterial { get; set; }

    public virtual Material CodMaterialNavigation { get; set; } = null!;

    public virtual OrdenFabricacion CodProductoFabricaNavigation { get; set; } = null!;
}
