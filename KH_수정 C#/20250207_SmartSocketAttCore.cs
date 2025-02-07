using System.Windows.Controls;

using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopSmartAtom;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPage;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;


namespace Softpower.SmartMaker.TopSmartAtomManager.SmartCore
{
	public class SmartSocketAttCore : SmartAtomAttCore
	{
		public SmartSocketAttCore (Atom pAtom)
			: base (pAtom)
		{
		}

		public override UserControl GetAttPage ()
		{
			SmartSocketAttPage SmartAttPage = new SmartSocketAttPage ();
			if (null != SmartAttPage)
			{
				SmartSocketAttrib pAttrib = this.AttAtom.GetAttrib () as SmartSocketAttrib;
				if (null != pAttrib)
				{
					SmartAttPage.WndVisible = pAttrib.IsAtomHidden;
					SmartAttPage.Vanish = pAttrib.IsVanish;
                    SmartAttPage.AtomDisabled = pAttrib.IsDisabled;                 // 20250207 KH 비활성화기능 추가
                    SmartAttPage.SocketType = pAttrib.SocketType;

					SmartAttPage.IPAddress = pAttrib.IPAddress;
					SmartAttPage.PortNumber = pAttrib.PortNumber;

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
			SmartSocketAttPage SmartAttPage = this.CurrentAttPage as SmartSocketAttPage;

			if (null != SmartAttPage)
			{
				SmartSocketAttrib pAttrib = this.AttAtom.GetAttrib () as SmartSocketAttrib;
				if (null != pAttrib)
				{
					SmartSocketAttrib pOrgAttrib = new SmartSocketAttrib ();
					CloneObject.CloneProperty (pAttrib, pOrgAttrib);

					pAttrib.IsAtomHidden = SmartAttPage.WndVisible;
					pAttrib.IsVanish = SmartAttPage.Vanish;
                    pAttrib.IsDisabled = SmartAttPage.AtomDisabled;     // 20250207 KH 비활성화기능 추가
                    pAttrib.SocketType = SmartAttPage.SocketType;

					pAttrib.IPAddress = SmartAttPage.IPAddress;
					pAttrib.PortNumber = SmartAttPage.PortNumber;

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
						//Undo 속성 저장
						pAttrib.ChangeAttribCommand (pOrgAttrib);

						this.AttAtom.CompletePropertyChanged ();
					}
				}
			}
		}
	}
}
