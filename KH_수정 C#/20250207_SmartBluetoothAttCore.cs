using System.Windows.Controls;

using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopSmartAtom;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPage;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;


namespace Softpower.SmartMaker.TopSmartAtomManager.SmartCore
{
	public class SmartBluetoothAttCore : SmartAtomAttCore
	{
		public SmartBluetoothAttCore (Atom pAtom)
			: base (pAtom)
		{
		}

		public override UserControl GetAttPage ()
		{
			SmartBluetoothAttPage SmartAttPage = new SmartBluetoothAttPage ();
			if (null != SmartAttPage)
			{
				SmartBluetoothAttrib pAttrib = this.AttAtom.GetAttrib () as SmartBluetoothAttrib;
				if (null != pAttrib)
				{
					SmartAttPage.WndVisible = pAttrib.IsAtomHidden;
					SmartAttPage.Vanish = pAttrib.IsVanish;
                    SmartAttPage.AtomDisabled = pAttrib.IsDisabled;                 // 20250207 KH 비활성화기능 추가
                    SmartAttPage.ConnectType = pAttrib.ConnectType;
					SmartAttPage.SimpleConnect = pAttrib.IsSimpleConnect;

					SmartAttPage.ProtocolType = pAttrib.ProtocolType;
					SmartAttPage.PairingType = pAttrib.PairingType;

					SmartAttPage.BluetoothUUID = pAttrib.BluetoothUUID;
					SmartAttPage.BluetoothMAC = pAttrib.BluetoothMAC;

					SmartAttPage.InputMethod = pAttrib.InputMethod;
					SmartAttPage.OutputMethod = pAttrib.OutputMethod;

					SmartAttPage.BluetoothNotify = pAttrib.BluetoothNotify;

					if (true == PQAppBase.SpaceWord)
					{
						SmartAttPage.InputValue = SMProperVar_Eng.GetViewData_Atom (pAttrib.InputValue);
						SmartAttPage.OutputValue = SMProperVar_Eng.GetViewData_Atom (pAttrib.OutputValue);
					}
					else
					{
						SmartAttPage.InputValue = pAttrib.InputValue;
						SmartAttPage.OutputValue = pAttrib.OutputValue;
					}

					SmartAttPage.Information = this.AttAtom.Information;
				}
			}

			return SmartAttPage;
		}

		public override void OnUpdateAtt ()
		{
			SmartBluetoothAttPage SmartAttPage = this.CurrentAttPage as SmartBluetoothAttPage;

			if (null != SmartAttPage)
			{
				SmartBluetoothAttrib pAttrib = this.AttAtom.GetAttrib () as SmartBluetoothAttrib;
				if (null != pAttrib)
				{
					SmartBluetoothAttrib pOrgAttrib = new SmartBluetoothAttrib ();
					CloneObject.CloneProperty (pAttrib, pOrgAttrib);

					pAttrib.IsAtomHidden = SmartAttPage.WndVisible;
					pAttrib.IsVanish = SmartAttPage.Vanish;
                    pAttrib.IsDisabled = SmartAttPage.AtomDisabled;     // 20250207 KH 비활성화기능 추가
                    pAttrib.ConnectType = SmartAttPage.ConnectType;
					pAttrib.IsSimpleConnect = SmartAttPage.SimpleConnect;

					pAttrib.ProtocolType = SmartAttPage.ProtocolType;
					pAttrib.PairingType = SmartAttPage.PairingType;

					pAttrib.BluetoothUUID = SmartAttPage.BluetoothUUID;
					pAttrib.BluetoothMAC = SmartAttPage.BluetoothMAC;

					pAttrib.InputMethod = SmartAttPage.InputMethod;
					pAttrib.OutputMethod = SmartAttPage.OutputMethod;

					pAttrib.BluetoothNotify = SmartAttPage.BluetoothNotify;

					if (true == PQAppBase.SpaceWord)
					{
						pAttrib.InputValue = SMProperVar_Eng.GetSaveData_Atom (SmartAttPage.InputValue);
						pAttrib.OutputValue = SMProperVar_Eng.GetSaveData_Atom (SmartAttPage.OutputValue);
					}
					else
					{
						pAttrib.InputValue = SmartAttPage.InputValue;
						pAttrib.OutputValue = SmartAttPage.OutputValue;
					}

					if (false == CloneObject.IsEqualProperty (pOrgAttrib, pAttrib))
					{
						pAttrib.ChangeAttribCommand (pOrgAttrib);

						this.AttAtom.CompletePropertyChanged ();
					}
				}
			}
		}
	}
}
