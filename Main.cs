using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Drawing.Drawing2D;
using System.Media;
using System.Text;
using TinyJson;

namespace FontMaker
{

	public enum TMegaCopyStatus
	{
		None, Selecting, Selected, Pasting
	}

	public partial class TMainForm : Form
	{
		private static SolidBrush blackBrush = new(Color.Black);
		private static SolidBrush whiteBrush = new(Color.White);
		private static SolidBrush cyanBrush = new(Color.Cyan);
		private static SolidBrush redBrush = new(Color.Red);

		private List<System.Windows.Forms.Button> ActionListNormalModeOnly = new();

		public TMainForm()
		{
			InitializeComponent();

			// Admin the action list
			ActionListNormalModeOnly.Add(b_rol);
			ActionListNormalModeOnly.Add(b_ror);
			ActionListNormalModeOnly.Add(b_hmir);
			ActionListNormalModeOnly.Add(b_vmir);
			ActionListNormalModeOnly.Add(b_shl);
			ActionListNormalModeOnly.Add(b_shr);
			ActionListNormalModeOnly.Add(b_shu);
			ActionListNormalModeOnly.Add(b_shd);
			ActionListNormalModeOnly.Add(b_inv);
			ActionListNormalModeOnly.Add(b_clr);
			ActionListNormalModeOnly.Add(b_ress);
			ActionListNormalModeOnly.Add(b_resd);

			this.Load += new System.EventHandler(FormCreate!);
		}

		private void FormCreate(object sender, EventArgs e)
		{
			DoubleBuffered = true;

			var pfs = new string[] { "BAK (00)", "PF0 (01)", "PF1 (10)", "PF2 (11)" };

			ac = 2;

			MainUnit.CheckResources();
			LoadPalette();

			//undobuffer initialization
			undoBufferIndex = 0;

			for (var a = 0; a <= UNDOBUFFERSIZE; a++)
			{
				undoBufferFlags[a] = -1;
			}

			selectedCharacterIndex = 0;

			// Init which font is shown on each line of the preview window
			for (var a = 0; a < VIEW_HEIGHT; a++)
			{
				chsline[a] = 1;
			}

			pathf = AppContext.BaseDirectory;

			var paramCount = Environment.GetCommandLineArgs().Length;

			string? ext;
			if (Environment.GetCommandLineArgs().Length - 1 == 1)
			{
				var pathf = Path.GetDirectoryName(Environment.GetCommandLineArgs()[1]) + "\\";
				ext = Path.GetExtension(Environment.GetCommandLineArgs()[1]).ToLower();

				switch (ext.ToLower())
				{
					case ".fn2":
						{
							//Load_font(Environment.GetCommandLineArgs()[1], 2);
							//tempstring = Environment.GetCommandLineArgs()[1].Substring(-1, Environment.GetCommandLineArgs()[1].Length - 4);
							//fname1 = tempstring + "1.fnt";
							//fname2 = tempstring + "2.fnt";
						}
						break;

					case ".fnt":
						{
							fname1 = Environment.GetCommandLineArgs()[1];
						}
						break;

					case ".atrview":
						{
							LoadViewFile(Environment.GetCommandLineArgs()[1], true);
							UpdateFormCaption();
							RedrawSet();
							RedrawLineTypes();
							RedrawView();
							RedrawPal();
							RedrawViewChar();
							RedrawChar();
							fname2 = pathf + Path.DirectorySeparatorChar + "Default.fnt";

						}
						break;
					default:
						fname1 = Environment.GetCommandLineArgs()[1];
						break;
				}

				Timer1.Enabled = false;
				i_abo.Visible = false;
			}
			else
			{
				// If no input file set upon start, then show splashscreen
				Timer1.Enabled = true;
				i_abo.Visible = true;

				LoadViewFile("default.atrview", true);
				UpdateFormCaption();
				RedrawSet();
				RedrawLineTypes();
				RedrawView();
				RedrawPal();
				RedrawViewChar();
				RedrawChar();
				ext = ".atrview";
				b_save1Click(null, EventArgs.Empty);
			}

			if (ext != ".atrview")
			{
				default_pal();
				RedrawPal();
				grid();

				if (ext != ".fn2")
				{
					Load_font(fname1, 0);
					Load_font(fname2, 1);
					UpdateFormCaption();
				}
				else
				{
					UpdateFormCaption();
				}

				RedrawSet();
				DefaultView();
				RedrawView();
			}
			for (var a = 0; a < 4; a++)
			{
				lb_cs1.Items.Add(pfs[a]);
				lb_cs2.Items.Add(pfs[a]);
			}

			lb_cs1.SelectedIndex = 0;
			lb_cs2.SelectedIndex = 0;
			lb_cs1Click(null, EventArgs.Empty);
			lb_cs2Click(null, EventArgs.Empty);

			Add2UndoInitial(); //initial undobuffer entry
			UpdateUndoButtons(false);

			// Init the gui
			ComboBoxWriteMode.SelectedIndex = 0;

			MakeSomeColouredBlocks();

			I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
		}

