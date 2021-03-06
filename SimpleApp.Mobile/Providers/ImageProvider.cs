using System;
using System.IO;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SimpleApp.Core;

namespace SimpleApp.Mobile.Providers
{
	public class ImageProvider : IImageProvider
	{
		private byte[] _currentImages;

		public async Task<byte[]> GetImageAsync()
		{
			using var source = await GetSourceAsync();
			using var ms = new MemoryStream();
			source.Position = 0;
			source.CopyTo(ms);
			source.Position = 0;
			_currentImages = ms.ToArray();

			return _currentImages;
		}

		public byte[] GetCurrentImage()
		{
			return _currentImages ?? Array.Empty<byte>();
		}

		public async Task<byte[]> ApplyFilter(string filterName)
		{
			//Process the image
			//_currentImages

			return await Task.FromResult(_currentImages);
		}

		public async Task<Stream> GetSourceAsync()
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				return await Task.FromResult(new MemoryStream());
			}

			var mediaOptions = new PickMediaOptions()
			{
				PhotoSize = PhotoSize.Medium
			};

			var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

			if (selectedImageFile == null)
			{
				return await Task.FromResult(new MemoryStream());
			}

			return selectedImageFile.GetStream();
		}
	}
}