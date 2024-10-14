using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cargohub.models
{
    public class Transfer
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public int? TransferFrom { get; set; } // Nullable since transfer_from can be null
        public int TransferTo { get; set; }
        public string TransferStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Item> Items { get; set; }
    }
}