﻿using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

#pragma warning disable WFO1000

namespace FontMaker
{
	public partial class ExportViewWindow : Form
	{
		private const int BITMAP_WIDTH = 640;
		private const int BITMAP_HEIGHT = 416;

		private const int EXPORT_WIDTH = 40;
		private const int EXPORT_HEIGHT = 26;

		private const int CHAR_PIXEL_WIDTH = 16;

		private int PreviousExportType { get; set; } = -1;
		private int PreviousDataType { get; set; } = -1;

		private Rectangle FullViewRegion = new(0, 0, 40, 26);

		private bool PreviousTransposeFlag { get; set; } = false;
		private bool RememberSelection { get; set; }

		public bool InColorMode { get; set; }
		public int WhichColorMode { get; set; }

		public bool InOffsetUpdate { get; set; }

		public enum FormatTypes
		{
			BinaryData = 0,
			Assembler,
			Action,
			AtariBasic,
			FastBasic,
			MADSdta,
			CDataArray,
			MadPascalArray
		};

		private static Compressors.CompressorType _compressorId = Compressors.CompressorType.ZX0;
		private static string _compressorName = string.Empty;

		public enum SelectionStatusFlags
		{
			None, Selecting, Selected
		}

		private static readonly SolidBrush cyanBrush = new(Color.Cyan);

		private Rectangle _exportRegion = new(0, 0, 40, 26);
		private Point _exportOffset = new Point(0, 0);

		private SelectionStatusFlags _selectionStatus;
		private int OffsetX { get; set; }
		private int OffsetY { get; set; }

		public ExportViewWindow()
		{
			InitializeComponent();
			RememberSelection = true;
		}

		public void Setup(bool inColorMode, int whichColorMode, Compressors.CompressorType whichCompressor)
		{
			InColorMode = inColorMode;
			WhichColorMode = whichColorMode;
			_compressorId = whichCompressor;
			_compressorName = Compressors.GetName(whichCompressor);

			withCompression.Text = $"Compress the data with {_compressorName}";
		}

		#region Load/Save the current configuration
		private void ExportViewWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (checkBoxRememberState.Checked)
			{
				PreviousExportType = ComboBoxExportType.SelectedIndex;
				PreviousDataType = ComboBoxDataType.SelectedIndex;
				PreviousTransposeFlag = checkBoxTranspose.Checked;
				RememberSelection = true;
			}
			else
			{
				RememberSelection = false;
			}
		}

		public void LoadConfiguration(bool rememberSelection, int exportType, int dataType, Rectangle box, Point offset, bool transpose)
		{
			if (rememberSelection)
			{
				if (exportType < 0 || exportType >= ComboBoxExportType.Items.Count)
					exportType = 0;
				PreviousExportType = exportType;

				PreviousDataType = dataType;
				_exportRegion = box;
				_exportOffset = offset;
				if (_exportRegion.X < 0) _exportRegion.X = 0;
				if (_exportRegion.Y < 0) _exportRegion.Y = 0;
				if (_exportRegion.X + _exportRegion.Width >= AtariView.Width) _exportRegion.Width = AtariView.Width - _exportRegion.X;
				if (_exportRegion.Y + _exportRegion.Height >= AtariView.Height) _exportRegion.Height = AtariView.Height - _exportRegion.X;
				if (_exportRegion.Width < 1) _exportRegion.Width = 1;
				if (_exportRegion.Height < 1) _exportRegion.Height = 1;
			}
		}

		public (bool, int, int, Rectangle, Point, bool) SaveConfiguration()
		{
			return (RememberSelection, PreviousExportType, PreviousDataType, _exportRegion, _exportOffset, PreviousTransposeFlag);
		}
		#endregion

