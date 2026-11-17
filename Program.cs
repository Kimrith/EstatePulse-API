using EstatePulse.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Database Context with Connection Pooling
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add Mediator
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

// 3. Add In-Memory Cache
builder.Services.AddMemoryCache();

// 4. Add Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serves avatar images uploaded to wwwroot
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

// Pre-warm DbContext model compilation & connection pool to eliminate 1.2s cold start latency
_ = Task.Run(async () =>
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _ = await context.Database.CanConnectAsync();
        _ = await context.Users.AsNoTracking().Take(1).ToListAsync();
    }
    catch
    {
        // Ignore warm-up failure on initial container boot
    }
});

app.Run();