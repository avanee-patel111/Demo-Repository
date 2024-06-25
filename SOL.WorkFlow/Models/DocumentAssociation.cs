using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class DocumentAssociation
    {
        public int DocId { get; set; }
        public int WorkFlowId { get; set; }
        public int EntityId { get; set; }       
    }
}
