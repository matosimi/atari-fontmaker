namespace FontMaker
{
	partial class ExportViewWindow
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
			Button_Export = new Button();
			Button_Cancel = new Button();
			ButtonCopyClipboard = new Button();
			Label1 = new Label();
			Label2 = new Label();
			ComboBoxExportType = new ComboBox();
			ComboBoxDataType = new ComboBox();
			pictureBoxAtariViewSmall = new PictureBox();
			pictureBoxViewEditorRubberBand = new PictureBox();
			MemoExport = new RichTextBox();
			label3 = new Label();
			label4 = new Label();
			numericFromX = new NumericUpDown();
			numericFromY = new NumericUpDown();
			numericHeight = new NumericUpDown();
			numericWidth = new NumericUpDown();
			label5 = new Label();
			label6 = new Label();
			labelDimensions = new Label();
			timerUpdateExportSample = new System.Windows.Forms.Timer(components);
			buttonResetSelection = new Button();
			label7 = new Label();
			saveDialog = new SaveFileDialog();
			checkBoxRememberState = new CheckBox();
			checkBoxTranspose = new CheckBox();
			((System.ComponentModel.ISupportInitialize)pictureBoxAtariViewSmall).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxViewEditorRubberBand).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericFromX).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericFromY).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericHeight).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericWidth).BeginInit();
			SuspendLayout();
			// 
			// Button_Export
			// 
			Button_Export.Location = new Point(269, 8);
			Button_Export.Name = "Button_Export";
			Button_Export.Size = new Size(81, 25);
			Button_Export.TabIndex = 11;
			Button_Export.Text = "Export ...";
			Button_Export.UseVisualStyleBackColor = true;
			Button_Export.Click += ButtonExport_Click;
			// 
			// Button_Cancel
			// 
			Button_Cancel.Location = new Point(365, 8);
			Button_Cancel.Name = "Button_Cancel";
			Button_Cancel.Size = new Size(81, 25);
			Button_Cancel.TabIndex = 12;
			Button_Cancel.Text = "Cancel";
			Button_Cancel.UseVisualStyleBackColor = true;
			Button_Cancel.Click += Button_Cancel_Click;
			// 
			// ButtonCopyClipboard
			// 
			ButtonCopyClipboard.Location = new Point(269, 41);
			ButtonCopyClipboard.Name = "ButtonCopyClipboard";
			ButtonCopyClipboard.Size = new Size(81, 25);
			ButtonCopyClipboard.TabIndex = 13;
			ButtonCopyClipboard.Text = "To Clipboard";
			ButtonCopyClipboard.UseVisualStyleBackColor = true;
			ButtonCopyClipboard.Click += ButtonCopyClipboard_Click;
			// 
			// Label1
			// 
			Label1.AutoSize = true;
			Label1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			Label1.Location = new Point(8, 11);
			Label1.Name = "Label1";
			Label1.Size = new Size(82, 13);
			Label1.TabIndex = 14;
			Label1.Text = "Export view as :";
			// 
			// Label2
			// 
			Label2.AutoSize = true;
			Label2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			Label2.Location = new Point(8, 47);
			Label2.Name = "Label2";
			Label2.Size = new Size(59, 13);
			Label2.TabIndex = 16;
			Label2.Text = "Data type :";
			// 
			// ComboBoxExportType
			// 
			ComboBoxExportType.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxExportType.FormattingEnabled = true;
			ComboBoxExportType.Items.AddRange(new object[] { "Binary Data", "Assembler", "Action! ", "Atari Basic", "FastBasic", "MADS dta" });
			ComboBoxExportType.Location = new Point(104, 8);
			ComboBoxExportType.Name = "ComboBoxExportType";
			ComboBoxExportType.Size = new Size(145, 21);
			ComboBoxExportType.TabIndex = 15;
			ComboBoxExportType.SelectedIndexChanged += ComboBoxExportType_SelectedIndexChanged;
			// 
			// ComboBoxDataType
			// 
			ComboBoxDataType.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxDataType.FormattingEnabled = true;
			ComboBoxDataType.Location = new Point(104, 44);
			ComboBoxDataType.Name = "ComboBoxDataType";
			ComboBoxDataType.Size = new Size(145, 21);
			ComboBoxDataType.TabIndex = 17;
			ComboBoxDataType.SelectedIndexChanged += ComboBoxDataTypeChange;
			// 
			// pictureBoxAtariViewSmall
			// 
			pictureBoxAtariViewSmall.Location = new Point(8, 83);
			pictureBoxAtariViewSmall.Name = "pictureBoxAtariViewSmall";
			pictureBoxAtariViewSmall.Size = new Size(320, 208);
			pictureBoxAtariViewSmall.TabIndex = 18;
			pictureBoxAtariViewSmall.TabStop = false;
			pictureBoxAtariViewSmall.MouseDown += pictureBoxAtariViewSmall_MouseDown;
			pictureBoxAtariViewSmall.MouseMove += pictureBoxAtariViewSmall_MouseMove;
			pictureBoxAtariViewSmall.MouseUp += pictureBoxAtariViewSmall_MouseUp;
			// 
			// pictureBoxViewEditorRubberBand
			// 
			pictureBoxViewEditorRubberBand.BackColor = Color.Transparent;
			pictureBoxViewEditorRubberBand.BorderStyle = BorderStyle.FixedSingle;
			pictureBoxViewEditorRubberBand.Location = new Point(33, 102);
			pictureBoxViewEditorRubberBand.Margin = new Padding(0);
			pictureBoxViewEditorRubberBand.Name = "pictureBoxViewEditorRubberBand";
			pictureBoxViewEditorRubberBand.Size = new Size(20, 20);
			pictureBoxViewEditorRubberBand.TabIndex = 19;
			pictureBoxViewEditorRubberBand.TabStop = false;
			pictureBoxViewEditorRubberBand.Visible = false;
			pictureBoxViewEditorRubberBand.Resize += pictureBoxViewEditorRubberBand_Resize;
			// 
			// MemoExport
			// 
			MemoExport.Location = new Point(343, 83);
			MemoExport.Name = "MemoExport";
			MemoExport.Size = new Size(342, 324);
			MemoExport.TabIndex = 20;
			MemoExport.Text = "";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(8, 298);
			label3.Name = "label3";
			label3.Size = new Size(43, 13);
			label3.TabIndex = 21;
			label3.Text = "From X:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(8, 326);
			label4.Name = "label4";
			label4.Size = new Size(43, 13);
			label4.TabIndex = 22;
			label4.Text = "From Y:";
			// 
			// numericFromX
			// 
			numericFromX.Location = new Point(52, 296);
			numericFromX.Maximum = new decimal(new int[] { 39, 0, 0, 0 });
			numericFromX.Name = "numericFromX";
			numericFromX.Size = new Size(49, 20);
			numericFromX.TabIndex = 24;
			numericFromX.ValueChanged += numericFromX_ValueChanged;
			// 
			// numericFromY
			// 
			numericFromY.Location = new Point(52, 322);
			numericFromY.Maximum = new decimal(new int[] { 25, 0, 0, 0 });
			numericFromY.Name = "numericFromY";
			numericFromY.Size = new Size(49, 20);
			numericFromY.TabIndex = 25;
			numericFromY.ValueChanged += numericFromY_ValueChanged;
			// 
			// numericHeight
			// 
			numericHeight.Location = new Point(159, 322);
			numericHeight.Maximum = new decimal(new int[] { 26, 0, 0, 0 });
			numericHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			numericHeight.Name = "numericHeight";
			numericHeight.Size = new Size(49, 20);
			numericHeight.TabIndex = 29;
			numericHeight.Value = new decimal(new int[] { 1, 0, 0, 0 });
			numericHeight.ValueChanged += numericHeight_ValueChanged;
			// 
			// numericWidth
			// 
			numericWidth.Location = new Point(159, 296);
			numericWidth.Maximum = new decimal(new int[] { 40, 0, 0, 0 });
			numericWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			numericWidth.Name = "numericWidth";
			numericWidth.Size = new Size(49, 20);
			numericWidth.TabIndex = 28;
			numericWidth.Value = new decimal(new int[] { 1, 0, 0, 0 });
			numericWidth.ValueChanged += numericWidth_ValueChanged;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new Point(118, 326);
			label5.Name = "label5";
			label5.Size = new Size(41, 13);
			label5.TabIndex = 27;
			label5.Text = "Height:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new Point(118, 298);
			label6.Name = "label6";
			label6.Size = new Size(38, 13);
			label6.TabIndex = 26;
			label6.Text = "Width:";
			// 
			// labelDimensions
			// 
			labelDimensions.AutoSize = true;
			labelDimensions.Location = new Point(98, 354);
			labelDimensions.Name = "labelDimensions";
			labelDimensions.Size = new Size(16, 13);
			labelDimensions.TabIndex = 30;
			labelDimensions.Text = "...";
			// 
			// timerUpdateExportSample
			// 
			timerUpdateExportSample.Interval = 250;
			timerUpdateExportSample.Tick += timerUpdateExportSample_Tick;
			// 
			// buttonResetSelection
			// 
			buttonResetSelection.Location = new Point(253, 296);
			buttonResetSelection.Name = "buttonResetSelection";
			buttonResetSelection.Size = new Size(75, 23);
			buttonResetSelection.TabIndex = 31;
			buttonResetSelection.Text = "Reset";
			buttonResetSelection.UseVisualStyleBackColor = true;
			buttonResetSelection.Click += buttonResetSelection_Click;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new Point(8, 354);
			label7.Name = "label7";
			label7.Size = new Size(84, 13);
			label7.TabIndex = 32;
			label7.Text = "Selected region:";
			// 
			// saveDialog
			// 
			saveDialog.FileName = "saveDialog";
			// 
			// checkBoxRememberState
			// 
			checkBoxRememberState.AutoSize = true;
			checkBoxRememberState.Checked = true;
			checkBoxRememberState.CheckState = CheckState.Checked;
			checkBoxRememberState.Location = new Point(365, 46);
			checkBoxRememberState.Name = "checkBoxRememberState";
			checkBoxRememberState.Size = new Size(164, 17);
			checkBoxRememberState.TabIndex = 33;
			checkBoxRememberState.Text = "Remember current selection?";
			checkBoxRememberState.UseVisualStyleBackColor = true;
			// 
			// checkBoxTranspose
			// 
			checkBoxTranspose.AutoSize = true;
			checkBoxTranspose.Checked = true;
			checkBoxTranspose.CheckState = CheckState.Checked;
			checkBoxTranspose.Location = new Point(12, 383);
			checkBoxTranspose.Name = "checkBoxTranspose";
			checkBoxTranspose.Size = new Size(172, 17);
			checkBoxTranspose.TabIndex = 34;
			checkBoxTranspose.Text = "Column export (transpose data)";
			checkBoxTranspose.UseVisualStyleBackColor = true;
			checkBoxTranspose.CheckedChanged += CheckBoxTranspose_CheckedChanged;
			// 
			// ExportViewWindow
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = Button_Cancel;
			ClientSize = new Size(690, 412);
			Controls.Add(checkBoxTranspose);
			Controls.Add(checkBoxRememberState);
			Controls.Add(label7);
			Controls.Add(buttonResetSelection);
			Controls.Add(labelDimensions);
			Controls.Add(numericHeight);
			Controls.Add(numericWidth);
			Controls.Add(label5);
			Controls.Add(label6);
			Controls.Add(numericFromY);
			Controls.Add(numericFromX);
			Controls.Add(label4);
			Controls.Add(label3);
			Controls.Add(MemoExport);
			Controls.Add(pictureBoxViewEditorRubberBand);
			Controls.Add(pictureBoxAtariViewSmall);
			Controls.Add(Label1);
			Controls.Add(Label2);
			Controls.Add(ComboBoxExportType);
			Controls.Add(ComboBoxDataType);
			Controls.Add(Button_Export);
			Controls.Add(Button_Cancel);
			Controls.Add(ButtonCopyClipboard);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			KeyPreview = true;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ExportViewWindow";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Export view to ...";
			FormClosing += ExportViewWindow_FormClosing;
			Load += ExportViewWindow_Load;
			KeyDown += ExportViewWindow_KeyDown;
			MouseWheel += ExportViewWindow_MouseWheel;
			((System.ComponentModel.ISupportInitialize)pictureBoxAtariViewSmall).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBoxViewEditorRubberBand).EndInit();
			((System.ComponentModel.ISupportInitialize)numericFromX).EndInit();
			((System.ComponentModel.ISupportInitialize)numericFromY).EndInit();
			((System.ComponentModel.ISupportInitialize)numericHeight).EndInit();
			((System.ComponentModel.ISupportInitialize)numericWidth).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private Button Button_Export;
		private Button Button_Cancel;
		private Button ButtonCopyClipboard;
		private Label Label1;
		private Label Label2;
		private ComboBox ComboBoxExportType;
		private ComboBox ComboBoxDataType;
		private PictureBox pictureBoxAtariViewSmall;
		private PictureBox pictureBoxViewEditorRubberBand;
		private RichTextBox MemoExport;
		private Label label3;
		private Label label4;
		private NumericUpDown numericFromX;
		private NumericUpDown numericFromY;
		private NumericUpDown numericHeight;
		private NumericUpDown numericWidth;
		private Label label5;
		private Label label6;
		private Label labelDimensions;
		private System.Windows.Forms.Timer timerUpdateExportSample;
		private Button buttonResetSelection;
		private Label label7;
		private SaveFileDialog saveDialog;
		private CheckBox checkBoxRememberState;
		private CheckBox checkBoxTranspose;
	}
}