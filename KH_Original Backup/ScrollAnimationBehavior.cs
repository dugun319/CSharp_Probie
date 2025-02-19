using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

using ScrollAnimateBehavior.Helpers;

using Softpower.SmartMaker.TopApp;

//https://stackoverflow.com/questions/20731402/animated-smooth-scrolling-on-scrollviewer
namespace ScrollAnimateBehavior.AttachedBehaviors
{
	public static class ScrollAnimationBehavior
	{
		private static DispatcherTimer timer = null;

		private static bool m_bStartAnimation = false;

		private static double m_dMouseWheelOffset = 0;
		private static ScrollViewer m_ScrollViewer = null;


		#region Private ScrollViewer for ListBox

		private static ScrollViewer _listBoxScroller = new ScrollViewer ();
		private static ScrollViewer _ItemsControlScroller = new ScrollViewer ();

		#endregion

		#region VerticalOffset Property

		public static DependencyProperty VerticalOffsetProperty =
			DependencyProperty.RegisterAttached ("VerticalOffset",
												typeof (double),
												typeof (ScrollAnimationBehavior),
												new UIPropertyMetadata (0.0, OnVerticalOffsetChanged));

		public static void SetVerticalOffset (FrameworkElement target, double value)
		{
			if (null != target)
			{
				target.SetValue (VerticalOffsetProperty, value);
			}
		}

		public static double GetVerticalOffset (FrameworkElement target)
		{
			return (double)target.GetValue (VerticalOffsetProperty);
		}

		public static DependencyProperty HorizontalOffsetProperty =
			DependencyProperty.RegisterAttached ("HorizontalOffset",
										typeof (double),
										typeof (ScrollAnimationBehavior),
										new UIPropertyMetadata (0.0, OnHorizontalOffsetChanged));

		public static void SetHorizontalOffset (FrameworkElement target, double value)
		{
			if (null != target)
			{
				target.SetValue (HorizontalOffsetProperty, value);
			}
		}

		public static double GetHorizontalOffset (FrameworkElement target)
		{
			return (double)target.GetValue (HorizontalOffsetProperty);
		}

		#endregion

		#region TimeDuration Property

		public static DependencyProperty TimeDurationProperty =
			DependencyProperty.RegisterAttached ("TimeDuration",
												typeof (TimeSpan),
												typeof (ScrollAnimationBehavior),
												new PropertyMetadata (new TimeSpan (0, 0, 0, 0, 0)));

		public static void SetTimeDuration (FrameworkElement target, TimeSpan value)
		{
			if (null != target)
			{
				target.SetValue (TimeDurationProperty, value);
			}
		}

		public static TimeSpan GetTimeDuration (FrameworkElement target)
		{
			return (TimeSpan)target.GetValue (TimeDurationProperty);
		}

		#endregion

		#region PointsToScroll Property

		public static DependencyProperty PointsToScrollProperty =
			DependencyProperty.RegisterAttached ("PointsToScroll",
												typeof (double),
												typeof (ScrollAnimationBehavior),
												new PropertyMetadata (0.0));

		public static void SetPointsToScroll (FrameworkElement target, double value)
		{
			if (null != target)
			{
				target.SetValue (PointsToScrollProperty, value);
			}
		}

		public static double GetPointsToScroll (FrameworkElement target)
		{
			return (double)target.GetValue (PointsToScrollProperty);
		}

		#endregion

		#region ScrollStepSize Property

		public static DependencyProperty ScrollStepSizeProperty =
			DependencyProperty.RegisterAttached ("ScrollStepSize",
												typeof (int),
												typeof (ScrollAnimationBehavior),
												new PropertyMetadata (0));

		public static void SetScrollStepSize (FrameworkElement target, int value)
		{
			if (null != target)
			{
				target.SetValue (ScrollStepSizeProperty, value);
			}
		}

		public static int GetScrollStepSize (FrameworkElement target)
		{
			return (int)target.GetValue (ScrollStepSizeProperty);
		}

		#endregion

		#region | AccelerationRatioProperty |

		public static DependencyProperty AccelerationRatioProperty =
			DependencyProperty.RegisterAttached ("AccelerationRatio",
												typeof (double),
												typeof (ScrollAnimationBehavior),
												new PropertyMetadata (0.0));

		public static void SetAccelerationRatio (FrameworkElement target, double value)
		{
			if (null != target)
			{
				target.SetValue (AccelerationRatioProperty, value);
			}
		}

