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

namespace MaryaWPF.Views
{
    /// <summary>
    /// Logique d'interaction pour CategoryDisplayView.xaml
    /// </summary>
    public partial class CategoryDisplayView : UserControl
    {
        public CategoryDisplayView()
        {
            InitializeComponent();
        }

        /*
        private void Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Services.SelectionChanged -= Services_SelectionChanged;
            Services.SelectedItem = null;
            Categories.SelectionChanged += Categories_SelectionChanged;
        }

        private void Services_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Categories.SelectionChanged -= Categories_SelectionChanged;
            Categories.SelectedItem = null;
            Services.SelectionChanged += Services_SelectionChanged;

        } */
    }
}
