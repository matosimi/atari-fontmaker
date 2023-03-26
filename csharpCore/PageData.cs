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

	public partial class TMainForm
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
				var pageOne = new PageData(vw, chsline);
				Pages.Add(pageOne);

				CurrentPageIndex = 0;
			}

			InPagesSetup = true;
			cbPages.Items.Clear();
			cbPages.ResetText();

			foreach (var page in Pages)
			{
				var idx = cbPages.Items.Add(page.Name);
				page.Index = idx;
			}

			cbPages.SelectedIndex = 0;
			CurrentPageIndex = cbPages.SelectedIndex;

			InPagesSetup = false;

			UpdatePageDisplay();
		}

		/// <summary>
		/// Select a page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbPages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (InPagesSetup) return;

			SwopPage(saveCurrent:true);

			UpdatePageDisplay();
		}

		private void SwopPage(bool saveCurrent)
		{
			var nextPageIndex = cbPages.SelectedIndex;
			if (nextPageIndex == -1) return;

			if (saveCurrent)
			{
				// Save the current page data
				var currentPage = Pages[CurrentPageIndex];
				currentPage.Save(vw, chsline);
			}

			SwopPageAction(nextPageIndex);

		}

		private void SwopPageAction(int nextPageIndex)
		{
			// Select the next page and copy the data
			var page = Pages[nextPageIndex];
			vw = page.View.Clone() as byte[,];
			chsline = page.SelectedFont.Clone() as byte[];

			CurrentPageIndex = nextPageIndex;
		}

		private void UpdatePageDisplay()
		{
			lblCurrentPageIndex.Text = $"#{cbPages.SelectedIndex}";
			RedrawView();

			btnDeletePage.Enabled = Pages.Count > 1;
		}

		/// <summary>
		/// Add a new page, by duplicating the current page data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAddPage_Click(object sender, EventArgs e)
		{
			// Save the current page data
			var currentPage = Pages[CurrentPageIndex];
			currentPage.Save(vw, chsline);

			// Create a new page
			var nextNr = Pages.Max(page => page.Nr) + 1;
			var pageOne = new PageData(nextNr, $"Page {nextNr}", vw, chsline, -1);
			Pages.Add(pageOne);

			InPagesSetup = true;
			cbPages.Items.Clear();
			cbPages.ResetText();

			foreach (var page in Pages)
			{
				var idx = cbPages.Items.Add(page.Name);
				page.Index = idx;
			}

			cbPages.SelectedIndex = cbPages.Items.Count - 1;
			CurrentPageIndex = cbPages.SelectedIndex;

			InPagesSetup = false;
			UpdatePageDisplay();
		}

		private void btnDeletePage_Click(object sender, EventArgs e)
		{
			if (Pages.Count <= 1) return;
			var answer = MessageBox.Show("Are you sure you want to delete the page?", "Delete page", MessageBoxButtons.YesNo);
			if (answer == DialogResult.No) return;

			// Delete the page at the current index
			InPagesSetup = true;
			Pages.RemoveAt(CurrentPageIndex);

			// Rebuild the combo list
			cbPages.Items.Clear();
			cbPages.ResetText();

			foreach (var page in Pages)
			{
				var idx = cbPages.Items.Add(page.Name);
				page.Index = idx;
			}

			if (CurrentPageIndex >= cbPages.Items.Count - 1)
			{
				CurrentPageIndex = cbPages.Items.Count - 1;
			}

			cbPages.SelectedIndex = CurrentPageIndex;

			InPagesSetup = false;

			SwopPage(saveCurrent: false);

			UpdatePageDisplay();
		}

		private void btnEditPage_Click(object sender, EventArgs e)
		{
			// Save the current page data
			var currentPage = Pages[CurrentPageIndex];
			currentPage.Save(vw, chsline);

			// Show the editor
			var pe = new PageEditor(Pages, CurrentPageIndex);
			var action = pe.ShowDialog();
			if (action == DialogResult.OK)
			{
				// Save the data back and
				Pages = pe.GetPages();

				// Rebuild the combo list
				InPagesSetup = true;
				cbPages.Items.Clear();
				cbPages.ResetText();

				var toSelect = 0;

				foreach (var page in Pages)
				{
					var idx = cbPages.Items.Add(page.Name);
					page.Index = idx;
					if (currentPage.Nr == page.Nr)
						toSelect = idx;
				}

				cbPages.SelectedIndex = toSelect;
				SwopPage(false);

				InPagesSetup = false;

				UpdatePageDisplay();
			}


		}
	}
}
