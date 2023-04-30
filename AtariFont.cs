using System.Security.Policy;

namespace FontMaker
{
	public static class AtariFont
	{
		public const int NumFontBytes = 1024 * 4;         // How many bytes to all the fonts take up

		#region Data
		/// <summary>
		/// All bytes for 4x Atari fonts (128 characters * 8 bytes per character * 4 fonts)
		/// </summary>
		public static byte[] FontBytes { get; set; } = new byte[NumFontBytes];

		private static byte[] OneCharacterBuffer = new byte[8];
		private static byte[,] One8x8Buffer = new byte[8, 8];

		#endregion

		#region Methods

		/// <summary>
		/// Load a single or dual font file into a specific bank (or two consecutive banks)
		/// </summary>
		/// <param name="filename">File to load the data from</param>
		/// <param name="fontNr">0,1,2,3 - one of the banks to load the font data into</param>
		/// <param name="dual">True then a double font will be loaded into consecutive banks</param>
		public static void LoadFont(string filename, int fontNr, bool dual)
		{
			var expectedSize = dual ? 2048 : 1024;

			try
			{
				using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
				var numReadBytes = fs.Read(FontBytes, fontNr * 1024, expectedSize);

				if (numReadBytes != expectedSize)
				{
					MessageBox.Show($@"File size different, expected to load {expectedSize} bytes, but loaded {numReadBytes}");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($@"Failed to load font {fontNr + 1} from '{filename}'. Reason:{ex.Message}");
			}
		}

		/// <summary>
		/// Save the 1024 bytes of a font away to a file
		/// </summary>
		/// <param name="filename">File to save the 1024 bytes to</param>
		/// <param name="fontNr">0,1,2,3 - one of the font banks to save from</param>
		public static void SaveFont(string filename, int fontNr)
		{
			try
			{
				using var fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.Write);
				fs.Write(FontBytes, fontNr * 1024, 1024);
			}
			catch(Exception ex)
			{
				MessageBox.Show($@"Failed to save font {fontNr+1} to '{filename}'. Reason:{ex.Message}");
			}
		}

		/// <summary>
		/// Clear a specific font
		/// </summary>
		/// <param name="fontNr">0,1,2,3 - one of the font banks</param>
		public static void ClearFont(int fontNr)
		{
			Array.Clear(FontBytes, fontNr * 1024, 1024);
		}

		/// <summary>
		/// Copy array of bytes to somewhere in the font
		/// </summary>
		/// <param name="src">Source array</param>
		/// <param name="srcOffset">Offset in the source from where to start copying</param>
		/// <param name="destOffset">Offset in the font data where to start copying to</param>
		/// <param name="count">How many bytes to copy</param>
		public static void CopyTo(byte[] src, int srcOffset, int destOffset, int count)
		{
			Buffer.BlockCopy(src, srcOffset, FontBytes, destOffset, count);
		}

		/// <summary>
		/// Find the location in the font bytes where a specific characterIndex starts.
		/// </summary>
		/// <param name="characterIndex">0 - 511: One of the characters in a bank</param>
		/// <param name="onBank2">true when on the 2nd bank of fonts</param>
		/// <returns>Offset where the characterIndex data starts</returns>
		public static int GetCharacterOffset(int characterIndex, bool onBank2)
		{
			var ry = characterIndex / 32;
			var rx = characterIndex % 32;

			if (ry > 3 && ry < 12)
			{
				ry -= 4;
			}

			if (ry > 11 && ry < 16)
			{
				ry -= 8;
			}

			return ry * 32 * 8 + rx * 8 + (onBank2 ? 2048 : 0);
		}

		public static void RotateLeft(int characterIndex, bool onBank2)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);
			Array.Clear(OneCharacterBuffer);
			Buffer.BlockCopy(FontBytes, hp, OneCharacterBuffer, 0, 8);

