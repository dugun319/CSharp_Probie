using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.DelegateEventResource;
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
		public event CommonDelegateEvents.OnNotifyObjectEventHandler NotifyLineColorChanged;

		private ColorPopup m_ColorPopup;	// ColorPopup 컨트롤

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
				return LineThicknessCombo.SelectedIndex;
			}
			set
			{
				LineThicknessCombo.SelectedIndex = value;
			}
		}
		
		private ColorPopup ColorPopup
		{
			get
			{
				if (null == m_ColorPopup)
				{
					m_ColorPopup = new ColorPopup ();
					m_ColorPopup.NotifyColorChanged += ColorPopup_NotifyColorChanged;
				}
				return m_ColorPopup;
			}
		}
		
		
		public int LineColor
		{
			get
			{
				SolidColorBrush brush = ColorPopup.SelectColor as SolidColorBrush; 
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
				ColorPopup.SelectColor = newBrush;  // 팝업 색상 업데이트
				LineColorRectangle.Fill = newBrush; // UI 사각형 색상 변경
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
			this.LineColorUnit.MouseEnter += LineColorUnit_MouseEnter;
			this.LineColorUnit.MouseLeave += LineColorUnit_MouseLeave;
			this.LineColorUnit.MouseLeftButtonDown += LineColorUnit_MouseLeftButtonDown;

			this.ColorPopup.Opened += Popup_Opened;
		}

		private void InitPopup ()
		{
			ColorPopup.PlacementTarget = LineColorUnit;
			ColorPopup.Placement = PlacementMode.Bottom;
			ColorPopup.StaysOpen = false;
		}

		private void LineColorUnit_MouseEnter (object sender, MouseEventArgs e)
		{

		}

		private void LineColorUnit_MouseLeave (object sender, MouseEventArgs e)
		{

		}

		private void LineColorUnit_MouseLeftButtonDown (object sender, MouseEventArgs e)
		{
			ColorPopup.StaysOpen = true;
			ColorPopup.IsOpen = true;
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
			LineColorChanged ((Brush)applyValue);
			var SelectColor = ColorPopup.SelectColor;
			LineColorRectangle.Fill = SelectColor;
			ColorPopup.IsOpen = false;
		}

		private void LineColorChanged (Brush fontBrush)
		{
			if (null != NotifyLineColorChanged)
			{
				NotifyLineColorChanged (fontBrush);
			}
		}
	}
}
