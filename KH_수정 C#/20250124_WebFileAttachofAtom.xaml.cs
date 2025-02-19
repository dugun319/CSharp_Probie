using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.DBCoreX.Common;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.StyleResourceDictionary;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.JsonServer;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopSmartAtom.AtomImages;
using Softpower.SmartMaker.TopWebAtom.Component;
using Softpower.SmartMaker.TopWebAtom.Data;

namespace Softpower.SmartMaker.TopWebAtom
{
	/// <summary>
	/// Interaction logic for FileAttachofAtom.xaml`
	/// </summary>
	public partial class WebFileAttachofAtom : WebFileAttachAtomBase
	{
		private class RowItem
		{
			private object m_objTag;

			public object Tag
			{
				get
				{
					return m_objTag;
				}
				set
				{
					m_objTag = value;
				}
			}

			public RowItem ()
			{
			}

			public RowItem (ImageSource image1, string str2)
			{
				this.col1 = image1;
				this.col2 = str2;
			}

			public ImageSource col1 { set; get; }
			public string col2 { set; get; }

		}

		private BitmapImage m_imageButton = null;
		private BitmapImage m_imageRollover = null;

		private IconImage iconList = new IconImage ();
		public Hashtable m_mapLocalFilePath = new Hashtable ();
		protected CStringArray m_Contents = null;   // 2009.02.13 이정대, 멀티다운로드
		public event EventHandler OpenFile;

		public WebFileAttachofAtom ()
		{
			InitializeComponent ();
			FlleAttachControl ();
		}

		public WebFileAttachofAtom (Atom atomCore)
			: base (atomCore)
		{
			InitializeComponent ();
			FlleAttachControl ();
		}

		protected override void InitializeAtomCore ()
		{
			m_AtomCore = new WebFileAttachAtom ();
		}

		protected override void InitializeResizeAdorner ()
		{
			m_ResizeAdorner = new Point8Adorner (this);
		}

		protected override void InitializeAtomSize ()
		{
			Size atomSize = DefaultAtomSizeManager.GetDefaultRect (AtomType.WebFileAttach);
			this.Width = atomSize.Width;
			this.Height = atomSize.Height;
		}

		protected override Size ArrangeOverride (Size arrangeBounds)
		{
			//if (false == string.IsNullOrEmpty(ButtonImagePath))
			//{
			//    string strImagePath = ButtonImagePath;
			//    strImagePath = PathHandler.GetFullPath(PQAppBase.DefaultPath, strImagePath);

			//    if (true == File.Exists(strImagePath))
			//    {
			//        FileInfo ImgFile = new FileInfo(strImagePath);
			//        System.Drawing.Image NinePatchedImage = AndroidCommonPaint.GetNinePatchedImage(strImagePath, new System.Drawing.Size((int)labelButton.Width, (int)Height));

			//        if (System.Drawing.Color.Transparent != NinePatchBrightnessColor)
			//        {
			//            AndroidCommonPaint.SetColorEx(NinePatchedImage, NinePatchBrightnessColor);
			//        }

			//        byte[] byteData = AndroidCommonPaint.ImageToByteArray(NinePatchedImage, ImgFile.Extension);

			//        using (MemoryStream ms = new MemoryStream(byteData))
			//        {
			//            var decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
			//            BitmapSource ButtonBitmapSource = decoder.Frames[0] as BitmapSource;
			//            labelButton.Source = ButtonBitmapSource;
			//            //ScanAtomRectangle.Fill = new ImageBrush(ButtonBitmapSource);
			//        }
			//    }
			//}

			return base.ArrangeOverride (arrangeBounds);
		}

		public override void ChangeAtomMode (int nRunMode)
		{
			base.ChangeAtomMode (nRunMode);

			if (0 == nRunMode)
			{
				AtomSplitter.IsEnabled = true;
			}
			else
			{
				AtomSplitter.IsEnabled = false;
			}
		}

