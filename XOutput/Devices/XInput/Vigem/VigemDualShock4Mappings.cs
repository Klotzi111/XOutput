using System.Collections.Generic;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace XOutput.Devices.XInput.Vigem
{
	public class VigemDualShock4Mappings : IVigemMappings
	{
		private VigemDualShock4Mappings()
		{
		}

		public static VigemMappingCollection CreateMappingCollection()
		{
			Dictionary<XInputTypes, IVigemButtonMapping> buttonMappings = new()
			{
				{ XInputTypes.A, new VigemDualShock4ButtonMapping(DualShock4Button.Cross) },
				{ XInputTypes.B, new VigemDualShock4ButtonMapping(DualShock4Button.Circle) },
				{ XInputTypes.X, new VigemDualShock4ButtonMapping(DualShock4Button.Square) },
				{ XInputTypes.Y, new VigemDualShock4ButtonMapping(DualShock4Button.Triangle) },
				{ XInputTypes.L1, new VigemDualShock4ButtonMapping(DualShock4Button.ShoulderLeft) },
				{ XInputTypes.R1, new VigemDualShock4ButtonMapping(DualShock4Button.ShoulderRight) },
				{ XInputTypes.Back, new VigemDualShock4ButtonMapping(DualShock4Button.Share) },
				{ XInputTypes.Start, new VigemDualShock4ButtonMapping(DualShock4Button.Options) },
				{ XInputTypes.Home, new VigemDualShock4ButtonMapping(DualShock4SpecialButton.Ps) },
				{ XInputTypes.R3, new VigemDualShock4ButtonMapping(DualShock4Button.ThumbRight) },
				{ XInputTypes.L3, new VigemDualShock4ButtonMapping(DualShock4Button.ThumbLeft) },
				{ XInputTypes.UP, new VigemDualShock4DPadDirectionButtonMapping(DualShock4DPadDirection.North, DPadDirection.Up) },
				{ XInputTypes.DOWN, new VigemDualShock4DPadDirectionButtonMapping(DualShock4DPadDirection.South, DPadDirection.Down) },
				{ XInputTypes.LEFT, new VigemDualShock4DPadDirectionButtonMapping(DualShock4DPadDirection.West, DPadDirection.Left) },
				{ XInputTypes.RIGHT, new VigemDualShock4DPadDirectionButtonMapping(DualShock4DPadDirection.East, DPadDirection.Right) }
			};

			Dictionary<XInputTypes, IVigemAxisMapping> axisMappings = new()
			{
				{ XInputTypes.LX, new VigemDualShock4AxisMapping(DualShock4Axis.LeftThumbX) },
				{ XInputTypes.LY, new VigemDualShock4AxisMapping(DualShock4Axis.LeftThumbY) },
				{ XInputTypes.RX, new VigemDualShock4AxisMapping(DualShock4Axis.RightThumbX) },
				{ XInputTypes.RY, new VigemDualShock4AxisMapping(DualShock4Axis.RightThumbY) }
			};

			Dictionary<XInputTypes, IVigemSliderMapping> sliderMappings = new()
			{
				{ XInputTypes.L2, new VigemDualShock4SliderMapping(DualShock4Slider.LeftTrigger) },
				{ XInputTypes.R2, new VigemDualShock4SliderMapping(DualShock4Slider.RightTrigger) }
			};

			return new(buttonMappings, axisMappings, sliderMappings);
		}

		public static DualShock4DPadDirection GetDualShock4DPadDirection(DPadDirection dPadDirection)
		{
			return dPadDirection switch
			{
				(DPadDirection.Up | DPadDirection.Left) => DualShock4DPadDirection.Northwest,
				DPadDirection.Left => DualShock4DPadDirection.West,
				(DPadDirection.Down | DPadDirection.Left) => DualShock4DPadDirection.Southwest,
				DPadDirection.Down => DualShock4DPadDirection.South,
				(DPadDirection.Down | DPadDirection.Right) => DualShock4DPadDirection.Southeast,
				DPadDirection.Right => DualShock4DPadDirection.East,
				(DPadDirection.Up | DPadDirection.Right) => DualShock4DPadDirection.Northeast,
				DPadDirection.Up => DualShock4DPadDirection.North,
				_ => DualShock4DPadDirection.None,
			};
		}
	}

	/// <summary>
	/// Mapping value helper for Vigem buttons
	/// </summary>
	public class VigemDualShock4DPadDirectionButtonMapping : IVigemButtonMapping
	{
		public DualShock4DPadDirection Type { get; set; }
		public DPadDirection DPadDirection { get; }

		object IVigemButtonMapping.Type => Type;

		public VigemDualShock4DPadDirectionButtonMapping(DualShock4DPadDirection dpadButton, DPadDirection dPadDirection)
		{
			Type = dpadButton;
			DPadDirection = dPadDirection;
		}
	}

	/// <summary>
	/// Mapping value helper for Vigem buttons
	/// </summary>
	public class VigemDualShock4ButtonMapping : IVigemButtonMapping
	{
		public DualShock4Button Type { get; set; }

		object IVigemButtonMapping.Type => Type;

		public VigemDualShock4ButtonMapping(DualShock4Button button)
		{
			Type = button;
		}
	}

	/// <summary>
	/// Mapping value helper for Vigem axes
	/// </summary>
	public class VigemDualShock4AxisMapping : IVigemAxisMapping
	{
		public DualShock4Axis Type { get; set; }

		object IVigemAxisMapping.Type => Type;

		public VigemDualShock4AxisMapping(DualShock4Axis axis)
		{
			Type = axis;
		}
	}

	/// <summary>
	/// Mapping value helper for Vigem axes
	/// </summary>
	public class VigemDualShock4SliderMapping : IVigemSliderMapping
	{
		public DualShock4Slider Type { get; set; }

		object IVigemSliderMapping.Type => Type;

		public VigemDualShock4SliderMapping(DualShock4Slider slider)
		{
			Type = slider;
		}
	}
}
