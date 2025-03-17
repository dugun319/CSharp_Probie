using System;
using System.Collections.Generic;
using System.Windows.Media;

using Softpower.SmartMaker.TopApp.CommonLib;

using static Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView.EBookQuizOptionNode;

namespace Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView
{
	/// <summary>
	/// [과거 버전] 문항별 퀴즈 옵션 정보, 현재 미사용
	/// </summary>
	public class EBookQuizOptionNode_Old
	{
		public QuizOption_A11 A1 { get; set; } = new QuizOption_A11 ();
		public QuizOption_A21 A2 { get; set; } = new QuizOption_A21 ();
		public QuizOption_A31 A3 { get; set; } = new QuizOption_A31 ();
		public QuizOption_A41 A4 { get; set; } = new QuizOption_A41 ();
		public QuizOption_A51 A5 { get; set; } = new QuizOption_A51 ();
		public QuizOption_A61 A6 { get; set; } = new QuizOption_A61 ();

		public QuizOption_C11 C1 { get; set; } = new QuizOption_C11 ();
		public QuizOption_E11 E1 { get; set; } = new QuizOption_E11 ();
	}

	/// <summary>
	/// 문항별 퀴즈 옵션 정보
	/// </summary>
	public class EBookQuizOptionNode
	{
		public QuizOption_A11 A11 { get; set; } = new QuizOption_A11 ();	// 선다형 학습용
		public QuizOption_A15 A15 { get; set; } = new QuizOption_A15 ();    // 선다형 평가형

		public QuizOption_A21 A21 { get; set; } = new QuizOption_A21 ();    // 선긋기 학습용
		public QuizOption_A25 A25 { get; set; } = new QuizOption_A25 ();    // 선긋기 평가형

		public QuizOption_A31 A31 { get; set; } = new QuizOption_A31 ();    // 끌어놓기 학습용
		public QuizOption_A35 A35 { get; set; } = new QuizOption_A35 ();    // 끌어놓기 평가형

		public QuizOption_A41 A41 { get; set; } = new QuizOption_A41 ();    // 빈칸채움 학습용
		public QuizOption_A45 A45 { get; set; } = new QuizOption_A45 ();    // 빈칸채움 평가형

		public QuizOption_A51 A51 { get; set; } = new QuizOption_A51 ();    // 순서맞춤 학습용
		public QuizOption_A55 A55 { get; set; } = new QuizOption_A55 ();    // 순서맞춤 평가형

		public QuizOption_A61 A61 { get; set; } = new QuizOption_A61 ();    // OX형 학습용
		public QuizOption_A65 A65 { get; set; } = new QuizOption_A65 ();    // OX형 평가형

		public QuizOption_C11 C11 { get; set; } = new QuizOption_C11 ();    // 단답형 학습용
		public QuizOption_C15 C15 { get; set; } = new QuizOption_C15 ();    // 단답형 평가형

		public QuizOption_C21 C21 { get; set; } = new QuizOption_C21 ();    // 서술형 학습용
		public QuizOption_C25 C25 { get; set; } = new QuizOption_C25 ();    // 서술형 평가형

		public QuizOption_E11 E11 { get; set; } = new QuizOption_E11 ();    // 그려넣기 학습용

		/// <summary>
		/// 선다형 속성
		/// </summary>
		public class QuizOption_A11
		{
			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우", //"사용자 정의",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> AnswerEffectList { get; } = new List<string> ()
			{
				"원형", "체크",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public int AnswerEffectWidth { get; set; } = 25;
			public int AnswerEffectHeight { get; set; } = 25;

			/// <summary>
			/// 배열방식
			/// </summary>
			public int DirectionType { get; set; } = 0; //0 : 상하, 1 : 좌우, 2 : 사용자 정의

			/// <summary>
			/// 선택표시
			/// </summary>
			public int AnswerEffect { get; set; } = 0; //0 : 원형, 1 : 체크

			/// <summary>
			/// 답항수
			/// </summary>
			public int AnswerCount { get; set; } = 4;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 1;

			/// <summary>
			/// 중복답안 허용
			/// </summary>
			public bool IsMultiAnswer { get; set; } = false;

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}

