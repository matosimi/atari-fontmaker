using System.Drawing.Drawing2D;
using System.Media;

namespace FontMaker
{
	// FontMaker GUI is broken into 6 parts
	// +--------+------------------+----------+--------------------------------------+
	// | A      | B                | C        | D                                    |
	// | New    | Character Editor | GFX mode | View/Screen editor                   |
	// | Load...|                  | Recolor  | Paging                               |
	// +--------+------------------+----------|                                      |
	// | E                                    |                                      |
	// | Undo/Redo/Duplicate/MegaCopy/FontBank|                                      |
	// +--------------------------------------|                                      |
	// | F                                    |                                      |
	// | Font selector                        |                                      |
	// |                                      |                                      |
	// +--------------------------------------+--------------------------------------+
	//
	// A - General actions - New, Load, Save, Save As, Clear, About and Quit buttons
	// B - Character editor - Edit an individual character (with copy paste)
	// C - Colors - Switch between mono and color, edit colors etc (with export)
	// D - View editor 40x26 - Edit a sample screen with multiple pages
	// E - Status area - Redo, undo, check duplicates, mega copy, font bank selector
	// F - Font selector - Select one of the 512 characters in a font bank
	//
	// Section A handlers are located in the Generals.cs file
	// Section B handlers are located in the CharacterEditor.cs file
	// Section C handlers are located in the Colors.cs file
	// Section D handlers are located in the AtariViewEditor.cs file
	// Section E handlers are located in the Status.cs file
	// Section F handlers are located in the FontSelector.cs file

	/// <summary>
	/// Main form of the Atari FontMaker application
	/// </summary>
	public partial class FontMakerForm : Form
	{

		public enum MegaCopyStatusFlags
		{
			None, Selecting, Selected, Pasting
		}

		internal MegaCopyStatusFlags megaCopyStatus = MegaCopyStatusFlags.None;

		private static readonly SolidBrush blackBrush = new(Color.Black);
		private static readonly SolidBrush whiteBrush = new(Color.White);
		private static readonly SolidBrush cyanBrush = new(Color.Cyan);
		private static readonly SolidBrush redBrush = new(Color.Red);
		private static readonly SolidBrush yellowBrush = new(Color.Yellow);
		private static readonly SolidBrush greenBrush = new(Color.Green);

		private readonly List<Button> ActionListNormalModeOnly = new();

		#region All data used by Atari Font Maker
		public AtariColorSelectorForm AtariColorSelector { get; set; } = new AtariColorSelectorForm();
		public ExportWindow ExportWindowForm { get; set; } = new ExportWindow();

		/// <summary>
		/// The Atari color palette. Loaded from "altirraPAL.pal"
		/// </summary>
		internal Color[] AtariPalette { get; set; } = new Color[256];
		internal bool InColorMode { get; set; } = false;
		internal int ActiveColorNr { get; set; }
		/// <summary>
		/// The 6 active colors: These are indexes into the Atari color palette
		/// 2 for mono (index 0 + 1)
		/// 5 for color (index 1, 2, 3, 4, 5 [inverse of 4])
		/// </summary>
		internal byte[] SetOfSelectedColors { get; set; } = new byte[6];
		internal SolidBrush[] BrushCache { get; set; } = new SolidBrush[6];


		internal string CurrentDataFolder { get; set; } = string.Empty;
		internal string Font1Filename { get; set; } = string.Empty;
		internal string Font2Filename { get; set; } = string.Empty;
		internal string Font3Filename { get; set; } = string.Empty;
		internal string Font4Filename { get; set; } = string.Empty;


		/// <summary>
		/// Index of the selected character in the font bank (0-511)
		/// </summary>
		internal int SelectedCharacterIndex { get; set; } = 0;          // The current character 0 - 511
		/// <summary>
		/// Index of the last duplicate character found
		/// </summary>
		internal int DuplicateCharacterIndex { get; set; } = 0;

		/// <summary>
		/// Location where the currently copied data will be pasted to
		/// </summary>
		private Point CopyPasteTargetLocation;

		private Rectangle CopyPasteRange;

		#endregion



