using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using ScrollAnimateBehavior.AttachedBehaviors;

using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom.Components.RecyclerBrowse.ViewModel;
using Softpower.SmartMaker.TopControl.Components.MVVM;

namespace Softpower.SmartMaker.TopAtom.Components.RecyclerBrowse.View
{
	/// <summary>
	/// BrowseContentsView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class BrowseContentsView : UserControl
	{
		#region | xaml |

		private readonly string _OriginXaml =
					@"<DataTemplate >
                        <DataTemplate.Resources>
                            <Converter:BoolToVisibilityConverter x:Key=""BoolToVisibilityConverter""/>
                        </DataTemplate.Resources>
                        <Border x:Name=""RootBorder""   Width=""{Binding RowWidth}"" HorizontalAlignment=""Stretch"">
                            <Grid x:Uid=""RootGrid"" x:Name=""RootGrid"" Background=""{Binding CurrentRowBackground}"">

                                <Grid.RowDefinitions>
                                    <RowDefinition x:Uid=""RowDefinition_1"" Height=""*""></RowDefinition>
                                    <RowDefinition x:Uid=""RowDefinition_2"" Height=""1""></RowDefinition>
                                </Grid.RowDefinitions>
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition x:Uid=""ColumnDefinition_1"" Width=""Auto""/>
                                    <ColumnDefinition x:Uid=""ColumnDefinition_2""/>
                                </Grid.ColumnDefinitions>
                                <Border x:Uid=""PanelBorder"" x:Name=""PanelBorder"" Grid.ColumnSpan=""2"" Grid.RowSpan=""2"" 
		                                CornerRadius=""0,0,0,0"" Background=""{Binding PanelBorderBackground}""/>

                                <Grid x:Uid=""RowCheckBox"" x:Name=""RowCheckBox"" Grid.Column=""0"" Grid.Row=""0"" >
                                    <Line 
                                        x:Uid=""RowVerticalLine"" x:Name=""RowVerticalLine"" Stretch=""Fill"" X1=""0"" Y1=""0"" X2=""0"" Y2=""1"" 
		                                Stroke=""Silver"" StrokeDashArray=""1,2"" StrokeThickness=""1"" Grid.Column=""1"" HorizontalAlignment=""Right""
		                                Visibility=""{Binding ShowVerticalSeparator, Converter={StaticResource BoolToVisibilityConverter}, ConverterParameter=hidden}""></Line>
                                    <TopAtom:BrowseRowCheckBox 
                                        x:Name=""m_BrowseCheckBox"" Width=""{Binding BrowseCheckBoxWidth}"" Height=""{Binding BrowseCheckBoxWidth}""
                                        IsChecked=""{Binding IsChecked, Mode=TwoWay}"" IsCheckBoxVisible=""{Binding IsCheckBoxVisible, Mode=TwoWay}""
                                        Visibility=""{Binding IsCheckBoxVisible, Converter={StaticResource BoolToVisibilityConverter}, Mode=TwoWay}""/>
                                </Grid>

                                <ItemsControl x:Name=""CellItemsControl"" Grid.Column=""1"" Grid.Row=""0"" ItemsSource=""{Binding CellDataList}"">
                                    <ItemsControl.ItemsPanel>
                                        <ItemsPanelTemplate>
                                            <WrapPanel Orientation=""Horizontal""/>
                                        </ItemsPanelTemplate>
                                    </ItemsControl.ItemsPanel>

                                    <ItemsControl.ItemTemplate>
                                        <DataTemplate >
                                            <Border x:Name=""CellRootborder"" Width=""{Binding CellWidth}"" Height=""{Binding CellHeight}"" Margin=""{Binding CellMargin}""
                                                    Visibility=""{Binding CellVisibility}"">
                                                <Grid>
                                                    <Grid.ColumnDefinitions>
                                                        <ColumnDefinition x:Uid=""ColumnDefinition_1"" Width=""*""></ColumnDefinition>
                                                        <ColumnDefinition x:Uid=""ColumnDefinition_2"" Width=""1""></ColumnDefinition>
                                                    </Grid.ColumnDefinitions>

                                                    <Grid x:Name=""CellRootGrid"" Margin=""3"" Grid.Column=""0"">
                                                        <Grid.ColumnDefinitions>
                                                            <ColumnDefinition x:Name=""IndentColumn"" Width=""{Binding IndentColumnWidth}""/>
                                                            <ColumnDefinition Width=""1*""/>
                                                        </Grid.ColumnDefinitions>
                                                        <Border x:Name=""ReplyIndentBorder"" Grid.Column=""0"" Width=""10"" Height=""10"" BorderThickness=""2,0,0,2"" 
                                                                BorderBrush=""LightGray"" HorizontalAlignment=""Right"" Visibility=""{Binding ReplyIndentBorderVisibility}"" />
                                                        <TextBlock x:Uid=""RowDataTextBlock"" x:Name=""RowDataTextBlock"" HorizontalAlignment=""{Binding TextAlignment}"" 
                                                                   TextWrapping=""NoWrap"" Text=""{Binding RowDataText}"" Foreground=""{Binding RowDataTextColor}"" 
                                                                   Background=""{Binding RowDataBackColor}""  Visibility=""{Binding RowDataTextBlockVisibility}""
                                                                   VerticalAlignment=""Center"" Padding=""5"" Grid.Column=""1"" TextTrimming=""CharacterEllipsis""/>
                                                    </Grid>
                                                    <Line x:Uid=""RowVerticalLine"" x:Name=""RowVerticalLine"" 
                                                          Visibility=""{Binding ShowVerticalSeparator, Converter={StaticResource BoolToVisibilityConverter}, ConverterParameter=hidden}""
                                                          Stretch=""Fill"" X1=""0"" Y1=""0"" X2=""0"" Y2=""1"" Stroke=""Silver""
                                                          StrokeDashArray=""1,2"" StrokeThickness=""1"" Grid.Column=""1""></Line>
                                                    <Line x:Name=""RowLine"" Grid.ColumnSpan=""2"" Stroke=""Silver"" X1=""0"" Y1=""0"" X2=""400"" Y2=""0"" Stretch=""Fill"" 
                                                          StrokeThickness=""1"" StrokeDashArray=""1,2"" VerticalAlignment=""Bottom""
                                                          Visibility=""{Binding ShowRowLineSeparator, Converter={StaticResource BoolToVisibilityConverter}, ConverterParameter=hidden}""/>
                                                </Grid>
                                            </Border>
                                        </DataTemplate>
                                    </ItemsControl.ItemTemplate>
                                </ItemsControl>

                                <BrowseLine:BrowseRowSeperator x:Uid=""RowSeperatorLine"" x:Name=""RowSeperatorLine""  Grid.Row=""1"" Grid.ColumnSpan=""2""
                                                               Visibility=""{Binding ShowRowSeparatorLine, Converter={StaticResource BoolToVisibilityConverter}, ConverterParameter=hidden}""/>
                            </Grid>
                        </Border>
                    </DataTemplate>";

		private readonly string _WrapPanel =
										@"<ItemsPanelTemplate>
                                            <WrapPanel Orientation=""Horizontal""/>
                                        </ItemsPanelTemplate>";
		private readonly string _StackPanel =
										@"<ItemsPanelTemplate>
                                            <VirtualizingStackPanel Orientation=""Horizontal""/>
                                        </ItemsPanelTemplate>";
		#endregion

		public event CommonDelegateEvents.OnNotifyObjectEventHandler OnNotifyScrollHasReachedTheEndEvent;
		public event CommonDelegateEvents.OnScrollChangedEventHandler ScrollChangedEventHandler;
		public event CommonDelegateEvents.OnNotifyMethodExecutedEventHandler OnUpdateScrollableHeightChanged;
		private ScrollViewer _ContentScroll = null;
		private double prevOffset = 0;

		public ScrollViewer ContentScroll
		{
			get
			{
				if (null == _ContentScroll)
				{
					_ContentScroll = RowItemsControl.Template.FindName ("InnerScrollViewer", RowItemsControl) as ScrollViewer;

					if (null == _ContentScroll)
						return new ScrollViewer ();
					else
					{
						_ContentScroll.Loaded += ContentScrollViewer_Loaded;
					}
				}

				return _ContentScroll;
			}
		}

		public BrowseContentsView ()
		{
			InitializeComponent ();
			this.DataContextChanged += BrowseContentsView_DataContextChanged;
		}

		private void BrowseContentsView_DataContextChanged (object sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.DataContext is BrowseContentsViewModel ViewModel)
			{
				ViewModel.OnCallScrollStateChange += ViewModel_OnCallScrollToEnd;
			}
		}

