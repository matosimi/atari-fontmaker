namespace FontMaker
{
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
			View = view.Clone() as byte[,];
			SelectedFont = selectedFont.Clone() as byte[];
			Index = index;
		}

		public PageData(PageData from) :
			this(from.Nr, from.Name, from.View, from.SelectedFont, from.Index)
		{
		}

		public void Save(byte[,] view, byte[] selectedFont)
		{
			View = view.Clone() as byte[,];
			SelectedFont = selectedFont.Clone() as byte[];
		}


		/// <summary>
		/// Unique nr of the page in the list of pages
		/// </summary>
		public int Nr { get; set; }

		public string Name { get; set; }


		public byte[,] View { get; set; }

		public byte[] SelectedFont { get; set; }

		public int Index { get; set; }
	}

	public class SavedPageData
	{
		public int Nr { get; set; }
		public string Name { get; set; }
		public string View { get; set; }
		public string SelectedFont { get; set; }
	}

	public partial class FontMakerForm
	{
		// Everything to do with pages goes here
		public List<PageData> Pages = new List<PageData>();

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
			Pages = new List<PageData>();
			CurrentPageIndex = -1;
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

			InPagesSetup = false;

			UpdatePageDisplay();
		}
	

		private void SwopPage(bool saveCurrent)
		{
			var nextPageIndex = comboBoxPages.SelectedIndex;
			if (nextPageIndex == -1) return;

			if (saveCurrent)
			{
				// Save the current page data
				var currentPage = Pages[CurrentPageIndex];
				currentPage.Save(AtariView.ViewBytes, AtariView.UseFontOnLine);
			}

			SwopPageAction(nextPageIndex);

		}

		private void SwopPageAction(int nextPageIndex)
		{
			// Select the next page and copy the data
			var page = Pages[nextPageIndex];
			AtariView.ViewBytes = page.View.Clone() as byte[,];
			AtariView.UseFontOnLine = page.SelectedFont.Clone() as byte[];

			CurrentPageIndex = nextPageIndex;
		}

		private void UpdatePageDisplay()
		{
			labelCurrentPageIndex.Text = $"#{comboBoxPages.SelectedIndex}";
			RedrawView();
			RedrawLineTypes();

			buttonDeletePage.Enabled = Pages.Count > 1;
		}




	}
}