		public override void DoPostEditMode ()
		{
			SetReadOnlyControl ();
		}

		public override void CompleteAtomSizeChanged ()
		{
			base.CompleteAtomSizeChanged ();

			labelButton.Width = ButtonImage.Width;
			if ((this.Width / 4) <= ButtonImage.Width)
			{
				labelButton.Width = this.Width / 4;
			}

			UpdateButtonWidth ();
		}

		protected override void InitializeDelegateEvents ()
		{
			base.InitializeDelegateEvents ();
		}

		public override void SetAtomBorder (Brush applyBrush)
		{
			AtomBorderRectangle.Stroke = applyBrush;
		}

		public override Brush GetAtomBorder ()
		{
			return AtomBorderRectangle.Stroke;
		}

		public override void SetAtomThickness (Thickness applyThickness)
		{
			AtomBorderRectangle.StrokeThickness = applyThickness.Left;
		}

		public override Thickness GetAtomThickness ()
		{
			return new Thickness (AtomBorderRectangle.StrokeThickness);
		}

		public override void SetAtomDashArray (DoubleCollection applyDashArray)
		{
			AtomBorderRectangle.StrokeDashArray = applyDashArray;
		}

		public override DoubleCollection GetAtomDashArray ()
		{
			return AtomBorderRectangle.StrokeDashArray;
		}

		public override void CloneAtom (AtomBase ClonedAtom, bool bDeepCopy)
		{
			ClonedAtom.SetAtomBackground (this.GetAtomBackground ());
			ClonedAtom.SetAtomBorder (this.GetAtomBorder ());
			ClonedAtom.SetAtomDashArray (this.GetAtomDashArray ());
			ClonedAtom.SetAtomThickness (this.GetAtomThickness ());
			ClonedAtom.SetTextUnderLine (this.GetTextUnderLine ());

			base.CloneAtom (ClonedAtom, bDeepCopy);
		}

		public override void SerializeLoadSync_AttribToAtom (bool bIs80Model)
		{
			base.SerializeLoadSync_AttribToAtom (bIs80Model);

			this.ImagePathChanged ();

		}

		public override void Sync_AttribToAtom ()
		{
			base.Sync_AttribToAtom ();

			this.ImagePathChanged ();
		}

		public override void CompletePropertyChanged ()
		{
			SetReadOnlyControl ();
			SetAttachStyle (AttachStyle);

			ImagePathChanged ();
		}

		private void ImagePathChanged ()
		{
			if (null != AtomCore)
			{
				WebFileAttachAtom atomCore = AtomCore as WebFileAttachAtom;
				atomCore.ImagePathChanged ();
			}
		}

		public BitmapImage ButtonImage
		{
			set { m_imageButton = value; }
			get { return m_imageButton; }
		}

		public BitmapImage RolloverImage
		{
			set { m_imageRollover = value; }
			get { return m_imageRollover; }
		}


		public double ButtonImageWidth
		{
			get { return labelButton.Width; }
			set { labelButton.Width = value; }
		}

		public void FlleAttachControl ()
		{
			if (LC.LANG.ENGLISH == LC.PQLanguage)
			{
				LC.FormLocalize ("FlleAttachControl", this);
			}

			InitImageList ();

			m_Contents = new CStringArray ();

			ResourceDictionary findedResource = StyleResourceManager.GetApplicationComponentStyle (StyleCategory.StyleComponentCategory.ScrollViewer);
			ListViewScrollViewer.Resources = findedResource;

			if (null != findedResource)
			{
				ControlTemplate applyResource = findedResource["DefaultScrollViewerControlTemplate"] as ControlTemplate;
				ListViewScrollViewer.Template = applyResource;
			}

			InitContextMenu ();
		}

