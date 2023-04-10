using System.Drawing.Drawing2D;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FontMaker
{
	public partial class ExportViewWindow : Form
	{
		public enum FormatTypes
		{
			BinaryData,
			Assembler,
			Action,
			AtariBasic,
			FastBasic,
			MADSdta,
		};

		public enum SelectionStatusFlags
		{
			None, Selecting, Selected
		}

		private static readonly SolidBrush cyanBrush = new(Color.Cyan);

		private Rectangle _exportRegion;

		private SelectionStatusFlags _selectionStatus;

		public ExportViewWindow()
		{
			InitializeComponent();
		}

		private void ExportViewWindow_Load(object sender, EventArgs e)
		{
			RedrawSmallView();
			MemoExport.Clear();

			ComboBoxExportType.SelectedIndex = -1;
			ComboBoxDataType.SelectedIndex = -1;

			ComboBoxExportType.SelectedIndex = 0;   // This will fire the export type handler and setup the rest of the GUI

			_exportRegion = new Rectangle(0, 0, 40, 26);

			UpdateRegionEdits();

			pictureBoxViewEditorRubberBand.SetBounds(pictureBoxAtariViewSmall.Left - 2, pictureBoxAtariViewSmall.Top - 2, _exportRegion.Width * 8 + 4, _exportRegion.Height * 8 + 4);
			pictureBoxViewEditorRubberBand.Visible = true;
		}

		private void UpdateRegionEdits()
		{
			numericFromX.Value = _exportRegion.X;
			numericFromY.Value = _exportRegion.Y;
			numericWidth.Value = _exportRegion.Width;
			numericHeight.Value = _exportRegion.Height;

			UpdateRegionLabel();
		}

		private void UpdateRegionLabel()
		{
			labelDimensions.Text = $"({_exportRegion.X}, {_exportRegion.Y}) - ({_exportRegion.Width}, {_exportRegion.Height}) @ {(_exportRegion.Width * _exportRegion.Height)} bytes";
		}

		public void RedrawSmallView()
		{
			var colorOffset = false ? 512 : 0;
			var img = Helpers.GetImage(pictureBoxAtariViewSmall);
			using (var gr = Graphics.FromImage(img))
			{
				var destRect = new Rectangle
				{
					Width = 1,
					Height = 1,
				};

				var srcRect = new Rectangle
				{
					Width = 1,
					Height = 1,
				};

				for (var y = 0; y < AtariView.VIEW_HEIGHT; y++)
				{
					for (var x = 0; x < AtariView.VIEW_WIDTH; x++)
					{
						var rx = AtariView.ViewBytes[x, y] % 32;
						var ry = AtariView.ViewBytes[x, y] / 32;

						destRect.X = x * 8;
						destRect.Y = y * 8;

						srcRect.X = rx * 16;
						srcRect.Y = ry * 16 + Constants.FontYOffset[AtariView.UseFontOnLine[y] - 1] + colorOffset;

						for (var h = 0; h < 8; ++h)
						{
							for (var w = 0; w < 8; ++w)
							{
								gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect, GraphicsUnit.Pixel);
								destRect.X++;
								srcRect.X += 2;
							}

							destRect.X -= 8;
							srcRect.X -= 16;
							destRect.Y++;
							srcRect.Y += 2;
						}

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
			if (ComboBoxExportType.SelectedIndex >= 0)
			{
				Button_Export.Enabled = true;
			}

			if ((FormatTypes)ComboBoxExportType.SelectedIndex > FormatTypes.BinaryData)
			{
				MemoExport.Text = GenerateFileAsText(
					_exportRegion,
					(FormatTypes)ComboBoxExportType.SelectedIndex,
					ComboBoxDataType.SelectedIndex);
			}
		}

		/// <summary>
		/// Export data to assembler language, action! and atari basic
		/// </summary>
		/// <param name="exportType"></param>
		/// <param name="dataType"></param>
		/// <returns></returns>
		public static string GenerateFileAsText(Rectangle exportRegion, FormatTypes exportType, int dataType)
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
				sb.AppendLine("PROC view()=[");
			}

			if (exportType == FormatTypes.AtariBasic)
			{
				sb.AppendLine("10000 REM *** DATA VIEW ***");
				sb.Append("10010 DATA ");
			}

			if (exportType == FormatTypes.FastBasic)
			{
				sb.Append("data view() byte = ");
			}

			if (exportType == FormatTypes.MADSdta)
			{
				sb.Append("\tdta ");
			}

			// Find the view bytes and export them
			var exportSize = exportRegion.Width * exportRegion.Height;
			var viewBytes = new byte[exportSize];
			var writeIndex = 0;
			for (var y = exportRegion.Y; y < exportRegion.Y + exportRegion.Height; ++y)
			{
				for (var x = exportRegion.X; x < exportRegion.X + exportRegion.Width; ++x)
				{
					viewBytes[writeIndex++] = AtariView.ViewBytes[x, y];
				}
			}

			var bytesLeft = exportSize;
			for (var index = 0; index < exportSize; index++)
			{
				if (dataType == 1)
				{
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
					line++;

					if (exportType == FormatTypes.FastBasic)
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
				}

				if (charCounter != 8 && charCounter != 0 && bytesLeft > 0)
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

		private void pictureBoxAtariViewSmall_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.X >= pictureBoxAtariViewSmall.Width || e.Y >= pictureBoxAtariViewSmall.Height || e.X < 0 || e.Y < 0)
			{
				return;
			}

			var rx = e.X / 8;
			var ry = e.Y / 8;

			switch (_selectionStatus)
			{
				case SelectionStatusFlags.None:
				case SelectionStatusFlags.Selected:
					{
						if (e.Button == MouseButtons.Left)
						{
							// Define copy origin point
							_exportRegion = new Rectangle(rx, ry, 1, 1);
							UpdateRegionEdits();
							_selectionStatus = SelectionStatusFlags.Selecting;

							pictureBoxViewEditorRubberBand.SetBounds(pictureBoxAtariViewSmall.Left + e.X - e.X % 8 - 2, pictureBoxAtariViewSmall.Top + e.Y - e.Y % 8 - 2, 10, 10);
							pictureBoxViewEditorRubberBand.Visible = true;
						}
						break;
					}
			}
		}

		private void pictureBoxAtariViewSmall_MouseUp(object sender, MouseEventArgs e)
		{
			if ((e.X >= pictureBoxAtariViewSmall.Width) || (e.Y >= pictureBoxAtariViewSmall.Height))
			{
				return;
			}

			var rx = e.X / 8;
			var ry = e.Y / 8;

			switch (_selectionStatus)
			{
				case SelectionStatusFlags.Selecting:
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
						break;
					}
			}
		}

		private void pictureBoxAtariViewSmall_MouseMove(object sender, MouseEventArgs e)
		{
			switch (_selectionStatus)
			{
				case SelectionStatusFlags.Selecting:
					{
						if (e.X >= pictureBoxAtariViewSmall.Width || e.Y >= pictureBoxAtariViewSmall.Height)
						{
							return;
						}

						var rx = e.X / 8;
						var ry = e.Y / 8;

						var origWidth = pictureBoxViewEditorRubberBand.Width;
						var origHeight = pictureBoxViewEditorRubberBand.Height;

						var w = 10;
						var h = 10;
						var temp = (rx - _exportRegion.X + 1) * 8 + 4;
						if (temp >= 10)
							w = temp;

						temp = (ry - _exportRegion.Y + 1) * 8 + 4;
						if (temp >= 10)
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

		private static bool _busyWith_ViewEditorRubberBandResize = false;

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
			var graphicsPath = new GraphicsPath();
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
				if (_exportRegion.X + _exportRegion.Width > 40)
					_exportRegion.Width = 40 - _exportRegion.X;
				UpdateRegionEdits();

				pictureBoxViewEditorRubberBand.SetBounds(pictureBoxAtariViewSmall.Left + _exportRegion.X * 8 - 2, pictureBoxAtariViewSmall.Top + _exportRegion.Y * 8 - 2, _exportRegion.Width * 8 + 4, _exportRegion.Height * 8 + 4);

				timerUpdateExportSample.Enabled = true;
			}
		}

		private void numericWidth_ValueChanged(object sender, EventArgs e)
		{
			if (_selectionStatus is SelectionStatusFlags.None or SelectionStatusFlags.Selected)
			{
				_exportRegion.Width = (int)numericWidth.Value;
				if (_exportRegion.X + _exportRegion.Width > 40)
					_exportRegion.Width = 40 - _exportRegion.X;
				UpdateRegionEdits();

				pictureBoxViewEditorRubberBand.SetBounds(pictureBoxAtariViewSmall.Left + _exportRegion.X * 8 - 2, pictureBoxAtariViewSmall.Top + _exportRegion.Y * 8 - 2, _exportRegion.Width * 8 + 4, _exportRegion.Height * 8 + 4);

				timerUpdateExportSample.Enabled = true;
			}
		}

		private void numericFromY_ValueChanged(object sender, EventArgs e)
		{
			if (_selectionStatus is SelectionStatusFlags.None or SelectionStatusFlags.Selected)
			{
				_exportRegion.Y = (int)numericFromY.Value;
				if (_exportRegion.Y + _exportRegion.Height > 26)
					_exportRegion.Height = 26 - _exportRegion.Y;
				UpdateRegionEdits();

				pictureBoxViewEditorRubberBand.SetBounds(pictureBoxAtariViewSmall.Left + _exportRegion.X * 8 - 2, pictureBoxAtariViewSmall.Top + _exportRegion.Y * 8 - 2, _exportRegion.Width * 8 + 4, _exportRegion.Height * 8 + 4);

				timerUpdateExportSample.Enabled = true;
			}
		}

		private void numericHeight_ValueChanged(object sender, EventArgs e)
		{
			if (_selectionStatus is SelectionStatusFlags.None or SelectionStatusFlags.Selected)
			{
				_exportRegion.Height = (int)numericHeight.Value;
				if (_exportRegion.Y + _exportRegion.Height > 26)
					_exportRegion.Height = 26 - _exportRegion.Y;
				UpdateRegionEdits();

				pictureBoxViewEditorRubberBand.SetBounds(pictureBoxAtariViewSmall.Left + _exportRegion.X * 8 - 2, pictureBoxAtariViewSmall.Top + _exportRegion.Y * 8 - 2, _exportRegion.Width * 8 + 4, _exportRegion.Height * 8 + 4);

				timerUpdateExportSample.Enabled = true;
			}
		}

		private void timerUpdateExportSample_Tick(object sender, EventArgs e)
		{
			timerUpdateExportSample.Enabled = false;

			if ((FormatTypes)ComboBoxExportType.SelectedIndex > FormatTypes.BinaryData)
			{
				MemoExport.Text = GenerateFileAsText(
					_exportRegion,
					(FormatTypes)ComboBoxExportType.SelectedIndex,
					ComboBoxDataType.SelectedIndex);
			}
		}

		private void buttonResetSelection_Click(object sender, EventArgs e)
		{
			_exportRegion = new Rectangle(0, 0, 40, 26);
			UpdateRegionEdits();
			pictureBoxViewEditorRubberBand.SetBounds(pictureBoxAtariViewSmall.Left - 2, pictureBoxAtariViewSmall.Top - 2, _exportRegion.Width * 8 + 4, _exportRegion.Height * 8 + 4);
		}

		private void ButtonCopyClipboard_Click(object sender, EventArgs e)
		{
			if (MemoExport.Text.Length > 0)
				Clipboard.SetText(MemoExport.Text);
		}

		private void ButtonExport_Click(object sender, EventArgs e)
		{
			if ((FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.BinaryData)
			{
				saveDialog.Filter = $@"View (*.dat)|*.dat";
				saveDialog.DefaultExt = "dat";
				saveDialog.FileName = $@"View.dat";

				if (saveDialog.ShowDialog() == DialogResult.OK)
				{
					SaveAsBinaryData(saveDialog.FileName, _exportRegion);
				}

				return;
			}

			// Rest of the options are text / .txt
			saveDialog.Filter = $@"View (*.txt)|*.txt";
			saveDialog.DefaultExt = "txt";

			if (saveDialog.ShowDialog() == DialogResult.OK)
			{
				var text = GenerateFileAsText(_exportRegion, (FormatTypes)ComboBoxExportType.SelectedIndex, ComboBoxDataType.SelectedIndex);
				File.WriteAllText(saveDialog.FileName, text);
			}

		}

		private void SaveAsBinaryData(string fileName, Rectangle exportRegion)
		{
			var exportSize = exportRegion.Width * exportRegion.Height;
			var viewBytes = new byte[exportSize];
			var writeIndex = 0;
			for (var y = exportRegion.Y; y < exportRegion.Y + exportRegion.Height; ++y)
			{
				for (var x = exportRegion.X; x < exportRegion.X + exportRegion.Width; ++x)
				{
					viewBytes[writeIndex++] = AtariView.ViewBytes[x, y];
				}
			}

			try
			{
				using var writer = new BinaryWriter(File.OpenWrite(fileName));
				writer.Write(viewBytes);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Unable to export binary data. Have error: {ex.Message}");
			}
		}
	}
}
