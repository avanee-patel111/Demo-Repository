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
    
    public partial class WF_BK_MS_INDUSTRY
    {
        public WF_BK_MS_INDUSTRY()
        {
            this.WF_BK_MS_KPIS = new HashSet<WF_BK_MS_KPIS>();
            this.WF_BK_MS_SALES_CATEGORY = new HashSet<WF_BK_MS_SALES_CATEGORY>();
            this.WF_BK_KPIS = new HashSet<WF_BK_KPIS>();
        }
    
        public int ID { get; set; }
        public string INDUSTRY_NAME { get; set; }
        public string OPERATION_HOUR_LABLE_ONE { get; set; }
        public string OPERATION_HOUR_LABLE_TWO { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool ACTIVE_FLAG { get; set; }
        public bool DELETED_FLAG { get; set; }
    
        public virtual ICollection<WF_BK_MS_KPIS> WF_BK_MS_KPIS { get; set; }
        public virtual ICollection<WF_BK_MS_SALES_CATEGORY> WF_BK_MS_SALES_CATEGORY { get; set; }
        public virtual ICollection<WF_BK_KPIS> WF_BK_KPIS { get; set; }
    }
}
