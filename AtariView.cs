namespace FontMaker
{
	public static class AtariView
	{
		public const int VIEW_WIDTH = 40;
		public const int VIEW_HEIGHT = 26;
		public const int VIEW_HEIGHT_TALL = 13;

		#region Data

		/// <summary>
		/// All bytes showing on the 40x26 screen
		/// </summary>
		public static byte[,] ViewBytes = new byte[VIEW_WIDTH, VIEW_HEIGHT];

		/// <summary>
		/// Which fontNr is used on which line of the view
		/// </summary>
		public static byte[] UseFontOnLine = new byte[VIEW_HEIGHT];

		#endregion

		public static void Setup()
		{
			for (var a = 0; a < AtariView.VIEW_HEIGHT; a++)
			{
				UseFontOnLine[a] = 1;
			}
		}
	}
}
