using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.CommonLib.Interface;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopControl.Components.ColorComponents;
using Softpower.SmartMaker.TopControl.Components.TileAndRuler;

namespace Softpower.SmartMaker.TopProcessEdit
{
	/// <summary>
	/// Interaction logic for TileGuideConfigPanel.xaml
	/// </summary>
	public partial class TileGuideConfigPanel : Grid, IFlexibleWindowContentBehavior
	{

		private ITileGuideViewBehavior m_TileView = null;

		#region Property;

		public ITileGuideViewBehavior TileView
		{
			get
			{
				return m_TileView;
			}
			set
			{
				m_TileView = value;
			}
		}

		public Brush GroupBox1_BorderBrush
		{
			get
			{
				return GroupBox1.BorderBrush;
			}
			set
			{
				GroupBox1.BorderBrush = value;
			}
		}

		public Brush GroupBox2_BorderBrush
		{
			get
			{
				return GroupBox2.BorderBrush;
			}
			set
			{
				GroupBox2.BorderBrush = value;
			}
		}

        public Brush GroupBox3_BorderBrush
        {
            get
            {
                return GroupBox3.BorderBrush;
            }
            set
            {
                GroupBox3.BorderBrush = value;
            }
        }

        #endregion Property;


        public TileGuideConfigPanel ()
		{
			InitializeComponent ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("TileGuideConfigPanel", this);
			}

			InitComboBoxItems ();
			InitEvent ();
		}

		private void InitEvent ()
		{
			TileGapDecimalUpDown.TextInput += TileGapDecimalUpDown_TextInput;
		}



		public TileGuideConfigPanel (ArrayList alParam) : this ()
		{
			InitConfigPage (alParam);
		}

		public void InitComboBoxItems ()
		{
			TileMeasureTypeComboBox.Items.Add (LC.GS ("TopProcessEdit_TileGuideConfigPanel_1"));
			TileMeasureTypeComboBox.Items.Add (LC.GS ("TopProcessEdit_TileGuideConfigPanel_2"));
			TileMeasureTypeComboBox.Items.Add (LC.GS ("TopProcessEdit_TileGuideConfigPanel_3"));
			TileMeasureTypeComboBox.Items.Add (LC.GS ("TopProcessEdit_TileGuideConfigPanel_4"));
		}

		private void InitConfigPage (ArrayList alParam)
		{
			try
			{
				TileView = alParam[0] as ITileGuideViewBehavior;
				TileGapDecimalUpDown.Value = (decimal)TileView.GetTileSize ();
				TileMeasureTypeComboBox.SelectedIndex = (int)TileView.GetTileMeasureType ();
				GroupBox1_BorderBrush = new SolidColorBrush (TileView.GetTileColor ());
				GroupBox2_BorderBrush = new SolidColorBrush (TileView.GetTileColor ());
                GroupBox3_BorderBrush = new SolidColorBrush (TileView.GetTileColor());
            }
			catch
			{
				TileGapDecimalUpDown.Value = 20;
				TileMeasureTypeComboBox.SelectedIndex = 0;
				GroupBox1_BorderBrush = new SolidColorBrush (Colors.Black);
				GroupBox2_BorderBrush = new SolidColorBrush (Colors.Black);
                GroupBox3_BorderBrush = new SolidColorBrush (Colors.Black);
            }
		}

		void TileGapDecimalUpDown_TextInput (object sender, TextCompositionEventArgs e)
		{

		}

		#region interface

		public void ProcessOK ()
		{
			try
			{
				TileMeasureTypeComboBox.Focus ();
				int nSelectedIndex = 0;

				if (null != TileMeasureTypeComboBox.SelectedItem)
				{
					nSelectedIndex = TileMeasureTypeComboBox.SelectedIndex;
				}

				TileView.SetTileSize ((double)TileGapDecimalUpDown.Value);
				TileView.SetTileColor (((SolidColorBrush)GroupBox1_BorderBrush).Color);
				TileView.SetTileMeasureType ((RTConfig.RulerMeasureType)nSelectedIndex);
			}
			catch
			{
				TileView.SetTileSize (10);
				TileView.SetTileColor (Colors.Black);
				TileView.SetTileMeasureType (RTConfig.RulerMeasureType.Pixel);
			}
			TileView.InitTileGuide ();
		}

		public void ProcessCancel ()
		{
		}

		#endregion interface


		#region EventHandler 

		public void ColorRect_MouseUp (object sender, MouseEventArgs e)
		{
			Rectangle SelectedRectangle = sender as Rectangle;

			if (null != SelectedRectangle)
			{
				GroupBox1_BorderBrush = SelectedRectangle.Fill;
				GroupBox2_BorderBrush = SelectedRectangle.Fill;
                GroupBox3_BorderBrush = SelectedRectangle.Fill;
            }
		}

		private void TileMeasureTypeComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (null != TileMeasureTypeComboBox.SelectedItem)
			{
				int nSelectedIndex = TileMeasureTypeComboBox.SelectedIndex;

				string strFormat = "F0";
				double dIncrement = 1;
				double dMaximum = 1000;
				double dMinimum = 0;

				switch ((RTConfig.RulerMeasureType)nSelectedIndex)
				{
					case RTConfig.RulerMeasureType.Pixel:
					case RTConfig.RulerMeasureType.Point:
						break;
					case RTConfig.RulerMeasureType.Inch:
					case RTConfig.RulerMeasureType.Centimeter:
						strFormat = "F1";
						dIncrement = 0.1;
						dMaximum = 10;
						break;
				}

				TileGapDecimalUpDown.FormatString = strFormat;
				TileGapDecimalUpDown.Increment = (decimal)dIncrement;
				TileGapDecimalUpDown.Maximum = (decimal)dMaximum;
				TileGapDecimalUpDown.Minimum = (decimal)dMinimum;

				if (TileGapDecimalUpDown.Maximum < TileGapDecimalUpDown.Value)
				{
					TileGapDecimalUpDown.Value = TileGapDecimalUpDown.Maximum;
				}

				if (TileGapDecimalUpDown.Minimum > TileGapDecimalUpDown.Value)
				{
					TileGapDecimalUpDown.Value = TileGapDecimalUpDown.Minimum;
				}
			}
		}

        #endregion EventHandler


        // 20250203 KH 그리드 안내선 설정 창에 다른색 선택(색상팔레트) 기능 추가하기
        private void DifferentColorStackPanelClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;

            CommonPaletteWindow cpWindow = new CommonPaletteWindow();

            cpWindow.Owner = Application.Current.MainWindow;
			cpWindow.OldBrush = GroupBox3_BorderBrush;

            if (true == cpWindow.ShowDialog((FrameworkElement)sender))
            {
                GroupBox1_BorderBrush = cpWindow.CurrentBrush;
                GroupBox2_BorderBrush = cpWindow.CurrentBrush;
                GroupBox3_BorderBrush = cpWindow.CurrentBrush;
            }
        }

        private void DifferentColorStackPanelMouseEnter(object sender, MouseEventArgs e)
        {
			GroupBox row = sender as GroupBox;

			if (null != row)
			{
                row.Background = Brushes.LightCyan;
            }
        }

        private void DifferentColorStackPanelMouseLeave(object sender, MouseEventArgs e)
        {
            GroupBox row = sender as GroupBox;

            if (null != row)
            {
                row.Background = Brushes.AliceBlue;
            }
        }
    }
}
