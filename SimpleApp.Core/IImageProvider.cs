using System.IO;
using System.Threading.Tasks;

namespace SimpleApp.Core
{
	public interface IImageProvider
	{
		Task<byte[]> GetImageAsync();

		byte[] GetCurrentImage();

		Task<Stream> GetSourceAsync();

		Task<byte[]> ApplyFilter(string filterName);
	}
}