using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

using Softpower.SmartMaker.TopApp.ValueConverter;
using Softpower.SmartMaker.TopControl.Components.ChartJS.Models;

namespace Softpower.SmartMaker.TopControl.Components.Clock
{
	public class ClockTimer : FrameworkElement
	{
        // DispatcherTimer 사용 할 때
        // System.Timers.Timer 로 대체
        // private DispatcherTimer m_timer;

        private Timer m_timer;  // System.Timers.Timer로 변경
        private Stopwatch m_stopwatch = new Stopwatch();  // Stopwatch를 추가하여 정밀한 시간 계산

        private DateTime m_StartTime = DateTime.Now;
        public event EventHandler TimerEnd;

        //Timer_Tick Event 중복 호출 방지용 플래그
        private bool isTicking = false;             

        // 스레드 안전성을 위한 Lock 객체
        // Timer의 타이머의 Elapsed 이벤트가 호출될 때, 여러 스레드가 타이머 연산을 시도하는 것을 방지하고 타이머의 연산이 순차적으로 진행.
        private readonly object lockObject = new object();

        // 타이머 시작 기준 시간
        private DateTime anchorTime; 

        private TimeSpan m_StartTimeSpan = new TimeSpan(0, 0, 0);
        private TimeSpan m_CurrentTimeSpan = new TimeSpan(0, 0, 0);
        private TimeSpan m_MaximumTimeSpan = new TimeSpan(23, 59, 59);

        // Function Flag
        // 0:시각, 1:스톱워치, 2:타이머
        private int m_nFunctionKind = 0;			

        private static DependencyProperty DateTimeProperty =
            DependencyProperty.Register("DateTime", typeof(object), typeof(ClockTimer),
            new PropertyMetadata(DateTime.Now));

        private static DependencyProperty TimeSpanProperty =
            DependencyProperty.Register("TimeSpan", typeof(TimeSpan), typeof(ClockTimer),
            new PropertyMetadata(TimeSpan.MinValue));

        private string m_strTimeZoneID;

        // 0:시각, 1:스톱워치, 2:타이머
        public int FunctionKind
		{
			set
			{
				m_nFunctionKind = value;

				switch (value)
				{
                    case 0:
                        if (m_timer != null) m_timer.Interval = 1000;
                        break;
                    case 1:
                        //정밀한 시간간격을 위해서 10ms 간격으로 갱신
                        if (m_timer != null) m_timer.Interval = 10;
                        break;
                    case 2:
                        if (m_timer != null) m_timer.Interval = 1000;
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

            /*
            // DispatcherTimer로 변경
            m_timer = new DispatcherTimer();
            m_timer.Tick += Timer_Tick;
            m_timer.Interval = TimeSpan.FromMilliseconds(10);  // 더 작은 간격으로 설정하여 정확성 증가
            */

            m_timer = new Timer();
            m_timer.Elapsed += Timer_Tick;
            // 타이머 10ms 간격(정밀하게 갱신)
            m_timer.Interval = 10;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (lockObject)
            {

                if (isTicking)
                {
                    return;
                }

                try
                {
                    isTicking = true;

                    // 현재 시간과 기준 시간의 차이를 계산
                    TimeSpan elapsedSinceAnchor = DateTime.Now - anchorTime;
                    long elapsedMilliseconds = (long)elapsedSinceAnchor.TotalMilliseconds;

                    // Debug.WriteLine($"Elapsed Time: {m_stopwatch.ElapsedMilliseconds} ms");


                    switch (m_nFunctionKind)
                    {
                        case 0: // 시각
                            Dispatcher.Invoke(() => SetValue(DateTime.Now));
                            break;

                        case 1: // 스톱워치
                            m_CurrentTimeSpan = m_StartTimeSpan + TimeSpan.FromMilliseconds(elapsedMilliseconds);
                            Dispatcher.Invoke(() => SetDisplayValue(m_CurrentTimeSpan));
                            break;

                        case 2: // 타이머
                            m_CurrentTimeSpan = m_StartTimeSpan - TimeSpan.FromMilliseconds(elapsedMilliseconds);
                            m_CurrentTimeSpan = TimeSpan.FromSeconds(Math.Round(m_CurrentTimeSpan.TotalSeconds)); // 초 단위로 반올림
                            Debug.WriteLine($"[Timer Mode] Current TimeSpan: {m_CurrentTimeSpan}  Elapsed Time: {m_stopwatch.ElapsedMilliseconds} ms");

                            if (m_CurrentTimeSpan.TotalSeconds <= 0)
                            {
                                m_CurrentTimeSpan = TimeSpan.Zero;
                                Stop();
                                TimerEnd?.Invoke(this, null);
                            }
                            
                            Dispatcher.Invoke(delegate { SetDisplayValue(m_CurrentTimeSpan); });


                            // Dispatcher.Invoke(() => SetDisplayValue(m_CurrentTimeSpan));
                            // 
                            break;
                    }
                }
                finally
                {
                    isTicking = false;
                }
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
                anchorTime = DateTime.Now;
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

        public void SetDisplayValue (TimeSpan nValue)
		{
			SetValue (DateTimeProperty, nValue);
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
