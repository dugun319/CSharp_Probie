using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.VisualBasic;

using Newtonsoft.Json;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.CommonLib;
using Softpower.SmartMaker.TopApp.Metadata.QuizMaker;
using Softpower.SmartMaker.TopAtom.Components.TabViewAtom;
using Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView;
using Softpower.SmartMaker.TopControl.Components.Dialog;

using static Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView.EBookQuizOptionNode;

namespace Softpower.SmartMaker.TopAtom.Ebook
{
	/// <summary>
	/// EBookQuizViewofAtom.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class EBookQuizViewofAtom : EBookQuizViewAtomBase
	{
		#region | 임시로 추가 추후 논리 보강 필요 |

		public static int BackgroundKeyIndex = 263;

		#endregion

		//정답입력모드에서 사용하는 백업 리스트
		private List<Attrib> m_BackupAttribList = new List<Attrib> ();

		private Dictionary<AtomBase, FrameworkElement> m_LineQuizQuestionList = new Dictionary<AtomBase, FrameworkElement> ();
		private Dictionary<AtomBase, FrameworkElement> m_LineQuizAnswerList = new Dictionary<AtomBase, FrameworkElement> ();

		private Dictionary<AtomBase, FrameworkElement> m_QuestionSignList = new Dictionary<AtomBase, FrameworkElement> ();
		private Dictionary<AtomBase, FrameworkElement> m_AnswerSignList = new Dictionary<AtomBase, FrameworkElement> ();

		private Dictionary<string, FrameworkElement> m_AnswerEffectList = new Dictionary<string, FrameworkElement> ();

		private ContextMenu m_ModeChangeMenu = null;

		private bool m_IsLoaded = false;

		#region | Property |

		public override BaseViewControl MainTabViewControl
		{
			get
			{
				return EBookQuizViewControl;
			}
		}

		public int AtomAnswerEditMode { get; set; } = 0; // 0 : 편집모드, 1 : 정답입력모드, 2 : 배점 입력 모드

		public Grid RootEidtModeToolBar
		{
			get { return EidtModeToolBar; }
		}

		/// <summary>
		/// 퀴즈메이커를 통해 동적으로 생성된 아톰인경우에만 true
		/// </summary>
		public bool IsEmbedMode { get; set; } = false;

		/// <summary>
		/// 채점 동작이 이루어졌을때 정답인경우에만 true
		/// </summary>
		public bool IsAnswer { get; set; } = false;

		/// <summary>
		/// 점수
		/// </summary>
		public double AnswerPoint { get; set; } = 0;

		#endregion

		public EBookQuizViewofAtom ()
		{
			InitializeComponent ();
			InitStyle ();
			InitEvent ();

			//var window = new QuizViewTypeOptionWindow ();
			//window.Owner = Application.Current.MainWindow;
			//window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			//if (true == window.ShowDialog ())
			//{
			//	var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			//	atomAttrib.DisplayQuizType = window.QuizType;
			//}
		}

		public EBookQuizViewofAtom (Atom atomCore) : base (atomCore)
		{
			InitializeComponent ();

			InitStyle ();
			InitEvent ();
		}

		private void InitStyle ()
		{
			m_ModeChangeMenu = new ContextMenu ();

			MenuItem item1 = new MenuItem () { Header = "정답등록하기", Tag = 1, };
			MenuItem item2 = new MenuItem () { Header = "배점등록하기", Tag = 2, };
			MenuItem item3 = new MenuItem () { Header = "종료", Tag = 0, };

			m_ModeChangeMenu.Items.Add (item1);
			m_ModeChangeMenu.Items.Add (item2);
			m_ModeChangeMenu.Items.Add (item3);

			m_ModeChangeMenu.PlacementTarget = this;
			m_ModeChangeMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;

			item1.Click += Item_Click;
			item2.Click += Item_Click;
			item3.Click += Item_Click;
		}

		private void Item_Click (object sender, RoutedEventArgs e)
		{
			var menu = sender as MenuItem;
			if (int.TryParse (menu.Tag?.ToString (), out int mode))
			{
				if (mode != AtomAnswerEditMode)
				{
					if (0 != AtomAnswerEditMode && 0 != mode)
					{
						SetModeChanged (0);
					}

					SetModeChanged (mode);
				}
			}
		}

		private void InitEvent ()
		{
			this.Loaded += EBookQuizViewofAtom_Loaded;
			this.PreviewKeyDown += EBookQuizViewofAtom_PreviewKeyDown;

			EBookQuizViewControl.QuizViewContainerPanel.OnMoveAtom += QuizViewContainerPanel_OnMoveAtom;
			EBookQuizViewControl.QuizViewContainerPanel.OnDeleteAtom += QuestionViewContainerPanel_OnDeleteAtom;

			AnswerActionRadio1.Checked += AnswerActionRadio_Checked;
			AnswerActionRadio1.Unchecked += AnswerActionRadio_Checked;

			AnswerActionRadio2.Checked += AnswerActionRadio_Checked;
			AnswerActionRadio2.Unchecked += AnswerActionRadio_Checked;

			TabPage tabPage = this.EBookQuizViewControl.GetFirstTabPage ();
			if (null != tabPage)
			{
				tabPage.OnUpdateChildrenPosition += TabPage_OnUpdateChildrenPosition;
			}
		}

		private void AnswerActionRadio_Checked (object sender, RoutedEventArgs e)
		{
			if (2 == this.AtomAnswerEditMode)
			{
				if (true == AnswerActionRadio1.IsChecked)
				{
					EBookQuizViewControl.IsEnabled = false;
				}
				else
				{
					EBookQuizViewControl.IsEnabled = true;
				}
			}
		}

		private void TabPage_OnUpdateChildrenPosition (object sender)
		{
			var atom = sender as AtomBase;

			if (null != atom.AtomCore)
			{
				SetLineObjectEffecf (atom);
				SetActionSignEffect (atom.AtomCore);
			}
		}

		private void Canvas_SizeChanged (object sender, SizeChangedEventArgs e)
		{

		}

		private void EBookQuizViewofAtom_PreviewKeyDown (object sender, KeyEventArgs e)
		{
			if (Key.F3 == e.Key && 1 == this.AtomCore.AtomRunMode)
			{
				ExecuteAnswer ();
			}
			else if (Key.F4 == e.Key)
			{
				ClearQuizView ();
			}
		}

		public void ClearQuizView ()
		{
			EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.IsExecuteEngine = true;

			foreach (var item in m_AnswerEffectList)
			{
				var parent = item.Value.Parent as Panel;

				if (null != parent)
					parent.Children.Remove (item.Value);
			}

			m_AnswerEffectList.Clear ();
			EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.ClearQuizViewEngine ();
		}

		private void EBookQuizViewofAtom_Loaded (object sender, RoutedEventArgs e)
		{
			UpdateLatestViewItems ();

			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var atomList = EBookQuizViewControl.GetAllAtomsInTabView ().Cast<AtomBase> ().ToList ();

			atomAttrib.DataMap = new Dictionary<Atom, EBookQuizPropertyNode> ();
			atomAttrib.AnswerDataMap = new Dictionary<Atom, EBookQuizAnswerValueNode> ();

			foreach (var node in atomAttrib.SerializeData)
			{
				var findAtom = atomList.Find (item => item.AtomCore.GetProperVar () == node.Name);

				if (null != findAtom)
				{
					atomAttrib.DataMap.Add (findAtom.AtomCore, node);
				}
			}

			foreach (var node in atomAttrib.SerializeAnswerData)
			{
				foreach (var item in node.Values)
				{
					var findAtom = atomList.Find (i => i.AtomCore.GetProperVar () == item.AtomName);

					if (null != findAtom && false == atomAttrib.AnswerDataMap.ContainsKey (findAtom.AtomCore))
					{
						atomAttrib.AnswerDataMap.Add (findAtom.AtomCore, item);
					}
				}
			}

			m_IsLoaded = true;
			CompletePropertyChanged ();
		}

		protected override void InitializeAtomCore ()
		{
			m_AtomCore = new EBookQuizViewAtom ();
		}

		protected override void InitializeResizeAdorner ()
		{
			m_ResizeAdorner = new Point8Adorner (this);
		}

		protected override void InitializeAtomSize ()
		{
			Size atomSize = DefaultAtomSizeManager.GetDefaultRect (AtomType.EBookQuizView);
			this.Width = atomSize.Width;
			this.Height = atomSize.Height;
		}

		public override void CompletePropertyChanged ()
		{
			var atomCore = this.AtomCore as EBookQuizViewAtom;
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var atoms = this.GetAllAtomCores ();
			var answerCount = 0;
			var questionCount = 0;
			var isDefaultAtomName = GetQuizViewDefaultAtomName ();
			var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
			var info = this.AtomCore.Information;

			foreach (var item in atomAttrib.DataMap)
			{
				if (item.Value.ActionType == QuizAction.Answer)
				{
					answerCount++;
				}
				else if (item.Value.ActionType == QuizAction.Question)
				{
					questionCount++;
				}
			}

			if (isDefaultAtomName && GetIsMakeDefaultAtom ())
			{
				//퀴즈 옵션에서 문/답항 항목을 설정한경우 아톰 자동 생성
				info.MakeQuizViewDefaultAtom (this.AtomCore);
			}

			atomCore.MakeAnswerGroup ();

			foreach (TabViewItem item in TabViewItem)
			{
				if (item.CurrentTabPage.Parent is Panel panel)
				{
					panel.Children.Remove (item.CurrentTabPage);
				}
			}

			EBookQuizViewControl.ChangeTabViewItems (TabViewItem);

			engine.AtomAttrib = atomAttrib;
			engine.CompletePropertyChanged ();

			DrawingQuizEffect ();


			if (0 == this.AtomCore.AtomRunMode && false == IsEmbedMode)
			{
				info.ChangeQuizBlockAtomName (this.AtomCore);
			}
		}

		private bool GetIsMakeDefaultAtom ()
		{
			var atomAttrib = this.AtomCore.Attrib as EBookQuizViewAttrib;

			if (null != atomAttrib)
			{
				/*
				 * 20250306 KH 끌어놓기 양식 자동생성 구현하여 주석처리함.
				//끌어놓기의 경우 문/답항 설정하는 속성창이 없기 때문에 무조건 false처리
				if (atomAttrib.DisplayQuizType == QuizType.A31 ||
					 atomAttrib.DisplayQuizType == QuizType.A35)
					return false;
				*/
				if (m_IsLoaded && IsAutoMakeAtom && IsOpenQuizOptionPage)
					return true;
			}

			return false;
		}

		public override void SetAtomFontSize (double dApplySize)
		{
			base.SetAtomFontSize (dApplySize);

			this.FontSize = dApplySize;

			foreach (TabViewItem tabViewItem in TabViewItem)
			{
				if (System.Windows.Visibility.Visible == tabViewItem.CurrentTabPage.GetVirtualRectangleVisibility ())
				{
					foreach (var item in m_QuestionSignList)
					{
						if (item.Value is TextBlock block)
							block.FontSize = dApplySize;
					}

					foreach (var item in m_AnswerSignList)
					{
						if (item.Value is TextBlock block)
							block.FontSize = dApplySize;
					}

					DrawingQuizEffect ();

					break;
				}
			}
		}

		public override double GetAtomFontSize ()
		{
			foreach (TabViewItem tabViewItem in TabViewItem)
			{
				if (System.Windows.Visibility.Visible == tabViewItem.CurrentTabPage.GetVirtualRectangleVisibility ())
				{
					return this.FontSize;
				}
			}

			return base.GetAtomFontSize ();
		}

		public override void SetAtomFontColor (Brush applyBrush)
		{
			base.SetAtomFontColor (applyBrush);

			foreach (TabViewItem tabViewItem in TabViewItem)
			{
				if (System.Windows.Visibility.Visible == tabViewItem.CurrentTabPage.GetVirtualRectangleVisibility ())
				{
					this.Foreground = applyBrush;

					foreach (var item in m_QuestionSignList)
					{
						if (item.Value is TextBlock block)
							block.Foreground = applyBrush;
					}

					foreach (var item in m_AnswerSignList)
					{
						if (item.Value is TextBlock block)
							block.Foreground = applyBrush;
					}

					DrawingQuizEffect ();

					break;
				}
			}
		}

		public override Brush GetAtomFontColor ()
		{
			foreach (TabViewItem tabViewItem in TabViewItem)
			{
				if (System.Windows.Visibility.Visible == tabViewItem.CurrentTabPage.GetVirtualRectangleVisibility ())
				{
					return this.Foreground;
				}
			}

			return base.GetAtomFontColor ();
		}

		private void A2TargetAtom_SizeChanged (object sender, SizeChangedEventArgs e)
		{
			var atomBase = sender as AtomBase;
			SetLineObjectEffecf (atomBase);
			SetActionSignEffect (atomBase.AtomCore);
		}

		public override void SerializeLoadSync_AttribToAtom (bool bIs80Model)
		{
			base.SerializeLoadSync_AttribToAtom (bIs80Model);
		}

		public override void Sync_FontAttribToAtom (Attrib atomAttrib, bool bIs80Model)
		{
			base.Sync_FontAttribToAtom (atomAttrib, bIs80Model);

			int nFontKey = atomAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._FONT);
			CObjectFont pObjectFont = null;
			atomAttrib.GetGDIObjFromKey (ref pObjectFont, nFontKey);

			if (null != pObjectFont)
			{
				SetAtomFontSize (pObjectFont.SelectFont.Size);
				SetAtomFontColor (pObjectFont.WpfColor);
			}
		}

