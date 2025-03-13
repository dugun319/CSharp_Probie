using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.DesignHelper.Common;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.CommonLib;
using Softpower.SmartMaker.TopApp.Metadata.QuizMaker;
using Softpower.SmartMaker.TopAtom.Components.TabViewAtom;
using Softpower.SmartMaker.TopControl.Components.Dialog;

namespace Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView
{
	/* 2024-10-25 kys
	- 초기 설계에서는 A1 ~ C2 형태로 퀴즈 유형을 설계했고 동작하도록 논리 구현했음
	- 문제 출제기가 추가되면서 동일한 풀이 방식을 가지는 문제임에도 편집기능을 구분하기 위해 A11,  A15 형태로 구분하는 논리가 추가됨
	- 현재 해당 코드에서 A65 유형이면서도 동작은 A61로 하는것 처럼 QuizType을 혼용해서 사용했기 때문에 추후 전체적인 논리 변경이 필요한 상황 
	 */
	public class EBookQuizViewEngine
	{
		private QuizType _ActionQuizType = QuizType.None;

		private Line _CurrentLine = null;
		private Grid _CurrentImageControl = null;

		private TabPage _SourceTabPage = null;

		private EBookQuizViewAttrib _AtomAttrib = null;
		private EBookQuizAnswerPopupControl _EBookQuizAnswerPopupControl = null;

		private AtomBase _TargetAtomBase = null;

		private Dictionary<string, int> _AtomEditColorMap = new Dictionary<string, int> ();
		private Dictionary<Atom, object> _AtomEditValueData = new Dictionary<Atom, object> ();


		private List<Line> _CurrentE1DrawingList = new List<Line> ();

		private Dictionary<Line, bool> _LineAnimateMap = new Dictionary<Line, bool>();		// 애니메이션 적용 여부 확인
		private Dictionary<Line, bool> _LineOverMap = new Dictionary<Line, bool> ();		// 선을 지나갔는지 확인
		private Dictionary<Line, CancellationTokenSource> _LineTimerMap = new Dictionary<Line, CancellationTokenSource> ();		// 선에 충분한 시간을 머물렀는지 확인

		// 선그리기 속성 굵기 애니메이션, original thickness 저장 Map
		private Dictionary<Line, double> _OriginalThicknessMap = new Dictionary<Line, double> ();


		#region | Property |

		public Grid ActionGrid { get; set; } = null;
		public Grid EffectGrid { get; set; } = null;

		public TabPage SourceTabPage
		{
			get { return _SourceTabPage; }
			set
			{
				_SourceTabPage = value;
			}
		}

		public Dictionary<string, int> AtomEditColorMap
		{
			get { return _AtomEditColorMap; }
		}

		public Dictionary<AtomBase, FrameworkElement> LineQuizQuestionList { get; set; } = new Dictionary<AtomBase, FrameworkElement> ();
		public Dictionary<AtomBase, FrameworkElement> LineQuizAnswerList { get; set; } = new Dictionary<AtomBase, FrameworkElement> ();
		public Dictionary<AtomBase, FrameworkElement> AnswerCheckControlMap { get; set; } = new Dictionary<AtomBase, FrameworkElement> ();
		public Dictionary<string, EBookQuizElementAtomBase> QuizElementMap { get; set; } = new Dictionary<string, EBookQuizElementAtomBase> ();
		public Dictionary<string, Canvas> DrawCanvasMap { get; set; } = new Dictionary<string, Canvas> ();

		public EBookQuizViewAttrib AtomAttrib
		{
			get { return _AtomAttrib; }
			set { _AtomAttrib = value; }
		}

		public bool IsExecuteEngine { get; set; } = true;

		#endregion

		public EBookQuizViewEngine ()
		{
			InitStyle ();
			InitEvent ();
		}

		private void InitStyle ()
		{
			_EBookQuizAnswerPopupControl = new EBookQuizAnswerPopupControl ();
			_EBookQuizAnswerPopupControl.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
			_EBookQuizAnswerPopupControl.StaysOpen = false;
		}

		private void InitEvent ()
		{
			_EBookQuizAnswerPopupControl.OnNotifyMenuClick += EBookQuizAnswerPopupControl_OnNotifyMenuClick;
		}

		public void ExecuteMouseDown (MouseButtonEventArgs e)
		{
			if (false == IsExecuteEngine)
				return;

			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);
			var quizType = GetQuizType (atom);

			switch (quizType)
			{
				case QuizType.A11: //선다형
				case QuizType.A15:
					ExecuteMouseDown_A1 (e);
					break;
				case QuizType.A21: //선긋기
				case QuizType.A25:
					ExecuteMouseDown_A2 (e);
					break;
				case QuizType.A31: //끌어놓기
				case QuizType.A35:
					ExecuteMouseDown_A3 (e);
					break;
				case QuizType.A51: //순서맞춤
				case QuizType.A55:
					ExecuteMouseDown_A5 (e);
					break;
				case QuizType.E11: //그려넣기
					ExecuteMouseDown_E1 (e);
					break;
				case QuizType.A71: //이동하기
					ExecuteMouseDown_A7 (e);
					break;
				default:
					_ActionQuizType = quizType;
					break;
			}
		}

		public void ExecuteMouseUp (MouseButtonEventArgs e)
		{
			if (false == IsExecuteEngine)
				return;

			if (e.ChangedButton == MouseButton.Left)
			{
				switch (_ActionQuizType)
				{
					case QuizType.A11: //선다형
					case QuizType.A15:
						ExecuteMouseUp_A1 (e);
						break;
					case QuizType.A21: //선긋기
					case QuizType.A25:
						ExecuteMouseUp_A2 (e);
						break;
					case QuizType.A31: //끌어놓기
					case QuizType.A35:
						ExecuteMouseUp_A3 (e);
						break;
					case QuizType.A51: //순서맞춤
					case QuizType.A55:
						ExecuteMouseUp_A5 (e);
						break;
					case QuizType.A61: //OX퀴즈
					case QuizType.A65:
						ExecuteMouseUp_A6 (e);
						break;
					case QuizType.E11: //그려넣기
						ExecuteMouseUp_E1 (e);
						break;
					case QuizType.A71: //이동하기
						ExecuteMouseUp_A7 (e);
						break;
				}
			}
			else if (e.ChangedButton == MouseButton.Right)
			{
				switch (_ActionQuizType)
				{
					case QuizType.A21: //선긋기
					case QuizType.A25:
						ExecuteMouseUp_A2 (e);
						break;
					case QuizType.A31: //끌어놓기
					case QuizType.A35:
						ExecuteMouseUp_A3 (e);
						break;
					case QuizType.A51:
					case QuizType.A55:
						ExecuteMouseUp_A5 (e);
						break;
					case QuizType.A61: ///OX
					case QuizType.A65:
						ExecuteMouseRightUp_A6 (e);
						break;
					case QuizType.E11: //그려넣기
						ExecuteMouseRightUp_E1 (e);
						break;
					case QuizType.A71: //이동하기
						break;
				}
			}
		}

		public void ExecuteMouseMove (MouseEventArgs e)
		{
			if (false == IsExecuteEngine)
				return;

			if (e.LeftButton == MouseButtonState.Pressed)
			{
				switch (_ActionQuizType)
				{
					case QuizType.A11: //선다형
					case QuizType.A15:
						ExecuteMouseMove_A1 (e);
						break;
					case QuizType.A21: //선긋기
					case QuizType.A25:
						ExecuteMouseMove_A2 (e);
						break;
					case QuizType.A31: //끌어놓기
					case QuizType.A35:
						ExecuteMouseMove_A3 (e);
						break;
					case QuizType.A51: //순서맞춤
					case QuizType.A55:
						ExecuteMouseMove_A5 (e);
						break;
					case QuizType.E11: //그려넣기
						ExecuteMouseMove_E1 (e);
						break;
					//case QuizType.E2: //색칠하기
					//	ExecuteMouseMove_E2 (e);
					//	break;
					case QuizType.A71: //색칠하기
						ExecuteMouseMove_A7 (e);
						break;
				}
			}
		}

		/*
		 * ExecuteMouseDown (MouseButtonEventArgs e)
		 * ExecuteMouseMove (MouseEventArgs e)
		 * ExecuteMouseUp (MouseButtonEventArgs e)
		 */

		#region | 선다형 (A1) |

		private void ExecuteMouseDown_A1 (MouseButtonEventArgs e)
		{
			_ActionQuizType = QuizType.A11;
		}

		private void ExecuteMouseMove_A1 (MouseEventArgs e)
		{

		}

		private void ExecuteMouseUp_A1 (MouseButtonEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);
			var atomCore = atom?.AtomCore;

			if (null == atom || null == atomCore)
				return;

			if (false == AtomAttrib.DataMap.ContainsKey (atomCore))
				return;

			var node = AtomAttrib.DataMap[atomCore];

