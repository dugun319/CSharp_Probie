using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CefSharp;

using Softpower.SmartMaker.Common.Global;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopWebAtom.Component.HtmlEditor;

namespace Softpower.SmartMaker.TopWebAtom
{
	/// <summary>
	/// Interaction logic for DHtmlEditofAtom.xaml
	/// </summary>
	public partial class WebDHtmlEditofAtom : WebDHtmlEditAtomBase
	{
		//private double m_OldHeight;

		public WebDHtmlEditofAtom ()
		{
			InitializeComponent ();

			InitEvent ();
			InitStyle ();
		}

		public WebDHtmlEditofAtom (Atom atomCore)
			: base (atomCore)
		{

			InitializeComponent ();

			InitEvent ();
			InitStyle ();
		}

		private void InitStyle ()
		{
			HtmlEditorManager.IsToolBar = false;
			HtmlEditorManager.IsDropTimer = false;

			SetHtmlEditorManagerAttrib ();
		}

		private void InitEvent ()
		{
			this.PreviewMouseLeftButtonDown += DHtmlEditofAtom_PreviewMouseLeftButtonDown;
		}

		public void AutoSize ()
		{
			if (!IsLoaded)
			{
				this.Loaded += DHtmlEditofAtom_Loaded;
				return;
			}

			if (HtmlEditorManager.ChromeLoad && !HtmlEditorManager.IsBrowserInitialized)
			{
				HtmlEditorManager.ChromeEditor.IsBrowserInitializedChanged += ChromeEditor_IsBrowserInitializedChanged;
				return;
			}

			if (!HtmlEditorManager.ChromeLoad)
			{
				HtmlEditorManager.OnChromeLoadCompleted += HtmlEditorManager_OnChromeLoadCompleted;
				return;
			}

			if (HtmlEditorManager.ChromeEditor.IsLoading)
			{
				HtmlEditorManager.ChromeEditor.LoadingStateChanged += ChromeEditor_LoadingStateChanged;
				return;
			}

			this.Dispatcher.BeginInvoke (new Action (async delegate
			{
				await AutoSizeToHtmlSize ();
			}));

		}

		public async Task AutoSizeToHtmlSize ()
		{
			string sh = await HtmlEditorManager.ChromeEditor.GetTotalHeight ();
			if (!string.IsNullOrEmpty (sh))
			{
				double docHeight = _Kiss.toDouble (sh);
				Application.Current.Dispatcher.Invoke (() =>
				{
					this.Height = 0 < docHeight ? docHeight + HtmlEditorManager.TopPadding + 2 : docHeight;
					ChangeBoundStatus ();
				});
			}
		}

		private void ChromeEditor_LoadingStateChanged (object sender, LoadingStateChangedEventArgs e)
		{
			HtmlEditorManager.ChromeEditor.LoadingStateChanged -= ChromeEditor_LoadingStateChanged;
			AutoSize ();
		}

		private void HtmlEditorManager_OnChromeLoadCompleted ()
		{
			HtmlEditorManager.OnChromeLoadCompleted -= HtmlEditorManager_OnChromeLoadCompleted;
			AutoSize ();
		}

		private void ChromeEditor_IsBrowserInitializedChanged (object sender, DependencyPropertyChangedEventArgs e)
		{
			HtmlEditorManager.ChromeEditor.IsBrowserInitializedChanged -= ChromeEditor_IsBrowserInitializedChanged;
			AutoSize ();
		}

		private void DHtmlEditofAtom_Loaded (object sender, RoutedEventArgs e)
		{
			this.Loaded -= DHtmlEditofAtom_Loaded;
			AutoSize ();
		}

		#region | 이벤트 |

		void DHtmlEditofAtom_PreviewMouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{

		}

		#endregion //이벤트

		#region override

		protected override void InitializeAtomCore ()
		{
			m_AtomCore = new WebDHtmlEditAtom ();
		}

		protected override void InitializeResizeAdorner ()
		{
			m_ResizeAdorner = new Point8Adorner (this);
		}

		protected override void InitializeAtomSize ()
		{
			Size atomSize = DefaultAtomSizeManager.GetDefaultRect (AtomType.WebDHtmlEdit);
			this.Width = atomSize.Width;
			this.Height = atomSize.Height;
		}

		public override void SetAtomFontSize (double dApplySize)
		{
			base.SetAtomFontSize (dApplySize);
			HtmlEditorManager.DefaultFontSize = dApplySize.ToString ();
		}

		public override void SetAtomFontColor (Brush applyBrush)
		{
			base.SetAtomFontColor (applyBrush);

			if (applyBrush is SolidColorBrush)
			{
				HtmlEditorManager.FontColor = ((SolidColorBrush)applyBrush).Color;
			}
		}

		public override void SetAtomFontFamily (FontFamily applyFontFamily)
		{
			base.SetAtomFontFamily (applyFontFamily);
			HtmlEditorManager.FontName = FontFamily.ToString ();
		}


		public override void SetAtomBackground (Brush applyBrush)
		{
			this.Background = applyBrush;
		}

		public override Brush GetAtomBackground ()
		{
			return this.Background;
		}

		public override void SetAtomBorder (Brush applyBrush)
		{
			RootBorder.BorderBrush = applyBrush;
		}

		public override Brush GetAtomBorder ()
		{
			return RootBorder.BorderBrush;
		}

		public override void SetAtomThickness (Thickness applyThickness)
		{
			RootBorder.BorderThickness = applyThickness;
		}

		public override Thickness GetAtomThickness ()
		{
			return RootBorder.BorderThickness;
		}

