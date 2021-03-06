using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Converter
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			ConvertVectoreToBase64AndSave();
			Console.WriteLine("Done");
			Console.ReadLine();
		}

		private static void ConvertVectoreToBase64AndSave()
		{
			var vectore_lines = File.ReadAllLines("style_vectors.txt");

			var output_filename = "style_vectors_converted.txt";

			var output = new List<string>();
			foreach (var line in vectore_lines)
			{
				var vector = line.Split(",").Select(it => (float)Convert.ToDouble(it)).ToArray();
				var buffer = ConvertHelper.ToByteArray(vector);
				var result = Convert.ToBase64String(buffer);
				output.Add(result);

				//Console.WriteLine(string.Join(",", vector.Select(it => $"{it}")));
				//Console.WriteLine(string.Join(",", ConvertHelper.ToFloatArray(buffer).Select(it => $"{it}")));
				//Console.WriteLine();
			}

			if (File.Exists(output_filename))
				File.Delete(output_filename);

			File.WriteAllLines(output_filename, output);
		}
	}
}