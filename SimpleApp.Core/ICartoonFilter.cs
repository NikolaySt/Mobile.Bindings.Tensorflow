using System.Threading.Tasks;

namespace SimpleApp.Core
{
	public interface ICartoonFilter
	{
		Task<byte[]> ApplyAsync(
			byte[] imageBytes,
			Dicitionary<string, object> context);
	}
}