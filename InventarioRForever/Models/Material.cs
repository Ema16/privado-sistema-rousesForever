using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioRForever.Models;

public partial class Material
{
    public int CodMaterial { get; set; }

    public string? NombreMaterial { get; set; }

    public int CodCategoria { get; set; }

    public int CodTipoMaterial { get; set; }

    public int CodInventario { get; set; }


    [NotMapped]
    public int? Stock { get; set; }

    public virtual Categorium CodCategoriaNavigation { get; set; } = null!;

    public virtual Inventario CodInventarioNavigation { get; set; } = null!;

    public virtual TipoMaterial CodTipoMaterialNavigation { get; set; } = null!;

    public virtual ICollection<FabricacionMaterial> FabricacionMaterials { get; set; } = new List<FabricacionMaterial>();

    public virtual ICollection<MaterialOrdenFabricacion> MaterialOrdenFabricacions { get; set; } = new List<MaterialOrdenFabricacion>();

    public virtual ICollection<RecepcionMercancium> RecepcionMercancia { get; set; } = new List<RecepcionMercancium>();
}
