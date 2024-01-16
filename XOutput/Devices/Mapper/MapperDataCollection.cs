using System;
using System.Collections.Generic;
using XOutput.Devices.XInput;

namespace XOutput.Devices.Mapper
{
	public class MapperDataCollection
	{
		public List<MapperData> Mappers { get; set; }

		public double CenterPoint
		{
			get => centerPoint;
			set
			{
				if (value != centerPoint)
				{
					centerPoint = value;
					lowRange = centerPoint;
					highRange = 1 - centerPoint;
				}
			}
		}

		private double centerPoint = -1;
		private double lowRange;
		private double highRange;

		public MapperDataCollection() : this(new List<MapperData>(), 0)
		{

		}
		public MapperDataCollection(params MapperData[] data) : this(new List<MapperData>(data), 0)
		{

		}

		public MapperDataCollection(List<MapperData> mappers) : this(mappers, 0)
		{

		}

		public MapperDataCollection(List<MapperData> mappers, double centerPoint)
		{
			Mappers = mappers;
			CenterPoint = centerPoint;
		}

		/// <summary>
		/// Gets the value based on center point.
		/// </summary>
		/// <param name="value">Measured data to convert</param>
		/// <returns>Mapped value</returns>
		public double GetValue(XInputTypes type)
		{
			// has less object allocations than LINQ
			var val = centerPoint;
			foreach (var mapper in Mappers)
			{
				if (mapper.Source != null)
				{
					var v = mapper.GetValue(mapper.Source.Get(type));
					val += DiffFromCenter(v);
				}
			}
			return val;
		}

		private double DiffFromCenter(double value)
		{
			if (Math.Abs(value - centerPoint) < 0.0001)
			{
				return 0;
			}
			if (value < centerPoint)
			{
				return (value - centerPoint) * lowRange;
			}
			return (value - centerPoint) * highRange;
		}
	}
}
