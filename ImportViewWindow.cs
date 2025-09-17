#pragma warning disable WFO1000

namespace FontMaker;
public partial class ImportViewWindow : Form
{

	private const int PREVIEW_WIDTH = 40;
	private const int PREVIEW_HEIGHT = 26;
	public bool InColorMode { get; set; }
	public int WhichColorMode { get; set; }

	private byte[]? FileDataToImportFrom { get; set; }

	// 40x26 view buffer
	private byte[,]? ImportedViewBytes { get; set; }

	private bool RememberSelection { get; set; }
	private int PreviousLineWidth { get; set; } = 1;
	private int PreviousSkipX { get; set; }
	private int PreviousSkipY { get; set; }
	private int PreviousWidth { get; set; } = 1;
	private int PreviousHeight { get; set; } = 1;

	public ImportViewWindow()
	{
		InitializeComponent();
		RememberSelection = true;
		UpdateDataFeedback();
	}

	public void LoadConfiguration(bool rememberSelection, int lineWidth, int skipX, int skipY, int width, int height)
	{
		if (rememberSelection)
		{
			if (lineWidth < 1)
				lineWidth = 1;
			if (skipX < 0) skipX = 0;
			if (skipY < 0) skipY = 0;
			if (width < 1) width = 1;
			if (height < 1) height = 1;

			PreviousLineWidth = lineWidth;
			PreviousSkipX = skipX;
			PreviousSkipY = skipY;
			PreviousWidth = width;
			PreviousHeight = height;
		}
	}

	public (bool, int, int, int, int, int) SaveConfiguration()
	{
		return (RememberSelection, PreviousLineWidth, PreviousSkipX, PreviousSkipY, PreviousWidth, PreviousHeight);
	}

	/// <summary>
	/// Form is loading, set the saved/initial load configuration
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ImportViewWindow_Load(object sender, EventArgs e)
	{
		checkBoxRememberState.Checked = RememberSelection;
		if (RememberSelection)
		{
			numericLineWidth.Value = PreviousLineWidth;
			numericSkipX.Value = PreviousSkipX;
			numericSkipY.Value = PreviousSkipY;
			numericWidth.Value = PreviousWidth;
			numericHeight.Value = PreviousHeight;
		}
	}

	/// <summary>
	/// Form is closing, save the configuration if needed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ImportViewWindow_FormClosing(object sender, FormClosingEventArgs e)
	{
		RememberSelection = checkBoxRememberState.Checked;
		if (checkBoxRememberState.Checked)
		{
			PreviousLineWidth = (int)numericLineWidth.Value;
			PreviousSkipX = (int)numericSkipX.Value;
			PreviousSkipY = (int)numericSkipY.Value;
			PreviousWidth = (int)numericWidth.Value;
			PreviousHeight = (int)numericHeight.Value;
		}
	}

	private void Button_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	/// <summary>
	/// Load data to be imported.
	/// Loaded data will end up in 'FileDataToImportFrom'
	/// </summary>
	private void Button_LoadData_Click(object sender, EventArgs e)
	{
		dialogOpenFile.FileName = string.Empty;
		dialogOpenFile.Filter = $@"Any binary data|*.*";
		var ok = dialogOpenFile.ShowDialog();

		if (ok == DialogResult.OK)
		{
			FileDataToImportFrom = null;
			try
			{
				FileDataToImportFrom = File.ReadAllBytes(dialogOpenFile.FileName);
				if (FileDataToImportFrom == null || FileDataToImportFrom.Length == 0)
				{
					MessageBox.Show($@"Failed to load '{dialogOpenFile.FileName}'. File is empty.");
					FileDataToImportFrom = null;
				}
				UpdateDataFeedback();
			}
			catch (Exception ex)
			{
				MessageBox.Show($@"Failed to load '{dialogOpenFile.FileName}'. Reason:{ex.Message}");
				FileDataToImportFrom = null;
				UpdateDataFeedback();
			}
		}
	}

