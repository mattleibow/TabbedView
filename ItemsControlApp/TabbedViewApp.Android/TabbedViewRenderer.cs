using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Extended;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Android.Content;

[assembly: ExportRenderer(typeof(TabbedView), typeof(TabbedViewRenderer))]

namespace Xamarin.Forms.Extended
{
	public class TabbedViewRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<TabbedView, ViewPager>
	{
		private ViewPager viewPager;
		private TabLayout tabLayout;

		protected override void OnElementChanged(ElementChangedEventArgs<TabbedView> e)
		{
			base.OnElementChanged(e);

			var oldTabbedView = e.OldElement;
			var newTabbedView = e.NewElement;

			// detach from the old views
			if (oldTabbedView != null)
			{
				oldTabbedView.Tabs.CollectionChanged -= OnTabsChanged;

				if (tabLayout != null)
				{
					tabLayout.SetupWithViewPager(null);
					viewPager.Adapter = null;

					tabLayout = null;
					viewPager = null;
				}
			}

			// create the new native views
			if (tabLayout == null)
			{
				viewPager = new ViewPager(Context)
				{
				};

				tabLayout = new TabLayout(Context)
				{
					TabMode = TabLayout.ModeFixed,
					TabGravity = TabLayout.GravityFill
				};

				var lp = new ViewPager.LayoutParams()
				{
					Width = LayoutParams.MatchParent,
					Height = LayoutParams.WrapContent,
					Gravity = (int)GravityFlags.Top
				};

				viewPager.AddView(tabLayout, lp);
				viewPager.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

				SetNativeControl(viewPager);
			}

			// attach to the new views
			if (newTabbedView != null)
			{
				newTabbedView.Tabs.CollectionChanged += OnTabsChanged;

				viewPager.Adapter = new MyPagerAdapter(newTabbedView.Tabs);

				OnTabsChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		private void OnTabsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				foreach (Tab tab in Element.Tabs)
				{
					tab.PropertyChanged += OnTabPropertyChanged;
				}
			}
			else
			{
				foreach (Tab tab in e.OldItems)
				{
					tab.PropertyChanged -= OnTabPropertyChanged;
				}
				foreach (Tab tab in e.NewItems)
				{
					tab.PropertyChanged += OnTabPropertyChanged;
				}
			}

			tabLayout.SetupWithViewPager(viewPager);

			UpdateTabs();
		}

		private void UpdateTabs()
		{
			var tabs = Element.Tabs;
			for (int i = 0; i < tabs.Count; i++)
			{
				UpdateTab(tabLayout.GetTabAt(i), tabs[i]);
			}
		}

		private static void UpdateTab(TabLayout.Tab nativeTab, Tab tab)
		{
			nativeTab.SetText(tab.Title);
		}

		private void OnTabPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateTabs();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == TabbedView.CurrentTabProperty.PropertyName)
			{

			}
		}

		private class MyPagerAdapter : PagerAdapter
		{
			public MyPagerAdapter(ObservableCollection<Tab> tabs)
			{
				Tabs = tabs;
			}

			public ObservableCollection<Tab> Tabs { get; }

			public override int Count => Tabs.Count;

			public override bool IsViewFromObject(global::Android.Views.View view, Java.Lang.Object obj)
			{
				return view == obj;
			}

			public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
			{
				var tab = Tabs[position];
				var renderer = GetTabRenderer(tab);

				var tabContainer = new TabContainer(container.Context, tab, renderer);
				container.AddView(tabContainer);
				return tabContainer;

				//container.AddView(renderer.View);
				//return renderer.View;
			}

			private static IVisualElementRenderer GetTabRenderer(Tab tab)
			{
				var view = tab.Content;
				var renderer = Xamarin.Forms.Platform.Android.Platform.GetRenderer(view);
				if (renderer == null)
				{
					renderer = Xamarin.Forms.Platform.Android.Platform.CreateRenderer(view);
					Xamarin.Forms.Platform.Android.Platform.SetRenderer(view, renderer);
				}
				return renderer;
			}

			public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
			{
				container.RemoveView((global::Android.Views.View)obj);
			}
		}

		internal class TabContainer : ViewGroup
		{
			public TabContainer(Context context, Tab tab, IVisualElementRenderer renderer)
				: base(context)
			{
				Tab = tab;
				Renderer = renderer;

				Renderer.View.RemoveFromParent();
				AddView(Renderer.View);
			}

			public Tab Tab { get; }

			public IVisualElementRenderer Renderer { get; }

			protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
			{
				if (Renderer == null)
				{
					return;
				}

				var width = Context.FromPixels(right - left);
				var height = Context.FromPixels(bottom - top);
				Tab.Content.Layout(new Rectangle(0, 0, width, height));

				Renderer.UpdateLayout();
			}

			protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
			{
				if (Renderer == null)
				{
					SetMeasuredDimension(0, 0);
					return;
				}

				var width = (int)Context.FromPixels(MeasureSpec.GetSize(widthMeasureSpec));
				var height = (int)Context.FromPixels(MeasureSpec.GetSize(heightMeasureSpec));
				var request = Tab.Content.Measure(width, height, MeasureFlags.IncludeMargins);
				Tab.Content.Layout(new Rectangle(0, 0, width, height));
				var widthSpec = MeasureSpec.MakeMeasureSpec((int)Context.ToPixels(width), MeasureSpecMode.Exactly);
				var heightSpec = MeasureSpec.MakeMeasureSpec((int)Context.ToPixels(height), MeasureSpecMode.Exactly);
				Renderer.View.Measure(widthMeasureSpec, heightMeasureSpec);
				SetMeasuredDimension(widthSpec, heightSpec);
			}
		}
	}
}
