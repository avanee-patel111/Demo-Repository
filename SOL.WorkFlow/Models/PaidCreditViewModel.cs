using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public class PaidCreditViewModel
    {
        public int PAID_CREDIT_ID { get; set; }
        public string VENDOR_CREDIT_REFERENCE { get; set; }
        public int VENDOR_CREDIT_ID { get; set; }
        public int BILL_ID { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool ACTIVE_FLAG { get; set; }
        public bool DELETED_FLAG { get; set; }
    }
}
