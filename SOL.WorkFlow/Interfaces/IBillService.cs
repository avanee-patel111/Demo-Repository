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
    public interface IBillService<T>
    {
        /// <summary>
        /// Saves the bill.
        /// </summary>
        /// <param name="objBillingViewModel">The object billing view model.</param>
        /// <param name="addedBy">The added by.</param>
        /// <returns></returns>
        object SaveBill(BillingDetailsViewModel objBillingViewModel, string companyUrl,
            string companyLogo, string addedBy);
        /// <summary>
        /// Gets the new document identifier by bill identifier.
        /// </summary>
        /// <param name="billId">The bill identifier.</param>
        /// <returns></returns>
        int GetNewDocIdByEntityId(int entityId, int workFlowId);
        /// <summary>
        /// Gets the bill identifier.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <param name="pageNo">The page no.</param>
        /// <returns></returns>
        int GetEntityId(int docId, int pageNo, int workFlowId);
        WF_BILLING_DETAIL GetBillDetailById(int billId);
        int UpdateBillCredit(WF_BILLING_DETAIL objBillDetail);
        /// <summary>
        /// Gets the invoice number.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <param name="pageNo">The page no.</param>
        /// <returns></returns>       
        object GetInvoiceNumber(int docId, int pageNo, int workFlowId);
        /// <summary>
        /// Gets the associated pages by document identifier.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <returns></returns>
        List<int> GetAssociatedPagesByDocId(int docId, int workFlowId);
        /// <summary>
        /// Gets the associated pages.
        /// </summary>
        /// <param name="entityId">The bill identifier.</param>
        /// <returns></returns>
        List<int> GetAssociatedPages(int entityId, int workFlowId);
        /// <summary>
        /// Saves the tobe processed widget.
        /// </summary>
        /// <param name="triggerEventModel">The trigger event model.</param>
        void SaveTobeProcessedWidget(DocumentUploadedModel triggerEventModel);
        /// <summary>
        /// Deletes to be processed.
        /// </summary>
        /// <param name="toProcessedId">To processed identifier.</param>
        /// <param name="userId">The user identifier.</param>
        void DeleteToBeProcessed(int toProcessedId, int userId);
        /// <summary>
        /// Checks the no of invoice for same vendor.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="VendorId">The vendor identifier.</param>
        /// <returns></returns>
        int CheckNoOfInvoiceForSameVendor(string invoiceNumber, int VendorId);
        /// <summary>
        /// Uploads the document.
        /// </summary>
        /// <param name="uploadDocument">The upload document.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        int UploadDocument(UploadDocument uploadDocument, int clientId, ref string errorMessage);
        /// <summary>
        /// Uploads the document.
        /// </summary>
        /// <param name="uploadDocument">The upload document.</param>
        /// <param name="folderId">The folderId identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        int UploadDocumentbyFolderId(UploadDocument uploadDocument, int folderId, ref string errorMessage);
        /// <summary>
        /// Copies the document from ecm.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        int CopyDocumentFromEcm(int docId, int clientId, int userId, UserType userType, ref string errorMessage);

        int CopyDocumentToFolder(int docId, int destinationfolderId, int workflowId, int userId, UserType userType, ref string errorMessage);

        /// <summary>
        /// Gets the bill document detail.
        /// </summary>
        /// <param name="billId">The bill identifier.</param>
        /// <returns></returns>
        object GetWorkFlowDocumentDetail(int billId, int workFlowId);
        /// <summary>
        /// Gets the document identifier by bill identifier.
        /// </summary>
        /// <param name="entityId">The bill identifier.</param>
        /// <returns></returns>
        int GetDocIdByEntityId(int entityId, int workFlowId);
        /// <summary>
        /// Gets the pages count.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <returns></returns>
        int? GetPagesCount(int docId);
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
        bool SaveAddToExistingPage(int entityId, int workFlowId, int[] selectedPages, int docId, int userId, UserType userType,
            string addedBy, ref string errorMessage);
        /// <summary>
        /// Approves the bill status.
        /// </summary>
        /// <param name="objBillingViewModel">The object billing view model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        int ApproveBillStatus(UpdateStatusViewModel objBillingViewModel, int userId, UserType userType, string companyUrl,
            string companyLogo);

        object UnAssociatePage(int newDocId, int originalDocId, int pageNo,
          int userId, UserType _userType, string addedBy, int p4);

        List<int> GetWorkflowUsers(int entityId, int workFlowId);
    }
}