		public FontMakerForm()
		{
			InitializeComponent();

			// Admin the action list
			ActionListNormalModeOnly.Add(buttonRotateLeft);
			ActionListNormalModeOnly.Add(buttonRotateRight);
			ActionListNormalModeOnly.Add(buttonMirrorHorizontal);
			ActionListNormalModeOnly.Add(buttonMirrorVertical);
			ActionListNormalModeOnly.Add(buttonShiftLeft);
			ActionListNormalModeOnly.Add(buttonShiftRight);
			ActionListNormalModeOnly.Add(buttonShiftUp);
			ActionListNormalModeOnly.Add(buttonShiftDown);
			ActionListNormalModeOnly.Add(buttonInverse);
			ActionListNormalModeOnly.Add(buttonClear);
			ActionListNormalModeOnly.Add(buttonRestoreSaved);
			ActionListNormalModeOnly.Add(buttonRestoreDefault);

			this.Load += FormCreate!;
		}

		private void FormCreate(object sender, EventArgs e)
		{
			// Init the gui
			DoubleBuffered = true;
			ActiveColorNr = 2;
			SelectedCharacterIndex = 0;
			comboBoxWriteMode.SelectedIndex = 0;

			CurrentDataFolder = AppContext.BaseDirectory;

			UndoBuffer.Setup();
			AtariView.Setup();
			LoadPalette();

			string? ext;
			if (Environment.GetCommandLineArgs().Length - 1 == 1)
			{
				var loadPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[1]) + Path.DirectorySeparatorChar;
				ext = Path.GetExtension(Environment.GetCommandLineArgs()[1]).ToLower();

				switch (ext)
				{
					case ".fn2":
						{
							// TODO: Load a .fn2 file
							// It has 2048 bytes and effectively contains two fonts
							//Load_font(Environment.GetCommandLineArgs()[1], 0, true);
							//tempstring = Environment.GetCommandLineArgs()[1].Substring(-1, Environment.GetCommandLineArgs()[1].Length - 4);
							//Font1Filename = tempstring + "1.fnt";
							//Font2Filename = tempstring + "2.fnt";
						}
						break;

					case ".fnt":
						{
							Font1Filename = Environment.GetCommandLineArgs()[1];
						}
						break;

					case ".atrview":
						{
							LoadViewFile(Environment.GetCommandLineArgs()[1], true);
							UpdateFormCaption();
							RedrawFonts();
							RedrawLineTypes();
							RedrawView();
							RedrawPal();
							RedrawViewChar();
							RedrawChar();
							Font2Filename = Path.Join(loadPath, "Default.fnt");
						}
						break;
					default:
						Font1Filename = Environment.GetCommandLineArgs()[1];
						break;
				}

				timerAutoCloseAboutBox.Enabled = false;
				pictureBoxAbout.Visible = false;
			}
			else
			{
				// If no input file set upon start, then show splash screen
				timerAutoCloseAboutBox.Enabled = true;
				pictureBoxAbout.Visible = true;

				LoadViewFile(null, true);
				UpdateFormCaption();
				RedrawFonts();
				RedrawLineTypes();
				RedrawView();
				RedrawPal();
				RedrawViewChar();
				RedrawChar();
				ext = ".atrview";
				//SaveFont1_Click(null, EventArgs.Empty);
			}

			if (ext != ".atrview")
			{
				SetupDefaultPalColors();
				RedrawPal();
				RedrawGrid();

				if (ext != ".fn2")
				{
					// Load each of the fonts into the banks
					AtariFont.LoadFont(Font1Filename, 0, false);
					AtariFont.LoadFont(Font2Filename, 1, false);
					AtariFont.LoadFont(Font3Filename, 2, false);
					AtariFont.LoadFont(Font4Filename, 3, false);

					UpdateFormCaption();
				}
				else
				{
					UpdateFormCaption();
				}

				RedrawFonts();
				DefaultView();
				RedrawView();
			}

			listBoxRecolorSource.SelectedIndex = 0;
			listBoxRecolorTarget.SelectedIndex = 0;
			RedrawRecolorSource();
			RedrawRecolorTarget();

			UndoBuffer.Add2UndoInitial(); // initial undo buffer entry
			UpdateUndoButtons(false);

