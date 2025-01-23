using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopSmartWebAtomEdit.AttBase;

namespace Softpower.SmartMaker.TopSmartWebAtomEdit.AttPage
{
	public partial class SmartWebPGPayPageEx : AttWebBasePage
	{
		private bool m_bIsWebModel = false;
		private bool m_bIsEBookModel = false;

		public int PaymentType    //  // 1: WebPG, 2: WebKakaoPay, 3: InAppBilling(구글인앱), 4: 해외결제(PaddleInApp), 5: Paypal, 6: IamPort, 7: SBAtom
        {
			get
			{
				if (PGAtomRadioButton.IsChecked.Value)
					return 1;
				else if (KakaoRadioButton.IsChecked.Value)
					return 2;
                else if (InAppRadioButton.IsChecked.Value)
                    return 3;
                else if (PaddleRadioButton.IsChecked.Value)
					return 4;
				else if (PaypalRadioButton.IsChecked.Value)
					return 5;
				else if (IamPortRadioButton.IsChecked.Value)
					return 6;
                else if (SBAtomRadioButton.IsChecked.Value)
                    return 7;

                return 0;
			}
			set
			{
				
				PGAtomRadioButton.IsChecked = (value == 1) ? true : false;
				KakaoRadioButton.IsChecked = (value == 2) ? true : false;
                InAppRadioButton.IsChecked = (value == 3) ? true : false;
				PaddleRadioButton.IsChecked = (value == 4) ? true : false;
				PaypalRadioButton.IsChecked = (value == 5) ? true : false;
				IamPortRadioButton.IsChecked = (value == 6) ? true : false;
                SBAtomRadioButton.IsChecked = (value == 7) ? true : false;

                VisiblaPaymentType ();
			}
		}

		#region 속성변수 InAppBilling

		public int InAppSubscription
		{
			get
			{
				if (InAppNormalRadio.IsChecked.Value)
					return 0;
				else if (InAppSubscriptionRadio.IsChecked.Value)
					return 1;

				return 0;
			}
			set
			{
				InAppNormalRadio.IsChecked = (value == 0) ? true : false;
				InAppSubscriptionRadio.IsChecked = (value == 1) ? true : false;
			}
		}

		public string ProductId     //  상품아이디
		{
			get { return textProductId.Text; }
			set { textProductId.Text = value; }
		}

		public bool IsContinuity    //  재구매 가능 여부 
		{
			get { return (bool)ContinuityRadioButton.IsChecked ? true : false; }
			set
			{
				ContinuityRadioButton.IsChecked = value;
				ContinuityRadioButton1.IsChecked = !value;
			}
		}

		public string LicenseKey
		{
			get { return textLicenseKey.Text; }
			set { textLicenseKey.Text = value; }
		}

		#endregion

		#region 속성변수 PGBilling

		public string SID           // 상점아이디
		{
			get { return textSID.Text; }
			set { textSID.Text = value; }
		}

		public string SID_R           // 상점아이디_정기
		{
			get { return textSID_R.Text; }
			set { textSID_R.Text = value; }
		}

		public string SignKey       // SignKey
		{
			get { return textSignKey.Text; }
			set { textSignKey.Text = value; }
		}

		public string SignKey_R       // SignKey_정기
		{
			get { return textSignKey_R.Text; }
			set { textSignKey_R.Text = value; }
		}

		public string Money         // 거래금액
		{
			get { return textPG03.Text; }
			set { textPG03.Text = value; }
		}

		public string UName         // 구매자명
		{
			get { return textPG04.Text; }
			set { textPG04.Text = value; }
		}

		public string ProName       // 상품명
		{
			get { return textPG05.Text; }
			set { textPG05.Text = value; }
		}

		public string HP            // 휴대폰번호
		{
			get { return textPG06.Text; }
			set { textPG06.Text = value; }
		}

		public string PGPayment    // 결제수단
		{
			get { return textPG07.Text; }
			set { textPG07.Text = value; }
		}

		public string HPPMethod
		{
			get { return textPG08.Text; }
			set { textPG08.Text = value; }
		}

		//public int PGPaymentType    //  결제수단 - 신용카드, 핸드폰
		//{
		//    get { return (bool)CardRadioButton.IsChecked ? 0 : 1; }
		//    set
		//    {
		//        CardRadioButton.IsChecked = (value == 0) ? true : false;
		//        HPRadioButton.IsChecked = (value == 1) ? true : false;
		//    }
		//}


