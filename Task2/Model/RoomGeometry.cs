using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Axelerate.Model
{
    public class RoomGeometry
    {
        public XYZ UpperLeft { get; set; }
        public XYZ UpperRight { get; set; }
        public XYZ LowerLeft { get; set; }
        public XYZ LowerRight { get; set; }
        public Room Room { get; set; }
        public double Distance { get; set; }

        
    }

    
}
