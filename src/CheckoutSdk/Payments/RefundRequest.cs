using System.Collections.Generic;

namespace Checkout.Payments
{
    public class RefundRequest
    {
        public int? Amount { get; set; }
        public string Reference { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}