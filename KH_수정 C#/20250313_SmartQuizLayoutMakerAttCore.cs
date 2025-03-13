using System;
using System.Diagnostics;
using System.Windows.Controls;

using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.TopLight.Dynamic;
using Softpower.SmartMaker.TopProcess.Component.ViewModels;
using Softpower.SmartMaker.TopProcess.Component.Views;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPage;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;

namespace Softpower.SmartMaker.TopSmartAtomManager.SmartCore
{
	public class SmartQuizLayoutMakerAttCore : SmartAtomAttCore
	{
		private QuizMakerView _QuizMakerView = null;

		public SmartQuizLayoutMakerAttCore (QuizMakerView quizMakerView)
		{
			this._QuizMakerView = quizMakerView;
		}

		public override UserControl GetQuizMakerOptionPage ()
		{
			var document = _QuizMakerView.Document as QuizMakerDoc;
			var metaData = document.PageMetadata.QuizMetaData;
			var quizLayoutMetaData = metaData.QuizLayoutMetaData;
			var quizType = metaData.QuizType;
			var ebookQuizOptionNode = metaData.EBookQuizOptionNode;

			if (quizType == QuizType.None)
			{
				//퀴즈유형 지정 안되어 있으면 단답형으로 처리
				quizType = QuizType.C11;
			}

			var attPage = new SmartEBookQuizViewOptionAttPage (quizType);

			if (null != ebookQuizOptionNode)
			{
				switch (quizType)
				{
					case QuizType.A11: //선다형
						attPage.A11Page.DirectionType = ebookQuizOptionNode.A11.DirectionType;
						attPage.A11Page.AnswerEffect = ebookQuizOptionNode.A11.AnswerEffect;
						attPage.A11Page.AnswerCount = ebookQuizOptionNode.A11.AnswerCount;
						attPage.A11Page.AnswerSignType = ebookQuizOptionNode.A11.AnswerSignType;
						attPage.A11Page.IsMultiAnswer = ebookQuizOptionNode.A11.IsMultiAnswer;
						break;
					case QuizType.A15: //선다형
						attPage.A15Page.DirectionType = ebookQuizOptionNode.A15.DirectionType;
						attPage.A15Page.AnswerEffect = ebookQuizOptionNode.A15.AnswerEffect;
						attPage.A15Page.AnswerCount = ebookQuizOptionNode.A15.AnswerCount;
						attPage.A15Page.AnswerSignType = ebookQuizOptionNode.A15.AnswerSignType;
						attPage.A15Page.IsMultiAnswer = ebookQuizOptionNode.A15.IsMultiAnswer;

						attPage.A15Page.IsMakeBodyContent = quizLayoutMetaData.IsMakeBodyContent;
						attPage.A15Page.VerticalOffset = quizLayoutMetaData.VerticalOffset;
						break;
					case QuizType.A21: //선긋기
						attPage.A21Page.DirectionType = ebookQuizOptionNode.A21.DirectionType;
						attPage.A21Page.LineType = ebookQuizOptionNode.A21.LineType;
						attPage.A21Page.QuestionCount = ebookQuizOptionNode.A21.QuestionCount;
						attPage.A21Page.AnswerCount = ebookQuizOptionNode.A21.AnswerCount;
						attPage.A21Page.QuestionSignType = ebookQuizOptionNode.A21.QuestionSignType;
						attPage.A21Page.AnswerSignType = ebookQuizOptionNode.A21.AnswerSignType;
						attPage.A21Page.LineColor = ebookQuizOptionNode.A21.LineColor;
						attPage.A21Page.EllipseColor = ebookQuizOptionNode.A21.EllipseColor;
						attPage.A21Page.LineThickness = (int) ebookQuizOptionNode.A21.LineThickness;
						break;
					case QuizType.A25: //선긋기
						attPage.A25Page.DirectionType = ebookQuizOptionNode.A25.DirectionType;
						attPage.A25Page.LineType = ebookQuizOptionNode.A25.LineType;
						attPage.A25Page.QuestionCount = ebookQuizOptionNode.A25.QuestionCount;
						attPage.A25Page.AnswerCount = ebookQuizOptionNode.A25.AnswerCount;
						attPage.A25Page.QuestionSignType = ebookQuizOptionNode.A25.QuestionSignType;
						attPage.A25Page.AnswerSignType = ebookQuizOptionNode.A25.AnswerSignType;						
						attPage.A25Page.LineColor = ebookQuizOptionNode.A25.LineColor;
						attPage.A25Page.EllipseColor = ebookQuizOptionNode.A25.EllipseColor;
						attPage.A25Page.LineThickness = (int) ebookQuizOptionNode.A25.LineThickness;

						attPage.A25Page.VerticalOffset = quizLayoutMetaData.VerticalOffset;
						break;
					case QuizType.A31: //끌어놓기

						break;
					case QuizType.A35: //끌어놓기
						attPage.A35Page.AnswerRunType = ebookQuizOptionNode.A35.AnswerRunType;
						attPage.A35Page.DirectionType = ebookQuizOptionNode.A35.DirectionType;
						attPage.A35Page.QuestionCount = ebookQuizOptionNode.A35.QuestionCount;
						attPage.A35Page.AnswerCount = ebookQuizOptionNode.A35.AnswerCount;
						attPage.A35Page.QuestionSignType = ebookQuizOptionNode.A35.QuestionSignType;
						attPage.A35Page.AnswerSignType = ebookQuizOptionNode.A35.AnswerSignType;

						attPage.A35Page.VerticalOffset = quizLayoutMetaData.VerticalOffset;
						break;
					case QuizType.A41: //빈칸채움
						attPage.A45Page.AnswerSignType = ebookQuizOptionNode.A41.AnswerSignType;
						attPage.A45Page.AnswerCount = ebookQuizOptionNode.A41.AnswerCount;
						attPage.A45Page.IsAnswerArea = ebookQuizOptionNode.A41.IsAnswerArea;
						break;
					case QuizType.A45: //빈칸채움
						attPage.A45Page.AnswerSignType = ebookQuizOptionNode.A45.AnswerSignType;
						attPage.A45Page.AnswerCount = ebookQuizOptionNode.A45.AnswerCount;
						attPage.A45Page.IsAnswerArea = ebookQuizOptionNode.A45.IsAnswerArea;
						break;
					case QuizType.A51: //순서맞춤
						attPage.A51Page.QuestionCount = ebookQuizOptionNode.A51.QuestionCount;
						attPage.A51Page.AnswerCount = ebookQuizOptionNode.A51.AnswerCount;
						attPage.A51Page.QuestionSignType = ebookQuizOptionNode.A51.QuestionSignType;
						attPage.A51Page.AnswerSignType = ebookQuizOptionNode.A51.AnswerSignType;
						break;
					case QuizType.A55: //순서맞춤
						attPage.A55Page.QuestionCount = ebookQuizOptionNode.A55.QuestionCount;
						attPage.A55Page.AnswerCount = ebookQuizOptionNode.A55.AnswerCount;
						attPage.A55Page.QuestionSignType = ebookQuizOptionNode.A55.QuestionSignType;
						attPage.A55Page.AnswerSignType = ebookQuizOptionNode.A55.AnswerSignType;
						break;
					case QuizType.A61: //OX퀴즈
						attPage.A61Page.AnswerCount = ebookQuizOptionNode.A61.AnswerCount;
						attPage.A61Page.AnswerSignType = ebookQuizOptionNode.A61.AnswerSignType;
						attPage.A61Page.ToolType = ebookQuizOptionNode.A61.ToolType;
						break;
					case QuizType.A65: //OX퀴즈
						attPage.A65Page.AnswerCount = ebookQuizOptionNode.A65.AnswerCount;
						attPage.A65Page.AnswerSignType = ebookQuizOptionNode.A65.AnswerSignType;
						attPage.A65Page.ToolType = ebookQuizOptionNode.A65.ToolType;
						attPage.A65Page.AnswerDirectionType = ebookQuizOptionNode.A65.AnswerDirectionType;
						attPage.A65Page.AnswerDisplayType = ebookQuizOptionNode.A65.AnswerDisplayType;

						attPage.A65Page.VerticalOffset = quizLayoutMetaData.VerticalOffset;
						break;
					case QuizType.C15: //단답형
						attPage.C15Page.AnswerCount = ebookQuizOptionNode.C15.AnswerCount;
						attPage.C15Page.AnswerSignType = ebookQuizOptionNode.C15.AnswerSignType;
						attPage.C15Page.AnswerDirectionType = ebookQuizOptionNode.C15.AnswerDirectionType;
						attPage.C15Page.AnswerDisplayType = ebookQuizOptionNode.C15.AnswerDisplayType;

						attPage.C15Page.VerticalOffset = quizLayoutMetaData.VerticalOffset;
						break;
					case QuizType.C11: //단답형
						attPage.C11Page.DirectionType = ebookQuizOptionNode.C11.DirectionType;
						attPage.C11Page.AnswerCount = ebookQuizOptionNode.C11.AnswerCount;
						attPage.C11Page.AnswerSignType = ebookQuizOptionNode.C11.AnswerSignType;
						break;
					case QuizType.C25: //서술형
						attPage.C25Page.DirectionType = ebookQuizOptionNode.C25.DirectionType;
						attPage.C25Page.QuestionSignType = ebookQuizOptionNode.C25.QuestionSignType;
						attPage.C25Page.AnswerSignType = ebookQuizOptionNode.C25.AnswerSignType;
						break;
					case QuizType.E11: //그려넣기
						attPage.E11Page.AnswerCount = ebookQuizOptionNode.E11.AnswerCount;
						attPage.E11Page.AnswerColor = ebookQuizOptionNode.E11.AnswerColor;
						attPage.E11Page.ActionType = ebookQuizOptionNode.E11.ActionType;
						attPage.E11Page.LineType = ebookQuizOptionNode.E11.LineType;
						attPage.E11Page.ToolType = ebookQuizOptionNode.E11.ToolType;
						attPage.E11Page.AnswerRunType = ebookQuizOptionNode.E11.AnswerRunType;
						break;
					default:
						Trace.TraceError ("SmartQuizLayoutMakerAttCore GetQuizMakerOptionPage quiztype not found : " + quizType);
						break;
				}
			}

			return attPage;
		}

