
#region |  ##### Using #####  |

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.CommonLib;
using Softpower.SmartMaker.TopApp.CommonLib.Interface;
using Softpower.SmartMaker.TopApp.CommonLib.StringGenerator;
using Softpower.SmartMaker.TopApp.TypeConverter;
using Softpower.SmartMaker.TopAtom.Commands;
using Softpower.SmartMaker.TopAtom.Components.GridTableAtom.Components;
using Softpower.SmartMaker.TopAtom.Components.GridTableEx;
using Softpower.SmartMaker.TopAtom.Data;
using Softpower.SmartMaker.TopAtom.Ebook;
using Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView;
using Softpower.SmartMaker.TopControl.Components.Adorners;


#endregion

namespace Softpower.SmartMaker.TopAtom
{
	public class AtomBase : UserControl
	{
		#region |  ##### 상수 #####  |

		private const double LOCK_OPACITY = 0.7;
		private const double NORMAL_OPACITY = 1;

		#endregion//상수

		#region |  ##### Protected 전역변수 #####  |

		protected PopupofAtom m_PopupAtom;

		protected Brush m_BindedPopupBrush = WPFColorConverter.ConvertHexToWPFBrush ("#4C006CAA");
		protected Brush m_BindedScrollBrush = WPFColorConverter.ConvertHexToWPFBrush ("#4CFFBC5C");

		protected Atom m_AtomCore;
		protected AdornerBase m_ResizeAdorner;

		protected bool m_bPopupAutoClose;   // 스마트콘텐츠 아톰 확장기능보강 (이수기능 : 팝업닫기)

		protected bool m_IsTextEditMode = false; //2019-12-12 kys Edit모드의 키보드를 정상적으로 전달하기 위해서 추가함
												 //2020-06-29 kys 아톰 내부 스크롤 사용 여부 - 웹모들 편집모드 UI 개선사항
												 //true일경우 해당 아톰 내부에 스크롤이 생성되어 있는 경우로 스크롤 이벤트시 아톰 내부에서만 스크롤 이벤트가 동작한다.
												 //false일경우 해당 아톰 내부에 스크롤이 없는 경우로 스크롤 이벤트시 전역 스크롤 이벤트가 동작한다. - MainWorkcanvasManager.cs OnNotifyScrollChanged 동작
		protected bool m_IsEnabledScroll = false;

		//2020-08-05 kys 웹모델에서 액티브 뷰 영역에 포함된경우 Ture로 체크되며 Ture일때 아톰이 화면밖으로 변경될경우 StopContent가 동작한다.
		protected bool m_IsActiveAction = false;

		//2020-09-15 kys 액션관리자에서 정지 또는 실행할 수 있는지를 설정하는 옵션이다.
		protected bool m_CanManagedActiveView = true;

		//2021-12-01 kys 스크롤 행으로 생성된 아톰인경우 true 
		//1. 콤보 데이터 가져올때 스크롤 행별로 가져오는것이 아닌 최초 1번만 가져올 수 있도록 하기 위해서 사용
		protected bool m_IsScrollCellItem = false;

		#endregion//Protected 전역변수

		#region |  ##### 델리게이트 이벤트 핸들러 선언 #####  |

		/// <summary>
		/// 아톰이 클릭되었음을 알리는 델리게이트 :
		/// TopBuild \ TopProcess \ Component \ ViewModels \ DMTDoc : sourceAtom.AtomClickedEvent += HandleAtomClickedEvent;
		/// </summary>
		public event CommonDelegateEvents.OnNotifyObjectEventHandler AtomClickedEvent = null;

		/// <summary>
		/// 한개의 ArrayList를 넘겨주는 델리게이트 :
		/// </summary>
		public event CommonDelegateEvents.OnNotifyArrayListEventHandler ShowAtomContextMenuEvent = null;

		/// <summary>
		/// 한개의 object값을 전달하는 델리게이트 :
		/// </summary>
		public event CommonDelegateEvents.OnNotifyObjectEventHandler OnNotifyCommandEvent = null;

		/// <summary>
		/// 한개의 object값을 전달하는 델리게이트 :
		/// </summary>
		public event CommonDelegateEvents.OnNotifyObjectEventHandler AtomDoubleClickedEvent = null;

		/// <summary>
		/// 두개의 object값을 전달하는 델리게이트 :
		/// </summary>
		public event CommonDelegateEvents.OnNotifyTwoObjectEventHandler OnNotifyChangeAttribEvent = null;

		/// <summary>
		/// 한개의 bool값을 전달하는 델리게이트 : 2014-10-31-M01 에디터 모드에서 이동 처리
		/// </summary>
		public event CommonDelegateEvents.OnNotifyMethodExecutedEventHandler OnNotifyMoveEvent = null;

		/// <summary>
		/// 단순히 메소드가 실행되었음을 알리는 델릴게이트(파라미터와 리턴값이 없다) : 2014-11-03-M02 텍스트편집기 에디터 모드 변경
		/// </summary>
		public event CommonDelegateEvents.OnNotifyMethodExecutedEventHandler OnNotifyMoveCompletedEvent = null;

		//아톰 크기 변경중임을 알리는 이벤트
		public event CommonDelegateEvents.OnNotifyThreeObjectEventHandler OnNotifyReSizeEvent = null;

		/// <summary>
		/// 아톰실행시, 실행완료 이벤트 (ex: 미디어재생완료, TTS 완료, 번역완료... 등등)
		/// </summary>
		public event CommonDelegateEvents.OnNotifyObjectEventHandler OnAtomExecuteComplete = null;

		#endregion//델리게이트 이벤트 핸들러 선언


		#region |  ##### 속성 #####  |

		public AdornerBase ResizeAdorner
		{
			get
			{
				return m_ResizeAdorner;
			}
			set
			{
				m_ResizeAdorner = value;
			}
		}

		public Atom AtomCore
		{
			get
			{
				return m_AtomCore;
			}
			set
			{
				m_AtomCore = value;
			}
		}

		//키보드를 통한 속성값 표시 모드 (아톰명 표시, 필드명 표시 등등)
		public bool IsZIndexTextVisible { get; set; } = false;
		public bool IsRelativeTabIndexTextVisible { get; set; } = false;
		public bool IsAbsoluteTabIndexTextVisible { get; set; } = false;
		public bool IsDebugDBInfoVisible { get; set; } = false;
		public bool IsAtomNameTextVisible { get; set; } = false;
		public bool IsAtomFieldTextVisible { get; set; } = false;

		//Undo / Redo를 위한 속성값들
		public FrameworkElement PreOwner { get; set; } = null;
		public Brush PreBackground { get; set; } = Brushes.White;
		public Thickness PreBorderThicknes { get; set; } = new Thickness (1);
		public Thickness PreMargin { get; set; } = new Thickness (0);

		public System.Drawing.Color NinePatchBrightnessColor { get; set; } = System.Drawing.Color.Transparent;

		//편집시 필요한 속성
		public bool IsAtomResizing { get; set; } = false;
		public double HitTestWidth { get; set; } = 0;
		public double HitTestHeight { get; set; } = 0;

		public double MoveStartLeft { get; set; } = 0;
		public double MoveStartTop { get; set; } = 0;


		//2021-12-21 kys 연계효과에서 사용하기 위해서 만듬 실행모드에서 AtomX, AtomY등의 값이 변경되기 때문에 변경되지 않는값을 지정
		public double EditAtomHeight { get; set; } = 0;
		public double EditAtomWidth { get; set; } = 0;
		public double EditAtomX { get; set; } = 0;
		public double EditAtomY { get; set; } = 0;

		// 스마트콘텐츠 아톰 확장기능보강 (이수기능 : 팝업닫기)
		public bool PopupAutoClose
		{
			get { return m_bPopupAutoClose; }
			set { m_bPopupAutoClose = value; }
		}

		public virtual bool TextEditMode
		{
			get { return m_IsTextEditMode; }
			set
			{
				if (m_IsTextEditMode != value)
				{
					m_IsTextEditMode = value;

					SetFormChange ();
					ChangeAtomTextEditMode ();
				}
			}
		}

		public virtual bool IsEnabledScroll
		{
			get { return m_IsEnabledScroll; }
			set { m_IsEnabledScroll = value; }
		}

		public bool IsWebDoc
		{
			get { return this.AtomCore.Information.IsWebdoc (); }
		}

		public bool IsActiveAction
		{
			get { return m_IsActiveAction; }
			set { m_IsActiveAction = value; }
		}

		public bool CanManagedActiveView
		{
			get { return m_CanManagedActiveView; }
			set { m_CanManagedActiveView = value; }
		}

		public bool IsScrollCellItem
		{
			get { return m_IsScrollCellItem; }
			set { m_IsScrollCellItem = value; }
		}

		#endregion

		#region |  ##### 생성자 #####  |


		public AtomBase () : base ()
		{
			InitializeAtomBase ();
		}

		public AtomBase (Atom atomCore)
			: base ()
		{
			InitializeAtomBase (atomCore);
		}

		private void InitializeAtomBase (Atom atomCore = null)
		{
			//AtomBase에서만 쓰는 변수 셋팅
			InitializePrivateVariable ();

            /*
			// 공통속성 정의
			InitializeCommonProperties ();
			Debug.WriteLine($"Softpower.SmartMaker.TopAtom.AtomBase.atomCore.AtomType Line #262 is {(atomCore == null ? "null" : atomCore.AtomType.ToString())}");
			*/

            if (null == atomCore)
			{
                // 공통속성 정의
                // 공통속성의 경우 새로운 Class를 생성하는 경우에만 실행하도록 수정 20251. 24. KH
                InitializeCommonProperties();

                //AtomCore 셋팅
                InitializeAtomCore ();
			}
			else
			{
				this.AtomCore = atomCore;
			}

			this.AtomCore.AtomType = AtomCore.GetUniqueEnumType ();

			if (this.AtomCore.AtomType == AtomType.None)
			{
				if (!(this is EBookQuizElementContentofAtom)) //2024-10-25 kys 퀴즈블록 OX퀴즈 풀이시 사용되는 아톰 예외처리함
					Trace.TraceError ("this.AtomCore.AtomType None : " + this.AtomCore.GetType ());
			}

			//InitializeAtomBinding 셋팅
			InitializeAtomBinding ();

			//AnimationHelper 셋팅
			InitializeAnimationHelper ();

			//View - Model Binding
			InitializePropertyBinding ();

			//아톰의 DefaultProperVar 정의
			InitializeAtomDefaultProperVar ();

			//Adorner 셋팅
			InitializeResizeAdorner ();

			//아톰의 사이즈 셋팅
			if (Kiss.DoubleEqual (0, AtomCore.Attrib.AtomWidth + AtomCore.Attrib.AtomHeight))
				InitializeAtomSize ();

			//AtomBase 이벤트
			InitializeEvent ();

			//사용자 정의 Delegate 이벤트 초기화
			InitializeDelegateEvents ();
		}

