using System.Collections.Generic;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using XOutput.Logging;

namespace XOutput.Devices.XInput.Vigem
{
	/// <summary>
	/// ViGEm XOutput implementation.
	/// </summary>
	public sealed class VigemDevice : IXOutputInterface
	{
		private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(VigemDevice));

		private static readonly bool deviceDriverAvailable;

		private static bool CheckDriverAvailable()
		{
			try
			{
				new ViGEmClient().Dispose();
				return true;
			}
			catch
			{
				// all kind of exceptions mean that ViGEm is not available (or at least not useable)
				return false;
			}
		}

		static VigemDevice()
		{
			deviceDriverAvailable = CheckDriverAvailable();
		}

		private readonly ViGEmClient client;
		private readonly Dictionary<int, IVirtualGamepad> controllers = new();

		public VigemDevice()
		{
			client = new ViGEmClient();
		}

		/// <summary>
		/// Gets if <see cref="VigemDevice"/> is available.
		/// </summary>
		/// <returns></returns>
		public static bool IsAvailable()
		{
			return deviceDriverAvailable;
		}

		/// <summary>
		/// Implements <see cref="IXOutputInterface.Plugin(int)"/>
		/// </summary>
		/// <param name="controllerCount">number of controller</param>
		/// <returns>If it was successful</returns>
		public bool Plugin(int controllerCount)
		{
			return Plugin(controllerCount, EmulatedControllerType.Xbox360);
		}

		/// <summary>
		/// Adds a new controller to the system.
		/// Because Vigem supports Xbox360 and DualShock4 controllers you can specify the type of the controller.
		/// </summary>
		/// <param name="controllerCount">number of controller</param>
		/// <returns>If it was successful</returns>
		public bool Plugin(int controllerCount, EmulatedControllerType type)
		{
			var controller = type.CreateController(client);
			controller.Connect();
			controllers.Add(controllerCount, controller);
			return true;
		}

		/// <summary>
		/// Implements <see cref="IXOutputInterface.Unplug(int)"/>
		/// </summary>
		/// <param name="controllerCount">number of controller</param>
		/// <returns>If it was successful</returns>
		public bool Unplug(int controllerCount)
		{
			if (controllers.ContainsKey(controllerCount))
			{
				var controller = controllers[controllerCount];
				controllers.Remove(controllerCount);
				controller.Disconnect();
				return true;
			}
			return false;
		}

		private void SetControllerValues(IVirtualGamepad controller, Dictionary<XInputTypes, double> values)
		{
			var mappingCollection = VigemMappingCollection.MappingCollectionLookup[controller.GetControllerType()];
			// required for DualShock4 to set the DPad direction at the end
			var dPadDirection = DPadDirection.None;

			foreach (var valueEntry in values)
			{
				if (valueEntry.Key.IsAxis())
				{
					var mapping = mappingCollection.axisMappings[valueEntry.Key];
					switch (controller)
					{
						case IXbox360Controller xbox360Controller:
							var value = mapping.GetValueForXbox(valueEntry.Value);
							xbox360Controller.SetAxisValue((Xbox360Axis)mapping.Type, value);
							break;
						case IDualShock4Controller dualShock4Controller:
							var byteValue = mapping.GetValueForDS4(valueEntry.Value);
							dualShock4Controller.SetAxisValue((DualShock4Axis)mapping.Type, byteValue);
							break;
					};
				}
				else if (valueEntry.Key.IsSlider())
				{
					var mapping = mappingCollection.sliderMappings[valueEntry.Key];
					var value = mapping.GetValue(valueEntry.Value);
					switch (controller)
					{
						case IXbox360Controller xbox360Controller:
							xbox360Controller.SetSliderValue((Xbox360Slider)mapping.Type, value);
							break;
						case IDualShock4Controller dualShock4Controller:
							dualShock4Controller.SetSliderValue((DualShock4Slider)mapping.Type, value);
							break;
					};
				}
				else
				{
					var mapping = mappingCollection.buttonMappings[valueEntry.Key];
					var value = mapping.GetValue(valueEntry.Value);
					switch (controller)
					{
						case IXbox360Controller xbox360Controller:
							xbox360Controller.SetButtonState((Xbox360Button)mapping.Type, value);
							break;
						case IDualShock4Controller dualShock4Controller:
							{
								if (mapping is VigemDualShock4DPadDirectionButtonMapping dualShock4DPadMapping)
								{
									dPadDirection |= value ? dualShock4DPadMapping.DPadDirection : DPadDirection.None;
								}
								else
								{
									dualShock4Controller.SetButtonState((DualShock4Button)mapping.Type, value);
								}
								break;
							}
					};
				}
			}

			// for DualShock4 set DPad direction at the end
			if (controller is IDualShock4Controller dualShock4Controller2)
			{
				dualShock4Controller2.SetDPadDirection(VigemDualShock4Mappings.GetDualShock4DPadDirection(dPadDirection));
			}
		}

		/// <summary>
		/// Implements <see cref="IXOutputInterface.Report(int, Dictionary{XInputTypes, double})"/>
		/// </summary>
		/// <param name="controllerCount">Number of controller</param>
		/// <param name="values">values for each XInput</param>
		/// <returns>If it was successful</returns>
		public bool Report(int controllerCount, Dictionary<XInputTypes, double> values)
		{
			if (controllers.ContainsKey(controllerCount))
			{
				var controller = controllers[controllerCount];
				SetControllerValues(controller, values);
				return true;
			}
			return false;
		}

		public void Dispose()
		{
			foreach (var controller in controllers.Values)
			{
				controller.Disconnect();
			}
			client.Dispose();
		}

		public IVirtualGamepad GetController(int controllerCount)
		{
			return controllers[controllerCount];
		}

	}
}
