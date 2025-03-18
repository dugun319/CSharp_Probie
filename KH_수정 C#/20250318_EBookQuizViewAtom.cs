using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using CefSharp.DevTools.CSS;
using CefSharp.DevTools.Debugger;

using Newtonsoft.Json;

using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.Metadata;
using Softpower.SmartMaker.TopApp.Metadata.QuizMaker;
using Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView;
using Softpower.SmartMaker.TopAtom.Ebook.Components.EBookQuizView.AnswerEngine;

namespace Softpower.SmartMaker.TopAtom.Ebook
{
	public class EBookQuizViewAtom : TabViewAtom
	{
		private EBookQuizAnswerEngineManager m_EBookQuizAnswerEngineManager = new EBookQuizAnswerEngineManager ();

		//점수
		private double _Point = 0;

		#region | Property |

		public EBookQuizAnswerEngineManager QuizAnswerEngineManager
		{
			get { return m_EBookQuizAnswerEngineManager; }
		}

		public double Point
		{
			get { return _Point; }
			set { _Point = value; }
		}

		#endregion

		public override void Serialize (CArchive ar)
		{
			base.Serialize (ar);
		}

		protected override void InitializeAttrib ()
		{
			m_Attrib = new EBookQuizViewAttrib ();
		}

		public override AtomType GetUniqueEnumType ()
		{
			return AtomType.EBookQuizView;
		}

		protected override bool InitScriptEventMaps ()
		{
			m_pScriptEventMaps = new CMapWordToStringX ();

			m_pScriptEventMaps.SetAt (EVS_TYPE.EVS_A_CHECKANSWER_END, "정답여부"); // _문제풀이완료
			m_pScriptEventMaps.SetAt (EVS_TYPE.EVS_A_CHECKANSWER_SUCCEED, ""); // _문제풀이성공
			m_pScriptEventMaps.SetAt (EVS_TYPE.EVS_A_CHECKANSWER_FAILED, ""); // _문제풀이실패

			return true;
		}

		public override void PrepareSerializeData ()
		{
			base.PrepareSerializeData ();

			var atomAttrib = this.GetAttrib () as EBookQuizViewAttrib;

			atomAttrib.SerializeData.Clear ();

			foreach (KeyValuePair<Atom, EBookQuizPropertyNode> item in atomAttrib.DataMap)
			{
				var node = item.Value;
				node.Name = item.Key.GetProperVar ();
				atomAttrib.SerializeData.Add (node);
			}

			MakeAnswerGroup ();
		}

		public void MakeAnswerGroup ()
		{
			var atomAttrib = this.GetAttrib () as EBookQuizViewAttrib;
			var groupNames = new List<string> ();

			foreach (KeyValuePair<Atom, EBookQuizAnswerValueNode> item in atomAttrib.AnswerDataMap)
			{
				var groupName = item.Value.GroupName;
				var findItem = atomAttrib.SerializeAnswerData.Find (i => i.Name == groupName);

				if (null == findItem)
				{
					findItem = new EBookQuizAnswerNode ()
					{
						Name = groupName,
					};

					atomAttrib.SerializeAnswerData.Add (findItem);
				}

				bool isExists = false;

				foreach (var cValue in findItem.Values)
				{
					if (item.Value.AtomName == cValue.AtomName && item.Value.AnswerValue == cValue.AnswerValue
						&& item.Value.AnswerValueType == cValue.AnswerValueType)
					{
						isExists = true;
					}
				}

				if (false == isExists)
				{
					findItem.Values.Add (item.Value);
				}
			}
		}

		public void ExecuteAnswer ()
		{
			var ofAtom = this.GetOfAtom () as EBookQuizViewofAtom;
			ofAtom.ExecuteAnswer ();
		}

		/// 버튼에서 아톰실행 기능
		/// </summary>
		public override bool ActionAtomExecute (Atom refAtom)
		{
			bool bExecute = true;

			ExecuteAnswer ();

			return bExecute;
		}

