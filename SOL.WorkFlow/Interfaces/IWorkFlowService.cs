using SOL.Common.Models;
using SOL.Common.Business.Events.Workflow;
using SOL.ECM.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IWorkFlowService<T>
    {

        /// <summary>
        /// Saves the work flow.
        /// </summary>
        /// <param name="documentWorkflow">The document workflow.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        int SaveWorkFlow(WorkFlowDetailsViewModel documentWorkflow, int userId, ref string errorMessage, int userType, ref bool isAuthorized);

        /// <summary>
        /// Gets the work flow.
        /// </summary>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <returns></returns>
        WorkFlowDetailsViewModel GetWorkFlow(int workFlowId);

        /// <summary>
        /// Gets the work flow short tile.
        /// </summary>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <returns></returns>
        string GetWorkFlowShortTile(int workFlowId);

        /// <summary>
        /// Gets the work flow short tile.
        /// </summary>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <returns></returns>
        string GetWorkFlowFullTile(int workFlowId);

        /// <summary>
        /// Saves the document work flow step.
        /// </summary>
        /// <param name="documentWorkflowModel">The document workflow model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        void SaveDocumentWorkFlowStep(WorkFlowDetailsViewModel documentWorkflowModel, int userId,
            ref string errorMessage);

        /// <summary>
        /// Saves the document work flow approval.
        /// </summary>
        /// <param name="documentWorkflowModel">The document workflow model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        void SaveDocumentWorkFlowApproval(WorkFlowDetailsViewModel documentWorkflowModel, int userId,
           ref string errorMessage);

        /// <summary>
        /// Get folder Path
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        string GetFolderPath(int folderId);

        /// <summary>
        /// Saves the document work flow escalation.
        /// </summary>
        /// <param name="documentWorkflowModel">The document workflow model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        void SaveDocumentWorkFlowEscalation(WorkFlowDetailsViewModel documentWorkflowModel, int userId,
       ref string errorMessage);

        List<int> GetAssocitePages(DocumentAssociation documentAssociation);

        WFCommonFields GetWorkFlowCommonFields(int workFlowId);

        bool IsDocumentApproved(int workFlowId, int pageNo, int docId);

        int? GetWorkFlowCustomFieldTypeId(int workFlowId);

        int GetWorkflowInitiator(int workFlowId, int entityId);

        int GetDocIdByEntityId(int entityId, int workFlowId);

        int GetNewDocIdByEntityId(int entityId, int workFlowId);

        void DeleteWorkflow(int workFlowId, int entityId, int userId, UserType userType, ref string errorMessage);

        WORKFLOW_CUSTOM WorkflowOwnerDisableSection(int workFlowId);

        void ReOrderSubItems(int destinationOrderId, int sourceOrderId, int userId, int userType, int listId);
    }
}
