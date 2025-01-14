using SharpGL;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using SharpGL.SceneGraph.Lighting;



namespace UngDungVe2D
{

    public partial class Form1 : Form
    {
        bool flag = true;
        enum DrawMode
        {
            None,       
            Triangle,   //Tam giác
            Circle,     //Hình tròn
            Quadrilateral,  //Tứ giác lồi
            Line,       //Đường thẳng
            Linear,     //Phương trình tuyến tính
            Quadratic,  //Phương trình bậc hai
            Cubic,      //Phương trình bậc ba
            Beizer,     //Đường cong beizer
            Spline      //Đường cong spline
        }

        //Khởi tạo DrawMode
        private DrawMode drawMode = DrawMode.None;

        //LƯU TRỮ HÌNH VẼ
        private List<Action<OpenGL>> shapes = new List<Action<OpenGL>>();
        private List<PointF> point = new List<PointF> ();

        //TRỤC TỌA ĐỘ
        private Button drawAxes;
        private bool isDrawAxes = false;

        //HÌNH HỌC
        public abstract class Shape
        {
            public int ID { get; set; }
            public List<Tuple<float, float>> Vertices { get; set; } // Danh sách các đỉnh (x, y)
            public Color Color { get; set; } // Màu sắc của hình

            public Shape(int id, Color color)
            {
                ID = id;
                Color = color;
                Vertices = new List<Tuple<float, float>>();
            }

            public abstract void Draw(OpenGL gl); // Phương thức vẽ hình
        }
        public class ShapeManager
        {
            public List<Shape> Shapes { get; private set; }

            public ShapeManager()
            {
                Shapes = new List<Shape>();
            }

            public void AddShape(Shape shape)
            {
                Shapes.Add(shape); // Thêm hình vào danh sách
            }

            public void DrawAllShapes(OpenGL gl)
            {
                // Vẽ tất cả các hình đã lưu trong danh sách
                foreach (var shape in Shapes)
                {
                    shape.Draw(gl); // Vẽ từng hình
                }
            }
        }

        //VẼ TAM GIÁC
        private Button drawTriangleButton;
        public class Triangle : Shape
        {
            public Triangle(int id, Color color, Tuple<float, float> v1, Tuple<float, float> v2, Tuple<float, float> v3)
                : base(id, color)
            {
                // Thêm 3 đỉnh của tam giác vào danh sách Vertices
                Vertices.Add(v1);
                Vertices.Add(v2);
                Vertices.Add(v3);
            }

            public override void Draw(OpenGL gl)
            {
                gl.Begin(OpenGL.GL_TRIANGLES); // Bắt đầu vẽ tam giác
                gl.Color(Color.R, Color.G, Color.B); // Thiết lập màu sắc

                // Vẽ từng đỉnh của tam giác
                foreach (var vertex in Vertices)
                {
                    gl.Vertex(vertex.Item1, vertex.Item2); // Đặt vị trí từng đỉnh (chỉ có x và y)
                }

                gl.End();
            }
        }


        //HÌNH TRÒN
        private Button drawCircleButton;
        private (float x, float y) circleCenter;
        private float circleRadius;

        //TỨ GIÁC LỒI
        private Button drawQuadrilateralButton;

        //VẼ ĐOẠN THẲNG
        private Button drawLineButton;

        //VẼ PHƯƠNG TRÌNH BẬC HAI
        private Button drawQuadraticButton;
        private TextBox aTextBox, bTextBox, cTextBox;

