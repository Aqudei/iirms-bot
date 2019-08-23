using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIRMSBot2.Model
{
    class Item : Caliburn.Micro.PropertyChangedBase
    {
        private readonly string _fileName;
        private ITEM_STATUS _itemStatus = ITEM_STATUS.PENDING;

        public Item(string fileName)
        {
            _fileName = fileName;
        }

        public enum ITEM_STATUS
        {
            PENDING,
            READING,
            UPLOADING,
            FAILURE,
            SUCCESS
        }

        public string Error { get; set; }

        public string FileName => _fileName;
        public ITEM_STATUS ItemStatus { get => _itemStatus; set => Set(ref _itemStatus, value); }


        protected bool Equals(Item other)
        {
            return _fileName == other._fileName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Item)obj);
        }

        public override int GetHashCode()
        {
            return (_fileName != null ? _fileName.GetHashCode() : 0);
        }
    }
}
