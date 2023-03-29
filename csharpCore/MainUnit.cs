using System.CodeDom;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FontMaker
{
	public static partial class MainUnit
	{
		public static TMainForm MainForm { get; set; }

		public static readonly string TITLE = "Atari FontMaker";


		/// <summary>
		/// Open a given url in the default browser
		/// </summary>
		/// <param name="url">url to open</param>
		public static void OpenURL(string url)
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

		public static void GetBuildInfo(ref int V1, ref int V2, ref int V3, ref int V4)
		{
			try
			{
				Assembly thisAssem = typeof(MainUnit).Assembly;
				AssemblyName thisAssemName = thisAssem.GetName();

				Version ver = thisAssemName.Version;
				V1 = ver.Major;
				V2 = ver.Minor;
				V3 = ver.Build;
				V4 = ver.Revision;

				return;
			}
			catch (Exception ex)
			{
				// MessageBox.Show(ex.Message);
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

		public static int GetCharacterPointer(int character, bool onBank2)
		{
			var ry = character / 32;
			var rx = character % 32;

			if ((ry > 3) && (ry < 12))
			{
				ry -= 4;
			}

			if ((ry > 11) && (ry < 16))
			{
				ry -= 8;
			}

			return ry * 32 * 8 + rx * 8 + (onBank2 ? 2048 : 0);
		}

		public static byte[] DecodeBW(byte ln)
		{
			var op = new byte[8];

			for (var c = 7; c >= 0; c--)
			{
				op[c] = (byte)(ln % 2);
				ln = (byte)(ln >> 1);
			}

			return op;
		}

		public static byte[] DecodeCL(byte ln)
		{
			var op = new byte[4];

			for (var c = 3; c >= 0; c--)
			{
				op[c] = (byte)(ln % 4);
				ln = (byte)(ln >> 2);
			}

			return op;
		}

		public static byte EncodeBW(byte[] cd)
		{
			byte v = 128;
			byte c = 0;
			byte o = 0;
			do
			{
				if (cd[c] != 0)
					o |= v;
				c++;
				v = (byte)(v >> 1);
			}
			while (v != 0);

			return o;
		}

		public static byte EncodeCL(byte[] cd)
		{
			byte v = 64;
			byte c = 0;
			byte o = 0;

			do
			{
				o = (byte)(o + v * cd[c]);
				c++;
				v = (byte)(v >> 2);
			}
			while (v != 0);

			return o;
		}

		public static byte[,] Get5ColorCharacter(int character, bool onBank2)
		{
			var res = new byte[8, 8];
			var charPtr = GetCharacterPointer(character, onBank2);

			for (var i = 0; i < 8; i++)
			{
				var clData = DecodeCL(MainForm.ft[charPtr]);

				for (var j = 0; j < 4; j++)
				{
					res[j, i] = clData[j];
				}

				charPtr++;
			}

			return res;
		}

		public static byte[,] Get2ColorCharacter(int character, bool onBank2)
		{
			var res = new byte[8, 8];
			var charPtr = GetCharacterPointer(character, onBank2);

			for (var i = 0; i < 8; i++)
			{
				var clData = DecodeBW(MainForm.ft[charPtr + i]);

				for (var j = 0; j < 8; j++)
				{
					res[j, i] = clData[j];
				}
			}

			return res;
		}

		public static void Set2ColorCharacter(byte[,] pixels, int character, bool onBank2)
		{
			var charline2 = new byte[8];
			var fontByteIndex = GetCharacterPointer(character, onBank2);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 8; b++)
				{
					charline2[b] = pixels[b, a];
				}

				MainForm.ft[fontByteIndex + a] = EncodeBW(charline2);
			}
		}

		public static void Set5ColorCharacter(byte[,] character5color, int character, bool onBank2)
		{
			var charline5 = new byte[4];
			var fontByteIndex = GetCharacterPointer(character, onBank2);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 4; b++)
				{
					charline5[b] = character5color[b, a];
				}

				MainForm.ft[fontByteIndex + a] = EncodeCL(charline5);
			}
		}

		public static T GetResource<T>(string shortResourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = $"FontMaker.Resources.{shortResourceName}";
			if (typeof(T) == typeof(string))
			{
				using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				using (StreamReader reader = new StreamReader(stream))
				{

                    return (T)(object)reader.ReadToEnd();
				}
			}
			else if (typeof(T) == typeof(byte[]))
			{
				using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				using (BinaryReader reader = new(stream))
				{

					return (T)(object)reader.ReadBytes((int)stream.Length);
				}

			}
			else
				throw new Exception("Unsupported resource object type");
		}


        public static void ExitApplication()
		{
			var re = MessageBox.Show("Are you sure you want to quit?", "Atari FontMaker", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (re == DialogResult.Yes)
			{
				MainForm.Exit();
			}
		}

		/// <summary>
		/// Find the palette entry that best approximates the request RGB color
		/// </summary>
		/// <param name="rr">Red color component</param>
		/// <param name="gg">Green color component</param>
		/// <param name="bb">Blue color component</param>
		/// <param name="pal">Reference palette where the best fit is searched in</param>
		/// <returns>Index in the palette that best matches the input RGB</returns>
		public static byte FindClosest(byte rr, byte gg, byte bb, Color[] pal)
		{
			byte best = 0;
			var bestDistance = 9999999;

			for (var j = 0; j < 128; j++)
			{
				var i = j * 2; //only compare with EVEN palette indexes
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
