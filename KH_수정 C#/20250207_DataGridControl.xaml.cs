
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
//using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopControlRun.SmartDateTimePicker;

namespace Softpower.SmartMaker.TopAtom.DataGrids
{
    #region | Converter |

    public class CheckBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string content)
            {
                string CheckFormat = parameter as string;
                bool bResult = false;

                CheckFormat = CheckFormat.Substring(CheckFormat.IndexOf(":") + 1);
                string[] arDisplay = CheckFormat.Split(new char[] { '$' });

                if (true == arDisplay.Contains(content))
                    bResult = content == arDisplay[0] ? true : false;

                if (string.IsNullOrEmpty(content)) return false;

                return bResult;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool bValue)
            {
                string CheckFormat = parameter as string;
                string strResult = "";

                CheckFormat = CheckFormat.Substring(CheckFormat.IndexOf(":") + 1);
                string[] arDisplay = CheckFormat.Split(new char[] { '$' });

                strResult = true == bValue ? arDisplay[0] : arDisplay[1];

                return strResult;
            }

            return null;
        }
    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string content)
            {
                string format = parameter as string;

                content = !string.IsNullOrEmpty(content) ? TopApp.ValueConverter.DateTimeConverter.DisplayValue(format, content) : null;

                return content;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string content)
            {
                string format = parameter as string;

                content = !string.IsNullOrEmpty(content) ? TopApp.ValueConverter.DateTimeConverter.DisplayValue(format, content) : null;

                return content;
            }

            return null;
        }
    }

    #endregion

    /// <summary>
    /// Interaction logic for GridWindow.xaml
    /// </summary>
    public partial class DataGridControl : UserControl
    {
        public event CommonDelegateEvents.OnNotifyMethodExecutedEventHandler OnNotifyBlockSearch;
        public event CommonDelegateEvents.OnNotifyIntValueEventHandler OnNotifySelectCell;
        public event CommonDelegateEvents.OnGetObjectEventHandler OnGetBrowseAtomList;

        //private int m_nStartIndex = 1;
        private int m_ParameterMode = 0;
        private int m_FrozenRowCount = 0;

        private double m_Opacity = 1;

        private bool m_bEvent = false;
        private bool m_bFilterEnable = false;

        private Hashtable m_DateFormatInfo = new Hashtable();
        private Hashtable m_CheckFormatInfo = new Hashtable();

        public int ParameterMode
        {
            get { return m_ParameterMode; }
            set { m_ParameterMode = value; }
        }

        public int FrozenColumnCount
        {
            get { return RootDataGrid.FrozenColumnCount; }
            set { RootDataGrid.FrozenColumnCount = value; }
        }

        public int FrozenRowCount
        {
            get { return m_FrozenRowCount; }
            set { m_FrozenRowCount = value; }
        }

        public bool FilterEnable
        {
            get { return m_bFilterEnable; }
            set { m_bFilterEnable = value; }
        }

        public DataGridControl()
        {
            InitializeComponent();
            InitEvent();
        }

        private void InitEvent()
        {
            this.PreviewMouseLeftButtonDown += GridWindow_PreviewMouseLeftButtonDown;

            RootDataGrid.Loaded += RootDataGrid_Loaded;
            RootDataGrid.LayoutUpdated += RootDataGrid_LayoutUpdated;
            RootDataGrid.SelectedCellsChanged += RootDataGrid_SelectedCellsChanged;
        }

        private void RootDataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            DeduplicateData();
        }

        private void RootDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (null != OnNotifySelectCell)
            {
                if (-1 != RootDataGrid.SelectedIndex)
                {
                    OnNotifySelectCell(RootDataGrid.SelectedIndex);
                }
            }
        }

        private void RootDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RootDataGrid.BorderThickness = new System.Windows.Thickness(1, 0, 1, 1);
            ScrollViewer scroll = TopApp.WPFFindChildHelper.FindVisualChild<ScrollViewer>(RootDataGrid);

            if (null != scroll)
            {
                scroll.ScrollChanged += scroll_ScrollChanged;
            }
        }

        private void scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;

            if (null != scroll && false == m_bEvent && 0 < e.VerticalOffset)
            {
                m_bEvent = true;
                int nViewItemCount = 0;

                double dVerticalOffset = e.VerticalOffset;

                double dHeight = RootDataGrid.ActualHeight;
                dHeight -= RootDataGrid.ColumnHeaderHeight;

                nViewItemCount = (int)(dHeight / RootDataGrid.RowHeight);

                if (dVerticalOffset > RootDataGrid.Items.Count - nViewItemCount)
                {
                    if (null != OnNotifyBlockSearch)
                    {
                        OnNotifyBlockSearch();
                        scroll.ScrollToVerticalOffset(dVerticalOffset);
                        this.UpdateLayout(); //UpdateLayout이 없는경우 바인딩 에러 발생할 수 있음
                    }
                }

                m_bEvent = false;
            }
        }

        private void GridWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void RootDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            element = element.Parent as FrameworkElement;
            element = element.Parent as FrameworkElement;

            TextBlock textBlock = WPFFindChildHelper.FindVisualChild<TextBlock>(element);

            string strColumn = textBlock.DataContext.ToString();
            DataView view = RootDataGrid.ItemsSource as DataView;
            DataTable copyView = view.Table.Copy();

            List<string> data = (from DataRowView rowView in copyView.DefaultView group rowView by rowView[strColumn] into newRowView select newRowView.Key.ToString()).ToList();

            data.Sort((x, y) => StringCompareManager.CompareTo(x, y));

            DataGridFilterPopup popup = new DataGridFilterPopup();

            popup.SetBorderColor(GetBorderBrush());
            popup.SetBackColor(GetAtomBackground());

            popup.PlacementTarget = textBlock;
            popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            popup.OnSelectItem += popup_OnSelectItem;
            popup.SetPopup(strColumn, data);
            popup.ShowPopup();
        }

        private void popup_OnSelectItem(object objValue)
        {
            if (null != objValue)
            {
                DataView view = RootDataGrid.ItemsSource as DataView;
                if (null != view)
                {
                    view.RowFilter = objValue.ToString();
                }
                DeduplicateData();
            }
        }

        public void DeduplicateData()
        {
            if (null != OnGetBrowseAtomList)
            {
                CObArray BrowseAtomList = OnGetBrowseAtomList() as CObArray;
                if (null == BrowseAtomList) return;
                DeduplicateData(BrowseAtomList);
            }
        }

        public void DeduplicateData(CObArray browseAtomList)
        {
            DataView view = RootDataGrid.ItemsSource as DataView;
            if (null == view ||
                null == view.DataViewManager ||
                null == view.DataViewManager.DataSet ||
                0 == view.DataViewManager.DataSet.Tables.Count ||
                false == browseAtomList.ToArray().Where(item => (item as BrowseItem).IsSameDataHide == true).Any()) return;


            DataTable dataTable = view.DataViewManager.DataSet.Tables[0];
            for (int i = 0; i < browseAtomList.Count; i++)
            {
                BrowseItem browseAtom = browseAtomList[i] as BrowseItem;
                int nDataKind = browseAtom.ColumnKind;
                if (browseAtom.IsSameDataHide == true)
                {
                    string strSameData = null;
                    for (int nRow = 0; nRow < GetRowCount() - 1; nRow++)
                    {
                        DataGridCell cell = GetCell(nRow, i);
                        DataRowView rowView = RootDataGrid.Items[nRow] as DataRowView;
                        if (null != cell && null != rowView)
                        {
                            string cellValue = rowView.Row[i].ToString();
                            FrameworkElement cellContent = cell.Content as FrameworkElement;
                            if (null != strSameData && null != cellContent)
                            {
                                if (strSameData.Equals(cellValue))
                                {
                                    cell.Visibility = Visibility.Collapsed;
                                    continue;
                                }
                                cell.Visibility = Visibility.Visible;
                            }
                            strSameData = cellValue;
                        }
                    }
                }
            }
        }

        public void SetData(DataTable dataTable, CObArray browseAtomList, DBGridExAtom atomCore)
        {
            if (null == dataTable)
                return;

            m_bEvent = true;

            int nFrozenColumnCount = 1;
            int nDataKind = -1;
            double dRowHeight = 30;

            var atomAttrib = atomCore.Attrib as DBGridExAttrib;

            if (null != atomAttrib)
            {
                nFrozenColumnCount = atomAttrib.FixedCol;
                dRowHeight = atomAttrib.RowHeight;
            }
            

            RootDataGrid.ItemsSource = null;
            RootDataGrid.Columns.Clear();
            m_DateFormatInfo.Clear();
            m_CheckFormatInfo.Clear();

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                BrowseItem browseAtom = null != browseAtomList ? browseAtomList.GetAt(i) as BrowseItem : null;

                if (null != browseAtom)
                {
                    nDataKind = browseAtom.ColumnKind;
                    switch (nDataKind)
                    {
                        case 0:
                            {
                                DataGridTextColumn textColumn = new DataGridTextColumn
                                {
                                    //DataTable의 경우 필드명으로 설정되어있기 표시되는 명칭은 CBrowseAtom.Label에서 가져오도록 한다.
                                    Header = browseAtom.Label,
                                    Width = browseAtom.ColumnWidth,
                                    IsReadOnly = true == RootDataGrid.IsReadOnly ? true : !browseAtom.IsEnabled,
                                    Binding = new System.Windows.Data.Binding(dataTable.Columns[i].ColumnName) { Mode = BindingMode.TwoWay },

                                    CanUserReorder = false, //사용자가 Darg & Drop으로 해더 위치를 변경할 수 없음
                                    CellStyle = nFrozenColumnCount > i ? this.Resources["GridCellHeaderStyle"] as System.Windows.Style : this.Resources["GridCellRowStyle"] as System.Windows.Style,
                                };
                                RootDataGrid.Columns.Add(textColumn);
                                break;
                            }
                        case 1:
                            {
                                string DateFormat = browseAtom.Display;
                                if (!string.IsNullOrEmpty(DateFormat) &&
                                    DateFormat.Split(':') is string[] splitedArr &&
                                    1 < splitedArr.Length)
                                {
                                    DateFormat = DateFormat.Split(':')[1];
                                }

                                DataGridTextColumn dateColumn = new DataGridTextColumn
                                {
                                    Header = browseAtom.Label,
                                    Width = browseAtom.ColumnWidth,
                                    IsReadOnly = true == RootDataGrid.IsReadOnly ? true : !browseAtom.IsEnabled,
                                    Binding = new System.Windows.Data.Binding(dataTable.Columns[i].ColumnName)
                                    {
                                        Mode = BindingMode.TwoWay,
                                        Converter = new DateConverter(),
                                        ConverterParameter = DateFormat
                                    },

                                    CanUserReorder = false,
                                    CellStyle = nFrozenColumnCount > i ? this.Resources["GridCellHeaderStyle"] as System.Windows.Style : this.Resources["GridDateInputCellStyle"] as System.Windows.Style,
                                };
                                m_DateFormatInfo[browseAtom.Label] = DateFormat;
                                RootDataGrid.Columns.Add(dateColumn);
                                break;
                            }
                        case 2:
                            {
                                string strDisplay = false == string.IsNullOrEmpty(browseAtom.Display) ? browseAtom.Display : "$Check:True$False";

                                DataGridCheckBoxColumn checkColumn = new DataGridCheckBoxColumn
                                {
                                    Header = browseAtom.Label,
                                    Width = browseAtom.ColumnWidth,
                                    IsReadOnly = true == RootDataGrid.IsReadOnly ? true : !browseAtom.IsEnabled,
                                    Binding = new System.Windows.Data.Binding(dataTable.Columns[i].ColumnName)
                                    {
                                        Mode = BindingMode.TwoWay,
                                        Converter = new CheckBoxConverter(),
                                        ConverterParameter = strDisplay
                                    },

                                    IsThreeState = false,
                                    CanUserReorder = false,
                                    CellStyle = nFrozenColumnCount > i ? this.Resources["GridCellHeaderStyle"] as System.Windows.Style : this.Resources["GridCellCheckBoxStyle"] as System.Windows.Style,
                                    ElementStyle = this.Resources["GridCheckBoxStyle"] as System.Windows.Style
                                };

                                m_CheckFormatInfo[browseAtom.Label] = strDisplay;
                                RootDataGrid.Columns.Add(checkColumn);
                                break;
                            }
                        case 3:
                            {
                                bool bUserValue = true;
                                List<string> columnList = new List<string>();
                                if (true == string.IsNullOrEmpty(browseAtom.SumData) && RUNMODE_TYPE.PLAY_MODE == atomCore?.AtomRunMode)
                                {
                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        columnList.Add(row[i].ToString());
                                    }

                                    //if (-1 < columnList.Count)
                                    //{
                                    //	columnList = columnList.Distinct<string> ().ToList ();
                                    //	columnList.Sort ();
                                    //	bUserValue = false;
                                    //}

                                    columnList = columnList.Distinct<string>().ToList();
                                    columnList.Sort();
                                    bUserValue = false;
                                }

                                DataGridComboBoxColumn comboColumn = new DataGridComboBoxColumn
                                {
                                    Header = browseAtom.Label,
                                    Width = browseAtom.ColumnWidth,
                                    IsReadOnly = true == RootDataGrid.IsReadOnly ? true : !browseAtom.IsEnabled,

                                    ItemsSource = bUserValue ? browseAtom.SumData.Split(KeyDefine.ENTER_RETURN).ToList<string>() : columnList,
                                    SelectedValueBinding = new System.Windows.Data.Binding(dataTable.Columns[i].ColumnName) { Mode = BindingMode.TwoWay },

                                    CanUserReorder = false,
                                    CellStyle = nFrozenColumnCount > i ? this.Resources["GridCellHeaderStyle"] as System.Windows.Style : this.Resources["GridCellColorStyle"] as System.Windows.Style
                                };
                                RootDataGrid.Columns.Add(comboColumn);
                                break;
                            }
                        case 4:
                            {
                                DataGridHyperlinkColumn hyperlinkColumn = new DataGridHyperlinkColumn
                                {
                                    Header = browseAtom.Label,
                                    Width = browseAtom.ColumnWidth,
                                    IsReadOnly = true == RootDataGrid.IsReadOnly ? true : !browseAtom.IsEnabled,
                                    Binding = new System.Windows.Data.Binding(dataTable.Columns[i].ColumnName) { Mode = BindingMode.TwoWay },

                                    CanUserReorder = false,
                                    CellStyle = nFrozenColumnCount > i ? this.Resources["GridCellHeaderStyle"] as System.Windows.Style : this.Resources["GridCellColorStyle"] as System.Windows.Style,
                                    ElementStyle = this.Resources["GridHyperlinkStyle"] as System.Windows.Style
                                };
                                RootDataGrid.Columns.Add(hyperlinkColumn);
                                break;
                            }

                    }
                }
                else
                {
                    DataGridTextColumn textColumn = new DataGridTextColumn();
                    //1차원배열, 2차원 배열의 경우 DB동작을 하지 않기 때문에 단순 Binding처리만 해준다.
                    textColumn.Header = dataTable.Columns[i].ColumnName;
                    textColumn.Binding = new System.Windows.Data.Binding(dataTable.Columns[i].ColumnName);

                    textColumn.CanUserReorder = false; //사용자가 Darg & Drop으로 해더 위치를 변경할 수 없음
                    textColumn.CellStyle = nFrozenColumnCount > i ? this.Resources["GridCellRowStyle"] as System.Windows.Style : null;
                    RootDataGrid.Columns.Add(textColumn);

                }
            }

            RootDataGrid.ItemsSource = dataTable.DefaultView;

            RootDataGrid.RowHeight = dRowHeight;
            RootDataGrid.ColumnHeaderHeight = dRowHeight;
         
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, (Action)delegate ()
            {
                SetFilterEnable(m_bFilterEnable);
                SetOpacity();
                SetTextHorizontalAlignment(browseAtomList);

                m_bEvent = false;
            });

        }


        public void SetFrozenColumnCount()
        {
            for (int i = 0; i < RootDataGrid.Columns.Count; i++)
            {
                //DataGridTextColumn textColumn = RootDataGrid.Columns[i] as DataGridTextColumn;
                RootDataGrid.Columns[i].CellStyle = FrozenColumnCount > i ? this.Resources["GridCellHeaderStyle"] as System.Windows.Style : this.Resources["GridCellRowStyle"] as System.Windows.Style;
            }
        }

        public void SetDummyCellStyle()
        {
            for (int i = 0; i < RootDataGrid.Columns.Count; i++)
            {
                RootDataGrid.Columns[i].CellStyle = this.Resources["GridCellRowStyle"] as System.Windows.Style;
            }
        }

        void RootDataGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetFilterEnable(m_bFilterEnable);
        }

        private void SetFilterEnable(bool bEnabled)
        {
            List<DataGridColumnHeader> item = WPFFindChildHelper.GetVisualChildCollection<DataGridColumnHeader>(RootDataGrid);

            foreach (DataGridColumnHeader header in item)
            {
                Grid filterGrid = WPFFindChildHelper.FindVisualChild<Grid>(header, "FilterGrid");

                if (null != filterGrid)
                {
                    if (true == bEnabled)
                    {
                        filterGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        filterGrid.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public int GetRowCount()
        {
            if (null != RootDataGrid.Items)
            {
                return RootDataGrid.Items.Count;
            }

            return 0;
        }

        public int GetColumnWidth(int nIndex)
        {
            if (-1 < nIndex && nIndex < RootDataGrid.Columns.Count)
            {
                return (int)RootDataGrid.Columns[nIndex].ActualWidth;
            }

            return 0;
        }

        public void SetAtomBackground(Brush applyBrush)
        {
            this.Resources["Header_Background"] = applyBrush;
            this.Resources["Header_Border"] = applyBrush;
        }

        public void SetBorderBrush(Brush applyBrush)
        {
            this.Resources["Header_Border"] = applyBrush;
        }

        public Brush GetAtomBackground()
        {
            return this.Resources["Header_Background"] as Brush;
        }

        public Brush GetBorderBrush()
        {
            return this.Resources["Header_Border"] as Brush;
        }

        public void SetSeparator(int nType)
        {
            RootDataGrid.VerticalGridLinesBrush = Brushes.Transparent;
            RootDataGrid.HorizontalGridLinesBrush = Brushes.Transparent;

            switch (nType)
            {
                case 0: //가로
                    RootDataGrid.HorizontalGridLinesBrush = Brushes.LightGray;
                    break;
                case 1: //세로
                    RootDataGrid.VerticalGridLinesBrush = Brushes.LightGray;
                    break;
                case 2: //격자
                    RootDataGrid.VerticalGridLinesBrush = Brushes.LightGray;
                    RootDataGrid.HorizontalGridLinesBrush = Brushes.LightGray;
                    break;
                case 3: //없음
                    break;
            }
        }

        public void SetRowHeight(double dHeight)
        {
            if (null != RootDataGrid)
            {
                RootDataGrid.RowHeight = dHeight;
            }
        }

        public void SetOpacity(int nOpacity)
        {
            m_Opacity = 0 < nOpacity ? ((double)nOpacity / 100) : 0;
            SetOpacity();
        }

        private void SetOpacity()
        {
            if (null != RootDataGrid.ItemsSource)
            {
                List<Border> borderlist = WPFFindChildHelper.GetVisualChildCollection<Border>(RootDataGrid, "OpacityBorder");

                if (null != borderlist)
                {
                    foreach (Border b in borderlist)
                    {
                        b.Opacity = m_Opacity;
                    }
                }
            }
        }

        private void SetTextHorizontalAlignment(CObArray browseAtomList)
        {
            if (null == browseAtomList)
                return;

            //CBrowseAtom

            DataGridCellsPanel cellPanel = WPFFindChildHelper.FindVisualChild<DataGridCellsPanel>(RootDataGrid);

            if (null != cellPanel && browseAtomList.Count <= cellPanel.Children.Count)
            {
                for (int i = 0; i < browseAtomList.Count; i++)
                {
                    var cell = cellPanel.Children[i];
                    BrowseItem browseAtom = browseAtomList[i] as BrowseItem;

                    HorizontalAlignment type = (HorizontalAlignment)browseAtom.AlignType;

                    var blockList = WPFFindChildHelper.FindVisualChildList<TextBlock>(cell);

                    foreach (var block in blockList)
                    {
                        block.HorizontalAlignment = type;
                        block.TextTrimming = TextTrimming.CharacterEllipsis;
                        // 202502067 그리드 아톰 Content 말줄임표시
                    }
                }
            }


            DataGridRowsPresenter rowPanel = WPFFindChildHelper.FindVisualChild<DataGridRowsPresenter>(RootDataGrid);

            if (null != rowPanel && browseAtomList.Count <= rowPanel.Children.Count)
            {
                List<List<TextBlock>> blockMapList = new List<List<TextBlock>>();

                for (int i = 0; i < rowPanel.Children.Count; i++)
                {
                    var row = rowPanel.Children[i];
                    var blockList = WPFFindChildHelper.FindVisualChildList<TextBlock>(row);

                    blockMapList.Add(blockList);
                }

                for (int row = 0; row < blockMapList.Count; row++)
                {
                    for (int column = 0; column < browseAtomList.Count; column++)
                    {
                        if (column < browseAtomList.Count)
                        {
                            BrowseItem browseAtom = browseAtomList[column] as BrowseItem;
                            HorizontalAlignment type = (HorizontalAlignment)browseAtom.AlignType;

                            List<TextBlock> blockList = blockMapList[row];

                            if (column < blockList.Count)
                            {
                                blockMapList[row][column].HorizontalAlignment = type;
                                blockMapList[row][column].TextTrimming = TextTrimming.CharacterEllipsis;
                                // 202502067 그리드 아톰 Content 말줄임표시
                            }
                        }
                    }
                }
            }
        }

        public void SetCellBackColor(int nRow, int nColumn, Brush applyBrush)
        {
            TextBlock block = GetCellTextBlock(nRow, nColumn);
            if (null != block)
            {
                block.Background = applyBrush;
            }
        }

        public void SetCellFontColor(int nRow, int nColumn, Brush applyBrush)
        {
            TextBlock block = GetCellTextBlock(nRow, nColumn);
            if (null != block)
            {
                block.Foreground = applyBrush;
            }
        }

        public void SetCellFontSize(int nRow, int nColumn, double dFontSize)
        {
            TextBlock block = GetCellTextBlock(nRow, nColumn);
            if (null != block)
            {
                block.FontSize = dFontSize;
            }
        }

        public void SetCellFontBold(int nRow, int nColumn, bool bBold)
        {
            TextBlock block = GetCellTextBlock(nRow, nColumn);
            if (null != block)
            {
                block.FontWeight = true == bBold ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        public void SetCellFontItalic(int nRow, int nColumn, bool bItalic)
        {
            TextBlock block = GetCellTextBlock(nRow, nColumn);
            if (null != block)
            {
                block.FontStyle = true == bItalic ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        public void SetCellUnderLine(int nRow, int nColumn, bool bUnderLine)
        {
            TextBlock block = GetCellTextBlock(nRow, nColumn);

            if (null != block)
            {
                if (true == bUnderLine)
                {
                    if (false == block.TextDecorations.Contains(TextDecorations.Underline[0]))
                    {
                        block.TextDecorations.Add(TextDecorations.Underline);
                    }
                }
                else
                {
                    block.TextDecorations.Remove(TextDecorations.Underline[0]);
                }
            }
        }

        public void SetCellStrikethrough(int nRow, int nColumn, bool bStrikethrough)
        {
            TextBlock block = GetCellTextBlock(nRow, nColumn);

            if (null != block)
            {
                if (true == bStrikethrough)
                {
                    if (false == block.TextDecorations.Contains(TextDecorations.Strikethrough[0]))
                    {
                        block.TextDecorations.Add(TextDecorations.Strikethrough);
                    }
                }
                else
                {
                    block.TextDecorations.Remove(TextDecorations.Strikethrough[0]);
                }
            }
        }

        public void SetCellFontFamily(int nRow, int nColumn, string strFontName)
        {
            TextBlock block = GetCellTextBlock(nRow, nColumn);

            if (null != block)
            {
                block.FontFamily = new FontFamily(strFontName);
            }
        }

        public void SetCellFocus(int nRow, int nColumn)
        {
            DataGridCell dataGridCell = GetCell(nRow, nColumn);
            if (null != dataGridCell)
            {
                dataGridCell.IsSelected = true;
                Softpower.SmartMaker.TopControl.Components.Helper.KeyboardHelper.Focus(dataGridCell);
            }
        }

        public TextBlock GetCellTextBlock(int row, int column)
        {
            DataGridCell dataGridCell = GetCell(row, column);
            if (null != dataGridCell)
            {
                TextBlock block = WPFFindChildHelper.FindVisualChild<TextBlock>(dataGridCell);
                return block;
            }

            return null;
        }

        public DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = WPFFindChildHelper.FindVisualChild<DataGridCellsPresenter>(rowContainer);

                if (null != presenter)
                {
                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                    return cell;
                }
            }

            return null;
        }

        public DataGridRow GetRow(int index)
        {
            DataGridRow row = RootDataGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
            return row;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Documents.Hyperlink lHyperlink = e.OriginalSource as System.Windows.Documents.Hyperlink;
            string lUri = lHyperlink.NavigateUri.OriginalString;
            Process.Start(lUri);
        }

        private void DataGridDateColumn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            DataGridTextColumn column = cell.Column as DataGridTextColumn;
            TextBlock tb = cell.Content as TextBlock;

            if (true == column.IsReadOnly)
            {
                return;
            }

            string header = cell.Column.Header.ToString();
            string DateFormat = m_DateFormatInfo[header].ToString();

            string strDate = !string.IsNullOrEmpty(tb.Text) ? tb.Text : DateTime.Today.ToString("yyyyMMdd");
            DateConverter converter = new DateConverter();
            string strPicker = (string)converter.Convert(strDate, typeof(string), "yyyyMMdd", null);

            string dateString = ShowPicker(strPicker);
            if (true == string.IsNullOrEmpty(dateString))
                return;

            tb.Text = TopApp.ValueConverter.DateTimeConverter.DisplayValue(DateFormat, dateString);
        }

        private string ShowPicker(string strDateValue)
        {
            Window pickerWindow = null;
            string strResult = "";

            double dLeft = 0;
            double dTop = 0;

            pickerWindow = new SmartDatePicker(strDateValue, false);
            dLeft = 230;
            dTop = 225;

            pickerWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            pickerWindow.Left = GetActualMainWindowLeft(dLeft);
            pickerWindow.Top = GetActualMainWindowTop(dTop);

            if (true == pickerWindow.ShowDialog())
            {
                strResult = ((SmartDatePicker)pickerWindow).DateValue;
            }

            return strResult;
        }

        private double GetActualMainWindowLeft(double dPickerWidth)
        {
            Window MainWindow = Application.Current.MainWindow;
            PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual(MainWindow);
            System.Windows.Media.Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
            double thisDpiWidthFactor = m.M11;
            double thisDpiHeightFactor = m.M22;

            Point ptMouse = PointToScreen(Mouse.GetPosition(this));

            double dLeft = ptMouse.X / thisDpiWidthFactor;

            System.Windows.Forms.Screen currentScreen = null;

            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (ptMouse.X >= screen.WorkingArea.Left && ptMouse.X <= screen.WorkingArea.Left + screen.WorkingArea.Width
                    && ptMouse.Y >= screen.WorkingArea.Top && ptMouse.Y <= screen.WorkingArea.Top + screen.WorkingArea.Height)
                {
                    currentScreen = screen;
                    break;
                }
            }

            if (null == currentScreen)
            {
                return (ptMouse.X / thisDpiWidthFactor);
            }

            double nCurLeft = currentScreen.WorkingArea.Left / thisDpiWidthFactor;
            double nCurWidth = currentScreen.WorkingArea.Width / thisDpiWidthFactor;

            if (nCurLeft + nCurWidth <= dLeft + dPickerWidth)
            {
                return (nCurLeft + nCurWidth - dPickerWidth - 50);
            }

            return dLeft;
        }

        private double GetActualMainWindowTop(double dPickerHeight)
        {
            Window MainWindow = Application.Current.MainWindow;
            PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual(MainWindow);
            System.Windows.Media.Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
            double thisDpiWidthFactor = m.M11;
            double thisDpiHeightFactor = m.M22;

            Point ptMouse = PointToScreen(Mouse.GetPosition(this));

            double dTop = ptMouse.Y / thisDpiHeightFactor;

            System.Windows.Forms.Screen currentScreen = null;

            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (ptMouse.X >= screen.WorkingArea.Left && ptMouse.X <= screen.WorkingArea.Left + screen.WorkingArea.Width
                    && ptMouse.Y >= screen.WorkingArea.Top && ptMouse.Y <= screen.WorkingArea.Top + screen.WorkingArea.Height)
                {
                    currentScreen = screen;
                    break;
                }
            }

            if (null == currentScreen)
            {
                return (ptMouse.Y / thisDpiWidthFactor) + 15;
            }


            double nCurTop = currentScreen.WorkingArea.Top / thisDpiHeightFactor;
            double nCurHeight = currentScreen.WorkingArea.Height / thisDpiHeightFactor;


            if (nCurTop + nCurHeight <= dTop + dPickerHeight)
            {
                return (nCurTop + nCurHeight - dPickerHeight - 50);
            }

            return dTop + 15;
        }

        // 20250206 KH 그리드 아톰 UI변경 시 정렬 재실행
        private void DataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            if (null != OnGetBrowseAtomList)
            {
                CObArray BrowseAtomList = OnGetBrowseAtomList() as CObArray;

                if (null == BrowseAtomList)
                {
                    return;
                }
                SetTextHorizontalAlignment(BrowseAtomList);
            }
        }
    }
}