		// 이니시스 정기결제 
		// 0:단건결제, 1:정기결제
		public int RegularType
		{
			get
			{
				if (SimpleRadioButton.IsChecked.Value)
					return 0;
				else if (RegularRadioButton.IsChecked.Value)
					return 1;

				return 0;
			}
			set
			{
				SimpleRadioButton.IsChecked = (value == 0) ? true : false;
				RegularRadioButton.IsChecked = (value == 1) ? true : false;

				RegularVisibleStatus ();
			}
		}

		public string BillingKey
		{
			get { return this.textBillingKey.Text; }
			set { this.textBillingKey.Text = value; }
		}

		public string ApiKey
		{
			get { return this.textApiKey.Text; }
			set { this.textApiKey.Text = value; }
		}

		public string ApiIv
		{
			get { return this.textApiIv.Text; }
			set { this.textApiIv.Text = value; }
		}

		//  카드번호 
		public string CardNumber
		{
			get { return this.textCardNumber.Text; }
			set { this.textCardNumber.Text = value; }
		}

		//  카드유효기간(YYMM) 
		public string CardExpireDate
		{
			get { return this.textCardExpireDate.Text; }
			set { this.textCardExpireDate.Text = value; }
		}

		//  카드비밀번호(앞2자리) 
		public string CardSmartMakerCode
		{
			get { return this.textCardPassword.Text; }
			set { this.textCardPassword.Text = value; }
		}

		//  생년월일(YYMMDD) 
		public string RegNo
		{
			get { return this.textRegNo.Text; }
			set { this.textRegNo.Text = value; }
		}

		//  EMail
		public string Email
		{
			get { return this.textEmail.Text; }
			set { this.textEmail.Text = value; }
		}


		// 0:1일,1:1달,21년
		public int RegularCycle
		{
			get
			{
				if (CycleDayRadioButton.IsChecked.Value)
					return 0;
				else if (CycleMonthRadioButton.IsChecked.Value)
					return 1;
				else if (CycleYearRadioButton.IsChecked.Value)
					return 2;

				return 0;
			}
			set
			{
				CycleDayRadioButton.IsChecked = (value == 0) ? true : false;
				CycleMonthRadioButton.IsChecked = (value == 1) ? true : false;
				CycleYearRadioButton.IsChecked = (value == 2) ? true : false;

				RegularVisibleStatus ();
			}
		}

		#endregion

		#region 속성변수 KakaoBilling

		public string CID           // 가맹점 코드
		{
			get { return textKakao01.Text; }
			set { textKakao01.Text = value; }
		}

		public string APIKEY        // REST API KEY
		{
			get { return textKakao02.Text; }
			set { textKakao02.Text = value; }
		}

		public string KakaoProName  // 상품명
		{
			get { return textKakao03.Text; }
			set { textKakao03.Text = value; }
		}

		public string KakaoMoney    // 상품 금액
		{
			get { return textKakao04.Text; }
			set { textKakao04.Text = value; }
		}

		public string KakaoUID      // 가맹점 회원 id
		{
			get { return textKakao05.Text; }
			set { textKakao05.Text = value; }
		}

		#endregion

		#region 속성변수 SBBilling

		public string MerchantID
		{
			get { return textSB001.Text; }
			set { textSB001.Text = value; }
		}

		public string ServiceID
		{
			get { return textSB002.Text; }
			set { textSB002.Text = value; }
		}

		public string HashCode
		{
			get { return textSB003.Text; }
			set { textSB003.Text = value; }
		}

		public string CustCode
		{
			get { return textSB004.Text; }
			set { textSB004.Text = value; }
		}

		public string Amount
		{
			get { return textSB005.Text; }
			set { textSB005.Text = value; }
		}

		public string ItemID
		{
			get { return textSB006.Text; }
			set { textSB006.Text = value; }
		}
		#endregion

		#region 속성변수 PaddleInApp

		public string PaddleVendorID      // 가맹점ID
		{
			get { return textPaddleVendorID.Text; }
			set { textPaddleVendorID.Text = value; }
		}

		public string PaddleAPIKey      // API Key
		{
			get { return textPaddleAPIKey.Text; }
			set { textPaddleAPIKey.Text = value; }
		}

