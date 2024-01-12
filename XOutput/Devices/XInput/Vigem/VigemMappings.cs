using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace XOutput.Devices.XInput.Vigem
{
	/// <summary>
	/// Holds all the mappings for a Vigem device.
	/// This class is immutable.
	/// </summary>
	public class VigemMappingCollection
	{
		public static readonly FrozenDictionary<EmulatedControllerType, VigemMappingCollection> MappingCollectionLookup =
			Enum.GetValues<EmulatedControllerType>()
			.Select(t => new KeyValuePair<EmulatedControllerType, VigemMappingCollection>(t, t.CreateMapping()))
			.ToFrozenDictionary();

		public readonly FrozenDictionary<XInputTypes, IVigemButtonMapping> buttonMappings;
		public readonly FrozenDictionary<XInputTypes, IVigemAxisMapping> axisMappings;
		public readonly FrozenDictionary<XInputTypes, IVigemSliderMapping> sliderMappings;

		public VigemMappingCollection(IReadOnlyDictionary<XInputTypes, IVigemButtonMapping> buttonMappings,
			IReadOnlyDictionary<XInputTypes, IVigemAxisMapping> axisMappings,
			IReadOnlyDictionary<XInputTypes, IVigemSliderMapping> sliderMappings)
		{
			this.buttonMappings = buttonMappings.ToFrozenDictionary();
			this.axisMappings = axisMappings.ToFrozenDictionary();
			this.sliderMappings = sliderMappings.ToFrozenDictionary();
		}
	}

	public interface IVigemMappings
	{
		public static abstract VigemMappingCollection CreateMappingCollection();
	}

	/// <summary>
	/// Mapping value helper for Vigem buttons
	/// </summary>
	public interface IVigemButtonMapping
	{
		public object Type { get; }

		public bool GetValue(double value)
		{
			return value > 0.5;
		}
	}

	/// <summary>
	/// Mapping value helper for Vigem axes
	/// </summary>
	public interface IVigemAxisMapping
	{
		public object Type { get; }

		/// <summary>
		/// Used by Xbox controllers
		/// </summary>
		public short GetValueForXbox(double value)
		{
			return (short)((value - 0.5) * 2 * short.MaxValue);
		}

		/// <summary>
		/// Used by DualShock4 controllers
		/// </summary>
		public byte GetValueForDS4(double value)
		{
			return (byte)(value * byte.MaxValue);
		}
	}

	/// <summary>
	/// Mapping value helper for Vigem axes
	/// </summary>
	public interface IVigemSliderMapping
	{
		public object Type { get; }

		public byte GetValue(double value)
		{
			return (byte)(value * byte.MaxValue);
		}
	}
}
