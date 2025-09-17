#pragma warning disable WFO1000
using System.Text;

namespace FontMaker;

#region Load/Save tile / tile-set data
public class AtrTileSetJson
{
	public const string MY_VERSION = "1";

	public string? Version { get; set; }

	/// <summary>
	/// For each tile we have:
	/// - index where to store it
	/// - characters (5x5)
	/// - selected font
	/// </summary>
	public List<SavedTileData>? Tiles { get; set; }
}

public class AtrTileJson
{
	public const string MY_VERSION = "1";
	public string? Version { get; set; }
	public SavedTileData Tile { get; set; }
}

public class SavedTileData
{
	public int Nr { get; set; }
	public string View { get; set; }
	public string Font { get; set; }
	/// <summary>
	/// Indicate which spot is a null
	/// 1 = null
	/// 0 = data
	/// </summary>
	public string Nulls { get; set; }

	public int Width { get; set; } = 5;
	public int Height { get; set; } = 5;
}
#endregion

// A tile set is a collection of tiles that can be used to paint onto the view window.

public class TileData
{
	public const int OLD_TILE_WIDTH = 5;
	public const int OLD_TILE_HEIGHT = 5;

	public const int TILE_WIDTH = 8;
	public const int TILE_HEIGHT = 8;

	public bool IsValid()
	{
		for (var y = 0; y < TILE_HEIGHT; y++)
		{
			for (var x = 0; x < TILE_WIDTH; ++x)
			{
				if (View[x, y] != null)
					return true;
			}
		}

		return false;
	}

	/// <summary>
	/// The 5x5 byte? array that represents the characters (or null) of the tile
	/// </summary>
	public byte?[,] View { get; set; }	
	public byte[] SelectedFont { get; set;} = new byte[TILE_HEIGHT];        // 1,2,3,4

	private static readonly byte?[,] WorkBuffer = new byte?[TILE_WIDTH, TILE_HEIGHT];

	public TileData()
	{
		View = new byte?[TILE_WIDTH, TILE_HEIGHT];
		for (var a = 0; a < TILE_HEIGHT; a++)
		{
			SelectedFont[a] = 1;
		}
		for (var a = 0; a < TILE_WIDTH; a++)
		{
			for (var b = 0; b < TILE_HEIGHT; b++)
			{
				View[a, b] = null;
			}
		}
	}

	public TileData(byte?[,]? view, byte[] selectedFont)
	{
		if (view == null)
			View = new byte?[TILE_WIDTH, TILE_HEIGHT];
		else
			View = (view.Clone() as byte?[,])!;
		SelectedFont = (selectedFont.Clone() as byte[])!;
	}

	public TileData(TileData from) :
		this(from.View, from.SelectedFont)
	{
	}

	/// <summary>
	/// Generate a string representation of the tile data
	/// </summary>
	/// <returns></returns>
	public SavedTileData? Save(int tileNr)
	{
		var numNulls = 0;
		// Check if the tile is empty
		var sbView = new StringBuilder();
		var sbNulls = new StringBuilder();
		for (var y = 0; y < TILE_HEIGHT; y++)
		{
			for (var x = 0; x < TILE_WIDTH; x++)
			{
				var theChar = View[x, y];
				if (theChar == null)
				{
					sbNulls.Append('1');
					theChar = 0;
					++numNulls;
				}
				else
				{
					sbNulls.Append('0');
				}
				sbView.Append($"{theChar:X2}");
			}
		}

		if (numNulls == TILE_HEIGHT * TILE_WIDTH)
			return null;

		var data = new SavedTileData()
		{
			Nr = tileNr,
			View = sbView.ToString(),
			Font = Convert.ToHexString(SelectedFont),
			Nulls = sbNulls.ToString(),
			Width = TILE_WIDTH,
			Height = TILE_HEIGHT,
		};
		return data;
	}

	public bool Load(SavedTileData data)
	{
		var width = data.Width;
		var height = data.Height;

		if (width == 0) width = OLD_TILE_WIDTH;
		if (height == 0) height = OLD_TILE_HEIGHT;

		var viewData = Convert.FromHexString(data.View);		// Convert the HEX string into bytes and store in the View

		var index = 0;
		for (var y = 0; y < height; ++y)
		{
			for (var x = 0; x < width; ++x)
			{
				View[x, y] = data.Nulls[index] == '0' ? viewData[index] : null;
				++index;
			}
		}

		var selectedFont = Convert.FromHexString(data.Font);
		for (var i = 0; i < height; ++i)
		{
			SelectedFont[i] = selectedFont[i];
		}

		return true;
	}

	public void RotateRight()
	{
		Push();
		for (var y = 0; y < TILE_HEIGHT; ++y)
		{
			for (var x = 0; x < TILE_WIDTH; ++x)
			{
				WorkBuffer[x, y] = View[y, TILE_WIDTH - x - 1];
			}
		}
		for (var y = 0; y < TILE_HEIGHT; ++y)
		{
			for (var x = 0; x < TILE_WIDTH; ++x)
			{
				View[x, y] = WorkBuffer[x, y];
			}
		}
	}

	public void RotateLeft()
	{
		Push();
		for (var y = 0; y < TILE_HEIGHT; ++y)
		{
			for (var x = 0; x < TILE_WIDTH; ++x)
			{
				WorkBuffer[x, y] = View[TILE_HEIGHT - y - 1, x];
			}
		}
		for (var y = 0; y < TILE_HEIGHT; ++y)
		{
			for (var x = 0; x < TILE_WIDTH; ++x)
			{
				View[x, y] = WorkBuffer[x, y];
			}
		}
	}

