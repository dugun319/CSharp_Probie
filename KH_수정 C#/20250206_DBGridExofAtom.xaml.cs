using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom.DataGrids;

namespace Softpower.SmartMaker.TopAtom
{
	/// <summary>
	/// Interaction logic for DBGridExofAtom.xaml
	/// </summary>
	public partial class CDBGridExofAtom : DBGridExAtomBase
	{
		public CDBGridExofAtom ()
		{
			InitializeComponent ();

			InitEvent ();
		}

		public CDBGridExofAtom (Atom atomCore)
			: base (atomCore)
		{
			InitializeComponent ();

			InitEvent ();
		}

		private void InitStyle ()
		{
			this.Background = Brushes.Transparent;
		}

		private void InitEvent ()
		{
			this.PreviewMouseLeftButtonDown += CDBGridExofAtom_PreviewMouseLeftButtonDown;
			DataGridControl.OnNotifyBlockSearch += DataGridControl_OnNotifyBlockSearch;
			DataGridControl.OnNotifySelectCell += DataGridControl_OnNotifySelectCell;
			DataGridControl.OnGetBrowseAtomList += GetBrowseAtomList;
		}

		private void DataGridControl_OnNotifyBlockSearch ()
		{
			if (RUNMODE_TYPE.PLAY_MODE == this.AtomCore.AtomRunMode)
			{
				DBGridExAtom atomCore = this.AtomCore as DBGridExAtom;

				if (null != atomCore)
				{
					if (-1 != atomCore.ProcessEvent (0, EVS_TYPE.EVS_A_MOVE_LASTROW))
					{
						if (0 <= MsgHandler.CALL_MSG_HANDLER (atomCore, EVS_TYPE.EVS_A_MOVE_LASTROW, null))
						{
							atomCore.ExecuteBlockSearch ();

							atomCore.ProcessEvent (1, EVS_TYPE.EVS_A_MOVE_LASTROW);
						}
					}
				}
			}
		}

		private void DataGridControl_OnNotifySelectCell (int index)
		{
			DBGridExAtom atomCore = this.AtomCore as DBGridExAtom;

			if (null != atomCore)
			{
				CStringArray strSelectedLoadFieldValues = GetSelectedLoadFieldRowValue (index);//참조정보키가 켜져있는 값
				CStringArray strLoadFieldColumnNames = atomCore.GetLoadFieldBrowseAtomColumnName ();//참조정보키가 켜져있는 항목명
				CStringArray strLoadFieldTableAndColumnNames = atomCore.GetLoadFieldBrowseAtomTableColumnName ();

				atomCore.ReDBIO (ref strLoadFieldTableAndColumnNames, strLoadFieldColumnNames, strSelectedLoadFieldValues, true);
			}
		}

		private void CDBGridExofAtom_PreviewMouseLeftButtonDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{

		}

		private CStringArray GetSelectedLoadFieldRowValue (int index)
		{
			CStringArray array = new CStringArray ();

			DBGridExAtom atomCore = this.AtomCore as DBGridExAtom;
			DBGridExAttrib atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;

			DataTable dataTable = atomAttrib.DataSet.Tables[0];

			List<int> loadFieldIndex = atomAttrib.BrowseItemList.Cast<BrowseItem> ().Where (item => item.IsLoadField).Select (item => atomAttrib.BrowseItemList.IndexOf (item)).ToList ();

			DataRow row = dataTable.Rows[index];

			foreach (int columnIndex in loadFieldIndex)
			{
				array.Add (row[columnIndex].ToString ());
			}

			return array;
		}

		#region | Protected override |

		protected override void InitializeAtomCore ()
		{
			m_AtomCore = new DBGridExAtom ();
		}

		protected override void InitializeResizeAdorner ()
		{
			m_ResizeAdorner = new Point8Adorner (this);
		}

		protected override void InitializeAtomSize ()
		{
			Size atomSize = DefaultAtomSizeManager.GetDefaultRect (AtomType.DataGrid);
			this.Width = atomSize.Width;
			this.Height = atomSize.Height;
		}

		#endregion

		public override void SetAtomBackground (Brush applyBrush)
		{
			DataGridControl.SetAtomBackground (applyBrush);
		}

