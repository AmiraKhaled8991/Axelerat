using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Axelerate.Model;
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
using System.Xml.Linq;
using Line = Autodesk.Revit.DB.Line;
using System.Windows.Media;
using Task1.Model;

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

            AddFamilyCommand = new MyCommand(AddFamilyCommandMethod, CanAddFamilyCommand);


        }

        #endregion

        #region methods
        public static void AddFamilyCommandMethod(object parameter)
        {

            

            // Get the wall element
            Wall wall = Application.RoomCommand.Doc.GetElement(Application.RoomCommand.wallRef.ElementId) as Wall;
            Element wallElement = Application.RoomCommand.Doc.GetElement(Application.RoomCommand.wallRef);
            LocationCurve locationPoint = wall.Location as LocationCurve;
            Line wallLine = locationPoint.Curve as Line;
            XYZ wallLocation = wallLine.Origin;
            //LocationPoint locationPoint = wallElement.Location as LocationPoint;

            

            string roomName = "Bathroom";
            List<Room> roomList = RoomExtensions.GetAllRoomGeometry(roomName);

            Room NearestRoom = RoomExtensions.GetNearestRoomWall(roomList, wallLocation);

            XYZ doorPoint = RoomExtensions.getDoorPoint(NearestRoom);

            List<XYZ> familyPoints = RoomExtensions.GetRoomPoints( NearestRoom);

            XYZ familyPoint = RoomExtensions.FindMaxPoint(familyPoints, doorPoint);

            RoomExtensions.addFamily(familyPoint,  wallElement);


        }

        public bool CanAddFamilyCommand(object parameter)
        {

            return true;
        }


       

        #endregion

    }
}