using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class DocumentWorkflowEscalationModel
    {
        public Nullable<byte> APPROVAL_EXPIRE_DAYS { get; set; }
        public Nullable<byte> APPROVAL_EXPIRE_HOURS { get; set; }
        public Nullable<byte> APPROVAL_EXPIRE_MINUTES { get; set; }
        public Nullable<bool> IS_REMINDER { get; set; }
        public Nullable<byte> REMINDER_MINUTES { get; set; }
        public Nullable<int> ESCALATION_TYPE_ID { get; set; }
        public Nullable<int> ESCALATION_USER_ID { get; set; }
        public bool IsDeleted { get; set; }
    }
}
