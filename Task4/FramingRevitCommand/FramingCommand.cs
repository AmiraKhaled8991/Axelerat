using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task4;

namespace Axelerate.Application
{
    [Transaction(TransactionMode.Manual)]

    public class FramingCommand : IExternalCommand
    {
        public static UIDocument UiDoc { get; set; }
        public static Document Doc { get; set; }

        public static Reference wallRef { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UiDoc = commandData.Application.ActiveUIDocument;

            Doc = UiDoc.Document;

            try
            {


                return Result.Succeeded;
            }
            catch (Exception ex)
            {

                message = ex.Message;

                return Result.Failed;
            }
        }
    }
}
