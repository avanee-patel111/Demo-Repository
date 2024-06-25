using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public class ApplyCreditViewModel
    {
       public int VENDOR_CREDIT_ID { get; set; }
       public int BILL_ID { get; set; }
       public decimal AMOUNT { get; set; }
       public decimal AVAILABLE_AMOUNT { get; set; }
       public string REFERENCE { get; set; }
       public int VENDOR_ID { get; set; }
       public decimal APPLY_AMOUNT { get; set; }
    }
}
