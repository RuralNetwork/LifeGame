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
        //Singleton
        public static GraphicsEngine Instance;

        //Had to use Canvas, since I want an absolute positioning in space.
        //When a new Map is created, there will be limits, then, since we know how big it is, we center it
        //Viewbox was too limited as per the child it could have, stackpanel puts everything with a DOM logic
        //canvas was the only thing that let me do what I wanted, if there is something better we can change it now, later it will be too painful
        private Canvas _canvas { get; set; }
        //For any callback use this as type
        public delegate void Del();
        public double canvasHeight { get; set; }
        public double canvasWidth { get; set; }
        public int hexaW = 40;
        public int hexaH = 34;

        public Dictionary<ThingType, ImageBrush> TerrainBrushes = new Dictionary<ThingType, ImageBrush>();
        public Dictionary<ActionType, ImageBrush> ActionBrushes = new Dictionary<ActionType, ImageBrush>();

        /// <summary>
        /// This is the update and draw speed.
        /// </summary>
        public float FPS = 10;

        public GraphicsEngine(Canvas canvas)
        {
            Instance = this;
            var directory = "file:///" + System.IO.Directory.GetCurrentDirectory() + @"\Resources\";
            TerrainBrushes.Add(ThingType.Earth, new ImageBrush(new BitmapImage(new Uri(directory + "earth.png"))));
            TerrainBrushes.Add(ThingType.Grass, new ImageBrush(new BitmapImage(new Uri(directory + "grass.png"))));
            TerrainBrushes.Add(ThingType.Water, new ImageBrush(new BitmapImage(new Uri(directory + "water.png"))));
            TerrainBrushes.Add(ThingType.Bush, new ImageBrush(new BitmapImage(new Uri(directory + "bush.png"))));
            TerrainBrushes.Add(ThingType.Berry, new ImageBrush(new BitmapImage(new Uri(directory + "berry.png"))));
            TerrainBrushes.Add(ThingType.Sand, new ImageBrush(new BitmapImage(new Uri(directory + "sand.png"))));

            ActionBrushes.Add(ActionType.Walk, new ImageBrush(new BitmapImage(new Uri(directory + "being.png"))));
            ActionBrushes.Add(ActionType.Breed, new ImageBrush(new BitmapImage(new Uri(directory + "breeding.png"))));
            ActionBrushes.Add(ActionType.Eat, new ImageBrush(new BitmapImage(new Uri(directory + "eating.png"))));
            ActionBrushes.Add(ActionType.Sleep, new ImageBrush(new BitmapImage(new Uri(directory + "sleeping.png"))));
            ActionBrushes.Add(ActionType.Fight , new ImageBrush(new BitmapImage(new Uri(directory + "being.png"))));
            ActionBrushes.Add(ActionType.Take, new ImageBrush(new BitmapImage(new Uri(directory + "being.png"))));
            ActionBrushes.Add(ActionType.Drop, new ImageBrush(new BitmapImage(new Uri(directory + "being.png"))));
            ActionBrushes.Add(ActionType.MakeSound, new ImageBrush(new BitmapImage(new Uri(directory + "being.png"))));

            _canvas = canvas;
            canvasHeight = canvas.Height;
            canvasWidth = canvas.Width;
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
                    callbackOK();
                }

            }


        }
        public void addBeing(Being obj)
        {
            Polygon poligono = new Polygon();
            poligono.Points = getPointCollectionBeing();
            poligono.Fill = ActionBrushes[ActionType.Walk];

            TranslateTransform translate = new TranslateTransform((Double)30 * obj.Location.CartesianX, (Double)30 * obj.Location.CartesianY);
            poligono.RenderTransform = translate;

            this._canvas.Children.Add(poligono);
            //Links the polygon to the thing
            obj.polygon = poligono;
        }

        public void WalkAnimation(Being being)
        {
            Debug.Write("Move\n");
            if (FPS > 0)
            {
                DoubleAnimationUsingKeyFrames xWalk = new DoubleAnimationUsingKeyFrames();
                DoubleAnimationUsingKeyFrames yWalk = new DoubleAnimationUsingKeyFrames();
                xWalk.Duration = TimeSpan.FromSeconds(1 / FPS);
                yWalk.Duration = TimeSpan.FromSeconds(1 / FPS);
                var cell = being.OldLoc;
                xWalk.KeyFrames.Add(new LinearDoubleKeyFrame((double)30 * cell.CartesianX, KeyTime.FromPercent(0)));
                yWalk.KeyFrames.Add(new LinearDoubleKeyFrame((double)30 * cell.CartesianY, KeyTime.FromPercent(0)));
                for (int i = 1; i <= being.NCellsWalk; i++)
                {
                    var dummyCell = cell.GetNearCell(being.Direction);
                    if (i < being.NCellsWalk)
                    {
                        cell = Simulation.Instance.Cycle(dummyCell);
                    }
                    else
                    {
                        cell = being.Location;
                    }

                    xWalk.KeyFrames.Add(new LinearDoubleKeyFrame((double)30 * dummyCell.CartesianX, KeyTime.FromPercent((double)i / being.NCellsWalk)));
                    yWalk.KeyFrames.Add(new LinearDoubleKeyFrame((double)30 * dummyCell.CartesianY, KeyTime.FromPercent((double)i / being.NCellsWalk)));

                    if (!cell.Equals(dummyCell))
                    {
                        xWalk.KeyFrames.Add(new DiscreteDoubleKeyFrame((double)30 * cell.CartesianX, KeyTime.FromPercent((double)i / being.NCellsWalk)));
                        yWalk.KeyFrames.Add(new DiscreteDoubleKeyFrame((double)30 * cell.CartesianY, KeyTime.FromPercent((double)i / being.NCellsWalk)));
                    }
                }

                Transform translate = being.polygon.RenderTransform;
                translate.BeginAnimation(TranslateTransform.XProperty, xWalk);
                translate.BeginAnimation(TranslateTransform.YProperty, yWalk);
            }
        }

        public void ChangeBeingTex(Being being)
        {
            being.polygon.Fill = ActionBrushes[being.LastAction];
        }


        public void removeThing(Thing obj)
        {
            Debug.Write("Removed Thing\n");
            this._canvas.Children.Remove(obj.polygon);
        }

        public void addCell(Thing obj)
        {

            Polygon poligono = new Polygon();

            poligono.Points = this.getPointCollection();
            //poligono.Name = "thing" + obj.ID;
            //This should store the object he represents
            poligono.DataContext = obj;
            poligono.Stroke = System.Windows.Media.Brushes.White;
            poligono.StrokeThickness = 1;

            //poligono.Fill = new SolidColorBrush(switchColor(obj.Type));
            //System.Drawing.Image image = System.Drawing.Image.FromFile("grass.png"); 

            poligono.Fill = TerrainBrushes[obj.Type];

            //Set the position inside the canvas
            TranslateTransform translate = new TranslateTransform((Double)30 * obj.Location.CartesianX, (Double)30 * obj.Location.CartesianY);
            poligono.RenderTransform = translate;

            poligono.AddHandler(Polygon.MouseUpEvent, new RoutedEventHandler(cellaMouseEnter));

            //Adds the polygon to the canvas
            this._canvas.Children.Add(poligono);
            //Links the polygon to the thing
            obj.polygon = poligono; //la fantasia con i nomi...
        }

        public void updateCell(Thing obj)
        {
            obj.polygon.Fill = TerrainBrushes[obj.Type];
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
            //cosa.showID();
            cosa.ChangeType(this.currentType, null);
            cosa.Apply();
        }
    }
}
