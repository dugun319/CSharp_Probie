using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom.Components.CalendarAtom;
using Softpower.SmartMaker.TopAtom.Ebook.Components.SubControl;
using Softpower.SmartMaker.TopControl.Components.ScreenCapture;
using Softpower.SmartMaker.TopControlRun.SmartDateTimePicker;

namespace Softpower.SmartMaker.TopAtom
{
	/// <summary>
	/// CalendarofAtom.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class CalendarofAtom : CalendarAtomBase
	{
		private const int CALENDAR_COLUMN = 7;
		private const int CALENDAR_ROW = 6;

		private List<CalendarItem> m_CalendarItemList = null;
		private Dictionary<string, List<Brush>> m_RunModeFlagMap = null; //업무규칙으로 변경한 글자색 및 배경색
																		 //private Dictionary<string, CalendarScheduleData> m_CalendarScheduleDataList = null; //업무규칙으로 사용자가 지정함
		private List<CalendarScheduleData> m_CalendarScheduleDataList = null;    //전체 일정
		private List<CalendarScheduleData> m_CalendarScheduleInsertList = null;   //업무규칙상으로 추가한 일정
		private List<CalendarScheduleData> m_CalendarScheduleUpDateList = null; //DB에서 로드한 일정을 변경한 경우
		private List<CalendarScheduleData> m_CalendarScheduleDeleteList = null;   //DB에서 로드한 일정을 삭제했을경우

		private int m_nYear;
		private int m_nMonth;
		private int m_nDay;
		private int m_nBorderIndex = 0;

		private CalendarItem m_TempItem = null;

		private readonly string STR_YEAR = LC.GS ("TopAtom_CalendarAtomCore_3"); //년
		private readonly string STR_MONTH = LC.GS ("TopAtom_CalendarAtomCore_4"); //월

		private CalendarSlideAnimation m_CalendarSlideAnimation;

		public event CommonDelegateEvents.OnNotifyObjectEventHandler OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent;

        #region 속성

        public Dictionary<string, List<Brush>> RunModeFlagMap
		{
			get { return m_RunModeFlagMap; }
		}

		public List<CalendarItem> CalendarItemList
		{
			get { return m_CalendarItemList; }
		}


		public List<CalendarScheduleData> CalendarScheduleData
		{
			get { return m_CalendarScheduleDataList; }
		}

		public List<CalendarScheduleData> CalendarScheduleInsertList
		{
			get { return m_CalendarScheduleInsertList; }
		}

		public List<CalendarScheduleData> CalendarScheduleUpDateList
		{
			get { return m_CalendarScheduleUpDateList; }
		}

		public List<CalendarScheduleData> CalendarScheduleDeleteList
		{
			get { return m_CalendarScheduleDeleteList; }
		}

		public int BorderIndex
		{
			get { return m_nBorderIndex; }
			set
			{
				m_nBorderIndex = value;
				if (AtomCore?.GetAttrib () is CalendarAttrib attrib)
				{
					attrib.BorderIndex = value;
				}
			}
		}

		public int Year
		{
			get { return m_nYear; }
			set { m_nYear = value; }
		}

		public int Month
		{
			get { return m_nMonth; }
			set { m_nMonth = value; }
		}

		public int Day
		{
			get { return m_nDay; }
			set { m_nDay = value; }
		}

		public bool IsEnabledBorderBrush
		{
			get
			{
				if (0 == BorderIndex || 3 == BorderIndex)
				{
					return true;
				}

				return false;
			}
		}

		#endregion

		public CalendarofAtom ()
		{
			InitializeComponent ();
			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("ChartofAtom", this);
			}

			InitStyle ();
			InitEvent ();
		}

		public CalendarofAtom (Atom atomCore)
			: base (atomCore)
		{
			InitializeComponent ();
			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("ChartofAtom", this);
			}

