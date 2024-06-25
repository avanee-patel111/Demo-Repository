using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public class CustomWorkFlowCustomMetadataFieldValueModel
   {
       public int CUSTOM_WORKFLOW_CUSTOM_FIELD_ID { get; set; }
       public int CUSTOM_WORKFLOW_ID { get; set; }      
       public int CUSTOM_FIELD_TYPE_ID { get; set; }
       public byte DATA_TYPE_ID { get; set; }
       public Nullable<int> DDL_VALUE { get; set; }
       public Nullable<bool> BOOLEAN_VALUE { get; set; }
       public Nullable<int> INTEGER_VALUE { get; set; }
       public Nullable<decimal> DECIMAL_VALUE { get; set; }
       public Nullable<System.DateTime> DATETIME_VALUE { get; set; }
       public string TEXT_VALUE { get; set; }
       public string TEXT_AREA_VALUE { get; set; }
        public Nullable<System.TimeSpan> TIME_VALUE { get; set; }
        public Nullable<bool> UNIQUE_FIELD { get; set; }
       public string FIELD_LABEL { get; set; }
        public Nullable<int> WORKFLOW_ENTRIES { get; set; }
    }
}