		private void InitImageList ()
		{
			ButtonImage = ManageSmartAtomImage.GetSmartAtomImage ("FileAttach.addfile");
			RolloverImage = ManageSmartAtomImage.GetSmartAtomImage ("FileAttach.addfile");

			if (LC.LANG.JAPAN == LC.PQLanguage)
			{
				ButtonImage = ManageSmartAtomImage.GetSmartAtomImage ("FileAttach.addfile_j");
				RolloverImage = ManageSmartAtomImage.GetSmartAtomImage ("FileAttach.addfile_j");
			}
			else if (LC.LANG.ENGLISH == LC.PQLanguage)
			{
				ButtonImage = ManageSmartAtomImage.GetSmartAtomImage ("FileAttach.addfile_en");
				RolloverImage = ManageSmartAtomImage.GetSmartAtomImage ("FileAttach.addfile_en");
			}
			if (labelButton.Source != ButtonImage)
			{
				labelButton.Source = ButtonImage;
				labelButton.Width = ButtonImage.Width + 2;

				UpdateButtonWidth ();
			}
		}

		public void SetButtonImage (BitmapImage ActionBitmapImage, ImageSource ActionBitmapSource)
		{
			if (null != ButtonImage)
			{
				ButtonImage = ActionBitmapImage;
				labelButton.Source = ButtonImage;

				if (1 != AtomCore.AtomRunMode)
				{
					labelButton.Width = ActionBitmapImage.Width;
					if ((this.Width / 4) <= ActionBitmapImage.Width)
					{
						labelButton.Width = this.Width / 4;
					}
				}

			}
		}

		#region 버튼이미지처리

		private void labelButton_MouseEnter (object sender, System.Windows.Input.MouseEventArgs e)
		{
			WebFileAttachAtom actionAtomCore = AtomCore as WebFileAttachAtom;

			if (null != actionAtomCore)
			{
				actionAtomCore.SetMouseEnterBackground ();
			}
		}

		private void labelButton_MouseLeave (object sender, System.Windows.Input.MouseEventArgs e)
		{
			WebFileAttachAtom actionAtomCore = AtomCore as WebFileAttachAtom;

			if (null != actionAtomCore)
			{
				actionAtomCore.SetMouseLeaveBackground ();
			}
		}


		#endregion

		public void SetRollOverImage (BitmapImage ActionBitmapImage)
		{
			labelButton.Source = ActionBitmapImage;
		}

		public void SetInitButtonImage ()
		{
			InitImageList ();

			labelButton.Source = ButtonImage;
		}

		private void ButtonImageChange ()
		{
			labelButton.Source = RolloverImage;
		}

		private void AddIconItems (string strPath)
		{
			ImageSource iconSource = iconList.GetImage (strPath);
			RowItem item = new RowItem (iconSource, strPath);

			this.listView.Items.Add (item);
		}

		#region Apply/Release RunModeProperty

		public override void ReleaseRunModeProperty ()
		{
			base.ReleaseRunModeProperty ();

			ClearFileList (true);

			OpenFile -= new EventHandler (OnFileAttachOpenFile);
		}

		public override void ApplyRunModeProperty ()
		{
			base.ApplyRunModeProperty ();

			OpenFile += new EventHandler (OnFileAttachOpenFile);
		}

		#endregion

		private void labelButton_MouseDown (object sender, MouseButtonEventArgs e)
		{
			if (null != this.OpenFile)
			{
				switch (AttachStyle)
				{
					case 0:
						ShowFileDialog ();
						break;
					case 1:
						{
							if (null != this.OpenFile)
							{
								OpenFile (this, null);  // OnShowFile ();
							}
						}
						break;
				}
			}
		}

		public void ClearFileList (bool bLocal)
		{
			listView.Items.Clear ();

			if (false != bLocal)
			{
				m_mapLocalFilePath.Clear ();
			}
		}

		public ArrayList GetFileList ()
		{
			ArrayList fileList = new ArrayList ();

			foreach (RowItem item in listView.Items)
			{
				fileList.Add (item.Tag);
			}

			return fileList;
		}

