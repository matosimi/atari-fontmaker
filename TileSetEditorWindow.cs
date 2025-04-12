#pragma warning disable WFO1000
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using TinyJson;
using static FontMaker.FontMakerForm;

namespace FontMaker;

public partial class TileSetEditorWindow : Form
{
	public static readonly SolidBrush GrayBrush = new(Color.Gray);
	public static readonly SolidBrush LightGrayBrush = new(Color.LightGray);

	private const int TileEditorHeight = 80;
	private const int OneCharInEditorDimension = 16;

	private const int TileSetPictureWidth = 800;
	private const int TileSetPictureHeight = 80;

	private const int CurrentTilePictureWidth = 80;
	private const int CurrentTilePictureHeight = 80;

	private int LastViewCharacterX { get; set; }
	private int LastViewCharacterY { get; set; }

	private int FontNr { get; set; } = 0;

	private int SelectedCharacterIndex { get; set; } // The current character 0 - 511

	/// <summary>
	/// Which tile set was selected/Which is the current active tile set?
	/// </summary>
	private int SelectedTileNr { get; set; } = 0;

	/// <summary>
	/// Which tile set is shown on the left hand side of the tile set window?
	/// 0 - 90
	/// </summary>
	private int TileSetOffset { get; set; } = 0;

	private FontMakerForm MainForm { get; set; }

	public TileSetEditorWindow(FontMakerForm mainForm)
	{
		MainForm = mainForm;

		// Build the GUI
		InitializeComponent();
		MakeColorBoxes();

		SelectedTileNr = 0;
	}

	private void TilesWindow_Load(object sender, EventArgs e)
	{
		SwitchColorMode();
	}

	/// <summary>
	/// The form is closing.
	/// Let's just hide it and not close it.
	/// </summary>
	private void TileSetEditorWindow_FormClosing(object sender, FormClosingEventArgs e)
	{
		this.Hide();
		e.Cancel = true; // This cancels the close event
	}

