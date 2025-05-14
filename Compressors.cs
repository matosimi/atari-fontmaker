using System.Diagnostics;

namespace FontMaker;

public static class Compressors
{
	public enum CompressorType
	{
		ZX0 = 0,
		ZX1 = 1,
		ZX2 = 2,
		APULTRA = 3,
	}

	private static bool IsCompressor0Available { get; set;}
	private static bool IsCompressor1Available { get; set; }
	private static bool IsCompressor2Available { get; set; }
	private static bool IsCompressor3Available { get; set; }

	private static string TempPath { get; set; }
	private static string Zx0Exe { get; set; }
	private static string Zx1Exe { get; set; }
	private static string Zx2Exe { get; set; }
	private static string ApultraExe { get; set; }

	// ==========================================================================
	// ZX0 compressor interface
	// ==========================================================================
	public static void Prepare()
	{
		try
		{
			TempPath = Path.Combine(Path.GetTempPath(), "afm2025");
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

			Zx1Exe = Path.Combine(TempPath, "zx1.exe");
			if (!File.Exists(Zx1Exe))
			{
				// Create the zx1.exe file
				var buffer = Helpers.GetResource<byte[]>("zx1.exe");
				using var fs = new FileStream(Zx1Exe, FileMode.Create, FileAccess.Write);
				fs.Write(buffer, 0, buffer.Length);
				fs.Close();
			}

			Zx2Exe = Path.Combine(TempPath, "zx2.exe");
			if (!File.Exists(Zx2Exe))
			{
				// Create the zx2.exe file
				var buffer = Helpers.GetResource<byte[]>("zx2.exe");
				using var fs = new FileStream(Zx2Exe, FileMode.Create, FileAccess.Write);
				fs.Write(buffer, 0, buffer.Length);
				fs.Close();
			}

			ApultraExe = Path.Combine(TempPath, "apultra.exe");
			if (!File.Exists(ApultraExe))
			{
				// Create the apultra.exe file
				var buffer = Helpers.GetResource<byte[]>("apultra.exe");
				using var fs = new FileStream(ApultraExe, FileMode.Create, FileAccess.Write);
				fs.Write(buffer, 0, buffer.Length);
				fs.Close();
			}

			IsCompressor0Available = File.Exists(Zx0Exe);
			IsCompressor1Available = File.Exists(Zx1Exe);
			IsCompressor2Available = File.Exists(Zx2Exe);
			IsCompressor3Available = File.Exists(ApultraExe);
		}
		catch (Exception ex)
		{
			IsCompressor0Available = false;
			IsCompressor1Available = false;
			IsCompressor2Available = false;
			IsCompressor3Available = false;
		}
	}

	public static byte[] Compress(byte[] data, CompressorType which)
	{
		var (available, exe, cmdLine) = which switch
		{
			CompressorType.ZX0 => (IsCompressor0Available, Zx0Exe, "-f"),
			CompressorType.ZX1 => (IsCompressor1Available, Zx1Exe, "-f"),
			CompressorType.ZX2 => (IsCompressor2Available, Zx2Exe, "-f"),
			CompressorType.APULTRA => (IsCompressor2Available, ApultraExe, string.Empty),
			_ => (false, null, string.Empty)
		};
		if (!available) return data;

		try
		{
			var tempFile = Path.Combine(TempPath, "temp.zx");
			using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
			{
				fs.Write(data, 0, data.Length);
				fs.Close();
			}

			// Make sure the output file doesn't exist
			var outputPath = Path.Combine(TempPath, "output.zx");
			if (File.Exists(outputPath))
				File.Delete(outputPath);

			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = exe,
					Arguments = $"{cmdLine} {tempFile} {outputPath}",
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

	public static string GetName(CompressorType which)
	{
		return which switch
		{
			CompressorType.ZX0 => "ZX0",
			CompressorType.ZX1 => "ZX1",
			CompressorType.ZX2 => "ZX2",
			CompressorType.APULTRA => "apultra",
			_ => "Unknown"
		};

	}
}