        //VẼ ĐƯỜNG CONG BEIZER
        private List<(float x, float y)> bezierPoints = new List<(float x, float y)>();
        // Danh sách các điểm cần vẽ
        private List<(float x, float y)> points = new List<(float x, float y)>();
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        public Form1()
        {
            InitializeComponent();

            AllocConsole();

            //TRỤC TỌA ĐỘ
            // Tạo và thiết lập button
            drawAxes = new Button();
            drawAxes.Text = "Trục tọa độ";
            drawAxes.Location = new System.Drawing.Point(10, 850);  // Vị trí của button
            drawAxes.Size = new System.Drawing.Size(100, 30); // Kích thước của button
            // Thêm sự kiện click cho button
            drawAxes.Click += DrawAxesButton_Click;            
            // Thêm button vào form
            Controls.Add(drawAxes);

            //TAM GIÁC
            // Tạo và thiết lập button
            drawTriangleButton = new Button();
            drawTriangleButton.Text = "Vẽ Tam giác";
            drawTriangleButton.Location = new System.Drawing.Point(900, 30);  // Vị trí của button
            drawTriangleButton.Size = new System.Drawing.Size(300, 50); // Kích thước của button
            // Thêm sự kiện click cho button
            drawTriangleButton.Click += DrawTriangleButton_Click;
            // Thêm button vào form
            Controls.Add(drawTriangleButton);

            //HÌNH TRÒN
            // Tạo và thiết lập button
            drawCircleButton = new Button();
            drawCircleButton.Text = "Vẽ Hình tròn";
            drawCircleButton.Location = new System.Drawing.Point(900, 130);  // Vị trí của button
            drawCircleButton.Size = new System.Drawing.Size(300, 50); // Kích thước của button
            // Thêm sự kiện click cho button
            drawCircleButton.Click += DrawCircleButton_Click;
            // Thêm button vào form
            Controls.Add(drawCircleButton);

            //TỨ GIÁC LỒI
            drawQuadrilateralButton = new Button();
            drawQuadrilateralButton.Text = "Vẽ Tứ giác";
            drawQuadrilateralButton.Location = new System.Drawing.Point(900, 230); // Vị trí của button
            drawQuadrilateralButton.Size = new System.Drawing.Size(300, 50); // Kích thước của button
            drawQuadrilateralButton.Click += DrawQuadrilateralButton_Click; // Thêm sự kiện click
            Controls.Add(drawQuadrilateralButton);


            //ĐOẠN THẲNG
            drawLineButton = new Button();
            drawLineButton.Text = "Vẽ Đoạn Thẳng";
            drawLineButton.Location = new System.Drawing.Point(900, 330);  // Vị trí của button
            drawLineButton.Size = new System.Drawing.Size(300, 50); // Kích thước của button
            drawLineButton.Click += DrawLineButton_Click;
            Controls.Add(drawLineButton);

            //PHƯƠNG TRÌNH BẬC 2
            // Nút vẽ phương trình bậc 2
            drawQuadraticButton = new Button();
            drawQuadraticButton.Text = "Vẽ Phương trình bậc 2";
            drawQuadraticButton.Location = new System.Drawing.Point(900, 830);
            drawQuadraticButton.Size = new System.Drawing.Size(500, 100);
            drawQuadraticButton.Click += DrawQuadraticButton_Click;
            Controls.Add(drawQuadraticButton);
            aTextBox = new TextBox { Text = "0", Location = new System.Drawing.Point(1500, 830), Width = 150 };
            bTextBox = new TextBox { Text = "0", Location = new System.Drawing.Point(1500, 890), Width = 150 };
            cTextBox = new TextBox { Text = "0", Location = new System.Drawing.Point(1500, 950), Width = 150 };

            Controls.Add(new Label { Text = "a:", Location = new System.Drawing.Point(1400, 830), Width = 100, Height = 50 });
            Controls.Add(aTextBox);

            Controls.Add(new Label { Text = "b:", Location = new System.Drawing.Point(1400, 890), Width = 100, Height = 50 });
            Controls.Add(bTextBox);

            Controls.Add(new Label { Text = "c:", Location = new System.Drawing.Point(1400, 950), Width = 100, Height = 50 });
            Controls.Add(cTextBox);

            //BEIZER
            Button drawBezierButton = new Button();
            drawBezierButton.Text = "Vẽ đường cong Bezier";
            drawBezierButton.Location = new System.Drawing.Point(900, 430);
            drawBezierButton.Size = new System.Drawing.Size(300, 50);
            drawBezierButton.Click += DrawBezierButton_Click;
            Controls.Add(drawBezierButton);

            // Gắn các sự kiện khởi tạo, vẽ, và nhấp chuột
            openglControl1.OpenGLInitialized += OpenGLControl1_Init;
            openglControl1.OpenGLDraw += Draw;
            openglControl1.MouseClick += MouseClick;
        }

