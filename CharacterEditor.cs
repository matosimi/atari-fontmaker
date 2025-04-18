using System.Drawing.Drawing2D;
using System.Media;
using System.Runtime.Serialization;
using TinyJson;
#pragma warning disable CA1416
#pragma warning disable WFO1000

namespace FontMaker
{
	internal class CharacterEditor
	{
	}

	public class ClipboardJson
	{
		/// <summary>
		/// Width of the data. Min 1
		/// </summary>
		public string? Width { get; set; }
		/// <summary>
		/// Height of the data. Min 1
		/// </summary>
		public string? Height { get; set; }

		/// <summary>
		/// The bytes that make up the individual characters. 1 byte per char
		/// </summary>
		public string? Chars { get; set; }
		/// <summary>
		/// The bytes that make up the font characters. 8 bytes/char
		/// </summary>
		public string? Data { get; set; }

		/// <summary>
		/// Describes which font the individual rows come from.  If copied from the font area we know the font and bank,
		/// if from the view area we can determine the font from the "Lines"
		/// i.e. 111222334
		/// </summary>
		public string? FontNr { get; set; }

		/// <summary>
		/// 0/1 per character to indicate if a specific char needs to be used or skipped
		/// </summary>
		public string? Nulls { get; set; }


		[IgnoreDataMember]
		public int ParsedWidth { get; set; }
		[IgnoreDataMember]
		public int ParsedHeight { get; set; }

		/// <summary>
		/// Verify that the parsed JSON is valid and fix if there are any issues
		/// </summary>
		public bool VerifyWidthHeight()
		{
			// Need width and height of at least 1
			if (int.TryParse(Width, out var parsedWidth) == false || int.TryParse(Height, out var parsedHeight) == false)
			{
				return false;
			}

			ParsedWidth = parsedWidth;
			ParsedHeight = parsedHeight;

			if (ParsedWidth < 1 || ParsedHeight < 1)
			{
				return false;
			}

			return true;
		}

		public void FixCharacters()
		{
			// Check that there are width*height*2 characters
			// Chars are hex encoded, hence the *2
			if (string.IsNullOrWhiteSpace(Chars))
			{
				Chars = new string('0', ParsedWidth * ParsedHeight * 2);
			}
			if (Chars.Length < ParsedWidth * ParsedHeight * 2)
			{
				// Append enough to make it the correct length
				Chars += new string('0', ParsedWidth * ParsedHeight * 2 - Chars.Length);
			}
		}

		public void FixData()
		{
			// Check that there are width*height*2*8 chars for the font data
			if (string.IsNullOrWhiteSpace(Data))
			{
				Data = new string('0', ParsedWidth * ParsedHeight * 2 * 8);
			}

			if (Data.Length < ParsedWidth * ParsedHeight * 2 * 8)
			{
				// Append enough to make it the correct length
				Data += new string('0', ParsedWidth * ParsedHeight * 2 * 8 - Data.Length);
			}
		}

		public void FixFontNr()
		{
			// Check that there are height font numbers
			if (string.IsNullOrWhiteSpace(FontNr))
			{
				FontNr = new string('1', ParsedHeight);
			}

			if (FontNr.Length < ParsedHeight)
			{
				// Append enough to make it the correct length
				FontNr += new string('1', ParsedHeight - FontNr.Length);
			}
		}

		public void FixNulls()
		{
			// Check that there are width*height nulls
			if (string.IsNullOrWhiteSpace(Nulls))
			{
				Nulls = new string('0', ParsedWidth * ParsedHeight);
			}

			if (Nulls.Length < ParsedWidth * ParsedHeight)
			{
				// Append enough to make it the correct length
				Nulls += new string('0', ParsedWidth * ParsedHeight - Nulls.Length);
			}
		}
	}

	public partial class FontMakerForm
	{
		public const int EDITOR_WIDTH = 8;
		public const int EDITOR_HEIGHT = 8;

		public const int EDITOR_PIXEL_WIDTH = EDITOR_WIDTH * 20;
		public const int EDITOR_PIXEL_HEIGHT = EDITOR_HEIGHT * 20;

		/// <summary>
		/// Internal copy of the JSON used for copy + paste
		/// </summary>
		public string LocalCopyOfClipboardData { get; set; } = string.Empty;

		private readonly byte[] _tempPixelBuffer = new byte[40 * 8]; // Init to the max value that it can be and reuse it everywhere
		private byte[,] _pixelBuffer = new byte[EDITOR_WIDTH, EDITOR_HEIGHT];

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
			if (e.X < 0 || e.Y < 0 || e.X >= (EDITOR_PIXEL_WIDTH / (InMode5 ? 2 : 1)) || e.Y >= EDITOR_PIXEL_HEIGHT)
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

