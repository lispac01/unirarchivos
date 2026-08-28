using Microsoft.EntityFrameworkCore;
using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Data;

public class MantenimientoDbContext : DbContext
{
    public MantenimientoDbContext(DbContextOptions<MantenimientoDbContext> options)
        : base(options)
    {
    }

    public DbSet<CtClienteMtto> ClientesMtto => Set<CtClienteMtto>();
    public DbSet<CtMContenedor> Contenedores => Set<CtMContenedor>();
    public DbSet<CtEspMtto> EspecialidadesMtto => Set<CtEspMtto>();
    public DbSet<CtEspDelEmp> EspecialidadesEmpleado => Set<CtEspDelEmp>();
    public DbSet<CppEspDelEmp> CppEspecialidadesEmpleado => Set<CppEspDelEmp>();
    public DbSet<TrIngresoContenedor> IngresosContenedor => Set<TrIngresoContenedor>();
    public DbSet<TrPreviajeContenedor> PreviajesContenedor => Set<TrPreviajeContenedor>();
    public DbSet<CtTareaDeMtto> TareasMtto => Set<CtTareaDeMtto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CtClienteMtto>(entity =>
        {
            entity.ToTable("ct_clientemtto");

            entity.HasKey(x => x.CodCliente);

            entity.Property(x => x.CodCliente)
                .HasColumnName("cod_cliente")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.NombreCliente)
                .HasColumnName("nombre_cliente")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.CodDpto)
                .HasColumnName("cod_dpto")
                .HasMaxLength(20)
                .IsRequired(false);

            entity.Property(x => x.ImpMovMo)
                .HasColumnName("imp_mov_mo")
                .HasPrecision(18, 2);

            entity.Property(x => x.ImpMovMo2)
                .HasColumnName("imp_mov_mo2")
                .HasPrecision(18, 2);

            entity.Property(x => x.Activo)
                .HasColumnName("activo");
        });

        modelBuilder.Entity<CtMContenedor>(entity =>
        {
            entity.ToTable("ct_mcontenedor");

            entity.HasKey(x => x.CodContenedor);

            entity.Property(x => x.CodContenedor)
                .HasColumnName("cod_contenedor")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.CodCliente)
                .HasColumnName("cod_cliente")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.Activo)
                .HasColumnName("activo");

            entity.HasOne(x => x.Cliente)
                .WithMany()
                .HasForeignKey(x => x.CodCliente)
                .HasConstraintName("FK_ct_mcontenedor_ct_clientemtto")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CtEspMtto>(entity =>
        {
            entity.ToTable("ct_espmtto");

            entity.HasKey(x => x.CodEspMtto);

            entity.Property(x => x.CodEspMtto)
                .HasColumnName("cod_esp_mtto")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.NomEspMtto)
                .HasColumnName("nom_esp_mtto")
                .HasMaxLength(150)
                .IsRequired();
        });

        modelBuilder.Entity<CtEspDelEmp>(entity =>
        {
            entity.ToTable("ct_espdelemp");

            entity.HasKey(x => x.CodTit);

            entity.Property(x => x.CodTit)
                .HasColumnName("cod_tit")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.NomTit)
                .HasColumnName("nom_tit")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Usuario)
                .HasColumnName("usuario")
                .HasMaxLength(50)
                .IsRequired(false);
        });

        modelBuilder.Entity<CppEspDelEmp>(entity =>
        {
            entity.ToTable("cpp_espdelemp");

            entity.HasKey(x => new { x.CodTit, x.CodEspMtto });

            entity.Property(x => x.CodTit)
                .HasColumnName("cod_tit")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.CodEspMtto)
                .HasColumnName("cod_esp_mtto")
                .HasMaxLength(20)
                .IsRequired();

            entity.HasOne(x => x.Tecnico)
                .WithMany(x => x.EspecialidadesAsignadas)
                .HasForeignKey(x => x.CodTit)
                .HasConstraintName("FK_cpp_espdelemp_ct_espdelemp")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Especialidad)
                .WithMany()
                .HasForeignKey(x => x.CodEspMtto)
                .HasConstraintName("FK_cpp_espdelemp_ct_espmtto")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TrIngresoContenedor>(entity =>
        {
            entity.ToTable("tr_ingresocontenedor");

            entity.HasKey(x => x.NumIngreso);

            entity.Property(x => x.NumIngreso)
                .HasColumnName("num_ingreso")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.CodContenedor)
                .HasColumnName("cod_contenedor")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.FecIngreso)
                .HasColumnName("fec_ingreso")
                .HasColumnType("datetime2")
                .IsRequired();

            entity.HasOne(x => x.Contenedor)
                .WithMany()
                .HasForeignKey(x => x.CodContenedor)
                .HasConstraintName("FK_tr_ingresocontenedor_ct_mcontenedor")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TrPreviajeContenedor>(entity =>
        {
            entity.ToTable("tr_previajecontenedor");

            entity.HasKey(x => x.NroPreviaje);

            entity.HasIndex(x => new { x.NumIngreso, x.CodEspMtto })
                .IsUnique();

            entity.Property(x => x.NroPreviaje)
                .HasColumnName("nro_previaje")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.NumIngreso)
                .HasColumnName("num_ingreso")
                .IsRequired();

            entity.Property(x => x.CodEspMtto)
                .HasColumnName("cod_esp_mtto")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.CodTit)
                .HasColumnName("cod_tit")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.FecPreviaje)
                .HasColumnName("fec_previaje")
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(x => x.Observaciones)
                .HasColumnName("observaciones")
                .HasMaxLength(1000)
                .IsRequired(false);

            entity.Property(x => x.Habilitado)
                .HasColumnName("habilitado");

            entity.HasOne(x => x.Ingreso)
                .WithMany()
                .HasForeignKey(x => x.NumIngreso)
                .HasConstraintName("FK_tr_previajecontenedor_tr_ingresocontenedor")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Especialidad)
                .WithMany()
                .HasForeignKey(x => x.CodEspMtto)
                .HasConstraintName("FK_tr_previajecontenedor_ct_espmtto")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tecnico)
                .WithMany()
                .HasForeignKey(x => x.CodTit)
                .HasConstraintName("FK_tr_previajecontenedor_ct_espdelemp")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CtTareaDeMtto>(entity =>
        {
            entity.ToTable("ct_tareademtto");

            entity.HasKey(x => x.NroTarea);

            entity.Property(x => x.NroTarea)
                .HasColumnName("nro_tarea")
                .ValueGeneratedNever();

            entity.Property(x => x.NombreTarea)
                .HasColumnName("nombre_tarea")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.CodEspMtto)
                .HasColumnName("cod_esp_mtto")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.TiempoEstimado)
                .HasColumnName("tiempo_estimado")
                .HasPrecision(6, 2)
                .IsRequired();

            entity.Property(x => x.Activo)
                .HasColumnName("activo");

            entity.HasOne(x => x.Especialidad)
                .WithMany()
                .HasForeignKey(x => x.CodEspMtto)
                .HasConstraintName("FK_ct_tareademtto_ct_espmtto")
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
