using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.Define;
using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;

namespace Softpower.SmartMaker.TopSmartAtom
{
	public partial class SmartLocAddrofAtom : SmartLocAddrAtomBase
	{
		private DispatcherTimer m_timer;

		public SmartLocAddrofAtom ()
		{
			InitializeComponent ();

			//기본 이미지 셋팅
			InitializeDefaultImage ();
		}

		public SmartLocAddrofAtom (Atom atomCore) : base (atomCore)
		{
			InitializeComponent ();

			//기본 이미지 셋팅
			InitializeDefaultImage ();
		}

		protected override void InitializeAtomCore ()
		{
			m_AtomCore = new SmartLocAddrAtom ();
		}

		protected override void InitializeAtomSize ()
		{
			Size atomSize = DefaultAtomSizeManager.GetDefaultRect (AtomType.Location);
			this.Width = atomSize.Width;
			this.Height = atomSize.Height;
		}

		public override void SetAtomBorder (Brush applyBrush)
		{
			AtomBorderRectangle.Stroke = applyBrush;
        }

        protected override void InitializeResizeAdorner ()
        {
            m_ResizeAdorner = new Point8Adorner (this);
        }

        public override Brush GetAtomBorder ()
		{
			return AtomBorderRectangle.Stroke;
		}

		public override void SetAtomThickness (Thickness applyThickness)
		{
			AtomBorderRectangle.StrokeThickness = applyThickness.Left;
		}

		public override Thickness GetAtomThickness ()
		{
			return new Thickness (AtomBorderRectangle.StrokeThickness);
		}

		public override void SetAtomDashArray (DoubleCollection applyDashArray)
		{
			AtomBorderRectangle.StrokeDashArray = applyDashArray;
		}

		public override DoubleCollection GetAtomDashArray ()
		{
			return AtomBorderRectangle.StrokeDashArray;
		}

		#region Apply/Release RunModeProperty

		public override void ReleaseRunModeProperty ()
		{
			base.ReleaseRunModeProperty ();

			AddrTextBlock.Text = "";

            //AddrTextBlock.Visibility = System.Windows.Visibility.Collapsed;

			StopTimer ();
		}

		public override void ApplyRunModeProperty ()
		{
			base.ApplyRunModeProperty ();

			if (AtomCore.Attrib is SmartLocAddrAttrib atomAttrib)
			{
				if (!atomAttrib.IsDisableListen)
				{
					CheckCurrAddress ();

					ExecuteLocaddrUpdate ();
				}
			}

		}

		public override void DoPostRunMode ()
		{
			if (AtomCore.Attrib is SmartLocAddrAttrib atomAttrib)
			{
				if (!atomAttrib.IsDisableListen)
				{
					ExecuteLocaddrUpdate ();
				}
			}
			StartTimer ();
		}

		#endregion

		public void CheckCurrAddress ()
		{
			SmartLocAddrAtom atomCore = AtomCore as SmartLocAddrAtom;
			if (null != atomCore)
			{
				atomCore.SetNowPosition ();
				SetView ();
			}
		}

		public string GetLocationValue (bool bAtomDisplay)
		{
			string strValue = "";

			SmartLocAddrAtom atomCore = AtomCore as SmartLocAddrAtom;

			SmartLocAddrAttrib atomAttrib = atomCore.GetAttrib () as SmartLocAddrAttrib;
			switch (atomAttrib.LocDisplayType)
			{
				case LOCDISPLAY_TYPE.Coordinate:    // 좌표값
					if (false == string.IsNullOrEmpty (atomCore.Latitude) && false == string.IsNullOrEmpty (atomCore.Longitude))
					{
						if (false != bAtomDisplay)
						{
							strValue = string.Format (LC.GS ("TopSmartAtom_SmartLocAddrofAtom_1"), atomCore.Latitude, atomCore.Longitude);
						}
						else
						{
							strValue = string.Format ("{0},{1}", atomCore.Latitude, atomCore.Longitude);
						}
					}
					else
					{
						strValue = string.Empty;
					}
					break;
				case LOCDISPLAY_TYPE.Address:       // 주소값 (법정동)
					strValue = atomCore.AddressLegalcode;
					break;
				case LOCDISPLAY_TYPE.Post:          // 우편번호
					strValue = atomCore.PostalCode;
					break;
				case LOCDISPLAY_TYPE.Admcode:       // 주소값 (행정동)
					strValue = atomCore.AddressAdmcode;
					break;
				case LOCDISPLAY_TYPE.Addr:          // 주소값 (지번주소)
					strValue = atomCore.AddressAddr;
					break;
				case LOCDISPLAY_TYPE.Roadaddr:      // 주소값 (도로명 주소)
					strValue = atomCore.AddressRoadaddr;
					break;
			}

			return strValue;
		}