				if (InColorMode == false)
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
					// Color mode but which one?
					// 4/5 have the same editor
					switch (WhichColorMode)
					{
						default:
						case 4:
						case 5:
						{
							var charline5col = AtariFont.DecodeColor2Bit(AtariFont.FontBytes[hp + ry]);

							for (var a = 0; a < 4; a++)
							{
								charline5col[a] = Constants.Bits2ColorIndex[charline5col[a]];
							}

							var rx = e.X / CharXWidth;

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
							gr.FillRectangle(BrushCache[col], rx * CharXWidth, ry * 20, CharXWidth, 20);

							// Recode to byte and save to charset
							for (var a = 0; a < 4; a++)
							{
								charline5col[a] = Constants.ColorIndex2Bits[charline5col[a]];
							}

							AtariFont.FontBytes[hp + ry] = AtariFont.EncodeColor2Bit(charline5col);
							DoChar();
							break;
						}

						case 10:
						{
							var charLine9Colors = AtariFont.DecodeColor4Bit(AtariFont.FontBytes[hp + ry]);

							for (var a = 0; a < 2; a++)
							{
								charLine9Colors[a] = Constants.FourBits2ColorIndex[charLine9Colors[a]];
							}

							var rx = e.X / (CharXWidth * 2);

							LastCharacterPixelX = rx;

							if (e.Button == MouseButtons.Left)
							{
								if (comboBoxWriteMode.SelectedIndex == 0)
								{
									if (charLine9Colors[rx] != Active4BitColorNr)
									{
										charLine9Colors[rx] = (byte)Active4BitColorNr;
									}
									else
									{
										charLine9Colors[rx] = 0;
									}
								}
								else
								{
									charLine9Colors[rx] = (byte)Active4BitColorNr;
								}
							}
							else if (e.Button == MouseButtons.Right)
							{
								// Delete
								charLine9Colors[rx] = 0;
							} 

							// Draw pixel
							var col = charLine9Colors[rx];
							gr.FillRectangle(BrushCache[col+1], rx * CharXWidth * 2, ry * 20, CharXWidth * 2, 20);

							// Recode to byte and save to charset
							for (var a = 0; a < 2; a++)
							{
								charLine9Colors[a] = Constants.ColorIndex2FourBits[charLine9Colors[a]];
							}

							AtariFont.FontBytes[hp + ry] = AtariFont.EncodeColor4Bit(charLine9Colors);
							DoChar();
							break;
						}
					}

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
				if (e.X < 0 || e.Y < 0 || e.X >= EDITOR_PIXEL_WIDTH || e.Y >= EDITOR_PIXEL_HEIGHT)
					return;
				var je = false;
				int nx;
				var ny = e.Y / 20;

				if (InColorMode)
				{
					switch (WhichColorMode)
					{
						default: // 4,5
						{
							nx = e.X / CharXWidth;
							break;
						}
						case 10:
						{
							nx = e.X / CharXWidth / 2;
							break;
						}
					}
					
				}
				else
				{
					nx = e.X / 20;
				}

