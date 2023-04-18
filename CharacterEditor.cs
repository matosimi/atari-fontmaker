using System.Media;
using TinyJson;

namespace FontMaker
{
	internal class CharacterEditor
	{
	}

	public class ClipboardJSON
	{
		public string Width { get; set; }
		public string Height { get; set; }
		public string Chars { get; set; }
		public string Data { get; set; }
	}

	public partial class FontMakerForm
	{
		/// <summary>
		/// Internal copy of the JSON used for copy + paste
		/// </summary>
		private string _localCopyOfClipboardData = string.Empty;

		/// <summary>
		/// Character pixel being edited: X-coordinate
		/// </summary>
		internal int LastCharacterPixelX { get; set; }
		/// <summary>
		/// Character pixel being edited: Y-coordinate
		/// </summary>
		internal int LastCharacterPixelY { get; set; }

		/// <summary>
		/// Remember which mouse button was pressed. Used to simulate mouse-clicks during mouse movement.
		/// </summary>
		internal MouseButtons TrackLastMouseButton { get; set; }

		/// <summary>
		/// Flag set when the mouse goes down.
		/// Cleared on [Control] key down.
		/// When set then a mouse-down click is simulated during mouse movement in the editor.
		/// </summary>
		internal bool ContinueCharacterDrawInMove { get; set; }

		// Character editor interactions

		#region Character Editor mouse interactions

		public void ActionCharacterEditorMouseDown(MouseEventArgs e)
		{
			if (e.X < 0 || e.Y < 0 || e.X >= pictureBoxCharacterEditor.Width || e.Y >= pictureBoxCharacterEditor.Height)
				return;

			var img = Helpers.GetImage(pictureBoxCharacterEditor);
			using (var gr = Graphics.FromImage(img))
			{
				ContinueCharacterDrawInMove = true;
				TrackLastMouseButton = e.Button;

				if (Control.ModifierKeys == Keys.Control)
				{
					ContinueCharacterDrawInMove = false;
				} //ctrl+click no button toggle

				var hp = AtariFont.GetCharacterOffset(SelectedCharacterIndex, checkBoxFontBank.Checked);
				var ry = e.Y / 20;
				LastCharacterPixelY = ry;

				if (!InColorMode)
				{
					var charline2col = AtariFont.DecodeMono(AtariFont.FontBytes[hp + ry]);
					var rx = e.X / 20;
					LastCharacterPixelX = rx;

					if (e.Button == MouseButtons.Left)
					{
						if (comboBoxWriteMode.SelectedIndex == 0)
						{
							if (charline2col[rx] == 0)
							{
								charline2col[rx] = 1;
							}
							else
							{
								charline2col[rx] = 0;
							}
						}
						else
						{
							charline2col[rx] = 1;
						}
					}
					else if (e.Button == MouseButtons.Right)
					{
						charline2col[rx] = 0;
					} //delete

					AtariFont.FontBytes[hp + ry] = AtariFont.EncodeMono(charline2col);
					DoChar();

					var brush = BrushCache[charline2col[rx] == 1 ? 0 : 1];
					gr.FillRectangle(brush, rx * 20, ry * 20, 20, 20);
					gr.FillRectangle(BrushCache[0], rx * 20, ry * 20, 1, 1);
				}
				else
				{
					var charline5col = AtariFont.DecodeColor(AtariFont.FontBytes[hp + ry]);

					for (var a = 0; a < 4; a++)
					{
						charline5col[a] = Constants.Bits2ColorIndex[charline5col[a]];
					}

					var rx = e.X / 40;
					LastCharacterPixelX = rx;

					if (e.Button == MouseButtons.Left)
					{
						if (comboBoxWriteMode.SelectedIndex == 0)
						{
							if (charline5col[rx] != ActiveColorNr)
							{
								charline5col[rx] = (byte)ActiveColorNr;
							}
							else
							{
								charline5col[rx] = 1;
							}
						}
						else
						{
							charline5col[rx] = (byte)ActiveColorNr;
						}
					}
					else if (e.Button == MouseButtons.Right)
					{
						charline5col[rx] = 1;
					} //delete

					// Draw pixel
					gr.FillRectangle(BrushCache[charline5col[rx]], rx * 40, ry * 20, 40, 20);

					// Recode to byte and save to charset
					for (var a = 0; a < 4; a++)
					{
						charline5col[a] = Constants.ColorIndex2Bits[charline5col[a]];
					}

					AtariFont.FontBytes[hp + ry] = AtariFont.EncodeColor(charline5col);
					DoChar();
				}

				RedrawViewChar();
				UpdateUndoButtons(CharacterEdited());
				CheckDuplicate();
			}
			pictureBoxCharacterEditor.Refresh();
		}

