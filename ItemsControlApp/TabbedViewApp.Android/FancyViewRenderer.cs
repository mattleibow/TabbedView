using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Extended;
using Xamarin.Forms.Platform.Android;
using Android.Content;

[assembly: ExportRenderer(typeof(FancyView), typeof(FancyViewRenderer))]

namespace Xamarin.Forms.Extended
{
	public class FancyViewRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<FancyView, FrameLayout>
	{
		private FrameLayout nativeView;

		protected override void OnElementChanged(ElementChangedEventArgs<FancyView> e)
		{
			base.OnElementChanged(e);

			var oldFancy = e.OldElement;
			var newFancy = e.NewElement;

			// detach from the old views
			if (oldFancy != null)
			{
				if (nativeView != null)
				{
					nativeView.RemoveFromParent();
					nativeView = null;
				}
			}

			// create the new native views
			if (nativeView == null)
			{
				nativeView = new FrameLayout(Context)
				{
					LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
				};

				SetNativeControl(nativeView);
			}

			// attach to the new views
			if (newFancy != null)
			{
				UpdateViews();
			}
		}

		private void UpdateViews()
		{
			if (nativeView != null)
			{
				nativeView.RemoveAllViews();

				var renderer = GetRenderer(Element);
				var tabContainer = new FancyContainer(Context, Element, renderer);

				tabContainer.SetBackgroundColor(Color.Blue.MultiplyAlpha(0.2).ToAndroid());

				nativeView.AddView(tabContainer);
			}
		}

		private static IVisualElementRenderer GetRenderer(FancyView fancy)
		{
			var view = fancy.Content;
			var renderer = Xamarin.Forms.Platform.Android.Platform.GetRenderer(view);
			if (renderer == null)
			{
				renderer = Xamarin.Forms.Platform.Android.Platform.CreateRenderer(view);
				Xamarin.Forms.Platform.Android.Platform.SetRenderer(view, renderer);
			}
			return renderer;
		}

		internal class FancyContainer : ViewGroup
		{
			public FancyContainer(Context context, FancyView fancy, IVisualElementRenderer renderer)
				: base(context)
			{
				Fancy = fancy;
				Renderer = renderer;

				Renderer.View.RemoveFromParent();
				AddView(Renderer.View);
			}

			public FancyView Fancy { get; }

			public IVisualElementRenderer Renderer { get; }

			protected override void OnLayout(bool changed, int l, int t, int r, int b)
			{
				Renderer.UpdateLayout();
			}

			protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
			{
				Renderer.View.Measure(widthMeasureSpec, heightMeasureSpec);
				SetMeasuredDimension(Renderer.View.MeasuredWidth, Renderer.View.MeasuredHeight);
			}
		}
	}
}
