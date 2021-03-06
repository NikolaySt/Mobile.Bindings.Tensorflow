using System;

namespace Converter
{
	public static class ConvertHelper
	{
		public static byte[] ToByteArray(float[] values)
		{
			var size = sizeof(float) * values.Length;
			var result = new byte[size];
			var index = 0;
			foreach (var v in values)
			{
				var buff = BitConverter.GetBytes(v);
				Array.Copy(buff, 0, result, index, sizeof(float));
				index += sizeof(float);
			}

			return result;
		}

		public static float[] ToFloatArray(byte[] values)
		{
			var size =  values.Length / sizeof(float);
			var result = new float[size];
			var index = 0;
			for (var i = 0; i < values.Length; i+=4)
			{
				var buff = new[] { values[i], values[i+1], values[i+2], values[i+3] };
				var v = BitConverter.ToSingle(buff, 0);
				result[index++] = v;
			}

			return result;
		}
	}
}