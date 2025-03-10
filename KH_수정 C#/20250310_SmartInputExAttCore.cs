using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopAtom.Ebook;
using Softpower.SmartMaker.TopSmartAtomEdit.AttFieldPage;
using Softpower.SmartMaker.TopSmartAtomEdit.AttPage;
using Softpower.SmartMaker.TopWebAtom;
using Softpower.SmartMaker.TopWebAtom.Data;

namespace Softpower.SmartMaker.TopSmartAtomManager.SmartCore
{
	public class SmartInputExAttCore : SmartInputAttCore
	{
		public SmartInputExAttCore (Atom SelectedAtom)
			: base (SelectedAtom)
		{
		}
		public SmartInputExAttCore (Atom SelectedAtom, List<object> SelectAtomList)
			: base (SelectedAtom, SelectAtomList)
		{
		}

		public SmartInputExAttCore (Atom SelectedAtom, bool bIsWebModel, bool bIsEBookModel, List<object> SelectAtomList)
			: base (SelectedAtom, bIsWebModel, bIsEBookModel, SelectAtomList)
		{
		}

		public override UserControl GetAttPage ()
		{
			if (this.AttAtom is InputAtom)
			{
				return base.GetAttPage ();
			}
			else if (this.AttAtom is InputSpinnerAtom)
			{
				SmartInputAttPage SmartAttPage = new SmartInputAttPage ();

				InputSpinnerAttrib pAttrib = this.AttAtom.GetAttrib () as InputSpinnerAttrib;
				if (null != pAttrib)
				{
					SmartAttPage.IsInputSpinner = true;

					SmartAttPage.WndReadOnly = pAttrib.WndReadOnly;
					SmartAttPage.BorderRadius = pAttrib.BorderRadius;
					SmartAttPage.BorderCircle = pAttrib.BorderCircle;

					SmartAttPage.InsertComma = pAttrib.InsertComma;
					SmartAttPage.Prime = pAttrib.Prime;
					SmartAttPage.NotZero = pAttrib.NotZero;
					SmartAttPage.NotDefault = pAttrib.NotDefault;

					SmartAttPage.MinNum = pAttrib.MinNum;
					SmartAttPage.MaxNum = pAttrib.MaxNum;

					SmartAttPage.WndVisible = pAttrib.IsAtomHidden;
					SmartAttPage.WndDisabled = pAttrib.IsDisabled;
					SmartAttPage.Vanish = pAttrib.IsVanish;

					if (-1 < pAttrib.LeftImageKey)
					{
						CObjectImage objectImage = null;
						pAttrib.GetGDIObjFromKey (ref objectImage, pAttrib.LeftImageKey);

						if (null != objectImage)
						{
							SmartAttPage.LeftImagePath = objectImage.ImagePath;
						}
					}

					if (-1 < pAttrib.RightImageKey)
					{
						CObjectImage objectImage = null;
						pAttrib.GetGDIObjFromKey (ref objectImage, pAttrib.RightImageKey);

						if (null != objectImage)
						{
							SmartAttPage.RightImagePath = objectImage.ImagePath;
						}
					}
				}

				return SmartAttPage;
			}
			else if (this.AttAtom is EBookGridPaperAtom)
			{
				var atomAttrib = this.AttAtom.GetAttrib () as EBookGridPaperAttrib;

				SmartInputAttPage SmartAttPage = new SmartInputAttPage ();

				SmartAttPage.IsLetterInput = true;
				SmartAttPage.GridRow = atomAttrib.Row;
				SmartAttPage.GridColumn = atomAttrib.Column;
				SmartAttPage.GridLeftMargin = atomAttrib.LeftMargin;
				SmartAttPage.GridTopMargin = atomAttrib.TopMargin;

				SmartAttPage.WndReadOnly = atomAttrib.IsReadOnly;

				SmartAttPage.WndVisible = atomAttrib.IsAtomHidden;
				SmartAttPage.WndDisabled = atomAttrib.IsDisabled;
				SmartAttPage.Vanish = atomAttrib.IsVanish;

				return SmartAttPage;
			}

			return base.GetAttPage ();
		}

