using SOL.Common.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IApprovalService<T>
    {
        /// <summary>
        /// Saves the approver note.
        /// </summary>
        /// <param name="objApproverNoteViewModel">The object approver note view model.</param>
        void SaveApproverNote(ApproverNoteViewModel objApproverNoteViewModel);
        /// <summary>
        /// Manages the approver note.
        /// </summary>
        /// <param name="objBillingView">The object billing view.</param>
        /// <param name="billId">The bill identifier.</param>
        void ManageApproverNote(WF_BILLING_DETAIL objBillingView, int billId);
        /// <summary>
        /// Updates the approver order.
        /// </summary>
        /// <param name="objApprover">The object approver.</param>
        void UpdateApproverOrder(WORKFLOW_REVIEWER[] objApprover);
        /// <summary>
        /// Deletes the approver.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="contactId">The contact identifier.</param>
        void DeleteApprover(int entityId, int contactId, int userId, int workflowId);
        void DeleteReviewer(int entityId, int contactId, int userId, int workflowId);
        void DragAndDropInReviewerTable(int destinationId, int sourceId, int _userId,int WORKFLOW_DEFINITION_ID, int WORKFLOW_ID);
        WORKFLOW_DEFINITION GetCustomWorkflowIsSingleOrNot(int customWorkflowId);
        //WORKFLOW_CUSTOM GetCheckWorkflowMultipleEntries(int customWorkflowId);
        /// <summary>
        /// Saves the approvers.
        /// </summary>
        /// <param name="approvers">The approvers.</param>
        /// <param name="approvalNote">The approval note.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="workflowId">The workflow identifier.</param>
        ApproverStatus SaveApprovers(WORKFLOW_REVIEWER[] approvers, string approvalNote, int entityId, int workflowId, int userId, byte? approvalStatus, byte? ByOverView, string companyLogo);
        void SaveWorkflowMessage(WorkFlowMessage approvalNote, int userId,  string companyLogo);
        /// <summary>
        /// Manages the approvers on approvals.
        /// </summary>
        /// <param name="Approvers">The approvers.</param>
        /// <param name="ApproverNote">The approver note.</param>
        /// <param name="status">The status.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <param name="userId">The user identifier.</param>
        void ManageApproversOnApprovals(WORKFLOW_REVIEWER[] Approvers, string ApproverNote,int? WorkflowOwner, byte? status,
            int workflowDefinitionId, int workFlowId, int userId,string companyLogo);
        /// <summary>
        /// Determines whether [is any pending aprrover] [the specified bill identifier].
        /// </summary>
        /// <param name="entityId">The bill identifier.</param>
        /// <param name="workflowId">The workflow identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is any pending aprrover] [the specified bill identifier]; otherwise, <c>false</c>.
        /// </returns>
        bool IsAnyPendingAprrover(int entityId, int workflowId);
        /// <summary>
        /// Determines whether [is current user pre approver] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="approvers">The approvers.</param>
        /// <returns>
        ///   <c>true</c> if [is current user pre approver] [the specified user identifier]; otherwise, <c>false</c>.
        /// </returns>
        bool IsCurrentUserPreApprover(int userId, WORKFLOW_REVIEWER[] approvers);

        /// <summary>
        /// Determines whether [is pre approver email notification] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="approvers">The approvers.</param>
        /// <returns>
        ///   <c>true</c> if [is pre approver email notification] [the specified user identifier]; otherwise, <c>false</c>.
        /// </returns>
        List<int> IsPreApproverEmailNotification(int userId, WORKFLOW_REVIEWER[] approvers);

        /// <summary>
        /// Gets the next approver.
        /// </summary>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        WORKFLOW_REVIEWER GetNextApprover(int workFlowId, int entityId);

        /// <summary>
        /// Gets the role users.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        List<int> GetRoleUsers(int roleId);

        /// <summary>
        /// Gets the workflow users.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <returns></returns>
        List<int> GetWorkflowUsers(int entityId, int workFlowId);
        List<WF_APPROVER_NOTE> GetApproverNotes(int customWorkflowId);
    }
}