			InitStyle ();
			InitEvent ();
		}

		private void InitStyle ()
		{
			m_CalendarItemList = new List<CalendarItem> ();
			m_RunModeFlagMap = new Dictionary<string, List<Brush>> ();
			m_CalendarScheduleDataList = new List<CalendarScheduleData> ();

			m_CalendarScheduleInsertList = new List<CalendarScheduleData> ();
			m_CalendarScheduleUpDateList = new List<CalendarScheduleData> ();
			m_CalendarScheduleDeleteList = new List<CalendarScheduleData> ();

			LeftButton.IsEnabled = false; //편집모드에서 비활성화
			RightButton.IsEnabled = false;  //편집모드에서 비활성화

		}

		private void InitAtomAttribStyle ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				UpdateHorizontalAlignment ();
				UpdateDayBorderThickness ();
				UpdateVerticalAlignment ();
				UpdateAtomFontSize ();
				UpdateAtomFontColor ();
				UpdateAtomFontFamily ();
			}
		}

		private void UpdateHorizontalAlignment ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				DateTimeGrid.HorizontalAlignment = pAttrib.DateTimeGridHorizontalAlignment;

				Sunday.HorizontalAlignment = pAttrib.DayWeekGridHorizontalAlignment;
				Monday.HorizontalAlignment = pAttrib.DayWeekGridHorizontalAlignment;
				Tuesday.HorizontalAlignment = pAttrib.DayWeekGridHorizontalAlignment;
				Wednesday.HorizontalAlignment = pAttrib.DayWeekGridHorizontalAlignment;
				Thursday.HorizontalAlignment = pAttrib.DayWeekGridHorizontalAlignment;
				Friday.HorizontalAlignment = pAttrib.DayWeekGridHorizontalAlignment;
				Saturday.HorizontalAlignment = pAttrib.DayWeekGridHorizontalAlignment;

				if (null != m_CalendarItemList)
				{
					foreach (CalendarItem pItem in m_CalendarItemList)
					{
						pItem.SetHorizontalAlignment (pAttrib.DayGridHorizontalAlignment);
						//pItem.NumBlock.HorizontalAlignment = pAttrib.DayGridHorizontalAlignment;
					}
				}
			}
		}

		private void UpdateDayBorderThickness ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				for (int y = 0; y < CALENDAR_ROW; y++)
				{
					for (int x = 0; x < CALENDAR_COLUMN; x++)
					{
						int nItemIndex = x + (y * CALENDAR_COLUMN);
						if (nItemIndex < m_CalendarItemList.Count)
						{
							CalendarItem pItem = m_CalendarItemList[nItemIndex];

							if (true == pAttrib.DisplayDayBorderThickness)
							{
								Thickness thickness = new Thickness (1);

								if (0 < x)
									thickness.Left = 0;
								if (0 < y)
									thickness.Top = 0;

								if (pAttrib.PenFill)
								{
									if (x == 0)
										thickness.Left = 0;

									if (x == CALENDAR_COLUMN - 1)
										thickness.Right = 0;

									if (y == CALENDAR_ROW - 1)
										thickness.Bottom = 0;
								}

								pItem.BorderThickness = thickness;
							}
							else
							{
								pItem.BorderThickness = new Thickness (0);
							}
						}
					}
				}
			}
		}

		private void UpdateVerticalAlignment ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				CurrentDate.VerticalAlignment = pAttrib.DateTimeGridVerticalAlignment;

				Sunday.VerticalAlignment = pAttrib.DayWeekGridVerticalAlignment;
				Monday.VerticalAlignment = pAttrib.DayWeekGridVerticalAlignment;
				Tuesday.VerticalAlignment = pAttrib.DayWeekGridVerticalAlignment;
				Wednesday.VerticalAlignment = pAttrib.DayWeekGridVerticalAlignment;
				Thursday.VerticalAlignment = pAttrib.DayWeekGridVerticalAlignment;
				Friday.VerticalAlignment = pAttrib.DayWeekGridVerticalAlignment;
				Saturday.VerticalAlignment = pAttrib.DayWeekGridVerticalAlignment;

				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					pItem.SetVerticalAlignment (pAttrib.DayGridVerticalAlignment);
				}
			}
		}

		private void UpdateAtomFontSize ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				CurrentDate.FontSize = pAttrib.DateTimeGridFontSize;

				Sunday.FontSize = pAttrib.DayWeekGridFontSize;
				Monday.FontSize = pAttrib.DayWeekGridFontSize;
				Tuesday.FontSize = pAttrib.DayWeekGridFontSize;
				Wednesday.FontSize = pAttrib.DayWeekGridFontSize;
				Thursday.FontSize = pAttrib.DayWeekGridFontSize;
				Friday.FontSize = pAttrib.DayWeekGridFontSize;
				Saturday.FontSize = pAttrib.DayWeekGridFontSize;

				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					pItem.NumBlock.FontSize = pAttrib.DayGridFontSize;
				}

			}
		}

		private void UpdateAtomFontColor ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				CurrentDate.Foreground = pAttrib.DateTimeGridFontColor;

				//토요일, 일요일인경우 색상변경 안함
				Sunday.Foreground = Brushes.Red;
				Saturday.Foreground = Brushes.Blue;
				Monday.Foreground = pAttrib.DayWeekGridFontColor;
				Tuesday.Foreground = pAttrib.DayWeekGridFontColor;
				Wednesday.Foreground = pAttrib.DayWeekGridFontColor;
				Thursday.Foreground = pAttrib.DayWeekGridFontColor;
				Friday.Foreground = pAttrib.DayWeekGridFontColor;

				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					if (true == pItem.IsColorChanged && false == pItem.bHoliday) //토요일, 일요일, 공휴일 인경우 색상변경 안함
					{
						pItem.SetForeground (pAttrib.DayGridFontColor);
					}
				}
			}
		}

		private void UpdateAtomFontFamily ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				CurrentDate.FontFamily = pAttrib.DateTimeGridFontFamily;

				Sunday.FontFamily = pAttrib.DayWeekGridFontFamily;
				Monday.FontFamily = pAttrib.DayWeekGridFontFamily;
				Tuesday.FontFamily = pAttrib.DayWeekGridFontFamily;
				Wednesday.FontFamily = pAttrib.DayWeekGridFontFamily;
				Thursday.FontFamily = pAttrib.DayWeekGridFontFamily;
				Friday.FontFamily = pAttrib.DayWeekGridFontFamily;
				Saturday.FontFamily = pAttrib.DayWeekGridFontFamily;

				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					pItem.NumBlock.FontFamily = pAttrib.DayWeekGridFontFamily;
				}
			}
		}

		private void InitEvent ()
		{
			this.PreviewMouseLeftButtonDown += CalendarofAtom_PreviewMouseLeftButtonDown;
			this.MouseLeftButtonDown += CalendarofAtom_MouseLeftButtonDown;

			this.LeftButton.MouseEnter += Button_MouseEnter;
			this.RightButton.MouseEnter += Button_MouseEnter;

			this.LeftButton.MouseLeave += Button_MouseLeave;
			this.RightButton.MouseLeave += Button_MouseLeave;

			this.LeftButton.MouseLeftButtonDown += LeftButton_MouseLeftButtonDown;
			this.RightButton.MouseLeftButtonDown += RightButton_MouseLeftButtonDown;

			this.Border1.MouseLeftButtonDown += Border1_MouseLeftButtonDown;
			this.Border2.MouseLeftButtonDown += Border2_MouseLeftButtonDown;
			this.Border3.MouseLeftButtonDown += Border3_MouseLeftButtonDown;

			this.LostFocus += CalendarofAtom_LostFocus;
			this.Loaded += CalendarofAtom_Loaded;

			this.CurrentDate.MouseLeftButtonDown += CurrentDate_MouseLeftButtonDown;
			this.CurrentDayControl.MouseLeftButtonDown += CurrentDayControl_MouseLeftButtonDown;
		}

		void CurrentDate_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (1 == this.AtomCore.AtomRunMode)
			{
				CalendarAtom pAtomCore = this.AtomCore as CalendarAtom;
				if (null != pAtomCore)
				{
                    // AndroidDateTimePicker pDateTimePicker = new AndroidDateTimePicker ();
                    SmartDatePicker pDateTimePicker = new SmartDatePicker();

                    pDateTimePicker.WindowStartupLocation = WindowStartupLocation.Manual;
					pDateTimePicker.ShowInTaskbar = false;

					pDateTimePicker.Left = GetActualMainWindowLeft (180);
					pDateTimePicker.Top = GetActualMainWindowTop (250);

					// 2024.05.20 beh Dialog에서 일자도 변경할 수 있도록 보강 
					if (0 < Day)
					{
						pDateTimePicker.DateValue = pAtomCore.GetStringDatetime (Year, Month, Day);
					}
					else
					{
						pDateTimePicker.DateValue = pAtomCore.GetStringDatetime (Year, Month, 1);
					}

					if (true == pDateTimePicker.ShowDialog ())
					{
						DateTime pDateTime = DateTime.Parse (pDateTimePicker.DateValue);
						Day = pDateTime.Day;
						pAtomCore.MoveCalendarDate (pDateTime.Year, pDateTime.Month);
						CalendarItem pItem = GetCalendarItem (pDateTime);
						pItem.SetEnterFocus ((SolidColorBrush)(new BrushConverter ().ConvertFrom ("#CCE6E6E6")), Year, Month, Day);
					}
				}
			}
		}

		private void CurrentDayControl_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (1 == this.AtomCore.AtomRunMode)
			{
				CalendarAtom pAtomCore = this.AtomCore as CalendarAtom;
				if (null != pAtomCore)
				{
					DateTime dateNow = DateTime.Now;
					pAtomCore.MoveCalendarDate (dateNow.Year, dateNow.Month);
				}
			}
		}

		private double GetActualMainWindowLeft (double dPickerWidth)
		{
			Window MainWindow = Application.Current.MainWindow;
			PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual (MainWindow);
			System.Windows.Media.Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
			double thisDpiWidthFactor = m.M11;
			double thisDpiHeightFactor = m.M22;

			Point ptMouse = PointToScreen (Mouse.GetPosition (this));

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

		private double GetActualMainWindowTop (double dPickerHeight)
		{

			Window MainWindow = Application.Current.MainWindow;
			PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual (MainWindow);
			System.Windows.Media.Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
			double thisDpiWidthFactor = m.M11;
			double thisDpiHeightFactor = m.M22;

			Point ptMouse = PointToScreen (Mouse.GetPosition (this));

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

		void CalendarofAtom_Loaded (object sender, RoutedEventArgs e)
		{
			this.Loaded -= CalendarofAtom_Loaded;

			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				if (0 == pAttrib.DisplayStartDateType)
				{
					DrawCalendar (DateTime.Now.Year, DateTime.Now.Month, false);
				}
				else
				{
					DrawCalendar (pAttrib.StartYear, pAttrib.StartMonth, false);
				}

				InitAtomAttribStyle ();
			}
		}

		#region 이벤트

		void RightButton_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			RightButtonDown ();
		}

		void LeftButton_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			LeftButtonDown ();
		}

		void Button_MouseLeave (object sender, MouseEventArgs e)
		{
			SetCursor (false);
		}

		void Button_MouseEnter (object sender, MouseEventArgs e)
		{
			SetCursor (true);
		}

		void CalendarofAtom_LostFocus (object sender, RoutedEventArgs e)
		{
			if (0 == this.AtomCore.AtomRunMode)
			{
				//SetBorder (0);
			}
		}

		private void CalendarofAtom_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (0 == this.AtomCore.AtomRunMode)
			{
				SetBorder (0);
                //20250212 KH 캘린더아톰 선택 이후   Line 설정 보강 및 Popup Enable / Disable 오류
                if (BorderIndex == 0)
                {
                    base.SelectAtom();
                }
            }
		}

		void Border1_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (0 == this.AtomCore.AtomRunMode)
			{
				SetBorder (1);
				if (Visibility.Visible == GetResizeAdornerVisibility ()) e.Handled = true;
            }
		}

		void Border2_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (0 == this.AtomCore.AtomRunMode)
			{
				SetBorder (2);
				if (Visibility.Visible == GetResizeAdornerVisibility ()) e.Handled = true;
            }
		}

		void Border3_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (0 == this.AtomCore.AtomRunMode)
			{
				SetBorder (3);
                if (Visibility.Visible == GetResizeAdornerVisibility()) e.Handled = true;
                //20250212 KH 캘린더아톰 선택 이후   Line 설정 보강 및 Popup Enable / Disable 오류
                if (BorderIndex == 3)
				{
                    base.SelectAtom();
                }
            }
		}


		private void SetBorder (int nIndex)
		{
			Border1.BorderThickness = new Thickness (0);
			Border2.BorderThickness = new Thickness (0);
			Border3.BorderThickness = new Thickness (0);

			if (0 != this.AtomCore.AtomRunMode)
				return;

			BorderIndex = nIndex;

            switch (nIndex)
			{
				case 1:
					Border1.BorderThickness = new Thickness (1);
					break;
				case 2:
					Border2.BorderThickness = new Thickness (1);
					break;
				case 3:
					Border3.BorderThickness = new Thickness (1);
					break;
			}
		}

		public void RightButtonDown ()
		{
			PreviewStartAnimationControl (true);

			DeleteCalendarData ();

			m_nMonth++;
			if (12 < m_nMonth)
			{
				m_nMonth -= 12;
				m_nYear++;
			}

			CalendarAtom pAtomCore = this.AtomCore as CalendarAtom;
			if (null != pAtomCore)
			{
				pAtomCore.LoadSchedule (m_nYear, m_nMonth);
			}

			DrawCalendar (m_nYear, m_nMonth, true);

			StartAnimationControl ();

			OnNotifyChangeEvent ();
		}

		public void LeftButtonDown ()
		{
			PreviewStartAnimationControl (false);

			DeleteCalendarData ();

			m_nMonth--;
			if (0 == m_nMonth)
			{
				m_nMonth = 12;
				m_nYear--;
			}

			CalendarAtom pAtomCore = this.AtomCore as CalendarAtom;
			if (null != pAtomCore)
			{
				pAtomCore.LoadSchedule (m_nYear, m_nMonth);
			}

			DrawCalendar (m_nYear, m_nMonth, true);

			StartAnimationControl ();

			OnNotifyChangeEvent ();
		}

		private void SetCursor (bool bEnter)
		{
			if (true == bEnter)
			{
				this.Cursor = Cursors.Hand;
			}
			else
			{
				this.Cursor = Cursors.Arrow;
			}

		}

		void CalendarofAtom_PreviewMouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			//임시
		}

		#endregion

		private void DeleteCalendarData ()
		{
			foreach (CalendarScheduleData pData in m_CalendarScheduleDataList)
			{
				pData.ClearTextBlock ();
			}

			m_CalendarScheduleDataList.Clear ();
		}


		public void DrawCalendar ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				DrawCalendar (m_nYear, m_nMonth, true);
			}
		}

		public void DrawCalendar (int nYear, int nMonth, bool SetEvent)
		{
			SetDrawDisplayType ();
			SetDrawLanguageType ();
			SetDrawDateTimeGrid ();

			if (0 >= nYear)
			{
				nYear = DateTime.Now.Year;
			}
			if (0 >= nMonth)
			{
				nMonth = DateTime.Now.Month;
			}
			else if (12 < nMonth)
			{
				int temp = (int)(nMonth / 12);
				nMonth = nMonth % 12;
				nYear += temp;
			}

			if (null == m_CalendarItemList)
			{
				m_CalendarItemList = new List<CalendarItem> ();
			}

			m_CalendarItemList.Clear ();
			MainGrid.Children.Clear ();

			int nDay = 1; //달력을 그리기 위해서는 현재달의 요일을 가져와야 하기 때문에

			DateTime pDate = new DateTime (nYear, nMonth, nDay); //화면에 표시해야 되는 달의 일자
			int nStart = (int)pDate.DayOfWeek;
			int nDayCount = DateTime.DaysInMonth (nYear, nMonth);

			m_nYear = nYear;
			m_nMonth = nMonth;

			string strCurrentDate = string.Empty;

			if (LC.LANG.KOREAN == LC.PQLanguage || LC.LANG.JAPAN == LC.PQLanguage)
			{
				if (10 > m_nMonth)
				{
					strCurrentDate = string.Format ("{0}{1}  {2}{3}", m_nYear, STR_YEAR, m_nMonth, STR_MONTH); //0년  0월
				}
				else
				{
					strCurrentDate = string.Format ("{0}{1} {2}{3}", m_nYear, STR_YEAR, m_nMonth, STR_MONTH); //0년 0월
				}
			}
			else
			{
				System.Globalization.CultureInfo en = new System.Globalization.CultureInfo ("en-US");
				strCurrentDate = string.Format ("{0} {1}", m_nYear, pDate.ToString ("MMM", en));
			}

			CurrentDate.Text = strCurrentDate;

			CreateCalendarItem (nYear, nMonth, nStart, nDayCount); //생성
			UpdateDayBorderThickness ();

			if (true == SetEvent)
			{
				SetCalendarItemEvent ();
			}
			else
			{
				DeleteCalendarItemEvent ();
			}

			DrawCalendarSchedule ();
		}

		private void CreateCalendarItem (int nYear, int nMonth, int nStart, int nDayCount)
		{
			CalendarAtom pAtomCore = this.AtomCore as CalendarAtom;
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			int nDisplayType = pAttrib.DisplayDayWeekType;

			int nDrawType = 0; // 0 - 이전달 , 1 - 현재, 2 - 다음
			int nDrawCount = 1;

			int nBeforeYear = nYear - 1;
			int nBeforeMonth = nMonth - 1;

			int nAfterYear = nYear + 1;
			int nAfterMonth = nMonth + 1;

			if (1 > nBeforeMonth)
			{
				nBeforeYear--;
				nBeforeMonth = 12;
			}

			if (12 < nAfterMonth)
			{
				nAfterYear++;
				nAfterMonth = 1;
			}

			int nBeforeMaxDay = DateTime.DaysInMonth (nBeforeYear, nBeforeMonth);
			int nAfterDay = 1;

			for (int y = 0; y < CALENDAR_ROW; y++)
			{
				for (int x = 0; x < CALENDAR_COLUMN; x++)
				{
					int nIndex = x + (y * CALENDAR_COLUMN);
					CalendarItem pItem = new CalendarItem ();

					pItem.Margin = new Thickness (0);
					pItem.SetBackgroundColor (this.AtomBorder.Background);

					pItem.BorderBrush = pAttrib.DayBorderBrushColor;

					pItem.SetHorizontalAlignment (pAttrib.DayGridHorizontalAlignment);
					pItem.SetVerticalAlignment (pAttrib.DayGridVerticalAlignment);
					pItem.NumBlock.Foreground = pAttrib.DayGridFontColor;

					if (0 < pAttrib.DayGridFontSize)
					{
						pItem.NumBlock.FontSize = pAttrib.DayGridFontSize;
						pItem.NumBlock2.FontSize = Math.Round (pAttrib.DayGridFontSize * 0.6);
					}

					if (null != pAttrib.DayGridFontFamily)
					{
						pItem.NumBlock.FontFamily = pAttrib.DayGridFontFamily;
					}

					Grid.SetColumn (pItem, x);
					Grid.SetRow (pItem, y);

					int nSaturyday = -1;
					int nSunday = -1;

					if (0 == nDisplayType)
					{
						nSaturyday = 6;
						nSunday = 0;
					}
					else if (1 == nDisplayType)
					{
						nSaturyday = 5;
						nSunday = 6;
					}

					if (nSunday == x) //일요일
					{
						pItem.SetForeground (Brushes.Red);
						pItem.IsColorChanged = false;
					}

					else if (nSaturyday == x) //토요일
					{
						pItem.SetForeground (Brushes.Blue);
						pItem.IsColorChanged = false;
					}

					if (0 == nDisplayType)
					{
						//일-월-화-수-목-금-토-일
						if (nStart == nIndex) //같을경우 그리기 시작
						{
							nDrawType = 1;
						}
					}
					else if (1 == nDisplayType)
					{
						//월-화-수-목-금-토-일 순으로 표시되어 있음

						//nStart는 일~토 기준 nIndex는 월~일 기준으로 되어있음
						if (0 < nStart) //시작일이 월~금 일때
						{
							if (nStart - 1 == nIndex)
							{
								nDrawType = 1;
							}
						}
						else if (0 == nStart && 6 == nIndex)
						{
							nDrawType = 1;
						}
					}

					if (0 == nDrawType)
					{
						int nBeforeDay = (nBeforeMaxDay - (nStart - nIndex)) + 1; //1Base표기를 위해서

						pItem.Year = nBeforeYear;
						pItem.Month = nBeforeMonth;
						pItem.Day = nBeforeDay;
						pItem.DayWeek = x;
						pItem.DisplayType = nDrawType;
					}
					else if (1 == nDrawType)
					{
						pItem.Year = nYear;
						pItem.Month = nMonth;
						pItem.Day = nDrawCount;
						pItem.DayWeek = x;

						pItem.OnClickEvent += OnNotifyClickEvent; //일자가 지정된 칸만 클릭이벤트 동작한다.

						if (nYear == DateTime.Now.Year && nMonth == DateTime.Now.Month && nDrawCount == DateTime.Now.Day) //달력이 오늘날짜인경우
						{
							//pItem.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#CC036EDC"));
							pItem.SerCurrentDay ();
						}

						string strDate = pAtomCore.GetStringDatetime (nYear, nMonth, nDrawCount);

						if (true == m_RunModeFlagMap.ContainsKey (strDate))
						{
							List<Brush> pList = m_RunModeFlagMap[strDate];

							if (1 < pList.Count)
							{
								pItem.SetForeground (pList[0]);
								pItem.SetBackgroundColor (pList[1]);
							}
						}

						nDrawCount++;

						if (nDrawCount > nDayCount)
						{
							nDrawType = 2;
						}
					}
					else if (2 == nDrawType)
					{
						pItem.Year = nAfterYear;
						pItem.Month = nAfterMonth;
						pItem.Day = nAfterDay;
						pItem.DayWeek = x;
						pItem.DisplayType = nDrawType;
						nAfterDay++;
					}

					m_CalendarItemList.Add (pItem);
					this.MainGrid.Children.Add (pItem);
				}
			}
		}

		private void OnNotifyClickEvent (object sender)
		{
			OnNotifyClickEvent (sender, true);
		}

		private void OnNotifyClickEvent (object sender, bool IsCallEvent)
		{
			var pAtomCore = this.AtomCore as CalendarAtom;
			CalendarAttrib pAttrib = pAtomCore.GetAttrib () as CalendarAttrib;
			CalendarItem pItem = sender as CalendarItem;

			if (null != pItem && null != pAtomCore && null != pAttrib)
			{
				Year = pItem.Year;
				Month = pItem.Month;
				Day = pItem.Day;

				if (null != m_TempItem)
				{
					//m_TempItem.SelectBorder.BorderThickness = new Thickness(0);
					//m_TempItem.Background = m_TempBrush;
					m_TempItem.SetLeaveFocus (m_TempItem.ItemBackGround);
				}

				//pItem.SelectBorder.BorderThickness = new Thickness(1.5);
				m_TempItem = pItem;
				//pItem.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#CCE6E6E6"));

				pItem.SetEnterFocus ((SolidColorBrush)(new BrushConverter ().ConvertFrom ("#CCE6E6E6")), Year, Month, Day);

				int nRow = Grid.GetRow (pItem);
				int nColumn = Grid.GetColumn (pItem);

				string strDate = pAtomCore.GetStringDatetime (pItem.Year, pItem.Month, pItem.Day);

				if (IsCallEvent)
				{
					CVariantX[] pvaArgs = new CVariantX[1 + 1];
					pvaArgs[0] = new CVariantX (1);
					pvaArgs[1] = new CVariantX (strDate);

					if (-1 != AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_CLICK, pvaArgs))
					{
						if (0 <= MsgHandler.CALL_MSG_HANDLER (AtomCore, EVS_TYPE.EVS_A_CLICK, pvaArgs))
						{
							AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_CLICK, pvaArgs);
						}
					}
				}
			}
		}

		public void OnNotifyChangeEvent ()
		{
			CVariantX[] pvaArgs = new CVariantX[1 + 2];
			pvaArgs[0] = new CVariantX (2);
			pvaArgs[1] = new CVariantX (m_nYear);
			pvaArgs[2] = new CVariantX (m_nMonth);

			if (-1 != AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_SCHEDULE_CHANGE, pvaArgs))
			{
				if (0 <= MsgHandler.CALL_MSG_HANDLER (this.AtomCore, EVS_TYPE.EVS_A_SCHEDULE_CHANGE, pvaArgs))
				{
					AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_SCHEDULE_CHANGE, pvaArgs);
				}
			}
		}

		protected override void InitializeAtomCore ()
		{
			m_AtomCore = new CalendarAtom ();
		}

		protected override void InitializeResizeAdorner ()
		{
			m_ResizeAdorner = new Point8Adorner (this);
		}

		protected override void InitializeAtomSize ()
		{
			Size atomSize = DefaultAtomSizeManager.GetDefaultRect (AtomType.Calendar);
			this.Width = atomSize.Width;
			this.Height = atomSize.Height;
		}

		public override void SetAtomShowBorder (bool bUseLine)
		{
			if (3 == BorderIndex)
			{
				//3인경우 선없을을 설정해도 pen이 아닌 Attrib에 데이터 저장되기 때문에 별도로 처리하지 않는다.
			}
			else
			{
				base.SetAtomShowBorder (bUseLine);
			}
		}

		#region | Atom UI Property |

		#region | Border Brush |

		public override void SetAtomBorder (Brush applyBrush)
		{
			if (3 == BorderIndex)
			{
				SetBorderCalendarItem (applyBrush);
			}
			else
			{
				this.AtomBorder.BorderBrush = applyBrush;
			}
		}

		public void SetBorderCalendarItem (Brush applyBrush)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayBorderBrushColor = applyBrush;
				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					pItem.BorderBrush = applyBrush;
				}
			}
		}

		public override Brush GetAtomBorder ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				if (3 == BorderIndex)
				{
					return pAttrib.DayBorderBrushColor;
				}
				else
				{
					return this.AtomBorder.BorderBrush;
				}
			}

			return base.GetAtomBorder ();
		}

		#endregion

		#region | AtomBackground |

		public override void SetAtomBackground (Brush applyBrush)
		{
			AtomBorder.Background = applyBrush;

			Border1.Background = applyBrush;
			Border2.Background = applyBrush;
			Border3.Background = applyBrush;

			SetColorCalendarItem (applyBrush);
			SetColorDayofWeekTextBlock (applyBrush);
		}

		public void SetColorCalendarItem (Brush pBrush)
		{

			if (null != m_CalendarItemList)
			{
				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					pItem.SetBackgroundColor (pBrush);
				}
			}
		}

		public void SetColorDayofWeekTextBlock (Brush pBrush)
		{
			Sunday.Background = pBrush;
			Monday.Background = pBrush;
			Tuesday.Background = pBrush;
			Wednesday.Background = pBrush;
			Thursday.Background = pBrush;
			Friday.Background = pBrush;
			Saturday.Background = pBrush;
		}

		public override Brush GetAtomBackground ()
		{
			return AtomBorder.Background;
		}

		#endregion

		#region | HorizontalAlignment |

		public override void SetHorizontalTextAlignment (HorizontalAlignment applyHorizontalTextAlignment)
		{
			switch (BorderIndex)
			{
				case 1:
					SetHorizontalTextDateTimeGrid (applyHorizontalTextAlignment);
					break;
				case 2:
					SetHorizontalTextDayofWeek (applyHorizontalTextAlignment);
					break;
				default:
					SetHorizontalTextCalendarItem (applyHorizontalTextAlignment);
					break;
			}
		}

		public void SetHorizontalTextDateTimeGrid (HorizontalAlignment applyHorizontalTextAlignment)
		{
			CalendarAttrib pAtomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAtomAttrib)
			{
				pAtomAttrib.DateTimeGridHorizontalAlignment = applyHorizontalTextAlignment;
				DateTimeGrid.HorizontalAlignment = applyHorizontalTextAlignment;
			}
		}

		public void SetHorizontalTextDayofWeek (HorizontalAlignment applyHorizontalTextAlignment)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayWeekGridHorizontalAlignment = applyHorizontalTextAlignment;

				Sunday.HorizontalAlignment = applyHorizontalTextAlignment;
				Monday.HorizontalAlignment = applyHorizontalTextAlignment;
				Tuesday.HorizontalAlignment = applyHorizontalTextAlignment;
				Wednesday.HorizontalAlignment = applyHorizontalTextAlignment;
				Thursday.HorizontalAlignment = applyHorizontalTextAlignment;
				Friday.HorizontalAlignment = applyHorizontalTextAlignment;
				Saturday.HorizontalAlignment = applyHorizontalTextAlignment;
			}
		}

		public void SetHorizontalTextCalendarItem (HorizontalAlignment applyHorizontalTextAlignment)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayGridHorizontalAlignment = applyHorizontalTextAlignment;

				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					pItem.SetHorizontalAlignment (applyHorizontalTextAlignment);
				}
			}
		}

		public override HorizontalAlignment GetHorizontalTextAlignment ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				switch (BorderIndex)
				{
					case 1: return pAttrib.DateTimeGridHorizontalAlignment;
					case 2: return pAttrib.DayWeekGridHorizontalAlignment;
					default: return pAttrib.DayGridHorizontalAlignment;
				}
			}

			return base.GetHorizontalTextAlignment ();
		}

		#endregion

		#region | AtomThickness |

		public override void SetAtomThickness (Thickness applyThickness)
		{
			if (3 == BorderIndex)
			{
				SetThicknessCalendarItem (applyThickness);
			}
			else
			{
				CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib; //2개의 border속성을 관리해야 되기 때문에 별도로 처리해준다.

				if (null != pAttrib)
				{
					AtomBorder.BorderThickness = applyThickness;

					if (Kiss.DoubleEqual (0, applyThickness.Left) && Kiss.DoubleEqual (0, applyThickness.Right) &&
						Kiss.DoubleEqual (0, applyThickness.Top) && Kiss.DoubleEqual (0, applyThickness.Bottom))
					{
						pAttrib.PenFill = false;
					}
					else
					{
						pAttrib.PenFill = true;
					}
				}
			}

			UpdateDayBorderThickness ();
		}

		public void SetThicknessCalendarItem (Thickness applyThickness)
		{
			CalendarAttrib pAtomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAtomAttrib)
			{
				Thickness pThickness = new Thickness ();

				if (0 < applyThickness.Left)
				{
					pAtomAttrib.DisplayDayBorderThickness = true;
					pThickness = new Thickness (0.5);
				}
				else
				{
					pAtomAttrib.DisplayDayBorderThickness = false;
					pThickness = new Thickness (0);
				}
			}
		}

		public override Thickness GetAtomThickness ()
		{
			if (3 == BorderIndex)
			{
				return GetThicknessCalendarItem ();
			}

			return AtomBorder.BorderThickness;
		}

		public Thickness GetThicknessCalendarItem ()
		{
			if (null != m_CalendarItemList && m_CalendarItemList.FirstOrDefault () is var Item)
			{
				double max = Math.Max (
					Math.Max (Item.BorderThickness.Left, Item.BorderThickness.Top),
					Math.Max (Item.BorderThickness.Right, Item.BorderThickness.Bottom));
				
				return new Thickness (max);
			}

			return new Thickness (0);
		}

		#endregion

		#region | VerticalAlignment |

		public override void SetVerticalTextAlignment (VerticalAlignment applyVerticalTextAlignment)
		{
			switch (BorderIndex)
			{
				case 1:
					SetVerticalDateTimeGrid (applyVerticalTextAlignment);
					break;
				case 2:
					SetVerticalTextDayofWeek (applyVerticalTextAlignment);
					break;
				default:
					SetVertiCalTextCalendarItem (applyVerticalTextAlignment);
					break;
			}
		}

		public void SetVerticalDateTimeGrid (VerticalAlignment applyVerticalTextAlignment)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DateTimeGridVerticalAlignment = applyVerticalTextAlignment;
				CurrentDate.VerticalAlignment = applyVerticalTextAlignment;
			}
		}

		public void SetVerticalTextDayofWeek (VerticalAlignment applyVerticalTextAlignment)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayWeekGridVerticalAlignment = applyVerticalTextAlignment;
				Sunday.VerticalAlignment = applyVerticalTextAlignment;
				Monday.VerticalAlignment = applyVerticalTextAlignment;
				Tuesday.VerticalAlignment = applyVerticalTextAlignment;
				Wednesday.VerticalAlignment = applyVerticalTextAlignment;
				Thursday.VerticalAlignment = applyVerticalTextAlignment;
				Friday.VerticalAlignment = applyVerticalTextAlignment;
				Saturday.VerticalAlignment = applyVerticalTextAlignment;
			}
		}

		public void SetVertiCalTextCalendarItem (VerticalAlignment applyVerticalTextAlignment)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayGridVerticalAlignment = applyVerticalTextAlignment;

				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					pItem.SetVerticalAlignment (applyVerticalTextAlignment);
				}
			}
		}

		public override VerticalAlignment GetVerticalTextAlignment ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				switch (BorderIndex)
				{
					case 1: return pAttrib.DateTimeGridVerticalAlignment;
					case 2: return pAttrib.DayWeekGridVerticalAlignment;
					default: return pAttrib.DayGridVerticalAlignment;
				}
			}

			return base.GetVerticalTextAlignment ();
		}

		#endregion

		#region | AtomFontSize |

		public override void SetAtomFontSize (double dApplySize)
		{
			switch (BorderIndex)
			{
				case 1:
					SetFontSizeDateTimeGrid (dApplySize);
					break;
				case 2:
					SetFontSizeDayofWeek (dApplySize);
					break;
				default:
					SetFontSizeCalendarItem (dApplySize);
					break;
			}
		}

		public void SetFontSizeDateTimeGrid (double dApplySize)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DateTimeGridFontSize = dApplySize;
				CurrentDate.FontSize = dApplySize;
			}
		}

		public void SetFontSizeDayofWeek (double dApplySize)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayWeekGridFontSize = dApplySize;

				Sunday.FontSize = dApplySize;
				Monday.FontSize = dApplySize;
				Tuesday.FontSize = dApplySize;
				Wednesday.FontSize = dApplySize;
				Thursday.FontSize = dApplySize;
				Friday.FontSize = dApplySize;
				Saturday.FontSize = dApplySize;
			}
		}

		public void SetFontSizeCalendarItem (double dApplySize)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayGridFontSize = dApplySize;

				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					pItem.NumBlock.FontSize = dApplySize;
					pItem.NumBlock2.FontSize = Math.Round (dApplySize * 0.6);
				}
			}

			DrawCalendar ();
		}

		public override double GetAtomFontSize ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				switch (BorderIndex)
				{
					case 1: return pAttrib.DateTimeGridFontSize;
					case 2: return pAttrib.DayWeekGridFontSize;
					default: return pAttrib.DayGridFontSize;
				}
			}

			return base.GetAtomFontSize ();
		}

		#endregion


		#region | AtomFontFamily |

		public override void SetAtomFontFamily (FontFamily applyFontFamily)
		{
			switch (BorderIndex)
			{
				case 1:
					SetFontFamilyDateTimeGrid (applyFontFamily);
					break;
				case 2:
					SetFontFamilyDayofWeek (applyFontFamily);
					break;
				default:
					SetFontFamilyCalendarItem (applyFontFamily);
					break;
			}

			//base.SetAtomFontFamily(applyFontFamily);
		}

		public void SetFontFamilyDateTimeGrid (FontFamily applyFontFamily)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DateTimeGridFontFamily = applyFontFamily;
				CurrentDate.FontFamily = applyFontFamily;
			}
		}

		public void SetFontFamilyDayofWeek (FontFamily applyFontFamily)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayWeekGridFontFamily = applyFontFamily;

				Sunday.FontFamily = applyFontFamily;
				Monday.FontFamily = applyFontFamily;
				Tuesday.FontFamily = applyFontFamily;
				Wednesday.FontFamily = applyFontFamily;
				Thursday.FontFamily = applyFontFamily;
				Friday.FontFamily = applyFontFamily;
				Saturday.FontFamily = applyFontFamily;
			}
		}

		public void SetFontFamilyCalendarItem (FontFamily applyFontFamily)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayGridFontFamily = applyFontFamily;

				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					pItem.NumBlock.FontFamily = applyFontFamily;
				}
			}
		}

		public override FontFamily GetAtomFontFamily ()
		{
			CalendarAttrib pAtomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAtomAttrib)
			{
				switch (BorderIndex)
				{
					case 1: return pAtomAttrib.DateTimeGridFontFamily;
					case 2: return pAtomAttrib.DayWeekGridFontFamily;
					default: return pAtomAttrib.DayGridFontFamily;
				}
			}

			return base.GetAtomFontFamily ();
		}

		#endregion

		#region | AtomFontColor |

		public override void SetAtomFontColor (Brush applyBrush)
		{
			switch (BorderIndex)
			{
				case 1:
					SetFontColorDateTimeGrid (applyBrush);
					break;
				case 2:
					SetFontColorDayofWeek (applyBrush);
					break;
				default:
					SetFontColorCalendarItem (applyBrush);
					break;
			}
		}

		public void SetFontColorDateTimeGrid (Brush applyBrush)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DateTimeGridFontColor = applyBrush;
				CurrentDate.Foreground = applyBrush;
			}
		}

		public void SetFontColorDayofWeek (Brush applyBrush)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayWeekGridFontColor = applyBrush;

				//토요일, 일요일인경우 색상변경 안함
				//Sunday.Foreground = applyBrush;
				//Saturday.Foreground = applyBrush;
				Monday.Foreground = applyBrush;
				Tuesday.Foreground = applyBrush;
				Wednesday.Foreground = applyBrush;
				Thursday.Foreground = applyBrush;
				Friday.Foreground = applyBrush;
			}
		}

		public void SetFontColorCalendarItem (Brush applyBrush)
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				pAttrib.DayGridFontColor = applyBrush;

				foreach (CalendarItem pItem in m_CalendarItemList)
				{
					if (true == pItem.IsColorChanged && false == pItem.bHoliday) //토요일, 일요일, 공휴일 인경우 색상변경 안함
					{
						pItem.SetForeground (applyBrush);
					}
				}
			}
		}

		public override Brush GetAtomFontColor ()
		{
			CalendarAttrib pAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAttrib)
			{
				switch (BorderIndex)
				{
					case 1: return pAttrib.DateTimeGridFontColor;
					case 2: return pAttrib.DayWeekGridFontColor;
					default: return pAttrib.DayGridFontColor;
				}
			}

			return base.GetAtomFontColor ();
		}

		#endregion

		#endregion

		public override void CompletePropertyChanged ()
		{
			CalendarAttrib pAtomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != pAtomAttrib)
			{
				DrawCalendar (pAtomAttrib.StartYear, pAtomAttrib.StartMonth, false);
			}
		}

		private void SetDrawLanguageType ()
		{
			CalendarAttrib pAtomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;
			switch (pAtomAttrib.LanguageType)
			{
				case 0:
					Sunday.Text = "일";
					Monday.Text = "월";
					Tuesday.Text = "화";
					Wednesday.Text = "수";
					Thursday.Text = "목";
					Friday.Text = "금";
					Saturday.Text = "토";
					break;
				case 1:
					Sunday.Text = "Sun";
					Monday.Text = "Mon";
					Tuesday.Text = "Tue";
					Wednesday.Text = "Wed";
					Thursday.Text = "Thu";
					Friday.Text = "Fri";
					Saturday.Text = "Sat";
					break;
				case 2:
					Sunday.Text = "日";
					Monday.Text = "月";
					Tuesday.Text = "火";
					Wednesday.Text = "水";
					Thursday.Text = "木";
					Friday.Text = "金";
					Saturday.Text = "土";
					break;
			}
		}

		private void SetDrawDisplayType ()
		{
			CalendarAttrib pAtomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			int nIndex = pAtomAttrib.DisplayDayWeekType;

			if (0 == nIndex)
			{
				Grid.SetColumn (Sunday, 0);
				Grid.SetColumn (Monday, 1);
				Grid.SetColumn (Tuesday, 2);
				Grid.SetColumn (Wednesday, 3);
				Grid.SetColumn (Thursday, 4);
				Grid.SetColumn (Friday, 5);
				Grid.SetColumn (Saturday, 6);
			}
			else if (1 == nIndex)
			{
				Grid.SetColumn (Monday, 0);
				Grid.SetColumn (Tuesday, 1);
				Grid.SetColumn (Wednesday, 2);
				Grid.SetColumn (Thursday, 3);
				Grid.SetColumn (Friday, 4);
				Grid.SetColumn (Saturday, 5);
				Grid.SetColumn (Sunday, 6);
			}
		}

		private void SetDrawDateTimeGrid ()
		{
			CalendarAttrib pAtomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;
			if (null != pAtomAttrib)
			{
				if (false == pAtomAttrib.HideDateTimeGrid) //표시
				{
					if (3 < LayoutGrid.RowDefinitions.Count)
					{
						LayoutGrid.RowDefinitions[0].Height = new GridLength (0.1, GridUnitType.Star);
						LayoutGrid.RowDefinitions[1].Height = new GridLength (1.5, GridUnitType.Star);
						Border1.Visibility = Visibility.Visible;
					}
				}
				else //숨김
				{
					if (3 < LayoutGrid.RowDefinitions.Count)
					{
						LayoutGrid.RowDefinitions[0].Height = new GridLength (0, GridUnitType.Pixel);
						LayoutGrid.RowDefinitions[1].Height = new GridLength (0, GridUnitType.Pixel);
						Border1.Visibility = Visibility.Collapsed;
					}
				}
			}
		}

		public override void SetResizeAdornerVisibility (Visibility isVisible, bool bIsRoutedChildren)
		{
			base.SetResizeAdornerVisibility (isVisible, bIsRoutedChildren);

			if (isVisible == Visibility.Collapsed)
			{
				SetBorder (0);
			}
		}

		public override void ChangeAtomMode (int nRunMode)
		{
			CalendarAtom pAtomCore = this.AtomCore as CalendarAtom;
			CalendarAttrib pAtomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (0 == nRunMode) //편집모드
			{
				m_CalendarScheduleDataList.Clear ();
				m_CalendarScheduleInsertList.Clear ();
				m_CalendarScheduleUpDateList.Clear ();
				m_CalendarScheduleDeleteList.Clear ();

				LeftButton.IsEnabled = false;
				RightButton.IsEnabled = false;

				m_RunModeFlagMap.Clear ();

				DrawCalendar (pAtomAttrib.StartYear, pAtomAttrib.StartMonth, false);
				DeleteCalendarItemEvent ();

				DropAnimationControl ();
			}
			else if (1 == nRunMode)
			{
				BorderIndex = 0;
				//pAtomCore.LoadSchedule(m_nYear, m_nMonth);
				SetCalendarItemEvent ();
				LeftButton.IsEnabled = true;
				RightButton.IsEnabled = true;

				DrawCalendar (pAtomAttrib.StartYear, pAtomAttrib.StartMonth, true);
			}

			base.ChangeAtomMode (nRunMode);
		}

		#region | Get, Set CalendarScheduleData UI Property |

		public void SetCalendarScheduleData_Background (string strTitle, Brush pBrush)
		{
			CalendarScheduleData pData = GetCalendarScheduleData (strTitle);
			if (null != pData)
			{
				if (true == pData.DBLoad && false == pData.Background.Equals (pBrush))
				{
					CalendarScheduleUpDateList.Add (pData);
				}

				pData.Background = pBrush;
			}
		}

		public void SetCalendarScheduleData_Foreground (string strTitle, Brush pBrush)
		{
			CalendarScheduleData pData = GetCalendarScheduleData (strTitle);
			if (null != pData)
			{
				if (true == pData.DBLoad && false == pData.Foreground.Equals (pBrush))
				{
					CalendarScheduleUpDateList.Add (pData);
				}

				pData.Foreground = pBrush;
			}
		}

		#endregion

		#region | Get, Set SetCalendarItem UI Property |

		public void SetCalendarItem_Background (string strDate, Brush pBrush)
		{
			CalendarItem pData = GetCalendarItem (strDate);
			if (null != pData)
			{
				pData.SetBackgroundColor (pBrush);

				//선택된 날짜의 배경색을 변경한경우 tempitem을 null처리 한다. 다른 날짜를 선택할경우 변경된 배경색이 날라가기 때문에
				if (m_TempItem == pData)
				{
					m_TempItem = null;
				}
			}
		}

		public void SetCalendarItem_Foreground (string strDate, Brush pBrush)
		{
			CalendarItem pData = GetCalendarItem (strDate);
			if (null != pData)
			{
				pData.SetForeground (pBrush);
			}
		}

		public void SetCalendarItem_Holiday (string strDate)
		{
			CalendarItem pData = GetCalendarItem (strDate);
			if (null != pData)
			{
				pData.bHoliday = true;
				pData.SetForeground (Brushes.Red);
			}
		}

		#endregion

		#region | Get CalendarItem |

		public CalendarItem GetCalendarItem (DateTime pDate)
		{
			return GetCalendarItem (pDate.Year, pDate.Month, pDate.Day);
		}

		public CalendarItem GetCalendarItem (int nYear, int nMonth, int nDay)
		{
			foreach (CalendarItem pItem in m_CalendarItemList)
			{
				if (nYear == pItem.Year && nMonth == pItem.Month && nDay == pItem.Day)
				{
					return pItem;
				}
			}

			return null;
		}

		public CalendarItem GetCalendarItem (string strDate)
		{
			CalendarAtom pAtomCore = this.AtomCore as CalendarAtom;

			if (null != pAtomCore && false == string.IsNullOrEmpty (strDate))
			{
				DateTime pDate = pAtomCore.GetDateParse (strDate);
				return GetCalendarItem (pDate);
			}

			return null;
		}

		#endregion

		#region | Get CalendarScheduleData |

		public CalendarScheduleData GetCalendarScheduleData (string strTitle)
		{
			if (null != CalendarScheduleData && 0 < CalendarScheduleData.Count)
			{
				foreach (CalendarScheduleData pData in CalendarScheduleData)
				{
					if (true == pData.Title.Equals (strTitle))
					{
						return pData;
					}
				}
			}

			return null;
		}

		#endregion

		public void SetCalendarItemEvent ()
		{
			foreach (CalendarItem pItem in m_CalendarItemList)
			{
				pItem.SetEvent ();
			}
		}

		public void DeleteCalendarItemEvent ()
		{
			foreach (CalendarItem pItem in m_CalendarItemList)
			{
				pItem.DeleteEvent ();
			}
		}

		public void AddCalendarScheduleData (string strTitle, string strStartDate, string strEndDate, string strContent, Color pBackColor, Color pFontColor) //업무규칙으로 추가시 사용
		{
			CalendarScheduleData pData = GetCalendarScheduleData (strTitle);
			bool bAdd = false;

			if (null == pData)
			{
				bAdd = true;
				pData = new CalendarScheduleData ();
			}

			pData.StartDate = strStartDate;
			pData.EndDate = strEndDate;
			pData.Title = strTitle;
			pData.Content = strContent;
			pData.Background = new SolidColorBrush (pBackColor);
			pData.Foreground = new SolidColorBrush (pFontColor);

			if (true == bAdd)
			{
				m_CalendarScheduleDataList.Add (pData);
				m_CalendarScheduleInsertList.Add (pData);
			}
			else
			{
				if (true == pData.DBLoad)
				{
					if (false == m_CalendarScheduleUpDateList.Contains (pData))
					{
						m_CalendarScheduleUpDateList.Add (pData);
					}
				}
			}
		}

		public void DeleteSchedule ()
		{
			m_CalendarScheduleDataList.Clear ();
		}

		public void AddDBCalendarScheduleData (int nSerialnum, string strStartDate, string strEndDate, string strTitle, string strContent, Color pBackColor, Color pFontColor) //DB로드시에만 사용
		{
			CalendarScheduleData pData = GetCalendarScheduleData (strTitle);

			bool bAdd = false;

			if (null == pData)
			{
				CalendarScheduleData pTempData = GetCalendarScheduleData (strTitle); //삭제처리된 일정인경우 화면에 표시하지 않음
				if (null != pTempData)
				{
					return;
				}

				bAdd = true;
				pData = new CalendarScheduleData ();
			}

			pData.Serialnum = nSerialnum;
			pData.StartDate = strStartDate;
			pData.EndDate = strEndDate;
			pData.Title = strTitle;
			pData.Content = strContent;
			pData.DBLoad = true;
			pData.Background = new SolidColorBrush (pBackColor);
			pData.Foreground = new SolidColorBrush (pFontColor);

			if (true == bAdd)
			{
				m_CalendarScheduleDataList.Add (pData);
			}
		}

		public void DeleteCalendarScheduleData (string strTitle)
		{
			CalendarScheduleData pData = GetCalendarScheduleData (strTitle);

			if (null != pData)
			{
				if (true == pData.DBLoad)
				{
					m_CalendarScheduleDeleteList.Add (pData);
				}

				List<Label> pList = pData.LabelList;

				if (null != pList)
				{
					for (int i = 0; i < pList.Count; i++)
					{
						Label pTempBlock = pList[i];

						MainGrid.Children.Remove (pTempBlock);
					}
				}

				m_CalendarScheduleDataList.Remove (pData);
			}
		}

		public void DrawCalendarSchedule ()
		{
			CalendarAtom atomCore = this.AtomCore as CalendarAtom;
			CalendarAttrib atomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null == atomCore || null == atomAttrib)
				return;

			List<CalendarScheduleData> pDrawList = new List<CalendarScheduleData> ();

			foreach (CalendarScheduleData pitem in m_CalendarScheduleDataList)
			{
				DateTime pStartDate = atomCore.GetDateParse (pitem.StartDate);
				DateTime pEndDate = atomCore.GetDateParse (pitem.EndDate);

				if (null != pitem.LabelList)
				{
					foreach (Label pBlock in pitem.LabelList)
					{
						MainGrid.Children.Remove (pBlock);
					}
					pitem.ClearTextBlock ();
				}

				if (pStartDate.Year <= m_nYear && m_nYear <= pEndDate.Year)
				{
					pDrawList.Add (pitem); //2023.07.03 beh 일정선택 이벤트에서 일정 시작일, 종료일이 DB와 다르게 나오는 현상 있어 시작일, 종료일 현재달의 1일, 마지막일로 설정하는 논리 제거

					//if (m_nMonth == pStartDate.Month && m_nMonth == pEndDate.Month)
					//{
					//	pDrawList.Add(pitem);
					//}
					//else if (m_nMonth == pStartDate.Month)
					//{
					//	CalendarScheduleData pTempData = new CalendarScheduleData(pitem);

					//	int nDay = DateTime.DaysInMonth(pStartDate.Year, pStartDate.Month);

					//	pTempData.EndDate =  pAtomCore.GetStringDatetime(pEndDate.Year, pStartDate.Month, nDay);

					//	pDrawList.Add(pTempData);
					//}
					//else if (m_nMonth == pEndDate.Month)
					//{
					//	CalendarScheduleData pTempData = new CalendarScheduleData(pitem);

					//	pTempData.StartDate = pAtomCore.GetStringDatetime(pEndDate.Year, pEndDate.Month, 1);

					//	pDrawList.Add(pTempData);
					//}
					//else if (m_nMonth >= pStartDate.Month && pEndDate.Month >= m_nMonth) 
					//{
					//	//일정의 길이가 3달을 넘어가는 경우에 발생하며 현재월전체에 일정을 추가한다.
					//	CalendarScheduleData pTempData = new CalendarScheduleData(pitem);

					//	int nLastDay = DateTime.DaysInMonth(pStartDate.Year, m_nMonth);

					//	pTempData.StartDate = pAtomCore.GetStringDatetime(pEndDate.Year, m_nMonth, 1);
					//	pTempData.EndDate = pAtomCore.GetStringDatetime(pEndDate.Year, m_nMonth, nLastDay);

					//	pDrawList.Add(pTempData);						
					//}
				}
			}

			if (LC.LANG.KOREAN == LC.PQLanguage && atomAttrib.IsShowHoliday)
			{
				List<CalendarScheduleData> pHolidayList = HolidayManager.Instance.GetData (m_nYear, m_nMonth);

				if (null != pHolidayList)
				{
					pDrawList.Sort (delegate (CalendarScheduleData x, CalendarScheduleData y) //공휴일을 무조건 앞쪽에 배치해야되기 때문에 임의로 정렬시킴
					{
						return x.StartDate.CompareTo (y.StartDate);
					});

					foreach (CalendarScheduleData data in pHolidayList)
					{
						data.Foreground = Brushes.White;
						data.Background = (SolidColorBrush)(new BrushConverter ().ConvertFrom ("#DBEE6055"));

						pDrawList.Insert (0, data);
					}
				}
			}

			this.Dispatcher.BeginInvoke (DispatcherPriority.Render, new Action (delegate
			{
				DrawScheduleData (pDrawList);
			}));
		}

		private void DrawScheduleData (List<CalendarScheduleData> drawList)
		{
			CalendarAtom atomCore = this.AtomCore as CalendarAtom;
			CalendarAttrib atomAttrib = atomCore.GetAttrib () as CalendarAttrib;

			if (null == atomCore || null == atomAttrib)
				return;
			/*
			 * 1. 하나의 칸에 그릴수 있는 일정의 최대 개수를 구한다.
			 * 2. 각 일자별 일정을 계산하여 넘치는 날짜를 찾는다.
			 * 3. 넘치는 경우 숫자로 표기한다.
			 */

			//일정의 개수 수하는 방식
			//일정 control의 높이 :  전체 샐의 높이에 offset% 높이
			// (전체영역 -  일자가 차지한 영역) / 일정 control 높이

			double cellHeight = MainGrid.RowDefinitions[0].ActualHeight;
			double cellWidth = MainGrid.ColumnDefinitions[0].ActualWidth;

			//double offset = 25;
			//double scheduleHeight = cellHeight * (offset * 0.01);
			//double dMeasurSize = GetMeasureString (atomAttrib.DayGridFontSize);
			//double dMargins = 0;

			double numHeight = GetMeasureString (atomAttrib.DayGridFontSize);
			double scheduleFontSize = atomAttrib.DayGridFontSize * 0.75;
			double scheduleHeight = numHeight * 0.8; // GetMeasureString (scheduleFontSize);
			double xMargin = 2;
			double yMargin = 1;

			int maxScheduleCount = (int)((cellHeight - numHeight) / scheduleHeight);
			int nLastDay = DateTime.DaysInMonth (this.Year, this.Month);

			Dictionary<DateTime, List<CalendarScheduleData>> map = new Dictionary<DateTime, List<CalendarScheduleData>> ();

			drawList.Sort (delegate (CalendarScheduleData x, CalendarScheduleData y)
			{
				var xValue = atomCore.GetDateParse (x.EndDate) - atomCore.GetDateParse (x.StartDate);
				var yValue = atomCore.GetDateParse (y.EndDate) - atomCore.GetDateParse (y.StartDate);

				if ((false == x.Holiday && false == y.Holiday) || (true == x.Holiday && y.Holiday))
				{
					if (xValue == yValue)
						return y.Index.CompareTo (x.Index);
					else
						return xValue.CompareTo (yValue);
				}
				else
				{
					if (x.Holiday)
						return 1;
					else
						return -1;
				}
			});

			drawList.Reverse ();

			foreach (CalendarScheduleData item in drawList)
			{
				item.IsDraw = true;
				item.DrawCurrentRow = -1;
				List<DateTime> dateTimeList = GetDateTimeRange (item);

				foreach (DateTime date in dateTimeList)
				{
					if (false == map.ContainsKey (date))
					{
						map.Add (date, new List<CalendarScheduleData> ());
					}

					map[date].Add (item);
				}
			}

			foreach (CalendarScheduleData item in drawList)
			{
				DateTime startDateTime = atomCore.GetDateParse (item.StartDate);
				DateTime endDateTime = atomCore.GetDateParse (item.EndDate);

				if (false == map.ContainsKey (startDateTime))
					continue;

				int nCount = (endDateTime - startDateTime).Days;
				int nCurrentRow = -1;
				for (int i = 0; i <= nCount; i++)
				{
					DateTime currentDate = startDateTime.AddDays (i);
					int temp = map[currentDate].Max (mapItem => mapItem.DrawCurrentRow) + 1;
					nCurrentRow = nCurrentRow < temp ? temp : nCurrentRow;
				}

				item.DrawCurrentRow = nCurrentRow;
			}

			bool isSkip = 0 < map.Where (mapItem => maxScheduleCount <= mapItem.Value.Max (item => item.DrawCurrentRow)).Count ();

			if (true == isSkip)
			{
				foreach (List<CalendarScheduleData> ItemList in map.Values)
				{
					if (ItemList.Count > maxScheduleCount)
					{
						foreach (var item in ItemList)
						{
							if (maxScheduleCount - 1 <= item.DrawCurrentRow)
							{
								// 2022-07-27 kys 표시할 일정이 1개만 존재하는경우 화면상에서 짤리더라도 그려준다.

								DateTime startDateTime = atomCore.GetDateParse (item.StartDate);
								DateTime endDateTime = atomCore.GetDateParse (item.EndDate);
								DateTime currentDate = startDateTime;
								bool isOnlySchedule = true;

								while (currentDate <= endDateTime)
								{
									if (true == map.ContainsKey (currentDate))
									{
										if (1 < map[currentDate].Count)
										{
											isOnlySchedule = false;
											break;
										}
									}

									currentDate = currentDate.AddDays (1);
								}

								if (false == isOnlySchedule)
								{
									item.IsDraw = false;
								}
							}
						}
					}
				}
			}

			foreach (CalendarScheduleData item in drawList)
			{
				if (true == item.IsDraw)
				{
					List<DateTime> dateTimeList = GetDateTimeRange (item);

					Dictionary<int, KeyValuePair<int, int>> position = new Dictionary<int, KeyValuePair<int, int>> ();

					foreach (DateTime date in dateTimeList)
					{
						CalendarItem calendarItem = GetCalendarItem (date);

						if (null == calendarItem) continue;

						int nRow = Grid.GetRow (calendarItem);
						int nColumn = Grid.GetColumn (calendarItem);

						if (false == position.ContainsKey (nRow))
						{
							position.Add (nRow, new KeyValuePair<int, int> (nColumn, 1));
						}
						else
						{
							position[nRow] = new KeyValuePair<int, int> (position[nRow].Key, position[nRow].Value + 1);
						}
					}

					foreach (var pos in position)
					{
						int nRow = pos.Key;
						int nColumn = pos.Value.Key;
						int nColumnSpan = pos.Value.Value;
						int nCurrentRow = item.DrawCurrentRow;

						nCurrentRow = 0 < nCurrentRow ? nCurrentRow : 0;

						double dTop = numHeight + (nCurrentRow * scheduleHeight) + (nCurrentRow * yMargin);

						ScheduleItem scheduleItem = CreateScheduleItem (item);

						Grid.SetRow (scheduleItem, nRow);
						Grid.SetColumn (scheduleItem, nColumn);
						Grid.SetColumnSpan (scheduleItem, nColumnSpan);

						scheduleItem.FontSize = scheduleFontSize;
						scheduleItem.Height = scheduleHeight - (yMargin * 2);
						scheduleItem.VerticalAlignment = System.Windows.VerticalAlignment.Top;
						scheduleItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
						scheduleItem.Margin = new Thickness (xMargin, dTop, xMargin, yMargin);

						MainGrid.Children.Add (scheduleItem);
					}
				}
			}

			foreach (var itemPair in map)
			{
				List<CalendarScheduleData> dataList = itemPair.Value;

				List<CalendarScheduleData> drawItemDataList = dataList.Where (item => item.IsDraw).ToList ();
				var hideItemList = dataList.Where (item => false == item.IsDraw).ToList ();

				if (0 < hideItemList.Count)
				{
					CalendarItem calendarItem = GetCalendarItem (itemPair.Key);
					ScheduleItem scheduleItem = CreateScheduleMultiItem (itemPair.Key, hideItemList);

					int nCurrentRow = maxScheduleCount - 1;
					int nRow = Grid.GetRow (calendarItem);
					int nColumn = Grid.GetColumn (calendarItem);
					int nColumnSpan = (scheduleItem.EndDateTime - scheduleItem.StartDateTime).Days + 1;

					nCurrentRow = 0 < nCurrentRow ? nCurrentRow : 0;

					double dTop = numHeight + (nCurrentRow * scheduleHeight) + (nCurrentRow * yMargin);

					Grid.SetRow (scheduleItem, nRow);
					Grid.SetColumn (scheduleItem, nColumn);
					Grid.SetColumnSpan (scheduleItem, nColumnSpan);

					scheduleItem.FontSize = scheduleFontSize;
					scheduleItem.Height = scheduleHeight - (yMargin * 2);
					scheduleItem.VerticalAlignment = System.Windows.VerticalAlignment.Top;
					scheduleItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
					scheduleItem.Margin = new Thickness (xMargin, dTop, xMargin, yMargin);

					MainGrid.Children.Add (scheduleItem);
				}
			}
		}

		public List<DateTime> GetDateTimeRange (CalendarScheduleData item)
		{
			CalendarAtom atomCore = this.AtomCore as CalendarAtom;

			List<DateTime> dateTimeList = new List<DateTime> ();

			DateTime startDateTime = atomCore.GetDateParse (item.StartDate);
			DateTime endDateTime = atomCore.GetDateParse (item.EndDate);

			DateTime currentDate = startDateTime;
			do
			{
				dateTimeList.Add (currentDate);
				currentDate = currentDate.AddDays (1);
			} while (currentDate <= endDateTime);

			return dateTimeList;
		}

		public ScheduleItem CreateScheduleItem (CalendarScheduleData data)
		{
			CalendarAtom atomCore = this.AtomCore as CalendarAtom;
			CalendarAttrib atomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != atomCore && null != atomAttrib)
			{
				ScheduleItem scheduleItem = new ScheduleItem ();

				scheduleItem.ScheduleBackground = data.Background;
				scheduleItem.ScheduleForeground = data.Foreground;
				scheduleItem.StartDateTime = atomCore.GetDateParse (data.StartDate);
				scheduleItem.EndDateTime = atomCore.GetDateParse (data.EndDate);
				scheduleItem.ScheduleTitle = data.Title;
				scheduleItem.ScheduleContent = GetDisplayContent (data.Content);

				scheduleItem.MouseLeftButtonDown -= ScheduleItem_MouseLeftButtonDown;
				scheduleItem.MouseLeftButtonDown += ScheduleItem_MouseLeftButtonDown;

				return scheduleItem;
			}

			return null;
		}

		public ScheduleItem CreateScheduleMultiItem (DateTime date, List<CalendarScheduleData> dataList)
		{
			CalendarAtom atomCore = this.AtomCore as CalendarAtom;
			CalendarAttrib atomAttrib = this.AtomCore.GetAttrib () as CalendarAttrib;

			if (null != atomCore && null != atomAttrib)
			{
				ScheduleItem scheduleItem = new ScheduleItem ();

				scheduleItem.ScheduleBackground = Brushes.Transparent;
				scheduleItem.ScheduleForeground = Brushes.Gray;
				scheduleItem.StartDateTime = date;
				scheduleItem.EndDateTime = date;
				scheduleItem.ScheduleTitle = "";
				scheduleItem.ScheduleContent = string.Format ("+{0}", dataList.Count);
				scheduleItem.IsMultiItem = true;

				scheduleItem.MouseLeftButtonDown -= ScheduleItem_MouseLeftButtonDown;
				scheduleItem.MouseLeftButtonDown += ScheduleItem_MouseLeftButtonDown;

				scheduleItem.Tag = dataList;

				return scheduleItem;
			}

			return null;
		}

		public ScheduleItem GetScheduleItem (CalendarScheduleData data)
		{
			ScheduleItem itemList = MainGrid.Children.OfType<ScheduleItem> ().Where (item => item.ScheduleTitle == data.Title).FirstOrDefault ();
			return itemList;
		}

		public double GetMeasureString (double dFontSize)
		{
			double dHeight = 0;
			string strText = "99";

			var formattedText = new FormattedText (
				strText,
				System.Globalization.CultureInfo.CurrentUICulture,
				FlowDirection.LeftToRight,
				new Typeface (GetAtomFontFamily (), GetAtomFontStyle (), GetAtomFontWeight (), FontStretch), dFontSize, Brushes.Black);

			dHeight = formattedText.Height;

			return dHeight;
		}

		private string GetDisplayContent (string strContent)
		{
			int nIndex = strContent.IndexOf ("$");

			if (-1 < nIndex)
			{
				return strContent.Substring (0, nIndex);
			}

			return strContent;
		}

		private void ScheduleItem_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			CalendarAtom atomCore = this.AtomCore as CalendarAtom;

			if (1 == this.AtomCore.AtomRunMode) //실행모드일때
			{
				CVariantX[] pvaArgs = new CVariantX[5 + 1];
				CalendarItem calendarItem = GetMouseOverCalendarItem ();
				string strCurrentDate = null != calendarItem ? string.Format ("{0:D4}{1:D2}{2:D2}", calendarItem.Year, calendarItem.Month, calendarItem.Day) : "";

				if (null != calendarItem)
				{
					// 2024.10.14 beh 일정 클릭 시에도 해당 날짜 배경색 변경되도록 보강
					OnNotifyClickEvent (calendarItem, false);
				}

				ScheduleItem scheduleItem = sender as ScheduleItem;

				if (null != scheduleItem)
				{
					if (false == scheduleItem.IsMultiItem)
					{
						string strTitle = scheduleItem.ScheduleTitle;
						string strStartDate = scheduleItem.StartDateTime.ToString ("yyyyMMdd");
						string strEndDate = scheduleItem.EndDateTime.ToString ("yyyyMMdd");
						string strContent = scheduleItem.ScheduleContent;

						pvaArgs[0] = new CVariantX (5);
						pvaArgs[1] = new CVariantX (strTitle);
						pvaArgs[2] = new CVariantX (strStartDate);
						pvaArgs[3] = new CVariantX (strEndDate);
						pvaArgs[4] = new CVariantX (strContent);
						pvaArgs[5] = new CVariantX (strCurrentDate);
					}
					else
					{
						List<CalendarScheduleData> scheduleDataList = scheduleItem.Tag as List<CalendarScheduleData>;

						if (null != scheduleDataList)
						{
							CVarArrayX titleArray = new CVarArrayX ();
							CVarArrayX startDateArray = new CVarArrayX ();
							CVarArrayX endDateArray = new CVarArrayX ();
							CVarArrayX contentDateArray = new CVarArrayX ();

							foreach (CalendarScheduleData data in scheduleDataList)
							{
								titleArray.Add (new CVariantX (data.Title));
								startDateArray.Add (new CVariantX (data.StartDate));
								endDateArray.Add (new CVariantX (data.EndDate));
								contentDateArray.Add (new CVariantX (data.Content));
							}

							pvaArgs[0] = new CVariantX (5);
							pvaArgs[1] = new CVariantX (titleArray);
							pvaArgs[2] = new CVariantX (startDateArray);
							pvaArgs[3] = new CVariantX (endDateArray);
							pvaArgs[4] = new CVariantX (contentDateArray);
							pvaArgs[5] = new CVariantX (strCurrentDate);
						}
					}

					if (-1 != AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_SCHEDULE_CLICK, pvaArgs)) //일정클릭
					{
						if (0 <= MsgHandler.CALL_MSG_HANDLER (AtomCore, EVS_TYPE.EVS_A_SCHEDULE_CLICK, pvaArgs))
						{
							AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_SCHEDULE_CLICK, pvaArgs);
						}
					}
				}
			}
		}

		private string GetCurrentDateMouseOver ()
		{
			Point pt = Mouse.GetPosition (MainGrid);

			double widthCol = MainGrid.ActualWidth / 7;
			double heightRow = MainGrid.ActualHeight / 6;

			int nCol = (int)(pt.X / widthCol);
			int nRow = (int)(pt.Y / heightRow);

			CalendarItem calendarItem = MainGrid.Children.OfType<CalendarItem> ().Where (item => nRow == Grid.GetRow (item) && nCol == Grid.GetColumn (item)).FirstOrDefault ();

			if (null != calendarItem)
			{
				return string.Format ("{0:D4}{1:D2}{2:D2}", calendarItem.Year, calendarItem.Month, calendarItem.Day);
			}

			return "";
		}

		private CalendarItem GetMouseOverCalendarItem ()
		{
			Point pt = Mouse.GetPosition (MainGrid);

			double widthCol = MainGrid.ActualWidth / 7;
			double heightRow = MainGrid.ActualHeight / 6;

			int nCol = (int)(pt.X / widthCol);
			int nRow = (int)(pt.Y / heightRow);

			CalendarItem calendarItem = MainGrid.Children.OfType<CalendarItem> ().Where (item => nRow == Grid.GetRow (item) && nCol == Grid.GetColumn (item)).FirstOrDefault ();

			if (null != calendarItem)
			{
				return calendarItem;
			}

			return null;
		}

		#region Calendar Animation
		private void CreateAnimationControl ()
		{
			if (null == m_CalendarSlideAnimation)
			{
				m_CalendarSlideAnimation = new CalendarSlideAnimation ();

				m_CalendarSlideAnimation.ContainerGrid = this.LayoutGrid;
				m_CalendarSlideAnimation.MainGrid = this.MainGrid;

				m_CalendarSlideAnimation.CreateAnimationControl ();
			}
			else
			{
				m_CalendarSlideAnimation.VisibleAnimationControl ();
			}
		}

		private void DropAnimationControl ()
		{
			if (null != m_CalendarSlideAnimation)
			{
				m_CalendarSlideAnimation.DropAnimationControl ();
				m_CalendarSlideAnimation = null;
			}
		}

		private void PreviewStartAnimationControl (bool bInclease)
		{
			CreateAnimationControl ();

			m_CalendarSlideAnimation.FirstPage.ImageSource = ScreenCaptureHelper.GetRenderTargetImage (this.MainGrid);
			m_CalendarSlideAnimation.TabInclease = bInclease;
		}

		private void StartAnimationControl ()
		{
			this.MainGrid.UpdateLayout ();

			m_CalendarSlideAnimation.SecondPage.ImageSource = ScreenCaptureHelper.GetRenderTargetImage (this.MainGrid);
			m_CalendarSlideAnimation.BeginAnimation ();
		}
		#endregion
	}
}