			byte mask = 128;
			for (var i = 0; i < 8; ++i)
			{
				One8x8Buffer[0, 7 - i] = (byte)(OneCharacterBuffer[0] & mask);
				One8x8Buffer[1, 7 - i] = (byte)(OneCharacterBuffer[1] & mask);
				One8x8Buffer[2, 7 - i] = (byte)(OneCharacterBuffer[2] & mask);
				One8x8Buffer[3, 7 - i] = (byte)(OneCharacterBuffer[3] & mask);
				One8x8Buffer[4, 7 - i] = (byte)(OneCharacterBuffer[4] & mask);
				One8x8Buffer[5, 7 - i] = (byte)(OneCharacterBuffer[5] & mask);
				One8x8Buffer[6, 7 - i] = (byte)(OneCharacterBuffer[6] & mask);
				One8x8Buffer[7, 7 - i] = (byte)(OneCharacterBuffer[7] & mask);
				mask >>= 1;
			}

			for (var y = 0; y < 8; ++y)
			{
				mask = 128;
				byte v = 0;
				for (var x = 0; x < 8; ++x)
				{
					v = (byte)(v | ((One8x8Buffer[x, y] == 0) ? 0 : mask));
					mask >>= 1;
				}

				FontBytes[hp++] = v;
			}
		}
		public static void RotateRight(int characterIndex, bool onBank2)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);
			Array.Clear(OneCharacterBuffer);
			Buffer.BlockCopy(FontBytes, hp, OneCharacterBuffer, 0, 8);

			byte mask = 128;
			for (var i = 0; i < 8; ++i)
			{
				One8x8Buffer[0, i] = (byte)(OneCharacterBuffer[7] & mask);
				One8x8Buffer[1, i] = (byte)(OneCharacterBuffer[6] & mask);
				One8x8Buffer[2, i] = (byte)(OneCharacterBuffer[5] & mask);
				One8x8Buffer[3, i] = (byte)(OneCharacterBuffer[4] & mask);
				One8x8Buffer[4, i] = (byte)(OneCharacterBuffer[3] & mask);
				One8x8Buffer[5, i] = (byte)(OneCharacterBuffer[2] & mask);
				One8x8Buffer[6, i] = (byte)(OneCharacterBuffer[1] & mask);
				One8x8Buffer[7, i] = (byte)(OneCharacterBuffer[0] & mask);
				mask >>= 1;
			}

			for (var y = 0; y < 8; ++y)
			{
				mask = 128;
				byte v = 0;
				for (var x = 0; x < 8; ++x)
				{
					v = (byte)(v | ((One8x8Buffer[x, y] == 0) ? 0 : mask));
					mask >>= 1;
				}

				FontBytes[hp++] = v;
			}
		}

		// http://graphics.stanford.edu/~seander/bithacks.html#BitReverseTable
		public static void MirrorHorizontalMono(int characterIndex, bool onBank2)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);
			for (var i = 0; i < 8; ++i)
			{
				var v = FontBytes[hp];
				FontBytes[hp++] = (byte)(((v * 0x0802LU & 0x22110LU) | (v * 0x8020LU & 0x88440LU)) * 0x10101LU >> 16);
			}
		}
		public static void MirrorHorizontalColor(int characterIndex, bool onBank2)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);
			for (var i = 0; i < 8; ++i)
			{
				var v = FontBytes[hp];
				FontBytes[hp++] = (byte)(((v & 3) << 6) | ((v & 12) << 2) | ((v & 48) >> 2) | ((v & 192) >> 6));
			}
		}
		public static void MirrorVertical(int characterIndex, bool onBank2)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);

			Buffer.BlockCopy(FontBytes, hp, OneCharacterBuffer, 0, 8);

			for (var i = 7; i >= 0; --i)
			{
				FontBytes[hp++] = OneCharacterBuffer[i];
			}
		}

		public static void ShiftUp(int characterIndex, bool onBank2)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);
			var topByte = FontBytes[hp];
			for (var i = 0; i < 7; ++i, ++hp)
			{
				FontBytes[hp] = FontBytes[hp + 1];
			}

			FontBytes[hp] = topByte;
		}
		public static void ShiftDown(int characterIndex, bool onBank2)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);
			var bottomByte = FontBytes[hp + 7];
			for (var i = hp + 7; i > hp; --i)
			{
				FontBytes[i] = FontBytes[i - 1];
			}
			FontBytes[hp] = bottomByte;
		}

		public static void ShiftLeft(int characterIndex, bool onBank2, bool inColor)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);
			for (var i = 0; i < 8; ++i)
			{
				var v = FontBytes[hp];
				if ((v & 128) > 0)
				{
					v = (byte)((v << 1) | 1);
				}
				else
				{
					v = (byte)(v << 1);
				}

				if (inColor)
				{
					if ((v & 128) > 0)
					{
						v = (byte)((v << 1) | 1);
					}
					else
					{
						v = (byte)(v << 1);
					}
				}

				FontBytes[hp++] = v;
			}
		}
		public static void ShiftRight(int characterIndex, bool onBank2, bool inColor)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);
			for (var i = 0; i < 8; ++i)
			{
				var v = FontBytes[hp];
				if ((v & 1) > 0)
				{
					v = (byte)((v >> 1) | 128);
				}
				else
				{
					v = (byte)(v >> 1);
				}

				if (inColor)
				{
					if ((v & 1) > 0)
					{
						v = (byte)((v >> 1) | 128);
					}
					else
					{
						v = (byte)(v >> 1);
					}
				}

				FontBytes[hp++] = v;
			}
		}

		public static void InvertCharacter(int characterIndex, bool onBank2)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);

			for (var y = 0; y < 8; y++)
			{
				FontBytes[hp + y] = (byte)(FontBytes[hp + y] ^ 255);
			}
		}
		public static void ClearCharacter(int characterIndex, bool onBank2)
		{
			var hp = GetCharacterOffset(characterIndex, onBank2);

			for (var y = 0; y < 8; y++)
			{
				FontBytes[hp + y] = 0;
			}
		}

		public static byte[] DecodeMono(byte ln)
		{
			var op = new byte[8];

			for (var c = 7; c >= 0; c--)
			{
				op[c] = (byte)(ln % 2);
				ln = (byte)(ln >> 1);
			}

			return op;
		}

		public static byte[] DecodeColor(byte ln)
		{
			var op = new byte[4];

			for (var c = 3; c >= 0; c--)
			{
				op[c] = (byte)(ln % 4);
				ln = (byte)(ln >> 2);
			}

			return op;
		}

		public static byte EncodeMono(byte[] cd)
		{
			byte v = 128;
			byte c = 0;
			byte o = 0;
			do
			{
				if (cd[c] != 0)
					o |= v;
				c++;
				v = (byte)(v >> 1);
			}
			while (v != 0);

			return o;
		}

		public static byte EncodeColor(byte[] cd)
		{
			byte v = 64;
			byte c = 0;
			byte o = 0;

			do
			{
				o = (byte)(o + v * cd[c]);
				c++;
				v = (byte)(v >> 2);
			}
			while (v != 0);

			return o;
		}

		public static byte[,] Get5ColorCharacter(int character, bool onBank2)
		{
			var res = new byte[8, 8];
			var charPtr = GetCharacterOffset(character, onBank2);

			for (var i = 0; i < 8; i++)
			{
				var clData = DecodeColor(FontBytes[charPtr]);

				for (var j = 0; j < 4; j++)
				{
					res[j, i] = clData[j];
				}

				charPtr++;
			}

			return res;
		}

		public static byte[,] Get2ColorCharacter(int character, bool onBank2)
		{
			var res = new byte[8, 8];
			var charPtr = GetCharacterOffset(character, onBank2);

			for (var i = 0; i < 8; i++)
			{
				var clData = DecodeMono(FontBytes[charPtr + i]);

				for (var j = 0; j < 8; j++)
				{
					res[j, i] = clData[j];
				}
			}

			return res;
		}

		public static void Set2ColorCharacter(byte[,] pixels, int character, bool onBank2)
		{
			var charline2 = new byte[8];
			var fontByteIndex = GetCharacterOffset(character, onBank2);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 8; b++)
				{
					charline2[b] = pixels[b, a];
				}

				FontBytes[fontByteIndex + a] = EncodeMono(charline2);
			}
		}

		public static void Set5ColorCharacter(byte[,] character5Color, int character, bool onBank2)
		{
			var fourPixels = new byte[4];
			var fontByteIndex = GetCharacterOffset(character, onBank2);

			for (var y = 0; y < 8; y++)
			{
				for (var x = 0; x < 4; x++)
				{
					fourPixels[x] = character5Color[x, y];
				}

				FontBytes[fontByteIndex + y] = EncodeColor(fourPixels);
			}
		}

		public static void ShiftFontLeft(int characterIndex, bool onBank2, bool makeHole)
		{
			// Find out which font nr we are dealing with
			var hp = GetCharacterOffset(characterIndex, onBank2);
			var fontNr = hp / 1024; // 0,1,2,3
			var startOfFontData = fontNr * 1024;

			if (makeHole == false)
			{
				// No hole so save the first char's data
				Buffer.BlockCopy(FontBytes, startOfFontData, OneCharacterBuffer, 0, 8);
				// Move the whole font to the left (8 bytes)
				Buffer.BlockCopy(FontBytes, startOfFontData + 8, FontBytes, startOfFontData, 1024 - 8);
				// Copy back the first char to the last char
				Buffer.BlockCopy(OneCharacterBuffer, 0, FontBytes, startOfFontData + 1024 - 8, 8);
			}
			else
			{
				// Make a hole at the current location
				var length = hp - startOfFontData;
				if (length > 0)
				{
					Buffer.BlockCopy(FontBytes, startOfFontData+8, FontBytes, startOfFontData, length);
				}
				// Clear the char at the current location
				Array.Clear(FontBytes, hp, 8);
			}
		}

		public static void DeleteAndShiftRight(int characterIndex, bool onBank2)
		{
			// Find out which font nr we are dealing with
			var hp = GetCharacterOffset(characterIndex, onBank2);
			var fontNr = hp / 1024; // 0,1,2,3
			var startOfFontData = fontNr * 1024;

			// Make a hole at the current location
			var length = hp - startOfFontData;
			if (length > 0)
			{
				Buffer.BlockCopy(FontBytes, startOfFontData, FontBytes, startOfFontData + 8, length);
			}
			// Clear the char at the current location
			Array.Clear(FontBytes, startOfFontData, 8);
		}

		public static void DeleteAndShiftLeft(int characterIndex, bool onBank2)
		{
			// Find out which font nr we are dealing with
			var hp = GetCharacterOffset(characterIndex, onBank2);
			var fontNr = hp / 1024; // 0,1,2,3
			var startOfFontData = fontNr * 1024;
			var nextFontData = startOfFontData + 1024;
		
			var length = nextFontData - hp;
			if (length > 0)
			{
				Buffer.BlockCopy(FontBytes, hp + 8, FontBytes, hp, length - 8);
			}
			// Clear the char at the current location
			Array.Clear(FontBytes, nextFontData - 8, 8);
		}

		public static void ShiftFontRight(int characterIndex, bool onBank2, bool makeHole)
		{
			// Find out which font nr we are dealing with
			var hp = GetCharacterOffset(characterIndex, onBank2);
			var fontNr = hp / 1024; // 0,1,2,3
			var startOfFontData = fontNr * 1024;
			var nextFontData = startOfFontData + 1024;

			if (makeHole == false)
			{
				// No hole so save the last char's data
				Buffer.BlockCopy(FontBytes, nextFontData-8, OneCharacterBuffer, 0, 8);
				// Move the whole font to the right (8 bytes)
				Buffer.BlockCopy(FontBytes, startOfFontData, FontBytes, startOfFontData+8, 1024 - 8);
				// Copy back the last char to the first char
				Buffer.BlockCopy(OneCharacterBuffer, 0, FontBytes, startOfFontData, 8);
			}
			else
			{
				// Make a hole at the current location
				var length = nextFontData - hp;
				if (length > 0)
				{
					Buffer.BlockCopy(FontBytes, hp, FontBytes, hp + 8, length-8);
				}
				// Clear the char at the current location
				Array.Clear(FontBytes, hp, 8);
			}

		}

		#endregion
	}
}
