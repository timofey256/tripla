using System.Net.Http; 
using System.Text.RegularExpressions;
using System.Threading;

namespace Tripla;

class Program {
	private static void BuildApp(string[] args) {
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllersWithViews();

		var app = builder.Build();

		if (!app.Environment.IsDevelopment())
		{
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseRouting();

		app.MapControllerRoute(
			name: "default",
			pattern: "{controller}/{action=Index}/{id?}"
		);

		//app.MapFallbackToFile("index.html");

		app.Run();
	}
	
	public static void Main(string[] args) {
		BuildApp(args);
	}
}
