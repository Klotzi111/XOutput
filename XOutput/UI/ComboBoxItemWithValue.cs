using System.Windows.Controls;

namespace XOutput.UI
{
	public class ComboBoxItemWithValue<T> : ComboBoxItem
	{
		public readonly T value;

		public ComboBoxItemWithValue(T value)
		{
			this.value = value;
		}
	}
}