		#endregion//생성자

		#region |  ##### Private 메서드 #####  |

		private void InitializePrivateVariable ()
		{
			this.Cursor = Cursors.SizeAll;

			m_BindedScrollBrush = WPFColorConverter.ConvertHexToWPFBrush ("#4CFFBC5C");
		}

		private void InitializeEvent ()
		{
			this.Initialized += AtomBase_Initialized;
			this.MouseLeftButtonDown += AtomBase_MouseLeftButtonDown;
			this.DragOver += AtomBase_DragOver;
			this.Drop += AtomBase_Drop;
			this.MouseDoubleClick += AtomBase_MouseDoubleClick;
			this.MouseDown += AtomBase_MouseDown;
		}

		#endregion//Private 메서드 

		#region |  ##### Public Virtual 메서드 #####  |

		//2020-11-17 kys 편집모드에서 Html태그 삽입 아톰의 크롬 로드 시점을 위해서 추가함 WebMotionManager.cs에서 호출
		public virtual void ChangeAtomVisibility_WebDoc (bool bVisible)
		{

		}

		//현재 아톰이 액티뷰 영역에 표시되어 컨텐츠 실행
		public virtual void StartContent_WebMotionManager ()
		{

		}

		//현재 아톰이 액티뷰 영역 밖으로 이동하여 컨텐츠 종료
		public virtual void StopContent_WebMotionManager ()
		{

		}

		/// <summary>
		/// 스크롤이나 탭뷰는 자식이 있기대문에 bIsRoutedChildren옵션을 통해 자식들도 메소드를 적용할지 결정한다.
		/// override 할때 base를 꼭 호출하도록 한다.
		/// </summary>
		/// <param name="isVisible"></param>
		/// <param name="bIsRoutedChildren"></param>
		public virtual void SetResizeAdornerVisibility (Visibility isVisible, bool bIsRoutedChildren)
		{
			if (null != m_ResizeAdorner)
			{
				if (null == m_ResizeAdorner.Parent)
				{
					AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer (this);

					if (null != adornerLayer)
					{
						adornerLayer.Add (m_ResizeAdorner);
					}
				}

				if (m_ResizeAdorner.Visibility != isVisible && Visibility.Visible == isVisible)
				{
					MoveStartLeft = this.Margin.Left;
					MoveStartTop = this.Margin.Top;
				}


				m_ResizeAdorner.Visibility = isVisible;


				if (null != AtomCore)
				{
					AtomCore.NotifyResizeAdornerVisibilityToPathUnit (isVisible == System.Windows.Visibility.Visible);
				}

				if (!TextEditMode)
				{
					switch (isVisible)
					{
						case Visibility.Visible:
							this.Focusable = true;
							this.Focus ();
							break;
						case Visibility.Collapsed:
							this.Focusable = false;
							UnTryInputFocus ();
							break;
					}
				}
			}
		}

		public virtual Visibility GetResizeAdornerVisibility ()
		{
			if (null != m_ResizeAdorner)
			{
				return m_ResizeAdorner.Visibility;
			}

			return Visibility.Collapsed;
		}

		public virtual FontStyle GetAtomFontStyle ()
		{
			return FontStyle;
		}

		public virtual void SetAtomFontStyle (FontStyle applyFontStyle)
		{
			FontStyle = applyFontStyle;
		}

		public virtual FontWeight GetAtomFontWeight ()
		{
			return FontWeight;
		}

		public virtual void SetAtomFontWeight (FontWeight applyFontWeight)
		{
			FontWeight = applyFontWeight;
		}

		public virtual Brush GetAtomBorder ()
		{
			return null;
		}

		public virtual void SetAtomBorder (Brush applyBrush)
		{
		}

		public virtual Brush GetAtomFontColor ()
		{
			return Foreground;
		}

		public virtual void SetAtomFontColor (Brush applyBrush)
		{
			Foreground = applyBrush;
		}

		public virtual double GetAtomFontSize ()
		{
			return FontSize;
		}

		public virtual void SetAtomFontSize (double dApplySize)
		{
			FontSize = dApplySize;
		}

		public virtual void SetAtomFontSizeUp ()
		{
		}

		public virtual void SetAtomFontSizeDown ()
		{
		}

		public virtual FontFamily GetAtomFontFamily ()
		{
			return FontFamily;
		}

		public virtual void SetAtomFontFamily (FontFamily applyFontFamily)
		{
			FontFamily = applyFontFamily;
		}

		public virtual Brush GetAtomBackground ()
		{
			return Background;
		}

		public virtual void SetAtomIndent (double indent)
		{

		}

		public virtual void SetAtomOutdent (double outdent)
		{

		}

		public virtual double GetAtomIndent ()
		{
			return 0;
		}

		public virtual double GetAtomOutdent ()
		{
			return 0;
		}

		public virtual void SetAtomBackground (Brush applyBrush)
		{
			Background = applyBrush;
		}

		public virtual Thickness GetAtomThickness ()
		{
			return new Thickness (0);
		}

		public virtual void SetAtomThickness (Thickness applyThickness)
		{
		}

		public virtual int GetAtomOpacity ()
		{
			if (false != this.AtomCore.Attrib.IsLocked)
			{
				Attrib atomAttrib = AtomCore.GetAttrib ();
				return atomAttrib.AtomOpacity;
			}
			else
			{
				int nOpacity = (int)(this.Opacity * 100);
				return nOpacity;
			}
		}

		public virtual void SetAtomOpacity (int nOpacity)
		{
			if (null != AtomCore)
			{
				Attrib attrib = AtomCore.GetAttrib ();
				if (null != attrib)
				{
					this.Opacity = (double)(nOpacity / 100.0);

					attrib.AtomOpacity = nOpacity;
				}
			}
		}

		public virtual DoubleCollection GetAtomDashArray ()
		{
			return null;
		}

		public virtual void SetAtomDashArray (DoubleCollection applyDashArray)
		{
		}

		public virtual HorizontalAlignment GetHorizontalTextAlignment ()
		{
			return HorizontalAlignment.Left;
		}

		public virtual void SetHorizontalTextAlignment (HorizontalAlignment applyHorizontalTextAlignment)
		{
		}

		public virtual VerticalAlignment GetVerticalTextAlignment ()
		{
			return VerticalAlignment.Center;
		}

		public virtual void SetVerticalTextAlignment (VerticalAlignment applyVerticalTextAlignment)
		{
		}

		public virtual TextDecorationLocation GetTextUnderLine ()
		{
			return TextDecorationLocation.Baseline;
		}

		public virtual void StrikeOutChanged ()
		{
		}

		public virtual void SetAtomFieldType (string strAtomFieldType)
		{
		}