		public void ActionCharacterEditorMouseMove(MouseEventArgs e)
		{
			if (ContinueCharacterDrawInMove)
			{
				var je = false;
				int nx;
				var ny = e.Y / 20;

				if (InColorMode)
				{
					nx = e.X / 40;
				}
				else
				{
					nx = e.X / 20;
				}

				if (e.X < 0 || e.X > pictureBoxCharacterEditor.Width || e.Y < 0 || e.Y > pictureBoxCharacterEditor.Height)
				{
					je = true;
				}

				if (!je && (nx != LastCharacterPixelX || ny != LastCharacterPixelY))
				{
					ActionCharacterEditorMouseDown(new MouseEventArgs(TrackLastMouseButton, 1, e.X, e.Y, 0));
				}
			}
		}

		public void ActionCharacterEditorMouseUp()
		{
			ContinueCharacterDrawInMove = false;
		}

		#endregion

		#region Draw font character

		// redraws character that is being edited/selected in character edit window
		public void RedrawChar()
		{
			var img = Helpers.GetImage(pictureBoxCharacterEditor);
			using (var gr = Graphics.FromImage(img))
			{
				if (!InColorMode)
				{
					//gr.0
					var character2Color = AtariFont.Get2ColorCharacter(SelectedCharacterIndex, checkBoxFontBank.Checked);

					for (var a = 0; a < 8; a++)
					{
						for (var b = 0; b < 8; b++)
						{
							var brush = new SolidBrush(character2Color[b, a] == 0 ? AtariPalette[SetOfSelectedColors[1]] : AtariPalette[SetOfSelectedColors[0]]);
							gr.FillRectangle(brush, b * 20, a * 20, 20, 20);
							gr.FillRectangle(whiteBrush, b * 20, a * 20, 1, 1);
						}
					}
				}
				else
				{
					var character5color = AtariFont.Get5ColorCharacter(SelectedCharacterIndex, checkBoxFontBank.Checked);

					for (var a = 0; a < 8; a++)
					{
						for (var b = 0; b < 4; b++)
						{
							var brush = BrushCache[Constants.Bits2ColorIndex[character5color[b, a]]];
							gr.FillRectangle(brush, b * 40, a * 20, 40, 20);
						}
					}
				}
			}
			pictureBoxCharacterEditor.Refresh();
		}

		public void RedrawGrid()
		{
			var img = Helpers.GetImage(pictureBoxCharacterEditor);
			using (var gr = Graphics.FromImage(img))
			{
				var brush = BrushCache[1];
				gr.FillRectangle(brush, 0, 0, pictureBoxCharacterEditor.Width, pictureBoxCharacterEditor.Height);

				for (var b = 0; b < 8; b++)
				{
					for (var a = 0; a < 8; a++)
					{
						gr.FillRectangle(whiteBrush, a * 20, b * 20, 1, 1);
					}
				}
			}
			pictureBoxCharacterEditor.Refresh();
		}

		#endregion

		#region Character editor button actions
		// All the character editor buttons have their click handlers defined here

		private void ActionCharacterEditorColor1MouseDown()
		{
			var bu = (int)pictureBoxCharacterEditorColor1.Tag;
			pictureBoxCharacterEditorColor1.Tag = ActiveColorNr;
			ActiveColorNr = bu;
			RedrawPal();
		}

