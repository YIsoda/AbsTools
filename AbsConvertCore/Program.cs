namespace AbsConvertCore
{
	using static System.Console;
	class Program
	{
		static void Main(string[] args)
		{
			CommandLineApplication.RunApplication(args);
#if DEBUG
			ReadKey();
#endif
		}
	}
}