		public void SetView ()
		{
			string strValue = GetLocationValue (true);

			this.AddrTextBlock.Text = strValue;
		}

		private void InitializeDefaultImage ()
		{
		}

		public override void CloneAtom (AtomBase ClonedAtom, bool bDeepCopy)
		{
			SmartLocAddrofAtom LocAddrAtom = ClonedAtom as SmartLocAddrofAtom;
			LocAddrAtom.SetAtomBackground (this.GetAtomBackground ());
			LocAddrAtom.SetAtomBorder (this.GetAtomBorder ());
			LocAddrAtom.SetAtomDashArray (this.GetAtomDashArray ());
			LocAddrAtom.SetAtomThickness (this.GetAtomThickness ());
			base.CloneAtom (ClonedAtom, bDeepCopy);
		}

		public override void SerializeLoadSync_AttribToAtom (bool bIs80Model)
		{
			base.SerializeLoadSync_AttribToAtom (bIs80Model);
		}

		public void StartTimer ()
		{
			SmartLocAddrAttrib atomAttrib = this.AtomCore.GetAttrib () as SmartLocAddrAttrib;

			if (false == atomAttrib.IsDisableListen)
			{
				m_timer = new DispatcherTimer ();

				m_timer.Interval = TimeSpan.FromSeconds (atomAttrib.RefreshTime);
				m_timer.Tick += new EventHandler (Timer_Tick);
				m_timer.Start ();
			}
		}

		public void StopTimer ()
		{
			if (null != m_timer)
			{
				m_timer.Stop ();
				m_timer = null;
			}
		}

		private void Timer_Tick (object sender, EventArgs e)
		{
			ExecuteLocaddrUpdate ();
		}

		private void ExecuteLocaddrUpdate ()
		{
			Dispatcher.Invoke (DispatcherPriority.Normal, new Action (delegate
			{
				CheckCurrAddress ();

                if (this.AtomCore.AtomRunMode == RUNMODE_TYPE.PLAY_MODE)
				{
					SmartLocAddrAtom locationAtomCore = AtomCore as SmartLocAddrAtom;

					// (주소값, 위도, 경도, 고도) 전달값
					CVariantX[] pvaArgs = new CVariantX[4 + 1];

					pvaArgs[0] = new CVariantX (4);
					pvaArgs[1] = new CVariantX (GetLocationValue (false));    // 주소값
					pvaArgs[2] = new CVariantX (locationAtomCore.Latitude);   // 위도
					pvaArgs[3] = new CVariantX (locationAtomCore.Longitude);   // 경도
					pvaArgs[4] = new CVariantX ("");   // 고도
													   //

					//위치정보가 갱신되면 다음 문단을 실행한다.(주소값, 위도, 경도, 고도)
					if (-1 != this.AtomCore.ProcessEvent (0, EVS_TYPE.EVS_A_LOCADDR_UPDATE, pvaArgs))
					{
						if (0 <= this.AtomCore.OnCallMsgHandler (EVS_TYPE.EVS_A_LOCADDR_UPDATE, pvaArgs))
						{
							this.AtomCore.ProcessEvent (1, EVS_TYPE.EVS_A_LOCADDR_UPDATE, pvaArgs);
						}
					}
				}
			}));
		}
	}
}
