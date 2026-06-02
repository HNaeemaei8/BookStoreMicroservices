using BookStore.Ordering.API.Extention;
using BookStore.Ordering.Infrastructure.Extensions;
using BookStore.Ordering.Infrastructure.Persistence;
using BookStore.Shared.API.DependencyInjection;
using BookStore.Shared.API.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Reflection;




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddOrderingApi();

builder.Services.AddInfrastructure(
    builder.Configuration);


builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<
        BookStore.Ordering.Application.AssemblyReference>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSharedApi();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

    db.Database.Migrate();
}

app.Run();