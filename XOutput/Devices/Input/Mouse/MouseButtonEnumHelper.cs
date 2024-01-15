using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace XOutput.Devices.Input.Mouse
{
	public static class MouseButtonEnumHelper
	{
		public static MouseButtons ToFormsMouseButtons(this MouseButton button)
		{
			switch (button)
			{
				case MouseButton.Left:
					return MouseButtons.Left;
				case MouseButton.Right:
					return MouseButtons.Right;
				case MouseButton.Middle:
					return MouseButtons.Middle;
				case MouseButton.XButton1:
					return MouseButtons.XButton1;
				case MouseButton.XButton2:
					return MouseButtons.XButton2;
				default:
					throw new ArgumentException("Invalid mouse button", nameof(button));
			}
		}

		public static MouseButton ToWPFMouseButton(this MouseButtons button)
		{
			switch (button)
			{
				case MouseButtons.Left:
					return MouseButton.Left;
				case MouseButtons.Right:
					return MouseButton.Right;
				case MouseButtons.Middle:
					return MouseButton.Middle;
				case MouseButtons.XButton1:
					return MouseButton.XButton1;
				case MouseButtons.XButton2:
					return MouseButton.XButton2;
				default:
					throw new ArgumentException("Invalid mouse button", nameof(button));
			}
		}
	}
}
