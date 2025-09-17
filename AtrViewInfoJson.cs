namespace FontMaker;
/// <summary>
/// The data structure of the .atrview file
/// </summary>
public class AtrViewInfoJson
{
	public string? Version { get; set; }

	/// <summary>
	/// Mode 4/5 indicator
	/// 0 = mode 2 B & W
	/// 1 = mode 4 Color
	/// 2 = mode 5 Color
	/// 3 = mode 10 9 colors
	/// </summary>
	public string ColoredGfx { get; set; } = string.Empty;

	/// <summary>
	/// Characters in the view
	/// </summary>
	public string Chars { get; set; } = string.Empty;

	public int Width { get; set; } = 40;
	public int Height { get; set; } = 26;

	/// <summary>
	/// Use which font on which of the 26 lines of the view "1234"
	/// </summary>
	public string Lines { get; set; } = string.Empty;

	/// <summary>
	/// Hex encoded array of the 6 selected colors
	/// </summary>
	public string Colors { get; set; } = string.Empty;

	public string Fontname1 { get; set; } = string.Empty;
	public string Fontname2 { get; set; } = string.Empty;
	public string? Fontname3 { get; set; }
	public string? Fontname4 { get; set; }

	/// <summary>
	/// Hex encoded 1024 bytes per font
	/// </summary>
	public string Data { get; set; } = string.Empty;

	public string FortyBytes { get; set; } = string.Empty;

	/// <summary>
	/// For each page we have some data:
	/// - name
	/// - characters (40x26)
	/// - selected font
	/// </summary>
	public List<SavedPageData>? Pages { get; set; }

	/// <summary>
	/// For each tile we have:
	/// - index where to store it
	/// - characters (5x5)
	/// - selected font
	/// </summary>
	public List<SavedTileData>? Tiles { get; set; }
}
