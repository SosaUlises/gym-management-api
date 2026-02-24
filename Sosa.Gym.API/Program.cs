using Microsoft.EntityFrameworkCore;
using Sosa.Gym.API;
using Sosa.Gym.Application;
using Sosa.Gym.Application.Exceptions;
using Sosa.Gym.Common;
using Sosa.Gym.Infraestructure;
using Sosa.Gym.Infraestructure.DataBase;
using Sosa.Gym.Infraestructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// Controllers + filtro global
builder.Services.AddControllers(options => options.Filters.Add<ExceptionManager>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddWebApi()
    .AddCommon()
    .AddApplication()
    .AddInfraestructure(builder.Configuration);

// CORS
var allowedOrigins = "AllowedOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataBaseService>();
    await db.Database.MigrateAsync();
}

// Seed roles/admin
await IdentityDataSeed.SeedRolesAsync(app);

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sosa Gym v1");
    c.RoutePrefix = string.Empty;
});


app.UseCors(allowedOrigins);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
