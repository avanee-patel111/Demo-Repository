using SOL.Common.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface ICustomWorkflowService<T>
    {
        object SaveCustomWorkFlow(CustomWorkFlowModel customWorkFlowModel, int userId, UserType userType, string addedBy,
               ref string errorMessage, string companyLogo, string companyUrl);

        object DeleteCustomFieldsEntry(int Entries,int WorkflowId, int _userId, UserType _userType);
        object SaveEditCustomFieldValue(CustomWorkFlowModel customWorkFlowModel,int _userId);

        object SaveCustomWorkFlowDraftToNotstarted(workflowDraftToNotstarted customWorkFlowModel, int userId, UserType userType, string addedBy,
               ref string errorMessage, string companyLogo, string companyUrl);
        
        object EditCustomWorkFlowTitle(WorkflowCustomTilte customWorkFlowModel, int userId, UserType userType, string addedBy,
               ref string errorMessage, string companyLogo, string companyUrl);
        object CustomworkflowFields(CustomWorkflowCustomField CustomworkflowCustomFields,
                 ref string errorMessage, int _userId);
        object CustomMultiworkflowFields(CustomWorkflowCustomField CustomworkflowCustomFields,
                ref string errorMessage, int _userId);
        object CustomWorkflowFieldMultiple(CustomWorkflowCustomField CustomworkflowCustomFields,
                 ref string errorMessage, int _userId);
        object CustomMultiWorkflowFieldMultiple(CustomWorkflowCustomField CustomworkflowCustomFields,
                 ref string errorMessage, int _userId);
        void ApproveCustomWorkflowStatus(UpdateStatusViewModel objUpdateStatus, int userId, UserType userType, 
            string companyLogo, string companyUrl);
        WORKFLOW_CUSTOM GetCustomWorkFlow(int customWorkflowId);
        int? GetWorkflowIdinWorkflow(int docId, int userId);
        bool IsDocViewableInWorkFlow(int contactId, int docId, int workflowId);
        int? GetCustomEntityIdInWorkFlow(int docId, int userId);

        int? GetFolderIdByWorkflowId(int customWorkflowId);
    }
}
