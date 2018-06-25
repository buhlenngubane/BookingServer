using System;
using System.Collections.Generic;

namespace BookingServer.Models.Accommodations
{
    public partial class Property
    {
        public Property()
        {
            AccDetail = new HashSet<AccDetail>();
        }

        public int PropId { get; set; }
        public int AccId { get; set; }
        public string PropName { get; set; }
        public byte[] Picture { get; set; }

        public Accommodation Acc { get; set; }
        public ICollection<AccDetail> AccDetail { get; set; }
    }
}
