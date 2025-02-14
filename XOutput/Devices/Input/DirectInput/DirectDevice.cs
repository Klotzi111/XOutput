using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using SharpDX;
using SharpDX.DirectInput;
using XOutput.Logging;
using XOutput.Tools;

namespace XOutput.Devices.Input.DirectInput
{
	/// <summary>
	/// Device that contains data for a DirectInput device
	/// </summary>
	public sealed class DirectDevice : IInputDevice
	{
		#region Constants
		/// <summary>
		/// The delay in milliseconds to sleep between input reads.
		/// </summary>
		public const int ReadDelayMs = 1;

		private static readonly Regex hidRegex = new Regex("(hid)#([^#]+)#([^#]+)");
		#endregion

		#region Events
		/// <summary>
		/// Triggered periodically to trigger input read from Direct input device.
		/// <para>Implements <see cref="IDevice.InputChanged"/></para>
		/// </summary>
		public event DeviceInputChangedHandler? InputChanged;
		/// <summary>
		/// Triggered when the any read or write fails.
		/// <para>Implements <see cref="IInputDevice.Disconnected"/></para>
		/// </summary>
		public event DeviceDisconnectedHandler? Disconnected;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the GUID of the controller.
		/// </summary>
		public Guid Id => deviceInstance.InstanceGuid;
		/// <summary>
		/// <para>Implements <see cref="IInputDevice.UniqueId"/></para>
		/// </summary>
		public string UniqueId => deviceInstance.InstanceGuid.ToString();
		/// <summary>
		/// Gets the product name of the device.
		/// <para>Implements <see cref="IInputDevice.DisplayName"/></para>
		/// </summary>
		public string DisplayName => deviceInstance.ProductName;
		/// <summary>
		/// Gets or sets if the device is connected and ready to use.
		/// <para>Implements <see cref="IInputDevice.Connected"/></para>
		/// </summary>
		public bool Connected
		{
			get => connected;
			set
			{
				if (value != connected)
				{
					if (!connected)
					{
						Disconnected?.Invoke(this, new DeviceDisconnectedEventArgs());
					}
					connected = value;
				}
			}
		}
		/// <summary>
		/// <para>Implements <see cref="IDevice.DPads"/></para>
		/// </summary>
		public IEnumerable<DPadDirection> DPads => state.DPads;
		/// <summary>
		/// <para>Implements <see cref="IDevice.Sources"/></para>
		/// </summary>
		public IEnumerable<InputSource> Sources => sources;
		/// <summary>
		/// <para>Implements <see cref="IInputDevice.ForceFeedbackCount"/></para>
		/// </summary>
		public int ForceFeedbackCount => actuators.Count;
		/// <summary>
		/// <para>Implements <see cref="IInputDevice.InputConfiguration"/></para>
		/// </summary>
		public InputConfig InputConfiguration => inputConfig;

		public string? HardwareID
		{
			get
			{
				if (deviceInstance.IsHumanInterfaceDevice)
				{
					return IdHelper.GetHardwareId(joystick.Properties.InterfacePath);
				}
				return null;
			}
		}
		#endregion

		private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(DirectDevice));
		private readonly DeviceInstance deviceInstance;
		private readonly Joystick joystick;
		private readonly DirectInputSource[] sources;
		private readonly DeviceState state;
		private readonly EffectInfo? force;
		private readonly List<DirectDeviceForceFeedback> actuators = new List<DirectDeviceForceFeedback>();
		private readonly InputConfig inputConfig;
		private bool connected = false;
		private readonly Thread inputRefresher;
		private bool disposed = false;
		private DeviceInputChangedEventArgs deviceInputChangedEventArgs;

