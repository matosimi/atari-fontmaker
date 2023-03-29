using System.Media;

namespace FontMaker
{
	internal class MainCharacter
	{
	}

	public partial class TMainForm
	{
		// Character editor interactions

		#region Font view interactions

		public void i_chMouseDown(object sender, MouseEventArgs e)
		{
			if (e.X < 0 || e.Y < 0 || e.X >= i_ch.Width || e.Y >= i_ch.Height)
				return;

			var img = GetImage(i_ch);
			using (var gr = Graphics.FromImage(img))
			{
				clck = true;
				ButtonHeld = e.Button;

				if (Control.ModifierKeys == Keys.Control)
				{
					clck = false;
				} //ctrl+click no button toggle

				var hp = MainUnit.GetCharacterPointer(selectedCharacterIndex, cbFontBank.Checked);
				var ry = e.Y / 20;
				cly = ry;

				if (!gfx)
				{
					var charline2col = MainUnit.DecodeBW(ft[hp + ry]);
					var rx = e.X / 20;
					clx = rx;

					if (e.Button == MouseButtons.Left)
					{
						if (ComboBoxWriteMode.SelectedIndex == 0)
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

					ft[hp + ry] = MainUnit.EncodeBW(charline2col);
					DoChar();

					var brush = cpalBrushes[charline2col[rx] == 1 ? 0 : 1];
					gr.FillRectangle(brush, rx * 20, ry * 20, 20, 20);
					gr.FillRectangle(cpalBrushes[0], rx * 20, ry * 20, 1, 1);
				}
				else
				{
					var charline5col = MainUnit.DecodeCL(ft[hp + ry]);

					for (var a = 0; a < 4; a++)
					{
						charline5col[a] = bits2colorIndex[charline5col[a]];
					}

					var rx = e.X / 40;
					clx = rx;

					if (e.Button == MouseButtons.Left)
					{
						if (ComboBoxWriteMode.SelectedIndex == 0)
						{
							if (charline5col[rx] != activeColorNr)
							{
								charline5col[rx] = (byte)activeColorNr;
							}
							else
							{
								charline5col[rx] = 1;
							}
						}
						else
						{
							charline5col[rx] = (byte)activeColorNr;
						}
					}
					else if (e.Button == MouseButtons.Right)
					{
						charline5col[rx] = 1;
					} //delete

					// Draw pixel
					gr.FillRectangle(cpalBrushes[charline5col[rx]], rx * 40, ry * 20, 40, 20);

					// Recode to byte and save to charset
					for (var a = 0; a < 4; a++)
					{
						charline5col[a] = colorIndex2bits[charline5col[a]];
					}

					ft[hp + ry] = MainUnit.EncodeCL(charline5col);
					DoChar();
				}

				RedrawViewChar();
				UpdateUndoButtons(CharacterEdited());
				CheckDuplicate();
			}
			i_ch.Refresh();
		}

		public void i_chMouseMove(object sender, MouseEventArgs e)
		{
			if (clck)
			{
				var je = false;
				var nx = 0;
				var ny = e.Y / 20;

				if (gfx)
				{
					nx = e.X / 40;
				}
				else
				{
					nx = e.X / 20;
				}

				if ((e.X < 0) || (e.X > i_ch.Width) || (e.Y < 0) || (e.Y > i_ch.Height))
				{
					je = true;
				}

				if ((!je) && ((nx != clx) || (ny != cly)))
				{
					i_chMouseDown(null, new MouseEventArgs(ButtonHeld, 1, e.X, e.Y, 0));
				}
			}
		}

		public void i_chMouseUp(object sender, MouseEventArgs e)
		{
			clck = false;
		}

		#endregion

		#region Draw font character


		// redraws character that is being edited/selected in character edit window
		public void RedrawChar()
		{
			var img = GetImage(i_ch);
			using (var gr = Graphics.FromImage(img))
			{
				if (!gfx)
				{
					//gr.0
					var character2Color = MainUnit.Get2ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

					for (var a = 0; a < 8; a++)
					{
						for (var b = 0; b < 8; b++)
						{
							var brush = new SolidBrush(character2Color[b, a] == 0 ? palette[cpal[1]] : palette[cpal[0]]);
							gr.FillRectangle(brush, b * 20, a * 20, 20, 20);
							gr.FillRectangle(whiteBrush, b * 20, a * 20, 1, 1);
						}
					}
				}
				else
				{
					var character5color = MainUnit.Get5ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

					for (var a = 0; a < 8; a++)
					{
						for (var b = 0; b < 4; b++)
						{
							var brush = cpalBrushes[bits2colorIndex[character5color[b, a]]];
							gr.FillRectangle(brush, b * 40, a * 20, 40, 20);
						}
					}
				}
			}
			i_ch.Refresh();
		}

		public void RedrawGrid()
		{
			var img = GetImage(i_ch);
			using (var gr = Graphics.FromImage(img))
			{
				var brush = cpalBrushes[1];
				gr.FillRectangle(brush, 0, 0, i_ch.Width, i_ch.Height);

				for (var b = 0; b < 8; b++)
				{
					for (var a = 0; a < 8; a++)
					{
						gr.FillRectangle(whiteBrush, a * 20, b * 20, 1, 1);
					}
				}
			}
			i_ch.Refresh();
		}

		#endregion

		#region Font button handlers
		// All the character editor buttons have their click handlers defined here

		#region Left action buttons: Color selector, Rotate Left, Horizontal Mirror, Shift Left, Shift Down, Restore Default, Restore Last Saved, Copy
		private void Ic1MouseDown(object sender, MouseEventArgs e)
		{
			var bu = (int)Ic1.Tag;
			Ic1.Tag = activeColorNr;
			activeColorNr = bu;
			RedrawPal();
		}

		private void ButtonRotateLeftClicked(object sender, EventArgs e)
		{
			ExecuteRotateLeft();
		}

		private void ButtonMirrorHorizontalClicked(object sender, EventArgs e)
		{
			ExecuteMirrorHorizontal();
		}

		private void ButtonShiftLeftClicked(object sender, EventArgs e)
		{
			ExecuteShiftLeft();
		}

		public void ButtonShiftDownClicked(object sender, EventArgs e)
		{
			ExecuteShiftDown();
		}

		public void ButtonRestoreFromDefaultFontClicked(object sender, EventArgs e)
		{
			try
			{
				var whichCharacter = selectedCharacterIndex & 127;

				// Load the font bytes
				byte[] data = MainUnit.GetResource<byte[]>("Default.fnt");

				var fontNumber = selectedCharacterIndex > 255 ? 1 : 0;
				fontNumber += cbFontBank.Checked ? 2 : 0;

				for (var a = 0; a < 8; a++)
				{
					ft[whichCharacter * 8 + a + fontNumber * 1024] = data[whichCharacter * 8 + a];
				}

				DoChar();
				RedrawViewChar();
				I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
				CheckDuplicate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void ButtonRestoreFromLastSavedClicked(object sender, EventArgs e)
		{
			try
			{
				var whichCharacter = selectedCharacterIndex & 127;
				var fontNumber = selectedCharacterIndex / 256;       // Font 0 or 1
				fontNumber += cbFontBank.Checked ? 2 : 0;

				var filename = fname1;
				if (fontNumber == 1) filename = fname2;
				else if (fontNumber == 2) filename = fname3;
				else if (fontNumber == 3) filename = fname4;

				// Load the font bytes
				var data = File.ReadAllBytes(filename);

				var characterOffset = whichCharacter * 8;

				for (var a = 0; a < 8; a++)
				{
					ft[characterOffset + fontNumber * 1024 + a] = data[characterOffset + a];
				}

				DoChar();
				RedrawViewChar();
				I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
				CheckDuplicate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void ButtonCopyToClipboardClicked(object sender, EventArgs e)
		{
			ExecuteCopyToClipboard(false);
		}

		#endregion // Left action buttons

		#region Right action buttons: Color selector, Rotate Right, Vertical Mirror, Shift Right, Shift Up, Invert, Clear, Paste

		private void Ic2MouseDown(object sender, MouseEventArgs e)
		{
			var bu = (int)Ic2.Tag;
			Ic2.Tag = activeColorNr;
			activeColorNr = bu;
			RedrawPal();
		}

		private void ButtonRotateRightClicked(object sender, EventArgs e)
		{
			ExecuteRotateRight();
		}

		private void ButtonMirrorVerticalClicked(object sender, EventArgs e)
		{
			ExecuteMirrorVertical();
		}

		public void ButtonShiftRightClicked(object sender, EventArgs e)
		{
			ExecuteShiftRight();
		}

		private void ButtonShiftUpClicked(object sender, EventArgs e)
		{
			ExecuteShiftUp();
		}

		public void ButtonInverseClicked(object sender, EventArgs e)
		{
			ExecuteInvertCharacter();
		}

		public void ButtonClearClicked(object sender, EventArgs e)
		{
			ExecuteClearCharacter();
		}

		public void ButtonPasteClicked(object sender, EventArgs e)
		{
			if (SpeedButtonMegaCopy.Checked)
			{
				if (Clipboard.GetText() != clipboardLocal)
				{
					Shape1.Visible = false;
					Shape1v.Visible = false;
				}

				RevalidateClipboard();
				megaCopyStatus = TMegaCopyStatus.Pasting;
			}
			else
			{
				ExecutePastFromClipboard(false);
			}
		}

		#endregion // Right action buttons

		#endregion

		#region Character editor actions

		public void ExecuteRotateLeft()
		{
			var tmp2 = new byte[8, 8];

			if (!gfx)
			{
				var src2 = MainUnit.Get2ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 8; b++)
					{
						tmp2[b, a] = src2[7 - a, b];
					}
				}

				MainUnit.Set2ColorCharacter(tmp2, selectedCharacterIndex, cbFontBank.Checked);
				DoChar();
				RedrawChar();
				RedrawViewChar();
				CheckDuplicate();
			}
		}

		public void ExecuteRotateRight()
		{
			var tmp2 = new byte[8, 8];

			if (!gfx)
			{
				var src2 = MainUnit.Get2ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 8; b++)
					{
						tmp2[b, a] = src2[a, 7 - b];
					}
				}

				MainUnit.Set2ColorCharacter(tmp2, selectedCharacterIndex, cbFontBank.Checked);
				DoChar();
				RedrawChar();
				RedrawViewChar();
				CheckDuplicate();
			}
		}


		public void ExecuteMirrorHorizontal()
		{
			var tmp = new byte[8, 8];

			if (!gfx)
			{
				var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 8; b++)
					{
						tmp[b, a] = src[7 - b, a];
					}
				}

				MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex, cbFontBank.Checked);
			}
			else
			{
				var src5 = MainUnit.Get5ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 4; b++)
					{
						tmp[b, a] = src5[3 - b, a];
					}
				}

				MainUnit.Set5ColorCharacter(tmp, selectedCharacterIndex, cbFontBank.Checked);
			}

			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		public void ExecuteMirrorVertical()
		{
			var tmp = new byte[8, 8];

			var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 8; b++)
				{
					tmp[a, b] = src[a, 7 - b];
				}
			}

			MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex, cbFontBank.Checked);
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}


		private void ExecuteShiftLeft()
		{
			var tmp = new byte[8, 8];
			int repeatTime = repeatTime = gfx ? 1 : 0;   // perform same shift twice in mode 4
			var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

			for (var i = 0; i <= repeatTime; i++)
			{
				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 8; b++)
					{
						var h = b < 7 ? b + 1 : 0;

						tmp[b, a] = src[h, a];
					}
				}

				if (repeatTime == 1)
					src = tmp.Clone() as byte[,];
			}

			MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex, cbFontBank.Checked);
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		public void ExecuteShiftRight()
		{
			var tmp = new byte[8, 8];
			int repeatTime = repeatTime = gfx ? 1 : 0;  // perform same shift twice in mode 4
			var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

			for (var i = 0; i <= repeatTime; i++)
			{
				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 8; b++)
					{
						var h = b > 0 ? b - 1 : 7;

						tmp[b, a] = src[h, a];
					}
				}

				if (repeatTime == 1)
					src = tmp.Clone() as byte[,];
			}

			MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex, cbFontBank.Checked);
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		private void ExecuteShiftUp()
		{
			var tmp = new byte[8, 8];

			var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 8; b++)
				{
					var h = (a < 7) ? a + 1 : 0;

					tmp[b, a] = src[b, h];
				}
			}

			MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex, cbFontBank.Checked);
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		private void ExecuteShiftDown()
		{
			var tmp = new byte[8, 8];
			var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 8; b++)
				{
					var h = a > 0 ? a - 1 : 7;
					tmp[b, a] = src[b, h];
				}
			}

			MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex, cbFontBank.Checked);
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		private void ExecuteInvertCharacter()
		{
			if (!gfx)
			{
				var hp = MainUnit.GetCharacterPointer(selectedCharacterIndex, cbFontBank.Checked);

				for (var a = 0; a < 8; a++)
				{
					ft[hp + a] = (byte)(ft[hp + a] ^ 255);
				}

				DoChar();
				RedrawChar();
				RedrawViewChar();
				CheckDuplicate();
			}
			else
			{
				SystemSounds.Beep.Play();
			}
		}

		public void ExecuteClearCharacter()
		{
			var hp = MainUnit.GetCharacterPointer(selectedCharacterIndex, cbFontBank.Checked);

			for (var a = 0; a < 8; a++)
			{
				ft[hp + a] = 0;
			}

			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		#endregion // Character editor actions

	}
}
