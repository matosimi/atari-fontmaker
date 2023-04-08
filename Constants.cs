namespace FontMaker
{
	internal static class Constants
	{
		public static readonly string Title = "Atari FontMaker";

		public static Rectangle RectFontBank12 = new Rectangle(0, 0, 512, 256);
		public static Rectangle RectFontBank34 = new Rectangle(0, 256, 512, 512);

		public static Rectangle[] WhereAreTheFontBanksComingFrom = new Rectangle[4]
		{
			new Rectangle(0, 0, 512, 256),
			new Rectangle(0, 256, 512, 512),
			new Rectangle(0, 512, 512, 768),
			new Rectangle(0, 768, 512, 1024),
		};

		public static int[] FontYOffset = new int[4] { 0, 128, 256, 384 };

		public static int[] FontPageOffset = new int[4] { 0, 0, 256, 256 };

		public static readonly byte[] ColorIndex2Bits = new byte[] { 0, 0, 1, 2, 3, 3 };
		public static readonly byte[] Bits2ColorIndex = new byte[] { 1, 2, 3, 4 };


	}
}