		private void ActionCharacterEditorRestoreDefault()
		{
			try
			{
				var whichCharacter = SelectedCharacterIndex & 127;

				// Load the font bytes
				byte[] data = Helpers.GetResource<byte[]>("Default.fnt");

				var fontNumber = SelectedCharacterIndex > 255 ? 1 : 0;
				fontNumber += checkBoxFontBank.Checked ? 2 : 0;

				AtariFont.CopyTo(data, whichCharacter * 8, whichCharacter * 8 + fontNumber * 1024, 8);

				DoChar();
				RedrawViewChar();
				ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 1, (SelectedCharacterIndex % 32) * 16, (SelectedCharacterIndex / 32) * 16, 0));
				CheckDuplicate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void ActionCharacterEditorRestoreSaved()
		{
			try
			{
				var whichCharacter = SelectedCharacterIndex & 127;
				var fontNumber = SelectedCharacterIndex / 256;       // Font 0 or 1
				fontNumber += checkBoxFontBank.Checked ? 2 : 0;

				var filename = Font1Filename;
				if (fontNumber == 1) filename = Font2Filename;
				else if (fontNumber == 2) filename = Font3Filename;
				else if (fontNumber == 3) filename = Font4Filename;

				// Load the font bytes
				var data = File.ReadAllBytes(filename);
				AtariFont.CopyTo(data, whichCharacter * 8, whichCharacter * 8 + fontNumber * 1024, 8);

				DoChar();
				RedrawViewChar();
				ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 1, (SelectedCharacterIndex % 32) * 16, (SelectedCharacterIndex / 32) * 16, 0));
				CheckDuplicate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void ActionCharacterEditorColor2MouseDown()
		{
			var bu = (int)pictureBoxCharacterEditorColor2.Tag;
			pictureBoxCharacterEditorColor2.Tag = ActiveColorNr;
			ActiveColorNr = bu;
			RedrawPal();
		}

		private void ExecuteRotateLeft()
		{
			if (!InColorMode)
			{
				AtariFont.RotateLeft(SelectedCharacterIndex, checkBoxFontBank.Checked);
				UpdateCharacterViews();
			}
		}

		private void ExecuteRotateRight()
		{
			if (!InColorMode)
			{
				AtariFont.RotateRight(SelectedCharacterIndex, checkBoxFontBank.Checked);
				UpdateCharacterViews();
			}
		}

		private void ExecuteMirrorHorizontal()
		{
			if (InColorMode)
			{
				AtariFont.MirrorHorizontalColor(SelectedCharacterIndex, checkBoxFontBank.Checked);
			}
			else
			{
				AtariFont.MirrorHorizontalMono(SelectedCharacterIndex, checkBoxFontBank.Checked);
			}
			UpdateCharacterViews();
		}

		private void ExecuteMirrorVertical()
		{
			AtariFont.MirrorVertical(SelectedCharacterIndex, checkBoxFontBank.Checked);
			UpdateCharacterViews();
		}

		private void ExecuteShiftLeft()
		{
			AtariFont.ShiftLeft(SelectedCharacterIndex, checkBoxFontBank.Checked, InColorMode);
			UpdateCharacterViews();
		}

		public void ExecuteShiftRight()
		{
			AtariFont.ShiftRight(SelectedCharacterIndex, checkBoxFontBank.Checked, InColorMode);
			UpdateCharacterViews();
		}

		private void ExecuteShiftUp()
		{
			AtariFont.ShiftUp(SelectedCharacterIndex, checkBoxFontBank.Checked);
			UpdateCharacterViews();
		}

		private void ExecuteShiftDown()
		{
			AtariFont.ShiftDown(SelectedCharacterIndex, checkBoxFontBank.Checked);
			UpdateCharacterViews();
		}

		private void ExecuteInvertCharacter()
		{
			if (!InColorMode)
			{
				AtariFont.InvertCharacter(SelectedCharacterIndex, checkBoxFontBank.Checked);

				UpdateCharacterViews();
			}
			else
			{
				SystemSounds.Beep.Play();
			}
		}

		private void ExecuteClearCharacter()
		{
			AtariFont.ClearCharacter(SelectedCharacterIndex, checkBoxFontBank.Checked);
			UpdateCharacterViews();
		}

