#pragma warning disable WFO1000

using System.Drawing;

namespace FontMaker
{
	// Section C - Color management
	internal class Colors
	{
	}

	public class ColorModesDef
	{
		public int Key { get; set; }
		public string Value { get; set; }
	}
	
	public partial class FontMakerForm
	{
		public List<ColorModesDef> ColorModesList =
		[
			new ColorModesDef() { Key = 4, Value = "Mode 4 (5 cols)" },
			new ColorModesDef() { Key = 5, Value = "Mode 5 (5 cols)" },
			new ColorModesDef() { Key = 10, Value = "Mode 10 (9 cols)" },
		];

		/// <summary>
		/// Are we showing one of the color switchers
		/// </summary>
		public bool ShowColorSwitcher { get; set; }

		public List<string> ColorSets { get; set; } = [];

		/// <summary>
		/// The index in the combo box that is currently selected
		/// </summary>
		public int CurrentColorSetIndex { get; set; } = -1;

		/// <summary>
		/// Which color mode is to be used?
		/// 4 - 5 colors (4x8 pixels per char)
		/// 5 - 5 colors / tall pixels (4x8 pixels per char)
		/// 10 - 9 colors / double color clock (2x8 pixels per char)
		/// </summary>
		private int _whichColorMode = 4;
		public int WhichColorMode { get => _whichColorMode;
			set
			{
				_whichColorMode = value;
				AtariFontRenderer.WhichColorMode = value;
			}
		}

		public bool InColorSetSetup { get; set; }


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

		/// <summary>
		/// Switch the GUI into the correct mode
		/// 0 - B/W
		/// 1 - mode 4
		/// 2 - mode 5
		/// 3 - mode 10
		/// </summary>
		/// <param name="targetMode"></param>
		private void SetupColorMode(int targetMode)
		{
			if (targetMode == 0 && !InColorMode)
				return;		// Already in B/W, nothing to do

			if (targetMode == 0 && InColorMode)
			{
				// Switch to B/W mode, ignore what the color mode setting is!
				SwitchGfxMode();
				return;
			}

			// Target of B/W is done.
			// Can only be color now!

			// Switch into the correct color mode: 4,5,10
			InColorSetSetup = true;
			switch (targetMode)
			{
				default:
					cmbColorMode.SelectedValue = 4;
					break;
				case 2:
					cmbColorMode.SelectedValue = 5;
					break;
				case 3:
					cmbColorMode.SelectedValue = 10;
					break;
			}

			WhichColorMode = (int)cmbColorMode.SelectedValue;
			InColorSetSetup = false;

			if (!InColorMode)
			{
				// Switch from B/W to the correct color mode
				SwitchGfxMode();
			}
			else
			{
				// Change into the correct color mode
				ColorMode_Change();
			}
		}

		private int WhatColorModeToSave()
		{
			if (!InColorMode)
				return 0;
			switch (WhichColorMode)
			{
				case 4:
				default:
					return 1;
				case 5:
					return 2;
				case 10:
					return 3;
			}
		}

		/// <summary>
		/// Switch graphics mode
		/// B/W -> Color -> B/W
		/// </summary>
		private void SwitchGfxMode()
		{
			// Toggle between B/W and color mode
			InColorMode = !InColorMode;

			// Switch into the correct color mode
			ColorMode_Change();

			RedrawView();

			// Now simulate a mouse click in the font selector. The idea is to get the
			// color character into the edit area.
			// DANGER: When in MegaCopy mode this would paste the copy area into the font.
			// Hence, turn off the MegaCopy mode, do the simulated click and then restore the MegaCopy mode
			var fontRubberBandVisible = pictureBoxFontSelectorRubberBand.Visible;
			var viewRubberBandVisible = pictureBoxViewEditorRubberBand.Visible;
			var savedRange = CopyPasteRange;
			SimulateSafeLeftMouseButtonClick();
			pictureBoxViewEditorRubberBand.Visible = viewRubberBandVisible;
			pictureBoxFontSelectorRubberBand.Visible = fontRubberBandVisible;
			CopyPasteRange = savedRange;
			
			if (!buttonMegaCopy.Checked)
			{
				RevalidateCharButtons();
			}
			else
			{
				// Make sure the copy area is redrawn
				RevalidateClipboard();
			}

			if (!InColorMode && ShowColorSwitcher)
			{
				ShowColorSwitchSetup_Click(null!, EventArgs.Empty);
			}

			ConfigureClipboardActionButtons();
		}