		public void AddFileList (string strFileName, string strContent)
		{

			ImageSource iconSource = iconList.GetImage (strFileName);
			RowItem item = new RowItem (iconSource, strFileName);
			listView.Items.Add (item);

			item.Tag = strContent;
		}

		public string GetLocalFilePath (string strFileName)
		{
			string strFilePath = "";

			string strName = ServerPathHandler.GetServerFileName (strFileName);
			if (0 == strName.Length)
				strName = strFileName;

			if (false != m_mapLocalFilePath.Contains (strName))
			{
				strFilePath = m_mapLocalFilePath[strName] as string;
			}

			return strFilePath;
		}

		public CStringArray GetImageFileNameArray ()
		{
			CStringArray ImageFileNameArray = new CStringArray ();

			foreach (string strContent in GetFileList ())
			{
				string ImageFileName = ServerPathHandler.GetServerFileName (strContent);
				if (false == string.IsNullOrEmpty (ImageFileName))
				{
					ImageFileNameArray.Add (ImageFileName);
				}
			}

			return ImageFileNameArray;
		}

		public CStringArray GetLocalFilePathArray ()
		{
			CStringArray localFilePathArray = new CStringArray ();

			foreach (string strContent in GetFileList ())
			{
				string strLocalFilePath = GetLocalFilePath (strContent);
				if (false == string.IsNullOrEmpty (strLocalFilePath))
				{
					localFilePathArray.Add (strLocalFilePath);
				}
			}

			return localFilePathArray;
		}

		protected long GetTotalSize (string[] strFileNames)
		{
			long nSize = 0;

			foreach (string strFilePath in m_mapLocalFilePath.Values)
			{
				if (false != File.Exists (strFilePath))
				{
					FileInfo fileInfo = new FileInfo (strFilePath);
					nSize += fileInfo.Length;   // Byte
				}
			}

			foreach (string strFileName in strFileNames)
			{
				FileInfo fileInfo = new FileInfo (strFileName);
				nSize += fileInfo.Length;   // Byte
			}

			return nSize;
		}

		/// 파일크기,갯수,용량 제한
		protected bool IsEnableCheckAttachFile (string[] saFileNames)
		{
			if (FileCount < this.listView.Items.Count + saFileNames.Length)
			{
				string strMessage = string.Format (LC.GS ("TopWebAtom_FileAttachofAtom_3"), FileCount);
				_Message80.Show (strMessage);

				return false;
			}

			long nSize = GetTotalSize (saFileNames);
			if (TotalSize * 1024 < nSize)
			{
				string strMessage = string.Format (LC.GS ("TopWebAtom_FileAttachofAtom_1"), TotalSize);
				_Message80.Show (strMessage);

				return false;
			}

			foreach (string strFilePath in saFileNames)
			{
				FileInfo fileInfo = new FileInfo (strFilePath);
				int nFileLength = (int)(fileInfo.Length / 1024);

				if (FileSize < nFileLength)
				{
					string strMessage = string.Format (LC.GS ("TopWebAtom_FileAttachofAtom_2"), FileSize);
					_Message80.Show (strMessage);

					return false;
				}
			}

			return true;
		}