				if (e.X < 0 || e.X > EDITOR_PIXEL_WIDTH || e.Y < 0 || e.Y > EDITOR_PIXEL_HEIGHT)
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
					// gr.0 / Mode 2 / Black and White / Mono
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
					switch (WhichColorMode)
					{
						default:
						case 4:
						case 5:
						{
							var character5color = AtariFont.Get5ColorCharacter(SelectedCharacterIndex, checkBoxFontBank.Checked);
							var shiftColor3 = SelectedCharacterIndex switch
							{
								((>= 0) and (< 128)) or ((>= 256) and (< 384)) => false,
								((>= 128) and (< 256)) or ((>= 384) and (< 512)) => true,
								_ => throw new NotImplementedException(),
							};

							// If tall mode, blank the right side of the character
							if (InMode5)
							{
								gr.FillRectangle(EmptyBrush, 4 * CharXWidth, 0, 4 * CharXWidth, 20 * 8);
							}

							for (var y = 0; y < 8; y++)
							{
								for (var x = 0; x < 4; x++)
								{
									var col = Constants.Bits2ColorIndex[character5color[x, y]];
									if (col == 4 && shiftColor3) ++col;
									gr.FillRectangle(BrushCache[col], x * CharXWidth, y * 20, CharXWidth, 20);
								}
							}
							break;
						}
						case 10:
						{
							var character4BitColor = AtariFont.Get4BitColorCharacter(SelectedCharacterIndex, checkBoxFontBank.Checked);

							for (var y = 0; y < 8; y++)
							{
								for (var x = 0; x < 2; x++)
								{
									var col = Constants.FourBits2ColorIndex[character4BitColor[x, y]];
									gr.FillRectangle(BrushCache[col+1], x * CharXWidth * 2, y * 20, CharXWidth * 2, 20);
								}
							}
							break;
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

		private void ActionCharacterEditorColor9Selected()
		{
			Active4BitColorNr = cmbColor9Menu.SelectedIndex;
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
			AtariFont.MirrorHorizontal(SelectedCharacterIndex, checkBoxFontBank.Checked, InColorMode, WhichColorMode);

			UpdateCharacterViews();
		}

		private void ExecuteMirrorVertical()
		{
			AtariFont.MirrorVertical(SelectedCharacterIndex, checkBoxFontBank.Checked);
			UpdateCharacterViews();
		}

		private void ExecuteShiftLeft()
		{
			AtariFont.ShiftLeft(SelectedCharacterIndex, checkBoxFontBank.Checked, InColorMode, WhichColorMode);
			UpdateCharacterViews();
		}

		public void ExecuteShiftRight()
		{
			AtariFont.ShiftRight(SelectedCharacterIndex, checkBoxFontBank.Checked, InColorMode, WhichColorMode);
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
				AtariFontUndoBuffer.Add2Undo(true); // Add 2 undo but don't change index
			}

			AtariFontUndoBuffer.Undo(); // Copy the old fonts back

			UpdateUndoButtons(CharacterEdited());
			RedrawChar();
			RedrawFonts();
			RedrawView();
			return true;
		}

		public bool Redo()
		{
			AtariFontUndoBuffer.Redo();

			UpdateUndoButtons(CharacterEdited());
			RedrawChar();
			RedrawFonts();
			RedrawView();
			return true;
		}

		// updates undo/redo button state based on info if character has been edited and what the buffer index is
		public void UpdateUndoButtons(bool edited)
		{
			var (redoEnabled, undoEnabled) = AtariFontUndoBuffer.GetRedoUndoButtonState(edited);
			buttonRedo.Enabled = redoEnabled;
			buttonUndo.Enabled = undoEnabled;
		}

		public void ExecuteCopyToClipboard(bool sourceIsView)
		{
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;
			var fontNr = string.Empty; // 1234
			var nulls = string.Empty;

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
							nulls += (checkBoxSkipChar0.Checked && AtariView.ViewBytes[j, i] == trackBarSkipCharX.Value) ? '1' : '0';
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
							nulls += '0';
						}

						for (var k = 0; k < 8; k++)
						{
							fontBytes += $"{AtariFont.FontBytes[charInFont + k]:X2}";
						}
					}
					fontNr += whichFontNr;
				}

				var jo = new ClipboardJson()
				{
					Width = (CopyPasteRange.Width + 1).ToString(),
					Height = (CopyPasteRange.Height + 1).ToString(),
					Chars = characterBytes,
					Data = fontBytes,
					FontNr = fontNr,
					Nulls = nulls,
				};
				var json = jo.ToJson();
				SafeSetClipboard(json);

				// The font and characters have been copied to the clipboard
				// Enabled specific clipboard modification/action buttons
				ConfigureClipboardActionButtons();