		private void MakeSomeColouredBlocks()
		{
			// Paint something into the window
			var img = GetImage(Shape1);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(redBrush, new Rectangle(0, 0, img.Width, img.Height));
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			Shape1.Region = new Region(graphicsPath);
			Shape1.Refresh();

			// Paint something into the window
			img = GetImage(Shape1v);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(cyanBrush, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			Shape1v.Region = new Region(graphicsPath);

			Shape1v.Refresh();

			// Paint something into the window
			img = GetImage(Shape2);
			using (var gr = Graphics.FromImage(img))
			{
				var trans = new SolidBrush(Color.Green);
				gr.FillRectangle(trans, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			Shape2.Region = new Region(graphicsPath);
			Shape2.Refresh();

			// Paint something into the window
			img = GetImage(Shape2v);
			using (var gr = Graphics.FromImage(img))
			{
				var trans = new SolidBrush(Color.Yellow);
				gr.FillRectangle(trans, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));

			Shape2v.Region = new Region(graphicsPath);
			Shape2v.Refresh();

			// Paint something into the window
			img = GetImage(ShapeDupes);
			using (var gr = Graphics.FromImage(img))
			{
				var trans = new SolidBrush(Color.Purple);
				gr.FillRectangle(trans, new Rectangle(0, 0, img.Width, img.Height));
			}
			graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(18, 0, 2, 20));
			graphicsPath.AddRectangle(new Rectangle(0, 18, 20, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, 20));
			for (var x = 0; x < 20; ++x)
			{
				graphicsPath.AddRectangle(new Rectangle(x, x, 2, 2));
				graphicsPath.AddRectangle(new Rectangle(18 - x, x, 2, 2));
			}

			ShapeDupes.Region = new Region(graphicsPath);
			ShapeDupes.Refresh();
		}

		private void FormCloseQuery(object sender, FormClosingEventArgs e)
		{
			
			e.Cancel = true;
			MainUnit.exitowiec();
		}

		public void i_chMouseDown(object sender, MouseEventArgs e)
		{
			if (e.X < 0 || e.Y < 0 || e.X >= i_ch.Width || e.Y >= i_ch.Height)
				return;

			var img = GetImage(i_ch);
			using (var gr = Graphics.FromImage(img))
			{
				clck = true;
				ButtonHeld = e.Button;

				if (Control.ModifierKeys == Keys.Control)
				{
					clck = false;
				} //ctrl+click no button toggle

				var hp = MainUnit.GetCharacterPointer(selectedCharacterIndex);
				var ry = e.Y / 20;
				cly = ry;

				if (!gfx)
				{
					var charline2col = MainUnit.DecodeBW(ft[hp + ry]);
					var rx = e.X / 20;
					clx = rx;

					if (e.Button == MouseButtons.Left)
					{
						if (ComboBoxWriteMode.SelectedIndex == 0)
						{
							if (charline2col[rx] == 0)
							{
								charline2col[rx] = 1;
							}
							else
							{
								charline2col[rx] = 0;
							}
						}
						else
						{
							charline2col[rx] = 1;
						}
					}
					else if (e.Button == MouseButtons.Right)
					{
						charline2col[rx] = 0;
					} //delete

					ft[hp + ry] = MainUnit.EncodeBW(charline2col);
					DoChar();

					//var brush = new SolidBrush( charline2col[rx] == 1 ? palette[cpal[0]] : palette[cpal[1]]);
					var brush = cpalBrushes[charline2col[rx] == 1 ? 0 : 1];
					gr.FillRectangle(brush, rx * 20, ry * 20, 20, 20);
					gr.FillRectangle(cpalBrushes[0], rx * 20, ry * 20, 1, 1);
					//i_ch.Canvas.FillRect(bounds(rx * 20, ry * 20, 20, 20));
					//i_ch.Canvas.Pixels[rx * 20, ry * 20] = palette[cpal[0]];
				}
				else
				{
					var charline5col = MainUnit.DecodeCL(ft[hp + ry]);

					for (var a = 0; a < 4; a++)
					{
						charline5col[a] = bits2colorIndex[charline5col[a]];
					}

					var rx = e.X / 40;
					clx = rx;

					if (e.Button == MouseButtons.Left)
					{
						if (ComboBoxWriteMode.SelectedIndex == 0)
						{
							if (charline5col[rx] != ac)
							{
								charline5col[rx] = (byte)ac;
							}
							else
							{
								charline5col[rx] = 1;
							}
						}
						else
						{
							charline5col[rx] = (byte)ac;
						}
					}
					else if (e.Button == MouseButtons.Right)
					{
						charline5col[rx] = 1;
					} //delete

					// Draw pixel
					//i_ch.Canvas.Brush.Color = palette[cpal[charline5col[rx]]];
					//i_ch.canvas.FillRect(bounds(rx * 40, ry * 20, 40, 20));
					gr.FillRectangle(cpalBrushes[charline5col[rx]], rx * 40, ry * 20, 40, 20);

					//recode to byte and save to charset
					for (var a = 0; a < 4; a++)
					{
						charline5col[a] = colorIndex2bits[charline5col[a]];
					}

					ft[hp + ry] = MainUnit.EncodeCL(charline5col);
					DoChar();
				}

				RedrawViewChar();
				UpdateUndoButtons(CharacterEdited());
				CheckDuplicate();
			}
			i_ch.Refresh();
		}

		public void i_chMouseMove(object sender, MouseEventArgs e)
		{
			if (clck)
			{
				bool je = false;
				var nx = 0;
				var ny = e.Y / 20;

				if (gfx)
				{
					nx = e.X / 40;
				}
				else
				{
					nx = e.X / 20;
				}

				if ((e.X < 0) || (e.X > i_ch.Width) || (e.Y < 0) || (e.Y > i_ch.Height))
				{
					je = true;
				}

				if ((!je) && ((nx != clx) || (ny != cly)))
				{
					i_chMouseDown(null, new MouseEventArgs(ButtonHeld, 1, e.X, e.Y, 0));
				}
			}
		}

		public void i_chMouseUp(object sender, MouseEventArgs e)
		{
			clck = false;
		}

		public void i_chsetMouseDown(object sender, MouseEventArgs e)
		{
			var ry = e.Y / 16;

			if (chsline[ry] == 2)
			{
				chsline[ry] = 1;
			}
			else
			{
				chsline[ry] = 2;
			}

			RedrawLineTypes();
			RedrawView();
		}

		/* Public declarations */
		//	ActionList contains actions with different TAG parameter, legend:
		//	0 - action that does not modify character data
		//	1 - action that modifies character data
		//	2 - action that modifies character data applicable only on Mode 2
		public void CheckDuplicate()
		{
			if (cb_dupes.Checked == false || cb_dupes.Enabled == false)
			{
				return;
			}

			duplicateCharacterIndex = selectedCharacterIndex;

			if (FindDuplicateChar() != selectedCharacterIndex)
			{
				TimerDuplicates.Enabled = true;
			}
			else
			{
				TimerDuplicates.Enabled = false;
				ShapeDupes.Visible = false;
			}
		}

		public void SetColor(int colorNum)
		{
			//colorNum := colorNum + 2; //added offset in font palette
			if (ac != colorNum)
			{
				if ((int)(Ic1.Tag) == colorNum)
				{
					Ic1MouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
				}
				else
				{
					Ic2MouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
				}
			}
		}

		// repaint actual character in font area
		public void DoChar()
		{
			var img = GetImage(I_fn);
			using (var gr = Graphics.FromImage(img))
			{
				UpdateUndoButtons(CharacterEdited());
				var ry = selectedCharacterIndex / 32;
				var rx = selectedCharacterIndex % 32;

				if ((ry > 3) && (ry < 12))
				{
					ry -= 4;
				}

				if ((ry > 11) && (ry < 16))
				{
					ry -= 8;
				}

				var hp = ry * 32 * 8 + rx * 8;

				if (!gfx)
				{
					for (var a = 0; a < 8; a++)
					{
						var line2color = MainUnit.DecodeBW(ft[hp + a]);

						for (var b = 0; b < 8; b++)
						{
							if (hp < 1024)
							{
								gr.FillRectangle(cpalBrushes[1 - line2color[b]], rx * 16 + b * 2, ry * 16 + a * 2, 2, 2);
								//I_fn.Canvas.Brush.Color = palette[cpal[1 - Convert.ToInt32(line2color[b])]];
								//I_fn.Canvas.FillRect(bounds(rx * 16 + b * 2, ry * 16 + a * 2, 2, 2));
								gr.FillRectangle(cpalBrushes[line2color[b]], rx * 16 + b * 2, ry * 16 + a * 2 + 64, 2, 2);
								//I_fn.Canvas.Brush.Color = palette[cpal[Convert.ToInt32(line2color[b])]];
								//I_fn.Canvas.FillRect(bounds(rx * 16 + b * 2, ry * 16 + a * 2 + 64, 2, 2));
							}
							else
							{
								gr.FillRectangle(cpalBrushes[1 - line2color[b]], rx * 16 + b * 2, ry * 16 + a * 2 + 64, 2, 2);
								//I_fn.Canvas.Brush.Color = palette[cpal[1 - Convert.ToInt32(line2color[b])]];
								//I_fn.Canvas.FillRect(bounds(rx * 16 + b * 2, ry * 16 + a * 2 + 64, 2, 2));
								gr.FillRectangle(cpalBrushes[line2color[b]], rx * 16 + b * 2, ry * 16 + a * 2 + 128, 2, 2);
								//I_fn.Canvas.Brush.Color = palette[cpal[Convert.ToInt32(line2color[b])]];
								//I_fn.Canvas.FillRect(bounds(rx * 16 + b * 2, ry * 16 + a * 2 + 128, 2, 2));
							}
						}
					}
				}
				else
				{
					for (var a = 0; a < 8; a++)
					{
						var line5color = MainUnit.DecodeCL(ft[hp + a]);

						for (var b = 0; b < 4; b++)
						{
							if (hp < 1024)
							{
								var brush = cpalBrushes[bits2colorIndex[line5color[b]]];
								gr.FillRectangle(brush, rx * 16 + b * 4, ry * 16 + a * 2, 4, 2);
								//I_fn.Canvas.Brush.Color = palette[cpal[bits2colorIndex[line5color[b]]]];
								//I_fn.Canvas.FillRect(bounds(rx * 16 + b * 4, ry * 16 + a * 2, 4, 2));

								if (line5color[b] == 3)
								{
									brush = cpalBrushes[5];
									//I_fn.Canvas.Brush.Color = palette[cpal[5]];
								}

								gr.FillRectangle(brush, rx * 16 + b * 4, ry * 16 + a * 2 + 64, 4, 2);
								//I_fn.Canvas.FillRect(bounds(rx * 16 + b * 4, ry * 16 + a * 2 + 64, 4, 2));
							}
							else
							{
								var brush = cpalBrushes[bits2colorIndex[line5color[b]]];
								gr.FillRectangle(brush, rx * 16 + b * 4, ry * 16 + a * 2 + 64, 4, 2);
								//I_fn.Canvas.Brush.Color = palette[cpal[bits2colorIndex[line5color[b]]]];
								//I_fn.Canvas.FillRect(bounds(rx * 16 + b * 4, ry * 16 + a * 2 + 64, 4, 2));

								if (line5color[b] == 3)
								{
									brush = cpalBrushes[5];
									//I_fn.Canvas.Brush.Color = palette[cpal[5]];
								}

								gr.FillRectangle(brush, rx * 16 + b * 4, ry * 16 + a * 2 + 128, 4, 2);
								//I_fn.Canvas.FillRect(bounds(rx * 16 + b * 4, ry * 16 + a * 2 + 128, 4, 2));
							}
						}
					}
				}
			}

			I_fn.Refresh();
		}


		private void i_colMouseDown(object sender, MouseEventArgs e)
		{
			if (Control.ModifierKeys == Keys.Shift)
			{
				var od = MessageBox.Show("Restore default colors?", "Atari FontMaker", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

				if (od == DialogResult.Yes)
				{
					default_pal();
					RedrawPal();
					gfx = !gfx;
					b_gfxClick(null, EventArgs.Empty);
				}
			}
			else
			{
				var wh = e.X / 45 + (e.Y / 18) * 2;
				AtariColorSelectorUnit.AtariColorSelectorForm.SetSelectedColorIndex(cpal[wh]);
				AtariColorSelectorUnit.AtariColorSelectorForm.ShowDialog();

				switch (wh)
				{
					case 0:
						{
							cpal[0] = (byte)(AtariColorSelectorUnit.AtariColorSelectorForm.selectedColorIndex % 16 + (cpal[1] / 16) * 16);
							UpdateBrushCache(0);
						}
						break;

					case 1:
						{
							cpal[1] = AtariColorSelectorUnit.AtariColorSelectorForm.selectedColorIndex;
							UpdateBrushCache(1);
							cpal[0] = (byte)((cpal[1] / 16) * 16 + cpal[0] % 16);
							UpdateBrushCache(0);
						}
						break;

					default:
						cpal[wh] = AtariColorSelectorUnit.AtariColorSelectorForm.selectedColorIndex;
						UpdateBrushCache(wh);
						break;
				}
			}

			RedrawPal();
			RedrawSet();
			RedrawChar();
			RedrawView();
			lb_cs1Click(null, EventArgs.Empty);
			lb_cs2Click(null, EventArgs.Empty);

			BuildBrushCache();
		}

		public void b_load1Click(object sender, EventArgs e)
		{
			string tempstring = string.Empty;
			d_open.FileName = string.Empty;
			d_open.InitialDirectory = pathf;
			d_open.Filter = "Atari font 1 or Dual font (*.fnt,*.fn2)|*.fnt;*.fn2";
			var ok = d_open.ShowDialog();

			if (ok == DialogResult.OK)
			{
				var dual = Path.GetExtension(d_open.FileName) == ".fn2";
				Load_font(d_open.FileName, (dual ? 1 : 0) * 2); //0 if regular, 2 if dual
				pathf = Path.GetDirectoryName(d_open.FileName) + "\\";

				if (dual)
				{
					tempstring = d_open.FileName.Substring(-1, d_open.FileName.Length - 4);
					fname1 = tempstring + "1.fnt";
					fname2 = tempstring + "2.fnt";
				}
				else
				{
					fname1 = d_open.FileName;
				}

				UpdateFormCaption();
				RedrawSet();
				I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
				RedrawView();
				Add2UndoFullDifferenceScan(); //full font scan
			}

			CheckDuplicate();
		}

		public void b_load2Click(object sender, EventArgs e)
		{
			d_open.FileName = string.Empty;
			d_open.InitialDirectory = pathf;
			d_open.Filter = "Atari font 2 (*.fnt)|*.fnt";
			var ok = d_open.ShowDialog();

			if (ok == DialogResult.OK)
			{
				Load_font(d_open.FileName, 1);
				pathf = Path.GetDirectoryName(d_open.FileName) + "\\";
				fname2 = d_open.FileName;
				UpdateFormCaption();
				RedrawSet();
				I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
				RedrawView();
				Add2UndoFullDifferenceScan(); //full font scan
			}

			CheckDuplicate();
		}

		public void b_save1Click(object sender, EventArgs e)
		{
			Save_font(fname1, 0);
		}

		public void b_save2Click(object sender, EventArgs e)
		{
			Save_font(fname2, 1);
		}

		public void b_save1asClick(object sender, EventArgs e)
		{

			d_save.FileName = string.Empty;
			d_save.InitialDirectory = pathf;
			d_save.Filter = "Atari font 1(*.fnt)|*.fnt";
			d_save.DefaultExt = "fnt";
			var ok = d_save.ShowDialog();

			if (ok == DialogResult.OK)
			{
				Save_font(d_save.FileName, 0);
				pathf = Path.GetDirectoryName(d_save.FileName) + "\\";
				fname1 = d_save.FileName;
			}

			UpdateFormCaption();
		}

		public void b_save2asClick(object sender, EventArgs e)
		{
			d_save.FileName = string.Empty;
			d_save.InitialDirectory = pathf;
			d_save.Filter = "Atari font 2(*.fnt)|*.fnt";
			d_save.DefaultExt = "fnt";
			var ok = d_save.ShowDialog();

			if (ok == DialogResult.OK)
			{
				Save_font(d_save.FileName, 1);
				pathf = Path.GetDirectoryName(d_save.FileName) + "\\";
				fname2 = d_save.FileName;
			}

			UpdateFormCaption();
		}

		public void i_viewMouseDown(object sender, MouseEventArgs e)
		{
			if ((e.X >= i_view.Width) || (e.Y >= i_view.Height) || e.X < 0 || e.Y < 0)
			{
				return;
			}

			var rx = e.X / 16;
			var ry = e.Y / 16;

			if (SpeedButtonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case TMegaCopyStatus.None:
					case TMegaCopyStatus.Selected:
						{
							if (e.Button == MouseButtons.Left)
							{
								// Define copy origin point
								copyRange.Y = ry;
								copyRange.X = rx;
								megaCopyStatus = TMegaCopyStatus.Selecting;
								Shape1v.Left = i_view.Left + e.X - e.X % 16 - 2;
								Shape1v.Top = i_view.Top + e.Y - e.Y % 16 - 2;
								Shape1v.Width = 20;
								Shape1v.Height = 20;
								Shape1v.Visible = true;
								Shape1.Visible = false;
								Debug.Print("Set the shape1v size 20x20");
							}
						}
						break;

					case TMegaCopyStatus.Pasting:
						{
							if (!MouseValidView(e.X, e.Y))
							{
								return;
							}

							// Paste
							if (e.Button == MouseButtons.Left)
							{
								copyTarget = new Point(rx, ry);
								Clipboard_pasteExecute(sender, true);
								ResetMegaCopyStatus();
							}
						}
						break;
				}
			}
			else
			{
				// Not in MegaCopy mode.
				// Draw or read character
				clckv = true;

				if (Control.ModifierKeys == Keys.Control)
				{
					clckv = false;
				} //ctrl+click nezapina toggle

				if (ry >= i_view.Height / 16)
				{
					return;
				}

				clxv = rx;
				clyv = ry;

				if (e.Button == MouseButtons.Left)
				{
					vw[rx, ry] = (byte)(selectedCharacterIndex % 256);
					RedrawViewChar();
				}

				if (e.Button == MouseButtons.Right)
				{
					var readChar = vw[rx, ry];
					var bx = readChar % 32;
					var by = readChar / 32;

					if (chsline[ry] == 2)
					{
						by = by | 8;
					}

					// Select the character in the font
					I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, bx * 16 + 4, by * 16 + 4, 0));
				}
			}
		}

		public void i_viewMouseUp(object sender, MouseEventArgs e)
		{
			clckv = false;

			if ((e.X >= i_view.Width) || (e.Y >= i_view.Height))
			{
				return;
			}

			var rx = e.X / 16;
			var ry = e.Y / 16;

			if (SpeedButtonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case TMegaCopyStatus.Selecting:
						{
							if (ry <= copyRange.Y)
							{
								copyRange.Height = 0;
							}
							else
							{
								copyRange.Height = ry - copyRange.Y;
							}

							if (rx <= copyRange.X)
							{
								copyRange.Width = 0;
							}
							else
							{
								copyRange.Width = rx - copyRange.X;
							}

							megaCopyStatus = TMegaCopyStatus.Selected;
							//Clipboard_copyExecute(sender,true);
							/*shape2.Left := i_fn.left + x - x mod 16 - 2;
							shape2.Top := i_fn.Top + y - y mod 16 - 2;
							Shape2.Width := Shape1.Width;
							Shape2.Height := Shape1.Height;
							Shape2.Visible := True;
							*/
							break;
						}
				}
			}
		}