	/// <summary>
	/// Create the character selection rubber band box
	/// </summary>
	private void MakeColorBoxes()
	{
		// Create the character selector picture box
		var img = Helpers.GetImage(pictureBoxFontSelectorRubberBand);
		using (var gr = Graphics.FromImage(img))
		{
			gr.FillRectangle(FontMakerForm.RedBrush, new Rectangle(0, 0, 20, 20));
		}
		var graphicsPath = new GraphicsPath();
		graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
		graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
		graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
		graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

		pictureBoxFontSelectorRubberBand.Region = new Region(graphicsPath);
		pictureBoxFontSelectorRubberBand.Refresh();
		graphicsPath.Dispose();

		ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 0, 0 + 4, 0 + 4, 0));
	}

	/// <summary>
	/// The mouse wheel was moved.
	/// Move the character selector left/right
	/// or if the Control key is pressed move it up/down.
	/// </summary>
	/// <param name="e">Mouse event</param>
	public void Form_MouseWheel(object sender, MouseEventArgs e)
	{
		var step = e.Delta > 0 ? -1 : 1;
		if (Control.ModifierKeys == Keys.Alt)
		{
			if (step < 0) buttonPrevTile_Click(null!, EventArgs.Empty);
			else buttonNextTile_Click(null!, EventArgs.Empty);

			CopyCurrentTileToClipboard(true);
		}
		else if (e.X >= pictureBoxTileSets.Left && e.X < pictureBoxTileSets.Right && e.Y >= pictureBoxTileSets.Top && e.Y < pictureBoxTileSets.Bottom)
		{
			// Over the pictureBox

			// Get the current horizontal scroll position
			var currentScrollPosition = hScrollBarTiles.Value;

			// Calculate the new scroll position
			var newScrollPosition = currentScrollPosition + step;

			// Ensure the new scroll position is within bounds
			newScrollPosition = Math.Max(0, Math.Min(newScrollPosition, hScrollBarTiles.Maximum - 9));

			// Set the new horizontal scroll position
			hScrollBarTiles.Value = newScrollPosition;
		}
		else
		{
			// Font selector is moved
			if (Control.ModifierKeys == Keys.Control)
			{
				step *= 32;
			}

			var nextCharacterIndex = SelectedCharacterIndex + step;

			nextCharacterIndex %= 256;
			if (nextCharacterIndex < 0)
				nextCharacterIndex += 256;

			var bx = nextCharacterIndex % 32;
			var by = nextCharacterIndex / 32;
			ActionFontSelectorMouseDown(new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0));
		}
	}

	private void DrawTileSetView()
	{
		var colorOffset = MainForm.InColorMode ? 512 : 0;

		using var pen = new Pen(WhiteBrush);
		var img = Helpers.GetImage(pictureBoxTileSets);
		using var wrapMode = new ImageAttributes();
		using (var gr = Graphics.FromImage(img))
		{
			var destRect = new Rectangle
			{
				Width = OneCharInEditorDimension,
				Height = OneCharInEditorDimension,
			};

			var srcRect = new Rectangle
			{
				X = 0,
				Y = 0, // Will be set below
				Width = 16,
				Height = 16,
			};

			gr.FillRectangle(GrayBrush, 0, 0, TileSetPictureWidth, TileSetPictureHeight);

			for (var i = 0; i < 10; ++i)
			{
				var drawIt = TileSet.Tiles[i + TileSetOffset];
				var tileXOffset = i * CurrentTilePictureWidth;

				for (var y = 0; y < TileData.TILE_HEIGHT; y++)
				{
					var fontYOffset = (drawIt.SelectedFont[y] - 1) * 128 + colorOffset;

					for (var x = 0; x < TileData.TILE_WIDTH; x++)
					{
						var theChar = drawIt.View[x, y];
						if (theChar == null)
							continue;

						var rx = theChar.Value % 32; // Character x,y
						var ry = theChar.Value / 32;

						srcRect.X = rx * 16;
						srcRect.Y = ry * 16 + fontYOffset;

						destRect.X = x * OneCharInEditorDimension + tileXOffset;
						destRect.Y = y * OneCharInEditorDimension;
						gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, wrapMode);
					}
				}

				gr.DrawLine(pen, i * 80, 0, i * 80, TileSetPictureHeight);

			}
		}
		pictureBoxTileSets.Refresh();
		var tileNr = TileSetOffset;
		labelTile0.Text = $@"Tile: {(++tileNr)}";
		labelTile1.Text = $@"Tile: {(++tileNr)}";
		labelTile2.Text = $@"Tile: {(++tileNr)}";
		labelTile3.Text = $@"Tile: {(++tileNr)}";
		labelTile4.Text = $@"Tile: {(++tileNr)}";
		labelTile5.Text = $@"Tile: {(++tileNr)}";
		labelTile6.Text = $@"Tile: {(++tileNr)}";
		labelTile7.Text = $@"Tile: {(++tileNr)}";
		labelTile8.Text = $@"Tile: {(++tileNr)}";
		labelTile9.Text = $@"Tile: {(++tileNr)}";
	}

	public void SwitchColorMode()
	{
		Redraw();
	}

	public void Redraw()
	{
		UpdateFontSelectorView();
		SimulateSafeLeftMouseButtonClickInFontSelectionArea();
		DrawTileSetView();
		NewTileSelected();
	}

	#region Font Nr selection has changed!
	public void UpdateFontSelectorView()
	{
		var src = new Rectangle(0, 0, 512, 128);
		if (MainForm.InColorMode)
		{
			src.Offset(0, 512);
		}
		src.Offset(0, FontNr * 128);

		// var colorOffset = InColorMode ? 512 : 0;
		var img = Helpers.GetImage(pictureBoxFontSelector);
		using (var gr = Graphics.FromImage(img))
		{
			// Copy font bank 1 or 2
			gr.DrawImage(AtariFontRenderer.BitmapFontBanks, 0, 0, src, GraphicsUnit.Pixel);
		}

		pictureBoxFontSelector.Refresh();
	}

	private void buttonPrevFontNr_Click(object sender, EventArgs e)
	{
		--FontNr;
		if (FontNr < 0) FontNr += 4;
		UpdateSelectedFontNr();
	}

	private void buttonNextFontNr_Click(object sender, EventArgs e)
	{
		FontNr = (FontNr + 1) % 4; // 0-3
		UpdateSelectedFontNr();
	}

	private void UpdateSelectedFontNr()
	{
		labelFontNr.Text = (FontNr + 1).ToString();
		UpdateFontSelectorView();
		SimulateSafeLeftMouseButtonClickInFontSelectionArea();
	}
	#endregion

	#region Mouse movement/clicks in the font selector picture
	private void FontSelector_MouseDown(object sender, MouseEventArgs e)
	{
		ActionFontSelectorMouseDown(e);
	}
	public void SimulateSafeLeftMouseButtonClickInFontSelectionArea()
	{
		var mouseEvent = new MouseEventArgs(MouseButtons.Left, 0, (SelectedCharacterIndex % 32) * 16, (SelectedCharacterIndex / 32) * 16, 0);
		ActionFontSelectorMouseDown(mouseEvent);
	}

	private void ActionFontSelectorMouseDown(MouseEventArgs e)
	{
		if (e.X < 0 || e.X >= 512 || e.Y < 0 || e.Y >= 128)
		{
			return;
		}

		var rx = e.X / 16;
		var ry = e.Y / 16;
		SelectedCharacterIndex = rx + ry * 32;

		pictureBoxFontSelectorRubberBand.Left = pictureBoxFontSelector.Bounds.Left + e.X - e.X % 16 - 2;
		pictureBoxFontSelectorRubberBand.Top = pictureBoxFontSelector.Bounds.Top + e.Y - e.Y % 16 - 2;

		labelEditCharInfo.Text = $"Font {(FontNr + 1)}\n${SelectedCharacterIndex:X2} #{SelectedCharacterIndex}";
		RedrawChar();
	}

	private void RedrawChar()
	{
		RenderCharIntoPictureBox(pictureBoxCurrenctChar);
	}

	public byte RenderCharIntoPictureBox(PictureBox target)
	{
		var theChar = (byte)(SelectedCharacterIndex % 256);

		var colorOffset = MainForm.InColorMode ? 512 : 0;
		var img = Helpers.GetImage(target);
		using (var gr = Graphics.FromImage(img))
		{
			var destRect = new Rectangle
			{
				X = 0,
				Y = 0,
				Width = 16,
				Height = 16,
			};

			var rx = SelectedCharacterIndex % 32; // Character x,y
			var ry = SelectedCharacterIndex / 32;

			var srcRect = new Rectangle
			{
				X = rx * 16,
				Y = ry * 16, // Will be set below
				Width = 16,
				Height = 16,
			};

			var fontYOffset = FontNr * 128 + colorOffset;
			srcRect.Y = ry * 16 + fontYOffset;

			gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect, GraphicsUnit.Pixel);

		}

		target.Refresh();

		return theChar;
	}
	#endregion

	#region Font per line selector

	/// <summary>
	/// Draw the font indicator next to the sample tile.
	/// Column of 5x 1-4 font indicators
	/// </summary>
	private static readonly string?[] ToDraw = ["?", "1", "2", "3", "4"];
	public void RedrawLineTypes()
	{
		using var pen = new Pen(FontMakerForm.BlackBrush);
		var img = Helpers.GetImage(pictureBoxCharacterSetSelector);
		using (var gr = Graphics.FromImage(img))
		{
			gr.FillRectangle(FontMakerForm.WhiteBrush, 0, 0, img.Width, img.Height);

			for (var a = 0; a < 5; a++)
			{
				gr.DrawString(ToDraw[TileSet.CurrentTile!.SelectedFont[a]], this.Font, FontMakerForm.BlackBrush, 3, a * OneCharInEditorDimension);
			}
		}

		pictureBoxCharacterSetSelector.Refresh();
	}

	private void pictureBoxCharacterSetSelector_MouseDown(object sender, MouseEventArgs e)
	{
		if (TileSet.CurrentTile == null)
			return;
		var ry = e.Y / OneCharInEditorDimension;

		if (ry is < 0 or >= 5)
		{
			// Note: This should not happen but when running on Mac things get funky
			return;
		}

		if (Control.ModifierKeys == Keys.Control)
		{
			// Setup to 1
			TileSet.CurrentTile.SelectedFont[ry] = 1;
		}
		else if (Control.ModifierKeys == Keys.Shift || e.Button == MouseButtons.Right)
		{
			// Cycle backwards
			--TileSet.CurrentTile.SelectedFont[ry];
			if (TileSet.CurrentTile.SelectedFont[ry] == 0)
				TileSet.CurrentTile.SelectedFont[ry] = 4;
		}
		else
		{
			// Cycle forward
			++TileSet.CurrentTile.SelectedFont[ry];
			if (TileSet.CurrentTile.SelectedFont[ry] == 5)
				TileSet.CurrentTile.SelectedFont[ry] = 1;
		}

		RedrawLineTypes();
		DrawCurrentTile();
		UpdateTileSetWindow();
	}
	#endregion

	#region Select a tile

	/// <summary>
	/// The TileSet picture box was clicked, and we have to select a new tile
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void pictureBoxTileSets_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.X < 0 || e.X >= 800 || e.Y < 0 || e.Y >= 80)
		{
			return;
		}

		var selectIndex = e.X / 80;

		var nextTileToSelect = selectIndex + TileSetOffset;
		if (nextTileToSelect == SelectedTileNr)
			return;

		SelectedTileNr = nextTileToSelect;
		NewTileSelected();
	}


	private void pictureBoxTileSets_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.X < 0 || e.X >= 800 || e.Y < 0 || e.Y >= 80)
		{
			return;
		}

		if (e.Button == MouseButtons.Left)
		{
			// Left button is held down, and we are moving the mouse
			var selectIndex = e.X / 80;

			var nextTileToSelect = selectIndex + TileSetOffset;
			if (nextTileToSelect == SelectedTileNr)
				return;
			SelectedTileNr = nextTileToSelect;
			NewTileSelected();
		}

	}

	#endregion



	private void buttonPrevTile_Click(object sender, EventArgs e)
	{
		if (SelectedTileNr > 0)
		{
			--SelectedTileNr;
			NewTileSelected();
		}
	}

	private void buttonNextTile_Click(object sender, EventArgs e)
	{
		if (SelectedTileNr < 99)
		{
			++SelectedTileNr;
			NewTileSelected();
		}
	}

	private void NewTileSelected()
	{
		TileSet.CurrentTile = TileSet.Tiles[SelectedTileNr];
		labelTileNr.Text = (SelectedTileNr + 1).ToString();

		RedrawLineTypes();
		DrawCurrentTile();
		UpdateTileSetWindow();
	}

	/// <summary>
	/// Draw the current tile's data into the picture box
	/// </summary>
	private void DrawCurrentTile()
	{
		if (TileSet.CurrentTile == null)
			return;
		var colorOffset = MainForm.InColorMode ? 512 : 0;

		using var pen = new Pen(LightGrayBrush);
		var img = Helpers.GetImage(pictureBoxEditTile);
		using var wrapMode = new ImageAttributes();
		using (var gr = Graphics.FromImage(img))
		{
			// Draw the tile background
			gr.FillRectangle(GrayBrush, 0, 0, CurrentTilePictureWidth, CurrentTilePictureHeight);

			var destRect = new Rectangle
			{
				Width = OneCharInEditorDimension,
				Height = OneCharInEditorDimension,
			};

			var srcRect = new Rectangle
			{
				X = 0,
				Y = 0, // Will be set below
				Width = 16,
				Height = 16,
			};

			for (var y = 0; y < TileData.TILE_HEIGHT; y++)
			{
				var fontYOffset = (TileSet.CurrentTile.SelectedFont[y] - 1) * 128 + colorOffset;

				for (var x = 0; x < TileData.TILE_WIDTH; x++)
				{
					var theChar = TileSet.CurrentTile.View[x, y];
					if (theChar == null)
						continue;

					var rx = theChar.Value % 32; // Character x,y
					var ry = theChar.Value / 32;

					srcRect.X = rx * 16;
					srcRect.Y = ry * 16 + fontYOffset;

					destRect.X = x * OneCharInEditorDimension;
					destRect.Y = y * OneCharInEditorDimension;
					gr.DrawImage(AtariFontRenderer.BitmapFontBanks, destRect, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			for (var i = 1; i < 5; ++i)
			{
				gr.DrawLine(pen, i * OneCharInEditorDimension, 0, i * OneCharInEditorDimension, CurrentTilePictureHeight);
				gr.DrawLine(pen, 0, i * OneCharInEditorDimension, CurrentTilePictureWidth, i * OneCharInEditorDimension);
			}
		}
		pictureBoxEditTile.Refresh();

	}

	/// <summary>
	/// Draw the current tile-set view.
	/// 10 tiles starting from 'TileSetOffset'
	/// </summary>
	private void UpdateTileSetWindow()
	{

		if (SelectedTileNr >= TileSetOffset && SelectedTileNr < TileSetOffset + 10)
		{
			// Current tile is in the visible range.
			DrawTileSetView();
		}

	}

	private void hScrollBarTiles_ValueChanged(object sender, EventArgs e)
	{
		TileSetOffset = hScrollBarTiles.Value;
		DrawTileSetView();
	}

	private void pictureBoxEditTile_MouseDown(object sender, MouseEventArgs e)
	{
		ActionEditorMouseDown(e);
	}

	private void pictureBoxEditTile_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.X is >= 0 and < CurrentTilePictureWidth &&
			e.Y is >= 0 and < CurrentTilePictureHeight &&
			(e.X / OneCharInEditorDimension != LastViewCharacterX || e.Y / OneCharInEditorDimension != LastViewCharacterY)
			&& (e.Button & (MouseButtons.Left | MouseButtons.Right)) > 0)
		{
			ActionEditorMouseDown(new MouseEventArgs(e.Button, 1, e.X, e.Y, -1000));      // -1000 indicates that this is a simulated event
		}
	}

	private void ActionEditorMouseDown(MouseEventArgs e)
	{
		if (e.X >= CurrentTilePictureWidth || e.Y >= CurrentTilePictureHeight || e.X < 0 || e.Y < 0)
		{
			return;
		}

		var rx = e.X / OneCharInEditorDimension;
		var ry = e.Y / OneCharInEditorDimension;

		if (rx >= TileData.TILE_WIDTH || ry >= TileData.TILE_HEIGHT)
			return;

		LastViewCharacterX = rx;
		LastViewCharacterY = ry;

		if (e.Button == MouseButtons.Left)
		{
			TileSet.CurrentTile!.View[rx, ry] = (byte)(SelectedCharacterIndex % 256);
		}
		else if (e.Button == MouseButtons.Right)
		{
			TileSet.CurrentTile!.View[rx, ry] = null;
		}
		DrawCurrentTile();
		UpdateTileSetWindow();
	}


	/// <summary>
	/// Clear the current tile
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void buttonTileClear_Click(object sender, EventArgs e)
	{
		if (TileSet.CurrentTile == null)
			return;

		for (var y = 0; y < TileData.TILE_HEIGHT; y++)
		{
			for (var x = 0; x < TileData.TILE_WIDTH; x++)
			{
				TileSet.CurrentTile!.View[x, y] = null;
			}

			TileSet.CurrentTile.SelectedFont[y] = 1;
		}
		DrawCurrentTile();
		UpdateTileSetWindow();
	}

	#region Copy/Paste tile data
	private void buttonTileCopy_Click(object sender, EventArgs e)
	{
		CopyCurrentTileToClipboard(false);
	}

	private void buttonTilePaste_Click(object sender, EventArgs e)
	{
		ActionPaste();
	}

	private void ActionPaste()
	{
		PasteToCurrentTileFromClipboard();
		DrawCurrentTile();
		UpdateTileSetWindow();
		RedrawLineTypes();
	}

	private void CopyCurrentTileToClipboard(bool makeActive)
	{
		if (TileSet.CurrentTile == null)
			return;

		var characterBytes = string.Empty;
		var fontBytes = string.Empty;
		var fontNr = string.Empty; // 1234
		var nulls = string.Empty;
		// Optimization:
		// if only part of the tile is used, we can save space by not copying the whole tile
		var minX = TileData.TILE_WIDTH;
		var minY = TileData.TILE_HEIGHT;
		var maxX = 0;
		var maxY = 0;

		for (var y = 0; y < TileData.TILE_HEIGHT; y++)
		{
			for (var x = 0; x < TileData.TILE_WIDTH; x++)
			{
				if (TileSet.CurrentTile.View[x, y] != null)
				{
					if (x < minX) minX = x;
					if (y < minY) minY = y;
					if (x > maxX) maxX = x;
					if (y > maxY) maxY = y;
				}
			}
		}

		if (maxX < minX || maxY < minY)
		{
			// Nothing to copy
			return;
		}

		var width = maxX - minX + 1;
		var height = maxY - minY + 1;

		for (var i = minY; i <= maxY; i++)
		{
			var whichFontNr = 1;
			for (var j = minX; j <= maxX; j++)
			{
				var thisChar = TileSet.CurrentTile.View[j, i];

				characterBytes += (thisChar == null ? "00" : $"{thisChar:X2}");

				var charInFont = ((thisChar ?? 0) % 128) * 8 + (TileSet.CurrentTile.SelectedFont[i] - 1) * 1024;
				whichFontNr = TileSet.CurrentTile.SelectedFont[i];
				nulls += thisChar == null ? '1' : '0';

				if (thisChar == null)
				{
					fontBytes += "0000000000000000";
				}
				else
				{
					for (var k = 0; k < 8; k++)
					{
						fontBytes += $"{AtariFont.FontBytes[charInFont + k]:X2}";
					}
				}
			}
			fontNr += whichFontNr;
		}

		var jo = new ClipboardJson()
		{
			Width = (width).ToString(),
			Height = (height).ToString(),
			Chars = characterBytes,
			Data = fontBytes,
			FontNr = fontNr,
			Nulls = nulls,
		};
		var json = jo.ToJson();
		MainForm.SafeSetClipboard(json);

		if (makeActive)
		{
			MainForm.SwitchToTileDrawing();
		}
	}

	private void PasteToCurrentTileFromClipboard()
	{
		if (TileSet.CurrentTile == null)
			return;

		int width;
		int height;
		string? characterBytes;
		string? fontNr;
		string? nulls;

		try
		{
			var jsonText = MainForm.SafeGetClipboard();
			var jsonObj = jsonText.FromJson<ClipboardJson?>();
			if (jsonObj == null)
				return;
			int.TryParse(jsonObj.Width, out width);
			int.TryParse(jsonObj.Height, out height);

			characterBytes = jsonObj.Chars;
			fontNr = jsonObj.FontNr;
			nulls = jsonObj.Nulls!;

			if (string.IsNullOrEmpty(characterBytes) || characterBytes.Length == 0)
			{
				MessageBox.Show(@"Clipboard data parsing error");
				return;
			}
		}
		catch (Exception)
		{
			MessageBox.Show(@"Clipboard data parsing error");
			return;
		}

		var charsBytes = Convert.FromHexString(characterBytes);
		for (var y = 0; y < height && y < TileData.TILE_HEIGHT; y++)
		{
			for (var x = 0; x < width && x < TileData.TILE_WIDTH; x++)
			{
				if (nulls[y * width + x] == '1')
					continue;
				TileSet.CurrentTile.View[x, y] = charsBytes[y * width + x];
			}

			// Set the fontNr
			try
			{
				var fontNrValue = int.Parse(fontNr[y].ToString());
				if (fontNrValue is > 0 and < 5)
				{
					TileSet.CurrentTile.SelectedFont[y] = (byte)fontNrValue;
				}
			}
			catch (Exception _)
			{
				// ignored
			}
		}
	}
	#endregion

	#region Load/Save/New tile-set
	private void buttonNewTileSet_Click(object sender, EventArgs e)
	{
		var re = MessageBox.Show(@"Are you sure you want to reset the current tile set? Everything will be lost!", Constants.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

		if (re == DialogResult.Yes)
		{
			TileSetOffset = 0;
			TileSet.Setup();
			Redraw();
		}
	}

	private void buttonLoadTileSet_Click(object sender, EventArgs e)
	{
		dialogOpenFile.FileName = string.Empty;
		dialogOpenFile.InitialDirectory = MainForm.CurrentDataFolder;
		dialogOpenFile.Filter = $@"Tile Set (*.atrset)|*.atrset";
		var ok = dialogOpenFile.ShowDialog();

		if (ok == DialogResult.OK)
		{
			var jsonText = File.ReadAllText(dialogOpenFile.FileName, Encoding.UTF8);
			var jsonObj = jsonText.FromJson<AtrTileSetJson>();

			if (jsonObj.Version != AtrTileSetJson.MY_VERSION)
			{
				MessageBox.Show($@"Failed to load tile-set from '{dialogOpenFile.FileName}'.");
				return;
			}

			TileSet.Setup();
			TileSetOffset = 0;
			if (jsonObj.Tiles?.Count > 0)
			{
				foreach (var tileData in jsonObj.Tiles)
				{
					TileSet.Load(tileData);
				}
			}

			Redraw();
		}
	}

	private void buttonSaveTileSet_Click(object sender, EventArgs e)
	{
		dialogSaveFile.FileName = string.Empty;
		dialogSaveFile.InitialDirectory = MainForm.CurrentDataFolder;
		dialogSaveFile.DefaultExt = "atrset";
		dialogSaveFile.Filter = @"Tile Set (*.atrset)|*.atrset";

		if (dialogSaveFile.ShowDialog() == DialogResult.OK)
		{
			var jo = new AtrTileSetJson();
			jo.Version = AtrTileSetJson.MY_VERSION;

			jo.Tiles = [];
			for (var index = 0; index < TileSet.Tiles.Length; index++)
			{
				var data = TileSet.Save(index);
				if (data != null)
				{
					jo.Tiles.Add(data);
				}
			}

			// Finished creating the object. Serialize it!
			var txt = jo.ToJson();
			File.WriteAllText(dialogSaveFile.FileName, txt, Encoding.UTF8);
		}
	}
	#endregion

	#region Load/Save one tile
	private void buttonLoadCurrentTile_Click(object sender, EventArgs e)
	{
		dialogOpenFile.FileName = string.Empty;
		dialogOpenFile.InitialDirectory = MainForm.CurrentDataFolder;
		dialogOpenFile.Filter = $@"Tile (*.atrtile)|*.atrtile";
		var ok = dialogOpenFile.ShowDialog();

		if (ok == DialogResult.OK)
		{
			var jsonText = File.ReadAllText(dialogOpenFile.FileName, Encoding.UTF8);
			var jsonObj = jsonText.FromJson<AtrTileJson>();

			if (jsonObj.Version != AtrTileJson.MY_VERSION)
			{
				MessageBox.Show($@"Failed to load tile from '{dialogOpenFile.FileName}'.");
				return;
			}

			TileSet.CurrentTile.Load(jsonObj.Tile);
			DrawCurrentTile();
			UpdateTileSetWindow();
		}
	}

	private void buttonSaveCurrentTile_Click(object sender, EventArgs e)
	{
		dialogSaveFile.FileName = string.Empty;
		dialogSaveFile.InitialDirectory = MainForm.CurrentDataFolder;
		dialogSaveFile.DefaultExt = "atrtile";
		dialogSaveFile.Filter = @"Tile (*.atrtile)|*.atrtile";

		if (dialogSaveFile.ShowDialog() == DialogResult.OK)
		{
			var jo = new AtrTileJson
			{
				Version = AtrTileJson.MY_VERSION,
				Tile = TileSet.CurrentTile.Save(0),
			};

			// Finished creating the object. Serialize it!
			var txt = jo.ToJson();
			File.WriteAllText(dialogSaveFile.FileName, txt, Encoding.UTF8);
		}
	}
	#endregion

	private void buttonRedraw_Click(object sender, EventArgs e)
	{
		Redraw();
	}

	private void TileSetEditorWindow_Activated(object sender, EventArgs e)
	{
		Redraw();
	}

	/// <summary>
	/// Use the current tile in the main window
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void buttonUse_Click(object sender, EventArgs e)
	{
		CopyCurrentTileToClipboard(true);
	}

	private void UpdateTileSetVisual()
	{
		DrawCurrentTile();
		UpdateTileSetWindow();
	}

	private void buttonRotateLeft_Click(object sender, EventArgs e)
	{
		TileSet.CurrentTile?.RotateLeft();
		UpdateTileSetVisual();
	}

	private void buttonRotateRight_Click(object sender, EventArgs e)
	{
		TileSet.CurrentTile?.RotateRight();
		UpdateTileSetVisual();
	}

	private void buttonMirrorH_Click(object sender, EventArgs e)
	{
		TileSet.CurrentTile?.MirrorHorizontal();
		UpdateTileSetVisual();
	}

	private void buttonMirrorV_Click(object sender, EventArgs e)
	{
		TileSet.CurrentTile?.MirrorVertical();
		UpdateTileSetVisual();
	}

	private void buttonShiftLeft_Click(object sender, EventArgs e)
	{
		TileSet.CurrentTile?.ShiftLeft();
		UpdateTileSetVisual();
	}

	private void ShiftRight_Click(object sender, EventArgs e)
	{
		TileSet.CurrentTile?.ShiftRight();
		UpdateTileSetVisual();
	}

	private void buttonShiftDown_Click(object sender, EventArgs e)
	{
		TileSet.CurrentTile?.ShiftDown();
		UpdateTileSetVisual();
	}

	private void buttonShiftUp_Click(object sender, EventArgs e)
	{
		TileSet.CurrentTile?.ShiftUp();
		UpdateTileSetVisual();
	}

	private void TileSetEditorWindow_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Control)
		{
			if (e.KeyCode == Keys.C)
			{
				CopyCurrentTileToClipboard(false);
				return;
			}

			// Ctrl + V = Paste from clipboard
			if (e.KeyCode == Keys.V)
			{
				ActionPaste();
				return;
			}
		}
	}
}
