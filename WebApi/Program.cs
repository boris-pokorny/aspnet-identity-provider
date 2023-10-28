using Domain;
using JsonWebKeyStore;
using Persistence;
using TokenGenerator;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

services.AddControllers();

services.AddSwaggerGen();

services.AddJsonWebKeyStore();

services.AddPersistence();

services.AddTokenGenerator();

services.AddDomain();

var app = builder.Build();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger ();

// Enable middleware to serve swagger-ui
app.UseSwaggerUI (c => {
    c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Identity API V1");
});

app.UseRouting ();

app.MapControllers();

app.Run();