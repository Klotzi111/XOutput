using System.Windows.Input;
using XOutput.Devices.Input.Virtual;

namespace XOutput.Devices.Input.Keyboard
{
	/// <summary>
	/// Direct input source.
	/// </summary>
	public class KeyboardSource : InputSource, IBasicVirtualInputSource
	{
		private readonly Key key;

		public KeyboardSource(IInputDevice inputDevice, string name, Key key) : base(inputDevice, name, InputSourceTypes.Button, (int)key)
		{
			this.key = key;
		}

		internal bool Refresh()
		{
			double newValue = System.Windows.Input.Keyboard.IsKeyDown(key) ? 1 : 0;
			return RefreshValue(newValue);
		}

		bool IBasicVirtualInputSource.Refresh() => Refresh();
	}
}
