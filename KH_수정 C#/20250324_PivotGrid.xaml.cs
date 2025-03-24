using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.TopApp;

namespace Softpower.SmartMaker.TopAtom.Components.TreeAtom
{
	/// <summary>
	/// Interaction logic for PivotGrid.xaml
	/// </summary>
	public partial class PivotGrid : UserControl
	{

		public event CommonDelegateEvents.OnNotifyObjectEventHandler OnNotifyRowExpanderMouseDownEvent = null;
		public event CommonDelegateEvents.OnNotifyTwoObjectEventHandler OnNotifyMouseDownEvent = null;

		public event CommonDelegateEvents.OnNotifyObjectEventHandler OnNotifyChangeHeaderLevel; // 180806_AHN
		public event CommonDelegateEvents.OnNotifyArrayListEventHandler OnNotifySorting;
		public event CommonDelegateEvents.OnNotifyArrayListEventHandler OnNotifyChangeHeaderPosition;
		public event CommonDelegateEvents.OnNotifyObjectEventHandler OnNotifyHeaderClicked;
		public event CommonDelegateEvents.OnNotifyBoolValueEventHandler OnNotifyHeaderChecked;
		public event CommonDelegateEvents.OnNotifyTwoObjectEventHandler OnNotifyChangedBrowseAtomRealLen;
		public event CommonDelegateEvents.OnNotifyIntValueEventHandler OnNotifySeperateLineDoubleClick;
		public event CommonDelegateEvents.OnNotifyArrayListEventHandler ChangeColumnShowHide;
		public event CommonDelegateEvents.OnNotifyIntValueEventHandler ShowFilterDlg;

		private PivotGridManager m_PivotGridManager;
		private PivotGridItemModel m_RootModel;
		private List<PivotGridItemModel> m_DataSource;
		private PivotGridItemManager m_PivotGridItemManager;
		private double[] m_arColumnWidth;
		private List<string> m_arHeaderList;


		private PivotGridItem m_GridItem;
		private int m_nSelectIndex;

		private bool isHeaderView = true;

		private TreeAttrib pTreeAttrib = null;

		//private static CTreeAttrib temp1;
		//private static int numbertemp1 = 0;

		//private int m_widthArryNum;


		private bool isRoot = false;

		private ArrayList KeyLabelList = null;

		private int m_headerCount = 0;

		public double[] ColumnWidth
		{
			get { return m_arColumnWidth; }
			set { m_arColumnWidth = value; }
		}


		public int SelectIndex
		{
			get
			{
				return m_nSelectIndex;
			}
			set
			{
				m_nSelectIndex = value;
			}
		}

		public int HeaderCount
		{
			get { return m_headerCount; }
			set { m_headerCount = value; }
		}

		public bool IsRoot
		{
			get { return isRoot; }
			set { isRoot = value; }
		}

		public ScrollViewer PivotGridScrollViewer
		{
			get
			{
				return PivotScrollViewer;
			}
		}

		public List<PivotGridItemModel> DataSource
		{
			get { return m_DataSource; }
			set { m_DataSource = value; }
		}

		public PivotGridItemModel RootModel
		{
			get { return m_RootModel; }
			set { m_RootModel = value; }
		}

		public PivotGridItemManager PivotGridManager
		{
			get { return m_PivotGridItemManager; }
		}

		public bool ViewHeader
		{
			get { return isHeaderView; }
			set { isHeaderView = value; }
		}

		public void SetExpanderItem (PivotGridItemRowExpander currentItem)
		{
			m_PivotGridItemManager.Expander = currentItem;
		}


		public PivotGrid ()
		{
			InitializeComponent ();
			InitializeManager ();
			InitializeHeaderList ();
			InitializeDefaultData ();
			//SetPivotGridItems(null, 0);
			SelectIndex = 0;
		}