		public string PaddleProductID   // 상품ID
		{
			get { return textPaddleProductID.Text; }
			set { textPaddleProductID.Text = value; }
		}

		public string PaddleEmail  // 메일
		{
			get { return textPaddleEmail.Text; }
			set { textPaddleEmail.Text = value; }
		}

		#endregion

		#region | PayPal Property |

		public int PayPalPaymentType
		{
			get
			{
				return true == PayPalNormalRadio.IsChecked ? 0 : 1;
			}
			set
			{
				if (0 == value)
				{
					PayPalNormalRadio.IsChecked = true;
				}
				else
				{
					PayPalSubscriptionRadio.IsChecked = true;
				}
			}
		}

		public string PayPalID
		{
			get
			{
				if (true == PayPalNormalRadio.IsChecked)
				{
					return PayPalIDTextBox.Text;
				}
				else
				{
					return PayPalIDTextBox2.Text;
				}

			}
			set
			{
				PayPalIDTextBox.Text = value;
				PayPalIDTextBox2.Text = value;
			}
		}

		public string PayPalSmartMakerCode
		{
			get
			{
				if (true == PayPalNormalRadio.IsChecked)
				{
					return PayPalPasswordBox.Text;
				}
				else
				{
					return PayPalPasswordBox2.Text;
				}
			}
			set
			{
				PayPalPasswordBox.Text = value;
				PayPalPasswordBox2.Text = value;
			}
		}

		public string PayPalProductName
		{
			get { return PayPalProductNameBox.Text; }
			set { PayPalProductNameBox.Text = value; }
		}

		public string PayPalCurrencyCode
		{
			get { return PayPalCurrencyCodesBox.Text; }
			set { PayPalCurrencyCodesBox.Text = value; }
		}

		public string PayPalMoney
		{
			get { return PayPalMoneyBox.Text; }
			set { PayPalMoneyBox.Text = value; }
		}

		public string PayPalUserName
		{
			get
			{
				if (false == PayPalSubscriptionRadio.IsChecked)
				{
					return PayPalUserNameBox.Text;
				}
				else
				{
					return PayPalUserNameBox2.Text;
				}
			}
			set
			{
				PayPalUserNameBox.Text = value;
				PayPalUserNameBox2.Text = value;
			}
		}

		public string PayPalPhoneNumber
		{
			get
			{
				if (false == PayPalSubscriptionRadio.IsChecked)
				{
					return PayPalPhoneNumberBox.Text;
				}
				else
				{
					return PayPalPhoneNumberBox2.Text;
				}
			}
			set
			{
				PayPalPhoneNumberBox.Text = value;
				PayPalPhoneNumberBox2.Text = value;
			}
		}

		//public string PayPalEMail
		//{
		//    get
		//    {
		//        if (false == PayPalSubscriptionRadio.IsChecked)
		//        {
		//            return PayPalEMailBox.Text;
		//        }
		//        else
		//        {
		//            return PayPalEMailBox2.Text;
		//        }
		//    }
		//    set
		//    {
		//        PayPalEMailBox.Text = value;
		//        PayPalEMailBox2.Text = value;
		//    }
		//}

		public string PayPalPlanID
		{
			get { return PayPalPlanIDBox.Text; }
			set { PayPalPlanIDBox.Text = value; }
		}

		public bool PayPalTestPayment
		{
			get { return PayPalTestPaymentCheckBox.IsChecked; }
			set { PayPalTestPaymentCheckBox.IsChecked = value; }

		}

		#endregion

		#region | 아임포트 |

		public string IamPortBody
		{
			get { return IamPortBodyTextBox.Text; }
			set { IamPortBodyTextBox.Text = value; }
		}

		#endregion

