//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SOL.WorkFlow.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class WF_APPROVER_NOTE
    {
        public int APPROVER_NOTE_ID { get; set; }
        public Nullable<int> ENTITY_ID { get; set; }
        public Nullable<int> CONTACT_ID { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool DELETED_FLAG { get; set; }
        public string APPROVER_NOTE { get; set; }
    }
}
