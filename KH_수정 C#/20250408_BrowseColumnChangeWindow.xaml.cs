using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtomRun.BrowseHelper;
using Softpower.SmartMaker.TopControl.ResizableWindow;

namespace Softpower.SmartMaker.TopAtomRun
{
	public class ColumnListBoxItem
	{
		private bool m_IsChecked = false;
		private string m_strText;

		public bool IsChecked
		{
			get
			{
				return m_IsChecked;
			}
			set
			{
				m_IsChecked = value;
			}
		}

		public string Text
		{
			get
			{
				return m_strText;
			}
			set
			{
				m_strText = value;
			}
		}

		public ColumnListBoxItem ()
			: this (false, string.Empty)
		{

		}

		public ColumnListBoxItem (string strText) : this (false, strText)
		{

		}

		public ColumnListBoxItem (bool bIsChecked, string strText)
		{
			Text = strText;
			IsChecked = bIsChecked;
		}
	}


	/// <summary>
	/// Interaction logic for BrowseColumnChangeWindow.xaml
	/// </summary>
	public partial class BrowseColumnChangeWindow : CanResizeWindow
	{
		private ArrayList m_poaBrowse = new ArrayList ();
		private BrowseColumnVariant m_BrowseColumnVariant;
		private bool bColumnListChanged = false;

		public bool ColumnListChanged
		{
			get { return bColumnListChanged; }
			set { bColumnListChanged = value; }
		}

		public BrowseColumnChangeWindow ()
		{
			var baseContent = Content;
			InitializeComponent ();
			var childContent = Content;
			Content = baseContent;
			ContentGrid = childContent as Grid;

			InitEvent ();
			InitStyle ();

			InitButtonStyle ();
			InitButtonImage ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("BrowseColumnChangeWindow", this);
			}
		}

		private void InitStyle ()
		{
			SystemButtonType = ResizeConfig.ResizeWindowSystemButtonType.None;
			IsResizable = true;
			CaptionHeight = 25;
			Title = LC.GS ("TopAtomRun_BrowseColumnChangeWindow_1"); //항목변경
			SizeToContent = System.Windows.SizeToContent.Manual;
		}

		private void InitEvent ()
		{
			Loaded += BrowseColumnChangeWindow_Loaded;
			OKButton.MouseLeftButtonUp += OKButton_MouseLeftButtonUp;
			CancelButton.MouseLeftButtonUp += CancelButton_MouseLeftButtonUp;

			TopNewButton.MouseLeftButtonUp += TopNewButton_MouseLeftButtonUp;
			TopSaveButton.MouseLeftButtonDown += TopSaveButton_MouseLeftButtonUp;
			TopDeleteButton.MouseLeftButtonDown += TopDeleteButton_MouseLeftButtonUp;

			this.LayoutComboBox.SelectionChanged = LayoutComboBox_SelectionChanged;
		}

		private void InitButtonStyle ()
		{
			TopNewButton.BorderThickness = new System.Windows.Thickness (0.5);
			TopSaveButton.BorderThickness = new System.Windows.Thickness (0.5);
			TopDeleteButton.BorderThickness = new System.Windows.Thickness (0.5);
		}

		private void InitButtonImage ()
		{
			string TOPMENU_NEW_IMAGE = TopControl.Resources.TopCommonMenu.ResourceManager.TOPCOMMONMENU_NEW_IMAGE;
			string TOPMENU_SAVE_IMAGE = TopControl.Resources.TopCommonMenu.ResourceManager.TOPCOMMONMENU_SAVE_IMAGE;
			string TOPMENU_DELETE_IMAGE = TopControl.Resources.TopCommonMenu.ResourceManager.TOPCOMMONMENU_DELETE_IMAGE;

			TopNewImage.Source = TopControl.Resources.TopCommonMenu.ResourceManager.Instance.GetTopCommonMenuImage (TOPMENU_NEW_IMAGE);
			TopSaveImage.Source = TopControl.Resources.TopCommonMenu.ResourceManager.Instance.GetTopCommonMenuImage (TOPMENU_SAVE_IMAGE);
			TopDeleteImage.Source = TopControl.Resources.TopCommonMenu.ResourceManager.Instance.GetTopCommonMenuImage (TOPMENU_DELETE_IMAGE);
		}

		public void SetInitValue (ArrayList poaBrowse)
		{
			m_poaBrowse = poaBrowse.Clone () as ArrayList;
		}

