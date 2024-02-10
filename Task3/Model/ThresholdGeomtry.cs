using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4.Model
{
    public class ThresholdGeomtry
    {
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

        public void JoinIntersectedFloors(Document doc, Floor floor1, Floor floor2)
        {
            // Get the geometry of the intersected floors
            GeometryElement geometry1 = floor1.get_Geometry(new Options());
            GeometryElement geometry2 = floor2.get_Geometry(new Options());

            // Create a list to hold all floor boundary curves
            List<Curve> floorCurves = new List<Curve>();

            // Add curves from the first floor
            foreach (GeometryObject obj in geometry1)
            {
                Solid solid = obj as Solid;
                if (solid != null)
                {
                    foreach (Face face in solid.Faces)
                    {
                        floorCurves.AddRange(face.GetEdgesAsCurveLoops()[0]);
                    }
                }
            }

            // Add curves from the second floor
            foreach (GeometryObject obj in geometry2)
            {
                Solid solid = obj as Solid;
                if (solid != null)
                {
                    foreach (Face face in solid.Faces)
                    {
                        floorCurves.AddRange(face.GetEdgesAsCurveLoops()[0]);
                    }
                }
            }
        }


        #region solution

        #endregion
    }
}
