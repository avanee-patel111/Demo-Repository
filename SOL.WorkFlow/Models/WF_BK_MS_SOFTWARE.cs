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
    
    public partial class WF_BK_MS_SOFTWARE
    {
        public WF_BK_MS_SOFTWARE()
        {
            this.WF_BK_BOOKKEEPING_CONFIGURATION = new HashSet<WF_BK_BOOKKEEPING_CONFIGURATION>();
            this.WF_BK_BOOKKEEPING_CONFIGURATION1 = new HashSet<WF_BK_BOOKKEEPING_CONFIGURATION>();
        }
    
        public int BK_SOFTWARE_ID { get; set; }
        public byte SOFTWARE_TYPE_ID { get; set; }
        public string SOFTWARE_NAME { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool ACTIVE_FLAG { get; set; }
        public bool DELETED_FLAG { get; set; }
    
        public virtual WF_BK_MS_SOFTWARE WF_BK_MS_SOFTWARE1 { get; set; }
        public virtual WF_BK_MS_SOFTWARE WF_BK_MS_SOFTWARE2 { get; set; }
        public virtual ICollection<WF_BK_BOOKKEEPING_CONFIGURATION> WF_BK_BOOKKEEPING_CONFIGURATION { get; set; }
        public virtual ICollection<WF_BK_BOOKKEEPING_CONFIGURATION> WF_BK_BOOKKEEPING_CONFIGURATION1 { get; set; }
    }
}