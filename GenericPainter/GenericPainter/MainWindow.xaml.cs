using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static GenericPainter.MainWindow;
using Path = System.IO.Path;

namespace GenericPainter
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool mClicked = false;
        private bool lineClicked = false;
        private bool freeLineClicked = false;
        private bool rectangleClicked = false;
        private bool circleClicked = false;
        private bool eraserClicked = false;

        // Line
        private Line currentLine;
        private List<Line> freehandLines = new List<Line>();         //자유곡선 Data
        private List<LineData> lineDataList = new List<LineData>();     //직선 Data
        private Brush mycolor;
        private Brush fillColor;
        private double StrokeThicknessValue;

        private UIElement selectedShape = null;     // 이전 선택도형 확인

        // 동그라미
        private Ellipse currentEllipse;
        private Ellipse selectedEllipse;
        private Point circleStartPoint; // 원 그리기 시작 지점


        // 사각형
        private Rectangle currentRectangle;
        private Rectangle selectedRectangle;

        private Point prePosition;
        private Point nowPosition;

        private bool isDragging = false;        // 개체 이동 상태
        private Point previousPosition;         // 개체 이동을 위한 이전 마우스 위치


        // Resize Handler etc
        private bool isResizing = false;        // 크기 조정 상태
        private Point resizeStartPoint;         // 크기 조정 시작 지점
        private Rectangle resizeHandleTopLeft;          //resizeHandle
        private Rectangle resizeHandleTopRight;
        private Rectangle resizeHandleBottomLeft;
        private Rectangle resizeHandleBottomRight;
        private List<Rectangle> resizeHandles = new List<Rectangle>();

        // 지우개
        private bool isErasing = false;
        private Point lastPoint;


        // Json 저장을 위한 Data Class(좌표 Data)
        public class LineData
        {
            public double X1 { get; set; }
            public double Y1 { get; set; }
            public double X2 { get; set; }
            public double Y2 { get; set; }
            public string Color { get; set; }
            public double StrokeThickness { get; set; }

        }

        // Json 저장을 위한 Data Class(Line Data)
        public class DrawingData
        {
            public List<LineData> Lines { get; set; } = new List<LineData>();
        }

        public class ShapeData
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public string FillColor { get; set; }
            public double StrokeThickness { get; set; }
        }

        public class RectangleData : ShapeData
        {
            // 사각형 특화데이터 추가 가능
        }

        public class EllipseData : ShapeData
        {
            // 원 특화데이터 추가 가능
        }


        // 빈 캔버스를 누르면 반응 (resizeHamdler 삭제를 위한 것인데 수정이 필요함)
        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            // RemoveResizeHandles();

            if (isDragging)
            {
                isDragging = false; // 드래깅 상태 중지
            }
        }
        // Function for Mouse

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mClicked = true;
            prePosition = e.GetPosition(canvas);
            nowPosition = prePosition;
            mycolor = Brushes.Black;
            fillColor = Brushes.White;
            StrokeThicknessValue = 2;

            var clickedObject = e.OriginalSource as UIElement;

            if (lineClicked)
            {
                currentLine = new Line();
                currentLine.X1 = prePosition.X;
                currentLine.Y1 = prePosition.Y;
                currentLine.Stroke = mycolor;
                currentLine.StrokeThickness = StrokeThicknessValue;

                canvas.Children.Add(currentLine);

                lineDataList.Add(new LineData
                {
                    X1 = currentLine.X1,
                    Y1 = currentLine.Y1,
                    X2 = currentLine.X2,
                    Y2 = currentLine.Y2,
                    Color = ((SolidColorBrush)currentLine.Stroke).Color.ToString(),
                    StrokeThickness = currentLine.StrokeThickness
                });
            }

            if (freeLineClicked)
            {
                Line line = new Line
                {
                    X1 = prePosition.X,
                    Y1 = prePosition.Y,
                    X2 = prePosition.X,
                    Y2 = prePosition.Y,
                    Stroke = mycolor,
                    StrokeThickness = StrokeThicknessValue
                };

                freehandLines.Add(line);
                canvas.Children.Add(line);
            }

            if (circleClicked)
            {
                currentEllipse = new Ellipse
                {
                    Stroke = mycolor,       // 원의 테두리 색
                    Fill = fillColor,       // 원의 채우기 색
                    StrokeThickness = StrokeThicknessValue
                };

                // 원 그리기를 시작하는 지점
                circleStartPoint = prePosition;

                // Canvas에 추가
                canvas.Children.Add(currentEllipse);
            }

            if (rectangleClicked)
            {
                currentRectangle = new Rectangle
                {
                    Stroke = mycolor,
                    Fill = fillColor,
                    StrokeThickness = StrokeThicknessValue
                };

                // Canvas에 추가
                canvas.Children.Add(currentRectangle);
            }


            if (clickedObject is Ellipse clickedEllipse)
            {
                if (selectedShape != null)
                {
                    if (selectedShape is Ellipse prevEllipse)
                    {
                        prevEllipse.Stroke = Brushes.Black;
                        prevEllipse.StrokeThickness = StrokeThicknessValue;
                    }
                    else if (selectedShape is Rectangle prevRectangle)
                    {
                        prevRectangle.Stroke = Brushes.Black;
                        prevRectangle.StrokeThickness = StrokeThicknessValue;
                    }
                }

                selectedShape = clickedEllipse;
                selectedEllipse = clickedEllipse;

                selectedEllipse.Stroke = Brushes.Red;
                selectedEllipse.StrokeThickness = 5;

                isDragging = true;
                previousPosition = e.GetPosition(canvas);

                CreateResizeHandles(selectedEllipse);
                circleClicked = false;
            }
            else
            {
                selectedEllipse = null;
            }


            if (clickedObject is Rectangle clickedRectangle)
            {
                if (selectedShape != null)
                {
                    if (selectedShape is Ellipse prevEllipse)
                    {
                        prevEllipse.Stroke = Brushes.Black;
                        prevEllipse.StrokeThickness = StrokeThicknessValue;
                    }
                    else if (selectedShape is Rectangle prevRectangle)
                    {
                        prevRectangle.Stroke = Brushes.Black;
                        prevRectangle.StrokeThickness = StrokeThicknessValue;
                    }
                }

                selectedShape = clickedRectangle;

                // 클릭된 사각형을 선택
                selectedRectangle = clickedRectangle;

                // 선택된 사각형에 빨간색 테두리 / 두께5 적용
                selectedRectangle.Stroke = Brushes.Red;
                selectedRectangle.StrokeThickness = 5;

                isDragging = true;                          // 이동 시작
                previousPosition = e.GetPosition(canvas);

                CreateResizeHandles(selectedRectangle);
                rectangleClicked = false;                   // 사각형 그리기는 중단

            }
            else
            {
                selectedRectangle = null;  // 사각형이 아니면 선택된 객체를 null로 설정
            }



            if (eraserClicked && isErasing)
            {
                // 지우개 모드일 때
                if (eraserClicked && isErasing)
                {
                    var position = e.GetPosition(canvas);
                    var hitObject = VisualTreeHelper.HitTest(canvas, position);

                    if (hitObject != null)
                    {
                        // Line 객체를 클릭한 경우
                        if (hitObject.VisualHit is Line line)
                        {
                            canvas.Children.Remove(line);
                            freehandLines.Remove(line);  // freehandLines에서 해당 line 제거
                        }

                        // Rectangle 객체를 클릭한 경우
                        else if (hitObject.VisualHit is Rectangle rectangle)
                        {
                            canvas.Children.Remove(rectangle);

                        }
                    }
                }
                else
                {
                    mClicked = true;
                    prePosition = e.GetPosition(canvas);
                    nowPosition = prePosition;
                    mycolor = Brushes.Black;
                    fillColor = Brushes.White;
                }
            }

        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mClicked = false;
            isDragging = false;         // 이동 종료

            /*
            currentRectangle = null;    // 사각형 그리기 종료
            currentEllipse = null;
            */

            if (isErasing)
            {
                return;                         // 지우개 모드일 때는 그림을 그리지 않음
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (mClicked && lineClicked && currentLine != null)
            {
                var nowPosition = e.GetPosition(canvas);

                currentLine.X2 = nowPosition.X;
                currentLine.Y2 = nowPosition.Y;

                prePosition = nowPosition;

                LineData currentLineData = lineDataList.Last();
                currentLineData.X2 = nowPosition.X;
                currentLineData.Y2 = nowPosition.Y;
            }

            if (mClicked && freeLineClicked)
            {
                var nowPosition = e.GetPosition(canvas);

                if (freehandLines.Count > 0)
                {

                    var lastLine = freehandLines.Last();
                    lastLine.X2 = nowPosition.X;
                    lastLine.Y2 = nowPosition.Y;


                    Line newLine = new Line
                    {
                        X1 = lastLine.X2,
                        Y1 = lastLine.Y2,
                        X2 = nowPosition.X,
                        Y2 = nowPosition.Y,
                        Stroke = mycolor,
                        StrokeThickness = StrokeThicknessValue
                    };

                    freehandLines.Add(newLine);
                    canvas.Children.Add(newLine);
                }
            }

            if (mClicked && circleClicked && currentEllipse != null)
            {
                nowPosition = e.GetPosition(canvas);

                // 원의 크기 계산 (마우스 이동에 따라)
                double width = nowPosition.X - circleStartPoint.X;
                double height = nowPosition.Y - circleStartPoint.Y;

                /*
                // 원그리기(타원이 아닌 정원)
                if (Math.Abs(width) > Math.Abs(height))
                {
                    height = width;
                }
                else
                {
                    width = height;
                }
                */

                // 원의 위치 설정
                Canvas.SetLeft(currentEllipse, circleStartPoint.X);
                Canvas.SetTop(currentEllipse, circleStartPoint.Y);

                currentEllipse.Width = Math.Abs(width);
                currentEllipse.Height = Math.Abs(height);
            }

            if (isDragging && selectedEllipse != null)
            {
                // 현재 마우스 위치
                Point currentPosition = e.GetPosition(canvas);

                // 이동할 거리 계산
                double offsetX = currentPosition.X - previousPosition.X;
                double offsetY = currentPosition.Y - previousPosition.Y;

                // 선택된 원의 위치 업데이트
                Canvas.SetLeft(selectedEllipse, Canvas.GetLeft(selectedEllipse) + offsetX);
                Canvas.SetTop(selectedEllipse, Canvas.GetTop(selectedEllipse) + offsetY);

                // 이전 마우스 위치 갱신
                previousPosition = currentPosition;
            }


            if (mClicked && rectangleClicked && currentRectangle != null)
            {
                // 마우스 움직임에 따라 사각형의 크기와 위치 계산
                nowPosition = e.GetPosition(canvas);

                double left = prePosition.X;
                double top = prePosition.Y;
                double width = nowPosition.X - prePosition.X;
                double height = nowPosition.Y - prePosition.Y;

                // 마우스를 왼쪽/위쪽으로 드래그한 경우 크기와 위치 반전
                if (nowPosition.X < prePosition.X)
                {
                    left = nowPosition.X;
                    width *= -1;
                }
                if (nowPosition.Y < prePosition.Y)
                {
                    top = nowPosition.Y;
                    height *= -1;
                }

                // 위치와 크기 설정
                Canvas.SetLeft(currentRectangle, left);
                Canvas.SetTop(currentRectangle, top);
                currentRectangle.Width = width;
                currentRectangle.Height = height;

                currentRectangle.Fill = fillColor;
            }

            if (isDragging && selectedRectangle != null)
            {
                // 현재 마우스 위치
                Point currentPosition = e.GetPosition(canvas);

                // 이동할 거리 계산
                double offsetX = currentPosition.X - previousPosition.X;
                double offsetY = currentPosition.Y - previousPosition.Y;

                // 선택된 사각형의 위치 업데이트
                Canvas.SetLeft(selectedRectangle, Canvas.GetLeft(selectedRectangle) + offsetX);
                Canvas.SetTop(selectedRectangle, Canvas.GetTop(selectedRectangle) + offsetY);

                // 이전 마우스 위치 갱신
                previousPosition = currentPosition;

                RemoveResizeHandles();
            }

            if (mClicked && eraserClicked && isErasing)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var currentPoint = e.GetPosition(canvas);
                    var hitObject = VisualTreeHelper.HitTest(canvas, currentPoint);

                    if (hitObject != null)
                    {
                        // Line 객체를 클릭한 경우
                        if (hitObject.VisualHit is Line line)
                        {
                            // RemovePartOfLine(line, currentPoint);    개체를 지우는 것이 아닌 닿은 부분만 삭제(수정필요)

                            canvas.Children.Remove(line);
                            freehandLines.Remove(line);  // 자유곡선 리스트에서 해당 선 제거

                        }
                        // Rectangle 객체를 클릭한 경우
                        else if (hitObject.VisualHit is Rectangle rectangle)
                        {
                            canvas.Children.Remove(rectangle);
                        }
                    }

                    lastPoint = currentPoint;
                }
            }


        }


        private void canvas_MouseRightButtonDown(object sender, RoutedEventArgs e)
        {

        }

        private void canvas_MouseRightButtonUp(object sender, RoutedEventArgs e)
        {

        }


        // Function for Buttons

        private void button_line_click(object sender, RoutedEventArgs e)
        {
            RemoveResizeHandles();

            lineClicked = true;
            freeLineClicked = false;
            rectangleClicked = false;
            circleClicked = false;
            eraserClicked = false;
        }

        private void button_freeLine_click(object sender, RoutedEventArgs e)
        {
            RemoveResizeHandles();

            freeLineClicked = true;
            lineClicked = false;
            rectangleClicked = false;
            circleClicked = false;
            eraserClicked = false;
        }


        private void button_circle_click(object sender, RoutedEventArgs e)
        {
            RemoveResizeHandles();

            freeLineClicked = false;
            lineClicked = false;
            rectangleClicked = false;
            circleClicked = true;
            eraserClicked = false;
        }


        private void button_rectangle_click(object sender, RoutedEventArgs e)
        {
            RemoveResizeHandles();

            freeLineClicked = false;
            lineClicked = false;
            rectangleClicked = true;
            circleClicked = false;
            eraserClicked = false;
        }

        private void button_eraser_click(object sender, RoutedEventArgs e)
        {
            RemoveResizeHandles();

            freeLineClicked = false;
            lineClicked = false;
            rectangleClicked = false;
            circleClicked = false;
            eraserClicked = true;

            isErasing = !isErasing;
            if (isErasing)
            {
                // 지우개 모드 활성화
                canvas.Cursor = Cursors.Hand;
            }
            else
            {
                // 지우개 모드 비활성화
                canvas.Cursor = Cursors.Arrow;
            }
        }


        // 사각형 관련 함수
        private void UpdateRectangleColor()
        {
            /*
            // 이미 그려진 사각형들에 대해 색상 업데이트
            foreach (var child in canvas.Children)
            {
                if (child is Rectangle rectangle)
                {
                    rectangle.Fill = fillColor;
                }
            }
            */

            // 선택된 사각형이 있을 경우 색상 업데이트
            if (selectedRectangle != null)
            {
                // 선택된 사각형에 색상 적용
                selectedRectangle.Fill = fillColor;
            }
        }

        private void UpdateCircleColor()
        {
            /*
            // 이미 그려진 원들에 대해 색상 업데이트
            foreach (var child in canvas.Children)
            {
                if (child is Ellipse ellipss)
                {
                    ellipss.Fill = fillColor;
                }
            }
            */

            // 선택된 원이 있을 경우 색상 업데이트
            if (selectedEllipse != null)
            {
                // 선택된 원에 색상 적용
                selectedEllipse.Fill = fillColor;
            }
        }

        /*
        private void comboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            double comboboxValue;

            if (comboBox != null && comboBox.SelectedItem != null)
            {

                ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;

                if(selectedItem != null && selectedItem.Content != null)
                {
                    string selectedValue = selectedItem.Content.ToString();
                    if (double.TryParse(selectedValue, out comboboxValue))
                    {
                        // 변환 성공, comboboxValue에 값이 저장됨
                        MessageBox.Show($"선택된 값: {comboboxValue}");

                        if (selectedRectangle != null)
                        {
                            // 선택된 사각형에 색상 적용
                            selectedRectangle.StrokeThickness = comboboxValue;
                        }
                        else if (selectedEllipse != null)
                        {
                            selectedEllipse.StrokeThickness = comboboxValue;
                        }
                        else if (currentLine != null)
                        {
                            currentLine.StrokeThickness = comboboxValue;
                        }
                    }
                    else
                    {
                        MessageBox.Show("선택된 항목이 숫자가 아닙니다.");
                    }
                }
                else
                {
                    MessageBox.Show("선택된 항목이 ComboBox Item이 아닙니다.");
                }

            }
            else
            {
                MessageBox.Show("선택된 항목이 없습니다.");
            }
        }
        */

        // 색상 버튼 클릭 시
        private void color_black_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.Black;
            fillColor = Brushes.Black;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }

        }

        private void color_white_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.Black;
            fillColor = Brushes.White;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }

        private void color_red_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.Red;
            fillColor = Brushes.Red;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }

        private void color_orange_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.Orange;
            fillColor = Brushes.Orange;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }

        private void color_yellow_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.Yellow;
            fillColor = Brushes.Yellow;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }

        private void color_green_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.Green;
            fillColor = Brushes.Green;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }

        private void color_blue_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.Blue;
            fillColor = Brushes.Blue;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }

        private void color_navy_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.Navy;
            fillColor = Brushes.Navy;

            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }

        private void color_purple_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.Purple;
            fillColor = Brushes.Purple;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }

        private void color_lightGray_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.LightGray;
            fillColor = Brushes.LightGray;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }

        private void color_gray_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.Gray;
            fillColor = Brushes.Gray;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }

        private void color_darkGray_click(object sender, RoutedEventArgs e)
        {
            mycolor = Brushes.DarkGray;
            fillColor = Brushes.DarkGray;
            if (selectedShape != null)
            {
                if (selectedShape is Ellipse prevEllipse)
                {
                    UpdateCircleColor();
                }
                else if (selectedShape is Rectangle prevRectangle)
                {
                    UpdateRectangleColor();
                }
            }
        }
        /*
        private void button_jsonSave_click(object sender, RoutedEventArgs e)
        {
            //fileName은 test로 지정
            var fileName = "line.json";
            var fileName2 = "shape.json";

            // filePath를 다운로드 폴더로 지정
            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string filePath = Path.Combine(downloadsFolder, fileName);
            string filePath2 = Path.Combine(downloadsFolder, fileName2);

            try
            {
                if (!Directory.Exists(downloadsFolder))
                {
                    Directory.CreateDirectory(downloadsFolder);
                }

                var drawingData = new DrawingData();

                // 직선 데이터 추가
                if (lineDataList != null)
                {
                    drawingData.Lines.AddRange(lineDataList);
                }

                // 자유곡선 데이터 추가
                foreach (var line in freehandLines)
                {
                    // 각 선의 좌표와 색상 저장
                    var lineData = new LineData
                    {
                        X1 = line.X1,
                        Y1 = line.Y1,
                        X2 = line.X2,
                        Y2 = line.Y2,
                        Color = ((SolidColorBrush)line.Stroke).Color.ToString(),
                        StrokeThickness = line.StrokeThickness
                    };

                    drawingData.Lines.Add(lineData);
                }

                List<ShapeData> shapes = new List<ShapeData>();
                if (currentRectangle != null)
                {
                    shapes.Add(new RectangleData
                    {
                        X = Canvas.GetLeft(currentRectangle),
                        Y = Canvas.GetTop(currentRectangle),
                        Width = currentRectangle.Width,
                        Height = currentRectangle.Height,
                        FillColor = (currentRectangle.Fill as SolidColorBrush)?.Color.ToString() ?? "#010101",
                        StrokeThickness = currentRectangle.StrokeThickness
                    });

                    //MessageBox.Show("사각형이 있습니다");
                }
                else
                {
                    //MessageBox.Show("사각형이 없습니다");
                }

                if (selectedRectangle != null)
                {
                    shapes.Add(new RectangleData
                    {
                        X = Canvas.GetLeft(selectedRectangle),
                        Y = Canvas.GetTop(selectedRectangle),
                        Width = selectedRectangle.Width,
                        Height = selectedRectangle.Height,
                        FillColor = (selectedRectangle.Fill as SolidColorBrush)?.Color.ToString() ?? "#010101",
                        StrokeThickness = selectedRectangle.StrokeThickness
                    });
                }
                else
                {
                }

                if (currentEllipse != null)
                {
                    shapes.Add(new EllipseData
                    {
                        X = Canvas.GetLeft(currentEllipse),
                        Y = Canvas.GetTop(currentEllipse),
                        Width = currentEllipse.Width,
                        Height = currentEllipse.Height,
                        FillColor = (currentEllipse.Fill as SolidColorBrush)?.Color.ToString() ?? "#010101",
                        StrokeThickness = currentEllipse.StrokeThickness
                    });
                }
                else
                {
                }

                if (selectedEllipse != null)
                {
                    shapes.Add(new EllipseData
                    {
                        X = Canvas.GetLeft(selectedEllipse),
                        Y = Canvas.GetTop(selectedEllipse),
                        Width = selectedEllipse.Width,
                        Height = selectedEllipse.Height,
                        FillColor = (selectedEllipse.Fill as SolidColorBrush)?.Color.ToString() ?? "#010101",
                        StrokeThickness = selectedEllipse.StrokeThickness
                    });
                }
                else
                {
                }


                string json = JsonConvert.SerializeObject(drawingData, Formatting.Indented);

                string shapeJson = JsonConvert.SerializeObject(shapes, Formatting.Indented);

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {

                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(json);
                    }
                }
                MessageBox.Show($"파일이 저장되었습니다: {filePath}");

                using (FileStream fs = new FileStream(filePath2, FileMode.Create, FileAccess.Write))
                {

                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(shapeJson);
                    }
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                // 파일 접근 권한 오류
                MessageBox.Show($"접근 권한 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DirectoryNotFoundException ex)
            {
                // 폴더 찾을 수 없음 오류
                MessageBox.Show($"폴더를 찾을 수 없습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                // IO 오류
                MessageBox.Show($"파일 입출력 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (JsonSerializationException ex)
            {
                // JSON 직렬화 오류
                MessageBox.Show($"JSON 직렬화 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // 기타 모든 오류
                MessageBox.Show($"알 수 없는 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } 
        */

        private void button_jsonSave_click(object sender, RoutedEventArgs e)
        {
            var lineFileName = "line.json";
            var rectFileName = "rect.json";
            var elliFileName = "elli.json";

            // filePath를 다운로드 폴더로 지정
            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string lineFilePath = Path.Combine(downloadsFolder, lineFileName);
            string rectFilePath = Path.Combine(downloadsFolder, rectFileName);
            string elliFilePath = Path.Combine(downloadsFolder, elliFileName);

            try
            {
                if (!Directory.Exists(downloadsFolder))
                {
                    Directory.CreateDirectory(downloadsFolder);
                }

                var drawingData = new DrawingData();

                // 직선 데이터 추가
                if (lineDataList != null)
                {
                    drawingData.Lines.AddRange(lineDataList);
                }

                // 자유곡선 데이터 추가
                foreach (var line in freehandLines)
                {
                    // 각 선의 좌표와 색상 저장
                    var lineData = new LineData
                    {
                        X1 = line.X1,
                        Y1 = line.Y1,
                        X2 = line.X2,
                        Y2 = line.Y2,
                        Color = ((SolidColorBrush)line.Stroke).Color.ToString(),
                        StrokeThickness = line.StrokeThickness
                    };

                    drawingData.Lines.Add(lineData);
                }

                List<RectangleData> rect = new List<RectangleData>();
                if (currentRectangle != null)
                {
                    rect.Add(new RectangleData
                    {
                        X = Canvas.GetLeft(currentRectangle),
                        Y = Canvas.GetTop(currentRectangle),
                        Width = currentRectangle.Width,
                        Height = currentRectangle.Height,
                        FillColor = (currentRectangle.Fill as SolidColorBrush)?.Color.ToString() ?? "#010101",
                        StrokeThickness = currentRectangle.StrokeThickness
                    });

                    //MessageBox.Show("사각형이 있습니다");
                }
                else
                {
                    //MessageBox.Show("사각형이 없습니다");
                }


                if (selectedRectangle != null)
                {
                    rect.Add(new RectangleData
                    {
                        X = Canvas.GetLeft(selectedRectangle),
                        Y = Canvas.GetTop(selectedRectangle),
                        Width = selectedRectangle.Width,
                        Height = selectedRectangle.Height,
                        FillColor = (selectedRectangle.Fill as SolidColorBrush)?.Color.ToString() ?? "#010101",
                        StrokeThickness = selectedRectangle.StrokeThickness
                    });
                }
                else
                {
                }


                List<EllipseData> elli = new List<EllipseData>();

                if (currentEllipse != null)
                {
                    elli.Add(new EllipseData
                    {
                        X = Canvas.GetLeft(currentEllipse),
                        Y = Canvas.GetTop(currentEllipse),
                        Width = currentEllipse.Width,
                        Height = currentEllipse.Height,
                        FillColor = (currentEllipse.Fill as SolidColorBrush)?.Color.ToString() ?? "#010101",
                        StrokeThickness = currentEllipse.StrokeThickness
                    });
                }
                else
                {
                }

                if (selectedEllipse != null)
                {
                    elli.Add(new EllipseData
                    {
                        X = Canvas.GetLeft(selectedEllipse),
                        Y = Canvas.GetTop(selectedEllipse),
                        Width = selectedEllipse.Width,
                        Height = selectedEllipse.Height,
                        FillColor = (selectedEllipse.Fill as SolidColorBrush)?.Color.ToString() ?? "#010101",
                        StrokeThickness = selectedEllipse.StrokeThickness
                    });
                }
                else
                {
                }


                string lineJson = JsonConvert.SerializeObject(drawingData, Formatting.Indented);
                string rectJson = JsonConvert.SerializeObject(rect, Formatting.Indented);
                string elliJson = JsonConvert.SerializeObject(elli, Formatting.Indented);

                using (FileStream fs = new FileStream(lineFilePath, FileMode.Create, FileAccess.Write))
                {

                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(lineJson);
                    }
                }
                MessageBox.Show($"파일이 저장되었습니다: {lineFilePath}");

                using (FileStream fs = new FileStream(rectFilePath, FileMode.Create, FileAccess.Write))
                {

                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(rectJson);
                    }
                }

                using (FileStream fs = new FileStream(elliFilePath, FileMode.Create, FileAccess.Write))
                {

                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(elliJson);
                    }
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                // 파일 접근 권한 오류
                MessageBox.Show($"접근 권한 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DirectoryNotFoundException ex)
            {
                // 폴더 찾을 수 없음 오류
                MessageBox.Show($"폴더를 찾을 수 없습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                // IO 오류
                MessageBox.Show($"파일 입출력 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (JsonSerializationException ex)
            {
                // JSON 직렬화 오류
                MessageBox.Show($"JSON 직렬화 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // 기타 모든 오류
                MessageBox.Show($"알 수 없는 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void button_jsonLoad_click(object sender, RoutedEventArgs e)
        {
            var lineFileName = "line.json";
            var rectFileName = "rect.json";
            var elliFileName = "elli.json";

            // filePath를 다운로드 폴더로 지정
            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string lineFilePath = Path.Combine(downloadsFolder, lineFileName);
            string rectFilePath = Path.Combine(downloadsFolder, rectFileName);
            string elliFilePath = Path.Combine(downloadsFolder, elliFileName);

            // filePath를 다운로드 폴더로 지정
            /*
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)는 현재 사용자의 프로필 폴더 경로를 반환
            string downloadsFolder = Path.Combine(@"C:\Git\CSharp\KH_project", "KH_JsonData");
            이스케이프 문자 무시: 문자열 내에서 백슬래시(\)를 특수한 문자로 해석하지 않게 만들어줌
            예를 들어, "C:\\Git\\CSharp\\KH_project"와 같이 이스케이프 문자를 사용해야 하는 경우
            @"C:\Git\CSharp\KH_project"와 같이 문자열을 바로 쓸 수 있음
            */

            try
            {
                // 기존 캔버스 클리어
                canvas.Children.Clear();

                // 파일이 존재하는지 확인
                if (File.Exists(lineFilePath))
                {
                    // 파일 핸들러를 사용하여 JSON 데이터를 읽기
                    using (FileStream fs = new FileStream(lineFilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader reader = new StreamReader(fs))
                        {
                            // JSON 문자열 읽기
                            string json = reader.ReadToEnd();

                            // JSON을 DrawingData 객체로 역직렬화
                            var drawingData = JsonConvert.DeserializeObject<DrawingData>(json);

                            // 각 선을 캔버스에 다시 그리기
                            foreach (var lineData in drawingData.Lines)
                            {
                                // Line 객체 생성
                                var line = new Line
                                {
                                    X1 = lineData.X1,
                                    Y1 = lineData.Y1,
                                    X2 = lineData.X2,
                                    Y2 = lineData.Y2,
                                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lineData.Color)),
                                    StrokeThickness = lineData.StrokeThickness
                                };

                                // 캔버스에 선 추가
                                canvas.Children.Add(line);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("파일이 존재하지 않습니다.");
                }

                if (File.Exists(rectFilePath))
                {

                    string json = File.ReadAllText(rectFilePath);
                    List<RectangleData> shapes = JsonConvert.DeserializeObject<List<RectangleData>>(json);

                    foreach (RectangleData rectangleData in shapes)
                    {
                        var rectangle = new Rectangle
                        {
                            Width = rectangleData.Width,
                            Height = rectangleData.Height,
                            Stroke = Brushes.Black,
                            StrokeThickness = 2
                        };

                        Canvas.SetLeft(rectangle, rectangleData.X);
                        Canvas.SetTop(rectangle, rectangleData.Y);

                        MessageBox.Show($"{rectangle}");

                        canvas.Children.Add(rectangle);
                    }

                }
                else
                {
                    MessageBox.Show("파일이 존재하지 않습니다.");
                }

                if (File.Exists(elliFilePath))
                {

                    string json = File.ReadAllText(elliFilePath);
                    List<EllipseData> shapes = JsonConvert.DeserializeObject<List<EllipseData>>(json);

                    foreach (EllipseData ellipseData in shapes)
                    {
                        var ellipse = new Ellipse
                        {
                            Width = ellipseData.Width,
                            Height = ellipseData.Height,
                            Stroke = Brushes.Black,
                            StrokeThickness = 2
                        };

                        Canvas.SetLeft(ellipse, ellipseData.X);
                        Canvas.SetTop(ellipse, ellipseData.Y);

                        MessageBox.Show($"{ellipse}");

                        canvas.Children.Add(ellipse);
                    }

                }
                else
                {
                    MessageBox.Show("파일이 존재하지 않습니다.");
                }
            }
            catch (FileNotFoundException ex)
            {
                // 파일을 찾을 수 없는 경우
                MessageBox.Show($"파일을 찾을 수 없습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                // 파일 접근 권한 오류
                MessageBox.Show($"파일 접근 권한 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                // 파일 읽기 관련 입출력 오류
                MessageBox.Show($"파일 읽기 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (JsonSerializationException ex)
            {
                // JSON 역직렬화 오류
                MessageBox.Show($"JSON 역직렬화 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // 기타 예외 처리
                MessageBox.Show($"알 수 없는 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button_free_click(object sender, RoutedEventArgs e)
        {
            RemoveResizeHandles();

            if (selectedShape is Ellipse prevEllipse)
            {
                prevEllipse.Stroke = Brushes.Black;
                prevEllipse.StrokeThickness = 1;
            }
            else if (selectedShape is Rectangle prevRectangle)
            {
                prevRectangle.Stroke = Brushes.Black;
                prevRectangle.StrokeThickness = 1;
            }

            selectedShape = null;     // 이전 선택도형 확인
            selectedEllipse = null;
            selectedRectangle = null;
        }

        private void button_clear_click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
        }


        // 크기조절

        private void CreateResizeHandles(UIElement selectedElement)
        {
            if (selectedElement is Rectangle rectangle)
            {
                if (rectangle == null)
                {
                    throw new ArgumentNullException(nameof(rectangle), "사각형이 선택되지 않았습니다.");
                }
                // 기존 핸들 제거
                RemoveResizeHandles();

                // 사각형의 4모서리에 핸들을 생성합니다.
                resizeHandleTopLeft = CreateResizeHandle(ResizeDirection.TopLeft);
                resizeHandleTopRight = CreateResizeHandle(ResizeDirection.TopRight);
                resizeHandleBottomLeft = CreateResizeHandle(ResizeDirection.BottomLeft);
                resizeHandleBottomRight = CreateResizeHandle(ResizeDirection.BottomRight);

                // 핸들을 캔버스에 추가
                canvas.Children.Add(resizeHandleTopLeft);
                canvas.Children.Add(resizeHandleTopRight);
                canvas.Children.Add(resizeHandleBottomLeft);
                canvas.Children.Add(resizeHandleBottomRight);

                resizeHandles.Add(resizeHandleTopLeft);
                resizeHandles.Add(resizeHandleTopRight);
                resizeHandles.Add(resizeHandleBottomLeft);
                resizeHandles.Add(resizeHandleBottomRight);

                // 핸들의 위치를 사각형에 맞춰 설정
                PositionResizeHandles(rectangle);

            }
            else if (selectedElement is Ellipse ellipse)
            {
                if (ellipse == null)
                {
                    throw new ArgumentNullException(nameof(ellipse), "원이 선택되지 않았습니다.");
                }
                // 기존 핸들 제거
                RemoveResizeHandles();

                resizeHandleTopLeft = CreateResizeHandle(ResizeDirection.TopLeft);
                resizeHandleTopRight = CreateResizeHandle(ResizeDirection.TopRight);
                resizeHandleBottomLeft = CreateResizeHandle(ResizeDirection.BottomLeft);
                resizeHandleBottomRight = CreateResizeHandle(ResizeDirection.BottomRight);

                // 핸들을 캔버스에 추가
                canvas.Children.Add(resizeHandleTopLeft);
                canvas.Children.Add(resizeHandleTopRight);
                canvas.Children.Add(resizeHandleBottomLeft);
                canvas.Children.Add(resizeHandleBottomRight);

                resizeHandles.Add(resizeHandleTopLeft);
                resizeHandles.Add(resizeHandleTopRight);
                resizeHandles.Add(resizeHandleBottomLeft);
                resizeHandles.Add(resizeHandleBottomRight);

                PositionResizeHandles(ellipse);

            }

        }


        private enum ResizeDirection
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        private Rectangle CreateResizeHandle(ResizeDirection direction)
        {
            var handle = new Rectangle
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red,
                Cursor = Cursors.SizeAll
            };

            handle.MouseLeftButtonDown += (sender, e) =>
            {
                isResizing = true;
                resizeStartPoint = e.GetPosition(canvas);
                e.Handled = true; // 이벤트 버블링 방지
            };

            handle.MouseMove += (sender, e) =>
            {
                if (isResizing && selectedRectangle != null)
                {
                    var currentPoint = e.GetPosition(canvas);
                    ResizeRectangle(selectedRectangle, direction, currentPoint);
                    e.Handled = true;
                }
                else if (isResizing && selectedEllipse != null)
                {
                    var currentPoint = e.GetPosition(canvas);
                    ResizeEllipse(selectedEllipse, direction, currentPoint);
                    e.Handled = true;
                }
            };

            handle.MouseLeftButtonUp += (sender, e) =>
            {
                isResizing = false;
                e.Handled = true;
                RemoveResizeHandles();
            };

            handle.MouseLeave += resizeHandl_MouseLeave;

            return handle;
        }

        private void resizeHandl_MouseLeave(object sender, EventArgs e)
        {
            isResizing = false;
            RemoveResizeHandles();
        }

        private void RemoveResizeHandles()
        {
            foreach (var handle in resizeHandles)
            {
                canvas.Children.Remove(handle); // 캔버스에서 제거
            }

            resizeHandles.Clear(); // 리스트 초기화
        }

        private void PositionResizeHandles(UIElement selectedElement)
        {
            if (selectedElement is Rectangle)
            {
                Rectangle rectangle = (Rectangle)selectedElement;

                if (rectangle == null)
                {
                    throw new ArgumentNullException(nameof(rectangle), "사각형이 선택되지 않았습니다.");
                }

                // 각 핸들이 null인지 확인
                if (resizeHandleTopLeft == null || resizeHandleTopRight == null || resizeHandleBottomLeft == null || resizeHandleBottomRight == null)
                {
                    throw new InvalidOperationException("Resize 핸들이 초기화되지 않았습니다.");
                }

                // 사각형의 위치에 맞춰 핸들을 배치
                double left = Canvas.GetLeft(rectangle);
                double top = Canvas.GetTop(rectangle);
                double width = rectangle.Width;
                double height = rectangle.Height;

                Canvas.SetLeft(resizeHandleTopLeft, left - resizeHandleTopLeft.Width / 2);
                Canvas.SetTop(resizeHandleTopLeft, top - resizeHandleTopLeft.Height / 2);

                Canvas.SetLeft(resizeHandleTopRight, left + width - resizeHandleTopRight.Width / 2);
                Canvas.SetTop(resizeHandleTopRight, top - resizeHandleTopRight.Height / 2);

                Canvas.SetLeft(resizeHandleBottomLeft, left - resizeHandleBottomLeft.Width / 2);
                Canvas.SetTop(resizeHandleBottomLeft, top + height - resizeHandleBottomLeft.Height / 2);

                Canvas.SetLeft(resizeHandleBottomRight, left + width - resizeHandleBottomRight.Width / 2);
                Canvas.SetTop(resizeHandleBottomRight, top + height - resizeHandleBottomRight.Height / 2);

            }
            else if (selectedElement is Ellipse ellipse)
            {

                double left = Canvas.GetLeft(ellipse);
                double top = Canvas.GetTop(ellipse);
                double width = ellipse.Width;
                double height = ellipse.Height;

                /*
                Canvas.SetLeft(resizeHandleTopLeft, left + width / 2);
                Canvas.SetTop(resizeHandleTopLeft, top - resizeHandleTopLeft.Height / 2);

                Canvas.SetLeft(resizeHandleTopRight, left + width - resizeHandleTopLeft.Height / 2);
                Canvas.SetTop(resizeHandleTopRight, top - height / 2);

                Canvas.SetLeft(resizeHandleBottomLeft, left + width / 2);
                Canvas.SetTop(resizeHandleBottomLeft, Top + height - resizeHandleBottomLeft.Height / 2);

                Canvas.SetLeft(resizeHandleBottomRight, left);
                Canvas.SetTop(resizeHandleBottomRight, top - height / 2 - resizeHandleBottomRight.Height / 2);
                */


                Canvas.SetLeft(resizeHandleTopLeft, left - resizeHandleTopLeft.Width / 2);
                Canvas.SetTop(resizeHandleTopLeft, top - resizeHandleTopLeft.Height / 2);

                Canvas.SetLeft(resizeHandleTopRight, left + width - resizeHandleTopRight.Width / 2);
                Canvas.SetTop(resizeHandleTopRight, top - resizeHandleTopRight.Height / 2);

                Canvas.SetLeft(resizeHandleBottomLeft, left - resizeHandleBottomLeft.Width / 2);
                Canvas.SetTop(resizeHandleBottomLeft, top + height - resizeHandleBottomLeft.Height / 2);

                Canvas.SetLeft(resizeHandleBottomRight, left + width - resizeHandleBottomRight.Width / 2);
                Canvas.SetTop(resizeHandleBottomRight, top + height - resizeHandleBottomRight.Height / 2);

            }
        }


        private void ResizeRectangle(Rectangle rectangle, ResizeDirection direction, Point currentPoint)
        {
            if (rectangle == null) return;

            double deltaX = currentPoint.X - resizeStartPoint.X;
            double deltaY = currentPoint.Y - resizeStartPoint.Y;

            double newLeft = Canvas.GetLeft(rectangle);
            double newTop = Canvas.GetTop(rectangle);
            double newWidth = rectangle.Width;
            double newHeight = rectangle.Height;

            const double minSize = 10; // 최소 크기 설정

            switch (direction)
            {
                case ResizeDirection.TopLeft:
                    newLeft += deltaX;
                    newTop += deltaY;
                    newWidth -= deltaX;
                    newHeight -= deltaY;
                    break;

                case ResizeDirection.TopRight:
                    newTop += deltaY;
                    newWidth += deltaX;
                    newHeight -= deltaY;
                    break;

                case ResizeDirection.BottomLeft:
                    newLeft += deltaX;
                    newWidth -= deltaX;
                    newHeight += deltaY;
                    break;

                case ResizeDirection.BottomRight:
                    newWidth += deltaX;
                    newHeight += deltaY;
                    break;
            }

            // 최소 크기 확인
            if (newWidth < minSize) newWidth = minSize;
            if (newHeight < minSize) newHeight = minSize;

            // 사각형 위치와 크기 업데이트
            Canvas.SetLeft(rectangle, newLeft);
            Canvas.SetTop(rectangle, newTop);
            rectangle.Width = newWidth;
            rectangle.Height = newHeight;

            // 핸들 위치 업데이트
            PositionResizeHandles(rectangle);

            // 크기 조정 시작 지점 갱신
            resizeStartPoint = currentPoint;
        }

        private void ResizeEllipse(Ellipse ellipse, ResizeDirection direction, Point currentPoint)
        {
            if (ellipse == null) return;

            double deltaX = currentPoint.X - resizeStartPoint.X;
            double deltaY = currentPoint.Y - resizeStartPoint.Y;

            double newLeft = Canvas.GetLeft(ellipse);
            double newTop = Canvas.GetTop(ellipse);
            double newWidth = ellipse.Width;
            double newHeight = ellipse.Height;

            const double minSize = 10; // 최소 크기 설정

            switch (direction)
            {
                case ResizeDirection.TopLeft:
                    newLeft += deltaX;
                    newTop += deltaY;
                    newWidth -= deltaX;
                    newHeight -= deltaY;
                    break;

                case ResizeDirection.TopRight:
                    newTop += deltaY;
                    newWidth += deltaX;
                    newHeight -= deltaY;
                    break;

                case ResizeDirection.BottomLeft:
                    newLeft += deltaX;
                    newWidth -= deltaX;
                    newHeight += deltaY;
                    break;

                case ResizeDirection.BottomRight:
                    newWidth += deltaX;
                    newHeight += deltaY;
                    break;
            }

            // 최소 크기 확인
            if (newWidth < minSize) newWidth = minSize;
            if (newHeight < minSize) newHeight = minSize;

            // 사각형 위치와 크기 업데이트
            Canvas.SetLeft(ellipse, newLeft);
            Canvas.SetTop(ellipse, newTop);
            ellipse.Width = newWidth;
            ellipse.Height = newHeight;

            // 핸들 위치 업데이트
            PositionResizeHandles(ellipse);

            // 크기 조정 시작 지점 갱신
            resizeStartPoint = currentPoint;
        }


        /*
         * 선이 닿는 부분만 삭제하는 로직 : 큰 차이 없음
        
        private void RemovePartOfLine(Line line, Point eraserPosition)
        {
            // 지우개 크기 (반경)
            double eraserRadius = 10;

            // 선의 시작점과 끝점 구하기
            double x1 = line.X1, y1 = line.Y1;
            double x2 = line.X2, y2 = line.Y2;

            // 지우개와 선 사이의 최소 거리를 계산하여, 지우개 반경 내에 있는 부분만 지우기
            double distanceToLineStart = GetDistanceFromPointToLine(eraserPosition, x1, y1, x2, y2);
            double distanceToLineEnd = GetDistanceFromPointToLine(eraserPosition, x2, y2, x1, y1);

            // 두 점 사이의 거리 계산
            if (distanceToLineStart < eraserRadius || distanceToLineEnd < eraserRadius)
            {
                canvas.Children.Remove(line);  // 선 지우기
                freehandLines.Remove(line);    // 자유곡선 리스트에서 해당 선 제거
            }
        }

        private double GetDistanceFromPointToLine(Point point, double x1, double y1, double x2, double y2)
        {
            double A = point.X - x1;
            double B = point.Y - y1;
            double C = x2 - x1;
            double D = y2 - y1;

            double dot = A * C + B * D;
            double len_sq = C * C + D * D;
            double param = -1.0;

            // 점이 선 위에 있는지, 아니면 선의 연장선에 있는지 계산
            if (len_sq != 0)  // length squared = 0이면 선의 길이가 0, 즉 점이 아님
            {
                param = dot / len_sq;
            }

            double xx, yy;

            if (param < 0)
            {
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            // 거리 계산
            double dx = point.X - xx;
            double dy = point.Y - yy;

            return Math.Sqrt(dx * dx + dy * dy);
        }

         */

    }
}
