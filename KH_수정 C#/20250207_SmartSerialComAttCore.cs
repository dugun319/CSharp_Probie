﻿using System.Windows.Controls;

using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopSmartAtom;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPage;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;


namespace Softpower.SmartMaker.TopSmartAtomManager.SmartCore
{
	public class SmartSerialComAttCore : SmartAtomAttCore
	{
		public SmartSerialComAttCore (Atom pAtom)
			: base (pAtom)
		{
		}

		public override UserControl GetAttPage ()
		{
			SmartSerialComAttPage SmartAttPage = new SmartSerialComAttPage ();
			if (null != SmartAttPage)
			{
				SmartSerialComAttrib pAttrib = this.AttAtom.GetAttrib () as SmartSerialComAttrib;
				if (null != pAttrib)
				{
					SmartAttPage.WndVisible = pAttrib.IsAtomHidden;
					SmartAttPage.Vanish = pAttrib.IsVanish;
                    SmartAttPage.AtomDisabled = pAttrib.IsDisabled;                 // 20250207 KH 비활성화기능 추가
                    SmartAttPage.PortName = pAttrib.PortName;
					SmartAttPage.BaudRate = pAttrib.BaudRate;
					SmartAttPage.Parity = pAttrib.Parity;
					SmartAttPage.DataBits = pAttrib.DataBits;
					SmartAttPage.StopBits = pAttrib.StopBits;

					SmartAttPage.InputMethod = pAttrib.InputMethod;
					SmartAttPage.OutputMethod = pAttrib.OutputMethod;

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
			SmartSerialComAttPage SmartAttPage = this.CurrentAttPage as SmartSerialComAttPage;

			if (null != SmartAttPage)
			{
				SmartSerialComAttrib pAttrib = this.AttAtom.GetAttrib () as SmartSerialComAttrib;
				if (null != pAttrib)
				{
					SmartSerialComAttrib pOrgAttrib = new SmartSerialComAttrib ();
					CloneObject.CloneProperty (pAttrib, pOrgAttrib);

					pAttrib.IsAtomHidden = SmartAttPage.WndVisible;
					pAttrib.IsVanish = SmartAttPage.Vanish;
                    pAttrib.IsDisabled = SmartAttPage.AtomDisabled;     // 20250207 KH 비활성화기능 추가
                    pAttrib.PortName = SmartAttPage.PortName;
					pAttrib.BaudRate = SmartAttPage.BaudRate;
					pAttrib.Parity = SmartAttPage.Parity;
					pAttrib.DataBits = SmartAttPage.DataBits;
					pAttrib.StopBits = SmartAttPage.StopBits;

					pAttrib.InputMethod = SmartAttPage.InputMethod;
					pAttrib.OutputMethod = SmartAttPage.OutputMethod;

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
