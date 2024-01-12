using System;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using XOutput.Devices.XInput.Vigem;

namespace XOutput.Devices
{
	public enum EmulatedControllerType
	{
		Xbox360,
		DualShock4,
	}

	public static class EmulatedControllerTypeHelper
	{
		public static IVirtualGamepad CreateController(this EmulatedControllerType type, ViGEmClient client)
		{
			return type switch
			{
				EmulatedControllerType.Xbox360 => client.CreateXbox360Controller(),
				EmulatedControllerType.DualShock4 => client.CreateDualShock4Controller(),
				_ => throw new ArgumentException($"Invalid type: {type}"),
			};
		}

		/// <summary>
		/// Creates a mapping collection for the given controller type.
		/// NOTE: You probably never need to create a new instance of <see cref="VigemMappingCollection"/> because that class is immutable
		/// and there is <see cref="VigemMappingCollection.MappingCollectionLookup"/> that already contains all instances.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static VigemMappingCollection CreateMapping(this EmulatedControllerType type)
		{
			return type switch
			{
				EmulatedControllerType.Xbox360 => VigemXBox360Mappings.CreateMappingCollection(),
				EmulatedControllerType.DualShock4 => VigemDualShock4Mappings.CreateMappingCollection(),
				_ => throw new ArgumentException($"Invalid {nameof(EmulatedControllerType)}"),
			};
		}

		public static EmulatedControllerType GetControllerType(this IVirtualGamepad controller)
		{
			return controller switch
			{
				IXbox360Controller => EmulatedControllerType.Xbox360,
				IDualShock4Controller => EmulatedControllerType.DualShock4,
				_ => throw new ArgumentException($"Invalid controller type: {controller.GetType()}"),
			};
		}
	}
}
