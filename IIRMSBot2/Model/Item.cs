using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIRMSBot2.Model
{
    class Item : Caliburn.Micro.PropertyChangedBase
    {
        private string _fileName;
        private ITEM_STATUS _itemStatus = ITEM_STATUS.PENDING;

        public enum ITEM_STATUS
        {
            PENDING,
            READING,
            UPLOADING,
            FAILURE,
            SUCCESS
        }

        public string FileName { get => _fileName; set => Set(ref _fileName, value); }
        public ITEM_STATUS ItemStatus { get => _itemStatus; set => Set(ref _itemStatus, value); }


    }
}
