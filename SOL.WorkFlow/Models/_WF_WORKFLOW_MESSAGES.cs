using SOL.Common.Business.Events.ECM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
  public  partial class WF_WORKFLOW_MESSAGES
    {

        public CommunicationShare CommunicationShare { get; set; }
        public bool IsShareWithOthers { get; set; }
    }
}
