using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom.DataGrids;
using Softpower.SmartMaker.TopAtomRun.BrowseHelper;
using Softpower.SmartMaker.TopAtomRun;
using Softpower.SmartMaker.TopControl.Components.Dialog;
using System.Diagnostics;
using System;
using System.Windows.Controls.Primitives;
using Softpower.SmartMaker.TopControl.Components.ListDragDropManager;
using System.Windows.Documents;

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
			this.PreviewMouseMove += OnPreviewMouseMove;
			DataGridControl.OnNotifyBlockSearch += DataGridControl_OnNotifyBlockSearch;
			DataGridControl.OnNotifySelectCell += DataGridControl_OnNotifySelectCell;
			DataGridControl.OnGetBrowseAtomList += GetBrowseAtomList;

			DataGridControl.AddHandler (UserControl.MouseRightButtonDownEvent, new MouseButtonEventHandler (ContentsPanel_OnNotifyRowRightDoubleClickEvent));

			// DataGridControl에서 발생하는 이벤트를 직접 감지
			DataGridControl.AddHandler (PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler (OnPreviewMouseLeftButtonDown), true);
			DataGridControl.AddHandler (PreviewMouseMoveEvent, new MouseEventHandler (OnPreviewMouseMove), true);
			DataGridControl.AddHandler (DropEvent, new DragEventHandler (OnDrop), true);
			DataGridControl.AddHandler (DragOverEvent, new DragEventHandler (OnDragOver), true);
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

				AtomCore.DirectExecute (0x00000001);
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
				DataGridControl.SetDummyCellStyle ();

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

		#region Show/Hide
		// 부모 탐색
		public static T FindParent<T> (DependencyObject child) where T : DependencyObject
		{
			while (child != null)
			{
				if (child is T t)
					return t;

				child = VisualTreeHelper.GetParent (child);
			}
			return null;
		}

		// 자식 탐색
		public static T FindVisualChild<T> (DependencyObject parent) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount (parent); i++)
			{
				var child = VisualTreeHelper.GetChild (parent, i);
				if (child is T t)
					return t;

				var result = FindVisualChild<T> (child);
				if (result != null)
					return result;
			}
			return null;
		}

		private void CDBGridExofAtom_PreviewMouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			
			/*
			// 클릭 위치 가져오기
			Point clickPoint = e.GetPosition (DataGridControl.RootDataGrid);

			// 클릭 위치에 있는 Row 찾기
			var element = e.OriginalSource as DependencyObject;
			var row = FindParent<DataGridRow> (element);

			if (row != null)
			{
				// CellsPresenter 찾기 (row 내부에 있음)
				var presenter = FindVisualChild<DataGridCellsPresenter> (row);
				if (presenter == null)
				{
					// 가상화된 경우 강제로 생성
					DataGridControl.RootDataGrid.ScrollIntoView (row, DataGridControl.RootDataGrid.Columns[0]);
					presenter = FindVisualChild<DataGridCellsPresenter> (row);
				}

				// 클릭된 셀의 위치 계산
				for (int i = 0; i < DataGridControl.RootDataGrid.Columns.Count; i++)
				{
					var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex (i);
					if (cell != null)
					{
						Point cellOrigin = cell.TranslatePoint (new Point (0, 0), DataGridControl.RootDataGrid);
						Rect cellRect = new Rect (cellOrigin, cell.RenderSize);
						if (cellRect.Contains (clickPoint))
						{
							// 해당 셀 찾았음
							var column = DataGridControl.RootDataGrid.Columns[i];
							if (column is DataGridTextColumn textColumn)
							{
								MessageBox.Show ($"클릭한 컬럼 인덱스: {i}, 헤더: {textColumn.Header}");
							}
							break;
						}
					}
				}
			}
			*/
		}

		void ContentsPanel_OnNotifyRowRightDoubleClickEvent (object sender, MouseButtonEventArgs e)
		{
			if (1 == AtomCore.AtomRunMode && 2 == e.ClickCount)
			{
				BrowseAttrib atomAttrib = AtomCore.GetAttrib () as BrowseAttrib;

				TopDoc pDoc = AtomCore.Information.GetDocument () as TopDoc;
				string strFileName = pDoc.GetFileName ();
				if (string.IsNullOrEmpty (strFileName))
				{
					ToastMessge.Show ("파일 저장 후 실행해야합니다.");
					return;
				}

				BrowseColumnChangeWindow dlg = new BrowseColumnChangeWindow ();
				dlg.SetInitValue (atomAttrib.BrowseItemList);

				string layout = "ALL";

				BrowseColumnVariant browseColumnVariant = BrowseColumnHelper.Instance.GetBrowseColumnVariant (strFileName, atomAttrib.AtomProperVar);

				if (null != browseColumnVariant)
				{
					layout = browseColumnVariant.CurrentLayout;
				}

				dlg.SetInitValue (strFileName, atomAttrib.AtomProperVar, layout, atomAttrib.BrowseItemList);

				if (true == dlg.ShowDialog ())
				{
					bool bIsChecked;

					for (int nIndex = 0; nIndex < atomAttrib.BrowseItemList.Count; nIndex++)
					{
						bIsChecked = dlg.IsCheckedColumn (nIndex);
						((BrowseItem)atomAttrib.BrowseItemList[nIndex]).DisplayOrHide = true == bIsChecked ? 0 : 1;
					}
				}
				SetColumnVisibility ();
			}
		}

		public void SetColumnVisibility ()
		{
			var atomCore = this.AtomCore as DBGridExAtom;
			var atomAttrib = this.AtomCore.GetAttrib () as DBGridExAttrib;

			for (int i = 0; i < atomAttrib.BrowseItemList.Count; i++)
			{
				DataGridControl.SetColumnVisibility (i, ((BrowseItem)atomAttrib.BrowseItemList[i]).DisplayOrHide);
			}
		}

		private static CObArray m_OriginalBrowseItemList = new CObArray ();

		public CObArray OriginalBrowseItemList
		{
			get { return m_OriginalBrowseItemList; }
		}

		private CObArray DeepCopy (CObArray source)
		{
			CObArray copy = new CObArray ();
			foreach (BrowseItem item in source)
			{
				BrowseItem newItem = new BrowseItem ();
				newItem.AssignCopy (item);
				copy.Add (newItem);
			}
			return copy;
		}

		#endregion

		#region Drag&Drop
		private DataGridColumnHeader _draggingColumnHeader;
		private DataGridColumn _draggingColumn = null;

		private Point _dragStartPoint;
		private Point _dragOffset; // 마우스 클릭 시의 오프셋

		private DragAdorner _adorner;
		private AdornerLayer _adornerLayer;

		private void OnPreviewMouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			DependencyObject source = e.OriginalSource as DependencyObject;
			if (source == null) return;

			// 부모에서 DataGridColumnHeader 찾기
			DataGridColumnHeader header = FindParent<DataGridColumnHeader> (source);

			// 자식에서도 찾아보기
			if (header == null)
				header = FindVisualChild<DataGridColumnHeader> (source);

			// 컬럼 헤더 드래그 시작
			if (header != null)
			{
				OnHeaderDragStart (header);
			}
		}

		private void OnDrop (object sender, DragEventArgs e)
		{
			// 컬럼 드롭 처리
			if (e.Data.GetData (typeof (DataGridColumn)) is DataGridColumn column)
			{
				HandleColumnDrop (column);
			}
		}

		private void HandleColumnDrop (DataGridColumn column)
		{
			// 컬럼 위치 변경 로직
			MessageBox.Show ($"컬럼 '{column.Header}' 드롭 완료!");
		}

		private void OnDragOver (object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.Move;
			e.Handled = true;
		}

		private void OnHeaderDragStart (DataGridColumnHeader header)
		{
			if (header == null)
				return;

			var size = new Size (header.ActualWidth, header.ActualHeight);
			var brush = new VisualBrush (header) { Opacity = 0.5 };

			// 어도너 추가
			_adornerLayer = AdornerLayer.GetAdornerLayer (this);
			if (_adornerLayer == null)
				return;

			_adorner = new DragAdorner (this, size, brush);
			_adornerLayer.Add (_adorner);

			Mouse.Capture (this);

			DragDrop.DoDragDrop (header, header.Column, DragDropEffects.Move);

			RemoveAdorner ();
		}

		private void RemoveAdorner ()
		{
			if (_adorner != null && _adornerLayer != null)
			{
				_adornerLayer.Remove (_adorner);
				_adorner = null;
				_adornerLayer = null;
			}
		}

		private void OnPreviewMouseMove (object sender, MouseEventArgs e)
		{
			if (_adorner != null)
			{
				var position = e.GetPosition (this);
				var position1 = e.GetPosition (this);
				var position2 = e.GetPosition (_adorner);
				var position3 = e.GetPosition (Application.Current.MainWindow);

				_adorner.SetOffsets (position.X, position.Y);

				double dLeft = position.X;
				double dTop = position.Y;

				_adorner.Margin = new Thickness (dLeft, 0, 0, 0);

				Debug.WriteLine ($"[MouseMove] _adorner != null | X: {position.X}, Y: {position.Y}");
				Debug.WriteLine ($"[MouseMove] _adorner != null | X: {position1.X}, Y: {position1.Y}");
				Debug.WriteLine ($"[MouseMove] _adorner != null | X: {position2.X}, Y: {position2.Y}");
				Debug.WriteLine ($"[MouseMove] _adorner != null | X: {position3.X}, Y: {position3.Y}");


			}
			else
			{
				Debug.WriteLine ("[MouseMove] _adorner is NULL");
			}
		}

		private void DataGridColumnHeader_DragOver (object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.Move;
			e.Handled = true;
		}

		#endregion
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
