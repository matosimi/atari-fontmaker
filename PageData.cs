#pragma warning disable WFO1000
namespace FontMaker;

/// <summary>
/// Page data container
/// Unique name, view and font select data
/// </summary>
public class PageData
{
	public PageData(byte[,] view, byte[] selectedFont) :
		this(1, "Page 1", view, selectedFont, 0)
	{
	}

	public PageData(int nr, string name, byte[,] view, byte[] selectedFont, int index)
	{
		Nr = nr;
		Name = name;
		View = (view.Clone() as byte[,])!;
		SelectedFont = (selectedFont.Clone() as byte[])!;
		Index = index;

		UndoBuffer = new AtariViewUndoBuffer();
	}

	public PageData(SavedPageData? pageSrc, int index, int viewWidth)
	{
		// Init the page from saved json

		// Fix for an upgrade
		if (pageSrc.Width == 0)
			pageSrc.Width = 40;
		if (pageSrc.Height == 0)
			pageSrc.Height = 26;

		var bytes = Convert.FromHexString(pageSrc.View);
		var idx = 0;
		var viewData = new byte[pageSrc.Width, pageSrc.Height];

		for (var y = 0; y < pageSrc.Height; ++y)
		{
			for (var x = 0; x < viewWidth; ++x)
			{
				viewData[x, y] = bytes[idx];
				++idx;
			}
		}

		Nr = pageSrc.Nr;
		Name = pageSrc.Name;
		Width = pageSrc.Width;
		Height = pageSrc.Height;
		View = viewData;
		SelectedFont = Convert.FromHexString(pageSrc.SelectedFont);
		Index = index;

		UndoBuffer = new AtariViewUndoBuffer();
	}

	public PageData(PageData from) :
		this(from.Nr, from.Name, from.View, from.SelectedFont, from.Index)
	{
		Width = from.Width;
		Height = from.Height;
	}

	/// <summary>
	/// Save the current AtariView data to the page container
	/// </summary>
	/// <param name="view"></param>
	/// <param name="selectedFont"></param>
	public void Save()
	{
		View = AtariView.ViewBytes.Clone() as byte[,];
		SelectedFont = AtariView.UseFontOnLine.Clone() as byte[];
		Width = AtariView.Width;
		Height = AtariView.Height;
		OffsetX = AtariView.OffsetX;
		OffsetY = AtariView.OffsetY;
	}


	/// <summary>
	/// Unique nr of the page in the list of pages
	/// </summary>
	public int Nr { get; set; }

	public string Name { get; set; }

	public byte[,] View { get; set; }

	public byte[] SelectedFont { get; set; }

	public int Index { get; set; }

	public AtariViewUndoBuffer UndoBuffer { get; set; }

	public int Width { get; set; } = 40;
	public int Height { get; set; } = 26;
	public int OffsetX { get; set; } = 0;
	public int OffsetY { get; set; } = 0;
}

public class SavedPageData
{
	public int Nr { get; set; }
	public string Name { get; set; }
	public string View { get; set; }
	public string SelectedFont { get; set; }

	public int Width { get; set; } = 40;
	public int Height { get; set; } = 26;

	public SavedPageData(PageData? srcPage)
	{
		if (srcPage == null)
			return;

		Nr = srcPage.Nr;
		Name = srcPage.Name;
		SelectedFont = Convert.ToHexString(srcPage.SelectedFont);
		Width = srcPage.Width;
		Height = srcPage.Height;

		var pageView = string.Empty;
		for (var y = 0; y < srcPage.Height; y++)
		{
			for (var x = 0; x < srcPage.Width; x++)
			{
				pageView = pageView + $"{srcPage.View[x, y]:X2}";
			}
		}

		View = pageView;
	}
}

public partial class FontMakerForm
{
	// Everything to do with pages goes here
	public List<PageData> Pages { get; set; } = [];

	public PageData? CurrentPage { get; set; }

	/// <summary>
	/// The index in the combo box that is currently selected
	/// </summary>
	public int CurrentPageIndex { get; set; } = -1;

	public bool InPagesSetup { get; set; }

	/// <summary>
	/// Clear the old page data
	/// </summary>
	public void ClearPageList()
	{
		Pages = [];
		CurrentPageIndex = -1;
		CurrentPage = null;
	}

	/// <summary>
	/// After data has been loaded from file build the initial page data
	/// </summary>
	public void BuildPageList()
	{
		// If the page list is empty then create ONE default page
		if (Pages.Count == 0)
		{
			var pageOne = new PageData(AtariView.ViewBytes, AtariView.UseFontOnLine);
			Pages.Add(pageOne);

			CurrentPageIndex = 0;
		}

		InPagesSetup = true;
		comboBoxPages.Items.Clear();
		comboBoxPages.ResetText();

		foreach (var page in Pages)
		{
			var idx = comboBoxPages.Items.Add(page.Name);
			page.Index = idx;
		}

		comboBoxPages.SelectedIndex = 0;
		CurrentPageIndex = comboBoxPages.SelectedIndex;
		CurrentPage = Pages[CurrentPageIndex];

		InPagesSetup = false;

		UpdatePageDisplay();
		TransferPagesToViewActions();
	}

	public void SavePageSwitch(int nextPage)
	{
		if (nextPage < 0 || nextPage >= Pages.Count) return;
		comboBoxPages.SelectedIndex = nextPage;
	}
	private void SaveCurrentPage()
	{
		// Save the current page data
		var currentPage = Pages[CurrentPageIndex];
		currentPage.Save();
	}

	private void SwopPage(bool saveCurrent)
	{
		var nextPageIndex = comboBoxPages.SelectedIndex;
		if (nextPageIndex == -1) return;

		if (saveCurrent)
		{
			SaveCurrentPage();
		}

		SwopPageAction(nextPageIndex);
	}

	private void SwopToPage(int nextPageIndex)
	{
		SaveCurrentPage();
		SwopPageAction(nextPageIndex);
	}

	private void SwopPageAction(int nextPageIndex)
	{
		// Select the next page and copy the data
		var page = Pages[nextPageIndex];

		AtariView.ViewBytes = page.View.Clone() as byte[,];
		AtariView.UseFontOnLine = page.SelectedFont.Clone() as byte[];
		AtariView.Width = page.Width;
		AtariView.Height = page.Height;
		AtariView.OffsetX = page.OffsetX;
		AtariView.OffsetY = page.OffsetY;

		UpdateHVScrollBars(AtariView.OffsetX, AtariView.OffsetY);

		CurrentPageIndex = nextPageIndex;
		CurrentPage = Pages[CurrentPageIndex];
	}

	private void UpdatePageDisplay()
	{
		labelCurrentPageInfo.Text = $"{comboBoxPages.SelectedIndex+1} of {comboBoxPages.Items.Count}";
		RedrawView();
		RedrawLineTypes();

		buttonDeletePage.Enabled = Pages.Count > 1;

		UpdateViewUndoButtons();
	}
}