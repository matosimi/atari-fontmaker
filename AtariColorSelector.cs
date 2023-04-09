﻿namespace FontMaker
{
	public partial class AtariColorSelectorForm : Form
	{
		public AtariColorSelectorForm()
		{
			InitializeComponent();

			this.FormClosed += FormClose!;
			this.Load += FormCreate!;
		}

		public void FormCreate(object sender, EventArgs e)
		{
			DoubleBuffered = true;
			DrawPalette();
			DrawActualColor();
		}

		public void DrawPalette()
		{
			var matrix = new Bitmap(128 + 16, 256 + 16);
			var gr = Graphics.FromImage(matrix);
			gr.Clear(this.BackColor);

			for (var y = 0; y < 16; y++)
			{
				// Vertical marks
				gr.DrawString($"{y:X}", this.Font, new SolidBrush(ForeColor), 16 * 8 + 2, y * 16);
				for (var x = 0; x < 8; x++)
					gr.FillRectangle(new SolidBrush(palette[y * 16 + x * 2]), x * 16, y * 16, 16, 16);
			}
			gr.DrawRectangle(new Pen(new SolidBrush(Color.White)), (selectedColorIndex % 16) * 8, (selectedColorIndex / 16) * 16, 15, 15);

			// Horizontal marks
			for (var x = 0; x < 8; x++)
				gr.DrawString($"{(x * 2):X}", this.Font, new SolidBrush(this.ForeColor), 16 * x, 16 * 16 + 2);
			// Clear the old image and set new bitmap
			ImagePalette.Image?.Dispose();
			ImagePalette.Image = matrix;

			gr.Dispose();

		}

		public void DrawActualColor()
		{
			var w = ImageSelected.Width;
			var h = ImageSelected.Height;
			LabelOldColor.Text = $@"${selectedColorIndex:X2} - {selectedColorIndex}";
			LabelActualColor.Text = $@"${actualColorIndex:X2} - {actualColorIndex}";
			var clr = new Bitmap(w, h);
			var gr = Graphics.FromImage(clr);
			gr.FillRectangle(new SolidBrush(palette[selectedColorIndex]), 0, 0, w, h / 2);
			gr.FillRectangle(new SolidBrush(palette[actualColorIndex]), 0, h / 2, w, h / 2);
			gr.Dispose();
			ImageSelected.Image?.Dispose();
			ImageSelected.Image = clr;
		}

		public void ImagePaletteMouseMove(object sender, MouseEventArgs e)
		{
			if (e.X < 16 * 8 && e.Y < 16 * 16)
			{
				actualColorIndex = (byte)((e.Y / 16) * 16 + (e.X / 16) * 2);
				actualColor = palette[actualColorIndex];
				DrawActualColor();
			}
		}

		public void ImagePaletteMouseDown(object sender, MouseEventArgs e)
		{
			selectedColorIndex = actualColorIndex;
			Close();
		}

		public void SetSelectedColorIndex(byte paletteIndex)
		{
			selectedColorIndex = paletteIndex;
		}

		public void FormClose(object sender, FormClosedEventArgs action)
		{
			if (DialogResult == DialogResult.OK)
			{
				selectedColorIndex = actualColorIndex;
			}
			else
			{
				actualColorIndex = selectedColorIndex;
			}
		}

		public void SetPalette(Color[] pal)
		{
			palette = pal;
		}

		internal Color[] palette;// = new Color[256];
		public byte selectedColorIndex;
		public byte actualColorIndex;
		public Color actualColor = Color.Empty;

		private void AtariColorSelectorForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				Close();
			}
		}
	}
}