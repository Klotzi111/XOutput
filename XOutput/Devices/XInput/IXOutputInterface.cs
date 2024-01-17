using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace XOutput.Devices.XInput
{
	/// <summary>
	/// Main interface for XOutput devices.
	/// </summary>
	public interface IXOutputInterface : IDisposable
	{
		/// <summary>
		/// Gets all the controller types that this controller can be emulated as.
		/// </summary>
		public ImmutableArray<EmulatedControllerType> SupportedControllerTypes { get; }
		/// <summary>
		/// Gets the default controller type to emulate.
		/// </summary>
		public EmulatedControllerType DefaultControllerType { get; }

		/// <summary>
		/// Plugs in a new virutal XInput device.
		/// </summary>
		/// <param name="controllerCount">number of controller</param>
		/// <returns>If it was successful</returns>
		bool Plugin(int controllerCount);
		/// <summary>
		/// Unplugs an existing virutal XInput device.
		/// </summary>
		/// <param name="controllerCount">number of controller</param>
		/// <returns>If it was successful</returns>
		bool Unplug(int controllerCount);
		/// <summary>
		/// Sends data to the virtual XInput device.
		/// </summary>
		/// <param name="controllerCount">Number of controller</param>
		/// <param name="values">values for each XInput</param>
		/// <returns>If it was successful</returns>
		bool Report(int controllerCount, Dictionary<XInputTypes, double> values);
	}
}
