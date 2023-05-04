using Microsoft.VisualBasic;
using System.Drawing.Drawing2D;
using System.Text;
using TinyJson;

namespace FontMaker
{
	internal class AtariViewEditor
	{
	}

	public class AtrViewInfoJSON
	{
		public string Version { get; set; }
		/// <summary>
		/// Mode 4 indicator
		/// 1 = mode 4
		/// </summary>
		public string ColoredGfx { get; set; }

		/// <summary>
		/// Characters in the view
		/// </summary>
		public string Chars { get; set; }

		/// <summary>
		/// Use which font on which of the 26 lines of the view "1234"
		/// </summary>
		public string Lines { get; set; }

		/// <summary>
		/// Hex encoded array of the 6 selected colors
		/// </summary>
		public string Colors { get; set; }

		public string Fontname1 { get; set; }
		public string Fontname2 { get; set; }
		public string? Fontname3 { get; set; }
		public string? Fontname4 { get; set; }

		/// <summary>
		/// Hex encoded 1024 bytes per font
		/// </summary>
		public string Data { get; set; }

		public string FortyBytes { get; set; }

		public List<SavedPageData> Pages { get; set; }
	}


	// All functions that interact with the view can be found here
	public partial class FontMakerForm
	{
		/// <summary>
		/// View editor width indicator 32/40 characters wide
		/// </summary>
		internal string FortyBytes { get; set; } = string.Empty;

		internal int LastViewCharacterX { get; set; }
		internal int LastViewCharacterY { get; set; }

		internal bool ContinueViewDrawInMove { get; set; } = false;

		/// <summary>
		/// Detect a right-double-click and reset/cancel the mega copy paste mode
		/// </summary>
		/// <param name="e"></param>
		public void ActionAtariViewDoubleClick(MouseEventArgs e)
		{
			// Setup selection by right DoubleClick
			if (e.Button == MouseButtons.Right)
			{
				ResetMegaCopyStatus();
			}
		}

		/// <summary>
		/// Select a character for editing
		/// </summary>
		/// <param name="charNr">0 - 1023</param>
		public void PickCharacter(int charNr)
		{
			var bx = charNr % 32;
			var by = charNr / 32;

			var want2ndBank = by >= 16;
			// Check if the font bank needs to be switched
			if ((checkBoxFontBank.Checked == false && want2ndBank) ||
			    (checkBoxFontBank.Checked == true && !want2ndBank) )
			{
				checkBoxFontBank.Checked = !checkBoxFontBank.Checked;
				SwitchFontBank();
			}

			if (by > 16)
				by -= 16;

			var old_megaCopyStatus = megaCopyStatus;
			megaCopyStatus = MegaCopyStatusFlags.None;
			var mouseEvent = new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0);
			ActionFontSelectorMouseDown(mouseEvent);
			ActionFontSelectorMouseUp(mouseEvent);
			megaCopyStatus = old_megaCopyStatus;

			//ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 1, bx * 16 + 4, by * 16 + 4, 0));
		}

		public void PickPage(int pageIndex)
		{
			comboBoxPages.SelectedIndex = pageIndex = pageIndex;
		}

