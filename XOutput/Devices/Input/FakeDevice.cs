using System;
using System.Collections.Generic;

namespace XOutput.Devices.Input
{
	public sealed class FakeDevice : IInputDevice
	{
		public string DisplayName => throw new NotImplementedException();

		public string UniqueId => throw new NotImplementedException();

		public bool Connected => throw new NotImplementedException();

		public string? HardwareID => throw new NotImplementedException();

		public int ForceFeedbackCount => throw new NotImplementedException();

		public InputConfig InputConfiguration => throw new NotImplementedException();

		public IEnumerable<DPadDirection> DPads => throw new NotImplementedException();

		public IEnumerable<InputSource> Sources => throw new NotImplementedException();

		event DeviceDisconnectedHandler? IInputDevice.Disconnected
		{
			add
			{
				throw new NotImplementedException();
			}

			remove
			{
				throw new NotImplementedException();
			}
		}

		event DeviceInputChangedHandler? IDevice.InputChanged
		{
			add
			{
				throw new NotImplementedException();
			}

			remove
			{
				throw new NotImplementedException();
			}
		}

		public void Dispose() => throw new NotImplementedException();
		public double Get(InputSource source) => throw new NotImplementedException();
		public bool RefreshInput(bool force = false) => throw new NotImplementedException();
		public void SetForceFeedback(double big, double small) => throw new NotImplementedException();
	}
}
