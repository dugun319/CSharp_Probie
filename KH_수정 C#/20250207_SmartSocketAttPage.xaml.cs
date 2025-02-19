using System.Windows;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.TopSmartAtomEdit.AttBase;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPageStyle;

namespace Softpower.SmartMaker.TopSmartAtomEdit.AttPage
{
	/// <summary>
	/// Interaction logic for SmartSocketAttPage.xaml
	/// </summary>
	public partial class SmartSocketAttPage : AttBasePage
	{
		#region Initialize

		public SmartSocketAttPage ()
		{
			InitializeComponent ();

			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("SmartSocketAttPage", this);
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

			if (LC.LANG.ENGLISH == LC.PQLanguage)
			{
				WPFTextBlock2_1.FontSize = 11;
			}
		}

		private void InitEvent ()
		{
			this.Loaded += SmartSocketAttPage_Loaded;
		}

		private void SmartSocketAttPage_Loaded (object sender, RoutedEventArgs e)
		{
			this.IPAddressTextBox.Information = this.Information;
			this.PortNumberTextBox.Information = this.Information;

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

        public int SocketType
		{
			get
			{
				int nType = 0;
				if ((bool)this.SocketClientRadioButton.IsChecked) nType = 0;
				else if ((bool)this.SocketServerRadioButton.IsChecked) nType = 1;

				return nType;
			}
			set
			{
				switch (value)
				{
					case 0:
						this.SocketClientRadioButton.IsChecked = true;
						break;
					case 1:
						this.SocketServerRadioButton.IsChecked = true;
						break;
				}
			}
		}

		public string IPAddress
		{
			get
			{
				return this.IPAddressTextBox.Text;
			}
			set
			{
				this.IPAddressTextBox.Text = value;
			}
		}

		public string PortNumber
		{
			get
			{
				return this.PortNumberTextBox.Text;
			}
			set
			{
				this.PortNumberTextBox.Text = value;
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

		private void SocketRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			IPAddressTextBox.IsEnabled = (0 == SocketType) ? true : false;
		}


	}//end classs
}//end namespace
