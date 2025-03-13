using System.Drawing.Drawing2D;

namespace FontMaker
{
	internal class FontSelector
	{
	}

	// All functions that interact with the four fonts can be found here
	public partial class FontMakerForm
	{
		public const int FONT_SELECTOR_WIDTH = 512;
		public const int FONT_SELECTOR_HEIGHT = 256;

		/// <summary>
		/// Redraws whole font area: font banks and the pictureBoxFontSelector view into the bank (either page 0 or page 1)
		/// Draw the 4 fonts into the bitmapFontBanks bitmap
		/// </summary>
		public void RedrawFonts()
		{
			AtariFontRenderer.RenderAllFonts();
			ShowCorrectFontBank();
		}

		public void ShowCorrectFontBank()
		{
			var src = Constants.WhereAreTheFontBanksComingFrom[(checkBoxFontBank.Checked ? 1 : 0) + (InColorMode ? 2 : 0)];

			var img = Helpers.GetImage(pictureBoxFontSelector);
			using (var grD = Graphics.FromImage(img))
			{
				// Copy font bank 1 or 2
				grD.DrawImage(AtariFontRenderer.BitmapFontBanks, 0, 0, src, GraphicsUnit.Pixel);
			}
			pictureBoxFontSelector.Refresh();
		}

		/// <summary>
		/// Repaint actual character into the font area
		/// 1st into bmpFontBank and then copy into pictureBoxFontSelector
		/// </summary>
		public void DoChar()
		{
			UpdateUndoButtons(CharacterEdited());

			AtariFontRenderer.RenderOneCharacter(SelectedCharacterIndex, checkBoxFontBank.Checked);

			// Where do the font pixels come from
			var src = Constants.WhereAreTheFontBanksComingFrom[(checkBoxFontBank.Checked ? 1 : 0) + (InColorMode ? 2 : 0)];

			var img = Helpers.GetImage(pictureBoxFontSelector);
			using (var grD = Graphics.FromImage(img))
			{
				// Copy font bank 1 or 2
				grD.DrawImage(AtariFontRenderer.BitmapFontBanks, 0, 0, src, GraphicsUnit.Pixel);
			}

			pictureBoxFontSelector.Refresh();
		}

