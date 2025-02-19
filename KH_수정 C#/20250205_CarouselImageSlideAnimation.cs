using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using ScrollAnimateBehavior.AttachedBehaviors;

using Softpower.SmartMaker.TopApp;
using Softpower.SmartMaker.TopAtom;
using Softpower.SmartMaker.TopControl.Components.Animation;
using Softpower.SmartMaker.TopControl.Components.Animation.Sliding;
using static Softpower.SmartMaker.TopSmartAtom.CarouselImageAtom;

namespace Softpower.SmartMaker.TopSmartAtom.Components.Carousel.Animation
{
    public class CarouselImageSlideAnimation : CarouselImageAnimation
    {
        protected SlidingAnimationControl m_SlidingAnimationControl;
        protected Stretch m_ImageStretch = Stretch.Uniform;

        public override void CreateAnimationControl()
        {
            m_SlidingAnimationControl = new SlidingAnimationControl();

            m_SlidingAnimationControl.IndexChanged += SlidingAnimationControl_IndexChanged;

            this.CarouselContainer.Children.Add(m_SlidingAnimationControl);

            ScrollAnimationBehavior.SetPointsToScroll(m_SlidingAnimationControl.ScrollViewer, this.CarouselContainer.ActualWidth);
        }

        public override void DropAnimationControl()
        {
            if (null != m_SlidingAnimationControl)
            {
                m_SlidingAnimationControl.ScrollPanel.Children.Clear();

                this.CarouselContainer.Children.Remove(m_SlidingAnimationControl);
                m_SlidingAnimationControl = null;
            }
        }

        private void SlidingAnimationControl_IndexChanged(int nSelect)
        {
            this.CurrentIndex = nSelect;
            base.IndexChangedEvent(nSelect);
        }