			public bool ContainsAnswerSign (string s)
			{
				foreach (var item in AnswerSignMap)
				{
					if (item.Value.Contains (s))
						return true;
				}

				return false;
			}
		}


		/// <summary>
		/// 선다형 속성
		/// </summary>
		public class QuizOption_A15
		{
			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우", //"사용자 정의",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> AnswerEffectList { get; } = new List<string> ()
			{
				"원형", "체크",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public int AnswerEffectWidth { get; set; } = 25;
			public int AnswerEffectHeight { get; set; } = 25;

			/// <summary>
			/// 배열방식
			/// </summary>
			public int DirectionType { get; set; } = 0; //0 : 상하, 1 : 좌우, 2 : 사용자 정의

			/// <summary>
			/// 선택표시
			/// </summary>
			public int AnswerEffect { get; set; } = 0; //0 : 원형, 1 : 체크

			/// <summary>
			/// 답항수
			/// </summary>
			public int AnswerCount { get; set; } = 4;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 1;

			/// <summary>
			/// 중복답안 허용
			/// </summary>
			public bool IsMultiAnswer { get; set; } = false;

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}

			public bool ContainsAnswerSign (string s)
			{
				foreach (var item in AnswerSignMap)
				{
					if (item.Value.Contains (s))
						return true;
				}

				return false;
			}
		}

		/// <summary>
		/// 선긋기 속성
		/// </summary>
		public class QuizOption_A21
		{
			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> LineTypeList { get; } = new List<string> ()
			{
				"──────",
				"---------------",
			};

			public static List<string> LineThicknessList { get; } = new List<string> ()
			{
				"1",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
			};

			public static List<string> LineColorList { get; } = new List<string> ()
			{
				"Black",
				"DarkGray",
				"Gray",
				"Red",
				"Orange",
				"Yellow",
				"Green",
				"Blue",
				"Navy",
				"Purple",				
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public static Dictionary<int, List<string>> QuestionSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			/// <summary>
			/// 배열방식
			/// </summary>
			public int DirectionType { get; set; } = 0; //0 : 상하, 1 : 좌우

			/// <summary>
			/// 선형패턴
			/// </summary>
			public int LineType { get; set; } = 0; // 0 : 실선, 1 : 점선

			/// <summary>
			// 정답선색상
			/// </summary>
			public int LineColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Black);

			public int EllipseColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Black);

			public double LineThickness { get; set; } = 0;

			// 선택된 선 두께 설정
			public void SetLineThickness (int selectedIndex)
			{
				if (selectedIndex >= 0 && selectedIndex < LineThicknessList.Count)
				{
					// LineThicknessList의 선택된 값(문자열)을 double로 변환하여 LineThickness에 설정
					LineThickness = Convert.ToDouble (LineThicknessList[selectedIndex]);
				}
			}

			// 선택된 선 색상 설정
			public void SetLineColor (int selectedIndex)
			{
				if (selectedIndex >= 0 && selectedIndex < LineColorList.Count)
				{
					// LineColorList의 선택된 색상(문자열)을 Brush로 변환하여 LineColor에 설정
					var colorName = LineColorList[selectedIndex];
					var colorBrush = (Brush)new BrushConverter ().ConvertFromString (colorName);
					LineColor = WPFColorConverter.ConvertMediaBrushToArgb (colorBrush);
				}
			}


			/// <summary>
			/// 문항수
			/// </summary>
			public int QuestionCount { get; set; } = 5;

			/// <summary>
			/// 답항수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 문항기호
			/// </summary>
			public int QuestionSignType { get; set; } = 0;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			/// <summary>
			/// 좌우여백
			/// </summary>
			public int HorizontalOffset { get; set; } = 5;

			/// <summary>
			/// 상하여백
			/// </summary>
			public int VerticalOffset { get; set; } = 5;

