using System.Drawing.Drawing2D;
using System.Text;

namespace FontMaker
{
	/// <summary>
	/// Analyze how many times each character in the 4 fonts is being used in the various pages
	/// </summary>
	public partial class FontAnalysisWindow : Form
	{
		#region Load/Save ranges and values
		public static int AnalysisMinColorIndex = 0;
		public static int AnalysisMaxColorIndex = 4;
		public static int AnalysisMinAlpha = 64;
		public static int AnalysisMaxAlpha = 192;

		private int PreviousHighlightColor { get; set; }
		private int PreviousAlpha { get; set; } = 128;
		private bool PreviousDuplicates { get; set; }
		#endregion

		private bool _inLoad;
		private SolidBrush? _drawBrush;

		internal class AnalysisDetailsEntry
		{
			public int PageIndex { get; set; }
			public int FirstOccurrenceIndex { get; set; }
		}
		private readonly List<AnalysisDetailsEntry> _analysisDetailsPerLine = new();
		private readonly int[] _fullFontCharCounter = new int[256 * 4];       // Normal and inverse characters separated
		private readonly int[] _combinedCharCounter = new int[128 * 4];       // Normal and inverse characters counted as one



		private readonly int[] _duplicateOfChar = new int[128 * 4];             // Indicate if a char at position x is a duplicate of another char.  If its -1 then no duplicate
		private readonly List<int> _duplicateDetailsPerLine = new();

		/// <summary>
		/// Which char was selected for detailed analysis?
		/// </summary>
		private int SelectedCharacterIndex { get; set; }

		#region Data from the outside
		public bool InColorMode { get; set; }
		public List<PageData>? Pages { get; set; }
		#endregion