		public void SetInitValue (string strFileName, string strProperVar, string strLayoutName, ArrayList poaBrowse)
		{
			BrowseColumnFile browseColumnFile = BrowseColumnHelper.Instance.GetBrowseColumnFile (strFileName);
			if (null == browseColumnFile)
				BrowseColumnHelper.Instance.UpdateBrowseColumnVariant (strFileName, strProperVar, strLayoutName, poaBrowse);

			browseColumnFile = BrowseColumnHelper.Instance.GetBrowseColumnFile (strFileName);
			if (null != browseColumnFile)
			{
				BrowseColumnVariant browseColumnVariant = browseColumnFile.GetBrowseColumnVariant (strProperVar);
				if (null != browseColumnVariant)
				{
					m_BrowseColumnVariant = browseColumnVariant;

					foreach (string strLayout in browseColumnVariant.BrowseColumnList.Keys)
					{
						LayoutComboBox.Items.Add (strLayout);
					}

					if (LayoutComboBox.Items.Count > 0)
					{
						if (LayoutComboBox.Items.Contains (strLayoutName))
							LayoutComboBox.SelectedItem = strLayoutName;
						else
							LayoutComboBox.SelectedIndex = 0;
					}
				}
			}
		}

		private void LayoutComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			string strLayout = LayoutComboBox.SelectedItem as string;
			if (false == string.IsNullOrEmpty (strLayout))
			{
				List<BrowseColumn> listBrowseColumn = m_BrowseColumnVariant.GetBrowseColumnList (strLayout);
				if (null != listBrowseColumn)
				{
					ColumnListBox.Items.Clear ();

					if(ColumnListChanged)
					{
						for(int i = 0; i < m_poaBrowse.Count; i++)
						{
							BrowseItem item = m_poaBrowse[i] as BrowseItem;
							ColumnListBox.Items.Add (new ColumnListBoxItem (0 == listBrowseColumn[i].DisplayOrHide, item.BrowseVar));
						}
					}
					else
					{
						foreach (BrowseColumn browseColumn in listBrowseColumn)
						{
							ColumnListBox.Items.Add (new ColumnListBoxItem (0 == browseColumn.DisplayOrHide ? true : false, browseColumn.Label));
						}
					}						
				}
			}
		}

		public bool IsCheckedColumn (int nColumn)
		{
			if (ColumnListBox.Items.Count <= nColumn || 0 > nColumn)
				return false;

			return ((ColumnListBoxItem)ColumnListBox.Items[nColumn]).IsChecked;
		}

		private bool FillColumnCtrl ()
		{
			int nIndex = 0;
			string sLabel = "";

			for (nIndex = 0; nIndex < m_poaBrowse.Count; nIndex++)
			{
				BrowseItem pBrowseAtom = (BrowseItem)m_poaBrowse[nIndex];
				sLabel = pBrowseAtom.GetLabel_Type_Len ();
				ColumnListBox.Items.Add (new ColumnListBoxItem (0 == pBrowseAtom.DisplayOrHide ? true : false, sLabel));
			}
			return true;
		}

		/// <summary>
		/// 레이아웃 추가시 기본이름
		/// </summary>
		/// <returns></returns>
		private string GetNewLayoutName ()
		{
			string strDefaultName = "LAYOUT";

			int nIndex = 0;
			while (true)
			{
				string strNewName = $"{strDefaultName}{nIndex++}";
				if (false == m_BrowseColumnVariant.BrowseColumnList.ContainsKey (strNewName))
				{
					return strNewName;
				}
			}
		}

		/// <summary>
		/// 새 레이아웃 추가
		/// </summary>
		private void NewBrowseColumnLayout ()
		{
			string strNewLayout = GetNewLayoutName ();

			if (false == m_BrowseColumnVariant.BrowseColumnList.ContainsKey (strNewLayout))
			{
				List<BrowseColumn> listBrowseColumn = BrowseColumnHelper.Instance.GetConvertBrowseColumn (m_poaBrowse);
				m_BrowseColumnVariant.BrowseColumnList.Add (strNewLayout, listBrowseColumn);

				LayoutComboBox.Items.Add (strNewLayout);
				LayoutComboBox.SelectedIndex = LayoutComboBox.Items.Count - 1;
			}
		}


		/// <summary>
		/// 현재 레이아웃 이름 바꾸기
		/// </summary>
		private void RenameBrowseColumnLayout ()
		{
			string strOrgLayout = LayoutComboBox.SelectedItem.ToString ();
			string strRenameLayout = LayoutComboBox.EdittingText;

			if (0 < LayoutComboBox.SelectedIndex)
			{
				if (false == string.IsNullOrEmpty (strOrgLayout))
				{
					if (false == string.Equals (strOrgLayout, strRenameLayout))
					{
						if (false == m_BrowseColumnVariant.BrowseColumnList.ContainsKey (strRenameLayout))
						{
							List<BrowseColumn> listBrowseColumn = m_BrowseColumnVariant.BrowseColumnList[strOrgLayout];

							m_BrowseColumnVariant.BrowseColumnList.Remove (strOrgLayout);
							m_BrowseColumnVariant.BrowseColumnList.Add (strRenameLayout, listBrowseColumn);

							int nIndex = LayoutComboBox.SelectedIndex;
							LayoutComboBox.Items.Remove (strOrgLayout);
							LayoutComboBox.Items.Insert (nIndex, strRenameLayout);
						}
					}
				}
			}
			else
			{
				LayoutComboBox.EdittingText = m_BrowseColumnVariant.CurrentLayout;

				_Message80.Show ("기본 레이아웃은 변경할 수 없습니다.");
			}
		}

		/// <summary>
		/// 현재 레이아웃 저장
		/// </summary>
		private void SaveBrowseColumnLayout ()
		{
			string strSelectedLayout = LayoutComboBox.EdittingText;
			if (false == string.IsNullOrEmpty (strSelectedLayout))
			{
				if (m_BrowseColumnVariant.BrowseColumnList.ContainsKey (strSelectedLayout))
				{
					List<BrowseColumn> listBrowseColumn = m_BrowseColumnVariant.BrowseColumnList[strSelectedLayout];

					int nIndex = 0;
					foreach (ColumnListBoxItem listBoxItem in ColumnListBox.Items)
					{
						if (nIndex < listBrowseColumn.Count)
						{
							BrowseColumn browseColumn = listBrowseColumn[nIndex];

							browseColumn.DisplayOrHide = true == listBoxItem.IsChecked ? 0 : 1;
							if (0 == browseColumn.DisplayOrHide)
							{
								if (0 == browseColumn.RealLen)
								{
									browseColumn.RealLen = 115; // BrowseContentsPanel.DEFAULT_COLUMN_WIDTH;
								}
							}
						}

						nIndex++;
					}
				}
			}
		}

		/// <summary>
		/// 레이아웃 삭제
		/// </summary>
		private void DeleteBrowseColumnLayout ()
		{
			int nSelectedIndex = LayoutComboBox.SelectedIndex;
			if (0 < nSelectedIndex)
			{
				string strSelectedLayout = LayoutComboBox.SelectedItem.ToString ();
				if (m_BrowseColumnVariant.BrowseColumnList.ContainsKey (strSelectedLayout))
				{
					m_BrowseColumnVariant.BrowseColumnList.Remove (strSelectedLayout);

					LayoutComboBox.Items.RemoveAt (nSelectedIndex);

					LayoutComboBox.SelectedIndex = 0;
				}
			}
			else
			{
				_Message80.Show ("기본 레이아웃은 삭제할 수 없습니다.");
			}
		}

		void BrowseColumnChangeWindow_Loaded (object sender, RoutedEventArgs e)
		{
		}

		void TopNewButton_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			NewBrowseColumnLayout ();
		}

		void TopSaveButton_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			RenameBrowseColumnLayout ();
			SaveBrowseColumnLayout ();
		}

		void TopDeleteButton_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DeleteBrowseColumnLayout ();
		}

		void OKButton_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
		{
			m_BrowseColumnVariant.CurrentLayout = LayoutComboBox.EdittingText;
			BrowseColumnHelper.Instance.FileSaveBrowseColumn ();

			DialogResult = true;
		}

		void CancelButton_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
		{
			DialogResult = false;
		}

		public void disableGridContent1 ()
		{
			LayoutComboBox.IsEnabled = false;
			TopNewButton.Visibility = Visibility.Collapsed;
			TopSaveButton.Visibility = Visibility.Collapsed;
			TopDeleteButton.Visibility = Visibility.Collapsed;
		}
	}
}
