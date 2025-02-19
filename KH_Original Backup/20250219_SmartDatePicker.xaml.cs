using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using ScrollAnimateBehavior.AttachedBehaviors;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.TopApp.CommonLib;
using Softpower.SmartMaker.TopApp.ValueConverter;
using Softpower.SmartMaker.TopSmartResourceManager;

namespace Softpower.SmartMaker.TopControlRun.SmartDateTimePicker
{
	/// <summary>
	/// SmartDatePicker.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class SmartDatePicker : Window
	{
		int m_nYear = 2020;
		int m_nMonth = 7;
		int m_nDay = 31;

		private int m_nYearMinValue = 1900;
		private int m_nYearMaxValue = 2050;

		private int m_nMonthMinValue = 1;
		private int m_nMonthMaxValue = 12;

		private int m_nDayMinValue = 1;
		private int m_nDayMaxValue = 31;

		private const int m_nStepSize = 40;

		public string DateValue
		{
			get
			{
				return string.Format ("{0:D4}-{1:D2}-{2:D2}", m_nYear, m_nMonth, m_nDay);
			}

			set
			{
				if (8 == value.Length)
				{
					DateTime dateTime = DateTime.ParseExact (value, "yyyyMMdd", null);
					m_nYear = dateTime.Year;
					m_nMonth = dateTime.Month;
					m_nDay = dateTime.Day;

					ResetInitDay ();
					ResetDateScrollPosition ();
				}
			}
		}

		public SmartDatePicker ()
		{
			InitializeComponent ();

			SetImage ();
			InitDefaultDate ();

			InitDate ();

			InitEvent ();

			ResetDateScrollPosition ();
			InitDateChangeEvent ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("SmartDatePicker", this);
			}
		}

		public SmartDatePicker (string strDateValue, bool bLunisolar)
			: this ()
		{
			this.DateValue = strDateValue;

			LunisolarTextBlock.Visibility = true == bLunisolar ? Visibility.Visible : Visibility.Collapsed;
		}

		private void InitEvent ()
		{
			this.PreviewKeyDown += SmartDatePicker_PreviewKeyDown;
		}

