using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.StyleResourceDictionary;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom.Components.RecyclerBrowse.ViewModel;
using Softpower.SmartMaker.TopAtom.Components.TreeAtom;

namespace Softpower.SmartMaker.TopAtom
{
	public partial class TreeofAtom : TreeAtomBase
	{

		private CStringArray m_saLoadFieldName;  // Table + Field명
		private CStringArray m_saLoadProperVar;

		public bool IsRunMode
		{
			get
			{
				bool bRunMode = AtomCore.AtomRunMode == 1;
				return bRunMode;
			}
		}

		public TreeofAtom ()
		{
			InitializeComponent ();

			Init ();
			InitEvent ();
		}

		public TreeofAtom (Atom atomCore)
			: base (atomCore)
		{
			InitializeComponent ();

			Init ();
			InitEvent ();
		}

		private void Init ()
		{
			m_saLoadFieldName = new CStringArray ();
			m_saLoadProperVar = new CStringArray ();
			PivotRoot.PivotScrollViewer.Focusable = false;

			ApplyComponentStyle ();
		}

		private void InitEvent ()
		{
			PivotRoot.OnNotifyRowExpanderMouseDownEvent += PivotRoot_OnNotifyRowExpanderMouseDownEvent;
			PivotRoot.OnNotifyMouseDownEvent += PivotRoot_OnNotifyMouseDownEvent;
			PivotRoot.PivotGridScrollViewer.ScrollChanged += PivotGridScrollViewer_ScrollChanged;
			PivotRoot.OnNotifySeperateLineDragComplete += ColumnHeaderPanel_OnNotifySeperateLineDragComplete;
			PivotRoot.OnNotifyChangedBrowseAtomRealLen += ColumnHeaderPanel_OnNotifyChangedBrowseAtomRealLen;
			this.PreviewMouseDown += TreeofAtom_PreviewMouseDown;
		}

		public void ApplyComponentStyle ()
		{
			switch (StyleResourceManager.CurrentTheme)
			{
				case StyleCategory.StyleThemeCategory.Default:
					{
						ScrollViewer CurrentScrollViewer = PivotRoot.PivotGridScrollViewer;

						CurrentScrollViewer.Style = StyleResourceManager.GetAlphaScrollViewerStyle ();

						break;
					}

				default: break;
			}
		}

		protected override void InitializeAtomCore ()
		{
			m_AtomCore = new TreeAtom ();
		}

		protected override void InitializeResizeAdorner ()
		{
			m_ResizeAdorner = new Point8Adorner (this);
		}

		protected override void InitializeAtomSize ()
		{
			Size atomSize = DefaultAtomSizeManager.GetDefaultRect (AtomType.DataTree);
			this.Width = atomSize.Width;
			this.Height = atomSize.Height;
		}

		public override void SetAtomBorder (Brush applyBrush)
		{
			AtomBorderRectangle.Stroke = applyBrush;
		}

		public override Brush GetAtomBorder ()
		{
			return AtomBorderRectangle.Stroke;
		}

		public override void SetAtomThickness (Thickness applyThickness)
		{
			AtomBorderRectangle.StrokeThickness = applyThickness.Left;
		}

		public override Thickness GetAtomThickness ()
		{
			return new Thickness (AtomBorderRectangle.StrokeThickness);
		}

		public override void SetAtomDashArray (DoubleCollection applyDashArray)
		{
			AtomBorderRectangle.StrokeDashArray = applyDashArray;
		}

		public override DoubleCollection GetAtomDashArray ()
		{
			return AtomBorderRectangle.StrokeDashArray;
		}

		public override void CompletePropertyChanged ()
		{
			SetColumns ();
		}

