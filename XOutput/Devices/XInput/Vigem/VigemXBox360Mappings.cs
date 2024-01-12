using System.Collections.Generic;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace XOutput.Devices.XInput.Vigem
{
	public class VigemXBox360Mappings : IVigemMappings
	{
		private VigemXBox360Mappings()
		{
		}

		public static VigemMappingCollection CreateMappingCollection()
		{
			Dictionary<XInputTypes, IVigemButtonMapping> buttonMappings = new()
			{
				{ XInputTypes.A, new VigemXbox360ButtonMapping(Xbox360Button.A) },
				{ XInputTypes.B, new VigemXbox360ButtonMapping(Xbox360Button.B) },
				{ XInputTypes.X, new VigemXbox360ButtonMapping(Xbox360Button.X) },
				{ XInputTypes.Y, new VigemXbox360ButtonMapping(Xbox360Button.Y) },
				{ XInputTypes.L1, new VigemXbox360ButtonMapping(Xbox360Button.LeftShoulder) },
				{ XInputTypes.R1, new VigemXbox360ButtonMapping(Xbox360Button.RightShoulder) },
				{ XInputTypes.Back, new VigemXbox360ButtonMapping(Xbox360Button.Back) },
				{ XInputTypes.Start, new VigemXbox360ButtonMapping(Xbox360Button.Start) },
				{ XInputTypes.Home, new VigemXbox360ButtonMapping(Xbox360Button.Guide) },
				{ XInputTypes.R3, new VigemXbox360ButtonMapping(Xbox360Button.RightThumb) },
				{ XInputTypes.L3, new VigemXbox360ButtonMapping(Xbox360Button.LeftThumb) },
				{ XInputTypes.UP, new VigemXbox360ButtonMapping(Xbox360Button.Up) },
				{ XInputTypes.DOWN, new VigemXbox360ButtonMapping(Xbox360Button.Down) },
				{ XInputTypes.LEFT, new VigemXbox360ButtonMapping(Xbox360Button.Left) },
				{ XInputTypes.RIGHT, new VigemXbox360ButtonMapping(Xbox360Button.Right) }
			};

			Dictionary<XInputTypes, IVigemAxisMapping> axisMappings = new()
			{
				{ XInputTypes.LX, new VigemXbox360AxisMapping(Xbox360Axis.LeftThumbX) },
				{ XInputTypes.LY, new VigemXbox360AxisMapping(Xbox360Axis.LeftThumbY) },
				{ XInputTypes.RX, new VigemXbox360AxisMapping(Xbox360Axis.RightThumbX) },
				{ XInputTypes.RY, new VigemXbox360AxisMapping(Xbox360Axis.RightThumbY) }
			};

			Dictionary<XInputTypes, IVigemSliderMapping> sliderMappings = new()
			{
				{ XInputTypes.L2, new VigemXbox360SliderMapping(Xbox360Slider.LeftTrigger) },
				{ XInputTypes.R2, new VigemXbox360SliderMapping(Xbox360Slider.RightTrigger) }
			};

			return new(buttonMappings, axisMappings, sliderMappings);
		}
	}

	/// <summary>
	/// Mapping value helper for Vigem buttons
	/// </summary>
	public class VigemXbox360ButtonMapping : IVigemButtonMapping
	{
		public Xbox360Button Type { get; set; }

		object IVigemButtonMapping.Type => Type;

		public VigemXbox360ButtonMapping(Xbox360Button button)
		{
			Type = button;
		}
	}

	/// <summary>
	/// Mapping value helper for Vigem axes
	/// </summary>
	public class VigemXbox360AxisMapping : IVigemAxisMapping
	{
		public Xbox360Axis Type { get; set; }

		object IVigemAxisMapping.Type => Type;

		public VigemXbox360AxisMapping(Xbox360Axis axis)
		{
			Type = axis;
		}
	}

	/// <summary>
	/// Mapping value helper for Vigem axes
	/// </summary>
	public class VigemXbox360SliderMapping : IVigemSliderMapping
	{
		public Xbox360Slider Type { get; set; }

		object IVigemSliderMapping.Type => Type;

		public VigemXbox360SliderMapping(Xbox360Slider slider)
		{
			Type = slider;
		}
	}
}
