using System;
using System.Linq;
using System.Threading;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using XOutput.Devices.Input;
using XOutput.Devices.Input.DirectInput;
using XOutput.Devices.Mapper;
using XOutput.Devices.XInput;
using XOutput.Devices.XInput.SCPToolkit;
using XOutput.Devices.XInput.Vigem;
using XOutput.Logging;

namespace XOutput.Devices
{
	/// <summary>
	/// GameController is a container for input devices, mappers and output devices.
	/// </summary>
	public sealed class GameController : IDisposable
	{
		/// <summary>
		/// Gets the output device.
		/// </summary>
		public XOutputDevice XInput => xInput;
		/// <summary>
		/// Gets the mapping of the input device.
		/// </summary>
		public InputMapper Mapper => mapper;
		/// <summary>
		/// Gets the name of the input device.
		/// </summary>
		public string DisplayName => mapper.Name;
		/// <summary>
		/// Gets the number of the controller.
		/// </summary>
		public int ControllerCount => controllerCount;
		/// <summary>
		/// Gets if any XInput emulation is installed.
		/// </summary>
		public bool HasXOutputInstalled => xOutputInterface != null;
		/// <summary>
		/// Gets if force feedback is supported.
		/// </summary>
		public bool ForceFeedbackSupported => xOutputInterface is VigemDevice;
		/// <summary>
		/// Gets if using different controller types is supported.
		/// </summary>
		public bool ControllerTypeSupported => xOutputInterface is VigemDevice;
		/// <summary>
		/// Gets the force feedback device.
		/// </summary>
		public IInputDevice? ForceFeedbackDevice { get; set; }

		private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GameController));

		private readonly InputMapper mapper;
		private readonly XOutputDevice xInput;
		private readonly IXOutputInterface? xOutputInterface;

		private bool running;
		private Action? afterStopAction;
		private int controllerCount = 0;
		private IVirtualGamepad? controller;
		private Delegate? controllerEventHandler = null;

		public GameController(InputMapper mapper)
		{
			this.mapper = mapper;
			xOutputInterface = CreateXOutput();
			xInput = new XOutputDevice(mapper);
			if (!string.IsNullOrEmpty(mapper.ForceFeedbackDevice))
			{
				var device = InputDevices.Instance.GetDevices().OfType<DirectDevice>().FirstOrDefault(d => d.UniqueId == mapper.ForceFeedbackDevice);
				if (device != null)
				{
					ForceFeedbackDevice = device;
				}
			}
		}

		private IXOutputInterface? CreateXOutput()
		{
			if (VigemDevice.IsAvailable())
			{
				logger.Info("ViGEm devices are used.");
				return new VigemDevice();
			}
			else if (ScpDevice.IsAvailable())
			{
				logger.Warning("SCP Toolkit devices are used.");
				return new ScpDevice();
			}
			else
			{
				logger.Error("Neither ViGEm nor SCP devices can be used.");
				return null;
			}
		}

		~GameController()
		{
			Dispose();
		}

		/// <summary>
		/// Disposes all used resources
		/// </summary>
		public void Dispose()
		{
			Stop();
			xInput?.Dispose();
			xOutputInterface?.Dispose();
		}

		/// <summary>
		/// Starts the emulation of the device
		/// </summary>
		public int Start(Action? afterStopAction = null)
		{
			if (!HasXOutputInstalled)
			{
				return 0;
			}
			controllerCount = Controllers.Instance.GetId();
			if (controller != null)
			{
				RemoveControllerEventHandlers();
			}
			if (xOutputInterface == null)
			{
				return 0;
			}
			if (xOutputInterface.Unplug(controllerCount))
			{
				// Wait for unplugging
				Thread.Sleep(10);
			}

			var pluginResult = false;
			if (xOutputInterface is VigemDevice vigem)
			{
				pluginResult = vigem.Plugin(controllerCount, EmulatedControllerType.DualShock4);
			}
			else
			{
				pluginResult = xOutputInterface.Plugin(controllerCount);
			}
			if (pluginResult)
			{
				XInput.InputChanged += XInputInputChanged;
				this.afterStopAction = afterStopAction;
				running = true;

				logger.Info($"Emulation started on {ToString()}.");
				if (ForceFeedbackSupported)
				{
					logger.Info($"Force feedback mapping is connected on {ToString()}.");
					controller = ((VigemDevice)xOutputInterface).GetController(controllerCount);
					if (controller is IXbox360Controller xbox360Controller)
					{
						xbox360Controller.FeedbackReceived += ControllerFeedbackReceivedXboxOne;
						controllerEventHandler = (Xbox360FeedbackReceivedEventHandler)ControllerFeedbackReceivedXboxOne;
					}
					else if (controller is IDualShock4Controller dualShock4Controller)
					{
						dualShock4Controller.FeedbackReceived += ControllerFeedbackReceivedDualShock4;
						controllerEventHandler = (DualShock4FeedbackReceivedEventHandler)ControllerFeedbackReceivedDualShock4;
					}
				}
			}
			else
			{
				ResetId();
			}
			return controllerCount;
		}

		private void RemoveControllerEventHandlers()
		{
			if (controllerEventHandler != null)
			{
				if (controller is IXbox360Controller xbox360Controller)
				{
					xbox360Controller.FeedbackReceived -= (Xbox360FeedbackReceivedEventHandler)controllerEventHandler;
				}
				else if (controller is IDualShock4Controller dualShock4Controller)
				{
					dualShock4Controller.FeedbackReceived -= (DualShock4FeedbackReceivedEventHandler)controllerEventHandler;
				}
			}
			controllerEventHandler = null;
		}

		/// <summary>
		/// Stops the emulation of the device
		/// </summary>
		public void Stop()
		{
			if (running)
			{
				running = false;
				XInput.InputChanged -= XInputInputChanged;

				if (ForceFeedbackSupported && controller != null)
				{
					RemoveControllerEventHandlers();

					logger.Info($"Force feedback mapping is disconnected on {ToString()}.");
				}
				xOutputInterface?.Unplug(controllerCount);

				logger.Info($"Emulation stopped on {ToString()}.");
				ResetId();
				afterStopAction?.Invoke();
				afterStopAction = null;
			}
		}

		public override string ToString()
		{
			return DisplayName;
		}

		private void XInputInputChanged(object sender, DeviceInputChangedEventArgs e)
		{
			if (xOutputInterface?.Report(controllerCount, XInput.GetValues()) == false)
			{
				Stop();
			}
		}

		private void ControllerFeedbackReceivedXboxOne(object sender, Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360FeedbackReceivedEventArgs e)
		{
			ForceFeedbackDevice?.SetForceFeedback((double)e.LargeMotor / byte.MaxValue, (double)e.SmallMotor / byte.MaxValue);
		}

		private void ControllerFeedbackReceivedDualShock4(object sender, Nefarius.ViGEm.Client.Targets.DualShock4.DualShock4FeedbackReceivedEventArgs e)
		{
			ForceFeedbackDevice?.SetForceFeedback((double)e.LargeMotor / byte.MaxValue, (double)e.SmallMotor / byte.MaxValue);
		}

		private void ResetId()
		{
			if (controllerCount != 0)
			{
				Controllers.Instance.DisposeId(controllerCount);
				controllerCount = 0;
			}
		}
	}
}
