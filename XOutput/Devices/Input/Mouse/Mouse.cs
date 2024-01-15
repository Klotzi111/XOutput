using System;
using System.Linq;
using System.Windows.Input;
using XOutput.Devices.Input.Virtual;

namespace XOutput.Devices.Input.Mouse
{
	/// <summary>
	/// Mouse input device.
	/// </summary>
	public sealed class Mouse : BasicVirtualInputDevice<MouseSource>
	{
		/// <summary>
		/// Creates a new mouse device instance.
		/// </summary>
		public Mouse()
			: base("Mouse", LanguageModel.Instance.Translate("Mouse"))
		{
		}

		~Mouse()
		{
			Dispose();
		}

		protected override MouseSource[] GetSources()
		{
			return Enum.GetValues(typeof(MouseButton)).OfType<MouseButton>().Select(x => new MouseSource(this, x.ToString(), x)).ToArray();
		}
	}
}
