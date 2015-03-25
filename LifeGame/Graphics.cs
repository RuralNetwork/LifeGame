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
        //For any callback use this as type
        public delegate void Del(GraphicsEngine engine); 
        public double canvasHeight { get; set; }
        public double canvasWidth { get; set; }
        public int hexaW = 40;
        public int hexaH = 34;
        public GraphicsEngine(Canvas canvas)
        {
            _canvas = canvas;
            canvasHeight = canvas.Height;
            canvasWidth = canvas.Width;
            //tentativo di parent
            //var parent = canvas.Parent;

        }
        public void setViewbox(Canvas newCanvas)
        {
            _canvas = newCanvas;
        }
        //Get the points to make a hexagon
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
        
        public void messageBox(int x, int y, int w, int h, string message, int life=0, Del callbackOK=null, Del callbackNO=null)
        {
            //If x and y are null, put it in the center
            var rect = new System.Windows.Shapes.Rectangle();
            if (x == null && y == null)
            {
                Double X = (double)((this.canvasWidth / 2) - (w / 2));
                Double Y = (double)((this.canvasHeight / 2) - (h / 2));

                TranslateTransform translate = new TranslateTransform(X, Y);
            }
            else
            {
                TranslateTransform translate = new TranslateTransform((double)x, (double)y);
            }
            rect.Height = h;
            rect.Width = w;

            rect.Fill = System.Windows.Media.Brushes.Beige;

            this._canvas.Children.Add(rect);
            if (life > 0)
            {
                //Fade out after a bit
            }
            else
            {
                //Put buttons

                if (callbackOK != null)
                {
                    callbackOK(this);
                }

            }

            
        }
        public void addCell(GridPoint location/*,ImageBrush sprite or something, it's possible*/)
        {
            Polygon poligono = new Polygon();
            

            poligono.Points = this.getPointCollection(location.X, location.Y);
            poligono.Stroke = System.Windows.Media.Brushes.Black;
            poligono.StrokeThickness = 1;

            //Set the position inside the canvas
            TranslateTransform translate = new TranslateTransform((Double)30 * location.X, (Double)((34 * location.Y) + (location.X % 2 == 0 ? 0 : 17)));
            poligono.RenderTransform = translate;

            poligono.AddHandler(Polygon.MouseMoveEvent, new RoutedEventHandler(cellaMouseEnter));
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
}
