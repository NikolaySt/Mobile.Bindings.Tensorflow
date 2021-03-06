using System;
using System.IO;
using System.Resources;
using System.Threading.Tasks;

namespace SimpleApp.Core
{
	public interface IContentProvider
	{
		Task<Stream> GetStreamAsync(string url);
	}
}