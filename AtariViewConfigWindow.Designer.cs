namespace FontMaker;

partial class AtariViewConfigWindow
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
		numericHeight = new NumericUpDown();
		numericWidth = new NumericUpDown();
		label5 = new Label();
		label6 = new Label();
		label3 = new Label();
		label1 = new Label();
		label2 = new Label();
		labelCurrentInfo = new Label();
		buttonResize = new Button();
		buttonCancel = new Button();
		label4 = new Label();
		labelNewInfo = new Label();
		labelNewDifference = new Label();
		radio1 = new RadioButton();
		radio2 = new RadioButton();
		radio4 = new RadioButton();
		radio3 = new RadioButton();
		label7 = new Label();
		label8 = new Label();
		label9 = new Label();
		((System.ComponentModel.ISupportInitialize)numericHeight).BeginInit();
		((System.ComponentModel.ISupportInitialize)numericWidth).BeginInit();
		SuspendLayout();
		// 
		// numericHeight
		// 
		numericHeight.Location = new Point(213, 63);
		numericHeight.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
		numericHeight.Name = "numericHeight";
		numericHeight.Size = new Size(62, 22);
		numericHeight.TabIndex = 2;
		numericHeight.TextAlign = HorizontalAlignment.Center;
		numericHeight.ThousandsSeparator = true;
		numericHeight.Value = new decimal(new int[] { 26, 0, 0, 0 });
		numericHeight.ValueChanged += numericHeight_ValueChanged;
		numericHeight.KeyPress += numericWidth_KeyPress;
		// 
		// numericWidth
		// 
		numericWidth.Location = new Point(57, 63);
		numericWidth.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
		numericWidth.Name = "numericWidth";
		numericWidth.Size = new Size(62, 22);
		numericWidth.TabIndex = 1;
		numericWidth.TextAlign = HorizontalAlignment.Center;
		numericWidth.ThousandsSeparator = true;
		numericWidth.Value = new decimal(new int[] { 40, 0, 0, 0 });
		numericWidth.ValueChanged += numericWidth_ValueChanged;
		numericWidth.KeyPress += numericWidth_KeyPress;
		// 
		// label5
		// 
		label5.AutoSize = true;
		label5.Location = new Point(165, 65);
		label5.Name = "label5";
		label5.Size = new Size(45, 13);
		label5.TabIndex = 31;
		label5.Text = "Height:";
		// 
		// label6
		// 
		label6.AutoSize = true;
		label6.Location = new Point(8, 65);
		label6.Name = "label6";
		label6.Size = new Size(42, 13);
		label6.TabIndex = 30;
		label6.Text = "Width:";
		// 
		// label3
		// 
		label3.AutoSize = true;
		label3.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
		label3.Location = new Point(2, 2);
		label3.Name = "label3";
		label3.Size = new Size(92, 13);
		label3.TabIndex = 34;
		label3.Text = "Page Dimension";
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(8, 21);
		label1.Name = "label1";
		label1.Size = new Size(72, 13);
		label1.TabIndex = 35;
		label1.Text = "Current Size:";
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Location = new Point(8, 47);
		label2.Name = "label2";
		label2.Size = new Size(56, 13);
		label2.TabIndex = 36;
		label2.Text = "New Size:";
		// 
		// labelCurrentInfo
		// 
		labelCurrentInfo.AutoSize = true;
		labelCurrentInfo.Location = new Point(83, 21);
		labelCurrentInfo.Name = "labelCurrentInfo";
		labelCurrentInfo.Size = new Size(11, 13);
		labelCurrentInfo.TabIndex = 42;
		labelCurrentInfo.Text = "-";
		// 
		// buttonResize
		// 
		buttonResize.Location = new Point(10, 192);
		buttonResize.Name = "buttonResize";
		buttonResize.Size = new Size(100, 40);
		buttonResize.TabIndex = 3;
		buttonResize.Text = "Resize";
		buttonResize.UseVisualStyleBackColor = true;
		buttonResize.Click += buttonResize_Click;
		// 
		// buttonCancel
		// 
		buttonCancel.Location = new Point(176, 195);
		buttonCancel.Name = "buttonCancel";
		buttonCancel.Size = new Size(100, 40);
		buttonCancel.TabIndex = 4;
		buttonCancel.Text = "Cancel";
		buttonCancel.UseVisualStyleBackColor = true;
		// 
		// label4
		// 
		label4.BorderStyle = BorderStyle.FixedSingle;
		label4.Location = new Point(4, 42);
		label4.Name = "label4";
		label4.Size = new Size(280, 2);
		label4.TabIndex = 45;
		// 
		// labelNewInfo
		// 
		labelNewInfo.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
		labelNewInfo.Location = new Point(12, 90);
		labelNewInfo.Name = "labelNewInfo";
		labelNewInfo.Size = new Size(263, 13);
		labelNewInfo.TabIndex = 46;
		labelNewInfo.Text = "-";
		labelNewInfo.TextAlign = ContentAlignment.TopCenter;
		labelNewInfo.UseMnemonic = false;
		// 
		// labelNewDifference
		// 
		labelNewDifference.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
		labelNewDifference.Location = new Point(12, 107);
		labelNewDifference.Name = "labelNewDifference";
		labelNewDifference.Size = new Size(263, 13);
		labelNewDifference.TabIndex = 47;
		labelNewDifference.Text = "-";
		labelNewDifference.TextAlign = ContentAlignment.TopCenter;
		labelNewDifference.UseMnemonic = false;
		// 
		// radio1
		// 
		radio1.AutoSize = true;
		radio1.Checked = true;
		radio1.Location = new Point(72, 154);
		radio1.Name = "radio1";
		radio1.Size = new Size(31, 17);
		radio1.TabIndex = 49;
		radio1.TabStop = true;
		radio1.Text = "1";
		radio1.UseVisualStyleBackColor = true;
		// 
		// radio2
		// 
		radio2.AutoSize = true;
		radio2.Location = new Point(109, 154);
		radio2.Name = "radio2";
		radio2.Size = new Size(31, 17);
		radio2.TabIndex = 50;
		radio2.Text = "2";
		radio2.UseVisualStyleBackColor = true;
		// 
		// radio4
		// 
		radio4.AutoSize = true;
		radio4.Location = new Point(183, 154);
		radio4.Name = "radio4";
		radio4.Size = new Size(31, 17);
		radio4.TabIndex = 52;
		radio4.Text = "4";
		radio4.UseVisualStyleBackColor = true;
		// 
		// radio3
		// 
		radio3.AutoSize = true;
		radio3.Location = new Point(146, 154);
		radio3.Name = "radio3";
		radio3.Size = new Size(31, 17);
		radio3.TabIndex = 51;
		radio3.Text = "3";
		radio3.UseVisualStyleBackColor = true;
		// 
		// label7
		// 
		label7.AutoSize = true;
		label7.Location = new Point(39, 133);
		label7.Name = "label7";
		label7.Size = new Size(208, 13);
		label7.TabIndex = 53;
		label7.Text = "What font nr to use for resized height?";
		// 
		// label8
		// 
		label8.BorderStyle = BorderStyle.FixedSingle;
		label8.Location = new Point(3, 127);
		label8.Name = "label8";
		label8.Size = new Size(280, 2);
		label8.TabIndex = 54;
		// 
		// label9
		// 
		label9.BorderStyle = BorderStyle.FixedSingle;
		label9.Location = new Point(3, 177);
		label9.Name = "label9";
		label9.Size = new Size(280, 2);
		label9.TabIndex = 55;
		// 
		// AtariViewConfigWindow
		// 
		AutoScaleDimensions = new SizeF(6F, 13F);
		AutoScaleMode = AutoScaleMode.Font;
		CancelButton = buttonCancel;
		ClientSize = new Size(287, 241);
		Controls.Add(label9);
		Controls.Add(label8);
		Controls.Add(label7);
		Controls.Add(radio4);
		Controls.Add(radio3);
		Controls.Add(radio2);
		Controls.Add(radio1);
		Controls.Add(labelNewDifference);
		Controls.Add(labelNewInfo);
		Controls.Add(label4);
		Controls.Add(buttonCancel);
		Controls.Add(buttonResize);
		Controls.Add(labelCurrentInfo);
		Controls.Add(label2);
		Controls.Add(label1);
		Controls.Add(label3);
		Controls.Add(numericHeight);
		Controls.Add(numericWidth);
		Controls.Add(label5);
		Controls.Add(label6);
		FormBorderStyle = FormBorderStyle.FixedDialog;
		KeyPreview = true;
		MaximizeBox = false;
		MinimizeBox = false;
		Name = "AtariViewConfigWindow";
		StartPosition = FormStartPosition.CenterParent;
		Text = "Update Page Size";
		((System.ComponentModel.ISupportInitialize)numericHeight).EndInit();
		((System.ComponentModel.ISupportInitialize)numericWidth).EndInit();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private NumericUpDown numericHeight;
	private NumericUpDown numericWidth;
	private Label label5;
	private Label label6;
	private Label label3;
	private Label label1;
	private Label label2;
	private Label labelCurrentInfo;
	private Button buttonResize;
	private Button buttonCancel;
	private Label label4;
	private Label labelNewInfo;
	private Label labelNewDifference;
	private RadioButton radio1;
	private RadioButton radio2;
	private RadioButton radio4;
	private RadioButton radio3;
	private Label label7;
	private Label label8;
	private Label label9;
}