using System.Drawing.Imaging;
using System.Text;

namespace FontMaker
{
	public partial class ExportFontWindow : Form
	{
		public enum FormatTypes
		{
			ImageBmpMono = 0,
			ImageBmpColor,
			Assembler,
			Action,
			AtariBasic,
			FastBasic,
			MADSdta,
			BasicListingFile, // 7
		};

		public ExportFontWindow()
		{
			InitializeComponent();
			Load += FormCreate!;
		}

		public void FormCreate(object _, EventArgs __)
		{
			DoubleBuffered = true;
			ClearMemo();
			ComboBoxFontNumber.SelectedIndex = 0;
			ComboBoxExportType.SelectedIndex = -1;
			ComboBoxDataType.SelectedIndex = -1;

			ComboBoxExportType.SelectedIndex = 0;   // This will fire the export type handler and setup the rest of the GUI
		}

		public void ComboBoxExportTypeChange(object _, EventArgs __)
		{
			if (ComboBoxExportType.SelectedIndex >= 0)
			{
				ComboBoxDataType.Text = string.Empty;
				ComboBoxDataType.Items.Clear();
				ComboBoxDataType.Enabled = true;
				/* bmp mono, bmp color, assembler, action, basic, fastbasic, mads_dta, basic LST */
				MemoExport.Enabled = true;

				switch ((FormatTypes)ComboBoxExportType.SelectedIndex)
				{
					case FormatTypes.ImageBmpMono:
						{
							ButtonCopyClipboard.Enabled = false;

							ComboBoxDataType.Items.Add("Binary");
							ComboBoxDataType.SelectedIndex = 0;
							ComboBoxDataType.Enabled = false;
							MemoExport.Text = string.Empty;
						}
						break;

					case FormatTypes.ImageBmpColor:
						{
							ButtonCopyClipboard.Enabled = false;

							ComboBoxDataType.Items.Add("Binary");
							ComboBoxDataType.SelectedIndex = 0;
							ComboBoxDataType.Enabled = false;
							MemoExport.Text = string.Empty;
						}
						break;

					case FormatTypes.Assembler:
						{
							ButtonCopyClipboard.Enabled = true;

							ComboBoxDataType.Text = @"Select an item";
							ComboBoxDataType.Items.Add("Byte in decimal");
							ComboBoxDataType.Items.Add("Byte in hexadecimal");
							ComboBoxDataType.SelectedIndex = 0;
						}
						break;

					case FormatTypes.Action:
						{
							ButtonCopyClipboard.Enabled = true;

							ComboBoxDataType.Text = @"Select an item";
							ComboBoxDataType.Items.Add("Byte in decimal");
							ComboBoxDataType.Items.Add("Byte in hexadecimal");
							ComboBoxDataType.SelectedIndex = 0;
						}
						break;

					case FormatTypes.AtariBasic:
						{
							ButtonCopyClipboard.Enabled = true;

							ComboBoxDataType.Items.Add("Byte in decimal");
							ComboBoxDataType.SelectedIndex = 0;
							ComboBoxDataType.Enabled = false;
						}
						break;

					case FormatTypes.FastBasic:
						{
							ButtonCopyClipboard.Enabled = true;

							ComboBoxDataType.Items.Add("Byte in decimal");
							ComboBoxDataType.SelectedIndex = 0;
							ComboBoxDataType.Enabled = false;
						}
						break;

					case FormatTypes.MADSdta:
						{
							ButtonCopyClipboard.Enabled = true;

							ComboBoxDataType.Text = @"Select an item";
							ComboBoxDataType.Items.Add("Byte in decimal");
							ComboBoxDataType.Items.Add("Byte in hexadecimal");
							ComboBoxDataType.SelectedIndex = 0;
						}
						break;

					case FormatTypes.BasicListingFile:
						{
							ButtonCopyClipboard.Enabled = false;

							ComboBoxDataType.Items.Add("Basic listing");
							ComboBoxDataType.SelectedIndex = 0;
							ComboBoxDataType.Enabled = false;
							MemoExport.Text =
								"This export option generates basic .LST file that can be easily incorporated to your " +
								"own basic source with ENTER \"D:filename.LST\". Export file contains Basic lines 0 to 11 " +
								"with assembly routine that very quickly deploys font to memtop.";
						}
						break;
				}

				ComboBoxDataTypeChange(this, EventArgs.Empty);
			}
		}

		public void ComboBoxDataTypeChange(object _, EventArgs __)
		{
			if (ComboBoxExportType.SelectedIndex >= 0)
			{
				Button_SaveAs.Enabled = true;
			}

			if ((FormatTypes)ComboBoxExportType.SelectedIndex > FormatTypes.ImageBmpColor && (FormatTypes)ComboBoxExportType.SelectedIndex < FormatTypes.BasicListingFile)
			{
				MemoExport.Text = GenerateFileAsText(
					ComboBoxFontNumber.SelectedIndex,
					(FormatTypes)ComboBoxExportType.SelectedIndex,
					ComboBoxDataType.SelectedIndex);
			}
		}