		public override void CloneAtom (AtomBase ClonedAtom, bool bDeepCopy)
		{
			TreeofAtom TreeAtom = ClonedAtom as TreeofAtom;
			TreeAtom.SetAtomBackground (this.GetAtomBackground ());
			TreeAtom.SetAtomBorder (this.GetAtomBorder ());
			TreeAtom.SetAtomDashArray (this.GetAtomDashArray ());
			TreeAtom.SetAtomThickness (this.GetAtomThickness ());

			base.CloneAtom (ClonedAtom, bDeepCopy);
		}

		public override void SerializeLoadSync_AttribToAtom (bool bIs80Model)
		{
			base.SerializeLoadSync_AttribToAtom (bIs80Model);

			TreeAttrib atomAttrib = null == AtomCore ? null : AtomCore.GetAttrib () as TreeAttrib;

			if (null == atomAttrib)
			{
				return;
			}

			this.CompletePropertyChanged ();
		}


		public override void Sync_AttribToAtom ()
		{
			base.Sync_AttribToAtom ();
		}


		#region Apply/Release RunModeProperty

		public override void ReleaseRunModeProperty ()
		{
			base.ReleaseRunModeProperty ();

			this.AtomImage.Visibility = System.Windows.Visibility.Visible;

			DeleteAll ();

			PivotRoot.PivotScrollViewer.Focusable = false;
		}

		public override void ApplyRunModeProperty () // 실행모드시에 가장먼저 호출됨
		{
			TreeAtom treeCore = AtomCore as TreeAtom;

			treeCore.InitRunMode ();

			base.ApplyRunModeProperty ();

			this.AtomImage.Visibility = System.Windows.Visibility.Hidden;
			PivotRoot.PivotScrollViewer.Focusable = true;
		}

		#endregion

		public override void DoPostEditMode ()
		{
			// 만약 사용자 지정 트리일경우 편집모드로 전환시 데이터를 초기화 시킨다.
			if (2 == this.TreeType)
			{
				TreeAtom treeCore = AtomCore as TreeAtom;
				treeCore.TreeSource = null;
				treeCore.TreeGridItem = null;
			}

		}

		//------------------------------------------------------------------------

		public void DeleteAll ()
		{
			PivotRoot.DeleteAllColumns ();
			PivotRoot.DeleteAllRows ();
		}


		public void SetColumns ()
		{
			TreeAtom treeCore = AtomCore as TreeAtom;
			TreeAttrib treeAttrib = treeCore.Attrib as TreeAttrib;
			//CObArray m_oaBrowse = treeAttrib.BrowseAtom;
			CObArray m_oaBrowse = new CObArray ();
			int m_nTreeType = treeAttrib.TreeType;

			foreach (object temp in treeAttrib.BrowseItemList)
			{
				m_oaBrowse.Add (temp);

				BrowseItem tempAtom = temp as BrowseItem;
				if (true == tempAtom.IsParent)
				{
					treeAttrib.KeyLabel[0] = tempAtom.Label;
				}
				else if (true == tempAtom.IsChild)
				{
					treeAttrib.KeyLabel[1] = tempAtom.Label;
				}

			}
			// 상하위 구조의 경우 상위키, 하위키는 출력되면 안되기 때문에 삭제하는 논리
			if (0 < treeAttrib.KeyLabel.Length)
			{
				if (false == string.IsNullOrEmpty (treeAttrib.KeyLabel[0]))
				{
					for (int i = 0; i < m_oaBrowse.Count; i++)
					{
						BrowseItem tempAtom = (BrowseItem)m_oaBrowse[i];
						if (treeAttrib.KeyLabel[0] == tempAtom.Label)
						{
							m_oaBrowse.RemoveAt (i);
						}
					}
				}
			}
			
			PivotRoot.SetPivotGridItems (m_oaBrowse, m_nTreeType, treeAttrib);		
		}

		public void MakeTree ()
		{
			PivotRoot.SetRows ();
		}
		public PivotGridItem GetGridItem ()
		{
			return PivotRoot.GetGridItem ();
		}

		public List<PivotGridItemModel> GetDataSourece ()
		{
			return PivotRoot.DataSource;
		}


