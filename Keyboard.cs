namespace FontMaker
{
	internal class Keyboard
	{
	}

	public partial class FontMakerForm
	{

		public void ExecuteSelectPreviousCharacter()
		{
			if (!buttonMegaCopy.Checked)
			{
				if (CharacterEdited())
				{
					UndoBuffer.Add2Undo(true);
				}
			}

			SelectedCharacterIndex--;
			if (SelectedCharacterIndex < 0) SelectedCharacterIndex += 512;
			SetCharCursor();
		}

		public void ExecuteSelectNextCharacter()
		{
			if (!buttonMegaCopy.Checked)
			{
				if (CharacterEdited())
				{
					UndoBuffer.Add2Undo(true);
				}
			}

			SelectedCharacterIndex++;
			if (SelectedCharacterIndex >= 512) SelectedCharacterIndex -= 512;
			SetCharCursor();
		}

		public void ExecuteEscapeKeyPressed()
		{
			if (buttonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case MegaCopyStatusFlags.None:
						break;

					case MegaCopyStatusFlags.Selecting:
						ResetMegaCopyStatus();
						break;

					case MegaCopyStatusFlags.Selected:
						break;

					case MegaCopyStatusFlags.Pasting:
					case MegaCopyStatusFlags.PastingView:
					case MegaCopyStatusFlags.PastingFont:
						ResetMegaCopyStatus();
						break;
				}
			}
		}
	}
}
