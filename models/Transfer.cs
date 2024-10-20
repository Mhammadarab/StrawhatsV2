using System;
using System.Collections.Generic;

namespace Cargohub.models
{
    public class Transfer
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public int? Transfer_From { get; set; } // Nullable since transfer_from can be null
        public int Transfer_To { get; set; }
        public string Transfer_Status { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public List<ItemDetail> Items { get; set; }
    }
}
