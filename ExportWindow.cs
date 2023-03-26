using System.Drawing.Imaging;
using static FontMaker.ExportWindowUnit;

namespace FontMaker
{
	public partial class ExportWindow : Form
	{
		public ExportWindow()
		{
			InitializeComponent();
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(FormClose!);
			this.Load += new System.EventHandler(FormCreate!);
		}

		public void FormCreate(object sender, EventArgs e)
		{
			DoubleBuffered = true;
			ClearMemo();
			ComboBoxFontNumber.SelectedIndex = 0;
			ComboBoxExportType.SelectedIndex = 0;
			ComboBoxDataType.SelectedIndex = -1;
		}

		public void FormClose(object sender, FormClosedEventArgs action)
		{
			Button_Export.Enabled = false;
			//ComboBoxExportType.SelectedIndex = -1;
			//ComboBoxDataType.SelectedIndex = -1;
			ComboBoxDataType.Enabled = false;
			ComboBoxExportType.Text = "select an item";
			ComboBoxDataType.Text = "select an item";
		}

		public void ComboBoxExportTypeChange(object sender, EventArgs e)
		{
			if (ComboBoxExportType.SelectedIndex >= 0)
			{
				ComboBoxDataType.Text = string.Empty;
				ComboBoxDataType.Items.Clear();
				ComboBoxDataType.Enabled = true;
				/* bmp, assembler, action, basic, fastbasic, mads_dta, basic LST */
				MemoExport.Enabled = true;

				switch ((FormatTypes)ComboBoxExportType.SelectedIndex)
				{
					case FormatTypes.ImageBMP:
						{
							ComboBoxDataType.Items.Add("Binary");
							ComboBoxDataType.SelectedIndex = 0;
							ComboBoxDataType.Enabled = false;
							Button_Export.Enabled = true;
							MemoExport.Text = string.Empty;
						}
						break;

					case FormatTypes.Assembler:
						{
							ComboBoxDataType.Text = "Select an item";
							ComboBoxDataType.Items.Add("Byte in decimal");
							ComboBoxDataType.Items.Add("Byte in hexadecimal");
							ComboBoxDataType.SelectedIndex = 0;
							Button_Export.Enabled = false;
						}
						break;

					case FormatTypes.Action:
						{
							ComboBoxDataType.Text = "Select an item";
							ComboBoxDataType.Items.Add("Byte in decimal");
							ComboBoxDataType.Items.Add("Byte in hexadecimal");
							ComboBoxDataType.SelectedIndex = 0;
							Button_Export.Enabled = false;
						}
						break;

					case FormatTypes.AtariBasic:
						{
							ComboBoxDataType.Items.Add("Byte in decimal");
							ComboBoxDataType.SelectedIndex = 0;
							ComboBoxDataType.Enabled = false;
							Button_Export.Enabled = true;
						}
						break;

					case FormatTypes.FastBasic:
						{
							ComboBoxDataType.Items.Add("Byte in decimal");
							ComboBoxDataType.SelectedIndex = 0;
							ComboBoxDataType.Enabled = false;
							Button_Export.Enabled = true;
						}
						break;

					case FormatTypes.MADSdta:
						{
							ComboBoxDataType.Text = "Select an item";
							ComboBoxDataType.Items.Add("Byte in decimal");
							ComboBoxDataType.Items.Add("Byte in hexadecimal");
							ComboBoxDataType.SelectedIndex = 0;
							Button_Export.Enabled = false;
						}
						break;

					case FormatTypes.BasicListingFile:
						{
							ComboBoxDataType.Items.Add("Basic listing");
							ComboBoxDataType.SelectedIndex = 0;
							ComboBoxDataType.Enabled = false;
							Button_Export.Enabled = true;
							MemoExport.Text =
								"This export option generates basic .LST file that can be easily incorporated to your " +
								"own basic source with ENTER \"D:filename.LST\". Export file contains Basic lines 0 to 11 " +
								"with assembly routine that very quickly deploys font to memtop.";
						}
						break;
				}

				ComboBoxDataTypeChange(this, EventArgs.Empty);
			}
		}

		public void ComboBoxDataTypeChange(object sender, EventArgs e)
		{
			if (ComboBoxExportType.SelectedIndex >= 0)
			{
				Button_Export.Enabled = true;
			}

			if ((ComboBoxExportType.SelectedIndex > 0) && (ComboBoxExportType.SelectedIndex < 6))
			{
				MemoExport.Text = ExportWindowUnit.GenerateFileAsText(
					ComboBoxFontNumber.SelectedIndex,
					(FormatTypes)ComboBoxExportType.SelectedIndex,
					ComboBoxDataType.SelectedIndex);
			}
		}

		public void Button_ExportClick(object sender, EventArgs e)
		{
			var format = (FormatTypes)ComboBoxExportType.SelectedIndex;

			if ((FormatTypes)ComboBoxExportType.SelectedIndex == FormatTypes.ImageBMP)
			{
				if (ComboBoxFontNumber.SelectedIndex == 0)
				{
					d_save.Filter = "Font1 (*.bmp)|*.bmp";
				}
				else
				{
					d_save.Filter = "Font2 (*.bmp)|*.bmp";
				}

				d_save.DefaultExt = "bmp";

				if (d_save.ShowDialog() == DialogResult.OK)
				{
					ExportWindowUnit.SaveFontBMP(ComboBoxFontNumber.SelectedIndex, d_save.FileName, MainUnit.MainForm.bmpFontBanks);
				}

				return;
			}

			if (ComboBoxExportType.SelectedIndex >= 1 && ComboBoxExportType.SelectedIndex < 6)
			{
				if (ComboBoxFontNumber.SelectedIndex == 0)
				{
					d_save.Filter = "Font1 (*.txt)|*.txt";
				}
				else
				{
					d_save.Filter = "Font2 (*.txt)|*.txt";
				}

				d_save.DefaultExt = "txt";

				if (d_save.ShowDialog() == DialogResult.OK)
				{
					var text = ExportWindowUnit.GenerateFileAsText(ComboBoxFontNumber.SelectedIndex, (FormatTypes)ComboBoxExportType.SelectedIndex, ComboBoxDataType.SelectedIndex);

					File.WriteAllText(d_save.FileName, text);
				}

				return;
			}

			if (ComboBoxExportType.SelectedIndex == 6)
			{
				if (ComboBoxFontNumber.SelectedIndex == 0)
				{
					d_save.Filter = "Font1 (*.lst)|*.lst";
				}
				else
				{
					d_save.Filter = "Font2 (*.lst)|*.lst";
				}

				d_save.DefaultExt = "lst";

				if (d_save.ShowDialog() == DialogResult.OK)
				{
					ExportWindowUnit.SaveRemFont(ComboBoxFontNumber.SelectedIndex, d_save.FileName);
				}
			}
		}

		public void Button_CancelClick(object sender, EventArgs e)
		{
			Close();
		}

		public void MemoExportKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 'A')
			{
				(sender as RichTextBox).SelectAll();
				e.KeyChar = '\x0';
			}
		}

		public void ButtonCopyClipboardClick(object sender, EventArgs e)
		{
			if (MemoExport.Text.Length > 0)
				Clipboard.SetText(MemoExport.Text);
		}

		public void ClearMemo()
		{
			MemoExport.Clear();
		}

		private void ComboBoxFontNumber_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxDataTypeChange(null, EventArgs.Empty);
		}
	}
}
