using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InventarioRForever.Models;

public partial class InventarioRfContext : DbContext
{
    public InventarioRfContext()
    {
    }

    public InventarioRfContext(DbContextOptions<InventarioRfContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bodega> Bodegas { get; set; }

    public virtual DbSet<Calidad> Calidads { get; set; }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Descuento> Descuentos { get; set; }

    public virtual DbSet<DevolucionMaterial> DevolucionMaterials { get; set; }

    public virtual DbSet<DevolucionProducto> DevolucionProductos { get; set; }

    public virtual DbSet<Fabrica> Fabricas { get; set; }

    public virtual DbSet<FabricacionMaterial> FabricacionMaterials { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Inventario> Inventarios { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<MaterialOrdenFabricacion> MaterialOrdenFabricacions { get; set; }

    public virtual DbSet<MetodoPago> MetodoPagos { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<OrdenFabricacion> OrdenFabricacions { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<RecepcionMercancium> RecepcionMercancia { get; set; }

    public virtual DbSet<RolUsuario> RolUsuarios { get; set; }

    public virtual DbSet<Sucursal> Sucursals { get; set; }

    public virtual DbSet<TipMovimiento> TipMovimientos { get; set; }

    public virtual DbSet<TipoMaterial> TipoMaterials { get; set; }

    public virtual DbSet<TipoVentum> TipoVenta { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Ventum> Venta { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
       // => optionsBuilder.UseMySQL("Server=192.168.99.100;port=3306;Database=Inventario_RF;Uid=emanuel;Pwd=emanuel.amperez;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bodega>(entity =>
        {
            entity.HasKey(e => e.CodBodega).HasName("PRIMARY");

            entity.ToTable("Bodega");

            entity.HasIndex(e => e.CodMovimiento, "RefMovimiento89");

            entity.Property(e => e.CodBodega).HasColumnName("cod_bodega");
            entity.Property(e => e.Capacidad).HasColumnName("capacidad");
            entity.Property(e => e.CodMovimiento).HasColumnName("cod_movimiento");
            entity.Property(e => e.Contenedor).HasColumnName("contenedor");
            entity.Property(e => e.Isla).HasColumnName("isla");
            entity.Property(e => e.Nivel).HasColumnName("nivel");
            entity.Property(e => e.NombreBodega)
                .HasMaxLength(250)
                .HasColumnName("nombre_bodega");
            entity.Property(e => e.Seccion)
                .HasMaxLength(10)
                .HasColumnName("seccion");

            entity.HasOne(d => d.CodMovimientoNavigation).WithMany(p => p.Bodegas)
                .HasForeignKey(d => d.CodMovimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefMovimiento89");
        });

        modelBuilder.Entity<Calidad>(entity =>
        {
            entity.HasKey(e => e.CodCalidad).HasName("PRIMARY");

            entity.ToTable("Calidad");

            entity.Property(e => e.CodCalidad).HasColumnName("cod_calidad");
            entity.Property(e => e.NombreCalidad)
                .HasMaxLength(150)
                .HasColumnName("nombre_calidad");
        });

        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.CodCategoria).HasName("PRIMARY");

            entity.Property(e => e.CodCategoria).HasColumnName("cod_categoria");
            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(250)
                .HasColumnName("nombre_categoria");
            entity.Property(e => e.TipoCategoria)
                .HasMaxLength(50)
                .HasColumnName("tipo_categoria");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.CodColor).HasName("PRIMARY");

            entity.ToTable("Color");

            entity.Property(e => e.CodColor).HasColumnName("cod_color");
            entity.Property(e => e.NombreColor)
                .HasMaxLength(150)
                .HasColumnName("nombre_color");
        });

        modelBuilder.Entity<Descuento>(entity =>
        {
            entity.HasKey(e => e.CodProductoDescuento).HasName("PRIMARY");

            entity.ToTable("Descuento");

            entity.HasIndex(e => e.CodProducto, "RefProducto40");

            entity.Property(e => e.CodProductoDescuento).HasColumnName("cod_producto_descuento");
            entity.Property(e => e.CodProducto).HasColumnName("cod_producto");
            entity.Property(e => e.Descuento1).HasColumnName("descuento");
            entity.Property(e => e.EstadoDescuento)
                .HasMaxLength(150)
                .HasColumnName("estado_descuento");
            entity.Property(e => e.FechaDescuento)
                .HasColumnType("datetime")
                .HasColumnName("fecha_descuento");

            entity.HasOne(d => d.CodProductoNavigation).WithMany(p => p.Descuentos)
                .HasForeignKey(d => d.CodProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefProducto40");
        });

        modelBuilder.Entity<DevolucionMaterial>(entity =>
        {
            entity.HasKey(e => e.CodDevolucionMaterial).HasName("PRIMARY");

            entity.ToTable("devolucionMaterial");

            entity.HasIndex(e => e.CodMovimiento, "RefMovimiento90");

            entity.HasIndex(e => e.CodProveedorMaterial, "RefRecepcionMercancia52");

            entity.HasIndex(e => e.CodUsuario, "RefUsuario51");

            entity.Property(e => e.CodDevolucionMaterial).HasColumnName("cod_devolucion_material");
            entity.Property(e => e.CantidadDevueltaMaterial).HasColumnName("cantidad_devuelta_material");
            entity.Property(e => e.CodMovimiento).HasColumnName("cod_movimiento");
            entity.Property(e => e.CodProveedorMaterial).HasColumnName("cod_proveedor_material");
            entity.Property(e => e.CodUsuario).HasColumnName("cod_usuario");
            entity.Property(e => e.FechaDevolucion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_devolucion");
            entity.Property(e => e.Motivo)
                .HasMaxLength(250)
                .HasColumnName("motivo");
            entity.Property(e => e.TipoDevolucion)
                .HasMaxLength(250)
                .HasColumnName("tipo_devolucion");

            entity.HasOne(d => d.CodMovimientoNavigation).WithMany(p => p.DevolucionMaterials)
                .HasForeignKey(d => d.CodMovimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefMovimiento90");

            entity.HasOne(d => d.CodProveedorMaterialNavigation).WithMany(p => p.DevolucionMaterials)
                .HasForeignKey(d => d.CodProveedorMaterial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefRecepcionMercancia52");

            entity.HasOne(d => d.CodUsuarioNavigation).WithMany(p => p.DevolucionMaterials)
                .HasForeignKey(d => d.CodUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefUsuario51");
        });

        modelBuilder.Entity<DevolucionProducto>(entity =>
        {
            entity.HasKey(e => e.CodDevolucionVenta).HasName("PRIMARY");

            entity.ToTable("devolucionProducto");

            entity.HasIndex(e => e.CodMovimiento, "RefMovimiento88");

            entity.HasIndex(e => e.CodUsuario, "RefUsuario11");

            entity.HasIndex(e => e.CodVenta, "RefVenta50");

            entity.HasIndex(e => e.CodBodega, "SDFG");

            entity.Property(e => e.CodDevolucionVenta).HasColumnName("cod_devolucion_venta");
            entity.Property(e => e.CantidadDevueltaProducto).HasColumnName("cantidad_devuelta_producto");
            entity.Property(e => e.CodBodega).HasColumnName("cod_bodega");
            entity.Property(e => e.CodMovimiento).HasColumnName("cod_movimiento");
            entity.Property(e => e.CodUsuario).HasColumnName("cod_usuario");
            entity.Property(e => e.CodVenta).HasColumnName("cod_venta");
            entity.Property(e => e.FechaDevolucion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_devolucion");
            entity.Property(e => e.Motivo)
                .HasMaxLength(250)
                .HasColumnName("motivo");

            entity.HasOne(d => d.CodBodegaNavigation).WithMany(p => p.DevolucionProductos)
                .HasForeignKey(d => d.CodBodega)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SDFG");

            entity.HasOne(d => d.CodMovimientoNavigation).WithMany(p => p.DevolucionProductos)
                .HasForeignKey(d => d.CodMovimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefMovimiento88");

            entity.HasOne(d => d.CodUsuarioNavigation).WithMany(p => p.DevolucionProductos)
                .HasForeignKey(d => d.CodUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefUsuario11");

            entity.HasOne(d => d.CodVentaNavigation).WithMany(p => p.DevolucionProductos)
                .HasForeignKey(d => d.CodVenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefVenta50");
        });

        modelBuilder.Entity<Fabrica>(entity =>
        {
            entity.HasKey(e => e.CodFabrica).HasName("PRIMARY");

            entity.ToTable("Fabrica");

            entity.Property(e => e.CodFabrica).HasColumnName("cod_fabrica");
            entity.Property(e => e.Direccion)
                .HasMaxLength(150)
                .HasColumnName("direccion");
            entity.Property(e => e.NombreFabrica)
                .HasMaxLength(250)
                .HasColumnName("nombre_fabrica");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<FabricacionMaterial>(entity =>
        {
            entity.HasKey(e => e.CodFabricacionMaterial).HasName("PRIMARY");

            entity.ToTable("Fabricacion_Material");

            entity.HasIndex(e => e.CodMaterial, "RefMaterial99");

            entity.HasIndex(e => e.CodProductoFabrica, "RefOrden_Fabricacion98");

            entity.Property(e => e.CodFabricacionMaterial).HasColumnName("cod_fabricacion_material");
            entity.Property(e => e.CantidaMaterial).HasColumnName("cantida_material");
            entity.Property(e => e.CodMaterial).HasColumnName("cod_material");
            entity.Property(e => e.CodProductoFabrica).HasColumnName("cod_producto_fabrica");

            entity.HasOne(d => d.CodMaterialNavigation).WithMany(p => p.FabricacionMaterials)
                .HasForeignKey(d => d.CodMaterial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefMaterial99");

            entity.HasOne(d => d.CodProductoFabricaNavigation).WithMany(p => p.FabricacionMaterials)
                .HasForeignKey(d => d.CodProductoFabrica)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefOrden_Fabricacion98");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.CodFactura).HasName("PRIMARY");

            entity.ToTable("Factura");

            entity.HasIndex(e => e.CodUsuario, "RefUsuario9");

            entity.Property(e => e.CodFactura).HasColumnName("cod_factura");
            entity.Property(e => e.CodUsuario).HasColumnName("cod_usuario");
            entity.Property(e => e.FechaFactura)
                .HasColumnType("datetime")
                .HasColumnName("fecha_factura");
            entity.Property(e => e.ImporteTotal).HasColumnName("importe_total");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(250)
                .HasColumnName("metodo_pago");

            entity.HasOne(d => d.CodUsuarioNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.CodUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefUsuario9");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.CodInventario).HasName("PRIMARY");

            entity.ToTable("Inventario");

            entity.Property(e => e.CodInventario).HasColumnName("cod_inventario");
            entity.Property(e => e.FechaInventario)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inventario");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.StockDevuelto).HasColumnName("stock_devuelto");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.CodMaterial).HasName("PRIMARY");

            entity.ToTable("Material");

            entity.HasIndex(e => e.CodCategoria, "RefCategoria25");

            entity.HasIndex(e => e.CodInventario, "RefInventario72");

            entity.HasIndex(e => e.CodTipoMaterial, "RefTipoMaterial68");

            entity.Property(e => e.CodMaterial).HasColumnName("cod_material");
            entity.Property(e => e.CodCategoria).HasColumnName("cod_categoria");
            entity.Property(e => e.CodInventario).HasColumnName("cod_inventario");
            entity.Property(e => e.CodTipoMaterial).HasColumnName("cod_tipo_material");
            entity.Property(e => e.NombreMaterial)
                .HasMaxLength(250)
                .HasColumnName("nombre_material");

            entity.HasOne(d => d.CodCategoriaNavigation).WithMany(p => p.Materials)
                .HasForeignKey(d => d.CodCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCategoria25");

            entity.HasOne(d => d.CodInventarioNavigation).WithMany(p => p.Materials)
                .HasForeignKey(d => d.CodInventario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefInventario72");

            entity.HasOne(d => d.CodTipoMaterialNavigation).WithMany(p => p.Materials)
                .HasForeignKey(d => d.CodTipoMaterial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefTipoMaterial68");
        });

        modelBuilder.Entity<MaterialOrdenFabricacion>(entity =>
        {
            entity.HasKey(e => e.CodMaterialOrden).HasName("PRIMARY");

            entity.ToTable("Material_ordenFabricacion");

            entity.HasIndex(e => e.CodMaterial, "RefMaterial76");

            entity.HasIndex(e => e.CodProductoFabrica, "RefOrden_Fabricacion77");

            entity.Property(e => e.CodMaterialOrden).HasColumnName("cod_material_orden");
            entity.Property(e => e.CodMaterial).HasColumnName("cod_material");
            entity.Property(e => e.CodProductoFabrica).HasColumnName("cod_producto_fabrica");

            entity.HasOne(d => d.CodMaterialNavigation).WithMany(p => p.MaterialOrdenFabricacions)
                .HasForeignKey(d => d.CodMaterial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefMaterial76");

            entity.HasOne(d => d.CodProductoFabricaNavigation).WithMany(p => p.MaterialOrdenFabricacions)
                .HasForeignKey(d => d.CodProductoFabrica)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefOrden_Fabricacion77");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.CodMetodoPago).HasName("PRIMARY");

            entity.ToTable("metodoPago");

            entity.Property(e => e.CodMetodoPago).HasColumnName("cod_metodo_pago");
            entity.Property(e => e.MetodoPago1)
                .HasMaxLength(250)
                .HasColumnName("metodo_pago");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(250)
                .HasColumnName("observaciones");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.CodMovimiento).HasName("PRIMARY");

            entity.ToTable("Movimiento");

            entity.HasIndex(e => e.CodTipoMovimiento, "ReftipMovimiento56");

            entity.Property(e => e.CodMovimiento).HasColumnName("cod_movimiento");
            entity.Property(e => e.Caducidad)
                .HasColumnType("datetime")
                .HasColumnName("caducidad");
            entity.Property(e => e.CantidadMovimiento).HasColumnName("cantidad_movimiento");
            entity.Property(e => e.CantidadStockDisponible).HasColumnName("cantidad_stock_disponible");
            entity.Property(e => e.CodTipoMovimiento).HasColumnName("cod_tipoMovimiento");
            entity.Property(e => e.FechaMovimiento)
                .HasColumnType("datetime")
                .HasColumnName("fecha_movimiento");
            entity.Property(e => e.Lote)
                .HasMaxLength(150)
                .HasColumnName("lote");
            entity.Property(e => e.StockMenosInventario).HasColumnName("stock_menos_inventario");

            entity.HasOne(d => d.CodTipoMovimientoNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.CodTipoMovimiento)
                .HasConstraintName("ReftipMovimiento56");
        });

        modelBuilder.Entity<OrdenFabricacion>(entity =>
        {
            entity.HasKey(e => e.CodProductoFabrica).HasName("PRIMARY");

            entity.ToTable("Orden_Fabricacion");

            entity.HasIndex(e => e.CodFabrica, "RefFabrica31");

            entity.HasIndex(e => e.CodMovimiento, "RefMovimiento91");

            entity.HasIndex(e => e.CodProducto, "RefProducto30");

            entity.Property(e => e.CodProductoFabrica).HasColumnName("cod_producto_fabrica");
            entity.Property(e => e.CantidadFabricacion).HasColumnName("cantidad_fabricacion");
            entity.Property(e => e.CodFabrica).HasColumnName("cod_fabrica");
            entity.Property(e => e.CodMovimiento).HasColumnName("cod_movimiento");
            entity.Property(e => e.CodProducto).HasColumnName("cod_producto");
            entity.Property(e => e.EstadoFabricacion)
                .HasMaxLength(150)
                .HasColumnName("estado_fabricacion");
            entity.Property(e => e.EstadoIngresoInventario)
                .HasMaxLength(150)
                .HasColumnName("estado_ingreso_inventario");
            entity.Property(e => e.FechaFabricacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fabricacion");
            entity.Property(e => e.Observacion)
                .HasMaxLength(250)
                .HasColumnName("observacion");

            entity.HasOne(d => d.CodFabricaNavigation).WithMany(p => p.OrdenFabricacions)
                .HasForeignKey(d => d.CodFabrica)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefFabrica31");

            entity.HasOne(d => d.CodMovimientoNavigation).WithMany(p => p.OrdenFabricacions)
                .HasForeignKey(d => d.CodMovimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefMovimiento91");

            entity.HasOne(d => d.CodProductoNavigation).WithMany(p => p.OrdenFabricacions)
                .HasForeignKey(d => d.CodProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefProducto30");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.CodPedido).HasName("PRIMARY");

            entity.ToTable("Pedido");

            entity.HasIndex(e => e.CodFactura, "RefFactura14");

            entity.HasIndex(e => e.CodUsuario, "RefUsuario12");

            entity.HasIndex(e => e.CodMetodoPago, "RefmetodoPago85");

            entity.Property(e => e.CodPedido).HasColumnName("cod_pedido");
            entity.Property(e => e.CodFactura).HasColumnName("cod_factura");
            entity.Property(e => e.CodMetodoPago).HasColumnName("cod_metodo_pago");
            entity.Property(e => e.CodUsuario).HasColumnName("cod_usuario");
            entity.Property(e => e.EstadoPedido)
                .HasMaxLength(250)
                .HasColumnName("estado_pedido");
            entity.Property(e => e.FechaPedido)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pedido");

            entity.HasOne(d => d.CodFacturaNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.CodFactura)
                .HasConstraintName("RefFactura14");

            entity.HasOne(d => d.CodMetodoPagoNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.CodMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefmetodoPago85");

            entity.HasOne(d => d.CodUsuarioNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.CodUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefUsuario12");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.CodProducto).HasName("PRIMARY");

            entity.ToTable("Producto");

            entity.HasIndex(e => e.CodCalidad, "RefCalidad39");

            entity.HasIndex(e => e.CodCategoria, "RefCategoria79");

            entity.HasIndex(e => e.CodColor, "RefColor37");

            entity.HasIndex(e => e.CodInventario, "RefInventario73");

            entity.HasIndex(e => e.CodMovimiento, "RefMovimiento87");

            entity.Property(e => e.CodProducto).HasColumnName("cod_producto");
            entity.Property(e => e.CodCalidad).HasColumnName("cod_calidad");
            entity.Property(e => e.CodCategoria).HasColumnName("cod_categoria");
            entity.Property(e => e.CodColor).HasColumnName("cod_color");
            entity.Property(e => e.CodInventario).HasColumnName("cod_inventario");
            entity.Property(e => e.CodMovimiento).HasColumnName("cod_movimiento");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoProducto)
                .HasMaxLength(150)
                .HasColumnName("estado_producto");
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(250)
                .HasColumnName("nombre_producto");
            entity.Property(e => e.PrecioCompra).HasColumnName("precio_compra");
            entity.Property(e => e.PrecioVenta).HasColumnName("precio_venta");

            entity.HasOne(d => d.CodCalidadNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CodCalidad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCalidad39");

            entity.HasOne(d => d.CodCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CodCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefCategoria79");

            entity.HasOne(d => d.CodColorNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CodColor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefColor37");

            entity.HasOne(d => d.CodInventarioNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CodInventario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefInventario73");

            entity.HasOne(d => d.CodMovimientoNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CodMovimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefMovimiento87");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.CodProveedor).HasName("PRIMARY");

            entity.ToTable("Proveedor");

            entity.Property(e => e.CodProveedor).HasColumnName("cod_proveedor");
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<RecepcionMercancium>(entity =>
        {
            entity.HasKey(e => e.CodProveedorMaterial).HasName("PRIMARY");

            entity.HasIndex(e => e.CodMaterial, "RefMaterial23");

            entity.HasIndex(e => e.CodProveedor, "RefProveedor24");

            entity.Property(e => e.CodProveedorMaterial).HasColumnName("cod_proveedor_material");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CantidadProducida)
                .HasColumnType("datetime")
                .HasColumnName("cantidad_producida");
            entity.Property(e => e.CodMaterial).HasColumnName("cod_material");
            entity.Property(e => e.CodProveedor).HasColumnName("cod_proveedor");

            entity.HasOne(d => d.CodMaterialNavigation).WithMany(p => p.RecepcionMercancia)
                .HasForeignKey(d => d.CodMaterial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefMaterial23");

            entity.HasOne(d => d.CodProveedorNavigation).WithMany(p => p.RecepcionMercancia)
                .HasForeignKey(d => d.CodProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefProveedor24");
        });

        modelBuilder.Entity<RolUsuario>(entity =>
        {
            entity.HasKey(e => e.CodRolUsuario).HasName("PRIMARY");

            entity.ToTable("Rol_Usuario");

            entity.Property(e => e.CodRolUsuario).HasColumnName("cod_rol_usuario");
            entity.Property(e => e.Rol)
                .HasMaxLength(100)
                .HasColumnName("rol");
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.CodSucursal).HasName("PRIMARY");

            entity.ToTable("Sucursal");

            entity.Property(e => e.CodSucursal).HasColumnName("cod_sucursal");
            entity.Property(e => e.Departamento)
                .HasMaxLength(250)
                .HasColumnName("departamento");
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .HasColumnName("direccion");
            entity.Property(e => e.Municipio)
                .HasMaxLength(250)
                .HasColumnName("municipio");
            entity.Property(e => e.NombreSucursal)
                .HasMaxLength(250)
                .HasColumnName("nombre_sucursal");
            entity.Property(e => e.Observacion)
                .HasMaxLength(250)
                .HasColumnName("observacion");
            entity.Property(e => e.Telefono)
                .HasMaxLength(250)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<TipMovimiento>(entity =>
        {
            entity.HasKey(e => e.CodTipoMovimiento).HasName("PRIMARY");

            entity.ToTable("tipMovimiento");

            entity.Property(e => e.CodTipoMovimiento).HasColumnName("cod_tipoMovimiento");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(250)
                .HasColumnName("tipo_movimiento");
        });

        modelBuilder.Entity<TipoMaterial>(entity =>
        {
            entity.HasKey(e => e.CodTipoMaterial).HasName("PRIMARY");

            entity.ToTable("TipoMaterial");

            entity.Property(e => e.CodTipoMaterial).HasColumnName("cod_tipo_material");
            entity.Property(e => e.NombreTipoMaterial)
                .HasMaxLength(250)
                .HasColumnName("nombre_tipo_material");
        });

        modelBuilder.Entity<TipoVentum>(entity =>
        {
            entity.HasKey(e => e.CodTipoVenta).HasName("PRIMARY");

            entity.ToTable("Tipo_venta");

            entity.Property(e => e.CodTipoVenta).HasColumnName("cod_tipo_venta");
            entity.Property(e => e.NombreTipoVenta)
                .HasMaxLength(150)
                .HasColumnName("nombre_tipo_venta");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.CodUsuario).HasName("PRIMARY");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.CodRolUsuario, "RefRol_Usuario69");

            entity.Property(e => e.CodUsuario).HasColumnName("cod_usuario");
            entity.Property(e => e.ActivarUsuario)
                .HasMaxLength(150)
                .HasColumnName("activar_usuario");
            entity.Property(e => e.Apellido1)
                .HasMaxLength(150)
                .HasColumnName("apellido1");
            entity.Property(e => e.Apellido2)
                .HasMaxLength(250)
                .HasColumnName("apellido2");
            entity.Property(e => e.CodRolUsuario).HasColumnName("cod_rol_usuario");
            entity.Property(e => e.Contacto)
                .HasMaxLength(50)
                .HasColumnName("contacto");
            entity.Property(e => e.Contrasenia)
                .HasMaxLength(250)
                .HasColumnName("contrasenia");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(150)
                .HasColumnName("direccion");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.Nit)
                .HasMaxLength(150)
                .HasColumnName("nit");
            entity.Property(e => e.Nombre1)
                .HasMaxLength(250)
                .HasColumnName("nombre1");
            entity.Property(e => e.Nombre2)
                .HasMaxLength(250)
                .HasColumnName("nombre2");
            entity.Property(e => e.OtrosNombres)
                .HasMaxLength(250)
                .HasColumnName("otros_nombres");
            entity.Property(e => e.RecContrasenia)
                .HasMaxLength(250)
                .HasColumnName("rec_contrasenia");

            entity.HasOne(d => d.CodRolUsuarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.CodRolUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefRol_Usuario69");
        });

        modelBuilder.Entity<Ventum>(entity =>
        {
            entity.HasKey(e => e.CodVenta).HasName("PRIMARY");

            entity.HasIndex(e => e.CodFactura, "RefFactura63");

            entity.HasIndex(e => e.CodMovimiento, "RefMovimiento86");

            entity.HasIndex(e => e.CodPedido, "RefPedido15");

            entity.HasIndex(e => e.CodProducto, "RefProducto20");

            entity.HasIndex(e => e.CodSucursal, "RefSucursal83");

            entity.HasIndex(e => e.CodTipoVenta, "RefTipo_venta96");

            entity.Property(e => e.CodVenta).HasColumnName("cod_venta");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CodFactura).HasColumnName("cod_factura");
            entity.Property(e => e.CodMovimiento).HasColumnName("cod_movimiento");
            entity.Property(e => e.CodPedido).HasColumnName("cod_pedido");
            entity.Property(e => e.CodProducto).HasColumnName("cod_producto");
            entity.Property(e => e.CodSucursal).HasColumnName("cod_sucursal");
            entity.Property(e => e.CodTipoVenta).HasColumnName("cod_tipo_venta");
            entity.Property(e => e.FechaPedido)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pedido");
            entity.Property(e => e.MetodoDePago)
                .HasMaxLength(150)
                .HasColumnName("metodo_de_pago");

            entity.HasOne(d => d.CodFacturaNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.CodFactura)
                .HasConstraintName("RefFactura63");

            entity.HasOne(d => d.CodMovimientoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.CodMovimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefMovimiento86");

            entity.HasOne(d => d.CodPedidoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.CodPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefPedido15");

            entity.HasOne(d => d.CodProductoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.CodProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefProducto20");

            entity.HasOne(d => d.CodSucursalNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.CodSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefSucursal83");

            entity.HasOne(d => d.CodTipoVentaNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.CodTipoVenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefTipo_venta96");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
