namespace Xamarin.Forms.Extended
{
	[ContentProperty(nameof(Content))]
	public class FancyView : View
	{
		public static readonly BindableProperty ContentProperty =
			BindableProperty.Create(nameof(Content), typeof(View), typeof(FancyView), default(View));

		public View Content
		{
			get { return (View)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
	}
}
