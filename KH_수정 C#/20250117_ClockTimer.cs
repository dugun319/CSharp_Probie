using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

using Softpower.SmartMaker.TopApp.ValueConverter;

namespace Softpower.SmartMaker.TopControl.Components.Clock
{
	public class ClockTimer : FrameworkElement
	{
		private System.Timers.Timer m_systemTimer;
		private DispatcherTimer m_timer;    // 사용안함 Timer 로 대체
		private DateTime m_StartTime = DateTime.Now;

        // Stopwatch를 추가하여 정밀한 시간 계산
        private Stopwatch m_stopwatch = new Stopwatch();


        private TimeSpan m_StartTimeSpan = new TimeSpan (0, 0, 0);
		private TimeSpan m_CurrentTimeSpan = new TimeSpan (0, 0, 0);
		private TimeSpan m_MaximumTimeSpan = new TimeSpan (23, 59, 59);

		private int m_nFunctionKind = 0;    // 0:시각, 1:스톱워치, 2:타이머
		private string m_strTimeZoneID;

		private static DependencyProperty DateTimeProperty =
			DependencyProperty.Register ("DateTime", typeof (object), typeof (ClockTimer),
			new PropertyMetadata (DateTime.Now));

		private static DependencyProperty TimeSpanProperty =
			DependencyProperty.Register ("TimeSpan", typeof (TimeSpan), typeof (ClockTimer),
			new PropertyMetadata (TimeSpan.MinValue));

		public event EventHandler TimerEnd;

		// 0:시각, 1:스톱워치, 2:타이머
		public int FunctionKind
		{
			set
			{
				m_nFunctionKind = value;

				switch (value)
				{
					case 0:
						if (null != m_timer) m_timer.Interval = TimeSpan.FromMilliseconds (1000);
						if (null != m_systemTimer) m_systemTimer.Interval = TimeSpan.FromMilliseconds (1000).TotalMilliseconds;
						break;
					case 1:
						if (null != m_timer) m_timer.Interval = TimeSpan.FromMilliseconds (10);
						if (null != m_systemTimer) m_systemTimer.Interval = TimeSpan.FromMilliseconds (10).TotalMilliseconds; ;
						break;
					case 2:
						if (null != m_timer) m_timer.Interval = TimeSpan.FromMilliseconds (1000);
						if (null != m_systemTimer) m_systemTimer.Interval = TimeSpan.FromMilliseconds (1000).TotalMilliseconds; ;
						break;
				}
			}
		}

		public TimeSpan MaximumTimeSpan
		{
			set { m_MaximumTimeSpan = value; }
		}

		public TimeSpan CurrentTimeSpan
		{
			get { return m_CurrentTimeSpan; }
			set { m_CurrentTimeSpan = value; }
		}

		public string TimeZoneID
		{
			get { return m_strTimeZoneID; }
			set { m_strTimeZoneID = value; }
		}

		protected override void OnInitialized (EventArgs e)
		{
			base.OnInitialized (e);

			// DispatcherTimer 대신에 Timer 로 대체

			m_timer = new DispatcherTimer ();
			m_timer.Interval = TimeSpan.FromMilliseconds (1000);
			m_timer.Tick += new EventHandler (Timer_Tick);

			/*
			m_systemTimer = new System.Timers.Timer ();
			m_systemTimer.Interval = TimeSpan.FromMilliseconds (1000).TotalMilliseconds;
			m_systemTimer.Elapsed += SystemTimer_Elapsed;
			*/
		}

		private void SystemTimer_Elapsed (object sender, System.Timers.ElapsedEventArgs e)
		{
			Dispatcher.BeginInvoke (DispatcherPriority.Background, new System.Action (delegate ()
			{
				switch (m_nFunctionKind)
				{
					case 0: // 시각
						{
							DateTime nowTime = DateTimeConverter.GetDateTimeZoneNow (TimeZoneID);
							SetValue (nowTime);
						}
						break;
					case 1: // 스톱워치
						{
							DateTime nowTime = DateTimeConverter.GetDateTimeZoneNow (TimeZoneID);
							TimeSpan timeSpan = nowTime - m_StartTime;
							m_CurrentTimeSpan = m_StartTimeSpan + timeSpan;

							if (m_CurrentTimeSpan > m_MaximumTimeSpan)
							{
								m_CurrentTimeSpan = m_MaximumTimeSpan;
								Stop ();

								if (null != TimerEnd)
								{
									TimerEnd (this, null);
								}
							}

							SetDisplayValue (m_CurrentTimeSpan);
						}
						break;
					case 2: // 타이머
						{
							DateTime nowTime = DateTimeConverter.GetDateTimeZoneNow (TimeZoneID);
							TimeSpan timeSpan = nowTime - m_StartTime;
							m_CurrentTimeSpan = m_StartTimeSpan - timeSpan;

							if (m_CurrentTimeSpan.TotalSeconds < 0)
							{
								m_CurrentTimeSpan = new TimeSpan (0, 0, 0);
								Stop ();

								if (null != TimerEnd)
								{
									TimerEnd (this, null);
								}
							}

							SetDisplayValue (m_CurrentTimeSpan);
						}
						break;
				}
			}));
		}

