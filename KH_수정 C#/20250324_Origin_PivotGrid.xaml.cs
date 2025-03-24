using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
			BrowseItem pBrowseAtom;
			RootGridData.Children.Clear ();

			List<string> arField = new List<string> ();

			for (int i = 0; i < m_oaBrowse.Count; i++)
			{
				arField.Add ("");
			}

			for (int _i = 0; _i < m_oaBrowse.Count; _i++)
			{
				pBrowseAtom = (BrowseItem)m_oaBrowse[_i];

				string sTable = pBrowseAtom.Table.Trim ();
				string sField = pBrowseAtom.Field;
				string sLabel = pBrowseAtom.Label;
				int nLen = pBrowseAtom.Len;
				int nDisplayOrHide = pBrowseAtom.DisplayOrHide;

				if (nLen <= 0)
					nLen = 10;

				//arField[_i] = sField;
				arField[_i] = sLabel;
			}

			// 헤더가 하나도 없으면 리턴.
			if (arField.Count <= 0)
			{
				//RootGridHeader.Children.Clear();
				return;
			}

			if (true == TreeAttrib.IsShowKeyLabel) //키값을 표시하지 않을경우 
			{
				KeyLabelList = new ArrayList ();
				m_arHeaderList = new List<string> (new string[arField.Count]);
				int number = 0;
				for (int i = 0; i < m_oaBrowse.Count; i++)
				{
					BrowseItem tempAtom = m_oaBrowse[i] as BrowseItem;
					if (true == string.IsNullOrEmpty (tempAtom.Display) || false == tempAtom.Display.Contains ("LEVEL"))
					{
						m_arHeaderList[number] = arField[i];
						number++;
					}
					else
					{
						KeyLabelList.Add (i);
					}
				}
				/*
                for (int i = number; i < arField.Length; i++)
                {
                    m_arHeaderList[i] = "";
                }
                */
			}
			else
			{
				m_arHeaderList = arField;
			}

			//상하위 트리일경우 해더의 마지막 값에 null값이 없어서 여기서 강제로 추가해줌 논리 보강시 변경 필요
			if (false == string.IsNullOrEmpty (m_arHeaderList[m_arHeaderList.Count - 1]))
			{
				int number = m_arHeaderList.Count + 1;
				List<string> tempstr = new List<string> ();

				for (int i = 0; i < m_arHeaderList.Count; i++)
				{
					if (!(i < tempstr.Count))
						tempstr.Add (string.Empty);

					tempstr[i] = m_arHeaderList[i];
				}

				m_arHeaderList = tempstr;
			}

			HeaderGrid.Children.Clear ();

			pTreeAttrib = TreeAttrib;
			m_arColumnWidth = new double[m_arHeaderList.Count];

			PivotGridHeader header = new PivotGridHeader (m_arHeaderList.ToArray ());

			/*
			 for (int i = 0; i < TreeAttrib.BrowseAtom.Count; i++)
			 {
				 CBrowseAtom tempAtom = TreeAttrib.BrowseAtom[i] as CBrowseAtom;
				 m_arColumnWidth[i] = tempAtom.ColumnWidth;
			 }
			 */

			for (int i = 0; i < m_arHeaderList.Count; i++)
			{
				if (true == string.IsNullOrEmpty (m_arHeaderList[i])) continue;
				string temp1 = m_arHeaderList[i].Trim ();
				foreach (BrowseItem tempAtom in TreeAttrib.BrowseItemList)
				{
					//string temp2 = tempAtom.BrowseVar.Trim();
					string temp2 = tempAtom.Label.Trim ();

					if (temp1 == temp2)
					{
						if (0 >= tempAtom.ColumnWidth)
						{
							tempAtom.ColumnWidth = 100;
						}
						m_arColumnWidth[i] = tempAtom.ColumnWidth;
						break;
					}
				}
			}

			header.ColumnWidth = m_arColumnWidth;
			header.MakeHeader ();

			m_headerCount = header.HeaderCount;

			header.OnColumnSplitterDragDeltaEvent += header_OnColumnSplitterDragDeltaEvent;
			HeaderGrid.Children.Add (header);

			//기본 크기는 30으로 고정한후에 행높이 속성에 따라서 Height값을 조정한다.
			HeaderGrid.Height = 30;
			int headerAddSize = TreeAttrib.HeaderGap;
			if (0 < headerAddSize)
			{
				HeaderGrid.Height += (HeaderGrid.Height / 100) * headerAddSize;
			}

			if (true == TreeAttrib.IsHeaderHidden) // 해더감춤 속성이 설정되어 있는 경우
			{
				header.Visibility = Visibility.Collapsed;
				Grid.SetRow (PivotScrollViewer, 0);
				Grid.SetRowSpan (PivotScrollViewer, 2);
				PivotScrollViewer.Height = TreeAttrib.AtomHeight;
			}
			else
			{
				header.Visibility = Visibility.Visible;
				Grid.SetRow (PivotScrollViewer, 1);
				PivotScrollViewer.Height = TreeAttrib.AtomHeight - HeaderGrid.Height;
			}

			BrowseItem temp = TreeAttrib.BrowseItemList[0] as BrowseItem;
		}

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
				PivotGridHeader header = HeaderGrid.Children[0] as PivotGridHeader;

				List<PivotGridItem> SortItem = new List<PivotGridItem> ();
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