		public void SetCharCursor()
		{
			if (CharacterEdited())
			{
				UndoBuffer.Add2Undo(true);
			}

			SelectedCharacterIndex = SelectedCharacterIndex % 512;
			var bx = SelectedCharacterIndex % 32;
			var by = SelectedCharacterIndex / 32;
			ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0));
		}

		// Compare two chars within 1 font
		public bool CompareChars(int c1, int c2, int fontNr, int fontBankOffset)
		{
			var p1 = (c1 % 128) * 8 + 1024 * fontNr;
			var p2 = (c2 % 128) * 8 + 1024 * fontNr;
			var diff = false;
			var i = 7;

			do
			{
				if (AtariFont.FontBytes[p1 + i + fontBankOffset] != AtariFont.FontBytes[p2 + i + fontBankOffset])
				{
					diff = true;
				}

				i--;
			}
			while (!((i < 0) || diff));

			return !diff;
		}

		// Finds nearest duplicate char to DuplicateCharacterIndex
		public int FindDuplicateChar()
		{
			var myChar = SelectedCharacterIndex % 256;
			var fontNr = SelectedCharacterIndex / 256;		// 0/1 index into the font bank

			var fontBankOffset = checkBoxFontBank.Checked ? 2048 : 0;

			var inverse = myChar / 128;

			var nextCharIdx = DuplicateCharacterIndex + 1;
			nextCharIdx = nextCharIdx % 128 + 128 * inverse;
			var found = false;

			while (!found && (nextCharIdx + fontNr * 256 != DuplicateCharacterIndex))
			{
				if (CompareChars(nextCharIdx, SelectedCharacterIndex, fontNr, fontBankOffset) && (nextCharIdx + fontNr * 256 != SelectedCharacterIndex))
				{
					found = true;
					break;
				}

				nextCharIdx++;
				nextCharIdx = nextCharIdx % 128 + 128 * inverse;
			}

			var res = nextCharIdx + fontNr * 256;

			if (!found)
			{
				res = SelectedCharacterIndex;
			}

			return res;
		}

		public bool IsMousePositionValidForPasting(int x, int y)
		{
			if ((x >= FONT_SELECTOR_WIDTH - (CopyPasteRange.Width) * 16) || (y >= FONT_SELECTOR_HEIGHT - (CopyPasteRange.Height) * 16))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public void ActionFontSelectorMouseDown(MouseEventArgs e)
		{
			int fontChar;
			int fontNr;

			if (e.X < 0 || e.X >= FONT_SELECTOR_WIDTH || e.Y < 0 || e.Y >= FONT_SELECTOR_HEIGHT)
			{
				return;
			}

			if (!buttonMegaCopy.Checked)
			{
				if (CharacterEdited())
				{
					UndoBuffer.Add2Undo(true);
				}
			}

			var rx = e.X / 16;
			var ry = e.Y / 16;
			SelectedCharacterIndex = rx + ry * 32;

			if (SelectedCharacterIndex > 255)
			{
				fontChar = SelectedCharacterIndex % 256;
				fontNr = 2;
			}
			else
			{
				fontChar = SelectedCharacterIndex;
				fontNr = 1;
			}

			fontNr += checkBoxFontBank.Checked ? 2 : 0;

			comboBoxPasteIntoFontNr.SelectedIndex = fontNr - 1;

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

								// Setup the selection rubber band
								pictureBoxFontSelectorRubberBand.Left = pictureBoxFontSelector.Left + e.X - e.X % 16 - 2;
								pictureBoxFontSelectorRubberBand.Top = pictureBoxFontSelector.Top + e.Y - e.Y % 16 - 2;
								pictureBoxFontSelectorRubberBand.Width = 20;
								pictureBoxFontSelectorRubberBand.Height = 20;
								pictureBoxFontSelectorRubberBand.Visible = true;

								pictureBoxViewEditorRubberBand.Visible = false;
							}
						}
						break;

					case MegaCopyStatusFlags.Pasting:
						{
							if (!IsMousePositionValidForPasting(e.X, e.Y))
							{
								return;
							}

							if (e.Button == MouseButtons.Left)
							{
								CopyPasteTargetLocation = new Point(rx, ry);
								UndoBuffer.Add2UndoFullDifferenceScan();
								UpdateUndoButtons(false);
								ExecutePasteFromClipboard(false);
								ResetMegaCopyStatus();
							}
						}
						break;
				}
			}
			else
			{
				pictureBoxFontSelectorRubberBand.Left = pictureBoxFontSelector.Bounds.Left + e.X - e.X % 16 - 2;
				pictureBoxFontSelectorRubberBand.Top = pictureBoxFontSelector.Bounds.Top + e.Y - e.Y % 16 - 2;

				CopyPasteRange.Y = ry;
				CopyPasteRange.X = rx;
				CopyPasteRange.Width = 0;
				CopyPasteRange.Height = 0;

				labelEditCharInfo.Text = $"Font {fontNr}\n${fontChar:X2} #{fontChar}";
				RedrawChar();
				CheckDuplicate();
			}
		}

		public void ActionFontSelectorMouseUp(MouseEventArgs e)
		{
			if (e.X < 0 || e.X >= FONT_SELECTOR_WIDTH || e.Y < 0 || e.Y >= FONT_SELECTOR_HEIGHT)
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
						}
						break;
				}
			}
		}

		public void ActionFontSelectorMouseMove(MouseEventArgs e)
		{
			if (buttonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case MegaCopyStatusFlags.Selecting:
						{
							if (e.X < 0 || e.X >= FONT_SELECTOR_WIDTH || e.Y < 0 || e.Y >= FONT_SELECTOR_HEIGHT)
							{
								return;
							}

							var rx = e.X / 16;
							var ry = e.Y / 16;

							int origWidth = pictureBoxFontSelectorRubberBand.Width;
							int origHeight = pictureBoxFontSelectorRubberBand.Height;

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
								pictureBoxFontSelectorRubberBand.Size = new Size(w, h);
							}
						}
						break;

					case MegaCopyStatusFlags.Pasting:
						{
							if (!IsMousePositionValidForPasting(e.X, e.Y))
							{
								pictureBoxFontSelectorPasteCursor.Visible = false;
								pictureBoxFontSelectorMegaCopyImage.Visible = false;
								return;
							}
                            
							if (PastingToView)
                            {
                                PastingToView = false;
                                RevalidateClipboard();
                            }

                            pictureBoxFontSelectorPasteCursor.Left = pictureBoxFontSelector.Left + e.X - e.X % 16 - 2;
							pictureBoxFontSelectorPasteCursor.Top = pictureBoxFontSelector.Top + e.Y - e.Y % 16 - 2;
							pictureBoxFontSelectorMegaCopyImage.Left = pictureBoxFontSelectorPasteCursor.Left + 2;
							pictureBoxFontSelectorMegaCopyImage.Top = pictureBoxFontSelectorPasteCursor.Top + 2;
							pictureBoxFontSelectorPasteCursor.Visible = true;
							pictureBoxFontSelectorMegaCopyImage.Visible = true;
							pictureBoxViewEditorPasteCursor.Visible = false;
							pictureBoxViewEditorMegaCopyImage.Visible = false;
						}
						break;
				}
			}
		}

		private void ResizeFontSelectorPasteCursor()
		{
			var img = Helpers.NewImage(pictureBoxFontSelectorPasteCursor);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(GreenBrush, new Rectangle(0, 0, img.Width, img.Height));
				pictureBoxFontSelectorPasteCursor.Region?.Dispose();
				pictureBoxFontSelectorPasteCursor.Size = new Size(img.Width, img.Height);

			}
			using var graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, pictureBoxFontSelectorPasteCursor.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(pictureBoxFontSelectorPasteCursor.Width - 2, 0, 2, pictureBoxFontSelectorPasteCursor.Height));
			graphicsPath.AddRectangle(new Rectangle(0, pictureBoxFontSelectorPasteCursor.Height - 2, pictureBoxFontSelectorPasteCursor.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, pictureBoxFontSelectorPasteCursor.Height));
			pictureBoxFontSelectorPasteCursor.Region = new Region(graphicsPath);

			pictureBoxFontSelectorPasteCursor.Refresh();
		}
	}
}
