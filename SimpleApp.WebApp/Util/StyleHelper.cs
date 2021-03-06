using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleApp.WebApp.Util
{
	public static class StyleHelper
	{
		public static string[] GetAvailableStyles()
		{
			var assembly = Assembly.GetAssembly(typeof(StyleHelper));
			var resourceName = $"{assembly.GetName().Name}.style_vectors_converted.txt";
			var resources = assembly.GetManifestResourceNames();
			Debug.WriteLine(string.Join(", ", resources));
			using var stream = assembly.GetManifestResourceStream(resourceName);
			using var reader = new StreamReader(stream);
			var text = reader.ReadToEnd();
			var result = text.Split("\r\n").ToArray();

			return result;
		}
	}
}