		public void SetExapnderItem (PivotGridItemRowExpander currentItem)
		{
			currentItem.SetExpand (true);
			PivotRoot.SetExpanderItem (currentItem);
		}


		public void settingTitle (List<PivotGridItemModel> value)
		{
			PivotRoot.DataSource = value;
			PivotRoot.IsRoot = true;
		}

		public void SetChildTree ()
		{
			PivotRoot.SetColumnWidht ();
		}

		public void settingTree (List<PivotGridItemModel> value)
		{
			PivotRoot.DataSource = value;
		}

		void PivotRoot_OnNotifyRowExpanderMouseDownEvent (object objValue)
		{
			TreeAtom treeCore = AtomCore as TreeAtom;

			treeCore.OnBeforeExpand (objValue);
		}

		private void TreeofAtom_PreviewMouseDown (object sender, MouseButtonEventArgs e)
		{
			foreach (var child in PivotRoot.HeaderGrid.Children)
			{
				if (child is BrowseColumnHeader header)
				{
					header.ShowSelectFlag = false;
				}
			}

			/*
            if (null != AtomCore && RUNMODE_TYPE.PLAY_MODE == AtomCore.GetRunMode ())
            {
                if (1 == e.ClickCount)
                {
                    if (-1 != AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_CLICK))
                    {
                        MsgHandler.CALL_MSG_HANDLER (AtomCore, EVS_TYPE.EVS_A_CLICK, null);
                        AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_CLICK);
                    }
                }
                else if (2 == e.ClickCount)
                {
                    if (-1 != AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_DBL_CLICK))
                    {
                        MsgHandler.CALL_MSG_HANDLER (AtomCore, EVS_TYPE.EVS_A_DBL_CLICK, null);
                        AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_DBL_CLICK);
                    }
                }
            }*/
		}

		public void SetShowPlusMinus ()
		{
			TreeAttrib atomAttrib = this.AtomCore.GetAttrib () as TreeAttrib;

			if (null == atomAttrib)
				return;

			PivotGridItem rootPanel = PivotRoot.RootGridData.Children[0] as PivotGridItem;

			if (null != rootPanel)
			{
				for (int i = 0; i < rootPanel.Children.Count; i++)
				{
					PivotGridItem treeItem = rootPanel.Children[i] as PivotGridItem;

					PivotGridRowTitlePanel titlePanel = WPFFindChildHelper.FindVisualChild<PivotGridRowTitlePanel> (treeItem);

					if (null != titlePanel)
					{
						titlePanel.IsVisibleRowExpander = true == atomAttrib.IsShowPlusMinus ? Visibility.Visible : Visibility.Hidden;
					}
				}
			}
		}

		public void ExpanderMakeRows ()
		{
			PivotRoot.ExpanderMakeRows ();
		}

		public void ExpanderMakeRows (List<PivotGridItemModel> dataSource, PivotGridItem currentItem)
		{
			PivotRoot.ExpanderMakeRows (dataSource, currentItem);
		}

