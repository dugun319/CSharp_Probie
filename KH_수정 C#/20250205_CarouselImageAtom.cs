using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.CommonLib.BusyIndicators;
using Softpower.SmartMaker.TopAtom;

namespace Softpower.SmartMaker.TopSmartAtom
{
	[Serializable]
	public class CarouselImageAtom : BrowseAtom
	{
		private ArrayList alRealDataRows = new ArrayList ();
		private ArrayList alDisplayDataRows = new ArrayList ();
		private static int alRowCount;

        public static int AlRowCount
        {
            get { return alRowCount; }
			set { alRowCount = value; }
        }

        public ArrayList RealDataRows
		{
			get { return alRealDataRows; }
		}

		public ArrayList DisplayDataRows
		{
			get { return alDisplayDataRows; }
		}

		public override CObArray BrowseItemList
		{
			get
			{
				CarouselImageAttrib atomAttrib = (CarouselImageAttrib)GetAttrib ();
				return atomAttrib.BrowseItemList;
			}
		}

		public override CObArray OABrowse
		{
			get
			{
				CarouselImageAttrib atomAttrib = (CarouselImageAttrib)GetAttrib ();
				return atomAttrib.BrowseItemList;
			}
			set
			{
				CarouselImageAttrib atomAttrib = (CarouselImageAttrib)GetAttrib ();
				atomAttrib.BrowseItemList = value;
			}
		}

		public CarouselImageAtom ()
			: base ()
		{
			//
			// TODO: 여기에 생성자 논리를 추가합니다.
			//		
		}

		public override AtomType GetUniqueEnumType ()
		{
			return AtomType.CarouselImage;
		}

		///////////////////////////////////////////////////////
		// Serialize.... <2003. 10. 31. cys>
		public override void Serialize (CArchive ar)
		{
			base.Serialize (ar);
		}

		//////////////////////////////////////////////////////////////////////////

		protected override void InitializeAttrib ()
		{
			m_Attrib = new CarouselImageAttrib ();
		}

		public override bool DirectExecute (int dwOption)
		{
			InitCarouselImage ();

			CarouselImageAttrib pImageCarouselAttrib = this.GetAttrib () as CarouselImageAttrib;
			if (false != pImageCarouselAttrib.IsDirectExecute)
				return ExecuteBrowse ();

			return false;
		}

		public override bool BrowseStart (string strWhere, Attrib pAttrib, bool bExecute, bool bScript)
		{
			InitCarouselImage ();
			return ExecuteBrowse ();
		}

		private void InitCarouselImage ()
		{
			CarouselImageofAtom ofAtom = this.GetOfAtom () as CarouselImageofAtom;
			ofAtom?.ClearImages ();
			this.SetMappingFieldIndexAndColumnIndex ();
		}

		private bool ExecuteBrowse ()
		{
			bool isLoad = true;

			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;

			if (null != pCarouselofAtom)
			{
				isLoad = LoadImageData ();

				if (false != isLoad)
				{
					ViewImageData ();
				}
			}

			return isLoad;
		}

		public void ViewImageData ()
		{
			CarouselImageAttrib pImageCarouselAttrib = this.GetAttrib () as CarouselImageAttrib;
			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;

			if (null != pCarouselofAtom && null != pImageCarouselAttrib)
			{
				pCarouselofAtom.DefaultSelectIndex ();
				if (pImageCarouselAttrib.SlideAutoPlay)
				{
					pCarouselofAtom.CarouselContainer.AutoPlaySlideStart (pImageCarouselAttrib.SlidePlayCycle);
				}
			}
		}

