using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

using Softpower.SmartMaker.TopApp.ValueConverter;
using Softpower.SmartMaker.TopControl.Components.ChartJS.Models;

namespace Softpower.SmartMaker.TopControl.Components.Clock
{
	public class ClockTimer : FrameworkElement
	{
        private DateTime m_StartTime = DateTime.Now;
        public event EventHandler TimerEnd;
        private Stopwatch m_stopwatch = new Stopwatch();  // Stopwatch를 추가하여 정밀한 시간 계산

        private TimeSpan m_StartTimeSpan = new TimeSpan(0, 0, 0);
        private TimeSpan m_CurrentTimeSpan = new TimeSpan(0, 0, 0);
        private TimeSpan m_MaximumTimeSpan = new TimeSpan(23, 59, 59);

        private int m_nFunctionKind = 0;			// 0:시각, 1:스톱워치, 2:타이머
        private bool isTicking = false;             // 중복 호출 방지용 플래그

        private static DependencyProperty DateTimeProperty =
            DependencyProperty.Register("DateTime", typeof(object), typeof(ClockTimer),
            new PropertyMetadata(DateTime.Now));

        private static DependencyProperty TimeSpanProperty =
            DependencyProperty.Register("TimeSpan", typeof(TimeSpan), typeof(ClockTimer),
            new PropertyMetadata(TimeSpan.MinValue));

        private string m_strTimeZoneID;
        private DispatcherTimer m_timer;  // DispatcherTimer로 변경

        // 0:시각, 1:스톱워치, 2:타이머
        public int FunctionKind
		{
			set
			{
				m_nFunctionKind = value;

				switch (value)
				{
                    case 0:
                        if (m_timer != null) m_timer.Interval = TimeSpan.FromMilliseconds(1000);
                        break;
                    case 1:
                        if (m_timer != null) m_timer.Interval = TimeSpan.FromMilliseconds(10);
                        break;
                    case 2:
                        if (m_timer != null) m_timer.Interval = TimeSpan.FromMilliseconds(1000);
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
            base.OnInitialized(e);

            // DispatcherTimer로 변경
            m_timer = new DispatcherTimer();
            m_timer.Tick += Timer_Tick;
            m_timer.Interval = TimeSpan.FromMilliseconds(10);  // 더 작은 간격으로 설정하여 정확성 증가

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isTicking)
            {
                return;
            }

            try
            {
                isTicking = true;
                DateTime nowTime = DateTime.Now;
                TimeSpan elapsedTime = m_stopwatch.Elapsed;

                Debug.WriteLine($"Elapsed Time: {elapsedTime.TotalSeconds} seconds");

                switch (m_nFunctionKind)
                {
                    case 0: // 시각
                        SetValue(nowTime);
                        break;
                    case 1: // 스톱워치
                        m_CurrentTimeSpan = m_StartTimeSpan + elapsedTime;
                        SetDisplayValue(m_CurrentTimeSpan);
                        break;
                    case 2: // 타이머
                        m_CurrentTimeSpan = m_StartTimeSpan - elapsedTime;

                        if (m_CurrentTimeSpan.TotalSeconds <= 0)
                        {
                            m_CurrentTimeSpan = TimeSpan.Zero;
                            Stop();
                            TimerEnd?.Invoke(this, null);
                        }

                        SetDisplayValue(m_CurrentTimeSpan);
                        break;
                }
            }
            finally
            {
                isTicking = false;
            }
        }

        public void Start()
        {
            if (m_timer != null)
            {
                // Stopwatch 리셋하고 정확히 시작
                m_stopwatch.Reset();
                m_stopwatch.Start();
                m_timer.Start();

                // StartTimeSpan을 현재 시간으로 설정
                m_StartTimeSpan = m_CurrentTimeSpan;
            }
        }

        public void Stop()
        {
            if (m_timer != null)
            {
                m_timer.Stop();
                m_StartTime = DateTime.Now;
                m_StartTimeSpan = m_CurrentTimeSpan;
                m_stopwatch.Stop();
            }
        }

        public void Pause()
        {
            Stop();

            m_StartTime = DateTime.Now;
            m_StartTimeSpan = m_CurrentTimeSpan;
        }

        public void SetValue(DateTime nValue)
        {
            SetValue(DateTimeProperty, nValue);

            switch (m_nFunctionKind)
            {
                case 0:
                    m_CurrentTimeSpan = new TimeSpan(nValue.Hour, nValue.Minute, nValue.Second);
                    break;
                case 1:
                    m_StartTimeSpan = new TimeSpan(nValue.Hour, nValue.Minute, nValue.Second);
                    m_CurrentTimeSpan = m_StartTimeSpan;
                    break;
                case 2:
                    m_StartTimeSpan = new TimeSpan(nValue.Hour, nValue.Minute, nValue.Second);
                    m_CurrentTimeSpan = m_StartTimeSpan;
                    break;
            }
        }

        public void SetDisplayValue (TimeSpan nValue)
		{
			SetValue (DateTimeProperty, nValue);
		}

        public void SetValue(TimeSpan nValue)
        {
            SetValue(DateTimeProperty, nValue);

            switch (m_nFunctionKind)
            {
                case 1:
                    m_StartTimeSpan = new TimeSpan(0, 0, 0);
                    m_CurrentTimeSpan = m_StartTimeSpan;
                    break;
                case 2:
                    m_StartTimeSpan = new TimeSpan(nValue.Hours, nValue.Minutes, nValue.Seconds);
                    m_CurrentTimeSpan = m_StartTimeSpan;
                    break;
            }
        }
    }
}
