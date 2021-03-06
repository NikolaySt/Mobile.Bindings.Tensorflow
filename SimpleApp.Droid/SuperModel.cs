using System;
using System.Threading.Tasks;
using Android.App;
using Java.IO;
using Java.Nio;
using Java.Nio.Channels;
using tf = Xamarin.TensorFlow.Lite;

namespace SimpleApp.Droid
{
	public abstract class SuperModel
	{
		protected abstract Task InitInterpeterAsync();

		public static tf.Interpreter GetInterpreter(string modelFile, bool gpu = true)
		{
			if (string.IsNullOrWhiteSpace(modelFile))
				throw new ArgumentException($"'{nameof(modelFile)}' cannot be null or whitespace", nameof(modelFile));

			var assetDescriptor = Application.Context.Assets.OpenFd(modelFile);
			var inputStream = new FileInputStream(assetDescriptor.FileDescriptor);

			ByteBuffer byteBuffer = inputStream.Channel.Map(
				FileChannel.MapMode.ReadOnly,
				assetDescriptor.StartOffset,
				assetDescriptor.DeclaredLength);

			if (gpu)
			{
				// with GPU
				var gpuDelegate = new tf.GPU.GpuDelegate();
				var options = new tf.Interpreter.Options().AddDelegate(gpuDelegate);
				return new tf.Interpreter(byteBuffer, options);
			}
			else
			{
				return new tf.Interpreter(byteBuffer);
			}
		}
	}
}