using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Axelerate.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Line = Autodesk.Revit.DB.Line;

namespace Axelerate.ViewModel
{
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {


            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyname));

        }

        #region commands
        public MyCommand AddFamilyCommand { get; set; }
        #endregion

        #region constructor
        public MainWindowViewModel()
        {



        }

        #endregion

        #region methods
        
        public bool CanAddFamilyCommand(object parameter)
        {

            return true;
        }

        
        #endregion

    }
}