		public static double GetAccelerationRatio (FrameworkElement target)
		{
			return (double)target.GetValue (AccelerationRatioProperty);
		}

		#endregion

		#region OnVerticalOffset Changed

		private static void OnVerticalOffsetChanged (DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			ScrollViewer scrollViewer = target as ScrollViewer;

			if (scrollViewer != null)
			{
				scrollViewer.ScrollToVerticalOffset ((double)e.NewValue);
			}
		}

		private static void OnHorizontalOffsetChanged (DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			ScrollViewer scrollViewer = target as ScrollViewer;

			if (scrollViewer != null)
			{
				scrollViewer.ScrollToHorizontalOffset ((double)e.NewValue);
			}
		}

		#endregion

		#region IsEnabled Property

		public static DependencyProperty IsEnabledProperty =
												DependencyProperty.RegisterAttached ("IsEnabled",
												typeof (bool),
												typeof (ScrollAnimationBehavior),
												new UIPropertyMetadata (false, OnIsEnabledChanged));

		public static void SetIsEnabled (FrameworkElement target, bool value)
		{
			if (null != target)
			{
				target.SetValue (IsEnabledProperty, value);
			}
		}

		public static bool GetIsEnabled (FrameworkElement target)
		{
			return (bool)target.GetValue (IsEnabledProperty);
		}

		#endregion

		#region OnIsEnabledChanged Changed

		private static void OnIsEnabledChanged (DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var target = sender;

			if (target != null && target is ScrollViewer)
			{
				ScrollViewer scroller = target as ScrollViewer;
				scroller.Loaded += new RoutedEventHandler (scrollerLoaded);
			}

			if (target != null && target is ListBox)
			{
				ListBox listbox = target as ListBox;
				listbox.Loaded += new RoutedEventHandler (listboxLoaded);
			}
		}

		#endregion

		#region AnimateScroll Helper

		public static void AnimateScroll (ScrollViewer scrollViewer, double ToValue, bool bMultiExcute = true)
		{
			if (false != m_bStartAnimation)
				return;

			m_bStartAnimation = bMultiExcute;

			if (scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Visible ||
				scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden)
			{
				DoubleAnimation verticalAnimation = new DoubleAnimation ();

				verticalAnimation.From = scrollViewer.VerticalOffset;
				verticalAnimation.To = ToValue;
				verticalAnimation.Duration = new Duration (GetTimeDuration (scrollViewer));
				verticalAnimation.AccelerationRatio = GetAccelerationRatio (scrollViewer);

				Storyboard storyboard = new Storyboard ();
				storyboard.Completed += (s, e) =>
				{
					m_bStartAnimation = false;
					timer?.Stop ();

					//
					/*
					Task task = Task.Run (() =>
					{
						scrollViewer.Dispatcher.Invoke (new Action (delegate
						{
							int stepsize = GetScrollStepSize (scrollViewer);
							if (0 < stepsize)
							{
								double vertOffset = GetVerticalOffset (scrollViewer);
								double offset = (int)Math.Round (vertOffset / stepsize) * stepsize;
								SetVerticalOffset (scrollViewer, offset);
							}
						}));
					});*/
					//
				};

				storyboard.Children.Add (verticalAnimation);
				Storyboard.SetTarget (verticalAnimation, scrollViewer);
				Storyboard.SetTargetProperty (verticalAnimation, new PropertyPath (ScrollAnimationBehavior.VerticalOffsetProperty));
				storyboard.Begin ();
			}
			else
			{
				DoubleAnimation horizontalAnimation = new DoubleAnimation ();

				horizontalAnimation.From = scrollViewer.HorizontalOffset;
				horizontalAnimation.To = ToValue;
				horizontalAnimation.Duration = new Duration (GetTimeDuration (scrollViewer));
				horizontalAnimation.AccelerationRatio = GetAccelerationRatio (scrollViewer);

				Storyboard storyboard = new Storyboard ();
				storyboard.Completed += storyboard_Completed;

				storyboard.Children.Add (horizontalAnimation);
				Storyboard.SetTarget (horizontalAnimation, scrollViewer);
				Storyboard.SetTargetProperty (horizontalAnimation, new PropertyPath (ScrollAnimationBehavior.HorizontalOffsetProperty));
				storyboard.Begin ();
			}
		}

		static void storyboard_Completed (object sender, EventArgs e)
		{
			m_bStartAnimation = false;
			timer?.Stop ();
		}