		public override Brush GetAtomBackground ()
		{
			return DataGridControl.GetAtomBackground ();
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

		public override void CloneAtom (AtomBase ClonedAtom, bool bDeepCopy)
		{
			ClonedAtom.SetAtomBackground (this.GetAtomBackground ());
			ClonedAtom.SetAtomBorder (this.GetAtomBorder ());
			ClonedAtom.SetAtomDashArray (this.GetAtomDashArray ());
			ClonedAtom.SetAtomThickness (this.GetAtomThickness ());
			ClonedAtom.SetTextUnderLine (this.GetTextUnderLine ());

			base.CloneAtom (ClonedAtom, bDeepCopy);
		}

		public override void SerializeLoadSync_AttribToAtom (bool bIs80Model)
		{
			base.SerializeLoadSync_AttribToAtom (bIs80Model);

			DBGridExAttrib atomAttrib = null == AtomCore ? null : AtomCore.GetAttrib () as DBGridExAttrib;
			if (null == atomAttrib)
			{
				return;
			}

			this.CompletePropertyChanged ();
		}

		public override void Sync_AttribToAtom ()
		{
			base.Sync_AttribToAtom ();

			DBGridExAttrib atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;

			if (null != atomAttrib)
			{
				DataGridControl.FrozenColumnCount = atomAttrib.FixedCol;
				DataGridControl.FrozenRowCount = atomAttrib.FixedRow;
				DataGridControl.RootDataGrid.IsReadOnly = atomAttrib.IsReadOnly;
				DataGridControl.RootDataGrid.RowHeight = atomAttrib.RowHeight;
				DataGridControl.FilterEnable = 1 == this.AtomCore.AtomRunMode ? atomAttrib.IsFilterEnable : false;

				DataGridControl.SetFrozenColumnCount ();
				SetSelectedType (atomAttrib.SelectType); // 0:셀단위 1:행단위 2:열단위
				DataGridControl.SetOpacity (atomAttrib.Opacity);
				SurfaceCanvas.Margin = new Thickness (0, atomAttrib.RowHeight, 0, 0);
			}
		}

		public override void ReleaseRunModeProperty ()
		{
			base.ReleaseRunModeProperty ();
		}

		public override void ApplyRunModeProperty ()
		{
			base.ApplyRunModeProperty ();
		}

		public override void CompletePropertyChanged ()
		{
			//보강 필요
			DBGridExAttrib atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;

			DataGridControl.FrozenColumnCount = atomAttrib.FixedCol;
			DataGridControl.FrozenRowCount = atomAttrib.FixedRow;
			DataGridControl.RootDataGrid.IsReadOnly = atomAttrib.IsReadOnly;
			DataGridControl.RootDataGrid.RowHeight = atomAttrib.RowHeight;
			DataGridControl.FilterEnable = 1 == this.AtomCore.AtomRunMode ? atomAttrib.IsFilterEnable : false;

			DataGridControl.SetFrozenColumnCount ();
			SetSelectedType (atomAttrib.SelectType); // 0:셀단위 1:행단위 2:열단위 3:읽기전용
			DataGridControl.SetOpacity (atomAttrib.Opacity);
			SurfaceCanvas.Margin = new Thickness (0, atomAttrib.RowHeight, 0, 0);

			InsertDummyData ();
		}

		public void SetSelectedType (int nSelectedType)
		{
			DataGridControl.RootDataGrid.IsReadOnly = false;

			switch (nSelectedType)
			{
				case 0:
					DataGridControl.RootDataGrid.SelectionUnit = DataGridSelectionUnit.Cell; //셀
					break;
				case 1:
					DataGridControl.RootDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow; //행
					break;
				case 2:
					DataGridControl.RootDataGrid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader; //열단위?
					break;
			}
		}

		public override void ChangeAtomMode (int nRunMode)
		{
			base.ChangeAtomMode (nRunMode);

			var atomCore = this.AtomCore as DBGridExAtom;
			var atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;

			if (RUNMODE_TYPE.TOOL_MODE == nRunMode)
			{
				this.IsEnabledScroll = false;
				//this.GridAtomImage.Visibility = System.Windows.Visibility.Visible;
				DataGridControl.FilterEnable = false;
				this.SurfaceCanvas.Visibility = Visibility.Visible;

				// 컬럼 너비 저장
				for (int i = 0; i < atomAttrib.BrowseItemList.Count; i++)
				{
					BrowseItem browseAtom = atomAttrib.BrowseItemList.GetAt (i) as BrowseItem;

					if (null != browseAtom)
					{
						browseAtom.ColumnWidth = DataGridControl.GetColumnWidth (i);
					}
				}

				InsertDummyData ();
			}
			else
			{
				//실행모드 전환시 미리보기 데이터 삭제
				if (0 < atomAttrib.DataSet.Tables.Count)
				{
					DataTable dataTable = atomAttrib.DataSet.Tables[0];
					dataTable.Rows.Clear ();
					DataGridControl.SetData (dataTable, atomAttrib.BrowseItemList, atomCore);
				}

				this.IsEnabledScroll = true;
				//this.GridAtomImage.Visibility = System.Windows.Visibility.Collapsed;
				DataGridControl.FilterEnable = atomAttrib.IsFilterEnable;
				this.SurfaceCanvas.Visibility = Visibility.Collapsed;
			}
		}

		public override void OnFirstMakeSync (bool bFalse)
		{
			DBGridExAttrib atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;

			if (null != atomAttrib)
			{
				SolidColorBrush SolidBrush = GetAtomBackground () as SolidColorBrush;
				if (null != SolidBrush)
				{
					atomAttrib.BackColor = SolidBrush.Color;
				}
			}

			double dFirstFontSize = PQAppBase.DefaultFontSize;
			SetAtomFontSize (dFirstFontSize);
			AtomCore.GetAttrib ().SetAtomFontSize (dFirstFontSize);

			InsertDummyData ();
		}

		public CObArray GetBrowseAtomList ()
		{
			DBGridExAttrib atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;
			if (null != atomAttrib && null != atomAttrib.BrowseItemList && RUNMODE_TYPE.PLAY_MODE == this.AtomCore.AtomRunMode)
			{
				return atomAttrib.BrowseItemList;
			}
			return null;
		}

		public void SetData (DataTable dataTable)
		{
			var atomCore = this.AtomCore as DBGridExAtom;
			var atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;

			if (null != atomAttrib)
			{
				DataGridControl.SetData (dataTable, atomAttrib.BrowseItemList, atomCore);
			}

			// 컬럼 너비 복원
			//CDBGridExAttrib atomAttrib = null == AtomCore ? null : AtomCore.GetAttrib () as CDBGridExAttrib;
			//if (null != atomAttrib)
			//{
			//    CBrowseAtom browseAtom = atomAttrib.BrowseAtom.GetAt (0) as CBrowseAtom;
			//    if (null != browseAtom)
			//    {
			//        GridWindowEditor.HeaderColumnDefinition.Width = new GridLength (browseAtom.ColumnWidth);

			//        int nIndex = 0;
			//        foreach (DataGridColumn col in GridWindowEditor.UserDataGridMain.GetDataGrid.Columns)
			//        {
			//            browseAtom = atomAttrib.BrowseAtom.GetAt (nIndex) as CBrowseAtom;
			//            if (null != browseAtom)
			//            {
			//                col.Width = browseAtom.ColumnWidth;
			//            }

			//            nIndex++;
			//        }

			//        nIndex = 1;
			//        foreach (ColumnDefinition col in GridWindowEditor.MainHeaderControlColumn.ColumnDefinitions)
			//        {
			//            browseAtom = atomAttrib.BrowseAtom.GetAt (nIndex) as CBrowseAtom;
			//            if (null != browseAtom)
			//            {
			//                col.Width = new GridLength (browseAtom.ColumnWidth);
			//            }

			//            nIndex++;
			//        }
			//    }
			//}
			//
		}

		public void InsertDummyData ()
		{
			var atomCore = this.AtomCore as DBGridExAtom;
			var atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;

			if (null != atomAttrib)
			{
				int nColumn = atomAttrib.FixedCol;

				atomAttrib.InitDataSet ();

				DataTable dataTable = atomAttrib.DataSet.Tables[0];

				int nMaxCount = dataTable.Columns.Count;

				if (0 == nMaxCount)
				{
					if (0 < atomAttrib.BrowseItemList.Count)
					{
						foreach (BrowseItem browse in atomAttrib.BrowseItemList)
						{
							if (false == dataTable.Columns.Contains (browse.Label))
								dataTable.Columns.Add (browse.Label);
						}

						//nMaxCount = atomAttrib.BrowseAtom.Count;
						nMaxCount = dataTable.Columns.Count;
					}
					else
					{
						for (int i = 0; i < 5; i++)
						{
							dataTable.Columns.Add (LC.GS ("TopAtom_DBGridExofAtom_1808_1") + i); //미리보기
						}
						nMaxCount = 5;
					}
				}

				for (int i = 0; i < 5; i++)
				{
					object[] objArray = new object[nMaxCount];

					for (int j = 0; j < objArray.Length; j++)
					{
						objArray[j] = LC.GS ("TopAtom_DBGridExofAtom_1808_1");
					}

					dataTable.Rows.Add (objArray);
				}

				DataGridControl.SetData (dataTable, atomAttrib.BrowseItemList, atomCore);
				DataGridControl.SetDummyCellStyle();

                //if (0 < atomAttrib.BrowseAtom.Count)
                //{
                //    for (int i = 0; i < atomAttrib.BrowseAtom.Count; i++)
                //    {
                //        CBrowseAtom browse = atomAttrib.BrowseAtom.GetAt (i) as CBrowseAtom;
                //        dataTable.Columns.Add (browse.Label);
                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < 5; i++)
                //    {
                //        dataTable.Columns.Add ("미리보기" + i);
                //    }

                //}

                //for (int i = 0; i < 5; i++)
                //{
                //    dataTable.Rows.Add (LC.GS ("TopAtom_DBGridExofAtom_1808_1"));
                //}
            }


			//atomAttrib.InitDataSet ();

			//if (atomAttrib.DirectExec)
			//{
			//    if (bRunMode)
			//        return;
			//}

			//if (atomAttrib.DataSet.Tables.Count > 0)
			//{
			//    // 2015-05-06 JAEYOUNG DB필드명이 없는 컬럼 추가
			//    DataTable dtData = new DataTable ();
			//    //DataTable dtData = atomAttrib.DataSet.Tables[0].Clone();
			//    if (atomAttrib.BrowseAtom.Count > 0)
			//    {
			//        foreach (CBrowseAtom pBrowseAtom in atomAttrib.BrowseAtom)
			//        {
			//            dtData.Columns.Add (pBrowseAtom.Label);
			//        }
			//    }

			//    if (dtData.Columns.Count > 0)
			//    {
			//        if (bRunMode)
			//        {
			//            dtData.Rows.Add (" ");
			//        }
			//        else
			//        {
			//            for (int i = 1; i < 10; i++)
			//            {
			//                dtData.Rows.Add (LC.GS ("TopAtom_DBGridExofAtom_1808_1"));
			//            }
			//        }

			//        SetData (dtData);
			//        GridDataControl.Visibility = System.Windows.Visibility.Visible;
			//    }
			//    else
			//    {
			//        if (!bRunMode)
			//        {
			//            GridDataControl.Visibility = System.Windows.Visibility.Hidden;
			//        }
			//    }
			//}
		}

