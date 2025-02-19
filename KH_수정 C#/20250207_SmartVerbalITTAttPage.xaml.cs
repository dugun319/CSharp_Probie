using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.License.Manager.Join.SolutionEx;
using Softpower.SmartMaker.TopApp;    //  라이선스 체제 변경안(v3.0)
using Softpower.SmartMaker.TopApp.License;
using Softpower.SmartMaker.TopSmartAtomEdit.AttBase;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPageStyle;

namespace Softpower.SmartMaker.TopSmartAtomEdit.AttPage
{
	/// <summary>
	/// Interaction logic for SmartVerbalITTAttPage.xaml
	/// </summary>
	public partial class SmartVerbalITTAttPage : AttBasePage
	{
		#region Initialize

		public SmartVerbalITTAttPage ()
		{
			InitializeComponent ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("SmartVerbalITTAttPage", this);

				if (LC.LANG.JAPAN == LC.PQLanguage)
				{
					GridContent4.ColumnDefinitions[1].Width = new GridLength (1, GridUnitType.Auto);
					GridContent4.ColumnDefinitions[2].Width = new GridLength (1, GridUnitType.Auto);
					GridContent4.ColumnDefinitions[3].Width = new GridLength (1, GridUnitType.Auto);
					GridContent4.ColumnDefinitions[4].Width = new GridLength (1, GridUnitType.Auto);
				}
			}

			InitDetectionTypeCollection ();

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
			this.GroupBox3.Style = AttPageStyleManager.StyleManager.GetAttPageGroupBoxStyle ();
			this.GroupBox4.Style = AttPageStyleManager.StyleManager.GetAttPageGroupBoxStyle ();
			this.GroupBox5.Style = AttPageStyleManager.StyleManager.GetAttPageGroupBoxStyle ();

			this.DetectionTypeComboBox.ItemsSource = DetectionTypeCollection;
			this.DetectionTypeComboBox.DisplayMemberPath = "Key";
			this.DetectionTypeComboBox.SelectedValuePath = "Value";
		}

		private void InitEvent ()
		{
			this.InputSearchButton.AddHandler (System.Windows.Controls.UserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler (InputSearchButton_Click), true);
			this.Loaded += SmartVerbalITTAttPage_Loaded;
		}

		private void SmartVerbalITTAttPage_Loaded (object sender, RoutedEventArgs e)
		{
			this.InputMethodTextBox.Information = this.Information;
			this.OutputTtsTextBox.Information = this.Information;
			this.OutputMethodTextBox.Information = this.Information;
		}

