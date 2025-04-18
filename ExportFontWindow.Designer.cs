namespace FontMaker
{
	partial class ExportFontWindow
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
			Label1 = new Label();
			Label2 = new Label();
			Label3 = new Label();
			ComboBoxExportType = new ComboBox();
			ComboBoxDataType = new ComboBox();
			Button_SaveAs = new Button();
			Button_Cancel = new Button();
			MemoExport = new RichTextBox();
			ComboBoxFontNumber = new ComboBox();
			ButtonCopyClipboard = new Button();
			saveDialog = new SaveFileDialog();
			SuspendLayout();
			// 
			// Label1
			// 
			Label1.AutoSize = true;
			Label1.Location = new Point(8, 46);
			Label1.Name = "Label1";
			Label1.Size = new Size(85, 13);
			Label1.TabIndex = 0;
			Label1.Text = "Export font to :";
			// 
			// Label2
			// 
			Label2.AutoSize = true;
			Label2.Location = new Point(8, 78);
			Label2.Name = "Label2";
			Label2.Size = new Size(62, 13);
			Label2.TabIndex = 1;
			Label2.Text = "Data type :";
			// 
			// Label3
			// 
			Label3.AutoSize = true;
			Label3.Location = new Point(8, 13);
			Label3.Name = "Label3";
			Label3.Size = new Size(68, 13);
			Label3.TabIndex = 2;
			Label3.Text = "Select font :";
			// 
			// ComboBoxExportType
			// 
			ComboBoxExportType.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxExportType.FormattingEnabled = true;
			ComboBoxExportType.Items.AddRange(new object[] { "Image BMP Mono", "Image BMP Color", "Assembler", "Action! ", "Atari Basic", "FastBasic", "MADS dta", "C data {}", "MadPascal Array", "Binary Data", "Basic listing file" });
			ComboBoxExportType.Location = new Point(104, 43);
			ComboBoxExportType.Name = "ComboBoxExportType";
			ComboBoxExportType.Size = new Size(145, 21);
			ComboBoxExportType.TabIndex = 0;
			ComboBoxExportType.SelectedIndexChanged += ComboBoxExportTypeChange;
			// 
			// ComboBoxDataType
			// 
			ComboBoxDataType.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxDataType.FormattingEnabled = true;
			ComboBoxDataType.Location = new Point(104, 75);
			ComboBoxDataType.Name = "ComboBoxDataType";
			ComboBoxDataType.Size = new Size(145, 21);
			ComboBoxDataType.TabIndex = 1;
			ComboBoxDataType.SelectedIndexChanged += ComboBoxDataTypeChange;
			// 
			// Button_SaveAs
			// 
			Button_SaveAs.Location = new Point(269, 8);
			Button_SaveAs.Name = "Button_SaveAs";
			Button_SaveAs.Size = new Size(81, 25);
			Button_SaveAs.TabIndex = 2;
			Button_SaveAs.Text = "Save as...";
			Button_SaveAs.UseVisualStyleBackColor = true;
			Button_SaveAs.Click += ButtonSaveAsClick;
			// 
			// Button_Cancel
			// 
			Button_Cancel.Location = new Point(269, 73);
			Button_Cancel.Name = "Button_Cancel";
			Button_Cancel.Size = new Size(81, 25);
			Button_Cancel.TabIndex = 3;
			Button_Cancel.Text = "Cancel";
			Button_Cancel.UseVisualStyleBackColor = true;
			Button_Cancel.Click += Button_CancelClick;
			// 
			// MemoExport
			// 
			MemoExport.Location = new Point(8, 104);
			MemoExport.Name = "MemoExport";
			MemoExport.Size = new Size(342, 324);
			MemoExport.TabIndex = 4;
			MemoExport.Text = "";
			MemoExport.KeyPress += MemoExportKeyPress;
			// 
			// ComboBoxFontNumber
			// 
			ComboBoxFontNumber.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxFontNumber.FormattingEnabled = true;
			ComboBoxFontNumber.Items.AddRange(new object[] { "1", "2", "3", "4", "1+2", "3+4", "1+2+3+4" });
			ComboBoxFontNumber.Location = new Point(104, 8);
			ComboBoxFontNumber.Name = "ComboBoxFontNumber";
			ComboBoxFontNumber.Size = new Size(145, 21);
			ComboBoxFontNumber.TabIndex = 5;
			ComboBoxFontNumber.SelectedIndexChanged += ComboBoxFontNumber_SelectedIndexChanged;
			// 
			// ButtonCopyClipboard
			// 
			ButtonCopyClipboard.Location = new Point(269, 42);
			ButtonCopyClipboard.Name = "ButtonCopyClipboard";
			ButtonCopyClipboard.Size = new Size(81, 25);
			ButtonCopyClipboard.TabIndex = 6;
			ButtonCopyClipboard.Text = "To Clipboard";
			ButtonCopyClipboard.UseVisualStyleBackColor = true;
			ButtonCopyClipboard.Click += ButtonCopyClipboardClick;
			// 
			// saveDialog
			// 
			saveDialog.FileName = "saveDialog";
			// 
			// ExportFontWindow
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			BackgroundImageLayout = ImageLayout.None;
			CancelButton = Button_Cancel;
			ClientSize = new Size(360, 443);
			Controls.Add(Label1);
			Controls.Add(Label2);
			Controls.Add(Label3);
			Controls.Add(ComboBoxExportType);
			Controls.Add(ComboBoxDataType);
			Controls.Add(Button_SaveAs);
			Controls.Add(Button_Cancel);
			Controls.Add(MemoExport);
			Controls.Add(ComboBoxFontNumber);
			Controls.Add(ButtonCopyClipboard);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ExportFontWindow";
			Padding = new Padding(505, 354, 0, 0);
			Text = "Export font to ...";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.ComboBox ComboBoxExportType;
		private System.Windows.Forms.ComboBox ComboBoxDataType;
		private System.Windows.Forms.Button Button_SaveAs;
		private System.Windows.Forms.Button Button_Cancel;
		private System.Windows.Forms.RichTextBox MemoExport;
		private System.Windows.Forms.ComboBox ComboBoxFontNumber;
		private System.Windows.Forms.Button ButtonCopyClipboard;
		private System.Windows.Forms.SaveFileDialog saveDialog;
	}
}