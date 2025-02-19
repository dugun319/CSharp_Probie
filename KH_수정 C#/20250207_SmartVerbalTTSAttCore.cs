using System.Windows.Controls;

using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopAtom.Verbal;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPage;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;

namespace Softpower.SmartMaker.TopSmartAtomManager.SmartVerbalCore
{
	public class SmartVerbalTTSAttCore : SmartAtomAttCore
	{
		public SmartVerbalTTSAttCore (Atom pAtom)
			: base (pAtom)
		{

		}

		public override UserControl GetAttPage ()
		{
			SmartVerbalTTSAttPage SmartAttPage = new SmartVerbalTTSAttPage ();

			VerbalTTSAttrib pAttrib = this.AttAtom.GetAttrib () as VerbalTTSAttrib;

			if (null != pAttrib)
			{
				SmartAttPage.WndVisible = pAttrib.IsAtomHidden;
				SmartAttPage.Vanish = pAttrib.IsVanish;
                SmartAttPage.atomDisabled = pAttrib.IsDisabled;                 // 20250207 KH 비활성화기능 추가
                SmartAttPage.LanguageCode = pAttrib.LanguageCode;
				SmartAttPage.VoiceType = pAttrib.VoiceType;
				SmartAttPage.VoicePitch = pAttrib.VoicePitch;
				SmartAttPage.VoiceRate = pAttrib.VoiceRate;
				SmartAttPage.SoundVolume = pAttrib.SoundVolume;
				SmartAttPage.EnableTag = pAttrib.IsEnableTag;
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
			SmartVerbalTTSAttPage SmartAttPage = this.CurrentAttPage as SmartVerbalTTSAttPage;

			if (null != SmartAttPage)
			{
				VerbalTTSAttrib pAttrib = this.AttAtom.GetAttrib () as VerbalTTSAttrib;

				if (null != pAttrib)
				{
					VerbalTTSAttrib pOrgAttrib = new VerbalTTSAttrib ();
					CloneObject.CloneProperty (pAttrib, pOrgAttrib);

					pAttrib.IsAtomHidden = SmartAttPage.WndVisible;
					pAttrib.IsVanish = SmartAttPage.Vanish;
                    pAttrib.IsDisabled = SmartAttPage.atomDisabled;     // 20250207 KH 비활성화기능 추가
                    pAttrib.LanguageCode = SmartAttPage.LanguageCode;
					pAttrib.VoiceType = SmartAttPage.VoiceType;
					pAttrib.VoicePitch = SmartAttPage.VoicePitch;
					pAttrib.VoiceRate = SmartAttPage.VoiceRate;
					pAttrib.SoundVolume = SmartAttPage.SoundVolume;
					pAttrib.IsEnableTag = SmartAttPage.EnableTag;
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
						pAttrib.ChangeAttribCommand (pOrgAttrib);
						AttAtom.CompletePropertyChanged ();
					}
				}
			}
		}
	}
}