		/// <summary>
		/// 아톰이 최종적으로 삭제되기 전에 부르는 메소드
		/// 아톰이 삭제되기 직전에 꼭 해제시켜야 할 것들을 처리하는 곳
		/// </summary>
		/// <returns></returns>
		public virtual bool DisposeAll ()
		{
			if (null != m_AtomCore)
			{
				if (null != AtomCore.HyperLinkAtom)
				{
					_Message80.Show (LC.GS ("TopAtom_AtomBase_1"));
					return false;
				}

				if (true == this.AtomCore.IsBindedPopup)
				{
					_Message80.Show (LC.GS ("TopAtom_AtomBase_2"));
					return false;
				}
				else if (true == this.AtomCore.IsBindedScroll)
				{
					_Message80.Show (LC.GS ("TopAtom_AtomBase_3"));
					return false;
				}

				if (null != AtomCore.BindBlockAtom)
				{
					_Message80.Show (LC.GS ("TopAtom_AtomBase_2"));
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// F3 눌렀을 경우 아톰명 표시
		/// </summary>
		public virtual void TryAtomNameTextVisible ()
		{
		}

		/// <summary>
		/// F2 눌렀을 경우 입력포커스 주는함수
		/// </summary>
		public virtual void TryInputFocus ()
		{
		}

		/// <summary>
		/// 80 F2 눌렀을 경우 입력포커스 주는함수 이후 Tab을 눌렀을때 입력포커스 아웃
		/// </summary>
		public virtual void UnTryInputFocus ()
		{
		}

		public virtual void BindValueConverter ()
		{
		}

		public virtual void InvalidateAll ()
		{
			InvalidateVisual ();
		}

		public virtual void CompletePropertyChanged ()
		{
		}

		/// <summary>
		/// 2020-04-28 kys 아톰 크기변환후 NinePach이미지를 적용시키기 위해서
		/// </summary>
		public virtual void CompleteAtomSizeChanged ()
		{
		}

		/// <summary>
		/// 복사완료후 GDI객체, Adorner등을 설정해주기 위해 추가함
		/// </summary>
		public virtual void CompleteAtomCopye ()
		{
			this.SetResizeAdornerVisibility (System.Windows.Visibility.Visible, true);
		}

		public virtual void NotifyChangedValueByInnerLogic (object changedValue)
		{
		}

		/// <summary>
		/// 이 아톰이 스크롤에 묶여 있으면 스크롤아톰을 리턴한다
		/// </summary>
		/// <returns></returns>
		public virtual FrameworkElement GetScrollAtom ()
		{
			FrameworkElement bindedAtom = this;

			//2024-07-08 kys 모델삽입을 스크롤로 묶고, 모델삽입 안에 있는 아톰에 접근을 시도하면
			//GetScrollAtom이 null이 아닌 모델삽입을 묶은 스크롤이 리턴되는 현상있어 보강

			// 2024.08.02 beh 여기서 ScrollIndex로 체크할 경우 ScrollIndex가 디시리얼라이즈되기 전에는 ScrollAtom을 찾아오지 못 하는 문제 있어 주석처리

			//if (int.MaxValue == this.AtomCore.GetScrollIndex ())
			//	return null;

			while (true)
			{
				FrameworkElement parentElement = bindedAtom.Parent as FrameworkElement;

				if (null != parentElement)
				{
					if (parentElement is EBookPageAtomBase)
						break;

					if (parentElement is ScrollAtomBase)
					{
						return parentElement;
					}
					else
					{
						bindedAtom = parentElement;
					}
				}
				else
				{
					return null;
				}
			}
			return null;
		}

		public virtual PopupofAtom GetPopupAtom ()
		{
			return m_PopupAtom;
		}

		public virtual void SetPopupAtom (PopupofAtom SourcePopupAtom)
		{
			m_PopupAtom = SourcePopupAtom;
		}

		/// <summary>
		/// 이 아톰을 소유하고있는 View를 가져온다.(OwnerView의 종류에는 0 : View   1: TabView   2 : Scroll이 있다.)
		/// </summary>
		/// <returns></returns>
		public virtual FrameworkElement GetOwnerView ()
		{
			FrameworkElement bindedAtom = this;

			while (true)
			{
				FrameworkElement parentElement = bindedAtom.Parent as FrameworkElement;

				if (null != parentElement)
				{
					if (parentElement is TopView)
					{
						return parentElement;
					}
					else if (parentElement is TabViewAtomBase)
					{
						return parentElement;
					}
					else if (parentElement is ScrollAtomBase)
					{
						return parentElement;
					}
					else if (parentElement is TableViewBase)
					{
						return parentElement;
					}
					else if (parentElement is AtomBase)
					{
						return parentElement;
					}
					else
					{
						bindedAtom = parentElement;
					}
				}
				else
				{
					return null;
				}
			}
		}

		public virtual void NotifyCommandToCommander (ICommandBehavior SourceCommand)
		{
			if (SourceCommand is ChangeAtomSizeCommand)
			{
				CompleteAtomSizeChanged ();
			}

			if (null != OnNotifyCommandEvent)
			{
				OnNotifyCommandEvent (SourceCommand);
			}
		}

		public virtual void NotifyMoveEvent ()
		{
			if (null != OnNotifyMoveEvent)
			{
				OnNotifyMoveEvent ();
			}
		}

		/// <summary>
		/// 2014-11-03-M02 텍스트편집기 에디터 모드 변경
		/// </summary>
		public virtual void NotifyMoveCompletedEvent ()
		{
			if (null != OnNotifyMoveCompletedEvent)
			{
				OnNotifyMoveCompletedEvent ();
			}

		}


		public virtual void NotifyReSizeEvent (Thickness preSize, int ResizeDirection = -1)
		{
			if (null != OnNotifyReSizeEvent)
			{
				OnNotifyReSizeEvent (this, preSize, ResizeDirection);
			}
		}

		/// <summary>
		/// 아톰실행시, 실행완료 이벤트 (ex: 미디어재생완료, TTS 완료, 번역완료... 등등)
		/// </summary>
		/// <param name="bValue"></param>
		public virtual void AtomExecuteComplete ()
		{
			if (null != OnAtomExecuteComplete)
			{
				OnAtomExecuteComplete (this);
			}
		}

		/// <summary>
		/// 폰트, 글자크기, Bold, Italic, UnderLine, 글자색상
		/// Border 선색, 선굵기, 선유형
		/// 채우기 색
		/// </summary>
		public virtual void SerializeLoadSync_AttribToAtom (bool bIs80Model)
		{
			if (null == this.AtomCore || null == AtomCore.GetAttrib ())
			{
				return;
			}

			Attrib atomAttrib = this.AtomCore.GetAttrib () as Attrib;

			this.Sync_FontAttribToAtom (atomAttrib, bIs80Model);
			this.Sync_PenAttribToAtom (atomAttrib, bIs80Model);
			this.Sync_BrushAttribToAtom (atomAttrib, bIs80Model);
			this.Sync_OpacityAttribToAtom (atomAttrib, bIs80Model);
		}

		/// <summary>
		/// Attrib을 통해 ofAtom View 관련 동기화
		/// </summary>
		public virtual void Sync_AttribToAtom ()
		{
			if (null == this.AtomCore || null == AtomCore.GetAttrib ())
			{
				return;
			}

			Attrib atomAttrib = this.AtomCore.GetAttrib () as Attrib;
			this.Sync_FontAttribToAtom (atomAttrib, true);
			this.Sync_PenAttribToAtom (atomAttrib, true);
			this.Sync_BrushAttribToAtom (atomAttrib, true);
		}

		/// <summary>
		/// 바이딩을 통해 동기화 할수 없는 속성들에 대해 
		/// 아톰이 생성될시 Attrib에 Atom의 속성값을 동기화해줌 
		/// 예 : 아톰 배경색 
		/// Attrib(예시) - 상속 구조에 따라 배경색 member valiable의 가진 class가 다르며 
		/// Atom(예시) - 여러 Component로 구성된 검색창 같은 아톰은 특정 Component만 생상을 변경해야함 
		/// 상기 예시를 통해 바인딩으로 할경우 상당히 복잡한 경우의 수가 발생 
		/// 
		/// virtual 메서드를 통해 아톰 생성시 동기화가 필요한 아톰은 동기화 시킴 
		/// 
		/// 유의 : 속성값의 초기값이 동기화 되었다고 착각되어지는 경우가 있음 - Atttib 와 Atom의 초기값이 같을때 발생 
		/// 따라서 만약 Atom의 초기값 변경시(Xaml) 저장 불러오기할때엔 Attib의 값을 사용하므로 달라보이는경우가 생김
		/// </summary>
		public virtual void OnFirstMakeSync (bool bFalse)
		{
			Information info = AtomCore.Information as Information;
			double dViewScale = info.GetViewPercentOfScale ();

			//if (100 > dViewScale)
			//{
			//    if (bFalse)
			//        return;

			//    double dFirstFontSize = 12d * 100d / dViewScale;
			//    SetAtomFontSize(dFirstFontSize);
			//    AtomCore.GetAttrib().SetAtomFontSize(dFirstFontSize);
			//}

			//2014-11-20-M01 Default Font 처리
			double dFirstFontSize = PQAppBase.DefaultFontSize;
			SetAtomFontSize (dFirstFontSize);
			AtomCore.GetAttrib ().SetAtomFontSize (dFirstFontSize);
        }

		/// <summary>
		/// 참조관계상 웹아톰 개별 로직은  override 시킴 
		/// </summary>
		/// <param name="atomAttrib"></param>
		/// <param name="bIs80Model"></param>
		public virtual void Sync_BrushAttribToAtom (Attrib atomAttrib, bool bIs80Model)
		{
			Brush atomBrush = null;

			int nBrushKey = atomAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._BRUSH);
			CObjectBrush pObjectBrush = null;
			atomAttrib.GetGDIObjFromKey (ref pObjectBrush, nBrushKey);

			if (true == bIs80Model)
			{
				if (atomAttrib is PopupAttrib /*|| atomAttrib is CScrollAttrib*/) //2019-11-27 kys 스크롤 배경색 지정되도록 변경함
				{
					return;
				}

				if (atomAttrib is LookAttrib)
				{
					if (null == pObjectBrush)
					{
						return;
					}

					if (false == ((LookAttrib)atomAttrib).BrushFill)
					{
						atomBrush = new SolidColorBrush (Colors.Transparent);
					}
					else
					{
						atomBrush = pObjectBrush.WpfColor;
					}
				}
				else if (atomAttrib is LandAttrib)
				{
					LandAttrib landAttrib = atomAttrib as LandAttrib;
					atomBrush = new SolidColorBrush (landAttrib.BackColor);
				}
			}
			else
			{
				if (atomAttrib is PopupAttrib || atomAttrib is ScrollAttrib)
				{
					return;
				}

				else if (atomAttrib is LandAttrib)
				{
					LandAttrib landAttrib = atomAttrib as LandAttrib;
					atomBrush = new SolidColorBrush (landAttrib.BackColor);
				}
				else if (atomAttrib is LookAttrib)
				{
					if (null == pObjectBrush)
						return;

					if (false == ((LookAttrib)atomAttrib).BrushFill)
					{
						atomBrush = new SolidColorBrush (Colors.Transparent);
					}
					else
					{
						atomBrush = pObjectBrush.WpfColor;
					}
				}
				else if (atomAttrib is DecorAttrib)
				{
					if (atomAttrib is DecorImageAttrib)
					{
						atomBrush = new SolidColorBrush (Colors.White);
					}
				}
				else
				{
					if (null == pObjectBrush)
						return;

					atomBrush = pObjectBrush.WpfColor;
				}
			}

			if (null != atomBrush)
			{
				//if (Colors.Transparent == ((SolidColorBrush)atomBrush).Color)
				//{
				//	this.SetAtomBackground(null);
				//}
				//else
				{
					this.SetAtomBackground (atomBrush);
				}
			}
		}

		public void Sync_PenAttribToAtom (Attrib atomAttrib, bool bIs80Model)
		{
			// 2014-12-02 JAEYOUNG : Generator 모드에서는 리턴. 
			// 특정 아톰에 Border 가 None 으로 바뀌는현상.
			if (PQAppBase.IsGeneratorMode == true)
				return;

			if (true == bIs80Model)
			{
				if (atomAttrib is LandAttrib)
				{
					if (System.Windows.Forms.BorderStyle.None == ((LandAttrib)atomAttrib).BorderStyle)
					{
						this.SetAtomThickness (new Thickness (0));
						PreBorderThicknes = new Thickness (1);
					}
					else
					{
						this.SetAtomThickness (new Thickness (1));
						PreBorderThicknes = new Thickness (0);
					}

					// 테두리색 지원
					LandAttrib landAttrib = atomAttrib as LandAttrib;
					this.SetAtomBorder (new SolidColorBrush (landAttrib.BorderColor));
					//
				}
				else if (atomAttrib is DecorAttrib)
				{
					int nPenKey = atomAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._PEN);
					CObjectPen pObjectPen = null;
					atomAttrib.GetGDIObjFromKey (ref pObjectPen, nPenKey);

					if (true == ((DecorAttrib)atomAttrib).PenFill)
					{
						PreBorderThicknes = new Thickness (pObjectPen.PenThick);
						this.SetAtomThickness (new Thickness (pObjectPen.PenThick));
						this.SetAtomBorder (pObjectPen.WpfColor);
						this.SetAtomDashArray (From70To80.Int_To_DashArrayStyle (pObjectPen.PenStyle));
					}
					else
					{
						PreBorderThicknes = new Thickness (pObjectPen.PenThick);
						this.SetAtomThickness (new Thickness (0));
					}
				}
			}
			else
			{

				bool bNoLine = false;
				//80 70모드에서 로드된 아톰은 그리는것이 아닌 속성 자체를 변화시켜줘야함 

				int nPenKey = atomAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._PEN);
				CObjectPen pObjectPen = null;
				atomAttrib.GetGDIObjFromKey (ref pObjectPen, nPenKey);

				if (atomAttrib is PopupAttrib ||
					atomAttrib is LabelNumAttrib ||
					atomAttrib is TabViewAttrib ||
					atomAttrib is CheckAttrib ||
					atomAttrib is RadioAttrib ||
					atomAttrib is SignalAttrib ||
					atomAttrib is DateAttrib ||
					atomAttrib is ChartAttrib ||
					atomAttrib is SearchConditionAttrib ||
					atomAttrib is ActionAttrib)
				{
					bNoLine = true;

					if (atomAttrib is DecorAttrib)
					{
						((DecorAttrib)atomAttrib).PenFill = false;
					}
					else if (atomAttrib is LandAttrib)
					{
						((LandAttrib)atomAttrib).BorderStyle = System.Windows.Forms.BorderStyle.None;
					}
				}
				else if (atomAttrib is DecorAttrib)
				{
					if (false == ((DecorAttrib)atomAttrib).PenFill)
					{
						bNoLine = true;
					}
					else
					{
						bNoLine = false;
					}
				}
				else if (atomAttrib is LandAttrib)
				{
					if (System.Windows.Forms.BorderStyle.None == ((LandAttrib)atomAttrib).BorderStyle)
					{
						bNoLine = true;
					}
					else
					{
						bNoLine = false;
					}
				}

				if (null != pObjectPen)
				{
					PreBorderThicknes = new Thickness (pObjectPen.PenThick);
					this.SetAtomThickness (new Thickness (bNoLine ? 0 : pObjectPen.PenThick));
					this.SetAtomBorder (pObjectPen.WpfColor);
					this.SetAtomDashArray (From70To80.Int_To_DashArrayStyle (pObjectPen.PenStyle));
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="atomAttrib"></param>
		/// <param name="bIs80Model"></param>
		public virtual void Sync_OpacityAttribToAtom (Attrib atomAttrib, bool bIs80Model)
		{
			if (false != this.AtomCore.Attrib.IsLocked)
			{
				this.Opacity = LOCK_OPACITY;
			}
			else
			{
				this.Opacity = (double)(atomAttrib.AtomOpacity / 100.0);
			}
		}

		public virtual void ChangeAtomMode (int nRunMode)
		{
			this.AtomCore.AtomRunMode = nRunMode;
			switch (nRunMode)
			{
				case -1:
					{
						VisualStateManager.GoToState (this, "EditMode", true);
						break;
					}

				case 0:
					{
						this.Cursor = Cursors.SizeAll;
						VisualStateManager.GoToState (this, "BaseMode", true);
						ReleaseRunModeProperty ();

						this.ToolTip = null;
						break;
					}

				case 1:
					{
						this.Cursor = Cursors.Arrow;

						//2014-12-01-M01 타이핑 모드 처리
						UnTryInputFocus ();
						VisualStateManager.GoToState (this, "RunMode", true);
						ApplyRunModeProperty ();
						SetResizeAdornerVisibility (Visibility.Collapsed, true);

						if (true == PQAppBase.IsEduTechMode && true == PQAppBase.IsAccessibility)
						{
							var accessibilityInfo = this.AtomCore.GetAttrib ().AccessibilityInfo;
							if (null != accessibilityInfo && true == accessibilityInfo.IsWebAccessibility)
							{
								//입력란 데이터입력란 입니다.
								//저장하기 기능버튼 입니다.
								this.ToolTip = $"{this.AtomCore.GetProperVar ()} {this.AtomCore.DefaultAtomTypeName} 입니다";
							}
						}
						break;
					}

				default: break;
			}
		}

		public virtual void NotifyCurrentLocationAndSize ()
		{
			Attrib atomAttrib = this.AtomCore.GetAttrib ();

			if (atomAttrib is RoundSquareAttrib)
			{
				GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyCurrentLocationAndSize (this.Margin, this.Width, this.Height, (atomAttrib as RoundSquareAttrib).CornerX);
			}
			else if (atomAttrib is LabelAttrib)
			{
				LabelAttrib labelAttrib = atomAttrib as LabelAttrib;
				if (0 < labelAttrib.BorderRadius)
				{
					GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyCurrentLocationAndSize (this.Margin, this.Width, this.Height, labelAttrib.BorderRadius);
				}
				else
				{
					GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyCurrentLocationAndSize (this.Margin, this.Width, this.Height);
				}
			}
			else if (atomAttrib is DecorImageAttrib)
			{
				DecorImageAttrib decorAttrib = atomAttrib as DecorImageAttrib;
				if (0 < decorAttrib.BorderRadius)
				{
					GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyCurrentLocationAndSize (this.Margin, this.Width, this.Height, decorAttrib.BorderRadius);
				}
				else
				{
					GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyCurrentLocationAndSize (this.Margin, this.Width, this.Height);
				}
			}
			else
			{
				GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyCurrentLocationAndSize (this.Margin, this.Width, this.Height);
			}
		}

		/// <summary>
		/// Attrib 백업 이전에 실행 
		/// </summary>
		public virtual void ApplyRunModeProperty ()
		{
			Atom atomCore = AtomCore as Atom;
			Attrib atomAttrib = atomCore.GetAttrib () as Attrib;

			if (atomCore is PopupAtom)
			{
				this.Visibility = Visibility.Collapsed;
			}
			else
			{
				this.Visibility = (true == atomAttrib.IsAtomHidden) ? Visibility.Collapsed : Visibility.Visible;
			}

			this.IsEnabled = !atomAttrib.IsDisabled;

			//잠김아톰 런모드일때는 투명도 원래대로
			this.Opacity = (double)(atomAttrib.AtomOpacity / 100.0);

			//스크롤 행이 아닌경우에만 원본값 유지 논리 스크롤의 행들은 원본 아톰에서 CloneAtom할때 값 가져온다.
			if (false == m_IsScrollCellItem)
			{
				EditAtomWidth = this.AtomCore.Attrib.AtomWidth;
				EditAtomHeight = this.AtomCore.Attrib.AtomHeight;
				EditAtomX = this.AtomCore.Attrib.AtomX;
				EditAtomY = this.AtomCore.Attrib.AtomY;
			}
		}

		/// <summary>
		/// Attrib 롤백 이전에 실행 
		/// </summary>
		public virtual void ReleaseRunModeProperty ()
		{
			this.Visibility = Visibility.Visible;
			this.IsEnabled = true;

			//잠김아톰 에딧모드일때  투명도 0.5
			if (false != this.AtomCore.Attrib.IsLocked)
			{
				this.Opacity = LOCK_OPACITY;
			}
			else
			{
				Attrib sourceAttrib = AtomCore.GetAttrib ();
				this.Opacity = (double)(sourceAttrib.AtomOpacity / 100.0);
			}
		}

		/// <summary>
		/// Attrib 백업 이후에 실행 
		/// </summary>
		public virtual void DoPostRunMode ()
		{
		}

		/// <summary>
		/// Attrib 롤백 이후에 실행 
		/// </summary>
		public virtual void DoPostEditMode ()
		{
		}

		public virtual bool CheckAtomComponentHasGotMouseHandle ()
		{
			return true;
		}

		/// <summary>
		/// 아톰 속성 복사
		/// Shallow Copy (일반적으로 Alt + 방향키로 복사하며, DB데이터는 제외한 얕은 복사)
		/// 은 오병권 연구원 계획이였는듯....
		/// bool 값으로 Shallow/Deep 카피 구분함
		/// </summary>
		/// <param name="ClonedAtom"></param>
		/// <param name="bDeepCopy">아톰 전체속성복사(true) / 스타일복사(false)</param>
		public virtual void CloneAtom (AtomBase ClonedAtom, bool bDeepCopy)
		{
			ClonedAtom.EditAtomWidth = this.EditAtomWidth;
			ClonedAtom.EditAtomHeight = this.EditAtomHeight;
			ClonedAtom.EditAtomX = this.EditAtomX;
			ClonedAtom.EditAtomY = this.EditAtomY;

			ClonedAtom.FontFamily = this.FontFamily;
			ClonedAtom.FontSize = this.FontSize;
			ClonedAtom.FontStyle = this.FontStyle;
			ClonedAtom.FontWeight = this.FontWeight;
			ClonedAtom.Width = this.Width;
			ClonedAtom.Height = this.Height;
			ClonedAtom.Margin = this.Margin;
			ClonedAtom.PreMargin = this.PreMargin;
			ClonedAtom.PreOwner = this.PreOwner;
			ClonedAtom.HorizontalAlignment = this.HorizontalAlignment;
			ClonedAtom.VerticalAlignment = this.VerticalAlignment;
			ClonedAtom.AtomCore.AtomType = this.AtomCore.AtomType;

			ClonedAtom.IsEnabledScroll = this.IsEnabledScroll;

			int nOpacity = this.GetAtomOpacity ();
			ClonedAtom.SetAtomOpacity (nOpacity);
			ClonedAtom.AtomCore.Attrib.IsShadow = this.AtomCore.Attrib.IsShadow;

			Attrib ClonedAttrib = ClonedAtom.AtomCore.GetAttrib ();
			Attrib SourceAttrib = AtomCore.GetAttrib ();
			CGDIMgr TargetGDIManager = ClonedAttrib.GetGDIManager ();

			CObjectBrush sourceBrush = null;
			CObjectPen sourcePen = null;
			CObjectFont sourceFont = null;
			CObjectImage sourceImage = null;
			CObjectImage sourceRolloverImage = null;

			int nSourceBrushKey = SourceAttrib.GetGDIKey ((int)OBJECTKEY_TYPE._BRUSH);
			int nSourcePenKey = SourceAttrib.GetGDIKey ((int)OBJECTKEY_TYPE._PEN);
			int nSourceFontKey = SourceAttrib.GetGDIKey ((int)OBJECTKEY_TYPE._FONT);

			SourceAttrib.GetGDIObjFromKey (ref sourceBrush, nSourceBrushKey);
			SourceAttrib.GetGDIObjFromKey (ref sourcePen, nSourcePenKey);
			SourceAttrib.GetGDIObjFromKey (ref sourceFont, nSourceFontKey);

			int nTargetBrushKey = ClonedAttrib.GetGDIKey ((int)OBJECTKEY_TYPE._BRUSH);
			int nTargetPenKey = ClonedAttrib.GetGDIKey ((int)OBJECTKEY_TYPE._PEN);
			int nTargetFontKey = ClonedAttrib.GetGDIKey ((int)OBJECTKEY_TYPE._FONT);


			if (0 < nTargetBrushKey)
			{
				if (null != sourceBrush)
				{
					CObjectBrush newBrush = new CObjectBrush ();
					newBrush.AddRef ();
					newBrush.Color = sourceBrush.Color;
					newBrush.BrushStyle = sourceBrush.BrushStyle;
					TargetGDIManager.GetKey (ref nTargetBrushKey, newBrush);
				}
				ClonedAttrib.SetGDIKey ((int)OBJECTKEY_TYPE._BRUSH, nTargetBrushKey);
			}

			if (0 < nTargetPenKey)
			{
				if (null != sourcePen)
				{
					CObjectPen newPen = new CObjectPen ();
					newPen.AddRef ();
					newPen.Color = sourcePen.Color;
					newPen.PenThick = sourcePen.PenThick;
					newPen.PenStyle = sourcePen.PenStyle;
					TargetGDIManager.GetKey (ref nTargetPenKey, newPen);
				}
				ClonedAttrib.SetGDIKey ((int)OBJECTKEY_TYPE._PEN, nTargetPenKey);
			}

			if (0 < nTargetFontKey)
			{
				if (null != sourceFont)
				{
					CObjectFont newFont = new CObjectFont ();
					newFont.AddRef ();

					string strFontName = sourceFont.SelectFont.OriginalFontName;
					float fFontSize = sourceFont.SelectFont.Size;
					bool bBold = sourceFont.SelectFont.Bold;
					bool bItalic = sourceFont.SelectFont.Italic;
					bool bUnderLine = sourceFont.SelectFont.Underline;
					bool bStrikeOut = sourceFont.SelectFont.Strikeout;

					System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;

					if (true == bBold) fontStyle |= System.Drawing.FontStyle.Bold;
					if (true == bItalic) fontStyle |= System.Drawing.FontStyle.Italic;
					if (true == bUnderLine) fontStyle |= System.Drawing.FontStyle.Underline;
					if (true == bStrikeOut) fontStyle |= System.Drawing.FontStyle.Strikeout;

					newFont.SelectFont = new System.Drawing.Font (strFontName, fFontSize, fontStyle);
					newFont.Color = sourceFont.Color;
					TargetGDIManager.GetKey (ref nTargetFontKey, newFont);
				}

				ClonedAttrib.SetGDIKey ((int)OBJECTKEY_TYPE._FONT, nTargetFontKey);
			}


			if (true == bDeepCopy)
			{
				int nSourceImageKey = SourceAttrib.GetGDIKey ((int)OBJECTKEY_TYPE._IMAGE);
				int nSourceRolloverImageKey = SourceAttrib.GetRolloverKey ();

				SourceAttrib.GetGDIObjFromKey (ref sourceImage, nSourceImageKey);
				SourceAttrib.GetGDIObjFromKey (ref sourceRolloverImage, nSourceRolloverImageKey);

				int nTargetImageKey = ClonedAttrib.GetGDIKey ((int)OBJECTKEY_TYPE._IMAGE);
				int nTargetRolloverImageKey = ClonedAttrib.GetRolloverKey ();

				ClonedAtom.AtomCore.Attrib.IsVanish = this.AtomCore.Attrib.IsVanish;
				ClonedAtom.AtomCore.Attrib.IsAtomHidden = this.AtomCore.Attrib.IsAtomHidden;
				ClonedAtom.AtomCore.Attrib.IsDisabled = this.AtomCore.Attrib.IsDisabled;
				ClonedAtom.Visibility = this.Visibility;

				//
				ClonedAtom.AtomCore.Attrib.ModelName = this.AtomCore.Attrib.ModelName;
				ClonedAtom.AtomCore.Attrib.RefProperVar = this.AtomCore.Attrib.RefProperVar;

				if (null != this.AtomCore.Processing) //2020-10-16 kys 진행관리자가 설정된 아톰을 복사한경우 진행관리자를 공유하는 버그 발생 Clone처리해줌
				{
					if (null != this.AtomCore.Information)
					{
						ClonedAtom.AtomCore.Processing = this.AtomCore.Information.CloneProcessing (this.AtomCore.Processing);
					}
					else
					{
						ClonedAtom.AtomCore.Processing = this.AtomCore.Processing;
					}
				}
				//

				if (0 < nTargetImageKey)
				{
					if (null != sourceImage)
					{
						CObjectImage newImage = new CObjectImage ();
						newImage.ImagePath = sourceImage.ImagePath;
						TargetGDIManager.GetKey (ref nTargetImageKey, sourceImage);
					}
					ClonedAttrib.SetGDIKey ((int)OBJECTKEY_TYPE._IMAGE, nTargetImageKey);
				}

				if (0 < nTargetRolloverImageKey)
				{
					if (null != sourceRolloverImage)
					{
						CObjectImage newRolloverImage = new CObjectImage ();
						newRolloverImage.ImagePath = sourceRolloverImage.ImagePath;
						TargetGDIManager.GetKey (ref nTargetRolloverImageKey, sourceRolloverImage);
					}
					ClonedAttrib.SetRolloverKey (nTargetRolloverImageKey);
				}

				if (null != SourceAttrib.EBookQuestionsInfo)
				{
					ClonedAttrib.EBookQuestionsInfo = SourceAttrib.EBookQuestionsInfo.Clone ();
				}

				if (null != SourceAttrib.AccessibilityInfo)
				{
					ClonedAttrib.AccessibilityInfo = SourceAttrib.AccessibilityInfo.Clone ();
				}

				// 객체표현
				ObjectStyleAttrib sourceObjectStyle = SourceAttrib.GetObStyle ();
				ObjectStyleAttrib cloneObjectStyle = ClonedAttrib.GetObStyle ();
				CloneObject.CloneProperty (sourceObjectStyle, cloneObjectStyle);
			}

			//탭뷰 계열 아톰일경우 리턴
			if (AtomCore is TabViewAtom)
			{
				return;
			}

			ClonedAtom.CompletePropertyChanged ();
		}

		/// <summary>
		/// 런모드에서 포커스를 가졌는지 체크
		/// </summary>
		/// <returns></returns>
		public virtual bool IsFocusedOnRunMode ()
		{
			//Trace.TraceError("[Error]There is no override method on child");
			return false;
		}

		/// <summary>
		/// 런모드 포커스 상실할때 호출
		/// </summary>
		public virtual void LostFocusOnRunMode ()
		{
		}

		/// <summary>
		/// true List
		/// InputAtomBase
		/// RadioAtomBase
		/// CheckAtomBase
		/// ButtonAtomBase
		/// </summary>
		/// <returns></returns>
		public virtual bool IsRunModeTabMovable ()
		{
			return false;
		}

		/// <summary>
		/// View의 Scaletransform변경에 영향을 받지 않는 컨트롤들의 
		/// Scale 변경은 해당 메서드 override하여 구현
		/// </summary>
		/// <param name="dScale"></param>
		public virtual void OnChangeScreenScale (double dScale)
		{
		}

		public virtual string GetTextForVoice ()
		{
			ExtensionScrollAtom scrollAtomCore = this.AtomCore.GetScrollAtom () as ExtensionScrollAtom;
			ExtensionScrollAtom targetScrollAtomCore = this.AtomCore.GetScrollAtom () as ExtensionScrollAtom;

			//2021-06-15 kys 스크롤에 묶인 기능버튼에서 같은 스크롤에 묶인 입력란 참조시 같은행에 존재하는 입력란의 값을 찾아오도록 한다.
			if (null != scrollAtomCore && null != targetScrollAtomCore && scrollAtomCore == targetScrollAtomCore)
			{
				int nRow = ShiftLib.ROW (this.AtomCore.GetScrollIndex ());
				string strTextValue = scrollAtomCore.GetContentString (this.AtomCore.AtomProperVar, nRow, false);

				return strTextValue;
			}
			else
			{
				string strTextValue = this.AtomCore.GetContentString (true);
				return strTextValue;
			}
		}

		/// <summary>
		/// 2014-12-10-M01 검색창 아톰 Undo/Redo를 위한 Font처리
		/// </summary>
		/// <param name="applyBrush"></param>
		/// <param name="bHeader"></param>
		public virtual void SetAtomFontColor (Brush applyBrush, bool bHeader)
		{
			if (!bHeader)
				Foreground = applyBrush;
		}

		public virtual void SetAtomFontWeight (FontWeight applyFontWeight, bool bHeader)
		{
			if (!bHeader)
				FontWeight = applyFontWeight;
		}

		public virtual void SetAtomFontStyle (FontStyle applyFontStyle, bool bHeader)
		{
			if (!bHeader)
				FontStyle = applyFontStyle;
		}

		public virtual void SetAtomFontSize (double dApplySize, bool bHeader)
		{
			if (!bHeader)
				FontSize = dApplySize;
		}

		public virtual void SetTextUnderLine (TextDecorationLocation underLine, bool bHeader)
		{
		}

		public virtual void SetTextUnderLine (TextDecorationLocation underLine)
		{
		}

		public virtual object GetAtomCore ()
		{
			return null;
		}

		public virtual void SetAtomHorizontalTextAlignment (System.Windows.HorizontalAlignment alignmentType)
		{
		}

		public virtual void SetAtomVerticalTextAlignmentEvent (System.Windows.VerticalAlignment alignmentType)
		{
		}

		public virtual void SetAtomBorderColor (System.Windows.Media.Brush applyBrush)
		{
		}

		public virtual void SetAtomBorderThickness (Thickness applyThickness)
		{
		}

		public virtual void SetAtomShowBorder (bool bUseLine)
		{
		}

		public virtual void SetAtomBackground (System.Windows.Media.Brush applyBrush, bool bIsNoBackground)
		{
		}

		public virtual void SetAtomDashArrayInCore (DoubleCollection applyDashArray)
		{
		}

		public virtual void PrintItemRemove ()
		{
		}

		public virtual void SetCornerRadius (int nValue)
		{
		}

		public virtual void AtomBase_DropItem (ArrayList alData)
		{
		}

		public virtual void Create_SearchWindow ()
		{
		}

		public virtual void RunMode_MouseMove (int nX, int nY, bool bMouseDownLeft)
		{
		}

		public virtual void CloseAtom ()
		{
			this.DataContext = null;

			if (null != m_ResizeAdorner && m_ResizeAdorner.Parent is AdornerLayer adornerLayer)
			{
				adornerLayer.Remove (m_ResizeAdorner);
			}

			m_ResizeAdorner = null;
		}

		/// <summary>
		/// 편집모드에서 자식아톰전체 리스트를 가져오기 위해서 사용한다. VisualTree를 사용하기 때문에 UI가 그려진 상황에서만 활용 가능
		/// </summary>
		/// <returns></returns>
		public virtual List<AtomBase> GetBindAtomList ()
		{
			List<AtomBase> atomList = new List<AtomBase> ();

			atomList = WPFFindChildHelper.GetVisualChildCollection<AtomBase> (this);

			return atomList;
		}

		public virtual void ExecuteAtomClickEvent ()
		{
			Atom atomCore = this.AtomCore;

			if (null != atomCore)
			{
				if (null != atomCore)
				{
					if (-1 != atomCore.ProcessEvent (0, EVS_TYPE.EVS_A_CLICK))
					{
						if (0 <= MsgHandler.CALL_MSG_HANDLER (atomCore, EVS_TYPE.EVS_A_CLICK, null))
						{
							atomCore.ProcessEvent (1, EVS_TYPE.EVS_A_CLICK);
						}
					}
				}

				//스크롤, 그룹묶기에 묶인경우 이벤트 발생
				Information info = atomCore.Information;
				if (null != info)
				{
					info.ExecuteParentScriptEvent (atomCore, EVS_TYPE.EVS_A_CLICK);
				}

				// AtomBase 에서 처리되고 있어 주석처리 함 : 재검토 필요
				//PlayAnimation (AnimationDetailEventDefine.ADE_Atom_Click);
			}
		}

		public virtual void ShowAtomContextMenu ()
		{
			if (true == PQAppBase.IsDeveloperMode)
			{
				Trace.TraceInformation (this.AtomCore.AtomType + " : " + this.GetType ());
			}

			if (null != ShowAtomContextMenuEvent)
			{
				TopView pTopView = (TopView)(m_AtomCore.Information.GetOwnerWnd ());
				DOC_KIND docType = pTopView.GetDocument ().DocType;

				ArrayList alContextMenuParameterList = new ArrayList { this.AtomCore.Attrib.DefaultAtomProperVar, m_AtomCore, docType, this.AtomCore.AtomType };
				ShowAtomContextMenuEvent (alContextMenuParameterList);
			}
		}

		public virtual void SelectAtom ()
        {
            GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyCurrentLocationAndSize (this.Margin, this.Width, this.Height);
            this.SetResizeAdornerVisibility (Visibility.Visible, false);
			this.NotifyCurrentLocationAndSize ();
			this.MoveStartLeft = this.Margin.Left;
			this.MoveStartTop = this.Margin.Top;

			if (null != AtomClickedEvent)
			{
				AtomClickedEvent (this);
			}
		}

		public virtual void InitAtomBaseWhenAtomCoreHasChanged (Atom atomCore)
		{
			AtomCore = atomCore;

			InitializeAtomBase (atomCore);
			//InitializePropertyBinding ();
			//InitializeAtomDefaultProperVar (); Attrib으로 이동
			//InitializeDelegateEvents ();
		}

		public virtual void Sync_FontAttribToAtom (Attrib atomAttrib, bool bIs80Model)
		{
			if (atomAttrib is PopupAttrib)
			{
				return;
			}
			else if (atomAttrib is LandAttrib || atomAttrib is SquareAttrib)
			{
				int nFontKey = atomAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._FONT);
				CObjectFont pObjectFont = null;
				atomAttrib.GetGDIObjFromKey (ref pObjectFont, nFontKey);

				if (null != pObjectFont)
				{
					System.Drawing.Font selectedFont = pObjectFont.SelectFont;

					if (null != selectedFont)
					{
						//80 직접 지정에서 override 메서드로 변경 
						//this.FontFamily = pObjectFont.WpfFont;
						this.SetAtomFontFamily (pObjectFont.WpfFont);

						//this.FontSize = selectedFont.Size;
						//this.FontWeight = true == selectedFont.Bold ? FontWeights.Bold : FontWeights.Normal;
						//this.SetAtomFontFamily(pObjectFont.WpfFont);

						this.SetAtomFontSize (selectedFont.Size);
						this.SetAtomFontWeight (true == selectedFont.Bold ? FontWeights.Bold : FontWeights.Normal);

						this.SetTextUnderLine (true == selectedFont.Underline ? TextDecorationLocation.Underline : TextDecorationLocation.Baseline);
						this.StrikeOutChanged ();

						//80 직접 지정에서 override 메서드로 변경 
						//this.FontStyle = true == selectedFont.Italic ? FontStyles.Italic : FontStyles.Normal;
						//this.Foreground = pObjectFont.WpfColor;
						this.SetAtomFontStyle (true == selectedFont.Italic ? FontStyles.Italic : FontStyles.Normal);

						if (atomAttrib is LandAttrib)
							this.SetAtomFontColor (new SolidColorBrush (((LandAttrib)atomAttrib).ForeColor));
						else
							this.SetAtomFontColor (pObjectFont.WpfColor);

						pObjectFont.SelectFont = selectedFont;
					}
				}

				//if (atomAttrib is CLandAttrib)
				//{
				//	this.Foreground = new SolidColorBrush(((CLandAttrib)atomAttrib).WndForeColor);
				//}
			}
			else if (atomAttrib is GridTableExAttrib gridAttrib)
			{
				foreach (GridTableExItem item in gridAttrib.CellDataList)
				{
					if (null != item)
					{
						CObjectFont pObjectFont = null;
						atomAttrib.GetGDIObjFromKey (ref pObjectFont, item.FontKey);

						if (null != pObjectFont)
						{
							System.Drawing.Font selectedFont = pObjectFont.SelectFont;

							if (null != selectedFont)
							{
								item.CellFontFamily = pObjectFont.WpfFont;

								pObjectFont.SelectFont = selectedFont;
							}
						}
					}
				}
			}
		}


		public virtual void ChangeBoundStatus ()
		{
			//스크롤에 묶인경우 원본아톰은 연계효과 동작을 처리하지 않는다.
			if (false == this is ScrollAtomBase && null != this.AtomCore.GetScrollAtom () && false == m_IsScrollCellItem)
				return;

			Attrib atomAttrib = this.AtomCore.GetAttrib ();

			if (null != atomAttrib.BoundsStatus)
			{
				Information info = this.AtomCore.Information;

				foreach (CBoundsStatus boundsStatus in atomAttrib.BoundsStatus)
				{
					double dDiffValue = -1;

					double atomX = false == double.IsNaN (this.AtomCore.Attrib.AtomX) ? this.AtomCore.Attrib.AtomX : this.Margin.Left;
					double atomY = false == double.IsNaN (this.AtomCore.Attrib.AtomY) ? this.AtomCore.Attrib.AtomY : this.Margin.Top;

					double atomWidth = false == double.IsNaN (this.AtomCore.Attrib.AtomWidth) ? this.AtomCore.Attrib.AtomWidth : this.ActualWidth;
					double atomHeight = false == double.IsNaN (this.AtomCore.Attrib.AtomHeight) ? this.AtomCore.Attrib.AtomHeight : this.ActualHeight;

					if (0 == boundsStatus.BoundStatus) //상하 크기
					{
						dDiffValue = atomHeight - EditAtomHeight + (atomY - EditAtomY);
					}
					else if (1 == boundsStatus.BoundStatus) //상하 위치
					{
						dDiffValue = (atomHeight - EditAtomHeight) + (atomY - EditAtomY);
					}
					else if (2 == boundsStatus.BoundStatus) //좌우 크기 
					{
						dDiffValue = atomWidth - EditAtomWidth + (atomX - EditAtomX);
					}
					else if (3 == boundsStatus.BoundStatus) //좌우 위치
					{
						dDiffValue = (atomWidth - EditAtomWidth) + (atomX - EditAtomX);
					}

					boundsStatus.BaseAtomName = this.ToString ();

					if (false == double.IsNaN (dDiffValue))
					{
						if (false == this is ScrollAtomBase && null != this.AtomCore.GetScrollAtom ())
						{
							ScrollAtom scrollAtomCore = this.AtomCore.GetScrollAtom ();

							int nScrollIndex = this.AtomCore.GetScrollIndex ();
							int nRow = ShiftLib.ROW (nScrollIndex);

							boundsStatus.ChangeBoundStatusScroll (scrollAtomCore, nRow, dDiffValue);
						}
						else
						{
							boundsStatus.ChangeBoundStatus (info, dDiffValue);
						}
					}
				}

				info.ReSizeView ();
			}
		}

		public virtual void ChangeBoundStatusLoaction ()
		{
			//스크롤에 묶인경우 원본아톰은 연계효과 동작을 처리하지 않는다.
			if (null != this.GetScrollAtom () && false == m_IsScrollCellItem)
				return;

			//위치조정에 대한 연계효과만 동작할 수 있다.
			Attrib atomAttrib = this.AtomCore.GetAttrib ();

			if (null != atomAttrib.BoundsStatus)
			{
				foreach (CBoundsStatus boundsStatus in atomAttrib.BoundsStatus)
				{
					double diffValue = 0;

					switch (boundsStatus.BoundStatus)
					{
						case 0: // 상하 크기
							diffValue = (this.AtomCore.Attrib.AtomHeight - EditAtomHeight) + (this.AtomCore.Attrib.AtomY - EditAtomY);
							break;
						case 1: // 상하 위치
							diffValue = (this.AtomCore.Attrib.AtomHeight - EditAtomHeight) + (this.AtomCore.Attrib.AtomY - EditAtomY);
							break;
						case 2: // 좌우 크기
							diffValue = this.AtomCore.Attrib.AtomWidth - EditAtomWidth + (this.AtomCore.Attrib.AtomX - EditAtomX);
							break;
						case 3: // 좌우 위치
							diffValue = (this.AtomCore.Attrib.AtomWidth - EditAtomWidth) + (this.AtomCore.Attrib.AtomX - EditAtomX);
							break;
					}

					if (false == this is ScrollAtomBase && null != this.AtomCore.GetScrollAtom ())
					{
						ScrollAtom scrollAtomCore = this.AtomCore.GetScrollAtom ();

						int nScrollIndex = this.AtomCore.GetScrollIndex ();
						int nRow = ShiftLib.ROW (nScrollIndex);

						boundsStatus.ChangeBoundStatusScroll (scrollAtomCore, nRow, diffValue);
					}
					else
					{
						boundsStatus.ChangeBoundStatus (this.AtomCore.Information, diffValue);
					}
				}
			}
		}

		#endregion //Public Virtual 메서드


		#region |  ##### Protected Virtual 메서드 |

		protected virtual void ExecuteAtomDoubleClickEvent ()
		{
			Atom atomCore = this.AtomCore;

			if (null != atomCore)
			{
				if (-1 != atomCore.ProcessEvent (0, EVS_TYPE.EVS_A_DBL_CLICK))
				{
					if (0 <= MsgHandler.CALL_MSG_HANDLER (atomCore, EVS_TYPE.EVS_A_DBL_CLICK, null))
					{
						atomCore.ProcessEvent (1, EVS_TYPE.EVS_A_DBL_CLICK);
					}
				}

				//스크롤, 그룹묶기에 묶인경우 이벤트 발생
				Information info = atomCore.Information;
				if (null != info)
				{
					info.ExecuteParentScriptEvent (atomCore, EVS_TYPE.EVS_A_DBL_CLICK);
				}

				PlayAnimation (AnimationDetailEventDefine.ADE_Atom_DoubleClick);
			}
		}

		protected virtual void ExecuteAtomLongClickEvent ()
		{
			Atom atomCore = this.AtomCore as Atom;

			if (null != atomCore)
			{
				if (-1 != atomCore.ProcessEvent (0, EVS_TYPE.EVS_A_LONGCLICK))
				{
					if (0 <= MsgHandler.CALL_MSG_HANDLER (atomCore, EVS_TYPE.EVS_A_LONGCLICK, null))
					{
						atomCore.ProcessEvent (1, EVS_TYPE.EVS_A_LONGCLICK);
					}
				}

				//스크롤, 그룹묶기에 묶인경우 이벤트 발생
				Information info = atomCore.Information;
				if (null != info)
				{
					info.ExecuteParentScriptEvent (atomCore, EVS_TYPE.EVS_A_LONGCLICK);
				}

				PlayAnimation (AnimationDetailEventDefine.ADE_Atom_LongClick);
			}
		}

		protected virtual void InitializeAtomCore ()
		{
		}

		protected virtual void InitializeAtomBinding ()
		{
			var attrib = this.AtomCore.Attrib;
			this.DataContext = attrib;

			InitBinding (FrameworkElement.WidthProperty, nameof (attrib.AtomWidth));
			InitBinding (FrameworkElement.HeightProperty, nameof (attrib.AtomHeight));
			InitBinding (FrameworkElement.MarginProperty, nameof (attrib.AtomMargin));
			InitBinding (Control.TabIndexProperty, nameof (attrib.AtomRelativeTabIndex));

			attrib.PropertyChanged -= AtomPropertyChanged;
			attrib.PropertyChanged += AtomPropertyChanged;

			this.DataContextChanged += AtomBase_DataContextChanged;
		}

		private void InitBinding (DependencyProperty dp, string name)
		{
			var binding = new Binding (name);
			binding.Source = this.AtomCore.Attrib;
			binding.Mode = BindingMode.TwoWay;
			this.SetBinding (dp, binding);
		}

		private void AtomBase_DataContextChanged (object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is Attrib attrib)
			{
				attrib.PropertyChanged -= AtomPropertyChanged;
				attrib.PropertyChanged += AtomPropertyChanged;
			}
		}

		protected virtual void AtomPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			try
			{
				if (e.PropertyName.Equals (nameof (this.AtomCore.Attrib.IsShadow)))
				{
					RefreshShadowEffect ();
				}
			}
			catch (Exception ex)
			{
				Trace.TraceError (ex.ToString ());
			}
		}

