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
using System.Security.Cryptography;
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
        #endregion

        #region constructor
        public MainWindowViewModel()
        {

        }

        #endregion

        #region methods
        public static List<Line> GetVerticalLinesOfFraming(Wall wall,double distance=15)
        {
            List <Line> lines = new List<Line> ();

            LocationCurve wallCurve = wall.Location as LocationCurve;
            Line wallLine = wallCurve.Curve as Line;
            XYZ startPoint = wallLine.Evaluate(0.5, true); // Get the midpoint of the curve

              


            // Get the base and top constraints of the wall
            Level baseLevel = wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId() == ElementId.InvalidElementId ?
                null : wall.Document.GetElement(wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId()) as Level;
            Level topLevel = wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId() == ElementId.InvalidElementId ?
                null : wall.Document.GetElement(wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId()) as Level;

            if (baseLevel == null || topLevel == null)
            {
                // Not a valid wall with base and top constraints
                return lines;
            }


            XYZ rightPoint = new XYZ();
            XYZ leftPoint = new XYZ();

            for (int i = 0; i < 15; i+=5)
            {
                if (i == 0)
                {
                    rightPoint = startPoint * i;
                    leftPoint = startPoint * i;

                }
                else
                {
                    rightPoint += rightPoint;
                    leftPoint += leftPoint;
                }
            }
        
            


            // Create lines extending in both directions from the center of the wall
            XYZ topPoint = new XYZ(startPoint.X, startPoint.Y, topLevel.Elevation);
            XYZ basePoint = new XYZ(startPoint.X, startPoint.Y, baseLevel.Elevation);
            XYZ offsetVector = new XYZ(distance, 0, 0); // Offset vector in X direction

            Line line1 = Line.CreateBound(topPoint, topPoint + offsetVector);
            Line line2 = Line.CreateBound(topPoint, topPoint - offsetVector);
            Line line3 = Line.CreateBound(basePoint, basePoint + offsetVector);
            Line line4 = Line.CreateBound(basePoint, basePoint - offsetVector);

            lines.Add(line1);
            lines.Add(line2);
            lines.Add(line3);
            lines.Add(line4);

            return lines;
        }


        public static void AddFamilyCommandMethod(object parameter)
        {

        }

            public bool CanCreateCommand(object parameter)
        {

            return true;
        }




        #endregion

    }
}
