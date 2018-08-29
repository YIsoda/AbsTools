namespace AbsConvertCore
{
	using static System.Console;
	class Program
	{
		static void Main(string[] args)
		{
			CommandLineApp.RunApplication(args);
#if DEBUG
			ReadKey();
#endif
		}
	}
}