		public override ICollection GetScriptMapEntrys ()
		{
			ArrayList scriptList = new ArrayList ();

			// 속성
			scriptList.Add (new CScriptInfo (203, 0, true, 1, true));   // 값		#0
			scriptList.Add (new CScriptInfo (286, 0, true, 1, true));   // 화면감춤	#1

			scriptList.Add (new CScriptInfo (251, 1, true, 2, false));  // 좌표		#2
			scriptList.Add (new CScriptInfo (300, 0, true, 3, false));  // 크기		#3
			scriptList.Add (new CScriptInfo (301, 0, true, 4, false));  // 높이		#4
			scriptList.Add (new CScriptInfo (306, 2, false, 5, false)); // 이동		#5

			scriptList.Add (new CScriptInfo (585, 2, false, 5, false)); // 채점하기	#6

			return scriptList;
		}

		public override int Action (int wPropID, CVariantX[] pvaArgs, CVariantX pRetVal)
		{
			switch (wPropID)
			{
				case 5: 
				case 306: return raw_MoveAtom (pvaArgs, pRetVal);
				case 6: 
				case 585: return raw_ExecuteAnswer (pvaArgs, pRetVal);
			}

			return GetProperty (wPropID, pvaArgs, pRetVal);
		}

		public override int SetProperty (int wPropID, CVariantX[] pvaArgs, CVariantX pRetVal)
		{
			switch (wPropID)
			{
				case 0:
				case 203:
					return set_QuizViewValue (pvaArgs, pRetVal);
			}

			return base.SetProperty (wPropID, pvaArgs, pRetVal);
		}

		public override int GetProperty (int wPropID, CVariantX[] pvaArgs, CVariantX pRetVal)
		{
			switch (wPropID)
			{
				case 0:
				case 203:
					return get_QuizViewValue (pvaArgs, pRetVal);
			}

			return base.GetProperty (wPropID, pvaArgs, pRetVal);
		}

		public int set_QuizViewValue (CVariantX[] pvaArgs, CVariantX pRetVal)
		{
			if (null == pRetVal)
				return StdCore.E_FAIL;

			var atomBase = this.AtomBase as EBookQuizViewofAtom;
			var jsonData = pRetVal.ToStringX ();

			atomBase.ClearQuizView ();

			if (null != jsonData)
			{
				var inputDataList = JsonConvert.DeserializeObject<List<EBookQuizAnswerNode>> (jsonData);
				var atomList = atomBase.GetAllAtomCores ();

				if (null != inputDataList)
				{
					foreach (var inputData in inputDataList)
					{
						foreach (var value in inputData.Values)
						{
							//EBookQuizAnswerValueNode
							var answerValue = value.AnswerValue?.ToString ();
							var atomName = value.AtomName;

							var atom = atomList.Find (i => i.AtomProperVar == atomName);

							if (null != atom)
							{
								atom.SetContentString (answerValue, true);
							}
						}
					}
				}
			}

			return StdCore.S_OK;
		}

		public int get_QuizViewValue (CVariantX[] pvaArgs, CVariantX pRetVal)
		{
			var atomBase = this.AtomBase as EBookQuizViewofAtom;
			var atomAttrib = this.Attrib as EBookQuizViewAttrib;

			int kind = 1 < pvaArgs?.Length ? pvaArgs[1].ToInt () : -1;

			if (kind == 1356) // #정답
			{
				//정답값
				var answerJson = JsonConvert.SerializeObject (atomAttrib.AnswerDataMap);
				pRetVal.SetValue (answerJson);
			}
			else if (kind == 1357) // #점수
			{
				//점수
				pRetVal.SetValue (atomBase.AnswerPoint);
			}
			else if (kind == 1355) //#채점
			{
				//true : 정답
				//false : 오답
				pRetVal.SetValue (atomBase.IsAnswer);
			}
			else
			{
				//사용자 입력값
				var inputNode = atomBase.MakeInputNodes ();
				if (null != inputNode)
				{
					var objList = new List<object> ();
					foreach (var i in inputNode)
					{
						objList.Add (i.Values);
					}

					var inputNodeJson = JsonConvert.SerializeObject (objList);
					pRetVal.SetValue (inputNodeJson);
				}
			}

			return StdCore.S_OK;
		}

