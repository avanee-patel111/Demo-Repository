using SOL.Admin.Interfaces;
using SOL.Common.Models;
using SOL.Common.Business.Interfaces;
using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Services
{
    public class ApprovalService<T> : IApprovalService<T>
    {
        IApproverRepository<T> _repAapproval;
        ICustomWorkflowRepository<T> _repCustomWorkflow;
        IWorkflowMessageService<T> _srvWorkflowMessage;
        IWorkflowTimelineService<T> _srvTimeLine = null;
        IFeedService<T> _srvFeed = null;
        IAdminService<T> _srvAdmin;
        public const string APPROVER_NOTE = "Auto Approved";
        public const int ORDER_NO = 1;
        private const Int16 TITLE_MAX_LENGTH = 500;
        public ApprovalService(IApproverRepository<T> repAapproval, ICustomWorkflowRepository<T> repCustomWorkflow, IFeedService<T> srvFeed, IWorkflowMessageService<T> srvWorkflowMessage,
            IWorkflowTimelineService<T> srvTimeLine, IAdminService<T> srvAdmin)
        {
            this._repAapproval = repAapproval;
            this._repCustomWorkflow = repCustomWorkflow;
            this._srvFeed = srvFeed;
            this._srvWorkflowMessage = srvWorkflowMessage;
            this._srvTimeLine = srvTimeLine;
            this._srvAdmin = srvAdmin;
        }

        /// <summary>
        /// Saves the approvers.
        /// </summary>
        /// <param name="approvers">The approvers.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="workflowId">The workflow identifier.</param>
        public ApproverStatus SaveApprovers(WORKFLOW_REVIEWER[] approvers, string approvalNote, int entityId, int workflowId,
            int userId, byte? approvalStatus,byte? ByOverView, string companyLogo)
        {
            if (approvers.Count() > 0)
            {
                var date = DateTime.UtcNow;
                var useExistingApprovers = false;
                var existingApprovers = _repAapproval.GetEntityApprovers(entityId, workflowId);
                var CustomWorkflows = _repAapproval.GetworkflowCustombyworkflowId(entityId, workflowId);

                if (ByOverView == 0)
                {
                   
                    foreach (var approver in approvers)
                    {
                        useExistingApprovers = true;
                        var existingApprover = existingApprovers.Where(x => x.CONTACT_ID == approver.CONTACT_ID
                            && x.IS_ROLE == approver.IS_ROLE).FirstOrDefault();
                        if (existingApprover != null)
                        {
                            existingApprovers.Remove(existingApprover);
                        }
                        else
                        {
                            existingApprovers.Add(approver);
                        }
                    }
                    _repAapproval.DeleteApprover(entityId, workflowId);
                }

                if (ByOverView == 1)
                {
                    foreach (var item in approvers)
                    {
                        var objApprover = new WORKFLOW_REVIEWER();
                        objApprover.WORKFLOW_ID = entityId;
                        objApprover.WORKFLOW_DEFINITION_ID = workflowId;
                        var IsWaitingSTtatus = _repAapproval.GetWaitingStatus(entityId, workflowId);
                        if (IsWaitingSTtatus == null)
                        {
                            objApprover.REVIEWER_STATUS = (byte)ApproverStatus.Waiting;
                        }
                        else if (IsWaitingSTtatus.REVIEWER_STATUS == 2)
                        {
                            objApprover.REVIEWER_STATUS = (byte)ApproverStatus.Upcoming;
                        }
                        else
                        {
                            objApprover.REVIEWER_STATUS = (byte)ApproverStatus.Waiting;
                        }
                        //objApprover.REVIEWER_STATUS = (item.ORDER_ID == 2 ? (byte)ApproverStatus.Waiting :
                        //    (byte)ApproverStatus.Upcoming);
                        objApprover.CONTACT_ID = item.CONTACT_ID;
                        objApprover.ORDER_ID = item.ORDER_ID;
                        objApprover.IS_ROLE = item.IS_ROLE;
                        objApprover.USER_ID = userId;
                        objApprover.DATE_OF_ENTRY = date;
                        objApprover.USER_MODIFIED = userId;
                        objApprover.DATE_MODIFIED = date;
                        objApprover.DELETED_FLAG = false;
                        objApprover.REVIEWER_NOTE = item.REVIEWER_NOTE;
                        objApprover.REQUIRED_DATE = item.REQUIRED_DATE;

                        item.REVIEWER_ID = _repAapproval.SaveApprover(objApprover);
                        if(CustomWorkflows.WORKFLOW_STATUS == 4)
                        {
                            CustomWorkflows.WORKFLOW_STATUS = 2;
                            CustomWorkflows.WORKFLOW_OWNER = item.CONTACT_ID;
                            _repCustomWorkflow.SaveCustomWorkFlow(CustomWorkflows);
                        }
                        if (item.REVIEWER_NOTE != null)
                        {
                            var reviewerNote = new WorkFlowMessage();
                            reviewerNote.CONTACT_ID = item.CONTACT_ID;
                            reviewerNote.APPROVAL_STATUS = approvalStatus;
                            reviewerNote.WORKFLOW_ID = entityId;
                            reviewerNote.WORKFLOW_DEFINITION_ID = workflowId;
                            reviewerNote.NOTE_TO_PAYER = item.REVIEWER_NOTE;
                            SaveWorkflowMessage(reviewerNote, userId, companyLogo);
                        }
                    }
                }
                else
                {
                    foreach (var item in approvers)
                    {
                        var objApprover = new WORKFLOW_REVIEWER();
                        objApprover.WORKFLOW_ID = entityId;
                        objApprover.WORKFLOW_DEFINITION_ID = workflowId;
                        objApprover.REVIEWER_STATUS = (item.ORDER_ID == ORDER_NO ? (byte)ApproverStatus.Waiting :
                            (byte)ApproverStatus.Upcoming);
                        objApprover.CONTACT_ID = item.CONTACT_ID;
                        objApprover.ORDER_ID = item.ORDER_ID;
                        objApprover.IS_ROLE = item.IS_ROLE;
                        objApprover.USER_ID = userId;
                        objApprover.DATE_OF_ENTRY = date;
                        objApprover.USER_MODIFIED = userId;
                        objApprover.DATE_MODIFIED = date;
                        objApprover.DELETED_FLAG = false;
                        objApprover.REVIEWER_NOTE = item.REVIEWER_NOTE;
                        objApprover.REQUIRED_DATE = item.REQUIRED_DATE;

                        item.REVIEWER_ID = _repAapproval.SaveApprover(objApprover);
                        if (item.REVIEWER_NOTE != null)
                        {
                            var reviewerNote = new WorkFlowMessage();
                            reviewerNote.CONTACT_ID = item.CONTACT_ID;
                            reviewerNote.APPROVAL_STATUS = approvalStatus;
                            reviewerNote.WORKFLOW_ID = entityId;
                            reviewerNote.WORKFLOW_DEFINITION_ID = workflowId;
                            reviewerNote.NOTE_TO_PAYER = item.REVIEWER_NOTE;
                            SaveWorkflowMessage(reviewerNote, userId, companyLogo);
                        }
                    }
                }
                    


                if (useExistingApprovers == true)
                {
                    existingApprovers = existingApprovers.OrderBy(x => x.ORDER_ID).ToList();
                    foreach (var existingApprover in existingApprovers)
                    {
                        _srvTimeLine.SaveWorflowTimeline(entityId, workflowId, (int)approvalStatus, existingApprover.CONTACT_ID, null,
            (int)WorkflowTimelineObject.Approver, (int)WorkflowTimelineEvents.Approver_Added, approvalNote, userId, date,
            existingApprover.ORDER_ID, existingApprover.IS_ROLE);
                    }
                }
                else
                {
                    foreach (var approver in approvers)
                    {
                        _srvTimeLine.SaveWorflowTimeline(entityId, workflowId, (int)approvalStatus, approver.CONTACT_ID, null,
                   (int)WorkflowTimelineObject.Approver, (int)WorkflowTimelineEvents.Approver_Added, approvalNote, userId, date,
                   approver.ORDER_ID, approver.IS_ROLE);
                    }
                }
            }
            SaveNoteInMessages(approvalNote,0, entityId, workflowId, userId, approvalStatus, companyLogo);

            //Auto Approve
            var isBreakLoop = false;
            ApproverStatus finalApprovalStatus = ApproverStatus.Assigned;
            foreach (var item in approvers)
            {
                
                bool isUserInRole = (item.IS_ROLE??false) &&  _srvAdmin.IsUserInRole(item.CONTACT_ID, userId);
                if ((item.CONTACT_ID == userId || isUserInRole) && !isBreakLoop)
                {
                    finalApprovalStatus = PreApprovedApprover(item.REVIEWER_ID, item.CONTACT_ID, entityId, workflowId, userId, companyLogo);
                }
                else
                {
                    isBreakLoop = true;
                }

            }
            return finalApprovalStatus;
        }

        private void SaveNoteInMessages(string approvalNote,int? WorkflowOwner,int entityId, int workflowId, int userId, byte? status,string companyLogo)
        {
            
            if (!string.IsNullOrEmpty(approvalNote))
            {
                var workflowMessage = new WF_WORKFLOW_MESSAGES();               
                workflowMessage.DESCRIPTION = approvalNote;
                workflowMessage.TITLE = approvalNote.Length > TITLE_MAX_LENGTH? approvalNote.Substring(0, TITLE_MAX_LENGTH - 3) + "...": approvalNote;
                workflowMessage.MESSAGE_TYPE = status;
                workflowMessage.PROCESS_ID = workflowId;
                workflowMessage.WORKFLOW_ID = entityId;
                workflowMessage.CONTACT_ID = WorkflowOwner;
                _srvWorkflowMessage.SaveWorkFlowMessage(workflowMessage, userId ,companyLogo);
            }
        }

        public void SaveWorkflowMessage(WorkFlowMessage approvalNote,int userId,  string companyLogo)
        {
            var reviewerNote = approvalNote.NOTE_TO_PAYER;
            if (!string.IsNullOrEmpty(reviewerNote))
            {
                var workflowMessage = new WF_WORKFLOW_MESSAGES();
                workflowMessage.DESCRIPTION = reviewerNote;
                workflowMessage.TITLE = reviewerNote.Length > TITLE_MAX_LENGTH ? reviewerNote.Substring(0, TITLE_MAX_LENGTH - 3) + "..." : reviewerNote;
                workflowMessage.MESSAGE_TYPE = approvalNote.APPROVAL_STATUS;
                workflowMessage.PROCESS_ID = approvalNote.WORKFLOW_ID;
                workflowMessage.WORKFLOW_ID = approvalNote.WORKFLOW_DEFINITION_ID;
                workflowMessage.CONTACT_ID = approvalNote.CONTACT_ID;
                _srvWorkflowMessage.SaveWorkFlowMessage(workflowMessage, userId, companyLogo);
            }
        }
        /// <summary>
        /// Deletes the approver.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="contactId">The contact identifier.</param>
        public void DeleteApprover(int entityId, int contactId, int userId, int workflowId)
        {
            var approver = _repAapproval.GetApproverByContactId(entityId, contactId, workflowId);
            _repAapproval.DeleteApprover(entityId, contactId);
            _srvTimeLine.SaveWorflowTimeline(entityId, approver.WORKFLOW_ID, null, contactId, null,
          (int)WorkflowTimelineObject.Approver, (int)WorkflowTimelineEvents.Approver_Deleted, string.Empty, userId, DateTime.UtcNow, null, null);
        }
        public void DeleteReviewer(int entityId, int contactId, int userId, int workflowId)
        {
            
            var IsContactIsInWaiting = _repAapproval.IsUserInWaiting(entityId, workflowId, contactId);
            if(IsContactIsInWaiting.REVIEWER_STATUS == 2)
            {
              
                _repAapproval.DeleteReviewer(entityId, contactId);
                var WorkFlowReviewerDetails = _repAapproval.GetWorkFlowReviewers(entityId, workflowId);
                WorkFlowReviewerDetails.REVIEWER_STATUS = (byte)ApproverStatus.Waiting;
                var CustomWorkflow = _repAapproval.GetworkflowCustombyworkflowId(entityId, workflowId);
                CustomWorkflow.WORKFLOW_OWNER = WorkFlowReviewerDetails.CONTACT_ID;
                CustomWorkflow.IS_ROLE = WorkFlowReviewerDetails.IS_ROLE;
                _repAapproval.SaveApprover(WorkFlowReviewerDetails);
                _repCustomWorkflow.SaveCustomWorkFlow(CustomWorkflow);

            }
            else
            {
                _repAapproval.DeleteReviewer(entityId, contactId);
            }
            var date = DateTime.UtcNow;
            _srvTimeLine.SaveWorflowTimeline(entityId, workflowId, IsContactIsInWaiting.REVIEWER_STATUS, IsContactIsInWaiting.CONTACT_ID, null,
                  (int)WorkflowTimelineObject.Approver, (int)WorkflowTimelineEvents.Approver_Deleted, null, userId, date,
                  IsContactIsInWaiting.ORDER_ID, IsContactIsInWaiting.IS_ROLE);
        }
        /// <summary>
        /// Updates the approver order.
        /// </summary>
        /// <param name="objApprover">The object approver.</param>
        /// 

        public ApproverStatus PreApprovedApprover(int approverId, int contactId, int entityId, int workFlowId, int userId,string companyLogo)
        {
            var nextApprovalStatus = ApproverStatus.Approved;
            var objApprover = _repAapproval.GetApproverById(approverId);
            objApprover.REVIEWER_STATUS = (int)ApproverStatus.Approved;
            objApprover.CONTACT_ID = contactId;
            objApprover.USER_MODIFIED = userId;
            objApprover.DATE_MODIFIED = DateTime.UtcNow;
            _repAapproval.SaveApprover(objApprover);

            var approver = _repAapproval.GetUpcomingApproverByEntityId(entityId, workFlowId);
            if (approver != null)
            {
                approver.REVIEWER_STATUS = (byte)ApproverStatus.Waiting;
                nextApprovalStatus = ApproverStatus.Waiting;
                _repAapproval.SaveApprover(approver);
            }
            _srvTimeLine.SaveWorflowTimeline(entityId, workFlowId, (int)ApproverStatus.Approved, contactId, null,
    (int)WorkflowTimelineObject.Approver_Actions, (int)WorkflowTimelineEvents.Entity_Approed, APPROVER_NOTE, userId, DateTime.UtcNow, null, null);

            SaveNoteInMessages(APPROVER_NOTE,0, entityId, workFlowId, userId, (int)ApproverStatus.Approved,companyLogo);
            return nextApprovalStatus;
        }
        public void UpdateApproverOrder(WORKFLOW_REVIEWER[] objApprover)
        {
            // var noOfApproved = _repAapproval.GetNoOfApproved(objApprover[0].APPROVER_ENTITY_ID);
            //var orderId = noOfApproved + 1;

            foreach (var item in objApprover)
            {
                var approver = _repAapproval.GetApproverByContactId(item.WORKFLOW_ID, item.CONTACT_ID, item.WORKFLOW_DEFINITION_ID);
                if (approver != null)
                {
                    approver.ORDER_ID = item.ORDER_ID;
                    approver.REVIEWER_STATUS = (item.ORDER_ID == ORDER_NO ? (byte)ApproverStatus.Waiting : (byte)ApproverStatus.Upcoming);
                    _repAapproval.UpdateApproverOrder(approver);
                }
            }
        }
        /// <summary>
        /// Manages the approvers on approvals.
        /// </summary>
        /// <param name="Approvers">The approvers.</param>
        /// <param name="status">The bill status.</param>
        /// <param name="billId">The bill identifier.</param>
        public void ManageApproversOnApprovals(WORKFLOW_REVIEWER[] Approvers, string ApproverNote, int? WorkflowOwner, byte? status,
            int workflowDefinitionId, int workFlowId, int userId,string companyLogo)
        {
            var currentApprover = Approvers.Where(x => x.REVIEWER_STATUS == (int)ApproverStatus.Waiting).FirstOrDefault();
            if (status != 4)
            {
                if (Approvers != null)
                {
               
                    foreach (var item in Approvers)
                    {
                        var objApprover = _repAapproval.GetApproverById(item.REVIEWER_ID);
                        bool CheckFlag = false;
                        if (CheckFlag == false)
                        {
                            if (status == (int)FlowStatus.Denied)
                            {
                                objApprover.REVIEWER_STATUS = 5;
                                CheckFlag = true;
                                objApprover.REVIEWER_STATUS = (item.REVIEWER_STATUS != (int)ApproverStatus.Approved
                                    ? (item.REVIEWER_STATUS == (int)ApproverStatus.Waiting
                                    ? (byte)ApproverStatus.Denied : (byte)ApproverStatus.Upcoming) : (byte)ApproverStatus.Approved);
                            }
                            if (status == (int)FlowStatus.Approved)
                            {
                                objApprover.REVIEWER_STATUS = 4;
                                CheckFlag = true;
                                objApprover.REVIEWER_STATUS = (item.REVIEWER_STATUS != (int)ApproverStatus.Approved
                                    ? (item.REVIEWER_STATUS == (int)ApproverStatus.Waiting
                                    ? (byte)ApproverStatus.Approved : (byte)ApproverStatus.Upcoming) : (byte)ApproverStatus.Approved);
                            }
                            if (status == (int)FlowStatus.Assigned)
                            {
                                objApprover.REVIEWER_STATUS = 1;
                                CheckFlag = true;
                                objApprover.REVIEWER_STATUS = (item.REVIEWER_STATUS != (int)ApproverStatus.Approved
                                    ? (item.REVIEWER_STATUS == (int)ApproverStatus.Waiting
                                    ? (byte)ApproverStatus.Assigned : (byte)ApproverStatus.Upcoming) : (byte)ApproverStatus.Approved);
                            }
                        }
                        objApprover.CONTACT_ID = item.CONTACT_ID;
                        objApprover.USER_MODIFIED = item.USER_MODIFIED;
                        objApprover.DATE_MODIFIED = (item.REVIEWER_STATUS != (byte)ApproverStatus.Waiting ? objApprover.DATE_MODIFIED : DateTime.UtcNow);
                        _repAapproval.SaveApprover(objApprover);
                    }
                }
           
            //var approver = _repAapproval.GetUpcomingApproverByEntityId(entityId, workFlowId);
            //if (approver != null && status != (int)FlowStatus.Denied)
            //{
            //    approver.REVIEWER_STATUS = (byte)ApproverStatus.Waiting;
            //    _repAapproval.SaveApprover(approver);
            //}
            var approver = _repAapproval.GetApproverByworkflowId(WorkflowOwner, workFlowId);
            if(approver != null)
            {
               // if(status == 2)
              //  {
                 approver.REVIEWER_STATUS = 2;
                //}
                //if(status == 3)
                //{
                //    approver.REVIEWER_STATUS = 5;
                //}
                
                approver.DATE_MODIFIED = DateTime.UtcNow;
                approver.USER_MODIFIED = approver.USER_ID;
                _repAapproval.SaveApprover(approver);
            }


            }
            if (WorkflowOwner != null)
            {
                var CustomWorkflow = _repAapproval.GetworkflowCustombyworkflowId(workFlowId, workflowDefinitionId);
                var WorkflowOWnerIsRole = _repAapproval.GetworkflowOwnerIsRole(workFlowId, WorkflowOwner);
                if (status == 4)
                {
                    CustomWorkflow.WORKFLOW_STATUS = 3;

                }
                else
                {

                    if (CustomWorkflow.WORKFLOW_STATUS == 3)
                    {
                        CustomWorkflow.WORKFLOW_STATUS = 2;
                    }
                    if (CustomWorkflow.WORKFLOW_STATUS == 1)
                    {
                        CustomWorkflow.WORKFLOW_STATUS = 2;
                    }
                    if(WorkflowOWnerIsRole == true)
                    {

                        CustomWorkflow.WORKFLOW_OWNER = WorkflowOwner;
                        CustomWorkflow.IS_ROLE = true;
                    }
                    else
                    {
                        CustomWorkflow.WORKFLOW_OWNER = WorkflowOwner;
                        CustomWorkflow.IS_ROLE = false;
                    }
                   
                }
            
                CustomWorkflow.DATE_MODIFIED = DateTime.UtcNow;
                var approverStatus = _repAapproval.GetApproverStatusByworkflowId(workFlowId);
                var StatusFlag = false;
                foreach (var item in approverStatus)
                {
                    if (item.REVIEWER_STATUS == 4)
                    {
                        StatusFlag = true;
                    }
                    else
                    {
                        StatusFlag = false;
                        break;
                    }
                }
                if(StatusFlag == true)
                {
                    CustomWorkflow.WORKFLOW_STATUS = 4;
                }
                else
                {
                    if(CustomWorkflow.WORKFLOW_STATUS == 4) {
                        CustomWorkflow.WORKFLOW_STATUS = 2;
                    }
                    
                }
                _repCustomWorkflow.SaveCustomWorkFlow(CustomWorkflow);
            }
            if (status == (int)FlowStatus.Approved)
            {
                if (currentApprover == null)
                {

                    _srvTimeLine.SaveWorflowTimeline(workFlowId, workflowDefinitionId, (int)status, userId, null,
                (int)WorkflowTimelineObject.Approver_Actions, (int)WorkflowTimelineEvents.Entity_Approed, ApproverNote, userId, DateTime.UtcNow, null, null);
                }
                else
                {
                    _srvTimeLine.SaveWorflowTimeline(workFlowId, workflowDefinitionId, (int)status, currentApprover.CONTACT_ID, null,
            (int)WorkflowTimelineObject.Approver_Actions, (int)WorkflowTimelineEvents.Entity_Approed, ApproverNote, userId, DateTime.UtcNow, null, null);
                }
            }
            if (status == (int)FlowStatus.Denied)
            {
                if (currentApprover == null)
                {
                    _srvTimeLine.SaveWorflowTimeline(workFlowId, workflowDefinitionId, (int)status, userId, null,
                    (int)WorkflowTimelineObject.Approver_Actions, (int)WorkflowTimelineEvents.Entity_Denied, ApproverNote, userId, DateTime.UtcNow, null, null);
                }
                else
                {
                    _srvTimeLine.SaveWorflowTimeline(workFlowId, workflowDefinitionId, (int)status, currentApprover.CONTACT_ID, null,
                    (int)WorkflowTimelineObject.Approver_Actions, (int)WorkflowTimelineEvents.Entity_Denied, ApproverNote, userId, DateTime.UtcNow, null, null);
                }
        
            }
            if (status == (int)FlowStatus.Assigned)
            {
                if (currentApprover == null)
                {
                    _srvTimeLine.SaveWorflowTimeline(workFlowId, workflowDefinitionId, (int)status, userId, null,
                      (int)WorkflowTimelineObject.Approver_Actions, (int)WorkflowTimelineEvents.Entity_Assigned, ApproverNote, userId, DateTime.UtcNow, null, null);
                }
                else
                {
                    _srvTimeLine.SaveWorflowTimeline(workFlowId, workflowDefinitionId, (int)status, currentApprover.CONTACT_ID, null,
                  (int)WorkflowTimelineObject.Approver_Actions, (int)WorkflowTimelineEvents.Entity_Assigned, ApproverNote, userId, DateTime.UtcNow, null, null);
                }
               
            }
            if (status == (int)FlowStatus.Draft)
            {
                if (currentApprover == null)
                {
                    _srvTimeLine.SaveWorflowTimeline(workFlowId, workflowDefinitionId, (int)status, userId, null,
                 (int)WorkflowTimelineObject.Approver_Actions, (int)WorkflowTimelineEvents.Entity_OnHOld, ApproverNote, userId, DateTime.UtcNow, null, null);
                }
                else
                {
                    _srvTimeLine.SaveWorflowTimeline(workFlowId, workflowDefinitionId, (int)status, currentApprover.CONTACT_ID, null,
              (int)WorkflowTimelineObject.Approver_Actions, (int)WorkflowTimelineEvents.Entity_OnHOld, ApproverNote, userId, DateTime.UtcNow, null, null);
                }
  
            }
            SaveNoteInMessages(ApproverNote, WorkflowOwner,workflowDefinitionId, workFlowId, userId, status, companyLogo);
        }
        /// <summary>
        /// Manages the approver note.
        /// </summary>
        /// <param name="objBillingView">The object billing view.</param>
        /// <param name="billId">The bill identifier.</param>
        public void ManageApproverNote(WF_BILLING_DETAIL objBillingView, int entityId)
        {
            if (!string.IsNullOrEmpty(objBillingView.NOTE_TO_PAYER))
            {
                var objApproverNote = new ApproverNoteViewModel();
                objApproverNote.ENTITY_ID = entityId;
                objApproverNote.CONTACT_ID = objBillingView.USER_ID;
                objApproverNote.APPROVER_NOTE = objBillingView.NOTE_TO_PAYER;
                objApproverNote.USER_MODIFIED = objBillingView.USER_ID;
                objApproverNote.USER_ID = objBillingView.USER_ID;
                SaveApproverNote(objApproverNote);
            }
        }
        /// <summary>
        /// Saves the approver note.
        /// </summary>
        /// <param name="objApproverNoteViewModel">The object approver note view model.</param>
        public void SaveApproverNote(ApproverNoteViewModel objApproverNoteViewModel)
        {
            var objApproverNote = new WF_APPROVER_NOTE();
            objApproverNote.APPROVER_NOTE_ID = objApproverNoteViewModel.APPROVER_NOTE_ID;
            objApproverNote.ENTITY_ID = objApproverNoteViewModel.ENTITY_ID;
            objApproverNote.CONTACT_ID = objApproverNoteViewModel.CONTACT_ID;
            objApproverNote.APPROVER_NOTE = objApproverNoteViewModel.APPROVER_NOTE;
            objApproverNote.USER_ID = objApproverNoteViewModel.USER_ID;
            objApproverNote.DATE_OF_ENTRY = DateTime.UtcNow;
            objApproverNote.USER_MODIFIED = objApproverNoteViewModel.USER_MODIFIED;
            objApproverNote.DATE_MODIFIED = DateTime.UtcNow;
            objApproverNote.DELETED_FLAG = false;
            _repAapproval.SaveApproverNote(objApproverNote);

        }
        /// <summary>
        /// Determines whether [is any pending aprrover] [the specified bill identifier].
        /// </summary>
        /// <param name="entityId">The bill identifier.</param>
        /// <param name="workflowId">The workflow identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is any pending aprrover] [the specified bill identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAnyPendingAprrover(int entityId, int workflowId)
        {
            return _repAapproval.IsAnyPendingAprrover(entityId, workflowId);
        }

        /// <summary>
        /// Determines whether [is current user pre approver] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="approvers">The approvers.</param>
        /// <returns>
        ///   <c>true</c> if [is current user pre approver] [the specified user identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrentUserPreApprover(int userId, WORKFLOW_REVIEWER[] approvers)
        {
            var isPreApprover = false;
            var isBreakLoop = false;
            if (approvers == null)
            {
                return false;
            }
            foreach (var item in approvers)
            {
                var isUserInRole = _srvAdmin.IsUserInRole(item.CONTACT_ID, userId);
                if ((item.CONTACT_ID == userId || isUserInRole) && !isBreakLoop)
                {
                    isPreApprover = true;
                }
                else
                {
                    isBreakLoop = true;
                }
            }
            return isPreApprover;
        }

        /// <summary>
        /// Determines whether [is pre approver email notification] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="approvers">The approvers.</param>
        /// <returns>
        ///   <c>true</c> if [is pre approver email notification] [the specified user identifier]; otherwise, <c>false</c>.
        /// </returns>
        public List<int> IsPreApproverEmailNotification(int userId, WORKFLOW_REVIEWER[] approvers)
        {
            List<int> users = new List<int>();
            if (approvers == null)
            {
                return users;
            }
            var isBreakLoop = false;
            foreach (var item in approvers)
            {
                var isUserInRole = _srvAdmin.IsUserInRole(item.CONTACT_ID, userId);

                if ((item.CONTACT_ID == userId || isUserInRole) && !isBreakLoop)
                {
                    users = _srvAdmin.GetRoleUsers(item.CONTACT_ID);
                }
                else
                {
                    isBreakLoop = true;
                }
            }
            return users;
        }

        /// <summary>
        /// Gets the next approver.
        /// </summary>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        public WORKFLOW_REVIEWER GetNextApprover(int workFlowId, int entityId)
        {
            return _repAapproval.GetNextApprover(workFlowId, entityId);
        }

        /// <summary>
        /// Gets the role users.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public List<int> GetRoleUsers(int roleId)
        {
            return _srvAdmin.GetRoleUsers(roleId);
        }

        public List<int> GetWorkflowUsers(int entityId, int workFlowId)
        {
            List<int> users = new List<int>();
            var workflowInitiator = _repAapproval.GetWorkflowInitiator(entityId, workFlowId);
            users.Add(workflowInitiator);
            var existingApprovers = _repAapproval.GetEntityApprovers(entityId, workFlowId);
            foreach (var approver in existingApprovers)
            {
                if (approver.IS_ROLE.HasValue)
                {
                    if (approver.IS_ROLE.Value == true)
                    {
                        var roleUsers = GetRoleUsers(approver.CONTACT_ID);
                        foreach (var user in roleUsers)
                        {
                            if (workflowInitiator != user) { users.Add(user); }                            
                        }
                    }
                    else
                    {
                        if (workflowInitiator != approver.CONTACT_ID) { users.Add(approver.CONTACT_ID); }
                    }
                }
                else
                {
                    if (workflowInitiator != approver.CONTACT_ID) { users.Add(approver.CONTACT_ID); } 
                }
            }            
            return users;
        }
        public List<WF_APPROVER_NOTE> GetApproverNotes(int customWorkflowId)
        {
            var GetApporvalNotes = _repAapproval.GetApproverNotes(customWorkflowId);
            return GetApporvalNotes;
        }
        public void DragAndDropInReviewerTable(int destinationId, int sourceId, int _userId,int WORKFLOW_DEFINITION_ID , int WORKFLOW_ID)
        {

            var Reviewers = _repAapproval.GetReviewerByWorkFlowIDd(WORKFLOW_DEFINITION_ID);
            var SourceField = Reviewers.Where(x => x.REVIEWER_ID == sourceId).FirstOrDefault();
            var sourceFieldOrderId = SourceField.ORDER_ID;
            var destinationField = Reviewers.Where(x => x.REVIEWER_ID == destinationId).FirstOrDefault();
            var destinationFieldOrderId = destinationField.ORDER_ID;
            if (sourceFieldOrderId == destinationFieldOrderId)
            {
                return;
            }
           // byte newStatus = (byte)((destinationFieldOrderId == 1) ? ApproverStatus.Waiting : ApproverStatus.Upcoming);
            if (sourceFieldOrderId > destinationFieldOrderId)
            {
                var destinationFieldOrders = Reviewers.Where(x => x.ORDER_ID >= destinationFieldOrderId && x.ORDER_ID < sourceFieldOrderId);
                foreach (var destinationFieldOrder in destinationFieldOrders)
                {
                    destinationFieldOrder.ORDER_ID += 1;
                    destinationFieldOrder.USER_MODIFIED = _userId;
                    destinationFieldOrder.DATE_MODIFIED = DateTime.UtcNow;

                  //  destinationFieldOrder.REVIEWER_STATUS = (destinationFieldOrder.ORDER_ID == 1) ? (byte)ApproverStatus.Waiting : (byte)ApproverStatus.Upcoming;
                }
                SourceField.ORDER_ID = destinationFieldOrderId;
                SourceField.USER_MODIFIED = _userId;
                SourceField.DATE_OF_ENTRY = DateTime.UtcNow;

               // SourceField.REVIEWER_STATUS = newStatus;
            }
            if (sourceFieldOrderId < destinationFieldOrderId)
            {
                var sourceFieldOrders = Reviewers.Where(x => x.ORDER_ID > sourceFieldOrderId && x.ORDER_ID <= destinationFieldOrderId);
                foreach (var sourceFieldOrder in sourceFieldOrders)
                {
                    sourceFieldOrder.ORDER_ID -= 1;
                    sourceFieldOrder.USER_MODIFIED = _userId;
                    sourceFieldOrder.DATE_MODIFIED = DateTime.UtcNow;

                   // sourceFieldOrder.REVIEWER_STATUS = (sourceFieldOrder.ORDER_ID == 1) ? (byte)ApproverStatus.Waiting : (byte)ApproverStatus.Upcoming;
                }
                SourceField.ORDER_ID = destinationFieldOrderId;
                SourceField.USER_MODIFIED = _userId;
                SourceField.DATE_OF_ENTRY = DateTime.UtcNow;

               // SourceField.REVIEWER_STATUS = newStatus;
            }
            //foreach (var reviewer in Reviewers)
            //{
            //    reviewer.REVIEWER_STATUS = (reviewer.ORDER_ID == 1) ? (byte)ApproverStatus.Waiting : (byte)ApproverStatus.Upcoming;
            //}
            _repAapproval.SaveChanges1();
            var WfReviewers = _repAapproval.GetApproverStatusByworkflowId(WORKFLOW_ID);
         
            var checkflagApproved = true;
            var checkflagReject = true;
            var checkflagWaiting = true;
            var checkflagupcoming = true;
            var checkflagAssigned = true;
            var orderIdApproved = 0;
            var oderIdReject = 0;
            var orderIdwaiting = 0;
            var orderIdupcoming = 0;
            var orderIdAssigned = 0;
            var ContactIdApproved = 0;
            var ContactIdReject = 0;
            var ContactIdwaiting = 0;
            var ContactIdupcoming = 0;
            var ContactIdAssigned = 0;
            for (int i = 0; i < WfReviewers.Count; i++)
            {
                if(WfReviewers[i].REVIEWER_STATUS == 4)
                {
                    if(checkflagApproved == true)
                    {
                        orderIdApproved = WfReviewers[i].ORDER_ID.Value;
                        ContactIdApproved = WfReviewers[i].CONTACT_ID;
                        checkflagApproved = false;
                    }
                }
                
                if(WfReviewers[i].REVIEWER_STATUS == 5)
                {
                    if(checkflagReject == true)
                    {
                        oderIdReject = WfReviewers[i].ORDER_ID.Value;
                        ContactIdReject = WfReviewers[i].CONTACT_ID;
                        checkflagReject = false;
                    }
                }
                if(WfReviewers[i].REVIEWER_STATUS == 2)
                {
                    if(checkflagWaiting == true)
                    {
                        orderIdwaiting = WfReviewers[i].ORDER_ID.Value;
                        ContactIdwaiting = WfReviewers[i].CONTACT_ID;
                        checkflagWaiting = false;
                    }
                }
                
                if(WfReviewers[i].REVIEWER_STATUS == 3)
                {
                    if(checkflagupcoming == true)
                    {
                        orderIdupcoming = WfReviewers[i].ORDER_ID.Value;
                        ContactIdupcoming = WfReviewers[i].CONTACT_ID;
                        checkflagupcoming = false;
                    }
                }
                
                if(WfReviewers[i].REVIEWER_STATUS == 1)
                {
                    if(checkflagAssigned == true)
                    {
                        orderIdAssigned = WfReviewers[i].ORDER_ID.Value;
                        ContactIdAssigned = WfReviewers[i].CONTACT_ID;
                        checkflagAssigned = false;
                    }
                }
                
            }

            if(orderIdupcoming < orderIdwaiting)
            {
                var upcomingReviewer = _repAapproval.GetApproverByworkflowId(ContactIdupcoming, WORKFLOW_ID);
                if(upcomingReviewer!=null)
                {
                    upcomingReviewer.REVIEWER_STATUS = 2;
                    _repAapproval.UpdateApproverOrder(upcomingReviewer);
                    var workFlow = new WORKFLOW_CUSTOM();
                    workFlow = _repCustomWorkflow.GetCustomWorkFlow(WORKFLOW_ID);
                    workFlow.WORKFLOW_OWNER = ContactIdupcoming;
                    _repCustomWorkflow.SaveCustomWorkFlow(workFlow);
                }

                var waitingReviewer = _repAapproval.GetApproverByworkflowId(ContactIdwaiting, WORKFLOW_ID);
                if (waitingReviewer != null)
                {
                    waitingReviewer.REVIEWER_STATUS = 3;
                    _repAapproval.UpdateApproverOrder(waitingReviewer);
                }

            }

        }

        public WORKFLOW_DEFINITION GetCustomWorkflowIsSingleOrNot(int customWorkflowId)
        {
            return _repAapproval.GetCustomWorkflowIsSingleOrNot( customWorkflowId);
        }
        //public WORKFLOW_CUSTOM GetCheckWorkflowMultipleEntries(int customWorkflowId)
        //{
        //    return _repAapproval.GetCheckWorkflowMultipleEntries( customWorkflowId);
        //}
    }
}
