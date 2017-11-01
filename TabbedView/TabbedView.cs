using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Extended
{
	[ContentProperty(nameof(Tabs))]
	public class TabbedView : View
	{
		public static readonly BindableProperty CurrentTabProperty =
			BindableProperty.Create(nameof(CurrentTab), typeof(Tab), typeof(TabbedView), default(Tab));

		public Tab CurrentTab
		{
			get { return (Tab)GetValue(CurrentTabProperty); }
			set { SetValue(CurrentTabProperty, value); }
		}

		public ObservableCollection<Tab> Tabs { get; } = new ObservableCollection<Tab>();
	}
}