		public SmartWebPGPayPageEx (bool bIsWebModel, bool bIsEBookModel)
		{
			InitializeComponent ();

			m_bIsWebModel = bIsWebModel;
			m_bIsEBookModel = bIsEBookModel;

			// 다국어 번역
			if (LC.LANG.KOREAN != LC.PQLanguage)
			{
				LC.FormLocalize ("SmartWebPGPayPageEx", this);
			}

			// 지역별 결제수단
			switch (LC.PQLanguage)
			{
				case LC.LANG.KOREAN:
					{
						SBAtomRadioButton.Visibility = Visibility.Collapsed;

						if (true == m_bIsWebModel)
						{
							InAppRadioButton.Visibility = Visibility.Collapsed;
							KakaoRadioButton.Visibility = Visibility.Collapsed;

							Grid.SetColumn (PaddleRadioButton, 1);

							Grid.SetColumn (PaypalRadioButton, 2);
							Grid.SetRow (PaypalRadioButton, 0);

							RadioGrid.ColumnDefinitions[1].Width = new GridLength (85, GridUnitType.Pixel);
							RadioGrid.RowDefinitions[1].Height = new GridLength (0, GridUnitType.Pixel);
						}
						else
						{
							//if (false == PQAppBase.strTrial)
							//{
							//	PaypalRadioButton.IsEnabled = false;
							//	PaddleRadioButton.IsEnabled = false;
							//}
						}

						if (true == PQAppBase.strTrial)
						{
							if (true == m_bIsWebModel)
							{
								Grid.SetColumn (IamPortRadioButton, 3);
								Grid.SetRow (IamPortRadioButton, 0);
							}

							IamPortRadioButton.Visibility = Visibility.Visible;
						}
					}
					break;
				case LC.LANG.ENGLISH:
					{
						InAppRadioButton.Visibility = (false != m_bIsWebModel) ? Visibility.Collapsed : Visibility.Visible;
						PGAtomRadioButton.Visibility = Visibility.Collapsed;
						KakaoRadioButton.Visibility = Visibility.Collapsed;
						SBAtomRadioButton.Visibility = Visibility.Collapsed;
						PaddleRadioButton.Visibility = Visibility.Visible;
					}
					break;
				case LC.LANG.JAPAN:
					{
						PGAtomRadioButton.Visibility = Visibility.Collapsed;
						KakaoRadioButton.Visibility = Visibility.Collapsed;

						RadioGrid.RowDefinitions[1].Height = new GridLength (0, GridUnitType.Pixel);
						Grid.SetRow (PaypalRadioButton, 0);

						if (true == m_bIsWebModel)
						{
							InAppRadioButton.Visibility = Visibility.Collapsed;
							Grid.SetColumn (PaddleRadioButton, 1);
							Grid.SetColumn (PaypalRadioButton, 2);
						}
						else
						{
							InAppRadioButton.Visibility = Visibility.Visible;
							Grid.SetColumn (InAppRadioButton, 1);

							Grid.SetColumn (PaddleRadioButton, 2);
							Grid.SetColumn (PaypalRadioButton, 3);
						}
					}
					break;
			}

			ResourceDictionary sourceResource = new ResourceDictionary ();
			sourceResource.Source = new Uri ("pack://application:,,,/StyleResourceDictionary;component/Style/AttPageStyle/AttPageTemplate.xaml");
			Style UserControlStyle = sourceResource["AttPageStyle"] as Style;
			Style TitleGridStyle = sourceResource["AttPageTitleGridStyle"] as Style;
			Style TitleTextStyle = sourceResource["AttPageTitleTextStyle"] as Style;

			this.Style = UserControlStyle;
			this.TitleGrid.Style = TitleGridStyle;
			this.TitleTextBlock.Style = TitleTextStyle;
		}

		public override void SetTitle (string strTitle)
		{
			this.TitleTextBlock.Text = strTitle;
		}

		private void OnPreviewDragOver (object sender, DragEventArgs e)
		{
			e.Handled = true; // 180730_AHN

			ListViewItem itemSource = (ListViewItem)e.Data.GetData (typeof (ListViewItem));

			if (null == itemSource)
				return;

			e.Effects = DragDropEffects.All;

		}

		private void textPG01_PreviewDrop (object sender, DragEventArgs e)
		{
			ListViewItem itemSource = (ListViewItem)e.Data.GetData (typeof (ListViewItem));
			if (null == itemSource)
				return;

			List<string> subText = itemSource.Content as List<string>;

			if (subText == null)
				return;

			if (subText.Count > 1)
				textSID.Text = subText[1];
		}

		private void textPG02_PreviewDrop (object sender, DragEventArgs e)
		{
			ListViewItem itemSource = (ListViewItem)e.Data.GetData (typeof (ListViewItem));
			if (null == itemSource)
				return;

			List<string> subText = itemSource.Content as List<string>;

			if (subText == null)
				return;

			if (subText.Count > 1)
				textSignKey.Text = subText[1];
		}



