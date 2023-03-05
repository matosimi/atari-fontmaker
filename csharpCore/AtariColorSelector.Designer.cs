namespace FontMaker
{
	partial class TAtariColorSelectorForm
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
			ImagePalette = new PictureBox();
			ImageSelected = new PictureBox();
			LabelActualColor = new Label();
			LabelOldColor = new Label();
			((System.ComponentModel.ISupportInitialize)ImagePalette).BeginInit();
			((System.ComponentModel.ISupportInitialize)ImageSelected).BeginInit();
			SuspendLayout();
			// 
			// ImagePalette
			// 
			ImagePalette.Location = new Point(8, 0);
			ImagePalette.Name = "ImagePalette";
			ImagePalette.Size = new Size(145, 272);
			ImagePalette.TabIndex = 0;
			ImagePalette.TabStop = false;
			ImagePalette.MouseDown += ImagePaletteMouseDown;
			ImagePalette.MouseMove += ImagePaletteMouseMove;
			// 
			// ImageSelected
			// 
			ImageSelected.Location = new Point(159, 72);
			ImageSelected.Name = "ImageSelected";
			ImageSelected.Size = new Size(105, 121);
			ImageSelected.TabIndex = 1;
			ImageSelected.TabStop = false;
			// 
			// LabelActualColor
			// 
			LabelActualColor.AutoSize = true;
			LabelActualColor.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			LabelActualColor.Location = new Point(192, 199);
			LabelActualColor.Name = "LabelActualColor";
			LabelActualColor.Size = new Size(40, 13);
			LabelActualColor.TabIndex = 2;
			LabelActualColor.Text = "$01 - 1";
			LabelActualColor.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// LabelOldColor
			// 
			LabelOldColor.AutoSize = true;
			LabelOldColor.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			LabelOldColor.Location = new Point(192, 53);
			LabelOldColor.Name = "LabelOldColor";
			LabelOldColor.Size = new Size(40, 13);
			LabelOldColor.TabIndex = 3;
			LabelOldColor.Text = "$00 - 0";
			LabelOldColor.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// TAtariColorSelectorForm
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			BackgroundImageLayout = ImageLayout.None;
			ClientSize = new Size(284, 280);
			Controls.Add(ImagePalette);
			Controls.Add(ImageSelected);
			Controls.Add(LabelActualColor);
			Controls.Add(LabelOldColor);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Name = "TAtariColorSelectorForm";
			Text = "AtariColorSelector";
			KeyDown += TAtariColorSelectorForm_KeyDown;
			((System.ComponentModel.ISupportInitialize)ImagePalette).EndInit();
			((System.ComponentModel.ISupportInitialize)ImageSelected).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.PictureBox ImagePalette;
		private System.Windows.Forms.PictureBox ImageSelected;
		private System.Windows.Forms.Label LabelActualColor;
		private System.Windows.Forms.Label LabelOldColor;
	}
}