		private void SwitchFontBank()
		{
			FontBank_CheckedChanged(0, EventArgs.Empty);
			ShowCorrectFontBank();

			// Now simulate a mouse click in the font selector. The idea is to get the
			// color character into the edit area.
			// DANGER: When in MegaCopy mode this would paste the copy area into the font.
			// Hence, turn off the MegaCopy mode, do the simulated click and then restore the MegaCopy mode
			SimulateSafeLeftMouseButtonClick();

			CheckDuplicate();
		}

		#region Recolor actions
		public void RedrawRecolorSource()
		{
			var img = Helpers.GetImage(pictureBoxRecolorSourceColor);
			using (var gr = Graphics.FromImage(img))
			{
				FillColorSelectorBackground(gr, listBoxRecolorSource.SelectedIndex + 1);
				DrawColorLabelsEx(gr, 1, 2, listBoxRecolorSource.SelectedIndex + 2, listBoxRecolorSource.SelectedIndex + 1);
			}

			pictureBoxRecolorSourceColor.Refresh();
		}

		public void RedrawRecolorTarget()
		{
			var img = Helpers.GetImage(pictureBoxRecolorTargetColor);
			using (var gr = Graphics.FromImage(img))
			{
				FillColorSelectorBackground(gr, listBoxRecolorTarget.SelectedIndex + 1);
				DrawColorLabelsEx(gr, 1, 2, listBoxRecolorTarget.SelectedIndex + 2, listBoxRecolorTarget.SelectedIndex + 1);
			}

			pictureBoxRecolorTargetColor.Refresh();
		}


		public void RedrawRecolorMode10Source()
		{
			var idx = listBoxRecolorSourceMode10.SelectedIndex;
			var img = Helpers.GetImage(pictureBoxRecolorSourceColorMode10);
			using var gr = Graphics.FromImage(img);
			var brush = BrushCache[listBoxRecolorSourceMode10.SelectedIndex + 1];
			gr.FillRectangle(brush, 0, 0, 49, 17);
			gr.DrawString($"Color {idx}", this.Font, brush.Color.G > 128 ? BlackBrush : WhiteBrush, 1, 2);

			pictureBoxRecolorSourceColorMode10.Refresh();
		}

		public void RedrawRecolorMode10Target()
		{
			var idx = listBoxRecolorTargetMode10.SelectedIndex;
			var img = Helpers.GetImage(pictureBoxRecolorTargetColorMode10);
			using var gr = Graphics.FromImage(img);
			var brush = BrushCache[idx + 1];
			gr.FillRectangle(brush, 0, 0, 49, 17);
			gr.DrawString($"Color {idx}", this.Font, brush.Color.G > 128 ? BlackBrush : WhiteBrush, 1, 2);

			pictureBoxRecolorTargetColorMode10.Refresh();
		}
		#endregion

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

				var numLinesOfColors = WhichColorMode == 10 ? 5 : 3;
				for (var a = 0; a < numLinesOfColors; a++)
				{
					for (var b = 0; b < 2; b++)
					{
						gr.FillRectangle(BrushCache[b + a * 2], b * 45, a * 18, 45, 22);
						DrawColorLabels(gr, b * 45, a * 18 + 2, b + a * 2, b + a * 2);
					}
				}
			}

			pictureBoxPalette.Refresh();

			img = Helpers.GetImage(pictureBoxCharacterEditorColor1);
			using (var gr = Graphics.FromImage(img))
			{
				var tagVal = (int)pictureBoxCharacterEditorColor1.Tag;
				FillColorSelectorBackground(gr, tagVal);
				DrawColorLabelsEx(gr, 1, 2, tagVal, tagVal);
			}

			pictureBoxCharacterEditorColor1.Refresh();

			img = Helpers.GetImage(pictureBoxCharacterEditorColor2);
			using (var gr = Graphics.FromImage(img))
			{
				var tagVal = (int)pictureBoxCharacterEditorColor2.Tag;
				FillColorSelectorBackground(gr, tagVal);
				DrawColorLabelsEx(gr, 1, 2, tagVal, tagVal);
			}

			pictureBoxCharacterEditorColor2.Refresh();

