using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom.Ebook.Components.SlideImage.Animation;
using Softpower.SmartMaker.TopAtom.Ebook.Components.SubControl;
using Softpower.SmartMaker.TopAtom.Helper;

namespace Softpower.SmartMaker.TopAtom.Ebook.Components
{
	/// <summary>
	/// Interaction logic for EBookSlideImage.xaml
	/// </summary>
	public partial class EBookSlideImage : Grid
	{
		public event CommonDelegateEvents.OnNotify2IntValueEventHandler NotifyAngleChanged;
		public event CommonDelegateEvents.OnNotify2IntValueEventHandler NotifyTitleHeightChanged;
		public event CommonDelegateEvents.OnNotify2IntValueEventHandler NotifyContentHeightChanged;
		public event CommonDelegateEvents.OnNotifyIntAndObjectEventHandler NotifyTitleChanged;
		public event CommonDelegateEvents.OnNotifyIntAndObjectEventHandler NotifyContentChanged;
		public event CommonDelegateEvents.OnNotify3IntValueEventHandler NotifyTextVerticalAlignChanged;
		public event CommonDelegateEvents.OnNotifyIntValueEventHandler NotifyMainTitleHeightChanged;
		public event CommonDelegateEvents.OnNotifyIntValueEventHandler NotifyMainTitleVerticalAlignChanged;
		public event CommonDelegateEvents.OnNotifyObjectEventHandler NotifyMainTitleChanged;

		public event CommonDelegateEvents.OnGetObjectEventHandler GetThirdControlLayer;

		private string DefaultLeftImage = "Softpower.SmartMaker.TopAtom.AtomImages.SlideLeft.png";
		private string DefaultRightImage = "Softpower.SmartMaker.TopAtom.AtomImages.SlideRight.png";

		private SolidColorBrush m_SelectedColor = new SolidColorBrush (Color.FromArgb (255, 13, 133, 133));
		private SolidColorBrush m_DefaultColor = new SolidColorBrush (Colors.LightGray);
		private SolidColorBrush m_MouseEnterColor = new SolidColorBrush (Colors.DarkGray);
		private FrameworkElement m_SelectedSlideButton = null;
		private ScaleTransform m_SelectedImageTrans = null;
		private ScaleTransform m_MouseEnterImageTrans = null;

		public int m_nSlideCount = 0;
		private Orientation m_ButtonOrientation = Orientation.Horizontal;
		//정렬
		private int m_nButtonAlign = 0;
		private Dictionary<int, EBookImageInfo> m_ImageInfos;
		private Dictionary<int, EBookVoiceInfo> m_VoiceInfos;
		private Dictionary<int, BitmapImage> m_Images;

		private DispatcherTimer m_SlideTimer = null;

        // 20250203 KH 북모델 이미지표시란 아톰  - 좌우슬라이딩 효과 보완 - 마지막일때  회전될수 있도록 논리보완
        // 20250203 KH Variant For SlideImage AutoPlay
        private int totalCount;

		bool m_bIsRunMode = false;
		private Atom m_pOwnerAtom;

		//2014-11-10
		public event CommonDelegateEvents.OnNotifyMethodExecutedEventHandler NotifyEditModeChange;

		private RotateTransform ImageTransform;

		private int m_nGifPlayType = -1;
		private int m_nGifPlayCount = 0;

		//2019-11-20 kys
		private bool m_bAutoPlay = false;
		private int m_nSlideAnimationType;   // 애니메이션효과 (0:페이드인아웃, 1:좌우슬라이딩, 2:입체박스효과, 3:커버플로우, 4:수평회전목마, 5:수직회전목마)

		// 애니메이션효과
		private SiideImageAnimation m_SiideImageAnimation;

		//private CubeAnimationControl m_CubeAnimationControl; 
		//private CoverFlowControl m_CoverFlowControl;
		//private CarouselAnimationControl m_CarouselAnimationControl; 
		//

		private bool IsRunMode
		{
			get
			{
				return m_bIsRunMode;
			}
			set
			{
				m_bIsRunMode = value;
			}
		}

		private DispatcherTimer SlideTimer
		{
			get { return m_SlideTimer; }
			set { m_SlideTimer = value; }
		}

		public new SolidColorBrush Background
		{
			get
			{
				return ImageUnit1.Background;
			}
			set
			{
				ImageUnit1.Background = ImageUnit2.Background = value;
			}
		}


		public Thickness BorderThickness
		{
			get
			{
				return ImageUnit1.BorderThickness;
			}
			set
			{
				ImageUnit1.BorderThickness = ImageUnit2.BorderThickness = value;
			}

		}

		public DoubleCollection DashArray
		{
			get
			{
				return ImageUnit1.DashArray;
			}
			set
			{
				ImageUnit1.DashArray = ImageUnit2.DashArray = value;
			}
		}

		public SolidColorBrush BorderBrush
		{
			get
			{
				return ImageUnit1.BorderBrush;
			}
			set
			{
				ImageUnit1.BorderBrush = ImageUnit2.BorderBrush = value;
			}
		}

		public bool IsDeepEditMode
		{
			get
			{
				return ImageUnit1.IsDeepEditMode;
			}
			set
			{
				if (true == IsRunMode)
				{
					return;
				}

				ImageUnit1.IsDeepEditMode = value;
				ImageUnit2.IsDeepEditMode = value;

				TopLine.Visibility = true == value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
				TopSplitter.Visibility = true == value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
				TitleTextUnit.IsDeepEditMode = value;
			}
		}

		public void SettingRotateTrans (RotateTransform tempTrans)
		{
			ImageTransform = tempTrans;
			ImageUnit1.SettingRotateTrans (tempTrans);
			ImageUnit2.SettingRotateTrans (tempTrans);
		}

		public ScaleTransform SelectedImageTrans
		{
			get
			{
				if (null == m_SelectedImageTrans)
				{
					m_SelectedImageTrans = new ScaleTransform (1.2, 1.2);
				}
				return m_SelectedImageTrans;
			}
		}

		public ScaleTransform MouseEnterImageTrans
		{
			get
			{
				if (null == m_MouseEnterImageTrans)
				{
					m_MouseEnterImageTrans = new ScaleTransform (1.1, 1.1);
				}
				return m_MouseEnterImageTrans;
			}
		}

		public Type ButtonType
		{
			get
			{
				return ButtonContainer.Children[0].GetType ();
			}
		}