			/// <summary>
			/// 이벤트 객체 넓이
			/// </summary>
			public int ObjectWidth { get; set; } = 10;

			/// <summary>
			/// 이벤트 객체 높이
			/// </summary>
			public int ObjectHeight { get; set; } = 10;

			/// <summary>
			/// 이벤트 객체 배경색
			/// </summary>
			public int ObjectBackgroundColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Gray);

			/// <summary>
			/// 이벤트 객체 테두리색
			/// </summary>
			public int ObjectBorderColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Gray);

			public string GetQuestionSign (int index)
			{
				if (-1 < QuestionSignType && QuestionSignMap.ContainsKey (QuestionSignType))
				{
					if (-1 < index && index < QuestionSignMap[QuestionSignType].Count)
					{
						return QuestionSignMap[QuestionSignType][index];
					}
				}

				return "";
			}

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}

			public bool ContainsAnswerSign (string s)
			{
				foreach (var item in AnswerSignMap)
				{
					if (item.Value.Contains (s))
						return true;
				}

				return false;
			}
		}


		/// <summary>
		/// 선긋기 속성
		/// </summary>
		public class QuizOption_A25
		{
			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> LineTypeList { get; } = new List<string> ()
			{
				"──────",
				"---------------",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public static Dictionary<int, List<string>> QuestionSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public static List<string> LineThicknessList { get; } = new List<string> ()
			{
				"1",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
			};

			/// <summary>
			/// 배열방식
			/// </summary>
			public int DirectionType { get; set; } = 0; //0 : 상하, 1 : 좌우

			/// <summary>
			/// 선형패턴
			/// </summary>
			public int LineType { get; set; } = 0; // 0 : 실선, 1 : 점선

			/// <summary>
			/// 선색상
			/// </summary>
			public int LineColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Black);

			public int EllipseColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Black);

			public double LineThickness { get; set; } = 0;

			// 선택된 선 두께 설정
			public void SetLineThickness (int selectedIndex)
			{
				if (selectedIndex >= 0 && selectedIndex < LineThicknessList.Count)
				{
					// LineThicknessList의 선택된 값(문자열)을 double로 변환하여 LineThickness에 설정
					LineThickness = Convert.ToDouble (LineThicknessList[selectedIndex]);
				}
			}


			/// <summary>
			/// 문항수
			/// </summary>
			public int QuestionCount { get; set; } = 5;

			/// <summary>
			/// 답항수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 문항기호
			/// </summary>
			public int QuestionSignType { get; set; } = 0;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			/// <summary>
			/// 좌우여백
			/// </summary>
			public int HorizontalOffset { get; set; } = 5;

			/// <summary>
			/// 상하여백
			/// </summary>
			public int VerticalOffset { get; set; } = 5;

			/// <summary>
			/// 이벤트 객체 넓이
			/// </summary>
			public int ObjectWidth { get; set; } = 8;

			/// <summary>
			/// 이벤트 객체 높이
			/// </summary>
			public int ObjectHeight { get; set; } = 8;

			/// <summary>
			/// 이벤트 객체 배경색
			/// </summary>
			public int ObjectBackgroundColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Gray);

			/// <summary>
			/// 이벤트 객체 테두리색
			/// </summary>
			public int ObjectBorderColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Gray);

			public string GetQuestionSign (int index)
			{
				if (-1 < QuestionSignType && QuestionSignMap.ContainsKey (QuestionSignType))
				{
					if (-1 < index && index < QuestionSignMap[QuestionSignType].Count)
					{
						return QuestionSignMap[QuestionSignType][index];
					}
				}

				return "";
			}

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}

			public bool ContainsAnswerSign (string s)
			{
				foreach (var item in AnswerSignMap)
				{
					if (item.Value.Contains (s))
						return true;
				}

				return false;
			}
		}

		/// <summary>
		/// 끌어놓기 속성
		/// </summary>
		public class QuizOption_A31
		{
			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우",
			};

			public static List<string> AnswerRunTypeList { get; } = new List<string> ()
			{
				"완전일치",
				"부분일치",
			};

			public static List<string> QuestionSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public static Dictionary<int, List<string>> QuestionSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			/// <summary>
			/// 이동방식
			/// 0 : 확정이동
			/// 1 : 영역이동
			/// </summary>
			public int ActionType { get; set; } = 0;

			/// <summary>
			/// 채점 방식
			/// </summary>
			public int AnswerRunType { get; set; } = 0;

			/// <summary>
			/// 배열방식
			/// </summary>
			public int DirectionType { get; set; } = 0; //0 : 상하, 1 : 좌우

			/// <summary>
			/// 문항수
			/// </summary>
			public int QuestionCount { get; set; } = 5;

			/// <summary>
			/// 답항수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 문항기호
			/// </summary>
			public int QuestionSignType { get; set; } = 0;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			public string GetQuestionSign (int index)
			{
				if (-1 < QuestionSignType && QuestionSignMap.ContainsKey (QuestionSignType))
				{
					if (-1 < index && index < QuestionSignMap[QuestionSignType].Count)
					{
						return QuestionSignMap[QuestionSignType][index];
					}
				}

				return "";
			}

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}

			public bool ContainsAnswerSign (string s)
			{
				foreach (var item in AnswerSignMap)
				{
					if (item.Value.Contains (s))
						return true;
				}

				return false;
			}
		}

		/// <summary>
		/// 끌어놓기 속성
		/// </summary>
		public class QuizOption_A35
		{
			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우",
			};

			public static List<string> AnswerRunTypeList { get; } = new List<string> ()
			{
				"완전일치",
				"부분일치",
			};

			public static List<string> QuestionSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public static Dictionary<int, List<string>> QuestionSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			/// <summary>
			/// 이동방식
			/// 0 : 확정이동
			/// 1 : 영역이동
			/// </summary>
			public int ActionType { get; set; } = 0;

			/// <summary>
			/// 채점 방식
			/// </summary>
			public int AnswerRunType { get; set; } = 0;

			/// <summary>
			/// 배열방식
			/// </summary>
			public int DirectionType { get; set; } = 0; //0 : 상하, 1 : 좌우

			/// <summary>
			/// 문항수
			/// </summary>
			public int QuestionCount { get; set; } = 5;

			/// <summary>
			/// 답항수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 문항기호
			/// </summary>
			public int QuestionSignType { get; set; } = 0;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			public string GetQuestionSign (int index)
			{
				if (-1 < QuestionSignType && QuestionSignMap.ContainsKey (QuestionSignType))
				{
					if (-1 < index && index < QuestionSignMap[QuestionSignType].Count)
					{
						return QuestionSignMap[QuestionSignType][index];
					}
				}

				return "";
			}

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}

			public bool ContainsAnswerSign (string s)
			{
				foreach (var item in AnswerSignMap)
				{
					if (item.Value.Contains (s))
						return true;
				}

				return false;
			}
		}

		/// <summary>
		/// 빈칸채움 속성
		/// </summary>
		public class QuizOption_A41
		{
			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			/// <summary>
			/// 답항형식
			/// </summary>
			public int AnswerType { get; set; } = 0;

			/// <summary>
			/// 답항 수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			/// <summary>
			/// 퀴즈메이커 전용 - 답란 별도생성
			/// </summary>
			public bool IsAnswerArea { get; set; } = true;

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}
		}

		/// <summary>
		/// 빈칸채움 속성
		/// </summary>
		public class QuizOption_A45
		{
			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			/// <summary>
			/// 답항형식
			/// </summary>
			public int AnswerType { get; set; } = 0;

			/// <summary>
			/// 답항 수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			/// <summary>
			/// 퀴즈메이커 전용 - 답란 별도생성
			/// </summary>
			public bool IsAnswerArea { get; set; } = true;

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}
		}

		/// <summary>
		/// 순서맞춤
		/// </summary>
		public class QuizOption_A51
		{
			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우",
			};

			public static List<string> QuestionSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public static Dictionary<int, List<string>> QuestionSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};


			/// <summary>
			/// 문항수
			/// </summary>
			public int QuestionCount { get; set; } = 5;

			/// <summary>
			/// 답항수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 문항기호
			/// </summary>
			public int QuestionSignType { get; set; } = 0;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			public string GetQuestionSign (int index)
			{
				if (-1 < QuestionSignType && QuestionSignMap.ContainsKey (QuestionSignType))
				{
					if (-1 < index && index < QuestionSignMap[QuestionSignType].Count)
					{
						return QuestionSignMap[QuestionSignType][index];
					}
				}

				return "";
			}

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}

			public bool ContainsAnswerSign (string s)
			{
				foreach (var item in AnswerSignMap)
				{
					if (item.Value.Contains (s))
						return true;
				}

				return false;
			}
		}

		/// <summary>
		/// 순서맞춤
		/// </summary>
		public class QuizOption_A55
		{
			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우",
			};

			public static List<string> QuestionSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public static Dictionary<int, List<string>> QuestionSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};


			/// <summary>
			/// 문항수
			/// </summary>
			public int QuestionCount { get; set; } = 5;

			/// <summary>
			/// 답항수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 문항기호
			/// </summary>
			public int QuestionSignType { get; set; } = 0;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			public string GetQuestionSign (int index)
			{
				if (-1 < QuestionSignType && QuestionSignMap.ContainsKey (QuestionSignType))
				{
					if (-1 < index && index < QuestionSignMap[QuestionSignType].Count)
					{
						return QuestionSignMap[QuestionSignType][index];
					}
				}

				return "";
			}

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}

			public bool ContainsAnswerSign (string s)
			{
				foreach (var item in AnswerSignMap)
				{
					if (item.Value.Contains (s))
						return true;
				}

				return false;
			}
		}

		/// <summary>
		/// OX형
		/// </summary>
		public class QuizOption_A61
		{
			public static List<string> AnswerDirectionTypeList { get; } = new List<string>
			{
				"답항 우측",
				"답항 좌측",
			};

			public static List<string> AnswerDisplayTypeList { get; } = new List<string>
			{
				"없음",
				"(괄호)",
			};

			public static List<string> AnswerDisplayValueList { get; } = new List<string>
			{
				"",
				"(    )",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> ToolTypeList { get; } = new List<string>
			{
				"O - X",
				"부등호 (<=>)",
				"순서설정",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			/// <summary>
			/// 답항 수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			/// <summary>
			/// 답항형식
			/// </summary>
			public int AnswerType { get; set; } = 0;

			/// <summary>
			/// 선택표시
			/// </summary>
			public int AnswerEffect { get; set; } = 0;

			/// <summary>
			/// 동작방식
			/// </summary>
			public int ToolType { get; set; } = 0;  // 0 : OX , 1 : 부등호(간략), 2 : 부등호 (상세)


			/// <summary>
			/// 퀴즈메이커 전용 - 배열 방식  0 : 답항 우측, 답항 좌측
			/// </summary>
			public int AnswerDirectionType { get; set; } = 0;

			/// <summary>
			/// 퀴즈메이커 전용 - 답항 형식  0 : 공백, 1 : 괄호
			/// </summary>
			public int AnswerDisplayType { get; set; } = 1;

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}
		}

		/// <summary>
		/// OX형
		/// </summary>
		public class QuizOption_A65
		{
			public static List<string> AnswerDirectionTypeList { get; } = new List<string>
			{
				"답항 우측",
				"답항 좌측",
			};

			public static List<string> AnswerDisplayTypeList { get; } = new List<string>
			{
				"없음",
				"(괄호)",
			};

			public static List<string> AnswerDisplayValueList { get; } = new List<string>
			{
				"",
				"(    )",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> ToolTypeList { get; } = new List<string>
			{
				"O - X",
				"부등호 (<=>)",
				"순서설정",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			/// <summary>
			/// 답항 수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			/// <summary>
			/// 답항형식
			/// </summary>
			public int AnswerType { get; set; } = 0;

			/// <summary>
			/// 선택표시
			/// </summary>
			public int AnswerEffect { get; set; } = 0;

			/// <summary>
			/// 동작방식
			/// </summary>
			public int ToolType { get; set; } = 0;  // 0 : OX , 1 : 부등호(간략), 2 : 부등호 (상세)


			/// <summary>
			/// 퀴즈메이커 전용 - 배열 방식  0 : 답항 우측, 답항 좌측
			/// </summary>
			public int AnswerDirectionType { get; set; } = 0;

			/// <summary>
			/// 퀴즈메이커 전용 - 답항 형식  0 : 공백, 1 : 괄호
			/// </summary>
			public int AnswerDisplayType { get; set; } = 1;

			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}
		}

		/// <summary>
		/// 단답형 문제속성
		/// </summary>
		public class QuizOption_C11
		{
			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우",
			};

			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> AnswerRunTypeList { get; } = new List<string> ()
			{
				"완전일치",
				"부분일치",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			/// <summary>
			/// 배열방식
			/// </summary>
			public int DirectionType { get; set; } = 0; //0 : 상하, 1 : 좌우

			/// <summary>
			/// 답항 수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			/// <summary>
			/// 채점 방식
			/// </summary>
			public int AnswerRunType { get; set; } = 0;

			/// <summary>
			/// 답항형식
			/// </summary>
			public int AnswerType { get; set; } = 0;

			/// <summary>
			/// 선택표시
			/// </summary>
			public int AnswerEffect { get; set; } = 0;

			/// <summary>
			/// 답항기호 리턴
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}
		}

		/// <summary>
		/// 단답형 문제속성
		/// </summary>
		public class QuizOption_C15
		{
			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> AnswerRunTypeList { get; } = new List<string> ()
			{
				"완전일치",
				"부분일치",
			};

			public static List<string> AnswerDirectionTypeList { get; } = new List<string>
			{
				"답항 우측",
				"답항 좌측",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public static List<string> AnswerDisplayTypeList { get; } = new List<string>
			{
				"없음",
				"(괄호)",
			};

			public static List<string> AnswerDisplayValueList { get; } = new List<string>
			{
				"",
				"(    )",
			};

			/// <summary>
			/// 답항 수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			/// <summary>
			/// 채점 방식
			/// </summary>
			public int AnswerRunType { get; set; } = 0;

			/// <summary>
			/// 답항형식
			/// </summary>
			public int AnswerType { get; set; } = 0;

			/// <summary>
			/// 선택표시
			/// </summary>
			public int AnswerEffect { get; set; } = 0;

			/// <summary>
			/// 퀴즈메이커 전용 - 배열 방식  0 : 답항 우측, 답항 좌측
			/// </summary>
			public int AnswerDirectionType { get; set; } = 0;

			/// <summary>
			/// 퀴즈메이커 전용 - 답항 형식  0 : 공백, 1 : 괄호
			/// </summary>
			public int AnswerDisplayType { get; set; } = 1;

			/// <summary>
			/// 답항기호 리턴
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}
		}

		/// <summary>
		/// 서술형 문제속성
		/// </summary>
		public class QuizOption_C21
		{
			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			/// <summary>
			/// 답항 수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			/// <summary>
			/// 답항기호 리턴
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}
		}

		/// <summary>
		/// 서술형 문제속성
		/// </summary>
		public class QuizOption_C25
		{
			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우",
			};


			public static List<string> AnswerSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static List<string> QuestionSignTypeList { get; } = new List<string>
			{
				"없음", "①", "ⓐ", "㉠", "㉮",
			};

			public static Dictionary<int, List<string>> AnswerSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			public static Dictionary<int, List<string>> QuestionSignMap { get; } = new Dictionary<int, List<string>>
			{
				{0, new List<string> () { "", "", "", "", "", "", "", "", "", "" } },
				{1, new List<string> () { "①", "②","③","④","⑤","⑥","⑦","⑧","⑨", "⑩"} },
				{2, new List<string> () { "ⓐ","ⓑ","ⓒ","ⓓ","ⓔ","ⓕ","ⓖ","ⓗ","ⓘ","ⓙ", } },
				{3, new List<string> () { "㉠", "㉡", "㉢", "㉣", "㉤", "㉥", "㉦", "㉧", "㉨", "㉩", } },
				{4, new List<string> () { "㉮", "㉯", "㉰", "㉱", "㉲", "㉳", "㉴", "㉵", "㉶", "㉷", } },
			};

			/// <summary>
			/// 배열방식
			/// </summary>
			public int DirectionType { get; set; } = 0; //0 : 상하, 1 : 좌우

			/// <summary>
			/// 답항기호
			/// </summary>
			public int AnswerSignType { get; set; } = 0;

			/// <summary>
			/// 문항기호
			/// </summary>
			public int QuestionSignType { get; set; } = 0;

			/// <summary>
			/// 답항기호 리턴
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public string GetAnswerSign (int index)
			{
				if (-1 < AnswerSignType && AnswerSignMap.ContainsKey (AnswerSignType))
				{
					if (-1 < index && index < AnswerSignMap[AnswerSignType].Count)
					{
						return AnswerSignMap[AnswerSignType][index];
					}
				}

				return "";
			}

			public string GetQuestionSign (int index)
			{
				if (-1 < QuestionSignType && QuestionSignMap.ContainsKey (QuestionSignType))
				{
					if (-1 < index && index < QuestionSignMap[QuestionSignType].Count)
					{
						return QuestionSignMap[QuestionSignType][index];
					}
				}

				return "";
			}
		}

		/// <summary>
		/// 그려넣기 문제속성
		/// </summary>
		public class QuizOption_E11
		{
			public enum RectType
			{
				None = -1,
				Line,
				Rectangle,
				Ellipse,
			};

			public static List<string> DirectionTypeStrList { get; } = new List<string> ()
			{
				"상하", "좌우", //"사용자 정의",
			};

			public static List<string> ActionTypeList { get; } = new List<string>
			{
				"선형 그리기",
				"도형 그리기",
				"따라 그리기",
				"색상 칠하기",
			};

			public static List<string> LineTypeList { get; } = new List<string> ()
			{
				"──────",
				"---------------",
			};

			public static List<string> LineThicknessList { get; } = new List<string> ()
			{
				"1",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
			};

			public static List<string> FillToolTypeList { get; } = new List<string> ()
			{
				"그림 붓",
				"영역 칠",
			};

			public static List<string> RectToolTypeList { get; } = new List<string> ()
			{
				"직선",
				"사각형",
				"원",
			};

			public static List<string> AnswerRunTypeList { get; } = new List<string> ()
			{
				"영역일치",
				"개수일치",
			};

			/// <summary>
			/// 배열방식
			/// </summary>
			public int DirectionType { get; set; } = 0; //0 : 상하, 1 : 좌우

			/// <summary>
			/// 풀이방식
			/// </summary>
			public int ActionType { get; set; } = 0;

			/// <summary>
			/// 선형 패턴
			/// </summary>
			public int LineType { get; set; } = 0;

			/// <summary>
			/// 색칠 도구
			/// </summary>
			public int ToolType { get; set; } = 0;

			/// <summary>
			/// 도형 도구
			/// </summary>
			public int ShapeType { get; set; } = 0;

			/// <summary>
			/// 색상 선택
			/// </summary>
			public int AnswerColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Black);

			/// <summary>
			/// 답항수
			/// </summary>
			public int AnswerCount { get; set; } = 5;

			/// <summary>
			/// 답항 객체
			/// </summary>
			public int AnswerObjectType { get; set; } = 0;

			/// <summary>
			/// 채점 방식
			/// </summary>
			public int AnswerRunType { get; set; } = 0;

			// LineColor & Thickness 추가
			public int LineColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Black);
			public double LineThickness { get; set; } = 0;
			public int BackColor { get; set; } = WPFColorConverter.ConvertMediaBrushToArgb (Brushes.Black);
		}

	}
}