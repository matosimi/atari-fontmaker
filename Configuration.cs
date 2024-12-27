using System.Text;
using TinyJson;

namespace FontMaker
{
	public class ConfigurationJson
	{
		public List<string>? ColorSets { get; set; } = [];
		public int AnalysisColor { get; set; }
		public int AnalysisAlpha { get; set; }

		public bool AnalysisDuplicates { get; set; }
		public int AnalysisDupColor { get; set; }
		public int AnalysisDupAlpha { get; set; }

		public bool ExportViewRemember {get; set; }
		public int ExportViewExportType { get; set; }
		public int ExportViewDataType { get; set; }
		public int ExportViewRegionX { get; set; }
		public int ExportViewRegionY { get; set; }
		public int ExportViewRegionW { get; set; }
		public int ExportViewRegionH { get; set; }
		public bool ExportViewTranspose { get; set; }

	}
	public static class Configuration
	{

		private static readonly string Filename = Path.Join(AppContext.BaseDirectory, "FontMaker.json");

		public static ConfigurationJson Values { get; set; } = new ConfigurationJson();

		public static void Load()
		{
			try
			{
				var jsonText = File.ReadAllText(Filename);				// Load JSON configuration file
				Values = jsonText.FromJson<ConfigurationJson>();		// Parse the JSON into an object
			}
			catch
			{
				// ignored
			}

			VerifyDefaults();
		}

		public static void Save()
		{
			try
			{
				var txt = Values.ToJson();

				File.WriteAllText(Filename, txt, Encoding.UTF8);
			}
			catch
			{
				// ignored
			}
		}

		public static void VerifyDefaults()
		{
			Values ??= new ConfigurationJson();
			// Make sure that there are 6 color sets
			Values.ColorSets ??= [];

			if (Values.ColorSets.Count < 6)
			{
				for (var i = Values.ColorSets.Count; i < 6; ++i)
				{
					Values.ColorSets.Add("0E0028CA9446");
				}
			}

			if (Values.AnalysisColor < FontAnalysisWindow.AnalysisMinColorIndex || Values.AnalysisColor > FontAnalysisWindow.AnalysisMaxColorIndex)
			{
				Values.AnalysisColor = FontAnalysisWindow.AnalysisMinColorIndex;
			}

			if (Values.AnalysisAlpha < FontAnalysisWindow.AnalysisMinAlpha || Values.AnalysisAlpha > FontAnalysisWindow.AnalysisMaxAlpha)
			{
				Values.AnalysisAlpha = FontAnalysisWindow.AnalysisMinAlpha + (FontAnalysisWindow.AnalysisMaxAlpha - FontAnalysisWindow.AnalysisMinAlpha) / 2;
			}

			if (Values.AnalysisDupColor < FontAnalysisWindow.AnalysisMinColorIndex || Values.AnalysisDupColor > FontAnalysisWindow.AnalysisMaxDuplicateColorIndex)
			{
				Values.AnalysisDupColor = FontAnalysisWindow.AnalysisMinColorIndex;
			}
			if (Values.AnalysisDupAlpha < FontAnalysisWindow.AnalysisMinAlpha || Values.AnalysisDupAlpha > FontAnalysisWindow.AnalysisMaxAlpha)
			{
				Values.AnalysisDupAlpha = FontAnalysisWindow.AnalysisMinAlpha + (FontAnalysisWindow.AnalysisMaxAlpha - FontAnalysisWindow.AnalysisMinAlpha) / 2;
			}

			if (Values.ExportViewRegionX is < 0 or >= 40) Values.ExportViewRegionX = 0;
			if (Values.ExportViewRegionY is < 0 or >= 26) Values.ExportViewRegionY = 0;
			if (Values.ExportViewRegionX + Values.ExportViewRegionW >= 40) Values.ExportViewRegionW = 1;
			if (Values.ExportViewRegionY + Values.ExportViewRegionH >= 26) Values.ExportViewRegionH = 1;
		}
	}

	public partial class FontMakerForm
	{
		public void LoadConfiguration()
		{
			Configuration.Load();

			// Transfer the loaded values
			ColorSets = Configuration.Values.ColorSets;

			FontAnalysisWindowForm.SetDefaults(
				Configuration.Values.AnalysisColor,
				Configuration.Values.AnalysisAlpha,
				Configuration.Values.AnalysisDuplicates,
				Configuration.Values.AnalysisDupColor,
				Configuration.Values.AnalysisDupAlpha
			);
			ExportViewWindowForm.LoadConfiguration(
				Configuration.Values.ExportViewRemember,
				Configuration.Values.ExportViewExportType,
				Configuration.Values.ExportViewDataType,
				new Rectangle(Configuration.Values.ExportViewRegionX, Configuration.Values.ExportViewRegionY, Configuration.Values.ExportViewRegionW, Configuration.Values.ExportViewRegionH),
				Configuration.Values.ExportViewTranspose
			);
		}

		public void SaveConfiguration()
		{
			// Transfer the settings into the configuration
			Configuration.Values.ColorSets = ColorSets;
			Configuration.Values.AnalysisColor = FontAnalysisWindowForm.GetHighlightColor;
			Configuration.Values.AnalysisAlpha = FontAnalysisWindowForm.GetHighlightAlpha;

			Configuration.Values.AnalysisDuplicates = FontAnalysisWindowForm.GetDuplicates;
			Configuration.Values.AnalysisDupColor = FontAnalysisWindowForm.GetDuplicateColor;
			Configuration.Values.AnalysisDupAlpha = FontAnalysisWindowForm.GetDuplicateAlpha;

			(Configuration.Values.ExportViewRemember, Configuration.Values.ExportViewExportType, Configuration.Values.ExportViewDataType, var exportRegion, Configuration.Values.ExportViewTranspose)
				= ExportViewWindowForm.SaveConfiguration();
			Configuration.Values.ExportViewRegionX = exportRegion.X;
			Configuration.Values.ExportViewRegionY = exportRegion.Y;
			Configuration.Values.ExportViewRegionW = exportRegion.Width;
			Configuration.Values.ExportViewRegionH = exportRegion.Height;

			Configuration.Save();
		}
	}
}
