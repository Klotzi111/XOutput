using System;
using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json;

namespace XOutput.Devices.Mapper
{
	/// <summary>
	/// Contains mapping data to Xinput conversion.
	/// </summary>
	public class MapperData
	{
		/// <summary>
		/// From data device
		/// </summary>
		public string? InputDevice { get; set; }
		/// <summary>
		/// From data type
		/// </summary>
		public string? InputType { get; set; }
		/// <summary>
		/// Data source
		/// </summary>
		[JsonIgnore]
		public InputSource Source
		{
			get => source;
			set
			{
				var newValue = value ?? DisabledInputSource.Instance;
				if (newValue != source)
				{
					source = newValue;
					InputType = source.Offset.ToString();
					InputDevice = source.InputDevice?.UniqueId;
				}
			}
		}
		/// <summary>
		/// Minimum value
		/// </summary>
		public double MinValue { get; set; }
		/// <summary>
		/// Maximum value
		/// </summary>
		public double MaxValue { get; set; }
		/// <summary>
		/// Deadzone
		/// </summary>
		public double Deadzone { get; set; }
		/// <summary>
		/// Anti-Deadzone
		/// </summary>
		public double AntiDeadzone { get; set; }
		/// <summary>
		/// From data type
		/// </summary>
		public string? ValueFunction { get; set; }

		private readonly ExpressionEvaluator evaluator = new();
		private InputSource source;

		public MapperData()
		{
			InputType = "0";
			source = DisabledInputSource.Instance;
			MinValue = 0;
			MaxValue = 0;
			Deadzone = 0;
			AntiDeadzone = 0;
		}

		/// <summary>
		/// Gets the value based on minimum and maximum values.
		/// </summary>
		/// <param name="value">Measured data to convert</param>
		/// <returns>Mapped value</returns>
		public double GetValue(double value)
		{
			double range = MaxValue - MinValue;
			double mappedValue;
			if (Math.Abs(range) < 0.0001)
			{
				mappedValue = MinValue;
			}
			else
			{
				var readValue = value;
				var deadzonedValue = readValue;

				if (Math.Abs(readValue - 0.5) < Deadzone)
				{
					deadzonedValue = 0.5;
				}

				if (ValueFunction == null)
				{
					if (AntiDeadzone != 0)
					{
						var sign = deadzonedValue < 0.5 ? -1 : 1;
						readValue = (((Math.Abs((deadzonedValue - 0.5) * 2) * (1 - AntiDeadzone)) + AntiDeadzone) * sign / 2) + 0.5;
					}
					mappedValue = (readValue - MinValue) / range;
				}
				else
				{
					try
					{
						// we set the variables every time because we do not know if the script changed them
						evaluator.Variables["MinValue"] = MinValue;
						evaluator.Variables["MaxValue"] = MaxValue;
						evaluator.Variables["Deadzone"] = Deadzone;
						evaluator.Variables["AntiDeadzone"] = AntiDeadzone;
						evaluator.Variables["Range"] = range;
						evaluator.Variables["RawValue"] = value;
						evaluator.Variables["Value"] = deadzonedValue;
						evaluator.Variables["x"] = deadzonedValue;

						mappedValue = evaluator.ScriptEvaluate<double>(ValueFunction);
					}
					catch
					{
						// in case of error just always be zero
						// this way one notices that something is very wrong
						mappedValue = 0;
					}
				}

				// clamp value to 0-1 range
				mappedValue = Math.Min(Math.Max(mappedValue, 0), 1);
			}
			return mappedValue;
		}
		public void SetSourceWithoutSaving(InputSource? value)
		{
			var newValue = value ?? DisabledInputSource.Instance;
			if (newValue != source)
			{
				source = newValue;
			}
		}
	}
}
