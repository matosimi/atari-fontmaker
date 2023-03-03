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
			this.ImagePalette = new System.Windows.Forms.PictureBox();
			this.ImageSelected = new System.Windows.Forms.PictureBox();
			this.LabelActualColor = new System.Windows.Forms.Label();
			this.LabelOldColor = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.ImagePalette)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ImageSelected)).BeginInit();
			this.SuspendLayout();
			// 
			// ImagePalette
			// 
			this.ImagePalette.Location = new System.Drawing.Point(8, 0);
			this.ImagePalette.Name = "ImagePalette";
			this.ImagePalette.Size = new System.Drawing.Size(145, 272);
			this.ImagePalette.TabIndex = 0;
			this.ImagePalette.TabStop = false;
			this.ImagePalette.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImagePaletteMouseDown);
			this.ImagePalette.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImagePaletteMouseMove);
			// 
			// ImageSelected
			// 
			this.ImageSelected.Location = new System.Drawing.Point(159, 72);
			this.ImageSelected.Name = "ImageSelected";
			this.ImageSelected.Size = new System.Drawing.Size(105, 121);
			this.ImageSelected.TabIndex = 1;
			this.ImageSelected.TabStop = false;
			// 
			// LabelActualColor
			// 
			this.LabelActualColor.AutoSize = true;
			this.LabelActualColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.LabelActualColor.Location = new System.Drawing.Point(192, 199);
			this.LabelActualColor.Name = "LabelActualColor";
			this.LabelActualColor.Size = new System.Drawing.Size(40, 13);
			this.LabelActualColor.TabIndex = 2;
			this.LabelActualColor.Text = "$01 - 1";
			this.LabelActualColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LabelOldColor
			// 
			this.LabelOldColor.AutoSize = true;
			this.LabelOldColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.LabelOldColor.Location = new System.Drawing.Point(192, 53);
			this.LabelOldColor.Name = "LabelOldColor";
			this.LabelOldColor.Size = new System.Drawing.Size(40, 13);
			this.LabelOldColor.TabIndex = 3;
			this.LabelOldColor.Text = "$00 - 0";
			this.LabelOldColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// AtariColorSelector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(284, 280);
			this.Controls.Add(this.ImagePalette);
			this.Controls.Add(this.ImageSelected);
			this.Controls.Add(this.LabelActualColor);
			this.Controls.Add(this.LabelOldColor);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AtariColorSelector";
			this.Text = "AtariColorSelector";
			((System.ComponentModel.ISupportInitialize)(this.ImagePalette)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ImageSelected)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox ImagePalette;
		private System.Windows.Forms.PictureBox ImageSelected;
		private System.Windows.Forms.Label LabelActualColor;
		private System.Windows.Forms.Label LabelOldColor;
	}
}