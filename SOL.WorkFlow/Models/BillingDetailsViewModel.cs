using SOL.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class BillingDetailsViewModel
    {
        public WF_BILLING_DETAIL BillingDetail { get; set; }
        public WF_BILLING_ACCOUNT[] BillingAccounts { get; set; }
        public WORKFLOW_REVIEWER[] Approvers { get; set; }
        public UserType UserType { get; set; }
        public int[] AssocitaedPages { get; set; }
        public bool IsApproveMode { get; set; }    
        public int ENTITY_TYPE { get; set; }
        public List<DocumentWorkFlow> documentId { get; set; }

    }
}
