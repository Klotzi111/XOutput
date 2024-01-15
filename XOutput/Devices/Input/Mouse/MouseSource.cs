using System.Windows.Input;
using Gma.System.MouseKeyHook;
using XOutput.Devices.Input.Virtual;

namespace XOutput.Devices.Input.Mouse
{
	/// <summary>
	/// Direct input source.
	/// </summary>
	public class MouseSource : InputSource, IBasicVirtualInputSource
	{
		private readonly MouseButton key;
		private double state = 0;

		public MouseSource(IInputDevice inputDevice, string name, MouseButton key) : base(inputDevice, name, InputSourceTypes.Button, (int)key)
		{
			this.key = key;
			GlobalInputEventHelper.GlobalInputEventManager.MouseDownExt += MouseEventHandler;
			GlobalInputEventHelper.GlobalInputEventManager.MouseUpExt += MouseEventHandler;
		}

		~MouseSource()
		{
			Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			// we always want to remove the handlers because either the object is to be disposed or the object is to be finalized
			// trying to remove if the handlers are not contained will not cause any harm
			GlobalInputEventHelper.GlobalInputEventManager.MouseDownExt -= MouseEventHandler;
			GlobalInputEventHelper.GlobalInputEventManager.MouseUpExt -= MouseEventHandler;

			// because from now on the state will never change we also set the state to 0
			state = 0;
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

		bool IBasicVirtualInputSource.Refresh() => Refresh();
	}
}