		/// <summary>
		/// Creates a new DirectDevice instance.
		/// </summary>
		/// <param name="deviceInstance">SharpDX instanse</param>
		/// <param name="joystick">SharpDX joystick</param>
		public DirectDevice(DeviceInstance deviceInstance, Joystick joystick)
		{
			this.deviceInstance = deviceInstance;
			this.joystick = joystick;
			var buttonObjectInstances = joystick.GetObjects(DeviceObjectTypeFlags.Button).Where(b => b.Usage > 0).OrderBy(b => b.ObjectId.InstanceNumber).Take(128).ToArray();
			var buttons = buttonObjectInstances.Select((b, i) => new DirectInputSource(this, "Button " + b.Usage, InputSourceTypes.Button, b.Offset, state => state.Buttons[i] ? 1 : 0)).ToArray();
			var axes = GetAxes().OrderBy(a => a.Usage).Take(24).Select(GetAxisSource);
			var sliders = GetSliders().OrderBy(a => a.Usage).Select(GetSliderSource);
			DirectInputSource[] dpads;
			var povCount = joystick.Capabilities.PovCount;
			if (povCount > 0)
			{
				dpads = new DirectInputSource[povCount * 4];
				for (var i = 0; i < povCount; i++)
				{
					var offset = i * 4;
					// this field is required because otherwise the lambda will use a reference to the variable i and therefore will always be wrong
					var localCopyOfI = i;
					dpads[offset + 0] = new DirectInputSource(this, "DPad" + (i + 1) + " Up", InputSourceTypes.Dpad, 1000 + offset, state => GetDPadValue(localCopyOfI, state).HasFlag(DPadDirection.Up) ? 1 : 0);
					dpads[offset + 1] = new DirectInputSource(this, "DPad" + (i + 1) + " Down", InputSourceTypes.Dpad, 1001 + offset, state => GetDPadValue(localCopyOfI, state).HasFlag(DPadDirection.Down) ? 1 : 0);
					dpads[offset + 2] = new DirectInputSource(this, "DPad" + (i + 1) + " Left", InputSourceTypes.Dpad, 1002 + offset, state => GetDPadValue(localCopyOfI, state).HasFlag(DPadDirection.Left) ? 1 : 0);
					dpads[offset + 3] = new DirectInputSource(this, "DPad" + (i + 1) + " Right", InputSourceTypes.Dpad, 1003 + offset, state => GetDPadValue(localCopyOfI, state).HasFlag(DPadDirection.Right) ? 1 : 0);
				}
			}
			else
			{
				dpads = Array.Empty<DirectInputSource>();
			}
			sources = buttons.Concat(axes).Concat(sliders).Concat(dpads).ToArray();

			joystick.Properties.AxisMode = DeviceAxisMode.Absolute;
			try
			{
				joystick.SetCooperativeLevel(new WindowInteropHelper(Application.Current.MainWindow).Handle, CooperativeLevel.Background | CooperativeLevel.Exclusive);
			}
			catch (Exception)
			{
				logger.Warning($"Failed to set cooperative level to exclusive for {ToString()}");
			}
			joystick.Acquire();
			if (deviceInstance.ForceFeedbackDriverGuid != Guid.Empty)
			{
				var constantForce = joystick.GetEffects().FirstOrDefault(x => x.Guid == EffectGuid.ConstantForce);
				if (constantForce == null)
				{
					force = joystick.GetEffects().FirstOrDefault();
				}
				else
				{
					force = constantForce;
				}

				if (force != null)
				{
					var actuatorAxes = joystick.GetObjects().Where(doi => doi.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.ForceFeedbackActuator)).ToArray();
					for (int i = 0; i < actuatorAxes.Length; i++)
					{
						if (i + 1 < actuatorAxes.Length)
						{
							actuators.Add(new DirectDeviceForceFeedback(joystick, force, actuatorAxes[i], actuatorAxes[i + 1]));
							i++;
						}
						else
						{
							actuators.Add(new DirectDeviceForceFeedback(joystick, force, actuatorAxes[i]));
						}
					}
				}
			}
			try
			{
				logger.Info(joystick.Properties.InstanceName + " " + ToString());
				logger.Info(PrettyPrint.ToString(joystick));
				logger.Info(PrettyPrint.ToString(joystick.GetObjects()));
			}
			catch { }
			foreach (var obj in joystick.GetObjects())
			{
				logger.Info("  " + obj.Name + " " + obj.ObjectId + " offset: " + obj.Offset + " objecttype: " + obj.ObjectType.ToString() + " " + obj.Usage);
			}
			state = new DeviceState(sources, joystick.Capabilities.PovCount);
			deviceInputChangedEventArgs = new DeviceInputChangedEventArgs(this);
			inputConfig = new InputConfig(ForceFeedbackCount);
			inputRefresher = new Thread(InputRefresher)
			{
				Name = ToString() + " input reader"
			};
			inputRefresher.SetApartmentState(ApartmentState.STA);
			inputRefresher.IsBackground = true;
			Connected = true;
			inputRefresher.Start();
		}

		~DirectDevice()
		{
			Dispose();
		}

		/// <summary>
		/// Disposes all resources.
		/// </summary>
		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				inputRefresher?.Interrupt();
				foreach (var actuator in actuators)
				{
					actuator.Dispose();
				}
				joystick.Dispose();
			}
		}

		/// <summary>
		/// Display name followed by the deviceID.
		/// <para>Overrides <see cref="object.ToString()"/></para>
		/// </summary>
		/// <returns>Friendly name</returns>
		public override string ToString()
		{
			return UniqueId;
		}

		private void InputRefresher()
		{
			try
			{
				while (Connected)
				{
					Connected = RefreshInput();
					Thread.Sleep(ReadDelayMs);
				}
			}
			catch (ThreadInterruptedException)
			{
				// Thread has been interrupted
			}
		}

		/// <summary>
		/// Gets the current state of the inputTpye.
		/// <para>Implements <see cref="IDevice.Get(InputSource)"/></para>
		/// </summary>
		/// <param name="source">Type of input</param>
		/// <returns>Value</returns>
		public double Get(InputSource source)
		{
			return source.Value;
		}

		/// <summary>
		/// Sets the force feedback motor values.
		/// <para>Implements <see cref="IInputDevice.SetForceFeedback(double, double)"/></para>
		/// </summary>
		/// <param name="big">Big motor value</param>
		/// <param name="small">Small motor value</param>
		public void SetForceFeedback(double big, double small)
		{
			if (ForceFeedbackCount == 0)
			{
				return;
			}
			if (!inputConfig.ForceFeedback)
			{
				big = 0;
				small = 0;
			}
			foreach (var actuator in actuators)
			{
				actuator.SetForceFeedback(big, small);
			}
		}

		/// <summary>
		/// Refreshes the current state. Triggers <see cref="InputChanged"/> event.
		/// </summary>
		/// <returns>if the input was available</returns>
		public bool RefreshInput(bool force = false)
		{
			state.ResetChanges();
			if (!disposed)
			{
				try
				{
					joystick.Poll();
					// get state only once for updating all sources
					var currentState = GetCurrentState();
					for (int i = 0; i < state.DPads.Count(); i++)
					{
						state.SetDPad(i, GetDPadValue(i, currentState));
					}
					foreach (var source in sources)
					{
						if (source.Refresh(currentState))
						{
							state.MarkChanged(source);
						}
					}
					var changes = state.GetChanges(force);
					var dpadChanges = state.GetChangedDpads(force);
					if (changes.Any() || dpadChanges.Any())
					{
						deviceInputChangedEventArgs.Refresh(changes, dpadChanges);
						InputChanged?.Invoke(this, deviceInputChangedEventArgs);
					}
					return true;
				}
				catch (Exception)
				{
					logger.Warning($"Poll failed for {ToString()}");
					return false;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the current value of an axis.
		/// </summary>
		/// <param name="axis">Axis index</param>
		/// <returns>Value</returns>
		private int GetAxisValue(int instanceNumber, JoystickState state)
		{
			if (instanceNumber < 0)
			{
				throw new ArgumentException(nameof(instanceNumber));
			}
			switch (instanceNumber)
			{
				case 0:
					return state.X;
				case 1:
					return ushort.MaxValue - state.Y;
				case 2:
					return state.Z;
				case 3:
					return state.RotationX;
				case 4:
					return ushort.MaxValue - state.RotationY;
				case 5:
					return state.RotationZ;
				case 6:
					return state.AccelerationX;
				case 7:
					return ushort.MaxValue - state.AccelerationY;
				case 8:
					return state.AccelerationZ;
				case 9:
					return state.AngularAccelerationX;
				case 10:
					return ushort.MaxValue - state.AngularAccelerationY;
				case 11:
					return state.AngularAccelerationZ;
				case 12:
					return state.ForceX;
				case 13:
					return ushort.MaxValue - state.ForceY;
				case 14:
					return state.ForceZ;
				case 15:
					return state.TorqueX;
				case 16:
					return ushort.MaxValue - state.TorqueY;
				case 17:
					return state.TorqueZ;
				case 18:
					return state.VelocityX;
				case 19:
					return ushort.MaxValue - state.VelocityY;
				case 20:
					return state.VelocityZ;
				case 21:
					return state.AngularVelocityX;
				case 22:
					return ushort.MaxValue - state.AngularVelocityY;
				case 23:
					return state.AngularVelocityZ;
				default:
					return 0;
			}
		}

		/// <summary>
		/// Gets the current value of a slider.
		/// </summary>
		/// <param name="slider">Slider index</param>
		/// <returns>Value</returns>
		private int GetSliderValue(int slider, JoystickState state)
		{
			if (slider < 1)
			{
				throw new ArgumentException(nameof(slider));
			}
			return state.Sliders[slider - 1];
		}

		/// <summary>
		/// Gets the current value of a DPad.
		/// </summary>
		/// <param name="dpad">DPad index</param>
		/// <returns>Value</returns>
		private DPadDirection GetDPadValue(int dpad, JoystickState state)
		{
			switch (state.PointOfViewControllers[dpad])
			{
				case -1: return DPadDirection.None;
				case 0: return DPadDirection.Up;
				case 4500: return DPadDirection.Up | DPadDirection.Right;
				case 9000: return DPadDirection.Right;
				case 13500: return DPadDirection.Down | DPadDirection.Right;
				case 18000: return DPadDirection.Down;
				case 22500: return DPadDirection.Down | DPadDirection.Left;
				case 27000: return DPadDirection.Left;
				case 31500: return DPadDirection.Up | DPadDirection.Left;
				default:
					throw new ArgumentException(nameof(dpad));
			}
		}

		/// <summary>
		/// Gets and initializes available axes for the device.
		/// </summary>
		/// <returns><see cref="DirectInputTypes"/> of the axes</returns>
		private DeviceObjectInstance[] GetAxes()
		{
			var axes = joystick.GetObjects(DeviceObjectTypeFlags.AbsoluteAxis).Where(o => o.ObjectType != ObjectGuid.Slider).ToArray();
			foreach (var axis in axes)
			{
				var properties = joystick.GetObjectPropertiesById(axis.ObjectId);
				try
				{
					properties.Range = new InputRange(ushort.MinValue, ushort.MaxValue);
					properties.DeadZone = 0;
					properties.Saturation = 10000;
				}
				catch (SharpDXException ex)
				{
					logger.Error(ex);
				}
			}
			return axes;
		}

		/// <summary>
		/// Gets available sliders for the device.
		/// </summary>
		/// <returns><see cref="DirectInputTypes"/> of the axes</returns>
		private DeviceObjectInstance[] GetSliders()
		{
			return joystick.GetObjects().Where(o => o.ObjectType == ObjectGuid.Slider).ToArray();
		}

		/// <summary>
		/// Reads the current state of the device.
		/// </summary>
		/// <returns>state</returns>
		private JoystickState GetCurrentState()
		{
			try
			{
				// creating the instance ourselves reduces the performance overhead of Activator.CreateInstance
				// because this method is called very often it helps a lot
				var state = new JoystickState();
				joystick.GetCurrentState(ref state);
				return state;
			}
			catch (Exception)
			{
				Connected = false;
				throw;
			}
		}

		private DirectInputSource GetAxisSource(DeviceObjectInstance instance)
		{
			InputSourceTypes type = InputSourceTypes.AxisX;
			if (instance.ObjectType == ObjectGuid.XAxis || instance.ObjectType == ObjectGuid.RxAxis)
			{
				type = InputSourceTypes.AxisX;
			}
			else if (instance.ObjectType == ObjectGuid.YAxis || instance.ObjectType == ObjectGuid.RyAxis)
			{
				type = InputSourceTypes.AxisY;
			}
			else if (instance.ObjectType == ObjectGuid.ZAxis || instance.ObjectType == ObjectGuid.RzAxis)
			{
				type = InputSourceTypes.AxisZ;
			}
			int axisCount;
			if (instance.Usage >= 48)
			{
				axisCount = instance.Usage - 48;
			}
			else
			{
				axisCount = instance.ObjectId.InstanceNumber;
			}
			string name = instance.Name;
			return new DirectInputSource(this, name, type, instance.Offset, state => GetAxisValue(axisCount, state) / (double)ushort.MaxValue);
		}

		private DirectInputSource GetSliderSource(DeviceObjectInstance instance, int i)
		{
			string name = instance.Name;
			return new DirectInputSource(this, name, InputSourceTypes.Slider, instance.Offset, state => GetSliderValue(i + 1, state) / (double)ushort.MaxValue);
		}
	}
}
