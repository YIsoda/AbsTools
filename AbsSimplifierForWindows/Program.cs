namespace AbsSimplifierForWindows
{
	using static System.Console;
	using AbsConvertCore;
	class Program
	{
		static void Main(string[] args)
		{
			CommandLineApp.RunApplication(args);
#if Debug
			ReadKey();
#endif
		}
	}
}
