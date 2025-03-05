using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.DelegateEventResource;
using Softpower.SmartMaker.TopApp.CommonLib.Interface;
using Softpower.SmartMaker.TopApp.FormInformation;
using Softpower.SmartMaker.TopApp.License;
using Softpower.SmartMaker.TopApp.Metadata;

namespace Softpower.SmartMaker.TopApp
{
	public class GlobalEventReceiver
	{
		#region 멤버변수
		private static GlobalEventReceiver m_GlobalEventReceiver = new GlobalEventReceiver ();
		private List<KeyValuePair<object, object>> m_lstParameterList = new List<KeyValuePair<object, object>> ();
		#endregion

		#region 프로퍼티
		public static GlobalEventReceiver UniqueGlobalEventRecevier
		{
			get
			{
				if (null == m_GlobalEventReceiver)
				{
					m_GlobalEventReceiver = new GlobalEventReceiver ();
				}

				return m_GlobalEventReceiver;
			}
		}
		#endregion

		#region 이벤트

		public event CommonDelegateEvents.OnNotifyKeyValueListAndReturnArrayListEventHandler OnNotifyGlobalEventOccured = null;

		#endregion

		public GlobalEventReceiver ()
		{

		}

		public void ApprovalViewClose ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ApprovalViewClose, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void ApprovalComplete (ArrayList ApprovalList)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ApprovalComplete, ApprovalList));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void ShowSmartApprovalView (Hashtable AtomAttrib)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ShowSmartApprovalView, AtomAttrib));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public ArrayList GetRecentFileList ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.GetRecentFileList, null));
				return OnNotifyGlobalEventOccured (m_lstParameterList);
			}

			return null;
		}

		public void NotifyRecentMenuItemClicked (string strPath)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.RecentFileClicked, strPath));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyRecentMenuItemRemove (string strPath)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.RecentFileRemove, strPath));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyRecentMenuItemFolder (string strPath)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.RecentFileFolder, strPath));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyActiveModelFile (int nHashKey)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ActiveModelFile, nHashKey));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyMainMenuItemClicked (string strKey)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.MainMenuClicked, strKey));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyInvalidateAll ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.InvalidateAll, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyServerConnectionChanged (bool bIsSuccess)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ServerConnectionChanged, bIsSuccess));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyFrameStateChanged (object SourceFrame)
		{
			if (null != SourceFrame)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.FrameStateChanged, SourceFrame));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SourceFrameMode">DMTFrame</param>
		public void NotifyFrameModeChanged (object SourceFrame)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.FrameModeChanged, SourceFrame));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyCloseAllPopup ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.CloseAllPopup, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyCloseAtomAttWindow ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.CloseAtomAttWindow, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public ArrayList NotifyMakeAtom (AtomType SourceAtomType, bool bIsImmediately, bool bIsStackHistory)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				// 1. 시험사아톰 사용여부 체크 
				// 2. 라이선스에 따른 아톰 사용여부 체크

				int nCheckEnable = LicenseHelper.Instance.IsEnableSolutionTrialAtom (SourceAtomType);

				if (0 == nCheckEnable || LicenseHelper.Instance.IsEnableSolutionService (SourceAtomType, nCheckEnable))
				{
					m_lstParameterList.Clear ();

					List<bool> MakeAtomParameter = new List<bool> () { bIsImmediately, bIsStackHistory };
					KeyValuePair<AtomType, List<bool>> ParameterPackage = new KeyValuePair<AtomType, List<bool>> (SourceAtomType, MakeAtomParameter);
					m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.MakeAtom, ParameterPackage));
					return OnNotifyGlobalEventOccured (m_lstParameterList);
				}
			}

			return null;
		}

		public void NotifyWorkScriptMenu (string strWorkKey)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.WorkScriptMenu, strWorkKey));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyClosedScriptFrame ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ClosedScriptFrame, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyWorkExecuteMenu (string strWorkKey)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.WorkExecuteMenu, strWorkKey));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyClosedMakeExecuteMenuFrame ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.CloseMakeExecuteMenuFrame, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyCloseDesignHelper ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.CloseDesignHelper, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyApplyDesignHelperImage (DictionaryEntry DictionaryParameter)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ApplyDesignHelperImage, DictionaryParameter));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public IFlexibleWindowBehavior GetFlexibleWindowAboutThisType (FlexibleDialogDefine.FlexibleDialogType DialogType)
		{
			WindowCollection SubWindows = Application.Current.MainWindow.OwnedWindows;
			return GetFlexibleWindowAboutThisType (DialogType, SubWindows);
		}

		private IFlexibleWindowBehavior GetFlexibleWindowAboutThisType (FlexibleDialogDefine.FlexibleDialogType DialogType, WindowCollection OwnedWindows)
		{
			if (null != OwnedWindows)
			{
				int nCountOfWindows = OwnedWindows.Count;

				for (int nIndex = 0; nIndex < nCountOfWindows; nIndex++)
				{
					IFlexibleWindowBehavior CurrentWindow = OwnedWindows[nIndex] as IFlexibleWindowBehavior;

					if (null != CurrentWindow)
					{
						FlexibleDialogDefine.FlexibleDialogType CurrentWindowType = CurrentWindow.GetFlexibleDialogType ();

						if (CurrentWindowType == DialogType)
						{
							return CurrentWindow;
						}

						WindowCollection SubWindow = CurrentWindow.GetOwnedWindows ();
						GetFlexibleWindowAboutThisType (DialogType, SubWindow);
					}
				}
			}

			return null;
		}

		public void NotifyAtomExpandContainerChanged (MainExpandMenuType enumMainExpandMenuContainerType)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.AtomExpandContainerChanged, enumMainExpandMenuContainerType));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyCurrentLocationAndSize (Thickness SourceMargin, double dSourceWidth, double dSourceHeight)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				List<double> LocationAndSizeList = new List<double> ();
				LocationAndSizeList.Add (SourceMargin.Left);
				LocationAndSizeList.Add (SourceMargin.Top);
				LocationAndSizeList.Add (dSourceWidth);
				LocationAndSizeList.Add (dSourceHeight);
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.NotifyLocationAndSize, LocationAndSizeList));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyCurrentLocationAndSize (Thickness SourceMargin, double dSourceWidth, double dSourceHeight, double dSourceRound)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				List<double> LocationAndSizeList = new List<double> ();
				LocationAndSizeList.Add (SourceMargin.Left);
				LocationAndSizeList.Add (SourceMargin.Top);
				LocationAndSizeList.Add (dSourceWidth);
				LocationAndSizeList.Add (dSourceHeight);
				LocationAndSizeList.Add (dSourceRound);
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.NotifyLocationAndSize, LocationAndSizeList));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyCurrentLocation (Thickness SourceMargin)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.NotifyLocation, SourceMargin));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyCurrentServerIconClicked (bool bValue)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.NotifyStatusBarServerIconClicked, bValue));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyUpgradeStatusClicked (bool bValue)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.NotifyUpgradeStatusClicked, bValue));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyCurrentLocationAndSizeChanged (LocationAndSizeDefine.LocationAndSizeType SourceType, double dValue)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				KeyValuePair<LocationAndSizeDefine.LocationAndSizeType, double> ValueList = new KeyValuePair<LocationAndSizeDefine.LocationAndSizeType, double> (SourceType, dValue);
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.NotifyLocationAndSizeChanged, ValueList));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyAtomZIndexChanged (ChangeAtomZIndexDefine.ActionType ActionType)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.AtomZIndexChanged, ActionType));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyFontSizeChanged (double dFontSize)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.FontSizeChanged, dFontSize));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyShortCutClicked (int nShortCutType)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ShortCutClicked, nShortCutType));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyArduinoShortCutClicked (int nShortCutType)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ArduinoShortCutClicked, nShortCutType));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyMakeNewModel (object objModelDescription)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.MakeNewModel, objModelDescription));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyLinkMakeStore (object objModelDescription)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.LinkMakeStore, objModelDescription));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyAtomSelectChanged (ICommandBehavior SourceCommand)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.AtomSelectChanged, SourceCommand));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifySaveModel ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.SaveModel, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifySaveAsModel ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.SaveAsModel, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyOpenQuizMakerModel ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.OpenQuizMakerModel, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyShowDefaultIntervalDialog ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ShowDefaultIntervalDialog, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public ArrayList GetCurrentSelectedAtoms ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.GetCurrentSelectedAtoms, null));
				return OnNotifyGlobalEventOccured (m_lstParameterList);
			}

			return null;
		}

		public bool NotifyChangeAtomNames (ArrayList lstApplyAtoms, string strApplyAtomName)
		{
			bool bResult = false;

			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				KeyValuePair<ArrayList, string> Parameter = new KeyValuePair<ArrayList, string> (lstApplyAtoms, strApplyAtomName);
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.SetAtomNames, Parameter));
				ArrayList alResult = OnNotifyGlobalEventOccured (m_lstParameterList);

				if (null != alResult && 1 == alResult.Count)
				{
					bResult = (bool)alResult[0];
				}
			}

			return bResult;
		}


		public void NotifyChangeAtomFieldType (ArrayList lstApplyAtoms, string strApplyAtomFieldType)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				KeyValuePair<ArrayList, string> Parameter = new KeyValuePair<ArrayList, string> (lstApplyAtoms, strApplyAtomFieldType);
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.SetAtomFieldType, Parameter));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyWindowStateTypeChanged (WindowStateType ChangedType)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.NotifyWindowStateType, ChangedType));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyCloseMainWindow ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.CloseMainWindow, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public WindowStateType GetCurrentWindowStateType ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.GetWindowStateType, null));
				ArrayList objCurrentWindowState = OnNotifyGlobalEventOccured (m_lstParameterList);

				if (null != objCurrentWindowState)
				{
					return (WindowStateType)objCurrentWindowState[0];
				}
			}

			return WindowStateType.None;
		}

		public void NotifyShowFlowMapDialog ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ShowFlowMap, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void ShowAttPageInGridTableAtom (object objAtom, string strKeyword)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				object[] dataList = new object[2];
				dataList[0] = objAtom;
				dataList[1] = strKeyword;
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ShowAttPageInGridTableAtom, dataList));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public ArrayList NotifyLoadModelFromInnerLogic (string strFilePath)
		{
			m_lstParameterList.Clear ();
			m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.LoadModelFromInnerLogic, strFilePath));

			return OnNotifyGlobalEventOccured (m_lstParameterList);
		}

		public ArrayList NotifyLoadModelFromInnerLogic (string strFilePath, int nBookPage)
		{
			m_lstParameterList.Clear ();

			object[] dataList = new object[2];
			dataList[0] = strFilePath;
			dataList[1] = nBookPage;

			m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.LoadModelFromBookPage, dataList));

			return OnNotifyGlobalEventOccured (m_lstParameterList);
		}

		public ArrayList NotifyLoadModelFromInnerLogic (string strFilePath, int nBookPage, bool DoModal)
		{
			m_lstParameterList.Clear ();

			object[] dataList = new object[3];
			dataList[0] = strFilePath;
			dataList[1] = nBookPage;
			dataList[2] = DoModal;

			m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.LoadModelFromBookPage, dataList));

			return OnNotifyGlobalEventOccured (m_lstParameterList);
		}

		/// <summary>
		/// MakerStore 에서 상세폼 띄울대 동작
		/// </summary>
		/// <param name="strFilePath"></param>
		/// <returns></returns>
		public ArrayList NotifyLoadModelFromMakerStore (string strFilePath)
		{
			m_lstParameterList.Clear ();
			m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.LoadModelFromMakerStore, strFilePath));
			return OnNotifyGlobalEventOccured (m_lstParameterList);
		}

		/// <summary>
		/// Simulator 에서 상세폼 띄울대 동작
		/// </summary>
		/// <param name="strFilePath"></param>
		/// <returns></returns>
		public ArrayList NotifyLoadModelFromSimulator (string strFilePath)
		{
			m_lstParameterList.Clear ();
			m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.LoadModelFromSimulator, strFilePath));
			return OnNotifyGlobalEventOccured (m_lstParameterList);
		}

		/// <summary>
		/// Simulator 에서 최상위 폼 띄울대 동작
		/// </summary>
		/// <param name="strFilePath"></param>
		/// <returns></returns>
		public ArrayList NotifyLoadTopMostModelFromSimulator (string strFilePath)
		{
			m_lstParameterList.Clear ();
			m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.LaodTopMostModelFromSimulator, strFilePath));
			return OnNotifyGlobalEventOccured (m_lstParameterList);
		}

		//2014-11-19-M01 글자툴바 재정비 및 현재 글꼴 저장
		public void NotifyToolBarFontFamilyAddSelectFont ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ToolBarFontFamilyAddSelectFont, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		//2014-11-25-M01 EBookToolBarEvent 처리
		public void NotifyEBookToolBarHandleCommonEvent (DelegateStructType.EventSourceType EventSourceType, int nEventKey, object value)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				object[] dataList = new object[3];
				dataList[0] = EventSourceType;
				dataList[1] = nEventKey;
				dataList[2] = value;
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.EBookToolBarHandleCommonEvent, dataList));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		//2014-11-27-M02 현재 DMTView에 포커스 주기 ( 포커스 아웃 )
		public void NotifyCurrentDMTViewFocusedEvent (DependencyObject scope)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.CurrentDMTViewFocusedEvent, scope));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		//2020-02-19 kys 기준장치설정 Bar 높이 변경
		public void NotifyBarHeightChange (int nTopBarHeight, int nBottomBarHeight)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				List<int> pList = new List<int> ();
				pList.Add (nTopBarHeight);
				pList.Add (nBottomBarHeight);

				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.BarHeightChange, pList));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyShowLinkHelp ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ShowLinkHelp, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyFileOpen (string strFilePath)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.FileOpen, strFilePath));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyCreateResourceFile (string strFilePath)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.CreateResourceFile, strFilePath));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyWebDeployment (string strProjectName, bool bReDeploy)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				ArrayList array = new ArrayList ();
				array.Add (strProjectName);
				array.Add (bReDeploy);

				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.WebDeployment, array));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyRegeneration (string strProjectName)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				ArrayList array = new ArrayList ();
				array.Add (strProjectName);

				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.WebRegeneration, array));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyExecuteReportModel (bool isPreView, string strReportModel, FormDataManager formDataManager, string strFileName)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				List<object> list = new List<object> ();
				list.Add (isPreView);
				list.Add (strReportModel);
				list.Add (formDataManager);
				list.Add (strFileName);

				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ExecuteReportModel, list));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyTabIndexButton (bool IsCellIndex)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.NotifyTabIndexButton, IsCellIndex));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyToggleFrameMode ()
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ToggleFrameMode, null));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifySapLaunchpad (string strTitle, string strServiceURL)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				ArrayList list = new ArrayList ();
				list.Add (strTitle);
				list.Add (strServiceURL);

				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.SapLaunchpad, list));

				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void NotifyMakeSlideMasterModel (object objSlideMasterViewModel)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.MakeSlideMaster, objSlideMasterViewModel));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void ShowAtomAttPage (object AtomCore, bool IsWeb, bool IsEBook, string ContextItem)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				ArrayList list = new ArrayList ();
				list.Add (AtomCore);
				list.Add (IsWeb);
				list.Add (IsEBook);
				list.Add (ContextItem);

				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ShowAtomAttPage, list));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

		public void ClearParamList ()
		{
			if (null != m_lstParameterList)
			{
				m_lstParameterList.Clear ();
			}
		}

		public ArrayList DeployQuizMaker (string quizCode, object document)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				ArrayList list = new ArrayList ();
				list.Add (quizCode);
				list.Add (document);

				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.DeployQuizMaker, list));
				return OnNotifyGlobalEventOccured (m_lstParameterList);
			}

			return null;
		}

		public void ShowQuizMakerPopup (string menuName)
		{
			if (null != OnNotifyGlobalEventOccured)
			{
				m_lstParameterList.Clear ();
				m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ShowQuizMakerPopup, menuName));
				OnNotifyGlobalEventOccured (m_lstParameterList);
			}
		}

        public void ChangeCurrentAtomFont (string FontName)
        {
            if (null != OnNotifyGlobalEventOccured)
            {
                m_lstParameterList.Clear ();
                m_lstParameterList.Add (new KeyValuePair<object, object> (DelegateEventKeys.MainWindowReceiverEventKey.ChangeAtomFont, FontName));
                OnNotifyGlobalEventOccured (m_lstParameterList);
            }
        }

		// CallBack 함수가 필요한 경우 사용
        public void NotifyMakeNewQuizBlockModel(object objModelDescription, Action callback)
        {
            if (null != OnNotifyGlobalEventOccured)
            {
                m_lstParameterList.Clear();
                m_lstParameterList.Add(new KeyValuePair<object, object>(DelegateEventKeys.MainWindowReceiverEventKey.MakeNewQuizBlockModel, objModelDescription));
                OnNotifyGlobalEventOccured(m_lstParameterList);

                callback?.Invoke();  // 콜백이 null이 아닐 경우 호출
            }
        }

        public void NotifyMakeNewQuizBlockModel(object objModelDescription, PageMetadata pageMetadata)
        {
            if (null != OnNotifyGlobalEventOccured)
            {
                ArrayList list = new ArrayList();
				list.Add(objModelDescription);
				list.Add(pageMetadata);

                m_lstParameterList.Clear();
                m_lstParameterList.Add(new KeyValuePair<object, object>(DelegateEventKeys.MainWindowReceiverEventKey.MakeNewQuizBlockModel, list));
                OnNotifyGlobalEventOccured(m_lstParameterList);
            }
        }
    }
}
