using SOL.Addressbook.Interfaces;
using SOL.Common.Models;
using SOL.PMS.Interfaces;
using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Services
{
    public class WorkFlowPayrollService<T> : IWorkFlowPayrollService<T>
    {
        IWorkFlowPayrollRepository<T> _repPayroll = null;
        IAddressbookRepository<T> _repAdrbook;
        IApprovalService<T> _srvApproval = null;
        IWorkflowDocumentService<T> _srvWfDocumentService = null;
        IPmsService<T> _srvPms;
        IWorkFlowRepository<T> _repWorkFlow;
        IWorkflowTimelineService<T> _srvTimeLine = null;
        public WorkFlowPayrollService(IWorkFlowPayrollRepository<T> repPayroll, IAddressbookRepository<T> repAdrbook,
            IApprovalService<T> srvApproval, IWorkflowDocumentService<T> srvWfDocumentService, IPmsService<T> srvPms,
             IWorkFlowRepository<T> repWorkFlow, IWorkflowTimelineService<T> srvTimeLine)
        {
            _repPayroll = repPayroll;
            this._repAdrbook = repAdrbook;
            _srvApproval = srvApproval;
            this._srvWfDocumentService = srvWfDocumentService;
            this._srvPms = srvPms;
            this._repWorkFlow = repWorkFlow;
            this._srvTimeLine = srvTimeLine;
        }

        public object SavePayroll(WF_PAYROLL payroll, int userId, UserType userType, string addedBy, ref string errorMessage, string companyLogo, string companyUrl)
        {
            var workFlowId = (int)CustomWorkflowTypes.PAYROLL;
            var payrollId = payroll.PAYROLL_ID;
            var oldPayrollId = payrollId;
            bool genrateDocument = true;
            bool isCheckOriginalName = false;
            var isClosePage = true;
            var loadNextPage = true;
            byte ByOverView = 0;
            if (payroll.PAYROLL_ID != default(int))
            {
                genrateDocument = false;
            }
            var associtaedPages = payroll.AssocitaedPages;
            int projectId = _srvPms.GetProjectIdByClientId(payroll.CLIENT_ID);
            if (payroll.PAYROLL_ID == default(int))
            {
                payroll.DELETED_FLAG = false;
                payroll.USER_ID = userId;
                payroll.USER_MODIFIED = userId;
                payroll.DATE_OF_ENTRY = DateTime.UtcNow;
                payroll.DATE_MODIFIED = DateTime.UtcNow;
                _repPayroll.SavePayroll(payroll);
                payrollId = payroll.PAYROLL_ID;
                _srvTimeLine.SaveWorflowTimeline(payroll.PAYROLL_ID, workFlowId, payroll.APPROVAL_STATUS, null, null,
                    (int)WorkflowTimelineObject.Payroll, (int)WorkflowTimelineEvents.Payroll_Created, string.Empty, userId, DateTime.UtcNow, null, null);
            }
            else
            {
                var orignalPayroll = _repPayroll.GetPayroll(payroll.PAYROLL_ID);
                payroll.CLIENT_ID = orignalPayroll.CLIENT_ID;
                orignalPayroll.APPROVAL_STATUS = payroll.APPROVAL_STATUS;
                orignalPayroll.PAYROLL_DATE = payroll.PAYROLL_DATE;
                orignalPayroll.TITLE = payroll.TITLE;
                orignalPayroll.DESCRIPTION = payroll.DESCRIPTION;
                orignalPayroll.DATE_MODIFIED = DateTime.UtcNow;
                orignalPayroll.USER_MODIFIED = userId;
                if (payroll.APPROVAL_STATUS == (int)FlowStatus.Assigned)
                {
                    loadNextPage = false;
                }
                _repPayroll.SavePayroll(orignalPayroll);
                _srvTimeLine.SaveWorflowTimeline(payroll.PAYROLL_ID, workFlowId, payroll.APPROVAL_STATUS, null, null,
                (int)WorkflowTimelineObject.Payroll, (int)WorkflowTimelineEvents.Payroll_Updated, string.Empty, userId, DateTime.UtcNow, null, null);
            }
            SavePayrollLineItems(payrollId, payroll.PayrollLineItems, userId);
            if (payroll.APPROVAL_STATUS != (int)FlowStatus.Draft)
            {
                _srvApproval.SaveApprovers(payroll.Approvers, payroll.NOTE_TO_PAYER, payroll.PAYROLL_ID,
                    workFlowId, userId, payroll.Status, ByOverView, companyLogo);
            }
            var newDocId = 0;

            var documentName = string.Empty;
            var clientName = _repAdrbook.GetCompanyNameByCompanyId(payroll.CLIENT_ID);
            var docId = payroll.DOCUMENT_ID;
            var originelDocId = docId;
            var documentId = payroll.documentId;
            if (clientName != null)
            {
                documentName = payroll.TITLE;
            }
            _srvWfDocumentService.ManageBillDocuments(payrollId, workFlowId, addedBy, (byte)payroll.APPROVAL_STATUS,
         genrateDocument, isCheckOriginalName, associtaedPages, userId, userType, ref errorMessage, projectId, docId, ref originelDocId,
         ref newDocId, ref isClosePage, payroll.IsApproveMode, documentName);
            if (oldPayrollId == default(int))
            {
                _srvWfDocumentService.SaveDocumentEntity(workFlowId, payrollId, documentId, newDocId, userId);
            }
            if (newDocId == default(int))
            {
                newDocId = originelDocId;
            }
            SendPreEmailNotification(payroll.Approvers, userId, workFlowId, payrollId, companyLogo, companyUrl);
            var obj = new object();
            obj = new
            {
                originelDocId = originelDocId,
                isClosePage = isClosePage,
                payrollId = payrollId,
                newDocId = newDocId,
                loadNextPage = loadNextPage
            };
            return obj;
        }
        private void SavePayrollLineItems(int payrollId, List<WF_PAYROLL_LINE_ITEMS> payrollLineItems, int userId)
        {
            var lineItems = new List<WF_PAYROLL_LINE_ITEMS>();
            foreach (var payrollLineItem in payrollLineItems)
            {
                if (payrollLineItem.PAYROLL_LINE_ITEM_ID == default(int))
                {
                    payrollLineItem.PAYROLL_ID = payrollId;
                    payrollLineItem.DELETED_FLAG = false;
                    payrollLineItem.USER_ID = userId;
                    payrollLineItem.USER_MODIFIED = userId;
                    payrollLineItem.DATE_OF_ENTRY = DateTime.UtcNow;
                    payrollLineItem.DATE_MODIFIED = DateTime.UtcNow;
                    lineItems.Add(payrollLineItem);
                }
                else
                {
                    var orignalPayrollLineItem = _repPayroll.GetPayrollLineItem(payrollLineItem.PAYROLL_LINE_ITEM_ID);
                    orignalPayrollLineItem.REQUESTED_HOURS = payrollLineItem.REQUESTED_HOURS;
                    orignalPayrollLineItem.OVERTIME_HOURS = payrollLineItem.OVERTIME_HOURS;
                    orignalPayrollLineItem.TIME_OFF = payrollLineItem.TIME_OFF;
                    orignalPayrollLineItem.DESIGNATION = payrollLineItem.DESIGNATION;
                    orignalPayrollLineItem.DATE_MODIFIED = DateTime.UtcNow;
                    orignalPayrollLineItem.USER_MODIFIED = userId;
                    lineItems.Add(orignalPayrollLineItem);
                }
            }
            _repPayroll.SavePayrollLineItem(lineItems);
        }

        /// <summary>
        /// Approves the bill status.
        /// </summary>
        /// <param name="objBillingViewModel">The object billing view model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        public void ApprovePayrollStatus(UpdateStatusViewModel updateStatusViewModel, int userId, UserType userType, string companyLogo, string companyUrl)
        {
            string errorMessage = string.Empty;
            var workFlowId = (int)CustomWorkflowTypes.PAYROLL;
            var flowStatus = updateStatusViewModel.Status;
            var entityId = updateStatusViewModel.EntityId;
            _srvApproval.ManageApproversOnApprovals(updateStatusViewModel.Approvers, updateStatusViewModel.ApproverNote,null,
                flowStatus, entityId, workFlowId, userId, companyLogo);
            var payroll = _repPayroll.GetPayroll(entityId);
            var isAnyPendingAprrovers = _srvApproval.IsAnyPendingAprrover(entityId, workFlowId);
            var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, updateStatusViewModel.WorkFlowId);
            if (flowStatus == (int)FlowStatus.Denied)
            {
                payroll.APPROVAL_STATUS = flowStatus.Value;
                _srvWfDocumentService.MoVeDocumentToDeniedFolder(userId, userType, ref errorMessage,
                       docId, docId, workFlowId);
            }
            else if (isAnyPendingAprrovers == false)
            {
                payroll.APPROVAL_STATUS = (int)flowStatus;

                _srvWfDocumentService.MoVeDocumentToCompletedFolder(userId, userType, ref errorMessage,
                        docId, docId, "Payroll", workFlowId);
            }
            payroll.USER_MODIFIED = userId;
            payroll.DATE_MODIFIED = DateTime.UtcNow;
            _repPayroll.SavePayroll(payroll);
            if (flowStatus == (int)FlowStatus.Denied)
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                  companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Denied, false, null);
            }
            else if (isAnyPendingAprrovers == false || flowStatus == (int)FlowStatus.Denied)
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                 companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Approved, false, null);

                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                 companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.completed, false, null);
            }
            else
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                 companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Approved, false, null);

                var approver = _srvApproval.GetNextApprover(workFlowId, entityId);
                if (approver != null)
                {
                    if (approver.IS_ROLE.Value == true)
                    {
                        var users = _srvApproval.GetRoleUsers(approver.CONTACT_ID);
                        if (users.Count() > 0)
                        {
                            foreach (var contactId in users)
                            {
                                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(contactId, workFlowId, entityId, companyUrl,
                                companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, false, null);
                            }
                        }
                    }
                    else
                    {
                        _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(approver.CONTACT_ID, workFlowId, entityId, companyUrl,
                            companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, false, null);
                    }
                }
            }
        }

        private void SendPreEmailNotification(WORKFLOW_REVIEWER[] approvers, int userId,
       int workFlowId, int entityId, string companyLogo, string companyUrl)
        {
            var isPreApproveUser = _srvApproval.IsCurrentUserPreApprover(userId, approvers);
            if (isPreApproveUser)
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                         companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, false, null);
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                        companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Approved, false, null);
            }
            var preApprovedUsers = _srvApproval.IsPreApproverEmailNotification(userId, approvers);
            if (preApprovedUsers.Count() > 0)
            {
                foreach (var contactId in preApprovedUsers)
                {
                    if (contactId != userId)
                    {
                        _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(contactId, workFlowId, entityId, companyUrl,
                       companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, true, userId);

                        _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(contactId, workFlowId, entityId, companyUrl,
                         companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Approved, true, userId);
                    }
                }
            }
            var approver = _srvApproval.GetNextApprover(workFlowId, entityId);
            if (approver != null)
            {
                if (approver.IS_ROLE.Value == true)
                {
                    var users = _srvApproval.GetRoleUsers(approver.CONTACT_ID);
                    if (users.Count() > 0)
                    {
                        foreach (var contactId in users)
                        {
                            _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(contactId, workFlowId, entityId, companyUrl,
                            companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, false, null);
                        }
                    }
                }
                else
                {
                    _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(approver.CONTACT_ID, workFlowId, entityId, companyUrl,
                        companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, false, null);
                }
            }
            var isAnyPendingAprrovers = _srvApproval.IsAnyPendingAprrover(entityId, workFlowId);
            if (isAnyPendingAprrovers == false)
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                           companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.completed, false, null);
            }
        }


        public WF_PAYROLL GetPayroll(int payrollId)
        {
            return _repPayroll.GetPayroll(payrollId);
        }
    }
}
