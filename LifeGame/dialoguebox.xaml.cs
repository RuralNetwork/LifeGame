using System;
using System.Collections.Generic;
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

namespace LifeGame
{
    /// <summary>
    /// Logica di interazione per dialoguebox.xaml
    /// </summary>
    public partial class dialoguebox : UserControl
    {
        /// <summary>
        /// Shows a message or makes the user choose
        /// </summary>
        /// <param name="text">String Required Default:Sei sicuro?</param>
        /// <param name="type">Bool true=ok/annulla false=fade</param>
        /// <param name="life">int how long should it survive</param>
        public dialoguebox(string text="Sei sicuro?",bool type=true /*true=ok/annulla--false=fade*/,int life=7,string color="#FFFFFF")
        {
            InitializeComponent();
            if(type){
                //show ok/annulla
            }
            message.Text = text;
            Color Color = (Color)ColorConverter.ConvertFromString(color);
            background.Fill = new SolidColorBrush(Color);
            
        }
    }
}