			if (node.ActionType == QuizAction.Answer)
			{
				var quizType = GetQuizType (atomCore.AtomBase);
				var isMultiAnswer = false;

				if (quizType == QuizType.A11)
				{
					isMultiAnswer = AtomAttrib.EBookQuizOptionNode.A11.IsMultiAnswer;
				}
				else if (quizType == QuizType.A15)
				{
					isMultiAnswer = AtomAttrib.EBookQuizOptionNode.A15.IsMultiAnswer;
				}
				else
				{
					PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
				}

				if (false == isMultiAnswer)
				{
					foreach (var item in AnswerCheckControlMap)
					{
						ActionGrid.Children.Remove (item.Value);
					}

					AnswerCheckControlMap.Clear ();
				}

				MakeCheckControl (atom);
			}
		}

		public void MakeCheckControl (AtomBase targetAtom)
		{
			var targetAtomCore = targetAtom.AtomCore;
			var atomName = targetAtomCore.GetProperVar ();
			var quizType = AtomAttrib.DisplayQuizType;

			var answerEffect = 0;

			var answerEffectWidth = 0d;
			var answerEffectHeight = 0d;
			var isMultiAnswer = false;

			if (quizType == QuizType.A11)
			{
				answerEffect = AtomAttrib.EBookQuizOptionNode.A11.AnswerEffect;
				answerEffectWidth = AtomAttrib.EBookQuizOptionNode.A11.AnswerEffectWidth;
				answerEffectHeight = AtomAttrib.EBookQuizOptionNode.A11.AnswerEffectHeight;
				isMultiAnswer = AtomAttrib.EBookQuizOptionNode.A11.IsMultiAnswer;
			}
			else if (quizType == QuizType.A15)
			{
				answerEffect = AtomAttrib.EBookQuizOptionNode.A15.AnswerEffect;
				answerEffectWidth = AtomAttrib.EBookQuizOptionNode.A15.AnswerEffectWidth;
				answerEffectHeight = AtomAttrib.EBookQuizOptionNode.A15.AnswerEffectHeight;
				isMultiAnswer = AtomAttrib.EBookQuizOptionNode.A15.IsMultiAnswer;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			var dx = targetAtomCore.Attrib.AtomX + (targetAtomCore.Attrib.AtomWidth / 2 - answerEffectWidth / 2);
			var dy = targetAtomCore.Attrib.AtomY + (targetAtomCore.Attrib.AtomHeight / 2 - answerEffectHeight / 2);

			// 수직정렬 고려
			if (targetAtomCore.Attrib is SquareAttrib)
			{
				SquareAttrib squareAttrib = targetAtomCore.Attrib as SquareAttrib;
				if (squareAttrib.LineAlignment == System.Drawing.StringAlignment.Near)
					dy = targetAtomCore.Attrib.AtomY - (targetAtomCore.Attrib.AtomHeight - answerEffectHeight);
				else if (squareAttrib.LineAlignment == System.Drawing.StringAlignment.Far)
					dy = targetAtomCore.Attrib.AtomY + (targetAtomCore.Attrib.AtomHeight - answerEffectHeight);
			}
			//

			if (AnswerCheckControlMap.ContainsKey (targetAtom))
			{
				ActionGrid.Children.Remove (AnswerCheckControlMap[targetAtom]);
				AnswerCheckControlMap.Remove (targetAtom);
				return;
			}

			if (false == isMultiAnswer)
			{
				foreach (var item in AnswerCheckControlMap)
				{
					ActionGrid.Children.Remove (item.Value);
				}

				AnswerCheckControlMap.Clear ();
			}

			var grid = new Grid
			{
				Margin = new Thickness (dx, dy, 0, 0),
				Width = answerEffectWidth,
				Height = answerEffectHeight,
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Left,
			};

			if (0 == answerEffect)
			{
				var ellipse = new Ellipse
				{
					Stroke = Brushes.Red,
				};

				grid.Children.Add (ellipse);
			}
			else if (1 == answerEffect)
			{
				var Path = new System.Windows.Shapes.Path
				{
					Stroke = Brushes.Red,
					Data = Geometry.Parse ("M3,11 10,17 21,6"),
				};

				grid.Children.Add (Path);
			}

			AnswerCheckControlMap.Add (targetAtom, grid);
			ActionGrid.Children.Add (grid);
		}

		#endregion

		#region | 선긋기 (A2) |

		private void ExecuteMouseDown_A2 (MouseButtonEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);

			if (null == atom)
				return;

			ExecuteMouseDown_A2 (atom);
		}

		public void ExecuteMouseDown_A2 (AtomBase atom)
		{
			var quizType = GetQuizType (atom);

			ActionGrid.Children.Clear ();

			_ActionQuizType = QuizType.A21;
			var node = GetPropertyNode (atom);
			var atomName = atom.AtomCore.GetProperVar ();

			if (QuizAction.Question == node.ActionType)
			{
				FrameworkElement targetElement = atom;
				_TargetAtomBase = atom;

				if (true == LineQuizAnswerList.ContainsKey (atom))
				{
					targetElement = LineQuizAnswerList[atom];
				}
				else if (true == LineQuizQuestionList.ContainsKey (atom))
				{
					targetElement = LineQuizQuestionList[atom];
				}

				double dx = targetElement.Margin.Left + (targetElement.ActualWidth / 2);
				double dy = targetElement.Margin.Top + (targetElement.ActualHeight / 2);

				var lineColor = unchecked((int)0xFF000000);
				var lineType = 0;
				double lineThickness = 3;

				if (quizType == QuizType.A21)
				{
					lineColor = AtomAttrib.EBookQuizOptionNode.A21.LineColor;
					lineType = AtomAttrib.EBookQuizOptionNode.A21.LineType;
					if(AtomAttrib.EBookQuizOptionNode.A21.LineThickness > 0)
					{
						lineThickness = AtomAttrib.EBookQuizOptionNode.A21.LineThickness;
					}
				}

				else if (quizType == QuizType.A25)
				{
					lineColor = AtomAttrib.EBookQuizOptionNode.A25.LineColor;
					lineType = AtomAttrib.EBookQuizOptionNode.A25.LineType;
					if (AtomAttrib.EBookQuizOptionNode.A25.LineThickness > 0)
					{
						lineThickness = AtomAttrib.EBookQuizOptionNode.A25.LineThickness;
					}
				}
				else
					PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");

				_CurrentLine = new Line ()
				{
					X1 = dx,
					Y1 = dy,
					X2 = dx,
					Y2 = dy,
					VerticalAlignment = System.Windows.VerticalAlignment.Top,
					HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
					Stroke = WPFColorConverter.ConvertArgbToMediaBrush (lineColor),
					StrokeThickness = lineThickness,
					StrokeStartLineCap = PenLineCap.Round,
					StrokeEndLineCap = PenLineCap.Round,
				};

				if (1 == lineType)
				{
					_CurrentLine.StrokeDashArray = new DoubleCollection () { 7, 7, 7, 7 };
				}

				_CurrentLine.MouseLeftButtonUp -= CurrentLine_MouseLeftButtonUpA2;
				_CurrentLine.MouseLeftButtonUp += CurrentLine_MouseLeftButtonUpA2;

				_CurrentLine.Tag = atom.AtomCore.GetProperVar ();

				ActionGrid.Children.Add (_CurrentLine);
			}
		}

		private void CurrentLine_MouseLeftButtonUpA2 (object sender, MouseButtonEventArgs e)
		{
			ExecuteMouseUp_A2 (e);
		}

		private void ExecuteMouseUp_A2 (MouseButtonEventArgs e)
		{
			if (null != _CurrentLine)
			{
				var pt = e.GetPosition (ActionGrid);
				var atom = ExecuteHitText (pt);

				ExecuteMouseUp_A2 (atom);
			}
		}

		public void ExecuteMouseUp_A2 (AtomBase atom)
		{
			if (null == atom)
			{
				var pt = Mouse.GetPosition (ActionGrid);

				foreach (var item in LineQuizAnswerList)
				{
					var element = item.Value;

					if (element.Margin.Left <= pt.X && pt.X <= element.Margin.Left + element.ActualWidth &&
						element.Margin.Top <= pt.Y && pt.Y <= element.Margin.Top + element.ActualHeight)
					{
						atom = item.Key;
					}
				}
			}

			if (null != atom)
			{
				var atomName = atom.AtomCore.GetProperVar ();
				var node = GetPropertyNode (atom);

				if ((QuizType.A21 == node.QuizType || QuizType.A25 == node.QuizType) &&
					QuizAction.Answer == node.ActionType)
				{
					MakeLineA2 (_TargetAtomBase, atom);
				}
			}

			ActionGrid.Children.Remove (_CurrentLine);
			_CurrentLine = null;
		}

		public void MakeLineA2 (AtomBase startAtom, AtomBase endAtom)
		{
			FrameworkElement startElement = null;
			FrameworkElement endElement = null;

			//2025-02-12 kys 특정 상황에서 에러 발생했다는 로그는 있으나, 재현되지 않아 임시로 예외처리 진행함
			if (null == startAtom || null == endAtom)
			{
				ToastMessge.Show ("선긋기 정답 데이터 설정 중 에러가 발생했습니다.");

				if (null != _CurrentLine)
				{
					ActionGrid.Children.Remove (_CurrentLine);
					_CurrentLine = null;
				}

				return;
			}

			if (null == startElement)
			{
				startElement = LineQuizQuestionList.ContainsKey (startAtom) ? LineQuizQuestionList[startAtom] : startAtom;
			}

			if (null == endElement)
			{
				endElement = LineQuizAnswerList.ContainsKey (endAtom) ? LineQuizAnswerList[endAtom] : endAtom;
			}

			var quizType = this.AtomAttrib.DisplayQuizType;
			var startAtomName = startAtom.AtomCore.GetProperVar ();
			var endAtomName = endAtom.AtomCore.GetProperVar ();

			double x1 = startElement.Margin.Left + (startElement.ActualWidth / 2);
			double y1 = startElement.Margin.Top + (startElement.ActualHeight / 2);

			double x2 = endElement.Margin.Left + (endElement.ActualWidth / 2);
			double y2 = endElement.Margin.Top + (endElement.ActualHeight / 2);

			var lineColor = unchecked((int)0xFF000000);
			var lineType = 0;
			double lineThickness = 3;

			if (quizType == QuizType.A21)
			{
				lineColor = AtomAttrib.EBookQuizOptionNode.A21.LineColor;
				lineType = AtomAttrib.EBookQuizOptionNode.A21.LineType;
				if (AtomAttrib.EBookQuizOptionNode.A21.LineThickness > 0)
				{
					lineThickness = AtomAttrib.EBookQuizOptionNode.A21.LineThickness;
				}
			}
			else if (quizType == QuizType.A25)
			{
				lineColor = AtomAttrib.EBookQuizOptionNode.A25.LineColor;
				lineType = AtomAttrib.EBookQuizOptionNode.A25.LineType;
				if (AtomAttrib.EBookQuizOptionNode.A25.LineThickness > 0)
				{
					lineThickness = AtomAttrib.EBookQuizOptionNode.A25.LineThickness;
				}
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			var line = new Line
			{
				X1 = x1,
				Y1 = y1,
				X2 = x2,
				Y2 = y2,
				Tag = $"{startAtomName},{endAtomName}",
				VerticalAlignment = System.Windows.VerticalAlignment.Top,
				HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
				Stroke = WPFColorConverter.ConvertArgbToMediaBrush (lineColor),
				StrokeThickness = lineThickness,
				StrokeStartLineCap = PenLineCap.Round,
				StrokeEndLineCap = PenLineCap.Round,
			};

			line.MouseRightButtonUp -= Line_MouseRightButtonUp;
			line.MouseRightButtonUp += Line_MouseRightButtonUp;

			line.MouseEnter -= Line_MouseEnter;
			line.MouseEnter += Line_MouseEnter;

			line.MouseLeave -= Line_MouseLeave;
			line.MouseLeave += Line_MouseLeave;

			if (1 == lineType)
			{
				line.StrokeDashArray = new DoubleCollection () { 7, 7, 7, 7 };
			}

			bool isExists = false;

			foreach (var drawLine in EffectGrid.Children.OfType<Line> ())
			{
				if (line.Tag?.ToString () == drawLine.Tag?.ToString ())
				{
					isExists = true;
					break;
				}
			}

			if (false == isExists)
			{
				EffectGrid.Children.Add (line);
			}
		}

		private void Line_MouseRightButtonUp (object sender, MouseButtonEventArgs e)
		{
			var line = sender as Line;
			EffectGrid.Children.Remove (line);
		}

		public void ExecuteMouseMove_A2 (MouseEventArgs e)
		{
			if (null != _CurrentLine)
			{
				var pt = e.GetPosition (ActionGrid);

				_CurrentLine.X2 = pt.X;
				_CurrentLine.Y2 = pt.Y;
			}
		}

		public void ExecuteMouseMove_A2 (AtomBase atom)
		{
			if (null != _CurrentLine)
			{
				var pt = Mouse.GetPosition (ActionGrid);

				_CurrentLine.X2 = pt.X;
				_CurrentLine.Y2 = pt.Y;
			}
		}

		private async void Line_MouseEnter (object sender, MouseEventArgs e)
		{
			var line = sender as Line;
			if (line == null) return;

			// 원래 두께를 한 번만 저장 (없을 경우 추가)
			if (!_OriginalThicknessMap.ContainsKey (line))
				_OriginalThicknessMap[line] = line.StrokeThickness;

			// 이미 애니메이션이 적용된 경우 다시 적용하지 않음
			if (_LineAnimateMap.ContainsKey (line) && _LineAnimateMap[line])
				return;

			// 중복실행 및 마우스가 움직였을 때 무분별하게 반응하는 것 방지
			// 새로운 CancellationTokenSource 생성 및 저장
			if (_LineTimerMap.ContainsKey (line))
			{
				_LineTimerMap[line].Cancel ();  // 기존 타이머 취소
				_LineTimerMap.Remove (line);
			}

			// 100ms 동안 머무르는지 확인
			CancellationTokenSource cts = new CancellationTokenSource ();
			_LineTimerMap[line] = cts;  // 타이머 저장

			try
			{
				await Task.Delay (100, cts.Token);  // 100ms 대기
			}
			catch (TaskCanceledException)
			{
				return;
			}

			// 마우스 버튼이 눌리지 않은 상태인지 확인
			if (Mouse.LeftButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Released)
			{
				// 마우스가 같은 선 위에 있는지 확인 후 true로 설정
				if (!_LineOverMap.ContainsKey (line))
					_LineOverMap[line] = true;
				else if (!_LineOverMap[line])
					_LineOverMap[line] = true;

				// 마우스가 같은 선 위에 있는지 확인
				if (_LineOverMap[line])
				{
					// 원래 두께로 복구 (저장된 두께 사용)
					double originalThickness = _OriginalThicknessMap[line];

					// 선의 굵기에 따라 애니메이션 배율 계산 (1에서 9까지 굵기에 비례)
					double scaleFactor = 2 - (originalThickness - 1) / 10;

					LineAnimateThickness (line, originalThickness, (originalThickness * scaleFactor));

					_LineAnimateMap[line] = true;
				}				
			}
			// 타이머 제거
			_LineTimerMap.Remove (line);
		}

		private async void Line_MouseLeave (object sender, MouseEventArgs e)
		{
			var line = sender as Line;
			if (line == null) return;

			// 기존 타이머가 있으면 취소 후 삭제 (마우스가 나가면 대기 중인 애니메이션 실행 안 함)
			if (_LineTimerMap.ContainsKey (line))
			{
				_LineTimerMap[line].Cancel ();
				_LineTimerMap.Remove (line);
			}

			// 현재 애니메이션 실행 중인지 확인 (중복 실행 방지)
			if (_LineAnimateMap.ContainsKey (line) && _LineAnimateMap[line])
			{ 
				// 원래 두께로 복구 (저장된 두께 사용)
				double originalThickness = _OriginalThicknessMap.ContainsKey (line) 
					? _OriginalThicknessMap[line]
					: line.StrokeThickness / 2;
				
				await Task.Delay (50);
				LineAnimateThickness (line, line.StrokeThickness, originalThickness);

				// 애니메이션 진행 상태 업데이트
				_LineAnimateMap[line] = false;					
			}
			if (_LineOverMap.ContainsKey (line))
				_LineOverMap[line] = false;
		}

		private void LineAnimateThickness (Line line, double from, double to)
		{
			DoubleAnimation thicknessAnimation = new DoubleAnimation
			{
				From = from,
				To = to,
				Duration = TimeSpan.FromSeconds (0.5),
				EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
			};
		
			line.BeginAnimation (Shape.StrokeThicknessProperty, thicknessAnimation);
		}
		#endregion

		#region | 끌어놓기 (A3) |

		private void ExecuteMouseDown_A3 (MouseButtonEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);

			if (null == atom)
				return;

			ActionGrid.Children.Clear ();

			_ActionQuizType = QuizType.A31;
			var node = GetPropertyNode (atom);

			if (QuizAction.Question == node.ActionType)
			{
				double dx = pt.X - (atom.ActualWidth / 2);
				double dy = pt.Y - (atom.ActualHeight / 2);

				var brush = new VisualBrush (atom);
				brush.Stretch = Stretch.None;

				_TargetAtomBase = atom;

				_CurrentImageControl = new Grid
				{
					Margin = new Thickness (dx, dy, 0, 0),
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Left,
					Background = brush,
					Width = atom.ActualWidth,
					Height = atom.ActualHeight,
				};

				_CurrentImageControl.MouseUp += CurrentImageControl_MouseUp;
				_CurrentImageControl.MouseMove += CurrentImageControl_MouseMove;

				Grid.SetZIndex (_CurrentImageControl, 1000);
				ActionGrid.Children.Add (_CurrentImageControl);
			}
		}

		private void ExecuteMouseDown_A5 (MouseButtonEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);

			if (null == atom)
				return;

			ActionGrid.Children.Clear ();

			_ActionQuizType = QuizType.A51;
			var node = GetPropertyNode (atom);

			if (QuizAction.Question == node.ActionType)
			{
				double dx = pt.X - (atom.ActualWidth / 2);
				double dy = pt.Y - (atom.ActualHeight / 2);

				var brush = new VisualBrush (atom);
				brush.Stretch = Stretch.None;

				_TargetAtomBase = atom;

				_CurrentImageControl = new Grid
				{
					Margin = new Thickness (dx, dy, 0, 0),
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Left,
					Background = brush,
					Width = atom.ActualWidth,
					Height = atom.ActualHeight,
				};

				_CurrentImageControl.MouseUp += CurrentImageControl_MouseUp;
				_CurrentImageControl.MouseMove += CurrentImageControl_MouseMove;

				Grid.SetZIndex (_CurrentImageControl, 1000);
				ActionGrid.Children.Add (_CurrentImageControl);
			}
		}

		private void CurrentImageControl_MouseMove (object sender, MouseEventArgs e)
		{
			ExecuteMouseMove (e);
		}

		private void CurrentImageControl_MouseUp (object sender, MouseButtonEventArgs e)
		{
			ExecuteMouseUp (e);
		}

		private void ExecuteMouseUp_A3 (MouseButtonEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);

			if (null != atom && null != _CurrentImageControl)
			{
				var node = GetPropertyNode (atom);
				var quizType = node.QuizType;
				if ((QuizType.A31 == quizType || QuizType.A35 == quizType) &&
					QuizAction.Answer == node.ActionType)
				{
					ActionGrid.Children.Remove (_CurrentImageControl);
					var actionType = 0;

					if (quizType == QuizType.A31)
					{
						actionType = AtomAttrib.EBookQuizOptionNode.A31.ActionType;
					}
					else if (quizType == QuizType.A35)
					{
						actionType = AtomAttrib.EBookQuizOptionNode.A35.ActionType;
					}
					else
					{
						PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
					}

					var startAtom = _TargetAtomBase.AtomCore;
					var endAtom = atom.AtomCore;

					if (0 == actionType)
					{
						//확정이동
						SetDragAtomData (startAtom, endAtom);
					}
					else if (1 == actionType)
					{
						//영역이동
						CopyDragAtomData (startAtom, endAtom);
					}

					_CurrentImageControl = null;
				}
				else
				{
					ActionGrid.Children.Remove (_CurrentImageControl);
					_CurrentImageControl = null;
				}
			}
			else
			{
				ActionGrid.Children.Remove (_CurrentImageControl);
				_CurrentImageControl = null;
			}
		}

		public void ExecuteMouseMove_A3 (MouseEventArgs e)
		{
			if (null != _CurrentImageControl)
			{
				var pt = e.GetPosition (ActionGrid);
				_CurrentImageControl.Margin = new Thickness (
					pt.X - _CurrentImageControl.Width / 2,
					pt.Y - _CurrentImageControl.Height / 2,
					0, 0);
			}
		}

		private void ExecuteMouseUp_A5 (MouseButtonEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);

			if (null != atom && null != _CurrentImageControl)
			{
				var node = GetPropertyNode (atom);

				if ((QuizType.A51 == node.QuizType || QuizType.A55 == node.QuizType) &&
					QuizAction.Answer == node.ActionType)
				{
					ActionGrid.Children.Remove (_CurrentImageControl);

					var startAtom = _TargetAtomBase.AtomCore;
					var endAtom = atom.AtomCore;

					//확정이동
					SetDragAtomData (startAtom, endAtom);
					_CurrentImageControl = null;
				}
				else
				{
					ActionGrid.Children.Remove (_CurrentImageControl);
					_CurrentImageControl = null;
				}
			}
			else
			{
				ActionGrid.Children.Remove (_CurrentImageControl);
				_CurrentImageControl = null;
			}
		}

		public void ExecuteMouseMove_A5 (MouseEventArgs e)
		{
			if (null != _CurrentImageControl)
			{
				var pt = e.GetPosition (ActionGrid);
				_CurrentImageControl.Margin = new Thickness (
					pt.X - _CurrentImageControl.Width / 2,
					pt.Y - _CurrentImageControl.Height / 2,
					0, 0);
			}
		}

		public void SetDragAtomData (Atom startAtom, Atom endAtom)
		{
			if (startAtom is DecorImageAtom && endAtom is DecorImageAtom)
			{
				//이미지 정보 전달
				DecorImageAttrib startImageAttrib = startAtom.GetAttrib () as DecorImageAttrib;
				DecorImageAttrib endImageAttrib = endAtom.GetAttrib () as DecorImageAttrib;

				if (false == _AtomEditValueData.ContainsKey (endAtom))
					_AtomEditValueData.Add (endAtom, $"{endImageAttrib.ImageKey}${endImageAttrib.ImagePath}");

				endImageAttrib.ImageKey = startImageAttrib.ImageKey;
				endImageAttrib.ImagePath = startImageAttrib.ImagePath;
				endAtom.CompletePropertyChanged ();
			}
			else if (startAtom is EBookTextAtom && endAtom is EBookTextAtom)
			{
				EBookTextAtom startTextAtom = startAtom as EBookTextAtom;
				EBookTextAtom endTextAtom = endAtom as EBookTextAtom;

				EBookTextofAtom startTextofAtom = startTextAtom.GetOfAtom () as EBookTextofAtom;
				EBookTextofAtom endTextofAtom = endTextAtom.GetOfAtom () as EBookTextofAtom;

				FlowDocument startFlowDocument = startTextofAtom.GetFlowDocument ();
				FlowDocument endFlowDocument = endTextofAtom.GetFlowDocument ();

				if (false == _AtomEditValueData.ContainsKey (endAtom))
				{
					using (MemoryStream rtfMemory = new MemoryStream ())
					using (MemoryStream xamlMemory = new MemoryStream ())
					{
						TextRange loadRange = new TextRange (endFlowDocument.ContentStart, endFlowDocument.ContentEnd);
						loadRange.Save (rtfMemory, System.Windows.DataFormats.Rtf);
						loadRange.Save (xamlMemory, System.Windows.DataFormats.XamlPackage);

						_AtomEditValueData.Add (endAtom, new List<byte[]> () { rtfMemory.ToArray (), xamlMemory.ToArray () });
					}
				}

				using (MemoryStream rtfMemory = new MemoryStream ())
				using (MemoryStream xamlMemory = new MemoryStream ())
				{
					TextRange saveRange = new TextRange (startFlowDocument.ContentStart, startFlowDocument.ContentEnd);
					saveRange.Save (rtfMemory, System.Windows.DataFormats.Rtf);
					saveRange.Save (xamlMemory, System.Windows.DataFormats.XamlPackage);

					TextRange loadRange = new TextRange (endFlowDocument.ContentStart, endFlowDocument.ContentEnd);
					loadRange.Load (rtfMemory, System.Windows.DataFormats.Rtf);
					loadRange.Load (xamlMemory, System.Windows.DataFormats.XamlPackage);
				};
			}
			else
			{
				if (false == _AtomEditValueData.ContainsKey (endAtom))
					_AtomEditValueData.Add (endAtom, endAtom.GetContentString (false));

				endAtom.SetContentString (startAtom.GetContentString (false), true);
			}
		}

		public void SetTextAtomData (Atom atom, string value)
		{
			if (false == _AtomEditValueData.ContainsKey (atom))
				_AtomEditValueData.Add (atom, atom.GetContentString (false));

			atom.SetContentString (value, true);
		}

		private void CopyDragAtomData (Atom startAtom, Atom endAtom)
		{
			EBookQuizElementCanvasofAtom elementofAtom = null;

			var endAtomBase = endAtom.GetOfAtom ();
			var endAtomName = endAtom.GetProperVar ();

			if (false == QuizElementMap.ContainsKey (endAtomName))
			{
				var elementAtom = new EBookQuizElementCanvasofAtom ()
				{
					Margin = endAtomBase.Margin,
					Width = endAtomBase.Width,
					Height = endAtomBase.Height,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Left,
				};

				var zindex = Grid.GetZIndex (endAtomBase);
				Grid.SetZIndex (elementAtom, zindex);

				_SourceTabPage.RootCanvas.Children.Add (elementAtom);
				QuizElementMap.Add (endAtomName, elementAtom);
			}

			elementofAtom = QuizElementMap[endAtomName] as EBookQuizElementCanvasofAtom;

			if (null != elementofAtom)
			{
				var pt = Mouse.GetPosition (endAtomBase);
				_CurrentImageControl.Margin = new Thickness (pt.X - _CurrentImageControl.Width / 2, pt.Y - _CurrentImageControl.Height / 2, 0, 0);

				elementofAtom.ContentGrid.Children.Add (_CurrentImageControl);
			}
		}

		#endregion

		#region | A6 |

		public void ExecuteMouseUp_A6 (MouseButtonEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);

			if (null == atom)
				return;

			var node = GetPropertyNode (atom);

			if (QuizAction.Answer != node.ActionType)
				return;

			var atomName = atom.AtomCore.GetProperVar ();
			var quizType = node.QuizType;
			if (false == QuizElementMap.ContainsKey (atomName))
			{
				var toolType = 0;

				if (quizType == QuizType.A61)
				{
					toolType = AtomAttrib.EBookQuizOptionNode.A61.ToolType;
				}
				else if (quizType == QuizType.A65)
				{
					toolType = AtomAttrib.EBookQuizOptionNode.A65.ToolType;
				}
				else
				{
					PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
				}

				if (2 == toolType)
				{
					int count = 1;
					foreach (var item in QuizElementMap)
					{
						if (item.Value is EBookQuizElementContentofAtom)
						{
							count++;
						}
					}

					MakeContentElement_A6 (atom, count.ToString ());
				}
				else
				{
					MakeContentElement_A6 (atom);
				}
			}
		}

		public void ExecuteMouseRightUp_A6 (MouseButtonEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);

			if (null == atom)
				return;

			var node = GetPropertyNode (atom);

			if (QuizAction.Answer != node.ActionType)
				return;

			var quizType = node.QuizType;

			_TargetAtomBase = atom;
			_ActionQuizType = QuizType.A61;

			if (quizType == QuizType.A61 ||
				quizType == QuizType.A65)
			{
				var toolType = 0;

				if (quizType == QuizType.A61)
				{
					toolType = AtomAttrib.EBookQuizOptionNode.A61.ToolType;
				}
				else if (quizType == QuizType.A65)
				{
					toolType = AtomAttrib.EBookQuizOptionNode.A65.ToolType;
				}
				else
				{
					PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
				}

				if (2 == toolType)
				{
					//숫자입력모드일때 우클릭시 입력값 초기화
					var atomName = atom.AtomCore.GetProperVar ();

					if (true == QuizElementMap.ContainsKey (atomName))
					{
						var contentAtom = QuizElementMap[atomName] as EBookQuizElementContentofAtom;

						if (null != contentAtom)
						{
							QuizElementMap.Remove (atomName);

							if (contentAtom.Parent is Panel panel)
							{
								panel.Children.Remove (contentAtom);
							}
						}
					}
				}
				else
				{
					_EBookQuizAnswerPopupControl.SetPopup (quizType, AtomAttrib);
					_EBookQuizAnswerPopupControl.PlacementTarget = atom;
					_EBookQuizAnswerPopupControl.IsOpen = true;
					e.Handled = true;
				}
			}
		}

		private void EBookQuizAnswerPopupControl_OnNotifyMenuClick (string strValue)
		{
			MakeContentElement_A6 (_TargetAtomBase, strValue);
		}

		public void MakeContentElement_A6 (AtomBase targetAtomBase, string strValue = "")
		{
			var targetAtomCore = targetAtomBase.AtomCore;
			var targetName = targetAtomBase.AtomCore.GetProperVar ();

			var quizType = this.AtomAttrib.DisplayQuizType;
			var itemList = new List<string> ();

			if (quizType == QuizType.A61)
			{
				if (false == _EBookQuizAnswerPopupControl.ItemMap.ContainsKey (QuizType.A61))
				{
					_EBookQuizAnswerPopupControl.SetPopup (QuizType.A61, AtomAttrib);
				}

				itemList = _EBookQuizAnswerPopupControl.ItemMap[QuizType.A61];
			}
			else if (quizType == QuizType.A65)
			{
				if (false == _EBookQuizAnswerPopupControl.ItemMap.ContainsKey (QuizType.A65))
				{
					_EBookQuizAnswerPopupControl.SetPopup (QuizType.A65, AtomAttrib);
				}

				itemList = _EBookQuizAnswerPopupControl.ItemMap[QuizType.A65];
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			if (false == QuizElementMap.ContainsKey (targetName))
			{
				var elementAtom = new EBookQuizElementContentofAtom ()
				{
					Margin = targetAtomBase.Margin,
					Width = targetAtomBase.Width,
					Height = targetAtomBase.Height,
					Cursor = Cursors.Arrow,
				};

				elementAtom.MouseLeftButtonDown -= ElementAtom_MouseLeftButtonDown;
				elementAtom.MouseLeftButtonDown += ElementAtom_MouseLeftButtonDown;

				if (string.IsNullOrEmpty (strValue))
					strValue = itemList[0];

				elementAtom.SetContent (strValue);

				var viewAtomBase = WPFFindChildHelper.FindVisualParent<AtomBase> (_SourceTabPage);
				var fontSize = targetAtomBase.GetAtomFontSize ();
				var fontColor = targetAtomBase.GetAtomFontColor ();

				elementAtom.SetAtomFontSize (fontSize);
				elementAtom.SetAtomFontColor (fontColor);

				Grid.SetZIndex (elementAtom, Grid.GetZIndex (targetAtomBase));

				QuizElementMap.Add (targetName, elementAtom);
				_SourceTabPage.RootCanvas.Children.Add (elementAtom);
			}
			else
			{
				var contentAtom = QuizElementMap[targetName] as EBookQuizElementContentofAtom;
				SetNextValue_A6 (contentAtom);
			}
		}

		private void ElementAtom_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			var atom = sender as EBookQuizElementContentofAtom;
			SetNextValue_A6 (atom);
		}

		private void SetNextValue_A6 (EBookQuizElementContentofAtom atom)
		{
			var quizType = this.AtomAttrib.DisplayQuizType;

			if (_EBookQuizAnswerPopupControl.ItemMap.ContainsKey (quizType))
			{
				var itemList = _EBookQuizAnswerPopupControl.ItemMap[quizType];
				var content = atom.GetContent ();
				var index = itemList.IndexOf (content);
				var newData = index + 1 < itemList.Count ? itemList[index + 1] : itemList[0];

				atom.SetContent (newData);
			}
		}

		#endregion

		#region | 이동하기 (A7) |

		private void ExecuteMouseDown_A7 (MouseButtonEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);

			if (null == atom)
				return;

			ActionGrid.Children.Clear ();

			_ActionQuizType = QuizType.A71;
			var node = GetPropertyNode (atom);

			if (QuizAction.Question == node.ActionType)
			{
				double dx = pt.X - (atom.ActualWidth / 2);
				double dy = pt.Y - (atom.ActualHeight / 2);


				_TargetAtomBase = atom;

				_TargetAtomBase.Margin = new Thickness (dx, dy, 0, 0);
			}
		}

		private void ExecuteMouseMove_A7 (MouseEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);

			double dx = pt.X - (_TargetAtomBase.ActualWidth / 2);
			double dy = pt.Y - (_TargetAtomBase.ActualHeight / 2);

			_TargetAtomBase.Margin = new Thickness (dx, dy, 0, 0);
		}

		private void ExecuteMouseUp_A7 (MouseButtonEventArgs e)
		{

		}

		#endregion

		#region | 그려넣기 (E1) | 

		private void ExecuteMouseDown_E1 (MouseButtonEventArgs e)
		{
			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);

			if (null == atom)
				return;

			_ActionQuizType = QuizType.E11;
			_TargetAtomBase = atom;

			var node = GetPropertyNode (atom);
			var targetElement = atom;

			if (QuizAction.Answer != node.ActionType)
				return;

			var quizType = node.QuizType;
			var dx = targetElement.Margin.Left + (targetElement.ActualWidth / 2);
			var dy = targetElement.Margin.Top + (targetElement.ActualHeight / 2);
			var dw = targetElement.ActualWidth;
			var dh = targetElement.ActualHeight;

			var targetAtomName = _TargetAtomBase.AtomCore.GetProperVar ();

			var lienType = 0;
			var answerColor = 0;
			var actionType = 0;
			var toolType = 0;


			if (quizType == QuizType.E11)
			{
				actionType = AtomAttrib.EBookQuizOptionNode.E11.ActionType;
				lienType = AtomAttrib.EBookQuizOptionNode.E11.LineType;
				answerColor = AtomAttrib.EBookQuizOptionNode.E11.AnswerColor;
				toolType = AtomAttrib.EBookQuizOptionNode.E11.ToolType;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			if (0 == actionType)
			{
				//선 그리기
				_CurrentLine = new Line ()
				{
					X1 = dx,
					Y1 = dy,
					X2 = dx,
					Y2 = dy,
					VerticalAlignment = System.Windows.VerticalAlignment.Top,
					HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
					Stroke = WPFColorConverter.ConvertArgbToMediaBrush (answerColor),
				};

				if (1 == lienType)
				{
					_CurrentLine.StrokeDashArray = new DoubleCollection () { 7, 7, 7, 7 };
				}

				_CurrentLine.MouseLeftButtonUp -= CurrentLine_MouseLeftButtonUpE1;
				_CurrentLine.MouseLeftButtonUp += CurrentLine_MouseLeftButtonUpE1;

				_CurrentLine.Tag = atom.AtomCore.GetProperVar ();

				ActionGrid.Children.Add (_CurrentLine);
			}
			else if (1 == actionType)
			{
				//도형 그리기
				if (false == DrawCanvasMap.ContainsKey (targetAtomName))
				{
					MakeE1RectCanvas (targetElement);
				}
			}
			else if (2 == actionType)
			{
				//따라 그리기
				if (false == DrawCanvasMap.ContainsKey (targetAtomName))
				{
					var canvas = new Canvas
					{
						Margin = new Thickness (targetElement.Margin.Left, targetElement.Margin.Top, 0, 0),
						Width = dw,
						Height = dh,
						VerticalAlignment = System.Windows.VerticalAlignment.Top,
						HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
						Background = Brushes.Transparent,
						ClipToBounds = true,
					};

					canvas.MouseLeftButtonDown -= DrawCanvas_MouseLeftButtonDown;
					canvas.MouseLeftButtonDown += DrawCanvas_MouseLeftButtonDown;

					canvas.MouseMove -= DrawCanvas_MouseMove;
					canvas.MouseMove += DrawCanvas_MouseMove;

					canvas.MouseLeftButtonUp -= DrawCanvas_MouseLeftButtonUp;
					canvas.MouseLeftButtonUp += DrawCanvas_MouseLeftButtonUp;

					canvas.Cursor = Cursors.Pen;

					#region | ContextMenu |

					var contextMenu = new ContextMenu ();
					MenuItem clearItem = new MenuItem { Header = "초기화", };

					clearItem.Tag = canvas;

					clearItem.Click -= ClearItem_Click;
					clearItem.Click += ClearItem_Click;

					contextMenu.Items.Add (clearItem);
					canvas.ContextMenu = contextMenu;

					#endregion

					DrawCanvasMap.Add (targetAtomName, canvas);

					ActionGrid.Children.Add (canvas);
				}
			}
			else if (3 == actionType)
			{
				//색칠하기
				if (0 == toolType)
				{
					if (false == DrawCanvasMap.ContainsKey (targetAtomName))
					{
						var canvas = new Canvas
						{
							Margin = new Thickness (targetElement.Margin.Left, targetElement.Margin.Top, 0, 0),
							Width = dw,
							Height = dh,
							VerticalAlignment = System.Windows.VerticalAlignment.Top,
							HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
							Background = Brushes.Transparent,
							ClipToBounds = true,
						};

						canvas.MouseLeftButtonDown -= FillCanvas_MouseLeftButtonDown;
						canvas.MouseLeftButtonDown += FillCanvas_MouseLeftButtonDown;

						canvas.MouseMove -= FillCanvas_MouseMove;
						canvas.MouseMove += FillCanvas_MouseMove;

						canvas.MouseLeftButtonUp -= FillCanvas_MouseLeftButtonUp;
						canvas.MouseLeftButtonUp += FillCanvas_MouseLeftButtonUp;

						canvas.Cursor = Cursors.Pen;

						#region | ContextMenu |

						var contextMenu = new ContextMenu ();
						MenuItem clearItem = new MenuItem { Header = "초기화", };

						clearItem.Tag = canvas;

						clearItem.Click -= ClearItem_Click;
						clearItem.Click += ClearItem_Click;

						contextMenu.Items.Add (clearItem);
						canvas.ContextMenu = contextMenu;

						#endregion

						DrawCanvasMap.Add (targetAtomName, canvas);

						ActionGrid.Children.Add (canvas);
					}
					else
					{
						if (_AtomEditColorMap.ContainsKey (targetAtomName) && DrawCanvasMap.ContainsKey (targetAtomName))
						{
							var canvas = DrawCanvasMap[targetAtomName];

							var editColor = _AtomEditColorMap[targetAtomName];
							var currentColor = WPFColorConverter.ConvertMediaBrushToArgb (atom.GetAtomBackground ());

							if (Visibility.Visible != canvas.Visibility && editColor == currentColor)
							{
								canvas.Visibility = Visibility.Visible;
							}
						}
					}
				}
			}
		}

		private void MakeE1RectCanvas (AtomBase targetElement)
		{
			var dw = targetElement.ActualWidth;
			var dh = targetElement.ActualHeight;
			var targetAtomName = targetElement.AtomCore.GetProperVar ();

			var canvas = new Canvas
			{
				Margin = new Thickness (targetElement.Margin.Left, targetElement.Margin.Top, 0, 0),
				Width = dw,
				Height = dh,
				VerticalAlignment = System.Windows.VerticalAlignment.Top,
				HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
				Background = Brushes.Transparent,
				ClipToBounds = true,
			};

			canvas.MouseLeftButtonDown -= RectCanvas_MouseLeftButtonDown;
			canvas.MouseLeftButtonDown += RectCanvas_MouseLeftButtonDown;

			canvas.MouseMove -= RectCanvas_MouseMove;
			canvas.MouseMove += RectCanvas_MouseMove;

			canvas.MouseLeftButtonUp -= RectCanvas_MouseLeftButtonUp;
			canvas.MouseLeftButtonUp += RectCanvas_MouseLeftButtonUp;

			canvas.Cursor = Cursors.Pen;

			#region | ContextMenu |

			var contextMenu = new ContextMenu ();
			MenuItem clearItem = new MenuItem { Header = "초기화", };

			clearItem.Tag = canvas;
			clearItem.Click -= ClearItem_Click;
			clearItem.Click += ClearItem_Click;

			contextMenu.Items.Add (clearItem);
			canvas.ContextMenu = contextMenu;

			#endregion

			DrawCanvasMap.Add (targetAtomName, canvas);

			ActionGrid.Children.Add (canvas);
		}

		private void ExecuteMouseMove_E1 (MouseEventArgs e)
		{
			if (MouseButtonState.Pressed == e.LeftButton)
			{
				var quizType = AtomAttrib.DisplayQuizType;
				var actionType = 0;
				var toolType = 0;

				if (quizType == QuizType.E11)
				{
					actionType = AtomAttrib.EBookQuizOptionNode.E11.ActionType;
					toolType = AtomAttrib.EBookQuizOptionNode.E11.ToolType;
				}
				else
				{
					PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
				}

				if (0 == actionType)
				{
					if (null != _CurrentLine)
					{
						var pt = e.GetPosition (ActionGrid);

						_CurrentLine.X2 = pt.X;
						_CurrentLine.Y2 = pt.Y;
					}
				}
				else if (3 == actionType)
				{
					//마우스를 누른 상태로 이동만 해도 칠해질 수 있도록 논리 보강
					if (1 == toolType)
					{
						var pt = e.GetPosition (ActionGrid);
						var atom = ExecuteHitText (pt);

						if (null != atom && true == AtomAttrib.DataMap.ContainsKey (atom.AtomCore))
						{
							var node = AtomAttrib.DataMap[atom.AtomCore];
							if (node.ActionType == QuizAction.Answer && node.QuizType == QuizType.E11)
							{
								FillRectE1 (atom, true);
							}
						}
					}
				}
			}
		}

		private void ExecuteMouseUp_E1 (MouseButtonEventArgs e)
		{
			if (null == _TargetAtomBase)
				return;

			var pt = e.GetPosition (ActionGrid);
			var atom = ExecuteHitText (pt);

			var quizType = AtomAttrib.DisplayQuizType;
			var actionType = 0;
			var toolType = 0;

			if (quizType == QuizType.E11)
			{
				actionType = AtomAttrib.EBookQuizOptionNode.E11.ActionType;
				toolType = AtomAttrib.EBookQuizOptionNode.E11.ToolType;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			if (0 == actionType)
			{
				//선그리기
				if (null != atom)
				{
					var node = GetPropertyNode (atom);

					if (QuizType.E11 == node.QuizType &&
						QuizAction.Answer == node.ActionType)
					{
						MakeLineE1 (_TargetAtomBase, atom);
					}
				}

				_TargetAtomBase = null;
				ActionGrid.Children.Remove (_CurrentLine);
				_CurrentLine = null;
			}
			else if (3 == actionType)
			{
				if (0 == toolType)
				{
					if (null != _TargetAtomBase)
					{
						var targetName = _TargetAtomBase.AtomCore.GetProperVar ();

						if (DrawCanvasMap.ContainsKey (targetName))
						{
							var canvas = DrawCanvasMap[targetName];
							CheckCoverage (canvas);
						}
					}
				}
				else if (1 == toolType)
				{
					if (null != atom && true == AtomAttrib.DataMap.ContainsKey (atom.AtomCore))
					{
						var node = AtomAttrib.DataMap[atom.AtomCore];
						if (node.ActionType == QuizAction.Answer && node.QuizType == QuizType.E11)
						{
							FillRectE1 (atom, true);
						}
					}
				}
			}
		}

		private void ExecuteMouseRightUp_E1 (MouseButtonEventArgs e)
		{
			var quizType = AtomAttrib.DisplayQuizType;
			var actionType = 0;
			var toolType = 0;

			if (quizType == QuizType.E11)
			{
				actionType = AtomAttrib.EBookQuizOptionNode.E11.ActionType;
				toolType = AtomAttrib.EBookQuizOptionNode.E11.ToolType;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			if (3 == actionType)
			{
				var pt = e.GetPosition (ActionGrid);
				var atom = ExecuteHitText (pt);

				if (null != atom)
				{
					if (0 == toolType)
					{
						var atomName = atom.AtomCore.GetProperVar ();
						if (DrawCanvasMap.ContainsKey (atomName))
						{
							var color = WPFColorConverter.ConvertMediaBrushToArgb (atom.GetAtomBackground ());

							if (_AtomEditColorMap.ContainsKey (atomName))
							{
								if (_AtomEditColorMap[atomName] != color)
								{
									FillRectE1 (atom);
								}
							}
						}
					}
					else
					{
						FillRectE1 (atom);
					}
				}
			}
		}

		#region | 도형 그리기 |

		private void RectCanvas_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			var canvas = sender as Canvas;
			var pt = e.GetPosition (canvas);

			var quizType = this.AtomAttrib.DisplayQuizType;
			var actionType = 0;
			var toolType = 0;
			var answerColor = 0;
			var lineType = 0;

			canvas.Tag = pt;

			if (quizType == QuizType.E11)
			{
				actionType = AtomAttrib.EBookQuizOptionNode.E11.ActionType;
				toolType = AtomAttrib.EBookQuizOptionNode.E11.ToolType;
				answerColor = AtomAttrib.EBookQuizOptionNode.E11.AnswerColor;
				lineType = AtomAttrib.EBookQuizOptionNode.E11.LineType;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			if (1 == actionType && 0 == toolType)
			{
				//도형그리기 - 직선
				_CurrentLine = new Line ()
				{
					X1 = pt.X,
					X2 = pt.X,
					Y1 = pt.Y,
					Y2 = pt.Y,
					Stroke = WPFColorConverter.ConvertArgbToMediaBrush (answerColor),
				};

				if (1 == lineType)
				{
					_CurrentLine.StrokeDashArray = new DoubleCollection () { 3, 3, 3, 3 };
				}

				canvas.Children.Add (_CurrentLine);
			}
		}

		private void RectCanvas_MouseMove (object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var quizType = AtomAttrib.DisplayQuizType;
				var canvas = sender as Canvas;
				var pt2 = e.GetPosition (canvas);

				var actionType = 0;
				var toolType = 0;
				var answerColor = 0;
				var lineType = 0;

				if (quizType == QuizType.E11)
				{
					actionType = AtomAttrib.EBookQuizOptionNode.E11.ActionType;
					toolType = AtomAttrib.EBookQuizOptionNode.E11.ToolType;
					answerColor = AtomAttrib.EBookQuizOptionNode.E11.AnswerColor;
					lineType = AtomAttrib.EBookQuizOptionNode.E11.LineType;
				}
				else
				{
					PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
				}

				if (null == canvas.Tag)
					return;

				if (1 == actionType &&
					0 == toolType)
				{
					//도형그리기 - 직선
					//_CurrentLine.X2 = 

					var difX = Math.Abs (pt2.X - _CurrentLine.X1);
					var difY = Math.Abs (pt2.Y - _CurrentLine.Y1);

					if (difX < difY)
					{
						_CurrentLine.X2 = _CurrentLine.X1;
						_CurrentLine.Y2 = pt2.Y;
					}
					else
					{
						_CurrentLine.X2 = pt2.X;
						_CurrentLine.Y2 = _CurrentLine.Y1;
					}
				}
				else
				{
					var pt = (Point)canvas.Tag;

					Line line = new Line ()
					{
						Stroke = WPFColorConverter.ConvertArgbToMediaBrush (answerColor),
						X1 = pt.X,
						Y1 = pt.Y,
						X2 = pt2.X,
						Y2 = pt2.Y,
					};

					if (1 == lineType)
					{
						line.StrokeDashArray = new DoubleCollection () { 3, 3, 3, 3 };
					}

					canvas.Tag = pt2;
					canvas.Children.Add (line);

					_CurrentE1DrawingList.Add (line);
				}
			}
		}

		private void RectCanvas_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
		{
			var quizType = this.AtomAttrib.DisplayQuizType;

			var actionType = 0;
			var toolType = 0;

			var atomName = _TargetAtomBase.AtomCore.GetProperVar ();
			var panel = sender as Panel;


			if (quizType == QuizType.E11)
			{
				actionType = AtomAttrib.EBookQuizOptionNode.E11.ActionType;
				toolType = AtomAttrib.EBookQuizOptionNode.E11.ToolType;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			if (0 == _CurrentE1DrawingList.Count)
				return;

			if (1 == actionType)
			{
				//도형 그리기
				if (0 == toolType)
				{
					//직선

					_CurrentE1DrawingList.Clear ();

					var dx = _CurrentLine.X1;
					var dw = _CurrentLine.X2 - _CurrentLine.X1;
					var dy = _CurrentLine.Y1;
					var dh = _CurrentLine.Y2 - _CurrentLine.Y1;

					MakeE1Rect (atomName, dx, dy, dw, dh);
					_CurrentLine = null;
				}
				else if (1 == toolType || 2 == toolType)
				{
					//사각형
					//원
					var dx = _CurrentE1DrawingList.Min (i => i.X1);
					var dx2 = _CurrentE1DrawingList.Max (i => i.X2);

					var dy = _CurrentE1DrawingList.Min (i => i.Y1);
					var dy2 = _CurrentE1DrawingList.Max (i => i.Y2);

					var dw = dx2 - dx;
					var dh = dy2 - dy;

					foreach (var i in _CurrentE1DrawingList)
					{
						panel.Children.Remove (i);
					}

					_CurrentE1DrawingList.Clear ();

					if (30 > dw || 30 > dh)
						return;

					MakeE1Rect (atomName, dx, dy, dw, dh);
				}
			}
		}

		public void MakeE1Rect (string atomName, double dx, double dy, double dw, double dh, EBookQuizOptionNode.QuizOption_E11.RectType type = EBookQuizOptionNode.QuizOption_E11.RectType.None)
		{
			if (false == DrawCanvasMap.ContainsKey (atomName))
			{
				var atom = FindAtom (atomName);
				MakeE1RectCanvas (atom);
			}

			var canvas = DrawCanvasMap[atomName];
			var quizType = this.AtomAttrib.DisplayQuizType;

			var toolType = 0;
			var answerColor = 0;
			var lineType = 0;

			if (quizType == QuizType.E11)
			{
				toolType = AtomAttrib.EBookQuizOptionNode.E11.ToolType;
				answerColor = AtomAttrib.EBookQuizOptionNode.E11.AnswerColor;
				lineType = AtomAttrib.EBookQuizOptionNode.E11.LineType;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			switch (type)
			{
				case EBookQuizOptionNode.QuizOption_E11.RectType.Line:
					toolType = 0;
					break;
				case EBookQuizOptionNode.QuizOption_E11.RectType.Rectangle:
					toolType = 1;
					break;
				case EBookQuizOptionNode.QuizOption_E11.RectType.Ellipse:
					toolType = 2;
					break;
			}

			if (0 == toolType)
			{
				//직선
				var line = new Line ()
				{
					X1 = dx,
					X2 = dx + dw,
					Y1 = dy,
					Y2 = dy + dh,
					Stroke = WPFColorConverter.ConvertArgbToMediaBrush (answerColor),
					StrokeThickness = 1,
					Fill = Brushes.Transparent,
				};

				if (1 == lineType)
				{
					line.StrokeDashArray = new DoubleCollection () { 3, 3, 3, 3 };
				}

				canvas.Children.Add (line);
			}
			else if (1 == toolType)
			{
				//사각형
				var rectangle = new Rectangle ()
				{
					Margin = new Thickness (dx, dy, 0, 0),
					Width = dw,
					Height = dh,
					Stroke = WPFColorConverter.ConvertArgbToMediaBrush (answerColor),
					StrokeThickness = 1,
					Fill = Brushes.Transparent,
				};

				if (1 == lineType)
				{
					rectangle.StrokeDashArray = new DoubleCollection () { 3, 3, 3, 3 };
				}

				canvas.Children.Add (rectangle);
			}
			else if (2 == toolType)
			{
				//원
				var ellipse = new Ellipse ()
				{
					Margin = new Thickness (dx, dy, 0, 0),
					Width = dw,
					Height = dh,
					Stroke = WPFColorConverter.ConvertArgbToMediaBrush (answerColor),
					StrokeThickness = 1,
					Fill = Brushes.Transparent,
				};

				if (1 == lineType)
				{
					ellipse.StrokeDashArray = new DoubleCollection () { 3, 3, 3, 3 };
				}

				canvas.Children.Add (ellipse);
			}
		}

		#endregion

		#region | 따라그리기 |

		private void DrawCanvas_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			var canvas = sender as Canvas;
			canvas.Tag = e.GetPosition (canvas);
		}

		private void DrawCanvas_MouseMove (object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var canvas = sender as Canvas;

				if (null == canvas.Tag)
					return;

				var pt = (Point)canvas.Tag;
				var pt2 = e.GetPosition (canvas);

				var quizType = this.AtomAttrib.DisplayQuizType;
				var answerColor = 0;
				var lineType = 0;

				if (quizType == QuizType.E11)
				{
					answerColor = AtomAttrib.EBookQuizOptionNode.E11.AnswerColor;
					lineType = AtomAttrib.EBookQuizOptionNode.E11.LineType;
				}
				else
				{
					PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
				}

				Line line = new Line ()
				{
					Stroke = WPFColorConverter.ConvertArgbToMediaBrush (answerColor),
					X1 = pt.X,
					Y1 = pt.Y,
					X2 = pt2.X,
					Y2 = pt2.Y
				};

				if (1 == lineType)
				{
					line.StrokeDashArray = new DoubleCollection () { 7, 7, 7, 7 };
				}

				canvas.Tag = pt2;
				canvas.Children.Add (line);
			}
		}

		private void DrawCanvas_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
		{

		}

		#endregion

		#region | 선그리기 |

		public void MakeLineE1 (AtomBase startAtom, AtomBase endAtom)
		{
			if (startAtom == endAtom)
			{
				Trace.TraceInformation ("[error] MakeLineE1 (AtomBase startAtom, AtomBase endAtom) - 시작 / 도착 아톰이 동일함");
				return;
			}

			FrameworkElement startElement = startAtom;
			FrameworkElement endElement = endAtom;

			var startAtomName = startAtom.AtomCore.GetProperVar ();
			var endAtomName = endAtom.AtomCore.GetProperVar ();

			double x1 = startElement.Margin.Left + (startElement.ActualWidth / 2);
			double y1 = startElement.Margin.Top + (startElement.ActualHeight / 2);

			double x2 = endElement.Margin.Left + (endElement.ActualWidth / 2);
			double y2 = endElement.Margin.Top + (endElement.ActualHeight / 2);

			var quizType = this.AtomAttrib.DisplayQuizType;

			var answerColor = 0;
			var lineType = 0;

			if (quizType == QuizType.E11)
			{
				answerColor = AtomAttrib.EBookQuizOptionNode.E11.AnswerColor;
				lineType = AtomAttrib.EBookQuizOptionNode.E11.LineType;
			}
			else
			{
				PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
			}

			var line = new Line
			{
				VerticalAlignment = System.Windows.VerticalAlignment.Top,
				HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
				Stroke = WPFColorConverter.ConvertArgbToMediaBrush (answerColor),
				StrokeStartLineCap = PenLineCap.Round,
				StrokeEndLineCap = PenLineCap.Round,
				X1 = x1,
				Y1 = y1,
				X2 = x2,
				Y2 = y2,
				Tag = $"{startAtomName},{endAtomName}",
			};

			if (1 == lineType)
			{
				line.StrokeDashArray = new DoubleCollection () { 7, 7, 7, 7 };
			}

			bool isExists = false;

			foreach (var drawLine in EffectGrid.Children.OfType<Line> ())
			{
				if (line.Tag?.ToString () == drawLine.Tag?.ToString ())
				{
					isExists = true;
					break;
				}
			}

			if (false == isExists)
			{
				EffectGrid.Children.Add (line);
				line.MouseRightButtonDown += Line_MouseRightButtonDown;
			}
		}

		private void CurrentLine_MouseLeftButtonUpE1 (object sender, MouseButtonEventArgs e)
		{
			ExecuteMouseUp_E1 (e);
		}

		private void Line_MouseRightButtonDown (object sender, MouseButtonEventArgs e)
		{
			if (sender is UIElement element)
			{
				EffectGrid.Children.Remove (element);
			}
		}

		#endregion

		#region | 색칠하기 |

		#region | 영역 |

		public void FillRectE1 (AtomBase atom, bool isOnlyFill = false)
		{
			if (null != atom)
			{
				var atomCore = atom.AtomCore;
				var atomName = atom.AtomCore.GetProperVar ();

				var quizType = this.AtomAttrib.DisplayQuizType;
				var answerColor = 0;

				if (quizType == QuizType.E11)
				{
					answerColor = AtomAttrib.EBookQuizOptionNode.E11.AnswerColor;
				}
				else
				{
					PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
				}

				if (false == _AtomEditColorMap.ContainsKey (atomName))
				{
					var atomColor = WPFColorConverter.ConvertMediaBrushToArgb (atom.GetAtomBackground ());
					_AtomEditColorMap.Add (atomName, atomColor);
				}

				var currentColor = WPFColorConverter.ConvertMediaBrushToArgb (atom.GetAtomBackground ());

				if (_AtomEditColorMap[atomName] != currentColor)
				{
					if (true == isOnlyFill)
						return;

					answerColor = _AtomEditColorMap[atomName];
				}

				atomCore.SetProperty (EBookQuizViewofAtom.BackgroundKeyIndex, null, new CVariantX (answerColor));
				atomCore.GetOfAtom ().SetAtomBackground (WPFColorConverter.ConvertArgbToMediaBrush (answerColor));
			}
		}

		#endregion

		#region | 브러시 |

		private void FillCanvas_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			var canvas = sender as Canvas;
			canvas.Tag = e.GetPosition (canvas);

			foreach (var item in DrawCanvasMap)
			{
				if (item.Value == canvas)
				{
					_TargetAtomBase = FindAtom (item.Key);
				}
			}
		}

		private void FillCanvas_MouseMove (object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var canvas = sender as Canvas;

				if (null == canvas.Tag)
				{
					canvas.Tag = e.GetPosition (canvas);
					return;
				}

				var targetAtomName = _TargetAtomBase.AtomCore.GetProperVar ();

				if (null == canvas || Brushes.Transparent != canvas.Background ||
					false == DrawCanvasMap.ContainsKey (targetAtomName))
					return;

				var quizType = this.AtomAttrib.DisplayQuizType;

				var answerColor = 0;
				var lineType = 0;

				if (quizType == QuizType.E11)
				{
					answerColor = AtomAttrib.EBookQuizOptionNode.E11.AnswerColor;
					lineType = AtomAttrib.EBookQuizOptionNode.E11.LineType;
				}
				else
				{
					PQAppBase.TraceDebugLog ($"not found QuizType : {quizType}");
				}

				if (DrawCanvasMap[targetAtomName] != canvas)
				{
					CheckCoverage (DrawCanvasMap[targetAtomName]);
					DrawCanvasMap[targetAtomName].Tag = null;

					foreach (var item in DrawCanvasMap)
					{
						if (canvas == item.Value)
						{
							_TargetAtomBase = FindAtom (item.Key);
							break;
						}
					}
				}

				var pt = (Point)canvas.Tag;
				var pt2 = e.GetPosition (canvas);

				Line line = new Line ()
				{
					StrokeThickness = 10,
					Stroke = WPFColorConverter.ConvertArgbToMediaBrush (answerColor),
					X1 = pt.X,
					Y1 = pt.Y,
					X2 = pt2.X,
					Y2 = pt2.Y
				};

				if (1 == lineType)
				{
					line.StrokeDashArray = new DoubleCollection () { 7, 7, 7, 7 };
				}

				canvas.Tag = pt2;
				canvas.Children.Add (line);
			}
		}

		private void FillCanvas_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
		{
			CheckCoverage (sender as Canvas);
			e.Handled = true;
		}

		private void CheckCoverage (Canvas canvas)
		{
			if (null == _TargetAtomBase)
				return;

			var bitmapSource = SaveCanvasToPng (canvas);
			if (null == bitmapSource)
				return;

			var isResult = TranslateImage.Manager.CheckCoverage (bitmapSource);

			if (isResult)
			{
				// 영역의 70% 이상을 그렸음
				foreach (var item in DrawCanvasMap)
				{
					if (canvas == item.Value)
					{
						if (_TargetAtomBase.AtomCore.GetProperVar () != item.Key)
						{
							//시작 아톰이 아닌 다른 아톰에서 up 이벤트가 발생한경우 
							//간혈적으로 _TargetAtomBase가 변경되는 경우가 있어 논리 보강
							_TargetAtomBase = FindAtom (item.Key);
						}
					}
				}

				FillRectE1 (_TargetAtomBase);

				canvas.Children.Clear ();
				canvas.Visibility = Visibility.Collapsed;
				canvas.Tag = null;
				_TargetAtomBase = null;
			}
			else
			{
				// 영역의 70%를 그리지 못함
			}
		}

		public BitmapSource SaveCanvasToPng (Canvas canvas)
		{
			if (1 > canvas.ActualWidth || 1 > canvas.ActualHeight)
				return null;

			RenderTargetBitmap renderBitmap = new RenderTargetBitmap (
			(int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);

			VisualBrush sourceBrush = new VisualBrush (canvas);

			DrawingVisual drawingVisual = new DrawingVisual ();
			DrawingContext drawingContext = drawingVisual.RenderOpen ();

			using (drawingContext)
			{
				drawingContext.DrawRectangle (sourceBrush, null, new Rect (0, 0, canvas.ActualWidth, canvas.ActualHeight));
			}

			renderBitmap.Render (drawingVisual);

			return renderBitmap;
		}

		#endregion

		#endregion

		private void ClearItem_Click (object sender, RoutedEventArgs e)
		{
			if ((sender as MenuItem).Tag is Panel canvas)
			{
				canvas.Children.Clear ();
				canvas.Background = Brushes.Transparent;

				foreach (var item in DrawCanvasMap)
				{
					string targetAtomName = item.Key;
					if (item.Value == canvas)
					{
						if (_AtomEditColorMap.ContainsKey (targetAtomName))
						{
							var editColor = _AtomEditColorMap[item.Key];
							var targetAtom = FindAtom (item.Key);
							var targetAtomAttrib = targetAtom.AtomCore.GetAttrib();
							Brush editColorBrush = WPFColorConverter.ConvertArgbToMediaBrush(editColor);

                            if (targetAtom != null && targetAtomAttrib != null)
							{
                                targetAtom.SetAtomBackground(editColorBrush);
								targetAtomAttrib.SetAtomBackground(editColorBrush, editColorBrush == Brushes.Transparent);
                            }                          

                        }
					}
				}
			}
		}

		#endregion

		private QuizType GetQuizType (AtomBase atom)
		{
			if (null != atom && true == _AtomAttrib.DataMap.ContainsKey (atom.AtomCore))
			{
				return _AtomAttrib.DataMap[atom.AtomCore].QuizType;
			}

			return QuizType.None;
		}

		private EBookQuizPropertyNode GetPropertyNode (AtomBase atom)
		{
			if (null != atom && true == AtomAttrib.DataMap.ContainsKey (atom.AtomCore))
				return AtomAttrib.DataMap[atom.AtomCore];

			return null;
		}

		private AtomBase ExecuteHitText (Point pt)
		{
			if (null == _SourceTabPage)
				return null;

			var canvas = _SourceTabPage.RootCanvas;

			if (null != canvas)
			{
				var atomList = canvas.Children.OfType<AtomBase> ();
				atomList = atomList.OrderBy (i => Grid.GetZIndex (i));

				foreach (var atom in atomList)
				{
					if (Visibility.Visible != atom.Visibility || null == atom.AtomCore)
						continue;

					double dx = atom.Margin.Left;
					double dy = atom.Margin.Top;
					double dw = atom.ActualWidth;
					double dh = atom.ActualHeight;

					if (dx <= pt.X && pt.X <= dx + dw &&
						dy <= pt.Y && pt.Y <= dy + dh)
					{
						//atom
						if (true == _AtomAttrib.DataMap.ContainsKey (atom.AtomCore))
						{
							var property = _AtomAttrib.DataMap[atom.AtomCore];
							return atom;
						}
					}
				}
			}

			return null;
		}

		public AtomBase FindAtom (string atomName)
		{
			var atoms = _SourceTabPage.RootCanvas.Children.OfType<AtomBase> ().ToList ();

			foreach (var item in atoms)
			{
				if (atomName == item.AtomCore.GetProperVar ())
				{
					return item;
				}
			}

			return null;
		}

		public void ClearQuizViewEngine ()
		{
			ActionGrid.Children.Clear ();
			EffectGrid.Children.Clear ();

			var canvas = _SourceTabPage?.RootCanvas;
			var atomList = canvas.Children.OfType<AtomBase> ().ToList ();

			foreach (var item in QuizElementMap)
			{
				var parent = item.Value.Parent as Panel;

				if (null != parent)
				{
					parent.Children.Remove (item.Value);
				}
			}

			foreach (var item in DrawCanvasMap)
			{
				if (item.Value is FrameworkElement element)
				{
					if (element.Parent is Panel panel)
					{
						panel.Children.Remove (element);
					}
				}
			}

			foreach (var item in AnswerCheckControlMap)
			{
				if (item.Value.Parent is Panel panel)
				{
					panel.Children.Remove (item.Value);
				}
			}

			foreach (var item in _AtomEditValueData)
			{
				if (item.Key is DecorImageAtom decorImageAtom)
				{
					var decorImageAttrib = decorImageAtom.Attrib as DecorImageAttrib;
					var value = item.Value?.ToString ();

					if (false == string.IsNullOrEmpty (value))
					{
						int index = value.IndexOf ("$");
						var imageKey = value.Substring (0, index);
						var imagePath = value.Substring (index + 1, value.Length - (index + 1));
						decorImageAttrib.ImageKey = _Kiss.toInt32 (imageKey);
						decorImageAttrib.ImagePath = imagePath;
						decorImageAtom.CompletePropertyChanged ();
					}
				}
				else if (item.Key is EBookTextAtom eBookTextAtom)
				{
					var eBookTextofAtom = eBookTextAtom.AtomBase as EBookTextofAtom;

					FlowDocument flowDocument = eBookTextofAtom.GetFlowDocument ();

					var value = item.Value as List<byte[]>;

					if (null != value && 1 < value.Count)
					{
						using (MemoryStream rtfMemory = new MemoryStream (value[0]))
						using (MemoryStream xamlMemory = new MemoryStream (value[1]))
						{
							TextRange range = new TextRange (flowDocument.ContentStart, flowDocument.ContentEnd);

							range.Load (rtfMemory, System.Windows.DataFormats.Rtf);
							range.Load (xamlMemory, System.Windows.DataFormats.XamlPackage);
						}
					}
				}
				else
				{
					var value = item.Value?.ToString ();
					item.Key.SetContentString (value, true);
				}
			}

			foreach (var item in _AtomEditColorMap)
			{
				var atomName = item.Key;

				var targetAtom = atomList.Find (i => i.AtomCore.AtomProperVar == atomName);

				if (null != targetAtom)
				{
					targetAtom.SetAtomBackground (WPFColorConverter.ConvertArgbToMediaBrush (item.Value));
				}
			}

			_AtomEditValueData.Clear ();
			_AtomEditColorMap.Clear ();

			_LineAnimateMap.Clear ();
			_LineOverMap.Clear ();

			QuizElementMap.Clear ();
			DrawCanvasMap.Clear ();
			AnswerCheckControlMap.Clear ();
		}

		public void CompletePropertyChanged ()
		{
			var quizType = AtomAttrib.DisplayQuizType;

			if (quizType == QuizType.A61)
			{
				_EBookQuizAnswerPopupControl.SetPopup (QuizType.A61, AtomAttrib);
			}
			else if (quizType == QuizType.A65)
			{
				_EBookQuizAnswerPopupControl.SetPopup (QuizType.A65, AtomAttrib);
			}
		}

	}
}