using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{    
   public class BookkeepingConfigurationCreditCardDetailsModel
   {
       public int CREADIT_CARD_DETAILS_ID { get; set; }
       public int COMPANY_ID { get; set; }
       public string BANK_NAME { get; set; }
       public int CARD_TYPE { get; set; }
       public string CARD_TYPE_NAME { get; set; }
       public string CARD_NUMBER { get; set; }
    }
}