		protected virtual void RefreshShadowEffect ()
		{
			if (Brushes.Transparent == this.GetAtomBorder () && Brushes.Transparent == this.GetAtomBackground ())
			{
				this.Effect = null;
			}
			else
			{
				this.Effect = null;

				if (true == AtomCore.Attrib.IsShadow)
				{
					DropShadowEffect dropShadowEffect = new DropShadowEffect ();

					dropShadowEffect.ShadowDepth = 5;
					dropShadowEffect.Color = Color.FromArgb (255, 209, 209, 209);
					dropShadowEffect.BlurRadius = 10;
					dropShadowEffect.Opacity = 0.9;

					this.Effect = dropShadowEffect;
				}
			}
		}

		protected virtual void InitializeResizeAdorner ()
		{
			//if (null != m_ResizeAdorner)
			//{
			//	m_ResizeAdorner.OnGetTabViewOfAtomEvent += OnGetTabViewOfAtomEvent;
			//}
		}

		protected virtual void InitializeAtomSize ()
		{
		}

		protected virtual void InitializeAnimationHelper ()
		{
			//m_AnimationHelper = new AtomAnimationHelper(this);
		}

		protected virtual void InitializeAtomDefaultProperVar ()
		{

		}

        public new double FontSize
        {
            get => base.FontSize;
            set
            {
                base.FontSize = value;
            }
        }