		public int ButtonTypeIndex
		{
			get
			{
				Type bType = ButtonType;

				if (bType == typeof (Ellipse))
				{
					return 0;
				}
				else if (bType == typeof (Rectangle))
				{
					return 1;
				}
				else if (bType == typeof (TextBlock))
				{
					return 2;
				}
				else if (bType == typeof (Image))
				{
					return 3;
				}
				else if (bType == typeof (Border))
				{
					return 4;
				}
				else
				{
					return 0;
				}
			}
		}

		public FrameworkElement SelectedButton
		{
			get
			{
				return m_SelectedSlideButton;
			}
			set
			{
				if (null != m_SelectedSlideButton)
				{
					if (m_SelectedSlideButton is Shape)
					{
						((Shape)m_SelectedSlideButton).Fill = m_DefaultColor;
					}
					else if (m_SelectedSlideButton is TextBlock)
					{
						((TextBlock)m_SelectedSlideButton).Foreground = m_MouseEnterColor;
					}
					else if (m_SelectedSlideButton is Image)
					{
						((Image)m_SelectedSlideButton).RenderTransform = null;
					}
					else if (m_SelectedSlideButton is Border)
					{
					}
				}

				m_SelectedSlideButton = value;

				if (null != value)
				{
					switch (ButtonTypeIndex)
					{
						case 0:
						case 1:
							((Shape)m_SelectedSlideButton).Fill = m_SelectedColor;
							break;
						case 2:
							((TextBlock)m_SelectedSlideButton).Foreground = m_SelectedColor;
							break;
						case 3:
							SelectedImageTrans.CenterX = m_SelectedSlideButton.ActualWidth / 2;
							SelectedImageTrans.CenterY = m_SelectedSlideButton.ActualHeight / 2;
							((Image)m_SelectedSlideButton).RenderTransform = SelectedImageTrans;
							break;
						case 4:
							break;
					}
				}
			}
		}

		public Dictionary<int, EBookImageInfo> ImageInfoDic
		{
			get
			{
				return m_ImageInfos;
			}
			set
			{
				m_ImageInfos = value;
			}
		}

		public Dictionary<int, EBookVoiceInfo> VoiceInfoDic
		{
			get
			{
				return m_VoiceInfos;
			}
			set
			{
				m_VoiceInfos = value;
			}
		}

		public Dictionary<int, BitmapImage> ImageDic
		{
			get
			{
				return m_Images;
			}
			set
			{
				m_Images = value;
			}
		}


		/// <summary>
		/// 0 == Left
		/// 1 == center
		/// 2 == right
		/// 3 == stretch
		/// </summary>
		public int ButtonAlign
		{
			get
			{
				return m_nButtonAlign;
			}
			set
			{
				m_nButtonAlign = value;
			}
		}


		public int SlideCount
		{
			get
			{
				return m_nSlideCount;
			}
			set
			{
				m_nSlideCount = value;
			}
		}

		public int CurrentImageIndex
		{
			get
			{
				return (int)SelectedButton.Tag;
			}
		}

		public Orientation ButtonOrientation
		{
			get
			{
				return m_ButtonOrientation;
			}
			set
			{
				m_ButtonOrientation = value;
			}
		}

		public bool IsEnabledScroll
		{
			get { return ImageUnit1.IsEnabledScroll; }
			set
			{
				ImageUnit1.IsEnabledScroll = value;
				ImageUnit2.IsEnabledScroll = value;
			}
		}

		public EBookSlideImage ()
		{
			InitializeComponent ();
			InitEvent ();
			ButtonTypeCountChanged (0, 2);
			InitButtonImage ();
		}

		private void InitEvent ()
		{
			ImageUnit1.NotifyAngleChanged += ImageUnit_NotifyAngleChanged;
			ImageUnit2.NotifyAngleChanged += ImageUnit_NotifyAngleChanged;

			ImageUnit1.NotifyTitleHeightChanged += ImageUnit_NotifyTitleHeightChanged;
			ImageUnit2.NotifyTitleHeightChanged += ImageUnit_NotifyTitleHeightChanged;

			ImageUnit1.NotifyContentHeightChanged += ImageUnit_NotifyContentHeightChanged;
			ImageUnit2.NotifyContentHeightChanged += ImageUnit_NotifyContentHeightChanged;

			ImageUnit1.NotifyTitleChanged += ImageUnit_NotifyTitleChanged;
			ImageUnit2.NotifyTitleChanged += ImageUnit_NotifyTitleChanged;

			ImageUnit1.NotifyContentChanged += ImageUnit_NotifyContentChanged;
			ImageUnit2.NotifyContentChanged += ImageUnit_NotifyContentChanged;

			ImageUnit1.GetThirdControlLayer += ImageUnit_GetThirdControlLayer;
			ImageUnit2.GetThirdControlLayer += ImageUnit_GetThirdControlLayer;

			ImageUnit1.NotifyTextVerticalAlignChanged += ImageUnit_NotifyTextVerticalAlignChanged;
			ImageUnit2.NotifyTextVerticalAlignChanged += ImageUnit_NotifyTextVerticalAlignChanged;


			TitleTextUnit.GetThirdControlLayer += ImageUnit_GetThirdControlLayer;
			TitleTextUnit.NotifyDocumentChanged += TitleTextUnit_NotifyDocumentChanged;
			TopSplitter.DragCompleted += TopSplitter_DragCompleted;


			//2014-11-10 
			ImageUnit1.NotifyEditModeChange += ImageUnit_NotifyEditModeChange;
			TitleTextUnit.NotifyEditModeChange += ImageUnit_NotifyEditModeChange;

		}

		private void InitButtonImage ()
		{
			BitmapImage leftImage = ManageAtomImage.GetAtomImageFromBase (DefaultLeftImage, typeof (EBookImageAtomBase));
			BitmapImage RightImage = ManageAtomImage.GetAtomImageFromBase (DefaultRightImage, typeof (EBookImageAtomBase));

			if (null != leftImage) LeftButton.Source = leftImage;
			if (null != RightImage) RightButton.Source = RightImage;
		}

		void ImageUnit_NotifyEditModeChange ()
		{
			if (null != NotifyEditModeChange)
				NotifyEditModeChange ();
		}

		void TitleTextUnit_NotifyDocumentChanged (object objValue)
		{
			if (null != NotifyMainTitleChanged)
			{
				NotifyMainTitleChanged (objValue);
			}
		}