		private void textPG03_PreviewDrop (object sender, DragEventArgs e)
		{
			ListViewItem itemSource = (ListViewItem)e.Data.GetData (typeof (ListViewItem));
			if (null == itemSource)
				return;

			List<string> subText = itemSource.Content as List<string>;

			if (subText == null)
				return;

			if (subText.Count > 1)
				textPG03.Text = subText[1];
		}


		private void textPG04_PreviewDrop (object sender, DragEventArgs e)
		{
			ListViewItem itemSource = (ListViewItem)e.Data.GetData (typeof (ListViewItem));
			if (null == itemSource)
				return;

			List<string> subText = itemSource.Content as List<string>;

			if (subText == null)
				return;

			if (subText.Count > 1)
				textPG04.Text = subText[1];
		}

		private void textPG05_PreviewDrop (object sender, DragEventArgs e)
		{
			ListViewItem itemSource = (ListViewItem)e.Data.GetData (typeof (ListViewItem));
			if (null == itemSource)
				return;

			List<string> subText = itemSource.Content as List<string>;

			if (subText == null)
				return;

			if (subText.Count > 1)
				textPG05.Text = subText[1];
		}


		private void textPG06_PreviewDrop (object sender, DragEventArgs e)
		{
			ListViewItem itemSource = (ListViewItem)e.Data.GetData (typeof (ListViewItem));
			if (null == itemSource)
				return;

			List<string> subText = itemSource.Content as List<string>;

			if (subText == null)
				return;

			if (subText.Count > 1)
				textPG06.Text = subText[1];
		}

		private void textPG07_PreviewDrop (object sender, DragEventArgs e)
		{
			ListViewItem itemSource = (ListViewItem)e.Data.GetData (typeof (ListViewItem));
			if (null == itemSource)
				return;

			List<string> subText = itemSource.Content as List<string>;

			if (subText == null)
				return;

			if (subText.Count > 1)
				textPG07.Text = subText[1];
		}

		private void textPG08_PreviewDrop (object sender, DragEventArgs e)
		{

			ListViewItem itemSource = (ListViewItem)e.Data.GetData (typeof (ListViewItem));
			if (null == itemSource)
				return;

			List<string> subText = itemSource.Content as List<string>;

			if (subText == null)
				return;

			if (subText.Count > 1)
				textPG07.Text = subText[1];

		}

		//  구글인앱결제(0), PG결제(1), 카카오페이(2), SB결제(3), Paddle결제(4), Paypal(5)
		private void VisiblaPaymentType ()
		{
			this.StackPanel_PGAtom.Visibility = System.Windows.Visibility.Collapsed;        // PG결제
			this.StackPanel_KakaoAtom.Visibility = System.Windows.Visibility.Collapsed;     // 카카오페이 결제
            this.StackPanel_InAppAtom.Visibility = System.Windows.Visibility.Collapsed;     // 인앱 결제
			this.StackPanel_PaddleAtom.Visibility = System.Windows.Visibility.Collapsed;    // Paddle결제
			this.StackPanel_PaypalAtom.Visibility = System.Windows.Visibility.Collapsed;      // Paypal
			this.StackPanel_IamPortAtom.Visibility = System.Windows.Visibility.Collapsed;      //아임포트
            this.StackPanel_SBAtom.Visibility = System.Windows.Visibility.Collapsed;        // SB결제

            switch (this.PaymentType)
			{
				
				case 1:
					this.StackPanel_PGAtom.Visibility = Visibility.Visible;
					break;
				case 2:
					this.StackPanel_KakaoAtom.Visibility = Visibility.Visible;
					break;
                case 3:
                    this.StackPanel_InAppAtom.Visibility = Visibility.Visible;
                    break;
				case 4:
					this.StackPanel_PaddleAtom.Visibility = Visibility.Visible;
					break;
				case 5:
					this.StackPanel_PaypalAtom.Visibility = Visibility.Visible;
					break;
				case 6:
					this.StackPanel_IamPortAtom.Visibility = Visibility.Visible;
					break;
                case 7:
                    this.StackPanel_SBAtom.Visibility = Visibility.Visible;
                    break;
            }
		}

		private void PaymentTypeRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			VisiblaPaymentType ();
		}