		public void ShowFileDialog ()
		{
			System.Windows.Forms.OpenFileDialog openDlg = new System.Windows.Forms.OpenFileDialog ();

			openDlg.FileName = "";
			openDlg.CheckFileExists = true;
			openDlg.CheckPathExists = true;
			openDlg.Multiselect = true;
			PathHandler.SetInitialDirectory (openDlg);
			System.Windows.Forms.DialogResult res = openDlg.ShowDialog ();

			if (res == System.Windows.Forms.DialogResult.OK)
			{
				bool bEnable = IsEnableCheckAttachFile (openDlg.FileNames);
				if (false == bEnable)
					return;

				long tick = DateTime.Now.Ticks;

				foreach (string strFilePath in openDlg.FileNames)
				{
					string strFileName = Path.GetFileName (strFilePath);

					ImageSource iconSource = iconList.GetImage (strFilePath);
					RowItem item = new RowItem (iconSource, strFileName);

					this.listView.Items.Add (item);

					FileInfo fileInfo = new FileInfo (strFilePath);
					int nFileLength = (int)(fileInfo.Length / 1024);

					// 사용할폴더/생성된파일명.확장자$올린파일명$파일크기 -> 2020-11-20 kys 서버와 동기화를 위해 파일 포맷 변경 (사용할폴더/생성된파일명.확장자$올린파일명.확장자$파일크기)
					// 2024.08.23 beh 복수 파일 추가 시 Tick이 동일하여 서버에 파일이 한 개만 올라가는 현상 수정
					string strContent = string.Format (@"{0}/{1}{2}${3}${4}",
						AttachPath.Replace ("\\", "/"),
						tick++.ToString (),
						System.IO.Path.GetExtension (strFileName),
						System.IO.Path.GetFileName (strFileName),
						nFileLength);

					item.Tag = strContent;

					m_mapLocalFilePath[strFileName] = strFilePath;
				}

				WebFileAttachAtom fileAttach = this.AtomCore as WebFileAttachAtom;
				fileAttach.UpdateContentString ();
			}
		}


		#region ContextMenu

		private void InitContextMenu ()
		{
			if (false == ReadOnly)
			{
				this.listView.ContextMenu = new System.Windows.Controls.ContextMenu ();

				MenuItem FileMenuItem = new MenuItem ();
				FileMenuItem.Header = LC.GS ("TopWebAtom_FileAttachofAtom_4");
				FileMenuItem.Click += ContextMenuItem_Click;

				this.listView.ContextMenu.Items.Add (FileMenuItem);

				this.listView.MouseRightButtonUp -= ListView_MouseRightButtonUp;
				this.listView.MouseRightButtonUp += ListView_MouseRightButtonUp;
			}
		}

		private void ListView_MouseRightButtonUp (object sender, MouseButtonEventArgs e)
		{
			if (null != this.listView.ContextMenu)
			{
				this.listView.ContextMenu.IsOpen = true;
			}
		}

		private void ContextMenuItem_Click (object sender, RoutedEventArgs e)
		{
			string strMenuHeader = ((MenuItem)sender).Header.ToString ();
			WebFileAttachAtom fileAttach = this.AtomCore as WebFileAttachAtom;
			if (0 < listView.SelectedItems.Count)
			{
				RowItem item = listView.SelectedItems[0] as RowItem;
				string itemtag = item.Tag as string;
				m_mapLocalFilePath.Remove (GetRealFileName (itemtag));
				listView.Items.Remove (item);
				fileAttach.UpdateContentString ();
			}
		}

		private void ReleaseContextMenu ()
		{
			this.listView.ContextMenu = null;
		}

		#endregion

		//public void AutoSizeFindButton(bool bAutoSize)
		//{
		//    if (null != m_imageButton && false != bAutoSize)
		//    {
		//        labelButton.Text = "";

		//        labelButton.Width = m_imageButton.Width;
		//        labelButton.Height = m_imageButton.Height;
		//    }
		//    else
		//    {
		//        labelButton.Text = LC.GS("찾아보기");

		//        Graphics dc = labelButton.CreateGraphics();
		//        labelButton.Width = (int)dc.MeasureString(LC.GS(" 찾아보기 "), labelButton.Font).Width;
		//        dc.Dispose();
		//    }

		//    labelButton.Invalidate();
		//}

		protected int GetButtonWidth ()
		{
			return (int)labelButton.Width;
		}

