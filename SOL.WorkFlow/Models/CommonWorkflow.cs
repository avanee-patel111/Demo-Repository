using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class CommonWorkflow
    {
        public int DocId { get; set; }
        public int OriginalDocId { get; set; }
        public int PageNo { get; set; }
        public int WorkflowId { get; set; }
        public int EntityId { get; set; }
    }
}