			img = Helpers.GetImage(pictureBoxActionColor);
			using (var gr = Graphics.FromImage(img))
			{
				FillColorSelectorBackground(gr, ActiveColorNr);
				DrawColorLabelsEx(gr, 1, 2, ActiveColorNr, ActiveColorNr);
			}

			pictureBoxActionColor.Refresh();
		}

		/// <summary>
		/// There are two color selection boxes and one active color indicator.
		/// This functions sets the background color of the graphics object.
		/// In the case of color 3 (which is index 4 in the palette) draw the
		/// two possible colors. 
		/// </summary>
		/// <param name="gr"></param>
		/// <param name="colorNr"></param>
		private void FillColorSelectorBackground(Graphics gr, int colorNr)
		{
			if (colorNr == 4)
			{
				// Special case for color 3.
				// In graphics mode it can be one of two colors.
				// The color depends on the index of the character >=128 then its the alternative color
				var brush = BrushCache[colorNr];
				gr.FillRectangle(brush, 0, 0, 25, 17);
				brush = BrushCache[colorNr + 1];
				gr.FillRectangle(brush, 25, 0, 49, 17);
			}
			else
			{
				var brush = BrushCache[colorNr];
				gr.FillRectangle(brush, 0, 0, 49, 17);
			}
		}

		/// <summary>
		/// Draw the palette name into the color box.
		/// </summary>
		/// <param name="ic">Graphics object to draw into</param>
		/// <param name="x">X offset of text in box</param>
		/// <param name="y">Y offset of text in box</param>
		/// <param name="num">Palette entry</param>
		/// <param name="color"></param>
		private static string[] labels = [
			"LUM", "BAK - 00", 
			"PF0 - 01", "PF1 - 10", 
			"PF2 - 11", "PF3 - 11",
			"", "",
			"", "",
		];
		private static string[] mode10Labels = [
			"LUM", "0",
			"1", "2",
			"3", "4",
			"5", "6",
			"7", "8",
		];

		public void DrawColorLabels(Graphics ic, int x, int y, int num, int colorNr)
		{
			var color = AtariPalette[SetOfSelectedColors[colorNr]];
			ic.DrawString(WhichColorMode == 10 ? mode10Labels[num] : labels[num], this.Font, color.G > 128 ? BlackBrush : WhiteBrush, x, y);
		}

		public void DrawColorLabelsEx(Graphics ic, int x, int y, int num, int colorNr)
		{
			if (colorNr == 4)
			{
				// PF2 - 11
				// Special processing for this color. It has two colors allocated to it so the string needs to be split in two
				// 4 chars and then the rest
				var leftText = "PF2 ";
				var color = AtariPalette[SetOfSelectedColors[colorNr]];
				ic.DrawString(leftText, this.Font, color.G > 128 ? BlackBrush : WhiteBrush, x, y);

				var sz = ic.MeasureString(leftText, this.Font);

				var rightText = "- 11";
				color = AtariPalette[SetOfSelectedColors[colorNr+1]];
				ic.DrawString(rightText, this.Font, color.G > 128 ? BlackBrush : WhiteBrush, x + sz.Width, y);

			}
			else
			{
				// Draw the palette entry name into the graphic using ONE color
				var color = AtariPalette[SetOfSelectedColors[colorNr]];
				ic.DrawString(labels[num], this.Font, color.G > 128 ? BlackBrush : WhiteBrush, x, y);
			}
		}

		/// <summary>
		/// Mouse down in the color palette box
		/// </summary>
		/// <param name="e"></param>
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
						break;
					}

					case 1:
					{
						SetOfSelectedColors[1] = AtariColorSelector.selectedColorIndex;
						UpdateBrushCache(1);
						SetOfSelectedColors[0] = (byte)((SetOfSelectedColors[1] / 16) * 16 + SetOfSelectedColors[0] % 16);
						UpdateBrushCache(0);
						break;
					}

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
			SaveColorSet();
		}


		public void ColorSwitch2Bit(int idx1, int idx2)
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

		public void ColorSwitch4Bit(int idx1, int idx2)
		{
			var src = AtariFont.Get4BitColorCharacter(SelectedCharacterIndex, checkBoxFontBank.Checked);

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

			AtariFont.Set4BitCharacter(src, SelectedCharacterIndex, checkBoxFontBank.Checked);

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
				BrushCache[i]?.Dispose();
				BrushCache[i] = new SolidBrush(AtariPalette[SetOfSelectedColors[i]]);
			}

			EmptyBrush ??= new SolidBrush(Color.FromKnownColor(KnownColor.DarkGray));

			AtariFontRenderer.RebuildPalette(SetOfSelectedColors);
		}

		public void UpdateBrushCache(int index)
		{
			if (index < 0 || index >= BrushCache.Length)
				throw new NotImplementedException("Oi something is wrong in UpdateBrushCache");

			BrushCache[index]?.Dispose();
			BrushCache[index] = new SolidBrush(AtariPalette[SetOfSelectedColors[index]]);
		}

		public void BuildColorModeList()
		{
			InColorSetSetup = true;
			cmbColorMode.Items.Clear();
			cmbColorMode.ResetText();

			cmbColorMode.DataSource = ColorModesList;
			cmbColorMode.ValueMember = "Key";
			cmbColorMode.DisplayMember = "Value";

			cmbColorMode.SelectedValue = 4;
			WhichColorMode = (int)cmbColorMode.SelectedValue;
			InColorSetSetup = false;
		}

		public void BuildColorSetList()
		{
			InColorSetSetup = true;

			comboBoxColorSets.Items.Clear();
			comboBoxColorSets.ResetText();

			for (var idx = 0; idx < ColorSets.Count; ++idx)
			{
				comboBoxColorSets.Items.Add(idx == 0 ? "Project colors": $"Alt colors {idx}");
			}

			comboBoxColorSets.SelectedIndex = 0;
			CurrentColorSetIndex = comboBoxColorSets.SelectedIndex;

			InColorSetSetup = false;
		}

		public void SetPrimaryColorSetData()
		{
			ColorSets[0] = Convert.ToHexString(SetOfSelectedColors);
		}

		private void SwopColorSet(bool saveCurrent)
		{
			var nextColorSetIndex = comboBoxColorSets.SelectedIndex;
			if (nextColorSetIndex == -1) return;

			if (saveCurrent)
			{
				// Save the current color data
				var currentColors = Convert.ToHexString(SetOfSelectedColors);
				ColorSets[CurrentColorSetIndex] = currentColors;
			}

			SwopColorSetAction(nextColorSetIndex);
		}

		private void SwopColorSetAction(int nextColorSetIndex)
		{
			var colors = ColorSets[nextColorSetIndex];

			// Load the AtariPalette selection
			SetOfSelectedColors = Convert.FromHexString(FixColorHexString(colors));
			BuildBrushCache();

			CurrentColorSetIndex = nextColorSetIndex;

			RedrawPal();
			RedrawFonts();
			RedrawChar();
			RedrawView();
			
			RedrawRecolorPanel();
		}

		private void SaveColorSet()
		{
			// Save the current color data
			ColorSets[CurrentColorSetIndex] = Convert.ToHexString(SetOfSelectedColors);
		}


		private void ColorMode_Change()
		{
			InMode5 = false;

			if (!InColorMode)
			{
				RedrawLineTypes();
				ShowCorrectFontBank();
				ShowColorSelectors();
				return;
			}

			WhichColorMode = (int)(cmbColorMode.SelectedValue ?? 4);		// Fallback to mode 4
			RedrawPal();

			InMode5 = WhichColorMode == 5;
			AtariFontRenderer.RebuildFontCache(WhichColorMode);

			RedrawLineTypes();
			ShowCorrectFontBank();
			RedrawView();
			RedrawChar();

			ShowColorSelectors();
			if (ShowColorSwitcher)
			{
				// Make sure that the correct color switcher is shown
				ShowColorSwitchSetup_Click(null!, EventArgs.Empty);
				ShowColorSwitchSetup_Click(null!, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Which color selectors:
		/// B/W, Mode 4 & 5 => 3x picture box
		/// Mode 10 => drop down list
		/// </summary>
		public void ShowColorSelectors()
		{
			var inManyColorMode = InColorMode && WhichColorMode == 10;

			pictureBoxActionColor.Visible = !inManyColorMode;
			pictureBoxCharacterEditorColor1.Visible = !inManyColorMode;
			pictureBoxCharacterEditorColor2.Visible = !inManyColorMode;
			cmbColor9Menu.Visible = inManyColorMode;
			if (inManyColorMode)
				cmbColor9Menu.Refresh();
		}

		public string FixColorHexString(string inputHexString)
		{
			if (inputHexString.Length == 12)
			{
				return $"{inputHexString}161AB4BA";
			}
			return inputHexString;
		}
	}
}
