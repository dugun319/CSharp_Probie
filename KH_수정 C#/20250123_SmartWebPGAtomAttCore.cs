using System.Diagnostics;
using System.Windows.Controls;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopControl.Components.Payment;
using Softpower.SmartMaker.TopSmartAtom;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;
using Softpower.SmartMaker.TopSmartWebAtomEdit.AttPage;
using Softpower.SmartMaker.TopWebAtom;

namespace Softpower.SmartMaker.TopSmartAtomManager.SmartWebCore
{
	class SmartWebPGAtomAttCore : SmartAtomAttCore
	{

		#region 필드 자료형, 사이즈 상수선언
		private const int NUM_SIZE = 40;
		private const int MONEY_SIZE = 4;
		private const int NAME_SIZE = 20;
		private const int PRONAME_SIZE = 20;
		private const int TYPE_SIZE = 12;
		private const int EMAIL_SIZE = 40;
		private const int HP_SIZE = 14;
		private const int LOGOIMG_SIZE = 60;
		private const int PROIMG_SIZE = 60;
		private const int TAX_SIZE = 4;
		private const string NUM_TYPE = "char";
		private const string MONEY_TYPE = "int";
		private const string NAME_TYPE = "char";
		private const string PRONAME_TYPE = "char";
		private const string TYPE_TYPE = "char";
		private const string EMAIL_TYPE = "char";
		private const string HP_TYPE = "char";
		private const string LOGOIMG_TYPE = "char";
		private const string PROIMG_TYPE = "char";
		private const string TAX_TYPE = "int";

		#endregion


		public SmartWebPGAtomAttCore (Atom pAtom)
			: base (pAtom)
		{
		}

		public SmartWebPGAtomAttCore (Atom pAtom, bool bIsWebModel, bool bIsEBookModel)
			: base (pAtom, bIsWebModel, bIsEBookModel)
		{
		}

		private void SetFieldInfo (Atom p_Atom, int n_Size, string str_Type)
		{
			MasterInputAttrib pAttrib = p_Atom.GetAttrib () as MasterInputAttrib;

			if (pAttrib != null)
			{
				MasterInputAttrib pOrgAttrib = new MasterInputAttrib ();
				CloneObject.CloneProperty (pAttrib, pOrgAttrib);

				bool bIsChanged = pAttrib.FieldType.Equals (str_Type);
				// 정렬형태 자동 설정 부분

				InputAttrib pInputAttrib = pAttrib as InputAttrib;

				if (!bIsChanged)
				{
					if (null != pInputAttrib)
					{
						if ("int" == str_Type)
						{
							pInputAttrib.WndHorzAlign = System.Windows.Forms.HorizontalAlignment.Right;

							pInputAttrib.InputNum = true;
							pInputAttrib.IsInsertComma = true;
							if (true == str_Type.Equals ("int"))
							{
								pInputAttrib.Prime = "\"0\"";
							}
							else
							{
								pInputAttrib.Prime = "\"2\"";
							}
						}
						else
						{
							pInputAttrib.InputNum = false;
							pInputAttrib.IsInsertComma = false;
							pInputAttrib.WndHorzAlign = System.Windows.Forms.HorizontalAlignment.Left;
						}
					}
				}

				pAttrib.FieldType = str_Type;
				pAttrib.FieldLength = n_Size;

				if (false == CloneObject.IsEqualProperty (pOrgAttrib, pAttrib))
				{
					SetUndoRedoInfo ();
				}

				p_Atom.SetAttrib (pAttrib);
				p_Atom.NotifyAttrib (pAttrib);

				p_Atom.Refresh ();
			}
		}

