namespace FontMaker
{
	partial class TMainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TMainForm));
			i_view = new PictureBox();
			I_fn = new PictureBox();
			ShapeDupes = new PictureBox();
			i_abo = new PictureBox();
			i_chset = new PictureBox();
			ImageMegacopy = new PictureBox();
			ImageMegaCopyV = new PictureBox();
			Label_char_view = new Label();
			p_xx = new Panel();
			Bevel3 = new Panel();
			i_ch = new PictureBox();
			Ic1 = new PictureBox();
			Ic2 = new PictureBox();
			b_shu = new Button();
			b_vmir = new Button();
			b_shr = new Button();
			b_shl = new Button();
			b_shd = new Button();
			b_ror = new Button();
			b_rol = new Button();
			b_ress = new Button();
			b_resd = new Button();
			b_inv = new Button();
			b_hmir = new Button();
			b_clr = new Button();
			b_cpy = new Button();
			b_pst = new Button();
			p_hh = new Panel();
			b_load1 = new Button();
			b_save1as = new Button();
			b_new = new Button();
			b_about = new Button();
			b_quit = new Button();
			b_save1 = new Button();
			b_save2 = new Button();
			b_load2 = new Button();
			b_save2as = new Button();
			p_zz = new Panel();
			Bevel4 = new Panel();
			i_col = new PictureBox();
			b_colorSwSetup = new Button();
			b_gfx = new Button();
			b_exportBMP = new Button();
			b_colorSwitch = new Button();
			b_lview = new Button();
			b_clrview = new Button();
			b_sview = new Button();
			p_status = new Panel();
			i_actcol = new PictureBox();
			l_char = new Label();
			l_col = new Label();
			cb_dupes = new CheckBox();
			SpeedButtonMegaCopy = new CheckBox();
			SpeedButtonUndo = new Button();
			SpeedButtonRedo = new Button();
			ComboBoxWriteMode = new ComboBox();
			p_color_switch = new Panel();
			i_rec1 = new PictureBox();
			i_rec2 = new PictureBox();
			lb_cs1 = new ListBox();
			lb_cs2 = new ListBox();
			ListBoxDebug = new ListBox();
			b_enterText = new Button();
			CheckBox40bytes = new CheckBox();
			d_open = new OpenFileDialog();
			d_save = new SaveFileDialog();
			Timer1 = new System.Windows.Forms.Timer(components);
			DebugTimer = new System.Windows.Forms.Timer(components);
			TimerDuplicates = new System.Windows.Forms.Timer(components);
			Shape1 = new PictureBox();
			Shape1v = new PictureBox();
			Shape2 = new PictureBox();
			Shape2v = new PictureBox();
			toolTips = new ToolTip(components);
			((System.ComponentModel.ISupportInitialize)i_view).BeginInit();
			((System.ComponentModel.ISupportInitialize)I_fn).BeginInit();
			I_fn.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ShapeDupes).BeginInit();
			((System.ComponentModel.ISupportInitialize)i_abo).BeginInit();
			((System.ComponentModel.ISupportInitialize)i_chset).BeginInit();
			((System.ComponentModel.ISupportInitialize)ImageMegacopy).BeginInit();
			((System.ComponentModel.ISupportInitialize)ImageMegaCopyV).BeginInit();
			p_xx.SuspendLayout();
			Bevel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)i_ch).BeginInit();
			((System.ComponentModel.ISupportInitialize)Ic1).BeginInit();
			((System.ComponentModel.ISupportInitialize)Ic2).BeginInit();
			p_hh.SuspendLayout();
			p_zz.SuspendLayout();
			Bevel4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)i_col).BeginInit();
			p_status.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)i_actcol).BeginInit();
			p_color_switch.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)i_rec1).BeginInit();
			((System.ComponentModel.ISupportInitialize)i_rec2).BeginInit();
			((System.ComponentModel.ISupportInitialize)Shape1).BeginInit();
			((System.ComponentModel.ISupportInitialize)Shape1v).BeginInit();
			((System.ComponentModel.ISupportInitialize)Shape2).BeginInit();
			((System.ComponentModel.ISupportInitialize)Shape2v).BeginInit();
			SuspendLayout();
			// 
			// i_view
			// 
			i_view.Location = new Point(536, 0);
			i_view.Name = "i_view";
			i_view.Size = new Size(640, 416);
			i_view.TabIndex = 0;
			i_view.TabStop = false;
			i_view.MouseDoubleClick += i_viewMouseDoubleClick;
			i_view.MouseDown += i_viewMouseDown;
			i_view.MouseMove += i_viewMouseMove;
			i_view.MouseUp += i_viewMouseUp;
			// 
			// I_fn
			// 
			I_fn.BorderStyle = BorderStyle.FixedSingle;
			I_fn.Controls.Add(ShapeDupes);
			I_fn.Location = new Point(2, 208);
			I_fn.Name = "I_fn";
			I_fn.Size = new Size(512, 256);
			I_fn.TabIndex = 1;
			I_fn.TabStop = false;
			I_fn.MouseDown += I_fnMouseDown;
			I_fn.MouseMove += I_fnMouseMove;
			I_fn.MouseUp += I_fnMouseUp;
			// 
			// ShapeDupes
			// 
			ShapeDupes.BackColor = Color.Transparent;
			ShapeDupes.Location = new Point(0, 0);
			ShapeDupes.Margin = new Padding(0);
			ShapeDupes.Name = "ShapeDupes";
			ShapeDupes.Size = new Size(20, 20);
			ShapeDupes.TabIndex = 11;
			ShapeDupes.TabStop = false;
			ShapeDupes.Visible = false;
			// 
			// i_abo
			// 
			i_abo.Image = (Image)resources.GetObject("i_abo.Image");
			i_abo.Location = new Point(536, 136);
			i_abo.Name = "i_abo";
			i_abo.Size = new Size(512, 128);
			i_abo.TabIndex = 2;
			i_abo.TabStop = false;
			i_abo.Visible = false;
			i_abo.MouseDown += i_aboMouseDown;
			// 
			// i_chset
			// 
			i_chset.Location = new Point(520, 0);
			i_chset.Name = "i_chset";
			i_chset.Size = new Size(15, 416);
			i_chset.TabIndex = 3;
			i_chset.TabStop = false;
			i_chset.MouseDown += i_chsetMouseDown;
			// 
			// ImageMegacopy
			// 
			ImageMegacopy.Location = new Point(26, 256);
			ImageMegacopy.Name = "ImageMegacopy";
			ImageMegacopy.Size = new Size(105, 105);
			ImageMegacopy.TabIndex = 4;
			ImageMegacopy.TabStop = false;
			ImageMegacopy.Visible = false;
			ImageMegacopy.MouseDown += ImageMegaCopyMouseDown;
			ImageMegacopy.MouseMove += ImageMegaCopyMouseMove;
			// 
			// ImageMegaCopyV
			// 
			ImageMegaCopyV.Location = new Point(560, 296);
			ImageMegaCopyV.Name = "ImageMegaCopyV";
			ImageMegaCopyV.Size = new Size(105, 105);
			ImageMegaCopyV.TabIndex = 5;
			ImageMegaCopyV.TabStop = false;
			ImageMegaCopyV.Visible = false;
			ImageMegaCopyV.MouseDown += ImageMegaCopyVMouseDown;
			ImageMegaCopyV.MouseMove += ImageMegaCopyVMouseMove;
			// 
			// Label_char_view
			// 
			Label_char_view.AutoSize = true;
			Label_char_view.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			Label_char_view.Location = new Point(525, 446);
			Label_char_view.Name = "Label_char_view";
			Label_char_view.Size = new Size(99, 13);
			Label_char_view.TabIndex = 6;
			Label_char_view.Text = "Char: Font1 $00 #0";
			// 
			// p_xx
			// 
			p_xx.BorderStyle = BorderStyle.FixedSingle;
			p_xx.Controls.Add(Bevel3);
			p_xx.Controls.Add(Ic1);
			p_xx.Controls.Add(Ic2);
			p_xx.Controls.Add(b_shu);
			p_xx.Controls.Add(b_vmir);
			p_xx.Controls.Add(b_shr);
			p_xx.Controls.Add(b_shl);
			p_xx.Controls.Add(b_shd);
			p_xx.Controls.Add(b_ror);
			p_xx.Controls.Add(b_rol);
			p_xx.Controls.Add(b_ress);
			p_xx.Controls.Add(b_resd);
			p_xx.Controls.Add(b_inv);
			p_xx.Controls.Add(b_hmir);
			p_xx.Controls.Add(b_clr);
			p_xx.Controls.Add(b_cpy);
			p_xx.Controls.Add(b_pst);
			p_xx.Location = new Point(113, 0);
			p_xx.Name = "p_xx";
			p_xx.Size = new Size(289, 177);
			p_xx.TabIndex = 1;
			// 
			// Bevel3
			// 
			Bevel3.Controls.Add(i_ch);
			Bevel3.Location = new Point(63, 7);
			Bevel3.Name = "Bevel3";
			Bevel3.Size = new Size(162, 162);
			Bevel3.TabIndex = 2;
			// 
			// i_ch
			// 
			i_ch.Location = new Point(0, 0);
			i_ch.Name = "i_ch";
			i_ch.Size = new Size(160, 160);
			i_ch.TabIndex = 3;
			i_ch.TabStop = false;
			i_ch.MouseDown += i_chMouseDown;
			i_ch.MouseMove += i_chMouseMove;
			i_ch.MouseUp += i_chMouseUp;
			// 
			// Ic1
			// 
			Ic1.Location = new Point(8, 8);
			Ic1.Name = "Ic1";
			Ic1.Size = new Size(49, 17);
			Ic1.TabIndex = 4;
			Ic1.TabStop = false;
			Ic1.Tag = 3;
			Ic1.MouseDown += Ic1MouseDown;
			// 
			// Ic2
			// 
			Ic2.Location = new Point(232, 8);
			Ic2.Name = "Ic2";
			Ic2.Size = new Size(49, 17);
			Ic2.TabIndex = 5;
			Ic2.TabStop = false;
			Ic2.Tag = 4;
			Ic2.MouseDown += Ic2MouseDown;
			// 
			// b_shu
			// 
			b_shu.Location = new Point(232, 91);
			b_shu.Margin = new Padding(0);
			b_shu.Name = "b_shu";
			b_shu.Size = new Size(49, 20);
			b_shu.TabIndex = 10;
			b_shu.Tag = 1;
			b_shu.Text = "SHU";
			toolTips.SetToolTip(b_shu, "Shift Up");
			b_shu.UseVisualStyleBackColor = true;
			b_shu.Click += b_shuClick;
			// 
			// b_vmir
			// 
			b_vmir.Location = new Point(232, 49);
			b_vmir.Margin = new Padding(0);
			b_vmir.Name = "b_vmir";
			b_vmir.Size = new Size(49, 20);
			b_vmir.TabIndex = 8;
			b_vmir.Tag = 1;
			b_vmir.Text = "V MIR";
			toolTips.SetToolTip(b_vmir, "Vertical Mirror (Shift+M)");
			b_vmir.UseVisualStyleBackColor = true;
			b_vmir.Click += Mirror_verticalExecute;
			// 
			// b_shr
			// 
			b_shr.Location = new Point(232, 70);
			b_shr.Margin = new Padding(0);
			b_shr.Name = "b_shr";
			b_shr.Size = new Size(49, 20);
			b_shr.TabIndex = 9;
			b_shr.Tag = 1;
			b_shr.Text = "SHR";
			toolTips.SetToolTip(b_shr, "Shift Right");
			b_shr.UseVisualStyleBackColor = true;
			b_shr.Click += b_shrClick;
			// 
			// b_shl
			// 
			b_shl.Location = new Point(8, 70);
			b_shl.Margin = new Padding(0);
			b_shl.Name = "b_shl";
			b_shl.Size = new Size(49, 20);
			b_shl.TabIndex = 2;
			b_shl.Tag = 1;
			b_shl.Text = "SHL";
			toolTips.SetToolTip(b_shl, "Shift Left");
			b_shl.UseVisualStyleBackColor = true;
			b_shl.Click += Shift_leftExecute;
			// 
			// b_shd
			// 
			b_shd.Location = new Point(8, 91);
			b_shd.Margin = new Padding(0);
			b_shd.Name = "b_shd";
			b_shd.Size = new Size(49, 20);
			b_shd.TabIndex = 3;
			b_shd.Tag = 1;
			b_shd.Text = "SHD";
			toolTips.SetToolTip(b_shd, "Shift Down");
			b_shd.UseVisualStyleBackColor = true;
			b_shd.Click += b_shdClick;
			// 
			// b_ror
			// 
			b_ror.Location = new Point(232, 28);
			b_ror.Margin = new Padding(0);
			b_ror.Name = "b_ror";
			b_ror.Size = new Size(49, 20);
			b_ror.TabIndex = 7;
			b_ror.Tag = 2;
			b_ror.Text = "ROR";
			toolTips.SetToolTip(b_ror, "Rotate Right (Shift+R)");
			b_ror.UseVisualStyleBackColor = true;
			b_ror.Click += Rotate_rightExecute;
			// 
			// b_rol
			// 
			b_rol.Location = new Point(8, 28);
			b_rol.Margin = new Padding(0);
			b_rol.Name = "b_rol";
			b_rol.Size = new Size(49, 20);
			b_rol.TabIndex = 0;
			b_rol.Tag = 2;
			b_rol.Text = "ROL";
			b_rol.TextAlign = ContentAlignment.TopCenter;
			toolTips.SetToolTip(b_rol, "Rotate Left (R)");
			b_rol.UseMnemonic = false;
			b_rol.UseVisualStyleBackColor = true;
			b_rol.Click += Rotate_leftExecute;
			// 
			// b_ress
			// 
			b_ress.Location = new Point(8, 133);
			b_ress.Margin = new Padding(0);
			b_ress.Name = "b_ress";
			b_ress.Size = new Size(49, 20);
			b_ress.TabIndex = 5;
			b_ress.Text = "RES S";
			toolTips.SetToolTip(b_ress, "Restore Last Saved");
			b_ress.UseVisualStyleBackColor = true;
			b_ress.Click += b_ressClick;
			// 
			// b_resd
			// 
			b_resd.Location = new Point(8, 112);
			b_resd.Margin = new Padding(0);
			b_resd.Name = "b_resd";
			b_resd.Size = new Size(49, 20);
			b_resd.TabIndex = 4;
			b_resd.Text = "RES D";
			toolTips.SetToolTip(b_resd, "Restore Default");
			b_resd.UseVisualStyleBackColor = true;
			b_resd.Click += b_resdClick;
			// 
			// b_inv
			// 
			b_inv.Location = new Point(232, 112);
			b_inv.Margin = new Padding(0);
			b_inv.Name = "b_inv";
			b_inv.Size = new Size(49, 20);
			b_inv.TabIndex = 11;
			b_inv.Tag = 2;
			b_inv.Text = "INV";
			toolTips.SetToolTip(b_inv, "Invert (i)");
			b_inv.UseVisualStyleBackColor = true;
			b_inv.Click += b_invClick;
			// 
			// b_hmir
			// 
			b_hmir.Location = new Point(8, 49);
			b_hmir.Margin = new Padding(0);
			b_hmir.Name = "b_hmir";
			b_hmir.Size = new Size(49, 20);
			b_hmir.TabIndex = 1;
			b_hmir.Tag = 1;
			b_hmir.Text = "H MIR";
			toolTips.SetToolTip(b_hmir, "Horizontal Mirror (M)");
			b_hmir.UseVisualStyleBackColor = true;
			b_hmir.Click += Mirror_horizontalExecute;
			// 
			// b_clr
			// 
			b_clr.Location = new Point(232, 133);
			b_clr.Margin = new Padding(0);
			b_clr.Name = "b_clr";
			b_clr.Size = new Size(49, 20);
			b_clr.TabIndex = 12;
			b_clr.Tag = 1;
			b_clr.Text = "CLR";
			toolTips.SetToolTip(b_clr, "Clear");
			b_clr.UseVisualStyleBackColor = true;
			b_clr.Click += b_clrClick;
			// 
			// b_cpy
			// 
			b_cpy.Location = new Point(8, 154);
			b_cpy.Name = "b_cpy";
			b_cpy.Size = new Size(49, 20);
			b_cpy.TabIndex = 6;
			b_cpy.Text = "CPY";
			toolTips.SetToolTip(b_cpy, "Copy To Clipboard");
			b_cpy.UseVisualStyleBackColor = true;
			b_cpy.Click += Clipboard_copyExecute;
			// 
			// b_pst
			// 
			b_pst.Location = new Point(232, 154);
			b_pst.Margin = new Padding(0);
			b_pst.Name = "b_pst";
			b_pst.Size = new Size(49, 20);
			b_pst.TabIndex = 13;
			b_pst.Text = "PST";
			toolTips.SetToolTip(b_pst, "Paste From Clipboard");
			b_pst.UseVisualStyleBackColor = true;
			b_pst.Click += b_pstClick;
			// 
			// p_hh
			// 
			p_hh.BorderStyle = BorderStyle.FixedSingle;
			p_hh.Controls.Add(b_load1);
			p_hh.Controls.Add(b_save1as);
			p_hh.Controls.Add(b_new);
			p_hh.Controls.Add(b_about);
			p_hh.Controls.Add(b_quit);
			p_hh.Controls.Add(b_save1);
			p_hh.Controls.Add(b_save2);
			p_hh.Controls.Add(b_load2);
			p_hh.Controls.Add(b_save2as);
			p_hh.Location = new Point(1, 0);
			p_hh.Name = "p_hh";
			p_hh.Size = new Size(105, 177);
			p_hh.TabIndex = 0;
			// 
			// b_load1
			// 
			b_load1.Location = new Point(3, 46);
			b_load1.Margin = new Padding(0);
			b_load1.Name = "b_load1";
			b_load1.Size = new Size(50, 20);
			b_load1.TabIndex = 1;
			b_load1.Text = "Load 1";
			b_load1.UseVisualStyleBackColor = true;
			b_load1.Click += b_load1Click;
			// 
			// b_save1as
			// 
			b_save1as.Location = new Point(3, 91);
			b_save1as.Margin = new Padding(0);
			b_save1as.Name = "b_save1as";
			b_save1as.Size = new Size(50, 20);
			b_save1as.TabIndex = 4;
			b_save1as.Text = "as...";
			b_save1as.UseVisualStyleBackColor = true;
			b_save1as.Click += b_save1asClick;
			// 
			// b_new
			// 
			b_new.Location = new Point(7, 8);
			b_new.Name = "b_new";
			b_new.Size = new Size(90, 20);
			b_new.TabIndex = 0;
			b_new.Text = "New";
			b_new.UseVisualStyleBackColor = true;
			b_new.Click += b_newClick;
			// 
			// b_about
			// 
			b_about.Location = new Point(8, 128);
			b_about.Name = "b_about";
			b_about.Size = new Size(89, 20);
			b_about.TabIndex = 5;
			b_about.Text = "About";
			b_about.UseVisualStyleBackColor = true;
			b_about.Click += b_aboutClick;
			// 
			// b_quit
			// 
			b_quit.Location = new Point(8, 152);
			b_quit.Name = "b_quit";
			b_quit.Size = new Size(89, 20);
			b_quit.TabIndex = 6;
			b_quit.Text = "Quit";
			b_quit.UseVisualStyleBackColor = true;
			b_quit.Click += b_quitClick;
			// 
			// b_save1
			// 
			b_save1.Location = new Point(3, 72);
			b_save1.Margin = new Padding(0);
			b_save1.Name = "b_save1";
			b_save1.Size = new Size(50, 20);
			b_save1.TabIndex = 2;
			b_save1.Text = "Save 1";
			b_save1.UseVisualStyleBackColor = true;
			b_save1.Click += b_save1Click;
			// 
			// b_save2
			// 
			b_save2.Location = new Point(52, 72);
			b_save2.Margin = new Padding(0);
			b_save2.Name = "b_save2";
			b_save2.Size = new Size(50, 20);
			b_save2.TabIndex = 3;
			b_save2.Text = "Save 2";
			b_save2.UseVisualStyleBackColor = true;
			b_save2.Click += b_save2Click;
			// 
			// b_load2
			// 
			b_load2.Location = new Point(52, 46);
			b_load2.Margin = new Padding(0);
			b_load2.Name = "b_load2";
			b_load2.Size = new Size(50, 20);
			b_load2.TabIndex = 7;
			b_load2.Text = "Load 2";
			b_load2.UseVisualStyleBackColor = true;
			b_load2.Click += b_load2Click;
			// 
			// b_save2as
			// 
			b_save2as.Location = new Point(52, 91);
			b_save2as.Margin = new Padding(0);
			b_save2as.Name = "b_save2as";
			b_save2as.Size = new Size(50, 20);
			b_save2as.TabIndex = 8;
			b_save2as.Text = "as...";
			b_save2as.UseVisualStyleBackColor = true;
			b_save2as.Click += b_save2asClick;
			// 
			// p_zz
			// 
			p_zz.BorderStyle = BorderStyle.FixedSingle;
			p_zz.Controls.Add(Bevel4);
			p_zz.Controls.Add(b_colorSwSetup);
			p_zz.Controls.Add(b_gfx);
			p_zz.Controls.Add(b_exportBMP);
			p_zz.Controls.Add(b_colorSwitch);
			p_zz.Location = new Point(409, 0);
			p_zz.Name = "p_zz";
			p_zz.Size = new Size(105, 177);
			p_zz.TabIndex = 2;
			// 
			// Bevel4
			// 
			Bevel4.BackgroundImageLayout = ImageLayout.None;
			Bevel4.BorderStyle = BorderStyle.FixedSingle;
			Bevel4.Controls.Add(i_col);
			Bevel4.Location = new Point(7, 39);
			Bevel4.Margin = new Padding(0);
			Bevel4.Name = "Bevel4";
			Bevel4.Size = new Size(92, 56);
			Bevel4.TabIndex = 0;
			// 
			// i_col
			// 
			i_col.Location = new Point(0, 0);
			i_col.Margin = new Padding(0);
			i_col.Name = "i_col";
			i_col.Size = new Size(90, 54);
			i_col.TabIndex = 1;
			i_col.TabStop = false;
			toolTips.SetToolTip(i_col, "Shift+Click to restore default");
			i_col.MouseDown += i_colMouseDown;
			// 
			// b_colorSwSetup
			// 
			b_colorSwSetup.Image = (Image)resources.GetObject("b_colorSwSetup.Image");
			b_colorSwSetup.Location = new Point(72, 114);
			b_colorSwSetup.Name = "b_colorSwSetup";
			b_colorSwSetup.Size = new Size(22, 22);
			b_colorSwSetup.TabIndex = 2;
			b_colorSwSetup.Click += b_colorSwSetupClick;
			// 
			// b_gfx
			// 
			b_gfx.Location = new Point(8, 8);
			b_gfx.Name = "b_gfx";
			b_gfx.Size = new Size(89, 25);
			b_gfx.TabIndex = 0;
			b_gfx.Text = "Change GFX";
			toolTips.SetToolTip(b_gfx, "Mode 2 / 4");
			b_gfx.UseVisualStyleBackColor = true;
			b_gfx.Click += b_gfxClick;
			// 
			// b_exportBMP
			// 
			b_exportBMP.Location = new Point(8, 144);
			b_exportBMP.Name = "b_exportBMP";
			b_exportBMP.Size = new Size(89, 25);
			b_exportBMP.TabIndex = 1;
			b_exportBMP.Text = "Export font";
			b_exportBMP.UseVisualStyleBackColor = true;
			b_exportBMP.Click += b_exportBMPClick;
			// 
			// b_colorSwitch
			// 
			b_colorSwitch.Location = new Point(8, 112);
			b_colorSwitch.Name = "b_colorSwitch";
			b_colorSwitch.Size = new Size(64, 25);
			b_colorSwitch.TabIndex = 2;
			b_colorSwitch.Text = "Recolor";
			b_colorSwitch.UseVisualStyleBackColor = true;
			b_colorSwitch.Click += b_colorSwitchClick;
			// 
			// b_lview
			// 
			b_lview.Location = new Point(864, 416);
			b_lview.Name = "b_lview";
			b_lview.Size = new Size(89, 25);
			b_lview.TabIndex = 3;
			b_lview.Text = "Load View";
			b_lview.UseVisualStyleBackColor = true;
			b_lview.Click += b_lviewClick;
			// 
			// b_clrview
			// 
			b_clrview.Location = new Point(768, 416);
			b_clrview.Name = "b_clrview";
			b_clrview.Size = new Size(89, 25);
			b_clrview.TabIndex = 4;
			b_clrview.Text = "Clear View";
			b_clrview.UseVisualStyleBackColor = true;
			b_clrview.Click += b_clrviewClick;
			// 
			// b_sview
			// 
			b_sview.Location = new Point(960, 416);
			b_sview.Name = "b_sview";
			b_sview.Size = new Size(89, 25);
			b_sview.TabIndex = 5;
			b_sview.Text = "Save View";
			b_sview.UseVisualStyleBackColor = true;
			b_sview.Click += b_sviewClick;
			// 
			// p_status
			// 
			p_status.Controls.Add(i_actcol);
			p_status.Controls.Add(l_char);
			p_status.Controls.Add(l_col);
			p_status.Controls.Add(cb_dupes);
			p_status.Controls.Add(SpeedButtonMegaCopy);
			p_status.Controls.Add(SpeedButtonUndo);
			p_status.Controls.Add(SpeedButtonRedo);
			p_status.Controls.Add(ComboBoxWriteMode);
			p_status.Location = new Point(-1, 179);
			p_status.Name = "p_status";
			p_status.Size = new Size(515, 25);
			p_status.TabIndex = 6;
			// 
			// i_actcol
			// 
			i_actcol.Location = new Point(232, 6);
			i_actcol.Name = "i_actcol";
			i_actcol.Size = new Size(49, 17);
			i_actcol.TabIndex = 0;
			i_actcol.TabStop = false;
			// 
			// l_char
			// 
			l_char.AutoSize = true;
			l_char.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			l_char.Location = new Point(8, 7);
			l_char.Name = "l_char";
			l_char.Size = new Size(102, 13);
			l_char.TabIndex = 1;
			l_char.Text = "Char: Font 1 $00 #0";
			// 
			// l_col
			// 
			l_col.AutoSize = true;
			l_col.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
			l_col.Location = new Point(199, 7);
			l_col.Name = "l_col";
			l_col.Size = new Size(34, 13);
			l_col.TabIndex = 2;
			l_col.Text = "Color:";
			// 
			// cb_dupes
			// 
			cb_dupes.AutoSize = true;
			cb_dupes.Location = new Point(366, 5);
			cb_dupes.Name = "cb_dupes";
			cb_dupes.Size = new Size(49, 17);
			cb_dupes.TabIndex = 11;
			cb_dupes.Text = "DUP";
			toolTips.SetToolTip(cb_dupes, "Show duplicate characters within font");
			cb_dupes.UseVisualStyleBackColor = true;
			cb_dupes.Click += cb_dupesClick;
			// 
			// SpeedButtonMegaCopy
			// 
			SpeedButtonMegaCopy.Appearance = Appearance.Button;
			SpeedButtonMegaCopy.CheckAlign = ContentAlignment.MiddleCenter;
			SpeedButtonMegaCopy.Location = new Point(426, 3);
			SpeedButtonMegaCopy.Name = "SpeedButtonMegaCopy";
			SpeedButtonMegaCopy.Size = new Size(89, 21);
			SpeedButtonMegaCopy.TabIndex = 3;
			SpeedButtonMegaCopy.Text = "Mega Copy";
			SpeedButtonMegaCopy.TextAlign = ContentAlignment.MiddleCenter;
			toolTips.SetToolTip(SpeedButtonMegaCopy, "Toggle MegaCopy Mode");
			SpeedButtonMegaCopy.Click += SpeedButtonMegaCopyClick;
			// 
			// SpeedButtonUndo
			// 
			SpeedButtonUndo.Image = (Image)resources.GetObject("SpeedButtonUndo.Image");
			SpeedButtonUndo.Location = new Point(298, 2);
			SpeedButtonUndo.Name = "SpeedButtonUndo";
			SpeedButtonUndo.Size = new Size(22, 22);
			SpeedButtonUndo.TabIndex = 4;
			toolTips.SetToolTip(SpeedButtonUndo, "Undo Font Change");
			SpeedButtonUndo.Click += Undo_FontExecute;
			// 
			// SpeedButtonRedo
			// 
			SpeedButtonRedo.Image = (Image)resources.GetObject("SpeedButtonRedo.Image");
			SpeedButtonRedo.Location = new Point(326, 2);
			SpeedButtonRedo.Name = "SpeedButtonRedo";
			SpeedButtonRedo.Size = new Size(22, 22);
			SpeedButtonRedo.TabIndex = 5;
			toolTips.SetToolTip(SpeedButtonRedo, "Redo Font Change");
			SpeedButtonRedo.Click += Redo_FontExecute;
			// 
			// ComboBoxWriteMode
			// 
			ComboBoxWriteMode.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxWriteMode.FormattingEnabled = true;
			ComboBoxWriteMode.Items.AddRange(new object[] { "Rewrite", "Insert" });
			ComboBoxWriteMode.Location = new Point(122, 3);
			ComboBoxWriteMode.Name = "ComboBoxWriteMode";
			ComboBoxWriteMode.Size = new Size(66, 21);
			ComboBoxWriteMode.TabIndex = 0;
			// 
			// p_color_switch
			// 
			p_color_switch.Controls.Add(i_rec1);
			p_color_switch.Controls.Add(i_rec2);
			p_color_switch.Controls.Add(lb_cs1);
			p_color_switch.Controls.Add(lb_cs2);
			p_color_switch.Location = new Point(544, 8);
			p_color_switch.Name = "p_color_switch";
			p_color_switch.Size = new Size(153, 97);
			p_color_switch.TabIndex = 7;
			p_color_switch.Visible = false;
			// 
			// i_rec1
			// 
			i_rec1.Location = new Point(16, 77);
			i_rec1.Name = "i_rec1";
			i_rec1.Size = new Size(49, 17);
			i_rec1.TabIndex = 0;
			i_rec1.TabStop = false;
			i_rec1.Tag = 4;
			// 
			// i_rec2
			// 
			i_rec2.Location = new Point(88, 77);
			i_rec2.Name = "i_rec2";
			i_rec2.Size = new Size(49, 17);
			i_rec2.TabIndex = 1;
			i_rec2.TabStop = false;
			i_rec2.Tag = 4;
			// 
			// lb_cs1
			// 
			lb_cs1.FormattingEnabled = true;
			lb_cs1.Location = new Point(8, 8);
			lb_cs1.Name = "lb_cs1";
			lb_cs1.Size = new Size(65, 56);
			lb_cs1.TabIndex = 0;
			lb_cs1.Click += lb_cs1Click;
			// 
			// lb_cs2
			// 
			lb_cs2.FormattingEnabled = true;
			lb_cs2.Location = new Point(80, 8);
			lb_cs2.Name = "lb_cs2";
			lb_cs2.Size = new Size(65, 56);
			lb_cs2.TabIndex = 1;
			lb_cs2.Click += lb_cs2Click;
			// 
			// ListBoxDebug
			// 
			ListBoxDebug.FormattingEnabled = true;
			ListBoxDebug.Items.AddRange(new object[] { "todo:" });
			ListBoxDebug.Location = new Point(624, 128);
			ListBoxDebug.Name = "ListBoxDebug";
			ListBoxDebug.Size = new Size(219, 186);
			ListBoxDebug.TabIndex = 8;
			// 
			// b_enterText
			// 
			b_enterText.Enabled = false;
			b_enterText.Location = new Point(520, 416);
			b_enterText.Name = "b_enterText";
			b_enterText.Size = new Size(89, 25);
			b_enterText.TabIndex = 9;
			b_enterText.Text = "Enter text";
			toolTips.SetToolTip(b_enterText, "Text to clipboard, hold SHIFT while clicking to inverse");
			b_enterText.UseVisualStyleBackColor = true;
			b_enterText.MouseDown += b_enterTextMouseDown;
			// 
			// CheckBox40bytes
			// 
			CheckBox40bytes.AutoSize = true;
			CheckBox40bytes.Location = new Point(696, 422);
			CheckBox40bytes.Name = "CheckBox40bytes";
			CheckBox40bytes.Size = new Size(67, 17);
			CheckBox40bytes.TabIndex = 10;
			CheckBox40bytes.Text = "40 Bytes";
			toolTips.SetToolTip(CheckBox40bytes, "Switch between 32 and 40 byte screen width");
			CheckBox40bytes.UseVisualStyleBackColor = true;
			CheckBox40bytes.Click += CheckBox40bytesClick;
			// 
			// d_open
			// 
			d_open.FileName = "d_open";
			// 
			// d_save
			// 
			d_save.FileName = "d_save";
			// 
			// Timer1
			// 
			Timer1.Enabled = true;
			Timer1.Interval = 5000;
			Timer1.Tick += Timer1Timer;
			// 
			// DebugTimer
			// 
			DebugTimer.Interval = 10;
			DebugTimer.Tick += DebugTimerTimer;
			// 
			// TimerDuplicates
			// 
			TimerDuplicates.Tick += TimerDuplicatesTimer;
			// 
			// Shape1
			// 
			Shape1.BackColor = Color.Transparent;
			Shape1.Location = new Point(167, 296);
			Shape1.Margin = new Padding(0);
			Shape1.Name = "Shape1";
			Shape1.Size = new Size(20, 20);
			Shape1.TabIndex = 12;
			Shape1.TabStop = false;
			Shape1.MouseDown += Shape1MouseDown;
			Shape1.MouseMove += Shape1MouseMove;
			Shape1.MouseUp += Shape1MouseUp;
			Shape1.Resize += Shape1_Resize;
			// 
			// Shape1v
			// 
			Shape1v.BackColor = Color.Transparent;
			Shape1v.BorderStyle = BorderStyle.FixedSingle;
			Shape1v.Location = new Point(198, 296);
			Shape1v.Margin = new Padding(0);
			Shape1v.Name = "Shape1v";
			Shape1v.Size = new Size(20, 20);
			Shape1v.TabIndex = 13;
			Shape1v.TabStop = false;
			Shape1v.Visible = false;
			Shape1v.MouseDown += Shape1vMouseDown;
			Shape1v.MouseMove += Shape1vMouseMove;
			Shape1v.MouseUp += Shape1vMouseUp;
			Shape1v.Resize += Shape1v_Resize;
			// 
			// Shape2
			// 
			Shape2.BackColor = Color.Transparent;
			Shape2.BorderStyle = BorderStyle.FixedSingle;
			Shape2.Location = new Point(167, 329);
			Shape2.Margin = new Padding(0);
			Shape2.Name = "Shape2";
			Shape2.Size = new Size(20, 20);
			Shape2.TabIndex = 14;
			Shape2.TabStop = false;
			Shape2.Visible = false;
			Shape2.MouseDown += Shape2MouseDown;
			Shape2.MouseLeave += Shape1MouseLeave;
			Shape2.MouseMove += Shape2MouseMove;
			Shape2.MouseUp += Shape2MouseUp;
			// 
			// Shape2v
			// 
			Shape2v.BackColor = Color.Transparent;
			Shape2v.BorderStyle = BorderStyle.FixedSingle;
			Shape2v.Location = new Point(198, 329);
			Shape2v.Margin = new Padding(0);
			Shape2v.Name = "Shape2v";
			Shape2v.Size = new Size(20, 20);
			Shape2v.TabIndex = 15;
			Shape2v.TabStop = false;
			Shape2v.Visible = false;
			Shape2v.MouseDown += Shape2vMouseDown;
			Shape2v.MouseLeave += Shape2vMouseLeave;
			Shape2v.MouseMove += Shape2vMouseMove;
			Shape2v.MouseUp += Shape2vMouseUp;
			// 
			// TMainForm
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			BackgroundImageLayout = ImageLayout.None;
			ClientSize = new Size(1048, 465);
			Controls.Add(i_abo);
			Controls.Add(p_color_switch);
			Controls.Add(ImageMegacopy);
			Controls.Add(ImageMegaCopyV);
			Controls.Add(Shape2);
			Controls.Add(Shape2v);
			Controls.Add(Shape1v);
			Controls.Add(Shape1);
			Controls.Add(i_view);
			Controls.Add(I_fn);
			Controls.Add(i_chset);
			Controls.Add(Label_char_view);
			Controls.Add(p_xx);
			Controls.Add(p_hh);
			Controls.Add(p_zz);
			Controls.Add(b_lview);
			Controls.Add(b_clrview);
			Controls.Add(b_sview);
			Controls.Add(p_status);
			Controls.Add(ListBoxDebug);
			Controls.Add(b_enterText);
			Controls.Add(CheckBox40bytes);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = (Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			Name = "TMainForm";
			Text = "MainForm";
			FormClosing += FormCloseQuery;
			KeyDown += TMainForm_KeyDown;
			MouseWheel += FormMouseWheel;
			((System.ComponentModel.ISupportInitialize)i_view).EndInit();
			((System.ComponentModel.ISupportInitialize)I_fn).EndInit();
			I_fn.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)ShapeDupes).EndInit();
			((System.ComponentModel.ISupportInitialize)i_abo).EndInit();
			((System.ComponentModel.ISupportInitialize)i_chset).EndInit();
			((System.ComponentModel.ISupportInitialize)ImageMegacopy).EndInit();
			((System.ComponentModel.ISupportInitialize)ImageMegaCopyV).EndInit();
			p_xx.ResumeLayout(false);
			Bevel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)i_ch).EndInit();
			((System.ComponentModel.ISupportInitialize)Ic1).EndInit();
			((System.ComponentModel.ISupportInitialize)Ic2).EndInit();
			p_hh.ResumeLayout(false);
			p_zz.ResumeLayout(false);
			Bevel4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)i_col).EndInit();
			p_status.ResumeLayout(false);
			p_status.PerformLayout();
			((System.ComponentModel.ISupportInitialize)i_actcol).EndInit();
			p_color_switch.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)i_rec1).EndInit();
			((System.ComponentModel.ISupportInitialize)i_rec2).EndInit();
			((System.ComponentModel.ISupportInitialize)Shape1).EndInit();
			((System.ComponentModel.ISupportInitialize)Shape1v).EndInit();
			((System.ComponentModel.ISupportInitialize)Shape2).EndInit();
			((System.ComponentModel.ISupportInitialize)Shape2v).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.PictureBox i_view;
		public System.Windows.Forms.PictureBox I_fn;
		private System.Windows.Forms.PictureBox i_abo;
		private System.Windows.Forms.PictureBox i_chset;
		//private System.Windows.Forms.TShape Shape1;
		//private System.Windows.Forms.TShape Shape2v;
		//private System.Windows.Forms.TShape Shape2;
		//private System.Windows.Forms.TShape Shape1v;
		private System.Windows.Forms.PictureBox ImageMegacopy;
		private System.Windows.Forms.PictureBox ImageMegaCopyV;
		private System.Windows.Forms.Label Label_char_view;
		//private System.Windows.Forms.TShape ShapeDupes;
		private System.Windows.Forms.Panel p_xx;
		private System.Windows.Forms.Panel Bevel3;
		private System.Windows.Forms.PictureBox i_ch;
		private System.Windows.Forms.PictureBox Ic1;
		private System.Windows.Forms.PictureBox Ic2;
		private System.Windows.Forms.Button b_shu;
		private System.Windows.Forms.Button b_vmir;
		private System.Windows.Forms.Button b_shr;
		private System.Windows.Forms.Button b_shl;
		private System.Windows.Forms.Button b_shd;
		private System.Windows.Forms.Button b_ror;
		private System.Windows.Forms.Button b_rol;
		private System.Windows.Forms.Button b_ress;
		private System.Windows.Forms.Button b_resd;
		private System.Windows.Forms.Button b_inv;
		private System.Windows.Forms.Button b_hmir;
		private System.Windows.Forms.Button b_clr;
		private System.Windows.Forms.Button b_cpy;
		private System.Windows.Forms.Button b_pst;
		private System.Windows.Forms.Panel p_hh;
		private System.Windows.Forms.Button b_load1;
		private System.Windows.Forms.Button b_save1as;
		private System.Windows.Forms.Button b_new;
		private System.Windows.Forms.Button b_about;
		private System.Windows.Forms.Button b_quit;
		private System.Windows.Forms.Button b_save1;
		private System.Windows.Forms.Button b_save2;
		private System.Windows.Forms.Button b_load2;
		private System.Windows.Forms.Button b_save2as;
		private System.Windows.Forms.Panel p_zz;
		private System.Windows.Forms.Panel Bevel4;
		private System.Windows.Forms.PictureBox i_col;
		private System.Windows.Forms.Button b_colorSwSetup;
		private System.Windows.Forms.Button b_gfx;
		private System.Windows.Forms.Button b_exportBMP;
		private System.Windows.Forms.Button b_colorSwitch;
		private System.Windows.Forms.Button b_lview;
		private System.Windows.Forms.Button b_clrview;
		private System.Windows.Forms.Button b_sview;
		private System.Windows.Forms.Panel p_status;
		private System.Windows.Forms.PictureBox i_actcol;
		private System.Windows.Forms.Label l_char;
		private System.Windows.Forms.Label l_col;
		private System.Windows.Forms.CheckBox SpeedButtonMegaCopy;
		private System.Windows.Forms.Button SpeedButtonUndo;
		private System.Windows.Forms.Button SpeedButtonRedo;
		private System.Windows.Forms.ComboBox ComboBoxWriteMode;
		private System.Windows.Forms.Panel p_color_switch;
		private System.Windows.Forms.PictureBox i_rec1;
		private System.Windows.Forms.PictureBox i_rec2;
		private System.Windows.Forms.ListBox lb_cs1;
		private System.Windows.Forms.ListBox lb_cs2;
		private System.Windows.Forms.ListBox ListBoxDebug;
		private System.Windows.Forms.Button b_enterText;
		private System.Windows.Forms.CheckBox CheckBox40bytes;
		private System.Windows.Forms.CheckBox cb_dupes;
		private System.Windows.Forms.OpenFileDialog d_open;
		private System.Windows.Forms.SaveFileDialog d_save;
		private System.Windows.Forms.Timer Timer1;
		private System.Windows.Forms.PictureBox ShapeDupes;
		/*private System.Windows.Forms.TActionList ActionListNormalModeOnly;
		private System.Windows.Forms.TAction Previous_char;
		private System.Windows.Forms.TAction Next_char;
		private System.Windows.Forms.TAction Rotate_left;
		private System.Windows.Forms.TAction Rotate_right;
		private System.Windows.Forms.TAction Mirror_horizontal;
		private System.Windows.Forms.TAction Mirror_vertical;
		private System.Windows.Forms.TAction Color1;
		private System.Windows.Forms.TAction Color2;
		private System.Windows.Forms.TAction Color3;
		private System.Windows.Forms.TAction Shift_left;
		private System.Windows.Forms.TAction Shift_right;
		private System.Windows.Forms.TAction Shift_up;
		private System.Windows.Forms.TAction Shift_down;
		private System.Windows.Forms.TAction Invert;
		private System.Windows.Forms.TAction Clear;
		private System.Windows.Forms.TAction Restore_default;
		private System.Windows.Forms.TAction Restore_lastSaved;
		private System.Windows.Forms.TActionList ActionList2;
		private System.Windows.Forms.TAction Clipboard_copy;
		private System.Windows.Forms.TAction Clipboard_paste;
		private System.Windows.Forms.TAction EscapePressed;
		private System.Windows.Forms.TAction Undo_Font;
		private System.Windows.Forms.TAction Redo_Font;
		private System.Windows.Forms.TApplicationEvents ApplicationEvents1;*/
		private System.Windows.Forms.Timer DebugTimer;
		private System.Windows.Forms.Timer TimerDuplicates;


		public void Timer1Timer(object sender, EventArgs e)
		{
			i_abo.Visible = false;
			Timer1.Enabled = false;
		}

		public void DebugTimerTimer(object sender, EventArgs e)
		{
			/*
			int a = 0;
			ListBoxDebug.Visible = true;
			ListBoxDebug.Items.Clear();
			ListBoxDebug.Items.Add("undoindex:" + Convert.ToString(undoBufferIndex));
			ListBoxDebug.Items.Add("undoflags:" + Convert.ToString(undoBufferFlags[undoBufferIndex]));
			ListBoxDebug.Items.Add("charchanged:" + Convert.ToString(Convert.ToInt32(CharacterEdited())));

			for (a = 0; a <= 10; a++)
			{
				ListBoxDebug.Items.add("undobuffer(" + Convert.ToString(a) + "): " + Convert.ToString(undoBufferFlags[a]));
			}
			*/
		}


		public void TimerDuplicatesTimer(object sender, EventArgs e)
		{
			var dupe = FindDuplicateChar();

			if (dupe != selectedCharacterIndex)
			{
				duplicateCharacterIndex = dupe;

				var rx = dupe % 32;
				var ry = dupe / 32;
				var fontNr = dupe / 256;

				ShapeDupes.Left = /*I_fn.Left + */rx * 16 - 2;
				ShapeDupes.Top = /*I_fn.Top + */ry * 16 - 2;
				ShapeDupes.Visible = true;
			}
		}

		private PictureBox Shape1;
		private PictureBox Shape1v;
		private PictureBox Shape2;
		private PictureBox Shape2v;
		private ToolTip toolTips;
	}
}