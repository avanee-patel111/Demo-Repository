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
    
    public partial class WF_CUSTOM_WORKFLOW_CUSTOM_FIELD_MULTIPLE
    {
        public int CUSTOM_WORKFLOW_CUSTOM_FIELD_MULTIPLE_ID { get; set; }
        public int CUSTOM_WORKFLOW_ID { get; set; }
        public int CUSTOM_FIELD_TYPE_ID { get; set; }
        public byte DATA_TYPE_ID { get; set; }
        public Nullable<int> VALUE { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool ACTIVE_FLAG { get; set; }
        public bool DELETED_FLAG { get; set; }
    
        public virtual WF_CUSTOM_WORKFLOW WF_CUSTOM_WORKFLOW { get; set; }
    }
}