		public override UserControl GetAttPage ()
		{
			SmartWebPGPayPageEx pPage = new SmartWebPGPayPageEx (m_bIsWebModel, m_bIsEBookModel);

			if (null != pPage)
			{
				Attrib pSrcAttrib = this.AttAtom.GetAttrib ();
				if (pSrcAttrib is PGAttrib)
				{
					if (LC.LANG.JAPAN == LC.PQLanguage) //2019-10-18 kys 일본어 버전일때 SB를 기본으로 설정함
					{
						pPage.PaymentType = 3;
					}
					else if (LC.LANG.ENGLISH == LC.PQLanguage)
					{
						pPage.PaymentType = 4; // 영문버전일때 Paddle 인앱결제를 기본으로 설정함
					}
					else
					{
						pPage.PaymentType = 1;
					}

					PGAttrib pAttrib = AttAtom.GetAttrib () as PGAttrib;
					if (null != pAttrib)
					{
						pPage.SID = pAttrib.SID;
						pPage.SignKey = pAttrib.SignKey;
						pPage.Money = pAttrib.Money;
						pPage.UName = pAttrib.UName;
						pPage.ProName = pAttrib.ProName;
						pPage.HP = pAttrib.HP;
						pPage.PGPayment = pAttrib.PGPayment;
						pPage.HPPMethod = pAttrib.HppMethod;

						// 정기결제
						pPage.SID_R = pAttrib.SID_R;
						pPage.SignKey_R = pAttrib.SignKey_R;
						pPage.RegularType = pAttrib.RegularType;
						pPage.BillingKey = pAttrib.BillingKey;
						pPage.ApiKey = pAttrib.ApiKey;
						pPage.ApiIv = pAttrib.ApiIv;
						pPage.RegularCycle = pAttrib.RegularCycle;
						pPage.CardNumber = pAttrib.CardNumber;
						pPage.CardExpireDate = pAttrib.CardExpireDate;
						pPage.CardSmartMakerCode = pAttrib.CardSmartMakerCode;
						pPage.RegNo = pAttrib.RegNo;
						pPage.Email = pAttrib.Email;
					}
				}				
				else if (pSrcAttrib is KakaoPayAttrib)
				{
					pPage.PaymentType = 2;

					KakaoPayAttrib pAttrib = this.AttAtom.GetAttrib () as KakaoPayAttrib;
					if (null != pAttrib)
					{
						pPage.CID = pAttrib.CID;
						pPage.APIKEY = pAttrib.APIKEY;
						pPage.KakaoProName = pAttrib.KakaoProName;
						pPage.KakaoMoney = pAttrib.KakaoMoney;
						pPage.KakaoUID = pAttrib.KakaoUID;
					}
				}
                else if (pSrcAttrib is InAppBillingAttrib)
                {
                    pPage.PaymentType = 3;

                    InAppBillingAttrib pAttrib = this.AttAtom.GetAttrib() as InAppBillingAttrib;
                    if (null != pAttrib)
                    {
                        pPage.InAppSubscription = pAttrib.InAppSubscription;
                        pPage.ProductId = pAttrib.ProductId;
                        pPage.IsContinuity = pAttrib.IsContinuity;
                        pPage.LicenseKey = pAttrib.LicenseKey;
                    }
                }                
				else if (pSrcAttrib is PaddleInAppAttrib)
				{
					pPage.PaymentType = 4;

					PaddleInAppAttrib pAttrib = this.AttAtom.GetAttrib () as PaddleInAppAttrib;
					if (null != pAttrib)
					{
						pPage.PaddleVendorID = pAttrib.PaddleVendorID;
						pPage.PaddleAPIKey = pAttrib.PaddleAPIKey;
						pPage.PaddleProductID = pAttrib.PaddleProductID;
						pPage.PaddleEmail = pAttrib.Email;
					}
				}
				else if (pSrcAttrib is PaypalAttrib)
				{
					pPage.PaymentType = 5;

					PaypalAttrib pAttrib = this.AttAtom.GetAttrib () as PaypalAttrib;
					if (null != pAttrib)
					{
						pPage.PayPalPaymentType = pAttrib.PayPalPaymentType;
						pPage.PayPalMoney = pAttrib.PayPalCost;
						pPage.PayPalID = pAttrib.PayPalID;
						pPage.PayPalSmartMakerCode = pAttrib.PayPalSmartMakerCode;
						pPage.PayPalProductName = pAttrib.PayPalProductName;
						pPage.PayPalCurrencyCode = pAttrib.PayPalCurrencyCode;
						pPage.PayPalUserName = pAttrib.PayPalUserName;
						pPage.PayPalPhoneNumber = pAttrib.PayPalPhoneNumber;
						//pPage.PayPalEMail = pAttrib.PayPalEMail;
						pPage.PayPalPlanID = pAttrib.PayPalPlanID;
						pPage.PayPalTestPayment = pAttrib.PayPalTestPayment;
					}
				}
				else if (pSrcAttrib is IamportAttrib)
				{
					IamportAttrib atomAttrib = this.AttAtom.GetAttrib () as IamportAttrib;

					pPage.PaymentType = 6;
					pPage.IamPortBody = atomAttrib.Body;
				}
                else if (pSrcAttrib is SoftBankAttrib)
                {
                    pPage.PaymentType = 7;

                    SoftBankAttrib pAttrib = this.AttAtom.GetAttrib() as SoftBankAttrib;
                    if (null != pAttrib)
                    {
                        pPage.MerchantID = pAttrib.MerchantID;
                        pPage.ServiceID = pAttrib.ServiceID;
                        pPage.HashCode = pAttrib.HashCode;
                        pPage.CustCode = pAttrib.CustCode;
                        pPage.ItemID = pAttrib.ItemID;
                        pPage.Amount = pAttrib.Amount;
                    }
                }
            }

			return pPage;
		}

