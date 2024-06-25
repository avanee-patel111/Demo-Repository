using SOL.Addressbook.Interfaces;
using SOL.Common.Models;
using SOL.Common.Business.Interfaces;
using SOL.Common.Business.Models;
using SOL.ECM.Interfaces;
using SOL.ECM.Models;
using SOL.PMS.Interfaces;
using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mail;

namespace SOL.WorkFlow.Services
{
    public class WorkflowDocumentService<T> : IWorkflowDocumentService<T>
    {
        private INodeService<T> _srvNode;
        private IPmsService<T> _srvPms;
        private IAddressbookService<int> _srvAddressbook = null;
        private IWorkflowTimelineService<T> _srvTimeLine = null;
        private IBaseService _srvBase;
        private IWorkFlowRepository<T> _repWorkFlow;
        private IPayrollRepository<T> _repPayroll;
        private IWorkFlowPayrollRepository<T> _repPWfayroll = null;
        private IDailySalesReportRepository<int> _repDailySalesReport = null;
        private ICustomWorkflowRepository<int> _repCustomWorkflow = null;
        private IPayrollReportRepository<int> _repPayrollReport = null;

        string attachmentTempFolder = ConfigurationManager.AppSettings["TempDataFolder"].ToString();
        string attachmentFolder = ConfigurationManager.AppSettings["Documents"].ToString();
        string ecmVersionsFolder = ConfigurationManager.AppSettings["ECMVersionsFolder"].ToString();
        string openOfficePath = ConfigurationManager.AppSettings["PortableOpenOfficeExecutable"].ToString();
        string openOfficeRequestTimeout = ConfigurationManager.AppSettings["OpenOfficeRequestTimeOutSeconds"].ToString();
        string imageConversionMethod = ConfigurationManager.AppSettings["imageConversionMethod"].ToString();
        string isopenOffice = ConfigurationManager.AppSettings["UseOpenOffice"].ToString();
        public WorkflowDocumentService(INodeService<T> srvNode, IPmsService<T> srvPms, IWorkFlowRepository<T> repWorkFlow,
            IPayrollRepository<T> repPayroll, IAddressbookService<int> srvAddressbook, IWorkFlowPayrollRepository<T> repWfPayroll,
            IDailySalesReportRepository<int> repDailySalesReport, IWorkflowTimelineService<T> srvTimeLine,
            ICustomWorkflowRepository<int> repCustomWorkflow, IBaseService srvBase, IPayrollReportRepository<int> repPayrollReport)
        {
            this._srvBase = srvBase;
            this._srvNode = srvNode;
            this._srvPms = srvPms;
            this._repWorkFlow = repWorkFlow;
            this._repPayroll = repPayroll;
            this._srvAddressbook = srvAddressbook;
            this._repPWfayroll = repWfPayroll;
            this._repDailySalesReport = repDailySalesReport;
            this._srvTimeLine = srvTimeLine;
            this._repCustomWorkflow = repCustomWorkflow;
            this._repPayrollReport = repPayrollReport;
        }

        /// <summary>
        /// Moves the document in to be reviewed folder.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="originelDocId">The originel document identifier.</param>
        /// <param name="projectId">The project identifier.</param>
        public void MoveDocumentInToBeReviewedFolder(int userId, UserType userType,
      ref string errorMessage, int docId, int originelDocId, int projectId)
        {
            var destinationFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed,
                    projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
            CutDocument(userId, userType, ref errorMessage, docId, originelDocId, destinationFolderId);
        }
        private void CutDocument(int userId, UserType userType, ref string errorMessage, int docId,
            int originelDocId, int destinationFolderId)
        {
            var documentOperation = new DocumentOperation();
            var oldDocument = _srvNode.GetDocument(originelDocId);
            documentOperation.DestinationFolderId = destinationFolderId;
            documentOperation.SourceFolderId = oldDocument.FOLDER_ID;
            documentOperation.ORIGINEL_DOC_ID = originelDocId;
            List<int> docIds = new List<int>();
            docIds.Add(docId);
            documentOperation.DocIds = docIds.ToArray();
            documentOperation.isByPass = true;
            _srvNode.CutDocuments(documentOperation, userId, attachmentFolder, userType, true, ref errorMessage);

        }

        /// <summary>
        /// Cuts the docucmet.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="originelDocId">The originel document identifier.</param>
        /// <param name="destinationFolderId">The destination folder identifier.</param>
        public void MoVeDocumentToCompletedFolder(int userId, UserType userType, ref string errorMessage, int docId,
            int originelDocId, string vendorName, int workFlowId)
        {
            var document = _srvNode.GetDocument(docId);
            var entityId = document.ENTITY_ID;
            int projectId = 0;
            if (entityId.HasValue)
            {
                projectId = entityId.Value;
            }
            int parentId, destinationFolderId = default(int);
            switch ((CustomWorkflowTypes)workFlowId)
            {
                case CustomWorkflowTypes.BILLING_DETAILS:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.VendorInvoices, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    if (parentId == 0)
                    {
                        CreateMissingFolderStructureForBillingDetails(ECMGlobal.VendorInvoices, projectId,ECMGlobal.ECMLibraryId.ContentStructure);
                    }
                    destinationFolderId = _srvNode.SaveWorkFlowFolders(vendorName, parentId, userId);
                    break;
                case CustomWorkflowTypes.VENDOR_CREDIT:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.VendorInvoices, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowFolders(vendorName, parentId, userId);
                    break;
                case CustomWorkflowTypes.PAYROLL:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.PayrollReports, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowFolders(string.Empty, parentId, userId);
                    break;
                case CustomWorkflowTypes.DSReport:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.SalesInvoiceReports, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowFolders(string.Empty, parentId, userId);
                    break;
                case CustomWorkflowTypes.PayrollReport:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.PayrollReports, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowFolders(string.Empty, parentId, userId);
                    break;
                default:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.Other, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowFolders(string.Empty, parentId, userId);
                    break;
            }
            CutDocument(userId, userType, ref errorMessage, docId, originelDocId, destinationFolderId);
        }

        private void CreateMissingFolderStructureForBillingDetails(string vendorInvoices, int projectId, ECMGlobal.ECMLibraryId contentStructure)
        {
            var parentFolder = ECMGlobal.PurchaseAndPayables;

        }

        /// <summary>
        /// Moes the ve document to denied folder.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="originelDocId">The originel document identifier.</param>
        /// <param name="workFlowId">The work flow identifier.</param>
        public void MoVeDocumentToDeniedFolder(int userId, UserType userType, ref string errorMessage, int docId,
           int originelDocId, int workFlowId)
        {
            var document = _srvNode.GetDocument(docId);
            var entityId = document.ENTITY_ID;
            int projectId = 0;
            if (entityId.HasValue)
            {
                projectId = entityId.Value;
            }
            int parentId, destinationFolderId = default(int);
            switch ((CustomWorkflowTypes)workFlowId)
            {
                case CustomWorkflowTypes.BILLING_DETAILS:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowDeniedFolders(parentId, parentId, userId);
                    break;
                case CustomWorkflowTypes.VENDOR_CREDIT:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowDeniedFolders(parentId, parentId, userId);
                    break;
                case CustomWorkflowTypes.PAYROLL:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowDeniedFolders(parentId, parentId, userId);
                    break;
                case CustomWorkflowTypes.DSReport:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowDeniedFolders(parentId, parentId, userId);
                    break;
                case CustomWorkflowTypes.PayrollReport:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowDeniedFolders(parentId, parentId, userId);
                    break;
                default:
                    parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed, projectId,
                    (int)ECMGlobal.ECMLibraryId.ContentStructure);
                    destinationFolderId = _srvNode.SaveWorkFlowDeniedFolders(parentId, parentId, userId);
                    break;
            }
            CutDocument(userId, userType, ref errorMessage, docId, originelDocId, destinationFolderId);
        }

