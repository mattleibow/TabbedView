namespace Xamarin.Forms.Extended
{
	[ContentProperty(nameof(Content))]
	public class Tab : Element
	{
		public static readonly BindableProperty ContentProperty =
			BindableProperty.Create(nameof(Content), typeof(View), typeof(Tab), default(View));

		public static readonly BindableProperty TitleProperty =
			BindableProperty.Create(nameof(Title), typeof(string), typeof(Tab), default(string));

		public View Content
		{
			get { return (View)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
	}
}
