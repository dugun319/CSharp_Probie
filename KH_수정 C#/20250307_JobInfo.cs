using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.DBCoreX.DBManager.POJO;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.Script;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.FormInformation;
using Softpower.SmartMaker.TopApp.Metadata.QuizMaker;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopAtom.Commands;
using Softpower.SmartMaker.TopAtom.Ebook;
using Softpower.SmartMaker.TopControl.Components.Container;
using Softpower.SmartMaker.TopControl.Components.Dialog;
using Softpower.SmartMaker.TopLight.Dynamic;
using Softpower.SmartMaker.TopLight.Edutech.LRS;
using Softpower.SmartMaker.TopLight.Models;
using Softpower.SmartMaker.TopLight.ProcessEventManager;
using Softpower.SmartMaker.TopLight.ViewModels;
using Softpower.SmartMaker.TopLight.Views;
using Softpower.SmartMaker.TopWebAtom;

namespace Softpower.SmartMaker.TopLight
{
	public class JobInfo : Information
	{
		//public const int CP_SET_USER_AND_DATE = 1000;

		protected LightDMTView m_pLightDMTView;

		public JobInfo ()
		{

		}

		public void ActiveWnd ()
		{
		}

		public override int DBIO (bool bLoad, int nSearchOper, Object pObj, Object lParam, bool IsClear = true)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			return pLightJDoc.DBIO (bLoad, nSearchOper, pObj as Atom, lParam as Atom, IsClear);
		}

		public void Set2stLoad (bool bScnd)
		{
		}

		public override List<Atom> GetAllAtomCoresTabOrdered ()
		{
			return this.m_pLightDMTView.GetAllAtomCoresTabOrdered ();
		}

		public override List<Atom> GetAllAtomCores ()
		{
			return this.m_pLightDMTView.GetAllAtomCores ();
		}

		public override List<Atom> GetBizLogicAllAtomCores ()
		{
			return this.m_pLightDMTView.GetBizLogicAllAtomCores ();
		}

		public override List<Atom> GetViewAtomCores ()
		{
			return this.m_pLightDMTView.GetViewAtomCores ();
		}

		public override List<AtomBase> GetAllChildren ()
		{
			return this.m_pLightDMTView.GetAllChildren ();
		}

		public override List<AtomBase> GetViewChildren ()
		{
			return this.m_pLightDMTView.GetViewChildren ();
		}

		public override CMultiList GetAllAtomCoresForSerialize ()
		{
			return m_pLightDMTView.GetMultiList ();
		}

		/// <summary>
		/// 80 자신이 속한 현재 뷰(메인뷰/ 텝뷰 페이지) 의 아톰을 리턴한다
		/// </summary>
		/// <param name="baseAtmCore"></param>
		/// <returns></returns>
		public override List<Atom> GetAtomCoreList (Atom baseAtomCore)
		{
			return this.GetAllAtomCoresTabOrdered ();

			//AtomBase ofAtom = baseAtomCore.GetOfAtom() as AtomBase;
			//if (null != ofAtom.GetTabViewAtom())
			//{
			//    FrameworkElement TabViewElement = ofAtom.GetTabViewAtom() as FrameworkElement;
			//    TabViewofAtom TabViewAtom = TabViewElement as TabViewofAtom;

			//    //80 현재 텝페이지 아톰 리턴
			//    return TabViewAtom.GetAtomCoresInCurrentTabPage();

			//    //80탭뷰의 전체 리턴
			//    //return TabViewAtom.GetAllAtomCores();
			//}
			//else
			//{
			//    return this.GetViewAtomCores();
			//}
		}
		public override object GetUserKey ()
		{
			object pUserKey = null;
			return pUserKey;
		}

		public override CDatabaseX GetDatabaseX (int nDBIndex)
		{
			if (null != m_pLightDMTView)
			{
				LightJDoc pLightJDoc = m_pLightDMTView.Document;
				if (null != pLightJDoc)
				{
					// 2015-03-09-M01 : [KWON] 업무규칙(서버접속)을 이용한 서버변경시 DB Index기준으로 전환.
					string strModuleKey = string.Empty;
					if (PQAppBase.UseModifyServer && nDBIndex > 0)
					{
						return PQAppBase.KissGetDatabaseX (nDBIndex);
					}
					else
					{
						if (0 < pLightJDoc.StoreMode)
						{
							strModuleKey = StoreDefine.StoreKey;
						}
						else
						{
							strModuleKey = pLightJDoc.GetModuleKeyFromDBIndex (nDBIndex);
						}
					}

					if (true == _Kiss._isempty (strModuleKey))
						return null;

					return PQAppBase.KissGetDatabaseX (strModuleKey);
				}
			}

			return null;
		}

		public override string GetModuleKeyFromDBIndex (int nDBIndex)
		{
			if (null != m_pLightDMTView)
			{
				LightJDoc pLightJDoc = m_pLightDMTView.Document;
				if (null != pLightJDoc)
				{
					return pLightJDoc.GetModuleKeyFromDBIndex (nDBIndex);
				}
			}

			return "";
		}

		public override string GetLoginInfo (int nDBIndex, USER_INFO dsInfo)
		{
			if (null != m_pLightDMTView)
			{
				LightJDoc pLightJDoc = m_pLightDMTView.Document;
				if (null != pLightJDoc)
				{
					string strModuleKey = pLightJDoc.GetModuleKeyFromDBIndex (nDBIndex);
					if (true == _Kiss._isempty (strModuleKey)) return "";

					return PQAppBase.KissGetLoginInfo (strModuleKey, dsInfo);
				}
			}

			return "";

		}

		public override CStringArray GetDBIndexToKeyArray ()
		{
			if (null != m_pLightDMTView)
			{
				LightJDoc pLightJDoc = m_pLightDMTView.Document;
				if (null != pLightJDoc)
				{
					return pLightJDoc.GetDBIndexToKeyArray ();
				}
			}

			return null;

		}


		public override CObArray GetHelperList (bool bAllView)
		{
			CObArray ppltHelper = null;

			//80
			//if (null != m_pLightDMTView)
			//{
			//    if (false == bAllView)
			//    {
			//        ppltHelper = m_pLightDMTView.GetHelperList();
			//    }
			//    else
			//    {
			//        LightJDoc pDoc = m_pLightDMTView.GetDocument();
			//        if (null != pDoc)
			//        {
			//            ppltHelper = pDoc.GetHelperList(null);
			//        }
			//    }
			//}

			return ppltHelper;
		}

		public override string GetOwnerFormName ()
		{
			string strFormName = "";
			if (null == m_pLightDMTView)
				return strFormName;

			LightJDoc pDoc = m_pLightDMTView.Document;
			if (null != pDoc)
				strFormName = pDoc.GetFormName ();

			return strFormName;
		}

		public override string GetOwnerFormTitle ()
		{
			string strFormName = "";
			if (null == m_pLightDMTView)
				return strFormName;

			LightJDoc pDoc = m_pLightDMTView.Document;

			if (null != pDoc)
				strFormName = pDoc.GetFormTitle ();

			return strFormName;
		}



		/// <summary>
		/// information 멤버변수인  m_pLightDMTView 반환 한다 
		/// </summary>
		/// <returns></returns>
		public override object GetOwnerWnd ()
		{

			return m_pLightDMTView;
		}


		public override void CallParentKeyDown (int nChar)
		{
			//80
			//m_pLightDMTView.OnKeyDown(nChar);
		}



		public override void ReceiveHelpstring (string lpszHelpstring)
		{
			//80
			//if (null != m_pLightDMTView)
			//{
			//    LightJDoc pDoc = m_pLightDMTView.GetDocument();
			//    if (null != pDoc)
			//    {
			//        CustomStatusBar pStatusBar = pDoc.CustStatusBar;
			//        if (null != pStatusBar)
			//        {
			//            pStatusBar.HelpString = lpszHelpstring;
			//        }
			//    }
			//}
		}

		//2006 BOS최적화 이우성 공통입력란 처리
		//모든 입력란을 숨겨준다.

		/*
         * 2008.01.10 황성민
         * 스크롤에서 공통입력란이 다중으로 동작하는 오류가 있어서
         * 다시 주석을 풀었음
         */
		public override void HideAllCommonEdit ()
		{
			//80
			//if (null != m_pLightDMTView)
			//    m_pLightDMTView.HideAllCommonEdit();
		}


		//입력란의 종류에 따라 그에 맞는 입력란을 반환해 준다.
		//80
		//public override void GetEdit(ref Control ppWnd, COMMONEDIT_TYPE editKind)
		//{
		//    //2006 BOS최적화 이우성 공통입력란 처리
		//    if (null != m_pLightDMTView)
		//        ppWnd = m_pLightDMTView.GetEdit(editKind);
		//    //
		//}
		public override void SendString (string lpszSentString) { ReceiveHelpstring (lpszSentString); }

		public override void SwitchScreenAllView ()
		{
		}

		//80
		//public override void InvalidRect(Rectangle rc)
		//{
		//    if (null != m_pLightDMTView)
		//    {
		//        m_pLightDMTView.Invalidate(rc);
		//    }
		//}

