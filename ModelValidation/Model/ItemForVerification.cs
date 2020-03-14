using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ModelValidation.Core;

namespace ModelValidation
{
    public class ItemForVerification :  ViewModelBase
    {
        private Family _family;

        public string Name => _family.Name;
        public Guid Guid { get; set; }
        public double Version { get; set; }
        public bool IsItemHaveSchema { get; set; }
        public bool IsItemNameExistsInDb { get; set; }
        public bool IsItemGuidExistsInDb { get; set; }
        public string Status { get; set; }
        public double DbVersion { get; set; }

        public ItemForVerification(Family family)
        {
            _family = family;
        }

    }
}