		private void ExportViewWindow_Load(object sender, EventArgs e)
		{
			// Check if the view is larger than 40x26. If so then enable the horizontal and or vertical scrollbars
			if (AtariView.Width > 40)
			{
				// Horizontal scroll bar
				hScrollBar.Enabled = true;
				hScrollBar.Maximum = AtariView.Width - 40;
				hScrollBar.Value = 0;

				numericFromX.Maximum = AtariView.Width - 1;
				numericOffsetX.Maximum = AtariView.Width - 40;
				numericWidth.Maximum = AtariView.Width;
				FullViewRegion.Width = AtariView.Width;

				labelOffsetX.Enabled = true;
				numericOffsetX.Enabled = true;
			}

			if (AtariView.Height > 26)
			{
				// Vertical scroll bar
				vScrollBar.Enabled = true;
				vScrollBar.Maximum = AtariView.Height - 26;
				vScrollBar.Value = 0;

				numericFromY.Maximum = AtariView.Height - 1;
				numericOffsetY.Maximum = AtariView.Height - 26;
				numericHeight.Maximum = AtariView.Height;
				FullViewRegion.Height = AtariView.Height;

				labelOffsetY.Enabled = true;
				numericOffsetY.Enabled = true;
			}

			pictureBoxViewEditorRubberBand_Resize(null, EventArgs.Empty);
			RedrawSmallView();
			MemoExport.Clear();

			ComboBoxExportType.SelectedIndex = 0;
			ComboBoxDataType.SelectedIndex = -1;

			if (RememberSelection)
			{
				ComboBoxExportType.SelectedIndex = PreviousExportType != -1 ? PreviousExportType : 0; // This will fire the export type handler and setup the rest of the GUI

				// Check that the range is still valid
				if (PreviousDataType < 0 || PreviousDataType >= ComboBoxDataType.Items.Count)
					PreviousDataType = 0;
				ComboBoxDataType.SelectedIndex = PreviousDataType != -1 ? PreviousDataType : 0;
				checkBoxTranspose.Checked = PreviousTransposeFlag;
			}
			else
			{

				ComboBoxExportType.SelectedIndex = 0; // This will fire the export type handler and setup the rest of the GUI

				_exportRegion = new Rectangle(0, 0, AtariView.Width, AtariView.Height);
			}

			UpdateRegionEdits();
		}

		private void UpdateRegionEdits()
		{
			if (InOffsetUpdate) return;
			InOffsetUpdate = true;

			numericFromX.Value = _exportRegion.X;
			numericFromY.Value = _exportRegion.Y;
			numericWidth.Value = _exportRegion.Width;
			numericHeight.Value = _exportRegion.Height;
			numericOffsetX.Value = 0;
			numericOffsetY.Value = 0;

			InOffsetUpdate = false;

			UpdateRegionLabel();
			RedrawSmallView();
			ShowSelectionRubberBand();
		}

		private void UpdateRegionLabel()
		{
			labelDimensions.Text = $"({_exportRegion.X}, {_exportRegion.Y}) - ({_exportRegion.Width}, {_exportRegion.Height}) @ {(_exportRegion.Width * _exportRegion.Height)} bytes";
		}

		public void RedrawSmallView()
		{
			var colorOffset = InColorMode ? 512 : 0;
			var img = Helpers.GetImage(pictureBoxAtariViewSmall);
			using (var gr = Graphics.FromImage(img))
			using (var wrapMode = new ImageAttributes())
			{
				wrapMode.SetWrapMode(WrapMode.TileFlipXY);
				gr.InterpolationMode = InterpolationMode.NearestNeighbor;
				//gr.Clear(AtariPalette[SetOfSelectedColors[1]]);

				var destRect = new Rectangle
				{
					Width = 16,
					Height = 16,
				};

				var srcRect = new Rectangle
				{
					Width = 16,
					Height = 16,
				};

				for (var y = 0; y < EXPORT_HEIGHT; y++)
				{
					for (var x = 0; x < EXPORT_WIDTH; x++)
					{
						var charFromView = AtariView.ViewBytes[OffsetX + x, OffsetY + y];
						var rx = charFromView % 32;
						var ry = charFromView / 32;

						destRect.X = x * 16;
						destRect.Y = y * 16;

						srcRect.X = rx * 16;
						srcRect.Y = ry * 16 + Constants.FontYOffset[AtariView.UseFontOnLine[OffsetY + y] - 1] + colorOffset;

						gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, wrapMode);
					}
				}
			}