		public override void NotifyChangedValueByInnerLogic (object pObject)
		{
			ArrayList alParams = pObject as ArrayList;

			if (null == alParams || 3 > alParams.Count)
			{
				return;
			}

			bool bAppend = (bool)alParams[0];
			int nRowCount = (int)alParams[1];
			ArrayList alRows = (ArrayList)alParams[2];

			//CBrowse atomCore = this.AtomCore as CBrowse;
			//CBrowseAttrib atomAttrib = atomCore.GetAttrib() as CBrowseAttrib;

			DBGridExAttrib atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;
			//atomAttrib.InitDataSet();

			if (0 < atomAttrib.DataSet.Tables.Count)
			{
				// 2015-05-06 JAEYOUNG DB필드명이 없는 컬럼 추가
				DataTable dtBuffer = new DataTable ();
				//DataTable dtData = atomAttrib.DataSet.Tables[0].Clone();
				if (atomAttrib.BrowseItemList.Count > 0)
				{
					foreach (BrowseItem pBrowseAtom in atomAttrib.BrowseItemList)
					{
						dtBuffer.Columns.Add (pBrowseAtom.Label);
					}
				}

				foreach (CStringArray strArray in alRows)
				{
					int nIdx = 0;

					object[] obj = new object[strArray.Count];

					foreach (string strData in strArray)
					{
						obj[nIdx] = strData;
						nIdx++;
					}

					dtBuffer.Rows.Add (obj);
				}
				SetData (dtBuffer);
			}
		}

