using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOL.Common.Business.Events.ECM;

namespace SOL.WorkFlow.Models
{
  public  class CustomWorkFlowModel
  {
      public int WORKFLOW_DEFINITION_ID { get; set; }
      public int WORKFLOW_ID { get; set; }
      public Nullable<int> CLIENT_ID { get; set; }
      public System.DateTime DATE { get; set; }
      public string TITLE { get; set; }
      public string DESCRIPTION { get; set; }     
      public CustomWorkFlowCustomMetadataFieldMultiValueModel[] CustomWorkFlowCustomMetadataFieldMultiValueModel { get; set; }
      public CustomWorkFlowCustomMetadataFieldValueModel[] CustomWorkFlowCustomMetadataFieldValueModel { get; set; }
      public int DOC_ID { get; set; }
      public int[] AssocitaedPages { get; set; }
      public bool IsApproveMode { get; set; }
      public Nullable<byte> APPROVAL_STATUS { get; set; }
      public List<DsrReportSalesCategory> DsrReportSalesCategory { get; set; }
      public List<DsrReportLineItems> DsrReportLineItems { get; set; }
      public WORKFLOW_REVIEWER[] Approvers { get; set; }
      public string NOTE_TO_PAYER { get; set; }
        public int WORKFLOW_OWNER { get; set; }
        public int WORKFLOW_STATUS { get; set; }
        public bool WORKFLOW_OWNER_IS_ROLE { get; set; }
      public WorkFlowMessage Workflowmsg { get; set; }
      //public DocumentWorkFlow[] documentId { get; set; }
        public List<DocumentWorkFlow> documentId { get; set; }
        public int FOLDER_ID { get; set; }
      public bool SAVE_AS_DRAFT { get; set; }
        public WorkflowlinkDocument[] LINK_DOC_ID { get; set; }
      public List<UploadedFilesModel> UploadedFiles { get; set; }

    }
}
