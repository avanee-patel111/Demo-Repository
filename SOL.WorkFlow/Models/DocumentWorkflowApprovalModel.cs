using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public class DocumentWorkflowApprovalModel
   {      
       public Nullable<int> APPROVAL_LEVEL { get; set; }
       public Nullable<int> USER_OR_ROLE { get; set; }
       public Nullable<bool> IS_USER_ROLE { get; set; }
    }
}
