using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Axelerate.Model;
using Autodesk.Revit.DB.Structure;

namespace Task1.Model
{
    public class RoomExtensions
    {
        public static List<Room> GetAllRoomGeometry(string roomName)
        {
            List<Room> rooms = new List<Room>();
            FilteredElementCollector collector = new FilteredElementCollector(Application.RoomCommand.Doc);
            ICollection<Element> roomElements = collector.OfClass(typeof(SpatialElement)).ToElements();

            foreach (Element element in roomElements)
            {
                SpatialElement room = element as SpatialElement;
                if (room != null && room is Room)
                {
                    Room r = room as Room;
                    if (r.Name.StartsWith(roomName))
                    {
                        rooms.Add(r);
                    }
                }
            }
            return rooms;
        }
        public static Room GetNearestRoomWall(List<Room> rooms, XYZ point)
        {
            List<RoomGeometry> roomDistances = new List<RoomGeometry>();

            // Calculate distance from the given point to each room
            foreach (Room room in rooms)
            {
                LocationPoint roomLocation = room.Location as LocationPoint;
                if (roomLocation != null)
                {
                    double distance = roomLocation.Point.DistanceTo(point);
                    roomDistances.Add(new RoomGeometry { Room = room, Distance = distance });
                }
            }

            // Find the room with the shortest distance
            Room nearestRoom = null;
            double minDistance = double.MaxValue;
            foreach (RoomGeometry roomDistance in roomDistances)
            {
                if (roomDistance.Distance < minDistance)
                {
                    minDistance = roomDistance.Distance;
                    nearestRoom = roomDistance.Room;
                }
            }

            return nearestRoom;
        }

        public static XYZ getDoorPoint(Room doorRoom)
        {
            FilteredElementCollector doorCollector = new FilteredElementCollector(Application.RoomCommand.Doc)
           .OfCategory(BuiltInCategory.OST_Doors)
           .OfClass(typeof(FamilyInstance));


            Element doorElement = null;
            foreach (FamilyInstance door in doorCollector)
            {
                if (door.FromRoom != null)
                {
                    if (door.ToRoom.Name == doorRoom.Name)
                    {
                        doorElement = door;
                        break;
                    }
                    else if (door.FromRoom.Name == doorRoom.Name)
                    {
                        doorElement = door;

                        break;
                    }
                }
            }

            LocationPoint doorLoc = doorElement.Location as LocationPoint;
            XYZ doorPoint = doorLoc.Point;
            return doorPoint;
        }

        public static List<XYZ> GetRoomPoints(Room NearestRoom)
        {
            GeometryElement room = NearestRoom.ClosedShell;
            var roomBoundingBox = room.GetBoundingBox();

            // Get the minimum and maximum extents of the bounding box
            XYZ minPoint = roomBoundingBox.Min;
            XYZ maxPoint = roomBoundingBox.Max;

            // Calculate the four corner points of the room
            List<XYZ> cornerPoints = new List<XYZ>
            {
                new XYZ(minPoint.X, minPoint.Y, minPoint.Z),
                new XYZ(maxPoint.X, minPoint.Y, minPoint.Z),
                new XYZ(maxPoint.X, maxPoint.Y, minPoint.Z),
                new XYZ(minPoint.X, maxPoint.Y, minPoint.Z)

            };


            return cornerPoints;

        }

        public static XYZ FindMaxPoint(List<XYZ> points, XYZ referencePoint)
        {
            if (points.Count == 0)
                return null;

            double maxDistance = double.MinValue;
            XYZ maxPoint = null;

            foreach (XYZ point in points)
            {
                double distance = point.DistanceTo(referencePoint);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxPoint = point;
                }
            }

            return maxPoint;
        }

        public static void addFamily(XYZ pointXYZ, Element wall)
        {
            // Specify the name of the family symbol you want to find
            string symbolName = "ADA";

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
                    if (!symbol.IsActive)
                    {
                        symbol.Activate();
                    }
                    FamilyInstance familyInstance1 = Application.RoomCommand.Doc.Create.NewFamilyInstance(pointXYZ, symbol, wall, level, StructuralType.NonStructural);
                    FamilyInstance familyInstance2 = Application.RoomCommand.Doc.Create.NewFamilyInstance(pointXYZ, symbol, wall, level, StructuralType.NonStructural);
                    familyInstance2.flipFacing();

                    FamilyInstance familyInstance = deleteFamily(familyInstance1, familyInstance2);
                    Location location = familyInstance.Location;

                    // Check if the Location is a LocationPoint (e.g., for furniture, equipment, etc.)
                    if (location is LocationPoint locationPoint)
                    {
                        // Set the Elevation property of the LocationPoint to zero
                        XYZ position = locationPoint.Point;
                        position = new XYZ(position.X, position.Y + 3.5, 0.0);
                        locationPoint.Point = position;
                    }


                    trans.Commit();


                }
            }
            else
            {
                TaskDialog.Show("Message", "Load this family type (ADA)");
            }
        }

        public static FamilyInstance deleteFamily(FamilyInstance familyInstance1, FamilyInstance familyInstance2)
        {
            FamilyInstance familyInstance = null;
            

            if (familyInstance1.Room.Name.StartsWith("Bathroom"))
            {
                ElementId familyInstance2Id = familyInstance2.Id;
                Application.RoomCommand.Doc.Delete(familyInstance2.Id);
                familyInstance = familyInstance1;
            }
            else if (familyInstance2.Room.Name.StartsWith("Bathroom"))
            {
                ElementId familyInstance2Id = familyInstance1.Id;
                Application.RoomCommand.Doc.Delete(familyInstance1.Id);
                familyInstance = familyInstance2;

            }
            return familyInstance;

        }
       
        public List<Element> GetDoorsInRoom(Room room, Document doc)
        {
            List<Element> doorsInRoom = new List<Element>();

            // Get the elements within the room's bounding box
            BoundingBoxXYZ roomBoundingBox = room.get_BoundingBox(null);
            Outline outline = new Outline(roomBoundingBox.Min, roomBoundingBox.Max);
            BoundingBoxIntersectsFilter bbFilter = new BoundingBoxIntersectsFilter(outline);

            // Filter for doors
            ElementCategoryFilter doorCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);

            // Create a logical AND filter to combine the room bounding box filter and the door category filter
            LogicalAndFilter logicalAndFilter = new LogicalAndFilter(bbFilter, doorCategoryFilter);

            // Get the elements in the room that are doors
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<Element> elementsInRoom = collector.WherePasses(logicalAndFilter).ToElements().ToList();

            // Check if each element is a door and add it to the list if it is
            foreach (Element element in elementsInRoom)
            {
                if (element is FamilyInstance door)
                {
                    doorsInRoom.Add(door);
                }
            }

            return doorsInRoom;
        }

    }
}
