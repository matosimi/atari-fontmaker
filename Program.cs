namespace FontMaker
{
	internal static class Program
	{
		public static FontMakerForm MainForm { get; set; }

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.SetHighDpiMode(HighDpiMode.DpiUnaware);

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			MainForm = new FontMakerForm();
			Application.Run(MainForm);
		}
	}
}