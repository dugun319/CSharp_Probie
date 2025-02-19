using System.Windows.Controls;

using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopAtom.Verbal;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPage;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;

namespace Softpower.SmartMaker.TopSmartAtomManager.SmartVerbalCore
{
	public class SmartVerbalITTAttCore : SmartAtomAttCore
	{
		public SmartVerbalITTAttCore (Atom pAtom)
			: base (pAtom)
		{

		}

		public override UserControl GetAttPage ()
		{
			SmartVerbalITTAttPage SmartAttPage = new SmartVerbalITTAttPage ();

			VerbalITTAttrib pAttrib = this.AttAtom.GetAttrib () as VerbalITTAttrib;

			if (null != pAttrib)
			{
				SmartAttPage.WndVisible = pAttrib.IsAtomHidden;
				SmartAttPage.Vanish = pAttrib.IsVanish;
                SmartAttPage.atomDisabled = pAttrib.IsDisabled;                 // 20250207 KH 비활성화기능 추가
                SmartAttPage.ApiKey = pAttrib.ApiKey;
				SmartAttPage.DetectionType = pAttrib.DetectionType;

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

			return SmartAttPage;
		}

		public override void OnUpdateAtt ()
		{
			SmartVerbalITTAttPage SmartAttPage = this.CurrentAttPage as SmartVerbalITTAttPage;

			if (null != SmartAttPage)
			{
				VerbalITTAttrib pAttrib = this.AttAtom.GetAttrib () as VerbalITTAttrib;

				if (null != pAttrib)
				{
					VerbalITTAttrib pOrgAttrib = new VerbalITTAttrib ();
					CloneObject.CloneProperty (pAttrib, pOrgAttrib);

					pAttrib.IsAtomHidden = SmartAttPage.WndVisible;
					pAttrib.IsVanish = SmartAttPage.Vanish;
                    pAttrib.IsDisabled = SmartAttPage.atomDisabled;     // 20250207 KH 비활성화기능 추가
                    pAttrib.ApiKey = SmartAttPage.ApiKey;
					pAttrib.DetectionType = SmartAttPage.DetectionType;

					pAttrib.InputMethod = SmartAttPage.InputMethod;
					pAttrib.OutputMethod = SmartAttPage.OutputMethod;

					/* 원본
                    pAttrib.InputValue = SmartAttPage.InputValue;
                    pAttrib.OutputValue = SmartAttPage.OutputValue;
                    */

					pAttrib.InputValue = SMProperVar_Eng.GetSaveData_Atom (SmartAttPage.InputValue);
					pAttrib.OutputValue = SMProperVar_Eng.GetSaveData_Atom (SmartAttPage.OutputValue);


					if (false == CloneObject.IsEqualProperty (pOrgAttrib, pAttrib))
					{
						pAttrib.ChangeAttribCommand (pOrgAttrib);
						AttAtom.CompletePropertyChanged ();
					}
				}
			}
		}
	}
}