		private void ViewModel_OnCallScrollToEnd (bool IsToHome)
		{
			Application.Current.Dispatcher.BeginInvoke (new Action (delegate
			{
				if (IsToHome) ContentScroll.ScrollToHome ();
				else ContentScroll.ScrollToEnd ();
			}));
		}

		// Bottom-Right Corner 투명처리
		private void ContentScrollViewer_Loaded (object sender, RoutedEventArgs e)
		{
			Rectangle cornerRectangle = WPFFindChildHelper.FindVisualChild<Rectangle> (ContentScroll);
			if (null != cornerRectangle)
			{
				cornerRectangle.Fill = Brushes.Transparent;
			}

			//InitScrollAnimation (sender);
		}

		private void InitScrollAnimation (object sender)
		{
			if (sender is ScrollViewer pScrollViewer)
			{
				ScrollAnimationBehavior.SetIsEnabled (pScrollViewer, true);
				ScrollAnimationBehavior.SetEventHandlersForScrollViewer (pScrollViewer);
				ScrollAnimationBehavior.SetTimeDuration (pScrollViewer, new TimeSpan (0, 0, 0, 0, 500));
				ScrollAnimationBehavior.SetAccelerationRatio (pScrollViewer, 0.3);
				ScrollAnimationBehavior.SetPointsToScroll (pScrollViewer, 10);
			}
		}

