using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public class PayrollReportModel
    {
        public int PAYROLL_REPORT_ID { get; set; }
        public int WORKFLOW_ID { get; set; }
        public int CLIENT_ID { get; set; }
        public string TITLE { get; set; }
        public System.DateTime PAYROLL_REPORT_DATE { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<byte> APPROVAL_STATUS { get; set; }
        public int YEAR_START_DAY { get; set; }
        public int YEAR_START_MONTH { get; set; }
        public int YEAR_END_DAY { get; set; }
        public int YEAR_END_MONTH { get; set; }
        public System.DateTime PAYROLL_PERIOD_START_DATE { get; set; }
        public System.DateTime PAYROLL_PERIOD_END_DATE { get; set; }
        public bool IS_RECORDED { get; set; }
        public int PAYROLL_CYCLE_ID { get; set; }
        public System.DateTime PAYROLL_END_DATE { get; set; }
        public System.DateTime PAYROLL_DUE_DATE { get; set; }


        public int DOC_ID { get; set; }
        public int[] AssocitaedPages { get; set; }
        public bool IsApproveMode { get; set; }       
        public WORKFLOW_REVIEWER[] Approvers { get; set; }
        public string NOTE_TO_PAYER { get; set; }
        public List<DocumentWorkFlow> documentId { get; set; }


    }
}