		private void InitializeManager ()
		{
			m_PivotGridManager = new PivotGridManager ();
			m_PivotGridItemManager = new PivotGridItemManager ();

			m_PivotGridItemManager.OnNotifyRowExpanderMouseDownEvent += m_PivotGridItemManager_OnNotifyRowExpanderMouseDownEvent;
			m_PivotGridItemManager.OnNotifyMouseDownEvent += m_PivotGridItemManager_OnNotifyMouseDownEvent;
		}

		void m_PivotGridItemManager_OnNotifyMouseDownEvent (object objValue, object MouseEvent)
		{

			int nCount = DataSource.Count;

			if (nCount > 0)
			{
				for (int nIndex = 0; nIndex < nCount; nIndex++)
				{
					PivotGridItemModel Model = DataSource[nIndex];

					if (Model.Equals (objValue))
					{
						SelectIndex = nIndex;
						break;
					}
				}
			}
			else
			{
				SelectIndex = 0;
			}

			if (null != OnNotifyMouseDownEvent)
			{
				OnNotifyMouseDownEvent (objValue, MouseEvent);
			}

		}

		void m_PivotGridItemManager_OnNotifyRowExpanderMouseDownEvent (object objValue)
		{


			if (null != OnNotifyRowExpanderMouseDownEvent)
				OnNotifyRowExpanderMouseDownEvent (objValue);
		}

		private void InitializeHeaderList ()
		{
			m_arHeaderList = new List<string> { LC.GS("TopAtom_PivotGrid_1"),
				LC.GS("TopAtom_PivotGrid_2"),
				LC.GS("TopAtom_PivotGrid_3") };
		}

		private void InitializeDefaultData ()
		{
			//m_RootModel = new PivotGridItemModel();
			//m_RootModel.SubModels = m_PivotGridManager.GenerateDataSource();
			//m_DataSource = m_RootModel.SubModels;
			m_RootModel = new PivotGridItemModel ();
			m_DataSource = new List<PivotGridItemModel> ();
		}


		// Header 셋팅
		public void SetPivotGridItems (CObArray m_oaBrowse, int m_nTreeType, TreeAttrib TreeAttrib)
		{
			RootGridData.Children.Clear ();
			HeaderBorder.Visibility = Visibility.Visible;
			pTreeAttrib = TreeAttrib;

			MakeColumnHeader (m_oaBrowse);

			if (true == TreeAttrib.IsHeaderHidden) // 해더감춤 속성이 설정되어 있는 경우
			{
				HeaderBorder.Visibility = Visibility.Collapsed;
			}
		}


		
		#region BrowseColumnHeaderPanel 구현함수
		// KH 해더 UI변경

		public void MakeColumnHeader (CObArray headerList)
		{
			HeaderGrid.Children.Clear ();
			int nHeaderCount = headerList.Count;
			m_headerCount = nHeaderCount;
			m_arColumnWidth = new double[nHeaderCount];

			for (int i = 0; i < nHeaderCount; i++)
			{
				ColumnDefinition hearderColumnDefinition = new ColumnDefinition ();
				hearderColumnDefinition.Width = GridLength.Auto;
				HeaderGrid.ColumnDefinitions.Add (hearderColumnDefinition);
			}

			for (int i = 0; i < nHeaderCount; i++)
			{
				BrowseItem browseAtom = headerList[i] as BrowseItem;
				if (browseAtom == null)
					continue;

				BrowseColumnHeader browseHeader = new BrowseColumnHeader ();

				browseHeader.OnNotifySeperateLineDrag += OnChangeHeaderSize;
				browseHeader.ShowFilterDlg += BrowseHeader_ShowFilterDlg;

				browseHeader.ColumnHeaderText = browseAtom.Label;
				browseAtom.RealLen = -1 == browseAtom.RealLen ? BrowseContentsPanel.DEFAULT_COLUMN_WIDTH : browseAtom.RealLen;
				HeaderGrid.Tag = browseAtom;

				if (null == headerList[i])
				{
					m_arColumnWidth[i] = 0;
				}
				else
				{
					m_arColumnWidth[i] = browseAtom.RealLen;
				}

				Grid.SetColumn (browseHeader, i);
				HeaderGrid.Children.Add (browseHeader);
			}
		}

