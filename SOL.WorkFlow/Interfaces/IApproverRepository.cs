using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
   public interface IApproverRepository<T>
    {
        /// <summary>
        /// Deletes the approver.
        /// </summary>
        /// <param name="approverEntityId">The approver entity identifier.</param>
       void DeleteApprover(int approverEntityId, int workflowId);
        void DeleteReviewer(int approverEntityId, int workflowId);
        /// <summary>
        /// Updates the approver order.
        /// </summary>
        /// <param name="approver">The approver.</param>
        void UpdateApproverOrder(WORKFLOW_REVIEWER approver);
       /// <summary>
       /// Deletes the approver.
       /// </summary>
       /// <param name="approverEntityId">The approver entity identifier.</param>
       /// <param name="contactId">The contact identifier.</param>
       void DeleteApprover(int approverEntityId, int contactId, int workflowId);
        /// <summary>
        /// Gets the approver by contact identifier.
        /// </summary>
        /// <param name="approverEntityId">The approver entity identifier.</param>
        /// <param name="contactId">The contact identifier.</param>
        /// <returns></returns>
        WORKFLOW_REVIEWER GetApproverByContactId(int approverEntityId, int contactId, int workFlowId);
        WORKFLOW_DEFINITION GetCustomWorkflowIsSingleOrNot(int customWorkflowId);
        WORKFLOW_REVIEWER GetWaitingStatus(int approverEntityId, int workflowId);
        WORKFLOW_REVIEWER IsUserInWaiting(int approverEntityId, int workflowId, int contacid);
        WORKFLOW_REVIEWER GetWorkFlowReviewers(int approverEntityId, int workflowId);
        IEnumerable<WORKFLOW_REVIEWER> GetReviewerByWorkFlowIDd(int workFlowId);
        void SaveChanges1();
        /// <summary>
        /// Gets the upcoming approver by entity identifier.
        /// </summary>
        /// <param name="approverEntityId">The approver entity identifier.</param>
        /// <returns></returns>
        WORKFLOW_REVIEWER GetUpcomingApproverByEntityId(int approverEntityId, int workFlowId);
       /// <summary>
       /// Saves the approver.
       /// </summary>
       /// <param name="objApprover">The object approver.</param>
       /// <returns></returns>
       int SaveApprover(WORKFLOW_REVIEWER objApprover);
        /// <summary>
        /// Gets the approver by identifier.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns></returns>
        WORKFLOW_REVIEWER GetApproverById(int accountId);
        WORKFLOW_REVIEWER GetApproverByworkflowId(int? contactId , int workflowId);
        List<WORKFLOW_REVIEWER> GetApproverStatusByworkflowId( int workflowId);
        WORKFLOW_CUSTOM GetworkflowCustombyworkflowId(int workFlowId, int workflowDenitionId);

        bool? GetworkflowOwnerIsRole(int workFlowId, int? contactId);
       /// <summary>
       /// Determines whether [is any pending aprrove] [the specified bill identifier].
       /// </summary>
       /// <param name="entityId">The bill identifier.</param>
       /// <returns>
       ///   <c>true</c> if [is any pending aprrove] [the specified bill identifier]; otherwise, <c>false</c>.
       /// </returns>
        bool IsAnyPendingAprrover(int entityId, int workFlowId);
       /// <summary>
       /// Saves the approver note.
       /// </summary>
       /// <param name="objApproverNote">The object approver note.</param>
       void SaveApproverNote(WF_APPROVER_NOTE objApproverNote);
       /// <summary>
       /// Gets the entity approvers.
       /// </summary>
       /// <param name="entityId">The entity identifier.</param>
       /// <param name="workFlowId">The work flow identifier.</param>
       /// <returns></returns>
       List<WORKFLOW_REVIEWER> GetEntityApprovers(int entityId, int workFlowId);

        List<WF_APPROVER_NOTE> GetApproverNotes(int customWorkflowId);

        /// <summary>
        /// Gets the next approver.
        /// </summary>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        WORKFLOW_REVIEWER GetNextApprover(int workFlowId, int entityId);

       /// <summary>
       /// Gets the workflow initiator.
       /// </summary>
       /// <param name="workFlowId">The work flow identifier.</param>
       /// <param name="entityId">The entity identifier.</param>
       /// <returns></returns>
       int GetWorkflowInitiator(int entityId, int workFlowId);
    }
}
