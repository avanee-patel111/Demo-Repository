using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public enum ApproverStatus
    {
       Assigned = 1,
       Waiting = 2,
       Upcoming = 3,
       Approved = 4,
       Denied = 5
    }
}
