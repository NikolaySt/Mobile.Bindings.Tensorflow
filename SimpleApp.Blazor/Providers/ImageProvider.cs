using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using SimpleApp.Core;

namespace SimpleApp.Blazor.Providers
{
	public class ImageProvider : IImageProvider
	{
		public IJSRuntime JSRuntime { get; set; }

		public ImageProvider(IJSRuntime jsRuntime)
		{
			JSRuntime = jsRuntime;
		}

		public byte[] GetCurrentImage()
		{
			throw new NotImplementedException();
		}

		public async Task<byte[]> ApplyFilter(string filterName)
		{
			return await Task.FromResult(Array.Empty<byte>());
		}

		public async Task<byte[]> GetImageAsync()
		{
			using var source = await GetSourceAsync();
			return await Task.FromResult(Array.Empty<byte>());
		}

		public async Task<Stream> GetSourceAsync()
		{
			var JSObjRef = DotNetObjectReference.Create(this);

			await JSRuntime.InvokeVoidAsync("imageselector.openPicker", JSObjRef);

			return await Task.FromResult(new MemoryStream());
		}
	}
}