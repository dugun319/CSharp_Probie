namespace Softpower.SmartMaker.DelegateEventResource
{
	//프로그램에 사용되는 사용자이벤트에 대해 키 값을 부여
	//향 후 추가될 이벤트에 대비하여 각 모듈별로 100의 차이로 구분
	public class DelegateEventKeys
	{
		#region MainToolBar그룹
		public enum MainToolBarEventKey
		{
			RUN = 0,
			SERVERCONNECTION = 1,
			SELECTION = 2,
			LOCK = 3,
			LOADFILE = 4,
			SAVEFILE = 5,
			EMULATOR = 6
		}
		#endregion

		#region EditToolBar그룹
		public enum EditToolBarEventKey
		{
			COPY = 100,
			PASTE = 101,
			SPUID = 102,
			ATOMORDER = 103,
			RULER = 104,
			GRIDLINE = 105
		}
		#endregion

		#region ObjectPropertyToolBar그룹
		public enum ObjectPropertyEventKey
		{
			FONT = 200,
			FONTSIZE = 201,
			BOLD = 202,
			ITALIC = 203,
			UNDERLINE = 204,
			LEFTALIGN = 205,
			HORIZONTALMIDDLEALIGN = 206,
			RIGHTALIGN = 207,
			TOPALIGN = 208,
			VERTICALMIDDLEALIGN = 209,
			BOTTOMALIGN = 210,
			FONTCOLOR = 211,
			LINECOLOR = 212,
			BACKGROUNDCOLOR = 213,
			OBJECTALIGNMENT = 214,
			//ATOMNAME = 215,
			SHOWHIDE = 216,
			JUSTIFYALIGN = 217
		}

		public enum ColorDialogEventKey
		{
			BRUSH = 250,
			NOLINE = 251,
			LINETHICKNESS = 252,
			LINEDASHARRAY = 253,
			NOBACKGROUND = 254,
			BORDERVISIBILITYTYPE = 255,
			OPACITY = 256,
			SHADOW = 257,
			EXPANDAREA = 258,
		}
		#endregion

		#region Global(MainWindowReceiver) 그룹
		public enum MainWindowReceiverEventKey
		{
			MainMenuClicked = 1000,
			InvalidateAll = 1001,
			ServerConnectionChanged = 1002,
			FrameStateChanged = 1003,
			RecentFileClicked = 1004,
			GetRecentFileList = 1005,
			CloseAllPopup = 1006,
			MakeAtom = 1007,
			WorkScriptMenu = 1008,
			ClosedScriptFrame = 1009,
			CloseDesignHelper = 1010,
			ApplyDesignHelperImage = 1011,
			AtomExpandContainerChanged = 1012,
			NotifyLocationAndSize = 1013,
			NotifyLocation = 1014,
			NotifyLocationAndSizeChanged = 1015,
			AtomZIndexChanged = 1016,
			FontSizeChanged = 1017,
			ShortCutClicked = 1018,
			MakeNewModel = 1019,
			FrameModeChanged = 1020,
			AtomSelectChanged = 1021,
			NotifyCommandRecord = 1022,
			SaveModel = 1023,
			ShowDefaultIntervalDialog = 1024,
			GetCurrentSelectedAtoms = 1025,
			SetAtomNames = 1026,
			NotifyWindowStateType = 1027,
			GetWindowStateType = 1028,
			ShowSmartApprovalView = 1031,
			ApprovalComplete = 1032,
			ApprovalViewClose = 1033,
			ShowFlowMap = 1034,
			ShowAttPageInGridTableAtom = 1035,
			WorkExecuteMenu = 1036,
			CloseMakeExecuteMenuFrame = 1037,
			CloseMainWindow = 1038,
			LoadModelFromInnerLogic = 1039,
			ToolBarFontFamilyAddSelectFont = 1040,
			EBookToolBarHandleCommonEvent = 1041,
			CurrentDMTViewFocusedEvent = 1042,
			SetAtomFieldType = 1043,
			NotifyStatusBarServerIconClicked = 1044,
			ArduinoShortCutClicked = 1045,
			LoadModelFromMakerStore = 1046,
			LoadModelFromBookPage = 1047,
			LinkMakeStore = 1048,
			BarHeightChange = 1049,
			ShowLinkHelp = 1050,
			FileOpen = 1051,
			ShowAtomEditMap = 1052,
			CloseAtomAttWindow = 1053,
			CreateResourceFile = 1054,
			NotifyUpgradeStatusClicked = 1055,
			WebDeployment = 1056,
			WebRegeneration = 1057,
			LoadModelFromSimulator = 1058,
			LaodTopMostModelFromSimulator = 1059, //최상위 실행
			RecentFileRemove = 1060,
			ExecuteReportModel = 1061,
			NotifyTabIndexButton = 1062,
			ToggleFrameMode = 1063,
			SapLaunchpad = 1064,
			MakeSlideMaster = 1065,
			RecentFileFolder = 1066,
			ActiveModelFile = 1067,
			ShowAtomAttPage = 1068,
			DeployQuizMaker = 1069,
			ShowQuizMakerPopup = 1070,
			OpenQuizMakerModel = 1071,
			SaveAsModel = 1072,
            ChangeAtomFont = 1073,
            MakeNewQuizBlockModel = 1074,
        }
		#endregion

	}
}
