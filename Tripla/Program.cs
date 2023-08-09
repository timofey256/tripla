using System.Net.Http; 
using System.Text.RegularExpressions;

namespace Tripla;

class Program {
	private static void BuildApp(string[] args) {
		var builder = WebApplication.CreateBuilder(args);
	
		builder.Services.AddControllersWithViews();

		var app = builder.Build();
		app.UseHttpLogging();

		if (!app.Environment.IsDevelopment()) {
			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
		}
		
		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthorization();

		app.MapGet("/", () => "Hello World!");

		app.Run();
	}
	
	public static void Main(string[] args) {
		BuildApp(args);
	}
}
