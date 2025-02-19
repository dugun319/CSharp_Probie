using System.Collections.Generic;
using System.Windows.Controls;

using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopAtom.Verbal;
using Softpower.SmartMaker.TopAtom.Verbal.Voice;
using Softpower.SmartMaker.TopLight.ViewModels;
using Softpower.SmartMaker.TopLight.Voice;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPage;
using Softpower.SmartMaker.TopSmartAtomManager.BaseCore;

namespace Softpower.SmartMaker.TopSmartAtomManager.SmartVerbalCore
{
	public class SmartVerbalSTTAttCore : SmartAtomAttCore
	{
		public SmartVerbalSTTAttCore (Atom pAtom)
			: base (pAtom)
		{

		}

		public override UserControl GetAttPage ()
		{
			SmartVerbalSTTAttPage SmartAttPage = new SmartVerbalSTTAttPage ();

			VerbalSTTAttrib pAttrib = this.AttAtom.GetAttrib () as VerbalSTTAttrib;

			if (null != pAttrib)
			{
				SmartAttPage.WndVisible = pAttrib.IsAtomHidden;
				SmartAttPage.Vanish = pAttrib.IsVanish;
                SmartAttPage.atomDisabled = pAttrib.IsDisabled;					// 20250207 KH 비활성화기능 추가
                SmartAttPage.LanguageCode = pAttrib.LanguageCode;
				SmartAttPage.FromDecibel = pAttrib.FromDecibel;
				SmartAttPage.ToDecibel = pAttrib.ToDecibel;
				SmartAttPage.DelayTime = pAttrib.DelayTime;
				SmartAttPage.RepeatRecognize = pAttrib.IsRepeatRecognize;
				SmartAttPage.ScreenTouch = pAttrib.IsScreenTouch;
				SmartAttPage.ShakeDevice = pAttrib.IsShakeDevice;
				SmartAttPage.OutputMethod = pAttrib.OutputMethod;
				SmartAttPage.InputMethod = pAttrib.InputMethod;

				SmartAttPage.OutputTrans = pAttrib.OutputTrans;

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

				CloneObject.CloneListObject (pAttrib.VoiceCommandTable.VoiceCommandItems, SmartAttPage.VoiceCommandItems);

				//
				TopView pTopView = (TopView)(this.AttAtom.Information.GetOwnerWnd ());
				TopDoc pTopDoc = pTopView.GetDocument ();
				if (null != pTopDoc)
				{
					List<string> listAnimation = VoiceCommandManager.Instance.GetAnimationList (pTopDoc as LightJDoc);
					List<string> listScriptFunction = VoiceCommandManager.Instance.GetScriptFunctionList (pTopDoc as LightJDoc);
					Dictionary<string, string> dicAnimationKey = VoiceCommandManager.Instance.GetAnimationDictionaryKey (pTopDoc as LightJDoc);
					Dictionary<string, string> dicAnimationValue = VoiceCommandManager.Instance.GetAnimationDictionaryValue (pTopDoc as LightJDoc);

					VoiceCommandHelper.Instance.SetAnimationList (listAnimation);
					VoiceCommandHelper.Instance.SetAnimationDictionary (dicAnimationKey, dicAnimationValue);
					VoiceCommandHelper.Instance.SetScriptFunctionList (listScriptFunction);
				}
				//

				SmartAttPage.Information = this.AttAtom.Information;
			}

			return SmartAttPage;
		}

		public override void OnUpdateAtt ()
		{
			SmartVerbalSTTAttPage SmartAttPage = this.CurrentAttPage as SmartVerbalSTTAttPage;

			if (null != SmartAttPage)
			{
				VerbalSTTAttrib pAttrib = this.AttAtom.GetAttrib () as VerbalSTTAttrib;

				if (null != pAttrib)
				{
					VerbalSTTAttrib pOrgAttrib = new VerbalSTTAttrib ();
					CloneObject.CloneProperty (pAttrib, pOrgAttrib);

					pAttrib.IsAtomHidden = SmartAttPage.WndVisible;
					pAttrib.IsVanish = SmartAttPage.Vanish;
					pAttrib.IsDisabled = SmartAttPage.atomDisabled;		// 20250207 KH 비활성화기능 추가
                    pAttrib.LanguageCode = SmartAttPage.LanguageCode;
					pAttrib.FromDecibel = SmartAttPage.FromDecibel;
					pAttrib.ToDecibel = SmartAttPage.ToDecibel;
					pAttrib.DelayTime = SmartAttPage.DelayTime;
					pAttrib.IsRepeatRecognize = SmartAttPage.RepeatRecognize;
					pAttrib.IsScreenTouch = SmartAttPage.ScreenTouch;
					pAttrib.IsShakeDevice = SmartAttPage.ShakeDevice;
					pAttrib.OutputMethod = SmartAttPage.OutputMethod;
					pAttrib.InputMethod = SmartAttPage.InputMethod;
					pAttrib.OutputTrans = SmartAttPage.OutputTrans;

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

					CloneObject.CloneListObject (SmartAttPage.VoiceCommandItems, pAttrib.VoiceCommandTable.VoiceCommandItems);

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