        /// <summary>
        /// Determines whether [is document exist in folder] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="originelDocId">The originel document identifier.</param>
        /// <param name="document">The document.</param>
        /// <param name="originalName">Name of the original.</param>
        /// <param name="toBeProcessedFolderId">To be processed folder identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is document exist in folder] [the specified user identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDocumentExistInFolder(int userId, UserType userType, ref string errorMessage,
            int docId, ref int originelDocId, ECM_DOCUMENTS document, string originalName, int toBeProcessedFolderId)
        {
            var isNewFile = false;
            if (toBeProcessedFolderId == 0)
            {               
                return isNewFile;
            }
            var documentOperation = new DocumentOperation();
            var isExistDocId = _srvNode.CheckDcoumentExistInFolder(docId, toBeProcessedFolderId, originalName);
            if (isExistDocId == 0)
            {
                var existingFolderId = document.FOLDER_ID;
                documentOperation.DestinationFolderId = toBeProcessedFolderId;
                documentOperation.SourceFolderId = existingFolderId;
                documentOperation.ORIGINEL_DOC_ID = docId;
                List<int> docIds = new List<int>();
                docIds.Add(docId);
                documentOperation.DocIds = docIds.ToArray();
                documentOperation.isByPass = true;
                var newDocuments = _srvNode.CopyDocuments(documentOperation, userId,
                    attachmentFolder, userType, ecmVersionsFolder, true, ref errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return false;
                }
                var copyDocumentResults = newDocuments.CopyDocumentsResults;
                var newDocument = copyDocumentResults.FirstOrDefault();
                originelDocId = newDocument.NewDocId;
                isNewFile = true;
                if (newDocument.NewDocId != 0)
                {
                    _srvNode.ProcessDocument(newDocument.NewDocId, (byte)1, userId, userType, attachmentFolder,
                      attachmentTempFolder, isopenOffice, openOfficePath, openOfficeRequestTimeout,
                      imageConversionMethod, false, ecmVersionsFolder, ref errorMessage);
                }
            }
            else
            {
                isNewFile = true;
                originelDocId = isExistDocId;
            }
            return isNewFile;
        }

        public void ManageBillDocuments(int entityId, int workFlowId, string addedBy, byte? approvalStatus,
            bool genrateDocument, bool isCheckOriginalName, int[] associtaedPages,
            int userId, UserType userType, ref string errorMessage, int projectId, int docId, ref int originelDocId,
            ref int newDocId, ref bool isClosePage, bool IsApproveMode, string newDocuementName)
        {
            if (genrateDocument)
            {
                if (!IsApproveMode)
                {
                    GetNewDocumentId(entityId, workFlowId, addedBy, isCheckOriginalName, (int)approvalStatus,
                       associtaedPages, userId, userType, (byte)WorkflowGloble.WorkFlowLevel.AllowPageSelection, ref errorMessage, projectId, docId,
                       ref originelDocId, ref newDocId, newDocuementName);

                    if (newDocId != 0)
                    {
                        _srvNode.ProcessDocument(newDocId, (byte)1, userId, userType, attachmentFolder,
                          attachmentTempFolder, isopenOffice, openOfficePath, openOfficeRequestTimeout,
                          imageConversionMethod, false, ecmVersionsFolder, ref errorMessage);
                    }
                    isClosePage = ManageOldDocument(userId, originelDocId, isClosePage);
                }
            }
            else
            {
                if (approvalStatus != (int)FlowStatus.Draft)
                {
                    MoveDocumentInToBeReviewedFolder(userId, userType, ref errorMessage,
                        docId, originelDocId, projectId);
                }
            }
        }

        private void GetNewDocumentId(int entityId, int workFlowId, string addedBy, bool isCheckOriginalName,
        int approvalStatus, int[] associtaedPages, int userId, UserType userType, byte workFlowLevel,
        ref string errorMessage, int projectId, int docId, ref int originelDocId,
        ref int newDocId, string newDocuementName)
        {
            var document = _srvNode.GetDocument(docId);
            var originalName = document.ORIGINAL_FILE_NAME;
            int toBeProcessedFolderId = 0;
            var isNewFile = false;
            if (approvalStatus == (int)FlowStatus.Draft && workFlowLevel == (byte)WorkflowGloble.WorkFlowLevel.AllowPageSelection)
            {
                var parentId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed, projectId,
                (int)ECMGlobal.ECMLibraryId.ContentStructure);

                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName("Draft",
                parentId, (int)ECMGlobal.ECMLibraryId.ContentStructure);

                isNewFile = IsDocumentExistInFolder(userId, userType, ref errorMessage, docId,
                ref originelDocId, document, originalName, parentId);

                if (toBeProcessedFolderId == default(int))
                {
                    var aclId = _srvNode.GetAclByFolderId(parentId);
                    var objECMFolder = new ECMFolder()
                    {
                        FOLDER_ID = 0,
                        PARENT_ID = parentId,
                        FOLDER_NAME = "Draft",
                        DESCRIPTION = null,
                        ACL_ID = aclId,
                        ENTITY_ID = parentId,
                        ENTITY_TYPE = (int)ECMGlobal.ECMLibraryId.ContentStructure,
                    };
                    toBeProcessedFolderId = _srvNode.SaveFolder(objECMFolder, userId);
                }
            }
            else
            {
                var folderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed,
                                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);

                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed,
                                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);

                isNewFile = IsDocumentExistInFolder(userId, userType, ref errorMessage, docId,
                            ref originelDocId, document, originalName, folderId);
            }

            var billName = newDocuementName + ".pdf";

            newDocId = _srvNode.GenerateDocument(originelDocId, billName, associtaedPages, toBeProcessedFolderId,
                userId, userType, addedBy, isNewFile, docId, isCheckOriginalName);

            var NewPage = 1;
            SaveWorkFlowDocuments(associtaedPages, userId, originelDocId, newDocId, entityId, workFlowId, NewPage, ref errorMessage);
        }

        public void SaveWorkFlowDocuments(int[] associtaedPages, int userId, int originelDocId, int newDocId,
            int entityId, int workFlowId, int NewPage, ref string errorMessage)
        {
            if (associtaedPages == null || associtaedPages.Length == 0)
            {
                errorMessage = "Save SuccessFully.";
                return;

            }
            foreach (var item in associtaedPages)
            {
                var objWorkFlowDocument = new WORKFLOW_DOCUMENT();
                objWorkFlowDocument.DOC_ID = originelDocId;
                objWorkFlowDocument.PAGE_NO = item;
                objWorkFlowDocument.WORKFLOW_ID = entityId;
                objWorkFlowDocument.WORKFLOW_DEFINITION_ID = workFlowId;
                objWorkFlowDocument.NEW_DOC_ID = newDocId;
                objWorkFlowDocument.NEW_PAGE_NO = NewPage;
                objWorkFlowDocument.USER_ID = userId;
                objWorkFlowDocument.USER_MODIFIED = userId;
                objWorkFlowDocument.DATE_OF_ENTRY = DateTime.UtcNow;
                objWorkFlowDocument.DATE_MODIFIED = DateTime.UtcNow;
                objWorkFlowDocument.DELETED_FLAG = false;
                _repWorkFlow.SaveWorkFlowDocument(objWorkFlowDocument);
                NewPage++;
            }
        }

        public void ManageCustomFlowDocuments(int entityId, int workFlowId, List<DocumentWorkFlow> docId, int userId, int processFolderId, int toBeReviewdFolderId,
            UserType userType, int approvalStatus, byte workFlowLevel, ref int newDocId, ref int originelDocId, ref bool isClosePage,
            string newDocuementName, int[] associtaedPages, string addedBy, ref string errorMessage)
        {

            foreach (var item in docId)
            {
                if (workFlowLevel != (byte)WorkflowGloble.WorkFlowLevel.AllowPageSelection)
                {
                    associtaedPages = SetAssociatedPages(item.DOC_ID);
                }

                if (processFolderId != 0)
                {
                    var isNewFile = false;
                    var document = _srvNode.GetDocument(item.DOC_ID);
                    var originalName = document.ORIGINAL_FILE_NAME;
                    if (processFolderId != 0 && workFlowLevel == (byte)WorkflowGloble.WorkFlowLevel.AllowPageSelection)
                    {
                        isNewFile = IsDocumentExistInFolder(userId, userType, ref errorMessage, item.DOC_ID, ref originelDocId, document,
                                        originalName, processFolderId);
                    }
                    int draftFolderId = 0;
                    if (workFlowLevel == (byte)WorkflowGloble.WorkFlowLevel.AllowPageSelection)
                    {
                        if (approvalStatus == (int)FlowStatus.Draft)
                        {
                            draftFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName("Draft",
                            processFolderId, (int)ECMGlobal.ECMLibraryId.ContentStructure);

                            if (draftFolderId == default(int))
                            {
                                var objECMFolder = new ECMFolder()
                                {
                                    FOLDER_ID = 0,
                                    PARENT_ID = processFolderId,
                                    FOLDER_NAME = "Draft",
                                    DESCRIPTION = null,
                                    ACL_ID = null,
                                    ENTITY_ID = processFolderId,
                                    ENTITY_TYPE = (int)ECMGlobal.ECMLibraryId.ContentStructure,
                                };
                                draftFolderId = _srvNode.SaveFolder(objECMFolder, userId);
                            }
                        }
                        newDocuementName = newDocuementName + ".pdf";
                        if (toBeReviewdFolderId != 0)
                        {
                            draftFolderId = toBeReviewdFolderId;
                        }
                        newDocId = _srvNode.GenerateDocument(originelDocId, newDocuementName, associtaedPages, draftFolderId,
                            userId, userType, addedBy, isNewFile, item.DOC_ID, false);
                        if (newDocId != 0)
                        {
                            _srvNode.ProcessDocument(newDocId, (byte)1, userId, userType, attachmentFolder,
                              attachmentTempFolder, isopenOffice, openOfficePath, openOfficeRequestTimeout,
                              imageConversionMethod, false, ecmVersionsFolder, ref errorMessage);
                        }
                        var NewPage = 1;
                        SaveWorkFlowDocuments(associtaedPages, userId, originelDocId, newDocId, entityId, workFlowId, NewPage, ref errorMessage);
                        isClosePage = ManageOldDocument(userId, originelDocId, isClosePage);
                    }
                    else
                    {
                        newDocId = item.DOC_ID;
                        isClosePage = true;
                        if (toBeReviewdFolderId != 0)
                        {
                            CutDocument(userId, userType, ref errorMessage, item.DOC_ID, item.DOC_ID, toBeReviewdFolderId);
                        }
                    }
                }
                else
                {
                    newDocId = item.DOC_ID;
                    isClosePage = true;
                    var noOfPages = 1;
                    var oldDocument = _srvNode.GetDocument(item.DOC_ID);
                    if (oldDocument != null)
                    {
                        var fileName = oldDocument.SYSTEM_FILE_NAME;
                        var extention = Path.GetExtension(fileName).ToLower();
                        var isImageType = IsImageFileType(extention);
                        if (isImageType != true)
                        {
                            var noOfDocumentPages = _srvNode.GetImagePageCount(item.DOC_ID);
                            if (noOfDocumentPages.HasValue)
                            {
                                noOfPages = noOfDocumentPages.Value;
                            }
                        }
                    }
                    var NewPage = 1;
                    for (int i = 0; i < noOfPages; i++)
                    {
                        var objWorkFlowDocument = new WORKFLOW_DOCUMENT();
                        objWorkFlowDocument.DOC_ID = item.DOC_ID;
                        objWorkFlowDocument.PAGE_NO = i;
                        objWorkFlowDocument.WORKFLOW_ID = entityId;
                        objWorkFlowDocument.WORKFLOW_DEFINITION_ID = workFlowId;
                        objWorkFlowDocument.NEW_DOC_ID = newDocId;
                        objWorkFlowDocument.NEW_PAGE_NO = NewPage;
                        objWorkFlowDocument.USER_ID = userId;
                        objWorkFlowDocument.USER_MODIFIED = userId;
                        objWorkFlowDocument.DATE_OF_ENTRY = DateTime.UtcNow;
                        objWorkFlowDocument.DATE_MODIFIED = DateTime.UtcNow;
                        objWorkFlowDocument.DELETED_FLAG = false;
                        _repWorkFlow.SaveWorkFlowDocument(objWorkFlowDocument);
                        NewPage++;
                    }
                    ManageToBeProcessedWidget(userId, item.DOC_ID);
                }
            }
        }

        public void ManageCustomWorkflowDocuments(int entityId, int workFlowId, string addedBy, byte? approvalStatus,
            bool genrateDocument, bool isCheckOriginalName, int[] associtaedPages, byte workFlowLevel, int userId,
            UserType userType, ref string errorMessage, int projectId, List<DocumentWorkFlow> docId, ref int originelDocId, ref int newDocId,
            ref bool isClosePage, bool IsApproveMode, string newDocuementName)
        {
            foreach (var item in docId)
            {
                if (genrateDocument)
                {
                    if (!IsApproveMode)
                    {
                        if (workFlowLevel != (byte)WorkflowGloble.WorkFlowLevel.AllowPageSelection)
                        {
                            associtaedPages = SetAssociatedPages(item.DOC_ID);
                        }
                        GetNewDocumentId(entityId, workFlowId, addedBy, isCheckOriginalName, (int)approvalStatus,
                           associtaedPages, userId, userType, workFlowLevel, ref errorMessage, projectId, item.DOC_ID,
                           ref originelDocId, ref newDocId, newDocuementName);

                        if (newDocId != 0)
                        {
                            _srvNode.ProcessDocument(newDocId, (byte)1, userId, userType, attachmentFolder,
                              attachmentTempFolder, isopenOffice, openOfficePath, openOfficeRequestTimeout,
                              imageConversionMethod, false, ecmVersionsFolder, ref errorMessage);
                        }
                        isClosePage = ManageOldDocument(userId, originelDocId, isClosePage);
                    }
                }
                else
                {
                    if (approvalStatus != (int)FlowStatus.Draft)
                    {
                        MoveDocumentInToBeReviewedFolder(userId, userType, ref errorMessage,
                            item.DOC_ID, originelDocId, projectId);
                    }
                }
            }
        }
        public bool ManageOldDocument(int userId, int originelDocId, bool isClosePage)
        {
            var oldDocument = _srvNode.GetDocument(originelDocId);
            if (oldDocument != null)
            {
                var fileName = oldDocument.SYSTEM_FILE_NAME;
                var extention = Path.GetExtension(fileName).ToLower();
                var isImageType = IsImageFileType(extention);
                if (isImageType == true)
                {
                    _srvNode.DeletedDocument(originelDocId, userId);
                    isClosePage = true;
                    ManageToBeProcessedWidget(userId, originelDocId);
                }
                else
                {
                    var associatedPagesCount = _repWorkFlow.GetAssociatedPagesCountByDocId(originelDocId);
                    var noOfPages = _srvNode.GetImagePageCount(originelDocId);
                    if (noOfPages.HasValue)
                    {
                        if (associatedPagesCount == noOfPages)
                        {
                            _srvNode.DeletedDocument(originelDocId, userId);
                            isClosePage = true;
                            ManageToBeProcessedWidget(userId, originelDocId);
                        }
                    }
                }
            }
            return isClosePage;
        }


        private void ManageToBeProcessedWidget(int userId, int originelDocId)
        {
            var toBeProcessedWidget = _repWorkFlow.GetToBeProcessedWidget(originelDocId);
            if (toBeProcessedWidget != null)
            {
                toBeProcessedWidget.USER_MODIFIED = userId;
                toBeProcessedWidget.DATE_MODIFIED = DateTime.UtcNow;
                toBeProcessedWidget.DELETED_FLAG = true;
                _repWorkFlow.SaveTobeProcessedWidget(toBeProcessedWidget);
            }
        }

        private void UpdateTobeProcessedWidgetWorkFlowId(int userId,int workflowId, int originelDocId)
        {
            var toBeProcessedWidget = _repWorkFlow.GetToBeProcessedWidget(originelDocId);
            if (toBeProcessedWidget != null)
            {
                toBeProcessedWidget.WORKFLOW_ID = workflowId;
                toBeProcessedWidget.USER_MODIFIED = userId;
                toBeProcessedWidget.DATE_MODIFIED = DateTime.UtcNow;              
                _repWorkFlow.SaveTobeProcessedWidget(toBeProcessedWidget);
            }
        }

        public object UnAssociatePage(int newDocId, int originalDocId, int pageNo,
          int userId, UserType userType, string addedBy, int workFlowId)
        {
            bool isCheckOriginalName = false;
            var newDocId1 = 0;
            var objDocument = _repWorkFlow.GetDocumentByDocIdAndPageNo(originalDocId, pageNo, workFlowId);
            newDocId = objDocument.NEW_DOC_ID;
            var newPageNumber = objDocument.NEW_PAGE_NO;
            var entityId = objDocument.WORKFLOW_ID;
            var folderId = _srvNode.getfolderIdByDocId(newDocId);
            var newPages = _repWorkFlow.GetAssociatedNewPages(entityId.Value, newPageNumber, workFlowId);
            var newDocument = _srvNode.GetDocument(newDocId);
            if (newDocument != null)
            {
                if (newPages.Count > 0)
                {
                    List<DocumentWorkFlow> documentId = new List<DocumentWorkFlow>();
                   // DocumentWorkFlow[] docuemntId = {} ;
                    newDocId1 = _srvNode.GenerateDocument(newDocId, newDocument.ORIGINAL_FILE_NAME, newPages.ToArray(),
                        folderId.Value, userId, userType, addedBy, true, newDocId, isCheckOriginalName);
                    _repWorkFlow.SaveDocumentPages(newDocId, newDocId1, userId);
                    SaveDocumentEntity(workFlowId, entityId.Value, documentId, newDocId1, userId);
                }
                newDocument.USER_MODIFIED = userId;
                newDocument.DATE_MODIFIED = DateTime.UtcNow;
              
                newDocument.DELETED_FLAG = true;
            }
            _srvNode.SaveChanges();
            var oldFileName = "";
            var newFileName = "";
            int activityId = (int)DocumentActivity.Delete;
            _srvNode.SaveDocumentLog(newDocId, activityId, oldFileName, newFileName, userId);
            _srvNode.DeletedDocument(newDocId, userId);
            objDocument.PAGE_NO = pageNo;
            objDocument.USER_MODIFIED = userId;
            objDocument.DATE_MODIFIED = DateTime.UtcNow;
            objDocument.DELETED_FLAG = true;
            _repWorkFlow.SaveWorkFlowDocument(objDocument);
            _srvTimeLine.SaveWorflowTimeline(entityId.Value, workFlowId, null, objDocument.NEW_PAGE_NO, null,
            (int)WorkflowTimelineObject.Document, (int)WorkflowTimelineEvents.Document_Unassociate, string.Empty, userId, DateTime.UtcNow, null, null);
            var obj = new object();
            obj = new
            {
                AssociatedPages = _repWorkFlow.GetAssociatedPagesByDocId(originalDocId, workFlowId),
                NewDocId = newDocId1
            };
            return obj;
        }

        public int CopyDocumentFromEcm(int projectId, int docId, int userId, UserType userType,
            ref string errorMessage)
        {
            var folderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed,
              projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
            var documentOperation = new DocumentOperation();
            var document = _srvNode.GetDocument(docId);
            if (document == null)
            {
                errorMessage = "Document processing folder(s) are missing. Please contact system administrator.";
                return 0;
            }
            var existingFolderId = document.FOLDER_ID;
            documentOperation.DestinationFolderId = folderId;
            documentOperation.SourceFolderId = existingFolderId;
            documentOperation.ORIGINEL_DOC_ID = docId;
            documentOperation.isViewGenerated = (int)ViewProcessStatus.ProcessByViewer;
            List<int> docIds = new List<int>();
            docIds.Add(docId);
            documentOperation.DocIds = docIds.ToArray();
            documentOperation.isByPass = true;
            var result = _srvNode.CopyDocuments(documentOperation, userId,
                attachmentFolder, userType, ecmVersionsFolder, true, ref errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return 0;
            }
            var copyDocumentResults = result.CopyDocumentsResults;
            var newDocument = copyDocumentResults.FirstOrDefault();
            return newDocument.NewDocId;
        }

        public int CopyDocumentToFolder( int docId,int destinationFolderId, int workflowId, int userId, UserType userType,
        ref string errorMessage)
        {
            //var folderId = destinationFolderId;
            //var documentOperation = new DocumentOperation();
            //var document = _srvNode.GetDocument(docId);
            //if (document == null)
            //{
            //    errorMessage = "Document processing folder(s) are missing. Please contact system administrator.";
            //    return 0;
            //}
            //var existingFolderId = document.FOLDER_ID;
            //documentOperation.DestinationFolderId = folderId;
            //documentOperation.SourceFolderId = existingFolderId;
            //documentOperation.ORIGINEL_DOC_ID = docId;
            //documentOperation.isViewGenerated = (int)ViewProcessStatus.ProcessByViewer;
            //List<int> docIds = new List<int>();
            //docIds.Add(docId);
            //documentOperation.DocIds = docIds.ToArray();
            //documentOperation.isByPass = true;
          //  CopyDocument(int folderId, int oldDocId, int userId, ref string errorMessage)
            var result = _srvNode.CopyDocument(destinationFolderId, docId, userId, ref errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return 0;
            }

            UpdateTobeProcessedWidgetWorkFlowId(userId, workflowId, result);


            return result;
        }

        public int UploadDocument(UploadDocument uploadDocument, int projectId, ref string errorMessage)
        {
            var folderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed,
             projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
            uploadDocument.FolderId = folderId;
            uploadDocument.isViewGenerated = (int)ViewProcessStatus.ProcessByViewer;
            _srvNode.UploadDocument(uploadDocument, ref errorMessage);
            return uploadDocument.DocId;
        }
        public int UploadDocumentByFolder(UploadDocument uploadDocument, int folderId, ref string errorMessage)
        {          
            uploadDocument.FolderId = folderId;
            uploadDocument.isViewGenerated = (int)ViewProcessStatus.ProcessByViewer;
            _srvNode.UploadDocument(uploadDocument, ref errorMessage);
            return uploadDocument.DocId;
        }
        public object GetWorkFlowDocumentDetail(int entityId, int workFlowId)
        {
            var obj = new object();
            int newDocId = _repWorkFlow.GetNewDocIdByEntityId(entityId, workFlowId);
            var document = _srvNode.GetDocument(newDocId);
            if (document != null)
            {
                obj = new
                {
                    originalFileName = document.ORIGINAL_FILE_NAME,
                    newDocId = newDocId
                };
                return obj;
            }
            return obj;
        }

        public int? GetPagesCount(int docId)
        {
            var noOfPages = _srvNode.GetImagePageCount(docId);
            return noOfPages;
        }

        public void DeletedDocument(int docId, int userId)
        {
            _srvNode.DeletedDocument(docId, userId);
        }

        public bool SaveAddToExistingPage(int entityId, int workFlowId, int[] selectedPages, int docId, int userId,
            UserType userType, string addedBy, ref string errorMessage)
        {
            var newDocId = 0;
            var isClosePage = false;
            var currentDocId = _repWorkFlow.GetNewDocIdByEntityId(entityId, workFlowId);
            var documentTitle = "";
            var toBeProcessedFolderId = 0;
            if (workFlowId == (int)WorkflowGloble.WorkFlowType.VenderBill)
            {
                var billDetail = _repWorkFlow.GetBillByBillId(entityId);
                var projectId = _srvPms.GetProjectIdByClientId(billDetail.CLIENT_ID.Value);
                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed,
                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                documentTitle = billDetail.INVOICE_NUMBER + ".pdf";
                var venderName = _srvAddressbook.GetCompanyNameByCompanyId(billDetail.VENDOR_ID);
                if (venderName != null)
                {
                    documentTitle = venderName + "_" + billDetail.INVOICE_NUMBER + ".pdf";
                }
            }
            else if (workFlowId == (int)WorkflowGloble.WorkFlowType.VenderCredit)
            {
                var venderCreditDetails = _repWorkFlow.GetVenderCreditByCreditId(entityId);
                var projectId = _srvPms.GetProjectIdByClientId(venderCreditDetails.CLIENT_ID.Value);
                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed,
                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                documentTitle = venderCreditDetails.REFERENCE + ".pdf";
                var venderName = _srvAddressbook.GetCompanyNameByCompanyId(venderCreditDetails.VENDOR_ID);
                if (venderName != null)
                {
                    documentTitle = venderName + "_" + venderCreditDetails.REFERENCE + ".pdf";
                }
            }
            else if (workFlowId == (int)WorkflowGloble.WorkFlowType.Payroll)
            {
                var payroll = _repPWfayroll.GetPayroll(entityId);
                var projectId = _srvPms.GetProjectIdByClientId(payroll.CLIENT_ID);
                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed,
                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                documentTitle = payroll.TITLE + ".pdf";
            }
            else if (workFlowId == (int)WorkflowGloble.WorkFlowType.PayrollReport)
            {
                var payrollReport = _repPayrollReport.GetPayrollReport(entityId);
                var projectId = _srvPms.GetProjectIdByClientId(payrollReport.CLIENT_ID);
                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed,
                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                documentTitle = payrollReport.TITLE + ".pdf";
            }
            else if (workFlowId == (int)WorkflowGloble.WorkFlowType.DSReport)
            {
                var dsrReport = _repDailySalesReport.GetDailySalesReport(entityId);
                var projectId = _srvPms.GetProjectIdByClientId(dsrReport.CLIENT_ID);
                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed,
                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                documentTitle = dsrReport.TITLE + ".pdf";
            }
            else
            {
                var customWorkFlow = _repCustomWorkflow.GetCustomWorkFlow(entityId);
                var clientId = customWorkFlow.CLIENT_ID;
                var projectId = 0;
                if (clientId.HasValue)
                {
                    projectId = _srvPms.GetProjectIdByClientId(clientId.Value);
                    toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed,
                    projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                }
                else
                {
                    var documentProcessingWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
                    var isBookkeepingClientSpecific = true;
                    var workFlowLevel = (byte)WorkflowGloble.WorkFlowLevel.AllowPageSelection;
                    if (documentProcessingWorkflow != null)
                    {
                        if (documentProcessingWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC.HasValue)
                        {
                            isBookkeepingClientSpecific = documentProcessingWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC.Value;
                        }
                        workFlowLevel = documentProcessingWorkflow.WORK_FLOW_LEVEL.Value;
                    }
                    if (isBookkeepingClientSpecific)
                    {
                        if (clientId.HasValue)
                        {
                            projectId = _srvPms.GetProjectIdByClientId(clientId.Value);
                            toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeReviewed,
                            projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                        }
                    }
                    else
                    {
                        var steps = documentProcessingWorkflow.WORKFLOW_DEFINITION_STEPS.Where(x => x.DELETED_FLAG == false);
                        if (steps != null)
                        {
                            var step = steps.Where(x => x.STEP_ID == (int)WorkflowGloble.Step.ToBeReviewed).FirstOrDefault();
                            if (step != null)
                            {
                                if (step.FOLDER_ID.HasValue)
                                {
                                    toBeProcessedFolderId = step.FOLDER_ID.Value;
                                }
                            }
                        }
                    }
                }
                documentTitle = customWorkFlow.TITLE + ".pdf";
            }
            for (int i = 0; i < selectedPages.Length; i++)
            {
                bool isExist = _repWorkFlow.CheckPageIsExist(entityId, docId, selectedPages[i], currentDocId, workFlowId);
                if (isExist)
                {
                    selectedPages = selectedPages.Where(val => val != selectedPages[i]).ToArray();
                }
            }
            var newDocument = _srvNode.GenerateDocument(toBeProcessedFolderId, documentTitle, currentDocId, docId,
                selectedPages, userId, addedBy, userType);
            System.Reflection.PropertyInfo pi = newDocument.GetType().GetProperty("NewDocId");
            newDocId = (Int32)(pi.GetValue(newDocument, null));
            pi = newDocument.GetType().GetProperty("TotalCount");
            var newPage = (Int32)(pi.GetValue(newDocument, null));

            int newPageNumber = _repWorkFlow.GetLastPageNumber(entityId, workFlowId);
            newPageNumber++;
            SaveWorkFlowDocuments(selectedPages, userId, docId, newDocId, entityId, workFlowId, newPageNumber, ref errorMessage);
            var oldDocument = _repWorkFlow.GetWorkFlowDocument(entityId, workFlowId);
            _srvNode.DeletedDocument(currentDocId, userId);
            if (newDocId != 0)
            {
                _srvNode.ProcessDocument(newDocId, (byte)1, userId, userType, attachmentFolder,
                  attachmentTempFolder, isopenOffice, openOfficePath, openOfficeRequestTimeout,
                  imageConversionMethod, false, ecmVersionsFolder, ref errorMessage);
                if (oldDocument != null)
                {
                    foreach (var doc in oldDocument)
                    {
                        doc.NEW_DOC_ID = newDocId;
                    }
                    _repWorkFlow.SaveChanges();
                }
                List<DocumentWorkFlow> documentId = new List<DocumentWorkFlow>();
                //DocumentWorkFlow[] docuemntId = { };
                SaveDocumentEntity(workFlowId, entityId, documentId, newDocId, userId);
            }
            _srvTimeLine.SaveWorflowTimeline(entityId, workFlowId, null, selectedPages.Length, null,
       (int)WorkflowTimelineObject.Document, (int)WorkflowTimelineEvents.Document_Associate, string.Empty, userId, DateTime.UtcNow, null, null);
            isClosePage = ManageOldDocument(userId, docId, isClosePage);
            return isClosePage;
        }

        public bool IsImageFileType(string sFileExtension)
        {
            bool result = false;
            try
            {
                switch (sFileExtension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                    case ".bmp":
                    case ".tiff":
                    case ".tif":
                        result = true;
                        break;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public WORKFLOW_DOCUMENT_ENTITIES GetDocumentEntity(int workFlowId, int entityId)
        {
            return _repWorkFlow.GetDocumentEntity(workFlowId, entityId);

        }
        public void SaveDocumentEntity(int workFlowId, int entityId, List<DocumentWorkFlow> docId, int newDocId, int userId)
        {
            if (docId == null)
            {
                var docnullentity = new WORKFLOW_DOCUMENT_ENTITIES();
                docnullentity.WORKFLOW_DEFINITION_ID = workFlowId;
                docnullentity.DOC_ID = 0;
                docnullentity.NEW_DOC_ID = 0;
                docnullentity.WORKFLOW_ID = entityId;
                docnullentity.USER_ID = userId;
                docnullentity.USER_MODIFIED = userId;
                docnullentity.DATE_OF_ENTRY = DateTime.UtcNow;
                docnullentity.DATE_MODIFIED = DateTime.UtcNow;
                docnullentity.DELETED_FLAG = false;
                docnullentity.FOLDER_ID = newDocId;
                _repWorkFlow.SaveDocumentEntity(docnullentity);
            }
            else
            {
                if(docId.Count == 0)
                {
                    var documentEntitys = new WORKFLOW_DOCUMENT_ENTITIES();
                    documentEntitys.WORKFLOW_DEFINITION_ID = workFlowId;
                    documentEntitys.DOC_ID = 0;
                    documentEntitys.NEW_DOC_ID = 0;
                    documentEntitys.WORKFLOW_ID = entityId;
                    documentEntitys.USER_ID = userId;
                    documentEntitys.USER_MODIFIED = userId;
                    documentEntitys.DATE_OF_ENTRY = DateTime.UtcNow;
                    documentEntitys.DATE_MODIFIED = DateTime.UtcNow;
                    documentEntitys.DELETED_FLAG = false;
                    documentEntitys.FOLDER_ID = newDocId;
                    _repWorkFlow.SaveDocumentEntity(documentEntitys);
                }
                foreach (var item in docId)
                {

                    var documentEntity = GetDocumentEntity(workFlowId, entityId);
                    if (documentEntity == null)
                    {
                        documentEntity = new WORKFLOW_DOCUMENT_ENTITIES();
                        documentEntity.WORKFLOW_DEFINITION_ID = workFlowId;
                        documentEntity.DOC_ID = item.DOC_ID;
                        documentEntity.NEW_DOC_ID = item.DOC_ID;
                        documentEntity.WORKFLOW_ID = entityId;
                        documentEntity.USER_ID = userId;
                        documentEntity.USER_MODIFIED = userId;
                        documentEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        documentEntity.DATE_MODIFIED = DateTime.UtcNow;
                        documentEntity.DELETED_FLAG = false;
                        documentEntity.FOLDER_ID = newDocId;
                    }
                    else
                    {
                        documentEntity.NEW_DOC_ID = item.DOC_ID;
                        documentEntity.USER_MODIFIED = userId;
                        documentEntity.DATE_MODIFIED = DateTime.UtcNow;
                    }
                    _repWorkFlow.SaveDocumentEntity(documentEntity);
                }
            }
        }

        public void MoveDocumentInToBeReviewedFolderByStep(int userId, UserType userType, ref string errorMessage,
            int docId, int originelDocId, int folderId)
        {
            CutDocument(userId, userType, ref errorMessage, docId, originelDocId, folderId);
        }

        public void MoVeDocumentToCompletedFolderByStep(int userId, UserType userType, int docId, int folderId,
            ref string errorMessage)
        {
            var destinationFolderId = _srvNode.SaveWorkFlowFolders(string.Empty, folderId, userId);
            CutDocument(userId, userType, ref errorMessage, docId, docId, destinationFolderId);
        }

        public void MoVeDocumentToDeniedFolderByStep(int userId, UserType userType, int docId, int folderId,
            ref string errorMessage)
        {
            var destinationFolderId = _srvNode.SaveWorkFlowDeniedFolders(folderId, folderId, userId);
            CutDocument(userId, userType, ref errorMessage, docId, docId, destinationFolderId);
        }

        private int[] SetAssociatedPages(int docId)
        {
            List<int> associatedPagesList = new List<int>();
            var oldDocument = _srvNode.GetDocument(docId);
            if (oldDocument != null)
            {
                var fileName = oldDocument.SYSTEM_FILE_NAME;
                var extention = Path.GetExtension(fileName).ToLower();
                var isImageType = IsImageFileType(extention);
                if (isImageType == true)
                {
                    associatedPagesList.Add(1);
                }
                else
                {
                    var noOfDocumentPages = _srvNode.GetImagePageCount(docId);
                    if (noOfDocumentPages.HasValue)
                    {
                        for (int i = 1; i <= noOfDocumentPages; i++)
                        {
                            associatedPagesList.Add(i);
                        }
                    }
                }
            }
            int[] associatedPages = associatedPagesList.ToArray();
            return associatedPages;
        }

        public void SaveWorkFlowEmailEventEntryInOutBound(int userId, int workflowId, int entityId, string companyUrl,
            string companyLogo, int eventType, bool isPreUser, int? preUserId)
        {
            var workflowTitle = _repWorkFlow.GetWorkflowTitleById(workflowId);
            var toEmailId = _srvAddressbook.GetContactEmail(userId);
            if (toEmailId == null || toEmailId == "")
            {
                return;
            }
            var formId = _srvBase.GetCompanySetup<string>("DFLT_SENDER_EMAIL");
            var companyName = _srvBase.GetCompanySetup<string>("COMPANY_NAME");
            var copyRight = companyName + " 2000 - " + DateTime.Today.Year.ToString();
            var formatedSubject = "";
            var compUrl = " <a href='" + companyUrl + "'>WIP</a>";
            if (formId != null)
            {
                StringBuilder bodyMessage = null;
                switch ((CustomWorkflowTypes)workflowId)
                {
                    case CustomWorkflowTypes.BILLING_DETAILS:
                        SetBillNotification(userId, workflowId, entityId, companyUrl, compUrl, companyLogo,
                            eventType, isPreUser, preUserId, copyRight, workflowTitle, ref bodyMessage,
                            ref formatedSubject, ref toEmailId);
                        break;
                    case CustomWorkflowTypes.VENDOR_CREDIT:
                        SetVendorNotification(userId, workflowId, entityId, companyUrl, compUrl, companyLogo,
                            eventType, isPreUser, preUserId, copyRight, workflowTitle, ref bodyMessage,
                            ref formatedSubject, ref toEmailId);
                        break;
                    case CustomWorkflowTypes.DSReport:
                        SetDSReportNotification(userId, workflowId, entityId, companyUrl, compUrl, companyLogo,
                            eventType, isPreUser, preUserId, copyRight, workflowTitle, ref bodyMessage,
                            ref formatedSubject, ref toEmailId);
                        break;
                    case CustomWorkflowTypes.PAYROLL:
                        SetPayrollNotification(userId, workflowId, entityId, companyUrl, compUrl, companyLogo,
                            eventType, isPreUser, preUserId, copyRight, workflowTitle, ref bodyMessage,
                            ref formatedSubject, ref toEmailId);
                        break;
                    case CustomWorkflowTypes.PayrollReport:
                        SetPayrollReportNotification(userId, workflowId, entityId, companyUrl, compUrl, companyLogo,
                            eventType, isPreUser, preUserId, copyRight, workflowTitle, ref bodyMessage,
                            ref formatedSubject, ref toEmailId);
                        break;
                    default:
                        SetCustomWorkFlowNotification(userId, workflowId, entityId, companyUrl, compUrl, companyLogo,
                            eventType, isPreUser, preUserId, copyRight, workflowTitle, ref bodyMessage,
                            ref formatedSubject, ref toEmailId);
                        break;
                }
                if (toEmailId == null || toEmailId == "" || formId == null || formId == "" || bodyMessage == null)
                {
                    return;
                }
                var outboundCommunication = new OUTBOUND_COMMUNICATION()
                {
              
                    BODY_MESSAGE = bodyMessage.ToString(),
                    DATE_MODIFIED = DateTime.UtcNow,
                    DATE_OF_ENTRY = DateTime.UtcNow,
                    DELETED_FLAG = false,
                    SENT_FLAG = 0,
                    FROM_ID = formId,
                    SUBJECT_MESSAGE = formatedSubject,
                    TO_ID = toEmailId,
                    USER_ID = userId,
                    USER_MODIFIED = userId,
                    M_FORMAT = (byte)MailFormat.Html
                };

                _srvBase.SendEmail(outboundCommunication);
            }
        }

        private string GetEmailTemplatePath(string bodyFileName)
        {
            var appDomain = System.AppDomain.CurrentDomain;
            var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
            var strEmailTemplatePath = Path.Combine(basePath, "Resources", bodyFileName);
            return strEmailTemplatePath;
        }

        private string SetApprovalStatus(int approvalStatus)
        {
            string status = "Assigned";
            switch ((ApproverStatus)approvalStatus)
            {
                case ApproverStatus.Approved:
                    status = "Approved";
                    break;
                case ApproverStatus.Assigned:
                    status = "Assigned";
                    break;
                case ApproverStatus.Denied:
                    status = "Denied";
                    break;
                case ApproverStatus.Upcoming:
                    status = "Upcoming";
                    break;
                case ApproverStatus.Waiting:
                    status = "Waiting";
                    break;
                default:
                    status = "Assigned";
                    break;
            }
            return status;
        }

        private string GetWorkflowNote(int workFlowId, int entityId, int approvalStatus)
        {
            var note = "";
            note = _repWorkFlow.GetLastWorkFlowNote(workFlowId, entityId, approvalStatus);
            return note;
        }

        private void SetBillNotification(int userId, int workflowId, int entityId, string companyUrl, string compUrl,
            string WIPLogoEmailTemplate, int eventType, bool isPreUser, int? preUserId, string copyRight, string workflowTitle,
            ref StringBuilder bodyMessage, ref string formatedSubject, ref string toEmailId)
        {
            var userName = "";
            var clientName = "";
            var vendorName = "";
            var initiatorName = "";
            var preApprovedName = "";
            var strEmailTemplatePath = "";
            var note = "";
            var billingDetail = _repWorkFlow.GetBillByBillId(entityId);
            userName = _srvAddressbook.GetContactNameByContactId(userId);
            clientName = _srvAddressbook.GetCompanyNameByCompanyId(billingDetail.CLIENT_ID.Value);
            vendorName = _repPayroll.GetVendorNameByVendorId(billingDetail.VENDOR_ID);
            initiatorName = _srvAddressbook.GetContactNameByContactId(billingDetail.USER_ID);
            preApprovedName = userName;
            if (isPreUser)
            {
                preApprovedName = _srvAddressbook.GetContactNameByContactId(preUserId.Value);
            }

            StringBuilder content = null;
            var strEmailTemplate = _srvBase.GetEmailTemplatePath(WIPLogoEmailTemplate, EmailTemplateType.WithRequiredUserName);
            bodyMessage = new StringBuilder(strEmailTemplate);

            switch ((WorkflowGloble.WorkFlowEmailEventType)eventType)
            {
                case WorkflowGloble.WorkFlowEmailEventType.Create:
                    formatedSubject = "Bill of " + vendorName + " is ready to be paid";
                    strEmailTemplatePath = GetEmailTemplatePath("WorkFlowBillCreateEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }

                    bodyMessage.Replace("##UserName##", userName);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Client##", clientName);
                    content.Replace("##Vendor##", vendorName);
                    content.Replace("##Invoice##", billingDetail.INVOICE_NUMBER);
                    content.Replace("##Amount##", billingDetail.AMOUNT.ToString("C"));
                    content.Replace("##DueDate##", billingDetail.INVOICE_DATE.ToString("MM/dd/yyyy"));
                    content.Replace("##ApprovalStatus##", SetApprovalStatus(billingDetail.APPROVAL_STATUS.Value));
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting:
                    note = GetWorkflowNote(workflowId, entityId, billingDetail.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = "Bill or Vendor credit of " + vendorName + " needs your approval";
                    strEmailTemplatePath = GetEmailTemplatePath("WorkFlowBillApprovalEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }

                    bodyMessage.Replace("##UserName##", userName);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##Client##", clientName);
                    content.Replace("##Vendor##", vendorName);
                    content.Replace("##InvoiceTitle##", "Invoice#");
                    content.Replace("##Invoice##", billingDetail.INVOICE_NUMBER);
                    content.Replace("##Amount##", billingDetail.AMOUNT.ToString("C"));
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Approved:
                    note = GetWorkflowNote(workflowId, entityId, billingDetail.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = "Bill or Vendor credit of " + vendorName + " has been approved";
                    strEmailTemplatePath = GetEmailTemplatePath("WorkFlowBillApprovedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(billingDetail.USER_ID);

                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##ApprovedName##", preApprovedName);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Client##", clientName);
                    content.Replace("##Vendor##", vendorName);
                    content.Replace("##InvoiceTitle##", "Invoice#");
                    content.Replace("##Invoice##", billingDetail.INVOICE_NUMBER);
                    content.Replace("##Amount##", billingDetail.AMOUNT.ToString("C"));
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Denied:
                    note = GetWorkflowNote(workflowId, entityId, billingDetail.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(billingDetail.USER_ID);
                    formatedSubject = "Bill or Vendor credit of " + vendorName + " has been Denied";
                    strEmailTemplatePath = GetEmailTemplatePath("WorkFlowBillDeniedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, workflowId);
                    var url = "/Workflow/Bill?billId=" + entityId + "&docId=" + docId + "&documentProcessStatus=true";
                    var encodedUrl = HttpUtility.UrlEncode(url);
                    var deniedBillUrl = Path.Combine(companyUrl, "Home/Redirect/?url=" + encodedUrl);

                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##DeniedName##", userName);
                    content.Replace("##DeniedBillUrl##", deniedBillUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Client##", clientName);
                    content.Replace("##Vendor##", vendorName);
                    content.Replace("##InvoiceTitle##", "Invoice#");
                    content.Replace("##Invoice##", billingDetail.INVOICE_NUMBER);
                    content.Replace("##Amount##", billingDetail.AMOUNT.ToString("C"));
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.completed:
                    formatedSubject = "Bill or Vendor credit of " + vendorName + " has been processed sucessfully";
                    strEmailTemplatePath = GetEmailTemplatePath("WorkFlowBillCompletedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(billingDetail.USER_ID);
                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##WorkFlowName##", workflowTitle);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Client##", clientName);
                    content.Replace("##Vendor##", vendorName);
                    content.Replace("##InvoiceTitle##", "Invoice#");
                    content.Replace("##Invoice##", billingDetail.INVOICE_NUMBER);
                    content.Replace("##Amount##", billingDetail.AMOUNT.ToString("C"));
                    content.Replace("##CopyRight##", copyRight);
                    break;
                default:
                    break;
            }

            bodyMessage.Replace("##EmailContent##", content.ToString());
        }

        private void SetVendorNotification(int userId, int workflowId, int entityId, string companyUrl, string compUrl,
         string WIPLogoEmailTemplate, int eventType, bool isPreUser, int? preUserId, string copyRight, string workflowTitle,
         ref StringBuilder bodyMessage, ref string formatedSubject, ref string toEmailId)
        {
            var userName = "";
            var clientName = "";
            var vendorName = "";
            var initiatorName = "";
            var preApprovedName = "";
            var strEmailTemplatePath = "";
            var note = "";
            var creditDetail = _repWorkFlow.GetVenderCreditByCreditId(entityId);
            userName = _srvAddressbook.GetContactNameByContactId(userId);
            clientName = _srvAddressbook.GetCompanyNameByCompanyId(creditDetail.CLIENT_ID.Value);
            vendorName = _repPayroll.GetVendorNameByVendorId(creditDetail.VENDOR_ID);
            initiatorName = _srvAddressbook.GetContactNameByContactId(creditDetail.USER_ID);
            preApprovedName = userName;
            if (isPreUser)
            {
                preApprovedName = _srvAddressbook.GetContactNameByContactId(preUserId.Value);
            }


            StringBuilder content = null;
            var strEmailTemplate = _srvBase.GetEmailTemplatePath(WIPLogoEmailTemplate, EmailTemplateType.WithRequiredUserName);
            bodyMessage = new StringBuilder(strEmailTemplate);

            switch ((WorkflowGloble.WorkFlowEmailEventType)eventType)
            {
                case WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting:
                    note = GetWorkflowNote(workflowId, entityId, creditDetail.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = "Bill or Vendor credit of " + vendorName + " needs your approval";
                    strEmailTemplatePath = GetEmailTemplatePath("WorkFlowBillApprovalEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }

                    bodyMessage.Replace("##UserName##", userName);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##Client##", clientName);
                    content.Replace("##Vendor##", vendorName);
                    content.Replace("##InvoiceTitle##", "Ref#");
                    content.Replace("##Invoice##", creditDetail.REFERENCE);
                    content.Replace("##Amount##", creditDetail.AMOUNT.ToString("C"));
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Approved:
                    note = GetWorkflowNote(workflowId, entityId, creditDetail.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = "Bill or Vendor credit of " + vendorName + " has been approved";
                    strEmailTemplatePath = GetEmailTemplatePath("WorkFlowBillApprovedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(creditDetail.USER_ID);

                    bodyMessage.Replace("##UserName##", userName);
                    content.Replace("##InvoiceTitle##", "Ref#");
                    content.Replace("##ApprovedName##", preApprovedName);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Client##", clientName);
                    content.Replace("##Vendor##", vendorName);
                    content.Replace("##InvoiceTitle##", "Ref#");
                    content.Replace("##Invoice##", creditDetail.REFERENCE);
                    content.Replace("##Amount##", creditDetail.AMOUNT.ToString("C"));
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Denied:
                    note = GetWorkflowNote(workflowId, entityId, creditDetail.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(creditDetail.USER_ID);
                    formatedSubject = "Bill or Vendor credit of " + vendorName + " has been Denied";
                    strEmailTemplatePath = GetEmailTemplatePath("WorkFlowBillDeniedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, workflowId);
                    var url = "/VendorCredit/VendorCredit?vendorCreditId=" + entityId + "&docId=" + docId + "&documentProcessStatus=true";
                    var encodedUrl = HttpUtility.UrlEncode(url);
                    var deniedBillUrl = Path.Combine(companyUrl, "Home/Redirect/?url=" + encodedUrl);

                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##DeniedName##", userName);
                    content.Replace("##DeniedBillUrl##", deniedBillUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Client##", clientName);
                    content.Replace("##Vendor##", vendorName);
                    content.Replace("##InvoiceTitle##", "Ref#");
                    content.Replace("##Invoice##", creditDetail.REFERENCE);
                    content.Replace("##Amount##", creditDetail.AMOUNT.ToString("C"));
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.completed:
                    formatedSubject = "Bill or Vendor credit of " + vendorName + " has been processed sucessfully";
                    strEmailTemplatePath = GetEmailTemplatePath("WorkFlowBillCompletedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(creditDetail.USER_ID);

                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##WorkFlowName##", workflowTitle);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Client##", clientName);
                    content.Replace("##Vendor##", vendorName);
                    content.Replace("##InvoiceTitle##", "Ref#");
                    content.Replace("##Invoice##", creditDetail.REFERENCE);
                    content.Replace("##Amount##", creditDetail.AMOUNT.ToString("C"));
                    content.Replace("##CopyRight##", copyRight);
                    break;
                default:
                    break;
            }

            bodyMessage.Replace("##EmailContent##", content.ToString());
        }

        private void SetDSReportNotification(int userId, int workflowId, int entityId, string companyUrl, string compUrl,
            string WIPLogoEmailTemplate, int eventType, bool isPreUser, int? preUserId, string copyRight,
            string workflowTitle, ref StringBuilder bodyMessage, ref string formatedSubject, ref string toEmailId)
        {
            bool isPayroll = false;
            var strEmailTemplatePath = "";
            var note = "";
            var dailySalesReport = _repDailySalesReport.GetDailySalesReport(entityId);
            var userName = _srvAddressbook.GetContactNameByContactId(userId);
            var clientName = _srvAddressbook.GetCompanyNameByCompanyId(dailySalesReport.CLIENT_ID);
            var initiatorName = _srvAddressbook.GetContactNameByContactId(dailySalesReport.USER_ID);
            var customTable = SetStaticWorkFlowInvoiceReportTable(clientName, dailySalesReport.TITLE, dailySalesReport.TOTAL_SALES.ToString("C"), isPayroll);
            var preApprovedName = userName;
            if (isPreUser)
            {
                preApprovedName = _srvAddressbook.GetContactNameByContactId(preUserId.Value);
            }

            StringBuilder content = null;
            var strEmailTemplate = _srvBase.GetEmailTemplatePath(WIPLogoEmailTemplate, EmailTemplateType.WithRequiredUserName);
            bodyMessage = new StringBuilder(strEmailTemplate);

            switch ((WorkflowGloble.WorkFlowEmailEventType)eventType)
            {
                case WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting:
                    note = GetWorkflowNote(workflowId, entityId, dailySalesReport.REVIEWER_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = workflowTitle + " needs your approval";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowApprovalEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }

                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", userName);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Approved:
                    note = GetWorkflowNote(workflowId, entityId, dailySalesReport.REVIEWER_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = workflowTitle + " has been approved";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowApprovedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(dailySalesReport.USER_ID);

                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##ApprovedName##", preApprovedName);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Denied:
                    note = GetWorkflowNote(workflowId, entityId, dailySalesReport.REVIEWER_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(dailySalesReport.USER_ID);
                    formatedSubject = workflowTitle + " has been Denied";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowDeniedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, workflowId);
                    var url = "/DailySalesReport/DailySalesReport?workFlowId=" + workflowId + "&docId=" + docId + "&documentProcessStatus=true&entityId=" + entityId;
                    var encodedUrl = HttpUtility.UrlEncode(url);
                    var deniedBillUrl = Path.Combine(companyUrl, "Home/Redirect/?url=" + encodedUrl);

                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##DeniedName##", userName);
                    content.Replace("##DeniedBillUrl##", deniedBillUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.completed:
                    formatedSubject = workflowTitle + " has been processed sucessfully";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowCompletedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(dailySalesReport.USER_ID);

                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##WorkFlowName##", workflowTitle);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                default:
                    break;
            }

            bodyMessage.Replace("##EmailContent##", content.ToString());
        }

        private void SetPayrollNotification(int userId, int workflowId, int entityId, string companyUrl, string compUrl,
          string WIPLogoEmailTemplate, int eventType, bool isPreUser, int? preUserId, string copyRight,
          string workflowTitle, ref StringBuilder bodyMessage, ref string formatedSubject, ref string toEmailId)
        {
            bool isPayroll = true;
            var userName = "";
            var clientName = "";
            var initiatorName = "";
            var preApprovedName = "";
            var strEmailTemplatePath = "";
            var note = "";
            var payroll = _repPWfayroll.GetPayroll(entityId);
            userName = _srvAddressbook.GetContactNameByContactId(userId);
            clientName = _srvAddressbook.GetCompanyNameByCompanyId(payroll.CLIENT_ID);
            initiatorName = _srvAddressbook.GetContactNameByContactId(payroll.USER_ID);
            preApprovedName = userName;
            var customTable = SetStaticWorkFlowInvoiceReportTable(clientName, payroll.TITLE, "", isPayroll);
            if (isPreUser)
            {
                preApprovedName = _srvAddressbook.GetContactNameByContactId(preUserId.Value);
            }

            StringBuilder content = null;
            var strEmailTemplate = _srvBase.GetEmailTemplatePath(WIPLogoEmailTemplate, EmailTemplateType.WithRequiredUserName);
            bodyMessage = new StringBuilder(strEmailTemplate);

            switch ((WorkflowGloble.WorkFlowEmailEventType)eventType)
            {
                case WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting:
                    note = GetWorkflowNote(workflowId, entityId, payroll.APPROVAL_STATUS);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = workflowTitle + " needs your approval";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowApprovalEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }

                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", userName);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Approved:
                    note = GetWorkflowNote(workflowId, entityId, payroll.APPROVAL_STATUS);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = workflowTitle + " has been approved";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowApprovedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(payroll.USER_ID);

                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##ApprovedName##", preApprovedName);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Denied:
                    note = GetWorkflowNote(workflowId, entityId, payroll.APPROVAL_STATUS);
                    if (note == null)
                    {
                        note = "";
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(payroll.USER_ID);
                    formatedSubject = workflowTitle + " has been Denied";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowDeniedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, workflowId);
                    var url = "/Payroll/Payroll?payrollId=" + entityId + "&docId=" + docId + "&clientId=" + payroll.CLIENT_ID + "&vendorId=0&workFlowId=" + workflowId + "&documentProcessStatus=false";
                    var encodedUrl = HttpUtility.UrlEncode(url);
                    var deniedBillUrl = Path.Combine(companyUrl, "Home/Redirect/?url=" + encodedUrl);

                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##DeniedName##", userName);
                    content.Replace("##DeniedBillUrl##", deniedBillUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.completed:
                    formatedSubject = workflowTitle + " has been processed sucessfully";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowCompletedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(payroll.USER_ID);

                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##WorkFlowName##", workflowTitle);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                default:
                    break;
            }

            bodyMessage.Replace("##EmailContent##", content.ToString());
        }

        private void SetPayrollReportNotification(int userId, int workflowId, int entityId, string companyUrl, string compUrl,
            string WIPLogoEmailTemplate, int eventType, bool isPreUser, int? preUserId, string copyRight, string workflowTitle,
            ref StringBuilder bodyMessage, ref string formatedSubject, ref string toEmailId)
        {
            bool isPayroll = true;
            var userName = "";
            var clientName = "";
            var initiatorName = "";
            var preApprovedName = "";
            var strEmailTemplatePath = "";
            var note = "";
            var payrollReport = _repPayrollReport.GetPayrollReport(entityId);
            userName = _srvAddressbook.GetContactNameByContactId(userId);
            clientName = _srvAddressbook.GetCompanyNameByCompanyId(payrollReport.CLIENT_ID);
            initiatorName = _srvAddressbook.GetContactNameByContactId(payrollReport.USER_ID);
            preApprovedName = userName;
            var customTable = SetStaticWorkFlowInvoiceReportTable(clientName, payrollReport.TITLE, "", isPayroll);
            if (isPreUser)
            {
                preApprovedName = _srvAddressbook.GetContactNameByContactId(preUserId.Value);
            }

            StringBuilder content = null;
            var strEmailTemplate = _srvBase.GetEmailTemplatePath(WIPLogoEmailTemplate, EmailTemplateType.WithRequiredUserName);
            bodyMessage = new StringBuilder(strEmailTemplate);

            switch ((WorkflowGloble.WorkFlowEmailEventType)eventType)
            {
                case WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting:
                    note = GetWorkflowNote(workflowId, entityId, payrollReport.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = workflowTitle + " needs your approval";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowApprovalEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }

                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", userName);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Approved:
                    note = GetWorkflowNote(workflowId, entityId, payrollReport.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = workflowTitle + " has been approved";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowApprovedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(payrollReport.USER_ID);

                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##ApprovedName##", preApprovedName);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Denied:
                    note = GetWorkflowNote(workflowId, entityId, payrollReport.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(payrollReport.USER_ID);
                    formatedSubject = workflowTitle + " has been Denied";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowDeniedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, workflowId);
                    var url = "/PayrollReport/PayrollReport?workFlowId=" + workflowId + "&docId=" + docId + "&documentProcessStatus=false&entityId=" + entityId;
                    var encodedUrl = HttpUtility.UrlEncode(url);
                    var deniedBillUrl = Path.Combine(companyUrl, "Home/Redirect/?url=" + encodedUrl);

                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##DeniedName##", userName);
                    content.Replace("##DeniedBillUrl##", deniedBillUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.completed:
                    formatedSubject = workflowTitle + " has been processed sucessfully";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowCompletedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(payrollReport.USER_ID);

                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##WorkFlowName##", workflowTitle);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                default:
                    break;
            }
            bodyMessage.Replace("##EmailContent##", content.ToString());
        }

        private void SetCustomWorkFlowNotification(int userId, int workflowId, int entityId, string companyUrl, string compUrl,
            string WIPLogoEmailTemplate, int eventType, bool isPreUser, int? preUserId, string copyRight, string workflowTitle,
            ref StringBuilder bodyMessage, ref string formatedSubject, ref string toEmailId)
        {
            bool isClientSpecific = false;
            var strEmailTemplatePath = "";
            var note = "";
            var customWorkflow = _repCustomWorkflow.GetCustomWorkFlow(entityId);
            var userName = _srvAddressbook.GetContactNameByContactId(userId);
            var initiatorName = _srvAddressbook.GetContactNameByContactId(customWorkflow.USER_ID);
            var preApprovedName = userName;
            var customTable = "";
            var clientName = "";
            if (isPreUser)
            {
                preApprovedName = _srvAddressbook.GetContactNameByContactId(preUserId.Value);
            }
            if (customWorkflow.CLIENT_ID.HasValue)
            {
                isClientSpecific = true;
                clientName = _srvAddressbook.GetCompanyNameByCompanyId(customWorkflow.CLIENT_ID.Value);
            }
            customTable = SetCustomWorkflowInvoiceReportTable(clientName, customWorkflow.TITLE, isClientSpecific, entityId);

            StringBuilder content = null;
            var strEmailTemplate = _srvBase.GetEmailTemplatePath(WIPLogoEmailTemplate, EmailTemplateType.WithRequiredUserName);
            bodyMessage = new StringBuilder(strEmailTemplate);


            switch ((WorkflowGloble.WorkFlowEmailEventType)eventType)
            {
                case WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting:
                    note = GetWorkflowNote(workflowId, entityId, customWorkflow.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = workflowTitle + " needs your approval";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowApprovalEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }

                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", userName);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Approved:
                    note = GetWorkflowNote(workflowId, entityId, customWorkflow.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    formatedSubject = workflowTitle + " has been approved";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowApprovedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(customWorkflow.USER_ID);
                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##ApprovedName##", preApprovedName);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.Denied:
                    note = GetWorkflowNote(workflowId, entityId, customWorkflow.APPROVAL_STATUS.Value);
                    if (note == null)
                    {
                        note = "";
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(customWorkflow.USER_ID);
                    formatedSubject = workflowTitle + " has been Denied";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowDeniedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, workflowId);
                    var url = "/CustomWorkflow/CustomWorkflow?workFlowId=" + workflowId + "&docId=" + docId + "&documentProcessStatus=false&entityId=" + entityId;
                    var encodedUrl = HttpUtility.UrlEncode(url);
                    var deniedBillUrl = Path.Combine(companyUrl, "Home/Redirect/?url=" + encodedUrl);
                    content.Replace("##WorkFlowName##", workflowTitle);
                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##DeniedName##", userName);
                    content.Replace("##DeniedBillUrl##", deniedBillUrl);
                    content.Replace("##Note##", note);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                case WorkflowGloble.WorkFlowEmailEventType.completed:
                    formatedSubject = workflowTitle + " has been processed sucessfully";
                    strEmailTemplatePath = GetEmailTemplatePath("CustomWorkFlowCompletedEmailTemplate.html");
                    using (var streamReader = new StreamReader(strEmailTemplatePath))
                    {
                        content = new StringBuilder(streamReader.ReadToEnd());
                    }
                    toEmailId = _srvAddressbook.GetContactEmail(customWorkflow.USER_ID);
                    bodyMessage.Replace("##UserName##", initiatorName);
                    content.Replace("##WorkFlowName##", workflowTitle);
                    content.Replace("##WIP##", compUrl);
                    content.Replace("##InvoiceTable##", customTable);
                    content.Replace("##CopyRight##", copyRight);
                    break;
                default:
                    break;
            }

            bodyMessage.Replace("##EmailContent##", content.ToString());


        }

        private string SetStaticWorkFlowInvoiceReportTable(string clientName, string invoiceTitle, string amount, bool isPayroll)
        {
            string tblHtml = "";
            if (isPayroll)
            {
                tblHtml += "<table border='1' style='font-family: verdana,arial,sans-serif; font-size: 12px; color: #333333; border-width: 1px; border-color: #999999; border-collapse: collapse;' width='100%'>";
                tblHtml += "<tr>";
                tblHtml += "<td style='width: 60%; border-width: 1px; padding: 8px; border-style: solid; font-weight: bold; border-color: #999999; '>Client</td>";
                tblHtml += "<td style='width: 40%; border-width: 1px; padding: 8px; border-style: solid; font-weight: bold; border-color: #999999; '>Invoice# </td>";
                tblHtml += "</tr>";
                tblHtml += "<tr>";
                tblHtml += "<td style='border-width: 1px; padding: 8px; border-style: solid; border-color: #999999; '>" + clientName + "</td>";
                tblHtml += "<td style='border-width: 1px; padding: 8px; border-style: solid; border-color: #999999; '>" + invoiceTitle + "</td>";
                tblHtml += "</tr>";
                tblHtml += "</table>";
            }
            else
            {
                tblHtml += "<table border='1' style='font-family: verdana,arial,sans-serif; font-size: 12px; color: #333333; border-width: 1px; border-color: #999999; border-collapse: collapse;' width='100%'>";
                tblHtml += "<tr>";
                tblHtml += "<td style='width: 50%; border-width: 1px; padding: 8px; border-style: solid; font-weight: bold; border-color: #999999; '>Client</td>";
                tblHtml += "<td style='width: 30%; border-width: 1px; padding: 8px; border-style: solid; font-weight: bold; border-color: #999999; '>Invoice# </td>";
                tblHtml += "<td style='width: 20%; border-width: 1px; padding: 8px; border-style: solid; font-weight: bold; border-color: #999999; '>Amount</td>";
                tblHtml += "</tr>";
                tblHtml += "<tr>";
                tblHtml += "<td style='border-width: 1px; padding: 8px; border-style: solid; border-color: #999999; '>" + clientName + "</td>";
                tblHtml += "<td style='border-width: 1px; padding: 8px; border-style: solid; border-color: #999999; '>" + invoiceTitle + "</td>";
                tblHtml += " <td style='border-width: 1px; padding: 8px; border-style: solid; border-color: #999999; '>" + amount + "</td>";
                tblHtml += "</tr>";
                tblHtml += "</table>";
            }
            return tblHtml;
        }

        private string SetCustomWorkflowInvoiceReportTable(string clientName, string invoiceTitle, bool isClientSpecific, int customWorkflowId)
        {
            string tblHtml = "";
            string customValue = "";
            var customField = _repCustomWorkflow.GetCustomWorkflowLineItems(customWorkflowId).FirstOrDefault();
            if (customField != null)
            {
                var dataTypeId = customField.DATA_TYPE_ID;
                var fieldLabel = customField.FIELD_LABEL;
                switch ((ECMGlobal.MetadataType)dataTypeId)
                {
                    case ECMGlobal.MetadataType.Boolean:
                        var booleanValue = customField.BOOLEAN_VALUE;
                        customValue = "False";
                        if (booleanValue.HasValue)
                        {
                            if (booleanValue.Value)
                            {
                                customValue = "Yes";
                            }
                        }
                        break;
                    case ECMGlobal.MetadataType.Integer:
                        if (customField.INTEGER_VALUE.HasValue)
                            customValue = customField.INTEGER_VALUE.ToString();
                        break;
                    case ECMGlobal.MetadataType.Decimal:
                        if (customField.DECIMAL_VALUE.HasValue)
                            customValue = customField.DECIMAL_VALUE.ToString();
                        break;
                    case ECMGlobal.MetadataType.DateTime:
                        if (customField.DATETIME_VALUE.HasValue)
                            customValue = customField.DATETIME_VALUE.Value.ToString("MM/dd/yyyy");
                        break;
                    case ECMGlobal.MetadataType.Text:
                        customValue = customField.TEXT_VALUE;
                        break;
                    case ECMGlobal.MetadataType.TextArea:
                        customValue = customField.TEXT_AREA_VALUE;
                        break;
                    case ECMGlobal.MetadataType.DropDown:
                        if (customField.DDL_VALUE.HasValue)
                            customValue = customField.DDL_TEXT.ToString();
                        break;
                    case ECMGlobal.MetadataType.MultipleDropDown:
                        customValue = customField.DDL_TEXT;
                        break;
                }
                customValue = "<span style='font-weight: bold;'>" + fieldLabel + ":&nbsp;</span>" + customValue + "<br />";
            }
            if (isClientSpecific)
            {
                tblHtml += "<div style='font-family: verdana,arial,sans-serif; font-size: 12px; color: #333333; font-weight: 200;'>";
                tblHtml += "<span style='font-weight: bold;'>Client:&nbsp;</span>" + clientName + "<br />";
                tblHtml += "<span style='font-weight: bold;'>Title:&nbsp;</span>" + invoiceTitle + "<br />";
                tblHtml += customValue;
                tblHtml += "</div>";
            }
            else
            {
                tblHtml += "<div style='font-family: verdana,arial,sans-serif; font-size: 12px; color: #333333; font-weight: 200;'>";
                tblHtml += "<span style='font-weight: bold;'>Title:&nbsp;</span>" + invoiceTitle + "<br />";
                tblHtml += customValue;
                tblHtml += "</div>";
            }
            return tblHtml;
        }
    }
}