		#endregion

		#region NormalizeScrollPos Helper

		private static double NormalizeScrollPos (ScrollViewer scroll, double scrollChange, Orientation o)
		{
			double returnValue = scrollChange;

			if (scrollChange < 0)
			{
				returnValue = 0;
			}

			if (o == Orientation.Vertical && scrollChange > scroll.ScrollableHeight)
			{
				returnValue = scroll.ScrollableHeight;
			}
			else if (o == Orientation.Horizontal && scrollChange > scroll.ScrollableWidth)
			{
				returnValue = scroll.ScrollableWidth;
			}

			return returnValue;
		}

		#endregion

		#region UpdateScrollPosition Helper

		private static void UpdateScrollPosition (object sender)
		{
			ListBox listbox = sender as ListBox;

			if (listbox != null)
			{
				double scrollTo = 0;

				for (int i = 0; i < (listbox.SelectedIndex); i++)
				{
					ListBoxItem tempItem = listbox.ItemContainerGenerator.ContainerFromItem (listbox.Items[i]) as ListBoxItem;

					if (tempItem != null)
					{
						scrollTo += tempItem.ActualHeight;
					}
				}

				AnimateScroll (_listBoxScroller, scrollTo);
			}
		}

		#endregion

		#region SetEventHandlersForScrollViewer Helper

		public static void SetEventHandlersForScrollViewer (ScrollViewer scroller)
		{
			scroller.PreviewMouseWheel += new MouseWheelEventHandler (ScrollViewerPreviewMouseWheel);
			scroller.PreviewKeyDown += new KeyEventHandler (ScrollViewerPreviewKeyDown);
		}

		#endregion

		#region scrollerLoaded Event Handler

		private static void scrollerLoaded (object sender, RoutedEventArgs e)
		{
			ScrollViewer scroller = sender as ScrollViewer;
			if (null != scroller)
			{
				SetEventHandlersForScrollViewer (scroller);
			}
		}

		#endregion

		#region listboxLoaded Event Handler

		private static void listboxLoaded (object sender, RoutedEventArgs e)
		{
			ListBox listbox = sender as ListBox;

			_listBoxScroller = FindVisualChildHelper.GetFirstChildOfType<ScrollViewer> (listbox);
			SetEventHandlersForScrollViewer (_listBoxScroller);

			SetTimeDuration (_listBoxScroller, new TimeSpan (0, 0, 0, 0, 200));
			SetPointsToScroll (_listBoxScroller, 16.0);

			listbox.SelectionChanged += new SelectionChangedEventHandler (ListBoxSelectionChanged);
			listbox.Loaded += new RoutedEventHandler (ListBoxLoaded);
			listbox.LayoutUpdated += new EventHandler (ListBoxLayoutUpdated);
		}

		#endregion

		#region ScrollViewerPreviewMouseWheel Event Handler

		public static void ScrollViewerPreviewMouseWheel (object sender, MouseWheelEventArgs e)
		{
			if (null == timer)
			{
				timer = new DispatcherTimer ();
				timer.Interval = TimeSpan.FromMilliseconds (30);
				timer.Tick += timer_Tick;
			}

			timer.Stop ();

			double mouseWheelChange = (double)e.Delta;
			ScrollViewer scroller = (ScrollViewer)sender;

			m_dMouseWheelOffset += mouseWheelChange;
			m_ScrollViewer = scroller;

			timer.Start ();
			e.Handled = true;
		}

		static void timer_Tick (object sender, EventArgs e)
		{
			timer.Stop ();

			ScrollViewer scrollViewer = m_ScrollViewer;
			double newVOffset = scrollViewer.VerticalOffset;

			if (scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Visible ||
				scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden)
				newVOffset = scrollViewer.VerticalOffset;
			else
				newVOffset = scrollViewer.HorizontalOffset;

			double dPointScroll = GetPointsToScroll (scrollViewer);
			double mouseWheelChange = m_dMouseWheelOffset;
			m_dMouseWheelOffset = 0;

			if (0 < dPointScroll)
			{
				double dTemp = 120 / dPointScroll; //마우스의 휠 1칸의 값이 120으로 들어오기 때문에 dPiontScroll로 나누어 휠1칸당 dPiontScroll만큼 스크롤을 이동시키기 위해서 추가

				mouseWheelChange = mouseWheelChange / dTemp;
				mouseWheelChange -= (mouseWheelChange % dPointScroll);
			}

			newVOffset -= mouseWheelChange;

			if (scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Visible ||
				scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden)
			{
				if (newVOffset < 0)
				{
					AnimateScroll (scrollViewer, 0);
				}
				else if (newVOffset > scrollViewer.ScrollableHeight)
				{
					AnimateScroll (scrollViewer, scrollViewer.ScrollableHeight);
				}
				else
				{
					AnimateScroll (scrollViewer, newVOffset);
				}
			}
			else
			{
				if (newVOffset < 0)
				{
					AnimateScroll (scrollViewer, 0);
				}
				else if (newVOffset > scrollViewer.ScrollableWidth)
				{
					AnimateScroll (scrollViewer, scrollViewer.ScrollableWidth);
				}
				else
				{
					AnimateScroll (scrollViewer, newVOffset);
				}
			}
		}

