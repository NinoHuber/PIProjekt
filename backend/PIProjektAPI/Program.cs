using Microsoft.EntityFrameworkCore;
using PIProjekt.Data;
using PIProjekt.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddFiltering()
    .AddSorting(); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowVueApp",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(); // http://localhost:PORT/swagger
}

app.UseHttpsRedirection();
app.UseCors("AllowVueApp");
app.UseAuthorization();

app.MapControllers();

app.MapGraphQL(); //https://localhost:PORT/graphql

app.Run();