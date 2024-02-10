using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Axelerate.Model
{
    public class RoomUtility
    {

        public static Line getWallPoint(Wall wall)
        {

            // Load the family symbol you want to place
            FamilySymbol familySymbol = Application.RoomCommand.Doc.GetElement(Application.RoomCommand.wallRef.ElementId) as FamilySymbol;

            // Get the wall element
             wall = Application.RoomCommand.Doc.GetElement(Application.RoomCommand.wallRef.ElementId) as Wall;
            //Element wall = Application.RoomCommand.Doc.GetElement(Application.RoomCommand.wallRef.ElementId) ;


            // Get the location point of the wall
            LocationCurve locationPoint = wall.Location as LocationCurve;
            Line wallLine = locationPoint.Curve as Line;
            
            return wallLine;

        }

        public static XYZ getDoorPoint ()
        {
            FilteredElementCollector doorCollector = new FilteredElementCollector(Application.RoomCommand.Doc)
           .OfCategory(BuiltInCategory.OST_Doors)
           .OfClass(typeof(FamilyInstance));

            Element doorElement = doorCollector.ToElements().FirstOrDefault();
            LocationPoint doorLoc = doorElement.Location as LocationPoint;
            XYZ doorPoint = doorLoc.Point;
            return doorPoint;
        }

            public static XYZ getWallDoorDistance(Line wallLine, XYZ doorPoint)
        {
            XYZ wallStartPoint = wallLine.GetEndPoint(0);
            XYZ wallEndPoint = wallLine.GetEndPoint(1);
            XYZ pointXYZ = null;

            double distanceToStartPoint = doorPoint.DistanceTo(wallStartPoint);
            double distanceToEndPoint = doorPoint.DistanceTo(wallEndPoint);
            Line famLine = null;

            if (distanceToStartPoint >= distanceToEndPoint)
            {
                //pointXYZ = wallStartPoint;
                pointXYZ = new XYZ(wallStartPoint.X , wallStartPoint.Y + 3, wallStartPoint.Z);

                ;
            }
            else
            {
                //pointXYZ = wallEndPoint;
                pointXYZ = new XYZ(wallEndPoint.X , wallEndPoint.Y + 3, wallEndPoint.Z);

            }
            return pointXYZ;
        }

        public static void addFamily( XYZ pointXYZ, XYZ doorXYZ, Element wall)
        {
            // Specify the name of the family symbol you want to find
            string symbolName = "M_Toilet-Domestic-2D";

            // Create a filtered element collector to search for family symbols
            FilteredElementCollector collector = new FilteredElementCollector(Application.RoomCommand.Doc);
            ICollection<Element> symbols = collector.OfClass(typeof(FamilySymbol)).ToElements();

            // Find the family symbol with the specified name
            FamilySymbol symbol = symbols.FirstOrDefault(s => s.Name.Equals(symbolName)) as FamilySymbol;

            // Get the level of the family instance
            Level level = Application.RoomCommand.Doc.GetElement(wall.LevelId) as Level;

            if (symbol != null)
            {
                using (Transaction trans = new Transaction(Application.RoomCommand.Doc, "Change location"))
                {


                    trans.Start();

                    // Create a new instance of the family on the wall
                    FamilyInstance familyInstance1 = Application.RoomCommand.Doc.Create.NewFamilyInstance(pointXYZ, symbol, wall, level, StructuralType.NonStructural);
                    FamilyInstance familyInstance2 = Application.RoomCommand.Doc.Create.NewFamilyInstance(pointXYZ, symbol, wall, level, StructuralType.NonStructural);
                    familyInstance2.flipFacing();

                    XYZ familyInstance1Point = (familyInstance1.Location as LocationPoint) .Point;
                    XYZ familyInstance2Point = (familyInstance1.Location as LocationPoint).Point;
                    

                    double distance1 = familyInstance1Point.DistanceTo(doorXYZ);
                    double distance2 = familyInstance2Point.DistanceTo(doorXYZ);

                    if (distance1 < distance2)
                    {
                        
                        ElementId familyInstance2Id = familyInstance1.Id;
                        Application.RoomCommand.Doc.Delete(familyInstance2Id);
                    }
                    else
                    {
                        ElementId familyInstance1Id = familyInstance1.Id;
                        Application.RoomCommand.Doc.Delete(familyInstance1Id);
                    }


                    trans.Commit();


                }
            }
            else
            {
                TaskDialog.Show("Message", "Load this family type (M_Toilet-Domestic-2D)");
            }
        }

        #region how to know is the wall is vertical or horizontal revit api c#
        //// Determine if the wall is vertical or horizontal
        //bool isVertical = Math.Abs(wallCurve.Direction.Z) > 0.9; // Adjust the threshold as needed
        //bool isHorizontal = !isVertical;
        #endregion

        //public static void getWallFaces(Wall wall)
        //{
        //    // Get the geometry element of the wall
        //    GeometryElement geometryElement = wall.get_Geometry(new Options());

        //    // List to store the wall faces
        //    List<Face> wallFaces = new List<Face>();

        //    // Iterate over the geometry element to find the wall faces
        //    foreach (GeometryObject geometryObject in geometryElement)
        //    {
        //        if (geometryObject is Solid solid)
        //        {
        //            foreach (Face face in solid.Faces)
        //            {
        //                wallFaces.Add(face);
        //            }
        //        }
        //    }

        //    // Sort the wall faces by area in descending order
        //    wallFaces.Sort((a, b) => b.Area.CompareTo(a.Area));

        //    // Get the origins of the two largest faces
        //    if (wallFaces.Count == 1)
        //    {
        //        double origin1X = wallFaces[0].Evaluate(new UV()).X;
        //        double origin1Y = wallFaces[0].Evaluate(new UV()).Y;
        //        XYZ origin1 = new XYZ(origin1X, origin1Y, 0);
        //        double origin2X = wallFaces[0].Evaluate(new UV()).X;
        //        double origin2Y = wallFaces[0].Evaluate(new UV()).Y;
        //        XYZ origin2 = new XYZ(origin2X, origin2Y, 0);

        //        // Print the origins of the two largest faces
        //        TaskDialog.Show("Face Origins", $"Origin 1: {origin1}\nOrigin 2: {origin2}");
        //    }
        //    else
        //    {
        //        TaskDialog.Show("Error", "The wall does not have enough faces.");
        //    }

        //}

        //public static void getRoom (Element wallElement)
        //{
        //    SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
        //    options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center;

        //    IEnumerable<IList<BoundarySegment>> boundarySegments = wallElement.GetBoundarySegments(options);

        //    // Assuming the wall has only one room associated with it
        //    IList<BoundarySegment> segments = boundarySegments.FirstOrDefault();

        //    if (segments != null)
        //    {
        //        ElementId roomId = segments[0].ElementId;
        //        //Room room = Application.RoomCommand.Doc.GetElement(roomId) as Room;

        //        //if (room != null)
        //        //{
        //        //    // The 'room' variable now contains the room associated with the selected wall
        //        //}
        //    }
        //}




    }
}
