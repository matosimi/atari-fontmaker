namespace FontMaker
{
	public static class UndoBuffer
	{
		public const int UndoBufferSize = 250;

		public static byte[,] undoBuffer = new byte[UndoBufferSize + 1, AtariFont.NumFontBytes];

		public static int[] undoBufferFlags = new int[UndoBufferSize + 1];
		public static int undoBufferIndex = 0;

		public static void Setup()
		{
			undoBufferIndex = 0;
			for (var i = 0; i <= UndoBufferSize; i++)
			{
				undoBufferFlags[i] = -1;
			}
		}

		public static void Add2UndoInitial()
		{
			var prevUndoIndex = undoBufferIndex;

			for (var i = 0; i < AtariFont.NumFontBytes; i++)
			{
				undoBuffer[undoBufferIndex, i] = AtariFont.FontBytes[i];
			}

			undoBufferFlags[undoBufferIndex] = undoBufferFlags[prevUndoIndex] + 1;
			undoBufferFlags[(undoBufferIndex + 1) % UndoBufferSize] = -1; //disallow redo when change
		}

		public static void Add2Undo(bool difference)
		{
			if (difference)
			{
				var prevUndoIndex = undoBufferIndex;
				undoBufferIndex = (undoBufferIndex + 1) % UndoBufferSize; // size of undo buffer

				for (var i = 0; i < AtariFont.NumFontBytes; i++)
				{
					undoBuffer[undoBufferIndex, i] = AtariFont.FontBytes[i];
				}

				undoBufferFlags[undoBufferIndex] = undoBufferFlags[prevUndoIndex] + 1;
				undoBufferFlags[(undoBufferIndex + 1) % UndoBufferSize] = -1; // disallow redo when change
			}
		}

		public static void Add2UndoFullDifferenceScan()
		{
			var i = 0;
			var difference = false;

			// check the difference between last undo buffer and current font
			while ((i < AtariFont.NumFontBytes) && (!difference))
			{
				if (AtariFont.FontBytes[i] != undoBuffer[undoBufferIndex, i])
				{
					difference = true;
				}

				i++;
			}

			if (difference)
			{
				UndoBuffer.Add2Undo(true);
			}

			// Update the GUI state now
		}


		public static int GetPrevUndoIndex()
		{
			return undoBufferIndex - 1 < 0 ? UndoBufferSize - 1 : undoBufferIndex - 1;
		}

		public static void Undo()
		{
			var prevUndoIndex = GetPrevUndoIndex();

			for (var i = 0; i < AtariFont.NumFontBytes; i++)
			{
				AtariFont.FontBytes[i] = undoBuffer[prevUndoIndex, i];
			}

			undoBufferIndex = prevUndoIndex;
		}

		public static void Redo()
		{
			var nextUndoIndex = (undoBufferIndex + 1) % UndoBufferSize;

			if (undoBufferFlags[nextUndoIndex] > -1)
			{
				for (var i = 0; i < AtariFont.NumFontBytes; i++)
				{
					AtariFont.FontBytes[i] = undoBuffer[nextUndoIndex, i];
				}
			}

			undoBufferIndex = nextUndoIndex;
		}

		public static (bool, bool) GetRedoUndoButtonState(bool edited)
		{
			bool redoButtonEnabled;
			bool undoButtonEnabled;

			var nextUndoBufferIndex = (undoBufferIndex + 1) % UndoBufferSize;
			var prevUndoBufferIndex = GetPrevUndoIndex();

			// redo button handling
			if (undoBufferFlags[nextUndoBufferIndex] == -1)
			{
				redoButtonEnabled = false;
			}
			else if (edited)
			{
				redoButtonEnabled = false;
			}
			else
			{
				redoButtonEnabled = true;
			}

			// undo button handling
			if (edited)
			{
				undoButtonEnabled = true;
			}
			else if ((undoBufferFlags[undoBufferIndex] > undoBufferFlags[prevUndoBufferIndex]) && (undoBufferFlags[prevUndoBufferIndex] > -1))
			{
				undoButtonEnabled = true;
			}
			else
			{
				undoButtonEnabled = false;
			}

			return (redoButtonEnabled, undoButtonEnabled);
		}
	}
}
