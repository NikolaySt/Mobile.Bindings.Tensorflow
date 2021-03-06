using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using SimpleApp.Core;

namespace SimpleApp.Mobile.Providers
{
	public class ContentProvider : IContentProvider
	{
		private readonly IFileProvider _provider;

		public ContentProvider(IFileProvider provider)
		{
			_provider = provider;
		}

		public async Task<Stream> GetStreamAsync(string url)
		{
			var fileInfo = _provider.GetFileInfo(url);

			if (fileInfo != null && fileInfo.Exists)
				return fileInfo.CreateReadStream();

			return await Task.FromResult(new MemoryStream());
		}
	}
}