		private void OnChangeHeaderSize (object objHeader)
		{
			BrowseColumnHeader header = objHeader as BrowseColumnHeader;
			Point currentPos = Mouse.GetPosition (this);
			Point headerPos = header.TranslatePoint (new Point (0, 0), this);

			int nWidth = (int)(currentPos.X - headerPos.X);

			if (BrowseContentsPanel.DEFAULT_COLUMN_MINUMUN_WIDTH >= nWidth)
			{
				return;
			}

			BrowseItem browseHeader = header.Tag as BrowseItem;

			if (null == browseHeader)
			{
				return;
			}

			if (null != OnNotifyChangedBrowseAtomRealLen)
			{
				OnNotifyChangedBrowseAtomRealLen (browseHeader, nWidth);
			}

			header.Width = nWidth;
			browseHeader.RealLen = nWidth;
		}

		private void BrowseHeader_ShowFilterDlg (object objValue)
		{
			if (objValue is BrowseColumnHeader header)
			{
				int col = Grid.GetColumn (header);

				if (null != ShowFilterDlg)
				{
					ShowFilterDlg (col);
				}
			}
		}


		public PivotGridHeader ConvertBrowseColumnHeaderToPivotGridHeader (BrowseColumnHeader browseHeader)
		{
			if (browseHeader == null)
				return null;

			PivotGridHeader pivotHeader = new PivotGridHeader (new string[1] { browseHeader.ColumnHeaderText });
			var realLenth = browseHeader.GetMeasureString ().Width;

			// ColumnWidth 배열과 헤더 개수 초기화 (1개)
			if(realLenth > 0)
			{
				pivotHeader.ColumnWidth = new double[1] { realLenth };
			}
			else
			{
				pivotHeader.ColumnWidth = new double[1] { 100 };
			}

			pivotHeader.HeaderCount = 1;

			// ColumnDefinition 추가
			pivotHeader.ColumnDefinitionList.Clear ();
			ColumnDefinition colDef = new ColumnDefinition
			{
				Width = GridLength.Auto  // 또는 RealLen 값으로 GridLength 설정해도 됨
			};
			pivotHeader.ColumnDefinitionList.Add (colDef);

			// 헤더 텍스트 블록 추가
			TextBlock headerText = new TextBlock
			{
				Text = browseHeader.ColumnHeaderText,
				HorizontalAlignment = browseHeader.TextAlignment,
				VerticalAlignment = VerticalAlignment.Center
			};

			Grid.SetColumn (headerText, 0);
			pivotHeader.RootGrid.Children.Add (headerText);

			// 스타일 요소 복사
			pivotHeader.Background = browseHeader.Background;
			pivotHeader.BorderBrush = browseHeader.ColumnHeaderBorderColor;
			pivotHeader.BorderThickness = browseHeader.ColumnHeaderBorderThickness;
			pivotHeader.CornerRadius = browseHeader.ColumnHeaderCornerRadius;

			// 필요하면 Tag 도 복사
			pivotHeader.Tag = browseHeader.Tag;

			return pivotHeader;
		}


		#endregion BrowseColumnHeaderPanel 구현함수