		private bool LoadImageData ()
		{
			CarouselImageAttrib pImageCarouselAttrib = this.GetAttrib () as CarouselImageAttrib;
			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;

			int nDBIndex = 0;
			if (0 < this.BrowseItemList.Count)
			{
				BrowseItem pBrowseAtom = this.BrowseItemList[0] as BrowseItem;
				nDBIndex = pBrowseAtom.DBIndex;
			}

			CDatabaseX pDatabaseX = GetDatabaseX (nDBIndex);
			if (null == pDatabaseX || false == pDatabaseX.IsOpen)
				return false;

			CDBMaster pDBMaster = this.GetDBMaster ();
			TopDBManagerLibrary.DBMgrNode pQueryMgr = pDBMaster.GetAtSQLManager ((int)SQLQUERY_TYPE._USERSQL_, this.SQLIndex);
			if (null == pQueryMgr)
				return false;


			CMapStringToString mapTableToAlias = pQueryMgr.TableAliasMap;

			string sWhereAll = pQueryMgr.GetRealWhere (pDBMaster);
			string sRealTable = pQueryMgr.GetRealTable (pDBMaster);
			string sRealGroupBy = pQueryMgr.GetRealGroupBy ();

			CStringArray saDBIndexToKey = pDBMaster.GetInformation ().GetDBIndexToKeyArray ();

			if (null != pDatabaseX)
			{
				if (false != DBLib.IsCommonSQL (sWhereAll)) // 공통 문법 대응..
					sWhereAll = DBLib.GetDBMSQuery (sWhereAll, pDatabaseX, saDBIndexToKey);

				if (false != DBLib.IsCommonSQL (sRealTable)) // 공통 문법 대응..
					sRealTable = DBLib.GetDBMSQuery (sRealTable, pDatabaseX, saDBIndexToKey);

				if (false != DBLib.IsCommonSQL (sRealGroupBy)) // 공통 문법 대응..
					sRealGroupBy = DBLib.GetDBMSQuery (sRealGroupBy, pDatabaseX, saDBIndexToKey);
			}

			CDBCoreX pBlockSearch = new CDBCoreX (pDatabaseX);
			string strSQL = pQueryMgr.ToQuery (pDBMaster, -1, false, false);

			strSQL = PQAppBase.TableAliasing (strSQL, mapTableToAlias);

			if (false == pBlockSearch.OpenX (strSQL, 60))
				return false;

			//조건, 정렬 복구 (헤더 정렬 후 원본 조건을 되살리기 위해 Replace 했던 조건들은 초기화 해 줌)
			pQueryMgr.SetModifyPartSQL ((int)PART_TYPE._PART_WHERE_, true, "");
			pQueryMgr.SetModifyPartSQL ((int)PART_TYPE._PART_HAVING_, true, "");
			pQueryMgr.SetModifyPartSQL ((int)PART_TYPE._PART_ORDER_, true, "");

			int nTotalCnt = pBlockSearch.RecordCount;
			int nRowCnt = GetSearchBlockRowCount (nTotalCnt);

			ArrayList alRows = new ArrayList ();
			alRealDataRows.Clear ();

			GlobalWaitThread.WaitThread.Start ();

			// 데이터 먼저 검색할 수 있도록 논리 변경
			do
			{

				if (true == pBlockSearch.GetRowsSA (alRows, nRowCnt, false) || true == pBlockSearch.EOF)
				{
					//
					alRealDataRows.AddRange (alRows);
					SetImageDataRows (alRows);

					ArrayList alNewRows = BrowseNewRows (alRows);
					alDisplayDataRows.AddRange (alNewRows);
					alRows.Clear ();
					//

					ApplicationHelper.DoEvents ();
				}
			}
			while (false == pBlockSearch.EOF && false != pImageCarouselAttrib.AllLoad);
			//

			GlobalWaitThread.WaitThread.End ();

			this.m_pBlockSearch = pBlockSearch;

			return true;
		}

		/// <summary>
		/// 표현형식이 이미지표시 컬럼찾기
		/// </summary>
		/// <returns></returns>
		private int GetColumnDisplyImage ()
		{
			int nColumn = -1;

			foreach (BrowseItem pBrowseAtom in this.BrowseItemList)
			{
				string strDisplay = pBrowseAtom.GetDisplay (true);
				if (-1 < strDisplay.IndexOf ("$IMAGE:"))
				{
					nColumn = this.BrowseItemList.IndexOf (pBrowseAtom);
					break;
				}
			}

			return nColumn;
		}

		private void SetImageDataRows (ArrayList alRows)
		{
			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;

            alRowCount = alRows.Count;

            int nImageColumn = GetColumnDisplyImage ();
			if (-1 != nImageColumn)
			{
				for (int nRow = 0; nRow < alRows.Count; nRow++)
				{
					CStringArray psaField = alRows[nRow] as CStringArray;
					if (null != psaField && nImageColumn < psaField.Count)
					{
						object pValue = psaField[nImageColumn];
						if (null != pValue)
						{
							if (pValue.GetType () == typeof (byte[]))
							{
								byte[] byteValue = (byte[])pValue;
								pCarouselofAtom.AddValue (byteValue);
							}
							else if (pValue.GetType () == typeof (string))
							{
								string strValue = pValue.ToString ();

								if (-1 != strValue.IndexOf ("http:") || -1 != strValue.IndexOf ("HTTP:") ||
									-1 != strValue.IndexOf ("https:") || -1 != strValue.IndexOf ("HTTPS:"))
								{
									string strFileName = "";
									Stream pStream = HttpDownloadFile.HttpFileDownload (strValue, "", ref strFileName);
									if (null != pStream)
									{
										MemoryStream memoryStream = new MemoryStream ();
										pStream.CopyTo (memoryStream);

										byte[] byteValue = memoryStream.ToArray ();
										pCarouselofAtom.AddValue (byteValue);
									}
								}
								else
								{
									if (false == string.IsNullOrEmpty (strValue) && 260 > strValue.Length)
									{
										strValue = PQAppBase.KissGetFullPath (strValue);
										pCarouselofAtom.AddValue (strValue);
									}
									else if (true == StrLib.IsBase64String (strValue))
									{
										MemoryStream memoryStream = new MemoryStream (Convert.FromBase64String (strValue));
										byte[] byteValue = memoryStream.ToArray ();
										pCarouselofAtom.AddValue (byteValue);
									}
								}
							}
							else
							{
								pCarouselofAtom.AddValue ("");
							}
						}
						else
						{
							pCarouselofAtom.AddValue ("");
						}
					}
				}
			}
		}