        /// <summary>
        /// UserControl레벨의 공통 속성 정의
        /// </summary>
        protected virtual void InitializeCommonProperties ()
		{
            /*
			if (this.FontSize > 0)
			{
                Debug.WriteLine($"protected virtual void InitializeCommonProperties () is called /// base.FontSize = {this.FontSize}");
                return;
			}
			else
			{
                this.FontSize = 12;
            }
            */
            this.FontSize = 12;

            Debug.WriteLine($"protected virtual void InitializeCommonProperties () is called");

            if (LC.PQLanguage == LC.LANG.KOREAN)
			{
				this.FontFamily = new FontFamily ("Malgun Gothic");
			}
			else if (LC.PQLanguage == LC.LANG.JAPAN)
			{
				this.FontFamily = new FontFamily ("Yu Gothic UI");
			}
			else if (LC.PQLanguage == LC.LANG.ENGLISH)
			{
				this.FontFamily = new FontFamily ("Segoe UI");
			}
			else
			{
				this.FontFamily = new FontFamily ("Arial");
			}

			this.HorizontalAlignment = HorizontalAlignment.Left;
			this.VerticalAlignment = VerticalAlignment.Top;
			this.RenderTransformOrigin = new Point (0.5, 0.5);
		}

		protected virtual void InitializeDelegateEvents ()
		{
			if (null != m_AtomCore)
			{
				m_AtomCore.OnCompletePropertyChangedEvent -= AtomBase_OnCompletePropertyChangedEvent;
				m_AtomCore.OnNotifyChangedValueByInnerLogic -= AtomCore_OnNotifyChangedValueByInnerLogic;
				m_AtomCore.OnGetOfAtomHandler -= AtomCore_OnGetOfAtomHandler;
				m_AtomCore.OnNotifyChangeAttribEvent -= AtomCore_OnNotifyChangeAttribEvent;

				m_AtomCore.OnCompletePropertyChangedEvent += AtomBase_OnCompletePropertyChangedEvent;
				m_AtomCore.OnNotifyChangedValueByInnerLogic += AtomCore_OnNotifyChangedValueByInnerLogic;
				m_AtomCore.OnGetOfAtomHandler += AtomCore_OnGetOfAtomHandler;
				m_AtomCore.OnNotifyChangeAttribEvent += AtomCore_OnNotifyChangeAttribEvent;
			}
		}

