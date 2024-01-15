using XOutput.Tools;

namespace XOutput
{
	public static class ApplicationConfiguration
	{
		[ResolverMethod]
		public static ArgumentParser GetArgumentParser()
		{
			return new ArgumentParser();
		}
		[ResolverMethod]
		public static HidGuardianManager GetHidGuardianManager()
		{
			return new HidGuardianManager();
		}
		[ResolverMethod]
		public static RegistryModifier GetRegistryModifier()
		{
			return new RegistryModifier();
		}
	}
}