		private void SetImageFileRows (List<string> listFile)
		{
			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;

			foreach (string strFilePath in listFile)
			{
				if (-1 != strFilePath.IndexOf ("http:") || -1 != strFilePath.IndexOf ("HTTP:") ||
					-1 != strFilePath.IndexOf ("https:") || -1 != strFilePath.IndexOf ("HTTPS:"))
				{
					string strFileName = "";
					Stream pStream = HttpDownloadFile.HttpFileDownload (strFilePath, "", ref strFileName);
					if (null != pStream)
					{
						MemoryStream memoryStream = new MemoryStream ();
						pStream.CopyTo (memoryStream);

						byte[] byteValue = memoryStream.ToArray ();
						pCarouselofAtom.AddValue (byteValue);
					}
				}
				else
				{
					pCarouselofAtom.AddValue (strFilePath);
				}
			}
		}


		public override bool ScrollHasReachedTheEnd ()
		{
			CarouselImageAttrib pImageCarouselAttrib = this.GetAttrib () as CarouselImageAttrib;

			bool bNextRow = false;
			if (null == m_pBlockSearch)
				return false;

			if (false != this.m_pBlockSearch.EOF)
				return false;

			if (false != pImageCarouselAttrib.AllLoad)
				return false;

			ArrayList alRows = new ArrayList ();

			GlobalWaitThread.WaitThread.Start ();

			if (true == this.m_pBlockSearch.GetRowsSA (alRows, 10, false) || true == this.m_pBlockSearch.EOF)
			{
				if (0 < alRows.Count)
				{
					if (m_pBlockSearch.RecordCount == m_pBlockSearch.CursorPos)
					{
						bNextRow = false;
					}
					else
					{
						bNextRow = true;
					}

					SetImageDataRows (alRows);
				}
				else
				{
					bNextRow = false;
				}
			}

			GlobalWaitThread.WaitThread.End ();

			return bNextRow;
		}

		public CStringArray GetSelectedLoadFieldRowValue ()
		{
			CStringArray strLoadeFieldRowValue = new CStringArray ();

			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;
			int nSelectRow = pCarouselofAtom.GetSelectRow ();

			if (-1 < nSelectRow && nSelectRow < this.RealDataRows.Count)
			{
				CStringArray dataRow = this.RealDataRows[nSelectRow] as CStringArray;

				int nColumn = 0;
				foreach (BrowseItem browseAtom in this.BrowseItemList)
				{
					if (true == browseAtom.IsLoadField)
					{
						strLoadeFieldRowValue.Add (dataRow.GetAt (nColumn));
					}

					nColumn++;
				}
			}

			return strLoadeFieldRowValue;
		}

		public override CStringArray GetSelectedRowData ()
		{
			CStringArray dataRow = null;

			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;
			int nSelectRow = pCarouselofAtom.GetSelectRow ();

			if (-1 < nSelectRow && nSelectRow < this.RealDataRows.Count)
			{
				dataRow = this.RealDataRows[nSelectRow] as CStringArray;
			}

			return dataRow;
		}

		#region 아톱의 스크립트 속성 관련

		protected override bool InitScriptEventMaps ()
		{
			m_pScriptEventMaps = new CMapWordToStringX ();

			m_pScriptEventMaps.SetAt (EVS_TYPE.EVS_A_SEL_CHANGE, LC.GS ("TopAtom_InputAtomCore_9"));    // _선택변경
			m_pScriptEventMaps.SetAt (EVS_TYPE.EVS_A_CLICK, (string.Empty));        // _누름

			return true;
		}

		/// <summary>
		/// 사각형아톰의 스크립트 속성 리스트를 반환 합니다.
		/// </summary>
		/// <returns></returns>
		public override ICollection GetScriptMapEntrys ()
		{
			ArrayList ScriptList = new ArrayList ();

			ScriptList.Add (new CScriptInfo (203, 0, true, Script_Leveling.ServerClient, 1, true)); // 값			#0

			ScriptList.Add (new CScriptInfo (286, 0, true, Script_Leveling.ClientOnly, 3, true));   // 화면감춤		#1
			ScriptList.Add (new CScriptInfo (483, 0, true, 43, false));                             // 선택번호		#2

			ScriptList.Add (new CScriptInfo (372, 0, false, Script_Leveling.ClientOnly, 2, true));  // 검색시작		#3
			ScriptList.Add (new CScriptInfo (273, 0, false, 44, false));                            // 모두지우기	#4
			ScriptList.Add (new CScriptInfo (218, 0, false, Script_Leveling.ClientOnly, 64, false));// 모두삭제		#5

			ScriptList.Add (new CScriptInfo (306, 2, false, 8, false));                             // 이동			#6
			ScriptList.Add (new CScriptInfo (345, 2, false, 8, false));                             // 다시보기		#7


			return ScriptList;
		}

