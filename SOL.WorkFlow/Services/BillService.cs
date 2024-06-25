using SOL.Addressbook.Interfaces;
using SOL.Common.Models;
using SOL.Common.Business.Events.Workflow;
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
    public class BillService<T> : IBillService<T>, IEventSubscriber<DocumentUploadedModel>,
        IEventSubscriber<DocumentAssociatedPages>
    {
        #region Private Variables
        IApprovalService<T> _srvApproval = null;
        IChartsAccountService<T> _srvChartsAccount = null;
        IWorkflowDocumentService<T> _srvWfDocumentService = null;
        IWorkFlowRepository<T> _repWorkFlow;
        private IPmsService<T> _srvPms;
        IAddressbookService<int> _srvAddressbook = null;
        IPayrollRepository<T> _repPayroll;
        IWorkflowTimelineService<T> _srvTimeLine = null;

        string attachmentTempFolder = ConfigurationManager.AppSettings["TempDataFolder"].ToString();
        string attachmentFolder = ConfigurationManager.AppSettings["Documents"].ToString();
        string ecmVersionsFolder = ConfigurationManager.AppSettings["ECMVersionsFolder"].ToString();
        string openOfficePath = ConfigurationManager.AppSettings["PortableOpenOfficeExecutable"].ToString();
        string openOfficeRequestTimeout = ConfigurationManager.AppSettings["OpenOfficeRequestTimeOutSeconds"].ToString();
        string imageConversionMethod = ConfigurationManager.AppSettings["imageConversionMethod"].ToString();
        string isopenOffice = ConfigurationManager.AppSettings["UseOpenOffice"].ToString();
        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor.
        /// </summary>   
        public BillService(IApprovalService<T> srvApproval, IChartsAccountService<T> srvChartsAccount,
            IWorkflowDocumentService<T> srvWfDocumentService, IWorkFlowRepository<T> repWorkFlow, IPmsService<T> srvPms,
            IAddressbookService<int> srvAddressbook, IPayrollRepository<T> repPayroll, IWorkflowTimelineService<T> srvTimeLine)
        {

            this._srvChartsAccount = srvChartsAccount;
            this._srvWfDocumentService = srvWfDocumentService;
            this._repWorkFlow = repWorkFlow;
            this._srvPms = srvPms;
            this._srvAddressbook = srvAddressbook;
            this._srvApproval = srvApproval;
            this._repPayroll = repPayroll;
            this._srvTimeLine = srvTimeLine;
        }
        #endregion


        /// <summary>
        /// Saves the bill.
        /// </summary>
        /// <param name="objBillingViewModel">The object billing view model.</param>
        /// <param name="addedBy">The added by.</param>
        /// <returns></returns>
        public object SaveBill(BillingDetailsViewModel objBillingViewModel, string companyUrl,
            string companyLogo, string addedBy)
        {
            var approvalStatus = objBillingViewModel.BillingDetail.APPROVAL_STATUS;
            bool genrateDocument = true;
            bool isCheckOriginalName = false;
            var objBillingView = objBillingViewModel.BillingDetail;
            var oldBillId = objBillingView.BILL_ID;
            if (objBillingView.BILL_ID != default(int))
            {
                genrateDocument = false;
            }
            var associtaedPages = objBillingViewModel.AssocitaedPages;
            var userId = objBillingView.USER_ID;
            var userType = objBillingViewModel.UserType;
            var errorMessage = string.Empty;
            var clientId = objBillingView.CLIENT_ID;// objBillingView.VENDOR_ID;
            int projectId = _srvPms.GetProjectIdByClientId(clientId.Value);
            var docId = objBillingView.DOCUMENT_ID;
            var documentId = objBillingViewModel.documentId;
            var workFlowId = (int)CustomWorkflowTypes.BILLING_DETAILS; 
            var originelDocId = docId;
            var newDocId = 0;
            var isClosePage = true;
            var IsApproveMode = objBillingViewModel.IsApproveMode;
            int entityId = SaveBillDetails(objBillingView);
            byte ByOverView = 0;
            if (genrateDocument == true)
            {
                _srvTimeLine.SaveWorflowTimeline(entityId, workFlowId, approvalStatus, null, null,
                (int)WorkflowTimelineObject.VendorBill, (int)WorkflowTimelineEvents.Vendor_Bill_Created, string.Empty, userId, DateTime.UtcNow, null, null);
                if (approvalStatus != (int)FlowStatus.Draft)
                {
                    _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                    companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Create, false, null);
                }
            }
            else
            {
                _srvTimeLine.SaveWorflowTimeline(entityId, workFlowId, approvalStatus, null, null,
                (int)WorkflowTimelineObject.VendorBill, (int)WorkflowTimelineEvents.Vendor_Bill_Updated, string.Empty, userId, DateTime.UtcNow, null, null);
            }
            _srvChartsAccount.SaveBillBankAccount(objBillingViewModel.BillingAccounts, entityId, workFlowId, userId);
            if (approvalStatus != (int)FlowStatus.Draft && objBillingViewModel.Approvers != null)
            {
               var finalApprovalStatus = _srvApproval.SaveApprovers(objBillingViewModel.Approvers, objBillingView.NOTE_TO_PAYER, entityId, workFlowId, userId, approvalStatus, ByOverView, companyLogo);
                if (finalApprovalStatus == ApproverStatus.Approved)
                {
                    var bill = _repWorkFlow.GetBillByBillId(entityId);
                    bill.APPROVAL_STATUS = (byte)FlowStatus.Approved;
                    bill.USER_MODIFIED = userId;
                    bill.DATE_MODIFIED = DateTime.UtcNow;
                    _repWorkFlow.SaveBill(bill);
                }
            }
            else
            {
                newDocId = originelDocId;
            }
            if (associtaedPages != null)
            {
                var venderName = _repPayroll.GetVendorNameByVendorId(objBillingView.VENDOR_ID);
                var billName = string.Empty;
                if (venderName != null)
                {
                    billName = venderName + "_" + objBillingView.INVOICE_NUMBER;
                }
                _srvWfDocumentService.ManageBillDocuments(entityId, workFlowId, addedBy, approvalStatus, genrateDocument, isCheckOriginalName,
                associtaedPages, userId, userType, ref errorMessage, projectId, docId, ref originelDocId,
                ref newDocId, ref isClosePage, IsApproveMode, billName);
            }
            if (oldBillId == default(int))
            {
                _srvWfDocumentService.SaveDocumentEntity(workFlowId, entityId, documentId, newDocId, userId);
            }

            SendPreEmailNotification(objBillingViewModel.Approvers, userId, workFlowId, entityId, companyLogo, companyUrl);
            var obj = new object();
            obj = new
            {
                originelDocId = originelDocId,
                isClosePage = isClosePage,
                billId = entityId,
                newDocId = newDocId
            };
            return obj;
        }

        private int SaveBillDetails(WF_BILLING_DETAIL objBillingView)
        {
            var objBilling = new WF_BILLING_DETAIL();
            objBilling.BILL_ID = objBillingView.BILL_ID;
            objBilling.WORKFLOW_ID = objBillingView.WORKFLOW_ID;
            objBilling.CLIENT_ID = objBillingView.CLIENT_ID;
            objBilling.VENDOR_ID = objBillingView.VENDOR_ID;
            objBilling.DOCUMENT_ID = objBillingView.DOCUMENT_ID;
            objBilling.DOCUMENT_PAGE_NO = objBillingView.DOCUMENT_PAGE_NO;
            objBilling.INVOICE_NUMBER = objBillingView.INVOICE_NUMBER;
            objBilling.APPROVAL_STATUS = objBillingView.APPROVAL_STATUS;
            objBilling.PAYMENT_STATUS = objBillingView.PAYMENT_STATUS;
            objBilling.PAYMENT_TERM_ID = objBillingView.PAYMENT_TERM_ID;
            objBilling.INVOICE_DATE = objBillingView.INVOICE_DATE;
            objBilling.DUE_DATE = objBillingView.DUE_DATE;
            objBilling.AMOUNT = objBillingView.AMOUNT;
            objBilling.BALANCE = objBillingView.BALANCE;
            objBilling.BILL_DESCRIPTION = objBillingView.BILL_DESCRIPTION;
            objBilling.NOTE_TO_PAYER = objBillingView.NOTE_TO_PAYER;
            objBilling.IS_SPLIT_AMOUNT = objBillingView.IS_SPLIT_AMOUNT;
            objBilling.USER_ID = objBillingView.USER_ID;
            objBilling.DATE_OF_ENTRY = DateTime.UtcNow;
            objBilling.USER_MODIFIED = objBillingView.USER_MODIFIED;
            objBilling.DATE_MODIFIED = DateTime.UtcNow;
            objBilling.DELETED_FLAG = false;
            int billId = _repWorkFlow.SaveBill(objBilling);
            return billId;
        }

        public WF_BILLING_DETAIL GetBillDetailById(int billId)
        {
            return _repWorkFlow.GetBillByBillId(billId);
        }

        public int UpdateBillCredit(WF_BILLING_DETAIL objBillDetail)
        {
            return _repWorkFlow.UpdateBillCredit(objBillDetail);
        }
        
        /// <summary>
        /// Saves the add to existing page.
        /// </summary>
        /// <param name="entityId">The bill identifier.</param>
        /// <param name="selectedPages">The selected pages.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="addedBy">The added by.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public bool SaveAddToExistingPage(int entityId, int workFlowId, int[] selectedPages, int docId, int userId, UserType userType,
            string addedBy, ref string errorMessage)
        {
            return _srvWfDocumentService.SaveAddToExistingPage(entityId, workFlowId, selectedPages, docId, userId, userType,
           addedBy, ref errorMessage);
        }

        /// <summary>
        /// Approves the bill status.
        /// </summary>
        /// <param name="objBillingViewModel">The object billing view model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        public int ApproveBillStatus(UpdateStatusViewModel objBillingViewModel, int userId, UserType userType, string companyUrl,
            string companyLogo)
        {
            string errorMessage = string.Empty;
            var workFlowId = (int)CustomWorkflowTypes.BILLING_DETAILS;
            var billStatus = objBillingViewModel.Status;
            var entityId = objBillingViewModel.EntityId;
            _srvChartsAccount.SaveBillBankAccount(objBillingViewModel.BillingAccounts, entityId, 1, userId);
            _srvApproval.ManageApproversOnApprovals(objBillingViewModel.Approvers, objBillingViewModel.ApproverNote,null,
                billStatus, entityId, workFlowId, userId, companyLogo);
            var billDetail = _repWorkFlow.GetBillByBillId(entityId);
            var isAnyPendingAprrovers = _srvApproval.IsAnyPendingAprrover(entityId, workFlowId);
            var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, objBillingViewModel.WorkFlowId);
            if (billStatus == (int)FlowStatus.Denied)
            {
                billDetail.APPROVAL_STATUS = billStatus;
                _srvWfDocumentService.MoVeDocumentToDeniedFolder(userId, userType, ref errorMessage,
                       docId, docId, workFlowId);
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                   companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Denied, false, null);
            }
            else if (isAnyPendingAprrovers == false)
            {
                billDetail.APPROVAL_STATUS = billStatus;
                if (docId != 0)
                {
                    var vendorId = _repWorkFlow.GetVenderIdByBillId(entityId);
                    var vendorName = _repPayroll.GetVendorNameByVendorId(vendorId);
                    _srvWfDocumentService.MoVeDocumentToCompletedFolder(userId, userType, ref errorMessage,
                            docId, docId, vendorName, workFlowId);
                }
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
            billDetail.USER_MODIFIED = userId;
            billDetail.DATE_MODIFIED = DateTime.UtcNow;
            _repWorkFlow.UpdateApproverStatsu(billDetail);
            if (billStatus == (int)FlowStatus.Denied)
            {               
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                   companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Denied, false, null);
            }
            else if (isAnyPendingAprrovers == false)
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

        /// <summary>
        /// Gets the associated pages.
        /// </summary>
        /// <param name="entityId">The bill identifier.</param>
        /// <returns></returns>
        public List<int> GetAssociatedPages(int entityId, int workFlowId)
        {
            return _repWorkFlow.GetAssociatedPages(entityId, workFlowId);
        }

        /// <summary>
        /// Gets the associated pages by document identifier.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <returns></returns>
        public List<int> GetAssociatedPagesByDocId(int docId, int workFlowId)
        {
            return _repWorkFlow.GetAssociatedPagesByDocId(docId, workFlowId);
        }

        /// <summary>
        /// Gets the invoice number.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <param name="pageNo">The page no.</param>
        /// <returns></returns>
        public object GetInvoiceNumber(int docId, int pageNo, int workFlowId)
        {
            return _repWorkFlow.GetInvoiceNumber(docId, pageNo, workFlowId);
        }

        /// <summary>
        /// Gets the bill identifier.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <param name="pageNo">The page no.</param>
        /// <returns></returns>
        public int GetEntityId(int docId, int pageNo, int workFlowId)
        {
            return _repWorkFlow.GetEntityId(docId, pageNo, workFlowId);
        }

        /// <summary>
        /// Gets the new document identifier by bill identifier.
        /// </summary>
        /// <param name="billId">The bill identifier.</param>
        /// <returns></returns>
        public int GetNewDocIdByEntityId(int entityId, int workFlowId)
        {
            return _repWorkFlow.GetNewDocIdByEntityId(entityId, workFlowId);
        }

        /// <summary>
        /// Saves the tobe processed widget.
        /// </summary>
        /// <param name="triggerEventModel">The trigger event model.</param>
        public void SaveTobeProcessedWidget(DocumentUploadedModel triggerEventModel)
        {
            var folderId = triggerEventModel.FolderId;
            var docId = triggerEventModel.DocumentId;
            var userId = triggerEventModel.UserId;

            var objTobeProcesses = _repWorkFlow.IsExistDocumentInProcessedWidget(docId);
            if (objTobeProcesses == null)
            {
                objTobeProcesses = new WF_TOBEPROCESSED_WIDGET();
                objTobeProcesses.FOLDER_ID = folderId;
                objTobeProcesses.DOC_ID = docId;
                objTobeProcesses.FOLDER_NAME = triggerEventModel.FolderName;
                objTobeProcesses.SYSTEM_FILE_NAME = triggerEventModel.SystemFileName;
                objTobeProcesses.STATUS = 0;
                objTobeProcesses.WORKFLOW_ID = triggerEventModel.WorkFlowID;
                objTobeProcesses.USER_ID = triggerEventModel.UserId;
                objTobeProcesses.DATE_OF_ENTRY = DateTime.UtcNow;
                objTobeProcesses.USER_MODIFIED = triggerEventModel.UserId;
                objTobeProcesses.DATE_MODIFIED = DateTime.UtcNow;
                objTobeProcesses.DELETED_FLAG = false;
            }
            else
            {
                objTobeProcesses.FOLDER_ID = folderId;
                objTobeProcesses.DOC_ID = docId;
                objTobeProcesses.FOLDER_NAME = triggerEventModel.FolderName;
                objTobeProcesses.SYSTEM_FILE_NAME = triggerEventModel.SystemFileName;
            }
            _repWorkFlow.SaveTobeProcessedWidget(objTobeProcesses);
        }

        /// <summary>
        /// Deletes to be processed.
        /// </summary>
        /// <param name="toProcessedId">To processed identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public void DeleteToBeProcessed(int toProcessedId, int userId)
        {
            var toBeProcessed = _repWorkFlow.GetToBeProcessedById(toProcessedId);
            toBeProcessed.USER_MODIFIED = userId;
            toBeProcessed.DATE_MODIFIED = DateTime.UtcNow;
            toBeProcessed.DELETED_FLAG = true;
            _srvWfDocumentService.DeletedDocument(toBeProcessed.DOC_ID, userId);
            _repWorkFlow.DeleteToBeProcessed(toBeProcessed);
        }
        
        /// <summary>
        /// Checks the no of invoice for same vendor.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="VendorId">The vendor identifier.</param>
        /// <returns></returns>
        public int CheckNoOfInvoiceForSameVendor(string invoiceNumber, int VendorId)
        {
            return _repWorkFlow.CheckNoOfInvoiceForSameVendor(invoiceNumber, VendorId);
        }

        /// <summary>
        /// Uploads the document.
        /// </summary>
        /// <param name="uploadDocument">The upload document.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public int UploadDocument(UploadDocument uploadDocument, int clientId, ref string errorMessage)
        {
            int projectId = _srvPms.GetProjectIdByClientId(clientId);
            return _srvWfDocumentService.UploadDocument(uploadDocument, projectId, ref errorMessage);
        }

        /// <summary>
        /// Uploads the document by Folder.
        /// </summary>
        /// <param name="uploadDocument">The upload document.</param>
        /// <param name="folderId">The folderId identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public int UploadDocumentbyFolderId(UploadDocument uploadDocument, int folderId, ref string errorMessage)
        {
           
            return _srvWfDocumentService.UploadDocumentByFolder(uploadDocument, folderId, ref errorMessage);
        }

        /// <summary>
        /// Copies the document from ecm.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        public int CopyDocumentFromEcm(int docId, int clientId, int userId, UserType userType, ref string errorMessage)
        {            
            int projectId = _srvPms.GetProjectIdByClientId(clientId);
            return _srvWfDocumentService.CopyDocumentFromEcm(projectId, docId, userId, userType, ref errorMessage);
        }

        /// <summary>
        /// Copies the document from ecm.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        public int CopyDocumentToFolder(int docId, int destinationfolderId,int workflowId, int userId, UserType userType, ref string errorMessage)
        {
            //int projectId = _srvPms.GetProjectIdByClientId(clientId);
            return _srvWfDocumentService.CopyDocumentToFolder(docId, destinationfolderId, workflowId, userId, userType, ref errorMessage);
        }

        /// <summary>
        /// Gets the bill document detail.
        /// </summary>
        /// <param name="entityId">The bill identifier.</param>
        /// <returns></returns>
        public object GetWorkFlowDocumentDetail(int entityId, int workFlowId)
        {
            return _srvWfDocumentService.GetWorkFlowDocumentDetail(entityId, workFlowId);
        }

        /// <summary>
        /// Gets the document identifier by bill identifier.
        /// </summary>
        /// <param name="entityId">The bill identifier.</param>
        /// <returns></returns>
        public int GetDocIdByEntityId(int entityId, int workFlowId)
        {
            return _repWorkFlow.GetDocIdByEntityId(entityId, workFlowId);
        }

        /// <summary>
        /// Gets the pages count.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <returns></returns>
        public int? GetPagesCount(int docId)
        {
            return _srvWfDocumentService.GetPagesCount(docId);
        }

        public void Handle(DocumentUploadedModel triggerEventModel)
        {
            SaveTobeProcessedWidget(triggerEventModel);
        }

        public void Handle(DocumentAssociatedPages evt)
        {
            evt.ClientId = evt.ClientId;
        }
        public object UnAssociatePage(int newDocId, int originalDocId, int pageNo,
          int userId, UserType userType, string addedBy, int workFlowId)
        {
            return _srvWfDocumentService.UnAssociatePage(newDocId, originalDocId, pageNo,
           userId, userType, addedBy, workFlowId);
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


        public List<int> GetWorkflowUsers(int entityId, int workFlowId)
        {
            return _srvApproval.GetWorkflowUsers(entityId, workFlowId);
        }
    }
}
