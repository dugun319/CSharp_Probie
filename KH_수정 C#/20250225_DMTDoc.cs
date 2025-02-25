using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.Common.Log;
using Softpower.SmartMaker.Compiler;
using Softpower.SmartMaker.DBCoreX.Common;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.DesignHelper.Common;
using Softpower.SmartMaker.DesignHelper.Model;
using Softpower.SmartMaker.FileDBIO80;
using Softpower.SmartMaker.Script;
using Softpower.SmartMaker.TopAIAdaptorManager;
using Softpower.SmartMaker.TopAnimation;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.CommonLib.Interface;
using Softpower.SmartMaker.TopApp.CommonLib.StringGenerator;
using Softpower.SmartMaker.TopApp.EduTech.QuizMaker;
using Softpower.SmartMaker.TopApp.License;
using Softpower.SmartMaker.TopApp.MDI;
using Softpower.SmartMaker.TopApp.Metadata;
using Softpower.SmartMaker.TopApp.Metadata.QuizMaker;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopAtom.Commands;
using Softpower.SmartMaker.TopAtom.Commands.FromToolBarCommand;
using Softpower.SmartMaker.TopAtom.Comparer;
using Softpower.SmartMaker.TopAtom.Components;
using Softpower.SmartMaker.TopAtom.Components.ActionManager;
using Softpower.SmartMaker.TopAtom.Components.GridTableAtom.Behaviors;
using Softpower.SmartMaker.TopAtom.Components.GridTableAtom.Components;
using Softpower.SmartMaker.TopAtom.Components.TabViewAtom;
using Softpower.SmartMaker.TopAtom.Ebook;
using Softpower.SmartMaker.TopAtom.Ebook.Components;
using Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView;
using Softpower.SmartMaker.TopAtom.Verbal;
using Softpower.SmartMaker.TopControl.Components.Container;
using Softpower.SmartMaker.TopControl.Components.Dialog;
using Softpower.SmartMaker.TopControl.Components.EBook;
using Softpower.SmartMaker.TopControlEdit;
using Softpower.SmartMaker.TopControlEdit.EduTech;
using Softpower.SmartMaker.TopControlEdit.EventManager;
using Softpower.SmartMaker.TopDBManager80;
using Softpower.SmartMaker.TopDefine;
using Softpower.SmartMaker.TopLight;
using Softpower.SmartMaker.TopLight.Commands;
using Softpower.SmartMaker.TopLight.Dynamic;
using Softpower.SmartMaker.TopLight.Models;
using Softpower.SmartMaker.TopLight.ProcessEventManager;
using Softpower.SmartMaker.TopLight.Spoit;
using Softpower.SmartMaker.TopLight.ViewModels;
using Softpower.SmartMaker.TopProcess.AtomConvertManager;
using Softpower.SmartMaker.TopProcess.Commands;
using Softpower.SmartMaker.TopProcess.Component.CreateTable;
using Softpower.SmartMaker.TopProcess.Component.DynamicWeb;
using Softpower.SmartMaker.TopProcess.Component.QuizMaker;
using Softpower.SmartMaker.TopProcess.Component.StructData;
using Softpower.SmartMaker.TopProcess.Component.Views;
using Softpower.SmartMaker.TopProcess.SqlGenerator;
using Softpower.SmartMaker.TopProcessEdit.FlowManager.View;
using Softpower.SmartMaker.TopProcessEdit.FlowManager.ViewModel;
using Softpower.SmartMaker.TopProcessEdit.Helper;
using Softpower.SmartMaker.TopProcessEdit.NewFlowManager;
using Softpower.SmartMaker.TopSmartAtom;
using Softpower.SmartMaker.TopSmartAtom.Components.RadialMenu.Data;
using Softpower.SmartMaker.TopStructDataManager80;
using Softpower.SmartMaker.TopWebAtom;
using Softpower.SmartMaker.TopWebAtom.Commands;

using TopActionManager;
using TopActionManager.Character;

namespace Softpower.SmartMaker.TopProcess.Component.ViewModels
{
	public class DMTDoc : LightJDoc
	{
		#region |  ##### Private 전역변수 #####  |

		private Hashtable m_htProperSQLIndex = new Hashtable ();

		/// <summary>
		/// 2005.10.21, 이우성 테이블값 편집시 이전수정된 테이블명만 수정
		/// </summary>
		private bool m_bChangedTitle;

		private string m_strOldDefaultTable;

		private Stream m_currentFileStream;
		private bool m_canSaveDocument = true;

		private Softpower.SmartMaker.TopDBManager80.ERDGenManager.ERDGenManager80 m_pErdGenManager80 = null;

		private Dictionary<Atom, bool> m_LockBackupDic;

		/// <summary>
		/// Kiho : 2016-11-10 : Mouse 가 눌려진 상태에서 LeftCtrl 키가 눌려지면 원본 아톰들을 카피하고 전역변수에 넣어둔다
		/// 나중에 LeftCtrl 키 업일때 삭제하는 용도로 사용한다
		/// </summary>
		private List<AtomBase> m_lstOriginalCopyedAtoms;

		[NonSerialized]

		/// <summary>
		/// 80 추후작업 : 실행 질의문 관련
		/// </summary>
		private UserQueryDialog pUserQueryDialog = null;

		/// <summary>
		/// 2008.01.16 황성민
		/// 실행질의문 중[수정][저장] 모드를 확인하기 위해서 사용합니다.
		/// </summary>
		private bool m_pThisFormInsertMode = false;

		#endregion

		#region |  ##### Protected 전역변수 #####  |

		protected bool m_bTabChange;
		protected bool m_bFormChange = false;
		protected bool m_bModelSearch;

		protected int m_stnMaxIndex;

		/// <summary>
		/// Erd관련
		/// </summary>
		protected string m_strDefaultTable;

		protected DBManagerOwnerFrame m_DBManagerFrame;
		protected StructDataManagerOwnerFrame m_pStructDataMgrFrame;

		protected Atom m_pRButtonAtom;

		protected TopEdit80.CScrFrame m_pScriptWindow;

		/// <summary>
		/// 진행관리자
		/// </summary>
		protected FlowMap m_FlowMapWindow;

		/// <summary>
		/// 신규 진행관리자
		/// </summary>
		protected FlowManagerMainWndow m_FlowManagerWindow = null;

		/// <summary>
		/// 아톰편집도우미
		/// </summary>
		protected AtomEditMap m_AtomEditMapWindow;

		/// <summary>
		/// DB처리객체 종료 이벤트
		/// </summary>
		protected EventHandler m_DBMgrCloseEvent = null;

		protected Atom m_pRTempChangeAttribAtom;

		/// <summary>
		/// Ai 어댑터 관련
		/// </summary>
		protected AIAdaptorWindow m_AIAdaptorWindow = null;

		#endregion

		#region |  ##### Private 속성 #####  |

		private Dictionary<Atom, bool> LockBackupDic
		{
			get
			{
				if (null == m_LockBackupDic)
				{
					m_LockBackupDic = new Dictionary<Atom, bool> ();
				}
				return m_LockBackupDic;
			}
		}

		#endregion

		#region |  ##### Public Override 속성 #####  |

		/// <summary>
		/// 80 저장관련
		/// </summary>
		public override bool CanSaveDocument
		{
			get
			{
				return m_canSaveDocument;
			}
			set
			{
				m_canSaveDocument = value;
			}
		}

		public override AtomBase MakeAtomInTabView (TabViewAtomBase tabView, AtomType atomType, double x, double y)
		{
			DMTView view = GetParentView () as DMTView;

			var makeAtom = view.MakeAtomAsCommand (atomType, x, y, new Size (0, 0), null);

			if (null != tabView)
			{
				view.Children.Remove (makeAtom);

				AdjustSourceAtomRegionInTabView (makeAtom, tabView);
				AttachAtomAtTabView (makeAtom, tabView, false, -1);
				AutoRefreshEditWindow ();
			}

			return makeAtom;
		}

		/// <summary>
		/// 2024-12-01 kys 기획 변경으로 인해 [지시문 / 문항 / 답항]에 있는 정보는 별도 DB필드에서 관리 및 적용하도록 변경되어 해당 기능을 처리하기 위해 이 함수를 구현함
		/// 퀴즈메이커, 모델삽입 모두 해당 함수를 통해 각 아톰에 값을 설정해야만 정상적으로 표시되기 때문에 사용시 주의 필요
		/// </summary>
		public override void SetParameterData ()
		{
			var quizMetaData = this.PageMetadata.QuizMetaData;
			var quizContentInfo = quizMetaData.QuizContentInfo;
			var atomCores = this.GetAllAtomCores ();

			if (null == quizContentInfo || null == quizMetaData)
				return;

			var titleValueList = JsonConvert.DeserializeObject<List<QuizDBAtomValueNode>> (quizContentInfo.TitleText);
			var questionContentList = JsonConvert.DeserializeObject<List<QuizDBAtomValueNode>> (quizMetaData.QuizContentInfo.QuestionContent);
			var answerContentList = JsonConvert.DeserializeObject<List<QuizDBAtomValueNode>> (quizMetaData.QuizContentInfo.AnswerContent);

			if (null != titleValueList)
			{
				foreach (var node in titleValueList)
				{
					var atomCore = atomCores.Find (i => i.AtomProperVar == node.Name);

					if (null != atomCore)
					{
						SetQuizDBAtomValueNode (atomCore, node);
					}
				}
			}

			if (null != questionContentList)
			{
				foreach (var node in questionContentList)
				{
					var atomCore = atomCores.Find (i => i.AtomProperVar == node.Name);

					if (null != atomCore)
					{
						SetQuizDBAtomValueNode (atomCore, node);
					}
				}

			}

			if (null != answerContentList)
			{
				foreach (var node in answerContentList)
				{
					var atomCore = atomCores.Find (i => i.AtomProperVar == node.Name);

					if (null != atomCore)
					{
						SetQuizDBAtomValueNode (atomCore, node);
					}
				}
			}
		}

		protected void SetQuizDBAtomValueNode (Atom atomCore, QuizDBAtomValueNode node)
		{
			if (node.ValueType == TopApp.EduTech.QuizMaker.ValueType.Text && atomCore is SquareAtom)
			{
				var squareAttrib = atomCore.Attrib as SquareAttrib;
				squareAttrib.Title = node.Value?.ToString ();

				atomCore.AtomCore_OnNotifyChangedValueByInnerLogic (squareAttrib.Title);
			}
			else if (node.ValueType == TopApp.EduTech.QuizMaker.ValueType.Text && atomCore is EBookTextAtom)
			{
				var textofAtom = atomCore.AtomBase as EBookTextofAtom;
				var text = node.Value?.ToString ();

				textofAtom.SetTextSource (text);

				//2025-02-03 kys 지시문에 들어가는 글꼴은 항상 12포인트로 설정되도록 처리함..
				//2025-02-12 kys 지시문은 항상 Bold 처리하도록 한다.
				if (-1 < atomCore.AtomProperVar.IndexOf (QuizLayoutMetaDataDefine.TitleContentAtomName))
				{
					int titleFontSize = 12;
					textofAtom.SetAtomFontSize (titleFontSize);
					textofAtom.SetTexBoxFontSize (titleFontSize);
					textofAtom.SetAutoLineHeight (titleFontSize);

					textofAtom.SetAtomFontWeight (FontWeights.Bold);
				}
				//2025-02-13 kys 문/답항이 텍스트편집기로 이루어진경우 항상 글자 크기가 12포인트로 고정하도록 기능 처리함 (현재 기획상 글자 크기 고정)
				else if (-1 < atomCore.AtomProperVar.IndexOf (QuizLayoutMetaDataDefine.QuestionNumberAtomName) ||
					-1 < atomCore.AtomProperVar.IndexOf (QuizLayoutMetaDataDefine.AnswerNumberAtomName))
				{
					var atomName = atomCore.AtomProperVar.Replace (QuizLayoutMetaDataDefine.QuestionNumberAtomName, "");
					atomName = atomCore.AtomProperVar.Replace (QuizLayoutMetaDataDefine.AnswerNumberAtomName, "");
					
					if (StrLib.IsNumber (atomName))
					{
						int questionFontSize = 12;
						textofAtom.SetAtomFontSize (questionFontSize);
						textofAtom.SetTexBoxFontSize (questionFontSize);
						textofAtom.SetAutoLineHeight (questionFontSize);
					}
				}
			}
			else if (node.ValueType == TopApp.EduTech.QuizMaker.ValueType.Array && atomCore is EBookTextAtom)
			{
				var textAttrib = atomCore.Attrib as EBookTextAttrib;
				var list = JsonConvert.DeserializeObject<List<QuizDBAtomValueNode>> (node.Value?.ToString ());

				if (null != list)
				{
					foreach (var data in list)
					{
						if (data.ValueType == TopApp.EduTech.QuizMaker.ValueType.Xaml)
						{
							var byteArray = StrLib.FromBase64String (data.Value?.ToString ());

							if (null != byteArray)
							{
								using (var ms = new MemoryStream (byteArray))
								{
									var tr = new TextRange (textAttrib.Document.ContentStart, textAttrib.Document.ContentEnd);
									tr.Load (ms, DataFormats.Xaml);
								}
							}
						}
						else if (data.ValueType == TopApp.EduTech.QuizMaker.ValueType.RTF)
						{
							var byteArray = StrLib.FromBase64String (data.Value?.ToString ());

							if (null != byteArray)
							{
								using (var ms = new MemoryStream (byteArray))
								{
									var tr = new TextRange (textAttrib.Document.ContentStart, textAttrib.Document.ContentEnd);
									tr.Load (ms, DataFormats.Rtf);
								}
							}
						}
					}
				}
			}
			else if (node.ValueType == TopApp.EduTech.QuizMaker.ValueType.Path && atomCore is DecorImageAtom)
			{
				var decorImageAttrib = atomCore.Attrib as DecorImageAttrib;
				decorImageAttrib.ImagePath = node.Value?.ToString ();

				CObjectImage pObjectImage = new CObjectImage ();
				pObjectImage.AddRef ();

				int nKey = decorImageAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._IMAGE);

				pObjectImage.ImagePath = decorImageAttrib.ImagePath;

				nKey = decorImageAttrib.GetKeyFromGDIObj (pObjectImage, nKey);
				decorImageAttrib.SetGDIKey ((int)Define.OBJECTKEY_TYPE._IMAGE, nKey);
				atomCore.CompletePropertyChanged ();
			}
			else if (atomCore is InputAtom || atomCore is EBookGridPaperAtom)
			{
				//입력란, 개별 입력란의 경우 편집단계에서 데이터가 없기 때문에 생략하도록 한다.
			}
			else
			{
				PQAppBase.TraceDebugLog ("atom : " + atomCore.GetType () + "value type : " + node.ValueType.ToString ());
			}
		}

		#endregion

		#region |  ##### Public 속성 #####  |

		public List<AtomBase> LstOriginalCopyedAtoms
		{
			get
			{
				return this.m_lstOriginalCopyedAtoms;
			}
		}

		public TopEdit80.CScrFrame ScriptWindow
		{
			get
			{
				return m_pScriptWindow;
			}
			set
			{
				m_pScriptWindow = value;
			}
		}

		public FlowMap FlowMapWindow
		{
			get
			{
				return m_FlowMapWindow;
			}
			set
			{
				m_FlowMapWindow = value;
			}
		}

		public FlowManagerMainWndow FlowManagerWindow
		{
			get { return m_FlowManagerWindow; }
			set { m_FlowManagerWindow = value; }
		}

		public AtomEditMap AtomEditMapWindow
		{
			get
			{
				return m_AtomEditMapWindow;
			}
			set
			{
				m_AtomEditMapWindow = value;
			}
		}

		public DBManagerOwnerFrame DBManagerFrame
		{
			get
			{
				return m_DBManagerFrame;
			}
			set
			{
				m_DBManagerFrame = value;
			}
		}

		public StructDataManagerOwnerFrame StructDataManagerFrame
		{
			get
			{
				return m_pStructDataMgrFrame;
			}
			set
			{
				m_pStructDataMgrFrame = value;
			}
		}

		public Stream CurrentFileStream
		{
			get { return m_currentFileStream; }
			set { m_currentFileStream = value; }
		}

		public bool bNotClearValue
		{
			get
			{
				return m_pFrameAttrib.NotClear;
			}
		}

		public AIAdaptorWindow AIAdaptorMainWindow
		{
			get
			{
				if (null == m_AIAdaptorWindow)
				{
					m_AIAdaptorWindow = new AIAdaptorWindow (this, MainAIAdaptorManager);
					m_AIAdaptorWindow.Title = $"{GetSubWindowTitle()} - AI+ 어댑터";
				}

				return m_AIAdaptorWindow;
			}

			set { m_AIAdaptorWindow = value; }
		}

		public DynamicGridTable WebDynamicGrid
		{
			get
			{
				DMTView view = GetParentView () as DMTView;
				DMTFrame frame = null != view ? view.GetFrame () as DMTFrame : null;
				CFrameAttrib frameAttrib = GetFrameAttrib ();
				if (null != frame && null != frameAttrib)
				{
					if (frame.CurrentBaseScreenView is PhoneScreenView phoneScreenView)
					{
						return phoneScreenView.DynamicGrid as DynamicGridTable;
					}
				}

				return null;
			}
		}

		#endregion

		#region |  ##### 델리게이트 및 이벤트 선언 #####  |

		public event CommonDelegateEvents.OnNotifyMethodExecutedEventHandler OnNotifyViewPropertyChangedEvent = null;

		//2014-10-31-M01 에디터 모드에서 이동 처리
		public event CommonDelegateEvents.OnNotifyBoolValueEventHandler OnNotifyMoveEvent = null;

		//2014-11-03-M02 텍스트편집기 에디터 모드 변경
		public event CommonDelegateEvents.OnNotifyMethodExecutedEventHandler OnNotifyMoveCompletedEvent = null;
		public event CommonDelegateEvents.OnNotifyThreeObjectEventHandler OnNotifyReSizeEvent = null;


		//아톰 속성과 상단 툴바의 속성을 동기화하는 델리게이트
		//[ FontFamily updateFont ]
		//[ int nUpdateFontSize ]
		//[ FontWeight updateFontWeight ]
		//[ FontStyle updateFontStyle ]
		//[ TextDecorationCollection updateDecoration ]
		//[ HorizontalAlignment updateHorizontalAlignment ]
		//[ VerticalAlignment updateVerticalAlignment ]
		//[ Brush updateFontBrush ]
		//[ Brush updateLineBrush ]
		//[ Brush updateBackgroundBrush ]
		//[ bool bIsNoLine ]
		public event CommonDelegateEvents.OnNotifyObjectEventHandler OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent = null;

		#endregion

		#region |  ##### 생성자 #####  |

		public DMTDoc ()
		{
		}

		#endregion

		#region |  ##### Private 메서드 #####  |

		private void HandleShowAtomContextMenuEvent (ArrayList alParameterList)
		{
			try
			{
				var dmtView = GetParentView () as DMTView;
				var dmtFrame = dmtView.GetFrame () as DMTFrame;
				dmtFrame.OnShowAtomContextMenuEvent (alParameterList);
			}
			catch (Exception ex)
			{
				Trace.TraceError (ex.ToString ());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strAtomProperVar">검사할 아톰 이름</param>
		/// <param name="ExceptAtom">검사 대상에서 제외될 아톰</param>
		/// <returns></returns>
		private bool IsExistSameAtomProperVarInDMTView (string strAtomProperVar, AtomBase ExceptAtom)
		{
			List<Atom> AtomList = GetAllAtomCores ();
			AtomList.Remove (ExceptAtom.AtomCore);

			int Count = AtomList.Where (item => item.GetProperVar () == strAtomProperVar).Count ();

			return 0 < Count;
		}

		/// <summary>
		/// View나 TabView, Scroll에 선택된 아톰들이 있으면 True를 반환
		/// </summary>
		/// <returns></returns>
		private bool IsExistCurrentSelectedAtoms ()
		{
			List<AtomBase> lstCurrentSelectedAtomsInLightDMTView = GetCurrentSelectedAtomsInLightDMTView ();
			List<AtomBase> lstCurrentSelectedAtomsInTabView = GetCurrentSelectedAtomsInTabView ();
			List<AtomBase> lstCurrentSelectedAtomsInScroll = GetCurrentSelectedAtomsInScroll ();

			int nCurrentSelectedAtomsInLightDMTViewCount = lstCurrentSelectedAtomsInLightDMTView.Count;
			int nCurrentSelectedAtomsInTabViewCount = lstCurrentSelectedAtomsInTabView.Count;
			int nCurrentSelectedAtomsInScroll = lstCurrentSelectedAtomsInScroll.Count;

			lstCurrentSelectedAtomsInLightDMTView.Clear ();
			lstCurrentSelectedAtomsInTabView.Clear ();
			lstCurrentSelectedAtomsInScroll.Clear ();
			lstCurrentSelectedAtomsInLightDMTView = null;
			lstCurrentSelectedAtomsInTabView = null;
			lstCurrentSelectedAtomsInScroll = null;

			if (0 < nCurrentSelectedAtomsInLightDMTViewCount || 0 < nCurrentSelectedAtomsInTabViewCount || 0 < nCurrentSelectedAtomsInScroll)
			{
				return true;
			}

			//대체디자인 고려
			if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode)
			{
				List<AtomBase> selectAtomList = WebDynamicGrid.RootDesignGrid.GetSelectAtomList ();

				if (null != selectAtomList && 0 < selectAtomList.Count)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 스크롤을 삭제할 경우 스크롤에 바인딩된 아톰들의 부모가 변경이 된다.
		/// 이 경우 부모가 LightDMTView가 될 경우 아톰들이 View영역 내에 있는지 보고 부모를 붙일지 말지 결정한다.
		/// 2021-01-11 kys 스크롤에 묶인 아톰이 View영역 외부에 있더라도 스크롤 삭제 및 스크롤 묶기 해제 동작이 정상적으로 처리되어야 하기 때문에 비교 논리 주석처리함
		/// </summary>
		/// <param name="SourceScrollAtom"></param>
		/// <returns></returns>
		private bool TryAttachAtomAtView (ScrollAtomBase SourceScrollAtom)
		{
			var scrollAtomCore = SourceScrollAtom.AtomCore as ScrollAtom;
			List<AtomBase> lstBindedAtoms = scrollAtomCore.GetBindedAtoms ();

			FrameworkElement OwnerElement = SourceScrollAtom.GetOwnerView ();

			Size RegionSize = Size.Empty;
			Thickness StartPoint = new Thickness ();
			Thickness SourceScrollAtomMargin = SourceScrollAtom.Margin;
			bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstBindedAtoms, ref StartPoint, ref RegionSize);

			if (true == bIsCalculated)
			{
				Size ViewSize = GetViewSize ();
				double dViewWidth = ViewSize.Width;
				double dViewHeight = ViewSize.Height;

				double dResultRight = (RegionSize.Width + SourceScrollAtomMargin.Left);
				double dResultBottom = (RegionSize.Height + SourceScrollAtomMargin.Top);

				//if (dViewWidth <= dResultRight || dViewHeight <= dResultBottom)
				//{
				//	_Message80.Show(LC.GS("TopProcess_DMTDoc_32"));
				//	return false;
				//}

				SourceScrollAtom.ClearChildren ();

				//2020-04-06 kys 확장스크롤의 경우 스크롤의 자식으로 팝업에 묶인 아톰들이 포함되기 때문에 스크롤 삭제시 팝업에 묶인 아톰들도 상위View로 이동해야함
				if (SourceScrollAtom is ExtensionScrollofAtom)
				{
					List<AtomBase> pBindPopupAtom = new List<AtomBase> ();
					foreach (AtomBase CurrentAtom in lstBindedAtoms)
					{
						if (CurrentAtom is PopupofAtom)
						{
							PopupofAtom pPopupAtom = CurrentAtom as PopupofAtom;
							pBindPopupAtom.AddRange (pPopupAtom.GetBindedAtoms ());
						}
					}

					lstBindedAtoms.AddRange (pBindPopupAtom);
				}

				scrollAtomCore.UndoBindAtomList.Clear ();

				foreach (AtomBase CurrentAtom in lstBindedAtoms)
				{
					Thickness CurrentAtomMargin = CurrentAtom.Margin;

					double dResultLeft = (CurrentAtomMargin.Left + SourceScrollAtomMargin.Left);
					double dResultTop = (CurrentAtomMargin.Top + SourceScrollAtomMargin.Top);

					CurrentAtom.Margin = new Thickness (dResultLeft, dResultTop, 0, 0);

					CurrentAtom.AtomCore.IsBindedScroll = false;
					CurrentAtom.AtomCore.BindBlockAtom = null;

					scrollAtomCore.UndoBindAtomList.Add (CurrentAtom.AtomCore.AtomProperVar);

					this.AttachAtomAtView (CurrentAtom, AttachAtomEventDefine.AttachAtomEventType.DeleteScroll, false);
					CurrentAtom.SetResizeAdornerVisibility (Visibility.Visible, false);
				}
			}

			return true;
		}

		private void ChangeAllAtomDBInfoVisible (bool bIsDebugDBInfoVisible)
		{
			List<Atom> allAtomCoreList = GetAllAtomCores ();
			List<AtomBase> allAtomList = allAtomCoreList.Select (item => item.GetOfAtom ()).ToList ();

			foreach (AtomBase atom in allAtomList)
			{
				atom.IsDebugDBInfoVisible = bIsDebugDBInfoVisible;
			}
		}

		private void ChangeAllAtomZIndexTextVisible (bool bIsZIndexTextVisible)
		{
			List<Atom> allAtomCoreList = GetAllAtomCores ();
			List<AtomBase> allAtomList = allAtomCoreList.Select (item => item.GetOfAtom ()).ToList ();

			foreach (AtomBase atom in allAtomList)
			{
				atom.IsZIndexTextVisible = bIsZIndexTextVisible;
			}
		}

		private void ChangeAllAtomRelativeTabIndexTextVisible (bool bIsRelativeTabIndexTextVisible)
		{
			List<Atom> allAtomCoreList = GetAllAtomCores ();
			List<AtomBase> allAtomList = allAtomCoreList.Select (item => item.GetOfAtom ()).ToList ();

			foreach (AtomBase atom in allAtomList)
			{
				atom.IsRelativeTabIndexTextVisible = bIsRelativeTabIndexTextVisible;
			}
		}

		private void ChangeAllAtomAbsoluteTabIndexTextVisible (bool bIsAbsoluteTabIndexTextVisible)
		{
			List<Atom> allAtomCoreList = GetAllAtomCores ();
			List<AtomBase> allAtomList = allAtomCoreList.Select (item => item.GetOfAtom ()).ToList ();

			foreach (AtomBase atom in allAtomList)
			{
				atom.IsAbsoluteTabIndexTextVisible = bIsAbsoluteTabIndexTextVisible;
			}
		}

		private void ChangeAllAtomNameTextVisible (bool bIsAtomNameTextVisible)
		{
			List<Atom> allAtomCoreList = GetAllAtomCores ();
			List<AtomBase> allAtomList = allAtomCoreList.Select (item => item.GetOfAtom ()).ToList ();

			foreach (AtomBase atomBase in allAtomList)
			{
				atomBase.IsAtomNameTextVisible = bIsAtomNameTextVisible;
			}

			if (null != WebDynamicGrid)
			{
				DynamicGridTable gridTable = WebDynamicGrid as DynamicGridTable;

				if (null != gridTable)
				{
					List<Atom> atomCoreList = gridTable.RootDesignGrid.GetAllAtomCoreList ();

					foreach (Atom atomCore in atomCoreList)
					{
						AtomBase atomBase = atomCore.GetOfAtom ();
						atomBase.IsAtomNameTextVisible = bIsAtomNameTextVisible;
						atomBase.InvalidateVisual ();
					}
				}
			}
		}

		private void ChangeAllAtomFieldTextVisible (bool isAtomFieldTextVisible)
		{
			List<Atom> allAtomCoreList = GetAllAtomCores ();
			List<AtomBase> allAtomList = allAtomCoreList.Select (item => item.GetOfAtom ()).ToList ();

			foreach (AtomBase atomBase in allAtomList)
			{
				atomBase.IsAtomFieldTextVisible = isAtomFieldTextVisible;
			}

			if (null != WebDynamicGrid)
			{
				DynamicGridTable gridTable = WebDynamicGrid as DynamicGridTable;

				if (null != gridTable)
				{
					List<Atom> dynamicAtomCoreList = gridTable.RootDesignGrid.GetAllAtomCoreList ();
					List<AtomBase> dynamicAtomList = dynamicAtomCoreList.Select (item => item.GetOfAtom ()).ToList ();

					foreach (AtomBase atomBase in dynamicAtomList)
					{
						atomBase.IsAtomFieldTextVisible = isAtomFieldTextVisible;
					}
				}
			}
		}

		/// <summary>
		/// 애니메이션그룹 아톰 반환
		/// </summary>
		/// <returns></returns>
		public List<AtomBase> GetAnimationGroupAtoms ()
		{
			List<AtomBase> lstAnimationGroupAtoms = new List<AtomBase> ();

			foreach (Atom atomCore in GetAllAtomCores ())
			{
				if (typeof (EBookAnimationGroupAtom) == atomCore.GetType ())
				{
					lstAnimationGroupAtoms.Add (atomCore.GetOfAtom ());
				}
			}

			return lstAnimationGroupAtoms;
		}

		/// <summary>
		/// 동일한 아톰 묶음의 애니메이션 그룹이 있는지 체크
		/// </summary>
		/// <returns></returns>
		private bool PreCheckAnimationGroupMake ()
		{
			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInLightDMTView ();

			int nCurrentSelectedAtomsInLightDMTViewCount = lstCurrentSelectedAtoms.Count;
			int nCurrentSelectedAtomsInTabViewCount = 0;

			if (0 == nCurrentSelectedAtomsInLightDMTViewCount)
			{
				lstCurrentSelectedAtoms.Clear ();
				lstCurrentSelectedAtoms = null;
				lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInTabView ();
				nCurrentSelectedAtomsInTabViewCount = lstCurrentSelectedAtoms.Count;
			}

			if (1 >= lstCurrentSelectedAtoms.Count)
			{
				_Message80.Show (LC.GS ("TopProcess_DMTDoc_1808_1"));
				return false;
			}


			List<AtomBase> lstAnimationGroupAtoms = GetAnimationGroupAtoms ();

			foreach (AtomBase atomBase in lstAnimationGroupAtoms)
			{
				EBookAnimationGroupAtom agAtomCore = atomBase.AtomCore as EBookAnimationGroupAtom;

				if (true == agAtomCore.Compare (lstCurrentSelectedAtoms))
				{
					_Message80.Show (LC.GS ("TopProcess_DMTDoc_1808_2"));
					atomBase.SetResizeAdornerVisibility (Visibility.Visible, false);
					atomBase.NotifyCurrentLocationAndSize ();
					return false;
				}
			}

			return true;
		}

		private bool ReadyAnimationGroupAtom (AtomBase animationGroupAtom)
		{
			//방향키 복사기능 안되도록 함 
			if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
			{
				if (true == Keyboard.IsKeyDown (Key.Left)
					|| true == Keyboard.IsKeyDown (Key.Right)
					|| true == Keyboard.IsKeyDown (Key.Up)
					|| true == Keyboard.IsKeyDown (Key.Down))
				{
					return false;
				}
			}

			if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode)
			{
				//대체디자인 내부 그룹묶기 아톰 생성
				return ReadyAnimationGroupAtomReplaceDesign (animationGroupAtom);
			}
			else
			{
				return ReadyAnimationGroupAtomView (animationGroupAtom);
			}
		}

		private bool ReadyAnimationGroupAtomView (AtomBase animationGroupAtom)
		{
			Thickness AgAtomStartPoint = new Thickness ();
			Size AgAtomSize = Size.Empty;
			bool bIsSelectedAtomsInTabView = false;

			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInLightDMTView ();

			int nCurrentSelectedAtomsInLightDMTViewCount = lstCurrentSelectedAtoms.Count;
			int nCurrentSelectedAtomsInTabViewCount = 0;

			if (0 == nCurrentSelectedAtomsInLightDMTViewCount)
			{
				lstCurrentSelectedAtoms.Clear ();
				lstCurrentSelectedAtoms = null;
				lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInTabView ();
				nCurrentSelectedAtomsInTabViewCount = lstCurrentSelectedAtoms.Count;
				bIsSelectedAtomsInTabView = true;
			}

			bool bIsPossible = IsPossibleBindAnimationGroup (lstCurrentSelectedAtoms);

			if (true == bIsPossible)
			{
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstCurrentSelectedAtoms, ref AgAtomStartPoint, ref AgAtomSize);

				if (true == bIsCalculated)
				{
					double dMargin = 10;

					animationGroupAtom.Margin = new Thickness (AgAtomStartPoint.Left - dMargin, AgAtomStartPoint.Top - dMargin, 0, 0);
					animationGroupAtom.Width = AgAtomSize.Width + dMargin * 2;
					animationGroupAtom.Height = AgAtomSize.Height + dMargin * 2;

					if (animationGroupAtom is EBookAnimationGroupofAtom)
					{
						((EBookAnimationGroupofAtom)animationGroupAtom).SetGroupAtoms (lstCurrentSelectedAtoms);
					}
					else if (animationGroupAtom is WebQuickLinkofAtom) // 190426_AHN : 퀵링크 아톰
					{
						((WebQuickLinkofAtom)animationGroupAtom).SetGroupAtoms (lstCurrentSelectedAtoms);
					}

					if (true == bIsSelectedAtomsInTabView)
					{
						if (0 < nCurrentSelectedAtomsInTabViewCount)
						{
							AtomBase FirstAtom = lstCurrentSelectedAtoms[0] as AtomBase;
							FrameworkElement TabViewElement = FirstAtom.GetTabViewAtom ();

							if (null != TabViewElement)
							{
								TabViewAtomBase TabViewAtom = TabViewElement as TabViewAtomBase;
								AttachAtomAtTabView (animationGroupAtom, TabViewAtom, false, -1);
							}
						}
					}
					else
					{
						AttachAtomAtView (animationGroupAtom, AttachAtomEventDefine.AttachAtomEventType.Default, false);
					}
				}
			}

			return bIsPossible;
		}

		private bool ReadyAnimationGroupAtomReplaceDesign (AtomBase animationGroupAtom)
		{
			if (null == WebDynamicGrid)
				return false;

			WebReplaceDesignofAtom designAtom = WebDynamicGrid.RootDesignGrid.DesignAtom;

			if (null == designAtom)
				return false;

			List<AtomBase> selectAtomList = designAtom.GetSelectAtomList ();

			if (0 == selectAtomList.Count)
				return false;

			bool isCanBinded = IsPossibleBindAnimationGroup (selectAtomList);

			if (false == isCanBinded)
				return false;

			double left = selectAtomList.Min (item => item.Margin.Left);
			double top = selectAtomList.Min (item => item.Margin.Top);
			double right = selectAtomList.Max (item => item.Margin.Left + item.Width);
			double bottom = selectAtomList.Max (item => item.Margin.Top + item.Height);

			double width = right - left;
			double height = bottom - top;

			double margin = 10;

			animationGroupAtom.Margin = new Thickness (left - margin, top - margin, 0, 0);
			animationGroupAtom.Width = width + (margin * 2);
			animationGroupAtom.Height = height + (margin * 2);

			if (animationGroupAtom is EBookAnimationGroupofAtom)
			{
				((EBookAnimationGroupofAtom)animationGroupAtom).SetGroupAtoms (selectAtomList);
			}
			else if (animationGroupAtom is WebQuickLinkofAtom) // 190426_AHN : 퀵링크 아톰
			{
				((WebQuickLinkofAtom)animationGroupAtom).SetGroupAtoms (selectAtomList);
			}

			AttachAtomAtView (animationGroupAtom, AttachAtomEventDefine.AttachAtomEventType.Default, false);
			//designAtom.AddAtom (animationGroupAtom);

			return true;
		}

		private bool ReadyWebQuickLinkAtom (AtomBase pWebQuickLinkAtom)
		{
			Thickness AgAtomStartPoint = new Thickness ();
			Size AgAtomSize = Size.Empty;

			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInLightDMTView ();
			List<AtomBase> SelectedAtomListInTabView = GetCurrentSelectedAtomsInTabView ();
			int nCurrentSelectedAtomsInLightDMTViewCount = lstCurrentSelectedAtoms.Count;

			if (SelectedAtomListInTabView.Any ())
			{
				string tabViewPropVar = (SelectedAtomListInTabView.First ().GetTabViewAtom () as TabViewAtomBase).AtomCore.Attrib.DefaultAtomProperVar;
				_Message80.Show (string.Format (LC.GS ($"[{LC.GS (WebAtomName.QUICKLINK)}]\n [{tabViewPropVar}]에 생성할 수 없는 아톰입니다.")));
				return false;
			}

			//방향키 복사기능 안되도록 함 
			if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
			{
				if (true == Keyboard.IsKeyDown (Key.Left)
					|| true == Keyboard.IsKeyDown (Key.Right)
					|| true == Keyboard.IsKeyDown (Key.Up)
					|| true == Keyboard.IsKeyDown (Key.Down))
				{
					return false;
				}
			}

			bool bIsPossible = IsPossibleBindWebQuickLink (lstCurrentSelectedAtoms);

			if (true == bIsPossible)
			{
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstCurrentSelectedAtoms, ref AgAtomStartPoint, ref AgAtomSize);

				if (true == bIsCalculated)
				{
					double dMargin = 10;

					pWebQuickLinkAtom.Margin = new Thickness (AgAtomStartPoint.Left - dMargin, AgAtomStartPoint.Top - dMargin, 0, 0);
					pWebQuickLinkAtom.Width = AgAtomSize.Width + dMargin * 2;
					pWebQuickLinkAtom.Height = AgAtomSize.Height + dMargin * 2;

					((WebQuickLinkofAtom)pWebQuickLinkAtom).SetGroupAtoms (lstCurrentSelectedAtoms);

					AttachAtomAtView (pWebQuickLinkAtom, AttachAtomEventDefine.AttachAtomEventType.Default, false);
				}
			}

			return bIsPossible;
		}

		/// <summary>
		/// 디시리얼라이즈할때 팝업아톰 생성
		/// </summary>
		/// <param name="atomCore"></param>
		/// <returns></returns>
		private void ReadyPopupOrHyperAtom (Atom atomCore, AtomBase PopupAtom)
		{
			AttachAtomAtView (PopupAtom, AttachAtomEventDefine.AttachAtomEventType.Default, true);
		}

		private bool ReadyPopupOrHyperAtom (AtomBase PopupAtom)
		{
			Thickness PopupAtomStartPoint = new Thickness ();
			Size PopupAtomSize = Size.Empty;
			bool bIsSelectedAtomsInTabView = false;

			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInLightDMTView ();

			int nCurrentSelectedAtomsInLightDMTViewCount = lstCurrentSelectedAtoms.Count;
			int nCurrentSelectedAtomsInTabViewCount = 0;

			if (0 == nCurrentSelectedAtomsInLightDMTViewCount)
			{
				lstCurrentSelectedAtoms.Clear ();
				lstCurrentSelectedAtoms = null;
				lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInTabView ();
				nCurrentSelectedAtomsInTabViewCount = lstCurrentSelectedAtoms.Count;
				bIsSelectedAtomsInTabView = true;
			}

			//방향키로 복사할때엔 [현재 선택된 아톰들에 대한]로직은 필요없다
			if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
			{
				if (true == Keyboard.IsKeyDown (Key.Left)
					|| true == Keyboard.IsKeyDown (Key.Right)
					|| true == Keyboard.IsKeyDown (Key.Up)
					|| true == Keyboard.IsKeyDown (Key.Down))
				{
					lstCurrentSelectedAtoms.Clear ();
				}
			}

			bool bIsPossible = false;

			if (PopupAtom is PopupofAtom)
			{
				bIsPossible = IsPossibleBindPopup (lstCurrentSelectedAtoms);
			}
			else if (PopupAtom is WebHyperDataofAtom)
			{
				//2020-01-08 kys 웹탭뷰 내부에도 하이퍼링크 아톰을 생성시키기 위해서 lstCurrentSelectedAtoms에서 웹탭뷰를 삭제후 아톰생성한다.
				for (int i = 0; i < lstCurrentSelectedAtoms.Count; i++)
				{
					if (lstCurrentSelectedAtoms[i] is WebTabPanelAtomBase)
					{
						lstCurrentSelectedAtoms.RemoveAt (i);
						i--;
					}
				}

				bIsPossible = IsPossibleBindHyperLink (lstCurrentSelectedAtoms);
			}
			else if (PopupAtom is WebBlockofAtom)
			{
				bIsPossible = IsPossibleBindWebBlock (lstCurrentSelectedAtoms);
			}
			else if (PopupAtom is AutoCompleteofAtom)
			{
				bIsPossible = IsPossibleBindAutoComplete (lstCurrentSelectedAtoms);
			}

			if (true == bIsPossible)
			{
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstCurrentSelectedAtoms, ref PopupAtomStartPoint, ref PopupAtomSize);

				if (true == bIsCalculated)
				{
					PopupAtom.Margin = new Thickness (PopupAtomStartPoint.Left - 3, PopupAtomStartPoint.Top - 3, 0, 0);
					PopupAtom.Width = PopupAtomSize.Width + 6;
					PopupAtom.Height = PopupAtomSize.Height + 6;

					//팝업아톰들의 리스트를 넘겨준다.
					if (PopupAtom is PopupofAtom)
					{
						// 신규 팝업 묶기시, 기존 아톰생성 순에서 TabIndex 로 정렬
						lstCurrentSelectedAtoms.Sort (new RelativeTabIndexAtomComparer ());
						//

						((PopupofAtom)PopupAtom).BindAtoms (lstCurrentSelectedAtoms);
					}
					else if (PopupAtom is WebHyperDataofAtom)
					{
						((WebHyperDataofAtom)PopupAtom).BindAtoms (lstCurrentSelectedAtoms);
					}
					else if (PopupAtom is WebBlockofAtom)
					{
						((WebBlockofAtom)PopupAtom).BindAtoms (lstCurrentSelectedAtoms);
					}
					else if (PopupAtom is AutoCompleteofAtom)
					{
						((AutoCompleteofAtom)PopupAtom).BindAtoms (lstCurrentSelectedAtoms);
					}

					//팝업에 묶인 아톰들에게 묶였다는 속성을 준다.
					foreach (AtomBase atom in lstCurrentSelectedAtoms)
					{
						atom.SetResizeAdornerVisibility (Visibility.Collapsed, true);

						if (PopupAtom is PopupofAtom)
						{
							atom.AtomCore.IsBindedPopup = true;
						}
					}

					if (true == bIsSelectedAtomsInTabView)
					{
						if (0 < nCurrentSelectedAtomsInTabViewCount)
						{
							AtomBase FirstAtom = lstCurrentSelectedAtoms[0] as AtomBase;
							FrameworkElement TabViewElement = FirstAtom.GetTabViewAtom ();

							if (null != TabViewElement)
							{
								TabViewAtomBase TabViewAtom = TabViewElement as TabViewAtomBase;
								AttachAtomAtTabView (PopupAtom, TabViewAtom, false, -1);
							}
						}
					}
					else
					{
						AttachAtomAtView (PopupAtom, AttachAtomEventDefine.AttachAtomEventType.Default, false);
					}

					lstCurrentSelectedAtoms = null;

				}
			}

			return bIsPossible;
		}

		private void ReadyScrollAtom (Atom atomCore, ScrollAtomBase ScrollAtom)
		{
			ScrollAtom.OnNotifyReleaseBindedAtomsEvent += ScrollAtom_OnNotifyReleaseBindedAtomsEvent;
			AttachAtomAtView (ScrollAtom, AttachAtomEventDefine.AttachAtomEventType.Default, true);
		}

		/// <summary>
		/// 아톰들을 스크롤에 묶을 경우, 이 메소드로 부모에서 뗴어낸 후 아톰들을 스크롤에 묶는다.
		/// 기존 아톰들의 부모는 View이거나 TabView이므로 해당 부모에서 먼저 떼어낸 후 아톰들을 묶어야한다.
		/// </summary>
		/// <param name="sourceAtoms"></param>
		private void DeleteParentOfAtoms (List<AtomBase> sourceAtoms)
		{
			if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode)
			{
				WebDynamicGrid.RootDesignGrid.DesignAtom.RemoveAtom (sourceAtoms);
				return;
			}

			UIElementCollection CurrentChildren = GetChildren ();

			if (null != CurrentChildren)
			{
				foreach (AtomBase atom in sourceAtoms)
				{
					if (true == atom.AtomCore.IsBindedTabView)
					{
						FrameworkElement TabViewElement = atom.GetTabViewAtom ();

						if (null != TabViewElement)
						{
							TabViewAtomBase TabViewAtom = TabViewElement as TabViewAtomBase;
							TabViewAtom.DeleteAtomAboutAllTabPage (atom);

							if (atom is PopupofAtom)
							{
								PopupAtom bindPopup = atom.AtomCore as PopupAtom;
								List<AtomBase> bindAtomList = bindPopup.GetBindedAtoms ();

								if (null != bindAtomList)
								{
									foreach (AtomBase bindAtom in bindAtomList)
									{
										TabViewAtom.DeleteAtomAboutAllTabPage (bindAtom);
									}
								}
							}

							TabViewElement = null;
							TabViewAtom = null;
						}
					}
					else
					{
						CurrentChildren.Remove (atom);
					}
				}
			}

			CurrentChildren = null;
		}

		/// <summary>
		/// 여러개의 아톰이 선택되었을 때 아톰들을 포함하는 최소마진과 최소한의 영역을 구하는 함수
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="returnAtomsRegionStartMargin"></param>
		/// <param name="returnAtomsRegionSize"></param>
		/// <returns></returns>

		/// <summary>
		/// 팝업에 묶일 수 있는 아톰인지 검증한다.
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <returns></returns>
		private bool IsPossibleBindPopup (List<AtomBase> selectedAtoms)
		{
			foreach (AtomBase atom in selectedAtoms)
			{
				bool bIsBindedPopup = atom.AtomCore.IsBindedPopup;
				bool bIsBindedScroll = atom.AtomCore.IsBindedScroll;
				Type atomType = atom.GetType ();
				string strAtomName = atom.AtomCore.Attrib.DefaultAtomProperVar;

				// 2015-04-02 JAEYOUNG 제너레이터 에서는 경고창 뜨지않게함.
				if (PQAppBase.IsGeneratorMode == false)
				{
					if (atomType != typeof (InputofAtom) && atomType != typeof (DateofAtom))
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_33"), strAtomName));
						return false;
					}
					else if (true == bIsBindedScroll)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_34"), strAtomName));
						return false;
					}
					else if (true == bIsBindedPopup)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_35"), strAtomName));
						return false;
					}
				}
			}

			return true;
		}

		private bool IsPossibleBindAnimationGroup (List<AtomBase> lstCurrentSelectedAtoms)
		{
			foreach (AtomBase atom in lstCurrentSelectedAtoms)
			{
				Atom atomCore = atom.AtomCore as Atom;
				bool bIsBindedPopup = atom.AtomCore.IsBindedPopup;
				bool bIsBindedScroll = atom.AtomCore.IsBindedScroll;
				Atom hyperAtomCore = atomCore.HyperLinkAtom;
				bool bIsBindedHyperLink = null != hyperAtomCore ? true : false;

				string strAtomName = atom.AtomCore.Attrib.DefaultAtomProperVar;

				if (true == CheckBindManager.Instance.CanBindAnimationGroup (atomCore.GetType ()))
				{
					if (true == bIsBindedScroll)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_1808_3"), strAtomName));
						return false;
					}

					if (true == atomCore.Attrib.IsAnimationGroup)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_52"), strAtomName));
						return false;
					}

					//if (true == bIsBindedPopup)
					//{
					//    _Message80.Show(string.Format(LC.GS("TopProcess_DMTDoc_36"), strAtomName));
					//    return false;
					//}
					//else if (true == bIsBindedHyperLink && hyperAtomCore is WebHyperDataCore)
					//{
					//    _Message80.Show(string.Format(LC.GS("TopProcess_DMTDoc_37"), strAtomName));
					//    return false;
					//}
				}
				else
				{
					_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_26"), strAtomName, LC.GS (AtomNameDef.EBookGroupItem)));
					return false;
				}
			}
			return true;
		}

		private bool IsPossibleBindWebQuickLink (List<AtomBase> lstCurrentSelectedAtoms)
		{
			foreach (AtomBase atom in lstCurrentSelectedAtoms)
			{
				Atom atomCore = atom.AtomCore as Atom;
				bool bIsBindedPopup = atom.AtomCore.IsBindedPopup;
				bool bIsBindedScroll = atom.AtomCore.IsBindedScroll;
				Atom hyperAtomCore = atomCore.HyperLinkAtom;
				bool bIsBindedHyperLink = null != hyperAtomCore ? true : false;

				string strAtomName = atom.AtomCore.Attrib.DefaultAtomProperVar;

				if (true == CheckBindManager.Instance.CanWebQuickLinkGroup (atomCore.GetType ()))
				{
					if (true == bIsBindedScroll)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_1808_3"), strAtomName));
						return false;
					}
				}
				else
				{
					_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_26"), strAtomName, LC.GS (WebAtomName.QUICKLINK)));
					return false;
				}
			}
			return true;
		}

		private bool IsPossibleBindFloatingBar (List<AtomBase> lstCurrentSelectedAtoms)
		{
			foreach (AtomBase atom in lstCurrentSelectedAtoms)
			{
				Atom atomCore = atom.AtomCore as Atom;
				bool bIsBindedPopup = atom.AtomCore.IsBindedPopup;
				bool bIsBindedScroll = atom.AtomCore.IsBindedScroll;
				bool bIsBindedHyperLink = atomCore.IsHyperLinkBinded;
				string strAtomName = atom.AtomCore.Attrib.DefaultAtomProperVar;

				if (true == CheckBindManager.Instance.CanFloatingBarLinkGroup (atomCore.GetType ()))
				{
					if (true == bIsBindedScroll)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_1808_3"), strAtomName));
						return false;
					}
				}
				else
				{
					_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_26"), strAtomName, LC.GS (WebAtomName.QUICKLINK)));
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 하이퍼링크에 묶을 수 있는 아톰인지 검증한다. 
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <returns></returns>
		private bool IsPossibleBindHyperLink (List<AtomBase> selectedAtoms)
		{
			foreach (AtomBase atom in selectedAtoms)
			{
				Atom atomCore = atom.AtomCore as Atom;
				bool bIsBindedPopup = atom.AtomCore.IsBindedPopup;
				bool bIsBindedScroll = atom.AtomCore.IsBindedScroll;
				Atom hyperAtomCore = atomCore.HyperLinkAtom;
				bool bIsBindedHyperLink = null != hyperAtomCore ? true : false;

				string strAtomName = atom.AtomCore.Attrib.DefaultAtomProperVar;

				if (true == CheckBindManager.Instance.CanBindHyperLink (atomCore.GetType ()))
				{
					//스크롤에 또 묶일수 있음
					//if (true == bIsBindedScroll)
					//{
					//    _Message80.Show(string.Format("[{0}]은 스크롤에 묶여 있습니다.", strAtomName));
					//    return false;
					//}
					if (true == bIsBindedPopup)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_36"), strAtomName));
						return false;
					}
					else if (true == bIsBindedHyperLink && hyperAtomCore is WebHyperDataAtom)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_37"), strAtomName));
						return false;
					}
				}
				else
				{
					_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_26"), strAtomName, LC.GS (WebAtomName.HYPERLINK)));
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 블럭묶기에 묶을 수 있는 아톰인지 검증한다. 
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <returns></returns>
		private bool IsPossibleBindWebBlock (List<AtomBase> selectedAtoms)
		{
			foreach (AtomBase atom in selectedAtoms)
			{
				Atom atomCore = atom.AtomCore as Atom;
				bool bIsBindedPopup = atom.AtomCore.IsBindedPopup;
				bool bIsBindedScroll = atom.AtomCore.IsBindedScroll;
				Atom bindAtomCore = atomCore.BindBlockAtom;
				bool bIsBindedBlock = null != bindAtomCore ? true : false;

				string strAtomName = atom.AtomCore.Attrib.DefaultAtomProperVar;

				if (true == CheckBindManager.Instance.CanBindBlock (atomCore.GetType ()))
				{
					if (null != bindAtomCore)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_26"), strAtomName, LC.GS (WebAtomName.BLOCK)));
					}
					else
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_36"), strAtomName));
					}

					return false;
				}
				else
				{
					if (true == bIsBindedScroll)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_34"), strAtomName));
						return false;
					}
					else if (true == bIsBindedPopup)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_35"), strAtomName));
						return false;
					}
					else if (null != bindAtomCore)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_26"), strAtomName, LC.GS (WebAtomName.BLOCK)));
					}
				}
			}
			return true;
		}

		/// <summary>
		/// 자동완성에 묶을 수 있는 아톰인지 검증한다. 
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <returns></returns>
		private bool IsPossibleBindAutoComplete (List<AtomBase> selectedAtoms)
		{
			foreach (AtomBase atom in selectedAtoms)
			{
				Atom atomCore = atom.AtomCore as Atom;
				bool bIsBindedPopup = atom.AtomCore.IsBindedPopup;
				bool bIsBindedScroll = atom.AtomCore.IsBindedScroll;
				Atom autoCompleteAtomCore = atomCore.AutoCompleteAtom;
				bool bIsBindedAutoComplete = null != autoCompleteAtomCore ? true : false;

				string strAtomName = atom.AtomCore.Attrib.DefaultAtomProperVar;

				if (true == CheckBindManager.Instance.CanBindAutoComplete (atomCore.GetType ()))
				{
					if (true == bIsBindedPopup)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_36"), strAtomName));
						return false;
					}
					else if (true == bIsBindedAutoComplete && autoCompleteAtomCore is AutoCompleteAtom)
					{
						_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_37"), strAtomName));
						return false;
					}
				}
				else
				{
					_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_26"), strAtomName, LC.GS (AtomName.AUTOCOMPLETE)));
					return false;
				}
			}

			return true;
		}

		private bool IsPossibleBindScroll (List<AtomBase> selectedAtoms)
		{
			foreach (AtomBase atom in selectedAtoms)
			{
				bool bIsBindedPopup = atom.AtomCore.IsBindedPopup;
				bool bIsBindedScroll = atom.AtomCore.IsBindedScroll;
				bool IsAnimationGroup = atom.AtomCore.GetAttrib ().IsAnimationGroup;
				Type atomType = atom.GetType ();
				string strAtomName = atom.AtomCore.Attrib.DefaultAtomProperVar;

				if (true == bIsBindedPopup)
				{
					_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_38"), strAtomName));
					return false;
				}
				else if (true == bIsBindedScroll)
				{
					_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_39"), strAtomName));
					return false;
				}
				else if (true == IsAnimationGroup)
				{
					_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_52"), strAtomName)); //[]은 이미 그룹묶기에 묶여 있습니다.
					return false;
				}
				// 2019-10-21 kys 게시판 아톰은 스크롤에 묶을수 없도록 수정함
				// 2023.08.24 묶을 수 없는 아톰 판독 논리 전체적으로 보강 (AtomCore로 비교해 상속받는 아톰들도 적용될 수 있게 보강)
				else if (atom.AtomCore is TabViewAtom ||
					atom.AtomCore is ScrollAtom ||
					atom.AtomCore is BrowseAtom ||
					atom is GridTableOfAtom ||
					atom is GridTableExofAtom ||
					atom is EBookAnimationGroupAtomBase)
				{
					_Message80.Show (string.Format (LC.GS ("TopProcess_DMTDoc_40"), strAtomName));
					return false;
				}
			}

			return true;
		}

		private void HandleNotifyCommandEvent (object objValue)
		{
			ICommandBehavior SourceCommand = objValue as ICommandBehavior;
			Commander.AddCommand (SourceCommand);
			Commander.ExecuteCommand ();
		}

		/// <summary>
		/// 한 단계 앞으로
		/// </summary>
		/// <param name="SourceAtom"></param>
		private void AdjustAtomZIndexFrontAction (AtomBase SourceAtom)
		{
			UIElementCollection CurrentChildren = GetChildren ();
			int nChildrenCount = CurrentChildren.Count;

			int nCurrentAtomZindex = Canvas.GetZIndex (SourceAtom);
			int nLookingForIndex = nCurrentAtomZindex + 1;

			foreach (UIElement element in CurrentChildren)
			{
				int nTargetElementZIndex = Canvas.GetZIndex (element);

				if (nLookingForIndex == nTargetElementZIndex)
				{
					Canvas.SetZIndex (element, nCurrentAtomZindex);
					Canvas.SetZIndex (SourceAtom, nLookingForIndex);

					if (element is AtomBase targetAtom)
					{
						targetAtom.AtomCore.Attrib.ViewIndex = nCurrentAtomZindex;
						SourceAtom.AtomCore.Attrib.ViewIndex = nLookingForIndex;
					}
					break;
				}
			}
		}

		/// <summary>
		/// 맨 앞으로 가져오기
		/// </summary>
		/// <param name="SourceAtom"></param
		private void AdjustAtomZIndexFirstAction (AtomBase SourceAtom)
		{
			UIElementCollection CurrentChildren = GetChildren ();
			int nChildrenCount = CurrentChildren.Count;

			int nCurrentAtomZindex = Canvas.GetZIndex (SourceAtom);

			foreach (UIElement element in CurrentChildren)
			{
				int nTargetElementZIndex = Canvas.GetZIndex (element);

				if (nCurrentAtomZindex < nTargetElementZIndex)
				{
					Canvas.SetZIndex (element, nTargetElementZIndex - 1);

					if (element is AtomBase targetAtom)
					{
						targetAtom.AtomCore.Attrib.ViewIndex = nTargetElementZIndex - 1;
					}
				}
			}

			Canvas.SetZIndex (SourceAtom, nChildrenCount);
			SourceAtom.AtomCore.Attrib.ViewIndex = nChildrenCount;
		}

		/// <summary>
		/// 한 단계 뒤로
		/// </summary>
		/// <param name="SourceAtom"></param>
		private void AdjustAtomZIndexBackAction (AtomBase SourceAtom)
		{
			UIElementCollection CurrentChildren = GetChildren ();
			int nChildrenCount = CurrentChildren.Count;

			int nCurrentAtomZindex = Canvas.GetZIndex (SourceAtom);
			int nLookingForIndex = nCurrentAtomZindex - 1;

			foreach (UIElement element in CurrentChildren)
			{
				int nTargetElementZIndex = Canvas.GetZIndex (element);

				if (nLookingForIndex == nTargetElementZIndex)
				{
					Canvas.SetZIndex (element, nCurrentAtomZindex);
					Canvas.SetZIndex (SourceAtom, nLookingForIndex);

					if (element is AtomBase targetAtom)
					{
						targetAtom.AtomCore.Attrib.ViewIndex = nCurrentAtomZindex;
						SourceAtom.AtomCore.Attrib.ViewIndex = nLookingForIndex;
					}
					break;
				}
			}
		}

		/// <summary>
		/// 맨 뒤로 보내기
		/// </summary>
		/// <param name="SourceAtom"></param>
		public void AdjustAtomZIndexLastAction (AtomBase SourceAtom)
		{
			UIElementCollection CurrentChildren = GetChildren ();
			int nChildrenCount = CurrentChildren.Count;

			int nCurrentAtomZindex = Canvas.GetZIndex (SourceAtom);

			foreach (UIElement element in CurrentChildren)
			{
				int nTargetElementZIndex = Canvas.GetZIndex (element);

				if (nCurrentAtomZindex > nTargetElementZIndex)
				{
					Canvas.SetZIndex (element, nTargetElementZIndex + 1);

					if (element is AtomBase targetAtom)
					{
						targetAtom.AtomCore.Attrib.ViewIndex = nTargetElementZIndex + 1;
					}
				}
			}

			Canvas.SetZIndex (SourceAtom, 1);
			SourceAtom.AtomCore.Attrib.ViewIndex = 1;
		}

		private void AdjustAtomZindexAboutDelete (object hittedAtom)
		{
			AtomBase currentAtom = hittedAtom as AtomBase;

			if (null != currentAtom)
			{
				UIElementCollection CurrentChildren = GetChildren ();
				int nChildrenCount = CurrentChildren.Count;

				//현재 눌려진 hittedAtom를 중심으로 상단에 있는 Atom들의 zindex를 -1씩 해준다.
				int nCurrentAtomZindex = Canvas.GetZIndex (currentAtom);

				foreach (UIElement element in CurrentChildren)
				{
					int nTargetElementZIndex = Canvas.GetZIndex (element);

					if (nCurrentAtomZindex < nTargetElementZIndex)
					{
						Canvas.SetZIndex (element, nTargetElementZIndex - 1);

						if (element is AtomBase targetAtom)
						{
							targetAtom.AtomCore.Attrib.ViewIndex = nTargetElementZIndex - 1;
						}
					}
				}
			}
		}

		private AtomBase OnEditModeMoveFocusOnNextRelativeTabIndexAtom (AtomBase sourceAtom, FormMode.FrameMode CurrentFormMode, bool IsReverse)
		{
			if (FormMode.FrameMode.Executed != CurrentFormMode)
			{
				List<AtomBase> lstCurrentAtomBases = GetViewChildren ();
				int nChildrenCount = lstCurrentAtomBases.Count;

				if (null != lstCurrentAtomBases)
				{
					lstCurrentAtomBases.Sort (new RelativeTabIndexAtomComparer ());

					int nSourceAtomTabIndex = sourceAtom.AtomCore.Attrib.AtomRelativeTabIndex;
					AtomBase FirstAtom = null;

					if (true == IsReverse)
					{
						lstCurrentAtomBases.Reverse ();
					}

					foreach (AtomBase willNextAtomBase in lstCurrentAtomBases)
					{
						if (null == FirstAtom)
						{
							FirstAtom = willNextAtomBase;
						}

						int nWillRelativeTabIndex = willNextAtomBase.AtomCore.Attrib.AtomRelativeTabIndex;

						if (false == IsReverse && nSourceAtomTabIndex < nWillRelativeTabIndex)
						{
							sourceAtom.SetResizeAdornerVisibility (Visibility.Collapsed, false);
							willNextAtomBase.SetResizeAdornerVisibility (Visibility.Visible, false);
							return willNextAtomBase;
						}
						else if (true == IsReverse && nSourceAtomTabIndex > nWillRelativeTabIndex)
						{
							sourceAtom.SetResizeAdornerVisibility (Visibility.Collapsed, false);
							willNextAtomBase.SetResizeAdornerVisibility (Visibility.Visible, false);
							return willNextAtomBase;
						}
					}

					sourceAtom.SetResizeAdornerVisibility (Visibility.Collapsed, false);
					FirstAtom?.SetResizeAdornerVisibility (Visibility.Visible, false);

					return FirstAtom;
				}
			}
			return sourceAtom;
		}

		private void OnRunModeMoveFocusOnNextRelativeTabIndexAtom (AtomBase CurrentTabFocussedAtom)
		{
			CurrentTabFocussedAtom.LostFocusOnRunMode ();

			AtomBase willNextTabAtomBase = OnRunModeFindNextTabAtomInDMTView (CurrentTabFocussedAtom);

			if (null == willNextTabAtomBase)
				return;

			if (willNextTabAtomBase is ScrollAtomBase)
			{
				ScrollAtomBase scrollAtomBase = willNextTabAtomBase as ScrollAtomBase;
				scrollAtomBase.OnRunModeMoveFocusOnNextRelativeTabIndexAtom (CurrentTabFocussedAtom);
			}
			else
			{
				willNextTabAtomBase.AtomCore.Enable (true, true, 1);
			}
		}

		private AtomBase OnRunModeFindNextTabAtomInDMTView (AtomBase CurrentTabFocussedAtom)
		{
			if (CurrentTabFocussedAtom is TabViewAtomBase)
			{
				var tabviewAtom = CurrentTabFocussedAtom as TabViewAtomBase;
				if (null != tabviewAtom)
				{
					var atoms = tabviewAtom.GetCurrentActiveTabPage ().GetAllAtomList ();
					if (null != atoms && 0 < atoms.Count)
					{
						return atoms.First ();
					}
				}
			}

			List<AtomBase> lstCurrentAtomBases = GetViewChildren ();

			if (null != lstCurrentAtomBases)
			{
				lstCurrentAtomBases.Sort (new RelativeTabIndexAtomComparer ());

				int nSourceAtomTabIndex = CurrentTabFocussedAtom.AtomCore.Attrib.AtomRelativeTabIndex;
				int nWillRelativeTabIndex = -1;
				AtomBase FirstAtom = null;

				foreach (AtomBase willNextAtomBase in lstCurrentAtomBases)
				{
					if (false == willNextAtomBase.IsRunModeTabMovable ())
					{
						continue;
					}

					if (null == FirstAtom)
					{
						FirstAtom = willNextAtomBase;
					}

					nWillRelativeTabIndex = willNextAtomBase.AtomCore.Attrib.AtomRelativeTabIndex;

					if (nSourceAtomTabIndex < nWillRelativeTabIndex)
					{
						return willNextAtomBase;
					}
				}
				return FirstAtom;
			}
			else
			{
				return CurrentTabFocussedAtom;
			}
		}

		/// <summary>
		/// 런모드에서 현재 탭 포커스 가진 아톰 반환
		/// </summary>
		/// <returns></returns>
		private AtomBase GetRunModeCurrentTabFocussedAtom ()
		{
			List<Atom> lstAllAtoms = GetAllAtomCores ();

			foreach (Atom atomCore in lstAllAtoms)
			{
				if (atomCore is ExtensionScrollAtom)
				{
					ExtensionScrollofAtom ofAtom = atomCore.GetOfAtom () as ExtensionScrollofAtom;
					AtomBase focusAtom = ofAtom.GetFocusAtom ();

					if (null != focusAtom)
						return focusAtom;
				}
				else if (atomCore is ScrollAtom)
				{
					foreach (CMultiList realAtoms in ((ScrollAtom)atomCore).RealAtoms)
					{
						foreach (Atom clonedAtom in realAtoms)
						{
							AtomBase clonedAtomBase = clonedAtom.GetOfAtom ();

							if (null != clonedAtomBase)
							{
								if (true == clonedAtomBase.IsFocusedOnRunMode ())
								{
									return clonedAtomBase;
								}
							}
						}
					}
				}
				else if (atomCore is GridTableAtom)
				{
					GridTableAtom gridTableAtom = atomCore as GridTableAtom;
					AtomBase atomBase = gridTableAtom.GetFocusedOnRunMode ();
					if (null != atomBase && false != atomBase.IsFocusedOnRunMode ())
					{
						return atomBase;
					}
				}
				else
				{
					AtomBase atomBase = atomCore.GetOfAtom () as AtomBase;

					if (true == atomBase.IsFocusedOnRunMode ())
					{
						return atomBase;
					}
				}
			}
			return ((Atom)lstAllAtoms[0]).GetOfAtom ();
		}

		private DIR_TYPE CalculateAtomMarginForKeyboard (Key direction, ModifierKeys Modifier, out int offsetX, out int offsetY, out int offsetW, out int offsetH)
		{
			DIR_TYPE type = DIR_TYPE.DIR_NONE;
			offsetX = 0;
			offsetY = 0;
			offsetW = 0;
			offsetH = 0;

			if (ModifierKeys.Shift == Modifier)
			{
				//크기 확장
				var sizeOffset = 1;

				switch (direction)
				{
					case Key.Left:
						offsetX = sizeOffset * -1;
						offsetW = sizeOffset;
						type = DIR_TYPE.DIR_SHIFT_LEFT;
						break;
					case Key.Right:
						//offsetX = sizeOffset;
						offsetW = sizeOffset;
						type = DIR_TYPE.DIR_SHIFT_RIGHT;
						break;
					case Key.Up:
						offsetY = sizeOffset * -1;
						offsetH = sizeOffset;
						type = DIR_TYPE.DIR_SHIFT_UP;
						break;
					case Key.Down:
						//offsetY = sizeOffset;
						offsetH = sizeOffset;
						type = DIR_TYPE.DIR_SHIFT_DOWN;
						break;
					default: break;
				}
			}
			else if ((ModifierKeys.Shift | ModifierKeys.Control) == Modifier)
			{
				//크기 축소
				var sizeOffset = 1;

				switch (direction)
				{
					case Key.Left:
						offsetW = sizeOffset * -1;
						type = DIR_TYPE.DIR_SHIFT_CTRL_LEFT;
						break;
					case Key.Right:
						offsetX = sizeOffset;
						offsetW = sizeOffset * -1;
						type = DIR_TYPE.DIR_SHIFT_CTRL_RIGHT;
						break;
					case Key.Up:
						offsetH = sizeOffset * -1;
						type = DIR_TYPE.DIR_SHIFT_CTRL_UP;
						break;
					case Key.Down:
						offsetY = sizeOffset;
						offsetH = sizeOffset * -1;
						type = DIR_TYPE.DIR_SHIFT_CTRL_DOWN;
						break;
					default: break;
				}
			}
			else
			{
				//위치 변경
				int moveOffset = ModifierKeys.Control == Modifier ? 1 : 10;

				switch (direction)
				{
					case Key.Left:
						offsetX = moveOffset * -1;
						type = ModifierKeys.Control == Modifier ? DIR_TYPE.DIR_CTRL_LEFT : DIR_TYPE.DIR_LEFT;
						break;
					case Key.Right:
						offsetX = moveOffset;
						type = ModifierKeys.Control == Modifier ? DIR_TYPE.DIR_CTRL_RIGHT : DIR_TYPE.DIR_RIGHT;
						break;
					case Key.Up:
						offsetY = moveOffset * -1;
						type = ModifierKeys.Control == Modifier ? DIR_TYPE.DIR_CTRL_UP : DIR_TYPE.DIR_UP;
						break;
					case Key.Down:
						offsetY = moveOffset;
						type = ModifierKeys.Control == Modifier ? DIR_TYPE.DIR_CTRL_DOWN : DIR_TYPE.DIR_DOWN;
						break;
					default: break;
				}
			}

			return type;
		}

		/// <summary>
		/// [alt + 방향키] 복제시 DBIO 및 기타 정보 넘김
		/// </summary>
		/// <param name="TargetAtom"></param>
		/// <param name="OriginAtom"></param>
		private void ProcessCopyAtomCores (AtomBase TargetAtom, AtomBase OriginAtom)
		{
			Atom targetAtomCore = TargetAtom.AtomCore as Atom;
			Atom originAtomCore = OriginAtom.AtomCore as Atom;

			targetAtomCore.SetScriptIndex (this.GetMaxScriptIndex ());
			targetAtomCore.SetDBMaster (originAtomCore.GetDBMaster ());
			targetAtomCore.SetInformation (originAtomCore.Information);
			targetAtomCore.SetGDIManager (originAtomCore.GetGDIManager ());
		}

		private void ProcessCopyAtoms (List<AtomBase> selectedAtoms, Size EndSize, Key direction)
		{
			double dStretchWidth = (EndSize.Width + PQAppBase.DefaultInterval);
			double dStretchHeight = (EndSize.Height + PQAppBase.DefaultInterval);
			DMTView CurrentDMTView = GetParentView () as DMTView;

			if (null != CurrentDMTView)
			{
				SetAllAtomsAdornerVisibilityInLightDMTView (Visibility.Collapsed, true);
				MakeAtomCommand NewMakeAtomCommand = null;
				List<AtomBase> ClonedAtoms = new List<AtomBase> ();

				List<MakeAtomCommand> lstNewMakeAtomCommands = new List<MakeAtomCommand> ();

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness orgAtomMargin = atom.Margin;
					AtomType CurrentMakingAtomType = atom.AtomCore.AtomType;
					double dAtomWidth = atom.Width;
					double dAtomHeight = atom.Height;
					double dNewLeft = orgAtomMargin.Left;
					double dNewTop = orgAtomMargin.Top;
					bool bIsBindedPopup = atom.AtomCore.IsBindedPopup;
					bool bIsBindedScroll = atom.AtomCore.IsBindedScroll;

					//얕은 복사에서는 스크롤,팝업는 복사가 안된다.
					if (atom is ScrollAtomBase || atom is PopupAtomBase)
					{
						continue;
					}

					if (true == bIsBindedPopup || true == bIsBindedScroll)
					{
						continue;
					}

					switch (direction)
					{
						case Key.Down:
							{
								dNewLeft = orgAtomMargin.Left;
								dNewTop = orgAtomMargin.Top + dStretchHeight;
								break;
							}
						case Key.Right:
							{
								dNewLeft = orgAtomMargin.Left + dStretchWidth;
								dNewTop = orgAtomMargin.Top;
								break;
							}
						case Key.Left:
							{
								dNewLeft = orgAtomMargin.Left - dStretchWidth;

								//2014-10-07-M01 Alt + 방향키 아톰 복사 처리 ( 70버젼과 같이 )
								if (dNewLeft < 0)
									dNewLeft = 0;

								dNewTop = orgAtomMargin.Top;
								break;
							}
						case Key.Up:
							{
								dNewLeft = orgAtomMargin.Left;
								dNewTop = orgAtomMargin.Top - dStretchHeight;
								break;
							}
					}

					NewMakeAtomCommand = new MakeAtomCommand (CurrentDMTView, CurrentMakingAtomType, dNewLeft, dNewTop, new Size (dAtomWidth, dAtomHeight), null);

					//Commander.AddCommand(NewMakeAtomCommand);
					//object objMakedAtom = Commander.ExecuteCommand();

					object objMakedAtom = NewMakeAtomCommand.Execute ();
					AtomBase MakedAtom = objMakedAtom as AtomBase;

					ProcessCopyAtomCores (MakedAtom, atom);
					atom.CloneAtom (MakedAtom, false);
					MakedAtom.Margin = new Thickness (dNewLeft, dNewTop, 0, 0);
					ClonedAtoms.Add (MakedAtom);

					FrameworkElement OwnerElement = atom.GetOwnerView ();

					if (null != OwnerElement)
					{
						if (OwnerElement is TabViewAtomBase)
						{
							TabViewAtomBase OwnerTabView = OwnerElement as TabViewAtomBase;
							Thickness OwnerTabViewMargin = OwnerTabView.Margin;
							CurrentDMTView.Children.Remove (MakedAtom);

							AdjustSourceAtomRegionInTabView (MakedAtom, OwnerTabView);
							AttachAtomAtTabView (MakedAtom, OwnerTabView, false, -1);
							MakedAtom.Margin = new Thickness (dNewLeft, dNewTop, 0, 0);
						}
					}

					lstNewMakeAtomCommands.Add (NewMakeAtomCommand);
				}

				int nCountOfCommands = lstNewMakeAtomCommands.Count;

				if (0 < nCountOfCommands)
				{
					// Undo를 위한 ( Make한 후 excute를 실행했기 때문에 Group Commmand에서는 Execute 함수를 실행하지 않는다. )
					MakeGroupedAtomsCommand NewGroupedCommand = new MakeGroupedAtomsCommand (lstNewMakeAtomCommands);
					Commander.AddCommand (NewGroupedCommand);
					Commander.NoExecuteCommand ();

					foreach (AtomBase CurrentAtom in ClonedAtoms)
					{
						CurrentAtom.SetResizeAdornerVisibility (Visibility.Visible, false);
					}

					if (0 < ClonedAtoms.Count)
					{
						ClonedAtoms[0].AtomCore.Information.UpdatetPropertyToolBar (ClonedAtoms[0]);
					}
				}
			}
		}

		private bool IsPossibleCopyAtomsAtRegularIntervals (Key direction, Thickness startMargin, Size EndSize)
		{
			bool bIsPossible = false;

			Size LightDMTViewSize = GetViewSize ();

			if (null != LightDMTViewSize)
			{
				bIsPossible = IsPossibleCopyAboutRegion (direction, startMargin, EndSize, LightDMTViewSize);
			}

			return bIsPossible;
		}

		private bool IsPossibleCopyAboutRegion (Key direction, Thickness startMargin, Size EndSize, Size LightDMTViewSize)
		{
			Thickness CopiedAtomsRegionMargin;
			double dCopiedAtomsRegionEndX;
			double dCopiedAtomsRegionEndY;

			switch (direction)
			{
				case Key.Down:
					{
						CopiedAtomsRegionMargin = new Thickness (startMargin.Left, (startMargin.Top + EndSize.Height + PQAppBase.DefaultInterval), 0, 0);
						dCopiedAtomsRegionEndY = (CopiedAtomsRegionMargin.Top + EndSize.Height);

						if (dCopiedAtomsRegionEndY <= LightDMTViewSize.Height)
						{
							return true;
						}

						break;
					}
				case Key.Right:
					{
						CopiedAtomsRegionMargin = new Thickness (startMargin.Left + EndSize.Width + PQAppBase.DefaultInterval, startMargin.Top, 0, 0);
						dCopiedAtomsRegionEndX = (CopiedAtomsRegionMargin.Left + EndSize.Width);

						//2014-10-07-M01 Alt + 방향키 아톰 복사 처리 ( 70버젼과 같이 )
						//if (dCopiedAtomsRegionEndX <= LightDMTViewSize.Width)
						//{
						//    return true;
						//}

						return true;
					}
				case Key.Left:
					{
						CopiedAtomsRegionMargin = new Thickness (startMargin.Left - EndSize.Width - PQAppBase.DefaultInterval, startMargin.Top, 0, 0);
						dCopiedAtomsRegionEndX = (CopiedAtomsRegionMargin.Left + EndSize.Width);

						//2014-10-07-M01 Alt + 방향키 아톰 복사 처리 ( 70버젼과 같이 )
						//if (CopiedAtomsRegionMargin.Left >= 0)
						//{
						//    return true;
						//}

						return true;
					}
				case Key.Up:
					{
						CopiedAtomsRegionMargin = new Thickness (startMargin.Left, (startMargin.Top - EndSize.Height - PQAppBase.DefaultInterval), 0, 0);
						dCopiedAtomsRegionEndX = (CopiedAtomsRegionMargin.Left + EndSize.Width);

						if (CopiedAtomsRegionMargin.Top >= 0)
						{
							return true;
						}

						break;
					}

				default: return false;
			}

			return false;
		}

		private void StartCopyAtoms (Key direction, List<AtomBase> selectedAtoms, Thickness startMargin, Size EndSize)
		{
			ProcessCopyAtoms (selectedAtoms, EndSize, direction);
		}

		/// <summary>
		/// Ctrl 키보드 상태에서 마우스 이동시 아톰복사
		/// </summary>
		/// <param name="ptMove"></param>
		private void ProcessCopyAtoms (List<AtomBase> selectedAtoms, Point ptMove)
		{
			DMTView CurrentDMTView = GetParentView () as DMTView;
			if (null != CurrentDMTView)
			{
				SetAllAtomsAdornerVisibilityInLightDMTView (Visibility.Collapsed, true);
				MakeAtomCommand NewMakeAtomCommand = null;
				List<AtomBase> ClonedAtoms = new List<AtomBase> ();

				List<MakeAtomCommand> lstNewMakeAtomCommands = new List<MakeAtomCommand> ();

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness orgAtomMargin = atom.Margin;
					AtomType CurrentMakingAtomType = atom.AtomCore.AtomType;
					double dAtomWidth = atom.Width;
					double dAtomHeight = atom.Height;
					double dNewLeft = orgAtomMargin.Left;
					double dNewTop = orgAtomMargin.Top;
					//Kiho : 2016-11-10 : 위의 2줄 주석처리 후 아래 2줄 추가, 원본 위치에 Copy 하는 것이 아니라 이동한 위치에 Copy
					//double dNewLeft = orgAtomMargin.Left + ptMove.X;
					//double dNewTop = orgAtomMargin.Top + ptMove.Y;
					bool bIsBindedPopup = atom.AtomCore.IsBindedPopup;
					bool bIsBindedScroll = atom.AtomCore.IsBindedScroll;

					//얕은 복사에서는 스크롤,팝업,탭뷰는 복사가 안된다.
					if (atom is ScrollAtomBase || atom is PopupAtomBase || atom is TabViewAtomBase)
					{
						continue;
					}

					if (true == bIsBindedPopup || true == bIsBindedScroll)
					{
						continue;
					}

					NewMakeAtomCommand = new MakeAtomCommand (CurrentDMTView, CurrentMakingAtomType, dNewLeft, dNewTop, new Size (dAtomWidth, dAtomHeight), null);

					object objMakedAtom = NewMakeAtomCommand.Execute ();
					AtomBase MakedAtom = objMakedAtom as AtomBase;

					ProcessCopyAtomCores (MakedAtom, atom);
					atom.CloneAtom (MakedAtom, false);
					MakedAtom.Margin = new Thickness (dNewLeft, dNewTop, 0, 0);
					ClonedAtoms.Add (MakedAtom);
					//Kiho : 2016-11-10 : 추가
					this.m_lstOriginalCopyedAtoms.Add (MakedAtom);

					FrameworkElement OwnerElement = atom.GetOwnerView ();
					if (null != OwnerElement)
					{
						if (OwnerElement is TabViewAtomBase)
						{
							TabViewAtomBase OwnerTabView = OwnerElement as TabViewAtomBase;
							Thickness OwnerTabViewMargin = OwnerTabView.Margin;
							CurrentDMTView.Children.Remove (MakedAtom);

							AdjustSourceAtomRegionInTabView (MakedAtom, OwnerTabView);
							AttachAtomAtTabView (MakedAtom, OwnerTabView, false, -1);
							MakedAtom.Margin = new Thickness (dNewLeft, dNewTop, 0, 0);
						}
					}

					lstNewMakeAtomCommands.Add (NewMakeAtomCommand);
				}//end foreach atom

				int nCountOfCommands = lstNewMakeAtomCommands.Count;

				if (0 < nCountOfCommands)
				{
					// Undo를 위한 ( Make한 후 excute를 실행했기 때문에 Group Commmand에서는 Execute 함수를 실행하지 않는다. )
					MakeGroupedAtomsCommand NewGroupedCommand = new MakeGroupedAtomsCommand (lstNewMakeAtomCommands);
					Commander.AddCommand (NewGroupedCommand);
					Commander.NoExecuteCommand ();
				}

				foreach (AtomBase CurrentAtom in ClonedAtoms)
				{
					CurrentAtom.SetResizeAdornerVisibility (Visibility.Visible, true);
				}

				foreach (AtomBase CurrentAtom in selectedAtoms)
				{
					Thickness thickness = CurrentAtom.Margin;
					CurrentAtom.Margin = new Thickness (thickness.Left + ptMove.X, thickness.Top + ptMove.Y, 0, 0);

					CurrentAtom.SetResizeAdornerVisibility (Visibility.Collapsed, true);
				}
			}
		}

		private bool ApplyObjectPropertyToolBarGroupEvent (int nEventKey, object value)
		{

			List<AtomBase> CurrentSelectedAtoms = GetCurrentSelectedAtoms ();

			if (null != CurrentSelectedAtoms)
			{
				int nCurrentSelectedAtomsCount = CurrentSelectedAtoms.Count;

				if (0 < nCurrentSelectedAtomsCount)
				{
					AtomBase FirstAtom = CurrentSelectedAtoms[0] as AtomBase;
					FrameworkElement OwnerElement = FirstAtom.GetOwnerView ();

					if (null != OwnerElement)
					{
						if (OwnerElement is TopView)
						{
							ApplyObjectPropertyAtSelectedAtoms (nEventKey, value, CurrentSelectedAtoms, OwnerViewDefine.OwnerViewType.View);
						}
						else if (OwnerElement is TabViewAtomBase)
						{
							ApplyObjectPropertyAtSelectedAtoms (nEventKey, value, CurrentSelectedAtoms, OwnerViewDefine.OwnerViewType.TabView);
						}
						else if (OwnerElement is ScrollAtomBase)
						{
							ApplyObjectPropertyAtSelectedAtoms (nEventKey, value, CurrentSelectedAtoms, OwnerViewDefine.OwnerViewType.Scroll);
						}
						else
						{
							if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode)
							{
								//대체디자인일때 사용
								ApplyObjectPropertyAtSelectedAtoms (nEventKey, value, CurrentSelectedAtoms, OwnerViewDefine.OwnerViewType.View);
							}
						}
					}
					else if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode)
					{
						//반응형웹 대체디자인인 경우 별도 View에서 아톰을 찾아온다.
						if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode)
						{
							List<AtomBase> selectAtomList = WebDynamicGrid.RootDesignGrid.GetSelectAtomList ();
							ApplyObjectPropertyAtSelectedAtoms (nEventKey, value, selectAtomList, OwnerViewDefine.OwnerViewType.View);
							return true;
						}
					}
				}
				//뷰인 경우
				else
				{
					DMTView CurrentView = GetParentView () as DMTView;

					if (213 == nEventKey) //배경색 설정할때
					{
						if (null != WebDynamicGrid)
						{
							if (0 < WebDynamicGrid.GetSelectItemList ().Count)
							{
								Brush applyBrush = value as Brush;
								if (null != applyBrush)
								{
									DynamicGridTableCommand command = new DynamicGridTableCommand ();
									command.TargetGridTable = WebDynamicGrid;
									command.SetBeforeGridItemList (WebDynamicGrid.GridCellItem);

									WebDynamicGrid.SetBackground (applyBrush);

									command.SetAfterGridItemList (WebDynamicGrid.GridCellItem);
									m_CommandCommander.AddCommand (command);
									return true;
								}
							}
						}
					}
					else if (258 == nEventKey)
					{
						bool isExpandArea = _Kiss.toBool (value);

						DynamicGridTable gridTable = WebDynamicGrid as DynamicGridTable;

						if (null != gridTable)
						{
							gridTable.SetBackgroundExpandArea (isExpandArea);
						}
					}

					if (null != CurrentView)
					{
						ApplyObjectPropertyAtView (nEventKey, value);
					}
				}
			}

			CurrentSelectedAtoms = null;

			return true;
		}

		private void ApplyObjectPropertyAtView (int eventKey, object value)
		{

			var view = GetParentView () as DMTView;
			view?.SetViewPropertiesEvent (eventKey, value);

		}

		private void ApplyObjectPropertyAtSelectedAtoms (int nEventKey, object value, List<AtomBase> CurrentSelectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			bool bIsApply = false;

			switch (nEventKey)
			{
				//FontFamily
				case 200:
					{
						FontFamily font = (FontFamily)value;
						ApplyFontFamilyEvent (CurrentSelectedAtoms, font);
						break;
					}

				//FontSize
				case 201:
					{
						ApplyFontSizeEvent (CurrentSelectedAtoms, value);
						break;
					}

				//Bold
				case 202:
					{
						bIsApply = (bool)value;
						ApplyFontWeightEvent (CurrentSelectedAtoms, bIsApply);
						break;
					}

				//Italic
				case 203:
					{
						bIsApply = (bool)value;
						ApplyFontStyleEvent (CurrentSelectedAtoms, bIsApply);
						break;
					}

				//Underline
				case 204:
					{
						bIsApply = (bool)value;
						ApplyFontUnderLineEvent (CurrentSelectedAtoms, bIsApply);
						break;
					}

				//Text LeftAlignment
				case 205:
					{
						ApplyHorizontalTextAlignmentEvent (CurrentSelectedAtoms, HorizontalAlignment.Left);
						break;
					}

				//Text HorizontalMiddleAlignment
				case 206:
					{
						ApplyHorizontalTextAlignmentEvent (CurrentSelectedAtoms, HorizontalAlignment.Center);
						break;
					}

				//Text RightAlignment
				case 207:
					{
						ApplyHorizontalTextAlignmentEvent (CurrentSelectedAtoms, HorizontalAlignment.Right);
						break;
					}

				//Text VerticalTopAlignment
				case 208:
					{
						ApplyVerticalTextAlignmentEvent (CurrentSelectedAtoms, VerticalAlignment.Top);
						break;
					}

				//Text VerticalCenterAlignment
				case 209:
					{
						ApplyVerticalTextAlignmentEvent (CurrentSelectedAtoms, VerticalAlignment.Center);
						break;
					}

				//Text VerticalBottomAlignment
				case 210:
					{
						ApplyVerticalTextAlignmentEvent (CurrentSelectedAtoms, VerticalAlignment.Bottom);
						break;
					}

				//Font Color
				case 211:
					{
						Brush applyBrush = (Brush)value;
						ApplyFontColorEvent (CurrentSelectedAtoms, applyBrush);
						break;
					}

				//Line Color
				case 212:
					{
						Brush applyBrush = (Brush)value;
						ApplyLineColorEvent (CurrentSelectedAtoms, applyBrush);
						break;
					}

				//Background Color
				case 213:
					{
						ApplyBackgroundColorEvent (CurrentSelectedAtoms, value);
						break;
					}

				//ObjectAlignment
				case 214:
					{
						int nIndex = (int)value;
						ApplyObjectAlignmentEvent (CurrentSelectedAtoms, nIndex, ownerViewType);
						break;
					}

				//ShowHide
				case 216:
					{
						bool bIsHide = (bool)value;
						ApplyShowHide (CurrentSelectedAtoms, bIsHide);
						break;
					}

				//No Line
				case 251:
					{
						ApplyNoLineEvent (CurrentSelectedAtoms, (bool)value);
						break;
					}

				//Line Thickness
				case 252:
					{
						ApplyLineThicknessEvent (CurrentSelectedAtoms, Convert.ToDouble (value));
						break;
					}

				//Line Type
				case 253:
					{
						ApplyDashArrayEvent (CurrentSelectedAtoms, (DoubleCollection)value);
						break;
					}

				//No Background
				case 254:
					{
						ApplyNoBackgroundEvent (CurrentSelectedAtoms, (bool)value);
						break;
					}

				//BorderVisibilityType(Grid Table)
				case 255:
					{
						ApplyBorderVisibilityType (CurrentSelectedAtoms, (int)value);
						break;
					}

				// ColorDialogEventKey.OPACITY
				case 256:
					{
						ApplyAtomOpacityEvent (CurrentSelectedAtoms, (int)value);
						break;
					}

				// ColorDialogEventKey.SHADOW
				case 257:
					{
						ApplyAtomShadowEvent (CurrentSelectedAtoms, (bool)value);
						break;
					}

				default: break;
			}
		}

		private void ApplyFontFamilyEvent (List<AtomBase> selectedAtoms, FontFamily font)
		{
			List<ApplyFontFamilyCommand> lstApplyFontFamilyCommands = new List<ApplyFontFamilyCommand> ();

			foreach (AtomBase atom in selectedAtoms)
			{
				ApplyFontFamilyCommand NewApplyFontFamilyCommand = new ApplyFontFamilyCommand (atom, atom.FontFamily, font);
				lstApplyFontFamilyCommands.Add (NewApplyFontFamilyCommand);
			}

			ApplyFontFamiliesCommand NewApplyFontFamiliesCommand = new ApplyFontFamiliesCommand (lstApplyFontFamilyCommands);
			Commander.AddCommand (NewApplyFontFamiliesCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyFontSizeEvent (List<AtomBase> selectedAtoms, object objFontSize)
		{
			Type ParameterType = objFontSize.GetType ();

			List<ApplyFontSizeCommand> lstApplyFontSizeCommands = new List<ApplyFontSizeCommand> ();

			double dCurrentFontSize = 0D;

			if (typeof (ActionDefine.FontSizeUpDownType) == ParameterType)
			{
				if (ActionDefine.FontSizeUpDownType.Up == (ActionDefine.FontSizeUpDownType)objFontSize)
				{
					foreach (AtomBase atom in selectedAtoms)
					{
						dCurrentFontSize = atom.GetAtomFontSize ();
						if (dCurrentFontSize < 200)
						{
							dCurrentFontSize += 1;

							ApplyFontSizeCommand NewApplyFontSizeCommand = new ApplyFontSizeCommand (atom, atom.FontSize, dCurrentFontSize, ActionDefine.FontSizeUpDownType.Up);
							lstApplyFontSizeCommands.Add (NewApplyFontSizeCommand);

							//Kiho : 2016-11.29 : 여기서는 툴바의 폰트 크기를 업데이트 하는게 다다.
							GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyFontSizeChanged (dCurrentFontSize);
						}
					}
				}
				else if (ActionDefine.FontSizeUpDownType.Down == (ActionDefine.FontSizeUpDownType)objFontSize)
				{
					foreach (AtomBase atom in selectedAtoms)
					{
						dCurrentFontSize = atom.GetAtomFontSize ();
						if (1 < dCurrentFontSize)
						{
							dCurrentFontSize -= 1;

							ApplyFontSizeCommand NewApplyFontSizeCommand = new ApplyFontSizeCommand (atom, atom.FontSize, dCurrentFontSize, ActionDefine.FontSizeUpDownType.Down);
							lstApplyFontSizeCommands.Add (NewApplyFontSizeCommand);

							//Kiho : 2016-11.29 : 여기서는 툴바의 폰트 크기를 업데이트 하는게 다다.
							GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyFontSizeChanged (dCurrentFontSize);
						}
					}
				}
			}
			else
			{
				int nFontSize = Convert.ToInt32 (objFontSize);

				if (0 < nFontSize && nFontSize <= 200)
				{
					foreach (AtomBase atom in selectedAtoms)
					{
						ApplyFontSizeCommand NewApplyFontSizeCommand = new ApplyFontSizeCommand (atom, atom.FontSize, nFontSize, ActionDefine.FontSizeUpDownType.None);
						lstApplyFontSizeCommands.Add (NewApplyFontSizeCommand);
						//atom.SetAtomFontSize((double)nFontSize);
					}
				}
			}

			//Kiho : 2016-11-29 : 텍스트편집기 아톰의 폰트 크기 크게 / 작게 이벤트를 호출하게 된다.
			ApplyFontSizesCommand NewApplyFontSizesCommand = new ApplyFontSizesCommand (lstApplyFontSizeCommands);
			Commander.AddCommand (NewApplyFontSizesCommand);
			Commander.ExecuteCommand ();

			//Kiho : 2016-11.29 : 여기서는 툴바의 폰트 크기를 업데이트 하는게 다다.
			GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyFontSizeChanged (dCurrentFontSize);
		}

		private void ApplyFontWeightEvent (List<AtomBase> selectedAtoms, bool bIsApply)
		{
			List<ApplyFontWeightCommand> lstApplyFontWeightCommands = new List<ApplyFontWeightCommand> ();

			if (true == bIsApply)
			{
				foreach (AtomBase atom in selectedAtoms)
				{
					ApplyFontWeightCommand NewApplyFontWeightCommand = new ApplyFontWeightCommand (atom, atom.FontWeight, FontWeights.Bold);
					lstApplyFontWeightCommands.Add (NewApplyFontWeightCommand);
				}
			}
			else
			{
				foreach (AtomBase atom in selectedAtoms)
				{
					ApplyFontWeightCommand NewApplyFontWeightCommand = new ApplyFontWeightCommand (atom, atom.FontWeight, FontWeights.Normal);
					lstApplyFontWeightCommands.Add (NewApplyFontWeightCommand);
				}
			}

			ApplyFontWeightsCommand NewApplyFontWeightsCommand = new ApplyFontWeightsCommand (lstApplyFontWeightCommands);
			Commander.AddCommand (NewApplyFontWeightsCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyFontUnderLineEvent (List<AtomBase> selectedAtoms, bool bIsApply)
		{
			List<ApplyFontUnderlineCommand> lstApplyFontUnderlineCommands = new List<ApplyFontUnderlineCommand> ();

			if (true == bIsApply)
			{
				foreach (AtomBase atom in selectedAtoms)
				{
					ApplyFontUnderlineCommand NewApplyFontUnderlineCommand = new ApplyFontUnderlineCommand (atom, TextDecorationLocation.Baseline, TextDecorationLocation.Underline);
					lstApplyFontUnderlineCommands.Add (NewApplyFontUnderlineCommand);
				}
			}
			else
			{
				foreach (AtomBase atom in selectedAtoms)
				{
					ApplyFontUnderlineCommand NewApplyFontUnderlineCommand = new ApplyFontUnderlineCommand (atom, TextDecorationLocation.Underline, TextDecorationLocation.Baseline);
					lstApplyFontUnderlineCommands.Add (NewApplyFontUnderlineCommand);
				}
			}

			ApplyFontUnderlinesCommand NewApplyFontUnderlinesCommand = new ApplyFontUnderlinesCommand (lstApplyFontUnderlineCommands);
			Commander.AddCommand (NewApplyFontUnderlinesCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyFontStyleEvent (List<AtomBase> selectedAtoms, bool bIsApply)
		{
			List<ApplyFontStyleCommand> lstApplyFontStyleCommands = new List<ApplyFontStyleCommand> ();

			if (true == bIsApply)
			{
				foreach (AtomBase atom in selectedAtoms)
				{
					ApplyFontStyleCommand NewApplyFontStyleCommand = new ApplyFontStyleCommand (atom, atom.FontStyle, FontStyles.Italic);
					lstApplyFontStyleCommands.Add (NewApplyFontStyleCommand);
				}
			}
			else
			{
				foreach (AtomBase atom in selectedAtoms)
				{
					ApplyFontStyleCommand NewApplyFontStyleCommand = new ApplyFontStyleCommand (atom, atom.FontStyle, FontStyles.Normal);
					lstApplyFontStyleCommands.Add (NewApplyFontStyleCommand);
				}
			}

			ApplyFontStylesCommand NewApplyFontStylesCommand = new ApplyFontStylesCommand (lstApplyFontStyleCommands);
			Commander.AddCommand (NewApplyFontStylesCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyFontColorEvent (List<AtomBase> selectedAtoms, Brush applyBrush)
		{
			List<ApplyFontColorCommand> lstApplyFontColorCommands = new List<ApplyFontColorCommand> ();

			foreach (AtomBase atom in selectedAtoms)
			{
				Brush CurrentAtomForeground = atom.GetAtomFontColor ();

				if (null != CurrentAtomForeground)
				{
					CurrentAtomForeground = CurrentAtomForeground.Clone ();
				}

				Brush ClonedApplyBrush = applyBrush;

				if (null != ClonedApplyBrush)
				{
					ClonedApplyBrush = ClonedApplyBrush.Clone ();
				}

				ApplyFontColorCommand NewApplyFontColorCommand = new ApplyFontColorCommand (atom, CurrentAtomForeground, ClonedApplyBrush);
				lstApplyFontColorCommands.Add (NewApplyFontColorCommand);
			}

			ApplyFontColorsCommand NewApplyFontColorsCommand = new ApplyFontColorsCommand (lstApplyFontColorCommands);
			Commander.AddCommand (NewApplyFontColorsCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyHorizontalTextAlignmentEvent (List<AtomBase> selectedAtoms, HorizontalAlignment alignmentType)
		{
			List<ApplyHorizontalTextAlignmentCommand> lstApplyHorizontalTextAlignmentCommands = new List<ApplyHorizontalTextAlignmentCommand> ();

			foreach (AtomBase atom in selectedAtoms)
			{
				ApplyHorizontalTextAlignmentCommand NewApplyHorizontalTextAlignmentCommand = new ApplyHorizontalTextAlignmentCommand (atom, atom.GetHorizontalTextAlignment (), alignmentType);
				lstApplyHorizontalTextAlignmentCommands.Add (NewApplyHorizontalTextAlignmentCommand);
			}

			ApplyHorizontalTextAlignmentsCommand NewApplyHorizontalTextAlignmentsCommand = new ApplyHorizontalTextAlignmentsCommand (lstApplyHorizontalTextAlignmentCommands);
			Commander.AddCommand (NewApplyHorizontalTextAlignmentsCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyVerticalTextAlignmentEvent (List<AtomBase> selectedAtoms, VerticalAlignment alignmentType)
		{
			List<ApplyVerticalTextAlignmentCommand> lstApplyVerticalTextAlignmentCommands = new List<ApplyVerticalTextAlignmentCommand> ();

			foreach (AtomBase atom in selectedAtoms)
			{
				ApplyVerticalTextAlignmentCommand NewApplyVerticalTextAlignmentCommand = new ApplyVerticalTextAlignmentCommand (atom, atom.GetVerticalTextAlignment (), alignmentType);
				lstApplyVerticalTextAlignmentCommands.Add (NewApplyVerticalTextAlignmentCommand);
			}

			ApplyVerticalTextAlignmentsCommand NewApplyVerticalTextAlignmentsCommand = new ApplyVerticalTextAlignmentsCommand (lstApplyVerticalTextAlignmentCommands);
			Commander.AddCommand (NewApplyVerticalTextAlignmentsCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyLineColorEvent (List<AtomBase> selectedAtoms, Brush applyBrush)
		{
			List<ApplyLineColorCommand> lstApplyLineColorCommands = new List<ApplyLineColorCommand> ();

			foreach (AtomBase atom in selectedAtoms)
			{
				Brush CurrentAtomBorder = atom.GetAtomBorder ();

				if (null != CurrentAtomBorder)
				{
					CurrentAtomBorder = CurrentAtomBorder.Clone ();
				}

				Brush ClonedApplyBrush = applyBrush;

				if (null != ClonedApplyBrush)
				{
					ClonedApplyBrush = ClonedApplyBrush.Clone ();
				}

				if (0 >= atom.GetAtomThickness ().Left)
				{
					new ApplyNoLineCommand (atom, true, atom.PreBorderThicknes).Execute ();
				}

				ApplyLineColorCommand NewApplyLineColorCommand = new ApplyLineColorCommand (atom, CurrentAtomBorder, ClonedApplyBrush);
				lstApplyLineColorCommands.Add (NewApplyLineColorCommand);
			}

			ApplyLineColorsCommand NewApplyLineColorsCommand = new ApplyLineColorsCommand (lstApplyLineColorCommands);
			Commander.AddCommand (NewApplyLineColorsCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyBackgroundColorEvent (List<AtomBase> selectedAtoms, object objValue)
		{
			List<ApplyBackgroundCommand> lstApplyBackgroundCommands = new List<ApplyBackgroundCommand> ();

			if (objValue is Brush)
			{
				Brush ApplyBrush = objValue as Brush;

				foreach (AtomBase atom in selectedAtoms)
				{
					Brush CurrentAtomBackground = atom.GetAtomBackground ();

					if (null != CurrentAtomBackground)
					{
						CurrentAtomBackground = CurrentAtomBackground.Clone ();
					}

					if (null != ApplyBrush)
					{
						ApplyBrush = ApplyBrush.Clone ();
					}

					ApplyBackgroundCommand NewApplyBackgroundCommand = new ApplyBackgroundCommand (atom, CurrentAtomBackground, ApplyBrush);
					lstApplyBackgroundCommands.Add (NewApplyBackgroundCommand);
				}
			}
			else if (objValue is BitmapImage)
			{
				BitmapImage ConvertedBitmapImage = objValue as BitmapImage;
				ImageBrush ApplyImage = new ImageBrush (ConvertedBitmapImage);

				foreach (AtomBase atom in selectedAtoms)
				{
					Brush CurrentAtomBackground = atom.GetAtomBackground ();

					if (null != CurrentAtomBackground)
					{
						CurrentAtomBackground = CurrentAtomBackground.Clone ();
					}

					if (null != ApplyImage)
					{
						ApplyImage = ApplyImage.Clone ();
					}

					ApplyBackgroundCommand NewApplyBackgroundCommand = new ApplyBackgroundCommand (atom, CurrentAtomBackground, ApplyImage);
					lstApplyBackgroundCommands.Add (NewApplyBackgroundCommand);
				}
			}

			ApplyBackgroundsCommand NewApplyBackgroundsCommand = new ApplyBackgroundsCommand (lstApplyBackgroundCommands);
			Commander.AddCommand (NewApplyBackgroundsCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyNoLineEvent (List<AtomBase> selectedAtoms, bool bIsNoLine)
		{
			List<ApplyNoLineCommand> lstApplyNoLineCommands = new List<ApplyNoLineCommand> ();

			if (true == bIsNoLine)
			{
				foreach (AtomBase atom in selectedAtoms)
				{
					if (null != atom.AtomCore && null != atom.AtomCore.GetAttrib ())
					{
						ApplyNoLineCommand NewApplyNoLineCommand = new ApplyNoLineCommand (atom, false, atom.GetAtomThickness ());
						lstApplyNoLineCommands.Add (NewApplyNoLineCommand);
					}
				}
			}
			else
			{
				foreach (AtomBase atom in selectedAtoms)
				{
					if (null != atom.AtomCore && null != atom.AtomCore.GetAttrib ())
					{
						ApplyNoLineCommand NewApplyNoLineCommand = new ApplyNoLineCommand (atom, true, atom.PreBorderThicknes);
						lstApplyNoLineCommands.Add (NewApplyNoLineCommand);
					}
				}
			}

			ApplyNoLinesCommand NewApplyNoLinesCommand = new ApplyNoLinesCommand (lstApplyNoLineCommands);
			Commander.AddCommand (NewApplyNoLinesCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyLineThicknessEvent (List<AtomBase> selectedAtoms, double dThickness)
		{
			List<ApplyLineThicknessCommand> lstApplyLineThicknessCommands = new List<ApplyLineThicknessCommand> ();

			foreach (AtomBase atom in selectedAtoms)
			{
				if (null != atom.AtomCore && null != atom.AtomCore.GetAttrib ())
				{
					ApplyLineThicknessCommand NewApplyLineThicknessCommand = new ApplyLineThicknessCommand (atom, atom.GetAtomThickness ().Left, dThickness);
					lstApplyLineThicknessCommands.Add (NewApplyLineThicknessCommand);
				}
			}

			ApplyLineThicknessesCommand NewApplyLineThicknessesCommand = new ApplyLineThicknessesCommand (lstApplyLineThicknessCommands);
			Commander.AddCommand (NewApplyLineThicknessesCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyDashArrayEvent (List<AtomBase> selectedAtoms, DoubleCollection dashArray)
		{
			List<ApplyDashArrayCommand> lstApplyDashArrayCommands = new List<ApplyDashArrayCommand> ();

			foreach (AtomBase atom in selectedAtoms)
			{
				DoubleCollection CurrentDashArray = atom.GetAtomDashArray ();
				DoubleCollection ClonedCurrentDashArray = null;
				DoubleCollection ApplyDashArray = null;

				if (null != CurrentDashArray)
				{
					ClonedCurrentDashArray = CurrentDashArray.Clone ();
				}

				if (null != dashArray)
				{
					ApplyDashArray = dashArray.Clone ();
				}

				ApplyDashArrayCommand NewApplyDashArrayCommand = new ApplyDashArrayCommand (atom, ClonedCurrentDashArray, ApplyDashArray);
				lstApplyDashArrayCommands.Add (NewApplyDashArrayCommand);
			}

			ApplyDashArraysCommand NewApplyDashArraysCommand = new ApplyDashArraysCommand (lstApplyDashArrayCommands);
			Commander.AddCommand (NewApplyDashArraysCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyNoBackgroundEvent (List<AtomBase> selectedAtoms, bool bIsNoBackground)
		{
			List<ApplyNoBackgroundCommand> lstApplyNoBackgroundCommands = new List<ApplyNoBackgroundCommand> ();

			if (true == bIsNoBackground)
			{
				foreach (AtomBase atom in selectedAtoms)
				{
					Brush CurrentAtomBackground = atom.GetAtomBackground ();

					if (null != CurrentAtomBackground)
					{
						CurrentAtomBackground = CurrentAtomBackground.Clone ();
					}

					ApplyNoBackgroundCommand NewApplyNoBackgroundCommand = new ApplyNoBackgroundCommand (atom, bIsNoBackground, CurrentAtomBackground);
					lstApplyNoBackgroundCommands.Add (NewApplyNoBackgroundCommand);
				}
			}
			else
			{
				foreach (AtomBase atom in selectedAtoms)
				{
					Brush CurrentAtomPreBackground = atom.PreBackground;

					if (null != CurrentAtomPreBackground)
					{
						CurrentAtomPreBackground = CurrentAtomPreBackground.Clone ();
					}

					ApplyNoBackgroundCommand NewApplyNoBackgroundCommand = new ApplyNoBackgroundCommand (atom, bIsNoBackground, CurrentAtomPreBackground);
					lstApplyNoBackgroundCommands.Add (NewApplyNoBackgroundCommand);
				}
			}

			ApplyNoBackgroundsCommand NewApplyNoBackgroundsCommand = new ApplyNoBackgroundsCommand (lstApplyNoBackgroundCommands);
			Commander.AddCommand (NewApplyNoBackgroundsCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyBorderVisibilityType (List<AtomBase> selectedAtoms, int nBorderVisibilityType)
		{
			if (null != selectedAtoms)
			{
				int nCountOfSelectedAtoms = selectedAtoms.Count;

				if (1 == nCountOfSelectedAtoms)
				{
					AtomBase TargetAtom = selectedAtoms[0];

					if (TargetAtom is GridTableOfAtom)
					{
						GridTableOfAtom GridTableAtom = TargetAtom as GridTableOfAtom;
						GridTableAtom.ApplyBorderVisibilityType (nBorderVisibilityType);
					}
					else if (TargetAtom is GridTableExofAtom)
					{
						GridTableExofAtom gridTableAtom = TargetAtom as GridTableExofAtom;
						gridTableAtom.ApplyBorderVisibilityType (nBorderVisibilityType);
					}
				}
			}
		}

		private void ApplyAtomOpacityEvent (List<AtomBase> selectedAtoms, int nAtomOpacity)
		{
			List<ApplyOpacityCommand> lstApplyOpacityCommands = new List<ApplyOpacityCommand> ();

			foreach (AtomBase atom in selectedAtoms)
			{
				int nCurrentAtomOpacity = atom.GetAtomOpacity ();

				ApplyOpacityCommand NewApplyOpacityCommand = new ApplyOpacityCommand (atom, nCurrentAtomOpacity, nAtomOpacity);
				lstApplyOpacityCommands.Add (NewApplyOpacityCommand);
			}

			ApplyOpacitysCommand NewApplyOpacitysCommand = new ApplyOpacitysCommand (lstApplyOpacityCommands);
			Commander.AddCommand (NewApplyOpacitysCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyAtomShadowEvent (List<AtomBase> selectedAtoms, bool bShadow)
		{
			List<ApplyShadowCommand> lstApplyShadowCommands = new List<ApplyShadowCommand> ();

			foreach (AtomBase atom in selectedAtoms)
			{
				bool bCurrentAtomShadow = atom.AtomCore.Attrib.IsShadow;

				ApplyShadowCommand NewApplyShadowCommand = new ApplyShadowCommand (atom, bCurrentAtomShadow, bShadow);
				lstApplyShadowCommands.Add (NewApplyShadowCommand);
			}

			ApplyShadowsCommand NewApplyShadowsCommand = new ApplyShadowsCommand (lstApplyShadowCommands);
			Commander.AddCommand (NewApplyShadowsCommand);
			Commander.ExecuteCommand ();
		}

		private void ApplyShowHide (List<AtomBase> selectedAtoms, bool bIsHide)
		{
			foreach (AtomBase CurrentAtom in selectedAtoms)
			{
				CurrentAtom.AtomCore.Attrib.IsAtomHidden = bIsHide;
			}
		}

		private void ApplyObjectAlignmentEvent (List<AtomBase> selectedAtoms, int nIndex, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			switch (nIndex)
			{
				case 0: ProcessObjectLeftAlignment (selectedAtoms, ownerViewType); break;
				case 1: ProcessObjectRightAlignment (selectedAtoms, ownerViewType); break;
				case 2: ProcessObjectTopAlignment (selectedAtoms, ownerViewType); break;
				case 3: ProcessObjectBottomAlignment (selectedAtoms, ownerViewType); break;
				case 4: ProcessObjectHorizontalAlignment (selectedAtoms, ownerViewType); break;
				case 5: ProcessObjectVerticalAlignment (selectedAtoms, ownerViewType); break;
				case 6: ProcessObjectHorizontalArrangement (selectedAtoms, ownerViewType); break;
				case 7: ProcessObjectVerticalArrangement (selectedAtoms, ownerViewType); break;
				case 8: ProcessObjectHorizontalIntervalArrangement (selectedAtoms, ownerViewType); break;
				case 9: ProcessObjectVerticalIntervalArrangement (selectedAtoms, ownerViewType); break;
				case 10: ProcessObjectHorizontalSizeDivide (selectedAtoms, ownerViewType); break;
				case 11: ProcessObjectVerticalSizeDivide (selectedAtoms, ownerViewType); break;
				case 12: ProcessObjectSizeDivide (selectedAtoms, ownerViewType); break;
				default: break;
			}

			//2019-09-18 kys 아톰정렬 이후에 undo기능을 정상적으로 동작하기 위해서
			List<MoveAtomCommand> lstMoveAtomCommands = new List<MoveAtomCommand> ();

			foreach (AtomBase atom in selectedAtoms)
			{
				MoveAtomCommand newMoveAtomCommand = new MoveAtomCommand (atom, atom.Margin.Left, atom.Margin.Top, atom.MoveStartLeft, atom.MoveStartTop);
				lstMoveAtomCommands.Add (newMoveAtomCommand);

				if (atom is EBookAnimationGroupofAtom)
				{
					EBookAnimationGroupAtom groupAtom = atom.AtomCore as EBookAnimationGroupAtom;
					List<AtomBase> lstGroupAtoms = groupAtom.GetGroupAtoms ();
					foreach (AtomBase atomBase in lstGroupAtoms)
					{
						Thickness destMargin = atomBase.Margin;

						destMargin.Left += (atom.Margin.Left - atom.MoveStartLeft);
						destMargin.Top += (atom.Margin.Top - atom.MoveStartTop);

						atomBase.MoveStartLeft = atomBase.Margin.Left;
						atomBase.MoveStartTop = atomBase.Margin.Top;

						MoveAtomCommand newGroupAtomCommand = new MoveAtomCommand (atomBase, destMargin.Left, destMargin.Top, atomBase.MoveStartLeft, atomBase.MoveStartTop);

						lstMoveAtomCommands.Add (newGroupAtomCommand);
					}
				}
			}

			CommandCenter.CommandCommander commander = this.Commander;
			MoveGroupedAtomsCommand newMoveGroupedAtomsCommand = new MoveGroupedAtomsCommand (lstMoveAtomCommands);
			commander.AddCommand (newMoveGroupedAtomsCommand);
			commander.ExecuteCommand ();
		}

		/// <summary>
		/// 좌측정렬
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectLeftAlignment (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (1 < selectedAtoms.Count)
			{
				//AtomBase LastestPressedAtom = GetLatestPressedAtom();
				//double dStandardX = LastestPressedAtom.Margin.Left;
				//2020-01-09 kys 좌측정렬 마지막 아톰을 기준으로 정렬하던 논리에서 좌측 좌표값이 가장 작은 아톰을 기준으로 정렬하도록 논리 수정함
				double dStandardX = double.MaxValue;

				foreach (AtomBase atom in selectedAtoms)
				{
					if (dStandardX > atom.Margin.Left)
						dStandardX = atom.Margin.Left;
				}

				Size CurrentViewSize = GetViewSize ();

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness currentMargin = atom.Margin;
					double dCurrentWidth = atom.Width;
					double dLimitEndX = (dStandardX + dCurrentWidth);

					if (dLimitEndX < CurrentViewSize.Width)
					{
						atom.Margin = new Thickness (dStandardX, currentMargin.Top, 0, 0);
					}
				}

			}
			else if (1 == selectedAtoms.Count)
			{
				Size ViewSize = this.GetFrameAttrib ().FrameSize;
				AtomBase atom = selectedAtoms[0];

				Thickness currentMargin = atom.Margin;

				currentMargin.Left = 0;
				atom.Margin = currentMargin;
			}
		}

		/// <summary>
		/// 우측정렬
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectRightAlignment (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (1 < selectedAtoms.Count)
			{
				//AtomBase LastestPressedAtom = GetLatestPressedAtom();
				//(LastestPressedAtom.Margin.Left + LastestPressedAtom.Width);

				//2020-01-09 kys 우측정렬일때 기존에는 마지막에 선택된 아톰을 기준으로 정렬논리에서 
				//우측값이 가장 큰 아톰을 기준으로 정렬하도록 논리 수정함 -> tabview, scroll과 상관없이 적용되서 별도의 분기처리가 필요 없음
				double dStandardX = 0;

				foreach (AtomBase atom in selectedAtoms)
				{
					double dTempX = atom.Margin.Left + atom.Width;
					if (dStandardX < dTempX)
						dStandardX = dTempX;
				}

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness currentMargin = atom.Margin;
					double dCurrentWidth = atom.Width;
					double dCurrentEndX = (currentMargin.Left + dCurrentWidth);

					if (dStandardX >= dCurrentEndX)
					{
						double dOffSetX = dStandardX - dCurrentEndX;
						atom.Margin = new Thickness ((currentMargin.Left + dOffSetX), currentMargin.Top, 0, 0);
					}
					else
					{
						double dOffSetX = dCurrentEndX - dStandardX;
						double dResultLeft = (currentMargin.Left - dOffSetX);

						if (0 < dResultLeft)
						{
							atom.Margin = new Thickness (dResultLeft, currentMargin.Top, 0, 0);
						}
					}
				}
			}
			else if (1 == selectedAtoms.Count)
			{
				Size ViewSize = this.GetFrameAttrib ().FrameSize;
				AtomBase atom = selectedAtoms[0];

				Thickness currentMargin = atom.Margin;

				currentMargin.Left = ViewSize.Width - atom.Width;
				atom.Margin = currentMargin;
			}
		}

		/// <summary>
		/// 상단정렬
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectTopAlignment (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (1 < selectedAtoms.Count)
			{
				//AtomBase LastestPressedAtom = GetLatestPressedAtom();
				//double dStandardY = (LastestPressedAtom.Margin.Top);
				//2020-01-09 kys 상단정렬 마지막 아톰을 기준으로 정렬하는 논리에서 높이값이 가장 작은 아톰을 기준으로 정렬하도록 수정함
				double dStandardY = double.MaxValue;

				foreach (AtomBase atom in selectedAtoms)
				{
					if (dStandardY > atom.Margin.Top)
						dStandardY = atom.Margin.Top;
				}

				Size CurrentViewSize = GetViewSize (); //scroll이나 tabview일때 확인 필요함

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness currentMargin = atom.Margin;
					double dCurrentHeight = atom.Height;
					double dLimitEndY = (dStandardY + dCurrentHeight);

					if (dLimitEndY < CurrentViewSize.Height)
					{
						atom.Margin = new Thickness (currentMargin.Left, dStandardY, 0, 0);
					}
				}
			}
			else if (1 == selectedAtoms.Count)
			{
				Size ViewSize = this.GetFrameAttrib ().FrameSize;
				AtomBase atom = selectedAtoms[0];

				Thickness currentMargin = atom.Margin;

				currentMargin.Top = 0;
				atom.Margin = currentMargin;
			}
		}

		/// <summary>
		/// 하단정렬
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectBottomAlignment (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (1 < selectedAtoms.Count)
			{
				//AtomBase LastestPressedAtom = GetLatestPressedAtom();
				//double dStandardY = (LastestPressedAtom.Margin.Top + LastestPressedAtom.Height);

				double dStandardY = double.MinValue;

				foreach (AtomBase atom in selectedAtoms)
				{
					double dTemp = atom.Margin.Top + atom.Height;
					if (dStandardY < dTemp)
						dStandardY = dTemp;
				}

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness currentMargin = atom.Margin;
					double dCurrentHeight = atom.Height;
					double dCurrentEndY = (currentMargin.Top + dCurrentHeight);

					if (dStandardY >= dCurrentEndY)
					{
						double dOffSetY = dStandardY - dCurrentEndY;
						atom.Margin = new Thickness (currentMargin.Left, (currentMargin.Top + dOffSetY), 0, 0);
					}
					else
					{
						double dOffSetY = dCurrentEndY - dStandardY;
						double dResultLeft = (currentMargin.Top - dOffSetY);

						if (0 < dResultLeft)
						{
							atom.Margin = new Thickness (currentMargin.Left, dResultLeft, 0, 0);
						}
					}
				}

			}
			else if (1 == selectedAtoms.Count)
			{
				Size ViewSize = this.GetFrameAttrib ().FrameSize;
				AtomBase atom = selectedAtoms[0];

				Thickness currentMargin = atom.Margin;

				currentMargin.Top = ViewSize.Height - atom.Height;
				atom.Margin = currentMargin;
			}
		}

		/// <summary>
		/// 수평중앙정렬
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectHorizontalAlignment (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (1 < selectedAtoms.Count)
			{
				double dMin = double.MaxValue;
				double dMax = double.MinValue;

				foreach (AtomBase atom in selectedAtoms)
				{
					if (dMin > atom.Margin.Top)
					{
						dMin = atom.Margin.Top;
					}

					if (atom.Margin.Top + atom.Height > dMax)
					{
						dMax = atom.Margin.Top + atom.Height;
					}
				}

				double dStandardY = dMin + ((dMax - dMin) / 2);

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness currentMargin = atom.Margin;

					currentMargin.Top = dStandardY - (atom.Height / 2);

					atom.Margin = currentMargin;
				}
			}
			else if (1 == selectedAtoms.Count)
			{
				Size ViewSize = this.GetFrameAttrib ().FrameSize;
				AtomBase atom = selectedAtoms[0];

				Thickness currentMargin = atom.Margin;

				currentMargin.Top = (ViewSize.Height / 2) - (atom.Height / 2);
				atom.Margin = currentMargin;
			}
		}

		/// <summary>
		/// 수직중앙정렬
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectVerticalAlignment (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (1 < selectedAtoms.Count)
			{
				double dMin = double.MaxValue;
				double dMax = double.MinValue;

				foreach (AtomBase atom in selectedAtoms)
				{
					if (dMin > atom.Margin.Left)
					{
						dMin = atom.Margin.Left;
					}

					if (atom.Margin.Left + atom.Width > dMax)
					{
						dMax = atom.Margin.Left + atom.Width;
					}
				}

				double dStandard = dMin + ((dMax - dMin) / 2);

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness currentMargin = atom.Margin;

					currentMargin.Left = dStandard - (atom.Width / 2);

					atom.Margin = currentMargin;
				}
			}
			else if (1 == selectedAtoms.Count)
			{
				Size ViewSize = this.GetFrameAttrib ().FrameSize;
				AtomBase atom = selectedAtoms[0];

				Thickness currentMargin = atom.Margin;

				currentMargin.Left = (ViewSize.Width / 2) - (atom.Width / 2);
				atom.Margin = currentMargin;
			}
		}

		/// <summary>
		/// 수평중앙배열
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectHorizontalArrangement (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (0 < selectedAtoms.Count)
			{
				if (OwnerViewDefine.OwnerViewType.View == ownerViewType)
				{
					Size CurrentViewSize = GetViewSize ();
					double dHalfViewWidth = (CurrentViewSize.Width / 2);

					foreach (AtomBase atom in selectedAtoms)
					{
						Thickness currentMargin = atom.Margin;
						double dNewLeft = dHalfViewWidth - (atom.Width / 2);
						atom.Margin = new Thickness (dNewLeft, currentMargin.Top, 0, 0);
					}
				}
				else if (OwnerViewDefine.OwnerViewType.TabView == ownerViewType)
				{
					AtomBase firstAtom = selectedAtoms[0] as AtomBase;

					if (null != firstAtom)
					{
						FrameworkElement TabViewElement = firstAtom.GetTabViewAtom ();

						if (null != TabViewElement)
						{
							TabViewAtomBase TabViewAtom = TabViewElement as TabViewAtomBase;
							Size TabPageSize = TabViewAtom.GetTabPageSize ();
							double dHalfTabPageWidth = (TabPageSize.Width / 2);

							foreach (AtomBase atom in selectedAtoms)
							{
								Thickness currentMargin = atom.Margin;
								double dNewLeft = dHalfTabPageWidth - (atom.Width / 2);
								atom.Margin = new Thickness (dNewLeft, currentMargin.Top, 0, 0);
							}
						}
					}
				}
				else if (OwnerViewDefine.OwnerViewType.Scroll == ownerViewType)
				{
					AtomBase firstAtom = selectedAtoms[0] as AtomBase;

					if (null != firstAtom)
					{
						FrameworkElement ScrollElement = firstAtom.GetScrollAtom ();

						if (null != ScrollElement)
						{
							ScrollAtomBase ScrollAtom = ScrollElement as ScrollAtomBase;
							double dHalfScrollAtomWidth = (ScrollAtom.Width / 2);

							foreach (AtomBase atom in selectedAtoms)
							{
								Thickness currentMargin = atom.Margin;
								double dNewLeft = dHalfScrollAtomWidth - (atom.Width / 2);
								atom.Margin = new Thickness (dNewLeft, currentMargin.Top, 0, 0);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 수직중앙배열
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectVerticalArrangement (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (0 < selectedAtoms.Count)
			{
				if (OwnerViewDefine.OwnerViewType.View == ownerViewType)
				{
					Size CurrentViewSize = GetViewSize ();
					double dHalfViewHeight = (CurrentViewSize.Height / 2);

					foreach (AtomBase atom in selectedAtoms)
					{
						Thickness currentMargin = atom.Margin;
						double dNewTop = dHalfViewHeight - (atom.Height / 2);
						atom.Margin = new Thickness (currentMargin.Left, dNewTop, 0, 0);
					}
				}
				else if (OwnerViewDefine.OwnerViewType.TabView == ownerViewType)
				{
					AtomBase firstAtom = selectedAtoms[0] as AtomBase;

					if (null != firstAtom)
					{
						FrameworkElement TabViewElement = firstAtom.GetTabViewAtom ();

						if (null != TabViewElement)
						{
							TabViewAtomBase TabViewAtom = TabViewElement as TabViewAtomBase;
							Size TabPageSize = TabViewAtom.GetTabPageSize ();
							double dHalfTabPageHeight = (TabPageSize.Height / 2);

							foreach (AtomBase atom in selectedAtoms)
							{
								Thickness currentMargin = atom.Margin;
								double dNewTop = dHalfTabPageHeight - (atom.Height / 2);
								atom.Margin = new Thickness (currentMargin.Left, dNewTop, 0, 0);
							}
						}
					}
				}
				else if (OwnerViewDefine.OwnerViewType.Scroll == ownerViewType)
				{
					AtomBase firstAtom = selectedAtoms[0] as AtomBase;

					if (null != firstAtom)
					{
						FrameworkElement ScrollElement = firstAtom.GetScrollAtom ();

						if (null != ScrollElement)
						{
							ScrollAtomBase ScrollAtom = ScrollElement as ScrollAtomBase;
							double dHalfScrollAtomHeight = (ScrollAtom.Height / 2);

							foreach (AtomBase atom in selectedAtoms)
							{
								Thickness currentMargin = atom.Margin;
								double dNewTop = dHalfScrollAtomHeight - (atom.Height / 2);
								atom.Margin = new Thickness (currentMargin.Left, dNewTop, 0, 0);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 수평간격배열
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectHorizontalIntervalArrangement (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (2 < selectedAtoms.Count)
			{
				double dFirstElementX = 99999;
				double dFirstElementEndX = -1;
				double dLastElementX = 99999;
				double dLastElementEndX = -1;
				double dTotalWidthOfAtoms = 0;
				AtomBase firstAtom = null;
				AtomBase lastAtom = null;

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness currentMargin = atom.Margin;
					double nCurrentX = (currentMargin.Left);
					double nCurrentEndX = (currentMargin.Left + atom.Width);

					if (dFirstElementX > nCurrentX)
					{
						firstAtom = atom;
						dFirstElementX = nCurrentX;
						dFirstElementEndX = nCurrentEndX;
					}

					if (dLastElementEndX < nCurrentEndX)
					{
						lastAtom = atom;
						dLastElementEndX = nCurrentEndX;
						dLastElementX = nCurrentX;
					}

					dTotalWidthOfAtoms += (atom.Width);
				}

				if (null == firstAtom || null == lastAtom)
				{
					return;
				}
				else
				{
					double dOffsetOfFirstAndLast = (dLastElementEndX - dFirstElementX);

					if (dTotalWidthOfAtoms < dOffsetOfFirstAndLast)
					{
						double dRemainder = (dOffsetOfFirstAndLast - dTotalWidthOfAtoms);
						double dInterval = (dRemainder / (selectedAtoms.Count - 1));
						double dNewAtomLeft = dFirstElementEndX;

						selectedAtoms.Sort (delegate (AtomBase x, AtomBase y)
						{
							if (x.Margin.Left > y.Margin.Left) return 1;
							else if (x.Margin.Left < y.Margin.Left) return -1;
							return 0;
						});

						foreach (AtomBase atom in selectedAtoms)
						{
							if (firstAtom != atom && lastAtom != atom)
							{
								dNewAtomLeft = (dNewAtomLeft + dInterval);
								atom.Margin = new Thickness (dNewAtomLeft, atom.Margin.Top, 0, 0);
								dNewAtomLeft = (dNewAtomLeft + atom.Width);
							}
						}
					}
				}

				firstAtom = null;
				lastAtom = null;
			}
		}

		/// <summary>
		/// 수직간격배열
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectVerticalIntervalArrangement (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (2 < selectedAtoms.Count)
			{
				double dFirstElementY = 99999;
				double dFirstElementEndY = -1;
				double dLastElementY = 99999;
				double dLastElementEndY = -1;
				double dTotalHeightOfAtoms = 0;
				AtomBase firstAtom = null;
				AtomBase lastAtom = null;

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness currentMargin = atom.Margin;
					double dCurrentY = (currentMargin.Top);
					double dCurrentEndY = (currentMargin.Top + atom.Height);

					if (dFirstElementY > dCurrentY)
					{
						firstAtom = atom;
						dFirstElementY = dCurrentY;
						dFirstElementEndY = dCurrentEndY;
					}

					if (dLastElementEndY < dCurrentEndY)
					{
						lastAtom = atom;
						dLastElementEndY = dCurrentEndY;
						dLastElementY = dCurrentY;
					}

					dTotalHeightOfAtoms += (atom.Height);
				}


				if (null == firstAtom || null == lastAtom)
				{
					return;
				}
				else
				{
					double dOffsetOfFirstAndLast = (dLastElementEndY - dFirstElementY);

					if (dTotalHeightOfAtoms < dOffsetOfFirstAndLast)
					{
						double dRemainder = (dOffsetOfFirstAndLast - dTotalHeightOfAtoms);
						double dInterval = (dRemainder / (selectedAtoms.Count - 1));
						double dNewAtomTop = dFirstElementEndY;

						selectedAtoms.Sort (delegate (AtomBase x, AtomBase y)
						{
							if (x.Margin.Top > y.Margin.Top) return 1;
							else if (x.Margin.Top < y.Margin.Top) return -1;
							return 0;

						});

						foreach (AtomBase atom in selectedAtoms)
						{
							if (firstAtom != atom && lastAtom != atom)
							{
								dNewAtomTop = (dNewAtomTop + dInterval);
								atom.Margin = new Thickness (atom.Margin.Left, dNewAtomTop, 0, 0);
								dNewAtomTop = (dNewAtomTop + atom.Height);
							}
						}
					}
				}

				firstAtom = null;
				lastAtom = null;
			}
		}

		/// <summary>
		/// 수평크기맞춤
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectHorizontalSizeDivide (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (1 < selectedAtoms.Count)
			{
				AtomBase LastestPressedAtom = GetLatestPressedAtom ();

				if (null != LastestPressedAtom)
				{
					//2019-10-21 kys 탭뷰 내부에서 드래그 했을경우 LastestPressedAtom이 탭뷰로 지정되면서 크기 정렬이 정상적으로 되지 않는 버그가 있어서 에외처리함.
					if (LastestPressedAtom is TabViewAtomBase)
					{
						LastestPressedAtom = selectedAtoms[0];
					}

					double dStandardWidth = LastestPressedAtom.Width;

					Size CurrentViewSize = GetViewSize ();
					//double dLimitWidth = CurrentViewSize.Width;

					foreach (AtomBase atom in selectedAtoms)
					{
						Type atomType = atom.GetType ();
						Thickness currentMargin = atom.Margin;
						double dAtomX = currentMargin.Left;
						double dAtomEndX = (dAtomX + dStandardWidth);

						if (OwnerViewDefine.OwnerViewType.View == ownerViewType)
						{
							//if (0 < dAtomX && dLimitWidth > dAtomEndX) 2020-03-23 kys 화면 밖으로 나가더라도 크기 변경되도록함
							if (0 < dAtomX)
							{
								if (typeof (FreeLineofAtom) != atomType || typeof (VHLineofAtom) != atomType)
								{
									atom.Width = dStandardWidth;
								}
							}
						}
						else if (OwnerViewDefine.OwnerViewType.TabView == ownerViewType || OwnerViewDefine.OwnerViewType.Scroll == ownerViewType)
						{
							if (0 < dAtomX)
							{
								if (typeof (FreeLineofAtom) != atomType || typeof (VHLineofAtom) != atomType)
								{
									atom.Width = dStandardWidth;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 수직크기맞춤
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectVerticalSizeDivide (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (1 < selectedAtoms.Count)
			{
				AtomBase LastestPressedAtom = GetLatestPressedAtom ();

				if (null != LastestPressedAtom)
				{
					//2019-10-21 kys 탭뷰 내부에서 드래그 했을경우 LastestPressedAtom이 탭뷰로 지정되면서 크기 정렬이 정상적으로 되지 않는 버그가 있어서 에외처리함.
					if (LastestPressedAtom is TabViewAtomBase)
					{
						LastestPressedAtom = selectedAtoms[0];
					}

					double dStandardHeight = LastestPressedAtom.Height;

					Size CurrentViewSize = GetViewSize ();
					//double dLimitHeight = CurrentViewSize.Height;

					foreach (AtomBase atom in selectedAtoms)
					{
						Type atomType = atom.GetType ();
						Thickness currentMargin = atom.Margin;
						double dAtomY = currentMargin.Top;
						double dAtomEndY = (dAtomY + dStandardHeight);

						if (OwnerViewDefine.OwnerViewType.View == ownerViewType)
						{
							//if (0 < dAtomY && dLimitHeight > dAtomEndY)
							if (0 < dAtomY)
							{
								if (typeof (FreeLineofAtom) != atomType || typeof (VHLineofAtom) != atomType)
								{
									atom.Height = dStandardHeight;
								}
							}
						}
						else if (OwnerViewDefine.OwnerViewType.TabView == ownerViewType || OwnerViewDefine.OwnerViewType.Scroll == ownerViewType)
						{
							if (0 < dAtomY)
							{
								if (typeof (FreeLineofAtom) != atomType || typeof (VHLineofAtom) != atomType)
								{
									atom.Height = dStandardHeight;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 크기맞춤
		/// </summary>
		/// <param name="selectedAtoms"></param>
		/// <param name="ownerViewType"></param>
		private void ProcessObjectSizeDivide (List<AtomBase> selectedAtoms, OwnerViewDefine.OwnerViewType ownerViewType)
		{
			if (1 < selectedAtoms.Count)
			{
				Size CurrentViewSize = GetViewSize ();
				//double dLimitWidth = CurrentViewSize.Width;
				//double dLimitHeight = CurrentViewSize.Height;

				AtomBase LastestPressedAtom = GetLatestPressedAtom ();

				if (null != LastestPressedAtom)
				{
					//2019-10-21 kys 탭뷰 내부에서 드래그 했을경우 LastestPressedAtom이 탭뷰로 지정되면서 크기 정렬이 정상적으로 되지 않는 버그가 있어서 에외처리함.
					if (LastestPressedAtom is TabViewAtomBase)
					{
						LastestPressedAtom = selectedAtoms[0];
					}

					double dStandardWidth = LastestPressedAtom.Width;
					double dStandardHeight = LastestPressedAtom.Height;

					foreach (AtomBase atom in selectedAtoms)
					{
						Type atomType = atom.GetType ();
						Thickness currentMargin = atom.Margin;
						double dAtomX = currentMargin.Left;
						double dAtomY = currentMargin.Top;
						double dAtomEndX = (dAtomX + dStandardWidth);
						double dAtomEndY = (dAtomY + dStandardHeight);

						if (OwnerViewDefine.OwnerViewType.View == ownerViewType)
						{
							//if (0 < dAtomY && dLimitHeight > dAtomEndY && 0 < dAtomX && dLimitWidth > dAtomEndX)
							if (0 < dAtomY && 0 < dAtomX)
							{
								if (typeof (FreeLineofAtom) != atomType || typeof (VHLineofAtom) != atomType)
								{
									atom.Width = dStandardWidth;
									atom.Height = dStandardHeight;
								}
							}
						}
						else if (OwnerViewDefine.OwnerViewType.TabView == ownerViewType || OwnerViewDefine.OwnerViewType.Scroll == ownerViewType)
						{
							if (0 < dAtomY && 0 < dAtomX)
							{
								if (typeof (FreeLineofAtom) != atomType || typeof (VHLineofAtom) != atomType)
								{
									atom.Width = dStandardWidth;
									atom.Height = dStandardHeight;
								}
							}
						}
					}
				}
			}
		}

		private void DaulMonitorGetLocation (Window CurrentWindow, double dExpaendSize)
		{
			Window MainWindow = Application.Current.MainWindow;

			PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual (MainWindow);
			System.Windows.Media.Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
			double thisDpiWidthFactor = m.M11;
			double thisDpiHeightFactor = m.M22;


			double dLeft = MainWindow.Left * thisDpiWidthFactor;
			double dTop = MainWindow.Top * thisDpiHeightFactor;

			System.Windows.Forms.Screen currentScreen = null;

			foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
			{
				if (dLeft >= screen.WorkingArea.Left && dLeft <= screen.WorkingArea.Left + screen.WorkingArea.Width
					&& dTop >= screen.WorkingArea.Top && dTop <= screen.WorkingArea.Top + screen.WorkingArea.Height)
				{
					currentScreen = screen;
					break;
				}
			}


			if (null != currentScreen)
			{
				double dWidth = currentScreen.Bounds.Width / thisDpiWidthFactor;
				double MaxLen = (currentScreen.Bounds.Left + currentScreen.Bounds.Width) / thisDpiWidthFactor;
				double MinLen = currentScreen.Bounds.Left / thisDpiWidthFactor;
				double CurrentWindowLeft = CurrentWindow.Left + dExpaendSize;

				if (CurrentWindowLeft > 0)
				{
					if (CurrentWindowLeft < MinLen)
					{
						CurrentWindow.Left = CurrentWindow.Left + MinLen + dExpaendSize;

						if (CurrentWindow.Left > MaxLen - CurrentWindow.Width)
						{
							CurrentWindow.Left = MinLen + (dWidth / 2);
						}

					}
					else
					{
						if (CurrentWindowLeft > MaxLen)
						{
							CurrentWindow.Left = CurrentWindow.Left - MaxLen;
						}
					}
				}
			}
		}

		private void ScriptSerialSetting ()
		{
			//if (null != m_pScriptWindow)
			//{
			//    m_pScriptWindow.SetScriptToDOC();
			//}

			if (null != m_pScriptWindow)
			{
				m_pScriptWindow.SetScriptToDOC ();
			}

			return;
		}

		private string GetDBIndexToDataBaseName (int nDBIndex)
		{
			Hashtable htDBIndexToDataBaseName = new Hashtable ();

			if (true == PQAppBase.ConnectStatus)
			{
				ArrayList arDSInfos = PQAppBase.KissGetCommonLoginedDSInfoList ();
				if (null != arDSInfos && 0 != arDSInfos.Count)
				{
					for (int i = 0; i < arDSInfos.Count; i++)
					{
						CDSInfo ds = arDSInfos[i] as CDSInfo;
						if (null != ds)
						{
							htDBIndexToDataBaseName.Add (i, ds.DepName.Trim ());
						}
					}
				}
			}

			string strDataBaseName = string.Empty;

			if (true == htDBIndexToDataBaseName.ContainsKey (nDBIndex))
			{
				strDataBaseName = htDBIndexToDataBaseName[nDBIndex] as string;
			}

			return strDataBaseName;
		}

		private void SetTableInformationAtHashTable (string strTableName, string strDataBaseName, Hashtable htInformation)
		{
			if (false == string.IsNullOrEmpty (strTableName) &&
				false == string.IsNullOrEmpty (strDataBaseName))
			{
				if (false == htInformation.ContainsKey (strTableName))
				{
					htInformation.Add (strTableName, strDataBaseName);
				}
			}
		}

		private int GetDataBaseNameToDBIndex (string strDataBaseName)
		{
			Hashtable htDBIndexToDataBaseName = new Hashtable ();

			if (true == PQAppBase.ConnectStatus)
			{
				ArrayList arDSInfos = PQAppBase.KissGetCommonLoginedDSInfoList ();
				if (null != arDSInfos && 0 != arDSInfos.Count)
				{
					for (int i = 0; i < arDSInfos.Count; i++)
					{
						CDSInfo ds = arDSInfos[i] as CDSInfo;
						if (null != ds)
						{
							htDBIndexToDataBaseName.Add (ds.DepName.Trim (), i);
						}
					}
				}
			}

			int nDBIndex = 1;

			if (true == htDBIndexToDataBaseName.ContainsKey (strDataBaseName))
			{
				nDBIndex = (int)htDBIndexToDataBaseName[strDataBaseName];
			}

			return nDBIndex;
		}

		private void UpdateDataBaseInformation (string strTableName, string strDataBaseName)
		{
			for (int i = 0; i < this.GetOrderAtom ().Count; i++)
			{
				Atom pAtom = this.GetOrderAtom ().GetAt (i) as Atom;
				if (null == pAtom)
					continue;

				MasterInputAttrib pInputScndAttrib = pAtom.GetAttrib () as MasterInputAttrib;
				if (null != pInputScndAttrib)
				{
					string strOldTableName = pInputScndAttrib.GetTableName (true);
					int nDBIndex = pInputScndAttrib.GetDBIndex ();
					string strOldDataBaseName = GetDBIndexToDataBaseName (nDBIndex);

					if (false == string.IsNullOrEmpty (strTableName) &&
						false == string.IsNullOrEmpty (strDataBaseName) &&
						strTableName == strOldTableName)
					{
						pInputScndAttrib.SetDSN (strDataBaseName, true);
						pInputScndAttrib.SetDBIndex (GetDataBaseNameToDBIndex (strDataBaseName));
					}

					if (null != pInputScndAttrib.JoinDataManager)
					{
						foreach (JoinData pJoinData in pInputScndAttrib.JoinDataManager.GetJoinData ())
						{
							if (strTableName == pJoinData.TableName)
							{
								pJoinData.DataBaseName = strDataBaseName;
								pJoinData.DBIndex = GetDataBaseNameToDBIndex (strDataBaseName);
							}
						}
					}
				}

				PopupAttrib pPopAttrib = pAtom.GetAttrib () as PopupAttrib;
				if (null != pPopAttrib && 0 == pPopAttrib.Kind)
				{
					for (int nPopIndex = 0; nPopIndex < pPopAttrib.GetSubFieldCount (); nPopIndex++)
					{
						if (pPopAttrib.GetSubTableName (nPopIndex) == strTableName)
						{
							pPopAttrib.SetDSN (strDataBaseName, true);
							pPopAttrib.DBIndex = GetDataBaseNameToDBIndex (strDataBaseName);
							break;
						}
					}
				}

				ComboAttrib pComoAttrib = pAtom.GetAttrib () as ComboAttrib;
				if (null != pComoAttrib && 0 == pComoAttrib.Kind)
				{
					ArrayList saFieldInfo = new ArrayList ();
					pComoAttrib.GetFieldInfo (saFieldInfo);
					if (0 != saFieldInfo.Count)
					{
						for (int nComboIndex = 0; nComboIndex < saFieldInfo.Count; nComboIndex++)
						{
							string strFieldInfo = saFieldInfo[nComboIndex] as string;
							if (false == string.IsNullOrEmpty (strFieldInfo))
							{
								string[] strDBInfos = strFieldInfo.Split (new char[] { '$' });
								if (strDBInfos[0] == strTableName)
								{
									pComoAttrib.PopDSN = strDataBaseName;
									pComoAttrib.PopupDBIndex = GetDataBaseNameToDBIndex (strDataBaseName);
									break;
								}
							}
						}
					}
				}

				BrowseAttrib pBrowseAttrib = pAtom.GetAttrib () as BrowseAttrib;
				if (null != pBrowseAttrib)
				{
					foreach (BrowseItem pBrowseAtom in pBrowseAttrib.BrowseItemList)
					{
						if (strTableName == pBrowseAtom.GetTableName (true))
						{
							pBrowseAtom.DSN = strDataBaseName;
							pBrowseAtom.DBIndex = GetDataBaseNameToDBIndex (strDataBaseName);
						}
					}
				}

				SearchConditionAttrib pSearchAttrib = pAtom.GetAttrib () as SearchConditionAttrib;
				if (null != pSearchAttrib)
				{
					if (null != pSearchAttrib.oaAttrib)
					{
						for (int nIndex = 0; nIndex < pSearchAttrib.oaAttrib.Count; nIndex++)
						{
							PopupAttrib pSubPopAttrib = pSearchAttrib.oaAttrib[nIndex] as PopupAttrib;
							if (null != pSubPopAttrib)
							{
								for (int nPopIndex = 0; nPopIndex < pSubPopAttrib.GetSubFieldCount (); nPopIndex++)
								{
									if (pSubPopAttrib.GetSubTableName (nPopIndex) == strTableName)
									{
										pSubPopAttrib.SetDSN (strDataBaseName, true);
										pSubPopAttrib.DBIndex = GetDataBaseNameToDBIndex (strDataBaseName);
									}
								}
							}
						}
					}
				}

				DBGridExAttrib pDBGridExAttrib = pAtom.GetAttrib () as DBGridExAttrib;
				if (null != pDBGridExAttrib)
				{
					if (null != pDBGridExAttrib.PopAttrib)
					{
						CObArray oaPopAttrib = pDBGridExAttrib.PopAttrib;
						foreach (PopupAttrib pDBGridPopAttrib in oaPopAttrib)
						{
							for (int nPopIndex = 0; nPopIndex < pDBGridPopAttrib.GetSubFieldCount (); nPopIndex++)
							{
								if (pDBGridPopAttrib.GetSubTableName (nPopIndex) == strTableName)
								{
									pDBGridPopAttrib.SetDSN (strDataBaseName, true);
									pDBGridPopAttrib.DBIndex = GetDataBaseNameToDBIndex (strDataBaseName);
								}
							}
						}
					}
				}
			}
		}

		private object DeleteAnimationGroupAtom (object objAtom)
		{
			EBookAnimationGroupofAtom eagAtom = objAtom as EBookAnimationGroupofAtom;
			DMTView parentView = GetParentView () as DMTView;

			if (null == eagAtom || null == parentView)
			{
				return null;
			}

			parentView.DeleteAnimationGroupAtom (eagAtom);

			return null;
		}

		#region |  ----- PreExecute 관련 -----  |

		private string SetTableNameAtEmptyUnit ()
		{
			return SetTableNameAtEmptyUnit ((DMTView)this.GetParentView ());
		}

		private string SetTableNameAtEmptyUnit (DMTView pFirstView)
		{
			if (null == pFirstView)
				return string.Empty;


			string strTableName = string.Empty;

			ArrayList oaAtoms = new ArrayList ();
			List<Atom> atomCores = pFirstView.GetViewAtomCoresTabOrdered ();
			// 2023.03.03 F5 작업으로 테이블 및 필드명 설정시 둥근 사각형 그리기 아톰의 데이터도 불러올 수 있도록 수정
			List<Atom> sqaureAtomList = pFirstView.GetAllAtomCores ().Where (item => true == (item is SquareAtom || item is RoundSquareAtom || item is OvalAtom)).ToList ();
			List<SquareAttrib> sqaureAttrib = sqaureAtomList.Select (item => item.GetAttrib () as SquareAttrib).Cast<SquareAttrib> ().ToList ();

			strTableName = sqaureAttrib.Where (item => true == item.CheckTableName).FirstOrDefault ()?.Title;

			if (true == string.IsNullOrEmpty (strTableName))
			{
				sqaureAtomList.Sort (delegate (Atom x, Atom y)
				{
					double dx = x.Attrib.AtomY;
					double dy = y.Attrib.AtomY;

					return dx.CompareTo (dy);
				});

				SquareAttrib atomAttrib = sqaureAtomList.FirstOrDefault ()?.GetAttrib () as SquareAttrib;
				strTableName = atomAttrib?.Title ?? string.Empty;
			}

			if (false == string.IsNullOrEmpty (strTableName))
				strTableName = strTableName.Replace (" ", "");

			strTableName = TrimInValidStringRegex (strTableName);

			return strTableName;
		}

		/// <summary>
		/// InputScndAtom 로드필드 정하는 로직 
		/// </summary>
		private void SetPrimaryKeyInformation ()
		{
			bool bFoundKey = false;
			List<Atom> allAtomCores = this.GetAllAtomCoresTabOrdered ();

			foreach (Atom AtomCore in allAtomCores)
			{
				int n = AtomCore.Attrib.AtomAbsoluteTabIndex;
			}

			foreach (object objAtomCore in allAtomCores)
			{
				Atom currentAtomCore = objAtomCore as Atom;
				if (null == currentAtomCore) { continue; }
				MasterInputAttrib currentInputScndAttrib = currentAtomCore.GetAttrib () as MasterInputAttrib;
				if (null == currentInputScndAttrib) { continue; }
				if (true == currentInputScndAttrib.IsLoadField)
				{
					bFoundKey = true;
					break;
				}
			}

			if (true == bFoundKey) { return; }

			ArrayList oaAtoms = new ArrayList ();
			for (int i = 0; i < allAtomCores.Count; i++)
			{
				Atom pAtom = allAtomCores[i] as Atom;

				if (null != pAtom && false == (pAtom is ScanAtom))
				{
					MasterInputAttrib pInputScndAttrib = pAtom.GetAttrib () as MasterInputAttrib;
					if (null != pInputScndAttrib)
					{
						if (pInputScndAttrib is InputAttrib || pInputScndAttrib is DateAttrib) { oaAtoms.Add (pAtom); }
					}
				}
			}

			if (0 == oaAtoms.Count) { return; }
			Atom pKeyAtom = oaAtoms[0] as Atom;
			if (null != pKeyAtom)
			{
				MasterInputAttrib pKeyInputScndAttrib = pKeyAtom.GetAttrib () as MasterInputAttrib;
				if (null != pKeyInputScndAttrib)
				{
					pKeyInputScndAttrib.IsLoadField = true;
					pKeyInputScndAttrib.IsAutoLoad = true;
					pKeyInputScndAttrib.IsAutoIncrease = true;
					pKeyAtom.SetAttrib (pKeyInputScndAttrib);
				}
			}
		}

		private void SetAutoErdSetInformation (string strDefaultTableName)
		{
			List<Atom> lstViewAtoms = this.GetViewAtomCoresTabOrdered ();
			SetAutoErdSetInformation (lstViewAtoms, strDefaultTableName);
		}

		/// <summary>
		/// 80 뷰의 테이블 명을 정하는 로직 
		/// </summary>
		/// <param name="pView"></param>
		/// <param name="strDefaultTableName"></param>
		private void SetAutoErdSetInformation (List<Atom> lstAtomCores, string strDefaultTableName)
		{
			ArrayList oaTableSetRect = new ArrayList ();

			foreach (object objAtomCore in lstAtomCores)
			{
				Atom currentAtomCore = objAtomCore as Atom;
				if (null == currentAtomCore) { continue; }

				if (currentAtomCore is TabViewAtom)
				{
					TabViewAtomBase tabofAtom = currentAtomCore.GetOfAtom () as TabViewAtomBase;
					if (null == tabofAtom) continue;

					for (int i = 0; i < tabofAtom.GetTabPageCount (); i++)
					{
						List<Atom> tabPageItem = tabofAtom.GetTabPageAllAtomCores (i);
						string strTabItemThumbText = ((TabViewItem)tabofAtom.TabViewItem[i]).CheckTableName ? ((TabViewItem)tabofAtom.TabViewItem[i]).TabThumbText : strDefaultTableName;
						SetAutoErdSetInformation (tabPageItem, strTabItemThumbText);
					}
				}

				SquareAttrib currentSquareAttrib = currentAtomCore.GetAttrib () as SquareAttrib;
				if (null != currentSquareAttrib && true == currentSquareAttrib.CheckTableName)
				{
					oaTableSetRect.Add (currentAtomCore);
				}
			}

			if (0 != oaTableSetRect.Count)
			{
				oaTableSetRect.Sort (new AtomBoundsSort ());

				for (int i = 0; i < oaTableSetRect.Count; i++)
				{
					Atom pStartAtom = oaTableSetRect[i] as Atom;
					i++;
					Atom pEndAtom = null;
					if (i < oaTableSetRect.Count)
					{
						pEndAtom = oaTableSetRect[i] as Atom;
					}

					SquareAttrib pSquareAttrib = pStartAtom.GetAttrib () as SquareAttrib;
					string strTable = TrimInValidStringRegex (pSquareAttrib.Title);
					SetTableNameAToB (lstAtomCores, pStartAtom, pEndAtom, strTable);
					i--;
				}
			}
			else
			{
				if (false == string.IsNullOrEmpty (strDefaultTableName))
				{
					UpdateTableInformation (lstAtomCores, strDefaultTableName);
				}
			}
		}

		private void SetTableNameAToB (List<Atom> lstAtomCores, Atom startAtom, Atom endAtom, string strTableName)
		{
			var oaBoundsAtoms = new List<Atom> ();

			var startAttrib = startAtom.Attrib;

			var startY = startAtom.Attrib.AtomY;
			var endY = null != endAtom ? endAtom.Attrib.AtomY : 0;

			foreach (object obj in lstAtomCores)
			{
				Atom currentAtomCore = obj as Atom;

				if (null == currentAtomCore || currentAtomCore is FreeLineAtom || currentAtomCore is VHLineAtom) { continue; }

				var currentAttrib = currentAtomCore.Attrib;

				var currentX = currentAttrib.AtomX;
				var currentY = currentAttrib.AtomY;
				var currentWidth = currentAttrib.AtomWidth;
				var currentHeight = currentAttrib.AtomWidth;

				if (startY <= currentY &&
					(null == endAtom || 
					(0 == endAtom.Attrib.AtomX && 0 == endAtom.Attrib.AtomY && 
					0 == endAtom.Attrib.AtomWidth && 0 == endAtom.Attrib.AtomHeight))
					|| currentY + currentHeight < endY)
				{
					// 포함
					oaBoundsAtoms.Add (currentAtomCore);
				}
			}

			if (0 == oaBoundsAtoms.Count)
				return;

			UpdateTableInformation (oaBoundsAtoms, strTableName);
		}

		private void UpdateTableInformation (List<Atom> atoms, string strTableName)
		{
			foreach (var atom in atoms)
			{
				if (null == atom)
					continue;

				MasterInputAttrib pInputScndAttrib = atom.Attrib as MasterInputAttrib;
				if (null != pInputScndAttrib && true == pInputScndAttrib.IsSaveField)
				{
					if (true == string.IsNullOrEmpty (pInputScndAttrib.GetTableName (true)))
					{
						pInputScndAttrib.SetTableName (strTableName, true);
					}
				}
			}
		}

		// 아톰에서 ERD관련정보를 뽑아낸다.
		// 2009.01.22 황성민
		// DB처리객체에서 설정한 관계설정정보와 연관된 필드를 추가하는 논리 반영함
		private string GetDBFieldAttribInfo (Atom pInputAtom, COrderAtom pOrderAtom, Hashtable pErdRelationMap)
		{
			Attrib pAttrib = pInputAtom.Attrib;

			if (null == pAttrib || !(pAttrib is MasterInputAttrib))
				return string.Empty;

			string strFormName = this.GetFormName ();

			MasterInputAttrib pInputAttrib = (MasterInputAttrib)(pAttrib);

			string sDSN = pInputAttrib.GetDSN (false).Trim ();
			string sTableName = pInputAttrib.GetTableName (false).Trim ();
			string sFieldName = pInputAttrib.GetFieldName (false).Trim ();

			// 검사논리 최적화를 위해서 미리 비교하도록 밑에 있는 부분 위로 가져옴
			if (0 == sFieldName.Length) //field명이 없으면 생략.
				return string.Empty;

			string sFieldType = pInputAttrib.GetFieldType ().Trim ();
			string sLen = string.Empty;
			string sLableName = string.Empty;
			string strKey = string.Empty;
			string strFKValue = string.Empty;
			string sScriptIndex = string.Empty;
			string strPeriodLength = string.Empty;

			if (!(pInputAttrib is ScrollAttrib))
			{
				// 접두어 있는 필드 길이 계산
				string strPrefix = pInputAttrib.GetDefaultString ().Trim ();
				int nPreFixLength = 0;

				if (0 == strPrefix.IndexOf ("\"") && strPrefix.Length - 1 == strPrefix.LastIndexOf ("\""))
				{
					// 2009.01.19 이정대, 따옴표제거
					strPrefix = strPrefix.Replace ("\"", "");
					nPreFixLength = strPrefix.Length;
				}
				else
				{
					// 아톰명을 사용했을 경우 아톰을 찾아서 해당 크기를 설정해준다.
					if (string.Empty != strPrefix)
					{
						for (int i = 0; i < pOrderAtom.Count; i++)
						{
							Atom pAtom = pOrderAtom[i] as Atom;
							if (null == pAtom)
								continue;

							if (pAtom.GetProperVar () != strPrefix)
								continue;

							MasterInputAttrib pTargetAttrib = pAtom.Attrib as MasterInputAttrib;
							if (null == pTargetAttrib)
								continue;

							nPreFixLength = pTargetAttrib.GetFieldLen ();
						}
					}
				}

				int nLen = pInputAttrib.GetFieldLen () + nPreFixLength;

				if ("char" == sFieldType)
				{
					if (true == PQAppBase.UseUTF8InErd)
					{
						double dFieldLength = nLen * 1.5;
						dFieldLength = Math.Round (dFieldLength);
						sLen = string.Format ("{0}", dFieldLength.ToString ());
					}
					else
					{
						sLen = string.Format ("{0}", nLen.ToString ());
					}
				}
				else if ("text" != sFieldType)
				{
					sLen = string.Format ("{0}", nLen.ToString ());
				}
				//

				// 소수자릿수 논리 추가
				if ("float" == sFieldType || "double" == sFieldType)
				{
					InputAttrib pInputAttrib2 = pInputAttrib as InputAttrib;
					if (null != pInputAttrib2)
					{
						if (0 == pInputAttrib2.Prime.IndexOf ("\"") && pInputAttrib2.Prime.Length - 1 == pInputAttrib2.Prime.LastIndexOf ("\""))
						{
							// 2009.01.19 이정대, 따옴표제거
							strPeriodLength = pInputAttrib2.Prime.Replace ("\"", "");
						}
					}
				}
			}

			sLableName = FindLabelName (pInputAtom);
			if (0 == sLableName.Length)
			{
				//라벨이 없으면 만들어준다.
				string strName = MakeLableOfAtomKind (pInputAtom.GetType ().Name);
				sLableName = string.Format ("{0}_{1}", strName, pInputAtom.Attrib.AtomAbsoluteTabIndex.ToString ());
			}

			sScriptIndex = string.Format ("{0}", pInputAtom.Attrib.AtomAbsoluteTabIndex.ToString ());


			string strFKTable = string.Empty;
			string strFKField = string.Empty;

			// 2009.01.21 황성민
			// DB처리객체 관계설정 관련 정보 구축
			string strFullName = string.Format ("{0}.{1}", sTableName, sFieldName);
			if (true == pErdRelationMap.ContainsKey (strFullName))
			{
				string strRelationTableField = pErdRelationMap[strFullName] as string;

				string[] saRelation = strRelationTableField.Split (new char[] { '.' });

				if (2 == saRelation.Length)
				{
					strFKTable = saRelation[0]; // 관계테이블
					strFKField = saRelation[1]; // 관계필드
				}
			}

			// 참조변수에 의한 참조키
			if (0 < pAttrib.Operate.Length)
			{
				string strOperate = pAttrib.Operate.Replace ("#", "");

				foreach (Atom pMasterAtom in pOrderAtom)
				{
					if (pMasterAtom == null)
						continue;

					Attrib pMasterAttrib = pMasterAtom.Attrib;
					if (null == pMasterAttrib)
						continue;

					if (pMasterAttrib is MasterInputAttrib)
					{
						if (true == pMasterAtom.GetProperVar ().Equals (strOperate))
						{
							strFKTable = pMasterAttrib.GetTableName (false).Trim ();
							strFKField = pMasterAttrib.GetFieldName (false).Trim ();
							break;
						}
					}
				}
			}

			if (0 != strFKTable.Length && 0 != strFKField.Length)
			{
				strKey = "FK";
				strFKValue = string.Format ("{0}.{1}.{2}", strFKTable, strFKField, sFieldName);
			}

			if (true == pInputAttrib.IsLoadField)
			{
				strKey = true == strKey.Equals ("FK") ? "PFK" : "PK";
			}

			string strToken = new string ((char)ProcessDef.TOKEN_INDEX1, 1);

			string strFKTableFieldInfo = string.Empty;
			if (0 != strFKTable.Length && 0 != strFKField.Length)
			{
				string strTargetKey = "PK";
				string[] saFKFieldInfo = {
											sDSN, strToken,
											strFKTable, strToken,
											strFKField, strToken,
											sFieldType, strToken,
											sLen, strToken,
											sLableName, strToken,
											sScriptIndex, strToken,
											strTargetKey, strToken,
											string.Empty, strToken,
											strFormName, strToken,
											strPeriodLength, strToken
									   };
				strFKTableFieldInfo = string.Format ("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}", saFKFieldInfo);
			}

			string[] saFieldInfo = {
										sDSN, strToken,
										sTableName, strToken,
										sFieldName, strToken,
										sFieldType, strToken,
										sLen, strToken,
										sLableName, strToken,
										sScriptIndex, strToken,
										strKey, strToken,
										strFKValue, strToken,
										strFormName, strToken,
										strPeriodLength, strToken
								   };

			string strInfo = string.Format ("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}", saFieldInfo);

			if (0 != strFKTableFieldInfo.Length)
			{
				string strToken2 = new string ((char)ProcessDef.TOKEN_INDEX2, 1);
				strInfo += strToken2 + strFKTableFieldInfo;
			}

			return strInfo;
		}

		/// <summary>
		/// 2009.01.21 황성민 
		/// QueryManager의 관계설정 정보를 가져옵니다.
		/// </summary>
		/// <returns></returns>
		private Hashtable GetErdRelationMap ()
		{
			Hashtable pErdRelationMap = new Hashtable ();

			CObArray poaDBManager = this.GetDBMaster ().GetAtSQLKindManager ((int)SQLQUERY_TYPE._SELECT_);

			for (int i = 0; i < poaDBManager.Count; i++)
			{
				CDocQueryMgr pQueryMgr = poaDBManager[i] as CDocQueryMgr;
				if (null == pQueryMgr)
					continue;

				pQueryMgr.MakeERDRelationInfo (pErdRelationMap);
			}

			return pErdRelationMap;
		}

		private void MakeDBMangerLinkedVariables ()
		{
			this.GetDBMaster ().MakeConnectFieldIndex ();
		}

		#endregion

		#region |  ----- F10 ERD 자동생성 -----  |

		private bool MakeERDInfomation80 (TopDBManager80.ErdEventArgs msg)
		{
			CObArray poaFormInfo = GetFormInformation ();
			if (0 == poaFormInfo.Count)
			{
				_Message80.Show (LC.GS ("TopProcess_DMTDoc_11"));
				return false;
			}

			string strFormName = this.GetFormName ();
			Hashtable pFormInfoMap = this.ReGenFormInfoMap (poaFormInfo);

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

			Hashtable htFormInformation = new Hashtable ();
			htFormInformation.Add (strFormName, pFormInfoMap);
			msg.ServerConnectionString = strConn;
			msg.FormInformationMap = htFormInformation;

			return true;
		}

		private Hashtable ReGenFormInfoMap (CObArray poaFormInfo)
		{
			Hashtable pFormInfoMap = new Hashtable ();

			for (int i = 0; i < poaFormInfo.Count; i++)
			{
				CStringArray psaFieldInfo = poaFormInfo[i] as CStringArray;
				if (null == psaFieldInfo)
					continue;

				string strTableName = psaFieldInfo.GetAt (1);
				strTableName = strTableName.ToLower ();

				string strFieldName = psaFieldInfo.GetAt (2);

				if (0 == strTableName.Trim ().Length)
					continue;

				if (true == pFormInfoMap.ContainsKey (strTableName))
				{
					CObArray poaFieldInfoList = pFormInfoMap[strTableName] as CObArray;
					if (null != poaFieldInfoList)
					{
						bool bFound = false;
						for (int j = 0; j < poaFieldInfoList.Count; j++)
						{
							CStringArray saOldField = poaFieldInfoList.GetAt (j) as CStringArray;
							if (null == saOldField)
								continue;

							string strOldFieldName = saOldField.GetAt (2);

							if (strFieldName.ToUpper () == strOldFieldName.ToUpper ())
							{
								bFound = true;
								break;
							}
						}

						if (false == bFound)
						{
							poaFieldInfoList.Add (psaFieldInfo);
						}
					}
				}
				else
				{
					CObArray poaFieldInfoList = new CObArray ();
					poaFieldInfoList.Add (psaFieldInfo);

					pFormInfoMap.Add (strTableName, poaFieldInfoList);
				}
			}

			return pFormInfoMap;
		}

		private void MakeDBFieldPopAttribInfo (string strTableName, string strFieldName, string strFieldType, string strFieldLength, CObArray oaFormInfo)
		{
			string strDSN = string.Empty;
			string strPeriodLength = string.Empty; // 소수자릿수
			string strScriptIndex = string.Empty;
			string strKey = string.Empty;
			string strFKValue = string.Empty;
			string strFKTable = string.Empty;
			string strFKField = string.Empty;
			string strLableName = string.Empty;

			CStringArray psaRecordInfo = new CStringArray ();
			psaRecordInfo.Add (strDSN);         //	(0)
			psaRecordInfo.Add (strTableName);   //	(1)
			psaRecordInfo.Add (strFieldName);   //	(2)
			psaRecordInfo.Add (strFieldType);   //	(3)
			psaRecordInfo.Add (strFieldLength); //	(4)
			psaRecordInfo.Add (strLableName);   //	(5)
			psaRecordInfo.Add (strScriptIndex); //	(6)	//scriptIndex기준
			psaRecordInfo.Add (strKey);         //	(7)
			psaRecordInfo.Add (strFKValue);     //	(8)
			psaRecordInfo.Add (strPeriodLength);//	(9) // 소수자릿수

			oaFormInfo.Add (psaRecordInfo);
		}

		private void MakeDBFieldAttribInfo (Atom pInputAtom, CObArray oaFormInfo, COrderAtom pOrderAtom, Hashtable pErdRelationMap)
		{
			MasterInputAttrib pInputAttrib = pInputAtom.Attrib as MasterInputAttrib;
			if (null == pInputAttrib)
				return;

			if (false == pInputAttrib.IsSaveField)
				return;

			string strDSN = pInputAttrib.GetDSN (false).Trim ();
			string strTableName = pInputAttrib.GetTableName (false).Trim ();
			string strFieldName = pInputAttrib.GetFieldName (false).Trim ();
			if (0 == strFieldName.Length)
				return;

			string strFieldType = pInputAttrib.GetFieldType ().Trim ();

			string strLength = "text" == strFieldType ? string.Empty : pInputAttrib.GetFieldLen ().ToString ();
			string strPeriodLength = string.Empty; // 소수자릿수

			if (false == (pInputAttrib is ScrollAttrib))
			{
				// 접두어 있는 필드 길이 계산
				String strPrefix = pInputAttrib.GetDefaultString ().Replace ("\"", string.Empty);
				int nPreFixLength = strPrefix.Length;

				if (0 == strPrefix.IndexOf ("\"") && strPrefix.Length - 1 == strPrefix.LastIndexOf ("\""))
				{
					// 2009.01.19 이정대, 따옴표제거
					strPrefix = strPrefix.Replace ("\"", "");
					nPreFixLength = strPrefix.Length;
				}
				else
				{
					if (string.Empty != strPrefix)
					{
						for (int i = 0; i < pOrderAtom.Count; i++)
						{
							Atom pAtom = pOrderAtom[i] as Atom;
							if (null == pAtom)
								continue;

							if (pAtom.GetProperVar () != strPrefix)
								continue;

							MasterInputAttrib pTargetAttrib = pAtom.Attrib as MasterInputAttrib;
							if (null == pTargetAttrib)
								continue;

							nPreFixLength = pTargetAttrib.GetFieldLen ();
						}
					}
				}

				int nLen = pInputAttrib.GetFieldLen () + nPreFixLength;
				if ("char" == strFieldType)
				{
					if (true == PQAppBase.UseUTF8InErd)
					{
						double dFieldLength = nLen * 1.5;
						dFieldLength = Math.Round (dFieldLength);
						strLength = string.Format ("{0}", dFieldLength.ToString ());
					}
					else
					{
						strLength = string.Format ("{0}", nLen.ToString ());
					}
				}
				else if ("text" != strFieldType)
				{
					strLength = nLen.ToString ();
				}

				// 소수자릿수 논리 추가
				if ("float" == strFieldType || "double" == strFieldType)
				{
					InputAttrib pInputAttrib2 = pInputAttrib as InputAttrib;
					if (null != pInputAttrib2)
					{
						if (0 == pInputAttrib2.Prime.IndexOf ("\"") && pInputAttrib2.Prime.Length - 1 == pInputAttrib2.Prime.LastIndexOf ("\""))
						{
							// 2009.01.19 이정대, 따옴표제거
							strPeriodLength = pInputAttrib2.Prime.Replace ("\"", "");
						}
					}
				}
			}

			string strScriptIndex = pInputAtom.Attrib.AtomAbsoluteTabIndex.ToString ();
			string strKey = string.Empty;
			string strFKValue = string.Empty;
			string strFKTable = string.Empty;
			string strFKField = string.Empty;

			// 2009.01.21 황성민
			// DB처리객체 관계설정 관련 정보 구축
			string strFullName = string.Format ("{0}.{1}", strTableName, strFieldName);
			if (true == pErdRelationMap.ContainsKey (strFullName))
			{
				string strRelationTableField = pErdRelationMap[strFullName] as string;

				string[] saRelation = strRelationTableField.Split (new char[] { '.' });

				if (2 == saRelation.Length)
				{
					strFKTable = saRelation[0]; // 관계테이블
					strFKField = saRelation[1]; // 관계필드
				}
			}

			// 참조변수에 의한 참조키
			if (0 < pInputAttrib.Operate.Length)
			{
				String strOperate = pInputAttrib.Operate.Replace ("#", "");
				foreach (Atom pMasterAtom in pOrderAtom)
				{
					if (null == pMasterAtom)
						continue;

					Attrib pMasterAttrib = pMasterAtom.Attrib;
					if (null == pMasterAttrib)
						continue;

					if (pMasterAttrib is MasterInputAttrib)
					{
						//if (true == pMasterAttrib.AtomProperVar.Equals(strOperate)) 원본
						if (true == pMasterAttrib.GetAtomProperVar (false).Equals (strOperate))
						{
							strFKTable = pMasterAttrib.GetTableName (false);
							strFKField = pMasterAttrib.GetFieldName (false);
							break;
						}
					}
				}
			}

			string strLableName = FindLabelName (pInputAtom);
			if (0 == strLableName.Length)
			{
				//라벨이 없으면 만들어준다.
				string strName = MakeLableOfAtomKind (pInputAtom.GetType ().Name);
				strLableName = string.Format ("{0}_{1}", strName, pInputAtom.Attrib.AtomAbsoluteTabIndex.ToString ());
			}

			if (0 != strFKTable.Length && 0 != strFKField.Length)
			{
				strKey = "FK";
				strFKValue = string.Format ("{0}.{1}.{2}", strFKTable, strFKField, strFieldName);
			}

			if (true == pInputAttrib.IsLoadField)
			{
				strKey = true == strKey.Equals ("FK") ? "PFK" : "PK";
			}

			CStringArray psaRecordInfo = new CStringArray ();
			psaRecordInfo.Add (strDSN);         //	(0)
			psaRecordInfo.Add (strTableName);   //	(1)
			psaRecordInfo.Add (strFieldName);   //	(2)
			psaRecordInfo.Add (strFieldType);   //	(3)
			psaRecordInfo.Add (strLength);      //	(4)
			psaRecordInfo.Add (strLableName);   //	(5)
			psaRecordInfo.Add (strScriptIndex); //	(6)	//scriptIndex기준
			psaRecordInfo.Add (strKey);         //	(7)
			psaRecordInfo.Add (strFKValue);     //	(8)
			psaRecordInfo.Add (strPeriodLength);//	(9) // 소수자릿수

			oaFormInfo.Add (psaRecordInfo);

			string strFKTableFieldInfo = string.Empty;
			if (0 != strFKTable.Length && 0 != strFKField.Length)
			{
				CStringArray psaFKRecordInfo = new CStringArray ();
				psaFKRecordInfo.Add (strDSN);           //	(0)
				psaFKRecordInfo.Add (strFKTable);       //	(1)
				psaFKRecordInfo.Add (strFKField);       //	(2)
				psaFKRecordInfo.Add (strFieldType);     //	(3)
				psaFKRecordInfo.Add (strLength);        //	(4)
				psaFKRecordInfo.Add (strLableName);     //	(5)
				psaFKRecordInfo.Add (strScriptIndex);   //	(6)	//scriptIndex기준
				psaFKRecordInfo.Add (string.Empty);     //	(7) // PK
				psaFKRecordInfo.Add (string.Empty);     //	(8)
				psaFKRecordInfo.Add (strPeriodLength);  //	(9) // 소수자릿수
				oaFormInfo.Add (psaFKRecordInfo);
			}
		}

		#endregion

		#region |  ----- DB 처리객체 -----  |

		private void ShowDBMgrFrame80 (object pMdiParent, string strQueryName, bool bDoModal, int nSQLKind, int nSQLIndex)
		{
			if (null != m_DBManagerFrame)
			{
				if (false == bDoModal)
				{
					m_DBManagerFrame.ActivateWindow ();
					return;
				}
			}

			m_DBManagerFrame = new TopDBManager80.DBManagerOwnerFrame ();
			m_DBManagerFrame.DocShow = true;
			m_DBManagerFrame.Closed += DBManagerFrame_Closed;

			m_DBManagerFrame.Owner = Application.Current.MainWindow;

			//m_DBManagerFrame.OnShowErdManager += new ShowErdManagerEventHandler(m_DBManagerFrame_OnShowErdManager);

			m_DBManagerFrame.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			m_DBManagerFrame.Title = string.Format (LC.GS ("{0} - {1}"), GetSubWindowTitle (), LC.GS ("TopProcess_DMTDoc_14"));
		

			CDocDBManagerList pDBManagerList = new CDocDBManagerList (this);
			m_DBManagerFrame.Open (pDBManagerList, this);

			if (null != pMdiParent)
			{
				m_DBManagerFrame.Owner = (Window)pMdiParent;
			}

			if (-1 != nSQLKind && -1 != nSQLIndex)
			{
				CDocQueryMgr pQueryMgr = GetDBMaster ().GetAtSQLManager (nSQLKind, nSQLIndex) as CDocQueryMgr;
				if (null != pQueryMgr)
					strQueryName = pQueryMgr.Text;
			}

			if (string.Empty != strQueryName)
				m_DBManagerFrame.Open (strQueryName);

			if (true == bDoModal)
			{
				m_DBManagerFrame.ShowDialog ();
			}
			else
			{
				m_DBManagerFrame.Show ();
			}
		}

		public void ShowStructDataMgrFrame (object pMdiParent, int nServiceType)
		{
			if (null != m_pStructDataMgrFrame)
			{
				if (false != m_pStructDataMgrFrame.IsVisible)
					m_pStructDataMgrFrame.ActivateWindow ();
				else
					m_pStructDataMgrFrame.Show ();

				return;
			}

			CreateStructDataMgrFrame (pMdiParent, nServiceType);

			m_pStructDataMgrFrame.Show ();
		}

		/// <summary>
		/// nServiceType = 0 : OpenApi, 1 : REST-WS, 2 : SOAP-WS, 3 : SAP-RFC, 4: SAP-OData
		/// </summary>
		/// <param name="pMdiParent"></param>
		/// <param name="nServiceType"></param>
		public void CreateStructDataMgrFrame (object pMdiParent, int nServiceType)
		{
			if (null == m_pStructDataMgrFrame)
			{
				m_pStructDataMgrFrame = new StructDataManagerOwnerFrame ();
				m_pStructDataMgrFrame.Owner = pMdiParent as Window;
				m_pStructDataMgrFrame.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				m_pStructDataMgrFrame.ServiceType = nServiceType;
				m_pStructDataMgrFrame.Closed += StructDataMgrFrame_Closed;
				m_pStructDataMgrFrame.OnBrowseAtomUpdate += StructDataMgrFrame_OnBrowseAtomUpdate;
				m_pStructDataMgrFrame.OnMakeInputAtom += StructDataMgrFrame_OnMakeInputAtom;
				m_pStructDataMgrFrame.FormTitle = GetSubWindowTitle ();

				m_pStructDataMgrFrame.Open (m_pStructDataMgr);

				// 외부기능 연계 오픈시 DB처리객체 넘겨주는 것 추가
				CDocDBManagerList pDBManagerList = new CDocDBManagerList (this);

				m_pStructDataMgrFrame.Open (pDBManagerList);
				m_pStructDataMgrFrame.Owner = (Window)pMdiParent;
			}
			else
			{
				m_pStructDataMgrFrame.ServiceType = nServiceType;
				m_pStructDataMgrFrame.Open (m_pStructDataMgr);
			}
		}

		public void AutoLinkStructDataMgrFrame (string strContent)
		{
			m_pStructDataMgrFrame.AutoLinkStructDataManager (strContent);
		}

		private bool IsGlobalScriptUserVar (string strGlobartVar)
		{
			// 전역정보에 기본으로 들어가는 변수들
			// 이 변수들 이외에는 사용자가 정의한 변스 
			switch (strGlobartVar)
			{
				case "USER_ID":
				case "DP_SCODE":
				case "AB_SYEAR":
				case "DP_SNAME":
				case "EV_DECIMAL":
				case "MENU":
					return false;
				default:
					if (LC.GS ("TopProcess_DMTDoc_2").ToUpper () == strGlobartVar.ToUpper () || LC.GS ("TopProcess_DMTDoc_1").ToUpper () == strGlobartVar.ToUpper ())
						return false;
					break;
			}

			return true;
		}

		#endregion

		#region |  ----- 진행관리자 -----  |

		private string GetFuncName (string strFunction)
		{
			string strFunctionName = strFunction;


			if (0 == strFunction.IndexOf ("IM"))
			{
				strFunctionName = strFunctionName.Remove (0, 2);
				strFunctionName = CScriptApp.GetNameFromID (Convert.ToInt32 (strFunctionName));
			}
			else
			{
				string[] strInfo = strFunction.Split ('_');
				if (null != strInfo && 2 == strInfo.Length)
				{
					int nScriptIndex = Convert.ToInt32 (strInfo[0]);
					int nFunction = Convert.ToInt32 (strInfo[1]);

					IScriptObject scriptObject = GetAtomFromScriptID (nScriptIndex);
					if (null != scriptObject)
					{
						string strProper = scriptObject.Name;
						string strIDName = CScriptApp.GetNameFromID (nFunction);

						strFunctionName = strProper + strIDName;
					}
				}
			}

			return strFunctionName;
		}

		#endregion

		#region |  ----- 업무규칙 -----  |

		private bool IsExistPMScriptInfo (CObArray oaInfoList, string strKey)
		{
			bool bResult = false;

			foreach (CObArray oaInfo in oaInfoList)
			{
				string strInfo = oaInfo[1] as string;
				if (strInfo == strKey)
				{
					bResult = true;
					break;
				}
			}
			return bResult;
		}

		/// <summary>
		/// 해당 아톰의 진행관리 정보중 스크립트 관련 정보의 목록을 가져옵니다.
		/// </summary>
		/// <param name="pAtom">선택된 아톰</param>
		/// <returns>아톰의 스크립트 관련 정보 목록</returns>
		/// 2011.01.07 황성민
		/// 신규버전 진행관리자에서 사용
		private CObArray GetPMScriptInfo (Atom pAtom)
		{
			CObArray oaScriptInfoList = new CObArray ();
			CObArray oaInfo = pAtom.Processing;
			if (null == oaInfo || 0 == oaInfo.Count)
				return oaScriptInfoList;

			for (int nIndex = 0; nIndex < 2; nIndex++)
			{
				if (nIndex >= oaInfo.Count)
					continue;

				CObMapX mEvent = oaInfo[nIndex] as CObMapX;
				if (null == mEvent)
					continue;

				IDictionaryEnumerator ie = mEvent.GetEnumerator ();
				while (ie.MoveNext ())
				{
					CObArray oaProcessEventInformation = ie.Value as CObArray;
					if (null == oaProcessEventInformation)
						continue;

					for (int nProcessIndex = 0; nProcessIndex < oaProcessEventInformation.Count; nProcessIndex++)
					{
						ProcessEventInformation pProcessEventInformation = oaProcessEventInformation[nProcessIndex] as ProcessEventInformation;
						if (null == pProcessEventInformation)
							continue;

						CObArray oaPMInfo = pProcessEventInformation.PMObjects;

						for (int nIndex1 = 0; null != oaPMInfo && nIndex1 < oaPMInfo.Count; nIndex1++)
						{
							PMInfoScriptFunc pPMScriptInfo = oaPMInfo[nIndex1] as PMInfoScriptFunc;
							if (null == pPMScriptInfo)
								continue;

							if (true == IsExistPMScriptInfo (oaScriptInfoList, pPMScriptInfo.Function))
								continue;

							CObArray pScriptInfo = new CObArray (2);
							pScriptInfo.Add (GetFuncName (pPMScriptInfo.Function));
							pScriptInfo.Add (pPMScriptInfo.Function);
							oaScriptInfoList.Add (pScriptInfo);
						}
					}
				}
			}
			return oaScriptInfoList;
		}

		private bool IsExistsScriptFile ()
		{
			if (false == IsEBookDoc)
			{
				string strPath = String.Concat (this.FilePath, ".obj");
				return File.Exists (strPath);
			}
			else
			{
				TopView currentView = GetParentView () as TopView;
				EBookManager ebookManager = this.EBookManager as EBookManager;
				int nPage = ebookManager.GetCurrentPage (currentView);
				string strScriptEntry = EBookZipHelper.Instance.GetScriptObjEntryName (nPage);

				return EBookZipHelper.Instance.IsExistsEntry (ebookManager.LoadZip, strScriptEntry);
			}
		}

		#endregion//업무규칙

		#region |  ----- 디자인 도우미 -----  |

		/// <summary>
		/// 2014-10-24-M02 디자인도우미 이미지 저장 오류 ( GDI 저장 처리 )
		/// </summary>
		/// <param name="ViewAttrib"></param>
		private void SetBackImageGDIImage (CViewAttrib ViewAttrib)
		{
			// 이미지 변경에 대한 로직 추가
			CGDIMgr pGDIMgr = this.GetGDIManager ();
			if (null != pGDIMgr)
			{
				CObjectImage pObjectImage = new CObjectImage ();
				pObjectImage.AddRef ();
				pObjectImage.ImagePath = ViewAttrib.ImagePath;

				int nKey = ViewAttrib.ImageKey;
				pGDIMgr.GetKey (ref nKey, pObjectImage);
				pObjectImage.InitObjectGDI ();
				ViewAttrib.ImageKey = nKey;
			}
		}

		/// <summary>
		///  2022.12.26 beh 버튼 이미지 나인패치 미적용 오류 수정 위해 미사용 파라미터 int nImageWidth, int nImageHeight -> bool isChangeNinePatch = true로 변경
		/// </summary>
		/// <param name="strFilePath"></param>
		/// <param name="BrightnessColor"></param>
		/// <param name="isChangeNinePatch"></param>
		/// <returns></returns>
		private BitmapImage GetDecoImageDesign (string strFilePath, System.Drawing.Color BrightnessColor, bool isChangeNinePatch = true)
		{
			return TranslateImage.Manager.ReplaceImageColor (strFilePath, BrightnessColor, isChangeNinePatch);
		}

		/// <summary>
		/// 2014-10-24-M02 디자인도우미 이미지 저장 오류 ( GDI 저장 처리 )
		/// </summary>
		/// <param name="pAttrib"></param>
		/// <param name="strFilePath"></param>
		/// <param name="bitImage"></param>
		private void SetDecoImageGDIImage (Attrib pAttrib, string strFilePath, BitmapImage bitImage)
		{
			CObjectImage pObjectImage = null;
			int nKey = pAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._IMAGE);
			pAttrib.GetGDIObjFromKey (ref pObjectImage, nKey);

			pObjectImage = new CObjectImage ();
			pObjectImage.AddRef ();

			pObjectImage.ImagePath = strFilePath;
			pObjectImage.SelectImage = bitImage;

			nKey = pAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._IMAGE);

			if (strFilePath.Length == 0)
			{
				pAttrib.SetGDIKey ((int)Define.OBJECTKEY_TYPE._IMAGE, 0);
			}
			else
			{
				nKey = pAttrib.GetKeyFromGDIObj (pObjectImage, nKey);
				pAttrib.SetGDIKey ((int)Define.OBJECTKEY_TYPE._IMAGE, nKey);
			}
		}

		private void ApplyDesignHelperImage (int nDesignViewType, ImageSpecification ApplyImageSpec)
		{
			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtoms ();
			int nCountOfSelectedAtoms = lstCurrentSelectedAtoms.Count;
			double dImageWidth = ApplyImageSpec.HorizontalResolution;
			double dImageHeight = ApplyImageSpec.VerticalResolution;
			System.Drawing.Color BrightnessColor = ApplyImageSpec.BrightnessColor;
			string strFilePath = ApplyImageSpec.FilePath;
			ArrayList arMakedAtomList = null;

			if (true == File.Exists (strFilePath))
			{
				//2014-10-24-M01 디자인도우미 이미지 저장 오류
				BitmapImage SourceBitImage = CloneObject.FromFile (strFilePath);
				ImageBrush DesignImage = new ImageBrush (SourceBitImage);

				if (0 < nCountOfSelectedAtoms)
				{
					for (int nIndex = 0; nIndex < nCountOfSelectedAtoms; nIndex++)
					{
						switch (nDesignViewType)
						{
							case 2:
								{
									ActionofAtom ButtonAtom = lstCurrentSelectedAtoms[nIndex] as ActionofAtom;

									if (null == ButtonAtom)
									{
										arMakedAtomList = GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyMakeAtom (AtomType.FunctionButton, true, true);
										if (1 == arMakedAtomList.Count)
										{
											ButtonAtom = arMakedAtomList[0] as ActionofAtom;
										}
									}

									if (null != ButtonAtom)
									{
										ButtonAtom.NinePatchBrightnessColor = BrightnessColor;

										if (3 == nDesignViewType || 5 == nDesignViewType)
										{
											ButtonAtom.Width = dImageWidth;
											ButtonAtom.Height = dImageHeight;
										}

										if (BrightnessColor.Name != "Transparent")
										{
											int nPos = strFilePath.LastIndexOf ("\\");
											string strOrgFileName1 = (-1 != nPos) ? strFilePath.Substring (nPos + 1) : strFilePath;

											BitmapImage PreviewImage = GetDecoImageDesign (strFilePath, BrightnessColor, false);

											if (PreviewImage != null)
											{
												string strImagePath = SaveDesignImage (strFilePath, ButtonAtom.AtomCore, BrightnessColor);

												ActionAttrib pAttrib = ButtonAtom.AtomCore.GetAttrib () as ActionAttrib;

												pAttrib.BorderStyle = System.Windows.Forms.BorderStyle.None;
												SetDecoImageGDIImage (pAttrib, strImagePath, PreviewImage);

												ButtonAtom.SetAtomThickness (new Thickness (0));
												ButtonAtom.ImagePath = strImagePath;
											}
										}
										else
										{
											string strImagePath = SaveDesignImage (strFilePath, ButtonAtom.AtomCore);

											//2014-10-24-M02 디자인도우미 이미지 저장 오류 ( GDI 저장 처리 )
											ActionAttrib pAttrib = ButtonAtom.AtomCore.GetAttrib () as ActionAttrib;

											pAttrib.BorderStyle = System.Windows.Forms.BorderStyle.None;
											pAttrib.SetObjectImage (strImagePath, "", "");

											ButtonAtom.SetAtomThickness (new Thickness (0));
											ButtonAtom.ImagePath = strImagePath;
										}

										ButtonAtom.ImagePathChanged ();
									}

									break;
								}

							case 3:
							case 4:
							case 5:
								{
									DecorImageofAtom DecoImageAtom = lstCurrentSelectedAtoms[nIndex] as DecorImageofAtom;
									if (null != DecoImageAtom)
									{
										DecoImageAtom.NinePatchBrightnessColor = BrightnessColor;
										DecoImageAtom.Width = dImageWidth;
										DecoImageAtom.Height = dImageHeight;

										if (BrightnessColor.Name != "Transparent")
										{
											int nPos = strFilePath.LastIndexOf ("\\");
											string strOrgFileName1 = (-1 != nPos) ? strFilePath.Substring (nPos + 1) : strFilePath;
											BitmapImage PreviewImage = GetDecoImageDesign (strFilePath, BrightnessColor);

											if (PreviewImage != null)
											{
												string strImagePath = SaveDesignImage (strFilePath, DecoImageAtom.AtomCore, BrightnessColor);

												DecorImageAttrib pAttrib = DecoImageAtom.AtomCore.GetAttrib () as DecorImageAttrib;
												SetDecoImageGDIImage (pAttrib, strImagePath, PreviewImage);

												DecoImageAtom.ImagePath = strImagePath;
											}
										}
										else
										{
											string strImagePath = SaveDesignImage (strFilePath, DecoImageAtom.AtomCore);

											//2014-10-24-M02 디자인도우미 이미지 저장 오류 ( GDI 저장 처리 )
											DecorImageAttrib pAttrib = DecoImageAtom.AtomCore.GetAttrib () as DecorImageAttrib;
											SetDecoImageGDIImage (pAttrib, strImagePath, null);
											DecoImageAtom.ImagePath = strImagePath;
										}

										DecoImageAtom.CompletePropertyChanged ();
									}
									else
									{
										ActionofAtom ButtonAtom = lstCurrentSelectedAtoms[nIndex] as ActionofAtom;
										if (null != ButtonAtom)
										{
											ButtonAtom.NinePatchBrightnessColor = BrightnessColor;
											if (3 == nDesignViewType || 5 == nDesignViewType)
											{
												ButtonAtom.Width = dImageWidth;
												ButtonAtom.Height = dImageHeight;
											}

											if (BrightnessColor.Name != "Transparent")
											{
												int nPos = strFilePath.LastIndexOf ("\\");
												string strOrgFileName1 = (-1 != nPos) ? strFilePath.Substring (nPos + 1) : strFilePath;
												BitmapImage PreviewImage = GetDecoImageDesign (strFilePath, BrightnessColor);
												if (PreviewImage != null)
												{
													string strImagePath = SaveDesignImage (strFilePath, ButtonAtom.AtomCore, BrightnessColor);

													ActionAttrib pAttrib = ButtonAtom.AtomCore.GetAttrib () as ActionAttrib;
													SetDecoImageGDIImage (pAttrib, strImagePath, PreviewImage);

													ButtonAtom.ImagePath = strImagePath;
												}
											}
											else
											{
												string strImagePath = SaveDesignImage (strFilePath, ButtonAtom.AtomCore);
												//2014-10-24-M02 디자인도우미 이미지 저장 오류 ( GDI 저장 처리 )
												ActionAttrib pAttrib = ButtonAtom.AtomCore.GetAttrib () as ActionAttrib;
												pAttrib.SetObjectImage (strImagePath, "", "");
												ButtonAtom.ImagePath = strImagePath;
											}

											ButtonAtom.ImagePathChanged ();
										}
										else
										{
											arMakedAtomList = GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyMakeAtom (AtomType.DecorImage, true, true);
											if (null != arMakedAtomList)
											{
												int nCountOfAtom = arMakedAtomList.Count;

												if (1 == nCountOfAtom)
												{
													DecoImageAtom = arMakedAtomList[0] as DecorImageofAtom;
													DecoImageAtom.NinePatchBrightnessColor = BrightnessColor;
													DecoImageAtom.Width = dImageWidth;
													DecoImageAtom.Height = dImageHeight;

													if (BrightnessColor.Name != "Transparent")
													{
														int nPos = strFilePath.LastIndexOf ("\\");
														string strOrgFileName1 = (-1 != nPos) ? strFilePath.Substring (nPos + 1) : strFilePath;
														BitmapImage PreviewImage = GetDecoImageDesign (strFilePath, BrightnessColor);

														if (PreviewImage != null)
														{
															string strImagePath = SaveDesignImage (strFilePath, DecoImageAtom.AtomCore, BrightnessColor);

															DecorImageAttrib pAttrib = DecoImageAtom.AtomCore.GetAttrib () as DecorImageAttrib;
															SetDecoImageGDIImage (pAttrib, strImagePath, PreviewImage);

															DecoImageAtom.ImagePath = strImagePath;
														}
													}
													else
													{
														string strImagePath = SaveDesignImage (strFilePath, DecoImageAtom.AtomCore);

														//2014-10-24-M02 디자인도우미 이미지 저장 오류 ( GDI 저장 처리 )
														DecorImageAttrib pAttrib = DecoImageAtom.AtomCore.GetAttrib () as DecorImageAttrib;
														SetDecoImageGDIImage (pAttrib, strImagePath, null);

														DecoImageAtom.ImagePath = strImagePath;
													}

													DecoImageAtom.CompletePropertyChanged ();
												}
											}
										}
									}

									break;
								}

							default: break;
						}
					}
				}

				else
				{
					switch (nDesignViewType)
					{
						case 2:
							{
								arMakedAtomList = GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyMakeAtom (AtomType.FunctionButton, true, true);

								if (null != arMakedAtomList)
								{
									int nCountOfAtom = arMakedAtomList.Count;

									if (1 == nCountOfAtom)
									{
										ActionofAtom ButtonAtom = arMakedAtomList[0] as ActionofAtom;
										ButtonAtom.NinePatchBrightnessColor = BrightnessColor;

										if (3 == nDesignViewType || 5 == nDesignViewType)
										{
											ButtonAtom.Width = dImageWidth;
											ButtonAtom.Height = dImageHeight;
										}

										if (BrightnessColor.Name != "Transparent")
										{
											int nPos = strFilePath.LastIndexOf ("\\");
											string strOrgFileName1 = (-1 != nPos) ? strFilePath.Substring (nPos + 1) : strFilePath;
											BitmapImage PreviewImage = GetDecoImageDesign (strFilePath, BrightnessColor);
											if (PreviewImage != null)
											{
												string strImagePath = SaveDesignImage (strFilePath, ButtonAtom.AtomCore, BrightnessColor);

												ActionAttrib pAttrib = ButtonAtom.AtomCore.GetAttrib () as ActionAttrib;
												SetDecoImageGDIImage (pAttrib, strImagePath, PreviewImage);

												ButtonAtom.ImagePath = strImagePath;
											}
										}
										else
										{
											string strImagePath = SaveDesignImage (strFilePath, ButtonAtom.AtomCore);
											ActionAttrib pAttrib = ButtonAtom.AtomCore.GetAttrib () as ActionAttrib;
											pAttrib.SetObjectImage (strImagePath, "", "");
											ButtonAtom.ImagePath = strImagePath;
										}

										ButtonAtom.ImagePathChanged ();
									}
								}

								break;
							}
						case 3:
						case 4:
						case 5:
							{
								arMakedAtomList = GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyMakeAtom (AtomType.DecorImage, true, true);

								if (null != arMakedAtomList)
								{
									int nCountOfAtom = arMakedAtomList.Count;

									if (1 == nCountOfAtom)
									{
										DecorImageofAtom DecoImageAtom = arMakedAtomList[0] as DecorImageofAtom;
										DecoImageAtom.NinePatchBrightnessColor = BrightnessColor;
										DecoImageAtom.Width = dImageWidth;
										DecoImageAtom.Height = dImageHeight;

										if (BrightnessColor.Name != "Transparent")
										{
											int nPos = strFilePath.LastIndexOf ("\\");
											string strOrgFileName1 = (-1 != nPos) ? strFilePath.Substring (nPos + 1) : strFilePath;
											BitmapImage PreviewImage = GetDecoImageDesign (strFilePath, BrightnessColor);

											if (PreviewImage != null)
											{
												string strImagePath = SaveDesignImage (strFilePath, DecoImageAtom.AtomCore, BrightnessColor);
												DecorImageAttrib pAttrib = DecoImageAtom.AtomCore.GetAttrib () as DecorImageAttrib;
												SetDecoImageGDIImage (pAttrib, strImagePath, PreviewImage);

												DecoImageAtom.ImagePath = strImagePath;
											}

										}
										else
										{
											string strImagePath = SaveDesignImage (strFilePath, DecoImageAtom.AtomCore, System.Drawing.Color.Transparent);
											//2014-10-24-M02 디자인도우미 이미지 저장 오류 ( GDI 저장 처리 )
											DecorImageAttrib pAttrib = DecoImageAtom.AtomCore.GetAttrib () as DecorImageAttrib;
											SetDecoImageGDIImage (pAttrib, strImagePath, null);

											DecoImageAtom.ImagePath = strImagePath;
										}

										DecoImageAtom.CompletePropertyChanged ();
									}
								}

								break;
							}

						default: break;
					}
				}
			}
		}

		private string SaveDesignImage (string strFilePath, Atom atomCore)
		{
			return SaveDesignImage (strFilePath, atomCore, System.Drawing.Color.Transparent);
		}

		private string SaveDesignImage (string strFilePath, Atom atomCore, System.Drawing.Color color)
		{
			string strNewFilePath = "";

			if (true == File.Exists (strFilePath))
			{
				string strAtomName = "";

				if (null != atomCore)
				{
					strAtomName = atomCore.GetProperVar (true);
				}
				else
				{
					//atomCore가 Null인경우 폼의 배경그림설정을 했기 때문에 폼명칭으로 이미지 저장한다.
					strAtomName = this.ExeFormName;
				}

				string strFileName = Path.GetFileName (strFilePath);
				string strExtension = Path.GetExtension (strFilePath);
				string strTargetFilePath = "@Path:\\Images\\";

				strTargetFilePath = PQAppBase.KissGetFullPath (strTargetFilePath);

				if (false == Directory.Exists (strTargetFilePath))
				{
					Directory.CreateDirectory (strTargetFilePath);
				}

				if (System.Drawing.Color.Transparent != color)
				{
					//색상값이 있는경우
					strFileName = string.Format ("{0}_{1}", strAtomName, strFileName);

					strTargetFilePath += strFileName;

					//미리보기 이미지 저장하면 안됨
					BitmapImage image = TranslateImage.Manager.ReplaceImageColor (strFilePath, color, false);
					PngBitmapEncoder encoder = new PngBitmapEncoder ();
					encoder.Frames.Add (BitmapFrame.Create (image));

					using (FileStream fs = new FileStream (strTargetFilePath, FileMode.Create))
					{
						encoder.Save (fs);
					}

					strNewFilePath = PathHandler.GetRelativePath (strTargetFilePath);

				}
				else
				{
					//원본 파일만 있는경우
					strTargetFilePath += strFileName;

					File.Copy (strFilePath, strTargetFilePath, true);
					strNewFilePath = PathHandler.GetRelativePath (strTargetFilePath);
				}
			}

			return strNewFilePath;
		}

		#endregion

		#region |  ----- 실행질의문 관련 -----  |

		/// <summary>
		/// 2008.01.16 황성민
		/// 실행질의문 창을 닫을때 사용합니다.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pUserQueryDialog_Closed (object sender, EventArgs e)
		{
			pUserQueryDialog = null;
		}

		#endregion

		#endregion

		public override void AdjustAtomProperVar (AtomBase sourceAtom)
		{
			DMTView view = GetParentView () as DMTView;
			CFrameAttrib frameAttrib = this.GetFrameAttrib ();

			if (null != view && null != frameAttrib)
			{
				if (true == frameAttrib.IsDynamicMode)
				{
					string strNewAtomName = view.AdjustDynamicWebAtomName (sourceAtom);
					sourceAtom.AtomCore.AtomProperVar = strNewAtomName;
				}
				else
				{
					base.AdjustAtomProperVar (sourceAtom);
				}
			}
		}

		public bool CalculateAtomsRegionStartPointAndSize (List<AtomBase> selectedAtoms, ref Thickness returnAtomsRegionStartMargin, ref Size returnAtomsRegionSize)
		{
			if (0 < selectedAtoms.Count)
			{
				double nMinimumX = 99999;
				double nMinimumY = 99999;
				double nMaximumX = 0;
				double nMaximumY = 0;

				foreach (AtomBase atom in selectedAtoms)
				{
					Thickness currentAtomMargin = atom.Margin;
					double dCurrentAtomLeft = currentAtomMargin.Left;
					double dCurrentAtomTop = currentAtomMargin.Top;
					double dCurrentAtomWidth = dCurrentAtomLeft + (!Kiss.DoubleEqual (0, atom.ActualWidth) ? atom.ActualWidth : atom.Width);
					double dCurrentAtomHeight = dCurrentAtomTop + (!Kiss.DoubleEqual (0, atom.ActualHeight) ? atom.ActualHeight : atom.Height);

					if (nMinimumX > dCurrentAtomLeft)
						nMinimumX = dCurrentAtomLeft;

					if (nMinimumY > dCurrentAtomTop)
						nMinimumY = dCurrentAtomTop;


					if (nMaximumX < dCurrentAtomWidth)
						nMaximumX = dCurrentAtomWidth;

					if (nMaximumY < dCurrentAtomHeight)
						nMaximumY = dCurrentAtomHeight;
				}

				returnAtomsRegionStartMargin = new Thickness (nMinimumX, nMinimumY, 0, 0);
				returnAtomsRegionSize = new Size (Math.Max (5, nMaximumX - nMinimumX), Math.Max (5, nMaximumY - nMinimumY));
				return true;
			}

			return false;
		}

		public Thickness GetAtomRealMargin (AtomBase selectedAtom)
		{
			Thickness lastestAtomMargin = new Thickness ();
			lastestAtomMargin.Left = selectedAtom.Margin.Left;
			lastestAtomMargin.Top = selectedAtom.Margin.Top;
			lastestAtomMargin.Right = selectedAtom.ActualWidth;
			lastestAtomMargin.Bottom = selectedAtom.ActualHeight;

			if (null != selectedAtom.GetTabViewAtom ())
			{
				FrameworkElement tabAtom = selectedAtom.GetTabViewAtom ();

				if (tabAtom is ExpandViewofAtom)
				{
					//확장뷰인경우 상단, 하단 영역별로 margin처리 해줘야함
					ExpandViewofAtom expandAtom = tabAtom as ExpandViewofAtom;

					int nPage = expandAtom.GetTabPageIndexToAtom (selectedAtom);

					if (0 == nPage)
					{
						lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left;
						lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top;
					}
					else
					{
						ExpandViewAttrib expandAttrib = expandAtom.AtomCore.GetAttrib () as ExpandViewAttrib;

						if (0 == expandAttrib.ExpandType)
						{
							lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left;
							lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top + (expandAtom.ActualHeight - expandAttrib.FixedViewHeight);
						}
						else if (1 == expandAttrib.ExpandType)
						{
							lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left;
							lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top + expandAttrib.FixedViewHeight;
						}
					}
				}
				else if (tabAtom is ScrollViewofAtom)
				{
					ScrollViewofAtom scrollViewAtom = tabAtom as ScrollViewofAtom;

					Point ptOffset = scrollViewAtom.GetScrollOffset ();

					lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left - ptOffset.X;
					lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top - ptOffset.Y;
				}
				else if (tabAtom is SlidingViewofAtom)
				{
					SlidingViewofAtom scrollViewAtom = tabAtom as SlidingViewofAtom;

					Point ptOffset = scrollViewAtom.GetScrollOffset ();

					lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left - ptOffset.X;
					lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top - ptOffset.Y;
				}
				else if (tabAtom is AccordionViewofAtom)
				{
					AccordionViewofAtom accordionViewofAtom = tabAtom as AccordionViewofAtom;
					AccordionViewAttrib accordionViewAttrib = accordionViewofAtom.AtomCore.GetAttrib () as AccordionViewAttrib;

					double nTopMargin = accordionViewofAtom.GetTopMarginAccordionView ();
					double nLeftMargin = accordionViewAttrib.TabMargin;

					Point ptOffset = accordionViewofAtom.GetScrollOffset ();

					lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left + nLeftMargin - ptOffset.X;
					lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top + nTopMargin - ptOffset.Y;
				}
				else if (tabAtom is EBookQuizViewofAtom)
				{
					var ofAtom = tabAtom as EBookQuizViewofAtom;

					Point ptOffset = ofAtom.GetScrollOffset ();

					lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left - ptOffset.X;
					lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top - ptOffset.Y;
				}
				else if (tabAtom is TabViewAtomBase)
				{
					TabViewAtomBase tabViewAtomBase = tabAtom as TabViewAtomBase;

					if (Dock.Top == tabViewAtomBase.TabAlignment)
					{
						//2021-10-06 kys 탭뷰의 탭항목이 여러줄로 표시될경우를 고려하여 논리 보강함
						double dTabHeight = tabViewAtomBase.MainTabViewControl.GetTabThumbControlHeight ();

						lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left;
						lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top + dTabHeight;
					}
					else if (Dock.Left == tabViewAtomBase.TabAlignment)
					{
						lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left;
						lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top;
					}
					else
					{
						lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left;
						lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top;
					}
				}
				else
				{
					lastestAtomMargin.Left = lastestAtomMargin.Left + tabAtom.Margin.Left;
					lastestAtomMargin.Top = lastestAtomMargin.Top + tabAtom.Margin.Top;
				}
			}

			if (null != selectedAtom.GetScrollAtom ())
			{
				FrameworkElement scrollAtom = selectedAtom.GetScrollAtom ();
				lastestAtomMargin.Left = lastestAtomMargin.Left + scrollAtom.Margin.Left;
				lastestAtomMargin.Top = lastestAtomMargin.Top + scrollAtom.Margin.Top;
			}

			return lastestAtomMargin;
		}

		public Thickness AdjustMovePoint (Thickness atomMargin)
		{
			Thickness adjustedPoint = new Thickness ();
			DMTView currentDMTView = GetParentView () as DMTView;
			DMTFrame currentDMTFrame = currentDMTView.GetFrame () as DMTFrame; ;

			if (null == currentDMTView?.TileView)
				return atomMargin;

			double dConvertedTileSize = currentDMTView.TileView.GetPixelByMeasureType (-1);

			bool bChanged = false;

			adjustedPoint.Left = atomMargin.Left;
			adjustedPoint.Top = atomMargin.Top;


			if (true == Keyboard.IsKeyDown (Key.LeftShift)) //Left Shift를 누르고 이동한경우 격자의 영향을 받지 않는다.
			{
				bChanged = true;
			}

			if (false == bChanged && true == currentDMTView.IsTileModeOn)
			{
				//if (true == currentDMTView.IsTileModeOn &&
				//	true == Keyboard.IsKeyDown(Key.LeftCtrl) && false == Keyboard.IsKeyDown(Key.LeftShift))

				//2021-03-12 kys 격자 자석 논리 보강
				double dOffset = PQAppBase.SmartGuideRange;

				double dLeft = (atomMargin.Left + dConvertedTileSize) % dConvertedTileSize;
				double dLeft2 = (atomMargin.Left + atomMargin.Right + dConvertedTileSize) % dConvertedTileSize;

				double dTop = (atomMargin.Top + dConvertedTileSize) % dConvertedTileSize;
				double dTop2 = (atomMargin.Top + atomMargin.Bottom + dConvertedTileSize) % dConvertedTileSize;

				if (dOffset >= dLeft)
				{
					adjustedPoint.Left = ((int)(atomMargin.Left / dConvertedTileSize)) * dConvertedTileSize;
					bChanged = true;
				}
				else if (dConvertedTileSize - dOffset <= dLeft2)
				{
					double dTemp = (((int)((atomMargin.Left + atomMargin.Right) / dConvertedTileSize)) + 1) * dConvertedTileSize;
					adjustedPoint.Left = dTemp - atomMargin.Right;
					bChanged = true;
				}

				if (dOffset >= dTop)
				{
					adjustedPoint.Top = ((int)(atomMargin.Top / dConvertedTileSize)) * dConvertedTileSize;
					bChanged = true;
				}
				else if (dConvertedTileSize - dOffset <= dTop2)
				{
					double dTemp = (((int)((atomMargin.Top + atomMargin.Bottom) / dConvertedTileSize)) + 1) * dConvertedTileSize;
					adjustedPoint.Top = dTemp - atomMargin.Bottom;
					bChanged = true;
				}
			}
			else if (false == bChanged && true == currentDMTFrame.IsRulerModeOn)
			{
				if (true == currentDMTView.CheckeEvent_GetNearestGuidLineGap ())
				{
					Point ptGuideLineGap = currentDMTView.ExecuteGetNearestGuidLineGap (atomMargin);
					adjustedPoint.Left = atomMargin.Left + ptGuideLineGap.X;
					adjustedPoint.Top = atomMargin.Top + ptGuideLineGap.Y;
				}

				//0보다 작은경우 배치 안되도록 논리 보강 <-- 0보다 작게 배치하는경우가 있음
				//adjustedPoint.Left = 0 > adjustedPoint.Left ? 0 : adjustedPoint.Left;
				//adjustedPoint.Top = 0 > adjustedPoint.Top ? 0 : adjustedPoint.Top;
			}

			return adjustedPoint;
		}

		#region |  ##### Protected Override 메서드 #####  |

		protected override bool InitializeDocument ()
		{
			bool bIsSuccess = base.InitializeDocument ();

			//80 m_bLock = false;

			// <2003. 10. 15. cys> [Script]
			m_stnMaxIndex = 0;
			//m_pEditLink	  = null;  

			m_pScriptWindow = null;
			m_strDefaultTable = "";
			//	m_pCursorData	= new CursorData ();

			//********************************************************************//
			// <2003. 10. 15. cys> [Script]
			// 이 아래 부분은 스크립트를 위한 변수들을 초기화 하는 곳임돠..
			// 다른 변수들의 초기화는 이 윗부분에 해 주세용~~ ^^
			//********************************************************************//
			m_pRButtonAtom = null;
			///////////////////////////////////////////////////////////////////////

			m_bChangedTitle = false;
			m_bTabChange = false;

			m_strOldDefaultTable = "";

			//80m_bReferenceForm = false;
			m_pGDITable.SetDefaultGDIInfo ();
			m_pFrameAttrib = new CDMTFrameAttrib (this);

			return bIsSuccess;
		}

		protected override void DynamicGridSerialize (CArchive ar)
		{
			if (true == m_pFrameAttrib.IsDynamicMode)
			{
				DMTView dmtView = GetParentView () as DMTView;
				DMTFrame dmtFrame = dmtView.GetFrame () as DMTFrame;

				if (null != dmtView && null != dmtFrame)
				{
					if (true == ar.Stored)
					{
						//시리얼라이즈 동작전에 현재 크기를 저장한다.
						dmtFrame.CurrentPhoneScreenView.DynamicGrid.SaveGridData ();
					}
				}

				base.DynamicGridSerialize (ar);

				if (false == ar.Stored)
				{
					if (null != dmtView)
					{
						//폼 불러왔을때 반응형 그리드를 표시해준다.
						DynamicGridTable gridTable = dmtFrame?.CurrentPhoneScreenView.DynamicGrid as DynamicGridTable;

						if (null != gridTable)
						{
							dmtView.RefreshDynamicGrid ();
							gridTable.DeSerializeReplaceDesignAtom ();
						}

					}
				}
			}
		}

		protected override void JsonSaveWebService (bool IsSave)
		{
			if (IsSave)
			{
				string strFilePath = this.FilePath;
				string strExtension = Path.GetExtension (this.FilePath);
				strFilePath = strFilePath.Replace (strExtension, ".QWS");

				if (null == m_WebServiceDoc)
					m_WebServiceDoc = new ScriptWebServiceDoc ();

				if (null != m_pScriptWindow && false == string.IsNullOrEmpty (m_pScriptWindow.WebServiceSourceText))
				{
					m_WebServiceDoc.DocType = DOC_KIND._docScriptWebService;
					m_WebServiceDoc.SourceText = m_pScriptWindow.WebServiceSourceText;
					m_WebServiceDoc.FilePath = strFilePath;
					m_WebServiceDoc.OnSaveJsonDocument (strFilePath);
				}
			}

			base.JsonSaveWebService (IsSave);
		}

		protected override void SaveWebServiceSerialize (bool isStored)
		{
			if (true == isStored)
			{
				string strFilePath = this.FilePath;
				string strExtension = Path.GetExtension (this.FilePath);
				strFilePath = strFilePath.Replace (strExtension, ".QWS");

				if (null == m_WebServiceDoc)
					m_WebServiceDoc = new ScriptWebServiceDoc ();

				if (null != m_pScriptWindow && false == string.IsNullOrEmpty (m_pScriptWindow.WebServiceSourceText))
				{
					m_WebServiceDoc.DocType = DOC_KIND._docScriptWebService;
					m_WebServiceDoc.SourceText = m_pScriptWindow.WebServiceSourceText;
					m_WebServiceDoc.FilePath = strFilePath;
					m_WebServiceDoc.OnSaveDocument (strFilePath);
				}
			}

			base.SaveWebServiceSerialize (isStored);
		}

		#endregion

		#region |  ##### Protected 메서드 #####  |

		#region |  ----- 업무규칙 -----  |

		protected bool CreateScriptEdit80 ()
		{
			DropScriptEdit80 (true);

			m_pScriptWindow = new TopEdit80.CScrFrame (this.m_pScriptServer.IsEBLScriptSource ());

			//if (null == m_pScriptWindow)
			//{
			//	_Exception.ThrowEmptyException();
			//}

			m_pScriptWindow.CreateFrame (Application.Current.MainWindow);
			m_pScriptWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			m_pScriptWindow.Hide ();
			m_pScriptWindow.Closed += ScriptWindow_Closed;
			m_pScriptWindow.OnAtomClick += ScriptWindow_OnAtomClick;

			return true;
		}

		private void ScriptWindow_OnAtomClick (string strValue)
		{
			List<Atom> atomCoreList = GetAllAtomCores ();

			if (null != atomCoreList)
			{
				Atom atomCore = atomCoreList.Where (item => strValue == item.GetProperVar ()).FirstOrDefault ();

				if (null != atomCore)
				{
					atomCore.GetOfAtom ().SelectAtom ();
				}
			}
		}

		#endregion

		#endregion


		#region |  ##### 이벤트 #####  |

		protected void Atom_OnNotifyMoveCompletedEvent ()
		{
			try
			{

				/// Adorner 테두리 클릭한 채로 이동 후 마우스 업 시 이벤트 발생
				/// 아톰 내부를 클릭한 채로 이동 후 마우스 업 이벤트는 HandleView_PreviewMouseUp 재정의 함수에서 발생됨
				var dmtView = GetParentView () as DMTView;
				dmtView.ExecuteMouseUp ();
			}
			catch (Exception ex)
			{
				Trace.TraceError (ex.ToString ());
			}
		}

		void Atom_OnNotifyReSizeEvent (object obj, object pre, object direction)
		{
			OnNotifyReSizeEvent?.Invoke (obj, pre, direction);

            try
            {
                var dmtView = GetParentView () as DMTView;

                dmtView.ResizeElement (obj as FrameworkElement, (Thickness)pre, (int)direction);
            }
            catch (Exception ex)
            {
                Trace.TraceError (ex.ToString ());
            }
        }

        /// <summary>
        /// Adorner 를 드래그할 때 발생되는 이벤트
        /// </summary>
        private void Atom_OnNotifyMoveEvent ()
		{
			try
			{
				var dmtView = GetParentView () as DMTView;
				dmtView.ExecuteDragMove ();
			}
			catch (Exception ex)
			{
				Trace.TraceError (ex.ToString ());
			}
		}

		private void Atom_OnNotifyChangeAttribEvent (object objValue, object objValue_t)
		{
			if (null == objValue_t)
				return;

			// objValue ( attirb ) , objValue_t ( AtomBase )
			ChangeAtomAttribCommand NewChangeAtomAttribCommand = new ChangeAtomAttribCommand ((AtomBase)objValue_t, objValue);
			Commander.AddCommand (NewChangeAtomAttribCommand);
			Commander.ExecuteCommand ();

			RefreshAtomEditProperty ();
			RefreshFlowMapProperty ();
		}

		private void Atom_AtomDoubleClickedEvent (object objValue)
		{
			try
			{
				var dmtView = GetParentView () as DMTView;
				var dmtFrame = dmtView.GetFrame () as DMTFrame;

				dmtFrame.OnAtomDoubleClickedEvent (objValue as AtomBase);
			}
			catch (Exception ex)
			{
				Trace.TraceError (ex.ToString ());
			}
		}

		private bool ScrollAtom_OnNotifyReleaseBindedAtomsEvent (FrameworkElement SourceScrollAtom)
		{
			if (null != SourceScrollAtom)
			{
				ScrollAtomBase ScrollAtom = SourceScrollAtom as ScrollAtomBase;
				return TryAttachAtomAtView (ScrollAtom);
			}

			return false;
		}

		private void HandleAtomClickedEvent (object sender)
		{
			AtomBase CurrentClickedAtom = (AtomBase)sender;

			if (null != WebDynamicGrid && true == WebDynamicGrid.IsSelectItem)
			{
				WebDynamicGrid.EexecuteLostFocus ();
			}

			if (null != OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent)
			{
				FrameworkElement OwnerElement = CurrentClickedAtom.GetOwnerView ();

				if (null != OwnerElement)
				{
					if (OwnerElement is TopView)
					{

					}
					else if (OwnerElement is TabViewAtomBase)
					{
						TabViewAtomBase TabViewAtom = OwnerElement as TabViewAtomBase;
						TabViewAtom.HandleAtomClicked (CurrentClickedAtom);
					}
					else if (OwnerElement is ScrollAtomBase)
					{
						ScrollAtomBase ScrollAtom = OwnerElement as ScrollAtomBase;
						ScrollAtom ScrollCore = ScrollAtom.AtomCore as ScrollAtom;
						ScrollCore.HandleAtomClicked (CurrentClickedAtom);
					}
					else if (OwnerElement is WebReplaceDesignofAtom WebReplaceDesingAtom)
					{
					}
				}

				DMTView dmtView = GetParentView () as DMTView;

				if (null != dmtView)
				{
					bool isShift = Keyboard.IsKeyDown (Key.LeftShift) || Keyboard.IsKeyDown (Key.RightShift);

					if (false == isShift)
					{
						List<AtomBase> selectAtomList = GetCurrentSelectedAtoms ();
						selectAtomList.Remove (CurrentClickedAtom);

						foreach (AtomBase atom in selectAtomList)
						{
							atom.SetResizeAdornerVisibility (Visibility.Collapsed, true);
						}
					}
				}

				int nCurrentSelectedAtomCount = GetCurrentSelectedAtomsCount ();
				if (1 == nCurrentSelectedAtomCount)
				{
					ToolBarProperty toolbarProperty = GetAtomProperties (CurrentClickedAtom);
					OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent (toolbarProperty);

					// 아톰선택 상태 변경 시 화면잠금 툴바 상태 업데이트될 수 있도록 보강
					DMTFrame currentFrame = dmtView?.GetFrame () as DMTFrame;
					GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyFrameModeChanged (currentFrame);
				}
			}

			CurrentClickedAtom = null;
		}

		private void StructDataMgrFrame_Closed (object sender, EventArgs e)
		{
			if (null != m_pStructDataMgrFrame)
			{
				int totalCount = m_pStructDataMgrFrame.Count;

				for (int index = 0; index < totalCount; index++)
				{

				}

				if (null != m_pStructDataMgrFrame.Owner)
				{
					m_pStructDataMgrFrame.Owner.Focus ();
				}
				m_pStructDataMgrFrame = null;
			}
		}

		/// <summary>
		/// AtomName.BROWSE, AtomName.COMBOBOX, AtomName.POPUP, AtomName.SCROLL
		/// </summary>
		/// <param name="arrBrowseItemInfo"></param>
		/// <param name="strAtomType"></param>
		private void StructDataMgrFrame_OnBrowseAtomUpdate (ArrayList arrBrowseItemInfo, string strAtomType)
		{
			AutoCreateManager.Instance.BrowseAtomUpdate (this, arrBrowseItemInfo, strAtomType);
		}

		/// <summary>
		/// SAP 에서 자동으로 (사각형 + 입력란) 아톰 생성작업
		/// </summary>
		/// <param name="arrInputAtomInfo"></param>
		/// <param name="strInputAtomType"></param>
		private void StructDataMgrFrame_OnMakeInputAtom (ArrayList arrInputAtomInfo, string strAtomType)
		{
			bool bGroupIgnore = true;
			//if (string.Equals (strAtomType, "Import") || string.Equals (strAtomType, "Export"))	// SAP RFC 와 OData 구분용
			//if (string.Equals (strAtomType, "Export")) // SAP RFC 와 OData 구분용
			//	bGroupIgnore = false;

			AutoCreateManager.Instance.MakeInputAtom (this, arrInputAtomInfo, bGroupIgnore, true);
		}

		private object AnimationWindow_OnGetAnimationManager ()
		{
			return AnimationManager;
		}

		private object AnimationWindow_OnGetAllAtomList ()
		{
			return GetAllAtomCores ();
		}

		private object AnimationWindow_OnMakeAnimationGroup ()
		{
			return MakeAnimationGroupAtom ();
		}

		private object AnimationWindow_OnDeleteAnimationGroup (object objParam)
		{
			return DeleteAnimationGroupAtom (objParam);
		}

		#region |  ----- 업무규칙 -----  |

		private void ScriptWindow_Closed (object sender, EventArgs e)
		{
			m_pScriptWindow = null;
		}

		#endregion

		#endregion

		#region |  ##### Public Override 메서드 #####  |

		public override object GetScriptWindowObject ()
		{
			return ScriptWindow;
		}

		public override JObject GetJsonData ()
		{
			ScriptSerialSetting (); // 스크립트 정보 설정...

			AutoSaveErdDoc (); // Erd 정보 저장..

			// 2013-11-29-C02 : Web인 경우 플러그 활성.
			if (IsEnableWebPage ())
			{
				//__IsWebPage : 2 2022-03-02 kys 반응형웹관련 아톰별 ObjectStyleAttCore 클래스에 시리얼라이즈 추가됨
				JsonHelper.Instance.WebVersion = 2;
			}

			return base.GetJsonData ();
		}

		public override void LoadJsonData (JToken jsonData)
		{
			base.LoadJsonData (jsonData);
		}

		public override bool EndJsonLoad ()
		{
			base.EndJsonLoad ();
			EndSerializeQuizMaker ();
			return true;
		}

		public override bool PrepareSerialize (CArchive ar)
		{
			base.PrepareSerialize (ar);

			if (DocType == DOC_KIND._docQuizMaker && ar.Stored)
			{
				var quizMakerMetaData = PageMetadata.QuizMetaData;
				PrepareSerializeQuizMaker ();

				//2024-07-22 kys 시리얼라이즈를 통해 저장될때 불필요한 format값은 저장하지 않도록 초기화 처리한다.
				//if (null != quizMakerMetaData?.QuizContentInfo)
				//	quizMakerMetaData.QuizContentInfo.Format = "";
			}

			return true;
		}

		public override void Serialize (CArchive ar)
		{
			if (false != ar.Stored) // 저장일 경우..
			{
				ScriptSerialSetting (); // 스크립트 정보 설정...

				AutoSaveErdDoc (); // Erd 정보 저장..

				// 2013-11-29-C02 : Web인 경우 플러그 활성.
				if (IsEnableWebPage ())
				{
					//__IsWebPage : 2 2022-03-02 kys 반응형웹관련 아톰별 ObjectStyleAttCore 클래스에 시리얼라이즈 추가됨
					ar.Set_Version ("__IsWebPage", 2);
				}

				//80 작업계속
				//if (6 == this.GetNtoaDocKind() || 0 == this.GetNtoaDocKind()) // //	0:QPG, 1:NTOA, 2:EDMS, 3:KMS, 5:QWP, 6:QPS (SMART FORM)
				//{
				//    DMTView pDMTView = this.GetParentView() as DMTView;

				//    if (null != pDMTView)
				//    {
				//        pDMTView.PrepareVirtualViewSize();
				//    }
				//}
			}

			base.Serialize (ar);



			//80
			//if (false == ar.Stored)
			//{
			//    CheckFormPassword();
			//}
		}

		public override bool EndSerialize (CArchive ar)
		{
			if (DocType == DOC_KIND._docQuizMaker && false == ar.Stored)
			{
				EndSerializeQuizMaker ();
			}

			base.EndSerialize (ar);

			return true;
		}

		public override void AttachAtomAtView (AtomBase atom, AttachAtomEventDefine.AttachAtomEventType EventType, bool bIsSerialize)
		{
			base.AttachAtomAtView (atom, EventType, bIsSerialize);

			atom.AtomClickedEvent -= HandleAtomClickedEvent;
			atom.ShowAtomContextMenuEvent -= HandleShowAtomContextMenuEvent;
			atom.OnNotifyCommandEvent -= HandleNotifyCommandEvent;
			atom.AtomDoubleClickedEvent -= Atom_AtomDoubleClickedEvent;
			atom.OnNotifyChangeAttribEvent -= Atom_OnNotifyChangeAttribEvent;
			atom.OnNotifyMoveEvent -= Atom_OnNotifyMoveEvent;
			atom.OnNotifyMoveCompletedEvent -= Atom_OnNotifyMoveCompletedEvent;
			atom.OnNotifyReSizeEvent -= Atom_OnNotifyReSizeEvent;

			atom.AtomClickedEvent += HandleAtomClickedEvent;
			atom.ShowAtomContextMenuEvent += HandleShowAtomContextMenuEvent;
			atom.OnNotifyCommandEvent += HandleNotifyCommandEvent;
			atom.AtomDoubleClickedEvent += Atom_AtomDoubleClickedEvent;
			atom.OnNotifyChangeAttribEvent += Atom_OnNotifyChangeAttribEvent;

			//2014-10-31-M01 에디터 모드에서 이동 처리
			atom.OnNotifyMoveEvent += Atom_OnNotifyMoveEvent;
			atom.OnNotifyMoveCompletedEvent += Atom_OnNotifyMoveCompletedEvent;
			atom.OnNotifyReSizeEvent += Atom_OnNotifyReSizeEvent;
		}

		public override void AttachAtomAtTabView (AtomBase sourceAtom, TabViewAtomBase targetTabView, bool bIsSerialize, int nPageIndex)
		{
			base.AttachAtomAtTabView (sourceAtom, targetTabView, bIsSerialize, nPageIndex);

			sourceAtom.AtomClickedEvent -= HandleAtomClickedEvent;
			sourceAtom.ShowAtomContextMenuEvent -= HandleShowAtomContextMenuEvent;
			sourceAtom.OnNotifyCommandEvent -= HandleNotifyCommandEvent;
			sourceAtom.OnNotifyMoveEvent -= Atom_OnNotifyMoveEvent;
			sourceAtom.OnNotifyReSizeEvent -= Atom_OnNotifyReSizeEvent;

			sourceAtom.AtomClickedEvent += HandleAtomClickedEvent;
			sourceAtom.ShowAtomContextMenuEvent += HandleShowAtomContextMenuEvent;
			sourceAtom.OnNotifyCommandEvent += HandleNotifyCommandEvent;
		}

		/// <summary>
		/// LightDMTView와 TabView와 Scroll을 통틀어 현재 선택된 아톰의 숫자를 리턴
		/// </summary>
		/// <returns></returns>
		public override int GetCurrentSelectedAtomsCount ()
		{
			List<AtomBase> selectAtomList = GetCurrentSelectedAtoms ();
			return selectAtomList.Count;

			//List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInLightDMTView();
			//int nCurrentSelectedAtomsCount = lstCurrentSelectedAtoms.Count;

			//if (1 <= nCurrentSelectedAtomsCount)
			//{
			//	return nCurrentSelectedAtomsCount;
			//}
			//else
			//{
			//	UIElementCollection CurrentChildren = GetChildren();
			//	int nChildrenCount = CurrentChildren.Count;

			//	for (int nIndex = 0; nIndex < nChildrenCount; nIndex++)
			//	{
			//		AtomBase CurrentAtom = CurrentChildren[nIndex] as AtomBase;
			//		Type CurrentAtomType = CurrentAtom.GetType();

			//		if (CurrentAtom is TabViewAtomBase)
			//		{
			//			TabViewAtomBase TabViewAtom = CurrentAtom as TabViewAtomBase;
			//			int nSelectedChildAtomCount = TabViewAtom.GetCurrentSelectedChildAtomCount();

			//			if (1 <= nSelectedChildAtomCount)
			//			{
			//				return nSelectedChildAtomCount;
			//			}
			//			else
			//			{
			//				List<AtomBase> CurrentTabPageChildren = TabViewAtom.GetChildrenInCurrentTabPage();

			//				if (null != CurrentTabPageChildren)
			//				{
			//					foreach (AtomBase atom in CurrentTabPageChildren)
			//					{
			//						if (atom is ScrollAtomBase)
			//						{
			//							ScrollAtomBase ScrollAtom = atom as ScrollAtomBase;
			//							CScrollable ScrollCore = ScrollAtom.AtomCore as CScrollable;

			//							int nSelectedChildAtomCountInScroll = ScrollCore.GetCurrentSelectedChildAtomCount();

			//							if (1 <= nSelectedChildAtomCountInScroll)
			//							{
			//								return nSelectedChildAtomCountInScroll;
			//							}

			//							ScrollAtom = null;
			//						}
			//					}
			//				}
			//			}

			//			TabViewAtom = null;
			//		}
			//		else if (CurrentAtom is ScrollAtomBase)
			//		{
			//			ScrollAtomBase ScrollAtom = CurrentAtom as ScrollAtomBase;
			//			CScrollable ScrollCore = ScrollAtom.AtomCore as CScrollable;

			//			int nSelectedChildAtomCount = ScrollCore.GetCurrentSelectedChildAtomCount();

			//			if (1 <= nSelectedChildAtomCount)
			//			{
			//				return nSelectedChildAtomCount;
			//			}

			//			ScrollAtom = null;
			//		}

			//		CurrentAtom = null;
			//	}

			//	CurrentChildren = null;
			//}

			//lstCurrentSelectedAtoms = null;

			//return 0;
		}

		public override List<object> GetCurrentSelectedAtomsObject ()
		{
			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtoms ();

			if (0 == lstCurrentSelectedAtoms.Count)
			{
				//대체디자인 고려
				if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode)
				{
					lstCurrentSelectedAtoms = WebDynamicGrid.RootDesignGrid.GetSelectAtomList ();
					return lstCurrentSelectedAtoms.Cast<object> ().ToList ();
				}
			}

			List<object> CurrentSelectedAtoms = new List<object> ();

			foreach (object atom in lstCurrentSelectedAtoms)
			{
				CurrentSelectedAtoms.Add (atom);
			}

			return CurrentSelectedAtoms;
		}

		public override int GetCurrentSelectedAtomsInLightDMTViewCount ()
		{
			return GetCurrentSelectedAtomsInLightDMTView ().Count;
		}

		public override bool ApplyAtomNameObject (List<object> selectedAtoms, string strAtomName)
		{
			List<AtomBase> ListselectedAtoms = new List<AtomBase> ();

			foreach (AtomBase atoms in selectedAtoms)
			{
				ListselectedAtoms.Add (atoms);
			}

			return ApplyAtomName (ListselectedAtoms, strAtomName);
		}

		public override void ApplyAtomNameFieldType (List<object> selectedAtoms, string strAtomFieldType)
		{
			List<AtomBase> ListselectedAtoms = new List<AtomBase> ();

			foreach (AtomBase atoms in selectedAtoms)
			{
				ListselectedAtoms.Add (atoms);
			}

			ApplyAtomFieldType (ListselectedAtoms, strAtomFieldType);
		}

		public override void SetAttribToAtom (ref Atom _TargetAtom, object objType)
		{
			AtomType _TargetType = _TargetAtom.GetUniqueEnumType ();
			DMTView pView = this.GetParentView () as DMTView;
			SmartAtomCoreManager _ConvertManager = null;

			switch (_TargetType)
			{
				case AtomType.Advertise:
				case AtomType.AdContents:
					_ConvertManager = new SmartAdvertiseManager ();
					break;
				case AtomType.Checkbox:
				case AtomType.ToggleSwitch:
				case AtomType.RatingBar:
					_ConvertManager = new SmartCheckManager ();
					break;
				case AtomType.AnalogClock:
				case AtomType.DigitalClock:
					_ConvertManager = new SmartClockManager ();
					break;
				case AtomType.QRCode:
				case AtomType.BarCode:
					_ConvertManager = new SmartCodeManager ();
					break;
				case AtomType.DecorImage:
				case AtomType.AnimationImage:
					_ConvertManager = new SmartDecorImageManager ();
					break;
				case AtomType.DataGrid:
				case AtomType.StructureGrid:
					_ConvertManager = new SmartGridAtomManager ();
					break;
				case AtomType.Scan:
				case AtomType.WebPicture:
					_ConvertManager = new SmartImageManager ();
					break;
				case AtomType.DataInput:
				case AtomType.InputSpinner:
				case AtomType.AutoComplete:
				case AtomType.EBookGridPaper:
					_ConvertManager = new SmartInputManager ();
					break;
				case AtomType.Map:
				case AtomType.GoogleMap:
				case AtomType.NaverMap:
				case AtomType.KakaoMap:
				case AtomType.TMap:
					_ConvertManager = new SmartMapViewManager ();
					break;
				case AtomType.Media:
				case AtomType.YouTube:
					_ConvertManager = new SmartMediaManager ();
					break;
				case AtomType.InAppBilling:
				case AtomType.WebPG:
				case AtomType.WebKakaoPay:
				case AtomType.WebSoftBank:
				case AtomType.PaddleInApp:
				case AtomType.Paypal:
				case AtomType.IamPort:
					_ConvertManager = new SmartPaymentManager ();
					break;
				case AtomType.Progress:
				case AtomType.VerticalProgress:
				case AtomType.RadialProgress:
					_ConvertManager = new SmartProgressManager ();
					break;
				case AtomType.RangeSlider:
				case AtomType.VerticalRangeSlider:
				case AtomType.RadialRangeSlider:
					_ConvertManager = new SmartRangeSliderManager ();
					break;
				case AtomType.BindScroll:
				case AtomType.BindMatrixScroll:
					_ConvertManager = new SmartScrollManager ();
					break;
				case AtomType.Slider:
				case AtomType.VerticalSlider:
				case AtomType.RadialSlider:
					_ConvertManager = new SmartSliderManager ();
					break;
				case AtomType.TabView:
				case AtomType.ExpandView:
				case AtomType.ScrollView:
				case AtomType.SlidingView:
				case AtomType.AccordionView:
					_ConvertManager = new SmartTabViewManager ();
					break;
				case AtomType.DataTree:
				case AtomType.StructureTree:
					_ConvertManager = new SmartTreeManager ();
					break;
				case AtomType.WebMenu:
				case AtomType.WebSlideMenu:
				case AtomType.WebCoverFlowMenu:
				case AtomType.WebDropMenu:
				case AtomType.WebTabMenu:
				case AtomType.WebLabelMenu:
				case AtomType.WebLinkMenu:
				case AtomType.WebTreeMenu:
				case AtomType.WebComboMenu:
				case AtomType.RadialMenu:
					_ConvertManager = new SmartWebMenuManager ();
					break;
				default:
					break;
			}

			AtomConvertCommand command = new AtomConvertCommand (_ConvertManager, _TargetAtom, pView, objType);
			Commander.AddCommand (command);
			_TargetAtom = Commander.ExecuteCommand () as Atom;
		}

		public override void SetAttribScrollAtom (ref Atom scroll)
		{
			DMTView pView = this.GetParentView () as DMTView;
			Type scrollType = scroll.GetType ();

			if (typeof (ScrollAtom) == scrollType)
			{
				SmartScrollManager.SetAttribScrollAtom (ref scroll, pView, 0);
			}
			else if (typeof (MatrixScrollAtom) == scrollType)
			{
				SmartScrollManager.SetAttribScrollAtom (ref scroll, pView, 1);
			}
		}

		public override void SetAttribMenuAtom (ref Atom webMenu, MenuType menuType)
		{
			// 6개의 웹메뉴를 1개의 메뉴로 통합하기 위한 작업.
			if (webMenu is WebMenuAtom && menuType != MenuType.RadialMenu)
			{
				WebMenuAttrib pSourceAttrib = webMenu.GetAttrib () as WebMenuAttrib;
				if (pSourceAttrib.GetMenuType () != (int)pSourceAttrib.MenuType)
				{
					DMTView pView = this.GetParentView () as DMTView;
					SmartWebMenuManager.SetAttribMenuAtom (ref webMenu, pView, menuType);

					if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode)
					{
						WebReplaceDesignofAtom designAtom = WebDynamicGrid.RootDesignGrid.DesignAtom;

						if (null != designAtom)
						{
							designAtom.ConvertAtomCore (webMenu);
						}
					}
				}
			}
			else
			{
				DMTView pView = this.GetParentView () as DMTView;
				SmartWebMenuManager.SetAttribMenuAtom (ref webMenu, pView, menuType);
			}
		}

		public override void SetAttribAdvertiseAtom (ref Atom pAtom)
		{
			// AdMob, AdContents 통합
			DMTView pView = this.GetParentView () as DMTView;

			if (pAtom is AdvertiseAtom)
			{
				SmartAdvertiseManager.SetAttribAdvertiseAtom (ref pAtom, pView, 0);
			}
			else if (pAtom is AdContentsAtom)
			{
				SmartAdvertiseManager.SetAttribAdvertiseAtom (ref pAtom, pView, 1);
			}
		}

		public override void SetAttribPaymentAtom (ref Atom pAtom)
		{
			// PGAtom, InAppBilling 통합
			DMTView pView = this.GetParentView () as DMTView;

			PaymentBaseAttrib pAttrib = pAtom.GetAttrib () as PaymentBaseAttrib;
			SmartPaymentManager.SetAttribPaymentAtom (ref pAtom, pView, pAttrib.PaymentType);
		}

		public override void SetAttribMapViewAtom (ref Atom mapView)
		{
			// 4개의 지도아톰을 1개의 지도아톰으로 통합하기 위한 작업.
			if (mapView is MapViewAtom)
			{
				MapViewAttrib pSourceAttrib = mapView.GetAttrib () as MapViewAttrib;
				if (pSourceAttrib.GetMapViewKind () != pSourceAttrib.MapViewKind)
				{
					DMTView pView = this.GetParentView () as DMTView;
					SmartMapViewManager.SetAttribMapViewAtom (ref mapView, pView, pSourceAttrib.MapViewKind);
				}
			}
		}

		public override void SetAttribGridAtom (ref Atom pAtom)
		{
			DMTView pView = this.GetParentView () as DMTView;

			if (pAtom is DBGridExAtom)
			{
				DBGridExAttrib pAttrib = pAtom.GetAttrib () as DBGridExAttrib;
				SmartGridAtomManager.SetAttribGridAtom (ref pAtom, pView, pAttrib.ParameterMode);
			}
			else if (pAtom is StructureGridAtom)
			{
				StructureGridAttrib pAttrib = pAtom.GetAttrib () as StructureGridAttrib;
				SmartGridAtomManager.SetAttribGridAtom (ref pAtom, pView, pAttrib.ParameterMode);
			}
		}

		public override void SetAttribDecorImageAtom (ref Atom pAtom)
		{
			DMTView pView = this.GetParentView () as DMTView;

			DecorImageAttrib pAttrib = pAtom.GetAttrib () as DecorImageAttrib;
			SmartDecorImageManager.SetAttribDecorImageoAtom (ref pAtom, pView, pAttrib.ImageType);
		}

		public override void SetAttribTreeAtom (ref Atom pAtom, int nTreeType)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartTreeManager.SetAttribTreeAtom (ref pAtom, pView, nTreeType);
		}

		public override void SetAttribTabViewAtom (ref Atom pAtom, int nTabViewType)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartTabViewManager.SetAttribTabViewAtom (ref pAtom, pView, nTabViewType);
		}

		public override void SetAttribProgressAtom (ref Atom pAtom, int nProgressType)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartProgressManager.SetAttribProgressAtom (ref pAtom, pView, nProgressType);
		}

		public override void SetAttribSliderAtom (ref Atom pAtom, int nSliderType)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartSliderManager.SetAttribSliderAtom (ref pAtom, pView, nSliderType);
		}

		public override void SetAttribRangeSliderAtom (ref Atom pAtom, int nSliderType)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartRangeSliderManager.SetAttribRangeSliderAtom (ref pAtom, pView, nSliderType);
		}

		public override void SetAttribClockAtom (ref Atom pAtom, int nClockType)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartClockManager.SetAttribClockAtom (ref pAtom, pView, nClockType);
		}

		public override void SetAttribCodeAtom (ref Atom pAtom, int nClockType)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartCodeManager.SetAttribCodeAtom (ref pAtom, pView, nClockType);
		}

		public override void SetAttribMediaAtom (ref Atom pAtom, int nPlayerKind)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartMediaManager.SetAttribMediaAtom (ref pAtom, pView, nPlayerKind);
		}

		public override void SetAttribCheckAtom (ref Atom pAtom, int nCheckType)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartCheckManager.SetAttribCheckAtom (ref pAtom, pView, nCheckType);
		}

		public override void SetAttribInputAtom (ref Atom pAtom, int nInputType)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartInputManager.SetAttribInputAtom (ref pAtom, pView, nInputType);
		}

		public override void SetAttribImageAtom (ref Atom pAtom, int nImageKind)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartImageManager.SetAttribImageAtom (ref pAtom, pView, nImageKind);
		}

		public override void SetAttribFileAttachAtom (ref Atom pAtom, int nImageKind)
		{
			DMTView pView = this.GetParentView () as DMTView;

			SmartFileAttachManager.SetAttribFileAttachAtom (ref pAtom, pView, nImageKind);
		}

		public override bool IsFormChange ()
		{
			return m_bFormChange;
		}

		/// <summary>
		/// Ctrl + C 눌렀을 경우 전역클립보드의 데이터캐시에 임시 저장.
		/// </summary>
		public override void ReadyDeepCloneAtoms ()
		{
			try
			{
				List<AtomBase> CurrentSelectedAtoms = GetCurrentSelectedAtoms ();
				DMTView CurrentDMTView = GetParentView () as DMTView;

				if (0 == CurrentSelectedAtoms.Count || null == CurrentDMTView)
				{
					GlobalClipboard.CacheData = null;
					return;
				}

				GlobalClipboard.CacheData = CurrentSelectedAtoms.OrderBy (item => Grid.GetZIndex (item)).ToList ();

				DataObject dataObject = new DataObject ();
				dataObject.SetData ("Atom", true);
				//클립보드 셋팅논리 필요 캡처 논리도 필요할듯

				if (1 == CurrentSelectedAtoms.Count)
				{
					AtomBase baseAtom = CurrentSelectedAtoms[0];
					dataObject.SetData ("AtomName", baseAtom.AtomCore.GetProperVar (true));

					if (baseAtom is SquareofAtom)
					{
						SquareofAtom ofAtom = CurrentSelectedAtoms[0] as SquareofAtom;
						SquareAttrib atomAttrib = ofAtom.AtomCore.GetAttrib () as SquareAttrib;

						string strText = atomAttrib.Title;
						dataObject.SetData (DataFormats.Text, strText);
					}
					else if (baseAtom is EBookTextofAtom)
					{
						EBookTextofAtom ofAtom = CurrentSelectedAtoms[0] as EBookTextofAtom;
						EBookTextAttrib atomAttrib = ofAtom.AtomCore.GetAttrib () as EBookTextAttrib;

						TextRange tr = ofAtom.GetTextRange ();
						string strText = string.Empty;
						using (MemoryStream ms = new MemoryStream ())
						{
							tr.Save (ms, DataFormats.Rtf);
							strText = System.Text.Encoding.ASCII.GetString (ms.ToArray ());
						}

						dataObject.SetData (DataFormats.Rtf, strText);
						dataObject.SetData (DataFormats.Text, ofAtom.GetText ());
					}
					else if (baseAtom is WebHtmlTagofAtom)
					{
						WebHtmlTagofAtom ofAtom = CurrentSelectedAtoms[0] as WebHtmlTagofAtom;
						WebHtmlTagAttrib atomAttrib = ofAtom.AtomCore.GetAttrib () as WebHtmlTagAttrib;

						string strText = atomAttrib.HtmlBody;
						dataObject.SetData (DataFormats.Html, strText);
					}

					BitmapSource bitmapSource = Softpower.SmartMaker.TopControl.Components.ScreenCapture.ScreenCaptureHelper.GetRenderViewImage (CurrentDMTView);

					//AwayFromZero를 명시적으로 선언하지 않으면 0.5가 버려지는 경우가 생김
					int nX = (int)Math.Round (baseAtom.Margin.Left, 0, MidpointRounding.AwayFromZero);
					int nY = (int)Math.Round (baseAtom.Margin.Top, 0, MidpointRounding.AwayFromZero);
					int nWidth = (int)Math.Round (baseAtom.ActualWidth, 0, MidpointRounding.AwayFromZero);
					int nHeight = (int)Math.Round (baseAtom.ActualHeight, 0, MidpointRounding.AwayFromZero);

					if (nX + nWidth > CurrentDMTView.ActualWidth)
					{
						nWidth = (int)(CurrentDMTView.ActualWidth - nX);
					}

					if (nY + nHeight > CurrentDMTView.ActualHeight)
					{
						nHeight = (int)(CurrentDMTView.ActualHeight - nY);
					}

					CroppedBitmap croppedBitmap = new CroppedBitmap (bitmapSource, new Int32Rect (nX, nY, nWidth, nHeight));

					dataObject.SetData (DataFormats.Bitmap, croppedBitmap);
					dataObject.SetData (DataFormats.Text, baseAtom.AtomCore.GetProperVar ());
				}
				else
				{
					//이미지
					BitmapSource bitmapSource = Softpower.SmartMaker.TopControl.Components.ScreenCapture.ScreenCaptureHelper.GetRenderViewImage (CurrentDMTView);

					int nViewX = int.MaxValue;
					int nViewY = int.MaxValue;
					int nViewWidth = 0;
					int nViewHeight = 0;

					foreach (AtomBase ofAtom in CurrentSelectedAtoms)
					{
						int nX = (int)Math.Round (ofAtom.Margin.Left, 0, MidpointRounding.AwayFromZero);
						int nY = (int)Math.Round (ofAtom.Margin.Top, 0, MidpointRounding.AwayFromZero);
						int nWidth = (int)Math.Round (ofAtom.ActualWidth, 0, MidpointRounding.AwayFromZero);
						int nHeight = (int)Math.Round (ofAtom.ActualHeight, 0, MidpointRounding.AwayFromZero);

						if (nViewX > nX)
						{
							nViewX = nX;
						}

						if (nViewY > nY)
						{
							nViewY = nY;
						}

						if (nViewWidth < (nX - nViewX) + nWidth)
						{
							nViewWidth = (nX - nViewX) + nWidth;
						}

						if (nViewHeight < (nY - nViewY) + nHeight)
						{
							nViewHeight = (nY - nViewY) + nHeight;
						}

					}

					if (0 > nViewX)
					{
						nViewX = 0;
					}

					if (0 > nViewY)
					{
						nViewY = 0;
					}

					if (nViewX + nViewWidth > CurrentDMTView.ActualWidth)
					{
						nViewWidth = (int)(CurrentDMTView.ActualWidth - nViewX);
					}

					if (nViewY + nViewHeight > CurrentDMTView.ActualHeight)
					{
						nViewHeight = (int)(CurrentDMTView.ActualHeight - nViewY);
					}

					CroppedBitmap croppedBitmap = new CroppedBitmap (bitmapSource, new Int32Rect (nViewX, nViewY, nViewWidth, nViewHeight));

					dataObject.SetData (DataFormats.Bitmap, croppedBitmap);
				}

				Clipboard.SetDataObject (dataObject);
			}
			catch (Exception ex)
			{
				Trace.TraceError (ex.Message);
				LogManager.WriteLog (ex);
			}
		}

		public override void ProcessDeepCloneAtoms (int BasePositionType = 0)
		{
			//Ctrl + V : 현재 마우스 커서 위치로 아톰 붙여넣기
			//Ctrl + Shift + V :
			//1. 선택된 아톰이 없는경우 원본 아톰 위치로 붙여넣기, 동일한 자리에 아톰이 있는경우 +5씩 이동해서 붙여넣는다
			//2. 선택된 아톰이 있는경우 복사된 아톰의 서식을 붙여넣기 한다.

			object CacheData = GlobalClipboard.CacheData;

			if (null != CacheData)
			{
				List<AtomBase> DeepCloneSourceAtoms = CacheData as List<AtomBase>;
				List<AtomBase> selectAtomList = GetCurrentSelectedAtoms ();
				DMTView currentDMTView = GetParentView () as DMTView;

				if (0 < selectAtomList.Count && 1 == DeepCloneSourceAtoms.Count && 0 == selectAtomList.Count (item => item is TabViewAtomBase))
				{
					if (Keyboard.IsKeyDown (Key.LeftShift) || Keyboard.IsKeyDown (Key.RightShift))
					{
						//서식복사
						Attrib spoidAttrib = DeepCloneSourceAtoms.FirstOrDefault ()?.AtomCore?.GetAttrib ();

						if (null != spoidAttrib)
						{
							SpoitView spoidView = new SpoitView ();

							foreach (AtomBase atom in selectAtomList)
							{
								Attrib atomAttrib = atom.AtomCore.GetAttrib ();
								spoidView.SpoidCopyStruct (spoidAttrib, atomAttrib);
								atom.Sync_AttribToAtom ();
							}
						}
						return;
					}
				}

				//
				DeepCloneSourceAtoms.Sort (delegate (AtomBase x, AtomBase y)
				{
					int xZIndex = Grid.GetZIndex (x);
					int yZIndex = Grid.GetZIndex (y);

					return xZIndex.CompareTo (yZIndex);
				});
				//

				if (null != DeepCloneSourceAtoms && 0 < DeepCloneSourceAtoms.Count)
				{
					//m_nOffset = m_nOffsetCount * 10;
					//m_nOffsetCount++;

					List<DeepCloneAtomCommand> lstDeepCloneAtomCommands = new List<DeepCloneAtomCommand> ();

					//붙여넣기할 모든 아톰의 좌표가 충돌될때만 처리하도록 한다.
					List<Atom> atomCoreList = currentDMTView.GetAllAtomCores ();

					int nCount = 0;
					int nCopyOffset = 0 == PQAppBase.DefaultInterval ? 1 : PQAppBase.DefaultInterval;
					bool bCheck = true;

					while (bCheck)
					{
						foreach (AtomBase currentAtom in DeepCloneSourceAtoms)
						{
							bCheck = false;
							foreach (Atom atomCore in atomCoreList)
							{
								AtomBase ofAtom = atomCore.GetOfAtom ();

								if (null != ofAtom)
								{
									if (Kiss.DoubleEqual (ofAtom.Margin.Left, currentAtom.Margin.Left + (nCount * nCopyOffset)) &&
										Kiss.DoubleEqual (ofAtom.Margin.Top, currentAtom.Margin.Top + (nCount * nCopyOffset)))
									{
										bCheck = true;
										break;
									}
								}
							}

							if (false == bCheck)
							{
								break;
							}
						}

						if (true == bCheck)
						{
							nCount++;
						}
					}

					//기준 아톰 탐색
					int nAtomBasePositionType = BasePositionType; // 0-마우스 포인트 위치로 복사, 1-원본 아톰좌표기준으로 복사

					if (Keyboard.IsKeyDown (Key.LeftShift) || Keyboard.IsKeyDown (Key.RightShift))
					{
						nAtomBasePositionType = 1;
					}

					AtomBase pLeftbaseAtom = null;

					if (0 == nAtomBasePositionType)
					{
						foreach (AtomBase atom in DeepCloneSourceAtoms)
						{
							if (null == pLeftbaseAtom)
							{
								pLeftbaseAtom = atom;
							}
							else if (pLeftbaseAtom.Margin.Left > atom.Margin.Left)
							{
								pLeftbaseAtom = atom;
							}
							else if (Kiss.DoubleEqual (pLeftbaseAtom.Margin.Left, atom.Margin.Left) && pLeftbaseAtom.Margin.Top > atom.Margin.Top)
							{
								pLeftbaseAtom = atom;
							}
						}
					}

					foreach (AtomBase CurrentAtom in DeepCloneSourceAtoms)
					{
						Thickness CurrentAtomMargin = CurrentAtom.Margin;

						double dNewLeft = CurrentAtomMargin.Left + (nCount * nCopyOffset);
						double dNewTop = CurrentAtomMargin.Top + (nCount * nCopyOffset);

						this.DeepCopyInterval = nCount * nCopyOffset; //스크롤, 하이퍼링크와같이 자식 아톰이 있는경우 offset 고려해주기 위해서 설정함

						if (0 == nAtomBasePositionType)
						{
							Point pt = Mouse.GetPosition (currentDMTView);
							if (null != pLeftbaseAtom)
							{
								dNewLeft = pt.X + (CurrentAtom.Margin.Left - pLeftbaseAtom.Margin.Left);
								dNewTop = pt.Y + (CurrentAtom.Margin.Top - pLeftbaseAtom.Margin.Top);
							}

							if (0 > dNewLeft) dNewLeft = 0;
							if (0 > dNewTop) dNewTop = 0;
						}

						double dAtomWidth = CurrentAtom.Width;
						double dAtomHeight = CurrentAtom.Height;
						bool bIsBindedScroll = CurrentAtom.AtomCore.IsBindedScroll;
						bool bIsBindedPopup = CurrentAtom.AtomCore.IsBindedPopup;
						bool bIsBindedHyperLink = null != CurrentAtom.AtomCore.HyperLinkAtom ? true : false; // 190430_AHN_BINDE_HYPERLINK
						bool bIsBindAnimationGroup = CurrentAtom.AtomCore.GetAttrib ().IsAnimationGroup;

						if (true == bIsBindedPopup || true == bIsBindedScroll || true == bIsBindedHyperLink || true == bIsBindAnimationGroup)
						{
							continue;
						}

						AtomType WillCloneAtomType = CurrentAtom.AtomCore.AtomType;

						if (false != LicenseHelper.Instance.IsEnableSolutionService (WillCloneAtomType))
						{
							DeepCloneAtomCommand NewDeepCloneAtomCommand = new DeepCloneAtomCommand (currentDMTView, WillCloneAtomType, dNewLeft, dNewTop, new Size (dAtomWidth, dAtomHeight), null, CurrentAtom);
							lstDeepCloneAtomCommands.Add (NewDeepCloneAtomCommand);
						}
					}

					DeepCloneGroupedAtomsCommand NewDeepCloneGroupedAtomsCommand = new DeepCloneGroupedAtomsCommand (lstDeepCloneAtomCommands);
					Commander.AddCommand (NewDeepCloneGroupedAtomsCommand);
					var CommandResult = Commander.ExecuteCommand ();

					if (CommandResult is List<AtomBase> CopiedAtomList && 0 < CopiedAtomList.Count)
					{
						CopiedAtomList[0].AtomCore.Information.UpdatetPropertyToolBar (CopiedAtomList[0]);
					}
				}
			}
		}

		public override void UpdateCurrentSelectedAtomsLocationAndSize (KeyValuePair<LocationAndSizeDefine.LocationAndSizeType, double> ValueList)
		{
			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtoms ();
			List<MoveAtomCommand> lstMoveAtomCommands = new List<MoveAtomCommand> ();
			List<ExpandAtomCommand> lstExpandAtomCommands = new List<ExpandAtomCommand> ();
			LocationAndSizeDefine.LocationAndSizeType EventType = ValueList.Key;

			CommandCenter.CommandCommander commander = this.Commander;

			foreach (AtomBase CurrentAtom in lstCurrentSelectedAtoms)
			{
				if (false == CurrentAtom.AtomCore.Attrib.IsLocked)
				{
					Thickness CurrentAtomMargin = CurrentAtom.Margin;
					double dOrgLeft = CurrentAtomMargin.Left;
					double dOrgTop = CurrentAtomMargin.Top;
					double dValue = ValueList.Value;

					switch (EventType)
					{
						case LocationAndSizeDefine.LocationAndSizeType.Round:
							{
								if (1 <= dValue && 9999 >= dValue)
								{
									double originRadius = 0;

									if (CurrentAtom is RoundSquareofAtom)
									{
										originRadius = (CurrentAtom.AtomCore as RoundSquareAtom).GetCornerRadius ();
									}
									else
									{
										//CurrentAtom.BorderRadius = (int)dValue;
										originRadius = CurrentAtom.AtomCore.Attrib.BorderRadius;
									}

									RadiusChangeCommand radiusCommand = new RadiusChangeCommand (CurrentAtom, dValue, originRadius);
									commander.AddCommand (radiusCommand);
								}

								break;
							}

						case LocationAndSizeDefine.LocationAndSizeType.XPosition:
							{
								if (-5535 <= dValue && 59999 >= dValue)
								{
									//CurrentAtom.Margin = new Thickness(dValue, dOrgTop, 0, 0);

									MoveAtomCommand newGroupAtomCommand
										= new MoveAtomCommand (CurrentAtom, dValue, dOrgTop, CurrentAtom.Margin.Left, CurrentAtom.Margin.Top);

									lstMoveAtomCommands.Add (newGroupAtomCommand);
								}
								break;
							}

						case LocationAndSizeDefine.LocationAndSizeType.YPosition:
							{
								if (-5535 <= dValue && 59999 >= dValue)
								{
									//CurrentAtom.Margin = new Thickness(dOrgLeft, dValue, 0, 0);

									MoveAtomCommand newGroupAtomCommand
										= new MoveAtomCommand (CurrentAtom, dOrgLeft, dValue, CurrentAtom.Margin.Left, CurrentAtom.Margin.Top);

									lstMoveAtomCommands.Add (newGroupAtomCommand);
								}
								break;
							}
						case LocationAndSizeDefine.LocationAndSizeType.Width:
							{
								if (1 <= dValue && 9999 >= dValue)
								{
									ExpandAtomCommand NewMoveAtomCommand = null;

									if (CurrentAtom is VHLineofAtom)
									{
										VHLineofAtom VHLineAtom = CurrentAtom as VHLineofAtom;
										if (!VHLineAtom.GetVHLinePosition ())
										{
											VHLineAtom.NotifyCurrentLocationAndSize ();
											break;
										}
									}

									NewMoveAtomCommand = new ExpandAtomCommand (
											CurrentAtom, DIR_TYPE.DIR_NONE, CurrentAtom.Margin.Left, CurrentAtom.Margin.Top, CurrentAtom.Margin.Left, CurrentAtom.Margin.Top,
											new Size (CurrentAtom.Width, CurrentAtom.Height), new Size (dValue, CurrentAtom.Height));

									lstExpandAtomCommands.Add (NewMoveAtomCommand);
								}
								break;
							}

						case LocationAndSizeDefine.LocationAndSizeType.Height:
							{
								if (1 <= dValue && 9999 >= dValue)
								{
									ExpandAtomCommand NewMoveAtomCommand = null;

									if (CurrentAtom is VHLineofAtom)
									{
										VHLineofAtom VHLineAtom = CurrentAtom as VHLineofAtom;
										if (VHLineAtom.GetVHLinePosition ())
										{
											VHLineAtom.NotifyCurrentLocationAndSize ();
											break;
										}
									}

									NewMoveAtomCommand = new ExpandAtomCommand (
											CurrentAtom, DIR_TYPE.DIR_NONE, CurrentAtom.Margin.Left, CurrentAtom.Margin.Top, CurrentAtom.Margin.Left, CurrentAtom.Margin.Top,
											new Size (CurrentAtom.Width, CurrentAtom.Height), new Size (CurrentAtom.Width, dValue));

									lstExpandAtomCommands.Add (NewMoveAtomCommand);
								}
								break;
							}

						default: break;
					}
				}
			}

			switch (EventType)
			{
				case LocationAndSizeDefine.LocationAndSizeType.Round:
					break;
				case LocationAndSizeDefine.LocationAndSizeType.XPosition:
					{
						MoveGroupedAtomsCommand newMoveGroupedAtomsCommand = new MoveGroupedAtomsCommand (lstMoveAtomCommands);
						commander.AddCommand (newMoveGroupedAtomsCommand);
					}
					break;
				case LocationAndSizeDefine.LocationAndSizeType.YPosition:
					{
						MoveGroupedAtomsCommand newMoveGroupedAtomsCommand = new MoveGroupedAtomsCommand (lstMoveAtomCommands);
						commander.AddCommand (newMoveGroupedAtomsCommand);
					}
					break;
				case LocationAndSizeDefine.LocationAndSizeType.Width:
					{
						ExpandAtomsCommand NewExpandAtomsCommand = new ExpandAtomsCommand (lstExpandAtomCommands);
						commander.AddCommand (NewExpandAtomsCommand);
					}
					break;
				case LocationAndSizeDefine.LocationAndSizeType.Height:
					{
						ExpandAtomsCommand NewExpandAtomsCommand = new ExpandAtomsCommand (lstExpandAtomCommands);
						commander.AddCommand (NewExpandAtomsCommand);
					}
					break;
				default: break;
			}

			commander.ExecuteCommand ();

			//메뉴 영역에 아톰 포함여부 체크
			CheckAtomPosition ();
			//
		}

		/// <summary>
		/// 아톰이 하나일때 잠겨있는지 체크
		/// </summary>
		/// <returns></returns>
		public override bool IsToolLocked ()
		{
			bool bIsLock = false;
			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtoms ();
			int nCurrentSelectedAtomsCount = lstCurrentSelectedAtoms.Count;

			//아톰이 하나 선택된 경우
			if (1 == nCurrentSelectedAtomsCount)
			{
				AtomBase atom = lstCurrentSelectedAtoms[0] as AtomBase;
				bIsLock = atom.AtomCore.Attrib.IsLocked;
			}

			return bIsLock;
		}

		public override void OnToolActionManager ()
		{
			if (null == this.ActionManagerEditorWindow)
			{
				ActionManagerEditorWindow = new ActionManagerEditor ();
			}

			ActionManagerEditorWindow.Closed += ActionManagerEditorWindow_Closed;
			ActionManagerEditorWindow.OnChangeCharacter -= ActionManagerEditorWindow_OnCompleteCompile;
			ActionManagerEditorWindow.OnSaveActionManager -= ActionManagerEditorWindow_OnSaveActionManager;

			ActionManagerEditorWindow.OnChangeCharacter += ActionManagerEditorWindow_OnCompleteCompile;
			ActionManagerEditorWindow.OnSaveActionManager += ActionManagerEditorWindow_OnSaveActionManager;

			ActionManagerEditorWindow.LoadCompleted (this.ActionManager);

			if (null != ActionManagerEditorWindow)
			{
				ActionManagerEditorWindow.Show ();
			}
		}

		public override void OnToolAIAdaptor ()
		{
			AIAdaptorMainWindow.Closed += AIAdaptorMainWindow_Closed;

			AIAdaptorMainWindow.Show ();
		}

		public override void OnToolAIAdaptorDBManager ()
		{
			AIAdaptorDBCreateWindow dbCreate = new AIAdaptorDBCreateWindow ();

			dbCreate.Show ();
		}

		/// <summary>
		/// 애니메이션 관리자 
		/// </summary>
		public override void OnToolAnimation ()
		{
			if (null == this.AnimationWindow)
			{
				AnimationWindow = new TopAnimationWindow ();
				AnimationWindow.Title = $"{GetSubWindowTitle ()}";

				//
				DMTView CurrentDMTView = GetParentView () as DMTView;
				DMTFrame ownerFrame = CurrentDMTView.GetFrame () as DMTFrame;
				if (null != ownerFrame)
				{
					ToolBarProperty toolbarProperty = GetCurrentSelectedAtomProperties ();
					ownerFrame.NotifyAtomSelectedToAnimationWindow (toolbarProperty);
				}
				//
			}

			AnimationWindow.OnGetAllAtomList -= AnimationWindow_OnGetAllAtomList;
			AnimationWindow.OnGetAnimationManager -= AnimationWindow_OnGetAnimationManager;
			AnimationWindow.OnMakeAnimationGroup -= AnimationWindow_OnMakeAnimationGroup;
			AnimationWindow.OnDeleteAnimationGroup -= AnimationWindow_OnDeleteAnimationGroup;
			AnimationWindow.OnGetAllAtomList += AnimationWindow_OnGetAllAtomList;
			AnimationWindow.OnGetAnimationManager += AnimationWindow_OnGetAnimationManager;
			AnimationWindow.OnMakeAnimationGroup += AnimationWindow_OnMakeAnimationGroup;
			AnimationWindow.OnDeleteAnimationGroup += AnimationWindow_OnDeleteAnimationGroup;

			if (null != AnimationWindow)
			{
				AnimationWindow.AnimationLayer = ((DMTView)GetParentView ()).AnimationLayer;
				AnimationWindow.MDIParentWindow = Application.Current.MainWindow;

				DaulMonitorGetLocation (AnimationWindow, 300);

				AnimationWindow.Show ();
				AnimationWindow.Focus ();
			}
		}

		public override ArrayList GetBrowseAtomNameList ()
		{
			ArrayList arrBrowseAtomNameList = new ArrayList ();

			foreach (Atom pAtom in GetViewAtomCores ())
			{
				if (pAtom is ReportBrowseAtom)
				{
					arrBrowseAtomNameList.Add (pAtom.GetProperVar ());
				}
			}
			return arrBrowseAtomNameList;
		}

		public override void SetStoreMode ()
		{
			if (0 == StoreMode)
				return;

			DMTView currentView = GetParentView () as DMTView;

			if (null != currentView)
			{
				CFrameAttrib frameAttrib = this.GetFrameAttrib ();
				frameAttrib.SystemNavigation = true; //2021-07-21 kys 스토어 모드일때는 탐색메뉴를 표시하지 않는다.

				currentView.CompletePropertyChanged ();
			}
		}

		public override object GetTaretObject (string strSelAtom)
		{
			DMTView dmtView = GetParentView () as DMTView;

			if (null != dmtView)
			{
				string strAtomName = strSelAtom;

				// 2024.12.05 beh 아톰명에는 특수문자를 사용할 수 없으므로 괄호가 포함된 경우 폼 이름으로 판단한다
				if (false != string.IsNullOrEmpty (strSelAtom) ||
					(strSelAtom.Length > 2 && strSelAtom[0] == '(' && strSelAtom[strSelAtom.Length - 1] == ')'))
					return this;

				if (LC.LANG.ENGLISH == LC.PQLanguage)
					strAtomName = SMProperVar_Eng.GetSaveData_Atom (strAtomName); //공팩이 포함되어있을경우 제거해준다.

				List<Atom> atomCoreList = GetAllAtomCores ();

				Atom atomCore = atomCoreList.Where (item => strAtomName == item.GetProperVar ()).FirstOrDefault ();

				return atomCore;
			}

			return null;
		}

		public override void ChangeAtomTextEditMode (bool isTextEdit)
		{
			DMTView currentDMTView = GetParentView () as DMTView;
			DMTFrame currentDMTFrame = currentDMTView?.GetFrame () as DMTFrame;

			if (true != currentDMTFrame?.IsRulerModeOn && true != currentDMTFrame?.IsTextRulerView)
				return;

			List<AtomBase> atomList = currentDMTView.GetSelectAtomList ();
			List<WebHtmlTagofAtom> htmlAtomList = atomList.OfType<WebHtmlTagofAtom> ().ToList ();

			if (true == isTextEdit && 0 == atomList.Count)
			{
				PQAppBase.TraceDebugLog ("true == isTextEdit && 0 == atomList.Count");
			}

			if (htmlAtomList.Count < 2)
			{
				if (true == isTextEdit)
				{
					currentDMTFrame.InitTextRulerView ();
				}
				else
				{
					currentDMTFrame.ClearTextRulerView ();
				}
			}
			else
			{
				currentDMTFrame.ClearTextRulerView ();
			}
		}


		#region |  ----- PreExecute 관련 -----  |
		public override void ChangeDefaultTableName (string strTitle)
		{
			m_strDefaultTable = strTitle;
			m_bChangedTitle = true;
		}

		/// <summary>
		/// ERD변경정보를 PQSDN로 전송. HKJ 03.12.
		/// </summary>
		public override void AutoErdInfoUpdate ()
		{
			// 폼을 파일에 저장 후 ERD정보를 새로 업데이트한다.
			AutoSetErdInfoByTouch ();
		}

		public override void SetErdAndDBInfoBeforePreExecute ()
		{
			if (true == m_bDropDBMaster)
			{
				//this.AutoSetErdInfoByTouch();
				this.ReMakeDBMaster ();
			}

			DMTView currentView = this.GetParentView () as DMTView;
			bool bFirst = true;

			if (null == currentView)
			{
				return;
			}

			//80 DB 연결이 되었을경우 실행하면 CFieldMaster.m_strValue 속성에 데이터가 채워짐 그 값을 초기화 및..
			currentView.EmptyAtomList (null, LightDef.CLEAR_LOADFIELD | LightDef.CLEAR_ALLCLEAR, false);
			FormMode.FrameMode nOldRunMode = currentView.GetFormMode ();

			if (FormMode.FrameMode.Executed == nOldRunMode) //80 편집모드 -> 실행모드
			{
				if (false != bFirst)
				{
					CreateHelperList ();
					bFirst = false;
				}
				currentView.CreateViewHelperList ();
			}
		}

		public override void ExecuteRunMode (CObArray poaAtomData, bool bShowWindow, ScriptDoc pParentDOC)
		{
			base.ExecuteRunMode (poaAtomData, bShowWindow, pParentDOC);
			DMTView currentView = this.GetParentView () as DMTView;

			if (null == currentView) return;

			//액션관리자 실행
			if (null != ActionManagerEditorWindow)
				ActionManagerEditorWindow.ChangeRunMode (this.ActionManager);

			this.ActionManager.AnimationLayer = currentView;
			this.ActionManager.Play ();

			//탭 초점이동 기능 설정 (실행모드)
			SetRunModeTabMode ();

			//학습정보 데이터 수집
			SetRunModeLRSView ();
		}

		public override void PreExecute ()
		{
			DMTView currentView = this.GetParentView () as DMTView;

			if (null == currentView)
			{
				return;
			}

			currentView.JobRun ();

			if (false != this.EmbededNavigationMode)
				return;

			FormMode.FrameMode frameMode = currentView.GetFormMode ();

			if (null != currentView && true == IsChangeTabNum ())
			{
				//SetUndoRedoInfo(pMainView, m_pOrderAtom, ProcessDef.UNDO_TAB_ORDER, false);
				SetChangeTabNum (false);
			}

			if (FormMode.FrameMode.Executed == frameMode) // 편집모드 -> 실행모드 
			{
				// 프로그램관리자 임시변수 클리어
				this.PMVariable ().Clear ();
				// 1. 각 아톰의 연산식을 실행 모드로 전환한다..(빠른 실행을 위해..)
				MakeOperateArray ();
				MakeConditionOperateArray ();

				MakeSQLStatement ();
				MakeDBMangerLinkedVariables ();
				base.BuildCvtInfo ();
				m_bDropDBMaster = false;

				string strQueryInfo = PQAppBase.QueryInfo;
				SetQueryInfoDoc ("", LC.GS ("TopProcess_DMTDoc_7"));

				string strFormName = GetFormName ();
				this.ExeFormName = strFormName;// scriptdoc 함

				if (_PQRemoting.ServiceType == SERVICE_TYPE._3Tier)
					PQAppBase.SetQueryInfo (strQueryInfo);

				AnimationManager.ToRunMode ();
				EmphasisAnimationManager.ToRunMode ();

				//2020-07-06 kys 업무규칙 셋팅 논리 추가 
				//검색창.검색완료 업무규칙 이벤트와 같이 폼열림 이벤트 전에 동작하는 비동기 이벤트들을 정상적으로 실행시켜 주기 위해서 추가함
				InitScript (this);

				ExecuteReadyEnd ();

				DoPostRunMode ();

				AlertAtomSetting ();

				if (null != m_poaAtomData && 0 < m_poaAtomData.Count)
				{
					// 메인폼에서 파라미터 넘길경우
					LoadContent (m_poaAtomData, false);
				}
				else
				{
					DBIO (true, SQL_RECORD_TYPE.EQUAL_RECORD, null, null);
				}

				DirectExecute (); // 즉시실행 논리 위치 변경 - 기본검색키 동작 이후에 처리 할수 있도록
								  //

				EBookManager ebookManager = this.EBookManager as EBookManager;
				bool isEbookMainPage = false;

				if (null == EBookManager)
				{
					ExecuteScript (); // 스크립트를 실행 시켜 준다..
				}
				else
				{
					int nPage = ebookManager.GetCurrentPage ();

					if (-1 != nPage)
					{
						if (GetParentView () == ebookManager.GetPageViewMap ()[nPage])
						{
							isEbookMainPage = true;
							ExecuteScript ();
						}
					}
				}

				// 2. 작업 년도, 회사명 등.. 자동 컨버전이 필요한 넘들을.. 컨버전 해준다..
				base.BuildCvtInfo ();

				//QPM은 그냥 플레이
				if (null == ebookManager && false == IsWebDoc)
				{
					PlayAnimation (AnimationDetailEventDefine.ADE_Form_Loaded);
					PlayFormLoadedGif ();
				}
				else if (true == IsWebDoc)
				{
					//2020-08-10 kys 애니관리자 폼열림 시점을 액티브뷰 쪽으로 변경함
					PlayAnimationWebFormLoaded ();
				}
				//QEB는 현재 페이지만 플레이
				else
				{
					if (true == isEbookMainPage)
					{
						//스마트콘텐츠 아톰 확장기능보강 (이수기능)
						List<Atom> listAtom = GetAllAtomCores ();
						EBookCourseStudyManager.Instance.FormLoad (listAtom);
						//

						PlayAnimation (AnimationDetailEventDefine.ADE_Form_Loaded);
						PlayEmphasisAnimation (AnimationDetailEventDefine.ADE_Form_Loaded);

						PlayFormLoadedGif ();
					}
				}

				//액션관리자 실행
				if (null != ActionManagerEditorWindow)
					ActionManagerEditorWindow.ChangeRunMode (this.ActionManager);

				this.ActionManager.AnimationLayer = currentView;
				this.ActionManager.Play ();

				//탭 초점이동 기능 설정 (실행모드)
				SetRunModeTabMode ();

				//학습정보 데이터 수집
				SetRunModeLRSView ();
			}
			else // 실행모드 -> 편집모드
			{
				DropHelperList ();
				currentView.DropOwnerAtom ();
				currentView.DropViewHelperList ();
				CreateHelperList (); //2005.10.10 이현숙 (기본적으로 HelperList가 잇어야 한다.)
				m_bDropDBMaster = true;
				//폼 오픈 애니메이션 플래그 초기화
				FormLoadAnimationPlayed = false;

				DoPostEditMode ();

				AnimationManager.ToEditMode ();
				EmphasisAnimationManager.ToEditMode ();

				//액션관리자 종료
				this.ActionManager.Stop ();

				//탭 초점이동 기능 설정 (편집모드)
				SetEditModeTabMode ();

				//학습정보데이터 수집
				SetEditModeLRSView ();
			}
		}

		private void SetRunModeTabMode ()
		{
			var viewAtoms = GetViewAtomCores ();

			foreach (var atom in viewAtoms)
			{
				var atomBase = atom.GetOfAtom ();
				if (true == atomBase.IsRunModeTabMovable ())
				{
					atomBase.IsTabStop = true;
					atomBase.Focusable = true;

					if (atomBase is InputAtomBase || atomBase is ScrollAtomBase)
					{
						atomBase.IsTabStop = false;
						atomBase.Focusable = false;
					}

					if (atomBase is ExtensionScrollAtomBase || atomBase is TabViewAtomBase || atomBase is InputAtomBase)
					{
						atomBase.SetValue (KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.Local);
					}
					else
					{
						atomBase.SetValue (KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.None);
					}
				}
				else
				{
					atomBase.IsTabStop = false;
					atomBase.Focusable = false;
					atomBase.SetValue (KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.None);
				}
			}
		}

		private void SetEditModeTabMode ()
		{
			var viewAtoms = GetViewAtomCores ();

			foreach (var atom in viewAtoms)
			{
				atom.GetOfAtom ().Focusable = false;
			}
		}

		private void SetRunModeLRSView ()
		{
			var atoms = GetAllAtomCores ();

			foreach (var atom in atoms)
			{
				var ofAtom = atom.GetOfAtom ();

				ofAtom.IsVisibleChanged -= LRSAtom_IsVisibleChanged;
				ofAtom.IsVisibleChanged += LRSAtom_IsVisibleChanged;
			}
		}

		private void SetEditModeLRSView ()
		{
			var atoms = GetAllAtomCores ();

			foreach (var atom in atoms)
			{
				var ofAtom = atom.GetOfAtom ();

				ofAtom.IsVisibleChanged -= LRSAtom_IsVisibleChanged;
			}
		}

		private void LRSAtom_IsVisibleChanged (object sender, DependencyPropertyChangedEventArgs e)
		{
			if (false == this.GetRunMode ())
				return;

			if (sender is AtomBase ofAtom)
			{
				var pmList = GetPMInfoList (ofAtom.AtomCore);

				if (0 < pmList.Count)
				{
					ofAtom.AtomCore.LRSProcessEvent ();
				}

				//var groupAtoms = this.GetAllAtomCores ().OfType<EBookAnimationGroupAtomCore> ().ToList ();
				//if (null != groupAtoms && 0 < groupAtoms.Count)
				//{
				//	var targetGroup = groupAtoms.Find (item => item.GroupAtomList.Contains (ofAtom.AtomCore));

				//	if (null != targetGroup)
				//	{
				//		pmList = GetPMInfoList (targetGroup);

				//		if (0 < pmList.Count)
				//		{
				//			targetGroup.LRSProcessEvent ();
				//		}
				//	}
				//}
			}
		}

		public override void PlayAnimationWebFormLoaded ()
		{
			DMTView currentView = this.GetParentView () as DMTView;

			if (null != currentView)
			{
				DMTFrame pDMTFrame = currentView.GetFrame () as DMTFrame;

				if (null != pDMTFrame && null != pDMTFrame.MotionManager)
				{
					List<Atom> pAtomList = pDMTFrame.MotionManager.GetStartAnimationAtom ();
					PlayAnimation (AnimationDetailEventDefine.ADE_Form_Loaded, pAtomList);
				}
			}
		}

		public override bool MakeSQLStatement ()
		{
			bool bResult = false;

			//80 필요없을듯 탭안의 모든 아톰리스트 구성하여 탭 아톰에 넣어줌 텝페이지 상관없음 
			//탭뷰아톰이 있는 경우 탭뷰 안에 있는 아톰리스트를 만들어줌
			//MakeTapViewAtomList();

			CSQLGenerator pSQLGen = new CSQLGenerator ();
			bResult = pSQLGen.MakeSQLStatement (this);

			this.m_htProperSQLIndex = pSQLGen.PROPERSQLINDEX;
			this.InitDocQueryMgr (); // 도큐먼트와 쿼리메니저 연결
			return bResult;
		}

		public override void MakeDynmaicGrid ()
		{
			CFrameAttrib frameAttrib = this.GetFrameAttrib ();
			if (null != frameAttrib && true == frameAttrib.IsDynamicMode)
			{
				DMTView currentView = this.GetParentView () as DMTView;
				DMTFrame currentFrame = currentView.GetFrame () as DMTFrame;
				DynamicGridTable gridTable = currentFrame.CurrentPhoneScreenView.DynamicGrid as DynamicGridTable;
				gridTable.ViewMode ();
			}
		}

		public override void MakeProcessManager ()
		{
			if (true == PQAppBase.IsProcessManager)
			{
				if (null != FlowManagerWindow)
				{
					FlowManagerViewModel viewModel = FlowManagerWindow?.DataContext as FlowManagerViewModel;
					viewModel?.CompletedProcessEdit ();
				}
			}
		}

		public override void MakeAtomData ()
		{
			List<Atom> atoms = GetAllAtomCores ();

			foreach (var atom in atoms)
			{
				atom.PrepareSerializeData ();
			}
		}

		public override bool GetRunMode ()
		{
			DMTView pView = this.GetParentView () as DMTView;

			if (null != pView && FormMode.FrameMode.Executed == pView.GetFormMode ())
				return true;

			return false;
		}

		/// <summary>
		/// [팝업묶기 창]을 통해 아톰을 팝업에 바인딩할경우
		/// 디시리얼라이즈 할때 
		/// </summary>
		/// <param name="lstBindedAtoms"></param>
		public override void BindAtomToBindPopup (PopupofAtom PopupAtom, List<AtomBase> lstBindedAtoms)
		{
			PopupAtom.ReleaseBindedAtoms ();

			Thickness PopupAtomStartPoint = new Thickness ();
			Size PopupAtomSize = Size.Empty;
			int nBindedAtomsCount = lstBindedAtoms.Count;
			bool bIsPossible = IsPossibleBindPopup (lstBindedAtoms);

			if (true == bIsPossible)
			{
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstBindedAtoms, ref PopupAtomStartPoint, ref PopupAtomSize);

				if (true == bIsCalculated)
				{
					//팝업아톰들의 리스트를 넘겨준다.
					PopupAtom.BindAtoms (lstBindedAtoms);

					//팝업에 묶인 아톰들에게 묶였다는 속성을 준다.
					foreach (AtomBase atom in lstBindedAtoms)
					{
						atom.SetResizeAdornerVisibility (Visibility.Collapsed, true);
						atom.AtomCore.IsBindedPopup = true;
						atom.AtomCore.BindBlockAtom = PopupAtom.AtomCore;
					}
				}
			}
		}

		/// <summary>
		/// [팝업묶기 창]을 통해 아톰을 팝업에 바인딩할경우
		/// 디시리얼라이즈 할때 
		/// </summary>
		/// <param name="lstBindedAtoms"></param>
		public override void BindAtomToWebBlock (WebBlockofAtom BlockAtom, List<AtomBase> lstBindedAtoms)
		{
			BlockAtom.ReleaseBindedAtoms ();

			Thickness PopupAtomStartPoint = new Thickness ();
			Size PopupAtomSize = Size.Empty;
			int nBindedAtomsCount = lstBindedAtoms.Count;
			bool bIsPossible = IsPossibleBindWebBlock (lstBindedAtoms);

			if (true == bIsPossible)
			{
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstBindedAtoms, ref PopupAtomStartPoint, ref PopupAtomSize);

				if (true == bIsCalculated)
				{
					//팝업아톰들의 리스트를 넘겨준다.
					BlockAtom.BindAtoms (lstBindedAtoms);

					//팝업에 묶인 아톰들에게 묶였다는 속성을 준다.
					foreach (AtomBase atom in lstBindedAtoms)
					{
						atom.SetResizeAdornerVisibility (Visibility.Collapsed, true);
						atom.AtomCore.BindBlockAtom = BlockAtom.AtomCore;
					}
				}
			}
		}

		/// <summary>
		/// 디시리얼라이즈(로드) 할때 
		/// </summary>
		/// <param name="HyperAtom"></param>
		/// <param name="lstBindedAtoms"></param>
		public override void BindAtomToHyperLink (WebHyperDataofAtom HyperAtom, List<AtomBase> lstBindedAtoms)
		{
			HyperAtom.ReleaseBindedAtoms ();

			Thickness HyperAtomStartPoint = new Thickness ();
			Size HyperAtomSize = Size.Empty;
			int nBindedAtomsCount = lstBindedAtoms.Count;
			bool bIsPossible = IsPossibleBindHyperLink (lstBindedAtoms);

			if (true == bIsPossible)
			{
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstBindedAtoms, ref HyperAtomStartPoint, ref HyperAtomSize);

				if (true == bIsCalculated)
				{
					HyperAtom.BindAtoms (lstBindedAtoms);

					foreach (AtomBase atom in lstBindedAtoms)
					{
						atom.SetResizeAdornerVisibility (Visibility.Collapsed, true);
						atom.AtomCore.HyperLinkAtom = HyperAtom.AtomCore;
					}
				}
			}
		}

		public override void BindAtomToAutoComplete (AutoCompleteofAtom AutoCompleteAtom, List<AtomBase> lstBindedAtoms)
		{
			AutoCompleteAtom.ReleaseBindedAtoms ();

			Thickness HyperAtomStartPoint = new Thickness ();
			Size HyperAtomSize = Size.Empty;
			int nBindedAtomsCount = lstBindedAtoms.Count;
			bool bIsPossible = IsPossibleBindAutoComplete (lstBindedAtoms);

			if (true == bIsPossible)
			{
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstBindedAtoms, ref HyperAtomStartPoint, ref HyperAtomSize);

				if (true == bIsCalculated)
				{
					AutoCompleteAtom.BindAtoms (lstBindedAtoms);

					foreach (AtomBase atom in lstBindedAtoms)
					{
						atom.SetResizeAdornerVisibility (Visibility.Collapsed, true);
						atom.AtomCore.AutoCompleteAtom = AutoCompleteAtom.AtomCore;
					}
				}
			}
		}

		/// <summary>
		/// [스크롤묶기 창]을 통해 아톰을 스크롤에 바인딩할 경우
		/// 디시리얼라이즈 할때 
		/// </summary>
		public override void BindAtomToScroll (AtomBase bindingAtomBase, List<AtomBase> lstBindedAtoms, bool bRefresh)
		{
			if (bindingAtomBase is ScrollAtomBase)
			{
				ScrollAtomBase ScrollAtom = bindingAtomBase as ScrollAtomBase;
				//바인딩된 아톰 해제
				if (true == bRefresh)
				{
					ScrollAtom.DisposeAll ();
				}

				Thickness ScrollAtomStartPoint = new Thickness ();
				Size ScrollAtomSize = Size.Empty;

				int nBindeAtomsCount = lstBindedAtoms.Count;
				bool bIsPossibleBindScroll = IsPossibleBindScroll (lstBindedAtoms);

				if (true == bIsPossibleBindScroll)
				{
					bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstBindedAtoms, ref ScrollAtomStartPoint, ref ScrollAtomSize);

					if (true == bIsCalculated)
					{
						DeleteParentOfAtoms (lstBindedAtoms);
					}
				}

				ScrollAtom.BindAtoms (lstBindedAtoms);
				AdjustAllAtomZIndexInLightDMTView ();
				AdjustAllAtomRelativeTabIndexInLightDMTView ();
				AutoRefreshEditWindow ();
			}
		}

		public override void BindAtomToTabPage (TabViewAtomBase tabOfAtom, int nTabPageIndex, AtomBase targetAtomBase)
		{
			if (null == tabOfAtom || 0 > nTabPageIndex || null == targetAtomBase) return;

			this.GetChildren ().Remove (targetAtomBase);
			this.AttachAtomAtTabView (targetAtomBase, tabOfAtom, true, nTabPageIndex);

			targetAtomBase.SetResizeAdornerVisibility (Visibility.Visible, false);
		}

		#endregion

		#region |  ----- 저장관련 -----  |
		/// <summary>
		/// 계속적인 작업을 위해 파일을 제어한다. 
		/// </summary>
		/// <param name="strFilePath"></param>
		public override void CatchCurrentFile (string strFilePath)
		{
			// 스트림 복사시 루틴 중지
			if (true == this.IsDocCopy || true == this.TemplateMode || true == this.IsEmbededMode || true == this.LicensePass)
				return;

			if (File.Exists (strFilePath))
			{
				try
				{
					if ((File.GetAttributes (strFilePath) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
					{
						this.CanSaveDocument = true;
						Stream tempStream = File.OpenWrite (strFilePath);
						tempStream.Close ();
					}
				}
				catch (IOException)
				{
					_Message80.Show (LC.GS ("TopProcess_DMTDoc_5"));
					DMTView currentView = this.GetParentView () as DMTView;
					currentView.IsReadonlyMode = true;
					this.CanSaveDocument = false;
				}

				CurrentFileStream = new FileStream (strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
		}

		public override void CloseCurrentFileStream ()
		{
			if (null != CurrentFileStream)
			{
				CurrentFileStream.Close ();
				CurrentFileStream = null;
			}
		}

		/// <summary>
		/// F12 키가 눌렸을 경우..폼을 저장한다..
		/// </summary>
		/// <returns></returns>
		public override bool SaveDocument ()
		{
			if (false == this.IsEBookDoc)
			{
				//2004.12.28 LHS
				if (false == _Kiss._isempty (this.FilePath))
				{
					return this.OnSaveDocument (this.FilePath);
				}

				DMTView CurrentView = GetParentView () as DMTView;

				if (null != CurrentView)
				{
					CurrentView.NotifyNewSave ();
					return true;
				}
			}

			return false;
		}

		public override bool OnSaveDocument (string lpszPathName)
		{
			if (false == CanSaveDocument)
			{
				return false;
			}

			DMTView CurrentView = GetParentView () as DMTView;

			bool bIsSomethingMode = false;

			if (null != CurrentView)
			{
				bIsSomethingMode = CurrentView.IsSpoitMode () | CurrentView.IsChangeTabIndexMode ();

				if (true == bIsSomethingMode)
				{
					OnToolRollBackLockStateAllAtoms ();
				}
			}

			// 사용하지 않은 Image 제거
			RemoveGDIResource ();

			SaveTemplateInformation ();
			bool bResult = base.OnSaveDocument (lpszPathName);

			if (true == bIsSomethingMode)
			{
				OnToolLockAndBackupAllAtoms ();
			}

			AutoRefreshEditWindow ();

			return bResult;
		}

		/// <summary>
		/// Script Language Conversion...
		/// </summary>
		/// <returns></returns>
		public override int ChangeScriptLanguage ()
		{
			CScriptApp.LoadScriptKeyword ((int)LC.PQLanguage);

			CStringArray pIndexSource = GetIndexSource ();
			if (null == pIndexSource || 0 == pIndexSource.GetSize ())
				return 1;

			string strIndexSource = "";
			for (int i = 0; i < pIndexSource.GetSize (); i++)
				strIndexSource = string.Concat (strIndexSource, pIndexSource.GetAt (i));

			CCompManager pManager = new CCompManager ();
			pManager.SetFormDoc (this);
			pManager.SetFormExe (this.m_pScriptServer.GetFormExe ());
			string strResult = pManager.DoIndexConversion (strIndexSource);
			SetScriptSource (strResult);
			m_pScriptServer.SetLanguage ((int)LC.PQLanguage);
			return 1;
		}

		#endregion

		#region |  ----- DB 처리객체 -----  |
		public override ArrayList GetDocLinkedGlobalScriptUserVarItems ()
		{
			ArrayList oaLinkedItems = new ArrayList ();

			foreach (string strGlobartVar in PQAppBase.GlobalInfo.Keys)
			{
				if (IsGlobalScriptUserVar (strGlobartVar))
				{
					CDocLinkedItem pItem = new CDocLinkedItem (strGlobartVar, LC.GS ("TopProcess_DMTDoc_46"), DataType._String);
					oaLinkedItems.Add (pItem);
				}
			}

			return oaLinkedItems;
		}

		public override ArrayList GetDocLinkedItems ()
		{
			ArrayList oaLinkedItems = new ArrayList ();

			foreach (Atom pAtom in this.GetOrderAtom ())
			{
				if (true == pAtom.IsKindOf (typeof (TouchAtom)) || true == pAtom.IsKindOf (typeof (PopupAtom)) || true == pAtom.IsKindOf (typeof (StructureTreeAtom)))
				{
					//string strProperVar = pAtom.GetProperVar(); 원본
					string strProperVar = pAtom.GetProperVar (false);
					int nFieldType = -1;
					pAtom.GetAttrib ().GetFieldType (ref nFieldType);

					DMTView currentView = this.GetParentView () as DMTView;
					CDocLinkedItem pItem = new CDocLinkedItem (strProperVar, pAtom.GetDefaultProperVar (), (DataType)nFieldType);
					oaLinkedItems.Add (pItem);
				}
				else if (true == pAtom.IsKindOf (typeof (BrowseAtom)))
				{
					BrowseAtom pBrowse = pAtom as BrowseAtom;
					if (null != pBrowse)
					{
						DMTView currentView = this.GetParentView () as DMTView;

						string strBrowseVar = pBrowse.GetProperVar ();
						foreach (BrowseItem pBrowseAtom in pBrowse.OABrowse)
						{
							string strVar = pBrowseAtom.BrowseVar;
							if (true == string.IsNullOrEmpty (strVar))
								continue;

							string strProperVar = string.Format ("{0}.{1}", strBrowseVar, strVar);

							CDocLinkedItem pItem = new CDocLinkedItem (strProperVar, pAtom.GetDefaultProperVar (), pBrowse.GetDataTypeOfField (strVar));
							oaLinkedItems.Add (pItem);
						}
					}
				}
				else if (true == pAtom.IsKindOf (typeof (StructureGridAtom)))
				{
					StructureGridAtom pStructure = pAtom as StructureGridAtom;
					if (null != pStructure)
					{
						string strStructureVar = pStructure.GetProperVar ();

						foreach (StructureGridItem model in pStructure.GetParameterList ())
						{
							string strVar = model.ParameterName;
							if (true == string.IsNullOrEmpty (strVar))
								continue;

							string strProperVar = string.Format ("{0}.{1}", strStructureVar, strVar);

							CDocLinkedItem pItem = new CDocLinkedItem (strProperVar, pAtom.GetDefaultProperVar (), DataType._Char);
							oaLinkedItems.Add (pItem);
						}
					}
				}

			}

			return oaLinkedItems;
		}

		public override Dictionary<string, string> AtomNameList
		{
			get
			{
				return base.AtomNameList;
			}
			set
			{
				base.AtomNameList = value;
			}
		}

		public override ArrayList GetDocLinkedScriptItems ()
		{
			ArrayList oaLinkedItems = new ArrayList ();

			CDBMaster pDBMaster = this.GetDBMaster ();
			foreach (string strScriptVar in pDBMaster.ScriptVarMap.Keys)
			{
				if (true == string.IsNullOrEmpty (strScriptVar) || 0 == strScriptVar.IndexOf ("_"))
					continue;

				CDocLinkedItem pItem = new CDocLinkedItem (strScriptVar, LC.GS ("TopProcess_DMTDoc_47"), DataType._String);
				oaLinkedItems.Add (pItem);
			}

			return oaLinkedItems;
		}

		#endregion

		#region |  ----- 설계도 생성 / DB관리자 -----  |

		public override Hashtable GetErdAutoInformation ()
		{
			return this.ReGenFormInfoMap (GetFormInformation ());
		}

		#endregion//설계도 생성 / DB관리자

		#region |  ----- 진행관리자 -----  |

		public override Window ShowProcessEventManager ()
		{
			if (true == PQAppBase.IsProcessManager)
			{
				if (null == FlowManagerWindow)
				{
					FlowManagerWindow = new FlowManagerMainWndow ();
					FlowManagerViewModel flowManagerViewModel = new FlowManagerViewModel (this);

					FlowManagerWindow.Owner = Application.Current.MainWindow;
					FlowManagerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					FlowManagerWindow.DataContext = flowManagerViewModel;
				}

				FlowManagerWindow.Show ();

				return FlowManagerWindow;
			}

			if (null == this.FlowMapWindow)
			{
				FlowMapWindow = new FlowMap (this);
			}

			if (null != FlowMapWindow)
			{
				DaulMonitorGetLocation (FlowMapWindow, 387);

				FlowMapWindow.Show ();
				FlowMapWindow.Focus ();
			}

			FlowMapWindow.ChangeSourceDocument (this);
			FlowMapWindow.MDIParentWindow = Application.Current.MainWindow;

			return FlowMapWindow;
		}

		public override void LoadQueryMgrFrame (System.Windows.Window pForm, string strQueryName, bool bDoModal)
		{
			if (true == bDoModal)
			{
				if (null == pForm)
				{
					pForm = null;
				}

				ShowDBMgrFrame80 (pForm, strQueryName, bDoModal, -1, -1);
			}
			else
				ShowDBMgrFrame80 (null, strQueryName, bDoModal, -1, -1);
		}


		public override void LoadQueryMgrFrame (Window pForm, EventHandler CloseEvent)
		{
			//CloseEvent, 
			m_DBMgrCloseEvent = CloseEvent;
			ShowDBMgrFrame80 (pForm, string.Empty, true, -1, -1);
		}

		/// <summary>
		/// 2007.07.11 이현숙 함수 구성루틴 수정
		/// </summary>
		/// <returns></returns>
		public override Hashtable GetScriptFunction ()
		{
			Hashtable htResult = new Hashtable ();

			CFormExe formExe = GetScriptServer ().GetFormExe ();
			if (null != formExe)
			{
				string[] saFunction = formExe.GetFunction ();

				int nCount = saFunction.Length;
				for (int i = 0; i < nCount; i++)
				{
					string strIndexFunc = saFunction[i];
					string strFunction = GetFuncName (strIndexFunc);

					htResult.Add (strFunction, strIndexFunc);
				}
			}

			return htResult;
		}

		public override bool DropScriptEdit80 (bool bCloseDocument)
		{
			if (null != m_pScriptWindow)
			{
				if (false != bCloseDocument)
					m_pScriptWindow.Close ();
				m_pScriptWindow = null;
			}

			return true;
		}

		public override IScriptObject GetAtomFromName (string strAtomName)
		{
			if (false == this.GetRunMode () && true == FlowManagerWindow?.IsVisible)
			{
				List<Atom> atomCoreList = GetAllAtomCores ();
				Atom findAtom = atomCoreList.Where (item => strAtomName == item.GetProperVar ()).FirstOrDefault ();
				return findAtom;
			}

			return base.GetAtomFromName (strAtomName);
		}

		#endregion//진행관리자

		#region |  ----- 아톰편집도우미 -----  |
		#region |  Window ShowAtomEditManager()  |

		public override Window ShowAtomEditManager ()
		{
			if (null == this.AtomEditMapWindow)
			{
				AtomEditMapWindow = new AtomEditMap (this);
				AtomEditMapWindow.Title = $"{GetSubWindowTitle ()} - 아톰편집 도우미";
			}

			if (null != AtomEditMapWindow)
			{
				DaulMonitorGetLocation (AtomEditMapWindow, 387);

				AtomEditMapWindow.Show ();
				AtomEditMapWindow.Focus ();
			}

			AtomEditMapWindow.MDIParentWindow = Application.Current.MainWindow;

			return AtomEditMapWindow;
		}
		#endregion
		#endregion//아톰편집도우미

		#region |  ----- 업무규칙 -----  |

		#region |  bool DropMultiBlockEdit80(bool bCloseDocument)  |

		public override bool DropMultiBlockEdit80 (bool bCloseDocument)
		{
			if (null != m_pScriptWindow)
			{
				m_pScriptWindow.WizardBAR.IsEnabled = true;
				m_pScriptWindow.IS_READ_ONLY = false;
				m_pScriptWindow.FORCE_SOURCE_CHANGE = true;
				m_pScriptWindow.ShowEditWindow ((ScriptDoc)this, "", false);
				m_pScriptWindow.Activate ();
				m_pScriptWindow.Show ();
				m_pScriptWindow.OnLoadSourceText ();
			}

			return true;
		}

		#endregion

		#region |  Window ShowEditWindow80(int dwOption)  |

		public override Window ShowEditWindow80 (int dwOption)
		{
			//bool bResult = true;

			#region 권한 여부에 따른 스크립트 편집 가능 여부에 따른 처리
			// 2007. 10. 23, 김은숙, 권한 때문에 스크립트가 열리지 않을 경우에는 메시지를 따로 처리 해 줌
			// 1. 스크립트를 수정할 수 있는지 검사한다.

			if (false == IsModifyAuthority ())
			{
				int dwFormType = GetVersion ();
				if (0 == (dwFormType & VersionDefine.VFORM_SCRIPT))
					return null;
			}

			#endregion


			#region 폼에서 [업무규칙] 메뉴를 클릭했을 경우
			// 3. 편집기가 열려 있지 않는 경우 새로운 편집기를 열어서 맹글어 준다..
			bool bCreateEdit = false;

			if (null == m_pScriptWindow)
			{
				if (true == this.m_pScriptServer.HasObjScriptSource () && this.FilePath != "")
				{
					if (false == IsExistsScriptFile ())
					{
						string strMsg = LC.GS ("TopProcess_DMTDoc_48");
						//strMsg = string.Format(strMsg, strPath);
						if (System.Windows.MessageBoxResult.No == _Message80.Show (strMsg, "", MessageBoxButton.YesNo, MessageBoxImage.Question))
							return null;
					}
				}

				CreateScriptEdit80 ();
				bCreateEdit = true;
			}

			if (null != m_pScriptWindow)
			{
				// 읽기 전용으로 만들 부분
				this.m_pScriptWindow.IS_READ_ONLY = false;
				this.m_pScriptWindow.WizardBAR.IsEnabled = true;

				//80 추후작업 
				//m_pScriptWindow.CustStatusBar = this.CustStatusBar;

				// 5. 편집기에 폼 정보를 설정한후 편집기를 Show 한다..
				string strSelAtom = (null != m_pRButtonAtom) ? m_pRButtonAtom.GetProperVar () : "";

				m_pScriptWindow.ShowEditWindow ((ScriptDoc)this, strSelAtom, bCreateEdit);
				m_pScriptWindow.ActivateWindow ();
				m_pScriptWindow.Show ();
				if (IsScriptChanged) ToastMessge.Show ("작성된 업무규칙 내용이 반영되지 않았습니다.\r\n 업무규칙 컴파일 작업을 실행해주세요.");
				m_pScriptWindow.OnLoadSourceText ();

				string strTitle = string.Empty != this.GetFormTitle () ? this.GetFormTitle () : this.GetFormName ();

				if (true == IsEBookDoc)
				{
					m_pScriptWindow.Title = string.Format (LC.GS ("TopProcess_DMTDoc_43"), strTitle, GetEBookPage (), LC.GS ("TopProcess_DMTDoc_19"));
				}
				else
				{
					m_pScriptWindow.Title = string.Format ("{0} - {1}", strTitle, LC.GS ("TopProcess_DMTDoc_19"));
				}
			}

			#endregion


			return m_pScriptWindow;
		}

		#endregion

		#region |  void SetFormChange(bool bChange/* = true*/)  |

		public override void SetFormChange (bool bChange/* = true*/)
		{
			if (false == GetRunMode () && PROG_ID.MBIZMAKER_MOS != PQAppBase.ProgramID) //편집모드일때만 플레그값이 변경되도록 한다.
			{
				m_bFormChange = bChange;

				//DMTFrame pFrame = this.GetThisForm() as DMTFrame;
				//if (null != pFrame)
				//{
				//    pFrame.ShowChangedDoc();
				//}

				DMTView currentDMTView = GetParentView () as DMTView;

				if (null != currentDMTView)
				{
					DMTFrame pFrame = currentDMTView.GetFrame () as DMTFrame;

					if (null != pFrame && null != pFrame.CurrentCaptionBar)
					{
						bChange = bChange || pFrame.GetBookContentChanged ();

						pFrame.CurrentCaptionBar.SetSaveFlag (bChange);
					}
				}
			}
		}

		#endregion

		#region |  void CompletedScriptCompile()  |

		public override void CompletedScriptCompile ()
		{
			base.CompletedScriptCompile ();
			AutoRefreshEditWindow ();
		}

		#endregion

		#region |  ushort GetMaxScriptIndex()  |

		public override ushort GetMaxScriptIndex ()
		{
			if (0 == m_stnMaxIndex)
			{
				foreach (Atom pAtom in m_pOrderAtom)
				{
					if (null != pAtom)
					{
						SetMaxScriptIndex (pAtom.GetScriptIndex ());
					}
				}
			}

			List<Atom> pAtomCoreList = this.GetAllAtomCores ();

			ushort strReturn = (ushort)++m_stnMaxIndex;
			ushort strMax = strReturn;
			bool bExist = false;
			foreach (Atom pAtom in pAtomCoreList)
			{
				ushort strIndex = pAtom.GetScriptIndex ();
				if (strReturn == strIndex)
				{
					bExist = true;
				}

				if (strMax < strIndex)
				{
					strMax = strIndex;
				}
			}

			if (false != bExist)
			{
				m_stnMaxIndex = ++strMax;
			}

			return (ushort)m_stnMaxIndex;
		}

		#endregion
		#endregion//업무규칙

		#region |  ----- 실행질의문 관련 -----  |
		#region |  void ShowRuntimeSQLView(string strSQL)  |
		/*
		 * 2008.01.16 황성민
		 * 실행질의문 창 논리 변경에 의해서 DOC에서 관리합니다.
		 */
		public override void ShowRuntimeSQLView (string strSQL)
		{
			if (null == pUserQueryDialog)
			{
				pUserQueryDialog = new UserQueryDialog ();
				//pUserQueryDialog.StartPosition = FormStartPosition.Manual;
				pUserQueryDialog.Closed += new EventHandler (pUserQueryDialog_Closed);

				//CDMTFrame pFrame = this.GetThisForm() as CDMTFrame;
				//if (null != pFrame)
				//{
				//    Form pTopForm = pFrame.TopLevelControl as Form;
				//    if (null != pTopForm)
				//    {
				//        pUserQueryDialog.MdiParent = pTopForm;

				//        int nX = pTopForm.Width - pUserQueryDialog.Width - 140;
				//        int nY = pTopForm.Height - pUserQueryDialog.Height - 140;

				//        if (0 > nX)
				//            nX = 0;
				//        if (0 > nY)
				//            nY = 0;
				//        pUserQueryDialog.Location = new Point(nX, nY);
				//    }
				//}
			}
			pUserQueryDialog.QueryText = strSQL;
			pUserQueryDialog.Focus ();
			pUserQueryDialog.Show ();
		}

		#endregion

		#region |  void SetFormInsertMode(bool bInsert)  |
		/*
		 * 2008.01.16 황성민
		 * 실행질의문 중 [수정][저장] 모드를 확인하기 위해서 사용합니다.
		 */
		public override void SetFormInsertMode (bool bInsert)
		{
			m_pThisFormInsertMode = bInsert;
		}

		#endregion

		#region |  bool GetFormInsertMode()  |
		/*
		 * 2008.01.16 황성민
		 * 실행질의문 중 [수정][저장] 모드를 확인하기 위해서 사용합니다.
		 */
		public override bool GetFormInsertMode ()
		{
			return m_pThisFormInsertMode;
		}

		#endregion
		#endregion//실행질의문 관련

		#region |  ----- EBook 관련 -----  |
		#region |  void DetachCurrentDocument_OnEBook()  |

		public override void DetachCurrentDocument_OnEBook ()
		{
			RefreshAnimationCoreManager ();

			if (null != AnimationWindow)
			{
				FormLoadAnimationPlayed = false;
				AnimationWindow.AnimationLayer = null;
				AnimationWindow.OnGetAllAtomList -= AnimationWindow_OnGetAllAtomList;
				AnimationWindow.OnGetAnimationManager -= AnimationWindow_OnGetAnimationManager;
				AnimationWindow.OnMakeAnimationGroup -= AnimationWindow_OnMakeAnimationGroup;
				AnimationWindow.OnDeleteAnimationGroup -= AnimationWindow_OnDeleteAnimationGroup;
			}
		}

		#endregion

		#region |  void AttachNewDocument_OnEBook()  |

		public override void AttachNewDocument_OnEBook ()
		{
			RefreshAnimationCoreManager ();

			if (null != AnimationWindow)
			{
				AnimationWindow.AnimationLayer = ((DMTView)GetParentView ()).AnimationLayer;

				AnimationWindow.OnGetAllAtomList -= AnimationWindow_OnGetAllAtomList;
				AnimationWindow.OnGetAnimationManager -= AnimationWindow_OnGetAnimationManager;
				AnimationWindow.OnMakeAnimationGroup -= AnimationWindow_OnMakeAnimationGroup;
				AnimationWindow.OnDeleteAnimationGroup -= AnimationWindow_OnDeleteAnimationGroup;

				AnimationWindow.OnGetAllAtomList += AnimationWindow_OnGetAllAtomList;
				AnimationWindow.OnGetAnimationManager += AnimationWindow_OnGetAnimationManager;
				AnimationWindow.OnMakeAnimationGroup += AnimationWindow_OnMakeAnimationGroup;
				AnimationWindow.OnDeleteAnimationGroup += AnimationWindow_OnDeleteAnimationGroup;

			}
		}

		#endregion
		#endregion//EBook 관련

		#region |  AtomBase MakeAnimationGroupAtom()  |
		/// <summary>
		/// 애니메이션그룹 아톰을 만든다.
		/// </summary>
		/// <returns></returns>
		public override AtomBase MakeAnimationGroupAtom ()
		{
			if (true == PreCheckAnimationGroupMake ())
			{
				ArrayList alMakedAtomList = GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyMakeAtom (AtomType.EBookAnimationGroup, false, true);

				if (null == alMakedAtomList || 0 == alMakedAtomList.Count)
				{
					return null;
				}

				return alMakedAtomList[0] as AtomBase;
			}

			return null;
		}

		public override AtomBase DeleteAnimationGroupAtom ()
		{
			DMTView CurrentDMTView = GetParentView () as DMTView;
			CurrentDMTView.DeleteCurrentSelectedAtoms ();

			return null;
		}

		#endregion

		#endregion //Public Override 메서드

		#region |  ##### Public 메서드 #####  |

		/// <summary>
		///  마우스가 눌린 지점에 있는 아톰들을 모두 가져온 후에 ZIndex가 가장 높은 탭뷰아톰을 가져온다. 단 ExceptAtom은 TempAtom이므로 제외한다.
		/// </summary>
		/// <param name="PointOverLightDMTView"></param>
		/// <param name="ExceptAtom"></param>
		/// <returns></returns>
		public AtomBase GetTopMostTabViewAtomOfMousePosition (Point PointOverLightDMTView, AtomBase ExceptAtom)
		{
			List<object> AtomListOfMoustPoint = new List<object> ();
			AtomBase TopMostAtom = null;
			int nPrevZIndex = -1;

			UIElementCollection CurrentChildren = GetChildren ();
			int nCurrentChildrenCount = CurrentChildren.Count;

			for (int nIndex = 0; nIndex < nCurrentChildrenCount; nIndex++)
			{
				AtomBase currentAtom = CurrentChildren[nIndex] as AtomBase;

				if (null != currentAtom)
				{
					if (ExceptAtom != currentAtom)
					{
						//2019-10-21 kys 탭뷰에 포함되는 아톰 위치를 비교할때 기준점을 왼쪽 상단이 아닌 아톰 중심으로 지정해준다.
						Point point = new Point ();
						point.X = PointOverLightDMTView.X + ExceptAtom.Width / 2;
						point.Y = PointOverLightDMTView.Y + ExceptAtom.Height / 2;

						double dAtomStartX = currentAtom.Margin.Left;
						double dAtomStartY = currentAtom.Margin.Top;
						double dAtomEndX = (currentAtom.Margin.Left + currentAtom.Width);
						double dAtomEndY = (currentAtom.Margin.Top + currentAtom.Height);

						if (point.X >= dAtomStartX && point.X <= dAtomEndX && point.Y >= dAtomStartY && point.Y <= dAtomEndY)
						{
							AtomListOfMoustPoint.Add (currentAtom);
						}
					}
				}

				currentAtom = null;
			}

			foreach (AtomBase CurrentAtom in AtomListOfMoustPoint)
			{
				int nCurrentAtomZIndex = Canvas.GetZIndex (CurrentAtom);

				if (nPrevZIndex <= nCurrentAtomZIndex)
				{
					TopMostAtom = CurrentAtom;
					nPrevZIndex = nCurrentAtomZIndex;
				}
			}

			AtomListOfMoustPoint.Clear ();
			AtomListOfMoustPoint = null;
			CurrentChildren = null;

			if (null != TopMostAtom)
			{
				Type TopMostAtomType = TopMostAtom.GetType ();

				if (TopMostAtom is TabViewAtomBase)
				{
					return TopMostAtom;
				}
			}

			return null;
		}

		public AtomBase ReadyAtom (AtomType AtomKey, Atom atomCore)
		{
			bool bIsSerialize = atomCore != null ? true : false;
			AtomBase ReadyAtom = null;

			switch (AtomKey)
			{
				case AtomType.VHLine:
					{
						if (null == atomCore)
						{
							ReadyAtom = new VHLineofAtom ();
						}
						else
						{
							ReadyAtom = new VHLineofAtom (atomCore);
						}
						break;
					}

				case AtomType.FreeLine:
					{
						if (null == atomCore)
						{
							ReadyAtom = new FreeLineofAtom ();
						}
						else
						{
							ReadyAtom = new FreeLineofAtom (atomCore);
						}
						break;
					}

				case AtomType.Square:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SquareofAtom ();
						}
						else
						{
							ReadyAtom = new SquareofAtom (atomCore);

						}
						break;
					}

				case AtomType.Oval:
					{
						if (null == atomCore)
						{
							ReadyAtom = new OvalofAtom ();
						}
						else
						{
							ReadyAtom = new OvalofAtom (atomCore);
						}
						break;
					}

				case AtomType.RoundRectangle:
					{
						if (null == atomCore)
						{
							ReadyAtom = new RoundSquareofAtom ();
						}
						else
						{
							ReadyAtom = new RoundSquareofAtom (atomCore);
						}
						break;
					}

				case AtomType.DecorImage:
					{
						if (null == atomCore)
						{
							ReadyAtom = new DecorImageofAtom ();
						}
						else
						{
							ReadyAtom = new DecorImageofAtom (atomCore);
						}
						break;
					}
				case AtomType.AnimationImage:
					{
						if (null == atomCore)
						{
							ReadyAtom = new AnimationImageofAtom ();
						}
						else
						{
							ReadyAtom = new AnimationImageofAtom (atomCore);
						}
						break;
					}
				case AtomType.TabView:
					{
						if (null == atomCore)
						{
							ReadyAtom = new TabViewofAtom ();
						}
						else
						{
							ReadyAtom = new TabViewofAtom (atomCore);
						}
						break;
					}
				case AtomType.ExpandView:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ExpandViewofAtom ();
						}
						else
						{
							ReadyAtom = new ExpandViewofAtom (atomCore);
						}
						break;
					}
				case AtomType.ScrollView:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ScrollViewofAtom ();
						}
						else
						{
							ReadyAtom = new ScrollViewofAtom (atomCore);
						}
						break;
					}
				case AtomType.SlidingView:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SlidingViewofAtom ();
						}
						else
						{
							ReadyAtom = new SlidingViewofAtom (atomCore);
						}
						break;
					}
				case AtomType.AccordionView:
					{
						if (null == atomCore)
						{
							ReadyAtom = new AccordionViewofAtom ();
						}
						else
						{
							ReadyAtom = new AccordionViewofAtom (atomCore);
						}
						break;
					}
				case AtomType.GridTable:
					{
						IMakeRemoveAtomInGridTable makeRemoveAtomInGridTable = GetParentView () as IMakeRemoveAtomInGridTable;

						if (null == atomCore)
						{
							ReadyAtom = new GridTableOfAtom (makeRemoveAtomInGridTable);
						}
						else
						{
							ReadyAtom = new GridTableOfAtom (atomCore, makeRemoveAtomInGridTable);
						}
						break;
					}
				case AtomType.GridTableEx:
					{
						if (null == atomCore)
						{
							ReadyAtom = new GridTableExofAtom ();
						}
						else
						{
							ReadyAtom = new GridTableExofAtom (atomCore);
						}
						break;
					}
				case AtomType.DataInput:
					{
						if (null == atomCore)
						{
							ReadyAtom = new InputofAtom ();
						}
						else
						{
							ReadyAtom = new InputofAtom (atomCore);
						}
						break;
					}

				case AtomType.DateInput:
					{
						if (null == atomCore)
						{
							ReadyAtom = new DateofAtom ();
						}
						else
						{
							ReadyAtom = new DateofAtom (atomCore);
						}
						break;
					}

				case AtomType.Scan:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ScanofAtom ();
						}
						else
						{
							ReadyAtom = new ScanofAtom (atomCore);
						}
						break;
					}

				case AtomType.Audio:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartAudioofAtom ();
						}
						else
						{
							ReadyAtom = new SmartAudioofAtom (atomCore);
						}
						break;
					}

				case AtomType.MultiMedia: // 180918_AHN
					{
						if (null == atomCore)
						{
							ReadyAtom = new MultiMediaofAtom ();
						}
						else
						{
							ReadyAtom = new MultiMediaofAtom (atomCore);
						}
						break;
					}

				case AtomType.Popup:
					{
						bool bIsPossible = IsExistCurrentSelectedAtoms ();

						if (null == atomCore)
						{
							if (false != bIsPossible)
							{
								ReadyAtom = new PopupofAtom ();
								ReadyPopupOrHyperAtom (ReadyAtom);
								return ReadyAtom;
							}
						}
						else
						{
							ReadyAtom = new PopupofAtom (atomCore);
							ReadyPopupOrHyperAtom (atomCore, ReadyAtom);
							return ReadyAtom;
						}
					}
					break;
				case AtomType.EBookAnimationGroup:
					{
						bool bIsPossible = IsExistCurrentSelectedAtoms ();

						if (null == atomCore)
						{
							ReadyAtom = new EBookAnimationGroupofAtom ();
							ReadyAnimationGroupAtom (ReadyAtom);
							return ReadyAtom;
						}
						else
						{
							ReadyAtom = new EBookAnimationGroupofAtom (atomCore);
							ReadyPopupOrHyperAtom (atomCore, ReadyAtom);
							return ReadyAtom;
						}
					}
				case AtomType.WebHyperLink:
					{
						bool bIsPossible = IsExistCurrentSelectedAtoms ();

						if (null == atomCore)
						{
							ReadyAtom = new WebHyperDataofAtom ();

							if (false == ReadyPopupOrHyperAtom (ReadyAtom))
							{
								ReadyAtom = null;
								break;
							}

							if (0 < ((WebHyperDataofAtom)ReadyAtom).GetBindedAtoms ().Count)
							{
								return ReadyAtom;
							}
						}
						else
						{
							ReadyAtom = new WebHyperDataofAtom (atomCore);
							ReadyPopupOrHyperAtom (atomCore, ReadyAtom);
							return ReadyAtom;
						}
						break;
					}
				case AtomType.WebHyperLink70:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebHyperLinkofAtom ();
							return ReadyAtom;
						}
						else
						{
							ReadyAtom = new WebHyperDataofAtom (atomCore);
							return ReadyAtom;
						}
					}
				case AtomType.WebQuickLink:
					{
						bool bIsPossible = IsExistCurrentSelectedAtoms ();

						if (null == atomCore)
						{
							ReadyAtom = new WebQuickLinkofAtom ();
							ReadyWebQuickLinkAtom (ReadyAtom);
							return ReadyAtom;
						}
						else
						{
							ReadyAtom = new WebQuickLinkofAtom (atomCore);
							ReadyPopupOrHyperAtom (atomCore, ReadyAtom);
							return ReadyAtom;
						}
					}
				case AtomType.FloatingBar:
					{
						bool bIsPossible = IsExistCurrentSelectedAtoms ();

						if (null == atomCore)
						{
							ReadyAtom = new FloatingBarofAtom ();
						}
						else
						{
							ReadyAtom = new FloatingBarofAtom (atomCore);
						}
					}
					break;
				case AtomType.Combobox:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ComboofAtom ();
						}
						else
						{
							ReadyAtom = new ComboofAtom (atomCore);
						}
						break;
					}

				case AtomType.Radio:
					{
						if (null == atomCore)
						{
							ReadyAtom = new RadioofAtom ();
						}
						else
						{
							ReadyAtom = new RadioofAtom (atomCore);
						}
						break;
					}

				case AtomType.StructureGrid:
					{
						if (null == atomCore)
						{
							ReadyAtom = new CStructureGridofAtom ();
						}
						else
						{
							ReadyAtom = new CStructureGridofAtom (atomCore);
						}
						break;
					}
				case AtomType.Checkbox:
					{
						if (null == atomCore)
						{
							ReadyAtom = new CheckofAtom ();
						}
						else
						{
							ReadyAtom = new CheckofAtom (atomCore);
						}
						break;
					}
				case AtomType.Paint:
					{
						if (null == atomCore)
						{
							ReadyAtom = new PaintofAtom ();
						}
						else
						{
							ReadyAtom = new PaintofAtom (atomCore);
						}
						break;
					}

				case AtomType.ToggleSwitch:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ToggleSwitchofAtom ();
						}
						else
						{
							ReadyAtom = new ToggleSwitchofAtom (atomCore);
						}
						break;
					}

				case AtomType.BindScroll:
					{
						//if (null == atomCore)
						//{
						//    bool bIsPossible = IsExistCurrentSelectedAtoms();

						//    if (true == bIsPossible)
						//    {
						//        ReadyAtom = new ScrollofAtom();
						//        ReadyScrollAtom((ScrollAtomBase)ReadyAtom);
						//        return ReadyAtom;
						//    }
						//}
						//else
						//{
						//    ReadyAtom = new ScrollofAtom(atomCore);
						//    ReadyScrollAtom(atomCore, (ScrollAtomBase)ReadyAtom);
						//    return ReadyAtom;
						//}

						bool bIsMultiMeadiAtom = false;
						foreach (AtomBase _atomBase in GetCurrentSelectedAtomsInLightDMTView ())
						{
							if (_atomBase is MultiMediaAtomBase)
							{
								bIsMultiMeadiAtom = true;
								break;
							}
						}

						if (null == atomCore)
						{
							bool bIsPossible = IsExistCurrentSelectedAtoms ();

							if (true == bIsPossible)
							{
								if (bIsMultiMeadiAtom)
								{
									ReadyAtom = new ExtensionScrollofAtom ();
									ReadyScrollAtom ((ExtensionScrollofAtom)ReadyAtom);
								}
								else
								{
									ReadyAtom = new ScrollofAtom ();
									ReadyScrollAtom ((ScrollAtomBase)ReadyAtom);
								}
								return ReadyAtom;
							}
						}
						else
						{
							if (bIsMultiMeadiAtom)
							{
								ReadyAtom = new ExtensionScrollofAtom (atomCore);
								ReadyScrollAtom (atomCore, (ExtensionScrollofAtom)ReadyAtom);
							}
							else
							{
								ReadyAtom = new ScrollofAtom (atomCore);
								ReadyScrollAtom (atomCore, (ScrollAtomBase)ReadyAtom);
							}
							return ReadyAtom;
						}

						break;
					}
				case AtomType.BindExtensionScroll: // 181102_AHN_EXTENSIONSCROLL
					{
						if (null == atomCore)
						{
							bool bIsPossible = IsExistCurrentSelectedAtoms ();

							if (true == bIsPossible)
							{
								ReadyAtom = new ExtensionScrollofAtom ();
								ReadyScrollAtom ((ExtensionScrollofAtom)ReadyAtom);
								return ReadyAtom;
							}
						}
						else
						{
							ReadyAtom = new ExtensionScrollofAtom (atomCore);
							ReadyScrollAtom (atomCore, (ExtensionScrollofAtom)ReadyAtom);
							return ReadyAtom;
						}

						break;
					}
				case AtomType.WebDisplay:
					{
						if (null == atomCore)
						{
							bool bIsPossible = IsExistCurrentSelectedAtoms ();

							if (true == bIsPossible)
							{
								ReadyAtom = new WebDisplayofAtom ();
								ReadyScrollAtom ((WebDisplayofAtom)ReadyAtom);
								return ReadyAtom;
							}
						}
						else
						{
							ReadyAtom = new WebDisplayofAtom (atomCore);
							ReadyScrollAtom (atomCore, (WebDisplayofAtom)ReadyAtom);
							return ReadyAtom;
						}
						break;
					}
				case AtomType.WebScroll:
					{
						if (null == atomCore)
						{
							bool bIsPossible = IsExistCurrentSelectedAtoms ();

							if (true == bIsPossible)
							{
								ReadyAtom = new WebScrollPageofAtom ();
								ReadyScrollAtom ((WebScrollPageofAtom)ReadyAtom);
								return ReadyAtom;
							}
						}
						else
						{
							ReadyAtom = new WebScrollPageofAtom (atomCore);
							ReadyScrollAtom (atomCore, (WebScrollPageofAtom)ReadyAtom);
							return ReadyAtom;
						}
						break;
					}
				case AtomType.WebRss:
					{
						if (null == atomCore)
						{
							bool bIsPossible = IsExistCurrentSelectedAtoms ();

							if (true == bIsPossible)
							{
								ReadyAtom = new WebRssofAtom ();
								ReadyScrollAtom ((WebRssofAtom)ReadyAtom);
								return ReadyAtom;
							}
						}
						else
						{
							ReadyAtom = new WebRssofAtom (atomCore);
							ReadyScrollAtom (atomCore, (WebRssofAtom)ReadyAtom);
							return ReadyAtom;
						}
						break;
					}
				case AtomType.WebBlock:
					{
						bool bIsPossible = IsExistCurrentSelectedAtoms ();

						if (null == atomCore)
						{
							if (false != bIsPossible)
							{
								ReadyAtom = new WebBlockofAtom ();
								ReadyPopupOrHyperAtom (ReadyAtom);
								return ReadyAtom;
							}
						}
						else
						{
							ReadyAtom = new WebBlockofAtom (atomCore);
							ReadyPopupOrHyperAtom (atomCore, ReadyAtom);
							return ReadyAtom;
						}
						break;
					}
				case AtomType.LabelNum:
					{
						if (null == atomCore)
						{
							ReadyAtom = new LabelNumofAtom ();
						}
						else
						{
							ReadyAtom = new LabelNumofAtom (atomCore);
						}
						break;
					}

				case AtomType.DataTree:
					{

						if (null == atomCore)
						{
							ReadyAtom = new TreeofAtom ();
						}
						else
						{
							ReadyAtom = new TreeofAtom (atomCore);
						}
						break;
					}
				case AtomType.StructureTree:
					{
						if (null == atomCore)
						{
							ReadyAtom = new StructureTreeofAtom ();
						}
						else
						{
							ReadyAtom = new StructureTreeofAtom (atomCore);
						}
					}
					break;
				case AtomType.DataGrid:
					{
						if (null == atomCore)
						{
							ReadyAtom = new CDBGridExofAtom ();
						}
						else
						{
							ReadyAtom = new CDBGridExofAtom (atomCore);
						}
						break;
					}
				case AtomType.Chart:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ChartofAtom ();
						}
						else
						{
							ReadyAtom = new ChartofAtom (atomCore);
						}
						break;
					}
				case AtomType.ChartJS:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ChartJSofAtom ();
						}
						else
						{
							ReadyAtom = new ChartJSofAtom (atomCore);
						}
						break;
					}

				case AtomType.ReportBrowse:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ReportBrowseofAtom ();
						}
						else
						{
							ReadyAtom = new ReportBrowseofAtom (atomCore);
						}
						break;
					}

				case AtomType.SearchCondition:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SearchConditionofAtom ();
						}
						else
						{
							ReadyAtom = new SearchConditionofAtom (atomCore);
						}
						break;
					}

				case AtomType.CarouselImage:
					{
						if (null == atomCore)
						{
							ReadyAtom = new CarouselImageofAtom ();
						}
						else
						{
							ReadyAtom = new CarouselImageofAtom (atomCore);
						}
						break;
					}
				case AtomType.Gauge:
					{
						if (null == atomCore)
						{
							ReadyAtom = new GaugeofAtom ();
						}
						else
						{
							ReadyAtom = new GaugeofAtom (atomCore);
						}
						break;
					}

				case AtomType.InputSpinner:
					{
						if (null == atomCore)
						{
							ReadyAtom = new InputSpinnerofAtom ();
						}
						else
						{
							ReadyAtom = new InputSpinnerofAtom (atomCore);
						}
						break;
					}

				case AtomType.FunctionButton:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ActionofAtom ();
						}
						else
						{
							ReadyAtom = new ActionofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebMenu:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebDummyMenuofAtom ();
						}
						else
						{
							ReadyAtom = new WebDummyMenuofAtom (atomCore);
						}
						break;
					}

				case AtomType.WebDropMenu:
				case AtomType.WebSlideMenu:
				case AtomType.WebTabMenu:
				case AtomType.WebLabelMenu:
				case AtomType.WebLinkMenu:
				case AtomType.WebTreeMenu:
				case AtomType.WebComboMenu:
				case AtomType.WebCoverFlowMenu:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebDummyMenuofAtom ();
						}
						else
						{
							ReadyAtom = new WebDummyMenuofAtom (atomCore);
						}
						break;
					}

				case AtomType.Location:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartLocAddrofAtom ();
						}
						else
						{
							ReadyAtom = new SmartLocAddrofAtom (atomCore);
						}
						break;
					}

				case AtomType.Signal:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SignalofAtom ();
						}
						else
						{
							ReadyAtom = new SignalofAtom (atomCore);
						}
						break;
					}

				case AtomType.Schedule:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ScheduleofAtom ();
						}
						else
						{
							ReadyAtom = new ScheduleofAtom (atomCore);
						}
						break;
					}

				case AtomType.Progress:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ProgressofAtom ();
						}
						else
						{
							ReadyAtom = new ProgressofAtom (atomCore);
						}
						break;
					}

				case AtomType.VerticalProgress:
					{
						if (null == atomCore)
						{
							ReadyAtom = new VerticalProgressofAtom ();
						}
						else
						{
							ReadyAtom = new VerticalProgressofAtom (atomCore);
						}
						break;
					}

				case AtomType.RadialProgress:
					{
						if (null == atomCore)
						{
							ReadyAtom = new RadialProgressofAtom ();
						}
						else
						{
							ReadyAtom = new RadialProgressofAtom (atomCore);
						}
						break;
					}

				case AtomType.Slider:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SliderofAtom ();
						}
						else
						{
							ReadyAtom = new SliderofAtom (atomCore);
						}
						break;
					}

				case AtomType.VerticalSlider:
					{
						if (null == atomCore)
						{
							ReadyAtom = new VerticalSliderofAtom ();
						}
						else
						{
							ReadyAtom = new VerticalSliderofAtom (atomCore);
						}
						break;
					}

				case AtomType.RadialSlider:
					{
						if (null == atomCore)
						{
							ReadyAtom = new RadialSliderofAtom ();
						}
						else
						{
							ReadyAtom = new RadialSliderofAtom (atomCore);
						}
						break;
					}

				case AtomType.RangeSlider:
					{
						if (null == atomCore)
						{
							ReadyAtom = new RangeSliderofAtom ();
						}
						else
						{
							ReadyAtom = new RangeSliderofAtom (atomCore);
						}
						break;
					}

				case AtomType.VerticalRangeSlider:
					{
						if (null == atomCore)
						{
							ReadyAtom = new VerticalRangeSliderofAtom ();
						}
						else
						{
							ReadyAtom = new VerticalRangeSliderofAtom (atomCore);
						}
						break;
					}

				case AtomType.RadialRangeSlider:
					{
						if (null == atomCore)
						{
							ReadyAtom = new RadialRangeSliderofAtom ();
						}
						else
						{
							ReadyAtom = new RadialRangeSliderofAtom (atomCore);
						}
						break;
					}

				case AtomType.Message:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartMessageofAtom ();
						}
						else
						{
							ReadyAtom = new SmartMessageofAtom (atomCore);
						}
						break;
					}
				case AtomType.AttachFile:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartAttachofAtom ();
						}
						else
						{
							ReadyAtom = new SmartAttachofAtom (atomCore);
						}
						break;
					}
				case AtomType.Calendar:
					{
						if (null == atomCore)
						{
							ReadyAtom = new CalendarofAtom ();
						}
						else
						{
							ReadyAtom = new CalendarofAtom (atomCore);
						}
					}
					break;
				case AtomType.AnalogClock:
					{
						if (null == atomCore)
						{
							ReadyAtom = new AnalogClockofAtom ();
						}
						else
						{
							ReadyAtom = new AnalogClockofAtom (atomCore);
						}
						break;
					}
				case AtomType.DigitalClock:
					{
						if (null == atomCore)
						{
							ReadyAtom = new DigitalClockofAtom ();
						}
						else
						{
							ReadyAtom = new DigitalClockofAtom (atomCore);
						}
						break;
					}
				case AtomType.RatingBar:
					{
						if (null == atomCore)
						{
							ReadyAtom = new RatingBarofAtom ();
						}
						else
						{
							ReadyAtom = new RatingBarofAtom (atomCore);
						}
						break;
					}
				case AtomType.QRCode:
					{
						if (null == atomCore)
						{
							ReadyAtom = new QRCodeofAtom ();
						}
						else
						{
							ReadyAtom = new QRCodeofAtom (atomCore);
						}
						break;
					}
				case AtomType.BarCode:
					{
						if (null == atomCore)
						{
							ReadyAtom = new BarCodeofAtom ();
						}
						else
						{
							ReadyAtom = new BarCodeofAtom (atomCore);
						}
						break;
					}
				case AtomType.AddExternDocument:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ExternDocumenViewerofAtom ();
						}
						else
						{
							ReadyAtom = new ExternDocumenViewerofAtom (atomCore);
						}
						break;
					}
				case AtomType.AddWebDocument:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebIFrameofAtom ();
						}
						else
						{
							ReadyAtom = new WebIFrameofAtom (atomCore);
						}
						break;
					}
				case AtomType.Media:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebFlashofAtom ();
						}
						else
						{
							ReadyAtom = new WebFlashofAtom (atomCore);
						}
						break;
					}
				case AtomType.Map:
				case AtomType.GoogleMap:
				case AtomType.NaverMap:
				case AtomType.KakaoMap:
				case AtomType.TMap:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartMapViewofAtom ();
						}
						else
						{
							ReadyAtom = new SmartMapViewofAtom (atomCore);
						}
						break;
					}

				case AtomType.Advertise:
					{
						if (null == atomCore)
						{
							ReadyAtom = new AdvertiseofAtom ();
						}
						else
						{
							ReadyAtom = new AdvertiseofAtom (atomCore);
						}
						break;
					}
				case AtomType.Bluetooth:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartBluetoothofAtom ();
						}
						else
						{
							ReadyAtom = new SmartBluetoothofAtom (atomCore);
						}
						break;
					}
				case AtomType.SerialCom:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartSerialComofAtom ();
						}
						else
						{
							ReadyAtom = new SmartSerialComofAtom (atomCore);
						}
						break;
					}
				case AtomType.Socket:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartSocketofAtom ();
						}
						else
						{
							ReadyAtom = new SmartSocketofAtom (atomCore);
						}
						break;
					}
				case AtomType.NfcAdapter:
					{
						if (null == atomCore)
						{
							ReadyAtom = new NfcAdapterofAtom ();
						}
						else
						{
							ReadyAtom = new NfcAdapterofAtom (atomCore);
						}
						break;
					}
				case AtomType.Messenger:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartMessengerofAtom ();
						}
						else
						{
							ReadyAtom = new SmartMessengerofAtom (atomCore);
						}
						break;
					}
				case AtomType.YouTube:
					{
						if (null == atomCore)
						{
							ReadyAtom = new YouTubeofAtom ();
						}
						else
						{
							ReadyAtom = new YouTubeofAtom (atomCore);
						}
						break;
					}
				case AtomType.Postal:
					{
						if (null == atomCore)
						{
							ReadyAtom = new PostalofAtom ();
						}
						else
						{
							ReadyAtom = new PostalofAtom (atomCore);
						}
						break;
					}
				case AtomType.RadialMenu:
					{
						if (null == atomCore)
						{
							ReadyAtom = new RadialMenuofAtom ();
						}
						else
						{
							ReadyAtom = new RadialMenuofAtom (atomCore);
						}
						break;
					}
				case AtomType.Indicator:
					{
						if (null == atomCore)
						{
							ReadyAtom = new IndicatorofAtom ();
						}
						else
						{
							ReadyAtom = new IndicatorofAtom (atomCore);
						}
						break;
					}
				case AtomType.AutoComplete:
					{
						bool bIsPossible = IsExistCurrentSelectedAtoms ();

						if (null == atomCore)
						{
							ReadyAtom = new AutoCompleteofAtom ();
							ReadyPopupOrHyperAtom (ReadyAtom);
							return ReadyAtom;
						}
						else
						{
							ReadyAtom = new AutoCompleteofAtom (atomCore);
							ReadyPopupOrHyperAtom (atomCore, ReadyAtom);
							return ReadyAtom;
						}
					}
				case AtomType.InAppBilling:
					{
						if (null == atomCore)
						{
							ReadyAtom = new InAppBillingofAtom ();
						}
						else
						{
							ReadyAtom = new InAppBillingofAtom (atomCore);
						}
						break;
					}
				case AtomType.ApprovalLine:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartApprovalofAtom ();
						}
						else
						{
							ReadyAtom = new SmartApprovalofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebDataTable:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebTableofAtom ();
						}
						else
						{
							ReadyAtom = new WebTableofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebPG:
					{
						if (null == atomCore)
						{
							ReadyAtom = new PGofAtom ();
						}
						else
						{
							ReadyAtom = new PGofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebKakaoPay:
					{
						if (null == atomCore)
						{
							ReadyAtom = new KakaoPayofAtom ();
						}
						else
						{
							ReadyAtom = new KakaoPayofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebSoftBank:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SoftBankofAtom ();
						}
						else
						{
							ReadyAtom = new SoftBankofAtom (atomCore);
						}
						break;
					}
				case AtomType.PaddleInApp:
					{
						if (null == atomCore)
						{
							ReadyAtom = new PaddleInAppofAtom ();
						}
						else
						{
							ReadyAtom = new PaddleInAppofAtom (atomCore);
						}
						break;
					}
				case AtomType.Paypal:
					{
						if (null == atomCore)
						{
							ReadyAtom = new PaypalofAtom ();
						}
						else
						{
							ReadyAtom = new PaypalofAtom (atomCore);
						}
						break;
					}
				case AtomType.IamPort:
					{
						if (null == atomCore)
						{
							ReadyAtom = new IamportofAtom ();
						}
						else
						{
							ReadyAtom = new IamportofAtom (atomCore);
						}
					}
					break;
				case AtomType.SNSLogin:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SNSLoginofAtom ();
						}
						else
						{
							ReadyAtom = new SNSLoginofAtom (atomCore);
						}
						break;
					}
				case AtomType.PassAuth:
					{
						if (null == atomCore)
						{
							ReadyAtom = new PassAuthofAtom ();
						}
						else
						{
							ReadyAtom = new PassAuthofAtom (atomCore);
						}
						break;
					}
				case AtomType.BiometricAuth:
					{
						if (null == atomCore)
						{
							ReadyAtom = new BiometricofAtom ();
						}
						else
						{
							ReadyAtom = new BiometricofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebBoard:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebBoardofAtom ();
						}
						else
						{
							ReadyAtom = new WebBoardofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebLogin:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebLoginofAtom ();
						}
						else
						{
							ReadyAtom = new WebLoginofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebFileAttach:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebFileAttachofAtom ();
						}
						else
						{
							ReadyAtom = new WebFileAttachofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebFileUpload:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebFileUploadofAtom ();
						}
						else
						{
							ReadyAtom = new WebFileUploadofAtom (atomCore);
						}
						break;
					}
					break;
				case AtomType.WebPicture:
					{
						if (null == atomCore)
						{
							ReadyAtom = new PictureofAtom ();
						}
						else
						{
							ReadyAtom = new PictureofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebDHtmlEdit:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebDHtmlEditofAtom ();
						}
						else
						{
							ReadyAtom = new WebDHtmlEditofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebHtmlTag:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebHtmlTagofAtom ();
						}
						else
						{
							ReadyAtom = new WebHtmlTagofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebInsertModel:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebModelofAtom ();
						}
						else
						{
							ReadyAtom = new WebModelofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebTabView:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebTabPanelofAtom ();
						}
						else
						{
							ReadyAtom = new WebTabPanelofAtom (atomCore);
						}
						break;
					}
				case AtomType.WebSlide:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebSlidePanelofAtom ();
						}
						else
						{
							ReadyAtom = new WebSlidePanelofAtom (atomCore);
							bIsSerialize = true;
						}
						break;
					}
				case AtomType.WebAdsense:
					{
						if (null == atomCore)
						{
							ReadyAtom = new WebAdsenseofAtom ();
						}
						else
						{
							ReadyAtom = new WebAdsenseofAtom (atomCore);
						}
						break;
					}
				case AtomType.EBookText:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookTextofAtom ();
						}
						else
						{
							ReadyAtom = new EBookTextofAtom (atomCore);
						}
						break;
					}
				case AtomType.EBookImage:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookImageofAtom ();
						}
						else
						{
							ReadyAtom = new EBookImageofAtom (atomCore);
						}
						break;
					}
				case AtomType.EBookAvatar:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookAvatarofAtom ();
						}
						else
						{
							ReadyAtom = new EBookAvatarofAtom (atomCore);
						}
						break;
					}
				case AtomType.EBookVoice:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookVoiceofAtom ();
						}
						else
						{
							ReadyAtom = new EBookVoiceofAtom (atomCore);
						}
						break;
					}
				case AtomType.EBookMedia:
					{
						// 스마트콘텐츠 아톰 확장기능보강 
						//if (PQAppBase.IsSmartBookSerialize)
						{
							if (null == atomCore)
							{
								ReadyAtom = new EBookMediaofAtom ();
							}
							else
							{
								ReadyAtom = new EBookMediaofAtom (atomCore);
							}
						}
						//else
						//{
						//    if (null == atomCore)
						//    {
						//        ReadyAtom = new FlashofAtom ();
						//    }
						//    else
						//    {
						//        ReadyAtom = new FlashofAtom (atomCore);
						//    }
						//}
						break;
					}
				case AtomType.EBookChart:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookChartofAtom ();
						}
						else
						{
							ReadyAtom = new EBookChartofAtom (atomCore);
						}
						break;
					}
				case AtomType.EBookMultiMedia:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookMultiMediaofAtom ();
						}
						else
						{
							ReadyAtom = new EBookMultiMediaofAtom (atomCore);
						}
						break;
					}
				case AtomType.EBookFigure:
				case AtomType.EBookSpeechBalloon:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookFigureOfAtom ();
							DMTView currentView = GetParentView () as DMTView;
							((EBookFigureAtom)ReadyAtom.AtomCore).SetFigureType (null, currentView.MakeFigureType);
						}
						else
						{
							ReadyAtom = new EBookFigureOfAtom (atomCore);
						}
						break;
					}
				case AtomType.EBookPage:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookPageofAtom ();
						}
						else
						{
							ReadyAtom = new EBookPageofAtom (atomCore);
						}
						break;
					}
				case AtomType.EBookQuestions:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookQuestionsofAtom ();
						}
						else
						{
							ReadyAtom = new EBookQuestionsofAtom (atomCore);
						}
						break;
					}
				case AtomType.EBookPageNumber:
					{
						if (null == atomCore)
						{
							ReadyAtom = new PageNumberofAtom ();
						}
						else
						{
							ReadyAtom = new PageNumberofAtom (atomCore);
						}
						break;
					}

				case AtomType.CanvasEditor:
					{
						if (null == atomCore)
						{
							ReadyAtom = new CanvasEditorofAtom ();
						}
						else
						{
							ReadyAtom = new CanvasEditorofAtom (atomCore);
						}
					}
					break;
				case AtomType.EBookGrapPaper:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookGrapPaperofAtom ();
						}
						else
						{
							ReadyAtom = new EBookGrapPaperofAtom (atomCore);
						}
					}
					break;
				case AtomType.EBookQuizView:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookQuizViewofAtom ();
							var atomAttrib = ReadyAtom.AtomCore.GetAttrib () as EBookQuizViewAttrib;

							DMTView currentView = GetParentView () as DMTView;
							atomAttrib.DisplayQuizType = currentView.MakeQuizType;

							bool isView = ReadyEBookQuizViewAtom (ReadyAtom as EBookQuizViewAtomBase);

							if (true == isView)
								return ReadyAtom;
						}
						else
						{
							ReadyAtom = new EBookQuizViewofAtom (atomCore);
						}
					}
					break;
				case AtomType.EBookGridPaper:
					{
						if (null == atomCore)
						{
							ReadyAtom = new EBookGridPaperofAtom ();
						}
						else
						{
							ReadyAtom = new EBookGridPaperofAtom (atomCore);
						}
					}
					break;
				case AtomType.VerbalSTT:
					{
						if (null == atomCore)
						{
							ReadyAtom = new VerbalSTTofAtom ();
						}
						else
						{
							ReadyAtom = new VerbalSTTofAtom (atomCore);
						}
						break;
					}
				case AtomType.VerbalTrans:
					{
						if (null == atomCore)
						{
							ReadyAtom = new VerbalTransofAtom ();
						}
						else
						{
							ReadyAtom = new VerbalTransofAtom (atomCore);
						}
						break;
					}
				case AtomType.VerbalTTS:
					{
						if (null == atomCore)
						{
							ReadyAtom = new VerbalTTSofAtom ();
						}
						else
						{
							ReadyAtom = new VerbalTTSofAtom (atomCore);
						}
						break;
					}
				case AtomType.VerbalITT:
					{
						if (null == atomCore)
						{
							ReadyAtom = new VerbalITTofAtom ();
						}
						else
						{
							ReadyAtom = new VerbalITTofAtom (atomCore);
						}
						break;
					}
				case AtomType.ChatBot:
					{
						if (null == atomCore)
						{
							ReadyAtom = new SmartChatBotofAtom ();
						}
						else
						{
							ReadyAtom = new SmartChatBotofAtom (atomCore);
						}
					}
					break;
				case AtomType.ActionCharacter:
					{
						if (null == atomCore)
						{
							ReadyAtom = new ActionCharacterofAtom ();
						}
						else
						{
							ReadyAtom = new ActionCharacterofAtom (atomCore);
						}
					}
					break;


				default: break;
			}

			if (null != ReadyAtom)
			{
				AttachAtomAtView (ReadyAtom, AttachAtomEventDefine.AttachAtomEventType.Default, bIsSerialize);

				if (ReadyAtom is GridTableOfAtom)
				{
					if (false == bIsSerialize)
					{
						GridTableOfAtom GTAtom = ReadyAtom as GridTableOfAtom;
						GTAtom.InitializeDefaultTable (3, 3);
					}
				}
			}

			return ReadyAtom;
		}

		public void ReadyBindPopupOrHyperLinkAsRedo (AtomBase SourceAtom)
		{
			if (null != SourceAtom)
			{
				Thickness PopupAtomStartPoint = new Thickness ();
				Size PopupAtomSize = Size.Empty;

				List<AtomBase> lstBindedAtoms = new List<AtomBase> ();

				if (SourceAtom is PopupofAtom)
				{
					lstBindedAtoms = ((PopupofAtom)SourceAtom).GetBindedAtoms ();
				}
				else if (SourceAtom is WebHyperDataofAtom)
				{
					lstBindedAtoms = ((WebHyperDataofAtom)SourceAtom).GetBindedAtoms ();
				}

				int nBindedAtomsCount = lstBindedAtoms.Count;
				bool bIsPossible = IsPossibleBindPopup (lstBindedAtoms);

				if (true == bIsPossible)
				{
					bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstBindedAtoms, ref PopupAtomStartPoint, ref PopupAtomSize);

					if (true == bIsCalculated)
					{
						if (0 < nBindedAtomsCount)
						{
							DMTView CurrentDMTView = GetParentView () as DMTView;
							FrameworkElement PreOwnerElement = SourceAtom.PreOwner;

							//팝업에 묶인 아톰들에게 묶였다는 속성을 준다.
							foreach (AtomBase atom in lstBindedAtoms)
							{
								atom.SetResizeAdornerVisibility (Visibility.Collapsed, true);

								if (SourceAtom is PopupofAtom)
								{
									atom.AtomCore.IsBindedPopup = true;

									atom.InvalidateVisual ();
								}
							}

							if (SourceAtom is WebHyperDataofAtom)
							{
								((WebHyperDataofAtom)SourceAtom).SetBindedAtomsHyperLink ();
							}


							if (PreOwnerElement is TopView)
							{
								AttachAtomAtView (SourceAtom, AttachAtomEventDefine.AttachAtomEventType.Default, false);
							}
							else if (PreOwnerElement is TabPage)
							{
								TabPage CurrentOwner = PreOwnerElement as TabPage;
								CurrentOwner.AddAtom (SourceAtom);
								CurrentDMTView.TopViewAdornerLayer.Add (SourceAtom.ResizeAdorner);
							}

							lstBindedAtoms = null;
						}
					}
				}
			}
		}

		public void UnBindAtomClickEventsRelatedWithView (object objAtom)
		{
			AtomBase TargetAtom = objAtom as AtomBase;

			if (null != TargetAtom)
			{
				TargetAtom.AtomClickedEvent -= HandleAtomClickedEvent;
			}
		}

		public void SelectAllAtoms ()
		{
			AtomBase latestPressedAtom = GetCurrentSelectedAtoms ().FirstOrDefault ();

			SetAllAtomsAdornerVisibilityInLightDMTView (Visibility.Collapsed, true);

			if (null != latestPressedAtom)
			{
				if (true == latestPressedAtom.AtomCore.IsBindedTabView ||
					latestPressedAtom is TabViewAtomBase)
				{
					FrameworkElement TabViewElement = latestPressedAtom is TabViewAtomBase ? latestPressedAtom : latestPressedAtom.GetTabViewAtom ();

					if (null != TabViewElement)
					{
						TabViewAtomBase TabViewAtom = TabViewElement as TabViewAtomBase;
						TabViewAtom.SelectAllAtoms ();
					}
				}
				else if (true == latestPressedAtom.AtomCore.IsBindedScroll ||
					latestPressedAtom is ScrollAtomBase)
				{
					FrameworkElement ScrollElement = latestPressedAtom is ScrollAtomBase ? latestPressedAtom : latestPressedAtom.GetScrollAtom ();

					if (null != ScrollElement)
					{
						ScrollAtomBase ScrollAtom = ScrollElement as ScrollAtomBase;
						ScrollAtom ScrollCore = ScrollAtom.AtomCore as ScrollAtom;
						ScrollCore.SelectAllAtoms ();
					}
				}
				else
				{
					SetAllAtomsAdornerVisibilityInLightDMTView (Visibility.Visible, false);
				}
			}
			else
			{
				SetAllAtomsAdornerVisibilityInLightDMTView (Visibility.Visible, false);
			}

			if (null != OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent)
			{
				ToolBarProperty toolbarProperty = GetCurrentSelectedAtomProperties ();

				if (null == toolbarProperty) //2020-11-24 kys Ctrl + A 로 아톰 선택시에도 정렬버튼 활성화를 위해 논리 보강
				{
					AtomBase pLatestedPressedAtom = null;

					List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtoms ();
					toolbarProperty = GetCurrentFirstSelectedAtomProperties (ref pLatestedPressedAtom);
				}

				if (null != toolbarProperty)
				{
					OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent (toolbarProperty);
				}
			}
		}

		/// <summary>
		/// 아톰 속성에 대한 상단툴바 및 하단 좌표정보 업데이트
		/// </summary>
		public override void SetCurrentSelectedAtomProperties (AtomBase atom)
		{
			if (null != OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent)
			{
				List<AtomBase> listCurrentSelectedAtoms = GetCurrentSelectedAtoms ();
				if (0 == listCurrentSelectedAtoms.Count)
				{
					listCurrentSelectedAtoms.Add (atom);
				}
				AtomBase currentAtom = listCurrentSelectedAtoms[0];

				ToolBarProperty toolbarProperty = GetAtomProperties (currentAtom);
				toolbarProperty.SelectAtomCount = listCurrentSelectedAtoms.Count;

				OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent (toolbarProperty);
				GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyCurrentLocationAndSize (currentAtom.Margin, currentAtom.Width, currentAtom.Height);
			}
		}

		public override void SelectAtomUpdateFlowMap (AtomBase atom)
		{
			if (null != atom)
			{
				//2024-11-16 kys 여러 아톰을 동시에 선택한경우 진행관리자 선택을 초기화 하도록 논리 보강
				var selectAtoms = GetCurrentSelectedAtoms ();

				//아톰선택 시 신버전 진행관리자에 표시
				if (null != this.FlowManagerWindow)
				{
					FlowManagerViewModel viewModel = FlowManagerWindow?.DataContext as FlowManagerViewModel;

					if (1 != selectAtoms.Count)
					{
						viewModel.IsEnableSelectEvent = false;
						viewModel?.SelectFlowAtomIsList (null);
						viewModel.IsEnableSelectEvent = true;
						return;
					}

					viewModel.IsEnableSelectEvent = false;
					viewModel?.SelectFlowAtomIsList (atom.AtomCore);
					viewModel.IsEnableSelectEvent = true;
				}
				//아톰선택시 진행관리자에 표시 
				if (null != this.FlowMapWindow)
				{
					FlowMapWindow.SelectFlowAtomIsList (atom.AtomCore);
				}

				//아톰선택시 아톰편집도우미에 표시 
				if (null != this.AtomEditMapWindow)
				{
					AtomEditMapWindow.SelectFlowAtomIsList (atom.AtomCore);
				}
			}
		}

		public void UpdateFontToolbarForCopyedText (ArrayList arrProperty)
		{
			if (null != OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent)
			{
				if (null != arrProperty)
				{
					OnNotifyToolBarAboutCurrentSelectedAtomPropertiesEvent (arrProperty);
				}
			}
		}

		/// <summary>
		/// 렉트트레커 안에 들어온 아톰들 모두 선택
		/// </summary>
		/// <param name="FirstClickedX"></param>
		/// <param name="FirstClickedY"></param>
		/// <param name="CurrentX"></param>
		/// <param name="CurrentY"></param>
		public void SelectAtomsInRectTracker (double FirstClickedX, double FirstClickedY, double CurrentX, double CurrentY)
		{
			UIElementCollection CurrentChildren = GetChildren ();
			int nCurrentChildrenCount = CurrentChildren.Count;

			for (int nIndex = 0; nIndex < nCurrentChildrenCount; nIndex++)
			{
				AtomBase atom = CurrentChildren[nIndex] as AtomBase;

				bool IsLeftShift = Keyboard.IsKeyDown (Key.LeftShift);

				if (null != atom)
				{
					Thickness currentAtomStart = atom.Margin;
					double dCurrentAtomEndX = atom.Width + currentAtomStart.Left;
					double dCurrentAtomEndY = atom.Height + currentAtomStart.Top;

					if (CurrentX > FirstClickedX && CurrentY > FirstClickedY)
					{
						if (FirstClickedX <= currentAtomStart.Left && FirstClickedY <= currentAtomStart.Top &&
							CurrentX >= dCurrentAtomEndX && CurrentY >= dCurrentAtomEndY)
						{
							if (Visibility.Visible == atom.GetResizeAdornerVisibility () && true == IsLeftShift)
							{
								atom.SetResizeAdornerVisibility (Visibility.Collapsed, false);
							}
							else
							{
								atom.SetResizeAdornerVisibility (Visibility.Visible, false);
								atom.NotifyCurrentLocationAndSize ();
							}

							//2014-11-05-M02 선택되었을때 선택모드 변경 
							//2014-11-07-M01 에디트모드가 있는 아톰 UI 개선 ( EBOOK 관련 )
							//2020-06-30 kys 편집모드 UI논리 변경해서 주석처리함
							//if (atom is EBookTextAtomBase)
							//{
							//	(atom as EBookTextofAtom).StartEdit();
							//}
							//else if (atom is EBookFigureAtomBase)
							//{
							//	(atom as EBookFigureOfAtom).StartEdit();
							//}
						}
					}
					else if (CurrentX < FirstClickedX && CurrentY > FirstClickedY)
					{
						if (CurrentX <= currentAtomStart.Left && FirstClickedY <= currentAtomStart.Top &&
								FirstClickedX >= dCurrentAtomEndX && CurrentY >= dCurrentAtomEndY)
						{
							if (Visibility.Visible == atom.GetResizeAdornerVisibility () && true == IsLeftShift)
							{
								atom.SetResizeAdornerVisibility (Visibility.Collapsed, false);
							}
							else
							{
								atom.SetResizeAdornerVisibility (Visibility.Visible, false);
								atom.NotifyCurrentLocationAndSize ();
							}

							//2014-11-05-M02 선택되었을때 선택모드 변경 
							//2014-11-07-M01 에디트모드가 있는 아톰 UI 개선 ( EBOOK 관련 )
							//2020-06-30 kys 편집모드 UI논리 변경해서 주석처리함
							//if (atom is EBookTextAtomBase)
							//{
							//	(atom as EBookTextofAtom).StartEdit();
							//}
							//else if (atom is EBookFigureAtomBase)
							//{
							//	(atom as EBookFigureOfAtom).StartEdit();
							//}
						}
					}
					else if (CurrentX < FirstClickedX && CurrentY < FirstClickedY)
					{
						if (CurrentX <= currentAtomStart.Left && CurrentY <= currentAtomStart.Top &&
									FirstClickedX >= dCurrentAtomEndX && FirstClickedY >= dCurrentAtomEndY)
						{
							if (Visibility.Visible == atom.GetResizeAdornerVisibility () && true == IsLeftShift)
							{
								atom.SetResizeAdornerVisibility (Visibility.Collapsed, false);
							}
							else
							{
								atom.SetResizeAdornerVisibility (Visibility.Visible, false);
								atom.NotifyCurrentLocationAndSize ();
							}

							//2014-11-05-M02 선택되었을때 선택모드 변경 
							//2014-11-07-M01 에디트모드가 있는 아톰 UI 개선 ( EBOOK 관련 )
							//2020-06-30 kys 편집모드 UI논리 변경해서 주석처리함
							//if (atom is EBookTextAtomBase)
							//{
							//	(atom as EBookTextofAtom).StartEdit();
							//}
							//else if (atom is EBookFigureAtomBase)
							//{
							//	//(atom as EBookFigureOfAtom).StartEdit();
							//}
						}
					}
					else if (CurrentX > FirstClickedX && CurrentY < FirstClickedY)
					{
						if (FirstClickedX <= currentAtomStart.Left && CurrentY <= currentAtomStart.Top &&
										CurrentX >= dCurrentAtomEndX && FirstClickedY >= dCurrentAtomEndY)
						{
							if (Visibility.Visible == atom.GetResizeAdornerVisibility () && true == IsLeftShift)
							{
								atom.SetResizeAdornerVisibility (Visibility.Collapsed, false);
							}
							else
							{
								atom.SetResizeAdornerVisibility (Visibility.Visible, false);
								atom.NotifyCurrentLocationAndSize ();
							}

							//2014-11-05-M02 선택되었을때 선택모드 변경 
							//2014-11-07-M01 에디트모드가 있는 아톰 UI 개선 ( EBOOK 관련 )
							//2020-06-30 kys 편집모드 UI논리 변경해서 주석처리함
							//if (atom is EBookTextAtomBase)
							//{
							//	(atom as EBookTextofAtom).StartEdit();
							//}
							//else if (atom is EBookFigureAtomBase)
							//{
							//	(atom as EBookFigureOfAtom).StartEdit();
							//}
						}
					}
				}
			}
		}

		public bool DeleteAtom (AtomBase SourceDeleteAtom)
		{
			return DeleteAtom (SourceDeleteAtom, false);
		}

		public bool DeleteAtom (AtomBase sourceDeleteAtom, bool isAtomTypeChange)
		{
			var dmtView = GetParentView () as DMTView;
			var frame = dmtView?.GetFrame () as DMTFrame;

			if (null == dmtView || null == frame)
				return false;

			bool isDeleteSuccess = true;
			UIElementCollection currentChildren = GetChildren ();

			foreach (var item in currentChildren)
			{
				if (item is AtomBase atom)
				{
					string ItemProperVar = atom.AtomCore.AtomProperVar;
					string SourceProperVar = sourceDeleteAtom.AtomCore.AtomProperVar;
					if (ItemProperVar == SourceProperVar)
					{
						sourceDeleteAtom = atom;
						break;
					}
				}
			}

			if (null != sourceDeleteAtom.GetScrollAtom ())
			{
				FrameworkElement ScrollElement = sourceDeleteAtom.GetScrollAtom ();

				if (null != ScrollElement)
				{
					ScrollAtomBase scrollAtom = ScrollElement as ScrollAtomBase;
					if (null != scrollAtom)
					{
						ScrollAtom scrollCore = scrollAtom.AtomCore as ScrollAtom;
						if (null != scrollCore)
						{
							sourceDeleteAtom.PreOwner = sourceDeleteAtom.GetOwnerView ();

							List<AtomBase> bindAtomList = scrollCore.GetBindedAtoms ();

							if (false == isAtomTypeChange && 1 == bindAtomList.Count)
							{
								string strMsg = LC.GS ("TopProcess_DMTDoc_54"); //해당 아톰을 삭제할 경우 스크롤아톰도 같이 삭제됩니다.\n삭제하시겠습니까?
								MessageBoxResult result = _Message80.Show (strMsg, "SmartMaker", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
								if (MessageBoxResult.Cancel == result)
								{
									return false;
								}
							}

							isDeleteSuccess = scrollCore.DeleteAtom (sourceDeleteAtom);

							if (sourceDeleteAtom is PopupofAtom BindPopupAtom)
							{
								var BindedAtomList = scrollAtom.GetBindedAtoms ();
								BindedAtomList.AddRange (BindPopupAtom.GetBindedAtoms ());
								BindPopupAtom.ReleaseBindedAtoms ();
								BindAtomToScroll (scrollAtom, BindedAtomList, true);
							}

							if (false == isAtomTypeChange && true == isDeleteSuccess && 0 == scrollCore.GetBindedAtoms ().Count)
							{
								return DeleteAtom (scrollAtom);
							}
						}
					}
				}
			}
			else if (null != sourceDeleteAtom.GetTabViewAtom ())
			{
				FrameworkElement TabViewElement = sourceDeleteAtom.GetTabViewAtom ();

				if (null != TabViewElement)
				{
					TabViewAtomBase TabViewAtom = TabViewElement as TabViewAtomBase;
					isDeleteSuccess = TabViewAtom.DeleteAtomAboutAllTabPage (sourceDeleteAtom);
				}
			}
			else
			{
				if (null != sourceDeleteAtom.AtomCore.BindGroupAtom)
				{
					Atom groupAtomCore = sourceDeleteAtom.AtomCore.BindGroupAtom;
					AtomBase groupAtom = groupAtomCore.GetOfAtom ();

					List<Atom> bindAtomCoreList = new List<Atom> ();
					if (groupAtomCore is EBookAnimationGroupAtom)
					{
						EBookAnimationGroupAtom animationGroup = groupAtomCore as EBookAnimationGroupAtom;
						bindAtomCoreList = animationGroup.GroupAtomList;

						if (1 == bindAtomCoreList.Count)
						{
							string strMsg = LC.GS ("TopProcess_DMTDoc_55"); //해당 아톰을 삭제할 경우 그룹묶기아톰도 같이 삭제됩니다.\n삭제하시겠습니까?
							MessageBoxResult result = _Message80.Show (strMsg, "SmartMaker", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
							if (MessageBoxResult.Cancel == result)
							{
								return false;
							}
						}

						animationGroup.GroupAtomList.Remove (sourceDeleteAtom.AtomCore);

						if (0 == animationGroup.GroupAtomList.Count)
						{
							isDeleteSuccess = DeleteAtom (animationGroup.GetOfAtom ());
						}
					}
					else if (groupAtomCore is WebQuickLinkAtom)
					{
						WebQuickLinkAtom quickLink = groupAtomCore as WebQuickLinkAtom;
						bindAtomCoreList = quickLink.GroupAtomList;

						if (1 == bindAtomCoreList.Count)
						{
							string strMsg = LC.GS ("TopProcess_DMTDoc_56"); //해당 아톰을 삭제할 경우 퀵링크아톰도 같이 삭제됩니다.\n삭제하시겠습니까?
							MessageBoxResult result = _Message80.Show (strMsg, "SmartMaker", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
							if (MessageBoxResult.Cancel == result)
							{
								return false;
							}
						}

						quickLink.GroupAtomList.Remove (sourceDeleteAtom.AtomCore);

						if (0 == quickLink.GroupAtomList.Count)
						{
							isDeleteSuccess = DeleteAtom (quickLink.GetOfAtom ());
						}
					}

					List<AtomBase> bindAtomList = bindAtomCoreList.Select (item => item.GetOfAtom ()).ToList ();
					dmtView.EditEventManager.SetGroupAtomReSize (groupAtom, bindAtomList);
				}
				else if (null != sourceDeleteAtom.AtomCore.AutoCompleteAtom) // 자동입력완성 삭제
				{
					Atom autoCompleteAtom = sourceDeleteAtom.AtomCore.AutoCompleteAtom;
					DeleteAtom (autoCompleteAtom.GetOfAtom ());
				}
				else if (true == sourceDeleteAtom.AtomCore.IsBindedPopup)
				{
					var PopupAtom = sourceDeleteAtom.GetPopupAtom ();
					var BindedAtomList = PopupAtom.GetBindedAtoms ();
					if (1 == BindedAtomList.Count)
					{
						string strMsg = "해당 아톰을 삭제할 경우 팝업 아톰도 같이 삭제됩니다.\n삭제하시겠습니까?";
						MessageBoxResult result = _Message80.Show (strMsg, "SmartMaker", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
						if (MessageBoxResult.Cancel == result)
						{
							return false;
						}
					}

					PopupAtom.ReleaseBindedAtom (sourceDeleteAtom);
					if (0 == PopupAtom.GetBindedAtoms ().Count)
					{
						DeleteAtom (PopupAtom);
					}
				}

				sourceDeleteAtom.CloseAtom ();

				bool bIsDisposeSuccess = sourceDeleteAtom.DisposeAll ();

				if (false == bIsDisposeSuccess)
				{
					isDeleteSuccess = bIsDisposeSuccess;
					return isDeleteSuccess;
				}

				sourceDeleteAtom.PreOwner = sourceDeleteAtom.GetOwnerView ();
				AdjustAtomZindexAboutDelete (sourceDeleteAtom);
				AdjustAtomRelativeTabIndexForDeleteAction (sourceDeleteAtom);
				currentChildren.Remove (sourceDeleteAtom);
			}

			if (sourceDeleteAtom == dmtView.LatestedPressedAtom)
			{
				dmtView.LatestedPressedAtom = null;
			}


			AdjustAtomAbsoluteTabIndex ();
			InvalidateLightDMTView ();

			AutoRefreshEditWindow ();

			if (true == isDeleteSuccess)
			{
				RefreshAnimationWindow ();

				if (!frame.IsFocused)
				{
					frame.Focus ();
				}
			}

			return isDeleteSuccess;
		}

		public bool DeleteAtomInGridTable (AtomBase SourceDeleteAtom)
		{
			bool bIsDeleteSuccess = true;
			UIElementCollection CurrentChildren = GetChildren ();

			bool bIsDisposeSuccess = SourceDeleteAtom.DisposeAll ();

			if (false == bIsDisposeSuccess)
			{
				bIsDeleteSuccess = bIsDisposeSuccess;
				return bIsDeleteSuccess;
			}

			AdjustAtomAbsoluteTabIndex ();
			InvalidateLightDMTView ();
			return bIsDeleteSuccess;
		}

		public void ChangeDebugDBInfoVisible ()
		{
			m_bIsDebugDBInfoVisible = !m_bIsDebugDBInfoVisible;
			ChangeAllAtomDBInfoVisible (m_bIsDebugDBInfoVisible);

			InvalidateLightDMTView ();
		}

		public void ChangeZIndexTextVisible ()
		{
			m_bIsZIndexTextVisible = !m_bIsZIndexTextVisible;
			ChangeAllAtomZIndexTextVisible (m_bIsZIndexTextVisible);

			InvalidateLightDMTView ();
		}

		public void ChangeRelativeTabIndexTextVisible ()
		{
			m_bIsRelativeTabIndexTextVisible = !m_bIsRelativeTabIndexTextVisible;
			ChangeAllAtomRelativeTabIndexTextVisible (m_bIsRelativeTabIndexTextVisible);

			InvalidateLightDMTView ();
		}

		public void ChangeAbsoluteTabIndexTextVisible ()
		{
			m_bIsAbsoluteTabIndexTextVisible = !m_bIsAbsoluteTabIndexTextVisible;
			ChangeAllAtomAbsoluteTabIndexTextVisible (m_bIsAbsoluteTabIndexTextVisible);

			InvalidateLightDMTView ();
		}

		public void ChangeAtomNameTextVisible ()
		{
			if (true == m_IsAtomFieldTextVisible)
			{
				ChangeAtomFieldTextVisible ();
			}

			m_bIsAtomNameTextVisible = !m_bIsAtomNameTextVisible;
			ChangeAllAtomNameTextVisible (m_bIsAtomNameTextVisible);

			InvalidateLightDMTView ();
		}

		public void ChangeAtomFieldTextVisible ()
		{
			if (true == m_bIsAtomNameTextVisible)
			{
				ChangeAtomNameTextVisible ();
			}

			m_IsAtomFieldTextVisible = !m_IsAtomFieldTextVisible;
			ChangeAllAtomFieldTextVisible (m_IsAtomFieldTextVisible);

			InvalidateLightDMTView ();
		}

		public void ReadyScrollAtom (ScrollAtomBase ScrollAtom)
		{
			Thickness ScrollAtomStartPoint = new Thickness ();
			Size ScrollAtomSize = Size.Empty;
			bool bIsSelectedAtomsInTabView = false;
			List<AtomBase> lstCurrentSelectedAtoms = null;
			int nCurrentSelectedAtomsInTabViewCount = 0;
			int nCurrentSelectedAtomsInScrollCount = 0;

			lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInLightDMTView ();
			int nCurrentSelectedAtomsInLightDMTViewCount = lstCurrentSelectedAtoms.Count;

			if (0 == nCurrentSelectedAtomsInLightDMTViewCount)
			{
				lstCurrentSelectedAtoms.Clear ();
				lstCurrentSelectedAtoms = null;
				lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInTabView ();
				nCurrentSelectedAtomsInTabViewCount = lstCurrentSelectedAtoms.Count;
				bIsSelectedAtomsInTabView = true;

				if (0 == nCurrentSelectedAtomsInTabViewCount)
				{
					lstCurrentSelectedAtoms.Clear ();
					lstCurrentSelectedAtoms = null;
					lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInScroll ();
					nCurrentSelectedAtomsInScrollCount = lstCurrentSelectedAtoms.Count;
				}
			}

			if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode && 0 == lstCurrentSelectedAtoms.Count)
			{
				bIsSelectedAtomsInTabView = false;

				lstCurrentSelectedAtoms = WebDynamicGrid.RootDesignGrid.GetSelectAtomList ();
			}

			lstCurrentSelectedAtoms.Sort (new ViewIndexAtomComparer ());

			bool bIsPossibleBindScroll = IsPossibleBindScroll (lstCurrentSelectedAtoms);

			if (true == bIsPossibleBindScroll)
			{
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstCurrentSelectedAtoms, ref ScrollAtomStartPoint, ref ScrollAtomSize);

				if (true == bIsCalculated)
				{
					DMTView CurrentDMTView = GetParentView () as DMTView;
					ScrollAtom.OnNotifyReleaseBindedAtomsEvent += ScrollAtom_OnNotifyReleaseBindedAtomsEvent;

					ScrollAtom.Margin = new Thickness (
						Math.Max (ScrollAtomStartPoint.Left - ScrollAtom.DefaultSizeGap, 0)
						, Math.Max (ScrollAtomStartPoint.Top - ScrollAtom.DefaultSizeGap, 0)
						, 0, 0
						);
					ScrollAtom.InitialWidth = ScrollAtomSize.Width + ScrollAtom.DefaultSizeGap * 2;
					ScrollAtom.InitialHeight = ScrollAtomSize.Height + ScrollAtom.DefaultSizeGap * 2;
					ScrollAtom.Width = ScrollAtomSize.Width + ScrollAtom.DefaultSizeGap * 2;
					ScrollAtom.Height = ScrollAtomSize.Height + ScrollAtom.DefaultSizeGap * 2;


					if (true == bIsSelectedAtomsInTabView)
					{
						if (0 < nCurrentSelectedAtomsInTabViewCount)
						{
							AtomBase FirstAtom = lstCurrentSelectedAtoms[0] as AtomBase;
							FrameworkElement TabViewElement = FirstAtom.GetTabViewAtom ();

							if (null != TabViewElement)
							{
								TabViewAtomBase TabViewAtom = TabViewElement as TabViewAtomBase;

								AttachAtomAtTabView (ScrollAtom, TabViewAtom, false, -1);
								DeleteParentOfAtoms (lstCurrentSelectedAtoms);

								Adorner[] ScrollAtomAdorner = CurrentDMTView.TopViewAdornerLayer.GetAdorners (ScrollAtom);

								if (null == ScrollAtomAdorner)
								{
									CurrentDMTView.TopViewAdornerLayer.Add (ScrollAtom.ResizeAdorner);
								}

								// 팝업아톰 정보가 null 처리 되어 복원
								foreach (AtomBase atomBase in lstCurrentSelectedAtoms)
								{
									if (atomBase is PopupAtomBase)
									{
										PopupAtom popupAtom = atomBase.AtomCore as PopupAtom;
										popupAtom.ResetBindedAtoms ();
									}
								}
								//
							}
						}
					}
					else
					{
						AttachAtomAtView (ScrollAtom, AttachAtomEventDefine.AttachAtomEventType.Default, false);
						DeleteParentOfAtoms (lstCurrentSelectedAtoms);
					}

					ScrollAtom.BindAtoms (lstCurrentSelectedAtoms);
					AdjustAllAtomZIndexInLightDMTView ();
					AdjustAllAtomRelativeTabIndexInLightDMTView ();

					lstCurrentSelectedAtoms = null;

				}
			}
		}

		private bool ReadyEBookQuizViewAtom (EBookQuizViewAtomBase atomBase)
		{
			bool isSelectedAtom = IsExistCurrentSelectedAtoms ();
			var selectedAtoms = GetCurrentSelectedAtomsInLightDMTView ();

			if (0 < selectedAtoms.Count && selectedAtoms.Count <= 3)
			{
				atomBase.IsAutoMakeAtom = true;
			}
			else if (0 == selectedAtoms.Count)
			{
				atomBase.IsAutoMakeAtom = true;
			}

			if (false == isSelectedAtom)
				return false;

			Thickness startPoint = new Thickness ();
			Size atomSize = Size.Empty;
			double gap = 5;

			var isCalculated = CalculateAtomsRegionStartPointAndSize (selectedAtoms, ref startPoint, ref atomSize);

			if (true == isCalculated)
			{
				atomBase.Margin = new Thickness (
					startPoint.Left - gap,
					startPoint.Top - gap,
					0, 0
					);

				atomBase.Width = atomSize.Width + gap * 2;
				atomBase.Height = atomSize.Height + gap * 2;
			}

			AttachAtomAtView (atomBase, AttachAtomEventDefine.AttachAtomEventType.Default, false);
			DeleteParentOfAtoms (selectedAtoms);

			foreach (var subAtom in selectedAtoms)
			{
				double subX = subAtom.Margin.Left - startPoint.Left;
				double subY = subAtom.Margin.Top - startPoint.Top;

				subAtom.Margin = new Thickness (subX + gap, subY + gap, 0, 0);
				atomBase.AddAtomAtCurrentTabPage (subAtom);
			}

			return true;
		}

		public void ReadyScrollAtomAsRedo (ScrollAtomBase sourceScrollAtom)
		{
			if (null != sourceScrollAtom && sourceScrollAtom.AtomCore is ExtensionScrollAtom extensionScrollAtom)
			{
				Thickness ScrollAtomStartPoint = new Thickness ();
				Size ScrollAtomSize = Size.Empty;

				List<AtomBase> lstCurrentBindedAtoms = extensionScrollAtom.GetBindedAtoms ();

				if (0 == lstCurrentBindedAtoms.Count)
				{
					foreach (var atomName in extensionScrollAtom.UndoBindAtomList)
					{
						var findAtom = GetAtomFromName (atomName) as Atom;
						if (null != findAtom)
						{
							lstCurrentBindedAtoms.Add (findAtom.AtomBase);
						}
					}
				}

				bool bIsPossibleBindScroll = IsPossibleBindScroll (lstCurrentBindedAtoms);

				if (true == bIsPossibleBindScroll)
				{
					bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstCurrentBindedAtoms, ref ScrollAtomStartPoint, ref ScrollAtomSize);

					if (true == bIsCalculated)
					{
						DMTView CurrentDMTView = GetParentView () as DMTView;
						FrameworkElement PreOwnerElement = sourceScrollAtom.PreOwner;

						if (PreOwnerElement is TopView)
						{
							AttachAtomAtView (sourceScrollAtom, AttachAtomEventDefine.AttachAtomEventType.Default, false);
							DeleteParentOfAtoms (lstCurrentBindedAtoms);
						}
						else if (PreOwnerElement is TabPage)
						{
							TabPage CurrentOwnerTabPage = PreOwnerElement as TabPage;
							CurrentOwnerTabPage.AddAtom (sourceScrollAtom);
							DeleteParentOfAtoms (lstCurrentBindedAtoms);
							CurrentDMTView.TopViewAdornerLayer.Add (sourceScrollAtom.ResizeAdorner);
						}

						sourceScrollAtom.BindAtoms (lstCurrentBindedAtoms);
						AdjustAllAtomZIndexInLightDMTView ();
						AdjustAllAtomRelativeTabIndexInLightDMTView ();
						lstCurrentBindedAtoms = null;
					}
				}
			}
		}

		public void AdjustAtomZIndex (AtomBase SourceAtom, ChangeAtomZIndexDefine.ActionType ApplyType)
		{
			if (ChangeAtomZIndexDefine.ActionType.한단계앞으로 == ApplyType)
			{
				AdjustAtomZIndexFrontAction (SourceAtom);
			}
			else if (ChangeAtomZIndexDefine.ActionType.한단계뒤로 == ApplyType)
			{
				AdjustAtomZIndexBackAction (SourceAtom);
			}
			else if (ChangeAtomZIndexDefine.ActionType.맨앞으로가져오기 == ApplyType)
			{
				AdjustAtomZIndexFirstAction (SourceAtom);
			}
			else if (ChangeAtomZIndexDefine.ActionType.맨뒤로보내기 == ApplyType)
			{
				AdjustAtomZIndexLastAction (SourceAtom);
			}


			//2014-11-21-M01 Ebook editMode 있는 아톰 Index처리 
			if (SourceAtom is EBookTextAtomBase)
			{
				//(SourceAtom as EBookTextofAtom).ZIndexBackup = Canvas.GetZIndex(SourceAtom);
			}
			else if (SourceAtom is EBookFigureAtomBase)
			{
				(SourceAtom as EBookFigureOfAtom).ZIndexBackup = Canvas.GetZIndex (SourceAtom);
			}
			else if (SourceAtom is EBookImageAtomBase)
			{
				(SourceAtom as EBookImageofAtom).ZIndexBackup = Canvas.GetZIndex (SourceAtom);
			}
			else if (SourceAtom is EBookAvatarAtomBase)
			{
				(SourceAtom as EBookAvatarofAtom).ZIndexBackup = Canvas.GetZIndex (SourceAtom);
			}
		}

		/// <summary>
		/// 아톰이 삭제되었을 경우에 사용하는 탭오더 변경 로직
		/// </summary>
		/// <param name="sourceAtom"></param>
		public void AdjustAtomRelativeTabIndexForDeleteAction (AtomBase sourceAtom)
		{
			if (null != sourceAtom)
			{
				UIElementCollection CurrentChildren = GetChildren ();
				int nChildrenCount = CurrentChildren.Count;

				List<AtomBase> atombaseList = GetChildren ().Cast<AtomBase> ().ToList ();

				atombaseList.Sort (delegate (AtomBase x, AtomBase y)
				{
					return x.AtomCore.Attrib.AtomRelativeTabIndex.CompareTo (y.AtomCore.Attrib.AtomRelativeTabIndex);
				});

				int nSourceAtomRelativeTabIndex = sourceAtom.TabIndex;
				int nIndex = 1;

				if (true == atombaseList.Contains (sourceAtom))
				{
					atombaseList.Remove (sourceAtom);
				}

				foreach (AtomBase atom in atombaseList)
				{
					atom.AtomCore.Attrib.AtomRelativeTabIndex = nIndex;
					nIndex++;
				}

			}
		}

		public override void AdjustAtomAbsoluteTabIndex ()
		{
			List<Atom> atomCoreList = GetViewAtomCores ();
			atomCoreList.Sort (new AbsoluteTabIndexAtomCoreComparer ());

			for (int i = 0; i < atomCoreList.Count; i++)
			{
				Atom atomCore = atomCoreList[i];

				if (atomCore is ScrollAtom)
				{
					ScrollAtom scrollAtomCore = atomCore as ScrollAtom;

					if (false == scrollAtomCore.IsTabViewBinded)
					{
						List<Atom> scrollAtomList = scrollAtomCore.GetBindedAtomCores ();
						scrollAtomList.Sort (new AbsoluteTabIndexAtomCoreComparer ());

						atomCoreList.InsertRange (i, scrollAtomList);
						i = i + scrollAtomList.Count;
					}
				}
				else if (atomCore is TabViewAtom)
				{
					TabViewAtomBase tabViewAtom = atomCore.GetOfAtom () as TabViewAtomBase;
					if (null != tabViewAtom)
					{
						List<AtomBase> tabViewAtomList = tabViewAtom.GetAllChildren ();

						if (null != tabViewAtomList)
						{
							List<Atom> tabViewAtomCoreList = new List<Atom> ();

							for (int j = 0; j < tabViewAtomList.Count; j++)
							{
								AtomBase atomBase = tabViewAtomList[j] as AtomBase;
								if (null != atomBase && null != atomBase.AtomCore)
								{
									tabViewAtomCoreList.Add (atomBase.AtomCore);
								}
							}

							tabViewAtomCoreList.Sort (new AbsoluteTabIndexAtomCoreComparer ());
							atomCoreList.InsertRange (i + 1, tabViewAtomCoreList); //탭뷰의 경우 AbsoluteTabIndex가 자식아톰들보다 먼저 있기 때문에 +1 처리
						}
						//i = i + tabViewAtomCoreList.Count; 탭뷰 내부에 스크롤이 있을수도 있기 때문에 Skip하지 않는다.
					}
				}
				//팝업의 경우 GetViewAtomCores에 팝업 내부 아톰도 포함되기 때문에 별도 처리해주지 않는다.
			}

			int nTabIndex = 1;
			int num = 0;
			AdjustAbsoluteTabIndex (atomCoreList, ref nTabIndex, ref num, null);
		}


		private void AdjustAbsoluteTabIndex (List<Atom> atomCoreList, ref int nTabIndex, ref int nAtomIndex, Atom parentAtom)
		{
			for (int i = nAtomIndex; i < atomCoreList.Count; i++)
			{
				Atom atomCore = atomCoreList[i];

				if (parentAtom is ScrollAtom && false == atomCore.IsScrollBinded)
				{
					nAtomIndex = i - 1;
					break;
				}

				//스크롤이나 팝업에 묶인 경우 내부 아톰의 TabIndex를 우선 설정하고 이후에 TabIndex를 설정한다.
				if (atomCore is ScrollAtom)
				{
					int nTemp = i + 1;
					AdjustAbsoluteTabIndex (atomCoreList, ref nTabIndex, ref nTemp, atomCore);
					atomCore.Attrib.AtomAbsoluteTabIndex = nTabIndex;
					nTabIndex++;
					i = nTemp;
				}
				else
				{
					atomCore.Attrib.AtomAbsoluteTabIndex = nTabIndex;
					nTabIndex++;
				}
			}
		}

		public void AdjustAllAtomZIndexInLightDMTView ()
		{
			UIElementCollection CurrentChildren = GetChildren ();

			int nChildrenCount = CurrentChildren.Count;

			for (int nIndex = 0; nIndex < nChildrenCount; nIndex++)
			{
				AtomBase CurrentAtom = CurrentChildren[nIndex] as AtomBase;

				if (null != CurrentAtom)
				{
					Canvas.SetZIndex (CurrentAtom, nIndex + 1);
				}
			}
		}

		public void AdjustAllAtomRelativeTabIndexInLightDMTView ()
		{
			UIElementCollection CurrentChildren = GetChildren ();
			int nChildrenCount = CurrentChildren.Count;

			for (int nIndex = 0; nIndex < nChildrenCount; nIndex++)
			{
				AtomBase CurrentAtom = CurrentChildren[nIndex] as AtomBase;

				if (null != CurrentAtom)
				{
					CurrentAtom.TabIndex = (nIndex + 1);
				}
			}
		}

		/// <summary>
		/// 마지막에 포커스를 가졌던 아톰이 있으면 이 아톰을 기준으로 탭한다.
		/// </summary>
		/// <param name="CurrentFormMode"></param>
		/// <param name="IsReverse"></param>
		/// <returns></returns>
		public AtomBase OnEditModeMoveFocusOnRelativeTabIndex (FormMode.FrameMode CurrentFormMode, bool IsReverse)
		{
			var dmtView = GetParentView () as DMTView;
			AtomBase lastPressedAtom = GetLatestPressedAtom ();

			if (null != lastPressedAtom)
			{
				FrameworkElement OwnerElement = lastPressedAtom.GetOwnerView ();

				if (null != OwnerElement)
				{
					if (OwnerElement is TopView)
					{
						return OnEditModeMoveFocusOnNextRelativeTabIndexAtom (lastPressedAtom, CurrentFormMode, IsReverse);
					}
					else if (OwnerElement is TabViewAtomBase)
					{
						TabViewAtomBase TabViewAtom = OwnerElement as TabViewAtomBase;
						return TabViewAtom.OnEditModeMoveFocusOnNextRelativeTabIndexAtom (lastPressedAtom, CurrentFormMode, IsReverse);
					}
					else if (OwnerElement is ScrollAtomBase)
					{
						ScrollAtomBase ScrollAtom = OwnerElement as ScrollAtomBase;
						return ScrollAtom.OnEditModeMoveFocusOnNextRelativeTabIndexAtom (lastPressedAtom, CurrentFormMode, IsReverse);
					}
				}
			}
			else
			{
				//2024-11-14 kys 아무것도 선택되어 있지 않은경우 첫번째 아톰으로 초점 이동하도록 논리 보강
				var viewAtomList = dmtView.GetViewAtomCores ().Select (i => i.AtomBase).ToList ();
				if (0 < viewAtomList.Count)
				{
					viewAtomList.Sort (new RelativeTabIndexAtomComparer ());
					lastPressedAtom = viewAtomList[0];
					lastPressedAtom.SetResizeAdornerVisibility (Visibility.Visible, false);
				}
			}

			return lastPressedAtom;
		}

		public override void OnRunModeMoveFocusOnRelativeTabIndex ()
		{
			AtomBase currentTabFocussedAtom = GetRunModeCurrentTabFocussedAtom ();

			if (null != currentTabFocussedAtom)
			{
				FrameworkElement ownerElement = currentTabFocussedAtom.GetOwnerView ();

				if (null != ownerElement)
				{
					if (ownerElement is TopView)
					{
						OnRunModeMoveFocusOnNextRelativeTabIndexAtom (currentTabFocussedAtom);
					}
					else if (ownerElement is TabViewAtomBase)
					{
						var tabviewAtomBase = ownerElement as TabViewAtomBase;
						bool bTabMove = tabviewAtomBase.OnRunModeMoveFocusOnNextRelativeTabIndexAtom (currentTabFocussedAtom);

						if (false == bTabMove) //탭뷰 내부에서 View쪽으로 빠져나올때 필요
						{
							var viewList = GetViewAtomCores ();
							int index = viewList.IndexOf (tabviewAtomBase.AtomCore);

							index = index < viewList.Count ? index + 1 : 0;
							var atomCore = viewList[index];

							atomCore.Enable (true, true, 1);
						}
					}
					else if (ownerElement is ScrollAtomBase)
					{
						ScrollAtomBase scrollAtomBase = ownerElement as ScrollAtomBase;
						bool bTabMode = scrollAtomBase.OnRunModeMoveFocusOnNextRelativeTabIndexAtom (currentTabFocussedAtom);

						if (false == bTabMode)
						{
							OnRunModeMoveFocusOnNextRelativeTabIndexAtom (scrollAtomBase);
						}

						//((ScrollAtomBase)ownerElement).OnRunModeMoveFocusOnNextRelativeTabIndexAtom (currentTabFocussedAtom);
					}
					else if (ownerElement is TableViewBase)
					{
						// GridTable 내부에서 이동시 필요
						TableViewBase tableView = ownerElement as TableViewBase;
						GridTableOfAtom gridTableOfAtom = tableView.AtomBase as GridTableOfAtom;
						if (null != gridTableOfAtom)
						{
							bool bTabMove = gridTableOfAtom.OnRunModeMoveFocusOnNextRelativeTabIndexAtom (currentTabFocussedAtom);
							if (false == bTabMove)  // Grid 내부에서 View 쪽으로 빠져나올때 필요
							{
								OnRunModeMoveFocusOnNextRelativeTabIndexAtom (gridTableOfAtom);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 키보드로 움직이는 아톰에 대한 Moving을 조정한다.
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="Modifier"></param>
		/// <returns></returns>
		public bool HandleAtomMovingForKeyboard (Key direction, ModifierKeys Modifier)
		{
			var selectedAtoms = GetCurrentSelectedAtoms ();
			var view = GetParentView () as DMTView;

			// 2020-06-11 kys 그룸아톰 자체를 키보드로 이동했을때 내부 아톰들도 같이 이동시키기 위해서

			var isAnimationGroupAtom = false;
			var animationGroupAtomList = new List<EBookAnimationGroupofAtom> (selectedAtoms.OfType<EBookAnimationGroupofAtom> ());

			if (0 < animationGroupAtomList?.Count ())
			{
				isAnimationGroupAtom = true;
				foreach (var atom in animationGroupAtomList)
				{
					var groupAtoms = atom.GetGroupAtoms ();
					if (0 < groupAtoms.Count)
						selectedAtoms.AddRange (groupAtoms);
				}
			}

			#region | 문제풀이아톰 내부 항목 |

			if (0 == selectedAtoms.Count)
			{
				var questionsList = WPFFindChildHelper.FindVisualChildList<EBookQuestionsofAtom> (view);

				if (null != questionsList)
				{
					foreach (var questions in questionsList)
					{
						var subList = WPFFindChildHelper.FindVisualChildList<AtomBase> (questions).Where (item => Visibility.Visible == item.GetResizeAdornerVisibility ());

						if (null != subList && 0 < subList.Count ())
						{
							selectedAtoms.AddRange (subList);
						}
					}
				}
			}

			#endregion

			if (0 == selectedAtoms.Count)
				return false;

			var offsetX = 0;
			var offsetY = 0;
			var offsetW = 0;
			var offsetH = 0;

			var expandAtomCommands = new List<ExpandAtomCommand> ();
			var type = CalculateAtomMarginForKeyboard (direction, Modifier, out offsetX, out offsetY, out offsetW, out offsetH);

			var isExecuteEvent = true;

			foreach (var atom in selectedAtoms)
			{
				var atomCore = atom.AtomCore;
				var attrib = atomCore.Attrib;

				var oldX = attrib.AtomX;
				var oldY = attrib.AtomY;
				var oldW = attrib.AtomWidth;
				var oldH = attrib.AtomHeight;

				var newX = oldX + offsetX;
				var newY = oldY + offsetY;
				var newW = oldW + offsetW;
				var newH = oldH + offsetH;

				newX = Math.Max (0, newX);
				newY = Math.Max (0, newY);

				var oldSize = new Size (oldW, oldH);
				var newSize = new Size (newW, newH);

				if ((0 == oldX && 0 == newX && 0 != offsetX) ||
					(0 == oldY && 0 == newY && 0 != offsetY))
				{
					isExecuteEvent = false;
					break;
				}

				var moveCommand = new ExpandAtomCommand (atom, type, newX, newY, oldX, oldY, oldSize, newSize);
				expandAtomCommands.Add (moveCommand);
			}

			if(isExecuteEvent)
			{
				var command = new ExpandAtomsCommand (expandAtomCommands);

				//1px 단위로 이동할때만 Merge처리함
				if (type == DIR_TYPE.DIR_LEFT || type == DIR_TYPE.DIR_RIGHT ||
					type == DIR_TYPE.DIR_UP || type == DIR_TYPE.DIR_DOWN)
				{
					command.IsMergeCommand = false;
				}
				else
				{
					command.IsMergeCommand = true;
				}

				Commander.AddCommand (command);
				Commander.ExecuteCommand ();
			}

			if (isAnimationGroupAtom)
				view.ResizeEBookAnimationGroup ();

			selectedAtoms[0].NotifyCurrentLocationAndSize ();

			return true;
		}

		/// <summary>
		/// 원래 LightDMTView에 있던 아톰이 탭뷰에 바인딩 될 때 탭뷰를 기준으로 마진을 재조정한다.
		/// </summary>
		/// <param name="sourceAtom"></param>
		/// <param name="targetTabViewAtom"></param>
		public void AdjustSourceAtomRegionInTabView (AtomBase sourceAtom, TabViewAtomBase targetTabViewAtom)
		{
			Point ptStartOffset = targetTabViewAtom.GetAdjustSourceAtomRegionInTabView (sourceAtom);

			sourceAtom.Margin = new Thickness (ptStartOffset.X, ptStartOffset.Y, 0, 0);
		}

		public AtomBase GiveAtomInputFocus ()
		{
			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtoms ();
			int nCountOfSelectedAtoms = lstCurrentSelectedAtoms.Count;

			if (1 == nCountOfSelectedAtoms)
			{
				AtomBase TargetAtom = lstCurrentSelectedAtoms[0];
				TargetAtom.TryInputFocus ();

				return TargetAtom;
			}

			return null;
		}

		/// <summary>
		/// 현재 선택된 아톰의 프로퍼티들을 가져와서 상단 툴바와 동기화
		/// </summary>
		/// <returns></returns>
		public ToolBarProperty GetCurrentSelectedAtomProperties ()
		{
			ToolBarProperty toolbarProperty = null;

			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtoms ();
			int nCurrentSelectedAtomsCount = lstCurrentSelectedAtoms.Count;

			//아톰이 하나 선택된 경우
			if (1 == nCurrentSelectedAtomsCount)
			{
				AtomBase atom = lstCurrentSelectedAtoms[0] as AtomBase;
				toolbarProperty = GetAtomProperties (atom);
			}

			return toolbarProperty;
		}

		/// <summary>
		///  현재 선택된 아톰 첫번째의 프로퍼티들을 가져와서 상단 툴바와 동기화
		/// </summary>
		/// <param name="LatestedPressedAtom"></param>
		/// <returns></returns>
		public ToolBarProperty GetCurrentFirstSelectedAtomProperties (ref AtomBase LatestedPressedAtom)
		{
			ToolBarProperty toolbarProperty = null;
			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtoms ();

			int nCurrentSelectedAtomsCount = lstCurrentSelectedAtoms.Count;

			if (0 < nCurrentSelectedAtomsCount)
			{
				AtomBase atom = lstCurrentSelectedAtoms[0] as AtomBase;
				toolbarProperty = GetAtomProperties (atom);
				toolbarProperty.SelectAtomCount = nCurrentSelectedAtomsCount;
				LatestedPressedAtom = atom;

				atom.NotifyCurrentLocationAndSize ();
			}

			return toolbarProperty;
		}

		/// <summary>
		/// LightDMTView와 TabView와 Scroll을 통틀어 현재 선택된 아톰들을 리턴
		/// </summary>
		/// <returns></returns>
		public List<AtomBase> GetCurrentSelectedAtoms ()
		{
			List<AtomBase> selectAtomBase = new List<AtomBase> ();

			if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode)
			{
				selectAtomBase = WebDynamicGrid.RootDesignGrid.GetSelectAtomList ();
			}
			else
			{
				foreach (Atom atomcore in this.GetAllAtomCores ())
				{
					AtomBase atom = atomcore.GetOfAtom ();
					if (Visibility.Visible == atom.GetResizeAdornerVisibility ())
					{
						selectAtomBase.Add (atom);
					}
				}
			}

			return selectAtomBase;
		}

		/// <summary>
		/// 여러개의 아톰을 선택했을 때 스크롤이나 팝업이 포함되어 있으면 삭제하지 않고 경고메시지
		/// 스크롤과 팝업은 따로따로 삭제해주어야 한다.
		/// </summary>
		/// <param name="SourceAtomList"></param>
		/// <returns></returns>		
		public bool IsExistScrollOrPopup (List<AtomBase> SourceAtomList)
		{
			if (null != SourceAtomList)
			{
				foreach (AtomBase CurrentAtom in SourceAtomList)
				{
					Type CurrentAtomType = CurrentAtom.GetType ();

					if (CurrentAtom is PopupAtomBase || CurrentAtom is ScrollAtomBase)
					{
						return true;
					}
				}
			}

			return false;
		}


		/// <summary>
		/// 파라미터로 온 아톰들에 대해서 가장 높은 ZIndex를 가진 아톰을 반환한다.
		/// 아톰들은 뷰, 탭뷰, 스크롤에 대해서 상대적인 ZIndex를 가지기 때문에 이러한 Tree구조도 완벽히 고려한다.
		/// </summary>
		/// <param name="lstAtoms"></param>
		/// <returns></returns>
		public AtomBase GetTopMostAtomForZIndex (List<AtomBase> lstAtoms)
		{
			return GetTopMostAtomForZIndex2 (lstAtoms);

			//int nTopMostZIndex = -9999;
			//AtomBase TopMostAtom = null;

			////잠김아톰은 현재 마우스포인트 지점에 잠김아톰 딱 하나만 있을때만 선택되도록 수정
			//foreach (AtomBase atom in lstAtoms)
			//{
			//	//잠겨있을때
			//	if (true == atom.IsLock())
			//	{
			//		if (atom.IsBindedScroll || atom.IsBindedTabView)
			//		{
			//			if (2 == lstAtoms.Count) return atom;
			//			else if (2 < lstAtoms.Count) continue;
			//		}
			//		else
			//		{
			//			//하나면 잠긴 아톰 자체선택 
			//			if (1 == lstAtoms.Count) return atom;
			//			//두개 이상이면 잠긴 아톰의 아래의 것 선택
			//			else if (1 < lstAtoms.Count) continue;
			//		}
			//	}

			//	int nCurrentAtomZindex = Canvas.GetZIndex(atom);

			//	if (true == atom.IsBindedTabView)
			//	{
			//		FrameworkElement TabViewElement = atom.GetTabViewAtom();

			//		if (null != TabViewElement)
			//		{
			//			TabViewAtomBase TabViewAtom = TabViewElement as TabViewAtomBase;
			//			bool bIsInActiveTabPage = TabViewAtom.IsAtomInActiveTabPage(atom);

			//			if (true == bIsInActiveTabPage)
			//			{
			//				int nTabViewZIndex = Canvas.GetZIndex(TabViewElement);
			//				int nFinalZIndex = nTabViewZIndex + nCurrentAtomZindex;

			//				if (nTopMostZIndex < nFinalZIndex)
			//				{
			//					nTopMostZIndex = nFinalZIndex;
			//					TopMostAtom = atom;
			//				}
			//			}
			//		}
			//	}
			//	else if (true == atom.IsBindedScroll)
			//	{
			//		FrameworkElement ScrollElement = atom.GetScrollAtom();

			//		if (null != ScrollElement)
			//		{
			//			int nScrollZIndex = Canvas.GetZIndex(ScrollElement);

			//			ScrollAtomBase ScrollAtom = ScrollElement as ScrollAtomBase;

			//			if (true == ScrollAtom.IsBindedTabView)
			//			{
			//				FrameworkElement TabViewElement = ScrollAtom.GetTabViewAtom();

			//				if (null != TabViewElement)
			//				{
			//					int nTabViewZIndex = Canvas.GetZIndex(TabViewElement);
			//					int nFinalZIndex = nScrollZIndex + nTabViewZIndex + nCurrentAtomZindex;

			//					if (nTopMostZIndex < nFinalZIndex)
			//					{
			//						nTopMostZIndex = nFinalZIndex;
			//						TopMostAtom = atom;
			//					}
			//				}
			//			}
			//			else
			//			{
			//				int nFinalZIndex = nScrollZIndex + nCurrentAtomZindex;

			//				if (nTopMostZIndex < nFinalZIndex)
			//				{
			//					nTopMostZIndex = nFinalZIndex;
			//					TopMostAtom = atom;
			//				}
			//			}
			//		}
			//	}
			//             else if (true == atom.IsBindedPopup) //2020-02-14 팝업에 묶인 아톰이 스크롤 내부로 그려지는 논리 추가되면서 zindex논리 보강함
			//             {
			//                 FrameworkElement ScrollElement = atom.GetPopupAtom().GetScrollAtom();
			//                 if (null != ScrollElement)
			//                 {
			//                     FrameworkElement popupElement = atom.GetPopupAtom ();
			//                     int nPopupZIndex = Canvas.GetZIndex (popupElement);

			//                     int nScrollZIndex = Canvas.GetZIndex(ScrollElement);

			//                     ScrollAtomBase ScrollAtom = ScrollElement as ScrollAtomBase;

			//                     if (true == ScrollAtom.IsBindedTabView)
			//                     {
			//                         FrameworkElement TabViewElement = ScrollAtom.GetTabViewAtom();

			//                         if (null != TabViewElement)
			//                         {
			//                             int nTabViewZIndex = Canvas.GetZIndex(TabViewElement);
			//                             int nFinalZIndex = nScrollZIndex + nTabViewZIndex + nCurrentAtomZindex;

			//                             if (nTopMostZIndex < nFinalZIndex)
			//                             {
			//                                 nTopMostZIndex = nFinalZIndex;
			//                                 TopMostAtom = atom;
			//                             }
			//                         }
			//                     }
			//                     else
			//                     {
			//                         //int nFinalZIndex = nScrollZIndex + nPopupZIndex + nCurrentAtomZindex;
			//                         int nFinalZIndex = nScrollZIndex + nCurrentAtomZindex;

			//                         if (nTopMostZIndex < nFinalZIndex)
			//                         {
			//                             nTopMostZIndex = nFinalZIndex;
			//                             TopMostAtom = atom;
			//                         }
			//                     }
			//                 }
			//                 else
			//                 {
			//                     int nCurrentAtomZIndex = Canvas.GetZIndex(atom);

			//                     if (nTopMostZIndex < nCurrentAtomZIndex)
			//                     {
			//                         nTopMostZIndex = nCurrentAtomZIndex;
			//                         TopMostAtom = atom;
			//                     }
			//                 }
			//             }
			//             else
			//             {
			//                 int nCurrentAtomZIndex = Canvas.GetZIndex(atom);

			//                 if (nTopMostZIndex < nCurrentAtomZIndex)
			//                 {
			//                     nTopMostZIndex = nCurrentAtomZIndex;
			//                     TopMostAtom = atom;
			//                 }
			//             }
			//}

			//return TopMostAtom;
		}

		public AtomBase GetTopMostAtomForZIndex2 (List<AtomBase> lstAtoms)
		{
			int nTopMostZIndex = -9999;
			AtomBase TopMostAtom = null;

			//잠김아톰은 현재 마우스포인트 지점에 잠김아톰 딱 하나만 있을때만 선택되도록 수정
			foreach (AtomBase atom in lstAtoms)
			{
				//잠겨있을때
				if (true == atom.AtomCore.Attrib.IsLocked)
				{
					if (atom.AtomCore.IsBindedScroll || atom.AtomCore.IsBindedTabView)
					{
						if (2 == lstAtoms.Count) return atom;
						else if (2 < lstAtoms.Count) continue;
					}
					else
					{
						//하나면 잠긴 아톰 자체선택 
						if (1 == lstAtoms.Count) return atom;
						//두개 이상이면 잠긴 아톰의 아래의 것 선택
						else if (1 < lstAtoms.Count) continue;
					}
				}

				int nCurrentAtomZindex = Canvas.GetZIndex (atom);
				int nTabViewZIndex = 0;
				int nScrollZIndex = 0;
				int nPopupZIndex = 0;

				FrameworkElement TabViewElement = atom.GetTabViewAtom ();
				if (null != TabViewElement)
				{
					TabViewAtomBase TabViewAtom = TabViewElement as TabViewAtomBase;
					bool bIsInActiveTabPage = TabViewAtom.IsAtomInActiveTabPage (atom);
					if (true == bIsInActiveTabPage)
					{
						nTabViewZIndex = Canvas.GetZIndex (TabViewElement);
					}
					else
					{
						continue;
					}
				}

				FrameworkElement ScrollElement = atom.GetScrollAtom ();
				if (null != ScrollElement)
				{
					nScrollZIndex = Canvas.GetZIndex (ScrollElement);
				}

				FrameworkElement PopupElement = atom.GetPopupAtom ();
				if (null != PopupElement)
				{
					nPopupZIndex = Canvas.GetZIndex (PopupElement);

					PopupofAtom pPopupAtom = PopupElement as PopupofAtom;
					ScrollElement = pPopupAtom.GetScrollAtom ();
					if (null != ScrollElement)
					{
						nScrollZIndex = Canvas.GetZIndex (ScrollElement);
					}
				}

				int nFinalZIndex = nTabViewZIndex + nScrollZIndex + nPopupZIndex + nCurrentAtomZindex;

				if (nTopMostZIndex < nFinalZIndex)
				{
					nTopMostZIndex = nFinalZIndex;
					TopMostAtom = atom;
				}
			}

			return TopMostAtom;
		}

		/// <summary>
		/// View위에 있는 아톰들중 현재 선택된 아톰들을 모두 가져온다.(탭뷰에 바인딩된 아톰이나 스크롤에 바인딩된 아톰들은 예외)
		/// </summary>
		/// <returns></returns>
		public List<AtomBase> GetCurrentSelectedAtomsInLightDMTView ()
		{
			List<AtomBase> lstCurrentSelectedAtomsInLightDMTView = new List<AtomBase> ();

			UIElementCollection CurrentChildren = GetChildren ();
			int nCurrentChildrenCount = CurrentChildren.Count;

			for (int nIndex = 0; nIndex < nCurrentChildrenCount; nIndex++)
			{
				AtomBase currentAtom = CurrentChildren[nIndex] as AtomBase;

				if (null != currentAtom)
				{
					if (Visibility.Visible == currentAtom.GetResizeAdornerVisibility ())
					{
						lstCurrentSelectedAtomsInLightDMTView.Add (currentAtom);
					}
				}
			}

			return lstCurrentSelectedAtomsInLightDMTView;
		}

		/// <summary>
		/// 탭뷰에 바인딩된 아톰들중 현재 선택된 아톰들을 모두 가져온다.
		/// </summary>
		/// <returns></returns>
		public List<AtomBase> GetCurrentSelectedAtomsInTabView ()
		{
			List<AtomBase> lstCurrentSelectedChildAtoms = new List<AtomBase> ();

			UIElementCollection CurrentChildren = GetChildren ();
			int nCurrentChildrenCount = CurrentChildren.Count;

			for (int nIndex = 0; nIndex < nCurrentChildrenCount; nIndex++)
			{
				AtomBase currentAtom = CurrentChildren[nIndex] as AtomBase;

				if (null != currentAtom)
				{
					if (currentAtom is TabViewAtomBase)
					{
						TabViewAtomBase TabViewAtom = currentAtom as TabViewAtomBase;
						int nCurrentSelectedChildAtomCount = TabViewAtom.GetCurrentSelectedChildAtomCount ();

						if (0 < nCurrentSelectedChildAtomCount)
						{
							lstCurrentSelectedChildAtoms = TabViewAtom.GetCurrentSelectedChildAtoms ();
						}
					}
				}
			}

			return lstCurrentSelectedChildAtoms;
		}

		/// <summary>
		/// 스크롤에 바인딩된 아톰들중 현재 선택된 아톰들을 모두 가져온다.
		/// </summary>
		/// <returns></returns>
		public List<AtomBase> GetCurrentSelectedAtomsInScroll ()
		{
			List<AtomBase> lstCurrentSelectedChildAtoms = new List<AtomBase> ();

			UIElementCollection CurrentChildren = GetChildren ();

			for (int nIndex = 0; nIndex < CurrentChildren.Count; nIndex++)
			{
				AtomBase currentAtom = CurrentChildren[nIndex] as AtomBase;

				if (null != currentAtom)
				{
					if (currentAtom is ScrollAtomBase)
					{
						ScrollAtomBase ScrollAtom = currentAtom as ScrollAtomBase;
						ScrollAtom ScrollCore = ScrollAtom.AtomCore as ScrollAtom;

						int nCurrentSelectedChildAtomCount = ScrollCore.GetCurrentSelectedChildAtomCount ();

						if (0 < nCurrentSelectedChildAtomCount)
						{
							lstCurrentSelectedChildAtoms = ScrollCore.GetCurrentSelectedChildAtoms ();
						}
					}
				}
			}

			return lstCurrentSelectedChildAtoms;
		}


		/// <summary>
		/// 2020-08-13 kys 모든 선택된 아톰을 가져온다. (바인딩된 아톰도 포함)
		/// </summary>
		/// <returns></returns>
		public List<AtomBase> GetCurrnetSelectedAtomsInAll ()
		{
			List<AtomBase> lstCurrentSelectedChildAtoms = new List<AtomBase> ();

			List<Atom> pAtomCoreList = this.GetAllAtomCores ();

			foreach (Atom pAtomCore in pAtomCoreList)
			{
				AtomBase pAtomBase = pAtomCore.GetOfAtom ();

				if (Visibility.Visible == pAtomBase.GetResizeAdornerVisibility ())
				{
					lstCurrentSelectedChildAtoms.Add (pAtomBase);
				}
			}

			return lstCurrentSelectedChildAtoms;
		}

		public ToolBarProperty GetAtomProperties (AtomBase sourceAtom)
		{
			ToolBarProperty toolBarProperty = new ToolBarProperty ();

			if (null != sourceAtom)
			{
				bool CellBorderTypesVisibility = sourceAtom is GridTableOfAtom || sourceAtom is GridTableExofAtom;
				Brush lineBrush = sourceAtom.GetAtomBorder ();
				Brush backgroundBrush = sourceAtom.GetAtomBackground ();
				Thickness atomThickness = sourceAtom.GetAtomThickness ();
				DoubleCollection atomDashArray = sourceAtom.GetAtomDashArray ();
				bool bIsNoBackground = false;
				bool bIsNoLine = 0 >= atomThickness.Left;

				if (Brushes.Transparent == backgroundBrush || null == backgroundBrush || backgroundBrush is ImageBrush)
				{
					bIsNoBackground = true;
				}
				else
				{
					//2019-07-17 기능버튼의 채우기 없음 옵션이 취소되는 문제가 발생하여 추가함
					//실행모드에서 기능버튼의 background를 동작에 맞게 변경한다. 이때 기능버튼의 backgroudnBrush의 IsFrozen 속성이 true에서 false로 변경되면서 Brushes.Transparent과 일치하지 않는 문제가 발생함
					//따라서 Brushes의 16진수 값을 가지고 와서 비교하는 논리를 추가함
					string str16BackgroundBrush = backgroundBrush.ToString ();
					string str16TransparentBrush = Brushes.Transparent.ToString ();

					if (str16TransparentBrush == str16BackgroundBrush)
					{
						bIsNoBackground = true;
					}
				}

				string strFontName = string.Empty;
				string strFieldType = string.Empty;

				Atom atomCore = sourceAtom.AtomCore as Atom;
				Attrib atomAttrib = null == atomCore ? null : atomCore.GetAttrib ();
				CObjectFont pObjectFont = null;
				FontFamily atomFontFamily = null;

				if (null == atomCore || null == atomAttrib)
				{
					atomFontFamily = sourceAtom.GetAtomFontFamily ();
				}
				else
				{
					int nKey = atomAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._FONT);
					atomAttrib.GetGDIObjFromKey (ref pObjectFont, nKey);

					if (null != pObjectFont && null != pObjectFont.SelectFont)
					{
						atomFontFamily = new FontFamily (pObjectFont.SelectFont.OriginalFontName);
					}

					if (null == atomFontFamily)
					{
						atomFontFamily = sourceAtom.GetAtomFontFamily ();
					}

					strFieldType = atomAttrib.GetFieldType ();
				}

				bool? bIsEnabledBorderBrush = null;

				//2020-05-15 kys 캘린더 아톰의 경우 여러개의 폰트키를 관리해주고 있기 때문에 별도 처리 필요
				if (sourceAtom is CalendarofAtom ||
					sourceAtom is AccordionViewofAtom ||
					sourceAtom is TabViewofAtom ||
					sourceAtom is RadialMenuofAtom)
				{
					atomFontFamily = sourceAtom.GetAtomFontFamily ();
				}

				if (sourceAtom is CalendarofAtom)
				{
					CalendarofAtom pAtom = sourceAtom as CalendarofAtom;
					bIsEnabledBorderBrush = pAtom.IsEnabledBorderBrush;
                    //20250211 KH CalendarAttrib Get BorderIndex
                    CalendarAttrib attrib = (CalendarAttrib)pAtom.AtomCore.GetAttrib();
                    toolBarProperty.SelectAtomeBorderIndex = attrib.BorderIndex;

                }

                toolBarProperty.TargetType = sourceAtom.GetType ();
				toolBarProperty.UpdateFontFamily = atomFontFamily;
				toolBarProperty.FontSize = sourceAtom.GetAtomFontSize ();
				toolBarProperty.UpdateFontWeight = sourceAtom.GetAtomFontWeight ();
				toolBarProperty.UpdateFontStyle = sourceAtom.GetAtomFontStyle ();
				toolBarProperty.UpdateTextDecorationLocation = sourceAtom.GetTextUnderLine ();
				toolBarProperty.UpdateHorizontalAlignment = sourceAtom.GetHorizontalTextAlignment ();
				toolBarProperty.UpdateVerticalAlignment = sourceAtom.GetVerticalTextAlignment ();
				toolBarProperty.FontBrush = sourceAtom.GetAtomFontColor ();
				toolBarProperty.LineBrush = lineBrush;
				toolBarProperty.BackgroundBrush = backgroundBrush;
				toolBarProperty.IsNoLIne = bIsNoLine;
				toolBarProperty.LineThickness = atomThickness;
				toolBarProperty.UpdateDoubleCollection = atomDashArray;
				toolBarProperty.IsNoBackgrond = bIsNoBackground;
				toolBarProperty.AtomName = sourceAtom.AtomCore.AtomProperVar;
				toolBarProperty.IsHide = sourceAtom.AtomCore.Attrib.IsAtomHidden;
				toolBarProperty.IsCellBordertypeVisibilty = CellBorderTypesVisibility;
				toolBarProperty.ValueType = strFieldType;
				toolBarProperty.AtomOpactiy = sourceAtom.GetAtomOpacity ();
				toolBarProperty.IsLineColorEnabled = bIsEnabledBorderBrush;
				toolBarProperty.IsShadow = sourceAtom.AtomCore.Attrib.IsShadow;
				toolBarProperty.SelectAtomCount = 1;
			}

			return toolBarProperty;
		}

		public bool CheckAtomCanMove (List<AtomBase> lstCurrentSelectedAtoms)
		{
			foreach (AtomBase atomBase in lstCurrentSelectedAtoms)
			{
				if (false == atomBase.CheckAtomComponentHasGotMouseHandle ())
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// ReadyAtom의 위치를 조정한다.
		/// </summary>
		/// <param name="currentPoint"></param>
		/// <param name="ReadyAtom"></param>
		public void HandleReadyAtomLocation (Point currentPoint, AtomBase ReadyAtom)
		{
			Size LightDMTViewSize = GetViewSize ();
			Thickness orgMargin = ReadyAtom.Margin;
			Thickness resultMargin = new Thickness ();

			double dLeft = currentPoint.X - (ReadyAtom.Width / 2);
			double dTop = currentPoint.Y - (ReadyAtom.Height / 2);
			resultMargin.Left = dLeft;
			resultMargin.Top = dTop;

			if (0 > dLeft)
			{
				resultMargin.Left = 0;
			}

			if (0 > dTop)
			{
				resultMargin.Top = 0;
			}

			ReadyAtom.Margin = resultMargin;
		}

		/// <summary>
		/// 멀티셀렉트 된 아톰들을 움직일 경우 마진이 뷰의 영역의 끝에 닿으면 유효하지 않은 그룹이므로 false
		/// </summary>
		/// <param name="lstCurrentSelectedAtoms"></param>
		/// <param name="lstSelectedAtomsMarginAfter"></param>
		/// <returns></returns>
		public bool IsValidMarginGroup (List<AtomBase> lstCurrentSelectedAtoms, List<Thickness> lstSelectedAtomsMarginAfter)
		{
			Size CurrentViewSize = GetViewSize ();
			int nAtomsCount = lstCurrentSelectedAtoms.Count;

			if (1 == nAtomsCount)
			{
				AtomBase CurrentAtom = lstCurrentSelectedAtoms[0] as AtomBase;
				Thickness applyMargin = (Thickness)lstSelectedAtomsMarginAfter[0];

				double dStartX = 0 < applyMargin.Left ? applyMargin.Left : 0;
				double dStartY = 0 < applyMargin.Top ? applyMargin.Top : 0;

				//if (0 > dStartX)
				//{
				//    dStartX = 0;
				//}

				//if (0 > dStartY)
				//{
				//    dStartY = 0;
				//}

				lstSelectedAtomsMarginAfter[0] = new Thickness (dStartX, dStartY, 0, 0);
			}
			else if (1 < nAtomsCount)
			{
				double minX = lstSelectedAtomsMarginAfter.Select (margin => margin.Left).Min ();
				double minY = lstSelectedAtomsMarginAfter.Select (margin => margin.Top).Min ();

				for (int nIndex = 0; nIndex < nAtomsCount; nIndex++)
				{
					AtomBase CurrentAtom = lstCurrentSelectedAtoms[nIndex] as AtomBase;
					Thickness applyMargin = (Thickness)lstSelectedAtomsMarginAfter[nIndex];

					double dStartX = 0 > minX ? applyMargin.Left - minX : applyMargin.Left;
					double dStartY = 0 > minY ? applyMargin.Top - minY : applyMargin.Top;

					//if (0 >= dStartX || 0 >= dStartY)
					//{
					//    return false;
					//}

					//80
					//if (0 > dStartX)
					//{
					//    dStartX = 0;
					//}

					//if (0 > dStartY)
					//{
					//    dStartY = 0;
					//}

					lstSelectedAtomsMarginAfter[nIndex] = new Thickness (dStartX, dStartY, 0, 0);
				}
			}

			return true;
		}

		public void CopyAtomsAtRegularIntervals (Key direction)
		{
			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInLightDMTView ();
			List<AtomBase> lstCurrentSelectedAtomsInTabView = GetCurrentSelectedAtomsInTabView ();
			List<AtomBase> lstCurrentSelectedAtomsInScroll = GetCurrentSelectedAtomsInScroll ();

			if (0 < lstCurrentSelectedAtoms.Count)
			{
				Thickness startMargin = new Thickness ();
				Size EndSize = Size.Empty;
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstCurrentSelectedAtoms, ref startMargin, ref EndSize);

				if (true == bIsCalculated)
				{
					//bool bIsPossibleCopy = IsPossibleCopyAtomsAtRegularIntervals(direction, startMargin, EndSize);

					//if (true == bIsPossibleCopy)
					//{
					//    StartCopyAtoms(direction, lstCurrentSelectedAtoms, startMargin, EndSize);
					//}

					StartCopyAtoms (direction, lstCurrentSelectedAtoms, startMargin, EndSize);
				}
			}

			if (0 < lstCurrentSelectedAtomsInTabView.Count)
			{
				Thickness startMargin = new Thickness ();
				Size EndSize = Size.Empty;
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstCurrentSelectedAtomsInTabView, ref startMargin, ref EndSize);

				if (true == bIsCalculated)
				{
					//bool bIsPossibleCopy = IsPossibleCopyAtomsAtRegularIntervals(direction, startMargin, EndSize);

					//if (true == bIsPossibleCopy)
					//{
					//    StartCopyAtoms(direction, lstCurrentSelectedAtomsInTabView, startMargin, EndSize);
					//}

					StartCopyAtoms (direction, lstCurrentSelectedAtomsInTabView, startMargin, EndSize);
				}
			}

			if (0 < lstCurrentSelectedAtomsInScroll.Count)
			{
				Thickness startMargin = new Thickness ();
				Size EndSize = Size.Empty;
				bool bIsCalculated = CalculateAtomsRegionStartPointAndSize (lstCurrentSelectedAtomsInScroll, ref startMargin, ref EndSize);

				if (true == bIsCalculated)
				{
					//bool bIsPossibleCopy = IsPossibleCopyAtomsAtRegularIntervals(direction, startMargin, EndSize);

					//if (true == bIsPossibleCopy)
					//{
					//    StartCopyAtoms(direction, lstCurrentSelectedAtomsInScroll, startMargin, EndSize);
					//}

					StartCopyAtoms (direction, lstCurrentSelectedAtomsInScroll, startMargin, EndSize);
				}
			}

			lstCurrentSelectedAtoms = null;
			lstCurrentSelectedAtomsInTabView = null;
			lstCurrentSelectedAtomsInScroll = null;
		}

		/// <summary>
		/// Ctrl 키보드 상태에서 마우스 이동시 아톰복사
		/// </summary>
		/// <param name="ptMove"></param>
		public void CopyAtomsAtMouseMove (Point ptMove)
		{
			List<AtomBase> lstCurrentSelectedAtoms = GetCurrentSelectedAtomsInLightDMTView ();
			List<AtomBase> lstCurrentSelectedAtomsInTabView = GetCurrentSelectedAtomsInTabView ();
			List<AtomBase> lstCurrentSelectedAtomsInScroll = GetCurrentSelectedAtomsInScroll ();

			//Kiho : 2016-11-10 : 복사되는 아톰을 저장하기 위한 변수 할당
			if (this.m_lstOriginalCopyedAtoms == null)
			{
				this.m_lstOriginalCopyedAtoms = new List<AtomBase> ();
			}
			else
				this.m_lstOriginalCopyedAtoms.Clear ();

			if (0 < lstCurrentSelectedAtoms.Count)
			{
				ProcessCopyAtoms (lstCurrentSelectedAtoms, ptMove);
			}

			if (0 < lstCurrentSelectedAtomsInTabView.Count)
			{
				ProcessCopyAtoms (lstCurrentSelectedAtomsInTabView, ptMove);
			}

			if (0 < lstCurrentSelectedAtomsInScroll.Count)
			{
				ProcessCopyAtoms (lstCurrentSelectedAtomsInScroll, ptMove);
			}

			lstCurrentSelectedAtoms = null;
			lstCurrentSelectedAtomsInTabView = null;
			lstCurrentSelectedAtomsInScroll = null;
		}

		public bool ApplyCommonEvent (DelegateStructType.EventSourceType EventSourceType, int nEventKey, object value)
		{
			int nEventSourceKey = (int)EventSourceType;

			switch (nEventSourceKey)
			{
				case 0:
				case 1:
				case 2: return ApplyObjectPropertyToolBarGroupEvent (nEventKey, value);
				default: break;
			}

			return false;
		}

		public bool ApplyAtomName (List<AtomBase> selectedAtoms, string strAtomName)
		{
			bool bResult = false;

			//대체디자인 모드가 활성화된 경우
			if (null != WebDynamicGrid && true == WebDynamicGrid.IsDesignMode)
			{
				if (1 == selectedAtoms.Count)
				{
					bResult = true;

					AtomBase selectAtom = selectedAtoms.FirstOrDefault ();

					if (null == selectAtom || selectAtom.AtomCore.AtomProperVar == strAtomName)
						return true;

					List<Atom> atomCoreList = GetAllAtomCores ();
					WebReplaceDesignofAtom designAtom = WebDynamicGrid.RootDesignGrid.DesignAtom;

					DynamicGridTableAttrib.SetAtomList (atomCoreList);

					if (null != designAtom)
					{
						int nRow = Grid.GetRow (designAtom);
						int nColumn = Grid.GetColumn (designAtom);

						DynamicGridItem gridItem = DynamicGridTableAttrib.GetGridItem (designAtom);
						List<Atom> allAtomList = GetAllAtomCores ();
						List<Atom> cellAtomList = gridItem.AtomCoreList;
						List<Atom> designAtomList = designAtom.GetCurrentModeAtomList ();

						//대체디자인이 설정된 원본셀의 아톰명칭과 같은경우만 중복명칭 가능
						if (null != designAtomList)
						{
							if (0 < designAtomList.Where (item => strAtomName == item.GetProperVar ()).Count ())
							{
								bResult = false;
							}
						}

						if (null != allAtomList && null != cellAtomList)
						{
							allAtomList.RemoveAll (item => cellAtomList.Contains (item));

							if (true == bResult && 0 < allAtomList.Where (item => strAtomName == item.GetProperVar ()).Count ())
							{
								bResult = false;
							}
						}

						if (false == bResult)
						{
							_Message80.Show (LC.GS ("TopProcess_DMTDoc_6"));
						}
						else
						{
							selectAtom.AtomCore.AtomProperVar = strAtomName;
							AutoRefreshEditWindow ();
						}
					}
				}

				return bResult;
			}

			if (string.Empty != strAtomName)
			{
				int nAtomCount = selectedAtoms.Count;

				if (1 == nAtomCount)
				{
					AtomBase TargetAtom = selectedAtoms[0];
					bResult = true;

					if (IsExistSameAtomProperVarInDMTView (strAtomName, TargetAtom))
					{
						_Message80.Show (LC.GS ("TopProcess_DMTDoc_6"));
						return false;
					}

					if (!PQAppBase.IsValidProperVar (strAtomName))
					{
						_Message80.Show (LC.GS ("아톰명에는 특수문자 또는 공백을 사용할 수 없습니다."));
						return false;
					}

					// 사용할수 없는 아톰명입니다. (자연어 사전에 있는 RUN 과 충돌문제)
					// 내장함수.. (101 ~ 200) 

					int nID = CScriptApp.GetIDFromName (strAtomName);
					CFuncServer funcServer = new CFuncServer ();

					if (funcServer.GetScriptFuncNameIDList ().Contains (nID) ||
						strAtomName == LC.GS ("DicInfo_DicInfo_2004_10") || //조회결과
						strAtomName == LC.GS ("DicInfo_DicInfo_2004_5") //실행결과
						)
					{
						_Message80.Show (LC.GS ("TopProcess_DMTDoc_2004_1"));
						return false;
					}

					if (TargetAtom.AtomCore.AtomProperVar != strAtomName)
					{
						TargetAtom.AtomCore.AtomProperVar = strAtomName;

						//F3키를 통해 아톰명이 표시된 상태로 변경하는 경우 바로 적용될 수 있도록 논리 보강
						if (TargetAtom.IsAtomNameTextVisible)
							TargetAtom.InvalidateVisual ();

						//애니관리자 진행관리자, 아톰편집도우미등 아톰 리스트 갱신
						AutoRefreshEditWindow ();
					}
				}
			}

			return bResult;
		}

		public void ApplyAtomFieldType (List<AtomBase> selectedAtoms, string strAtomFieldType)
		{
			if (string.Empty != strAtomFieldType)
			{
				int nAtomCount = selectedAtoms.Count;

				if (1 == nAtomCount)
				{
					AtomBase TargetAtom = selectedAtoms[0];
					TargetAtom.SetAtomFieldType (strAtomFieldType);
				}
			}
		}

		#region |  ----- PreExecute 관련 -----  |
		/// <summary>
		/// 80 로직이 약간 이상함 
		/// strDefaultTableName = SetTableNameAtEmptyUnit()은 첫번째 뷰에서 테이블명을 찾는다 
		/// SetAutoErdSetInformation(strDefaultTableName)에서 첫번째뷰의 테이블명을 다시찾는 로직이 들어있음 
		/// 없으면 strDefaultTableName을 넣는다. 기능 상의 문제는 없음  
		/// </summary>
		public void AutomaticSetEmptyKeyAndTable ()
		{
			//80 테이블 명을 정하는 로직 
			//80 view 상의 아톰을 대상으로 
			string strDefaultTableName = SetTableNameAtEmptyUnit ();
			this.ChangeDefaultTableName (strDefaultTableName);

			//80 LoadField를 정하는 로직 
			//80 모든 아톰을 대상으로
			SetPrimaryKeyInformation ();
			SetAutoErdSetInformation (strDefaultTableName);
		}

		public void SetDefaultTable (string strDefaultTable)
		{
			m_strDefaultTable = strDefaultTable;
			m_bChangedTitle = false;
		}

		public string GetDefaultTable ()
		{
			return m_strDefaultTable;
		}

		/// <summary>
		/// 레이블 이름을 생성한다.
		/// </summary>
		/// <param name="pTouchAtom"></param>
		/// <returns></returns>
		public string FindLabelName (Atom pTouchAtom)
		{
			COrderAtom pOrderAtom = GetOrderAtom ();
			if (null == pOrderAtom)
				return "";

			int nScriptIndex = pTouchAtom.GetScriptIndex ();
			string sScriptIndex = string.Format ("${0}$", (object)(nScriptIndex));

			foreach (Atom pAtom in pOrderAtom)
			{
				if (false != _Kiss.IsKindOf (pAtom, typeof (LookAtom)))
				{
					string strScriptIndex = ((LookAtom)(pAtom)).GetIncludedScriptIndex ();
					if (0 == strScriptIndex.Length) continue;

					strScriptIndex = string.Concat ("$", strScriptIndex);
					if (-1 < strScriptIndex.IndexOf (sScriptIndex))
					{
						Attrib pAttrib = pAtom.Attrib;
						if (null == pAttrib) continue;
						string strLabel = ((LookAttrib)(pAttrib)).GetTitle ();
						return strLabel;
					}
				}
			}
			return "";
		}

		public string MakeLableOfAtomKind (string lpszClassName)
		{
			string strLabel = (ProcessDef.EG_TIGHT_TITLE);

			if (true == lpszClassName.Equals ("CNtoaKwd"))
				strLabel = NtoaAtomName.KWD;
			else if (true == lpszClassName.Equals ("CNtoaDHtml"))
				strLabel = NtoaAtomName.URL;
			else if (true == lpszClassName.Equals ("CNtoaMail"))
				strLabel = NtoaAtomName.MAILMESSAGE;
			else if (true == lpszClassName.Equals ("CNtoaAttach"))
				strLabel = NtoaAtomName.ATTACH;
			else if (true == lpszClassName.Equals ("CNtoaSender"))
				strLabel = NtoaAtomName.SENDER;
			else if (true == lpszClassName.Equals ("CNtoaMemo"))
				strLabel = NtoaAtomName.MEMO;
			else if (true == lpszClassName.Equals ("CNtoaPenMemo"))
				strLabel = NtoaAtomName.PENMEMO;
			else if (true == lpszClassName.Equals ("CNtoaRuntime"))
				strLabel = NtoaAtomName.RUNTIME;
			else if (true == lpszClassName.Equals ("CNtoaMove"))
				strLabel = NtoaAtomName.INSERTIMAGE;
			else if (true == lpszClassName.Equals ("CNtoaReceipt"))
				strLabel = NtoaAtomName.RECEIPT;
			else if (true == lpszClassName.Equals ("CNtoaReceive"))
				strLabel = NtoaAtomName.RECEIVE;
			else if (true == lpszClassName.Equals ("CNtoaOffice"))
				strLabel = NtoaAtomName.OFFICE;
			else if (true == lpszClassName.Equals ("CNtoaURL"))
				strLabel = NtoaAtomName.URL;
			else if (true == lpszClassName.Equals ("CScan"))
				strLabel = ProcessDef.FI_LABEL_SCAN;
			else if (true == lpszClassName.Equals ("CApprove"))
				strLabel = ProcessDef.FI_LABEL_APPROVE;
			else if (true == lpszClassName.Equals ("CSignal"))
				strLabel = AtomName.SIGNAL;

			return strLabel;
		}

		/// <summary>
		/// LHS (Erd자동저장)
		/// </summary>
		public void AutoSetErdInfoByTouch ()
		{
			if (false == PQAppBase.IsAutoDBSet) { return; }

			this.AutomaticSetEmptyKeyAndTable ();

			CDMTFrameAttrib pFrameAttrib = this.m_pFrameAttrib as CDMTFrameAttrib;

			if (null == pFrameAttrib)
				return;

			bool bAutoSave = pFrameAttrib.AutoERDSave;
			bool bOverWrite = pFrameAttrib.OldErdInfoOverWrite;

			string strDefaultTable = GetDefaultTable ();

			strDefaultTable = strDefaultTable.Replace (" ", "");

			DMTView dmtView = this.GetParentView () as DMTView;
			if (null == dmtView) { return; }



			List<Atom> lstAtomCores = new List<Atom> ();
			foreach (Atom atomCore in this.GetViewAtomCoresTabOrdered ())
			{
				lstAtomCores.Add (atomCore);

				if (atomCore is ScrollAtom)
				{
					ScrollAtom scrollAtomCore = atomCore as ScrollAtom;

					if (null != scrollAtomCore)
					{
						lstAtomCores.AddRange (scrollAtomCore.GetBindedAtomCores ());
					}
				}
			}

			foreach (Atom pAtom in lstAtomCores)
			{
				MasterInputAttrib pAttrib = pAtom.Attrib as MasterInputAttrib;
				//Table명 설정
				if (null != pAttrib)
				{
					if (pAtom is VirtualAtom)
						continue;

					// 2008.07.10 이정대, ERD연동 관련 동작시 저장안함일 경우, 제외함
					if (false == pAttrib.IsSaveField)
						continue;

					string strTable = pAttrib.GetTableName (false);

					if (0 < strDefaultTable.Length && (0 == strTable.Length || bOverWrite))
					{
						//2005.10.28, 이우성, 테이블명 설정
						if (m_bChangedTitle)
						{
							//테이블명이 같거나 테이블명이 공백일 경우에 테이블값을 변경해준다.
							if (0 == string.Compare (m_strOldDefaultTable, strTable, true) || 0 == strTable.Length)
								pAttrib.SetTableName (strDefaultTable, true);
						}
						else
						{
							//테이블명이 공백인 경우만 테이블값을 넣어준다.
							if (0 == strTable.Length)
								pAttrib.SetTableName (strDefaultTable, true);
						}
					}
				}

				//Label과 입력란의 연결 정보 clear
				if (false != pAtom.IsKindOf (typeof (SquareAtom)) ||
					false != pAtom.IsKindOf (typeof (OvalAtom)) ||
					false != pAtom.IsKindOf (typeof (RoundSquareAtom)))
				{
					string strNull = "";
					((LookAtom)(pAtom)).SetIncludedScriptIndex (strNull);
				}
			}

			//테이블값이 변경된것이므로 true로 설정
			m_bChangedTitle = true;

			//80 필요없음
			//m_strOldDefaultTable = strDefaultTable;



			// 가까운 데코(사각형)아톰으로 DB 필드명 만드는 로직 
			// 뷰에 있는 라디오 아톰들을 그룹으로 묶은 리스트를 라디오 아톰들 각각에게 설정한다.			


			dmtView.ClearRadioGroup ();
			dmtView.SetRadioGroup (GetViewAtomCores ());
			dmtView.AutoSetErdInfoByTouch (lstAtomCores);

			//80 전혀 타지 않는 로직으로 판단됨 
			//currentDMTView.MakeErdReleationForScroll();

			ToastMessge.Show (LC.GS ("TopProcess_DMTDoc_53")); //DB정보 자동설정 완료
		}

		public bool IsChangeTabNum ()
		{
			return m_bTabChange;
		}

		public void SetChangeTabNum (bool bChange)
		{
			m_bTabChange = bChange;
		}

		#endregion//PreExecute 관련

		public string GetDefaultProperVar (Atom pNewAtom)
		{
			Attrib CurrentAtomAttrib = pNewAtom.GetAttrib ();

			if (null != CurrentAtomAttrib)
			{
				return CurrentAtomAttrib.DefaultAtomProperVar;
			}

			return string.Empty;
		}

		public override string GetDefaultProperVar (string strDefaultProperVar)
		{
			CommonStringGenerator.StringGenerator.Clear ();
			string strNewAtomProperVar = strDefaultProperVar;
			UIElementCollection CurrentChildren = GetChildren ();
			int nChildrenCount = CurrentChildren.Count;

			for (int i = 0; ; i++)
			{
				bool bExistSameName = false;

				if (0 < i)
					strNewAtomProperVar = CommonStringGenerator.StringGenerator.AppendFormat ("{0}{1}", strDefaultProperVar, (i + 1)).ToString ();

				for (int nIndex = 0; nIndex < nChildrenCount; nIndex++)
				{
					AtomBase CurrentAtom = CurrentChildren[nIndex] as AtomBase;

					if (null != CurrentAtom)
					{
						Atom CurrentAtomCore = CurrentAtom.AtomCore;

						if (null != CurrentAtomCore)
						{
							if (strNewAtomProperVar == CurrentAtomCore.GetProperVar ())
							{
								bExistSameName = true;
								break;
							}
						}
					}
				}

				if (false == bExistSameName)
				{
					break;
				}
			}

			return strNewAtomProperVar;
		}

		public string TrimInValidString (string strLabelName)
		{
			string strColumn = strLabelName;
			strColumn = strColumn.TrimStart (null);
			strColumn = strColumn.TrimEnd (null);

			char[] chCode = {'-', '`', '~', '!', '@', '#', '$', '%', '^', '&',
								'*', '(', ')', '-', '+', '=', '\\', '|', '{',
								'}', '[', ']', ':', ';', '\'', '\"', ',', '.',
								'<', '>', '/', '?', '\r', '\n', '\t', '\b', '\0'};

			for (int i = 0; chCode[i] != '\0'; i++)
			{
				strColumn = strColumn.Replace (chCode[i], ' ');
			}

			strColumn = strColumn.Replace (" ", "");

			return strColumn;
		}

		public string TrimInValidStringRegex (string strLabelName)
		{
			string strColumn = strLabelName;

			//strColumn = Regex.Replace (strLabelName, @"[^a-zA-Z0-9가-힣_]", "", RegexOptions.Singleline, TimeSpan.FromMilliseconds (3000));
			// 전각숫자 0~9 까지 추가 함 \uFF10-\uFF19
			strColumn = Regex.Replace (strLabelName, @"[^a-zA-Z가-힣\u4E00-\u9FFF\u3040-\u30FC0-9\uFF10-\uFF19_]", "", RegexOptions.Singleline, TimeSpan.FromMilliseconds (3000));
			strColumn = strColumn.Trim ();

			int num = -1;

			for (int i = 0; i < strColumn.Length; i++)
			{
				char ch = strColumn[i];

				if ('0' <= ch && ch <= '9') //숫자일경우
				{
					num = i;
				}
				else
					break;
			}

			if (-1 < num)
			{
				//2020-03-25 kys 숫자로 시작하는 경우 앞에 있는 숫자를 전부 제거한다. 123입력란 -> 입력란
				num++;
				strColumn = strColumn.Substring (num, strColumn.Length - num);
			}

			// 예약어 처리
			if (IsReservedKeyword (strColumn))
			{
				strColumn = $"_{strColumn}";
			}


			return strColumn;
		}

		private bool IsReservedKeyword (string input)
		{
			// 데이터베이스 시스템의 예약어 목록
			string[] reservedKeywords = { "select", "insert", "update", "delete", "create", "drop", "alter", "join", "where", "order", "group", "having", "union", "distinct" };

			return reservedKeywords.Contains (input.ToLower ());
		}

		public string GetDefaultFieldName (string strCopyVar, Atom pNewAtom, bool bUseSPI)
		{
			string strNewVar = strCopyVar;
			for (int i = 0; ; i++)
			{
				bool bExistSameName = false;
				DMTView pView = this.GetParentView () as DMTView;
				List<Atom> pAtomList = pView.GetAllAtomCores ();
				foreach (Atom pAtomCore in pAtomList)
				{
					//AtomBase atomBase = objAtomBase as AtomBase;
					//if (null == atomBase) { continue; }
					//CAtom pAtom = atomBase.AtomCore;

					//if (null == pAtom)//|| pAtom.GetType () != pNewAtom.GetType ())
					//continue;
					//2005.12.10, 이우성, 자신의 변수명은 비교대상에서 제외한다.
					if (false != string.Equals (pAtomCore.GetFieldName ().ToUpper (), strNewVar.ToUpper ()) && false == pAtomCore.Equals (pNewAtom))
					{
						bExistSameName = true;
						break;
					}
				}

				//if (false != bExistSameName) 2020-03-06 kys F5눌러서 분석시에 필드명이 같은경우 기본 데이터를 만들어주기 위해서 주석처리함
				//break;

				if (false != bExistSameName)
				{
					if (false != bUseSPI)
					{
						string strNum = Convert.ToString (i + 1);
						strNewVar = string.Concat (strCopyVar, strNum);
					}
					else
					{
						// 2007.12.27 이정대, 같은 이름을 가진 아톰이 있으면, '기본이름 + 숫자'를 이름으로 준다.
						string strNum = "";
						if (i != 0)
							strNum = Convert.ToString (i);

						Attrib CurrentAtomAttrib = pNewAtom.GetAttrib ();

						if (null != CurrentAtomAttrib)
						{
							strNewVar = string.Concat (CurrentAtomAttrib.DefaultAtomProperVar, strNum);
						}
					}
				}
				else
				{
					break;
				}
			}
			return strNewVar;
		}

		/// <summary>
		/// 아톰을 복사했을 경우 고유변수를 초기화 시키지 않기 위해서
		/// 아톰명과 필드명을 체크하여 사용할 아톰명과 필드명을 반환한다.
		/// </summary>
		/// <param name="strCopyVar">사용하기 원하는 아톰명</param>
		/// <param name="pNewAtom">해당 아톰</param>
		/// <param name="bUseSPI">SPI 방법론에 사용되고 있는가?</param>
		/// <returns>부여가능한 아톰, 필드명</returns>
		public string GetDefaultProperVar (string strCopyVar, Atom pNewAtom, bool bUseSPI)
		{
			string strNewVar = strCopyVar;
			for (int i = 0; ; i++)
			{
				bool bExistSameName = false;

				DMTView pView = this.GetParentView () as DMTView;

				if (null == pView)
				{
					break;
				}

				foreach (Atom pAtom in pView.GetAllAtomCores ())
				{
					if (null == pAtom)//|| pAtom.GetType () != pNewAtom.GetType ())
						continue;
					//2005.12.10, 이우성, 자신의 변수명은 비교대상에서 제외한다.
					if (false != string.Equals (pAtom.GetProperVar (), strNewVar) && true != pAtom.Equals (pNewAtom))
					{
						bExistSameName = true;
						break;
					}
				}

				if (false != bExistSameName)
				{
					if (false != bUseSPI)
					{
						string strNum = Convert.ToString (i + 1);
						strNewVar = string.Concat (strCopyVar, strNum);
					}
					else
					{
						// 2007.12.27 이정대, 같은 이름을 가진 아톰이 있으면, '기본이름 + 숫자'를 이름으로 준다.
						string strNum = "";
						if (i != 0)
							strNum = Convert.ToString (i);

						Attrib CurrentAtomAttrib = pNewAtom.GetAttrib ();

						if (null != CurrentAtomAttrib)
						{
							strNewVar = string.Concat (CurrentAtomAttrib.DefaultAtomProperVar, strNum);
						}
					}
				}
				else
				{
					break;
				}
			}
			return strNewVar;
		}

		public string GetActionQuery (int nType)
		{
			string strSQL = "";
			DMTView pView = this.GetParentView () as DMTView;

			if (true == m_bFormChange)
			{
				if (false != m_bDropDBMaster) { ReMakeDBMaster (); }
				if (null == pView) return string.Empty;
				CreateHelperList ();
				pView.CreateViewHelperList ();
				pView.AddAllAtomToHelperList (false, -1);
				MakeSQLStatement ();
			}

			switch (nType)
			{
				case (int)AC_TYPE.AC_PGUP:
					strSQL = m_pDBMaster.GetAllSQLStatement ((int)SQLQUERY_TYPE._SELECT_, SQL_RECORD_TYPE.PREV_RECORD, true);
					break;// select
				case (int)AC_TYPE.AC_PGDN:
					strSQL = m_pDBMaster.GetAllSQLStatement ((int)SQLQUERY_TYPE._SELECT_, SQL_RECORD_TYPE.NEXT_RECORD, true);
					break;
				case (int)AC_TYPE.AC_SAVE:
				case (int)AC_TYPE.AC_SAVECLOSE:
					strSQL = m_pDBMaster.GetAllSQLStatement ((int)SQLQUERY_TYPE._INSERT_, SQL_RECORD_TYPE.UPDATE_RECORD, true);
					break;
				case (int)AC_TYPE.AC_SAVE_UPDATE:
					strSQL = m_pDBMaster.GetAllSQLStatement ((int)SQLQUERY_TYPE._UPDATE_, SQL_RECORD_TYPE.UPDATE_RECORD, true);
					break;
				case (int)AC_TYPE.AC_DELETE:
					strSQL = m_pDBMaster.GetAllSQLStatement ((int)SQLQUERY_TYPE._DELETE_, SQL_RECORD_TYPE.DELETE_RECORD, true);
					break;
			}

			if (true == m_bFormChange)
			{
				pView.DropViewHelperList ();
			}
			return strSQL;
		}

		#region |  ----- 저장관련 -----  |

		public void SaveTemplateInformation ()
		{
			#region|  주석  |
			//80 추후작업 템플릿 관련
			//if (null == this.m_pTemplateManager)
			//    return;
			//string strFileName = this.FilePath + ".template";

			//try
			//{
			//    this.m_pTemplateManager.SaveInformation();

			//    XmlSerializer pSerializer = new XmlSerializer(typeof(Softpower.SmartMaker.TopDefine.TemplateManager));

			//    using (TextWriter pWriter = new StreamWriter(strFileName))
			//    {
			//        pSerializer.Serialize(pWriter, this.m_pTemplateManager);
			//        pWriter.Close();
			//    }
			//}
			//catch (Softpower.SmartMaker.Define.SmartMakerException e)
			//{
			//}
			#endregion
		}

		/// <summary>
		/// 이미지 추가할때마다 Key 값이 계속 증가하는 논리이기 때문에
		/// 아톰삭제시 Key 값을 삭제하는 논리 (Undo,Redo 및 Key 값 재배치 논리필요) 가 없기 때문에
		/// 저장시 사용되지 않는 이미지 삭제함.
		/// </summary>
		public void RemoveGDIResource ()
		{
			Hashtable mapAtom = new Hashtable ();

			List<Atom> atomCoreList = this.GetOrderAtom ().Cast<Atom> ().ToList ();

			//반응형웹 논리 보강
			if (null != WebDynamicGrid)
			{
				List<Atom> dynamicAtomCoreList = WebDynamicGrid.RootDesignGrid.GetAllAtomCoreList ();

				if (0 < dynamicAtomCoreList.Count)
					atomCoreList.AddRange (dynamicAtomCoreList);
			}

			foreach (Atom pAtom in atomCoreList)
			{
				Attrib pAttrib = pAtom.GetAttrib ();
				int nImageKey = pAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._IMAGE);

				if (0 <= nImageKey)
				{
					if (false == mapAtom.ContainsKey (nImageKey))
					{
						mapAtom[nImageKey] = nImageKey;
					}
				}

				int nRolloverKey = pAttrib.GetRolloverKey ();
				if (0 <= nRolloverKey)
				{
					if (false == mapAtom.ContainsKey (nRolloverKey))
					{
						mapAtom[nRolloverKey] = nRolloverKey;
					}
				}

				if (pAtom is EBookImageAtom)
				{
					EBookImageAttrib ebookImageAttrib = pAtom.GetAttrib () as EBookImageAttrib;
					Dictionary<int, EBookImageInfo> ebookImageDic = ebookImageAttrib.ImageInfoDic;

					foreach (var varItem in ebookImageDic)
					{
						if (null == varItem.Value)
						{
							continue;
						}

						nImageKey = varItem.Value.ImageGDIKey;

						if (false == mapAtom.ContainsKey (nImageKey))
						{
							mapAtom[nImageKey] = nImageKey;
						}

						int ChangedType = varItem.Value.ImageDataSize;

						if (0 < ChangedType)
						{
							EBookImageAttrib EBookAttrib = pAtom.GetAttrib () as EBookImageAttrib;
							EBookAttrib.DPIChanged (varItem.Value.ImageGDIKey, ChangedType);
						}
					}

					if (ebookImageAttrib is EBookSlideImageAttrib slideAttrib)
					{
						if (-1 < slideAttrib.LeftImageKey)
						{
							mapAtom[slideAttrib.LeftImageKey] = slideAttrib.LeftImageKey;
						}

						if (-1 < slideAttrib.RightImageKey)
						{
							mapAtom[slideAttrib.RightImageKey] = slideAttrib.RightImageKey;
						}
					}
				}

				if (pAtom is TreeAtom) // 트리아톰의 경우 확장기호 2개 아이콘 1개 총3개의 이미지를 관리해줘야함
				{
					TreeAttrib pTreeAttrib = pAtom.GetAttrib () as TreeAttrib;
					if (false == string.IsNullOrEmpty (pTreeAttrib.ExpandImagePath) && false == string.IsNullOrEmpty (pTreeAttrib.CollaspeImagePath))
					{
						mapAtom[pTreeAttrib.ImageKey] = pTreeAttrib.ImageKey;
						mapAtom[pTreeAttrib.ExpanderImageKey] = pTreeAttrib.ExpanderImageKey;
						mapAtom[pTreeAttrib.CollaspeImageKey] = pTreeAttrib.CollaspeImageKey;
					}
				}

				if (pAtom is TabViewAtom) // 탭뷰 배경이미지
				{
					TabViewAttrib pTabAttrib = pAtom.GetAttrib () as TabViewAttrib;
					foreach (TabViewItem tabViewItem in pTabAttrib.TabViewItem)
					{
						if (0 <= tabViewItem.ImageKey)
						{
							if (false == mapAtom.ContainsKey (tabViewItem.ImageKey))
							{
								mapAtom[tabViewItem.ImageKey] = tabViewItem.ImageKey;
							}
						}
					}
				}

				if (pAtom is CheckAtom)
				{
					CheckAttrib pCheckAttrib = pAtom.GetAttrib () as CheckAttrib;

					if (-1 < pCheckAttrib.CheckImageKey)
					{
						mapAtom[pCheckAttrib.CheckImageKey] = pCheckAttrib.CheckImageKey;
					}

					if (-1 < pCheckAttrib.UncheckImageKey)
					{
						mapAtom[pCheckAttrib.UncheckImageKey] = pCheckAttrib.UncheckImageKey;
					}
				}

				if (pAtom is RadioAtom)
				{
					RadioAttrib pRadioAttrib = pAtom.GetAttrib () as RadioAttrib;

					if (-1 < pRadioAttrib.CheckImageKey)
					{
						mapAtom[pRadioAttrib.CheckImageKey] = pRadioAttrib.CheckImageKey;
					}

					if (-1 < pRadioAttrib.UncheckImageKey)
					{
						mapAtom[pRadioAttrib.UncheckImageKey] = pRadioAttrib.UncheckImageKey;
					}
				}

				if (pAtom is RatingBarAtom)
				{
					RatingBarAttrib ratingBarAttrib = pAtom.GetAttrib () as RatingBarAttrib;

					if (-1 < ratingBarAttrib.CheckImageKey)
					{
						mapAtom[ratingBarAttrib.CheckImageKey] = ratingBarAttrib.CheckImageKey;
					}

					if (-1 < ratingBarAttrib.UncheckImageKey)
					{
						mapAtom[ratingBarAttrib.UncheckImageKey] = ratingBarAttrib.UncheckImageKey;
					}
				}

				if (pAtom is ExtensionScrollAtom)
				{
					ExtensionScrollAttrib scrollAttrib = pAtom.GetAttrib () as ExtensionScrollAttrib;

					if (-1 < scrollAttrib.CheckImageKey)
					{
						mapAtom[scrollAttrib.CheckImageKey] = scrollAttrib.CheckImageKey;
					}

					if (-1 < scrollAttrib.UncheckImageKey)
					{
						mapAtom[scrollAttrib.UncheckImageKey] = scrollAttrib.UncheckImageKey;
					}
				}

				if (pAtom is AnalogClockAtom)
				{
					AnalogClockAttrib clockAttrib = pAtom.GetAttrib () as AnalogClockAttrib;
					if (-1 < clockAttrib.ClockImageKey)
					{
						mapAtom[clockAttrib.ClockImageKey] = clockAttrib.ClockImageKey;
					}
				}

				if (pAtom is RadialMenuAtom)
				{
					RadialMenuAttrib radialMenuAttrib = pAtom.GetAttrib () as RadialMenuAttrib;

					foreach (RadialMenuItemInfo menuItem in radialMenuAttrib.RadialMenuInfo.MenuItemInfoList)
					{
						if (-1 < menuItem.ContentImageKey)
						{
							mapAtom[menuItem.ContentImageKey] = menuItem.ContentImageKey;
						}
					}

					if (-1 < radialMenuAttrib.RadialMenuInfo.CentralInfo.CentralImageKey)
					{
						mapAtom[radialMenuAttrib.RadialMenuInfo.CentralInfo.CentralImageKey] = radialMenuAttrib.RadialMenuInfo.CentralInfo.CentralImageKey;
					}
				}

				if (pAtom is InputSpinnerAtom)
				{
					InputSpinnerAttrib inputSpinnerAttrib = pAtom.GetAttrib () as InputSpinnerAttrib;

					if (-1 < inputSpinnerAttrib.LeftImageKey)
					{
						mapAtom[inputSpinnerAttrib.LeftImageKey] = inputSpinnerAttrib.LeftImageKey;
					}

					if (-1 < inputSpinnerAttrib.RightImageKey)
					{
						mapAtom[inputSpinnerAttrib.RightImageKey] = inputSpinnerAttrib.RightImageKey;
					}
				}

				if (pAtom is ActionAtom)
				{
					ActionAttrib actionAttrib = pAtom.GetAttrib () as ActionAttrib;
					if (-1 < actionAttrib.IconImageKey)
					{
						mapAtom[actionAttrib.IconImageKey] = actionAttrib.IconImageKey;
					}
				}
			}


			DMTView view = this.GetParentView () as DMTView;
			CViewAttrib pViewAtrrib = view.GetViewAttrib () as CViewAttrib;

			int nViewImageKey = pViewAtrrib.ImageKey;
			if (0 <= nViewImageKey)
			{
				if (false == mapAtom.ContainsKey (nViewImageKey))
				{
					mapAtom[nViewImageKey] = nViewImageKey;
				}
			}


			CMapWordToObX pMap = m_pGDITable.GetGDIMap ((int)OBJECTKEY_TYPE._IMAGE);
			foreach (int nKey in pMap.Keys)
			{
				if (0 <= nKey && false == mapAtom.ContainsKey (nKey))
				{
					CObjectImage pObjectImage = pMap[nKey] as CObjectImage;
					if (null != pObjectImage && null != pObjectImage.SelectImage)
					{
						pObjectImage.ImagePath = "";
						//80 필요 없을듯 
						//pObjectImage.SelectImage.Dispose();
						pObjectImage.Dispose ();
					}
				}
			}

			mapAtom.Clear ();
		}

		#endregion//저장관련

		#region |  ----- F10 ERD 자동생성 -----  |

		#region |  ----- F10 ERD 자동생성 : 엔진 80 -----  |

		public void OnSendToDatabaseTableQuery80 (bool bDetailMoode)
		{
			if (false == PQAppBase.ConnectStatus)
			{
				_Message80.Show (LC.GS ("SmartOffice_MainWindow_7"));
				return;
			}

			string strSQL = "";

			//2020-05-08 kys 달력아톰 DB자동생성 논리 보강
			List<Atom> pAtomList = GetAllAtomCores ();

			foreach (Atom pAtomCore in pAtomList)
			{
				if (pAtomCore is CalendarAtom)
				{
					if (true == CreateTableHelper.Instance.CreateTable (pAtomCore as CalendarAtom))
					{
						_Message80.Show (LC.GS ("TopProcess_DMTDoc_2020_05_1")); // 캘린더관리아톰의 일정 테이블이 정상적으로 동작합니다.
						strSQL = null;
					}
					else
					{
						_Message80.Show (LC.GS ("TopProcess_DMTDoc_2020_05_2")); //캘린더관리아톰의 일정 테이블을 생성하는중 오류가 발생했습니다.
					}
				}
				else if (pAtomCore is SignalAtom)
				{
					if (true == CreateTableHelper.Instance.CreateTable (pAtomCore as SignalAtom))
					{
						strSQL = null;
					}
					else
					{
					}
				}
				else if (pAtomCore is ScheduleAtom)
				{
					if (true == CreateTableHelper.Instance.CreateTable (pAtomCore as ScheduleAtom))
					{
						strSQL = null;
					}
					else
					{
					}
				}
			}

			strSQL = this.GetExecuteableQuery ();
			if (false == string.IsNullOrEmpty (strSQL))
			{
				LogManager.WriteLog (LogType.Information, "Key.F10 OnSendToDatabaseTableQuery80");
				OnSendToDatabaseTableQuery80 (true, bDetailMoode);
			}

			if (null != strSQL && 0 == strSQL.Length)
			{
				_Message80.Show (LC.GS ("TopProcess_DMTDoc_51")); //데이터베이스 테이블 생성 및 변경할 내용이 없습니다.
			}
		}

		public void OnSendToDatabaseTableQuery80 (bool bShowDialog, bool bDetailMode)
		{
			// null != m_pErdGenManager || 
			if (null != m_pErdGenManager80)
			{
				TopDBManager80.ErdEventArgs msg = new TopDBManager80.ErdEventArgs ();
				MakeERDInfomation80 (msg);

				Window mainWindow = Application.Current.MainWindow;

				TopDBManager80.ERDGenManager.ERDSendQueryWindow sendQueryWindow = new TopDBManager80.ERDGenManager.ERDSendQueryWindow ();

				sendQueryWindow.Owner = mainWindow;
				sendQueryWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				sendQueryWindow.Icon = mainWindow.Icon;
				sendQueryWindow.SetErdInformation (msg.ServerConnectionString, msg.FormInformationMap);
				sendQueryWindow.IsDetailMode = bDetailMode;

				if (true == bShowDialog)
				{
					if (true == sendQueryWindow.ShowDialog ())
					{
						sendQueryWindow.SendToQuery ();
					}
				}
				else
				{
					sendQueryWindow.SendToQuery ();
				}
			}
		}
		#endregion//F10 ERD 자동생성 : 엔진 80

		public string GetExecuteableQuery ()
		{
			TopDBManager80.ErdEventArgs msg = new TopDBManager80.ErdEventArgs ();

			bool bMake = MakeERDInfomation80 (msg);

			if (false != bMake)
			{
				if (null == m_pErdGenManager80)
				{
					m_pErdGenManager80 = new TopDBManager80.ERDGenManager.ERDGenManager80 ();
				}

				m_pErdGenManager80.SetServerFormInfo (msg.ServerConnectionString, msg.FormInformationMap);
				m_pErdGenManager80.ShowErd ();

				m_pErdGenManager80.DrawDMTInformation ();

				return m_pErdGenManager80.GetExecuteableQuery ();
			}

			return null;
		}

		public CObArray GetFormInformation ()
		{
			CObArray poaFormInfo = new CObArray ();

			COrderAtom pOrderAtom = new COrderAtom ();
			COrderAtom pOrderAtomTemp = new COrderAtom ();
			Hashtable pDBMap = new Hashtable ();

			List<Atom> lstAllAtomCore = GetAllAtomCoresTabOrdered ();

			//80 모든 아톰이 리턴되는지 확인 체크 
			for (int i = 0; i < lstAllAtomCore.Count; i++)
			{
				Atom pAtom = this.GetOrderAtom ()[i] as Atom;
				if (null == pAtom)
				{
					continue;
				}

				pOrderAtomTemp.SetAtGrow (pAtom.Attrib.AtomAbsoluteTabIndex, pAtom);
			}

			for (int nIndex = 0; nIndex < pOrderAtomTemp.Count; nIndex++)
			{
				Atom pAtom = pOrderAtomTemp[nIndex] as Atom;
				if (null == pAtom)
					continue;

				ScrollAtom pScroll = pAtom as ScrollAtom;
				if (null != pScroll)
					pOrderAtom.AddRange (pScroll.ERDRelationList ());

				if (-1 == pOrderAtom.IndexOf (pAtom))
					pOrderAtom.Add (pAtom);
			}

			Hashtable pErdRelationMap = GetErdRelationMap ();

			for (int i = 0; i < pOrderAtom.Count; i++)
			{
				Atom pInputAtom = pOrderAtom[i] as Atom;
				if (null == pInputAtom)
					continue;

				Attrib pInputAttrib = pInputAtom.Attrib;
				if (null == pInputAttrib)
					continue;

				if (pInputAttrib is MasterInputAttrib)
				{
					int nDBIndex = pInputAtom.GetDBIndex (false);
					string strTableName = pInputAttrib.GetTableName (false);

					if (0 != strTableName.Length)
					{
						if (false == pDBMap.ContainsKey (strTableName))
						{
							pDBMap.Add (strTableName, nDBIndex);
						}
					}

					MakeDBFieldAttribInfo (pInputAtom, poaFormInfo, pOrderAtom, pErdRelationMap);
				}

				else if (pInputAttrib is PopupAttrib)
				{
					PopupAttrib pPopAttrib = pInputAttrib as PopupAttrib;
					if (null != pPopAttrib)
					{
						foreach (string strFieldInfo in pPopAttrib.OrgFieldInfo)
						{
							string[] saColumn = strFieldInfo.Split ("$@".ToCharArray ());
							string strTableName = saColumn.Length > 0 ? saColumn[0].Trim () : string.Empty;
							string strFieldName = saColumn.Length > 1 ? saColumn[1].Trim () : string.Empty;
							string strFieldType = saColumn[2];
							string strFieldLength = "text" == strFieldType ? string.Empty : saColumn[3];

							MakeDBFieldPopAttribInfo (strTableName, strFieldName, strFieldType, strFieldLength, poaFormInfo);

						}
					}
				}
			}

			return poaFormInfo;
		}

		#endregion//F10 ERD 자동생성

		#region |  ----- DB 처리객체 -----  |

		public void OnUserSql (object MdiParentForm, bool isNew)
		{
			ShowDBMgrFrame80 (MdiParentForm, "", false, -1, -1);
		}

		public override void SaveDBManager (string strName)
		{
			if (null != m_DBManagerFrame)
			{
				int totalCount = m_DBManagerFrame.Count;

				for (int index = 0; index < m_DBManagerFrame.Count; index++)
				{
					CDocQueryMgr docQuery = m_DBManagerFrame[index] as CDocQueryMgr;

					if (null != docQuery && strName == docQuery.Text) //현재 선택된 DB처리객체만 저장
					{
						CDBMaster dbMaster = this.GetDBMaster ();

						if (TopDBManagerLibrary.SqlMode._None != docQuery.SqlMode && -1 < docQuery.SQLIndex)
						{
							dbMaster.UpdateSQLManager (docQuery, (int)SQLQUERY_TYPE._USERSQL_);
						}
						else
						{
							int nSQLIndex = dbMaster.AddSQLManager (docQuery, (int)SQLQUERY_TYPE._USERSQL_);

							if (-1 == nSQLIndex)
								continue;

							docQuery.OuterSQL = true;
							docQuery.SQLIndex = nSQLIndex;
						}
					}
				}
			}

			AutoRefreshEditWindow ();
		}

		public void DBManagerFrame_Closed (object sender, EventArgs e)
		{
			//2021-05-12 kys DB처리객체가 종료되었을때가 아닌 저장버튼을 눌렀을때로 저장 시점 변경
			//if (null != m_DBManagerFrame)
			//{
			//    int totalCount = m_DBManagerFrame.Count;

			//    for (int index = 0; index < totalCount; index++)
			//    {
			//        if (null == m_DBManagerFrame[index] || false == (m_DBManagerFrame[index] is CDocQueryMgr) ||
			//            TopDBManagerLibrary.SqlMode._None == m_DBManagerFrame[index].SqlMode ||
			//            -1 != m_DBManagerFrame[index].SQLIndex)
			//            continue;

			//        int nSQLIndex = this.GetDBMaster().AddSQLManager((CDocQueryMgr)m_DBManagerFrame[index], (int)SQLQUERY_TYPE._USERSQL_);

			//        if (-1 == nSQLIndex)
			//            continue;

			//        ((CDocQueryMgr)m_DBManagerFrame[index]).OuterSQL = true;
			//        ((CDocQueryMgr)m_DBManagerFrame[index]).SQLIndex = nSQLIndex;

			//    }

			//    if (null != m_DBManagerFrame.Owner)
			//    {
			//        m_DBManagerFrame.Owner.Focus();
			//    }
			//    m_DBManagerFrame.Closed -= DBManagerFrame_Closed;
			//    m_DBManagerFrame = null;
			//}

			m_DBManagerFrame = null;

			if (null != m_DBMgrCloseEvent)
			{
				m_DBMgrCloseEvent (sender, e);
				m_DBMgrCloseEvent = null;
			}
		}

		/// <summary>
		/// 구조화데이터 처리..
		/// </summary>
		/// <param name="pMdiParent"></param>
		/// <param name="isNew"></param>
		public MDIForm OnStructData (object pMdiParent, int nServiceType)
		{
			ShowStructDataMgrFrame (pMdiParent, nServiceType);
			return null;
		}

		#endregion//DB 처리객체

		#region |  ----- 진행관리자 -----  |

		public void OnEditModeProcessManager ()
		{
			if (null != this.FlowMapWindow && true == FlowMapWindow.RunModeHide)
			{
				FlowMapWindow.Show ();
				FlowMapWindow.Focus ();
				FlowMapWindow.RunModeHide = false;
			}

			if (null != m_FlowManagerWindow && true == m_FlowManagerWindow.RunModeHide)
			{
				m_FlowManagerWindow.Show ();
				m_FlowManagerWindow.RunModeHide = false;
			}
		}

		public void OnRunModeHideProcessManager ()
		{
			if (null != this.FlowMapWindow && true == this.FlowMapWindow.IsVisible)
			{
				FlowMapWindow.RunModeHide = true;
				FlowMapWindow.Hide ();
			}

			if (null != m_FlowManagerWindow && true == m_FlowManagerWindow.IsVisible)
			{
				m_FlowManagerWindow.RunModeHide = true;
				m_FlowManagerWindow.Hide ();
			}
		}

		#endregion

		#region |  ----- 아톰편집도우미 -----  |

		public void OnEditModeAtomEditManager ()
		{
			if (null != this.AtomEditMapWindow && true == AtomEditMapWindow.RunModeHide)
			{
				AtomEditMapWindow.Show ();
				AtomEditMapWindow.Focus ();
				AtomEditMapWindow.RunModeHide = false;

				AtomEditMapWindow.RefreshVisibleAtomProperty ();
			}
		}

		public void OnRunModeHideAtomEditManager ()
		{
			if (null != this.AtomEditMapWindow && true == this.AtomEditMapWindow.IsVisible)
			{
				AtomEditMapWindow.RunModeHide = true;
				AtomEditMapWindow.Hide ();
			}
		}

		#endregion

		#region | 메뉴영역에 걸쳐져 있는 아톰 찾기 |

		public void CheckAtomPosition ()
		{
			DMTView currentView = GetParentView () as DMTView;
			if (null != currentView)
			{
				DMTFrame currentFrame = currentView.GetFrame () as DMTFrame;

				if (null != currentFrame)
				{
					List<Atom> atomCoreList = GetViewAtomCores ();
					List<Rect> atomRectList = new List<Rect> ();

					foreach (Atom atomCore in atomCoreList)
					{
						if (0 <= atomCore.Attrib.AtomWidth && 0 <= atomCore.Attrib.AtomHeight)
						{
							atomRectList.Add (new Rect (atomCore.Attrib.AtomX, atomCore.Attrib.AtomY, atomCore.Attrib.AtomWidth, atomCore.Attrib.AtomHeight));
						}
					}

					if (null != currentFrame.CurrentPhoneScreenView)
					{
						currentFrame.CurrentPhoneScreenView.AtomMoveEnd (atomRectList);
					}
				}
			}
		}

		#endregion

		#region |  ----- 업무규칙 -----  |

		public void AssignScriptIndex (AtomBase pAtom)
		{
			if (pAtom == null)
				return;

			Atom pAtomCore = pAtom.AtomCore;
			if (null != pAtomCore)
			{
				AssignScriptIndex (pAtomCore);
			}
		}

		/// <summary>
		/// .net
		/// </summary>
		/// <param name="pAtomCore"></param>
		public void AssignScriptIndex (Atom pAtomCore)
		{
			if (null == pAtomCore)
			{
				return;
			}

			if (0 == m_stnMaxIndex)
			{
				foreach (Atom _pAtom in this.GetAllAtomCores ())
				{
					if (null != _pAtom)
					{
						SetMaxScriptIndex (_pAtom.GetScriptIndex ());
					}
				}
			}
			pAtomCore.SetScriptIndex ((ushort)++m_stnMaxIndex);
		}

		public bool SetMaxScriptIndex (ushort stnMaxIndex)
		{
			bool bRight = false;

			if (0 <= stnMaxIndex && 65535 > stnMaxIndex)
			{
				bRight = true;

				if (m_stnMaxIndex < stnMaxIndex)
					m_stnMaxIndex = stnMaxIndex;
			}
			else
			{
				bRight = false;
				_Message80.Show (LC.GS ("TopProcess_DMTDoc_12"));
			}

			return bRight;
		}

		#endregion//업무규칙

		#region |  ----- 디자인 도우미 -----  |

		public void ApplyDesignHelperEvent (DictionaryEntry DictionaryParameter, CViewAttrib ViewAttrib)
		{
			int nDesignViewType = Convert.ToInt32 (DictionaryParameter.Key);
			ImageSpecification ApplyImageSpec = DictionaryParameter.Value as ImageSpecification;

			CFrameAttrib FrameAttrib = GetFrameAttrib ();

			if (null != ApplyImageSpec)
			{
				switch (nDesignViewType)
				{
					//배경이미지
					case 1:
						{
							if (null != OnNotifyViewPropertyChangedEvent)
							{
								//2021-02-02 디자인 도우미로 배경이미지 설정시 @Path:\\Images\\ 경로에 이미지 복사하고 해당 이미지 참조할 수 있도록 논리 보강
								double dImageWidth = ApplyImageSpec.HorizontalResolution;
								double dImageHeight = ApplyImageSpec.VerticalResolution;
								System.Drawing.Color BrightnessColor = ApplyImageSpec.BrightnessColor;
								string strFilePath = ApplyImageSpec.FilePath;
								string strImagePath = "";

								if (BrightnessColor.Name != "Transparent")
								{
									BitmapImage PreviewImage = GetDecoImageDesign (strFilePath, BrightnessColor);
									strImagePath = SaveDesignImage (strFilePath, null, BrightnessColor);
								}
								else
								{
									strImagePath = SaveDesignImage (strFilePath, null);
								}

								ViewAttrib.BrightnessColor = System.Drawing.Color.Transparent;
								ViewAttrib.ImagePath = strImagePath;
								ViewAttrib.IsBackImg = true;

								SetBackImageGDIImage (ViewAttrib);
								OnNotifyViewPropertyChangedEvent ();
							}

							break;
						}

					case 2:
					case 3:
					case 4:
					case 5:
						{
							ApplyDesignHelperImage (nDesignViewType, ApplyImageSpec);
							break;
						}

					default: break;

				}
			}
		}

		#endregion//디자인 도우미

		#region | 액션관리자 |

		private void ActionManagerEditorWindow_Closed (object sender, EventArgs e)
		{
			ActionManagerEditorWindow = null;
		}

		private void ActionManagerEditorWindow_OnSaveActionManager (object objValue)
		{
			string strFilePath = this.FilePath;

			if (true == string.IsNullOrEmpty (strFilePath))
			{
				//파일 경로가 없는경우 모델명으로 대체
				strFilePath = this.ExeFormName;
			}

			this.ActionManager.Save (strFilePath);
		}

		private void ActionManagerEditorWindow_OnCompleteCompile (object objValue)
		{
			DMTView CurrentView = GetParentView () as DMTView;
			ActionManager actionManager = objValue as ActionManager;

			if (null != CurrentView && null != actionManager)
			{
				actionManager.ToClone (this.ActionManager);

				List<CharacterManager> characterList = this.ActionManager.CharacterList;

				int nCreateNum = 0;
				foreach (CharacterManager character in characterList)
				{
					string strName = character.Name;

					ActionCharacterofAtom atomBase = this.ActionManager.GetTargetAtom (strName);
					if (null == atomBase)
					{
						ActionCharacterControl characterControl = character.CharacterControl;
						atomBase = CurrentView.MakeAtomAsCommand (AtomType.ActionCharacter, 0, 0, new Size (150, 150), null) as ActionCharacterofAtom;
						nCreateNum++;
						if (null != atomBase)
						{
							atomBase.SetCharacterControl (characterControl, character.Name, character.FilePath);
							this.ActionManager.SetTargetAtom (strName, atomBase);
						}
					}
				}
			}
		}

		#endregion

		#region | AI+ 어댑터 |

		private void AIAdaptorMainWindow_Closed (object sender, EventArgs e)
		{
			AIAdaptorMainWindow = null;
		}

		#endregion

		#region |  ----- 실행관리자 관련 -----  |

		/// <summary>
		/// 2008.01.16 황성민 
		/// 사용중인 실행질의문 창이 존재하면 실행질의문 창을 종료합니다.
		/// </summary>
		public void DropSearchViewer ()
		{
			if (null != pUserQueryDialog)
			{
				pUserQueryDialog.Close ();
				pUserQueryDialog = null;
			}
		}

		#endregion//실행관리자 관련

		/// <summary>
		/// 환경설정에서 ERD 정보 설정전달
		/// </summary>
		public override void ResetFrameAttrib ()
		{
			CDMTFrameAttrib pFrameAttrib = GetFrameAttrib () as CDMTFrameAttrib;
			if (null != pFrameAttrib)
			{
				pFrameAttrib.ResetFrameAttrib ();
			}
		}

		public bool CheckDBMgrFrame ()
		{
			if (null != m_DBManagerFrame && Visibility.Visible == m_DBManagerFrame.Visibility)
			{
				m_DBManagerFrame.ToFront ();
				_Message80.Show (LC.GS ("TopProcess_DMTDoc_18"));
				return false;
			}

			m_DBManagerFrame = null;
			return true;
		}

		/// <summary>
		/// 프로그램 진행관리자를 닫습니다.
		/// </summary>
		/// 2007. 5. 8 황성민
		public void DropPMEventViewer ()
		{
			if (null != FlowMapWindow)
			{
				FlowMapWindow.Close ();
				FlowMapWindow.CloseView ();
				FlowMapWindow = null;
			}

			if (null != FlowManagerWindow)
			{
				FlowManagerWindow.Close ();
				FlowManagerWindow = null;
			}
		}

		/// <summary>
		/// 아톰편집도우미를 닫습니다.
		/// </summary>
		public void DropAtomEditManager ()
		{
			if (null != AtomEditMapWindow)
			{
				AtomEditMapWindow.Close ();
				AtomEditMapWindow = null;
			}
		}

		public void DropStructDataMgrFrame ()
		{
			if (null != m_pStructDataMgrFrame)
			{
				m_pStructDataMgrFrame.Close ();
				m_pStructDataMgrFrame = null;
			}
		}

		public TopEdit80.CScrFrame GetScriptWindow ()
		{
			return m_pScriptWindow;
		}

		/// <summary>
		/// 선택아톰(단일/복수) 잠금 / 해제
		/// </summary>
		public void OnToolLock ()
		{
			List<AtomBase> lstCurrentSelectedAtoms = new List<AtomBase> ();
			lstCurrentSelectedAtoms.AddRange (GetCurrentSelectedAtomsInLightDMTView ());
			lstCurrentSelectedAtoms.AddRange (GetCurrentSelectedAtomsInTabView ());
			lstCurrentSelectedAtoms.AddRange (GetCurrentSelectedAtomsInScroll ());

			//아톰 잠금 / 해제 
			if (lstCurrentSelectedAtoms.Any ())
			{
				foreach (AtomBase atom in lstCurrentSelectedAtoms)
				{
					atom.OnToolLockOrUnLock ();
				}
			}
			//모델 잠금 / 해제
			else
			{
				DMTView currentView = GetParentView () as DMTView;
				currentView.IsLockAllMode = !currentView.IsLockAllMode;
			}
		}

		/// <summary>
		/// 모델내의 모든 아톰 잠금 
		/// </summary>
		public void OnToolLockAllAtoms ()
		{
			List<Atom> lstViewAtomCores = GetAllAtomCores ();

			foreach (Atom atomCore in lstViewAtomCores)
			{
				AtomBase atom = atomCore.GetOfAtom () as AtomBase;
				atom.OnToolLock ();
			}
		}

		/// <summary>
		/// 모델내의 모든 아톰 잠금 해제
		/// </summary>
		public void OnToolUnLockAllAtoms ()
		{
			List<Atom> lstViewAtomCores = GetAllAtomCores ();

			foreach (Atom atomCore in lstViewAtomCores)
			{
				AtomBase atom = atomCore.GetOfAtom () as AtomBase;
				atom.OnToolUnLock ();
			}
		}

		/// <summary>
		/// 스포이드모드, 탭인덱스 변경모드로 갈때
		/// 아톰을 모두 잠그면서 이전 잠금상태를 보존
		/// </summary>
		public void OnToolLockAndBackupAllAtoms ()
		{
			//현재 백업된 상태가 있으므로 리턴시킴
			if (0 < LockBackupDic.Count)
			{
				return;
			}

			List<Atom> lstViewAtomCores = GetAllAtomCores ();

			foreach (Atom atomCore in lstViewAtomCores)
			{
				LockBackupDic.Add (atomCore, atomCore.Attrib.IsLocked);
				AtomBase atom = atomCore.GetOfAtom () as AtomBase;
				atom.OnToolLock ();
			}
		}

		public void DropActionManager ()
		{
			if (null != ActionManagerEditorWindow)
			{
				ActionManagerEditorWindow.Close ();
				ActionManagerEditorWindow = null;
			}
		}

		public void DropToolAnimation ()
		{
			if (null != AnimationManager)
			{
				AnimationManager.DisposeAnimationAll ();
				EmphasisAnimationManager.DisposeAnimationAll ();
			}

			if (null != this.AnimationWindow)
			{
				AnimationWindow.AnimationLayer = null;

				AnimationWindow.OnGetAllAtomList -= AnimationWindow_OnGetAllAtomList;
				AnimationWindow.OnGetAnimationManager -= AnimationWindow_OnGetAnimationManager;
				AnimationWindow.OnMakeAnimationGroup -= AnimationWindow_OnMakeAnimationGroup;
				AnimationWindow.OnDeleteAnimationGroup -= AnimationWindow_OnDeleteAnimationGroup;

				AnimationWindow.Close ();
				AnimationWindow.CloseView ();

				AnimationWindow = null;
			}
		}

		public void DropAIAdaptorWindow ()
		{
			if (null != m_AIAdaptorWindow)
			{
				m_AIAdaptorWindow.Close ();
				m_AIAdaptorWindow = null;
			}
		}

		public override bool OnCloseDocument ()
		{
			base.OnCloseDocument ();

			ExitExecute ();

			//진행관리자 종료
			DropPMEventViewer ();

			//아톰 종료
			DropAtom ();

			//MotionManager 종료
			DropMotionManager ();

			//아톰편집기 도우미 종료
			DropAtomEditManager ();

			//OpenAPI 종료
			DropStructDataMgrFrame ();

			//인공지능 어댑터 종료
			DropAIAdaptorWindow ();

			//실행질의의문창 종료
			DropSearchViewer ();

			//업무규칙 종료
			DropScriptEdit80 (true);
			DropScriptEdit (true);

			//애니메이션 종료
			DropToolAnimation ();

			//액션관리자 종료
			DropActionManager ();

			//파일 스트림 종료
			CloseCurrentFileStream ();

			DropGDIManager ();

			return true;
		}

		private void DropAtom ()
		{
			DMTView currentView = GetParentView () as DMTView;

			if (null != currentView)
			{
				currentView.CloseAllAtoms ();
			}
		}

		private void DropMotionManager ()
		{
			DMTView currentView = GetParentView () as DMTView;
			if (null != currentView)
			{

				DMTFrame frame = currentView.GetFrame () as DMTFrame;

				if (null != frame)
				{
					frame.MotionManager.CloseFrame ();
				}
			}
		}

		/// <summary>
		/// 애니메이션 관리자 숨김
		/// </summary>
		public void OnToolHideAnimation ()
		{
			if (null != this.AnimationWindow)
			{
				AnimationWindow.Hide ();
			}
		}

		public void OnEditModeActionManager ()
		{
			if (null != ActionManagerEditorWindow && true == ActionManagerEditorWindow.RunModeHide)
			{
				this.ActionManagerEditorWindow.Show ();
			}
		}

		public void OnEditModeAnimation ()
		{
			if (null != this.AnimationWindow && true == AnimationWindow.RunModeHide)
			{
				AnimationWindow.Show ();
				AnimationWindow.Focus ();
				AnimationWindow.RunModeHide = false;
				DMTView currentView = GetParentView () as DMTView;
				currentView.AnimationLayer.Visibility = Visibility.Visible;
			}
		}

		public void OnRunModeHideAnimation ()
		{
			if (null != this.AnimationWindow && true == this.AnimationWindow.IsVisible)
			{
				AnimationWindow.RunModeHide = true;
				AnimationWindow.Hide ();
			}

			DMTView currentView = GetParentView () as DMTView;

			if (null != currentView?.AnimationLayer)
			{
				currentView.AnimationLayer.Visibility = Visibility.Collapsed;
			}
		}

		/// <summary>
		/// 아톰의 잠금상태를 롤백
		/// </summary>
		public void OnToolRollBackLockStateAllAtoms ()
		{
			if (null == LockBackupDic || 0 == LockBackupDic.Count)
			{
				return;
			}

			foreach (var lockInfo in LockBackupDic)
			{
				AtomBase atom = ((Atom)lockInfo.Key).GetOfAtom () as AtomBase;

				if (true == lockInfo.Value)
				{
					atom.OnToolLock ();
				}
				else
				{
					atom.OnToolUnLock ();
				}
			}

			LockBackupDic.Clear ();
		}

		/// <summary>
		/// Erd저장 관련 함수
		/// </summary>
		public void AutoSaveErdDoc ()
		{
			CDMTFrameAttrib pFrameAttrib = GetFrameAttrib () as CDMTFrameAttrib;

			if (null != pFrameAttrib)
			{
				bool bAutoSave = pFrameAttrib.AutoERDSave;
				bool bOverWrite = pFrameAttrib.OldErdInfoOverWrite;

				string strDomain = AppDomain.CurrentDomain.ToString ();
				strDomain = strDomain.ToUpper ();

				if (false != bAutoSave)
				{
					if (false != bOverWrite && -1 < strDomain.IndexOf ("PQBLD.EXE"))
					{
						// 2002.07.20 LHS (ERD저장)
						string strMessage = LC.GS ("TopProcess_DMTDoc_24");

						//MT04_ERD_INFO_SAVE
						if (MessageBoxResult.No == _Message80.Show (strMessage, LC.GS ("TopProcess_DMTDoc_13"), MessageBoxButton.YesNo, MessageBoxImage.Question))
							return;
					}

					AutoSetErdInfoByTouch ();
				}
			}
		}

		#region |  ----- Page Changed 관련 -----  |
		/// <summary>
		/// 페이지가 변경되면 진행관리자의 내용을 변경된 페이지에 맞춰 세팅 
		/// </summary>
		/// <param name="dmtDoc"></param>
		public void OnNotifyPageChangeTo_ProcessManager ()
		{
			if (null != FlowMapWindow)
			{
				FlowMapWindow.ChangeSourceDocument (this);
			}

			if (null != FlowManagerWindow)
			{
				FlowManagerViewModel flowManagerViewModel = new FlowManagerViewModel (this);
				FlowManagerWindow.DataContext = flowManagerViewModel;
			}
		}

		/// <summary>
		/// 페이지가 변경되면 아톰편집도우미 내용을 변경된 페이지에 맞춰 세팅 
		/// </summary>
		/// <param name="dmtDoc"></param>
		public void OnNotifyPageChangeTo_AtomEditManager ()
		{
			if (null != AtomEditMapWindow)
			{
				AtomEditMapWindow.ChangeSourceDocument (this);
			}
		}

		public void OnNotifyPageChange_DetachStructDataManager ()
		{
			if (null != m_pStructDataMgrFrame)
			{
				m_pStructDataMgrFrame.Closed -= StructDataMgrFrame_Closed;
				m_pStructDataMgrFrame.OnMakeInputAtom -= StructDataMgrFrame_OnMakeInputAtom;
				m_pStructDataMgrFrame.OnBrowseAtomUpdate -= StructDataMgrFrame_OnBrowseAtomUpdate;
				m_pStructDataMgrFrame = null;
			}
		}

		public void OnNotifyPageChange_AttachStructDataManager (StructDataManagerOwnerFrame structDataManager, int nNewPage)
		{
			if (null != structDataManager)
			{
				StructDataManagerFrame = structDataManager;

				StructDataManagerFrame.Closed += StructDataMgrFrame_Closed;
				StructDataManagerFrame.OnBrowseAtomUpdate += StructDataMgrFrame_OnBrowseAtomUpdate;
				StructDataManagerFrame.OnMakeInputAtom += StructDataMgrFrame_OnMakeInputAtom;

				string strTitle = string.Empty != this.GetFormTitle () ? this.GetFormTitle () : this.GetFormName ();

				if (true == IsEBookDoc)
				{
					StructDataManagerFrame.FormTitle = string.Format (LC.GS ("TopProcess_DMTDoc_43"), strTitle, nNewPage);
				}
				else
				{
					StructDataManagerFrame.FormTitle = strTitle;
				}

				StructDataManagerFrame.Open (m_pStructDataMgr);
				CDocDBManagerList pDBManagerList = new CDocDBManagerList (this);
				StructDataManagerFrame.Open (pDBManagerList);
			}
		}

		public void OnNotifyPageChange_DetachScriptWindow ()
		{
			this.ScriptWindow.Closed -= ScriptWindow_Closed;
			this.ScriptWindow = null;
		}

		/// <summary>
		/// 업무규칙 페이지 열릴때 처리 
		/// </summary>
		/// <param name="scriptWindow"></param>
		/// <param name="nPage"></param>
		public void OnNotifyPageChange_AttachScriptWindow (TopEdit80.CScrFrame scriptWindow, int nPage)
		{
			string strSelAtom = (null != m_pRButtonAtom) ? m_pRButtonAtom.GetProperVar () : string.Empty;


			this.ScriptWindow = scriptWindow;
			this.ScriptWindow.Closed += ScriptWindow_Closed;
			this.ScriptWindow.ShowEditWindow ((ScriptDoc)this, strSelAtom, false);
			this.ScriptWindow.ActivateWindow ();
			//scriptWindow.Show();
			this.ScriptWindow.FORCE_SOURCE_CHANGE = true;
			this.ScriptWindow.OnLoadSourceText ();
			string strTitle = string.Empty != this.GetFormTitle () ? this.GetFormTitle () : this.GetFormName ();

			if (true == IsEBookDoc)
			{
				ScriptWindow.Title = string.Format (LC.GS ("TopProcess_DMTDoc_43"), strTitle, nPage, LC.GS ("TopProcess_DMTDoc_20"));
			}
			else
			{
				ScriptWindow.Title = string.Format ("{0} - {1}", strTitle, LC.GS ("TopProcess_DMTDoc_20"));
			}
		}

		/// <summary>
		/// 디비처리객체 페이지가 닫힐때 처리
		/// </summary>
		public void OnNotifyPageChange_DetachDBManager ()
		{
			if (null == DBManagerFrame)
			{
				return;
			}
			int totalCount = m_DBManagerFrame.Count;

			for (int index = 0; index < totalCount; index++)
			{
				if (null == m_DBManagerFrame[index] || false == (m_DBManagerFrame[index] is CDocQueryMgr) ||
					TopDBManagerLibrary.SqlMode._None == m_DBManagerFrame[index].SqlMode ||
					-1 != m_DBManagerFrame[index].SQLIndex)
					continue;

				int nSQLIndex = this.GetDBMaster ().AddSQLManager ((CDocQueryMgr)m_DBManagerFrame[index], (int)SQLQUERY_TYPE._USERSQL_);

				if (-1 == nSQLIndex)
					continue;

				((CDocQueryMgr)m_DBManagerFrame[index]).OuterSQL = true;
				((CDocQueryMgr)m_DBManagerFrame[index]).SQLIndex = nSQLIndex;

			}

			m_DBManagerFrame.Closed -= DBManagerFrame_Closed;
			m_DBManagerFrame = null;

		}

		/// <summary>
		/// 에딧 모드에서 페이지가 변경되면 DB처리객체의 내용을 변경된 페이지에 맞춰 세팅
		/// </summary>
		/// <param name="dmtDoc"></param>
		public void OnNotifyPageChange_AttachDBManager (DBManagerOwnerFrame dmManager, int nNewPage)
		{
			if (null == dmManager)
			{
				return;
			}

			DBManagerFrame = dmManager;
			DBManagerFrame.Closed += DBManagerFrame_Closed;

			string strTitle = string.Empty != this.GetFormTitle () ? this.GetFormTitle () : this.GetFormName ();

			DBManagerFrame.Title = string.Format (LC.GS ("{0} - {1}"), GetSubWindowTitle (), LC.GS ("TopProcess_DMTDoc_14"));

			CDocDBManagerList pDBManagerList = new CDocDBManagerList (this);
			m_DBManagerFrame.Open (pDBManagerList, this);
		}

		#endregion//Page Changed 관련

		public override object GetThisForm ()
		{
			DMTView currentView = this.GetParentView () as DMTView;
			if (null != currentView)
			{
				DMTFrame pDMTFrame = currentView.GetFrame () as DMTFrame;
				return pDMTFrame;
			}

			return null;
		}

		public Hashtable GetTableAndDataBaseInformation ()
		{
			string strTableName = string.Empty;
			string strDataBaseName = string.Empty;
			int nDBIndex = 0;
			Hashtable htInformation = new Hashtable ();

			for (int i = 0; i < this.GetOrderAtom ().Count; i++)
			{
				Atom pAtom = this.GetOrderAtom ().GetAt (i) as Atom;
				if (null == pAtom)
					continue;

				MasterInputAttrib pInputScndAttrib = pAtom.GetAttrib () as MasterInputAttrib;
				if (null != pInputScndAttrib)
				{
					strTableName = pInputScndAttrib.GetTableName (true);
					nDBIndex = pInputScndAttrib.GetDBIndex ();
					strDataBaseName = pInputScndAttrib.GetDSN (true);
					if (true == string.IsNullOrEmpty (strDataBaseName))
					{
						strDataBaseName = GetDBIndexToDataBaseName (nDBIndex);
					}

					SetTableInformationAtHashTable (strTableName, strDataBaseName, htInformation);

					if (null != pInputScndAttrib.JoinDataManager)
					{
						foreach (JoinData pJoinData in pInputScndAttrib.JoinDataManager.GetJoinData ())
						{
							strTableName = pJoinData.TableName;
							strDataBaseName = pJoinData.DataBaseName;
							if (true == string.IsNullOrEmpty (strDataBaseName))
							{
								strDataBaseName = GetDBIndexToDataBaseName (pJoinData.DBIndex);
							}

							SetTableInformationAtHashTable (strTableName, strDataBaseName, htInformation);
						}
					}
				}

				PopupAttrib pPopAttrib = pAtom.GetAttrib () as PopupAttrib;
				if (null != pPopAttrib && 0 == pPopAttrib.Kind)
				{
					for (int nPopIndex = 0; nPopIndex < pPopAttrib.GetSubFieldCount (); nPopIndex++)
					{
						strTableName = pPopAttrib.GetSubTableName (nPopIndex);
						strDataBaseName = pPopAttrib.GetDSN (true);
						if (true == string.IsNullOrEmpty (strDataBaseName))
						{
							strDataBaseName = GetDBIndexToDataBaseName (pPopAttrib.DBIndex);
						}

						SetTableInformationAtHashTable (strTableName, strDataBaseName, htInformation);
					}
				}

				ComboAttrib pComoAttrib = pAtom.GetAttrib () as ComboAttrib;
				if (null != pComoAttrib && 0 == pComoAttrib.Kind)
				{
					strDataBaseName = pComoAttrib.PopDSN;
					if (true == string.IsNullOrEmpty (strDataBaseName))
					{
						strDataBaseName = GetDBIndexToDataBaseName (pComoAttrib.PopupDBIndex);
					}

					ArrayList saFieldInfo = new ArrayList ();
					pComoAttrib.GetFieldInfo (saFieldInfo);
					if (0 != saFieldInfo.Count)
					{
						for (int nComboIndex = 0; nComboIndex < saFieldInfo.Count; nComboIndex++)
						{
							string strFieldInfo = saFieldInfo[nComboIndex] as string;
							if (false == string.IsNullOrEmpty (strFieldInfo))
							{
								string[] strDBInfos = strFieldInfo.Split (new char[] { '$' });
								strTableName = strDBInfos[0];

								SetTableInformationAtHashTable (strTableName, strDataBaseName, htInformation);
							}
						}
					}
				}

				BrowseAttrib pBrowseAttrib = pAtom.GetAttrib () as BrowseAttrib;
				if (null != pBrowseAttrib)
				{
					foreach (BrowseItem pBrowseAtom in pBrowseAttrib.BrowseItemList)
					{
						strTableName = pBrowseAtom.GetTableName (true);
						strDataBaseName = pBrowseAtom.DSN;
						if (true == string.IsNullOrEmpty (strDataBaseName))
						{
							strDataBaseName = GetDBIndexToDataBaseName (pBrowseAtom.DBIndex);
						}

						SetTableInformationAtHashTable (strTableName, strDataBaseName, htInformation);
					}
				}

				SearchConditionAttrib pSearchAttrib = pAtom.GetAttrib () as SearchConditionAttrib;
				if (null != pSearchAttrib)
				{
					if (null != pSearchAttrib.oaAttrib)
					{
						for (int nIndex = 0; nIndex < pSearchAttrib.oaAttrib.Count; nIndex++)
						{
							PopupAttrib pSubPopAttrib = pSearchAttrib.oaAttrib[nIndex] as PopupAttrib;
							if (null != pSubPopAttrib)
							{
								for (int nPopIndex = 0; nPopIndex < pSubPopAttrib.GetSubFieldCount (); nPopIndex++)
								{
									strTableName = pSubPopAttrib.GetSubTableName (nPopIndex);
									strDataBaseName = pSubPopAttrib.GetDSN (true);
									if (true == string.IsNullOrEmpty (strDataBaseName))
									{
										strDataBaseName = GetDBIndexToDataBaseName (pSubPopAttrib.DBIndex);
									}

									SetTableInformationAtHashTable (strTableName, strDataBaseName, htInformation);
								}
							}
						}
					}
				}

				DBGridExAttrib pDBGridExAttrib = pAtom.GetAttrib () as DBGridExAttrib;
				if (null != pDBGridExAttrib)
				{
					if (null != pDBGridExAttrib.PopAttrib)
					{
						CObArray oaPopAttrib = pDBGridExAttrib.PopAttrib;
						foreach (PopupAttrib pDBGridPopAttrib in oaPopAttrib)
						{
							for (int nPopIndex = 0; nPopIndex < pDBGridPopAttrib.GetSubFieldCount (); nPopIndex++)
							{
								strTableName = pDBGridPopAttrib.GetSubTableName (nPopIndex);
								strDataBaseName = pDBGridPopAttrib.GetDSN (true);
								if (true == string.IsNullOrEmpty (strDataBaseName))
								{
									strDataBaseName = GetDBIndexToDataBaseName (pDBGridPopAttrib.DBIndex);
								}

								SetTableInformationAtHashTable (strTableName, strDataBaseName, htInformation);
							}
						}
					}
				}
			}

			return htInformation;
		}

		public void UpdateDataBaseInformation (Hashtable htDataBaseInformation)
		{
			foreach (string strTableName in htDataBaseInformation.Keys)
			{
				string strDataBaseName = htDataBaseInformation[strTableName] as string;

				if (true == string.IsNullOrEmpty (strTableName) ||
					true == string.IsNullOrEmpty (strDataBaseName))
					continue;

				UpdateDataBaseInformation (strTableName, strDataBaseName);
			}
		}

		/// <summary>
		/// 2020-06-11 kys 새로고침 버튼이 없어도 아톰이 추가된 내용을 바로바로 반영하기 위해서 추가함
		/// </summary>
		public virtual void AutoRefreshEditWindow ()
		{
			//DB처리객체 아톰 리스트 갱신
			RefreshDBManager ();

			//외부기능연계, OpenApi 아톰 리스트 갱신
			RefreshStructDataManager ();

			//애니메이션 리스트 갱신
			RefreshAnimationWindow ();

			//진행관리자 리스트 갱신
			RefreshFlowMap ();

			//아톰편집도우미 리스트 갱신
			RefreshAtomEditMap ();

			//Ai어댑터 아톰 리스트 갱신
			RefreshAIAdaptor ();

			//DB처리객체 갱신
			RefreshDBManagerFrame ();
		}

		public string GetSubWindowTitle ()
		{
			string strTitle = string.Empty != this.GetFormTitle () ? this.GetFormTitle () : this.GetFormName ();
			return strTitle;
		}

		//진행관리자 리스트 갱신
		public override void RefreshFlowMap ()
		{
			if (null != this.FlowMapWindow)
			{
				FlowMapWindow.RefreshList ();
				FlowMapWindow.Title = $"{GetSubWindowTitle ()} - 진행관리자";
			}

			if (null != FlowManagerWindow)
			{
				FlowManagerViewModel viewModel = FlowManagerWindow?.DataContext as FlowManagerViewModel;

				if (null != viewModel)
				{
					viewModel.SetAtomList ();
					viewModel.FormName = GetSubWindowTitle ();
				}
			}
		}

		public void RefreshFlowMapProperty ()
		{
			if (null != this.FlowMapWindow)
			{
				FlowMapWindow.RefreshAtomProperty ();
			}
		}

		public void ReplaceAtomFlowManager (List<Atom> atomList)
		{
			if (null == FlowManagerWindow)
			{
				FlowManagerWindow = new FlowManagerMainWndow ();
				FlowManagerViewModel flowManagerViewModel = new FlowManagerViewModel (this);

				FlowManagerWindow.Owner = Application.Current.MainWindow;
				FlowManagerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				FlowManagerWindow.DataContext = flowManagerViewModel;
			}

			FlowManagerViewModel viewModel = FlowManagerWindow?.DataContext as FlowManagerViewModel;
			viewModel?.SetAtomList (atomList);
		}

		/// <summary>
		/// 아톰편집도우미 아톰 리스트 갱신
		/// </summary>
		public void RefreshAtomEditMap ()
		{
			if (null != this.AtomEditMapWindow)
			{
				AtomEditMapWindow.RefreshList ();
				AtomEditMapWindow.Title = $"{GetSubWindowTitle ()} - 아톰편집 도우미";
			}
		}

		public void ReplaceAtomEditMap (List<Atom> atomList)
		{
			if (null == this.AtomEditMapWindow)
			{
				AtomEditMapWindow = new AtomEditMap (this);
				AtomEditMapWindow.Title = $"{GetSubWindowTitle ()} - 아톰편집 도우미";
			}

			AtomEditMapWindow.ReplaceList (atomList);
		}

		public void RefreshAtomEditProperty ()
		{
			if (null != this.AtomEditMapWindow)
			{
				AtomEditMapWindow.RefreshAtomProperty ();
			}
		}

		public void RefreshDBManager ()
		{
			if (null != m_DBManagerFrame)
			{
				m_DBManagerFrame.DBManagerContainer.RefreshLinkAtomList ();
			}
		}

		public void RefreshStructDataManager ()
		{
			if (null != m_pStructDataMgrFrame)
			{
				m_pStructDataMgrFrame.RefreshAtomList ();
				m_pStructDataMgrFrame.FormTitle = GetSubWindowTitle ();
			}
		}

		/// <summary>
		/// 애니메이션 리스트 까지 정리
		/// </summary>
		public void RefreshAnimationWindow ()
		{
			if (null != AnimationWindow)
			{
				AnimationWindow.RefreshList ();
				AnimationWindow.Title = $"{GetSubWindowTitle ()}";
			}
		}

		public void RefreshAIAdaptor ()
		{
			if (null != this.m_AIAdaptorWindow)
			{
				m_AIAdaptorWindow.RefreshAtomList ();
				m_AIAdaptorWindow.Title = $"{GetSubWindowTitle ()} - AI+ 어댑터";
			}
		}

		public void RefreshDBManagerFrame ()
		{
			if (null != DBManagerFrame)
			{
				DBManagerFrame.Title = string.Format (LC.GS ("{0} - {1}"), GetSubWindowTitle (), LC.GS ("TopProcess_DMTDoc_14"));
			}
		}

		/// <summary>
		/// 학습맵 설정
		/// </summary>
		public void OnShowStudyMapDialog ()
		{
			StudyMapDialog dialog = new StudyMapDialog ();

			dialog.CourseCode = this.PageMetadata.StudyMap.CourseCode;
			dialog.StudyMapCode = this.PageMetadata.StudyMap.StudyMapCode;

			if (false != dialog.ShowDialog ())
			{
				SetFormChange (true);
				this.PageMetadata.StudyMap.CourseCode = dialog.CourseCode;
				this.PageMetadata.StudyMap.StudyMapCode = dialog.StudyMapCode;
			}
		}

		/// <summary>
		/// 디시리얼라이즈 이후 저장 데이터 -> 실행객체 로 변환해주는 함수
		/// </summary>
		public virtual void EndSerializeQuizMaker ()
		{
			if (DocType != DOC_KIND._docQuizMaker && DocType != DOC_KIND._docQuizLayoutMaker)
				return;

			var atomCores = GetAllAtomCores ();
			var metaData = this.PageMetadata;

            //20250225 KH QQM Save&Load 관련 null exception 보완
            if (null == metaData.QuizMetaData)
            {
                metaData.QuizMetaData = new QuizMakerMetaData();
            }

            if (metaData.QuizMetaData.DataList == null)
            {
                metaData.QuizMetaData.DataList = new List<KeyValuePair<object, EBookQuizPropertyNode>>();
            }

            metaData.QuizMetaData.DataList.Clear ();
			metaData.QuizMetaData.AnswerDataList.Clear ();

			foreach (var item in metaData.QuizMetaData.QuizPropertyNodeList)
			{
				var atomCore = atomCores.Find (i => i.GetProperVar () == item.Name);

				if (null != atomCore)
					metaData.QuizMetaData.DataList.Add (new KeyValuePair<object, EBookQuizPropertyNode> (atomCore, item));
			}

			foreach (var node in metaData.QuizMetaData.QuizAnswerNodeList)
			{
				foreach (var item in node.Values)
				{
					var atomCore = atomCores.Find (i => i.GetProperVar () == item.AtomName);

					if (null != atomCore)
						metaData.QuizMetaData.AnswerDataList.Add (new KeyValuePair<object, EBookQuizAnswerValueNode> (atomCore, item));
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void PrepareSerializeQuizMaker ()
		{
			if (DocType != DOC_KIND._docQuizMaker)
				return;

			var metaData = this.PageMetadata;
			metaData.QuizMetaData.QuizAnswerNodeList.Clear ();
			metaData.QuizMetaData.QuizPropertyNodeList.Clear ();

			for (int i = 0; i < metaData.QuizMetaData.DataList.Count; i++)
			{
				if (null == metaData.QuizMetaData.DataList[i].Key)
				{
					metaData.QuizMetaData.DataList.RemoveAt (i);
					i--;
				}
			}

			for (int i = 0; i < metaData.QuizMetaData.AnswerDataList.Count; i++)
			{
				if (null == metaData.QuizMetaData.AnswerDataList[i].Key)
				{
					metaData.QuizMetaData.AnswerDataList.RemoveAt (i);
					i--;
				}
			}

			foreach (var item in metaData.QuizMetaData.DataList)
			{
				var atomCore = item.Key as Atom;
				item.Value.Name = atomCore.GetProperVar ();
				metaData.QuizMetaData.QuizPropertyNodeList.Add (item.Value);
			}

			var quizAnswerNode = new EBookQuizAnswerNode ();
			quizAnswerNode.QuizType = metaData.QuizMetaData.QuizType;

			foreach (var item in metaData.QuizMetaData.AnswerDataList)
			{
				var atomCore = item.Key as Atom;
				item.Value.AtomName = atomCore.AtomProperVar;
				quizAnswerNode.Values.Add (item.Value);
			}

			metaData.QuizMetaData.QuizAnswerNodeList.Add (quizAnswerNode);
		}

		public override void SaveQuizViewAtomForQQM (EBookQuizViewAtom atom)
		{
			if (null == atom)
				return;

			try
			{
				var fileDialog = new Softpower.SmartMaker.FileDBIO80.FileDBWindow (false, PROG_KIND._SMT, DOC_KIND._docQuizMaker, "");
				fileDialog.FilePath = PQAppBase.DefaultPath;
				fileDialog.FileName = atom.Attrib.AtomProperVar + ".QQM";

				string strProp = ExtensionInfo.GetPropertyFromFileName (fileDialog.FileName);
				fileDialog.DBProperty = _Kiss._isempty (strProp) ? "O" : strProp;

				if (fileDialog.ShowDialog () == true)
				{
					var jsonText = QuizMakerSaveManager.Instance.SaveJsonData (this, atom);

					var filePath = fileDialog.FilePath;
					var byteData = Encoding.UTF8.GetBytes (jsonText);


					//20250225 KH For QQM save & load

                    EBookManager = null;

                    OnSaveDocument(filePath);

                    /*
					var byteData = Encoding.UTF8.GetBytes (jsonText);

					using (var hgs = new GZipStream (File.Create (filePath), CompressionMode.Compress))
					{
						//outStream에 압축을 시킨다.
						hgs.Write (byteData, 0, byteData.Length);
					}
					*/

                    ToastMessge.Show ("저장되었습니다.");
				}
			}
			catch (IOException ex)
			{
				LogManager.WriteLog (ex);
				Trace.TraceError (ex.ToString ());
			}
		}



		#endregion//Public 메서드
	}
}
