using System.Drawing.Imaging;

namespace FontMaker
{
	public static class AtariFontRenderer
	{
		private static readonly Color[] AtariColors = new Color[256];
		private static readonly byte[] MyPalette = new byte[AtariConstants.NumColors]; // Mono (0 + 1) Color (1, 2, 3, 4, 5, [6,7,8,])
		private static readonly int[] CachedColors = new int[AtariConstants.NumColors];

		private static readonly int[] Mode4Colors = new int[5]; // Map from color index to actual color value

		// Map from a nibble to an actual color index
		// 0-8 map to colors 0-8 (+1 for the CachedColors index)
		// 9,10(A),11(B) map to color 8
		// 12(C) maps to color 4
		// 13(D) maps to color 5
		// 14(E) maps to color 6
		// 15(F) maps to color 7
		private static readonly int[] Mode10Colors = new int[16];

		private static int[] _mode10ColorMappings = [0, 1, 2, 3, 4, 5, 6, 7, 8, 8, 8, 8, 4, 5, 6, 7];

		public static int WhichColorMode { get; set; } = 4;     // 4/5/10

		/// <summary>
		/// In which color mode is the cache rendered?
		/// </summary>
		public static int CachedColorMode { get; set; } = 0;

		/// <summary>
		/// This is the bitmap that contains all 4 fonts draw in 2x2 pixel size and in mono and in color
		/// </summary>
		public static Bitmap BitmapFontBanks { get; set; } = new(512, 512 + 512);
		// Width:  32 char @ 8 pixels (width of a char) x 2 (zoom level=2) = 512 pixels
		// Height: 32 char @ 8 pixels (height of a char) x 2 (zoom level=2) x 4 rows x 2 (normal+inverse) x 2 (fonts per bank) x 2 (# of banks) x 2 (mono/color) = 1024 pixels

		public static void SetPalette(Color[] colors)
		{
			colors.CopyTo(AtariColors, 0);
		}

		public static void RebuildPalette(byte[] selectorColorIndexes)
		{
			for (var i = 0; i < selectorColorIndexes.Length; i++)
			{
				MyPalette[i] = selectorColorIndexes[i];
				CachedColors[i] = AtariColors[selectorColorIndexes[i]].ToArgb();
			}
			// Build the mode 4 color mappings (0-3 + inverse of 3)
			Mode4Colors[0] = CachedColors[1];
			Mode4Colors[1] = CachedColors[2];
			Mode4Colors[2] = CachedColors[3];
			Mode4Colors[3] = CachedColors[4];
			Mode4Colors[4] = CachedColors[5];

			for (var i = 0; i < 16; i++)
			{
				Mode10Colors[i] = CachedColors[_mode10ColorMappings[i] + 1];
			}
		}

