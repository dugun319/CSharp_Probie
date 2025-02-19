using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopBuild.ViewModels.ToolBar.ToolBarGroup;
using Softpower.SmartMaker.TopBuild.Views.ToolBar.ToolBarComboBoxes;
using Softpower.SmartMaker.TopProcess;

namespace Softpower.SmartMaker.TopBuild.Views.ToolBar.ToolBarGroup
{
	public partial class OfficeObjectToolBarGroup : ToolBarGroupBase
	{
		private bool m_bUpdateQuickMenu = true;

		public OfficeObjectToolBarGroup ()
		{
			InitializeComponent ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("OfficeObjectToolBarGroup", this);
			}

			InitializeData ();
			InitializeSubComboBoxEvents ();
		}

		private void InitializeSubComboBoxEvents ()
		{
			LineColorComboBox.OnTranslateColorDialogEvent += LineColorComboBox_OnTranslateColorDialogEvent;
			BackgroundColorComboBox.OnTranslateColorDialogEvent += BackgroundColorComboBox_OnTranslateColorDialogEvent;
		}

		private void InitializeData ()
		{
			if (null != DataContext)
			{
				OfficeObjectToolBarGroupViewModel ViewModel = DataContext as OfficeObjectToolBarGroupViewModel;

				if (null != ViewModel)
				{
					ZIndexComboBox.ItemSource = ViewModel.GetItemSource (0);
					AlignmentComboBox.ItemSource = ViewModel.GetItemSource (1);
					ArrangementComboBox.ItemSource = ViewModel.GetItemSource (2);
					ArrangementComboBox.SelectedIndex = 2;
				}
			}
		}