		public override void OnUpdateQuizmakerOptionPage ()
		{
			var document = _QuizMakerView.Document as QuizMakerDoc;
			var quizType = document.PageMetadata.QuizMetaData.QuizType;
			var attPage = CurrentAttPage as SmartEBookQuizViewOptionAttPage;
			var metaData = document.PageMetadata.QuizMetaData;
			var quizLayoutMetaData = metaData.QuizLayoutMetaData;
			var ebookQuizOptionNode = metaData.EBookQuizOptionNode;

			if (null != attPage && null != ebookQuizOptionNode)
			{
				switch (quizType)
				{
					case QuizType.A11: //선다형
						ebookQuizOptionNode.A11.DirectionType = attPage.A11Page.DirectionType;
						ebookQuizOptionNode.A11.AnswerEffect = attPage.A11Page.AnswerEffect;
						ebookQuizOptionNode.A11.AnswerCount = attPage.A11Page.AnswerCount;
						ebookQuizOptionNode.A11.AnswerSignType = attPage.A11Page.AnswerSignType;
						ebookQuizOptionNode.A11.IsMultiAnswer = attPage.A11Page.IsMultiAnswer;
						break;
					case QuizType.A15: //선다형
						ebookQuizOptionNode.A15.DirectionType = attPage.A15Page.DirectionType;
						ebookQuizOptionNode.A15.AnswerEffect = attPage.A15Page.AnswerEffect;
						ebookQuizOptionNode.A15.AnswerCount = attPage.A15Page.AnswerCount;
						ebookQuizOptionNode.A15.AnswerSignType = attPage.A15Page.AnswerSignType;
						ebookQuizOptionNode.A15.IsMultiAnswer = attPage.A15Page.IsMultiAnswer;

						quizLayoutMetaData.IsMakeBodyContent = attPage.A15Page.IsMakeBodyContent;
						quizLayoutMetaData.VerticalOffset = attPage.A15Page.VerticalOffset;

						//정렬 방향에 따라 ColumnCount 설정 논리 (기본값 : 세로-1, 가로-2)
						quizLayoutMetaData.ColumnCount = 1 == ebookQuizOptionNode.A15.DirectionType ? Math.Max (2, quizLayoutMetaData.ColumnCount) : 1;
						break;
					case QuizType.A21: //선긋기
						ebookQuizOptionNode.A21.DirectionType = attPage.A21Page.DirectionType;
						ebookQuizOptionNode.A21.LineType = attPage.A21Page.LineType;
						ebookQuizOptionNode.A21.QuestionCount = attPage.A21Page.QuestionCount;
						ebookQuizOptionNode.A21.AnswerCount = attPage.A21Page.AnswerCount;
						ebookQuizOptionNode.A21.QuestionSignType = attPage.A21Page.QuestionSignType;
						ebookQuizOptionNode.A21.AnswerSignType = attPage.A21Page.AnswerSignType;
						ebookQuizOptionNode.A21.LineColor = attPage.A21Page.LineColor;
						ebookQuizOptionNode.A21.EllipseColor = attPage.A21Page.EllipseColor;
						ebookQuizOptionNode.A21.LineThickness = attPage.A21Page.LineThickness;
						break;
					case QuizType.A25: //선긋기
						ebookQuizOptionNode.A25.DirectionType = attPage.A25Page.DirectionType;
						ebookQuizOptionNode.A25.LineType = attPage.A25Page.LineType;
						ebookQuizOptionNode.A25.QuestionCount = attPage.A25Page.QuestionCount;
						ebookQuizOptionNode.A25.AnswerCount = attPage.A25Page.AnswerCount;
						ebookQuizOptionNode.A25.QuestionSignType = attPage.A25Page.QuestionSignType;
						ebookQuizOptionNode.A25.AnswerSignType = attPage.A25Page.AnswerSignType;
						ebookQuizOptionNode.A25.LineColor = attPage.A25Page.LineColor;
						ebookQuizOptionNode.A25.EllipseColor = attPage.A25Page.EllipseColor;
						ebookQuizOptionNode.A25.LineThickness = attPage.A25Page.LineThickness;

						quizLayoutMetaData.VerticalOffset = attPage.A25Page.VerticalOffset;
						break;
					case QuizType.A31: //끌어놓기
						break;
					case QuizType.A35: //끌어놓기
						ebookQuizOptionNode.A35.AnswerRunType = attPage.A35Page.AnswerRunType;
						ebookQuizOptionNode.A35.DirectionType = attPage.A35Page.DirectionType;
						ebookQuizOptionNode.A35.QuestionCount = attPage.A35Page.QuestionCount;
						ebookQuizOptionNode.A35.AnswerCount = attPage.A35Page.AnswerCount;
						ebookQuizOptionNode.A35.QuestionSignType = attPage.A35Page.QuestionSignType;
						ebookQuizOptionNode.A35.AnswerSignType = attPage.A35Page.AnswerSignType;

						quizLayoutMetaData.VerticalOffset = attPage.A35Page.VerticalOffset;
						break;
					case QuizType.A41: //빈칸채움
						ebookQuizOptionNode.A41.AnswerSignType = attPage.A41Page.AnswerSignType;
						ebookQuizOptionNode.A41.AnswerCount = attPage.A41Page.AnswerCount;
						ebookQuizOptionNode.A41.IsAnswerArea = attPage.A41Page.IsAnswerArea;
						break;
					case QuizType.A45: //빈칸채움
						ebookQuizOptionNode.A45.AnswerSignType = attPage.A45Page.AnswerSignType;
						ebookQuizOptionNode.A45.AnswerCount = attPage.A45Page.AnswerCount;
						ebookQuizOptionNode.A45.IsAnswerArea = attPage.A45Page.IsAnswerArea;
						break;
					case QuizType.A51: //순서맞춤
						ebookQuizOptionNode.A51.QuestionCount = attPage.A51Page.QuestionCount;
						ebookQuizOptionNode.A51.AnswerCount = attPage.A51Page.AnswerCount;
						ebookQuizOptionNode.A51.QuestionSignType = attPage.A51Page.QuestionSignType;
						ebookQuizOptionNode.A51.AnswerSignType = attPage.A51Page.AnswerSignType;
						break;
					case QuizType.A55: //순서맞춤
						ebookQuizOptionNode.A55.QuestionCount = attPage.A55Page.QuestionCount;
						ebookQuizOptionNode.A55.AnswerCount = attPage.A55Page.AnswerCount;
						ebookQuizOptionNode.A55.QuestionSignType = attPage.A55Page.QuestionSignType;
						ebookQuizOptionNode.A55.AnswerSignType = attPage.A55Page.AnswerSignType;
						break;
					case QuizType.A61: //OX
						ebookQuizOptionNode.A61.AnswerCount = attPage.A61Page.AnswerCount;
						ebookQuizOptionNode.A61.AnswerSignType = attPage.A61Page.AnswerSignType;
						ebookQuizOptionNode.A61.ToolType = attPage.A61Page.ToolType;
						break;
					case QuizType.A65: //OX
						ebookQuizOptionNode.A65.AnswerCount = attPage.A65Page.AnswerCount;
						ebookQuizOptionNode.A65.AnswerSignType = attPage.A65Page.AnswerSignType;
						ebookQuizOptionNode.A65.ToolType = attPage.A65Page.ToolType;
						ebookQuizOptionNode.A65.AnswerDirectionType = attPage.A65Page.AnswerDirectionType;
						ebookQuizOptionNode.A65.AnswerDisplayType = attPage.A65Page.AnswerDisplayType;

						quizLayoutMetaData.VerticalOffset = attPage.A65Page.VerticalOffset;
						break;
					case QuizType.C11: //단답형
						ebookQuizOptionNode.C11.AnswerCount = attPage.C11Page.AnswerCount;
						ebookQuizOptionNode.C11.AnswerSignType = attPage.C11Page.AnswerSignType;
						ebookQuizOptionNode.C11.DirectionType = attPage.C11Page.DirectionType;
						break;
					case QuizType.C15: //단답형
						ebookQuizOptionNode.C15.AnswerCount = attPage.C15Page.AnswerCount;
						ebookQuizOptionNode.C15.AnswerSignType = attPage.C15Page.AnswerSignType;
						ebookQuizOptionNode.C15.AnswerDirectionType = attPage.C15Page.AnswerDirectionType;
						ebookQuizOptionNode.C15.AnswerDisplayType = attPage.C15Page.AnswerDisplayType;

						quizLayoutMetaData.VerticalOffset = attPage.C15Page.VerticalOffset;
						break;
					case QuizType.C25: //서술형
						ebookQuizOptionNode.C25.DirectionType = attPage.C25Page.DirectionType;
						ebookQuizOptionNode.C25.QuestionSignType = attPage.C25Page.QuestionSignType;
						ebookQuizOptionNode.C25.AnswerSignType = attPage.C25Page.AnswerSignType;
						break;
					case QuizType.E11: //그려넣기
						ebookQuizOptionNode.E11.AnswerCount = attPage.E11Page.AnswerCount;
						ebookQuizOptionNode.E11.AnswerColor = attPage.E11Page.AnswerColor;
						ebookQuizOptionNode.E11.ActionType = attPage.E11Page.ActionType;
						ebookQuizOptionNode.E11.LineType = attPage.E11Page.LineType;
						ebookQuizOptionNode.E11.ToolType = attPage.E11Page.ToolType;
						ebookQuizOptionNode.E11.AnswerRunType = attPage.E11Page.AnswerRunType;
						break;
					default:
						Trace.TraceError ("SmartQuizLayoutMakerAttCore OnUpdateQuizmakerOptionPage quiztype not found : " + quizType);
						break;
				}
			}

			document.SetFormChange (true);
			_QuizMakerView.CompletePropertyChanged ();
		}

	}
}
