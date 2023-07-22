using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class Fabrica
{
    public int CodFabrica { get; set; }

    public string? NombreFabrica { get; set; }

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public virtual ICollection<OrdenFabricacion> OrdenFabricacions { get; set; } = new List<OrdenFabricacion>();
}
