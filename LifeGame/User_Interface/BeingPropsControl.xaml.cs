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
    /// Logica di interazione per BeingPropsControl.xaml
    /// </summary>
    public partial class BeingPropsControl : UserControl
    {
        public BeingPropsControl()
        {
            InitializeComponent();
        }

        public void InitStats(Being being)
        {
            sex.Text = (being.Brain.State[1] > 0 ? "Maschio" : "Femmina");
            nNodes.Text = being.Brain.State.Length.ToString();
            nLinks.Text = being.Brain.Links.Length.ToString();
        }

        public void UpdateStats(Being being)
        {
            energy.Text = (being.Properties[(ThingProperty)BeingMutableProp.Energy] / 10000f * 100f).ToString("0.0") + "%";
            health.Text = (being.Properties[(ThingProperty)BeingMutableProp.Health] * 100f).ToString("0.0") + "%";
            integrity.Text = (being.Properties[(ThingProperty)BeingMutableProp.Integrity] * 100f).ToString("0.0") + "%";
            hunger.Text = (being.Properties[(ThingProperty)BeingMutableProp.Hunger] * 100f).ToString("0.0") + "%";
            thirst.Text = (being.Properties[(ThingProperty)BeingMutableProp.Thirst] * 100f).ToString("0.0") + "%";
        }
    }
}
