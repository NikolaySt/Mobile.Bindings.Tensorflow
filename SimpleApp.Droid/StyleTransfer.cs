using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Java.IO;
using Java.Nio;
using Java.Nio.Channels;
using SimpleApp.Core;
using tf = Xamarin.TensorFlow.Lite;

namespace SimpleApp.Droid
{
	public class StyleTransfer : SuperModel, IStyleTransferFilter
	{
		public const int FloatSize = 4;

		public const int PixelSize = 3;

		public MappedByteBuffer mappedByteBuffer = default;

		public tf.Interpreter transfer_interpreter = default;

		public List<string> labels = default;

		private readonly int[] imageByteRange = new int[] { 0, 255 };

		private readonly int[] clipByteRange = new int[] { 0, 1 };

		public async Task<byte[]> ApplyAsync(
			byte[] imageBytes,
			string base64Style)
		{
			await InitInterpeterAsync();

			var inoutTensors = GetInOutTensors(imageBytes, base64Style);

			transfer_interpreter.RunForMultipleInputsOutputs(inoutTensors.Item1, inoutTensors.Item2);

			return GetImageAsJpegFromOutput(inoutTensors.Item2);
		}

		protected override async Task InitInterpeterAsync()
		{
			if (transfer_interpreter != null)
				return;

			//transfer_interpreter = GetInterpreter("model_transfer.tflite", false);

			transfer_interpreter = GetInterpreter("style_transfer_hybrid.tflite", false);
			var tensor = transfer_interpreter.GetInputTensor(0);
			transfer_interpreter.ResizeInput(tensor.Index(), new int[] { 1, 356, 356, 3 });

			await Task.CompletedTask;
		}

		private Tuple<Java.Lang.Object[], IDictionary<Java.Lang.Integer, Java.Lang.Object>> GetInOutTensors(byte[] imageBytes, string base64Style)
		{
			var tensor = transfer_interpreter.GetInputTensor(0);
			var shape = tensor.Shape();
			var width = shape[1];
			var height = shape[2];
			var content_input = GetPhotoAsByteBuffer(imageBytes, width, height);

			var transfer_outputs = GetOutputByteBuffer(width * height * PixelSize);

			var outputMap = new Dictionary<Java.Lang.Integer, Java.Lang.Object>
			{
				{ new Java.Lang.Integer(0), transfer_outputs }
			};

			var style = System.Convert.FromBase64String(base64Style);
			var style_outputs = style.ToByteBuffer();

			return new Tuple<Java.Lang.Object[], IDictionary<Java.Lang.Integer, Java.Lang.Object>>
				(new Java.Lang.Object[] { content_input, style_outputs }, outputMap);
		}

		public byte[] GetImageAsJpegFromOutput(IDictionary<Java.Lang.Integer, Java.Lang.Object> outputMap)
		{
			var tensor = transfer_interpreter.GetInputTensor(0);
			var shape = tensor.Shape();
			var width = shape[1];
			var height = shape[2];

			var outputTensor = outputMap.First().Value as ByteBuffer;

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
				byteBuffer.PutFloat(pixelVal.Red().Clip(imageByteRange, clipByteRange));
				byteBuffer.PutFloat(pixelVal.Green().Clip(imageByteRange, clipByteRange));
				byteBuffer.PutFloat(pixelVal.Blue().Clip(imageByteRange, clipByteRange));
			}

			return byteBuffer;
		}

		/// <summary>
		/// https://github.com/tensorflow/tensorflow/issues/34992
		/// </summary>
		/// <param name="byteBuffer"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		private Bitmap ByteBufferToImage(ByteBuffer byteBuffer, int width, int height)
		{
			byteBuffer.Rewind();
			var pixels = new int[width * height];
			var index = 0;

			var size = byteBuffer.Limit();
			for (var i = 0; i < size; i += 12)
			{
				var r = (int)byteBuffer.GetFloat(i).Clip(clipByteRange, imageByteRange);
				var g = (int)byteBuffer.GetFloat(i + 4).Clip(clipByteRange, imageByteRange);
				var b = (int)byteBuffer.GetFloat(i + 8).Clip(clipByteRange, imageByteRange);
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