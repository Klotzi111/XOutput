using Gma.System.MouseKeyHook;

namespace XOutput.Devices.Input
{
	public static class GlobalInputEventHelper
	{
		// Important to use the same instance of the global input event manager
		// Otherwise we will waste a lot of resources
		public static readonly IKeyboardMouseEvents GlobalInputEventManager = Hook.GlobalEvents();
	}
}
