using System.Collections.Generic;
using Microsoft.Win32;

namespace XOutput.Tools
{
	public class HidGuardianManager
	{
		private static readonly string parameters = "SYSTEM\\CurrentControlSet\\Services\\HidGuardian\\Parameters";
		private static readonly string whiteList = parameters + "\\Whitelist";
		private static readonly string affectedDevices = "AffectedDevices";

		public void ResetPid(int pid)
		{
			if (RegistryModifier.KeyExists(Registry.LocalMachine, whiteList))
			{
				RegistryModifier.DeleteTree(Registry.LocalMachine, whiteList);
			}
			RegistryModifier.CreateKey(Registry.LocalMachine, whiteList + "\\" + pid);
		}

		public List<string> GetDevices()
		{
			object value = RegistryModifier.GetValue(Registry.LocalMachine, parameters, affectedDevices);
			if (value is string[])
			{
				return new List<string>(value as string[]);
			}
			return new List<string>();
		}

		public void AddAffectedDevice(string device)
		{
			if (device == null)
			{
				return;
			}
			var devices = GetDevices();
			devices.Add(device);
			RegistryModifier.SetValue(Registry.LocalMachine, parameters, affectedDevices, devices.ToArray());
		}

		public bool RemoveAffectedDevice(string device)
		{
			if (device == null)
			{
				return false;
			}
			var devices = GetDevices();
			bool removed = devices.Remove(device);
			if (removed)
			{
				RegistryModifier.SetValue(Registry.LocalMachine, parameters, affectedDevices, devices.ToArray());
			}
			return removed;
		}

		public bool IsAffected(string device)
		{
			var devices = GetDevices();
			return devices.Contains(device);
		}
	}
}
