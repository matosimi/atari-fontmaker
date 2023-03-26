using System.Drawing.Drawing2D;
using System.Media;
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

			activeColorNr = 2;

			MainUnit.CheckResources();          // Make sure that the default files we need are unpacked in the exe folder
			LoadPalette();

			// UndoBuffer initialization
			undoBufferIndex = 0;

			for (var a = 0; a <= UNDOBUFFERSIZE; a++)
			{
				undoBufferFlags[a] = -1;
			}

			selectedCharacterIndex = 0;

			// Init which font is shown on each line of the preview window
			for (var a = 0; a < Constants.VIEW_HEIGHT; a++)
			{
				chsline[a] = 1;
			}

			pathf = AppContext.BaseDirectory;

			var paramCount = Environment.GetCommandLineArgs().Length;

			string? ext;
			if (Environment.GetCommandLineArgs().Length - 1 == 1)
			{
				var pathf = Path.GetDirectoryName(Environment.GetCommandLineArgs()[1]) + Path.DirectorySeparatorChar;
				ext = Path.GetExtension(Environment.GetCommandLineArgs()[1]).ToLower();

				switch (ext)
				{
					case ".fn2":
						{
							// TODO: Load a .fn2 file
							// It has 2048 bytes and effectively contains two fonts
							//Load_font(Environment.GetCommandLineArgs()[1], 0, true);
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
							fname2 = Path.Join(pathf, "Default.fnt");
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
				// If no input file set upon start, then show splash screen
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
				SetupDefaultPalColors();
				RedrawPal();
				RedrawGrid();

				if (ext != ".fn2")
				{
					// Load each of the fonts into the banks
					Load_font(fname1, 0, false);
					Load_font(fname2, 1, false);
					Load_font(fname3, 2, false);
					Load_font(fname4, 3, false);
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
			MainUnit.ExitApplication();
		}

		/* Public declarations */
		//	ActionList contains actions with different TAG parameter, legend:
		//	0 - action that does not modify character data
		//	1 - action that modifies character data
		//	2 - action that modifies character data applicable only on Mode 2
		public void CheckDuplicate()
		{
			if ((cb_dupes.Checked == false) || (cb_dupes.Enabled == false))
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
			if (activeColorNr != colorNum)
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

		private void i_colMouseDown(object sender, MouseEventArgs e)
		{
			if (Control.ModifierKeys == Keys.Shift)
			{
				var od = MessageBox.Show("Restore default colors?", "Atari FontMaker", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

				if (od == DialogResult.Yes)
				{
					SetupDefaultPalColors();
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
			RedrawRecolorPanel();

			BuildBrushCache();
		}

		public void RedrawRecolorPanel()
		{
			lb_cs1Click(null, EventArgs.Empty);
			lb_cs2Click(null, EventArgs.Empty);
		}

		/// <summary>
		/// Load 1 or Load 3 button was clicked
		/// Load a font into bank 1 or 3, or in dual mode into banks 1+2 or 3+4
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void b_load1Click(object sender, EventArgs e)
		{
			d_open.FileName = string.Empty;
			d_open.InitialDirectory = pathf;
			d_open.Filter = "Atari font 1 or Dual font (*.fnt,*.fn2)|*.fnt;*.fn2";
			var ok = d_open.ShowDialog();

			if (ok == DialogResult.OK)
			{
				var fontBankOffset = cbFontBank.Checked ? 2 : 0;
				var dual = Path.GetExtension(d_open.FileName) == ".fn2";
				Load_font(d_open.FileName, fontBankOffset, dual);
				pathf = Path.GetDirectoryName(d_open.FileName) + Path.DirectorySeparatorChar;

				if (dual)
				{
					var tempstring = d_open.FileName.Substring(0, d_open.FileName.Length - 4);
					if (cbFontBank.Checked == false)
					{
						fname1 = tempstring + "1.fnt";
						fname2 = tempstring + "2.fnt";
					}
					else
					{
						fname3 = tempstring + "3.fnt";
						fname4 = tempstring + "4.fnt";
					}
				}
				else
				{
					if (cbFontBank.Checked == false)
						fname1 = d_open.FileName;
					else
						fname3 = d_open.FileName;
				}

				UpdateFormCaption();
				RedrawSet();
				I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
				RedrawView();
				Add2UndoFullDifferenceScan(); // Full font scan
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
				var fontBankOffset = cbFontBank.Checked ? 2 : 0;
				Load_font(d_open.FileName, fontBankOffset + 1, false);

				pathf = Path.GetDirectoryName(d_open.FileName) + Path.DirectorySeparatorChar;

				if (cbFontBank.Checked == false)
					fname2 = d_open.FileName;
				else
					fname4 = d_open.FileName;

				UpdateFormCaption();
				RedrawSet();
				I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
				RedrawView();
				Add2UndoFullDifferenceScan(); //full font scan
			}

			CheckDuplicate();
		}

		/// <summary>
		/// Save the font in bank 1/3 away
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void b_save1Click(object sender, EventArgs e)
		{
			Save_font(fname1, cbFontBank.Checked == false ? 0 : 2);
		}

		/// <summary>
		/// Save the font in bank 2/4 away
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void b_save2Click(object sender, EventArgs e)
		{
			Save_font(fname2, cbFontBank.Checked == false ? 1 : 3);
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
				Save_font(d_save.FileName, cbFontBank.Checked == false ? 0 : 2);
				pathf = Path.GetDirectoryName(d_save.FileName) + Path.DirectorySeparatorChar;

				if (cbFontBank.Checked == false)
					fname1 = d_save.FileName;
				else
					fname3 = d_save.FileName;
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
				Save_font(d_save.FileName, cbFontBank.Checked == false ? 1 : 3);
				pathf = Path.GetDirectoryName(d_save.FileName) + Path.DirectorySeparatorChar;

				if (cbFontBank.Checked == false)
					fname2 = d_save.FileName;
				else
					fname4 = d_save.FileName;
			}

			UpdateFormCaption();
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

		public void b_aboutClick(object sender, EventArgs e)
		{
			i_abo.Visible = !i_abo.Visible;
		}

		public void i_aboMouseDown(object sender, MouseEventArgs e)
		{
			MainUnit.OpenURL("http://matosimi.atari.org");
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
				var brush = cpalBrushes[tagVal];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				drawTxt(gr, 1, 1, tagVal, palette[cpal[tagVal]]);

			}
			Ic1.Refresh();

			img = GetImage(Ic2);
			using (var gr = Graphics.FromImage(img))
			{
				var tagVal = (int)Ic2.Tag;
				var brush = cpalBrushes[tagVal];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				drawTxt(gr, 1, 1, tagVal, palette[cpal[tagVal]]);
			}
			Ic2.Refresh();

			img = GetImage(i_actcol);
			using (var gr = Graphics.FromImage(img))
			{
				var brush = cpalBrushes[activeColorNr];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				drawTxt(gr, 1, 1, activeColorNr, palette[cpal[activeColorNr]]);
			}
			i_actcol.Refresh();
		}

		/// <summary>
		/// Repaint actual character into the font area
		/// 1st into bmpFontBank and then copy into I_fn
		/// </summary>
		public void DoChar()
		{
			UpdateUndoButtons(CharacterEdited());

			using (var gr = Graphics.FromImage(bmpFontBanks))
			{
				var ry = selectedCharacterIndex / 32;
				var rx = selectedCharacterIndex % 32;
				var bankOffset = cbFontBank.Checked ? 256 : 0;
				var fontInBankOffset = cbFontBank.Checked ? 2048 : 0;           // How far into the ft[] buffer are we looking?

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
					for (var y = 0; y < 8; y++)
					{
						var line2color = MainUnit.DecodeBW(ft[hp + y + fontInBankOffset]);

						for (var x = 0; x < 8; x++)
						{
							if (hp < 1024)
							{
								gr.FillRectangle(cpalBrushes[1 - line2color[x]], rx * 16 + x * 2, ry * 16 + y * 2 + bankOffset, 2, 2);
								gr.FillRectangle(cpalBrushes[line2color[x]], rx * 16 + x * 2, ry * 16 + y * 2 + 64 + bankOffset, 2, 2);
							}
							else
							{
								gr.FillRectangle(cpalBrushes[1 - line2color[x]], rx * 16 + x * 2, ry * 16 + y * 2 + 64 + bankOffset, 2, 2);
								gr.FillRectangle(cpalBrushes[line2color[x]], rx * 16 + x * 2, ry * 16 + y * 2 + 128 + bankOffset, 2, 2);
							}
						}
					}
				}
				else
				{
					for (var y = 0; y < 8; y++)
					{
						var line5color = MainUnit.DecodeCL(ft[hp + y + fontInBankOffset]);

						for (var x = 0; x < 4; x++)
						{
							if (hp < 1024)
							{
								var brush = cpalBrushes[bits2colorIndex[line5color[x]]];
								gr.FillRectangle(brush, rx * 16 + x * 4, ry * 16 + y * 2 + bankOffset, 4, 2);

								if (line5color[x] == 3)
								{
									brush = cpalBrushes[5];
								}

								gr.FillRectangle(brush, rx * 16 + x * 4, ry * 16 + y * 2 + 64 + bankOffset, 4, 2);
							}
							else
							{
								var brush = cpalBrushes[bits2colorIndex[line5color[x]]];
								gr.FillRectangle(brush, rx * 16 + x * 4, ry * 16 + y * 2 + 64 + bankOffset, 4, 2);

								if (line5color[x] == 3)
								{
									brush = cpalBrushes[5];
								}

								gr.FillRectangle(brush, rx * 16 + x * 4, ry * 16 + y * 2 + 128 + bankOffset, 4, 2);
							}
						}
					}
				}
			}
			var img = GetImage(I_fn);
			using (var grD = Graphics.FromImage(img))
			{
				// Copy font bank 1 or 2
				grD.DrawImage(bmpFontBanks, 0, 0, cbFontBank.Checked == false ? Constants.RectFontBank12 : Constants.RectFontBank34, GraphicsUnit.Pixel);
			}
			/*
			var img = GetImage(I_fn);
			using (var gr = Graphics.FromImage(img))
			{
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
								gr.FillRectangle(cpalBrushes[line2color[b]], rx * 16 + b * 2, ry * 16 + a * 2 + 64, 2, 2);
							}
							else
							{
								gr.FillRectangle(cpalBrushes[1 - line2color[b]], rx * 16 + b * 2, ry * 16 + a * 2 + 64, 2, 2);
								gr.FillRectangle(cpalBrushes[line2color[b]], rx * 16 + b * 2, ry * 16 + a * 2 + 128, 2, 2);
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

								if (line5color[b] == 3)
								{
									brush = cpalBrushes[5];
								}

								gr.FillRectangle(brush, rx * 16 + b * 4, ry * 16 + a * 2 + 64, 4, 2);
							}
							else
							{
								var brush = cpalBrushes[bits2colorIndex[line5color[b]]];
								gr.FillRectangle(brush, rx * 16 + b * 4, ry * 16 + a * 2 + 64, 4, 2);

								if (line5color[b] == 3)
								{
									brush = cpalBrushes[5];
								}

								gr.FillRectangle(brush, rx * 16 + b * 4, ry * 16 + a * 2 + 128, 4, 2);
							}
						}
					}
				}
			}
			*/

			I_fn.Refresh();
		}

		/// <summary>
		/// Redraws whole font area: font banks and the I_fn view into the bank (either page 0 or page 1)
		/// Draw the 4 fonts into the bmpFontBanks bitmap
		/// </summary>
		public void RedrawSet()
		{
			// Where are each of the font's bytes to be found. Start with the first character (0)
			var byteIndex = new int[4]
			{
				1024 * 0,
				1024 * 1,
				1024 * 2,
				1024 * 3,
			};

			// Draw the 4 fonts to the bitmap
			using (var gr = Graphics.FromImage(bmpFontBanks))
			{
				if (gfx == false)
				{
					// Graphics mode 0 (B & W)
					var brush = cpalBrushes[1];
					gr.FillRectangle(brush, 0, 0, 512, 512);

					brush = cpalBrushes[0];

					var rowOffset = 0;          // Row offset 0, 16, 32, 48
					for (var row = 0; row < 4; row++, rowOffset += 16)
					{
						var colOffset = 0;
						for (var col = 0; col < 32; col++, colOffset += 16)
						{
							for (var y = 0; y < 8; y++)
							{
								// Get the bytes of the character
								var f1 = ft[byteIndex[0]];
								var f2 = ft[byteIndex[1]];
								var f3 = ft[byteIndex[2]];
								var f4 = ft[byteIndex[3]];

								var mask = 128;
								for (var x = 0; x < 8; x++)
								{
									// Font 1
									if ((f1 & mask) != 0)
									{
										gr.FillRectangle(brush, colOffset + x * 2, y * 2 + rowOffset, 2, 2);
									}
									else
									{
										gr.FillRectangle(brush, colOffset + x * 2, y * 2 + rowOffset + 64, 2, 2);
									}

									// Font 2
									if ((f2 & mask) != 0)
									{
										gr.FillRectangle(brush, colOffset + x * 2, y * 2 + rowOffset + 128, 2, 2);
									}
									else
									{
										gr.FillRectangle(brush, colOffset + x * 2, y * 2 + rowOffset + 192, 2, 2);
									}

									// Font 3
									if ((f3 & mask) != 0)
									{
										gr.FillRectangle(brush, colOffset + x * 2, y * 2 + rowOffset + 256, 2, 2);
									}
									else
									{
										gr.FillRectangle(brush, colOffset + x * 2, y * 2 + rowOffset + 320, 2, 2);
									}

									// Font 4
									if ((f4 & mask) != 0)
									{
										gr.FillRectangle(brush, colOffset + x * 2, y * 2 + rowOffset + 384, 2, 2);
									}
									else
									{
										gr.FillRectangle(brush, colOffset + x * 2, y * 2 + rowOffset + 448, 2, 2);
									}

									mask >>= 1;
								}

								// Move to the next byte in the character
								++byteIndex[0];
								++byteIndex[1];
								++byteIndex[2];
								++byteIndex[3];
							}
						}
					}
				}
				else
				{
					// Mode 4
					var rowOffset = 0;          // Row offset 0, 16, 32, 48
					for (var row = 0; row < 4; row++, rowOffset += 16)
					{
						var colOffset = 0;
						for (var col = 0; col < 32; col++, colOffset += 16)
						{
							for (var y = 0; y < 8; y++)
							{
								// Get the bytes of the character
								var f1 = ft[byteIndex[0]];
								var f2 = ft[byteIndex[1]];
								var f3 = ft[byteIndex[2]];
								var f4 = ft[byteIndex[3]];

								var mask = 128 | 64;
								for (var x = 0; x < 4; x++)
								{
									// Font 1
									var fb = (f1 & mask) >> (6 - x * 2);
									var brush = cpalBrushes[fb + 1];
									gr.FillRectangle(brush, colOffset + x * 4, y * 2 + rowOffset, 4, 2);
									// Invert
									if (fb == 4)
									{
										brush = cpalBrushes[5];
									}
									gr.FillRectangle(brush, colOffset + x * 4, y * 2 + rowOffset + 64, 4, 2);

									// Font 2
									fb = (f2 & mask) >> (6 - x * 2);
									brush = cpalBrushes[fb + 1];
									gr.FillRectangle(brush, colOffset + x * 4, y * 2 + rowOffset + 128, 4, 2);
									// Invert
									if (fb == 4)
									{
										brush = cpalBrushes[5];
									}
									gr.FillRectangle(brush, colOffset + x * 4, y * 2 + rowOffset + 192, 4, 2);

									// Font 3
									fb = (f3 & mask) >> (6 - x * 2);
									brush = cpalBrushes[fb + 1];
									gr.FillRectangle(brush, colOffset + x * 4, y * 2 + rowOffset + 256, 4, 2);
									// Invert
									if (fb == 4)
									{
										brush = cpalBrushes[5];
									}
									gr.FillRectangle(brush, colOffset + x * 4, y * 2 + rowOffset + 320, 4, 2);

									// Font 4
									fb = (f4 & mask) >> (6 - x * 2);
									brush = cpalBrushes[fb + 1];
									gr.FillRectangle(brush, colOffset + x * 4, y * 2 + rowOffset + 384, 4, 2);
									// Invert
									if (fb == 4)
									{
										brush = cpalBrushes[5];
									}
									gr.FillRectangle(brush, colOffset + x * 4, y * 2 + rowOffset + 448, 4, 2);

									mask >>= 2;
								}

								// Move to the next byte in the character
								++byteIndex[0];
								++byteIndex[1];
								++byteIndex[2];
								++byteIndex[3];
							}
						}
					}
				}
			}

			var img = GetImage(I_fn);
			using (var grD = Graphics.FromImage(img))
			{
				// Copy font bank 1 or 2
				grD.DrawImage(bmpFontBanks, 0, 0, cbFontBank.Checked == false ? Constants.RectFontBank12 : Constants.RectFontBank34, GraphicsUnit.Pixel);
			}
			/*
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

				if (gfx == false)
				{
					// Graphics mode 0 (B & W)
					var brush = cpalBrushes[1];
					gr.FillRectangle(brush, 0, 0, 512, I_fn.Height);
					brush = cpalBrushes[0];

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
										gr.FillRectangle(brush, a * 16 + c * 2, b * 2 + d * 16, 2, 2);
									}
									else
									{
										gr.FillRectangle(brush, a * 16 + c * 2, b * 2 + d * 16 + 64, 2, 2);
									}

									if (ou2[c] == 1)
									{
										gr.FillRectangle(brush, a * 16 + c * 2, b * 2 + d * 16 + 128, 2, 2);
									}
									else
									{
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

									var brush = cpalBrushes[fb];
									gr.FillRectangle(brush, a * 16 + c * 4, b * 2 + d * 16, 4, 2);

									if (fb == 4)
									{
										brush = cpalBrushes[5];
									}

									gr.FillRectangle(brush, a * 16 + c * 4, b * 2 + d * 16 + 64, 4, 2);

									fb = ov2[c] + 1;
									brush = cpalBrushes[fb];
									gr.FillRectangle(brush, a * 16 + c * 4, b * 2 + d * 16 + 128, 4, 2);

									if (fb == 4)
									{
										brush = cpalBrushes[5];
									}

									gr.FillRectangle(brush, a * 16 + c * 4, b * 2 + d * 16 + 192, 4, 2);
								}
							}
						}
					}
				}
			}
			*/
			I_fn.Refresh();
		}

		/// <summary>
		/// Load a single or dual font file into a specific bank (or two consecutive banks)
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="bankOffset"></param>
		/// <param name="dual"></param>
		public void Load_font(string filename, int bankOffset, bool dual)
		{
			var expSize = dual ? 2048 : 1024;

			var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
			var size = fs.Length;
			var loadSize = Math.Min(expSize, size);

			try
			{
				fs.Read(ft, (int)(bankOffset * loadSize), (int)loadSize);
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

		/// <summary>
		/// Save the 1024 bytes of a font away to a file
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="fontNr"></param>
		public void Save_font(string filename, int fontNr)
		{
			var fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.Write);

			try
			{
				fs.Write(ft, fontNr * 1024, 1024);

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
				var brush = cpalBrushes[lb_cs1.SelectedIndex + 1];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				drawTxt(gr, 1, 1, lb_cs1.SelectedIndex + 2, palette[cpal[lb_cs1.SelectedIndex + 1]]);
			}
			i_rec1.Refresh();
		}

		public void lb_cs2Click(object sender, EventArgs e)
		{
			var img = GetImage(i_rec2);
			using (var gr = Graphics.FromImage(img))
			{
				var brush = cpalBrushes[lb_cs2.SelectedIndex + 1];
				gr.FillRectangle(brush, 0, 0, 49, 17);
				drawTxt(gr, 1, 1, lb_cs2.SelectedIndex + 2, palette[cpal[lb_cs2.SelectedIndex + 1]]);
			}
			i_rec2.Refresh();
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

		public void UpdateFormCaption()
		{
			MainUnit.MainForm.Text = MainUnit.TITLE + " v" + MainUnit.GetBuildInfoAsString() + " - " + Path.GetFileName(fname1) + "/" + Path.GetFileName(fname2) + "/" + Path.GetFileName(fname3) + "/" + Path.GetFileName(fname4);
			b_save1.Visible = true;
			b_save2.Visible = true;
		}

		public void ExecuteCopyToClipboard(bool sourceIsView)
		{
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;
			var charInFont = 0;

			var fontInBankOffset = cbFontBank.Checked ? 2048 : 0;

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
							fontBytes = fontBytes + String.Format("{0:X2}", ft[charInFont + k + fontInBankOffset]);
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

			var fontInBankOffset = cbFontBank.Checked ? 2048 : 0;

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
					fontBytes = fontBytes + $"{ft[charInFont + k + fontInBankOffset]:X2}";
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

		public void ExecutePastFromClipboard(bool targetIsView)
		{
			var width = 0;
			var height = 0;
			var characterBytes = string.Empty;
			var fontBytes = string.Empty;

			var fontInBankOffset = cbFontBank.Checked ? 2048 : 0;

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
								ft[charInFont + k + fontInBankOffset] = charsBytes[(ii * width + jj) * 8 + k];
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

				var hp = MainUnit.GetCharacterPointer(selectedCharacterIndex, cbFontBank.Checked);

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

		public void Add2Undo(bool difference)
		{
			if (difference)
			{
				var prevUndoIndex = undoBufferIndex;
				undoBufferIndex = (undoBufferIndex + 1) % UNDOBUFFERSIZE; //size of undo buffer

				for (var i = 0; i < 2048 + 2048; i++)
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

			for (var i = 0; i < 2048 + 2048; i++)
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
			while ((i < 2048 + 2048) && (!difference))
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
			for (var i = 0; i < 2048 + 2048; i++)
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
				for (var i = 0; i < 2048 + 2048; i++)
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
			var ptr = MainUnit.GetCharacterPointer(selectedCharacterIndex, cbFontBank.Checked);

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


		public void b_newClick(object sender, EventArgs e)
		{
			var re = MessageBox.Show("Are you sure to load default character sets?", "Atari FontMaker", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

			if (re == DialogResult.Yes)
			{
				pathf = AppContext.BaseDirectory;
				MainUnit.CheckResources();
				LoadViewFile("default.atrview", true);
				cbFontBank.Checked = false;
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
			MainUnit.ExitApplication();
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
			var src = MainUnit.Get5ColorCharacter(selectedCharacterIndex, cbFontBank.Checked);

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

			MainUnit.Set5ColorCharacter(src, selectedCharacterIndex, cbFontBank.Checked);

			DoChar();
			RedrawChar();
			RedrawView();
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
				b_colorSwSetupClick(0, EventArgs.Empty);
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
					ExecuteRotateRight();
				else
					ExecuteRotateLeft();
				return;
			}

			if (e.KeyCode == Keys.M)
			{
				if (e.Shift)
					ExecuteMirrorVertical();
				else
					ExecuteMirrorHorizontal();
				return;
			}

			if (e.KeyCode == Keys.D1)
			{
				Color1Execute(0, EventArgs.Empty);
				return;
			}
			if (e.KeyCode == Keys.D2)
			{
				Color2Execute(0, EventArgs.Empty);
				return;
			}
			if (e.KeyCode == Keys.D3)
			{
				Color3Execute(0, EventArgs.Empty);
				return;
			}

			if (e.KeyCode == Keys.I)
			{
				ExecuteInvertCharacter();
				return;
			}

			if (e.KeyCode == Keys.Escape)
			{
				EscapePressedExecute(0, EventArgs.Empty);
				return;
			}

			if (e.Control && e.KeyCode == Keys.C)
			{
				ExecuteCopyToClipboard(false);
				return;
			}

			if (e.Control && e.KeyCode == Keys.V)
			{
				ButtonPasteClicked(0, EventArgs.Empty);
				return;
			}

			if (e.Control && e.KeyCode == Keys.Z)
			{
				Undo_FontExecute(0, EventArgs.Empty);
				return;
			}
			// Ctrl + Y = Redo font change
			if (e.Control && e.KeyCode == Keys.Y)
			{
				Redo_FontExecute(0, EventArgs.Empty);
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

		private void cbFontBank_Click(object sender, EventArgs e)
		{
			cbFontBank_CheckedChanged(0, EventArgs.Empty);
			RedrawSet();
			I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
			I_fnMouseUp(null, new MouseEventArgs(MouseButtons.Left, 0, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
			CheckDuplicate();
		}

		private void cbFontBank_CheckedChanged(object sender, EventArgs e)
		{
			if (cbFontBank.Checked == false)
			{
				// Bank 1 + 2
				cbFontBank.ImageIndex = 0;
				b_load1.Text = "Load 1";
				b_load2.Text = "Load 2";
				b_save1.Text = "Save 1";
				b_save2.Text = "Save 2";
				btnClearFont1.Text = "Clear 1";
				btnClearFont2.Text = "Clear 2";
			}
			else
			{
				cbFontBank.ImageIndex = 1;
				b_load1.Text = "Load 3";
				b_load2.Text = "Load 4";
				b_save1.Text = "Save 3";
				b_save2.Text = "Save 4";
				btnClearFont1.Text = "Clear 3";
				btnClearFont2.Text = "Clear 4";
			}
		}

		private void btnClearFont1_Click(object sender, EventArgs e)
		{
			var fontBankOffset = cbFontBank.Checked ? 2 : 0;

			var re = MessageBox.Show($"Are you sure to clear font {1 + fontBankOffset}?", "Atari FontMaker", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

			if (re == DialogResult.Yes)
			{
				ClearFont(0 + fontBankOffset);
			}
		}

		private void btnClearFont2_Click(object sender, EventArgs e)
		{
			var fontBankOffset = cbFontBank.Checked ? 2 : 0;

			var re = MessageBox.Show($"Are you sure to clear font {2 + fontBankOffset}?", "Atari FontMaker", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

			if (re == DialogResult.Yes)
			{
				ClearFont(1 + fontBankOffset);
			}
		}


		private void ClearFont(int fontNr)
		{
			var dest = fontNr * 1024;
			for (var i = 0; i < 1024; ++i)
			{
				ft[dest++] = 0;

			}
			RedrawSet();
			I_fnMouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, (selectedCharacterIndex % 32) * 16, (selectedCharacterIndex / 32) * 16, 0));
			RedrawView();
			CheckDuplicate();
		}

	}
}