namespace FontMaker;
public partial class AtariViewConfigWindow : Form
{
	public int NewWidth => (int)numericWidth.Value;
	public int NewHeight => (int)numericHeight.Value;
	public byte NewFontNr
	{
		get
		{
			if (radio2.Checked)
				return 2;
			if (radio3.Checked)
				return 3;
			if (radio4.Checked)
				return 4;
			return 1;
		}
	}

	public AtariViewConfigWindow()
	{
		InitializeComponent();

		labelCurrentInfo.Text = $"{AtariView.Width}x{AtariView.Height} @ {AtariView.Width * AtariView.Height} bytes";

		numericWidth.Value = AtariView.Width;
		numericHeight.Value = AtariView.Height;
		ActionNewInfoUpdate();
	}

	private void numericWidth_ValueChanged(object sender, EventArgs e)
	{
		ActionNewInfoUpdate();
	}

	private void numericHeight_ValueChanged(object sender, EventArgs e)
	{
		ActionNewInfoUpdate();
	}

	private void ActionNewInfoUpdate(bool limit = true)
	{
		if (limit)
		{
			if (numericWidth.Value < 40)
				numericWidth.Value = 40;

			if (numericHeight.Value < 26)
				numericHeight.Value = 26;
		}

		labelNewInfo.Text = $"{numericWidth.Value}x{numericHeight.Value} @ {numericWidth.Value * numericHeight.Value} bytes";

		if (numericWidth.Value != AtariView.Width || numericHeight.Value != AtariView.Height)
		{
			var xChange = (int)numericWidth.Value - AtariView.Width;
			var yChange = (int)numericHeight.Value - AtariView.Height;
			labelNewDifference.Text = $"Change in width: {xChange}  Change in height: {yChange}";
			//buttonResize.Enabled = true;
		}
		else
		{
			labelNewDifference.Text = "No change!";
			//buttonResize.Enabled = false;
		}
	}

	private void numericWidth_KeyPress(object sender, KeyPressEventArgs e)
	{
		//ActionNewInfoUpdate(false);
	}

	private void buttonResize_Click(object sender, EventArgs e)
	{
		if ((int)numericWidth.Value != AtariView.Width || (int)numericHeight.Value != AtariView.Height)
		{
			// Something has changed
			var doResize = true;

			if ((int)numericWidth.Value < AtariView.Width || (int)numericHeight.Value < AtariView.Height)
			{
				// Ok before a shrink
				var ok = MessageBox.Show(@"Do you want to change the view size? You might loose data?", Constants.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				doResize = ok == DialogResult.Yes;
			}

			if (doResize)
			{
				DialogResult = DialogResult.OK;
				Close();
				return;
			}
		}
		else
		{
			Close();
		}
	}
}
