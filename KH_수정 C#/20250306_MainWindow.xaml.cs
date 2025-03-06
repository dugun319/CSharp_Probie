
#region |  ##### Using #####  |

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using CommandCenter;
using DocumentFormat.OpenXml.EMMA;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Softpower.SmartMaker.AnimationLib;
using Softpower.SmartMaker.ArduinoEdit.ViewModels;
using Softpower.SmartMaker.ArduinoEdit.Views;
using Softpower.SmartMaker.ArduinoProcess.Views;
using Softpower.SmartMaker.Common.Error;
using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.Common.Log;
using Softpower.SmartMaker.DBCoreX.Common;
using Softpower.SmartMaker.DBCoreX.DBManager.Profiler;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.DeployGenerator;
using Softpower.SmartMaker.DeployServer;
using Softpower.SmartMaker.FileDBIO80;
using Softpower.SmartMaker.FileDBIO80.Option;
using Softpower.SmartMaker.License.Manager;
using Softpower.SmartMaker.License.Manager.Join.SolutionEx;    //  라이선스 체제 변경안(v3.0)
using Softpower.SmartMaker.ModelGenerator.Components.TabView.Model;
using Softpower.SmartMaker.PDFConverter;
using Softpower.SmartMaker.PDFConverter.Components.ViewModel.Element;
using Softpower.SmartMaker.Script;
using Softpower.SmartMaker.SmartCMS;
using Softpower.SmartMaker.SmartCMS.Components;
using Softpower.SmartMaker.SmartCMS.ViewModel;
using Softpower.SmartMaker.SmartGenerator.TopHtml.PQBos.Service;
using Softpower.SmartMaker.StyleResourceDictionary;
using Softpower.SmartMaker.TopAnimation.Controls.TTS;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.Analytics;
using Softpower.SmartMaker.TopApp.Aws;
using Softpower.SmartMaker.TopApp.ChromeBrowser;
using Softpower.SmartMaker.TopApp.CommonLib.BusyIndicators;
using Softpower.SmartMaker.TopApp.CommonLib.Interface;
using Softpower.SmartMaker.TopApp.CommonLib.StringGenerator;
using Softpower.SmartMaker.TopApp.EduTech;
using Softpower.SmartMaker.TopApp.EduTech.CMS;
using Softpower.SmartMaker.TopApp.EduTech.QuizMaker;
using Softpower.SmartMaker.TopApp.FormInformation;
using Softpower.SmartMaker.TopApp.License;
using Softpower.SmartMaker.TopApp.Login;
using Softpower.SmartMaker.TopApp.Login.Hanwha;
using Softpower.SmartMaker.TopApp.MDI;
using Softpower.SmartMaker.TopApp.Metadata;
using Softpower.SmartMaker.TopApp.Metadata.QuizMaker;
using Softpower.SmartMaker.TopApp.TTS.Helper;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopAtom.Ebook.Components;
using Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView;
using Softpower.SmartMaker.TopBuild.InteractiveTutorial;
using Softpower.SmartMaker.TopBuild.Views.EduTech;
using Softpower.SmartMaker.TopBuild.Views.EduTech.AcdemyManager;
using Softpower.SmartMaker.TopBuild.Views.EduTech.ProjectManager;
using Softpower.SmartMaker.TopBuild.Views.EduTech.QuizDBManager;
using Softpower.SmartMaker.TopBuild.Views.EduTech.UserInfoManager;
using Softpower.SmartMaker.TopBuild.Views.ExpandSubMenu;
using Softpower.SmartMaker.TopBuild.Views.FrameToggle;
using Softpower.SmartMaker.TopBuild.Views.License;
using Softpower.SmartMaker.TopBuild.Views.OpenXML;
using Softpower.SmartMaker.TopBuild.Views.OpenXML.MultiConverter;
using Softpower.SmartMaker.TopBuild.Views.OpenXML.Presentation;
using Softpower.SmartMaker.TopBuild.Views.OpenXML.Word;
using Softpower.SmartMaker.TopBuild.Views.PDF;
using Softpower.SmartMaker.TopBuild.Views.Sap;
using Softpower.SmartMaker.TopBuild.Views.Security;
using Softpower.SmartMaker.TopBuild.Views.Simulator;
using Softpower.SmartMaker.TopBuild.Views.StatusBar.Components;
using Softpower.SmartMaker.TopBuild.Views.Subsidiary;
using Softpower.SmartMaker.TopBuild.Views.WebFont;
using Softpower.SmartMaker.TopControl.Components.Container;
using Softpower.SmartMaker.TopControl.Components.Dialog;
using Softpower.SmartMaker.TopControl.Components.EBook;
using Softpower.SmartMaker.TopControl.Components.EBook.SlideMaster.Helper;
using Softpower.SmartMaker.TopControl.Components.EBook.SlideMaster.Model;
using Softpower.SmartMaker.TopControl.Components.PaintTool;
using Softpower.SmartMaker.TopControlEdit.KeyManager;
using Softpower.SmartMaker.TopControlEdit.PushSoundManager;
using Softpower.SmartMaker.TopDBManager80.ERDGenManager;
using Softpower.SmartMaker.TopEdit80;
using Softpower.SmartMaker.TopLight;
using Softpower.SmartMaker.TopLight.Commands;
using Softpower.SmartMaker.TopLight.Spoit;
using Softpower.SmartMaker.TopLight.StructDataMgr.Profiler;
using Softpower.SmartMaker.TopLight.ViewModels;
using Softpower.SmartMaker.TopMenu;
using Softpower.SmartMaker.TopProcess;
using Softpower.SmartMaker.TopProcess.CMSGenerator;
using Softpower.SmartMaker.TopProcess.Component.ViewModels;
using Softpower.SmartMaker.TopProcess.Component.Views;
using Softpower.SmartMaker.TopProcessEdit.TraceManager;
using Softpower.SmartMaker.TopReportAtom.AtomControl.BaseControl;
using Softpower.SmartMaker.TopReportAtom.AtomCore;
using Softpower.SmartMaker.TopReportProcess.View;
using Softpower.SmartMaker.TopReportProcess.ViewModel;
using Softpower.SmartMaker.TopSmartAtom;
using Softpower.SmartMaker.TopSmartAtomEdit.Function80;
using Softpower.SmartMaker.TopSmartAtomManager.BaseContextMenu;
using Softpower.SmartMaker.TopSmartAtomManager.BaseControls;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;
using Softpower.SmartMaker.TopSmartAtomManager.SmartCore;
using Softpower.SmartMaker.TopStructDataManager80.Tool.Launchpad;

#endregion

namespace Softpower.SmartMaker.TopBuild
{
	public partial class MainWindow : Window, IMDIParentWindow
	{
		#region |  ##### DllImport : 마우스 위치의 모니터  정보 #####  |

		[DllImport ("user32.dll")]
		[return: MarshalAs (UnmanagedType.Bool)]
		internal static extern bool GetCursorPos (ref Win32Point pt);

		[StructLayout (LayoutKind.Sequential)]
		internal struct Win32Point
		{
			public Int32 X;
			public Int32 Y;
		};

		#endregion//DllImport

		#region |  ##### Private 전역변수 #####  |

		private bool m_bIsWindowResizing;
		private object m_pJobGInfo = null;
		private string m_strProperty = string.Empty;
		private string m_strExploreFile = string.Empty;
		private bool m_bErdAuto = false;
		private Point m_PtMouseGripStart;

		private Softpower.SmartMaker.TopProcessEdit.DBManager80.DBFieldTreeWindow m_dbFieldTree80;
		private Softpower.SmartMaker.TopProcessEdit.DBManager80.DBFieldTreeWindow m_dbHanaFieldTree;
		private Softpower.SmartMaker.TopProcessEdit.TraceManager.SQLTraceWindow m_SQLTraceWindow;
		private Softpower.SmartMaker.TopProcessEdit.TraceManager.HttpTraceWindow m_HttpTraceWindow;
		private Softpower.SmartMaker.TopProcessEdit.TraceManager.ScriptTraceWindow m_ScriptTraceWindow;
		private Softpower.SmartMaker.TopProcessEdit.TraceManager.LRSTraceWindow m_LRSTraceWindow;
		private EdutechQuizDBManager m_EdutechQuizDBManager;

		private MainWindowManager m_MainWindowManager;
		private AnimatableWindow m_AtomAttPageWindow;

		private SubsidiaryWindow m_SubsidiaryWindow;
		private SimulatorWindow m_SimulatorWindow;
		private GoogleFontWindow m_GoogleFontWindow;
		private PDFViewer m_PDFViewer;
		private FrameTogglePopup _FrameTogglePopup = new FrameTogglePopup ();
		private QuizMakerPreviewWindow _QuizMakerPreviewWindow = null;

		private bool m_pIsFreeVersion = false;
		private bool m_IsFileSaveFlag = false;
		//private JettyServer m_pJettyServer = new JettyServer();
		//private EmulatorManager m_pEmulatorManager = null;

		#endregion//Private 전역변수


		#region |  ##### Private 속성 #####  |

		public FrameTogglePopup FrameTogglePopup
		{
			get
			{
				_FrameTogglePopup.Closed -= FrameTogglePopup_Closed;
				_FrameTogglePopup.Closed += FrameTogglePopup_Closed;

				if (null == _FrameTogglePopup.Owner)
				{
					_FrameTogglePopup.Owner = this;
				}

				return _FrameTogglePopup;
			}
		}

		#region |  AnimatableWindow AtomAttWindow  |

		private AnimatableWindow AtomAttWindow
		{
			get
			{
				if (null == m_AtomAttPageWindow)
				{
					m_AtomAttPageWindow = new AnimatableWindow ();
					RegisterName ("AtomAttWindow", m_AtomAttPageWindow);
					m_AtomAttPageWindow.Owner = this;
				}
				return m_AtomAttPageWindow;
			}
			set
			{
				m_AtomAttPageWindow = value;
			}
		}

		#endregion

		private SubsidiaryWindow SubsidiaryWindow
		{
			get
			{
				if (null == m_SubsidiaryWindow)
				{
					m_SubsidiaryWindow = new SubsidiaryWindow ();
					m_SubsidiaryWindow.Owner = this;
					m_SubsidiaryWindow.Closed += SubsidiaryWindow_Closed;
				}
				return m_SubsidiaryWindow;
			}
			set
			{
				m_SubsidiaryWindow = value;
			}
		}

		private void SubsidiaryWindow_Closed (object sender, EventArgs e)
		{
			m_SubsidiaryWindow = null;
		}

		private SimulatorWindow SimulatorWindow
		{
			get
			{
				if (null == m_SimulatorWindow)
				{
					m_SimulatorWindow = new SimulatorWindow ();
					m_SimulatorWindow.Owner = this;
					m_SimulatorWindow.Closed += SimulatorWindow_Closed;
				}
				return m_SimulatorWindow;
			}
			set
			{
				m_SimulatorWindow = value;
			}
		}

		private void SimulatorWindow_Closed (object sender, EventArgs e)
		{
			m_SimulatorWindow = null;
		}


		#region |  object JobGInfo  |

		private object JobGInfo
		{
			get
			{
				if (null == m_pJobGInfo)
				{
					m_pJobGInfo = new JobGInfo (this);
				}

				return m_pJobGInfo;
			}
		}

		#endregion

		#endregion//Private 속성


		#region |  ##### Public 속성 #####  |

		#region |  bool ErdAuto  |

		public bool ErdAuto
		{
			get
			{
				return m_bErdAuto;
			}
			set
			{
				m_bErdAuto = value;
			}
		}

		public bool ActiveDocumentRunMode
		{
			get
			{
				TopDoc pDoc = GetActiveDocument ();
				if (null != pDoc)
				{
					return pDoc.GetRunMode ();
				}
				return false;
			}
		}
		#endregion

		#region |  string DBFileProperty  |

		public string DBFileProperty
		{
			get
			{
				return m_strProperty;
			}
			set
			{
				m_strProperty = value;
			}
		}

		#endregion

		#region |  string ExploreFile  |

		public string ExploreFile
		{
			get { return m_strExploreFile; }
			set
			{
				m_strExploreFile = value;
			}
		}

		#endregion

		#region |  FrameworkElement MoveAreaControl : MDI  |

		public FrameworkElement MoveAreaControl
		{
			get
			{
				return MainScrollViewer;
			}
		}

		#endregion

		public EdutechQuizDBManager EdutechQuizDBManager
		{
			get { return m_EdutechQuizDBManager; }
		}

		#endregion//Public 속성


		#region |  ##### 생성자 #####  |

		#region |  MainWindow()  |

		public MainWindow ()
		{
			InitializeComponent ();
			InitWindowStyle ();
			InitializeVariable ();
			InitializeMainWindowEvents ();
			InitializeChildrenEvents ();
			InitMessage ();

			ApplyApplicationComponentStyle (StyleResourceManager.CurrentTheme);
			UpdateMenuStateAll (null);

			//PQAppBase.CompleteJettyServerEnv += OnCompleteJettyServerEnv;
			//SetVersionView();

			// 181217_AHN : 다국어 글꼴 속성 (메인 프레임)
			// - LC80.FormLocalize() 글꼴 속성 설정
			if (LC.PQLanguage == LC.LANG.JAPAN)
			{
				this.FontFamily = new System.Windows.Media.FontFamily ("Yu Gothic UI");
			}

			//2020-10-22 kys 안내선 정보 로드
			Softpower.SmartMaker.TopApp.Ruler.RulerDataManager.Load ();

			//2020-12-11 kys 캘린더아톰 공휴일정보 로드
			TopAtom.Components.CalendarAtom.HolidayManager.Instance.Load ();

			// SSL 
			PQAppBase.InitServicePointManager ();

			PrivateFontManager.Instance.TaskAddFontRegistry ();

			Softpower.SmartMaker.TopControl.mBizSplash.Instance.Close ();
			Application.Current.MainWindow = this;
		}

		#endregion

		#endregion//생성자


		#region |  ##### Private 메서드 #####  |

		#region |  void InitWindowStyle()  |

		private void InitWindowStyle ()
		{

			WindowChrome Resizable_BorderLess_Chrome = new WindowChrome ();
			Resizable_BorderLess_Chrome.GlassFrameThickness = new Thickness (5);
			Resizable_BorderLess_Chrome.CornerRadius = new CornerRadius (0);
			Resizable_BorderLess_Chrome.CaptionHeight = 24;
			Resizable_BorderLess_Chrome.ResizeBorderThickness = new Thickness (5);
			WindowChrome.SetIsHitTestVisibleInChrome (MainMenu.WindowButtonGrid, true);
			WindowChrome.SetIsHitTestVisibleInChrome (MainMenu.FileMenuItemTextBlock, true);
			WindowChrome.SetIsHitTestVisibleInChrome (MainMenu.EditMenuItemTextBlock, true);
			WindowChrome.SetIsHitTestVisibleInChrome (MainMenu.ToolMenuItemTextBlock, true);
			WindowChrome.SetIsHitTestVisibleInChrome (MainMenu.DBMenuItemTextBlock, true);
			WindowChrome.SetIsHitTestVisibleInChrome (MainMenu.ServerMenuItemTextBlock, true);
			WindowChrome.SetIsHitTestVisibleInChrome (MainMenu.DeployMenuItemTextBlock, true);
			WindowChrome.SetIsHitTestVisibleInChrome (MainMenu.WindowMenuItemTextBlock, true);
			WindowChrome.SetIsHitTestVisibleInChrome (MainMenu.ConfigMenuItemTextBlock, true);
			WindowChrome.SetIsHitTestVisibleInChrome (MainMenu.HelpMenuItemTextBlock, true);
			WindowChrome.SetWindowChrome (this, Resizable_BorderLess_Chrome);

			MainStatusBarSeperator.Background = new SolidColorBrush (Color.FromArgb (255, 209, 209, 209));
		}

		#endregion

		#region |  void InitializeVariable() : MainWindowManager 객체 생성  |

		private void InitializeVariable ()
		{
			m_MainWindowManager = new MainWindowManager ();
		}

		#endregion

		#region |  void InitializeMainWindowEvents()  |

		private void InitializeMainWindowEvents ()
		{
			this.PreviewMouseMove += MainWindow_PreviewMouseMove;
			this.PreviewMouseDown += MainWindow_PreviewMouseDown;
			this.PreviewKeyDown += MainWindow_PreviewKeyDown;
			this.PreviewKeyUp += MainWindow_PreviewKeyUp;//Kiho : 2016-11-04 추가, LeftCtrl Key Up 이벤트 발생시 처리를 위해.
			this.MouseUp += MainWindow_MouseUp;
			this.Loaded += MainWindow_Loaded;
			this.Deactivated += MainWindow_Deactivated;

			this.Closing += MainWindow_Closing;
			this.StateChanged += MainWindow_StateChanged;
			this.Activated += MainWindow_Activated;

			this.ContentRendered += MainWindow_ContentRendered;

			Softpower.SmartMaker.TopApp.SingleInstance.SingleInstanceApplication.NewInstanceMessage += new Softpower.SmartMaker.TopApp.SingleInstance.NewInstanceMessageEventHandler (SingleInstanceApplication_NewInstanceMessage);

			this.KeyDown += MainWindow_KeyDown;
			this.KeyUp += MainWindow_KeyUp;
			MainScrollViewer.ScrollChanged += MainScrollViewer_ScrollChanged;
		}

		private void MainScrollViewer_ScrollChanged (object sender, ScrollChangedEventArgs e)
		{
			if (MainCanvas?.CurrentDMTFrame is DMTFrame dmtFrame)
			{
				var document = dmtFrame.GetDocument () as DMTDoc;
				if (null != document && document.DocType == DOC_KIND._docWeb && null != dmtFrame.MotionManager)
				{
					dmtFrame.MotionManager.ScrollViewer_ScrollChanged (sender, e);
				}
			}
		}

		void MainWindow_PreviewMouseMove (object sender, MouseEventArgs e)
		{

		}

		#endregion

		#region |  void InitializeChildrenEvents()  |

		private void InitializeChildrenEvents ()
		{
			//상단 툴바에 대한 이벤트
			MainToolBarPanel.OnToolBarItemClickedEvent += MainToolBarPanel_OnToolBarItemClickedEvent;
			MainToolBarPanel.CommonEvent += MainToolBarPanel_CommonEvent;

			//하단 상태바에 대한 이벤트
			MainStatusBar.OnNotifyScaleValueEvent += MainStatusBar_OnNotifyScaleValueEvent;
			MainStatusBar.OnGetPhysicalScaleValueEvent += MainStatusBar_OnGetPhysicalScaleValueEvent;

			//중앙 작업캔버스에 대한 이벤트
			//MainCanvas.ShowAttPageEvent += MainCanvas_ShowAttPageEvent;
			MainCanvas.ShowAtomContextMenuEvent += MainCanvas_ShowAtomContextMenuEvent;
			MainCanvas.OnNotifyAppAtomMenuEnableEvent += MainCanvas_NotifyAppAtomMenuEnableEvent;
			MainCanvas.OnNotifyRemoveDMTFrameEvent += MainCanvas_NotifyRemoveDMTFrameEvent;
			MainCanvas.OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent += MainCanvas_OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent;
			MainCanvas.OnNotifyToolBarAboutCurrentSelectedViewPropertiesEvent += MainCanvas_OnNotifyToolBarAboutCurrentSelectedViewPropertiesEvent;

			MainCanvas.OnNotifyToolBarLockCurrentSelectedAtomEvent += MainCanvas_OnNotifyToolBarLockCurrentSelectedAtomEvent;
			MainCanvas.OnNotifyToolBarRunModeCurrentSelectedAtomEvent += MainCanvas_OnNotifyToolBarRunModeCurrentSelectedAtomEvent;
			MainCanvas.OnNotifyToolBarNoCurrentSelectedAtomEvent += MainCanvas_OnNotifyToolBarNoCurrentSelectedAtomEvent;
			MainCanvas.OnNotifyScaleOfFocusedScreenEvent += MainCanvas_OnNotifyScaleOfFocusedScreenEvent;
			MainCanvas.OnNotifyScaleOfFocusedScreenNoResizeEvent += MainCanvas_OnNotifyScaleOfFocusedScreenNoResizeEvent;
			MainCanvas.OnNotifyActiveCurrentDMTFrameEvent += MainCanvas_OnNotifyActiveCurrentDMTFrameEvent;
			MainCanvas.AtomDoubleClickedEvent += MainCanvas_AtomDoubleClickedEvent;
			MainCanvas.NotifyNewModelSave += MainCanvas_NotifyNewModelSave;
			MainCanvas.OnNotifyMainScrollChanged += MainCanvas_OnNotifyMainScrollChanged;

			GlobalEventReceiver.UniqueGlobalEventRecevier.OnNotifyGlobalEventOccured += HandleGlobalEvent;

			if (false != PQAppBase.IsEnableMakerStore)
			{
				LicenseEventReceiver.Instance.OnNotifyLicenseEventOccured += Instance_OnNotifyLicenseEventOccured;
			}
		}

		#endregion

		#region |  void InitMessage()  |

		private void InitMessage ()
		{

			_Message80.Init ();
		}

		#endregion

		#region |  void ApplyApplicationComponentStyle(StyleCategory.StyleThemeCategory StyleKey)  |

		private void ApplyApplicationComponentStyle (StyleCategory.StyleThemeCategory StyleKey)
		{
			ResourceDictionary FindedResource = null;

			switch (StyleKey)
			{
				case StyleCategory.StyleThemeCategory.Default:
					{
						FindedResource = StyleResourceManager.GetApplicationComponentStyle (StyleCategory.StyleComponentCategory.ScrollViewer);
						MainScrollViewer.Resources = FindedResource;

						ControlTemplate ApplyResource = FindedResource["DefaultScrollViewerControlTemplate"] as ControlTemplate;
						MainScrollViewer.Template = ApplyResource;

						break;
					}

				default: break;
			}
		}

		#endregion

		#region |  void SetVersionView()  |

		//private void SetVersionView()
		//{

		//	if (true == PQAppBase.IsSmartMakerVersion)
		//	{
		//		RootGrid.Background = new SolidColorBrush (System.Windows.Media.Color.FromArgb (255, 238, 245, 251));
		//	}
		//}

		#endregion


		#endregion

		#region |  void OnExecuteUpgrade()  |

		/// <summary>
		/// 업그레이드 동작은 ExecuteUpgrade 에서 처리
		/// </summary>
		private void OnExecuteUpgrade (bool bEnableCancel)
		{
			if (LicenseService.CheckRegistered (PQAppBase.CurrentProductCode))
			{
				//
				List<string> listNotice = UpgradeManager.Instance.GetUpgradeNotice ();

				if (0 < listNotice.Count)
				{
					UpgradeNoticeWindow upgradeNotice = new UpgradeNoticeWindow ();
					upgradeNotice.Notice = listNotice;
					upgradeNotice.Owner = this;
					upgradeNotice.EnableCancel = bEnableCancel;

					if (false != upgradeNotice.ShowDialog ())
					{
						_Registry.WriteValue ("Environment\\Upgrade", "IsShowUpgradeMsg", 0);  // 업그레이드 메시지박스 안띄우기, IsShowConfirmCustMessage

						bool bUpgrade = LicenseService.ExecuteUpgrade (RegistryCoreX.ProductEnvPath, false, false, true);
						if (false == bUpgrade)
						{
							_Message80.Show (LC.GS ("SmartOffice_MainWindow_1812_1"));
						}

						_Registry.WriteValue ("Environment\\Upgrade", "IsShowUpgradeMsg", 1);
					}
					else
					{
						MainStatusBar.UpgradeStatusNotifier.IsEnabled = true;
					}
				}
				else
				{
					bool bUpgrade = LicenseService.ExecuteUpgrade (RegistryCoreX.ProductEnvPath, false, false, true);
					if (false == bUpgrade)
					{
						_Message80.Show (LC.GS ("SmartOffice_MainWindow_1812_1"));
					}
				}
				//
			}
		}

		#endregion

		#region |  void OnShowLanguageTool()  |

		private void OnShowLanguageGenerator ()
		{
			//Softpower.SmartMaker.LanguageGenerator.LanguageGeneratorWizard wizard = new Softpower.SmartMaker.LanguageGenerator.LanguageGeneratorWizard ();
			//wizard.Owner = this;
			//wizard.ShowDialog ();
		}

		#endregion


		#region |  ----- Jetty 처리관련 -----  |
		#region |  void TransmitEmulator() : Jetty 서버 관련  |

		void TransmitEmulator ()
		{
			Dispatcher.Invoke (DispatcherPriority.Normal, new Action (delegate
			{
				TransmitDocInformation ();
			}));
		}

		#endregion

		#region |  void ExitJettyModule() : Jetty 서버 관련  |

		private void ExitJettyModule ()
		{
			//m_pJettyServer.ExitJettyModule();
		}

		#endregion
		#endregion//Jetty 처리관련

		#region |  ----- 라이선스 처리관련 -----  |
		#region |  void SetFreeLicense() : 라이선스 처리관련  |

		private void SetFreeLicense ()
		{
			PQAppBase.IsIOS = true;
			PQAppBase.IsWebContents = true;
			PQAppBase.IsAndroidApp = true;
			PQAppBase.IsWebApplication = true;
		}

		#endregion

		#region |  void SetLicenseInfo() : 라이선스 처리관련  |

		private void SetLicenseInfo ()
		{

			SetLicenseType80 ();
#if !DEBUG
                PQAppBase.UseSecurity = _Registry.ReadBool("ASSISTANCE", "UseSecurity", false);
                PQAppBase.SecurityUserName = LicenseService.GetCustomerName();
                PQAppBase.SecurityID = LicenseService.GetCustomerID();
                PQAppBase.SecurityPass = LicenseService.GetContentsPassword();
                PQAppBase.LicenseCompany = LicenseService.GetLicenseNumber();
#endif
		}

		#endregion

		#region |  void SetLicenseType80() : 라이선스 처리관련  |

		private void SetLicenseType80 ()
		{

			for (int i = 1; i < 7; i++)
			{
				bool bEnable = LicenseService.IsEnableServiceRange (i);
				SetLicenseRoll80 (i, bEnable);
			}
		}

		#endregion

		#region |  void SetLicenseRoll80(int nIndex, bool bEnable) : 라이선스 처리관련  |

		private void SetLicenseRoll80 (int nIndex, bool bEnable)
		{
			//
			switch (nIndex)
			{
				case 1: PQAppBase.IsWebContents = bEnable; break;
				case 2: PQAppBase.IsWebApplication = bEnable; break;
				case 3: PQAppBase.IsWindowApplication = bEnable; break;
				case 4: PQAppBase.IsAndroidApp = bEnable; break;
				case 5: PQAppBase.IsIOS = bEnable; break;
				case 6:
					PQAppBase.IsPremiumMarket = bEnable;
					break;
			}
		}

		#endregion

		#region |  void OnUnInstallLicense() : 라이선스 관련처리  |

		private void OnUnInstallLicense ()
		{
			if (MessageBoxResult.Yes == _Message80.Show (LC.GS ("SmartOffice_MainWindow_1808_1"), LC.GS ("SmartOffice_MainWindow_1808_2"), MessageBoxButton.YesNo, MessageBoxImage.Information))
			{
				bool bResult = LicenseService.UnistallLicense (PQAppBase.CurrentProductCode);
				if (false != bResult)
				{
					if (LC.PQLanguage == LC.LANG.KOREAN)
					{
						if (false == PQAppBase.IsEduTechMode)
						{
							// 190904_AHN - MessagePush 서비스체크
							// - 라이선스 해지 후 서비스 재시작, 레지스트리 초기화 (라이선스 정보 변경 되는 경우)
							MessagePushService messagePushService = new MessagePushService ();
							messagePushService.CheckMessagePushService (MessagePushService.RESTART_MODE, MessagePushService.SERVICE_INSTALL);
							messagePushService.Write_RegToBool (MessagePushService.REG_SUBKEY, MessagePushService.REG_KEY, false);
							//
						}
					}

					_Message80.Show (LC.GS ("SmartOffice_MainWindow_1808_3"), LC.GS ("SmartOffice_MainWindow_1808_4"), MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
		}

		#endregion

		#region |  void OnLicenseInfo() : 라이선스 관련처리  |

		private void OnLicenseInfo ()
		{
			LicenseInfoWindow liWindow = new LicenseInfoWindow ();

			liWindow.Owner = this;
			liWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			liWindow.OnUpdateLicenseType += liWindow_OnUpdateLicenseType;

			liWindow.ShowDialog ();
		}

		#endregion

		#region | void OnSetPushSound () : 푸시알림 관리자 | 

		private void OnSetPushSound ()
		{
			if (true == LicenseHelper.Instance.IsEnableSolutionService (SolutionService.PushMessage, true) ||
				true == PQAppBase.strTrial)
			{
				PushSoundManagerWindow window = new PushSoundManagerWindow ();
				window.Owner = this;
				window.Show ();
			}
		}

		#endregion

		#region |  void OnSetDeveloperKey () : API키 관리 |

		private void OnSetDeveloperKey ()
		{
			KeyManagerWindow keyManagerWindow = new KeyManagerWindow ();
			keyManagerWindow.SelectMode = false;
			keyManagerWindow.Owner = this;
			keyManagerWindow.Show ();
		}

		#endregion

		#region |  void OnRegisterLicense() : 라이선스 관련처리  |

		private void OnRegisterLicense ()
		{
			if (false == LicenseCheck ())
			{
				return;
			}

			SetLicenseInfo ();
		}

		#endregion
		#endregion//라이선스 처리관련

		#region |  bool PreventMainWindowPreviewKeyEvent()  |
		/// <summary>
		/// 메인 윈도우의 키 이벤트를 막는경우 체크
		/// </summary>
		/// <returns></returns>
		private bool PreventMainWindowPreviewKeyEvent ()
		{
			if (true == MainToolBarPanel.OfficeShortCutToolBar.AtomNameTextBox.IsFocused
			  //|| true == FontFamilyComboBoxControl.FontFamilyComboBox.ValueTextBox.IsFocused
			  || true == MainToolBarPanel.OfficeTextToolBar.FontSizeComboBox.ValueTextBox.IsFocused
			  || true == MainStatusBar.LocationStackPanel.IsFocused
			  || true == MainStatusBar.IsFocused ()
			  //|| true == Softpower.SmartMaker.TopWebAtom.Component.HtmlTagManager.FeManager.IsFocused
			  || (null != MainCanvas.CurrentDMTFrame && true == MainCanvas.CurrentDMTFrame.CurrentCaptionBar.IsCaptionPageUnitEditted ()))
			{
				return true;
			}

			BaseFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			if (null != CurrentDMTFrame)
			{
				TopView CurrentDMTView = CurrentDMTFrame.GetCurrentView ();
				if (null != CurrentDMTView)
				{
					bool bValue = CurrentDMTView.AtomEditMode ();
					return bValue;
				}
			}

			return false;
		}

		#endregion

		#region |  void AttPage_PreviewKeyDown(object sender, KeyEventArgs e)  |

		//AttPage를 띄웟을 경우 AttPage 내에서만 키이벤트를 제어한다.
		//private void AttPage_PreviewKeyDown (object sender, KeyEventArgs e)
		//{
		//	switch (e.Key)
		//	{
		//		case Key.Escape:
		//			AtomAttWindow.EndWithNoAnimation ();
		//			break;
		//		case Key.Enter:
		//			if (null != AtomAttWindow && AtomAttWindow.Visibility == Visibility.Visible
		//				&& false == AtomAttWindow.OutBoundComboBoxOpened)
		//			{
		//				AtomAttWindow.EndAnimation ();
		//			}
		//			break;
		//	}
		//}

		#endregion

		private void FrameTogglePopup_Closed (object sender, EventArgs e)
		{
			_FrameTogglePopup = new FrameTogglePopup ();
		}

		#region |  void OnAtomPaste()  |

		private void OnAtomPaste ()
		{
			BaseFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			if (null != CurrentDMTFrame)
			{
				TopView CurrentDMTView = CurrentDMTFrame.GetCurrentView ();
				TopDoc CurrentDocument = CurrentDMTView.GetDocument ();

				CurrentDocument.ProcessDeepCloneAtoms (1);
				CurrentDocument.SetAllAtomsAdornerVisibilityInLightDMTView (Visibility.Collapsed, true);
			}
		}

		#endregion

		#region |  void OnAtomCopy()  |

		private void OnAtomCopy ()
		{

			//
			List<BaseFrame> frameCollecton = this.MainCanvas.GetAllFrame ();
			foreach (BaseFrame frame in frameCollecton)
			{
				TopDoc doc = frame.GetCurrentView ().GetDocument ();
				if (null != doc)
				{
					if (doc is DMTDoc)
					{
						DMTDoc dmtDoc = doc as DMTDoc;
						//dmtDoc.OffsetCount = 0;
					}
				}
			}
			//

			BaseFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			if (null != CurrentDMTFrame)
			{
				TopView CurrentDMTView = CurrentDMTFrame.GetCurrentView ();
				TopDoc CurrentDocument = CurrentDMTView.GetDocument ();

				CurrentDocument.ReadyDeepCloneAtoms ();
			}
		}

		#endregion


		private void OnDeleteAtom ()
		{
			BaseFrame currentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

			if (null != currentFrame)
			{
				TopView currentView = currentFrame.GetCurrentView ();
				if (null != currentView)
				{
					currentView.DeleteCurrentSelectedAtoms ();
				}
			}
		}

		#region |  void OnAtomPaste()  |

		private void OnAtomCut ()
		{
			OnAtomCopy ();

			BaseFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			if (null != CurrentDMTFrame)
			{
				TopView CurrentDMTView = CurrentDMTFrame.GetCurrentView ();
				CurrentDMTView.DeleteCurrentSelectedAtoms ();
			}
		}

		#endregion

		#region |  void CloseAllPopup()  |
		//현재 동적으로 열린 모든 팝업을 닫아주는 메소드
		private void CloseAllPopup ()
		{
			MainMenuPopupPanel.Visibility = Visibility.Collapsed;
			MainToolBarPanel.CloseAllToolBarPopup ();

			FigureAtomPopupMenu.Unit.IsOpen = false;
			SpeechBalloonPopupMenu.Unit.IsOpen = false;
			QuizViewAtomPopupMenu.Unit.IsOpen = false;

			BaseFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			if (null != CurrentDMTFrame)
			{
				FormMode.FrameMode CurrentFrameMode = CurrentDMTFrame.GetFormMode ();

				if (FormMode.FrameMode.Executed != CurrentFrameMode)
				{
					MainAtomContextMenuOpen (false);
					MainAtomContextMenuPanel.Child = null;
				}
			}
		}

		private void CloseAtomAttWindow ()
		{
			if (null != AtomAttWindow && AtomAttWindow.Visibility == Visibility.Visible
				&& false == AtomAttWindow.OutBoundComboBoxOpened)
			{
				AtomAttWindow.EndAnimation ();
			}
		}

		#endregion

		#region |  void OnShowAttPageInGridTableAtom(object objAtom, string strContextMenu) |

		private void OnShowAttPageInGridTableAtom (object objAtom, string strContextMenu)
		{
			SmartAtomAttCore attCore = SmartAtomManager.ShowAttPage (objAtom, false, false);
			MainCanvas_ShowAttPageEvent (attCore, objAtom, strContextMenu);
		}

		private void OnShowAtomAttPage (object objAtom, bool IsWeb, bool IsEBook, string strContextMenu)
		{
			SmartAtomAttCore attCore = SmartAtomManager.ShowAttPage (objAtom, IsWeb, IsEBook);
			MainCanvas_ShowAttPageEvent (attCore, objAtom, strContextMenu);
		}

		#endregion

		#region |  void RelocationAtomContextMenu(Point currentMousePoint)  |

		private void RelocationAtomContextMenu (Point currentMousePoint)
		{

			if (ActualWidth < currentMousePoint.X + MainAtomContextMenuPanel.ActualWidth)
			{
				currentMousePoint.X = ActualWidth - MainAtomContextMenuPanel.ActualWidth - 5;
			}

			if (ActualHeight < currentMousePoint.Y + MainAtomContextMenuPanel.ActualHeight)
			{
				currentMousePoint.Y = ActualHeight - MainAtomContextMenuPanel.ActualHeight - 5;
			}
			MainAtomContextMenuPanel.Margin = new Thickness (currentMousePoint.X, currentMousePoint.Y, 0, 0);
		}

		#endregion

		#region |  bool IsWillParentCenterPosition(UserControl ucSubControl, int nAllowGap) : 호출하는 곳 없음  |

		private bool IsWillParentCenterPosition (UserControl ucSubControl, int nAllowGap)
		{

			Point currentPoint = Mouse.GetPosition (this);
			double nAttPageWidth = ucSubControl.Width;
			double nAttPageHeight = (true == Double.IsNaN (ucSubControl.ActualHeight)) ? 300 : ucSubControl.ActualHeight;

			if (this.Width - nAllowGap < currentPoint.X + nAttPageWidth || this.Height - nAllowGap < currentPoint.Y + nAttPageHeight) { return true; }
			else { return false; }
		}

		#endregion

		#region |  ArrayList OnLoadModelFromInnerLogic(string strFilePath)  |
		/// <summary>
		/// 2015-07-07 JAEYOUNG 
		/// DocMenu 는 TopDoc에 상속되있는 폼이 아니기 때문에 DocMenu는 따로 폼을 로드시킨다.
		/// </summary>
		/// <param name="strFilePath">로드하려는 폼의 경로</param>
		/// <returns></returns>
		private ArrayList OnLoadModelFromInnerLogic (string strFilePath, int nBookPage, bool DoModal)
		{
			System.IO.FileInfo info = new System.IO.FileInfo (strFilePath);
			if (false == info.Exists)
			{
				string strMessage = String.Concat (strFilePath, LC.GS ("TopLight_JobInfo_1"));
				_Message80.Show (strMessage);
				return null;
			}

			DOC_KIND nDocKind = ExtensionInfo.GetDocKindFromFileName (strFilePath);

			ArrayList alData = null;

			if (DOC_KIND._docMenu == nDocKind)
			{
				CTopMenuDoc pDoc = CreateMenuDocument ();

				pDoc.FilePath = strFilePath;

				pDoc.OnOpenDocument (strFilePath);

				//pDoc.SetOwner(this);
				pDoc.Show ();

				alData = new ArrayList ();
				alData.Add (pDoc);
			}
			else if (DOC_KIND._docNone != nDocKind)
			{
				MakeNewModel (nDocKind, nBookPage);
				SkinFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as SkinFrame;

				if (DOC_KIND._docEBook == nDocKind || DOC_KIND._docSlideMaster == nDocKind)
				{
					CurrentDMTFrame.OnLoadEBookModel (strFilePath, nBookPage);
					CurrentDMTFrame.CurrentCaptionBar.SetEBookCurrentPage (nBookPage);
				}
				else
				{
					CurrentDMTFrame.OnOpenDocument (strFilePath);
				}

				TopView CurrentView = CurrentDMTFrame.GetCurrentView ();
				LightJDoc CurrentDoc = CurrentView.GetDocument () as LightJDoc;

				alData = new ArrayList ();
				alData.Add (CurrentDoc);

				if (null != CurrentDoc && false == CurrentDoc.IsDynamicMode && false == CurrentDoc.IsEBookDoc) //메뉴 영역 설정
				{
					int nTopHeight = CurrentDoc.GetFrameAttrib ().TopBarHeight;
					int nBottomHeight = CurrentDoc.GetFrameAttrib ().BottomBarHeight;

					if (0 < nTopHeight || 0 < nBottomHeight)
					{
						BarHeightChange (nTopHeight, nBottomHeight);
					}
				}

				if (null != CurrentDoc && true == CurrentDoc.IsWebDoc)
				{
					if (null != CurrentDMTFrame)
					{
						DMTFrame dmtFrame = CurrentDMTFrame as DMTFrame;
						if (null != dmtFrame)
						{
							GlobalWaitThread.WaitThread.Start (dmtFrame);
							dmtFrame.MotionManager.FormLoad = true;
							dmtFrame.MotionManager.CheckAtomVisibility ();
							dmtFrame.LoadHtmlTagAtom ();
						}
					}
				}
			}

			return alData;
		}
		#endregion

		#region |  ArrayList OnLoadModelFromMakerStore (string strFilePath)  |
		/// <summary>
		/// MakerStore 에서 상세품 띄울때
		/// </summary>
		/// <param name="strFilePath">로드하려는 폼의 경로</param>
		/// <returns></returns>
		private ArrayList OnLoadModelFromMakerStore (string strFilePath)
		{
			ArrayList alData = null;

			if (null != m_SubsidiaryWindow)
			{
				TopDoc pDoc = m_SubsidiaryWindow.LoadModelFromMakerStore (strFilePath);
				if (null != pDoc)
				{
					alData = new ArrayList ();
					alData.Add (pDoc);
				}
			}

			return alData;
		}
		#endregion

		#region |  ArrayList OnLoadModelFromSimulator (string strFilePath)  |
		/// <summary>
		/// Simulator 에서 상세품 띄울때
		/// </summary>
		/// <param name="strFilePath">로드하려는 폼의 경로</param>
		/// <returns></returns>
		private ArrayList OnLoadModelFromSimulator (string strFilePath)
		{
			ArrayList alData = null;

			if (null != m_SimulatorWindow)
			{
				TopDoc pDoc = m_SimulatorWindow.LoadModelFromSimulator (strFilePath);
				if (null != pDoc)
				{
					alData = new ArrayList ();
					alData.Add (pDoc);
				}
			}

			return alData;
		}
		#endregion

		#region |  ArrayList OnLoadTopMostModelFromSimulator (string strFilePath)  |
		/// <summary>
		/// Simulator 에서 최상위 폼 띄울때
		/// </summary>
		/// <param name="strFilePath">로드하려는 폼의 경로</param>
		/// <returns></returns>
		private ArrayList OnLoadTopMostModelFromSimulator (string strFilePath)
		{
			ArrayList alData = null;

			if (null != m_SimulatorWindow)
			{
				TopDoc pDoc = m_SimulatorWindow.LoadTopMostModelFromSimulator (strFilePath);
				if (null != pDoc)
				{
					alData = new ArrayList ();
					alData.Add (pDoc);
				}
			}

			return alData;
		}
		#endregion

		#region |  void ShowDMTView()  |

		private void ShowDMTView ()
		{

			//DMTView CurrentDMTView = MainCanvas.CurrentDMTFrame.CurrentDMTView;

			TopView CurrentDMTView = MainCanvas.CurrentDMTFrame.GetCurrentView ();
			CurrentDMTView.Visibility = Visibility.Visible;
		}

		#endregion

		#region |  void ProcessHistoryView(KeyValuePair<string, CommandAction> Parameter)  |

		private void ProcessHistoryView (KeyValuePair<string, CommandAction> Parameter)
		{

			//string strCommand = Parameter.Key;
			//CommandAction CommandType = Parameter.Value;

			//if (CommandAction.Undo == CommandType)
			//{
			//	int nCountOfHistory = TempHistoryListBox.Items.Count;

			//	if (0 < nCountOfHistory)
			//	{
			//		TempHistoryListBox.Items.RemoveAt (nCountOfHistory - 1);
			//	}
			//}
			//else
			//{
			//	TempHistoryListBox.Items.Add (strCommand);
			//}
		}

		#endregion

		#region |  void ExecuteShortCut(int nShortCutType)  |

		private void ExecuteShortCut (int nShortCutType)
		{
			switch (nShortCutType)
			{
				case 0:
					{
						OnShowProcessManager ();
					}
					break;
				case 1:
					{
						OnShowDBManagerDialog ();
					}
					break;
				case 2:
					{
						OnShowFormScriptEditDialog80 ();
					}
					break;
				case 3:
					{
						SwapVisibilityOfDesignHelper ();
					}
					break;
				case 4:
					{
						// OnAutoFormsErdCreator();
						OnAutoTableErdCreator (); // 메뉴얼 정비작업으로 기능변경 (2021-05-20)
					}
					break;
				case 5:
					{

					}
					break;
				case 6: //복사간격조정
					{
						m_MainWindowManager.ShowDialog (FlexibleDialogDefine.FlexibleDialogType.IntervalInformation, true, null);
					}
					break;
				case 7: //찾아바꾸기
					{
						OnShowFindAndReplaceDialog ();
					}
					break;
				case 8: //아톰명보기
					{
						ChangeAtomNameTextVisible ();
					}
					break;
				case 9:
					{
						OnShowVideoPlay ();
					}
					break;
				case 10:
					{
						OnShowAnimationManager ();
					}
					break;
				case 11:
					{
						OnShowAtomEditManager ();
					}
					break;
				case 12:
					{
						OnShowActionManager ();
					}
					break;
				case 13:
					{
						OnShowSimulator ();
					}
					break;
				case 14:
					{
						OnShowNavigationGraph ();
					}
					break;
				case 15:
					{
						OnShowGoogleFontWindow ();
					}
					break;
				case 16:
					{
						OnShowPageMasterWindow ();
					}
					break;
				default: break;
			}
		}

		#endregion

		#region |  void ExecuteArduinoShortCut(int nShortCutType)  |

		private void ExecuteArduinoShortCut (int nShortCutType)
		{
			switch (nShortCutType)
			{
				case 0:     // 원격제어   ArduinoRemote
					break;
				case 1:     // 편집기     ArduinoScript
					OnShowFormScriptEditDialog80 ();
					break;
				case 2:     // 제어관리자 ArduinoFlow
					OnShowArduinoManager ();
					break;
				case 3:     // 앱전환	  ArduinoApp
					break;
				case 4:     // 컴파일	  ArduinoCompile
					break;
				case 5:     // 업로드	  ArduinoUpload
					break;
			}
		}

		#endregion

		// 원격제어   ArduinoRemote
		private void OnSetArduinoRemote ()
		{
		}

		// 앱전환	  ArduinoApp
		private void OnSetArduinoApp ()
		{
		}

		// 컴파일	  ArduinoCompile
		private void OnArduinoCompile ()
		{
			BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			TopDoc CurrentDoc = null == CurrentFrame.GetCurrentView () ? null : CurrentFrame.GetCurrentView ().GetDocument ();
			if (null != CurrentDoc)
			{
				Softpower.SmartMaker.TopEdit80.CScrFrame ScriptFrame = CurrentDoc.GetScriptWindowObject () as Softpower.SmartMaker.TopEdit80.CScrFrame;
				if (null != ScriptFrame)
				{
					ScriptFrame.StartCompile ();
				}
			}
		}

		// 업로드	  ArduinoUpload
		private void OnArduinoUpload ()
		{
		}


		#region |  void UpdateToolBarFontSize(double dFontSize)  |

		private void UpdateToolBarFontSize (double dFontSize)
		{
			EBookTextToolBar.Instance.SetFontSize (dFontSize);
			EBookTextTopToolBar.Unit.SetFontSize (dFontSize);

			MainToolBarPanel.UpdateFontSize (dFontSize);
		}

		#endregion

		#region |  void AdjustAtomZIndex(ChangeAtomZIndexDefine.ActionType ApplyType)  |

		private void AdjustAtomZIndex (ChangeAtomZIndexDefine.ActionType ApplyType)
		{

			MainCanvas.AdjustAtomZIndex (ApplyType);
		}

		#endregion

		#region |  ----- 하단 상태바 X,Y,Width,Height 업데이트 -----  |
		#region |  void UpdateCurrentLocationAndSize(KeyValuePair<LocationAndSizeDefine.LocationAndSizeType, double> ValueList)  |

		private void UpdateCurrentLocationAndSize (KeyValuePair<LocationAndSizeDefine.LocationAndSizeType, double> ValueList)
		{

			MainCanvas.UpdateCurrentLocationAndSize (ValueList);
		}

		#endregion

		#region |  void UpdateStatusBarAboutLocationAndSize(List<double> LocationAndSizeList)  |

		private void UpdateStatusBarAboutLocationAndSize (List<double> LocationAndSizeList)
		{

			MainStatusBar.UpdateLocationAndSize (LocationAndSizeList);
		}

		#endregion

		#region |  void UpdateStatusBarAboutLocation(Thickness NewMargin)  |

		private void UpdateStatusBarAboutLocation (Thickness NewMargin)
		{

			MainStatusBar.UpdateLocation (NewMargin);
		}

		#endregion
		#endregion//하단 상태바 X,Y,Width,Height 업데이트

		#region |  void SwapAtomExpandMenu(MainExpandMenuType SourceType) : 코드없음  |

		private void SwapAtomExpandMenu (MainExpandMenuType SourceType)
		{

			//switch (SourceType)
			//{
			//    case MainExpandMenuType.AppAtomMenu:
			//        {
			//            MainExpandMenuContainer.CurrentExpandMenuType = (int)MainExpandMenuType.AppAtomMenu;
			//            break;
			//        }
			//    case MainExpandMenuType.EbookAtomMenu:
			//        {
			//            MainExpandMenuContainer.CurrentExpandMenuType = (int)MainExpandMenuType.EbookAtomMenu;
			//            break;
			//        }
			//    case MainExpandMenuType.WebAtomMenu:
			//        {
			//            MainExpandMenuContainer.CurrentExpandMenuType = (int)MainExpandMenuType.WebAtomMenu;
			//            break;
			//        }
			//    case MainExpandMenuType.ScriptMenu:
			//        {
			//            MainExpandMenuContainer.CurrentExpandMenuType = (int)MainExpandMenuType.ScriptMenu;
			//            break;
			//        }
			//    case MainExpandMenuType.ExecuteMenu:
			//        {
			//            MainExpandMenuContainer.CurrentExpandMenuType = (int)MainExpandMenuType.ExecuteMenu;
			//            break;
			//        }
			//    default: break;
			//}
		}

		#endregion

		#region |  void AttemptApplyDesignHelperEvent(DictionaryEntry DictionaryParameter)  |

		private void AttemptApplyDesignHelperEvent (DictionaryEntry DictionaryParameter)
		{
			BaseFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			if (null != CurrentDMTFrame)
			{
				TopView CurrentView = CurrentDMTFrame.GetCurrentView ();
				if (null != CurrentView)
				{
					CurrentView.ApplyDesignHelperEvent (DictionaryParameter);
				}
			}
		}

		#endregion

		#region |  void ProcessMainMenuClickedEvent(string strValue)  |

		private void ProcessMainMenuClickedEvent (string strValue)
		{
			if (MainMenuDef.NewModelMenuItem == strValue)
			{
				m_MainWindowManager.ShowDialog (FlexibleDialogDefine.FlexibleDialogType.ModelSelectView, true, null);
			}
			else if (MainMenuDef.ServerConnectionMenuItem == strValue)
			{
				OnLogin (MainMenuDef.ServerConnectionMenuItem);
			}
			else if (MainMenuDef.ServerDisConnectionMenuItem == strValue)
			{
				OnLogin (MainMenuDef.ServerDisConnectionMenuItem);
			}
			else if (MainMenuDef.OpenModelMenuItem == strValue)
			{
				OnFileOpen ();
			}
			else if (MainMenuDef.SaveModelMenuItem == strValue)
			{
				OnFileSave ();
			}
			else if (MainMenuDef.SaveAsModelMenuItem == strValue)
			{
				OnFileSaveAs ();
			}
			else if (MainMenuDef.FinishJobMenuItem == strValue)
			{
				OnProgramExit ();
			}
			else if (MainMenuDef.SystemMenuItem == strValue)
			{
				m_MainWindowManager.ShowDialog (FlexibleDialogDefine.FlexibleDialogType.SystemInformation, false, null);
			}
			else if (MainMenuDef.RecentFileMenuItem == strValue)
			{

			}
			else if (MainMenuDef.DBFunctionMenuItem == strValue)
			{
				OnShowDBManagerDialog ();
			}
			else if (MainMenuDef.CreateERDMenuItem == strValue)
			{
				OnAutoErdCreator ();
			}
			else if (MainMenuDef.DBManagerMenuItem == strValue)
			{
				OnShowFieldSettingDialog ();
			}
			else if (MainMenuDef.QuizDBManagerMenuItem == strValue)
			{
				OnShowQuizDBManager ();
			}
			else if (MainMenuDef.HanaManagerMenuItem == strValue)
			{
				OnShowHanaFieldSettingDialog ();
			}
			else if (MainMenuDef.CreateTableMenuItem == strValue)
			{
				//OnAutoFormsErdCreator();
				OnAutoTableErdCreator (); // 메뉴얼 정비작업으로 기능변경 (2021-05-20)
			}
			else if (MainMenuDef.DesignHelperMenuItem == strValue)
			{
				SwapVisibilityOfDesignHelper ();
			}
			else if (MainMenuDef.CreatePackageMenuItem == strValue)
			{
				OnPacking80 ();
			}
			else if (MainMenuDef.CreatePackageAndroidMenuItem == strValue)
			{
				OnPacking (0);
			}
			else if (MainMenuDef.CreatePackageIOSMenuItem == strValue)
			{
				OnPacking (1);
			}
			else if (MainMenuDef.CreatePackageWindowsMenuItem == strValue)
			{
				OnPacking (2);
			}
			else if (MainMenuDef.MarketUploadMenuItem == strValue)
			{
				OnPublish (1);
			}
			else if (MainMenuDef.TestMarketUploadNativeAppMenuItem == strValue)
			{
				OnPublish (0);
			}
			else if (MainMenuDef.BusinessMarketUploadMenuItem == strValue)
			{
				OnPublish (2);
			}
			else if (MainMenuDef.MarketUploadListMenuItem == strValue)
			{
				OnMarketList ();
			}
			else if (MainMenuDef.LocalBusinessStoreMenuItem == strValue)
			{
				OnPublish (3);
			}
			else if (MainMenuDef.GoogleStoreMenuItem == strValue)
			{
				StoreUpload (0);
			}
			else if (MainMenuDef.AppleStoreMenuItem == strValue)
			{
				StoreUpload (1);
			}
			else if (MainMenuDef.TestMarketUploadWebSiteMenuItem == strValue)
			{
				OnServerDeploy (0);
			}
			else if (MainMenuDef.DeployDynamicWebMenuItem == strValue)
			{
				OnServerDeploy (0);
			}
			else if (MainMenuDef.DeployStaticWebMenuItem == strValue)
			{
				OnServerDeploy (1);
			}
			else if (MainMenuDef.DistributionWebServerMenuItem == strValue)
			{
				if (true == PQAppBase.IsEduTechMode) OnServerDeploy (1);
			}
			else if (MainMenuDef.DeployWebAppMenuItem == strValue)
			{
				OnServerDeploy (2);
			}
			else if (MainMenuDef.DeployReportMenuItem == strValue)
			{
				OnServerDeploy (3);
			}
			else if (MainMenuDef.DeployDirectWebServerMenuItem == strValue)
			{
				OnServerDeployDirect ();
			}
			else if (MainMenuDef.FlowManagerMenu == strValue)
			{
				OnShowProcessManager ();
			}
			else if (MainMenuDef.AtomEditManagerMenu == strValue)
			{
				OnShowAtomEditManager ();
			}
			else if (MainMenuDef.AnimationHelperMenuItem == strValue)
			{
				OnShowAnimationManager ();
			}
			else if (MainMenuDef.ArduinoFlowMenuItem == strValue)
			{
				OnShowArduinoManager ();
			}
			else if (MainMenuDef.PaintToolMenuItem == strValue)
			{
				OnShowPaintTool ();
			}
			else if (MainMenuDef.ScriptMenuItem == strValue)
			{
				OnShowFormScriptEditDialog80 ();
			}
			else if (MainMenuDef.OperatingEnvironmentMenuItem == strValue)
			{
				OnShowGlobalSettingWindow (0);
			}
			else if (MainMenuDef.DeviceInformationItem == strValue)
			{
				m_MainWindowManager.ShowDialog (FlexibleDialogDefine.FlexibleDialogType.DeviceInformation, true, null);
			}
			else if (MainMenuDef.TTSSettingItem == strValue)
			{
				OnShowTTSSettingDialog ();
			}
			else if (MainMenuDef.OpenAPIMenuItem == strValue)
			{
				OnShowStructDataMgrDialog (0);
			}
			else if (MainMenuDef.ExternalFunctionMenuItem == strValue)
			{

			}
			else if (MainMenuDef.ExternalFunctionRestMenuItem == strValue)
			{
				OnShowStructDataMgrDialog (1);
			}
			else if (MainMenuDef.ExternalFunctionSoapMenuItem == strValue)
			{
				OnShowStructDataMgrDialog (2);
			}
			else if (MainMenuDef.ExternalFunctionSapMenuItem == strValue)
			{
				OnShowStructDataMgrDialog (3);
			}
			else if (MainMenuDef.ExternalFunctionODataMenuItem == strValue)
			{
				OnShowStructDataMgrDialog (4);
			}
			else if (MainMenuDef.FindAndReplaceMenuItem == strValue)
			{
				OnShowFindAndReplaceDialog ();
			}
			else if (MainMenuDef.ContentSecurityMenuItem == strValue)
			{
				OnShowContentSecurityDialog ();
			}
			else if (MainMenuDef.ElearningMenuItem == strValue)
			{
				OnShowVideoPlay ();
			}
			else if (MainMenuDef.QnAMenuItem == strValue)
			{
				OnGoQnACafe ();
			}
			else if (MainMenuDef.SecurityContentsMenuItem == strValue)
			{
				OnShowSecurityDialog ();
			}
			else if (MainMenuDef.UpgradeMenuItem == strValue)
			{
				OnExecuteUpgrade (true);
			}
			else if (MainMenuDef.PrintPageInformationItem == strValue)
			{
				SetPageEnvironment (4);
			}
			else if (MainMenuDef.LanguageGeneratorItem == strValue)
			{
				OnShowLanguageGenerator ();
			}
			else if (MainMenuDef.CompanyInformationMenuItem == strValue)
			{
				OnShowSectorManager ();
			}
			else if (MainMenuDef.DepartmentInformationMenuItem == strValue)
			{
				OnShowDepartmentManager ();
			}
			else if (MainMenuDef.UserInformationMenuItem == strValue)
			{
				OnShowUserManager ();
			}
			else if (MainMenuDef.AcademyMenuItem == strValue)
			{
				OnShowAcademyManager ();
			}
			else if (MainMenuDef.RegisterLicenseMenuItem == strValue)
			{
				OnRegisterLicense ();
			}
			else if (MainMenuDef.LicenseInfoMenuItem == strValue)
			{
				OnLicenseInfo ();
			}
			else if (MainMenuDef.PushSoundKeyItem == strValue)
			{
				OnSetPushSound ();
			}
			else if (MainMenuDef.DeveloperKeyItem == strValue)
			{
				OnSetDeveloperKey ();
			}
			else if (MainMenuDef.UnInstalllLicenseMenuItem == strValue)
			{
				OnUnInstallLicense ();
			}
			else if (MainMenuDef.DeleteMenuItem == strValue)
			{
				OnDeleteAtom ();
			}
			else if (MainMenuDef.CutMenuItem == strValue)
			{
				OnAtomCut ();
			}
			else if (MainMenuDef.CopyMenuItem == strValue)
			{
				OnAtomCopy ();
			}
			else if (MainMenuDef.PasteMenuItem == strValue)
			{
				OnAtomPaste ();
			}
			else if (MainMenuDef.ServerInstallMenuItem == strValue)
			{
				OnServerInstall ();
			}
			else if (MainMenuDef.SubscriptionContractMenuItem == strValue)
			{
				OnSubscriptionContract ();
			}
			else if (MainMenuDef.SubscriptionRenewMenuItem == strValue)
			{
				OnSubscriptionRenew ();
			}
			else if (MainMenuDef.ActionManagerMenuItem == strValue)
			{
				OnShowActionManager ();
			}
			else if (MainMenuDef.AIAdaptorMenuItem == strValue)
			{
				OnShowAIAdaptor ();
			}
			else if (MainMenuDef.AIAdaptorDBManagerMenuItem == strValue)
			{
				OnShowAIAdaptorDBManager ();
			}
			else if (MainMenuDef.SAPFunctionMenuItem == strValue)
			{
				OnShowSAPFunctionNavigation ();
			}
			else if (MainMenuDef.SAPODataMenuItem == strValue)
			{
				OnShowSAPODataNavigation ();
			}
			else if (MainMenuDef.ServerTraceMenuItem == strValue)
			{
				OnShowServerTrace ();
			}
			else if (MainMenuDef.SQLTraceMenuItem == strValue)
			{
				OnShowSQLTrace ();
			}
			else if (MainMenuDef.HttpTraceMenuItem == strValue)
			{
				OnShowHttpTrace ();
			}
			else if (MainMenuDef.ScriptTraceMenuItem == strValue)
			{
				OnShowScriptTrace ();
			}
			else if (MainMenuDef.LRSTraceMenuItem == strValue)
			{
				OnShowLRSTrace ();
			}
			else if (MainMenuDef.ProjectMetadataMenuItem == strValue)
			{
				OnShowProjectMetadataDialog ();
			}
			else if (MainMenuDef.FrameAttribMenuItem == strValue)
			{
				OnShowFrameAttribDialog ();
			}
			else if (MainMenuDef.EBookQuestionsOptionMenuItem == strValue)
			{
				OnShowEBookQuestionsOptionDialog ();
			}
			else if (MainMenuDef.EduTechPDFMenuItem == strValue)
			{
				OnShowPDFViewerDialog ();
			}
			else if (MainMenuDef.EduTechPPTMenuItem == strValue)
			{
				ConvertOfficeDialog ();
			}
			else if (MainMenuDef.EduTechMultiConvertMenuItem == strValue)
			{
				ConvertMultiOfficeDialog ();
			}
			else if (MainMenuDef.TopProjectManager == strValue)
			{
				ShowEdutechProjectManager ();
			}
			else if (MainMenuDef.TopUserManager == strValue)
			{
				ShowEdutechUserManager ();
			}
			else if (MainMenuDef.CascadeMenuItem == strValue)
			{
				WindowManagerLayout (0);
			}
			else if (MainMenuDef.MinimizedMenuItem == strValue)
			{
				WindowManagerLayout (1);
			}
			else if (MainMenuDef.CloseAllMenuItem == strValue)
			{
				WindowManagerLayout (2);
			}
			else if (MainMenuDef.ExceptionMenuItem == strValue)
			{
				WindowManagerLayout (3);
			}
			else if (MainMenuDef.QuizCMSItem1 == strValue)
			{
				ShowPrintExaminationWindow ();
			}
			else if (MainMenuDef.QuizCMSItem2 == strValue)
			{
				ShowWebExaminationWindow ();
			}
			else if (MainMenuDef.QuizCMSItem3 == strValue)
			{
				ShowStudyExaminationWindow ();
			}
			else if (MainMenuDef.QuizCMSItem4 == strValue)
			{
				ShowAppWebServiceWindow ();
			}
			else if (MainMenuDef.QuizCMSItem5 == strValue)
			{
				ShowCMSListWindow ();
			}
		}

		#endregion

		#region |  void OnShowTTSSettingDialog()  |

		private void OnShowTTSSettingDialog ()
		{
			if (false != TTSHelper.Instance.CheckTTS ())
			{
				TTSSettingWindow tsWindow = new TTSSettingWindow ();
				tsWindow.Owner = this;
				tsWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
				tsWindow.ShowDialog ();
			}
		}

		#endregion

		#region |  void OnShowSecurityDialog()  |

		private void OnShowSecurityDialog ()
		{
			TopDoc pDoc = this.GetActiveDocument () as TopDoc;
			CKnowledgeBank DocKnowledgeBank = pDoc.GetKnowledgeBankObject () as CKnowledgeBank;

			if (null == DocKnowledgeBank)
				return;

			ArrayList SecurityParamList = new ArrayList ();
			SecurityParamList.Add (DocKnowledgeBank);

			m_MainWindowManager.ShowDialog (FlexibleDialogDefine.FlexibleDialogType.SecurityDialog, false, SecurityParamList);
		}

		#endregion

		#region |  void MakeNewModel(DOC_KIND docKind)  |

		private void MakeNewModel (DOC_KIND docKind)
		{
			MakeNewModel (docKind, false, 1);
		}

		private void MakeNewModel (DOC_KIND docKind, int nBookPage)
		{
			MakeNewModel (docKind, false, nBookPage);
		}

		private void MakeNewModel (DOC_KIND docKind, bool bIsExtendedPageModel)
		{
			MakeNewModel (docKind, bIsExtendedPageModel, 1);
		}

		#endregion

		#region |  void MakeNewModel(DOC_KIND docKind, bool bIsExtendedPageModel)  |

		private void MakeNewModel (DOC_KIND docKind, bool bIsExtendedPageModel, int nBookPage)
		{
			if (0 == MainCanvas.GetAllFrame ().Count)
			{
				Storyboard MainAppAtomMenuBackOutStart = AnimationManager.GetAnimationByName ("AtomMenuContainerBackOutStart");
				Storyboard MainScrollViewerBackOutStart = AnimationManager.GetAnimationByName ("MainScrollViewerBackOutStart");

				MainAppAtomMenuBackOutStart.Begin (MainExpandMenuContainer);
				MainScrollViewerBackOutStart.Begin (MainScrollViewer);
			}

			switch (docKind)
			{
				case DOC_KIND._docProcess:
				case DOC_KIND._docSmart:
				case DOC_KIND._docWeb:
				case DOC_KIND._docPCWeb:
				case DOC_KIND._docEBook:
				case DOC_KIND._docEBookPage:
				case DOC_KIND._docSlideMaster:
				case DOC_KIND._docQuizMaker:
					{
						MainCanvas.MakeNewModel (docKind, bIsExtendedPageModel, nBookPage);
						UpdateMenuStateAll (MainCanvas.CurrentDMTFrame);
					}
					break;
				case DOC_KIND._docMenu:
					{
						OnNewMenuCreate ();
					}
					break;
				case DOC_KIND._docScript: // 전역스크립트 생성
					{
						OnNewGlobalScriptCreate ();
					}
					break;
				case DOC_KIND._docService: // 백그라운드 서비스 스크립트 생성
					{
						OnNewServiceScriptCreate ();
					}
					break;
				case DOC_KIND._docReport:
					{
						OnNewReportCreate (docKind);
					}
					break;
				case DOC_KIND._docArduino:
				case DOC_KIND._docArduinoS:
					{
						OnNewArduinoEditCreate (docKind);
					}
					break;
				case DOC_KIND._docArduinoC:
					{
						OnNewArduinoCEditCreate ();
					}
					break;
				case DOC_KIND._docArduinoC_compile: // 2018.11.23 윤용상 : 제어관리자에서 컴파일된 소스를 C변환버튼클릭시 생성해주기위한 Create함수
					{
						OnCompleteArduinoEditCreate ();
					}
					break;
				case DOC_KIND._docQuizLayoutMaker:
					{
						OnCreateNewQuizLayoutMaker ();
					}
					break;
				case DOC_KIND._docQuizMultiLayoutMaker:
					{
						OnCreateNewQuizMultiLayoutMaker ();
					}
					break;
			}
		}

		#endregion

		#region |  void SwapVisibilityOfDesignHelper()  |

		private void SwapVisibilityOfDesignHelper ()
		{

			Visibility DesignHelperVisibility = MainDesignHelper.Visibility;

			switch (DesignHelperVisibility)
			{
				case Visibility.Visible:
					{
						MainDesignHelper.Visibility = Visibility.Collapsed;
						break;
					}

				case Visibility.Collapsed:
					{
						MainDesignHelper.Visibility = Visibility.Visible;
						break;
					}

				default: break;
			}
		}

		#endregion

		#region |  void OpenRecentFile(string strRecentFileFullPath) : 최근파일열기  |

		private void OpenRecentFile (string strRecentFileFullPath)
		{
			string strFileName = strRecentFileFullPath.Trim ();

			if (0 == strFileName.IndexOf ("DB:\\"))
			{
				FileOpen (strFileName);
			}
			else
			{
				if (true == new FileInfo (strFileName).Exists)
				{
					FileOpen (strFileName);
				}
			}
		}

		#endregion

		#region |  void RemoveRecentFile(string strRecentFileFullPath) : 최근파일항목제거  |

		private void RemoveRecentFile (string strRecentFileFullPath)
		{
			m_MainWindowManager.RemoveRecentFile (strRecentFileFullPath);
		}

		#endregion

		#region |  void FolderRecentFile(string strRecentFileFullPath) : 최근파일폴더열기 |

		private void FolderRecentFile (string strRecentFileFullPath)
		{
			m_MainWindowManager.FolderRecentFile (strRecentFileFullPath);
		}

		#endregion

		#region |  void OnNewMenuCreate()  |

		private void OnNewMenuCreate ()
		{
			CTopMenuDoc menuDoc = CreateMenuDocument ();
			menuDoc.Show ();
		}

		#endregion

		#region |  void OnNewGlobalScriptCreate()  |

		/// <summary>
		/// 전역스크립트 신규작성
		/// </summary>
		private void OnNewGlobalScriptCreate ()
		{
			CScrFrame scrFrame = CreateScrFrame ();


			scrFrame.NewFileSave += this.FileSaveEventHandler;
			scrFrame.ActivateWindow ();
			scrFrame.Show ();

		}

		#endregion

		#region |  void OnNewServiceScriptCreate()  |

		/// <summary>
		/// 서비스스크립트 신규작성
		/// </summary>
		private void OnNewServiceScriptCreate ()
		{
			CScrFrame scrFrame = CreateServiceFrame ();

			scrFrame.NewFileSave += this.FileSaveEventHandler;
			scrFrame.ActivateWindow ();
			scrFrame.Show ();

		}

		#endregion


		#region |  ----- 메뉴 작성 윈도우 -----  |
		#region |  CTopMenuDoc CreateMenuDocument()  |
		/// <summary>
		/// 메뉴 작성 윈도우 생성
		/// </summary>
		/// <returns></returns>
		private CTopMenuDoc CreateMenuDocument ()
		{
			if (0 == MainCanvas.GetAllFrame ().Count && 0 == ExecuteMenuManager.Instance.MenuCount)
			{
				Storyboard MainAppAtomMenuBackOutStart = AnimationManager.GetAnimationByName ("AtomMenuContainerBackOutStart");
				Storyboard MainScrollViewerBackOutStart = AnimationManager.GetAnimationByName ("MainScrollViewerBackOutStart");

				MainAppAtomMenuBackOutStart.Begin (MainExpandMenuContainer);
				MainScrollViewerBackOutStart.Begin (MainScrollViewer);
			}

			ExecuteMenuManager.Instance.CaptionSource = MainMenu.AppImage.Source as BitmapImage;
			//MainExpandMenuContainer.IsEnabled = true;

			CTopMenuDoc currentMenuDoc = ExecuteMenuManager.Instance.CreateMenuDocument (this);
			return currentMenuDoc;
		}

		#endregion

		#endregion//메뉴 작성 윈도우

		#region |  CScrDoc CreateScrDocument()  |

		private ScriptDoc CreateScrDocument ()
		{
			Softpower.SmartMaker.TopEdit80.CScrFrame scrFrame = CreateScrFrame ();
			return scrFrame.Document;
		}

		private ScriptDoc CreateServiceDocument ()
		{
			Softpower.SmartMaker.TopEdit80.CScrFrame scrFrame = CreateServiceFrame ();
			return scrFrame.Document;
		}

		#endregion

		#region |  Softpower.SmartMaker.TopEdit80.CScrFrame CreateScrFrame()  |

		private Softpower.SmartMaker.TopEdit80.CScrFrame CreateScrFrame ()
		{
			Softpower.SmartMaker.TopEdit80.CScrFrame scrFrm = new Softpower.SmartMaker.TopEdit80.CScrFrame ();

			scrFrm.CreateFrame (this);
			scrFrm.ShowEditWindow (null, "", true);
			scrFrm.Activated += ScriptFrame_Activated;

			return scrFrm;
		}

		private Softpower.SmartMaker.TopEdit80.CScrFrame CreateServiceFrame ()
		{
			Softpower.SmartMaker.TopEdit80.CScrFrame scrFrm = new Softpower.SmartMaker.TopEdit80.CScrFrame ();

			scrFrm.CreateServiceFrame (this);
			scrFrm.ShowEditWindow (null, "", true);
			scrFrm.Activated += ScriptFrame_Activated;

			return scrFrm;
		}

		private void ScriptFrame_Activated (object sender, EventArgs e)
		{
			UpdateMenuStateAll (sender);
		}

		#endregion

		private void OnNewReportCreate (DOC_KIND docKind)
		{
			MainCanvas.MakeNewModel (docKind);
			UpdateMenuStateAll (MainCanvas.CurrentDMTFrame as Softpower.SmartMaker.TopReportLight.View.ReportLightFrame);
		}

		#region |  Arduino  |

		private void OnNewArduinoEditCreate (DOC_KIND docKind)
		{
			MainCanvas.MakeNewModel (docKind);
			UpdateMenuStateAll (MainCanvas.CurrentDMTFrame as ArduinoFrame);
		}

		private void OnNewArduinoCEditCreate ()
		{
			CreateArduinoEditDocument ();
		}

		private void OnCreateNewQuizLayoutMaker ()
		{
			MainCanvas.MakeNewModel (DOC_KIND._docQuizLayoutMaker);
		}

		private void OnCreateNewQuizMultiLayoutMaker ()
		{
			MainCanvas.MakeNewModel (DOC_KIND._docQuizMultiLayoutMaker);
		}

		private Softpower.SmartMaker.ArduinoEdit.ViewModels.ArduinoEditDoc CreateArduinoEditDocument ()
		{
			Softpower.SmartMaker.ArduinoEdit.Views.ArduinoEditFrame arduionFrame = CreateArduinoEditFrame ();

			arduionFrame.Document.DocType = DOC_KIND._docArduinoC;

			return arduionFrame.Document;
		}

		private Softpower.SmartMaker.ArduinoEdit.Views.ArduinoEditFrame CreateArduinoEditFrame ()
		{
			Softpower.SmartMaker.ArduinoEdit.Views.ArduinoEditFrame arduionFrame = new Softpower.SmartMaker.ArduinoEdit.Views.ArduinoEditFrame ();
			arduionFrame.CreateFrame (this);
			arduionFrame.ShowEditWindow (null, "", true);

			arduionFrame.ActivateWindow ();
			arduionFrame.Show ();

			return arduionFrame;
		}

		// 2018.11.23 윤용상 : 제어관리자 C변환버튼 클릭시 GlobalEventReceiver를 통해 C언어창을 열고 컴파일된 c언어 문자열을 load해준다.
		private void OnCompleteArduinoEditCreate ()
		{
			CompleteArduinoEditDocument ();
		}

		private Softpower.SmartMaker.ArduinoEdit.ViewModels.ArduinoEditDoc CompleteArduinoEditDocument ()
		{
			Softpower.SmartMaker.ArduinoEdit.Views.ArduinoEditFrame arduionFrame = CompleteArduinoEditFrame ();

			//arduionFrame.Document.DocType = DOC_KIND._docArduinoC_compile;

			return arduionFrame.Document;
		}

		private Softpower.SmartMaker.ArduinoEdit.Views.ArduinoEditFrame CompleteArduinoEditFrame ()
		{
			Softpower.SmartMaker.ArduinoEdit.Views.ArduinoEditFrame arduionFrame = new Softpower.SmartMaker.ArduinoEdit.Views.ArduinoEditFrame ();
			arduionFrame.CreateFrame (this);
			arduionFrame.ShowEditWindow (null, "", true);

			arduionFrame.ActivateWindow ();
			arduionFrame.Show ();

			arduionFrame.OnLoadC_sourceText ();

			return arduionFrame;
		}


		#endregion
		#region |  void OnFileOpen()  |
		/// <summary>
		/// 모델열기
		/// </summary>
		private void OnFileOpen ()
		{
			GlobalWaitThread.WaitThread.Start (this);

			Softpower.SmartMaker.FileDBIO80.FileDBWindow fileDialog = new Softpower.SmartMaker.FileDBIO80.FileDBWindow (PROG_KIND._SMT);
			fileDialog.MultiSelect = true;
			fileDialog.Owner = this;
			fileDialog.ShowDialog ();

			if (true == fileDialog.DialogResult)
			{
				GlobalWaitThread.WaitThread.Start (this);

				ArrayList listOpenFiles = fileDialog.MultiSelectList ();

				foreach (object bof in listOpenFiles)
				{
					string strFileName = string.Empty;

					if (true == fileDialog.IsDBFile) //DB파일이면.
					{
						Softpower.SmartMaker.FileDBIO80.FileIOInfo dbFileInfo = bof as Softpower.SmartMaker.FileDBIO80.FileIOInfo;
						Softpower.SmartMaker.FileDBIO80.DBFileItem bDBfile = dbFileInfo.dbFileItem;

						DOC_KIND bDocKind = ExtensionInfo.GetDocKindFromProperty (bDBfile.DBProperty);//파일의 종류를 가져옴.

						strFileName = bDBfile.Name;
						DBFileOpen (bDocKind, strFileName, bDBfile.RegNumber, bDBfile.DBProperty, false);
						strFileName = string.Concat (fileDialog.FolderPath, "\\", strFileName);
						string strProperty = ExtensionInfo.GetExtFromProperty (bDBfile.DBProperty);
						strFileName = string.Concat (strFileName, ".", strProperty);
					}
					else if (true == fileDialog.IsAwsS3File) // AwsS3 파일이면.
					{
						strFileName = bof.ToString ();

						AwsS3FileOpen (strFileName);
					}
					else
					{
						strFileName = bof.ToString ();
						FileOpen (strFileName);
					}

					m_MainWindowManager.AddRecentFile (strFileName);
				}

				listOpenFiles.Clear ();

				GlobalWaitThread.WaitThread.End ();
			}
		}

		#endregion

		#region |  void OnFileOpen()  |
		/// <summary>
		/// 모델열기
		/// </summary>
		private void OnQuizMakerFileOpen ()
		{
			GlobalWaitThread.WaitThread.Start (this);

			Softpower.SmartMaker.FileDBIO80.FileDBWindow fileDialog = new Softpower.SmartMaker.FileDBIO80.FileDBWindow (PROG_KIND._SMT);
			fileDialog.MultiSelect = true;
			fileDialog.Owner = this;
			fileDialog.ShowDialog ();

			if (true == fileDialog.DialogResult)
			{
				GlobalWaitThread.WaitThread.Start (this);

				ArrayList listOpenFiles = fileDialog.MultiSelectList ();

				if (0 < listOpenFiles.Count)
				{
					var filePath = listOpenFiles[0]?.ToString ();
					if (File.Exists (filePath) && -1 < filePath.IndexOf (".QLM", StringComparison.OrdinalIgnoreCase))
					{
						if (MainCanvas.CurrentDMTFrame is QuizMakerFrame quizMakerFrame)
						{
							quizMakerFrame.ClearDocumentData ();
							quizMakerFrame.OnOpenDocument (filePath);
						}
					}
				}

				GlobalWaitThread.WaitThread.End ();
			}
		}

		#endregion

		#region |  void DBFileOpen(DOC_KIND docKind, string strFileName, string strRegNumber, string strProperty, bool bModelBank)  |

		private void DBFileOpen (DOC_KIND docKind, string strFileName, string strRegNumber, string strProperty, bool bModelBank)
		{
			if (DOC_KIND._docReport == docKind && false == CheckExistenceOfPrinter ())
			{
				return;
			}

			m_strProperty = strProperty;

			TopDoc pDoc = null;
			MakeNewModel (docKind);

			if (DOC_KIND._docMenu == docKind)
			{
				pDoc = ExecuteMenuManager.Instance.GetActiveDocument ();
			}
			else
			{
				pDoc = this.GetActiveDocument () as TopDoc;

				if (null == pDoc)
				{
					Softpower.SmartMaker.Common.Exception._Exception.Throw (LC.GS ("SmartOffice_MainWindow_8"));
				}

				if (DOC_KIND._docProcess == docKind || DOC_KIND._docSmart == docKind)
				{
					if (pDoc is DMTDoc)
					{
						((DMTDoc)pDoc).SetGlobalPtr (this.JobGInfo);
					}
				}
			}

			if (null != pDoc)
			{
				if (_Kiss.Equals (strProperty, "I"))
				{
					pDoc.OnDBOpenDocument (strFileName, strRegNumber, bModelBank, strProperty);
				}
				else
				{
					pDoc.OnDBOpenDocument (strFileName, strRegNumber, bModelBank);
				}

				pDoc.DBRegNumber = strRegNumber;
				pDoc.FilePath = bModelBank ? "" : strFileName; //LHS 
															   //pDoc.GetThisForm().Text = pDoc.GetFileName();

				//80
				//if (DOC_KIND._docScript == docKind)
				//{
				//    ScrFrame scrFrame = (CScrFrame)pDoc.GetThisForm();
				//    scrFrame.ShowEditWindow(null, "", true);
				//    scrFrame.NewFileSave += new ProcessQ.TopEdit.FileSaveEventHandlerX(this.FileSaveEventHandler);
				//}
				//else if (DOC_KIND._docSmart == docKind && false != bModelBank)
				//{
				//    CDMTFrame dmtFrame = (CDMTFrame)pDoc.GetThisForm();
				//}

				pDoc.Show ();
			}

		}

		#endregion

		#region |  bool FileOpen(string strFilePath)  |

		public bool FileOpen (string strFilePath, bool isTemplateMode = false)
		{
			bool bResult = false;

			DOC_KIND nDocKind = ExtensionInfo.GetDocKindFromFileName (strFilePath);

			if (nDocKind == DOC_KIND._docMenu)
			{
				bResult = FileOpenMenuDoc (strFilePath);
			}
			else if (nDocKind == DOC_KIND._docScript)
			{
				bResult = FileOpenScript (strFilePath);
			}
			else if (nDocKind == DOC_KIND._docService)
			{
				bResult = FileOpenServiceScript (strFilePath);
			}
			else if (nDocKind == DOC_KIND._docReport)
			{
				bResult = FileOpenReport (nDocKind, strFilePath, isTemplateMode);
			}
			else if (nDocKind == DOC_KIND._docQuizLayoutMaker)
			{
				bResult = FileOpenQuizLayoutModel (nDocKind, strFilePath, isTemplateMode);
			}
			else if (nDocKind == DOC_KIND._docQuizMultiLayoutMaker)
			{
				bResult = FileOpenQuizMultiLayoutModel (nDocKind, strFilePath, isTemplateMode);
			}
			else if (nDocKind == DOC_KIND._docArduino || nDocKind == DOC_KIND._docArduinoS)
			{
				bResult = FileOpenArduino (nDocKind, strFilePath);
			}
			else if (nDocKind == DOC_KIND._docArduinoC)
			{
				bResult = FileOpenArduinoC (strFilePath);
			}
			else if (nDocKind != DOC_KIND._docNone)
			{
				MakeNewModel (nDocKind);

				if (LC.LANG.ENGLISH == LC.PQLanguage)
				{
					SkinFrame pOldFrame = MainCanvas.CurrentDMTFrame as SkinFrame;
					if (null != pOldFrame)
					{
						Softpower.SmartMaker.TopLight.ViewModels.LightJDoc pOldDoc = pOldFrame.GetCurrentView ().GetDocument () as Softpower.SmartMaker.TopLight.ViewModels.LightJDoc;

						pOldDoc.AtomNameList = new Dictionary<string, string> (SMProperVar_Eng.MapAtomName);
						pOldDoc.SymbolNameList = new Dictionary<string, string> (SMProperVar_Eng.SymbolData);

						SMProperVar_Eng.MapAtomName = null;
						SMProperVar_Eng.SymbolData = null;
					}
				}

				var currentDMTFrame = MainCanvas.CurrentDMTFrame as DMTFrame;

				var document = currentDMTFrame.GetDocument () as DMTDoc;
				var titleName = Path.GetFileNameWithoutExtension (strFilePath);
				document.TemplateMode = isTemplateMode;

				switch (nDocKind)
				{
					case DOC_KIND._docEBook:
					case DOC_KIND._docSlideMaster:
						if (false == PQAppBase.IsSmartMakerVersion)
						{
							return false;
						}

						if (File.Exists (strFilePath))
						{
							currentDMTFrame.TemplateTempPath = MakeTemplateCopyFile (strFilePath);
							currentDMTFrame.OnLoadEBookModel (currentDMTFrame.TemplateTempPath);
						}
						else
						{
							currentDMTFrame.OnLoadEBookModel (strFilePath);
						}

						break;
					default:
						currentDMTFrame.OnOpenDocument (strFilePath);
						break;
				}

				if (null != currentDMTFrame.GetCurrentView ())
				{
					DMTDoc pLightDoc = this.GetActiveDocument () as DMTDoc;
					if (null != pLightDoc)
					{
						if (false == pLightDoc.IsDoorLockPassword ())   // 컨텐츠 파일보안이 설정된 폼일 경우 일치하지 않을 경우 읽기전용으로 열림.
						{
							SecurityMessageWindow ();
						}

						Softpower.SmartMaker.TopLight.Models.CFrameAttrib frameAttrib = pLightDoc.GetFrameAttrib ();

						int nTopHeight = frameAttrib.TopBarHeight;
						int nBottomHeight = frameAttrib.BottomBarHeight;

						if (0 < nTopHeight || 0 < nBottomHeight) //메뉴 영역 설정
						{
							if (false == pLightDoc.IsDynamicMode && false == pLightDoc.IsEBookDoc)
							{
								BarHeightChange (nTopHeight, nBottomHeight);
							}
						}
					}
				}

				DMTDoc pDoc = this.GetActiveDocument () as DMTDoc;
				if (null == pDoc) return false;

				if (0 == strFilePath.IndexOf ("DB:\\"))
				{
					int nPos = strFilePath.LastIndexOf (".");
					pDoc.FilePath = strFilePath.Substring (0, nPos);
				}
				else
				{
					pDoc.FilePath = strFilePath;

					pDoc.GetFrameAttrib ().Title = titleName;
					currentDMTFrame.ChangeCaptionText (titleName);
				}

				bResult = true;

				if (LC.LANG.ENGLISH == LC.PQLanguage)
				{
					pDoc.AtomNameList = SMProperVar_Eng.MapAtomName;
					pDoc.SymbolNameList = SMProperVar_Eng.SymbolData;
				}

				if (null != currentDMTFrame.GetCurrentView ())
				{
					DMTDoc pLightDoc = this.GetActiveDocument () as DMTDoc;
					if (null != pLightDoc && false == pLightDoc.IsDoorLockPassword ())   // 컨텐츠 파일보안이 설정된 폼일 경우 일치하지 않을 경우 읽기전용으로 열림.
					{
						SecurityMessageWindow ();
					}
				}

				DMTFrame dmtFrame = currentDMTFrame as DMTFrame;

				if (null != pDoc && DOC_KIND._docWeb == pDoc.DocType && null != dmtFrame)
				{
					GlobalWaitThread.WaitThread.Start (dmtFrame);
					//dmtFrame.MotionManager.FormLoad = true;
					//dmtFrame.MotionManager.CheckAtomVisibility ();
					dmtFrame.LoadHtmlTagAtom ();
				}
			}
			else if (nDocKind == DOC_KIND._docNone && 0 < strFilePath.IndexOf (".b", StringComparison.OrdinalIgnoreCase))
			{
				string strFileName = Path.GetFileNameWithoutExtension (strFilePath);

				DOC_KIND backupDocKind = ExtensionInfo.GetDocKindFromFileName (strFileName);
				if (DOC_KIND._docNone != backupDocKind)
				{
					if (DOC_KIND._docSmart == backupDocKind || DOC_KIND._docWeb == backupDocKind)
					{
						string strJson = File.ReadAllText (strFilePath);
						strJson = AutoBackupProcessManager.Instance.UnZip (strJson);

						ModelDescription modelDescription = new ModelGenerator.Components.TabView.Model.ModelDescription ();
						modelDescription.DocKind = backupDocKind;

						GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyMakeNewModel (modelDescription);

						DMTFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as DMTFrame;
						DMTView view = CurrentDMTFrame.GetCurrentView () as DMTView;

						view.SetAutoBackupData (strJson);
					}
					else
					{
						//북모델, 출력물은 백업 논리 구현 필요
					}
				}
			}

			return bResult;
		}

		private string MakeTemplateCopyFile (string filePath)
		{
			string newFilePath = filePath;

			try
			{
				var extension = Path.GetExtension (filePath);
				var copyName = Path.GetFileNameWithoutExtension (filePath) + "_" + Guid.NewGuid ().ToString () + extension;
				var copyFolder = Path.Combine (PQAppBase.BaseModulePath, "__Temp");
				var copyPath = Path.Combine (copyFolder, copyName);

				if (false == Directory.Exists (copyFolder))
					Directory.CreateDirectory (copyFolder);

				File.Copy (filePath, copyPath);
				newFilePath = copyPath;
			}
			catch (IOException ex)
			{
				Trace.TraceError (ex.ToString ());
				LogManager.WriteLog (ex);
				_Error80.Show (ex);
			}

			return newFilePath;
		}

		private bool FileOpenMenuDoc (string strFilePath)
		{
			bool bOpen = false;

			CTopMenuDoc pDoc = CreateMenuDocument ();

			pDoc.FilePath = strFilePath;
			bOpen = pDoc.OnOpenDocument (strFilePath);
			if (false != bOpen)
			{
				CTopMenuView menuView = pDoc.GetThisWindow () as CTopMenuView;
				menuView.Title = pDoc.GetFileName ();

				pDoc.Show ();
			}


			return bOpen;
		}

		private bool FileOpenScript (string strFilePath)
		{
			bool bOpen = false;

			CScrFrame scrFrame = CreateScrFrame ();
			ScriptDoc pDoc = scrFrame.Document;

			pDoc.FilePath = strFilePath;


			if (true == pDoc.OnOpenDocument (strFilePath))
			{
				scrFrame.NewFileSave += this.FileSaveEventHandler;
				scrFrame.ActivateWindow ();
				scrFrame.Show ();
				scrFrame.OnLoadSourceText ();
				scrFrame.Title = pDoc.GetFileName ();

				bOpen = true;
			}


			return bOpen;
		}

		private bool FileOpenServiceScript (string strFilePath)
		{
			bool bOpen = false;

			CScrFrame scrFrame = CreateServiceFrame ();
			ScriptDoc pDoc = scrFrame.Document;

			pDoc.FilePath = strFilePath;


			if (true == pDoc.OnOpenDocument (strFilePath))
			{
				scrFrame.NewFileSave += this.FileSaveEventHandler;
				scrFrame.ActivateWindow ();
				scrFrame.Show ();
				scrFrame.OnLoadSourceText ();
				scrFrame.Title = pDoc.GetFileName ();

				bOpen = true;
			}


			return bOpen;
		}

		private bool FileOpenReport (DOC_KIND nDocKind, string strFilePath, bool isTemplateMode)
		{
			bool bOpen = false;

			MakeNewModel (nDocKind);

			BaseFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as BaseFrame;


			CurrentDMTFrame.OnOpenDocument (strFilePath);
			bOpen = true;


			TopDoc pDoc = this.GetActiveDocument () as TopDoc;
			if (0 == strFilePath.IndexOf ("DB:\\"))
			{
				int nPos = strFilePath.LastIndexOf (".");
				pDoc.FilePath = strFilePath.Substring (0, nPos);
			}
			else
			{
				pDoc.FilePath = strFilePath;
				string strFileName = Path.GetFileNameWithoutExtension (strFilePath);

				//pDoc.GetFrameAttrib().Title = strFileName;
				CurrentDMTFrame.ChangeCaptionText (strFileName);
			}

			pDoc.TemplateMode = isTemplateMode;

			GlobalWaitThread.WaitThread.End ();

			return bOpen;
		}

		private bool FileOpenQuizLayoutModel (DOC_KIND nDocKind, string strFilePath, bool isTemplateMode)
		{
			bool isOpen = false;

			MakeNewModel (nDocKind);

			var frame = MainCanvas.CurrentDMTFrame as BaseFrame;

			if (false == string.IsNullOrEmpty (strFilePath))
			{
				frame.OnOpenDocument (strFilePath);
				isOpen = true;

				var document = this.GetActiveDocument () as TopDoc;
				document.FilePath = strFilePath;
				document.TemplateMode = isTemplateMode;

				string strFileName = Path.GetFileNameWithoutExtension (strFilePath);

				frame.ChangeCaptionText (strFileName);

				GlobalWaitThread.WaitThread.End ();
			}

			return isOpen;
		}

		private bool FileOpenQuizMultiLayoutModel (DOC_KIND nDocKind, string strFilePath, bool isTemplateMode)
		{
			bool isOpen = false;

			MakeNewModel (nDocKind);

			var frame = MainCanvas.CurrentDMTFrame as BaseFrame;

			if (false == string.IsNullOrEmpty (strFilePath))
			{
				frame.OnOpenDocument (strFilePath);
				isOpen = true;

				var document = this.GetActiveDocument () as TopDoc;
				document.FilePath = strFilePath;
				document.TemplateMode = isTemplateMode;

				string strFileName = Path.GetFileNameWithoutExtension (strFilePath);

				frame.ChangeCaptionText (strFileName);

				GlobalWaitThread.WaitThread.End ();
			}

			return isOpen;
		}

		private bool FileOpenArduino (DOC_KIND nDocKind, string strFilePath)
		{
			bool bOpen = false;

			MakeNewModel (nDocKind);

			SkinFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as SkinFrame;


			CurrentDMTFrame.OnOpenDocument (strFilePath);
			bOpen = true;


			TopDoc pDoc = this.GetActiveDocument () as TopDoc;

			if (0 == strFilePath.IndexOf ("DB:\\"))
			{
				int nPos = strFilePath.LastIndexOf (".");
				pDoc.FilePath = strFilePath.Substring (0, nPos);
			}
			else
			{
				pDoc.FilePath = strFilePath;
				string strFileName = Path.GetFileNameWithoutExtension (strFilePath);

				CurrentDMTFrame.ChangeCaptionText (strFileName);
			}

			return bOpen;
		}

		private bool FileOpenArduinoC (string strFilePath)
		{
			bool bOpen = false;

			ArduinoEditFrame scrFrame = CreateArduinoEditFrame ();
			ArduinoEditDoc pDoc = scrFrame.Document;

			pDoc.FilePath = strFilePath;

			if (true == pDoc.OnOpenDocument (strFilePath))
			{
				scrFrame.NewFileSave += this.FileSaveEventHandler;
				scrFrame.ActivateWindow ();
				scrFrame.Show ();
				scrFrame.Title = pDoc.GetFileName ();

				bOpen = true;
			}

			return bOpen;
		}

		#endregion

		#region 컨텐츠 파일보안
		private void SecurityMessageWindow ()
		{
			SecurityLockWindow securityWindow = new SecurityLockWindow ();
			securityWindow.Owner = this;
			securityWindow.Icon = this.Icon;
			securityWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			securityWindow.OnLockPassword += securityWindow_OnLockPassword;

			bool bDialog = (bool)securityWindow.ShowDialog ();
			if (false == bDialog)
			{
				MainCanvas.CurrentDMTFrame.GetCurrentView ().IsReadonlyMode = true;

				_Message80.Show (LC.GS ("SmartOffice_MainWindow_1808_5"), "SmartMaker", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		void securityWindow_OnLockPassword (object sender, object strValue)
		{
			DMTDoc pLightDoc = this.GetActiveDocument () as DMTDoc;
			if (null != pLightDoc)
			{
				string strPassword = pLightDoc.GetDoorLockPass ((string)strValue);
				string strLockPass = pLightDoc.GetKnowledgeBank ().LockPassWord;

				if (false != string.Equals (strPassword, strLockPass))
				{
					SecurityLockWindow securityWindow = sender as SecurityLockWindow;
					securityWindow.DialogResult = true;

					securityWindow.Close ();
				}
				else
				{
					_Message80.Show (LC.GS ("SmartOffice_MainWindow_1808_6"), "SmartMaker", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}
		#endregion

		#region |  void ScriptGlobalMainFrameLoading()  |

		private void ScriptGlobalMainFrameLoading ()
		{
			PQAppBase.KissAddGlobalInfo ("MENU", "NONE.QMU", false);
			CScriptApp.LoadGlobalScript (true);
		}

		#endregion

		#region |  void ScriptGlobalDel()  |

		private void ScriptGlobalDel ()
		{
			CScriptApp.CloseScriptServer ("_@Global_", true);
		}

		#endregion

		#region |  void UserLoginX(bool bIsAuto)  |
		/// <summary>
		/// 서버접속.
		/// </summary>
		/// <param name="bIsAuto">자동/입력</param>
		private void UserLoginX (bool bIsAuto)
		{
			if (null == PQAppBase.UserLogin)
			{
				PQAppBase.UserLogin = new CUserLogin ();
				PQAppBase.UserLogin.ShowOptionDlg += new OptionDlgEventHandler (ShowDatabaseEnviroment);
				PQAppBase.UserLogin.DBConnect += new ConnectEventHandler (OnDBConnect);
				PQAppBase.UserLogin.DBDisConnect += new DisConnectEventHandler (OnDBDisConnect);
				PQAppBase.UserLogin.HostingInformationSetEvent += new HostingInformationSetEventHandler (UserLogin_HostingInformationSetEvent);
			}
			else if (false == PQAppBase.UserLogin.IsAddedEvent ())
			{
				PQAppBase.UserLogin.ShowOptionDlg += new OptionDlgEventHandler (ShowDatabaseEnviroment);
				PQAppBase.UserLogin.DBConnect += new ConnectEventHandler (OnDBConnect);
				PQAppBase.UserLogin.DBDisConnect += new DisConnectEventHandler (OnDBDisConnect);
				PQAppBase.UserLogin.HostingInformationSetEvent += new HostingInformationSetEventHandler (UserLogin_HostingInformationSetEvent);
			}

			LOGIN_RESULT _result = LOGIN_RESULT.LOGIN_CANCEL;

			//TopComm Login 기능
			if (0 == PQAppBase.CompanyCode)
			{
				if (bIsAuto)
				{
					_result = PQAppBase.UserLoginX (LOGIN_FLAG.LOGIN_AUTOSYSUSER_CONN);

					if (LOGIN_RESULT.LOGIN_FAIL == _result)
					{
						RegLib.SetUIntRegInfo (RegistryCoreX.ProductServerPath + "LastLogin", "AutoLogin", 0);
					}
				}
				else
				{
					_result = PQAppBase.UserLoginX ();
				}
			}
			else if (1 == PQAppBase.CompanyCode)    //한화 AIDT OIDC 기능
			{
				var window = new HanwhaLoginWindow ();
				window.Owner = Application.Current.MainWindow;
				window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

				_result = LOGIN_RESULT.LOGIN_CANCEL;
				window.ShowDialog ();

				_result = window.LoginResult;

				if (_result == LOGIN_RESULT.LOGIN_SUCCESS)
				{
					PQAppBase.ConnectStatus = true;
				}
			}

			CUserLogin _login = PQAppBase.UserLogin;

			string strDBKind = _login.GetFirstDepLoginInfo (USER_INFO._DBKind);
			if (LOGIN_RESULT.LOGIN_CANCEL != _result)
			{
				InitDBIndexToKeyArray ();
				// 젼역스크립트 동작
				if (true == PQAppBase.ScriptGlobalIsLoading)
				{
					ScriptGlobalMainFrameLoading ();
				}
				else
				{
					ScriptGlobalDel ();
				}
				CallServerConnect (_login.LoginID, strDBKind, _login.LoginPWD, PQAppBase.ConnectStatus);
			}

			bool bIsConnect = PQAppBase.ConnectStatus;

			if (LOGIN_RESULT.LOGIN_SUCCESS == _result)
			{
				bIsConnect = true;
			}
			else if (LOGIN_RESULT.LOGIN_FAIL == _result)
			{
				bIsConnect = false;
			}

			ChangeStatusAndMenu (bIsConnect);
		}

		#endregion

		#region |  TopDoc CreateSaveDocument(DOC_KIND docKind, string strProperty, bool bTemplateMode, bool bShadowOpen)  |

		private TopDoc CreateSaveDocument (DOC_KIND docKind, string strProperty, bool bTemplateMode, bool bShadowOpen)
		{

			TopDoc pDoc = null;

			switch (docKind)
			{
				case DOC_KIND._docReport:
					if (!CheckExistenceOfPrinter ())
						return null;

					//80 pDoc = this.CreateExpandedReportDocument();
					break;
				case DOC_KIND._docProcedure:
				case DOC_KIND._docPA:
				case DOC_KIND._docOfficeDoc:
				case DOC_KIND._docOfficeDocX:
				case DOC_KIND._docErd:

					break;
				case DOC_KIND._docMenu:
					pDoc = CreateMenuDocument ();
					break;
				case DOC_KIND._docScript:
					pDoc = CreateScrDocument ();
					break;
				case DOC_KIND._docService:
					pDoc = CreateServiceDocument ();
					break;
				case DOC_KIND._docArduinoC:
					pDoc = CreateArduinoEditDocument ();
					break;
				case DOC_KIND._docWeb:
					pDoc = CreateDMTDocument (5, bTemplateMode, bShadowOpen);
					break;
				case DOC_KIND._docSmart:
					pDoc = CreateDMTDocument (6, bTemplateMode, bShadowOpen);
					break;
				case DOC_KIND._docProcess:
					pDoc = CreateDMTDocument (0, bTemplateMode, bShadowOpen);
					break;
				default:
					{
						int serializeNum = 0;
						if (strProperty != "0")
						{
							serializeNum = 1;   // NTOA
						}

						if (1 == serializeNum && !CheckExistenceOfPrinter ())
							return null;

						// Serialize 최적화

						//80 pDoc = CreateDMTDocument(serializeNum, bTemplateMode, bShadowOpen);		//NTOA
						break;
					}
			}

			return pDoc;
		}

		#endregion

		#region |  DMTDoc CreateDMTDocument(int nDocKind, bool bTemplateMode, bool bShadowOpen)  |

		private DMTDoc CreateDMTDocument (int nDocKind, bool bTemplateMode, bool bShadowOpen)
		{

			DMTDoc pDMTDoc = new DMTDoc ();
			pDMTDoc.TemplateMode = bTemplateMode;
			//pDMTDoc.IsShadowOpen = bShadowOpen;
			//80 pDMTDoc.SetCursorData(m_pCursorData);

			// NTOA	- 서식모델
			if (2 == nDocKind)
			{
				//80 pDMTDoc.SetNewDocument(true);
				pDMTDoc.SetNtoaDocKind (1);
			}
			else
			{
				pDMTDoc.SetNtoaDocKind (nDocKind);  //	0:QPG, 1:NTOA, 2:EDMS, 3:KMS, 5:QWP, 6:QPS (SMART FORM)
			}

			if (false == bShadowOpen)
			{
				//80 ToolBarShowStatus(pDMTDoc);	// ToolBar 처리를 먼저하게 하여 깜빡임 제거하게 함
				//80 UserToolBarShowStatus(pDMTDoc);

				if (true == bTemplateMode)
				{
					/*80 templateManagerControl1.RemoveControls();
					templateManagerControl1.Location = new Point(0, templateManagerControl1.Location.Y);
					templateManagerControl1.Visible = true;*/

				}
			}

			//80 pDMTDoc.CreateForm(this);
			//80 pDMTDoc.CustStatusBar = this.customStatusBar1;
			pDMTDoc.SetGlobalPtr (this.JobGInfo);
			//80 pDMTDoc.AtomLayoutInfoFrm = this.AtomLayoutInfoFrm;

			DMTFrame pDMTFrame = pDMTDoc.GetThisForm () as DMTFrame;
			if (null != pDMTFrame)
			{
				/*80 MdiChildrenWindowPos(pDMTFrame);
				pDMTFrame.NewFileSave += new ProcessQ.TopProcess.FileSaveEventHandler(this.FileSaveEventHandler);
				pDMTFrame.OnAtomSelected += new ProcessQ.TopProcess.OnAtomSelectedEventHandler(dmtFrame_OnAtomSelected);
				pDMTFrame.MdiActivate += new ProcessQ.TopProcess.MdiActivateEventHandler(this.MainFrame_MdiChildActivate);
				pDMTFrame.VideoPlayerEvent += new ProcessQ.TopProcess.VideoPlayerEventHandler(this.VideoPlayStop);*/

				pDMTFrame.OnOffsetEventHandler += new EventHandler (DMTFrame_OnOffsetEventHandler);
			}

			return pDMTDoc;
		}

		#endregion

		#region |  bool CheckExistenceOfPrinter() : 항상 true 반환  |

		private bool CheckExistenceOfPrinter ()
		{

			return true;
		}

		#endregion

		#region |  int CallServerConnect(String strLoginID, String strDBKind, String strLoginPWD, bool bConnectStatus)  |

		private int CallServerConnect (String strLoginID, String strDBKind, String strLoginPWD, bool bConnectStatus)
		{
			return (CScriptApp.CallServerConnMag (strLoginID, strDBKind, strLoginPWD, bConnectStatus));
		}

		#endregion

		#region |  void ChangeStatusAndMenu(bool bIsConnect)  |

		private void ChangeStatusAndMenu (bool bIsConnect)
		{

			CUserLogin _login = PQAppBase.UserLogin;
			string strDBKind = _login.GetFirstDepLoginInfo (USER_INFO._DBKind);
			DB_KIND dbKind = ("" != strDBKind) ? (DB_KIND)Convert.ToInt32 (strDBKind) : DB_KIND._dbSQL;

			if (false == PQAppBase.IsSystemUser (_login.LoginID, dbKind))
			{
				bIsConnect = false;
			}

			//80 추후작업 
			/*
			// 조직관리
           
			this.menuMainEnvironmentGroupCompany.Enabled = bIsConnect;
			this.menuMainEnvironmentGroupTeam.Enabled = bIsConnect;
			this.menuMainEnvironmentGroupUser.Enabled = bIsConnect;

			// 모델검사

			this.menuMainEditModelCheck.Enabled = bIsConnect;

			// 서버접속<->서버절단
			if (bIsConnect)
			{
				this.menuMainCommonLogin.Text = m_strDisConnectString;
			}
			else
			{
				this.menuMainCommonLogin.Text = m_strConnectString;
			}
			*/
		}

		#endregion

		#region |  void InitDBIndexToKeyArray()  |

		private void InitDBIndexToKeyArray ()
		{

			foreach (object childObj in MainCanvas.GetAllFrame ())
			{
				if (childObj is DMTFrame)
				{
					DMTFrame childFrame = childObj as DMTFrame;
					childFrame.InitDBIndexToKeyArray ();
				}
				else if (childObj is ReportDMTFrame)
				{
					ReportDMTFrame childFrame = childObj as ReportDMTFrame;
					childFrame.InitDBIndexToKeyArray ();
				}
			}
		}

		#endregion

		#region |  void ApplyApplicationTheme(StyleCategory.StyleThemeCategory StyleKey)  |
		/// <summary>
		/// 전체적인 테마를 바꾸어주는 함수
		/// </summary>
		/// <param name="StyleKey"></param>
		private void ApplyApplicationTheme (StyleCategory.StyleThemeCategory StyleKey)
		{

			Dictionary<string, ResourceDictionary> AppResources = StyleResourceManager.ApplicationResourceDictionary;

			if (null != AppResources)
			{

				CommonStringGenerator.StringGenerator.Clear ();
				CommonStringGenerator.StringGenerator.AppendFormat ("{0}ThemeDictionary", StyleKey.ToString ());
				string strStyleKey = CommonStringGenerator.StringGenerator.ToString ();

				StyleResourceManager.ApplyTheme (AppResources, OutLineGrid, strStyleKey, "ApplicationOutLineGridStyle");
				//상단 메뉴 스타일
				Menu rootMenu = MainMenu.FindName ("RootMenu") as Menu;
				StyleResourceManager.ApplyTheme (AppResources, rootMenu, strStyleKey, "ApplicationMenuStyle");

				//하단 스테이터스바 스타일
				Border statusBorder = MainStatusBar.FindName ("StatusBorder") as Border;
				StyleResourceManager.ApplyTheme (AppResources, statusBorder, strStyleKey, "ApplicationStatusBarStyle");

				//상단 툴바 스타일
				Border rootToolBar = MainToolBarPanel.FindName ("RootBorder") as Border;
				StyleResourceManager.ApplyTheme (AppResources, rootToolBar, strStyleKey, "ApplicationToolBarStyle");

			}
		}

		#endregion

		#region |  void SendRootMouseEventToAtom()  |
		/// <summary>
		/// 최초 마우스 이벤트 아톰에게 전달
		/// 중간에 마우스 이벤트 가로 채는것 우회
		/// </summary>
		private void SendRootMouseEventToAtom ()
		{

			//GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyCurrentLocationAndSize(this.Margin, 0, 0);

			if (null != MainCanvas.CurrentDMTFrame)
			{
				MainCanvas.CurrentDMTFrame.OnSendRootMouseEventToAtom ();
			}

		}

		#endregion

		#region |  void CheckAllPopupPanels(MouseButtonEventArgs e)  |

		private void CheckAllPopupPanels (MouseButtonEventArgs e)
		{

			//속성창은 포커스를 잃으면 꺼지는 특징이 있다.
			//따라서 현재 편집하려는 속성창이 없거나, 속성창에 마우스 이벤트가 잃어나지 않는다면 속성창을 끈다.
			HitTestResult isHit = null;

			if (null != AtomAttWindow && AtomAttWindow.Visibility == Visibility.Visible
				&& false == AtomAttWindow.OutBoundComboBoxOpened)
			{
				isHit = VisualTreeHelper.HitTest (AtomAttWindow, e.GetPosition (AtomAttWindow));

				if (null == isHit)
				{
					HitTestResult runHit = VisualTreeHelper.HitTest (this.MainToolBarPanel.OfficeToolBar.RunImage, e.GetPosition (this.MainToolBarPanel.OfficeToolBar.RunImage));
					if (null != runHit)
					{
						// 설정창이 열여있는 상태에서 RunMode 전환시, 저장 이벤트 처리순서 문제로 Animation 처리 없이 저장할수 있도록 합니다.
						AtomAttWindow.EndSaveNoAnimation ();
					}
					else
					{
						AtomAttWindow.EndAnimation ();
					}
				}
			}

			if (false == MainAtomContextMenuPanel.IsMouseOver)
			{
				MainAtomContextMenuOpen (false);
				MainAtomContextMenuPanel.Child = null;
			}

			isHit = null;
			isHit = VisualTreeHelper.HitTest (MainMenuPopupPanel, e.GetPosition (MainMenuPopupPanel));

			if (null == isHit)
			{
				MainMenuPopupPanel.Visibility = Visibility.Collapsed;
			}

			List<Popup> lstAllToolBarPopups = MainToolBarPanel.GetAllPopupPanels ();

			foreach (Popup CurrentPopup in lstAllToolBarPopups)
			{
				UIElement PopupChild = CurrentPopup.Child;
				isHit = null;
				isHit = VisualTreeHelper.HitTest (PopupChild, e.GetPosition (PopupChild));

				if (null == isHit)
				{
					CurrentPopup.IsOpen = false;
				}
			}

			FigureAtomPopupMenu.Unit.IsOpen = false;
			SpeechBalloonPopupMenu.Unit.IsOpen = false;
			QuizViewAtomPopupMenu.Unit.IsOpen = false;

			if (FrameTogglePopup.ShowActivated)
				FrameTogglePopup.Close ();
		}

		#endregion

		#region | void CheckMouseFocus() |

		private void CheckMouseFocus ()
		{
			TopDoc topDoc = GetActiveDocument ();

			if (null == topDoc)
			{
				List<double> pList = new List<double> ();
				pList.Add (0);
				pList.Add (0);
				pList.Add (0);
				pList.Add (0);
				if (true == MainStatusBar.CheckDefaultValue ())
					UpdateStatusBarAboutLocationAndSize (pList);
			}
		}

		#endregion

		#region |  ----- 파일 처리관련 -----  |
		#region |  void OnFileSaveAs()  |
		/// <summary>
		/// 다른 저장 
		/// </summary>
		private void OnFileSaveAs ()
		{
			BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			if (null != CurrentFrame)
			{
				FileSaveAs ();
				ChangeCaptionText ();
			}
			else
			{
				TopDoc menuDoc = ExecuteMenuManager.Instance.GetActiveDocument ();
				if (null != menuDoc)
				{
					FileSaveAs ();
				}
			}
		}

		#endregion

		#region |  void OnFileSave()  |
		/// <summary>
		/// 모델 저장 
		/// </summary>
		private void OnFileSave ()
		{
			BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			if (null != CurrentFrame)
			{
				FileSave ();
				ChangeCaptionText ();
			}
			else
			{
				//Softpower.SmartMaker.TopReportLight.ViewModel.ReportLightDoc doc = MainCanvas.CurrentDMTFrame.GetDocument () as Softpower.SmartMaker.TopReportLight.ViewModel.ReportLightDoc;

				//System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog ();
				//saveFileDialog.Filter = "출력물 모델|*.QRP";
				////saveFileDialog1.Title = "Save an Image File";
				//if (System.Windows.Forms.DialogResult.OK == saveFileDialog.ShowDialog ())
				//{
				//	doc.OnSaveDocument (saveFileDialog.FileName);
				//}
			}
		}

		#endregion

		#region |  void ChangeCaptionText()  |

		private void ChangeCaptionText ()
		{
			if (null != MainCanvas.CurrentDMTFrame)
			{
				MainCanvas.CurrentDMTFrame.ChangeCaptionText ();
			}
		}

		#endregion

		#region |  void FileSave()  |

		private void FileSave ()
		{
			try
			{
				DOC_KIND DocKind = this.GetActiveDocKind ();
				TopDoc pDoc = GetActiveDocument ();

				if (null == pDoc)
				{
					return;
				}

				if (true == m_IsFileSaveFlag)
				{
					Trace.TraceInformation ("[TopBuild] private void FileSave () 파일 중복 저장 시도");
					LogManager.WriteLog (LogType.Warning, "[TopBuild] private void FileSave () 파일 중복 저장 시도");
					return;
				}

				m_IsFileSaveFlag = true;

				if (DOC_KIND._docSmart == DocKind || DOC_KIND._docProcess == DocKind || DOC_KIND._docWeb == DocKind)
				{
					if (false == pDoc.CanSaveDocument && false == pDoc.IsFormChange ())
						return;

					BaseFrame currentFrame = this.MainCanvas.CurrentDMTFrame as BaseFrame;
					TopView pDMTView = currentFrame.GetCurrentView ();

					bool pRunMode = pDoc.GetRunMode ();

					if (false != pRunMode)
					{
						pDMTView.RollbackAtomAttribs (pDoc.GetRunMode ());
					}
					else
					{
						//2020-09-08 kys 아톰편집모드를 종료한다.
						pDMTView.SetChangeAtomSaveMode ();
					}
				}

				string strFilePath = pDoc.FilePath;

				// 2006.5. 15, 김영택, Ctrl+s일때 저장되지 않는 사항...수정. 
				// 원인은 파일명만을 가지고 제어를 하다 보니 그냥 일반 파일로 인식
				// 같은 이름을 가진 파일을 실행 디렉토리에 생성함.
				// DB저장시는 reg값을 가지고 저장이 되므로. 그것을 이용하여 DB파일인지 
				// 확인이 가능하다. 

				if (0 < strFilePath.Length && ((0 == strFilePath.IndexOf ("DB:\\")) || (null != pDoc.DBRegNumber && 10 == pDoc.DBRegNumber.Length)))//DB파일
				{
					DBFileSave (DocKind, pDoc, false);
				}
				else if (0 < strFilePath.Length && 0 != strFilePath.IndexOf ("DB:\\"))
				{
					System.IO.FileInfo fileInfo = new System.IO.FileInfo (strFilePath);
					if (false == fileInfo.Exists)
					{
						System.IO.FileStream pFileStream = System.IO.File.Create (strFilePath);
						pFileStream.Close ();
					}

					if ((System.IO.File.GetAttributes (strFilePath) & System.IO.FileAttributes.ReadOnly) != System.IO.FileAttributes.ReadOnly && false == pDoc.TemplateMode)//일반파일
					{
						FileSave (pDoc, DocKind, strFilePath);
					}
					else
					{
						FileSaveAs ();
					}
				}
				else //새로저장
				{
					FileSaveAs ();
				}
			}
			// 2007.09.13 이정대, 파일저장시 경로가 맞지 않는 Exception 처리 추가
			catch (System.IO.DirectoryNotFoundException ex2)
			{
				Trace.TraceError (ex2.ToString ());
				string strMsg = LC.GS ("SmartOffice_MainWindow_13") + "\n";
				strMsg += LC.GS ("SmartOffice_MainWindow_14") + "\n";
				string strTitle = LC.GS ("SmartOffice_MainWindow_6");
				//bool bResult = false;
				MessageBoxResult mbResult = _Message80.Show (strMsg, strTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
				if (MessageBoxResult.Yes == mbResult)
				{
					FileSaveAs ();
				}
			}


			m_IsFileSaveFlag = false;
		}

		#endregion
		#endregion//파일 처리관련	

		#region |  ----- 도구 관련 처리 -----  |

		#region |  void OnShowFormScriptEditDialog80() : 업무규칙  |
		/// <summary>
		/// 업무규칙
		/// </summary>
		private void OnShowFormScriptEditDialog80 ()
		{
			TopDoc pDoc = null;
			BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

			Window mdiChildWindow = null;

			if (null != CurrentFrame && null != CurrentFrame.GetCurrentView ())
			{
				TopView CurrentDMTView = CurrentFrame.GetCurrentView ();
				pDoc = CurrentDMTView.GetDocument ();

				if (null != pDoc)
				{
					MainExpandMenuContainer.CurrentExpandMenuType = (int)MainExpandMenuType.ScriptMenu;
					mdiChildWindow = pDoc.ShowEditWindow80 (0x00000000);
					mdiChildWindow.Activated += ScriptWindow80_Activated;
					mdiChildWindow.Closed += ScriptWindow80_Closed;
				}
			}
		}

		#endregion

		#region |  void OnShowLinkHelp() : 업무규칙 도움말  |
		/// <summary>
		/// 업무규칙 도움말
		/// </summary>
		private void OnShowLinkHelp ()
		{
			try
			{
				//Softpower.SmartMaker.TopControlEdit.SubControl.WinCHMView pView = new TopControlEdit.SubControl.WinCHMView ();
				//pView.Owner = Application.Current.MainWindow;
				//pView.Show ();
				//pView.Navigate ("");
				Process.Start ("https://store.smartmaker.com/WebHelp/index.html");
			}
			catch (Exception ex)
			{
				LogManager.WriteLog (LogType.Error, ex.Message);
			}
		}

		#endregion


		#region |  void OnShowStructDataMgrDialog(bool isOpenAPI)  |
		/// <summary>
		/// 외부기능연계 객체
		/// </summary>
		private void OnShowStructDataMgrDialog (int nServiceType) // 0 : OpenApi, 1 : REST-WS, 2 : SOAP-WS, 3 : SAP-RFC, 4: SAP-OData
		{
			if (false == PQAppBase.strTrial)
			{
				switch (nServiceType)
				{
					case 1: //REST-WS, SOAP-WS 는 프로버전에서만 사용 가능
					case 2:
						if (false == LicenseHelper.Instance.IsEnableSolutionService (SolutionService.StructService))
						{
							return;
						}
						break;
					case 3: //SAP-RFC 엔터프라이즈버전에서만 사용 가능
						if (false == LicenseHelper.Instance.IsEnableSolutionService (SolutionService.Legacy))
						{
							return;
						}
						break;
					case 4: //SAP-RFC 엔터프라이즈버전에서만 사용 가능
						if (false == LicenseHelper.Instance.IsEnableSolutionService (SolutionService.Legacy))
						{
							return;
						}
						break;
				}
			}

			if (null != MainCanvas.CurrentDMTFrame)
			{
				MainCanvas.CurrentDMTFrame.OnStructData (this, nServiceType);
			}
		}

		#endregion

		#region |  void OnShowProcessManager() : 진행관리자  |
		/// <summary>
		/// 진행관리자
		/// </summary>
		private void OnShowProcessManager ()
		{

			TopDoc pDoc = null;
			BaseFrame currentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

			if (null != currentFrame && null != currentFrame.GetCurrentView ())
			{
				pDoc = currentFrame.GetCurrentView ().GetDocument ();

				if (null != pDoc)
				{
					pDoc.ShowProcessEventManager ();
				}
			}
		}

		#endregion

		#region |  void OnShowAtomEditManager() : 아톰편집 도우미  |
		/// <summary>
		/// 진행관리자
		/// </summary>
		private void OnShowAtomEditManager ()
		{
			TopDoc pDoc = null;
			BaseFrame currentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

			if (null != currentFrame && null != currentFrame.GetCurrentView ())
			{
				pDoc = currentFrame.GetCurrentView ().GetDocument ();

				if (null != pDoc)
				{
					pDoc.ShowAtomEditManager ();
				}
			}
		}

		#endregion

		#region |  void OnShowAnimationManager() : 애니관리자  |
		/// <summary>
		///  애니메이션
		/// </summary>
		private void OnShowAnimationManager ()
		{
			if (false != LicenseHelper.Instance.IsEnableSolutionService (SolutionService.AnimationManager))
			{
				BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

				if (null != CurrentFrame)
				{
					TopView CurrentDMTView = CurrentFrame.GetCurrentView ();
					TopDoc CurrentDMTDoc = null == CurrentDMTView ? null : CurrentDMTView.GetDocument ();

					if (null != CurrentDMTDoc)
					{
						CurrentDMTDoc.OnToolAnimation ();
					}
				}
			}
		}

		#endregion

		#region |  void OnShowArduinoManager() : 아두이노관리자  |
		/// <summary>
		///  애니메이션
		/// </summary>
		private void OnShowArduinoManager ()
		{
			if (false != LicenseHelper.Instance.IsEnableSolutionService (SolutionService.ArduinoManager))
			{
				BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

				if (null != CurrentFrame)
				{
					TopView CurrentDMTView = CurrentFrame.GetCurrentView ();
					TopDoc CurrentDMTDoc = null == CurrentDMTView ? null : CurrentDMTView.GetDocument ();

					if (null != CurrentDMTDoc)
					{
						CurrentDMTDoc.OnToolArduino ();
					}
				}
			}
		}

		#endregion

		#region |  void OnShowPaintTool() : 이미지편집기  |
		/// <summary>
		/// 이미지편집기(PaintDotNet)
		/// </summary>
		private void OnShowPaintTool ()
		{

			if (false != LicenseHelper.Instance.IsEnableSolutionService (SolutionService.PaintTool))
			{
				PaintToolManager ptManager = new PaintToolManager ();
				ptManager.ShowTool (Application.Current.MainWindow, "");
			}
		}

		#endregion

		#region |  void OnAutoFormsErdCreator() : 테이블 생성  |
		/// <summary>
		/// 테이블 생성
		/// </summary>
		private void OnAutoTableErdCreator ()
		{
			DMTDoc pDMTDoc = this.GetActiveDocument () as DMTDoc;
			if (null != pDMTDoc)
			{
				pDMTDoc.AutoErdInfoUpdate ();                   // F5 동작
				pDMTDoc.OnSendToDatabaseTableQuery80 (true);    // F10 동작
			}
		}

		private void OnAutoFormsErdCreator ()
		{
			if (true == ErdAuto)
			{
				return;
			}

			if (false != LicenseHelper.Instance.IsEnableSolutionService (SolutionService.TopComm))
			{
				Softpower.SmartMaker.TopDBManager80.ErdEventArgs msg = new Softpower.SmartMaker.TopDBManager80.ErdEventArgs ();
				msg.ServerConnectionString = GetConnectionString ();
				msg.FormInformationMap = new Hashtable ();

				// -- 화면상에 있는 폼  

				List<BaseFrame> oaChildForm = MainCanvas.GetAllFrame ();
				//80 메인 캔버스에 추가되는것 QPM이 아닐경우 추후작업 
				//ArrayList oaChildForm = GetChildFrames(DOC_KIND._docProcess);

				foreach (object objFrame in oaChildForm)
				{
					//DMTFrame pFrame = objFrame as DMTFrame;
					BaseFrame pFrame = objFrame as BaseFrame;
					if (null == pFrame)
					{
						continue;
					}

					//DMTDoc currentDoc = pFrame.CurrentDMTView.Document as DMTDoc;
					TopDoc currentDoc = pFrame.GetCurrentView ().GetDocument ();

					CDMTFrameAttrib frameAttrib = currentDoc.GetFrameAttribObject () as CDMTFrameAttrib;

					currentDoc.AutoErdInfoUpdate ();

					Hashtable htFormInformation = currentDoc.GetErdAutoInformation ();
					if (0 != htFormInformation.Count)
					{
						string strFormName = currentDoc.GetFormName ();
						if (false == string.IsNullOrEmpty (strFormName) && false == msg.FormInformationMap.ContainsKey (strFormName))
						{

							msg.FilePaths.Add (string.IsNullOrEmpty (currentDoc.FilePath) ? frameAttrib.Title : currentDoc.FilePath);
							msg.FormInformationMap.Add (currentDoc.GetFormName (), htFormInformation);
						}
					}
				}

				ERDAutoGenWindow pErdAutoGenWindow = new ERDAutoGenWindow ();
				pErdAutoGenWindow.CaptionIcon = MainMenu.AppImage.Source as BitmapImage;
				pErdAutoGenWindow.Owner = this;
				pErdAutoGenWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
				pErdAutoGenWindow.Closed += pErdAutoGenWindow_Closed;
				pErdAutoGenWindow.OnShowErdManager += pErdAutoGenWindow_OnShowErdManager;
				pErdAutoGenWindow.InitFormsErdGenView (msg);
				pErdAutoGenWindow.Show ();
				pErdAutoGenWindow.IsExistDB ();
				pErdAutoGenWindow.Title = LC.GS ("SmartOffice_MainWindow_1812_2");
			}
		}

		#endregion

		#region SQL 추적기
		/// <summary>
		/// SQL 추적기
		/// </summary>
		private void OnShowSQLTrace ()
		{
			if (null == m_SQLTraceWindow)
			{
				m_SQLTraceWindow = new SQLTraceWindow ();

				m_SQLTraceWindow.Owner = this;
				m_SQLTraceWindow.Closed += SQLTraceWindow_Closed;

				m_SQLTraceWindow.Show ();

				SQLTraceReceiver.Instance.NotifySQLTraceEvent += SQLTrace_NotifySQLTraceEvent;
				SQLTraceReceiver.Instance.NotifySQLElapsedEvent += SQLTrace_NotifySQLElapsedEvent;
			}
			else
			{
				m_SQLTraceWindow.ActivateWindow ();
			}
		}

		private void SQLTrace_NotifySQLTraceEvent (string strSQL)
		{
			if (null != m_SQLTraceWindow)
			{
				var t = Task.Run (() =>
				{
					Dispatcher.Invoke (DispatcherPriority.Background, (Action)(() =>
					{
						if (null != m_SQLTraceWindow)
						{
							m_SQLTraceWindow.AppendSQL (strSQL);
						}
					}));
				});
			}
		}

		private void SQLTrace_NotifySQLElapsedEvent (string strElapsedTime)
		{
			if (null != m_SQLTraceWindow)
			{
				var t = Task.Run (() =>
				{
					Dispatcher.Invoke (DispatcherPriority.Background, (Action)(() =>
					{
						m_SQLTraceWindow.AppendElapsedTime (strElapsedTime);
					}));
				});
			}
		}

		void SQLTraceWindow_Closed (object sender, EventArgs e)
		{
			m_SQLTraceWindow = null;

			SQLTraceReceiver.Instance.NotifySQLTraceEvent -= SQLTrace_NotifySQLTraceEvent;
			SQLTraceReceiver.Instance.NotifySQLElapsedEvent -= SQLTrace_NotifySQLElapsedEvent;
		}
		#endregion

		#region HTTP 추적기
		/// <summary>
		/// HTTP 추적기
		/// </summary>
		private void OnShowHttpTrace ()
		{
			if (null == m_HttpTraceWindow)
			{
				m_HttpTraceWindow = new HttpTraceWindow ();

				m_HttpTraceWindow.Owner = this;
				m_HttpTraceWindow.Closed += HttpTraceWindow_Closed;

				m_HttpTraceWindow.Show ();

				HttpTraceReceiver.Instance.NotifyRequestTraceEvent += HttpTraceReceiver_NotifyRequestTraceEvent;
				HttpTraceReceiver.Instance.NotifyResponseTraceEvent += HttpTraceReceiver_NotifyResponseTraceEvent;
			}
			else
			{
				m_HttpTraceWindow.ActivateWindow ();
			}
		}

		private void HttpTraceReceiver_NotifyRequestTraceEvent (string strRequest)
		{
			if (null != m_HttpTraceWindow)
			{
				var t = Task.Run (() =>
				{
					Dispatcher.Invoke (DispatcherPriority.Background, (Action)(() =>
					{
						m_HttpTraceWindow.AppendRequest (strRequest);
					}));
				});
			}
		}

		private void HttpTraceReceiver_NotifyResponseTraceEvent (string strResponse)
		{
			if (null != m_HttpTraceWindow)
			{
				var t = Task.Run (() =>
				{
					Dispatcher.Invoke (DispatcherPriority.Background, (Action)(() =>
					{
						m_HttpTraceWindow.AppendResponse (strResponse);
					}));
				});
			}
		}

		private void HttpTraceWindow_Closed (object sender, EventArgs e)
		{
			m_HttpTraceWindow = null;

			HttpTraceReceiver.Instance.NotifyRequestTraceEvent -= HttpTraceReceiver_NotifyRequestTraceEvent;
			HttpTraceReceiver.Instance.NotifyResponseTraceEvent -= HttpTraceReceiver_NotifyResponseTraceEvent;
		}
		#endregion

		#region Script 추적기
		/// <summary>
		/// Script 추적기
		/// </summary>
		private void OnShowScriptTrace ()
		{
			if (null == m_ScriptTraceWindow)
			{
				m_ScriptTraceWindow = new ScriptTraceWindow ();

				m_ScriptTraceWindow.Owner = this;
				m_ScriptTraceWindow.Closed += ScriptTraceWindow_Closed;

				m_ScriptTraceWindow.Show ();

				ScriptTraceReceiver.Instance.NotifyRefreshTraceEvent += ScriptTraceReceiver_NotifyRefreshTraceEvent;
				ScriptTraceReceiver.Instance.NotifyScriptSymbolTraceEvent += ScriptTraceReceiver_NotifyScriptSymbolTraceEvent;
				ScriptTraceReceiver.Instance.NotifyVarNameTraceEvent += ScriptTraceReceiver_NotifyVarNameTraceEvent;
			}
			else
			{
				m_ScriptTraceWindow.ActivateWindow ();
			}

			RefreshScriptTrace ();
		}

		void ScriptTraceReceiver_NotifyRefreshTraceEvent ()
		{
			if (null != m_ScriptTraceWindow)
			{
				var t = Task.Run (() =>
				{
					Dispatcher.Invoke (DispatcherPriority.Background, (Action)(() =>
					{
						RefreshScriptTrace ();
					}));
				});
			}
		}

		void ScriptTraceReceiver_NotifyScriptSymbolTraceEvent (int nSymbolID)
		{
			if (null != m_ScriptTraceWindow)
			{
				var t = Task.Run (() =>
				{
					Dispatcher.Invoke (DispatcherPriority.Background, (Action)(() =>
					{
						m_ScriptTraceWindow.RefreshVariableItem (nSymbolID);
					}));
				});
			}
		}

		void ScriptTraceReceiver_NotifyVarNameTraceEvent (string strVarName)
		{
			if (null != m_ScriptTraceWindow)
			{
				var t = Task.Run (() =>
				{
					Dispatcher.Invoke (DispatcherPriority.Background, (Action)(() =>
					{
						m_ScriptTraceWindow.RefreshVariableItem (strVarName);
					}));
				});

			}
		}

		private void RefreshScriptTrace ()
		{
			if (null != m_ScriptTraceWindow)
			{
				LightJDoc pLightDoc = this.GetActiveDocument () as LightJDoc;
				if (null != pLightDoc && false != pLightDoc.GetRunMode ())
				{
					m_ScriptTraceWindow.Document = pLightDoc;
					m_ScriptTraceWindow.SetVariableList ();
				}
			}
		}

		private void ScriptTraceWindow_Closed (object sender, EventArgs e)
		{
			m_ScriptTraceWindow = null;

			ScriptTraceReceiver.Instance.NotifyRefreshTraceEvent -= ScriptTraceReceiver_NotifyRefreshTraceEvent;
			ScriptTraceReceiver.Instance.NotifyScriptSymbolTraceEvent -= ScriptTraceReceiver_NotifyScriptSymbolTraceEvent;
			ScriptTraceReceiver.Instance.NotifyVarNameTraceEvent -= ScriptTraceReceiver_NotifyVarNameTraceEvent;
		}

		#endregion

		#region | 학습정보 추적기 | 

		private void OnShowLRSTrace ()
		{
			if (null == m_LRSTraceWindow)
			{
				m_LRSTraceWindow = new LRSTraceWindow ();

				m_LRSTraceWindow.Owner = this;
				m_LRSTraceWindow.Closed += LRSTraceWindow_Closed;

				m_LRSTraceWindow.Show ();
			}
			else
			{
				m_LRSTraceWindow.ActivateWindow ();
			}

			RefreshLRSTrace ();
		}


		private void RefreshLRSTrace ()
		{
			if (null != m_LRSTraceWindow)
			{
				LightJDoc pLightDoc = this.GetActiveDocument () as LightJDoc;
				if (null != pLightDoc)
				{
					m_LRSTraceWindow.Document = pLightDoc;
					m_LRSTraceWindow.RefreshLRSData ();
				}
			}
		}

		private void LRSTraceWindow_Closed (object sender, EventArgs e)
		{
			m_LRSTraceWindow = null;
		}

		#endregion

		/// <summary>
		/// 프로젝트 정보
		/// </summary>
		private void OnShowProjectMetadataDialog ()
		{
			BaseFrame currentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			TopView currentView = currentFrame.GetCurrentView ();

			if (currentFrame is DMTFrame)
			{
				MainCanvas_ShowAttPageEvent (new SmartProjectMetaAttCore (currentFrame as DMTFrame), null, "프로젝트정보");
			}
		}

		/// <summary>
		/// 프로그램 속성
		/// </summary>
		private void OnShowFrameAttribDialog ()
		{
			BaseFrame currentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			TopView currentView = currentFrame.GetCurrentView ();

			if (currentView is DMTView)
			{
				LightJDoc pCurrentDoc = currentView.GetDocument () as LightJDoc;
				MainCanvas_ShowAttPageEvent (new SmartFrameAttCore (currentView as DMTView, pCurrentDoc.IsWebDoc, pCurrentDoc.IsEBookDoc), null, "프로그램속성");
			}
		}

		/// <summary>
		/// 문제풀이 속성
		/// </summary>
		private void OnShowEBookQuestionsOptionDialog ()
		{
			BaseFrame currentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			TopView currentView = currentFrame.GetCurrentView ();

			if (currentView is DMTView)
			{
				LightJDoc pCurrentDoc = currentView.GetDocument () as LightJDoc;
				MainCanvas_ShowAttPageEvent (new SmartFrameAttCore (currentView as DMTView, pCurrentDoc.IsWebDoc, pCurrentDoc.IsEBookDoc), null, "문제풀이속성");
			}
		}

		#region |  ----- PDF분석기-----  |

		/// <summary>
		/// PDF분석기
		/// </summary>
		private void OnShowPDFViewerDialog ()
		{
			var pdfViewer = new PDFViewer ();

			pdfViewer.Owner = this;
			pdfViewer.OnCreateAtom += PDFViewer_OnCreateAtom;
			pdfViewer.OnCreateNewPage += PDFViewer_OnCreateNewPage;

			m_PDFViewer = pdfViewer;
			pdfViewer.Show ();
		}

		private void PDFViewer_OnCreateAtom (object objValue)
		{
			GlobalWaitThread.WaitThread.Start ();
			try
			{
				BaseFrame currentFrame = MainCanvas?.CurrentDMTFrame as BaseFrame;
				TopView currentView = currentFrame?.GetCurrentView ();

				if (currentView is DMTView dmtView)
				{
					LightJDoc pCurrentDoc = currentView.GetDocument () as LightJDoc;

					pCurrentDoc.SetFormChange (true);

					var elementList = objValue as List<PDFElement>;
					PDFConvertAtomManger.Instance.MakeAtomData (this.MainCanvas, elementList);
				}
			}
			catch (Exception ex)
			{
				Trace.TraceError (ex.ToString ());
			}

			GlobalWaitThread.WaitThread.End ();
		}

		private void PDFViewer_OnCreateNewPage (object objValue)
		{
			DMTFrame currentFrame = MainCanvas?.CurrentDMTFrame as DMTFrame;

			if (null != currentFrame?.EBookManager)
			{
				currentFrame.EBookManager.AddNewPage ();
			}
		}

		#endregion |  ----- PDF분석기 -----  |

		#region |  ----- pptx 컨버전 -----  |
		/// <summary>
		/// pptx 컨버전
		/// </summary>
		private void ConvertOfficeDialog ()
		{
			System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog ();

			string strFilter = String.Concat ("Office 파일|*.pptx;*.docx|",
			   "프레젠테이션 파일 (*.pptx)|*.pptx|",
			   "문서파일 (*.docx)|*.docx|",
			   "All Files (*.*)|*.*");

			openFile.Title = "Office 파일 컨버트";
			openFile.CheckFileExists = true;
			openFile.Filter = strFilter;

			System.Windows.Forms.DialogResult result = openFile.ShowDialog ();
			if (System.Windows.Forms.DialogResult.OK == result)
			{
				var filePath = openFile.FileName;
				var fileName = Path.GetFileNameWithoutExtension (filePath);
				var fileExt = Path.GetExtension (filePath);

				GlobalWaitThread.WaitThread.Start ();

				OpenXmlPresentationHelper.Instance.OnCreateNewPage -= OpenXmlDocument_OnCreateNewPage;
				OpenXmlPresentationHelper.Instance.OnCreateNewPage += OpenXmlDocument_OnCreateNewPage;

				if (0 == string.CompareOrdinal (fileExt, ".pptx"))
					OpenXmlPresentationHelper.Instance.OpenPresentation (this.MainCanvas, filePath);
				else if (0 == string.CompareOrdinal (fileExt, ".docx"))
					OpenXmlWordHelper.Instance.OpenDocument (this.MainCanvas, filePath);

				DMTFrame currentFrame = MainCanvas?.CurrentDMTFrame as DMTFrame;
				if (null != currentFrame?.EBookManager)
				{
					currentFrame.EBookManager.MovePage (1);
				}

				GlobalWaitThread.WaitThread.End ();
			}
		}

		private void OpenXmlDocument_OnCreateNewPage (object objValue)
		{
			DMTFrame currentFrame = MainCanvas?.CurrentDMTFrame as DMTFrame;
			if (null != currentFrame?.EBookManager)
			{
				currentFrame.EBookManager.AddNewPage ();
			}
		}

		#endregion |  ----- pptx 컨버전 -----  |

		#region | 통합 문서 컨버전 |

		private void ConvertMultiOfficeDialog ()
		{
			var window = new MultiConverterWindow ();

			OpenXmlConvertManager.Instance.OnCreateNewPage -= OpenXmlDocument_OnCreateNewPage;
			OpenXmlConvertManager.Instance.OnCreateNewPage += OpenXmlDocument_OnCreateNewPage;

			window.Show ();
		}

		#endregion

		private void OnShowActionManager ()
		{
			//if (false != LicenseHelper.Instance.IsEnableSolutionService(SolutionService.AnimationManager))
			//{
			BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

			if (null != CurrentFrame)
			{
				TopView CurrentDMTView = CurrentFrame.GetCurrentView ();
				TopDoc CurrentDMTDoc = null == CurrentDMTView ? null : CurrentDMTView.GetDocument ();

				if (null != CurrentDMTDoc)
				{
					CurrentDMTDoc.OnToolActionManager ();
				}
			}
			//}
		}

		private void OnShowAIAdaptor ()
		{
			BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

			if (null != CurrentFrame)
			{
				TopView CurrentDMTView = CurrentFrame.GetCurrentView ();
				TopDoc CurrentDMTDoc = null == CurrentDMTView ? null : CurrentDMTView.GetDocument ();

				if (null != CurrentDMTDoc)
				{
					CurrentDMTDoc.OnToolAIAdaptor ();
				}
			}
		}

		private void OnShowAIAdaptorDBManager ()
		{
			BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

			if (null != CurrentFrame)
			{
				TopView CurrentDMTView = CurrentFrame.GetCurrentView ();
				TopDoc CurrentDMTDoc = null == CurrentDMTView ? null : CurrentDMTView.GetDocument ();

				if (null != CurrentDMTDoc)
				{
					CurrentDMTDoc.OnToolAIAdaptorDBManager ();
				}
			}
		}

		#region | CMS |

		private void ShowPrintExaminationWindow ()
		{
			PrintExaminationWindow window = new PrintExaminationWindow ();

			window.Owner = this;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			if (true == window.ShowDialog ())
			{
				ShowCMSWindow (window.DataContext as CMSInfo);
			}
		}

		private void ShowWebExaminationWindow()
		{
			WebExaminationWindow window = new WebExaminationWindow ();

			window.Owner = this;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			if (true == window.ShowDialog ())
			{
				ShowCMSWindow (window.DataContext as CMSInfo);
			}
		}

		private void ShowStudyExaminationWindow()
		{
			StudyExaminationWindow window = new StudyExaminationWindow ();

			window.Owner = this;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			if (true == window.ShowDialog ())
			{
				ShowCMSWindow (window.DataContext as CMSInfo);
			}
		}

		private void ShowAppWebServiceWindow ()
		{
			AppWebServiceWindow window = new AppWebServiceWindow ();

			window.Owner = this;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			if (true == window.ShowDialog ())
			{
				ShowCMSWindow (window.DataContext as CMSInfo);
			}
		}

		private void ShowCMSWindow (CMSInfo info)
		{
			info?.SetPageInfoValue ();

			QuizListWindow window = new QuizListWindow (info);

			window.Owner = this;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			window.Closed += Window_Closed;

			if (window.DataContext is QuizListWindowViewModel viewModel)
			{
				viewModel.OnNotifyOpenModel -= ViewModel_OnNotifyOpenModel;
				viewModel.OnNotifyOpenModel += ViewModel_OnNotifyOpenModel;
			}

			window.Show ();
		}

		private void Window_Closed (object sender, EventArgs e)
		{
			QuizListWindow window = sender as QuizListWindow;
			QuizListWindowViewModel viewModel = window.DataContext as QuizListWindowViewModel;
			List<ExaminationInfo> selectQuizData = viewModel.GetSelectQuizData ();

			if (!viewModel.IsCreated)
				return;

			var quizCodeList = selectQuizData.Select (i => i.QuizCode).ToList ();
			window.Info.QuizCodeJson = JsonConvert.SerializeObject (quizCodeList);

			//정답 조회
			var answerResult = EduTechServletManager.Instance.SelectCMSQuizMakerAnswer (quizCodeList, out string error);

			if (false == string.IsNullOrEmpty (error))
			{
				ToastMessge.Show (error);
				return;
			}

			Dictionary<int, object> answerMap = new Dictionary<int, object> ();

			if (null != answerResult)
			{
				for (int i = 0; i < answerResult.Rows.Count; i++)
				{
					DataRow row = answerResult.Rows[i] as DataRow;
					var answerJson = row["QC7"].ToString ();
					answerMap.Add (i, answerJson);
				}

				window.Info.AnswerList = JsonConvert.SerializeObject (answerMap);
			}

			var result = EduTechServletManager.Instance.InsertExamination (window.Info, selectQuizData, out string code);

			if (false == string.IsNullOrEmpty (result))
			{
				ToastMessge.Show (result);
			}
			else
			{
				ToastMessge.Show ("문제 출제에 성공했습니다.");
			}
		}

		private void ViewModel_OnNotifyOpenModel (ExaminationInfo examinationInfo)
		{
			if (null == examinationInfo)
			{
				PQAppBase.TraceDebugLog ("ViewModel_OnNotifyOpenModel null examinationInfo");
				return;
			}

			DataTable quizDataTable = EduTechServletManager.Instance.SelectQuizMakerData (examinationInfo?.QuizCode, out string error);

			QuizInfoDBNode dbInfo = new QuizInfoDBNode ();

			dbInfo.QuizIndexInfo = EduTechServletManager.Instance.ConvertDataTableToList<QuizIndexInfo> (quizDataTable)?.FirstOrDefault ();
			dbInfo.QuizContentInfo = EduTechServletManager.Instance.ConvertDataTableToList<QuizContentInfo> (quizDataTable)?.FirstOrDefault ();

			Uri uri =  new Uri (new Uri (PQAppBase.HttpURL), dbInfo.QuizContentInfo.PreviewImageUrl);

			if (null != _QuizMakerPreviewWindow)
			{
				_QuizMakerPreviewWindow.Close ();
			}

			_QuizMakerPreviewWindow =  new QuizMakerPreviewWindow (uri.ToString ());
			_QuizMakerPreviewWindow.Owner = this;
			_QuizMakerPreviewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			_QuizMakerPreviewWindow.Show ();
			

			//if (null != dbInfo.QuizIndexInfo && null != dbInfo.QuizContentInfo)
			//{
			//	MakeQuizMakerForDB (dbInfo);
			//}
		}

		/// <summary>
		/// CMS 리스트를 화면에 표시한다.
		/// </summary>
		private void ShowCMSListWindow ()
		{
			CMSListWindow window = new CMSListWindow ();
			window.Owner = this;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			if (true == window.ShowDialog () && window.DataContext is CMSListViewModel viewModel)
			{
				//생성 버튼을 눌러 종료한 경우에만 window.DialogResult가 true이다.
				var number = viewModel.SelectCMSNumber;

				MakeNewModel (DOC_KIND._docEBook);

				DMTFrame frame = MainCanvas.CurrentDMTFrame as DMTFrame;
				if (null != frame)
				{
					//고사 번호를 기준으로 QEB 생성하는 기능
					CMSGeneratorManager.Instnace.ExecuteGenerator (frame, number);
				}
			}
		}

		#endregion

		#endregion//도구 관련 처리

		#region |  ----- 설계도생성 / DB 관리자 -----  |
		#region |  void OnShowFieldSettingDialog()  |
		/// <summary>
		/// DB관리자
		/// </summary>
		private void OnShowFieldSettingDialog ()
		{
			ReportDMTFrame reportFrame = MainCanvas.CurrentDMTFrame as ReportDMTFrame;

			if (null != reportFrame) //출력물 모델인경우 별도 DB관리자 호출
			{
				reportFrame.ShowReportDBManager ();
			}
			else
			{
				if (null == m_dbFieldTree80)
				{
					// 2014-03-20-M01 : 3Tire에서 테이블 생성 가능하도록 수정함.
					int nIsHosting = RegLib.GetStrRegInfo (RegistryCoreX.ProductEnvPath + "ASSISTANCE", "IsHostingServer", 0);
					if (0 != nIsHosting || _PQRemoting.ServiceType == SERVICE_TYPE._3Tier)
					{
						PQAppBase.UseDatabaseX = true;
					}
					else
					{
						PQAppBase.UseDatabaseX = false;
					}

					m_dbFieldTree80 = new Softpower.SmartMaker.TopProcessEdit.DBManager80.DBFieldTreeWindow ();
					m_dbFieldTree80.CaptionIcon = MainMenu.AppImage.Source as BitmapImage;
					m_dbFieldTree80.Owner = this;
					m_dbFieldTree80.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
					m_dbFieldTree80.Closed += m_dbFieldTree80_Closed;
					m_dbFieldTree80.Show ();
				}

				m_dbFieldTree80.ActivateWindow ();
			}
		}

		#endregion

		private void OnShowHanaFieldSettingDialog ()
		{
			ReportDMTFrame reportFrame = MainCanvas.CurrentDMTFrame as ReportDMTFrame;

			if (null != reportFrame) //출력물 모델인경우 별도 DB관리자 호출
			{
				reportFrame.ShowReportDBManager ();
			}
			else
			{
				if (null == m_dbHanaFieldTree)
				{
					PQAppBase.UseDatabaseX = true;

					m_dbHanaFieldTree = new Softpower.SmartMaker.TopProcessEdit.DBManager80.DBFieldTreeWindow ();
					m_dbHanaFieldTree.CaptionIcon = MainMenu.AppImage.Source as BitmapImage;
					m_dbHanaFieldTree.Title = MainMenuDef.HanaManagerMenuItem;
					m_dbHanaFieldTree.Owner = this;
					m_dbHanaFieldTree.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
					m_dbHanaFieldTree.Closed += m_dbHanaFieldTree_Closed;
					m_dbHanaFieldTree.Show ();
				}

				m_dbHanaFieldTree.ActivateWindow ();
			}
		}

		private void OnShowQuizDBManager ()
		{
			if (null == m_EdutechQuizDBManager)
			{
				m_EdutechQuizDBManager = new EdutechQuizDBManager ();
				m_EdutechQuizDBManager.Owner = this;

				m_EdutechQuizDBManager.OnNotifyOpenModel -= EdutechQuizDBManager_OnNotifyOpenModel;
				m_EdutechQuizDBManager.OnNotifyOpenModel += EdutechQuizDBManager_OnNotifyOpenModel;

				m_EdutechQuizDBManager.Closed -= EdutechQuizDBManager_Closed;
				m_EdutechQuizDBManager.Closed += EdutechQuizDBManager_Closed;

				m_EdutechQuizDBManager.Show ();
			}
		}

		private void EdutechQuizDBManager_OnNotifyOpenModel (object objValue)
		{
			if (objValue is QuizInfoDBNode info)
			{
				MakeQuizMakerForDB (info);
			}
		}

		private void MakeQuizMakerForDB (QuizInfoDBNode info)
		{
			if (info.QuizIndexInfo.QuizType == nameof (QuizType.N15))
			{
				//일관형
				CreateQuizMakerMultiFrame (info);
			}
			else
			{
				CreateQuizMakerFrame (info);
			}
		}

		private void CreateQuizMakerMultiFrame (QuizInfoDBNode info)
		{
			MakeNewModel (DOC_KIND._docQuizMultiLayoutMaker);

			if (MainCanvas.CurrentDMTFrame is QuizMakerMultiFrame frame)
			{
				var document = frame.GetDocument () as QuizMakerMultiDoc;

				var quizIndexInfoJson = JsonConvert.SerializeObject (info.QuizIndexInfo);
				var quizContentInfoJson = JsonConvert.SerializeObject (info.QuizContentInfo);

				document.PageMetadata.QuizMetaData.QuizIndexInfo = JsonConvert.DeserializeObject<QuizIndexInfo> (quizIndexInfoJson);
				document.PageMetadata.QuizMetaData.QuizContentInfo = JsonConvert.DeserializeObject<QuizContentInfo> (quizContentInfoJson);
				document.PageMetadata.QuizMetaData.QuizAnswerNodeList = JsonConvert.DeserializeObject<List<EBookQuizAnswerNode>> (info.QuizContentInfo.AnswerValue);

				document.EndSerializeQuizMaker ();

				var dataTable = EduTechServletManager.Instance.SelectQuizMakerData (info.QuizContentInfo.QuizCode, out string error);

				if (string.IsNullOrEmpty (error))
				{
					var contentList = EduTechServletManager.Instance.ConvertDataTableToList<QuizContentInfo> (dataTable);

					frame.QuizMakerFrameCount = contentList.Count;
					frame.QuizMakerFrameList.Clear ();
					frame.RefreshQuizMakerFrame (false);

					for (int i = 0; i < contentList.Count; i++)
					{
						var subContentInfo = contentList[i];
						if (frame.QuizMakerFrameList[i] is QuizMakerFrame quizMakerFrame)
						{
							if (quizMakerFrame.CurrentDMTView.Document is QuizMakerDoc subQuizMakerDoc)
							{
								subQuizMakerDoc.LoadJsonDataString (subContentInfo.Format);
								subQuizMakerDoc.PageMetadata.QuizMetaData.QuizAnswerNodeList = JsonConvert.DeserializeObject<List<EBookQuizAnswerNode>> (subContentInfo.AnswerValue);

								subQuizMakerDoc.EndSerializeQuizMaker ();
							}
						}
					}

					frame.CurrentBaseScreenView.DataContext = document.PageMetadata.QuizMetaData;
					frame.SetUpdateMode ();
				}
			}
		}

		private void CreateQuizMakerFrame (QuizInfoDBNode info)
		{
			string json = info.QuizContentInfo.Format;
			MakeNewModel (DOC_KIND._docQuizLayoutMaker);

			var dmtFrame = MainCanvas.CurrentDMTFrame as QuizMakerFrame;

			if (null != dmtFrame)
			{
				var document = dmtFrame.GetDocument () as QuizMakerDoc;
				document.LoadJsonDataString (json);

				var quizIndexInfoJson = JsonConvert.SerializeObject (info.QuizIndexInfo);
				var quizContentInfoJson = JsonConvert.SerializeObject (info.QuizContentInfo);

				document.PageMetadata.QuizMetaData.QuizIndexInfo = JsonConvert.DeserializeObject<QuizIndexInfo> (quizIndexInfoJson);
				document.PageMetadata.QuizMetaData.QuizContentInfo = JsonConvert.DeserializeObject<QuizContentInfo> (quizContentInfoJson);
				document.PageMetadata.QuizMetaData.QuizAnswerNodeList = JsonConvert.DeserializeObject<List<EBookQuizAnswerNode>> (info.QuizContentInfo.AnswerValue);

				document.EndSerializeQuizMaker ();

				dmtFrame.CurrentBaseScreenView.DataContext = document.PageMetadata.QuizMetaData;
				dmtFrame.SetUpdateMode ();
			}
		}

		private void EdutechQuizDBManager_Closed (object sender, EventArgs e)
		{
			m_EdutechQuizDBManager = null;
		}


		#region |  void OnAutoErdCreator() : 설계도생성  |
		/// <summary>
		/// 설계도 생성
		/// </summary>
		private void OnAutoErdCreator ()
		{
			if (true == ErdAuto)
			{
				return;
			}

			ERDAutoGenWindow pErdAutoGenWindow = new ERDAutoGenWindow ();
			pErdAutoGenWindow.CaptionIcon = MainMenu.AppImage.Source as BitmapImage;
			pErdAutoGenWindow.Owner = this;
			pErdAutoGenWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			pErdAutoGenWindow.Closed += pErdAutoGenWindow_Closed;
			pErdAutoGenWindow.OnShowErdManager += pErdAutoGenWindow_OnShowErdManager;
			pErdAutoGenWindow.InitErdGenView ();
			pErdAutoGenWindow.Show ();
			pErdAutoGenWindow.IsExistDB ();
			pErdAutoGenWindow.Title = LC.GS ("TopDBManager80_ERDAutoGenWindow_2");
		}

		#endregion

		#region |  void FFlushDoc(DMTDoc pDoc) : 코드없음  |

		private void FFlushDoc (DMTDoc pDoc)
		{

			//throw new Exception("80 추후작업 폼 닫기 코드 요구됨");
			//80 추후작업 폼 닫기 코드 요구됨
		}

		#endregion

		#region |  string GetConnectionString()  |

		private string GetConnectionString ()
		{

			string strUserID = null == PQAppBase.UserLogin ? string.Empty : PQAppBase.UserLogin.TopCommUser;
			string strUserPWD = null == PQAppBase.UserLogin ? string.Empty : PQAppBase.UserLogin.TopCommPW;

			string strConnect = string.Empty;
			string strServerName = string.Empty;
			string strServerPath = string.Empty;
			string strOwner = string.Empty;
			int nDBKind = 0;

			if (null != PQAppBase.UserLogin && null != PQAppBase.UserLogin.LoginDepList && 1 < PQAppBase.UserLogin.LoginDepList.Count)
			{
				CDSInfo pDSInfo = PQAppBase.UserLogin.GetDataSourceInfo (1);
				if (null != pDSInfo)
				{
					strServerName = pDSInfo.ServerName;
					nDBKind = pDSInfo.DBKind;
					strOwner = pDSInfo.Owner;
				}
			}

			string strConn = string.Format ("{0};{1};{2};{3};{4};{5};{6}", strUserID, strUserPWD, strConnect, strServerName, nDBKind.ToString (), strServerPath, strOwner);
			return strConn;
		}

		#endregion
		#endregion//설계도생성 / DB 관리자

		#region |  ----- 배포 관련 -----  |

		#region | void OnPacking() |

		private void OnPacking (int nType)
		{
			if (false == PQAppBase.ConnectStatus)
			{
				LOGIN_RESULT _result = PQAppBase.UserLoginX ();
				if (LOGIN_RESULT.LOGIN_SUCCESS != _result)
				{
					return;
				}
			}

			if (2 == nType)
			{
				if (_PQRemoting.ServiceType == SERVICE_TYPE._Alone)
				{
					_Message80.Show ("윈도우 패킹 시 단독 시스템 운영 DB를 사용할 경우 DB 동작 오류가 발생할 수 있습니다.", "경고",
						System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
				}

				//윈도우 패킹 라이선스 체크
				if (false == PQAppBase.strTrial && false == LicenseHelper.Instance.IsEnableSolutionService (SolutionService.PackingExe))
				{
					return;
				}
			}

			Softpower.SmartMaker.AndroidPackage.PackageMainWindow packageMainWindow = new Softpower.SmartMaker.AndroidPackage.PackageMainWindow (nType);
			packageMainWindow.ShowInTaskbar = false;
			packageMainWindow.Owner = this;
			packageMainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

			// 2024.02.16 beh Android SDK 설치되지 않았을 때 패킹창 열리지 않도록 보강
			if (false == packageMainWindow.AndroidStudioDownloadCheck (this)) return;


			packageMainWindow.Show ();

		}

		#endregion

		#region |  void OnPacking80()  |
		/// <summary>
		/// 패키지생성하기80
		/// </summary>
		private void OnPacking80 ()
		{
			/*
			Softpower.SmartMaker.Android.Package.PackageWizard packageWizard = new Softpower.SmartMaker.Android.Package.PackageWizard();
			packageWizard.CaptionIcon = MainMenu.AppImage.Source as BitmapImage;
			packageWizard.Owner = this;
			packageWizard.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			packageWizard.ShowDialog();*/
		}

		#endregion

		#region | void OnPublish() |

		private void OnPublish (int nType)
		{
			Softpower.SmartMaker.AndroidPackage.UpLoadMainWindow upLoadMainWindow = new Softpower.SmartMaker.AndroidPackage.UpLoadMainWindow (nType);
			upLoadMainWindow.ShowInTaskbar = false;
			upLoadMainWindow.Owner = this;
			upLoadMainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			upLoadMainWindow.ShowDialog ();
		}

		#endregion

		#region |  void OnPublish80()  |
		/// <summary>
		/// 마켓 업로드80
		/// </summary>
		private void OnPublish80 ()
		{
			/*
			Softpower.SmartMaker.Android.Package.Page.Upload.UploadWizard deployWizard = new Softpower.SmartMaker.Android.Package.Page.Upload.UploadWizard();
			deployWizard.CaptionIcon = MainMenu.AppImage.Source as BitmapImage;
			deployWizard.Owner = this;
			deployWizard.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			deployWizard.ShowDialog();*/
		}

		#endregion

		private void StoreUpload (int nType)
		{
			Softpower.SmartMaker.AndroidPackage.OutSotreMainWindow storeMainWindow = new Softpower.SmartMaker.AndroidPackage.OutSotreMainWindow (nType);
			storeMainWindow.ShowInTaskbar = false;
			storeMainWindow.Owner = this;
			storeMainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			storeMainWindow.ShowDialog ();

		}

		#region |  void OnMarketList()  |

		private void OnMarketList ()
		{
			Softpower.SmartMaker.AndroidPackage.UpLoadListMainWindow upLoadListMainWindow = new Softpower.SmartMaker.AndroidPackage.UpLoadListMainWindow ();
			upLoadListMainWindow.ShowInTaskbar = false;
			upLoadListMainWindow.Owner = this;
			upLoadListMainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			upLoadListMainWindow.ShowDialog ();
		}

		#endregion

		#region |  void OnMarketList80()  |
		/// <summary>
		/// 마켓 업로드 리스트80
		/// </summary>
		private void OnMarketList80 ()
		{
			/*
			Softpower.SmartMaker.Android.Package.Page.UploadList.UploadListWizard listWizard = new Softpower.SmartMaker.Android.Package.Page.UploadList.UploadListWizard();
			listWizard.CaptionIcon = MainMenu.AppImage.Source as BitmapImage;
			listWizard.Owner = this;
			listWizard.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			listWizard.ShowDialog();*/
		}

		#endregion


		private void OnServerDeploy (int deployType) // 0 : 반응형웹, 1 : 적응형 웹, 2 Web App, 3 출력물 모델
		{
			if (!PQAppBase.ConnectStatus)
			{
				UserLoginX (false);
				return;
			}

			if (2 == deployType)
			{
				if (false == LicenseHelper.Instance.IsEnableSolutionService (SolutionService.WebApp))
					return;
			}
			else if (3 == deployType)
			{
				if (false == LicenseHelper.Instance.IsEnableSolutionService (SolutionService.ReportGenerator))
					return;
			}

			if (LicenseHelper.Instance.IsExpiryDay ())
			{
				if (false == PQAppBase.IsDeveloperMode)
				{
					if (SERVICE_TYPE._3Tier != _PQRemoting.ServiceType || false == PQAppBase.ConnectStatus)
					{
						_Message80.Show (LC.GS ("SmartOffice_MainWindow_23")); //3계층구조로 서버접속 해야합니다
						return;
					}

					if (true == PQAppBase.IsEduTechMode && 2 < PQAppBase.KissGetDocLevel ())
					{
						_Message80.Show (LC.GS ("배포 권한이 없습니다. 서버 관리자에게 문의하세요.")); //배포 권한이 없습니다.
						return;
					}
				}

				if (1 == PQAppBase.KissGetDocLevel () || true == _PQRemoting.UseJsonServer) //신서버는 권한논리가 없는듯?
				{
					//[리펙토링 임시]
					if (false != _PQRemoting.UseJsonServer)
					{
						if (PQAppBase.IsEduTechMode && PQAppBase.strTrial && -1 < PQAppBase.ServerFrontVersion.IndexOf ("v2", StringComparison.OrdinalIgnoreCase))
						{
							var deployWizard = new SmartGenerator.DeployGenerator.DeployGeneratorWizard (deployType);
							deployWizard.Owner = this;
							deployWizard.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
							deployWizard.ShowDialog ();
						}
						else
						{
							var deployWizard = new DeployGeneratorWizard (deployType);
							deployWizard.Owner = this;
							deployWizard.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
							deployWizard.ShowDialog ();
						}
					}
					else
					{
						var deployWizard = new DeployServerWizard ();
						deployWizard.Owner = this;
						deployWizard.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
						deployWizard.ShowDialog ();
					}
				}
				else
				{
					_Message80.Show (LC.GS ("SmartOffice_MainWindow_22")); //배포 권한이 없습니다.
				}
			}
			else
			{
				LicenseHelper.Instance.CheckExpiryDate ();
			}
		}

		private void OnServerDeployDirect ()
		{
			if (false != _PQRemoting.UseJsonServer)
			{
				List<LightJDoc> listDocument = new List<LightJDoc> ();

				List<Softpower.SmartMaker.TopControl.Components.Container.BaseFrame> frameCollecton = this.MainCanvas.GetAllFrame ();
				foreach (BaseFrame frame in frameCollecton)
				{
					LightJDoc doc = frame.GetCurrentView ().GetDocument () as LightJDoc;
					if (null != doc)
					{
						listDocument.Add (doc);
					}
				}

				//[리펙토링 임시]
				Softpower.SmartMaker.DeployGenerator.DeployGeneratorWizard deployWizard = new Softpower.SmartMaker.DeployGenerator.DeployGeneratorWizard (0);
				deployWizard.Owner = this;
				deployWizard.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
				deployWizard.ShowDeployDirectDialog (listDocument);
			}
		}

		private void OnServerDeployDirect (string strProjectName, bool bReDeploy)
		{
			DMTFrame frame = MainCanvas.CurrentDMTFrame as DMTFrame;

			if (null != frame)
			{
				DMTDoc document = frame.CurrentDMTView.Document as DMTDoc;
				//[리펙토링 임시]
				Softpower.SmartMaker.DeployGenerator.Manager.DeploySimulationManager.Instance.ShowDeployDirectPage (document, strProjectName, bReDeploy);
			}
		}

		private void OnRegeneration (string strProjectName)
		{
			//[리펙토링 임시]
			Softpower.SmartMaker.DeployGenerator.Manager.DeploySimulationManager.Instance.ExecuteRegenerationProject (strProjectName);
		}

		/// <summary>
		/// SAP런처 둥록
		/// </summary>
		private void OnServerDeploySapLaunchpad (string strTitle, string strServiceURL)
		{
			LaunchpadWindow launchpadWindow = new LaunchpadWindow ();
			launchpadWindow.GeneralTitle = strTitle;
			launchpadWindow.ServiceURL = strServiceURL;

			launchpadWindow.ShowDialog ();
		}

		#endregion//배포 관련

		#region |  void OnShowGlobalSettingWindow() : 환경  |

		private void OnShowGlobalSettingWindow (int nTabIndex)
		{

			//디버그에서 보안설정 X
			bool bHasLicense = false;
#if !DEBUG
            bHasLicense = 0 != PQAppBase.LicenseCompany.Length;
#endif
			TopManageWindow manageWindow = new TopManageWindow (bHasLicense);
			manageWindow.Owner = this;
			manageWindow.ShowDefaultSelectPage (nTabIndex);
			manageWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

			if (3 == nTabIndex)
				manageWindow.Title = MainMenuDef.ContentSecurityMenuItem;

			if (true == manageWindow.ShowDialog ())
			{
				if (0 == nTabIndex)
				{
					List<Softpower.SmartMaker.TopControl.Components.Container.BaseFrame> frameCollecton = this.MainCanvas.GetAllFrame ();

					foreach (BaseFrame frame in frameCollecton)
					{
						TopDoc doc = frame.GetCurrentView ().GetDocument ();
						if (null != doc)
						{
							doc.ResetFrameAttrib ();    // Registry ERD 정보..
						}
					}
				}

				//if (true == bHasLicense)
				{
					LicenseService.UpdateContentsPassword (PQAppBase.SecurityPass);
				}

				if (true == PQAppBase.IsAutoBackup)
				{
					AutoBackupProcessManager.Instance.Start ();
				}
				else
				{
					AutoBackupProcessManager.Instance.Stop ();
				}
			}
			else if (manageWindow.IsBuilderClose)
			{
				_Message80.Show (LC.GS ("SmartOffice_MainWindow_3"));
				this.Close ();
			}
		}

		#endregion

		#region |  void AddMdiChildForm(MDIForm mdiChildForm, MDIManager.MDI_CHILDFORM_STARTLOCATION startLocation) : MDI 관련  |

		private void AddMdiChildForm (MDIForm mdiChildForm, MDIManager.MDI_CHILDFORM_STARTLOCATION startLocation)
		{

			MDIManager.AddMdiChildForm (mdiChildForm, startLocation);
		}

		#endregion

		#region |  ----- 편집관련 -----  |
		#region |  void OnShowTileGuideConfigDialg()  |

		private void OnShowTileGuideConfigDialg ()
		{

			if (null != MainCanvas.CurrentDMTFrame)
			{
				//DMTFrame CurrentFrame = MainCanvas.CurrentDMTFrame as DMTFrame;
				//DMTView CurrentView = CurrentFrame.CurrentDMTView as DMTView;

				BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
				TopView CurrentView = CurrentFrame.GetCurrentView ();

				if (true == CurrentView.IsTileModeOn)
				{
					CurrentView.TileView.ClearTileGuide ();
				}
				else
				{
					ArrayList alParam = new ArrayList ();
					alParam.Add (CurrentView.TileView);
					m_MainWindowManager.ShowDialog (FlexibleDialogDefine.FlexibleDialogType.TileGuideConfig, true, alParam);
				}
			}
		}

		#endregion

		#region |  void OnShowRulerGuide()  |

		private void OnShowRulerGuide ()
		{

			if (null != MainCanvas.CurrentDMTFrame)
			{
				BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

				if (false == CurrentFrame.IsRulerModeOn)
				{
					CurrentFrame.InitRulerGuideView ();
				}
				else
				{
					CurrentFrame.ClearRulerGuideView ();
				}
			}
		}

		#endregion

		#region |  void OnShowFindAndReplaceDialog()  |

		private void OnShowFindAndReplaceDialog ()
		{
			if (null != MainCanvas.CurrentDMTFrame)
			{
				BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
				TopView CurrentView = CurrentFrame.GetCurrentView ();
				TopDoc CurrentDoc = CurrentView.GetDocument ();

				List<object> lstAtoms = CurrentView.GetAllAtomCoresObject ();

				// 2015-04-14 JAEYOUNG 이미 List에 모든 아톰정보를 담고 있어서 한번더 add 할필요없음.
				//List<CAtom> CurrentAtoms = new List<CAtom>();
				//foreach (CAtom atom in lstAtoms)
				//{
				//    CurrentAtoms.Add(atom);
				//}
				//lstAtoms.AddRange(CurrentAtoms);

				if (null != CurrentView)
				{
					ArrayList alParam = new ArrayList ();
					alParam.Add (lstAtoms);
					alParam.Add (CurrentDoc.GetDBMasterObject ());
					m_MainWindowManager.ShowDialog (FlexibleDialogDefine.FlexibleDialogType.FindAndReplace, false, alParam, CurrentFrame);
				}
			}
		}

		#endregion

		#region |  void OnShowContentSecurityDialog()  |
		private void OnShowContentSecurityDialog ()
		{
			if (false != LicenseHelper.Instance.IsEnableSolutionService (SolutionService.ContentSecurity))
			{
				OnShowGlobalSettingWindow (3);
			}
		}
		#endregion

		#region |  void ChangeAtomNameTextVisible()  |

		private void ChangeAtomNameTextVisible ()
		{

			if (null != MainCanvas.CurrentDMTFrame)
			{
				BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
				TopView CurrentView = CurrentFrame.GetCurrentView ();

				if (CurrentView != null)
				{
					CurrentView.ChangeAtomNameTextVisible ();
				}
			}
		}

		#endregion

		#region |  void OnToolSelect()  |
		/// <summary>
		/// 객체선택 모드 / 일반모드
		/// </summary>
		private void OnToolSelect ()
		{

			MainCanvas.OnToolSelect ();
		}

		#endregion

		#region |  void OnToolSpoit()  |
		/// <summary>
		/// 객체속성복사 모드 
		/// </summary>
		private void OnToolSpoit ()
		{

			MainCanvas.OnToolSpoit ();
		}

		#endregion

		#region |  void OnToolTabIndex()  |
		/// <summary>
		/// 탭인덱스 변경모드 
		/// </summary>
		private void OnToolTabIndex ()
		{

			MainCanvas.OnToolTabIndex ();
		}

		#endregion

		#region |  void OnToolCellIndex()  |
		/// <summary>
		/// 탭인덱스 변경모드 
		/// </summary>
		private void OnToolCellIndex ()
		{
			MainCanvas.OnToolCellIndex ();
		}

		#endregion


		private void OnShowUndo ()
		{
			if (null != MainCanvas.CurrentDMTFrame)
			{
				if (MainCanvas.CurrentDMTFrame is DMTFrame)
				{
					DMTView CurrentDMTView = MainCanvas.CurrentDMTFrame.GetCurrentView () as DMTView;
					if (null != CurrentDMTView)
					{
						CurrentDMTView.UndoHistory ();
					}
				}
				else if (MainCanvas.CurrentDMTFrame is ReportDMTFrame)
				{
					ReportDMTView reportView = MainCanvas.CurrentDMTFrame.GetCurrentView () as ReportDMTView;
					if (null != reportView)
					{
						reportView.Document.UndoHistory ();
					}
				}
			}
		}

		private void OnShowRedo ()
		{
			if (null != MainCanvas.CurrentDMTFrame)
			{
				if (MainCanvas.CurrentDMTFrame is DMTFrame)
				{
					DMTView CurrentDMTView = MainCanvas.CurrentDMTFrame.GetCurrentView () as DMTView;
					if (null != CurrentDMTView)
					{
						CurrentDMTView.RedoHistory ();
					}
				}
				else if (MainCanvas.CurrentDMTFrame is ReportDMTFrame)
				{
					ReportDMTView reportView = MainCanvas.CurrentDMTFrame.GetCurrentView () as ReportDMTView;
					if (null != reportView)
					{
						reportView.Document.RedoHistory ();
					}
				}
			}
		}

		#endregion//편집관련

		#region |  void OnShowVideoPlay() : 도움말  |

		private void OnShowVideoPlay ()
		{
			Visibility VideoPlayeVisivility = SubsidiaryWindow.Visibility;

			switch (VideoPlayeVisivility)
			{
				case Visibility.Visible:
					{
						_Registry.WriteValue ("ASSISTANCE", "IsShowVideoPlayer", false);

						if (System.Windows.WindowState.Minimized == SubsidiaryWindow.GetWindowState () ||
							0 != SubsidiaryWindow.GetTabIndex ())
						{
							SubsidiaryWindow.ShowPage (0);
						}
						else
						{
							//SubsidiaryWindow.Close (); 2020-03-06 kys 메이커스토어 창은 종료되면 안되기 때문에 주석처리함
						}

						break;
					}

				case Visibility.Collapsed:
					{
						SubsidiaryWindow.ShowPage (0);

						_Registry.WriteValue ("ASSISTANCE", "IsShowVideoPlayer", true);
						break;
					}

				default: break;
			}
		}

		#endregion

		#region | 폼 시뮬레이터 |
		/// <summary>
		/// 폼 시뮬레이터
		/// </summary>
		private void OnShowSimulator ()
		{
			//String strStorageUrl = @"https://aidtqalcmcdn.aiktbook.com/aidt/data/aidt/cct/test/BookCDN/page1.html";
			//CDNViewWindow view = new CDNViewWindow ("TITLE", strStorageUrl);
			//view.Show ();

			TopDoc pDoc = GetActiveDocument ();
			if (null != pDoc && pDoc is DMTDoc)
			{
				string strFilePath = pDoc.FilePath;
				if (false == string.IsNullOrEmpty (strFilePath))
				{
					SimulatorWindow.ShowPage (strFilePath);
				}
			}
			return;
		}
		#endregion

		#region | 폼 네비게이션 |
		/// <summary>
		/// 폼 네비게이션
		/// </summary>
		private void OnShowNavigationGraph ()
		{
			FileDBWindow fileDialog = new FileDBWindow (true, true, LC.GS ("AndroidPackage80_ProjectPage_2006_1"), LC.GS ("TopMenu_TopMenuWindow_35"));
			fileDialog.DocKind = DOC_KIND._docSmart;

			fileDialog.ShowDialog ();
			if (true == fileDialog.DialogResult)
			{
				Softpower.SmartMaker.Navigation.View.NavigationWindow window = new Softpower.SmartMaker.Navigation.View.NavigationWindow ();
				window.Owner = this;
				window.StartFilePath = fileDialog.FilePath;
				window.Show ();
			}
		}
		#endregion

		#region | 구글폰트 |
		/// <summary>
		/// 구글폰트 연동창 (작업중....)
		/// </summary>
		/// 
		private GoogleFontWindow GoogleFontWindow
		{
			get
			{
				if (null == m_GoogleFontWindow)
				{
					m_GoogleFontWindow = new GoogleFontWindow ();
					m_GoogleFontWindow.Owner = this;
					m_GoogleFontWindow.Closed += GoogleFontWindow_Closed;
					m_GoogleFontWindow.OnChangedFontFamily += GoogleFontWindow_OnChangedFontFamily;
				}
				return m_GoogleFontWindow;
			}
			set
			{
				m_GoogleFontWindow = value;
			}
		}

		private void GoogleFontWindow_Closed (object sender, EventArgs e)
		{
			m_GoogleFontWindow = null;
		}

		private void OnShowGoogleFontWindow ()
		{
			GoogleFontWindow.Show ();
		}

		private void GoogleFontWindow_OnChangedFontFamily (object objValue)
		{
			//
			//
		}
		#endregion

		#region | 페이지마스터 도구 |
		/// <summary>
		/// 페이지마스터 도구
		/// </summary>
		private void OnShowPageMasterWindow ()
		{
			SlideMasterHelper.Instance.ToggleSlideMaster ();
		}

		private void OnMakeSlideMasterModel (SlideMasterItemModel itemModel)
		{
			ModelDescription modelDescription = new ModelGenerator.Components.TabView.Model.ModelDescription ();
			modelDescription.DocKind = DOC_KIND._docSlideMaster;

			GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyMakeNewModel (modelDescription);

			DMTFrame currentDMTFrame = MainCanvas.CurrentDMTFrame as DMTFrame;
			if (null != currentDMTFrame)
			{
				currentDMTFrame.PageMasterManager.InitCategorySlideMaster (itemModel);
				currentDMTFrame.ToPageMasterMode (true);
				currentDMTFrame.PageMasterManager.PagerControl.IsExpanded = true;

				string strDefaultFormName = SlideMasterCategoryHelper.Instance.GetSlideMasterFileName (itemModel);

				LightJDoc document = currentDMTFrame.GetCurrentView ().GetDocument () as LightJDoc;
				if (null != document)
				{
					document.SetFormName (strDefaultFormName);
					document.SetFormTitle (strDefaultFormName);
					currentDMTFrame.ChangeCaptionText (strDefaultFormName);
				}
			}
		}

		#endregion
		#region | 모델파일 Active |
		private void ActiveModelFile (int nHashKey)
		{
			MainCanvas.ActiveModelFile (nHashKey);
		}
		#endregion

		#region | SAP RFC/ODATA |
		/// <summary>
		/// SAP BAPI Function 탐색기
		/// </summary>
		private void OnShowSAPFunctionNavigation ()
		{
			SapRfcHelper.Instance.MainCanvas = MainCanvas;
			SapRfcHelper.Instance.ShowSapFunction ();
		}

		/// <summary>
		/// SAP OData Service 탐색기
		/// </summary>
		private void OnShowSAPODataNavigation ()
		{
			SapODataHelper.Instance.MainCanvas = MainCanvas;
			SapODataHelper.Instance.ShowODataService ();
		}
		#endregion

		#region |  void OnGoQnACafe() : Cafe Q&A |

		private void OnGoQnACafe ()
		{
			string strUrl = @"https://cafe.naver.com/softmania";

			ProcessStartInfo pStart = new ProcessStartInfo (strUrl);
			Process.Start (pStart);
		}

		#endregion

		#region |  void ProcessScriptMenuClickedEvent(string strMenu)  |

		private void ProcessScriptMenuClickedEvent (string strMenu)
		{
			BaseFrame CurrentFrame = MainCanvas.CurrentDMTFrame as BaseFrame;
			TopDoc CurrentDoc = null == CurrentFrame.GetCurrentView () ? null : CurrentFrame.GetCurrentView ().GetDocument ();

			if (null == CurrentDoc)
			{
				return;
			}

			Softpower.SmartMaker.TopEdit80.CScrFrame ScriptFrame = CurrentDoc.GetScriptWindowObject () as Softpower.SmartMaker.TopEdit80.CScrFrame;
			if (null == ScriptFrame)
			{
				return;
			}

			if (ScriptMenuDefine.Compile == strMenu)
			{
				ScriptFrame.StartCompile ();
			}
			else if (ScriptMenuDefine.Find == strMenu)
			{
				ScriptFrame.OnScrSearch ();
			}
			else if (ScriptMenuDefine.Replace == strMenu)
			{
				ScriptFrame.OnScrReplace ();
			}
			else if (ScriptMenuDefine.OperatingEnvironment == strMenu)
			{
				ScriptFrame.OnScrEnv ();
			}
			else if (ScriptMenuDefine.HelpString == strMenu)
			{
				OnShowLinkHelp ();
			}
		}

		#endregion

		#region |  void OnCloseMainWindow()  |
		/// <summary>
		/// 메인 윈도우 닫을때 처리
		/// </summary>
		private void OnCloseMainWindow ()
		{
			bool bCloseMainWindow = true;

			if (LC.LANG.ENGLISH == LC.PQLanguage)
			{
				Softpower.SmartMaker.TopApp.PopupHelp.HelpPopupDocumentManager.Instance.SaveXml ();
			}

			//DMTFrame
			foreach (object objFrame in MainCanvas.GetAllFrame ())
			{
				if (objFrame is DMTFrame)
				{
					DMTFrame CurrentFrame = objFrame as DMTFrame;

					if (false == CurrentFrame.OnClosing (true))
					{
						bCloseMainWindow = false;
						break;
					}
				}
			}

			//RulerData
			Softpower.SmartMaker.TopApp.Ruler.RulerDataManager.Save ();

			//공휴일 정보
			TopAtom.Components.CalendarAtom.HolidayManager.Instance.Save ();

			//Menu
			foreach (CTopMenuView menuView in ExecuteMenuManager.Instance.MenuViewList)
			{
				if (true == menuView.OnWindowClosing ())
				{
					bCloseMainWindow = false;
					break;
				}
			}

			//패킹 자바 데몬 종료
			CloseJavaDaemon ();

			//임시폴더 제거
			CloseEBookTempFolder ();

			if (true == bCloseMainWindow)
			{
				GoogleAnalyticsGAHelper.Instance.EndAnalytics ();

				if (this.WindowState != WindowState.Minimized && this.WindowState != WindowState.Maximized)
				{
					_Registry.WriteValue ("Environment\\PROCESS", "Width", this.Width);
					_Registry.WriteValue ("Environment\\PROCESS", "Height", this.Height);
				}

				_Registry.WriteValue ("Environment\\PROCESS", "Left", this.Left);
				_Registry.WriteValue ("Environment\\PROCESS", "Top", this.Top);

				_Registry.WriteValue ("Environment\\PROCESS", "WindowState", (int)this.WindowState);

				//
				int nScreen = 0;
				foreach (var screen in System.Windows.Forms.Screen.AllScreens)
				{
					if (screen.Bounds.Left < this.Left && this.Left < screen.Bounds.Left + screen.Bounds.Width
						&& screen.Bounds.Top < this.Top && this.Top < screen.Bounds.Top + screen.Bounds.Height)
					{
						break;
					}
					nScreen++;
				}

				_Registry.WriteValue ("Environment\\PROCESS", "ScreenNum", nScreen);
				//

				//메이커스토어 상태 저장
				if (null != m_SubsidiaryWindow)
					_Registry.WriteValue ("Environment\\PROCESS", "MakeStoreState", (int)m_SubsidiaryWindow.GetWindowState ());

				//자동 백업 프로세스 종료
				AutoBackupProcessManager.Instance.CloseProcess ();

				PrivateFontManager.Instance.TaskRemoveFontRegistry ();

				this.Close ();
			}
		}

		#endregion

		private void CloseJavaDaemon ()
		{
			if (false == PQAppBase.strTrial)
				return;

			int nDaemonPacknig = _Kiss.toInt32 (_Registry.ReadValue ("ASSISTANCE", "DaemonPacknig", 0));

			if (1 != nDaemonPacknig)
				return;

			//ConsoleExtension.Hide ();
			var searcher = new System.Management.ManagementObjectSearcher ("select * from Win32_Process where name='java.exe'");
			var collection = searcher.Get ();

			foreach (System.Management.ManagementObject managementObject in collection)
			{
				object commandLine = managementObject["CommandLine"];
				if (null != commandLine)
				{
					string strCommandLine = commandLine.ToString ();
					if (-1 < strCommandLine.IndexOf ("org.gradle.launcher.daemon.boot", StringComparison.OrdinalIgnoreCase) &&
						-1 < strCommandLine.IndexOf ("gradle-launcher-6.7.1.jar", StringComparison.OrdinalIgnoreCase)
						)
					{
						int processID = _Kiss.toInt32 (managementObject["handle"]);
						Process javaDeamon = Process.GetProcessById (processID);
						javaDeamon.Kill ();
						//managementObject.InvokeMethod ("Terminate", null);
					}
				}
			}

		}

		/// <summary>
		/// 프로그램 종료시 북모델 임시저장 파일들 제거
		/// </summary>
		private void CloseEBookTempFolder ()
		{
			var folder = Path.Combine (PQAppBase.BaseModulePath, "__BookTemp");
			var folder2 = Path.Combine (PQAppBase.BaseModulePath, "__Temp");

			if (true == Directory.Exists (folder))
			{
				Directory.Delete (folder, true);
			}

			if (true == Directory.Exists (folder2))
			{
				Directory.Delete (folder2, true);
			}
		}

		#region |  void InitHardwareacceleration()  |

		private void InitHardwareacceleration ()
		{

			var hwndSource = PresentationSource.FromVisual (this) as HwndSource;
			if (hwndSource != null)
			{
				var hwndTarget = hwndSource.CompositionTarget;
				if (hwndTarget != null) hwndTarget.RenderMode = RenderMode.SoftwareOnly;
			}
		}

		#endregion

		#region |  void UpdateToolBarFontFamilyAddSelectFont() : 글자툴바 재정비 및 현재 글꼴 저장  |
		//2014-11-19-M01 글자툴바 재정비 및 현재 글꼴 저장
		private void UpdateToolBarFontFamilyAddSelectFont ()
		{

			MainToolBarPanel.UpdateToolBarFontFamilyAddSelectFont ();
		}

		#endregion

		#region |  void CurrentDMTViewFocused(DependencyObject scope) : 현재 DMTView에 포커스 주기 ( 포커스 아웃 )  |
		//2014-11-27-M02 현재 DMTView에 포커스 주기 ( 포커스 아웃 )
		private void CurrentDMTViewFocused (DependencyObject scope)
		{

			Keyboard.ClearFocus ();
			FocusManager.SetFocusedElement (scope, this as IInputElement);
		}

		#endregion

		#region |  void SetPageEnvironment(int CommandCode)  |

		private void SetPageEnvironment (int CommandCode)
		{

			if (null != MainCanvas.CurrentDMTFrame)
			{
				MainCanvas.CurrentDMTFrame.ProcessMenuCommand (CommandCode);
			}
		}

		#endregion

		#region |  void OnShowSectorManager()  |

		private void OnShowSectorManager ()
		{

			//PQAppBase.IsSmartBuilder = false;

			string strFile = GetExistOrganFile (LC.GS ("SmartOffice_MainWindow_1808_7"));
			if (strFile.Length > 0)
			{
				//ExecuteOrganFile(strFile);
				FileOpen (strFile);
			}

			//PQAppBase.IsSmartBuilder = true;

			MainToolBarPanel_OnToolBarItemClickedEvent (0);
		}

		#endregion

		#region |  void OnShowDepartmentManager()  |

		private void OnShowDepartmentManager ()
		{

			//PQAppBase.IsSmartBuilder = false;

			string strFile = GetExistOrganFile (LC.GS ("SmartOffice_MainWindow_1808_8"));
			if (strFile.Length > 0)
			{
				//ExecuteOrganFile(strFile);
				FileOpen (strFile);
			}

			//PQAppBase.IsSmartBuilder = true;

			MainToolBarPanel_OnToolBarItemClickedEvent (0);
		}

		#endregion

		#region |  void OnShowUserManager()  |

		private void OnShowUserManager ()
		{

			if (false == PQAppBase.IsEduTechMode)
			{
				//PQAppBase.IsSmartBuilder = false;

				string strFile = GetExistOrganFile (LC.GS ("SmartOffice_MainWindow_1808_9"));
				if (strFile.Length > 0)
				{
					//ExecuteOrganFile(strFile);
					FileOpen (strFile);
				}

				//PQAppBase.IsSmartBuilder = true;

				MainToolBarPanel_OnToolBarItemClickedEvent (0);
			}
			else
			{
				OnShowAcademyUserManager ();
			}
		}

		#endregion


		#region |  void OnShowAcademyManager()  |

		private void OnShowAcademyUserManager ()
		{
			//에듀태크 사용자정보 설정 다이얼로그
			if (1 != PQAppBase.KissGetDocLevel ())
			{
				ToastMessge.Show ("관리자 계정으로 로그인 해야만 사용 가능합니다.");
				return;
			}

			if (1 == PQAppBase.CompanyCode)
			{
				ToastMessge.Show ("현재 스마트서버 버전에서는 지원하지 않은 기능입니다.");
				return;
			}

			AcademyUserManagerWindow window = new AcademyUserManagerWindow ();
			window.Owner = this;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.Show ();
		}

		#endregion


		#region |  void OnShowAcademyManager()  |

		private void OnShowAcademyManager ()
		{
			//에듀태크 소속기관 설정 다이얼로그
			if (1 != PQAppBase.KissGetDocLevel ())
			{
				ToastMessge.Show ("관리자 계정으로 로그인 해야만 사용 가능합니다.");
				return;
			}

			if (1 == PQAppBase.CompanyCode)
			{
				ToastMessge.Show ("현재 스마트서버 버전에서는 지원하지 않은 기능입니다.");
				return;
			}

			AcademyManagerWindow window = new AcademyManagerWindow ();
			window.Owner = this;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.Show ();
		}

		#endregion

		#region |  string GetExistOrganFile(string strFileName)  |

		private string GetExistOrganFile (string strFileName)
		{

			string strPath = PQAppBase.ModulePath;
			if (strPath.Length > 0)
			{
				strPath = strPath + "\\" + strFileName;

				System.IO.FileInfo info = new System.IO.FileInfo (strPath);
				if (false != info.Exists)
				{
					return strPath;
				}
				else
				{
					_Message80.Show (LC.GS ("SmartOffice_MainWindow_1808_10"));
				}
			}

			return string.Empty;
		}

		#endregion

		#region |  string GetPlainTextFromHtml(string htmlString)  |

		private string GetPlainTextFromHtml (string htmlString)
		{

			int tagStartIndex = htmlString.IndexOf ('<');

			if (tagStartIndex > 0)
			{
				htmlString = htmlString.Substring (tagStartIndex, htmlString.Length - tagStartIndex);
			}
			string htmlTagPattern = "<.*?>";
			var regexCss = new Regex ("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds (3000));
			htmlString = regexCss.Replace (htmlString, string.Empty);
			htmlString = Regex.Replace (htmlString, htmlTagPattern, string.Empty, RegexOptions.None, TimeSpan.FromMilliseconds (3000));
			htmlString = Regex.Replace (htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline, TimeSpan.FromMilliseconds (3000));
			htmlString = htmlString.Replace ("&nbsp;", string.Empty);

			return htmlString;
		}

		#endregion


		#region |  ##### Protected Virtual 메서드 #####  |

		#region |  bool LicenseCheck() : 라이선스 처리 관련  |

		protected virtual bool LicenseCheck ()
		{
			bool bResult = true;

			if (true == PQAppBase.strTrial && true == LicenseHelper.Instance.FreeLicense && true == Debugger.IsAttached)
			{
				return true;
			}

			if (false != AcademyService.Enabled)
			{
				LicenseService.AcademyID = AcademyService.AcademyID;
				LicenseService.AcademyServer = AcademyService.AcademyServer;
			}

			bool bEnable = LicenseService.IsEnableLicense (PQAppBase.CurrentProductCode);
			if (false == bEnable)
			{
				LogManager.WriteLog (LogType.Error, "LicenseCheck false");
				LogManager.WriteLog (LogType.Error, LicenseService.Description);
				//_Message80.Show(LC.GS("SmartOffice_MainWindow_1"));

				if (string.IsNullOrEmpty (LicenseService.Description))
				{
					_Message80.Show (LC.GS ("SmartOffice_MainWindow_1808_11"), LC.GS ("SmartOffice_MainWindow_1808_12"));
				}
				else
				{
					_Message80.Show (LicenseService.Description);
				}

				bResult = false;
			}
			else
			{
				string strCode = LicenseService.GetLicenseControl ();

				if (false != strCode.Equals ("P"))
				{
					_Message80.Show (LicenseService.Description);
				}

				// 자동업데이트 오동작 파일 제거

				string[] strFiles = { "SDe06.dll", "SDb01.dll" };
				foreach (string strFile in strFiles)
				{
					string strTempFile = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, strFile);
					if (false != File.Exists (strTempFile))
					{
						File.Delete (strTempFile);
					}
				}

				//

				// 클라우드 서버 환경설정
				CloudService.InitCloudService ();
				//

				// MariaDB 서비스체크
				MariaDBService maridDBService = new MariaDBService ();
				maridDBService.CheckMariaDBService ();
				//

				// 190904_AHN - MessagePush 서비스체크
				// - 서비스 등록 또는 시작, 트레이 아이콘 시작 프로세스 등록
				if (LC.PQLanguage == LC.LANG.KOREAN)
				{
					if (false == PQAppBase.IsEduTechMode)
					{
						MessagePushService messagePushService = new MessagePushService ();
						messagePushService.deleteService ();
						messagePushService.CheckMessagePushService (MessagePushService.RUN_MODE, MessagePushService.SERVICE_INSTALL);
						messagePushService.SetStartProcess ();
					}
				}
				//

				this.ImageCanvas.InvalidateVisual ();

				EduTechLicenseHelper.Instance.InitEduTechDefaultValue ();

				GoogleAnalyticsGAHelper.Instance.StartAnalytics ();
			}

			return bResult;
		}

		#endregion

		#endregion//Protected Virtual 메서드


		#region |  ##### Protected 메서드 #####  |

		#region |  DOC_KIND GetActiveDocKind()  |

		protected DOC_KIND GetActiveDocKind ()
		{
			DOC_KIND docKind = DOC_KIND._docNone;

			WindowCollection childWindows = this.OwnedWindows;
			foreach (Window cWindow in childWindows)
			{
				if (cWindow is CScrFrame && true == cWindow.IsActive)
				{
					CScrFrame scrFrame = cWindow as CScrFrame;
					docKind = scrFrame.GetDocKind ();
					break;
				}
			}

			if (docKind == DOC_KIND._docNone)
			{
				BaseFrame currentFrame = this.MainCanvas.CurrentDMTFrame as BaseFrame;
				if (null != currentFrame)
				{
					TopView currentView = currentFrame.GetCurrentView ();
					if (null != currentView)
					{
						TopDoc pDoc = currentView.GetDocument ();
						if (null != pDoc)
						{
							docKind = pDoc.DocType;
						}
					}
				}
			}

			return docKind;
		}

		#endregion

		#region |  ----- 파일저장 처리관련 -----  |
		#region |  string GetRealFileName(string strFileName)  |

		protected string GetRealFileName (string strFileName)
		{
			string strAllScript = " - " + LC.GS ("SmartOffice_MainWindow_12");
			string strSP = " - " + LC.GS ("SmartOffice_MainWindow_5");
			string strScript = " - " + LC.GS ("SmartOffice_MainWindow_15");

			strFileName = strFileName.Replace (strAllScript, "");
			strFileName = strFileName.Replace (strScript, "");
			strFileName = strFileName.Replace (strSP, "");
			return strFileName;
		}

		#endregion
		#endregion//파일저장 처리관련

		#endregion//Protected 메서드


		#region |  ##### 이벤트 #####  |

		#region |  void SingleInstanceApplication_NewInstanceMessage(object sender, object message)  |
		/// <summary>
		/// 2015.04.22
		/// 파라미터에 의한 파일 실행. InitializeMainWindowEvents메서드에서 이벤트 핸들러 추가
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="message"></param>
		private void SingleInstanceApplication_NewInstanceMessage (object sender, object message)
		{

			string strFilePath = message as string;

			if (false == string.IsNullOrEmpty (strFilePath))
			{
				ExploreFile = strFilePath;
				this.Activate ();

				if (this.WindowState == WindowState.Minimized)
					this.WindowState = WindowState.Maximized;
			}
		}

		#endregion

		#region |  void MainWindow_Activated(object sender, EventArgs e)  |
		/// <summary>
		/// 2015.04.22
		/// 파라미터로 전달받은 파일경로를 Open
		/// OnLoad 에서는 시각화가 되어있지 않은 상태에서 View 에 AdornerLayer.GetAdornerLayer(this); 호출시 null을 발생하여 
		/// Activated 에서 파일 Open 처리. InitializeMainWindowEvents 메서드에서 이벤트 핸들러 추가
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MainWindow_Activated (object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty (ExploreFile))
			{
				this.Dispatcher.BeginInvoke (new Action (delegate
				{
					string strFileName = ExploreFile;
					ExploreFile = string.Empty;

					if (this.FileOpen (strFileName))
					{
						m_MainWindowManager.AddRecentFile (strFileName);
					}
				}));
			}
		}

		#endregion

		#region |  void MainWindow_StateChanged(object sender, EventArgs e)  |
		/// <summary>
		/// InitializeMainWindowEvents 메서드에서 이벤트 핸들러 추가.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MainWindow_StateChanged (object sender, EventArgs e)
		{
			switch (this.WindowState)
			{
				case System.Windows.WindowState.Maximized:
					BorderThickness = new Thickness (6);
					break;
				default:
					BorderThickness = new Thickness (1);
					break;
			}

			this.MainMenu.UpdateWindowState (this.WindowState);
		}

		#endregion

		#region |  void MainWindow_Deactivated(object sender, EventArgs e)  |
		/// <summary>
		/// InitializeMainWindowEvents 메서드에서 이벤트 핸들러 추가.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MainWindow_Deactivated (object sender, EventArgs e)
		{
			MainToolBarPanel.CloseAllToolBarPopup ();
		}

		#endregion

		#region |  ----- 상단 툴바에 대한 이벤트 -----  |
		#region |  void MainToolBarPanel_OnToolBarItemClickedEvent(int nValue)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 등록.
		/// MainToolBarPanel.OnToolBarItemClickedEvent 호출.
		/// </summary>
		/// <param name="nValue"></param>
		private void MainToolBarPanel_OnToolBarItemClickedEvent (int nValue)
		{
			//DMTFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame;
			BaseFrame baseFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

			switch (nValue)
			{
				//Run
				case 0:
					{

						if (null == baseFrame || baseFrame.IsModalMode)
							return;

						//GlobalWaitThread.WaitThread.Start();
						//Thread.Sleep(300);

						CloseAllPopup ();
						MainCanvas.ChangeFormModeButtonClick ();

						if (null != baseFrame)
						{
							FormMode.FrameMode CurrentFrameMode = baseFrame.GetFormMode ();
							//DMTView CurrentDMTView = CurrentDMTFrame.CurrentDMTView;
							TopView CurrentDMTView = baseFrame.GetCurrentView ();

							if (FormMode.FrameMode.Executed != CurrentFrameMode && Visibility.Hidden == CurrentDMTView.Visibility)
							{

								if (baseFrame is DMTFrame)
								{
									DMTFrame dmtFrame = baseFrame as DMTFrame;
									dmtFrame.CloseSmartApprovalView ();
								}

								ShowDMTView ();
							}
						}

						//GlobalWaitThread.WaitThread.End();
						break;
					}

				//Select
				case 1:
					{
						OnToolSelect ();
						break;
					}

				//Lock
				case 2:
					{
						MainCanvas.OnToolLock ();
						break;
					}

				//LoadFile
				case 3:
					{
						OnFileOpen ();
						break;
					}

				//SaveFile
				case 4:
					{
						OnFileSave ();
						break;
					}

				//Copy
				case 5:
					{
						OnAtomCopy ();
						break;
					}

				//Paste
				case 6:
					{
						OnAtomPaste ();
						break;
					}

				//Spoit
				case 7:
					{
						OnToolSpoit ();
						break;
					}

				//TabIndex
				case 8:
					{
						OnToolTabIndex ();
						break;
					}
				//Ruler
				case 9:
					{
						OnShowRulerGuide ();
						break;
					}

				//TileGrid
				case 10:
					{
						OnShowTileGuideConfigDialg ();
						break;
					}
				//Undo
				case 11:
					{
						OnShowUndo ();
					}
					break;
				//Redo
				case 12:
					{
						OnShowRedo ();
					}
					break;
				case 13:
					{
						OnToolCellIndex ();
						break;
					}
				default: break;
			}

			//실행 메뉴 활성비활성
			//메뉴MainToolBarPanel.UpdateToolBarEnable(CurrentDMTFrame);
			UpdateMenuStateAll (baseFrame);

			GlobalWaitThread.WaitThread.End ();
		}

		#endregion

		#region |  bool MainToolBarPanel_CommonEvent(DelegateStructType.EventSourceType EventSourceType, int nEventKey, object value)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 등록.
		/// MainToolBarPanel.CommonEvent 호출.
		/// </summary>
		/// <param name="EventSourceType"></param>
		/// <param name="nEventKey"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private bool MainToolBarPanel_CommonEvent (DelegateStructType.EventSourceType EventSourceType, int nEventKey, object value)
		{

			return MainCanvas.HandleCommonEvent (EventSourceType, nEventKey, value);
		}

		#endregion
		#endregion//상단 툴바에 대한 이벤트

		#region |  ----- 하단 상태바에 대한 이벤트 -----  |
		#region |  void MainStatusBar_OnNotifyScaleValueEvent(double dValue)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 등록.
		/// MainStatusBar.OnNotifyScaleValueEvent 호출.
		/// </summary>
		/// <param name="dValue"></param>
		private void MainStatusBar_OnNotifyScaleValueEvent (double dValue, bool IsInitial = false)
		{
			MainCanvas.ChangeScreenScale (dValue);

			MainCanvas.ChangeScreenScaleCenter (MainScrollViewer);
		}

		#endregion

		#region |  double MainStatusBar_OnGetPhysicalScaleValueEvent()  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 등록.
		/// MainStatusBar.OnGetPhysicalScaleValueEvent 호출.
		/// </summary>
		/// <returns></returns>
		private double MainStatusBar_OnGetPhysicalScaleValueEvent ()
		{

			return MainCanvas.GetPhysicalScaleValue ();
		}

		#endregion
		#endregion//하단 상태바에 대한 이벤트

		#region |  ----- 중앙작업 캔버스에 대한 이벤트 -----  |
		#region |  void MainCanvas_OnNotifyToolBarNoCurrentSelectedAtomEvent(bool bValue)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가
		/// MainCanvas.OnNotifyToolBarNoCurrentSelectedAtomEvent 호출.
		/// </summary>
		/// <param name="bValue"></param>
		void MainCanvas_OnNotifyToolBarNoCurrentSelectedAtomEvent (bool bValue)
		{

			MainToolBarPanel.ChangeToolbarNoSelect (bValue);
		}

		#endregion

		#region |  void MainCanvas_OnNotifyToolBarRunModeCurrentSelectedAtomEvent(bool bValue) : 코드없음  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.OnNotifyToolBarRunModeCurrentSelectedAtomEvent 호출.
		/// </summary>
		/// <param name="bValue"></param>
		private void MainCanvas_OnNotifyToolBarRunModeCurrentSelectedAtomEvent (bool bValue)
		{
		}

		#endregion

		#region |  void MainCanvas_OnNotifyToolBarLockCurrentSelectedAtomEvent(bool bValue)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.OnNotifyToolBarLockCurrentSelectedAtomEvent 호출.
		/// </summary>
		/// <param name="bValue"></param>
		private void MainCanvas_OnNotifyToolBarLockCurrentSelectedAtomEvent (bool bValue)
		{
			MainToolBarPanel.ChangeToolbarLockMode (bValue);
		}

		#endregion

		#region |  void MainCanvas_ShowAtomContextMenuEvent(ArrayList alParameterList)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.ShowAtomContextMenuEvent 호출.
		/// </summary>
		/// <param name="alParameterList"></param>
		private void MainCanvas_ShowAtomContextMenuEvent (ArrayList alParameterList)
		{
			Point currentPoint = Mouse.GetPosition (this);
			Point ptMouse = PointToScreen (currentPoint);

			MainAtomContextMenuPanel.Child = null;
			SmartContextMenu commonSmartContextMenu = SmartContextMenuManager.GetAtomContextMenu (alParameterList);
			commonSmartContextMenu.ShowAttPageEvent += MainCanvas_ShowAttPageEvent;

			MainAtomContextMenuPanel.Child = commonSmartContextMenu;

			MainAtomContextMenuPanel.Closed -= MainAtomContextMenuPanel_Closed;
			MainAtomContextMenuPanel.Closed += MainAtomContextMenuPanel_Closed;

			commonSmartContextMenu.Visibility = System.Windows.Visibility.Visible;

			MainAtomContextMenuOpen (true);

			if (commonSmartContextMenu.ContextChild is BaseContextMenuGrid)
			{
				Size PopupSize = MainAtomContextMenuPanel.Child.DesiredSize;
				(commonSmartContextMenu.ContextChild as BaseContextMenuGrid).GridColChange (
					(commonSmartContextMenu.ContextChild as BaseContextMenuGrid).SetLocation (ptMouse, PopupSize));
			}

			RelocationAtomContextMenu (currentPoint);
		}

		private void MainAtomContextMenuPanel_Closed (object sender, EventArgs e)
		{
			// StaysOpen = flase 일경우, PreviewMouseDown 이벤트가 발생하지 않아,
			// AtomEditListArea 에서만 StaysOpen = flase 로 변경
			//
			MainAtomContextMenuPanel.StaysOpen = true;
		}

		#endregion

		#region |  void MainCanvas_ShowEmbeddedAtomContextMenuEvent(ArrayList alDataList)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.ShowEmbeddedAtomContextMenuEvent 호출
		/// </summary>
		/// <param name="alDataList"></param>
		private void MainCanvas_ShowEmbeddedAtomContextMenuEvent (ArrayList alDataList)
		{
			Trace.TraceInformation ("주석처리함 추후 확인 필요	: MainCanvas_ShowEmbeddedAtomContextMenuEvent");
			//Point currentPoint = Mouse.GetPosition(this);
			//MainAtomContextMenuPanel.Child = null;
			//SmartContextMenu commonSmartContextMenu = SmartContextMenuManager.GetEmbeddedAtomContextMenu(alDataList);
			//commonSmartContextMenu.ShowAttPageEvent += MainCanvas_ShowAttPageEvent;

			//MainAtomContextMenuPanel.Child = commonSmartContextMenu;
			//commonSmartContextMenu.Visibility = System.Windows.Visibility.Visible;
			//MainAtomContextMenuOpen(true);

			//RelocationAtomContextMenu(currentPoint);
		}

		#endregion

		#region |  void MainCanvas_NotifyAppAtomMenuEnableEvent(Object formMode)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.OnNotifyAppAtomMenuEnableEvent 호출
		/// </summary>
		/// <param name="formMode"></param>
		private void MainCanvas_NotifyAppAtomMenuEnableEvent (Object formMode)
		{
			MainExpandMenuContainer.UpdateMenuState (formMode as BaseFrame);
		}

		#endregion

		#region |  void MainCanvas_OnNotifyScaleOfFocusedScreenEvent(double dValue)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.OnNotifyScaleOfFocusedScreenEvent 호출
		/// </summary>
		/// <param name="dValue"></param>
		private void MainCanvas_OnNotifyScaleOfFocusedScreenEvent (double dValue)
		{

			MainStatusBar.UpdateScaleSliderValue (dValue);
		}

		#endregion

		#region |  void MainCanvas_OnNotifyScaleOfFocusedScreenNoResizeEvent(double dValue)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.OnNotifyScaleOfFocusedScreenNoResizeEvent 호출
		/// </summary>
		/// <param name="dValue"></param>
		private void MainCanvas_OnNotifyScaleOfFocusedScreenNoResizeEvent (double dValue)
		{
			MainStatusBar.UpdateScaleSliderValueNoResize (dValue);
		}

		#endregion

		private void MainCanvas_OnNotifyActiveCurrentDMTFrameEvent (object objValue)
		{
			DMTFrame currentFrame = objValue as DMTFrame;
			if (null != currentFrame)
			{
				RefreshScriptTrace ();
				RefreshLRSTrace ();
			}
		}

		#region |  void MainCanvas_OnNotifyToolBarAboutCurrentSelectedViewPropertiesEvent(ArrayList alEventDataList)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.OnNotifyToolBarAboutCurrentSelectedViewPropertiesEvent 호출
		/// </summary>
		/// <param name="alEventDataList"></param>
		private void MainCanvas_OnNotifyToolBarAboutCurrentSelectedViewPropertiesEvent (object toolbarProperty)
		{
			MainToolBarPanel.UpdateObjectPropertyToolBar (toolbarProperty);
		}

		#endregion

		#region |  void MainCanvas_OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent(ArrayList alEventDataList)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent 호출
		/// </summary>
		/// <param name="alEventDataList"></param>
		private void MainCanvas_OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent (object toolbarProperty)
		{
			MainToolBarPanel.UpdateObjectPropertyToolBar (toolbarProperty);
		}

		#endregion

		#region |  bool MainCanvas_NotifyRemoveDMTFrameEvent()  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.OnNotifyRemoveDMTFrameEvent 호출
		/// </summary>
		/// <returns>DMTFrame Window가 모두 닫히면 true</returns>
		public bool MainCanvas_NotifyRemoveDMTFrameEvent ()
		{
			UpdateMenuStateAll (null);

			return IsAllDMTFrameClosded ();
		}

		#endregion

		#region |  void MainCanvas_ShowAttPageEvent(object atomAttCore, object objAtom, string strContextMenu)  |
		/// <summary>
		/// MainCanvas_ShowAtomContextMenuEvent 이벤트에서 핸들러 등록.러 등록.
		/// commonSmartContextMenu.ShowAttPageEvent 호출.
		/// </summary>
		/// <param name="atomAttCore"></param>
		/// <param name="objAtom"></param>
		/// <param name="strContextMenu"></param>
		private void MainCanvas_ShowAttPageEvent (object atomAttCore, object objAtom, string strContextMenu)
		{
			if (null != atomAttCore)
			{
				MainAtomContextMenuOpen (false);

				ChangeAtomZIndexDefine.ActionType ApplyType = ChangeAtomZIndexDefine.ActionType.한단계앞으로;

				if (ChangeAtomZIndexDefine.ActionType.한단계앞으로.ToString () == strContextMenu)
				{
					ApplyType = ChangeAtomZIndexDefine.ActionType.한단계앞으로;
					MainCanvas.AdjustAtomZIndex (ApplyType);
				}
				else if (ChangeAtomZIndexDefine.ActionType.한단계뒤로.ToString () == strContextMenu)
				{
					ApplyType = ChangeAtomZIndexDefine.ActionType.한단계뒤로;
					MainCanvas.AdjustAtomZIndex (ApplyType);
				}
				else if (ChangeAtomZIndexDefine.ActionType.맨앞으로가져오기.ToString () == strContextMenu)
				{
					ApplyType = ChangeAtomZIndexDefine.ActionType.맨앞으로가져오기;
					MainCanvas.AdjustAtomZIndex (ApplyType);
				}
				else if (ChangeAtomZIndexDefine.ActionType.맨뒤로보내기.ToString () == strContextMenu)
				{
					ApplyType = ChangeAtomZIndexDefine.ActionType.맨뒤로보내기;
					MainCanvas.AdjustAtomZIndex (ApplyType);
				}
				else
				{
					if (atomAttCore is SmartFrameAttCore)
					{
						((SmartFrameAttCore)atomAttCore).NotifyFormScriptDialogOpen -= MainWindow_NotifyFormScriptDialogOpen;
						((SmartFrameAttCore)atomAttCore).NotifyFormScriptDialogOpen += MainWindow_NotifyFormScriptDialogOpen;
					}

					UserControl ucAttPage = null;

					DOC_KIND docKind = GetActiveDocKind ();

					if (DOC_KIND._docReport == docKind)
					{
						ucAttPage = ((SmartAtomAttCore)atomAttCore).ShowReportAttPage (objAtom as ReportAtomCore, strContextMenu);
					}
					else
					{
						ucAttPage = ((SmartAtomAttCore)atomAttCore).ShowAttPage (objAtom as Atom, strContextMenu);
					}

					if (null != ucAttPage)
					{
						AtomAttWindow.SetContent (ucAttPage);
						AtomAttWindow.AttCore = (SmartAtomAttCore)atomAttCore;
					}
				}

				MainAtomContextMenuPanel.Child = null;

				//애니메이션 윈도우의 애니메이션 셋팅
				string strCurrentAnimationType = AnimationKeyWord.KeyWord.SlideAndFade.ToString ();
				string strEndAnimationKeyword = string.Format ("{0}End", strCurrentAnimationType);
				AtomAttWindow.SetAnimationKeyword (string.Empty, strEndAnimationKeyword);
			}
		}

		private UserControl ShowAttPageReport (object atomAttCore, object objAtom, string strContextMenu)
		{
			UserControl ucAttPage = null;

			//ucAttPage = ((SmartAtomAttCore)atomAttCore).ShowAttPageReport (objAtom as CAtom, strContextMenu);

			return ucAttPage;
		}

		#endregion

		#region |  void MainCanvas_AtomDoubleClickedEvent(object objValue)  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.AtomDoubleClickedEvent 호출.
		/// </summary>
		/// <param name="objValue"></param>
		void MainCanvas_AtomDoubleClickedEvent (object objValue)
		{
			AtomBase atomBase = objValue as AtomBase;
			
			if (null != atomBase && false == atomBase.TextEditMode)
			{
				Atom atomCore = atomBase.AtomCore as Atom;
				if (null != atomCore)
				{
					Information currentInfo = atomCore.Information as Information;
					bool bIsWebDoc = false;
					bool bIsEBookModel = false;

					if (null != currentInfo)
					{
						bIsWebDoc = currentInfo.IsWebdoc ();
						bIsEBookModel = currentInfo.IsEBookModel ();
					}

					if (false != LicenseHelper.Instance.IsEnableSolutionService (atomBase.AtomCore.AtomType))
					{
						MainCanvas_ShowAttPageEvent (SmartAtomManager.ShowAttPage (atomCore, bIsWebDoc, bIsEBookModel), atomCore, "스타일속성");
					}
				}
				else
				{
					//IDrawable atomItem = atomBase.GetAtomCore() as IDrawable;

					//if (null != atomItem)
					//{
					//	MainCanvas_ShowAttPageEvent(SmartAtomManager.ShowAttPage(atomItem, false, false), atomItem, "스타일속성");
					//}
				}
			}
		}

		#endregion

		#region |  void MainCanvas_NotifyNewModelSave(object objValue) : 파일저장 처리관련  |
		/// <summary>
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 추가.
		/// MainCanvas.NotifyNewModelSave 호출
		/// </summary>
		/// <param name="objValue"></param>
		private void MainCanvas_NotifyNewModelSave (object objValue)
		{
			BaseFrame currentFrame = objValue as BaseFrame;
			TopView currentView = null != currentFrame ? currentFrame.GetCurrentView () : null;
			TopDoc currentDoc = (null != currentView ? currentView.GetDocument () : null);

			if (null == currentFrame || null == currentView || null == currentDoc)
			{
				return;
			}

			FileSaveAs (currentDoc);
		}

		#endregion

		#region | void MainCanvas_OnNotifyMainScrollChanged(double dValue) : 메인 스크롤 처리 관련 |

		void MainCanvas_OnNotifyMainScrollChanged (double dValue)
		{
			if (null != MainScrollViewer)
			{
				MainScrollViewer.ScrollToVerticalOffset (MainScrollViewer.VerticalOffset + dValue);
			}
		}

		#endregion


		#endregion//중앙작업 캔버스에 대한 이벤트

		#region |  void MainWindow_Closing(object sender, CancelEventArgs e)  |
		/// <summary>
		/// InitializeMainWindowEvents 메서드에서 이벤트 핸들러 추가.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void MainWindow_Closing (object sender, CancelEventArgs e)
		{
			if (false == e.Cancel)
			{
				GlobalWaitThread.WaitThread.End ();
#if DEBUG
				//LicenseService.ExecuteUpgrade (RegistryCoreX.ProductEnvPath, false, false, true);
#else
				if (LicenseService.CheckRegistered(PQAppBase.CurrentProductCode))
                {
                    LicenseService.ExecuteUpgrade(RegistryCoreX.ProductEnvPath, false, false, false);
                }
#endif
			}
		}

		#endregion

		#region |  void MainWindow_ContentRendered(object sender, EventArgs e)  |
		/// <summary>
		/// InitializeMainWindowEvents 메서드에서 이벤트 핸들러 추가.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindow_ContentRendered (object sender, EventArgs e)
		{

			if (true == LicenseHelper.Instance.FreeLicense && LC.LANG.KOREAN != LC.PQLanguage)
			{
				return; //한국어 버전이 아닌경우 테스트를 위해 라이선스 체크 안함
			}

			if (false == m_pIsFreeVersion)
			{
				if (false == LicenseCheck ())
				{
					if (false == LicenseService.CheckRegistered (PQAppBase.CurrentProductCode))
					{
						//_Message80.Show ("고객등록 후 사용하실수 있습니다", "알림");
						OnProgramExit ();
						return;
					}
					else
					{
						if (false == LicenseHelper.Instance.FreeLicense)
						{
							LicenseHelper.Instance.SetSolutionStandardType ();
						}
					}
				}

				SetLicenseInfo ();
				MainToolBarPanel.UpdateSolutionType ();

				//if (LicenseHelper.Instance.IsExpiryDay ())
				//{
				//	LicenseHelper.Instance.CheckExpiryDate ();
				//}
				//else
				//{
				//	LicenseHelper.Instance.CheckExpiryDate ();
				//}

				LicenseHelper.Instance.CheckExpiryDate ();
			}
			else if (true == m_pIsFreeVersion)
			{
				SetFreeLicense ();
			}

			AutoBackupProcessManager.Instance.Init (this);

			if (true == PQAppBase.IsProjectManager)
			{
				ProjectControlManager.Instance.Start ();
				ProjectControlManager.Instance.OnOpenFile += ProjectControlManager_OnOpenFile;
			}


			// 동영상 강의 및 메이커스토어 띄우기
			if (LC.PQLanguage == LC.LANG.KOREAN || LC.PQLanguage == LC.LANG.JAPAN)
			{
				if (false == LicenseHelper.Instance.FreeLicense && false == PQAppBase.IsEduTechMode)
					ShowSubsidiaryWindow ();
			}
			//
		}

		private void ProjectControlManager_OnOpenFile (object objValue)
		{
			this.Dispatcher.BeginInvoke (new Action (delegate
			{
				FileOpen (objValue?.ToString ());
			}));
		}

		async Task DelayShowSubsidiaryWindow ()
		{
			int nBeginTime = 1;
			await Task.Delay ((int)(nBeginTime * 100));

			// 동영상 강의 및 메이커스토어 띄우기
			if (7 < LicenseHelper.Instance.GetInstallDayCount ())
				SubsidiaryWindow.ShowPage (1);
			else
				SubsidiaryWindow.ShowPage (0);
		}

		private void ShowSubsidiaryWindow ()
		{
			var t = Task.Run (() =>
			{
				this.Dispatcher.Invoke (new Action (delegate
				{
					DelayShowSubsidiaryWindow ();
				}));
			});

		}

		#endregion

		#region |  void MainWindow_Loaded(object sender, RoutedEventArgs e)  |
		/// <summary>
		/// InitializeMainWindowEvents 메서드에서 이벤트 핸들러 추가.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindow_Loaded (object sender, RoutedEventArgs e)
		{
			MainCanvas.InvalidateAll ();

			InitHardwareacceleration ();

			if (SystemParameters.MaximizedPrimaryScreenWidth > 800)
			{
				int nWindowState = _Registry.ReadInt32 ("Environment\\PROCESS", "WindowState", (int)WindowState.Normal);
				if (nWindowState == (int)WindowState.Maximized)
				{
					this.WindowState = WindowState.Maximized;
				}
				else
				{
					this.Width = _Registry.ReadInt32 ("Environment\\PROCESS", "Width", 900);
					this.Height = _Registry.ReadInt32 ("Environment\\PROCESS", "Height", 680);
				}

				int nDefaultLeft = (int)((System.Windows.SystemParameters.PrimaryScreenWidth / 2) - (this.Width / 2));
				int nDefaultTop = (int)((System.Windows.SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2));

				int nMaxWidth = 0;
				int nMaxHeight = 0;

				foreach (var screen in System.Windows.Forms.Screen.AllScreens)
				{
					int nWidth = screen.Bounds.Left + screen.Bounds.Width;
					int nHeight = screen.Bounds.Top + screen.Bounds.Height;
					nMaxWidth = nMaxWidth < nWidth ? nWidth : nMaxWidth;
					nMaxHeight = nMaxHeight < nHeight ? nHeight : nMaxHeight;
				}

				int nLeft = _Registry.ReadInt32 ("Environment\\PROCESS", "Left", nDefaultLeft);
				int nTop = _Registry.ReadInt32 ("Environment\\PROCESS", "Top", nDefaultTop);

				nLeft = 0 < nLeft ? nLeft : 0;
				nTop = 0 < nTop ? nTop : 0;

				//빌더가 화면을 벗어난경우 화면최대치에 맞춰서 표시해주기 위한 논리
				nLeft = nMaxWidth < nLeft + this.Width ? (int)(nMaxWidth - this.Width) : nLeft;
				nTop = nMaxHeight < nTop + this.Height ? (int)(nMaxHeight - this.Height) : nTop;

				this.Left = nLeft;
				this.Top = nTop;
			}

			this.MainMenu.UpdateWindowState (this.WindowState);

			// 자동로그인
			if (false != LicenseService.CheckRegistered (PQAppBase.CurrentProductCode))
			{
				uint nAutoLogin = RegLib.GetUIntRegInfo (RegistryCoreX.ProductServerPath + "LastLogin", "AutoLogin");
				if (1 == nAutoLogin)
				{
					UserLoginX (true);
				}
			}
		}

		#endregion

		#region |  void OnCompleteJettyServerEnv(object sender, EventArgs e) : Jetty 서버 관련  |
		/// <summary>
		/// JettyServer 환경 설정이 완료될 때 발생하는 이벤트
		/// JettyServer 재시작을 시킨다. 
		/// 생성자에서 이벤트 핸들러 추가. PQAppBase.CompleteJettyServerEnv 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnCompleteJettyServerEnv (object sender, EventArgs e)
		{
		}

		#endregion

		#region |  ----- 메인 윈도우 키 이벤트 -----  |


		private void MainWindow_KeyDown (object sender, KeyEventArgs e)
		{
			try
			{
				BaseFrame currentDMTFrame = MainCanvas.CurrentDMTFrame as BaseFrame;

				#region | 최상위 편집 단축키 |

				if (e.KeyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.F8)
				{
					//F8 : 실행모드 전환
					MainToolBarPanel_OnToolBarItemClickedEvent (0);
				}
				else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.O)
				{
					//Ctrl + O : 파일열기
					OnFileOpen ();
				}
				else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.S &&
					FormMode.FrameMode.Focused == currentDMTFrame?.GetFormMode ())
				{
					//Ctrl + S : 파일저장
					OnFileSave ();
				}
				else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.N)
				{
					//Ctrl + N : 새로만들기
					ProcessMainMenuClickedEvent (MainMenuDef.NewModelMenuItem);
				}
				else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.M)
				{
					//Ctrl + M : 실행모드 전환
					MainToolBarPanel_OnToolBarItemClickedEvent (0);
				}
				else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.F4)
				{
					//Ctrl + F4 : 현재 모델 종료
					currentDMTFrame?.CaptionBar_CaptionButtonClickEvent (3);
				}
				else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Tab)
				{
					if (MainCanvas.GetAllFrame ().Any ())
					{
						FrameTogglePopup.CanvasManager = MainCanvas.CanvasManager;
						FrameTogglePopup.RefreshFrameList (MainCanvas.GetAllFrame ());

                        int idx = 1 < FrameTogglePopup.FrameVMList.Count ? 1 : 0;

                        if (0 < FrameTogglePopup.FrameVMList.Count && 
                            FrameTogglePopup.FrameVMList[idx] is var CurrentViewModel)
						{
							CurrentViewModel.IsSelected = true;
						}

						FrameTogglePopup.Show ();
                        e.Handled = true;
					}
				}

				#endregion

				if (null != currentDMTFrame)
				{
					//읽기 전용, 모두 잠금 모드일때 key input 리턴 
					if (true == currentDMTFrame.GetCurrentView ().IsReadonlyMode
						|| true == currentDMTFrame.GetCurrentView ().IsLockAllMode)
					{
						return;
					}

					//해당 DMTFrame부터 처리될 키 이벤트들은 이곳에 들어간다.
					//MainCanvas.OnPreviewKeyboardDown (sender, e);
				}
			}
			catch (Exception ex)
			{
				_Error80.Show (ex);
				LogManager.WriteLog (ex);
				Trace.TraceError (ex.ToString ());
			}
		}

		private void MainWindow_KeyUp (object sender, KeyEventArgs e)
		{

		}

		#region |  void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)  |
		/// <summary>
		/// InitializeMainWindowEvents 메서드에서 이벤트 핸들러 등록.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindow_PreviewKeyDown (object sender, KeyEventArgs e)
		{
			//if (e.Key == Key.F3)
			//{
			//	//ShowInteractiveTutorialWindow ();
			//	var element = MainCanvas.CurrentDMTFrame as FrameworkElement;

			//	var view = element;
			//	var rect = new Rect (0, 0, view.ActualWidth, view.ActualHeight);
			//	var visual = new DrawingVisual ();

			//	using (var dc = visual.RenderOpen ())
			//	{
			//		dc.DrawRectangle (Brushes.White, new Pen () { Brush = Brushes.LightGray, Thickness = 1 }, rect);
			//		dc.DrawRectangle (new VisualBrush (view), null, rect);
			//	}

			//	var renderTarget = new RenderTargetBitmap (
			//		(int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);
			//	renderTarget.Render (visual);

			//	using (var ms = new MemoryStream ())
			//	{
			//		var encoder = new JpegBitmapEncoder ();
			//		encoder.Frames.Add (BitmapFrame.Create (renderTarget));
			//		encoder.Save (ms);

			//		ms.Position = 0;
			//		File.WriteAllBytes ("temp.jpg", ms.ToArray ());
			//	}
			//}
		}

		#endregion



		#region |  void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)  |
		/// <summary>
		/// Kiho : 2016-11-04 추가, LeftCtrl Key Up 이벤트 발생시 처리를 위해.
		/// InitializeMainWindowEvents 메서드에서 이벤트 핸들러 등록.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindow_PreviewKeyUp (object sender, KeyEventArgs e)
        {
        }

		#endregion

		#region |  void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)  |
		/// <summary>
		/// InitializeMainWindowEvents 메서드에서 이벤트 핸들러 등록.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindow_PreviewMouseDown (object sender, MouseButtonEventArgs e)
		{
			SendRootMouseEventToAtom ();
			CheckAllPopupPanels (e);
			CheckMouseFocus ();
			if (null != m_SubsidiaryWindow)
			{
				//2019-11-21 동영상 학습 페이지 포커스 이벤트 처리를 위해서 호출한다.
				m_SubsidiaryWindow.MouseLeftButtonDownHandler (sender, e);
			}
		}

		#endregion

		#region |  void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)  |
		/// <summary>
		/// 메인윈도우에서 PreviewMouseUp 이벤트를 받아서 차일드 뷰들에게 알려준다.
		/// 어떠한 용도로도 사용될 수 있는데 현재는 DMTView에서 드래그중 마우스가 밖에서 Up이 되면 RectTracker잔상이 남는 문제때문에 자식에게 알려주고있다.
		/// InitializeMainWindowEvents 메서드에서 이벤트 핸들러 등록.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindow_MouseUp (object sender, MouseButtonEventArgs e)
		{
			MainCanvas.HiddenRectTrackerOfAllChildView ();
		}

		#endregion
		#endregion//메인 윈도우 키 이벤트

		#region |  ----- 윈도우 리사이즈 이벤트 -----  |
		#region |  void MainWindowResizeGrip_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)  |
		/// <summary>
		/// 자동 생성된 이벤트 핸들러
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindowResizeGrip_MouseLeftButtonDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{

			if (MouseButtonState.Pressed == e.LeftButton)
			{
				Mouse.Capture (MainWindowResizeGrip);
				m_PtMouseGripStart = e.GetPosition (MainWindowResizeGrip);
				m_bIsWindowResizing = true;
			}
		}

		#endregion

		#region |  void MainWindowResizeGrip_MouseMove(object sender, MouseEventArgs e)  |
		/// <summary>
		/// 자동 등록된 이벤트 핸들러.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindowResizeGrip_MouseMove (object sender, MouseEventArgs e)
		{

			if (true == m_bIsWindowResizing)
			{
				Point CurrentPoint = e.GetPosition (MainWindowResizeGrip);
				double dNewX = (CurrentPoint.X - m_PtMouseGripStart.X);
				double dNewY = (CurrentPoint.Y - m_PtMouseGripStart.Y);
				double dNewWidth = (Width + dNewX);
				double dNewHeight = (Height + dNewY);

				if (500 < dNewWidth && 400 < dNewHeight)
				{
					Width = dNewWidth;
					Height = dNewHeight;
					MainCanvas.ReLocationMinimizedFrames (dNewY);
				}
			}
		}

		#endregion

		#region |  void MainWindowResizeGrip_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)  |
		/// <summary>
		/// 자동 등록된 이벤트 핸들러.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindowResizeGrip_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{

			if (true == m_bIsWindowResizing)
			{
				Point CurrentPoint = e.GetPosition (MainWindowResizeGrip);
				double dNewX = (CurrentPoint.X - m_PtMouseGripStart.X);
				double dNewY = (CurrentPoint.Y - m_PtMouseGripStart.Y);
				double dNewWidth = (Width + dNewX);
				double dNewHeight = (Height + dNewY);

				if (500 < dNewWidth && 400 < dNewHeight)
				{
					Width = dNewWidth;
					Height = dNewHeight;
				}

				m_bIsWindowResizing = false;
				MainWindowResizeGrip.ReleaseMouseCapture ();

				//m_dPrevWindowWidth = this.Width;
				//m_dPrevWindowHeight = this.Height;
				//m_dPrevWindowLeft = this.Left;
				//m_dPrevWindowTop = this.Top;

				//MainCanvas.InvalidateAll();
			}
		}

		#endregion
		#endregion//윈도우 리사이즈 이벤트

		#region |  ArrayList HandleGlobalEvent(List<KeyValuePair<object, object>> ParameterDictionary)  |
		/// <summary>
		/// Main Window 전역 이벤트리시버
		/// InitializeChildrenEvents 메서드에서 이벤트 핸들러 등록.
		/// GlobalEventReceiver.UniqueGlobalEventRecevier.OnNotifyGlobalEventOccured 호출.
		/// </summary>
		/// <param name="ParameterDictionary"></param>
		/// <returns></returns>
		public ArrayList HandleGlobalEvent (List<KeyValuePair<object, object>> ParameterDictionary)
		{
			int nEventCount = ParameterDictionary.Count;

			for (int nIndex = 0; nIndex < nEventCount; nIndex++)
			{
				KeyValuePair<object, object> EventDataPair = ParameterDictionary[nIndex];
				DelegateEventKeys.MainWindowReceiverEventKey EventKey = (DelegateEventKeys.MainWindowReceiverEventKey)EventDataPair.Key;

				switch (EventKey)
				{
					case DelegateEventKeys.MainWindowReceiverEventKey.MakeAtom:
						{
							int nFrameCount = MainCanvas.GetAllFrame ().Count;

							if (0 < nFrameCount)
							{
								KeyValuePair<AtomType, List<bool>> MakeAtomParameter = (KeyValuePair<AtomType, List<bool>>)(EventDataPair.Value);
								AtomType AtomKey = MakeAtomParameter.Key;
								List<bool> lstParameters = MakeAtomParameter.Value;
								bool bIsImmediately = lstParameters[0];
								bool bIsStackHistory = lstParameters[1];

								AtomBase MakedAtom = MainCanvas.MakeAtom (AtomKey, bIsImmediately, bIsStackHistory);

								if (null != MakedAtom)
								{
									ArrayList arMakedAtomList = new ArrayList ();
									arMakedAtomList.Add (MakedAtom);
									return arMakedAtomList;
								}
								break;

							}

							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.WorkScriptMenu:
						{
							int nFrameCount = MainCanvas.GetAllFrame ().Count;

							if (0 < nFrameCount)
							{
								string strAtomKey = Convert.ToString (EventDataPair.Value);
								ProcessScriptMenuClickedEvent (strAtomKey);
							}
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.ClosedScriptFrame:
						{
							//MainExpandMenuContainer.CurrentExpandMenuType = MainExpandMenuContainer.PreExpandMenuType;
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.WorkExecuteMenu:
						{
							ExecuteMenuManager.Instance.SendMessageToMakeExecuteWindow (ParameterDictionary);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.CloseMakeExecuteMenuFrame:
						{
							//MainExpandMenuContainer.CurrentExpandMenuType = MainExpandMenuContainer.PreExpandMenuType;
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.MainMenuClicked:
						{
							string strValue = Convert.ToString (EventDataPair.Value);
							ProcessMainMenuClickedEvent (strValue);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.GetRecentFileList:
						{
							return m_MainWindowManager.GetRecentFileList ();
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.RecentFileClicked:
						{
							string strFullFilePath = Convert.ToString (EventDataPair.Value);
							OpenRecentFile (strFullFilePath);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.RecentFileRemove:
						{
							string strFullFilePath = Convert.ToString (EventDataPair.Value);
							RemoveRecentFile (strFullFilePath);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.RecentFileFolder:
						{
							string strFullFilePath = Convert.ToString (EventDataPair.Value);
							FolderRecentFile (strFullFilePath);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.InvalidateAll:
						{
							MainCanvas.ReLocationMinimizedFrames ();
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.ServerConnectionChanged:
						{
							bool bIsSuccess = Convert.ToBoolean (EventDataPair.Value);
							MainStatusBar.ChangeServerConnectionImage (bIsSuccess);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.FrameStateChanged:
						{
							BaseFrame SourceFrame = EventDataPair.Value as BaseFrame;
							MainCanvas.ChangeFrameState (SourceFrame);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.CloseAllPopup:
						{
							CloseAllPopup ();
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.CloseAtomAttWindow:
						{
							CloseAtomAttWindow ();
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.CloseDesignHelper:
						{
							SwapVisibilityOfDesignHelper ();
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.ApplyDesignHelperImage:
						{
							DictionaryEntry DesignHelperParameterList = (DictionaryEntry)(EventDataPair.Value);
							AttemptApplyDesignHelperEvent (DesignHelperParameterList);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.AtomExpandContainerChanged:
						{
							MainExpandMenuType AtomExpandContainerTypeParameterList = (MainExpandMenuType)(EventDataPair.Value);
							SwapAtomExpandMenu (AtomExpandContainerTypeParameterList);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.NotifyLocationAndSize:
						{
							List<double> LocationAndSizeList = (EventDataPair.Value) as List<double>;
							UpdateStatusBarAboutLocationAndSize (LocationAndSizeList);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.NotifyLocation:
						{
							Thickness NewMargin = (Thickness)(EventDataPair.Value);
							UpdateStatusBarAboutLocation (NewMargin);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.NotifyLocationAndSizeChanged:
						{
							KeyValuePair<LocationAndSizeDefine.LocationAndSizeType, double> ValueList = (KeyValuePair<LocationAndSizeDefine.LocationAndSizeType, double>)(EventDataPair.Value);
							UpdateCurrentLocationAndSize (ValueList);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.AtomZIndexChanged:
						{
							ChangeAtomZIndexDefine.ActionType ApplyType = (ChangeAtomZIndexDefine.ActionType)(EventDataPair.Value);
							AdjustAtomZIndex (ApplyType);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.FontSizeChanged:
						{
							double dChangedFontSize = Convert.ToDouble (EventDataPair.Value);
							UpdateToolBarFontSize (dChangedFontSize);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.ShortCutClicked:
						{
							int nShortCutType = Convert.ToInt32 (EventDataPair.Value);
							ExecuteShortCut (nShortCutType);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.ArduinoShortCutClicked:
						{
							int nShortCutType = Convert.ToInt32 (EventDataPair.Value);
							ExecuteArduinoShortCut (nShortCutType);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.MakeNewModel:
						{
							GlobalWaitThread.WaitThread.Start (this);

							if (EventDataPair.Value is ModelDescription CurrentModel)
							{
								string strFilePath = CurrentModel.FileName;
								DOC_KIND DocKind = CurrentModel.DocKind;
								bool bIsExtendedPageMode = CurrentModel.IsExtendedPageModel;

								if (LC.LANG.ENGLISH == LC.PQLanguage) //영문 공백논리 관련
								{
									//기존에 있던 폼에 데이터를 셋팅
									if (null != MainCanvas.CurrentDMTFrame)
									{
										Softpower.SmartMaker.TopLight.ViewModels.LightJDoc pCurrentDocument = MainCanvas.CurrentDMTFrame.GetCurrentView ().GetDocument () as Softpower.SmartMaker.TopLight.ViewModels.LightJDoc;

										if (null != pCurrentDocument)
										{
											pCurrentDocument.AtomNameList = SMProperVar_Eng.MapAtomName;
											pCurrentDocument.SymbolNameList = SMProperVar_Eng.SymbolData;
										}
									}

									SMProperVar_Eng.MapAtomName = null;
									SMProperVar_Eng.SymbolData = null;
								}


								if (DOC_KIND._docReport == DocKind)
								{
									GlobalWaitThread.WaitThread.End ();

									if (false == PQAppBase.strTrial && false == LicenseHelper.Instance.IsEnableSolutionService (SolutionService.ReportGenerator))
										break;

									GlobalWaitThread.WaitThread.Start ();
								}

								if (true == string.IsNullOrEmpty (strFilePath))
								{
									MakeNewModel (DocKind, bIsExtendedPageMode);

									if (DocKind == DOC_KIND._docQuizLayoutMaker)
									{
										if (MainCanvas.CurrentDMTFrame is QuizMakerFrame quizMakerFrame)
										{
											quizMakerFrame.MakeDefaultAtom ();
										}
									}
								}
								else
								{
									string strNewFilePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Template");

									if (LC.PQLanguage == LC.LANG.JAPAN)
										strNewFilePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "TemplateJP");
									else if (LC.PQLanguage == LC.LANG.ENGLISH)
										strNewFilePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "TemplateEN");//

									strNewFilePath = System.IO.Path.Combine (strNewFilePath, strFilePath);
									strNewFilePath = strNewFilePath.Trim ();

									if (-1 < strFilePath.IndexOf ("_Embedding_", StringComparison.OrdinalIgnoreCase))
									{
										var dmtFrame = MainCanvas.CurrentDMTFrame as DMTFrame;
										var document = dmtFrame?.GetDocument () as DMTDoc;

										if (null != document && document.DocType != DOC_KIND._docQuizMaker)
										{
											strNewFilePath = MakeTemplateCopyFile (strNewFilePath);
											dmtFrame.TemplateTempPath += strNewFilePath + ",";
											dmtFrame.EmbeddingTemplateForm (strNewFilePath);
											return null;
										}
									}
									else if (-1 < strFilePath.IndexOf ("_QuizLayout_", StringComparison.OrdinalIgnoreCase))
									{
										var quizTypeValue = strFilePath.Replace ("_QuizLayout_", "");

										MakeNewModel (DocKind, bIsExtendedPageMode);

										if (false == string.IsNullOrEmpty (quizTypeValue) && MainCanvas.CurrentDMTFrame is QuizMakerFrame quizMakerFrame)
										{
											if (Enum.TryParse<QuizType> (quizTypeValue, out QuizType quizType))
											{
												var quizMetaData = quizMakerFrame.QuizMakerView.Document.PageMetadata.QuizMetaData;
												quizMetaData.QuizType = quizType;
												quizMakerFrame.MakeDefaultAtom ();
											}
											else
											{
												Trace.TraceError ($"DelegateEventKeys.MainWindowReceiverEventKey.MakeNewModel 퀴즈 유형을 인식할 수 없습니다. {strFilePath}");
											}
										}
									}

									FileOpen (strNewFilePath, true);
								}

								if (LC.LANG.ENGLISH == LC.PQLanguage)
								{
									//세로 만들어진 폼으로 SmPorpvar데이터 셋팅
									if (null != MainCanvas.CurrentDMTFrame)
									{
										Softpower.SmartMaker.TopLight.ViewModels.LightJDoc pCurrentDoc = MainCanvas.CurrentDMTFrame.GetCurrentView ().GetDocument () as Softpower.SmartMaker.TopLight.ViewModels.LightJDoc;

										if (null != pCurrentDoc)
										{
											SMProperVar_Eng.MapAtomName = pCurrentDoc.AtomNameList;
											SMProperVar_Eng.SymbolData = pCurrentDoc.SymbolNameList;
											//SMProperVar_Eng.SymbolData = pCurrentDoc.GetDBMaster().ScriptVarMap.Cast<DictionaryEntry>().ToDictionary(d => d.Key.ToString(), d => d.Value.ToString());

										}
									}
								}
								if (null != MainCanvas.CurrentDMTFrame)
								{
									Softpower.SmartMaker.TopLight.ViewModels.LightJDoc pCurrentDoc = MainCanvas.CurrentDMTFrame.GetCurrentView ().GetDocument () as Softpower.SmartMaker.TopLight.ViewModels.LightJDoc;

									if (null != pCurrentDoc && false == pCurrentDoc.IsDynamicMode && false == pCurrentDoc.IsEBookDoc && false == pCurrentDoc.TemplateMode) //2020-02-21 kys App모델 일때만 메뉴영역 생성
									{
										int nTop = Softpower.SmartMaker.TopApp.DeviceInformation.Models.GlobalDeviceInformation.TopBarHeight;
										int nBottom = Softpower.SmartMaker.TopApp.DeviceInformation.Models.GlobalDeviceInformation.BottomBarHeight;

										BarHeightChange (nTop, nBottom);
									}
									else if (null != pCurrentDoc && true == pCurrentDoc.IsWebDoc)
									{
										DMTView dmtView = pCurrentDoc.GetParentView () as DMTView;
										if (null != dmtView)
										{
											dmtView.RefreshDynamicGrid ();
										}
									}
								}

								MainCanvas_OnNotifyToolBarNoCurrentSelectedAtomEvent (false);
							}
							
							GlobalWaitThread.WaitThread.End ();

							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.FrameModeChanged:
						{
							BaseFrame CurrentFrame = (BaseFrame)(EventDataPair.Value);
							UpdateMenuStateAll (CurrentFrame);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.AtomSelectChanged:
						{
							ICommandBehavior CurrentCommand = (ICommandBehavior)(EventDataPair.Value);
							MainCanvas.AtomSelectChanged (CurrentCommand);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.SaveModel:
						{
							OnFileSave ();
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.SaveAsModel:
						{
							OnFileSaveAs ();
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.OpenQuizMakerModel:
						{
							OnQuizMakerFileOpen ();

							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.ShowDefaultIntervalDialog:
						{
							m_MainWindowManager.ShowDialog (FlexibleDialogDefine.FlexibleDialogType.IntervalInformation, true, null);
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.GetCurrentSelectedAtoms:
						{

							if (null != MainCanvas.CurrentDMTFrame)
							{
								TopDoc CurrentDocument = MainCanvas.CurrentDMTFrame.GetCurrentView ().GetDocument ();
								if (null != CurrentDocument)
								{
									if (CurrentDocument is DMTDoc)
									{
										List<object> lstCurrentSelectedAtoms = CurrentDocument.GetCurrentSelectedAtomsObject ();

										List<AtomBase> CurrentSelectedAtoms = new List<AtomBase> ();
										foreach (AtomBase atom in lstCurrentSelectedAtoms)
										{
											CurrentSelectedAtoms.Add (atom);
										}

										ArrayList alSelectedAtomList = new ArrayList ();
										alSelectedAtomList.Add (CurrentSelectedAtoms);

										return alSelectedAtomList;
									}
									else if (CurrentDocument is ReportDMTDoc)
									{
										ReportDMTDoc reportDoc = CurrentDocument as ReportDMTDoc;
										List<ReportOfAtom> atomList = reportDoc.CurrentView.GetSelectAtomList ();

										ArrayList atomArrayList = new ArrayList ();
										atomArrayList.Add (atomList);
										return atomArrayList;
									}
								}
							}

							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.SetAtomNames:
						{
							KeyValuePair<ArrayList, string> Parameter = (KeyValuePair<ArrayList, string>)(EventDataPair.Value);
							ArrayList alAtomList = Parameter.Key;
							string strApplyAtomName = Parameter.Value;

							if (null != alAtomList)
							{
								int nlstCount = alAtomList.Count;

								if (1 == nlstCount)
								{
									if (MainCanvas.CurrentDMTFrame is DMTFrame)
									{
										List<AtomBase> lstApplyAtoms = alAtomList[0] as List<AtomBase>;
										bool bResult = MainCanvas.ApplyChangeAtomNames (lstApplyAtoms, strApplyAtomName);
										return new ArrayList () { bResult };
									}
									else if (MainCanvas.CurrentDMTFrame is ReportDMTFrame)
									{
										List<ReportOfAtom> lstApplyAtoms = alAtomList[0] as List<ReportOfAtom>;
										bool bResult = MainCanvas.ApplyChangeAtomNames (lstApplyAtoms, strApplyAtomName);
										return new ArrayList () { bResult };
									}
								}
							}

							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.SetAtomFieldType:
						{
							KeyValuePair<ArrayList, string> Parameter = (KeyValuePair<ArrayList, string>)(EventDataPair.Value);
							ArrayList alAtomList = Parameter.Key;
							string strApplyAtomFieldType = Parameter.Value;

							if (null != alAtomList)
							{
								int nlstCount = alAtomList.Count;

								if (1 == nlstCount)
								{
									List<AtomBase> lstApplyAtoms = alAtomList[0] as List<AtomBase>;
									MainCanvas.ApplyChangeAtomFieldType (lstApplyAtoms, strApplyAtomFieldType);
								}
							}

							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.NotifyWindowStateType:
						{
							//WindowStateType ChangedStateType = (WindowStateType)(EventDataPair.Value);
							//ChangeWindowState(ChangedStateType);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.CloseMainWindow:
						{
							OnCloseMainWindow ();
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.LoadModelFromInnerLogic:
						{
							string strFilePath = EventDataPair.Value.ToString ();
							return OnLoadModelFromInnerLogic (strFilePath, 1, false);
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.LoadModelFromBookPage:
						{
							object[] objValues = (object[])EventDataPair.Value;
							if (null != objValues && 2 <= objValues.Length)
							{
								string strFilePath = (string)objValues[0];
								int nBookPage = (int)objValues[1];
								bool DoModal = (bool)objValues[2];

								return OnLoadModelFromInnerLogic (strFilePath, nBookPage, DoModal);
							}
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.LoadModelFromMakerStore:
						{
							string strFilePath = EventDataPair.Value.ToString ();
							return OnLoadModelFromMakerStore (strFilePath);
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.LoadModelFromSimulator:
						{
							string strFilePath = EventDataPair.Value.ToString ();
							return OnLoadModelFromSimulator (strFilePath);
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.LaodTopMostModelFromSimulator:
						{
							string strFilePath = EventDataPair.Value.ToString ();
							return OnLoadTopMostModelFromSimulator (strFilePath);
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.ShowSmartApprovalView:
						{
							Hashtable AtomAttrib = EventDataPair.Value as Hashtable;

							SkinFrame CurrentDMTFrame = MainCanvas.CurrentDMTFrame as SkinFrame;

							if (null != CurrentDMTFrame)
							{
								CurrentDMTFrame.ShowSmartApprovalView (AtomAttrib);

								TopView CurrentDMTView = CurrentDMTFrame.GetCurrentView ();
								CurrentDMTView.Visibility = Visibility.Hidden;
							}

							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.ApprovalComplete:
						{
							ArrayList ApprovalInfoList = EventDataPair.Value as ArrayList;

							TopDoc CurrentDocument = MainCanvas.CurrentDMTFrame.GetCurrentView ().GetDocument ();

							UIElementCollection AtomCollection = CurrentDocument.GetChildren ();

							foreach (AtomBase SelectAtom in AtomCollection)
							{
								if (SelectAtom is SmartApprovalAtomBase)
								{
									SmartApprovalAtomBase ApprovalAtomBase = SelectAtom as SmartApprovalAtomBase;
									ApprovalAtomBase.SetSmartApprovalInfo (ApprovalInfoList);
									break;
								}
							}

							ShowDMTView ();
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.ApprovalViewClose:
						{
							ShowDMTView ();
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.ShowFlowMap:
						{
							OnShowProcessManager ();
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.ShowAtomEditMap:
						{
							OnShowAtomEditManager ();
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.ShowAttPageInGridTableAtom:
						{
							object[] objValues = (object[])EventDataPair.Value;
							object objAtom = objValues[0];
							string strContextMenu = objValues[1].ToString ();
							OnShowAttPageInGridTableAtom (objAtom, strContextMenu);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.ToolBarFontFamilyAddSelectFont:
						{
							UpdateToolBarFontFamilyAddSelectFont ();
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.EBookToolBarHandleCommonEvent:
						{
							object[] objValues = (object[])EventDataPair.Value;
							DelegateStructType.EventSourceType EventSourceType = (DelegateStructType.EventSourceType)objValues[0];
							int nEventKey = (int)objValues[1];
							object objValue = objValues[2];

							MainToolBarPanel_CommonEvent (EventSourceType, nEventKey, objValue);


							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.CurrentDMTViewFocusedEvent:
						{
							DependencyObject scope = (DependencyObject)(EventDataPair.Value);
							CurrentDMTViewFocused (scope);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.NotifyStatusBarServerIconClicked:
						{
							bool bValue = (bool)EventDataPair.Value;
							bValue &= PQAppBase.ConnectStatus;
							OnLogin (bValue ? MainMenuDef.ServerDisConnectionMenuItem : MainMenuDef.ServerConnectionMenuItem);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.NotifyUpgradeStatusClicked:
						{
							bool bValue = (bool)EventDataPair.Value;
							OnExecuteUpgrade (true);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.LinkMakeStore: //2019-12-03 kys MakeStore로 연결
						{
							int nLinkType = (int)EventDataPair.Value;
							LinkMakeStore (nLinkType);
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.BarHeightChange:
						{
							List<int> pList = (List<int>)EventDataPair.Value;
							int dTopBarHeight = pList[0];
							int dBottomBarheight = pList[1];

							BarHeightChange (dTopBarHeight, dBottomBarheight);

							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.ShowLinkHelp:
						{
							OnShowLinkHelp ();
							break;
						}

					case DelegateEventKeys.MainWindowReceiverEventKey.FileOpen:
						{
							string strFilePath = Convert.ToString (EventDataPair.Value);
							if (false != File.Exists (strFilePath))
							{
								FileOpen (strFilePath);
							}
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.CreateResourceFile:
						{
							//  2020-06-23 kys 윈도우패킹한 exe에서만 사용함
							//
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.WebDeployment:
						{
							ArrayList array = EventDataPair.Value as ArrayList;

							if (null != array && 1 < array.Count)
							{
								string strProjectName = array[0].ToString ();
								bool bReDeploy = (bool)array[1];

								OnServerDeployDirect (strProjectName, bReDeploy);
							}

							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.WebRegeneration:
						{
							ArrayList array = EventDataPair.Value as ArrayList;

							if (null != array && 0 < array.Count)
							{
								string strProjectName = array[0].ToString ();
								OnRegeneration (strProjectName);
							}
						}
						break;
					case DelegateEventKeys.MainWindowReceiverEventKey.ExecuteReportModel:
						{
							List<object> list = EventDataPair.Value as List<object>;

							if (null != list && 2 < list.Count)
							{
								bool isPreView = _Kiss.toBool (list[0]);
								string strReportFilePath = list[1].ToString ();
								FormDataManager formDataManager = list[2] as FormDataManager;
								string strPDFFilePath = list[3].ToString ();

								ExecuteReportModel (isPreView, strReportFilePath, formDataManager, strPDFFilePath);
							}
						}
						break;
					case DelegateEventKeys.MainWindowReceiverEventKey.NotifyTabIndexButton:
						{
							bool IsCellEvent = _Kiss.toBool (EventDataPair.Value);

							OnNotifyTabIndexButton (IsCellEvent);
						}
						break;
					case DelegateEventKeys.MainWindowReceiverEventKey.ToggleFrameMode:
						{
							MainToolBarPanel_OnToolBarItemClickedEvent (0);
						}
						break;
					case DelegateEventKeys.MainWindowReceiverEventKey.SapLaunchpad:
						{
							ArrayList array = EventDataPair.Value as ArrayList;
							if (null != array && 1 < array.Count)
							{
								string strTitle = array[0].ToString ();
								string strServiceURL = array[1].ToString ();

								OnServerDeploySapLaunchpad (strTitle, strServiceURL);

							}
							break;
						}
					case DelegateEventKeys.MainWindowReceiverEventKey.ShowAtomAttPage:
						{
							ArrayList array = EventDataPair.Value as ArrayList;
							if (null != array && 3 < array.Count)
							{
								bool IsWeb = _Kiss.toBool (array[1]);
								bool IsEBook = _Kiss.toBool (array[2]);
								string ContextItem = array[3].ToString ();

								OnShowAtomAttPage (array[0], IsWeb, IsEBook, ContextItem);
							}
						}
						break;
					case DelegateEventKeys.MainWindowReceiverEventKey.MakeSlideMaster:
						{
							SlideMasterItemModel slideMasterItemModel = EventDataPair.Value as SlideMasterItemModel;
							if (null != slideMasterItemModel)
							{
								OnMakeSlideMasterModel (slideMasterItemModel);
							}
						}
						break;
					case DelegateEventKeys.MainWindowReceiverEventKey.ActiveModelFile:
						{
							int nHashKey = _Kiss.toInt32 (EventDataPair.Value);
							ActiveModelFile (nHashKey);
						}
						break;
					case DelegateEventKeys.MainWindowReceiverEventKey.DeployQuizMaker:
						{
							if (EventDataPair.Value is ArrayList list)
							{
								if (1 < list.Count)
								{
									var quizCode = list[0]?.ToString ();
									var document = list[1] as DMTDoc;

									//PQAppBase.TraceDebugLog ("현재 소스코드에서 DeployService_v3 사용 불가");
									DeployService_v3 v3 = new DeployService_v3 ();

									var contentList = v3.DeployQQM (document, quizCode);
									return new ArrayList (contentList);
								}
							}
						}
						break;
					case DelegateEventKeys.MainWindowReceiverEventKey.ShowQuizMakerPopup:
						{
							var frame = MainCanvas.CurrentDMTFrame as BaseFrame;
							var view = frame.GetCurrentView () as TopView;
							var menuName = EventDataPair.Value?.ToString ();

							if (frame is QuizMakerMultiFrame quizMakerMultiFrame)
							{
								if (null != quizMakerMultiFrame.PopupTargetSubView)
									view = quizMakerMultiFrame.PopupTargetSubView;
							}
							
							if (false == string.IsNullOrEmpty (menuName))
							{
								SmartAtomAttCore attCore = SmartAtomManager.ShowAttPage (view, false, false);
								MainCanvas_ShowAttPageEvent (attCore, null, menuName);
							}
						}
						break;
                    case DelegateEventKeys.MainWindowReceiverEventKey.ChangeAtomFont:
                        {
                            var fontName = EventDataPair.Value?.ToString ();

                            if (false == string.IsNullOrEmpty (fontName))
                            {
                                MainCanvas.ChangeAtomFont (fontName);
                            }
                        }
                        break;

                    case DelegateEventKeys.MainWindowReceiverEventKey.MakeNewQuizBlockModel:
                        {
                            GlobalWaitThread.WaitThread.Start(this);
							ModelDescription CurrentModel = null;
							PageMetadata pageMetadata = null;

                            if (EventDataPair.Value is ArrayList list)
                            {
                                if (1 < list.Count)
                                {
                                    CurrentModel = list[0] as ModelDescription;
									pageMetadata = list[1] as PageMetadata;
                                }
                            }

                            //새로 생성될 model의 size를 결정
                            double currentDMTViewHeight = 0;
                            double currentDMTViewWidth = 0;

                            if (null != CurrentModel)
                            {
                                string strFilePath = CurrentModel.FileName;
                                DOC_KIND DocKind = CurrentModel.DocKind;
                                bool bIsExtendedPageMode = CurrentModel.IsExtendedPageModel;

                                if (LC.LANG.ENGLISH == LC.PQLanguage) //영문 공백논리 관련
                                {
                                    //기존에 있던 폼에 데이터를 셋팅
                                    if (null != MainCanvas.CurrentDMTFrame)
                                    {
                                        Softpower.SmartMaker.TopLight.ViewModels.LightJDoc pCurrentDocument = MainCanvas.CurrentDMTFrame.GetCurrentView().GetDocument() as Softpower.SmartMaker.TopLight.ViewModels.LightJDoc;

                                        if (null != pCurrentDocument)
                                        {
                                            pCurrentDocument.AtomNameList = SMProperVar_Eng.MapAtomName;
                                            pCurrentDocument.SymbolNameList = SMProperVar_Eng.SymbolData;
                                        }
                                    }

                                    SMProperVar_Eng.MapAtomName = null;
                                    SMProperVar_Eng.SymbolData = null;
                                }

                                // 새로운 모델 생성
                                if (true == string.IsNullOrEmpty(strFilePath))
                                {
                                    MakeNewModel(DocKind, bIsExtendedPageMode);
                                }

                                if (null != MainCanvas.CurrentDMTFrame)
                                {
                                    Softpower.SmartMaker.TopLight.ViewModels.LightJDoc pCurrentDoc = MainCanvas.CurrentDMTFrame.GetCurrentView().GetDocument() as Softpower.SmartMaker.TopLight.ViewModels.LightJDoc;

                                    if (null != pCurrentDoc && false == pCurrentDoc.IsDynamicMode && false == pCurrentDoc.IsEBookDoc && false == pCurrentDoc.TemplateMode) //2020-02-21 kys App모델 일때만 메뉴영역 생성
                                    {
                                        int nTop = Softpower.SmartMaker.TopApp.DeviceInformation.Models.GlobalDeviceInformation.TopBarHeight;
                                        int nBottom = Softpower.SmartMaker.TopApp.DeviceInformation.Models.GlobalDeviceInformation.BottomBarHeight;

                                        BarHeightChange(nTop, nBottom);
                                    }

                                    //20250305 KH QuizMetaData 저장
                                    pCurrentDoc.PageMetadata.QuizMetaData = pageMetadata.QuizMetaData;
                                }

                                //클립보드에 저장된 아톰정보 붙여넣기
                                object CacheData = GlobalClipboard.CacheData;

                                List<AtomBase> DeepCloneSourceAtoms = CacheData as List<AtomBase>;

                                DMTView currentDMTView = MainCanvas.CurrentDMTFrame.GetCurrentView() as DMTView;

                                // Z-Index 기준으로 정렬
                                DeepCloneSourceAtoms.Sort((x, y) => Grid.GetZIndex(x).CompareTo(Grid.GetZIndex(y)));

                                // 기준 아톰 찾기 (가장 왼쪽 위에 있는 아톰)
                                AtomBase referenceAtom = DeepCloneSourceAtoms.OrderBy(atom => atom.Margin.Left)
                                                                             .ThenBy(atom => atom.Margin.Top)
                                                                             .FirstOrDefault();

                                List<DeepCloneAtomCommand> cloneCommands = new List<DeepCloneAtomCommand>();

                                foreach (AtomBase sourceAtom in DeepCloneSourceAtoms)
                                {
                                    // 새 위치 계산 (기본위치 0, 0)
                                    double newLeft = sourceAtom.Margin.Left - referenceAtom.Margin.Left;
                                    double newTop = sourceAtom.Margin.Top - referenceAtom.Margin.Top;

                                    if (newLeft < 0) newLeft = 0;
                                    if (newTop < 0) newTop = 0;

                                    // 새 아톰 생성 명령 추가
                                    DeepCloneAtomCommand newCloneCommand = new DeepCloneAtomCommand(
                                        currentDMTView,
                                        sourceAtom.AtomCore.AtomType,
                                        newLeft,
                                        newTop,
                                        new Size(sourceAtom.Width, sourceAtom.Height),
                                        null,
                                        sourceAtom
                                    );

                                    //새로 생성될 model의 size를 EbookQuizViewAtom의 크기로 조정
                                    currentDMTViewHeight = sourceAtom.Width;
									currentDMTViewWidth = sourceAtom.Height;
                                    cloneCommands.Add(newCloneCommand);
                                }

                                // 붙여넣기 명령 실행
                                DeepCloneGroupedAtomsCommand pasteCommand = new DeepCloneGroupedAtomsCommand(cloneCommands);
								CommandCommander Commander = new CommandCommander();

                                Commander.AddCommand(pasteCommand);
                                var result = Commander.ExecuteCommand();

                                // UI 업데이트
                                if (result is List<AtomBase> pastedAtoms && pastedAtoms.Count > 0)
                                {
                                    pastedAtoms[0].AtomCore.Information.UpdatetPropertyToolBar(pastedAtoms[0]);
                                }

                                MainCanvas_OnNotifyToolBarNoCurrentSelectedAtomEvent(false);

								//EbookQuizViewAtom만 삭제하기

                                List<Atom> newAtomList = new List<Atom>();
                                newAtomList.Clear();
                                var oldAtomList = currentDMTView.GetAllAtomCores();

                                //view에서 QuizBlock을 제거하는 로직
                                foreach (var atom in oldAtomList)
                                {
                                    var atomElement = atom.GetOfAtom();

                                    if (atomElement.Parent != null)
                                    {
                                        var parent = atomElement.Parent as Panel;       // 부모가 Panel인지 확인
                                        parent?.Children.Remove(atomElement);
                                    }

                                    if (atomElement.ToString() == "AtomBase : 퀴즈블록")
                                    {
                                        continue;
                                    }

                                    newAtomList.Add(atom);
                                }

                                // 기존 Atom들을 다시 추가
                                foreach (var atom in newAtomList)
                                {
                                    var atomElement = atom.GetOfAtom();

                                    if (atomElement == null)
                                    {
                                        continue;    // null 체크 추가
                                    }

                                    // 중복 추가 방지 후 컨테이너에 추가
                                    if (!currentDMTView.Children.Contains(atomElement))
                                    {
                                        currentDMTView.Children.Add(atomElement);
                                    }
                                }

								DMTDoc activeDoc = GetActiveDocument() as DMTDoc;

								CDMTFrameAttrib frameAttrib = activeDoc.GetFrameAttrib() as CDMTFrameAttrib;

								frameAttrib.FrameSize = new Size(currentDMTViewHeight, currentDMTViewWidth);
                            }

                            FileSaveAs();

                            MainCanvas.CurrentDMTFrame.GetCurrentView().CloseModel();

                            GlobalWaitThread.WaitThread.End();

                            break;
                        }

                    default:
						break;
				}
			}

			GlobalEventReceiver.UniqueGlobalEventRecevier.ClearParamList ();


			return null;
		}

		#endregion

		#region |  void liWindow_OnUpdateLicenseType() : 라이선스 관련처리  |
		/// <summary>
		/// OnLicenseInfo 메서드에서 이벤트 핸들러 등록.
		/// liWindow.OnUpdateLicenseType 호출.
		/// </summary>
		void liWindow_OnUpdateLicenseType ()
		{

			SetLicenseInfo ();
		}

		#endregion

		#region |  void FileSaveEventHandler(object sender, EventArgs pArgs)  |
		/// <summary>
		/// FileOpen 메서드에서 이벤트 핸들러 등록.
		/// scrFrame.NewFileSave 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="pArgs"></param>
		private void FileSaveEventHandler (object sender, EventArgs pArgs)
		{
			((Window)sender).Activate ();
			FileSave ();
		}

		#endregion

		#region |  void DMTFrame_OnOffsetEventHandler(object sender, EventArgs e)  |
		/// <summary>
		/// CreateDMTDocument 메서드에서 이벤트 핸들러 등록.
		/// pDMTFrame.OnOffsetEventHandler 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DMTFrame_OnOffsetEventHandler (object sender, EventArgs e)
		{
			List<BaseFrame> frameCollecton = this.MainCanvas.GetAllFrame ();
			foreach (BaseFrame frame in frameCollecton)
			{
				TopDoc doc = frame.GetCurrentView ().GetDocument ();
				if (null != doc)
				{
					if (doc is DMTDoc)
					{
						DMTDoc dmtDoc = doc as DMTDoc;
						//dmtDoc.OffsetCount = 0;
					}
				}
			}
		}

		#endregion

		#region |  void ShowDatabaseEnviroment(object sender, EventArgs e)  |
		/// <summary>
		/// UserLoginX 메서드에서 이벤트 핸들러 등록.
		/// PQAppBase.UserLogin.ShowOptionDlg 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ShowDatabaseEnviroment (object sender, EventArgs e)
		{

			/*80 TopManageDialog manageDialog = new TopManageDialog(true);
			manageDialog.ShowDialog();*/
		}

		#endregion

		#region |  void OnDBDisConnect(object pSender, EventArgs pArgs)  |
		/// <summary>
		/// UserLoginX 메서드에서 이벤트 핸들러 등록.
		/// PQAppBase.UserLogin.DBDisConnect 호출.
		/// </summary>
		/// <param name="pSender"></param>
		/// <param name="pArgs"></param>
		private void OnDBDisConnect (object pSender, EventArgs pArgs)
		{

			ChangeStatusAndMenu (false);
		}

		#endregion

		#region |  void OnDBConnect(object pSender, EventArgs pArgs)  |
		/// <summary>
		/// UserLoginX 메서드에서 이벤트 핸들러 등록.
		/// PQAppBase.UserLogin.DBConnect 호출.
		/// </summary>
		/// <param name="pSender"></param>
		/// <param name="pArgs"></param>
		private void OnDBConnect (object pSender, EventArgs pArgs)
		{

			ChangeStatusAndMenu (PQAppBase.ConnectStatus);
		}

		#endregion

		#region |  void UserLogin_HostingInformationSetEvent() : 코드없음  |
		/// <summary>
		/// UserLoginX 메서드에서 이벤트 핸들러 등록.
		/// PQAppBase.UserLogin.HostingInformationSetEvent 호출.
		/// </summary>
		private void UserLogin_HostingInformationSetEvent ()
		{

			/*80 ServerPage sp = new ServerPage();
			sp.RadioAlone = true;
			sp.OnApplyDefault();*/
		}

		#endregion

		#region |  ----- 도구 처리 관련 -----  |

		#region |  void ScriptWindow_Activated(object sender, EventArgs e)  |
		/// <summary>
		/// OnShowFormScriptEditDialog 메서드에서 이벤트 핸들러 등록.
		/// mdiChildForm.Activated 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScriptWindow_Activated (object sender, EventArgs e)
		{
			UpdateMenuStateAll (sender);
		}

		#endregion

		#region |  void ScriptWindow80_Closed(object sender, EventArgs e)  |
		/// <summary>
		/// OnShowFormScriptEditDialog80 메서드에서 이벤트 핸들러 등록.
		/// mdiChildWindow.Closed 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ScriptWindow80_Closed (object sender, EventArgs e)
		{
			UpdateMenuStateAll (null);
		}

		#endregion

		#region |  void ScriptWindow80_Activated(object sender, EventArgs e)  |
		/// <summary>
		/// OnShowFormScriptEditDialog80 메서드에서 이벤트 핸들러 등록.
		/// mdiChildWindow.Activated 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ScriptWindow80_Activated (object sender, EventArgs e)
		{
			UpdateMenuStateAll (sender);
		}

		#endregion

		#region |  void MainWindow_NotifyFormScriptDialogOpen(object sender)  |
		/// <summary>
		/// MainCanvas_ShowAttPageEvent 이벤트에서 핸들러 등록.
		/// ((SmartFrameAttCore)atomAttCore).NotifyFormScriptDialogOpen 호출.
		/// </summary>
		/// <param name="sender"></param>
		private void MainWindow_NotifyFormScriptDialogOpen (object sender)
		{
			UpdateMenuStateAll (sender);
		}

		#endregion

		#region |  void pErdAutoGenWindow_OnShowErdManager(object sender, Softpower.SmartMaker.TopDBManager80.ErdEventArgs msg)  |
		/// <summary>
		/// OnAutoFormsErdCreator 메서드에서 이벤트 핸들러 등록.
		/// OnAutoErdCreator 메서드에서 이벤트 핸들러 등록.
		/// pErdAutoGenWindow.OnShowErdManager 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="msg"></param>
		void pErdAutoGenWindow_OnShowErdManager (object sender, Softpower.SmartMaker.TopDBManager80.ErdEventArgs msg)
		{

			msg.ServerConnectionString = GetConnectionString ();
			msg.FormInformationMap = new Hashtable ();

			foreach (string strFileName in msg.FilePaths)
			{
				DMTDoc pDMTDoc = ShadowFileOpen80 (strFileName);
				if (null != pDMTDoc)
				{
					string strFormName = pDMTDoc.GetFormName ();
					Hashtable htFormInformation = pDMTDoc.GetErdAutoInformation ();

					if (false == string.IsNullOrEmpty (strFormName) && 0 != htFormInformation.Keys.Count && false == msg.FormInformationMap.ContainsKey (strFormName))
					{
						msg.FormInformationMap.Add (strFormName, htFormInformation);
					}

					FFlushDoc (pDMTDoc);
				}
			}
		}

		#endregion

		#region |  void pErdAutoGenWindow_Closed(object sender, EventArgs e)  |
		/// <summary>
		/// OnAutoFormsErdCreator 메서드에서 이벤트 핸들러 등록.
		/// OnAutoErdCreator 메서드에서 이벤트 핸들러 등록.
		/// pErdAutoGenWindow.Closed 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void pErdAutoGenWindow_Closed (object sender, EventArgs e)
		{
			ErdAuto = false;
		}

		#endregion
		#endregion//도구 처리 관련

		#region |  ----- 설계도생성 / DB 관리자 -----  |
		#region |  void m_dbFieldTree80_Closed(object sender, EventArgs e)  |
		/// <summary>
		/// OnShowFieldSettingDialog 메서드에서 이벤트 핸들러 등록.
		/// m_dbFieldTree80.Closed 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void m_dbFieldTree80_Closed (object sender, EventArgs e)
		{
			m_dbFieldTree80 = null;
		}

		void m_dbHanaFieldTree_Closed (object sender, EventArgs e)
		{
			m_dbHanaFieldTree = null;
		}

		#endregion

		#region |  void pErdAutoGenManaer_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e) : 호출하는 곳 없음  |
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pErdAutoGenManaer_FormClosed (object sender, System.Windows.Forms.FormClosedEventArgs e)
		{
			ErdAuto = false;
		}

		#endregion

		#endregion//설계도생성 / DB 관리자

		#region |  ----- 서버연계 기능 -----  |

		// 서버설치
		private void OnServerInstall ()
		{
			if (false != PQAppBase.IsEnableMakerStore)
			{
				Softpower.SmartMaker.AwsServer.ServerWizard serverWizard = new Softpower.SmartMaker.AwsServer.ServerWizard ();
				serverWizard.CaptionIcon = MainMenu.AppImage.Source as BitmapImage;
				serverWizard.Owner = this;
				serverWizard.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
				serverWizard.ShowDialog ();
			}
		}

		// 구독약정 (서버 라이선스 메뉴로 통합)
		private void OnSubscriptionContract ()
		{
			if (false != PQAppBase.IsEnableMakerStore)
			{
				if (false == string.IsNullOrEmpty (_PQRemoting.HttpURL))
				{
					string strValue = ServerLicenseHelper.Instance.GetLicenseInfo (_PQRemoting.HttpURL, "License");
					if (false != string.IsNullOrEmpty (strValue))
					{
						LicenseService.ShowSmartServerMessage (SolutionService.ServerConsumer);
					}
					else
					{
						LicenseService.ShowSmartServerMessage (SolutionService.ServerBusiness);
					}
				}
				else
				{
				}
				//
				//Instance_OnNotifyLicenseEventOccured (3);
				//
			}
		}

		// 구독경신 (서버 라이선스 메뉴로 통합되어 사용되지 않음)
		private void OnSubscriptionRenew ()
		{
			if (false != PQAppBase.IsEnableMakerStore)
			{
				//Instance_OnNotifyLicenseEventOccured (4);
			}
		}

		// 모니터링
		private void OnShowServerTrace ()
		{
			if (false != PQAppBase.IsEnableMakerStore)
			{
				Softpower.SmartMaker.AwsServer.Page.Monitor.MonitoringWindow monitorWindow = new Softpower.SmartMaker.AwsServer.Page.Monitor.MonitoringWindow ();
				monitorWindow.CaptionIcon = MainMenu.AppImage.Source as BitmapImage;
				monitorWindow.Owner = this;
				monitorWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
				monitorWindow.ShowWindow ();
			}
		}

		/// <summary>
		/// 라이선스 구매 및 연장
		/// </summary>
		/// <param name="nValue"></param>
		private void Instance_OnNotifyLicenseEventOccured (int nValue)
		{
			if (false != PQAppBase.IsEnableMakerStore)
			{
				switch (nValue)
				{
					case 1:
						SubsidiaryWindow.ShowLicensePage ("품목정보(라이선스).QPM", "다이렉트:1,라이선스종류:PLUS"); // 저작솔루션 구매 (Plus)
						break;
					case 2:
						SubsidiaryWindow.ShowLicensePage ("품목정보(라이선스).QPM", "다이렉트:1,라이선스종류:AI+"); // 저작솔루션 구매 (AI+)
						break;
					case 3:
						SubsidiaryWindow.ShowLicensePage ("품목정보(라이선스).QPM", "다이렉트:1,라이선스종류:PRO"); // 저작솔루션 구매 (Pro)
						break;
					case 4:
						SubsidiaryWindow.ShowLicensePage ("품목정보(라이선스).QPM", "다이렉트:1,라이선스종류:ENT"); // 저작솔루션 구매 (ENT)
						break;
					case 5:
						SubsidiaryWindow.ShowLicensePage ("Showcase(라이선스).QPM", "다이렉트:1,정규식조건:스마트서버,라이선스결제타입:서버구입"); // 서버솔루션 구매
						break;
					case 6:
						SubsidiaryWindow.ShowLicensePage ("Showcase(라이선스).QPM", "다이렉트:1,정규식조건:스마트서버,라이선스결제타입:서버구입"); // 서버솔루션 갱신
						break;
				}
			}
		}

		#endregion // 서버연계 기능 |

		#region  |  ----- 탬플릿 기능 -----  |


		private void LinkMakeStore (int nType)
		{
			if (LC.LANG.KOREAN == LC.PQLanguage || LC.LANG.JAPAN == LC.PQLanguage)
			{
				switch (nType)
				{
					case 0:
						//App모델 메뉴얼
						SubsidiaryWindow.ShowLicensePage ("품목정보.QPM", "다이렉트:1,참조번호:201907100050");
						break;
					case 1:
						//Book모델 메뉴얼
						SubsidiaryWindow.ShowLicensePage ("품목정보.QPM", "다이렉트:1,참조번호:201907100051");
						break;
					case 2:
						//Web모델 메뉴얼
						SubsidiaryWindow.ShowLicensePage ("품목정보.QPM", "다이렉트:1,참조번호:201907100052");
						break;
					default:
						break;
				}
			}
		}

		#endregion

		#region  |  ----- 기준장치 설정(상단, 하단Bar 높이 설정) -----  |

		private void BarHeightChange (int nTopBarHeight, int nBottomBarHeight)
		{
			TopDoc pDoc = this.GetActiveDocument () as TopDoc;
			DMTView pView = pDoc?.GetParentView () as DMTView;
			DMTFrame frame = pView?.GetFrame () as DMTFrame;

			//퀴즈블록 등 메뉴영역이 없는경우 리턴처리
			if (null == frame?.CurrentPhoneScreenView)
				return;

			frame.CurrentPhoneScreenView.BarSetting (nTopBarHeight, nBottomBarHeight);
			Softpower.SmartMaker.TopLight.ViewModels.LightJDoc pJDoc = pDoc as Softpower.SmartMaker.TopLight.ViewModels.LightJDoc;
			if (null != pJDoc)
			{
				Softpower.SmartMaker.TopLight.Models.CFrameAttrib pFrameAttrib = pJDoc.GetFrameAttrib () as Softpower.SmartMaker.TopLight.Models.CFrameAttrib;
				if (null != pFrameAttrib)
				{
					if (-1 != nTopBarHeight)
					{
						pFrameAttrib.TopBarHeight = nTopBarHeight;
					}

					if (-1 != nBottomBarHeight)
					{
						pFrameAttrib.BottomBarHeight = nBottomBarHeight;

						if (0 < nBottomBarHeight)
						{
							pFrameAttrib.BottomBarStartHeight = (int)(pFrameAttrib.FrameSize.Height - frame.CurrentPhoneScreenView.CurrentPhoneStatusBar.Height - frame.CurrentPhoneScreenView.SystemNavigation.Height - nBottomBarHeight);
							pFrameAttrib.BottomBarEndHeight = (int)(pFrameAttrib.FrameSize.Height - frame.CurrentPhoneScreenView.CurrentPhoneStatusBar.Height - frame.CurrentPhoneScreenView.SystemNavigation.Height);
						}
						else
						{
							pFrameAttrib.BottomBarStartHeight = 0;
							pFrameAttrib.BottomBarEndHeight = 0;
						}
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// 자동 등록 이벤트 : ((Softpower.SmartMaker.TopBuild.MainWindow)(target)).Drop 호출
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Drop (object sender, DragEventArgs e)
		{

			//80 추후작업
			//if (false == m_pIsFreeVersion)
			//{
			//    SecurityFile moduleSecurity = PrdtLcnManager.ModuleSecurity;
			//    if (false == moduleSecurity.IsRegistedLicense())
			//        return;
			//}

			if (false != e.Data.GetDataPresent (DataFormats.FileDrop))
			{
				object pObj = e.Data.GetData (DataFormats.FileDrop);
				string[] pFiles = pObj as string[];
				foreach (string filename in pFiles)
				{
					//this.SetProgKind(PROG_KIND._SMT);
					if (this.FileOpen (filename))
					{
						string strFileName = filename;
						m_MainWindowManager.AddRecentFile (strFileName);
					}
				}
			}

			//2025-01-09 kys 퀴즈메이커 Drag & Drop 의 경우 뷰쪽에 이벤트가 호출되지 않아 임의로 여기서 호출하도록 처리함
			//이전 편집모드 이슈로 인해 아톰이 없는 상태에서 Frame / View에 포커스 인식 이슈가 있는것으로 보이지기 때문에 추후 편집기능 보강시 이부분 소스 제거해야함
			if (null != MainCanvas.CurrentDMTFrame)
			{
				if (true == e.Data.GetDataPresent ("QuizInfo"))
				{
					if (MainCanvas.CurrentDMTFrame.GetCurrentView () is DMTView dmtView)
					{
						dmtView.DMTView_Drop (sender, e);
					}
				}
			}
		}

		#region |  void Window_DragOver(object sender, DragEventArgs e)  |
		/// <summary>
		/// 자동등록 이벤트 : ((Softpower.SmartMaker.TopBuild.MainWindow)(target)).DragOver 호출.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_DragOver (object sender, DragEventArgs e)
		{
			if (false != e.Data.GetDataPresent (DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Link | DragDropEffects.Move | DragDropEffects.Copy;
			}
			else if (true == e.Data.GetDataPresent ("SlideMaster") || true == e.Data.GetDataPresent ("QuizInfo"))
			{

			}
			else
			{
				e.Effects = DragDropEffects.None;
			}

			//if (false != e.Data.GetDataPresent(DataFormats.FileDrop))
			//{
			//    e.Effects = DragDropEffects.Link;
			//}
			//else
			//{
			//    e.Effects = DragDropEffects.None;
			//}

			//e.Handled = true;
		}

		#endregion

		#endregion//이벤트

		#region |  ##### Public Static 메서드 #####  |

		#region |  Point GetMousePosition() : 마우스 위치의 모니터 정보  |

		public static Point GetMousePosition ()
		{

			Win32Point w32Mouse = new Win32Point ();
			GetCursorPos (ref w32Mouse);
			return new Point (w32Mouse.X, w32Mouse.Y);
		}

		#endregion

		#endregion//Public Static 메서드


		#region |  ##### Public Virtual 메서드 #####  |

		#region |  void FileSaveAs() : 파일저장 처리관련  |
		/// <summary>
		/// 다른저장
		/// </summary>
		public virtual void FileSaveAs ()
		{
			TopDoc pDoc = GetActiveDocument ();
			if (null == pDoc)
			{
				pDoc = ExecuteMenuManager.Instance.GetActiveDocument ();
			}

			if (null != pDoc)
			{
				FileSaveAs (pDoc);
			}
		}

		#endregion

		#endregion//Public Virtual 메서드


		#region |  ##### Public 메서드 #####  |

		#region |  void UpdateMenuStateAll(object objTarget)  |
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTarget">DMTFrame, Menu, Script, null</param>
		/// <returns></returns>
		public void UpdateMenuStateAll (object objTarget)
		{
			MainToolBarPanel.UpdateToolBarState (objTarget);
			MainExpandMenuContainer.UpdateMenuState (objTarget);
			MainMenu.UpdateMainMenuState (objTarget);

			//null이 넘어올경우에 한해 체크
			//모든창이 닫혔을때는 왼쪽 메뉴 영역을 숨김
			if (null == objTarget)
			{
				if (true == IsAllClosed ())
				{
					Storyboard MainAppAtomMenuBackOutEnd = AnimationManager.GetAnimationByName ("AtomMenuContainerBackOutEnd");
					Storyboard MainScrollViewerBackOutEnd = AnimationManager.GetAnimationByName ("MainScrollViewerBackOutEnd");

					MainAppAtomMenuBackOutEnd.Begin (MainExpandMenuContainer);
					MainScrollViewerBackOutEnd.Begin (MainScrollViewer);

					MainExpandMenuContainer.InitMenuState ();
				}
			}
		}

		#endregion

		#region |  void TransmitDocInformation() : Jetty 서버 관련  |

		public void TransmitDocInformation ()
		{
			//if (false == m_pJettyServer.JettyRun)
			//    return;

			//SkinFrame CurrentFrame = MainCanvas.CurrentDMTFrame;
			//if (null != CurrentFrame)
			//{
			//    TopView CurrentDMTView = CurrentFrame.GetCurrentView();
			//    TopDoc pTopDoc = CurrentDMTView.GetDocument();

			//    if (pTopDoc.GetType() != typeof(DMTDoc))
			//        return;

			//    DMTDoc pDoc = pTopDoc as DMTDoc;

			//    if (null != pDoc)
			//    {
			//        string strFilePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "tempDocument");
			//        pDoc.FilePath = strFilePath;
			//        pDoc.IsDocCopy = true;

			//        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
			//        {
			//            if (true == pDoc.SaveDocument())
			//            {
			//                string topcommString = "";
			//                string depString = "";

			//                string strDepCode = string.Empty;
			//                if (null != PQAppBase.UserLogin)
			//                {
			//                    CDSInfo topcommInfo = PQAppBase.UserLogin.GetTopCommDataSourceInfo();
			//                    string owner = topcommInfo.Owner;
			//                    string[] values = owner.Split(new char[] { '.' });
			//                    if (2 == values.Length)
			//                    {
			//                        owner = values[1];
			//                    }
			//                    topcommString = string.Format("{0}${1}${2}${3}${4}${5}",
			//                            topcommInfo.DepCode, topcommInfo.DepName, topcommInfo.DBName, topcommInfo.ModuleKey, topcommInfo.DBKind + 1, owner);

			//                    CDSInfo pInfo = PQAppBase.UserLogin.GetFirstDepDataSourceInfo();
			//                    if (null != pInfo)
			//                    {
			//                        strDepCode = pInfo.DepCode;

			//                        owner = pInfo.Owner;
			//                        values = owner.Split(new char[] { '.' });
			//                        if (2 == values.Length)
			//                        {
			//                            owner = values[1];
			//                        }
			//                        depString = string.Format("{0}${1}${2}${3}${4}${5}",
			//                            pInfo.DepCode, pInfo.DepName, pInfo.DBName, pInfo.ModuleKey, pInfo.DBKind + 1, owner);
			//                    }
			//                }

			//                string strID = null != PQAppBase.UserLogin ? PQAppBase.UserLogin.LoginID : string.Empty;
			//                string strPW = null != PQAppBase.UserLogin ? PQAppBase.UserLogin.LoginPWD : string.Empty;


			//                string strURL = string.Format(@"http://localhost:{0}/ups/Emulate.do?sn=start&mp={1}&li={2};{3};{4}&ts={5}&ds={6}",
			//                    PQAppBase.EmulatorPort, strFilePath, strDepCode, strID, strPW, topcommString, depString);
			//                strURL = strURL.Replace("\\", "/");
			//                string strReault = Softpower.SmartMaker.TopWebAtom.Component.ServerImage.SetHttpAction(strURL, "");
			//            }
			//        }));
			//    }
			//}
		}

		#endregion

		#region |  Rectangle GetNowScreenInMousePosition() : 마우스 위치의 모니터 정보  |

		public System.Drawing.Rectangle GetNowScreenInMousePosition ()
		{

			int nScreenCount = 0;

			nScreenCount = System.Windows.Forms.Screen.AllScreens.Length;
			System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.AllScreens[0].WorkingArea;

			if (nScreenCount < 2)
			{
				return workingArea;
			}
			else
			{
				Point point = GetMousePosition ();
				System.Drawing.Rectangle workingAreaSecond = System.Windows.Forms.Screen.AllScreens[1].WorkingArea;

				if ((int)point.X > workingArea.Width)
					return workingAreaSecond;
				else
					return workingArea;

			}
		}

		#endregion

		#region |  TopDoc GetActiveDocument()  |

		public TopDoc GetActiveDocument ()
		{
			TopDoc pDoc = null;

			WindowCollection childWindows = this.OwnedWindows;
			foreach (Window cWindow in childWindows)
			{
				if (cWindow is CScrFrame && true == cWindow.IsActive)
				{
					CScrFrame scrFrame = cWindow as CScrFrame;
					pDoc = scrFrame.Document;
					break;
				}
			}

			if (null == pDoc)
			{
				BaseFrame currentFrame = this.MainCanvas.CurrentDMTFrame as BaseFrame;
				if (null != currentFrame)
				{
					TopView currentView = currentFrame.GetCurrentView ();
					if (null != currentView)
					{
						pDoc = currentView.GetDocument ();
					}
				}
			}

			return pDoc;
		}

		#endregion

		#region |  void OnLogin(string strMenuText)  |
		/// <summary>
		/// 서버접속
		/// </summary>
		public void OnLogin (string strMenuText)
		{
			if (MainMenuDef.ServerConnectionMenuItem == strMenuText)
			{
				UserLoginX (false);
			}
			else
			{
				if (1 == PQAppBase.CompanyCode)    //한화 AIDT OIDC 기능
				{
					ChromeBrowserManager.Instance.DeleteCacheDirectory ();

					PQAppBase.ConnectStatus = false;
					ChangeStatusAndMenu (false);
				}
				else
				{
					CUserLogin _login = PQAppBase.UserLogin;
					if (null != _login)
					{
						_login.UserLogout ();
						_login.DisConnect ();
					}
				}
			}
		}

		#endregion

		#region |  ----- 파일저장 처리관련 -----  |
		#region |  void FileSaveAs(TopDoc pDoc) : 파일저장 처리관련  |

		public void FileSaveAs (TopDoc pDoc)
		{
			if (null == pDoc)
			{
				return;
			}

			if (pDoc is QuizMakerDoc)
			{
				QuizMakerFrame quizMakerFrame = (pDoc.GetParentView () as QuizMakerView)?.GetFrame () as QuizMakerFrame;
				if (true == quizMakerFrame?.IsMultiMode)
				{
					pDoc = quizMakerFrame.ParentMultiFrame.CurrentDMTView.Document;
				}
			}

			//옵션 전달자 초기화
			OptionHelper.Unit.ClearOptionInfo ();

			DOC_KIND docKind = pDoc.DocType;

			GlobalWaitThread.WaitThread.Start (this);

			Softpower.SmartMaker.FileDBIO80.FileDBWindow fileDialog = new Softpower.SmartMaker.FileDBIO80.FileDBWindow (false, PROG_KIND._SMT, docKind, "");
			if (false == pDoc.TemplateMode)
			{
				fileDialog.FilePath = pDoc.FilePath;
			}
			else
				fileDialog.FilePath = PQAppBase.DefaultPath;

			if (pDoc is DMTDoc)
			{
				DMTDoc document = pDoc as DMTDoc;
				CDMTFrameAttrib frameAttrib = document.GetFrameAttrib () as CDMTFrameAttrib;
				fileDialog.FileName = GetRealFileName (frameAttrib.Title);
			}
			else if (pDoc is ReportDMTDoc)
			{
				ReportDMTFrame reportFrame = MainCanvas.CurrentDMTFrame as ReportDMTFrame;
				fileDialog.FileName = reportFrame.CurrentCaptionBar.CaptionTitle;

			}
			else if (pDoc is CTopMenuDoc)
			{
				fileDialog.FileName = string.Format (LC.GS ("SmartOffice_MainWindow_20"), ExecuteMenuManager.Instance.MenuIndex);
			}

			if (docKind == DOC_KIND._docProcess && ((DMTDoc)pDoc).m_nNtoaDocKind == 1)
			{
				string strProp = ExtensionInfo.GetPropertyFromFileName (pDoc.FilePath);
				fileDialog.DBProperty = _Kiss._isempty (strProp) ? "A" : strProp;
			}
			else if (docKind == DOC_KIND._docWeb)   // 웹페이지모델
			{
				string strProp = ExtensionInfo.GetPropertyFromFileName (pDoc.FilePath);
				fileDialog.DBProperty = _Kiss._isempty (strProp) ? "2" : strProp;
			}
			else if (docKind == DOC_KIND._docSmart)
			{
				string strProp = ExtensionInfo.GetPropertyFromFileName (pDoc.FilePath);
				fileDialog.DBProperty = _Kiss._isempty (strProp) ? "3" : strProp;
			}
			else if (docKind == DOC_KIND._docEBook || docKind == DOC_KIND._docSlideMaster)
			{
				string strProp = ExtensionInfo.GetPropertyFromFileName (pDoc.FilePath);
				fileDialog.DBProperty = _Kiss._isempty (strProp) ? "O" : strProp;

				//폰트 저장옵션
				DMTDoc document = pDoc as DMTDoc;
				OptionHelper.Unit.FontSaveOption = ((EBookManager)document.EBookManager).FontSaveOption;
			}
			else if (docKind == DOC_KIND._docReport)
			{
				string strProp = ExtensionInfo.GetPropertyFromFileName (pDoc.FilePath);
				fileDialog.DBProperty = _Kiss._isempty (strProp) ? "4" : strProp;
			}
			else if (pDoc is ScriptDoc) // 전역스크립트
			{
				fileDialog.DBProperty = "5";
			}

			if (fileDialog.ShowDialog () == true)
			{
				if (false != fileDialog.IsDBFile)
				{
					// 2007.12.24 이정대, 스크립트 사용을 위하여 이름(모델명)을 준다.
					string strModelName = fileDialog.FileName;
					// 2007.12.29 이정대, 모델명이 "프로세스"로 시작할 경우에는 저장시 모델명 변경

					//if (DOC_KIND._docProcess == docKind && fileDialog.DBProperty.Equals ("0"))
					if (docKind == DOC_KIND._docSmart)
					{
						if (this.GetActiveDocument () is DMTDoc pDMTDoc)
						{
							string strFormname = pDMTDoc.GetFormName ();
							if (0 != strFormname.Length && 3 < strFormname.Length)
							{
								int nDefaultLength = pDMTDoc.GetDefaultFormName ().Length; //다국어 번역에 따라 글자수가 변경되기 때문에 예외처리함

								if (strFormname.Substring (0, nDefaultLength).Equals (pDMTDoc.GetDefaultFormName ()))
									pDMTDoc.SetFormTitle (strModelName);
							}
						}
					}

					pDoc.DBRegNumber = fileDialog.DBRegNumber;
					pDoc.FilePath = fileDialog.FilePath;
					this.DBFileProperty = fileDialog.DBProperty;
					CScriptApp.FormProperty = fileDialog.DBProperty;
					DBFileSave (docKind, pDoc, false);
				}
				else if (false != fileDialog.IsAwsS3File)
				{
					string strModelName = fileDialog.FileName;
					string strModelPath = fileDialog.FilePath;

					if (docKind == DOC_KIND._docSmart)
					{
						if (this.GetActiveDocument () is DMTDoc pDMTDoc)
						{
							string strFormname = pDMTDoc.GetFormName ();
							if (0 != strFormname.Length && 3 < strFormname.Length)
							{
								int nDefaultLength = pDMTDoc.GetDefaultFormName ().Length; //다국어 번역에 따라 글자수가 변경되기 때문에 예외처리함
								if (strFormname.Substring (0, nDefaultLength).Equals (pDMTDoc.GetDefaultFormName ()))
									pDMTDoc.SetFormTitle (strModelName);
							}
						}
					}

					AwsS3FileSave (pDoc, docKind, strModelPath);
				}
				else
				{
					string strModelName = string.Empty;

					// 2007.12.24 이정대, 스크립트 사용을 위하여 이름(모델명)을 준다.
					if (true == fileDialog.FileName.Contains ("."))
					{
						strModelName = fileDialog.FileName.Substring (0, fileDialog.FileName.LastIndexOf ("."));
					}
					else
					{
						strModelName = fileDialog.FileName;
					}
					//	2007.12.29 이정대, 모델명이 "프로세스"로 시작할 경우에는 저장시 모델명 변경

					//if (DOC_KIND._docProcess == docKind && fileDialog.DBProperty.Equals ("0"))
					DMTDoc pDMTDoc = pDoc as DMTDoc;

					if ((DOC_KIND._docSmart == docKind || DOC_KIND._docProcess == docKind)
						&& fileDialog.DBProperty.Equals ("3"))
					{
						if (null != pDMTDoc)
						{
							//string strFormname = pDMTDoc.GetFormName();
							//if (0 != strFormname.Length && 3 < strFormname.Length)
							//{
							//    int nDefaultLength = pDMTDoc.GetDefaultFormName().Length; //다국어 번역에 따라 글자수가 변경되기 때문에 예외처리함

							//    if (strFormname.Substring(0, nDefaultLength).Equals(pDMTDoc.GetDefaultFormName()))
							//    {
							//        pDMTDoc.SetFormTitle(strModelName);
							//        ChangeCaptionText();
							//    }
							//}


							pDMTDoc.SetFormTitle (strModelName);
							if (pDMTDoc.FilePath == "") pDMTDoc.SetFormName (strModelName);
							ChangeCaptionText ();

							pDMTDoc.CanSaveDocument = true;
						}
					}
					//else if (DOC_KIND._docWeb == docKind && fileDialog.DBProperty.Equals("2"))
					//{
					//    pDMTDoc.SetFormTitle(strModelName);
					//    pDMTDoc.CanSaveDocument = true;
					//}
					else if (DOC_KIND._docMenu != docKind && null != pDMTDoc)
					{
						pDMTDoc.SetFormTitle (strModelName);
						pDMTDoc.CanSaveDocument = true;
					}
					else if (DOC_KIND._docReport == docKind)
					{
						ReportDMTDoc reportDoc = pDoc as ReportDMTDoc;

						reportDoc.ExecuteChangeTitleName (strModelName);
						reportDoc.IsFormChanged = false;
					}

					//폰트저장 옵션
					if (null != pDMTDoc && true == pDMTDoc.IsEBookDoc)
					{
						((EBookManager)pDMTDoc.EBookManager).FontSaveOption = OptionHelper.Unit.FontSaveOption;
					}

					FileSave (pDoc, docKind, fileDialog.FilePath);

					// 2007.12.11 이정대, 모델뱅크(DB->로컬저장시)에서 열고, 수정후 Ctrl+S 로 저장시 RegNumber를 없애 주어야 한다
					// 나중에 RegNumber가 있으면, 디비 파일로 인식된어, Ctrl+S 가 적용되지 않는다.
					pDoc.DBRegNumber = string.Empty;
				}

				//2024-07-05 kys 탬플릿을 한번 저장한경우에는 TemplateMode을 false로 설정한다.
				if (true == pDoc.TemplateMode)
					pDoc.TemplateMode = false;
			}

		}

		#endregion

		#region |  void FileSave(TopDoc pDoc, DOC_KIND docKind, string strFileName) : 파일저장 처리관련  |

		public void FileSave (TopDoc pDoc, DOC_KIND docKind, string strFileName)
		{
			bool bFTPSaveFile = false;
			string strPath = strFileName;
			if (0 == strFileName.IndexOf ("FTP:\\"))
			{
				bFTPSaveFile = true;
				strPath = string.Concat (PQAppBase.BosPath, "FTPTempFiles");

				if (false == System.IO.Directory.Exists (strPath))
					System.IO.Directory.CreateDirectory (strPath);

				strPath = string.Concat (strPath, strFileName.Substring (strFileName.LastIndexOf ("\\")));
			}

			if (null != pDoc)
			{
				pDoc.FilePath = strPath;

				if (true == strPath.EndsWith ("QMF"))
				{
					//80 QMF 추후진행
					//PQAppBase.IsQMFFile = true;
					//pDoc.OnSaveQMFDocument(strPath);
				}
				else if (true == strPath.EndsWith ("QEB") || true == strPath.EndsWith ("QEBM"))
				{
					SkinFrame currentFrame = MainCanvas.CurrentDMTFrame as SkinFrame;
					if (pDoc == currentFrame.GetCurrentView ().GetDocument ())
					{
						if (false == EduTechLicenseHelper.Instance.IsSaveFolder (Path.GetDirectoryName (strPath)))
						{
							return;
						}

						if (true == currentFrame.OnSaveEBookModel (strPath))
						{
							m_MainWindowManager.AddRecentFile (strFileName);
						}
						else
						{
							pDoc.FilePath = string.Empty;
						}
					}
				}
				else
				{
					if (false != pDoc.OnSaveDocument (strPath))
					{
						m_MainWindowManager.AddRecentFile (strFileName);
					}
					else
					{
						pDoc.FilePath = string.Empty;
					}
				}
			}

			if (false != bFTPSaveFile)
			{
				strFileName = strFileName.Substring (strFileName.IndexOf ("\\") + 1);
				string strEnv = strFileName.Substring (0, strFileName.IndexOf ("\\"));

				string[] strEnvs = strEnv.Split (new char[] { '&' });

				Softpower.SmartMaker.TopApp.FtpLib pFtpClient = new FtpLib ();
				pFtpClient.SetConnectInfo (strEnvs[0], Convert.ToInt32 (strEnvs[1]), strEnvs[2], strEnvs[3]);
				pFtpClient.RemotePath = strEnvs[4];

				GlobalWaitThread.WaitThread.Start (this);

				if (false != pFtpClient.Login ())
				{
					if (false != pDoc?.IsSaveWebPage ())
					{
						strPath = strPath.Substring (0, strPath.LastIndexOf ("."));
						if (docKind == DOC_KIND._docProcess)
						{
							pFtpClient.Upload (strPath + ".xml");
							pFtpClient.Upload (strPath + "_Q.xml");
							pFtpClient.Upload (strPath + "_O.xml");
							pFtpClient.Upload (strPath + ".WQSC");
						}
						else if (docKind == DOC_KIND._docMenu)
						{
							pFtpClient.Upload (strPath + ".QMX");
						}
						else if (docKind == DOC_KIND._docWeb)
						{
							strPath = strPath.Substring (0, strPath.LastIndexOf ("\\") + 1);

							pFtpClient.Upload (strPath + "test.html");
							pFtpClient.Upload (strPath + "frame.html");
						}
					}
					else
					{
						pFtpClient.Upload (strPath);
						if (docKind == DOC_KIND._docSmart)
						{
							strPath = string.Concat (strPath, ".obj");
							pFtpClient.Upload (strPath);
						}
					}

					pFtpClient.Logout ();
				}

				GlobalWaitThread.WaitThread.End ();
			}
		}

		#endregion

		#region |  void DBFileSave(DOC_KIND docKind, TopDoc pDoc, bool bModelSave) : 파일저장 처리관련  |

		public void DBFileSave (DOC_KIND docKind, TopDoc pDoc, bool bModelSave)
		{
			if (null == pDoc)
				return;

			//파일이 존재하는지 확인한다.
			Softpower.SmartMaker.FileDBIO80.FileDBIO fileio = new Softpower.SmartMaker.FileDBIO80.FileDBIO (bModelSave);
			string strProperty = ExtensionInfo.GetPropertyFromFileName (pDoc.GetFileName ());
			if (true == string.IsNullOrEmpty (strProperty))
			{
				strProperty = this.DBFileProperty;
			}

			Softpower.SmartMaker.FileDBIO80.DBFileItem dbFileItem = new Softpower.SmartMaker.FileDBIO80.DBFileItem (pDoc.DBRegNumber, pDoc.GetFileName (), strProperty);
			string[] strValueAry = dbFileItem.GetDBFileIOInfoInArray ();

			if (true != fileio.IsExistFile (pDoc.DBRegNumber))
			{
				// insert문 실행 (F12를 제외한 나머지 정보를 Insert한다)
				fileio.ExecuteInsert (strValueAry);
			}

			{
				// update문 실행
				fileio.ExecuteUpdate (strValueAry);
			}

			if (false == pDoc.OnDBSaveDocument (pDoc.FilePath, pDoc.DBRegNumber, bModelSave))
			{
				if (DOC_KIND._docScript == docKind)
					pDoc.FilePath = string.Empty;
				return;
			}

			//80 추후작업 
			//LHS 소스 줄임 (2005.04.11)
			//Form pActiveFrame = GetActiveMdiChild();
			//if (null != pActiveFrame)
			//    pActiveFrame.Text = pDoc.GetFileName();

			// 2009.01.16 황성민
			// 파일 저장시 최근파일 목록에 추가
			m_MainWindowManager.AddRecentFile (pDoc.GetFileName ());
		}

		#endregion

		#region |  void AwsS3FileSave(TopDoc pDoc, DOC_KIND docKind, string strModelPath) : 파일저장 처리관련  |

		public void AwsS3FileSave (TopDoc pDoc, DOC_KIND docKind, string strModelPath)
		{
			string strTempPath = AwsS3Manager.GetRelativeFilePath (strModelPath);
			string strDirectory = Path.GetDirectoryName (strTempPath);
			string strFileName = Path.GetFileName (strModelPath);

			if (false != strDirectory.Equals (strFileName))
				strDirectory = "";

			string strTempFolder = Path.Combine (PQAppBase.BosPath, "AwsS3TempFiles", strDirectory);

			if (false == System.IO.Directory.Exists (strTempFolder))
				System.IO.Directory.CreateDirectory (strTempFolder);

			string strFilePath = Path.Combine (strTempFolder, strFileName);
			pDoc.FilePath = strFilePath;

			if (false != pDoc.OnSaveDocument (strFilePath))
			{
				AwsS3Manager.ThreadUploadFile (strDirectory, strFilePath);
			}
		}

		#endregion

		#region |  void AwsS3FileOpen(string strAwsS3FilePath) : 파일저장 처리관련  |

		public void AwsS3FileOpen (string strAwsS3FilePath)
		{

			string strTempPath = AwsS3Manager.GetRelativeFilePath (strAwsS3FilePath);
			string strFolder = Path.GetDirectoryName (strTempPath);
			string strFileName = Path.GetFileName (strTempPath);

			string strFilePath = AwsS3Manager.GetRelativeFilePath (strAwsS3FilePath);

			string strTempFolder = Path.Combine (PQAppBase.BosPath, "AwsS3TempFiles", strFolder);

			if (false == System.IO.Directory.Exists (strTempFolder))
				System.IO.Directory.CreateDirectory (strTempFolder);

			string strLocalFilePath = Path.Combine (strTempFolder, strFileName);

			bool bDownload = AwsS3Manager.ThreadDownloadFile (strTempPath, strLocalFilePath);
			if (false != bDownload)
			{
				if (File.Exists (strLocalFilePath))
				{
					FileOpen (strLocalFilePath);
				}
			}
		}

		#endregion
		#endregion//파일저장 처리관련

		#region |  void OnShowDBManagerDialog() : 디비 처리 객체  |

		public void OnShowDBManagerDialog ()
		{

			if (null != MainCanvas.CurrentDMTFrame)
			{
				MainCanvas.CurrentDMTFrame.OnUserSql (this, true);
			}
		}

		#endregion

		#region |  ----- 설계도생성 / DB 관리자 -----  |
		#region |  DMTDoc ShadowFileOpen(string strFileName)  |

		public DMTDoc ShadowFileOpen (string strFileName)
		{

			DOC_KIND nDocKind = ExtensionInfo.GetDocKindFromFileName (strFileName);
			string strProperty = ExtensionInfo.GetPropertyFromFileName (strFileName);

			bool bNtoa = false;
			if (nDocKind == DOC_KIND._docProcess || nDocKind == DOC_KIND._docSmart)
			{
				string strExt = System.IO.Path.GetExtension (strFileName).ToUpper ();
				bNtoa = strExt != ".QPG" ? true : false;
			}

			TopDoc pDoc = CreateSaveDocument (nDocKind, strProperty, false, true);

			if (DOC_KIND._docProcess == nDocKind || DOC_KIND._docSmart == nDocKind)
				((DMTDoc)pDoc).SetGlobalPtr ((object)m_pJobGInfo);

			pDoc.FilePath = strFileName;
			pDoc.CONVERT = true;

			// 모듈키 재생성 방지 위해 임의로 제품 아이디 변경
			PQAppBase.ProgramID = PROG_ID.PQ_NONE;
			pDoc.OnOpenDocument (strFileName);
			PQAppBase.ProgramID = PROG_ID.PQ_PB;

			return pDoc as DMTDoc;
		}

		#endregion

		#region |  DMTDoc ShadowFileOpen80(string strFilePath)  |

		public DMTDoc ShadowFileOpen80 (string strFilePath)
		{
			DMTFrame appModelDMTFrame = new DMTFrame (DOC_KIND._docNone);

			DMTView objDMTView = null;
			DMTDoc appModelDocument = null;

			string strModulePath = "";
			string strModulPath = PQAppBase.ModulePath;
			string tempPQAppBase = PQAppBase.DefaultPath;

			PQAppBase.ModulePath = _Registry.ReadString ("MODULE", "IsDir");
			PQAppBase.ProgramID = PROG_ID.PQ_NONE; // Deserialize 시점에서 PROG_ID.PQ_PB 이면 모듈키를 초기화 시키기 때문에 임시로 변경후 롤백처리.
												   //PQAppBase.IsGeneratorMode = true;
			PQAppBase.DefaultPath = strModulePath;

			objDMTView = appModelDMTFrame.GetCurrentView () as DMTView;

			appModelDocument = appModelDMTFrame.GetCurrentView ().GetDocument () as DMTDoc;
			appModelDocument.FilePath = strFilePath;
			appModelDocument.LicensePass = true;
			appModelDocument.OnOpenDocument (strFilePath);

			//objDMTView.Background = appModelDMTFrame.CurrentPhoneScreenView.CurrentFakeView.Background;

			//PQAppBase.IsGeneratorMode = false;
			PQAppBase.ProgramID = PROG_ID.PQ_PB; // Deserialize 시점에서 PROG_ID.PQ_PB 이면 모듈키를 초기화 시키기 때문에 임시로 변경후 롤백처리.
			PQAppBase.ModulePath = strModulPath;
			PQAppBase.DefaultPath = tempPQAppBase;

			return appModelDocument as DMTDoc;

		}

		#endregion
		#endregion//설계도생성 / DB 관리자

		#region |  ----- MDI 관련 -----  |
		#region |  void AddChildMiniUnit(IMDIChildWindow childWindow, Point ptChildWindow)  |
		/// <summary>
		/// 최소화된 창을 더한다 
		/// 위치를 잡아준다
		/// </summary>
		/// <param name="minimizedUnit"></param>
		public void AddChildMiniUnit (IMDIChildWindow childWindow, Point ptChildWindow)
		{
			Point ptMinimizeOnMainCanvas = MainCanvas.PointFromScreen (ptChildWindow);
			MinimizedUnit miniUnit = childWindow.MinimizedUnit;
			miniUnit.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

			double dUnitLeft = -1;
			double dUnitTop = -1;
			double dMainScrollVerticalOffset = MainScrollViewer.VerticalOffset;
			double dMainScrollHeight = MainScrollViewer.ActualHeight;

			dUnitLeft = Math.Max (ptMinimizeOnMainCanvas.X, 0);
			dUnitTop = Math.Max (ptMinimizeOnMainCanvas.Y, 0);

			double dUnitWidth = Kiss.DoubleEqual (0, miniUnit.ActualWidth) ? 190 : miniUnit.ActualWidth;
			double dUnitHeight = Kiss.DoubleEqual (0, miniUnit.ActualHeight) ? 20 : miniUnit.ActualHeight;

			if (dUnitLeft > MainCanvas.ActualWidth - dUnitWidth)
			{
				dUnitLeft = MainCanvas.ActualWidth - dUnitWidth;
			}

			if (dUnitTop > MainCanvas.ActualHeight - dUnitHeight)
			{
				dUnitTop = MainCanvas.ActualHeight - dUnitHeight;
			}

			if (dUnitTop < dMainScrollVerticalOffset)
			{
				dUnitTop = dMainScrollVerticalOffset;
			}
			else if (dUnitTop > dMainScrollHeight + dMainScrollVerticalOffset)
			{
				dUnitTop = dMainScrollVerticalOffset + dMainScrollHeight - dUnitHeight;
			}

			miniUnit.Margin = new Thickness (dUnitLeft, dUnitTop, 0, 0);
			MainCanvas.Children.Add (miniUnit);
			Canvas.SetZIndex (childWindow.MinimizedUnit, int.MaxValue);
		}

		#endregion

		#region |  void DeleteChildMiniUnit(IMDIChildWindow childWindow)  |
		/// <summary>
		/// 최소화창 삭제 
		/// </summary>
		/// <param name="minimizedUnit"></param>
		public void DeleteChildMiniUnit (IMDIChildWindow childWindow)
		{

			MainCanvas.Children.Remove ((FrameworkElement)childWindow.MinimizedUnit);
		}

		#endregion

		#region |  void ChildMiniToFront(IMDIChildWindow childWindow)  |
		/// <summary>
		/// 최소화창 앞으로 이동 
		/// </summary>
		/// <param name="minimizedUnit"></param>
		public void ChildMiniToFront (IMDIChildWindow childWindow)
		{

			Canvas.SetZIndex (childWindow.MinimizedUnit, int.MaxValue);
		}

		#endregion

		#region |  void ChildMiniToBack(IMDIChildWindow childWindow)  |
		/// <summary>
		/// 최소화창 뒤로 이동 
		/// </summary>
		/// <param name="minimizedUnit"></param>
		public void ChildMiniToBack (IMDIChildWindow childWindow)
		{

			Canvas.SetZIndex (childWindow.MinimizedUnit, int.MinValue);
		}

		#endregion
		#endregion//MDI 관련

		#region |  void OnProgramExit()  |

		public void OnProgramExit ()
		{
			Close ();
		}

		#endregion

		#region |  bool IsAllClosed()  |

		public bool IsAllClosed ()
		{
			if (true == IsAllDMTFrameClosded ())
			{
				if (0 == ExecuteMenuManager.Instance.MenuCount)
				{
					return true;
				}
			}
			return false;
		}

		#endregion

		#region |  bool IsAllDMTFrameClosded()  |

		public bool IsAllDMTFrameClosded ()
		{
			List<BaseFrame> lstFrame = new List<BaseFrame> ();

			foreach (object objItem in MainCanvas.GetAllFrame ())
			{
				if (objItem is BaseFrame)
				{
					lstFrame.Add ((BaseFrame)objItem);
				}
			}

			if (0 == lstFrame.Count)
			{
				return true;
			}

			return false;
		}

		#endregion


		#region MainAtomContextMenuOpen ()

		public void MainAtomContextMenuOpen (bool bOpen)
		{
			MainAtomContextMenuPanel.StaysOpen = true;
			MainAtomContextMenuPanel.IsOpen = bOpen;
			//MainAtomContextMenuPanel.StaysOpen = false;
		}

		#endregion

		public void ExecuteReportModel (bool isPreView, string strReportModel, FormDataManager formDataManager, string strFileName)
		{
			var reportDocument = new Softpower.SmartMaker.TopReportLight.ViewModel.ReportLightDoc ();

			strReportModel = PQAppBase.KissGetFullPath (strReportModel);
			reportDocument.IsDirectRun = true;
			reportDocument.OnOpenDocument (strReportModel);
			reportDocument.OutputFileName = strFileName;
			reportDocument.RunMode (formDataManager);

			if (true == isPreView)
			{
				var pdfViewWindow = new Softpower.SmartMaker.TopReportProcess.PDFPreViewWindow (reportDocument.ReleaseFilePath);
				pdfViewWindow.Owner = Application.Current.MainWindow;
				pdfViewWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				pdfViewWindow.Show ();
			}
		}

		private void OnNotifyTabIndexButton (bool IsCellIndex)
		{
			MainToolBarPanel.OfficeToolBar.OnNotifyTabIndexButton (IsCellIndex);
		}

		public void ShowEdutechUserManager ()
		{
			if (SERVICE_TYPE._3Tier != _PQRemoting.ServiceType || false == PQAppBase.ConnectStatus)
			{
				ToastMessge.Show (LC.GS ("SmartOffice_MainWindow_23")); //3계층구조로 서버접속 해야합니다
				return;
			}

			if (1 != PQAppBase.KissGetDocLevel ())
			{
				ToastMessge.Show ("관리자 계정으로 로그인 해야만 사용 가능합니다."); //3계층구조로 서버접속 해야합니다
				return;
			}

			if (1 == PQAppBase.CompanyCode)
			{
				ToastMessge.Show ("현재 스마트서버 버전에서는 지원하지 않은 기능입니다.");
				return;
			}

			EduTechUserInfoWindow userInfo = new EduTechUserInfoWindow ();

			userInfo.Owner = Application.Current.MainWindow;
			userInfo.ShowInTaskbar = false;
			userInfo.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			userInfo.Show ();
		}


		public void ShowEdutechProjectManager ()
		{
			if (SERVICE_TYPE._3Tier != _PQRemoting.ServiceType || false == PQAppBase.ConnectStatus)
			{
				ToastMessge.Show (LC.GS ("SmartOffice_MainWindow_23"));
				return;
			}

			if (true == EduTechLicenseHelper.Instance.IsEduTechStandardVersion () || false == EduTechLicenseHelper.Instance.IsEnableEduTechProjectLicense ())
			{
				ToastMessge.Show ("프로젝트 접근 권한이 부족합니다. 에듀태크 라이선스를 등록해야 합니다");
				return;
			}

			if (2 < PQAppBase.KissGetDocLevel ())
			{
				ToastMessge.Show ("프로젝트 접근 권한이 부족합니다. 편집자 권한 이상부터 접근 가능합니다");
				return;
			}

			if (1 == PQAppBase.CompanyCode)
			{
				ToastMessge.Show ("현재 스마트서버 버전에서는 지원하지 않은 기능입니다.");
				return;
			}

			EduTechProjectWindow userInfo = new EduTechProjectWindow ();

			userInfo.Owner = Application.Current.MainWindow;
			userInfo.ShowInTaskbar = false;
			userInfo.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			userInfo.Show ();
		}

		/// <summary>
		/// 모델창 정렬
		/// </summary>
		/// <param name="nMenu"></param>
		private void WindowManagerLayout (int nMenu)
		{
			switch (nMenu)
			{
				case 0: // 계단식 정렬
					MainCanvas.ModelCascadeLayout ();
					break;
				case 1: // 모든창 최소화
					MainCanvas.ModelMinimizedLayout ();
					break;
				case 2: // 모든창 닫기
					MainCanvas.ModelCloseAllLayout (null);
					break;
				case 3: // 현재창 제외 모두닫기
					MainCanvas.ModelCloseAllLayout (MainCanvas.CurrentDMTFrame);
					break;
			}
		}

		#endregion//Public 메서드

		#region | Interactive Tutorial |

		public void ShowInteractiveTutorialWindow ()
		{
			var window = new InteractiveTutorialWindow ();

			window.Width = this.ActualWidth;
			window.Height = this.ActualHeight;

			window.Left = this.Left;
			window.Top = this.Top;

			window.Owner = this;

			window.ShowDialog ();
		}

		#endregion


		#region |  ##### 주석소스 #####  |
		//private void LeftChangedEvent(object sender, System.EventArgs e)
		//{
		//if (CurrentWindowState != WindowStateType.Maximized && CurrentWindowState != WindowStateType.Minimized)
		//{

		//    if (this.Left > 0)
		//        m_dPrevWindowLeft = this.Left;

		//    if ( this.Top > 0 )
		//        m_dPrevWindowTop = this.Top;
		//}
		//}

		//private void TopChangedEvent(object sender, System.EventArgs e)
		//{
		//if (CurrentWindowState != WindowStateType.Maximized && CurrentWindowState != WindowStateType.Minimized)
		//{
		//    if (this.Left > 0)
		//        m_dPrevWindowLeft = this.Left;

		//    if (this.Top > 0)
		//        m_dPrevWindowTop = this.Top;
		//}
		//}

		//public void WindowStateTypeChanged(WindowStateType ChangedType)
		//{

		//    switch (ChangedType)
		//    {
		//        case WindowStateType.Normal:
		//            {
		//                ResizeManager.Instance.CanResize = true;

		//                WindowState = System.Windows.WindowState.Normal;
		//                this.Width = m_dPrevWindowWidth;
		//                this.Height = m_dPrevWindowHeight;
		//                this.Left = m_dPrevWindowLeft;
		//                this.Top = m_dPrevWindowTop;
		//                break;
		//            }

		//        case WindowStateType.Maximized:
		//            {
		//                ResizeManager.Instance.CanResize = false;

		//                WindowState = System.Windows.WindowState.Normal;
		//                m_dPrevWindowWidth = this.ActualWidth;
		//                m_dPrevWindowHeight = this.ActualHeight;
		//                m_dPrevWindowLeft = this.Left;
		//                m_dPrevWindowTop = this.Top;

		//                double dCenterX = Left + (ActualWidth / 2);

		//                if (0 <= dCenterX && dCenterX <= SystemParameters.PrimaryScreenWidth)
		//                {
		//                    Width = SystemParameters.WorkArea.Width;
		//                    Height = SystemParameters.WorkArea.Height;
		//                    Left = 0;
		//                    Top = 0;
		//                }
		//                else
		//                {
		//                    System.Windows.Forms.Screen[] allScreens = System.Windows.Forms.Screen.AllScreens;

		//                    foreach (System.Windows.Forms.Screen screen in allScreens)
		//                    {
		//                        if(screen.WorkingArea.Left <= dCenterX && dCenterX <= screen.WorkingArea.Left + screen.WorkingArea.Width)
		//                        {
		//                            Left = screen.WorkingArea.Left;
		//                            Top = screen.WorkingArea.Top;
		//                            Width = screen.WorkingArea.Width;
		//                            Height = screen.WorkingArea.Height;
		//                        }
		//                    }
		//                }

		//                //System.Drawing.Rectangle NowArea = GetNowScreenInMousePosition();

		//                //this.Width = SystemParameters.WorkArea.Width;
		//                //this.Height = SystemParameters.WorkArea.Height;
		//                //this.Left = 0;
		//                //this.Top = 0;

		//                //this.Width = NowArea.Width;
		//                //this.Height = NowArea.Height;
		//                //this.Left = NowArea.Left;
		//                //this.Top = NowArea.Top;

		//                break;
		//            }

		//        case WindowStateType.Minimized:
		//            {
		//                WindowState = System.Windows.WindowState.Minimized;
		//                break;
		//            }

		//        default: break;
		//    }

		//    //MainCanvas.InvalidateAll();
		//    MainCanvas.ReLocationMinimizedFrames();
		//}
		#endregion//주석소스

		#region 디버깅 용도

		private void GetSubClass ()
		{
			//debugWindow.Show();
		}

		#endregion 디버깅 용도
	}//end partial class MainWindow : Window, IMDIParentWindow
}//end namespace Softpower.SmartMaker.TopBuild