        // 20250204 KH SlideImage 회전로직 수정
        public override void BeginAnimation(int nSlideIndex)
        {
            System.Windows.Controls.ScrollViewer scrollViewer = m_SlidingAnimationControl.ScrollViewer;
            double containerWidth = this.CarouselContainer.ActualWidth;
            double targetPosition = containerWidth * nSlideIndex;
            DoubleAnimation horizontalAnimation = new DoubleAnimation();

            // 현재 위치 설정
            horizontalAnimation.From = scrollViewer.HorizontalOffset;

            // 마지막 슬라이드에서 첫 번째 슬라이드로 자연스럽게 이동
            if (nSlideIndex >= BitmapImageList.Count)
            {
                // 목표 위치를 '첫 번째 슬라이드 복제본'으로 설정
                targetPosition = containerWidth * (BitmapImageList.Count);

                horizontalAnimation.To = targetPosition;
                // 애니메이션 완료 후 실제 첫 번째 슬라이드 위치로 이동
                horizontalAnimation.Completed += (s, e) =>
                {
                    scrollViewer.ScrollToHorizontalOffset(0);
                };

            }
            else
            {
                horizontalAnimation.To = targetPosition;
            }

            Storyboard storyboard = new Storyboard();
            storyboard.Completed += storyboard_Completed;
            storyboard.Children.Add(horizontalAnimation);
            Storyboard.SetTarget(horizontalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(horizontalAnimation, new PropertyPath(ScrollAnimationBehavior.HorizontalOffsetProperty));

            storyboard.Begin();
        }

        /*
        public override void BeginAnimation (int nSlideIndex)
		{
			System.Windows.Controls.ScrollViewer scrollViewer = m_SlidingAnimationControl.ScrollViewer;
			double nPosition = this.CarouselContainer.ActualWidth * nSlideIndex;
			DoubleAnimation horizontalAnimation = new DoubleAnimation ();

			horizontalAnimation.From = scrollViewer.HorizontalOffset;
			if (nSlideIndex >= BitmapImageList.Count)
			{
				horizontalAnimation.From = -100;
				nPosition = this.CarouselContainer.ActualWidth * 0;
			}
			horizontalAnimation.To = nPosition;
			//horizontalAnimation.Duration = new Duration (GetTimeDuration (scrollViewer));
			//horizontalAnimation.AccelerationRatio = GetAccelerationRatio (scrollViewer);

			Storyboard storyboard = new Storyboard ();
			storyboard.Completed += storyboard_Completed;

			storyboard.Children.Add (horizontalAnimation);
			Storyboard.SetTarget (horizontalAnimation, scrollViewer);
			Storyboard.SetTargetProperty (horizontalAnimation, new PropertyPath (ScrollAnimationBehavior.HorizontalOffsetProperty));
			storyboard.Begin ();

			//ScrollAnimationBehavior.AnimateScroll (m_SlidingAnimationControl.ScrollViewer, nPosition, false);
		}
		*/

        private void storyboard_Completed(object sender, EventArgs e)
        {
        }

        public override void ClearImages()
        {
            base.ClearImages();

            m_SlidingAnimationControl?.ScrollPanel?.Children?.Clear();
        }

        //20250205 KH 스크롤 패널을 생성할 때, 제일 처음 이미지를 추가하여 N + 1 형태로 만들고 제일 처음으로 회귀하는 방식으로 수정

        public override void AddValue(byte[] pValue)
        {
            base.AddValue(pValue);

            if (this.BitmapImageList.Count == 0)
            {
                return; // 비어 있을 경우 조기 종료
            }

            // 마지막 이미지 추가
            // 리스트의 마지막 요소
            var lastImageSource = this.BitmapImageList[this.BitmapImageList.Count - 1];
            AddImageUnit(lastImageSource);

            if(this.BitmapImageList.Count == CarouselImageAtom.AlRowCount)
            {
                // 첫 번째 이미지 추가 (한 번 더)
                var firstImageSource = this.BitmapImageList[0]; // 리스트의 첫 번째 요소
                AddImageUnit(firstImageSource);
            }
        }

        private void AddImageUnit(ImageSource imageSource)
        {
            ImageUnit imageUnit = new ImageUnit
            {
                ImageSource = imageSource
            };

            double containerWidth = this.CarouselContainer.ActualWidth;
            if (Kiss.DoubleEqual(0, containerWidth) || double.IsNaN(containerWidth))
            {
                var ofAtom = WPFFindChildHelper.FindVisualParent<AtomBase>(m_SlidingAnimationControl);
                if (ofAtom != null)
                {
                    imageUnit.Width = ofAtom.AtomCore.Attrib.AtomWidth;
                }
                else
                {
                    imageUnit.Width = this.CarouselContainer.ActualWidth;
                }
            }
            else
            {
                imageUnit.Width = containerWidth;
            }

            imageUnit.Image.Stretch = m_ImageStretch;
            m_SlidingAnimationControl.ScrollPanel.Children.Add(imageUnit);
        }


        /*
        public override void AddValue(byte[] pValue)
        {
            base.AddValue(pValue);

            ImageUnit imageUnit = new ImageUnit();

            imageUnit.ImageSource = this.BitmapImageList[this.BitmapImageList.Count - 1];

            if (Kiss.DoubleEqual(0, this.CarouselContainer.ActualWidth) || true == double.IsNaN(this.CarouselContainer.ActualWidth))
            {
                var ofAtom = WPFFindChildHelper.FindVisualParent<AtomBase>(m_SlidingAnimationControl);
                imageUnit.Width = ofAtom.AtomCore.Attrib.AtomWidth;
            }
            else
            {
                imageUnit.Width = this.CarouselContainer.ActualWidth;
            }

            imageUnit.Image.Stretch = m_ImageStretch;

            m_SlidingAnimationControl.ScrollPanel.Children.Add(imageUnit);
        }
        */


        public override void AddValue(string strFilePath)
        {
            base.AddValue(strFilePath);

            ImageUnit imageUnit = new ImageUnit();

            imageUnit.ImageSource = this.BitmapImageList[this.BitmapImageList.Count - 1];
            imageUnit.Width = this.CarouselContainer.ActualWidth;
            imageUnit.Image.Stretch = m_ImageStretch;

            m_SlidingAnimationControl.ScrollPanel.Children.Add(imageUnit);
        }

        public override void SetImageStretch(Stretch imageStretch)
        {
            if (null != m_SlidingAnimationControl)
            {
                m_ImageStretch = imageStretch;
                m_SlidingAnimationControl.SetImageStretch(imageStretch);
            }
        }
    }
}
