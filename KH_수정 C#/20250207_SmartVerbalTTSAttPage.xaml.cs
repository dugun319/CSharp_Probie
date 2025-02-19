using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.License.Manager.Join.SolutionEx;    //  라이선스 체제 변경안(v3.0)
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.License;
using Softpower.SmartMaker.TopSmartAtomEdit.AttBase;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPageStyle;

namespace Softpower.SmartMaker.TopSmartAtomEdit.AttPage
{
	/// <summary>
	/// Interaction logic for SmartVerbalTTSAttPage.xaml
	/// </summary>
	public partial class SmartVerbalTTSAttPage : AttBasePage
	{
		#region Initialize

		public SmartVerbalTTSAttPage ()
		{
			InitLanguageCollection ();
			InitPitchCollection ();
			InitRateCollection ();

			InitializeComponent ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("SmartVerbalTTSAttPage", this);
			}

			this.SetStyle ();
			this.InitEvent ();
		}

		public override void SetTitle (string strTitle)
		{
			this.TitleTextBlock.Text = strTitle;
		}

		public override void SetStyle ()
		{
			this.Style = AttPageStyleManager.StyleManager.GetAttPageSyle ();
			this.TitleGrid.Style = AttPageStyleManager.StyleManager.GetAttPageTitleGridStyle ();
			this.TitleTextBlock.Style = AttPageStyleManager.StyleManager.GetAttPageTitleTextStyle ();
			this.GroupBox1.Style = AttPageStyleManager.StyleManager.GetAttPageGroupBoxStyle ();
			this.GroupBox2.Style = AttPageStyleManager.StyleManager.GetAttPageGroupBoxStyle ();
			this.GroupBox3.Style = AttPageStyleManager.StyleManager.GetAttPageGroupBoxStyle ();

			this.SelLangComboBox.ItemsSource = LanguageCollection;
			this.SelLangComboBox.DisplayMemberPath = "Key";
			this.SelLangComboBox.SelectedValuePath = "Value";

			this.RateComboBox.ItemsSource = RateCollection;
			this.RateComboBox.DisplayMemberPath = "Key";
			this.RateComboBox.SelectedValuePath = "Value";

			this.PitchComboBox.ItemsSource = PitchCollection;
			this.PitchComboBox.DisplayMemberPath = "Key";
			this.PitchComboBox.SelectedValuePath = "Value";
		}

		private void InitEvent ()
		{
			this.TransFileSearchButton.AddHandler (System.Windows.Controls.UserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler (TransFileSearchButton_Click), true);
			this.OutputSearchButton.AddHandler (System.Windows.Controls.UserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler (OutputSearchButton_Click), true);
			this.Loaded += SmartVerbalTTSAttPage_Loaded;
		}

		private void SmartVerbalTTSAttPage_Loaded (object sender, RoutedEventArgs e)
		{
			this.InputMethodTextBox.Information = this.Information;
			this.AtomNameTextBox.Information = this.Information;
		}

		#endregion Initialize

		#region Property
		public bool WndVisible
		{
			get
			{
				return (bool)this.VisibleCheckBox.IsChecked;
			}
			set
			{
				this.VisibleCheckBox.IsChecked = value;
			}
		}

		public bool Vanish
		{
			get
			{
				return (bool)this.VanishCheckBox.IsChecked;
			}
			set
			{
				this.VanishCheckBox.IsChecked = value;
			}
		}

        public bool atomDisabled
        {
            get
            {
                return (bool)this.AtomDisabledCheckBox.IsChecked;
            }
            set
            {
                this.AtomDisabledCheckBox.IsChecked = value;
            }
        }

        public string LanguageCode
		{
			get
			{
				return this.SelLangComboBox.SelectedValue as string;
			}
			set
			{
				this.SelLangComboBox.SelectedValue = value;
			}
		}