		public void SetReadOnlyControl ()
		{
			var atomAttrib = this.AtomCore.Attrib as WebFileAttachAttrib;

            if (AttachStyle == 0)
			{
				if (false != atomAttrib.IsReadOnly)
				{
					labelButton.Visibility = System.Windows.Visibility.Collapsed;
				}
				else
				{
					labelButton.Visibility = System.Windows.Visibility.Visible;
				}
			}
			else
			{
				labelButton.Visibility = System.Windows.Visibility.Collapsed;
			}

			if (true == atomAttrib.IsReadOnly)
			{
				//읽기전용일때는 삭제메뉴 표시하지 않도록함
				this.listView.ContextMenu = null;
				AtomSplitter.Visibility = Visibility.Collapsed;
				RunModeGrid.ColumnDefinitions[2].Width = new GridLength (0);
			}
			else
			{
				InitContextMenu ();
				AtomSplitter.Visibility = Visibility.Visible;
				RunModeGrid.ColumnDefinitions[2].Width = new GridLength (atomAttrib.ButtonWidth);
			}
		}

		protected void SetAttachStyle (int nAttachStyle)
		{
			AttachStyle = nAttachStyle;

			switch (nAttachStyle)
			{
				case 0:
					ShowListStyle ();
					break;
				case 1:
					ShowIconStyle ();
					break;
			}
		}

		protected void ShowIconStyle ()
		{
            AtomSplitter.Visibility = System.Windows.Visibility.Collapsed; // 추가
            labelButton.Visibility = System.Windows.Visibility.Collapsed; // 추가

            if (AtomSplitter != null) 
			{ 
				Debug.WriteLine("AtomSplitter is founded"); 
			}
            if (labelButton != null)
            {
                Debug.WriteLine("labelButton is founded");
            }

           // RootGrid.UpdateLayout();
        }

		protected void ShowListStyle ()
		{
            AtomSplitter.Visibility = System.Windows.Visibility.Visible;
            labelButton.Visibility = System.Windows.Visibility.Visible;

            if (AtomSplitter != null)
            {
                Debug.WriteLine("AtomSplitter is founded");
            }
            if (labelButton != null)
            {
                Debug.WriteLine("labelButton is founded");
            }

            // RootGrid.UpdateLayout();
        }

		public string GetSelectedFileName ()
		{
			string strFileName = "";

			RowItem item = GetSelectedItem ();
			if (null != item)
			{
				strFileName = item.col2;
			}

			return strFileName;
		}

		private string GetRealFileName (string tagstr)
		{
			string Filename = "";
			string[] tagArr = tagstr.Split ('$');

			if (null != tagArr)
			{
				if (1 < tagArr.Count ())
				{
					Filename = tagArr[1];
				}
			}

			return Filename;
		}

		private string GetSelectedFilePath ()
		{
			string strLocalPath = "";

			RowItem item = GetSelectedItem ();
			if (null != item && null != item.Tag)
			{
				strLocalPath = item.Tag as String;
			}

			return strLocalPath;
		}

		private RowItem GetSelectedItem ()
		{
			RowItem item = null;

			if (0 < this.listView.SelectedItems.Count)
			{
				item = listView.SelectedItems[0] as RowItem;
			}

			return item;
		}

		public void DefaultSelect ()
		{
			if (0 < this.listView.Items.Count)
			{
				this.listView.SelectedIndex = 0;
			}
		}

		public void OnShowFile ()
		{
			string strFileName = GetSelectedFileName ();
			string strLocalFile = GetLocalFilePath (strFileName);
			if (0 < strLocalFile.Length)
			{
				OnShowLocalFile (strLocalFile);
			}
			else
			{
				// 사용할폴더/생성된파일명.확장자$올린파일명$파일크기 -> 2020-11-20 kys 서버와 동기화를 위해 파일 포맷 변경 (사용할폴더/생성된파일명.확장자$올린파일명.확장자$파일크기)
				string strContent = GetSelectedFilePath ();

				if (null == m_Contents || 0 == m_Contents.Count)
				{
					string strPhysicalFile = ServerPathHandler.GetServerFilePath (strContent);
					OnShowServerFile (strPhysicalFile, strFileName);
				}
				else // 2009.02.13 이정대, 멀티다운로드 시
				{
					foreach (string str in m_Contents)
					{
						strContent = str;
						if (-1 != str.LastIndexOf ("/") && -1 != str.IndexOf ("$"))
						{
							strFileName = str.Substring (str.LastIndexOf ("/") + 1, str.IndexOf ("$") - str.LastIndexOf ("/") - 1);
						}

						string strPhysicalFile = ServerPathHandler.GetServerFilePath (strContent);
						OnShowServerFile (strPhysicalFile, strFileName);
					}
				}
			}
		}

