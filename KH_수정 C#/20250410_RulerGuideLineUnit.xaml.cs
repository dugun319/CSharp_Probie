using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.DelegateEventResource;

namespace Softpower.SmartMaker.TopControl.Components.TileAndRuler.RulerGuide.SubUnit
{
	/// <summary>
	/// Interaction logic for RulerGuideLineUnit.xaml
	/// </summary>
	public partial class RulerGuideLineUnit : Grid
	{

		public event CommonDelegateEvents.OnNotifyObjectEventHandler NotifyGuideLineSelected;
		public event CommonDelegateEvents.OnNotifyMethodExecutedEventHandler NotifyGuideLineMoveEnd;
		public event CommonDelegateEvents.OnNotifyMethodExecutedEventHandler NotifyGuideLineMoveStart;
		public event CommonDelegateEvents.OnNotifyArrayListEventHandler NotifyGuideLineMove;
		public event CommonDelegateEvents.OnNotifyBoolAndObjectEventHandler NotifyGuideLineToGlobalOrPage;
		public event CommonDelegateEvents.OnNotifyObjectEventHandler NotifyGuideLineDeleted;

		private RTConfig.RulerGuideLineDirectionType m_bGuideLineDirection = RTConfig.RulerGuideLineDirectionType.Horizontal;
		private bool m_bPreparedMove = false;
		private bool m_IsGlobal = false;
		private bool m_IsEBookModel = false;
		public bool IsVertical => GuideLineDirection == RTConfig.RulerGuideLineDirectionType.Vertical;

		public bool IsGlobal
		{
			get
			{
				return m_IsGlobal;
			}
			set
			{
				m_IsGlobal = value;
			}
		}

		public RTConfig.RulerGuideLineDirectionType GuideLineDirection
		{
			get
			{
				return m_bGuideLineDirection;
			}
			set
			{
				m_bGuideLineDirection = value;
			}
		}

		private bool PreparedMove
		{
			get
			{
				return m_bPreparedMove;
			}
			set
			{
				m_bPreparedMove = value;
			}
		}

		public RulerGuideLineUnit (bool IsEBookModel)
		{
			m_IsEBookModel = IsEBookModel;

			InitializeComponent ();
			InitEvent ();
			InitControl ();
			InitContextMenu ();
		}

		public RulerGuideLineUnit (bool IsEBookModel, RTConfig.RulerGuideLineDirectionType guideLineDirection)
			: this (IsEBookModel)
		{
			InitLayoutTransform (guideLineDirection);
		}

		public RulerGuideLineUnit (bool IsEBookModel, bool bIsHorizontal)
			: this (IsEBookModel)
		{
			InitLayoutTransform (true == bIsHorizontal ? RTConfig.RulerGuideLineDirectionType.Horizontal : RTConfig.RulerGuideLineDirectionType.Vertical);
		}

		private void InitLayoutTransform (RTConfig.RulerGuideLineDirectionType guideLineDirection)
		{
			this.GuideLineDirection = guideLineDirection;

			switch (guideLineDirection)
			{
				case RTConfig.RulerGuideLineDirectionType.Vertical:
					UnitRotateTrans.Angle = 90;
					//TextRotateTrans.Angle = -90;
					break;
			}
		}

		private void InitControl ()
		{
			GuideBody.StrokeThickness = RTConfig.RTManager ().RulerGuideLineUnit_BodyThickness;
			GuideBody.Fill = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_BodyNormalFillColor);
			GuideLine.StrokeThickness = RTConfig.RTManager ().RulerGuideLineUnit_LineThickness;

