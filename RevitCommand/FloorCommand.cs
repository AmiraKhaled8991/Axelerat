using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Axelerate.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Axelerate.RevitCommand
{
    [Transaction(TransactionMode.Manual)]

    public class FloorCommand : IExternalCommand
    {
        public static UIDocument UiDoc { get; set; }
        public static Document Doc { get; set; }
        public Result Execute(ExternalCommandData commandData,ref string message, ElementSet elements)
        {
            UiDoc = commandData.Application.ActiveUIDocument;

            Doc = UiDoc.Document;

            try
            {

                MainWindowFloor MW = new MainWindowFloor();

                MW.ShowDialog();

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