		protected void OnShowServerFile (string strFilePath, string strLocalPath)
		{
			if (0 < strFilePath.Length)
			{
				bool bDownload = false;

				if (true == _PQRemoting.UseJsonServer)
				{
					bDownload = JsonServerImage.FileDownload (strFilePath, strLocalPath);
				}
				else
				{
					bDownload = ServerImage.FileDownload (strFilePath, strLocalPath);
				}

				if (false != bDownload)
				{
					OnShowLocalFile (strLocalPath);
				}
			}
		}

		protected void OnShowLocalFile (string strFilePath)
		{
			Process.Start (strFilePath);
		}

		// 2009.02.13 이정대, 멀티다운로드, 리스트 업데이트
		public void UpdateContent (string strContent, bool bAdd /*true*/)
		{
			if (false != bAdd)
			{
				if (false == m_Contents.Contains (strContent))
				{
					m_Contents.Add (strContent);
				}
			}
			else
			{
				if (false != m_Contents.Contains (strContent))
				{
					m_Contents.Remove (strContent);
				}
			}
		}

		private void listView_MouseDoubleClick (object sender, MouseButtonEventArgs e)
		{
			Point ptMouse = Mouse.GetPosition (this.listView);

			HitTestResult hitTest = VisualTreeHelper.HitTest (this.listView, ptMouse);
			System.Windows.Controls.ListViewItem item = GetListViewItemFromEvent (hitTest.VisualHit) as System.Windows.Controls.ListViewItem;
			if (null != item)
			{
				if (null != this.OpenFile)
				{
					OpenFile (this, null);  //OnShowFile ();
				}
			}
		}

		private System.Windows.Controls.ListViewItem GetListViewItemFromEvent (object originalSource)
		{
			DependencyObject depObj = originalSource as DependencyObject;
			if (depObj != null)
			{
				// go up the visual hierarchy until we find the list view item the click came from  
				// the click might have been on the grid or column headers so we need to cater for this  
				DependencyObject current = depObj;
				while (current != null && current != this.listView)
				{
					System.Windows.Controls.ListViewItem ListViewItem = current as System.Windows.Controls.ListViewItem;
					if (ListViewItem != null)
					{
						return ListViewItem;
					}
					current = VisualTreeHelper.GetParent (current);
				}
			}

			return null;
		}

		protected void OnFileAttachOpenFile (object sender, EventArgs e)
		{
			string strFilePath = GetSelectedFileName ();

			CVariantX[] pvaArgs = new CVariantX[2];
			pvaArgs[0] = new CVariantX (1);
			pvaArgs[1] = new CVariantX (strFilePath);

			if (-1 != AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_OPEN_FILE, pvaArgs))
			{
				if (-1 != MsgHandler.CALL_MSG_HANDLER (this.AtomCore, EVS_TYPE.EVS_A_OPEN_FILE, pvaArgs))   // _파일열림
				{
					OnShowFile ();

					AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_OPEN_FILE, pvaArgs);
				}
			}
		}

		private void AtomSplitter_DragCompleted (object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			ButtonImageWidth = RunModeGrid.ColumnDefinitions[2].Width.Value;
			UpdateButtonWidth ();
		}

		public void UpdateButtonWidth ()
		{
			var atomAttrib = this.AtomCore.Attrib as WebFileAttachAttrib;

			atomAttrib.ButtonWidth = (int)ButtonImageWidth;
			RunModeGrid.ColumnDefinitions[2].Width = new GridLength (atomAttrib.ButtonWidth);
		}
	}
}
