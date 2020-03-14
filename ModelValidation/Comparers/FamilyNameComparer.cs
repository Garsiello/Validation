using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ModelValidation.Comparers
{
    public class FamilyNameComparer : IEqualityComparer<Family>
    {
        public bool Equals(Family x, Family y)
        {

            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.Name == y.Name;
        }


        public int GetHashCode(Family familyInstance)
        {

            int hashFamilyInstance = familyInstance.Name == null ? 0 :
                familyInstance.Name.GetHashCode();
            return hashFamilyInstance;
        }
    }
}
