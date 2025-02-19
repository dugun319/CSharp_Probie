using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Softpower.SmartMaker.AnimationLib;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.StyleResourceDictionary;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.EditSaveData;
using Softpower.SmartMaker.TopBuild.ViewModels.ExpandMenu;
using Softpower.SmartMaker.TopControl.Components.Expander;

namespace Softpower.SmartMaker.TopBuild.Views.ExpandMenu
{
	public partial class OfficeAppAtomMenuView : OfficeExpandMenuViewBase
	{
		private ExpanderOwner m_SelectExpand = null;
		protected override string MenuType => "AppMenu";

		protected override StackPanel ExpanderPanel => ExpandContainerStackPanel;

		#region 생성자

		public OfficeAppAtomMenuView ()
		{
			InitializeComponent ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("OfficeAppAtomMenuView", this);
			}

			InitializeExpandItems ();
			ApplyApplicationComponentStyle ();
		}


		#endregion

		#region 초기화

		private void ApplyApplicationComponentStyle ()
		{
			ResourceDictionary FindedResource = null;

			switch (StyleResourceManager.CurrentTheme)
			{
				case StyleCategory.StyleThemeCategory.Default:
					{
						FindedResource = StyleResourceManager.GetApplicationComponentStyle (StyleCategory.StyleComponentCategory.ScrollViewer);
						ExpandContainerScrollViewer.Resources = FindedResource;

						ControlTemplate ApplyResource = FindedResource["DefaultScrollViewerControlTemplate"] as ControlTemplate;
						ExpandContainerScrollViewer.Template = ApplyResource;

						break;
					}

				default: break;
			}
		}

		protected override void InitializeExpandMenuAnimation ()
		{
			m_MenuStartAnimation = AnimationManager.GetAnimationByName ("AtomMenuSlideStart");
			m_MenuEndAnimation = AnimationManager.GetAnimationByName ("AtomMenuSlideEnd");
		}

		protected override void InitializeExpandItems ()
		{
			if (null != DataContext)
			{
				OfficeAppAtomMenuViewModel ViewModel = DataContext as OfficeAppAtomMenuViewModel;

				ExpanderOwner FixedExpander = ViewModel.MakeFixedExpanderOwner ();

                FixedExpander.ItemsPanel.Visibility = Visibility.Collapsed;
				FixedExpander.Header.IsExpanded = false;
				FixedExpander.MouseMove += MakedExpander_MouseMove;
				FixedExpander.Header.MouseLeftButtonDown += MakedExpander_MouseLeftButtonDown;

				m_FixedExpander = FixedExpander;
				ExpandContainerStackPanel.Children.Add (FixedExpander);

				List<AtomExpanderItem> FixedExpanderList = new List<AtomExpanderItem> ();
				List<AtomExpanderItem> RecentExpanderList = new List<AtomExpanderItem> ();

				int nItemCount = 10;

				for (int nIndex = 0; nIndex < nItemCount; nIndex++)
				{
					ExpanderOwner MakedExpander = ViewModel.MakeExpander (nIndex);

					if (null != MakedExpander)
					{
						MakedExpander.ItemsPanel.Visibility = System.Windows.Visibility.Collapsed;
						MakedExpander.Header.IsExpanded = false;

						ExpanderItemsPanel ItemsPanel = MakedExpander.ItemsPanel;
						UIElementCollection ItemsPanelChildren = ItemsPanel.Children;

						MakedExpander.MouseMove += MakedExpander_MouseMove;
						MakedExpander.Header.MouseLeftButtonDown += MakedExpander_MouseLeftButtonDown;

						foreach (AtomExpanderItem Item in ItemsPanelChildren)
						{
							SetEventToExpanderItem (Item);

							if (EditSaveDataManager.Instance.IsContainsFixedAtomMenu (MenuType, Item.CurrentAtomType))
							{
								Item.IsFixed = true;

								AtomExpanderItem cloneItem = MakeFixedExpanderItem (Item);

								FixedExpanderList.Add (cloneItem);
							}

							if (EditSaveDataManager.Instance.IsContainsRecentAtomMenu (MenuType, Item.CurrentAtomType))
							{
								AtomExpanderItem cloneItem = MakeFixedExpanderItem (Item);
								cloneItem.IsRecentItem = true;

								RecentExpanderList.Add (cloneItem);
							}
						}

						ExpandContainerStackPanel.Children.Add (MakedExpander);
					}
				}

				AddFixedAreaItem (FixedExpanderList, RecentExpanderList);
			}
		}

