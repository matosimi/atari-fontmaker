using Microsoft.VisualBasic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using TinyJson;
#pragma warning disable WFO1000

namespace FontMaker;

internal class AtariViewEditor
{
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

	internal bool ContinueViewDrawInMove { get; set; }

	internal bool PreventScrollProcessing { get; set; }

	/// <summary>
	/// Detect a right-double-click and reset/cancel the mega copy/paste mode
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
	/// Select a character for editing.
	/// Only called by other windows to make sure that their reference character is in the editor
	/// </summary>
	/// <param name="charNr">0 - 1023</param>
	public void PickCharacter(int charNr)
	{
		var bx = charNr % 32;
		var by = charNr / 32;

		var want2ndBank = by >= 16;
		// Check if the font bank needs to be switched
		if ((checkBoxFontBank.Checked == false && want2ndBank) ||
		    (checkBoxFontBank.Checked == true && !want2ndBank))
		{
			checkBoxFontBank.Checked = !checkBoxFontBank.Checked;
			SwitchFontBank();
		}

		if (by > 16)
			by -= 16;

		var oldMegaCopyStatus = megaCopyStatus;
		megaCopyStatus = MegaCopyStatusFlags.None;
		var mouseEvent = new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0);
		ActionFontSelectorMouseDown(mouseEvent);
		ActionFontSelectorMouseUp(mouseEvent);
		megaCopyStatus = oldMegaCopyStatus;
	}

	/// <summary>
	/// Select a specific page to be shown
	/// </summary>
	/// <param name="pageIndex"></param>
	public void PickPage(int pageIndex)
	{
		comboBoxPages.SelectedIndex = pageIndex;
	}

	public void ActionAtariViewEditorMouseDown(MouseEventArgs e)
	{
		if (e.X >= AtariView.VIEW_PIXEL_WIDTH || e.Y >= AtariView.VIEW_PIXEL_HEIGHT || e.X < 0 || e.Y < 0)
		{
			return;
		}

		// Calc the character offset for the click position
		var rx = e.X / 16;
		var ry = e.Y / CellHeight;

		var viewWidth = Math.Min(GetActualViewWidth(), AtariView.Width);

		if (rx < 0 || rx >= viewWidth || ry < 0 || ry >= AtariView.VIEW_HEIGHT)
		{
			// Note: This should not happen but when running on Mac things get funky
			return;
		}

		if (buttonMegaCopy.Checked)
		{
			// We are the MegaCopy mode
			switch (megaCopyStatus)
			{
				case MegaCopyStatusFlags.None:
				case MegaCopyStatusFlags.Selected:
				{
					// If nothing is selected or already have a selection then define a new start point when left-clicked
					if (e.Button == MouseButtons.Left)
					{
						// Define copy origin point
						CopyPasteRange.X = AtariView.OffsetX + rx;
						CopyPasteRange.Y = AtariView.OffsetY + ry;
						megaCopyStatus = MegaCopyStatusFlags.Selecting;

						pictureBoxViewEditorRubberBand.SetBounds(pictureBoxAtariView.Left + e.X - e.X % 16 - 2, pictureBoxAtariView.Top + e.Y - e.Y % CellHeight - 2, 20, 20);
						pictureBoxViewEditorRubberBand.Visible = true;
						pictureBoxFontSelectorRubberBand.Visible = false;
					}
					break;
				}

				case MegaCopyStatusFlags.Pasting:
				case MegaCopyStatusFlags.PastingView:
				{
					if (!IsMouseValidForPastingIntoAtariView(e.X, e.Y))
					{
						return;
					}

					// Paste
					if (e.Button == MouseButtons.Left)
					{
						// Maintain undo buffer
						CopyPasteTargetLocation = new Point(rx, ry);
						ExecutePasteFromClipboard(true);

						if (Control.ModifierKeys == Keys.Alt || checkBoxStayInPasteMode.Checked)
							break;
						ResetMegaCopyStatus();
					}
					break;
				}
			}
		}
		else
		{
			// In draw/read mode
			// Draw or read character
			ContinueViewDrawInMove = true;

			if (Control.ModifierKeys == Keys.Control)
			{
				ContinueViewDrawInMove = false;
			} // Ctrl+click toggle

			if (ry >= AtariView.VIEW_PIXEL_HEIGHT / CellHeight)
			{
				return;
			}

			LastViewCharacterX = rx;
			LastViewCharacterY = ry;

			if (e.Button == MouseButtons.Left)
			{
				// Maintain undo buffer
				if (e.Delta != -1000)
					PushState();

				var theChar = (byte)(SelectedCharacterIndex % 256);
				if (Control.ModifierKeys == Keys.Shift)
				{
					theChar += 128;
				}

				AtariView.ViewBytes[AtariView.OffsetX + rx, AtariView.OffsetY + ry] = theChar;
				RedrawViewChar();
				return;
			}

			// Pick a character from the view
			// This also picks the correct font
			if (e.Button == MouseButtons.Right)
			{
				var readChar = AtariView.ViewBytes[AtariView.OffsetX + rx, AtariView.OffsetY + ry];
				var bx = readChar % 32;
				var by = readChar / 32;

				// If it is the 2nd font on the bank then add 8 to the y offset
				if (AtariView.UseFontOnLine[AtariView.OffsetY + ry] == 2 || AtariView.UseFontOnLine[AtariView.OffsetY + ry] == 4)
				{
					by = by | 8;
				}

				// Check if the font bank needs to be switched
				if ((checkBoxFontBank.Checked == false && (AtariView.UseFontOnLine[AtariView.OffsetY + ry] == 3 || AtariView.UseFontOnLine[AtariView.OffsetY + ry] == 4)) ||
				    (checkBoxFontBank.Checked == true && (AtariView.UseFontOnLine[AtariView.OffsetY + ry] == 1 || AtariView.UseFontOnLine[AtariView.OffsetY + ry] == 2)))
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

		if (buttonMegaCopy.Checked)
		{
			// In MegaCopy mode
			if ((e.X >= AtariView.VIEW_PIXEL_WIDTH) || (e.Y >= AtariView.VIEW_PIXEL_HEIGHT))
			{
				return;
			}

			var rx = e.X / 16;
			var ry = e.Y / CellHeight;

			var viewWidth = Math.Min(GetActualViewWidth(), AtariView.Width);
			if (rx < 0 || rx >= viewWidth || ry < 0 || ry >= AtariView.VIEW_HEIGHT)
			{
				// Note: This should not happen but when running on Mac things get funky
				return;
			}

			// Adjust for screen offset
			rx += AtariView.OffsetX;
			ry += AtariView.OffsetY;

			if (megaCopyStatus == MegaCopyStatusFlags.Selecting)
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

				UpdateViewActions();

			}
		}
	}

	public void ActionAtariViewEditorMouseMove(MouseEventArgs e)
	{
		int rx;
		int ry;
		if (buttonMegaCopy.Checked)
		{
			// In MegaCopy mode
			switch (megaCopyStatus)
			{
				case MegaCopyStatusFlags.Selecting:
				{
					if ((e.X >= AtariView.VIEW_PIXEL_WIDTH) || (e.Y >= AtariView.VIEW_PIXEL_HEIGHT))
					{
						return;
					}

					rx = AtariView.OffsetX + e.X / 16;
					ry = AtariView.OffsetY + e.Y / CellHeight;

					if (rx < 0 || rx >= AtariView.Width || ry < 0 || ry >= AtariView.Height)
					{
						// Note: This should not happen but when running on Mac things get funky
						return;
					}

					var origWidth = pictureBoxViewEditorRubberBand.Width;
					var origHeight = pictureBoxViewEditorRubberBand.Height;

					var w = 20;
					var h = CursorHeight;
					var temp = (rx - CopyPasteRange.X + 1) * 16 + 4;
					if (temp >= 20)
						w = temp;

					temp = (ry - CopyPasteRange.Y + 1) * CellHeight + 4;
					if (temp >= CursorHeight)
						h = temp;

					if (w != origWidth || h != origHeight)
					{
						pictureBoxViewEditorRubberBand.Size = new Size(w, h);
					}
					break;
				}

				case MegaCopyStatusFlags.Pasting:
				case MegaCopyStatusFlags.PastingView:
				{
					if (!IsMouseValidForPastingIntoAtariView(e.X, e.Y))
					{
						pictureBoxViewEditorPasteCursor.Visible = false;
						pictureBoxViewEditorMegaCopyImage.Visible = false;
						return;
					}

					if (!PastingToView)
					{
						PastingToView = true;
						RevalidateClipboard();
					}

					pictureBoxViewEditorPasteCursor.Location = new Point(pictureBoxAtariView.Left + e.X - e.X % 16 - 2, pictureBoxAtariView.Top + e.Y - e.Y % CellHeight - 2);
					pictureBoxViewEditorMegaCopyImage.Location = new Point(pictureBoxViewEditorPasteCursor.Left + 2, pictureBoxViewEditorPasteCursor.Top + 2);
					
					pictureBoxViewEditorPasteCursor.Visible = true;
					pictureBoxViewEditorMegaCopyImage.Visible = true;
					pictureBoxFontSelectorPasteCursor.Visible = false;
					pictureBoxFontSelectorMegaCopyImage.Visible = false;
					break;
				}
			}
		}
		else
		{
			// In draw/read mode
			if (ContinueViewDrawInMove 
			    && e.X is >= 0 and < AtariView.VIEW_PIXEL_WIDTH
			    && e.Y is >= 0 and < AtariView.VIEW_PIXEL_HEIGHT
			    && (e.X / 16 != LastViewCharacterX || e.Y / CellHeight != LastViewCharacterY))
			{
				ActionAtariViewEditorMouseDown(new MouseEventArgs(e.Button, 1, e.X, e.Y, -1000));      // -1000 indicates that this is a simulated event
			}

			pictureBoxViewEditorPasteCursor.Bounds = new Rectangle(pictureBoxAtariView.Left + e.X - e.X % 16 - 2, pictureBoxAtariView.Top + e.Y - e.Y % CellHeight - 2, 20, CursorHeight);
			ResizeViewEditorPasteCursor();

			pictureBoxViewEditorPasteCursor.Visible = true;
		}

		// Char under cursor:
		if ((e.X >= AtariView.VIEW_PIXEL_WIDTH) || (e.Y >= AtariView.VIEW_PIXEL_HEIGHT) || e.X < 0 || e.Y < 0)
		{
			return;
		}

		rx = AtariView.OffsetX + e.X / 16;
		ry = AtariView.OffsetY + e.Y / CellHeight;
			
		if (rx < 0 || rx >= AtariView.Width || ry < 0 || ry >= AtariView.Height)
		{
			// Note: This should not happen but when running on Mac things get funky
			return;
		}
		var fontchar = AtariView.ViewBytes[rx, ry];
		labelViewCharInfo.Text = $@"Char: Font {AtariView.UseFontOnLine[ry]} ${fontchar:X2} #{fontchar} @ {rx},{ry}";
	}

	public bool IsMouseValidForPastingIntoAtariView(int x, int y)
	{
		// When running on the Mac it breaks the actual pictureBoxAtariView.Width and Height
		// So the real bounds are hard coded here
		return (x < AtariView.VIEW_PIXEL_WIDTH - (CopyPasteRange.Width) * 16) && (y < AtariView.VIEW_PIXEL_HEIGHT - (CopyPasteRange.Height) * CellHeight);
	}

	/// <summary>
	/// Redraw whole view area by copying characters from font area
	/// </summary>
	public void RedrawView()
	{
		var colorOffset = InColorMode ? 512 : 0;
		var img = Helpers.GetImage(pictureBoxAtariView);
		using (var gr = Graphics.FromImage(img))
		using (var wrapMode = new ImageAttributes())
		{
			wrapMode.SetWrapMode(WrapMode.TileFlipXY);
			gr.InterpolationMode = InterpolationMode.NearestNeighbor;
			gr.Clear(AtariPalette[SetOfSelectedColors[1]]);

			var destRect = new Rectangle
			{
				Width = 16,
				Height = CellHeight,
			};

			var srcRect = new Rectangle
			{
				Width = 16,
				Height = 16,
			};

			var viewWidth = Math.Min(GetActualViewWidth(), AtariView.Width);
			for (var y = 0; y < ViewHeight; y++)
			{
				for (var x = 0; x < viewWidth; x++)
				{
					var charFromView = AtariView.ViewBytes[AtariView.OffsetX + x, AtariView.OffsetY + y];
					var rx = charFromView % 32;
					var ry = charFromView / 32;

					destRect.X = x * 16;
					destRect.Y = y * CellHeight;

					srcRect.X = rx * 16;
					srcRect.Y = ry * 16 + Constants.FontYOffset[AtariView.UseFontOnLine[AtariView.OffsetY + y] - 1] + colorOffset;

					gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, wrapMode);
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
		using (ImageAttributes wrapMode = new ImageAttributes())
		{
			wrapMode.SetWrapMode(WrapMode.TileFlipXY);
			gr.InterpolationMode = InterpolationMode.NearestNeighbor;
			var destRect = new Rectangle
			{
				Width = 16,
				Height = CellHeight,
			};

			var rx = SelectedCharacterIndex % 32; // Character x,y
			var ry = SelectedCharacterIndex / 32;

			var srcRect = new Rectangle
			{
				X = rx * 16,
				Y = ry * 16, // Will be set below
				Width = 16,
				Height = 16,
			};

			var viewWidth = Math.Min(GetActualViewWidth(), AtariView.Width);

			for (var y = 0; y < ViewHeight; y++)
			{
				var fontYOffset = Constants.FontPageOffset[AtariView.UseFontOnLine[AtariView.OffsetY + y] - 1] + colorOffset;

				ry = (ry | 8);

				if (AtariView.UseFontOnLine[AtariView.OffsetY + y] == 1 || AtariView.UseFontOnLine[AtariView.OffsetY + y] == 3)
				{
					ry = (ry ^ 8);
				}

				var ny = ry ^ 4; //ny checks invert notes sign (ry)
				var ep = (rx + ry * 32) % 256;
				var dp = (rx + ny * 32) % 256;


				for (var x = 0; x < viewWidth; x++) // Check
				{
					destRect.X = x * 16;
					destRect.Y = y * CellHeight;

					if (AtariView.ViewBytes[AtariView.OffsetX + x, AtariView.OffsetY + y] == (byte)ep)
					{
						srcRect.Y = ry * 16 + fontYOffset;
						gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, wrapMode);

					}

					if (AtariView.ViewBytes[AtariView.OffsetX + x, AtariView.OffsetY + y] == (byte)dp)
					{
						srcRect.Y = ny * 16 + fontYOffset;
						gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, wrapMode);

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

	private static readonly string?[] ToDraw = [null, "1", "2", "3", "4"];

	/// <summary>
	/// Draw the font indicator next to the sample screen.
	/// Column of 26x 1-4 font indicators
	/// </summary>
	public void RedrawLineTypes()
	{
		using var pen = new Pen(BlackBrush);
		var img = Helpers.GetImage(pictureBoxCharacterSetSelector);
		using (var gr = Graphics.FromImage(img))
		{
			gr.FillRectangle(WhiteBrush, 0, 0, img.Width, img.Height);
			for (var a = 0; a < ViewHeight; a++)
			{
				gr.DrawString(ToDraw[AtariView.UseFontOnLine[AtariView.OffsetY + a]], this.Font, BlackBrush, 4, 2 + a * CellHeight);
			}
			gr.DrawLine(pen, 2, 24 * CellHeight, 15, 24 * CellHeight);
		}

		pictureBoxCharacterSetSelector.Refresh();
	}

	public int GetActualViewWidth()
	{
		return comboBoxBytes.SelectedIndex switch
		{
			0 => 32,
			1 => 40,
			2 => 48,
			_ => 40
		};
	}

	/// <summary>
	/// Load the default or specified .atrview file
	/// </summary>
	/// <param name="filename"></param>
	/// <param name="forceLoadFont"></param>
	public void LoadViewFile(string? filename, bool forceLoadFont = false)
	{
		var hasLoadError = false;
		List<string> loadMessages = [];

		ClearPageList();

		var filenames = new string[4];

		try
		{
			var jsonText = string.IsNullOrWhiteSpace(filename) ? Helpers.GetResource<string>("default.atrview") : File.ReadAllText(filename, Encoding.UTF8);
			var jsonObj = jsonText.FromJson<AtrViewInfoJson>();

			_ = int.TryParse(jsonObj.Version, out var version);
			_ = int.TryParse(jsonObj.ColoredGfx, out var coloredGfx);

			if (version >= 1911)
			{
				// Take out the values from the parsed JSON container
				var colors = jsonObj.Colors;
				var fontBytes = jsonObj.Data;

				filenames[0] = jsonObj.Fontname1;
				filenames[1] = jsonObj.Fontname2;
				filenames[2] = jsonObj.Fontname3 ?? "Default.fnt";
				filenames[3] = jsonObj.Fontname4 ?? "Default.fnt";

				// Handle the older format where we have no width/height information for the view
				if (jsonObj.Width == 0)
				{
					jsonObj.Width = 40;
				}
				if (jsonObj.Height == 0)
				{
					jsonObj.Height = 26;
				}

				int viewWidth;
				if (version < 2007)
				{
					viewWidth = 32;
					FortyBytes = "0";
				}
				else
				{
					viewWidth = jsonObj.Width;
					FortyBytes = jsonObj.FortyBytes;
				}

				AtariView.ForcedResize(jsonObj.Width, jsonObj.Height);
				AtariView.Load(jsonObj.Lines, jsonObj.Chars, viewWidth);

				// Load the AtariPalette selection
				InColorSetSetup = true;
				SetOfSelectedColors = Convert.FromHexString(FixColorHexString(colors));
				SetPrimaryColorSetData();
				BuildBrushCache();
				InColorSetSetup = false;

				if (forceLoadFont || (MessageBox.Show(@"Would you like to load fonts embedded in this view file?", @"Load embedded fonts", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
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
					AtariFontUndoBuffer.Add2UndoFullDifferenceScan(); // Full font scan
					UpdateUndoButtons(false);
				}

				comboBoxBytes.SelectedIndex = FortyBytes switch
				{
					// 40 bytes width
					"0" => 0,
					"1" => 1,
					"2" => 2,
					_ => 1
				};

				// Load the page information
				if (jsonObj.Pages?.Count > 0)
				{
					Pages = [];
					for (var pageIndex = 0; pageIndex < jsonObj.Pages.Count; ++pageIndex)
					{
						Pages.Add(new PageData(jsonObj.Pages[pageIndex], pageIndex));
					}

					SwopPageAction(0);
				}

				TileSet.Setup();
				if (jsonObj.Tiles?.Count > 0)
				{
					foreach (var tileData in jsonObj.Tiles)
					{
						if (!TileSet.Load(tileData))
						{
							loadMessages.Add("Failed to load a tile.");
						}
					}
				}

				// If the GFX mode does not match then hit the GFX button until we are at the correct mode
				// InColorMode = bool: true then in color mode
				// WhichColorMode = int: 4/5/10
				SetupColorMode(coloredGfx);
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
		RedrawViewAndInfo();
	}

	// Save view file new edition
	public void SaveViewFile(string filename)
	{
		// Make sure that the current page's data is saved to the page container
		SwopPage(saveCurrent: true);

		var jo = new AtrViewInfoJson();

		var characterBytes = string.Empty;

		// Version
		jo.Version = "2023";
		// Which GFX mode is selected
		// B/W => 0
		// Mode 4 => 1
		// Mode 5 => 2
		// Mode 10 = 3
		jo.ColoredGfx = WhatColorModeToSave().ToString();

		jo.Width = AtariView.Width;
		jo.Height = AtariView.Height;

		// Characters in the current view
		for (var i = 0; i < AtariView.Height; i++)
		{
			for (var j = 0; j < AtariView.Width; j++)
			{
				characterBytes = characterBytes + $"{AtariView.ViewBytes[j, i]:X2}";
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
		jo.FortyBytes = GetActualViewWidth() switch
		{
			32 => "0",
			40 => "1",
			48 => "2",
			_ => "1"
		};

		// Save the page information
		jo.Pages = [];
		foreach (var srcPage in Pages)
		{
			jo.Pages.Add(new SavedPageData(srcPage));
		}

		// Save the TileSet information
		jo.Tiles = [];
		for (var index = 0; index < TileSet.Tiles.Length; index++)
		{
			var data = TileSet.Save(index);
			if (data != null)
			{
				jo.Tiles.Add(data);
			}
		}

		// Finished creating the object. Serialize it!
		var txt = jo.ToJson();
		File.WriteAllText(filename, txt, Encoding.UTF8);
	}

	/// <summary>
	/// Cycle through the fonts on each line 1 -> 2 -> 3 -> 4 -> 1
	/// </summary>
	public void ActionCharacterSetSelector(MouseEventArgs e)
	{
		var ry = e.Y / CellHeight;

		if (ry < 0 || ry >= AtariView.VIEW_HEIGHT)
		{
			// Note: This should not happen but when running on Mac things get funky
			return;
		}

		if (Control.ModifierKeys == Keys.Control)
		{
			// Setup to 1
			AtariView.UseFontOnLine[AtariView.OffsetY + ry] = 1;
		}
		else if (Control.ModifierKeys == Keys.Shift || e.Button == MouseButtons.Right)
		{
			// Cycle backwards
			--AtariView.UseFontOnLine[AtariView.OffsetY + ry];
			if (AtariView.UseFontOnLine[AtariView.OffsetY + ry] == 0)
				AtariView.UseFontOnLine[AtariView.OffsetY + ry] = 4;
		}
		else
		{
			// Cycle forward
			++AtariView.UseFontOnLine[AtariView.OffsetY + ry];
			if (AtariView.UseFontOnLine[AtariView.OffsetY + ry] == 5)
				AtariView.UseFontOnLine[AtariView.OffsetY + ry] = 1;
		}

		RedrawLineTypes();
		RedrawView();
	}

	public void ActionEnterText()
	{
		var text = Interaction.InputBox("Enter text of up to 32 characters\nIf you want inverted text then hold down SHIFT when you click [Ok]\nIf you want to use the 2nd font hold down CONTROL when you click [Ok]", "Enter text to be added to clipboard:", string.Empty);

		switch (text.Length)
		{
			case 0:
				return;
			case > 32:
				text = text[^32..];
				break;
		}

		RenderTextToClipboard(text, (Control.ModifierKeys & Keys.Shift) == Keys.Shift, (Control.ModifierKeys & Keys.Control) == Keys.Control);
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
				var loadSize = (int)Math.Min(fsDat.Length, AtariView.Height * AtariView.Width);

				try
				{
					buf = new byte[loadSize];
					fsDat.ReadExactly(buf, 0, loadSize);
				}
				finally
				{
					fsDat.Close();
				}

				// Copy the bytes into the screen
				for (var a = 0; a < AtariView.Height; a++)
				{
					for (var b = 0; b < AtariView.Width; b++)
					{
						if (a * AtariView.Width + b < loadSize)
						{
							AtariView.ViewBytes[b, a] = buf[a * AtariView.Width + b];
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

					fs.ReadExactly(buf, fsIndex, 1);
					version = buf[0];
					++fsIndex;
				}
				catch
				{
					// ignored
				}

				if (version > 3)
				{
					MessageBox.Show(@"File was created in newer version of FontMaker (incorrect file???)", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}

			byte c = 0;
			try
			{
				fs.ReadExactly(buf, fsIndex, 1);
				c = buf[0];
				++fsIndex;
			}
			catch
			{
				// ignored
			}


			SetupColorMode(c);
			//InColorMode = c == 0;
			//SwitchGfxMode();

			if (ext == ".vf2")
			{
				for (var a = 0; a <= 7; a++)
				{
					fs.ReadExactly(buf, fsIndex, 4);
					AtariView.UseFontOnLine[a] = (byte)BitConverter.ToInt32(buf, 0);
					fsIndex += 4;
				}
			}

			for (var a = 0; a < 6; a++)
			{
				byte rr = 0;
				byte gg = 0;
				byte bb = 0;
				try
				{
					fs.ReadExactly(buf, fsIndex, 3);
					fsIndex += 3;
					rr = buf[0];
					gg = buf[1];
					bb = buf[2];
				}
				catch
				{
					// ignored
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
							fs.ReadExactly(buf, fsIndex, 248);
							fsIndex += 248;
						}
						catch
						{
							// ignored
						}

						for (var a = 0; a < 8; a++)
						{
							for (var b = 0; b < 31; b++)
							{
								AtariView.ViewBytes[b, a] = buf[a * 31 + b];
							}
						}

						RedrawLineTypes();
						break;
					}

					case 3:
					{
						// 32x26 screen = 832 bytes
						try
						{
							fs.ReadExactly(buf, fsIndex, 32 * 26);
							fsIndex += 32 * 26;
						}
						catch
						{
							// ignored
						}

						for (var a = 0; a < 26; a++)
						{
							for (var b = 0; b < 32; b++)
							{
								AtariView.ViewBytes[b, a] = buf[a * 32 + b];
							}
						}

						RedrawLineTypes();
						break;
					}
				}
			}

			if (ext == ".vfn")
			{
				try
				{
					fs.ReadExactly(buf, fsIndex, 186);
				}
				catch
				{
					// ignored
				}

				for (var b = 0; b < 31; b++)
				{
					for (var a = 0; a < 6; a++)
					{
						AtariView.ViewBytes[b, a] = buf[a + b * 6];
					}

					for (var a = 6; a < 8; a++)
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
				var viewWidth = Math.Min(AtariView.Height, AtariView.Width);

				var data = new byte[AtariView.Height * AtariView.Width];
				for (var a = 0; a < AtariView.Height; a++)
				{
					for (var b = 0; b < AtariView.Width; b++)
					{
						data[b + a * AtariView.Width] = AtariView.ViewBytes[b, a];
					}
				}

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
		var ok = MessageBox.Show(@"Clear view window?", Constants.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

		if (ok == DialogResult.Yes)
		{
			PushState();

			for (var a = 0; a < AtariView.Height; a++)
			{
				AtariView.UseFontOnLine[a] = 1;
			}

			for (var a = 0; a < AtariView.Width; a++)
			{
				for (var b = 0; b < AtariView.Height; b++)
				{
					AtariView.ViewBytes[a, b] = 0;
				}
			}

			UpdateHVScrollBars();

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
		currentPage.Save();

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

		CurrentPage = Pages[CurrentPageIndex];
		UpdatePageDisplay();
		TransferPagesToViewActions();
	}

	public void ActionDeletePage()
	{
		if (Pages.Count <= 1) return;
		var answer = MessageBox.Show(@"Are you sure you want to delete the page?", @"Delete page", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
		TransferPagesToViewActions();
	}

	public void ActionEditPages()
	{
		// Save the current page data
		var currentPage = Pages[CurrentPageIndex];
		currentPage.Save();

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
			TransferPagesToViewActions();
		}
	}

	public void ActionConfigurePage()
	{
		var configWindow = new AtariViewConfigWindow();
		var action = configWindow.ShowDialog();
		if (action == DialogResult.OK)
		{
			PushState();
			AtariView.Resize(configWindow.NewWidth, configWindow.NewHeight, configWindow.NewFontNr);
			UpdateHVScrollBars(AtariView.OffsetX, AtariView.OffsetY);

			RedrawViewAndInfo();
		}
	}

	public void ActionAtariViewOffsetChanged()
	{
		AtariView.OffsetX = hScrollBar.Value;
		AtariView.OffsetY = vScrollBar.Value;

		RedrawViewAndInfo();
		UpdateSelectionPosition();
	}

	/// <summary>
	/// Switch to the next/previous page
	/// </summary>
	/// <param name="direction"></param>
	private void ActionNextPage(int direction = 1)
	{
		var nextPageId = comboBoxPages.SelectedIndex + direction;
		if (nextPageId == comboBoxPages.Items.Count)
		{
			nextPageId = 0;
		}

		if (nextPageId < 0)
			nextPageId = comboBoxPages.Items.Count - 1;

		PickPage(nextPageId);
	}

	private void ResizeViewEditorPasteCursor()
	{
		var img = Helpers.NewImage(pictureBoxViewEditorPasteCursor);
		using (var gr = Graphics.FromImage(img))
		{
			gr.FillRectangle(YellowBrush, new Rectangle(0, 0, img.Width, img.Height));
			pictureBoxViewEditorPasteCursor.Region?.Dispose();
			pictureBoxViewEditorPasteCursor.Size = new Size(img.Width, img.Height);
		}

		using var graphicsPath = new GraphicsPath();
		graphicsPath.AddRectangle(new Rectangle(0, 0, pictureBoxViewEditorPasteCursor.Width, 2));
		graphicsPath.AddRectangle(new Rectangle(pictureBoxViewEditorPasteCursor.Width - 2, 0, 2, pictureBoxViewEditorPasteCursor.Height));
		graphicsPath.AddRectangle(new Rectangle(0, pictureBoxViewEditorPasteCursor.Height - 2, pictureBoxViewEditorPasteCursor.Width, 2));
		graphicsPath.AddRectangle(new Rectangle(0, 0, 2, pictureBoxViewEditorPasteCursor.Height));
		pictureBoxViewEditorPasteCursor.Region = new Region(graphicsPath);

		pictureBoxViewEditorPasteCursor.Refresh();
	}

	private void TransferPagesToViewActions()
	{
		ViewActionsWindowForm?.RebuildPagesDropDown(Pages, comboBoxPages.SelectedIndex);
	}

	private void TransferPageSelectionToViewActions()
	{
		ViewActionsWindowForm?.SelectPage(comboBoxPages.SelectedIndex);
	}

	private void UpdateViewActions()
	{
		labelSelectedArea.Text = megaCopyStatus == MegaCopyStatusFlags.Selected 
			? $"Area: X:{CopyPasteRange.X} Y:{CopyPasteRange.Y} W:{CopyPasteRange.Width+1} H:{CopyPasteRange.Height+1}" 
			: string.Empty;

		ViewActionsWindowForm?.UpdateViewInformation(
			buttonMegaCopy.Checked
			, megaCopyStatus == MegaCopyStatusFlags.Selected && pictureBoxViewEditorRubberBand.Visible
			, CopyPasteRange
		);
	}

	/// <summary>
	/// Shift a region/whole screen left/right/up/down
	/// </summary>
	/// <param name="onPageNr">Which page </param>
	/// <param name="direction"></param>
	/// <param name="area"></param>
	public void ActionAreaShift(int onPageNr, DirectionFlags direction, Rectangle area)
	{
		switch (direction)
		{
			case DirectionFlags.Up:
			case DirectionFlags.Down:
				if (area.Height == 0) return;
				break;
			case DirectionFlags.Left:
			case DirectionFlags.Right:
				if (area.Width == 0) return;
				break;
		}

		SwopToPage(onPageNr);
		// AtariView.ViewBytes now has the selected page's data
		// Adjust the data in the selected area
		var from = AtariView.ViewBytes.Clone() as byte[,];
		if (from is null) return;

		PushState();

		switch (direction)
		{
			case DirectionFlags.Up:
			{
				for (var x = area.Left; x <= area.Right; ++x)
				{
					AtariView.ViewBytes[x, area.Bottom] = from[x, area.Top];
				}

				for (var y = area.Top + 1; y <= area.Bottom; ++y)
				{
					for (var x = area.Left; x <= area.Right; ++x)
					{
						AtariView.ViewBytes[x, y - 1] = from[x, y];
					}
				}

				break;
			}
			case DirectionFlags.Down:
			{
				for (var x = area.Left; x <= area.Right; ++x)
				{
					AtariView.ViewBytes[x, area.Top] = from[x, area.Bottom];
				}

				for (var y = area.Top; y < area.Bottom; ++y)
				{
					for (var x = area.Left; x <= area.Right; ++x)
					{
						AtariView.ViewBytes[x, y + 1] = from[x, y];
					}
				}

				break;
			}
			case DirectionFlags.Left:
			{
				for (var y = area.Top; y <= area.Bottom; ++y)
				{
					AtariView.ViewBytes[area.Right, y] = from[area.Left, y];
				}

				for (var x = area.Left + 1; x <= area.Right; ++x)
				{
					for (var y = area.Top; y <= area.Bottom; ++y)
					{
						AtariView.ViewBytes[x - 1, y] = from[x, y];
					}
				}

				break;
			}
			case DirectionFlags.Right:
			{
				for (var y = area.Top; y <= area.Bottom; ++y)
				{
					AtariView.ViewBytes[area.Left, y] = from[area.Right, y];
				}

				for (var x = area.Left; x < area.Right; ++x)
				{
					for (var y = area.Top; y <= area.Bottom; ++y)
					{
						AtariView.ViewBytes[x + 1, y] = from[x, y];
					}
				}

				break;
			}
		}

		UpdatePageDisplay();
	}

	public byte RenderCharIntoPictureBox(PictureBox target)
	{
		var onBank2 = checkBoxFontBank.Checked;
		var theChar = (byte)(SelectedCharacterIndex % 256);
		if (Control.ModifierKeys == Keys.Shift)
		{
			theChar += 128;
		}

		var colorOffset = InColorMode ? 512 : 0;
		var img = Helpers.GetImage(target);
		using (var gr = Graphics.FromImage(img))
		{
			var destRect = new Rectangle
			{
				X = 0,
				Y = 0,
				Width = 16,
				Height = 16,
			};

			var rx = SelectedCharacterIndex % 32; // Character x,y
			var ry = SelectedCharacterIndex / 32;

			var srcRect = new Rectangle
			{
				X = rx * 16,
				Y = ry * 16, // Will be set below
				Width = 16,
				Height = 16,
			};

			var whichFontNr = (onBank2 ? 2 : 0) + (SelectedCharacterIndex >= 256 ? 1 : 0);
			var fontYOffset = Constants.FontPageOffset[whichFontNr] + colorOffset;

			ry = ry | 8;

			if (whichFontNr is 0 or 2)
			{
				ry = (ry ^ 8);
			}

			var ny = ry ^ 4; //ny checks invert notes sign (ry)
			var ep = (rx + ry * 32) % 256;
			var dp = (rx + ny * 32) % 256;

			if (theChar == (byte)ep)
			{
				srcRect.Y = ry * 16 + fontYOffset;
				gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect, GraphicsUnit.Pixel);
			}

			if (theChar == (byte)dp)
			{
				srcRect.Y = ny * 16 + fontYOffset;
				gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect, GraphicsUnit.Pixel);
			}
		}

		target.Refresh();

		return theChar;
	}

	public void ReplaceCharXWithY(byte charX, byte charY, bool inFont1, bool inFont2, bool inFont3, bool inFont4, Rectangle area)
	{
		PushState();

		var activeFontNr = new List<int>();
		if (inFont1) activeFontNr.Add(1);
		if (inFont2) activeFontNr.Add(2);
		if (inFont3) activeFontNr.Add(3);
		if (inFont4) activeFontNr.Add(4);

		for (var y = area.Top; y <= area.Bottom; y++)
		{
			if (activeFontNr.Contains(AtariView.UseFontOnLine[y]))
			{
				for (var x = area.Left; x <= area.Right; x++)
				{
					if (AtariView.ViewBytes[x, y] == charX)
						AtariView.ViewBytes[x, y] = charY;
				}
			}
		}

		RedrawView();
	}

	public void FillArea(Rectangle area, byte fillerChar = 0)
	{
		PushState();

		for (var y = area.Top; y <= area.Bottom; y++)
		{
			for (var x = area.Left; x <= area.Right; x++)
			{
				AtariView.ViewBytes[x, y] = fillerChar;
			}
		}

		RedrawView();
	}

	public void PasteClipboardIntoView(string? clipBuffer, string? nulls, int width, int height)
	{
		if (clipBuffer == null)
			return;
		nulls ??= new string('0', width * height);
		PushState();

		var charsBytes = Convert.FromHexString(clipBuffer);
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var i = AtariView.OffsetY + y + CopyPasteTargetLocation.Y;
				var j = AtariView.OffsetX + x + CopyPasteTargetLocation.X;
				if (nulls[y*width + x] == '1')
					continue;
				if (checkBoxSkipChar0.Checked && charsBytes[y * width + x] == trackBarSkipCharX.Value)
					continue;
				AtariView.ViewBytes[j, i] = charsBytes[y * width + x];
			}
		}

		RedrawView();
	}

	public void ExecuteViewUndo()
	{
		CurrentPage?.UndoBuffer?.Undo();

		UpdateHVScrollBars(AtariView.OffsetX, AtariView.OffsetY);
		RedrawViewAndInfo();
		UpdateViewUndoButtons();
	}
	public void ExecuteViewRedo()
	{
		CurrentPage?.UndoBuffer?.Redo();

		UpdateHVScrollBars(AtariView.OffsetX, AtariView.OffsetY);
		RedrawViewAndInfo();
		UpdateViewUndoButtons();
	}

	public void PushState()
	{
		CurrentPage?.UndoBuffer.Push();
		UpdateViewUndoButtons();
	}
	public void UpdateViewUndoButtons()
	{
		if (CurrentPage is { UndoBuffer: not null })
		{
			var (undoEnabled, redoEnabled) = CurrentPage.UndoBuffer.GetRedoUndoButtonState();
			buttonViewUndo.Enabled = undoEnabled;
			buttonViewRedo.Enabled = redoEnabled;
		}
	}

	public void RedrawViewAndInfo()
	{
		RedrawView();
		RedrawLineTypes();

		UpdatePageSize();
	}

	public void UpdatePageSize()
	{
		labelPageSize.Text = $"{AtariView.Width} x {AtariView.Height}";
		labelOffsets.Text = $"TL: [{AtariView.OffsetX},{AtariView.OffsetY}]-[{AtariView.OffsetX+39},{AtariView.OffsetY+25}]";

		UpdateViewActions();
	}

	public void UpdateSelectionPosition()
	{
		if (megaCopyStatus == MegaCopyStatusFlags.Selected)
		{
			var rect = new Rectangle(CopyPasteRange.X, CopyPasteRange.Y, CopyPasteRange.Width + 1, CopyPasteRange.Height + 1);
			rect.Offset(-AtariView.OffsetX, -AtariView.OffsetY);

			var viewWidth = Math.Min(GetActualViewWidth(), AtariView.Width);
			var targetRect = new Rectangle(0,0, viewWidth, AtariView.VIEW_HEIGHT);
			rect.Intersect(targetRect);

			if (rect.IsEmpty)
			{
				// Selection box if out of bounds.
				// So hide it
				pictureBoxViewEditorRubberBand.Visible = false;
				return;
			}
			// Move the selection cursor

			pictureBoxViewEditorRubberBand.Location = new Point(pictureBoxAtariView.Left - 2 + rect.X * 16, pictureBoxAtariView.Top - 2 + rect.Y * CellHeight);
			pictureBoxViewEditorRubberBand.Size = new Size(rect.Width * 16 + 4, rect.Height * CellHeight + 4);
			pictureBoxViewEditorRubberBand.Visible = true;
		}
	}

	public void UpdateHVScrollBars(int offsetX = 0, int offsetY = 0)
	{
		PreventScrollProcessing = true;
		var maxHorizontal = Math.Max(0, AtariView.Width - GetActualViewWidth());
		hScrollBar.Maximum = maxHorizontal;
		if (offsetX > maxHorizontal)
			offsetX = maxHorizontal;
		hScrollBar.Value = offsetX;

		vScrollBar.Maximum = AtariView.Height - (CellHeight == 16 ? 26 : 13);
		if (offsetY > vScrollBar.Maximum)
			offsetY = vScrollBar.Maximum;
		vScrollBar.Value = offsetY;
		PreventScrollProcessing = false;
		UpdatePageSize();
		ActionAtariViewOffsetChanged();
	}
}