        // 1: WebPG, 2: WebKakaoPay, 3: InAppBilling(구글인앱), 4: 해외결제(PaddleInApp), 5: Paypal, 6: IamPort, 7: SBAtom
        public override void OnUpdateAtt ()
		{
			SmartWebPGPayPageEx pAttPage = this.CurrentAttPage as SmartWebPGPayPageEx;
			if (null != pAttPage)
			{
				PaymentBaseAttrib pAttrib = this.AttAtom.GetAttrib () as PaymentBaseAttrib;
				// pAttrib는 새로 선택된 Class의 속성임.

				if (null != pAttrib)
				{
					pAttrib.PaymentType = pAttPage.PaymentType;

					AtomBase atomBase = this.AttAtom.GetOfAtom () as AtomBase;

					PaymentBaseAttrib pOrgAttrib = System.Activator.CreateInstance (pAttrib.GetType ()) as PaymentBaseAttrib;

                    Atom pAtom = AttAtom;
					// pAtom은 Origine Class임

					int nCheckType = -1;

					if(this.AttAtom is PGAtom)
					{
						nCheckType = 1;

                    }else if(this.AttAtom is KakaoPayAtom)

                    {
                        nCheckType = 2;

                    }
                    else if (this.AttAtom is InAppBillingAtom)

                    {
                        nCheckType = 3;

                    }
                    else if (this.AttAtom is PaddleInAppAtom)

                    {
                        nCheckType = 4;

                    }
                    else if (this.AttAtom is PaypalAtom)

                    {
                        nCheckType = 5;

                    }
                    else if (this.AttAtom is IamportAtom)

                    {
                        nCheckType = 6;

                    }
                    else if (this.AttAtom is SoftBankAtom)

                    {
                        nCheckType = 7;

                    }

					Debug.WriteLine($"nCheckType is {nCheckType}");
                    Debug.WriteLine($"AttAtom.AtomType is {AttAtom.AtomType}");
                    Debug.WriteLine($"pAtom.AtomType is {pAtom.AtomType}");
                    Debug.Write($"pAttrib.PaymentType is {pAttrib.PaymentType}");
                    Debug.WriteLine($"pOrgAttrib.PaymentType is {pOrgAttrib.PaymentType}");

                    CloneObject.CloneProperty(pAttrib, pOrgAttrib);

					if(nCheckType != pAttrib.PaymentType)
					{
                        Information info = pAtom.Information;
                        //info.SetAttribPaymentAtom(ref pAtom);
                        info.SetAttribToAtom(ref pAtom, pAttPage.PaymentType);
                    }

                    //m_selectedAtom = pAtom;

                    PaymentBaseAttrib newAttrib = pAtom.GetAttrib () as PaymentBaseAttrib;
					if (pAtom is PGAtom)
					{
						PGAttrib pgAttrib = newAttrib as PGAttrib;

						// 결제속성창 업데이트
						pgAttrib.SID = pAttPage.SID;
						pgAttrib.SignKey = pAttPage.SignKey;
						pgAttrib.Money = pAttPage.Money;
						pgAttrib.UName = pAttPage.UName;
						pgAttrib.ProName = pAttPage.ProName;
						pgAttrib.HP = pAttPage.HP;
						pgAttrib.PGPayment = pAttPage.PGPayment;
						pgAttrib.HppMethod = pAttPage.HPPMethod;

						// 정기결제
						pgAttrib.SID_R = pAttPage.SID_R;
						pgAttrib.SignKey_R = pAttPage.SignKey_R;
						pgAttrib.RegularType = pAttPage.RegularType;
						pgAttrib.BillingKey = pAttPage.BillingKey;
						pgAttrib.ApiKey = pAttPage.ApiKey;
						pgAttrib.ApiIv = pAttPage.ApiIv;
						pgAttrib.CardNumber = pAttPage.CardNumber;
						pgAttrib.CardExpireDate = pAttPage.CardExpireDate;
						pgAttrib.CardSmartMakerCode = pAttPage.CardSmartMakerCode;
						pgAttrib.RegNo = pAttPage.RegNo;
						pgAttrib.Email = pAttPage.Email;
						pgAttrib.RegularCycle = pAttPage.RegularCycle;

						CMultiList temp = pAtom.GetAtomList (-1);
						if (temp != null)
						{
							//아톰 연결시 데이터 타입과 사이즈 정해진 크기로 설정되도록 함
							foreach (Atom atom in temp)
							{
								if (atom.Name == pAttrib.Num)
								{
									SetFieldInfo (atom, NUM_SIZE, NUM_TYPE);
								}
								else if (atom.Name == pAttrib.Money)
								{
									SetFieldInfo (atom, MONEY_SIZE, MONEY_TYPE);
								}
								else if (atom.Name == pAttrib.UName)
								{
									SetFieldInfo (atom, NAME_SIZE, NAME_TYPE);
								}
								else if (atom.Name == pAttrib.ProName)
								{
									SetFieldInfo (atom, PRONAME_SIZE, PRONAME_TYPE);
								}
								else if (atom.Name == pAttrib.Email)
								{
									SetFieldInfo (atom, EMAIL_SIZE, EMAIL_TYPE);
								}
								else if (atom.Name == pAttrib.HP)
								{
									SetFieldInfo (atom, HP_SIZE, HP_TYPE);
								}
								else if (atom.Name == pAttrib.LogoImg)
								{
									SetFieldInfo (atom, LOGOIMG_SIZE, LOGOIMG_TYPE);
								}
								else if (atom.Name == pAttrib.ProImg)
								{
									SetFieldInfo (atom, PROIMG_SIZE, PROIMG_TYPE);
								}
							}
						}
					}
					else if (pAtom is InAppBillingAtom)
					{
						InAppBillingAttrib inAppAttrib = newAttrib as InAppBillingAttrib;
						if (null != pAttrib)
						{
							// 결제속성창 업데이트
							inAppAttrib.InAppSubscription = pAttPage.InAppSubscription;
							inAppAttrib.ProductId = pAttPage.ProductId;
							inAppAttrib.IsContinuity = pAttPage.IsContinuity;
							inAppAttrib.LicenseKey = pAttPage.LicenseKey;
						}
					}
					else if (pAtom is KakaoPayAtom)
					{
						KakaoPayAttrib KakaoPayAttrib = newAttrib as KakaoPayAttrib;
						if (null != KakaoPayAttrib)
						{
							// 결제속성창 업데이트
							KakaoPayAttrib.CID = pAttPage.CID;
							KakaoPayAttrib.APIKEY = pAttPage.APIKEY;
							KakaoPayAttrib.KakaoProName = pAttPage.KakaoProName;
							KakaoPayAttrib.KakaoMoney = pAttPage.KakaoMoney;
							KakaoPayAttrib.KakaoUID = pAttPage.KakaoUID;
						}
					}
					else if (pAtom is SoftBankAtom)
					{
						SoftBankAttrib sbAttrib = newAttrib as SoftBankAttrib;
						if (null != sbAttrib)
						{
							// 결제속성창 업데이트
							sbAttrib.MerchantID = pAttPage.MerchantID;
							sbAttrib.ServiceID = pAttPage.ServiceID;
							sbAttrib.HashCode = pAttPage.HashCode;
							sbAttrib.CustCode = pAttPage.CustCode;
							sbAttrib.ItemID = pAttPage.ItemID;
							sbAttrib.Amount = pAttPage.Amount;
						}
					}
					else if (pAtom is PaddleInAppAtom)
					{
						PaddleInAppAttrib paddleAttrib = newAttrib as PaddleInAppAttrib;
						if (null != paddleAttrib)
						{
							// 결제속성창 업데이트
							paddleAttrib.PaddleVendorID = pAttPage.PaddleVendorID;
							paddleAttrib.PaddleAPIKey = pAttPage.PaddleAPIKey;
							paddleAttrib.PaddleProductID = pAttPage.PaddleProductID;
							paddleAttrib.Email = pAttPage.PaddleEmail;
						}
					}
					else if (pAtom is PaypalAtom)
					{
						PaypalAttrib paddleAttrib = newAttrib as PaypalAttrib;
						if (null != paddleAttrib)
						{
							// 결제속성창 업데이트
							paddleAttrib.PayPalPaymentType = pAttPage.PayPalPaymentType;
							paddleAttrib.PayPalCost = pAttPage.PayPalMoney;
							paddleAttrib.PayPalID = pAttPage.PayPalID;
							paddleAttrib.PayPalSmartMakerCode = pAttPage.PayPalSmartMakerCode;
							paddleAttrib.PayPalProductName = pAttPage.PayPalProductName;
							paddleAttrib.PayPalCurrencyCode = pAttPage.PayPalCurrencyCode;
							paddleAttrib.PayPalUserName = pAttPage.PayPalUserName;
							paddleAttrib.PayPalPhoneNumber = pAttPage.PayPalPhoneNumber;
							//paddleAttrib.PayPalEMail = pAttPage.PayPalEMail;
							paddleAttrib.PayPalPlanID = pAttPage.PayPalPlanID;
							paddleAttrib.PayPalTestPayment = pAttPage.PayPalTestPayment;
						}
					}
					else if (pAtom is IamportAtom)
					{
						IamportAttrib atomAttrib = newAttrib as IamportAttrib;
						atomAttrib.Body = pAttPage.IamPortBody;
					}

					if (pAtom != AttAtom)
					{
						//새로 생긴 아톰에 ofAtom과 델리게이트 연결 해줘야함
						atomBase.InitAtomBaseWhenAtomCoreHasChanged(m_selectedAtom);
						// - 결제속성의 방식 변경 후 저장할 경우 필수 (Attrib_OnNotifyChangeAttribEvent null 값 방지)
						AtomBase NewAtomBase = pAtom.GetOfAtom() as AtomBase;
						NewAtomBase.InitAtomBaseWhenAtomCoreHasChanged(pAtom);
						//
					}

					if (false == CloneObject.IsEqualProperty (pOrgAttrib, newAttrib))
					{
						m_selectedAtom.SetAttrib (newAttrib);
						pAtom.SetAttrib (newAttrib);

						//Undo 속성 저장
						newAttrib.ChangeAttribCommand (pOrgAttrib);

						pAtom.CompletePropertyChanged ();
					}

				}
			}
		}

