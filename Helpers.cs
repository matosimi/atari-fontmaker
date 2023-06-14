using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FontMaker
{
	public static class Helpers
	{
		public static byte AtariConvertChar(byte character)
		{
			if (character == 32)
			{
				return 0;
			}

			if (character is >= 48 and <= 90)
			{
				return (byte)(character - 32);
			}

			return character;
		}

		public static Image GetImage(PictureBox src)
		{
			var w = src.Bounds.Width;
			var h = src.Bounds.Height;

			if (src.Image != null)
				return src.Image;

			src.Image = new Bitmap(w, h);

			return src.Image;
		}

		public static Image NewImage(PictureBox src)
		{
			var w = src.Bounds.Width;
			var h = src.Bounds.Height;

			if (src.Image != null)
				src.Image.Dispose();

			src.Image = new Bitmap(w, h);

			return src.Image;
		}

		/// <summary>
		/// Open a given url in the default browser
		/// </summary>
		/// <param name="url">url to open</param>
		public static void OpenUrl(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch
			{
				// hack because of this: https://github.com/dotnet/corefx/issues/10361
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					Process.Start("xdg-open", url);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					Process.Start("open", url);
				}
				else
				{
					throw;
				}
			}
		}

		public static void GetBuildInfo(ref int v1, ref int v2, ref int v3, ref int v4)
		{
			try
			{
				var thisAssembly = typeof(Helpers).Assembly;
				var thisAssemblyName = thisAssembly.GetName();

				var ver = thisAssemblyName.Version;
				if (ver == null) return;
				v1 = ver.Major;
				v2 = ver.Minor;
				v3 = ver.Build;
				v4 = ver.Revision;
			}
			catch (Exception)
			{
				// ignored
			}
		}

		public static string GetBuildInfoAsString()
		{
			var v1 = 0;
			var v2 = 0;
			var v3 = 0;
			var v4 = 0;
			GetBuildInfo(ref v1, ref v2, ref v3, ref v4);
			return $"{v1}.{v2}.{v3}.{v4}";
		}

		public static T GetResource<T>(string shortResourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = $"FontMaker.Resources.{shortResourceName}";
			if (typeof(T) == typeof(string))
			{
				using var stream = assembly.GetManifestResourceStream(resourceName);
				using var reader = new StreamReader(stream);
				return (T)(object)reader.ReadToEnd();
			}
			else if (typeof(T) == typeof(byte[]))
			{
				using var stream = assembly.GetManifestResourceStream(resourceName);
				using var reader = new BinaryReader(stream);
				return (T)(object)reader.ReadBytes((int)stream.Length);
			}
			else
				throw new Exception("Unsupported resource object type");
		}

		/// <summary>
		/// Find the AtariPalette entry that best approximates the request RGB color
		/// </summary>
		/// <param name="rr">Red color component</param>
		/// <param name="gg">Green color component</param>
		/// <param name="bb">Blue color component</param>
		/// <param name="pal">Reference AtariPalette where the best fit is searched in</param>
		/// <returns>Index in the AtariPalette that best matches the input RGB</returns>
		public static byte FindClosest(byte rr, byte gg, byte bb, Color[] pal)
		{
			byte best = 0;
			var bestDistance = 9999999;

			for (var j = 0; j < 128; j++)
			{
				var i = j * 2; //only compare with EVEN AtariPalette indexes
				var distR = rr - pal[i].R;
				var distG = gg - pal[i].G;
				var distB = bb - pal[i].B;
				var newDistance = distR * distR + distG * distG + distB * distB;

				if (bestDistance > newDistance)
				{
					bestDistance = newDistance;
					best = (byte)i;
				}
			}

			return best;
		}
	}
}
