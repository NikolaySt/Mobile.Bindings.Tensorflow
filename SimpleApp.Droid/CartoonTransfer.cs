using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Java.Nio;
using SimpleApp.Core;
using tf = Xamarin.TensorFlow.Lite;

namespace SimpleApp.Droid
{
	public class CartoonTransfer : SuperModel, ICartoonFilter
	{
		public const int FloatSize = 4;

		public const int PixelSize = 3;

		private tf.Interpreter interpreter = default;

		public async Task<byte[]> ApplyAsync(
			byte[] imageBytes,
			Dicitionary<string, object> context)
		{
			await InitInterpeterAsync();

			var inoutTensors = GetInOutTensors(imageBytes);

			interpreter.Run(inoutTensors.Item1, inoutTensors.Item2);

			return GetImageAsJpegFromOutput(inoutTensors.Item2);
		}

		protected override async Task InitInterpeterAsync()
		{
			if (interpreter != null)
				return;

			interpreter = GetInterpreter("model_cartoon.tflite", false);

			//var index = interpreter.GetInputTensor(0).Index();
			//interpreter.ResizeInput(index, new int[] { 1, 50, 50, 3 });

			await Task.CompletedTask;
		}

		private Tuple<Java.Lang.Object, Java.Lang.Object> GetInOutTensors(byte[] imageBytes)
		{
			var tensor = interpreter.GetInputTensor(0);
			var shape = tensor.Shape();
			var width = shape[1];
			var height = shape[2];

			var content_input = GetPhotoAsByteBuffer(imageBytes, width, height);

			var transfer_outputs = GetOutputByteBuffer(width * height * PixelSize);

			return new Tuple<Java.Lang.Object, Java.Lang.Object>(content_input, transfer_outputs);
		}

		public byte[] GetImageAsJpegFromOutput(Java.Lang.Object output)
		{
			var tensor = interpreter.GetInputTensor(0);
			var shape = tensor.Shape();
			var width = shape[1];
			var height = shape[2];

			var outputTensor = output as ByteBuffer;

			using var bitmap_transfered = ByteBufferToImage(outputTensor, width, height);

			return bitmap_transfered.ExportAsJpeg();
		}

		private ByteBuffer GetPhotoAsByteBuffer(byte[] bytes, int newWidth, int newHeight)
		{
			var modelInputSize = FloatSize * newHeight * newWidth * PixelSize;
			using var bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);

			using var matrix = new Matrix();
			float scaleWidth = ((float)newWidth) / bitmap.Width;
			float scaleHeight = ((float)newHeight) / bitmap.Height;
			matrix.PostScale(Math.Min(scaleWidth, scaleHeight), Math.Min(scaleWidth, scaleHeight));

			using var resizedBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);

			using var outputimage = Bitmap.CreateBitmap(newWidth, newHeight, Bitmap.Config.Argb8888);
			var can = new Canvas(outputimage);
			can.DrawBitmap(resizedBitmap, (newWidth - resizedBitmap.Width) / 2, (newHeight - resizedBitmap.Height) / 2, null);

			//var resizedBitmap = Bitmap.CreateScaledBitmap(bitmap, newWidth, newHeight, true);

			var byteBuffer = ByteBuffer.AllocateDirect(modelInputSize);
			byteBuffer.Order(ByteOrder.NativeOrder());

			var pixels = new int[newWidth * newHeight];
			//resizedBitmap.GetPixels(pixels, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);
			outputimage.GetPixels(pixels, 0, outputimage.Width, 0, 0, outputimage.Width, outputimage.Height);
			bitmap.Recycle();

			var pixel = 0;
			for (var i = 0; i < pixels.Length; i++)
			{
				var pixelVal = pixels[pixel++];
				;
				byteBuffer.PutFloat(pixelVal.Red() / 127.5f - 1.0f);
				byteBuffer.PutFloat(pixelVal.Green() / 127.5f - 1.0f);
				byteBuffer.PutFloat(pixelVal.Blue() / 127.5f - 1.0f);
			}

			return byteBuffer;
		}

		private Bitmap ByteBufferToImage(ByteBuffer byteBuffer, int width, int height)
		{
			byteBuffer.Rewind();
			var pixels = new int[width * height];
			var index = 0;

			var size = byteBuffer.Limit();
			for (var i = 0; i < size; i += 12)
			{
				var r = (int)((byteBuffer.GetFloat(i) + 1.0f) * 127.5f);
				var g = (int)((byteBuffer.GetFloat(i + 4) + 1.0f) * 127.5f);
				var b = (int)((byteBuffer.GetFloat(i + 8) + 1.0f) * 127.5f);
				var a = 0xFF;

				pixels[index++] = a << 24 | r << 16 | g << 8 | b;
			}

			var bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

			bitmap.SetPixels(pixels, 0, width, 0, 0, width, height);

			return bitmap;
		}

		private ByteBuffer GetOutputByteBuffer(int outputSize)
		{
			var byteBuffer = ByteBuffer.AllocateDirect(FloatSize * outputSize);
			byteBuffer.Order(ByteOrder.NativeOrder());

			return byteBuffer;
		}
	}
}