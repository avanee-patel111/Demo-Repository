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
    
    public partial class WORKFLOW_CUSTOM
    {
        public WORKFLOW_CUSTOM()
        {
            this.WF_BILLING_ACCOUNT = new HashSet<WF_BILLING_ACCOUNT>();
            this.WF_BILLING_DETAIL = new HashSet<WF_BILLING_DETAIL>();
            this.WORKFLOW_CUSTOM_FIELD_MULTIPLE = new HashSet<WORKFLOW_CUSTOM_FIELD_MULTIPLE>();
            this.WORKFLOW_CUSTOM_FIELD = new HashSet<WORKFLOW_CUSTOM_FIELD>();
            this.WORKFLOW_CUSTOM_FIELD_MULTIPLE1 = new HashSet<WORKFLOW_CUSTOM_FIELD_MULTIPLE>();
            this.WORKFLOW_CUSTOM_FIELD1 = new HashSet<WORKFLOW_CUSTOM_FIELD>();
        }
    
        public int WORKFLOW_ID { get; set; }
        public int WORKFLOW_DEFINITION_ID { get; set; }
        public Nullable<int> CLIENT_ID { get; set; }
        public System.DateTime DATE { get; set; }
        public string TITLE { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<byte> APPROVAL_STATUS { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool DELETED_FLAG { get; set; }
        public Nullable<int> WORKFLOW_OWNER { get; set; }
        public bool SAVE_AS_DRAFT { get; set; }
        public Nullable<bool> IS_ROLE { get; set; }
        public Nullable<int> WORKFLOW_STATUS { get; set; }
    
        public virtual ICollection<WF_BILLING_ACCOUNT> WF_BILLING_ACCOUNT { get; set; }
        public virtual ICollection<WF_BILLING_DETAIL> WF_BILLING_DETAIL { get; set; }
        public virtual ICollection<WORKFLOW_CUSTOM_FIELD_MULTIPLE> WORKFLOW_CUSTOM_FIELD_MULTIPLE { get; set; }
        public virtual ICollection<WORKFLOW_CUSTOM_FIELD> WORKFLOW_CUSTOM_FIELD { get; set; }
        public virtual ICollection<WORKFLOW_CUSTOM_FIELD_MULTIPLE> WORKFLOW_CUSTOM_FIELD_MULTIPLE1 { get; set; }
        public virtual ICollection<WORKFLOW_CUSTOM_FIELD> WORKFLOW_CUSTOM_FIELD1 { get; set; }
    }
}
