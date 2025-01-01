namespace FontMaker
{
	internal static class Constants
	{
		public static readonly string Title = "Atari FontMaker";

		public static Rectangle[] WhereAreTheFontBanksComingFrom =
		[
			new(0, 0, 512, 256),
			new(0, 256, 512, 512),
			new(0, 512, 512, 768),
			new(0, 768, 512, 1024)
		];

		public static int[] FontYOffset = [0, 128, 256, 384];

		public static int[] FontPageOffset = [0, 0, 256, 256];

		public static readonly byte[] ColorIndex2Bits = [0, 0, 1, 2, 3, 3];
		public static readonly byte[] Bits2ColorIndex = [1, 2, 3, 4];

		public static readonly byte[] ColorIndex2FourBits = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
		public static readonly byte[] FourBits2ColorIndex = [0, 1, 2, 3, 4, 5, 6, 7, 8, 8, 8, 8, 4, 5, 6, 7];
	}
}
