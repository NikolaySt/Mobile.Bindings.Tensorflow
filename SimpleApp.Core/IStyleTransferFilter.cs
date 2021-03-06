using System.Threading.Tasks;

namespace SimpleApp.Core
{
	public interface IStyleTransferFilter
	{
		Task<byte[]> ApplyAsync(
			byte[] imageBytes,
			string base64Style);
	}
}