		public static void RebuildFontCache(int targetColorMode)
		{
			if (targetColorMode == CachedColorMode)
				return;
			RenderAllFonts();
		}
		/// <summary>
		/// Render 8 fonts (4 mono, 4 color) into the containing bitmap
		/// </summary>
		public static void RenderAllFonts()
		{
			CachedColorMode = WhichColorMode;

			var bmpData = BitmapFontBanks.LockBits(new Rectangle(0, 0, 512, 1024), ImageLockMode.WriteOnly, BitmapFontBanks.PixelFormat);

			// Map from a 2 bit color index (128|64), (32|16), (8|4), (2|1) to a color value
			// We don't shift the 2 bit color index to get a range of 0-3 but use the actual
			// value to access a mapping
			unsafe
			{
				// Setup some ptr manipulation numbers to move to the next line or to the next character
				// by simply adding or subtracting a number
				var toAddForNextLine = bmpData.Stride / 4;
				var toAddForNextDrawSection = 16 * 4 * bmpData.Stride / 4;
				var move2NextLine = (bmpData.Stride / 4 - 8) * 2;
				var move2TopOfNextChar = (bmpData.Stride / 4 * 16) - 16;

				// Where are each of the font's bytes to be found. Start with the first character (0)
				var byteIndex = new[]
				{
					1024 * 0,
					1024 * 1,
					1024 * 2,
					1024 * 3,
				};

				//  Draw the 4 fonts (Mono and Color mode)
				for (var row = 0; row < 4; row++)
				{
					// Setup the pointers where in memory the 8 fonts (4 normal + 4 inverted) will be drawn
					// Top 512 pixels of the bitmap are the mono fonts
					// Bottom 512 pixels of the bitmap are the color fonts
					var ptrFont0Mono = (int*)(bmpData.Scan0 + bmpData.Stride * 16 * row);
					var ptrFont0MonoNext = ptrFont0Mono + toAddForNextLine;
					var ptrFont0MonoInverse = ptrFont0Mono + toAddForNextDrawSection;
					var ptrFont0MonoInverseNext = ptrFont0MonoInverse + toAddForNextLine;

					var ptrFont1Mono = ptrFont0MonoInverse + toAddForNextDrawSection;
					var ptrFont1MonoNext = ptrFont1Mono + toAddForNextLine;
					var ptrFont1MonoInverse = ptrFont1Mono + toAddForNextDrawSection;
					var ptrFont1MonoInverseNext = ptrFont1MonoInverse + toAddForNextLine;

					var ptrFont2Mono = ptrFont1MonoInverse + toAddForNextDrawSection;
					var ptrFont2MonoNext = ptrFont2Mono + toAddForNextLine;
					var ptrFont2MonoInverse = ptrFont2Mono + toAddForNextDrawSection;
					var ptrFont2MonoInverseNext = ptrFont2MonoInverse + toAddForNextLine;

					var ptrFont3Mono = ptrFont2MonoInverse + toAddForNextDrawSection;
					var ptrFont3MonoNext = ptrFont3Mono + toAddForNextLine;
					var ptrFont3MonoInverse = ptrFont3Mono + toAddForNextDrawSection;
					var ptrFont3MonoInverseNext = ptrFont3MonoInverse + toAddForNextLine;

					var ptrFont0Color = ptrFont3MonoInverse + toAddForNextDrawSection;
					var ptrFont0ColorNext = ptrFont0Color + toAddForNextLine;
					var ptrFont0ColorInverse = ptrFont0Color + toAddForNextDrawSection;
					var ptrFont0ColorInverseNext = ptrFont0ColorInverse + toAddForNextLine;

					var ptrFont1Color = ptrFont0ColorInverse + toAddForNextDrawSection;
					var ptrFont1ColorNext = ptrFont1Color + toAddForNextLine;
					var ptrFont1ColorInverse = ptrFont1Color + toAddForNextDrawSection;
					var ptrFont1ColorInverseNext = ptrFont1ColorInverse + toAddForNextLine;

					var ptrFont2Color = ptrFont1ColorInverse + toAddForNextDrawSection;
					var ptrFont2ColorNext = ptrFont2Color + toAddForNextLine;
					var ptrFont2ColorInverse = ptrFont2Color + toAddForNextDrawSection;
					var ptrFont2ColorInverseNext = ptrFont2ColorInverse + toAddForNextLine;

					var ptrFont3Color = ptrFont2ColorInverse + toAddForNextDrawSection;
					var ptrFont3ColorNext = ptrFont3Color + toAddForNextLine;
					var ptrFont3ColorInverse = ptrFont3Color + toAddForNextDrawSection;
					var ptrFont3ColorInverseNext = ptrFont3ColorInverse + toAddForNextLine;

					// Draw 32 characters per line
					for (var col = 0; col < 32; col++)
					{
						// Draw the 8 lines of a character
						for (var y = 0; y < 8; y++)
						{
							// Get the bytes of the character
							var f1 = AtariFont.FontBytes[byteIndex[0]];
							var f2 = AtariFont.FontBytes[byteIndex[1]];
							var f3 = AtariFont.FontBytes[byteIndex[2]];
							var f4 = AtariFont.FontBytes[byteIndex[3]];

							int normCol;
							int invCol;
							var mask = 128;
							for (var x = 0; x < 8; ++x)
							{
								#region Font 1 in mono

								normCol = CachedColors[((f1 & mask) != 0) ? 0 : 1];
								invCol = CachedColors[((f1 & mask) != 0) ? 1 : 0];
								*ptrFont0Mono = normCol;
								++ptrFont0Mono;
								*ptrFont0Mono = normCol;
								++ptrFont0Mono;
								*ptrFont0MonoNext = normCol;
								++ptrFont0MonoNext;
								*ptrFont0MonoNext = normCol;
								++ptrFont0MonoNext;

								#endregion

								#region Font 1 in mono inverted

								*ptrFont0MonoInverse = invCol;
								++ptrFont0MonoInverse;
								*ptrFont0MonoInverse = invCol;
								++ptrFont0MonoInverse;
								*ptrFont0MonoInverseNext = invCol;
								++ptrFont0MonoInverseNext;
								*ptrFont0MonoInverseNext = invCol;
								++ptrFont0MonoInverseNext;

								#endregion

								#region Font 2 in mono

								normCol = CachedColors[((f2 & mask) != 0) ? 0 : 1];
								invCol = CachedColors[((f2 & mask) != 0) ? 1 : 0];
								*ptrFont1Mono = normCol;
								++ptrFont1Mono;
								*ptrFont1Mono = normCol;
								++ptrFont1Mono;
								*ptrFont1MonoNext = normCol;
								++ptrFont1MonoNext;
								*ptrFont1MonoNext = normCol;
								++ptrFont1MonoNext;

								#endregion

								#region Font 2 in mono inverted

								*ptrFont1MonoInverse = invCol;
								++ptrFont1MonoInverse;
								*ptrFont1MonoInverse = invCol;
								++ptrFont1MonoInverse;
								*ptrFont1MonoInverseNext = invCol;
								++ptrFont1MonoInverseNext;
								*ptrFont1MonoInverseNext = invCol;
								++ptrFont1MonoInverseNext;

								#endregion

								#region Font 3 in mono

								normCol = CachedColors[((f3 & mask) != 0) ? 0 : 1];
								invCol = CachedColors[((f3 & mask) != 0) ? 1 : 0];
								*ptrFont2Mono = normCol;
								++ptrFont2Mono;
								*ptrFont2Mono = normCol;
								++ptrFont2Mono;
								*ptrFont2MonoNext = normCol;
								++ptrFont2MonoNext;
								*ptrFont2MonoNext = normCol;
								++ptrFont2MonoNext;

								#endregion

								#region Font 3 in mono inverted

								*ptrFont2MonoInverse = invCol;
								++ptrFont2MonoInverse;
								*ptrFont2MonoInverse = invCol;
								++ptrFont2MonoInverse;
								*ptrFont2MonoInverseNext = invCol;
								++ptrFont2MonoInverseNext;
								*ptrFont2MonoInverseNext = invCol;
								++ptrFont2MonoInverseNext;

								#endregion

								#region Font 4 in mono

								normCol = CachedColors[((f4 & mask) != 0) ? 0 : 1];
								invCol = CachedColors[((f4 & mask) != 0) ? 1 : 0];
								*ptrFont3Mono = normCol;
								++ptrFont3Mono;
								*ptrFont3Mono = normCol;
								++ptrFont3Mono;
								*ptrFont3MonoNext = normCol;
								++ptrFont3MonoNext;
								*ptrFont3MonoNext = normCol;
								++ptrFont3MonoNext;

								#endregion

								#region Font 4 in mono inverted

								*ptrFont3MonoInverse = invCol;
								++ptrFont3MonoInverse;
								*ptrFont3MonoInverse = invCol;
								++ptrFont3MonoInverse;
								*ptrFont3MonoInverseNext = invCol;
								++ptrFont3MonoInverseNext;
								*ptrFont3MonoInverseNext = invCol;
								++ptrFont3MonoInverseNext;

								#endregion

								mask >>= 1;
							}

							// Draw the font in color (2-bits or 4-bits per pixel)
							switch (WhichColorMode)
							{
								case 4:
								case 5:
								default:
								{
									mask = 128 | 64;
									for (var x = 0; x < 4; ++x)
									{
										#region Font 1 in color

										var colorIndex = (f1 & mask) >> (6 - x * 2);
										normCol = Mode4Colors[colorIndex];
										if (colorIndex == 3) colorIndex++;
										invCol = Mode4Colors[colorIndex];

										*ptrFont0Color = normCol;
										++ptrFont0Color;
										*ptrFont0Color = normCol;
										++ptrFont0Color;
										*ptrFont0Color = normCol;
										++ptrFont0Color;
										*ptrFont0Color = normCol;
										++ptrFont0Color;

										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;
										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;
										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;
										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;

										#endregion

										#region Font 1 in color inverted

										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;
										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;
										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;
										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;

										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;
										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;
										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;
										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;

										#endregion

										#region Font 2 in color

										colorIndex = (f2 & mask) >> (6 - x * 2);
										normCol = Mode4Colors[colorIndex];
										if (colorIndex == 3) colorIndex++;
										invCol = Mode4Colors[colorIndex];

										*ptrFont1Color = normCol;
										++ptrFont1Color;
										*ptrFont1Color = normCol;
										++ptrFont1Color;
										*ptrFont1Color = normCol;
										++ptrFont1Color;
										*ptrFont1Color = normCol;
										++ptrFont1Color;

										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;
										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;
										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;
										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;

										#endregion

										#region Font 2 in color inverted

										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;
										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;
										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;
										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;

										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;
										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;
										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;
										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;

										#endregion

										#region Font 3 in color

										colorIndex = (f3 & mask) >> (6 - x * 2);
										normCol = Mode4Colors[colorIndex];
										if (colorIndex == 3) colorIndex++;
										invCol = Mode4Colors[colorIndex];

										*ptrFont2Color = normCol;
										++ptrFont2Color;
										*ptrFont2Color = normCol;
										++ptrFont2Color;
										*ptrFont2Color = normCol;
										++ptrFont2Color;
										*ptrFont2Color = normCol;
										++ptrFont2Color;

										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;
										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;
										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;
										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;

										#endregion

										#region Font 3 in color inverted

										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;
										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;
										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;
										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;

										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;
										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;
										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;
										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;

										#endregion

										#region Font 4 in color

										colorIndex = (f4 & mask) >> (6 - x * 2);
										normCol = Mode4Colors[colorIndex];
										if (colorIndex == 3) colorIndex++;
										invCol = Mode4Colors[colorIndex];

										*ptrFont3Color = normCol;
										++ptrFont3Color;
										*ptrFont3Color = normCol;
										++ptrFont3Color;
										*ptrFont3Color = normCol;
										++ptrFont3Color;
										*ptrFont3Color = normCol;
										++ptrFont3Color;

										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;
										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;
										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;
										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;

										#endregion

										#region Font 4 in color inverted

										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;
										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;
										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;
										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;

										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;
										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;
										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;
										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;

										#endregion

										mask >>= 2;
									}

									break;
								}

								case 10:
								{
									mask = 128 | 64 | 32 | 16;
									for (var x = 0; x < 2; ++x)
									{
										#region Font 1 in color

										var colorIndex = (f1 & mask) >> (4 - x * 4);
										invCol = normCol = Mode10Colors[colorIndex];

										*ptrFont0Color = normCol;
										++ptrFont0Color;
										*ptrFont0Color = normCol;
										++ptrFont0Color;
										*ptrFont0Color = normCol;
										++ptrFont0Color;
										*ptrFont0Color = normCol;
										++ptrFont0Color;
										*ptrFont0Color = normCol;
										++ptrFont0Color;
										*ptrFont0Color = normCol;
										++ptrFont0Color;
										*ptrFont0Color = normCol;
										++ptrFont0Color;
										*ptrFont0Color = normCol;
										++ptrFont0Color;

										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;
										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;
										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;
										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;
										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;
										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;
										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;
										*ptrFont0ColorNext = normCol;
										++ptrFont0ColorNext;

										#endregion

										#region Font 1 in color inverted

										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;
										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;
										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;
										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;
										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;
										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;
										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;
										*ptrFont0ColorInverse = invCol;
										++ptrFont0ColorInverse;

										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;
										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;
										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;
										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;
										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;
										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;
										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;
										*ptrFont0ColorInverseNext = invCol;
										++ptrFont0ColorInverseNext;

										#endregion

										#region Font 2 in color

										colorIndex = (f2 & mask) >> (4 - x * 4);
										invCol = normCol = Mode10Colors[colorIndex];

										*ptrFont1Color = normCol;
										++ptrFont1Color;
										*ptrFont1Color = normCol;
										++ptrFont1Color;
										*ptrFont1Color = normCol;
										++ptrFont1Color;
										*ptrFont1Color = normCol;
										++ptrFont1Color;
										*ptrFont1Color = normCol;
										++ptrFont1Color;
										*ptrFont1Color = normCol;
										++ptrFont1Color;
										*ptrFont1Color = normCol;
										++ptrFont1Color;
										*ptrFont1Color = normCol;
										++ptrFont1Color;

										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;
										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;
										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;
										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;
										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;
										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;
										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;
										*ptrFont1ColorNext = normCol;
										++ptrFont1ColorNext;

										#endregion

										#region Font 2 in color inverted

										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;
										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;
										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;
										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;
										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;
										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;
										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;
										*ptrFont1ColorInverse = invCol;
										++ptrFont1ColorInverse;

										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;
										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;
										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;
										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;
										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;
										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;
										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;
										*ptrFont1ColorInverseNext = invCol;
										++ptrFont1ColorInverseNext;

										#endregion

										#region Font 3 in color

										colorIndex = (f3 & mask) >> (4 - x * 4);
										invCol = normCol = Mode10Colors[colorIndex];

										*ptrFont2Color = normCol;
										++ptrFont2Color;
										*ptrFont2Color = normCol;
										++ptrFont2Color;
										*ptrFont2Color = normCol;
										++ptrFont2Color;
										*ptrFont2Color = normCol;
										++ptrFont2Color;
										*ptrFont2Color = normCol;
										++ptrFont2Color;
										*ptrFont2Color = normCol;
										++ptrFont2Color;
										*ptrFont2Color = normCol;
										++ptrFont2Color;
										*ptrFont2Color = normCol;
										++ptrFont2Color;

										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;
										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;
										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;
										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;
										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;
										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;
										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;
										*ptrFont2ColorNext = normCol;
										++ptrFont2ColorNext;

										#endregion

										#region Font 3 in color inverted

										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;
										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;
										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;
										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;
										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;
										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;
										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;
										*ptrFont2ColorInverse = invCol;
										++ptrFont2ColorInverse;

										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;
										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;
										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;
										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;
										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;
										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;
										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;
										*ptrFont2ColorInverseNext = invCol;
										++ptrFont2ColorInverseNext;

										#endregion

										#region Font 4 in color

										colorIndex = (f4 & mask) >> (4 - x * 4);
										invCol = normCol = Mode10Colors[colorIndex];

										*ptrFont3Color = normCol;
										++ptrFont3Color;
										*ptrFont3Color = normCol;
										++ptrFont3Color;
										*ptrFont3Color = normCol;
										++ptrFont3Color;
										*ptrFont3Color = normCol;
										++ptrFont3Color;
										*ptrFont3Color = normCol;
										++ptrFont3Color;
										*ptrFont3Color = normCol;
										++ptrFont3Color;
										*ptrFont3Color = normCol;
										++ptrFont3Color;
										*ptrFont3Color = normCol;
										++ptrFont3Color;

										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;
										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;
										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;
										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;
										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;
										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;
										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;
										*ptrFont3ColorNext = normCol;
										++ptrFont3ColorNext;

										#endregion

										#region Font 4 in color inverted

										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;
										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;
										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;
										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;
										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;
										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;
										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;
										*ptrFont3ColorInverse = invCol;
										++ptrFont3ColorInverse;

										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;
										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;
										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;
										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;
										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;
										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;
										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;
										*ptrFont3ColorInverseNext = invCol;
										++ptrFont3ColorInverseNext;

										#endregion

										mask >>= 4;
									}
									break;
								}
							}

							// Move to the next byte in the character
							++byteIndex[0];
							++byteIndex[1];
							++byteIndex[2];
							++byteIndex[3];

							// Move the draw pointers to the next line
							ptrFont0Mono += move2NextLine;
							ptrFont0MonoNext += move2NextLine;
							ptrFont0MonoInverse += move2NextLine;
							ptrFont0MonoInverseNext += move2NextLine;

							ptrFont1Mono += move2NextLine;
							ptrFont1MonoNext += move2NextLine;
							ptrFont1MonoInverse += move2NextLine;
							ptrFont1MonoInverseNext += move2NextLine;

							ptrFont2Mono += move2NextLine;
							ptrFont2MonoNext += move2NextLine;
							ptrFont2MonoInverse += move2NextLine;
							ptrFont2MonoInverseNext += move2NextLine;

							ptrFont3Mono += move2NextLine;
							ptrFont3MonoNext += move2NextLine;
							ptrFont3MonoInverse += move2NextLine;
							ptrFont3MonoInverseNext += move2NextLine;

							ptrFont0Color += move2NextLine;
							ptrFont0ColorNext += move2NextLine;
							ptrFont0ColorInverse += move2NextLine;
							ptrFont0ColorInverseNext += move2NextLine;

							ptrFont1Color += move2NextLine;
							ptrFont1ColorNext += move2NextLine;
							ptrFont1ColorInverse += move2NextLine;
							ptrFont1ColorInverseNext += move2NextLine;

							ptrFont2Color += move2NextLine;
							ptrFont2ColorNext += move2NextLine;
							ptrFont2ColorInverse += move2NextLine;
							ptrFont2ColorInverseNext += move2NextLine;

							ptrFont3Color += move2NextLine;
							ptrFont3ColorNext += move2NextLine;
							ptrFont3ColorInverse += move2NextLine;
							ptrFont3ColorInverseNext += move2NextLine;
						}

						// Move the draw pointers to the next character
						ptrFont0Mono -= move2TopOfNextChar;
						ptrFont0MonoNext -= move2TopOfNextChar;
						ptrFont0MonoInverse -= move2TopOfNextChar;
						ptrFont0MonoInverseNext -= move2TopOfNextChar;

						ptrFont1Mono -= move2TopOfNextChar;
						ptrFont1MonoNext -= move2TopOfNextChar;
						ptrFont1MonoInverse -= move2TopOfNextChar;
						ptrFont1MonoInverseNext -= move2TopOfNextChar;

						ptrFont2Mono -= move2TopOfNextChar;
						ptrFont2MonoNext -= move2TopOfNextChar;
						ptrFont2MonoInverse -= move2TopOfNextChar;
						ptrFont2MonoInverseNext -= move2TopOfNextChar;

						ptrFont3Mono -= move2TopOfNextChar;
						ptrFont3MonoNext -= move2TopOfNextChar;
						ptrFont3MonoInverse -= move2TopOfNextChar;
						ptrFont3MonoInverseNext -= move2TopOfNextChar;

						ptrFont0Color -= move2TopOfNextChar;
						ptrFont0ColorNext -= move2TopOfNextChar;
						ptrFont0ColorInverse -= move2TopOfNextChar;
						ptrFont0ColorInverseNext -= move2TopOfNextChar;

						ptrFont1Color -= move2TopOfNextChar;
						ptrFont1ColorNext -= move2TopOfNextChar;
						ptrFont1ColorInverse -= move2TopOfNextChar;
						ptrFont1ColorInverseNext -= move2TopOfNextChar;

						ptrFont2Color -= move2TopOfNextChar;
						ptrFont2ColorNext -= move2TopOfNextChar;
						ptrFont2ColorInverse -= move2TopOfNextChar;
						ptrFont2ColorInverseNext -= move2TopOfNextChar;

						ptrFont3Color -= move2TopOfNextChar;
						ptrFont3ColorNext -= move2TopOfNextChar;
						ptrFont3ColorInverse -= move2TopOfNextChar;
						ptrFont3ColorInverseNext -= move2TopOfNextChar;
					}
				}
			}

			BitmapFontBanks.UnlockBits(bmpData);
		}

