using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using FamilyManager.Actions;
using ModelValidation.Core;

namespace ModelValidation
{
    public class ValidationController : ViewModelBase
    {
        private List<ItemForVerification> _itemsForVerification;
        private ObservableCollection<ItemForVerification> _lastVersionItems;
        private ObservableCollection<ItemForVerification> _unregisteredItems;
        private ObservableCollection<ItemForVerification> _outdatedVersionItems;
        public List<ItemForVerification> ItemsForVerification => _itemsForVerification;
        public ObservableCollection<ItemForVerification> OutdatedVersionItems => _outdatedVersionItems;

        public ObservableCollection<ItemForVerification> LastVersionItems => _lastVersionItems;
        public ObservableCollection<ItemForVerification> UnregisteredItems => _unregisteredItems;

        public ValidationController(List<Family> families)
        {
            _itemsForVerification =
                new List<ItemForVerification>(families.Select(e => new ItemForVerification(e)));
            _lastVersionItems = new ObservableCollection<ItemForVerification>();
            _outdatedVersionItems = new ObservableCollection<ItemForVerification>();
            _unregisteredItems = new ObservableCollection<ItemForVerification>();
        }

        public void RunValidation()
        {
            foreach (var itemForVerification in _itemsForVerification)
            {
                SendRequest(itemForVerification);

                if (itemForVerification.IsItemNameExistsInDb)
                {
                    if (itemForVerification.IsItemHaveSchema)
                    {
                        if (itemForVerification.Status == "Действует")
                        {
                            if (itemForVerification.Version == itemForVerification.DbVersion)
                            {
                                _lastVersionItems.Add(itemForVerification);
                            }
                            else _outdatedVersionItems.Add(itemForVerification);
                        }
                        //Вот тут петрушка
                    }
                    else _unregisteredItems.Add(itemForVerification);

                }
                else _unregisteredItems.Add(itemForVerification);
            }
        }

        public void SendRequest(ItemForVerification item)
        {
            FamilyStorage familyStorage = new FamilyStorage();

            //var a = item.Name;

            //item.IsItemNameExistsInDb = true;
            //item.Guid = "";
            //item.Status = "";
            //item.Version = 1;
        }
    }
}