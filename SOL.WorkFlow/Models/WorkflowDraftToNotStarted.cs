using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOL.Common.Business.Events.ECM;

namespace SOL.WorkFlow.Models
{
    public class workflowDraftToNotstarted
    {
        public int WORKFLOW_DEFINITION_ID { get; set; }
        public int WORKFLOW_ID { get; set; }
        public int WORKFLOW_STATUS { get; set; }

    }
}