		public void SetRowHeight ()
		{
			DBGridExAttrib atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;

			if (null != DataGridControl)
			{
				DataGridControl.SetRowHeight (atomAttrib.RowHeight);
			}

			SurfaceCanvas.Margin = new Thickness (0, atomAttrib.RowHeight, 0, 0);
		}

		public void SetCellBackColor (int nRow, int nColumn, Brush applyBrush)
		{
			if (null != DataGridControl)
			{
				DataGridControl.SetCellBackColor (nRow, nColumn, applyBrush);
			}
		}

		public void SetCellFontColor (int nRow, int nColumn, Brush applyBrush)
		{
			if (null != DataGridControl)
			{
				DataGridControl.SetCellFontColor (nRow, nColumn, applyBrush);
			}
		}

		public void SetCellFontSize (int nRow, int nColumn, double dFontSize)
		{
			if (null != DataGridControl)
			{
				DataGridControl.SetCellFontSize (nRow, nColumn, dFontSize);
			}
		}

		public void SetCellFontBold (int nRow, int nColumn, bool bBold)
		{
			if (null != DataGridControl)
			{
				DataGridControl.SetCellFontBold (nRow, nColumn, bBold);
			}
		}

		public void SetCellFontItalic (int nRow, int nColumn, bool bItalic)
		{
			if (null != DataGridControl)
			{
				DataGridControl.SetCellFontItalic (nRow, nColumn, bItalic);
			}
		}

