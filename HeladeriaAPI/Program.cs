using HeladeriaAPI.Config;//para configuración como la base de datos.
using HeladeriaAPI.Services; //para poder registrar los servicios.
using HeladeriaAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; //para usar EF Core, la tecnología ORM que facilita el acceso a la base de datos.

var builder = WebApplication.CreateBuilder(args); //Se crea un builder para configurar la aplicación. Recibe argumentos de línea de comandos (args) que pueden usarse para configuración.

// Add services to the container.

//builder.Services.AddControllers(); 
//Aquí se agrega soporte para controladores (API controllers) a la app. Básicamente, te dice “vamos a usar controladores para manejar las peticiones HTTP”.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

//Se agregan servicios para generar documentación automática de la API con Swagger.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); //habilita exploración de endpoints para Swagger.
builder.Services.AddSwaggerGen(); //genera la documentación basada en los controladores y modelos.

// Para mostrar los errores de validación de manera personalizada
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
               kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? new string[0]
            );
return new BadRequestObjectResult(new ValidationErrorResponse(errors));
    };
});

// DB
//Se registra el contexto de base de datos ApplicationDbContext para usar SQL Server.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HeladeriaDB")); //lee la cadena de conexión desde appsettings.json. Esto habilita que la aplicación use EF Core con SQL Server para acceso a datos.
});


// Services
//Se registran los servicios para inyección de dependencias.
//AddScoped significa que cada petición HTTP recibe su propia instancia del servicio.
builder.Services.AddScoped<HeladoServices>();
builder.Services.AddScoped<IngredienteServices>();
builder.Services.AddScoped<EstadoServices>();
builder.Services.AddScoped<CategoriaServices>();

// Mapper
builder.Services.AddAutoMapper(typeof(Mapping));

//Se construye la aplicación con toda la configuración definida.
var app = builder.Build();
//app es ahora el objeto que representa la aplicación web lista para ejecutarse.

// Configure the HTTP request pipeline.
//Si la app está en modo desarrollo, activa Swagger para probar la API desde un navegador.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(config =>
{
    config.AllowAnyHeader();
    config.AllowAnyMethod();
    config.AllowAnyOrigin();
});

app.UseHttpsRedirection(); //Fuerza que las peticiones HTTP se redirijan a HTTPS para seguridad.

app.UseAuthorization(); //Añade el middleware de autorización. Aunque no tengas autenticación configurada aún, está listo para validar permisos.

app.MapControllers(); //Mapea las rutas para que las peticiones HTTP lleguen a los controladores correspondientes.

app.Run(); //Finalmente, ejecuta la aplicación y la pone a escuchar las peticiones.
