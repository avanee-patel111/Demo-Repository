using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public class UpdateStatusViewModel
    {
       public int WorkflowDefinitionId { get; set; }
        public int EntityId { get; set; }
       public int WorkFlowId { get; set; }
       public byte? Status { get; set; }
       public string ApproverNote { get; set; }

       public int? WorkflowOwner { get; set; }

       public WF_BILLING_ACCOUNT[] BillingAccounts { get; set; }
       public WORKFLOW_REVIEWER[] Approvers { get; set; }
    }
}
