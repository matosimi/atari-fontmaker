using System.Drawing.Imaging;

namespace FontMaker
{
	public static class AtariFontRenderer
	{
		private static Color[] _atariColors = new Color[256];
		private static byte[] myPalette = new byte[6]; // Mono (0 + 1) Color (1, 2, 3, 4, 5)
		private static int[] cachedColors = new int[6];

		private static readonly int[] _mode4Colors = new int[5]; // Map from color index to actual color value

		/// <summary>
		/// This is the bitmap that contains all 4 fonts draw in 2x2 pixel size and in mono and in color
		/// </summary>
		public static Bitmap BitmapFontBanks { get; set; } = new Bitmap(512, 512 + 512);
		// Width:  32 char @ 8 pixels (width of a char) x 2 (zoom level=2) = 512 pixels
		// Height: 32 char @ 8 pixels (height of a char) x 2 (zoom level=2) x 4 rows x 2 (normal+inverse) x 2 (fonts per bank) x 2 (# of banks) x 2 (mono/color) = 1024 pixels

		public static void SetPalette(Color[] colors)
		{
			_atariColors = colors;
		}

		public static void RebuildPalette(byte[] selectorColorIndexes)
		{
			for (var i = 0; i < selectorColorIndexes.Length; i++)
			{
				myPalette[i] = selectorColorIndexes[i];
				cachedColors[i] = _atariColors[selectorColorIndexes[i]].ToArgb();
			}
			// Build the mode 4 color mappings (0-3 + inverse of 3)
			_mode4Colors[0] = cachedColors[1];
			_mode4Colors[1] = cachedColors[2];
			_mode4Colors[2] = cachedColors[3];
			_mode4Colors[3] = cachedColors[4];
			_mode4Colors[4] = cachedColors[5];
		}

