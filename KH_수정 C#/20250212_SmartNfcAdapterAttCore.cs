using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopSmartAtom;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPage;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;

namespace Softpower.SmartMaker.TopSmartAtomManager.SmartCore
{
	public class SmartNfcAdapterAttCore : SmartAtomAttCore
	{
		public SmartNfcAdapterAttCore (Atom pAtom)
			: base (pAtom)
		{
		}

		public override UserControl GetAttPage ()
		{
			SmartNfcAdapterAttPage SmartAttPage = new SmartNfcAdapterAttPage ();
			if (null != SmartAttPage)
			{
				NfcAdapterAttrib pAttrib = this.AttAtom.GetAttrib () as NfcAdapterAttrib;
				if (null != pAttrib)
				{
					SmartAttPage.WndVisible = pAttrib.IsAtomHidden;
                    SmartAttPage.Vanish = pAttrib.IsVanish;
                    SmartAttPage.IsDisabled = pAttrib.IsDisabled;

					if (true == PQAppBase.SpaceWord)
					{
						SmartAttPage.InputValue = SMProperVar_Eng.GetViewData_Atom (pAttrib.InputValue);
						SmartAttPage.OutputValue = SMProperVar_Eng.GetViewData_Atom (pAttrib.OutputValue);
					}
					else
					{
						SmartAttPage.InputValue = pAttrib.InputValue;
						SmartAttPage.OutputValue = pAttrib.OutputValue;
						SmartAttPage.SerialNumber = pAttrib.SerialNumber;
					}

					SmartAttPage.Information = this.AttAtom.Information;
				}
			}

			return SmartAttPage;
		}

		public override void OnUpdateAtt ()
		{
			SmartNfcAdapterAttPage SmartAttPage = this.CurrentAttPage as SmartNfcAdapterAttPage;

			if (null != SmartAttPage)
			{
				NfcAdapterAttrib pAttrib = this.AttAtom.GetAttrib () as NfcAdapterAttrib;
				if (null != pAttrib)
				{
					SmartSocketAttrib pOrgAttrib = new SmartSocketAttrib ();
					CloneObject.CloneProperty (pAttrib, pOrgAttrib);

					pAttrib.IsAtomHidden = SmartAttPage.WndVisible;
					pAttrib.IsVanish = SmartAttPage.Vanish;
                    pAttrib.IsDisabled = SmartAttPage.IsDisabled;

					if (true == PQAppBase.SpaceWord)
					{
						pAttrib.InputValue = SMProperVar_Eng.GetSaveData_Atom (SmartAttPage.InputValue);
						pAttrib.OutputValue = SMProperVar_Eng.GetSaveData_Atom (SmartAttPage.OutputValue);
					}
					else
					{
						pAttrib.InputValue = SmartAttPage.InputValue;
						pAttrib.OutputValue = SmartAttPage.OutputValue;
						pAttrib.SerialNumber = SmartAttPage.SerialNumber;
					}

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
