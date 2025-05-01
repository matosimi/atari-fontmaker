using System.Diagnostics;

namespace FontMaker;
public static class Compressors
{
	public static bool IsCompressorAvailable { get; set;}

	private static string TempPath { get; set; }
	private static string Zx0Exe { get; set; }

	// ==========================================================================
	// ZX0 compressor interface
	// ==========================================================================
	public static void Prepare()
	{
		try
		{
			TempPath = Path.Combine(Directory.GetCurrentDirectory(), "temp");
			Directory.CreateDirectory(TempPath);

			Zx0Exe = Path.Combine(TempPath, "zx0.exe");
			if (!File.Exists(Zx0Exe))
			{
				// Create the zx0.exe file
				var buffer = Helpers.GetResource<byte[]>("zx0.exe");
				using var fs = new FileStream(Zx0Exe, FileMode.Create, FileAccess.Write);
				fs.Write(buffer, 0, buffer.Length);
				fs.Close();
			}

			IsCompressorAvailable = File.Exists(Zx0Exe);
		}
		catch (Exception ex)
		{
			IsCompressorAvailable = false;
		}
	}

	public static byte[] Compress(byte[] data)
	{
		if (!IsCompressorAvailable) return data;

		try
		{
			var tempFile = Path.Combine(TempPath, "temp.zx0");
			using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
			{
				fs.Write(data, 0, data.Length);
				fs.Close();
			}

			// Make sure the output file doesn't exist
			var outputPath = Path.Combine(TempPath, "output.zx0");
			if (File.Exists(outputPath))
				File.Delete(outputPath);

			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = Zx0Exe,
					Arguments = $"-f {tempFile} {outputPath}",
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				}
			};
			process.Start();
			process.WaitForExit();
			var compressedData = File.ReadAllBytes(outputPath);
			File.Delete(outputPath);
			return compressedData;
		}
		catch (Exception ex)
		{
			// Handle the exception as needed
			return data;
		}
	}
}
