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
    
    public partial class WF_BILLING_ACCOUNT
    {
        public int BILL_ACCOUNT_ID { get; set; }
        public int ACCOUNT_ENTITY_ID { get; set; }
        public int ACCOUNT_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal AMOUNT { get; set; }
        public Nullable<decimal> APPROVED_AMOUNT { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool DELETED_FLAG { get; set; }
        public int WORKFLOW_ID { get; set; }
    
        public virtual WORKFLOW_CUSTOM WORKFLOW_CUSTOM { get; set; }
    }
}