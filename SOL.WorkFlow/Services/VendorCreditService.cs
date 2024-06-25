using SOL.Addressbook.Interfaces;
using SOL.Common.Models;
using SOL.Common.Business.Interfaces;
using SOL.ECM.Interfaces;
using SOL.ECM.Models;
using SOL.PMS.Interfaces;
using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Services
{
    public class VendorCreditService<T> : IVendorCreditService<int>
    {
        IVendorCreditRepository<T> _repVendorCredit;
        IWorkFlowRepository<T> _repWorkFlow;
        IApprovalService<T> _srvApproval = null;
        IChartsAccountService<T> _srvChartsAccount = null;
        IPayrollRepository<T> _repPayroll;
        private IPmsService<T> _srvPms;
        IAddressbookService<int> _srvAddressbook = null;
        IWorkflowDocumentService<T> _srvWfDocumentService = null;
        IWorkflowTimelineService<T> _srvTimeLine = null;

        public VendorCreditService(IWorkFlowRepository<T> repWorkFlow, IPayrollRepository<T> repPayroll, IWorkflowDocumentService<T> srvWfDocumentService, IApprovalService<T> srvApproval, IChartsAccountService<T> srvChartsAccount, IVendorCreditRepository<T> repVendorCredit,
             IPmsService<T> srvPms, IAddressbookService<int> srvAddressbook, IWorkflowTimelineService<T> srvTimeLine)
        {
            this._repVendorCredit = repVendorCredit;
            this._srvPms = srvPms;
            this._srvAddressbook = srvAddressbook;
            this._repWorkFlow = repWorkFlow;
            this._srvChartsAccount = srvChartsAccount;
            this._srvApproval = srvApproval;
            this._srvWfDocumentService = srvWfDocumentService;
            this._repPayroll = repPayroll;
            this._srvTimeLine = srvTimeLine;
        }

        public WF_VENDOR_CREDIT_DETAIL GetVendorCreditById(int vendorCredit)
        {
            return _repVendorCredit.GetVendorCreditById(vendorCredit);
        }

        public int ApproveVenorCreditStatus(UpdateStatusViewModel objVenorCreditViewModel, int userId, UserType userType, string companyLogo, string companyUrl)
        {
            var workFlowId = (int)CustomWorkflowTypes.VENDOR_CREDIT;
            string errorMessage = string.Empty;
            var VendorCreditStatus = objVenorCreditViewModel.Status;
            var entityId = objVenorCreditViewModel.EntityId;
            _srvChartsAccount.SaveBillBankAccount(objVenorCreditViewModel.BillingAccounts, entityId, 1, userId);
            _srvApproval.ManageApproversOnApprovals(objVenorCreditViewModel.Approvers, objVenorCreditViewModel.ApproverNote,null,
                VendorCreditStatus, entityId, workFlowId, userId, companyLogo);
            var VendorCreditDetail = _repVendorCredit.GetVendorCreditById(entityId);
            var isAnyPendingAprrovers = _srvApproval.IsAnyPendingAprrover(entityId, workFlowId);
            var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, objVenorCreditViewModel.WorkFlowId);
            if (VendorCreditStatus == (int)FlowStatus.Denied)
            {
                VendorCreditDetail.APPROVAL_STATUS = VendorCreditStatus;
                _srvWfDocumentService.MoVeDocumentToDeniedFolder(userId, userType, ref errorMessage,
                       docId, docId, workFlowId);
            }
            else if (isAnyPendingAprrovers == false || VendorCreditStatus == (int)FlowStatus.Denied)
            {
                VendorCreditDetail.APPROVAL_STATUS = VendorCreditStatus;
                if (docId != 0)
                {
                    var vendorId = _repVendorCredit.GetVenderIdByVendorCreditId(entityId);
                    var vendorName = _repPayroll.GetVendorNameByVendorId(vendorId);
                    _srvWfDocumentService.MoVeDocumentToCompletedFolder(userId, userType, ref errorMessage,
                            docId, docId, vendorName, workFlowId);
                }
            }
            VendorCreditDetail.USER_MODIFIED = userId;
            VendorCreditDetail.DATE_MODIFIED = DateTime.UtcNow;
            _repVendorCredit.UpdateApproverStatsu(VendorCreditDetail);

            if (VendorCreditStatus == (int)FlowStatus.Denied)
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                  companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Denied, false, null);
            }
            else if (isAnyPendingAprrovers == false || VendorCreditStatus == (int)FlowStatus.Denied)
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
            return entityId;
        }

        public object SaveVendorCredit(VendorCreditViewModel objVendorCreditModel, string addedBy, string companyLogo, string companyUrl, ref string errorMessage)
        {
            var approvalStatus = objVendorCreditModel.VendorCreditDetail.APPROVAL_STATUS;
            var workFlowId = (int)CustomWorkflowTypes.VENDOR_CREDIT;
            bool genrateDocument = true;
            bool isCheckOriginalName = false;
            var documentOperation = new DocumentOperation();
            var objVendorCredit = objVendorCreditModel.VendorCreditDetail;
            if (objVendorCredit.VENDOR_CREDIT_ID != default(int))
            {
                genrateDocument = false;
            }
            var associtaedPages = objVendorCreditModel.AssocitaedPages;
            var userId = objVendorCredit.USER_ID;
            var userType = objVendorCreditModel.UserType;
            //var errorMessage = string.Empty;
            var vcId = objVendorCredit.VENDOR_CREDIT_ID;
            var oldVendorCreditId = vcId;
            var clientId = objVendorCredit.CLIENT_ID;// objBillingView.VENDOR_ID;
            int projectId = _srvPms.GetProjectIdByClientId(clientId.Value);
            var docId = objVendorCredit.DOCUMENT_ID;
            var documentId = objVendorCreditModel.documentId;
            var originelDocId = docId;
            var newDocId = 0;
            var isClosePage = true;
            var IsApproveMode = objVendorCreditModel.IsApproveMode;
            int entityId = SaveVendorCreditDetails(objVendorCredit);
            byte ByOverView = 0;
            if (genrateDocument == true)
            {
                _srvTimeLine.SaveWorflowTimeline(entityId, workFlowId, approvalStatus, null, null,
                (int)WorkflowTimelineObject.VendorCredit, (int)WorkflowTimelineEvents.Vendor_Credit_Created, string.Empty, userId, DateTime.UtcNow, null, null);
            }
            else
            {
                _srvTimeLine.SaveWorflowTimeline(entityId, workFlowId, approvalStatus, null, null,
                (int)WorkflowTimelineObject.VendorCredit, (int)WorkflowTimelineEvents.Vendor_Credit_Updated, string.Empty, userId, DateTime.UtcNow, null, null);
            }
            _srvChartsAccount.SaveBillBankAccount(objVendorCreditModel.VendorCreditAccounts, entityId, workFlowId, userId);
            if (approvalStatus != (int)FlowStatus.Draft)
            {
                _srvApproval.SaveApprovers(objVendorCreditModel.Approvers, objVendorCredit.NOTE_TO_PAYER, entityId, workFlowId, userId, approvalStatus, ByOverView, companyLogo);
            }

            if (oldVendorCreditId == 0)
            {
                var venderName = _repPayroll.GetVendorNameByVendorId(objVendorCredit.VENDOR_ID);
                var billName = string.Empty;
                if (venderName != null)
                {
                    billName = venderName + "_" + objVendorCredit.REFERENCE;
                }
                _srvWfDocumentService.ManageBillDocuments(entityId, workFlowId, addedBy, approvalStatus,
                genrateDocument, isCheckOriginalName, associtaedPages,
                 userId, userType, ref errorMessage, projectId, docId, ref originelDocId,
                ref newDocId, ref isClosePage, IsApproveMode, billName);
                if (!string.IsNullOrEmpty(errorMessage))

                {
                    return null;
                }
                _srvWfDocumentService.SaveDocumentEntity(workFlowId, entityId, documentId, newDocId, userId);
            }
            else
            {
                newDocId = docId;
                if (approvalStatus != (int)FlowStatus.Draft)
                {
                    _srvWfDocumentService.MoveDocumentInToBeReviewedFolder(userId, userType, ref errorMessage,
                        docId, originelDocId, projectId);
                }
            }
            SendPreEmailNotification(objVendorCreditModel.Approvers, userId, workFlowId, entityId, companyLogo, companyUrl);
            var obj = new object();
            obj = new
            {
                originelDocId = originelDocId,
                isClosePage = isClosePage,
                vendorCreditId = entityId,
                newDocId = newDocId
            };
            return obj;
        }

        private int SaveVendorCreditDetails(WF_VENDOR_CREDIT_DETAIL objVendorCredit)
        {
            int vendorCreditId = _repVendorCredit.SaveVendorCredit(objVendorCredit);
            return vendorCreditId;
        }

        public int UpdateVendorCredit(List<WF_VENDOR_CREDIT_DETAIL> objVendorCredit)
        {
            foreach (var item in objVendorCredit)
            {
                int vendorCreditId = _repVendorCredit.UpdateVendorCredit(item);
            }

            return 0;
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
    }
}