		void MakedExpander_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			m_SelectExpand = WPFFindChildHelper.FindAnchestor<ExpanderOwner> (sender as ExpanderHeader);
		}

		void MakedExpander_MouseMove (object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (m_SelectExpand == null)
					return;

				DragDrop.DoDragDrop (m_SelectExpand, m_SelectExpand, DragDropEffects.Move);
			}
		}

		#endregion

		protected override void ExpanderItem_MouseLeave (object sender, MouseEventArgs e)
		{
			ExpanderItem CurrentExpander = sender as ExpanderItem;
			CurrentExpander.Background = Application.Current.FindResource ("LeftToolBar_AtomMenuControl_SubItem_Background") as Brush;
			m_MenuEndAnimation.Begin (CurrentExpander);
		}

		protected override void ExpanderItem_MouseEnter (object sender, MouseEventArgs e)
		{
			ExpanderItem CurrentExpander = sender as ExpanderItem;
			CurrentExpander.Background = Application.Current.FindResource ("LeftToolBar_AtomMenuControl_SubItem_MouseOver_Background") as Brush;
			m_MenuStartAnimation.Begin (CurrentExpander);
		}

		protected override void ExpanderItem_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			m_SelectExpand = null;

			AtomExpanderItem CurrentExpander = sender as AtomExpanderItem;

			if (null != CurrentExpander)
			{
				if (CurrentExpander.IsFixStateChanging)
				{
					CurrentExpander.IsFixStateChanging = false;
					return;
				}

				GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyMakeAtom (CurrentExpander.CurrentAtomType, false, true);

				// 즐겨찾기 메뉴에 있는 아톰은 다시 선택해도 즐겨찾기에서 위치변경 되지 않음
                if (EditSaveDataManager.Instance.IsContainsRecentAtomMenu(MenuType, CurrentExpander.CurrentAtomType))
				{
					return;
				}
				else
				{
                    AddRecentExpanderItem(CurrentExpander);
                }
			}
		}

		public override void AdjustScrollViewerHeight (double dParentHeight)
		{
			double dScrollViewerHeight = (dParentHeight - TitleGrid.Height);
			ExpandContainerScrollViewer.Height = Math.Max (0, dScrollViewerHeight);
		}

		private void ExpanderGrid_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			OnExpandButtonClick ();
		}

		public override void ChangeExpandeMode (bool bIsFullMode)
		{
			//풀모드 일때
			if (true == bIsFullMode)
			{
				TitleTextBlock.Visibility = System.Windows.Visibility.Visible;

				ExpandArrow1.Angle = 270;
				ExpandArrow2.Angle = 270;
			}
			//축약모드 일때
			else
			{
				TitleTextBlock.Visibility = System.Windows.Visibility.Collapsed;

				ExpandArrow1.Angle = 90;
				ExpandArrow2.Angle = 90;
			}

			foreach (ExpanderOwner expanderOwner in ExpandContainerStackPanel.Children)
			{
				expanderOwner.ExpanderMode (bIsFullMode);
			}
		}

		private void ExpandContainerStackPanel_DragOver (object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent (typeof (ExpanderOwner)))
			{
				e.Effects = DragDropEffects.Move;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
		}

		private void ExpandContainerStackPanel_Drop (object sender, DragEventArgs e)
		{
			if (m_SelectExpand == null)
				return;

			if (e.Data.GetDataPresent (typeof (ExpanderOwner)))
			{
				ExpanderOwner NowOwner = null;

				GetMouseBelowNode (ExpandContainerStackPanel.Children, ref NowOwner, e.GetPosition (this));

				if (NowOwner == null)
					return;

				ExpanderOwner Owner = e.Data.GetData (typeof (ExpanderOwner)) as ExpanderOwner;

				int nIndex = ExpandContainerStackPanel.Children.IndexOf (NowOwner);

				ExpandContainerStackPanel.Children.Remove (Owner);
				ExpandContainerStackPanel.Children.Insert (nIndex, Owner);

			}
		}

		internal ExpanderOwner GetMouseBelowNode (UIElementCollection nodeCollection, ref ExpanderOwner WillSelectedNode, Point ptMouse)
		{
			foreach (ExpanderOwner node in nodeCollection)
			{
				if (null != WillSelectedNode)
				{
					break;
				}

				if (true == node.IsVisible)
				{
					Point ptNode = node.TransformToAncestor (this).Transform (new Point (0, 0));
					double dNodeWidth = node.ActualWidth;
					double dNodeHeight = node.ActualHeight;

					if (ptNode.X < ptMouse.X && ptNode.X + dNodeWidth > ptMouse.X
						&& ptNode.Y < ptMouse.Y && ptNode.Y + dNodeHeight > ptMouse.Y)
					{
						WillSelectedNode = node;
						break;
					}
				}
			}
			return WillSelectedNode;
		}


	}



}
