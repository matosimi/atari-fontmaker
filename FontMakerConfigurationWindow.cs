using System.ComponentModel;

namespace FontMaker;
public partial class FontMakerConfigurationWindow : Form
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Compressors.CompressorType CompressorId { get; set; }

	public FontMakerConfigurationWindow(Compressors.CompressorType compressorId)
	{
		InitializeComponent();

		CompressorId = compressorId;

		switch (compressorId)
		{
			case Compressors.CompressorType.ZX2: radioButtonZX2.Checked = true; break;
			case Compressors.CompressorType.ZX1: radioButtonZX1.Checked = true; break;
			case Compressors.CompressorType.ZX0:
			default:
				radioButtonZX0.Checked = true; break;
		}

	}

	private void buttonOk_Click(object sender, EventArgs e)
	{
		if (radioButtonZX0.Checked)
			CompressorId = Compressors.CompressorType.ZX0;
		else if (radioButtonZX1.Checked)
			CompressorId = Compressors.CompressorType.ZX1;
		else if (radioButtonZX2.Checked)
			CompressorId = Compressors.CompressorType.ZX2;
		else
		{
			CompressorId = Compressors.CompressorType.ZX0;
		}

		DialogResult = DialogResult.OK;
		Close();
	}
}