//private void MainHeaderControlColumn_OnNotifyChagedEditMode(bool bValue)
//{
//    if (null != OnNotifyChagedEditMode)
//    {
//        OnNotifyChagedEditMode(bValue);
//    }
//}

//public void DataGridBindingFromDBIO (DataTable dataTable, int nFreezonCount)
//{
//    if (1 > dataTable.Rows.Count)
//        return;

//    RootDataGrid.ItemsSource = null;

//    RootDataGrid.ItemsSource = dataTable.DefaultView;
//    RootDataGrid.FrozenColumnCount = nFreezonCount;
//}


//public void AutoHeaderHeightChange()
//{
//    //System.Collections.Generic.List<double> rowlist = UserDataGridMain.GetRowHeight();
//    //if (rowlist.Count > 0)
//    //{
//    //    HeaderRowDefinition.Height = new System.Windows.GridLength(rowlist[0]);
//    //    MainHeaderControlRow.SetRowHeaderHeightChange(rowlist);
//    //}
//}


//public void AutoHeaderWidthChange()
//{
//    //HeaderColumnDefinition.Width = new System.Windows.GridLength(UserDataGridMain.GetDataGrid.Columns[0].ActualWidth);
//    //MainHeaderControlColumn.SetColumnHeaderWidthChange(UserDataGridMain.GetDataGrid, m_nStartIndex);
//}


