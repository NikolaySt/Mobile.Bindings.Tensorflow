using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SimpleApp.Core;

namespace SimpleApp.Blazor.Providers
{
	public class ContentProvider : IContentProvider
	{
		private readonly HttpClient _http;

		public ContentProvider(IServiceCollection services)
		{
			_http = services.BuildServiceProvider().GetRequiredService<HttpClient>();
		}

		public Task<Stream> GetStreamAsync(string url)
		{
			return _http.GetStreamAsync(url);
		}
	}
}