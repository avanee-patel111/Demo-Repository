using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class CustomWorkFlowCustomMetadataFieldMultiValueModel
    {
        public int CUSTOM_WORKFLOW_CUSTOM_FIELD_MULTIPLE_ID { get; set; }
        public int CUSTOM_WORKFLOW_ID { get; set; }      
        public int CUSTOM_FIELD_TYPE_ID { get; set; }
        public byte DATA_TYPE_ID { get; set; }
        public Nullable<int> VALUE { get; set; }
        public Nullable<bool> UNIQUE_FIELD { get; set; }
        public string FIELD_LABEL { get; set; }
        public Nullable<int> WORKFLOW_ENTRIES { get; set; }

    }
}
