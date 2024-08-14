namespace FontMaker
{
	partial class ViewActionsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewActionsWindow));
            btnClose = new Button();
            comboBoxPages = new ComboBox();
            label1 = new Label();
            pictureBoxX = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            pictureBoxY = new PictureBox();
            buttonReplaceXwithYInView = new Button();
            labelReplaceX = new Label();
            labelReplaceY = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            buttonViewShiftLeft = new Button();
            imageListViewShift = new ImageList(components);
            toolTips = new ToolTip(components);
            buttonViewShiftRight = new Button();
            buttonViewShiftUp = new Button();
            buttonViewShiftDown = new Button();
            buttonAreaShiftDown = new Button();
            buttonAreaShiftUp = new Button();
            buttonAreaShiftRight = new Button();
            buttonAreaShiftLeft = new Button();
            label7 = new Label();
            label9 = new Label();
            label10 = new Label();
            labelAreaInfo = new Label();
            checkFont1 = new CheckBox();
            checkFont2 = new CheckBox();
            checkFont3 = new CheckBox();
            checkFont4 = new CheckBox();
            label8 = new Label();
            buttonReplaceXwithYInArea = new Button();
            label11 = new Label();
            label12 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxY).BeginInit();
            SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Location = new Point(5, 462);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(215, 23);
            btnClose.TabIndex = 0;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // comboBoxPages
            // 
            comboBoxPages.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxPages.FormattingEnabled = true;
            comboBoxPages.Location = new Point(87, 8);
            comboBoxPages.MaxDropDownItems = 10;
            comboBoxPages.Name = "comboBoxPages";
            comboBoxPages.Size = new Size(131, 21);
            comboBoxPages.TabIndex = 17;
            comboBoxPages.SelectedIndexChanged += ViewActionsWindow_Pages_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 11);
            label1.Name = "label1";
            label1.Size = new Size(77, 13);
            label1.TabIndex = 18;
            label1.Text = "Action on view";
            // 
            // pictureBoxX
            // 
            pictureBoxX.BackColor = Color.Transparent;
            pictureBoxX.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxX.Location = new Point(35, 59);
            pictureBoxX.Margin = new Padding(0);
            pictureBoxX.Name = "pictureBoxX";
            pictureBoxX.Size = new Size(18, 18);
            pictureBoxX.TabIndex = 24;
            pictureBoxX.TabStop = false;
            pictureBoxX.Click += pictureBoxX_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(1, 42);
            label2.Name = "label2";
            label2.Size = new Size(175, 13);
            label2.TabIndex = 25;
            label2.Text = "Replace all characters on a page:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(94, 61);
            label3.Name = "label3";
            label3.Size = new Size(36, 13);
            label3.TabIndex = 26;
            label3.Text = "WITH";
            // 
            // pictureBoxY
            // 
            pictureBoxY.BackColor = Color.Transparent;
            pictureBoxY.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxY.Location = new Point(142, 59);
            pictureBoxY.Margin = new Padding(0);
            pictureBoxY.Name = "pictureBoxY";
            pictureBoxY.Size = new Size(18, 18);
            pictureBoxY.TabIndex = 27;
            pictureBoxY.TabStop = false;
            pictureBoxY.Click += pictureBoxY_Click;
            // 
            // buttonReplaceXwithYInView
            // 
            buttonReplaceXwithYInView.Enabled = false;
            buttonReplaceXwithYInView.Location = new Point(5, 120);
            buttonReplaceXwithYInView.Name = "buttonReplaceXwithYInView";
            buttonReplaceXwithYInView.Size = new Size(108, 29);
            buttonReplaceXwithYInView.TabIndex = 28;
            buttonReplaceXwithYInView.Text = "Replace in View";
            buttonReplaceXwithYInView.UseVisualStyleBackColor = true;
            buttonReplaceXwithYInView.Click += ButtonReplaceXwithYInViewClick;
            // 
            // labelReplaceX
            // 
            labelReplaceX.Location = new Point(55, 61);
            labelReplaceX.Name = "labelReplaceX";
            labelReplaceX.Size = new Size(34, 13);
            labelReplaceX.TabIndex = 29;
            labelReplaceX.Text = "#255";
            // 
            // labelReplaceY
            // 
            labelReplaceY.Location = new Point(162, 61);
            labelReplaceY.Name = "labelReplaceY";
            labelReplaceY.Size = new Size(34, 13);
            labelReplaceY.TabIndex = 30;
            labelReplaceY.Text = "#255";
            // 
            // label4
            // 
            label4.BorderStyle = BorderStyle.FixedSingle;
            label4.Location = new Point(1, 154);
            label4.Name = "label4";
            label4.Size = new Size(222, 2);
            label4.TabIndex = 31;
            // 
            // label5
            // 
            label5.BorderStyle = BorderStyle.FixedSingle;
            label5.Location = new Point(1, 36);
            label5.Name = "label5";
            label5.Size = new Size(222, 2);
            label5.TabIndex = 32;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            label6.Location = new Point(3, 191);
            label6.Margin = new Padding(0);
            label6.Name = "label6";
            label6.Size = new Size(91, 13);
            label6.TabIndex = 33;
            label6.Text = "Shift View/Area:";
            // 
            // buttonViewShiftLeft
            // 
            buttonViewShiftLeft.ImageIndex = 0;
            buttonViewShiftLeft.ImageList = imageListViewShift;
            buttonViewShiftLeft.Location = new Point(1, 233);
            buttonViewShiftLeft.Name = "buttonViewShiftLeft";
            buttonViewShiftLeft.Size = new Size(24, 24);
            buttonViewShiftLeft.TabIndex = 34;
            buttonViewShiftLeft.TextImageRelation = TextImageRelation.ImageBeforeText;
            toolTips.SetToolTip(buttonViewShiftLeft, "Shift View Left");
            buttonViewShiftLeft.UseVisualStyleBackColor = true;
            buttonViewShiftLeft.Click += buttonViewShiftLeft_Click;
            // 
            // imageListViewShift
            // 
            imageListViewShift.ColorDepth = ColorDepth.Depth8Bit;
            imageListViewShift.ImageStream = (ImageListStreamer)resources.GetObject("imageListViewShift.ImageStream");
            imageListViewShift.TransparentColor = Color.Transparent;
            imageListViewShift.Images.SetKeyName(0, "CopyAreaShiftLeft.bmp");
            imageListViewShift.Images.SetKeyName(1, "CopyAreaShiftRight.bmp");
            imageListViewShift.Images.SetKeyName(2, "CopyAreaShiftUp.bmp");
            imageListViewShift.Images.SetKeyName(3, "CopyAreaShiftDown.bmp");
            // 
            // buttonViewShiftRight
            // 
            buttonViewShiftRight.ImageIndex = 1;
            buttonViewShiftRight.ImageList = imageListViewShift;
            buttonViewShiftRight.Location = new Point(47, 233);
            buttonViewShiftRight.Name = "buttonViewShiftRight";
            buttonViewShiftRight.Size = new Size(24, 24);
            buttonViewShiftRight.TabIndex = 35;
            buttonViewShiftRight.TextImageRelation = TextImageRelation.ImageBeforeText;
            toolTips.SetToolTip(buttonViewShiftRight, "Shift View Right");
            buttonViewShiftRight.UseVisualStyleBackColor = true;
            buttonViewShiftRight.Click += buttonViewShiftRight_Click;
            // 
            // buttonViewShiftUp
            // 
            buttonViewShiftUp.ImageIndex = 2;
            buttonViewShiftUp.ImageList = imageListViewShift;
            buttonViewShiftUp.Location = new Point(25, 210);
            buttonViewShiftUp.Name = "buttonViewShiftUp";
            buttonViewShiftUp.Size = new Size(24, 24);
            buttonViewShiftUp.TabIndex = 36;
            buttonViewShiftUp.TextImageRelation = TextImageRelation.ImageBeforeText;
            toolTips.SetToolTip(buttonViewShiftUp, "Shift View Up");
            buttonViewShiftUp.UseVisualStyleBackColor = true;
            buttonViewShiftUp.Click += buttonViewShiftUp_Click;
            // 
            // buttonViewShiftDown
            // 
            buttonViewShiftDown.ImageIndex = 3;
            buttonViewShiftDown.ImageList = imageListViewShift;
            buttonViewShiftDown.Location = new Point(24, 233);
            buttonViewShiftDown.Name = "buttonViewShiftDown";
            buttonViewShiftDown.Size = new Size(24, 24);
            buttonViewShiftDown.TabIndex = 37;
            buttonViewShiftDown.TextImageRelation = TextImageRelation.ImageBeforeText;
            toolTips.SetToolTip(buttonViewShiftDown, "Shift View Down");
            buttonViewShiftDown.UseVisualStyleBackColor = true;
            buttonViewShiftDown.Click += buttonViewShiftDown_Click;
            // 
            // buttonAreaShiftDown
            // 
            buttonAreaShiftDown.ImageIndex = 3;
            buttonAreaShiftDown.ImageList = imageListViewShift;
            buttonAreaShiftDown.Location = new Point(173, 233);
            buttonAreaShiftDown.Name = "buttonAreaShiftDown";
            buttonAreaShiftDown.Size = new Size(24, 24);
            buttonAreaShiftDown.TabIndex = 43;
            buttonAreaShiftDown.TextImageRelation = TextImageRelation.ImageBeforeText;
            toolTips.SetToolTip(buttonAreaShiftDown, "Shift View Down");
            buttonAreaShiftDown.UseVisualStyleBackColor = true;
            buttonAreaShiftDown.Click += buttonAreaShiftDown_Click;
            // 
            // buttonAreaShiftUp
            // 
            buttonAreaShiftUp.ImageIndex = 2;
            buttonAreaShiftUp.ImageList = imageListViewShift;
            buttonAreaShiftUp.Location = new Point(174, 210);
            buttonAreaShiftUp.Name = "buttonAreaShiftUp";
            buttonAreaShiftUp.Size = new Size(24, 24);
            buttonAreaShiftUp.TabIndex = 42;
            buttonAreaShiftUp.TextImageRelation = TextImageRelation.ImageBeforeText;
            toolTips.SetToolTip(buttonAreaShiftUp, "Shift View Up");
            buttonAreaShiftUp.UseVisualStyleBackColor = true;
            buttonAreaShiftUp.Click += buttonAreaShiftUp_Click;
            // 
            // buttonAreaShiftRight
            // 
            buttonAreaShiftRight.ImageIndex = 1;
            buttonAreaShiftRight.ImageList = imageListViewShift;
            buttonAreaShiftRight.Location = new Point(196, 233);
            buttonAreaShiftRight.Name = "buttonAreaShiftRight";
            buttonAreaShiftRight.Size = new Size(24, 24);
            buttonAreaShiftRight.TabIndex = 41;
            buttonAreaShiftRight.TextImageRelation = TextImageRelation.ImageBeforeText;
            toolTips.SetToolTip(buttonAreaShiftRight, "Shift View Right");
            buttonAreaShiftRight.UseVisualStyleBackColor = true;
            buttonAreaShiftRight.Click += buttonAreaShiftRight_Click;
            // 
            // buttonAreaShiftLeft
            // 
            buttonAreaShiftLeft.ImageIndex = 0;
            buttonAreaShiftLeft.ImageList = imageListViewShift;
            buttonAreaShiftLeft.Location = new Point(150, 233);
            buttonAreaShiftLeft.Name = "buttonAreaShiftLeft";
            buttonAreaShiftLeft.Size = new Size(24, 24);
            buttonAreaShiftLeft.TabIndex = 40;
            buttonAreaShiftLeft.TextImageRelation = TextImageRelation.ImageBeforeText;
            toolTips.SetToolTip(buttonAreaShiftLeft, "Shift View Left");
            buttonAreaShiftLeft.UseVisualStyleBackColor = true;
            buttonAreaShiftLeft.Click += buttonAreaShiftLeft_Click;
            // 
            // label7
            // 
            label7.BorderStyle = BorderStyle.FixedSingle;
            label7.Location = new Point(1, 261);
            label7.Name = "label7";
            label7.Size = new Size(222, 2);
            label7.TabIndex = 38;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(52, 216);
            label9.Name = "label9";
            label9.Size = new Size(35, 13);
            label9.TabIndex = 44;
            label9.Text = "VIEW";
            label9.TextAlign = ContentAlignment.TopRight;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(135, 216);
            label10.Name = "label10";
            label10.Size = new Size(36, 13);
            label10.TabIndex = 45;
            label10.Text = "AREA";
            label10.TextAlign = ContentAlignment.TopRight;
            // 
            // labelAreaInfo
            // 
            labelAreaInfo.Location = new Point(78, 160);
            labelAreaInfo.Name = "labelAreaInfo";
            labelAreaInfo.Size = new Size(142, 19);
            labelAreaInfo.TabIndex = 46;
            labelAreaInfo.TextAlign = ContentAlignment.MiddleRight;
            // 
            // checkFont1
            // 
            checkFont1.Appearance = Appearance.Button;
            checkFont1.AutoSize = true;
            checkFont1.Checked = true;
            checkFont1.CheckState = CheckState.Checked;
            checkFont1.Location = new Point(66, 92);
            checkFont1.Name = "checkFont1";
            checkFont1.Size = new Size(23, 23);
            checkFont1.TabIndex = 47;
            checkFont1.Text = "1";
            checkFont1.TextAlign = ContentAlignment.MiddleCenter;
            checkFont1.UseVisualStyleBackColor = true;
            checkFont1.CheckedChanged += checkFont_CheckedChanged;
            // 
            // checkFont2
            // 
            checkFont2.Appearance = Appearance.Button;
            checkFont2.AutoSize = true;
            checkFont2.Checked = true;
            checkFont2.CheckState = CheckState.Checked;
            checkFont2.Location = new Point(90, 92);
            checkFont2.Name = "checkFont2";
            checkFont2.Size = new Size(23, 23);
            checkFont2.TabIndex = 48;
            checkFont2.Text = "2";
            checkFont2.TextAlign = ContentAlignment.MiddleCenter;
            checkFont2.UseVisualStyleBackColor = true;
            checkFont2.CheckedChanged += checkFont_CheckedChanged;
            // 
            // checkFont3
            // 
            checkFont3.Appearance = Appearance.Button;
            checkFont3.AutoSize = true;
            checkFont3.Checked = true;
            checkFont3.CheckState = CheckState.Checked;
            checkFont3.Location = new Point(114, 92);
            checkFont3.Name = "checkFont3";
            checkFont3.Size = new Size(23, 23);
            checkFont3.TabIndex = 49;
            checkFont3.Text = "3";
            checkFont3.TextAlign = ContentAlignment.MiddleCenter;
            checkFont3.UseVisualStyleBackColor = true;
            checkFont3.CheckedChanged += checkFont_CheckedChanged;
            // 
            // checkFont4
            // 
            checkFont4.Appearance = Appearance.Button;
            checkFont4.AutoSize = true;
            checkFont4.Checked = true;
            checkFont4.CheckState = CheckState.Checked;
            checkFont4.Location = new Point(138, 92);
            checkFont4.Name = "checkFont4";
            checkFont4.Size = new Size(23, 23);
            checkFont4.TabIndex = 50;
            checkFont4.Text = "4";
            checkFont4.TextAlign = ContentAlignment.MiddleCenter;
            checkFont4.UseVisualStyleBackColor = true;
            checkFont4.CheckedChanged += checkFont_CheckedChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(93, 78);
            label8.Name = "label8";
            label8.Size = new Size(44, 13);
            label8.TabIndex = 51;
            label8.Text = "in fonts:";
            // 
            // buttonReplaceXwithYInArea
            // 
            buttonReplaceXwithYInArea.Enabled = false;
            buttonReplaceXwithYInArea.Location = new Point(115, 120);
            buttonReplaceXwithYInArea.Name = "buttonReplaceXwithYInArea";
            buttonReplaceXwithYInArea.Size = new Size(108, 29);
            buttonReplaceXwithYInArea.TabIndex = 52;
            buttonReplaceXwithYInArea.Text = "Replace in Area";
            buttonReplaceXwithYInArea.UseVisualStyleBackColor = true;
            buttonReplaceXwithYInArea.Click += buttonReplaceXwithYInArea_Click;
            // 
            // label11
            // 
            label11.BorderStyle = BorderStyle.FixedSingle;
            label11.Location = new Point(1, 184);
            label11.Name = "label11";
            label11.Size = new Size(222, 2);
            label11.TabIndex = 53;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(3, 163);
            label12.Name = "label12";
            label12.Size = new Size(77, 13);
            label12.TabIndex = 54;
            label12.Text = "Selected Area:";
            // 
            // ViewActionsWindow
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(224, 491);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(buttonReplaceXwithYInArea);
            Controls.Add(label8);
            Controls.Add(checkFont4);
            Controls.Add(checkFont3);
            Controls.Add(checkFont2);
            Controls.Add(checkFont1);
            Controls.Add(labelAreaInfo);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(buttonAreaShiftDown);
            Controls.Add(buttonAreaShiftUp);
            Controls.Add(buttonAreaShiftRight);
            Controls.Add(buttonAreaShiftLeft);
            Controls.Add(label7);
            Controls.Add(buttonViewShiftDown);
            Controls.Add(buttonViewShiftUp);
            Controls.Add(buttonViewShiftRight);
            Controls.Add(buttonViewShiftLeft);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(labelReplaceY);
            Controls.Add(labelReplaceX);
            Controls.Add(buttonReplaceXwithYInView);
            Controls.Add(pictureBoxY);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(pictureBoxX);
            Controls.Add(label1);
            Controls.Add(comboBoxPages);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "ViewActionsWindow";
            Text = "ViewActionsWindow";
            FormClosing += ViewActionsWindow_FormClosing;
            ((System.ComponentModel.ISupportInitialize)pictureBoxX).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxY).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnClose;
		private ComboBox comboBoxPages;
		private Label label1;
		private PictureBox pictureBoxX;
		private Label label2;
		private Label label3;
		private PictureBox pictureBoxY;
		private Button buttonReplaceXwithYInView;
		private Label labelReplaceX;
		private Label labelReplaceY;
		private Label label4;
		private Label label5;
		private Label label6;
		private Button buttonViewShiftLeft;
		private ImageList imageListViewShift;
		private ToolTip toolTips;
		private Button buttonViewShiftRight;
		private Button buttonViewShiftUp;
		private Button buttonViewShiftDown;
		private Label label7;
		private Button buttonAreaShiftDown;
		private Button buttonAreaShiftUp;
		private Button buttonAreaShiftRight;
		private Button buttonAreaShiftLeft;
		private Label label9;
		private Label label10;
		private Label labelAreaInfo;
		private CheckBox checkFont1;
		private CheckBox checkFont2;
		private CheckBox checkFont3;
		private CheckBox checkFont4;
		private Label label8;
		private Button buttonReplaceXwithYInArea;
		private Label label11;
		private Label label12;
	}
}