		public ObservableCollection<KeyValuePair<string, string>> DetectionTypeCollection { get; set; }
		private void InitDetectionTypeCollection ()
		{
			DetectionTypeCollection = new ObservableCollection<KeyValuePair<string, string>> ()
			{
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalITTAttPage_1808_2"), "TEXT_DETECTION"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalITTAttPage_1808_3"), "LABEL_DETECTION"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalITTAttPage_1808_4"), "FACE_DETECTION"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalITTAttPage_1808_5"), "LANDMARK_DETECTION"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalITTAttPage_1808_6"), "LOGO_DETECTION"),
			   new KeyValuePair<string, string> (LC.GS("TopSmartAtomEdit_SmartVerbalITTAttPage_1808_7"), "WEB_DETECTION"),
			};
		}

		#endregion Initialize

		private void InputRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetInputTextLabelStyle (this.InputMethod);
			SetInputTextBoxEnable (this.InputMethod);
		}

		private void TextRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetOutputTextBoxEnable (this.OutputMethod);
			SetOutputTextBoxStyle (1);
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

			SetOutputTextBoxEnable (this.OutputMethod);
			SetOutputTextBoxStyle (1);
		}

		private void TtsRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetOutputTextBoxEnable (this.OutputMethod);
			SetOutputTextBoxStyle (2);
		}

		private void OutputRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			if (false != VoiceRadioButton.IsChecked)
			{
				if (LicenseHelper.Instance.IsEnableSolutionService (SolutionService.VoiceCommand))
				{
				}
				else
				{
					OutputMethod = 0;
					return;
				}
			}

			SetOutputTextBoxEnable (this.OutputMethod);
			SetOutputTextBoxStyle (1);
		}

		private void SetInputTextLabelStyle (int nInputMethod)
		{
			switch (nInputMethod)
			{
				case 0:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalITTAttPage_1808_8");
					break;
				case 1:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalITTAttPage_1808_9");
					break;
				case 2:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalITTAttPage_1808_10");
					break;
				case 3:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartVerbalITTAttPage_1808_11");
					break;
			}
		}

		private void SetInputTextBoxEnable (int nInputMethod)
		{
			switch (nInputMethod)
			{
				case 0:
					this.InputMethodTextBox.IsEnabled = false;
					this.InputSearchButton.IsEnabled = false;
					break;
				case 1:
					this.InputMethodTextBox.IsEnabled = true;
					this.InputSearchButton.IsEnabled = false;
					break;
				case 2:
					this.InputMethodTextBox.IsEnabled = true;
					this.InputSearchButton.IsEnabled = true;
					break;
				case 3:
					this.InputMethodTextBox.IsEnabled = true;
					this.InputSearchButton.IsEnabled = false;
					break;
			}
		}

		private void SetOutputTextBoxEnable (int nInputMethod)
		{
			switch (nInputMethod)
			{
				case 0:
					this.OutputMethodTextBox.IsEnabled = true;
					break;
				case 1:
					this.OutputMethodTextBox.IsEnabled = true;
					break;
				case 2:
					this.OutputMethodTextBox.IsEnabled = true;
					break;
				case 3:
					this.OutputMethodTextBox.IsEnabled = true;
					break;
			}
		}
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

        public string ApiKey
		{
			get
			{
				return this.APIKeyTextBox.Text;
			}
			set
			{
				this.APIKeyTextBox.Text = value;
			}
		}

		public string DetectionType
		{
			get
			{
				return this.DetectionTypeComboBox.SelectedValue as string;
			}
			set
			{
				this.DetectionTypeComboBox.SelectedValue = value;
			}
		}

		public int InputMethod
		{
			get
			{
				int nType = 0;
				if ((bool)this.CameraRadioButton.IsChecked) nType = 0;
				else if ((bool)this.AtomRadioButton.IsChecked) nType = 1;
				else if ((bool)this.FileRadioButton.IsChecked) nType = 2;
				else if ((bool)this.UrlRadioButton.IsChecked) nType = 3;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.CameraRadioButton.IsChecked = true;
						break;
					case 1:
						this.AtomRadioButton.IsChecked = true;
						break;
					case 2:
						this.FileRadioButton.IsChecked = true;
						break;
					case 3:
						this.UrlRadioButton.IsChecked = true;
						break;
				}

				SetInputTextLabelStyle (value);
				SetInputTextBoxEnable (value);
			}
		}

		public int OutputMethod
		{
			get
			{
				int nType = 0;
				if ((bool)this.TextRadioButton.IsChecked) nType = 0;
				else if ((bool)this.CommandRadioButton.IsChecked) nType = 1;
				else if ((bool)this.VoiceRadioButton.IsChecked) nType = 2;
				else if ((bool)this.SearchRadioButton.IsChecked) nType = 3;

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
						this.VoiceRadioButton.IsChecked = true;
						break;
					case 3:
						this.SearchRadioButton.IsChecked = true;
						break;
				}

				SetOutputTextBoxEnable (value);
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

		#endregion Property

		private void SetOutputTextBoxStyle (int nOutputMethod)
		{
			switch (nOutputMethod)
			{
				case 0:
				case 1:
					Grid.SetColumn (this.OutputMethodTextBox, 1);
					Grid.SetColumnSpan (this.OutputMethodTextBox, 4);

					this.OutputTtsTextBox.Visibility = Visibility.Collapsed;

					this.OutputTtsSubLabel.Visibility = Visibility.Hidden;
					this.OutputValueSubLabel.Visibility = Visibility.Hidden;
					break;
				case 2:
					Grid.SetColumn (this.OutputTtsTextBox, 1);
					Grid.SetColumn (this.OutputMethodTextBox, 3);

					Grid.SetColumnSpan (this.OutputTtsTextBox, 2);
					Grid.SetColumnSpan (this.OutputMethodTextBox, 2);

					this.OutputTtsTextBox.Visibility = Visibility.Visible;

					this.OutputTtsSubLabel.Visibility = Visibility.Visible;
					this.OutputValueSubLabel.Visibility = Visibility.Visible;
					break;
			}
		}

		private void InputSearchButton_Click (object sender, RoutedEventArgs e)
		{
			e.Handled = true;

			OpenTextFileDialog ();
		}

		private void OpenTextFileDialog ()
		{
			var openFileDialog = new System.Windows.Forms.OpenFileDialog ();

			string strFilter = "All Files (*.*)|*.*";

			openFileDialog.Filter = strFilter;
			openFileDialog.FilterIndex = 1;

			if (openFileDialog.ShowDialog () == System.Windows.Forms.DialogResult.OK)
			{
				this.InputValue = PathHandler.GetRelativePath (openFileDialog.FileName);
			}
		}
	}//end classs
}//end namespace
