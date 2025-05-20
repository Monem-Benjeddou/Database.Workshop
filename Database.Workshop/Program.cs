using Database.Workshop.Data;
using Database.Workshop.MigrationAdo;
using Database.Workshop.Models;
using Database.Workshop.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<EfUserRepository>();
builder.Services.AddScoped<AdoUserRepository>();
builder.Services.AddScoped<DapperUserRepository>();

builder.Services.AddScoped<RepositoryFactory>(sp =>
{
    var repoType = RepositoryType.Ado;
    return new RepositoryFactory(sp, repoType);
});

builder.Services.AddControllers();
// builder.Services.AddSingleton<AdoMigrationService>();

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var migrator = scope.ServiceProvider.GetRequiredService<AdoMigrationService>();
//     await migrator.EnsureDatabaseCreatedAsync();
// }
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

