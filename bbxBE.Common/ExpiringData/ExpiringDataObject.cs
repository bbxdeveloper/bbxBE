using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Common.ExpiringData
{
    public class ExpiringDataObject
    {
        public ExpiringDataObject() { }
        public string Key { get; set; }
        public object Data { get; set; }
        public DateTime LastModifiedTimestamp { get; set; }
        public TimeSpan Lifetime { get; set; }
        public DateTime ExpiredTimeStamp
        {
            get { return LastModifiedTimestamp.Add(Lifetime); }
        }
    }
}
