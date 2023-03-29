using Microsoft.VisualBasic;
using System.Drawing.Drawing2D;
using System.Text;
using System.Text.Json;
using TinyJson;

namespace FontMaker
{
	internal class MainView
	{
	}

	// All functions that interact with the view can be found here
	public partial class TMainForm
	{
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
								ExecutePastFromClipboard(true);
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

							break;
						}
				}
			}
		}

		public void i_viewMouseMove(object sender, MouseEventArgs e)
		{
			int rx, ry;

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

		public bool MouseValidView(int x, int y)
		{
			if ((x >= i_view.Width - (copyRange.Width) * 16) || (y >= i_view.Height - (copyRange.Height) * 16))
			{
				return false;
			}
			else
			{
				return true;
			}

			return false;
		}

		public void ImageMegaCopyViewMouseMove(object sender, MouseEventArgs e)
		{
			i_viewMouseMove(sender, new MouseEventArgs(MouseButtons.None, 0, e.X + ImageMegaCopyV.Left - i_view.Left, e.Y + ImageMegaCopyV.Top - i_view.Top, 0));
		}
		public void ImageMegaCopyViewMouseDown(object sender, MouseEventArgs e)
		{
			i_viewMouseDown(sender, new MouseEventArgs(e.Button, 0, e.X + ImageMegaCopyV.Left - i_view.Left, e.Y + ImageMegaCopyV.Top - i_view.Top, 0));
		}

		/// <summary>
		/// Redraw whole view area by copying characters from font area
		/// </summary>
		public void RedrawView()
		{
			if (bmpFontBanks == null) return;
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

				for (var y = 0; y < Constants.VIEW_HEIGHT; y++)
				{
					for (var x = 0; x < Constants.VIEW_WIDTH; x++)
					{
						var rx = vw[x, y] % 32;
						var ry = (vw[x, y] / 32);

						destRect.X = x * 16;
						destRect.Y = y * 16;

						srcRect.X = rx * 16;
						srcRect.Y = ry * 16 + Constants.FontYOffset[chsline[y] - 1];

						gr.DrawImage(bmpFontBanks, destRect, srcRect, GraphicsUnit.Pixel);
					}
				}
			}
			i_view.Refresh();
		}

		/// <summary>
		/// Redraw all occurrences of character (selectedCharacterIndex) on bank X in view area 
		/// </summary>
		public void RedrawViewChar()
		{
			// TODO: Get the image to draw correctly

			var img = GetImage(i_view);
			using (var gr = Graphics.FromImage(img))
			{
				var destRect = new Rectangle
				{
					Width = 16,
					Height = 16,
				};

				var rx = selectedCharacterIndex % 32;       // Character x,y
				var ry = selectedCharacterIndex / 32;

				var srcRect = new Rectangle
				{
					X = rx * 16,
					Y = ry * 16,        // Will be set below
					Width = 16,
					Height = 16,
				};

				for (var y = 0; y < Constants.VIEW_HEIGHT; y++)
				{
					var fontYOffset = Constants.FontPageOffset[chsline[y] - 1];

					ry = (ry | 8);

					if (chsline[y] == 1 || chsline[y] == 3)
					{
						ry = (ry ^ 8);
					}

					var ny = ry ^ 4; //ny checks invert notes sign (ry)
					var ep = (rx + ry * 32) % 256;
					var dp = (rx + ny * 32) % 256;

					for (var x = 0; x < Constants.VIEW_WIDTH; x++)
					{
						destRect.X = x * 16;
						destRect.Y = y * 16;

						if (vw[x, y] == (byte)ep)
						{
							srcRect.Y = ry * 16 + fontYOffset;
							//gr.DrawImage(I_fn.Image, destRect, srcRect, GraphicsUnit.Pixel);
							gr.DrawImage(bmpFontBanks, destRect, srcRect, GraphicsUnit.Pixel);

						}

						if (vw[x, y] == (byte)dp)
						{
							srcRect.Y = ny * 16 + fontYOffset;
							//gr.DrawImage(I_fn.Image, destRect, srcRect, GraphicsUnit.Pixel);
							gr.DrawImage(bmpFontBanks, destRect, srcRect, GraphicsUnit.Pixel);
						}
					}
				}
			}
			i_view.Refresh();
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

		private static string[] ToDraw = new string[] { null, "1", "2", "3", "4" };

		/// <summary>
		/// Draw the font indicator next to the sample screen.
		/// Column of 26x 1-4 font indicators
		/// </summary>
		public void RedrawLineTypes()
		{
			var img = GetImage(i_chset);
			using (var gr = Graphics.FromImage(img))
			{
				gr.FillRectangle(whiteBrush, 0, 0, img.Width, img.Height);
				for (var a = 0; a < Constants.VIEW_HEIGHT; a++)
				{
					gr.DrawString(ToDraw[chsline[a]], this.Font, blackBrush, 4, 2 + a * 16);
				}
			}
			i_chset.Refresh();
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

		public void LoadViewFile(string filename, bool forceLoadFont = false)
		{
			ClearPageList();

			var filenames = new string[4];
			byte viewWidth = 0;

			try
			{
				string jtext = filename is null ? MainUnit.GetResource<string>("default.atrview") : File.ReadAllText(filename, Encoding.UTF8);
				var jsonObj = jtext.FromJson<AtrViewInfoJSON>();

				int.TryParse(jsonObj.Version, out var version);
				int.TryParse(jsonObj.ColoredGfx, out var coloredGfx);

				if (version >= 1911)
				{
					// Take out the values from the parsed JSON container
					var characterBytes = jsonObj.Chars;
					var lineTypes = jsonObj.Lines;
					var colors = jsonObj.Colors;
					var fontBytes = jsonObj.Data;

					filenames[0] = jsonObj.Fontname1;
					filenames[1] = jsonObj.Fontname2;
					filenames[2] = jsonObj.Fontname3 == null ? "Default.fnt" : jsonObj.Fontname3;
					filenames[3] = jsonObj.Fontname4 == null ? "Default.fnt" : jsonObj.Fontname4;

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
					for (var y = 0; y < Constants.VIEW_HEIGHT; ++y)
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
						b_gfxClick(0, EventArgs.Empty);
					}

					if ((forceLoadFont) || (MessageBox.Show("Would you like to load fonts embedded in this view file?", "Load embedded fonts", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes))
					{
						fname1 = filenames[0];
						fname2 = filenames[1];
						fname3 = filenames[2];
						fname4 = filenames[3];

						if (fontBytes.Length == 4096)
						{
							fontBytes = fontBytes + fontBytes;
						}
						ft = Convert.FromHexString(fontBytes);

						UpdateFormCaption();
						Add2UndoFullDifferenceScan(); //full font scan
					}

					// 40 bytes width
					if (((fortyBytes == "1") && (CheckBox40bytes.Checked == false))
						|| ((fortyBytes == "0") && (CheckBox40bytes.Checked == true)))
					{
						CheckBox40bytes.Checked = !CheckBox40bytes.Checked;
						CheckBox40bytesClick(0, EventArgs.Empty);
					}

					// Load the page information
					if (jsonObj.Pages != null && jsonObj.Pages.Count > 0)
					{
						Pages = new List<PageData>();
						for (var pageIndex = 0; pageIndex < jsonObj.Pages.Count; ++pageIndex)
						{
							var pageSrc = jsonObj.Pages[pageIndex];


							bytes = Convert.FromHexString(pageSrc.View);
							idx = 0;
							var viewData = new byte[Constants.VIEW_WIDTH, Constants.VIEW_HEIGHT];
							for (var y = 0; y < Constants.VIEW_HEIGHT; ++y)
							{
								for (var x = 0; x < viewWidth; ++x)
								{
									viewData[x, y] = bytes[idx];
									++idx;
								}
							}

							var page = new PageData(pageSrc.Nr, pageSrc.Name, viewData, Convert.FromHexString(pageSrc.SelectedFont), pageIndex);
							Pages.Add(page);
						}

						SwopPageAction(0);
					}
				}

				CheckDuplicate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);

				// Set some defaults!
				SetupDefaultPalColors();
				fname1 = "Default.fnt";
				fname2 = "Default.fnt";
				fname3 = "Default.fnt";
				fname4 = "Default.fnt";
			}

			// Make sure the cpal values have valid brushes for painting!
			BuildBrushCache();

			BuildPageList();
		}

		// save view file new edition
		public void SaveViewFile(string filename)
		{
			// Make sure that the current page's data is saved to the page container
			SwopPage(saveCurrent: true);

			var jo = new AtrViewInfoJSON();

			var characterBytes = string.Empty;

			// Version
			jo.Version = "2023";
			// Which GFX mode is selected
			jo.ColoredGfx = gfx ? "1" : "0";

			// Characters in the current view
			for (var i = 0; i < Constants.VIEW_HEIGHT; i++)
			{
				for (var j = 0; j < Constants.VIEW_WIDTH; j++)
				{
					characterBytes = characterBytes + String.Format("{0:X2}", vw[j, i]);
				}
			}

			jo.Chars = characterBytes;

			// Font selection information
			jo.Lines = Convert.ToHexString(chsline);

			// Colors
			jo.Colors = Convert.ToHexString(cpal);
			// Font names
			jo.Fontname1 = fname1;
			jo.Fontname2 = fname2;
			jo.Fontname3 = fname3;
			jo.Fontname4 = fname4;

			// Font data
			jo.Data = Convert.ToHexString(ft);

			// Width selection
			jo.FortyBytes = GetActualViewWidth() == 40 ? "1" : "0";

			// Save the page information
			jo.Pages = new List<SavedPageData>();
			foreach (var srcPage in Pages)
			{
				var page = new SavedPageData()
				{
					Nr = srcPage.Nr,
					Name = srcPage.Name,
					SelectedFont = Convert.ToHexString(srcPage.SelectedFont),
				};
				var pageView = string.Empty;
				for (var i = 0; i < Constants.VIEW_HEIGHT; i++)
				{
					for (var j = 0; j < Constants.VIEW_WIDTH; j++)
					{
						pageView = pageView + $"{srcPage.View[j, i]:X2}";
					}
				}

				page.View = pageView;
				jo.Pages.Add(page);
			}

			var txt = jo.ToJson();

			File.WriteAllText(filename, txt, Encoding.UTF8);
		}

		/// <summary>
		/// Cycle through the fonts on each line 1 -> 2 -> 3 -> 4 -> 1
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void i_chsetMouseDown(object sender, MouseEventArgs e)
		{
			var ry = e.Y / 16;

			if (Control.ModifierKeys == Keys.Control)
			{
				// Reset to 1
				chsline[ry] = 1;
			}
			else if (Control.ModifierKeys == Keys.Shift || e.Button == MouseButtons.Right)
			{
				// Cycle backwards
				--chsline[ry];
				if (chsline[ry] == 0)
					chsline[ry] = 4;
			}
			else
			{
				// Cycle forward
				++chsline[ry];
				if (chsline[ry] == 5)
					chsline[ry] = 1;
			}

			RedrawLineTypes();
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

			var buf = new byte[8192];

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
					loadSize = (int)Math.Min(fs.Length, Constants.VIEW_HEIGHT * viewWidth);

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
					for (a = 0; a < Constants.VIEW_HEIGHT; a++)
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
					var data = new byte[Constants.VIEW_HEIGHT * GetActualViewWidth()];
					for (var a = 0; a < Constants.VIEW_HEIGHT; a++)
						for (var b = 0; b < GetActualViewWidth(); b++)
							data[b + a * GetActualViewWidth()] = vw[b, a];
					File.WriteAllBytes(d_save.FileName, data);
				}

				pathf = Path.GetDirectoryName(d_save.FileName) + Path.DirectorySeparatorChar;
			}
		}

		public void b_clrviewClick(object sender, EventArgs e)
		{
			var ok = MessageBox.Show("Clear view window?", "Atari FontMaker", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

			if (ok == DialogResult.Yes)
			{
				for (var a = 0; a < Constants.VIEW_HEIGHT; a++)
				{
					chsline[a] = 1;
				}

				for (var a = 0; a < Constants.VIEW_WIDTH; a++)
				{
					for (var b = 0; b < Constants.VIEW_HEIGHT; b++)
					{
						vw[a, b] = 0;
					}
				}

				RedrawView();
				RedrawLineTypes();
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
	}
}
