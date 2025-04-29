namespace FontMaker
{

	public class AtariViewUndoInfo
	{
		public byte[,] ViewBytes;
		public byte[] UseFontOnLine;

		public int Width { get; set; }
		public int Height { get; set; }

		public int OffsetX { get; set; }
		public int OffsetY { get; set; }
	}

	public static class AtariView
	{
		public const int VIEW_WIDTH_LIMIT = 40;
		public const int VIEW_HEIGHT_LIMIT = 26;

		public const int VIEW_WIDTH = 40;
		public const int VIEW_HEIGHT = 26;
		public const int VIEW_HEIGHT_TALL = 13;

		public const int VIEW_PIXEL_WIDTH = VIEW_WIDTH * 8 * 2;     // Each char is 8x8 and the x2 is for the double scale factor
		public const int VIEW_PIXEL_HEIGHT = VIEW_HEIGHT * 8 * 2;

		#region Data

		/// <summary>
		/// All bytes showing on the 40x26 screen
		/// </summary>
		public static byte[,] ViewBytes = new byte[VIEW_WIDTH, VIEW_HEIGHT];

		/// <summary>
		/// Which fontNr is used on which line of the view
		/// </summary>
		public static byte[] UseFontOnLine = new byte[VIEW_HEIGHT];

		public static int Width { get; set; } = VIEW_WIDTH;
		public static int Height { get; set; } = VIEW_HEIGHT;

		public static int OffsetX { get; set; } = 0;
		public static int OffsetY { get; set; } = 0;

		#endregion

		public static AtariViewUndoInfo BuildForUndo()
		{
			return new AtariViewUndoInfo()
			{
				ViewBytes = ViewBytes.Clone() as byte[,],
				UseFontOnLine = UseFontOnLine.Clone() as byte[],
				Width = Width,
				Height = Height,
				OffsetX = OffsetX,
				OffsetY = OffsetY,
			};
		}

		public static void RestoreFromUndo(AtariViewUndoInfo undoInfo)
		{
			ViewBytes = undoInfo.ViewBytes.Clone() as byte[,];
			UseFontOnLine = undoInfo.UseFontOnLine.Clone() as byte[];
			Width = undoInfo.Width;
			Height = undoInfo.Height;
			OffsetX = undoInfo.OffsetX;
			OffsetY = undoInfo.OffsetY;
		}

		public static void Setup()
		{
			for (var i = 0; i < UseFontOnLine.Length; i++)
			{
				UseFontOnLine[i] = 1;
			}
		}

		public static void ForcedResize(int width, int height)
		{
			Width = width;
			Height = height;
			ViewBytes = new byte[Width, Height];
			UseFontOnLine = new byte[Height];

			OffsetX = 0;
			OffsetY = 0;

			Setup();
		}

		public static void Resize(int width, int height, byte fontNr)
		{
			// Change the size of the view to the new size
			var newViewBytes = new byte[width, height];
			var newUseFontOnLine = new byte[height];

			for (var y = 0; y < height; ++y)
				newUseFontOnLine[y] = fontNr;

			// Copy as much as possible of the old view into the new view
			for (var y = 0; y < height && y < Height; ++y)
			{
				for (var x = 0; x < width && x < Width; ++x)
				{
					newViewBytes[x, y] = ViewBytes[x, y];
				}
				newUseFontOnLine[y] = UseFontOnLine[y];
			}

			// Set the new view
			ViewBytes = newViewBytes;
			UseFontOnLine = newUseFontOnLine;
			Width = width;
			Height = height;

			if (OffsetX+40 >= width)
				OffsetX = width-40;
			if (OffsetY + 26 >= height)
				OffsetY = height - 26;
		}

		public static void Load(string lineTypes, string characterBytes, int viewWidth)
		{
			var useFontOnLine = Convert.FromHexString(lineTypes);
			for (var i = 0; i < useFontOnLine.Length; i++)
			{
				UseFontOnLine[i] = useFontOnLine[i];
				// If OLD format issues
				// Font numbers 1-4
				if (UseFontOnLine[i] == 0)
					++UseFontOnLine[i];
			}

			var bytes = Convert.FromHexString(characterBytes);
			var idx = 0;
			for (var y = 0; y < Height; ++y)
			{
				for (var x = 0; x < viewWidth; ++x)
				{
					ViewBytes[x, y] = bytes[idx];
					++idx;
				}
			}
		}
	}
}
