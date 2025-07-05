// para poder usar las clases de tus modelos (Helado, Ingrediente, Estado).
using HeladeriaAPI.Models.Categoria;
using HeladeriaAPI.Models.Estado;
using HeladeriaAPI.Models.Helado;
using HeladeriaAPI.Models.Ingrediente;
//te permite heredar de DbContext y usar EF Core.
using Microsoft.EntityFrameworkCore;

namespace HeladeriaAPI.Config
{
    public class ApplicationDbContext : DbContext //Creás tu clase ApplicationDbContext, heredando de DbContext, que es la clase base de EF Core. Esto convierte tu clase en el "puente" entre tus modelos y la base de datos real.
    {
        //Constructor que recibe las opciones para configurar el contexto (como la cadena de conexión).
        //Luego las pasa al constructor base (base(options)), necesario para que EF Core funcione correctamente.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        //Estas tres líneas dicen: "Estas entidades se van a convertir en tablas".
        public DbSet<Helado> Helados { get; set; } = null!;
        public DbSet<Estado> Estados { get; set; } = null!;
        public DbSet<Ingrediente> Ingredientes { get; set; } = null!;
        public DbSet<IngredienteHelado> IngredienteHelado { get; set; } = null!;

        public DbSet<Categoria> Categorias { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder) //Este método se llama cuando se está creando el modelo de la base de datos. Usás modelBuilder para configurar las entidades, relaciones, índices, valores por defecto, etc.
        {

            modelBuilder.Entity<Helado>().HasIndex(h => h.nombreHelado).IsUnique();
            modelBuilder.Entity<Helado>().HasData(
                new Helado { Id = 1, nombreHelado = "Menta Granizada", Descripcion = "Helado de menta con chips de chocolate", Precio = 1500.00, IsArtesanal = false, EstadoId = 1, FechaCreacion = new DateTime(2024, 07, 01), CategoriaId = 2 },
                new Helado { Id = 2, nombreHelado = "Sambayon", Descripcion = "Helado de sambayon", Precio = 2500.00, IsArtesanal = false, EstadoId = 1, FechaCreacion = new DateTime(2024, 07, 01), CategoriaId = 3 },
                new Helado { Id = 3, nombreHelado = "Pastel de lima", Descripcion = "Helado de pastel de lima", Precio = 1000.00, IsArtesanal = true, EstadoId = 3, FechaCreacion = new DateTime(2024, 07, 01), CategoriaId = 1 }
                );


            modelBuilder.Entity<Estado>().HasIndex(e => e.nombreEstado).IsUnique(); //Crea un índice único en la columna Nombre de la tabla Estados. Así se evita que haya dos estados con el mismo nombre (por ejemplo, dos "Disponible").
            //Inserta estos tres registros automáticamente al crear o migrar la base de datos. Muy útil para tener datos básicos predefinidos que necesita tu sistema.
            modelBuilder.Entity<Estado>().HasData(
                new Estado { Id = 1, nombreEstado = "Disponible" },
                new Estado { Id = 2, nombreEstado = "Pendiente" },
                new Estado { Id = 3, nombreEstado = "No Disponible" }
            );
            modelBuilder.Entity<Ingrediente>().HasIndex(e => e.nombreIngrediente).IsUnique(); //Lo mismo, pero para los ingredientes. No se permite que se repita un nombre como "Azúcar".
            modelBuilder.Entity<Ingrediente>().HasData(
                new Ingrediente { Id = 1, nombreIngrediente = "default" },
                new Ingrediente { Id = 2, nombreIngrediente = "Leche" },
                new Ingrediente { Id = 3, nombreIngrediente = "Azucar" },
                new Ingrediente { Id = 4, nombreIngrediente = "Alcohol" },
                new Ingrediente { Id = 5, nombreIngrediente = "Chocolate" },
                new Ingrediente { Id = 6, nombreIngrediente = "Crema" }
            );
            // Para que la fecha de creación se establezca automáticamente al crear un nuevo helado
            modelBuilder.Entity<Helado>().Property(h => h.FechaCreacion).HasDefaultValueSql("GETUTCDATE()"); //Indica que cuando se cree un helado, si no se indica fecha, se usará GETUTCDATE() de SQL Server. Es decir, pone automáticamente la fecha de creación en UTC.         

            //Relación muchos a muchos (Helado-Ingrendiente)
            // Relación muchos a muchos explícita entre Helado e Ingrediente
            modelBuilder.Entity<IngredienteHelado>()
                .HasKey(ih => new { ih.HeladoId, ih.IngredienteId }); // ✅ clave compuesta

            modelBuilder.Entity<IngredienteHelado>()
                .HasOne(ih => ih.Helado)
                .WithMany(h => h.IngredienteHelado)
                .HasForeignKey(ih => ih.HeladoId);

            modelBuilder.Entity<IngredienteHelado>()
                .HasOne(ih => ih.Ingrediente)
                .WithMany()
                .HasForeignKey(ih => ih.IngredienteId);

            modelBuilder.Entity<IngredienteHelado>().HasData(
    new IngredienteHelado { HeladoId = 1, IngredienteId = 2 }, // Leche en Menta Granizada
    new IngredienteHelado { HeladoId = 1, IngredienteId = 5 }, // Chocolate en Menta Granizada
    new IngredienteHelado { HeladoId = 1, IngredienteId = 3 }, // Azúcar en Menta Granizada
    new IngredienteHelado { HeladoId = 2, IngredienteId = 1 }  // Default en Sambayon
);




            modelBuilder.Entity<Categoria>().HasIndex(e => e.nombreCategoria).IsUnique();
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, nombreCategoria = "Frutales" },
                new Categoria { Id = 2, nombreCategoria = "Granizados" },
                new Categoria { Id = 3, nombreCategoria = "Chocolate" },
                new Categoria { Id = 4, nombreCategoria = "Al agua" }
            );

            //¿Qué significa esto?
            //Define una relación muchos a muchos entre Helado e Ingrediente.
            //Un helado puede tener muchos ingredientes y un ingrediente puede estar en muchos helados.
            //En EF Core se necesita una tabla intermedia para eso → IngredienteHelado.

            //Detalles:
            //.HasMany(u => u.Ingredientes): un helado tiene muchos ingredientes.
            //.WithMany(): y cada ingrediente también está en muchos helados.
            //.UsingEntity<IngredienteHelado>(...): usás una entidad intermedia que vos definiste (IngredienteHelado) como tabla de unión.
            //Dentro del UsingEntity:
            //l => l.HasOne<Ingrediente>().WithMany().HasForeignKey(e => e.IngredienteId): la relación hacia Ingrediente.
            //r => r.HasOne<Helado>().WithMany().HasForeignKey(e => e.HeladoId): la relación hacia Helado.

            //En resumen:
            //Este método:
            //Crea índices únicos.
            //Inserta datos predeterminados (semillas).
            //Establece valores por defecto.
            //Define relaciones complejas como muchos a muchos.
        }
    }
}
