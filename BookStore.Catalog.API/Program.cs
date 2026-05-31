using BookStore.Catalog.Application.Command;
using BookStore.Catalog.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateBookCommand).Assembly);
});

var app = builder.Build();

app.MapControllers();

app.Run();