		public int VoiceType
		{
			get
			{
				int nType = 0;
				if ((bool)this.FemaleRadioButton.IsChecked) nType = 0;
				else if ((bool)this.MaleRadioButton.IsChecked) nType = 1;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.FemaleRadioButton.IsChecked = true;
						break;
					case 1:
						this.MaleRadioButton.IsChecked = true;
						break;
				}
			}
		}

		public int VoicePitch
		{
			get
			{
				return (int)this.PitchComboBox.SelectedValue;
			}
			set
			{
				this.PitchComboBox.SelectedValue = value;
			}
		}

		public int VoiceRate
		{
			get
			{
				if (this.RateComboBox.SelectedIndex == 5)
				{
					return _Kiss.toInt32 (this.RateTextBox.Text);
				}
				else
				{
					return (int)this.RateComboBox.SelectedValue;
				}
			}
			set
			{
				this.RateComboBox.SelectedValue = value;

				if (-1 == this.RateComboBox.SelectedIndex)
				{
					this.RateComboBox.SelectedIndex = 5;
					this.RateTextBox.Text = value.ToString ();
				}
			}
		}

		public int SoundVolume
		{
			get { return _Kiss.toInt32 (VolumnUpDownBox.Value); }
			set { this.VolumnUpDownBox.Value = value; }
		}

		public bool EnableTag
		{
			get
			{
				return (bool)this.TagRadioButton.IsChecked;
			}
			set
			{
				this.NoTagRadioButton.IsChecked = !value;
				this.TagRadioButton.IsChecked = value;
			}
		}

		public int InputMethod
		{
			get
			{
				int nType = 0;
				if ((bool)this.AtomRadioButton.IsChecked) nType = 0;
				else if ((bool)this.VoiceFileRadioButton.IsChecked) nType = 1;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.AtomRadioButton.IsChecked = true;
						break;
					case 1:
						this.VoiceFileRadioButton.IsChecked = true;
						break;
				}

				SetInputTextLabelStyle (value);
			}
		}

		public int OutputMethod
		{
			get
			{
				int nType = 0;
				if ((bool)this.SoundRadioButton.IsChecked) nType = 0;
				else if ((bool)this.FileRadioButton.IsChecked) nType = 1;
				else if ((bool)this.SoundFileRadioButton.IsChecked) nType = 2;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.SoundRadioButton.IsChecked = true;
						break;
					case 1:
						this.FileRadioButton.IsChecked = true;
						break;
					case 2:
						this.SoundFileRadioButton.IsChecked = true;
						break;
				}
			}
		}

		public string InputValue
		{
			get { return this.InputMethodTextBox.Text; }
			set { this.InputMethodTextBox.Text = value; }
		}

		public string OutputValue
		{
			get { return this.OutputMethodTextBox.Text; }
			set { this.OutputMethodTextBox.Text = value; }
		}

		public ObservableCollection<KeyValuePair<string, string>> LanguageCollection { get; set; }
		private void InitLanguageCollection ()
		{
			LanguageCollection = new ObservableCollection<KeyValuePair<string, string>> ()
			{
               //new KeyValuePair<string, string> ("한국어", "ko-KR"),
               //new KeyValuePair<string, string> ("영어", "en-US"),
               //new KeyValuePair<string, string> ("중국어", "zh"),//new KeyValuePair<string, string> ("중국어", "zh-CN"),
               //new KeyValuePair<string, string> ("일본어", "ja"),
               //new KeyValuePair<string, string> ("프랑스어", "fr"),
               //new KeyValuePair<string, string> ("독일어", "de"),
               //new KeyValuePair<string, string> ("스페인어", "es"),
               //new KeyValuePair<string, string> ("이태리어", "it"),
               //new KeyValuePair<string, string> ("터키어", "tr"),
               //new KeyValuePair<string, string> ("러시아어", "ru"),

               //new KeyValuePair<string, string> ("베트남", "vi"),
               //new KeyValuePair<string, string> ("태국", "th"),
               //new KeyValuePair<string, string> ("홍콩", "hk"),//new KeyValuePair<string, string> ("홍콩", "zh-HK"),

               //new KeyValuePair<string, string> ("인도네시아어", "id"),	
               //new KeyValuePair<string, string> ("영어(영국)", "en-uk"),//new KeyValuePair<string, string> ("영어(영국)", "en-GB"),	
               //new KeyValuePair<string, string> ("영어(호주)", "en-au"),
               //new KeyValuePair<string, string> ("영어(인도)", "en-in"),
               //new KeyValuePair<string, string> ("스페인어(미국)", "es-us"),
               //new KeyValuePair<string, string> ("중국어(대만)", "zh-tw"),
               //new KeyValuePair<string, string> ("방글라데시어", "bn"),
               //new KeyValuePair<string, string> ("인도어(벵골어)", "bn-in"),
               //new KeyValuePair<string, string> ("인도어(힌두어)", "hi"),
               //new KeyValuePair<string, string> ("체코어", "cs"),
               //new KeyValuePair<string, string> ("덴마크어", "da"),
               //new KeyValuePair<string, string> ("네덜란드어", "nl"),
               //new KeyValuePair<string, string> ("타갈로그어(필리핀)", "tl"),
               //new KeyValuePair<string, string> ("핀란드어", "fi"),
               //new KeyValuePair<string, string> ("그리스어", "el"),
               //new KeyValuePair<string, string> ("헝가리어", "hu"),
               //new KeyValuePair<string, string> ("크메르어(캄보디아)", "km"),
               //new KeyValuePair<string, string> ("네팔어", "ne"),
               //new KeyValuePair<string, string> ("노르웨이어", "no"),
               //new KeyValuePair<string, string> ("폴란드어", "pl"),
               //new KeyValuePair<string, string> ("포루투갈어(브라질)", "pt"),
               //new KeyValuePair<string, string> ("루마니아어", "ro"),
               //new KeyValuePair<string, string> ("싱할라어(스리랑카)", "si"),
               //new KeyValuePair<string, string> ("슬로바키아어", "sk"),
               //new KeyValuePair<string, string> ("스웨덴어", "sv"),
               //new KeyValuePair<string, string> ("우크라이나어", "uk"),
               //new KeyValuePair<string, string> ("에스토니아어", "et")

               new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_2"), "ko-KR"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_3"), "en-US"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_4"), "zh"),//new KeyValuePair<string, string> ("중국어", "zh-CN"),
               new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_5"), "ja"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_6"), "fr"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_7"), "de"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_8"), "es"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_9"), "it"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_10"), "tr"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_11"), "ru"),

			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_12"), "vi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_13"), "th"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_14"), "hk"),//new KeyValuePair<string, string> ("홍콩", "zh-HK"),

               new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_15"), "id"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_16"), "en-uk"),//new KeyValuePair<string, string> ("영어(영국)", "en-GB"),	
               new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_17"), "en-au"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_18"), "en-in"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_19"), "es-us"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_20"), "zh-tw"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_21"), "bn"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_22"), "bn-in"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_23"), "hi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_24"), "cs"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_25"), "da"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_26"), "nl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_27"), "tl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_28"), "fi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_29"), "el"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_30"), "hu"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_31"), "km"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_32"), "ne"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_33"), "no"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_34"), "pl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_35"), "pt"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_36"), "ro"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_37"), "si"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_38"), "sk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_39"), "sv"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_40"), "uk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_41"), "et")
			};
		}

		public ObservableCollection<KeyValuePair<string, int>> PitchCollection { get; set; }
		private void InitPitchCollection ()     // 음정
		{
			PitchCollection = new ObservableCollection<KeyValuePair<string, int>> ()
			{
               //new KeyValuePair<string, int> ("매우낮음", -10),
               //new KeyValuePair<string, int> ("낮음", -5),
               //new KeyValuePair<string, int> ("보통", 0),
               //new KeyValuePair<string, int> ("높음", 5),
               //new KeyValuePair<string, int> ("매우높음", 10)

               new KeyValuePair<string, int> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_42"), -10),
			   new KeyValuePair<string, int> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_43"), -5),
			   new KeyValuePair<string, int> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_44"), 0),
			   new KeyValuePair<string, int> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_45"), 5),
			   new KeyValuePair<string, int> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_46"), 10)
			};
		}

		public ObservableCollection<KeyValuePair<string, int>> RateCollection { get; set; }
		private void InitRateCollection ()     // 속도
		{
			RateCollection = new ObservableCollection<KeyValuePair<string, int>> ()
			{
               //new KeyValuePair<string, int> ("매우느리게", -10),
               //new KeyValuePair<string, int> ("느리게", -5),
               //new KeyValuePair<string, int> ("보통속도", 0),
               //new KeyValuePair<string, int> ("빠르게", 5),
               //new KeyValuePair<string, int> ("메우빠르게", 10)

               new KeyValuePair<string, int> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_47"), -10),
			   new KeyValuePair<string, int> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_48"), -5),
			   new KeyValuePair<string, int> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_49"), 0),
			   new KeyValuePair<string, int> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_50"), 5),
			   new KeyValuePair<string, int> (LC.GS("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_51"), 10),
			   new KeyValuePair<string, int> (LC.GS("TopAnimation_AnimationHelper_2"), -1)
			};
		}
		#endregion Property

		private void RateComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (5 == RateComboBox.SelectedIndex)  // 사용자지정
			{
				RateTextBox.Visibility = System.Windows.Visibility.Visible;
				CustomRateUnit.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				RateTextBox.Visibility = System.Windows.Visibility.Collapsed;
				CustomRateUnit.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void SelLangComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (3 < SelLangComboBox.SelectedIndex)  // 한국어, 영어는 가능
			{
				if (LicenseHelper.Instance.IsEnableSolutionService (SolutionService.OutsideVoice))  // 외부음원 사용 (A02)
				{
				}
				else
				{
					if (LC.PQLanguage == LC.LANG.JAPAN)
					{
						SelLangComboBox.SelectedIndex = 3;  // 일본어
					}
					else
					{
						SelLangComboBox.SelectedIndex = 0;  // 한국어
					}
				}
			}
		}

		private void TransFileSearchButton_Click (object sender, RoutedEventArgs e)
		{
			e.Handled = true;

			OpenTextFileDialog ();
		}

		private void OpenTextFileDialog ()
		{
			var openFileDialog = new System.Windows.Forms.OpenFileDialog ();

			string strFilter = "Text Files|*.txt|" +
			"Text File (*.txt)|*.txt|" +
			"All Files (*.*)|*.*";

			openFileDialog.Filter = strFilter;
			openFileDialog.FilterIndex = 1;

			if (openFileDialog.ShowDialog () == System.Windows.Forms.DialogResult.OK)
			{
				this.InputValue = PathHandler.GetRelativePath (openFileDialog.FileName);
			}
		}

		private void OutputSearchButton_Click (object sender, RoutedEventArgs e)
		{
			e.Handled = true;

			OpenTextFolderDialog ();
		}

		private void OpenTextFolderDialog ()
		{
			System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog ();
			fb.ShowNewFolderButton = true;
			if (fb.ShowDialog () == System.Windows.Forms.DialogResult.OK)
			{
				this.OutputValue = PathHandler.GetRelativePath (fb.SelectedPath) + "\\";
			}
			fb.Dispose ();
		}

		private void SetInputTextLabelStyle (int nInputMethod)
		{
			switch (nInputMethod)
			{
				case 0:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_52");
					this.TransFileSearchButton.IsEnabled = false;
					break;
				case 1:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_53");
					this.TransFileSearchButton.IsEnabled = true;
					break;
			}
		}

		private void SetOutputTextLabelStyle (int nInputMethod)
		{
			switch (nInputMethod)
			{
				case 0:
					this.OutputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_54");
					this.OutputMethodTextBox.IsEnabled = false;
					this.OutputSearchButton.IsEnabled = false;
					break;
				case 1:
					this.OutputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_55");
					this.OutputMethodTextBox.IsEnabled = true;
					this.OutputSearchButton.IsEnabled = true;
					break;
				case 2:
					this.OutputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalTTSAttPage_1808_56");
					this.OutputMethodTextBox.IsEnabled = true;
					this.OutputSearchButton.IsEnabled = true;
					break;
			}
		}

		private void SpeedComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
		}

		private void AtomRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetInputTextLabelStyle (0);
		}

		private void AtomRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void VoiceFileRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetInputTextLabelStyle (1);
		}

		private void VoiceFileRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void SoundRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetOutputTextLabelStyle (0);
		}

		private void SoundRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void FileRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetOutputTextLabelStyle (1);
		}

		private void FileRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void SoundFileRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetOutputTextLabelStyle (2);
		}

		private void SoundFileRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}
	}//end classs
}//end namespace
