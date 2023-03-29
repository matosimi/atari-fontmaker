using System.Drawing.Imaging;
using System.Text;

namespace FontMaker
{
    public static class ExportWindowUnit
	{
		public enum FormatTypes
        {
			ImageBMP = 0,
			Assembler, 
			Action,
			AtariBasic, 
			FastBasic, 
			MADSdta, 
			BasicListingFile,
		};

		public static ExportWindow ExportWindowForm { get; set; } = new ExportWindow();

		public static void SaveFontBMP(int fi, string fn, Image bmp1)
		{
			var fntIndex = 128 * fi;

			var destRect = new Rectangle
			{
				X = 0,
				Y = 0,
				Width = 1,
				Height = 1,
			};

			var srcRect = new Rectangle
			{
				X = 0,
				Y = 0,
				Width = 1,
				Height = 1,
			};

			var bmp2 = new PictureBox();
			bmp2.Width = 256;
			bmp2.Height = 64;
			bmp2.Image = new Bitmap(256, 64, PixelFormat.Format24bppRgb);

			using (var gr = Graphics.FromImage(bmp2.Image))
			{
				for (var y = 0; y < 64; y++)
				{
					for (var x = 0; x < 256; x++)
					{
						srcRect.X = x * 2;
						srcRect.Y = y * 2 + fntIndex;
						destRect.X = x;
						destRect.Y = y;
						gr.DrawImage(bmp1, destRect, srcRect, GraphicsUnit.Pixel);
					}
				}
			}

			bmp2.Image.Save(fn, ImageFormat.Bmp);
		}

		public static void SaveRemFont(int fontIndex, string fileName)
		{
			// Load the basic starting REM font from disc
			try
			{
                byte[] buf = MainUnit.GetResource<byte[]>("basicremfont.lst");

                // Write the font values into the "loaded" font file
                for (var j = 0; j < 9; j++)
                {
                    for (var i = 0; i < 0x68; i++)
                    {
                        buf[6 + i + j * (0x68 + 7)] = MainUnit.MainForm.ft[1024 * fontIndex + i + 0x68 * j];
                    }
                }

                for (var i = 0; i < 0x58; i++)
                {
                    buf[6 + i + 9 * (0x68 + 7)] = MainUnit.MainForm.ft[1024 * fontIndex + i + 0x68 * 9];
                }

				// Write the updated font to disk
				File.WriteAllBytes(fileName, buf);
            }
			catch(Exception ex)
			{
                MessageBox.Show(ex.Message);
            }
        }

		/* //////////////////////////////////////////////////////////////////////////////
		          export data to assembler language, action! and atari basic
		//////////////////////////////////////////////////////////////////////////////-*/
		public static string GenerateFileAsText(int fontNumber, FormatTypes exportType, int dataType)
		{
			var sb = new StringBuilder();

			var line = 0;
			var num = 10010;
			var carac = 0;

			if (exportType == FormatTypes.Assembler)
			{
				sb.Append("\t.BYTE ");
			}

			if (exportType == FormatTypes.Action)
			{
				sb.AppendLine("PROC font()=[");
			}

			if (exportType == FormatTypes.AtariBasic)
			{
				sb.AppendLine("10000 REM *** DATA FONT ***");
				sb.Append("10010 DATA ");
			}

			if (exportType == FormatTypes.FastBasic)
			{
				sb.Append("data font() byte = ");
			}

			if (exportType == FormatTypes.MADSdta)
			{
				sb.Append("\tdta ");
			}

			for (var index = fontNumber * 1024; index < (fontNumber + 1) * 1024; index ++)
			{
				if (dataType == 1)
				{
					sb.Append($"${MainUnit.MainForm.ft[index]:X2}");
				}
				else
				{
					sb.Append($"{MainUnit.MainForm.ft[index]}");
				}

				++carac;

				if ((carac == 8) && (line != 127))
				{
					carac = 0;
					line ++;

					if (exportType == FormatTypes.FastBasic)
					{
						sb.Append(',');
					}

					sb.AppendLine(String.Empty);

					if (exportType == FormatTypes.Assembler)
					{
						sb.Append("\t.BYTE ");
					}

					if (exportType == FormatTypes.AtariBasic)
					{
						num += 10;
						sb.Append($"{num} DATA ");
					}

					if (exportType == FormatTypes.FastBasic)
					{
						sb.Append("data byte = ");
					}

					if (exportType == FormatTypes.MADSdta)
					{
						sb.Append("\tdta ");
					}
				}

				if ((carac != 8) && (carac != 0))
				{
					if (exportType == FormatTypes.Action)
					{
						sb.Append(' ');
					}

					if ((exportType == FormatTypes.Assembler) 
						|| (exportType == FormatTypes.AtariBasic) 
						|| (exportType == FormatTypes.FastBasic) 
						|| (exportType == FormatTypes.MADSdta))
					{
						sb.Append(',');
					}
				}
			}

			if (exportType == FormatTypes.Action)
			{
				sb.Append("\n]\nMODULE\n");
			}

			return sb.ToString();
		}
    }
}
