using System;
using System.Windows;
using System.Windows.Media;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.TopApp;

namespace Softpower.SmartMaker.TopProcess
{
	public enum ToolBarUpdateType
	{
		None = -1,
		View = 0,
		Atom = 1,
		WebDynamicGrid = 2, //반응형웹
	}

	public class ToolBarProperty
	{
		public ToolBarUpdateType ToolBarType { get; set; }
		public TextDecorationLocation UpdateTextDecorationLocation { get; set; }
		public HorizontalAlignment UpdateHorizontalAlignment { get; set; }
		public VerticalAlignment UpdateVerticalAlignment { get; set; }

		public Type TargetType { get; set; }
		public FontFamily UpdateFontFamily { get; set; }
		public FontWeight UpdateFontWeight { get; set; }
		public FontStyle UpdateFontStyle { get; set; }

		public Brush LineBrush { get; set; }
		public Brush BackgroundBrush { get; set; }
		public Brush FontBrush { get; set; }

		public Thickness LineThickness { get; set; }

		public DoubleCollection UpdateDoubleCollection { get; set; }

		public bool? IsLineColorEnabled { get; set; }
		public bool? IsBackgroundExpandArea { get; set; }

		public bool IsNoLIne { get; set; }
		public bool IsNoBackgrond { get; set; }
		public bool IsHide { get; set; }
		public bool IsCellBordertypeVisibilty { get; set; }
		public bool IsShadow { get; set; }

		public int AtomOpactiy { get; set; }
		public int SelectAtomCount { get; set; }

        //20250211 KH CalendarAttrib Get BorderIndex
        public int SelectAtomeBorderIndex { get; set; }

        public double FontSize { get; set; }

		public string AtomName { get; set; }
		public string ValueType { get; set; }

		public ToolBarProperty ()
		{
			SelectAtomCount = 0;
			AtomOpactiy = 100;

			UpdateFontFamily = new FontFamily (LC.GS ("EditorRes80_ScrEnv_1"));
			FontSize = PQAppBase.DefaultFontSize;
			FontBrush = Brushes.Black;
			UpdateFontWeight = FontWeights.Normal;
			UpdateFontStyle = FontStyles.Normal;

            SelectAtomeBorderIndex = 0;

            UpdateHorizontalAlignment = HorizontalAlignment.Left;
			UpdateVerticalAlignment = VerticalAlignment.Center;
			UpdateTextDecorationLocation = TextDecorationLocation.Baseline;

			IsLineColorEnabled = null;
			IsBackgroundExpandArea = null;
			IsShadow = false;
		}
	}
}