			MakeSomeColoredBlocks();

			ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 0, (SelectedCharacterIndex % 32) * 16, (SelectedCharacterIndex / 32) * 16, 0));
			CheckDuplicate();
		}

		private void MakeSomeColoredBlocks()
		{
			// Paint something into the window
			var img = Helpers.GetImage(pictureBoxFontSelectorRubberBand);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(redBrush, new Rectangle(0, 0, img.Width, img.Height));
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			pictureBoxFontSelectorRubberBand.Region = new Region(graphicsPath);
			pictureBoxFontSelectorRubberBand.Refresh();

			// Paint something into the window
			img = Helpers.GetImage(pictureBoxViewEditorRubberBand);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(cyanBrush, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			pictureBoxViewEditorRubberBand.Region = new Region(graphicsPath);

			pictureBoxViewEditorRubberBand.Refresh();

			// Paint something into the window
			img = Helpers.GetImage(pictureBoxFontSelectorPasteCursor);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(greenBrush, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			pictureBoxFontSelectorPasteCursor.Region = new Region(graphicsPath);
			pictureBoxFontSelectorPasteCursor.Refresh();

			// Paint something into the window
			img = Helpers.GetImage(pictureBoxViewEditorPasteCursor);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(yellowBrush, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			pictureBoxViewEditorPasteCursor.Region = new Region(graphicsPath);
			pictureBoxViewEditorPasteCursor.Refresh();

			// Paint something into the window
			img = Helpers.GetImage(pictureBoxDuplicateIndicator);
			using (var gr = Graphics.FromImage(img))
			{
				var trans = new SolidBrush(Color.Purple);
				gr.FillRectangle(trans, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));
			for (var x = 0; x < 20; ++x)
			{
				graphicsPath.AddRectangle(new Rectangle(x, x, 2, 2));
				graphicsPath.AddRectangle(new Rectangle(18 - x, x, 2, 2));
			}

			pictureBoxDuplicateIndicator.Region = new Region(graphicsPath);
			pictureBoxDuplicateIndicator.Refresh();
		}


		public void UpdateFormCaption()
		{
			Text = $@"{Constants.Title} v{Helpers.GetBuildInfoAsString()} - {Path.GetFileName(Font1Filename)}/{Path.GetFileName(Font2Filename)}/{Path.GetFileName(Font3Filename)}/{Path.GetFileName(Font4Filename)}";
			buttonSaveFont1.Visible = true;
			buttonSaveFont2.Visible = true;
		}

		#region Keyboard shotcuts

		private void Form_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Oemcomma)
			{
				ExecuteSelectPreviousCharacter();
				return;
			}

			if (e.KeyCode == Keys.OemPeriod)
			{
				ExecuteSelectNextCharacter();
				return;
			}

			if (e.KeyCode == Keys.R)
			{
				if (e.Shift)
					ExecuteRotateRight();
				else
					ExecuteRotateLeft();
				return;
			}

			if (e.KeyCode == Keys.M)
			{
				if (e.Shift)
					ExecuteMirrorVertical();
				else
					ExecuteMirrorHorizontal();
				return;
			}

			if (e.KeyCode == Keys.D1)
			{
				SetColor(2);
				return;
			}
			if (e.KeyCode == Keys.D2)
			{
				SetColor(3);
				return;
			}
			if (e.KeyCode == Keys.D3)
			{
				SetColor(4);
				return;
			}

			if (e.KeyCode == Keys.I)
			{
				ExecuteInvertCharacter();
				return;
			}

			if (e.KeyCode == Keys.Escape)
			{
				ExecuteEscapeKeyPressed();
				return;
			}

			if (e.Control && e.KeyCode == Keys.C)
			{
				ExecuteCopyToClipboard(false);
				return;
			}

			if (e.Control && e.KeyCode == Keys.V)
			{
				ExecutePasteFromClipboard();
				return;
			}

			if (e.Control && e.KeyCode == Keys.Z)
			{
				Undo_Click(0, EventArgs.Empty);
				return;
			}
			// Ctrl + Y = Redo font change
			if (e.Control && e.KeyCode == Keys.Y)
			{
				Redo_Click(0, EventArgs.Empty);
				return;
			}
		}
		#endregion

		#region Mouse Wheel

		private void Form_MouseWheel(object sender, MouseEventArgs e)
		{
			if (!buttonMegaCopy.Checked)
			{
				if (Control.ModifierKeys == Keys.Shift)
				{
					if (e.Delta > 0)
					{
						ActionCharacterEditorColor1MouseDown();
					}
					else
					{
						ActionCharacterEditorColor2MouseDown();
					}
				}
				else
				{
					int nextCharacterIndex;
					if (e.Delta > 0)
					{
						nextCharacterIndex = SelectedCharacterIndex - 1;
					}
					else
					{
						nextCharacterIndex = SelectedCharacterIndex + 1;
					}

					nextCharacterIndex = nextCharacterIndex % 512;
					if (nextCharacterIndex < 0)
						nextCharacterIndex += 512;

					var bx = nextCharacterIndex % 32;
					var by = nextCharacterIndex / 32;
					ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0));
				}
			}
		}

		#endregion


		// ==========================================================================
		// General font button handlers -> General.cs
		// New, Load, Save, Save As, Clear, About, Quit
		// ==========================================================================
		#region Section A - Font New/Load/Save/About/Quit Event Handlers
		public void New_Click(object _, EventArgs __)
		{
			ActionNewFontAndView();
		}
		public void LoadFont1_Click(object _, EventArgs __)
		{
			ActionLoadFont1();
		}
		public void LoadFont2_Click(object _, EventArgs __)
		{
			ActionLoadFont2();
		}
		public void SaveFont1_Click(object _, EventArgs __)
		{
			ActionSaveFont1();
		}
		public void SaveFont2_Click(object _, EventArgs __)
		{
			ActionSaveFont2();
		}
		public void SaveFont1As_Click(object _, EventArgs __)
		{
			ActionSaveFont1As();
		}
		public void SaveFont2As_Click(object _, EventArgs __)
		{
			ActionSaveFont2As();
		}
		public void ClearFont1_Click(object _, EventArgs __)
		{
			ActionClearFont(0);
		}
		public void ClearFont2_Click(object _, EventArgs __)
		{
			ActionClearFont(1);
		}
		public void About_Click(object sender, EventArgs __)
		{
			ActionShowAbout();
		}
		public void AboutPicture_MouseDown(object _, MouseEventArgs __)
		{
			ActionAboutUrl();
		}
		public void Quit_Click(object _, EventArgs __)
		{
			ActionExitApplication();
		}
		private void Form_CloseQuery(object _, FormClosingEventArgs e)
		{
			e.Cancel = true;
			ActionExitApplication();
		}
		#endregion

		// ==========================================================================
		// Character editor event handlers -> CharacterEdit.cs
		// ==========================================================================
		#region Section B - Character Editor Event Handlers

		#region Events on the left column of buttons: Color selector, Rotate Left, Horizontal Mirror, Shift Left, Shift Down, Restore Default, Restore Last Saved, Copy

		public void CharacterEditor_Color1_MouseDown(object sender, MouseEventArgs e)
		{
			ActionCharacterEditorColor1MouseDown();
		}
		public void CharacterEditor_RotateLeft_Click(object sender, EventArgs e)
		{
			ExecuteRotateLeft();
		}
		public void CharacterEditor_MirrorHorizontal_Click(object sender, EventArgs e)
		{
			ExecuteMirrorHorizontal();
		}
		public void CharacterEditor_ShiftLeft_Click(object sender, EventArgs e)
		{
			ExecuteShiftLeft();
		}
		public void CharacterEditor_ShiftDown_Click(object sender, EventArgs e)
		{
			ExecuteShiftDown();
		}
		public void CharacterEditor_RestoreDefault_Click(object sender, EventArgs e)
		{
			ActionCharacterEditorRestoreDefault();
		}
		public void CharacterEditor_RestoreSaved_Click(object sender, EventArgs e)
		{
			ActionCharacterEditorRestoreSaved();
		}
		public void CopyToClipboard_Click(object sender, EventArgs e)
		{
			ExecuteCopyToClipboard(false);
		}

		#endregion // Left button column

		#region Events of the character central display (large 8x8 editor)
		public void CharacterEditor_MouseDown(object _, MouseEventArgs e)
		{
			ActionCharacterEditorMouseDown(e);
		}

		public void CharacterEditor_MouseMove(object sender, MouseEventArgs e)
		{
			ActionCharacterEditorMouseMove(e);
		}

		public void CharacterEditor_MouseUp(object sender, MouseEventArgs e)
		{
			ActionCharacterEditorMouseUp();
		}
		#endregion

		#region Events on the right column of buttons: Color selector, Rotate Right, Vertical Mirrot, Shift Right, Shift Up, Invert, Clear, Paste
		public void CharacterEditor_Color2_MouseDown(object sender, MouseEventArgs e)
		{
			ActionCharacterEditorColor2MouseDown();
		}
		public void CharacterEditor_RotateRight_Click(object sender, EventArgs e)
		{
			ExecuteRotateRight();
		}
		public void CharacterEditor_MirrorVertical_Click(object sender, EventArgs e)
		{
			ExecuteMirrorVertical();
		}
		public void CharacterEditor_ShiftRight_Click(object sender, EventArgs e)
		{
			ExecuteShiftRight();
		}
		public void CharacterEditor_ShiftUp_Click(object sender, EventArgs e)
		{
			ExecuteShiftUp();
		}
		public void CharacterEditor_Inverse_Click(object sender, EventArgs e)
		{
			ExecuteInvertCharacter();
		}
		public void CharacterEditor_Clear_Click(object sender, EventArgs e)
		{
			ExecuteClearCharacter();
		}
		public void PasteFromClipboard_Click(object sender, EventArgs e)
		{
			ExecutePasteFromClipboard();
		}
		#endregion // Right button column

		#endregion

		// ==========================================================================
		// Area just to the right of the character editor
		// Handle graphics mode switch, color AtariPalette selection, recolor and export
		// ==========================================================================
		#region Section C - Color Change Event Handlers

		public void SwitchGraphicsMode_Click(object sender, EventArgs e)
		{
			SwitchGfxMode();
		}

		/// <summary>
		/// Show the Atari color AtariPalette and let the user select a new color
		/// </summary>
		public void Palette_MouseDown(object _, MouseEventArgs e)
		{
			InteractWithTheColorPalette(e);
		}

		public void Recolor_Click(object _, EventArgs __)
		{
			ColorSwitch(listBoxRecolorSource.SelectedIndex, listBoxRecolorTarget.SelectedIndex);
		}

		public void ShowColorSwitchSetup_Click(object _, EventArgs __)
		{
			panelColorSwitcher.Visible = !panelColorSwitcher.Visible;
		}

		public void Export_Click(object _, EventArgs __)
		{
			ExportWindowForm.ShowDialog();
		}

		public void RecolorSource_Click(object _, EventArgs __)
		{
			RedrawRecolorSource();
		}

		public void RecolorTarget_Click(object _, EventArgs __)
		{
			RedrawRecolorTarget();
		}

		#endregion

		// ==========================================================================
		// Atari View Editor event handlers
		// Right side of the app
		// Draw on the view, handle load/save, handle paging
		// ==========================================================================
		#region Section D - Atari View Editor event handlers

		/// <summary>
		/// Select which character set is to be used on a specific line of the screen
		/// </summary>
		public void ViewEditor_CharacterSetSelector_MouseDown(object sender, MouseEventArgs e)
		{
			ActionCharacterSetSelector(e);

		}
		private void ViewEditor_EnterText_Click(object sender, EventArgs e)
		{
			ActionEnterText();
		}

		public void ViewEditor_CheckBox40Bytes_Click(object sender, EventArgs e)
		{
			Width += (checkBox40Bytes.Checked ? 130 : -130);
		}

		public void ViewEditor_ClearView_Click(object sender, EventArgs e)
		{
			ActionClearView();
		}

		public void ViewEditor_LoadView_Click(object sender, EventArgs e)
		{
			ActionLoadView();
		}

		public void ViewEditor_SaveView_Click(object sender, EventArgs e)
		{
			ActionSaveView();
		}

		/// <summary>
		/// Handle the Pages combo box changing its selection
		/// </summary>
		public void ViewEditor_Pages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (InPagesSetup) return;

			SwopPage(saveCurrent: true);

			UpdatePageDisplay();
		}

		public void ViewEditor_AddPage_Click(object sender, EventArgs e)
		{
			ActionAddPage();
		}

		private void ViewEditor_DeletePage_Click(object sender, EventArgs e)
		{
			ActionDeletePage();
		}

		private void ViewEditor_EditPage_Click(object sender, EventArgs e)
		{
			ActionEditPages();
		}

		private void ViewEditor_DoubleClick(object sender, MouseEventArgs e)
		{
			ActionAtariViewDoubleClick(e);
		}

		public void ViewEditor_MouseDown(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseDown(e);
		}

		public void ViewEditor_MouseUp(object sender, MouseEventArgs e)
		{
			ActionAtariViewMouseUp(e);
		}

		public void ViewEditor_MouseMove(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseMove(e);
		}

		public void ViewEditor_MegaCopyImage_MouseDown(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseDown(new MouseEventArgs(e.Button, 0, e.X + pictureBoxViewEditorMegaCopyImage.Left - pictureBoxAtariView.Left, e.Y + pictureBoxViewEditorMegaCopyImage.Top - pictureBoxAtariView.Top, 0));
		}
		public void ViewEditor_MegaCopyImage_MouseMove(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseMove(new MouseEventArgs(MouseButtons.None, 0, e.X + pictureBoxViewEditorMegaCopyImage.Left - pictureBoxAtariView.Left, e.Y + pictureBoxViewEditorMegaCopyImage.Top - pictureBoxAtariView.Top, 0));
		}

		private void ViewEditor_MegaCopyImage_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			// Setup selection by right DoubleClick
			if (e.Button == MouseButtons.Right)
			{
				ResetMegaCopyStatus();
			}
		}

		public void ViewEditor_RubberBand_MouseDown(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseDown(new MouseEventArgs(e.Button, 0, e.X + pictureBoxViewEditorRubberBand.Left - pictureBoxAtariView.Left, e.Y + pictureBoxViewEditorRubberBand.Top - pictureBoxAtariView.Top, 0));
		}
		public void ViewEditor_RubberBand_MouseMove(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseMove(new MouseEventArgs(e.Button, 0, e.X + pictureBoxViewEditorRubberBand.Left - pictureBoxAtariView.Left, e.Y + pictureBoxViewEditorRubberBand.Top - pictureBoxAtariView.Top, 0));
		}
		public void ViewEditor_RubberBand_MouseUp(object sender, MouseEventArgs e)
		{
			ActionAtariViewMouseUp(new MouseEventArgs(e.Button, 0, e.X + pictureBoxViewEditorRubberBand.Left - pictureBoxAtariView.Left, e.Y + pictureBoxViewEditorRubberBand.Top - pictureBoxAtariView.Top, 0));
		}

		private static bool _busyWith_ViewEditorRubberBandResize = false;
		private void ViewEditor_RubberBand_Resize(object sender, EventArgs e)
		{
			if (_busyWith_ViewEditorRubberBandResize)
				return;
			_busyWith_ViewEditorRubberBandResize = true;

			var img = Helpers.NewImage(pictureBoxViewEditorRubberBand);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(cyanBrush, new Rectangle(0, 0, img.Width, img.Height));
				pictureBoxViewEditorRubberBand.Region?.Dispose();
				pictureBoxViewEditorRubberBand.Size = new Size(img.Width, img.Height);

			}
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, pictureBoxViewEditorRubberBand.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(pictureBoxViewEditorRubberBand.Width - 2, 0, 2, pictureBoxViewEditorRubberBand.Height));
			graphicsPath.AddRectangle(new Rectangle(0, pictureBoxViewEditorRubberBand.Height - 2, pictureBoxViewEditorRubberBand.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, pictureBoxViewEditorRubberBand.Height));
			pictureBoxViewEditorRubberBand.Region = new Region(graphicsPath);

			_busyWith_ViewEditorRubberBandResize = false;
		}

		public void ViewEditor_PasteCursor_MouseDown(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseDown(new MouseEventArgs(e.Button, 0, pictureBoxViewEditorPasteCursor.Left + e.X - pictureBoxAtariView.Left, pictureBoxViewEditorPasteCursor.Top + e.Y - pictureBoxAtariView.Top, 0));
		}
		public void ViewEditor_PasteCursor_MouseLeave(object sender, EventArgs e)
		{
			pictureBoxViewEditorPasteCursor.Visible = false;
			pictureBoxViewEditorMegaCopyImage.Visible = false;
		}
		public void ViewEditor_PasteCursor_MouseMove(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseMove(new MouseEventArgs(e.Button, 0, pictureBoxViewEditorPasteCursor.Left + e.X - pictureBoxAtariView.Left, pictureBoxViewEditorPasteCursor.Top + e.Y - pictureBoxAtariView.Top, 0));
		}
		public void ViewEditor_PasteCursor_MouseUp(object sender, MouseEventArgs e)
		{
			ActionAtariViewMouseUp(new MouseEventArgs(e.Button, 0, pictureBoxViewEditorPasteCursor.Left + e.X - pictureBoxAtariView.Left, pictureBoxViewEditorPasteCursor.Top + e.Y - pictureBoxAtariView.Top, 0));
		}

		#endregion

		// ==========================================================================
		// Area just above the font selector
		// Handle the Undo,Redo, duplicate, mega-copy and font bank events
		// ==========================================================================
		#region Section E - Under Character Editor
		public void Undo_Click(object sender, EventArgs e)
		{
			if (!ExecuteUndo())
			{
				SystemSounds.Beep.Play();
			}
		}

		public void Redo_Click(object sender, EventArgs e)
		{
			if (!Redo())
			{
				SystemSounds.Beep.Play();
			}
		}

		public void ShowDuplicates_Click(object sender, EventArgs e)
		{
			if (checkBoxShowDuplicates.Checked == false)
			{
				timerDuplicates.Enabled = false;
				pictureBoxDuplicateIndicator.Visible = false;
			}
			else
			{
				CheckDuplicate();
			}
		}

		public void MegaCopy_Click(object sender, EventArgs e)
		{
			// enable/disable actions between modes
			var ena = buttonMegaCopy.Checked;
			buttonEnterText.Enabled = ena;
			// hide character edit window
			pictureBoxCharacterEditor.Visible = !ena;

			// hide recolor if in mega copy mode
			if (ena && panelColorSwitcher.Visible)
			{
				ShowColorSwitchSetup_Click(0, EventArgs.Empty);
			}

			foreach (var item in ActionListNormalModeOnly)
			{
				item.Enabled = !ena;
			}

			if (ena)
			{
				// MegaCopy mode on
				megaCopyStatus = MegaCopyStatusFlags.None;
				buttonRecolor.Enabled = false;
				buttonShowColorSwitchSetup.Enabled = false;
				checkBoxShowDuplicates.Enabled = false;
				timerDuplicates.Enabled = false;
				pictureBoxDuplicateIndicator.Visible = false;

				pictureBoxViewEditorRubberBand.Left = pictureBoxAtariView.Left;
				pictureBoxViewEditorRubberBand.Top = pictureBoxAtariView.Top;
				pictureBoxViewEditorRubberBand.Visible = true;
			}
			else
			{
				// MegaCopy mode off
				// Turn the GUI back on
				pictureBoxFontSelectorRubberBand.Width = 20;
				pictureBoxFontSelectorRubberBand.Height = 20;
				pictureBoxFontSelectorRubberBand.Visible = true;
				pictureBoxViewEditorRubberBand.Visible = false;
				pictureBoxViewEditorPasteCursor.Width = 20;
				pictureBoxViewEditorPasteCursor.Height = 20;
				ResizeViewEditorPasteCursor();
				var bx = SelectedCharacterIndex % 32;
				var by = SelectedCharacterIndex / 32;
				ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0));
				RevalidateCharButtons();
				checkBoxShowDuplicates.Enabled = true;
				CheckDuplicate();
			}
		}

		public void FontBank_Click(object sender, EventArgs e)
		{
			SwitchFontBank();
		}

		public void FontBank_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxFontBank.Checked == false)
			{
				// Bank 1 + 2
				checkBoxFontBank.ImageIndex = 0;
				buttonLoadFont1.Text = @"Load 1";
				buttonLoadFont2.Text = @"Load 2";
				buttonSaveFont1.Text = @"Save 1";
				buttonSaveFont2.Text = @"Save 2";
				buttonClearFont1.Text = @"Clear 1";
				buttonClearFont2.Text = @"Clear 2";
			}
			else
			{
				checkBoxFontBank.ImageIndex = 1;
				buttonLoadFont1.Text = @"Load 3";
				buttonLoadFont2.Text = @"Load 4";
				buttonSaveFont1.Text = @"Save 3";
				buttonSaveFont2.Text = @"Save 4";
				buttonClearFont1.Text = @"Clear 3";
				buttonClearFont2.Text = @"Clear 4";
			}
		}

		#endregion

		// ==========================================================================
		// Font selector event handlers
		// Bottom-left area: Click to select the next active character
		// ==========================================================================
		#region Section F - Font selector event handlers

		public void FontSelector_MouseDown(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseDown(e);
		}

		public void FontSelector_MouseMove(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseMove(e);
		}

		public void FontSelector_MouseUp(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseUp(e);
		}

		public void FontSelector_MegaCopyImage_MouseDown(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseDown(new MouseEventArgs(e.Button, 0, e.X + pictureBoxFontSelectorMegaCopyImage.Left - pictureBoxFontSelector.Left, e.Y + pictureBoxFontSelectorMegaCopyImage.Top - pictureBoxFontSelector.Top, 0));
		}

		public void FontSelector_MegaCopyImage_MouseMove(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseMove(new MouseEventArgs(MouseButtons.None, 0, e.X + pictureBoxFontSelectorMegaCopyImage.Left - pictureBoxFontSelector.Left, e.Y + pictureBoxFontSelectorMegaCopyImage.Top - pictureBoxFontSelector.Top, 0));
		}

		public void FontSelector_RubberBand_MouseDown(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseDown(new MouseEventArgs(e.Button, 0, e.X + pictureBoxFontSelectorRubberBand.Left - pictureBoxFontSelector.Left, e.Y + pictureBoxFontSelectorRubberBand.Top - pictureBoxFontSelector.Top, 0));
		}
		public void FontSelector_RubberBand_MouseMove(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseMove(new MouseEventArgs(e.Button, 0, e.X + pictureBoxFontSelectorRubberBand.Left - pictureBoxFontSelector.Left, e.Y + pictureBoxFontSelectorRubberBand.Top - pictureBoxFontSelector.Top, 0));
		}
		public void FontSelector_RubberBand_MouseUp(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseUp(new MouseEventArgs(e.Button, 0, e.X + pictureBoxFontSelectorRubberBand.Left - pictureBoxFontSelector.Left, e.Y + pictureBoxFontSelectorRubberBand.Top - pictureBoxFontSelector.Top, 0));
		}

		private static bool _busyWith_FontSelectorRubberBandResize;
		private void FontSelector_RubberBand_Resize(object sender, EventArgs e)
		{
			if (_busyWith_FontSelectorRubberBandResize)
				return;
			_busyWith_FontSelectorRubberBandResize = true;

			var img = Helpers.NewImage(pictureBoxFontSelectorRubberBand);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(redBrush, new Rectangle(0, 0, img.Width, img.Height));
				pictureBoxFontSelectorRubberBand.Region?.Dispose();
				pictureBoxFontSelectorRubberBand.Size = new Size(img.Width, img.Height);

			}
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, pictureBoxFontSelectorRubberBand.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(pictureBoxFontSelectorRubberBand.Width - 2, 0, 2, pictureBoxFontSelectorRubberBand.Height));
			graphicsPath.AddRectangle(new Rectangle(0, pictureBoxFontSelectorRubberBand.Height - 2, pictureBoxFontSelectorRubberBand.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, pictureBoxFontSelectorRubberBand.Height));
			pictureBoxFontSelectorRubberBand.Region = new Region(graphicsPath);

			_busyWith_FontSelectorRubberBandResize = false;
		}

		public void FontSelector_PasteCursor_MouseDown(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseDown(new MouseEventArgs(e.Button, 0, pictureBoxFontSelectorPasteCursor.Left + e.X - pictureBoxFontSelector.Left, pictureBoxFontSelectorPasteCursor.Top + e.Y - pictureBoxFontSelector.Top, 0));
		}
		public void FontSelector_PasteCursor_MouseLeave(object _, EventArgs e)
		{
			pictureBoxFontSelectorPasteCursor.Visible = false;
			pictureBoxFontSelectorMegaCopyImage.Visible = false;
		}
		public void FontSelector_PasteCursor_MouseMove(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseMove(new MouseEventArgs(e.Button, 0, pictureBoxFontSelectorPasteCursor.Left + e.X - pictureBoxFontSelector.Left, pictureBoxFontSelectorPasteCursor.Top + e.Y - pictureBoxFontSelector.Top, 0));
		}
		public void FontSelector_PasteCursor_MouseUp(object _, MouseEventArgs e)
		{
			ActionFontSelectorMouseUp(new MouseEventArgs(e.Button, 0, pictureBoxFontSelectorPasteCursor.Left + e.X - pictureBoxFontSelector.Left, pictureBoxFontSelectorPasteCursor.Top + e.Y - pictureBoxFontSelector.Top, 0));
		}




		#endregion

		// ==========================================================================
		// Timer event handlers
		// ==========================================================================
		#region Timer Event Handlers

		public void AutoCloseAboutBox_Tick(object sender, EventArgs e)
		{
			pictureBoxAbout.Visible = false;
			timerAutoCloseAboutBox.Enabled = false;
		}

		public void TimerDuplicates_Tick(object sender, EventArgs e)
		{
			var idx = FindDuplicateChar();

			if (idx != SelectedCharacterIndex)
			{
				DuplicateCharacterIndex = idx;

				var rx = idx % 32;
				var ry = idx / 32;
				// var fontNr = idx / 256;

				pictureBoxDuplicateIndicator.Left = rx * 16 - 2;
				pictureBoxDuplicateIndicator.Top = ry * 16 - 2;
				pictureBoxDuplicateIndicator.Visible = true;
			}
		}

		#endregion


	}
}