using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioRForever.Models;

public partial class OrdenFabricacion
{
    public int CodProductoFabrica { get; set; }

    public int? CantidadFabricacion { get; set; }

    public DateTime? FechaFabricacion { get; set; }

    public string? EstadoFabricacion { get; set; }

    public string? EstadoIngresoInventario { get; set; }

    public string? Observacion { get; set; }

    public int CodProducto { get; set; }

    public int CodFabrica { get; set; }

    public int CodMovimiento { get; set; }


	//No son parte de la entidad

	[NotMapped]
	public string? NombreProducto { get; set; }

	[NotMapped]
	public string? NombreColor { get; set; }

	[NotMapped]
	public string? NombreCalidad { get; set; }

	[NotMapped]
    public List<int>? Materiales { get; set; }


	[NotMapped]
	public List<int>? Cantidades { get; set; }

    //No son parte de la entidad

	public virtual Fabrica CodFabricaNavigation { get; set; } = null!;

    public virtual Movimiento CodMovimientoNavigation { get; set; } = null!;

    public virtual Producto CodProductoNavigation { get; set; } = null!;

    public virtual ICollection<FabricacionMaterial> FabricacionMaterials { get; set; } = new List<FabricacionMaterial>();

    public virtual ICollection<MaterialOrdenFabricacion> MaterialOrdenFabricacions { get; set; } = new List<MaterialOrdenFabricacion>(); //Queda descartado
}
