using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.TopSmartAtomEdit.AttBase;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPageStyle;

namespace Softpower.SmartMaker.TopSmartAtomEdit.AttPage
{
	/// <summary>
	/// SmartNfcAdapterAttPage.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class SmartNfcAdapterAttPage : AttBasePage
	{
		public SmartNfcAdapterAttPage ()
		{
			InitializeComponent ();

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
		}

		private void InitEvent ()
		{
			this.Loaded += SmartNfcAdapterAttPage_Loaded;
		}

		private void SmartNfcAdapterAttPage_Loaded (object sender, RoutedEventArgs e)
		{
			this.InputMethodTextBox.Information = this.Information;
			this.OutputMethodTextBox.Information = this.Information;
		}

		#region | Property |

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

        public bool IsDisabled
		{
			get
			{
				return (bool)this.DisabledCheckBox.IsChecked;
			}
			set
			{
				this.DisabledCheckBox.IsChecked = value;
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

		public string SerialNumber
		{
			get { return this.SerialNumberTextBox.Text; }
			set { this.SerialNumberTextBox.Text = value; }
		}
		
		#endregion
	}
}