		public void SetCellStrikethrough (int nRow, int nColumn, bool bStrikethrough)
		{
			if (null != DataGridControl)
			{
				DataGridControl.SetCellStrikethrough (nRow, nColumn, bStrikethrough);
			}
		}

		public void SetCellUnderLine (int nRow, int nColumn, bool bUnderLine)
		{
			if (null != DataGridControl)
			{
				DataGridControl.SetCellUnderLine (nRow, nColumn, bUnderLine);
			}
		}

		public void SetCellFontFamily (int nRow, int nColumn, string strFontName)
		{
			if (null != DataGridControl)
			{
				DataGridControl.SetCellFontFamily (nRow, nColumn, strFontName);
			}
		}

	}
}



//public void CreateGridWindow()
//{
//    if (null == this.AtomCore || null == this.AtomCore.GetAttrib()) { return; }

//    CDBGridExAttrib atomAttrib = this.AtomCore.GetAttrib() as CDBGridExAttrib;
//    if (null != atomAttrib)
//    {
//        (AtomCore as CDBGridEx).CreateGridWindow ();
//        GridWindowEditor.AllClearInitialize ();
//    }
//}

//public int SelectedColumnIndex()
//{
//    return GridWindowEditor.UserDataGridMain.SelectedColumnIndex;
//}

//public int SelectedRowIndex()
//{
//    return GridWindowEditor.UserDataGridMain.SelectedRowIndex;
//}




// Row,Column 에 대한 Cell Return
//public DataGridCell GetCell(int row, int column)
//{
//    //DataGridRow rowContainer = (DataGridRow)this.GridWindowEditor.UserDataGridMain.DataGridMain.ItemContainerGenerator.ContainerFromIndex(row);
//    ////DataGridRow rowContainer = GetRow(row);

//    //if (rowContainer != null)
//    //{
//    //    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

//    //    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
//    //    if (cell == null)
//    //    {
//    //        this.GridWindowEditor.UserDataGridMain.DataGridMain.ScrollIntoView(rowContainer, this.GridWindowEditor.UserDataGridMain.DataGridMain.Columns[column]);
//    //        cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
//    //    }
//    //    return cell;
//    //}
//    return null;
//}

