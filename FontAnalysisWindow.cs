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

		private int PreviousHighlightColor { get; set; } = 0;
		private int PreviousAlpha { get; set; } = 128;
		#endregion

		private bool _inLoad;
		private SolidBrush? _drawBrush;

		internal class AnalysisDetailsEntry
		{
			public int PageIndex { get; set; }
			public int FirstOccurrenceIndex { get; set; }
		}
		private readonly List<AnalysisDetailsEntry> _analysisDetailsPerLine = new List<AnalysisDetailsEntry>();
		private readonly int[] _fullFontCharCounter = new int[256 * 4];       // Normal and inverse characters separated
		private readonly int[] _combinedCharCounter = new int[128 * 4];       // Normal and inverse characters counted as one

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
		public void SetDefaults(int whichColor, int whichAlpha)
		{
			PreviousHighlightColor = whichColor;
			PreviousAlpha = whichAlpha;
		}

		public int GetHighlightColor => PreviousHighlightColor;
		public int GetHighlightAlpha => PreviousAlpha;

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

			labelClickForUsageInfo.Visible = (nrUsed > 0) ? true : false;
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
	}
}
