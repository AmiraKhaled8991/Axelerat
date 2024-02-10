using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Axelerate.Application
{

    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class ExternalApp : IExternalApplication
    {

        const string Ribbon_Tab = "Axelerate";

        const string Ribbon_Panel1 = "Tasks";
        


        public Result OnStartup(UIControlledApplication application)
        {
            #region Rubbin Tab
            ////////////////////////// Create Ribbon Tab ///////////////////////////////////////////
            application.CreateRibbonTab(Ribbon_Tab);
            RibbonPanel panel1 = null;
           

            List<RibbonPanel> panels = application.GetRibbonPanels(Ribbon_Tab);
            panel1 = application.CreateRibbonPanel(Ribbon_Tab, Ribbon_Panel1);
           

            ///////////////////////////////////////////////////////

            #endregion

            #region Images
            ////////////////////////////////Rename Grids Pic //////////////////////////////////////////

            //Image img = Properties.Resources.test;
            //ImageSource imgSrc = GetImageSource(img);


            #endregion

            #region Create buttons data 
            // create button data(what the button does)

            string path = Assembly.GetExecutingAssembly().Location;
            PushButtonData btnTask1 = new PushButtonData("AddinBtn1", "Floor Create", path, "Axelerate.FloorRevitCommand.FloorCommand")
            {
                ToolTip = "Floor Create",
                //Image = imgSrc,
                //LargeImage = imgSrc,
            };

            //PushButtonData btnTask2 = new PushButtonData("AddinBtn2", "Room Create", path, "RoomRevitCommand.RoomCommand")
            //{
            //    ToolTip = "Room Create",
            //    //Image = imgSrc,
            //    //LargeImage = imgSrc,
            //};

            //PushButtonData btnTask4 = new PushButtonData("AddinBtn4", "Section Create", path, "SectionRevitCommand.SectionCommand")
            //{
            //    ToolTip = "Threshold Create",
            //    //Image = imgSrc,
            //    //LargeImage = imgSrc,
            //};

            #endregion

            #region Create button 

            //add the button to the ribbon


            PushButton button1 = panel1.AddItem(btnTask1) as PushButton;
            button1.Enabled = true;

            //PushButton button2 = panel1.AddItem(btnTask2) as PushButton;
            //button2.Enabled = true;

            //PushButton button4 = panel1.AddItem(btnTask4) as PushButton;
            //button4.Enabled = true;

            return Result.Succeeded;
            #endregion

        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;

        }

        private BitmapSource GetImageSource(Image img)
        {
            BitmapImage bmp = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;
                bmp.EndInit();
            }
            return bmp;
        }



        //    public Result OnShutdown(UIControlledApplication application)
        //    {
        //        return Result.Succeeded;

        //    }

        //    public Result OnStartup(UIControlledApplication application)
        //    {
        //        application.CreateRibbonTab("WPF APP");


        //        RibbonPanel ribbonPanel = application.CreateRibbonPanel("WPF APP", "Tag Automation");


        //        string path = Assembly.GetExecutingAssembly().Location;
        //        PushButtonData p1 = new PushButtonData("Btn1", "Tags Creator", path, "Axelerate.Application.RevitCommand");
        //        ribbonPanel.AddItem(p1);
        //        p1.ToolTip = "Tags Creator";
        //        Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(path), "Images","th"));
        //        BitmapImage bt = new BitmapImage();
        //        p1.ToolTipImage = bt;
        //        p1.LargeImage = bt;


        //        return Result.Succeeded;

        //    }
    }
}
