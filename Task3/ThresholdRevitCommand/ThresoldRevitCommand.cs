using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Windows.Controls;
using System.Windows.Shapes;
using Line = Autodesk.Revit.DB.Line;
using System.Reflection.Emit;
using System.Net;

namespace Task3.ThresholdRevitCommand
{
    [Transaction(TransactionMode.Manual)]
    public class ThresoldRevitCommand : IExternalCommand
    {
        public static IList<CurveLoop> GetDoorBoundary(FamilyInstance door)
        {
            Options options = new Options();
            GeometryElement geometryElement = door.get_Geometry(options);
            var doorBoundingBox = geometryElement.GetBoundingBox();
            XYZ minPoint = doorBoundingBox.Min;
            XYZ maxPoint = doorBoundingBox.Max;
            Parameter doorHeight = door.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM);

            double height = doorHeight.AsDouble();
            List<XYZ> cornerPoints = new List<XYZ>
            {
                new XYZ(minPoint.X, minPoint.Y, minPoint.Z - height),
                new XYZ(maxPoint.X, minPoint.Y, minPoint.Z - height),
                new XYZ(maxPoint.X, maxPoint.Y, minPoint.Z - height),
                new XYZ(minPoint.X, maxPoint.Y, minPoint.Z - height)

            };

            List<Line> lines = new List<Line>();
            for (int i = 0; i < cornerPoints.Count; i++)
            {
                int j = (i + 1) % cornerPoints.Count;
                Line line = Line.CreateBound(cornerPoints[i], cornerPoints[j]);
                lines.Add(line);
            }

            CurveLoop curveLoop = new CurveLoop();
            foreach (Line line in lines)
            {
                curveLoop.Append(line);
            }

            return new List<CurveLoop>() { curveLoop };
        }

        public void CreateFloorBoundary(Document doc, Room room)
        {
            IList<IList<BoundarySegment>> roomBoundarySegments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
            IList<CurveLoop> roomBoundaryLoops = new List<CurveLoop>();
            foreach (IList<BoundarySegment> boundarySegments in roomBoundarySegments)
            {
                CurveLoop curveLoop = new CurveLoop();
                foreach (BoundarySegment boundarySegment in boundarySegments)
                {
                    Curve curve = boundarySegment.GetCurve();
                    curveLoop.Append(curve);
                }
                roomBoundaryLoops.Add(curveLoop);
            }
            List<CurveLoop> doorBoundaryLoops = new List<CurveLoop>();
            FilteredElementCollector doorCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType();

            foreach (FamilyInstance doorInstance in doorCollector)
            {

                if (doorInstance.Room != null && doorInstance.Room.Id == room.Id)
                {
                    Wall hostWall = doc.GetElement(doorInstance.Host.Id) as Wall;
                    if (hostWall != null)
                    {
                        IList<CurveLoop> doorBoundaryLoop = GetDoorBoundary(doorInstance);
                        doorBoundaryLoops.AddRange(doorBoundaryLoop);
                    }
                }
            }

            using (Transaction tx = new Transaction(doc, "Create Floor Boundary"))
            {
                tx.Start();
                var level = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().Cast<Level>().FirstOrDefault();
                FloorType FloorType = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Floors).Cast<FloorType>().FirstOrDefault();
                if (roomBoundaryLoops.Count > 0)
                {
                    Floor f1 = Floor.Create(doc, roomBoundaryLoops, FloorType.Id, level.Id);
                    if (doorBoundaryLoops.Count > 0)
                    {
                        Floor f2 = Floor.Create(doc, doorBoundaryLoops, FloorType.Id, level.Id);
                        Options options = new Options();
                        options.ComputeReferences = true;
                        options.DetailLevel = ViewDetailLevel.Fine;

                        #region Joining the solids 

                         
                        GeometryElement geom1 = f1.get_Geometry(options);
                        GeometryElement geom2 = f2.get_Geometry(options);
                        if (geom2 != null && geom1 != null)
                        {
                            List<Solid> solids1 = new List<Solid>(geom1.GetEnumerator() as IEnumerable<Solid>);
                            List<Solid> solids2 = new List<Solid>(geom2.GetEnumerator() as IEnumerable<Solid>);

                            if (!JoinGeometryUtils.AreElementsJoined(doc, doc.GetElement(f1.Id), doc.GetElement(f2.Id)))
                            {
                                JoinGeometryUtils.JoinGeometry(doc, doc.GetElement(f1.Id), doc.GetElement(f2.Id));
                            }


                            Solid finalSolid = null;
                            Face bottomFace = null;
                            double minZ = double.MaxValue;
                            foreach (Solid solid1 in solids1)
                            {
                                foreach (Face face in solid1.Faces)
                                {
                                    BoundingBoxUV bbox = face.GetBoundingBox();
                                    UV center = (bbox.Min + bbox.Max) / 2;
                                    XYZ point = face.Evaluate(center);
                                    if (point.Z < minZ)
                                    {
                                        minZ = point.Z;
                                        bottomFace = face;
                                        finalSolid = solid1;
                                    }
                                }
                            }

                            IList<CurveLoop> boundary = bottomFace.GetEdgesAsCurveLoops();
                            Floor floor3 = Floor.Create(doc, boundary, FloorType.Id, level.Id);
                            ElementId id1 = f1.Id;
                            ElementId id2 = f2.Id;
                            doc.Delete(new List<ElementId>() { id1, id2 });


                        }

                        #endregion


                    }
                }
                tx.Commit();
            }
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UIDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UIDoc.Document;
            try
            {
                List<Room> rooms = new FilteredElementCollector(Doc)
                    .OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().Cast<Room>().ToList();

                foreach (Room room in rooms)
                {
                    CreateFloorBoundary(Doc, room);
                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}


