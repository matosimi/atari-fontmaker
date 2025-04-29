#pragma warning disable WFO1000
namespace FontMaker
{
	public partial class ViewActionsWindow : Form
	{
		private FontMakerForm? MainForm { get; set; }

		/// <summary>
		/// The index in the combo box that is currently selected
		/// </summary>
		public int CurrentPageIndex { get; set; } = -1;

		public Rectangle ActionArea { get; set; }

		private bool InPagesSetup { get; set; }

		#region Data from the outside
		public List<PageData>? Pages { get; set; } = null;
		#endregion

		private byte ReplaceThisChar { get; set; }
		private byte ReplaceWithThisChar { get; set; }
		private bool CanReplaceInArea { get; set; }

		private byte FillWithThisChar { get; set; }

		public ViewActionsWindow(FontMakerForm? mainForm)
		{
			MainForm = mainForm;
			InitializeComponent();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

		private void ViewActionsWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Hide();
			e.Cancel = true; // This cancels the close event
		}

		public void RebuildPagesDropDown(List<PageData>? pages, int selectIndex)
		{
			InPagesSetup = true;

			Pages = pages;
			// Clear the old list
			comboBoxPages.Items.Clear();
			comboBoxPages.ResetText();

			if (Pages is null || Pages.Count == 0)
				return;

			foreach (var page in Pages)
			{
				var idx = comboBoxPages.Items.Add(page.Name);
				page.Index = idx;
			}

			comboBoxPages.SelectedIndex = selectIndex; //comboBoxPages.Items.Count - 1;
			CurrentPageIndex = selectIndex;

			InPagesSetup = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="selectIndex"></param>
		public void SelectPage(int selectIndex)
		{
			InPagesSetup = true;

			comboBoxPages.SelectedIndex = selectIndex; //comboBoxPages.Items.Count - 1;
			CurrentPageIndex = selectIndex;

			InPagesSetup = false;
		}

		private void ViewActionsWindow_Pages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (InPagesSetup) return;

			MainForm?.PickPage(comboBoxPages.SelectedIndex);
		}

		private void pictureBoxX_Click(object sender, EventArgs e)
		{
			ReplaceThisChar = MainForm?.RenderCharIntoPictureBox(pictureBoxX) ?? 0;
			labelReplaceX.Text = $"#{ReplaceThisChar}";

			EnableReplaceCharButton();
		}

		private void pictureBoxY_Click(object sender, EventArgs e)
		{
			ReplaceWithThisChar = MainForm?.RenderCharIntoPictureBox(pictureBoxY) ?? 0;
			labelReplaceY.Text = $"#{ReplaceWithThisChar}";

			EnableReplaceCharButton();
		}
		private void checkFont_CheckedChanged(object sender, EventArgs e)
		{
			EnableReplaceCharButton();
		}

		private void EnableReplaceCharButton()
		{
			var isEnabled = ReplaceThisChar != ReplaceWithThisChar;
			var haveOneFont = checkFont1.Checked || checkFont2.Checked || checkFont3.Checked || checkFont4.Checked;

			buttonReplaceXwithYInView.Enabled = isEnabled && haveOneFont;
			buttonReplaceXwithYInArea.Enabled = CanReplaceInArea && isEnabled && haveOneFont;
		}
		private void ButtonReplaceXwithYInViewClick(object sender, EventArgs e)
		{
			MainForm?.ReplaceCharXWithY(ReplaceThisChar, ReplaceWithThisChar, checkFont1.Checked, checkFont2.Checked, checkFont3.Checked, checkFont4.Checked, new Rectangle(0, 0, AtariView.Width - 1, AtariView.Height - 1));
		}

		private void buttonReplaceXwithYInArea_Click(object sender, EventArgs e)
		{
			MainForm?.ReplaceCharXWithY(ReplaceThisChar, ReplaceWithThisChar, checkFont1.Checked, checkFont2.Checked, checkFont3.Checked, checkFont4.Checked, ActionArea);
		}

		#region External interface