		void ImageUnit_NotifyTextVerticalAlignChanged (int nValue1, int nValue2, int nValue3)
		{
			if (null != NotifyTextVerticalAlignChanged)
			{
				NotifyTextVerticalAlignChanged (nValue1, nValue2, nValue3);
			}
		}

		void TopSplitter_DragCompleted (object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			if (null != NotifyMainTitleHeightChanged)
			{
				NotifyMainTitleHeightChanged ((int)TitleTextUnit.ActualHeight);
			}
		}

		private void ImageUnit_NotifyContentChanged (int nValue, object objValue)
		{
			if (null != NotifyContentChanged)
			{
				NotifyContentChanged (nValue, objValue);
			}
		}

		private void ImageUnit_NotifyTitleChanged (int nValue, object objValue)
		{
			if (null != NotifyTitleChanged)
			{
				NotifyTitleChanged (nValue, objValue);
			}
		}

		private object ImageUnit_GetThirdControlLayer ()
		{
			if (null != GetThirdControlLayer)
			{
				return GetThirdControlLayer ();
			}
			return null;
		}

		void ImageUnit_NotifyContentHeightChanged (int nValue1, int nValue2)
		{
			if (null != NotifyContentHeightChanged)
			{
				NotifyContentHeightChanged (nValue1, nValue2);
			}
		}

		void ImageUnit_NotifyTitleHeightChanged (int nValue1, int nValue2)
		{
			if (null != NotifyTitleHeightChanged)
			{
				NotifyTitleHeightChanged (nValue1, nValue2);
			}
		}

		void ImageUnit_NotifyAngleChanged (int nValue1, int nValue2)
		{
			if (null != NotifyAngleChanged)
			{
				NotifyAngleChanged (nValue1, nValue2);
			}
		}


		#region Slide Button

