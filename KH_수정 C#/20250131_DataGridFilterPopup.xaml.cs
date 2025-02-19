using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Softpower.SmartMaker.Common.Localization;
using Softpower.SmartMaker.DelegateEventResource;

namespace Softpower.SmartMaker.TopAtom.DataGrids
{
	/// <summary>
	/// DataGridFilterPopup.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class DataGridFilterPopup : Popup
	{
		public event CommonDelegateEvents.OnNotifyObjectEventHandler OnSelectItem;

		public DataGridFilterPopup ()
		{
			InitializeComponent ();
		}

		public void SetPopup (string strColumnName, List<string> data)
		{
			ColumnLabel.Content = strColumnName;

			RootStackPanel.Children.Clear ();

			TextBlock clearBlock = new TextBlock ();
			clearBlock.Text = LC.GS ("(필터 초기화)"); ;
			clearBlock.Height = 30;

			clearBlock.MouseEnter += Block_MouseEnter;
			clearBlock.MouseLeave += Block_MouseLeave;

			clearBlock.MouseLeftButtonDown += clearBlock_MouseLeftButtonDown;

			RootStackPanel.Children.Add (clearBlock);

			foreach (string item in data)
			{
				TextBlock block = new TextBlock ();
				block.Text = item;
				block.Height = 30;

				block.MouseEnter += Block_MouseEnter;
				block.MouseLeave += Block_MouseLeave;

				block.MouseLeftButtonDown += Block_MouseLeftButtonDown;

				RootStackPanel.Children.Add (block);
			}

		}

		public void SetBorderColor (Brush brush)
		{
			Debug.WriteLine($"public void SetBorderColor (Brush brush) {brush}");
			BorderColor.BorderBrush = brush;
		}

		public void SetBackColor (Brush brush)
		{
			backgroundColor.Background = brush;
		}

		private void Block_MouseLeave (object sender, MouseEventArgs e)
		{
			TextBlock block = sender as TextBlock;

			if (null != block)
			{
				block.Background = Brushes.White;
			}
		}

		private void Block_MouseEnter (object sender, MouseEventArgs e)
		{
			TextBlock block = sender as TextBlock;

			if (null != block)
			{
				block.Background = Brushes.SkyBlue;
			}
		}

		private void Block_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			TextBlock block = sender as TextBlock;

			if (null != block)
			{
				ClosePopup ();

				if (null != OnSelectItem)
				{
					string strColumn = ColumnLabel.Content.ToString ();
					string strData = string.Format ("{0} = '{1}'", strColumn, block.Text);
					OnSelectItem (strData);
				}
			}
		}

		void clearBlock_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
		{
			TextBlock block = sender as TextBlock;

			if (null != block)
			{
				ClosePopup ();
				if (null != OnSelectItem)
				{
					OnSelectItem (""); //필터를 초기화
				}
			}
		}

		public void ShowPopup ()
		{
			this.StaysOpen = true;
			this.IsOpen = true;
			this.StaysOpen = false;
        }

		public void ClosePopup ()
		{
			this.StaysOpen = true;
			this.IsOpen = false;
			this.StaysOpen = false;
		}
	}
}
