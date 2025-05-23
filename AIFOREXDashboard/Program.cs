using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ����� ������� � ���� ��� �-Controllers �����
builder.Services.AddControllers();

var app = builder.Build();

// ����� ����� �����
app.UseRouting();

// ����� �-API ������� ���� (��� DashboardController)
app.MapControllers();

// ����� ����
app.Run();
