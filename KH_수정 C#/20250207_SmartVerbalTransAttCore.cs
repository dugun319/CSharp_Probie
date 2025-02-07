using System.Windows.Controls;

using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopAtom.Verbal;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPage;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;

namespace Softpower.SmartMaker.TopSmartAtomManager.SmartVerbalCore
{
	public class SmartVerbalTransAttCore : SmartAtomAttCore
	{
		public SmartVerbalTransAttCore (Atom pAtom)
			: base (pAtom)
		{

		}

		public override UserControl GetAttPage ()
		{
			SmartVerbalTransAttPage SmartAttPage = new SmartVerbalTransAttPage ();

			VerbalTransAttrib pAttrib = this.AttAtom.GetAttrib () as VerbalTransAttrib;

			if (null != pAttrib)
			{
				SmartAttPage.WndVisible = pAttrib.IsAtomHidden;
				SmartAttPage.Vanish = pAttrib.IsVanish;
                SmartAttPage.atomDisabled = pAttrib.IsDisabled;                 // 20250207 KH 비활성화기능 추가
                SmartAttPage.SourceLanguage = pAttrib.SourceLanguage;
				SmartAttPage.TargetLanguage = pAttrib.TargetLanguage;
				SmartAttPage.EnableTag = pAttrib.IsEnableTag;
				SmartAttPage.EnableSentence = pAttrib.IsEnableSentence;
				SmartAttPage.EnableParagraph = pAttrib.IsEnableParagraph;
				SmartAttPage.InputMethod = pAttrib.InputMethod;
				SmartAttPage.OutputMethod = pAttrib.OutputMethod;
				SmartAttPage.OutputTts = pAttrib.OutputTts;

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
			SmartVerbalTransAttPage SmartAttPage = this.CurrentAttPage as SmartVerbalTransAttPage;

			if (null != SmartAttPage)
			{
				VerbalTransAttrib pAttrib = this.AttAtom.GetAttrib () as VerbalTransAttrib;

				if (null != pAttrib)
				{
					VerbalTransAttrib pOrgAttrib = new VerbalTransAttrib ();
					CloneObject.CloneProperty (pAttrib, pOrgAttrib);

					pAttrib.IsAtomHidden = SmartAttPage.WndVisible;
					pAttrib.IsVanish = SmartAttPage.Vanish;
                    pAttrib.IsDisabled = SmartAttPage.atomDisabled;     // 20250207 KH 비활성화기능 추가
                    pAttrib.SourceLanguage = SmartAttPage.SourceLanguage;
					pAttrib.TargetLanguage = SmartAttPage.TargetLanguage;
					pAttrib.IsEnableTag = SmartAttPage.EnableTag;
					pAttrib.IsEnableSentence = SmartAttPage.EnableSentence;
					pAttrib.IsEnableParagraph = SmartAttPage.EnableParagraph;
					pAttrib.InputMethod = SmartAttPage.InputMethod;
					pAttrib.OutputMethod = SmartAttPage.OutputMethod;
					pAttrib.OutputTts = SmartAttPage.OutputTts;

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
						AttAtom.CompletePropertyChanged ();
					}
				}
			}
		}
	}
}
