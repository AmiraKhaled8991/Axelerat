using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Axelerate.Model;
using RevitAPIFinal.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Line = Autodesk.Revit.DB.Line;

namespace Axelerate.ViewModel
{
    public class RoomMainWindowViewModel:BaseClass
    {

        #region commands
        public MyCommand AddFamilyCommand { get; set; }
        #endregion

        #region constructor
        public RoomMainWindowViewModel()
        {

            AddFamilyCommand = new MyCommand(AddFamilyCommandMethod, CanAddFamilyCommand);


        }
        #endregion

        #region methods
        public static void AddFamilyCommandMethod(object parameter)
        {

            // Load the family symbol you want to place
            FamilySymbol familySymbol = Application.RoomCommand.Doc.GetElement(Application.RoomCommand.wallRef.ElementId) as FamilySymbol;

            // Get the wall element
            Wall wall = Application.RoomCommand.Doc.GetElement(Application.RoomCommand.wallRef.ElementId) as Wall;
            Element wallElement = Application.RoomCommand.Doc.GetElement(Application.RoomCommand.wallRef);


           Line wallLine = RoomUtility.getWallPoint(wall);

            XYZ doorPoint = RoomUtility.getDoorPoint();

            XYZ WallDoorPoint = RoomUtility.getWallDoorDistance(wallLine, doorPoint);

            RoomUtility.addFamily( WallDoorPoint,doorPoint, wallElement);








        }

        public bool CanAddFamilyCommand(object parameter)
        {

            return true;
        }

        
        #endregion

    }
}
