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
using Softpower.SmartMaker.TopAtom.Verbal;
using Softpower.SmartMaker.TopAtom.Verbal.Voice;
using Softpower.SmartMaker.TopSmartAtomEdit.AttBase;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPageStyle;
using Softpower.SmartMaker.TopSmartAtomEdit.SubControls.Voice;

namespace Softpower.SmartMaker.TopSmartAtomEdit.AttPage
{
	/// <summary>
	/// Interaction logic for SmartVerbalSTTAttPage.xaml
	/// </summary>
	public partial class SmartVerbalSTTAttPage : AttBasePage
	{
		#region Initialize

		private List<VoiceCommand> m_listVoiceCommand = new List<VoiceCommand> ();

		public List<VoiceCommand> VoiceCommandItems
		{
			get { return m_listVoiceCommand; }
			set { m_listVoiceCommand = value; }
		}

		public SmartVerbalSTTAttPage ()
		{
			InitLanguageCollection ();

			InitializeComponent ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("SmartVerbalSTTAttPage", this);
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

			this.OutputTransTextBox.FilterType = typeof (VerbalTrans);

			if (LC.LANG.ENGLISH == LC.PQLanguage)
			{
				Grid.SetColumn (VoiceCommandButton, 2);
				Grid.SetColumnSpan (VoiceCommandButton, 2);

				VoiceCommandButton.HorizontalAlignment = HorizontalAlignment.Right;

				VoiceCommandButton.Width = 105;
			}
		}

		private void InitEvent ()
		{
			this.VoiceCommandButton.AddHandler (System.Windows.Controls.UserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler (buttonVoiceCommand_Click), true);
			this.InputSearchButton.AddHandler (System.Windows.Controls.UserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler (inputSearchButton_Click), true);
			this.Loaded += SmartVerbalSTTAttPage_Loaded;
		}

		private void SmartVerbalSTTAttPage_Loaded (object sender, RoutedEventArgs e)
		{
			this.InputMethodTextBox.Information = this.Information;
			this.OutputTransTextBox.Information = this.Information;
			this.OutputMethodTextBox.Information = this.Information;
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

        public int InputMethod
		{
			get
			{
				int nType = 0;
				if ((bool)this.MicRadioButton.IsChecked) nType = 0;
				else if ((bool)this.AtomRadioButton.IsChecked) nType = 1;
				else if ((bool)this.FileRadioButton.IsChecked) nType = 2;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.MicRadioButton.IsChecked = true;
						break;
					case 1:
						this.AtomRadioButton.IsChecked = true;
						break;
					case 2:
						this.FileRadioButton.IsChecked = true;
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
				if ((bool)this.TextRadioButton.IsChecked) nType = 0;
				else if ((bool)this.CommandRadioButton.IsChecked) nType = 1;
				else if ((bool)this.TransRadioButton.IsChecked) nType = 2;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.TextRadioButton.IsChecked = true;
						break;
					case 1:
						this.CommandRadioButton.IsChecked = true;
						break;
					case 2:
						this.TransRadioButton.IsChecked = true;
						break;
				}

				SetOutputTextBoxStyle (value);
			}
		}

		public int FromDecibel
		{
			get { return _Kiss.toInt32 (this.FromDecibelTextBox.Text); }
			set { this.FromDecibelTextBox.Text = _Kiss.toString (value); }
		}

		public int ToDecibel
		{
			get { return _Kiss.toInt32 (this.ToDecibelTextBox.Text); }
			set { this.ToDecibelTextBox.Text = _Kiss.toString (value); }
		}

		public int DelayTime
		{
			get { return _Kiss.toInt32 (this.DelayTextBox.Text); }
			set { this.DelayTextBox.Text = _Kiss.toString (value); }
		}

		public bool RepeatRecognize
		{
			get { return this.RepeatRecognizeCheckBox.IsChecked; }
			set { this.RepeatRecognizeCheckBox.IsChecked = value; }
		}

		public bool ScreenTouch
		{
			get { return this.ScreenTouchCheckBox.IsChecked; }
			set { this.ScreenTouchCheckBox.IsChecked = value; }
		}

