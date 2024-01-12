using System.Windows.Media;
using XOutput.Devices;

namespace XOutput.UI.Component
{
	public class ControllerModel : ModelBase
	{
		// assign fallback so we can ensure that the controller is never null
		private GameController controller = new GameController(new Devices.Mapper.InputMapper("FAKE_NAME", "FAKE_ID"));
		public GameController Controller
		{
			get => controller;
			set
			{
				if (controller != value)
				{
					controller = value;
					OnPropertyChanged(nameof(Controller));
				}
			}
		}

		// assign fallback so we can ensure that the controller is never null
		private string buttonText = "UNKNOWN";
		public string ButtonText
		{
			get => buttonText;
			set
			{
				if (buttonText != value)
				{
					buttonText = value;
					OnPropertyChanged(nameof(ButtonText));
				}
			}
		}
		private bool started;
		public bool Started
		{
			get => started;
			set
			{
				if (started != value)
				{
					started = value;
					OnPropertyChanged(nameof(Started));
				}
			}
		}

		private bool canStart;
		public bool CanStart
		{
			get => canStart;
			set
			{
				if (canStart != value)
				{
					canStart = value;
					OnPropertyChanged(nameof(CanStart));
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

		public string DisplayName => Controller.ToString();

		public void RefreshName()
		{
			OnPropertyChanged(nameof(DisplayName));
		}
	}
}
