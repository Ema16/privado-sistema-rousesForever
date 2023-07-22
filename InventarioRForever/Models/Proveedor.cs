using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Proveedor
{
    public int CodProveedor { get; set; }

    public string? Nombre { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<RecepcionMercancium> RecepcionMercancia { get; set; } = new List<RecepcionMercancium>();
}
