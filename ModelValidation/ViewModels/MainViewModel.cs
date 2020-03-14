using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using ModelValidation.Comparers;
using ModelValidation.Core;
using ModelValidation.Views;

namespace ModelValidation
{
    public class MainViewModel : ViewModelBase
    {
        public ValidationController Controller { get; set; }
        public MainViewModel(Document doc)
        {
            var families
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.INVALID)
                    .OfClass(typeof(FamilyInstance))
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .Where(e => e.SuperComponent == null)
                    .Select(e => e.Symbol.Family)
                    .Distinct(new FamilyNameComparer())
                    .ToList();

            Controller = new ValidationController(families); 
            Controller.RunValidation();
        }
    }
}
