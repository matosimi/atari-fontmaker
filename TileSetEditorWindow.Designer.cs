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
		labelTile8 = new Label();
		labelTile9 = new Label();
		pictureBoxCurrenctChar = new PictureBox();
		label3 = new Label();
		label5 = new Label();
		label6 = new Label();
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
		label2 = new Label();
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
		hScrollBarTiles.Location = new Point(0, 100);
		hScrollBarTiles.Maximum = 99;
		hScrollBarTiles.Name = "hScrollBarTiles";
		hScrollBarTiles.ScaleScrollBarForDpiChange = false;
		hScrollBarTiles.Size = new Size(800, 17);
		hScrollBarTiles.TabIndex = 0;
		hScrollBarTiles.ValueChanged += hScrollBarTiles_ValueChanged;
		// 
		// pictureBoxTileSets
		// 
		pictureBoxTileSets.BackColor = SystemColors.ControlDark;
		pictureBoxTileSets.Location = new Point(0, 0);
		pictureBoxTileSets.Margin = new Padding(0);
		pictureBoxTileSets.Name = "pictureBoxTileSets";
		pictureBoxTileSets.Size = new Size(800, 80);
		pictureBoxTileSets.TabIndex = 1;
		pictureBoxTileSets.TabStop = false;
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
		pictureBoxFontSelector.Location = new Point(0, 266);
		pictureBoxFontSelector.Margin = new Padding(0);
		pictureBoxFontSelector.Name = "pictureBoxFontSelector";
		pictureBoxFontSelector.Size = new Size(512, 128);
		pictureBoxFontSelector.TabIndex = 43;
		pictureBoxFontSelector.TabStop = false;
		pictureBoxFontSelector.MouseDown += FontSelector_MouseDown;
		// 
		// buttonTilePaste
		// 
		buttonTilePaste.AutoSize = true;
		buttonTilePaste.Location = new Point(217, 220);
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
		buttonTileCopy.Location = new Point(6, 220);
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
		labelTileNr.Location = new Point(147, 236);
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
		buttonNextTile.Location = new Point(177, 234);
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
		buttonPrevTile.Location = new Point(121, 234);
		buttonPrevTile.Name = "buttonPrevTile";
		buttonPrevTile.Size = new Size(24, 24);
		buttonPrevTile.TabIndex = 49;
		buttonPrevTile.TextImageRelation = TextImageRelation.ImageBeforeText;
		buttonPrevTile.UseVisualStyleBackColor = true;
		buttonPrevTile.Click += buttonPrevTile_Click;
		// 
		// pictureBoxEditTile
		// 
		pictureBoxEditTile.Location = new Point(121, 151);
		pictureBoxEditTile.Margin = new Padding(0);
		pictureBoxEditTile.Name = "pictureBoxEditTile";
		pictureBoxEditTile.Size = new Size(80, 80);
		pictureBoxEditTile.TabIndex = 48;
		pictureBoxEditTile.TabStop = false;
		pictureBoxEditTile.MouseDown += pictureBoxEditTile_MouseDown;
		pictureBoxEditTile.MouseMove += pictureBoxEditTile_MouseMove;
		// 
		// pictureBoxFontSelectorRubberBand
		// 
		pictureBoxFontSelectorRubberBand.BackColor = Color.Transparent;
		pictureBoxFontSelectorRubberBand.Location = new Point(17, 288);
		pictureBoxFontSelectorRubberBand.Margin = new Padding(0);
		pictureBoxFontSelectorRubberBand.Name = "pictureBoxFontSelectorRubberBand";
		pictureBoxFontSelectorRubberBand.Size = new Size(20, 20);
		pictureBoxFontSelectorRubberBand.TabIndex = 54;
		pictureBoxFontSelectorRubberBand.TabStop = false;
		// 
		// label4
		// 
		label4.BorderStyle = BorderStyle.FixedSingle;
		label4.Location = new Point(0, 119);
		label4.Name = "label4";
		label4.Size = new Size(800, 2);
		label4.TabIndex = 55;
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(524, 266);
		label1.Name = "label1";
		label1.Size = new Size(44, 13);
		label1.TabIndex = 56;
		label1.Text = "Font #:";
		// 
		// labelFontNr
		// 
		labelFontNr.BackColor = Color.FromArgb(255, 128, 0);
		labelFontNr.BorderStyle = BorderStyle.FixedSingle;
		labelFontNr.Location = new Point(540, 280);
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
		buttonNextFontNr.Location = new Point(570, 278);
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
		buttonPrevFontNr.Location = new Point(514, 278);
		buttonPrevFontNr.Name = "buttonPrevFontNr";
		buttonPrevFontNr.Size = new Size(24, 24);
		buttonPrevFontNr.TabIndex = 57;
		buttonPrevFontNr.TextImageRelation = TextImageRelation.ImageBeforeText;
		buttonPrevFontNr.UseVisualStyleBackColor = true;
		buttonPrevFontNr.Click += buttonPrevFontNr_Click;
		// 
		// labelEditCharInfo
		// 
		labelEditCharInfo.Location = new Point(514, 305);
		labelEditCharInfo.Name = "labelEditCharInfo";
		labelEditCharInfo.Size = new Size(80, 29);
		labelEditCharInfo.TabIndex = 60;
		labelEditCharInfo.Text = "Font 1 $00 #0";
		labelEditCharInfo.TextAlign = ContentAlignment.MiddleLeft;
		// 
		// buttonTileClear
		// 
		buttonTileClear.AutoSize = true;
		buttonTileClear.Location = new Point(310, 220);
		buttonTileClear.Name = "buttonTileClear";
		buttonTileClear.Size = new Size(74, 23);
		buttonTileClear.TabIndex = 61;
		buttonTileClear.Text = "Clear";
		buttonTileClear.UseVisualStyleBackColor = true;
		buttonTileClear.Click += buttonTileClear_Click;
		// 
		// pictureBoxCharacterSetSelector
		// 
		pictureBoxCharacterSetSelector.Location = new Point(105, 151);
		pictureBoxCharacterSetSelector.Name = "pictureBoxCharacterSetSelector";
		pictureBoxCharacterSetSelector.Size = new Size(15, 80);
		pictureBoxCharacterSetSelector.TabIndex = 64;
		pictureBoxCharacterSetSelector.TabStop = false;
		pictureBoxCharacterSetSelector.MouseDown += pictureBoxCharacterSetSelector_MouseDown;
		// 
		// labelTile0
		// 
		labelTile0.BorderStyle = BorderStyle.Fixed3D;
		labelTile0.Location = new Point(0, 80);
		labelTile0.Name = "labelTile0";
		labelTile0.Size = new Size(80, 18);
		labelTile0.TabIndex = 65;
		labelTile0.Text = "Tile: 1";
		labelTile0.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile1
		// 
		labelTile1.BorderStyle = BorderStyle.Fixed3D;
		labelTile1.Location = new Point(80, 80);
		labelTile1.Name = "labelTile1";
		labelTile1.Size = new Size(80, 18);
		labelTile1.TabIndex = 66;
		labelTile1.Text = "Tile: 2";
		labelTile1.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile2
		// 
		labelTile2.BorderStyle = BorderStyle.Fixed3D;
		labelTile2.Location = new Point(160, 80);
		labelTile2.Name = "labelTile2";
		labelTile2.Size = new Size(80, 18);
		labelTile2.TabIndex = 67;
		labelTile2.Text = "Tile: 3";
		labelTile2.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile3
		// 
		labelTile3.BorderStyle = BorderStyle.Fixed3D;
		labelTile3.Location = new Point(240, 80);
		labelTile3.Name = "labelTile3";
		labelTile3.Size = new Size(80, 18);
		labelTile3.TabIndex = 68;
		labelTile3.Text = "Tile: 4";
		labelTile3.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile4
		// 
		labelTile4.BorderStyle = BorderStyle.Fixed3D;
		labelTile4.Location = new Point(320, 80);
		labelTile4.Name = "labelTile4";
		labelTile4.Size = new Size(80, 18);
		labelTile4.TabIndex = 69;
		labelTile4.Text = "Tile: 5";
		labelTile4.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile5
		// 
		labelTile5.BorderStyle = BorderStyle.Fixed3D;
		labelTile5.Location = new Point(400, 80);
		labelTile5.Name = "labelTile5";
		labelTile5.Size = new Size(80, 18);
		labelTile5.TabIndex = 70;
		labelTile5.Text = "Tile: 6";
		labelTile5.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile6
		// 
		labelTile6.BorderStyle = BorderStyle.Fixed3D;
		labelTile6.Location = new Point(480, 80);
		labelTile6.Name = "labelTile6";
		labelTile6.Size = new Size(80, 18);
		labelTile6.TabIndex = 71;
		labelTile6.Text = "Tile: 7";
		labelTile6.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile7
		// 
		labelTile7.BorderStyle = BorderStyle.Fixed3D;
		labelTile7.Location = new Point(560, 80);
		labelTile7.Name = "labelTile7";
		labelTile7.Size = new Size(80, 18);
		labelTile7.TabIndex = 72;
		labelTile7.Text = "Tile: 8";
		labelTile7.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile8
		// 
		labelTile8.BorderStyle = BorderStyle.Fixed3D;
		labelTile8.Location = new Point(640, 80);
		labelTile8.Name = "labelTile8";
		labelTile8.Size = new Size(80, 18);
		labelTile8.TabIndex = 73;
		labelTile8.Text = "Tile: 9";
		labelTile8.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// labelTile9
		// 
		labelTile9.BorderStyle = BorderStyle.Fixed3D;
		labelTile9.Location = new Point(720, 80);
		labelTile9.Name = "labelTile9";
		labelTile9.Size = new Size(80, 18);
		labelTile9.TabIndex = 74;
		labelTile9.Text = "Tile: 10";
		labelTile9.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// pictureBoxCurrenctChar
		// 
		pictureBoxCurrenctChar.BorderStyle = BorderStyle.Fixed3D;
		pictureBoxCurrenctChar.Location = new Point(181, 126);
		pictureBoxCurrenctChar.Name = "pictureBoxCurrenctChar";
		pictureBoxCurrenctChar.Size = new Size(20, 20);
		pictureBoxCurrenctChar.TabIndex = 75;
		pictureBoxCurrenctChar.TabStop = false;
		// 
		// label3
		// 
		label3.BorderStyle = BorderStyle.FixedSingle;
		label3.Location = new Point(0, 259);
		label3.Name = "label3";
		label3.Size = new Size(800, 2);
		label3.TabIndex = 77;
		// 
		// label5
		// 
		label5.AutoSize = true;
		label5.Location = new Point(97, 134);
		label5.Name = "label5";
		label5.Size = new Size(31, 13);
		label5.TabIndex = 78;
		label5.Text = "Font";
		// 
		// label6
		// 
		label6.AutoSize = true;
		label6.Location = new Point(148, 134);
		label6.Name = "label6";
		label6.Size = new Size(24, 13);
		label6.TabIndex = 79;
		label6.Text = "Tile";
		// 
		// buttonLoadCurrentTile
		// 
		buttonLoadCurrentTile.AutoSize = true;
		buttonLoadCurrentTile.Location = new Point(310, 174);
		buttonLoadCurrentTile.Margin = new Padding(0);
		buttonLoadCurrentTile.Name = "buttonLoadCurrentTile";
		buttonLoadCurrentTile.Size = new Size(74, 23);
		buttonLoadCurrentTile.TabIndex = 80;
		buttonLoadCurrentTile.Text = "Load Tile";
		buttonLoadCurrentTile.UseVisualStyleBackColor = true;
		buttonLoadCurrentTile.Click += buttonLoadCurrentTile_Click;
		// 
		// label7
		// 
		label7.BorderStyle = BorderStyle.FixedSingle;
		label7.Location = new Point(387, 125);
		label7.Name = "label7";
		label7.Size = new Size(2, 130);
		label7.TabIndex = 81;
		// 
		// buttonSaveCurrentTile
		// 
		buttonSaveCurrentTile.AutoSize = true;
		buttonSaveCurrentTile.Location = new Point(310, 197);
		buttonSaveCurrentTile.Margin = new Padding(0);
		buttonSaveCurrentTile.Name = "buttonSaveCurrentTile";
		buttonSaveCurrentTile.Size = new Size(74, 23);
		buttonSaveCurrentTile.TabIndex = 82;
		buttonSaveCurrentTile.Text = "Save Tile";
		buttonSaveCurrentTile.UseVisualStyleBackColor = true;
		buttonSaveCurrentTile.Click += buttonSaveCurrentTile_Click;
		// 
		// buttonLoadTileSet
		// 
		buttonLoadTileSet.AutoSize = true;
		buttonLoadTileSet.Location = new Point(666, 285);
		buttonLoadTileSet.Margin = new Padding(0);
		buttonLoadTileSet.Name = "buttonLoadTileSet";
		buttonLoadTileSet.Size = new Size(108, 23);
		buttonLoadTileSet.TabIndex = 83;
		buttonLoadTileSet.Text = "Load Tile Set";
		buttonLoadTileSet.UseVisualStyleBackColor = true;
		buttonLoadTileSet.Click += buttonLoadTileSet_Click;
		// 
		// buttonSaveTileSet
		// 
		buttonSaveTileSet.AutoSize = true;
		buttonSaveTileSet.Location = new Point(666, 313);
		buttonSaveTileSet.Margin = new Padding(0);
		buttonSaveTileSet.Name = "buttonSaveTileSet";
		buttonSaveTileSet.Size = new Size(108, 23);
		buttonSaveTileSet.TabIndex = 84;
		buttonSaveTileSet.Text = "Save Tile Set";
		buttonSaveTileSet.UseVisualStyleBackColor = true;
		buttonSaveTileSet.Click += buttonSaveTileSet_Click;
		// 
		// buttonNewTileSet
		// 
		buttonNewTileSet.AutoSize = true;
		buttonNewTileSet.Location = new Point(666, 342);
		buttonNewTileSet.Margin = new Padding(0);
		buttonNewTileSet.Name = "buttonNewTileSet";
		buttonNewTileSet.Size = new Size(108, 23);
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
		buttonRedraw.Location = new Point(515, 337);
		buttonRedraw.Name = "buttonRedraw";
		buttonRedraw.Size = new Size(79, 48);
		buttonRedraw.TabIndex = 87;
		buttonRedraw.Text = "Redraw / Update All";
		buttonRedraw.UseVisualStyleBackColor = true;
		buttonRedraw.Click += buttonRedraw_Click;
		// 
		// buttonUse
		// 
		buttonUse.BackColor = Color.Gold;
		buttonUse.Location = new Point(310, 128);
		buttonUse.Name = "buttonUse";
		buttonUse.Size = new Size(74, 46);
		buttonUse.TabIndex = 88;
		buttonUse.Text = "Draw with this Tile";
		buttonUse.UseVisualStyleBackColor = false;
		buttonUse.Click += buttonUse_Click;
		// 
		// buttonRotateLeft
		// 
		buttonRotateLeft.Location = new Point(6, 128);
		buttonRotateLeft.Name = "buttonRotateLeft";
		buttonRotateLeft.Size = new Size(82, 23);
		buttonRotateLeft.TabIndex = 91;
		buttonRotateLeft.Text = "Rotate Left";
		buttonRotateLeft.UseVisualStyleBackColor = true;
		buttonRotateLeft.Click += buttonRotateLeft_Click;
		// 
		// buttonRotateRight
		// 
		buttonRotateRight.Location = new Point(217, 128);
		buttonRotateRight.Name = "buttonRotateRight";
		buttonRotateRight.Size = new Size(82, 23);
		buttonRotateRight.TabIndex = 92;
		buttonRotateRight.Text = "Rotate Right";
		buttonRotateRight.UseVisualStyleBackColor = true;
		buttonRotateRight.Click += buttonRotateRight_Click;
		// 
		// buttonMirrorH
		// 
		buttonMirrorH.Location = new Point(6, 151);
		buttonMirrorH.Name = "buttonMirrorH";
		buttonMirrorH.Size = new Size(82, 23);
		buttonMirrorH.TabIndex = 93;
		buttonMirrorH.Text = "Mirror H";
		buttonMirrorH.UseVisualStyleBackColor = true;
		buttonMirrorH.Click += buttonMirrorH_Click;
		// 
		// buttonMirrorV
		// 
		buttonMirrorV.Location = new Point(217, 151);
		buttonMirrorV.Name = "buttonMirrorV";
		buttonMirrorV.Size = new Size(82, 23);
		buttonMirrorV.TabIndex = 94;
		buttonMirrorV.Text = "Mirror V";
		buttonMirrorV.UseVisualStyleBackColor = true;
		buttonMirrorV.Click += buttonMirrorV_Click;
		// 
		// buttonShiftLeft
		// 
		buttonShiftLeft.Location = new Point(6, 174);
		buttonShiftLeft.Name = "buttonShiftLeft";
		buttonShiftLeft.Size = new Size(82, 23);
		buttonShiftLeft.TabIndex = 95;
		buttonShiftLeft.Text = "Shift Left";
		buttonShiftLeft.UseVisualStyleBackColor = true;
		buttonShiftLeft.Click += buttonShiftLeft_Click;
		// 
		// ShiftRight
		// 
		ShiftRight.Location = new Point(217, 174);
		ShiftRight.Name = "ShiftRight";
		ShiftRight.Size = new Size(82, 23);
		ShiftRight.TabIndex = 96;
		ShiftRight.Text = "Shift Right";
		ShiftRight.UseVisualStyleBackColor = true;
		ShiftRight.Click += ShiftRight_Click;
		// 
		// buttonShiftDown
		// 
		buttonShiftDown.Location = new Point(6, 197);
		buttonShiftDown.Name = "buttonShiftDown";
		buttonShiftDown.Size = new Size(82, 23);
		buttonShiftDown.TabIndex = 97;
		buttonShiftDown.Text = "Shift Down";
		buttonShiftDown.UseVisualStyleBackColor = true;
		buttonShiftDown.Click += buttonShiftDown_Click;
		// 
		// buttonShiftUp
		// 
		buttonShiftUp.Location = new Point(217, 197);
		buttonShiftUp.Name = "buttonShiftUp";
		buttonShiftUp.Size = new Size(82, 23);
		buttonShiftUp.TabIndex = 98;
		buttonShiftUp.Text = "Shift Up";
		buttonShiftUp.UseVisualStyleBackColor = true;
		buttonShiftUp.Click += buttonShiftUp_Click;
		// 
		// label2
		// 
		label2.BorderStyle = BorderStyle.FixedSingle;
		label2.Location = new Point(638, 266);
		label2.Name = "label2";
		label2.Size = new Size(2, 130);
		label2.TabIndex = 99;
		// 
		// TileSetEditorWindow
		// 
		AutoScaleDimensions = new SizeF(6F, 13F);
		AutoScaleMode = AutoScaleMode.Font;
		BackColor = SystemColors.ControlLight;
		ClientSize = new Size(800, 395);
		Controls.Add(label2);
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
		Controls.Add(label6);
		Controls.Add(label5);
		Controls.Add(label3);
		Controls.Add(pictureBoxCurrenctChar);
		Controls.Add(labelTile9);
		Controls.Add(labelTile8);
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
	private Label labelTile8;
	private Label labelTile9;
	private PictureBox pictureBoxCurrenctChar;
	private Label label3;
	private Label label5;
	private Label label6;
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
	private Label label2;
}