namespace FontMaker
{
	public partial class TAtariColorSelectorForm : Form
	{
		public TAtariColorSelectorForm()
		{
			InitializeComponent();

			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(FormClose!);
			this.Load += new System.EventHandler(FormCreate!);
		}

		public void FormCreate(object Sender, EventArgs e)
		{
			DoubleBuffered = true;
			//  LoadPalette();
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

		public void ImagePaletteMouseMove(object Sender, MouseEventArgs e)
		{
			if ((e.X < 16 * 8) && (e.Y < 16 * 16))
			{
				actualColorIndex = (byte)((e.Y / 16) * 16 + (e.X / 16) * 2);
				actualColor = palette[actualColorIndex];
				DrawActualColor();
			}
		}

		public void ImagePaletteMouseDown(object Sender, MouseEventArgs e)
		{
			selectedColorIndex = actualColorIndex;
			this.Close();
		}



		public void FormShow(object Sender, EventArgs e)
		{
			DrawPalette();
			DrawActualColor();
		}



		public void SetSelectedColorIndex(byte paletteIndex)
		{
			selectedColorIndex = paletteIndex;
		}



		public void FormClose(object Sender, FormClosedEventArgs Action)
		{
			if (this.DialogResult == DialogResult.OK)
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



		public Color GetColor(byte paletteIndex)
		{
			Color getColor_result = Color.Empty;
			getColor_result = palette[paletteIndex];
			return getColor_result;
		}



		public Color[] GetMyPalette()
		{
			Color[] getMyPalette_result = new Color[256];
			getMyPalette_result = palette;
			return getMyPalette_result;
		}
		internal Color[] palette = new Color[256];
		public byte selectedColorIndex;
		public byte actualColorIndex;
		public Color actualColor = Color.Empty;

		private void TAtariColorSelectorForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				Close();
			}

		}
	}
}
