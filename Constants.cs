namespace FontMaker
{
	internal static class Constants
	{
		public static readonly string Title = "Atari FontMaker";

		public static Rectangle[] WhereAreTheFontBanksComingFrom = new Rectangle[]
		{
			new(0, 0, 512, 256),
			new(0, 256, 512, 512),
			new(0, 512, 512, 768),
			new(0, 768, 512, 1024),
		};

		public static int[] FontYOffset = new int[] { 0, 128, 256, 384 };

		public static int[] FontPageOffset = new int[] { 0, 0, 256, 256 };

		public static readonly byte[] ColorIndex2Bits = new byte[] { 0, 0, 1, 2, 3, 3 };
		public static readonly byte[] Bits2ColorIndex = new byte[] { 1, 2, 3, 4 };


	}
}
