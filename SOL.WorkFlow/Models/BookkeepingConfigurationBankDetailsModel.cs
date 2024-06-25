using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class BookkeepingConfigurationBankDetailsModel
    {
        public int BANK_DETAILS_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public string BANK_NAME { get; set; }
        public int ACCOUNT_TYPE { get; set; }
        public string ACCOUNT_TYPE_NAME { get; set; }
        public string ACCOUN_NUMBER { get; set; }
    }
}