	public void MirrorHorizontal()
	{
		Push();
		for (var y = 0; y < TILE_HEIGHT; ++y)
		{
			for (var x = 0; x < TILE_WIDTH / 2; ++x)
			{
				(View[x, y], View[TILE_WIDTH - x - 1, y]) = (View[TILE_WIDTH - x - 1, y], View[x, y]);
			}
		}
	}

	public void MirrorVertical()
	{
		Push();
		for (var x = 0; x < TILE_WIDTH; ++x)
		{
			for (var y = 0; y < TILE_HEIGHT / 2; ++y)
			{
				(View[x, y], View[x, TILE_HEIGHT - y - 1]) = (View[x, TILE_HEIGHT - y - 1], View[x, y]);
			}
		}
	}

	public void ShiftLeft()
	{
		Push();
		for (var y = 0; y < TILE_HEIGHT; ++y)
		{
			// Store the leftmost byte of the current row
			var leftmost = View[0, y];

			// Shift all elements in the row to the left
			for (var x = 0; x < TILE_WIDTH - 1; ++x)
			{
				View[x, y] = View[x + 1, y];
			}

			// Place the leftmost byte in the rightmost position
			View[TILE_WIDTH - 1, y] = leftmost;
		}
	}

	public void ShiftRight()
	{
		Push();
		for (var y = 0; y < TILE_HEIGHT; ++y)
		{
			// Store the rightmost byte of the current row
			var rightmost = View[TILE_WIDTH - 1, y];

			// Shift all elements in the row to the right
			for (var x = TILE_WIDTH - 1; x > 0; --x)
			{
				View[x, y] = View[x - 1, y];
			}

			// Place the rightmost byte in the leftmost position
			View[0, y] = rightmost;
		}
	}

	public void ShiftUp()
	{
		Push();
		for (var x = 0; x < TILE_WIDTH; ++x)
		{
			// Store the topmost byte of the current column
			var topmost = View[x, 0];

			// Shift all elements in the column up
			for (var y = 0; y < TILE_HEIGHT - 1; ++y)
			{
				View[x, y] = View[x, y + 1];
			}

			// Place the topmost byte in the bottommost position
			View[x, TILE_HEIGHT - 1] = topmost;
		}
	}

	public void ShiftDown()
	{
		Push();
		for (var x = 0; x < TILE_WIDTH; ++x)
		{
			// Store the bottommost byte of the current column
			var bottommost = View[x, TILE_HEIGHT - 1];

			// Shift all elements in the column down
			for (var y = TILE_HEIGHT - 1; y > 0; --y)
			{
				View[x, y] = View[x, y - 1];
			}

			// Place the bottommost byte in the topmost position
			View[x, 0] = bottommost;
		}
	}

	#region Undo/Redo

	public const int UndoBufferSize = 250;

	private readonly LinkedList<byte?[,]?> _undoCommands = new();
	private readonly Stack<byte?[,]?> _redoCommands = new();

	public void Push()
	{
		while (_undoCommands.Count >= UndoBufferSize)
		{
			_undoCommands.RemoveFirst();
		}
		_undoCommands.AddLast(View.Clone() as byte?[,]);
		if (_redoCommands.Count > 0)
			_redoCommands.Clear();
	}

	public void Undo()
	{
		if (_undoCommands.Count > 0)
		{
			// Save the current screen
			_redoCommands.Push(View.Clone() as byte?[,]);

			// Get the last screen and restore it
			var data = _undoCommands.Last();
			_undoCommands.RemoveLast();
			if (data != null)
				MyBlockCopy(data);
		}
	}

	public void Redo()
	{
		if (_redoCommands.Count > 0)
		{
			// Save the current screen
			while (_undoCommands.Count >= UndoBufferSize)
			{
				_undoCommands.RemoveFirst();
			}
			_undoCommands.AddLast(View.Clone() as byte?[,]);

			// Get the last screen and restore it
			var data = _redoCommands.Pop();
			if (data != null)
				MyBlockCopy(data);
		}
	}

	private void MyBlockCopy(byte?[,]? data)
	{
		if (data == null)
			return;
		for (var y = 0; y < TILE_HEIGHT; ++y)
		{
			for (var x = 0; x < TILE_WIDTH; ++x)
			{
				View[x, y] = data[x, y];
			}
		}
	}

	public (bool, bool) GetRedoUndoButtonState()
	{
		return (_undoCommands.Count > 0 ? true : false, _redoCommands.Count > 0 ? true : false);
	}

	#endregion
}


/// <summary>
/// All functions that help with TileSet management
/// </summary>
public static class TileSet
{
	public const int NUM_TILES_IN_SET = 256;

	public static TileData[] Tiles { get; set; } = new TileData[NUM_TILES_IN_SET];

	public static TileData? CurrentTile { get; set; }

	//public static TileData? CopiedTile { get; set; }

	public static void Setup()
	{
		for (var i = 0; i < Tiles.Length; i++)
		{
			Tiles[i] = new TileData();
		}

		CurrentTile = Tiles[0];
	}

	/// <summary>
	/// Save a specific tile's data to the 'SavedTileData' format
	/// </summary>
	/// <param name="tileNr"></param>
	/// <returns></returns>
	public static SavedTileData? Save(int tileNr)
	{
		return Tiles[tileNr].Save(tileNr);
	}

	/// <summary>
	/// Load a tile's data via the 'SavedTileData' format
	/// </summary>
	/// <param name="data"></param>
	/// <returns>true if the load was ok, false if there was an error</returns>
	public static bool Load(SavedTileData data)
	{
		try
		{
			return Tiles[data.Nr].Load(data);
		}
		catch (Exception _)
		{
			return false;
		}
	}
}