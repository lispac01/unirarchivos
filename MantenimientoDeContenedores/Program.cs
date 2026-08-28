using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MantenimientoDeContenedores.Data;
using MantenimientoDeContenedores.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se configuró la cadena de conexión DefaultConnection.");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MantenimientoDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IContenedorRepository, SqlContenedorRepository>();
builder.Services.AddScoped<IClienteMttoRepository, SqlClienteMttoRepository>();
builder.Services.AddScoped<IEspecialidadMttoRepository, SqlEspecialidadMttoRepository>();
builder.Services.AddScoped<IEspecialidadEmpleadoRepository, SqlEspecialidadEmpleadoRepository>();
builder.Services.AddScoped<IIngresoContenedorRepository, SqlIngresoContenedorRepository>();
builder.Services.AddScoped<IPreviajeContenedorRepository, SqlPreviajeContenedorRepository>();
builder.Services.AddScoped<ITareaMttoRepository, SqlTareaMttoRepository>();

var app = builder.Build();

EnsureDatabaseExists(connectionString);
EnsureApplicationSchema(connectionString);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void EnsureDatabaseExists(string connectionString)
{
    var sqlBuilder = new SqlConnectionStringBuilder(connectionString);
    var databaseName = sqlBuilder.InitialCatalog;

    if (string.IsNullOrWhiteSpace(databaseName))
    {
        throw new InvalidOperationException("La cadena de conexión debe incluir el nombre de la base de datos.");
    }

    var escapedDatabaseName = databaseName.Replace("]", "]]", StringComparison.Ordinal);
    sqlBuilder.InitialCatalog = "master";

    using var connection = new SqlConnection(sqlBuilder.ConnectionString);
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = $"IF DB_ID(N'{databaseName.Replace("'", "''", StringComparison.Ordinal)}') IS NULL CREATE DATABASE [{escapedDatabaseName}]";
    command.ExecuteNonQuery();
}

static void EnsureApplicationSchema(string connectionString)
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = @"
IF OBJECT_ID(N'dbo.ct_clientemtto', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ct_clientemtto]
    (
        [cod_cliente] NVARCHAR(20) NOT NULL,
        [nombre_cliente] NVARCHAR(150) NOT NULL,
        [cod_dpto] NVARCHAR(20) NULL,
        [imp_mov_mo] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_ct_clientemtto_imp_mov_mo] DEFAULT ((0)),
        [imp_mov_mo2] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_ct_clientemtto_imp_mov_mo2] DEFAULT ((0)),
        [activo] BIT NOT NULL CONSTRAINT [DF_ct_clientemtto_activo] DEFAULT ((1)),
        CONSTRAINT [PK_ct_clientemtto] PRIMARY KEY ([cod_cliente])
    )
END

IF OBJECT_ID(N'dbo.ct_mcontenedor', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ct_mcontenedor]
    (
        [cod_contenedor] NVARCHAR(20) NOT NULL,
        [nombre] NVARCHAR(150) NOT NULL,
        [cod_cliente] NVARCHAR(20) NOT NULL,
        [activo] BIT NOT NULL CONSTRAINT [DF_ct_mcontenedor_activo] DEFAULT ((1)),
        CONSTRAINT [PK_ct_mcontenedor] PRIMARY KEY ([cod_contenedor]),
        CONSTRAINT [FK_ct_mcontenedor_ct_clientemtto] FOREIGN KEY ([cod_cliente]) REFERENCES [dbo].[ct_clientemtto]([cod_cliente])
    )
END

IF OBJECT_ID(N'dbo.ct_espmtto', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ct_espmtto]
    (
        [cod_esp_mtto] NVARCHAR(20) NOT NULL,
        [nom_esp_mtto] NVARCHAR(150) NOT NULL,
        CONSTRAINT [PK_ct_espmtto] PRIMARY KEY ([cod_esp_mtto])
    )
END

IF OBJECT_ID(N'dbo.ct_espdelemp', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ct_espdelemp]
    (
        [cod_tit] NVARCHAR(20) NOT NULL,
        [nom_tit] NVARCHAR(150) NOT NULL,
        [usuario] NVARCHAR(50) NULL,
        CONSTRAINT [PK_ct_espdelemp] PRIMARY KEY ([cod_tit])
    )
END

