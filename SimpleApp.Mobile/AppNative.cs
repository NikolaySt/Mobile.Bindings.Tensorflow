using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.MobileBlazorBindings;
using SimpleApp.Core;
using SimpleApp.Mobile.Providers;
using Xamarin.Forms;

namespace SimpleApp.Mobile
{
	public class App : Application
	{
		public App(
			IFileProvider fileProvider = null,
			IStyleTransferFilter styleTransferFilter = null,
			ICartoonFilter cartoonFilter = null)
		{
			var hostBuilder = MobileBlazorBindingsHost.CreateDefaultBuilder()
				.ConfigureServices((hostContext, services) =>
				{
					// Adds web-specific services such as NavigationManager
					services.AddBlazorHybrid();

					// Register app-specific services
					services.AddScoped<IContentProvider, ContentProvider>();
					services.AddScoped<IImageProvider, ImageProvider>();

					if (styleTransferFilter != null)
						services.AddSingleton(styleTransferFilter);

					if (cartoonFilter != null)
						services.AddSingleton(cartoonFilter);
				})
				.UseWebRoot("wwwroot");

			if (fileProvider != null)
			{
				hostBuilder.UseStaticFiles(fileProvider);
			}
			else
			{
				hostBuilder.UseStaticFiles();
			}
			var host = hostBuilder.Build();

			MainPage = new ContentPage { Title = "SimpleApp Binding" };
			host.AddComponent<Main>(parent: MainPage);
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}