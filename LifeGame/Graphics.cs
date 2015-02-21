using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace LifeGame
{
    /* 
     * | constructor:   {constr}[Viewbox]   required
     * |                Starts a new instance of Graphics, sets the viewbox
     * | setViewbox:    {void}  [Viewbox]   required
     * |                Changes the viewbox
     * | addCell:       {void}  [int,int]   required
     * |                Adds a cell into the viewbox
     * |
     */
    public class GraphicsEngine
    {
        //Had to use Canvas, since I want an absolute positioning in space.
        //When a new Map is created, there will be limits, then, since we know how big it is, we center it
        //Viewbox was too limited as per the child it could have, stackpanel puts everything with a DOM logic
        //canvas was the only thing that let me do what I wanted, if there is something better we can change it now, later it will be too painful
        private Canvas _canvas { get; set; }
        public GraphicsEngine(Canvas canvas)
        {
            _canvas = canvas;

        }
        public void setViewbox(Canvas newCanvas)
        {
            _canvas = newCanvas;
        }
        private PointCollection getPointCollection(int x, int y)
        {
            PointCollection series = new PointCollection();
            series.Add(new System.Windows.Point(10, 0));
            series.Add(new System.Windows.Point(30, 0));
            series.Add(new System.Windows.Point(40, 17));
            series.Add(new System.Windows.Point(30, 34));
            series.Add(new System.Windows.Point(10, 34));
            series.Add(new System.Windows.Point(0, 17));
            return series;
        }
        public void addCell(int x, int y/*,ImageBrush sprite or something, it's possible*/)
        {
            Polygon poligono = new Polygon();
            TranslateTransform translate = new TranslateTransform((Double)30 * x, (Double)((34 * y) + (x % 2 == 0 ? 0 : 17)));

            poligono.Points = this.getPointCollection(x, y);
            poligono.Stroke = System.Windows.Media.Brushes.Black;
            poligono.StrokeThickness = 1;

            poligono.RenderTransform = translate;

            poligono.AddHandler(Polygon.MouseEnterEvent, new RoutedEventHandler(cellaMouseEnter));
            //Adds the polygon to the canvas
            this._canvas.Children.Add(poligono);
        }
        private void cellaMouseEnter(object sender, RoutedEventArgs e)
        {
            Polygon poligono = e.Source as Polygon;
            poligono.Fill = System.Windows.Media.Brushes.Aqua;
        }
        public void startSimulation()
        {
            Debug.Write("Simulation started\n");
        }
    }
    /* Well, this was a waste of 2 hour of my time. Finding out only now that all this code is alrady implemented in WPF, in the MSDN guide they use it to explain how it works, they do not provide this as a sample in order for you to use it, unless you are building something from scratch, but we are using pre-built things, so this is useless and conflits with the already-existing shit
    public class visualHost : FrameworkElement
    {
        // Provide a required override for the VisualChildrenCount property. 
        protected override int VisualChildrenCount
        {
            get { return board.Count; }
        }

        // Provide a required override for the GetVisualChild method. 
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= board.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return board[index];
        }

        private VisualCollection board;
        public visualHost()
        {
            board = new VisualCollection(this);
            board.Add(CreateDrawingVisualRectangle());
            Debug.Write("Prova 7.1\n");
            // Add the event handler for MouseLeftButtonUp. 
            this.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(visualHost_MouseLeftButtonUp);
        }
        // Create a DrawingVisual that contains a rectangle. 
        private DrawingVisual CreateDrawingVisualRectangle()
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            // Retrieve the DrawingContext in order to create new drawing content.
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Create a rectangle and draw it in the DrawingContext.
            Rect rect = new Rect(new System.Windows.Point(10, 10), new System.Windows.Size(320, 80));
            drawingContext.DrawRectangle(System.Windows.Media.Brushes.LightBlue, (System.Windows.Media.Pen)null, rect);

            // Persist the drawing content.
            drawingContext.Close();

            return drawingVisual;
        }
        // Capture the mouse event and hit test the coordinate point value against 
        // the child visual objects. 
        void visualHost_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Retreive the coordinates of the mouse button event.
            System.Windows.Point pt = e.GetPosition((UIElement)sender);

            // Initiate the hit test by setting up a hit test result callback method.
            VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(myCallback), new PointHitTestParameters(pt));
        }

        // If a child visual object is hit, toggle its opacity to visually indicate a hit. 
        public HitTestResultBehavior myCallback(HitTestResult result)
        {
            if (result.VisualHit.GetType() == typeof(DrawingVisual))
            {
                if (((DrawingVisual)result.VisualHit).Opacity == 1.0)
                {
                    ((DrawingVisual)result.VisualHit).Opacity = 0.4;
                }
                else
                {
                    ((DrawingVisual)result.VisualHit).Opacity = 1.0;
                }
            }

            // Stop the hit test enumeration of objects in the visual tree. 
            return HitTestResultBehavior.Stop;
        }
    }*/
}