		public void ActionAtariViewEditorMouseDown(MouseEventArgs e)
		{
			if ((e.X >= pictureBoxAtariView.Width) || (e.Y >= pictureBoxAtariView.Height) || e.X < 0 || e.Y < 0)
			{
				return;
			}

			var rx = e.X / 16;
			var ry = e.Y / 16;

			if (buttonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case MegaCopyStatusFlags.None:
					case MegaCopyStatusFlags.Selected:
						{
							if (e.Button == MouseButtons.Left)
							{
								// Define copy origin point
								CopyPasteRange.Y = ry;
								CopyPasteRange.X = rx;
								megaCopyStatus = MegaCopyStatusFlags.Selecting;
								pictureBoxViewEditorRubberBand.Left = pictureBoxAtariView.Left + e.X - e.X % 16 - 2;
								pictureBoxViewEditorRubberBand.Top = pictureBoxAtariView.Top + e.Y - e.Y % 16 - 2;
								pictureBoxViewEditorRubberBand.Width = 20;
								pictureBoxViewEditorRubberBand.Height = 20;
								pictureBoxViewEditorRubberBand.Visible = true;
								pictureBoxFontSelectorRubberBand.Visible = false;
							}
						}
						break;

					case MegaCopyStatusFlags.Pasting:
						{
							if (!MouseValidView(e.X, e.Y))
							{
								return;
							}

							// Paste
							if (e.Button == MouseButtons.Left)
							{
								CopyPasteTargetLocation = new Point(rx, ry);
								ExecutePasteFromClipboard(true);
								ResetMegaCopyStatus();
							}
						}
						break;
				}
			}
			else
			{
				// Not in MegaCopy mode.
				// Draw or read character
				ContinueViewDrawInMove = true;

				if (Control.ModifierKeys == Keys.Control)
				{
					ContinueViewDrawInMove = false;
				} // Ctrl+click nezapina toggle

				if (ry >= pictureBoxAtariView.Height / 16)
				{
					return;
				}

				LastViewCharacterX = rx;
				LastViewCharacterY = ry;

				if (e.Button == MouseButtons.Left)
				{
					var theChar = (byte)(SelectedCharacterIndex % 256);
					if (Control.ModifierKeys == Keys.Shift)
					{
						theChar += 128;
					}

					AtariView.ViewBytes[rx, ry] = theChar;
					RedrawViewChar();
				}

				// Pick a character from the view
				// This also picks the correct font
				if (e.Button == MouseButtons.Right)
				{
					var readChar = AtariView.ViewBytes[rx, ry];
					var bx = readChar % 32;
					var by = readChar / 32;

					// If its the 2nd font on the bank then add 8 to the y offset
					if (AtariView.UseFontOnLine[ry] == 2 || AtariView.UseFontOnLine[ry] == 4)
					{
						by = by | 8;
					}
					// Check if the font bank needs to be switched
					if ((checkBoxFontBank.Checked == false && (AtariView.UseFontOnLine[ry] == 3 || AtariView.UseFontOnLine[ry] == 4)) ||
					    (checkBoxFontBank.Checked == true && (AtariView.UseFontOnLine[ry] == 1 || AtariView.UseFontOnLine[ry] == 2)))
					{
						checkBoxFontBank.Checked = !checkBoxFontBank.Checked;
						SwitchFontBank();
					}

					// Select the character in the font
					ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 1, bx * 16 + 4, by * 16 + 4, 0));
				}
			}
		}

		public void ActionAtariViewMouseUp(MouseEventArgs e)
		{
			ContinueViewDrawInMove = false;

			if ((e.X >= pictureBoxAtariView.Width) || (e.Y >= pictureBoxAtariView.Height))
			{
				return;
			}

			var rx = e.X / 16;
			var ry = e.Y / 16;

			if (buttonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case MegaCopyStatusFlags.Selecting:
						{
							if (ry <= CopyPasteRange.Y)
							{
								CopyPasteRange.Height = 0;
							}
							else
							{
								CopyPasteRange.Height = ry - CopyPasteRange.Y;
							}

							if (rx <= CopyPasteRange.X)
							{
								CopyPasteRange.Width = 0;
							}
							else
							{
								CopyPasteRange.Width = rx - CopyPasteRange.X;
							}


							megaCopyStatus = MegaCopyStatusFlags.Selected;

							break;
						}
				}
			}
		}

		public void ActionAtariViewEditorMouseMove(MouseEventArgs e)
		{
			int rx, ry;

			if (buttonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case MegaCopyStatusFlags.Selecting:
						{
							if ((e.X >= pictureBoxAtariView.Width) || (e.Y >= pictureBoxAtariView.Height))
							{
								return;
							}

							rx = e.X / 16;
							ry = e.Y / 16;

							int origWidth = pictureBoxViewEditorRubberBand.Width;
							int origHeight = pictureBoxViewEditorRubberBand.Height;

							int w = 20;
							int h = 20;
							var temp = (rx - CopyPasteRange.X + 1) * 16 + 4;
							if (temp >= 20)
								w = temp;

							temp = (ry - CopyPasteRange.Y + 1) * 16 + 4;
							if (temp >= 20)
								h = temp;

							if (w != origWidth || h != origHeight)
							{
								pictureBoxViewEditorRubberBand.Size = new Size(w, h);
							}
						}
						break;

					case MegaCopyStatusFlags.Pasting:
						{
							if (!MouseValidView(e.X, e.Y))
							{
								pictureBoxViewEditorPasteCursor.Visible = false;
								pictureBoxViewEditorMegaCopyImage.Visible = false;
								return;
							}

							pictureBoxViewEditorPasteCursor.Left = pictureBoxAtariView.Left + e.X - e.X % 16 - 2;
							pictureBoxViewEditorPasteCursor.Top = pictureBoxAtariView.Top + e.Y - e.Y % 16 - 2;
							pictureBoxViewEditorPasteCursor.Visible = true;
							pictureBoxViewEditorMegaCopyImage.Visible = true;
							pictureBoxFontSelectorPasteCursor.Visible = false;
							pictureBoxFontSelectorMegaCopyImage.Visible = false;
							pictureBoxViewEditorMegaCopyImage.Left = pictureBoxViewEditorPasteCursor.Left + 2;
							pictureBoxViewEditorMegaCopyImage.Top = pictureBoxViewEditorPasteCursor.Top + 2;
						}
						break;
				}
			}
			else
			{
				if (ContinueViewDrawInMove && e.X >= 0 && e.X < pictureBoxAtariView.Width &&
                   e.Y >= 0 && e.Y < pictureBoxAtariView.Height &&
                   (e.X / 16 != LastViewCharacterX || e.Y / 16 != LastViewCharacterY))
				{
					ActionAtariViewEditorMouseDown(new MouseEventArgs(MouseButtons.Left, 1, e.X, e.Y, 0));
				}

				pictureBoxViewEditorPasteCursor.Width = 20;
				pictureBoxViewEditorPasteCursor.Height = 20;
				pictureBoxViewEditorPasteCursor.Left = pictureBoxAtariView.Left + e.X - e.X % 16 - 2;
				pictureBoxViewEditorPasteCursor.Top = pictureBoxAtariView.Top + e.Y - e.Y % 16 - 2;
				pictureBoxViewEditorPasteCursor.Visible = true;
			}

			// Char under cursor:
			if ((e.X >= pictureBoxAtariView.Width) || (e.Y >= pictureBoxAtariView.Height) || e.X < 0 || e.Y < 0)
			{
				return;
			}

			rx = e.X / 16;
			ry = e.Y / 16;
			var fontchar = AtariView.ViewBytes[rx, ry];
			labelViewCharInfo.Text = $@"Char: Font {AtariView.UseFontOnLine[ry]} ${fontchar:X2} #{fontchar} @ {rx},{ry}";
		}

		public bool MouseValidView(int x, int y)
		{
			return (x < pictureBoxAtariView.Width - (CopyPasteRange.Width) * 16) && (y < pictureBoxAtariView.Height - (CopyPasteRange.Height) * 16);
		}

		/// <summary>
		/// Redraw whole view area by copying characters from font area
		/// </summary>
		public void RedrawView()
		{
			var colorOffset = InColorMode ? 512 : 0;
			var img = Helpers.GetImage(pictureBoxAtariView);
			using (var gr = Graphics.FromImage(img))
			{
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

				for (var y = 0; y < AtariView.VIEW_HEIGHT; y++)
				{
					for (var x = 0; x < AtariView.VIEW_WIDTH; x++)
					{
						var rx = AtariView.ViewBytes[x, y] % 32;
						var ry = AtariView.ViewBytes[x, y] / 32;

						destRect.X = x * 16;
						destRect.Y = y * 16;

						srcRect.X = rx * 16;
						srcRect.Y = ry * 16 + Constants.FontYOffset[AtariView.UseFontOnLine[y] - 1] + colorOffset;

						gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect, GraphicsUnit.Pixel);
					}
				}
			}
			pictureBoxAtariView.Refresh();
		}

		/// <summary>
		/// Redraw all occurrences of character (SelectedCharacterIndex) on bank X in view area 
		/// </summary>
		public void RedrawViewChar()
		{
			var colorOffset = InColorMode ? 512 : 0;
			var img = Helpers.GetImage(pictureBoxAtariView);
			using (var gr = Graphics.FromImage(img))
			{
				var destRect = new Rectangle
				{
					Width = 16,
					Height = 16,
				};

				var rx = SelectedCharacterIndex % 32;       // Character x,y
				var ry = SelectedCharacterIndex / 32;

				var srcRect = new Rectangle
				{
					X = rx * 16,
					Y = ry * 16,        // Will be set below
					Width = 16,
					Height = 16,
				};

				for (var y = 0; y < AtariView.VIEW_HEIGHT; y++)
				{
					var fontYOffset = Constants.FontPageOffset[AtariView.UseFontOnLine[y] - 1] + colorOffset;

					ry = (ry | 8);

					if (AtariView.UseFontOnLine[y] == 1 || AtariView.UseFontOnLine[y] == 3)
					{
						ry = (ry ^ 8);
					}

					var ny = ry ^ 4; //ny checks invert notes sign (ry)
					var ep = (rx + ry * 32) % 256;
					var dp = (rx + ny * 32) % 256;

					for (var x = 0; x < AtariView.VIEW_WIDTH; x++)
					{
						destRect.X = x * 16;
						destRect.Y = y * 16;

						if (AtariView.ViewBytes[x, y] == (byte)ep)
						{
							srcRect.Y = ry * 16 + fontYOffset;
							//gr.DrawImage(pictureBoxFontSelector.Image, destRect, srcRect, GraphicsUnit.Pixel);
							gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect, GraphicsUnit.Pixel);

						}

						if (AtariView.ViewBytes[x, y] == (byte)dp)
						{
							srcRect.Y = ny * 16 + fontYOffset;
							//gr.DrawImage(pictureBoxFontSelector.Image, destRect, srcRect, GraphicsUnit.Pixel);
							gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect, GraphicsUnit.Pixel);
						}
					}
				}
			}
			pictureBoxAtariView.Refresh();
		}





		public void DefaultView()
		{
			const string dt = "Test"; // '\x34' + "he" + '\x0' + "quick" + '\x0' + "brown" + '\x0' + "fox" + '\x0' + "jumps";
			const string dy = "String"; // "over" + '\x0' + "the" + '\x0' + "lazy" + '\x0' + "dog";

			var bytes = Encoding.Default.GetBytes(dt);
			for (var a = 0; a < dt.Length; a++)
			{
				AtariView.ViewBytes[a + 2, 2] = bytes[a];
			}

			bytes = Encoding.Default.GetBytes(dy);
			for (var a = 0; a < dy.Length; a++)
			{
				AtariView.ViewBytes[a + 6, 3] = bytes[a];
			}

			RedrawLineTypes();
		}

		private static string?[] ToDraw = new string?[] { null, "1", "2", "3", "4" };

		/// <summary>
		/// Draw the font indicator next to the sample screen.
		/// Column of 26x 1-4 font indicators
		/// </summary>
		public void RedrawLineTypes()
		{
			var img = Helpers.GetImage(pictureBoxCharacterSetSelector);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(whiteBrush, 0, 0, img.Width, img.Height);
				for (var a = 0; a < AtariView.VIEW_HEIGHT; a++)
				{
					gr.DrawString(ToDraw[AtariView.UseFontOnLine[a]], this.Font, blackBrush, 4, 2 + a * 16);
				}
			}
			pictureBoxCharacterSetSelector.Refresh();
		}

		public int GetActualViewWidth()
		{
			return checkBox40Bytes.Checked ? 40 : 32;
		}

		public void LoadViewFile(string? filename, bool forceLoadFont = false)
		{
			ClearPageList();

			var filenames = new string[4];

			try
			{
				var jsonText = filename is null ? Helpers.GetResource<string>("default.atrview") : File.ReadAllText(filename, Encoding.UTF8);
				var jsonObj = jsonText.FromJson<AtrViewInfoJSON>();

				int.TryParse(jsonObj.Version, out var version);
				int.TryParse(jsonObj.ColoredGfx, out var coloredGfx);

				if (version >= 1911)
				{
					// Take out the values from the parsed JSON container
					var characterBytes = jsonObj.Chars;
					var lineTypes = jsonObj.Lines;
					var colors = jsonObj.Colors;
					var fontBytes = jsonObj.Data;

					filenames[0] = jsonObj.Fontname1;
					filenames[1] = jsonObj.Fontname2;
					filenames[2] = jsonObj.Fontname3 ?? "Default.fnt";
					filenames[3] = jsonObj.Fontname4 ?? "Default.fnt";

					int viewWidth;
					if (version < 2007)
					{
						viewWidth = 32;
						FortyBytes = "0";
					}
					else
					{
						viewWidth = 40;
						FortyBytes = jsonObj.FortyBytes;
					}

					AtariView.UseFontOnLine = Convert.FromHexString(lineTypes);
					for (var i = 0; i < AtariView.UseFontOnLine.Length; i++)
						++AtariView.UseFontOnLine[i];

					var bytes = Convert.FromHexString(characterBytes);
					var idx = 0;
					for (var y = 0; y < AtariView.VIEW_HEIGHT; ++y)
					{
						for (var x = 0; x < viewWidth; ++x)
						{
							AtariView.ViewBytes[x, y] = bytes[idx];
							++idx;
						}
					}
					// Load the AtariPalette selection
					InColorSetSetup = true;
					SetOfSelectedColors = Convert.FromHexString(colors);
					SetPrimaryColorSetData();
					BuildBrushCache();
					InColorSetSetup = false;

					// If the GFX (mode 0 or 4) does not match then hit the GFX button to switch the mode of the GUI
					if (InColorMode != (coloredGfx == 1))
					{
						SwitchGfxMode();
					}

					if ((forceLoadFont) || (MessageBox.Show(@"Would you like to load fonts embedded in this view file?", @"Load embedded fonts", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes))
					{
						Font1Filename = filenames[0];
						Font2Filename = filenames[1];
						Font3Filename = filenames[2];
						Font4Filename = filenames[3];

						if (fontBytes.Length == 4096)
						{
							fontBytes = fontBytes + fontBytes;
						}
						AtariFont.FontBytes = Convert.FromHexString(fontBytes);

						UpdateFormCaption();
						UndoBuffer.Add2UndoFullDifferenceScan(); //full font scan
						UpdateUndoButtons(false);
					}

					// 40 bytes width
					if ((FortyBytes == "1" && !checkBox40Bytes.Checked)
						|| (FortyBytes == "0" && !checkBox40Bytes.Checked))
					{
						checkBox40Bytes.Checked = !checkBox40Bytes.Checked;
						ViewEditor_CheckBox40Bytes_Click(0, EventArgs.Empty);
					}

					// Load the page information
					if (jsonObj.Pages?.Count > 0)
					{
						Pages = new List<PageData>();
						for (var pageIndex = 0; pageIndex < jsonObj.Pages.Count; ++pageIndex)
						{
							var pageSrc = jsonObj.Pages[pageIndex];


							bytes = Convert.FromHexString(pageSrc.View);
							idx = 0;
							var viewData = new byte[AtariView.VIEW_WIDTH, AtariView.VIEW_HEIGHT];
							for (var y = 0; y < AtariView.VIEW_HEIGHT; ++y)
							{
								for (var x = 0; x < viewWidth; ++x)
								{
									viewData[x, y] = bytes[idx];
									++idx;
								}
							}

							var page = new PageData(pageSrc.Nr, pageSrc.Name, viewData, Convert.FromHexString(pageSrc.SelectedFont), pageIndex);
							Pages.Add(page);
						}

						SwopPageAction(0);
					}
				}

				CheckDuplicate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);

				// Set some defaults!
				SetupDefaultPalColors();
				Font1Filename = "Default.fnt";
				Font2Filename = "Default.fnt";
				Font3Filename = "Default.fnt";
				Font4Filename = "Default.fnt";
			}

			// Make sure the SetOfSelectedColors values have valid brushes for painting!
			BuildBrushCache();
			BuildColorSetList();
			BuildPageList();
		}

		// save view file new edition
		public void SaveViewFile(string filename)
		{
			// Make sure that the current page's data is saved to the page container
			SwopPage(saveCurrent: true);

			var jo = new AtrViewInfoJSON();

			var characterBytes = string.Empty;

			// Version
			jo.Version = "2023";
			// Which GFX mode is selected
			jo.ColoredGfx = InColorMode ? "1" : "0";

			// Characters in the current view
			for (var i = 0; i < AtariView.VIEW_HEIGHT; i++)
			{
				for (var j = 0; j < AtariView.VIEW_WIDTH; j++)
				{
					characterBytes = characterBytes + String.Format("{0:X2}", AtariView.ViewBytes[j, i]);
				}
			}

			jo.Chars = characterBytes;

			// Font selection information
			jo.Lines = Convert.ToHexString(AtariView.UseFontOnLine);

			// Colors
			jo.Colors = Convert.ToHexString(SetOfSelectedColors);
			// Font names
			jo.Fontname1 = Font1Filename;
			jo.Fontname2 = Font2Filename;
			jo.Fontname3 = Font3Filename;
			jo.Fontname4 = Font4Filename;

			// Font data
			jo.Data = Convert.ToHexString(AtariFont.FontBytes);

			// Width selection
			jo.FortyBytes = GetActualViewWidth() == 40 ? "1" : "0";

			// Save the page information
			jo.Pages = new List<SavedPageData>();
			foreach (var srcPage in Pages)
			{
				var page = new SavedPageData()
				{
					Nr = srcPage.Nr,
					Name = srcPage.Name,
					SelectedFont = Convert.ToHexString(srcPage.SelectedFont),
				};
				var pageView = string.Empty;
				for (var i = 0; i < AtariView.VIEW_HEIGHT; i++)
				{
					for (var j = 0; j < AtariView.VIEW_WIDTH; j++)
					{
						pageView = pageView + $"{srcPage.View[j, i]:X2}";
					}
				}

				page.View = pageView;
				jo.Pages.Add(page);
			}

			var txt = jo.ToJson();

			File.WriteAllText(filename, txt, Encoding.UTF8);
		}

		/// <summary>
		/// Cycle through the fonts on each line 1 -> 2 -> 3 -> 4 -> 1
		/// </summary>
		public void ActionCharacterSetSelector(MouseEventArgs e)
		{
			var ry = e.Y / 16;

			if (Control.ModifierKeys == Keys.Control)
			{
				// Setup to 1
				AtariView.UseFontOnLine[ry] = 1;
			}
			else if (Control.ModifierKeys == Keys.Shift || e.Button == MouseButtons.Right)
			{
				// Cycle backwards
				--AtariView.UseFontOnLine[ry];
				if (AtariView.UseFontOnLine[ry] == 0)
					AtariView.UseFontOnLine[ry] = 4;
			}
			else
			{
				// Cycle forward
				++AtariView.UseFontOnLine[ry];
				if (AtariView.UseFontOnLine[ry] == 5)
					AtariView.UseFontOnLine[ry] = 1;
			}

			RedrawLineTypes();
			RedrawView();
		}

		public void ActionEnterText()
		{
			var text = Interaction.InputBox("Enter text", "Enter text to be added to clipboard:", string.Empty);

			switch (text.Length)
			{
				case 0:
					return;
				case > 32:
					text = text[^32..];
					break;
			}

			RenderTextToClipboard(text, Control.ModifierKeys == Keys.Shift);
		}

		/// <summary>
		/// Load data into the view area
		/// </summary>
		public void ActionLoadView()
		{
			byte version = 0;

			var buf = new byte[8192];

			dialogOpenFile.FileName = string.Empty;
			dialogOpenFile.InitialDirectory = CurrentDataFolder;
			dialogOpenFile.Filter = @"Atari FontMaker View (*.atrview,*.vf2,*.vfn)|*.atrview;*.vf2;*.vfn|Raw data (*.dat)|*.dat";
			var ok = dialogOpenFile.ShowDialog();

			if (ok == DialogResult.OK)
			{
				var viewWidth = GetActualViewWidth();
				var ext = Path.GetExtension(dialogOpenFile.FileName).ToLower();

				if (ext == ".atrview")
				{
					LoadViewFile(dialogOpenFile.FileName);
					CurrentDataFolder = Path.GetDirectoryName(dialogOpenFile.FileName) + Path.DirectorySeparatorChar;
					RedrawFonts();
					RedrawLineTypes();
					RedrawView();
					RedrawPal();
					RedrawViewChar();
					RedrawChar();
					return;
				}

				if (ext == ".dat")
				{
					using var fsDat = new FileStream(dialogOpenFile.FileName, FileMode.Open, FileAccess.Read, FileShare.None);
					var loadSize = (int)Math.Min(fsDat.Length, AtariView.VIEW_HEIGHT * viewWidth);

					try
					{
						var read = fsDat.Read(buf, 0, loadSize);
					}
					finally
					{
						fsDat.Close();
					}

					// Copy the bytes into the screen
					for (var a = 0; a < AtariView.VIEW_HEIGHT; a++)
					{
						for (var b = 0; b < viewWidth; b++)
						{
							if (a * viewWidth + b < loadSize)
							{
								AtariView.ViewBytes[b, a] = buf[a * viewWidth + b];
							}
						}
					}

					CurrentDataFolder = Path.GetDirectoryName(dialogOpenFile.FileName) + Path.DirectorySeparatorChar;
					RedrawFonts();
					RedrawLineTypes();
					RedrawView();
					RedrawPal();
					RedrawViewChar();
					RedrawChar();
					return;
				}
				// Handle old binary versions of view file
				// Bytes:
				// version : 1 byte
				// InColorMode : 1 byte
				using var fs = new FileStream(dialogOpenFile.FileName, FileMode.Open, FileAccess.Read, FileShare.None);
				var fsIndex = 0;

				if (ext == ".vf2")
				{
					try
					{

						fs.Read(buf, fsIndex, 1);
						version = buf[0];
						++fsIndex;
					}
					finally
					{
					}

					if (version > 3)
					{
						MessageBox.Show("File was created in newer version of FontMaker (incorrect file???)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}

				byte c = 0;
				try
				{
					fs.Read(buf, fsIndex, 1);
					c = buf[0];
					++fsIndex;
				}
				finally
				{
				}

				if (c == 0)
				{
					InColorMode = true;
				}
				else
				{
					InColorMode = false;
				}
				SwitchGfxMode();

				if (ext == ".vf2")
				{
					for (int a = 0; a <= 7; a++)
					{
						fs.Read(buf, fsIndex, 4);
						AtariView.UseFontOnLine[a] = (byte)BitConverter.ToInt32(buf, 0);
						fsIndex += 4;
					}
				}

				for (int a = 0; a < 6; a++)
				{
					byte rr = 0;
					byte gg = 0;
					byte bb = 0;
					try
					{
						fs.Read(buf, fsIndex, 3);
						fsIndex += 3;
						rr = buf[0];
						gg = buf[1];
						bb = buf[2];
					}
					finally
					{
					}

					SetOfSelectedColors[a] = Helpers.FindClosest(rr, gg, bb, AtariPalette);
					UpdateBrushCache(a);
				}

				if (ext == ".vf2")
				{
					switch (version)
					{
						case 2:
							{
								// 31 x 8 screen = 248 bytes
								try
								{
									fs.Read(buf, fsIndex, 248);
									fsIndex += 248;
								}
								finally
								{
								}

								for (int a = 0; a < 8; a++)
								{
									for (int b = 0; b < 31; b++)
									{
										AtariView.ViewBytes[b, a] = buf[a * 31 + b];
									}
								}

								RedrawLineTypes();
							}
							break;

						case 3:
							{
								// 32x26 screen = 832 bytes
								try
								{
									fs.Read(buf, fsIndex, 32 * 26);
									fsIndex += 32 * 26;
								}
								finally
								{
								}

								for (int a = 0; a < 26; a++)
								{
									for (int b = 0; b < 32; b++)
									{
										AtariView.ViewBytes[b, a] = buf[a * 32 + b];
									}
								}

								RedrawLineTypes();
							}
							break;
					}
				}

				if (ext == ".vfn")
				{
					try
					{
						fs.Read(buf, fsIndex, 186);
						fsIndex += 186;
					}
					finally
					{
					}

					for (int b = 0; b < 31; b++)
					{
						for (int a = 0; a < 6; a++)
						{
							AtariView.ViewBytes[b, a] = buf[a + b * 6];
						}

						for (int a = 6; a < 8; a++)
						{
							AtariView.ViewBytes[b, a] = 0;
						}
					}
				}

				fs.Close();
				RedrawFonts();
				RedrawView();
				RedrawPal();
				RedrawViewChar();
				RedrawLineTypes();
				CurrentDataFolder = Path.GetDirectoryName(dialogOpenFile.FileName) + Path.DirectorySeparatorChar;
			}
		}

		public void ActionSaveView()
		{
			dialogSaveFile.FileName = string.Empty;
			dialogSaveFile.InitialDirectory = CurrentDataFolder;
			dialogSaveFile.DefaultExt = "atrview";
			dialogSaveFile.Filter = @"Atari FontMaker View (*.atrview)|*.atrview|Raw data (*.dat)|*.dat";

			if (dialogSaveFile.ShowDialog() == DialogResult.OK)
			{
				var ext = Path.GetExtension(dialogSaveFile.FileName).ToLower();

				if (ext == ".atrview")
				{
					SaveViewFile(dialogSaveFile.FileName);
				}

				if (ext == ".dat")
				{
					var data = new byte[AtariView.VIEW_HEIGHT * GetActualViewWidth()];
					for (var a = 0; a < AtariView.VIEW_HEIGHT; a++)
						for (var b = 0; b < GetActualViewWidth(); b++)
							data[b + a * GetActualViewWidth()] = AtariView.ViewBytes[b, a];
					File.WriteAllBytes(dialogSaveFile.FileName, data);
				}

				CurrentDataFolder = Path.GetDirectoryName(dialogSaveFile.FileName) + Path.DirectorySeparatorChar;
			}
		}

		/// <summary>
		/// Clear the view area:
		/// - fill with char 0
		/// - reset to font 1
		/// </summary>
		public void ActionClearView()
		{
			var ok = MessageBox.Show(@"Clear view window?", Constants.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Error);

			if (ok == DialogResult.Yes)
			{
				for (var a = 0; a < AtariView.VIEW_HEIGHT; a++)
				{
					AtariView.UseFontOnLine[a] = 1;
				}

				for (var a = 0; a < AtariView.VIEW_WIDTH; a++)
				{
					for (var b = 0; b < AtariView.VIEW_HEIGHT; b++)
					{
						AtariView.ViewBytes[a, b] = 0;
					}
				}

				RedrawView();
				RedrawLineTypes();
			}
		}

		/// <summary>
		/// Add a new page, by duplicating the current page data and the font selection data
		/// </summary>
		public void ActionAddPage()
		{
			// Save the current page data
			var currentPage = Pages[CurrentPageIndex];
			currentPage.Save(AtariView.ViewBytes, AtariView.UseFontOnLine);

			// Create a new page
			var nextNr = Pages.Max(page => page.Nr) + 1;
			var pageOne = new PageData(nextNr, $"Page {nextNr}", AtariView.ViewBytes, AtariView.UseFontOnLine, -1);
			Pages.Add(pageOne);

			InPagesSetup = true;
			comboBoxPages.Items.Clear();
			comboBoxPages.ResetText();

			foreach (var page in Pages)
			{
				var idx = comboBoxPages.Items.Add(page.Name);
				page.Index = idx;
			}

			comboBoxPages.SelectedIndex = comboBoxPages.Items.Count - 1;
			CurrentPageIndex = comboBoxPages.SelectedIndex;

			InPagesSetup = false;
			UpdatePageDisplay();
		}

		public void ActionDeletePage()
		{
			if (Pages.Count <= 1) return;
			var answer = MessageBox.Show("Are you sure you want to delete the page?", "Delete page", MessageBoxButtons.YesNo);
			if (answer == DialogResult.No) return;

			// Delete the page at the current index
			InPagesSetup = true;
			Pages.RemoveAt(CurrentPageIndex);

			// Rebuild the combo list
			comboBoxPages.Items.Clear();
			comboBoxPages.ResetText();

			foreach (var page in Pages)
			{
				var idx = comboBoxPages.Items.Add(page.Name);
				page.Index = idx;
			}

			if (CurrentPageIndex >= comboBoxPages.Items.Count - 1)
			{
				CurrentPageIndex = comboBoxPages.Items.Count - 1;
			}

			comboBoxPages.SelectedIndex = CurrentPageIndex;

			InPagesSetup = false;

			SwopPage(saveCurrent: false);

			UpdatePageDisplay();
		}

		public void ActionEditPages()
		{
			// Save the current page data
			var currentPage = Pages[CurrentPageIndex];
			currentPage.Save(AtariView.ViewBytes, AtariView.UseFontOnLine);

			// Show the editor
			var pe = new PageEditor(Pages, CurrentPageIndex);
			var action = pe.ShowDialog();
			if (action == DialogResult.OK)
			{
				// Save the data back and
				Pages = pe.GetPages();

				// Rebuild the combo list
				InPagesSetup = true;
				comboBoxPages.Items.Clear();
				comboBoxPages.ResetText();

				var toSelect = 0;

				foreach (var page in Pages)
				{
					var idx = comboBoxPages.Items.Add(page.Name);
					page.Index = idx;
					if (currentPage.Nr == page.Nr)
						toSelect = idx;
				}

				comboBoxPages.SelectedIndex = toSelect;
				SwopPage(false);

				InPagesSetup = false;

				UpdatePageDisplay();
			}
		}

		private void ResizeViewEditorPasteCursor()
		{
			var img = Helpers.NewImage(pictureBoxViewEditorPasteCursor);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(yellowBrush, new Rectangle(0, 0, img.Width, img.Height));
				pictureBoxViewEditorPasteCursor.Region?.Dispose();
				pictureBoxViewEditorPasteCursor.Size = new Size(img.Width, img.Height);

			}
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, pictureBoxViewEditorPasteCursor.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(pictureBoxViewEditorPasteCursor.Width - 2, 0, 2, pictureBoxViewEditorPasteCursor.Height));
			graphicsPath.AddRectangle(new Rectangle(0, pictureBoxViewEditorPasteCursor.Height - 2, pictureBoxViewEditorPasteCursor.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, pictureBoxViewEditorPasteCursor.Height));
			pictureBoxViewEditorPasteCursor.Region = new Region(graphicsPath);

			pictureBoxViewEditorPasteCursor.Refresh();
		}
	}
}