		public void ButtonSaveAsClick(object sender, EventArgs e)
		{
			if ((FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.ImageBmpMono ||
				(FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.ImageBmpColor)
			{
				saveDialog.Filter = $@"Font{(ComboBoxFontNumber.SelectedIndex + 1)} (*.bmp)|*.bmp";
				saveDialog.DefaultExt = "bmp";
				saveDialog.FileName = $@"Font{(ComboBoxFontNumber.SelectedIndex + 1)}.bmp";

				if (saveDialog.ShowDialog() == DialogResult.OK)
				{
					SaveFontBMP(ComboBoxFontNumber.SelectedIndex, saveDialog.FileName, (FormatTypes)ComboBoxExportType.SelectedIndex != FormatTypes.ImageBmpMono);
				}

				return;
			}

			if ((FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.BasicListingFile)
			{
				saveDialog.Filter = $@"Font{(ComboBoxFontNumber.SelectedIndex + 1)} (*.lst)|*.lst";
				saveDialog.DefaultExt = "lst";

				if (saveDialog.ShowDialog() == DialogResult.OK)
				{
					SaveRemFont(ComboBoxFontNumber.SelectedIndex, saveDialog.FileName);
				}

				return;
			}

			// These two are handled
			// 0 = BMP
			// 6 = Basic listing
			// rest of the options are text / .txt
			saveDialog.Filter = $@"Font{(ComboBoxFontNumber.SelectedIndex + 1)} (*.txt)|*.txt";
			saveDialog.DefaultExt = "txt";

			if (saveDialog.ShowDialog() == DialogResult.OK)
			{
				var text = GenerateFileAsText(ComboBoxFontNumber.SelectedIndex, (FormatTypes)ComboBoxExportType.SelectedIndex, ComboBoxDataType.SelectedIndex);
				File.WriteAllText(saveDialog.FileName, text);
			}
		}

		public void Button_CancelClick(object sender, EventArgs e)
		{
			Close();
		}

		public void MemoExportKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 'A')
			{
				(sender as RichTextBox)?.SelectAll();
				e.KeyChar = '\x0';
			}
		}

		public void ButtonCopyClipboardClick(object sender, EventArgs e)
		{
			if (MemoExport.Text.Length > 0)
				Clipboard.SetText(MemoExport.Text);
		}

		public void ClearMemo()
		{
			MemoExport.Clear();
		}

		private void ComboBoxFontNumber_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxDataTypeChange(null!, EventArgs.Empty);
		}

		public static void SaveFontBMP(int fontNr, string filename, bool asColor)
		{
			var fntIndex = 128 * fontNr + (asColor ? 512 : 0);

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
						gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect, GraphicsUnit.Pixel);
					}
				}
			}

			bmp2.Image.Save(filename, ImageFormat.Bmp);
		}

		public static void SaveRemFont(int fontIndex, string fileName)
		{
			// Load the basic starting REM font from disc
			try
			{
				var buf = Helpers.GetResource<byte[]>("basicremfont.lst");

				// Write the font values into the "loaded" font file
				for (var j = 0; j < 9; j++)
				{
					for (var i = 0; i < 0x68; i++)
					{
						buf[6 + i + j * (0x68 + 7)] = AtariFont.FontBytes[1024 * fontIndex + i + 0x68 * j];
					}
				}

				for (var i = 0; i < 0x58; i++)
				{
					buf[6 + i + 9 * (0x68 + 7)] = AtariFont.FontBytes[1024 * fontIndex + i + 0x68 * 9];
				}

				// Write the updated font to disk
				File.WriteAllBytes(fileName, buf);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/* //////////////////////////////////////////////////////////////////////////////
		          
		//////////////////////////////////////////////////////////////////////////////-*/


		/// <summary>
		/// Export data to assembler language, action! and atari basic
		/// </summary>
		/// <param name="fontNumber"></param>
		/// <param name="exportType"></param>
		/// <param name="dataType"></param>
		/// <returns></returns>
		public static string GenerateFileAsText(int fontNumber, FormatTypes exportType, int dataType)
		{
			var sb = new StringBuilder();

			var line = 0;
			var lineNumber = 10010;
			var charCounter = 0;

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

			for (var index = fontNumber * 1024; index < (fontNumber + 1) * 1024; index++)
			{
				if (dataType == 1)
				{
					sb.Append($"${AtariFont.FontBytes[index]:X2}");
				}
				else
				{
					sb.Append($"{AtariFont.FontBytes[index]}");
				}

				++charCounter;

				if ((charCounter == 8) && (line != 127))
				{
					charCounter = 0;
					line++;

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
						lineNumber += 10;
						sb.Append($"{lineNumber} DATA ");
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

				if ((charCounter != 8) && (charCounter != 0))
				{
					switch (exportType)
					{
						case FormatTypes.Action:
							sb.Append(' ');
							break;
						case FormatTypes.Assembler or FormatTypes.AtariBasic or FormatTypes.FastBasic or FormatTypes.MADSdta:
							sb.Append(',');
							break;
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