		private void ContentScrollViewer_ScrollChanged (object sender, ScrollChangedEventArgs e)
		{
			if (null != ScrollChangedEventHandler)
			{
				ScrollChangedEventHandler (sender, e);
			}

			if (!Kiss.DoubleEqual (0.0, this.ContentScroll.ScrollableHeight) &&
				Kiss.DoubleEqual (this.ContentScroll.VerticalOffset, this.ContentScroll.ScrollableHeight))
			{
				if (null != OnUpdateScrollableHeightChanged)
				{
					OnUpdateScrollableHeightChanged ();
					OnUpdateScrollableHeightChanged -= UpdateVerticalOffsetOnHeightChange;
					return;
				}
				prevOffset = this.ContentScroll.VerticalOffset;
				OnUpdateScrollableHeightChanged += UpdateVerticalOffsetOnHeightChange;

				if (null != this.OnNotifyScrollHasReachedTheEndEvent)
				{
					this.OnNotifyScrollHasReachedTheEndEvent (null);
				}
			}
		}

		private void UpdateVerticalOffsetOnHeightChange ()
		{
			if (!Kiss.DoubleEqual (ContentScroll.ScrollableHeight, prevOffset))
			{
				ContentScroll.ScrollToVerticalOffset (prevOffset);
			}
		}

		private void RowPanel_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (sender is FrameworkElement element)
			{
				ViewPropertyManager.Instance.CallCommand (element, BrowsePropertyName.MouseLeftButtonDownCommand.ToString (), e);
			}
		}

		private void RowPanel_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
		{
			if (sender is FrameworkElement element)
			{
				ViewPropertyManager.Instance.CallCommand (element, BrowsePropertyName.MouseLeftButtonUpCommand.ToString (), e);
			}
		}

		private void RowPanel_MouseRightButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (sender is FrameworkElement element)
			{
				ViewPropertyManager.Instance.CallCommand (element, BrowsePropertyName.MouseRightButtonDownCommand.ToString (), e);
			}
		}

		private void RootGrid_MouseEnter (object sender, MouseEventArgs e)
		{
			if (sender is FrameworkElement element)
			{
				ViewPropertyManager.Instance.SetProperty (element, BrowsePropertyName.IsMouseEnter.ToString (), true);
			}
		}

		private void RootGrid_MouseLeave (object sender, MouseEventArgs e)
		{
			if (sender is FrameworkElement element)
			{
				ViewPropertyManager.Instance.SetProperty (element, BrowsePropertyName.IsMouseEnter.ToString (), false);
			}
		}

		private void InnerScrollViewer_PreviewMouseWheel (object sender, MouseWheelEventArgs e)
		{
			//ScrollAnimateBehavior.AttachedBehaviors.ScrollAnimationBehavior.ScrollViewerPreviewMouseWheel(sender, e);

			ScrollViewer scrollViewer = sender as ScrollViewer;
			if (scrollViewer != null)
			{
				if (e.Delta > 0)
				{
					scrollViewer.LineUp ();
				}
				else
				{
					scrollViewer.LineDown ();
				}

				e.Handled = true;
			}
		}
	}
}
