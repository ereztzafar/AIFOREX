using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// רישום שירותים – חובה כדי ש-Controllers יעבדו
builder.Services.AddControllers();

var app = builder.Build();

// התחלת מערכת ניתוב
app.UseRouting();

// חיבור ה-API למחלקות הבקר (כמו DashboardController)
app.MapControllers();

// הפעלת השרת
app.Run();
