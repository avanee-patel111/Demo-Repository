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
    
    public partial class WF_BK_KPIS
    {
        public int BK_KPIS_ID { get; set; }
        public int KPIS_ID { get; set; }
        public int INDUSTRY_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool DELETED_FLAG { get; set; }
    
        public virtual WF_BK_MS_INDUSTRY WF_BK_MS_INDUSTRY { get; set; }
        public virtual WF_BK_MS_KPIS WF_BK_MS_KPIS { get; set; }
    }
}