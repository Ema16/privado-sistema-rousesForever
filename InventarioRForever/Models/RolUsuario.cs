using System;
using System.Collections.Generic;

namespace InventarioRForever.Models;

public partial class RolUsuario
{
    public int CodRolUsuario { get; set; }

    public string? Rol { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