		#endregion

		#region ScrollViewerPreviewKeyDown Handler

		private static void ScrollViewerPreviewKeyDown (object sender, KeyEventArgs e)
		{
			ScrollViewer scroller = (ScrollViewer)sender;

			Key keyPressed = e.Key;
			double newVerticalPos = GetVerticalOffset (scroller);
			bool isKeyHandled = false;

			if (keyPressed == Key.Down)
			{
				newVerticalPos = NormalizeScrollPos (scroller, (newVerticalPos + GetPointsToScroll (scroller)), Orientation.Vertical);
				isKeyHandled = true;
			}
			else if (keyPressed == Key.PageDown)
			{
				newVerticalPos = NormalizeScrollPos (scroller, (newVerticalPos + scroller.ViewportHeight), Orientation.Vertical);
				isKeyHandled = true;
			}
			else if (keyPressed == Key.Up)
			{
				newVerticalPos = NormalizeScrollPos (scroller, (newVerticalPos - GetPointsToScroll (scroller)), Orientation.Vertical);
				isKeyHandled = true;
			}
			else if (keyPressed == Key.PageUp)
			{
				newVerticalPos = NormalizeScrollPos (scroller, (newVerticalPos - scroller.ViewportHeight), Orientation.Vertical);
				isKeyHandled = true;
			}

			if (!Kiss.DoubleEqual (newVerticalPos, GetVerticalOffset (scroller)))
			{
				//
				int nRatio = (int)(newVerticalPos / GetPointsToScroll (scroller));
				newVerticalPos = nRatio * GetPointsToScroll (scroller);
				//

				AnimateScroll (scroller, newVerticalPos);
			}

			e.Handled = isKeyHandled;
		}

		#endregion

		#region ListBox Event Handlers

		private static void ListBoxLayoutUpdated (object sender, EventArgs e)
		{
			UpdateScrollPosition (sender);
		}

		private static void ListBoxLoaded (object sender, RoutedEventArgs e)
		{
			UpdateScrollPosition (sender);
		}

		private static void ListBoxSelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			UpdateScrollPosition (sender);
		}

		#endregion

		#region ItemsControl Event Handlers

		private static void ItemsControlLoaded (object sender)
		{
			ItemsControl itemsControl = sender as ItemsControl;

			_ItemsControlScroller = itemsControl.Template.FindName ("ScrollViewer", itemsControl) as ScrollViewer;

			if (_ItemsControlScroller == null) return;

			SetEventHandlersForScrollViewer (_ItemsControlScroller);

			SetTimeDuration (_ItemsControlScroller, GetTimeDuration (itemsControl));
			SetPointsToScroll (_ItemsControlScroller, GetPointsToScroll (itemsControl));

			itemsControl.Loaded += new RoutedEventHandler (ItemsControl_Loaded);
			itemsControl.LayoutUpdated += new EventHandler (ItemsControlLayoutUpdated);
		}

		private static void ItemsControl_Loaded (object sender, RoutedEventArgs e)
		{
			IC_UpdateScrollPosition (sender);
		}

		private static void ItemsControlLayoutUpdated (object sender, EventArgs e)
		{
			IC_UpdateScrollPosition (sender);
		}
		private static void IC_UpdateScrollPosition (object sender)
		{
			ItemsControl itemsControl = sender as ItemsControl;

			if (itemsControl != null)
			{
				AnimateScroll (_ItemsControlScroller, _ItemsControlScroller.ContentVerticalOffset);
			}
		}

		#endregion
	}
}