		/// <summary>
		/// Setup the analysis window
		/// </summary>
		public FontAnalysisWindow()
		{
			InitializeComponent();

			// Paint something into the window
			var img = Helpers.GetImage(pictureBoxCursor);
			using (var gr = Graphics.FromImage(img))
			{
				using var trans = new SolidBrush(Color.Yellow);
				gr.FillRectangle(trans, new Rectangle(0, 0, img.Width, img.Height));
			}
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			pictureBoxCursor.Region = new Region(graphicsPath);
			pictureBoxCursor.Refresh();
			graphicsPath.Dispose();

			pictureBoxCursor.SetBounds(0, 0, 20, 20);
			pictureBoxCursor.Visible = true;


			img = Helpers.GetImage(pictureBoxInfoCursor);
			using (var gr = Graphics.FromImage(img))
			{
				using var trans = new SolidBrush(Color.Red);
				gr.FillRectangle(trans, new Rectangle(0, 0, img.Width, img.Height));
			}

			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 4));
			graphicsPath.AddRectangle(new Rectangle(16, 0, 4, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 16, 20, 4));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 4, 20));

			pictureBoxInfoCursor.Region = new Region(graphicsPath);
			pictureBoxInfoCursor.Refresh();
			graphicsPath.Dispose();

			pictureBoxInfoCursor.SetBounds(0, 0, 20, 20);
			pictureBoxInfoCursor.Visible = false;

			MakeBrushes();
		}

		/// <summary>
		/// Create the brush to draw over the characters with
		/// </summary>
		private void MakeBrushes()
		{
			if (_drawBrush != null) _drawBrush.Dispose();

			var alpha = trackBarAlpha.Value;
			// Which color
			if (radioButtonRed.Checked)
			{
				_drawBrush = new SolidBrush(Color.FromArgb(alpha, 255, 0, 0));
			}
			else if (radioButtonGreen.Checked)
			{
				_drawBrush = new SolidBrush(Color.FromArgb(alpha, 0, 255, 0));
			}
			else if (radioButtonBlue.Checked)
			{
				_drawBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 255));
			}
			else if (radioButtonBlack.Checked)
			{
				_drawBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
			}
			else
			{
				_drawBrush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 0));
			}
		}

		#region Load/Save settings for later restore
		public void SetDefaults(int whichColor, int whichAlpha, bool markDuplicates)
		{
			PreviousHighlightColor = whichColor;
			PreviousAlpha = whichAlpha;
			PreviousDuplicates = markDuplicates;
		}

		public int GetHighlightColor => PreviousHighlightColor;
		public int GetHighlightAlpha => PreviousAlpha;
		public bool GetDuplicates => PreviousDuplicates;

		#endregion

		private void FontAnalysisWindow_Load(object sender, EventArgs e)
		{
			_inLoad = true;
			switch (PreviousHighlightColor)
			{
				case 0:
					radioButtonRed.Checked = true;
					break;
				case 1:
					radioButtonGreen.Checked = true;
					break;
				case 2:
					radioButtonBlue.Checked = true;
					break;
				case 3:
					radioButtonBlack.Checked = true;
					break;
				case 4:
					radioButtonYellow.Checked = true;
					break;
			}

			trackBarAlpha.Value = PreviousAlpha;
			chkMarkDuplicates.Checked = PreviousDuplicates;

			_inLoad = false;

			MakeBrushes();

			DoAnalysis();

			checkBoxGFX.Checked = InColorMode;
			RedrawAnalysisView();

			ShowQuickCharacterInfo(0, 0);
		}
		private void FontAnalysisWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (radioButtonRed.Checked)
				PreviousHighlightColor = 0;
			else if (radioButtonGreen.Checked)
				PreviousHighlightColor = 1;
			else if (radioButtonBlue.Checked)
				PreviousHighlightColor = 2;
			else if (radioButtonBlack.Checked)
				PreviousHighlightColor = 3;
			else
				PreviousHighlightColor = 4;

			PreviousAlpha = trackBarAlpha.Value;
			PreviousDuplicates = chkMarkDuplicates.Checked;
		}

		/// <summary>
		/// Draw the font characters (in small format) each in a 10x10 block
		/// </summary>
		private void RedrawAnalysisView()
		{
			var src = new Rectangle(0, 0, 512, 512);
			if (InColorMode)
			{
				src.Offset(0, 512);
			}
			// var colorOffset = InColorMode ? 512 : 0;
			var img = Helpers.GetImage(pictureBoxFonts);
			using (var gr = Graphics.FromImage(img))
			{
				// Copy font bank 1 or 2
				gr.DrawImage(AtariFontRenderer.BitmapFontBanks, 0, 0, src, GraphicsUnit.Pixel);

				// Draw the analysis results onto the bitmap
				// A red rectangle over each character that has a 0 usage count
				for (var ch = 0; ch < 4 * 128; ++ch)
				{
					var yOffset = (ch / 128) * 64;
					if (_combinedCharCounter[ch] == 0)
					{
						var x = (ch * 16) % 512;
						var y = (ch / 32) * 16;
						gr.FillRectangle(_drawBrush, x, yOffset + y, 16, 16);
						gr.FillRectangle(_drawBrush, x, yOffset + y + 64, 16, 16);
					}
				}

				// Check if we want to mark duplicate characters in a font
				if (chkMarkDuplicates.Checked)
				{
					using var pen = new Pen(_drawBrush);
					// Yup, draw the marks
					for (var fontNr = 0; fontNr < 4; ++fontNr)
					{
						var fontIndex = 128 * fontNr;
						var yOffset = fontNr * 128;
						for (var ch = 0; ch < 128; ++ch)
						{
							if (_duplicateOfChar[fontIndex + ch] != -1)
							{
								var fromX = (ch * 16) % 512;
								var fromY = (ch / 32) * 16;
								var toX = (_duplicateOfChar[fontIndex + ch] * 16) % 512;
								var toY = (_duplicateOfChar[fontIndex + ch] / 32) * 16;

								gr.DrawLine(pen, 8 + fromX, yOffset + 8 + fromY, 8 + toX, yOffset + 8 + toY);
								gr.DrawLine(pen, 8 + fromX, yOffset + 64 + 8 + fromY, 8 + toX, yOffset + 64 + 8 + toY);
								gr.DrawRectangle(pen, fromX, yOffset + fromY, 16, 16);
								gr.DrawRectangle(pen, fromX, yOffset + fromY + 64, 16, 16);
							}
						}
					}
				}
			}
			pictureBoxFonts.Refresh();
		}

		/// <summary>
		/// Switch between mono and color modes
		/// </summary>
		private void checkBoxGFX_CheckedChanged(object _, EventArgs __)
		{
			InColorMode = checkBoxGFX.Checked;
			RedrawAnalysisView();
		}

		private void DoAnalysis()
		{
			Array.Clear(_fullFontCharCounter);
			Array.Clear(_combinedCharCounter);

			Pages.ForEach(page =>
			{
				for (var y = 0; y < 26; ++y)
				{
					var fontNr = page.SelectedFont[y];
					var fullFontOffset = (fontNr - 1) * 256;
					var combinedFontOffset = (fontNr - 1) * 128;
					for (var x = 0; x < 40; ++x)
					{
						var thisChar = page.View[x, y];
						++_fullFontCharCounter[fullFontOffset + thisChar];
						// Convert the inverse to a normal font
						if (thisChar >= 128) thisChar -= 128;
						++_combinedCharCounter[combinedFontOffset + thisChar];
					}
				}
			});

			// Run over each character in a font and find if there are any duplicates.
			// 128 chars per font
			for (var i = 0; i < _duplicateOfChar.Length; ++i)
				_duplicateOfChar[i] = -1;

			for (var fontNr = 0; fontNr < 4; ++fontNr)
			{
				var fontIndex = fontNr * 128;
				for (var srcCharIndex = 0; srcCharIndex < 128; ++srcCharIndex)
				{
					for (var lookAtCharNr = 0; lookAtCharNr < 128; ++lookAtCharNr)
					{
						if (_duplicateOfChar[fontIndex + lookAtCharNr] == -1
							&& srcCharIndex != lookAtCharNr && AtariFont.IsDuplicate(fontNr, srcCharIndex, lookAtCharNr)
						   )
						{
							var minCharNr = Math.Min(lookAtCharNr, srcCharIndex);
							_duplicateOfChar[fontIndex + lookAtCharNr] = minCharNr;
						}
					}
				}
			}

			// Draw the analysis to the bitmap
			RedrawAnalysisView();
		}

		/// <summary>
		/// Find out on which page the character has been used
		/// </summary>
		/// <param name="fontNr"></param>
		/// <param name="baseCharNr"></param>
		private void DoDetailedUsageAnalysis(int fontNr, int baseCharNr)
		{
			_analysisDetailsPerLine.Clear();

			var invertedCharNr = baseCharNr + 128;
			var sb = new StringBuilder();

			var charsUsedOnPages = new List<string>();

			sb.AppendLine($"Font {fontNr + 1} Base:${baseCharNr:X2} #{baseCharNr} Inv:${baseCharNr + 128:X2} #{baseCharNr + 128}");

			Pages.ForEach(page =>
			{
				var baseCharNrTimesUsedOnThisPage = 0;
				var invertedCharNrTimesUsedOnThisPage = 0;
				var indexOfFirstOccurrence = -1;

				for (var y = 0; y < 26; ++y)
				{
					var pageFontNr = page.SelectedFont[y] - 1;      // Make it zero based
					if (pageFontNr == fontNr)
					{
						for (var x = 0; x < 40; ++x)
						{
							var pageFontChar = page.View[x, y];
							baseCharNrTimesUsedOnThisPage += baseCharNr == pageFontChar ? 1 : 0;
							invertedCharNrTimesUsedOnThisPage += invertedCharNr == pageFontChar ? 1 : 0;
							if (baseCharNrTimesUsedOnThisPage + invertedCharNrTimesUsedOnThisPage == 1)
							{
								indexOfFirstOccurrence = x + y * 40;
							}
						}
					}
				}

				if (baseCharNrTimesUsedOnThisPage + invertedCharNrTimesUsedOnThisPage > 0)
				{
					// Some combo of the chars was used
					_analysisDetailsPerLine.Add(new AnalysisDetailsEntry()
					{
						PageIndex = page.Index,
						FirstOccurrenceIndex = indexOfFirstOccurrence,
					});
					if (baseCharNrTimesUsedOnThisPage > 0 && invertedCharNrTimesUsedOnThisPage == 0)
					{
						// Only base char was used on this page
						charsUsedOnPages.Add($"[{page.Name}] used normal {baseCharNrTimesUsedOnThisPage}x");
					}
					else if (invertedCharNrTimesUsedOnThisPage > 0 && baseCharNrTimesUsedOnThisPage == 0)
					{
						// The inverted char was used on this page
						charsUsedOnPages.Add($"[{page.Name}] used inverted {invertedCharNrTimesUsedOnThisPage}x");
					}
					else
					{
						charsUsedOnPages.Add($"[{page.Name}] used normal {baseCharNrTimesUsedOnThisPage}x & inverted {invertedCharNrTimesUsedOnThisPage}x");
					}
				}
			});
			charsUsedOnPages.ForEach(info =>
			{
				sb.AppendLine(info);
			});

			textBoxUsageInfo.Text = sb.ToString();
		}

		private void DoDetailedDuplicateAnalysis(int fontNr, int baseCharNr)
		{
			_duplicateDetailsPerLine.Clear();

			var sb = new StringBuilder();
			sb.AppendLine($"Font {fontNr + 1} ${baseCharNr:X2} #{baseCharNr}");

			var fontOffset = fontNr * 128;
			var charToLookFor = _duplicateOfChar[fontNr * 128 + baseCharNr];
			for (var i = 0; i < 128; ++i)
			{
				if (_duplicateOfChar[fontOffset + i] == charToLookFor && baseCharNr != i)
				{
					sb.AppendLine($"Dup @ ${i:X2} #{i}");
					_duplicateDetailsPerLine.Add(fontOffset + i);
				}

			}
			textBoxDuplicates.Text = sb.ToString();

		}

		private void ShowQuickCharacterInfo(int x, int y)
		{
			var rx = x / 16;
			var ry = y / 16;
			var fontNr = ry / 8;
			var fontChar = (rx + ry * 32) % 256;
			var baseFontChar = fontChar % 128;
			var invFontChar = baseFontChar + 128;
			// var nrUsed = FullFontCharCounter[fontChar + fontNr * 256];
			var nrUsed = _combinedCharCounter[baseFontChar + fontNr * 128];

			pictureBoxCursor.SetBounds(pictureBoxFonts.Left + x - x % 16 - 2, pictureBoxFonts.Top + y - y % 16 - 2, 20, 20);

			labelCursorInfo.Text = $@"Font {fontNr + 1} ${fontChar:X2} #{fontChar} {(fontChar >= 128 ? "[Inverse]" : "")}";
			labelUsedInfo.Text = $@"Base used {_fullFontCharCounter[baseFontChar + fontNr * 256]} x. Inverse used {_fullFontCharCounter[invFontChar + fontNr * 256]} x";

			labelClickForUsageInfo.Visible = (nrUsed > 0);
		}

		private void ShowDetailedCharacterInfo(int x, int y)
		{
			var rx = x / 16;
			var ry = y / 16;
			var fontNr = ry / 8;
			var fontChar = (rx + ry * 32) % 256;
			var baseFontChar = fontChar % 128;

			SelectedCharacterIndex = rx + ry * 32;

			pictureBoxInfoCursor.SetBounds(pictureBoxFonts.Left + x - x % 16 - 2, pictureBoxFonts.Top + y - y % 16 - 2, 20, 20);
			pictureBoxInfoCursor.Visible = true;

			var nrUsed = _combinedCharCounter[baseFontChar + fontNr * 128];
			if (nrUsed > 0)
			{
				DoDetailedUsageAnalysis(fontNr, baseFontChar);
			}

			textBoxUsageInfo.Visible = nrUsed > 0;

			Program.MainForm.PickCharacter((rx + ry * 32));

			var showDuplicates = chkMarkDuplicates.Checked;
			if (showDuplicates)
			{
				// Check if the char has any duplicates
				if (_duplicateOfChar[fontNr * 128 + baseFontChar] != -1)
				{
					DoDetailedDuplicateAnalysis(fontNr, baseFontChar);
					showDuplicates = true;
				}
				else
				{
					showDuplicates = false;
				}
			}
			textBoxDuplicates.Visible = showDuplicates;
		}

		private void pictureBoxFonts_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.X >= pictureBoxFonts.Width || e.Y >= pictureBoxFonts.Height)
			{
				return;
			}

			ShowQuickCharacterInfo(e.X, e.Y);
		}

		private void pictureBoxFonts_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.X >= pictureBoxFonts.Width || e.Y >= pictureBoxFonts.Height)
			{
				return;
			}
			if (e.Button == MouseButtons.Right)
			{
				HideUsageInfo();
			}
			else
			{
				ShowDetailedCharacterInfo(e.X, e.Y);
			}
		}


		private void UpdateAfterGuiChange()
		{
			if (_inLoad) return;
			MakeBrushes();
			RedrawAnalysisView();
		}
		private void radioButtonRed_CheckedChanged(object sender, EventArgs e)
		{
			UpdateAfterGuiChange();
		}
		private void radioButtonGreen_CheckedChanged(object sender, EventArgs e)
		{
			UpdateAfterGuiChange();
		}
		private void radioButtonBlue_CheckedChanged(object sender, EventArgs e)
		{
			UpdateAfterGuiChange();
		}
		private void radioButtonBlack_CheckedChanged(object sender, EventArgs e)
		{
			UpdateAfterGuiChange();
		}
		private void radioButtonYellow_CheckedChanged(object sender, EventArgs e)
		{
			UpdateAfterGuiChange();
		}
		private void trackBarAlpha_Scroll(object sender, EventArgs e)
		{
			UpdateAfterGuiChange();
		}

		private void pictureBoxCursor_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				HideUsageInfo();
			}
			else
				pictureBoxFonts_MouseDown(sender, e);
		}

		private void HideUsageInfo()
		{
			pictureBoxInfoCursor.Visible = false;
			textBoxUsageInfo.Visible = false;
		}

		private void pictureBoxInfoCursor_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				HideUsageInfo();
			}
		}

		#region Mouse Wheel

		private void Form_MouseWheel(object sender, MouseEventArgs e)
		{
			var step = e.Delta > 0 ? -1 : 1;
			if (Control.ModifierKeys == Keys.Control)
			{
				step *= 32;
			}
			var nextCharacterIndex = SelectedCharacterIndex + step;

			nextCharacterIndex = nextCharacterIndex % 1024;
			if (nextCharacterIndex < 0)
				nextCharacterIndex += 1024;

			var bx = nextCharacterIndex % 32;
			var by = nextCharacterIndex / 32;
			pictureBoxFonts_MouseDown(null!, new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0));
		}

		#endregion


		private void textBoxUsageInfo_MouseDown(object sender, MouseEventArgs e)
		{
			var charIndex = textBoxUsageInfo.GetCharIndexFromPosition(e.Location);
			var lineNr = textBoxUsageInfo.GetLineFromCharIndex(charIndex);

			if (lineNr > 0)
			{
				var locationInfo = _analysisDetailsPerLine[lineNr - 1];
				Program.MainForm.PickPage(locationInfo.PageIndex);
			}
		}

		private void chkMarkDuplicates_CheckedChanged(object sender, EventArgs e)
		{
			UpdateAfterGuiChange();
		}

		private void textBoxDuplicates_MouseUp(object sender, MouseEventArgs e)
		{
			var charIndex = textBoxDuplicates.GetCharIndexFromPosition(e.Location);
			var lineNr = textBoxDuplicates.GetLineFromCharIndex(charIndex);

			if (lineNr > 0)
			{
				var nextCharacterIndex = _duplicateDetailsPerLine[lineNr - 1];
				var bx = nextCharacterIndex % 32;
				var by = (nextCharacterIndex / 32) % 4;
				var fontNr = nextCharacterIndex / 128;

				pictureBoxFonts_MouseDown(null!, new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, fontNr * 128 + by * 16 + 4, 0));
			}
		}
	}
}