		/// <summary>
		/// 채점하기
		/// </summary>
		/// <param name="pvaArgs"></param>
		/// <param name="pRetVal"></param>
		/// <returns></returns>
		public int raw_ExecuteAnswer (CVariantX[] pvaArgs, CVariantX pRetVal)
		{
			ExecuteAnswer ();

			return StdCore.S_OK;
		}

		//20250226 KH EBookQuizViewAtom
		public PageMetadata GetPrepareToSaveQuizBlock ()
		{
			var metaData = new PageMetadata ()
			{
				QuizMetaData = new QuizMakerMetaData ()
			};

			EBookQuizViewAttrib attrib = m_Attrib as EBookQuizViewAttrib;

			//EBook으로 초기화되어 있어서  Type 변경
			metaData.QuizMetaData.QuizType = attrib.DisplayQuizType;

			var dataMap = attrib.DataMap;
			var answerMap = attrib.AnswerDataMap;
			var answerCount = 0;
			var questionCount = 0;

			// QuizMetaData.DataList 전체 DataList 정보 저장
			foreach (var item in dataMap)
			{
				RemovePrefixQAFromObject (item.Key);
				RemovePrefixQAFromObject (item.Value);
				
				item.Value.Name = item.Key.GetProperVar ();

				metaData.QuizMetaData.DataList.Add (new KeyValuePair<object, EBookQuizPropertyNode> (item.Key, item.Value));

				if (item.Value.ActionType == QuizAction.Answer)
				{
					answerCount++;
				}
				else if (item.Value.ActionType == QuizAction.Question)
				{
					questionCount++;
				}
			}

			// QuizMetaData.AnswerDataList 정답관련 정보 저장
			foreach (var item in answerMap)
			{
				RemovePrefixQAFromObject (item.Key);
				RemovePrefixQAFromObject (item.Value);

				metaData.QuizMetaData.AnswerDataList.Add (new KeyValuePair<object, EBookQuizAnswerValueNode> (item.Key, item.Value));
			}

			switch (metaData.QuizMetaData.QuizType)
			{
				case QuizType.A11:
					metaData.QuizMetaData.EBookQuizOptionNode.A11.AnswerCount = answerCount;
					break;
				case QuizType.A21:
					metaData.QuizMetaData.EBookQuizOptionNode.A21.QuestionCount = questionCount;
					metaData.QuizMetaData.EBookQuizOptionNode.A21.AnswerCount = answerCount;
					metaData.QuizMetaData.EBookQuizOptionNode = attrib.EBookQuizOptionNode;
					break;
				case QuizType.A31:
					metaData.QuizMetaData.EBookQuizOptionNode.A31.QuestionCount = questionCount;
					metaData.QuizMetaData.EBookQuizOptionNode.A31.AnswerCount = answerCount;
					break;
				case QuizType.A41:
					metaData.QuizMetaData.EBookQuizOptionNode.A41.AnswerCount = answerCount;
					break;
				case QuizType.A61:
					metaData.QuizMetaData.EBookQuizOptionNode.A61.AnswerCount = answerCount;
					break;
				case QuizType.C11:
					metaData.QuizMetaData.EBookQuizOptionNode.C11.AnswerCount = answerCount;
					break;
				case QuizType.E11:
					metaData.QuizMetaData.EBookQuizOptionNode.E11.AnswerCount = answerCount;
					metaData.QuizMetaData.EBookQuizOptionNode = attrib.EBookQuizOptionNode;
					break;
			}
		
			return metaData;
		}

		// 원래 값을 저장할 Dictionary (객체 참조를 키로 사용)
		private readonly Dictionary<object, Dictionary<string, string>> _originalValues = new Dictionary<object, Dictionary<string, string>> ();

