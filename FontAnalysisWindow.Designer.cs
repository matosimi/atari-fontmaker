﻿namespace FontMaker
{
	partial class FontAnalysisWindow
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
			pictureBoxFonts = new PictureBox();
			checkBoxGFX = new CheckBox();
			pictureBoxCursor = new PictureBox();
			groupBox1 = new GroupBox();
			radioButtonYellow = new RadioButton();
			radioButtonBlack = new RadioButton();
			trackBarAlpha = new TrackBar();
			radioButtonBlue = new RadioButton();
			radioButtonGreen = new RadioButton();
			radioButtonRed = new RadioButton();
			panel1 = new Panel();
			labelClickForUsageInfo = new Label();
			labelUsedInfo = new Label();
			labelCursorInfo = new Label();
			pictureBoxInfoCursor = new PictureBox();
			textBoxUsageInfo = new TextBox();
			chkMarkDuplicates = new CheckBox();
			textBoxDuplicates = new TextBox();
			buttonClose = new Button();
			groupBoxDuplicates = new GroupBox();
			trackBarDuplicates = new TrackBar();
			radioButtonDupBlue = new RadioButton();
			radioButtonDupGreen = new RadioButton();
			radioButtonDupRed = new RadioButton();
			((System.ComponentModel.ISupportInitialize)pictureBoxFonts).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxCursor).BeginInit();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)trackBarAlpha).BeginInit();
			panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBoxInfoCursor).BeginInit();
			groupBoxDuplicates.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)trackBarDuplicates).BeginInit();
			SuspendLayout();
			// 
			// pictureBoxFonts
			// 
			pictureBoxFonts.Location = new Point(4, 4);
			pictureBoxFonts.Name = "pictureBoxFonts";
			pictureBoxFonts.Size = new Size(512, 512);
			pictureBoxFonts.TabIndex = 19;
			pictureBoxFonts.TabStop = false;
			pictureBoxFonts.MouseDown += pictureBoxFonts_MouseDown;
			pictureBoxFonts.MouseMove += pictureBoxFonts_MouseMove;
			// 
			// checkBoxGFX
			// 
			checkBoxGFX.AutoSize = true;
			checkBoxGFX.Location = new Point(524, 4);
			checkBoxGFX.Name = "checkBoxGFX";
			checkBoxGFX.Size = new Size(47, 17);
			checkBoxGFX.TabIndex = 22;
			checkBoxGFX.Text = "GFX";
			checkBoxGFX.UseVisualStyleBackColor = true;
			checkBoxGFX.CheckedChanged += checkBoxGFX_CheckedChanged;
			// 
			// pictureBoxCursor
			// 
			pictureBoxCursor.BackColor = Color.Transparent;
			pictureBoxCursor.BorderStyle = BorderStyle.FixedSingle;
			pictureBoxCursor.Location = new Point(33, 27);
			pictureBoxCursor.Margin = new Padding(0);
			pictureBoxCursor.Name = "pictureBoxCursor";
			pictureBoxCursor.Size = new Size(20, 20);
			pictureBoxCursor.TabIndex = 23;
			pictureBoxCursor.TabStop = false;
			pictureBoxCursor.MouseDown += pictureBoxCursor_MouseDown;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(radioButtonYellow);
			groupBox1.Controls.Add(radioButtonBlack);
			groupBox1.Controls.Add(trackBarAlpha);
			groupBox1.Controls.Add(radioButtonBlue);
			groupBox1.Controls.Add(radioButtonGreen);
			groupBox1.Controls.Add(radioButtonRed);
			groupBox1.Location = new Point(522, 27);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(284, 76);
			groupBox1.TabIndex = 30;
			groupBox1.TabStop = false;
			groupBox1.Text = "Overlay color";
			// 
			// radioButtonYellow
			// 
			radioButtonYellow.AutoSize = true;
			radioButtonYellow.Location = new Point(215, 16);
			radioButtonYellow.Name = "radioButtonYellow";
			radioButtonYellow.Size = new Size(56, 17);
			radioButtonYellow.TabIndex = 6;
			radioButtonYellow.Text = "Yellow";
			radioButtonYellow.UseVisualStyleBackColor = true;
			radioButtonYellow.CheckedChanged += radioButtonYellow_CheckedChanged;
			// 
			// radioButtonBlack
			// 
			radioButtonBlack.AutoSize = true;
			radioButtonBlack.Location = new Point(162, 16);
			radioButtonBlack.Name = "radioButtonBlack";
			radioButtonBlack.Size = new Size(52, 17);
			radioButtonBlack.TabIndex = 5;
			radioButtonBlack.Text = "Black";
			radioButtonBlack.UseVisualStyleBackColor = true;
			radioButtonBlack.CheckedChanged += radioButtonBlack_CheckedChanged;
			// 
			// trackBarAlpha
			// 
			trackBarAlpha.AutoSize = false;
			trackBarAlpha.Location = new Point(9, 39);
			trackBarAlpha.Maximum = 192;
			trackBarAlpha.Minimum = 64;
			trackBarAlpha.Name = "trackBarAlpha";
			trackBarAlpha.Size = new Size(269, 31);
			trackBarAlpha.TabIndex = 4;
			trackBarAlpha.TickFrequency = 10;
			trackBarAlpha.TickStyle = TickStyle.TopLeft;
			trackBarAlpha.Value = 128;
			trackBarAlpha.Scroll += trackBarAlpha_Scroll;
			// 
			// radioButtonBlue
			// 
			radioButtonBlue.AutoSize = true;
			radioButtonBlue.Location = new Point(113, 16);
			radioButtonBlue.Name = "radioButtonBlue";
			radioButtonBlue.Size = new Size(46, 17);
			radioButtonBlue.TabIndex = 2;
			radioButtonBlue.Text = "Blue";
			radioButtonBlue.UseVisualStyleBackColor = true;
			radioButtonBlue.CheckedChanged += radioButtonBlue_CheckedChanged;
			// 
			// radioButtonGreen
			// 
			radioButtonGreen.AutoSize = true;
			radioButtonGreen.Location = new Point(55, 16);
			radioButtonGreen.Name = "radioButtonGreen";
			radioButtonGreen.Size = new Size(54, 17);
			radioButtonGreen.TabIndex = 1;
			radioButtonGreen.Text = "Green";
			radioButtonGreen.UseVisualStyleBackColor = true;
			radioButtonGreen.CheckedChanged += radioButtonGreen_CheckedChanged;
			// 
			// radioButtonRed
			// 
			radioButtonRed.AutoSize = true;
			radioButtonRed.Checked = true;
			radioButtonRed.Location = new Point(7, 16);
			radioButtonRed.Name = "radioButtonRed";
			radioButtonRed.Size = new Size(45, 17);
			radioButtonRed.TabIndex = 0;
			radioButtonRed.TabStop = true;
			radioButtonRed.Text = "Red";
			radioButtonRed.UseVisualStyleBackColor = true;
			radioButtonRed.CheckedChanged += radioButtonRed_CheckedChanged;
			// 
			// panel1
			// 
			panel1.BorderStyle = BorderStyle.FixedSingle;
			panel1.Controls.Add(labelClickForUsageInfo);
			panel1.Controls.Add(labelUsedInfo);
			panel1.Controls.Add(labelCursorInfo);
			panel1.Location = new Point(524, 109);
			panel1.Name = "panel1";
			panel1.Size = new Size(284, 60);
			panel1.TabIndex = 31;
			// 
			// labelClickForUsageInfo
			// 
			labelClickForUsageInfo.Location = new Point(7, 37);
			labelClickForUsageInfo.Name = "labelClickForUsageInfo";
			labelClickForUsageInfo.Size = new Size(248, 15);
			labelClickForUsageInfo.TabIndex = 28;
			labelClickForUsageInfo.Text = "Click for usage info";
			labelClickForUsageInfo.Visible = false;
			// 
			// labelUsedInfo
			// 
			labelUsedInfo.Location = new Point(7, 21);
			labelUsedInfo.Name = "labelUsedInfo";
			labelUsedInfo.Size = new Size(248, 15);
			labelUsedInfo.TabIndex = 27;
			labelUsedInfo.Text = ".";
			// 
			// labelCursorInfo
			// 
			labelCursorInfo.Location = new Point(7, 5);
			labelCursorInfo.Name = "labelCursorInfo";
			labelCursorInfo.Size = new Size(248, 15);
			labelCursorInfo.TabIndex = 26;
			labelCursorInfo.Text = "Font ...";
			// 
			// pictureBoxInfoCursor
			// 
			pictureBoxInfoCursor.BackColor = Color.Transparent;
			pictureBoxInfoCursor.BorderStyle = BorderStyle.FixedSingle;
			pictureBoxInfoCursor.Location = new Point(33, 66);
			pictureBoxInfoCursor.Margin = new Padding(0);
			pictureBoxInfoCursor.Name = "pictureBoxInfoCursor";
			pictureBoxInfoCursor.Size = new Size(20, 20);
			pictureBoxInfoCursor.TabIndex = 32;
			pictureBoxInfoCursor.TabStop = false;
			pictureBoxInfoCursor.MouseDown += pictureBoxInfoCursor_MouseDown;
			// 
			// textBoxUsageInfo
			// 
			textBoxUsageInfo.BackColor = SystemColors.Control;
			textBoxUsageInfo.Cursor = Cursors.Hand;
			textBoxUsageInfo.Location = new Point(525, 173);
			textBoxUsageInfo.Multiline = true;
			textBoxUsageInfo.Name = "textBoxUsageInfo";
			textBoxUsageInfo.ReadOnly = true;
			textBoxUsageInfo.ScrollBars = ScrollBars.Both;
			textBoxUsageInfo.ShortcutsEnabled = false;
			textBoxUsageInfo.Size = new Size(281, 233);
			textBoxUsageInfo.TabIndex = 33;
			textBoxUsageInfo.Visible = false;
			textBoxUsageInfo.WordWrap = false;
			textBoxUsageInfo.MouseDown += textBoxUsageInfo_MouseDown;
			// 
			// chkMarkDuplicates
			// 
			chkMarkDuplicates.AutoSize = true;
			chkMarkDuplicates.Location = new Point(524, 417);
			chkMarkDuplicates.Name = "chkMarkDuplicates";
			chkMarkDuplicates.Size = new Size(103, 17);
			chkMarkDuplicates.TabIndex = 34;
			chkMarkDuplicates.Text = "Mark Duplicates";
			chkMarkDuplicates.UseVisualStyleBackColor = true;
			chkMarkDuplicates.CheckedChanged += chkMarkDuplicates_CheckedChanged;
			// 
			// textBoxDuplicates
			// 
			textBoxDuplicates.BackColor = SystemColors.Control;
			textBoxDuplicates.Cursor = Cursors.Hand;
			textBoxDuplicates.Location = new Point(689, 412);
			textBoxDuplicates.Multiline = true;
			textBoxDuplicates.Name = "textBoxDuplicates";
			textBoxDuplicates.ReadOnly = true;
			textBoxDuplicates.ScrollBars = ScrollBars.Both;
			textBoxDuplicates.ShortcutsEnabled = false;
			textBoxDuplicates.Size = new Size(117, 104);
			textBoxDuplicates.TabIndex = 35;
			textBoxDuplicates.Visible = false;
			textBoxDuplicates.WordWrap = false;
			textBoxDuplicates.MouseUp += textBoxDuplicates_MouseUp;
			// 
			// buttonClose
			// 
			buttonClose.Location = new Point(737, 4);
			buttonClose.Name = "buttonClose";
			buttonClose.Size = new Size(75, 23);
			buttonClose.TabIndex = 21;
			buttonClose.Text = "Close";
			buttonClose.UseVisualStyleBackColor = true;
			// 
			// groupBoxDuplicates
			// 
			groupBoxDuplicates.Controls.Add(trackBarDuplicates);
			groupBoxDuplicates.Controls.Add(radioButtonDupBlue);
			groupBoxDuplicates.Controls.Add(radioButtonDupGreen);
			groupBoxDuplicates.Controls.Add(radioButtonDupRed);
			groupBoxDuplicates.Location = new Point(524, 440);
			groupBoxDuplicates.Name = "groupBoxDuplicates";
			groupBoxDuplicates.Size = new Size(164, 76);
			groupBoxDuplicates.TabIndex = 31;
			groupBoxDuplicates.TabStop = false;
			groupBoxDuplicates.Text = "Duplicate color";
			groupBoxDuplicates.Visible = false;
			// 
			// trackBarDuplicates
			// 
			trackBarDuplicates.AutoSize = false;
			trackBarDuplicates.Location = new Point(3, 39);
			trackBarDuplicates.Maximum = 192;
			trackBarDuplicates.Minimum = 64;
			trackBarDuplicates.Name = "trackBarDuplicates";
			trackBarDuplicates.Size = new Size(150, 31);
			trackBarDuplicates.TabIndex = 4;
			trackBarDuplicates.TickFrequency = 10;
			trackBarDuplicates.TickStyle = TickStyle.TopLeft;
			trackBarDuplicates.Value = 128;
			trackBarDuplicates.Scroll += trackBarDuplicates_Scroll;
			// 
			// radioButtonDupBlue
			// 
			radioButtonDupBlue.AutoSize = true;
			radioButtonDupBlue.Location = new Point(113, 16);
			radioButtonDupBlue.Name = "radioButtonDupBlue";
			radioButtonDupBlue.Size = new Size(46, 17);
			radioButtonDupBlue.TabIndex = 2;
			radioButtonDupBlue.Text = "Blue";
			radioButtonDupBlue.UseVisualStyleBackColor = true;
			radioButtonDupBlue.CheckedChanged += radioButtonDupBlue_CheckedChanged;
			// 
			// radioButtonDupGreen
			// 
			radioButtonDupGreen.AutoSize = true;
			radioButtonDupGreen.Location = new Point(55, 16);
			radioButtonDupGreen.Name = "radioButtonDupGreen";
			radioButtonDupGreen.Size = new Size(54, 17);
			radioButtonDupGreen.TabIndex = 1;
			radioButtonDupGreen.Text = "Green";
			radioButtonDupGreen.UseVisualStyleBackColor = true;
			radioButtonDupGreen.CheckedChanged += radioButtonDupGreen_CheckedChanged;
			// 
			// radioButtonDupRed
			// 
			radioButtonDupRed.AutoSize = true;
			radioButtonDupRed.Checked = true;
			radioButtonDupRed.Location = new Point(7, 16);
			radioButtonDupRed.Name = "radioButtonDupRed";
			radioButtonDupRed.Size = new Size(45, 17);
			radioButtonDupRed.TabIndex = 0;
			radioButtonDupRed.TabStop = true;
			radioButtonDupRed.Text = "Red";
			radioButtonDupRed.UseVisualStyleBackColor = true;
			radioButtonDupRed.CheckedChanged += radioButtonDupRed_CheckedChanged;
			// 
			// FontAnalysisWindow
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = buttonClose;
			ClientSize = new Size(818, 521);
			Controls.Add(groupBoxDuplicates);
			Controls.Add(textBoxDuplicates);
			Controls.Add(chkMarkDuplicates);
			Controls.Add(textBoxUsageInfo);
			Controls.Add(pictureBoxInfoCursor);
			Controls.Add(panel1);
			Controls.Add(groupBox1);
			Controls.Add(pictureBoxCursor);
			Controls.Add(checkBoxGFX);
			Controls.Add(buttonClose);
			Controls.Add(pictureBoxFonts);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "FontAnalysisWindow";
			Text = "Analyse characters in font usage";
			FormClosing += FontAnalysisWindow_FormClosing;
			Load += FontAnalysisWindow_Load;
			MouseWheel += Form_MouseWheel;
			((System.ComponentModel.ISupportInitialize)pictureBoxFonts).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxCursor).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)trackBarAlpha).EndInit();
			panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBoxInfoCursor).EndInit();
			groupBoxDuplicates.ResumeLayout(false);
			groupBoxDuplicates.PerformLayout();
			((System.ComponentModel.ISupportInitialize)trackBarDuplicates).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private PictureBox pictureBoxFonts;
		private CheckBox checkBoxGFX;
		private PictureBox pictureBoxCursor;
		private GroupBox groupBox1;
		private RadioButton radioButtonBlue;
		private RadioButton radioButtonGreen;
		private RadioButton radioButtonRed;
		private TrackBar trackBarAlpha;
		private RadioButton radioButtonBlack;
		private RadioButton radioButtonYellow;
		private Panel panel1;
		private Label labelUsedInfo;
		private Label labelCursorInfo;
		private Label labelClickForUsageInfo;
		private PictureBox pictureBoxInfoCursor;
		private TextBox textBoxUsageInfo;
		private CheckBox chkMarkDuplicates;
		private TextBox textBoxDuplicates;
		private Button buttonClose;
		private GroupBox groupBoxDuplicates;
		private TrackBar trackBarDuplicates;
		private RadioButton radioButtonDupBlue;
		private RadioButton radioButtonDupGreen;
		private RadioButton radioButtonDupRed;
	}
}