		/// <summary>
		/// A character has changed, so update all the views
		/// </summary>
		public void UpdateCharacterViews()
		{
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		#endregion // Character editor actions


		/// <summary>
		/// Some buttons are not available in mode 0, others not in mode 4
		/// </summary>
		public void RevalidateCharButtons()
		{
			if (!InColorMode)
			{
				buttonRecolor.Enabled = false;
				buttonShowColorSwitchSetup.Enabled = false;

				foreach (var item in ActionListNormalModeOnly)
				{
					var tag = (int)(item.Tag ?? 0);
					if (tag == 2)
						item.Enabled = true;
				}
			}
			else
			{
				buttonRecolor.Enabled = true;
				buttonShowColorSwitchSetup.Enabled = true;


				foreach (var item in ActionListNormalModeOnly)
				{
					var tag = (int)(item.Tag ?? 0);
					if (tag == 2)
						item.Enabled = false;
				}
			}
		}

		public bool ExecuteUndo()
		{
			if (CharacterEdited())
			{
				UndoBuffer.Add2Undo(true);  // Add 2 undo but don't change index
			}

			UndoBuffer.Undo();              // Copy the old fonts back

			UpdateUndoButtons(CharacterEdited());
			RedrawChar();
			RedrawFonts();
			RedrawView();
			return true;
		}

		public bool Redo()
		{
			UndoBuffer.Redo();

			UpdateUndoButtons(CharacterEdited());
			RedrawChar();
			RedrawFonts();
			RedrawView();
			return true;
		}

		// updates undo/redo button state based on info if character has been edited and whats the buffer index
		public void UpdateUndoButtons(bool edited)
		{
			var (redoEnabled, undoEnabled) = UndoBuffer.GetRedoUndoButtonState(edited);
			buttonRedo.Enabled = redoEnabled;
			buttonUndo.Enabled = undoEnabled;
		}

		public void ExecuteCopyToClipboard(bool sourceIsView)
		{
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;

			if ((buttonMegaCopy.Checked && (megaCopyStatus == MegaCopyStatusFlags.Selected)) || (!buttonMegaCopy.Checked))
			{
				if (pictureBoxFontSelectorRubberBand.Visible)
				{
					sourceIsView = false;
				}

				if (pictureBoxViewEditorRubberBand.Visible)
				{
					sourceIsView = true;
				}

				if (!pictureBoxFontSelectorRubberBand.Visible && !pictureBoxViewEditorRubberBand.Visible)
				{
					return;
				}

				for (var i = CopyPasteRange.Y; i <= CopyPasteRange.Bottom; i++)
				{
					for (var j = CopyPasteRange.X; j <= CopyPasteRange.Right; j++)
					{
						int charInFont;
						if (sourceIsView)
						{
							characterBytes = characterBytes + String.Format("{0:X2}", AtariView.ViewBytes[j, i]);
							charInFont = (AtariView.ViewBytes[j, i] % 128) * 8 + (AtariView.UseFontOnLine[i] - 1) * 1024;
						}
						else
						{
							characterBytes = characterBytes + String.Format("{0:X2}", (i * 32 + j) % 256);

							if (i / 8 == 0)
							{
								charInFont = ((i % 4) * 32 + j) * 8;
							}
							else
							{
								charInFont = ((i % 4 + 4) * 32 + j) * 8;
							} //second font
						}

						var fontInBankOffset = checkBoxFontBank.Checked ? 2048 : 0;

						for (var k = 0; k < 8; k++)
						{
							fontBytes = fontBytes + String.Format("{0:X2}", AtariFont.FontBytes[charInFont + k + fontInBankOffset]);
						}
					}
				}

				var jo = new ClipboardJSON()
				{
					Width = (CopyPasteRange.Width + 1).ToString(),
					Height = (CopyPasteRange.Height + 1).ToString(),
					Chars = characterBytes,
					Data = fontBytes,
				};
				var json = jo.ToJson();
				Clipboard.SetText(json);
				_localCopyOfClipboardData = json;
			}
		}

		public void Clipboard_copyText(string text, bool inverse)
		{
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;

			var fontInBankOffset = checkBoxFontBank.Checked ? 2048 : 0;

			CopyPasteRange.X = 0;
			CopyPasteRange.Y = 0;
			CopyPasteRange.Width = text.Length;
			CopyPasteRange.Height = 1;

			var chars = text.ToCharArray();
			for (var j = 0; j < text.Length; j++)
			{
				var character = Helpers.AtariConvertChar((byte)chars[j]);

				if (inverse)
				{
					character = (byte)(character | 128);
				}

				characterBytes = characterBytes + $"{character:X2}";
				var charInFont = character * 8;

				for (var k = 0; k < 8; k++)
				{
					fontBytes = fontBytes + $"{AtariFont.FontBytes[charInFont + k + fontInBankOffset]:X2}";
				}
			}

			var jo = new ClipboardJSON()
			{
				Width = text.Length.ToString(),
				Height = "1",
				Chars = characterBytes,
				Data = fontBytes,
			};
			var json = jo.ToJson();
			Clipboard.SetText(json);
			_localCopyOfClipboardData = json;
		}

		public void ExecutePasteFromClipboard()
		{
			if (buttonMegaCopy.Checked)
			{
				if (Clipboard.GetText() != _localCopyOfClipboardData)
				{
					pictureBoxFontSelectorRubberBand.Visible = false;
					pictureBoxViewEditorRubberBand.Visible = false;
				}

				RevalidateClipboard();
				megaCopyStatus = MegaCopyStatusFlags.Pasting;
			}
			else
			{
				ExecutePasteFromClipboard(false);
			}
		}

		public void ExecutePasteFromClipboard(bool targetIsView)
		{
			int width;
			int height;
			string characterBytes;
			string fontBytes;

			var fontInBankOffset = checkBoxFontBank.Checked ? 2048 : 0;

			try
			{
				var jsonText = Clipboard.GetText();
				var jsonObj = jsonText.FromJson<ClipboardJSON>();
				int.TryParse(jsonObj.Width, out width);
				int.TryParse(jsonObj.Height, out height);

				characterBytes = jsonObj.Chars;
				fontBytes = jsonObj.Data;
			}
			catch (Exception)
			{
				MessageBox.Show(@"Clipboard data parsing error");
				return;
			}

			if (buttonMegaCopy.Checked)
			{
				if (targetIsView)
				{
					var charsBytes = Convert.FromHexString(characterBytes);
					for (var y = 0; y < height; y++)
					{
						for (var x = 0; x < width; x++)
						{
							var i = y + CopyPasteTargetLocation.Y;
							var j = x + CopyPasteTargetLocation.X;
							AtariView.ViewBytes[j, i] = charsBytes[y * width + x];
						}
					}

					RedrawView();
				}
				else
				{
					var charsBytes = Convert.FromHexString(fontBytes);
					for (var ii = 0; ii < height; ii++)
					{
						for (var jj = 0; jj < width; jj++)
						{
							var i = ii + CopyPasteTargetLocation.Y;
							var j = jj + CopyPasteTargetLocation.X;
							SelectedCharacterIndex = i * 32 + j;

							int charInFont;
							if (i / 8 == 0)
							{
								charInFont = ((i % 4) * 32 + j) * 8;
							}
							else
							{
								charInFont = ((i % 4 + 4) * 32 + j) * 8;
							} //second font

							for (var k = 0; k < 8; k++)
							{
								AtariFont.FontBytes[charInFont + k + fontInBankOffset] = charsBytes[(ii * width + jj) * 8 + k];
							}

							//SetCharCursor;
							DoChar();
							RedrawChar();
							RedrawViewChar();
						}
					}

					UndoBuffer.Add2UndoFullDifferenceScan();
					UpdateUndoButtons(false);
				}
			}
			else
			{
				if (width + height > 2)
				{
					MessageBox.Show($@"Unable to paste clipboard outside MegaCopy mode. Clipboard contains {width}x{height} data.");
					return;
				}

				var hp = AtariFont.GetCharacterOffset(SelectedCharacterIndex, checkBoxFontBank.Checked);

				var bytes = Convert.FromHexString(fontBytes);
				Buffer.BlockCopy(bytes, 0, AtariFont.FontBytes, hp, 8);

				SetCharCursor();
				DoChar();
				RedrawChar();
				RedrawViewChar();
			}

			CheckDuplicate();
		}



		public void ResetMegaCopyStatus()
		{
			switch (megaCopyStatus)
			{
				case MegaCopyStatusFlags.None:
					break;

				case MegaCopyStatusFlags.Selecting:
					{
						megaCopyStatus = MegaCopyStatusFlags.None;
						// Font selector window
						pictureBoxFontSelectorRubberBand.Bounds = new Rectangle(-30, 0, 20, 20);
						// View editor window
						pictureBoxViewEditorRubberBand.Bounds = new Rectangle(-30, 0, 20, 20);
						break;
					}

				case MegaCopyStatusFlags.Selected:
					break;

				case MegaCopyStatusFlags.Pasting:
					{
						megaCopyStatus = MegaCopyStatusFlags.Selected;
						pictureBoxFontSelectorPasteCursor.Visible = false;
						pictureBoxFontSelectorMegaCopyImage.Visible = false;
						pictureBoxViewEditorPasteCursor.Visible = false;
						pictureBoxViewEditorMegaCopyImage.Visible = false;
					}
					break;
			}
		}

		public void RevalidateClipboard()
		{
			var jsonText = Clipboard.GetText();

			if (string.IsNullOrEmpty(jsonText))
				return;

			int width;
			int height;
			string characterBytes;
			string fontBytes;

			try
			{
				var jsonObj = jsonText.FromJson<ClipboardJSON>();
				int.TryParse(jsonObj.Width, out width);
				int.TryParse(jsonObj.Height, out height);

				characterBytes = jsonObj.Chars;
				fontBytes = jsonObj.Data;
			}
			catch (Exception)
			{
				return;
			}

			if (buttonMegaCopy.Checked)
			{
				var w = 16 * (width + 0);
				var h = 16 * (height + 0);

				pictureBoxFontSelectorMegaCopyImage.Size = new Size(w, h);
				pictureBoxViewEditorMegaCopyImage.Size = new Size(w, h);


				var img = Helpers.NewImage(pictureBoxFontSelectorMegaCopyImage);
				using (var gr = Graphics.FromImage(img))
				{
					gr.FillRectangle(cyanBrush, new Rectangle(0, 0, img.Width, img.Height));
				}

				DrawChars(pictureBoxFontSelectorMegaCopyImage, fontBytes, characterBytes, 0, 0, width, height, !InColorMode, 2);
				pictureBoxViewEditorMegaCopyImage.Image?.Dispose();
				pictureBoxViewEditorMegaCopyImage.Image = pictureBoxFontSelectorMegaCopyImage.Image;

				pictureBoxFontSelectorPasteCursor.Size = new Size(4 + w, 4 + h);
				ResizeFontSelectorPasteCursor();
				pictureBoxViewEditorPasteCursor.Size = new Size(4 + w, 4 + h);
				ResizeViewEditorPasteCursor();
			}
		}

		public void DrawChars(PictureBox targetImage, string data, string chars, int x, int y, int dataWidth, int dataHeight, bool gr0, int pixelsize)
		{
			var img = Helpers.GetImage(targetImage);
			using (var gr = Graphics.FromImage(img))
			{
				for (var i = 0; i < dataHeight; i++)
				{
					for (var j = 0; j < dataWidth; j++)
					{
						DrawChar(gr, data.Substring((i * dataWidth + j) * 16, 16), chars.Substring((i * dataWidth + j) * 2, 2), x + 8 * pixelsize * j, y + 8 * pixelsize * i, gr0, pixelsize);
					}
				}
			}
			targetImage.Refresh();
		}

		public void DrawChar(Graphics gr, string data, string character, int x, int y, bool gr0, int pixelsize)
		{
			var inverse = Convert.ToInt32($"0x{character}", 16) > 127;

			if (gr0)
			{
				for (var i = 0; i < 8; i++)
				{
					var line = Convert.ToInt32($"0x{data.Substring(i * 2, 2)}", 16);
					var bwdata = AtariFont.DecodeMono((byte)line);

					for (var j = 0; j < 8; j++)
					{
						var brush = BrushCache[Convert.ToInt32(!inverse ^ (bwdata[j] == 1))];

						gr.FillRectangle(brush, x + j * pixelsize, y + i * pixelsize, pixelsize, pixelsize);
					}
				}
			}
			else
			{
				for (var i = 0; i < 8; i++)
				{
					var line = Convert.ToInt32("0x" + data.Substring(i * 2, 2), 16);
					var cldata = AtariFont.DecodeColor((byte)line);

					for (var j = 0; j < 4; j++)
					{
						SolidBrush brush;
						if ((inverse) && (cldata[j] == 3))
						{
							brush = BrushCache[5];
						}
						else
						{
							brush = BrushCache[1 + cldata[j]];
						}

						gr.FillRectangle(brush, x + j * pixelsize * 2, y + i * pixelsize, 2 * pixelsize, pixelsize);
					}
				}
			}
		}


		// returns true if character has been edited (different to last saved in undo buffer)
		public bool CharacterEdited()
		{
			var ptr = AtariFont.GetCharacterOffset(SelectedCharacterIndex, checkBoxFontBank.Checked);

			for (var i = 0; i < 8; ++i)
			{
				if (AtariFont.FontBytes[ptr + i] != UndoBuffer.undoBuffer[UndoBuffer.undoBufferIndex, ptr + i])
					return true;
			}

			return false;
		}

		public void SetColor(int colorNum)
		{
			if (ActiveColorNr != colorNum)
			{
				if ((int)(pictureBoxCharacterEditorColor1.Tag) == colorNum)
				{
					ActionCharacterEditorColor1MouseDown();
				}
				else
				{
					ActionCharacterEditorColor2MouseDown();
				}
			}
		}

	}
}
