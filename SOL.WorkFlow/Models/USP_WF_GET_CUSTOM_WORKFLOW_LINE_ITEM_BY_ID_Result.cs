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
    
    public partial class USP_WF_GET_CUSTOM_WORKFLOW_LINE_ITEM_BY_ID_Result
    {
        public int CUSTOM_FIELD_TYPE_ID { get; set; }
        public int WORKFLOW_CUSTOM_FIELD_ID { get; set; }
        public int WORKFLOW_ID { get; set; }
        public string FIELD_LABEL { get; set; }
        public byte DATA_TYPE_ID { get; set; }
        public Nullable<int> DDL_VALUE { get; set; }
        public Nullable<bool> BOOLEAN_VALUE { get; set; }
        public Nullable<int> INTEGER_VALUE { get; set; }
        public Nullable<decimal> DECIMAL_VALUE { get; set; }
        public Nullable<System.DateTime> DATETIME_VALUE { get; set; }
        public string TEXT_VALUE { get; set; }
        public string TEXT_AREA_VALUE { get; set; }
        public Nullable<int> VALUE { get; set; }
        public string DDL_TEXT { get; set; }
        public Nullable<int> ORDER_ID { get; set; }
        public int IS_MULTIPLE_DRODOWN { get; set; }
        public Nullable<int> WORKFLOW_ENTRIES { get; set; }
    }
}
