using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class WorkFlowDetailsViewModel
    {
        /************ Start Workflow **********/
        public int WORKFLOW_ID { get; set; }
        public string WORKFLOW_TITLE { get; set; }
        public string SHORT_TITLE { get; set; }
        public Nullable<int> CUSTOME_FIELD_ID { get; set; }
        public Nullable<int> ACL_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<bool> IS_BOOKKEEPING_CLIENT_SPECIFIC { get; set; }
        public Nullable<byte> APPROVAL_CHANGES_TYPE { get; set; }
        public bool ACTIVE_FLAG { get; set; }
        public Nullable<byte> WORK_FLOW_LEVEL { get; set; }
        public Nullable<bool> ALLOW_MULTIPLE_ENTRIES { get; set; }
        public Nullable<bool> IS_SHOW_IN_MENU { get; set; }

        public Nullable<int> SECTION_ID { get; set; }

        /************  End Workflow  ***********/


        /************ Start Move **********/
        public List<DocumentWorkflowStepsModel> DocumentWorkflowSteps { get; set; }
        /************  End Move  ***********/

        /************ Start Approval **********/
        public List<DocumentWorkflowApprovalModel> DocumentWorkflowApproval { get; set; }
        /************  End Approval  ***********/

        /************ Start Escalation **********/
        public DocumentWorkflowEscalationModel DocumentWorkflowEscalation { get; set; }

        /************  End Escalation  ***********/
    }
}
