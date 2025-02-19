using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.License.Manager.Join.SolutionEx;    //  라이선스 체제 변경안(v3.0)
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopApp.License;
using Softpower.SmartMaker.TopAtom.Verbal;
using Softpower.SmartMaker.TopSmartAtomEdit.AttBase;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPageStyle;

namespace Softpower.SmartMaker.TopSmartAtomEdit.AttPage
{
	/// <summary>
	/// Interaction logic for SmartVerbalTransAttPage.xaml
	/// </summary>
	public partial class SmartVerbalTransAttPage : AttBasePage
	{
		#region Initialize

		public SmartVerbalTransAttPage ()
		{
			InitSourceLanguageCollection ();
			InitTargetLanguageCollection ();

			InitializeComponent ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("SmartVerbalTransAttPage", this);
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

			this.SrcLangComboBox.ItemsSource = SourceLanguageCollection;
			this.SrcLangComboBox.DisplayMemberPath = "Key";
			this.SrcLangComboBox.SelectedValuePath = "Value";

			this.TrgLangComboBox.ItemsSource = TargetLanguageCollection;
			this.TrgLangComboBox.DisplayMemberPath = "Key";
			this.TrgLangComboBox.SelectedValuePath = "Value";

			this.OutputTtsTextBox.FilterType = typeof (VerbalTTS);
		}

		private void InitEvent ()
		{
			this.TransFileSearchButton.AddHandler (System.Windows.Controls.UserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler (TransFileSearchButton_Click), true);
			this.Loaded += SmartVerbalTransAttPage_Loaded;
		}

		private void SmartVerbalTransAttPage_Loaded (object sender, RoutedEventArgs e)
		{
			this.InputMethodTextBox.Information = this.Information;
			this.AtomNameTextBox.Information = this.Information;
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

        public ObservableCollection<KeyValuePair<string, string>> SourceLanguageCollection { get; set; }
		public ObservableCollection<KeyValuePair<string, string>> TargetLanguageCollection { get; set; }

		private void InitSourceLanguageCollection ()
		{
			SourceLanguageCollection = new ObservableCollection<KeyValuePair<string, string>> ()
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

               new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_2"), "ko"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_3"), "en"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_4"), "zh"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_5"), "ja"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_6"), "fr"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_7"), "de"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_8"), "es"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_9"), "it"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_10"), "tr"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_11"), "ru"),

				 new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_12"), "vi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_13"), "th"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_14"), "hk"),

			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_15"), "id"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_16"), "en-uk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_17"), "en-au"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_18"), "en-in"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_19"), "es-us"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_20"), "zh-tw"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_21"), "bn"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_22"), "bn-in"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_23"), "hi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_24"), "cs"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_25"), "da"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_26"), "nl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_27"), "tl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_28"), "fi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_29"), "el"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_30"), "hu"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_31"), "km"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_32"), "ne"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_33"), "no"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_34"), "pl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_35"), "pt"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_36"), "ro"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_37"), "si"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_38"), "sk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_39"), "sv"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_40"), "uk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_41"), "et")
			};
		}

		private void InitTargetLanguageCollection ()
		{
			TargetLanguageCollection = new ObservableCollection<KeyValuePair<string, string>> ()
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

               new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_42"), "ko"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_43"), "en"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_44"), "zh"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_45"), "ja"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_46"), "fr"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_47"), "de"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_48"), "es"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_49"), "it"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_50"), "tr"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_51"), "ru"),

			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_52"), "vi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_53"), "th"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_54"), "hk"),

			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_55"), "id"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_56"), "en-uk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_57"), "en-au"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_58"), "en-in"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_59"), "es-us"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_60"), "zh-tw"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_61"), "bn"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_62"), "bn-in"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_63"), "hi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_64"), "cs"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_65"), "da"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_66"), "nl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_67"), "tl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_68"), "fi"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_69"), "el"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_70"), "hu"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_71"), "km"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_72"), "ne"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_73"), "no"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_74"), "pl"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_75"), "pt"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_76"), "ro"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_77"), "si"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_78"), "sk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_79"), "sv"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_80"), "uk"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_81"), "et")
			};
		}

		public string SourceLanguage
		{
			get
			{
				return this.SrcLangComboBox.SelectedValue as string;
			}
			set
			{
				this.SrcLangComboBox.SelectedValue = value;
			}
		}

		public string TargetLanguage
		{
			get
			{
				return this.TrgLangComboBox.SelectedValue as string;
			}
			set
			{
				this.TrgLangComboBox.SelectedValue = value;
			}
		}

		public int InputMethod
		{
			get
			{
				int nType = 0;
				if ((bool)this.AtomRadioButton.IsChecked) nType = 0;
				else if ((bool)this.FileRadioButton.IsChecked) nType = 1;

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
				else if ((bool)this.TtsRadioButton.IsChecked) nType = 2;

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
						this.TtsRadioButton.IsChecked = true;
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

		public string OutputTts
		{
			get { return this.OutputTtsTextBox.Text; }
			set { this.OutputTtsTextBox.Text = value; }
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

		public bool EnableSentence
		{
			get
			{
				return (bool)this.SentenceCheckBox.IsChecked;
			}
			set
			{
				this.SentenceCheckBox.IsChecked = value;
			}
		}

		public bool EnableParagraph
		{
			get
			{
				return (bool)this.ParagraphCheckBox.IsChecked;
			}
			set
			{
				this.ParagraphCheckBox.IsChecked = value;
			}
		}

		#endregion Property

		private void AtomRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetInputTextLabelStyle (0);
		}

		private void AtomRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void FileRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetInputTextLabelStyle (1);
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

		private void TtsRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetOutputTextBoxStyle (2);
		}

		private void TtsRadioButton_Unchecked (object sender, RoutedEventArgs e)
		{

		}

		private void SetInputTextLabelStyle (int nInputMethod)
		{
			switch (nInputMethod)
			{
				case 0:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_82");
					this.TransFileSearchButton.IsEnabled = false;
					break;
				case 1:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalTransAttPage_1808_83");
					this.TransFileSearchButton.IsEnabled = true;
					break;
			}
		}

		private void SetOutputTextBoxStyle (int nOutputMethod)
		{
			switch (nOutputMethod)
			{
				case 0:
				case 1:
					Grid.SetColumn (this.OutputMethodTextBox, 0);
					Grid.SetColumnSpan (this.OutputMethodTextBox, 2);

					this.OutputTtsTextBox.Visibility = Visibility.Collapsed;

					this.OutputTtsSubLabel.Visibility = Visibility.Hidden;
					this.OutputValueSubLabel.Visibility = Visibility.Hidden;
					break;
				case 2:
					Grid.SetColumn (this.OutputTtsTextBox, 0);
					Grid.SetColumn (this.OutputMethodTextBox, 1);

					Grid.SetColumnSpan (this.OutputTtsTextBox, 1);
					Grid.SetColumnSpan (this.OutputMethodTextBox, 1);

					this.OutputTtsTextBox.Visibility = Visibility.Visible;

					this.OutputTtsSubLabel.Visibility = Visibility.Visible;
					this.OutputValueSubLabel.Visibility = Visibility.Visible;
					break;
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

	}//end classs
}//end namespace
