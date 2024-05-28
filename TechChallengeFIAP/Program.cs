using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Microsoft.OpenApi.Models;
using TechChallengeFIAP.Middlewares;
using TechChallengeFIAP.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticSearch:Uri"]))
    {
        AutoRegisterTemplate = true,
    })
    .CreateLogger();

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// Criação do Midd que vai fazer a conexão com o banco de dados
builder.Services.AddDbContext<ContactDbContext>(options =>
 options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddControllersWithViews();



// Configuração Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Contact API",
        Version = "v1",
        Description = "API for managing contacts",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your-email@example.com"
        }
    });
    // Optionally include XML comments if you have XML documentation enabled
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Use custom request logging middleware
app.UseRequestLogging();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contact API V1");
    c.RoutePrefix = string.Empty; // Serve the UI at the app's root
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
