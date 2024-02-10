using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Axelerate.Application;
using Axelerate.Application;
using Axelerate.Command;
using Axelerate.RevitCommand;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Line = Autodesk.Revit.DB.Line;

namespace Axelerate.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        #region fields

        List<XYZ> Points = new List<XYZ>();
        List<Line> Lines = new List<Line>();

        FloorType floorType = null;

        IList<Element> floors = null;

        ElementId fFs = null;

        private string _selectedFloorType;
        public static ObservableCollection<string> revitFloorList { get; set; }


        #endregion

        #region probs  

        public string selectedFloorType
        {
            get { return _selectedFloorType; }
            set
            {
                _selectedFloorType = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region commands
        public MyCommand CreateCommand { get; set; }
        #endregion

        #region constructor
        public MainWindowViewModel()

        {

            FloorData();
            CreateCommand = new MyCommand(CreateCommandMethod, CanCreateCommand);


        }
        #endregion

        #region methods
        public void FloorData()
        {
            Points.Add(new XYZ(0, 0, 0));
            Points.Add(new XYZ(79, 0, 0));
            Points.Add(new XYZ(44, 25, 0));
            Points.Add(new XYZ(13, 25, 0));
            Points.Add(new XYZ(13, 40, 0));
            Points.Add(new XYZ(-8,40 , 0));
            Points.Add(new XYZ(55, 34, 0));
            Points.Add(new XYZ(55, 10, 0));
            Points.Add(new XYZ(79,34 , 0));
            Points.Add(new XYZ(55, 34, 0));
            Points.Add(new XYZ(0, 20, 0));
            Points.Add(new XYZ(0, 0, 0));
            Points.Add(new XYZ(55,10 , 0));
            Points.Add(new XYZ(44,12 , 0));
            Points.Add(new XYZ(-8,40 , 0));
            Points.Add(new XYZ(-8,20 , 0));
            Points.Add(new XYZ(79, 0, 0));
            Points.Add(new XYZ(79, 34, 0));
            Points.Add(new XYZ(44, 12, 0));
            Points.Add(new XYZ(44, 25, 0));
            Points.Add(new XYZ(-8, 20, 0));
            Points.Add(new XYZ(0, 20, 0));
            Points.Add(new XYZ(13,25 , 0));
            Points.Add(new XYZ(13,40 , 0));

            for (int i = 0; i< Points.Count-1;i += 2)
            {
                Line line = Line.CreateBound(Points[i], Points[i + 1]);
                Lines.Append(line);
            }

            floors = new FilteredElementCollector(FloorCommand.Doc).OfCategory(BuiltInCategory.OST_Floors)
                      .WhereElementIsElementType().ToElements();

            revitFloorList = new ObservableCollection<string>();

            foreach (Element ele in floors)
            {
                revitFloorList.Add(ele.Name);
            }
        }

        static bool isEqualPoint(XYZ p1, XYZ p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }
        static void bubbleSort(List<Line> Lines)
        {

            int i, j;
            Line temp;
            bool swapped;
            for (i = 0; i < Lines.Count - 1; i++)
            {
                
                if (!isEqualPoint(Lines[i].GetEndPoint(1), Lines[i + 1].GetEndPoint(0)))
                {
                    swapped = false;
                    for (j = i + 2; j < Lines.Count; j++)
                    {
                        if (isEqualPoint(Lines[i].GetEndPoint(1), Lines[j].GetEndPoint(0)))
                        {
                            // Swap arr[j] and arr[j+1]
                            temp = Lines[i + 1];
                            Lines[i + 1] = Lines[j];
                            Lines[j] = temp;
                            swapped = true;
                            break;
                        }
                    }

                    // If no two elements were
                    // swapped by inner loop, then break
                    if (swapped == false)
                    { 
                        TaskDialog.Show("Message", "The Lines aren't in curve loop");
                        break;
                    }
                }
            }
        }


        public void CreateCommandMethod (object parameter)
        {


            foreach (Element fEle in floors)
            {
                if (fEle.Name == selectedFloorType)
                {
                    fFs = fEle.GetTypeId();
                    floorType = fEle as FloorType;
                }
            }
            for (int i = 0; i < Points.Count-1; i += 2)
            {
                Line line = Line.CreateBound(Points[i], Points[i + 1]);
                Lines.Add(line);


            }
            bubbleSort(Lines);

            IList<Curve> floorCurves = new List<Curve> () ;
            CurveLoop floorCurveLoopes = new CurveLoop ();

            foreach (Line line in Lines)
            {
                Curve curve = line;
                floorCurves.Add(curve);
            }
            floorCurveLoopes = CurveLoop.Create(floorCurves);
            IList<CurveLoop> floorCurveLoop = new List<CurveLoop>() ;
            floorCurveLoop.Add(floorCurveLoopes);

            using (Transaction t = new Transaction(FloorCommand.Doc, "Create Floor"))
            {
                t.Start();

                try
                {
                    IList<Level> levels = new FilteredElementCollector(FloorCommand.Doc).OfClass(typeof(Level)).Cast<Level>().ToList();

                    Level zeroLevel = null;
                    foreach (Level level in levels)
                    {
                        if (level.Name == "Level 1")
                        {
                            zeroLevel = level;
                        }
                    }
                    //Floor floor = FloorCommand.Doc.Create.NewFloor(floorCurves, floorType, zeroLevel, false);
                    Floor floor = Floor.Create(FloorCommand.Doc, floorCurveLoop, floorType.Id, zeroLevel.Id);


                    Options options = new Options();
                    Element floorElement = floor;
                    var geom1 = floorElement.get_Geometry(options);


                }
                catch (Exception ex)
                {
                    TaskDialog.Show(ex.Message, ex.ToString());
                }

                t.Commit();



            }
        }
        public bool CanCreateCommand(object parameter)
        {

            return true;
        }
        #endregion

    }
}
