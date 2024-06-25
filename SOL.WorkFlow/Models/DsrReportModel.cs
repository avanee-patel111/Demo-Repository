using SOL.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class DsrReportModel
    {
        public int DSR_ID { get; set; }
        public string TITLE { get; set; }      
        public int CLIENT_ID { get; set; }
        public System.DateTime DSR_DATE { get; set; }
        public Nullable<decimal> OPERATING_HOURS { get; set; }
        public short NO_OF_CHECKS { get; set; }
        public int WORKFLOW_ID { get; set; }
        public decimal TOTAL_SALES { get; set; }
        public decimal AVERAGE_TOTAL { get; set; }
        public Nullable<decimal> AMEX_CC_RECEIPTS { get; set; }
        public Nullable<decimal> AMEX_CC_TIPS { get; set; }
        public Nullable<decimal> AMEX_CC_SALES_TEX { get; set; }
        public Nullable<decimal> M_V_D_CC_RECEIPTS { get; set; }
        public Nullable<decimal> M_V_D_CC_TIPS { get; set; }
        public Nullable<decimal> M_V_D_CC_SALES_TEX { get; set; }
        public Nullable<decimal> TOTAL_CC_RECEIPTS { get; set; }
        public Nullable<decimal> TOTAL_CC_TIPS { get; set; }
        public Nullable<decimal> TOTAL_CC_SALES_TEX { get; set; }
        public decimal NET_CC_SALES { get; set; }
        public decimal CC_TIPS_PERCENTAGE { get; set; }
        public decimal CASE_SALES { get; set; }
        public string DESCRIPTION { get; set; }

        public int DOC_ID { get; set; }
        public int[] AssocitaedPages { get; set; }
        public bool IsApproveMode { get; set; }
        public Nullable<byte> APPROVAL_STATUS { get; set; }
        public List<DsrReportSalesCategory> DsrReportSalesCategory { get; set; }
        public List<DsrReportLineItems> DsrReportLineItems { get; set; }
        public WORKFLOW_REVIEWER[] Approvers { get; set; }       
        public string NOTE_TO_PAYER { get; set; }
        public List<DocumentWorkFlow> documentId { get; set; }
    }
}
