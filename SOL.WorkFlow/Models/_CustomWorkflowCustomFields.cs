using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOL.Common.Business.Events.ECM;

namespace SOL.WorkFlow.Models
{
    public class CustomWorkflowCustomField
    {
        public int WORKFLOW_ID { get; set; }

        public CustomWorkFlowCustomMetadataFieldMultiValueModel[] CustomWorkFlowCustomMetadataFieldMultiValueModel { get; set; }
        public CustomWorkFlowCustomMetadataFieldValueModel[] CustomWorkFlowCustomMetadataFieldValueModel { get; set; }


    }
}