				UpdateClipboardInformation(CopyPasteRange.Width + 1, CopyPasteRange.Height + 1);
				PastingToView = sourceIsView;
				RevalidateClipboard();
			}
		}

		public void RenderTextToClipboard(string text, bool inverse, bool secondFont)
		{
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;
			var nulls = string.Empty;

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
				nulls += '0';
				var charInFont = (character & 127) * 8;

				for (var k = 0; k < 8; k++)
				{
					fontBytes = fontBytes + $"{AtariFont.FontBytes[charInFont + k + fontInBankOffset + (secondFont ? 1024 : 0)]:X2}";
				}
			}

			var jo = new ClipboardJson()
			{
				Width = text.Length.ToString(),
				Height = "1",
				Chars = characterBytes,
				Data = fontBytes,
				FontNr = checkBoxFontBank.Checked ? "3" : "1",
				Nulls = nulls,
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
				if (SafeGetClipboard() != LocalCopyOfClipboardData)
				{
					pictureBoxFontSelectorRubberBand.Visible = false;
					pictureBoxViewEditorRubberBand.Visible = false;
				}

				var (valid, nextState) = RevalidateClipboard();
				if (valid)
					megaCopyStatus = nextState;
			}
			else
			{
				ExecutePasteFromClipboard(false);
			}
		}

		/// <summary>
		/// Take the clipboard data and paste it into the font or view
		/// </summary>
		/// <param name="targetIsView"></param>
		public void ExecutePasteFromClipboard(bool targetIsView)
		{
			ClipboardJson? jsonObj = null;

			try
			{
				jsonObj = SafeGetClipboard().FromJson<ClipboardJson?>();
				if (jsonObj == null || !jsonObj.VerifyWidthHeight())
				{
					MessageBox.Show(@"Clipboard data parsing error!");
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
				// In MegaCopy mode, we need to paste the data into the font or view.
				// The data can be either a font or a view, so we need to check the target and the relevant
				// data requirements.
				if (targetIsView)
				{
					// Parse into the view area
					// Need the bytes that make up the area, if the bytes are null or not and the area dimensions
					jsonObj.FixCharacters();
					jsonObj.FixNulls();
					PasteClipboardIntoView(jsonObj.Chars, jsonObj.Nulls, jsonObj.ParsedWidth, jsonObj.ParsedHeight);
				}
				else
				{
					// Paste the clipboard data into the font window.
					// We only need the jsonObj.Data information for this.
					jsonObj.FixData();
					var charsBytes = Convert.FromHexString(jsonObj.Data!);

					var fontInBankOffset = checkBoxFontBank.Checked ? 2048 : 0;

					// Run over each character in the clipboard WxH area
					for (var y = 0; y < jsonObj.ParsedHeight; y++)
					{
						for (var x = 0; x < jsonObj.ParsedWidth; x++)
						{
							var i = y + CopyPasteTargetLocation.Y;
							var j = x + CopyPasteTargetLocation.X;
							SelectedCharacterIndex = i * 32 + j;

							int charInFont;
							if (i / 8 == 0)
							{
								// 1st font
								charInFont = ((i % 4) * 32 + j) * 8;
							}
							else
							{
								// 2nd font
								charInFont = ((i % 4 + 4) * 32 + j) * 8;
							} 

							for (var k = 0; k < 8; k++)
							{
								AtariFont.FontBytes[charInFont + k + fontInBankOffset] = charsBytes[(y * jsonObj.ParsedWidth + x) * 8 + k];
							}

							DoChar();
							RedrawChar();
							RedrawViewChar();
						}
					}

					AtariFontUndoBuffer.Add2UndoFullDifferenceScan();
					UpdateUndoButtons(false);
				}
			}
			else
			{
				// Paste a single character into the font
				// We need the jsonObj.Data information for this.
				if (jsonObj.ParsedWidth + jsonObj.ParsedHeight != 2)
				{
					MessageBox.Show($@"Unable to paste clipboard outside MegaCopy mode. Clipboard contains {jsonObj.ParsedWidth}x{jsonObj.ParsedHeight} data.");
					return;
				}

				// Find where the font data bytes start
				var hp = AtariFont.GetCharacterOffset(SelectedCharacterIndex, checkBoxFontBank.Checked);

				// Convert the clipboard font data to bytes
				jsonObj.FixData();
				var bytes = Convert.FromHexString(jsonObj.Data!);
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
				case MegaCopyStatusFlags.PastingView:
				case MegaCopyStatusFlags.PastingFont:
				{
					megaCopyStatus = MegaCopyStatusFlags.Selected;
					pictureBoxFontSelectorPasteCursor.Visible = false;
					pictureBoxFontSelectorMegaCopyImage.Visible = false;
					pictureBoxViewEditorPasteCursor.Visible = false;
					pictureBoxViewEditorMegaCopyImage.Visible = false;
					break;
				}
			}
		}

		/// <summary>
		/// Check what can be done with the contents of the clipboard.
		/// If the clipboard has
		///		.Data and .Chars, then it is an internal MegaCopy clipboard, and we can paste into the view and font
		///		.Data only then we can paste into the font
		///		.Chars only then we can paste into the view
		/// </summary>
		/// <returns></returns>
		public (bool, MegaCopyStatusFlags) RevalidateClipboard()
		{
			ClipboardJson? jsonObj;
			try
			{
				jsonObj = SafeGetClipboard().FromJson<ClipboardJson?>();
				if (jsonObj == null || !jsonObj.VerifyWidthHeight())
					return (false, MegaCopyStatusFlags.None);
			}
			catch (Exception)
			{
				return (false, MegaCopyStatusFlags.None);
			}

			// Adjust the CopyPasteRange to match the clipboard data
			var cpWidth = jsonObj.ParsedWidth - 1;
			var cpHeight = jsonObj.ParsedHeight - 1;

			if (CopyPasteRange.Width != cpWidth || CopyPasteRange.Height != cpHeight)
			{
				CopyPasteRange.X = 0;
				CopyPasteRange.Y = 0;
				CopyPasteRange.Width = cpWidth;
				CopyPasteRange.Height = cpHeight;
			}

			if (buttonMegaCopy.Checked == false)
				return (true, MegaCopyStatusFlags.Pasting);

			// MegaCopy mode
			var canPasteFont = false;
			var canPasteView = false;

			var data = jsonObj.Data ?? string.Empty;
			var chars = jsonObj.Chars ?? string.Empty;

			var neededDataLength = jsonObj.ParsedWidth * jsonObj.ParsedHeight * 8 * 2;  // 8 bytes per character in HEX (2 bytes per character)
			var neededCharsLength = jsonObj.ParsedWidth * jsonObj.ParsedHeight * 2;		// 2 bytes per character in HEX

			if (!string.IsNullOrWhiteSpace(data) && data.Length == neededDataLength)
				canPasteFont = true;

			if (!string.IsNullOrWhiteSpace(chars) && chars.Length == neededCharsLength)
				canPasteView = true;

			// Make sure we have the right data
			jsonObj.FixNulls();

			// Change the size of the font and view rubber bands
			var cursorWidth = 16 * jsonObj.ParsedWidth;
			var cursorHeight = ((PastingToView && InMode5) ? 32 : 16) * jsonObj.ParsedHeight;
			pictureBoxFontSelectorMegaCopyImage.Size = new Size(cursorWidth, cursorHeight);
			pictureBoxViewEditorMegaCopyImage.Size = new Size(cursorWidth, cursorHeight);

			// Make sure that the selector bitmap is created in the correct size
			Helpers.NewImage(pictureBoxFontSelectorMegaCopyImage);

			// Draw the clipboard data into the font selector bitmap
			DrawChars(pictureBoxFontSelectorMegaCopyImage, jsonObj.Data, jsonObj.Chars, jsonObj.Nulls, jsonObj.ParsedWidth, jsonObj.ParsedHeight, 2, PastingToView && InMode5 ? 4 : 2);

			pictureBoxViewEditorMegaCopyImage.Image?.Dispose();
			pictureBoxViewEditorMegaCopyImage.Image = pictureBoxFontSelectorMegaCopyImage.Image;
			
			pictureBoxViewEditorMegaCopyImage.Region?.Dispose();
			var region = pictureBoxFontSelectorMegaCopyImage.Region?.GetRegionData();
			if (region != null)
			{
				try
				{
					pictureBoxViewEditorMegaCopyImage.Region = new Region(region);
				}
				catch (Exception _)
				{
					// ignored
				}
			}

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

			pictureBoxFontSelectorPasteCursor.Size = new Size(4 + cursorWidth, 4 + cursorHeight);
			ResizeFontSelectorPasteCursor();
			pictureBoxViewEditorPasteCursor.Size = new Size(4 + cursorWidth, 4 + cursorHeight);
			ResizeViewEditorPasteCursor();

			if (canPasteFont && canPasteView)
				return (true, MegaCopyStatusFlags.Pasting);
			if (canPasteFont)
				return (true, MegaCopyStatusFlags.PastingFont);
			return (true, MegaCopyStatusFlags.PastingView);
		}

		public void DrawChars(
			PictureBox targetImage, 
			string? data, 
			string? chars, 
			string? nulls,		// Array of 0 or 1. 1 = do not draw character
			int dataWidth, 
			int dataHeight, 
			int pixelSizeX, 
			int pixelSizeY
		)
		{
			targetImage.Region?.Dispose();
			using var graphicsPath = new GraphicsPath();
			var img = Helpers.GetImage(targetImage);
			using (var gr = Graphics.FromImage(img))
			{
				for (var y = 0; y < dataHeight; y++)
				{
					for (var x = 0; x < dataWidth; x++)
					{
						if (nulls == null || nulls[y * dataWidth + x] == '0')
						{
							// Draw a character into the space
							string? charsX2;
							// Check if there is info on which characters to draw.
							// Fallback is the ?
							try
							{
								charsX2 = chars?.Substring((y * dataWidth + x) * 2, 2) ?? "1F"; // The ? in font
							}
							catch (Exception)
							{
								charsX2 = "1F"; // ?
							}

							string? dataX16 = null;
							// Check if there is font data for the character.
							// If there is done then copy the font data from the first font at the chars location
							try
							{
								if (data != null)
									dataX16 = data.Substring((y * dataWidth + x) * 16, 16);
							}
							catch (Exception)
							{
								dataX16 = null;
							}

							if (dataX16 == null)
							{
								// Could not get the font bytes, so copy them from the first font (on bank 1 or 2)
								var bytes = new byte[8];
								Buffer.BlockCopy(AtariFont.FontBytes, AtariFont.GetCharacterOffset(Convert.ToByte(charsX2, 16), checkBoxFontBank.Checked), bytes, 0, 8);
								dataX16 = Convert.ToHexString(bytes);
							}
							
							DrawChar(gr, dataX16, charsX2, 8 * pixelSizeX * x, 8 * pixelSizeY * y, pixelSizeX, pixelSizeY);

							graphicsPath.AddRectangle(new Rectangle(x * 8 * pixelSizeX, y * 8 * pixelSizeY, 8 * pixelSizeX, 8 * pixelSizeY));
						}
					}
				}
			}
			targetImage.Region = new Region(graphicsPath);
			targetImage.Refresh();
		}

		private void DrawChar(Graphics gr, string data, string character, int x, int y, int pixelSizeX, int pixelSizeY)
		{
			var inverse = Convert.ToInt32($"0x{character}", 16) > 127;

			if (!InColorMode)
			{
				// B/W
				for (var i = 0; i < 8; i++)
				{
					var line = Convert.ToInt32($"0x{data.Substring(i * 2, 2)}", 16);
					var bwData = AtariFont.DecodeMono((byte)line);

					for (var j = 0; j < 8; j++)
					{
						var brush = BrushCache[Convert.ToInt32(!inverse ^ (bwData[j] == 1))];

						gr.FillRectangle(brush, x + j * pixelSizeX, y + i * pixelSizeY, pixelSizeX, pixelSizeY);
					}
				}
			}
			else
			{
				switch (WhichColorMode)
				{
					case 4:
					case 5:
					default:
					{
						for (var i = 0; i < 8; i++)
						{
							var line = Convert.ToInt32("0x" + data.Substring(i * 2, 2), 16);
							var twoBitColorData = AtariFont.DecodeColor2Bit((byte)line);

							for (var j = 0; j < 4; j++)
							{
								SolidBrush brush;
								if ((inverse) && (twoBitColorData[j] == 3))
								{
									brush = BrushCache[5];
								}
								else
								{
									brush = BrushCache[1 + twoBitColorData[j]];
								}

								gr.FillRectangle(brush, x + j * pixelSizeX * 2, y + i * pixelSizeY, 2 * pixelSizeX, pixelSizeY);
							}
						}
						break;
					}

					case 10:
					{
						for (var i = 0; i < 8; i++)
						{
							var line = Convert.ToInt32("0x" + data.Substring(i * 2, 2), 16);
							var fourBitColorData = AtariFont.DecodeColor4Bit((byte)line);

							for (var j = 0; j < 2; j++)
							{
								var brush = BrushCache[1 + Constants.FourBits2ColorIndex[fourBitColorData[j]]];

								gr.FillRectangle(brush, x + j * pixelSizeX * 4, y + i * pixelSizeY, 4 * pixelSizeX, pixelSizeY);
							}
						}
						break;
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
				if (AtariFont.FontBytes[ptr + i] != AtariFontUndoBuffer.undoBuffer[AtariFontUndoBuffer.undoBufferIndex, ptr + i])
					return true;
			}

			return false;
		}

		public void SetColor(int colorNum)
		{
			switch (WhichColorMode)
			{
				case 4:
				case 5:
				default:
				{
					if (ActiveColorNr != colorNum && colorNum is >= 2 and <= 4)
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

					break;
				}
				case 10:
				{
					cmbColor9Menu.SelectedIndex = colorNum - 1;
					break;
				}
			}
		}


		#region Manipulate contents of MegaCopy area

		public bool CheckAllUnique(byte[] toCheck, string? fontNr)
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
					var jsonObj = SafeGetClipboard().FromJson<ClipboardJson?>();
					if (jsonObj == null || !jsonObj.VerifyWidthHeight())
						throw new Exception();

					var bytes = Convert.FromHexString(jsonObj.Chars ?? string.Empty);
					var fontNr = jsonObj.FontNr;
					allUnique = CheckAllUnique(bytes, fontNr);

					if (InColorMode == false)
						isSquare = jsonObj.ParsedWidth > 0 && jsonObj.ParsedWidth == jsonObj.ParsedHeight;
					else
					{
						switch (WhichColorMode)
						{
							case 4:
							case 5:
							default:
								isSquare = jsonObj.ParsedWidth > 0 && jsonObj.ParsedWidth == jsonObj.ParsedHeight * 2;
								break;
							case 10:
								isSquare = jsonObj.ParsedWidth > 0 && jsonObj.ParsedWidth == jsonObj.ParsedHeight * 4;
								break;
						}
					}
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
			string msg;
			if (w != 0 && h != 0)
			{
				msg = $"Copy Area: {w}x{h}";
			}
			else
			{
				// Try and parse the data from the clipboard
				try
				{
					var jsonObj = SafeGetClipboard().FromJson<ClipboardJson?>();
					if (jsonObj == null || !jsonObj.VerifyWidthHeight())
						throw new Exception();

					msg = $"Copy Area: {jsonObj.ParsedWidth}x{jsonObj.ParsedHeight}";
				}
				catch
				{
					msg = "Nothing clipped yet!";
				}
			}

			labelCopyAreaInfo.Text = msg;
		}


		/// <summary>
		/// Convert the fontBytes from the clipboard into an X.Y pixel buffer.
		/// Each pixel in each character is represented by one byte.
		/// </summary>
		/// <returns>Tuple with pixel buffer, used width and used height</returns>
		private (byte[,]?, int, int) GetFontPixelsFromClipboard()
		{
			if (!buttonMegaCopy.Checked)
				return (null, 0, 0);

			string? fontBytes;
			ClipboardJson? jsonObj;

			try
			{
				jsonObj = SafeGetClipboard().FromJson<ClipboardJson?>();
				if (jsonObj == null || !jsonObj.VerifyWidthHeight())
					return (null, 0, 0);

				fontBytes = jsonObj.Data;
			}
			catch (Exception)
			{
				return (null, 0, 0);
			}

			if (string.IsNullOrWhiteSpace(fontBytes))
				return (null, 0, 0);

			try
			{

				// If the cached buffer is big enough use it, otherwise make a bigger one
				if (_pixelBuffer.GetLength(0) < jsonObj.ParsedWidth * 8 || _pixelBuffer.GetLength(1) < jsonObj.ParsedHeight * 8)
					_pixelBuffer = new byte[jsonObj.ParsedWidth * 8, jsonObj.ParsedHeight * 8];

				var src = Convert.FromHexString(fontBytes);
				var srcIndex = 0;
				for (var y = 0; y < jsonObj.ParsedHeight; y++)
				{
					var targetY = y * 8;
					for (var x = 0; x < jsonObj.ParsedWidth; ++x)
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

				return (_pixelBuffer, jsonObj.ParsedWidth * 8, jsonObj.ParsedHeight * 8);
			}
			catch (Exception)
			{
				return (null, 0, 0);
			}
		}

		/// <summary>
		/// Convert the pixels back into font bytes in hex and stuff the result into the clipboard JSON
		/// </summary>
		/// <param name="pixels">X.Y expanded pixel bitmap</param>
		/// <returns></returns>
		private void StuffPixelsIntoClipboard(byte[,] pixels)
		{
			var jsonText = SafeGetClipboard();
			if (string.IsNullOrWhiteSpace(jsonText)) return;

			string? characterBytes;
			string? nulls;
			var fontBytes = string.Empty;
			string? fontNr;

			ClipboardJson? jsonObj;
			try
			{
				jsonObj = jsonText.FromJson<ClipboardJson?>();
				if (jsonObj == null || !jsonObj.VerifyWidthHeight())
					return;

				jsonObj.FixNulls();
				jsonObj.FixCharacters();
				jsonObj.FixNulls();

				characterBytes = jsonObj.Chars;
				fontNr = jsonObj.FontNr;
				nulls = jsonObj.Nulls;
			}
			catch (Exception)
			{
				return;
			}

			// Convert the pixel bitmap into characters (8x8 pixels)
			for (var y = 0; y < jsonObj.ParsedHeight; ++y)
			{
				var srcY = y * 8;
				for (var x = 0; x < jsonObj.ParsedWidth; ++x)
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

			var jo = new ClipboardJson()
			{
				Width = jsonObj.ParsedWidth.ToString(),
				Height = jsonObj.ParsedHeight.ToString(),
				Chars = characterBytes,
				Data = fontBytes,
				FontNr = fontNr, // Transfer the original values
				Nulls = nulls,
			};
			var json = jo.ToJson();
			SafeSetClipboard(json);
		}

		public void ExecuteCopyAreaShiftLeft()
		{
			var (pixelBuffer, pixelWidth, pixelHeight) = GetFontPixelsFromClipboard();
			if (pixelBuffer == null)
				return;

			var numShifts = AtariFont.HowManyPixels(InColorMode, WhichColorMode);

			for (var repeat = 0; repeat < numShifts; ++repeat)
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

			var numShifts = AtariFont.HowManyPixels(InColorMode, WhichColorMode);

			for (var repeat = 0; repeat < numShifts; ++repeat)
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

			string? characterBytes;
			string? fontBytes;

			ClipboardJson? jsonObj;

			try
			{
				jsonObj = jsonText.FromJson<ClipboardJson?>();
				if (jsonObj == null || !jsonObj.VerifyWidthHeight())
					return;

				jsonObj.FixData();
				jsonObj.FixCharacters();

				characterBytes = jsonObj.Chars;
				fontBytes = jsonObj.Data;
			}
			catch (Exception)
			{
				return;
			}

			var fontOffset = (comboBoxPasteIntoFontNr.SelectedIndex) * 1024;

			var bytes = Convert.FromHexString(fontBytes);
			var chars = Convert.FromHexString(characterBytes);

			var charIdx = 0;
			var srcFontDataIdx = 0;
			for (var y = 0; y < jsonObj.ParsedHeight; ++y)
			{
				for (var x = 0; x < jsonObj.ParsedWidth; ++x)
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
			AtariFontUndoBuffer.Add2UndoFullDifferenceScan();
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
				switch (WhichColorMode)
				{
					default:
					case 4:
					case 5:
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
						break;
					}
					case 10:
					{
						// Four bits per pixel
						for (var y = 0; y < pixelHeight; ++y)
						{
							for (var x = 0; x < pixelWidth; x += 4)
							{
								targetBuffer[x, y] = pixelBuffer[pixelWidth - 4 - x, y];
								targetBuffer[x + 1, y] = pixelBuffer[pixelWidth - 3 - x, y];
								targetBuffer[x + 2, y] = pixelBuffer[pixelWidth - 2 - x, y];
								targetBuffer[x + 3, y] = pixelBuffer[pixelWidth - 1 - x, y];
							}
						}

						break;
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
				switch (WhichColorMode)
				{
					case 4:
					case 5:
					default:
					{
						// The pixel space has to be a 2 to 1 ratio
						if (pixelWidth == pixelHeight * 2)
						{
							for (var y = 0; y < pixelHeight; ++y)
							{
								for (var x = 0; x < pixelWidth / 2; ++x)
								{
									targetBuffer[y * 2, pixelWidth / 2 - x - 1] = pixelBuffer[x * 2, y];
									targetBuffer[y * 2 + 1, pixelWidth / 2 - x - 1] = pixelBuffer[x * 2 + 1, y];
								}
							}
						}

						break;
					}
					case 10:
					{
						// The pixel space has to be a 4 to 1 ratio
						if (pixelWidth == pixelHeight * 4)
						{
							for (var y = 0; y < pixelHeight; ++y)
							{
								for (var x = 0; x < pixelWidth / 4; ++x)
								{
									targetBuffer[y * 4, pixelWidth / 4 - x - 1]     = pixelBuffer[x * 4, y];
									targetBuffer[y * 4 + 1, pixelWidth / 4 - x - 1] = pixelBuffer[x * 4 + 1, y];
									targetBuffer[y * 4 + 2, pixelWidth / 4 - x - 1] = pixelBuffer[x * 4 + 2, y];
									targetBuffer[y * 4 + 3, pixelWidth / 4 - x - 1] = pixelBuffer[x * 4 + 3, y];
								}
							}
						}

						break;
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
				switch (WhichColorMode)
				{
					case 4:
					case 5:
					default:
					{
						// The pixel space has to be a 2 to 1 ratio
						if (pixelWidth == pixelHeight * 2)
						{
							for (var y = 0; y < pixelHeight; ++y)
							{
								for (var x = 0; x < pixelWidth / 2; ++x)
								{
									targetBuffer[(pixelHeight - y) * 2 - 2, x] = pixelBuffer[x * 2, y];
									targetBuffer[(pixelHeight - y) * 2 - 1, x] = pixelBuffer[x * 2 + 1, y];
								}
							}
						}

						break;
					}
					case 10:
					{
						// The pixel space has to be a 4 to 1 ratio
						if (pixelWidth == pixelHeight * 4)
						{
							for (var y = 0; y < pixelHeight; ++y)
							{
								for (var x = 0; x < pixelWidth / 4; ++x)
								{
									targetBuffer[(pixelHeight - y) * 4 - 4, x] = pixelBuffer[x * 4, y];
									targetBuffer[(pixelHeight - y) * 4 - 3, x] = pixelBuffer[x * 4 + 1, y];
									targetBuffer[(pixelHeight - y) * 4 - 2, x] = pixelBuffer[x * 4 + 2, y];
									targetBuffer[(pixelHeight - y) * 4 - 1, x] = pixelBuffer[x * 4 + 3, y];
								}
							}
						}
						break;
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
				return LocalCopyOfClipboardData;
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
				// ignored
			}

			LocalCopyOfClipboardData = json;
		}

		#region Init

		/// <summary>
		/// Populate the Color9Menu items.
		/// </summary>
		private void BuildColorSelector()
		{
			cmbColor9Menu.Items.Clear();
			cmbColor9Menu.ResetText();

			for (var i = 0; i < 9; ++i)
			{
				cmbColor9Menu.Items.Add($"Color #{i}");
			}

			cmbColor9Menu.SelectedIndex = 2;
		}

		private void Color9Menu_DrawItem(DrawItemEventArgs e)
		{
			if (e.Index >= 0)
			{
				// Draw the background 
				var brush = BrushCache[e.Index + 1];		// +1 to skip the lumo value
				e.Graphics.FillRectangle(brush, e.Bounds);

				//e.DrawFocusRectangle();
				e.Graphics.DrawString($"{e.Index}", this.Font, brush.Color.G > 128 ? BlackBrush : WhiteBrush, e.Bounds.X, e.Bounds.Y);
			}
		}

		#endregion
	}
}