			if (false == IsGlobal)
			{
				GuideBody.Stroke = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_BodyNormalStrokeColor);
				GuideLine.Stroke = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_LineNomalStrokeColor);
			}
			else
			{
				GuideBody.Stroke = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_GlobalNormalColor);
				GuideLine.Stroke = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_GlobalNormalColor);
			}
		}

		private void InitEvent ()
		{
			//GuideBody.MouseLeftButtonDown += GuideBody_MouseLeftButtonDown;
			//GuideBody.MouseMove += GuideBody_MouseMove;
			//GuideBody.MouseLeftButtonUp += GuideBody_MouseLeftButtonUp;

			this.MouseLeftButtonDown += RulerGuideLineUnit_MouseLeftButtonDown;
			this.MouseMove += RulerGuideLineUnit_MouseMove;
			this.MouseLeftButtonUp += RulerGuideLineUnit_MouseLeftButtonUp;
		}

		private void InitContextMenu ()
		{
			ContextMenu contextMenu = new ContextMenu ();
			System.Windows.Controls.MenuItem deleteItem = new System.Windows.Controls.MenuItem ();
			System.Windows.Controls.MenuItem toGlobalOrLocalItem = new System.Windows.Controls.MenuItem ();

			toGlobalOrLocalItem.Header = LC.GS ("TopControl_RulerGuideLineUnit_2"); //모든 페이지만 적용
			deleteItem.Header = LC.GS ("TopControl_RulerGuideLineUnit_1");

			deleteItem.Click += DeleteItem_Click;
			toGlobalOrLocalItem.Click += ToGlobalOrLocalItem_Click;

			if (true == m_IsEBookModel)
			{
				contextMenu.Items.Add (toGlobalOrLocalItem);
			}
			contextMenu.Items.Add (deleteItem);
			GuideBody.ContextMenu = contextMenu;
		}

		void ToGlobalOrLocalItem_Click (object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.MenuItem glItem = sender as System.Windows.Controls.MenuItem;

			if (false == IsGlobal)
			{
				IsGlobal = true;
				glItem.Header = LC.GS ("TopControl_RulerGuideLineUnit_3");
			}
			else
			{
				IsGlobal = false;
				glItem.Header = LC.GS ("TopControl_RulerGuideLineUnit_4");
			}

			if (null != NotifyGuideLineToGlobalOrPage)
			{
				NotifyGuideLineToGlobalOrPage (IsGlobal, this);
			}
		}

		void DeleteItem_Click (object sender, RoutedEventArgs e)
		{
			Canvas parentCanas = Parent as Canvas;
			parentCanas.Children.Remove (this);

			if (null != NotifyGuideLineDeleted)
			{
				NotifyGuideLineDeleted (this);
			}
		}

		void GuideBody_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
		{
			if (null != NotifyGuideLineMoveEnd)
			{
				NotifyGuideLineMoveEnd ();
			}

			PreparedMove = false;
			GuideBody.ReleaseMouseCapture ();
			SetUnSelectedState ();
		}

		void GuideBody_MouseMove (object sender, MouseEventArgs e)
		{
			if (true == PreparedMove)
			{
				FrameworkElement parentAsFrameworkElement = this.Parent as FrameworkElement;
				ArrayList alParams = new ArrayList ();

				if (null != parentAsFrameworkElement)
				{
					Point ptMouse = Mouse.GetPosition (parentAsFrameworkElement);

					double dMaxVerticalOffSet = parentAsFrameworkElement.ActualHeight - GuideLine.Y1;
					double dMaxHorizontalOffSet = parentAsFrameworkElement.ActualWidth - GuideLine.Y1;
					//double dMinOffSet = RTConfig.RTManager().RulerHeight - GuideLine.Y1;
					double dMinOffSet = -GuideLine.Y1;

					if (dMinOffSet > ptMouse.Y)
					{
						ptMouse.Y = dMinOffSet;
					}

					if (dMinOffSet > ptMouse.X)
					{
						ptMouse.X = dMinOffSet;
					}

					if (dMaxVerticalOffSet < ptMouse.Y)
					{
						ptMouse.Y = dMaxVerticalOffSet;
					}

					if (dMaxHorizontalOffSet < ptMouse.X)
					{
						ptMouse.X = dMaxHorizontalOffSet;
					}

					switch (GuideLineDirection)
					{
						case RTConfig.RulerGuideLineDirectionType.Horizontal:
							Margin = new Thickness (0, ptMouse.Y, 0, 0);
							Tag = ptMouse.Y;
							alParams.Add (RTConfig.RulerGuideLineDirectionType.Horizontal);
							alParams.Add (ptMouse.Y + GuideLine.Y1);
							break;
						case RTConfig.RulerGuideLineDirectionType.Vertical:
							Margin = new Thickness (ptMouse.X, 0, 0, 0);
							Tag = ptMouse.X;
							alParams.Add (RTConfig.RulerGuideLineDirectionType.Vertical);
							alParams.Add (ptMouse.X + GuideLine.Y1);
							break;
					}

					if (null != NotifyGuideLineMove)
					{
						NotifyGuideLineMove (alParams);
					}
				}
			}
		}

		void GuideBody_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (null != NotifyGuideLineMoveStart)
			{
				NotifyGuideLineMoveStart ();
			}

			PreparedMove = true;
			GuideBody.CaptureMouse ();
			SetSelectedState ();
		}


		void RulerGuideLineUnit_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
		{
			if (null != NotifyGuideLineMoveEnd)
			{
				NotifyGuideLineMoveEnd ();
			}

			PreparedMove = false;
			GuideBody.ReleaseMouseCapture ();
			SetUnSelectedState ();
		}

		void RulerGuideLineUnit_MouseMove (object sender, MouseEventArgs e)
		{
			if (true == PreparedMove)
			{
				FrameworkElement parentAsFrameworkElement = this.Parent as FrameworkElement;
				ArrayList alParams = new ArrayList ();

				if (null != parentAsFrameworkElement)
				{
					Point ptMouse = Mouse.GetPosition (parentAsFrameworkElement);

					double dMaxVerticalOffSet = parentAsFrameworkElement.ActualHeight - GuideLine.Y1;
					double dMaxHorizontalOffSet = parentAsFrameworkElement.ActualWidth - GuideLine.Y1;
					//double dMinOffSet = RTConfig.RTManager().RulerHeight - GuideLine.Y1;
					double dMinOffSet = -GuideLine.Y1;

					if (dMinOffSet > ptMouse.Y)
					{
						ptMouse.Y = dMinOffSet;
					}

					if (dMinOffSet > ptMouse.X)
					{
						ptMouse.X = dMinOffSet;
					}

					if (dMaxVerticalOffSet < ptMouse.Y)
					{
						ptMouse.Y = dMaxVerticalOffSet;
					}

					if (dMaxHorizontalOffSet < ptMouse.X)
					{
						ptMouse.X = dMaxHorizontalOffSet;
					}

					switch (GuideLineDirection)
					{
						case RTConfig.RulerGuideLineDirectionType.Horizontal:
							Margin = new Thickness (0, ptMouse.Y, 0, 0);
							Tag = ptMouse.Y;
							alParams.Add (RTConfig.RulerGuideLineDirectionType.Horizontal);
							alParams.Add (ptMouse.Y + GuideLine.Y1);
							break;
						case RTConfig.RulerGuideLineDirectionType.Vertical:
							Margin = new Thickness (ptMouse.X, 0, 0, 0);
							Tag = ptMouse.X;
							alParams.Add (RTConfig.RulerGuideLineDirectionType.Vertical);
							alParams.Add (ptMouse.X + GuideLine.Y1);
							break;
					}

					if (null != NotifyGuideLineMove)
					{
						NotifyGuideLineMove (alParams);
					}
				}
			}
		}

		void RulerGuideLineUnit_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (null != NotifyGuideLineMoveStart)
			{
				NotifyGuideLineMoveStart ();
			}

			PreparedMove = true;
			GuideBody.CaptureMouse ();
			SetSelectedState ();
		}

		public void SetSelectedState ()
		{
			if (null != NotifyGuideLineSelected)
			{
				NotifyGuideLineSelected (this);
			}

			if (false == IsGlobal)
			{
				GuideBody.Fill = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_LineSelectedStrokeColor);
				GuideLine.Stroke = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_LineSelectedStrokeColor);
			}
			else
			{
				GuideBody.Fill = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_GlobalSelectedColor);
				GuideLine.Stroke = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_GlobalSelectedColor);
			}
		}

		public void SetUnSelectedState ()
		{
			if (false == IsGlobal)
			{
				GuideBody.Fill = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_BodyNormalFillColor);
				GuideLine.Stroke = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_LineNomalStrokeColor);
			}
			else
			{
				GuideBody.Fill = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_GlobalNormalColor);
				GuideLine.Stroke = new SolidColorBrush (RTConfig.RTManager ().RulerGuideLineUnit_GlobalNormalColor);
			}
		}

		public void GuideBody_MouseEnter (object sender, MouseEventArgs e)
		{
			//Cursor = Cursors.Hand;
		}

		public void GuideBody_MouseLeave (object sender, MouseEventArgs e)
		{
			//Cursor = Cursors.Arrow;
		}
	}
}