		// QuizBlock 생성할 때 만들어지는 Prefix를 제거하는 로직 (QuizView에서는 Prefix가 없어야지 정상적으로 문항/답항을 인식)
		public void RemovePrefixQAFromObject (object obj)
		{
			if (obj == null) return;

			Type type = obj.GetType ();
			PropertyInfo[] properties = type.GetProperties ();

			// Prefix를 원복하귀 위한 객체생성
			if (!_originalValues.ContainsKey (obj))
			{
				_originalValues[obj] = new Dictionary<string, string> ();
			}

			foreach (PropertyInfo prop in properties)
			{
				// 인덱서 (매개변수 필요) 속성은 무시
				if (prop.GetIndexParameters ().Length > 0) continue;

				if (prop.PropertyType == typeof (string) && prop.CanRead && prop.CanWrite)
				{
					string value = (string)prop.GetValue (obj);

					if (!string.IsNullOrEmpty (value) && (value.Contains ("A_") || value.Contains ("B_") || value.Contains ("C_")))
					{
						// 원래 값을 저장 (기존 값이 없을 때만 저장)
						if (!_originalValues[obj].ContainsKey (prop.Name))
						{
							_originalValues[obj][prop.Name] = value;
						}

						// "A_" 또는 "B_" 또는 "C_" 제거
						// AnswerValue Object는 substring 방식으로 처리 (아무 의미 없음)
						prop.SetValue (obj, value.Replace ("A_", "").Replace ("B_", "").Replace ("C_", ""));
					}
				}
				else if (prop.PropertyType != typeof (string) && prop.CanRead && prop.CanWrite)
				{
					// `AnswerValue`가 객체인 경우 단 한번만 접근
					object nestedObj = prop.GetValue (obj);

					// 만약 이 객체가 null이 아니고 `AnswerValue`를 처리하는 경우
					if (nestedObj != null && prop.Name == "AnswerValue")
					{
						// 객체가 또 다른 객체일 경우 재귀 없이 한 번만 처리
						if (nestedObj is string)
						{
							// `AnswerValue`가 string이라면 처리
							string nestedValue = (string)nestedObj;
							if (!string.IsNullOrEmpty (nestedValue) && (nestedValue.StartsWith ("A_") || nestedValue.StartsWith ("B_") || nestedValue.StartsWith ("C_")))
							{
								// 원래 값을 저장 (기존 값이 없을 때만 저장)
								if (!_originalValues[obj].ContainsKey (prop.Name))
								{
									_originalValues[obj][prop.Name] = nestedValue;
								}

								prop.SetValue (obj, nestedValue.Replace ("A_", "").Replace ("B_", "").Replace ("C_", ""));
							}
						}
						else
						{
							RemovePrefixQAFromObject (nestedObj);
						}						
					}
				}
			}
		}

		// QuizBlock 생성을 위해 삭제한 Prefix를 복원하는 로직 (QuizView에서는 Prefix가 없어야지 정상적으로 문항/답항을 인식)
		public void RestoreOriginalValues (object obj)
		{
			if (obj == null || !_originalValues.ContainsKey (obj)) return;

			Type type = obj.GetType ();
			PropertyInfo[] properties = type.GetProperties ();

			foreach (PropertyInfo prop in properties)
			{
				if (prop.PropertyType == typeof (string) && prop.CanRead && prop.CanWrite && _originalValues[obj].ContainsKey (prop.Name))
				{
					// 원래 값 복원
					prop.SetValue (obj, _originalValues[obj][prop.Name]);
				}				
				else if (!prop.PropertyType.IsPrimitive && prop.PropertyType != typeof (string) && prop.CanRead)
				{
					// **객체 타입인 경우 내부 탐색**
					object nestedObj = prop.GetValue (obj);

					// 객체가 또 다른 객체일 경우 재귀 없이 한 번만 처리
					if (nestedObj is string)
					{
						// 원래 값 복원 (Key가 없는 경우 있음)
						if (_originalValues[obj].ContainsKey (prop.Name))
						{
							prop.SetValue (obj, _originalValues[obj][prop.Name]);
						}
					}
					else
					{
						RestoreOriginalValues (nestedObj);
					}

				}				
			}
			// 복원할 대상이 있었다면 _originalValues에서 제거
			if (_originalValues.ContainsKey (obj))
			{
				_originalValues.Remove (obj);
			}
		}
	}
}