		/// <summary>
		/// Render a single character into the correct font (in mono and in color)
		/// Screen layout is
		/// Normal:  32 chars x 4 lines for font 1 or 3
		/// Inverse: 32 chars x 4 lines
		/// Normal:  32 chars x 4 lines for font 2 or 4
		/// Inverse: 32 chars x 4 lines
		/// </summary>
		/// <param name="selectedCharacterIndex">0-511 selecting which of the visible characters needs to be redrawn</param>
		/// <param name="onBank2">Font bank 2 indicator. Set to select font 3+4</param>
		public static void RenderOneCharacter(int selectedCharacterIndex, bool onBank2)
		{
			var ry = selectedCharacterIndex / 32;   // 0 - 15
			var rx = selectedCharacterIndex % 32;   // 0 - 31
			var fontInBankOffset = onBank2 ? 2048 : 0; // How far into the font byte buffer are we looking?

			var fontNr = (ry / 8) + (onBank2 ? 2 : 0);  // 0, 1, 2, 3

			var drawYOffset = ry % 4;               // How far down the 4 normal lines is this character

			// Font byte offset calculations: 0-15 map to 0,1,2,3,0,1,2,3,4,5,6,7,4,5,6,7
			if (ry is > 3 and < 12)
			{
				ry -= 4;
			}
			if (ry is > 11 and < 16)
			{
				ry -= 8;
			}

			var startOf8FontBytes = ry * 32 * 8 + rx * 8 + fontInBankOffset; // Source of the font bytes

			var bmpData = BitmapFontBanks.LockBits(new Rectangle(0, 0, 512, 1024), ImageLockMode.WriteOnly, BitmapFontBanks.PixelFormat);
			unsafe
			{
				// Setup some ptr manipulation numbers to move to the next line
				// by simply adding or subtracting a number
				var toAddForNextLine = bmpData.Stride / 4;
				var toAddForNextDrawSection = 16 * 4 * bmpData.Stride / 4;
				var move2NextLine = (bmpData.Stride / 4 - 8) * 2;

				// Setup the pointers where in memory the char will be drawn
				// Top 512 pixels of the bitmap are the mono fonts
				// Bottom 512 pixels of the bitmap are the color fonts
				var ptrMono = (int*)(bmpData.Scan0 + bmpData.Stride * (16 * 8 * fontNr + drawYOffset * 16) + rx * 16 * 4);
				var ptrMonoNext = ptrMono + toAddForNextLine;
				var ptrMonoInverse = ptrMono + toAddForNextDrawSection;
				var ptrMonoInverseNext = ptrMonoInverse + toAddForNextLine;
				var ptrColor = ptrMono + toAddForNextDrawSection * 8;
				var ptrColorNext = ptrColor + toAddForNextLine;
				var ptrColorInverse = ptrColor + toAddForNextDrawSection;
				var ptrColorInverseNext = ptrColorInverse + toAddForNextLine;

				// Draw the 8 lines of a character
				for (var y = 0; y < 8; y++, ++startOf8FontBytes)
				{
					// Get the bytes of the character
					var fontByte = AtariFont.FontBytes[startOf8FontBytes];

					int normCol;
					int invCol;
					var mask = 128;
					for (var x = 0; x < 8; ++x)
					{
						#region Font in mono

						normCol = CachedColors[((fontByte & mask) != 0) ? 0 : 1];
						invCol = CachedColors[((fontByte & mask) != 0) ? 1 : 0];
						*ptrMono = normCol;
						++ptrMono;
						*ptrMono = normCol;
						++ptrMono;
						*ptrMonoNext = normCol;
						++ptrMonoNext;
						*ptrMonoNext = normCol;
						++ptrMonoNext;

						#endregion

						#region Font in mono inverted

						*ptrMonoInverse = invCol;
						++ptrMonoInverse;
						*ptrMonoInverse = invCol;
						++ptrMonoInverse;
						*ptrMonoInverseNext = invCol;
						++ptrMonoInverseNext;
						*ptrMonoInverseNext = invCol;
						++ptrMonoInverseNext;

						#endregion

						mask >>= 1;
					}

					// Draw the font in color (2-bits or 4-bits per pixel)
					switch (WhichColorMode)
					{
						case 4:
						case 5:
						default:
						{
							mask = 128 | 64;
							for (var x = 0; x < 4; ++x)
							{
								#region Font in color

								var colorIndex = (fontByte & mask) >> (6 - x * 2);
								normCol = Mode4Colors[colorIndex];
								if (colorIndex == 3) colorIndex++;
								invCol = Mode4Colors[colorIndex];

								*ptrColor = normCol;
								++ptrColor;
								*ptrColor = normCol;
								++ptrColor;
								*ptrColor = normCol;
								++ptrColor;
								*ptrColor = normCol;
								++ptrColor;

								*ptrColorNext = normCol;
								++ptrColorNext;
								*ptrColorNext = normCol;
								++ptrColorNext;
								*ptrColorNext = normCol;
								++ptrColorNext;
								*ptrColorNext = normCol;
								++ptrColorNext;

								#endregion

								#region Font in color inverted

								*ptrColorInverse = invCol;
								++ptrColorInverse;
								*ptrColorInverse = invCol;
								++ptrColorInverse;
								*ptrColorInverse = invCol;
								++ptrColorInverse;
								*ptrColorInverse = invCol;
								++ptrColorInverse;

								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;
								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;
								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;
								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;

								#endregion

								mask >>= 2;
							}

							break;
						}
						case 10:
						{
							mask = 128 | 64 | 32 | 16;
							for (var x = 0; x < 2; ++x)
							{
								#region Font in color

								var colorIndex = (fontByte & mask) >> (4 - x * 4);
								invCol = normCol = Mode10Colors[colorIndex];

								*ptrColor = normCol;
								++ptrColor;
								*ptrColor = normCol;
								++ptrColor;
								*ptrColor = normCol;
								++ptrColor;
								*ptrColor = normCol;
								++ptrColor;
								*ptrColor = normCol;
								++ptrColor;
								*ptrColor = normCol;
								++ptrColor;
								*ptrColor = normCol;
								++ptrColor;
								*ptrColor = normCol;
								++ptrColor;

								*ptrColorNext = normCol;
								++ptrColorNext;
								*ptrColorNext = normCol;
								++ptrColorNext;
								*ptrColorNext = normCol;
								++ptrColorNext;
								*ptrColorNext = normCol;
								++ptrColorNext;
								*ptrColorNext = normCol;
								++ptrColorNext;
								*ptrColorNext = normCol;
								++ptrColorNext;
								*ptrColorNext = normCol;
								++ptrColorNext;
								*ptrColorNext = normCol;
								++ptrColorNext;

								#endregion

								#region Font in color inverted

								*ptrColorInverse = invCol;
								++ptrColorInverse;
								*ptrColorInverse = invCol;
								++ptrColorInverse;
								*ptrColorInverse = invCol;
								++ptrColorInverse;
								*ptrColorInverse = invCol;
								++ptrColorInverse;
								*ptrColorInverse = invCol;
								++ptrColorInverse;
								*ptrColorInverse = invCol;
								++ptrColorInverse;
								*ptrColorInverse = invCol;
								++ptrColorInverse;
								*ptrColorInverse = invCol;
								++ptrColorInverse;

								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;
								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;
								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;
								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;
								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;
								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;
								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;
								*ptrColorInverseNext = invCol;
								++ptrColorInverseNext;

								#endregion

								mask >>= 4;
							}
							break;
						}
					}

					ptrMono += move2NextLine;
					ptrMonoNext += move2NextLine;
					ptrMonoInverse += move2NextLine;
					ptrMonoInverseNext += move2NextLine;

					ptrColor += move2NextLine;
					ptrColorNext += move2NextLine;
					ptrColorInverse += move2NextLine;
					ptrColorInverseNext += move2NextLine;
				}
			}
			BitmapFontBanks.UnlockBits(bmpData);
		}
	}
}
