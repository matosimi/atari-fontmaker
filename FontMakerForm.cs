using System.Drawing.Drawing2D;
using System.Media;
#pragma warning disable WFO1000

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

		public enum DirectionFlags
		{
			Left, Right, Up, Down
		}

		internal MegaCopyStatusFlags megaCopyStatus { get; set; } = MegaCopyStatusFlags.None;

		private static readonly SolidBrush BlackBrush = new(Color.Black);
		private static readonly SolidBrush WhiteBrush = new(Color.White);
		private static readonly SolidBrush CyanBrush = new(Color.Cyan);
		private static readonly SolidBrush RedBrush = new(Color.Red);
		private static readonly SolidBrush YellowBrush = new(Color.Yellow);
		private static readonly SolidBrush GreenBrush = new(Color.Green);

		private readonly List<Button> ActionListNormalModeOnly = new();

		#region All data used by Atari Font Maker
		public AtariColorSelectorForm AtariColorSelector { get; set; } = new();
		public ExportFontWindow ExportFontWindowForm { get; set; } = new();
		public ExportViewWindow ExportViewWindowForm { get; set; } = new();

		public ImportViewWindow ImportViewWindowForm { get; set; } = new();
		public FontAnalysisWindow FontAnalysisWindowForm { get; set; } = new();

		public ViewActionsWindow? ViewActionsWindowForm { get; set; } = null;

		/// <summary>
		/// The Atari color palette. Loaded from "altirraPAL.pal"
		/// </summary>
		internal Color[] AtariPalette { get; set; } = new Color[256];
		internal bool InColorMode { get; set; } = false;
		private bool _inMode5 = false;
		internal bool InMode5
		{
			get => _inMode5;
			set
			{
				_inMode5 = value;
				CellHeight = _inMode5 ? 32 : 16;
				CursorHeight = _inMode5 ? 36 : 20;
				CharXWidth = _inMode5 ? 20 : 40;
				ViewHeight = _inMode5 ? AtariView.VIEW_HEIGHT_TALL : AtariView.VIEW_HEIGHT;
			}
		}

		internal int CellHeight { get; set; } = 16;
		internal int CursorHeight { get; set; } = 20;
		internal int CharXWidth { get; set; } = 40;
		internal int ViewHeight { get; set; } = AtariView.VIEW_HEIGHT;

		internal bool PastingToView { get; set; }
		internal int ActiveColorNr { get; set; }
		internal int Active4BitColorNr { get; set; }
		/// <summary>
		/// The X active colors: These are indexes into the Atari color palette
		/// B&W : 2 for mono (index 0 + 1)
		/// Mode 4 & 5 : 5 for color (index 1, 2, 3, 4, 5 [inverse of 4])
		/// Mode 10 : 9 colors
		/// </summary>
		internal byte[] SetOfSelectedColors { get; set; } = new byte[10];
		internal SolidBrush[] BrushCache { get; set; } = new SolidBrush[10];
		internal SolidBrush? EmptyBrush { get; set; }

		internal string CurrentDataFolder { get; set; } = string.Empty;
		internal string Font1Filename { get; set; } = string.Empty;
		internal string Font2Filename { get; set; } = string.Empty;
		internal string Font3Filename { get; set; } = string.Empty;
		internal string Font4Filename { get; set; } = string.Empty;


		/// <summary>
		/// Index of the selected character in the font bank (0-511)
		/// </summary>
		internal int SelectedCharacterIndex { get; set; } // The current character 0 - 511
		/// <summary>
		/// Index of the last duplicate character found
		/// </summary>
		internal int DuplicateCharacterIndex { get; set; }

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
			ActionListNormalModeOnly.Add(buttonFontShiftRightInsert);
			ActionListNormalModeOnly.Add(buttonFontShiftRightRotate);
			ActionListNormalModeOnly.Add(buttonFontShiftLeftRotate);
			ActionListNormalModeOnly.Add(buttonFontShiftLeftInsert);
			ActionListNormalModeOnly.Add(buttonFontDeleteCharShiftRight);
			ActionListNormalModeOnly.Add(buttonFontDeleteCharShiftLeft);

			ViewActionsWindowForm = new ViewActionsWindow(this);

			this.Load += FormCreate!;
		}

		private void FormCreate(object sender, EventArgs e)
		{
			// Init the gui
			DoubleBuffered = true;
			ActiveColorNr = 2;
			Active4BitColorNr = 2;
			SelectedCharacterIndex = 0;
			comboBoxWriteMode.SelectedIndex = 0;
			comboBoxPasteIntoFontNr.SelectedIndex = 0;

			CurrentDataFolder = AppContext.BaseDirectory;

			// Position fix
			panelColorSwitcherMode10.Location = panelColorSwitcher.Location;

			UndoBuffer.Setup();
			AtariView.Setup();
			LoadPalette();

			LoadConfiguration();

			BuildColorModeList();

			BuildColorSelector();

			UpdateViewActions();

			string? ext;
			if (Environment.GetCommandLineArgs().Length - 1 == 1)
			{
				var loadPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[1]) + Path.DirectorySeparatorChar;
				ext = Path.GetExtension(Environment.GetCommandLineArgs()[1]).ToLower();
				CurrentDataFolder = loadPath;

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
						if (string.IsNullOrWhiteSpace(Font2Filename)) Font2Filename = Path.Join(loadPath, "Default.fnt");
						if (string.IsNullOrWhiteSpace(Font3Filename)) Font3Filename = Path.Join(loadPath, "Default.fnt");
						if (string.IsNullOrWhiteSpace(Font4Filename)) Font4Filename = Path.Join(loadPath, "Default.fnt");
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
				LoadViewFile(null, true);
				UpdateFormCaption();
				RedrawFonts();
				RedrawLineTypes();
				RedrawView();
				RedrawPal();
				RedrawViewChar();
				RedrawChar();
				ext = ".atrview";

				// If no input file set upon start, then show splash screen
				timerAutoCloseAboutBox.Enabled = true;
				pictureBoxAbout.Left = pictureBoxAtariView.Left + (checkBox40Bytes.Checked ? (pictureBoxAtariView.Width - pictureBoxAbout.Width) / 2 : 0);
				pictureBoxAbout.Visible = true;
			}

			if (ext != ".atrview")
			{
				// Make sure the SetOfSelectedColors values have valid brushes for painting!
				// Load the AtariPalette selection
				SetupDefaultPalColors();
				CurrentColorSetIndex = 0;
				SaveColorSet();
				BuildColorSetList();
				BuildPageList();

				RedrawPal();
				RedrawGrid();

				if (ext != ".fn2")
				{
					// Load each of the fonts into the banks
					AtariFont.LoadFont(Font1Filename, 0, false);
					AtariFont.LoadFont(Font2Filename, 1, false);
					AtariFont.LoadFont(Font3Filename, 2, false);
					AtariFont.LoadFont(Font4Filename, 3, false);

					if (string.IsNullOrWhiteSpace(Font2Filename)) Font2Filename = Path.Join(CurrentDataFolder, "Default.fnt");
					if (string.IsNullOrWhiteSpace(Font3Filename)) Font3Filename = Path.Join(CurrentDataFolder, "Default.fnt");
					if (string.IsNullOrWhiteSpace(Font4Filename)) Font4Filename = Path.Join(CurrentDataFolder, "Default.fnt");


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
			listBoxRecolorSourceMode10.SelectedIndex = 0;
			listBoxRecolorTargetMode10.SelectedIndex = 0;

			RedrawRecolorSource();
			RedrawRecolorTarget();
			RedrawRecolorMode10Source();
			RedrawRecolorMode10Target();

			UndoBuffer.Add2UndoInitial(); // initial undo buffer entry
			UpdateUndoButtons(false);

			MakeSomeColoredBlocks();

			SimulateSafeLeftMouseButtonClick();

			CheckDuplicate();
		}

		private void MakeSomeColoredBlocks()
		{
			// Paint something into the window
			var img = Helpers.GetImage(pictureBoxFontSelectorRubberBand);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(RedBrush, new Rectangle(0, 0, img.Width, img.Height));
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			pictureBoxFontSelectorRubberBand.Region = new Region(graphicsPath);
			pictureBoxFontSelectorRubberBand.Refresh();
			graphicsPath.Dispose();

			// Paint something into the window
			img = Helpers.GetImage(pictureBoxViewEditorRubberBand);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(CyanBrush, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			pictureBoxViewEditorRubberBand.Region = new Region(graphicsPath);
			pictureBoxViewEditorRubberBand.Refresh();
			graphicsPath.Dispose();

			// Paint something into the window
			img = Helpers.GetImage(pictureBoxFontSelectorPasteCursor);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(GreenBrush, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			pictureBoxFontSelectorPasteCursor.Region = new Region(graphicsPath);
			pictureBoxFontSelectorPasteCursor.Refresh();
			graphicsPath.Dispose();

			// Paint something into the window
			img = Helpers.GetImage(pictureBoxViewEditorPasteCursor);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(YellowBrush, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			pictureBoxViewEditorPasteCursor.Region = new Region(graphicsPath);
			pictureBoxViewEditorPasteCursor.Refresh();
			graphicsPath.Dispose();

			// Paint something into the window
			img = Helpers.GetImage(pictureBoxDuplicateIndicator);
			using (var gr = Graphics.FromImage(img))
			{
				using var trans = new SolidBrush(Color.Purple);
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
			graphicsPath.Dispose();
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
			if (e.Control)
			{
				// Handle all the CTRL+... shortcuts here

				// CTRL+M = Switch Mega Copy mode on/off
				if (e.KeyCode == Keys.M)
				{
					buttonMegaCopy.Checked = !buttonMegaCopy.Checked;
					MegaCopy_Click(0, EventArgs.Empty);
					return;
				}

				// CTRL+Tab | CTRL+Shift+Tab = Switch between color modes
				if (e.KeyCode == Keys.Tab)
				{
					// CTRL+TAB = Switch between color modes
					ActionNextPage(e.Shift ? -1 : 1);
					return;
				}

				// Ctrl + C = Copy to clipboard
				if (e.KeyCode == Keys.C)
				{
					ExecuteCopyToClipboard(false);
					return;
				}

				// Ctrl + V = Paste from clipboard
				if (e.KeyCode == Keys.V)
				{
					ExecutePasteFromClipboard();
					return;
				}

				// Ctrl + Z = Undo font change
				if (e.Shift == false && e.KeyCode == Keys.Z)
				{
					var (_, undoEnabled) = UndoBuffer.GetRedoUndoButtonState(CharacterEdited());
					if (undoEnabled)
						Undo_Click(0, EventArgs.Empty);
					return;
				}
				// Ctrl + Y = Redo font change
				if (e.Shift == false && e.KeyCode == Keys.Y)
				{
					var (redoEnabled, _) = UndoBuffer.GetRedoUndoButtonState(CharacterEdited());
					if (redoEnabled)
						Redo_Click(0, EventArgs.Empty);
					return;
				}

				// View editor undo/redo
				// Ctrl + Z = Undo view change
				if (e.Shift && e.KeyCode == Keys.Z)
				{
					ExecuteViewUndo();
					return;
				}
				// Ctrl + Y = Redo view change
				if (e.Shift && e.KeyCode == Keys.Y)
				{
					ExecuteViewRedo();
					return;
				}

				// Switch to page X
				// CTRL + 1-9 = Switch to page 1-9
				if (e.KeyCode == Keys.D1)
				{
					SavePageSwitch(0);
					return;
				}
				if (e.KeyCode == Keys.D2)
				{
					SavePageSwitch(1);
					return;
				}
				if (e.KeyCode == Keys.D3)
				{
					SavePageSwitch(2);
					return;
				}
				if (e.KeyCode == Keys.D4)
				{
					SavePageSwitch(3);
					return;
				}
				if (e.KeyCode == Keys.D5)
				{
					SavePageSwitch(4);
					return;
				}
				if (e.KeyCode == Keys.D6)
				{
					SavePageSwitch(5);
					return;
				}
				if (e.KeyCode == Keys.D7)
				{
					SavePageSwitch(6);
					return;
				}
				if (e.KeyCode == Keys.D8)
				{
					SavePageSwitch(7);
					return;
				}
				if (e.KeyCode == Keys.D9)
				{
					SavePageSwitch(8);
					return;
				}
				if (e.KeyCode == Keys.D0)
				{
					SavePageSwitch(9);
					return;
				}


				return;
			}
			// ----------------------------------
			// The next handlers will only work in normal mode
			// If CTRL is pressed, then the above handlers will be used

			// Move character selection with , and .
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

			// R and Shift+R rotate left and right
			if (e.KeyCode == Keys.R)
			{
				if (e.Shift)
					ExecuteRotateRight();
				else
					ExecuteRotateLeft();
				return;
			}

			// M and Shift+M mirror horizontal and vertical
			if (e.KeyCode == Keys.M)
			{
				if (e.Shift)
					ExecuteMirrorVertical();
				else
					ExecuteMirrorHorizontal();
				return;
			}

			// B = Switch font bank
			if (e.KeyCode == Keys.B)
			{
				checkBoxFontBank.Checked = !checkBoxFontBank.Checked;
				SwitchFontBank();
				return;
			}

			// Quick color selection with 0-8
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
			if (e.KeyCode == Keys.D4)
			{
				SetColor(5);
				return;
			}
			if (e.KeyCode == Keys.D5)
			{
				SetColor(6);
				return;
			}
			if (e.KeyCode == Keys.D6)
			{
				SetColor(7);
				return;
			}
			if (e.KeyCode == Keys.D7)
			{
				SetColor(8);
				return;
			}
			if (e.KeyCode == Keys.D8)
			{
				SetColor(9);
				return;
			}
			if (e.KeyCode == Keys.D0)
			{
				SetColor(1);
				return;
			}

			// I - Invert character
			if (e.KeyCode == Keys.I)
			{
				ExecuteInvertCharacter();
				return;
			}

			// Esc - cancel selection
			if (e.KeyCode == Keys.Escape)
			{
				ExecuteEscapeKeyPressed();
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
					// Change the current color when the Shift key is held down and the mouse wheel is moved
					switch (WhichColorMode)
					{
						case 4:
						case 5:
						default:
						{
							if (e.Delta > 0)
							{
								ActionCharacterEditorColor1MouseDown();
							}
							else
							{
								ActionCharacterEditorColor2MouseDown();
							}
							break;
						}
						case 10:
						{
							var nextColor = (cmbColor9Menu.SelectedIndex + (e.Delta > 0 ? 1 : -1));
							if (nextColor < 0) nextColor = cmbColor9Menu.Items.Count - 1;
							if (nextColor >= cmbColor9Menu.Items.Count) nextColor = 0;
							cmbColor9Menu.SelectedIndex = nextColor;
							break;
						}
					}
				}
				else
				{
					var step = e.Delta > 0 ? -1 : 1;
					if (Control.ModifierKeys == Keys.Control)
					{
						step *= 32;
					}
					var nextCharacterIndex = SelectedCharacterIndex + step;

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
		public void AboutPicture_MouseDown(object _, MouseEventArgs e)
		{
			ActionAboutUrl(e.X < pictureBoxAbout.Width / 2);
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

		#region Bottom/Below character editor
		private void CharacterEditor_Color9Menu_SelectedIndexChanged(object sender, EventArgs e)
		{
			ActionCharacterEditorColor9Selected();
		}

		private void CharacterEditor_Color9Menu_DrawItem(object sender, DrawItemEventArgs e)
		{
			Color9Menu_DrawItem(e);
		}
		#endregion

		#endregion

		// ==========================================================================
		// Area just to the right of the character editor
		// Handle graphics mode switch, color AtariPalette selection, recolor and export
		// ==========================================================================
		#region Section C - Color Change Event Handlers

		public void SwitchGraphicsMode_Click(object sender, EventArgs e)
		{
			SwitchGfxMode();    // Switch between Mode 2 and color (which color depends on the drop-down)
		}

		private void SwitchColorMode_SelectedIndexChanged(object _, EventArgs __)
		{
			if (InColorSetSetup) return;
			ColorMode_Change(); // The color mode drop down has changed!
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
			switch (WhichColorMode)
			{
				case 4:
				case 5:
				default:
					ColorSwitch2Bit(listBoxRecolorSource.SelectedIndex, listBoxRecolorTarget.SelectedIndex);
					break;
				case 10:
					ColorSwitch4Bit(listBoxRecolorSourceMode10.SelectedIndex, listBoxRecolorTargetMode10.SelectedIndex);
					break;
			}
		}

		public void ShowColorSwitchSetup_Click(object _, EventArgs __)
		{
			ShowColorSwitcher = !ShowColorSwitcher;

			if (ShowColorSwitcher)
			{
				switch (WhichColorMode)
				{
					case 4:
					case 5:
					default:
						panelColorSwitcher.Visible = true;
						break;
					case 10:
						panelColorSwitcherMode10.Visible = true;
						break;
				}
			}
			else
			{
				panelColorSwitcher.Visible = false;
				panelColorSwitcherMode10.Visible = false;
			}
		}

		public void ExportFont_Click(object _, EventArgs __)
		{
			ExportFontWindowForm.ShowDialog();
		}

		#region Recolor interactions
		public void RecolorSource_Click(object _, EventArgs __)
		{
			RedrawRecolorSource();
		}

		public void RecolorTarget_Click(object _, EventArgs __)
		{
			RedrawRecolorTarget();
		}

		private void RecolorSourceMode10_Click(object sender, EventArgs e)
		{
			RedrawRecolorMode10Source();

		}
		private void RecolorTargetMode10_Click(object sender, EventArgs e)
		{
			RedrawRecolorMode10Target();
		}

		#endregion

		private void comboBoxColorSets_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (InColorSetSetup) return;

			SwopColorSet(saveCurrent: true);
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

		private void ViewEditor_ExportView_Click(object sender, EventArgs e)
		{
			ExportViewWindowForm.InColorMode = InColorMode;
			ExportViewWindowForm.WhichColorMode = WhichColorMode;
			ExportViewWindowForm.ShowDialog();
		}

		private void ViewEditor_ImportView_Click(object sender, EventArgs e)
		{
			ImportViewWindowForm.InColorMode = InColorMode;
			ImportViewWindowForm.WhichColorMode = WhichColorMode;
			ImportViewWindowForm.ShowDialog();
			RedrawView();
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
			TransferPageSelectionToViewActions();
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
			ActionAtariViewEditorMouseDown(new MouseEventArgs(e.Button, 0, e.X + pictureBoxViewEditorMegaCopyImage.Left - pictureBoxAtariView.Left, e.Y + pictureBoxViewEditorMegaCopyImage.Top - pictureBoxAtariView.Top, -1000));
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

		private void ViewEditor_RubberBand_VisibleChanged(object sender, EventArgs e)
		{
			// Rubber band visibility has changed
			UpdateViewActions();
		}

		public void ViewEditor_RubberBand_MouseDown(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseDown(new MouseEventArgs(e.Button, 0, e.X + pictureBoxViewEditorRubberBand.Left - pictureBoxAtariView.Left, e.Y + pictureBoxViewEditorRubberBand.Top - pictureBoxAtariView.Top, -1000));
		}
		public void ViewEditor_RubberBand_MouseMove(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseMove(new MouseEventArgs(e.Button, 0, e.X + pictureBoxViewEditorRubberBand.Left - pictureBoxAtariView.Left, e.Y + pictureBoxViewEditorRubberBand.Top - pictureBoxAtariView.Top, 0));
		}
		public void ViewEditor_RubberBand_MouseUp(object sender, MouseEventArgs e)
		{
			ActionAtariViewMouseUp(new MouseEventArgs(e.Button, 0, e.X + pictureBoxViewEditorRubberBand.Left - pictureBoxAtariView.Left, e.Y + pictureBoxViewEditorRubberBand.Top - pictureBoxAtariView.Top, 0));
		}

		private static bool _busyWith_ViewEditorRubberBandResize;
		private void ViewEditor_RubberBand_Resize(object sender, EventArgs e)
		{
			if (_busyWith_ViewEditorRubberBandResize)
				return;
			_busyWith_ViewEditorRubberBandResize = true;

			var img = Helpers.NewImage(pictureBoxViewEditorRubberBand);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(CyanBrush, new Rectangle(0, 0, img.Width, img.Height));
				pictureBoxViewEditorRubberBand.Region?.Dispose();
				pictureBoxViewEditorRubberBand.Size = new Size(img.Width, img.Height);
			}
			using var graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, pictureBoxViewEditorRubberBand.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(pictureBoxViewEditorRubberBand.Width - 2, 0, 2, pictureBoxViewEditorRubberBand.Height));
			graphicsPath.AddRectangle(new Rectangle(0, pictureBoxViewEditorRubberBand.Height - 2, pictureBoxViewEditorRubberBand.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, pictureBoxViewEditorRubberBand.Height));
			pictureBoxViewEditorRubberBand.Region = new Region(graphicsPath);

			_busyWith_ViewEditorRubberBandResize = false;
		}

		public void ViewEditor_PasteCursor_MouseDown(object sender, MouseEventArgs e)
		{
			ActionAtariViewEditorMouseDown(new MouseEventArgs(e.Button, 0, pictureBoxViewEditorPasteCursor.Left + e.X - pictureBoxAtariView.Left, pictureBoxViewEditorPasteCursor.Top + e.Y - pictureBoxAtariView.Top, -1000));
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

		private void ViewEditor_FontAnalysis_Click(object sender, EventArgs e)
		{
			SaveCurrentPage();
			FontAnalysisWindowForm.InColorMode = InColorMode;
			FontAnalysisWindowForm.Pages = Pages;
			FontAnalysisWindowForm.ShowDialog();
		}

		private void ViewEditor_ViewActions_Click(object sender, EventArgs e)
		{
			SaveCurrentPage();
			UpdateViewActions();

			ViewActionsWindowForm?.Show();

			TransferPagesToViewActions();
			//TransferPageSelectionToViewActions();
		}

		private void ViewUndo_Click(object sender, EventArgs e)
		{
			ExecuteViewUndo();
		}

		private void ViewRedo_Click(object sender, EventArgs e)
		{
			ExecuteViewRedo();
		}

		private void trackBarSkipCharX_Scroll(object sender, EventArgs e)
		{
			checkBoxSkipChar0.Text = $"Skip char #{trackBarSkipCharX.Value} on paste";
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
			// Enable/disable actions between modes
			var on = buttonMegaCopy.Checked;
			buttonEnterText.Enabled = on;
			lblInMegaCopyMode.Visible = on;

			// Hide character edit window
			pictureBoxCharacterEditor.Visible = !on;
			pictureBoxClipboardPreview.Visible = on;

			// Hide recolor if in mega copy mode
			if (on && ShowColorSwitcher)
			{
				ShowColorSwitchSetup_Click(0, EventArgs.Empty);
			}

			foreach (var item in ActionListNormalModeOnly)
			{
				item.Enabled = !on;
			}

			ConfigureClipboardActionButtons();
			UpdateClipboardInformation();

			if (on)
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
				pictureBoxViewEditorRubberBand.Size = new Size(20, 20);
				pictureBoxViewEditorRubberBand.Visible = false;

				pictureBoxFontSelectorRubberBand.Visible = false;

				pictureBoxViewEditorPasteCursor.Visible = false;
				pictureBoxViewEditorMegaCopyImage.Visible = false;
			}
			else
			{
				// MegaCopy mode off
				// Turn the GUI back on
				megaCopyStatus = MegaCopyStatusFlags.None;
				pictureBoxFontSelectorRubberBand.Width = 20;
				pictureBoxFontSelectorRubberBand.Height = 20;
				pictureBoxFontSelectorRubberBand.Visible = true;
				pictureBoxViewEditorRubberBand.Visible = false;
				pictureBoxViewEditorPasteCursor.Width = 20;
				pictureBoxViewEditorPasteCursor.Height = 20;
				ResizeViewEditorPasteCursor();
				pictureBoxViewEditorPasteCursor.Visible = false;
				pictureBoxViewEditorMegaCopyImage.Visible = false;

				pictureBoxFontSelectorMegaCopyImage.Visible = false;
				pictureBoxFontSelectorPasteCursor.Visible = false;

				SimulateSafeLeftMouseButtonClick();

				RevalidateCharButtons();
				checkBoxShowDuplicates.Enabled = true;
				CheckDuplicate();
			}

			UpdateViewActions();
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

		private void UpdateGuiAfterFontAction()
		{
			RedrawChar();
			RedrawFonts();
			UpdateCharacterViews();
			RedrawView();
			UndoBuffer.Add2UndoFullDifferenceScan();
			UpdateUndoButtons(false);
		}

		private void buttonFontShiftLeftInsert_Click(object sender, EventArgs e)
		{
			AtariFont.ShiftFontLeft(SelectedCharacterIndex, checkBoxFontBank.Checked, true);
			UpdateGuiAfterFontAction();
		}

		private void buttonFontShiftLeftRotate_Click(object sender, EventArgs e)
		{
			AtariFont.ShiftFontLeft(SelectedCharacterIndex, checkBoxFontBank.Checked, false);
			UpdateGuiAfterFontAction();
		}


		private void buttonFontDeleteCharShiftRight_Click(object sender, EventArgs e)
		{
			AtariFont.DeleteAndShiftRight(SelectedCharacterIndex, checkBoxFontBank.Checked);
			UpdateGuiAfterFontAction();
		}

		private void buttonFontDeleteCharShiftLeft_Click(object sender, EventArgs e)
		{
			AtariFont.DeleteAndShiftLeft(SelectedCharacterIndex, checkBoxFontBank.Checked);
			UpdateGuiAfterFontAction();
		}

		private void buttonFontShiftRightRotate_Click(object sender, EventArgs e)
		{
			AtariFont.ShiftFontRight(SelectedCharacterIndex, checkBoxFontBank.Checked, false);
			UpdateGuiAfterFontAction();
		}

		private void buttonFontShiftRightInsert_Click(object sender, EventArgs e)
		{
			AtariFont.ShiftFontRight(SelectedCharacterIndex, checkBoxFontBank.Checked, true);
			UpdateGuiAfterFontAction();
		}

		private void buttonCopyAreaShiftRight_Click(object sender, EventArgs e)
		{
			ExecuteCopyAreaShiftRight();
		}

		private void buttonCopyAreaShiftLeft_Click(object sender, EventArgs e)
		{
			ExecuteCopyAreaShiftLeft();
		}

		private void buttonCopyAreaShiftUp_Click(object sender, EventArgs e)
		{
			ExecuteCopyAreaShiftUp();
		}

		private void buttonCopyAreaShiftDown_Click(object sender, EventArgs e)
		{
			ExecuteCopyAreaShiftDown();
		}

		private void buttonPasteInPlace_Click(object sender, EventArgs e)
		{
			ExecuteClipboardInPlace();
		}

		private void buttonCopyAreaHMirror_Click(object sender, EventArgs e)
		{
			ExecuteCopyAreaHorizontalMirror();

		}

		private void buttonCopyAreaVMirror_Click(object sender, EventArgs e)
		{
			ExecuteCopyAreaVerticalMirror();
		}

		private void buttonCopyAreaInvert_Click(object sender, EventArgs e)
		{
			ExecuteCopyAreaInvert();
		}

		private void buttonCopyAreaRotateLeft_Click(object sender, EventArgs e)
		{
			ExecuteCopyAreaRotateLeft();
		}

		private void buttonCopyAreaRotateRight_Click(object sender, EventArgs e)
		{
			ExecuteCopyAreaRotateRight();
		}

		private void comboBoxPasteIntoFontNr_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonPasteInPlace.Text = $"Paste to Font {comboBoxPasteIntoFontNr.SelectedIndex + 1}";
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

		private void FontSelector_MegaCopyImage_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			// Setup selection by right DoubleClick
			if (e.Button == MouseButtons.Right)
			{
				ResetMegaCopyStatus();
			}
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
				gr.FillRectangle(RedBrush, new Rectangle(0, 0, img.Width, img.Height));
				pictureBoxFontSelectorRubberBand.Region?.Dispose();
				pictureBoxFontSelectorRubberBand.Size = new Size(img.Width, img.Height);

			}
			using var graphicsPath = new GraphicsPath();
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