		void SmartDatePicker_PreviewKeyDown (object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				this.DialogResult = false;
				this.Close ();
			}
			else if (e.Key == Key.Enter)
			{
				this.DialogResult = true;
				this.Close ();
			}
		}


		private void SetImage ()
		{
			BitmapImage bitImage = new BitmapImage ();

			string strResourceName = string.Empty;
			strResourceName = "Softpower.SmartMaker.TopSmartResourceManager.Resources.Images.DatePicker.ic_dialog_time.png";
			Stream currentStream = SmartResourceManager.GetResourceImage (strResourceName);
			bitImage = AndroidCommonPaint.LoadImageFromResource (strResourceName, currentStream);
			ClockImage.Source = bitImage;
		}

		void InitDefaultDate ()
		{
			DateTime dateTime = DateTime.Now;
			m_nYear = dateTime.Year;
			m_nMonth = dateTime.Month;
			m_nDay = dateTime.Day;
		}

		void InitDateChangeEvent ()
		{
			this.DateScrollViewer.ScrollChanged += DateScrollViewer_ScrollChanged;

			this.PrevDate.AddHandler (MouseLeftButtonDownEvent, new MouseButtonEventHandler (PrevDate_MouseLeftButtonDown), true);
			this.NextDate.AddHandler (MouseLeftButtonDownEvent, new MouseButtonEventHandler (NextDate_MouseLeftButtonDown), true);
		}

		void PrevDate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			double nOffset = ScrollAnimationBehavior.GetVerticalOffset (this.DateScrollViewer);
			nOffset = (int)Math.Round (nOffset / m_nStepSize) * m_nStepSize;
			double ToValue = nOffset - m_nStepSize;

			ScrollAnimationBehavior.AnimateScroll (this.DateScrollViewer, ToValue);
		}

		void NextDate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			double nOffset = ScrollAnimationBehavior.GetVerticalOffset (this.DateScrollViewer);
			nOffset = (int)Math.Round (nOffset / m_nStepSize) * m_nStepSize;
			double ToValue = nOffset + m_nStepSize;

			ScrollAnimationBehavior.AnimateScroll (this.DateScrollViewer, ToValue);
		}

		void UpdateDateText ()
		{
			UpdateDateValue ();

			DateTime dateTime = DateTime.ParseExact (this.DateValue, "yyyy-MM-dd", null);
			DateTextBlock.Text = dateTime.ToLongDateString ();
			LunisolarTextBlock.Text = DateTimeConverter.GetLunisolarToShortDateString (dateTime);
		}

		void DateScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (0 < Math.Abs (e.VerticalChange))
			{
				int nCurrentMonth = (int)this.DateScrollViewer.VerticalOffset / m_nStepSize + 1;
				if (1 <= nCurrentMonth && nCurrentMonth <= 12)
				{
					if (nCurrentMonth != m_nMonth)
					{
						m_nMonth = nCurrentMonth;

						ResetInitDay ();
						UpdateDateText ();
					}
				}
			}
		}


        void InitDate()
        {
            for (int i = 0 ; i < 100 ; i++)
            {
                DatePickerItem item = new DatePickerItem();
                if (1900 <= nYear && nYear <= 2100)
                {
                    item.NumText = nYear.ToString();
                    item.MaxLength = 4;
                    item.MinValue = 1900;
                    item.MaxValue = 2100;

                    m_nYearMinValue = 1900;
                    m_nYearMaxValue = 2100;
                }
                else
                {
                }

                DatePanelContainer.Children.Add(item);
            }
        }

		/*
        void InitYear ()
		{
			for (int nYear = 1899; nYear <= 2100 + 1; nYear++)
			{
				DatePickerItem item = new DatePickerItem ();
				if (1900 <= nYear && nYear <= 2100)
				{
					item.NumText = nYear.ToString ();
					item.MaxLength = 4;
					item.MinValue = 1900;
					item.MaxValue = 2100;

					m_nYearMinValue = 1900;
					m_nYearMaxValue = 2100;
				}
				else
				{
				}

				YearPanelContainer.Children.Add (item);
			}
		}

		void InitMonth ()
		{
			for (int nMonth = 0; nMonth <= 12 + 1; nMonth++)
			{
				DatePickerItem item = new DatePickerItem ();
				if (1 <= nMonth && nMonth <= 12)
				{
					item.NumText = nMonth.ToString ();
					item.MaxLength = 2;
					item.MinValue = 1;
					item.MaxValue = 12;

					m_nMonthMinValue = 1;
					m_nMonthMaxValue = 12;
				}

				MonthPanelContainer.Children.Add (item);
			}
		}

		void InitDay ()
		{
			int nMaxDay = DateTime.DaysInMonth (m_nYear, m_nMonth);

			for (int nDay = 0; nDay <= nMaxDay + 1; nDay++)
			{
				DatePickerItem item = new DatePickerItem ();
				if (1 <= nDay && nDay <= nMaxDay)
				{
					item.NumText = nDay.ToString ();
					item.MaxLength = 2;
					item.MinValue = 1;
					item.MaxValue = nMaxDay;

					m_nDayMinValue = 1;
					m_nDayMaxValue = nMaxDay;
				}

				DayPanelContainer.Children.Add (item);
			}
		}
		*/

		void UpdateDateValue ()
		{
			// 현재 PointsToScroll 40 으로 설정 1900년~ Grid.Row=1  
			int nYear = (int)(this.YearScrollViewer.VerticalOffset / m_nStepSize);
			int nMonth = (int)(this.MonthScrollViewer.VerticalOffset / m_nStepSize);
			int nDay = (int)(this.DayScrollViewer.VerticalOffset / m_nStepSize);

			if (nYear + 1 > YearPanelContainer.Children.Count)
			{
				nYear = YearPanelContainer.Children.Count - 2;
			}

			if (nMonth + 1 > MonthPanelContainer.Children.Count)
			{
				nMonth = MonthPanelContainer.Children.Count - 2;
			}

			if (nDay + 1 >= DayPanelContainer.Children.Count)
			{
				nDay = DayPanelContainer.Children.Count - 2;
			}

			DatePickerItem itemYear = this.YearPanelContainer.Children[nYear + 1] as DatePickerItem;
			DatePickerItem itemMonth = this.MonthPanelContainer.Children[nMonth + 1] as DatePickerItem;
			DatePickerItem itemDay = this.DayPanelContainer.Children[nDay + 1] as DatePickerItem;

			if (false == string.IsNullOrEmpty (itemYear.Text))
			{
				m_nYear = _Kiss.toInt32 (itemYear.Text);

				if (m_nYearMinValue > m_nYear)
				{
					m_nYear = m_nYearMinValue;
				}
				else if (m_nYearMaxValue < m_nYear)
				{
					m_nYear = m_nYearMaxValue;
				}
			}
			else
			{
				m_nYear = _Kiss.toInt32 (DateTime.Now.ToString ("yyyy"));
			}

			if (false == string.IsNullOrEmpty (itemMonth.Text))
			{
				m_nMonth = _Kiss.toInt32 (itemMonth.Text);

				if (m_nMonthMinValue > m_nMonth)
				{
					m_nMonth = m_nMonthMinValue;
				}
				else if (m_nMonthMaxValue < m_nMonth)
				{
					m_nMonth = m_nMonthMaxValue;
				}
			}
			else
			{
				m_nMonth = _Kiss.toInt32 (DateTime.Now.ToString ("MM"));
			}

			m_nDayMaxValue = DateTime.DaysInMonth (m_nYear, m_nMonth);

			if (false == string.IsNullOrEmpty (itemDay.Text))
			{
				m_nDay = _Kiss.toInt32 (itemDay.Text);
			}
			else
			{
				m_nDay = _Kiss.toInt32 (DateTime.Now.ToString ("dd"));
			}

			if (m_nDayMinValue > m_nDay)
			{
				m_nDay = m_nDayMinValue;
			}
			else if (m_nDayMaxValue < m_nDay)
			{
				m_nDay = m_nDayMaxValue;
			}
		}

		void ResetDateScrollPosition ()
		{
			// 현재 PointsToScroll 40 으로 설정 1900년~  Grid.Row=1
			ScrollAnimationBehavior.SetVerticalOffset (this.YearScrollViewer, (double)((m_nYear - 1900) * m_nStepSize));
			ScrollAnimationBehavior.SetVerticalOffset (this.MonthScrollViewer, (double)((m_nMonth - 1) * m_nStepSize));
			ScrollAnimationBehavior.SetVerticalOffset (this.DayScrollViewer, (double)((m_nDay - 1) * m_nStepSize));
		}

		void ResetInitDay ()
		{
			int nMaxDay = DateTime.DaysInMonth (m_nYear, m_nMonth);

			int nCount = DayPanelContainer.Children.Count - 2;
			if (nMaxDay < nCount)
			{
				DayPanelContainer.Children.RemoveRange (nMaxDay + 1, nCount - nMaxDay);
			}
			else if (nMaxDay > nCount)
			{
				for (int nDay = nCount + 1; nDay <= nMaxDay; nDay++)
				{
					DatePickerItem item = new DatePickerItem ();
					item.NumText = nDay.ToString ();
					item.MaxLength = 2;
					item.MinValue = 1;
					item.MaxValue = nMaxDay;

					DayPanelContainer.Children.Insert (nDay, item);
				}
			}
		}

		private void OkButton_Click (object sender, RoutedEventArgs e)
		{
			UpdateDateValue ();

			this.DialogResult = true;
		}

		private void CancelButton_Click (object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		public string GetRealValue ()
		{
			return string.Format ("{0:D4}{1:D2}{2:D2}", m_nYear, m_nMonth, m_nDay);
		}
	}
}
