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
		/// <summary>
		/// The bytes that make up the font characters. 8 bytes/char
		/// </summary>
		public string Data { get; set; }

		/// <summary>
		/// When set then some operation was performed on the pixels in the "Data" container
		/// and the values can't be pasted into the view window
		/// </summary>
		public bool Modified { get; set; }
		/// <summary>
		/// Describes which font the individual rows come from.  If copied from the font area we know the font and bank,
		/// if from the view area we can determine the font from the "Lines"
		/// i.e 111222334
		/// </summary>
		public string FontNr { get; set; }
	}

	public partial class FontMakerForm
	{
		/// <summary>
		/// Internal copy of the JSON used for copy + paste
		/// </summary>
		private string _localCopyOfClipboardData = string.Empty;

		private readonly byte[] _tempPixelBuffer = new byte[40 * 8]; // Init to the max value that it can be and reuse it everywhere
		private byte[,] _pixelBuffer = new byte[8, 8];

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
					// take into account that color 3 is different if the characters is inverted
					var shiftColor3 = SelectedCharacterIndex switch
                    {
                        ((>= 0) and (< 128)) or ((>= 256) and (< 384)) => false,
                        ((>= 128) and (< 256)) or ((>= 384) and (< 512)) => true,
                        _ => throw new NotImplementedException(),
                    };
					var col = charline5col[rx];
					if (col == 4 && shiftColor3) ++col;
					gr.FillRectangle(BrushCache[col], rx * 40, ry * 20, 40, 20);

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
							//var brush = new SolidBrush(character2Color[b, a] == 0 ? AtariPalette[SetOfSelectedColors[1]] : AtariPalette[SetOfSelectedColors[0]]);
							var brush = character2Color[b, a] == 0 ? BrushCache[1] : BrushCache[0];
							gr.FillRectangle(brush, b * 20, a * 20, 20, 20);
							gr.FillRectangle(WhiteBrush, b * 20, a * 20, 1, 1);
						}
					}
				}
				else
				{
					var character5color = AtariFont.Get5ColorCharacter(SelectedCharacterIndex, checkBoxFontBank.Checked);
					var shiftColor3 = SelectedCharacterIndex switch
                    {
                        ((>= 0) and (< 128)) or ((>= 256) and (< 384)) => false,
                        ((>= 128) and (< 256)) or ((>= 384) and (< 512)) => true,
                        _ => throw new NotImplementedException(),
                    };

					for (var y = 0; y < 8; y++)
					{
						for (var x = 0; x < 4; x++)
						{
							var col = Constants.Bits2ColorIndex[character5color[x, y]];
							if (col == 4 && shiftColor3) ++col;
							gr.FillRectangle(BrushCache[col], x * 40, y * 20, 40, 20);
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
						gr.FillRectangle(WhiteBrush, a * 20, b * 20, 1, 1);
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
				var fontNumber = SelectedCharacterIndex / 256; // Font 0 or 1
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
				UndoBuffer.Add2Undo(true); // Add 2 undo but don't change index
			}

			UndoBuffer.Undo(); // Copy the old fonts back

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
			var fontNr = string.Empty; // 1234

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
					var whichFontNr = 1;
					for (var j = CopyPasteRange.X; j <= CopyPasteRange.Right; j++)
					{
						int charInFont;
						if (sourceIsView)
						{
							characterBytes = characterBytes + $"{AtariView.ViewBytes[j, i]:X2}";
							charInFont = (AtariView.ViewBytes[j, i] % 128) * 8 + (AtariView.UseFontOnLine[i] - 1) * 1024;
							whichFontNr = AtariView.UseFontOnLine[i];
						}
						else
						{
							characterBytes = characterBytes + $"{(i * 32 + j) % 256:X2}";

							if (i / 8 == 0)
							{
								charInFont = ((i % 4) * 32 + j) * 8;
							}
							else
							{
								charInFont = ((i % 4 + 4) * 32 + j) * 8;
							} //second font

							charInFont += checkBoxFontBank.Checked ? 2048 : 0; // 3rd or 4th font?

							whichFontNr = (charInFont / 1024) + 1; // What is the font # the character is in?
						}

						for (var k = 0; k < 8; k++)
						{
							fontBytes += $"{AtariFont.FontBytes[charInFont + k]:X2}";
						}
					}
					fontNr += whichFontNr;
				}

				var jo = new ClipboardJSON()
				{
					Width = (CopyPasteRange.Width + 1).ToString(),
					Height = (CopyPasteRange.Height + 1).ToString(),
					Chars = characterBytes,
					Data = fontBytes,
					FontNr = fontNr,
				};
				var json = jo.ToJson();
				SafeSetClipboard(json);

				// The font and characters have been copied to the clipboard
				// Enabled specific clipboard modification/action buttons
				ConfigureClipboardActionButtons();

				UpdateClipboardInformation(CopyPasteRange.Width + 1, CopyPasteRange.Height + 1);
				RevalidateClipboard();
			}
		}

		public void RenderTextToClipboard(string text, bool inverse, bool secondFont)
		{
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;

			var fontInBankOffset = checkBoxFontBank.Checked ? 2048 : 0;

			CopyPasteRange.X = 0;
			CopyPasteRange.Y = 0;
			CopyPasteRange.Width = text.Length-1;
			CopyPasteRange.Height = 0;			// 0 means that is 1 line high

			var chars = text.ToCharArray();
			for (var j = 0; j < text.Length; j++)
			{
				var character = Helpers.AtariConvertChar((byte)chars[j]);

				if (inverse)
				{
					character = (byte)(character | 128);
				}

				characterBytes = characterBytes + $"{character:X2}";
				var charInFont = (character & 127) * 8;

				for (var k = 0; k < 8; k++)
				{
					fontBytes = fontBytes + $"{AtariFont.FontBytes[charInFont + k + fontInBankOffset + (secondFont ? 1024 : 0)]:X2}";
				}
			}

			var jo = new ClipboardJSON()
			{
				Width = text.Length.ToString(),
				Height = "1",
				Chars = characterBytes,
				Data = fontBytes,
				FontNr = checkBoxFontBank.Checked ? "3" : "1",
			};
			var json = jo.ToJson();
			SafeSetClipboard(json);

			UpdateClipboardInformation(text.Length, 1);
			RevalidateClipboard();
		}

		public void ExecutePasteFromClipboard()
		{
			if (buttonMegaCopy.Checked)
			{
				if (SafeGetClipboard() != _localCopyOfClipboardData)
				{
					pictureBoxFontSelectorRubberBand.Visible = false;
					pictureBoxViewEditorRubberBand.Visible = false;
				}

				if (RevalidateClipboard())
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
				var jsonText = SafeGetClipboard();
				var jsonObj = jsonText.FromJson<ClipboardJSON>();
				if (jsonObj == null)
					return;
				int.TryParse(jsonObj.Width, out width);
				int.TryParse(jsonObj.Height, out height);

				characterBytes = jsonObj.Chars;
				fontBytes = jsonObj.Data;

				if (fontBytes == null || fontBytes == string.Empty || fontBytes.Length == 0
				    || characterBytes == null || characterBytes == string.Empty || characterBytes.Length == 0)
				{
					MessageBox.Show(@"Clipboard data parsing error");
					return;
				}
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

		public bool RevalidateClipboard()
		{
			var jsonText = SafeGetClipboard();

			if (string.IsNullOrEmpty(jsonText))
				return false;

			int width;
			int height;
			string characterBytes;
			string fontBytes;

			try
			{
				var jsonObj = jsonText.FromJson<ClipboardJSON>();
				if (jsonObj == null)
					return false;
				int.TryParse(jsonObj.Width, out width);
				int.TryParse(jsonObj.Height, out height);

				characterBytes = jsonObj.Chars;
				fontBytes = jsonObj.Data;

				// Safety check
				if (width < 1 || height < 1) 
					return false;
				var cpWidth = width - 1;
				var cpHeight = height - 1;

				if (CopyPasteRange.Width != cpWidth || CopyPasteRange.Height != cpHeight)
				{
					CopyPasteRange.X = 0;
					CopyPasteRange.Y = 0;
					CopyPasteRange.Width = cpWidth;
					CopyPasteRange.Height = cpHeight;
				}
			}
			catch (Exception)
			{
				return false;
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
					gr.FillRectangle(CyanBrush, new Rectangle(0, 0, img.Width, img.Height));
				}

				DrawChars(pictureBoxFontSelectorMegaCopyImage, fontBytes, characterBytes, 0, 0, width, height, !InColorMode, 2);
				pictureBoxViewEditorMegaCopyImage.Image?.Dispose();
				pictureBoxViewEditorMegaCopyImage.Image = pictureBoxFontSelectorMegaCopyImage.Image;

				// Copy the image into the clipboard preview bitmap
				var imgPreview = Helpers.GetImage(pictureBoxClipboardPreview);
				using (var gr = Graphics.FromImage(imgPreview))
				{
					gr.FillRectangle(BlackBrush, new Rectangle(0, 0, imgPreview.Width, imgPreview.Height));

					var theRect = new Rectangle
					{
						X = 0,
						Y = 0,
						Width = Math.Min(imgPreview.Width, pictureBoxViewEditorMegaCopyImage.Image.Width),
						Height = Math.Min(imgPreview.Height, pictureBoxViewEditorMegaCopyImage.Image.Height),
					};
					gr.DrawImage(pictureBoxFontSelectorMegaCopyImage.Image, theRect, theRect, GraphicsUnit.Pixel);
				}

				pictureBoxClipboardPreview.Refresh();


				pictureBoxFontSelectorPasteCursor.Size = new Size(4 + w, 4 + h);
				ResizeFontSelectorPasteCursor();
				pictureBoxViewEditorPasteCursor.Size = new Size(4 + w, 4 + h);
				ResizeViewEditorPasteCursor();
			}

			return true;
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


		#region Manipulate contents of MegaCopy area

		public bool CheckAllUnique(byte[] toCheck, string fontNr)
		{
			var found = new bool[256];
			for (var i = 0; i < toCheck.Length; ++i)
			{
				var it = toCheck[i];
				if (found[it])
					return false;
				found[it] = true;
			}

			// So far ok.  Make sure that all chars come from the same font
			if (!string.IsNullOrEmpty(fontNr))
			{
				return fontNr.All(ch => ch == fontNr[0]);
			}

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		public void ConfigureClipboardActionButtons()
		{
			var on = buttonMegaCopy.Checked;
			var allUnique = false;
			var isSquare = false;
			if (on)
			{
				// If we are on then determine if the special in place paste can be used.
				// All characters in the selected area need to be unique AND come from the same font
				try
				{
					var jsonText = SafeGetClipboard();
					var jsonObj = jsonText.FromJson<ClipboardJSON>();
					if (jsonObj == null)
						throw new Exception();
					int.TryParse(jsonObj.Width, out var width);
					int.TryParse(jsonObj.Height, out var height);

					var bytes = Convert.FromHexString(jsonObj.Chars);
					var fontNr = jsonObj.FontNr;
					allUnique = CheckAllUnique(bytes, fontNr);
					isSquare = width > 0 && ((InColorMode == false && width == height) || (InColorMode == true && width == height * 2));
				}
				catch (Exception)
				{
					on = false;
				}
			}

			buttonCopyAreaShiftLeft.Enabled = on;
			buttonCopyAreaShiftRight.Enabled = on;
			buttonCopyAreaShiftUp.Enabled = on;
			buttonCopyAreaShiftDown.Enabled = on;
			buttonCopyAreaHMirror.Enabled = on;
			buttonCopyAreaVMirror.Enabled = on;
			buttonCopyAreaInvert.Enabled = on && (InColorMode == false);
			buttonCopyAreaRotateLeft.Enabled = on && isSquare;
			buttonCopyAreaRotateRight.Enabled = on && isSquare;
			buttonPasteInPlace.Enabled = on && allUnique;
			comboBoxPasteIntoFontNr.Enabled = on && allUnique;

			labelCopyAreaInfo.Visible = on;
		}

		public void UpdateClipboardInformation(int w = 0, int h = 0)
		{
			var msg = string.Empty;
			if (w != 0 && h != 0)
			{
				msg = $"Copy Area: {w}x{h}";
			}
			else
			{
				try
				{
					var jsonText = SafeGetClipboard();
					var jsonObj = jsonText.FromJson<ClipboardJSON>();
					if (jsonObj == null)
						throw new Exception();
					int.TryParse(jsonObj.Width, out var width);
					int.TryParse(jsonObj.Height, out var height);

					msg = $"Copy Area: {width}x{height}";
				}
				catch
				{
					msg = "Nothing clipped yet!";
				}
			}

			labelCopyAreaInfo.Text = msg;
		}


		/// <summary>
		/// Convert the fontBytes from the clipboard into a X.Y pixel buffer.
		/// Each pixel in each character is represented by one byte.
		/// </summary>
		/// <returns>Tuple with pixel buffer, used width and used height</returns>
		private (byte[,]?, int, int) GetFontPixelsFromClipboard()
		{
			if (!buttonMegaCopy.Checked)
				return (null, 0, 0);

			var jsonText = SafeGetClipboard();
			if (string.IsNullOrEmpty(jsonText))
				return (null, 0, 0);

			int width;
			int height;
			string fontBytes;

			try
			{
				var jsonObj = jsonText.FromJson<ClipboardJSON>();
				int.TryParse(jsonObj.Width, out width);
				int.TryParse(jsonObj.Height, out height);

				fontBytes = jsonObj.Data;
			}
			catch (Exception)
			{
				return (null, 0, 0);
			}

			// If the cached buffer is big enough use it, otherwise make a bigger one
			if (_pixelBuffer.GetLength(0) < width * 8 || _pixelBuffer.GetLength(1) < height * 8)
				_pixelBuffer = new byte[width * 8, height * 8];

			var src = Convert.FromHexString(fontBytes);
			var srcIndex = 0;
			for (var y = 0; y < height; y++)
			{
				var targetY = y * 8;
				for (var x = 0; x < width; ++x)
				{
					var targetX = x * 8;
					for (var z = 0; z < 8; ++z)
					{
						var line = src[srcIndex++];
						var mask = 128;
						for (var i = 0; i < 8; ++i)
						{
							_pixelBuffer[targetX + i, targetY + z] = (byte)((line & mask) == 0 ? 0 : 1);
							mask >>= 1;
						}
					}
				}
			}

			return (_pixelBuffer, width * 8, height * 8);
		}

		/// <summary>
		/// Convert the pixels back into font bytes in hex and stuff the result into the clipboard JSON
		/// </summary>
		/// <param name="pixels">X.Y expanded pixel bitmap</param>
		/// <returns></returns>
		private void StuffPixelsIntoClipboard(byte[,] pixels)
		{
			var jsonText = SafeGetClipboard();
			if (string.IsNullOrEmpty(jsonText)) return;

			int width;
			int height;
			string characterBytes;
			var fontBytes = string.Empty;
			var fontNr = string.Empty;

			try
			{
				var jsonObj = jsonText.FromJson<ClipboardJSON>();
				int.TryParse(jsonObj.Width, out width);
				int.TryParse(jsonObj.Height, out height);

				characterBytes = jsonObj.Chars;
				fontNr = jsonObj.FontNr;
			}
			catch (Exception)
			{
				return;
			}

			// Convert the pixel bitmap into characters (8x8 pixels)
			for (var y = 0; y < height; ++y)
			{
				var srcY = y * 8;
				for (var x = 0; x < width; ++x)
				{
					var srcX = x * 8;

					for (var inY = 0; inY < 8; ++inY)
					{
						var accu = 0;

						var mask = 128;
						for (var pixelX = 0; pixelX < 8; ++pixelX)
						{
							accu |= (pixels[srcX + pixelX, srcY + inY] > 0) ? mask : 0;
							mask >>= 1;
						}

						fontBytes += $"{accu:X2}";
					}
				}
			}

			var jo = new ClipboardJSON()
			{
				Width = width.ToString(),
				Height = height.ToString(),
				Chars = characterBytes,
				Data = fontBytes,
				Modified = true,
				FontNr = fontNr, // Transfer the original values
			};
			var json = jo.ToJson();
			SafeSetClipboard(json);
		}

		public void ExecuteCopyAreaShiftLeft()
		{
			var (pixelBuffer, pixelWidth, pixelHeight) = GetFontPixelsFromClipboard();
			if (pixelBuffer == null)
				return;

			for (var repeat = 0; repeat < (InColorMode ? 2 : 1); ++repeat)
			{
				// Copy out the left hand column
				for (var y = 0; y < pixelHeight; ++y)
				{
					_tempPixelBuffer[y] = pixelBuffer[0, y];
				}

				// Shift everything left
				for (var x = 0; x < pixelWidth - 1; ++x)
				{
					for (var y = 0; y < pixelHeight; ++y)
					{
						pixelBuffer[x, y] = pixelBuffer[x + 1, y];
					}
				}

				// Restore the left column to the right
				for (var y = 0; y < pixelHeight; ++y)
				{
					pixelBuffer[pixelWidth - 1, y] = _tempPixelBuffer[y];
				}
			}

			StuffPixelsIntoClipboard(pixelBuffer);
			ExecutePasteFromClipboard();
		}

		public void ExecuteCopyAreaShiftRight()
		{
			var (pixelBuffer, pixelWidth, pixelHeight) = GetFontPixelsFromClipboard();
			if (pixelBuffer == null)
				return;

			for (var repeat = 0; repeat < (InColorMode ? 2 : 1); ++repeat)
			{
				// Copy out the right hand column
				for (var y = 0; y < pixelHeight; ++y)
				{
					_tempPixelBuffer[y] = pixelBuffer[pixelWidth - 1, y];
				}

				// Shift everything right
				for (var x = pixelWidth - 1; x > 0; --x)
				{
					for (var y = 0; y < pixelHeight; ++y)
					{
						pixelBuffer[x, y] = pixelBuffer[x - 1, y];
					}
				}

				// Restore the right column to the left
				for (var y = 0; y < pixelHeight; ++y)
				{
					pixelBuffer[0, y] = _tempPixelBuffer[y];
				}
			}

			StuffPixelsIntoClipboard(pixelBuffer);
			ExecutePasteFromClipboard();
		}

		public void ExecuteCopyAreaShiftUp()
		{
			var (pixelBuffer, pixelWidth, pixelHeight) = GetFontPixelsFromClipboard();
			if (pixelBuffer == null)
				return;

			// Copy out the top row
			for (var x = 0; x < pixelWidth; ++x)
			{
				_tempPixelBuffer[x] = pixelBuffer[x, 0];
			}

			// Shift everything up
			for (var y = 0; y < pixelHeight - 1; ++y)
			{
				for (var x = 0; x < pixelWidth; ++x)
				{
					pixelBuffer[x, y] = pixelBuffer[x, y + 1];
				}
			}

			// Restore the top to the bottom
			for (var x = 0; x < pixelWidth; ++x)
			{
				pixelBuffer[x, pixelHeight - 1] = _tempPixelBuffer[x];
			}

			StuffPixelsIntoClipboard(pixelBuffer);
			ExecutePasteFromClipboard();
		}

		public void ExecuteCopyAreaShiftDown()
		{
			var (pixelBuffer, pixelWidth, pixelHeight) = GetFontPixelsFromClipboard();
			if (pixelBuffer == null)
				return;

			// Copy out the bottom row
			for (var x = 0; x < pixelWidth; ++x)
			{
				_tempPixelBuffer[x] = pixelBuffer[x, pixelHeight - 1];
			}

			// Shift everything down
			for (var y = pixelHeight - 1; y > 0; --y)
			{
				for (var x = 0; x < pixelWidth; ++x)
				{
					pixelBuffer[x, y] = pixelBuffer[x, y - 1];
				}
			}

			// Restore the bottom to the top
			for (var x = 0; x < pixelWidth; ++x)
			{
				pixelBuffer[x, 0] = _tempPixelBuffer[x];
			}

			StuffPixelsIntoClipboard(pixelBuffer);
			ExecutePasteFromClipboard();
		}

		/// <summary>
		/// Take the JSON (data/font bytes) and apply them to the characters in the currently selected font.
		/// </summary>
		public void ExecuteClipboardInPlace()
		{
			var jsonText = SafeGetClipboard();

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

			// var fontOffset = (AtariFont.GetCharacterOffset(SelectedCharacterIndex, checkBoxFontBank.Checked) / 1024) * 1024;
			var fontOffset = (comboBoxPasteIntoFontNr.SelectedIndex) * 1024;

			var bytes = Convert.FromHexString(fontBytes);
			var chars = Convert.FromHexString(characterBytes);

			var charIdx = 0;
			var srcFontDataIdx = 0;
			for (var y = 0; y < height; ++y)
			{
				for (var x = 0; x < width; ++x)
				{
					var theCharNr = chars[charIdx++];
					if (theCharNr >= 128)
					{
						// This is the inverse of the character
						// Switch back to the base character
						theCharNr -= 128;
					}

					var charOffset = theCharNr * 8 + fontOffset;
					for (var i = 0; i < 8; ++i)
						AtariFont.FontBytes[charOffset++] = bytes[srcFontDataIdx++];
				}
			}

			RedrawChar();
			RedrawFonts();
			CheckDuplicate();
			RedrawView();
			UndoBuffer.Add2UndoFullDifferenceScan();
			UpdateUndoButtons(false);
		}

		public void ExecuteCopyAreaHorizontalMirror()
		{
			var (pixelBuffer, pixelWidth, pixelHeight) = GetFontPixelsFromClipboard();
			if (pixelBuffer == null)
				return;

			var targetBuffer = (byte[,])pixelBuffer.Clone();
			if (InColorMode)
			{
				// Two bits per pixel
				for (var y = 0; y < pixelHeight; ++y)
				{
					for (var x = 0; x < pixelWidth; x += 2)
					{
						targetBuffer[x, y] = pixelBuffer[pixelWidth - 2 - x, y];
						targetBuffer[x + 1, y] = pixelBuffer[pixelWidth - 1 - x, y];
					}
				}
			}
			else
			{
				// One bit per pixel
				for (var y = 0; y < pixelHeight; ++y)
				{
					for (var x = 0; x < pixelWidth; ++x)
					{
						targetBuffer[x, y] = pixelBuffer[pixelWidth - 1 - x, y];
					}
				}
			}

			StuffPixelsIntoClipboard(targetBuffer);
			ExecutePasteFromClipboard();
		}

		public void ExecuteCopyAreaVerticalMirror()
		{
			var (pixelBuffer, pixelWidth, pixelHeight) = GetFontPixelsFromClipboard();
			if (pixelBuffer == null)
				return;

			var targetBuffer = (byte[,])pixelBuffer.Clone();

			for (var y = 0; y < pixelHeight; ++y)
			{
				for (var x = 0; x < pixelWidth; ++x)
				{
					targetBuffer[x, y] = pixelBuffer[x, pixelHeight - 1 - y];
				}
			}

			StuffPixelsIntoClipboard(targetBuffer);
			ExecutePasteFromClipboard();
		}

		public void ExecuteCopyAreaInvert()
		{
			var (pixelBuffer, pixelWidth, pixelHeight) = GetFontPixelsFromClipboard();
			if (pixelBuffer == null)
				return;

			for (var y = 0; y < pixelHeight; ++y)
			{
				for (var x = 0; x < pixelWidth; ++x)
				{
					pixelBuffer[x, y] = (byte)(pixelBuffer[x, y] == 0 ? 1 : 0);
				}
			}

			StuffPixelsIntoClipboard(pixelBuffer);
			ExecutePasteFromClipboard();
		}

		public void ExecuteCopyAreaRotateLeft()
		{
			var (pixelBuffer, pixelWidth, pixelHeight) = GetFontPixelsFromClipboard();
			if (pixelBuffer == null)
				return;

			var targetBuffer = (byte[,])pixelBuffer.Clone();
			if (InColorMode)
			{
				// Color mode:
				// The pixel space has to be a 2 to 1 ratio
				if (pixelWidth == pixelHeight * 2)
				{
					for (var y = 0; y < pixelHeight; ++y)
					{
						for (var x = 0; x < pixelWidth / 2; ++x)
						{
							targetBuffer[y*2, pixelWidth/2 - x -1] = pixelBuffer[x * 2, y];
							targetBuffer[y*2+1, pixelWidth/2 - x-1] = pixelBuffer[x * 2 + 1, y];
						}
					}
				}
			}
			else
			{
				// Mono mode:
				// The pixel space has to be square
				if (pixelWidth == pixelHeight)
				{

					for (var y = 0; y < pixelHeight; ++y)
					{
						for (var x = 0; x < pixelWidth; ++x)
						{
							targetBuffer[y, pixelWidth - 1 - x] = pixelBuffer[x, y];
						}
					}
				}
			}

			StuffPixelsIntoClipboard(targetBuffer);
			ExecutePasteFromClipboard();
		}

		public void ExecuteCopyAreaRotateRight()
		{
			var (pixelBuffer, pixelWidth, pixelHeight) = GetFontPixelsFromClipboard();
			if (pixelBuffer == null)
				return;

			var targetBuffer = (byte[,])pixelBuffer.Clone();

			if (InColorMode)
			{
				// Color mode:
				// The pixel space has to be a 2 to 1 ratio
				if (pixelWidth == pixelHeight * 2)
				{
					for (var y = 0; y < pixelHeight; ++y)
					{
						for (var x = 0; x < pixelWidth/2; ++x)
						{
							targetBuffer[(pixelHeight - y) * 2 - 2, x] = pixelBuffer[x * 2, y];
							targetBuffer[(pixelHeight - y) * 2 - 1, x] = pixelBuffer[x*2+1, y];
						}
					}
				}
			}
			else
			{
				// Mono mode:
				// The pixel space has to be square
				if (pixelWidth == pixelHeight)
				{
					for (var y = 0; y < pixelHeight; ++y)
					{
						for (var x = 0; x < pixelWidth; ++x)
						{
							targetBuffer[pixelHeight - 1 - y, x] = pixelBuffer[x, y];
						}
					}
				}
			}

			StuffPixelsIntoClipboard(targetBuffer);
			ExecutePasteFromClipboard();
		}

		#endregion

		public string SafeGetClipboard()
		{
			try
			{
				return Clipboard.GetText();
			}
			catch
			{
				return _localCopyOfClipboardData;
			}
		}

		public void SafeSetClipboard(string json)
		{
			try
			{
				Clipboard.SetText(json);
			}
			catch
			{
			}
			_localCopyOfClipboardData = json;
		}
	}
}
