using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class ApproverNoteViewModel
    {
        public int APPROVER_NOTE_ID { get; set; }
        public Nullable<int> ENTITY_ID { get; set; }
        public Nullable<int> CONTACT_ID { get; set; }
        public string APPROVER_NOTE { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool ACTIVE_FLAG { get; set; }
        public bool DELETED_FLAG { get; set; }
    }
}