		public bool ShakeDevice
		{
			get { return this.ShakeDeviceCheckBox.IsChecked; }
			set { this.ShakeDeviceCheckBox.IsChecked = value; }
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

		public string OutputTrans
		{
			get { return this.OutputTransTextBox.Text; }
			set { this.OutputTransTextBox.Text = value; }
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

		#endregion Property

		public ObservableCollection<KeyValuePair<string, string>> LanguageCollection { get; set; }

        private void InitLanguageCollection ()
		{
			LanguageCollection = new ObservableCollection<KeyValuePair<string, string>> ()
			{
               //new KeyValuePair<string, string> ("한국어", "ko"),
               //new KeyValuePair<string, string> ("영어", "en"),
               //new KeyValuePair<string, string> ("중국어", "zh"),
               //new KeyValuePair<string, string> ("일본어", "ja"),
               //new KeyValuePair<string, string> ("프랑스어", "fr"),
               //new KeyValuePair<string, string> ("독일어", "de"),
               //new KeyValuePair<string, string> ("스페인어", "es"),
               //new KeyValuePair<string, string> ("이태리어", "it"),
               //new KeyValuePair<string, string> ("터키어", "tr"),
               //new KeyValuePair<string, string> ("러시아어", "ru"),

               //new KeyValuePair<string, string> ("베트남", "vi"),
               //new KeyValuePair<string, string> ("태국", "th"),
               //new KeyValuePair<string, string> ("홍콩", "hk"),

               //new KeyValuePair<string, string> ("인도네시아어", "id"),	
               //new KeyValuePair<string, string> ("영어(영국)", "en-uk"),	
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

               new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_2"), "ko"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_3"), "en"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_4"), "zh"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_5"), "ja"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_6"), "fr"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_7"), "de"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_8"), "es"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_9"), "it"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_10"), "tr"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_11"), "ru"),

				 new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_12"), "vi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_13"), "th"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_14"), "hk"),

			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_15"), "id"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_16"), "en-uk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_17"), "en-au"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_18"), "en-in"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_19"), "es-us"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_20"), "zh-tw"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_21"), "bn"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_22"), "bn-in"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_23"), "hi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_24"), "cs"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_25"), "da"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_26"), "nl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_27"), "tl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_28"), "fi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_29"), "el"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_30"), "hu"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_31"), "km"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_32"), "ne"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_33"), "no"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_34"), "pl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_35"), "pt"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_36"), "ro"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_37"), "si"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_38"), "sk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_39"), "sv"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_40"), "uk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_41"), "et")
			};
		}

		private void MicRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetInputTextLabelStyle (0);
		}

		private void MicRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void AtomRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetInputTextLabelStyle (1);
		}

		private void AtomRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void FileRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetInputTextLabelStyle (2);
		}

		private void FileRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void TextRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetOutputTextBoxStyle (0);
		}

		private void TextRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void CommandRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			if (false != CommandRadioButton.IsChecked)
			{
				if (LicenseHelper.Instance.IsEnableSolutionService (SolutionService.VoiceCommand))
				{
				}
				else
				{
					TextRadioButton.IsChecked = true;
					return;
				}
			}

			SetOutputTextBoxStyle (1);
		}

		private void CommandRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void TransRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetOutputTextBoxStyle (2);
		}

		private void TransRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void SetInputTextLabelStyle (int nInputMethod)
		{
			switch (nInputMethod)
			{
				case 0:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_42");
					this.InputMethodTextBox.IsEnabled = false;
					this.InputSearchButton.IsEnabled = false;
					break;
				case 1:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_42");
					this.InputMethodTextBox.IsEnabled = true;
					this.InputSearchButton.IsEnabled = false;
					break;
				case 2:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalSTTAttPage_1808_43");
					this.InputMethodTextBox.IsEnabled = true;
					this.InputSearchButton.IsEnabled = true;
					break;
			}
		}

		private void SetOutputTextBoxStyle (int nOutputMethod)
		{
			switch (nOutputMethod)
			{
				case 0:
					Grid.SetColumn (this.OutputMethodTextBox, 0);
					Grid.SetColumnSpan (this.OutputMethodTextBox, 2);

					this.OutputTransTextBox.Visibility = Visibility.Collapsed;

					this.OutputTransSubLabel.Visibility = Visibility.Hidden;
					this.OutputValueSubLabel.Visibility = Visibility.Hidden;

					this.VoiceCommandButton.Visibility = Visibility.Collapsed;
					break;
				case 1:
					Grid.SetColumn (this.OutputMethodTextBox, 0);
					Grid.SetColumnSpan (this.OutputMethodTextBox, 2);

					this.OutputTransTextBox.Visibility = Visibility.Collapsed;

					this.OutputTransSubLabel.Visibility = Visibility.Hidden;
					this.OutputValueSubLabel.Visibility = Visibility.Hidden;

					this.VoiceCommandButton.Visibility = Visibility.Visible;
					break;
				case 2:
					Grid.SetColumn (this.OutputTransTextBox, 0);
					Grid.SetColumn (this.OutputMethodTextBox, 1);

					Grid.SetColumnSpan (this.OutputTransTextBox, 1);
					Grid.SetColumnSpan (this.OutputMethodTextBox, 1);

					this.OutputTransTextBox.Visibility = Visibility.Visible;

					this.OutputTransSubLabel.Visibility = Visibility.Visible;
					this.OutputValueSubLabel.Visibility = Visibility.Visible;

					this.VoiceCommandButton.Visibility = Visibility.Collapsed;

					break;
			}
		}

		private void inputSearchButton_Click (object sender, RoutedEventArgs e)
		{
			e.Handled = true;

			OpenAudioFileDialog ();
		}

		private void OpenAudioFileDialog ()
		{
			var openFileDialog = new System.Windows.Forms.OpenFileDialog ();

			string strFilter = "Voice Files|*.wav;*.mp3|" +
			"WAV Files (*.wav)|*.wav|" +
			"AVI Files (*.avi)|*.avi|" +
			"MP3 Files (*.mp3)|*.mp3|" +
			"All Files (*.*)|*.*";

			openFileDialog.Filter = strFilter;
			openFileDialog.FilterIndex = 1;

			if (openFileDialog.ShowDialog () == System.Windows.Forms.DialogResult.OK)
			{
				this.InputValue = PathHandler.GetRelativePath (openFileDialog.FileName);
			}
		}

		private void buttonVoiceCommand_Click (object sender, RoutedEventArgs e)
		{
			e.Handled = true;

			ShowVoiceCommandWindow ();
		}

		private void ShowVoiceCommandWindow ()
		{
			VoiceCommandWindow commandWindow = new VoiceCommandWindow ();

			commandWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

			CloneObject.CloneListObject (this.VoiceCommandItems, commandWindow.VoiceCommandItems);

			if (false != commandWindow.ShowDialog ())
			{
				CloneObject.CloneListObject (commandWindow.VoiceCommandItems, this.VoiceCommandItems);
			}
		}

	}//end classs
}//end namespace
