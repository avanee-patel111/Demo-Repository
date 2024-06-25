using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOL.Common.Business.Events.ECM;

namespace SOL.WorkFlow.Models
{
    public class CustomWorkFlowReviewers
    {
   
        public WORKFLOW_REVIEWER[] Approvers { get; set; }
        public string AppropverNote { get; set; }
        public int entityId { get; set; }

        public int workFlowId { get; set; }
        public byte approvalStatus { get; set; }
        public byte ByOverView { get; set; }
    }
}
