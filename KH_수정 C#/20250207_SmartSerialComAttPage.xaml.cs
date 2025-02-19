using System.Windows;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.TopSmartAtomEdit.AttBase;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPageStyle;

namespace Softpower.SmartMaker.TopSmartAtomEdit.AttPage
{
	/// <summary>
	/// Interaction logic for SmartSerialComAttPage.xaml
	/// </summary>
	public partial class SmartSerialComAttPage : AttBasePage
	{
		#region Initialize

		public SmartSerialComAttPage ()
		{
			InitializeComponent ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("SmartSerialComAttPage", this);
			}

			this.SetStyle ();
			this.InitEvent ();

			this.SetControlValue ();
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

			if (LC.LANG.ENGLISH == LC.PQLanguage)
			{
				WPFTextBlock2_1.FontSize = 11;
			}
		}

		private void InitEvent ()
		{
			this.Loaded += SmartSerialComAttPage_Loaded;
		}

		private void SmartSerialComAttPage_Loaded (object sender, RoutedEventArgs e)
		{
			this.InputMethodTextBox.Information = this.Information;
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

        public bool AtomDisabled
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

        public string PortName
		{
			get
			{
				return this.PortNameTextBox.Text;
			}
			set
			{
				this.PortNameTextBox.Text = value;
			}
		}

		public int BaudRate
		{
			get
			{
				return _Kiss.toInt32 (this.BaudRateTextBox.Text);
			}
			set
			{
				this.BaudRateTextBox.Text = value.ToString ();
			}
		}

		public int Parity
		{
			get
			{
				return this.ParityComboBox.SelectedIndex;
			}
			set
			{
				this.ParityComboBox.SelectedIndex = value;
			}
		}

		public int DataBits
		{
			get
			{
				return _Kiss.toInt32 (this.DataBitsTextBox.Text);
			}
			set
			{
				this.DataBitsTextBox.Text = value.ToString ();
			}
		}

		public int StopBits
		{
			get
			{
				return this.StopBitsComboBox.SelectedIndex;
			}
			set
			{
				this.StopBitsComboBox.SelectedIndex = value;
			}
		}

		public int InputMethod
		{
			get
			{
				int nType = 0;
				if ((bool)this.InputAtomRadioButton.IsChecked) nType = 0;
				else if ((bool)this.InputFileRadioButton.IsChecked) nType = 1;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.InputAtomRadioButton.IsChecked = true;
						break;
					case 1:
						this.InputFileRadioButton.IsChecked = true;
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
				if ((bool)this.OutputAtomRadioButton.IsChecked) nType = 0;
				else if ((bool)this.OutputFileRadioButton.IsChecked) nType = 1;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.OutputAtomRadioButton.IsChecked = true;
						break;
					case 1:
						this.OutputFileRadioButton.IsChecked = true;
						break;
				}

				SetOutputTextLabelStyle (value);
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

		private void SetControlValue ()
		{
			this.ParityComboBox.Items.Add (LC.GS ("None"));
			this.ParityComboBox.Items.Add (LC.GS ("Odd"));
			this.ParityComboBox.Items.Add (LC.GS ("Even"));
			this.ParityComboBox.Items.Add (LC.GS ("Mark"));
			this.ParityComboBox.Items.Add (LC.GS ("Space"));

			this.StopBitsComboBox.Items.Add (LC.GS ("None"));
			this.StopBitsComboBox.Items.Add (LC.GS ("One"));
			this.StopBitsComboBox.Items.Add (LC.GS ("Two"));
			this.StopBitsComboBox.Items.Add (LC.GS ("OnePointFive"));
		}

		private void InputRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetInputTextLabelStyle (this.InputMethod);
		}

		private void OutputRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetOutputTextLabelStyle (this.OutputMethod);
		}

		private void SetInputTextLabelStyle (int nInputMethod)
		{
			switch (nInputMethod)
			{
				case 0:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartSerialComAttPage_1808_2");
					break;
				case 1:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartSerialComAttPage_1808_3");
					break;
			}
		}

		private void SetOutputTextLabelStyle (int nInputMethod)
		{
			switch (nInputMethod)
			{
				case 0:
					this.OutputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartSerialComAttPage_1808_4");
					break;
				case 1:
					this.OutputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartSerialComAttPage_1808_5");
					break;
			}
		}
	}//end classs
}//end namespace
