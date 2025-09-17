namespace FontMaker;

partial class TileSetEditorWindow
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
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
		var resources = new System.ComponentModel.ComponentResourceManager(typeof(TileSetEditorWindow));
		hScrollBarTiles = new HScrollBar();
		pictureBoxTileSets = new PictureBox();
		imageListViewShift = new ImageList(components);
		pictureBoxFontSelector = new PictureBox();
		buttonTilePaste = new Button();
		buttonTileCopy = new Button();
		labelTileNr = new Label();
		buttonNextTile = new Button();
		buttonPrevTile = new Button();
		pictureBoxEditTile = new PictureBox();
		pictureBoxFontSelectorRubberBand = new PictureBox();
		label4 = new Label();
		label1 = new Label();
		labelFontNr = new Label();
		buttonNextFontNr = new Button();
		buttonPrevFontNr = new Button();
		labelEditCharInfo = new Label();
		buttonTileClear = new Button();
		pictureBoxCharacterSetSelector = new PictureBox();
		labelTile0 = new Label();
		labelTile1 = new Label();
		labelTile2 = new Label();
		labelTile3 = new Label();
		labelTile4 = new Label();
		labelTile5 = new Label();
		labelTile6 = new Label();
		labelTile7 = new Label();
		pictureBoxCurrenctChar = new PictureBox();
		buttonLoadCurrentTile = new Button();
		label7 = new Label();
		buttonSaveCurrentTile = new Button();
		buttonLoadTileSet = new Button();
		buttonSaveTileSet = new Button();
		buttonNewTileSet = new Button();
		dialogOpenFile = new OpenFileDialog();
		dialogSaveFile = new SaveFileDialog();
		buttonRedraw = new Button();
		buttonUse = new Button();
		buttonRotateLeft = new Button();
		buttonRotateRight = new Button();
		buttonMirrorH = new Button();
		buttonMirrorV = new Button();
		buttonShiftLeft = new Button();
		ShiftRight = new Button();
		buttonShiftDown = new Button();
		buttonShiftUp = new Button();
		buttonViewUndo = new Button();
		label8 = new Label();
		buttonViewRedo = new Button();
		checkBoxShowGrid = new CheckBox();
		((System.ComponentModel.ISupportInitialize)pictureBoxTileSets).BeginInit();
		((System.ComponentModel.ISupportInitialize)pictureBoxFontSelector).BeginInit();
		((System.ComponentModel.ISupportInitialize)pictureBoxEditTile).BeginInit();
		((System.ComponentModel.ISupportInitialize)pictureBoxFontSelectorRubberBand).BeginInit();
		((System.ComponentModel.ISupportInitialize)pictureBoxCharacterSetSelector).BeginInit();
		((System.ComponentModel.ISupportInitialize)pictureBoxCurrenctChar).BeginInit();
		SuspendLayout();
		// 
		// hScrollBarTiles
		// 
		hScrollBarTiles.LargeChange = 8;
		hScrollBarTiles.Location = new Point(0, 146);
		hScrollBarTiles.Maximum = 99;
		hScrollBarTiles.Name = "hScrollBarTiles";
		hScrollBarTiles.ScaleScrollBarForDpiChange = false;
		hScrollBarTiles.Size = new Size(1024, 17);
		hScrollBarTiles.TabIndex = 0;
		hScrollBarTiles.ValueChanged += hScrollBarTiles_ValueChanged;
		// 
		// pictureBoxTileSets
		// 
		pictureBoxTileSets.BackColor = SystemColors.ControlDark;
		pictureBoxTileSets.Location = new Point(0, 0);
		pictureBoxTileSets.Margin = new Padding(0);
		pictureBoxTileSets.Name = "pictureBoxTileSets";
		pictureBoxTileSets.Size = new Size(1024, 128);
		pictureBoxTileSets.TabIndex = 1;
		pictureBoxTileSets.TabStop = false;
		pictureBoxTileSets.DoubleClick += pictureBoxTileSets_DoubleClick;
		pictureBoxTileSets.MouseDown += pictureBoxTileSets_MouseDown;
		pictureBoxTileSets.MouseMove += pictureBoxTileSets_MouseMove;
		// 
		// imageListViewShift
		// 
		imageListViewShift.ColorDepth = ColorDepth.Depth8Bit;
		imageListViewShift.ImageStream = (ImageListStreamer)resources.GetObject("imageListViewShift.ImageStream");
		imageListViewShift.TransparentColor = Color.Transparent;
		imageListViewShift.Images.SetKeyName(0, "Left.bmp");
		imageListViewShift.Images.SetKeyName(1, "Right.bmp");
		// 
		// pictureBoxFontSelector
		// 
		pictureBoxFontSelector.BorderStyle = BorderStyle.FixedSingle;
		pictureBoxFontSelector.Location = new Point(462, 174);
		pictureBoxFontSelector.Margin = new Padding(0);
		pictureBoxFontSelector.Name = "pictureBoxFontSelector";
		pictureBoxFontSelector.Size = new Size(514, 130);
		pictureBoxFontSelector.TabIndex = 43;
		pictureBoxFontSelector.TabStop = false;
		pictureBoxFontSelector.MouseDown += FontSelector_MouseDown;
		// 
		// buttonTilePaste
		// 
		buttonTilePaste.AutoSize = true;
		buttonTilePaste.Location = new Point(254, 272);
		buttonTilePaste.Name = "buttonTilePaste";
		buttonTilePaste.Size = new Size(82, 23);
		buttonTilePaste.TabIndex = 53;
		buttonTilePaste.Text = "Paste";
		buttonTilePaste.UseVisualStyleBackColor = true;
		buttonTilePaste.Click += buttonTilePaste_Click;
		// 
		// buttonTileCopy
		// 
		buttonTileCopy.AutoSize = true;
		buttonTileCopy.Location = new Point(7, 272);
		buttonTileCopy.Margin = new Padding(0);
		buttonTileCopy.Name = "buttonTileCopy";
		buttonTileCopy.Size = new Size(82, 23);
		buttonTileCopy.TabIndex = 52;
		buttonTileCopy.Text = "Copy";
		buttonTileCopy.UseVisualStyleBackColor = true;
		buttonTileCopy.Click += buttonTileCopy_Click;
		// 
		// labelTileNr
		// 
		labelTileNr.BackColor = Color.FromArgb(255, 128, 0);
		labelTileNr.BorderStyle = BorderStyle.FixedSingle;
		labelTileNr.Location = new Point(35, 300);
		labelTileNr.Name = "labelTileNr";
		labelTileNr.Size = new Size(28, 20);
		labelTileNr.TabIndex = 51;
		labelTileNr.Text = "1";
		labelTileNr.TextAlign = ContentAlignment.MiddleCenter;
		labelTileNr.UseMnemonic = false;
		// 
		// buttonNextTile
		// 
		buttonNextTile.ImageIndex = 1;
		buttonNextTile.ImageList = imageListViewShift;
		buttonNextTile.Location = new Point(65, 298);
		buttonNextTile.Name = "buttonNextTile";
		buttonNextTile.Size = new Size(24, 24);
		buttonNextTile.TabIndex = 50;
		buttonNextTile.TextImageRelation = TextImageRelation.ImageBeforeText;
		buttonNextTile.UseVisualStyleBackColor = true;
		buttonNextTile.Click += buttonNextTile_Click;
		// 
		// buttonPrevTile
		// 
		buttonPrevTile.ImageIndex = 0;
		buttonPrevTile.ImageList = imageListViewShift;
		buttonPrevTile.Location = new Point(9, 298);
		buttonPrevTile.Name = "buttonPrevTile";
		buttonPrevTile.Size = new Size(24, 24);
		buttonPrevTile.TabIndex = 49;
		buttonPrevTile.TextImageRelation = TextImageRelation.ImageBeforeText;
		buttonPrevTile.UseVisualStyleBackColor = true;
		buttonPrevTile.Click += buttonPrevTile_Click;
		// 
		// pictureBoxEditTile
		// 
		pictureBoxEditTile.Location = new Point(116, 180);
		pictureBoxEditTile.Margin = new Padding(0);
		pictureBoxEditTile.Name = "pictureBoxEditTile";
		pictureBoxEditTile.Size = new Size(128, 128);
		pictureBoxEditTile.TabIndex = 48;
		pictureBoxEditTile.TabStop = false;
		pictureBoxEditTile.MouseDown += pictureBoxEditTile_MouseDown;
		pictureBoxEditTile.MouseMove += pictureBoxEditTile_MouseMove;
		// 
		// pictureBoxFontSelectorRubberBand
		// 
		pictureBoxFontSelectorRubberBand.BackColor = Color.Transparent;
		pictureBoxFontSelectorRubberBand.Location = new Point(473, 186);
		pictureBoxFontSelectorRubberBand.Margin = new Padding(0);
		pictureBoxFontSelectorRubberBand.Name = "pictureBoxFontSelectorRubberBand";
		pictureBoxFontSelectorRubberBand.Size = new Size(20, 20);
		pictureBoxFontSelectorRubberBand.TabIndex = 54;
		pictureBoxFontSelectorRubberBand.TabStop = false;
		// 
		// label4
		// 
		label4.BorderStyle = BorderStyle.FixedSingle;
		label4.Location = new Point(0, 167);
		label4.Name = "label4";
		label4.Size = new Size(1024, 2);
		label4.TabIndex = 55;
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(462, 314);
		label1.Name = "label1";
		label1.Size = new Size(44, 13);
		label1.TabIndex = 56;
		label1.Text = "Font #:";
		// 
		// labelFontNr
		// 
		labelFontNr.BackColor = Color.FromArgb(255, 128, 0);
		labelFontNr.BorderStyle = BorderStyle.FixedSingle;
		labelFontNr.Location = new Point(531, 310);
		labelFontNr.Name = "labelFontNr";
		labelFontNr.Size = new Size(28, 20);
		labelFontNr.TabIndex = 59;
		labelFontNr.Text = "1";
		labelFontNr.TextAlign = ContentAlignment.MiddleCenter;
		labelFontNr.UseMnemonic = false;
		// 
		// buttonNextFontNr
		// 
		buttonNextFontNr.ImageIndex = 1;
		buttonNextFontNr.ImageList = imageListViewShift;
		buttonNextFontNr.Location = new Point(561, 308);
		buttonNextFontNr.Name = "buttonNextFontNr";
		buttonNextFontNr.Size = new Size(24, 24);
		buttonNextFontNr.TabIndex = 58;
		buttonNextFontNr.TextImageRelation = TextImageRelation.ImageBeforeText;
		buttonNextFontNr.UseVisualStyleBackColor = true;
		buttonNextFontNr.Click += buttonNextFontNr_Click;
		// 
		// buttonPrevFontNr
		// 
		buttonPrevFontNr.ImageIndex = 0;
		buttonPrevFontNr.ImageList = imageListViewShift;
		buttonPrevFontNr.Location = new Point(505, 308);
		buttonPrevFontNr.Name = "buttonPrevFontNr";
		buttonPrevFontNr.Size = new Size(24, 24);
		buttonPrevFontNr.TabIndex = 57;
		buttonPrevFontNr.TextImageRelation = TextImageRelation.ImageBeforeText;
		buttonPrevFontNr.UseVisualStyleBackColor = true;
		buttonPrevFontNr.Click += buttonPrevFontNr_Click;
		// 
		// labelEditCharInfo
		// 
		labelEditCharInfo.AutoSize = true;
		labelEditCharInfo.Location = new Point(126, 314);
		labelEditCharInfo.Name = "labelEditCharInfo";
		labelEditCharInfo.Size = new Size(109, 13);
		labelEditCharInfo.TabIndex = 60;
		labelEditCharInfo.Text = "Font 1 $00 #0 @ 0,0";
		labelEditCharInfo.TextAlign = ContentAlignment.MiddleLeft;
		// 
		// buttonTileClear
		// 
		buttonTileClear.AutoSize = true;
		buttonTileClear.Location = new Point(344, 272);
		buttonTileClear.Name = "buttonTileClear";
		buttonTileClear.Size = new Size(81, 23);
		buttonTileClear.TabIndex = 61;
		buttonTileClear.Text = "Clear";
		buttonTileClear.UseVisualStyleBackColor = true;
		buttonTileClear.Click += buttonTileClear_Click;
		// 
		// pictureBoxCharacterSetSelector
		// 
		pictureBoxCharacterSetSelector.Location = new Point(100, 180);
		pictureBoxCharacterSetSelector.Name = "pictureBoxCharacterSetSelector";
		pictureBoxCharacterSetSelector.Size = new Size(15, 128);
		pictureBoxCharacterSetSelector.TabIndex = 64;
		pictureBoxCharacterSetSelector.TabStop = false;
		pictureBoxCharacterSetSelector.MouseDown += pictureBoxCharacterSetSelector_MouseDown;
		// 
		// labelTile0
		// 
		labelTile0.BorderStyle = BorderStyle.Fixed3D;
		labelTile0.Location = new Point(0, 128);
		labelTile0.Name = "labelTile0";
		labelTile0.Size = new Size(128, 18);
		labelTile0.TabIndex = 65;
		labelTile0.Text = "Tile: 1";
		labelTile0.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile1
		// 
		labelTile1.BorderStyle = BorderStyle.Fixed3D;
		labelTile1.Location = new Point(128, 128);
		labelTile1.Name = "labelTile1";
		labelTile1.Size = new Size(128, 18);
		labelTile1.TabIndex = 66;
		labelTile1.Text = "Tile: 2";
		labelTile1.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile2
		// 
		labelTile2.BorderStyle = BorderStyle.Fixed3D;
		labelTile2.Location = new Point(256, 128);
		labelTile2.Name = "labelTile2";
		labelTile2.Size = new Size(128, 18);
		labelTile2.TabIndex = 67;
		labelTile2.Text = "Tile: 3";
		labelTile2.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile3
		// 
		labelTile3.BorderStyle = BorderStyle.Fixed3D;
		labelTile3.Location = new Point(384, 128);
		labelTile3.Name = "labelTile3";
		labelTile3.Size = new Size(128, 18);
		labelTile3.TabIndex = 68;
		labelTile3.Text = "Tile: 4";
		labelTile3.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile4
		// 
		labelTile4.BorderStyle = BorderStyle.Fixed3D;
		labelTile4.Location = new Point(512, 128);
		labelTile4.Name = "labelTile4";
		labelTile4.Size = new Size(128, 18);
		labelTile4.TabIndex = 69;
		labelTile4.Text = "Tile: 5";
		labelTile4.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile5
		// 
		labelTile5.BorderStyle = BorderStyle.Fixed3D;
		labelTile5.Location = new Point(640, 128);
		labelTile5.Name = "labelTile5";
		labelTile5.Size = new Size(128, 18);
		labelTile5.TabIndex = 70;
		labelTile5.Text = "Tile: 6";
		labelTile5.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile6
		// 
		labelTile6.BorderStyle = BorderStyle.Fixed3D;
		labelTile6.Location = new Point(768, 128);
		labelTile6.Name = "labelTile6";
		labelTile6.Size = new Size(128, 18);
		labelTile6.TabIndex = 71;
		labelTile6.Text = "Tile: 7";
		labelTile6.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile7
		// 
		labelTile7.BorderStyle = BorderStyle.Fixed3D;
		labelTile7.Location = new Point(896, 128);
		labelTile7.Name = "labelTile7";
		labelTile7.Size = new Size(128, 18);
		labelTile7.TabIndex = 72;
		labelTile7.Text = "Tile: 8";
		labelTile7.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// pictureBoxCurrenctChar
		// 
		pictureBoxCurrenctChar.BorderStyle = BorderStyle.Fixed3D;
		pictureBoxCurrenctChar.Location = new Point(100, 312);
		pictureBoxCurrenctChar.Name = "pictureBoxCurrenctChar";
		pictureBoxCurrenctChar.Size = new Size(20, 20);
		pictureBoxCurrenctChar.TabIndex = 75;
		pictureBoxCurrenctChar.TabStop = false;
		// 
		// buttonLoadCurrentTile
		// 
		buttonLoadCurrentTile.AutoSize = true;
		buttonLoadCurrentTile.Location = new Point(344, 226);
		buttonLoadCurrentTile.Margin = new Padding(0);
		buttonLoadCurrentTile.Name = "buttonLoadCurrentTile";
		buttonLoadCurrentTile.Size = new Size(81, 23);
		buttonLoadCurrentTile.TabIndex = 80;
		buttonLoadCurrentTile.Text = "Load Tile";
		buttonLoadCurrentTile.UseVisualStyleBackColor = true;
		buttonLoadCurrentTile.Click += buttonLoadCurrentTile_Click;
		// 
		// label7
		// 
		label7.BorderStyle = BorderStyle.FixedSingle;
		label7.Location = new Point(439, 172);
		label7.Name = "label7";
		label7.Size = new Size(2, 193);
		label7.TabIndex = 81;
		// 
		// buttonSaveCurrentTile
		// 
		buttonSaveCurrentTile.AutoSize = true;
		buttonSaveCurrentTile.Location = new Point(344, 249);
		buttonSaveCurrentTile.Margin = new Padding(0);
		buttonSaveCurrentTile.Name = "buttonSaveCurrentTile";
		buttonSaveCurrentTile.Size = new Size(81, 23);
		buttonSaveCurrentTile.TabIndex = 82;
		buttonSaveCurrentTile.Text = "Save Tile";
		buttonSaveCurrentTile.UseVisualStyleBackColor = true;
		buttonSaveCurrentTile.Click += buttonSaveCurrentTile_Click;
		// 
		// buttonLoadTileSet
		// 
		buttonLoadTileSet.AutoSize = true;
		buttonLoadTileSet.Location = new Point(344, 295);
		buttonLoadTileSet.Margin = new Padding(0);
		buttonLoadTileSet.Name = "buttonLoadTileSet";
		buttonLoadTileSet.Size = new Size(81, 23);
		buttonLoadTileSet.TabIndex = 83;
		buttonLoadTileSet.Text = "Load Tile Set";
		buttonLoadTileSet.UseVisualStyleBackColor = true;
		buttonLoadTileSet.Click += buttonLoadTileSet_Click;
		// 
		// buttonSaveTileSet
		// 
		buttonSaveTileSet.AutoSize = true;
		buttonSaveTileSet.Location = new Point(344, 318);
		buttonSaveTileSet.Margin = new Padding(0);
		buttonSaveTileSet.Name = "buttonSaveTileSet";
		buttonSaveTileSet.Size = new Size(81, 23);
		buttonSaveTileSet.TabIndex = 84;
		buttonSaveTileSet.Text = "Save Tile Set";
		buttonSaveTileSet.UseVisualStyleBackColor = true;
		buttonSaveTileSet.Click += buttonSaveTileSet_Click;
		// 
		// buttonNewTileSet
		// 
		buttonNewTileSet.AutoSize = true;
		buttonNewTileSet.Location = new Point(344, 341);
		buttonNewTileSet.Margin = new Padding(0);
		buttonNewTileSet.Name = "buttonNewTileSet";
		buttonNewTileSet.Size = new Size(81, 23);
		buttonNewTileSet.TabIndex = 85;
		buttonNewTileSet.Text = "New Tile Set";
		buttonNewTileSet.UseVisualStyleBackColor = true;
		buttonNewTileSet.Click += buttonNewTileSet_Click;
		// 
		// dialogOpenFile
		// 
		dialogOpenFile.FileName = "Default";
		// 
		// dialogSaveFile
		// 
		dialogSaveFile.FileName = "Default";
		// 
		// buttonRedraw
		// 
		buttonRedraw.Location = new Point(462, 336);
		buttonRedraw.Name = "buttonRedraw";
		buttonRedraw.Size = new Size(162, 23);
		buttonRedraw.TabIndex = 87;
		buttonRedraw.Text = "Redraw / Update All";
		buttonRedraw.UseVisualStyleBackColor = true;
		buttonRedraw.Click += buttonRedraw_Click;
		// 
		// buttonUse
		// 
		buttonUse.BackColor = Color.Gold;
		buttonUse.Location = new Point(344, 180);
		buttonUse.Name = "buttonUse";
		buttonUse.Size = new Size(81, 46);
		buttonUse.TabIndex = 88;
		buttonUse.Text = "Draw with this Tile";
		buttonUse.UseVisualStyleBackColor = false;
		buttonUse.Click += buttonUse_Click;
		// 
		// buttonRotateLeft
		// 
		buttonRotateLeft.Location = new Point(7, 180);
		buttonRotateLeft.Name = "buttonRotateLeft";
		buttonRotateLeft.Size = new Size(82, 23);
		buttonRotateLeft.TabIndex = 91;
		buttonRotateLeft.Text = "Rotate Left";
		buttonRotateLeft.UseVisualStyleBackColor = true;
		buttonRotateLeft.Click += buttonRotateLeft_Click;
		// 
		// buttonRotateRight
		// 
		buttonRotateRight.Location = new Point(254, 180);
		buttonRotateRight.Name = "buttonRotateRight";
		buttonRotateRight.Size = new Size(82, 23);
		buttonRotateRight.TabIndex = 92;
		buttonRotateRight.Text = "Rotate Right";
		buttonRotateRight.UseVisualStyleBackColor = true;
		buttonRotateRight.Click += buttonRotateRight_Click;
		// 
		// buttonMirrorH
		// 
		buttonMirrorH.Location = new Point(7, 203);
		buttonMirrorH.Name = "buttonMirrorH";
		buttonMirrorH.Size = new Size(82, 23);
		buttonMirrorH.TabIndex = 93;
		buttonMirrorH.Text = "Mirror H";
		buttonMirrorH.UseVisualStyleBackColor = true;
		buttonMirrorH.Click += buttonMirrorH_Click;
		// 
		// buttonMirrorV
		// 
		buttonMirrorV.Location = new Point(254, 203);
		buttonMirrorV.Name = "buttonMirrorV";
		buttonMirrorV.Size = new Size(82, 23);
		buttonMirrorV.TabIndex = 94;
		buttonMirrorV.Text = "Mirror V";
		buttonMirrorV.UseVisualStyleBackColor = true;
		buttonMirrorV.Click += buttonMirrorV_Click;
		// 
		// buttonShiftLeft
		// 
		buttonShiftLeft.Location = new Point(7, 226);
		buttonShiftLeft.Name = "buttonShiftLeft";
		buttonShiftLeft.Size = new Size(82, 23);
		buttonShiftLeft.TabIndex = 95;
		buttonShiftLeft.Text = "Shift Left";
		buttonShiftLeft.UseVisualStyleBackColor = true;
		buttonShiftLeft.Click += buttonShiftLeft_Click;
		// 
		// ShiftRight
		// 
		ShiftRight.Location = new Point(254, 226);
		ShiftRight.Name = "ShiftRight";
		ShiftRight.Size = new Size(82, 23);
		ShiftRight.TabIndex = 96;
		ShiftRight.Text = "Shift Right";
		ShiftRight.UseVisualStyleBackColor = true;
		ShiftRight.Click += ShiftRight_Click;
		// 
		// buttonShiftDown
		// 
		buttonShiftDown.Location = new Point(7, 249);
		buttonShiftDown.Name = "buttonShiftDown";
		buttonShiftDown.Size = new Size(82, 23);
		buttonShiftDown.TabIndex = 97;
		buttonShiftDown.Text = "Shift Down";
		buttonShiftDown.UseVisualStyleBackColor = true;
		buttonShiftDown.Click += buttonShiftDown_Click;
		// 
		// buttonShiftUp
		// 
		buttonShiftUp.Location = new Point(254, 249);
		buttonShiftUp.Name = "buttonShiftUp";
		buttonShiftUp.Size = new Size(82, 23);
		buttonShiftUp.TabIndex = 98;
		buttonShiftUp.Text = "Shift Up";
		buttonShiftUp.UseVisualStyleBackColor = true;
		buttonShiftUp.Click += buttonShiftUp_Click;
		// 
		// buttonViewUndo
		// 
		buttonViewUndo.Image = (Image)resources.GetObject("buttonViewUndo.Image");
		buttonViewUndo.Location = new Point(275, 318);
		buttonViewUndo.Name = "buttonViewUndo";
		buttonViewUndo.Size = new Size(22, 22);
		buttonViewUndo.TabIndex = 100;
		buttonViewUndo.Click += buttonViewUndo_Click;
		// 
		// label8
		// 
		label8.AutoSize = true;
		label8.Location = new Point(266, 302);
		label8.Name = "label8";
		label8.Size = new Size(70, 13);
		label8.TabIndex = 101;
		label8.Text = "Undo/Redo:";
		// 
		// buttonViewRedo
		// 
		buttonViewRedo.Image = (Image)resources.GetObject("buttonViewRedo.Image");
		buttonViewRedo.Location = new Point(298, 318);
		buttonViewRedo.Name = "buttonViewRedo";
		buttonViewRedo.Size = new Size(22, 22);
		buttonViewRedo.TabIndex = 102;
		buttonViewRedo.Click += buttonViewRedo_Click;
		// 
		// checkBoxShowGrid
		// 
		checkBoxShowGrid.AutoSize = true;
		checkBoxShowGrid.Checked = true;
		checkBoxShowGrid.CheckState = CheckState.Checked;
		checkBoxShowGrid.Location = new Point(101, 337);
		checkBoxShowGrid.Name = "checkBoxShowGrid";
		checkBoxShowGrid.Size = new Size(80, 17);
		checkBoxShowGrid.TabIndex = 103;
		checkBoxShowGrid.Text = "Show Grid";
		checkBoxShowGrid.UseMnemonic = false;
		checkBoxShowGrid.UseVisualStyleBackColor = true;
		checkBoxShowGrid.CheckStateChanged += checkBoxShowGrid_CheckStateChanged;
		// 
		// TileSetEditorWindow
		// 
		AutoScaleDimensions = new SizeF(6F, 13F);
		AutoScaleMode = AutoScaleMode.Font;
		BackColor = SystemColors.ControlLight;
		ClientSize = new Size(1024, 370);
		Controls.Add(checkBoxShowGrid);
		Controls.Add(buttonViewUndo);
		Controls.Add(label8);
		Controls.Add(buttonViewRedo);
		Controls.Add(buttonShiftUp);
		Controls.Add(buttonShiftDown);
		Controls.Add(ShiftRight);
		Controls.Add(buttonShiftLeft);
		Controls.Add(buttonMirrorV);
		Controls.Add(buttonMirrorH);
		Controls.Add(buttonRotateRight);
		Controls.Add(buttonRotateLeft);
		Controls.Add(buttonUse);
		Controls.Add(buttonRedraw);
		Controls.Add(buttonNewTileSet);
		Controls.Add(buttonSaveTileSet);
		Controls.Add(buttonLoadTileSet);
		Controls.Add(buttonSaveCurrentTile);
		Controls.Add(label7);
		Controls.Add(buttonLoadCurrentTile);
		Controls.Add(pictureBoxCurrenctChar);
		Controls.Add(labelTile7);
		Controls.Add(labelTile6);
		Controls.Add(labelTile5);
		Controls.Add(labelTile4);
		Controls.Add(labelTile3);
		Controls.Add(labelTile2);
		Controls.Add(labelTile1);
		Controls.Add(labelTile0);
		Controls.Add(pictureBoxCharacterSetSelector);
		Controls.Add(buttonTileClear);
		Controls.Add(labelEditCharInfo);
		Controls.Add(labelFontNr);
		Controls.Add(buttonNextFontNr);
		Controls.Add(buttonPrevFontNr);
		Controls.Add(label1);
		Controls.Add(label4);
		Controls.Add(pictureBoxFontSelectorRubberBand);
		Controls.Add(buttonTilePaste);
		Controls.Add(buttonTileCopy);
		Controls.Add(labelTileNr);
		Controls.Add(buttonNextTile);
		Controls.Add(buttonPrevTile);
		Controls.Add(pictureBoxEditTile);
		Controls.Add(pictureBoxFontSelector);
		Controls.Add(pictureBoxTileSets);
		Controls.Add(hScrollBarTiles);
		FormBorderStyle = FormBorderStyle.FixedSingle;
		Icon = (Icon)resources.GetObject("$this.Icon");
		KeyPreview = true;
		MaximizeBox = false;
		Name = "TileSetEditorWindow";
		Text = "Tile Set Editor";
		Activated += TileSetEditorWindow_Activated;
		FormClosing += TileSetEditorWindow_FormClosing;
		Load += TilesWindow_Load;
		KeyDown += TileSetEditorWindow_KeyDown;
		MouseWheel += Form_MouseWheel;
		((System.ComponentModel.ISupportInitialize)pictureBoxTileSets).EndInit();
		((System.ComponentModel.ISupportInitialize)pictureBoxFontSelector).EndInit();
		((System.ComponentModel.ISupportInitialize)pictureBoxEditTile).EndInit();
		((System.ComponentModel.ISupportInitialize)pictureBoxFontSelectorRubberBand).EndInit();
		((System.ComponentModel.ISupportInitialize)pictureBoxCharacterSetSelector).EndInit();
		((System.ComponentModel.ISupportInitialize)pictureBoxCurrenctChar).EndInit();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private HScrollBar hScrollBarTiles;
	private PictureBox pictureBoxTileSets;
	private ImageList imageListViewShift;
	private PictureBox pictureBoxFontSelector;
	private Button buttonTilePaste;
	private Button buttonTileCopy;
	private Label labelTileNr;
	private Button buttonNextTile;
	private Button buttonPrevTile;
	private PictureBox pictureBoxEditTile;
	private PictureBox pictureBoxFontSelectorRubberBand;
	private Label label4;
	private Label label1;
	private Label labelFontNr;
	private Button buttonNextFontNr;
	private Button buttonPrevFontNr;
	private Label labelEditCharInfo;
	private Button buttonTileClear;
	private PictureBox pictureBoxCharacterSetSelector;
	private Label labelTile0;
	private Label labelTile1;
	private Label labelTile2;
	private Label labelTile3;
	private Label labelTile4;
	private Label labelTile5;
	private Label labelTile6;
	private Label labelTile7;
	private PictureBox pictureBoxCurrenctChar;
	private Button buttonLoadCurrentTile;
	private Label label7;
	private Button buttonSaveCurrentTile;
	private Button buttonLoadTileSet;
	private Button buttonSaveTileSet;
	private Button buttonNewTileSet;
	private OpenFileDialog dialogOpenFile;
	private SaveFileDialog dialogSaveFile;
	private Button buttonRedraw;
	private Button buttonUse;
	private Button buttonRotateLeft;
	private Button buttonRotateRight;
	private Button buttonMirrorH;
	private Button buttonMirrorV;
	private Button buttonShiftLeft;
	private Button ShiftRight;
	private Button buttonShiftDown;
	private Button buttonShiftUp;
	private Button buttonViewUndo;
	private Label label8;
	private Button buttonViewRedo;
	private CheckBox checkBoxShowGrid;
}