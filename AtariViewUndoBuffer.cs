namespace FontMaker;

public class AtariViewUndoBuffer
{
	public const int UndoBufferSize = 250;

	private readonly LinkedList<byte[,]?> _undoCommands = new();
	private readonly Stack<byte[,]?> _redoCommands = new();

	public void Push()
	{
		while(_undoCommands.Count >= UndoBufferSize)
		{
			_undoCommands.RemoveFirst();
		}
		_undoCommands.AddLast(AtariView.ViewBytes.Clone() as byte[,]);
		if (_redoCommands.Count > 0)
			_redoCommands.Clear();
	}

	public void Undo()
	{
		if (_undoCommands.Count > 0)
		{
			// Save the current screen
			_redoCommands.Push(AtariView.ViewBytes.Clone() as byte[,]);

			// Get the last screen and restore it
			var data = _undoCommands.Last();
			_undoCommands.RemoveLast();
			if (data != null)
				Buffer.BlockCopy(data, 0, AtariView.ViewBytes, 0, data.Length);
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
			_undoCommands.AddLast(AtariView.ViewBytes.Clone() as byte[,]);

			// Get the last screen and restore it
			var data = _redoCommands.Pop();
			if (data != null)
				Buffer.BlockCopy(data, 0, AtariView.ViewBytes, 0, data.Length);
		}
	}

	public (bool, bool) GetRedoUndoButtonState()
	{
		return (_undoCommands.Count > 0 ? true : false, _redoCommands.Count > 0 ? true : false);
	}
}