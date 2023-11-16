using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.Repositories;
using Play.Common.Settings;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
}
);
const string AllowedOriginSettings="AllowedOrigin";
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
builder.Services.AddMongo()
.AddMongoRepository<Item>("items")
.AddMassTransitWithRabbitMq();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<IItemRespository>(ItemRespository);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(b=>{
        b.WithOrigins(builder.Configuration[AllowedOriginSettings]??"")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
