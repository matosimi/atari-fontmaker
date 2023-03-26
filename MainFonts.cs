using System.Drawing.Drawing2D;

namespace FontMaker
{
	internal class MainFonts
	{
	}

	// All functions that interact with the four fonts can be found here
	public partial class TMainForm
	{

		public void SetCharCursor()
		{
			if (CharacterEdited())
			{
				Add2Undo(true);
			}

			selectedCharacterIndex = selectedCharacterIndex % 512;
			var bx = selectedCharacterIndex % 32;
			var by = selectedCharacterIndex / 32;
			I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0));
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
				if (ft[p1 + i + fontBankOffset] != ft[p2 + i + fontBankOffset])
				{
					diff = true;
				}

				i--;
			}
			while (!((i < 0) || (diff == true)));

			return !diff;
		}

		// Finds nearest duplicate char to duplicateCharacterIndex
		public int FindDuplicateChar()
		{
			var myChar = selectedCharacterIndex % 256;
			var fontNr = selectedCharacterIndex / 256;		// 0/1 index into the font bank

			var fontBankOffset = cbFontBank.Checked ? 2048 : 0;

			var inverse = myChar / 128;

			var nextCharIdx = duplicateCharacterIndex + 1;
			nextCharIdx = nextCharIdx % 128 + 128 * inverse;
			var found = false;

			while (!found && (nextCharIdx + fontNr * 256 != duplicateCharacterIndex))
			{
				if (CompareChars(nextCharIdx, selectedCharacterIndex, fontNr, fontBankOffset) && (nextCharIdx + fontNr * 256 != selectedCharacterIndex))
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
				res = selectedCharacterIndex;
			}

			return res;
		}

		public bool IsMousePositionValidForPasting(int x, int y)
		{
			if ((x >= I_fn.Width - (copyRange.Width) * 16) || (y >= I_fn.Height - (copyRange.Height) * 16))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public void I_fnMouseDown(object sender, MouseEventArgs e)
		{
			int fontchar = 0;
			int fontnr = 0;

			if (e.X < 0 || e.X >= I_fn.Width || e.Y < 0 || e.Y >= I_fn.Height)
			{
				return;
			}

			if (!SpeedButtonMegaCopy.Checked)
			{
				if (CharacterEdited())
				{
					Add2Undo(true);
				}
			}

			var rx = e.X / 16;
			var ry = e.Y / 16;
			selectedCharacterIndex = rx + ry * 32;

			if (selectedCharacterIndex > 255)
			{
				fontchar = selectedCharacterIndex % 256;
				fontnr = 2;
			}
			else
			{
				fontchar = selectedCharacterIndex;
				fontnr = 1;
			}

			if (SpeedButtonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case TMegaCopyStatus.None:
					case TMegaCopyStatus.Selected:
						{
							if (e.Button == MouseButtons.Left)
							{
								//define copy origin point
								copyRange.Y = ry;
								copyRange.X = rx;
								megaCopyStatus = TMegaCopyStatus.Selecting;
								Shape1.Left = I_fn.Left + e.X - e.X % 16 - 2;
								Shape1.Top = I_fn.Top + e.Y - e.Y % 16 - 2;
								Shape1.Width = 20;
								Shape1.Height = 20;
								Shape1.Visible = true;
								Shape1v.Visible = false;
							}
						}
						break;

					case TMegaCopyStatus.Pasting:
						{
							if (!IsMousePositionValidForPasting(e.X, e.Y))
							{
								return;
							}

							if (e.Button == MouseButtons.Left)
							{
								copyTarget = new Point(rx, ry);
								Add2UndoFullDifferenceScan();
								ExecutePastFromClipboard(false);
								ResetMegaCopyStatus();
							}

							// reset selection by right doubleclick
							/* TODO:
							if ((Shift.Contains(ssDouble)) && (Shift.Contains(ssRight)))
							{
								ResetMegaCopyStatus();
							}
							*/
						}
						break;
				}
			}
			else
			{
				Shape1.Left = I_fn.Bounds.Left + e.X - e.X % 16 - 2;
				Shape1.Top = I_fn.Bounds.Top + e.Y - e.Y % 16 - 2;

				copyRange.Y = ry;
				copyRange.X = rx;
				copyRange.Width = 0;
				copyRange.Height = 0;
				l_char.Text = $@"Char: Font {fontnr} ${fontchar:X2} #{fontchar}";
				RedrawChar();
				CheckDuplicate();
			}
		}

		public void I_fnMouseUp(object sender, MouseEventArgs e)
		{
			if ((e.X >= I_fn.Width) || (e.Y >= I_fn.Height))
			{
				return;
			}

			var rx = e.X / 16;
			var ry = e.Y / 16;

			if (SpeedButtonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case TMegaCopyStatus.Selecting:
						{
							if (ry <= copyRange.Y)
							{
								copyRange.Height = 0;
							}
							else
							{
								copyRange.Height = ry - copyRange.Y;
							}

							if (rx <= copyRange.X)
							{
								copyRange.Width = 0;
							}
							else
							{
								copyRange.Width = rx - copyRange.X;
							}

							megaCopyStatus = TMegaCopyStatus.Selected;
						}
						break;
				}
			}
		}

		public void I_fnMouseMove(object sender, MouseEventArgs e)
		{
			int rx = 0;
			int ry = 0;

			if (SpeedButtonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case TMegaCopyStatus.Selecting:
						{
							if (e.X < 0 || e.X >= I_fn.Width || e.Y < 0 || e.Y >= I_fn.Height)
							{
								return;
							}

							rx = e.X / 16;
							ry = e.Y / 16;

							int origWidth = Shape1.Width;
							int origHeight = Shape1.Height;

							int w = 20;
							int h = 20;
							var temp = (rx - copyRange.X + 1) * 16 + 4;
							if (temp >= 20)
								w = temp;

							temp = (ry - copyRange.Y + 1) * 16 + 4;
							if (temp >= 20)
								h = temp;

							if (w != origWidth || h != origHeight)
							{
								Shape1.Size = new Size(w, h);
							}
						}
						break;

					case TMegaCopyStatus.Pasting:
						{
							if (!IsMousePositionValidForPasting(e.X, e.Y))
							{
								Shape2.Visible = false;
								ImageMegacopy.Visible = false;
								return;
							}

							Shape2.Left = I_fn.Left + e.X - e.X % 16 - 2;
							Shape2.Top = I_fn.Top + e.Y - e.Y % 16 - 2;
							ImageMegacopy.Left = Shape2.Left + 2;
							ImageMegacopy.Top = Shape2.Top + 2;
							Shape2.Visible = true;
							ImageMegacopy.Visible = true;
							Shape2v.Visible = false;
							ImageMegaCopyV.Visible = false;
						}
						break;
				}
			}
		}

		private static bool inShape1Resize = false;

		private void Shape1_Resize(object sender, EventArgs e)
		{
			if (inShape1Resize)
				return;
			inShape1Resize = true;

			var img = NewImage(Shape1);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(redBrush, new Rectangle(0, 0, img.Width, img.Height));
				Shape1.Region?.Dispose();
				Shape1.Size = new Size(img.Width, img.Height);

			}
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, Shape1.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(Shape1.Width - 2, 0, 2, Shape1.Height));
			graphicsPath.AddRectangle(new Rectangle(0, Shape1.Height - 2, Shape1.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, Shape1.Height));
			Shape1.Region = new Region(graphicsPath);

			inShape1Resize = false;
		}

		public void ImageMegaCopyMouseMove(object sender, MouseEventArgs e)
		{
			I_fnMouseMove(sender, new MouseEventArgs(MouseButtons.None, 0, e.X + ImageMegacopy.Left - I_fn.Left, e.Y + ImageMegacopy.Top - I_fn.Top, 0));
		}

		public void ImageMegaCopyMouseDown(object sender, MouseEventArgs e)
		{
			I_fnMouseDown(sender, new MouseEventArgs(e.Button, 0, e.X + ImageMegacopy.Left - I_fn.Left, e.Y + ImageMegacopy.Top - I_fn.Top, 0));
		}

		public void Shape2MouseDown(object sender, MouseEventArgs e)
		{
			I_fnMouseDown(null, new MouseEventArgs(e.Button, 0, Shape2.Left + e.X - I_fn.Left, Shape2.Top + e.Y - I_fn.Top, 0));
		}
		public void Shape2MouseMove(object sender, MouseEventArgs e)
		{
			I_fnMouseMove(null, new MouseEventArgs(e.Button, 0, Shape2.Left + e.X - I_fn.Left, Shape2.Top + e.Y - I_fn.Top, 0));
		}
		public void Shape2MouseUp(object sender, MouseEventArgs e)
		{
			I_fnMouseUp(null, new MouseEventArgs(e.Button, 0, Shape2.Left + e.X - I_fn.Left, Shape2.Top + e.Y - I_fn.Top, 0));
		}

		public void Shape1MouseDown(object sender, MouseEventArgs e)
		{
			I_fnMouseDown(sender, new MouseEventArgs(e.Button, 0, e.X + Shape1.Left - I_fn.Left, e.Y + Shape1.Top - I_fn.Top, 0));
		}
		public void Shape1MouseMove(object sender, MouseEventArgs e)
		{
			I_fnMouseMove(sender, new MouseEventArgs(e.Button, 0, e.X + Shape1.Left - I_fn.Left, e.Y + Shape1.Top - I_fn.Top, 0));
		}
		public void Shape1MouseLeave(object sender, EventArgs e)
		{
			Shape2.Visible = false;
			ImageMegacopy.Visible = false;
		}
		public void Shape1MouseUp(object sender, MouseEventArgs e)
		{
			I_fnMouseUp(sender, new MouseEventArgs(e.Button, 0, e.X + Shape1.Left - I_fn.Left, e.Y + Shape1.Top - I_fn.Top, 0));
		}
	}
}
