using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class workFlowCommentMsgInfo
    {
        public WF_WORKFLOW_MESSAGE_COMMENT workflowMessageComment { get; set; }
        public int WorkflowId { get; set; }
        public int ProcessId { get; set; }
    }
}
