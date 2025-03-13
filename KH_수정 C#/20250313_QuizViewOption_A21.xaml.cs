using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView;
using Softpower.SmartMaker.TopAtom.Ebook.Components.SubControl;

namespace Softpower.SmartMaker.TopSmartAtomEdit.SubControls.QuizViewOption
{
	/// <summary>
	/// QuizViewOption_A21 : 선긋기 옵션창
	/// </summary>
	public partial class QuizViewOption_A21 : Grid
	{
		public ObservableCollection<ThicknessItem> LineThicknessList { get; set; }		
		// public ObservableCollection<ColorItem> LineColorList { get; set; }					// OptnioNode  에서 List를 받아서 ComboBox구현 (미사용)

		private ColorPopup m_LineColorPopup;			// ColorPopup 컨트롤
		private ColorPopup m_EllipseColorPopup;     // EllipseColorPopup 컨트롤

		private int shapeSelector = -1;

		public int DirectionType
		{
			get
			{
				return DirectionTypeCombo.SelectedIndex;
			}
			set
			{
				DirectionTypeCombo.SelectedIndex = value;
			}
		}

		public int LineType
		{
			get
			{
				return LineTypeCombo.SelectedIndex;
			}
			set
			{
				LineTypeCombo.SelectedIndex = value;
			}
		}

		public int LineThickness
		{
			get
			{
				// SelectedIndex를 반환하기 때문에 1px ~ 9px 까지 나올 수 있도록 +1
				return LineThicknessCombo.SelectedIndex + 1;
			}
			set
			{
				// SelectedIndex + 1 을 반환받기 때문에 1px ~ 9px 까지 나올 수 있도록 -1
				if (value >= 1 && value <= 9)
				{
					LineThicknessCombo.SelectedIndex = value - 1;
				}
			}
		}
		
		private ColorPopup LineColorPopup
		{
			get
			{
				if (null == m_LineColorPopup)
				{
					m_LineColorPopup = new ColorPopup ();
					m_LineColorPopup.NotifyColorChanged += ColorPopup_NotifyColorChanged;
				}
				return m_LineColorPopup;
			}
		}

		private ColorPopup EllipseColorPopup
		{
			get
			{
				if (null == m_EllipseColorPopup)
				{
					m_EllipseColorPopup = new ColorPopup ();
					m_EllipseColorPopup.NotifyColorChanged += ColorPopup_NotifyColorChanged;
				}
				return m_EllipseColorPopup;
			}
		}


		public int LineColor
		{
			get
			{
				SolidColorBrush brush = LineColorPopup.SelectColor as SolidColorBrush; 
				int argb = brush.Color.A << 24 | brush.Color.R << 16 | brush.Color.G << 8 | brush.Color.B;
				return argb;
			}
			set
			{
				// 정수 값을 ARGB 색상으로 변환
				byte a = (byte)((value >> 24) & 0xFF);
				byte r = (byte)((value >> 16) & 0xFF);
				byte g = (byte)((value >> 8) & 0xFF);
				byte b = (byte)(value & 0xFF);

				SolidColorBrush newBrush = new SolidColorBrush (Color.FromArgb (a, r, g, b));

				// 색상을 적용
				LineColorPopup.SelectColor = newBrush;			// 팝업 색상 업데이트
				LineColorRectangle.Fill = newBrush;				// UI 사각형 색상 변경
				LineColorRectangleBottom.Fill = newBrush;
			}
		}

		public int EllipseColor
		{
			get
			{
				SolidColorBrush brush = EllipseColorPopup.SelectColor as SolidColorBrush;
				int argb = brush.Color.A << 24 | brush.Color.R << 16 | brush.Color.G << 8 | brush.Color.B;
				return argb;
			}
			set
			{
				// 정수 값을 ARGB 색상으로 변환
				byte a = (byte)((value >> 24) & 0xFF);
				byte r = (byte)((value >> 16) & 0xFF);
				byte g = (byte)((value >> 8) & 0xFF);
				byte b = (byte)(value & 0xFF);

				SolidColorBrush newBrush = new SolidColorBrush (Color.FromArgb (a, r, g, b));

				// 색상을 적용
				EllipseColorPopup.SelectColor = newBrush;			// 팝업 색상 업데이트
				EllipseColorEllipse.Fill = newBrush;                // UI 사각형 색상 변경
				EllipseColorRectangleBottom.Fill = newBrush;       
			}
		}

		// 선 굵기 항목 클래스
		public class ThicknessItem
		{
			public double Thickness { get; set; }
			public string Name { get; set; }
		}

		// 색상 항목 클래스
		public class ColorItem
		{
			public string Name { get; set; }
			public SolidColorBrush Color { get; set; }
		}


		public int QuestionCount
		{
			get { return _Kiss.toInt32 (QuestionCountBox.Text); }
			set { QuestionCountBox.Text = value.ToString (); }
		}

