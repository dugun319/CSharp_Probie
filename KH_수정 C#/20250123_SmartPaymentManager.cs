using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopProcess.Component.ViewModels;
using Softpower.SmartMaker.TopProcess.Component.Views;

namespace Softpower.SmartMaker.TopProcess.AtomConvertManager
{
    /// <summary>
    /// // 1: WebPG, 2: WebKakaoPay, 3: InAppBilling(구글인앱), 4: 해외결제(PaddleInApp), 5: Paypal, 6: IamPort, 7: SBAtom
    /// </summary>
    public class SmartPaymentManager : SmartAtomCoreManager
	{
		public SmartPaymentManager ()
		{

		}

		public override void SetAttribToAtom (ref Atom pAtom, DMTView pView, object objType)
		{
			int nAtomType = (int)objType;

			SetAttribPaymentAtom (ref pAtom, pView, nAtomType);
		}

		public override object AtomTypeToObject (AtomType atomType)
		{
			switch (atomType)
			{
				case AtomType.WebPG: return 1;
				case AtomType.WebKakaoPay: return 2;
                case AtomType.InAppBilling: return 3;
				case AtomType.PaddleInApp: return 4;
				case AtomType.Paypal: return 5;
				case AtomType.IamPort: return 6;
                case AtomType.WebSoftBank: return 7;
                default: return -1;
			}
		}

		public static void SetAttribPaymentAtom (ref Atom pAtom, DMTView pView, int nPaymentType)
		{
			Atom pNewAtom = null;
			Attrib pSourceAttrib = pAtom.GetAttrib ();
			Attrib pTargetAttrib;
			AtomBase pNewAtomBase = null;
			DMTDoc CurrentDMTDoc = pView.Document as DMTDoc;

            // namespace Softpower.SmartMaker.TopSmartAtom
            // public class PaymentBaseAttrib 같이 확인필요
            // namespace Softpower.SmartMaker.TopSmartAtomManager.SmartWebCore
            // public override void OnUpdateAtt ()
            switch (nPaymentType)
			{
				case 1:
					{
						pNewAtomBase = MakeAtom (pView, AtomType.WebPG, pAtom);
                        pNewAtom = pNewAtomBase.AtomCore;
					}
					break;
				case 2:
					{
						pNewAtomBase = MakeAtom (pView, AtomType.WebKakaoPay, pAtom);
                        pNewAtom = pNewAtomBase.AtomCore;
					}
					break;
                case 3:
                    {
                        pNewAtomBase = MakeAtom(pView, AtomType.InAppBilling, pAtom);
                        pNewAtom = pNewAtomBase.AtomCore;
                    }
                    break;
				case 4:
					{
						pNewAtomBase = MakeAtom (pView, AtomType.PaddleInApp, pAtom);
                        pNewAtom = pNewAtomBase.AtomCore;
					}
					break;
				case 5:
					{
						pNewAtomBase = MakeAtom (pView, AtomType.Paypal, pAtom);
                        pNewAtom = pNewAtomBase.AtomCore;
					}
					break;
				case 6:
					{
						pNewAtomBase = MakeAtom (pView, AtomType.IamPort, pAtom);
                        pNewAtom = pNewAtomBase.AtomCore;
					}
					break;
                case 7:
                    {
                        pNewAtomBase = MakeAtom(pView, AtomType.WebSoftBank, pAtom);
                        pNewAtom = pNewAtomBase.AtomCore;
                    }
                    break;
                default:
					return;
			}

			pTargetAttrib = pNewAtom.GetAttrib ();

			CloneObject.CloneProperty (pSourceAttrib, pTargetAttrib);

			SetCloneAtomBaseInformation (ref pAtom, pNewAtom, pNewAtomBase, pView, false);
		}
	}
}
