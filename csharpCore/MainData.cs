namespace FontMaker
{
	public class MainData { }

	public class AtrViewInfoJSON
	{
		public string Version { get; set; }
		/// <summary>
		/// Mode 4 indicator
		/// 1 = mode 4
		/// </summary>
		public string ColoredGfx { get; set; }

		/// <summary>
		/// Characters in the view
		/// </summary>
		public string Chars { get; set; }

		public string Lines { get; set; }

		public string Colors { get; set; }

		public string Fontname1 { get; set; }
		public string Fontname2 { get; set; }
		public string Fontname3 { get; set; }
		public string Fontname4 { get; set; }

		public string Data { get; set; }

		public string FortyBytes { get; set; }

		public List<SavedPageData> Pages { get; set; }
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
		internal byte[] chsline = new byte[Constants.VIEW_HEIGHT];
		internal string pathf = string.Empty;

		internal string fname1 = string.Empty;
		internal string fname2 = string.Empty;
		internal string fname3 = string.Empty;
		internal string fname4 = string.Empty;

		internal bool gfx = false;
		internal int activeColorNr;
		internal byte[] cpal = new byte[6];
		internal SolidBrush[] cpalBrushes = new SolidBrush[6];

		internal int selectedCharacterIndex = 0;			// The current character 0 - 511
		internal int duplicateCharacterIndex = 0;
		internal byte[,] vw = new byte[Constants.VIEW_WIDTH, Constants.VIEW_HEIGHT];
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

		public byte[] ft = new byte[Constants.NUM_FONT_BYTES];

		public byte[,] undoBuffer = new byte[UNDOBUFFERSIZE + 1, Constants.NUM_FONT_BYTES];

		public int[] undoBufferFlags = new int[UNDOBUFFERSIZE + 1];
		public int undoBufferIndex = 0;

		/// <summary>
		/// This is the bitmap that contains all 4 fonts draw in 2x2 pixel size
		/// </summary>
		public Bitmap bmpFontBanks = new Bitmap(512, 512);


		/// <summary>
		/// Load the default "laoo.act" color palette
		/// </summary>
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

		/// <summary>
		/// Init some default colors into the app's color selection boxes
		/// </summary>
		public void SetupDefaultPalColors()
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

			src.Image = new Bitmap(w, h);

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