		protected virtual void InitializePropertyBinding ()
		{
			if (null != m_AtomCore)
			{
				Attrib bindingSource = m_AtomCore.GetAttrib ();

				//Binding propertyBinding = new Binding("AtomRunMode");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.AtomRunModeProperty, propertyBinding);

				//propertyBinding = new Binding("AtomFontFamily");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(Control.FontFamilyProperty, propertyBinding);

				//propertyBinding = new Binding("AtomFontSize");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(Control.FontSizeProperty, propertyBinding);

				//propertyBinding = new Binding("AtomFontStyle");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(Control.FontStyleProperty, propertyBinding);

				//propertyBinding = new Binding("AtomFontWeight");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(Control.FontWeightProperty, propertyBinding);

				//propertyBinding = new Binding("AtomHorizontalAlignment");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(Control.HorizontalAlignmentProperty, propertyBinding);

				//propertyBinding = new Binding("AtomVerticalAlignment");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(Control.VerticalAlignmentProperty, propertyBinding);

				//propertyBinding = new Binding("AtomMargin");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(Control.MarginProperty, propertyBinding);

				//propertyBinding = new Binding("AtomWidth");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(Control.WidthProperty, propertyBinding);

				//propertyBinding = new Binding("AtomHeight");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(Control.HeightProperty, propertyBinding);

				//propertyBinding = new Binding("AtomRelativeTabIndex");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(Control.TabIndexProperty, propertyBinding);

				//propertyBinding = new Binding("AtomAbsoluteTabIndex");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.AtomAbsoluteTabIndexProperty, propertyBinding);

				//propertyBinding = new Binding("DefaultAtomProperVar");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.DefaultAtomProperVarProperty, propertyBinding);

				//propertyBinding = new Binding("AtomProperVar");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.AtomProperVarProperty, propertyBinding);

				//propertyBinding = new Binding("IsBindedPopup");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.IsBindedPopupProperty, propertyBinding);

				//propertyBinding = new Binding("IsBindedScroll");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.IsBindedScrollProperty, propertyBinding);

				//propertyBinding = new Binding("IsBindedTabView");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.IsBindedTabViewProperty, propertyBinding);

				//propertyBinding = new Binding("WndVisible");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.WndVisibleProperty, propertyBinding);

				//propertyBinding = new Binding("WndDisabled");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.WndDisabledProperty, propertyBinding);

				//propertyBinding = new Binding("DwAttrib");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.DwAttribProperty, propertyBinding);

				//            propertyBinding = new Binding ("AtomOpacity");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//            this.SetBinding (AtomBase.AtomOpacityProperty, propertyBinding);

				//propertyBinding = new Binding("RefProperVar");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.RefProperVarProperty, propertyBinding);

				//propertyBinding = new Binding("Operate");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.OperateProperty, propertyBinding);

				//            propertyBinding = new Binding ("ConditionOperate");
				//            propertyBinding.Source = bindingSource;
				//            propertyBinding.Mode = BindingMode.TwoWay;
				//            this.SetBinding (AtomBase.ConditionOperateProperty, propertyBinding);

				//propertyBinding = new Binding("ChangeOperate");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.ChangeOperateProperty, propertyBinding);

				//propertyBinding = new Binding("ModelName");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.ModelNameProperty, propertyBinding);

				//propertyBinding = new Binding("Vanish");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.VanishProperty, propertyBinding);

				//propertyBinding = new Binding("AutoDisabled");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.AutoDisabledProperty, propertyBinding);

				//propertyBinding = new Binding("IsProperBtnExec");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.ProperBtnExecProperty, propertyBinding);

				//propertyBinding = new Binding("UseViewColor");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.UseViewColorProperty, propertyBinding);

				//propertyBinding = new Binding("StyleSheet");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.StyleSheetProperty, propertyBinding);

				//propertyBinding = new Binding("NinePatchBrightnessColor");
				//propertyBinding.Source = bindingSource;
				//propertyBinding.Mode = BindingMode.TwoWay;
				//this.SetBinding(AtomBase.NinePatchBrightnessColorProperty, propertyBinding);

				//            propertyBinding = new Binding ("EnableDrag");
				//            propertyBinding.Source = bindingSource;
				//            propertyBinding.Mode = BindingMode.TwoWay;
				//            this.SetBinding (AtomBase.EnableDragProperty, propertyBinding);

				//bindingSource = null;
			}
		}

