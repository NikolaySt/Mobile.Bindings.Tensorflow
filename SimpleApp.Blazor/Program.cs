using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using SimpleApp.Blazor.Providers;
using SimpleApp.Core;
using SimpleApp.WebApp;

namespace SimpleApp.Blazor
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
			builder.Services.AddScoped<IImageProvider, ImageProvider>();
			builder.Services.AddScoped<IContentProvider>(sr => new ContentProvider(builder.Services));

			await builder.Build().RunAsync();
		}
	}
}