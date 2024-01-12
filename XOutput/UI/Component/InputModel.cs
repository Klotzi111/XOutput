using System.Windows.Media;
using XOutput.Devices.Input;

namespace XOutput.UI.Component
{
	public class InputModel : ModelBase
	{
		// assign fallback so we can ensure that the controller is never null
		private IInputDevice device = new FakeDevice();
		public IInputDevice Device
		{
			get => device;
			set
			{
				if (device != value)
				{
					device = value;
					OnPropertyChanged(nameof(Device));
				}
			}
		}

		// assign fallback so we can ensure that the controller is never null
		private Brush background = Brushes.White;
		public Brush Background
		{
			get => background;
			set
			{
				if (background != value)
				{
					background = value;
					OnPropertyChanged(nameof(Background));
				}
			}
		}

		public string DisplayName { get { return string.Format("{0} ({1})", device?.DisplayName ?? "<null>", device?.UniqueId ?? "<null>"); } }
	}
}
