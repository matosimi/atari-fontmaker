﻿namespace FontMaker
{
	partial class PageEditor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(PageEditor));
			btnCancel = new Button();
			label1 = new Label();
			txtPageName = new TextBox();
			btnUpdate = new Button();
			lbPages = new ListBox();
			btnUp = new Button();
			btnDown = new Button();
			toolTips = new ToolTip(components);
			SuspendLayout();
			// 
			// btnCancel
			// 
			btnCancel.Location = new Point(152, 222);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(75, 23);
			btnCancel.TabIndex = 4;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.Click += btnCancel_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(5, 16);
			label1.Name = "label1";
			label1.Size = new Size(67, 13);
			label1.TabIndex = 1;
			label1.Text = "Page Name:";
			// 
			// txtPageName
			// 
			txtPageName.Location = new Point(76, 12);
			txtPageName.Name = "txtPageName";
			txtPageName.Size = new Size(180, 22);
			txtPageName.TabIndex = 1;
			toolTips.SetToolTip(txtPageName, "Edit the page name");
			txtPageName.TextChanged += txtPageName_TextChanged;
			txtPageName.Enter += txtPageName_Enter;
			txtPageName.KeyDown += txtPageName_KeyDown;
			txtPageName.Leave += txtPageName_Leave;
			// 
			// btnUpdate
			// 
			btnUpdate.Location = new Point(22, 222);
			btnUpdate.Name = "btnUpdate";
			btnUpdate.Size = new Size(75, 23);
			btnUpdate.TabIndex = 3;
			btnUpdate.Text = "Update";
			toolTips.SetToolTip(btnUpdate, "Save the current page settings");
			btnUpdate.UseVisualStyleBackColor = true;
			btnUpdate.Click += btnUpdate_Click;
			// 
			// lbPages
			// 
			lbPages.FormattingEnabled = true;
			lbPages.Location = new Point(5, 38);
			lbPages.Name = "lbPages";
			lbPages.Size = new Size(222, 173);
			lbPages.TabIndex = 2;
			lbPages.SelectedIndexChanged += lbPages_SelectedIndexChanged;
			lbPages.Enter += lbPages_Enter;
			lbPages.KeyDown += lbPages_KeyDown;
			lbPages.Leave += lbPages_Leave;
			// 
			// btnUp
			// 
			btnUp.Image = (Image)resources.GetObject("btnUp.Image");
			btnUp.Location = new Point(235, 74);
			btnUp.Name = "btnUp";
			btnUp.Size = new Size(21, 23);
			btnUp.TabIndex = 3;
			btnUp.TabStop = false;
			toolTips.SetToolTip(btnUp, "Move the page up in the sequence");
			btnUp.UseVisualStyleBackColor = true;
			btnUp.Click += btnUp_Click;
			// 
			// btnDown
			// 
			btnDown.Image = (Image)resources.GetObject("btnDown.Image");
			btnDown.Location = new Point(235, 103);
			btnDown.Name = "btnDown";
			btnDown.Size = new Size(21, 23);
			btnDown.TabIndex = 4;
			btnDown.TabStop = false;
			toolTips.SetToolTip(btnDown, "Move the page down in the sequence");
			btnDown.UseVisualStyleBackColor = true;
			btnDown.Click += btnDown_Click;
			// 
			// PageEditor
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = btnCancel;
			ClientSize = new Size(262, 252);
			Controls.Add(btnDown);
			Controls.Add(btnUp);
			Controls.Add(lbPages);
			Controls.Add(btnUpdate);
			Controls.Add(txtPageName);
			Controls.Add(label1);
			Controls.Add(btnCancel);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "PageEditor";
			Text = "Edit page names and order";
			Load += PageEditor_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button btnCancel;
		private Label label1;
		private TextBox txtPageName;
		private Button btnUpdate;
		private ListBox lbPages;
		private Button btnUp;
		private Button btnDown;
		private ToolTip toolTips;
	}
}