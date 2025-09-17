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
			CDataArray,
			MadPascalArray,
			BinaryData,
			BasicListingFile, // 10
		};

		private static Compressors.CompressorType _compressorId = Compressors.CompressorType.ZX0;
		private static string _compressorName = string.Empty;

		public ExportFontWindow()
		{
			InitializeComponent();
			Load += FormCreate!;
		}

		public void Setup(Compressors.CompressorType whichCompressor)
		{
			_compressorId = whichCompressor;
			_compressorName = Compressors.GetName(whichCompressor);

			withCompression.Text = $"Compress the data with {_compressorName}";
		}

		private void FormCreate(object _, EventArgs __)
		{
			DoubleBuffered = true;
			ClearMemo();
			ComboBoxFontNumber.SelectedIndex = 0;
			ComboBoxExportType.SelectedIndex = -1;
			ComboBoxDataType.SelectedIndex = -1;

			ComboBoxExportType.SelectedIndex = 0;   // This will fire the export type handler and setup the rest of the GUI
		}

		private void ComboBoxExportTypeChange(object _, EventArgs __)
		{
			if (ComboBoxExportType.SelectedIndex >= 0)
			{
				ComboBoxDataType.Text = string.Empty;
				ComboBoxDataType.Items.Clear();
				ComboBoxDataType.Enabled = true;
				/* bmp mono, bmp color, assembler, action, basic, fastbasic, mads_dta, binary data, basic LST */
				MemoExport.Enabled = true;

				withCompression.Enabled = true;

				switch ((FormatTypes)ComboBoxExportType.SelectedIndex)
				{
					case FormatTypes.ImageBmpMono:
					{
						ButtonCopyClipboard.Enabled = false;

						ComboBoxDataType.Items.Add("Binary");
						ComboBoxDataType.SelectedIndex = 0;
						ComboBoxDataType.Enabled = false;
						withCompression.Enabled = false;
						MemoExport.Text = string.Empty;
					}
					break;

					case FormatTypes.ImageBmpColor:
					{
						ButtonCopyClipboard.Enabled = false;

						ComboBoxDataType.Items.Add("Binary");
						ComboBoxDataType.SelectedIndex = 0;
						ComboBoxDataType.Enabled = false;
						withCompression.Enabled = false;
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

					case FormatTypes.CDataArray:
					{
						ButtonCopyClipboard.Enabled = true;

						ComboBoxDataType.Text = @"Select an item";
						ComboBoxDataType.Items.Add("Byte in decimal");
						ComboBoxDataType.Items.Add("Byte in hexadecimal");
						ComboBoxDataType.SelectedIndex = 0;
						break;
					}

					case FormatTypes.MadPascalArray:
					{
						ButtonCopyClipboard.Enabled = true;

						ComboBoxDataType.Text = @"Select an item";
						ComboBoxDataType.Items.Add("Byte in decimal");
						ComboBoxDataType.Items.Add("Byte in hexadecimal");
						ComboBoxDataType.SelectedIndex = 0;
						break;
					}

					case FormatTypes.BinaryData:
					{
						ButtonCopyClipboard.Enabled = false;

						ComboBoxDataType.Items.Add("Binary");
						ComboBoxDataType.SelectedIndex = 0;
						ComboBoxDataType.Enabled = false;
						MemoExport.Text = @"This export option generates a binary data file with 1, 2 or 4 of the fonts.";
						break;
					}

					case FormatTypes.BasicListingFile:
					{
						ButtonCopyClipboard.Enabled = false;

						ComboBoxDataType.Items.Add("Basic listing");
						ComboBoxDataType.SelectedIndex = 0;
						ComboBoxDataType.Enabled = false;
						withCompression.Enabled = false;
						MemoExport.Text =
							"This export option generates basic .LST file that can be easily incorporated to your " +
							"own basic source with ENTER \"D:filename.LST\". Export file contains Basic lines 0 to 11 " +
							"with assembly routine that very quickly deploys font to memtop.";
					}
					break;
				}

//				ComboBoxDataTypeChange(this, EventArgs.Empty);
			}
		}

		private void ComboBoxDataTypeChange(object _, EventArgs __)
		{
			if (ComboBoxExportType.SelectedIndex >= 0)
			{
				Button_SaveAs.Enabled = true;
			}
			// Can't save basic listing with multiple fonts
			if (ComboBoxFontNumber.SelectedIndex > 3 && (FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.BasicListingFile)
			{
				Button_SaveAs.Enabled = false;
			}

			switch ((FormatTypes)ComboBoxExportType.SelectedIndex)
			{
				case FormatTypes.ImageBmpMono:
				case FormatTypes.ImageBmpColor:
					labelSizeInfo.Text = string.Empty;
					break;

				case FormatTypes.Assembler:
				case FormatTypes.Action:
				case FormatTypes.AtariBasic:
				case FormatTypes.FastBasic:
				case FormatTypes.MADSdta:
				case FormatTypes.CDataArray:
				case FormatTypes.MadPascalArray:
				{
					var (newText, originalSize, dataSize) = GenerateFileAsText(
						ComboBoxFontNumber.SelectedIndex,
						(FormatTypes)ComboBoxExportType.SelectedIndex,
						ComboBoxDataType.SelectedIndex,
						withCompression.Enabled && withCompression.Checked);

					MemoExport.Text = newText;

					labelSizeInfo.Text = originalSize != dataSize ? $"Original font size: {originalSize} bytes  Compressed font size: {dataSize} bytes" : $"Font Size:{originalSize} bytes";
					break;
				}
				case FormatTypes.BinaryData:
				{
					var (_, originalSize, dataSize) = GetFontData(ComboBoxFontNumber.SelectedIndex, withCompression.Enabled && withCompression.Checked);
					labelSizeInfo.Text = originalSize != dataSize ? $"Original font size: {originalSize} bytes Compressed font size: {dataSize} bytes" : $"Font Size:{originalSize} bytes";
					break;
				}
				default:
				{
					var (fontStartByte, fontEndByte) = CalcFontStartEnd(ComboBoxFontNumber.SelectedIndex);
					labelSizeInfo.Text = $"Font Size: {fontEndByte - fontStartByte} bytes";
					break;
				}
			}
		}

		private static string MakeFilenamePartFromFontSelectionNr(int fontNr)
		{
			switch (fontNr)
			{
				default: // 0,1,2,3
					return $"Font{(fontNr + 1)}";
				case 4: // 1+2
					return "Font1+2";
				case 5: // 3+4
					return "Font3+4";
				case 6: // 1+2+3+4
					return "Font1+2+3+4";
			}
		}

		private static (int, int) CalcFontStartEnd(int fontNr)
		{
			int fontStartByte;
			int fontEndByte;

			switch (fontNr)
			{
				default:
					fontNr %= 4;
					fontStartByte = fontNr * 1024;
					fontEndByte = (fontNr + 1) * 1024;
					break;
				case 4: // 1+2
					fontStartByte = 0;
					fontEndByte = 2 * 1024;
					break;
				case 5: // 3+4
					fontStartByte = 2 * 1024;
					fontEndByte = 4 * 1024;
					break;
				case 6: // 1+2+3+4
					fontStartByte = 0;
					fontEndByte = 4 * 1024;
					break;
			}

			return (fontStartByte, fontEndByte);
		}

		private void ButtonSaveAsClick(object sender, EventArgs e)
		{
			if ((FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.ImageBmpMono ||
				(FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.ImageBmpColor)
			{
				saveDialog.Filter = $@"{MakeFilenamePartFromFontSelectionNr(ComboBoxFontNumber.SelectedIndex)} (*.bmp)|*.bmp";
				saveDialog.DefaultExt = "bmp";
				saveDialog.FileName = $@"{MakeFilenamePartFromFontSelectionNr(ComboBoxFontNumber.SelectedIndex)}.bmp";

				if (saveDialog.ShowDialog() == DialogResult.OK)
				{
					SaveFontBMP(ComboBoxFontNumber.SelectedIndex, saveDialog.FileName, (FormatTypes)ComboBoxExportType.SelectedIndex != FormatTypes.ImageBmpMono);
				}

				return;
			}
			if ((FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.BinaryData)
			{
				saveDialog.Filter = $@"{MakeFilenamePartFromFontSelectionNr(ComboBoxFontNumber.SelectedIndex)} (*.dat)|*.dat";
				saveDialog.DefaultExt = "dat";
				saveDialog.FileName = $@"{MakeFilenamePartFromFontSelectionNr(ComboBoxFontNumber.SelectedIndex)}.dat";

				if (saveDialog.ShowDialog() == DialogResult.OK)
				{
					SaveBinaryData(ComboBoxFontNumber.SelectedIndex, saveDialog.FileName, withCompression.Enabled && withCompression.Checked);
				}

				return;
			}

			if ((FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.BasicListingFile)
			{
				// Save a single font as an Atari Basic listing file
				saveDialog.Filter = $@"{MakeFilenamePartFromFontSelectionNr(ComboBoxFontNumber.SelectedIndex)} (*.lst)|*.lst";
				saveDialog.DefaultExt = "lst";

				if (saveDialog.ShowDialog() == DialogResult.OK)
				{
					SaveRemFont(ComboBoxFontNumber.SelectedIndex, saveDialog.FileName);
				}

				return;
			}

			// These are handled above:
			// 0 = BMP
			// 1 = BMP color
			// 7 = Binary data
			// 8 = Basic listing
			// rest of the options are text / .txt
			saveDialog.Filter = $@"{MakeFilenamePartFromFontSelectionNr(ComboBoxFontNumber.SelectedIndex)} (*.txt)|*.txt";
			saveDialog.DefaultExt = "txt";

			if (saveDialog.ShowDialog() == DialogResult.OK)
			{
				var (text, _, __) = GenerateFileAsText(ComboBoxFontNumber.SelectedIndex, (FormatTypes)ComboBoxExportType.SelectedIndex, ComboBoxDataType.SelectedIndex, withCompression.Enabled && withCompression.Checked);

				File.WriteAllText(saveDialog.FileName, text);
			}
		}

		private void Button_CancelClick(object sender, EventArgs e)
		{
			Close();
		}

		private void MemoExportKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 'A')
			{
				(sender as RichTextBox)?.SelectAll();
				e.KeyChar = '\x0';
			}
		}

		private void ButtonCopyClipboardClick(object sender, EventArgs e)
		{
			if (MemoExport.Text.Length > 0)
				Clipboard.SetText(MemoExport.Text);
		}

		private void ClearMemo()
		{
			MemoExport.Clear();
		}

		private void ComboBoxFontNumber_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxDataTypeChange(null!, EventArgs.Empty);
		}

		/// <summary>
		/// Save a font (or multiple) as a BMP file
		/// </summary>
		/// <param name="fontNr"></param>
		/// <param name="filename"></param>
		/// <param name="asColor"></param>
		private static void SaveFontBMP(int fontNr, string filename, bool asColor)
		{
			int startFontIndex;
			int pictureHeight;
			switch (fontNr)
			{
				default:    // 0,1,2,3
					startFontIndex = 128 * fontNr;
					pictureHeight = 64;
					break;
				case 4: // 1+2
					startFontIndex = 0;
					pictureHeight = 128;
					break;
				case 5: // 3+4
					startFontIndex = 256;
					pictureHeight = 128;
					break;
				case 6: // 1+2+3+4
					startFontIndex = 0;
					pictureHeight = 256;
					break;
			}

			if (asColor)
			{
				startFontIndex += 512;
			}

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
			bmp2.Height = pictureHeight;
			bmp2.Image = new Bitmap(256, pictureHeight, PixelFormat.Format24bppRgb);

			using (var gr = Graphics.FromImage(bmp2.Image))
			{
				for (var y = 0; y < pictureHeight; y++)
				{
					for (var x = 0; x < 256; x++)
					{
						srcRect.X = x * 2;
						srcRect.Y = y * 2 + startFontIndex;
						destRect.X = x;
						destRect.Y = y;
						gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect, GraphicsUnit.Pixel);
					}
				}
			}

			bmp2.Image.Save(filename, ImageFormat.Bmp);
		}

		private static void SaveRemFont(int fontIndex, string fileName)
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

		private static void SaveBinaryData(int fontNr, string filename, bool withCompression)
		{
			var (fontData, _, __) = GetFontData(fontNr, withCompression);

			try
			{
				File.WriteAllBytes(filename, fontData);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Unable to export binary data. Have error: {ex.Message}");
			}
		}

		/* //////////////////////////////////////////////////////////////////////////////
		          
		//////////////////////////////////////////////////////////////////////////////-*/


		/// <summary>
		/// Export data to assembler language, action! and atari basic
		/// </summary>
		/// <param name="fontNr">Index into the font selector, 0,1,2,3, 4,5,6</param>
		/// <param name="exportType"></param>
		/// <param name="dataType"></param>
		/// <param name="withCompression"></param>
		/// <returns>Text string with the output and the size of the date used to generate the output</returns>
		private static (string, int, int) GenerateFileAsText(int fontNr, FormatTypes exportType, int dataType, bool withCompression)
		{
			var sb = new StringBuilder();

			var lineNumber = 10010;
			var charCounter = 0;

			var (fontData, inputSize, dataSize) = GetFontData(fontNr, withCompression);

			if (exportType == FormatTypes.Assembler)
			{
				if (inputSize != dataSize)
					sb.AppendLine($"\t; Original size: {inputSize} bytes : {_compressorName} compressed size: {dataSize} bytes");
				else
					sb.AppendLine($"\t; Size: {inputSize} bytes");
				sb.Append("\t.BYTE ");
			}

			if (exportType == FormatTypes.Action)
			{
				if (inputSize != dataSize)
					sb.AppendLine($"; Original size: {inputSize} bytes : {_compressorName} compressed size: {dataSize} bytes");
				else
					sb.AppendLine($"; Size: {inputSize} bytes");
				sb.AppendLine("PROC FONT=*()");
				sb.AppendLine("[");
			}

			if (exportType == FormatTypes.AtariBasic)
			{
				sb.AppendLine("10000 REM *** DATA FONT ***");
				if (inputSize != dataSize)
					sb.AppendLine($"10001 REM Original size: {inputSize} bytes : {_compressorName} compressed size: {dataSize} bytes");
				else
					sb.AppendLine($"10001 REM Size: {inputSize} bytes");
				sb.Append("10010 DATA ");
			}

			if (exportType == FormatTypes.FastBasic)
			{
				if (inputSize != dataSize)
					sb.AppendLine($"` Original size: {inputSize} bytes : {_compressorName} compressed size: {dataSize} bytes");
				else
					sb.AppendLine($"` Size: {inputSize} bytes");
				sb.Append("data font() byte = ");
			}

			if (exportType == FormatTypes.MADSdta)
			{
				if (inputSize != dataSize)
					sb.AppendLine($"\t; Original size: {inputSize} bytes : {_compressorName} compressed size: {dataSize} bytes");
				else
					sb.AppendLine($"\t; Size: {inputSize} bytes");
				sb.Append("\tdta ");
			}

			if (exportType == FormatTypes.CDataArray)
			{
				if (inputSize != dataSize)
					sb.AppendLine($"// Original size: {inputSize} {_compressorName} compressed size: {dataSize} bytes");
				else
					sb.AppendLine($"// Size: {inputSize} bytes");
				sb.Append("{\n\t");
			}

			if (exportType == FormatTypes.MadPascalArray)
			{
				if (inputSize != dataSize)
					sb.AppendLine($"// Original size: {inputSize} bytes : {_compressorName} compressed size: {dataSize} bytes");
				else
					sb.AppendLine($"// Size: {inputSize} bytes");
				sb.Append($"font: array [0..{fontData.Length - 1}] of byte = (\n\t");
			}

			var bytesLeft = fontData.Length;

			for (var index = 0; index < fontData.Length; --bytesLeft, index++)
			{
				if (dataType == 1)
				{
					if (exportType == FormatTypes.CDataArray)
						sb.Append($"0x{fontData[index]:X2}");
					else
						sb.Append($"${fontData[index]:X2}");
				}
				else
				{
					sb.Append($"{fontData[index]}");
				}

				++charCounter;

				// Start the next line
				if (charCounter == 8 && bytesLeft > 1)
				{
					charCounter = 0;

					if (exportType is FormatTypes.FastBasic or FormatTypes.CDataArray or FormatTypes.MadPascalArray)
					{
						sb.Append(',');
					}

					sb.AppendLine(string.Empty);

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

					if (exportType is FormatTypes.CDataArray or FormatTypes.MadPascalArray)
					{
						sb.Append("\t");
					}
				}

				if ((charCounter != 8) && (charCounter != 0) && bytesLeft > 1)
				{
					switch (exportType)
					{
						case FormatTypes.Action:
							sb.Append(' ');
							break;
						case FormatTypes.Assembler:
						case FormatTypes.AtariBasic:
						case FormatTypes.FastBasic:
						case FormatTypes.MADSdta:
						case FormatTypes.CDataArray:
						case FormatTypes.MadPascalArray:
							sb.Append(',');
							break;
					}
				}
			}

			if (exportType == FormatTypes.Action)
			{
				sb.Append("\n]\nMODULE\n");
			}

			if (exportType == FormatTypes.CDataArray)
			{
				sb.Append("\n}");
			}

			if (exportType == FormatTypes.MadPascalArray)
			{
				sb.Append("\n);");
			}

			return (sb.ToString(), inputSize, fontData.Length);
		}

		private void WithCompressionCheckedChanged(object sender, EventArgs e)
		{
			ComboBoxDataTypeChange(null!, EventArgs.Empty);
		}

		private static (byte[], int, int) GetFontData(int fontNr, bool withCompression)
		{
			var (fontStartByte, fontEndByte) = CalcFontStartEnd(fontNr);

			var inputSize = fontEndByte - fontStartByte;

			var fontData = new byte[inputSize];
			var runner = 0;
			for (var index = fontStartByte; index < fontEndByte; index++)
			{
				fontData[runner++] = AtariFont.FontBytes[index];
			}

			if (withCompression)
			{
				var compressedFontData = Compressors.Compress(fontData, _compressorId);
				if (compressedFontData.Length < fontData.Length)
				{
					fontData = compressedFontData;
				}
			}

			return (fontData, inputSize, fontData.Length);
		}
	}
}
