namespace FontMaker
{
	// Section C - Color management
	internal class Colors
	{
	}

	public partial class FontMakerForm
	{

		/// <summary>
		/// Load the color AtariPalette 
		/// </summary>
		public void LoadPalette()
		{
			// Get the embedded resource AtariPalette as byte[] 
			var buffer = Helpers.GetResource<byte[]>("altirraPAL.pal");
			// Make Color values out of the bytes
			for (var i = 0; i < 256; ++i)
			{
				AtariPalette[i] = Color.FromArgb(buffer[i * 3], buffer[i * 3 + 1], buffer[i * 3 + 2]);
			}
			AtariColorSelector.SetPalette(AtariPalette);
			AtariFontRenderer.SetPalette(AtariPalette);
		}


		private void SwitchGfxMode()
		{
			InColorMode = !InColorMode;
			RedrawFonts();
			RedrawView();
			ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 0, (SelectedCharacterIndex % 32) * 16, (SelectedCharacterIndex / 32) * 16, 0));

			if (!buttonMegaCopy.Checked)
			{
				RevalidateCharButtons();
			}

			if (!InColorMode && panelColorSwitcher.Visible)
			{
				ShowColorSwitchSetup_Click(null!, EventArgs.Empty);
			}
		}


		public void RedrawRecolorSource()
		{
			var img = Helpers.GetImage(pictureBoxRecolorSourceColor);
			using (var gr = Graphics.FromImage(img))
			{
				var brush = BrushCache[listBoxRecolorSource.SelectedIndex + 1];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				DrawColorLabels(gr, 1, 1, listBoxRecolorSource.SelectedIndex + 2, AtariPalette[SetOfSelectedColors[listBoxRecolorSource.SelectedIndex + 1]]);
			}
			pictureBoxRecolorSourceColor.Refresh();
		}

		public void RedrawRecolorTarget()
		{
			var img = Helpers.GetImage(pictureBoxRecolorTargetColor);
			using (var gr = Graphics.FromImage(img))
			{
				var brush = BrushCache[listBoxRecolorTarget.SelectedIndex + 1];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				DrawColorLabels(gr, 1, 1, listBoxRecolorTarget.SelectedIndex + 2, AtariPalette[SetOfSelectedColors[listBoxRecolorTarget.SelectedIndex + 1]]);
			}
			pictureBoxRecolorTargetColor.Refresh();
		}

		public void RedrawPal()
		{
			var img = Helpers.GetImage(pictureBoxPalette);
			using (var gr = Graphics.FromImage(img))
			{
				gr.Clear(this.BackColor);

				// Check that the SetOfSelectedColors[0] color is correct
				var new0 = (byte)((SetOfSelectedColors[1] / 16) * 16 + SetOfSelectedColors[0] % 16);
				if (new0 != SetOfSelectedColors[0])
				{
					SetOfSelectedColors[0] = new0;
					UpdateBrushCache(0);
				}

				for (var a = 0; a < 3; a++)
				{
					for (var b = 0; b < 2; b++)
					{
						gr.FillRectangle(BrushCache[b + a * 2], b * 45, a * 18, 45, 22);
						DrawColorLabels(gr, b * 45, a * 18, b + a * 2, AtariPalette[SetOfSelectedColors[b + a * 2]]);
					}
				}
			}
			pictureBoxPalette.Refresh();

			img = Helpers.GetImage(pictureBoxCharacterEditorColor1);
			using (var gr = Graphics.FromImage(img))
			{
				var tagVal = (int)pictureBoxCharacterEditorColor1.Tag;
				var brush = BrushCache[tagVal];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				DrawColorLabels(gr, 1, 1, tagVal, AtariPalette[SetOfSelectedColors[tagVal]]);

			}
			pictureBoxCharacterEditorColor1.Refresh();

			img = Helpers.GetImage(pictureBoxCharacterEditorColor2);
			using (var gr = Graphics.FromImage(img))
			{
				var tagVal = (int)pictureBoxCharacterEditorColor2.Tag;
				var brush = BrushCache[tagVal];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				DrawColorLabels(gr, 1, 1, tagVal, AtariPalette[SetOfSelectedColors[tagVal]]);
			}
			pictureBoxCharacterEditorColor2.Refresh();

			img = Helpers.GetImage(pictureBoxActionColor);
			using (var gr = Graphics.FromImage(img))
			{
				var brush = BrushCache[ActiveColorNr];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				DrawColorLabels(gr, 1, 1, ActiveColorNr, AtariPalette[SetOfSelectedColors[ActiveColorNr]]);
			}
			pictureBoxActionColor.Refresh();
		}

		public void DrawColorLabels(Graphics ic, int x, int y, int num, Color color)
		{
			var labels = new[] { "LUM", "BAK - 00", "PF0 - 01", "PF1 - 10", "PF2 - 11", "PF3 - 11" };
			ic.DrawString(labels[num], this.Font, color.G > 128 ? blackBrush : whiteBrush, x, y);
		}

		public void InteractWithTheColorPalette(MouseEventArgs e)
		{
			if (Control.ModifierKeys == Keys.Shift)
			{
				var od = MessageBox.Show(@"Restore default colors?", Constants.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

				if (od == DialogResult.Yes)
				{
					SetupDefaultPalColors();
					RedrawPal();
					InColorMode = !InColorMode;
					SwitchGfxMode();
				}
			}
			else
			{
				var wh = e.X / 45 + (e.Y / 18) * 2;
				AtariColorSelector.SetSelectedColorIndex(SetOfSelectedColors[wh]);
				AtariColorSelector.ShowDialog();

				switch (wh)
				{
					case 0:
					{
						SetOfSelectedColors[0] = (byte)(AtariColorSelector.selectedColorIndex % 16 + (SetOfSelectedColors[1] / 16) * 16);
						UpdateBrushCache(0);
					}
						break;

					case 1:
					{
						SetOfSelectedColors[1] = AtariColorSelector.selectedColorIndex;
						UpdateBrushCache(1);
						SetOfSelectedColors[0] = (byte)((SetOfSelectedColors[1] / 16) * 16 + SetOfSelectedColors[0] % 16);
						UpdateBrushCache(0);
					}
						break;

					default:
						SetOfSelectedColors[wh] = AtariColorSelector.selectedColorIndex;
						UpdateBrushCache(wh);
						break;
				}
			}

			BuildBrushCache();

			RedrawPal();
			RedrawFonts();
			RedrawChar();
			RedrawView();
			RedrawRecolorPanel();
		}


		public void ColorSwitch(int idx1, int idx2)
		{
			var src = AtariFont.Get5ColorCharacter(SelectedCharacterIndex, checkBoxFontBank.Checked);

			for (var y = 0; y < 8; y++)
			{
				for (var x = 0; x < 4; x++)
				{
					if (src[x, y] == (byte)idx1)
					{
						src[x, y] = (byte)idx2;
					}
					else if (src[x, y] == idx2)
					{
						src[x, y] = (byte)idx1;
					}
				}
			}

			AtariFont.Set5ColorCharacter(src, SelectedCharacterIndex, checkBoxFontBank.Checked);

			DoChar();
			RedrawChar();
			RedrawView();
		}

		public void RedrawRecolorPanel()
		{
			RedrawRecolorSource();
			RedrawRecolorTarget();
		}

		/// <summary>
		/// Init some default colors into the app's color selection boxes
		/// </summary>
		public void SetupDefaultPalColors()
		{
			SetOfSelectedColors[0] = 14;
			SetOfSelectedColors[1] = 0;
			SetOfSelectedColors[2] = 40;
			SetOfSelectedColors[3] = 202;
			SetOfSelectedColors[4] = 148;
			SetOfSelectedColors[5] = 70;
			BuildBrushCache();
		}

		public void BuildBrushCache()
		{
			// Create the solid brushes to use for drawing the GUI and bitmaps
			for (var i = 0; i < SetOfSelectedColors.Length; ++i)
			{
				BrushCache[i] = new SolidBrush(AtariPalette[SetOfSelectedColors[i]]);
			}

			AtariFontRenderer.RebuildPalette(SetOfSelectedColors);
		}

		public void UpdateBrushCache(int index)
		{
			if (index < 0 || index >= BrushCache.Length)
				throw new NotImplementedException("Oi something is wrong in UpdateBrushCache");

			BrushCache[index] = new SolidBrush(AtariPalette[SetOfSelectedColors[index]]);
		}
	}
}
