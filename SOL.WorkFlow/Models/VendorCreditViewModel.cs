using SOL.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public class VendorCreditViewModel
    {
        public WF_VENDOR_CREDIT_DETAIL VendorCreditDetail { get; set; }
        public WF_BILLING_ACCOUNT[] VendorCreditAccounts { get; set; }
        public WORKFLOW_REVIEWER[] Approvers { get; set; }
        public UserType UserType { get; set; }
        public int[] AssocitaedPages { get; set; }
        public bool IsApproveMode { get; set; }
        public int WORKFLOW_ID { get; set; }
        public List<DocumentWorkFlow> documentId { get; set; }
    }
}
