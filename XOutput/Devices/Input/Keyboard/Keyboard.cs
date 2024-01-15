using System;
using System.Linq;
using System.Windows.Input;
using XOutput.Devices.Input.Virtual;

namespace XOutput.Devices.Input.Keyboard
{
	/// <summary>
	/// Keyboard input device.
	/// </summary>
	public sealed class Keyboard : BasicVirtualInputDevice<KeyboardSource>
	{
		/// <summary>
		/// Creates a new keyboard device instance.
		/// </summary>
		public Keyboard()
			: base("Keyboard", LanguageModel.Instance.Translate("Keyboard"))
		{
		}

		~Keyboard()
		{
			Dispose();
		}

		protected override KeyboardSource[] GetSources()
		{
			return Enum.GetValues(typeof(Key)).OfType<Key>().Where(x => x != Key.None).OrderBy(x => x.ToString()).Select(x => new KeyboardSource(this, x.ToString(), x)).ToArray();
		}
	}
}
