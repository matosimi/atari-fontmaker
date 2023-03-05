namespace FontMaker
{
	public class MainData { }

	public class AtrViewInfoJSON
	{
		public string Version { get; set; }
		public string ColoredGfx { get; set; }
		public string Chars { get; set; }

		public string Lines { get; set; }

		public string Colors { get; set; }

		public string Fontname1 { get; set; }

		public string Fontname2 { get; set; }

		public string Data { get; set; }

		public string FortyBytes { get; set; }

	}

	public class ClipboardJSON
	{
		public string Width { get; set; }
		public string Height { get; set; }
		public string Chars { get; set; }
		public string Data { get; set; }
	}

	public partial class TMainForm
	{
		public static readonly int VIEW_WIDTH = 40;
		public static readonly int VIEW_HEIGHT = 26;
		internal byte[] chsline = new byte[VIEW_HEIGHT - 1 + 1];
		internal string pathf = string.Empty;
		internal string fname1 = string.Empty;
		internal string fname2 = string.Empty;
		internal bool gfx = false;
		internal int ac;
		internal byte[] cpal = new byte[6];
		internal SolidBrush[] cpalBrushes = new SolidBrush[6];

		internal int selectedCharacterIndex = 0;			// The current character 0 - 511
		internal int duplicateCharacterIndex = 0;
		internal byte[,] vw = new byte[VIEW_WIDTH - 1 + 1, VIEW_HEIGHT - 1 + 1];
		internal int clx = 0;
		internal int cly = 0;
		internal int clxv = 0;
		internal int clyv = 0;
		internal bool clck = false;
		internal MouseButtons ButtonHeld;
		internal bool clckv = false;
		internal string clipboardLocal = string.Empty;
		internal TMegaCopyStatus megaCopyStatus = new TMegaCopyStatus();

		internal Color[] palette = new Color[256];

		internal Rectangle copyRange = new Rectangle();
		internal Point copyTarget { get; set; } = new Point();

		internal string fortyBytes = string.Empty;
		public static readonly byte[] colorIndex2bits = new byte[] { 0, 0, 1, 2, 3, 3 };
		public static readonly byte[] bits2colorIndex = new byte[] { 1, 2, 3, 4 };
		public static readonly int UNDOBUFFERSIZE = 250;
		public byte[] ft = new byte[2048];
		public byte[,] undoBuffer = new byte[UNDOBUFFERSIZE + 1, 2048];
		public int[] undoBufferFlags = new int[UNDOBUFFERSIZE + 1];
		public int undoBufferIndex = 0;


		public Font myFont = new System.Drawing.Font("Microsoft Sans Serif", 8, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);


		public void LoadPalette()
		{
			var buffer = File.ReadAllBytes("laoo.act");
			if (buffer.Length < 768)
				MessageBox.Show("Palette load error");

			// Make Color values out of the bytes
			for (var i = 0; i < 256; ++i)
			{
				palette[i] = Color.FromArgb(buffer[i * 3], buffer[i * 3 + 1], buffer[i * 3 + 2]);
			}
			AtariColorSelectorUnit.AtariColorSelectorForm.SetPalette(palette);
		}

		public void default_pal()
		{
			cpal[0] = 14;
			cpal[1] = 0;
			cpal[2] = 40;
			cpal[3] = 202;
			cpal[4] = 148;
			cpal[5] = 70;

			BuildBrushCache();
		}

		public void BuildBrushCache()
		{
			// Create the solid brushes to use for drawing the GUI and bitmaps
			for (var i = 0; i < cpal.Length; ++i)
			{
				cpalBrushes[i] = new SolidBrush(palette[cpal[i]]);
			}
		}

		public void UpdateBrushCache(int index)
		{
			if (index < 0 || index >= cpalBrushes.Length)
				throw new NotImplementedException("Oi something is wrong in UpdateBrushCache");

			cpalBrushes[index] = new SolidBrush(palette[cpal[index]]);
		}

		public static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
		{
			using (Graphics grD = Graphics.FromImage(destBitmap))
			{
				grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
			}
		}

		public Image GetImage(PictureBox src)
		{
			var w = src.Bounds.Width;
			var h = src.Bounds.Height;

			if (src.Image != null)
				return src.Image;

			var matrix = new Bitmap(w, h);
			src.Image = matrix;

			return src.Image;
		}

		public Image NewImage(PictureBox src)
		{
			var w = src.Bounds.Width;
			var h = src.Bounds.Height;

			if (src.Image != null) 
				src.Image.Dispose();

			src.Image = new Bitmap(w, h);

			return src.Image;
		}

		public void Exit()
		{
			Visible = false;
			Timer1.Enabled = false;
			i_abo.Visible = false;
			Environment.Exit(Environment.ExitCode);
		}
	}
}
