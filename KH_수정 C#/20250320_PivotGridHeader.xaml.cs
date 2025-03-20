using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.CommonLib;

namespace Softpower.SmartMaker.TopAtom.Components.TreeAtom
{
	public delegate void OnColumnSplitterDragDeltaEventHandler (double[] columnWidthList);

	public partial class PivotGridHeader : Border
	{
		public event OnColumnSplitterDragDeltaEventHandler OnColumnSplitterDragDeltaEvent = null;
		private double[] m_arColumnWidth;
		private string[] strHeaderList;

		private int headerCount = 0;

		public int ColumnDefinitionsCount
		{
			get
			{
				return RootGrid.ColumnDefinitions.Count;
			}
		}

		public int HeaderCount
		{
			get { return headerCount; }
			set { headerCount = value; }
		}

		public ColumnDefinitionCollection ColumnDefinitionList
		{
			get { return RootGrid.ColumnDefinitions; }
		}

		public PivotGridHeader (string[] strheaderList)
		{
			strHeaderList = strheaderList;
			InitializeComponent ();
			m_arColumnWidth = new double[strHeaderList.Length];
		}

		public double[] ColumnWidth
		{
			get
			{
				return m_arColumnWidth;
			}
			set
			{
				m_arColumnWidth = value;
			}
		}

		public void MakeHeader ()
		{
			HeaderCount = 0;
			for (int nIndex = 0; nIndex < strHeaderList.Length; nIndex++)
			{
				ColumnDefinition column = new ColumnDefinition ();
				int tempWidth = 2;

				if (true == string.IsNullOrEmpty (strHeaderList[nIndex]))
				{
					m_arColumnWidth[nIndex] = 0;
					tempWidth = 0;
				}

				HeaderCount++;

				column.Width = new GridLength (m_arColumnWidth[nIndex], GridUnitType.Pixel);
				RootGrid.ColumnDefinitions.Add (column);

				Grid columnHeaderGrid = new Grid ();
				RootGrid.Children.Add (columnHeaderGrid);
				Grid.SetColumn (columnHeaderGrid, nIndex);

				TextBlock columnheaderTextBlock = new TextBlock ();
				columnheaderTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
				columnheaderTextBlock.VerticalAlignment = VerticalAlignment.Center;
				columnheaderTextBlock.Text = strHeaderList[nIndex];
				columnHeaderGrid.Children.Add (columnheaderTextBlock);

				GridSplitter columnheaderSplitter = new GridSplitter ();
				columnheaderSplitter.DragDelta += columnheaderSplitter_DragDelta;
				columnheaderSplitter.Background = WPFColorConverter.ConvertHexToWPFBrush ("#FFC5DEFF");
				//columnheaderSplitter.Background = Brushes.Transparent;
				columnheaderSplitter.HorizontalAlignment = HorizontalAlignment.Right;
				columnheaderSplitter.VerticalAlignment = VerticalAlignment.Stretch;
				columnheaderSplitter.ResizeBehavior = GridResizeBehavior.BasedOnAlignment;
				columnheaderSplitter.Width = tempWidth;
				RootGrid.Children.Add (columnheaderSplitter);
				Grid.SetColumn (columnheaderSplitter, nIndex);
			}
		}

		private void columnheaderSplitter_DragDelta (object sender, DragDeltaEventArgs e)
		{
			if (null != OnColumnSplitterDragDeltaEvent)
			{
				int TargetCol = Grid.GetColumn ((sender as GridSplitter));
				for (int nIndex = 0; nIndex < RootGrid.ColumnDefinitions.Count; nIndex++)
				{
					ColumnDefinition column = RootGrid.ColumnDefinitions[nIndex];
					if (TargetCol == nIndex)
					{
						double width = column.Width.Value + e.HorizontalChange;
						column.Width = new GridLength (width, GridUnitType.Pixel);
						//m_arColumnWidth[nIndex] = width;
					}
					m_arColumnWidth[nIndex] = column.Width.Value;
				}

				OnColumnSplitterDragDeltaEvent (m_arColumnWidth);
			}
		}
	}
}
