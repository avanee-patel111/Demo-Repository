using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    using System;
    public class WorkFlowMessage
    {
        public int WORKFLOW_DEFINITION_ID { get; set; }
        public int WORKFLOW_ID { get; set; }
        public string NOTE_TO_PAYER { get; set; }
        public Nullable<byte> APPROVAL_STATUS { get; set; }
        public Nullable<int> CONTACT_ID { get; set; }
    }
}
