namespace FontMaker;

public class ViewUndoEntry
{
	public byte[,]? ViewBytes { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}

/// <summary>
/// Every page contains an undo buffer.
/// The buffer is fed with AtariView data before something is changed.
/// We maintain a linked list of undo data, and a stack of redo data.
/// The LinkList is limited to 250 entries, after which the oldest get discarded.
/// </summary>
public class AtariViewUndoBuffer
{
	public const int UndoBufferSize = 250;

	private readonly LinkedList<ViewUndoEntry> _undoCommands = [];
	private readonly Stack<ViewUndoEntry> _redoCommands = new();

	/// <summary>
	/// Push the current AtariView into the undo buffer
	/// </summary>
	public void Push()
	{
		while(_undoCommands.Count >= UndoBufferSize)
		{
			_undoCommands.RemoveFirst();
		}
		var newEntry = new ViewUndoEntry()
		{
			ViewBytes = AtariView.ViewBytes.Clone() as byte[,],
			Width = AtariView.Width,
			Height = AtariView.Height,
		};
		_undoCommands.AddLast(newEntry);
		if (_redoCommands.Count > 0)
			_redoCommands.Clear();
	}

	public void Undo()
	{
		if (_undoCommands.Count > 0)
		{
			var newEntry = new ViewUndoEntry()
			{
				ViewBytes = AtariView.ViewBytes.Clone() as byte[,],
				Width = AtariView.Width,
				Height = AtariView.Height,
			};
			// Save the current screen
			_redoCommands.Push(newEntry);

			// Get the last screen and restore it
			RestoreScreen(_undoCommands.Last());
			_undoCommands.RemoveLast();
		}
	}

	public void Redo()
	{
		if (_redoCommands.Count <= 0)
		{
			// Nothing to redo
			return;
		}

		// Save the current screen
		while (_undoCommands.Count >= UndoBufferSize)
		{
			_undoCommands.RemoveFirst();
		}
		var newEntry = new ViewUndoEntry()
		{
			ViewBytes = AtariView.ViewBytes.Clone() as byte[,],
			Width = AtariView.Width,
			Height = AtariView.Height,
		};
		_undoCommands.AddLast(newEntry);

		// Get the last screen and restore it
		RestoreScreen(_redoCommands.Pop());
	}

	private static void RestoreScreen(ViewUndoEntry data)
	{
		if (data is { ViewBytes: not null })
		{
			Buffer.BlockCopy(data.ViewBytes, 0, AtariView.ViewBytes, 0, data.ViewBytes.Length);
			AtariView.Width = data.Width;
			AtariView.Height = data.Height;
		}
	}

	public (bool, bool) GetRedoUndoButtonState()
	{
		return (_undoCommands.Count > 0, _redoCommands.Count > 0);
	}
}