		public void UpdateViewInformation(bool isMegaCopy, bool haveRegion, Rectangle region)
		{
			ActionArea = region;

			buttonAreaShiftLeft.Enabled = isMegaCopy && haveRegion;
			buttonAreaShiftRight.Enabled = isMegaCopy && haveRegion;
			buttonAreaShiftUp.Enabled = isMegaCopy && haveRegion;
			buttonAreaShiftDown.Enabled = isMegaCopy && haveRegion;
			buttonClearArea.Enabled = isMegaCopy && haveRegion;
			buttonFillArea.Enabled = isMegaCopy && haveRegion;

			CanReplaceInArea = isMegaCopy && haveRegion;

			if (isMegaCopy && haveRegion)
			{
				labelAreaInfo.Text = $"X:{region.Left} Y:{region.Top} W:{region.Width + 1} H:{region.Height + 1}";
			}
			else
			{
				labelAreaInfo.Text = string.Empty;
			}

			EnableReplaceCharButton();
		}
		#endregion

		private void buttonAreaShiftUp_Click(object sender, EventArgs e)
		{
			MainForm?.ActionAreaShift(comboBoxPages.SelectedIndex, FontMakerForm.DirectionFlags.Up, ActionArea);
		}
		private void buttonAreaShiftDown_Click(object sender, EventArgs e)
		{
			MainForm?.ActionAreaShift(comboBoxPages.SelectedIndex, FontMakerForm.DirectionFlags.Down, ActionArea);
		}
		private void buttonAreaShiftLeft_Click(object sender, EventArgs e)
		{
			MainForm?.ActionAreaShift(comboBoxPages.SelectedIndex, FontMakerForm.DirectionFlags.Left, ActionArea);
		}
		private void buttonAreaShiftRight_Click(object sender, EventArgs e)
		{
			MainForm?.ActionAreaShift(comboBoxPages.SelectedIndex, FontMakerForm.DirectionFlags.Right, ActionArea);
		}

		private void buttonViewShiftUp_Click(object sender, EventArgs e)
		{
			MainForm?.ActionAreaShift(comboBoxPages.SelectedIndex, FontMakerForm.DirectionFlags.Up, new Rectangle(0, 0, AtariView.Width - 1, AtariView.Height - 1));
		}
		private void buttonViewShiftDown_Click(object sender, EventArgs e)
		{
			MainForm?.ActionAreaShift(comboBoxPages.SelectedIndex, FontMakerForm.DirectionFlags.Down, new Rectangle(0, 0, AtariView.Width - 1, AtariView.Height - 1));
		}
		private void buttonViewShiftLeft_Click(object sender, EventArgs e)
		{
			MainForm?.ActionAreaShift(comboBoxPages.SelectedIndex, FontMakerForm.DirectionFlags.Left, new Rectangle(0, 0, AtariView.Width - 1, AtariView.Height - 1));
		}
		private void buttonViewShiftRight_Click(object sender, EventArgs e)
		{
			MainForm?.ActionAreaShift(comboBoxPages.SelectedIndex, FontMakerForm.DirectionFlags.Right, new Rectangle(0, 0, AtariView.Width - 1, AtariView.Height - 1));
		}

		private void buttonClearView_Click(object sender, EventArgs e)
		{
			MainForm?.FillArea(new Rectangle(0, 0, AtariView.Width-1, AtariView.Height-1), 0);
		}

		private void buttonClearArea_Click(object sender, EventArgs e)
		{
			MainForm?.FillArea(ActionArea, 0);
		}

		private void pictureBoxFill_Click(object sender, EventArgs e)
		{
			FillWithThisChar = MainForm?.RenderCharIntoPictureBox(pictureBoxFill) ?? 0;
			labelFiller.Text = $"#{FillWithThisChar}";

			EnableFillerButton();
		}
		private void EnableFillerButton()
		{
			var isEnabled = FillWithThisChar != ReplaceWithThisChar;
			var haveOneFont = checkFont1.Checked || checkFont2.Checked || checkFont3.Checked || checkFont4.Checked;

			buttonReplaceXwithYInView.Enabled = isEnabled && haveOneFont;
			buttonReplaceXwithYInArea.Enabled = CanReplaceInArea && isEnabled && haveOneFont;
		}

		private void buttonFillView_Click(object sender, EventArgs e)
		{
			MainForm?.FillArea(new Rectangle(0, 0, AtariView.Width - 1, AtariView.Height - 1), FillWithThisChar);
		}

		private void buttonFillArea_Click(object sender, EventArgs e)
		{
			MainForm?.FillArea(ActionArea, FillWithThisChar);
		}
	}
}