		public override void SaveDocument ()
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			pLightJDoc.OnSaveDocument (pLightJDoc.FilePath);
		}

		public override void FillDocInfo (DocInfo pDocInfo)
		{
			if (null != m_pLightDMTView)
			{
				m_pLightDMTView.FillDocInfo (pDocInfo);
			}
		}

		public override void NotifyClose ()
		{
		}

		public override void SetDocument (DocInfo pDocInfo, CLongBinary plongData, bool bDB /*= true*/)
		{
		}

		public override void DeleteDocument (DocInfo pDocInfo, bool bDB /*= true*/)
		{
		}

		public void GetDocInfo (DocInfo ppDocInfo)
		{
		}

		public int GetDocLastSeri (CDatabaseX pDatabaseX, string strTable, string strDocNum)
		{
			return -1;
		}

		public bool SetDocInfor (CDatabaseX pDatabaseX, DocInfo pDocInfo1, bool bDeleteDoc/*= false*/)
		{
			return false;
		}

		public override string GetRegistryPath ()
		{
			return m_pLightDMTView.Document.RegistryPath;
		}

		public virtual LightDMTView GetOwnerView ()
		{
			return m_pLightDMTView;
		}


		public override CMultiList GetCategoryList (bool bAllView)
		{
			CMultiList ppCategoryList = null;

			if (null != m_pLightDMTView)
			{
				if (false == bAllView)
				{
					ppCategoryList = m_pLightDMTView.GetCategoryList ();
				}
				else
				{
					LightJDoc pDoc = m_pLightDMTView.Document;
					if (null != pDoc)
						ppCategoryList = pDoc.GetCategoryList (null);
				}
			}

			return ppCategoryList;
		}

		public override CMultiList GetOperateList (bool bAllView)
		{
			CMultiList ppOperateList = null;

			if (null != m_pLightDMTView)
			{
				if (false == bAllView)
				{
					ppOperateList = m_pLightDMTView.GetOperateList ();
				}
				else
				{
					LightJDoc pDoc = m_pLightDMTView.Document;
					if (null != pDoc)
						ppOperateList = pDoc.GetOperateList (null);
				}
			}

			return ppOperateList;
		}

		public override CMapStringToOb GetProperVarMap (bool bAllView)
		{
			CMapStringToOb ppPropervarMap = null;

			if (null != m_pLightDMTView)
			{
				if (false == bAllView)
				{
					ppPropervarMap = m_pLightDMTView.GetProperVarMap ();
				}
				else
				{
					LightJDoc pDoc = m_pLightDMTView.Document;
					if (null != pDoc)
						ppPropervarMap = pDoc.GetProperVarMap (null);
				}
			}

			return ppPropervarMap;
		}

		//        public override CMapStringToOb GetLoadFieldMap(bool bAllView)
		//        {
		//            CMapStringToOb pLoadFieldMap = null;

		//            if (null != m_pLightDMTView)
		//            {
		//                if (false == bAllView)
		//                {
		//                    pLoadFieldMap = m_pLightDMTView.GetLoadFieldMap();
		//                }
		//                else
		//                {
		//                    LightJDoc pDoc = m_pLightDMTView.GetDocument();
		//                    if (null != pDoc)
		//                        pLoadFieldMap = pDoc.GetLoadFieldMap(null);
		//                }
		//            }

		//            return pLoadFieldMap;
		//        }

		//        public override void ExtendView(int nHeight)
		//        {
		//        }

		//        public bool GetAppInfo(CInfomation ppInfo)
		//        {
		//            return true;
		//        }

		//        public bool GetGDITable(Object ppGDIManager)
		//        {
		//            LightJDoc pDoc = m_pLightDMTView.GetDocument();
		//            if (null != pDoc)
		//                return pDoc.GetGDITable(ppGDIManager);

		//            ppGDIManager = null;
		//            return false;
		//        }

		//        public override bool SetThis(Object pObj) // script
		//        {
		//            return false;
		//        }

		//        public override Color GetViewColor()
		//        {
		//            return m_pLightDMTView.BackColor;
		//        }

		//        public bool CalculateAll()
		//        {
		//            return false;
		//        }

		//        public override int GetExtendLevel(string strKey)
		//        {
		//            int nJobLevel = 7;
		//            nJobLevel = PQAppBase.KissGetExtendLevel(strKey);
		//            return nJobLevel;
		//        }

		public override string GetStringAccess ()
		{
			LightJDoc pDoc = m_pLightDMTView.Document as LightJDoc;
			CKnowledgeBank pKnowledgeBank = pDoc.GetKnowledgeBank ();
			if (null != pKnowledgeBank)
			{
				string strRegistryFlag = pKnowledgeBank.RegistryFlag;
				if (0 < strRegistryFlag.Length)
				{
					char[] strClass = strRegistryFlag.ToCharArray ();

					string strData = new String (strClass[0], 1);
					return strData;
				}
			}

			return "";
		}

		//        public override string GetLightJDocDate()
		//        {
		//            return "";
		//        }

		//        public override string GetAppDate()
		//        {
		//            return "";
		//        }

		//        public bool DeleteTempDocument(CDocInfo pDocInfo)
		//        {
		//            return false;
		//        }

		//        public void SetTempDocument(CDocInfo pDocInfo, CLongBinary plongData)
		//        {
		//        }

		//        public override void SetAtomList(CAtom pAtom, bool bAddList /*= true*/)
		//        {
		//            if (null == m_pLightDMTView || null == pAtom)
		//                return;

		//            CMultiList pAtomList = m_pLightDMTView.GetAtomList(-1);
		//            if (null != pAtomList)
		//            {
		//                if (false != bAddList)
		//                {
		//                    pAtomList.AddTail(pAtom);
		//                }
		//                else
		//                {
		//                    int nPos = pAtomList.Find(pAtom);
		//                    if (-1 != nPos)
		//                    {
		//                        CAtom pFindedAtom = (CAtom)(pAtomList.GetAt(nPos));
		//                        if (pAtom != pFindedAtom)
		//                            return;

		//                        pAtomList.RemoveAt(nPos);
		//                    }
		//                }
		//            }
		//        }

		//        public override bool IsModify(int dwVersion)
		//        {
		//            LightJDoc pDoc = (LightJDoc)(m_pLightDMTView.GetDocument());
		//            if (null != pDoc)
		//                return pDoc.IsModify(dwVersion);
		//            return true;
		//        }

		//        public override bool IsAppendForm()
		//        {
		//            LightJDoc pDoc = (LightJDoc)(m_pLightDMTView.GetDocument());
		//            if (null != pDoc)
		//                return pDoc.IsAppendForm();
		//            return true;
		//        }

		//        //public override bool IsTopERP () // script
		//        //{
		//        //    return PQAppBase.IsTopERP;
		//        //    //return false;
		//        //}

		//        public override bool IsOnTabView()
		//        {
		//            if (null != m_pLightDMTView)
		//                return m_pLightDMTView.IsTabView();
		//            return false;
		//        }
		//        public override CMultiList GetTabViewAtomList()
		//        {
		//            if (null != m_pLightDMTView)
		//                return m_pLightDMTView.GetTabViewAtomList();
		//            return null;
		//        }
		//        public new UInt16 GetScriptIndex()
		//        {
		//            // Script
		//            //#ifndef __RUNMODE__
		//            LightJDoc pDoc = null;
		//            if (null == m_pLightDMTView || null == (pDoc = m_pLightDMTView.GetDocument()))
		//                return 0;
		//            return pDoc.GetMaxScriptIndex();
		//            //return 0;
		//        }

		public override bool GetCheckAtomList (ref CObList ppolAtomList)
		{
			LightJDoc pDoc = null;
			if (null == m_pLightDMTView || null == (pDoc = m_pLightDMTView.Document))
			{
				return false;
			}

			//80 추후작업 
			// 1. 모든 뷰 저장인지를 알아온다..
			//if (false != pDoc.GetAllSave())
			//{
			//    ppolAtomList = pDoc.GetOrderAtom();
			//}
			//else //2006.10.25, 이우성, 모든뷰 저장이 아닌경우 선택된 탭뷰의 아톰을 가져온다.
			//{
			//    int nSelectTab = -1;
			//    CTabView pTabView = m_pLightDMTView.GetTabViewAtom("");
			//    if (null != pTabView)
			//        nSelectTab = pTabView.GetSelectTab();

			//    foreach (CLightDMTView pView in pDoc.GetViewList())
			//    {
			//        if (nSelectTab == pView.GetTabNo())
			//        {
			//            ppolAtomList = pView.GetMultiList();
			//            break;
			//        }

			//    }
			//}

			ppolAtomList = pDoc.GetOrderAtom ();
			return true;
		}

		//        public override bool GetAutoHanChange()
		//        {
		//            return false;
		//        }

		//        public override string GetKorSource(string strIndex)
		//        {
		//            return "";
		//        }

		//        public override int GetScrLine()
		//        {
		//            return 0;
		//        }

		//        public override void SetKnowledgeInfo(bool bBackUp, string strVersionFlag, string strRegistryFlag)
		//        {
		//        }

		//        public override void ChangeSelectAtom(CAtom pAtom)
		//        {
		//        }

		// 기능 버튼의 추가, 수정, 화면지우기 기능등을 수행한다.
		public override bool OnButtonAction (int nActionType, Object pObject /*= null*/)
		{
			LightJDoc pDoc = null;

			if (null == m_pLightDMTView || null == (pDoc = (LightJDoc)(m_pLightDMTView.Document)))
			{
				return false;
			}

			int nRealAction = nActionType;

			if (0 > nActionType)
			{
				nRealAction = LightDef.CP_SET_USER_AND_DATE;
			}

			bool bResult = false;

			switch (nRealAction)
			{
				case (int)AC_TYPE.AC_CLEAR: // Clear Screen
					pDoc.ClearScreen (pObject as Atom, true, true, true);
					break;

				case (int)AC_TYPE.AC_SAVE_UPDATE: // Update
					{
						Atom atomCore = pObject as Atom;
						Atom paramAtomCore = null;

						if (null != atomCore)
						{
							string strRefAtomName = atomCore.GetRefProperVar ();
							if (false == string.IsNullOrEmpty (strRefAtomName))
							{
								paramAtomCore = atomCore.Find_Atom (typeof (TouchAtom), strRefAtomName, false, false);
							}
						}

						bResult = pDoc.OnModifySaveMode (nRealAction, pObject);
						pDoc.DBIO (false, SQL_RECORD_TYPE.UPDATE_RECORD, atomCore, paramAtomCore);
					}
					break;

				//80 추후작업 
				//case (int)AC_TYPE.AC_SAVE_INSERT: // Insert
				//case (int)AC_TYPE.AC_SAVE_UPDATE: // Update
				//    bResult = pDoc.OnModifySaveMode(nRealAction, pObject);
				//    break;

				//case (int)AC_TYPE.AC_CANCEL: // Cancel
				//    {
				//        bool bRollback = false;
				//        CAction pAction = (CAction)(pObject);
				//        CActionAttrib pActionAtt = null;
				//        if (null != pAction &&
				//            null != (pActionAtt = ((CActionAttrib)pAction.Attrib)) &&
				//            false != _Kiss.IsKindOf(pActionAtt, typeof(CActionAttrib)))
				//        {
				//            bRollback = pActionAtt.IsActionFlag();
				//        }

				//        if (false != bRollback)
				//            bResult = pDoc.OnRollback();// rollback
				//        else
				//            bResult = pDoc.OnModifySaveMode(nRealAction, pObject);
				//    } break;

				//case LightDef.CP_SET_USER_AND_DATE:
				//    {
				//        int nSetKind = -1 * nActionType;
				//        bResult = pDoc.SetUserAndDate(((_Kiss.toInt32(AC_TYPE.AC_DELETE) == nSetKind) ? 2 : 1));
				//    } break;

				default:
					break;
			}

			return bResult;
		}

		/*2006.4.12, 공통입력란 작업하면서 변경. 그룹박스속 입력란을 찾을 수 있도록						
        public override void ProcessDate (Point point)
        {
            m_pLightDMTView.Process_Date (point);
        }
 
        public override void LButtonDownPlayMode(Point point)
        {
            m_pLightDMTView.LButtonDown_PlayMode(point);
        }
        */

		//        public override bool GetIsAllowNull()
		//        {
		//            LightJDoc pDoc = (LightJDoc)(m_pLightDMTView.GetDocument());
		//            CFrameAttrib pFrameAttrib = pDoc.GetFrameAttrib();
		//            if (null != pFrameAttrib)
		//                return pFrameAttrib.AllNull;

		//            return false;
		//        }

		//        public new int GetJobLevel()
		//        {
		//            return 1;
		//        }

		//        public new int GetDocLevel()
		//        {
		//            return 1;
		//        }

		//        public override string GetGroupCode()
		//        {
		//            return PQAppBase.KissGetFirstDepLoginInfo(USER_INFO._GrpCode);
		//        }

		//        public new string GetKeyString()
		//        {
		//            return "";
		//        }

		//        public override string GetERPInfo(string lpszKind)
		//        {
		//            return "";
		//        }

		//        public override void SetERPInfo(string lpszKind, string strData)
		//        {
		//        }

		public override TopDoc CallReference (Atom pAtom, CObArray poaAtomData, string sModelName, string sExecName, Object pDocInfo)
		{
			return CallReference (pAtom, poaAtomData, sModelName, sExecName, pDocInfo, 1);
		}

		public override TopDoc CallReference (Atom pAtom, CObArray poaAtomData, string sModelName, string sExecName, Object pDocInfo, int nBookPage)
		{
			return CallReference (pAtom, poaAtomData, sModelName, sExecName, pDocInfo, nBookPage, false);
		}

		public override TopDoc CallReference (Atom pAtom, CObArray poaAtomData, string sModelName, string sExecName, Object pDocInfo, int nBookPage, bool bProperBtnExec)
		{
			LightJDoc pParentDOC = null;
			if (null == m_pLightDMTView || null == (pParentDOC = m_pLightDMTView.Document))
				return null;

			//2006.10.24, 이우성, 생성조건을 넣어주기 위해 파싱한다.
			string strArgs = "";
			int nArgePos = sModelName.LastIndexOf ("/");
			if (-1 < nArgePos)
			{
				strArgs = sModelName.Substring (nArgePos + 1);
				sModelName = sModelName.Substring (0, nArgePos);
			}
			//

			//////////////////////////////////////////////////////////////////////////
			// MDL경로 변경
			if (0 <= sModelName.IndexOf (".MDL,"))
			{
				sModelName = PQAppBase.KissGetFullPath (sModelName);

				int nStartIndex = sModelName.IndexOf (".MDL,");
				int nLastIndex = sModelName.LastIndexOf ("\\");
				int nLength = nLastIndex - nStartIndex;

				string str = "";
				if (nLastIndex < sModelName.Length)
					str = sModelName.Substring (nStartIndex, nLength);
				else
					str = ".MDL,,";

				sModelName = sModelName.Replace (str, "");
				sModelName = String.Concat (sModelName, ".QPG");
				sModelName = sModelName.Replace ("|0", "");
			}
			else if (0 > sModelName.IndexOf ("DB:\\"))
			{
				sModelName = PQAppBase.KissGetFullPath (sModelName);
			}

			sModelName = sModelName.Replace (".FRM", ".QPG");
			//////////////////////////////////////////////////////////////////////////

			// 2004/09/10 (Chang ki ju) - Create Document from Database File
			if (0 > sModelName.IndexOf ("http:") && 0 > sModelName.IndexOf ("DB:\\"))
			{
				/*
                System.IO.FileInfo info = new System.IO.FileInfo(sModelName);
                if (false == info.Exists)
                {
                    string strMessage = String.Concat(sModelName, LC.GS("TopLight_JobInfo_1"));
                    _Message80.Show(strMessage);
                    return null;
                }
                 * */
			}
			/////////////////////////////////////////////////////////////////////

			// 우선실행인지 여부를 결정한다..
			bool bDoModal = false;
			int nPos = sExecName.IndexOf ("&");
			if (-1 < nPos)
			{
				string strDoModal = sExecName.Substring (nPos + 1);

				if (0 == String.Compare ("DOMODAL", strDoModal, true))
					bDoModal = true;
			}

			LightJDoc pChildDOC = null;
			//새로운 모델을 만든다
			pChildDOC = m_pLightDMTView.MakeModelWhenDetail (sModelName, nBookPage, bDoModal);

			if (null == pChildDOC) return null;

			LightDMTView lightDMTView = pChildDOC.GetParentView () as LightDMTView;
			LightDMTFrame lightDMTFrame = lightDMTView.GetFrame ();

			string strTitle = System.IO.Path.GetFileNameWithoutExtension (sModelName);
			lightDMTFrame.ChangeCaptionText (strTitle);

			// 2004.03.29 khj 웹에서 외부모델연결로 상세폼 띄울 때 오류때문에 수정
			string strDomain = AppDomain.CurrentDomain.ToString ();
			strDomain = strDomain.ToUpper ();
			if (-1 < strDomain.IndexOf (".EXE"))        // Web 일때만
			{
				//80 mdi 구조 추후 작업 
				//
				// <2004. 09. 18. cys>
				/* 작업자: 김영택 2007.09.21
				 * 작업이유: BOS 개선 때문에 BOS 프레임과 모델간의 Owner관계가 없어지면서
				 *           상세폼을 띄울 때 띄우는 폼이 뜨는폼 위로 올라가는 현상이 발생하여
				 *           작업진행.
				 * 작업내용: BOS 프레임 대신에 띄우는 폼과 뜨는 폼 사이에 Owner관계를 주어
				 *           띄우는 폼이 항상 아래로 가게 한다. 
				 * 추가작업:김영택 2007.10.4
				 * 작업내용: 개선 BOS와 예전 BOS 병행하여 사용을 위해 처리. (백그라운드 처리 유무)
				 *--------------------------------------------------------------------------*/
				//Form form = pParentDOC.GetThisForm();
				//if (null != form && false == bDoModal)
				//{
				//    if (null != form.MdiParent) pParentWnd = form.MdiParent;
				//    else if (null != form.Owner) pParentWnd = form.Owner;
				//    //pParentWnd = form;
				//}
				//
			}

			// NTOA
			if (0 <= sModelName.IndexOf (".QFR"))
			{
				if (null != pChildDOC)
				{
					pChildDOC.SetNtoaDocKind (1);
				}
			}
			else if (0 <= sModelName.IndexOf (".QWP"))
			{
				if (null != pChildDOC)
				{
					pChildDOC.SetNtoaDocKind (5);   //	0:QPG, 1:NTOA, 2:EDMS, 3:KMS, 5:Web
				}
			}

			else if (0 <= sModelName.IndexOf (".QPM"))
			{
				if (null != pChildDOC)
				{
					pChildDOC.SetNtoaDocKind (6);   //	0:QPG, 1:NTOA, 2:EDMS, 3:KMS, 5:Web, 6: QPM
				}
			}

			//2004.10.02 LHS (FileIO통합)
			//if (null == pChildDOC || false == pChildDOC.CreateForm((Control)(pParentWnd)))
			//    _Exception.ThrowEmptyException();

			//80 information 넘김
			pChildDOC.SetGlobalPtr (pParentDOC.GetGlobalPtr ());

			//80 필요없음 
			//pChildDOC.FilePath = sModelName;
			//if (false == pChildDOC.OnOpenDocument(sModelName))
			//    _Exception.ThrowEmptyException();


			//80 필요없음 
			//pChildDOC.SetNormalizeVirtualView();
			//pChildDOC.GetThisForm().Text = pChildDOC.GetFormName();//pChildDOC.GetFileName ();

			//80 custombar 추후작업 
			//pChildDOC.CustStatusBar = pParentDOC.CustStatusBar;

			//NTOA
			if (0 <= sModelName.IndexOf (".QFR"))
			{
				if (null != pChildDOC)
				{
					pChildDOC.SetNtoaPageInfo ();
				}
			}

			if (null != poaAtomData && 1 < poaAtomData.GetSize ())
			{
				CDBMaster pRefDBMaster = (CDBMaster)(poaAtomData.GetAt (0));
				if (null != pRefDBMaster && false != _Kiss.IsKindOf (pRefDBMaster, typeof (CDBMaster)))
				{
					Object pObject = (object)(poaAtomData.GetAt (1));
					Atom objTmp = null;
					CMultiList objMulTmp = null;

					if (null != pObject)
					{
						if (null != (objTmp = pObject as Atom))
						{
							ScrollAtom pScrollable = null;
							for (int i = 1; i < poaAtomData.GetSize (); i++)
							{
								Atom pFindAtom = (Atom)(poaAtomData.GetAt (i));
								if (null == pFindAtom)
									continue;

								pFindAtom.SetDBMaster (pRefDBMaster);
								pFindAtom.AddRef ();
								// <2002. 07. 15. cys>
								if (false != _Kiss.IsKindOf (pFindAtom, typeof (DateAtom)))
									((DateAtom)(pFindAtom)).SetAttribInfomation ();

								if (false != _Kiss.IsKindOf (pFindAtom, typeof (ScrollAtom)))
								{
									pScrollable = (ScrollAtom)(pFindAtom);
								}
								else
								{
									if (null != pScrollable)
									{
										//여기에 이 코드가 꼭 필요한 것인지?
										// 이코드가 있을 경우 팝업에 묶이고 스크롤에 묶인 입력란의 부가정보를 띄우고 난후에
										// SetAtomtoPatchOccur 로 인해 연결된 아톰이 바뀌기 때문에 팝업이 뜨지 않는 현상이 일어남. 2005.9.21 PTJ
										//pFindAtom.SetAtomtoPatchOccur (pScrollable);
										pScrollable = null;
									}
								}
							}
						}
						else if (null != (objMulTmp = pObject as CMultiList))
						{
							ScrollAtom pScrollable = null;
							for (int i = 1; i < poaAtomData.GetSize (); i++)
							{
								CMultiList pLoadAtomList = (CMultiList)(poaAtomData.GetAt (i));

								foreach (Atom pInerAtom in pLoadAtomList)
								{
									if (null == pInerAtom)
										continue;

									pInerAtom.SetDBMaster (pRefDBMaster);
									if (false != _Kiss.IsKindOf (pInerAtom, typeof (ScrollAtom)))
										pScrollable = (ScrollAtom)(pInerAtom);
									else
									{
										if (null != pScrollable)
										{
											//여기에 이 코드가 꼭 필요한 것인지?
											// 이코드가 있을 경우 팝업에 묶이고 스크롤에 묶인 입력란의 부가정보를 띄우고 난후에
											// SetAtomtoPatchOccur 로 인해 연결된 아톰이 바뀌기 때문에 팝업이 뜨지 않는 현상이 일어남.kkj
											//	pInerAtom.SetAtomtoPatchOccur (pScrollable);
											pScrollable = null;
										}
									}
								}

							}
						}
					}
				}
			}

			SetChildDataReceiveAtom (pAtom);

			pChildDOC.InitChildLoadFieldData (poaAtomData);
			pChildDOC.IsReferenceForm = true;
			pChildDOC.SetCreateCond (strArgs);

			// 우선실행권(모달)인 경우
			//if (false != bDoModal)
			//{
			//	//pChildDOC.ExecuteRunMode();
			//	//pChildDOC.LoadContent(poaAtomData, false);
			//	//pChildDOC.ExecuteScript(pParentDOC);

			//	// 70 로직이 맞음 . 위에 80 로직은 문제가 있음 
			//	pChildDOC.ExecuteRunMode (poaAtomData, ((false == bDoModal) ? true : false), pParentDOC);

			//	//80 추후작업 모달로 띄우는것 논의가 필요할듯 
			//	//pChildDOC.ExecuteRunMode(poaAtomData, ((false == bDoModal) ? true : false), pParentDOC);
			//	//Form pNewForm = pChildDOC.GetThisForm();
			//	//if (null != pNewForm)
			//	//{
			//	//    /*
			//	//     *  일반폼에 검색창이 있는경우 검색창 스크롤의 최대값을 구해오지만,
			//	//     *  모달창에 검색창이 있는경우에는 컨트롤이 활성화 되지 않아서 최대값을 구해오지 못함
			//	//     *  모달창을 먼저 띄우고 숨겨서 검색창의 스크롤 최대값을 구한후, 
			//	//     *  모달형식으로 띄어줌
			//	//     */
			//	//    pNewForm.Show();
			//	//    pNewForm.Hide();
			//	//    pChildDOC.ExecuteRunMode(poaAtomData, ((false == bDoModal) ? true : false), pParentDOC);
			//	//    pNewForm.ShowDialog();
			//	//}
			//	//pNewForm.Dispose();
			//	//80 end 추후작업 
			//}
			//else
			//{
			//	// 폼열림 이벤트 및 부모폼 설정, 파리미터 전달 문제 발생함, 원래 70 로직으로 구현해야함.
			//	//pChildDOC.ExecuteRunMode();
			//	//pChildDOC.LoadContent(poaAtomData, false);
			//	//80 런모드 변경시 PreExecute에서 실행 
			//	//pChildDOC.ExecuteScript(pParentDOC);

			//	// 70 로직이 맞음 . 위에 80 로직은 문제가 있음 
			//	pChildDOC.ExecuteRunMode (poaAtomData, ((false == bDoModal) ? true : false), pParentDOC);
			//}

			pChildDOC.ExecuteRunMode (poaAtomData, ((false == bDoModal) ? true : false), pParentDOC);

			//  2020-12-18 kys 상세폼보기에서 우선실행권으로 실행한경우
			if (true == bProperBtnExec)
			{
				//lightDMTFrame.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Render, new Action (delegate

				LightDMTFrame parentFrame = m_pLightDMTView.GetFrame ();
				if (null != parentFrame)
				{
					CFrameAttrib frameAttrib = pChildDOC.GetFrameAttrib ();
					var parentFrameAttrib = parentFrame.CurrentLightDMTView.Document.GetFrameAttrib () as CFrameAttrib;

					var parentPhoneScreenView = parentFrame.CurrentBaseScreenView as PhoneScreenView;
					var phoneScreenView = lightDMTFrame.CurrentBaseScreenView as PhoneScreenView;

					if (null != parentPhoneScreenView)
					{
						Grid shadowGrid = parentPhoneScreenView.GetSlidingShadowGrid ();
						if (false == _Kiss.toBool (frameAttrib.FrameAttrib & (uint)FRAME_ATT.Transparent))
						{
							//parentFrame.CurrentPhoneScreenView.SetSlidingShadowGrid (Visibility.Visible);   // 뒷배경 반투명 처리
						}

						Size parentFrameSize = parentFrameAttrib.GetDisplayFrameSize ();
						Size frameSize = frameAttrib.GetDisplayFrameSize (); ;


						lightDMTFrame.ClearFrame ();


						double dScale = parentPhoneScreenView.CurrentTopView.PercentOfScale * 0.01;
						double dExpectedScreenViewWidth = frameSize.Width * dScale;
						double dExpectedScreenViewHeight = frameSize.Height * dScale;

						double dVHRatio = pChildDOC.TargetPhoneSpecification.VerticalResolution / pChildDOC.TargetPhoneSpecification.HorizontalResolution;

						dExpectedScreenViewWidth = dExpectedScreenViewWidth + lightDMTFrame.NonScreenHorizontalWidth;
						dExpectedScreenViewHeight = Math.Min (dExpectedScreenViewHeight, dExpectedScreenViewWidth * dVHRatio) + lightDMTFrame.NonScreenVerticalHeight;

						lightDMTFrame.Width = dExpectedScreenViewWidth;
						lightDMTFrame.Height = dExpectedScreenViewHeight;

						lightDMTFrame.SetLayoutScaleTransform_PhonScreenView (dScale, dScale);

						Point ptOffset = new Point (parentFrame.Margin.Left + ((((parentFrameSize.Width + parentFrame.NonScreenHorizontalWidth) * dScale) - lightDMTFrame.Width) / 2),
							parentFrame.Margin.Top + ((((parentFrameSize.Height + parentFrame.NonScreenVerticalHeight) * dScale) - lightDMTFrame.Height) / 2));
						lightDMTFrame.Margin = new Thickness (0 < ptOffset.X ? ptOffset.X : 0, 0 < ptOffset.Y ? ptOffset.Y : 0, 0, 0);

						if (parentFrame.Parent is Canvas canvas)
						{
							foreach (object child in canvas.Children)
							{
								if (child is BaseFrame frame &&
									frame != lightDMTFrame)
								{
									frame.IsHitTestVisible = false;
								}
							}
						}

						lightDMTFrame.IsModalMode = true;
						//lightDMTFrame.SetWindowMode_Modal (parentFrame, frameAttrib.FrameAttrib, frameSize);
					}
				}//));
			}
			//

			return pChildDOC;
		}

		public override void CallReference (Atom pAtom, CObArray poaAtomData, TopDoc pTopDoc)
		{
			LightJDoc pParentDOC = m_pLightDMTView.Document;
			LightJDoc pChildDOC = pTopDoc as LightJDoc;

			pChildDOC.SetGlobalPtr (pParentDOC.GetGlobalPtr ());

			if (null != poaAtomData && 1 < poaAtomData.GetSize ())
			{
				CDBMaster pRefDBMaster = (CDBMaster)(poaAtomData.GetAt (0));
				if (null != pRefDBMaster && false != _Kiss.IsKindOf (pRefDBMaster, typeof (CDBMaster)))
				{
					Object pObject = (object)(poaAtomData.GetAt (1));
					Atom objTmp = null;
					CMultiList objMulTmp = null;

					if (null != pObject)
					{
						if (null != (objTmp = pObject as Atom))
						{
							ScrollAtom pScrollable = null;
							for (int i = 1; i < poaAtomData.GetSize (); i++)
							{
								Atom pFindAtom = (Atom)(poaAtomData.GetAt (i));
								if (null == pFindAtom)
									continue;

								pFindAtom.SetDBMaster (pRefDBMaster);
								pFindAtom.AddRef ();
								// <2002. 07. 15. cys>
								if (false != _Kiss.IsKindOf (pFindAtom, typeof (DateAtom)))
									((DateAtom)(pFindAtom)).SetAttribInfomation ();

								if (false != _Kiss.IsKindOf (pFindAtom, typeof (ScrollAtom)))
								{
									pScrollable = (ScrollAtom)(pFindAtom);
								}
								else
								{
									if (null != pScrollable)
									{
										//여기에 이 코드가 꼭 필요한 것인지?
										// 이코드가 있을 경우 팝업에 묶이고 스크롤에 묶인 입력란의 부가정보를 띄우고 난후에
										// SetAtomtoPatchOccur 로 인해 연결된 아톰이 바뀌기 때문에 팝업이 뜨지 않는 현상이 일어남. 2005.9.21 PTJ
										//pFindAtom.SetAtomtoPatchOccur (pScrollable);
										pScrollable = null;
									}
								}
							}
						}
						else if (null != (objMulTmp = pObject as CMultiList))
						{
							ScrollAtom pScrollable = null;
							for (int i = 1; i < poaAtomData.GetSize (); i++)
							{
								CMultiList pLoadAtomList = (CMultiList)(poaAtomData.GetAt (i));

								foreach (Atom pInerAtom in pLoadAtomList)
								{
									if (null == pInerAtom)
										continue;

									pInerAtom.SetDBMaster (pRefDBMaster);
									if (false != _Kiss.IsKindOf (pInerAtom, typeof (ScrollAtom)))
										pScrollable = (ScrollAtom)(pInerAtom);
									else
									{
										if (null != pScrollable)
										{
											//여기에 이 코드가 꼭 필요한 것인지?
											// 이코드가 있을 경우 팝업에 묶이고 스크롤에 묶인 입력란의 부가정보를 띄우고 난후에
											// SetAtomtoPatchOccur 로 인해 연결된 아톰이 바뀌기 때문에 팝업이 뜨지 않는 현상이 일어남.kkj
											//	pInerAtom.SetAtomtoPatchOccur (pScrollable);
											pScrollable = null;
										}
									}
								}

							}
						}
					}
				}
			}

			SetChildDataReceiveAtom (pAtom);

			pChildDOC.InitChildLoadFieldData (poaAtomData);
			pChildDOC.IsReferenceForm = true;

			// 70 로직이 맞음 . 위에 80 로직은 문제가 있음 
			pChildDOC.ExecuteRunMode (poaAtomData, true, pParentDOC);
		}

		//        /// <summary>
		//        /// 상세폼을 Embedding 시키기 위해
		//        /// </summary>
		//        /// <param name="pAtom"></param>
		//        /// <param name="poaAtomData"></param>
		//        /// <param name="sModelName"></param>
		//        /// <param name="pParent"></param>
		//        /// <param name="bAutoSize"></param>
		//        /// <returns></returns>
		//        /// 
		//        public override CTopDoc CallReferenceEmbedding(CAtom pAtom, CObArray poaAtomData, string sModelName, Control pParent, bool bAutoSize)
		//        {
		//            Rectangle rcBounds = Rectangle.Empty;

		//            LightJDoc pParentDOC = null;
		//            if (null == m_pLightDMTView || null == (pParentDOC = m_pLightDMTView.GetDocument()))
		//                return null;

		//            string strArgs = "";
		//            int nArgePos = sModelName.LastIndexOf(" /");
		//            if (-1 < nArgePos)
		//            {
		//                strArgs = sModelName.Substring(nArgePos + 2);
		//                sModelName = sModelName.Substring(0, nArgePos);
		//                sModelName = sModelName.TrimEnd();
		//            }

		//            sModelName = PQAppBase.KissGetFullPath(sModelName);

		//            LightJDoc pChildDOC = null;
		//            try
		//            {
		//                pChildDOC = (LightJDoc)(System.Activator.CreateInstance(typeof(LightJDoc)));

		//                if (0 <= sModelName.IndexOf(".QWP"))
		//                {
		//                    if (null != pParent && pParent is Panel)
		//                    {
		//                        // Embedded 시는 스크롤 처리 없음.
		//                    }
		//                    else
		//                    {
		//                        pChildDOC.SetNtoaDocKind(5);	//	0:QPG, 1:NTOA, 2:EDMS, 3:KMS, 5:Web
		//                    }
		//                }

		//                if (null == pChildDOC || false == pChildDOC.CreateForm(pParent))
		//                    _Exception.ThrowEmptyException();

		//                pChildDOC.SetGlobalPtr(pParentDOC.GetGlobalPtr());
		//                if (false == pChildDOC.OnOpenDocument(sModelName))
		//                    _Exception.ThrowEmptyException();

		//                pChildDOC.FilePath = sModelName;
		//                if (null != pChildDOC.GetThisForm())
		//                    pChildDOC.GetThisForm().Text = pChildDOC.GetFormName();
		//                pChildDOC.CustStatusBar = pParentDOC.CustStatusBar;

		//                if (null != poaAtomData && 1 < poaAtomData.GetSize())
		//                {
		//                    CDBMaster pRefDBMaster = (CDBMaster)(poaAtomData.GetAt(0));
		//                    if (null != pRefDBMaster && false != _Kiss.IsKindOf(pRefDBMaster, typeof(CDBMaster)))
		//                    {
		//                        Object pObject = (object)(poaAtomData.GetAt(1));
		//                        CAtom objTmp = null;
		//                        CMultiList objMulTmp = null;

		//                        if (null != pObject)
		//                        {
		//                            if (null != (objTmp = pObject as CAtom))
		//                            {
		//                                CScrollable pScrollable = null;
		//                                for (int i = 1; i < poaAtomData.GetSize(); i++)
		//                                {
		//                                    CAtom pFindAtom = (CAtom)(poaAtomData.GetAt(i));
		//                                    if (null == pFindAtom)
		//                                        continue;

		//                                    pFindAtom.SetDBMaster(pRefDBMaster);
		//                                    pFindAtom.AddRef();
		//                                    // <2002. 07. 15. cys>
		//                                    if (false != _Kiss.IsKindOf(pFindAtom, typeof(CDate)))
		//                                        ((CDate)(pFindAtom)).SetAttribInfomation();

		//                                    if (false != _Kiss.IsKindOf(pFindAtom, typeof(CScrollable)))
		//                                    {
		//                                        pScrollable = (CScrollable)(pFindAtom);
		//                                    }
		//                                    else
		//                                    {
		//                                        if (null != pScrollable)
		//                                        {
		//                                            //여기에 이 코드가 꼭 필요한 것인지?
		//                                            // 이코드가 있을 경우 팝업에 묶이고 스크롤에 묶인 입력란의 부가정보를 띄우고 난후에
		//                                            // SetAtomtoPatchOccur 로 인해 연결된 아톰이 바뀌기 때문에 팝업이 뜨지 않는 현상이 일어남. 2005.9.21 PTJ
		//                                            //pFindAtom.SetAtomtoPatchOccur (pScrollable);
		//                                            pScrollable = null;
		//                                        }
		//                                    }
		//                                }
		//                            }
		//                            else if (null != (objMulTmp = pObject as CMultiList))
		//                            {
		//                                CScrollable pScrollable = null;
		//                                for (int i = 1; i < poaAtomData.GetSize(); i++)
		//                                {
		//                                    CMultiList pLoadAtomList = (CMultiList)(poaAtomData.GetAt(i));

		//                                    foreach (CAtom pInerAtom in pLoadAtomList)
		//                                    {
		//                                        if (null == pInerAtom)
		//                                            continue;

		//                                        pInerAtom.SetDBMaster(pRefDBMaster);
		//                                        if (false != _Kiss.IsKindOf(pInerAtom, typeof(CScrollable)))
		//                                            pScrollable = (CScrollable)(pInerAtom);
		//                                        else
		//                                        {
		//                                            if (null != pScrollable)
		//                                            {
		//                                                //여기에 이 코드가 꼭 필요한 것인지?
		//                                                // 이코드가 있을 경우 팝업에 묶이고 스크롤에 묶인 입력란의 부가정보를 띄우고 난후에
		//                                                // SetAtomtoPatchOccur 로 인해 연결된 아톰이 바뀌기 때문에 팝업이 뜨지 않는 현상이 일어남.kkj
		//                                                //	pInerAtom.SetAtomtoPatchOccur (pScrollable);
		//                                                pScrollable = null;
		//                                            }
		//                                        }
		//                                    }

		//                                }
		//                            }
		//                        }
		//                    }
		//                }
		//                SetChildDataReceiveAtom(pAtom);

		//                ///////////////////////////////////////////////////////////////////////////////
		//                // axDHTMLEdit_DocumentComplete
		//                ArrayList pArrayList = new ArrayList();
		//                foreach (CAtom pHtmlEdit in pChildDOC.GetOrderAtom())
		//                {
		//                    if (pHtmlEdit is ProcessQ.TopWebAtom.CWebDHtmlEdit ||
		//                        pHtmlEdit is ProcessQ.TopWebAtom.CWebHtmlTag)
		//                    {
		//                        pArrayList.Add(pHtmlEdit);
		//                    }
		//                }

		//                if (0 < pArrayList.Count)
		//                {
		//                    ScrollPanel pScrollPanel = pParentDOC.GetScrollPanel();
		//                    if (null != pScrollPanel)
		//                    {
		//                        pScrollPanel.AutoScroll = false;

		//                        if (pParent is Panel)
		//                            pParent.Visible = false;
		//                    }
		//                }
		//                ////////////////////////////////////////////////////////////////////////////////

		//                pChildDOC.InitChildLoadFieldData(poaAtomData);
		//                pChildDOC.IsReferenceForm = true;
		//                pChildDOC.SetCreateCond(strArgs);
		//                pChildDOC.ExecuteRunMode(poaAtomData, true, pParentDOC);

		//                ////////////////////////////////////////////////////////////////////////////////
		//                // axDHTMLEdit_DocumentComplete
		//                if (0 < pArrayList.Count)
		//                {
		//                    /*
		//                    while (true)
		//                    {
		//                        bool bDocumentComplete = true;
		//                        foreach (CAtom pHtmlEdit in pArrayList)
		//                        {
		//                            Application.DoEvents ();

		//                            if (pHtmlEdit is ProcessQ.TopWebAtom.CWebDHtmlEdit)
		//                            {
		//                                ProcessQ.TopWebAtom.CWebDHtmlEdit pDHtmlEdit = pHtmlEdit as ProcessQ.TopWebAtom.CWebDHtmlEdit;
		//                                bDocumentComplete &= pDHtmlEdit.DocumentComplete;
		//                            }
		//                            else if (pHtmlEdit is ProcessQ.TopWebAtom.CWebHtmlTag)
		//                            {
		//                                ProcessQ.TopWebAtom.CWebHtmlTag pHtmlTag = pHtmlEdit as ProcessQ.TopWebAtom.CWebHtmlTag;
		//                                bDocumentComplete &= pHtmlTag.DocumentComplete;
		//                            }
		//                        }

		//                        if (false != bDocumentComplete)
		//                            break;
		//                    };
		//                    */

		//                    ScrollPanel pScrollPanel = pParentDOC.GetScrollPanel();
		//                    if (null != pScrollPanel)
		//                    {
		//                        if (pParent is Panel)
		//                            pParent.Visible = true;

		//                        pScrollPanel.AutoScroll = true;
		//                        pScrollPanel.AutoScrollPosition = new Point(0, 0);
		//                    }

		//                    pArrayList.Clear();
		//                }
		//                ////////////////////////////////////////////////////////////////////////////////

		//                // AutoSize
		//                if (null != pParent && pParent is Panel)
		//                {
		//                    if (0 < pParent.Controls.Count)
		//                    {
		//                        if (false != bAutoSize)
		//                        {
		//                            rcBounds = pChildDOC.GetMaxBounds();

		//                            if (null != pParent.Parent)
		//                                pParent.Parent.Size = new Size(rcBounds.Width, rcBounds.Height);
		//                        }

		//                        CLightDMTView pView = pChildDOC.GetFirstView();
		//                        pView.Dock = DockStyle.None;
		//                        pView.Bounds = pParent.ClientRectangle;

		//                        pView.BringToFront();

		//                        foreach (Control pControl in pParent.Controls)
		//                        {
		//                            if (pControl != pView)
		//                            {
		//                                if (pControl is CLightDMTView)
		//                                {
		//                                    CLightDMTView pOldView = pControl as CLightDMTView;
		//                                    LightJDoc pOldDocument = pOldView.GetDocument();
		//                                    if (null != pOldDocument)
		//                                    {
		//                                        // 1. 이작업이 없으면 Embedding 된 경우 Document 가 소멸되지 않는다.
		//                                        //    ProcessQ.Script.CScriptDoc 의 m_poaChildDOCs 에서 자식폼들의 Document 를 멤버로
		//                                        //    가지고 있기 때문에 이 멤버를 Clear 시켜야 한다.
		//                                        //    Frame 이 있는 경우에는 CLightFrame.OnClosing 에서 ExitExecute 를 호출하여 소멸되지만
		//                                        //    Frame 이 없는 Embedding 된 경우 이 함수를 직접 호출한다.
		//                                        // 2. 소멸되는지는 ProcessQ.TopApp.CTopDoc  ~CTopDoc() 로 들어와야 완전한 소멸이 되는지 확인이 된다.
		//                                        // 3. 소멸자로 들어오지 않는 경우 어디선가 Document 를 멤버로 소유하고 있는 문제를 찾아보세요.
		//                                        pOldDocument.ExitExecute();
		//                                        //
		//                                    }
		//                                }

		//                                pControl.Dispose();
		//                            }
		//                        }
		//                    }

		//                    // SmartClient 크기조정
		//                    if (null != m_pLightDMTView && m_pLightDMTView.Parent is Panel)
		//                    {
		//                        while (null != pParent)
		//                        {
		//                            if (pParent is Panel)
		//                            {
		//                                if (pParent.Name == "panel1")	// TopCtrWnd.WebFrmWnd.panel1 (SmartClient) 
		//                                {
		//                                    rcBounds = pParentDOC.GetMaxBounds();

		//                                    pParent.Height = rcBounds.Height;
		//                                    pParent.Parent.Height = rcBounds.Height;
		//                                    break;
		//                                }
		//                            }

		//                            pParent = pParent.Parent;
		//                        }
		//                    }
		//                    //
		//                }
		//                //
		//            }
		//            catch (Softpower.SmartMaker.Define.SmartMakerException ex)
		//            {
		//                _Error80.Show(ex);
		//                if (null != pChildDOC)
		//                    pChildDOC = null;
		//                LogManager.WriteLog(ex);
		//            }
		//            return pChildDOC;
		//        }

		public override void SetDataFromChildForm (CObArray pDataFromChild)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
			{
				//부모폼 에서 작업
				LightJDoc pParentDoc = pLightJDoc.GetParentDOC () as LightJDoc;
				if (null != pParentDoc)
					pParentDoc.SetDataFromChildDoc (pDataFromChild);
			}
		}

		//2004.04.01 khj 호출처회귀
		public override void ReturnDataToParent ()
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;

			if (null != pLightJDoc)
			{
				//80 추후작업 
				//throw new Exception("ReturnDataToParent");
				//pLightJDoc.ReturnDataToParent();
			}
		}

		//        public override void EmptyAtomList(Control pTabView, int dwOption/*=0x00001000*/, bool bNotClear)
		//        {
		//            if (null == pTabView || pTabView.GetType() != typeof(CLightDMTView))
		//                return;

		//            CLightDMTView pView = (CLightDMTView)(pTabView);

		//            CMultiList polAtomList = pView.GetAtomList(-1);

		//            pView.EmptyAtomList(polAtomList, dwOption, bNotClear);
		//        }

		//        public override int GetRunModeX()
		//        {
		//            return (null != m_pLightDMTView) ? m_pLightDMTView.GetRunmode() : 0;
		//        }

		public override void GroupBoxZOrderChange ()
		{
			//80 추후작업
			return;
			//if (null != m_pLightDMTView)
			//{
			//    //m_pLightDMTView.GroupBoxZOrderChange();
			//}
		}

		//        // 2004.07.06 YY 추가...
		public override Atom GetProperVarAtom (string sProperVar, Object pFindView)
		{
			//2006.6.29,김영택, 이 부분은 입력란 스크립트 이벤트 두번누름을 사용할 때 
			// 내부에서 입력란을 가지고 있는 폼을 (자신폼)을 닫을 때 m_pLightDMTView 가
			// null인경우가 있어서 수정 하였음.
			LightJDoc pLightJDoc = (null == m_pLightDMTView) ? null : m_pLightDMTView.Document;
			if (null != pLightJDoc)
				return pLightJDoc.GetProperVarAtom (sProperVar, (LightDMTView)(pFindView));

			return null;
		}

		public override string GetDefaultProperVar (string sProperVar)
		{
			LightJDoc pLightJDoc = (null == m_pLightDMTView) ? null : m_pLightDMTView.Document;
			if (null != pLightJDoc)
				return pLightJDoc.GetDefaultProperVar (sProperVar);

			return null;
		}

		//        /*
		//        public override void LoadDocumentInfo (string strDocInfo)
		//        {
		//            LightJDoc  pLightJDoc = m_pLightDMTView.GetDocument ();
		//            if (null != pLightJDoc)
		//            {
		//                pLightJDoc.LoadDocumentInfo (strDocInfo);
		//            }
		//        }
		//        */

		//        public override void SetTransaction(bool bTrans)
		//        {
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            if (null != pLightJDoc)
		//            {
		//                pLightJDoc.SetTransaction(bTrans);
		//            }
		//        }

		public override bool IsEndAutoTrans
		{
			set
			{
				if (null != m_pLightDMTView)
				{
					LightJDoc pLightJDoc = m_pLightDMTView.Document as LightJDoc;
					if (null != pLightJDoc)
					{
						pLightJDoc.IsEndAutoTrans = value;
					}
				}
			}

			get
			{
				bool bEndAutoTrans = true;
				if (null != m_pLightDMTView)
				{
					LightJDoc pLightJDoc = m_pLightDMTView.Document as LightJDoc;
					if (null != pLightJDoc)
					{
						bEndAutoTrans = pLightJDoc.IsEndAutoTrans;
					}
				}
				return bEndAutoTrans;
			}

		}

		public override void ChangeDefaultTableName (string strTitle)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;

			if (null != pLightJDoc)
			{
				pLightJDoc.ChangeDefaultTableName (strTitle);
			}

		}

		//        /*
		//        /// <remarks>
		//        /// 작성자: 형국재
		//        /// 변경일: 2006.02.20
		//        /// </remarks>
		//        /// <summary>
		//        /// NtoaAtom->MasterDialog에서 결재처리할 때 LightJDoc에 BPM이벤트 발생시키기 위한 함수
		//        /// </summary>
		//        /// <param name="strDK2"></param>문서 처리단계 MST의 DK2
		//        public override void OccureBPMEvent (string strDK2)
		//        {
		//            /////////////////////////////////////////////////
		//            LightJDoc  pLightJDoc = m_pLightDMTView.GetDocument ();
		//            if (null != pLightJDoc)
		//            {
		//                pLightJDoc.OccureBPMEvent (strDK2);
		//            }
		//            /////////////////////////////////////////////////
		//        }

		//        public override bool IsBPM ()
		//        { 
		//            LightJDoc  pLightJDoc = m_pLightDMTView.GetDocument ();
		//            if (null != pLightJDoc && false != pLightJDoc.IsBPM ())
		//            {
		//                return true;
		//            }

		//            return false; 
		//        }
		//         * */

		public virtual void SetParent (LightDMTView pDMTView) { m_pLightDMTView = pDMTView; }
		//        //public virtual void	SetScrLine (int nLine)				 { m_nScrLine = nLine; }
		//        public override void ChangeDBIndexCustody(int nDBIndex)
		//        {
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            if (null != pLightJDoc)
		//            {
		//                //_Message80.Show("ChangeDBIndexCustody override");
		//                //pLightJDoc.GetPageManager().ChangeDBIndexCustody( nDBIndex );
		//            }
		//        }

		//        public override void PageManagerPaint()
		//        {
		//            /*
		//            LightJDoc  pLightJDoc = m_pLightDMTView.GetDocument ();
		//            if (null != pLightJDoc)
		//            {
		//                CPageManager pManager = pLightJDoc.GetPageManager ();
		//                if(null != pManager)
		//                    pManager.DrawNcPaint ();
		//            }
		//            return ;
		//             * */
		//        }

		//        public override void PageFrameTextSetting()
		//        {
		//            PageFrameTextSetting(0);
		//        }

		//        public override void PageFrameTextSetting(int nType)
		//        {
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            if (null != pLightJDoc)
		//            {
		//                Form pParentWnd = pLightJDoc.GetThisForm();
		//                CLightFrame pLightFrame = (CLightFrame)pParentWnd;

		//                pLightFrame.SetActiveOleTitle(nType);

		//            }
		//        }

		//        public override int GetDocDBIndexFromModuleKey(string strModuleKey)
		//        {
		//            int i = 0;
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            if (null != pLightJDoc)
		//            {
		//                //_Message80.Show("GetDocDBIndexFromModuleKey override");
		//                i = pLightJDoc.GetDocDBIndexFromModuleKey(strModuleKey);

		//            }
		//            //_Message80.Show(i.ToString());
		//            return i;
		//        }

		//        public override bool FileOpen(string strFileName)
		//        {
		//            LightJDoc pNewDOC = new LightJDoc();

		//            if (null != pNewDOC)
		//                pNewDOC.SetNtoaDocKind(1);

		//            if (null == pNewDOC || false == pNewDOC.CreateForm(null))
		//                _Exception.ThrowEmptyException();

		//            pNewDOC.OnOpenDocument(strFileName);
		//            pNewDOC.ExecuteRunMode(null, true, null);


		//            return true;
		//        }

		//        public override void FileOpenLoadSanction(string strFileName)
		//        {
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            CCreateForm pcf = new CCreateForm();

		//            pcf.SetCreateFormOption((0 == _Registry.ReadInt32("Bos", "FormSingleLoad", 0)) ? true : false);  //true.#생성 false.#호출
		//            //
		//            pcf.SetFormName(strFileName); // 파일경로

		//            pcf.SetCmdLine(1); // 보임형태

		//            string strFormName = pcf.GetFormName();
		//            strFormName = strFormName.Trim();
		//            if (0 == strFormName.Length)
		//                return;


		//            //CFormServer pFormServer = null;

		//            // 1. 새로운 폼을 하나 띄운다..
		//            CScriptDoc pNewDOC = CScriptApp.CreateForm(pLightJDoc, pcf);
		//            if (null == pNewDOC)
		//                _Exception.ThrowEmptyException();

		//            // 2. Server를 생성한다..
		//            //if (null == (pFormServer = (CFormServer)(System.Activator.CreateInstance (typeof (CFormServer)))))
		//            //_Exception.ThrowEmptyException ();
		//            //pFormServer.AddRef ();

		//            if (null != pLightJDoc)
		//                pNewDOC.SetCustStatusBar(pLightJDoc.GetCustStatusBar());

		//            pNewDOC.ScanDocChange();

		//            //pFormServer.SetDocument (pNewDOC);
		//            //CObjectX pFormChild = (CObjectX)pFormServer;
		//            // 결재실행
		//            string strRegisterNum = "";
		//            int nSanctionKind = 5;// 선람
		//            int nVersion = -1; //Version을 사용자가 셋팅하지 않을경우는 무조건 -1로 세팅하여 버전은 기본버전으로 호출하도록 한다.

		//            pNewDOC.SanctExecute(strRegisterNum, nSanctionKind, nVersion, "1010", "1113"); // 김은영만 테스트
		//        }

		//        /*
		//        public override void ScanDo (Form form, int nScanUnit)
		//        {
		//            CLightFrame pNewForm = (CLightFrame) form;
		//            LightJDoc pNewDOC = pNewForm.GetDocument ();

		//            CPageManager pManage = ((LightJDoc)pNewDOC).GetPageManager ();
		//            CAtom pSampleAtom = new CAtom ();

		//            CLightDMTView pDMTView = ((LightJDoc)pNewDOC).GetFirstView ();

		//            if(null != pManage)
		//            {
		//                pManage.OnScanImageAdd (ref pSampleAtom);

		//                CNtoaMove pMoveAtom = (CNtoaMove) pSampleAtom;
		//                pMoveAtom.SetOriginPoint (pDMTView.Location, false);
		//                pMoveAtom.SetOldPoint (new Point(pDMTView.Location.X + pDMTView.Size.Width, pDMTView.Location.Y + pDMTView.Size.Height), true);
		//                CMoveofAtom pMoveofAtom = (CMoveofAtom)pMoveAtom.GetWnd ();
		//                Rectangle rect = new Rectangle (pDMTView.Location ,pDMTView.Size);

		//                string strHandleMethod	= _Registry.ReadString ("Environment\\Scanner", "HandleMethod", "0");
		//                string strScanDestination	= _Registry.ReadString ("Environment\\Scanner", "ScanDestination", "0");
		//                string strResolution	= _Registry.ReadString ("Environment\\Scanner", "Resolution", "100");
		//                string strColorAttrib	= _Registry.ReadString ("Environment\\Scanner", "ColorAttrib", "2");
		//                string strBrightness  = _Registry.ReadString ("Environment\\Scanner", "Brightness", "50");

		//                string strContrast  = _Registry.ReadString ("Environment\\Scanner", "Contrast", "50");
		//                string strnX = _Registry.ReadString ("Environment\\Scanner", "nX", "50");
		//                string strnY  = _Registry.ReadString ("Environment\\Scanner", "nY", "50");
		//                string strnWidth  = _Registry.ReadString ("Environment\\Scanner", "nWidth", "50");
		//                string strnHeight  = _Registry.ReadString ("Environment\\Scanner", "nHeight", "50");
		//                string strPaper  = _Registry.ReadString ("Environment\\Scanner", "Paper", "50");




		//                int HandleMethod = Convert.ToInt32(strHandleMethod);
		//                int ScanDestination = Convert.ToInt32(strScanDestination);
		//                int nResolution = Convert.ToInt32(strResolution);
		//                int nColor = Convert.ToInt32(strColorAttrib);
		//                int nBrightness = Convert.ToInt32(strBrightness);
		//                int nContrast = Convert.ToInt32(strContrast);

		//                if(nBrightness >= 0 && nBrightness <= 50)
		//                {
		//                    nBrightness = -1 * (1000 - nBrightness * 20); 
		//                }
		//                else
		//                {
		//                    nBrightness = (nBrightness - 50) * 20;
		//                }

		//                if(nContrast >= 0 && nContrast <= 50)
		//                {
		//                    nContrast = -1 * (1000 - nContrast * 20); 
		//                }
		//                else
		//                {
		//                    nContrast = (nContrast - 50) * 20;
		//                }


		//                float nX = Convert.ToSingle (strnX);
		//                float nY = Convert.ToSingle (strnY);
		//                float nWidth = Convert.ToSingle (strnWidth);
		//                float nHeight = Convert.ToSingle (strnHeight);

		//                RectPaper rectpaper = new RectPaper();
		//                rectpaper.X = nX;
		//                rectpaper.Y = nY;
		//                rectpaper.Width = nWidth;
		//                rectpaper.Height = nHeight;


		//                pMoveofAtom.ScanPageUnit (rect, nScanUnit, nResolution, nColor, nBrightness, nContrast, rectpaper, false, null, ScanDestination, true);

		//                //	pMoveofAtom.ScanPageUnit (rect, nScanUnit);



		//                //	pManage.OnPageManager ();
		//            }
		//        }
		//         * */

		//        /*
		//        public string LoadDataGroupDB (CDBMaster pDBMaster, string strUser)
		//        {
		//            if (null == pDBMaster) return "";
		//            string strGroupCode = "";
		//            try
		//            {

		//                CDatabaseX pDatabaseX = (CDatabaseX) (pDBMaster.GetDatabaseX (0)); //topcomm 2->0
		//                if (null != pDatabaseX && false != pDatabaseX.IsOpen)
		//                {
		//                    //pDBMaster.RegistryTransX (pDatabaseX);

		//                    String strTable = "TOP_USR";
		//                    String strOwner  = pDBMaster.GetActiveOwner (0);//topcomm 2->0

		//                    if (false != _Kiss._isempty (strTable)) return "";

		//                    if (false == _Kiss._isempty (strOwner) && -1 == strTable.IndexOf (strOwner))
		//                        strTable = String.Concat (strOwner , strTable);



		//                    CSanctionProcess pSantionProcess = new CSanctionProcess (pDBMaster);


		//                    String strWhere = "";
		//                    //String strTemp  = "";
		//                    String strFields = "S03, S04";
		//                    String strOrder = "S03";
		//                    strWhere = String.Format("S00 = '{0}' AND S01 = '{1}'", pSantionProcess.GetDivisionCode (), strUser);



		//                    ArrayList oaData = new CObArray ();
		//                    bool bCheck = DBLib.KissSQLGetSelAllRows (ref oaData, pDatabaseX, strFields, strTable, strWhere, "", strOrder);
		//                    if (0 >= oaData.Count) return "";

		//                    int nObArryCount  = 0;
		//                    //int nStrArryCount = 0;
		//                    nObArryCount  = oaData.Count;

		//                    //int nItem = 0;
		//                    ArrayList pArrayList =(ArrayList) oaData[0]; 


		//                    String str1 = pArrayList [0].ToString ();	
		//                    //String str2 = pArrayList [1].ToString ();	
		//                    str1 = str1.Trim ();	
		//                    //str2 = str2.Trim ();	
		//                    strGroupCode = str1;	
		//                    //m_parNameData.Add(str2);


		//                    foreach (ArrayList pArrayList1 in oaData) 
		//                    {	
		//                        pArrayList1.Clear ();
		//                    }
		//                }
		//            }
		//            catch (Softpower.SmartMaker.Define.SmartMakerException e) 
		//            { 
		//                _Error80.Show (e);
		//                LogManager.WriteLog(e);
		//            }
		//            return strGroupCode;
		//        }
		//         * */


		//        public override void FileOpenBOD(string strFileName)
		//        {
		//            // 객체생성
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            CCreateForm pcf = new CCreateForm();

		//            pcf.SetCreateFormOption((0 == _Registry.ReadInt32("Bos", "FormSingleLoad", 0)) ? true : false);  //true.#생성 false.#호출
		//            //
		//            pcf.SetFormName(strFileName); // 파일경로

		//            pcf.SetCmdLine(1); // 보임형태

		//            string strFormName = pcf.GetFormName();
		//            strFormName = strFormName.Trim();
		//            if (0 == strFormName.Length)
		//                return;


		//            //CFormServer pFormServer = null;

		//            // 1. 새로운 폼을 하나 띄운다..
		//            CScriptDoc pNewDOC = CScriptApp.CreateForm(pLightJDoc, pcf);
		//            if (null == pNewDOC)
		//                _Exception.ThrowEmptyException();

		//            // 2. Server를 생성한다..
		//            //if (null == (pFormServer = (CFormServer)(System.Activator.CreateInstance (typeof (CFormServer)))))
		//            //_Exception.ThrowEmptyException ();
		//            //pFormServer.AddRef ();

		//            if (null != pLightJDoc)
		//                pNewDOC.SetCustStatusBar(pLightJDoc.GetCustStatusBar());

		//            //pFormServer.SetDocument (pNewDOC);
		//            //CObjectX pFormChild = (CObjectX)pFormServer;
		//            // 결재실행
		//            string strRegisterNum = "";
		//            int nSanctionKind = 1;
		//            int nVersion = -1; //Version을 사용자가 셋팅하지 않을경우는 무조건 -1로 세팅하여 버전은 기본버전으로 호출하도록 한다.

		//            pNewDOC.SanctExecute(strRegisterNum, nSanctionKind, nVersion);
		//            pNewDOC.MDBODButtonEnable(false);
		//        }

		//        public override void FormModelOpen(bool bD01, string strRegistNum, bool bSave, string strF01, string strF09, string strServiceSvr)
		//        {
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            if (null != pLightJDoc)
		//            {
		//                pLightJDoc.FormModelOpen(bD01, strRegistNum, bSave, strF01, strF09, strServiceSvr);
		//            }
		//            return;
		//        }

		//        /*
		//        public override string GetStrDK2 () 
		//        {
		//            string strDK2 = "2";
		//            LightJDoc  pLightJDoc = m_pLightDMTView.GetDocument ();
		//            CPageManager pManage = null;
		//            CMasterManager pMasterManager = null;
		//            MasterDialog pMasterDialog = null;
		//            if(null != pLightJDoc)
		//            {
		//                pManage = pLightJDoc.GetPageManager ();
		//                if(null != pManage)
		//                {
		//                    pMasterManager = pManage.GetMasterManager ();
		//                    if(null != pMasterManager)
		//                    {
		//                        pMasterDialog = pMasterManager.GetMasterPage ();
		//                        if(null != pMasterDialog)
		//                        {
		//                            strDK2 = pMasterDialog.GetStrDK2 ();
		//                        }
		//                    }
		//                }
		//            }
		//            return strDK2;
		//        }
		//         * */

		//        /*
		//        public override int GetMasterSanctionKind () 
		//        {
		//            int nSanctionKind = 1;
		//            LightJDoc  pLightJDoc = m_pLightDMTView.GetDocument ();
		//            CPageManager pManage = null;
		//            CMasterManager pMasterManager = null;
		//            MasterDialog pMasterDialog = null;
		//            if(null != pLightJDoc)
		//            {
		//                pManage = pLightJDoc.GetPageManager ();
		//                if(null != pManage)
		//                {
		//                    pMasterManager = pManage.GetMasterManager ();
		//                    if(null != pMasterManager)
		//                    {
		//                        pMasterDialog = pMasterManager.GetMasterPage ();
		//                        if(null != pMasterDialog)
		//                        {
		//                            nSanctionKind = pMasterDialog.GetMasterSanctionKind ();
		//                        }
		//                    }
		//                }
		//            }
		//            return nSanctionKind;

		//        }
		//         * */

		//        /*
		//        public override void OnScanPageClear ()
		//        {
		//            LightJDoc  pLightJDoc = m_pLightDMTView.GetDocument ();
		//            CPageManager pManage = null;
		//            CMasterManager pMasterManager = null;
		//            MasterDialog pMasterDialog = null;
		//            if(null != pLightJDoc)
		//            {
		//                pManage = pLightJDoc.GetPageManager ();
		//                if(null != pManage)
		//                {
		//                    pMasterManager = pManage.GetMasterManager ();
		//                    if(null != pMasterManager)
		//                    {
		//                        pMasterDialog = pMasterManager.GetMasterPage ();
		//                        pMasterDialog.ScanPageClear ();

		//                    }
		//                }
		//            }
		//        }
		//         * */

		//        /*
		//        public override void OnScanMoveAtomClear ()
		//        {
		//            LightJDoc  pLightJDoc = m_pLightDMTView.GetDocument ();
		//            if(null != pLightJDoc)
		//            {
		//                if(null != pLightJDoc.ScanMoveofAtom)
		//                    pLightJDoc.ScanMoveofAtom = null;
		//            }
		//        }
		//        */


		//        public override bool ExecuteScript()
		//        {
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            if (null != pLightJDoc)
		//            {
		//                pLightJDoc.ExecuteScript();
		//            }
		//            return true;
		//        }

		//        public override void F9KeyHandler()
		//        {
		//            m_pLightDMTView.F9KeyHandler();
		//        }

		//        public override void PrintPreView()
		//        {
		//            m_pLightDMTView.PrintPreView();
		//        }

		//        public override bool BuildCvtInfo()
		//        {
		//            bool bTrue = true;
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            if (null != pLightJDoc)
		//            {
		//                bTrue = pLightJDoc.BuildCvtInfo();
		//            }
		//            return bTrue;
		//        }

		//2007.06.25 이현숙 자연어 
		public override bool Review ()
		{
			bool bTrue = true;
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
			{
				bTrue = pLightJDoc.Review ();
			}
			return bTrue;
		}

		//        public override bool GetTransaction()
		//        {
		//            bool bTrue = false;
		//            if (null != m_pLightDMTView)
		//            {
		//                LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//                if (null != pLightJDoc)
		//                {
		//                    bTrue = pLightJDoc.GetTransaction();
		//                }
		//            }
		//            return bTrue;
		//        }

		//        public override CTransXMgr GetTransXMgr()
		//        {
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            CTransXMgr pTransMgr = null;
		//            if (null != pLightJDoc)
		//            {
		//                pTransMgr = pLightJDoc.GetTransXMgr();
		//            }
		//            return pTransMgr;
		//        }

		//        public override void SetTransXMgr(CTransXMgr pTransXMgr)
		//        {
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            if (null != pLightJDoc)
		//            {
		//                pLightJDoc.SetTransXMgr(pTransXMgr);
		//            }
		//        }

		//        /// <summary>
		//        /// 외부에서 DB에서 트랜잭션 메니져를 만들어 낸다.
		//        /// </summary>
		//        /// <returns></returns>
		//        public override bool SetOutsideQueryTransXMgr()
		//        {
		//            bool bResult = false;
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            if (null != pLightJDoc)
		//            {
		//                bResult = pLightJDoc.SetOutsideQueryTransXMgr();
		//            }

		//            return bResult;
		//        }

		//        public override void SetParentTransXMgr(CTransXMgr pTransXMgr, bool bTransParent)
		//        {
		//            LightJDoc pLightJDoc = m_pLightDMTView.GetDocument();
		//            LightJDoc pParentJDoc = pLightJDoc.GetParentDOC() as LightJDoc;
		//            if (null != pParentJDoc && null != pLightJDoc)
		//            {
		//                pLightJDoc.SetTransXMgr(pTransXMgr, bTransParent);
		//                pParentJDoc.SetTransXMgr(pTransXMgr, !bTransParent);
		//            }
		//        }


		/// <summary>
		/// 버튼.저장, 삭제()를 사용하는 경우 중간에 트랜잭션이 처리되는 것을 막기 위해 사용됨.
		/// </summary>
		/// <param name="bUseButtonMethod"></param>
		public override void SetUseButtonMethod (bool bUseButtonMethod)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document as LightJDoc;
			pLightJDoc.UseButtonMethod = bUseButtonMethod;
		}

		public override CMapStringToStringX PMVariable ()
		{
			CMapStringToStringX pMapStringToStringX = new CMapStringToStringX ();
			if (null != m_pLightDMTView)
				pMapStringToStringX = m_pLightDMTView.Document.PMVariable ();
			else
				pMapStringToStringX = new CMapStringToStringX ();
			return pMapStringToStringX;

		}

		public override int ProcessEvent (int nLvl, int wEventID, CVariantX[] pvaArgs = null)
		{
			if (null == m_pLightDMTView)
				return 1;
			return m_pLightDMTView.Document.ProcessEvent (nLvl, wEventID, pvaArgs);
		}

		public override int ProcessEvent (int nLvl, int wEventID, CObArray pCurrentInfo, CVariantX[] pvaArgs)
		{
			if (null == m_pLightDMTView)
				return 1;
			return m_pLightDMTView.Document.ProcessEvent (nLvl, wEventID, pCurrentInfo, pvaArgs);
		}

		//        #region 호출처회기 관련

		public void SetChildDataReceiveAtom (Atom pAtom)
		{
			if (null == pAtom)
				return;

			LightJDoc pParentDOC = m_pLightDMTView.Document;

			pParentDOC.SetChildDataReceiveAtom (pAtom);
		}


		/*
		* 2008.01.15 황성민
		* 실행질의문 보기의 논리변경에 의해서 추가합니다.
		*/
		public override void ShowRuntimeSQLView (string strSQL)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document as LightJDoc;
			if (null != pLightJDoc)
			{
				pLightJDoc.ShowRuntimeSQLView (strSQL);
			}
		}

		public override COrderAtom GetOrderAtom ()
		{
			return this.m_pLightDMTView.GetOrderAtom ();
		}


		#region 아톰 통합

		public override void SetAttribToAtom (ref Atom _atom, object objType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribToAtom (ref _atom, objType);
		}

		public override void SetAttribMenuAtom (ref Atom webMenu, MenuType menuType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribMenuAtom (ref webMenu, menuType);
		}

		public override void SetAttribAdvertiseAtom (ref Atom pAtom)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribAdvertiseAtom (ref pAtom);
		}

		public override void SetAttribPaymentAtom (ref Atom pAtom)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribPaymentAtom (ref pAtom);
		}

		public override void SetAttribDecorImageAtom (ref Atom pAtom)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribDecorImageAtom (ref pAtom);
		}

		public override void SetAttribMapViewAtom (ref Atom pAtom)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribMapViewAtom (ref pAtom);
		}

		public override void SetAttribGridAtom (ref Atom pAtom)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribGridAtom (ref pAtom);
		}

		public override void SetAttribTreeAtom (ref Atom pAtom, int nTreeType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribTreeAtom (ref pAtom, nTreeType);
		}

		public override void SetAttribTabViewAtom (ref Atom pAtom, int nTabViewType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribTabViewAtom (ref pAtom, nTabViewType);
		}

		public override void SetAttribProgressAtom (ref Atom pAtom, int nProgressType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribProgressAtom (ref pAtom, nProgressType);
		}

		public override void SetAttribSliderAtom (ref Atom pAtom, int nSliderType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribSliderAtom (ref pAtom, nSliderType);
		}

		public override void SetAttribRangeSliderAtom (ref Atom pAtom, int nSliderType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribRangeSliderAtom (ref pAtom, nSliderType);
		}

		public override void SetAttribClockAtom (ref Atom pAtom, int nClockType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribClockAtom (ref pAtom, nClockType);
		}

		public override void SetAttribCodeAtom (ref Atom pAtom, int nCodeType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribCodeAtom (ref pAtom, nCodeType);
		}

		public override void SetAttribMediaAtom (ref Atom pAtom, int nPlayerKind)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribMediaAtom (ref pAtom, nPlayerKind);
		}

		public override void SetAttribCheckAtom (ref Atom pAtom, int nCheckType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribCheckAtom (ref pAtom, nCheckType);
		}

		public override void SetAttribInputAtom (ref Atom pAtom, int nInputType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribInputAtom (ref pAtom, nInputType);
		}

		public override void SetAttribImageAtom (ref Atom pAtom, int nImageType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribImageAtom (ref pAtom, nImageType);
		}

		public override void SetAttribFileAttachAtom (ref Atom pAtom, int nImageType)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.Document;
			if (null != pLightJDoc)
				pLightJDoc.SetAttribFileAttachAtom (ref pAtom, nImageType);
		}
		#endregion

		public override Point GetMouseAbsolutePosition ()
		{
			if (null == this.m_pLightDMTView) return new Point (100, 100);
			Point ptRelative = System.Windows.Input.Mouse.GetPosition (this.m_pLightDMTView);
			Point ptAbsolute = this.m_pLightDMTView.PointToScreen (ptRelative);
			return ptAbsolute;
		}

		public override Point GetAtomCenterAbsolutePosition (Atom atomCore)
		{
			AtomBase atomBase = atomCore.GetOfAtom () as AtomBase;

			if (null == atomBase)
			{
				return base.GetAtomCenterAbsolutePosition (atomCore);
			}


			Window MainWindow = Application.Current.MainWindow;
			PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual (MainWindow);
			System.Windows.Media.Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
			double thisDpiWidthFactor = m.M11;
			double thisDpiHeightFactor = m.M22;

			double dCenterHeight = atomCore.Attrib.AtomHeight / 2;

			Point ptRelative = new Point ();
			Point ptAbsolute = atomBase.PointToScreen (ptRelative);
			ptAbsolute.X = ptAbsolute.X / thisDpiWidthFactor;
			ptAbsolute.Y = (ptAbsolute.Y + dCenterHeight) / thisDpiHeightFactor;
			return ptAbsolute;
		}

		public override object GetDocument ()
		{
			return this.m_pLightDMTView.Document;
		}

		public override Atom GetDuplicateRelationAtom (ScrollAtom pScrollAtom, Atom pAtom)
		{
			return this.m_pLightDMTView.GetDuplicateRelationAtom (pScrollAtom, pAtom);
		}

		public override string GetActionQuery (Atom atomCore)
		{
			return this.m_pLightDMTView.GetActionQuery (atomCore);
		}

		public override void CloseModel ()
		{
			m_pLightDMTView.CloseModel ();
		}

		public override double GetViewPercentOfScale ()
		{
			return m_pLightDMTView.PercentOfScale;
		}

		public override bool IsWebdoc ()
		{
			return m_pLightDMTView.Document.IsWebDoc;
		}

		public override Grid GetThirdControlLayer ()
		{
			return m_pLightDMTView.ThirdControlLayer;
		}

		public override Size GetViewSize ()
		{
			return m_pLightDMTView.GetViewAttrib ().FormSize;
		}

		public override Size GetUIViewSize ()
		{
			LightDMTFrame parentFrame = m_pLightDMTView.GetFrame ();

			if (null != parentFrame)
			{
				return new Size (parentFrame.CurrentBaseScreenView.ActualWidth, parentFrame.CurrentBaseScreenView.ActualHeight);
			}

			return base.GetUIViewSize ();
		}

		public override Canvas GetAnimationLayer ()
		{
			return m_pLightDMTView.AnimationLayer;
		}

		public override void PlayAnimation (Atom atomCore, AnimationDetailEventDefine adeType)
		{
			if (null != atomCore)
			{
				m_pLightDMTView.PlayAnimation (atomCore, adeType);
			}
			else
			{
				m_pLightDMTView.PlayAnimation (adeType);
			}
		}

		public override void PlayEmphasisAnimation (Atom atomCore)
		{
			m_pLightDMTView.PlayEmphasisAnimation (atomCore);
		}

		public override void StopEmphasisAnimation (Atom atomCore)
		{
			m_pLightDMTView.StopEmphasisAnimation (atomCore);
		}

		public override void PlayEmphasisAnimation (Atom atomCore, AnimationDetailEventDefine adeType)
		{
			m_pLightDMTView.PlayEmphasisAnimation (atomCore, adeType);
		}

		public override void SetFormChangeFromAtom ()
		{
			m_pLightDMTView.Document.SetFormChange (true);
		}

		public override void ChangeAtomTextEditMode (bool isTextEdit)
		{
			m_pLightDMTView.Document.ChangeAtomTextEditMode (isTextEdit);
		}

		public override bool IsEBookModel ()
		{
			return m_pLightDMTView.Document.IsEBookDoc;
		}

		public override void ChangeEBookImageAtom (ref Atom atomCore)
		{
			m_pLightDMTView.ChangeEBookImageAtom (ref atomCore);
		}

		public override bool IsHorizontalView ()
		{
			return m_pLightDMTView.Document.GetFrameAttrib ().IsHorizonalView;
		}

		public override bool IsNotClear ()
		{
			return m_pLightDMTView.Document.GetFrameAttrib ().NotClear;
		}

		public override void NotifyToolBarAboutCurrentSelectedAtomWhenCopy ()
		{
			m_pLightDMTView.NotifyToolBarAboutCurrentSelectedAtom ();
		}

		public override object GetPageMaster ()
		{
			LightDMTFrame parentFrame = m_pLightDMTView.GetFrame ();
			if (null != parentFrame)
			{
				// DigitalBook
				//return parentFrame.PageMasterView;
			}

			return null;
		}

		public override bool IsParentPageIsMasterView ()
		{
			PhoneScreenView pageMasterView = GetPageMaster () as PhoneScreenView;
			return null == pageMasterView ? false : pageMasterView.CurrentTopView == m_pLightDMTView;
		}

		public override int GetCurrentPageOnEBook ()
		{
			LightDMTFrame parentFrame = m_pLightDMTView.GetFrame ();

			if (null == parentFrame)
			{
				return -1;
			}

			int nPage = parentFrame.EBookManager.GetCurrentPage ();

			return nPage;
		}

		public override int GetTotalPageOnEBook ()
		{
			LightDMTFrame parentFrame = m_pLightDMTView.GetFrame ();

			if (null == parentFrame)
			{
				return -1;
			}

			int nTotalPage = parentFrame.EBookManager.GetPageViewMap ().Count;

			return nTotalPage;
		}

		public override void SetAtomProperVar (AtomBase atomBase, string strProperVar)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.GetDocument () as LightJDoc;
			pLightJDoc.ApplyAtomNameObject (new List<object> () { atomBase }, strProperVar);
		}

		public override int GetNtoaDocKind ()
		{
			return m_pLightDMTView.GetNtoaDocKind ();
		}

		public override AtomBase MakeAnimationGroupAtom ()
		{
			LightJDoc pLightJDoc = m_pLightDMTView.GetDocument () as LightJDoc;
			return pLightJDoc.MakeAnimationGroupAtom ();
		}

		public override AtomBase DeleteAnimationGroupAtom ()
		{
			LightJDoc pLightJDoc = m_pLightDMTView.GetDocument () as LightJDoc;
			return pLightJDoc.DeleteAnimationGroupAtom ();
		}

		public override int GetFrameSizeWidth ()
		{
			LightJDoc pLightJDoc = m_pLightDMTView.GetDocument () as LightJDoc;
			if (null != pLightJDoc)
			{
				Softpower.SmartMaker.TopLight.Models.CFrameAttrib frameAttrib = pLightJDoc.GetFrameAttrib ();

				if (null != frameAttrib)
				{
					return (int)(frameAttrib.FrameSize.Width);
				}
			}
			return 0;
		}

		public override int GetFrameSizeHeight ()
		{
			LightJDoc pLightJDoc = m_pLightDMTView.GetDocument () as LightJDoc;
			if (null != pLightJDoc)
			{
				CFrameAttrib frameAttrib = pLightJDoc.GetFrameAttrib ();

				if (null != frameAttrib)
				{
					return (int)(frameAttrib.FrameSize.Height);
				}
			}
			return 0;
		}

		public override void AutoDisabledMediaAtoms (Atom thisAtom)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.GetDocument () as LightJDoc;
			if (null != pLightJDoc)
			{
				pLightJDoc.AutoDisabledMediaAtoms (thisAtom);
			}
		}

		public override void ExecuteVoiceCommand (Atom pAtom, string strStatement, ref string strCommand)
		{
			LightJDoc pLightJDoc = m_pLightDMTView.GetDocument () as LightJDoc;
			if (null != pLightJDoc)
			{
				pLightJDoc.ExecuteVoiceCommand (pAtom, strStatement, ref strCommand);
			}
		}

		public override CObArray CloneProcessing (CObArray oldProcessing)
		{
			CObArray newList = new CObArray ();
			foreach (object item in oldProcessing)
			{
				CObMapX map = item as CObMapX;
				CObMapX newMap = new CObMapX ();
				foreach (DictionaryEntry item2 in map)
				{
					ArrayList eventList = item2.Value as ArrayList;
					CObArray pNewList = new CObArray ();

					foreach (var item3 in eventList)
					{
						ProcessEventInformation eventInfo = item3 as ProcessEventInformation;
						if (eventInfo != null)
							pNewList.Add (eventInfo.Clone ());
					}

					newMap.Add (item2.Key, pNewList);
				}

				newList.Add (newMap);
			}

			return newList;
		}

		public override int GetStoreMode ()
		{
			if (null != m_pLightDMTView)
			{
				return m_pLightDMTView.Document.StoreMode;
			}

			return 0;
		}


		public override void OnRunModeMoveFocusOnTabIndex ()
		{
			if (null != m_pLightDMTView)
			{
				m_pLightDMTView.OnRunModeMoveFocusOnTabIndex ();
			}
		}


		public override void ReSizeView ()
		{
			if (null != m_pLightDMTView)
			{
				LightJDoc document = m_pLightDMTView.Document;
				Softpower.SmartMaker.TopLight.Models.CFrameAttrib frameAttrib = document.GetFrameAttrib ();
				List<AtomBase> atomList = m_pLightDMTView.GetViewChildren (); // 탭내부, 스크롤에 묶인 아톰 제외

				var phoneScreenView = m_pLightDMTView.GetFrame ().CurrentBaseScreenView as PhoneScreenView;

				if (null == phoneScreenView)
					return;

				double dDefaultViewHeight = frameAttrib.FrameSize.Height;
				double dLastView = dDefaultViewHeight; //초기 설정한 크기보다 작아질수는 없다.
				double dBottomMenuBarHeight = phoneScreenView.GetMenuBottomHeight ();
				double dNavigationBarHeight = 0;

				if (true == document.IsEBookDoc)
					return;

				if (null != phoneScreenView.SystemNavigation)
					dNavigationBarHeight = phoneScreenView.SystemNavigation.ActualHeight;

				foreach (AtomBase atom in atomList)
				{
					double dTempHeight = atom.Margin.Top + atom.Height + dBottomMenuBarHeight + dNavigationBarHeight;

					if (dLastView < dTempHeight)
					{
						dLastView = dTempHeight;
					}
				}

				phoneScreenView.ContentHeight = dLastView;
			}
		}


		public override void ExecuteParentScriptEvent (Atom atomCore, int eventID)
		{
			if (null != atomCore)
			{
				string strAtomName = atomCore.GetProperVar ();

				ScrollAtom scrollAtom = atomCore.IsPatchScrollAtom (atomCore) as ScrollAtom;
				EBookAnimationGroupAtom groupAtom = atomCore.BindGroupAtom as EBookAnimationGroupAtom;

				if (null != scrollAtom)
				{
					scrollAtom.CallMsgHandler (eventID, strAtomName, -1);
				}

				if (null != groupAtom)
				{
					if (-1 != groupAtom.ProcessEvent (0, eventID))
					{
						CVariantX[] pvaArgs = new CVariantX[1 + 1];
						pvaArgs[0] = new CVariantX (1);
						pvaArgs[1] = new CVariantX (strAtomName);

						if (0 <= MsgHandler.CALL_MSG_HANDLER (groupAtom, eventID, pvaArgs))
						{
							groupAtom.ProcessEvent (1, eventID);
						}
					}
				}
			}
		}

		//현재 눌린 아톰이 스크롤, 그룹묶기에 묶여있는경우 스크롤 그룹묶기의 업무규칙을 실행하기 위해서 추가함
		public override void CallAtomBindingParentClickEvent (Atom pAtom)
		{
			if (null != pAtom)
			{
				string strAtomName = pAtom.GetProperVar ();

				ScrollAtom scrollAtom = pAtom.IsPatchScrollAtom (pAtom) as ScrollAtom;
				EBookAnimationGroupAtom groupAtom = pAtom.BindGroupAtom as EBookAnimationGroupAtom;

				if (null != scrollAtom)
				{
					scrollAtom.CallMsgHandler (EVS_TYPE.EVS_A_CLICK, strAtomName, -1);
				}

				if (null != groupAtom)
				{
					CVariantX[] pvaArgs = new CVariantX[1 + 1];
					pvaArgs[0] = new CVariantX (1);
					pvaArgs[1] = new CVariantX (strAtomName);

					if (-1 != groupAtom.ProcessEvent (0, EVS_TYPE.EVS_A_CLICK, pvaArgs))
					{
						if (0 <= MsgHandler.CALL_MSG_HANDLER (groupAtom, EVS_TYPE.EVS_A_CLICK, pvaArgs))
						{
							groupAtom.ProcessEvent (1, EVS_TYPE.EVS_A_CLICK, pvaArgs);
						}
					}
				}
			}
		}

		public override void SearchHeightChange (Atom searchAtomCore, double dChangeHeight)
		{
			if (null != m_pLightDMTView && null != m_pLightDMTView.Document)
			{
				LightJDoc document = m_pLightDMTView.Document as LightJDoc;

				List<Atom> atomCoreList = document.GetAllAtomCores ();

				foreach (Atom atomCore in atomCoreList)
				{
					if (searchAtomCore.Attrib.AtomY + searchAtomCore.Attrib.AtomHeight < atomCore.Attrib.AtomY)
					{
						atomCore.Attrib.AtomY += dChangeHeight;
					}
				}

				ReSizeView ();
			}
		}

		public override void ExecuteAIService (string strServiceName)
		{
			if (null != m_pLightDMTView && null != m_pLightDMTView.Document)
			{
				LightJDoc doc = m_pLightDMTView.Document;

				doc.ExecuteAIService (strServiceName);
			}
		}

		public override List<string> GetAIServceList ()
		{
			List<string> aiServiceList = new List<string> ();

			if (null != m_pLightDMTView && null != m_pLightDMTView.Document)
			{
				LightJDoc doc = m_pLightDMTView.Document;
				aiServiceList = doc.MainAIAdaptorManager.ServiceData.Keys.ToList ();
			}

			return aiServiceList;
		}

		public override void UpdatetPropertyToolBar (AtomBase atom)
		{
			if (null != m_pLightDMTView && null != m_pLightDMTView.Document)
			{
				m_pLightDMTView.LatestedPressedAtom = atom;
				m_pLightDMTView.Document.SetCurrentSelectedAtomProperties (atom);
				m_pLightDMTView.Document.SelectAtomUpdateFlowMap (atom);
			}
		}

		public override bool IsDynamicMode ()
		{
			if (null != m_pLightDMTView && null != m_pLightDMTView.Document)
			{
				CFrameAttrib frameAttrib = m_pLightDMTView.Document.GetFrameAttrib ();
				if (null != frameAttrib)
				{
					return frameAttrib.IsDynamicMode;
				}
			}

			return false;
		}

		public override bool IsUseScriptEvent (string strEventID)
		{

			if (null != m_pLightDMTView && null != m_pLightDMTView.Document)
			{
				LightJDoc document = m_pLightDMTView.Document;

				CScriptServer scriptServer = document.GetScriptServer ();

				if (null != scriptServer)
				{
					CFormExe formExe = scriptServer.GetFormExe ();

					if (null != formExe)
					{
						CMapStringToObX funcMap = formExe.GetFunctionMap ();

						if (null != funcMap)
						{
							if (true == funcMap.ContainsKey (strEventID))
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		public override void ExecuteReportModel (Atom baseActionAtom, bool isPreView, string strFilePath, string strRefValue, string strFileName)
		{
			if (null == m_pLightDMTView)
				return;

			List<Atom> atomCoreList = m_pLightDMTView.GetAllAtomCores ();
			string[] strArray = strRefValue.Split (',');

			FormDataManager formDataManager = new FormDataManager ();

			for (int i = 0; i < strArray.Length; i++)
			{
				string strToken = strArray[i].Trim ();
				string strAtomName = "";
				string strOuterName = "";

				if (-1 < strToken.IndexOf ("=")) //입력란 = 출력란   상세폼보기와 동일한 논리로 좌변항이 우변항으로 이동 (프로그래밍 방식과 반대)
				{
					string[] strTokenArray = strToken.Split ('=');
					if (1 < strTokenArray.Length)
					{
						strAtomName = strTokenArray[0];
						strOuterName = strTokenArray[1];
					}
				}
				else
				{
					strAtomName = strToken;
					strOuterName = strToken;
				}

				if (true == string.IsNullOrWhiteSpace (strAtomName) || true == string.IsNullOrWhiteSpace (strOuterName))
					continue;

				Atom atomCore = atomCoreList.Find (item => strAtomName == item.Name);

				if (null == atomCore)
					continue;

				if (atomCore is BrowseAtom)
				{
					BrowseAtom browseAtomCore = atomCore as BrowseAtom;
					CVariantX retValue = new CVariantX ();

					//출력전 전체 데이터를 조회한다.
					browseAtomCore.ScrollHasReachedTheEnd ();

					browseAtomCore.get_Value (null, retValue);
					CDataTableX dataTable = new CDataTableX ();
					CDataTableServer dataTableServer = retValue.m_pObjVal as CDataTableServer;
					dataTableServer.Dettach (dataTable, true);

					foreach (DataRow row in dataTable.Rows)
					{
						foreach (DataColumn column in dataTable.Columns)
						{
							string strColumnName = column.ColumnName;
							object data = row[strColumnName];

							formDataManager.AddData (strColumnName, data);
						}
					}
				}
				else if (atomCore is ExtensionScrollAtom)
				{
					ExtensionScrollAtom scrollAtomCore = atomCore as ExtensionScrollAtom;

					scrollAtomCore.ScrollHasReachedTheEnd ();

					DataTable dataTable = scrollAtomCore.GetDataTable ();

					foreach (DataRow row in dataTable.Rows)
					{
						foreach (DataColumn column in dataTable.Columns)
						{
							string strColumnName = column.ColumnName;
							object data = row[strColumnName];

							formDataManager.AddData (strColumnName, data);
						}
					}
				}
				else if (atomCore is EBookAnimationGroupAtom)
				{
					var groupAtom = atomCore as EBookAnimationGroupAtom;
					foreach (Atom bindAtomCore in groupAtom.GroupAtomList)
					{
						string strBindAtomName = bindAtomCore.GetProperVar ();
						string strValue = bindAtomCore.GetContentString (true);

						formDataManager.AddData (strBindAtomName, strValue);
					}
				}
				else
				{
					string strValue = baseActionAtom.GetAtomAttribValue (strAtomName);
					formDataManager.AddData (strOuterName, strValue);
				}
			}

			GlobalEventReceiver.UniqueGlobalEventRecevier.NotifyExecuteReportModel (isPreView, strFilePath, formDataManager, strFileName);
		}

		public override Atom FindJoinAtom (Atom targetAtom)
		{
			LightJDoc document = m_pLightDMTView.Document;

			if (null == document)
				return null;

			List<Atom> atomCoreList = document.GetAllAtomCores ();
			List<MasterInputAttrib> scndAttribList = atomCoreList.Where (item => item.GetAttrib () is MasterInputAttrib).Select (item => item.GetAttrib () as MasterInputAttrib).ToList ();

			string targetTableName = targetAtom.GetDBInfo ((int)TABLE_INFO._TABLE_NAME_, false, false);

			if (null != scndAttribList)
			{
				foreach (Atom atomCore in atomCoreList)
				{
					MasterInputAttrib scndAttrib = atomCore.GetAttrib () as MasterInputAttrib;

					if (null == scndAttrib || null == scndAttrib.JoinDataManager)
						continue;

					ArrayList joinDataList = scndAttrib.JoinDataManager.GetJoinData ();

					if (null != joinDataList && 0 < joinDataList.Count)
					{
						foreach (JoinData joinData in joinDataList)
						{
							if (joinData.TableName == targetTableName)
							{
								return atomCore;
							}
						}
					}
				}
			}

			return null;
		}

		public override int GetLoadSQLIndex (Atom targetAtom)
		{
			if (null == targetAtom)
				return -1;

			if (-1 != targetAtom.SQLIndex)
				return targetAtom.SQLIndex;

			string targetTableName = targetAtom.GetDBInfo ((int)TABLE_INFO._TABLE_NAME_, false, false);

			LightJDoc document = m_pLightDMTView.Document;

			if (null == document)
				return -1;

			List<Atom> atomCoreList = document.GetAllAtomCores ();

			foreach (Atom atomCore in atomCoreList)
			{
				int sqlIndex = atomCore.SQLIndex;

				if (-1 == sqlIndex)
					continue;

				string strTable = atomCore.GetDBInfo ((int)TABLE_INFO._TABLE_NAME_, false, false);

				if (strTable == targetTableName)
				{
					return sqlIndex;
				}
			}

			return -1;
		}

		public override void ExecuteOptionEffect (Atom atomCore, bool isCheck)
		{
			LightJDoc document = m_pLightDMTView.Document;
			document.RootEBookQuestionsTouchManager.ExecuteOptionEffect (atomCore, atomCore, isCheck);
		}

		public override void CallMoveAtomCommand (object objValue)
		{
			if (objValue is MoveGroupedAtomsCommand moveGroupedAtomsCommand && m_pLightDMTView.Document is LightJDoc document)
			{
				document.Commander.AddCommand (moveGroupedAtomsCommand);
				document.Commander.ExecuteCommand ();
			}
		}

		public override void OnGrading (Atom atomCore)
		{
			var targetAtomName = atomCore.GetRefProperVar ();
			var document = m_pLightDMTView.GetDocument () as LightJDoc;

			if (false == string.IsNullOrEmpty (targetAtomName))
			{
				//특정 아톰 채점

				var targetAtom = document.GetProperVarAtom (targetAtomName);

				if (null != targetAtom)
				{
					ExecuteGradingAtom (targetAtom);
				}
				else
				{
					//아톰명이 아닌 학습정보 이벤트 명칭(LRS)인 경우 
					var targetList = document.GetLRSEventAtoms (targetAtomName);

					foreach (var target in targetList)
					{
						ExecuteGradingAtom (target);
					}
				}
			}
			else
			{
				//전체 채점
				document.RootEBookQuestionsTouchManager.OnGrading ("");
			}
		}

		private void ExecuteGradingAtom (Atom targetAtom)
		{
			var document = m_pLightDMTView.GetDocument () as LightJDoc;

			if (null != targetAtom)
			{
				if (targetAtom is EBookAnimationGroupAtom groupAtomCore)
				{
					foreach (var subAtom in groupAtomCore.GroupAtomList)
					{
						ExecuteGradingAtom (subAtom);
					}
				}
				else if (targetAtom is EBookQuestionsAtom eBookQuestionsAtom)
				{
					var ofAtom = eBookQuestionsAtom.GetOfAtom () as EBookQuestionsofAtom;

					document.RootEBookQuestionsTouchManager.IsScriptEffect = true;
					ofAtom.CheckAnswer ();
					document.RootEBookQuestionsTouchManager.IsScriptEffect = false;
				}
				else if (targetAtom is EBookQuizViewAtom quizViewAtom)
				{
					quizViewAtom.ExecuteAnswer ();
				}
				else if (targetAtom is WebModelAtom webModelCore)
				{
					var runPath = webModelCore.GetContentString (false);
					var extension = Path.GetExtension (runPath);

					if (webModelCore.ChildDoc is LightJDoc childDoc)
					{
						var quizAtom = WPFFindChildHelper.FindVisualChild<EBookQuizViewofAtom> (childDoc.GetParentView () as TopView) as EBookQuizViewofAtom;

						if (null != quizAtom && quizAtom.AtomCore is EBookQuizViewAtom quizAtomCore)
						{
							quizAtomCore.ExecuteAnswer ();
						}
					}
				}
				else
				{
					document.RootEBookQuestionsTouchManager.OnGrading (targetAtom.GetProperVar ());
				}
			}
		}

		#region | LRS | 

		public override void LRSProcessEvent (Atom atomCore)
		{
			if (false == PQAppBase.IsEduTechMode)
				return;

			var document = m_pLightDMTView.Document;
			var lrsManager = document.RootLRSManager;
			var frame = m_pLightDMTView.GetFrame () as LightDMTFrame;

			if (atomCore is EBookQuestionsAtom eBookQuestionsAtomCore)
			{
				//문제풀이 아톰의 경우..
				var isShowEffect = document.RootEBookQuestionsTouchManager.IsShowAnswerEffect (eBookQuestionsAtomCore);

				var pmInfos = document.GetPMInfoList (eBookQuestionsAtomCore);
				var lrsPMList = pmInfos.Where (item => item is PMLRSIndependentInfo).Select (item => item as PMLRSIndependentInfo).ToList ();

				foreach (var lrsPM in lrsPMList)
				{
					var node = lrsManager.GetNode (lrsPM.EventName);

					if (false == node.IsStart)
					{
						if (true == isShowEffect)
						{
							lrsManager.Stop (node.EventName);
						}
						else
						{
							lrsManager.Start (node.EventName);
						}
					}
					else
					{
						if (true == isShowEffect)
						{
							lrsManager.Stop (lrsPM.EventName);
						}
					}
				}
			}
			else
			{
				ProcessEvent (0, EVS_TYPE.EVS_A_LRS_CHANGED, atomCore.GetCurrentInfo (0, EVS_TYPE.EVS_A_LRS_CHANGED), null);
				MsgHandler.CALL_MSG_HANDLER (atomCore, EVS_TYPE.EVS_A_LRS_CHANGED, null);
				ProcessEvent (1, EVS_TYPE.EVS_A_LRS_CHANGED, atomCore.GetCurrentInfo (1, EVS_TYPE.EVS_A_LRS_CHANGED), null);
			}

			//LRS 이벤트가 발생한 아톰이 그룹묶기 아톰이 아니면서, 그룹묶기에 묶여있는경우
			if (false == atomCore is EBookAnimationGroupAtom)
			{
				var groupAtoms = document.GetAllAtomCores ().OfType<EBookAnimationGroupAtom> ().ToList ();
				if (null != groupAtoms && 0 < groupAtoms.Count)
				{
					var targetGroup = groupAtoms.Find (item => item.GroupAtomList.Contains (atomCore));

					if (null != targetGroup)
					{
						//group Check
						SetLRSGroup (targetGroup);
					}
				}
			}
		}

		private void SetLRSGroup (EBookAnimationGroupAtom targetGroup)
		{
			var document = m_pLightDMTView.Document;
			var frame = m_pLightDMTView.GetFrame ();
			var phoneScreenView = frame.CurrentBaseScreenView as PhoneScreenView;
			var eBookQuestionsArea = phoneScreenView.EBookQuestionsArea;

			int totalAnswerObjectCount = 0;
			int useAnswerObjectCount = 0;

			foreach (var atom in targetGroup.GroupAtomList)
			{
				var attrib = atom.GetAttrib ();
				var info = attrib.EBookQuestionsInfo;
				var atomName = atom.GetProperVar ();

				if (null != info && false == string.IsNullOrEmpty (info.AnswerValue))
				{
					totalAnswerObjectCount++;
				}

				if (true == eBookQuestionsArea.AnswerEffectControlMap.ContainsKey (atomName))
				{
					useAnswerObjectCount++;
				}
			}

			var pmInfos = document.GetPMInfoList (targetGroup);
			var lrsPMList = pmInfos.Where (item => item is PMLRSStartInfo || item is PMLRSIndependentInfo || item is PMLRSEndInfo).ToList ();
			var lrsManager = document.RootLRSManager;

			if (0 < totalAnswerObjectCount)
			{
				SetLRSQuestionsGroup (totalAnswerObjectCount, useAnswerObjectCount, lrsPMList);
			}
		}

		private void SetLRSQuestionsGroup (int totalAnswerObjectCount, int useAnswerObjectCount, List<PMInfo> lrsPMList)
		{
			var document = m_pLightDMTView.Document;
			var lrsManager = document.RootLRSManager;

			if (totalAnswerObjectCount == useAnswerObjectCount)
			{
				foreach (var item in lrsPMList)
				{
					if (item is PMLRSEndInfo endInfo)
					{
						lrsManager.Stop (endInfo.EventName);
						lrsManager.SetNodeContentCount (endInfo.EventName, useAnswerObjectCount, totalAnswerObjectCount);
					}
					else if (item is PMLRSIndependentInfo independentInfo)
					{
						lrsManager.Stop (independentInfo.EventName);
						lrsManager.SetNodeContentCount (independentInfo.EventName, useAnswerObjectCount, totalAnswerObjectCount);
					}
				}
			}
			else
			{
				//start
				foreach (var item in lrsPMList)
				{
					if (item is PMLRSStartInfo startInfo)
					{
						lrsManager.Start (startInfo.EventName, startInfo.EventType, startInfo.CurriculumCode, startInfo.OwnCourseCode);
						lrsManager.SetNodeContentCount (startInfo.EventName, useAnswerObjectCount, totalAnswerObjectCount);
					}
					else if (item is PMLRSIndependentInfo independentInfo)
					{
						lrsManager.Start (independentInfo.EventName, independentInfo.EventType, independentInfo.CurriculumCode, independentInfo.OwnCourseCode);
						lrsManager.SetNodeContentCount (independentInfo.EventName, useAnswerObjectCount, totalAnswerObjectCount);
					}
				}
			}
		}

		private PMLRSIndependentInfo GetLRSIndependentInfo (Atom atomCore)
		{
			var infos = m_pLightDMTView.Document.GetPMInfoList (atomCore);
			return infos.OfType<PMLRSIndependentInfo> ()?.FirstOrDefault (); ;
		}

		public override void LRSStartMedia (Atom atomCore)
		{
			if (false == PQAppBase.IsEduTechMode)
				return;

			var document = m_pLightDMTView.GetDocument () as LightJDoc;
			var info = GetLRSIndependentInfo (atomCore);

			if (null != info)
			{
				var node = document.RootLRSManager.GetNode (info.EventName);

				if (null != node)
				{
					if (true == node.IsCompleted)
						node.ReTryCount++;

					if (atomCore is WebFlashAtom flashAtomCore)
						node.NodeContentType = LRSNodeContentType.Media;
				}


				LRSProcessEvent (atomCore);
			}
		}

		public override void LRSOpenMedia (Atom atomCore)
		{
			if (false == PQAppBase.IsEduTechMode)
				return;

			var document = m_pLightDMTView.GetDocument () as LightJDoc;
			var info = GetLRSIndependentInfo (atomCore);
			var node = document.RootLRSManager.GetNode (info?.EventName);

			if (null != node)
			{
				if (atomCore is WebFlashAtom flashAtomCore)
				{
					node.NodeContentType = LRSNodeContentType.Media;

					if (flashAtomCore.GetOfAtom () is WebFlashofAtom ofAtom)
					{
						int totalContentCount = (int)ofAtom.GetMaxPosition ();
						if (0 < totalContentCount)
						{
							node.TotalContentCount = totalContentCount;
						}
					}
				}
			}
		}

		public override void LRSEndMedia (Atom atomCore)
		{
			if (false == PQAppBase.IsEduTechMode)
				return;

			var document = m_pLightDMTView.GetDocument () as LightJDoc;
			var info = GetLRSIndependentInfo (atomCore);
			var node = document.RootLRSManager.GetNode (info?.EventName);

			if (null != node)
			{
				node.IsCompleted = true;

				if (0 < node.ReTryCount)
				{
					node.ReTryTime += DateTime.Now - node.StartDateTime;
				}
			}

			LRSProcessEvent (atomCore);
		}

		public override void LRSPauseMedia (Atom atomCore)
		{
			if (false == PQAppBase.IsEduTechMode)
				return;

			var document = m_pLightDMTView.GetDocument () as LightJDoc;
			var info = GetLRSIndependentInfo (atomCore);

			var node = document.RootLRSManager.GetNode (info?.EventName);
			if (null != node)
			{
				if (false == node.AttributeMap.ContainsKey (LRSDataSet.videolearning_pause_count))
				{
					node.AttributeMap.Add (LRSDataSet.videolearning_pause_count, "1");
				}
				else
				{
					var count = _Kiss.toInt32 (node.AttributeMap[LRSDataSet.videolearning_pause_count]);
					node.AttributeMap[LRSDataSet.videolearning_pause_count] = (count + 1).ToString ();
				}

				if (0 < node.ReTryCount && true == node.IsStart)
				{
					node.ReTryTime += DateTime.Now - node.StartDateTime;
				}

				document.RootLRSManager.Stop (info?.EventName);
			}
		}

		public override void LRSSkipMedia (Atom atomCore)
		{
			if (false == PQAppBase.IsEduTechMode)
				return;

			var document = m_pLightDMTView.GetDocument () as LightJDoc;
			var info = GetLRSIndependentInfo (atomCore);

			var node = document.RootLRSManager.GetNode (info?.EventName);
			if (null != node)
			{
				if (false == node.AttributeMap.ContainsKey (LRSDataSet.videolearning_skip_count))
				{
					node.AttributeMap.Add (LRSDataSet.videolearning_skip_count, "1");
				}
				else
				{
					var count = _Kiss.toInt32 (node.AttributeMap[LRSDataSet.videolearning_skip_count]);
					node.AttributeMap[LRSDataSet.videolearning_skip_count] = (count + 1).ToString ();
				}
			}
		}

		public override void LRSMuteMedia (Atom atomCore)
		{
			if (false == PQAppBase.IsEduTechMode)
				return;

			var document = m_pLightDMTView.GetDocument () as LightJDoc;
			var info = GetLRSIndependentInfo (atomCore);

			var node = document.RootLRSManager.GetNode (info?.EventName);
			if (null != node)
			{
				if (false == node.AttributeMap.ContainsKey (LRSDataSet.videolearning_mutecount))
				{
					node.AttributeMap.Add (LRSDataSet.videolearning_mutecount, "1");
				}
				else
				{
					var count = _Kiss.toInt32 (node.AttributeMap[LRSDataSet.videolearning_mutecount]);
					node.AttributeMap[LRSDataSet.videolearning_mutecount] = (count + 1).ToString ();
				}
			}
		}

		#endregion

		public override void MakeQuizViewDefaultAtom (Atom atom)
		{
			var atomCore = atom as EBookQuizViewAtom;
			var ofAtom = atom?.GetOfAtom () as EBookQuizViewofAtom;
			var atomAttrib = atom?.GetAttrib () as EBookQuizViewAttrib;
			var document = m_pLightDMTView.Document;

			if (null == atomCore || null == atomAttrib || null == document)
				return;

			var oldAnswerCount = 0;
			var oldQuestionCount = 0;

			var answerCount = 0;
			var questionCount = 0;

			switch (atomAttrib.DisplayQuizType)
			{
				case QuizType.A11: // 선다형
					answerCount = atomAttrib.EBookQuizOptionNode.A11.AnswerCount;
					break;
				case QuizType.A21: // 선긋기
					questionCount = atomAttrib.EBookQuizOptionNode.A21.QuestionCount;
					answerCount = atomAttrib.EBookQuizOptionNode.A21.AnswerCount;
					break;
				case QuizType.A31: // 끌어놓기
					questionCount = atomAttrib.EBookQuizOptionNode.A31.QuestionCount;
					answerCount = atomAttrib.EBookQuizOptionNode.A31.AnswerCount;
					break;
				case QuizType.A41: // 빈칸채움
					questionCount = 0;
					answerCount = atomAttrib.EBookQuizOptionNode.A41.AnswerCount;
					break;
				case QuizType.A61:
					answerCount = atomAttrib.EBookQuizOptionNode.A61.AnswerCount;
					break;
				case QuizType.C11: // 단답형
					questionCount = 0;
					answerCount = atomAttrib.EBookQuizOptionNode.C11.AnswerCount;
					break;
				case QuizType.E11: // 그려넣기
					questionCount = 0;
					answerCount = atomAttrib.EBookQuizOptionNode.E11.AnswerCount;
					break;
			}

			foreach (var item in atomAttrib.DataMap)
			{
				if (item.Value.ActionType == QuizAction.Answer)
				{
					oldAnswerCount++;
				}
				else if (item.Value.ActionType == QuizAction.Question)
				{
					oldQuestionCount++;
				}
			}

			if (oldQuestionCount < questionCount)
			{
				EBookQuizViewMakeSubAtom (QuizAction.Question, atomCore, oldQuestionCount, questionCount);
			}
			else if (oldQuestionCount > questionCount)
			{
				EBookQuizViewRemoveSubAtom (QuizAction.Question, atomCore, oldQuestionCount, questionCount);
			}

			if (oldAnswerCount < answerCount)
			{
				EBookQuizViewMakeSubAtom (QuizAction.Answer, atomCore, oldAnswerCount, answerCount);
			}
			else if (oldAnswerCount > answerCount)
			{
				EBookQuizViewRemoveSubAtom (QuizAction.Answer, atomCore, oldAnswerCount, answerCount);
			}
		}

		public override string GetEBookQuizViewSignCode (Atom atomCore)
		{
			var quizViewAtomCore = atomCore as EBookQuizViewAtom;
			var document = m_pLightDMTView.Document;
			var quizViewList = document.GetAllAtomCores ().OfType<EBookQuizViewAtom> ().ToList ();
			var sign = Convert.ToChar (65 + quizViewList.IndexOf (quizViewAtomCore)).ToString ();

			return sign;
		}

		private void EBookQuizViewMakeSubAtom (QuizAction actionType, EBookQuizViewAtom atomCore, int start, int count)
		{
			var ofAtom = atomCore?.GetOfAtom () as EBookQuizViewofAtom;
			var atomAttrib = atomCore?.GetAttrib () as EBookQuizViewAttrib;

			if (null == ofAtom || null == atomAttrib) return;

			var document = m_pLightDMTView.Document;

			var sign = GetEBookQuizViewSignCode (atomCore);

			for (int i = start; i < count; i++)
			{
				var pt = GetQuizViewMakeAtomPosition (actionType, i, atomCore, count,
					out Size atomSize, out AtomType makeAtomType);

				if (makeAtomType == AtomType.None)
				{
					Trace.TraceInformation ("[Error] EBookQuizViewMakeSubAtom Atom None");
					continue;
				}

				var dx = pt.X;
				var dy = pt.Y;

				var newAtom = document.MakeAtomInTabView (ofAtom, makeAtomType,
					dx + ofAtom.Margin.Left, dy + ofAtom.Margin.Top);
				var title = actionType == QuizAction.Question ? "문항" : "답항";

				var newAtomName = $"{sign}_{title}" + (0 < i ? i.ToString () : "");
				newAtom.AtomCore.AtomProperVar = newAtomName;

				newAtom.Width = atomSize.Width;
				newAtom.Height = atomSize.Height;

				atomAttrib.DataMap.Add (newAtom.AtomCore, new EBookQuizPropertyNode
				{
					ActionType = actionType,
					QuizType = atomAttrib.DisplayQuizType,
					Name = newAtom.AtomCore.GetProperVar (),
				});
			}
		}

		private Point GetQuizViewMakeAtomPosition (QuizAction actionType, int index,
			EBookQuizViewAtom atomCore, int maxCount, out Size atomSize, out AtomType makeAtomType)
		{
			var dx = 0.0d;
			var dy = 0.0d;

			var ofAtom = atomCore.GetOfAtom () as EBookQuizViewofAtom;
			var atomAttrib = atomCore.GetAttrib () as EBookQuizViewAttrib;

			atomSize = DefaultAtomSizeManager.GetDefaultRect (AtomType.Square);

			makeAtomType = AtomType.None;

			if (atomAttrib.DisplayQuizType == QuizType.A21)
			{
				//선긋기
				makeAtomType = AtomType.Square;
				var directionType = atomAttrib.EBookQuizOptionNode.A21.DirectionType;

				if (0 == directionType)
				{
					var baseWidth = ofAtom.ActualWidth / 8;
					var baseHeight = ofAtom.ActualHeight / 3;

					var baseX = baseWidth;
					var baseY = 0;

					atomSize.Width = baseWidth * 6 / (1.5 * maxCount);

					var marginWidth = atomSize.Width / 2;

					dx = baseX + (index * atomSize.Width);

					if (0 < index)
					{
						dx += marginWidth * index - 1;
					}

					if (QuizAction.Question == actionType)
					{
						dy = baseHeight - atomSize.Height;
					}
					else if (QuizAction.Answer == actionType)
					{
						dy = baseHeight * 2 + atomSize.Height;
					}

				}
				else if (1 == directionType)
				{
					var questionAtomWidth = atomSize.Width;
					var answerAtomWidth = atomSize.Width;

					var marginHeight = atomSize.Height / 2;

					var baseWidth = ofAtom.ActualWidth - questionAtomWidth - answerAtomWidth;
					var baseHeight = ofAtom.ActualHeight - (atomSize.Height * maxCount + marginHeight * (maxCount - 1));

					var offsetX = baseWidth / 5;
					var offsetY = baseHeight / 2;

					if (QuizAction.Question == actionType)
					{
						dx = offsetX;
						dy = offsetY + (index * marginHeight + atomSize.Height * index);
					}
					else if (QuizAction.Answer == actionType)
					{
						dx = offsetX * 4 + questionAtomWidth;
						dy = offsetY + (index * marginHeight + atomSize.Height * index);
					}
				}
			}
			else if (atomAttrib.DisplayQuizType == QuizType.A31)
			{
				// 끌어놓기
				makeAtomType = AtomType.DecorImage;
				atomSize = DefaultAtomSizeManager.GetDefaultRect (makeAtomType);
				var directionType = atomAttrib.EBookQuizOptionNode.A31.DirectionType;

				if (0 == directionType)
				{
					var baseWidth = ofAtom.ActualWidth / 8;
					var baseHeight = ofAtom.ActualHeight / 4;

					var baseX = baseWidth;
					var baseY = 0;

					atomSize.Width = baseWidth * 0.8;
					atomSize.Height = baseHeight * 0.4;

					var marginWidth = atomSize.Width / 2;

					dx = baseX + (index * atomSize.Width);

					if (0 < index)
					{
						dx += marginWidth * index - 1;
					}

					if (QuizAction.Question == actionType)
					{
						dy = baseHeight - atomSize.Height;
					}
					else if (QuizAction.Answer == actionType)
					{
						dy = (baseHeight * 2) + atomSize.Height;
					}
				}
				else if (1 == directionType)
				{
					var baseWidth = ofAtom.ActualWidth / 8;
					var baseHeight = ofAtom.ActualHeight / 4;

					var baseX = baseWidth;
					var baseY = 0;

					atomSize.Width = baseWidth * 0.8;
					atomSize.Height = baseHeight * 0.4;

					var marginHeight = atomSize.Height / 2;

					var offsetX = baseWidth;
					var offsetY = baseHeight / 2;

					if (QuizAction.Question == actionType)
					{
						dx = offsetX;
						dy = offsetY + (index * marginHeight + atomSize.Height * index);
					}
					else if (QuizAction.Answer == actionType)
					{
						dx = offsetX * 4 + baseWidth;
						dy = offsetY + (index * marginHeight + atomSize.Height * index);
					}
				}
			}
			else if (atomAttrib.DisplayQuizType == QuizType.A41)
			{
				//빈칸채움
				makeAtomType = AtomType.DataInput;
				atomSize = DefaultAtomSizeManager.GetDefaultRect (makeAtomType);
				
				var questionAtomWidth = atomSize.Width;
				var answerAtomWidth = atomSize.Width;

				var marginHeight = atomSize.Height / 2;

				var baseWidth = ofAtom.ActualWidth - questionAtomWidth - answerAtomWidth;
				var baseHeight = ofAtom.ActualHeight - (atomSize.Height * maxCount + marginHeight * (maxCount - 1));

				var offsetX = baseWidth / 8;
				var offsetY = baseHeight / 2;

				dx = offsetX * 2 + questionAtomWidth;
				dy = offsetY + (index * marginHeight + atomSize.Height * index);
			}
			else if (atomAttrib.DisplayQuizType == QuizType.C11)
			{
				//단답형
				makeAtomType = AtomType.DataInput;
				atomSize = DefaultAtomSizeManager.GetDefaultRect (makeAtomType);
				//var directionType = atomAttrib.EBookQuizOptionNode.C11.DirectionType;

				//if (0 == directionType)
				//{
				//상하
				var questionAtomWidth = atomSize.Width;
				var answerAtomWidth = atomSize.Width;

				var marginHeight = atomSize.Height / 2;

				var baseWidth = ofAtom.ActualWidth - questionAtomWidth - answerAtomWidth;
				var baseHeight = ofAtom.ActualHeight - (atomSize.Height * maxCount + marginHeight * (maxCount - 1));

				var offsetX = baseWidth / 5;
				var offsetY = baseHeight / 2;

				dx = offsetX * 2 + questionAtomWidth;
				dy = offsetY + (index * marginHeight + atomSize.Height * index);
				//}
				//else if (1 == directionType)
				//{
				//	//좌우
				//	var baseWidth = ofAtom.ActualWidth / 8;
				//	var baseHeight = ofAtom.ActualHeight / 3;

				//	var baseX = baseWidth;
				//	var baseY = 0;

				//	atomSize.Width = baseWidth * 6 / (1.5 * maxCount);

				//	var marginWidth = atomSize.Width / 2;

				//	dx = baseX + (index * atomSize.Width);

				//	if (0 < index)
				//	{
				//		dx += marginWidth * index - 1;
				//	}

				//	dy = baseHeight - atomSize.Height;
				//}
			}
			else if (atomAttrib.DisplayQuizType == QuizType.E11)
			{
				//그려넣기
				makeAtomType = AtomType.Square;
				atomSize = DefaultAtomSizeManager.GetDefaultRect (makeAtomType);
				var directionType = atomAttrib.EBookQuizOptionNode.E11.DirectionType;

				if (0 == directionType)
				{
					//상하
					var questionAtomWidth = atomSize.Width;
					var answerAtomWidth = atomSize.Width;

					var marginHeight = atomSize.Height / 2;

					var baseWidth = ofAtom.ActualWidth - questionAtomWidth - answerAtomWidth;
					var baseHeight = ofAtom.ActualHeight - (atomSize.Height * maxCount + marginHeight * (maxCount - 1));

					var offsetX = baseWidth / 5;
					var offsetY = baseHeight / 2;

					dx = offsetX * 2 + questionAtomWidth;
					dy = offsetY + (index * marginHeight + atomSize.Height * index);
				}
				else if (1 == directionType)
				{
					//좌우
					var baseWidth = ofAtom.ActualWidth / 8;
					var baseHeight = ofAtom.ActualHeight / 3;

					var baseX = baseWidth;
					var baseY = 0;

					atomSize.Width = baseWidth * 6 / (1.5 * maxCount);

					var marginWidth = atomSize.Width / 2;

					dx = baseX + (index * atomSize.Width);

					if (0 < index)
					{
						dx += marginWidth * index - 1;
					}

					dy = baseHeight - atomSize.Height;
				}
			}
			else
			{
				makeAtomType = AtomType.Square;
				atomSize = DefaultAtomSizeManager.GetDefaultRect (makeAtomType);
				var directionType = atomAttrib.EBookQuizOptionNode.E11.DirectionType;

				if (0 == directionType)
				{
					//상하
					var questionAtomWidth = atomSize.Width;
					var answerAtomWidth = atomSize.Width;

					var marginHeight = atomSize.Height / 2;

					var baseWidth = ofAtom.ActualWidth - questionAtomWidth - answerAtomWidth;
					var baseHeight = ofAtom.ActualHeight - (atomSize.Height * maxCount + marginHeight * (maxCount - 1));

					var offsetX = baseWidth / 5;
					var offsetY = baseHeight / 2;

					dx = offsetX * 2 + questionAtomWidth;
					dy = offsetY + (index * marginHeight + atomSize.Height * index);
				}
				else if (1 == directionType)
				{
					//좌우
					var baseWidth = ofAtom.ActualWidth / 8;
					var baseHeight = ofAtom.ActualHeight / 3;

					var baseX = baseWidth;
					var baseY = 0;

					atomSize.Width = baseWidth * 6 / (1.5 * maxCount);

					var marginWidth = atomSize.Width / 2;

					dx = baseX + (index * atomSize.Width);

					if (0 < index)
					{
						dx += marginWidth * index - 1;
					}

					dy = baseHeight - atomSize.Height;
				}
			}

			return new Point (dx, dy);
		}

		/*
		private Point MakeOvalAtomOnLineEffect (int index, int maxCount, AtomType makeAtomType)
		{
			makeAtomType = AtomType.Square;
			var directionType = atomAttrib.EBookQuizOptionNode.A21.DirectionType;

			if (0 == directionType)
			{
				var baseWidth = ofAtom.ActualWidth / 8;
				var baseHeight = ofAtom.ActualHeight / 3;

				var baseX = baseWidth;
				var baseY = 0;

				atomSize.Width = baseWidth * 6 / (1.5 * maxCount);

				var marginWidth = atomSize.Width / 2;

				dx = baseX + (index * atomSize.Width);

				if (0 < index)
				{
					dx += marginWidth * index - 1;
				}

				if (QuizAction.Question == actionType)
				{
					dy = baseHeight - atomSize.Height;
				}
				else if (QuizAction.Answer == actionType)
				{
					dy = baseHeight * 2 + atomSize.Height;
				}

				MakeOvalAtomOnLineEffect (index, maxCount, makeAtomType);
			}

			return new Point (dx, dy);
		}
		*/


		private void EBookQuizViewRemoveSubAtom (QuizAction actionType, EBookQuizViewAtom atomCore, int start, int end)
		{
			var ofAtom = atomCore?.GetOfAtom () as EBookQuizViewofAtom;
			var atomAttrib = atomCore?.GetAttrib () as EBookQuizViewAttrib;

			if (null == ofAtom || null == atomAttrib) return;
			var document = m_pLightDMTView.Document;

			var sign = GetEBookQuizViewSignCode (atomCore);
			var atomList = new List<Atom> ();

			foreach (var item in atomAttrib.DataMap)
			{
				if (item.Value.ActionType == actionType)
				{
					item.Value.Name = item.Key.GetProperVar ();
					atomList.Add (item.Key);
				}
			}

			atomList = atomList.OrderBy (i => i.GetProperVar ()).ToList ();

			for (int i = end; i < start; i++)
			{
				if (0 < i)
				{
					var targetAtom = atomList[i - 1];
					var targetofAtom = targetAtom.GetOfAtom ();

					targetofAtom.AtomCore.Attrib.AtomProperVar = targetofAtom.AtomCore.Attrib.DefaultAtomProperVar;
					AdjustAtomProperVar (targetAtom);
					atomAttrib.DataMap.Remove (targetAtom);
				}
			}
		}

		public override void AdjustAtomProperVar (Atom atomCore)
		{
			var document = m_pLightDMTView.Document;

			if (null != atomCore)
			{
				document.AdjustAtomProperVar (atomCore.GetOfAtom ());
			}
		}

		/// <summary>
		/// 일반 아톰 -> 문항/답항 설정시 아톰명 변경
		/// 문항/답항 -> 일반으로 변경시 기본 아톰명으로 변경 
		/// </summary>
		public override void ChangeQuizBlockAtomName (Atom targetAtom)
		{
			var sign = "";
			var dataMap = new Dictionary<Atom, EBookQuizPropertyNode> ();
			var bindAtoms = new List<Atom> ();

			if (null != targetAtom)
			{
				//일반 퀴즈블록에서 호출되면 퀴즈블록 내부에 있는 아톰들에만 적용된다.
				var quizViewAtom = targetAtom as EBookQuizViewAtom;
				var quizViewAtomBase = quizViewAtom.AtomBase as EBookQuizViewAtomBase;
				var atomAttrib = quizViewAtom.Attrib as EBookQuizViewAttrib;
				dataMap = atomAttrib.DataMap;
				sign = GetEBookQuizViewSignCode (targetAtom);
				bindAtoms = quizViewAtomBase.GetAllAtomCores ();
			}
			else
			{
				//퀴즈메이커에서 호출되면 폼에 있는 전체 문항 / 답항을 기준으로 아톰명이 적용된다.
				var document = m_pLightDMTView.Document as LightJDoc;
				var metaData = document.PageMetadata;
				sign = "A";

				foreach (var item in metaData.QuizMetaData.DataList)
				{
					var atom = item.Key as Atom;
					dataMap.Add (atom, item.Value);
				}

				bindAtoms = document.GetAllAtomCores ();
			}

			var isAtomNameChanged = ChangeElementAtomName (sign, dataMap);
			isAtomNameChanged = isAtomNameChanged || ChangeBaseAtomName (sign, dataMap, bindAtoms);

			if (isAtomNameChanged)
			{
				if (null != targetAtom)
				{
					ToastMessge.Show ($"{targetAtom.AtomProperVar}에 문항/답항 아톰명칭이 자동으로 변경되었습니다");
				}
				else
				{
					ToastMessge.Show ($"문항/답항 아톰명칭이 자동으로 변경되었습니다");
				}
			}
		}

		/// <summary>
		/// 답항, 문항으로 설정된 모든 아톰명이 기본 아톰명, 또는 자동설정 아톰명인 경우 추가되는 모든 아톰들에 대해서 자동으로 명칭을 변경해주는 기능
		/// A_문항1 ~ A_문항N
		/// A_답항1 ~ A_답항N
		/// </summary>
		private bool ChangeElementAtomName (string sign, Dictionary<Atom, EBookQuizPropertyNode> dataMap)
		{
			var answerAtomList = new List<Atom> ();
			var questionAtomList = new List<Atom> ();
			var docType = this.m_pLightDMTView.Document.DocType;

			var isAtomNameChanged = false;
			var isDefaultAtomName = GetQuizViewDefaultAtomName (sign, dataMap);

			if (false == isDefaultAtomName)
				return isAtomNameChanged;

			foreach (var item in dataMap)
			{
				if (item.Value.ActionType == QuizAction.Question)
				{
					questionAtomList.Add (item.Key);
				}
				else if (item.Value.ActionType == QuizAction.Answer)
				{
					answerAtomList.Add (item.Key);
				}
			}

			for (int i = 0; i < questionAtomList.Count; i++)
			{
				var targetAtom = questionAtomList[i];
				var targetName = targetAtom.GetProperVar ();

				var autoName = docType != DOC_KIND._docQuizMaker ? $"{sign}_문항{i + 1}" : $"문항{i + 1}";

				if (!targetName.Equals (autoName))
				{
					var targetattrib = targetAtom.GetAttrib ();
					targetattrib.AtomProperVar = autoName;
					isAtomNameChanged = true;
				}
			}

			for (int i = 0; i < answerAtomList.Count; i++)
			{
				var targetAtom = answerAtomList[i];
				var targetName = targetAtom.GetProperVar ();

				var autoName = docType != DOC_KIND._docQuizMaker ? $"{sign}_답항{i + 1}" : $"답항{i + 1}";

				if (!targetName.Equals (autoName))
				{
					targetAtom.GetOfAtom ().AtomCore.AtomProperVar = autoName;
					isAtomNameChanged = true;
				}
			}

			return isAtomNameChanged;
		}

		public bool GetQuizViewDefaultAtomName (string sign, Dictionary<Atom, EBookQuizPropertyNode> dataMap)
		{
			var isDefaultAtomName = true;
			var docType = this.m_pLightDMTView.Document.DocType;

			foreach (var item in dataMap)
			{
				var targetAtom = item.Key;
				var defaultName = targetAtom.GetDefaultProperVar ();
				var atomName = targetAtom.GetProperVar ();
				var autoName = "";

				if (item.Value.ActionType == QuizAction.Question)
				{
					autoName = docType != DOC_KIND._docQuizMaker ? $"{sign}_문항" : "문항";
				}
				else if (item.Value.ActionType == QuizAction.Answer)
				{
					autoName = docType != DOC_KIND._docQuizMaker ? $"{sign}_답항" : "답항";
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

		/// <summary>
		/// 문항, 답항외 나머지 아톰들중 A_문항, A_답항 형식에 아톰명 사용시 기본 아톰명으로 변경하는 논리 보강
		/// </summary>
		private bool ChangeBaseAtomName (string sign, Dictionary<Atom, EBookQuizPropertyNode> dataMap, List<Atom> bindAtoms)
		{
			var isChanged = false;
			var docType = this.m_pLightDMTView.Document.DocType;

			foreach (var targetAtom in bindAtoms)
			{
				if (true == dataMap.ContainsKey (targetAtom))
				{
					var actionType = dataMap[targetAtom].ActionType;

					if (actionType == QuizAction.Question || actionType == QuizAction.Answer)
						continue;
				}

				var targetName = targetAtom.GetProperVar ();

				if (docType == DOC_KIND._docQuizMaker)
				{
					//QQM에서 동작할때 문항, 답항으로만 설정됨
					targetName = targetName.Replace ($"문항", "");
					targetName = targetName.Replace ($"답항", "");
				}
				else
				{
					targetName = targetName.Replace ($"{sign}_문항", "");
					targetName = targetName.Replace ($"{sign}_답항", "");
				}

				if (false == string.IsNullOrEmpty (targetName) && int.TryParse (targetName, out int num))
				{
					var targetAttrib = targetAtom.GetAttrib ();
					var targetofAtom = targetAtom.GetOfAtom ();
					targetAttrib.AtomProperVar = targetofAtom.AtomCore.Attrib.DefaultAtomProperVar;
					AdjustAtomProperVar (targetAtom);
					isChanged = true;
				}
			}

			return isChanged;
		}

		public override Size GetFormSize ()
		{
			if (null != m_pLightDMTView)
			{
				var document = m_pLightDMTView.GetDocument () as LightJDoc;
				var frameAttrib = document.GetFrameAttrib ();
				var size = new Size (frameAttrib.FrameSize.Width, frameAttrib.FrameSize.Height);
				return size;
			}

			return new Size ();
		}

		public override DOC_KIND GetDocKind ()
		{
			if (null != m_pLightDMTView?.Document)
				return m_pLightDMTView.Document.DocType;

			return DOC_KIND._docNone;
		}

		public override QuizType GetQuizType ()
		{
			var document = m_pLightDMTView?.Document;

			if (null != document?.PageMetadata?.QuizMetaData)
			{
				return document.PageMetadata.QuizMetaData.QuizType;
			}

			return QuizType.None;
		}

		public override void SaveQuizViewAtomForQQM (EBookQuizViewAtom atom)
		{
			var document = m_pLightDMTView.Document;

			if (null != document && null != atom)
			{
				document.SaveQuizViewAtomForQQM (atom);
			}
		}
	}
}
