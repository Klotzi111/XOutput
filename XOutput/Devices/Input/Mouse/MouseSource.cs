using System.Windows.Input;
using Gma.System.MouseKeyHook;

namespace XOutput.Devices.Input.Mouse
{
	/// <summary>
	/// Direct input source.
	/// </summary>
	public class MouseSource : InputSource
	{
		private readonly MouseButton key;
		private double state = 0;

		public MouseSource(IInputDevice inputDevice, string name, MouseButton key) : base(inputDevice, name, InputSourceTypes.Button, (int)key)
		{
			this.key = key;
			GlobalInputEventHelper.GlobalInputEventManager.MouseDownExt += MouseEventHandler;
			GlobalInputEventHelper.GlobalInputEventManager.MouseUpExt += MouseEventHandler;
		}

		private void MouseEventHandler(object? sender, MouseEventExtArgs args)
		{
			if (args.Button.ToWPFMouseButton() == key)
			{
				if (args.IsMouseButtonUp != args.IsMouseButtonDown)
				{
					state = args.IsMouseButtonDown ? 1 : 0;
				} // else: What? The button is up and down at the same time?!
			}
		}

		internal bool Refresh()
		{
			return RefreshValue(state);
		}
	}
}
