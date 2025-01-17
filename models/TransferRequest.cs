using System;
using System.Collections.Generic;

namespace Cargohub.models
{
    public class TransferRequest
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
    }
}