		public override void OnUpdateAtt ()
		{
			SmartInputAttPage SmartAttPage = this.CurrentAttPage as SmartInputAttPage;
			if (null != SmartAttPage)
			{
				Atom pAtom = AttAtom;

				int nInputType = -1;

				if (this.AttAtom is InputAtom)
				{
					nInputType = 0;
					if (null != this.AttAtom.AutoCompleteAtom)
						nInputType = 2;
				}
				else if (this.AttAtom is InputSpinnerAtom)
				{
					nInputType = 1;
				}
				else if (this.AttAtom is EBookGridPaperAtom)
				{
					nInputType = 3;
				}

				if (nInputType != SmartAttPage.InputType)
				{
					Information info = pAtom.Information;
					//info.SetAttribInputAtom (ref pAtom, SmartAttPage.InputType);
					info.SetAttribToAtom (ref pAtom, SmartAttPage.InputType);

					if (SmartAttPage.InputType == 2)
						return;
				}

				if (pAtom is InputAtom)
				{
					base.OnUpdateInputAtt (pAtom);
				}
				else if (pAtom is InputSpinnerAtom)
				{
					InputSpinnerAttrib pAttrib = this.AttAtom.GetAttrib () as InputSpinnerAttrib;
					if (null != pAttrib)
					{
						InputSpinnerAttrib pOrgAttrib = new InputSpinnerAttrib ();
						CloneObject.CloneProperty (pAttrib, pOrgAttrib);

						pAttrib.WndReadOnly = SmartAttPage.WndReadOnly;
						pAttrib.BorderRadius = SmartAttPage.BorderRadius;
						pAttrib.BorderCircle = SmartAttPage.BorderCircle;

						pAttrib.InsertComma = SmartAttPage.InsertComma;
						pAttrib.Prime = SmartAttPage.Prime;
						pAttrib.NotZero = SmartAttPage.NotZero;
						pAttrib.NotDefault = SmartAttPage.NotDefault;

						pAttrib.MinNum = SmartAttPage.MinNum;
						pAttrib.MaxNum = SmartAttPage.MaxNum;

						pAttrib.IsAtomHidden = SmartAttPage.WndVisible;
						pAttrib.IsDisabled = SmartAttPage.WndDisabled;
						pAttrib.IsVanish = SmartAttPage.Vanish;

						bool IsLeftUnLoaded = false;
						bool IsRightUnLoaded = false;

						if (false == string.IsNullOrEmpty (SmartAttPage.LeftImagePath))
						{
							string strTempLeftImagePath = PQAppBase.KissGetFullPath (SmartAttPage.LeftImagePath);

							if (false == string.IsNullOrEmpty (strTempLeftImagePath) && false == File.Exists (strTempLeftImagePath))
							{
								IsLeftUnLoaded = true;
							}
							else
							{
								CObjectImage leftImage = new CObjectImage ();
								leftImage.ImagePath = SmartAttPage.LeftImagePath;
								pAttrib.LeftImageKey = pAttrib.GetKeyFromGDIObj (leftImage, pAttrib.LeftImageKey);
							}
						}
						else
						{
							pAttrib.LeftImageKey = -1;
							SmartAttPage.LeftImagePath = "";
						}

						if (false == string.IsNullOrEmpty (SmartAttPage.RightImagePath))
						{
							string strTempUnCheckImagePath = PQAppBase.KissGetFullPath (SmartAttPage.RightImagePath);

							if (false == string.IsNullOrEmpty (strTempUnCheckImagePath) && false == File.Exists (strTempUnCheckImagePath))
							{
								IsRightUnLoaded = true;
							}
							else
							{
								CObjectImage unCheckImage = new CObjectImage ();
								unCheckImage.ImagePath = SmartAttPage.RightImagePath;
								pAttrib.RightImageKey = pAttrib.GetKeyFromGDIObj (unCheckImage, pAttrib.RightImageKey);
							}
						}
						else
						{
							pAttrib.RightImageKey = -1;
							SmartAttPage.RightImagePath = "";
						}

						if (IsLeftUnLoaded || IsRightUnLoaded)
						{
							if (MessageBoxResult.Yes == _Message80.Show (LC.GS ("TopSmartAtomManager_SmartActionAttCore_1808_1"), "", MessageBoxButton.YesNo, MessageBoxImage.Question))
							{
								if (IsLeftUnLoaded)
								{
									pAttrib.LeftImageKey = -1;
									SmartAttPage.LeftImagePath = "";
								}

								if (IsRightUnLoaded)
								{
									pAttrib.RightImageKey = -1;
									SmartAttPage.RightImagePath = "";
								}
							}
						}

						if (false == CloneObject.IsEqualProperty (pOrgAttrib, pAttrib))
						{
							//Undo 속성 저장
							pAttrib.ChangeAttribCommand (pOrgAttrib);

							if (pAtom != AttAtom)
								pAtom.GetOfAtom ().InitAtomBaseWhenAtomCoreHasChanged (pAtom);

							pAtom.CompletePropertyChanged ();
						}
					}
				}
				else if (pAtom is EBookGridPaperAtom)
				{
					var atomAttrib = pAtom.GetAttrib () as EBookGridPaperAttrib;
					if (null != atomAttrib)
					{
						atomAttrib.Row = SmartAttPage.GridRow;
						atomAttrib.Column = SmartAttPage.GridColumn;
						atomAttrib.LeftMargin = SmartAttPage.GridLeftMargin;
						atomAttrib.TopMargin = SmartAttPage.GridTopMargin;

						atomAttrib.IsReadOnly = SmartAttPage.WndReadOnly;

						atomAttrib.IsAtomHidden = SmartAttPage.WndVisible;
						atomAttrib.IsDisabled = SmartAttPage.WndDisabled;
						atomAttrib.IsVanish = SmartAttPage.Vanish;

						pAtom.CompletePropertyChanged ();
					}
				}
			}
		}