		public void MakeView (int nWillButtonType, int nWillCount, int nButtonAlign, int nButtonPosition, int nButtonWidth, int nButtonHeight)
		{
			ButtonTypeCountChanged (nWillButtonType, nWillCount);
			ButtonAlignChanged (nButtonAlign);
			ButtonPositionChanged (nButtonPosition);
			if (4 != nWillButtonType) ButtonSizeChanged (nButtonWidth, nButtonHeight, 2);

			if (4 == nWillButtonType)
			{
				this.LeftButton.Visibility = System.Windows.Visibility.Visible;
				this.RightButton.Visibility = System.Windows.Visibility.Visible;
				LeftButton.Width = nButtonWidth;
				RightButton.Width = nButtonWidth;
				LeftButton.Height = nButtonHeight;
				RightButton.Height = nButtonHeight;
			}
			else
			{
				this.LeftButton.Visibility = System.Windows.Visibility.Collapsed;
				this.RightButton.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		public void MakeView (EBookSlideImageAttrib atomAttrib)
		{
			ButtonTypeCountChanged (atomAttrib.ButtonType, atomAttrib.SlideCount);
			ButtonAlignChanged (atomAttrib.ButtonAlign);
			ButtonPositionChanged (atomAttrib.ButtonPosition);
			UpdateArrowImage (atomAttrib);
			if (4 != atomAttrib.ButtonType) ButtonSizeChanged (atomAttrib.ButtonWidth, atomAttrib.ButtonHeight, 2);
		}

		private void UpdateArrowImage (EBookSlideImageAttrib atomAttrib)
		{
			this.LeftButton.Visibility = 4 == atomAttrib.ButtonType ? Visibility.Visible : Visibility.Collapsed;
			this.RightButton.Visibility = 4 == atomAttrib.ButtonType ? Visibility.Visible : Visibility.Collapsed;

			LeftButton.Width = atomAttrib.ButtonWidth;
			RightButton.Width = atomAttrib.ButtonWidth;
			LeftButton.Height = atomAttrib.ButtonHeight;
			RightButton.Height = atomAttrib.ButtonHeight;

			int nLeftImageKey = atomAttrib.LeftImageKey;
			int nRightImageKey = atomAttrib.RightImageKey;

			if (-1 < nLeftImageKey)
			{
				CObjectImage objectImage = null;
				atomAttrib.GetGDIObjFromKey (ref objectImage, nLeftImageKey);

				if (null != objectImage)
				{
					LeftButton.Source = objectImage.SelectImage;
				}
			}
			else
			{
				BitmapImage leftImage = ManageAtomImage.GetAtomImageFromBase (DefaultLeftImage, typeof (EBookImageAtomBase));

				if (null != leftImage) LeftButton.Source = leftImage;
			}

			if (-1 < nRightImageKey)
			{
				CObjectImage objectImage = null;
				atomAttrib.GetGDIObjFromKey (ref objectImage, nRightImageKey);

				if (null != objectImage)
				{
					RightButton.Source = objectImage.SelectImage;
				}
			}
			else
			{
				BitmapImage RightImage = ManageAtomImage.GetAtomImageFromBase (DefaultRightImage, typeof (EBookImageAtomBase));
				if (null != RightImage) RightButton.Source = RightImage;
			}
		}

		/// <summary>
		/// 슬라이드 버튼 생성
		/// </summary>
		/// <param name="nWillCount"></param>
		private void ButtonTypeCountChanged (int nWillButtonType, int nWillCount)
		{
			Type buttonType = null;

			switch (nWillButtonType)
			{
				case 0:
					buttonType = typeof (Ellipse);
					break;
				case 1:
					buttonType = typeof (Rectangle);
					break;
				case 2:
					buttonType = typeof (TextBlock);
					break;
				case 3:
					buttonType = typeof (Image);
					break;
				case 4: // 화살표
				case 5: // 표시안함
					buttonType = typeof (Border);
					break;

			}

			object objCurrentButton = null;
			Type currentButtonType = null;

			if (0 < ButtonContainer.Children.Count)
			{
				objCurrentButton = ButtonContainer.Children[0];
				currentButtonType = objCurrentButton.GetType ();
			}
			else
			{
				currentButtonType = typeof (Ellipse);
			}

			//갯수 변경의 경우
			if (buttonType == currentButtonType)
			{
				if (SlideCount == nWillCount)
				{
					return;
				}

				if (SlideCount < nWillCount)
				{
					for (int n = 0; n < nWillCount - SlideCount; n++)
					{
						MakeDivision (nWillCount + n);

						if (buttonType == typeof (Ellipse))
						{
							ButtonContainer.Children.Add (MakeEllipseButton (SlideCount + n));
						}
						else if (buttonType == typeof (Rectangle))
						{
							ButtonContainer.Children.Add (MakeRectangleButton (SlideCount + n));
						}
						else if (buttonType == typeof (TextBlock))
						{
							//TextBlock lastTextBlock = ButtonContainer.Children[SlideCount - 1] as TextBlock;
							//int nLastIndex = int.Parse(lastTextBlock.Text);
							//ButtonContainer.Children.Add(MakeNumberButton(nLastIndex));
							ButtonContainer.Children.Add (MakeNumberButton (SlideCount + n));
						}
						else if (buttonType == typeof (Image))
						{
							ButtonContainer.Children.Add (MakeThumbnailButton (SlideCount + n));
						}
						else if (buttonType == typeof (Border))
						{
							ButtonContainer.Children.Add (MakeBorderButton (SlideCount + n));
						}
					}
				}
				else if (SlideCount > nWillCount)
				{
					ButtonContainer.Children.RemoveRange (nWillCount, SlideCount - nWillCount);
					RemoveDivision (nWillCount, SlideCount - nWillCount);
				}
			}
			//타입의 변환
			else
			{
				ButtonContainer.Children.Clear ();

				for (int i = 0; i < nWillCount; i++)
				{
					MakeDivision (i);

					if (buttonType == typeof (Ellipse))
					{
						ButtonContainer.Children.Add (MakeEllipseButton (i));
					}
					else if (buttonType == typeof (Rectangle))
					{
						ButtonContainer.Children.Add (MakeRectangleButton (i));
					}
					else if (buttonType == typeof (TextBlock))
					{
						//TextBlock lastTextBlock = ButtonContainer.Children[SlideCount - 1] as TextBlock;
						//int nLastIndex = int.Parse(lastTextBlock.Text);
						//ButtonContainer.Children.Add(MakeNumberButton(nLastIndex));
						ButtonContainer.Children.Add (MakeNumberButton (i));
					}
					else if (buttonType == typeof (Image))
					{
						ButtonContainer.Children.Add (MakeThumbnailButton (i));
					}
					else if (buttonType == typeof (Border))
					{
						ButtonContainer.Children.Add (MakeBorderButton (i));
					}
				}

				RemoveDivision (nWillCount);
			}

			SlideCount = nWillCount;
		}

		private void SetButtonCommon (FrameworkElement feSlideButton, int nIndex)
		{
			if (Orientation.Horizontal == ButtonOrientation)
			{
				Grid.SetColumn (feSlideButton, nIndex);
			}
			else
			{
				Grid.SetRow (feSlideButton, nIndex);
			}
			//태그에 슬라이드 번호넣음
			feSlideButton.Tag = nIndex + 1;
			feSlideButton.MouseEnter += SlideButton_MouseEnter;
			feSlideButton.MouseLeave += SlideButton_MouseLeave;
			feSlideButton.MouseLeftButtonDown += SlideButton_MouseLeftButtonDown;
		}

		void SlideButton_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			ImageUnit1.StopFormLoadedGif ();
			ImageUnit2.StopFormLoadedGif ();

			FrameworkElement feButton = sender as FrameworkElement;
			SelectedButton = feButton;

			int nSlideIndex = (int)SelectedButton.Tag;
			ViewToSlide (nSlideIndex);
		}

		void SlideButton_MouseLeave (object sender, MouseEventArgs e)
		{
			FrameworkElement feButton = sender as FrameworkElement;

			if (SelectedButton == feButton)
			{
				return;
			}

			SlideButtonMouseLeave (feButton);
		}

		public void SlideButtonMouseLeave (FrameworkElement feButton)
		{
			switch (ButtonTypeIndex)
			{
				case 0:
				case 1:
					((Shape)feButton).Fill = m_DefaultColor;
					break;
				case 2:
					((TextBlock)feButton).FontWeight = FontWeights.Normal;
					break;
				case 3:
					((Image)feButton).RenderTransform = null;
					break;
			}
		}

		void SlideButton_MouseEnter (object sender, MouseEventArgs e)
		{
			FrameworkElement feButton = sender as FrameworkElement;

			if (SelectedButton == feButton)
			{
				return;
			}

			SlideButtonMouseEnter (feButton);
		}

		void SlideButtonMouseEnter (FrameworkElement feButton)
		{
			switch (ButtonTypeIndex)
			{
				case 0:
				case 1:
					((Shape)feButton).Fill = m_MouseEnterColor;
					break;
				case 2:
					((TextBlock)feButton).FontWeight = FontWeights.Bold;
					break;
				case 3:
					MouseEnterImageTrans.CenterX = feButton.ActualWidth / 2;
					MouseEnterImageTrans.CenterY = feButton.ActualHeight / 2;
					((Image)feButton).RenderTransform = MouseEnterImageTrans;
					break;
			}
		}

		private Ellipse MakeEllipseButton (int nIndex)
		{
			Ellipse thumbEllp = new Ellipse ();
			thumbEllp.Fill = m_DefaultColor;
			SetButtonCommon (thumbEllp, nIndex);
			return thumbEllp;
		}

		private Rectangle MakeRectangleButton (int nIndex)
		{
			Rectangle thumbRect = new Rectangle ();
			thumbRect.Fill = m_DefaultColor;
			SetButtonCommon (thumbRect, nIndex);
			return thumbRect;
		}

		private TextBlock MakeNumberButton (int nIndex)
		{
			TextBlock thumbNum = new TextBlock ();
			thumbNum.Foreground = m_MouseEnterColor;
			thumbNum.Text = (nIndex + 1).ToString ();
			SetButtonCommon (thumbNum, nIndex);
			return thumbNum;
		}

		private Image MakeThumbnailButton (int nIndex)
		{
			Image thumbImage = new Image ();
			thumbImage.Source = EmptyImage.Instance.Image;
			thumbImage.Stretch = Stretch.Fill;
			SetButtonCommon (thumbImage, nIndex);
			return thumbImage;
		}

		private Border MakeBorderButton (int nIndex)
		{
			Border thumbEllp = new Border ();
			SetButtonCommon (thumbEllp, nIndex);
			return thumbEllp;
		}

		public void ButtonSizeChanged (int nWidth, int nHeight, int nMargin)
		{
			foreach (FrameworkElement feChild in ButtonContainer.Children)
			{
				feChild.Height = nHeight;
				feChild.Width = nWidth;
				feChild.Margin = new Thickness (nMargin);
			}
		}


		private void RemoveDivision (int nIndex)
		{
			if (Orientation.Horizontal == ButtonOrientation)
			{
				int nCount = ButtonContainer.ColumnDefinitions.Count - nIndex;

				if (0 == nCount)
				{
					return;
				}

				ButtonContainer.ColumnDefinitions.RemoveRange (nIndex, nCount);
			}
			else
			{
				int nCount = ButtonContainer.RowDefinitions.Count - nIndex;

				if (0 == nCount)
				{
					return;
				}

				ButtonContainer.RowDefinitions.RemoveRange (nIndex, nCount);
			}
		}

		private void RemoveDivision (int nIndex, int nCount)
		{
			if (Orientation.Horizontal == ButtonOrientation)
			{
				ButtonContainer.ColumnDefinitions.RemoveRange (nIndex, nCount);
			}
			else
			{
				ButtonContainer.RowDefinitions.RemoveRange (nIndex, nCount);
			}
		}

		private void MakeDivision (int n)
		{

			if (Orientation.Horizontal == ButtonOrientation)
			{
				if (ButtonContainer.ColumnDefinitions.Count > n)
				{
					return;
				}
			}
			else
			{
				if (ButtonContainer.RowDefinitions.Count > n)
				{
					return;
				}
			}

			GridLength gLength = new GridLength (1, GridUnitType.Auto);

			switch (ButtonAlign)
			{
				case 3:
					gLength = new GridLength (1, GridUnitType.Star);
					break;
			}

			if (Orientation.Horizontal == ButtonOrientation)
			{
				ColumnDefinition colDef = new ColumnDefinition ();
				colDef.Width = gLength;
				ButtonContainer.ColumnDefinitions.Add (colDef);
			}
			else
			{
				RowDefinition rowDef = new RowDefinition ();
				rowDef.Height = gLength;
				ButtonContainer.RowDefinitions.Add (rowDef);
			}
		}

		/// <summary>
		/// 위치변경
		/// 0 == left
		/// 1 == top
		/// 2 == right
		/// 3 == bottom
		/// </summary>
		/// <param name="nPos"></param>
		public void ButtonPositionChanged (int nPos)
		{
			switch (nPos)
			{
				case 0:
					Grid.SetRow (ButtonContainer, 1);
					Grid.SetColumn (ButtonContainer, 0);
					ButtonContainer.HorizontalAlignment = HorizontalAlignment.Left;
					break;
				case 1:
					Grid.SetRow (ButtonContainer, 0);
					Grid.SetColumn (ButtonContainer, 1);
					ButtonContainer.VerticalAlignment = VerticalAlignment.Top;
					break;
				case 2:
					Grid.SetRow (ButtonContainer, 1);
					Grid.SetColumn (ButtonContainer, 2);
					ButtonContainer.HorizontalAlignment = HorizontalAlignment.Right;
					break;
				case 3:
					Grid.SetRow (ButtonContainer, 2);
					Grid.SetColumn (ButtonContainer, 1);
					ButtonContainer.VerticalAlignment = VerticalAlignment.Bottom;
					break;
			}




			switch (nPos)
			{
				case 1:
				case 3:

					if (Orientation.Horizontal == ButtonOrientation)
					{
						return;
					}

					ButtonOrientation = Orientation.Horizontal;

					if (0 < ButtonContainer.ColumnDefinitions.Count)
					{
						return;
					}

					int nColCount = ButtonContainer.RowDefinitions.Count;

					ButtonContainer.RowDefinitions.Clear ();

					for (int n = 0; n < nColCount; n++)
					{
						MakeDivision (n);
						Grid.SetRow (ButtonContainer.Children[n], 0);
						Grid.SetColumn (ButtonContainer.Children[n], n);
					}

					break;
				case 0:
				case 2:

					if (Orientation.Vertical == ButtonOrientation)
					{
						return;
					}

					ButtonOrientation = Orientation.Vertical;

					if (0 < ButtonContainer.RowDefinitions.Count)
					{
						return;
					}

					int nRowCount = ButtonContainer.ColumnDefinitions.Count;

					ButtonContainer.ColumnDefinitions.Clear ();

					for (int n = 0; n < nRowCount; n++)
					{
						MakeDivision (n);
						Grid.SetRow (ButtonContainer.Children[n], n);
						Grid.SetColumn (ButtonContainer.Children[n], 0);
					}

					break;
			}
		}

		/// <summary>
		/// 정렬 변경
		/// </summary>
		/// <param name="nAlign"></param>
		public void ButtonAlignChanged (int nAlign)
		{
			ButtonContainer.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

			switch (nAlign)
			{
				case 0:
					ButtonContainer.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
					ButtonContainer.VerticalAlignment = System.Windows.VerticalAlignment.Top;
					break;
				case 1:
					ButtonContainer.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
					ButtonContainer.VerticalAlignment = System.Windows.VerticalAlignment.Center;
					break;
				case 2:
					ButtonContainer.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
					ButtonContainer.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
					break;
				case 3:
					ButtonContainer.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
					ButtonContainer.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
					break;
			}

			switch (nAlign)
			{
				case 0:
				case 1:
				case 2:
					if (Orientation.Horizontal == ButtonOrientation)
					{
						foreach (ColumnDefinition colDef in ButtonContainer.ColumnDefinitions)
						{
							colDef.Width = new GridLength (1, GridUnitType.Auto);
						}
					}
					else
					{
						foreach (RowDefinition rowDef in ButtonContainer.RowDefinitions)
						{
							rowDef.Height = new GridLength (1, GridUnitType.Auto);
						}
					}
					break;
				case 3:
					if (Orientation.Horizontal == ButtonOrientation)
					{
						foreach (ColumnDefinition colDef in ButtonContainer.ColumnDefinitions)
						{
							colDef.Width = new GridLength (1, GridUnitType.Star);
						}

					}
					else
					{
						foreach (RowDefinition rowDef in ButtonContainer.RowDefinitions)
						{
							rowDef.Height = new GridLength (1, GridUnitType.Star);
						}
					}

					foreach (FrameworkElement button in ButtonContainer.Children)
					{
						button.Width = double.NaN;
						button.Height = double.NaN;

						if (button is Image imageButton)
						{
							imageButton.Stretch = Stretch.Fill;
						}
					}
					break;
			}
		}

		#endregion Slide Button

		public void SetImageInfo(Dictionary<int, EBookImageInfo> imageInfoDic, Dictionary<int, BitmapImage> bitImageDic, int nGifPlayType, int nGifPlayCount)
		{
			ImageInfoDic = imageInfoDic;
			ImageDic = bitImageDic;
			ImageUnit1.ClearImageInfo ();

            // 20250203 KH 북모델 이미지표시란 아톰  - 좌우슬라이딩 효과 보완 - 마지막일때  회전될수 있도록 논리보완
            totalCount = imageInfoDic.Count;

            if (0 == imageInfoDic.Count || 0 >= ButtonContainer.Children.Count || null == ButtonContainer.Children[0])
			{
				return;
			}

            ImageUnit1.ImageIndex = 1;

			//첫페이지가 있을때
			if (true == imageInfoDic.ContainsKey (1))
			{
				ImageUnit1.SetImageInfo (imageInfoDic[1], bitImageDic[1], nGifPlayType, nGifPlayCount);
				SelectedButton = ButtonContainer.Children[0] as FrameworkElement;
			}

			Type buttonType = ButtonContainer.Children[0].GetType ();

			if (buttonType == typeof (Image))
			{
				foreach (Image imagButton in ButtonContainer.Children)
				{
					int nKey = (int)imagButton.Tag;

					if (true == ImageDic.ContainsKey (nKey))
					{
						imagButton.Source = ImageDic[nKey];
					}
				}
			}
		}


        public void SetVoiceInfo (Dictionary<int, EBookVoiceInfo> voiceInfoDic)
		{
			VoiceInfoDic = voiceInfoDic;

			if (0 == voiceInfoDic.Count || 0 >= ButtonContainer.Children.Count || null == ButtonContainer.Children[0])
			{
				return;
			}

			//첫페이지가 있을때
			if (true == voiceInfoDic.ContainsKey (1))
			{
				ImageUnit1.SetVoiceInfo (voiceInfoDic[1]);
			}
		}

		public void ViewToSlide (int nSlideIndex)
		{
			ViewToSlide (nSlideIndex, true);
		}

		public void ViewToSlide (int nSlideIndex, bool bSlideAnimation)
		{
			ImageUnit1.EndTextEdit ();
			ImageUnit2.Visibility = System.Windows.Visibility.Visible;

			int nCurrentSlideIndex = ImageUnit1.ImageIndex;

			if (true == ImageInfoDic.ContainsKey (nCurrentSlideIndex))
			{
				BitmapImage currentBitImage = ImageDic.ContainsKey (nCurrentSlideIndex) ? ImageDic[nCurrentSlideIndex] : null;
				ImageUnit2.ImageIndex = nCurrentSlideIndex;
				ImageUnit2.SetImageInfo (ImageInfoDic[nCurrentSlideIndex], currentBitImage, m_nGifPlayType, m_nGifPlayCount);
			}

			//
			if (true == VoiceInfoDic.ContainsKey (nCurrentSlideIndex))
			{
				ImageUnit2.SetVoiceInfo (VoiceInfoDic[nCurrentSlideIndex]);
			}
			//

			ImageUnit1.ClearImageInfo ();
			ImageUnit1.ClearDocumentInfo ();
			ImageUnit1.ClearVoiceInfo ();

			if (true == ImageInfoDic.ContainsKey (nSlideIndex))
			{
				BitmapImage bitImage = ImageDic.ContainsKey (nSlideIndex) ? ImageDic[nSlideIndex] : null;

				ImageUnit1.ImageIndex = nSlideIndex;
				ImageUnit1.SetImageInfo (ImageInfoDic[nSlideIndex], bitImage, m_nGifPlayType, m_nGifPlayCount); //수정필요
				ImageUnit1.SetDocumentInfo (ImageInfoDic[nSlideIndex]);
			}

			if (true == VoiceInfoDic.ContainsKey (nSlideIndex))
			{
				ImageUnit1.SetVoiceInfo (VoiceInfoDic[nSlideIndex]);

				if (true == m_bAutoPlay)
					ImageUnit1.StartPlaySound (); // 2019-11-20 kys 슬라이드아톰 자동진행일경우 클릭 안해도 소리 출력되도록 기능보강함
			}

			// 슬라이드 효과
			if (false != bSlideAnimation)
			{
				if (null != m_pOwnerAtom && 1 == m_pOwnerAtom.GetOfAtom ().AtomCore.AtomRunMode)
				{
					SlideImageAnimation (nSlideIndex);
				}
			}
			//

			PlayFormLoadedGif ();

			Dispatcher.BeginInvoke (DispatcherPriority.Background, new System.Action (delegate ()
			{
				if (null != m_pOwnerAtom && 1 == m_pOwnerAtom.GetOfAtom ().AtomCore.AtomRunMode)
				{
					CVariantX[] pvaArgs = new CVariantX[1 + 1];
					pvaArgs[0] = new CVariantX (1);
					pvaArgs[1] = new CVariantX (nSlideIndex);

					if (-1 != this.m_pOwnerAtom.ProcessEvent (0, EVS_TYPE.EVS_A_SEL_CHANGE, pvaArgs))
					{
						if (0 <= MsgHandler.CALL_MSG_HANDLER (m_pOwnerAtom, EVS_TYPE.EVS_A_SEL_CHANGE, pvaArgs))
						{
							this.m_pOwnerAtom.ProcessEvent (1, EVS_TYPE.EVS_A_SEL_CHANGE, pvaArgs);
						}
					}
				}
			}));
		}

		
        // 슬라이드이미지 애니메이션 효과
        public void SlideImageAnimation (int nSlideIndex)
		{
			if (null == m_SiideImageAnimation)
			{
				m_SiideImageAnimation = SiideImageAnimationFactory.CreateSiideImageAnimation (m_nSlideAnimationType);
				if (null != m_SiideImageAnimation)
				{
					m_SiideImageAnimation.EBookSlideImage = this;
					m_SiideImageAnimation.CreateAnimationControl ();
				}
			}

			if (null != m_SiideImageAnimation)
			{
				int nCurrentSlideIndex = ImageUnit2.ImageIndex;

				// 20250203 KH 
				// m_SiideImageAnimation.TabInclease = (nCurrentSlideIndex < nSlideIndex);

				Debug.WriteLine($"SlideCount -> {SlideCount}");

                if (m_bAutoPlay == true && nCurrentSlideIndex == totalCount)
				{
					m_SiideImageAnimation.TabInclease = true;
                }
				else
				{
                    m_SiideImageAnimation.TabInclease = ((nCurrentSlideIndex - nSlideIndex) <= 0);
                }
                
				m_SiideImageAnimation.FirstPage = ImageUnit2;
				m_SiideImageAnimation.SecondPage = ImageUnit1;
			}


            if (null != m_SiideImageAnimation)
			{
				m_SiideImageAnimation.BeginAnimation (nSlideIndex);
			}
		}

		public void EndTextEdit ()
		{
			ImageUnit1.EndTextEdit ();
		}

		public void PlayFormLoadedGif ()
		{
			ImageUnit1.PlayFormLoadedGif ();
			ImageUnit2.PlayFormLoadedGif ();
		}

		public void StopFormLoadedGif ()
		{
			ImageUnit1.StopFormLoadedGif ();
			ImageUnit2.StopFormLoadedGif ();
		}

		public void ToRunMode (Atom pAtom)
		{
			m_pOwnerAtom = pAtom;

			IsDeepEditMode = false;
			IsRunMode = true;

			ImageUnit1.ToRunMode (pAtom);
			ImageUnit2.ToRunMode (pAtom);
			TitleTextUnit.ToRunMode ();

			//
			bool bPlay = false;

			Information info = pAtom.Information;

			if (null != info && false == info.IsEBookModel ())
			{
				bPlay = true;
			}

			EBookSlideImageAttrib slideAttrib = pAtom.GetAttrib () as EBookSlideImageAttrib;
			AutoPlaySlide (slideAttrib, bPlay);

			if (4 == slideAttrib.ButtonType)    // 화살표
			{
				SetLeftRightButtonStatus ();
			}
			else if (5 == slideAttrib.ButtonType) // 표시안함
			{
				ButtonContainer.Visibility = System.Windows.Visibility.Collapsed;
			}
			//

			this.m_nGifPlayType = slideAttrib.GifPlayType;
			this.m_nGifPlayCount = slideAttrib.GifPlayCount;

			this.m_nSlideAnimationType = slideAttrib.SlideAnimationType;

			if (null != m_SiideImageAnimation)
			{
				m_SiideImageAnimation.DropAnimationControl ();
				m_SiideImageAnimation = null;
			}

			// (0:페이드인아웃, 1:좌우슬라이딩, 2:입체박스효과, 3:커버플로우, 4:수평회전목마, 5:수직회전목마)
			switch (m_nSlideAnimationType)
			{
				case 0:
				case 1:
				case 2:
					break;
				case 3:
				case 4:
				case 5:
					SlideImageAnimation (1);

					if (null != m_pOwnerAtom && null != m_pOwnerAtom.GetOfAtom ())
						m_pOwnerAtom.GetOfAtom ().IsEnabledScroll = true;
					break;
			}
		}

		public void ToEditMode ()
		{
			IsRunMode = false;

			ImageUnit1.ToEditMode ();
			ImageUnit2.ToEditMode ();
			ImageUnit1.Visibility = System.Windows.Visibility.Visible;

			TitleTextUnit.ToEditMode ();

			this.m_nGifPlayType = -1;
			this.m_nGifPlayCount = 0;

			//
			AutoPlaySlideStop ();
			//

			if (null != m_SiideImageAnimation)
			{
				m_SiideImageAnimation.DropAnimationControl ();
				m_SiideImageAnimation = null;
			}
		}

		public void StartPlaySound ()
		{
			ImageUnit1.StartPlaySound ();
		}

		public void StopPlaySound ()
		{
			ImageUnit1.StopPlaySound ();
			ImageUnit2.StopPlaySound ();
		}

		public void StopAutoPlay ()
		{
			if (null != SlideTimer)
			{
				SlideTimer.Stop ();
				//SlideTimer.Tick -= AutoPlaySlideTimer_Tick;
			}

			ImageUnit1.StopPlaySound ();
			ImageUnit2.StopPlaySound ();

			ImageUnit1.StopFormLoadedGif ();
			ImageUnit2.StopFormLoadedGif ();
		}

		public void StartAutoPlay ()
		{
			if (null != SlideTimer)
			{
				ImageUnit1.ImageIndex = 1; //페이지 변경시 0번째 이미지부터 실행시키기 위해서
				
				ViewToSlide (1);
				SlideTimer.Start ();
			}
		}

		public void SetDocumentInfo (EBookSlideImageAttrib slideAttrib)
		{
			Dictionary<int, EBookImageInfo> imageInfoDic = slideAttrib.ImageInfoDic;

			if (null != slideAttrib.MainTitleDocument)
			{
				TitleTextUnit.Document = slideAttrib.MainTitleDocument;
				TitleTextUnit.TextVerticalAlignment = (VerticalAlignment)slideAttrib.MainTitleVertAlign;
				TitleRowDef.Height = new GridLength (slideAttrib.MainTitleHeight, GridUnitType.Pixel);
			}

			if (0 == imageInfoDic.Count)
			{
				return;
			}

			if (true == imageInfoDic.ContainsKey (1))
			{
				ImageUnit1.SetDocumentInfo (imageInfoDic[1]);
			}
		}

		public string GetTextForVoice ()
		{
			StringBuilder _sb = new StringBuilder ();
			_sb.AppendLine (TitleTextUnit.GetTextForVoice ());
			return _sb.ToString ();
		}

		public bool CheckMousePoint (Point ptMouse)
		{
			bool bReturn = false;

			return bReturn;
		}

		public void MultiMediaSetting ()
		{

		}

		public bool SetEditModeFristUnit ()
		{
			return false;
		}

		public bool IsRotateStarted
		{
			get
			{
				return ImageUnit1.IsRotateStarted;
			}
		}

		#region 글자 속성

		public void SetAtomFontSize (double dApplySize)
		{
			TitleTextUnit.SetAtomFontSize (dApplySize);

			ImageUnit1.SetAtomFontSize (dApplySize);
			ImageUnit2.SetAtomFontSize (dApplySize);

		}

		public void SetAtomFontColor (Brush applyBrush)
		{
			TitleTextUnit.SetAtomFontColor (applyBrush);

			ImageUnit1.SetAtomFontColor (applyBrush);
			ImageUnit2.SetAtomFontColor (applyBrush);
		}

		public void SetAtomFontFamily (FontFamily applyFontFamily)
		{
			TitleTextUnit.SetAtomFontFamily (applyFontFamily);

			ImageUnit1.SetAtomFontFamily (applyFontFamily);
			ImageUnit2.SetAtomFontFamily (applyFontFamily);
		}

		public void SetAtomFontWeight (FontWeight applyFontWeight)
		{
			TitleTextUnit.SetAtomFontWeight (applyFontWeight);

			ImageUnit1.SetAtomFontWeight (applyFontWeight);
			ImageUnit2.SetAtomFontWeight (applyFontWeight);
		}

		public void SetAtomFontStyle (FontStyle applyFontStyle)
		{
			TitleTextUnit.SetAtomFontStyle (applyFontStyle);

			ImageUnit1.SetAtomFontStyle (applyFontStyle);
			ImageUnit2.SetAtomFontStyle (applyFontStyle);
		}

		public void SetTextUnderLine (TextDecorationLocation underLine)
		{
			TitleTextUnit.SetTextUnderLine (underLine);

			ImageUnit1.SetTextUnderLine (underLine);
			ImageUnit2.SetTextUnderLine (underLine);
		}

		public void SetHorizontalTextAlignment (HorizontalAlignment applyHorizontalTextAlignment)
		{
			TitleTextUnit.SetHorizontalTextAlignment (applyHorizontalTextAlignment);

			ImageUnit1.SetHorizontalTextAlignment (applyHorizontalTextAlignment);
			ImageUnit2.SetHorizontalTextAlignment (applyHorizontalTextAlignment);
		}

		public void SetVerticalTextAlignment (VerticalAlignment applyVerticalTextAlignment)
		{
			TitleTextUnit.SetVerticalTextAlignment (applyVerticalTextAlignment);

			ImageUnit1.SetVerticalTextAlignment (applyVerticalTextAlignment);
			ImageUnit2.SetVerticalTextAlignment (applyVerticalTextAlignment);
		}

		public void SetAtomFontSizeUp ()
		{
			TitleTextUnit.SetAtomFontSizeUp ();

			ImageUnit1.SetAtomFontSizeUp ();
			ImageUnit2.SetAtomFontSizeUp ();
		}

		public void SetAtomFontSizeDown ()
		{
			TitleTextUnit.SetAtomFontSizeDown ();

			ImageUnit1.SetAtomFontSizeDown ();
			ImageUnit2.SetAtomFontSizeDown ();
		}

		#endregion

		private void AutoPlaySlideStop ()
		{
			if (null != SlideTimer)
			{
				SlideTimer.Stop ();
				SlideTimer.Tick -= AutoPlaySlideTimer_Tick;
			}
		}

		private void AutoPlaySlide (EBookSlideImageAttrib slideAttrib, bool bPlay)
		{
			if (false != slideAttrib.IsSlideAutoPlay && 0 < slideAttrib.SlidePlayCycle)
			{
				if (null == SlideTimer)
				{
					SlideTimer = new DispatcherTimer ();
				}

				SlideTimer.Stop ();
				SlideTimer.Interval = TimeSpan.FromSeconds (slideAttrib.SlidePlayCycle);
				SlideTimer.Tick -= AutoPlaySlideTimer_Tick;
				SlideTimer.Tick += AutoPlaySlideTimer_Tick;

				if (true == bPlay)
				{
					SlideTimer.Start ();
				}

				m_bAutoPlay = true;
			}
			else
			{
				if (null != SlideTimer)
				{
					SlideTimer.Stop ();
					SlideTimer.Tick -= AutoPlaySlideTimer_Tick;
				}

                // 20250203 KH 북모델 이미지표시란 아톰  - 좌우슬라이딩 효과 보완 - 마지막일때  회전될수 있도록 논리보완
                m_bAutoPlay = false;
            }
		}

		private void AutoPlaySlideTimer_Tick (object sender, EventArgs e)
		{
			int nCurrentSlideIndex = ImageUnit1.ImageIndex;

			SlideButtonMouseLeave (SelectedButton);

			if (nCurrentSlideIndex < ImageDic.Count)
			{
				nCurrentSlideIndex++;
				SelectedButton = ButtonContainer.Children[nCurrentSlideIndex - 1] as FrameworkElement;

				ViewToSlide (nCurrentSlideIndex);
			}
			else
			{
				SelectedButton = ButtonContainer.Children[0] as FrameworkElement;

				ViewToSlide (1);
			}
		}

		void LeftButton_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			int nCurrentSlideIndex = ImageUnit1.ImageIndex;

			if (1 < nCurrentSlideIndex)
			{
				SlideButtonMouseLeave (SelectedButton);

				nCurrentSlideIndex--;
				SelectedButton = ButtonContainer.Children[nCurrentSlideIndex - 1] as FrameworkElement;

				ViewToSlide (nCurrentSlideIndex);

				SetLeftRightButtonStatus ();

				//
				ImageUnit1.StartPlaySound ();
				//
			}
		}

