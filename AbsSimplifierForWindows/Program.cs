namespace AbsSimplifierForWindows
{
	using AbsConvertCore;
	class Program
	{
		static void Main(string[] args)
		{
			CommandLineApplication.RunApplication(args);
#if Debug
			ReadKey();
#endif
		}
	}
}