		/// <summary>
		/// Render 8 fonts (4 mono, 4 color) into the containing bitmap
		/// </summary>
		public static void RenderAllFonts()
		{
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
				var byteIndex = new int[]
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

							int ncol;
							int icol;
							int colorIndex;
							var mask = 128;
							for (var x = 0; x < 8; ++x)
							{
								#region Font 1 in mono

								ncol = cachedColors[((f1 & mask) != 0) ? 0 : 1];
								icol = cachedColors[((f1 & mask) != 0) ? 1 : 0];
								*ptrFont0Mono = ncol;
								++ptrFont0Mono;
								*ptrFont0Mono = ncol;
								++ptrFont0Mono;
								*ptrFont0MonoNext = ncol;
								++ptrFont0MonoNext;
								*ptrFont0MonoNext = ncol;
								++ptrFont0MonoNext;

								#endregion

								#region Font 1 in mono inverted

								*ptrFont0MonoInverse = icol;
								++ptrFont0MonoInverse;
								*ptrFont0MonoInverse = icol;
								++ptrFont0MonoInverse;
								*ptrFont0MonoInverseNext = icol;
								++ptrFont0MonoInverseNext;
								*ptrFont0MonoInverseNext = icol;
								++ptrFont0MonoInverseNext;

								#endregion

								#region Font 2 in mono

								ncol = cachedColors[((f2 & mask) != 0) ? 0 : 1];
								icol = cachedColors[((f2 & mask) != 0) ? 1 : 0];
								*ptrFont1Mono = ncol;
								++ptrFont1Mono;
								*ptrFont1Mono = ncol;
								++ptrFont1Mono;
								*ptrFont1MonoNext = ncol;
								++ptrFont1MonoNext;
								*ptrFont1MonoNext = ncol;
								++ptrFont1MonoNext;

								#endregion

								#region Font 2 in mono inverted

								*ptrFont1MonoInverse = icol;
								++ptrFont1MonoInverse;
								*ptrFont1MonoInverse = icol;
								++ptrFont1MonoInverse;
								*ptrFont1MonoInverseNext = icol;
								++ptrFont1MonoInverseNext;
								*ptrFont1MonoInverseNext = icol;
								++ptrFont1MonoInverseNext;

								#endregion

								#region Font 3 in mono

								ncol = cachedColors[((f3 & mask) != 0) ? 0 : 1];
								icol = cachedColors[((f3 & mask) != 0) ? 1 : 0];
								*ptrFont2Mono = ncol;
								++ptrFont2Mono;
								*ptrFont2Mono = ncol;
								++ptrFont2Mono;
								*ptrFont2MonoNext = ncol;
								++ptrFont2MonoNext;
								*ptrFont2MonoNext = ncol;
								++ptrFont2MonoNext;

								#endregion

								#region Font 3 in mono inverted

								*ptrFont2MonoInverse = icol;
								++ptrFont2MonoInverse;
								*ptrFont2MonoInverse = icol;
								++ptrFont2MonoInverse;
								*ptrFont2MonoInverseNext = icol;
								++ptrFont2MonoInverseNext;
								*ptrFont2MonoInverseNext = icol;
								++ptrFont2MonoInverseNext;

								#endregion

								#region Font 4 in mono

								ncol = cachedColors[((f4 & mask) != 0) ? 0 : 1];
								icol = cachedColors[((f4 & mask) != 0) ? 1 : 0];
								*ptrFont3Mono = ncol;
								++ptrFont3Mono;
								*ptrFont3Mono = ncol;
								++ptrFont3Mono;
								*ptrFont3MonoNext = ncol;
								++ptrFont3MonoNext;
								*ptrFont3MonoNext = ncol;
								++ptrFont3MonoNext;

								#endregion

								#region Font 4 in mono inverted

								*ptrFont3MonoInverse = icol;
								++ptrFont3MonoInverse;
								*ptrFont3MonoInverse = icol;
								++ptrFont3MonoInverse;
								*ptrFont3MonoInverseNext = icol;
								++ptrFont3MonoInverseNext;
								*ptrFont3MonoInverseNext = icol;
								++ptrFont3MonoInverseNext;

								#endregion

								mask >>= 1;
							}

							mask = 128 | 64;
							for (var x = 0; x < 4; ++x)
							{
								#region Font 1 in color

								colorIndex = (f1 & mask) >> (6-x*2);
								ncol = _mode4Colors[colorIndex];
								if (colorIndex == 3) colorIndex++;
								icol = _mode4Colors[colorIndex];

								*ptrFont0Color = ncol;
								++ptrFont0Color;
								*ptrFont0Color = ncol;
								++ptrFont0Color;
								*ptrFont0Color = ncol;
								++ptrFont0Color;
								*ptrFont0Color = ncol;
								++ptrFont0Color;

								*ptrFont0ColorNext = ncol;
								++ptrFont0ColorNext;
								*ptrFont0ColorNext = ncol;
								++ptrFont0ColorNext;
								*ptrFont0ColorNext = ncol;
								++ptrFont0ColorNext;
								*ptrFont0ColorNext = ncol;
								++ptrFont0ColorNext;

								#endregion

								#region Font 1 in color inverted

								*ptrFont0ColorInverse = icol;
								++ptrFont0ColorInverse;
								*ptrFont0ColorInverse = icol;
								++ptrFont0ColorInverse;
								*ptrFont0ColorInverse = icol;
								++ptrFont0ColorInverse;
								*ptrFont0ColorInverse = icol;
								++ptrFont0ColorInverse;

								*ptrFont0ColorInverseNext = icol;
								++ptrFont0ColorInverseNext;
								*ptrFont0ColorInverseNext = icol;
								++ptrFont0ColorInverseNext;
								*ptrFont0ColorInverseNext = icol;
								++ptrFont0ColorInverseNext;
								*ptrFont0ColorInverseNext = icol;
								++ptrFont0ColorInverseNext;
								#endregion

								#region Font 2 in color

								colorIndex = (f2 & mask) >> (6 - x * 2);
								ncol = _mode4Colors[colorIndex];
								if (colorIndex == 3) colorIndex++;
								icol = _mode4Colors[colorIndex];

								*ptrFont1Color = ncol;
								++ptrFont1Color;
								*ptrFont1Color = ncol;
								++ptrFont1Color;
								*ptrFont1Color = ncol;
								++ptrFont1Color;
								*ptrFont1Color = ncol;
								++ptrFont1Color;

								*ptrFont1ColorNext = ncol;
								++ptrFont1ColorNext;
								*ptrFont1ColorNext = ncol;
								++ptrFont1ColorNext;
								*ptrFont1ColorNext = ncol;
								++ptrFont1ColorNext;
								*ptrFont1ColorNext = ncol;
								++ptrFont1ColorNext;

								#endregion

								#region Font 2 in color inverted

								*ptrFont1ColorInverse = icol;
								++ptrFont1ColorInverse;
								*ptrFont1ColorInverse = icol;
								++ptrFont1ColorInverse;
								*ptrFont1ColorInverse = icol;
								++ptrFont1ColorInverse;
								*ptrFont1ColorInverse = icol;
								++ptrFont1ColorInverse;

								*ptrFont1ColorInverseNext = icol;
								++ptrFont1ColorInverseNext;
								*ptrFont1ColorInverseNext = icol;
								++ptrFont1ColorInverseNext;
								*ptrFont1ColorInverseNext = icol;
								++ptrFont1ColorInverseNext;
								*ptrFont1ColorInverseNext = icol;
								++ptrFont1ColorInverseNext;

								#endregion

								#region Font 3 in color

								colorIndex = (f3 & mask) >> (6 - x * 2);
								ncol = _mode4Colors[colorIndex];
								if (colorIndex == 3) colorIndex++;
								icol = _mode4Colors[colorIndex];

								*ptrFont2Color = ncol;
								++ptrFont2Color;
								*ptrFont2Color = ncol;
								++ptrFont2Color;
								*ptrFont2Color = ncol;
								++ptrFont2Color;
								*ptrFont2Color = ncol;
								++ptrFont2Color;

								*ptrFont2ColorNext = ncol;
								++ptrFont2ColorNext;
								*ptrFont2ColorNext = ncol;
								++ptrFont2ColorNext;
								*ptrFont2ColorNext = ncol;
								++ptrFont2ColorNext;
								*ptrFont2ColorNext = ncol;
								++ptrFont2ColorNext;

								#endregion

								#region Font 3 in color inverted

								*ptrFont2ColorInverse = icol;
								++ptrFont2ColorInverse;
								*ptrFont2ColorInverse = icol;
								++ptrFont2ColorInverse;
								*ptrFont2ColorInverse = icol;
								++ptrFont2ColorInverse;
								*ptrFont2ColorInverse = icol;
								++ptrFont2ColorInverse;

								*ptrFont2ColorInverseNext = icol;
								++ptrFont2ColorInverseNext;
								*ptrFont2ColorInverseNext = icol;
								++ptrFont2ColorInverseNext;
								*ptrFont2ColorInverseNext = icol;
								++ptrFont2ColorInverseNext;
								*ptrFont2ColorInverseNext = icol;
								++ptrFont2ColorInverseNext;

								#endregion

								#region Font 4 in color

								colorIndex = (f4 & mask) >> (6 - x * 2);
								ncol = _mode4Colors[colorIndex];
								if (colorIndex == 3) colorIndex++;
								icol = _mode4Colors[colorIndex];

								*ptrFont3Color = ncol;
								++ptrFont3Color;
								*ptrFont3Color = ncol;
								++ptrFont3Color;
								*ptrFont3Color = ncol;
								++ptrFont3Color;
								*ptrFont3Color = ncol;
								++ptrFont3Color;

								*ptrFont3ColorNext = ncol;
								++ptrFont3ColorNext;
								*ptrFont3ColorNext = ncol;
								++ptrFont3ColorNext;
								*ptrFont3ColorNext = ncol;
								++ptrFont3ColorNext;
								*ptrFont3ColorNext = ncol;
								++ptrFont3ColorNext;

								#endregion

								#region Font 4 in color inverted

								*ptrFont3ColorInverse = icol;
								++ptrFont3ColorInverse;
								*ptrFont3ColorInverse = icol;
								++ptrFont3ColorInverse;
								*ptrFont3ColorInverse = icol;
								++ptrFont3ColorInverse;
								*ptrFont3ColorInverse = icol;
								++ptrFont3ColorInverse;

								*ptrFont3ColorInverseNext = icol;
								++ptrFont3ColorInverseNext;
								*ptrFont3ColorInverseNext = icol;
								++ptrFont3ColorInverseNext;
								*ptrFont3ColorInverseNext = icol;
								++ptrFont3ColorInverseNext;
								*ptrFont3ColorInverseNext = icol;
								++ptrFont3ColorInverseNext;

								#endregion

								mask >>= 2;
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
			var ry = selectedCharacterIndex / 32;	// 0 - 15
			var rx = selectedCharacterIndex % 32;	// 0 - 31
			var fontInBankOffset = onBank2 ? 2048 : 0; // How far into the font byte buffer are we looking?

			var fontNr = (ry / 8) + (onBank2 ? 2 : 0);	// 0, 1, 2, 3

			var drawYOffset = ry % 4;				// How far down the 4 normal lines is this character

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

					int ncol;
					int icol;
					int colorIndex;
					int invIndex;
					var mask = 128;
					for (var x = 0; x < 8; ++x)
					{
						#region Font in mono

						ncol = cachedColors[((fontByte & mask) != 0) ? 0 : 1];
						icol = cachedColors[((fontByte & mask) != 0) ? 1 : 0];
						*ptrMono = ncol;
						++ptrMono;
						*ptrMono = ncol;
						++ptrMono;
						*ptrMonoNext = ncol;
						++ptrMonoNext;
						*ptrMonoNext = ncol;
						++ptrMonoNext;

						#endregion

						#region Font in mono inverted

						*ptrMonoInverse = icol;
						++ptrMonoInverse;
						*ptrMonoInverse = icol;
						++ptrMonoInverse;
						*ptrMonoInverseNext = icol;
						++ptrMonoInverseNext;
						*ptrMonoInverseNext = icol;
						++ptrMonoInverseNext;

						#endregion

						mask >>= 1;
					}

					mask = 128 | 64;
					for (var x = 0; x < 4; ++x)
					{
						#region Font in color

						colorIndex = (fontByte & mask) >> (6 - x * 2);
						ncol = _mode4Colors[colorIndex];
						if (colorIndex == 3) colorIndex++;
						icol = _mode4Colors[colorIndex];

						*ptrColor = ncol;
						++ptrColor;
						*ptrColor = ncol;
						++ptrColor;
						*ptrColor = ncol;
						++ptrColor;
						*ptrColor = ncol;
						++ptrColor;

						*ptrColorNext = ncol;
						++ptrColorNext;
						*ptrColorNext = ncol;
						++ptrColorNext;
						*ptrColorNext = ncol;
						++ptrColorNext;
						*ptrColorNext = ncol;
						++ptrColorNext;

						#endregion

						#region Font in color inverted

						*ptrColorInverse = icol;
						++ptrColorInverse;
						*ptrColorInverse = icol;
						++ptrColorInverse;
						*ptrColorInverse = icol;
						++ptrColorInverse;
						*ptrColorInverse = icol;
						++ptrColorInverse;

						*ptrColorInverseNext = icol;
						++ptrColorInverseNext;
						*ptrColorInverseNext = icol;
						++ptrColorInverseNext;
						*ptrColorInverseNext = icol;
						++ptrColorInverseNext;
						*ptrColorInverseNext = icol;
						++ptrColorInverseNext;

						#endregion

						mask >>= 2;
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