		public void i_viewMouseMove(object sender, MouseEventArgs e)
		{
			int rx, ry;

			/*  nx:=x div 16;
			  ny:=y div 16;
			  if (ny >= i_view.Height div 16) or (nx >= i_view.Width div 16) then exit;
			*/
			if (SpeedButtonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case TMegaCopyStatus.Selecting:
						{
							if ((e.X >= i_view.Width) || (e.Y >= i_view.Height))
							{
								return;
							}

							rx = e.X / 16;
							ry = e.Y / 16;

							int origWidth = Shape1v.Width;
							int origHeight = Shape1v.Height;

							int w = 20;
							int h = 20;
							var temp = (rx - copyRange.X + 1) * 16 + 4;
							if (temp >= 20)
								w = temp;

							temp = (ry - copyRange.Y + 1) * 16 + 4;
							if (temp >= 20)
								h = temp;

							if (w != origWidth || h != origHeight)
							{
								Shape1v.Size = new Size(w, h);
							}
						}
						break;

					case TMegaCopyStatus.Pasting:
						{
							if (!MouseValidView(e.X, e.Y))
							{
								Shape2v.Visible = false;
								ImageMegaCopyV.Visible = false;
								return;
							}

							Shape2v.Left = i_view.Left + e.X - e.X % 16 - 2;
							Shape2v.Top = i_view.Top + e.Y - e.Y % 16 - 2;
							Shape2v.Visible = true;
							ImageMegaCopyV.Visible = true;
							Shape2.Visible = false;
							ImageMegacopy.Visible = false;
							ImageMegaCopyV.Left = Shape2v.Left + 2;
							ImageMegaCopyV.Top = Shape2v.Top + 2;
						}
						break;
				}
			}
			else
			{
				var je = false;

				//if (ny >= i_view.Height div 16) or (nx >= i_view.Width div 16) then exit;

				if (clckv)
				{
					if ((e.X < 0) || (e.X > i_view.Width) || (e.Y < 0) || (e.Y > i_view.Height))
					{
						je = true;
					}

					if ((!je) && ((e.X / 16 != clxv) || (e.Y / 16 != clyv)))
					{
						i_viewMouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, e.X, e.Y, 0));
					}
				}

				Shape2v.Width = 20;
				Shape2v.Height = 20;
				Shape2v.Left = i_view.Left + e.X - e.X % 16 - 2;
				Shape2v.Top = i_view.Top + e.Y - e.Y % 16 - 2;
				Shape2v.Visible = true;
			}

			// Char under cursor:
			if ((e.X >= i_view.Width) || (e.Y >= i_view.Height) || e.X < 0 || e.Y < 0)
			{
				return;
			}

			rx = e.X / 16;
			ry = e.Y / 16;
			var fontchar = vw[rx, ry];
			Label_char_view.Text = $@"Char: Font {chsline[ry]} ${fontchar:X2} #{fontchar}";
		}

		private static bool inShape1VResize = false;

		private void Shape1v_Resize(object sender, EventArgs e)
		{
			if (inShape1VResize)
				return;
			inShape1VResize = true;

			var img = NewImage(Shape1v);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(cyanBrush, new Rectangle(0, 0, img.Width, img.Height));
				Shape1v.Region?.Dispose();
				Shape1v.Size = new Size(img.Width, img.Height);

			}
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, Shape1v.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(Shape1v.Width - 2, 0, 2, Shape1v.Height));
			graphicsPath.AddRectangle(new Rectangle(0, Shape1v.Height - 2, Shape1v.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, Shape1v.Height));
			Shape1v.Region = new Region(graphicsPath);

			inShape1VResize = false;
		}

		private void i_viewMouseDoubleClick(object sender, MouseEventArgs e)
		{
			// Reset selection by right DoubleClick
			if (e.Button == MouseButtons.Right)
			{
				ResetMegaCopyStatus();
			}

		}

		public void b_gfxClick(object sender, EventArgs e)
		{
			gfx = !gfx;
			RedrawSet();
			RedrawView();
			I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));

			if (!SpeedButtonMegaCopy.Checked)
			{
				RevalidateCharButtons();
			}

			if (!gfx && p_color_switch.Visible)
			{
				b_colorSwSetupClick(null, EventArgs.Empty);
			}
		}

		private void Ic1MouseDown(object sender, MouseEventArgs e)
		{
			var bu = (int)Ic1.Tag;
			Ic1.Tag = ac;
			ac = bu;
			RedrawPal();
		}

		private void Ic2MouseDown(object sender, MouseEventArgs e)
		{
			var bu = (int)Ic2.Tag;
			Ic2.Tag = ac;
			ac = bu;
			RedrawPal();
		}

		public void b_aboutClick(object sender, EventArgs e)
		{
			i_abo.Visible = !i_abo.Visible;
		}

		public void i_aboMouseDown(object sender, MouseEventArgs e)
		{
			//if (x<225)and(y>80) then
			MainUnit.OpenURL("http://matosimi.atari.org");
			//if (x>295)and(y>80) then shellexecute(form1.handle,'open','mailto:matosimi@centrum.sk','','',sw_show);
			i_abo.Visible = false;
		}

		public void RedrawPal()
		{
			var img = GetImage(i_col);
			using (var gr = Graphics.FromImage(img))
			{
				gr.Clear(this.BackColor);

				// Check that the cpal[0] color is correct
				var new0 = (byte)((cpal[1] / 16) * 16 + cpal[0] % 16);
				if (new0 != cpal[0])
				{
					cpal[0] = new0;
					UpdateBrushCache(0);
				}

				for (var a = 0; a < 3; a++)
				{
					for (var b = 0; b < 2; b++)
					{
						//i_col.Canvas.Brush.Color = palette[cpal[b + a * 2]];
						//i_col.Canvas.FillRect(bounds(b * 45, a * 18, 45, 22));
						//gr.FillRectangle(new SolidBrush(palette[cpal[b + a * 2]]), b * 45, a * 18, 45, 22);
						gr.FillRectangle(cpalBrushes[b + a * 2], b * 45, a * 18, 45, 22);
						drawTxt(gr, b * 45, a * 18, b + a * 2, palette[cpal[b + a * 2]]);
					}
				}
			}
			i_col.Refresh();

			img = GetImage(Ic1);
			using (var gr = Graphics.FromImage(img))
			{
				var tagVal = (int)Ic1.Tag;
				var brush = cpalBrushes[tagVal]; // new SolidBrush(palette[cpal[tagVal]]);
				gr.FillRectangle(brush, 0, 0, 49, 17);
				drawTxt(gr, 1, 1, tagVal, palette[cpal[tagVal]]);

				// Ic1.Canvas.Brush.Color = palette[cpal[Ic1.Tag]];
				// Ic1.Canvas.FillRect(bounds(0, 0, 49, 17));
				// MainUnit.drawTxt(Ic1, 1, 1, Ic1.Tag, Convert.ToInt32(palette[cpal[Ic1.Tag]]));

			}
			Ic1.Refresh();

			img = GetImage(Ic2);
			using (var gr = Graphics.FromImage(img))
			{
				var tagVal = (int)Ic2.Tag;
				var brush = cpalBrushes[tagVal]; // new SolidBrush(palette[cpal[tagVal]]);
				gr.FillRectangle(brush, 0, 0, 49, 17);
				drawTxt(gr, 1, 1, tagVal, palette[cpal[tagVal]]);

				// Ic2.Canvas.Brush.Color = palette[cpal[Ic2.Tag]];
				// Ic2.Canvas.FillRect(bounds(0, 0, 49, 17));
				// MainUnit.drawTxt(Ic2, 1, 1, Ic2.Tag, Convert.ToInt32(palette[cpal[Ic2.Tag]]));

			}
			Ic2.Refresh();

			img = GetImage(i_actcol);
			using (var gr = Graphics.FromImage(img))
			{
				var brush = cpalBrushes[ac]; // new SolidBrush(palette[cpal[ac]]);
				gr.FillRectangle(brush, 0, 0, 49, 17);
				drawTxt(gr, 1, 1, ac, palette[cpal[ac]]);

				// i_actcol.Canvas.Brush.Color = palette[cpal[ac]];
				// i_actcol.Canvas.FillRect(bounds(0, 0, 49, 17));
				// MainUnit.drawTxt(i_actcol, 1, 1, Convert.ToInt32(ac), Convert.ToInt32(palette[cpal[ac]]));

			}
			i_actcol.Refresh();
		}

		// redraws character that is being edited/selected in character edit window
		public void RedrawChar()
		{
			var img = GetImage(i_ch);
			using (var gr = Graphics.FromImage(img))
			{
				if (!gfx)
				{
					//gr.0
					var character2Color = MainUnit.Get2ColorCharacter(selectedCharacterIndex);

					for (var a = 0; a < 8; a++)
					{
						for (var b = 0; b < 8; b++)
						{
							var brush = new SolidBrush(character2Color[b, a] == 0 ? palette[cpal[1]] : palette[cpal[0]]);
							gr.FillRectangle(brush, b * 20, a * 20, 20, 20);
							//i_ch.Canvas.fillRect(bounds(b * 20, a * 20, 20, 20));
							gr.FillRectangle(whiteBrush, b * 20, a * 20, 1, 1);
							//i_ch.Canvas.Pixels[b * 20, a * 20] = Color.White;
						}
					}
				}
				else
				{
					var character5color = MainUnit.Get5ColorCharacter(selectedCharacterIndex);

					for (var a = 0; a < 8; a++)
					{
						for (var b = 0; b < 4; b++)
						{
							//var brush = new SolidBrush(palette[cpal[bits2colorIndex[character5color[b, a]]]]);
							var brush = cpalBrushes[bits2colorIndex[character5color[b, a]]];
							gr.FillRectangle(brush, b * 40, a * 20, 40, 20);
							//i_ch.Canvas.Brush.color = palette[cpal[bits2colorIndex[character5color[b, a]]]];
							//i_ch.Canvas.fillrect(bounds(b * 40, a * 20, 40, 20));
						}
					}
				}
			}
			i_ch.Refresh();
		}

		public void DefaultView()
		{
			const string dt = "Test"; // '\x34' + "he" + '\x0' + "quick" + '\x0' + "brown" + '\x0' + "fox" + '\x0' + "jumps";
			const string dy = "String"; // "over" + '\x0' + "the" + '\x0' + "lazy" + '\x0' + "dog";

			var bytes = Encoding.Default.GetBytes(dt);
			for (var a = 0; a < dt.Length; a++)
			{
				vw[a + 2, 2] = bytes[a];
			}

			bytes = Encoding.Default.GetBytes(dy);
			for (var a = 0; a < dy.Length; a++)
			{
				vw[a + 6, 3] = bytes[a];
			}

			RedrawLineTypes();
		}

		// Redraw whole view area by copying characters from font area
		public void RedrawView()
		{
			var img = GetImage(i_view);
			using (var gr = Graphics.FromImage(img))
			{
				var destRect = new Rectangle
				{
					Width = 16,
					Height = 16,
				};

				var srcRect = new Rectangle
				{
					Width = 16,
					Height = 16,
				};

				for (var b = 0; b < VIEW_HEIGHT; b++)
				{
					for (var a = 0; a < VIEW_WIDTH; a++)
					{
						var rx = vw[a, b] % 32;
						var ry = (vw[a, b] / 32);

						if (chsline[b] == 2)
						{
							ry = (ry | 8);
						}

						//i_view.Canvas.CopyRect(bounds(a * 16, b * 16, 16, 16), I_fn.Canvas, bounds(rx * 16, ry * 16, 16, 16));
						destRect.X = a * 16;
						destRect.Y = b * 16;

						srcRect.X = rx * 16;
						srcRect.Y = ry * 16;

						gr.DrawImage(I_fn.Image, destRect, srcRect, GraphicsUnit.Pixel);
					}
				}
			}
			i_view.Refresh();
		}

		// redraws whole font area
		public void RedrawSet()
		{
			var img = GetImage(I_fn);
			using (var gr = Graphics.FromImage(img))
			{

				byte a = 0;
				byte b = 0;
				byte c = 0;
				byte d = 0;

				byte[] ou = new byte[8];
				byte[] ou2 = new byte[8];
				byte[] ov = new byte[4];
				byte[] ov2 = new byte[4];

				if (!gfx)
				{
					//gr.0:
					//var brush = new SolidBrush(palette[cpal[1]]);
					var brush = cpalBrushes[1];
					gr.FillRectangle(brush, 0, 0, 512, I_fn.Height);
					//I_fn.Canvas.Brush.Color = palette[cpal[1]];
					//I_fn.Canvas.FillRect(bounds(0, 0, 512, I_fn.Height));
					//brush = new SolidBrush(palette[cpal[0]]);
					brush = cpalBrushes[0];
					//I_fn.Canvas.Brush.Color = palette[cpal[0]];

					for (d = 0; d < 4; d++)
					{
						for (a = 0; a < 32; a++)
						{
							for (b = 0; b < 8; b++)
							{
								ou = MainUnit.DecodeBW(ft[a * 8 + b + d * 32 * 8]);
								ou2 = MainUnit.DecodeBW(ft[a * 8 + b + d * 32 * 8 + 1024]);

								for (c = 0; c <= 7; c++)
								{
									if (ou[c] == 1)
									{
										//I_fn.Canvas.fillrect(bounds(a * 16 + c * 2, b * 2 + d * 16, 2, 2));
										gr.FillRectangle(brush, a * 16 + c * 2, b * 2 + d * 16, 2, 2);
									}
									else
									{
										//I_fn.Canvas.fillrect(bounds(a * 16 + c * 2, b * 2 + d * 16 + 64, 2, 2));
										gr.FillRectangle(brush, a * 16 + c * 2, b * 2 + d * 16 + 64, 2, 2);
									}

									if (ou2[c] == 1)
									{
										//I_fn.Canvas.fillrect(bounds(a * 16 + c * 2, b * 2 + d * 16 + 128, 2, 2));
										gr.FillRectangle(brush, a * 16 + c * 2, b * 2 + d * 16 + 128, 2, 2);
									}
									else
									{
										//I_fn.Canvas.fillrect(bounds(a * 16 + c * 2, b * 2 + d * 16 + 192, 2, 2));
										gr.FillRectangle(brush, a * 16 + c * 2, b * 2 + d * 16 + 192, 2, 2);
									}
								}
							}
						}
					}
				}
				else
				{
					for (d = 0; d < 4; d++)
					{
						for (a = 0; a < 32; a++)
						{
							for (b = 0; b < 8; b++)
							{
								ov = MainUnit.DecodeCL(ft[a * 8 + b + d * 32 * 8]);
								ov2 = MainUnit.DecodeCL(ft[a * 8 + b + d * 32 * 8 + 1024]);

								for (c = 0; c < 4; c++)
								{
									var fb = ov[c] + 1;

									//var brush = new SolidBrush(palette[cpal[fb]]);
									var brush = cpalBrushes[fb];
									gr.FillRectangle(brush, a * 16 + c * 4, b * 2 + d * 16, 4, 2);
									//I_fn.Canvas.Brush.Color = palette[cpal[fb]];
									//I_fn.Canvas.fillrect(bounds(a * 16 + c * 4, b * 2 + d * 16, 4, 2));

									if (fb == 4)
									{
										//brush = new SolidBrush(palette[cpal[5]]);
										brush = cpalBrushes[5];
										//I_fn.Canvas.Brush.Color = palette[cpal[5]];
									}

									gr.FillRectangle(brush, a * 16 + c * 4, b * 2 + d * 16 + 64, 4, 2);
									//I_fn.Canvas.fillrect(bounds(a * 16 + c * 4, b * 2 + d * 16 + 64, 4, 2));

									fb = ov2[c] + 1;
									//brush = new SolidBrush(palette[cpal[fb]]);
									brush = cpalBrushes[fb];
									gr.FillRectangle(brush, a * 16 + c * 4, b * 2 + d * 16 + 128, 4, 2);
									//I_fn.Canvas.Brush.Color = palette[cpal[fb]];
									//I_fn.Canvas.fillrect(bounds(a * 16 + c * 4, b * 2 + d * 16 + 128, 4, 2));

									if (fb == 4)
									{
										//brush = new SolidBrush(palette[cpal[5]]);
										brush = cpalBrushes[5];
										//I_fn.Canvas.Brush.Color = palette[cpal[5]];
									}

									gr.FillRectangle(brush, a * 16 + c * 4, b * 2 + d * 16 + 192, 4, 2);
									//I_fn.Canvas.fillrect(bounds(a * 16 + c * 4, b * 2 + d * 16 + 192, 4, 2));
								}
							}
						}
					}
				}
			}
			I_fn.Refresh();
		}

		public void Load_font(string filename, int number)
		{

			var expSize = 0;

			if (number == 2)
			{
				expSize = 2048;
			}
			else
			{
				expSize = 1024;
			}

			var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
			var size = fs.Length;
			var loadSize = Math.Min(expSize, size);

			try
			{
				if (number == 2)
				{
					fs.Read(ft, 0, (int)loadSize);
				}
				else
				{
					fs.Read(ft, (int)(number * loadSize), (int)loadSize);
				}
			}
			finally
			{
				fs.Close();
				fs = null;
			}

			if (loadSize != expSize)
			{
				MessageBox.Show($"File size different ({size}) than expected ({expSize}), {loadSize} bytes read.");
			}

			CheckDuplicate();
		}

		public void Save_font(string filename, int number)
		{
			var fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.Write);

			try
			{
				fs.Write(ft, number * 1024, 1024);
			}
			finally
			{
				fs.Close();
				fs = null;
			}
		}

		public void b_colorSwSetupClick(object sender, EventArgs e)
		{
			p_color_switch.Visible = !p_color_switch.Visible;
		}


		public void lb_cs1Click(object sender, EventArgs e)
		{
			var img = GetImage(i_rec1);
			using (var gr = Graphics.FromImage(img))
			{
				//var brush = new SolidBrush(palette[cpal[lb_cs1.SelectedIndex + 1]]);
				var brush = cpalBrushes[lb_cs1.SelectedIndex + 1];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				drawTxt(gr, 1, 1, lb_cs1.SelectedIndex + 2, palette[cpal[lb_cs1.SelectedIndex + 1]]);
			}
			/*
			i_rec1.Canvas.Brush.Color = palette[cpal[lb_cs1.SelectedIndex + 1]];
			i_rec1.Canvas.FillRect(bounds(0, 0, 49, 17));
			MainUnit.drawTxt(i_rec1, 1, 1, lb_cs1.SelectedIndex + 2, Convert.ToInt32(palette[cpal[lb_cs1.SelectedIndex + 1]]));
			*/
			i_rec1.Refresh();
		}



		public void lb_cs2Click(object sender, EventArgs e)
		{
			var img = GetImage(i_rec2);
			using (var gr = Graphics.FromImage(img))
			{
				//var brush = new SolidBrush(palette[cpal[lb_cs2.SelectedIndex + 1]]);
				var brush = cpalBrushes[lb_cs2.SelectedIndex + 1];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				drawTxt(gr, 1, 1, lb_cs2.SelectedIndex + 2, palette[cpal[lb_cs2.SelectedIndex + 1]]);
			}
			/*
			i_rec2.Canvas.Brush.Color = palette[cpal[lb_cs2.SelectedIndex + 1]];
			i_rec2.Canvas.FillRect(bounds(0, 0, 49, 17));
			MainUnit.drawTxt(i_rec2, 1, 1, lb_cs2.SelectedIndex + 2, Convert.ToInt32(palette[cpal[lb_cs2.SelectedIndex + 1]]));
			*/
			i_rec2.Refresh();
		}

		public void SetCharCursor()
		{
			if (CharacterEdited())
			{
				Add2Undo(true);
			}

			selectedCharacterIndex = selectedCharacterIndex % 512;
			var bx = selectedCharacterIndex % 32;
			var by = selectedCharacterIndex / 32;
			I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0));
		}

		public byte GetActualViewWidth()
		{
			byte getActualViewWidth_result = 0;

			if (CheckBox40bytes.Checked)
			{
				getActualViewWidth_result = 40;
			}
			else
			{
				getActualViewWidth_result = 32;
			}

			return getActualViewWidth_result;
		}


		//compare two chars within 1 font
		public bool CompareChars(int c1, int c2, int fontNr)
		{
			bool compareChars_result = false;
			int p1 = 0;
			int p2 = 0;
			int i = 0;
			bool diff = false;
			p1 = (c1 % 128) * 8 + 1024 * fontNr;
			p2 = (c2 % 128) * 8 + 1024 * fontNr;
			diff = false;
			i = 7;

			do
			{
				if (ft[p1 + i] != ft[p2 + i])
				{
					diff = true;
				}

				i--;
			}
			while (!((i < 0) || (diff == true)));

			compareChars_result = !diff;
			return compareChars_result;
		}

		//finds nearest duplicate char to duplicateCharacterIndex
		public int FindDuplicateChar()
		{
			var myChar = selectedCharacterIndex % 256;
			var fontNr = selectedCharacterIndex / 256;
			var inverse = myChar / 128;

			var I = duplicateCharacterIndex + 1;
			I = I % 128 + 128 * inverse;
			var found = false;

			while ((!found) && (I + fontNr * 256 != duplicateCharacterIndex))
			{
				if (CompareChars(I, selectedCharacterIndex, fontNr) && (I + fontNr * 256 != selectedCharacterIndex))
				{
					found = true;
				}

				I++;
				I = I % 128 + 128 * inverse;
			}

			var res = I - 1 + fontNr * 256;

			if (!found)
			{
				res = selectedCharacterIndex;
			}

			return res;
		}

		public bool MouseValidView(int X, int Y)
		{
			if ((X >= i_view.Width - (copyRange.Width) * 16) || (Y >= i_view.Height - (copyRange.Height) * 16))
			{
				return false;
			}
			else
			{
				return true;
			}

			return false;
		}

		public bool MouseValidFont(int X, int Y)
		{
			if ((X >= I_fn.Width - (copyRange.Width) * 16) || (Y >= I_fn.Height - (copyRange.Height) * 16))
			{
				return false;
			}
			else
			{
				return true;
			}

			return false;
		}

		public void I_fnMouseDown(object sender, MouseEventArgs e)
		{
			int fontchar = 0;
			int fontnr = 0;

			if (e.X < 0 || e.X >= I_fn.Width || e.Y < 0 || e.Y >= I_fn.Height)
			{
				return;
			}

			if (!SpeedButtonMegaCopy.Checked)
			{
				if (CharacterEdited())
				{
					Add2Undo(true);
				}
			}

			var rx = e.X / 16;
			var ry = e.Y / 16;
			selectedCharacterIndex = rx + ry * 32;

			if (selectedCharacterIndex > 255)
			{
				fontchar = selectedCharacterIndex % 256;
				fontnr = 2;
			}
			else
			{
				fontchar = selectedCharacterIndex;
				fontnr = 1;
			}

			if (SpeedButtonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case TMegaCopyStatus.None:
					case TMegaCopyStatus.Selected:
						{
							if (e.Button == MouseButtons.Left)
							{
								//define copy origin point
								copyRange.Y = ry;
								copyRange.X = rx;
								megaCopyStatus = TMegaCopyStatus.Selecting;
								Shape1.Left = I_fn.Left + e.X - e.X % 16 - 2;
								Shape1.Top = I_fn.Top + e.Y - e.Y % 16 - 2;
								Shape1.Width = 20;
								Shape1.Height = 20;
								Shape1.Visible = true;
								Shape1v.Visible = false;
							}
						}
						break;

					case TMegaCopyStatus.Pasting:
						{
							if (!MouseValidFont(e.X, e.Y))
							{
								return;
							}

							if (e.Button == MouseButtons.Left)
							{
								copyTarget = new Point(rx, ry);
								Add2UndoFullDifferenceScan();
								Clipboard_pasteExecute(sender, false);
								ResetMegaCopyStatus();
							}

							// reset selection by right doubleclick
							/* TODO:
							if ((Shift.Contains(ssDouble)) && (Shift.Contains(ssRight)))
							{
								ResetMegaCopyStatus();
							}
							*/
						}
						break;
				}
			}
			else
			{
				Shape1.Left = I_fn.Bounds.Left + e.X - e.X % 16 - 2;
				Shape1.Top = I_fn.Bounds.Top + e.Y - e.Y % 16 - 2;

				copyRange.Y = ry;
				copyRange.X = rx;
				copyRange.Width = 0;
				copyRange.Height = 0;
				l_char.Text = $@"Char: Font {fontnr} ${fontchar:X2} #{fontchar}";
				RedrawChar();
				CheckDuplicate();
			}
		}

		public void I_fnMouseUp(object sender, MouseEventArgs e)
		{
			int rx = 0;
			int ry = 0;

			if ((e.X >= I_fn.Width) || (e.Y >= I_fn.Height))
			{
				return;
			}

			//x := x mod I_fn.Width;
			//y := y mod I_fn.Height;
			rx = e.X / 16;
			ry = e.Y / 16;

			if (SpeedButtonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case TMegaCopyStatus.Selecting:
						{
							if (ry <= copyRange.Y)
							{
								copyRange.Height = 0;
							}
							else
							{
								copyRange.Height = ry - copyRange.Y;
							}

							if (rx <= copyRange.X)
							{
								copyRange.Width = 0;
							}
							else
							{
								copyRange.Width = rx - copyRange.X;
							}

							megaCopyStatus = TMegaCopyStatus.Selected;
							//Clipboard_copyExecute(sender);
							/*shape2.Left := i_fn.left + x - x mod 16 - 2;
							shape2.Top := i_fn.Top + y - y mod 16 - 2;
							Shape2.Width := Shape1.Width;
							Shape2.Height := Shape1.Height;
							Shape2.Visible := True;
							*/
						}
						break;
				}
			}
		}

		public void I_fnMouseMove(object sender, MouseEventArgs e)
		{
			int rx = 0;
			int ry = 0;

			/*memo1.Text := inttostr(copyrange.left) + '-' + inttostr(copyrange.Right) +
			    ' : ' + inttostr(copyRange.Width) + sLineBreak + inttostr(copyrange.top) + '-' + inttostr(copyrange.Bottom) +
			    ' : ' + inttostr(copyRange.Height);
			 */
			if (SpeedButtonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case TMegaCopyStatus.Selecting:
						{
							if (e.X < 0 || e.X >= I_fn.Width || e.Y < 0 || e.Y >= I_fn.Height)
							{
								return;
							}

							rx = e.X / 16;
							ry = e.Y / 16;

							int origWidth = Shape1.Width;
							int origHeight = Shape1.Height;

							int w = 20;
							int h = 20;
							var temp = (rx - copyRange.X + 1) * 16 + 4;
							if (temp >= 20)
								w = temp;

							temp = (ry - copyRange.Y + 1) * 16 + 4;
							if (temp >= 20)
								h = temp;

							if (w != origWidth || h != origHeight)
							{
								Shape1.Size = new Size(w, h);
							}
						}
						break;

					case TMegaCopyStatus.Pasting:
						{
							if (!MouseValidFont(e.X, e.Y))
							{
								Shape2.Visible = false;
								ImageMegacopy.Visible = false;
								return;
							}

							Shape2.Left = I_fn.Left + e.X - e.X % 16 - 2;
							Shape2.Top = I_fn.Top + e.Y - e.Y % 16 - 2;
							ImageMegacopy.Left = Shape2.Left + 2;
							ImageMegacopy.Top = Shape2.Top + 2;
							Shape2.Visible = true;
							ImageMegacopy.Visible = true;
							Shape2v.Visible = false;
							ImageMegaCopyV.Visible = false;
						}
						break;
				}
			}
		}

		private static bool inShape1Resize = false;

		private void Shape1_Resize(object sender, EventArgs e)
		{
			if (inShape1Resize)
				return;
			inShape1Resize = true;

			var img = NewImage(Shape1);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(redBrush, new Rectangle(0, 0, img.Width, img.Height));
				Shape1.Region?.Dispose();
				Shape1.Size = new Size(img.Width, img.Height);

			}
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new Rectangle(0, 0, Shape1.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(Shape1.Width - 2, 0, 2, Shape1.Height));
			graphicsPath.AddRectangle(new Rectangle(0, Shape1.Height - 2, Shape1.Width, 2));
			graphicsPath.AddRectangle(new Rectangle(0, 0, 2, Shape1.Height));
			Shape1.Region = new Region(graphicsPath);

			inShape1Resize = false;
		}

		public void drawTxt(Graphics ic, int x, int y, int num, Color color)
		{
			string[] texty = new string[] { "LUM", "BAK - 00", "PF0 - 01", "PF1 - 10", "PF2 - 11", "PF3 - 11" };
			ic.DrawString(texty[num], this.Font, color.G > 128 ? blackBrush : whiteBrush, x, y);
		}

		/// <summary>
		/// Some buttons are not available in mode 0, others not in mode 4
		/// </summary>
		public void RevalidateCharButtons()
		{
			if (!gfx)
			{
				b_colorSwitch.Enabled = false;
				b_colorSwSetup.Enabled = false;

				foreach (var item in ActionListNormalModeOnly)
				{
					var tag = (int)(item.Tag ?? 0);
					if (tag == 2)
						item.Enabled = true;
				}
			}
			else
			{
				b_colorSwitch.Enabled = true;
				b_colorSwSetup.Enabled = true;


				foreach (var item in ActionListNormalModeOnly)
				{
					var tag = (int)(item.Tag ?? 0);
					if (tag == 2)
						item.Enabled = false;
				}
			}
		}

		public void LoadViewFile(string filename, bool forceLoadFont = false)
		{
			string[] filenames = new string[2];
			byte viewWidth = 0;

			try
			{
				var jtext = File.ReadAllText(filename, Encoding.UTF8);

				var jsonObj = jtext.FromJson<AtrViewInfoJSON>();

				Int32.TryParse(jsonObj.Version, out var version);
				Int32.TryParse(jsonObj.ColoredGfx, out var coloredGfx);

				if (version >= 1911)
				{
					// Take out the values from the parsed JSON container
					var characterBytes = jsonObj.Chars;
					var lineTypes = jsonObj.Lines;
					var colors = jsonObj.Colors;
					var fontBytes = jsonObj.Data;
					filenames[0] = jsonObj.Fontname1;
					filenames[1] = jsonObj.Fontname2;

					if (version < 2007)
					{
						viewWidth = 32;
						fortyBytes = "0";
					}
					else
					{
						viewWidth = 40;
						fortyBytes = jsonObj.FortyBytes;
					}

					chsline = Convert.FromHexString(lineTypes);
					for (var i = 0; i < chsline.Length; i++)
						++chsline[i];

					var bytes = Convert.FromHexString(characterBytes);
					var idx = 0;
					for (var y = 0; y < VIEW_HEIGHT; ++y)
					{
						for (var x = 0; x < viewWidth; ++x)
						{
							vw[x, y] = bytes[idx];
							++idx;
						}
					}
					// Load the palette selection
					cpal = Convert.FromHexString(colors);
					BuildBrushCache();

					// If the GFX (mode 0 or 4) does not match then hit the GFX button to switch the mode of the GUI
					if (gfx != (coloredGfx == 1))
					{
						b_gfxClick(null, EventArgs.Empty);
					}

					if ((forceLoadFont) || (MessageBox.Show("Would you like to load fonts embedded in this view file?", "Load embedded fonts", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes))
					{
						fname1 = filenames[0];
						fname2 = filenames[1];

						ft = Convert.FromHexString(fontBytes);

						UpdateFormCaption();
						Add2UndoFullDifferenceScan(); //full font scan
					}

					// 40bytes
					if (((fortyBytes == "1") && (CheckBox40bytes.Checked == false))
						|| ((fortyBytes == "0") && (CheckBox40bytes.Checked == true)))
					{
						CheckBox40bytes.Checked = !CheckBox40bytes.Checked;
						CheckBox40bytesClick(null, null);
					}
				}

				CheckDuplicate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);

				// Set some defaults!
				default_pal();
				fname1 = "default.fnt";
				fname2 = "default.fnt";
			}

			// Make sure the cpal values have valid brushes for painting!
			BuildBrushCache();
		}

		// save view file new edition
		public void SaveViewFile(string filename)
		{
			var jo = new AtrViewInfoJSON();

			var characterBytes = string.Empty;

			//version
			jo.Version = "2007";
			//gfxmode
			jo.ColoredGfx = gfx ? "1" : "0";

			//characters
			for (var i = 0; i < VIEW_HEIGHT; i++)
			{
				for (var j = 0; j < VIEW_WIDTH; j++)
				{
					characterBytes = characterBytes + String.Format("{0:X2}", vw[j, i]);
				}
			}

			jo.Chars = characterBytes;

			//line types
			jo.Lines = Convert.ToHexString(chsline);

			//colors
			jo.Colors = Convert.ToHexString(cpal);
			//fontnames
			jo.Fontname1 = fname1;
			jo.Fontname2 = fname2;

			//fontdata
			jo.Data = Convert.ToHexString(ft);

			//width selection
			if (GetActualViewWidth() == 40)
			{
				jo.FortyBytes = "1";
			}
			else
			{
				jo.FortyBytes = "0";
			}

			var txt = jo.ToJson();

			File.WriteAllText(filename, txt, Encoding.UTF8);
		}

		public void UpdateFormCaption()
		{
			MainUnit.MainForm.Text = MainUnit.TITLE + " v" + MainUnit.GetBuildInfoAsString() + " - " + Path.GetFileName(fname1) + "/" + Path.GetFileName(fname2);
			b_save1.Visible = true;
			b_save2.Visible = true;
		}

		public void Clipboard_copyExecute(object sender, EventArgs e)
		{
			Clipboard_copyExecute(sender, false);
		}

		public void Clipboard_copyExecute(object sender, bool sourceIsView)
		{
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;
			var charInFont = 0;

			/*memo1.Text := inttostr(copyrange.left) + '-' + inttostr(copyrange.Right) +
			  ' : ' + inttostr(copyRange.Width) + sLineBreak + inttostr(copyrange.top) + '-' + inttostr(copyrange.Bottom) +
			  ' : ' + inttostr(copyRange.Height);
			 */
			if ((SpeedButtonMegaCopy.Checked && (megaCopyStatus == TMegaCopyStatus.Selected)) || (!SpeedButtonMegaCopy.Checked))
			{
				if (Shape1.Visible)
				{
					sourceIsView = false;
				}

				if (Shape1v.Visible)
				{
					sourceIsView = true;
				}

				if (!Shape1.Visible && !Shape1v.Visible)
				{
					return;
				}

				for (var i = copyRange.Y; i <= copyRange.Bottom; i++)
				{
					for (var j = copyRange.X; j <= copyRange.Right; j++)
					{
						if (sourceIsView)
						{
							characterBytes = characterBytes + String.Format("{0:X2}", vw[j, i]);
							charInFont = (vw[j, i] % 128) * 8 + (chsline[i] - 1) * 1024;
						}
						else
						{
							characterBytes = characterBytes + String.Format("{0:X2}", (i * 32 + j) % 256);

							if (i / 8 == 0)
							{
								charInFont = ((i % 4) * 32 + j) * 8;
							}
							else
							{
								charInFont = ((i % 4 + 4) * 32 + j) * 8;
							} //second font
						}

						for (var k = 0; k < 8; k++)
						{
							fontBytes = fontBytes + String.Format("{0:X2}", ft[charInFont + k]);
						}
					}
				}

				var jo = new ClipboardJSON()
				{
					Width = (copyRange.Width + 1).ToString(),
					Height = (copyRange.Height + 1).ToString(),
					Chars = characterBytes,
					Data = fontBytes,
				};
				var json = jo.ToJson();
				Clipboard.SetText(json);
				clipboardLocal = json;
			}
		}

		public void Clipboard_copyText(string text, bool inverse)
		{
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;

			copyRange.X = 0;
			copyRange.Y = 0;
			copyRange.Width = text.Length;
			copyRange.Height = 1;

			var chars = text.ToCharArray();
			for (var j = 0; j < text.Length; j++)
			{
				var character = AtariConvertChar((byte)chars[j]);

				if (inverse == true)
				{
					character = (byte)(character | 128);
				}

				characterBytes = characterBytes + $"{character:X2}";
				var charInFont = character * 8;

				for (var k = 0; k < 8; k++)
				{
					fontBytes = fontBytes + $"{ft[charInFont + k]:X2}";
				}
			}

			var jo = new ClipboardJSON()
			{
				Width = text.Length.ToString(),
				Height = "1",
				Chars = characterBytes,
				Data = fontBytes,
			};
			var json = jo.ToJson();
			Clipboard.SetText(json);
			clipboardLocal = json;
		}

		public void b_pstClick(object sender, EventArgs e)
		{
			if (SpeedButtonMegaCopy.Checked)
			{
				if (Clipboard.GetText() != clipboardLocal)
				{
					Shape1.Visible = false;
					Shape1v.Visible = false;
				}

				RevalidateClipboard();
				megaCopyStatus = TMegaCopyStatus.Pasting;
			}
			else
			{
				Clipboard_pasteExecute(sender, false);
			}
		}

		public void Clipboard_pasteExecute(object sender, EventArgs e)
		{
			Clipboard_pasteExecute(sender, false);
		}

		public void Clipboard_pasteExecute(object sender, bool targetIsView)
		{
			var width = 0;
			var height = 0;
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;

			int charInFont = 0;

			try
			{
				var jtext = Clipboard.GetText();
				var jsonObj = jtext.FromJson<ClipboardJSON>();
				Int32.TryParse(jsonObj.Width, out width);
				Int32.TryParse(jsonObj.Height, out height);

				characterBytes = jsonObj.Chars;
				fontBytes = jsonObj.Data;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Clipboard data parsing error");
				return;
			}

			if (SpeedButtonMegaCopy.Checked)
			{
				if (targetIsView)
				{
					var charsBytes = Convert.FromHexString(characterBytes);
					for (var ii = 0; ii < height; ii++)
					{
						for (var jj = 0; jj < width; jj++)
						{
							var i = ii + copyTarget.Y;
							var j = jj + copyTarget.X;
							vw[j, i] = charsBytes[ii * width + jj];
						}
					}

					RedrawView();
				}
				else
				{
					var charsBytes = Convert.FromHexString(fontBytes);
					for (var ii = 0; ii < height; ii++)
					{
						for (var jj = 0; jj < width; jj++)
						{
							var i = ii + copyTarget.Y;
							var j = jj + copyTarget.X;
							selectedCharacterIndex = i * 32 + j;

							if (i / 8 == 0)
							{
								charInFont = ((i % 4) * 32 + j) * 8;
							}
							else
							{
								charInFont = ((i % 4 + 4) * 32 + j) * 8;
							} //second font

							for (var k = 0; k < 8; k++)
							{
								ft[charInFont + k] = charsBytes[(ii * width + jj) * 8 + k];
							}

							//SetCharCursor;
							DoChar();
							RedrawChar();
							RedrawViewChar();
						}
					}

					Add2UndoFullDifferenceScan();
				}
			}
			else
			{
				if (width + height > 2)
				{
					MessageBox.Show($@"Unable to paste clipboard outside MegaCopy mode. Clipboard contains {width}x{height} data.");
					return;
				}

				var hp = MainUnit.GetCharacterPointer(selectedCharacterIndex);

				var bytes = Convert.FromHexString(fontBytes);
				for (var i = 0; i < 8; i++)
				{
					ft[hp + i] = bytes[i];
				}

				SetCharCursor();
				DoChar();
				RedrawChar();
				RedrawViewChar();
			}

			CheckDuplicate();
		}

		static string[] ToDraw = new string[] { null, "1", "2" };

		/// <summary>
		/// Draw the font indicator next to the sample screen.
		/// Column of 26x 1 or 2 font indicators
		/// </summary>
		public void RedrawLineTypes()
		{
			var img = GetImage(i_chset);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(whiteBrush, 0, 0, img.Width, img.Height);
				for (var a = 0; a < VIEW_HEIGHT; a++)
				{
					// i_chset.Canvas.TextOut(4, 2 + a * 16, Convert.ToString(chsline[a]));
					gr.DrawString(ToDraw[chsline[a]], this.Font, blackBrush, 4, 2 + a * 16);
				}
			}
			i_chset.Refresh();
		}

		// Redraw all occurrences of character (selectedCharacterIndex) in view area
		public void RedrawViewChar()
		{
			var img = GetImage(i_view);
			using (var gr = Graphics.FromImage(img))
			{
				var destRect = new Rectangle
				{
					Width = 16,
					Height = 16,
				};

				var rx = selectedCharacterIndex % 32;
				var ry = selectedCharacterIndex / 32;

				var srcRect = new Rectangle
				{
					X = rx * 16,
					Y = ry * 16,        // Will be set below
					Width = 16,
					Height = 16,
				};

				for (var b = 0; b < VIEW_HEIGHT; b++)
				{
					ry = (ry | 8);

					if (chsline[b] == 1)
					{
						ry = (ry ^ 8);
					}

					var ny = ry ^ 4; //ny checks invert notes sign (ry)
					var ep = (rx + ry * 32) % 256;
					var dp = (rx + ny * 32) % 256;

					for (var a = 0; a < VIEW_WIDTH; a++)
					{
						destRect.X = a * 16;
						destRect.Y = b * 16;

						if (vw[a, b] == (byte)ep)
						{
							srcRect.Y = ry * 16;
							gr.DrawImage(I_fn.Image, destRect, srcRect, GraphicsUnit.Pixel);
							//i_view.Canvas.CopyRect(bounds(a * 16, b * 16, 16, 16), I_fn.Canvas, bounds(rx * 16, ry * 16, 16, 16));

						}

						if (vw[a, b] == (byte)dp)
						{
							srcRect.Y = ny * 16;
							gr.DrawImage(I_fn.Image, destRect, srcRect, GraphicsUnit.Pixel);
							//i_view.Canvas.CopyRect(bounds(a * 16, b * 16, 16, 16), I_fn.Canvas, bounds(rx * 16, ny * 16, 16, 16));
						}
					}
				}
			}
			i_view.Refresh();
		}

		public void grid()
		{
			var img = GetImage(i_ch);
			using (var gr = Graphics.FromImage(img))
			{
				var brush = cpalBrushes[1];
				gr.FillRectangle(brush, 0, 0, i_ch.Width, i_ch.Height);

				for (var b = 0; b < 8; b++)
				{
					for (var a = 0; a < 8; a++)
					{
						//i_ch.Canvas.Pixels[a * 20, b * 20] = Color.White;
						gr.FillRectangle(whiteBrush, a * 20, b * 20, 1, 1);
					}
				}
			}
			i_ch.Refresh();
		}

		public void Add2Undo(bool difference)
		{
			if (difference)
			{
				var prevUndoIndex = undoBufferIndex;
				undoBufferIndex = (undoBufferIndex + 1) % UNDOBUFFERSIZE; //size of undo buffer

				for (var i = 0; i < 2048; i++)
				{
					undoBuffer[undoBufferIndex, i] = ft[i];
				}

				undoBufferFlags[undoBufferIndex] = undoBufferFlags[prevUndoIndex] + 1;
				undoBufferFlags[(undoBufferIndex + 1) % UNDOBUFFERSIZE] = -1; //disallow redo when change
			}
		}

		public void Add2UndoInitial()
		{
			int prevUndoIndex = undoBufferIndex;

			for (var i = 0; i < 2048; i++)
			{
				undoBuffer[undoBufferIndex, i] = ft[i];
			}

			undoBufferFlags[undoBufferIndex] = undoBufferFlags[prevUndoIndex] + 1;
			undoBufferFlags[(undoBufferIndex + 1) % UNDOBUFFERSIZE] = -1; //disallow redo when change
		}

		public void Add2UndoFullDifferenceScan()
		{
			bool difference = false;
			int i = 0;
			difference = false;
			i = 0;

			//check the difference between last undobuffer and current font
			while ((i < 2048) && (!difference))
			{
				if (ft[i] != undoBuffer[undoBufferIndex, i])
				{
					difference = true;
				}

				i++;
			}

			if (difference)
			{
				Add2Undo(true);
			}

			UpdateUndoButtons(false);
		}

		public bool Undo()
		{
			//  nextUndoIndex := (undoBufferIndex + 1) mod UNDOBUFFERSIZE;
			if (CharacterEdited())
			{
				Add2Undo(true); //add 2 undo but dont change index
			}

			var prevUndoIndex = GetPrevUndoIndex();

			/*  down := 1;
			if characterEdited then begin;Add2Undo(false);down := 2;end;

			undoBufferIndex := (undoBufferIndex - down) mod UNDOBUFFERSIZE;
			*/
			for (var i = 0; i < 2048; i++)
			{
				ft[i] = undoBuffer[prevUndoIndex, i];
			}

			undoBufferIndex = prevUndoIndex;
			UpdateUndoButtons(CharacterEdited());
			RedrawChar();
			RedrawSet();
			RedrawView();
			return true;
		}

		public bool Redo()
		{
			var nextUndoIndex = (undoBufferIndex + 1) % UNDOBUFFERSIZE;

			if (undoBufferFlags[nextUndoIndex] > -1)
			{
				for (var i = 0; i < 2048; i++)
				{
					ft[i] = undoBuffer[nextUndoIndex, i];
				}
			}

			undoBufferIndex = nextUndoIndex;
			UpdateUndoButtons(CharacterEdited());
			RedrawChar();
			RedrawSet();
			RedrawView();
			return true;
		}

		public int GetPrevUndoIndex()
		{
			int getPrevUndoIndex_result = 0;

			if (undoBufferIndex - 1 < 0)
			{
				getPrevUndoIndex_result = UNDOBUFFERSIZE - 1;
			}
			else
			{
				getPrevUndoIndex_result = undoBufferIndex - 1;
			}

			return getPrevUndoIndex_result;
		}

		//updates undo/redo button state based on info if character has been edited and whats the buffer index
		public void UpdateUndoButtons(bool edited)
		{
			int nextUndoBufferIndex = 0;
			int prevUndoBufferIndex = 0;
			nextUndoBufferIndex = (undoBufferIndex + 1) % UNDOBUFFERSIZE;
			prevUndoBufferIndex = GetPrevUndoIndex();

			//redo button handling
			if (undoBufferFlags[nextUndoBufferIndex] == -1)
			{
				SpeedButtonRedo.Enabled = false;
			}
			else if (edited)
			{
				SpeedButtonRedo.Enabled = false;
			}
			else
			{
				SpeedButtonRedo.Enabled = true;
			}

			//undo button handling
			if (edited)
			{
				SpeedButtonUndo.Enabled = true;
			}
			else if ((undoBufferFlags[undoBufferIndex] > undoBufferFlags[prevUndoBufferIndex]) && (undoBufferFlags[prevUndoBufferIndex] > -1))
			{
				SpeedButtonUndo.Enabled = true;
			}
			else
			{
				SpeedButtonUndo.Enabled = false;
			}
		}

		// returns true if character has been edited (different to last saved in undo buffer)
		public bool CharacterEdited()
		{
			bool characterEdited_result = false;
			byte i = 0;
			// var ptr = selectedCharacterIndex * 8;
			var ptr = MainUnit.GetCharacterPointer(selectedCharacterIndex);

			while ((i < 8) && (characterEdited_result == false))
			{
				characterEdited_result = ft[ptr + i] != undoBuffer[undoBufferIndex, ptr + i];
				i++;
			}

			return characterEdited_result;
		}

		public byte AtariConvertChar(byte character)
		{
			if (character == 32)
			{
				return 0;
			}

			if ((character >= 48) && (character <= 90))
			{
				return (byte)(character - 32);
			}

			return character;
		}

		public void ImageMegaCopyMouseMove(object sender, MouseEventArgs e)
		{
			I_fnMouseMove(sender, new MouseEventArgs(MouseButtons.None, 0, e.X + ImageMegacopy.Left - I_fn.Left, e.Y + ImageMegacopy.Top - I_fn.Top, 0));
		}



		public void ImageMegaCopyMouseDown(object sender, MouseEventArgs e)
		{
			I_fnMouseDown(sender, new MouseEventArgs(e.Button, 0, e.X + ImageMegacopy.Left - I_fn.Left, e.Y + ImageMegacopy.Top - I_fn.Top, 0));
		}

		public void ImageMegaCopyVMouseMove(object sender, MouseEventArgs e)
		{
			i_viewMouseMove(sender, new MouseEventArgs(MouseButtons.None, 0, e.X + ImageMegaCopyV.Left - i_view.Left, e.Y + ImageMegaCopyV.Top - i_view.Top, 0));
		}



		public void ImageMegaCopyVMouseDown(object sender, MouseEventArgs e)
		{
			i_viewMouseDown(sender, new MouseEventArgs(e.Button, 0, e.X + ImageMegaCopyV.Left - i_view.Left, e.Y + ImageMegaCopyV.Top - i_view.Top, 0));
		}


		public void ResetMegaCopyStatus()
		{
			switch (megaCopyStatus)
			{
				case TMegaCopyStatus.None:
					break;

				case TMegaCopyStatus.Selecting:
					{
						megaCopyStatus = TMegaCopyStatus.None;
						//font window
						Shape1.Bounds = new Rectangle(-30, 0, 20, 20);
						//view window
						Shape1v.Bounds = new Rectangle(-30, 0, 20, 20);
						break;
					}

				case TMegaCopyStatus.Selected:
					break;

				case TMegaCopyStatus.Pasting:
					{
						megaCopyStatus = TMegaCopyStatus.Selected;
						Shape2.Visible = false;
						ImageMegacopy.Visible = false;
						Shape2v.Visible = false;
						ImageMegaCopyV.Visible = false;
					}
					break;
			}
		}

		private void b_shuClick(object sender, EventArgs e)
		{
			var tmp = new byte[8, 8];

			int h = 0;
			var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 8; b++)
				{
					if (a < 7)
					{
						h = a + 1;
					}
					else
					{
						h = 0;
					}

					tmp[b, a] = src[b, h];
				}
			}

			MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex);
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		public void b_shrClick(object sender, EventArgs e)
		{
			var tmp = new byte[8, 8];
			int repeatTime = repeatTime = gfx ? 1 : 0;  // perform same shift twice in mode 4
			var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex);

			for (var i = 0; i <= repeatTime; i++)
			{
				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 8; b++)
					{
						int h;
						if (b > 0)
						{
							h = b - 1;
						}
						else
						{
							h = 7;
						}

						tmp[b, a] = src[h, a];
					}
				}

				if (repeatTime == 1)
					src = tmp.Clone() as byte[,];
			}

			MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex);
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		private void Shift_leftExecute(object sender, EventArgs e)
		{
			var tmp = new byte[8, 8];
			int repeatTime = repeatTime = gfx ? 1 : 0;   // perform same shift twice in mode 4
			var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex);

			for (var i = 0; i <= repeatTime; i++)
			{
				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 8; b++)
					{
						int h;
						if (b < 7)
						{
							h = b + 1;
						}
						else
						{
							h = 0;
						}

						tmp[b, a] = src[h, a];
					}
				}

				if (repeatTime == 1)
					src = tmp.Clone() as byte[,];
			}

			MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex);
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		public void b_shdClick(object sender, EventArgs e)
		{
			var tmp = new byte[8, 8];
			var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 8; b++)
				{
					var h = 0;
					if (a > 0)
					{
						h = a - 1;
					}
					else
					{
						h = 7;
					}

					tmp[b, a] = src[b, h];
				}
			}

			MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex);
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		public void Rotate_rightExecute(object sender, EventArgs e)
		{
			var tmp2 = new byte[8, 8];

			if (!gfx)
			{
				var src2 = MainUnit.Get2ColorCharacter(selectedCharacterIndex);

				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 8; b++)
					{
						tmp2[b, a] = src2[a, 7 - b];
					}
				}

				MainUnit.Set2ColorCharacter(tmp2, selectedCharacterIndex);
				DoChar();
				RedrawChar();
				RedrawViewChar();
			}

			CheckDuplicate();
		}

		public void Rotate_leftExecute(object sender, EventArgs e)
		{
			var tmp2 = new byte[8, 8];

			if (!gfx)
			{
				var src2 = MainUnit.Get2ColorCharacter(selectedCharacterIndex);

				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 8; b++)
					{
						tmp2[b, a] = src2[7 - a, b];
					}
				}

				MainUnit.Set2ColorCharacter(tmp2, selectedCharacterIndex);
				DoChar();
				RedrawChar();
				RedrawViewChar();
			}

			CheckDuplicate();
		}

		/// <summary>
		/// Restore a character from the last saved font.
		/// The font is loaded and the character is extracted and replaces the current character data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void b_ressClick(object sender, EventArgs e)
		{
			try
			{
				var whichCharacter = selectedCharacterIndex & 127;
				var fontNumber = selectedCharacterIndex / 256;       // Font 0 or 1

				// Load the font bytes
				var data = File.ReadAllBytes(fontNumber == 0 ? fname1 : fname2);

				var characterOffset = whichCharacter * 8;

				for (var a = 0; a < 8; a++)
				{
					ft[characterOffset + fontNumber * 1024 + a] = data[characterOffset + a];
				}

				DoChar();
				RedrawViewChar();
				I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
				CheckDuplicate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// Restore a character from the default font.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void b_resdClick(object sender, EventArgs e)
		{
			try
			{
				var whichCharacter = selectedCharacterIndex & 127;

				// Load the font bytes
				var data = File.ReadAllBytes(Path.Join(AppContext.BaseDirectory, "Default.fnt"));


				var fontNumber = selectedCharacterIndex > 255 ? 1 : 0;

				for (var a = 0; a < 8; a++)
				{
					ft[whichCharacter * 8 + a + fontNumber * 1024] = data[whichCharacter * 8 + a];
				}

				DoChar();
				RedrawViewChar();
				I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
				CheckDuplicate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void b_invClick(object sender, EventArgs e)
		{
			if (!gfx)
			{
				var hp = MainUnit.GetCharacterPointer(selectedCharacterIndex);

				for (var a = 0; a < 8; a++)
				{
					ft[hp + a] = (byte)(ft[hp + a] ^ 255);
				}

				DoChar();
				RedrawChar();
				RedrawViewChar();
				CheckDuplicate();
			}
			else
			{
				SystemSounds.Beep.Play();
			}
		}

		public void Mirror_horizontalExecute(object sender, EventArgs e)
		{
			var tmp = new byte[8, 8];

			if (!gfx)
			{
				var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex);

				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 8; b++)
					{
						tmp[b, a] = src[7 - b, a];
					}
				}

				MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex);
			}
			else
			{
				var src5 = MainUnit.Get5ColorCharacter(selectedCharacterIndex);

				for (var a = 0; a < 8; a++)
				{
					for (var b = 0; b < 4; b++)
					{
						tmp[b, a] = src5[3 - b, a];
					}
				}

				MainUnit.Set5ColorCharacter(tmp, selectedCharacterIndex);
			}

			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		public void Mirror_verticalExecute(object sender, EventArgs e)
		{
			var tmp = new byte[8, 8];

			var src = MainUnit.Get2ColorCharacter(selectedCharacterIndex);

			for (var a = 0; a < 8; a++)
			{
				for (var b = 0; b < 8; b++)
				{
					tmp[a, b] = src[a, 7 - b];
				}
			}

			MainUnit.Set2ColorCharacter(tmp, selectedCharacterIndex);
			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		public void b_clrClick(object sender, EventArgs e)
		{
			var hp = MainUnit.GetCharacterPointer(selectedCharacterIndex);

			for (var a = 0; a < 8; a++)
			{
				ft[hp + a] = 0;
			}

			DoChar();
			RedrawChar();
			RedrawViewChar();
			CheckDuplicate();
		}

		public void b_newClick(object sender, EventArgs e)
		{
			var re = MessageBox.Show("Are you sure to load default character sets?", "Atari FontMaker", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

			if (re == DialogResult.Yes)
			{
				pathf = AppContext.BaseDirectory;
				MainUnit.CheckResources();
				LoadViewFile("default.atrview", true);
				UpdateFormCaption();
				RedrawSet();
				RedrawLineTypes();
				RedrawView();
				RedrawPal();
				RedrawViewChar();
				RedrawChar();
				b_save1Click(null, EventArgs.Empty);
			}
		}

		public void b_quitClick(object sender, EventArgs e)
		{
			MainUnit.exitowiec();
		}

		public void b_exportBMPClick(object sender, EventArgs e)
		{
			ExportWindowUnit.ExportWindowForm.ShowDialog();
		}

		public void b_colorSwitchClick(object sender, EventArgs e)
		{
			ColorSwitch(lb_cs1.SelectedIndex, lb_cs2.SelectedIndex);
		}

		public void ColorSwitch(int idx1, int idx2)
		{
			var src = MainUnit.Get5ColorCharacter(selectedCharacterIndex);

			for (var y = 0; y < 8; y++)
			{
				for (var x = 0; x < 4; x++)
				{
					if (src[x, y] == (byte)idx1)
					{
						src[x, y] = (byte)idx2;
					}
					else if (src[x, y] == idx2)
					{
						src[x, y] = (byte)idx1;
					}
				}
			}

			MainUnit.Set5ColorCharacter(src, selectedCharacterIndex);

			DoChar();
			RedrawChar();
			RedrawView();
		}

		/// <summary>
		/// Load data into the view area
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void b_lviewClick(object sender, EventArgs e)
		{
			FileStream fs = null;
			byte a = 0;
			byte b = 0;
			byte c = 0;
			byte version = 0;

			int loadSize = 0;
			byte rr = 0;
			byte gg = 0;
			byte bb = 0;

			var buf = new byte[2048];

			d_open.FileName = string.Empty;
			d_open.InitialDirectory = pathf;
			d_open.Filter = "Atari FontMaker View (*.atrview,*.vf2,*.vfn)|*.atrview;*.vf2;*.vfn|Raw data (*.dat)|*.dat";
			var ok = d_open.ShowDialog();

			if (ok == DialogResult.OK)
			{
				var viewWidth = GetActualViewWidth();
				var ext = Path.GetExtension(d_open.FileName).ToLower();

				if (ext == ".atrview")
				{
					LoadViewFile(d_open.FileName);
					pathf = Path.GetDirectoryName(d_open.FileName) + Path.DirectorySeparatorChar;
					RedrawSet();
					RedrawLineTypes();
					RedrawView();
					RedrawPal();
					RedrawViewChar();
					RedrawChar();
					return;
				}

				if (ext == ".dat")
				{
					fs = new FileStream(d_open.FileName, FileMode.Open, FileAccess.Read, FileShare.None);
					loadSize = (int)Math.Min(fs.Length, VIEW_HEIGHT * viewWidth);

					try
					{
						fs.Read(buf, 0, loadSize);
					}
					finally
					{
						fs.Close();
						fs = null;
					}


					// Copy the bytes into the screen
					for (a = 0; a < VIEW_HEIGHT; a++)
					{
						for (b = 0; b < viewWidth; b++)
						{
							if (a * viewWidth + b < loadSize)
							{
								vw[b, a] = buf[a * viewWidth + b];
							}
						}
					}

					pathf = Path.GetDirectoryName(d_open.FileName) + Path.DirectorySeparatorChar;
					RedrawSet();
					RedrawLineTypes();
					RedrawView();
					RedrawPal();
					RedrawViewChar();
					RedrawChar();
					return;
				}

				// Handle old binary versions of view file
				// Bytes:
				// version : 1 byte
				// gfx : 1 byte
				fs = new FileStream(d_open.FileName, FileMode.Open, FileAccess.Read, FileShare.None);
				int fsIndex = 0;

				if (ext == ".vf2")
				{
					try
					{

						fs.Read(buf, fsIndex, 1);
						version = buf[0];
						++fsIndex;
					}
					finally
					{
					}

					if (version > 3)
					{
						MessageBox.Show("File was created in newer version of FontMaker (incorrect file???)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}

				try
				{
					fs.Read(buf, fsIndex, 1);
					c = buf[0];
					++fsIndex;
				}
				finally
				{
				}

				if (c == 0)
				{
					gfx = true;
				}
				else
				{
					gfx = false;
				}
				b_gfxClick(null, EventArgs.Empty);

				if (ext == ".vf2")
				{
					for (a = 0; a <= 7; a++)
					{
						fs.Read(buf, fsIndex, 4);
						chsline[a] = (byte)BitConverter.ToInt32(buf, 0);
						fsIndex += 4;
					}
				}

				for (a = 0; a <= 5; a++)
				{
					try
					{
						fs.Read(buf, fsIndex, 3);
						fsIndex += 3;
						rr = buf[0];
						gg = buf[1];
						bb = buf[2];
					}
					finally
					{
					}

					cpal[a] = MainUnit.FindClosest(rr, gg, bb, palette);
					UpdateBrushCache(a);
				}

				if (ext == ".vf2")
				{
					switch (version)
					{
						case 2:
							{
								// 31 x 8 screen = 248 bytes
								try
								{
									fs.Read(buf, fsIndex, 248);
									fsIndex += 248;
								}
								finally
								{
								}

								for (a = 0; a < 8; a++)
								{
									for (b = 0; b < 31; b++)
									{
										vw[b, a] = buf[a * 31 + b];
									}
								}

								RedrawLineTypes();
							}
							break;

						case 3:
							{
								// 32x26 screen = 832 bytes
								try
								{
									fs.Read(buf, fsIndex, 32 * 26);
									fsIndex += 32 * 26;
								}
								finally
								{
								}

								for (a = 0; a < 26; a++)
								{
									for (b = 0; b < 32; b++)
									{
										vw[b, a] = buf[a * 32 + b];
									}
								}

								RedrawLineTypes();
							}
							break;
					}
				}

				if (ext == ".vfn")
				{
					try
					{
						fs.Read(buf, fsIndex, 186);
						fsIndex += 186;
					}
					finally
					{
					}

					for (b = 0; b < 31; b++)
					{
						for (a = 0; a < 6; a++)
						{
							vw[b, a] = buf[a + b * 6];
						}

						for (a = 6; a < 8; a++)
						{
							vw[b, a] = 0;
						}
					}
				}

				fs.Close();
				fs = null;
				RedrawSet();
				RedrawView();
				RedrawPal();
				RedrawViewChar();
				RedrawLineTypes();
				pathf = Path.GetDirectoryName(d_open.FileName) + Path.DirectorySeparatorChar;
			}
		}

		public void b_sviewClick(object sender, EventArgs e)
		{
			d_save.FileName = string.Empty;
			d_save.InitialDirectory = pathf;
			d_save.DefaultExt = "atrview";
			d_save.Filter = @"Atari FontMaker View (*.atrview)|*.atrview|Raw data (*.dat)|*.dat";

			if (d_save.ShowDialog() == DialogResult.OK)
			{
				var ext = Path.GetExtension(d_save.FileName).ToLower();

				if (ext == ".atrview")
				{
					SaveViewFile(d_save.FileName);
				}

				if (ext == ".dat")
				{
					var data = new byte[VIEW_HEIGHT * GetActualViewWidth()];
					for (var a = 0; a < VIEW_HEIGHT; a++)
						for (var b = 0; b < GetActualViewWidth(); b++)
							data[b + a * GetActualViewWidth()] = vw[b, a];
					File.WriteAllBytes(d_save.FileName, data);
				}

				pathf = Path.GetDirectoryName(d_save.FileName) + "\\";
			}
		}

		public void b_clrviewClick(object sender, EventArgs e)
		{
			var ok = MessageBox.Show("Clear view window?", "Atari FontMaker", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

			if (ok == DialogResult.Yes)
			{
				for (var a = 0; a < VIEW_HEIGHT; a++)
				{
					chsline[a] = 1;
				}

				for (var a = 0; a < VIEW_WIDTH; a++)
				{
					for (var b = 0; b < VIEW_HEIGHT; b++)
					{
						vw[a, b] = 0;
					}
				}

				RedrawView();
				RedrawLineTypes();
			}
		}

		public void cb_dupesClick(object sender, EventArgs e)
		{
			if (cb_dupes.Checked == false)
			{
				TimerDuplicates.Enabled = false;
				ShapeDupes.Visible = false;
			}
			else
			{
				CheckDuplicate();
			}
		}

		public void SpeedButtonMegaCopyClick(object sender, EventArgs e)
		{
			// enable/disable actions between modes
			var ena = SpeedButtonMegaCopy.Checked;
			b_enterText.Enabled = ena;
			// hide character edit window
			i_ch.Visible = !ena;

			// hide recolor if in megacopy mode
			if (ena && p_color_switch.Visible)
			{
				b_colorSwSetupClick(null, EventArgs.Empty);
			}

			foreach (var item in ActionListNormalModeOnly)
			{
				item.Enabled = !ena;
			}

			if (ena)
			{
				// MegaCopy mode on
				megaCopyStatus = TMegaCopyStatus.None;
				b_colorSwitch.Enabled = false;
				b_colorSwSetup.Enabled = false;
				cb_dupes.Enabled = false;
				TimerDuplicates.Enabled = false;
				ShapeDupes.Visible = false;

				Shape1v.Left = i_view.Left;
				Shape1v.Top = i_view.Top;
				Shape1v.Visible = true;
			}
			else
			{
				// MegaCopy mode off
				// Turn the GUI back on
				Shape1.Width = 20;
				Shape1.Height = 20;
				Shape1.Visible = true;
				Shape1v.Visible = false;
				Shape2v.Width = 20;
				Shape2v.Height = 20;
				var bx = selectedCharacterIndex % 32;
				var by = selectedCharacterIndex / 32;
				I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0));
				RevalidateCharButtons();
				cb_dupes.Enabled = true;
				CheckDuplicate();
			}
		}


		public void Undo_FontExecute(object sender, EventArgs e)
		{
			if (!Undo())
			{
				SystemSounds.Beep.Play();
			}
		}

		public void Redo_FontExecute(object sender, EventArgs e)
		{
			if (!Redo())
			{
				SystemSounds.Beep.Play();
			}
		}

		public void b_enterTextMouseDown(object sender, MouseEventArgs e)
		{
			var text = Interaction.InputBox("Enter text", "Enter text to be added to clipboard:", string.Empty);

			if (text.Length == 0)
			{
				return;
			}

			if (text.Length > 32)
			{
				text = text.Substring(-1, 32);
			}

			Clipboard_copyText(text, Control.ModifierKeys == Keys.Shift);
		}

		public void CheckBox40bytesClick(object sender, EventArgs e)
		{
			if (CheckBox40bytes.Checked)
			{
				MainUnit.MainForm.Width = MainUnit.MainForm.Width + 130;
			}
			else
			{
				MainUnit.MainForm.Width = MainUnit.MainForm.Width - 130;
			}
		}



		public void Shape2vMouseDown(object sender, MouseEventArgs e)
		{
			i_viewMouseDown(null, new MouseEventArgs(e.Button, 0, Shape2v.Left + e.X - i_view.Left, Shape2v.Top + e.Y - i_view.Top, 0));
		}
		public void Shape2vMouseUp(object sender, MouseEventArgs e)
		{
			i_viewMouseUp(null, new MouseEventArgs(e.Button, 0, Shape2v.Left + e.X - i_view.Left, Shape2v.Top + e.Y - i_view.Top, 0));
		}
		public void Shape2vMouseLeave(object sender, EventArgs e)
		{
			Shape2v.Visible = false;
			ImageMegaCopyV.Visible = false;
		}
		public void Shape2vMouseMove(object sender, MouseEventArgs e)
		{
			i_viewMouseMove(null, new MouseEventArgs(e.Button, 0, Shape2v.Left + e.X - i_view.Left, Shape2v.Top + e.Y - i_view.Top, 0));
		}



		public void Shape2MouseDown(object sender, MouseEventArgs e)
		{
			I_fnMouseDown(null, new MouseEventArgs(e.Button, 0, Shape2.Left + e.X - I_fn.Left, Shape2.Top + e.Y - I_fn.Top, 0));
		}
		public void Shape2MouseMove(object sender, MouseEventArgs e)
		{
			I_fnMouseMove(null, new MouseEventArgs(e.Button, 0, Shape2.Left + e.X - I_fn.Left, Shape2.Top + e.Y - I_fn.Top, 0));
		}
		public void Shape2MouseUp(object sender, MouseEventArgs e)
		{
			I_fnMouseUp(null, new MouseEventArgs(e.Button, 0, Shape2.Left + e.X - I_fn.Left, Shape2.Top + e.Y - I_fn.Top, 0));
		}



		public void Shape1vMouseDown(object sender, MouseEventArgs e)
		{
			i_viewMouseDown(sender, new MouseEventArgs(e.Button, 0, e.X + Shape1v.Left - i_view.Left, e.Y + Shape1v.Top - i_view.Top, 0));
		}
		public void Shape1vMouseMove(object sender, MouseEventArgs e)
		{
			i_viewMouseMove(sender, new MouseEventArgs(e.Button, 0, e.X + Shape1v.Left - i_view.Left, e.Y + Shape1v.Top - i_view.Top, 0));
		}
		public void Shape1vMouseUp(object sender, MouseEventArgs e)
		{
			i_viewMouseUp(sender, new MouseEventArgs(e.Button, 0, e.X + Shape1v.Left - i_view.Left, e.Y + Shape1v.Top - i_view.Top, 0));
		}




		public void Shape1MouseDown(object sender, MouseEventArgs e)
		{
			//memo1.Text := memo1.Text + 'X:' + inttostr(x) + ' Y:' + inttostr(y);
			I_fnMouseDown(sender, new MouseEventArgs(e.Button, 0, e.X + Shape1.Left - I_fn.Left, e.Y + Shape1.Top - I_fn.Top, 0));
		}
		public void Shape1MouseMove(object sender, MouseEventArgs e)
		{
			I_fnMouseMove(sender, new MouseEventArgs(e.Button, 0, e.X + Shape1.Left - I_fn.Left, e.Y + Shape1.Top - I_fn.Top, 0));
		}
		public void Shape1MouseLeave(object sender, EventArgs e)
		{
			Shape2.Visible = false;
			ImageMegacopy.Visible = false;
		}
		public void Shape1MouseUp(object sender, MouseEventArgs e)
		{
			I_fnMouseUp(sender, new MouseEventArgs(e.Button, 0, e.X + Shape1.Left - I_fn.Left, e.Y + Shape1.Top - I_fn.Top, 0));
		}


		private void FormMouseWheel(object sender, MouseEventArgs e)
		{
			int bx = 0;
			int by = 0;
			int nextCharacterIndex = 0;

			if (!SpeedButtonMegaCopy.Checked)
			{
				if (Control.ModifierKeys == Keys.Shift)
				{
					if (e.Delta > 0)
					{
						Ic1MouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
					}
					else
					{
						Ic2MouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
					}

					//Handled = true;
				}
				else
				{
					if (e.Delta > 0)
					{
						nextCharacterIndex = selectedCharacterIndex - 1;
					}
					else
					{
						nextCharacterIndex = selectedCharacterIndex + 1;
					}

					nextCharacterIndex = nextCharacterIndex % 512;
					if (nextCharacterIndex < 0)
						nextCharacterIndex += 512;

					bx = nextCharacterIndex % 32;
					by = nextCharacterIndex / 32;
					I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, bx * 16 + 4, by * 16 + 4, 0));
					// = true;
				}
			}
		}

		public void RevalidateClipboard()
		{
			var jtext = Clipboard.GetText();

			if (string.IsNullOrEmpty(jtext))
				return;

			var width = 0;
			var height = 0;
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;

			int charInFont = 0;
			try
			{
				var jsonObj = jtext.FromJson<ClipboardJSON>();
				int.TryParse(jsonObj.Width, out width);
				int.TryParse(jsonObj.Height, out height);

				characterBytes = jsonObj.Chars;
				fontBytes = jsonObj.Data;
			}
			catch (Exception ex)
			{
				return;
			}

			if (SpeedButtonMegaCopy.Checked)
			{
				var w = 16 * (width + 0);
				var h = 16 * (height + 0);

				ImageMegacopy.Size = new Size(w, h);
				ImageMegaCopyV.Size = new Size(w, h);


				var img = NewImage(ImageMegacopy);
				using (var gr = Graphics.FromImage(img))
				{
					gr.FillRectangle(cyanBrush, new Rectangle(0, 0, img.Width, img.Height));
				}

				DrawChars(ImageMegacopy, fontBytes, characterBytes, 0, 0, width, height, !gfx, 2);
				ImageMegaCopyV.Image?.Dispose();
				ImageMegaCopyV.Image = ImageMegacopy.Image;

				Shape2.Size = new Size(4 + w, 4 + h);
				Shape2v.Size = new Size(w, h);
			}
		}

		public void DrawChars(PictureBox targetImage, string data, string chars, int x, int y, int dataWidth, int dataHeight, bool gr0, int pixelsize)
		{
			var img = GetImage(targetImage);
			using (var gr = Graphics.FromImage(img))
			{
				for (var i = 0; i < dataHeight; i++)
				{
					for (var j = 0; j < dataWidth; j++)
					{
						DrawChar(gr, data.Substring((i * dataWidth + j) * 16, 16), chars.Substring((i * dataWidth + j) * 2, 2), x + 8 * pixelsize * j, y + 8 * pixelsize * i, gr0, pixelsize);
					}
				}
			}
			targetImage.Refresh();
		}

		public void DrawChar(Graphics gr, string data, string character, int x, int y, bool gr0, int pixelsize)
		{
			var inverse = Convert.ToInt32($"0x{character}", 16) > 127;

			if (gr0)
			{
				for (var i = 0; i < 8; i++)
				{
					var line = Convert.ToInt32($"0x{data.Substring(i * 2, 2)}", 16);
					var bwdata = MainUnit.DecodeBW((byte)line);

					for (var j = 0; j < 8; j++)
					{
						var brush = cpalBrushes[Convert.ToInt32(!inverse ^ (bwdata[j] == 1))];

						gr.FillRectangle(brush, x + j * pixelsize, y + i * pixelsize, pixelsize, pixelsize);
					}
				}
			}
			else
			{
				for (var i = 0; i < 8; i++)
				{
					var line = Convert.ToInt32("0x" + data.Substring(i * 2, 2), 16);
					var cldata = MainUnit.DecodeCL((byte)line);

					for (var j = 0; j < 4; j++)
					{
						SolidBrush brush;
						if ((inverse) && (cldata[j] == 3))
						{
							brush = cpalBrushes[5];
						}
						else
						{
							brush = cpalBrushes[1 + cldata[j]];
						}

						gr.FillRectangle(brush, x + j * pixelsize * 2, y + i * pixelsize, 2 * pixelsize, pixelsize);
					}
				}
			}
		}
		#region Keyboard shotcuts

		private void TMainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Oemcomma)
			{
				Previous_charExecute();
				return;
			}

			if (e.KeyCode == Keys.OemPeriod)
			{
				Next_charExecute();
				return;
			}

			if (e.KeyCode == Keys.R)
			{
				if (e.Shift)
					Rotate_rightExecute(null, null);
				else
					Rotate_leftExecute(null, null);
				return;
			}

			if (e.KeyCode == Keys.M)
			{
				if (e.Shift)
					Mirror_verticalExecute(null, null);
				else
					Mirror_horizontalExecute(null, null);
				return;
			}

			if (e.KeyCode == Keys.D1)
			{
				Color1Execute(null, null);
				return;
			}
			if (e.KeyCode == Keys.D2)
			{
				Color2Execute(null, null);
				return;
			}
			if (e.KeyCode == Keys.D3)
			{
				Color3Execute(null, null);
				return;
			}

			if (e.KeyCode == Keys.I)
			{
				b_invClick(null, null);
				return;
			}

			if (e.KeyCode == Keys.Escape)
			{
				EscapePressedExecute(null, null);
				return;
			}

			if (e.Control && e.KeyCode == Keys.C)
			{
				Clipboard_copyExecute(null, null);
				return;
			}

			if (e.Control && e.KeyCode == Keys.V)
			{
				b_pstClick(null, null);
				return;
			}

			if (e.Control && e.KeyCode == Keys.Z)
			{
				Undo_FontExecute(null, null);
				return;
			}
			// Ctrl + Y = Redo font change
			if (e.Control && e.KeyCode == Keys.Y)
			{
				Redo_FontExecute(null, null);
				return;
			}



		}

		public void Previous_charExecute()
		{
			if (!SpeedButtonMegaCopy.Checked)
			{
				if (CharacterEdited())
				{
					Add2Undo(true);
				}
			}

			selectedCharacterIndex--;
			if (selectedCharacterIndex < 0) selectedCharacterIndex += 512;
			SetCharCursor();
		}

		public void Next_charExecute()
		{
			if (!SpeedButtonMegaCopy.Checked)
			{
				if (CharacterEdited())
				{
					Add2Undo(true);
				}
			}

			selectedCharacterIndex++;
			if (selectedCharacterIndex >= 512) selectedCharacterIndex -= 512;
			SetCharCursor();
		}

		public void Color1Execute(object sender, EventArgs e)
		{
			SetColor(2);
		}
		public void Color2Execute(object sender, EventArgs e)
		{
			SetColor(3);
		}
		public void Color3Execute(object sender, EventArgs e)
		{
			SetColor(4);
		}

		public void EscapePressedExecute(object sender, EventArgs e)
		{
			if (SpeedButtonMegaCopy.Checked)
			{
				switch (megaCopyStatus)
				{
					case TMegaCopyStatus.None:
						break;

					case TMegaCopyStatus.Selecting:
						ResetMegaCopyStatus();
						break;

					case TMegaCopyStatus.Selected:
						break;

					case TMegaCopyStatus.Pasting:
						ResetMegaCopyStatus();
						break;
				}
			}
		}


		#endregion
	}
}