		public override UserControl GetAtomViewAttPage ()
		{
			SmartWebPGAttPage pPage = new SmartWebPGAttPage ();

			if (null != pPage)
			{
				LabelAttrib pAttrib = AttAtom.GetAttrib () as LabelAttrib;
				if (null != pAttrib)
				{
					pPage.WndDisabled = pAttrib.IsDisabled;
					pPage.WndVisible = pAttrib.IsAtomHidden;
					pPage.Vanish = pAttrib.IsVanish;

					// 이미지
					CObjectImage pObjectImage = null;
					int nKey = pAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._IMAGE);
					pAttrib.GetGDIObjFromKey (ref pObjectImage, nKey);
					if (null != pObjectImage)
					{
						pPage.ImagePath = pObjectImage.ImagePath;
					}

					pPage.ImagePath = pAttrib.ImagePath;
				}
			}

			return pPage;
		}

		public override void OnUpdateAtomViewAtt ()
		{
			SmartWebPGAttPage pAttPage = this.CurrentAttPage as SmartWebPGAttPage;

			if (null != pAttPage)
			{
				PaymentBaseAttrib pAttrib = this.AttAtom.GetAttrib () as PaymentBaseAttrib;
				if (null != pAttrib)
				{
					Attrib pOrgAttrib = null;
					if (pAttrib is PGAttrib)
						pOrgAttrib = new PGAttrib ();
					else if (pAttrib is InAppBillingAttrib)
						pOrgAttrib = new InAppBillingAttrib ();
					else if (pAttrib is KakaoPayAttrib)
						pOrgAttrib = new KakaoPayAttrib ();
					else if (pAttrib is SoftBankAttrib)
						pOrgAttrib = new SoftBankAttrib ();
					else if (pAttrib is PaddleInAppAttrib)
						pOrgAttrib = new PaddleInAppAttrib ();
					else if (pAttrib is PaypalAttrib)
						pOrgAttrib = new PaypalAttrib ();
					else if (pAttrib is IamportAttrib)
						pOrgAttrib = new IamportAttrib ();

					if (null != pOrgAttrib)
						CloneObject.CloneProperty (pAttrib, pOrgAttrib);

					pAttrib.IsDisabled = pAttPage.WndDisabled;
					pAttrib.IsAtomHidden = pAttPage.WndVisible;
					pAttrib.IsVanish = pAttPage.Vanish;

					// 이미지 설정

					CObjectImage pObjectImage = new CObjectImage ();
					pObjectImage.AddRef ();

					pObjectImage.ImagePath = pAttPage.ImagePath;

					int nKey = pAttrib.GetGDIKey ((int)Define.OBJECTKEY_TYPE._IMAGE);
					nKey = pAttrib.GetKeyFromGDIObj (pObjectImage, nKey);
					pAttrib.SetGDIKey ((int)Define.OBJECTKEY_TYPE._IMAGE, nKey);

					pAttrib.ImagePath = pAttPage.ImagePath;

					if (false == CloneObject.IsEqualProperty (pOrgAttrib, pAttrib))
					{
						//Undo 속성 저장
						pAttrib.ChangeAttribCommand (pOrgAttrib);

						this.AttAtom.CompletePropertyChanged ();
					}
				}
			}
		}
	}
}