		/// <summary>
		/// 자동완성 데이터
		/// </summary>
		/// <returns></returns>
		public override UserControl GetAutoCompletePage ()
		{
			Atom pAutoComplete = AttAtom.AutoCompleteAtom;

			SmartAutoCompleteFieldAttPage SmartAttPage = new SmartAutoCompleteFieldAttPage ();
			if (null != SmartAttPage)
			{
				AutoCompleteAttrib pAttrib = pAutoComplete.GetAttrib () as AutoCompleteAttrib;
				if (null != pAttrib)
				{
					SmartAttPage.Kind = pAttrib.Kind;
					SmartAttPage.PopupDBIndex = pAttrib.PopupDBIndex;
					SmartAttPage.PopDSN = pAttrib.PopDSN;
					SmartAttPage.FieldInfo.Clear ();
					SmartAttPage.FieldInfo.AddRange (pAttrib.FieldInfo.Clone () as ArrayList);
					SmartAttPage.InitComboList ();
					SmartAttPage.Memory = pAttrib.Memory;
				}
			}

			return SmartAttPage;
		}

		/// <summary>
		/// 자동완성 데이터
		/// </summary>
		public override void OnUpdateAutoCompleteAtt ()
		{
			bool bChange = false;

			Atom pAutoComplete = AttAtom.AutoCompleteAtom;

			SmartAutoCompleteFieldAttPage SmartAttPage = this.CurrentAttPage as SmartAutoCompleteFieldAttPage;
			if (null != SmartAttPage)
			{
				AutoCompleteAttrib pAttrib = pAutoComplete.GetAttrib () as AutoCompleteAttrib;
				if (null != pAttrib)
				{
					AutoCompleteAttrib pOrgAttrib = new AutoCompleteAttrib ();

					CloneObject.CloneProperty (pAttrib, pOrgAttrib);
					CloneObject.CloneArray (pAttrib.FieldInfo, pOrgAttrib.FieldInfo);

					pAttrib.Kind = SmartAttPage.Kind;
					pAttrib.PopupDBIndex = SmartAttPage.PopupDBIndex;
					pAttrib.PopDSN = SmartAttPage.PopDSN;

					pAttrib.FieldInfo.Clear ();
					pAttrib.FieldInfo.AddRange (SmartAttPage.FieldInfo.Clone () as ArrayList);
					pAttrib.Memory = SmartAttPage.Memory;

					if (false == CloneObject.IsEqualProperty (pOrgAttrib, pAttrib))
					{
						bChange = true;
					}

					if (!CompareArray.IsEqualProperty (pOrgAttrib.FieldInfo, pAttrib.FieldInfo))
						bChange = true;

					if (bChange)
					{
						//Undo 속성 저장
						pAttrib.ChangeAttribCommand (pOrgAttrib);

						pAutoComplete.CompletePropertyChanged ();
					}
				}
			}
		}
	}
}