		void RightButton_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			int nCurrentSlideIndex = ImageUnit1.ImageIndex;

			if (nCurrentSlideIndex < ImageDic.Count)
			{
				SlideButtonMouseLeave (SelectedButton);

				nCurrentSlideIndex++;
				SelectedButton = ButtonContainer.Children[nCurrentSlideIndex - 1] as FrameworkElement;

				ViewToSlide (nCurrentSlideIndex);

				SetLeftRightButtonStatus ();

				//
				ImageUnit1.StartPlaySound ();
				//
			}
		}

		public void SetLeftRightButtonStatus ()
		{
			int nCurrentSlideIndex = ImageUnit1.ImageIndex;

			if (1 < nCurrentSlideIndex && nCurrentSlideIndex < ImageDic.Count)
			{
				this.LeftButton.Visibility = System.Windows.Visibility.Visible;
				this.RightButton.Visibility = System.Windows.Visibility.Visible;
			}
			else if (1 == nCurrentSlideIndex)
			{
				this.LeftButton.Visibility = System.Windows.Visibility.Hidden;
				this.RightButton.Visibility = System.Windows.Visibility.Visible;
			}
			else if (ImageDic.Count == nCurrentSlideIndex)
			{
				this.LeftButton.Visibility = System.Windows.Visibility.Visible;
				this.RightButton.Visibility = System.Windows.Visibility.Hidden;
			}
		}
	}
}
