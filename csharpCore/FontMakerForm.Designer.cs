namespace FontMaker
{
	partial class FontMakerForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontMakerForm));
			pictureBoxAtariView = new PictureBox();
			pictureBoxFontSelector = new PictureBox();
			pictureBoxDuplicateIndicator = new PictureBox();
			pictureBoxAbout = new PictureBox();
			pictureBoxCharacterSetSelector = new PictureBox();
			pictureBoxFontSelectorMegaCopyImage = new PictureBox();
			pictureBoxViewEditorMegaCopyImage = new PictureBox();
			labelViewCharInfo = new Label();
			p_xx = new Panel();
			Bevel3 = new Panel();
			pictureBoxCharacterEditor = new PictureBox();
			pictureBoxCharacterEditorColor1 = new PictureBox();
			pictureBoxCharacterEditorColor2 = new PictureBox();
			buttonShiftUp = new Button();
			buttonMirrorVertical = new Button();
			buttonShiftRight = new Button();
			buttonShiftLeft = new Button();
			buttonShiftDown = new Button();
			buttonRotateRight = new Button();
			buttonRotateLeft = new Button();
			buttonRestoreSaved = new Button();
			buttonRestoreDefault = new Button();
			buttonInverse = new Button();
			buttonMirrorHorizontal = new Button();
			buttonClear = new Button();
			buttonCopy = new Button();
			buttonPaste = new Button();
			p_hh = new Panel();
			buttonClearFont2 = new Button();
			buttonClearFont1 = new Button();
			buttonLoadFont1 = new Button();
			buttonSaveFont1As = new Button();
			buttonNew = new Button();
			buttonAbout = new Button();
			buttonQuit = new Button();
			buttonSaveFont1 = new Button();
			buttonSaveFont2 = new Button();
			buttonLoadFont2 = new Button();
			buttonSaveFont2As = new Button();
			p_zz = new Panel();
			Bevel4 = new Panel();
			pictureBoxPalette = new PictureBox();
			buttonShowColorSwitchSetup = new Button();
			buttonSwitchGraphicsMode = new Button();
			buttonExportFont = new Button();
			buttonRecolor = new Button();
			buttonLoadView = new Button();
			buttonClearView = new Button();
			buttonSaveView = new Button();
			p_status = new Panel();
			checkBoxFontBank = new CheckBox();
			imageListFont1234 = new ImageList(components);
			pictureBoxActionColor = new PictureBox();
			labelEditCharInfo = new Label();
			labelColor = new Label();
			checkBoxShowDuplicates = new CheckBox();
			buttonMegaCopy = new CheckBox();
			buttonUndo = new Button();
			buttonRedo = new Button();
			comboBoxWriteMode = new ComboBox();
			panelColorSwitcher = new Panel();
			pictureBoxRecolorSourceColor = new PictureBox();
			pictureBoxRecolorTargetColor = new PictureBox();
			listBoxRecolorSource = new ListBox();
			listBoxRecolorTarget = new ListBox();
			buttonEnterText = new Button();
			checkBox40Bytes = new CheckBox();
			dialogOpenFile = new OpenFileDialog();
			dialogSaveFile = new SaveFileDialog();
			timerAutoCloseAboutBox = new System.Windows.Forms.Timer(components);
			timerDuplicates = new System.Windows.Forms.Timer(components);
			pictureBoxFontSelectorRubberBand = new PictureBox();
			pictureBoxViewEditorRubberBand = new PictureBox();
			pictureBoxFontSelectorPasteCursor = new PictureBox();
			pictureBoxViewEditorPasteCursor = new PictureBox();
			toolTips = new ToolTip(components);
			comboBoxPages = new ComboBox();
			buttonAddPage = new Button();
			buttonDeletePage = new Button();
			buttonEditPage = new Button();
			labelCurrentPageIndex = new Label();
			buttonExportView = new Button();
			((System.ComponentModel.ISupportInitialize)pictureBoxAtariView).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxFontSelector).BeginInit();
			pictureBoxFontSelector.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxDuplicateIndicator).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxAbout).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxCharacterSetSelector).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxFontSelectorMegaCopyImage).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxViewEditorMegaCopyImage).BeginInit();
			p_xx.SuspendLayout();
			Bevel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxCharacterEditor).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxCharacterEditorColor1).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxCharacterEditorColor2).BeginInit();
			p_hh.SuspendLayout();
			p_zz.SuspendLayout();
			Bevel4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxPalette).BeginInit();
			p_status.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxActionColor).BeginInit();
			panelColorSwitcher.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxRecolorSourceColor).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxRecolorTargetColor).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxFontSelectorRubberBand).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxViewEditorRubberBand).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxFontSelectorPasteCursor).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxViewEditorPasteCursor).BeginInit();
			SuspendLayout();
			// 
			// pictureBoxAtariView
			// 
			pictureBoxAtariView.Location = new Point(536, 0);
			pictureBoxAtariView.Name = "pictureBoxAtariView";
			pictureBoxAtariView.Size = new Size(640, 416);
			pictureBoxAtariView.TabIndex = 0;
			pictureBoxAtariView.TabStop = false;
			pictureBoxAtariView.MouseDoubleClick += ViewEditor_DoubleClick;
			pictureBoxAtariView.MouseDown += ViewEditor_MouseDown;
			pictureBoxAtariView.MouseMove += ViewEditor_MouseMove;
			pictureBoxAtariView.MouseUp += ViewEditor_MouseUp;
			// 
			// pictureBoxFontSelector
			// 
			pictureBoxFontSelector.Controls.Add(pictureBoxDuplicateIndicator);
			pictureBoxFontSelector.Location = new Point(2, 207);
			pictureBoxFontSelector.Name = "pictureBoxFontSelector";
			pictureBoxFontSelector.Size = new Size(512, 256);
			pictureBoxFontSelector.TabIndex = 1;
			pictureBoxFontSelector.TabStop = false;
			pictureBoxFontSelector.MouseDown += FontSelector_MouseDown;
			pictureBoxFontSelector.MouseMove += FontSelector_MouseMove;
			pictureBoxFontSelector.MouseUp += FontSelector_MouseUp;
			// 
			// pictureBoxDuplicateIndicator
			// 
			pictureBoxDuplicateIndicator.BackColor = Color.Transparent;
			pictureBoxDuplicateIndicator.Location = new Point(0, 0);
			pictureBoxDuplicateIndicator.Margin = new Padding(0);
			pictureBoxDuplicateIndicator.Name = "pictureBoxDuplicateIndicator";
			pictureBoxDuplicateIndicator.Size = new Size(20, 20);
			pictureBoxDuplicateIndicator.TabIndex = 11;
			pictureBoxDuplicateIndicator.TabStop = false;
			pictureBoxDuplicateIndicator.Visible = false;
			// 
			// pictureBoxAbout
			// 
			pictureBoxAbout.Image = (Image)resources.GetObject("pictureBoxAbout.Image");
			pictureBoxAbout.InitialImage = null;
			pictureBoxAbout.Location = new Point(536, 136);
			pictureBoxAbout.Name = "pictureBoxAbout";
			pictureBoxAbout.Size = new Size(512, 128);
			pictureBoxAbout.TabIndex = 2;
			pictureBoxAbout.TabStop = false;
			pictureBoxAbout.Visible = false;
			pictureBoxAbout.MouseDown += AboutPicture_MouseDown;
			// 
			// pictureBoxCharacterSetSelector
			// 
			pictureBoxCharacterSetSelector.Location = new Point(520, 0);
			pictureBoxCharacterSetSelector.Name = "pictureBoxCharacterSetSelector";
			pictureBoxCharacterSetSelector.Size = new Size(15, 416);
			pictureBoxCharacterSetSelector.TabIndex = 3;
			pictureBoxCharacterSetSelector.TabStop = false;
			pictureBoxCharacterSetSelector.MouseDown += ViewEditor_CharacterSetSelector_MouseDown;
			// 
			// pictureBoxFontSelectorMegaCopyImage
			// 
			pictureBoxFontSelectorMegaCopyImage.Location = new Point(26, 256);
			pictureBoxFontSelectorMegaCopyImage.Name = "pictureBoxFontSelectorMegaCopyImage";
			pictureBoxFontSelectorMegaCopyImage.Size = new Size(105, 105);
			pictureBoxFontSelectorMegaCopyImage.TabIndex = 4;
			pictureBoxFontSelectorMegaCopyImage.TabStop = false;
			pictureBoxFontSelectorMegaCopyImage.Visible = false;
			pictureBoxFontSelectorMegaCopyImage.MouseDoubleClick += FontSelector_MegaCopyImage_MouseDoubleClick;
			pictureBoxFontSelectorMegaCopyImage.MouseDown += FontSelector_MegaCopyImage_MouseDown;
			pictureBoxFontSelectorMegaCopyImage.MouseMove += FontSelector_MegaCopyImage_MouseMove;
			// 
			// pictureBoxViewEditorMegaCopyImage
			// 
			pictureBoxViewEditorMegaCopyImage.Location = new Point(560, 296);
			pictureBoxViewEditorMegaCopyImage.Name = "pictureBoxViewEditorMegaCopyImage";
			pictureBoxViewEditorMegaCopyImage.Size = new Size(105, 105);
			pictureBoxViewEditorMegaCopyImage.TabIndex = 5;
			pictureBoxViewEditorMegaCopyImage.TabStop = false;
			pictureBoxViewEditorMegaCopyImage.Visible = false;
			pictureBoxViewEditorMegaCopyImage.MouseDoubleClick += ViewEditor_MegaCopyImage_MouseDoubleClick;
			pictureBoxViewEditorMegaCopyImage.MouseDown += ViewEditor_MegaCopyImage_MouseDown;
			pictureBoxViewEditorMegaCopyImage.MouseMove += ViewEditor_MegaCopyImage_MouseMove;
			// 
			// labelViewCharInfo
			// 
			labelViewCharInfo.AutoSize = true;
			labelViewCharInfo.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			labelViewCharInfo.Location = new Point(525, 446);
			labelViewCharInfo.Name = "labelViewCharInfo";
			labelViewCharInfo.Size = new Size(99, 13);
			labelViewCharInfo.TabIndex = 6;
			labelViewCharInfo.Text = "Char: Font1 $00 #0";
			// 
			// p_xx
			// 
			p_xx.BorderStyle = BorderStyle.FixedSingle;
			p_xx.Controls.Add(Bevel3);
			p_xx.Controls.Add(pictureBoxCharacterEditorColor1);
			p_xx.Controls.Add(pictureBoxCharacterEditorColor2);
			p_xx.Controls.Add(buttonShiftUp);
			p_xx.Controls.Add(buttonMirrorVertical);
			p_xx.Controls.Add(buttonShiftRight);
			p_xx.Controls.Add(buttonShiftLeft);
			p_xx.Controls.Add(buttonShiftDown);
			p_xx.Controls.Add(buttonRotateRight);
			p_xx.Controls.Add(buttonRotateLeft);
			p_xx.Controls.Add(buttonRestoreSaved);
			p_xx.Controls.Add(buttonRestoreDefault);
			p_xx.Controls.Add(buttonInverse);
			p_xx.Controls.Add(buttonMirrorHorizontal);
			p_xx.Controls.Add(buttonClear);
			p_xx.Controls.Add(buttonCopy);
			p_xx.Controls.Add(buttonPaste);
			p_xx.Location = new Point(113, 0);
			p_xx.Name = "p_xx";
			p_xx.Size = new Size(289, 177);
			p_xx.TabIndex = 1;
			// 
			// Bevel3
			// 
			Bevel3.BorderStyle = BorderStyle.FixedSingle;
			Bevel3.Controls.Add(pictureBoxCharacterEditor);
			Bevel3.Location = new Point(63, 7);
			Bevel3.Name = "Bevel3";
			Bevel3.Size = new Size(162, 162);
			Bevel3.TabIndex = 2;
			// 
			// pictureBoxCharacterEditor
			// 
			pictureBoxCharacterEditor.Location = new Point(0, 0);
			pictureBoxCharacterEditor.Name = "pictureBoxCharacterEditor";
			pictureBoxCharacterEditor.Size = new Size(160, 160);
			pictureBoxCharacterEditor.TabIndex = 3;
			pictureBoxCharacterEditor.TabStop = false;
			pictureBoxCharacterEditor.MouseDown += CharacterEditor_MouseDown;
			pictureBoxCharacterEditor.MouseMove += CharacterEditor_MouseMove;
			pictureBoxCharacterEditor.MouseUp += CharacterEditor_MouseUp;
			// 
			// pictureBoxCharacterEditorColor1
			// 
			pictureBoxCharacterEditorColor1.Location = new Point(8, 8);
			pictureBoxCharacterEditorColor1.Name = "pictureBoxCharacterEditorColor1";
			pictureBoxCharacterEditorColor1.Size = new Size(49, 17);
			pictureBoxCharacterEditorColor1.TabIndex = 4;
			pictureBoxCharacterEditorColor1.TabStop = false;
			pictureBoxCharacterEditorColor1.Tag = 3;
			pictureBoxCharacterEditorColor1.MouseDown += CharacterEditor_Color1_MouseDown;
			// 
			// pictureBoxCharacterEditorColor2
			// 
			pictureBoxCharacterEditorColor2.Location = new Point(232, 8);
			pictureBoxCharacterEditorColor2.Name = "pictureBoxCharacterEditorColor2";
			pictureBoxCharacterEditorColor2.Size = new Size(49, 17);
			pictureBoxCharacterEditorColor2.TabIndex = 5;
			pictureBoxCharacterEditorColor2.TabStop = false;
			pictureBoxCharacterEditorColor2.Tag = 4;
			pictureBoxCharacterEditorColor2.MouseDown += CharacterEditor_Color2_MouseDown;
			// 
			// buttonShiftUp
			// 
			buttonShiftUp.Location = new Point(232, 91);
			buttonShiftUp.Margin = new Padding(0);
			buttonShiftUp.Name = "buttonShiftUp";
			buttonShiftUp.Size = new Size(49, 20);
			buttonShiftUp.TabIndex = 10;
			buttonShiftUp.Tag = 1;
			buttonShiftUp.Text = "SHU";
			toolTips.SetToolTip(buttonShiftUp, "Shift Up");
			buttonShiftUp.UseVisualStyleBackColor = true;
			buttonShiftUp.Click += CharacterEditor_ShiftUp_Click;
			// 
			// buttonMirrorVertical
			// 
			buttonMirrorVertical.Location = new Point(232, 49);
			buttonMirrorVertical.Margin = new Padding(0);
			buttonMirrorVertical.Name = "buttonMirrorVertical";
			buttonMirrorVertical.Size = new Size(49, 20);
			buttonMirrorVertical.TabIndex = 8;
			buttonMirrorVertical.Tag = 1;
			buttonMirrorVertical.Text = "V MIR";
			toolTips.SetToolTip(buttonMirrorVertical, "Vertical Mirror (Shift+M)");
			buttonMirrorVertical.UseVisualStyleBackColor = true;
			buttonMirrorVertical.Click += CharacterEditor_MirrorVertical_Click;
			// 
			// buttonShiftRight
			// 
			buttonShiftRight.Location = new Point(232, 70);
			buttonShiftRight.Margin = new Padding(0);
			buttonShiftRight.Name = "buttonShiftRight";
			buttonShiftRight.Size = new Size(49, 20);
			buttonShiftRight.TabIndex = 9;
			buttonShiftRight.Tag = 1;
			buttonShiftRight.Text = "SHR";
			toolTips.SetToolTip(buttonShiftRight, "Shift Right");
			buttonShiftRight.UseVisualStyleBackColor = true;
			buttonShiftRight.Click += CharacterEditor_ShiftRight_Click;
			// 
			// buttonShiftLeft
			// 
			buttonShiftLeft.Location = new Point(8, 70);
			buttonShiftLeft.Margin = new Padding(0);
			buttonShiftLeft.Name = "buttonShiftLeft";
			buttonShiftLeft.Size = new Size(49, 20);
			buttonShiftLeft.TabIndex = 2;
			buttonShiftLeft.Tag = 1;
			buttonShiftLeft.Text = "SHL";
			toolTips.SetToolTip(buttonShiftLeft, "Shift Left");
			buttonShiftLeft.UseVisualStyleBackColor = true;
			buttonShiftLeft.Click += CharacterEditor_ShiftLeft_Click;
			// 
			// buttonShiftDown
			// 
			buttonShiftDown.Location = new Point(8, 91);
			buttonShiftDown.Margin = new Padding(0);
			buttonShiftDown.Name = "buttonShiftDown";
			buttonShiftDown.Size = new Size(49, 20);
			buttonShiftDown.TabIndex = 3;
			buttonShiftDown.Tag = 1;
			buttonShiftDown.Text = "SHD";
			toolTips.SetToolTip(buttonShiftDown, "Shift Down");
			buttonShiftDown.UseVisualStyleBackColor = true;
			buttonShiftDown.Click += CharacterEditor_ShiftDown_Click;
			// 
			// buttonRotateRight
			// 
			buttonRotateRight.Location = new Point(232, 28);
			buttonRotateRight.Margin = new Padding(0);
			buttonRotateRight.Name = "buttonRotateRight";
			buttonRotateRight.Size = new Size(49, 20);
			buttonRotateRight.TabIndex = 7;
			buttonRotateRight.Tag = 2;
			buttonRotateRight.Text = "ROR";
			toolTips.SetToolTip(buttonRotateRight, "Rotate Right (Shift+R)");
			buttonRotateRight.UseVisualStyleBackColor = true;
			buttonRotateRight.Click += CharacterEditor_RotateRight_Click;
			// 
			// buttonRotateLeft
			// 
			buttonRotateLeft.Location = new Point(8, 28);
			buttonRotateLeft.Margin = new Padding(0);
			buttonRotateLeft.Name = "buttonRotateLeft";
			buttonRotateLeft.Size = new Size(49, 20);
			buttonRotateLeft.TabIndex = 0;
			buttonRotateLeft.Tag = 2;
			buttonRotateLeft.Text = "ROL";
			buttonRotateLeft.TextAlign = ContentAlignment.TopCenter;
			toolTips.SetToolTip(buttonRotateLeft, "Rotate Left (R)");
			buttonRotateLeft.UseMnemonic = false;
			buttonRotateLeft.UseVisualStyleBackColor = true;
			buttonRotateLeft.Click += CharacterEditor_RotateLeft_Click;
			// 
			// buttonRestoreSaved
			// 
			buttonRestoreSaved.Location = new Point(8, 133);
			buttonRestoreSaved.Margin = new Padding(0);
			buttonRestoreSaved.Name = "buttonRestoreSaved";
			buttonRestoreSaved.Size = new Size(49, 20);
			buttonRestoreSaved.TabIndex = 5;
			buttonRestoreSaved.Text = "RES S";
			toolTips.SetToolTip(buttonRestoreSaved, "Restore Last Saved");
			buttonRestoreSaved.UseVisualStyleBackColor = true;
			buttonRestoreSaved.Click += CharacterEditor_RestoreSaved_Click;
			// 
			// buttonRestoreDefault
			// 
			buttonRestoreDefault.Location = new Point(8, 112);
			buttonRestoreDefault.Margin = new Padding(0);
			buttonRestoreDefault.Name = "buttonRestoreDefault";
			buttonRestoreDefault.Size = new Size(49, 20);
			buttonRestoreDefault.TabIndex = 4;
			buttonRestoreDefault.Text = "RES D";
			toolTips.SetToolTip(buttonRestoreDefault, "Restore Default");
			buttonRestoreDefault.UseVisualStyleBackColor = true;
			buttonRestoreDefault.Click += CharacterEditor_RestoreDefault_Click;
			// 
			// buttonInverse
			// 
			buttonInverse.Location = new Point(232, 112);
			buttonInverse.Margin = new Padding(0);
			buttonInverse.Name = "buttonInverse";
			buttonInverse.Size = new Size(49, 20);
			buttonInverse.TabIndex = 11;
			buttonInverse.Tag = 2;
			buttonInverse.Text = "INV";
			toolTips.SetToolTip(buttonInverse, "InvertCharacter (i)");
			buttonInverse.UseVisualStyleBackColor = true;
			buttonInverse.Click += CharacterEditor_Inverse_Click;
			// 
			// buttonMirrorHorizontal
			// 
			buttonMirrorHorizontal.Location = new Point(8, 49);
			buttonMirrorHorizontal.Margin = new Padding(0);
			buttonMirrorHorizontal.Name = "buttonMirrorHorizontal";
			buttonMirrorHorizontal.Size = new Size(49, 20);
			buttonMirrorHorizontal.TabIndex = 1;
			buttonMirrorHorizontal.Tag = 1;
			buttonMirrorHorizontal.Text = "H MIR";
			toolTips.SetToolTip(buttonMirrorHorizontal, "Horizontal Mirror (M)");
			buttonMirrorHorizontal.UseVisualStyleBackColor = true;
			buttonMirrorHorizontal.Click += CharacterEditor_MirrorHorizontal_Click;
			// 
			// buttonClear
			// 
			buttonClear.Location = new Point(232, 133);
			buttonClear.Margin = new Padding(0);
			buttonClear.Name = "buttonClear";
			buttonClear.Size = new Size(49, 20);
			buttonClear.TabIndex = 12;
			buttonClear.Tag = 1;
			buttonClear.Text = "CLR";
			toolTips.SetToolTip(buttonClear, "Clear");
			buttonClear.UseVisualStyleBackColor = true;
			buttonClear.Click += CharacterEditor_Clear_Click;
			// 
			// buttonCopy
			// 
			buttonCopy.Location = new Point(8, 154);
			buttonCopy.Name = "buttonCopy";
			buttonCopy.Size = new Size(49, 20);
			buttonCopy.TabIndex = 6;
			buttonCopy.Text = "CPY";
			toolTips.SetToolTip(buttonCopy, "Copy To Clipboard");
			buttonCopy.UseVisualStyleBackColor = true;
			buttonCopy.Click += CopyToClipboard_Click;
			// 
			// buttonPaste
			// 
			buttonPaste.Location = new Point(232, 154);
			buttonPaste.Margin = new Padding(0);
			buttonPaste.Name = "buttonPaste";
			buttonPaste.Size = new Size(49, 20);
			buttonPaste.TabIndex = 13;
			buttonPaste.Text = "PST";
			toolTips.SetToolTip(buttonPaste, "Paste From Clipboard");
			buttonPaste.UseVisualStyleBackColor = true;
			buttonPaste.Click += PasteFromClipboard_Click;
			// 
			// p_hh
			// 
			p_hh.BorderStyle = BorderStyle.FixedSingle;
			p_hh.Controls.Add(buttonClearFont2);
			p_hh.Controls.Add(buttonClearFont1);
			p_hh.Controls.Add(buttonLoadFont1);
			p_hh.Controls.Add(buttonSaveFont1As);
			p_hh.Controls.Add(buttonNew);
			p_hh.Controls.Add(buttonAbout);
			p_hh.Controls.Add(buttonQuit);
			p_hh.Controls.Add(buttonSaveFont1);
			p_hh.Controls.Add(buttonSaveFont2);
			p_hh.Controls.Add(buttonLoadFont2);
			p_hh.Controls.Add(buttonSaveFont2As);
			p_hh.Location = new Point(1, 0);
			p_hh.Name = "p_hh";
			p_hh.Size = new Size(105, 177);
			p_hh.TabIndex = 0;
			// 
			// buttonClearFont2
			// 
			buttonClearFont2.Location = new Point(52, 106);
			buttonClearFont2.Margin = new Padding(0);
			buttonClearFont2.Name = "buttonClearFont2";
			buttonClearFont2.Size = new Size(50, 20);
			buttonClearFont2.TabIndex = 10;
			buttonClearFont2.Text = "Clear 2";
			toolTips.SetToolTip(buttonClearFont2, "Blank out font");
			buttonClearFont2.UseVisualStyleBackColor = true;
			buttonClearFont2.Click += ClearFont2_Click;
			// 
			// buttonClearFont1
			// 
			buttonClearFont1.Location = new Point(3, 106);
			buttonClearFont1.Margin = new Padding(0);
			buttonClearFont1.Name = "buttonClearFont1";
			buttonClearFont1.Size = new Size(50, 20);
			buttonClearFont1.TabIndex = 9;
			buttonClearFont1.Text = "Clear 1";
			toolTips.SetToolTip(buttonClearFont1, "Blank out font");
			buttonClearFont1.UseVisualStyleBackColor = true;
			buttonClearFont1.Click += ClearFont1_Click;
			// 
			// buttonLoadFont1
			// 
			buttonLoadFont1.Location = new Point(3, 35);
			buttonLoadFont1.Margin = new Padding(0);
			buttonLoadFont1.Name = "buttonLoadFont1";
			buttonLoadFont1.Size = new Size(50, 20);
			buttonLoadFont1.TabIndex = 1;
			buttonLoadFont1.Text = "Load 1";
			toolTips.SetToolTip(buttonLoadFont1, "Load font");
			buttonLoadFont1.UseVisualStyleBackColor = true;
			buttonLoadFont1.Click += LoadFont1_Click;
			// 
			// buttonSaveFont1As
			// 
			buttonSaveFont1As.Location = new Point(3, 80);
			buttonSaveFont1As.Margin = new Padding(0);
			buttonSaveFont1As.Name = "buttonSaveFont1As";
			buttonSaveFont1As.Size = new Size(50, 20);
			buttonSaveFont1As.TabIndex = 4;
			buttonSaveFont1As.Text = "as...";
			buttonSaveFont1As.UseVisualStyleBackColor = true;
			buttonSaveFont1As.Click += SaveFont1As_Click;
			// 
			// buttonNew
			// 
			buttonNew.Location = new Point(7, 8);
			buttonNew.Name = "buttonNew";
			buttonNew.Size = new Size(90, 20);
			buttonNew.TabIndex = 0;
			buttonNew.Text = "New";
			buttonNew.UseVisualStyleBackColor = true;
			buttonNew.Click += New_Click;
			// 
			// buttonAbout
			// 
			buttonAbout.Location = new Point(8, 130);
			buttonAbout.Name = "buttonAbout";
			buttonAbout.Size = new Size(89, 20);
			buttonAbout.TabIndex = 5;
			buttonAbout.Text = "About";
			buttonAbout.UseVisualStyleBackColor = true;
			buttonAbout.Click += About_Click;
			// 
			// buttonQuit
			// 
			buttonQuit.Location = new Point(8, 154);
			buttonQuit.Name = "buttonQuit";
			buttonQuit.Size = new Size(89, 20);
			buttonQuit.TabIndex = 6;
			buttonQuit.Text = "Quit";
			buttonQuit.UseVisualStyleBackColor = true;
			buttonQuit.Click += Quit_Click;
			// 
			// buttonSaveFont1
			// 
			buttonSaveFont1.Location = new Point(3, 61);
			buttonSaveFont1.Margin = new Padding(0);
			buttonSaveFont1.Name = "buttonSaveFont1";
			buttonSaveFont1.Size = new Size(50, 20);
			buttonSaveFont1.TabIndex = 2;
			buttonSaveFont1.Text = "Save 1";
			buttonSaveFont1.UseVisualStyleBackColor = true;
			buttonSaveFont1.Click += SaveFont1_Click;
			// 
			// buttonSaveFont2
			// 
			buttonSaveFont2.Location = new Point(52, 61);
			buttonSaveFont2.Margin = new Padding(0);
			buttonSaveFont2.Name = "buttonSaveFont2";
			buttonSaveFont2.Size = new Size(50, 20);
			buttonSaveFont2.TabIndex = 3;
			buttonSaveFont2.Text = "Save 2";
			buttonSaveFont2.UseVisualStyleBackColor = true;
			buttonSaveFont2.Click += SaveFont2_Click;
			// 
			// buttonLoadFont2
			// 
			buttonLoadFont2.Location = new Point(52, 35);
			buttonLoadFont2.Margin = new Padding(0);
			buttonLoadFont2.Name = "buttonLoadFont2";
			buttonLoadFont2.Size = new Size(50, 20);
			buttonLoadFont2.TabIndex = 7;
			buttonLoadFont2.Text = "Load 2";
			toolTips.SetToolTip(buttonLoadFont2, "Load font");
			buttonLoadFont2.UseVisualStyleBackColor = true;
			buttonLoadFont2.Click += LoadFont2_Click;
			// 
			// buttonSaveFont2As
			// 
			buttonSaveFont2As.Location = new Point(52, 80);
			buttonSaveFont2As.Margin = new Padding(0);
			buttonSaveFont2As.Name = "buttonSaveFont2As";
			buttonSaveFont2As.Size = new Size(50, 20);
			buttonSaveFont2As.TabIndex = 8;
			buttonSaveFont2As.Text = "as...";
			buttonSaveFont2As.UseVisualStyleBackColor = true;
			buttonSaveFont2As.Click += SaveFont2As_Click;
			// 
			// p_zz
			// 
			p_zz.BorderStyle = BorderStyle.FixedSingle;
			p_zz.Controls.Add(Bevel4);
			p_zz.Controls.Add(buttonShowColorSwitchSetup);
			p_zz.Controls.Add(buttonSwitchGraphicsMode);
			p_zz.Controls.Add(buttonExportFont);
			p_zz.Controls.Add(buttonRecolor);
			p_zz.Location = new Point(409, 0);
			p_zz.Name = "p_zz";
			p_zz.Size = new Size(105, 177);
			p_zz.TabIndex = 2;
			// 
			// Bevel4
			// 
			Bevel4.BackgroundImageLayout = ImageLayout.None;
			Bevel4.BorderStyle = BorderStyle.FixedSingle;
			Bevel4.Controls.Add(pictureBoxPalette);
			Bevel4.Location = new Point(7, 39);
			Bevel4.Margin = new Padding(0);
			Bevel4.Name = "Bevel4";
			Bevel4.Size = new Size(92, 56);
			Bevel4.TabIndex = 0;
			// 
			// pictureBoxPalette
			// 
			pictureBoxPalette.Location = new Point(0, 0);
			pictureBoxPalette.Margin = new Padding(0);
			pictureBoxPalette.Name = "pictureBoxPalette";
			pictureBoxPalette.Size = new Size(90, 54);
			pictureBoxPalette.TabIndex = 1;
			pictureBoxPalette.TabStop = false;
			toolTips.SetToolTip(pictureBoxPalette, "Shift+Click to restore default");
			pictureBoxPalette.MouseDown += Palette_MouseDown;
			// 
			// buttonShowColorSwitchSetup
			// 
			buttonShowColorSwitchSetup.Enabled = false;
			buttonShowColorSwitchSetup.Image = (Image)resources.GetObject("buttonShowColorSwitchSetup.Image");
			buttonShowColorSwitchSetup.Location = new Point(72, 114);
			buttonShowColorSwitchSetup.Name = "buttonShowColorSwitchSetup";
			buttonShowColorSwitchSetup.Size = new Size(22, 22);
			buttonShowColorSwitchSetup.TabIndex = 2;
			buttonShowColorSwitchSetup.Click += ShowColorSwitchSetup_Click;
			// 
			// buttonSwitchGraphicsMode
			// 
			buttonSwitchGraphicsMode.Location = new Point(8, 8);
			buttonSwitchGraphicsMode.Name = "buttonSwitchGraphicsMode";
			buttonSwitchGraphicsMode.Size = new Size(89, 25);
			buttonSwitchGraphicsMode.TabIndex = 0;
			buttonSwitchGraphicsMode.Text = "Change GFX";
			toolTips.SetToolTip(buttonSwitchGraphicsMode, "Mode 2 / 4");
			buttonSwitchGraphicsMode.UseVisualStyleBackColor = true;
			buttonSwitchGraphicsMode.Click += SwitchGraphicsMode_Click;
			// 
			// buttonExportFont
			// 
			buttonExportFont.Location = new Point(8, 144);
			buttonExportFont.Name = "buttonExportFont";
			buttonExportFont.Size = new Size(89, 25);
			buttonExportFont.TabIndex = 1;
			buttonExportFont.Text = "Export font";
			buttonExportFont.UseVisualStyleBackColor = true;
			buttonExportFont.Click += ExportFont_Click;
			// 
			// buttonRecolor
			// 
			buttonRecolor.Enabled = false;
			buttonRecolor.Location = new Point(8, 112);
			buttonRecolor.Name = "buttonRecolor";
			buttonRecolor.Size = new Size(64, 25);
			buttonRecolor.TabIndex = 2;
			buttonRecolor.Text = "Recolor";
			buttonRecolor.UseVisualStyleBackColor = true;
			buttonRecolor.Click += Recolor_Click;
			// 
			// buttonLoadView
			// 
			buttonLoadView.Location = new Point(864, 416);
			buttonLoadView.Name = "buttonLoadView";
			buttonLoadView.Size = new Size(89, 25);
			buttonLoadView.TabIndex = 3;
			buttonLoadView.Text = "Load View";
			buttonLoadView.UseVisualStyleBackColor = true;
			buttonLoadView.Click += ViewEditor_LoadView_Click;
			// 
			// buttonClearView
			// 
			buttonClearView.Location = new Point(768, 416);
			buttonClearView.Name = "buttonClearView";
			buttonClearView.Size = new Size(89, 25);
			buttonClearView.TabIndex = 4;
			buttonClearView.Text = "Clear View";
			buttonClearView.UseVisualStyleBackColor = true;
			buttonClearView.Click += ViewEditor_ClearView_Click;
			// 
			// buttonSaveView
			// 
			buttonSaveView.Location = new Point(960, 416);
			buttonSaveView.Name = "buttonSaveView";
			buttonSaveView.Size = new Size(89, 25);
			buttonSaveView.TabIndex = 5;
			buttonSaveView.Text = "Save View";
			buttonSaveView.UseVisualStyleBackColor = true;
			buttonSaveView.Click += ViewEditor_SaveView_Click;
			// 
			// p_status
			// 
			p_status.Controls.Add(checkBoxFontBank);
			p_status.Controls.Add(pictureBoxActionColor);
			p_status.Controls.Add(labelEditCharInfo);
			p_status.Controls.Add(labelColor);
			p_status.Controls.Add(checkBoxShowDuplicates);
			p_status.Controls.Add(buttonMegaCopy);
			p_status.Controls.Add(buttonUndo);
			p_status.Controls.Add(buttonRedo);
			p_status.Controls.Add(comboBoxWriteMode);
			p_status.Location = new Point(-1, 179);
			p_status.Name = "p_status";
			p_status.Size = new Size(515, 25);
			p_status.TabIndex = 6;
			// 
			// checkBoxFontBank
			// 
			checkBoxFontBank.Appearance = Appearance.Button;
			checkBoxFontBank.AutoSize = true;
			checkBoxFontBank.FlatAppearance.CheckedBackColor = SystemColors.Control;
			checkBoxFontBank.FlatAppearance.MouseDownBackColor = SystemColors.Control;
			checkBoxFontBank.FlatAppearance.MouseOverBackColor = SystemColors.Control;
			checkBoxFontBank.FlatStyle = FlatStyle.Flat;
			checkBoxFontBank.ImageIndex = 0;
			checkBoxFontBank.ImageList = imageListFont1234;
			checkBoxFontBank.Location = new Point(469, 3);
			checkBoxFontBank.Name = "checkBoxFontBank";
			checkBoxFontBank.Size = new Size(46, 22);
			checkBoxFontBank.TabIndex = 12;
			checkBoxFontBank.TextImageRelation = TextImageRelation.ImageBeforeText;
			checkBoxFontBank.UseMnemonic = false;
			checkBoxFontBank.UseVisualStyleBackColor = true;
			checkBoxFontBank.CheckedChanged += FontBank_CheckedChanged;
			checkBoxFontBank.Click += FontBank_Click;
			// 
			// imageListFont1234
			// 
			imageListFont1234.ColorDepth = ColorDepth.Depth8Bit;
			imageListFont1234.ImageStream = (ImageListStreamer)resources.GetObject("imageListFont1234.ImageStream");
			imageListFont1234.TransparentColor = Color.Transparent;
			imageListFont1234.Images.SetKeyName(0, "12.bmp");
			imageListFont1234.Images.SetKeyName(1, "34.bmp");
			// 
			// pictureBoxActionColor
			// 
			pictureBoxActionColor.Location = new Point(228, 6);
			pictureBoxActionColor.Name = "pictureBoxActionColor";
			pictureBoxActionColor.Size = new Size(49, 17);
			pictureBoxActionColor.TabIndex = 0;
			pictureBoxActionColor.TabStop = false;
			// 
			// labelEditCharInfo
			// 
			labelEditCharInfo.AutoSize = true;
			labelEditCharInfo.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			labelEditCharInfo.Location = new Point(8, 7);
			labelEditCharInfo.Name = "labelEditCharInfo";
			labelEditCharInfo.Size = new Size(102, 13);
			labelEditCharInfo.TabIndex = 1;
			labelEditCharInfo.Text = "Char: Font 1 $00 #0";
			// 
			// labelColor
			// 
			labelColor.AutoSize = true;
			labelColor.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			labelColor.Location = new Point(195, 7);
			labelColor.Name = "labelColor";
			labelColor.Size = new Size(34, 13);
			labelColor.TabIndex = 2;
			labelColor.Text = "Color:";
			// 
			// checkBoxShowDuplicates
			// 
			checkBoxShowDuplicates.Location = new Point(341, 7);
			checkBoxShowDuplicates.Name = "checkBoxShowDuplicates";
			checkBoxShowDuplicates.Size = new Size(49, 17);
			checkBoxShowDuplicates.TabIndex = 11;
			checkBoxShowDuplicates.Text = "DUP";
			toolTips.SetToolTip(checkBoxShowDuplicates, "Show duplicate characters within font");
			checkBoxShowDuplicates.UseVisualStyleBackColor = true;
			checkBoxShowDuplicates.Click += ShowDuplicates_Click;
			// 
			// buttonMegaCopy
			// 
			buttonMegaCopy.Appearance = Appearance.Button;
			buttonMegaCopy.CheckAlign = ContentAlignment.MiddleCenter;
			buttonMegaCopy.Location = new Point(394, 3);
			buttonMegaCopy.Name = "buttonMegaCopy";
			buttonMegaCopy.Size = new Size(74, 21);
			buttonMegaCopy.TabIndex = 3;
			buttonMegaCopy.Text = "Mega Copy";
			buttonMegaCopy.TextAlign = ContentAlignment.MiddleCenter;
			toolTips.SetToolTip(buttonMegaCopy, "Toggle MegaCopy Mode");
			buttonMegaCopy.Click += MegaCopy_Click;
			// 
			// buttonUndo
			// 
			buttonUndo.Image = (Image)resources.GetObject("buttonUndo.Image");
			buttonUndo.Location = new Point(284, 2);
			buttonUndo.Name = "buttonUndo";
			buttonUndo.Size = new Size(22, 22);
			buttonUndo.TabIndex = 4;
			toolTips.SetToolTip(buttonUndo, "ExecuteUndo Font Change");
			buttonUndo.Click += Undo_Click;
			// 
			// buttonRedo
			// 
			buttonRedo.Image = (Image)resources.GetObject("buttonRedo.Image");
			buttonRedo.Location = new Point(312, 2);
			buttonRedo.Name = "buttonRedo";
			buttonRedo.Size = new Size(22, 22);
			buttonRedo.TabIndex = 5;
			toolTips.SetToolTip(buttonRedo, "Redo Font Change");
			buttonRedo.Click += Redo_Click;
			// 
			// comboBoxWriteMode
			// 
			comboBoxWriteMode.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxWriteMode.FormattingEnabled = true;
			comboBoxWriteMode.Items.AddRange(new object[] { "Rewrite", "Insert" });
			comboBoxWriteMode.Location = new Point(126, 3);
			comboBoxWriteMode.Name = "comboBoxWriteMode";
			comboBoxWriteMode.Size = new Size(66, 21);
			comboBoxWriteMode.TabIndex = 0;
			// 
			// panelColorSwitcher
			// 
			panelColorSwitcher.Controls.Add(pictureBoxRecolorSourceColor);
			panelColorSwitcher.Controls.Add(pictureBoxRecolorTargetColor);
			panelColorSwitcher.Controls.Add(listBoxRecolorSource);
			panelColorSwitcher.Controls.Add(listBoxRecolorTarget);
			panelColorSwitcher.Location = new Point(544, 8);
			panelColorSwitcher.Name = "panelColorSwitcher";
			panelColorSwitcher.Size = new Size(153, 97);
			panelColorSwitcher.TabIndex = 7;
			panelColorSwitcher.Visible = false;
			// 
			// pictureBoxRecolorSourceColor
			// 
			pictureBoxRecolorSourceColor.Location = new Point(16, 77);
			pictureBoxRecolorSourceColor.Name = "pictureBoxRecolorSourceColor";
			pictureBoxRecolorSourceColor.Size = new Size(49, 17);
			pictureBoxRecolorSourceColor.TabIndex = 0;
			pictureBoxRecolorSourceColor.TabStop = false;
			pictureBoxRecolorSourceColor.Tag = 4;
			// 
			// pictureBoxRecolorTargetColor
			// 
			pictureBoxRecolorTargetColor.Location = new Point(88, 77);
			pictureBoxRecolorTargetColor.Name = "pictureBoxRecolorTargetColor";
			pictureBoxRecolorTargetColor.Size = new Size(49, 17);
			pictureBoxRecolorTargetColor.TabIndex = 1;
			pictureBoxRecolorTargetColor.TabStop = false;
			pictureBoxRecolorTargetColor.Tag = 4;
			// 
			// listBoxRecolorSource
			// 
			listBoxRecolorSource.FormattingEnabled = true;
			listBoxRecolorSource.Items.AddRange(new object[] { "BAK (00)", "PF0 (01)", "PF1 (10)", "PF2 (11)" });
			listBoxRecolorSource.Location = new Point(8, 8);
			listBoxRecolorSource.Name = "listBoxRecolorSource";
			listBoxRecolorSource.Size = new Size(65, 56);
			listBoxRecolorSource.TabIndex = 0;
			listBoxRecolorSource.Click += RecolorSource_Click;
			// 
			// listBoxRecolorTarget
			// 
			listBoxRecolorTarget.FormattingEnabled = true;
			listBoxRecolorTarget.Items.AddRange(new object[] { "BAK (00)", "PF0 (01)", "PF1 (10)", "PF2 (11)" });
			listBoxRecolorTarget.Location = new Point(80, 8);
			listBoxRecolorTarget.Name = "listBoxRecolorTarget";
			listBoxRecolorTarget.Size = new Size(65, 56);
			listBoxRecolorTarget.TabIndex = 1;
			listBoxRecolorTarget.Click += RecolorTarget_Click;
			// 
			// buttonEnterText
			// 
			buttonEnterText.Enabled = false;
			buttonEnterText.Location = new Point(520, 416);
			buttonEnterText.Name = "buttonEnterText";
			buttonEnterText.Size = new Size(69, 25);
			buttonEnterText.TabIndex = 9;
			buttonEnterText.Text = "Enter text";
			toolTips.SetToolTip(buttonEnterText, "Text to clipboard, hold SHIFT while clicking to inverse");
			buttonEnterText.UseVisualStyleBackColor = true;
			buttonEnterText.Click += ViewEditor_EnterText_Click;
			// 
			// checkBox40Bytes
			// 
			checkBox40Bytes.AutoSize = true;
			checkBox40Bytes.Location = new Point(696, 422);
			checkBox40Bytes.Name = "checkBox40Bytes";
			checkBox40Bytes.Size = new Size(67, 17);
			checkBox40Bytes.TabIndex = 10;
			checkBox40Bytes.Text = "40 Bytes";
			toolTips.SetToolTip(checkBox40Bytes, "Switch between 32 and 40 byte screen width");
			checkBox40Bytes.UseVisualStyleBackColor = true;
			checkBox40Bytes.Click += ViewEditor_CheckBox40Bytes_Click;
			// 
			// dialogOpenFile
			// 
			dialogOpenFile.FileName = "Default";
			// 
			// dialogSaveFile
			// 
			dialogSaveFile.FileName = "Default";
			// 
			// timerAutoCloseAboutBox
			// 
			timerAutoCloseAboutBox.Interval = 5000;
			timerAutoCloseAboutBox.Tick += AutoCloseAboutBox_Tick;
			// 
			// timerDuplicates
			// 
			timerDuplicates.Tick += TimerDuplicates_Tick;
			// 
			// pictureBoxFontSelectorRubberBand
			// 
			pictureBoxFontSelectorRubberBand.BackColor = Color.Transparent;
			pictureBoxFontSelectorRubberBand.Location = new Point(167, 296);
			pictureBoxFontSelectorRubberBand.Margin = new Padding(0);
			pictureBoxFontSelectorRubberBand.Name = "pictureBoxFontSelectorRubberBand";
			pictureBoxFontSelectorRubberBand.Size = new Size(20, 20);
			pictureBoxFontSelectorRubberBand.TabIndex = 12;
			pictureBoxFontSelectorRubberBand.TabStop = false;
			pictureBoxFontSelectorRubberBand.MouseDown += FontSelector_RubberBand_MouseDown;
			pictureBoxFontSelectorRubberBand.MouseMove += FontSelector_RubberBand_MouseMove;
			pictureBoxFontSelectorRubberBand.MouseUp += FontSelector_RubberBand_MouseUp;
			pictureBoxFontSelectorRubberBand.Resize += FontSelector_RubberBand_Resize;
			// 
			// pictureBoxViewEditorRubberBand
			// 
			pictureBoxViewEditorRubberBand.BackColor = Color.Transparent;
			pictureBoxViewEditorRubberBand.BorderStyle = BorderStyle.FixedSingle;
			pictureBoxViewEditorRubberBand.Location = new Point(696, 296);
			pictureBoxViewEditorRubberBand.Margin = new Padding(0);
			pictureBoxViewEditorRubberBand.Name = "pictureBoxViewEditorRubberBand";
			pictureBoxViewEditorRubberBand.Size = new Size(20, 20);
			pictureBoxViewEditorRubberBand.TabIndex = 13;
			pictureBoxViewEditorRubberBand.TabStop = false;
			pictureBoxViewEditorRubberBand.Visible = false;
			pictureBoxViewEditorRubberBand.MouseDown += ViewEditor_RubberBand_MouseDown;
			pictureBoxViewEditorRubberBand.MouseMove += ViewEditor_RubberBand_MouseMove;
			pictureBoxViewEditorRubberBand.MouseUp += ViewEditor_RubberBand_MouseUp;
			pictureBoxViewEditorRubberBand.Resize += ViewEditor_RubberBand_Resize;
			// 
			// pictureBoxFontSelectorPasteCursor
			// 
			pictureBoxFontSelectorPasteCursor.BackColor = Color.Transparent;
			pictureBoxFontSelectorPasteCursor.BorderStyle = BorderStyle.FixedSingle;
			pictureBoxFontSelectorPasteCursor.Location = new Point(167, 329);
			pictureBoxFontSelectorPasteCursor.Margin = new Padding(0);
			pictureBoxFontSelectorPasteCursor.Name = "pictureBoxFontSelectorPasteCursor";
			pictureBoxFontSelectorPasteCursor.Size = new Size(20, 20);
			pictureBoxFontSelectorPasteCursor.TabIndex = 14;
			pictureBoxFontSelectorPasteCursor.TabStop = false;
			pictureBoxFontSelectorPasteCursor.Visible = false;
			pictureBoxFontSelectorPasteCursor.MouseDown += FontSelector_PasteCursor_MouseDown;
			pictureBoxFontSelectorPasteCursor.MouseLeave += FontSelector_PasteCursor_MouseLeave;
			pictureBoxFontSelectorPasteCursor.MouseMove += FontSelector_PasteCursor_MouseMove;
			pictureBoxFontSelectorPasteCursor.MouseUp += FontSelector_PasteCursor_MouseUp;
			// 
			// pictureBoxViewEditorPasteCursor
			// 
			pictureBoxViewEditorPasteCursor.BackColor = Color.Transparent;
			pictureBoxViewEditorPasteCursor.BorderStyle = BorderStyle.FixedSingle;
			pictureBoxViewEditorPasteCursor.Location = new Point(696, 329);
			pictureBoxViewEditorPasteCursor.Margin = new Padding(0);
			pictureBoxViewEditorPasteCursor.Name = "pictureBoxViewEditorPasteCursor";
			pictureBoxViewEditorPasteCursor.Size = new Size(20, 20);
			pictureBoxViewEditorPasteCursor.TabIndex = 15;
			pictureBoxViewEditorPasteCursor.TabStop = false;
			pictureBoxViewEditorPasteCursor.Visible = false;
			pictureBoxViewEditorPasteCursor.MouseDown += ViewEditor_PasteCursor_MouseDown;
			pictureBoxViewEditorPasteCursor.MouseLeave += ViewEditor_PasteCursor_MouseLeave;
			pictureBoxViewEditorPasteCursor.MouseMove += ViewEditor_PasteCursor_MouseMove;
			pictureBoxViewEditorPasteCursor.MouseUp += ViewEditor_PasteCursor_MouseUp;
			// 
			// comboBoxPages
			// 
			comboBoxPages.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxPages.FormattingEnabled = true;
			comboBoxPages.Location = new Point(695, 443);
			comboBoxPages.Name = "comboBoxPages";
			comboBoxPages.Size = new Size(121, 21);
			comboBoxPages.TabIndex = 16;
			comboBoxPages.SelectedIndexChanged += ViewEditor_Pages_SelectedIndexChanged;
			// 
			// buttonAddPage
			// 
			buttonAddPage.Location = new Point(817, 443);
			buttonAddPage.Name = "buttonAddPage";
			buttonAddPage.Size = new Size(25, 21);
			buttonAddPage.TabIndex = 17;
			buttonAddPage.Text = "+";
			buttonAddPage.UseVisualStyleBackColor = true;
			buttonAddPage.Click += ViewEditor_AddPage_Click;
			// 
			// buttonDeletePage
			// 
			buttonDeletePage.Location = new Point(845, 443);
			buttonDeletePage.Name = "buttonDeletePage";
			buttonDeletePage.Size = new Size(25, 21);
			buttonDeletePage.TabIndex = 18;
			buttonDeletePage.Text = "-";
			buttonDeletePage.UseVisualStyleBackColor = true;
			buttonDeletePage.Click += ViewEditor_DeletePage_Click;
			// 
			// buttonEditPage
			// 
			buttonEditPage.Location = new Point(872, 443);
			buttonEditPage.Name = "buttonEditPage";
			buttonEditPage.Size = new Size(38, 21);
			buttonEditPage.TabIndex = 19;
			buttonEditPage.Text = "Edit";
			buttonEditPage.UseVisualStyleBackColor = true;
			buttonEditPage.Click += ViewEditor_EditPage_Click;
			// 
			// labelCurrentPageIndex
			// 
			labelCurrentPageIndex.Location = new Point(663, 448);
			labelCurrentPageIndex.Name = "labelCurrentPageIndex";
			labelCurrentPageIndex.Size = new Size(30, 15);
			labelCurrentPageIndex.TabIndex = 20;
			labelCurrentPageIndex.Text = "#0";
			labelCurrentPageIndex.TextAlign = ContentAlignment.TopRight;
			labelCurrentPageIndex.UseMnemonic = false;
			// 
			// buttonExportView
			// 
			buttonExportView.Location = new Point(595, 416);
			buttonExportView.Name = "buttonExportView";
			buttonExportView.Size = new Size(70, 25);
			buttonExportView.TabIndex = 21;
			buttonExportView.Text = "Export view";
			buttonExportView.UseVisualStyleBackColor = true;
			buttonExportView.Click += ViewEditor_ExportView_Click;
			// 
			// FontMakerForm
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			BackgroundImageLayout = ImageLayout.None;
			ClientSize = new Size(1048, 466);
			Controls.Add(buttonExportView);
			Controls.Add(labelCurrentPageIndex);
			Controls.Add(buttonEditPage);
			Controls.Add(buttonDeletePage);
			Controls.Add(buttonAddPage);
			Controls.Add(comboBoxPages);
			Controls.Add(pictureBoxAbout);
			Controls.Add(panelColorSwitcher);
			Controls.Add(pictureBoxFontSelectorMegaCopyImage);
			Controls.Add(pictureBoxViewEditorMegaCopyImage);
			Controls.Add(pictureBoxFontSelectorPasteCursor);
			Controls.Add(pictureBoxViewEditorPasteCursor);
			Controls.Add(pictureBoxViewEditorRubberBand);
			Controls.Add(pictureBoxFontSelectorRubberBand);
			Controls.Add(pictureBoxAtariView);
			Controls.Add(pictureBoxFontSelector);
			Controls.Add(pictureBoxCharacterSetSelector);
			Controls.Add(labelViewCharInfo);
			Controls.Add(p_xx);
			Controls.Add(p_hh);
			Controls.Add(p_zz);
			Controls.Add(buttonLoadView);
			Controls.Add(buttonClearView);
			Controls.Add(buttonSaveView);
			Controls.Add(p_status);
			Controls.Add(buttonEnterText);
			Controls.Add(checkBox40Bytes);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = (Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			Name = "FontMakerForm";
			Text = "TheApp";
			FormClosing += Form_CloseQuery;
			KeyDown += Form_KeyDown;
			MouseWheel += Form_MouseWheel;
			((System.ComponentModel.ISupportInitialize)pictureBoxAtariView).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxFontSelector).EndInit();
			pictureBoxFontSelector.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBoxDuplicateIndicator).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxAbout).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxCharacterSetSelector).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxFontSelectorMegaCopyImage).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxViewEditorMegaCopyImage).EndInit();
			p_xx.ResumeLayout(false);
			Bevel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBoxCharacterEditor).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxCharacterEditorColor1).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxCharacterEditorColor2).EndInit();
			p_hh.ResumeLayout(false);
			p_zz.ResumeLayout(false);
			Bevel4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBoxPalette).EndInit();
			p_status.ResumeLayout(false);
			p_status.PerformLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxActionColor).EndInit();
			panelColorSwitcher.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBoxRecolorSourceColor).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxRecolorTargetColor).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxFontSelectorRubberBand).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxViewEditorRubberBand).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxFontSelectorPasteCursor).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxViewEditorPasteCursor).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxAtariView;
		private System.Windows.Forms.PictureBox pictureBoxFontSelector;
		private System.Windows.Forms.PictureBox pictureBoxAbout;
		private System.Windows.Forms.PictureBox pictureBoxCharacterSetSelector;

		private System.Windows.Forms.PictureBox pictureBoxFontSelectorMegaCopyImage;
		private System.Windows.Forms.PictureBox pictureBoxViewEditorMegaCopyImage;
		private System.Windows.Forms.Label labelViewCharInfo;

		private System.Windows.Forms.Panel p_xx;
		private System.Windows.Forms.Panel Bevel3;
		private System.Windows.Forms.PictureBox pictureBoxCharacterEditor;
		private System.Windows.Forms.PictureBox pictureBoxCharacterEditorColor1;
		private System.Windows.Forms.PictureBox pictureBoxCharacterEditorColor2;
		private System.Windows.Forms.Button buttonShiftUp;
		private System.Windows.Forms.Button buttonMirrorVertical;
		private System.Windows.Forms.Button buttonShiftRight;
		private System.Windows.Forms.Button buttonShiftLeft;
		private System.Windows.Forms.Button buttonShiftDown;
		private System.Windows.Forms.Button buttonRotateRight;
		private System.Windows.Forms.Button buttonRotateLeft;
		private System.Windows.Forms.Button buttonRestoreSaved;
		private System.Windows.Forms.Button buttonRestoreDefault;
		private System.Windows.Forms.Button buttonInverse;
		private System.Windows.Forms.Button buttonMirrorHorizontal;
		private System.Windows.Forms.Button buttonClear;
		private System.Windows.Forms.Button buttonCopy;
		private System.Windows.Forms.Button buttonPaste;
		private System.Windows.Forms.Panel p_hh;
		private System.Windows.Forms.Button buttonLoadFont1;
		private System.Windows.Forms.Button buttonSaveFont1As;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.Button buttonAbout;
		private System.Windows.Forms.Button buttonQuit;
		private System.Windows.Forms.Button buttonSaveFont1;
		private System.Windows.Forms.Button buttonSaveFont2;
		private System.Windows.Forms.Button buttonLoadFont2;
		private System.Windows.Forms.Button buttonSaveFont2As;
		private System.Windows.Forms.Panel p_zz;
		private System.Windows.Forms.Panel Bevel4;
		private System.Windows.Forms.PictureBox pictureBoxPalette;
		private System.Windows.Forms.Button buttonShowColorSwitchSetup;
		private System.Windows.Forms.Button buttonSwitchGraphicsMode;
		private System.Windows.Forms.Button buttonExportFont;
		private System.Windows.Forms.Button buttonRecolor;
		private System.Windows.Forms.Button buttonLoadView;
		private System.Windows.Forms.Button buttonClearView;
		private System.Windows.Forms.Button buttonSaveView;
		private System.Windows.Forms.Panel p_status;
		private System.Windows.Forms.PictureBox pictureBoxActionColor;
		private System.Windows.Forms.Label labelEditCharInfo;
		private System.Windows.Forms.Label labelColor;
		private System.Windows.Forms.CheckBox buttonMegaCopy;
		private System.Windows.Forms.Button buttonUndo;
		private System.Windows.Forms.Button buttonRedo;
		private System.Windows.Forms.ComboBox comboBoxWriteMode;
		private System.Windows.Forms.Panel panelColorSwitcher;
		private System.Windows.Forms.PictureBox pictureBoxRecolorSourceColor;
		private System.Windows.Forms.PictureBox pictureBoxRecolorTargetColor;
		private System.Windows.Forms.ListBox listBoxRecolorSource;
		private System.Windows.Forms.ListBox listBoxRecolorTarget;
		private System.Windows.Forms.Button buttonEnterText;
		private System.Windows.Forms.CheckBox checkBox40Bytes;
		private System.Windows.Forms.CheckBox checkBoxShowDuplicates;
		private System.Windows.Forms.OpenFileDialog dialogOpenFile;
		private System.Windows.Forms.SaveFileDialog dialogSaveFile;
		private System.Windows.Forms.Timer timerAutoCloseAboutBox;
		private System.Windows.Forms.PictureBox pictureBoxDuplicateIndicator;
		private System.Windows.Forms.Timer timerDuplicates;




		private PictureBox pictureBoxFontSelectorRubberBand;
		private PictureBox pictureBoxViewEditorRubberBand;
		private PictureBox pictureBoxFontSelectorPasteCursor;
		private PictureBox pictureBoxViewEditorPasteCursor;
		private ToolTip toolTips;
		private ComboBox comboBoxPages;
		private Button buttonAddPage;
		private Button buttonDeletePage;
		private Button buttonEditPage;
		private Label labelCurrentPageIndex;
		private CheckBox checkBoxFontBank;
		private ImageList imageListFont1234;
		private Button buttonClearFont1;
		private Button buttonClearFont2;
		private Button buttonExportView;
	}
}