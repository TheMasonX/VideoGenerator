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
using VideoGenerator.Models;
using VideoGenerator.ViewModels;

namespace VideoGenerator.Views
{
    /// <summary>
    /// Interaction logic for FileGrid.xaml
    /// </summary>
    public partial class FileGrid : UserControl
    {
        public FileGrid ()
        {
            InitializeComponent();
        }

        private void filesGrid_UnloadingRow (object sender, DataGridRowEventArgs e)
        {
            if(e.Row.Item is IDisposable data) data.Dispose();
            if (DataContext is FileGridVM vm) vm.RefreshCounts();
        }
    }
}