		private void Timer_Tick (object sender, EventArgs e)
		{
			DateTime nowTime = DateTime.Now;
            TimeSpan elapsedTime = m_stopwatch.Elapsed;

            switch (m_nFunctionKind)
			{
				case 0: // 시각
					{
                        // DateTime nowTime = DateTimeConverter.GetDateTimeZoneNow(TimeZoneID);
                        SetValue (nowTime);
					}
					break;
				case 1: // 스톱워치
					{
						// DateTime nowTime = DateTimeConverter.GetDateTimeZoneNow (TimeZoneID);
						// TimeSpan timeSpan = nowTime - m_StartTime;
						m_CurrentTimeSpan = m_StartTimeSpan + elapsedTime;

						SetDisplayValue (m_CurrentTimeSpan);
					}
					break;
				case 2: // 타이머
					{
						// DateTime nowTime = DateTimeConverter.GetDateTimeZoneNow (TimeZoneID);
						// TimeSpan timeSpan = nowTime - m_StartTime;

						m_CurrentTimeSpan = m_StartTimeSpan - elapsedTime;

						if (m_CurrentTimeSpan.TotalSeconds <= 0)
						{
							m_CurrentTimeSpan = TimeSpan.Zero;
							Stop ();
                            TimerEnd?.Invoke(this, null);
                        }

						SetDisplayValue (m_CurrentTimeSpan);
					}
					break;
			}
		}

		public void Start ()
		{
			if (m_timer != null)
			{

                m_stopwatch.Reset();
                m_stopwatch.Start();

                m_StartTime = DateTime.Now;
                m_CurrentTimeSpan = m_StartTimeSpan;


                //m_StartTime = DateTimeConverter.GetDateTimeZoneNow (TimeZoneID);

                // 첫 번째 Tick을 수동 호출하여 시간 차이 방지
                Timer_Tick(this, EventArgs.Empty);

                m_timer?.Start ();
			}

			if (null != m_systemTimer)
			{
				m_StartTime = DateTimeConverter.GetDateTimeZoneNow (TimeZoneID);

                m_CurrentTimeSpan = m_StartTimeSpan;

                // 첫 번째 Tick 즉시 호출
                SystemTimer_Elapsed(this, null);

                m_systemTimer.Start ();
			}
		}

		public void Stop ()
		{
			if (null != m_timer)
			{
                m_timer?.Stop();
                m_systemTimer?.Stop();
                m_stopwatch.Stop();
            }

			if (null != m_systemTimer)
			{
				m_systemTimer.Stop ();
			}
		}

		public void Pause ()
		{
			Stop ();

			m_StartTime = DateTimeConverter.GetDateTimeZoneNow (TimeZoneID);
			m_StartTimeSpan = m_CurrentTimeSpan;
		}

		public void SetValue (DateTime nValue)
		{
			SetValue (DateTimeProperty, nValue);

			switch (m_nFunctionKind)
			{
				case 0:
					{
						m_CurrentTimeSpan = new TimeSpan (nValue.Hour, nValue.Minute, nValue.Second);
					}
					break;
				case 1:
					{
						m_StartTimeSpan = new TimeSpan (nValue.Hour, nValue.Minute, nValue.Second);
						m_CurrentTimeSpan = m_StartTimeSpan;
					}
					break;
				case 2:
					{
						m_StartTimeSpan = new TimeSpan (nValue.Hour, nValue.Minute, nValue.Second);
						m_CurrentTimeSpan = m_StartTimeSpan;
					}
					break;
			}
		}

		public void SetDisplayValue (TimeSpan nValue)
		{
			SetValue (DateTimeProperty, nValue);
		}

		public void SetValue (TimeSpan nValue)
		{
			SetValue (DateTimeProperty, nValue);

			switch (m_nFunctionKind)
			{
				case 1:
					{
						m_StartTimeSpan = new TimeSpan (0, 0, 0);
						m_CurrentTimeSpan = m_StartTimeSpan;
					}
					break;
				case 2:
					{
						m_StartTimeSpan = new TimeSpan (nValue.Hours, nValue.Minutes, nValue.Seconds);
						m_CurrentTimeSpan = m_StartTimeSpan;
					}
					break;
			}
		}
	}
}
