namespace FontMaker
{
	internal class Status
	{
	}

	// All code for section E - Status display and some buttons
	public partial class FontMakerForm
	{

		public void CheckDuplicate()
		{
			if ((checkBoxShowDuplicates.Checked == false) || (checkBoxShowDuplicates.Enabled == false))
			{
				return;
			}

			DuplicateCharacterIndex = SelectedCharacterIndex;

			if (FindDuplicateChar() != SelectedCharacterIndex)
			{
				timerDuplicates.Enabled = true;
			}
			else
			{
				timerDuplicates.Enabled = false;
				pictureBoxDuplicateIndicator.Visible = false;
			}
		}
	}
}
