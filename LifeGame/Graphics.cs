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
        public const int GRID_WIDTH = 30;
        public const int GRID_HEIGHT = 20;
        public const int POPULATION_COUNT = 20;
        public const int MAX_POPULATION_COUNT = POPULATION_COUNT * 3 / 2;

        //Singleton
        public static GraphicsEngine Instance;

        //Had to use Canvas, since I want an absolute positioning in space.
        //When a new Map is created, there will be limits, then, since we know how big it is, we center it
        //Viewbox was too limited as per the child it could have, stackpanel puts everything with a DOM logic
        //canvas was the only thing that let me do what I wanted, if there is something better we can change it now, later it will be too painful
        MainWindow window;
        Canvas canvas;
        //For any callback use this as type
        //public double canvasHeight { get; set; }
        //public double canvasWidth { get; set; }


        public Dictionary<ThingType, ImageBrush> TerrainBrushes = new Dictionary<ThingType, ImageBrush>();
        public Dictionary<ActionType, ImageBrush> ActionBrushes = new Dictionary<ActionType, ImageBrush>();

        Polygon[] beingShapes;
        Polygon[][] terrainPolygons;

        /// <summary>
        /// This is the update and draw speed.
        /// </summary>
        public float FPS = 10;

        public GraphicsEngine(MainWindow window)
        {
            canvas = window.mainpanel;
            this.window = window;

            Instance = this;
            var directory = "file:///" + System.IO.Directory.GetCurrentDirectory() + @"\Resources\";
            TerrainBrushes.Add(ThingType.Earth, new ImageBrush(new BitmapImage(new Uri(directory + "earth.png"))));
            TerrainBrushes.Add(ThingType.Grass, new ImageBrush(new BitmapImage(new Uri(directory + "grass.png"))));
            TerrainBrushes.Add(ThingType.Water, new ImageBrush(new BitmapImage(new Uri(directory + "water.png"))));
            TerrainBrushes.Add(ThingType.Bush, new ImageBrush(new BitmapImage(new Uri(directory + "bush.png"))));
            TerrainBrushes.Add(ThingType.Berry, new ImageBrush(new BitmapImage(new Uri(directory + "berry.png"))));
            TerrainBrushes.Add(ThingType.Sand, new ImageBrush(new BitmapImage(new Uri(directory + "sand.png"))));
            TerrainBrushes.Add(ThingType.Mountain, new ImageBrush(new BitmapImage(new Uri(directory + "mountain.png"))));

            ActionBrushes.Add(ActionType.Walk, new ImageBrush(new BitmapImage(new Uri(directory + "being.png"))));
            ActionBrushes.Add(ActionType.Breed, new ImageBrush(new BitmapImage(new Uri(directory + "breeding.png"))));
            ActionBrushes.Add(ActionType.Eat, new ImageBrush(new BitmapImage(new Uri(directory + "eating.png"))));
            ActionBrushes.Add(ActionType.Sleep, new ImageBrush(new BitmapImage(new Uri(directory + "sleeping.png"))));
            ActionBrushes.Add(ActionType.Fight, new ImageBrush(new BitmapImage(new Uri(directory + "being.png"))));
            ActionBrushes.Add(ActionType.Take, new ImageBrush(new BitmapImage(new Uri(directory + "being.png"))));
            ActionBrushes.Add(ActionType.Drop, new ImageBrush(new BitmapImage(new Uri(directory + "being.png"))));
            ActionBrushes.Add(ActionType.MakeSound, new ImageBrush(new BitmapImage(new Uri(directory + "being.png"))));

            var hexPoints = new PointCollection();
            hexPoints.Add(new System.Windows.Point(10, 0));
            hexPoints.Add(new System.Windows.Point(30, 0));
            hexPoints.Add(new System.Windows.Point(40, 17));
            hexPoints.Add(new System.Windows.Point(30, 34));
            hexPoints.Add(new System.Windows.Point(10, 34));
            hexPoints.Add(new System.Windows.Point(0, 17));
            terrainPolygons = new Polygon[GRID_WIDTH][];
            for (int x = 0; x < GRID_WIDTH; x++)
            {
                terrainPolygons[x] = new Polygon[GRID_HEIGHT];
                for (int y = 0; y < GRID_HEIGHT; y++)
                {
                    var pt = new GridPoint(x, y);
                    var poly = new Polygon() { Points = hexPoints };
                    poly.RenderTransform = new TranslateTransform((Double)30 * pt.CartesianX, (Double)30 * pt.CartesianY);
                    poly.MouseUp += cellClick;
                    poly.Tag = pt;
                    terrainPolygons[x][y] = poly;
                    poly.Fill = TerrainBrushes[ThingType.Earth];
                    canvas.Children.Add(poly);
                }
            }
            int id = 0;
            PointCollection series = new PointCollection();
            series.Add(new System.Windows.Point(3, 0));
            series.Add(new System.Windows.Point(37, 0));
            series.Add(new System.Windows.Point(37, 34));
            series.Add(new System.Windows.Point(3, 34));
            beingShapes = new Polygon[MAX_POPULATION_COUNT];
            for (int i = 0; i < MAX_POPULATION_COUNT; i++)
            {
                var poly = new Polygon() { Points = series };
                poly.MouseUp += beingClick;
                poly.Tag = id;
                beingShapes[id++] = poly;
                canvas.Children.Add(poly);
                poly.Visibility = Visibility.Hidden;
            }

        }

        //public void messageBox(int x, int y, int w, int h, string message, int life = 0, Del callbackOK = null, Del callbackNO = null)
        //{
        //    //If x and y are null, put it in the center
        //    var rect = new System.Windows.Shapes.Rectangle();
        //    if (x == null && y == null)
        //    {
        //        Double X = (double)((this.canvasWidth / 2) - (w / 2));
        //        Double Y = (double)((this.canvasHeight / 2) - (h / 2));

        //        TranslateTransform translate = new TranslateTransform(X, Y);
        //    }
        //    else
        //    {
        //        TranslateTransform translate = new TranslateTransform((double)x, (double)y);
        //    }
        //    rect.Height = h;
        //    rect.Width = w;

        //    rect.Fill = System.Windows.Media.Brushes.Beige;

        //    this._canvas.Children.Add(rect);
        //    if (life > 0)
        //    {
        //        //Fade out after a bit
        //    }
        //    else
        //    {
        //        //Put buttons

        //        if (callbackOK != null)
        //        {
        //            callbackOK();
        //        }

        //    }


        //}
        public void addBeing(Being being)
        {
            var shape = beingShapes[being.ID];
            shape.RenderTransform = new TranslateTransform((Double)30 * being.Location.CartesianX, (Double)30 * being.Location.CartesianY);
            shape.Visibility = Visibility.Visible;
            ChangeBeingTex(being);
            shape.Fill = ActionBrushes[ActionType.Walk];
        }

        public void ChangeBeingTex(Being being)
        {
            beingShapes[being.ID].Fill = ActionBrushes[being.LastAction];
        }

        public void RemoveBeing(Being being)
        {
            beingShapes[being.ID].Visibility = Visibility.Hidden;
        }

        public void WalkAnimation(Being being)
        {
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
                        cell = Cycle(dummyCell);
                    }
                    else
                    {
                        cell = being.Location;
                    }


                    if (!cell.Equals(dummyCell) || i == being.NCellsWalk)
                    {
                        xWalk.KeyFrames.Add(new LinearDoubleKeyFrame((double)30 * dummyCell.CartesianX, KeyTime.FromPercent((double)i / being.NCellsWalk)));
                        yWalk.KeyFrames.Add(new LinearDoubleKeyFrame((double)30 * dummyCell.CartesianY, KeyTime.FromPercent((double)i / being.NCellsWalk)));
                        xWalk.KeyFrames.Add(new DiscreteDoubleKeyFrame((double)30 * cell.CartesianX, KeyTime.FromPercent((double)i / being.NCellsWalk)));
                        yWalk.KeyFrames.Add(new DiscreteDoubleKeyFrame((double)30 * cell.CartesianY, KeyTime.FromPercent((double)i / being.NCellsWalk)));
                    }
                }

                Transform translate = beingShapes[being.ID].RenderTransform;
                translate.BeginAnimation(TranslateTransform.XProperty, xWalk);
                translate.BeginAnimation(TranslateTransform.YProperty, yWalk);
            }
        }
        public void ChangeCell(Thing thing)
        {
            terrainPolygons[thing.Location.X][thing.Location.Y].Fill = TerrainBrushes[thing.Type];
        }

        private ThingType currentType = ThingType.Earth;

        public void changeBrush(string name)
        {
            this.currentType = (ThingType)Enum.Parse(typeof(ThingType), name, true);
        }

        void cellClick(object sender, RoutedEventArgs e)
        {
            Polygon poly = sender as Polygon;
            var loc = (GridPoint)poly.Tag;
            var thing = Simulation.Instance.Terrain[loc.X][loc.Y];
            thing.ChangeType(currentType, null);
            thing.Apply();
        }

        void beingClick(object sender, RoutedEventArgs e)
        {
            Polygon poly = sender as Polygon;
            var id = (int)poly.Tag;
            var being = Simulation.Instance.Population[id];
            window.mainGrid.ColumnDefinitions[0].Width = new GridLength(window.sidePanel.Width);
        }

        public void DeselectAll()
        {

        }


        static public GridPoint Cycle(GridPoint pt)
        {
            return new GridPoint(pt.X.Cycle(GRID_WIDTH), pt.Y.Cycle(GRID_HEIGHT));
        }
    }
}
