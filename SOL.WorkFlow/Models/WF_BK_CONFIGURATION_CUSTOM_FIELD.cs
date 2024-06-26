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
    
    public partial class WF_BK_CONFIGURATION_CUSTOM_FIELD
    {
        public int ID { get; set; }
        public int BKC_ID { get; set; }
        public int CUSTOM_FIELD_ID { get; set; }
        public int CUSTOM_FIELD_TYPE_ID { get; set; }
        public byte DATA_TYPE_ID { get; set; }
        public Nullable<int> DDL_VALUE { get; set; }
        public Nullable<bool> BOOLEAN_VALUE { get; set; }
        public Nullable<int> INTEGER_VALUE { get; set; }
        public Nullable<decimal> DECIMAL_VALUE { get; set; }
        public Nullable<System.DateTime> DATETIME_VALUE { get; set; }
        public string TEXT_VALUE { get; set; }
        public string TEXT_AREA_VALUE { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime DATE_OF_ENTRY { get; set; }
        public int USER_MODIFIED { get; set; }
        public System.DateTime DATE_MODIFIED { get; set; }
        public bool DELETED_FLAG { get; set; }
    
        public virtual WF_BK_BOOKKEEPING_CONFIGURATION WF_BK_BOOKKEEPING_CONFIGURATION { get; set; }
    }
}
