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
    
    public partial class WF_BK_MS_CUSTOMER_TYPE
    {
        public WF_BK_MS_CUSTOMER_TYPE()
        {
            this.WF_BK_CUSTOMER = new HashSet<WF_BK_CUSTOMER>();
        }
    
        public int TYPE_ID { get; set; }
        public string TYPE_NAME { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool ACTIVE_FLAG { get; set; }
        public bool DELETED_FLAG { get; set; }
    
        public virtual ICollection<WF_BK_CUSTOMER> WF_BK_CUSTOMER { get; set; }
    }
}