		public override void CloneAtom (AtomBase ClonedAtom, bool bDeepCopy)
		{
			ClonedAtom.SetAtomBackground (this.GetAtomBackground ());
			ClonedAtom.SetAtomBorder (this.GetAtomBorder ());
			ClonedAtom.SetAtomDashArray (this.GetAtomDashArray ());
			ClonedAtom.SetAtomThickness (this.GetAtomThickness ());
			ClonedAtom.SetTextUnderLine (this.GetTextUnderLine ());
			base.CloneAtom (ClonedAtom, bDeepCopy);
		}

		public override void NotifyChangedValueByInnerLogic (object changedValue)
		{
			base.NotifyChangedValueByInnerLogic (changedValue);

			if (AtomCore.GetAttrib () is WebDHtmlEditAttrib atomAttrib)
			{
				if (atomAttrib.IsAutoSizable) AutoSize ();
			}
		}

		public override void ChangeAtomMode (int nRunMode)
		{
			WebDHtmlEditAttrib atomAttrib = AtomCore.GetAttrib () as WebDHtmlEditAttrib;
			base.ChangeAtomMode (nRunMode);

			switch (nRunMode)
			{
				case 0:
					{
						this.IsEnabledScroll = false;
						AtomImage.Visibility = Visibility.Visible;

						HtmlEditorManager.Visibility = Visibility.Collapsed;
						HtmlEditorManager.CloseAtom ();
					}
					break;
				case 1:
					{
						this.IsEnabledScroll = true;
						AtomImage.Visibility = Visibility.Collapsed;

						HtmlEditorManager.AtomName = this.AtomCore.GetProperVar ();
						HtmlEditorManager.Visibility = Visibility.Visible;
						HtmlEditorManager.EditMode ();
						if (null != HtmlEditorManager.ChromeEditor) HtmlEditorManager.ChromeEditor.Focusable = atomAttrib.IsEditMode;

						if (true == atomAttrib.IsEditMode)
						{
							HtmlEditorManager.ShowToolBar ();
						}
					}
					break;
			}
		}

		public override void Sync_AttribToAtom ()
		{
			base.Sync_AttribToAtom ();

			var atomAttrib = this.AtomCore.GetAttrib () as WebDHtmlEditAttrib;
			if (null != atomAttrib)
			{
				AtomCore.SetContentString (atomAttrib.HtmlBody, true);
			}
		}

		public void SetEditMode ()
		{
			WebDHtmlEditAttrib atomAttrib = this.AtomCore.GetAttrib () as WebDHtmlEditAttrib;

			if (null != atomAttrib)
			{
				if (true == atomAttrib.IsEditMode)
				{
					HtmlEditorManager.ShowToolBar ();
				}
				else
				{
					HtmlEditorManager.HideToolBar ();
				}
			}
		}

		public override void ReleaseRunModeProperty ()
		{
			base.ReleaseRunModeProperty ();
		}

		public override void ApplyRunModeProperty ()
		{
			base.ApplyRunModeProperty ();

		}

		public override void DoPostRunMode ()
		{
			base.DoPostRunMode ();
		}

		public override void CompletePropertyChanged ()
		{
			WebDHtmlEditAttrib atomAttrib = this.AtomCore.GetAttrib () as WebDHtmlEditAttrib;

			if (null != atomAttrib)
			{
				SetHtmlEditorManagerAttrib ();

				if (null == HtmlEditorManager.ChromeEditor)
				{
					HtmlEditorManager.ChromeSetting ();
				}
				else
				{
					HtmlEditorManager.SetHtmlStyle ();
				}

				if (false == string.IsNullOrEmpty (atomAttrib.HtmlBody))
				{
					HtmlEditorManager.LoadHtml (atomAttrib.HtmlBody);
					AtomImage.Visibility = Visibility.Collapsed;
				}
				else
				{
					AtomImage.Visibility = Visibility.Visible;
				}
			}
		}

		public override void ChangeBoundStatus ()
		{
			var atomCore = AtomCore as WebDHtmlEditAtom;
			var atomAttrib = atomCore.GetAttrib () as WebDHtmlEditAttrib;

			if (atomAttrib.IsAutoSizable)
			{

				//double unitHeight =  HtmlEditorManager.GetContentSizeHeight ();
				//this.Height = unitHeight;
			}

			base.ChangeBoundStatus ();
		}

		private void SetHtmlEditorManagerAttrib ()
		{
			WebDHtmlEditAttrib atomAttrib = this.AtomCore.GetAttrib () as WebDHtmlEditAttrib;

			if (null != atomAttrib)
			{
				HtmlEditorManager.LeftPadding = atomAttrib.LeftMargin;
				HtmlEditorManager.TopPadding = atomAttrib.TopMargin;
				//HtmlEditorManager.LineHeight = atomAttrib.LineHeight;
				//HtmlEditorManager.LetterSpacing = atomAttrib.LetterSpacing;

				if (true == atomAttrib.IsScrollBars)
				{
					HtmlEditorManager.Overflow = OVERFLOW.AUTO;
				}
				else
				{
					HtmlEditorManager.Overflow = OVERFLOW.HIDDEN;
				}
			}
		}

		public override void CloseAtom ()
		{
			base.CloseAtom ();
			HtmlEditorManager.CloseAtom ();
		}

		#endregion override

		public string GetContentString ()
		{
			return HtmlEditorManager.GetInnerText ();
		}

		public string GetInnerHtml ()
		{
			return HtmlEditorManager.GetInnerHtml ();
		}

		public void SetInnerHtml (string strHtml)
		{
			HtmlEditorManager.LoadHtml (strHtml);
		}
	}
}
