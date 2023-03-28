namespace FontMaker
{
	internal class Constants
	{
		public const int VIEW_WIDTH = 40;
		public const int VIEW_HEIGHT = 26;

		public const int NUM_FONT_BYTES = 1024 * 4;			// How many bytes to all the fonts take up

		public static Rectangle RectFontBank12 = new Rectangle(0, 0, 512, 256);
		public static Rectangle RectFontBank34 = new Rectangle(0, 256, 512, 512);

		public static int[] FontYOffset = new int[4] { 0, 128, 256, 384 };

		public static int[] FontPageOffset = new int[4] { 0, 0, 256, 256 };
	}
}