IF OBJECT_ID(N'dbo.cpp_espdelemp', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[cpp_espdelemp]
    (
        [cod_tit] NVARCHAR(20) NOT NULL,
        [cod_esp_mtto] NVARCHAR(20) NOT NULL,
        CONSTRAINT [PK_cpp_espdelemp] PRIMARY KEY ([cod_tit], [cod_esp_mtto]),
        CONSTRAINT [FK_cpp_espdelemp_ct_espdelemp] FOREIGN KEY ([cod_tit]) REFERENCES [dbo].[ct_espdelemp]([cod_tit]) ON DELETE CASCADE,
        CONSTRAINT [FK_cpp_espdelemp_ct_espmtto] FOREIGN KEY ([cod_esp_mtto]) REFERENCES [dbo].[ct_espmtto]([cod_esp_mtto])
    )
END

IF OBJECT_ID(N'dbo.tr_ingresocontenedor', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[tr_ingresocontenedor]
    (
        [num_ingreso] INT IDENTITY(1,1) NOT NULL,
        [cod_contenedor] NVARCHAR(20) NOT NULL,
        [fec_ingreso] DATETIME2 NOT NULL,
        CONSTRAINT [PK_tr_ingresocontenedor] PRIMARY KEY ([num_ingreso]),
        CONSTRAINT [FK_tr_ingresocontenedor_ct_mcontenedor] FOREIGN KEY ([cod_contenedor]) REFERENCES [dbo].[ct_mcontenedor]([cod_contenedor])
    )
END

IF OBJECT_ID(N'dbo.tr_previajecontenedor', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[tr_previajecontenedor]
    (
        [nro_previaje] INT IDENTITY(1,1) NOT NULL,
        [num_ingreso] INT NOT NULL,
        [cod_esp_mtto] NVARCHAR(20) NOT NULL,
        [cod_tit] NVARCHAR(20) NOT NULL,
        [fec_previaje] DATETIME2 NOT NULL,
        [observaciones] NVARCHAR(1000) NULL,
        [habilitado] BIT NOT NULL CONSTRAINT [DF_tr_previajecontenedor_habilitado] DEFAULT ((0)),
        CONSTRAINT [PK_tr_previajecontenedor] PRIMARY KEY ([nro_previaje]),
        CONSTRAINT [FK_tr_previajecontenedor_tr_ingresocontenedor] FOREIGN KEY ([num_ingreso]) REFERENCES [dbo].[tr_ingresocontenedor]([num_ingreso]),
        CONSTRAINT [FK_tr_previajecontenedor_ct_espmtto] FOREIGN KEY ([cod_esp_mtto]) REFERENCES [dbo].[ct_espmtto]([cod_esp_mtto]),
        CONSTRAINT [FK_tr_previajecontenedor_ct_espdelemp] FOREIGN KEY ([cod_tit]) REFERENCES [dbo].[ct_espdelemp]([cod_tit])
    )
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_tr_previajecontenedor_num_ingreso_cod_esp_mtto'
        AND object_id = OBJECT_ID(N'dbo.tr_previajecontenedor')
)
BEGIN
    CREATE UNIQUE INDEX [UX_tr_previajecontenedor_num_ingreso_cod_esp_mtto]
        ON [dbo].[tr_previajecontenedor]([num_ingreso], [cod_esp_mtto])
END

IF OBJECT_ID(N'dbo.ct_tareademtto', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ct_tareademtto]
    (
        [nro_tarea] INT NOT NULL,
        [nombre_tarea] NVARCHAR(200) NOT NULL,
        [cod_esp_mtto] NVARCHAR(20) NOT NULL,
        [tiempo_estimado] DECIMAL(6,2) NOT NULL CONSTRAINT [DF_ct_tareademtto_tiempo_estimado] DEFAULT ((0)),
        [activo] BIT NOT NULL CONSTRAINT [DF_ct_tareademtto_activo] DEFAULT ((1)),
        CONSTRAINT [PK_ct_tareademtto] PRIMARY KEY ([nro_tarea]),
        CONSTRAINT [FK_ct_tareademtto_ct_espmtto] FOREIGN KEY ([cod_esp_mtto]) REFERENCES [dbo].[ct_espmtto]([cod_esp_mtto])
    )
END";
    command.ExecuteNonQuery();
}
