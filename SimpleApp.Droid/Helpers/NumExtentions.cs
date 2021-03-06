using System.IO;
using Android.Graphics;
using Java.Nio;

namespace SimpleApp.Droid
{
	public static class NumExtentions
	{
		public static int Red(this int pixelVal)
		{
			return pixelVal >> 16 & 0xFF;
		}

		public static int Green(this int pixelVal)
		{
			return pixelVal >> 8 & 0xFF;
		}

		public static int Blue(this int pixelVal)
		{
			return pixelVal & 0xFF;
		}

		public static float Clip(this int value,
			int[] originalRange,
			int[] newRange)
		{
			var scale = (float)(newRange[1] - newRange[0]) / (originalRange[1] - originalRange[0]);
			return newRange[0] + ((value - originalRange[0]) * scale);
		}

		public static float Clip(this float value,
			int[] originalRange,
			int[] newRange)
		{
			var scale = (float)(newRange[1] - newRange[0]) / (originalRange[1] - originalRange[0]);
			return newRange[0] + ((value - originalRange[0]) * scale);
		}

		public static float[] GetBytes(this uint value)
		{
			return new float[4] {
					value & 0xFF,
					(value >> 8) & 0xFF,
					(value >> 16) & 0xFF,
					(value >> 24) & 0xFF };
		}

		public static unsafe float[] GetBytes(this float value)
		{
			var val = *((uint*)&value);
			return GetBytes(val);
		}

		public static byte[] ToByteArray(this ByteBuffer byteBuffer)
		{
			byteBuffer.Flip();  // Sets limit to current write position.

			var limit = byteBuffer.Limit();
			byteBuffer.Rewind(); // Already done by flip I think.

			var buffer = new byte[limit];
			byteBuffer.Get(buffer);

			return buffer;
		}

		public static ByteBuffer ToByteBuffer(this byte[] data)
		{
			var bf = ByteBuffer.Wrap(data);
			bf.Rewind();

			return bf;
		}

		public static void SaveToJpeg(this Bitmap bitmap, string filename)
		{
			var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
			var filePath = System.IO.Path.Combine(sdCardPath, filename);
			using var stream = new FileStream(filePath, FileMode.Create);
			bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
			stream.Close();
		}

		public static byte[] ExportAsJpeg(this Bitmap bitmap)
		{
			using var stream = new MemoryStream();
			bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
			return stream.ToArray();
		}
	}
}