		private void AlignmentComboBox_OnNotifySelectionChangedEvent (object objValue)
		{
			int nCurrentSelectedIndex = AlignmentComboBox.SelectedIndex;

			if (-1 != nCurrentSelectedIndex)
			{
				bool bIsComplete = RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)DelegateEventKeys.ObjectPropertyEventKey.OBJECTALIGNMENT, nCurrentSelectedIndex);
			}
		}

		private void ArrangementComboBox_OnNotifySelectionChangedEvent (object objValue)
		{
			int nCurrentSelectedIndex = ArrangementComboBox.SelectedIndex;

			if (-1 != nCurrentSelectedIndex)
			{
				bool bIsComplete = RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)DelegateEventKeys.ObjectPropertyEventKey.OBJECTALIGNMENT, (nCurrentSelectedIndex + 10));
			}
		}

		private void ZIndexComboBox_OnNotifySelectionChangedEvent (object objValue)
		{
			int nCurrentSelectedIndex = ZIndexComboBox.SelectedIndex;

			if (-1 != nCurrentSelectedIndex)
			{
				ChangeAtomZIndexDefine.ActionType ApplyType = ChangeAtomZIndexDefine.ActionType.한단계앞으로;

				switch (nCurrentSelectedIndex)
				{
					case 0:
						{
							ApplyType = ChangeAtomZIndexDefine.ActionType.한단계앞으로;
							break;
						}

					case 1:
						{
							ApplyType = ChangeAtomZIndexDefine.ActionType.한단계뒤로;
							break;
						}

					case 2:
						{
							ApplyType = ChangeAtomZIndexDefine.ActionType.맨앞으로가져오기;
							break;
						}

					case 3:
						{
							ApplyType = ChangeAtomZIndexDefine.ActionType.맨뒤로보내기;
							break;
						}

					default: break;
				}

				GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyAtomZIndexChanged (ApplyType);
			}
		}

		private void LineColorComboBox_OnTranslateColorDialogEvent (DelegateEventKeys.ColorDialogEventKey ColorDialogKey, object applyValue)
		{
			bool bIsComplete = false;
			if (DelegateEventKeys.ColorDialogEventKey.BRUSH == ColorDialogKey)
			{
				QuickMenuData.NoQuickLineColor = false;
				QuickMenuData.QuickLineColor = (Brush)applyValue;
				bIsComplete = RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)DelegateEventKeys.ObjectPropertyEventKey.LINECOLOR, applyValue);

				if (true == bIsComplete)
				{
					LineColorComboBox.UpdateLineColorBrush ((Brush)applyValue);
				}
			}
			else
			{
				bool temp = false;
				if (true == bool.TryParse (applyValue.ToString (), out temp))
				{
					QuickMenuData.NoQuickLineColor = (bool)applyValue;
				}

				bIsComplete = RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)ColorDialogKey, applyValue);
			}
		}

		private void BackgroundColorComboBox_OnTranslateColorDialogEvent (DelegateEventKeys.ColorDialogEventKey ColorDialogKey, object applyValue)
		{
			bool bIsComplete = false;

			if (DelegateEventKeys.ColorDialogEventKey.BRUSH == ColorDialogKey)
			{
				QuickMenuData.NoQuickBackColor = false;

				QuickMenuData.QuickBackground = (Brush)applyValue;
				bIsComplete = RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)DelegateEventKeys.ObjectPropertyEventKey.BACKGROUNDCOLOR, applyValue);

				if (true == bIsComplete)
				{
					BackgroundColorComboBox.UpdateBackgroundColor ((Brush)applyValue);
				}
			}
			else
			{
				QuickMenuData.NoQuickBackColor = true;
				bIsComplete = RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)ColorDialogKey, applyValue);
			}
		}

		public void UpdateObjectPropertyToolBarGroup (ToolBarProperty toolBarProperty)
		{
			if (null != toolBarProperty)
			{
				Type SourceType = toolBarProperty.TargetType;
				Brush updateLineBrush = toolBarProperty.LineBrush;
				Brush updateBackgroundBrush = toolBarProperty.BackgroundBrush;
				Thickness updateLineThickness = toolBarProperty.LineThickness;
				DoubleCollection updateDashArray = toolBarProperty.UpdateDoubleCollection;

				bool bIsNoLine = toolBarProperty.IsNoLIne;
				bool bIsNoBackground = toolBarProperty.IsNoBackgrond;
				bool bIsHide = toolBarProperty.IsHide;
				bool CellBorderTypesVisibility = toolBarProperty.IsCellBordertypeVisibilty;
				bool bShadow = toolBarProperty.IsShadow;
				bool? isExnapdArea = toolBarProperty.IsBackgroundExpandArea;
				bool? isLineColorEnabled = toolBarProperty.IsLineColorEnabled;

				int nAtomOpacity = toolBarProperty.AtomOpactiy;
				int nSelectedAtomCount = toolBarProperty.SelectAtomCount;

                ////20250211 KH CalendarAttrib Get BorderIndex
                int nBorderIndex = toolBarProperty.SelectAtomeBorderIndex;

                LineColorComboBox.UpdateCellBorderVisibility (CellBorderTypesVisibility);
				LineColorComboBox.UpdateNoLine (bIsNoLine);


				LineColorComboBox.UpdateLineType (updateDashArray);

				BackgroundColorComboBox.UpdateAtomOpacity (nAtomOpacity);
				BackgroundColorComboBox.UpdateAtomShadow (bShadow);
				BackgroundColorComboBox.UpdateExnapdArea (isExnapdArea);
				BackgroundColorComboBox.UpdateNoBackground (bIsNoBackground);

				//2022-11-17 kys 파워포인트와 동일하게 배경색, 글자색은 현재 객체의 값이 아닌 마지막에 설정한 값으로 유지되도록 한다.
				LineColorComboBox.UpdateLineColorComboBox (SourceType);
				//LineColorComboBox.UpdateLineThickness(updateLineThickness);

				BackgroundColorComboBox.UpdateBackgroundColorComboBox (SourceType);
				//BackgroundColorComboBox.UpdateNoBackground(bIsNoBackground);

				UpdateShowHide (bIsHide);

				SetToolBarEnable (SourceType, nSelectedAtomCount, nBorderIndex);

				if (null != isLineColorEnabled)
				{
					LineColorComboBox.IsEnabled = (bool)isLineColorEnabled;
				}

			}
			else
			{
				UpdateDefault ();
				SetToolBarEnable (null, 0, 0);
			}
		}


		private void SetToolBarEnable (Type sourceType, int nAtomCount, int nBorderIndex)
		{
			AlignmentComboBox.IsEnabled = false;
			ArrangementComboBox.IsEnabled = false;
			ZIndexComboBox.IsEnabled = false;
			//BackgroundColorComboBox.IsEnabled = false;
			//LineColorComboBox.IsEnabled = false;
			ShowHideBorder.IsEnabled = false;
			QuickStyleBorder.IsEnabled = false;

			if (null != sourceType)
			{
				//if (typeof(Canvas) == sourceType)
				//{
				//    BackgroundColorComboBox.IsEnabled = true;
				//}

				//선만 사용하는 아톰들
				if (typeof (VHLineofAtom) == sourceType || typeof (FreeLineofAtom) == sourceType)
				{
					//LineColorComboBox.IsEnabled = true;
					ZIndexComboBox.IsEnabled = true;
				}
				else if (typeof (CalendarofAtom) == sourceType)
				{
					if(nBorderIndex == 3)
					{
							LineColorComboBox.BorderGroupPanel.Visibility = Visibility.Collapsed;

                    }
					else
					{
                        LineColorComboBox.LineTypeContentStackPanelHeader.Visibility = Visibility.Collapsed;
                        LineColorComboBox.LineTypeContentStackPanel.Visibility = Visibility.Collapsed;
                    }
				}
				else
				{
					ZIndexComboBox.IsEnabled = true;
					//BackgroundColorComboBox.IsEnabled = true;
					//LineColorComboBox.IsEnabled = true;
					ShowHideBorder.IsEnabled = true;
					QuickStyleBorder.IsEnabled = true;
				}
			}
			else
			{
				BackgroundColorComboBox.IsEnabled = true;
			}

			if (1 <= nAtomCount)
			{
				AlignmentComboBox.IsEnabled = true;
			}

			if (1 < nAtomCount)
			{
				ArrangementComboBox.IsEnabled = true;
			}
		}

		private void UpdateDefault ()
		{
			ShowHideBorder.Background = null;
		}

		private void UpdateShowHide (bool bIsHide)
		{
			switch (bIsHide)
			{
				case true:
					{
						ShowHideBorder.Background = OfficeMainToolBarView.AppliedBrush;
						break;
					}

				case false:
					{
						ShowHideBorder.Background = null;
						break;
					}

				default: break;
			}
		}

		public void CloseAllToolBarPopup ()
		{
			LineColorComboBox.ClosePopup ();
			BackgroundColorComboBox.ClosePopup ();
			AlignmentComboBox.ClosePopup ();
			ArrangementComboBox.ClosePopup ();
			ZIndexComboBox.ClosePopup ();
		}

		private void ShowHideBorder_MouseEnter (object sender, MouseEventArgs e)
		{
			Border CurrentBorder = sender as Border;
			Brush CurrentBrush = CurrentBorder.Background;

			if (null != CurrentBrush)
			{
				SolidColorBrush CurrentSolidBrush = CurrentBrush as SolidColorBrush;

				if (CurrentSolidBrush.Color != OfficeMainToolBarView.AppliedBrush.Color)
				{
					CurrentBorder.Background = OfficeMainToolBarView.MouseEnterBrush;
				}
			}
			else
			{
				CurrentBorder.Background = OfficeMainToolBarView.MouseEnterBrush;
			}
		}

		private void ShowHideBorder_MouseLeave (object sender, MouseEventArgs e)
		{
			Border CurrentBorder = sender as Border;
			Brush CurrentBrush = CurrentBorder.Background;

			if (null != CurrentBrush)
			{
				SolidColorBrush CurrentSolidBrush = CurrentBrush as SolidColorBrush;

				if (CurrentSolidBrush.Color != OfficeMainToolBarView.AppliedBrush.Color)
				{
					CurrentBorder.Background = null;
				}
			}
		}

		private void ShowHideBorder_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			bool bIsHide = true;
			Brush CurrentBrush = ShowHideBorder.Background;

			if (OfficeMainToolBarView.AppliedBrush == CurrentBrush)
			{
				bIsHide = false;
			}

			bool bIsComplete = RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)DelegateEventKeys.ObjectPropertyEventKey.SHOWHIDE, bIsHide);

			if (true == bIsComplete)
			{
				if (OfficeMainToolBarView.AppliedBrush == CurrentBrush)
				{
					ShowHideBorder.Background = null;
				}
				else
				{
					ShowHideBorder.Background = OfficeMainToolBarView.AppliedBrush;
				}
			}
		}

		public List<Popup> GetAllPopupPanels ()
		{
			List<Popup> lstPopupPanels = new List<Popup> ();
			lstPopupPanels.Add (AlignmentComboBox.GetPopupPanel ());
			lstPopupPanels.Add (ArrangementComboBox.GetPopupPanel ());
			lstPopupPanels.Add (ZIndexComboBox.GetPopupPanel ());
			lstPopupPanels.Add (BackgroundColorComboBox.GetPopupPanel ());
			lstPopupPanels.Add (LineColorComboBox.GetPopupPanel ());

			return lstPopupPanels;
		}

		private void QuickStyleBorder_MouseEnter (object sender, MouseEventArgs e)
		{
			Border CurrentBorder = sender as Border;
			Brush CurrentBrush = CurrentBorder.Background;

			if (null != CurrentBrush)
			{
				SolidColorBrush CurrentSolidBrush = CurrentBrush as SolidColorBrush;

				if (CurrentSolidBrush.Color != OfficeMainToolBarView.AppliedBrush.Color)
				{
					CurrentBorder.Background = OfficeMainToolBarView.MouseEnterBrush;
				}
			}
			else
			{
				CurrentBorder.Background = OfficeMainToolBarView.MouseEnterBrush;
			}
		}

		private void QuickStyleBorder_MouseLeave (object sender, MouseEventArgs e)
		{
			Border CurrentBorder = sender as Border;
			Brush CurrentBrush = CurrentBorder.Background;

			if (null != CurrentBrush)
			{
				SolidColorBrush CurrentSolidBrush = CurrentBrush as SolidColorBrush;

				if (CurrentSolidBrush.Color != OfficeMainToolBarView.AppliedBrush.Color)
				{
					CurrentBorder.Background = null;
				}
			}
		}

		private void QuickStyleBorder_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			m_bUpdateQuickMenu = false;
			//글꼴
			if (null != QuickMenuData.QuickFontFamily)
			{
				RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)DelegateEventKeys.ObjectPropertyEventKey.FONT, QuickMenuData.QuickFontFamily);
			}

			//글자 크기
			if (-1 < QuickMenuData.QuickFontSize)
			{
				RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)DelegateEventKeys.ObjectPropertyEventKey.FONTSIZE, QuickMenuData.QuickFontSize.ToString ());
			}

			//글자색
			if (null != QuickMenuData.QuickFontColor)
			{
				RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)DelegateEventKeys.ObjectPropertyEventKey.FONTCOLOR, QuickMenuData.QuickFontColor);
			}

			//외간선 색상
			if (null != QuickMenuData.QuickLineColor)
			{
				RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)DelegateEventKeys.ObjectPropertyEventKey.LINECOLOR, QuickMenuData.QuickLineColor);
			}

			//배경색 색상
			if (null != QuickMenuData.QuickBackground)
			{
				RaiseCommonEvent (DelegateStructType.EventSourceType.OBJECTPROPERTYTOOLBARGROUP, (int)DelegateEventKeys.ObjectPropertyEventKey.BACKGROUNDCOLOR, QuickMenuData.QuickBackground);
			}

			m_bUpdateQuickMenu = true;
		}

		public void UpdateQuickMenu ()
		{
			if (false == m_bUpdateQuickMenu)
				return;

			if (null != QuickMenuData.QuickFontColor)
			{
				QuickFontTextBlock.Foreground = QuickMenuData.QuickFontColor;
				QuickBackground.Fill = QuickMenuData.QuickFontColor;
			}

			if (null != QuickMenuData.QuickLineColor)
			{
				if (false == QuickMenuData.NoQuickLineColor)
				{
					QuickStyleBorder.BorderBrush = QuickMenuData.QuickLineColor;
				}
				else
				{
					QuickStyleBorder.BorderBrush = Brushes.Transparent;
				}
			}

			if (null != QuickMenuData.QuickBackground)
			{
				if (false == QuickMenuData.NoQuickBackColor)
				{
					QuickGrid.Background = QuickMenuData.QuickBackground;
				}
				else
				{
					QuickGrid.Background = Brushes.Transparent;
				}
			}
		}
	}
}