		public override void OnFirstMakeSync (bool bFalse)
		{
			base.OnFirstMakeSync (bFalse);

			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.AtomAttrib = atomAttrib;

			SetModeChanged (0);
		}

		public override void ShowAtomContextMenu ()
		{
			if (0 == AtomAnswerEditMode)
			{
				base.ShowAtomContextMenu ();
			}
			else
			{
				m_ModeChangeMenu.StaysOpen = false;
				m_ModeChangeMenu.IsOpen = true;
			}
		}

		private void QuizViewContainerPanel_OnMoveAtom (object objValue)
		{
			var commands = objValue as Softpower.SmartMaker.TopAtom.Commands.MoveGroupedAtomsCommand;
			foreach (var command in commands.MoveAtomCommandList)
			{
				var atom = command.TargetAtom;

				SetLineObjectEffecf (atom);
				SetActionSignEffect (atom.AtomCore);
			}

			this.AtomCore.Information.CallMoveAtomCommand (objValue);
		}

		private void QuestionViewContainerPanel_OnDeleteAtom (object objValue)
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var targetAtomBase = objValue as AtomBase;

			if (null != targetAtomBase.AtomCore)
			{
				atomAttrib.DataMap.Remove (targetAtomBase.AtomCore);
				var findItem = atomAttrib.SerializeData.Find (i => i.Name == targetAtomBase.AtomCore.GetProperVar ());
				atomAttrib.SerializeData.Remove (findItem);
			}