        private void OpenGLControl1_Init(object sender, EventArgs e)
        {
            OpenGL gl = openglControl1.OpenGL;

            gl.Disable(OpenGL.GL_DEPTH_TEST);

            // Chế độ chiếu trực giao 2D
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45.0f, (float)gl.RenderContextProvider.Width / gl.RenderContextProvider.Height, 0.1f, 100.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void AddShape(Action<OpenGL> shape)
        {
            shapes.Add(shape);
            openglControl1.DoRender();
        }

        private void Draw(object sender, RenderEventArgs args)
        {
            OpenGL gl = openglControl1.OpenGL;
            gl.Viewport(0, 0, openglControl1.Width, openglControl1.Height);
            // Thiết lập màu nền
            gl.ClearColor(1, 1, 1, 1);

            // Xóa bộ đệm và thiết lập lại ma trận
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            gl.LookAt(0.0, 0.0, 2.42,  // Eye position
                      0.0, 0.0, 0.0,  // Look-at point
                      0.0, 1.0, 0.0); // Up vector

            // Màu của điểm (đỏ)
            gl.Color(1.0f, 0.0f, 0.0f);

            // Vẽ các hình trong danh sách
            foreach(var shape in shapes)
            {
                shape(gl);
            }
            gl.Flush();

            // Vẽ các điểm trong danh sách
            gl.PointSize(3.0f); // Kích thước điểm
            gl.Begin(OpenGL.GL_POINTS);
            foreach (var point in points)
            {
                gl.Vertex(point.x, point.y);
            }
            gl.End();
            gl.Flush();

            //TRỤC TỌA ĐỘ
            if(isDrawAxes)
            {
                // Vẽ trục X và Y
                gl.Color(0.5f, 0.5f, 0.5f); // Màu xám nhạt cho trục
                gl.Begin(OpenGL.GL_LINES);

                // Trục X
                gl.Vertex(-1.0f, 0.0f); // Điểm bắt đầu (bên trái)
                gl.Vertex(1.0f, 0.0f);  // Điểm kết thúc (bên phải)

                // Trục Y
                gl.Vertex(0.0f, -1.0f); // Điểm bắt đầu (dưới)
                gl.Vertex(0.0f, 1.0f);  // Điểm kết thúc (trên)

                gl.End();

                //Các điểm trên trục
                gl.Color(1.0f, 0.0f, 0.0f);
                float tickSpacing = 0.1f;

                gl.Begin(OpenGL.GL_LINES);
                for (float x = -1.0f; x <= 1.0f && x != 0.0f; x += tickSpacing)
                {
                    gl.Vertex(x, -0.005f); // Vẽ mốc dưới trục X
                    gl.Vertex(x, 0.005f);  // Vẽ mốc trên trục X
                }
                gl.End();
                gl.Flush();

                gl.Begin(OpenGL.GL_LINES);
                for (float y = -1.0f; y <= 1.0f && y != 0.0f; y += tickSpacing)
                {
                    gl.Vertex(-0.005f, y); // Vẽ mốc trái trục Y
                    gl.Vertex(0.005f, y);  // Vẽ mốc phải trục Y
                }
                gl.End();
                gl.Flush();
            }    

            //HÌNH TRÒN
            if (drawMode == DrawMode.Circle && points.Count >= 2)
            {
                var (x1, y1) = points[0];
                var (x2, y2) = points[1];
                circleRadius = (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
                gl.Color(0.0f, 1.0f, 0.5f);
                gl.Begin(OpenGL.GL_LINE_LOOP);
                for (int i = 0; i < 100; i++)
                {
                    float theta = 2.0f * (float)Math.PI * i / 100;
                    float x = x1 + circleRadius * (float)Math.Cos(theta);
                    float y = y1 + circleRadius * (float)Math.Sin(theta);
                    gl.Vertex(x, y);
                }
                gl.End();
            }
            gl.Flush();

            //TỨ GIÁC LỒI
            if (drawMode == DrawMode.Quadrilateral && points.Count >= 4)
            {
                gl.Begin(OpenGL.GL_LINE_LOOP);
                foreach (var point in points)
                {
                    gl.Vertex(point.x, point.y);
                }
                gl.End();
            }
            gl.Flush();

            //ĐOẠN THẲNG
            if (drawMode == DrawMode.Line && points.Count >= 2)
            {
                gl.Color(0.0f, 1.0f, 0.0f);
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(points[0].x, points[0].y);
                gl.Vertex(points[1].x, points[1].y);
                gl.End();
            }
            gl.Flush();


            //BEIZER
            if (drawMode == DrawMode.Beizer && bezierPoints.Count == 4)
            {
                // Kết nối các điểm điều khiển bằng đoạn thẳng
                gl.Color(1f, 0.0f, 0.0f); // Màu xám cho đoạn thẳng điều khiển
                gl.Begin(OpenGL.GL_LINE_STRIP);
                foreach (var point in bezierPoints)
                {
                    gl.Vertex(point.x, point.y);
                }
                gl.End();

                // Vẽ đường Bezier
                gl.Color(1.0f, 0.0f, 1.0f); // Màu tím cho đường cong
                gl.Begin(OpenGL.GL_LINE_STRIP);
                for (float t = 0; t <= 1; t += 0.01f)
                {
                    var p = CalculateBezierPoint(t, bezierPoints);
                    gl.Vertex(p.x, p.y);
                }
                gl.End();
            }
            gl.Flush();

            gl.LookAt(0.0, 0.0, 2.42,  // Eye position
                      0.0, 0.0, 0.0,  // Look-at point
                      0.0, 1.0, 0.0); // Up vector


            //PHƯƠNG TRÌNH BẬC 2
            if (drawMode == DrawMode.Quadratic)
            {
                double a = double.Parse(aTextBox.Text);
                double b = double.Parse(bTextBox.Text);
                double c = double.Parse(cTextBox.Text);

                if (a != null && b != null && c != null)
                {
                    gl.Color(1.0f, 0.5f, 0.0f); // Màu của đồ thị

                    // Vẽ đồ thị hàm số bậc 2

                    gl.Begin(OpenGL.GL_LINE_STRIP);
                    for (float x = -2.0f; x <= 2.0f; x += 0.01f)
                    {
                        float y = (float)(a * x * x + b * x + c);
                        gl.Vertex(x, y);
                    }
                    gl.End();

                    // Tính đạo hàm và cực trị
                    if (flag)
                    {
                        QuadraticProperties(a, b, c);
                    }
                }
            }
            gl.Flush();


        }

        private void ResetDrawingState()
        {
            points.Clear();
            drawMode = DrawMode.None;
        }


        private void MouseClick(object sender, MouseEventArgs e)
        {
            // Chuyển đổi tọa độ chuột sang tọa độ OpenGL
            float x = (float)e.X / openglControl1.Width * 2 - 1.0f;
            float y = 1.0f - (float)e.Y / openglControl1.Height * 2;

            if (drawMode == DrawMode.Triangle)
            {
                if (points.Count < 3)
                {
                    points.Add((x, y));
                    Console.WriteLine($"Triangle point: ({x}, {y})");
                }
                if (points.Count == 3)  // Đủ 3 điểm để vẽ tam giác
                {
                    AddShape(gl =>
                    {
                        gl.Color(1.0f, 0.5f, 0.0f);
                        gl.Begin(OpenGL.GL_LINE_LOOP);
                        foreach (var point in points)
                        {
                            gl.Vertex(point.x, point.y);
                        }
                        gl.End();
                    });
                    double area = TriangleArea(points);
                    double perimeter = TrianglePerimeter(points);

                    Console.WriteLine($"Triangle Area: {area}");
                    Console.WriteLine($"Triangle Perimeter: {perimeter}");
                }
            }
            if (drawMode == DrawMode.Circle)
            {
                if (points.Count < 2)
                {
                    points.Add((x, y));
                    Console.WriteLine($"Circle point: ({x}, {y})");
                }
                if (points.Count == 2)  // Đủ 2 điểm để vẽ hình tròn
                {
                    var (x1, y1) = points[0];
                    var (x2, y2) = points[1];
                    circleRadius = (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

                    double area = Math.PI * Math.Pow(circleRadius, 2);
                    double perimeter = 2 * Math.PI * circleRadius;

                    Console.WriteLine($"Circle Area: {area}");
                    Console.WriteLine($"Circle Perimeter: {perimeter}");

                    openglControl1.Invalidate();
                }
            }
            if (drawMode == DrawMode.Quadrilateral)
            {
                if (points.Count < 4)
                {
                    points.Add((x, y));
                    Console.WriteLine($"Quadrilateral point: ({x}, {y})");
                }
                if (points.Count == 4) // Đủ 4 điểm để vẽ tứ giác
                {
                    double area = QuadrilateralArea(points);
                    double perimeter = QuadrilateralPerimeter(points);

                    Console.WriteLine($"Quadrilateral Area: {area}");
                    Console.WriteLine($"Quadrilateral Perimeter: {perimeter}");

                    openglControl1.Invalidate();
                }
            }
            if (drawMode == DrawMode.Line)
            {
                if (points.Count < 2)
                {
                    points.Add((x, y));
                    Console.WriteLine($"Click at: ({e.X}, {e.Y}) => OpenGL Coordinates ({x}, {y})");
                }

                if (points.Count == 2)
                {
                    double length = LineLength(points);
                    Console.WriteLine($"Length of the Line Segment: {length}");

                    // Yêu cầu làm mới OpenGLControl
                    openglControl1.Invalidate();
                }
            }
            if (drawMode == DrawMode.Beizer)
            {
                if (bezierPoints.Count < 4)
                {
                    bezierPoints.Add((x, y));
                    Console.WriteLine($"Control point: ({x}, {y})");

                    // Nếu đủ 4 điểm, yêu cầu vẽ
                    if (bezierPoints.Count == 4)
                    {
                        Console.WriteLine("Drawing Bezier.");
                        openglControl1.Invalidate(); // Yêu cầu làm mới OpenGL
                    }
                }
            }

        }
        private double Distance((float x, float y) p1, (float x, float y) p2)
        {
            return Math.Sqrt(Math.Pow(p2.x - p1.x, 2) + Math.Pow(p2.y - p1.y, 2));
        }

        //TRỤC TỌA ĐỘ.
        private void DrawAxesButton_Click(object sender, EventArgs e)
        {
            isDrawAxes = !isDrawAxes;
        }

        //TAM GIÁC
        private void DrawTriangleButton_Click(object sender, EventArgs e)
        {
            ResetDrawingState();
            points.Clear();
            drawMode = DrawMode.Triangle;
            // In ra thông báo
            Console.WriteLine("Click 3 points to draw Triangle");
        }
        private double TriangleArea(List<(float x, float y)> points)
        {
            if (points.Count != 3) return 0;

            float x1 = points[0].x, y1 = points[0].y;
            float x2 = points[1].x, y2 = points[1].y;
            float x3 = points[2].x, y3 = points[2].y;

            return 0.5 * Math.Abs(x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2));
        }
        private double TrianglePerimeter(List<(float x, float y)> points)
        {
            if (points.Count != 3) return 0;

            double d1 = Distance(points[0], points[1]);
            double d2 = Distance(points[1], points[2]);
            double d3 = Distance(points[2], points[0]);

            return d1 + d2 + d3;
        }

        //HÌNH TRÒN
        private void DrawCircleButton_Click(object sender, EventArgs e)
        {
            ResetDrawingState();
            points.Clear();
            drawMode = DrawMode.Circle;
            // In ra thông báo
            Console.WriteLine("Click 2 points to draw Circle, the first is Center, the second is on Edge");
        }

        //TỨ GIÁC LỒI
        private void DrawQuadrilateralButton_Click(Object sender, EventArgs e)
        {
            ResetDrawingState();
            points.Clear();
            drawMode = DrawMode.Quadrilateral;
            // In ra thông báo
            Console.WriteLine("Click 4 points to draw Quadrilateral");
        }
        private double QuadrilateralArea(List<(float x, float y)> points)
        {
            if (points.Count != 4) return 0;

            float x1 = points[0].x, y1 = points[0].y;
            float x2 = points[1].x, y2 = points[1].y;
            float x3 = points[2].x, y3 = points[2].y;
            float x4 = points[3].x, y4 = points[3].y;

            // Công thức diện tích đa giác theo tọa độ
            return 0.5 * Math.Abs(
                x1 * y2 + x2 * y3 + x3 * y4 + x4 * y1 -
                (y1 * x2 + y2 * x3 + y3 * x4 + y4 * x1)
            );
        }
        private double QuadrilateralPerimeter(List<(float x, float y)> points)
        {
            if (points.Count != 4) return 0;

            double d1 = Distance(points[0], points[1]);
            double d2 = Distance(points[1], points[2]);
            double d3 = Distance(points[2], points[3]);
            double d4 = Distance(points[3], points[0]);

            return d1 + d2 + d3 + d4;
        }

        //ĐOẠN THẲNG
        private void DrawLineButton_Click(object sender, EventArgs e)
        {
            ResetDrawingState();
            points.Clear();
            drawMode = DrawMode.Line;
            // In ra thông báo
            Console.WriteLine("Click 2 points to draw Line Segment");
        }
        private double LineLength(List<(float x, float y)> points)
        {
            if (points.Count != 2) return 0;

            // Công thức diện tích đa giác theo tọa độ
            return Distance(points[0], points[1]);
        }

        //PHƯƠNG TRÌNH BẬC 2
        private void DrawQuadraticButton_Click(object sender, EventArgs e)
        {
            ResetDrawingState();
            drawMode = DrawMode.Quadratic;
            points.Clear();
            Console.WriteLine("Quadratic Graph");

            openglControl1.Invalidate();
        }
        private void QuadraticProperties(double a, double b, double c)
        {
            // Tính đạo hàm
            Console.WriteLine("Derivative: y' = " + (2 * a) + "x + " + b);

            // Tính cực trị
            if (a != 0)
            {
                double xVertex = -b / (2 * a);
                double yVertex = a * xVertex * xVertex + b * xVertex + c;
                Console.WriteLine($"Extrenum: ({xVertex}, {yVertex})");
            }
            else
            {
                Console.WriteLine("Do not have any Extrenum!");
            }
            flag = false;
        }

        //BEIZER
        private void DrawBezierButton_Click(object sender, EventArgs e)
        {
            bezierPoints.Clear();
            points.Clear();
            ResetDrawingState();
            drawMode = DrawMode.Beizer;
            Console.WriteLine("Click 4 điểm để tạo đường Bezier.");
        }
        private (float x, float y) CalculateBezierPoint(float t, List<(float x, float y)> controlPoints)
        {
            float x = (float)(
                Math.Pow(1 - t, 3) * controlPoints[0].x +
                3 * Math.Pow(1 - t, 2) * t * controlPoints[1].x +
                3 * (1 - t) * Math.Pow(t, 2) * controlPoints[2].x +
                Math.Pow(t, 3) * controlPoints[3].x
            );

            float y = (float)(
                Math.Pow(1 - t, 3) * controlPoints[0].y +
                3 * Math.Pow(1 - t, 2) * t * controlPoints[1].y +
                3 * (1 - t) * Math.Pow(t, 2) * controlPoints[2].y +
                Math.Pow(t, 3) * controlPoints[3].y
            );

            return (x, y);
        }

    }
}
