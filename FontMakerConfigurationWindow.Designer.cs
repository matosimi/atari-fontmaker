namespace FontMaker;

partial class FontMakerConfigurationWindow
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
		buttonCancel = new Button();
		buttonOk = new Button();
		groupBox1 = new GroupBox();
		radioButtonZX2 = new RadioButton();
		radioButtonZX0 = new RadioButton();
		radioButtonZX1 = new RadioButton();
		groupBox1.SuspendLayout();
		SuspendLayout();
		// 
		// buttonCancel
		// 
		buttonCancel.Location = new Point(120, 94);
		buttonCancel.Name = "buttonCancel";
		buttonCancel.Size = new Size(100, 40);
		buttonCancel.TabIndex = 5;
		buttonCancel.Text = "Cancel";
		buttonCancel.UseVisualStyleBackColor = true;
		// 
		// buttonOk
		// 
		buttonOk.Location = new Point(3, 94);
		buttonOk.Name = "buttonOk";
		buttonOk.Size = new Size(100, 40);
		buttonOk.TabIndex = 6;
		buttonOk.Text = "Ok";
		buttonOk.UseVisualStyleBackColor = true;
		buttonOk.Click += buttonOk_Click;
		// 
		// groupBox1
		// 
		groupBox1.Controls.Add(radioButtonZX1);
		groupBox1.Controls.Add(radioButtonZX2);
		groupBox1.Controls.Add(radioButtonZX0);
		groupBox1.Location = new Point(12, 12);
		groupBox1.Name = "groupBox1";
		groupBox1.Size = new Size(208, 63);
		groupBox1.TabIndex = 8;
		groupBox1.TabStop = false;
		groupBox1.Text = "Which compressor should be used:";
		// 
		// radioButtonZX2
		// 
		radioButtonZX2.AutoSize = true;
		radioButtonZX2.Location = new Point(142, 32);
		radioButtonZX2.Name = "radioButtonZX2";
		radioButtonZX2.Size = new Size(43, 17);
		radioButtonZX2.TabIndex = 1;
		radioButtonZX2.TabStop = true;
		radioButtonZX2.Text = "ZX2";
		radioButtonZX2.UseVisualStyleBackColor = true;
		// 
		// radioButtonZX0
		// 
		radioButtonZX0.AutoSize = true;
		radioButtonZX0.Location = new Point(22, 32);
		radioButtonZX0.Name = "radioButtonZX0";
		radioButtonZX0.Size = new Size(43, 17);
		radioButtonZX0.TabIndex = 0;
		radioButtonZX0.TabStop = true;
		radioButtonZX0.Text = "ZX0";
		radioButtonZX0.UseVisualStyleBackColor = true;
		// 
		// radioButtonZX1
		// 
		radioButtonZX1.AutoSize = true;
		radioButtonZX1.Location = new Point(82, 32);
		radioButtonZX1.Name = "radioButtonZX1";
		radioButtonZX1.Size = new Size(43, 17);
		radioButtonZX1.TabIndex = 2;
		radioButtonZX1.TabStop = true;
		radioButtonZX1.Text = "ZX1";
		radioButtonZX1.UseVisualStyleBackColor = true;
		// 
		// FontMakerConfigurationWindow
		// 
		AcceptButton = buttonOk;
		AutoScaleDimensions = new SizeF(6F, 13F);
		AutoScaleMode = AutoScaleMode.Font;
		CancelButton = buttonCancel;
		ClientSize = new Size(225, 140);
		Controls.Add(groupBox1);
		Controls.Add(buttonOk);
		Controls.Add(buttonCancel);
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = false;
		MinimizeBox = false;
		Name = "FontMakerConfigurationWindow";
		StartPosition = FormStartPosition.CenterParent;
		Text = "Atari Font Maker Configuration";
		groupBox1.ResumeLayout(false);
		groupBox1.PerformLayout();
		ResumeLayout(false);
	}

	#endregion

	private Button buttonCancel;
	private Button buttonOk;
	private GroupBox groupBox1;
	private RadioButton radioButtonZX2;
	private RadioButton radioButtonZX0;
	private RadioButton radioButtonZX1;
}