//public void SetRowHeight(int nHeight)
//{
//    RootDataGrid.RowHeight = nHeight;
//}

//public void SetReadOnly(bool bReadOnly)
//{
//    RootDataGrid.IsReadOnly = bReadOnly;
//}

//public void SortDescription(string strHeaderName)
//{
//    //UserDataGridMain.SortDescription(strHeaderName);
//}

//private void HeaderControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
//{
//    //if (null != SizeChangeScrollviewerEvent)
//    //    SizeChangeScrollviewerEvent();
//}

//private void ScrollViewerDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
//{
//    //this.ScrollViewerRowHeader.ScrollToVerticalOffset((sender as ScrollViewer).VerticalOffset);
//    //this.ScrollViewerColumnHeader.ScrollToHorizontalOffset((sender as ScrollViewer).HorizontalOffset);
//}

//public void SetScrollViewerVerticalDataGrid(double dVerticalOffset)
//{
//    //this.ScrollViewerDataGrid.ScrollToVerticalOffset(dVerticalOffset);
//}

//public void SetScrollViewerHorizontalDataGrid(double dHorizontalOffset)
//{
//    //this.ScrollViewerDataGrid.ScrollToHorizontalOffset(dHorizontalOffset);
//}

//public void AllClearInitialize()
//{
//    //MainHeaderControlColumn.ColumnAllClear();
//    //UserDataGridMain.AllClearItmesSource();
//    //MainHeaderControlRow.RowAllClear();
//    //MainHeaderControlCross.AllClear();

