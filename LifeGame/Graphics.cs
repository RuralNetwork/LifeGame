using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
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
        private BitmapImage grass = new BitmapImage(new Uri("file:///" + System.IO.Directory.GetCurrentDirectory() + @"\Resources\grass.png"));
        private BitmapImage earth = new BitmapImage(new Uri("file:///" + System.IO.Directory.GetCurrentDirectory() + @"\Resources\earth.png"));
        private BitmapImage water = new BitmapImage(new Uri("file:///" + System.IO.Directory.GetCurrentDirectory() + @"\Resources\water.png"));
        private BitmapImage bush = new BitmapImage(new Uri("file:///" + System.IO.Directory.GetCurrentDirectory() + @"\Resources\bush.png"));
        private BitmapImage berry = new BitmapImage(new Uri("file:///" + System.IO.Directory.GetCurrentDirectory() + @"\Resources\berry.png"));
        private BitmapImage sand = new BitmapImage(new Uri("file:///" + System.IO.Directory.GetCurrentDirectory() + @"\Resources\sand.png"));
        public bool editing;

        /// <summary>
        /// This is the update and draw speed.
        /// </summary>
        public float FPS = 2;

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
        private PointCollection getPointCollection()
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
        private PointCollection getPointCollectionBeing()
        {
            PointCollection series = new PointCollection();
            series.Add(new System.Windows.Point(3, 0));
            series.Add(new System.Windows.Point(37, 0));
            series.Add(new System.Windows.Point(37, 34));
            series.Add(new System.Windows.Point(3, 34));
            return series;
        }

        public void messageBox(int x, int y, int w, int h, string message, int life = 0, Del callbackOK = null, Del callbackNO = null)
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
        public void addBeing(Being obj, GridPoint location)
        {
            Polygon poligono = new Polygon();
            poligono.Points = getPointCollectionBeing();
            BitmapImage being = new BitmapImage(new Uri("file:///" + System.IO.Directory.GetCurrentDirectory() + @"\Resources\being.png"));
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = being;
            poligono.Fill = brush;

            TranslateTransform translate = new TranslateTransform((Double)30 * location.X, (Double)((34 * location.Y) + (obj.Location.X % 2 == 0 ? 0 : 17)));
            poligono.RenderTransform = translate;

            this._canvas.Children.Add(poligono);
            //Links the polygon to the thing
            obj.polygon = poligono;
        }
        public void changeBeing(Being obj)
        {
            Debug.Write("Move\n");
            //obj.Location
            Duration duration = new Duration(TimeSpan.FromSeconds(1 / FPS));

            //TranslateTransform translate = new TranslateTransform((Double)30 * obj.Location.X, (Double)((34 * obj.Location.Y) + (obj.Location.X % 2 == 0 ? 0 : 17)));
            DoubleAnimation ascissa = new DoubleAnimation((Double)30 * obj.OldLoc.X, (Double)30 * obj.Location.X, duration);
            DoubleAnimation ordinata = new DoubleAnimation((Double)((34 * obj.OldLoc.Y) + (obj.OldLoc.X % 2 == 0 ? 0 : 17)), 
                                        (Double)((34 * obj.Location.Y) + (obj.Location.X % 2 == 0 ? 0 : 17)), duration);
            Transform translate = obj.polygon.RenderTransform;
            translate.BeginAnimation(TranslateTransform.XProperty, ascissa);
            translate.BeginAnimation(TranslateTransform.YProperty, ordinata);
        }
        //Careful, it removes also things
        public void removeBeing(Being obj)
        {
            Debug.Write("Removed Being\n");
            this._canvas.Children.Remove(obj.polygon);
        }
        //location is useless
        public void addCell(Thing obj, GridPoint location)
        {

            Polygon poligono = new Polygon();

            poligono.Points = this.getPointCollection();
            poligono.Name = "thing" + obj.ID;
            //This should store the object he represents
            poligono.DataContext = obj;
            poligono.Stroke = System.Windows.Media.Brushes.White;
            poligono.StrokeThickness = 1;

            //poligono.Fill = new SolidColorBrush(switchColor(obj.Type));
            //System.Drawing.Image image = System.Drawing.Image.FromFile("grass.png"); 

            poligono.Fill = switchGround(obj.Type);

            //Set the position inside the canvas
            TranslateTransform translate = new TranslateTransform((Double)30 * location.X, (Double)((34 * location.Y) + (location.X % 2 == 0 ? 0 : 17)));
            poligono.RenderTransform = translate;

            poligono.AddHandler(Polygon.MouseUpEvent, new RoutedEventHandler(cellaMouseEnter));

            //Adds the polygon to the canvas
            this._canvas.Children.Add(poligono);
            //Links the polygon to the thing
            obj.polygon = poligono; //la fantasia con i nomi...
        }
        private System.Windows.Media.Color switchColor(ThingType type)
        {
            System.Windows.Media.Color earth = System.Windows.Media.Color.FromRgb(205, 179, 128);
            System.Windows.Media.Color water = System.Windows.Media.Color.FromRgb(105, 210, 231);
            System.Windows.Media.Color color;
            switch (type)
            {
                case ThingType.Earth:
                    color = earth;
                    break;
                case ThingType.Water:
                    color = water;
                    break;
                default:
                    color = System.Windows.Media.Color.FromRgb(255, 255, 255);
                    break;
            }
            return color;
        }

        private ImageBrush switchGround(ThingType type)
        {
            BitmapImage image;
            ImageBrush brush = new ImageBrush();

            switch (type)
            {
                case ThingType.Earth:
                    image = earth;
                    break;
                case ThingType.Water:
                    image = water;
                    break;
                case ThingType.Grass:
                    image = grass;
                    break;
                case ThingType.Bush:
                    image = bush;
                    break;
                case ThingType.Berry:
                    image = berry;
                    break;
                case ThingType.Sand:
                    image = sand;
                    break;
                default:
                    image = grass;
                    break;
            }
            brush.ImageSource = image;
            return brush;
        }
        public void updateCell(Thing obj)
        {
            obj.polygon.Fill = switchGround(obj.Type);
        }
        private ThingType currentType = ThingType.Earth;
        public void changeBrush(string name)
        {
            this.currentType = (ThingType)Enum.Parse(typeof(ThingType), name, true);
        }
        private void cellaMouseEnter(object sender, RoutedEventArgs e)
        {
            Polygon poligono = e.Source as Polygon;
            Thing cosa = poligono.DataContext as Thing;

            /*poligono.Fill = System.Windows.Media.Brushes.Aqua;*/
            //Debug stuff
            Debug.Write("Over a polygon " + poligono.Name + "\n");
            cosa.showID();
            if (editing)
            {
                cosa.ChangeType(this.currentType, null);
                cosa.Apply();
            }
        }

        public void startSimulation()
        {
            Debug.Write("Simulation started\n");
        }
    }
}