		private void RegularTypeRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			RegularVisibleStatus ();
		}

		private void RegularVisibleStatus ()
		{
			if (0 == this.RegularType)
			{
				labelPG07.Visibility = Visibility.Visible;
				textPG07.Visibility = Visibility.Visible;

				labelPG08.Visibility = Visibility.Visible;
				textPG08.Visibility = Visibility.Visible;

				//------------------------
				labelPG00.Visibility = Visibility.Collapsed;
				textBillingKey.Visibility = Visibility.Collapsed;

				labelPG11.Visibility = Visibility.Collapsed;
				textApiKey.Visibility = Visibility.Collapsed;

				labelPG12.Visibility = Visibility.Collapsed;
				textApiIv.Visibility = Visibility.Collapsed;

				labelPG09.Visibility = Visibility.Collapsed;
				RegularCyclePanel.Visibility = Visibility.Collapsed;

				labelPG13.Visibility = Visibility.Collapsed;
				textCardNumber.Visibility = Visibility.Collapsed;

				labelPG14.Visibility = Visibility.Collapsed;
				textCardExpireDate.Visibility = Visibility.Collapsed;

				labelPG15.Visibility = Visibility.Collapsed;
				textCardPassword.Visibility = Visibility.Collapsed;

				labelPG16.Visibility = Visibility.Collapsed;
				textRegNo.Visibility = Visibility.Collapsed;

				labelPG17.Visibility = Visibility.Collapsed;
				textEmail.Visibility = Visibility.Collapsed;

				textSID.Visibility = Visibility.Visible;
				textSignKey.Visibility = Visibility.Visible;
				textSID_R.Visibility = Visibility.Collapsed;
				textSignKey_R.Visibility = Visibility.Collapsed;
			}
			else
			{
				labelPG07.Visibility = Visibility.Collapsed;
				textPG07.Visibility = Visibility.Collapsed;

				labelPG08.Visibility = Visibility.Collapsed;
				textPG08.Visibility = Visibility.Collapsed;

				//------------------------
				labelPG00.Visibility = Visibility.Visible;
				textBillingKey.Visibility = Visibility.Visible;

				labelPG12.Visibility = Visibility.Visible;
				textApiIv.Visibility = Visibility.Visible;

				labelPG11.Visibility = Visibility.Visible;
				textApiKey.Visibility = Visibility.Visible;

				labelPG13.Visibility = Visibility.Visible;
				textCardNumber.Visibility = Visibility.Visible;

				labelPG14.Visibility = Visibility.Visible;
				textCardExpireDate.Visibility = Visibility.Visible;

				labelPG15.Visibility = Visibility.Visible;
				textCardPassword.Visibility = Visibility.Visible;

				labelPG16.Visibility = Visibility.Visible;
				textRegNo.Visibility = Visibility.Visible;

				labelPG17.Visibility = Visibility.Visible;
				textEmail.Visibility = Visibility.Visible;

				labelPG09.Visibility = Visibility.Visible;
				RegularCyclePanel.Visibility = Visibility.Visible;

				textSID.Visibility = Visibility.Collapsed;
				textSignKey.Visibility = Visibility.Collapsed;
				textSID_R.Visibility = Visibility.Visible;
				textSignKey_R.Visibility = Visibility.Visible;
			}
		}

		private void PayPalNormalRadio_Checked (object sender, RoutedEventArgs e)
		{
			if (null != PayPalPaymentGrid && null != PayPalSubscriptionGrid)
			{
				if (true == PayPalNormalRadio.IsChecked)
				{
					PayPalPaymentGrid.Visibility = Visibility.Visible;
					PayPalSubscriptionGrid.Visibility = Visibility.Collapsed;
				}
				else
				{
					PayPalPaymentGrid.Visibility = Visibility.Collapsed;
					PayPalSubscriptionGrid.Visibility = Visibility.Visible;
				}
			}
		}


		private void InAppNormalRadio_Checked (object sender, RoutedEventArgs e)
		{
			if (null != PayPalPaymentGrid && null != PayPalSubscriptionGrid)
			{
				if (true == PayPalNormalRadio.IsChecked)
				{
					PayPalPaymentGrid.Visibility = Visibility.Visible;
					PayPalSubscriptionGrid.Visibility = Visibility.Collapsed;
				}
				else
				{
					PayPalPaymentGrid.Visibility = Visibility.Collapsed;
					PayPalSubscriptionGrid.Visibility = Visibility.Visible;
				}
			}
		}
	}
}
