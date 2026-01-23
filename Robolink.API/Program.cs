var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); // Cần thiết để Swagger tìm thấy các Endpoint
builder.Services.AddSwaggerGen();           // ĐĂNG KÝ provider cho Swagger

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(); // Kích hoạt giao diện đồ họa https://localhost:7241/swagger/index.html
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