			pictureBoxAtariViewSmall.Refresh();
		}

		private void Button_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void ComboBoxExportType_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxDataType.Text = string.Empty;
			ComboBoxDataType.Items.Clear();
			ComboBoxDataType.Enabled = true;
			/* Binary data, assembler, action, basic, fastbasic, mads_dta, basic LST */
			MemoExport.Enabled = true;

			switch ((FormatTypes)ComboBoxExportType.SelectedIndex)
			{
				case FormatTypes.BinaryData:
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
				case FormatTypes.CDataArray:
				case FormatTypes.MadPascalArray:
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
			}

			ComboBoxDataTypeChange(this, EventArgs.Empty);
		}

		private void ComboBoxDataTypeChange(object sender, EventArgs e)
		{
			var exportType = (FormatTypes)ComboBoxExportType.SelectedIndex;

			if (ComboBoxExportType.SelectedIndex >= 0)
			{
				Button_Export.Enabled = true;
			}

			UpdatePreviewData();
		}

		private static (byte[], int, int) GetExportData(Rectangle exportRegion, bool transpose, bool withCompression)
		{
			// Find the view bytes and export them
			var regionSize = exportRegion.Width * exportRegion.Height;
			var exportBytes = new byte[regionSize];
			var writeIndex = 0;

			if (!transpose)
			{
				for (var y = exportRegion.Y; y < exportRegion.Y + exportRegion.Height; ++y)
				{
					for (var x = exportRegion.X; x < exportRegion.X + exportRegion.Width; ++x)
					{
						exportBytes[writeIndex++] = AtariView.ViewBytes[x, y];
					}
				}
			}
			else
			{
				for (var x = exportRegion.X; x < exportRegion.X + exportRegion.Width; ++x)
				for (var y = exportRegion.Y; y < exportRegion.Y + exportRegion.Height; ++y)
					exportBytes[writeIndex++] = AtariView.ViewBytes[x, y];
			}

			if (withCompression)
			{
				var compressedViewData = Compressors.Compress(exportBytes, _compressorId);
				if (compressedViewData.Length < exportBytes.Length)
				{
					exportBytes = compressedViewData;
				}
			}

			return (exportBytes, regionSize, exportBytes.Length);
		}

		/// <summary>
		/// Export data to assembler language, action! and atari basic
		/// </summary>
		/// <param name="exportRegion">Which region of the page to export</param>
		/// <param name="exportType">Export data format</param>
		/// <param name="hasHex"></param>
		/// <returns></returns>
		private static (string, int, int) GenerateFileAsText(Rectangle exportRegion, FormatTypes exportType, bool hasHex, bool transpose, bool withCompression)
		{
			var (viewBytes, inputSize, dataSize) = GetExportData(exportRegion, transpose, withCompression);

			var sb = new StringBuilder();

			var lineNumber = 10010;
			var charCounter = 0;

			if (exportType == FormatTypes.Assembler)
			{
				if (inputSize != dataSize)
				{
					sb.AppendLine($"\t; Original size: {inputSize} bytes");
					sb.AppendLine($"\t; {_compressorName} compressed size: {dataSize} bytes");
				}
				else
					sb.AppendLine($"\t; Size: {inputSize} bytes");
				sb.Append("\t.BYTE ");
			}

			if (exportType == FormatTypes.Action)
			{
				if (inputSize != dataSize)
				{
					sb.AppendLine($"; Original size: {inputSize} bytes");
					sb.AppendLine($"; {_compressorName} compressed size: {dataSize} bytes");
				}
				else
					sb.AppendLine($"; Size: {inputSize} bytes");
				sb.AppendLine("PROC VIEW=*()");
				sb.AppendLine("[");
			}

			if (exportType == FormatTypes.AtariBasic)
			{
				sb.AppendLine("10000 REM *** DATA VIEW ***");
				if (inputSize != dataSize)
					sb.AppendLine($"10001 REM Original size: {inputSize} bytes : {_compressorName} compressed size: {dataSize} bytes");
				else
					sb.AppendLine($"10001 REM Size: {inputSize} bytes");
				sb.Append("10010 DATA ");
			}

			if (exportType == FormatTypes.FastBasic)
			{
				if (inputSize != dataSize)
				{
					sb.AppendLine($"` Original size: {inputSize} bytes");
					sb.AppendLine($"` {_compressorName} compressed size: {dataSize} bytes");
				}
				else
					sb.AppendLine($"` Size: {inputSize} bytes");
				sb.Append("data view() byte = ");
			}

			if (exportType == FormatTypes.MADSdta)
			{
				if (inputSize != dataSize)
				{
					sb.AppendLine($"\t; Original size: {inputSize} bytes");
					sb.AppendLine($"\t; {_compressorName} compressed size: {dataSize} bytes");
				}
				else
					sb.AppendLine($"\t; Size: {inputSize} bytes");
				sb.Append("\tdta ");
			}

			if (exportType == FormatTypes.CDataArray)
			{
				if (inputSize != dataSize)
				{
					sb.AppendLine($"// Original size: {inputSize}");
					sb.AppendLine($"// {_compressorName} compressed size: {dataSize} bytes");
				}
				else
					sb.AppendLine($"// Size: {inputSize} bytes");
				sb.Append("{\n\t");
			}

			if (exportType == FormatTypes.MadPascalArray)
			{
				if (inputSize != dataSize)
				{
					sb.AppendLine($"// Original size: {inputSize} bytes");
					sb.AppendLine($"// {_compressorName} compressed size: {dataSize} bytes");
				}
				else
					sb.AppendLine($"// Size: {inputSize} bytes");
				sb.Append($"data: array [0..{viewBytes.Length - 1}] of byte = (\n\t");
			}
			

			var bytesLeft = dataSize;
			for (var index = 0; index < dataSize; index++)
			{
				if (hasHex)
				{
					if (exportType == FormatTypes.CDataArray)
						sb.Append($"0x{viewBytes[index]:X2}");
					else
						sb.Append($"${viewBytes[index]:X2}");
				}
				else
				{
					sb.Append($"{viewBytes[index]}");
				}

				--bytesLeft;

				++charCounter;

				if (charCounter == 8 && bytesLeft > 0)
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

				if (charCounter != 8 && charCounter != 0 && bytesLeft > 0)
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
				sb.Append("\n);\n");
			}

			return (sb.ToString(), inputSize, viewBytes.Length);
		}

		private void pictureBoxAtariViewSmall_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.X >= BITMAP_WIDTH || e.Y >= BITMAP_HEIGHT || e.X < 0 || e.Y < 0)
			{
				return;
			}

			var rx = e.X / CHAR_PIXEL_WIDTH;
			var ry = e.Y / 16;

			if (rx < 0 || rx >= 40 || ry < 0 || ry >= 26)
			{
				return;
			}

			switch (_selectionStatus)
			{
				case SelectionStatusFlags.None:
				case SelectionStatusFlags.Selected:
				{
					if (e.Button == MouseButtons.Left)
					{
						// Define copy origin point
						_exportRegion = new Rectangle(OffsetX + rx, OffsetY + ry, 1, 1);
						UpdateRegionEdits();
						_selectionStatus = SelectionStatusFlags.Selecting;

						ShowSelectionRubberBand();
					}
					break;
				}
			}
		}

		private void pictureBoxAtariViewSmall_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.X >= BITMAP_WIDTH || e.Y >= BITMAP_HEIGHT || e.X < 0 || e.Y < 0)
			{
				return;
			}

			var rx = e.X / CHAR_PIXEL_WIDTH;
			var ry = e.Y / 16;

			if (rx < 0 || rx >= 40 || ry < 0 || ry >= 26)
			{
				return;
			}

			// Adjust for screen offset
			rx += OffsetX;
			ry += OffsetY;

			if (_selectionStatus == SelectionStatusFlags.Selecting && e.Button == MouseButtons.Left)
			{
				if (ry <= _exportRegion.Y)
				{
					_exportRegion.Height = 1;
				}
				else
				{
					_exportRegion.Height = ry - _exportRegion.Y + 1;
				}

				if (rx <= _exportRegion.X)
				{
					_exportRegion.Width = 1;
				}
				else
				{
					_exportRegion.Width = rx - _exportRegion.X + 1;
				}

				UpdateRegionEdits();

				_selectionStatus = SelectionStatusFlags.Selected;

				timerUpdateExportSample.Enabled = true;
			}
		}

		private void pictureBoxAtariViewSmall_MouseMove(object sender, MouseEventArgs e)
		{
			switch (_selectionStatus)
			{
				case SelectionStatusFlags.Selecting:
				{
					if (e.X >= BITMAP_WIDTH || e.Y >= BITMAP_HEIGHT || e.X < 0 || e.Y < 0)
					{
						return;
					}

					var rx = OffsetX + e.X / CHAR_PIXEL_WIDTH;
					var ry = OffsetY + e.Y / 16;

					if (rx < 0 || rx >= AtariView.Width || ry < 0 || ry >= AtariView.Height)
					{
						// Note: This should not happen but when running on Mac things get funky
						return;
					}

					var origWidth = pictureBoxViewEditorRubberBand.Width;
					var origHeight = pictureBoxViewEditorRubberBand.Height;

					var w = 20;
					var h = 20;
					var temp = (rx - _exportRegion.X + 1) * 16 + 4;
					if (temp >= 20)
						w = temp;

					temp = (ry - _exportRegion.Y + 1) * 16 + 4;
					if (temp >= 20)
						h = temp;

					if (w != origWidth || h != origHeight)
					{
						pictureBoxViewEditorRubberBand.Size = new Size(w, h);
					}

					if (ry <= _exportRegion.Y)
					{
						_exportRegion.Height = 1;
					}
					else
					{
						_exportRegion.Height = ry - _exportRegion.Y + 1;
					}

					if (rx <= _exportRegion.X)
					{
						_exportRegion.Width = 1;
					}
					else
					{
						_exportRegion.Width = rx - _exportRegion.X + 1;
					}
					UpdateRegionEdits();
					break;
				}

			}
		}

		private static bool _busyWith_ViewEditorRubberBandResize;

		private void pictureBoxViewEditorRubberBand_Resize(object sender, EventArgs e)
		{
			if (_busyWith_ViewEditorRubberBandResize)
				return;
			_busyWith_ViewEditorRubberBandResize = true;

			var img = Helpers.NewImage(pictureBoxViewEditorRubberBand);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(cyanBrush, new Rectangle(0, 0, img.Width, img.Height));
				pictureBoxViewEditorRubberBand.Region?.Dispose();
				pictureBoxViewEditorRubberBand.Size = new Size(img.Width, img.Height);

			}
			using var graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, pictureBoxViewEditorRubberBand.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(pictureBoxViewEditorRubberBand.Width - 2, 0, 2, pictureBoxViewEditorRubberBand.Height));
			graphicsPath.AddRectangle(new Rectangle(0, pictureBoxViewEditorRubberBand.Height - 2, pictureBoxViewEditorRubberBand.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, pictureBoxViewEditorRubberBand.Height));
			pictureBoxViewEditorRubberBand.Region = new Region(graphicsPath);

			_busyWith_ViewEditorRubberBandResize = false;
		}

		private void numericFromX_ValueChanged(object sender, EventArgs e)
		{
			if (_selectionStatus is SelectionStatusFlags.None or SelectionStatusFlags.Selected)
			{
				_exportRegion.X = (int)numericFromX.Value;
				if (_exportRegion.X + _exportRegion.Width > AtariView.Width)
					_exportRegion.Width = AtariView.Width - _exportRegion.X;
				UpdateRegionEdits();

				ShowSelectionRubberBand();
				timerUpdateExportSample.Enabled = true;
			}
		}

		private void numericWidth_ValueChanged(object sender, EventArgs e)
		{
			if (_selectionStatus is SelectionStatusFlags.None or SelectionStatusFlags.Selected)
			{
				_exportRegion.Width = (int)numericWidth.Value;
				if (_exportRegion.X + _exportRegion.Width > AtariView.Width)
					_exportRegion.Width = AtariView.Width - _exportRegion.X;
				UpdateRegionEdits();

				ShowSelectionRubberBand();
				timerUpdateExportSample.Enabled = true;
			}
		}

		private void numericFromY_ValueChanged(object sender, EventArgs e)
		{
			if (_selectionStatus is SelectionStatusFlags.None or SelectionStatusFlags.Selected)
			{
				_exportRegion.Y = (int)numericFromY.Value;
				if (_exportRegion.Y + _exportRegion.Height > AtariView.Height)
					_exportRegion.Height = AtariView.Height - _exportRegion.Y;
				UpdateRegionEdits();

				ShowSelectionRubberBand();
				timerUpdateExportSample.Enabled = true;
			}
		}

		private void numericHeight_ValueChanged(object sender, EventArgs e)
		{
			if (_selectionStatus is SelectionStatusFlags.None or SelectionStatusFlags.Selected)
			{
				_exportRegion.Height = (int)numericHeight.Value;
				if (_exportRegion.Y + _exportRegion.Height > AtariView.Height)
					_exportRegion.Height = AtariView.Height - _exportRegion.Y;
				UpdateRegionEdits();

				ShowSelectionRubberBand();
				timerUpdateExportSample.Enabled = true;
			}
		}

		private void timerUpdateExportSample_Tick(object sender, EventArgs e)
		{
			timerUpdateExportSample.Enabled = false;

			if ((FormatTypes)ComboBoxExportType.SelectedIndex > FormatTypes.BinaryData)
			{
				UpdatePreviewData();
			}
		}

		private void buttonResetSelection_Click(object sender, EventArgs e)
		{
			_exportRegion = new Rectangle(0, 0, AtariView.Width, AtariView.Height);
			UpdateRegionEdits();
			ShowSelectionRubberBand();
		}

		private void ShowSelectionRubberBand()
		{
			var rect = new Rectangle(_exportRegion.X, _exportRegion.Y, _exportRegion.Width, _exportRegion.Height);
			rect.Offset(-OffsetX, -OffsetY);

			var targetRect = new Rectangle(0, 0, EXPORT_WIDTH, EXPORT_HEIGHT);
			rect.Intersect(targetRect);

			if (rect.IsEmpty)
			{
				// Selection box if out of bounds.
				// So hide it
				pictureBoxViewEditorRubberBand.Visible = false;
				return;
			}
			// Move the selection cursor

			pictureBoxViewEditorRubberBand.SetBounds(pictureBoxAtariViewSmall.Left - 2 + rect.X * 16, pictureBoxAtariViewSmall.Top - 2 + rect.Y * 16, rect.Width * 16 + 4, rect.Height * 16 + 4);
			pictureBoxViewEditorRubberBand.Visible = true;
		}

		private void ButtonCopyClipboard_Click(object sender, EventArgs e)
		{
			if (MemoExport.Text.Length > 0)
			{
				try
				{
					Clipboard.SetText(MemoExport.Text);
				}
				catch
				{

				}
			}
		}

		private void ButtonExport_Click(object sender, EventArgs e)
		{
			if ((FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.BinaryData)
			{
				saveDialog.Filter = @"View (*.dat)|*.dat";
				saveDialog.DefaultExt = "dat";
				saveDialog.FileName = @"View.dat";

				if (saveDialog.ShowDialog() == DialogResult.OK)
				{
					SaveAsBinaryData(saveDialog.FileName, _exportRegion, checkBoxTranspose.Checked);
				}

				return;
			}

			// Rest of the options are text / .txt
			saveDialog.Filter = @"View (*.txt)|*.txt";
			saveDialog.DefaultExt = "txt";

			if (saveDialog.ShowDialog() == DialogResult.OK)
			{
				var (text, _, __) = GenerateFileAsText(
					_exportRegion, 
					(FormatTypes)ComboBoxExportType.SelectedIndex, 
					ComboBoxDataType.SelectedIndex == 1, 
					checkBoxTranspose.Checked,
					withCompression.Checked);
				File.WriteAllText(saveDialog.FileName, text);
			}

		}

		private void SaveAsBinaryData(string fileName, Rectangle exportRegion, bool transpose)
		{
			var exportSize = exportRegion.Width * exportRegion.Height;
			var viewBytes = new byte[exportSize];
			var writeIndex = 0;
			if (!transpose)
			{
				for (var y = exportRegion.Y; y < exportRegion.Y + exportRegion.Height; ++y)
				{
					for (var x = exportRegion.X; x < exportRegion.X + exportRegion.Width; ++x)
					{
						viewBytes[writeIndex++] = AtariView.ViewBytes[x, y];
					}
				}
			}
			else
			{
				for (var x = exportRegion.X; x < exportRegion.X + exportRegion.Width; ++x)
					for (var y = exportRegion.Y; y < exportRegion.Y + exportRegion.Height; ++y)
						viewBytes[writeIndex++] = AtariView.ViewBytes[x, y];
			}

			try
			{
				File.WriteAllBytes(fileName, viewBytes);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Unable to export binary data. Have error: {ex.Message}");
			}
		}

		/// <summary>
		/// Handle Ctrl+C which will copy the exported text to the clipboard
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportViewWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.C)
			{
				if (MemoExport.Text.Length > 0)
				{
					try
					{
						Clipboard.SetText(MemoExport.Text);
					}
					catch
					{

					}
				}
			}
		}

		/// <summary>
		/// Move the export selection box around the screen.
		/// Mouse wheel Up/Down moves in the X-axis.
		/// Ctrl + Mouse wheel Up/Down moves in the Y-axis.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportViewWindow_MouseWheel(object sender, MouseEventArgs e)
		{
			var stepX = e.Delta > 0 ? -1 : 1;
			var stepY = 0;
			if (Control.ModifierKeys == Keys.Control)
			{
				stepY = stepX;
				stepX = 0;
			}

			var newRect = _exportRegion;
			newRect.Offset(stepX, stepY);
			if (newRect.IntersectsWith(FullViewRegion))
			{
				var i = Rectangle.Intersect(FullViewRegion, newRect);
				if (newRect == i)
				{
					_exportRegion = newRect;
				}
				else
					return;
			}

			UpdateRegionEdits();
			ShowSelectionRubberBand();
		}

		private void CheckBoxTranspose_CheckedChanged(object sender, EventArgs e)
		{
			if ((FormatTypes)ComboBoxExportType.SelectedIndex > FormatTypes.BinaryData)
			{
				UpdatePreviewData();
			}
		}

		private void numericOffsetX_ValueChanged(object sender, EventArgs e)
		{
			if (InOffsetUpdate) return;
			InOffsetUpdate = true;

			OffsetX = (int)numericOffsetX.Value;
			hScrollBar.Value = OffsetX;

			InOffsetUpdate = false;
			RedrawSmallView();
			ShowSelectionRubberBand();
		}

		private void numericOffsetY_ValueChanged(object sender, EventArgs e)
		{
			if (InOffsetUpdate) return;
			InOffsetUpdate = true;

			OffsetY = (int)numericOffsetY.Value;
			vScrollBar.Value = OffsetY;

			InOffsetUpdate = false;
			RedrawSmallView();
			ShowSelectionRubberBand();
		}

		private void hScrollBar_ValueChanged(object sender, EventArgs e)
		{
			if (InOffsetUpdate) return;
			InOffsetUpdate = true;

			OffsetX = (int)hScrollBar.Value;
			numericOffsetX.Value = OffsetX;

			InOffsetUpdate = false;
			RedrawSmallView();
			ShowSelectionRubberBand();
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			if (InOffsetUpdate) return;
			InOffsetUpdate = true;

			OffsetY = (int)vScrollBar.Value;
			numericOffsetY.Value = OffsetY;

			InOffsetUpdate = false;

			RedrawSmallView();
			ShowSelectionRubberBand();
		}

		private void WithCompressionCheckedChanged(object sender, EventArgs e)
		{
			UpdatePreviewData();
		}

		private void UpdatePreviewData()
		{
			switch ((FormatTypes)ComboBoxExportType.SelectedIndex)
			{
				case FormatTypes.BinaryData:
				{
					var (viewBytes, originalSize, dataSize) = GetExportData(_exportRegion, checkBoxTranspose.Checked, withCompression.Checked);

					labelSizeInfo.Text = originalSize != dataSize ? $"Original export size: {originalSize} bytes  Compressed export size: {dataSize} bytes" : $"export Size:{originalSize} bytes";

					break;
				}

				default:
				{
					var (newText, originalSize, dataSize) = GenerateFileAsText(
						_exportRegion,
						(FormatTypes)ComboBoxExportType.SelectedIndex,
						ComboBoxDataType.SelectedIndex == 1,
						checkBoxTranspose.Checked,
						withCompression.Checked);

					MemoExport.Text = newText;

					labelSizeInfo.Text = originalSize != dataSize ? $"Original size: {originalSize} bytes, compressed size: {dataSize} bytes" : $"Data size:{originalSize} bytes";

					break;
				}
			}
		}
	}
}