		public bool SetRows ()
		{
			// ROW 배치
			if (true == pTreeAttrib.IsShowKeyLabel && 0 == pTreeAttrib.TreeType)
			{
				for (int j = 0; j < KeyLabelList.Count; j++)
				{
					int keynum = int.Parse (KeyLabelList[j].ToString ());
					foreach (PivotGridItemModel tempMode in m_DataSource)
					{
						if (tempMode.ContentsList[j]?.ToString () == pTreeAttrib.RootItemTitle)
						{
							continue;
						}

						tempMode.ContentsList.RemoveAt (j); // 키값 삭제하는 논리
						int Count = tempMode.ContentListCount;
						tempMode.ContentsList.Add (tempMode.ContentsList[Count - 1]);
					}
				}
			}
			else
			{
				for (int i = 0; i < m_DataSource.Count; i++)
				{
					if (HeaderCount < m_DataSource[i].ContentListCount) break;
					m_DataSource[i].ContentsList.Add (m_DataSource[i].ContentsList[HeaderCount - 1]);
				}
			}

			//정렬방식은 CBrowseAtom에서 받아와서 관리한다.
			HorizontalAlignment[] tempAlignment = new HorizontalAlignment[pTreeAttrib.BrowseItemList.Count + 2]; // 수정
			int number = 0;
			foreach (BrowseItem tempAtom in pTreeAttrib.BrowseItemList)
			{
				if (0 == pTreeAttrib.TreeType) //단일체계일경우
				{
					if (true == tempAtom.Display.Contains ("LEVEL") && true == pTreeAttrib.IsShowKeyLabel)
					{
						continue;
					}
				}
				else if (1 == pTreeAttrib.TreeType)
				{
					if (true == tempAtom.IsParent)
					{
						continue;
					}
				}

				switch (tempAtom.AlignType)
				{
					case 0:
						tempAlignment[number] = HorizontalAlignment.Left;
						break;
					case 1:
						tempAlignment[number] = HorizontalAlignment.Center;
						break;
					case 2:
						tempAlignment[number] = HorizontalAlignment.Right;
						break;
				}
				number++;
			}

			m_PivotGridItemManager.AlignmentType = tempAlignment;

			PivotGridItem Item = new PivotGridItem (m_DataSource, m_RootModel, m_arColumnWidth);
			m_PivotGridItemManager.pTreeAttrib = pTreeAttrib;

			m_PivotGridItemManager.setHeaderCount (HeaderCount);
			m_PivotGridItemManager.MakeRows (m_DataSource, m_arColumnWidth, Item);
			RootGridData.Children.Clear ();

			RootGridData.Children.Add (Item);

			//Grid.SetRow(Item, 0); 원래 주석이였음
			if (Item.SubItems.Count > 0)
			{
				SetWidthBinding (Item);
			}
			m_GridItem = Item;
			isRoot = false;
			return true;
		}

		public void SettingAttrib (TreeAttrib pTreeAttrib)
		{
			m_PivotGridItemManager.pTreeAttrib = pTreeAttrib;
		}

		public PivotGridItem GetGridItem ()
		{
			return m_GridItem;
		}

		public void SetColumnWidht ()
		{
			foreach (PivotGridItemModel temp in m_DataSource)
			{
				if (m_arColumnWidth.Length < temp.ContentListCount)
				{
					m_arColumnWidth = new double[temp.ContentListCount];
					for (int i = 0; i < m_arColumnWidth.Length; i++)
					{
						m_arColumnWidth[i] = 100;
					}
				}
			}
		}

		public void SetWidthBinding (PivotGridItem Item)
		{
			bool isChekc = true;
			if (RootGrid.Children.Count < 2)
			{
				isChekc = false;
			}

			if (true == isChekc)
			{
				BrowseColumnHeader browseColumnHeader = HeaderGrid.Children[0] as BrowseColumnHeader;

				PivotGridHeader header = ConvertBrowseColumnHeaderToPivotGridHeader (browseColumnHeader);

				List <PivotGridItem> SortItem = new List<PivotGridItem> ();
				if (0 < RootGridData.Children.Count)
				{
					PivotGridItem item = RootGridData.Children[0] as PivotGridItem;
					SortSubItems (item, SortItem);

					//for (int i = 0; i < header.ColumnDefinitionsCount; i++)
					for (int i = 0; i < header.ColumnDefinitionList.Count; i++)
					{
						//헤더와 첫번째 열 바인딩
						BindingFormation.ObjectWidthBinding (Item.SubItems[0].RowPanelGrid.ColumnDefinitions[i], header.ColumnDefinitionList[i]);

						for (int nSub = 1; nSub < SortItem.Count - 1; nSub++)
						{
							// 데이터 부분 바인딩 ( 순서대로 바인딩 1->2, 2->3, 3->4....)
							BindingFormation.ObjectWidthBinding (SortItem[nSub].RowPanelGrid.ColumnDefinitions[i], SortItem[nSub + 1].RowPanelGrid.ColumnDefinitions[i]);
						}
					}
				}
			}

		}


