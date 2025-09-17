namespace FontMaker;

partial class ImportViewWindow
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
		Button_Cancel = new Button();
		pictureBoxAtariViewSmall = new PictureBox();
		Button_LoadData = new Button();
		numericHeight = new NumericUpDown();
		numericWidth = new NumericUpDown();
		label5 = new Label();
		label6 = new Label();
		numericSkipY = new NumericUpDown();
		numericSkipX = new NumericUpDown();
		label4 = new Label();
		label3 = new Label();
		label7 = new Label();
		labelFileSize = new Label();
		numericLineWidth = new NumericUpDown();
		label1 = new Label();
		Button_Import = new Button();
		timerUpdateExportSample = new System.Windows.Forms.Timer(components);
		dialogOpenFile = new OpenFileDialog();
		label2 = new Label();
		labelActionInfo = new Label();
		labelSomeMoreInfo = new Label();
		checkBoxRememberState = new CheckBox();
		((System.ComponentModel.ISupportInitialize)pictureBoxAtariViewSmall).BeginInit();
		((System.ComponentModel.ISupportInitialize)numericHeight).BeginInit();
		((System.ComponentModel.ISupportInitialize)numericWidth).BeginInit();
		((System.ComponentModel.ISupportInitialize)numericSkipY).BeginInit();
		((System.ComponentModel.ISupportInitialize)numericSkipX).BeginInit();
		((System.ComponentModel.ISupportInitialize)numericLineWidth).BeginInit();
		SuspendLayout();
		// 
		// Button_Cancel
		// 
		Button_Cancel.Location = new Point(12, 409);
		Button_Cancel.Name = "Button_Cancel";
		Button_Cancel.Size = new Size(81, 25);
		Button_Cancel.TabIndex = 13;
		Button_Cancel.Text = "Cancel";
		Button_Cancel.UseVisualStyleBackColor = true;
		Button_Cancel.Click += Button_Cancel_Click;
		// 
		// pictureBoxAtariViewSmall
		// 
		pictureBoxAtariViewSmall.Location = new Point(12, 195);
		pictureBoxAtariViewSmall.Name = "pictureBoxAtariViewSmall";
		pictureBoxAtariViewSmall.Size = new Size(320, 208);
		pictureBoxAtariViewSmall.TabIndex = 19;
		pictureBoxAtariViewSmall.TabStop = false;
		// 
		// Button_LoadData
		// 
		Button_LoadData.Location = new Point(12, 12);
		Button_LoadData.Name = "Button_LoadData";
		Button_LoadData.Size = new Size(81, 25);
		Button_LoadData.TabIndex = 20;
		Button_LoadData.Text = "Load Data ...";
		Button_LoadData.UseVisualStyleBackColor = true;
		Button_LoadData.Click += Button_LoadData_Click;
		// 
		// numericHeight
		// 
		numericHeight.Location = new Point(171, 141);
		numericHeight.Maximum = new decimal(new int[] { 26, 0, 0, 0 });
		numericHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
		numericHeight.Name = "numericHeight";
		numericHeight.Size = new Size(49, 22);
		numericHeight.TabIndex = 37;
		numericHeight.Value = new decimal(new int[] { 20, 0, 0, 0 });
		numericHeight.ValueChanged += UpdateConstraints;
		// 
		// numericWidth
		// 
		numericWidth.Location = new Point(171, 115);
		numericWidth.Maximum = new decimal(new int[] { 40, 0, 0, 0 });
		numericWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
		numericWidth.Name = "numericWidth";
		numericWidth.Size = new Size(49, 22);
		numericWidth.TabIndex = 36;
		numericWidth.Value = new decimal(new int[] { 40, 0, 0, 0 });
		numericWidth.ValueChanged += UpdateConstraints;
		// 
		// label5
		// 
		label5.AutoSize = true;
		label5.Location = new Point(125, 145);
		label5.Name = "label5";
		label5.Size = new Size(45, 13);
		label5.TabIndex = 35;
		label5.Text = "Height:";
		// 
		// label6
		// 
		label6.AutoSize = true;
		label6.Location = new Point(125, 119);
		label6.Name = "label6";
		label6.Size = new Size(42, 13);
		label6.TabIndex = 34;
		label6.Text = "Width:";
		// 
		// numericSkipY
		// 
		numericSkipY.Location = new Point(64, 141);
		numericSkipY.Maximum = new decimal(new int[] { 250, 0, 0, 0 });
		numericSkipY.Name = "numericSkipY";
		numericSkipY.Size = new Size(49, 22);
		numericSkipY.TabIndex = 33;
		numericSkipY.ValueChanged += UpdateConstraints;
		// 
		// numericSkipX
		// 
		numericSkipX.Location = new Point(64, 115);
		numericSkipX.Maximum = new decimal(new int[] { 254, 0, 0, 0 });
		numericSkipX.Name = "numericSkipX";
		numericSkipX.Size = new Size(49, 22);
		numericSkipX.TabIndex = 32;
		numericSkipX.ValueChanged += UpdateConstraints;
		// 
		// label4
		// 
		label4.AutoSize = true;
		label4.Location = new Point(20, 145);
		label4.Name = "label4";
		label4.Size = new Size(40, 13);
		label4.TabIndex = 31;
		label4.Text = "Skip Y:";
		// 
		// label3
		// 
		label3.AutoSize = true;
		label3.Location = new Point(20, 119);
		label3.Name = "label3";
		label3.Size = new Size(41, 13);
		label3.TabIndex = 30;
		label3.Text = "Skip X:";
		// 
		// label7
		// 
		label7.AutoSize = true;
		label7.Location = new Point(99, 18);
		label7.Name = "label7";
		label7.Size = new Size(58, 13);
		label7.TabIndex = 38;
		label7.Text = "Data info:";
		// 
		// labelFileSize
		// 
		labelFileSize.AutoSize = true;
		labelFileSize.Location = new Point(156, 18);
		labelFileSize.Name = "labelFileSize";
		labelFileSize.Size = new Size(16, 13);
		labelFileSize.TabIndex = 39;
		labelFileSize.Text = "...";
		// 
		// numericLineWidth
		// 
		numericLineWidth.Location = new Point(78, 49);
		numericLineWidth.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
		numericLineWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
		numericLineWidth.Name = "numericLineWidth";
		numericLineWidth.Size = new Size(49, 22);
		numericLineWidth.TabIndex = 41;
		numericLineWidth.Value = new decimal(new int[] { 40, 0, 0, 0 });
		numericLineWidth.ValueChanged += UpdateConstraints;
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(12, 53);
		label1.Name = "label1";
		label1.Size = new Size(66, 13);
		label1.TabIndex = 40;
		label1.Text = "Line Width:";
		// 
		// Button_Import
		// 
		Button_Import.Location = new Point(251, 409);
		Button_Import.Name = "Button_Import";
		Button_Import.Size = new Size(81, 25);
		Button_Import.TabIndex = 42;
		Button_Import.Text = "Import";
		Button_Import.UseVisualStyleBackColor = true;
		Button_Import.Click += Button_Import_Click;
		// 
		// timerUpdateExportSample
		// 
		timerUpdateExportSample.Interval = 250;
		// 
		// dialogOpenFile
		// 
		dialogOpenFile.FileName = "Default";
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Location = new Point(135, 53);
		label2.Name = "label2";
		label2.Size = new Size(178, 13);
		label2.TabIndex = 43;
		label2.Text = "<-- How many bytes per data line";
		// 
		// labelActionInfo
		// 
		labelActionInfo.Location = new Point(224, 115);
		labelActionInfo.Name = "labelActionInfo";
		labelActionInfo.Size = new Size(108, 48);
		labelActionInfo.TabIndex = 44;
		labelActionInfo.Text = "...";
		// 
		// labelSomeMoreInfo
		// 
		labelSomeMoreInfo.BorderStyle = BorderStyle.FixedSingle;
		labelSomeMoreInfo.Location = new Point(12, 74);
		labelSomeMoreInfo.Name = "labelSomeMoreInfo";
		labelSomeMoreInfo.Size = new Size(320, 38);
		labelSomeMoreInfo.TabIndex = 45;
		labelSomeMoreInfo.Text = "...";
		// 
		// checkBoxRememberState
		// 
		checkBoxRememberState.AutoSize = true;
		checkBoxRememberState.Checked = true;
		checkBoxRememberState.CheckState = CheckState.Checked;
		checkBoxRememberState.Location = new Point(20, 171);
		checkBoxRememberState.Name = "checkBoxRememberState";
		checkBoxRememberState.Size = new Size(174, 17);
		checkBoxRememberState.TabIndex = 46;
		checkBoxRememberState.Text = "Remember current selection?";
		checkBoxRememberState.UseVisualStyleBackColor = true;
		// 
		// ImportViewWindow
		// 
		AutoScaleDimensions = new SizeF(6F, 13F);
		AutoScaleMode = AutoScaleMode.Font;
		CancelButton = Button_Cancel;
		ClientSize = new Size(344, 438);
		Controls.Add(checkBoxRememberState);
		Controls.Add(labelSomeMoreInfo);
		Controls.Add(labelActionInfo);
		Controls.Add(label2);
		Controls.Add(Button_Import);
		Controls.Add(numericLineWidth);
		Controls.Add(label1);
		Controls.Add(labelFileSize);
		Controls.Add(label7);
		Controls.Add(numericHeight);
		Controls.Add(numericWidth);
		Controls.Add(label5);
		Controls.Add(label6);
		Controls.Add(numericSkipY);
		Controls.Add(numericSkipX);
		Controls.Add(label4);
		Controls.Add(label3);
		Controls.Add(Button_LoadData);
		Controls.Add(pictureBoxAtariViewSmall);
		Controls.Add(Button_Cancel);
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = false;
		MinimizeBox = false;
		Name = "ImportViewWindow";
		StartPosition = FormStartPosition.CenterScreen;
		Text = "Import Binary data into View";
		FormClosing += ImportViewWindow_FormClosing;
		Load += ImportViewWindow_Load;
		((System.ComponentModel.ISupportInitialize)pictureBoxAtariViewSmall).EndInit();
		((System.ComponentModel.ISupportInitialize)numericHeight).EndInit();
		((System.ComponentModel.ISupportInitialize)numericWidth).EndInit();
		((System.ComponentModel.ISupportInitialize)numericSkipY).EndInit();
		((System.ComponentModel.ISupportInitialize)numericSkipX).EndInit();
		((System.ComponentModel.ISupportInitialize)numericLineWidth).EndInit();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private Button Button_Cancel;
	private PictureBox pictureBoxAtariViewSmall;
	private Button Button_LoadData;
	private NumericUpDown numericHeight;
	private NumericUpDown numericWidth;
	private Label label5;
	private Label label6;
	private NumericUpDown numericSkipY;
	private NumericUpDown numericSkipX;
	private Label label4;
	private Label label3;
	private Label label7;
	private Label labelFileSize;
	private NumericUpDown numericLineWidth;
	private Label label1;
	private Button Button_Import;
	private System.Windows.Forms.Timer timerUpdateExportSample;
	private OpenFileDialog dialogOpenFile;
	private Label label2;
	private Label labelActionInfo;
	private Label labelSomeMoreInfo;
	private CheckBox checkBoxRememberState;
}