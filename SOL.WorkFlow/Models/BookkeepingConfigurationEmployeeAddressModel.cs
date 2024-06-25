using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public class BookkeepingConfigurationEmployeeAddressModel
   {
       public int EMPLOYEE_ADDRESS_ID { get; set; }
       public int EMPLOYEE_ID { get; set; }
       public string ADDRESS1 { get; set; }
       public string ADDRESS2 { get; set; }
       public string CITY { get; set; }
       public Nullable<byte> COUNTRY_ID { get; set; }
       public Nullable<short> STATE_ID { get; set; }
       public string ZIP { get; set; }
    }
}