		public int AnswerCount
		{
			get { return _Kiss.toInt32 (AnswerCountBox.Text); }
			set { AnswerCountBox.Text = value.ToString (); }
		}

		public int QuestionSignType
		{
			get { return QuestionSignTypeCombo.SelectedIndex; }
			set { QuestionSignTypeCombo.SelectedIndex = value; }
		}

		public int AnswerSignType
		{
			get { return AnswerSignTypeCombo.SelectedIndex; }
			set { AnswerSignTypeCombo.SelectedIndex = value; }
		}

		public QuizViewOption_A21 ()
		{
			InitializeComponent ();
			DataContext = this;
			InItStyle ();
			InitEvent ();
			InitPopup ();
		}

		private void InItStyle ()
		{
			foreach (var item in EBookQuizOptionNode.QuizOption_A21.DirectionTypeStrList)
			{
				DirectionTypeCombo.Items.Add (item);
			}

			foreach (var item in EBookQuizOptionNode.QuizOption_A21.LineTypeList)
			{
				LineTypeCombo.Items.Add (item);
			}

			// 선 굵기 미리보기 추가
			LineThicknessList = new ObservableCollection<ThicknessItem> ();
			foreach (var item in EBookQuizOptionNode.QuizOption_A21.LineThicknessList)
			{
				ThicknessItem tempThickness = new ThicknessItem { Thickness = Convert.ToDouble (item), Name = item + "px" };
				LineThicknessList.Add (tempThickness);
			}
			LineThicknessCombo.ItemsSource = LineThicknessList;
			LineThicknessCombo.SelectedIndex = 0;

			/* 
			// 선 색상 미리보기 추가 OptnioNode  에서 List를 받아서 ComboBox구현 (미사용)
			LineColorList = new ObservableCollection<ColorItem> ();
			foreach (var item in EBookQuizOptionNode.QuizOption_A21.LineColorList)
			{
				LineColorList.Add (new ColorItem { Name = item, Color = (SolidColorBrush)new BrushConverter ().ConvertFromString (item) });
			}
			LineColorCombo.ItemsSource = LineColorList;
			*/

			foreach (var item in EBookQuizOptionNode.QuizOption_A21.AnswerSignTypeList)
			{
				AnswerSignTypeCombo.Items.Add (item);
				QuestionSignTypeCombo.Items.Add (item);
			}

			DirectionTypeCombo.SelectedIndex = 0;
			LineTypeCombo.SelectedIndex = 0;
			AnswerSignTypeCombo.SelectedIndex = 0;
			QuestionSignTypeCombo.SelectedIndex = 0;
			LineThicknessCombo.SelectedIndex = 0;		
		}

		private void InitEvent ()
		{
			this.LineColorUnit.MouseLeftButtonDown += LineColorUnit_MouseLeftButtonDown;
			this.EllipseColorUnit.MouseLeftButtonDown += EllipseColorUnit_MouseLeftButtonDown;

			this.LineColorPopup.Opened += Popup_Opened;
			this.EllipseColorPopup.Opened += Popup_Opened;
		}

		private void InitPopup ()
		{
			LineColorPopup.PlacementTarget = LineColorUnit;
			LineColorPopup.Placement = PlacementMode.Bottom;
			LineColorPopup.StaysOpen = false;

			EllipseColorPopup.PlacementTarget = EllipseColorUnit;
			EllipseColorPopup.Placement = PlacementMode.Bottom;
			EllipseColorPopup.StaysOpen = false;
		}

		private void LineColorUnit_MouseLeftButtonDown (object sender, MouseEventArgs e)
		{
			shapeSelector = 0;
			LineColorPopup.StaysOpen = true;
			LineColorPopup.IsOpen = true;
			e.Handled = true;
			
		}

		private void EllipseColorUnit_MouseLeftButtonDown (object sender, MouseEventArgs e)
		{
			shapeSelector = 1;
			EllipseColorPopup.StaysOpen = true;
			EllipseColorPopup.IsOpen = true;
			e.Handled = true;			
		}

		async Task DelayPopupAsync (Popup popup)
		{
			await Task.Delay (200);

			popup.StaysOpen = false;
		}

		void Popup_Opened (object sender, EventArgs e)
		{
			_ = DelayPopupAsync (sender as Popup);
		}

		void ColorPopup_NotifyColorChanged (DelegateEventKeys.ColorDialogEventKey ColorDialogKey, object applyValue)
		{
			var SelectColor = applyValue as Brush;

			if(shapeSelector < 0)
			{
				return;
			}
			else if(shapeSelector == 0)
			{
				LineColorRectangle.Fill = SelectColor;
				LineColorRectangleBottom.Fill = SelectColor;
				LineColorPopup.IsOpen = false;
			}
			else if (shapeSelector == 1)
			{
				EllipseColorEllipse.Fill = SelectColor;
				EllipseColorRectangleBottom.Fill = SelectColor;
				EllipseColorPopup.IsOpen = false;
			}			
		}
	}
}