			DrawingQuizEffect ();
		}

		public override void DoPostEditMode ()
		{
			base.DoPostEditMode ();
		}

		public override void DoPostRunMode ()
		{
			base.DoPostRunMode ();
		}

		public override void ApplyRunModeProperty ()
		{
			base.ApplyRunModeProperty ();
		}

		public override void ReleaseRunModeProperty ()
		{
			base.ReleaseRunModeProperty ();
		}

		public override void ChangeAtomMode (int nRunMode)
		{
			CompletePropertyChanged ();
			EBookQuizViewControl.ChangeAtomMode (nRunMode);
			m_AnswerEffectList.Clear ();

			if (0 == nRunMode)
			{
				//EidtModeToolBar.Visibility = Visibility.Visible;
			}
			else
			{
				if (0 != AtomAnswerEditMode)
				{
					SetModeChanged (0);
				}

				EidtModeToolBar.Visibility = Visibility.Collapsed;
				EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.IsExecuteEngine = true;
			}

			base.ChangeAtomMode (nRunMode);
		}

		private void DrawingQuizEffect ()
		{
			QuizEffectGrid.Children.Clear ();

			m_LineQuizAnswerList.Clear ();
			m_LineQuizQuestionList.Clear ();

			m_QuestionSignList.Clear ();
			m_AnswerSignList.Clear ();

			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var pen = new Pen () { Brush = Brushes.Transparent, };

			var answerList = new List<Atom> ();
			var questionList = new List<Atom> ();

			foreach (var item in atomAttrib.DataMap)
			{
				if (QuizType.A21 == item.Value.QuizType ||
					QuizType.A25 == item.Value.QuizType)
				{
					SetLineObjectEffecf (item.Key.GetOfAtom (), item.Value.ActionType);
				}

				if (QuizType.C11 == item.Value.QuizType ||
					QuizType.C15 == item.Value.QuizType)
				{
					SetTextObjectEffect (item.Key.GetOfAtom (), item.Value.ActionType);
				}

				if (item.Value.ActionType == QuizAction.Question)
				{
					questionList.Add (item.Key);
				}
				else if (item.Value.ActionType == QuizAction.Answer)
				{
					answerList.Add (item.Key);
				}
			}

			for (int i = 0; i < questionList.Count; i++)
			{
				SetActionSignEffect (questionList[i], i);
			}

			for (int i = 0; i < answerList.Count; i++)
			{
				SetActionSignEffect (answerList[i], i);
			}

			foreach (var item in atomAttrib.DataMap)
			{
				var atomBase = item.Key.GetOfAtom ();

				atomBase.SizeChanged -= A2TargetAtom_SizeChanged;

				if (QuizType.A21 == item.Value.QuizType ||
					QuizType.A25 == item.Value.QuizType)
				{
					atomBase.SizeChanged += A2TargetAtom_SizeChanged;
				}
			}

			var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;

			engine.LineQuizQuestionList = new Dictionary<AtomBase, FrameworkElement> (m_LineQuizQuestionList);
			engine.LineQuizAnswerList = new Dictionary<AtomBase, FrameworkElement> (m_LineQuizAnswerList);
		}


		/// <summary>
		/// 일반 아톰 -> 문항/답항 설정시 아톰명 변경
		/// 문항/답항 -> 일반으로 변경시 기본 아톰명으로 변경 
		/// </summary>
		//private void ChangeAtomName ()
		//{
		//	ChangeElementAtomName ();
		//	ChangeBaseAtomName ();
		//}

		/// <summary>
		/// 답항, 문항으로 설정된 모든 아톰명이 기본 아톰명, 또는 자동설정 아톰명인 경우 추가되는 모든 아톰들에 대해서 자동으로 명칭을 변경해주는 기능
		/// A_문항1 ~ A_문항N
		/// A_답항1 ~ A_답항N
		/// </summary>
		//private void ChangeElementAtomName ()
		//{
		//	var atomAttirb = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
		//	var info = this.AtomCore.Infomation;
		//	var sign = info.GetEBookQuizViewSignCode (this.AtomCore);
		//	var answerAtomList = new List<Atom> ();
		//	var questionAtomList = new List<Atom> ();

		//	var isAtomNameChanged = false;
		//	var isDefaultAtomName = GetQuizViewDefaultAtomName ();

		//	if (false == isDefaultAtomName)
		//		return;

		//	foreach (var item in atomAttirb.DataMap)
		//	{
		//		if (item.Value.ActionType == QuizAction.Question)
		//		{
		//			questionAtomList.Add (item.Key);
		//		}
		//		else if (item.Value.ActionType == QuizAction.Answer)
		//		{
		//			answerAtomList.Add (item.Key);
		//		}
		//	}

		//	for (int i = 0; i < questionAtomList.Count; i++)
		//	{
		//		var targetAtom = questionAtomList[i];
		//		var targetName = targetAtom.GetProperVar ();

		//		var autoName = $"{sign}_문항{i + 1}";

		//		if (!targetName.Equals (autoName))
		//		{
		//			var targetattrib = targetAtom.GetAttrib ();
		//			targetattrib.AtomProperVar = autoName;
		//			isAtomNameChanged = true;
		//		}
		//	}

		//	for (int i = 0; i < answerAtomList.Count; i++)
		//	{
		//		var targetAtom = answerAtomList[i];
		//		var targetName = targetAtom.GetProperVar ();

		//		var autoName = $"{sign}_답항{i + 1}";

		//		if (!targetName.Equals (autoName))
		//		{
		//			targetAtom.GetOfAtom ().AtomCore.AtomProperVar = autoName;
		//			isAtomNameChanged = true;
		//		}
		//	}

		//	if (isAtomNameChanged)
		//	{
		//		ToastMessge.Show ($"{this.AtomCore.AtomProperVar}에 문항/답항 아톰명칭이 자동으로 설정되었습니다");
		//	}
		//}

		/// <summary>
		/// 문항, 답항외 나머지 아톰들중 A_문항, A_답항 형식에 아톰명 사용시 기본 아톰명으로 변경하는 논리 보강
		/// </summary>
		//private void ChangeBaseAtomName ()
		//{
		//	var info = this.AtomCore.Infomation;
		//	var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
		//	var bindAtoims = this.GetAllAtomCores ();

		//	var sign = info.GetEBookQuizViewSignCode (this.AtomCore);

		//	foreach (var targetAtom in bindAtoims)
		//	{
		//		if (true == atomAttrib.DataMap.ContainsKey (targetAtom))
		//		{
		//			var actionType = atomAttrib.DataMap[targetAtom].ActionType;

		//			if (actionType == QuizAction.Question || actionType == QuizAction.Answer)
		//				continue;
		//		}

		//		var targetName = targetAtom.GetProperVar ();

		//		targetName = targetName.Replace ($"{sign}_문항", "");
		//		targetName = targetName.Replace ($"{sign}_답항", "");

		//		if (false == string.IsNullOrEmpty (targetName) && int.TryParse (targetName, out int num))
		//		{
		//			var targetAttrib = targetAtom.GetAttrib ();
		//			var targetofAtom = targetAtom.GetOfAtom ();
		//			targetAttrib.AtomProperVar = targetofAtom.AtomCore.Attrib.DefaultAtomProperVar;
		//			info.AdjustAtomProperVar (targetAtom);
		//		}
		//	}	
		//}

		public bool GetQuizViewDefaultAtomName ()
		{
			var isDefaultAtomName = true;

			var atomAttirb = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var info = this.AtomCore.Information;

			var sign = info.GetEBookQuizViewSignCode (this.AtomCore);

			foreach (var item in atomAttirb.DataMap)
			{
				var targetAtom = item.Key;
				var defaultName = targetAtom.GetDefaultProperVar ();
				var atomName = targetAtom.GetProperVar ();
				var autoName = "";

				if (item.Value.ActionType == QuizAction.Question)
				{
					autoName = $"{sign}_문항";
				}
				else if (item.Value.ActionType == QuizAction.Answer)
				{
					autoName = $"{sign}_답항";
				}

				if (!string.IsNullOrEmpty (defaultName)) atomName = atomName.Replace (defaultName, "");
				if (!string.IsNullOrEmpty (autoName)) atomName = atomName.Replace (autoName, "");

				if (false == string.IsNullOrEmpty (atomName) && false == int.TryParse (atomName, out int num))
				{
					isDefaultAtomName = false;
					break;
				}
			}

			return isDefaultAtomName;
		}

		#region | 효과 |

		#region | 문항 / 답항 기호 |

		private void SetActionSignEffect (Atom targetAtom, int index = -1)
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;

			if (false == atomAttrib.DataMap.ContainsKey (targetAtom))
				return;

			var quizType = atomAttrib.DisplayQuizType;
			var actionType = atomAttrib.DataMap[targetAtom].ActionType;
			var sign = GetActionSign (actionType, index);

			var dx = targetAtom.Attrib.AtomX;
			//var dy = (targetAtom.AtomY + targetAtom.AtomHeight / 2) - blockHeight / 2;
			var dy = targetAtom.Attrib.AtomY;

			FrameworkElement block = null;

			if (actionType == QuizAction.Answer)
			{
				if (false == m_AnswerSignList.ContainsKey (targetAtom.GetOfAtom ()))
				{
					if (true == string.IsNullOrEmpty (sign))
						return;

					if (QuizType.A11 == quizType ||
						QuizType.A15 == quizType)
					{
						//선다형일때 답항기호를 답항의 Text영역에 설정하는걸로 처리한다.
						if (targetAtom is LookAtom squareAtom)
						{
							var answerText = squareAtom.GetTitle ();

							if (true == string.IsNullOrEmpty (answerText) ||
								StrLib.IsNumber (answerText) ||
								atomAttrib.EBookQuizOptionNode.A11.ContainsAnswerSign (answerText))
							{
								squareAtom.SetTitle (sign);
								squareAtom.SetContentString (sign, false);
							}
						}

						return;
					}

					var newBlock = new TextBlock ()
					{
						Foreground = this.Foreground,
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalAlignment = HorizontalAlignment.Left,
						FontSize = this.FontSize,
						Text = sign,
					};

					m_AnswerSignList.Add (targetAtom.GetOfAtom (), newBlock);
					QuizEffectGrid.Children.Add (newBlock);
				}

				block = m_AnswerSignList[targetAtom.GetOfAtom ()];
			}
			else if (actionType == QuizAction.Question)
			{
				if (false == m_QuestionSignList.ContainsKey (targetAtom.GetOfAtom ()))
				{
					if (true == string.IsNullOrEmpty (sign))
						return;

					var newBlock = new TextBlock ()
					{
						Foreground = this.Foreground,
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalAlignment = HorizontalAlignment.Left,
						FontSize = this.FontSize,
						Text = sign,
					};

					m_QuestionSignList.Add (targetAtom.GetOfAtom (), newBlock);
					QuizEffectGrid.Children.Add (newBlock);
				}

				block = m_QuestionSignList[targetAtom.GetOfAtom ()];
			}

			if (null != block)
			{
				//if (0 < block.ActualWidth && 0 < block.ActualHeight)
				//{
				//	dx -= block.ActualWidth * 1.2;
				//}
				//else 
				if (block is TextBlock textBlock)
				{
					dx -= textBlock.FontSize * 1.2;
				}

				if (actionType == QuizAction.Answer &&
					(atomAttrib.DisplayQuizType == QuizType.A21) &&
					atomAttrib.EBookQuizOptionNode.A21.DirectionType == 1)
				{
					dx -= atomAttrib.EBookQuizOptionNode.A21.ObjectWidth * 1.5;
				}
				else if (actionType == QuizAction.Answer &&
					(atomAttrib.DisplayQuizType == QuizType.A25) &&
					atomAttrib.EBookQuizOptionNode.A25.DirectionType == 1)
				{
					dx -= atomAttrib.EBookQuizOptionNode.A25.ObjectWidth * 1.5;
				}

				dx = Math.Max (0, dx);
				dy = Math.Max (0, dy);

				block.Margin = new Thickness (dx, dy, 0, 0);
			}
		}

		private string GetActionSign (QuizAction type, int index)
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var quizOptionNode = atomAttrib.EBookQuizOptionNode;
			switch (atomAttrib.DisplayQuizType)
			{
				case QuizType.A11: //선다형
					return QuizAction.Answer == type ? quizOptionNode.A11.GetAnswerSign (index) : "";
				case QuizType.A15:
					return QuizAction.Answer == type ? quizOptionNode.A15.GetAnswerSign (index) : "";
				case QuizType.A21: //선긋기
					return QuizAction.Answer == type ? quizOptionNode.A21.GetAnswerSign (index) : quizOptionNode.A21.GetQuestionSign (index);
				case QuizType.A25:
					return QuizAction.Answer == type ? quizOptionNode.A25.GetAnswerSign (index) : quizOptionNode.A25.GetQuestionSign (index);
				case QuizType.A31: //끌어놓기
					return "";
				case QuizType.A35:
					return QuizAction.Answer == type ? quizOptionNode.A35.GetAnswerSign (index) : quizOptionNode.A35.GetQuestionSign (index);
				case QuizType.A41: //빈칸채움
					return "";
				case QuizType.A45:
					return QuizAction.Answer == type ? quizOptionNode.A45.GetAnswerSign (index) : "";
				case QuizType.A51: //순서맞춤
					return "";
				case QuizType.A55:
					return QuizAction.Answer == type ? quizOptionNode.A55.GetAnswerSign (index) : quizOptionNode.A55.GetQuestionSign (index);
				case QuizType.A61: //OX퀴즈
					return QuizAction.Answer == type ? quizOptionNode.A61.GetAnswerSign (index) : "";
				case QuizType.A65:
					return QuizAction.Answer == type ? quizOptionNode.A65.GetAnswerSign (index) : "";
				case QuizType.C11: //단답형
					return QuizAction.Answer == type ? quizOptionNode.C11.GetAnswerSign (index) : "";
				case QuizType.C15:
					return QuizAction.Answer == type ? quizOptionNode.C15.GetAnswerSign (index) : "";
				case QuizType.C21: //서술형
					return "";
				case QuizType.C25:
					return QuizAction.Answer == type ? quizOptionNode.C25.GetAnswerSign (index) : quizOptionNode.C25.GetQuestionSign (index);
				case QuizType.E11:
					return "";
				default:
					PQAppBase.TraceDebugLog ($"not found QuizType : {atomAttrib.DisplayQuizType}");
					break;
			}

			return "";
		}


		#endregion

		#region | 선긋기 효과 (블릿) |

		private void SetLineObjectEffecf (AtomBase targetAtom)
		{
			if (true == m_LineQuizQuestionList.ContainsKey (targetAtom))
			{
				SetLineObjectEffecf (targetAtom, QuizAction.Question);
			}
			else if (true == m_LineQuizAnswerList.ContainsKey (targetAtom))
			{
				SetLineObjectEffecf (targetAtom, QuizAction.Answer);
			}
		}

		private void SetLineObjectEffecf (AtomBase targetAtom, QuizAction type)
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var targetAtomName = targetAtom.AtomCore.GetProperVar ();
			var optionNode = atomAttrib.EBookQuizOptionNode;

			var objectWidth = 0;
			var objectHeight = 0;
			var objectBackgroundColor = 0;
			var objectBorderColor = 0;
			var directionType = 0;
			var verticalOffset = 0;
			var horizontalOffset = 0;

			var quizType = atomAttrib.DisplayQuizType;

			if (quizType == QuizType.A21)
			{
				objectWidth = optionNode.A21.ObjectWidth;
				objectHeight = optionNode.A21.ObjectHeight;
				objectBackgroundColor = optionNode.A21.ObjectBackgroundColor;
				objectBorderColor = optionNode.A21.ObjectBorderColor;
				directionType = optionNode.A21.DirectionType;
				verticalOffset = optionNode.A21.VerticalOffset;
				horizontalOffset = optionNode.A21.HorizontalOffset;
			}
			else if (quizType == QuizType.A25)
			{
				objectWidth = optionNode.A25.ObjectWidth;
				objectHeight = optionNode.A25.ObjectHeight;
				objectBackgroundColor = optionNode.A25.ObjectBackgroundColor;
				objectBorderColor = optionNode.A25.ObjectBorderColor;
				directionType = optionNode.A25.DirectionType;
				verticalOffset = optionNode.A25.VerticalOffset;
				horizontalOffset = optionNode.A25.HorizontalOffset;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			if (QuizAction.Question == type)
			{
				if (false == m_LineQuizQuestionList.ContainsKey (targetAtom))
				{
					var ellipse = new Ellipse
					{
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalAlignment = HorizontalAlignment.Left,
						Width = objectWidth,
						Height = objectWidth,
						Fill = WPFColorConverter.ConvertArgbToMediaBrush (objectBackgroundColor),
						Stroke = WPFColorConverter.ConvertArgbToMediaBrush (objectBorderColor),
						Tag = targetAtom,
					};

					ellipse.MouseLeftButtonDown -= Ellipse_MouseLeftButtonDown;
					ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;

					ellipse.MouseLeftButtonUp -= Ellipse_MouseLeftButtonUp;
					ellipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;

					ellipse.MouseMove -= Ellipse_MouseMove;
					ellipse.MouseMove += Ellipse_MouseMove;

					QuizEffectGrid.Children.Add (ellipse);
					m_LineQuizQuestionList.Add (targetAtom, ellipse);
				}

				if (0 == directionType)
				{
					//상하
					double dx = targetAtom.AtomCore.Attrib.AtomX;
					double dy = targetAtom.AtomCore.Attrib.AtomY;
					double dw = targetAtom.AtomCore.Attrib.AtomWidth;
					double dh = targetAtom.AtomCore.Attrib.AtomHeight;

					double x = (dx + (dw / 2)) - objectWidth / 2;
					double y = dy + dh + verticalOffset;

					m_LineQuizQuestionList[targetAtom].Margin = new Thickness (x, y, 0, 0);
				}
				else
				{
					//좌우
					double dx = targetAtom.AtomCore.Attrib.AtomX;
					double dy = targetAtom.AtomCore.Attrib.AtomY;
					double dw = targetAtom.AtomCore.Attrib.AtomWidth;
					double dh = targetAtom.AtomCore.Attrib.AtomHeight;

					double x = dx + dw + optionNode.A21.HorizontalOffset;
					double y = (dy + (dh / 2)) - optionNode.A21.ObjectHeight / 2;

					m_LineQuizQuestionList[targetAtom].Margin = new Thickness (x, y, 0, 0);
				}
			}
			else
			{
				if (false == m_LineQuizAnswerList.ContainsKey (targetAtom))
				{
					var ellipse = new Ellipse
					{
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalAlignment = HorizontalAlignment.Left,
						Width = objectWidth,
						Height = objectWidth,
						Fill = WPFColorConverter.ConvertArgbToMediaBrush (objectBackgroundColor),
						Stroke = WPFColorConverter.ConvertArgbToMediaBrush (objectBorderColor),
						Tag = targetAtom,
					};

					ellipse.MouseLeftButtonDown -= Ellipse_MouseLeftButtonDown;
					ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;

					ellipse.MouseLeftButtonUp -= Ellipse_MouseLeftButtonUp;
					ellipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;

					ellipse.MouseMove -= Ellipse_MouseMove;
					ellipse.MouseMove += Ellipse_MouseMove;

					QuizEffectGrid.Children.Add (ellipse);
					m_LineQuizAnswerList.Add (targetAtom, ellipse);
				}

				if (0 == directionType)
				{
					//상하
					double dx = targetAtom.AtomCore.Attrib.AtomX;
					double dy = targetAtom.AtomCore.Attrib.AtomY;
					double dw = targetAtom.AtomCore.Attrib.AtomWidth;
					double dh = targetAtom.AtomCore.Attrib.AtomHeight;

					double x = (dx + (dw / 2)) - objectWidth / 2;
					double y = dy - (verticalOffset + objectHeight);

					m_LineQuizAnswerList[targetAtom].Margin = new Thickness (x, y, 0, 0);
				}
				else
				{
					//좌우
					double dx = targetAtom.AtomCore.Attrib.AtomX;
					double dy = targetAtom.AtomCore.Attrib.AtomY;
					double dw = targetAtom.AtomCore.Attrib.AtomWidth;
					double dh = targetAtom.AtomCore.Attrib.AtomHeight;

					double x = dx - (horizontalOffset + objectWidth);
					double y = (dy + (dh / 2)) - objectHeight / 2;

					m_LineQuizAnswerList[targetAtom].Margin = new Thickness (x, y, 0, 0);
				}
			}
		}

		private void Ellipse_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			var ellipse = sender as Ellipse;
			var atomBase = ellipse.Tag as AtomBase;

			if (1 == this.AtomCore.AtomRunMode || 1 == this.AtomAnswerEditMode)
			{
				this.TextEditMode = true;
				EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.ExecuteMouseDown_A2 (atomBase);
			}
		}

		private void Ellipse_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
		{
			var ellipse = sender as Ellipse;
			var atomBase = ellipse.Tag as AtomBase;

			if (1 == this.AtomCore.AtomRunMode || 1 == this.AtomAnswerEditMode)
			{
				this.TextEditMode = false;
				EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.ExecuteMouseUp_A2 (atomBase);
			}
		}

		private void Ellipse_MouseMove (object sender, MouseEventArgs e)
		{
			var ellipse = sender as Ellipse;
			var atomBase = ellipse.Tag as AtomBase;

			if (1 == this.AtomCore.AtomRunMode || 1 == this.AtomAnswerEditMode)
			{
				this.TextEditMode = false;
				EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.ExecuteMouseMove_A2 (atomBase);
			}
		}

		#endregion

		#region | 단답형 입력효과 |

		private void SetTextObjectEffect (AtomBase targetAtom, QuizAction type)
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var targetAtomName = targetAtom.AtomCore.GetProperVar ();
			var optionNode = atomAttrib.EBookQuizOptionNode;

			var quizType = atomAttrib.DisplayQuizType;

			var answerDisplayType = 0;

			if (quizType == QuizType.C11)
			{
				answerDisplayType = 0;
			}
			else if (quizType == QuizType.C15)
			{
				answerDisplayType = optionNode.C15.AnswerDisplayType;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			if (QuizType.C15 == atomAttrib.DisplayQuizType && 1 == answerDisplayType)
			{
				var targetAtomCore = targetAtom.AtomCore;
				var x = targetAtomCore.Attrib.AtomX;
				var y = targetAtomCore.Attrib.AtomY;
				var w = targetAtomCore.Attrib.AtomWidth;
				var h = targetAtomCore.Attrib.AtomHeight;

				var textblock1 = new TextBlock
				{
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center,
					Text = "(",
					Background = null,
					Margin = new Thickness (0, 0, 0, 0),
					Height = h,
					FontSize = targetAtomCore.AtomBase.FontSize,
					Foreground = targetAtomCore.AtomBase.Foreground,
				};

				var textblock2 = new TextBlock
				{
					HorizontalAlignment = HorizontalAlignment.Right,
					VerticalAlignment = VerticalAlignment.Center,
					Text = ")",
					Margin = new Thickness (0, 0, 0, 0),
					Height = h,
					Background = null,
					FontSize = targetAtomCore.AtomBase.FontSize,
					Foreground = targetAtomCore.AtomBase.Foreground,
				};

				var grid = new Grid ()
				{
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Left,
					Margin = new Thickness (x, y, 0, 0),
					Width = w,
					Height = h,
					Background = null,
					Tag = targetAtomCore.Attrib.AtomProperVar,
				};

				grid.ColumnDefinitions.Add (new ColumnDefinition () { Width = new GridLength (1, GridUnitType.Auto) });
				grid.ColumnDefinitions.Add (new ColumnDefinition () { Width = new GridLength (1, GridUnitType.Star) });
				grid.ColumnDefinitions.Add (new ColumnDefinition () { Width = new GridLength (1, GridUnitType.Auto) });

				grid.Children.Add (textblock1);
				grid.Children.Add (textblock2);

				Grid.SetColumn (textblock1, 0);
				Grid.SetColumn (textblock2, 2);

				QuizEffectGrid.Children.Add (grid);
				m_LineQuizAnswerList.Add (targetAtom, grid);
			}
		}

		#endregion

		#endregion

		public Point GetScrollOffset ()
		{
			Point ptOffset = new Point ();

			TabPage tabPage = this.EBookQuizViewControl.GetFirstTabPage ();
			if (null != tabPage)
			{
				ScrollViewer scrollViewer = tabPage.GetScrollViewer ();
				if (null != scrollViewer)
				{
					ptOffset.X = scrollViewer.HorizontalOffset;
					ptOffset.Y = scrollViewer.VerticalOffset;
				}
			}

			return ptOffset;
		}

		public void SetModeChanged (int mode)
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;

			EidtModeToolBar.Visibility = Visibility.Collapsed;

			AnswerModeGrid.Visibility = Visibility.Collapsed;
			AnswerPointModeGrid.Visibility = Visibility.Collapsed;
			ModeTitleBlock.Visibility = Visibility.Collapsed;
			TotalAnswerPointBox.Visibility = Visibility.Collapsed;

			EBookQuizViewControl.IsEnabled = true;

			if (0 == mode && 0 != AtomAnswerEditMode)
			{
				this.TextEditMode = false;

				atomAttrib.TotalPoint = _Kiss.toDouble (TotalAnswerPointBox.Text);
				atomAttrib.AnswerActionType = true == AnswerActionRadio1.IsChecked ? 0 : 1;

				if (1 == AtomAnswerEditMode)
				{
					SaveAnswer ();
				}
				else
				{
					SaveAnswerPoint ();
				}

				SetContentRunMode (0);

				foreach (var bindAtom in GetAllAtomCores ())
				{
					var bindAtomAttrib = bindAtom.GetAttrib ();
					var bindBackupAttrib = m_BackupAttribList.Find (i => i.AtomProperVar == bindAtomAttrib.AtomProperVar);
					bindAtomAttrib.Commit (bindBackupAttrib);
				}

				foreach (var atom in GetAllAtomCores ())
				{
					atom.GetOfAtom ().Sync_AttribToAtom ();
				}

				AtomAnswerEditMode = mode;
			}
			else if (1 == mode && 1 != AtomAnswerEditMode)
			{
				EidtModeToolBar.Visibility = Visibility.Visible;

				AnswerModeGrid.Visibility = Visibility.Visible;
				ModeTitleBlock.Visibility = Visibility.Visible;
				ModeTitleBlock.Text = "정답등록하기";

				this.TextEditMode = true;

				m_BackupAttribList = new List<Attrib> ();

				foreach (var targetAtom in GetAllAtomCores ())
				{
					var targetAttrib = targetAtom.GetAttrib ();
					Type attribType = targetAttrib.GetType ();
					Attrib backupAttrib = Activator.CreateInstance (attribType) as Attrib;

					backupAttrib.Commit (targetAttrib);
					m_BackupAttribList.Add (backupAttrib);
				}

				SetContentRunMode (1);

				LoadAnswer ();
				AtomAnswerEditMode = mode;

			}
			else if (2 == mode)
			{
				EidtModeToolBar.Visibility = Visibility.Visible;

				AnswerPointModeGrid.Visibility = Visibility.Visible;
				ModeTitleBlock.Visibility = Visibility.Visible;
				TotalAnswerPointBox.Visibility = Visibility.Visible;
				ModeTitleBlock.Text = "배점등록하기";

				TotalAnswerPointBox.Text = atomAttrib.TotalPoint.ToString ();

				AnswerActionRadio1.IsChecked = atomAttrib.AnswerActionType == 0 ? true : false;
				AnswerActionRadio2.IsChecked = atomAttrib.AnswerActionType == 1 ? true : false;

				if (0 == atomAttrib.AnswerActionType)
				{
					EBookQuizViewControl.IsEnabled = false;
				}

				this.TextEditMode = true;

				m_BackupAttribList = new List<Attrib> ();

				foreach (var targetAtom in GetAllAtomCores ())
				{
					var targetAttrib = targetAtom.GetAttrib ();
					Type targetAttribType = targetAttrib.GetType ();
					Attrib backupAttrib = Activator.CreateInstance (targetAttribType) as Attrib;

					backupAttrib.Commit (targetAttrib);
					m_BackupAttribList.Add (backupAttrib);
				}

				SetContentRunMode (1);

				LoadAnswerPoint ();
				AtomAnswerEditMode = mode;
			}
		}

		private void SaveButton_Click (object sender, RoutedEventArgs e)
		{
			SetModeChanged (0);
		}

		#region  | Save Answer |

		public void SaveAnswer ()
		{
			var atomCore = this.AtomCore as EBookQuizViewAtom;
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;

			atomAttrib.AnswerDataMap.Clear ();

			foreach (var element in atomAttrib.DataMap)
			{
				if (QuizAction.Answer == element.Value.ActionType)
				{
					EBookQuizAnswerValueNode answerNode = SaveAnswerValue (element.Key, element.Value);

					if (null != answerNode)
					{
						atomAttrib.AnswerDataMap.Add (element.Key, answerNode);
					}
				}
			}

			atomAttrib.SerializeAnswerData.Clear ();
			atomCore.MakeAnswerGroup ();
		}

		private EBookQuizAnswerValueNode SaveAnswerValue (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			EBookQuizAnswerValueNode answerNode = null;
			switch (propertyNode.QuizType)
			{
				case QuizType.A11: //선다형
				case QuizType.A15:
					answerNode = SaveAnswerValue_A1 (targetAtom, propertyNode);
					break;
				case QuizType.A21: //선긋기
				case QuizType.A25:
					answerNode = SaveAnswerValue_A2 (targetAtom, propertyNode);
					break;
				case QuizType.A31: //항목이동
				case QuizType.A35:
					answerNode = SaveAnswerValue_A3 (targetAtom, propertyNode);
					break;
				case QuizType.A41: //빈칸채움
				case QuizType.A45:
					answerNode = SaveAnswerValue_A4 (targetAtom, propertyNode);
					break;
				case QuizType.A51: //순서맞춤
				case QuizType.A55:
					answerNode = SaveAnswerValue_A5 (targetAtom, propertyNode);
					break;
				case QuizType.A61: //OX
				case QuizType.A65:
					answerNode = SaveAnswerValue_A6 (targetAtom, propertyNode);
					break;
				case QuizType.C11: //단답형
				case QuizType.C15:
					answerNode = SaveAnswerValue_C1 (targetAtom, propertyNode);
					break;
				case QuizType.C21: //서술형
				case QuizType.C25:
					answerNode = SaveAnswerValue_C2 (targetAtom, propertyNode);
					break;
				case QuizType.E11: //그려넣기
					answerNode = SaveAnswerValue_E1 (targetAtom, propertyNode);
					break;
				//case QuizType.E2: //색칠하기
				//	answerNode = SaveAnswerValue_E2 (targetAtom, propertyNode);
				//	break;
				default:
					PQAppBase.TraceDebugLog ($"not found QuizType : {propertyNode.QuizType}");
					break;
			}

			return answerNode;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_A1 (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var targetAtomBase = targetAtom.GetOfAtom ();
			var elementName = targetAtom.GetProperVar ();

			var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
			var answerValue = engine.AnswerCheckControlMap.ContainsKey (targetAtomBase);

			var newNode = new EBookQuizAnswerValueNode
			{
				AtomName = elementName,
				AnswerValue = answerValue,
				AnswerValueType = QuizAnswerValueType.None
			};

			return newNode;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_A2 (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var elementName = targetAtom.GetProperVar ();
			var lineList = new List<string> ();
			//var elementValue = this.AtomCore.GetAtomAttribValue (elementName);

			var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;

			foreach (var line in engine.EffectGrid.Children.OfType<Line> ())
			{
				string tag = line.Tag?.ToString ();

				if (false == string.IsNullOrEmpty (tag))
				{
					var array = tag.Split (',');

					if (1 < array.Length && elementName == array[1]?.Trim ())
					{
						lineList.Add (array[0]);
					}
				}
			}

			var newNode = new EBookQuizAnswerValueNode
			{
				AtomName = elementName,
				AnswerValue = string.Join (",", lineList),
				AnswerValueType = QuizAnswerValueType.Line
			};

			return newNode;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_A3 (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var quizViewAttrib = this.AtomCore.Attrib as EBookQuizViewAttrib;

			var elementName = targetAtom.AtomProperVar;
			var answerValue = "";

			foreach (var item in quizViewAttrib.DataMap)
			{
				if (item.Key == targetAtom)
					continue;

				if (targetAtom is DecorImageAtom &&
					item.Key is DecorImageAtom decorImageAtom)
				{
					var targetAttrib = targetAtom.Attrib as DecorImageAttrib;
					var decorImageAttrib = decorImageAtom.Attrib as DecorImageAttrib;
					if (decorImageAttrib.ImagePath == targetAttrib.ImagePath &&
						decorImageAttrib.ImageKey == targetAttrib.ImageKey)
					{
						answerValue = decorImageAttrib.AtomProperVar;
						break;
					}
				}
				else if (targetAtom is EBookTextAtom &&
					item.Key is EBookTextAtom eBookTextAtom)
				{
					var targetofAtom = targetAtom.AtomBase as EBookTextofAtom;
					var textofAtom = eBookTextAtom.AtomBase as EBookTextofAtom;

					var startValue = targetofAtom.GetText ()?.Trim ();
					var endValue = textofAtom.GetText ()?.Trim ();

					if (startValue == endValue)
					{
						answerValue = eBookTextAtom.AtomProperVar;
						break;
					}
				}
				else
				{
					if (item.Key.GetContentString (false) == targetAtom.GetContentString (false))
					{
						answerValue = item.Key.AtomProperVar;
						break;
					}
				}
			}

			var newNode = new EBookQuizAnswerValueNode
			{
				AtomName = elementName,
				AnswerValue = answerValue,
				AnswerValueType = QuizAnswerValueType.BindName,
			};

			return newNode;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_A4 (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var elementName = targetAtom.GetProperVar ();
			var elementValue = this.AtomCore.GetAtomAttribValue (elementName);

			var newNode = new EBookQuizAnswerValueNode
			{
				AtomName = elementName,
				AnswerValue = elementValue,
				AnswerValueType = QuizAnswerValueType.Text,
			};

			return newNode;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_A5 (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var quizViewAttrib = this.AtomCore.Attrib as EBookQuizViewAttrib;

			var elementName = targetAtom.AtomProperVar;
			var answerValue = "";

			foreach (var item in quizViewAttrib.DataMap)
			{
				if (item.Key == targetAtom)
					continue;

				if (targetAtom is DecorImageAtom &&
					item.Key is DecorImageAtom decorImageAtom)
				{
					var targetAttrib = targetAtom.Attrib as DecorImageAttrib;
					var decorImageAttrib = decorImageAtom.Attrib as DecorImageAttrib;
					if (decorImageAttrib.ImagePath == targetAttrib.ImagePath &&
						decorImageAttrib.ImageKey == targetAttrib.ImageKey)
					{
						answerValue = decorImageAttrib.AtomProperVar;
						break;
					}
				}
				else if (targetAtom is EBookTextAtom &&
					item.Key is EBookTextAtom eBookTextAtom)
				{
					var targetofAtom = targetAtom.AtomBase as EBookTextofAtom;
					var textofAtom = eBookTextAtom.AtomBase as EBookTextofAtom;

					var startValue = targetofAtom.GetText ()?.Trim ();
					var endValue = textofAtom.GetText ()?.Trim ();

					if (startValue == endValue)
					{
						answerValue = eBookTextAtom.AtomProperVar;
						break;
					}
				}
				else
				{
					if (item.Key.GetContentString (false) == targetAtom.GetContentString (false))
					{
						answerValue = item.Key.AtomProperVar;
						break;
					}
				}
			}

			var newNode = new EBookQuizAnswerValueNode
			{
				AtomName = elementName,
				AnswerValue = answerValue,
				AnswerValueType = QuizAnswerValueType.BindName,
			};

			return newNode;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_A6 (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var targetAtomBase = targetAtom.GetOfAtom ();
			var elementName = targetAtom.GetProperVar ();
			var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;

			var newNode = new EBookQuizAnswerValueNode
			{
				AtomName = elementName,
				AnswerValueType = QuizAnswerValueType.None
			};

			if (engine.QuizElementMap.ContainsKey (elementName))
			{
				var elementControl = engine.QuizElementMap[elementName];
				if (elementControl is EBookQuizElementContentofAtom contentofAtom)
				{
					newNode.AnswerValue = contentofAtom.GetContent ();
				}
			}

			return newNode;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_C1 (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var elementName = targetAtom.GetProperVar ();
			var elementValue = this.AtomCore.GetAtomAttribValue (elementName);

			var newNode = new EBookQuizAnswerValueNode
			{
				AtomName = elementName,
				AnswerValue = elementValue,
				AnswerValueType = QuizAnswerValueType.Text,
			};

			return newNode;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_C2 (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var elementName = targetAtom.GetProperVar ();
			var elementValue = this.AtomCore.GetAtomAttribValue (elementName);

			var newNode = new EBookQuizAnswerValueNode
			{
				AtomName = elementName,
				AnswerValue = elementValue,
				AnswerValueType = QuizAnswerValueType.Text,
			};

			return newNode;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_E1 (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var actionType = atomAttrib.EBookQuizOptionNode.E11.ActionType;
			if (0 == actionType)
			{
				//선 그리기
				return SaveAnswerValue_E1_Line (targetAtom, propertyNode);
			}
			else if (1 == actionType)
			{
				//도형그리기
				return SaveAnswerValue_E1_Rect (targetAtom, propertyNode);
			}
			else if (2 == actionType)
			{
				//따라그리기

			}
			else if (3 == actionType)
			{
				//색칠하기
				return SaveAnswerValue_E1_Fill (targetAtom, propertyNode);
			}

			return null;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_E1_Line (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var elementName = targetAtom.GetProperVar ();
			var lineList = new List<string> ();
			//var elementValue = this.AtomCore.GetAtomAttribValue (elementName);

			var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;

			foreach (var line in engine.EffectGrid.Children.OfType<Line> ())
			{
				string tag = line.Tag?.ToString ();

				if (false == string.IsNullOrEmpty (tag))
				{
					var array = tag.Split (',');

					if (1 < array.Length && array.Contains (elementName))
					{
						lineList.AddRange (array);
					}
				}
			}

			lineList.Sort ();
			lineList = lineList.Distinct ().ToList ();
			lineList.Remove (elementName);

			if (0 < lineList.Count)
			{
				var newNode = new EBookQuizAnswerValueNode
				{
					AtomName = elementName,
					AnswerValue = string.Join (",", lineList),
					AnswerValueType = QuizAnswerValueType.Line
				};

				return newNode;
			}

			return null;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_E1_Rect (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var elementName = targetAtom.GetProperVar ();

			var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;

			if (engine.DrawCanvasMap.ContainsKey (elementName))
			{
				var canvas = engine.DrawCanvasMap[elementName];
				var values = new List<string> ();

				foreach (FrameworkElement element in canvas.Children)
				{
					var dx = 0.0d;
					var dy = 0.0d;
					var dw = 0.0d;
					var dh = 0.0d;

					var type = QuizOption_E11.RectType.None;

					if (null == element)
						continue;

					if (element is Line line)
					{
						type = QuizOption_E11.RectType.Line;
						dx = line.X1;
						dy = line.Y1;
						dw = Math.Abs (line.X2 - line.X1);
						dh = Math.Abs (line.Y2 - line.Y1);
					}
					else if (element is Rectangle rectangle)
					{
						type = QuizOption_E11.RectType.Rectangle;

						dx = rectangle.Margin.Left;
						dy = rectangle.Margin.Top;
						dw = rectangle.Width;
						dh = rectangle.Height;
					}
					else if (element is Ellipse ellipse)
					{
						type = QuizOption_E11.RectType.Ellipse;

						dx = ellipse.Margin.Left;
						dy = ellipse.Margin.Top;
						dw = ellipse.Width;
						dh = ellipse.Height;
					}

					values.Add ($"{type}, {dx}, {dy}, {dw}, {dh}");
				}

				if (0 < values.Count)
				{
					var newNode = new EBookQuizAnswerValueNode
					{
						AtomName = elementName,
						AnswerValue = JsonConvert.SerializeObject (values),
						AnswerValueType = QuizAnswerValueType.Rect
					};

					return newNode;
				}
			}

			return null;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_E1_Fill (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var elementName = targetAtom.GetProperVar ();
			var elementValue = false;

			var colorMap = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.AtomEditColorMap;

			if (true == colorMap.ContainsKey (elementName))
			{
				int editColor = colorMap[elementName];
				var retValue = new CVariantX (0);

				//배경색 가져오기
				targetAtom.GetProperty (BackgroundKeyIndex, null, retValue);

				int color = retValue.ToInt ();

				if (editColor != color)
				{
					elementValue = true;
				}
			}

			var newNode = new EBookQuizAnswerValueNode
			{
				AtomName = elementName,
				AnswerValue = elementValue,
				AnswerValueType = QuizAnswerValueType.Bool,
			};

			return newNode;
		}

		private EBookQuizAnswerValueNode SaveAnswerValue_E2 (Atom targetAtom, EBookQuizPropertyNode propertyNode)
		{
			var elementName = targetAtom.GetProperVar ();
			var elementValue = false;

			var colorMap = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.AtomEditColorMap;

			if (true == colorMap.ContainsKey (elementName))
			{
				int editColor = colorMap[elementName];
				var retValue = new CVariantX (0);

				//배경색 가져오기
				targetAtom.GetProperty (BackgroundKeyIndex, null, retValue);

				int color = retValue.ToInt ();

				if (editColor != color)
				{
					elementValue = true;
				}
			}

			var newNode = new EBookQuizAnswerValueNode
			{
				AtomName = elementName,
				AnswerValue = elementValue,
				AnswerValueType = QuizAnswerValueType.Bool,
			};

			return newNode;
		}
		#endregion

		#region | Load Answer |

		public void LoadAnswer ()
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;

			foreach (var element in atomAttrib.AnswerDataMap)
			{
				var elementAtom = element.Key;

				if (true == atomAttrib.DataMap.ContainsKey (elementAtom))
				{
					var propertyNode = atomAttrib.DataMap[elementAtom];
					switch (propertyNode.QuizType)
					{
						case QuizType.A11: //선다형
						case QuizType.A15:
							LoadAnswerValue_A1 (elementAtom, element.Value);
							break;
						case QuizType.A21: //선긋기
						case QuizType.A25:
							LoadAnswerValue_A2 (elementAtom, element.Value);
							break;
						case QuizType.A31: //항목이동
						case QuizType.A35:
							LoadAnswerValue_A3 (elementAtom, element.Value);
							break;
						case QuizType.A41: //빈칸채움
						case QuizType.A45:
							LoadAnswerValue_A4 (elementAtom, element.Value);
							break;
						case QuizType.A51: //순서맞춤
						case QuizType.A55:
							LoadAnswerValue_A5 (elementAtom, element.Value);
							break;
						case QuizType.A61: //OX
						case QuizType.A65:
							LoadAnswerValue_A6 (elementAtom, element.Value);
							break;
						case QuizType.C11: //단답형
						case QuizType.C15:
							LoadAnswerValue_C1 (elementAtom, element.Value);
							break;
						case QuizType.C21: //서술형
						case QuizType.C25:
							LoadAnswerValue_C2 (elementAtom, element.Value);
							break;
						case QuizType.E11: //그려넣기
							LoadAnswerValue_E1 (elementAtom, element.Value);
							break;
						//case QuizType.E2: //색칠하기
						//	LoadAnswerValue_E2 (elementAtom, element.Value);
						//	break;
						default:
							PQAppBase.TraceDebugLog ($"not found QuizType : {propertyNode.QuizType}");
							break;
					}
				}
			}
		}

		private void LoadAnswerValue_A1 (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var answerValue = _Kiss.toBool (valueNode.AnswerValue);

			if (true == answerValue)
			{
				var targetAtomBase = targetAtom.GetOfAtom ();
				var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
				engine.MakeCheckControl (targetAtomBase);
			}
		}

		private void LoadAnswerValue_A2 (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var answerValue = valueNode.AnswerValue?.ToString ();
			if (false == string.IsNullOrEmpty (answerValue))
			{
				var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
				var atomList = GetAllAtomCores ();
				var array = answerValue.Split (',');

				foreach (var startAtomName in array)
				{
					var startAtom = atomList.Find (item => item.GetProperVar () == startAtomName);

					if (null != startAtom)
					{
						engine.MakeLineA2 (startAtom.GetOfAtom (), targetAtom.GetOfAtom ());
					}
				}
			}
		}

		private void LoadAnswerValue_A3 (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var quizViewAttrib = this.AtomCore.Attrib as EBookQuizViewAttrib;

			if (valueNode.AnswerValueType == QuizAnswerValueType.BindName)
			{
				var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
				var answerValue = valueNode.AnswerValue?.ToString ();

				var targetNode = quizViewAttrib.DataMap.Where (i => i.Key.AtomProperVar == answerValue).FirstOrDefault ();

				if (null != targetNode.Key)
				{
					engine.SetDragAtomData (targetNode.Key, targetAtom);
				}
			}
		}

		private void LoadAnswerValue_A4 (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var text = null != valueNode.AnswerValue ? valueNode.AnswerValue.ToString () : "";

			var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
			engine.SetTextAtomData (targetAtom, text);
		}

		private void LoadAnswerValue_A5 (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var quizViewAttrib = this.AtomCore.Attrib as EBookQuizViewAttrib;

			if (valueNode.AnswerValueType == QuizAnswerValueType.BindName)
			{
				var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
				var answerValue = valueNode.AnswerValue?.ToString ();

				var targetNode = quizViewAttrib.DataMap.Where (i => i.Key.AtomProperVar == answerValue).FirstOrDefault ();

				if (null != targetNode.Key)
				{
					engine.SetDragAtomData (targetNode.Key, targetAtom);
				}
			}
		}

		private void LoadAnswerValue_A6 (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var answerValue = valueNode.AnswerValue?.ToString ();
			if (false == string.IsNullOrEmpty (answerValue))
			{
				var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
				engine.MakeContentElement_A6 (targetAtom.GetOfAtom (), answerValue);
			}
		}

		private void LoadAnswerValue_C1 (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var text = null != valueNode.AnswerValue ? valueNode.AnswerValue.ToString () : "";

			var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
			engine.SetTextAtomData (targetAtom, text);
		}

		private void LoadAnswerValue_C2 (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var text = null != valueNode.AnswerValue ? valueNode.AnswerValue.ToString () : "";
			targetAtom.SetContentString (text, false);
		}

		private void LoadAnswerValue_E1 (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var quizType = atomAttrib.DisplayQuizType;

			var actionType = 0;

			if (quizType == QuizType.E11)
			{
				actionType = atomAttrib.EBookQuizOptionNode.E11.ActionType;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			if (0 == actionType)
			{
				//선긋기
				LoadAnswerValue_E1_Line (targetAtom, valueNode);
			}
			else if (1 == actionType)
			{
				//도형그리기
				LoadAnswerValue_E1_Rect (targetAtom, valueNode);
			}
			else if (3 == actionType)
			{
				//색칠하기
				LoadAnswerValue_E1_Fill (targetAtom, valueNode);
			}
		}

		private void LoadAnswerValue_E1_Line (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var answerValue = valueNode.AnswerValue?.ToString ();
			if (false == string.IsNullOrEmpty (answerValue))
			{
				var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
				var atomList = GetAllAtomCores ();
				var array = answerValue.Split (',');

				foreach (var startAtomName in array)
				{
					var startAtom = atomList.Find (item => item.GetProperVar () == startAtomName);

					if (null != startAtom)
					{
						engine.MakeLineE1 (startAtom.GetOfAtom (), targetAtom.GetOfAtom ());
					}
				}
			}
		}

		private void LoadAnswerValue_E1_Rect (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var answerValue = valueNode.AnswerValue?.ToString ();
			if (false == string.IsNullOrEmpty (answerValue))
			{
				var engine = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine;
				var targetName = targetAtom.GetProperVar ();
				var list = JsonConvert.DeserializeObject<List<string>> (answerValue);

				foreach (var datas in list)
				{
					var values = datas.Split (',');

					if (4 <= values.Length)
					{
						var type = (QuizOption_E11.RectType)Enum.Parse (typeof (QuizOption_E11.RectType), values[0]);
						var dx = _Kiss.toDouble (values[1]);
						var dy = _Kiss.toDouble (values[2]);
						var dw = _Kiss.toDouble (values[3]);
						var dh = _Kiss.toDouble (values[4]);

						engine.MakeE1Rect (targetName, dx, dy, dw, dh, type);
					}
				}
			}
		}

		private void LoadAnswerValue_E1_Fill (Atom targetAtom, EBookQuizAnswerValueNode valueNode)
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var isColor = _Kiss.toBool (valueNode.AnswerValue);

			if (isColor)
			{
				var targetAtomName = targetAtom.GetProperVar ();
				var atomVariantX = new CVariantX (0);
				targetAtom.GetProperty (BackgroundKeyIndex, null, atomVariantX);
				var editMap = EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.AtomEditColorMap;

				editMap.Add (targetAtomName, atomVariantX.ToInt ());

				var color = atomAttrib.EBookQuizOptionNode.E11.AnswerColor;

				EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.FillRectE1 (targetAtom.AtomBase);

				//targetAtom.SetProperty (BackgroundKeyIndex, null, new CVariantX (color));
			}
		}

		//private void LoadAnswerValue_E2 (CAtom targetAtom, EBookQuizAnswerValueNode valueNode)
		//{
		//	var isColor = _Kiss.toBool (valueNode.AnswerValue);

		//	if (isColor)
		//	{
		//		var atomName = targetAtom.GetProperVar ();
		//		var atomVariantX = new CVariantX (0);
		//		targetAtom.GetProperty (BackgroundKeyIndex, null, atomVariantX);
		//		var editMap = EBookQuestionViewControl.QuestionViewContainerPanel.RootEBookQuestionViewEngine.AtomEditColorMap;

		//		editMap.Add (atomName, atomVariantX.ToInt ());

		//		int color = WPFColorConverter.ConvertWPFColorToDrawingArgb (SeletFillBrush.Color);
		//		targetAtom.SetProperty (BackgroundKeyIndex, null, new CVariantX (color));
		//	}
		//}

		#endregion

		#region | Save AnswerPoint |

		public void SaveAnswerPoint ()
		{
			var atomCore = this.AtomCore as EBookQuizViewAtom;
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;

			if (0 == atomAttrib.AnswerActionType)
			{
				// 단일채점
				atomAttrib.TotalPoint = _Kiss.toDouble (TotalAnswerPointBox.Text);
			}
			else
			{
				//부분채점

				Dictionary<string, double> pointMap = new Dictionary<string, double> ();
				var answerAtomList = new List<Atom> ();

				var totalPoint = 0;
				var totalCount = 0;
				var userInputPoint = 0;

				int.TryParse (TotalAnswerPointBox.Text, out totalPoint);

				foreach (var element in atomAttrib.DataMap)
				{
					if (QuizAction.Answer == element.Value.ActionType)
					{
						var targetAtom = element.Key;
						var targetAtomName = targetAtom.GetProperVar ();
						answerAtomList.Add (targetAtom);

						totalCount++;

						if (targetAtom is InputAtom inputAtomCore)
						{
							var data = inputAtomCore.GetContentString (false);
							if (int.TryParse (data, out int point))
							{
								if (false == pointMap.ContainsKey (targetAtomName))
								{
									pointMap.Add (targetAtomName, 0);
								}

								pointMap[targetAtomName] = point;
								userInputPoint += point;
							}
						}
					}
				}

				//부분채점인경우 그룹명칭을 아톰명으로 설정해 개별 채점동작하도록 보강
				//추후 다중유형 제공시 논리 변경이 필요할듯
				foreach (var item in atomAttrib.AnswerDataMap)
				{
					item.Value.GroupName = item.Key.GetProperVar ();
				}

				atomAttrib.SerializeAnswerData.Clear ();
				atomCore.MakeAnswerGroup ();

				var userInputCount = pointMap.Keys.Count;
				var baseCount = (totalCount - userInputCount);
				var basePoint = 0 < baseCount ? (totalPoint - userInputPoint) / baseCount : 0;

				if (1 > basePoint)
				{
					basePoint = 1;
					ToastMessge.Show ("총점보다 작성된 배점이 일치하지 않아 총점을 변경합니다.");

					totalPoint = userInputPoint + (totalCount - userInputCount);
					atomAttrib.TotalPoint = totalPoint;
					TotalAnswerPointBox.Text = $"{atomAttrib.TotalPoint}";
				}

				foreach (var node in atomAttrib.SerializeAnswerData)
				{
					if (true == pointMap.ContainsKey (node.Name))
					{
						node.Point = pointMap[node.Name];
					}
					else
					{
						node.Point = basePoint;
					}
				}
			}
		}

		#endregion

		#region | LoadAnswerPoint  |

		private void LoadAnswerPoint ()
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var atomList = GetAllAtomCores ();

			foreach (var node in atomAttrib.SerializeAnswerData)
			{
				var point = node.Point;

				if (Kiss.DoubleEqual (0, point) || 0 == node.Values.Count)
					continue;

				var atomPoint = node.Point / node.Values.Count;

				foreach (var valueNode in node.Values)
				{
					var targetAtom = atomList.Find (i => i.GetProperVar () == valueNode.AtomName);

					if (null != targetAtom)
					{
						targetAtom.SetContentString (atomPoint.ToString (), false);
					}
				}
			}
		}

		#endregion

		//private void AnswerInputMode_Click (object sender, RoutedEventArgs e)
		//{
		//	SetModeChanged (1);
		//}

		private void SetContentRunMode (int runMode)
		{
			ArrayList alAllTabViewAtoms = GetAllAtomsInTabView ();

			foreach (AtomBase atom in alAllTabViewAtoms)
			{
				if (null == atom.AtomCore)
					continue;

				if (0 == runMode)
				{
					atom.AtomCore.ClearContent (false, 0, false);
				}

				atom.ChangeAtomMode (runMode);

				if (1 == runMode)
				{
					atom.AtomCore.ReMakeFieldIndex ();
				}
			}

			EBookQuizViewControl.ChangeAtomMode (runMode);

			foreach (AtomBase atom in alAllTabViewAtoms)
			{
				if (0 == runMode)
				{
					atom.DoPostEditMode ();
				}
				else
				{
					atom.DoPostRunMode ();
				}
			}
		}

		public override void SetResizeAdornerVisibility (Visibility isVisible, bool bIsRoutedChildren)
		{
			base.SetResizeAdornerVisibility (isVisible, bIsRoutedChildren);
		}

		#region | 채점 |

		public void ExecuteAnswer ()
		{
			var atomCore = this.AtomCore as EBookQuizViewAtom;
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;

			var inputNodes = MakeInputNodes ();
			var answerNodes = new List<EBookQuizAnswerNode> (atomAttrib.SerializeAnswerData);

			IsAnswer = false;

			if (0 == atomAttrib.SerializeAnswerData.Count)
			{
				string msg = "정답이 설정되어 있지 않습니다";

				//2024-07-05 퀴즈메이커에서 만든폼의 경우 퀴즈블록 명칭이 설정되어 있지 않아 이렇게 처리함
				if (false == string.IsNullOrEmpty (AtomCore.Attrib.AtomProperVar))
				{
					msg = $"[{AtomCore.Attrib.AtomProperVar}]에 " + msg;
				}

				ToastMessge.Show (msg);
				return;
			}

			var isAnswerList = atomCore.QuizAnswerEngineManager.ExecuteAnswer (inputNodes, answerNodes);

			atomCore.Point = GetTotalPoint (answerNodes, isAnswerList);

			ShowAnswerEffect (answerNodes, isAnswerList);

			EBookQuizViewControl.QuizViewContainerPanel.RootEBookQuizViewEngine.IsExecuteEngine = false;
		}

		public List<EBookQuizAnswerNode> MakeInputNodes ()
		{
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var inputNodes = new List<EBookQuizAnswerNode> ();

			foreach (KeyValuePair<Atom, EBookQuizPropertyNode> item in atomAttrib.DataMap)
			{
				var targetAtom = item.Key;
				var propertyNode = item.Value;

				if (QuizAction.Answer == propertyNode.ActionType)
				{
					if (atomAttrib.AnswerDataMap.ContainsKey (targetAtom))
					{
						var answerValueNode = atomAttrib.AnswerDataMap[targetAtom];

						var findNode = inputNodes.Find (i => i.Name == answerValueNode.GroupName);

						if (null == findNode)
						{
							findNode = new EBookQuizAnswerNode ()
							{
								Name = answerValueNode.GroupName,
							};

							inputNodes.Add (findNode);
						}

						var inputValueNode = SaveAnswerValue (targetAtom, propertyNode);
						findNode.QuizType = propertyNode.QuizType;

						if (QuizType.E11 == atomAttrib.DisplayQuizType)
						{
							findNode.IsDirection = false;
							findNode.ActionType = atomAttrib.EBookQuizOptionNode.E11.ActionType;

							if (0 == atomAttrib.EBookQuizOptionNode.E11.AnswerRunType)
							{
								//개수 일치
								findNode.IsDirection = true;
							}
						}
						else if (QuizType.C11 == atomAttrib.DisplayQuizType ||
							QuizType.C15 == atomAttrib.DisplayQuizType)
						{
							findNode.IsDirection = false;

							if (0 == atomAttrib.EBookQuizOptionNode.C11.AnswerRunType)
							{
								//부분 일치
								findNode.IsDirection = true;
							}
						}
						else if (QuizType.A31 == atomAttrib.DisplayQuizType ||
							QuizType.A35 == atomAttrib.DisplayQuizType)
						{
							findNode.IsDirection = false;

							if (0 == atomAttrib.EBookQuizOptionNode.A31.AnswerRunType)
							{
								//부분 일치
								findNode.IsDirection = true;
							}
						}

						if (null != inputValueNode)
						{
							findNode.Values.Add (inputValueNode);
						}
					}
				}
			}

			return inputNodes;
		}

		private double GetTotalPoint (List<EBookQuizAnswerNode> nodes, List<bool> isAnswerList)
		{
			double point = 0;

			for (int i = 0; i < nodes.Count; i++)
			{
				if (i < isAnswerList.Count && isAnswerList[i])
				{
					point += nodes[i].Point;
				}
			}

			return point;
		}

		private void ShowAnswerEffect (List<EBookQuizAnswerNode> nodes, List<bool> isAnswerList)
		{
			//정오답 표시
			var atomAttrib = this.AtomCore.GetAttrib () as EBookQuizViewAttrib;
			var answerCount = 0;
			var targetAtomName = "";
			var effectMessage = "";
			var atoms = GetAllAtomCores ();

			var answerActionType = atomAttrib.AnswerActionType;
			var currentPoint = 0.0d;
			var isAnswerType = 2; // 0 : 오답, 1 : 부분정답 :, 2 : 정답

			var questionNodes = new List<EBookQuizPropertyNode> ();
			var answerNodes = new List<EBookQuizPropertyNode> ();

			//EBookQuizPropertyNode questionFirstNode = null;
			//EBookQuizPropertyNode answerFirstNode = null;

			for (int i = 0; i < nodes.Count; i++)
			{
				var node = nodes[i];
				if (true == isAnswerList[i])
				{
					currentPoint += node.Point;
					answerCount++;
				}
			}

			if (atomAttrib.AnswerActionType == 0)
			{
				currentPoint = atomAttrib.TotalPoint;
			}

			foreach (var item in atomAttrib.DataMap)
			{
				if (item.Value.ActionType == QuizAction.Question)
				{
					item.Value.Name = item.Key.GetProperVar ();
					questionNodes.Add (item.Value);
				}
				else if (item.Value.ActionType == QuizAction.Answer)
				{
					item.Value.Name = item.Key.GetProperVar ();
					answerNodes.Add (item.Value);
				}
			}

			AnswerPoint = currentPoint;
			if (isAnswerList.Count == answerCount)
			{
				isAnswerType = 2;
				effectMessage = $"정답입니다. ({currentPoint}점)";
				IsAnswer = true;

				CVariantX[] args = new CVariantX[2];
				args[0] = new CVariantX (1);
				args[1] = new CVariantX (true);

				// 업무규칙 이벤트 논리 보완 필요
				if (-1 != AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_CHECKANSWER_END, args))
				{
					if (0 <= MsgHandler.CALL_MSG_HANDLER (AtomCore, EVS_TYPE.EVS_A_CHECKANSWER_END, args))
					{
						AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_CHECKANSWER_END, args);
					}
				}

				if (-1 != this.AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_CHECKANSWER_SUCCEED))
				{
					if (-1 != this.AtomCore.OnCallMsgHandler (EVS_TYPE.EVS_A_CHECKANSWER_SUCCEED, null))
					{
						this.AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_CHECKANSWER_SUCCEED);
					}
				}
			}
			else if (1 == answerActionType && 0 < answerCount)
			{
				isAnswerType = 1;
				effectMessage = $"부분 정답입니다. ({currentPoint}점)";
			}
			else
			{
				isAnswerType = 0;
				effectMessage = $"오답입니다.";

				CVariantX[] args = new CVariantX[2];
				args[0] = new CVariantX (1);
				args[1] = new CVariantX (false);

				// 업무규칙 이벤트 논리 보완 필요
				if (-1 != AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_CHECKANSWER_END, args))
				{
					if (0 <= MsgHandler.CALL_MSG_HANDLER (AtomCore, EVS_TYPE.EVS_A_CHECKANSWER_END, args))
					{
						AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_CHECKANSWER_END, args);
					}
				}

				if (-1 != this.AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_CHECKANSWER_FAILED))
				{
					if (-1 != this.AtomCore.OnCallMsgHandler (EVS_TYPE.EVS_A_CHECKANSWER_FAILED, null))
					{
						this.AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_CHECKANSWER_FAILED);
					}
				}
			}

			if (false == IsEmbedMode && false == string.IsNullOrEmpty (effectMessage))
			{
				ToastMessge.Show (effectMessage);
			}

			if (0 == answerActionType)
			{
				//일괄채점
				switch (atomAttrib.DisplayQuizType)
				{
					case QuizType.A21: //선긋기
					case QuizType.A25:
						targetAtomName = 0 < questionNodes.Count ? questionNodes[0].Name : "";
						break;
					case QuizType.A11: //선다형
					case QuizType.A15:
					case QuizType.A61: // OX
					case QuizType.A65:
					case QuizType.C11: //단답형
					case QuizType.C15:
					case QuizType.C21: //서술형
					case QuizType.C25:
					case QuizType.E11: //그려넣기
						targetAtomName = 0 < answerNodes.Count ? answerNodes[0].Name : "";
						break;
					default:
						targetAtomName = 0 < answerNodes.Count ? answerNodes[0].Name : "";
						break;
				}

				var targetAtom = atoms.Find (i => i.GetProperVar () == targetAtomName);
				DrawAnswerEffect (targetAtom, isAnswerType);
			}
			else
			{
				//부분채점
				for (int i = 0; i < atomAttrib.SerializeAnswerData.Count; i++)
				{
					var item = atomAttrib.SerializeAnswerData[i];

					if (0 < item.Values.Count)
					{
						var isResult = isAnswerList[i];

						switch (atomAttrib.DisplayQuizType)
						{
							case QuizType.A21: //선긋기
							case QuizType.A25:
								targetAtomName = i < questionNodes.Count ? questionNodes[i].Name : "";
								break;
							case QuizType.C11: //단답형
							case QuizType.C15:
							case QuizType.C21: //서술형
							case QuizType.C25:
							case QuizType.E11: //그려넣기
								targetAtomName = i < answerNodes.Count ? answerNodes[i].Name : "";
								break;
							default:
								targetAtomName = 0 < answerNodes.Count ? answerNodes[i].Name : "";
								break;
						}

						var targetAtom = atoms.Find (j => j.GetProperVar () == targetAtomName);

						if (null != targetAtom)
							DrawAnswerEffect (targetAtom, isResult ? 2 : 0);
					}
				}
			}
		}

		private void DrawAnswerEffect (Atom targetAtom, int isAnswerType)
		{
			if (null != targetAtom)
			{
				var targetAtomName = targetAtom.GetProperVar ();
				if (false == m_AnswerEffectList.ContainsKey (targetAtomName))
				{
					var dx = targetAtom.Attrib.AtomX;
					var dy = targetAtom.Attrib.AtomY;

					var dw = 50d;
					var dh = 50d;

					dx -= dw / 2;
					dy -= dh / 2;

					var effectControl = new Image
					{
						Margin = new Thickness (dx, dy, 0, 0),
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalAlignment = HorizontalAlignment.Left,
						Width = 50,
						Height = 50,
					};

					QuizEffectGrid.Children.Add (effectControl);
					m_AnswerEffectList.Add (targetAtomName, effectControl);
				}

				var imagePath = "";

				if (0 == isAnswerType)
				{
					imagePath = "pack://application:,,,/BOS04;component/Resources/Edutech/Failed.png";
				}
				else
				{
					imagePath = "pack://application:,,,/BOS04;component/Resources/Edutech/Succed.png";
				}

				var imageControl = m_AnswerEffectList[targetAtomName] as Image;

				if (null != imageControl)
					imageControl.Source = new BitmapImage (new Uri (imagePath));
			}
		}

		#endregion

	}
}