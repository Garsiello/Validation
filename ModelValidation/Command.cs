#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ModelValidation.Comparers;
using ModelValidation.Views;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace ModelValidation
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            DoWork cmd = new DoWork(commandData);
            cmd.Main();

            return Result.Succeeded;
        }
    }

    public class DoWork
    {
        UIApplication uiapp;
        UIDocument uidoc;
        Application app;
        Document doc;
        public DoWork(ExternalCommandData commandData)
        {
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;
        }

        public void Main()
        {
            Window win = new Window();
            win.Content = new MainWindow(doc);
            win.Show();
            
            //foreach (FamilyInstance e in familyInstances)
            //{
                
                
            //}

            // Modify document within a transaction

            //using (Transaction tx = new Transaction(doc))
            //{
            //    tx.Start("Transaction Name");
            //    element.LookupParameter("Комментарии").Set("1");
            //    tx.Commit();
            //}

        }

    }
}