		public override int SetProperty (int wNameID, CVariantX[] pvaArgs, CVariantX pRetVal)
		{
			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;

			switch (wNameID)
			{
				case 0:
				case 203: return set_ImageCarouselProperty (203, pvaArgs, pRetVal);
				case 1:
				case 286: return set_GdiProperty (286, pvaArgs, pRetVal);
				case 2:
				case 483: return set_ImageCarouselProperty (483, pvaArgs, pRetVal);

				default:

					return StdCore.E_NOT_DEF_PROPERTY;
			}
		}

		public int set_ImageCarouselProperty (int wNameID, CVariantX[] pvaArgs, CVariantX pValue)
		{
			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;

			switch (wNameID)
			{
				case 203:
					{
						bool bDownLoadURL = false;

						if (null != pvaArgs && 1 < pvaArgs.Length)
						{
							switch (pvaArgs[1].ToInt ())
							{
								case 1084:  //서버
									bDownLoadURL = true;
									break;
							}
						}

						if ((int)_vtValue._vtArray == pValue.m_vt) // 배열로 들어오는 경우..
						{
							List<string> listFilePath = new List<string> ();

							CVarArrayX _pVarArrayX = pValue.m_pvaArray;
							foreach (CVariantX pvaData in _pVarArrayX)
							{
								string strFilePath = pvaData.ToStringX ();
								if (false == bDownLoadURL)
								{
									strFilePath = PQAppBase.KissGetFullPath (strFilePath);
								}
								else
								{
									strFilePath = PQAppBase.KissGetSmartServerDownloadPath (strFilePath);
								}

								listFilePath.Add (strFilePath);
							}

							SetImageFileRows (listFilePath);
						}

						ViewImageData ();
					}
					break;
				case 483:
					{
						int nSelID = pValue.ToInt ();
						nSelID = this.GetRealIndex (nSelID);

						pCarouselofAtom.SelectRow (nSelID);
					}
					break;

				default:

					return StdCore.E_NOT_DEF_PROPERTY;
			}

			return StdCore.S_OK;
		}

		public override int Action (int wPropID, CVariantX[] pvaArgs, CVariantX pRetVal)
		{
			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;

			switch (wPropID)
			{
				case 3:
				case 372:
					{
						pCarouselofAtom.ClearImages ();
						LoadImageData ();
						return StdCore.S_OK;
					}
				case 4:
				case 273:
				case 5:
				case 218:
					{
						pCarouselofAtom.ClearImages ();
						return StdCore.S_OK;
					}
				case 6:
				case 306: return raw_MoveAction (306, pvaArgs, pRetVal);
				case 7:
				case 345: return raw_LandAction (345, pvaArgs, pRetVal);
			}

			return GetProperty (wPropID, pvaArgs, pRetVal);
		}

		public override int GetProperty (int wNameID, CVariantX[] pvaArgs, CVariantX pRetVal)
		{
			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;

			switch (wNameID)
			{
				case 0:
				case 203: return get_GdiProperty (203, pvaArgs, pRetVal);
				case 1:
				case 286: return get_GdiProperty (286, pvaArgs, pRetVal);
				case 2:
				case 483:
					{
						int nSelID = pCarouselofAtom.GetSelectRow ();
						nSelID = this.GetRealReturnIndex (nSelID);
						pRetVal.SetValue (nSelID);
					}
					break;

				default: return StdCore.E_NOT_DEF_PROPERTY;
			}

			return StdCore.S_OK;
		}

		public int raw_MoveAction (int wNameID, CVariantX[] pvaArgs, CVariantX pRetVal)
		{
			CarouselImageofAtom pCarouselofAtom = this.GetOfAtom () as CarouselImageofAtom;

			switch (wNameID)
			{
				case 306: //이동
					{
						// 파라미터 처리..
						if (null != pvaArgs && 1 < pvaArgs.Length)
						{
							switch (pvaArgs[1].ToInt ())
							{
								case 1154: // #이전
									pCarouselofAtom.MovePrev ();
									break;
								case 1155: // #다음
									pCarouselofAtom.MoveNext ();
									break;
							}
						}
					}
					break;
			}

			return StdCore.S_OK;
		}

		#endregion

	}
}
