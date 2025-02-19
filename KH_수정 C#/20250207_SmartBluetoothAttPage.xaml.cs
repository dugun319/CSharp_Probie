using System.Windows;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.TopSmartAtomEdit.AttBase;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPageStyle;

namespace Softpower.SmartMaker.TopSmartAtomEdit.AttPage
{
	/// <summary>
	/// Interaction logic for SmartBluetoothAttPage.xaml
	/// </summary>
	public partial class SmartBluetoothAttPage : AttBasePage
	{
		#region Initialize

		public SmartBluetoothAttPage ()
		{
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
		}

		private void InitEvent ()
		{
			this.Loaded += SmartBluetoothAttPage_Loaded;
		}

		private void SmartBluetoothAttPage_Loaded (object sender, RoutedEventArgs e)
		{
			this.InputMethodTextBox.Information = this.Information;
			this.OutputMethodTextBox.Information = this.Information;
			this.NotifyValueTextBox.Information = this.Information;
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

        public int ConnectType
		{
			get
			{
				int nType = 0;
				if ((bool)this.ConnectionType0.IsChecked) nType = 0;
				else if ((bool)this.ConnectionType1.IsChecked) nType = 1;
				else if ((bool)this.ConnectionType2.IsChecked) nType = 2;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.ConnectionType0.IsChecked = true;
						break;
					case 1:
						this.ConnectionType1.IsChecked = true;
						break;
					case 2:
						this.ConnectionType2.IsChecked = true;
						break;
				}
			}
		}

		public bool SimpleConnect
		{
			get
			{
				bool bSimpleConnect = false;
				if ((bool)this.SimpleConnectSet.IsChecked) bSimpleConnect = true;
				else if ((bool)this.SimpleConnectUnset.IsChecked) bSimpleConnect = false;

				return bSimpleConnect;
			}
			set
			{
				if (value)
				{
					this.SimpleConnectSet.IsChecked = true;
				}
				else
				{
					this.SimpleConnectUnset.IsChecked = true;
				}

				SimpleConnectStatus ();
			}
		}

		public int ProtocolType
		{
			get
			{
				int nType = 0;
				if ((bool)this.UuidRadioButton.IsChecked) nType = 0;
				else if ((bool)this.SerialRadioButton.IsChecked) nType = 1;
				else if ((bool)this.UserRadioButton.IsChecked) nType = 2;
				else if ((bool)this.BleRadioButton.IsChecked) nType = 3;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.UuidRadioButton.IsChecked = true;
						break;
					case 1:
						this.SerialRadioButton.IsChecked = true;
						break;
					case 2:
						this.UserRadioButton.IsChecked = true;
						break;
					case 3:
						this.BleRadioButton.IsChecked = true;
						break;
				}
			}
		}

		public string BluetoothUUID
		{
			get
			{
				return this.BluetoothUUIDBox.Text;
			}
			set
			{
				this.BluetoothUUIDBox.Text = value;
			}
		}

		public string BluetoothNotify
		{
			get
			{
				return this.NotifyValueTextBox.Text;
			}
			set
			{
				this.NotifyValueTextBox.Text = value;
			}
		}

		public int PairingType
		{
			get
			{
				int nType = 0;
				if ((bool)this.AndroidRadioButton.IsChecked) nType = 0;
				else if ((bool)this.AppRadioButton.IsChecked) nType = 1;
				else if ((bool)this.AutoRadioButton.IsChecked) nType = 2;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.AndroidRadioButton.IsChecked = true;
						break;
					case 1:
						this.AppRadioButton.IsChecked = true;
						break;
					case 2:
						this.AutoRadioButton.IsChecked = true;
						break;
				}
			}
		}

		public string BluetoothMAC
		{
			get
			{
				return this.BluetoothMACBox.Text;
			}
			set
			{
				this.BluetoothMACBox.Text = value;
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

		private void ProtocolTypeRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SetProtocolType ();
		}

		private void SetProtocolType ()
		{
			switch (this.ProtocolType)
			{
				case 0:
					{
						BluetoothUUIDBox.IsEnabled = false;
						this.BluetoothUUID = "00000003-0000-1000-8000-00805f9b34fb";  //"00000000-0000-1000-8000-00805F9B34FB";
						this.NotifyLabel.Visibility = Visibility.Collapsed;
						this.NotifyValueTextBox.Visibility = Visibility.Collapsed;
					}
					break;
				case 1:
					{
						BluetoothUUIDBox.IsEnabled = false;
						this.BluetoothUUID = "00001101-0000-1000-8000-00805F9B34FB";
						this.NotifyLabel.Visibility = Visibility.Collapsed;
						this.NotifyValueTextBox.Visibility = Visibility.Collapsed;
					}
					break;
				case 2:
					{
						BluetoothUUIDBox.IsEnabled = true;
						this.NotifyLabel.Visibility = Visibility.Collapsed;
						this.NotifyValueTextBox.Visibility = Visibility.Collapsed;
					}
					break;
				case 3:
					{
						BluetoothUUIDBox.IsEnabled = true;
						this.NotifyLabel.Visibility = Visibility.Visible;
						this.NotifyValueTextBox.Visibility = Visibility.Visible;
					}
					break;
			}
		}

		private void SimpleConnectStatus ()
		{
			this.GroupBox2.IsEnabled = !this.SimpleConnect;
			this.GroupBox3.IsEnabled = !this.SimpleConnect;

			SetProtocolType ();
		}

		private void SimpleConnectRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			SimpleConnectStatus ();
		}

		private void ConnectTypeRadioButton_Checked (object sender, RoutedEventArgs e)
		{
		}

		private void PairingTypeRadioButton_Checked (object sender, RoutedEventArgs e)
		{
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
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartBluetoothAttPage_1808_2");
					break;
				case 1:
					this.InputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartBluetoothAttPage_1808_3");
					break;
			}
		}

		private void SetOutputTextLabelStyle (int nInputMethod)
		{
			switch (nInputMethod)
			{
				case 0:
					this.OutputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartBluetoothAttPage_1808_4");
					break;
				case 1:
					this.OutputMethodLabel.Text = LC.GS ("TopSmartAtomEdit_SmartBluetoothAttPage_1808_5");
					break;
			}
		}
	}//end classs
}//end namespace
