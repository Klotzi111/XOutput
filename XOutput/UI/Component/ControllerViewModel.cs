using System;
using System.Windows.Media;
using System.Windows.Threading;
using XOutput.Devices;
using XOutput.UI.Windows;

namespace XOutput.UI.Component
{
	public class ControllerViewModel : ViewModelBase<ControllerModel>, IDisposable
	{
		private const int BackgroundDelayMS = 500;
		private readonly Action<string>? log;
		private readonly DispatcherTimer timer = new DispatcherTimer();
		private readonly bool isAdmin;

		public ControllerViewModel(ControllerModel model, GameController controller, bool isAdmin, Action<string>? log) : base(model)
		{
			this.log = log;
			this.isAdmin = isAdmin;
			Model.Controller = controller;
			Model.ButtonText = "Start";
			Model.Background = Brushes.White;
			Model.Controller.XInput.InputChanged += InputDevice_InputChanged;
			timer.Interval = TimeSpan.FromMilliseconds(BackgroundDelayMS);
			timer.Tick += Timer_Tick;
		}

		public void Edit()
		{
			var controller = Model.Controller;
			if (controller != null)
			{
				var controllerSettingsWindow = new ControllerSettingsWindow(new ControllerSettingsViewModel(new ControllerSettingsModel(), controller, isAdmin), controller);
				controllerSettingsWindow.ShowDialog();
				Model.RefreshName();
			}
		}

		public void StartStop()
		{
			if (!Model.Started)
			{
				Start();
			}
			else
			{
				Model.Controller?.Stop();
			}
		}

		public void Start()
		{
			if (!Model.Started)
			{
				int controllerCount = 0;
				var controller = Model.Controller;
				if (controller != null)
				{
					controllerCount = controller.Start(() =>
					{
						Model.ButtonText = "Start";
						log?.Invoke(string.Format(LanguageModel.Instance.Translate("EmulationStopped"), controller.DisplayName));
						Model.Started = false;
					});
					if (controllerCount != 0)
					{
						Model.ButtonText = "Stop";
						log?.Invoke(string.Format(LanguageModel.Instance.Translate("EmulationStarted"), controller.DisplayName, controllerCount));
					}
				}
				Model.Started = controllerCount != 0;
			}
		}

		public void Dispose()
		{
			timer.Tick -= Timer_Tick;
			var controller = Model.Controller;
			if (controller != null)
			{
				controller.XInput.InputChanged -= InputDevice_InputChanged;
			}
		}

		private void Timer_Tick(object? sender, EventArgs e)
		{
			Model.Background = Brushes.White;
		}

		private void InputDevice_InputChanged(object? sender, DeviceInputChangedEventArgs e)
		{
			Model.Background = Brushes.LightGreen;
			timer.Stop();
			timer.Start();
		}
	}
}
