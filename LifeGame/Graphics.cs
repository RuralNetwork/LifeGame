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

            //Adds the polygon to the canvas
            this._canvas.Children.Add(poligono);
        }
        public void startSimulation()
        {
            Debug.Write("Simulation started\n");
        }
    }
}
