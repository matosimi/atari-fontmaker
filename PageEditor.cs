using System.Collections.ObjectModel;

namespace FontMaker
{
	public partial class PageEditor : Form
	{
		/// <summary>
		/// Build the new list of page data in the correct order
		/// </summary>
		/// <returns></returns>
		public List<PageData> GetPages()
		{
			var lst = new List<PageData>();
			for (var idx = 0; idx < PageOrder.Count; ++idx)
			{
				lst.Add(MyPages[PageOrder[idx]]);
			}

			return lst;
		}

		/// <summary>
		/// Original page data copy
		/// </summary>
		private List<PageData> MyPages { get; }

		/// <summary>
		/// What is the order the original pages will be arranged in.
		/// Uses the "Index" field of the page to set the order
		/// </summary>
		private ObservableCollection<int> PageOrder { get; }

		/// <summary>
		/// Index into Pages[] to indicate which one is being edited
		/// </summary>
		private int CurrentPageIndex { get; set; }

		/// <summary>
		/// List of data to be displayed in the ListBox (Index, Name)
		/// </summary>
		private List<dynamic> _dynList;

		private bool _inSetup;

		/// <summary>
		/// Create the PageEditor dialog.
		/// Makes a deep copy of all the page data.
		/// </summary>
		/// <param name="pages">List of current page data</param>
		/// <param name="currentPageIndex">Selected page index</param>
		public PageEditor(IEnumerable<PageData> pages, int currentPageIndex)
		{
			InitializeComponent();

			MyPages = new List<PageData>();
			foreach (var unit in pages)
			{
				MyPages.Add(new PageData(unit));
			}
			CurrentPageIndex = currentPageIndex;            // Which page is currently displayed
			PageOrder = new ObservableCollection<int>(MyPages.Select(pg => pg.Index).ToList());        // Index list (used to reorder the pages)
		}

		/// <summary>
		/// Populate the GUI
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PageEditor_Load(object sender, EventArgs e)
		{
			_inSetup = true;

			var page = MyPages[CurrentPageIndex];
			txtPageName.Text = page?.Name ?? "Unknown";

			// Build the list for the re-order list box
			RebuildListBox(CurrentPageIndex);

			_inSetup = false;
		}

		/// <summary>
		/// The page order list box is dynamically linked to the _dynList data.
		/// </summary>
		/// <param name="selectThisIndex">Which entry is selected</param>
		private void RebuildListBox(int selectThisIndex)
		{
			_inSetup = true;

			_dynList = new List<dynamic>();
			for (var idx = 0; idx < MyPages.Count; idx++)
			{
				var thisPage = MyPages[PageOrder[idx]];
				_dynList.Add(new { Index = thisPage.Index, Name = thisPage.Name, Page = thisPage });
			}

			lbPages.DataSource = null;
			lbPages.DataSource = _dynList;
			lbPages.DisplayMember = "Name";
			lbPages.ValueMember = "Index";

			lbPages.SetSelected(selectThisIndex, true);
			CurrentPageIndex = selectThisIndex;
			_inSetup = false;
		}

		/// <summary>
		/// Tell the caller that the editor is closed with OK
		/// </summary>
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			// Fill the data areas
			DialogResult = DialogResult.OK;
			Close();
		}

		/// <summary>
		/// Tell the caller that the editor is closed via cancel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		/// <summary>
		/// The name of a page was edited
		/// - save it in the page
		/// - validate the [Update] button state
		/// - rebuild the re-order list
		/// </summary>
		private void txtPageName_TextChanged(object sender, EventArgs e)
		{
			if (_inSetup) return;

			var page = MyPages[PageOrder[lbPages.SelectedIndex]];
			page.Name = txtPageName.Text;

			CheckValidState();

			RebuildListBox(lbPages.SelectedIndex);
		}

		/// <summary>
		/// Check that all pages have a name.
		/// If not then the [Update] button is disabled
		/// </summary>
		private void CheckValidState()
		{
			var valid = true;
			for (var i = 0; valid && i < MyPages.Count; i++)
			{
				if (MyPages[i].Name.Trim().Length == 0)
					valid = false;
			}

			btnUpdate.Enabled = valid;
		}

		/// <summary>
		/// Move the selected entry up
		/// </summary>
		private void btnUp_Click(object sender, EventArgs e)
		{
			MoveSelectedItem(lbPages, -1);
		}

		/// <summary>
		/// Move the selected entry down
		/// </summary>
		private void btnDown_Click(object sender, EventArgs e)
		{
			MoveSelectedItem(lbPages, 1);
		}

		/// <summary>
		/// Do the actual movement in the PageOrder list
		/// </summary>
		/// <param name="listBox">The list box that we look into</param>
		/// <param name="direction">-1 / +1</param>
		public void MoveSelectedItem(ListBox listBox, int direction)
		{

			// Checking selected item
			if (listBox.SelectedItem == null || listBox.SelectedIndex < 0)
				return; // No selected item - nothing to do

			// Calculate new index using move direction
			var newIndex = listBox.SelectedIndex + direction;

			// Checking bounds of the range
			if (newIndex < 0 || newIndex >= listBox.Items.Count)
				return; // Index out of range - nothing to do

			PageOrder.Move(listBox.SelectedIndex, newIndex);

			RebuildListBox(newIndex);
		}

		/// <summary>
		/// The reorder box selection has changed, so make the new page's name editable
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbPages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_inSetup) return;

			// A new page was selected
			// Map the new index to a page
			CurrentPageIndex = lbPages.SelectedIndex;

			var page = MyPages[PageOrder[CurrentPageIndex]];

			_inSetup = true;

			txtPageName.Text = page.Name;

			_inSetup = false;
		}
	}
}
