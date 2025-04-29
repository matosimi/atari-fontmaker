namespace FontMaker;

/// <summary>
/// Every page contains an undo buffer.
/// The buffer is fed with AtariView data before something is changed.
/// We maintain a linked list of undo data, and a stack of redo data.
/// The LinkList is limited to 250 entries, after which the oldest get discarded.
/// </summary>
public class AtariViewUndoBuffer
{
	public const int UndoBufferSize = 250;

	private readonly LinkedList<AtariViewUndoInfo> _undoCommands = [];
	private readonly Stack<AtariViewUndoInfo> _redoCommands = new();

	/// <summary>
	/// Push the current AtariView into the undo buffer
	/// </summary>
	public void Push()
	{
		while(_undoCommands.Count >= UndoBufferSize)
		{
			_undoCommands.RemoveFirst();
		}
		_undoCommands.AddLast(AtariView.BuildForUndo());
		if (_redoCommands.Count > 0)
			_redoCommands.Clear();
	}

	public void Undo()
	{
		if (_undoCommands.Count > 0)
		{
			// Save the current screen
			_redoCommands.Push(AtariView.BuildForUndo());

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
		_undoCommands.AddLast(AtariView.BuildForUndo());

		// Get the last screen and restore it
		RestoreScreen(_redoCommands.Pop());
	}

	private static void RestoreScreen(AtariViewUndoInfo data)
	{
		if (data is { ViewBytes: not null })
		{
			AtariView.RestoreFromUndo(data);
		}
	}

	public (bool, bool) GetRedoUndoButtonState()
	{
		return (_undoCommands.Count > 0, _redoCommands.Count > 0);
	}
}