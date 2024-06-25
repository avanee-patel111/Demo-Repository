using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public class CientsAndVendorModel
    {
        public int CUSTOMER_ID { get; set; }
        public string NAME { get; set; }
        public string PHONE_NO { get; set; }
        public int TYPE_ID { get; set; }
      
        public int COMPANY_ID { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string CITY { get; set; }
        public Nullable<short> STATE_ID { get; set; }
        public string ZIP { get; set; }
        public Nullable<byte> COUNTRY_ID { get; set; }
        public bool VENDOR_1099 { get; set; }
        public string TAX_ID { get; set; }
        public string EMAIL { get; set; }
        public string URL { get; set; }
    }
}
