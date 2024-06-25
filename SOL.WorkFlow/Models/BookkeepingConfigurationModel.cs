using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class BookkeepingConfigurationModel
    {
        public int BKC_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public string COMPANY_LEGAL_NAME { get; set; }
        public string DBA_NAME { get; set; }
        public int INDUSTRY_TYPE_ID { get; set; }
        public int COMPANY_ENTITY_TYPE_ID { get; set; }
        public int ACCOUNTING_SOFTWARE_ID { get; set; }
        public Nullable<int> POS_SOFTWARE_ID { get; set; }
        public decimal OT_MULTIPLIRE { get; set; }
        public Nullable<int> PAYROLL_CYCLE_ID { get; set; }
        public System.DateTime PAYROLL_DATE { get; set; }
        public System.DateTime PAYROLL_END_DATE { get; set; }

        public string PAYROL_CYCLE_NAME { get; set; }
        public string INDUSTRY_NAME { get; set; }
        public string COMPANY_ENTITY_TYPE { get; set; }
        public string POS_SOFTWARE_NAME { get; set; }
        public string ACCOUNTING_SOFTWARE_NAME { get; set; }
              
        public List<BookkeepingConfigurationBankDetailsModel> BankDetails { get; set; }
        public List<BookkeepingConfigurationCreditCardDetailsModel> CreditCardDetails { get; set; }
        public List<BookkeepingConfigurationOperationHoursModel> OperationHours { get; set; }
        public IEnumerable<CustomMetadataFieldValueModel> CustomMetadataFieldValueModel { get; set; }
        public IEnumerable<CustomMetadataFieldMultipleModel> CustomMetadataFieldMultipleModel { get; set; }
        public IEnumerable<CustomMetadataFieldsModel> CustomMetadataFieldsModel { get; set; }
       
    }
}
