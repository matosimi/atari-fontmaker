namespace FontMaker
{
	
	internal class General
	{
	}

	public partial class FontMakerForm
	{
		// Section A - General:  New, Load, Save, Save As, Clear, About and Quit buttons
		// Methods to implement the button actions

		#region Button and mouse event actions

		public void SimulateSafeLeftMouseButtonClick()
		{
			var old_megaCopyStatus = megaCopyStatus;
			megaCopyStatus = MegaCopyStatusFlags.None;
			var mouseEvent = new MouseEventArgs(MouseButtons.Left, 0, (SelectedCharacterIndex % 32) * 16, (SelectedCharacterIndex / 32) * 16, 0);
			ActionFontSelectorMouseDown(mouseEvent);
			ActionFontSelectorMouseUp(mouseEvent);
			megaCopyStatus = old_megaCopyStatus;
		}
		/// <summary>
		/// New the font and view data
		/// </summary>
		public void ActionNewFontAndView()
		{
			var re = MessageBox.Show(@"Are you sure you want to reset to the default character set and view? Everything will be lost!", Constants.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (re == DialogResult.Yes)
			{
				CurrentDataFolder = AppContext.BaseDirectory;
				LoadViewFile(null, true);
				checkBoxFontBank.Checked = false;
				UpdateFormCaption();
				RedrawFonts();
				RedrawLineTypes();
				RedrawView();
				RedrawPal();
				RedrawViewChar();
				RedrawChar();
				// SaveFont1_Click(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Load 1 or Load 3 button was clicked
		/// Load a font into bank 1 or 3, or in dual mode into banks 1+2 or 3+4
		/// </summary>
		public void ActionLoadFont1()
		{
			dialogOpenFile.FileName = string.Empty;
			dialogOpenFile.InitialDirectory = CurrentDataFolder;
			dialogOpenFile.Filter = $@"Atari font {(checkBoxFontBank.Checked ? 3 : 1)} or Dual font (*.fnt,*.fn2)|*.fnt;*.fn2";
			var ok = dialogOpenFile.ShowDialog();

			if (ok == DialogResult.OK)
			{
				var fontBankOffset = checkBoxFontBank.Checked ? 2 : 0;
				var dual = Path.GetExtension(dialogOpenFile.FileName) == ".fn2";

				AtariFont.LoadFont(dialogOpenFile.FileName, fontBankOffset, dual);

				CurrentDataFolder = Path.GetDirectoryName(dialogOpenFile.FileName) + Path.DirectorySeparatorChar;

				if (dual)
				{
					var tempstring = dialogOpenFile.FileName.Substring(0, dialogOpenFile.FileName.Length - 4);
					if (checkBoxFontBank.Checked == false)
					{
						Font1Filename = tempstring + "1.fnt";
						Font2Filename = tempstring + "2.fnt";
					}
					else
					{
						Font3Filename = tempstring + "3.fnt";
						Font4Filename = tempstring + "4.fnt";
					}
				}
				else
				{
					if (checkBoxFontBank.Checked == false)
						Font1Filename = dialogOpenFile.FileName;
					else
						Font3Filename = dialogOpenFile.FileName;
				}

				UpdateFormCaption();
				RedrawFonts();

				SimulateSafeLeftMouseButtonClick();

				RedrawView();
				UndoBuffer.Add2UndoFullDifferenceScan(); // Full font scan
				UpdateUndoButtons(false);
			}

			CheckDuplicate();
		}

		/// <summary>
		/// Load 2 or Load 3 button was clicked
		/// Load a font into bank 2 or 4
		/// </summary>
		public void ActionLoadFont2()
		{
			dialogOpenFile.FileName = string.Empty;
			dialogOpenFile.InitialDirectory = CurrentDataFolder;
			dialogOpenFile.Filter = $@"Atari font {(checkBoxFontBank.Checked ? 4 : 2)} (*.fnt)|*.fnt";
			var ok = dialogOpenFile.ShowDialog();

			if (ok == DialogResult.OK)
			{
				var fontBankOffset = checkBoxFontBank.Checked ? 2 : 0;

				AtariFont.LoadFont(dialogOpenFile.FileName, fontBankOffset + 1, false);

				CurrentDataFolder = Path.GetDirectoryName(dialogOpenFile.FileName) + Path.DirectorySeparatorChar;

				if (checkBoxFontBank.Checked == false)
					Font2Filename = dialogOpenFile.FileName;
				else
					Font4Filename = dialogOpenFile.FileName;

				UpdateFormCaption();
				RedrawFonts();
				SimulateSafeLeftMouseButtonClick();
				RedrawView();
				UndoBuffer.Add2UndoFullDifferenceScan(); //full font scan
				UpdateUndoButtons(false);
			}

			CheckDuplicate();
		}

		/// <summary>
		/// Save the font in bank 1/3 away
		/// </summary>
		public void ActionSaveFont1()
		{
			var filename = checkBoxFontBank.Checked ? Font3Filename : Font1Filename;
			AtariFont.SaveFont(filename, checkBoxFontBank.Checked == false ? 0 : 2);
		}

		/// <summary>
		/// Save the font in bank 2/4 away
		/// </summary>
		public void ActionSaveFont2()
		{
			var filename = checkBoxFontBank.Checked ? Font4Filename : Font2Filename;
			AtariFont.SaveFont(filename, checkBoxFontBank.Checked == false ? 1 : 3);
		}

		public void ActionSaveFont1As()
		{
			dialogSaveFile.FileName = string.Empty;
			dialogSaveFile.InitialDirectory = CurrentDataFolder;
			dialogSaveFile.Filter = $@"Atari font {(checkBoxFontBank.Checked ? 3 : 1)} (*.fnt)|*.fnt";
			dialogSaveFile.DefaultExt = "fnt";
			var ok = dialogSaveFile.ShowDialog();

			if (ok == DialogResult.OK)
			{
				AtariFont.SaveFont(dialogSaveFile.FileName, checkBoxFontBank.Checked == false ? 0 : 2);
				CurrentDataFolder = Path.GetDirectoryName(dialogSaveFile.FileName) + Path.DirectorySeparatorChar;

				if (checkBoxFontBank.Checked == false)
					Font1Filename = dialogSaveFile.FileName;
				else
					Font3Filename = dialogSaveFile.FileName;
			}

			UpdateFormCaption();
		}

		public void ActionSaveFont2As()
		{
			dialogSaveFile.FileName = string.Empty;
			dialogSaveFile.InitialDirectory = CurrentDataFolder;
			dialogSaveFile.Filter = $@"Atari font {(checkBoxFontBank.Checked ? 4 : 2)} (*.fnt)|*.fnt";
			dialogSaveFile.DefaultExt = "fnt";
			var ok = dialogSaveFile.ShowDialog();

			if (ok == DialogResult.OK)
			{
				AtariFont.SaveFont(dialogSaveFile.FileName, checkBoxFontBank.Checked == false ? 1 : 3);

				CurrentDataFolder = Path.GetDirectoryName(dialogSaveFile.FileName) + Path.DirectorySeparatorChar;

				if (checkBoxFontBank.Checked == false)
					Font2Filename = dialogSaveFile.FileName;
				else
					Font4Filename = dialogSaveFile.FileName;
			}

			UpdateFormCaption();
		}

		public void ActionClearFont(int fontOffset)
		{
			var fontBankOffset = checkBoxFontBank.Checked ? 2 : 0;

			var re = MessageBox.Show($@"Are you sure to clear font {fontOffset + 1 + fontBankOffset}?", Constants.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (re == DialogResult.Yes)
			{
				ClearFont(fontOffset + 0 + fontBankOffset);
			}
		}

		public void ActionShowAbout()
		{
			pictureBoxAbout.Left = pictureBoxAtariView.Left + (checkBox40Bytes.Checked ? (pictureBoxAtariView.Width - pictureBoxAbout.Width)/2 : 0);
			pictureBoxAbout.Visible = !pictureBoxAbout.Visible;

		}

		public void ActionAboutUrl(bool onLeftSide)
		{
			Helpers.OpenUrl(onLeftSide ? "http://matosimi.atari.org" : "https://retro.cerebus.co.za");
			pictureBoxAbout.Visible = false;
		}

		public void ActionExitApplication()
		{
			var re = MessageBox.Show(@"Are you sure you want to quit?", Constants.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (re == DialogResult.Yes)
			{
				SaveConfiguration();
				Exit();
			}
		}

		#endregion


		private void ClearFont(int fontNr)
		{
			AtariFont.ClearFont(fontNr);

			RedrawFonts();
			SimulateSafeLeftMouseButtonClick();
			RedrawView();
			CheckDuplicate();
		}

		public void Exit()
		{
			Visible = false;
			timerAutoCloseAboutBox.Enabled = false;
			pictureBoxAbout.Visible = false;
			Environment.Exit(Environment.ExitCode);
		}
	}
}
