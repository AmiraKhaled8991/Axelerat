//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI.Selection;
//using Autodesk.Revit.UI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Task3.ThresholdRevitCommand
//{
//    [Transaction(TransactionMode.Manual)]

//    public class ThresholdCommand : IExternalCommand
//    {
//        public static UIDocument UiDoc { get; set; }
//        public static Document Doc { get; set; }

//        public static Reference wallRef { get; set; }
//        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//        {
//            UiDoc = commandData.Application.ActiveUIDocument;

//            Doc = UiDoc.Document;

//            // Assuming you have an active Revit document and a selected wall
//            wallRef = UiDoc.Selection.PickObject(ObjectType.Element, "Select a wall");

//            try
//            {


//                return Result.Succeeded;
//            }
//            catch (Exception ex)
//            {

//                message = ex.Message;

//                return Result.Failed;
//            }
//        }
//    }
//}