//    //HeaderControl_SizeChanged(null, null);
//}

//public void AddRow(int nRow) //
//{
//    //if (nRow >= UserDataGridMain.DataGridMain.Items.Count && 0 < UserDataGridMain.DataGridMain.Items.Count)
//    //{
//    //    DataRowView pRowDataView = UserDataGridMain.DataGridMain.Items[0] as DataRowView;
//    //    DataTable pDataTable = pRowDataView.Row.Table;

//    //    for (int i = UserDataGridMain.DataGridMain.Items.Count; i <= nRow; i++)
//    //    {
//    //        string strLabel = (i + 1).ToString();
//    //        pDataTable.Rows.Add(strLabel, "");

//    //        MainHeaderControlRow.RowDefine(strLabel);
//    //    }

//    //    if (null != RowCountChangeEvent)
//    //    {
//    //        RowCountChangeEvent();
//    //    }
//    //}
//}

//public void SetColumnWidth (CObArray browseAtomList) //Column이 없는경우 RootGrid.FrozenColumnCount는 0으로 고정된다.
//{
//    //RootDataGrid.ItemsSource = null;
//    //RootDataGrid.Columns.Clear ();

//    for (int i = 0; i < browseAtomList.Count; i++)
//    {
//        CBrowseAtom browseAtom = browseAtomList.GetAt (i) as CBrowseAtom;
//        DataGridTextColumn textColumn = RootDataGrid.Columns[i] as DataGridTextColumn;
//        //textColumn.Header = browseAtom.Label;
//        //textColumn.Binding = new System.Windows.Data.Binding (browseAtom.BrowseVar);
//        textColumn.Width = browseAtom.ColumnWidth;

//        //textColumn.CanUserReorder = false; //사용자가 Darg & Drop으로 해더 위치를 변경할 수 없음
//        //textColumn.CellStyle = nFrozenColumnCount > i ? this.Resources["GridCellStyle"] as System.Windows.Style : null;
//        //RootDataGrid.Columns.Add (textColumn);
//    }
//}