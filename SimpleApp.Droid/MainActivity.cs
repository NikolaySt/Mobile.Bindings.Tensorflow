using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.MobileBlazorBindings.WebView.Android;
using SimpleApp.Mobile;
using SimpleApp.Core;
using Android.Content.Res;

namespace SimpleApp.Droid
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			BlazorHybridAndroid.Init();

			var fileProvider = new AssetFileProvider(Assets, "wwwroot");
			var styleTransfer = new StyleTransfer();
			var cartoonTransfer = new CartoonTransfer();

			base.OnCreate(bundle);

			Xamarin.Essentials.Platform.Init(this, bundle);
			global::Xamarin.Forms.Forms.Init(this, bundle);
			LoadApplication(new App(fileProvider, styleTransfer, cartoonTransfer));
		}

		public override void OnRequestPermissionsResult(
			int requestCode,
			string[] permissions,
			[GeneratedEnum] Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}