		/// <summary>
		/// Data List 정리 ( 순서 : 1, 1-1, 1-1-1, 1-2, 2 )
		/// </summary>
		/// <param name="rootItem"></param>
		/// <param name="SortItem"></param>
		private void SortSubItems (PivotGridItem rootItem, List<PivotGridItem> SortItem)
		{
			int nIndex = 0;
			int nCount = rootItem.SubItems.Count;

			SortItem.Add (rootItem);

			if (rootItem.SubItems == null)
				return;

			if (nCount < 1)
				return;

			for (nIndex = 0; nIndex < nCount; nIndex++)
			{
				SortSubItems (rootItem.SubItems[nIndex], SortItem);
			}

			return;

		}

		public bool AddRows ()
		{
			return true;
		}

		private void header_OnColumnSplitterDragDeltaEvent (double[] columnWidthList)
		{
			if (0 >= RootGridData.Children.Count)
			{
				for (int i = 0; i < m_arHeaderList.Count; i++)
				{
					if (true == string.IsNullOrEmpty (m_arHeaderList[i])) continue;
					string temp1 = m_arHeaderList[i].Trim ();
					foreach (BrowseItem tempAtom in pTreeAttrib.BrowseItemList)
					{
						string temp2 = tempAtom.Label.Trim ();
						if (temp1 == temp2)
						{
							tempAtom.ColumnWidth = _Kiss.toInt32 (columnWidthList[i]);
							break;
						}
					}
				}

				return;
			}

			PivotGridItem item = RootGridData.Children[0] as PivotGridItem;
			item.Model.ColumnWidthList = columnWidthList;
			m_PivotGridItemManager.SetColumnDefinitions (item, columnWidthList);

			//columnWidthList 

			for (int i = 0; i < m_arHeaderList.Count; i++)
			{
				if (true == string.IsNullOrEmpty (m_arHeaderList[i])) continue;
				string temp1 = m_arHeaderList[i].Trim ();
				foreach (BrowseItem tempAtom in pTreeAttrib.BrowseItemList)
				{
					//string temp2 = tempAtom.BrowseVar.Trim();
					string temp2 = tempAtom.Label.Trim ();
					if (temp1 == temp2)
					{
						tempAtom.ColumnWidth = _Kiss.toInt32 (columnWidthList[i]);
						break;
					}
				}
			}
			/*
            for (int i = 0; i < pTreeAttrib.BrowseAtom.Count; i++)
            {
                CBrowseAtom tempAtom = pTreeAttrib.BrowseAtom[i] as CBrowseAtom;
                tempAtom.ColumnWidth = _Kiss.toInt32(columnWidthList[i]);
            }*/
		}

		private void RootGrid_PreviewMouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (RootGridData.Children.Count < 1)
				return;

			PivotGridItem item = RootGridData.Children[0] as PivotGridItem;
			m_PivotGridItemManager.ParantItem = item;

		}


		public void ExpanderMakeRows ()
		{
			PivotGridItem currentItem = m_PivotGridItemManager.ExpanderMakeRows (KeyLabelList);
			if (null != currentItem)
			{
				SetWidthBinding (currentItem);
			}
		}

		public void ExpanderMakeRows (List<PivotGridItemModel> dataSource, PivotGridItem currentItem)
		{
			m_PivotGridItemManager.MakeRows (dataSource, currentItem.Model.ColumnWidthList, currentItem);
			SetWidthBinding (currentItem);
		}


		public void DeleteAllColumns ()
		{

		}

		public void DeleteAllRows ()
		{
			if (RootGridData.Children.Count > 0)
			{
				//PivotGridItem item = RootGrid.Children[1] as PivotGridItem;
				//m_PivotGridItemManager.AllItemClear();
				m_DataSource.Clear ();
				RootGridData.Children.Clear ();
			}
		}



	}
}