		void PivotRoot_OnNotifyMouseDownEvent (object objValue, object MouseEvent)
		{
			TreeAtom treeCore = AtomCore as TreeAtom;

			treeCore.OnAfterSelect (objValue);

			MouseButtonEventArgs e = MouseEvent as MouseButtonEventArgs;
			PivotGridItemModel gridItemModel = objValue as PivotGridItemModel;


			if (null != e && null != gridItemModel)
			{
				List<object> treeDataList = new List<object> (gridItemModel.ContentsList);

				CVariantX[] pvaArgs = new CVariantX[1 + 1];

				CVarArrayX dataArray = new CVarArrayX ();

				for (int i = 0; i < treeDataList.Count - 2; i++) //ContentsList에 표시된 데이터보다 빈값으로 2개더 추가되어 있어서 -2 처리해주고 있음
				{
					dataArray.Add (new CVariantX (treeDataList[i]));
				}

				pvaArgs[0] = new CVariantX (1);
				pvaArgs[1] = new CVariantX (dataArray);

				if (null != AtomCore && RUNMODE_TYPE.PLAY_MODE == AtomCore.AtomRunMode)
				{
					if (1 == e.ClickCount)
					{
						if (-1 != AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_CLICK, pvaArgs))
						{
							if (0 <= MsgHandler.CALL_MSG_HANDLER (AtomCore, EVS_TYPE.EVS_A_CLICK, pvaArgs))
							{
								AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_CLICK, pvaArgs);
							}
						}
					}
					else if (2 == e.ClickCount)
					{
						if (-1 != AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_DBL_CLICK, pvaArgs))
						{
							if (0 <= MsgHandler.CALL_MSG_HANDLER (AtomCore, EVS_TYPE.EVS_A_DBL_CLICK, pvaArgs))
							{
								AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_DBL_CLICK, pvaArgs);
							}
						}
					}
				}
			}
		}

		public int SelectMsgSend ()
		{
			if (null == AtomCore)
			{
				return -1;
			}

			int nResult = AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_SEL_CHANGE);
			if (-1 != nResult)
			{
				nResult = MsgHandler.CALL_MSG_HANDLER (AtomCore, EVS_TYPE.EVS_A_SEL_CHANGE, null);
				if (0 <= nResult)
				{
					AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_SEL_CHANGE);
				}
			}

			return nResult;
		}

		public void SetLoadFieldName (CStringArray psaLoadFieldName)
		{
			m_saLoadFieldName.Clear ();
			for (int i = 0; i < psaLoadFieldName.Count; i++)
			{
				string sLoadFieldName = psaLoadFieldName.GetAt (i);
				m_saLoadFieldName.Add (sLoadFieldName);
			}
		}

		public void SetLoadProperVar (CStringArray psaLoadProperVar)
		{
			m_saLoadProperVar.Clear ();
			for (int i = 0; i < psaLoadProperVar.Count; i++)
			{
				string strLoadProperVar = psaLoadProperVar.GetAt (i);
				m_saLoadProperVar.Add (strLoadProperVar);
			}
		}

		public CStringArray GetLoadProperVar () { return m_saLoadProperVar; }
		public CStringArray GetLoadFieldName () { return m_saLoadFieldName; }

		void PivotGridScrollViewer_ScrollChanged (object sender, ScrollChangedEventArgs e)
		{
			if (false == this.IsEnabledScroll)
			{
				ScrollViewer pScrollViewer = sender as ScrollViewer;
				if (null != pScrollViewer)
				{
					if (pScrollViewer.ActualHeight < pScrollViewer.ExtentHeight)
					{
						this.IsEnabledScroll = true;
					}
				}
			}
		}


		#region BrowseColumnHeader 함수 구현

		public override void SetResizeAdornerVisibility (Visibility isVisible, bool bIsRoutedChildren)
		{
			base.SetResizeAdornerVisibility (isVisible, bIsRoutedChildren);

			if (isVisible == System.Windows.Visibility.Collapsed)
			{
				foreach(var child in PivotRoot.HeaderGrid.Children)
				{
					if(child is BrowseColumnHeader header)
					{
						header.ShowSelectFlag = false;
					}
				}
			}
		}

		public override void SetAtomFontFamily (FontFamily applyFontFamily)
		{
			bool bHeaderArea = IsHeaderColumnSelected ();

			if(bHeaderArea)
			{
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{	
						header.SetFontFamily (applyFontFamily);
					}
				}
			}
			else
			{
				FontFamily originalFontFamily = null;
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						originalFontFamily = header.GetFontFamily ();
					}
				}
				
				base.SetAtomFontFamily (applyFontFamily);

				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						if(originalFontFamily != null)
						{
							header.SetFontFamily (originalFontFamily);
						}						
					}
				}
			}
		}

		public override void SetAtomBackground (Brush applyBrush)
		{
			bool bHeaderArea = IsHeaderColumnSelected ();

			if (true == bHeaderArea)
			{
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						header.SetBackground (applyBrush);
					}
				}
			}
			else
			{
				Brush originalBrush = null;
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						originalBrush = header.GetBackground ();
					}
				}

				base.SetAtomBackground (applyBrush);

				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						header.SetBackground (originalBrush);
					}
				}
			}
		}

		public override void SetAtomFontColor (Brush applyBrush)
		{
			bool bHeaderArea = IsHeaderColumnSelected ();

			if (true == bHeaderArea)
			{
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						header.SetFontColor (applyBrush);
					}
				}
			}
			else
			{
				Brush originalBrush = null;
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						originalBrush = header.GetFontColor ();
					}
				}

				base.SetAtomFontColor (applyBrush);

				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						header.SetFontColor (originalBrush);
					}
				}
			}
		}

		public override void SetAtomFontWeight (FontWeight applyFontWeight)
		{
			bool bHeaderArea = IsHeaderColumnSelected ();

			if (true == bHeaderArea)
			{
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						header.SetFontWeight (applyFontWeight);
					}
				}
			}
			else
			{
				FontWeight originalFontWeight = FontWeights.Normal;
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						originalFontWeight = header.GetFontWeight ();
					}
				}

				base.SetAtomFontWeight (applyFontWeight);				

				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						header.SetFontWeight (originalFontWeight);
					}
				}
			}
		}

		public override void SetAtomFontStyle (FontStyle applyFontStyle)
		{

			bool bHeaderArea = IsHeaderColumnSelected ();

			if (true == bHeaderArea)
			{
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						header.SetFontStyle (applyFontStyle);
					}
				}
			}
			else
			{
				FontStyle originalFontStyle = FontStyles.Normal;
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						originalFontStyle = header.GetFontStyle ();
					}
				}

				base.SetAtomFontStyle (applyFontStyle);

				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						header.SetFontStyle (originalFontStyle);
					}
				}
			}
		}

		public override void SetAtomFontSize (double dApplySize)
		{
			bool bHeaderArea = IsHeaderColumnSelected ();

			if (true == bHeaderArea)
			{
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						header.SetFontSize (dApplySize);
					}
				}
			}
			else
			{
				double originalFontSize = 10;
				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						originalFontSize = header.GetFontSize ();
					}
				}

				base.SetAtomFontSize (dApplySize);

				foreach (var child in PivotRoot.HeaderGrid.Children)
				{
					if (child is BrowseColumnHeader header)
					{
						header.SetFontSize (originalFontSize);
					}
				}
			}
		}


		// MouseDrag 완료 시 실행되는 함수
		// BrowseColumnHeader 사용때문에 구현한 함수로 특별한 기능은 없음.
		private void ColumnHeaderPanel_OnNotifySeperateLineDragComplete (object objValue)
		{
			UpdateLayout ();
		}

		void ColumnHeaderPanel_OnNotifyChangedBrowseAtomRealLen (object browseHeader, object width)
		{
			BrowseItem targetHeader = browseHeader as BrowseItem;
			int nWidth = _Kiss.toInt32 (width);

			if (null != targetHeader)
			{
				TreeAttrib atomAttrib = this.AtomCore.GetAttrib () as TreeAttrib;

				foreach (BrowseItem browseAtom in atomAttrib.BrowseItemList)
				{
					if (browseAtom.BrowseVar == targetHeader.BrowseVar)
					{
						browseAtom.RealLen = nWidth;
						break;
					}
				}
			}

			base.CompletePropertyChanged ();
		}

		public bool IsHeaderColumnSelected ()
		{
			bool bIsHeaderSelected = false;

			foreach (var child in PivotRoot.HeaderGrid.Children)
			{
				if (child is BrowseColumnHeader header)
				{
					bIsHeaderSelected = header.ShowSelectFlag;
				}
			}

			return bIsHeaderSelected;
		}

		#endregion

	}
}