//void DataGridMain_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
//{
//    //CDBGridExAttrib atomAttrib = this.AtomCore.GetAttrib() as CDBGridExAttrib;

//    //if (atomAttrib.SelectType == 2)
//    //{
//    //    int nCol = SelectedColumnIndex();

//    //    for (int i = 0; i < this.GridWindowEditor.UserDataGridMain.DataGridMain.Items.Count; i++)
//    //    {
//    //        DataGridCell cell = GetCell(i, nCol);
//    //        DataGridCellInfo cellInfo = new DataGridCellInfo(cell);
//    //        if (this.GridWindowEditor.UserDataGridMain.DataGridMain.SelectedCells.Contains(cellInfo) == false)
//    //            this.GridWindowEditor.UserDataGridMain.DataGridMain.SelectedCells.Add(cellInfo);
//    //    }
//    //}
//}


//void GridWindowEditor_SizeChangeScrollviewerEvent()
//{
//    GridWindowEditor_SizeChanged(null, null);
//}

//private void GridWindowEditor_SizeChanged(object sender, SizeChangedEventArgs e)
//{
//    //TextBlockScrollViewerVertical.Height = GridWindowEditor.AllSizeHeight;
//    //TextBlockScrollViewerHorizontal.Width = GridWindowEditor.AllSizeWidth;
//}

//private void ScrollViewerVertical_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
//{
//    GridWindowEditor.SetScrollViewerVerticalDataGrid((sender as ScrollViewer).VerticalOffset);

//    if (false == this.IsEnabledScroll)
//    {
//        ScrollViewer pScrollViewer = sender as ScrollViewer;
//        if (null != pScrollViewer)
//        {
//            if (pScrollViewer.ActualHeight < pScrollViewer.ExtentHeight)
//            {
//                this.IsEnabledScroll = true;
//            }
//        }
//    }
//}

//private void ScrollViewerHorizontal_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
//{
//    GridWindowEditor.SetScrollViewerHorizontalDataGrid((sender as ScrollViewer).HorizontalOffset);
//}

//private void SetRowHeight()
//{
//    CDBGridExAttrib atomAttrib = this.AtomCore.GetAttrib() as CDBGridExAttrib;
//    if (null != atomAttrib)
//    {
//        GridWindowEditor.RowHeight = atomAttrib.RowHeight;
//    }
//}

//private void SetDataGridSelectionUnit()
//{
//    //CDBGridExAttrib atomAttrib = this.AtomCore.GetAttrib() as CDBGridExAttrib;
//    //if (null != atomAttrib)
//    //{
//    //    // 0:셀단위 1:행단위 2:열단위 3:읽기전용
//    //    if (atomAttrib.SelectType == 0)
//    //        GridWindowEditor.UserDataGridMain.SelectionUnit = DataGridSelectionUnit.Cell;
//    //    else if (atomAttrib.SelectType == 1)
//    //        GridWindowEditor.UserDataGridMain.SelectionUnit = DataGridSelectionUnit.FullRow;
//    //    else if (atomAttrib.SelectType == 2)
//    //        GridWindowEditor.UserDataGridMain.SelectionUnit = DataGridSelectionUnit.Cell;
//    //}
//}

// DataGrid 가 셀 색상은 변경이 가능하지만, 투명도는 지원을 안함.
// https://social.msdn.microsoft.com/Forums/en-US/ac0bfbed-d568-4c5e-9c4e-7d5e15970824/transparent-datagrid?forum=csharplanguage
//private void SetOpacity()
//{
//    //CDBGridExAttrib atomAttrib = this.AtomCore.GetAttrib() as CDBGridExAttrib;
//    //if (atomAttrib != null)
//    //{
//    //    int nOpacity = atomAttrib.Opacity;

//    //    foreach (DataGridColumn col in GridWindowEditor.UserDataGridMain.DataGridMain.Columns)
//    //    {
//    //        if (col != null)
//    //        {                        
//    //            Style style = new Style(typeof(DataGridCell));
//    //            style.Setters.Add(new Setter(DataGridCell.BackgroundProperty,
//    //                 new SolidColorBrush(Color.FromArgb((byte)(nOpacity * 2.55), 255, 255, 255))));

//    //            col.CellStyle = style;
//    //        }
//    //    }
//    //}
//}
