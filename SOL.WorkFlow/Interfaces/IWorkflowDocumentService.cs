using SOL.Common.Models;
using SOL.ECM.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IWorkflowDocumentService<T>
    {
        /// <summary>
        /// Moves the document in to be reviewed folder.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="originelDocId">The originel document identifier.</param>
        /// <param name="projectId">The project identifier.</param>
        void MoveDocumentInToBeReviewedFolder(int userId, UserType userType,
       ref string errorMessage, int docId, int originelDocId, int projectId);

        /// <summary>
        /// Moves the document in to be reviewed folder by step.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="originelDocId">The originel document identifier.</param>
        /// <param name="folderId">The folder identifier.</param>
        void MoveDocumentInToBeReviewedFolderByStep(int userId, UserType userType, ref string errorMessage, int docId,
            int originelDocId, int folderId);

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
        bool IsDocumentExistInFolder(int userId, UserType userType, ref string errorMessage,
           int docId, ref int originelDocId, ECM_DOCUMENTS document, string originalName, int toBeProcessedFolderId);

        /// <summary>
        /// Cuts the docucmet.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="originelDocId">The originel document identifier.</param>
        /// <param name="destinationFolderId">The destination folder identifier.</param>
        void MoVeDocumentToCompletedFolder(int userId, UserType userType, ref string errorMessage, int docId,
             int originelDocId, string vendorName, int workFlowId);

        /// <summary>
        /// Moes the ve document to denied folder.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="originelDocId">The originel document identifier.</param>
        /// <param name="workFlowId">The work flow identifier.</param>
        void MoVeDocumentToDeniedFolder(int userId, UserType userType, ref string errorMessage, int docId,
           int originelDocId, int workFlowId);

        /// <summary>
        /// Manages the bill documents.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <param name="addedBy">The added by.</param>
        /// <param name="approvalStatus">The approval status.</param>
        /// <param name="genrateDocument">if set to <c>true</c> [genrate document].</param>
        /// <param name="isCheckOriginalName">if set to <c>true</c> [is check original name].</param>
        /// <param name="objBillingView">The object billing view.</param>
        /// <param name="associtaedPages">The associtaed pages.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="originelDocId">The originel document identifier.</param>
        /// <param name="newDocId">The new document identifier.</param>
        /// <param name="isClosePage">if set to <c>true</c> [is close page].</param>
        /// <param name="IsApproveMode">if set to <c>true</c> [is approve mode].</param>
        /// <param name="vendorId">The vendor identifier.</param>
        void ManageBillDocuments(int entityId, int workFlowId, string addedBy, byte? approvalStatus,
            bool genrateDocument, bool isCheckOriginalName, int[] associtaedPages,
            int userId, UserType userType, ref string errorMessage, int projectId, int docId, ref int originelDocId,
            ref int newDocId, ref bool isClosePage, bool IsApproveMode, string newDocuementName);

        /// <summary>
        /// Saves the work flow documents.
        /// </summary>
        /// <param name="selectedPages">The selected pages.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="newDocId">The new document identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <param name="newPageNumber">The new page number.</param>
        void SaveWorkFlowDocuments(int[] selectedPages, int userId, int docId, int newDocId, int entityId, int workFlowId, int newPageNumber, ref string errorMessage);

        /// <summary>
        /// Manages the custom flow documents.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="isClosePage">if set to <c>true</c> [is close page].</param>
        void ManageCustomFlowDocuments(int entityId, int workFlowId, List<DocumentWorkFlow> docId, int userId, int processFolderId, int toBeReviewdFolderId,
            UserType userType, int approvalStatus, byte workFlowLevel, ref int newDocId, ref int originelDocId, ref bool isClosePage,
            string newDocuementName, int[] associtaedPages, string addedBy, ref string errorMessage);

        /// <summary>
        /// Manages the custom workflow documents.
        /// </summary>
        /// <param name="customWorkflowId">The custom workflow identifier.</param>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <param name="addedBy">The added by.</param>
        /// <param name="approvalStatus">The approval status.</param>
        /// <param name="genrateDocument">if set to <c>true</c> [genrate document].</param>
        /// <param name="isCheckOriginalName">if set to <c>true</c> [is check original name].</param>
        /// <param name="associtaedPages">The associtaed pages.</param>
        /// <param name="workFlowLevel">The work flow level.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="originelDocId">The originel document identifier.</param>
        /// <param name="newDocId">The new document identifier.</param>
        /// <param name="isClosePage">if set to <c>true</c> [is close page].</param>
        /// <param name="IsApproveMode">if set to <c>true</c> [is approve mode].</param>
        /// <param name="customWorkflowTitle">The custom workflow title.</param>
        void ManageCustomWorkflowDocuments(int customWorkflowId, int workFlowId, string addedBy, byte? approvalStatus, bool genrateDocument,
            bool isCheckOriginalName, int[] associtaedPages, byte workFlowLevel, int userId, UserType userType, ref string errorMessage,
            int projectId, List<DocumentWorkFlow> docId, ref int originelDocId, ref int newDocId, ref bool isClosePage, bool IsApproveMode, string customWorkflowTitle);

        /// <summary>
        /// Manages the old document.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="isClosePage">if set to <c>true</c> [is close page].</param>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <returns></returns>
        bool ManageOldDocument(int userId, int docId, bool isClosePage);

        /// <summary>
        /// Uns the associate bill page.
        /// </summary>
        /// <param name="newDocId">The new document identifier.</param>
        /// <param name="originalDocId">The original document identifier.</param>
        /// <param name="pageNo">The page no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="addedBy">The added by.</param>
        /// <returns></returns>
        object UnAssociatePage(int newDocId, int originalDocId, int pageNo,
           int userId, UserType userType, string addedBy, int workFlowId);

        /// <summary>
        /// Copies the document from ecm.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        int CopyDocumentFromEcm(int projectId, int docId, int userId, UserType userType,
            ref string errorMessage);
       int CopyDocumentToFolder(int docId, int destinationFolderId, int workflowId, int userId, UserType userType,
           ref string errorMessage);
        int UploadDocument(UploadDocument uploadDocument, int projectId, ref string errorMessage);
        int UploadDocumentByFolder(UploadDocument uploadDocument, int folderId, ref string errorMessage);
        object GetWorkFlowDocumentDetail(int entityId, int workFlowId);

        int? GetPagesCount(int docId);

        void DeletedDocument(int docId, int userId);

        bool SaveAddToExistingPage(int entityId, int workFlowId, int[] selectedPages, int docId, int userId, UserType userType,
           string addedBy, ref string errorMessage);

        WORKFLOW_DOCUMENT_ENTITIES GetDocumentEntity(int workFlowId, int entityId);
        void SaveDocumentEntity(int workFlowId, int entityId, List<DocumentWorkFlow> docId, int newDocId, int userId);

        void MoVeDocumentToCompletedFolderByStep(int userId, UserType userType, int docId, int folderId, ref string errorMessage);

        void MoVeDocumentToDeniedFolderByStep(int userId, UserType userType, int docId, int folderId, ref string errorMessage);

        void SaveWorkFlowEmailEventEntryInOutBound(int userId, int workflowId, int entityId, string companyUrl,
            string companyLogo, int eventType, bool isPreUser, int? preUserid);
       
    }
}
