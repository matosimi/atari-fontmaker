using System.Text;
using TinyJson;

namespace FontMaker
{
	public class ConfigurationJSON
	{
		public List<string> ColorSets { get; set; }
		public int AnalysisColor { get; set; }
		public int AnalysisAlpha { get; set; }

	}
	public static class Configuration
	{
		public static string Filename = Path.Join(AppContext.BaseDirectory, "FontMaker.json");

		public static List<string> ColorSets { get; set; }
		public static int AnalysisColor { get; set; }
		public static int AnalysisAlpha { get; set; }

		public static void Load()
		{
			try
			{
				var jsonText = File.ReadAllText(Filename);
				var jsonObj = jsonText.FromJson<ConfigurationJSON>();

				ColorSets = jsonObj.ColorSets;
				AnalysisColor = jsonObj.AnalysisColor;
				AnalysisAlpha = jsonObj.AnalysisAlpha;
			}
			catch
			{

			}

			VerifyDefaults();
		}

		public static void Save()
		{
			try
			{
				var jo = new ConfigurationJSON();
				jo.ColorSets = ColorSets;
				jo.AnalysisColor = AnalysisColor;
				jo.AnalysisAlpha = AnalysisAlpha;

				var txt = jo.ToJson();

				File.WriteAllText(Filename, txt, Encoding.UTF8);
			}
			catch
			{
			}
		}

		public static void VerifyDefaults()
		{
			// Make sure that there are 6 color sets
			if (ColorSets == null)
				ColorSets = new List<string>();

			if (ColorSets.Count < 6)
			{
				for (var i = ColorSets.Count; i < 6; ++i)
				{
					ColorSets.Add("0E0028CA9446");
				}
			}

			if (AnalysisColor < FontAnalysisWindow.AnalysisMinColorIndex || AnalysisColor > FontAnalysisWindow.AnalysisMaxColorIndex)
			{
				AnalysisColor = FontAnalysisWindow.AnalysisMinColorIndex;
			}

			if (AnalysisAlpha < FontAnalysisWindow.AnalysisMinAlpha || AnalysisAlpha > FontAnalysisWindow.AnalysisMaxAlpha)
			{
				AnalysisAlpha = FontAnalysisWindow.AnalysisMinAlpha + (FontAnalysisWindow.AnalysisMaxAlpha - FontAnalysisWindow.AnalysisMinAlpha) / 2;
			}
		}
	}

	public partial class FontMakerForm
	{
		public void LoadConfiguration()
		{
			Configuration.Load();

			// Transfer the loaded values
			ColorSets = Configuration.ColorSets;

			FontAnalysisWindowForm.SetDefaults(Configuration.AnalysisColor, Configuration.AnalysisAlpha);
		}

		public void SaveConfiguration()
		{
			// Transfer the settings into the configuration
			Configuration.ColorSets = ColorSets;
			Configuration.AnalysisColor = FontAnalysisWindowForm.GetHighlightColor;
			Configuration.AnalysisAlpha = FontAnalysisWindowForm.GetHighlightAlpha;

			Configuration.Save();
		}
	}
}