	private void UpdateDataFeedback()
	{
		labelFileSize.Text = FileDataToImportFrom == null ? "(Load some data)" : $"{FileDataToImportFrom.Length} bytes";

		numericSkipX.Enabled = FileDataToImportFrom != null;
		numericSkipY.Enabled = FileDataToImportFrom != null;
		numericWidth.Enabled = FileDataToImportFrom != null;
		numericHeight.Enabled = FileDataToImportFrom != null;
		numericLineWidth.Enabled = FileDataToImportFrom != null;

		UpdateConstraints(null, EventArgs.Empty);
	}

	private void UpdateConstraints(object? _, EventArgs __)
	{
		var widthOk = numericSkipX.Value + numericWidth.Value <= (int)numericLineWidth.Value;
		numericSkipX.ForeColor = widthOk ? Color.Black : Color.Red;
		numericWidth.ForeColor = widthOk ? Color.Black : Color.Red;

		var heightOk = ((int)numericLineWidth.Value * numericSkipY.Value + (int)numericLineWidth.Value * numericHeight.Value <= (FileDataToImportFrom?.Length ?? 0));
		numericSkipY.ForeColor = heightOk ? Color.Black : Color.Red;
		numericHeight.ForeColor = heightOk ? Color.Black : Color.Red;

		labelActionInfo.Text = $"Skip {numericSkipX.Value} byte and then take {numericWidth.Value} bytes.";

		labelSomeMoreInfo.Text = FileDataToImportFrom == null ? "..." : $"# of lines: {(FileDataToImportFrom.Length / (int)numericLineWidth.Value)}";

		Button_Import.Enabled = widthOk && heightOk;

		if (widthOk && heightOk)
		{
			GenerateData();
			RenderTestData();
		}
	}

	private void GenerateData()
	{
		if (FileDataToImportFrom == null)
			return;

		// Copy the data from FileData into ViewBytes
		ImportedViewBytes = new byte[AtariView.Width, AtariView.Height];

		var lineWidth = (int)numericLineWidth.Value;
		var skipX = (int)numericSkipX.Value;
		var skipY = (int)numericSkipY.Value;
		var width = (int)numericWidth.Value;
		var height = (int)numericHeight.Value;

		var srcIndex = skipY * lineWidth;
		for (var y = 0; y < height; ++y)
		{
			for (var x = 0; x < width; ++x)
			{
				ImportedViewBytes[x, y] = FileDataToImportFrom[srcIndex + skipX + x];
			}

			srcIndex += lineWidth;
		}
	}

	/// <summary>
	/// Render the imported data into the 40x26 view
	/// </summary>
	private void RenderTestData()
	{
		if (ImportedViewBytes == null)
			return;

		var colorOffset = InColorMode ? 512 : 0;
		var img = Helpers.GetImage(pictureBoxAtariViewSmall);
		using (var gr = Graphics.FromImage(img))
		{
			var destRect = new Rectangle
			{
				Width = 1,
				Height = 1,
			};

			var srcRect = new Rectangle
			{
				Width = 1,
				Height = 1,
			};

			for (var y = 0; y < PREVIEW_HEIGHT; y++)
			{
				for (var x = 0; x < PREVIEW_WIDTH; x++)
				{
					var rx = ImportedViewBytes[x, y] % 32;
					var ry = ImportedViewBytes[x, y] / 32;

					destRect.X = x * 8;
					destRect.Y = y * 8;

					srcRect.X = rx * 16;
					srcRect.Y = ry * 16 + Constants.FontYOffset[AtariView.UseFontOnLine[y] - 1] + colorOffset;

					for (var h = 0; h < 8; ++h)
					{
						for (var w = 0; w < 8; ++w)
						{
							gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect, GraphicsUnit.Pixel);
							destRect.X++;
							srcRect.X += 2;
						}

						destRect.X -= 8;
						srcRect.X -= 16;
						destRect.Y++;
						srcRect.Y += 2;
					}

				}
			}
		}
		pictureBoxAtariViewSmall.Refresh();
	}

	private void Button_Import_Click(object sender, EventArgs e)
	{
		if (ImportedViewBytes != null)
		{
			for (var y = 0; y < AtariView.Height; ++y)
			{
				for (var x = 0; x < AtariView.Width; ++x)
				{
					AtariView.ViewBytes[x, y] = ImportedViewBytes[x, y];
				}
			}
		}

		Close();
	}
}