		protected virtual void PlayMode_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (AtomCore != null)
			{
				AtomCore.PlayAnimation (AnimationDetailEventDefine.ADE_Atom_Click);
				AtomCore.PlayEmphasisAnimation (AnimationDetailEventDefine.ADE_Atom_Click);
			}
		}

		#endregion//Protected Virtual 메서드

		#region |  ##### Public 메서드 #####  |

		public void UndoInitializeResizeAdorner ()
		{
			InitializeResizeAdorner ();
		}

		/// <summary>
		/// 이 아톰이 탭뷰에 묶여있으면 탭뷰를 리턴한다.
		/// </summary>
		/// <returns></returns>
		public FrameworkElement GetTabViewAtom ()
		{
			FrameworkElement bindedAtom = this;

			while (true)
			{
				FrameworkElement parentElement = bindedAtom.Parent as FrameworkElement;

				if (null != parentElement)
				{
					if (parentElement is TabViewAtomBase)
					{
						return parentElement;
					}
					else
					{
						bindedAtom = parentElement;
					}
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// 아톰을 잠금 / 해제
		/// </summary>
		public void OnToolLockOrUnLock ()
		{
			AtomCore.OnToolLockOrUnLock ();
			m_ResizeAdorner.LockOrUnLockHost (AtomCore.Attrib.IsLocked, this.GetType ());

			if (false != AtomCore.Attrib.IsLocked)
			{
				this.Opacity = LOCK_OPACITY;
			}
			else
			{
				Attrib atomAttrib = AtomCore.GetAttrib ();
				this.Opacity = (double)(atomAttrib.AtomOpacity / 100.0);
			}

			OnLockStateChange ();
		}

		public virtual void OnLockStateChange () { }

		/// <summary>
		/// 아톰 잠금
		/// </summary>
		public void OnToolLock ()
		{
			AtomCore.OnToolLock ();
			m_ResizeAdorner.LockOrUnLockHost (AtomCore.Attrib.IsLocked, this.GetType ());

			this.Opacity = LOCK_OPACITY;
			OnLockStateChange ();
		}

		/// <summary>
		/// 아톰 잠금 해제
		/// </summary>
		public void OnToolUnLock ()
		{
			AtomCore.OnToolUnLock ();
			m_ResizeAdorner.LockOrUnLockHost (AtomCore.Attrib.IsLocked, this.GetType ());

			Attrib atomAttrib = this.AtomCore.GetAttrib ();
			this.Opacity = (double)(atomAttrib.AtomOpacity / 100.0);
			OnLockStateChange ();
		}

		public void PlayAnimation (AnimationDetailEventDefine adeType)
		{
			AtomCore.PlayAnimation (adeType);
		}

		public void PlayEmphasisAnimation (AnimationDetailEventDefine adeType)
		{
			AtomCore.PlayEmphasisAnimation (adeType);
		}

		public void SetFormChange ()
		{
			Information info = AtomCore.Information;
			if (null != info)
			{
				info.SetFormChangeFromAtom ();
			}
		}

		public void ChangeAtomTextEditMode ()
		{
			Information info = this.AtomCore?.Information;
			info?.ChangeAtomTextEditMode (TextEditMode);
		}

		public TextAlignment GetTextAlignment (HorizontalAlignment horizontalTextAlignment)
		{
			switch (horizontalTextAlignment)
			{
				case HorizontalAlignment.Left: return TextAlignment.Left;
				case HorizontalAlignment.Right: return TextAlignment.Right;
				case HorizontalAlignment.Center: return TextAlignment.Center;
			}

			return TextAlignment.Justify;
		}

		#endregion//Public 메서드


		#region |  ##### 이벤트 #####  |

		protected virtual void AtomBase_Drop (object sender, DragEventArgs e)
		{
			if (0 == this.AtomCore.AtomRunMode)
			{
				string strData = (string)(e.Data.GetData (System.Windows.Forms.DataFormats.Text));
				Point ptMouse = e.GetPosition (this);

				if (false == string.IsNullOrEmpty (strData))
				{
					// 2014-03-24-C01
					string[] strArry = strData.Split (',');

					for (int i = 0; i < strArry.Length; i++)
					{
						ArrayList alDragData = new ArrayList ();
						strData = strArry[i];
						StrLib.KissSplitString (alDragData, strData, ';');

						if (3 < alDragData.Count)
						{
							Atom atomCore = AtomCore as Atom;
							if (null != atomCore)
							{
								if (5 == alDragData.Count)
									AtomCore.SetFieldInfoWhenDropFromSAPManager (alDragData, ptMouse);
								else
									AtomCore.SetDBInfoWhenDropFromTopDBManager (alDragData, ptMouse);
							}
							else
							{
								AtomBase_DropItem (alDragData);
							}
						}
					}

					InvalidateVisual ();
				}
			}
		}

		private void AtomBase_DragOver (object sender, DragEventArgs e)
		{
			if (false != e.Data.GetDataPresent (DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Move;
			}
			else
			{
				e.Effects = DragDropEffects.Link | DragDropEffects.Move | DragDropEffects.Copy;
			}

			e.Handled = true;
		}

		private void AtomCore_OnNotifyChangeAttribEvent (object objValue)
		{
			if (null != OnNotifyChangeAttribEvent)
				OnNotifyChangeAttribEvent (objValue, (AtomBase)this);
		}

		private void AtomBase_Initialized (object sender, EventArgs e)
		{
			//아톰의 컨버터 셋팅
			BindValueConverter ();
		}

		private object AtomCore_OnGetOfAtomHandler ()
		{
			return this;
		}

		private void AtomCore_OnNotifyChangedValueByInnerLogic (object changedValue)
		{
			NotifyChangedValueByInnerLogic (changedValue);
		}

		private void AtomBase_OnCompletePropertyChangedEvent ()
		{
			CompletePropertyChanged ();
		}

		public void AtomBase_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			switch (this.AtomCore.AtomRunMode)
			{
				//TOOL_MODE
				case 0:
					{
						break;
					}

				//PLAY_MODE
				case 1:
					{
						PlayMode_MouseLeftButtonDown (sender, e);
						break;
					}

				//ORDER_MODE
				case 2:
					{
						break;
					}
				//GHOST_MODE
				case 3:
					{
						break;
					}
				//RPTEDIT_MODE
				case 4:
					{
						break;
					}
				default: break;
			}
		}

		private void AtomBase_MouseDoubleClick (object sender, MouseButtonEventArgs e)
		{
			switch (this.AtomCore.AtomRunMode)
			{
				//TOOL_MODE
				case 0:
					{
						ToolMode_MousDoubleClick (sender, e);
						break;
					}

				//PLAY_MODE
				case 1:
					{
						PlayMode_MouseDoubleClick (sender, e);
						break;
					}
				//ORDER_MODE
				case 2:
					{
						break;
					}
				//GHOST_MODE
				case 3:
					{
						break;
					}
				//RPTEDIT_MODE
				case 4:
					{
						break;
					}
				default: break;
			}
		}

		private void AtomBase_MouseDown (object sender, MouseButtonEventArgs e)
		{
			if (this.GetScrollAtom () is ScrollAtomBase scroll)
			{
				scroll.Focus ();
			}
			else
			{
				this.Focusable = true;
				this.Focus ();
				this.Focusable = false;
			}
		}

		private void PlayMode_MouseDoubleClick (object sender, MouseButtonEventArgs e)
		{
			if (AtomCore != null)
				AtomCore.PlayAnimation (AnimationDetailEventDefine.ADE_Atom_DoubleClick);
		}

		private void ToolMode_MousDoubleClick (object sender, MouseButtonEventArgs e)
		{
			if (null != AtomDoubleClickedEvent && MouseButton.Left == e.ChangedButton
				&& true == CheckAtomComponentHasGotMouseHandle ())
			{
				if (AtomCore != null)
				{
					if (false != AtomCore.Attrib.IsLocked)
						return;
				}

				//탭과 스크롤 안에서 더블클릭했을때 처리하는것을 막음

				if (//this is Ebook.EBookTextofAtom || 2020-06-19 편집논리 변경으로 주석처리함
					 this is Ebook.EBookImageofAtom
					|| this is Ebook.EBookAvatarofAtom
					|| this is Ebook.EBookChartofAtom
					//|| this is Ebook.EBookFigureOfAtom
					|| this is TabViewAtomBase
					|| this is ScrollAtomBase
				   )
				{
					return;
				}

				AtomDoubleClickedEvent (this);
			}
		}

		#endregion//이벤트


		#region |  ##### Override 메서드 #####  |

		protected override void OnRender (DrawingContext drawingContext)
		{
			base.OnRender (drawingContext);

			if (null != m_AtomCore)
			{
				if (1 != this.AtomCore.AtomRunMode)
				{
					if (null != m_AtomCore)
					{
						Attrib AtomAttrib = m_AtomCore.GetAttrib () as Attrib;

						if (null != AtomAttrib)
						{
							if (true == this.AtomCore.IsBindedPopup || true == this.AtomCore.IsBindedScroll || null != AtomCore.HyperLinkAtom || null != AtomCore.BindBlockAtom)
							{
								drawingContext.DrawRectangle (m_BindedPopupBrush, null, new Rect (6, -4, 4, 4));
							}

							if (true == IsZIndexTextVisible)
							{
								var textPoint = new Point (10, -18);
								var textBrush = Brushes.Red;

								int nZindex = Canvas.GetZIndex (this);
								CommonStringGenerator.StringGenerator.Clear ();
								CommonStringGenerator.StringGenerator.AppendFormat ("Z{0}", nZindex);
								var formattedText = new FormattedText (CommonStringGenerator.StringGenerator.ToString (), CultureInfo.GetCultureInfo ("en-us"), FlowDirection.LeftToRight, new Typeface ("Tahoma"), 10, textBrush);
								drawingContext.DrawText (formattedText, textPoint);
							}

							if (true == IsRelativeTabIndexTextVisible)
							{
								var textPoint = new Point (-18, 0);
								var textBrush = Brushes.Orange;

								int nTabindex = TabIndex;
								CommonStringGenerator.StringGenerator.Clear ();
								CommonStringGenerator.StringGenerator.AppendFormat ("R{0}", nTabindex);
								var formattedText = new FormattedText (CommonStringGenerator.StringGenerator.ToString (), CultureInfo.GetCultureInfo ("en-us"), FlowDirection.LeftToRight, new Typeface ("Tahoma"), 10, textBrush);
								drawingContext.DrawText (formattedText, textPoint);
							}

							if (true == IsAbsoluteTabIndexTextVisible)
							{
								var textPoint = new Point (-18, 10);
								var textBrush = Brushes.Orange;

								CommonStringGenerator.StringGenerator.Clear ();
								CommonStringGenerator.StringGenerator.AppendFormat ("A{0}", this.AtomCore.Attrib.AtomAbsoluteTabIndex);
								var formattedText = new FormattedText (CommonStringGenerator.StringGenerator.ToString (), CultureInfo.GetCultureInfo ("en-us"), FlowDirection.LeftToRight, new Typeface ("Tahoma"), 10, textBrush);
								drawingContext.DrawText (formattedText, textPoint);
							}

							if (true == IsDebugDBInfoVisible)
							{
								var textPoint = new Point (0, this.Height + 3);
								var textBrush = Brushes.ForestGreen;

								string strFieldName = AtomAttrib.GetFieldName (false);
								string strFieldLength = AtomAttrib.GetFieldLen ().ToString ();
								string strFieldType = AtomAttrib.GetFieldType ();

								if (true != string.IsNullOrEmpty (strFieldName))
								{
									CommonStringGenerator.StringGenerator.Clear ();
									CommonStringGenerator.StringGenerator.Append (strFieldName).Append (" [").Append (strFieldType).Append ("(").Append (strFieldLength).Append (")").Append ("]");
									var formattedText = new FormattedText (CommonStringGenerator.StringGenerator.ToString (), CultureInfo.GetCultureInfo ("en-us"), FlowDirection.LeftToRight, new Typeface ("Tahoma"), 10, textBrush);
									drawingContext.DrawText (formattedText, textPoint);
								}
							}

							if (true == IsAtomFieldTextVisible)
							{
								if (false != AtomAttrib.IsLoadField)
								{
									BitmapImage clickImage = new BitmapImage (new Uri ("pack://application:,,,/BOS08;component/Resources/PK.png"));
									if (null != clickImage)
									{
										drawingContext.DrawImage (clickImage, new Rect (-15, -6, 18, 18));
									}
								}
							}

							TryAtomNameTextVisible ();
						}
					}
				}
			}
		}

		public override string ToString ()
		{
			if (true == string.IsNullOrEmpty (this.AtomCore.AtomProperVar))
			{
				return base.ToString ();
			}

			return $"AtomBase : {this.AtomCore.AtomProperVar}";
		}

		#endregion//Override 메서드
	}
}