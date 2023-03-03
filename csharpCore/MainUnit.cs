using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

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
				var assembly = Assembly.GetExecutingAssembly();
				var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
				var version = fvi.FileVersion.Split('.');
				V1 = Int32.Parse(version[0]);
				V2 = Int32.Parse(version[1]);
				V3 = Int32.Parse(version[2]);
				V4 = Int32.Parse(version[3]);
			}
			catch { }
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

		public static int GetCharacterPointer(int character)
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

			return ry * 32 * 8 + rx * 8;
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

		public static byte[,] Get5ColorCharacter(int character)
		{
			var get5ColorCharacter_result = new byte[8, 8];
			var charPtr = GetCharacterPointer(character);

			for (var i = 0; i < 8; i++)
			{
				var clData = DecodeCL(MainForm.ft[charPtr]);

				for (var j = 0; j < 4; j++)
				{
					get5ColorCharacter_result[j, i] = clData[j];
				}

				charPtr++;
			}

			return get5ColorCharacter_result;
		}

		public static byte[,] Get2ColorCharacter(int character)
		{
			var get2ColorCharacter_result = new byte[8, 8];
			var charPtr = GetCharacterPointer(character);

			for (var i = 0; i < 8; i++)
			{
				var clData = DecodeBW(MainForm.ft[charPtr + i]);

				for (var j = 0; j < 8; j++)
				{
					get2ColorCharacter_result[j, i] = clData[j];
				}
			}

			return get2ColorCharacter_result;
		}

		public static void Set2ColorCharacter(byte[,] character2color, int character)
		{
			var charline2 = new byte[8];
			var fontByteIndex = GetCharacterPointer(character);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 8; b++)
				{
					charline2[b] = character2color[b, a];
				}

				MainForm.ft[fontByteIndex + a] = EncodeBW(charline2);
			}
		}

		public static void Set5ColorCharacter(byte[,] character5color, int character)
		{
			var charline5 = new byte[4];
			var fontByteIndex = GetCharacterPointer(character);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 4; b++)
				{
					charline5[b] = character5color[b, a];
				}

				MainForm.ft[fontByteIndex + a] = EncodeCL(charline5);
			}
		}

		public static readonly string CutFromResourceNameToGetFilename = "FontMaker.Resources.";
		public static readonly string[] Filenames = new string[] { "Default.fnt", "basicremfont.lst", "default.atrview", "laoo.act" };
		/// <summary>
		/// Make sure that the basic files we need to boot are there.
		/// </summary>
		public static void CheckResources()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var names = assembly.GetManifestResourceNames();

			var loc = Path.GetDirectoryName(typeof(Program).Assembly.Location);

			foreach (var name in names)
			{
				var myName = name.Replace(CutFromResourceNameToGetFilename, "");
				if (Filenames.Contains(myName))
				{
					if (!File.Exists(myName))
					{
						var res = assembly.GetManifestResourceStream(name);
						if (res != null)
						{
							var bytes = new byte[res.Length];
							res.Read(bytes, 0, (int)res.Length);

							// Now write the file to the disc
							File.WriteAllBytes(myName, bytes);
						}
					}
				}
			}
		}

		public static void exitowiec()
		{
			var re = MessageBox.Show("Are you sure you want to quit?", "Atari FontMaker", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (re == DialogResult.Yes)
			{
				Environment.Exit(Environment.ExitCode);
			}
		}

		//TColor > palette index approximator
		public static byte FindClosest(byte rr, byte gg, byte bb, Color[] pal)
		{
			int distR;
			int distG;
			int distB;
			byte best = 0;
			int newDistance = 0;
			var bestDistance = 9999999;

			for (var j = 0; j < 128; j++)
			{
				var i = j * 2; //only compare with EVEN palette indexes
				distR = rr - pal[i].R;
				distG = gg - pal[i].G;
				distB = bb - pal[i].B;
				newDistance = distR * distR + distG * distG + distB * distB;

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
