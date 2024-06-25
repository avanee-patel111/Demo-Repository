using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IWorkFlowRepository<T>
    {
        void SaveTobeProcessedWidget(WF_TOBEPROCESSED_WIDGET objTobeProcessed);
        WF_TOBEPROCESSED_WIDGET IsExistDocumentInProcessedWidget(int docId);
        int SaveBill(WF_BILLING_DETAIL objBillingDetail);
        int UpdateApproverStatsu(WF_BILLING_DETAIL objBillingDetail);
        void SaveWorkFlowDocument(WORKFLOW_DOCUMENT objDocument);
        int CheckNoOfInvoiceForSameVendor(string invoiceNumber, int VendorId);
        object GetInvoiceNumber(int docId, int pageNo, int workFlowId);
        List<int> GetAssociatedPages(int entityId, int workFlowId);
        List<int> GetAssociatedNewPages(int entityId, int newPageNo, int workFlowId);
        int GetEntityId(int docId, int pageNo, int workFlowId);
        int GetNewDocIdByEntityId(int entityId, int workFlowId);
        void SaveDocumentPages(int oldDocId, int newDocId, int userId);
        WORKFLOW_DOCUMENT GetDocumentByDocIdAndPageNo(int docId, int pageNo, int workFlowId);
        WF_BILLING_DETAIL GetBillByBillId(int BillId);
        int GetDocIdByEntityId(int entityId, int workFlowId);
        int UpdateBillCredit(WF_BILLING_DETAIL objBillingDetail);
        List<int> GetAssociatedPagesByDocId(int docId, int workFlowId);
        int GetAssociatedPagesCountByDocId(int docId);
        bool CheckPageIsExist(int entityId, int docId, int currentPage, int currentDocId, int workFlowId);
        void SaveChanges();
        int GetLastPageNumber(int billId, int workFlowId);
        List<WORKFLOW_DOCUMENT> GetWorkFlowDocument(int entityId, int workFlowId);
        int GetVenderIdByBillId(int billId);
        WF_TOBEPROCESSED_WIDGET GetToBeProcessedWidget(int docId);
        WF_TOBEPROCESSED_WIDGET GetToBeProcessedById(int toProcessedId);
        void DeleteToBeProcessed(WF_TOBEPROCESSED_WIDGET objTobeProcessed);
        int SaveWorkFlow(WORKFLOW_DEFINITION documentWorkflow);
        void SaveDocumentWorkFlowStep(WORKFLOW_DEFINITION_STEPS documentWorkFlowStep);
        WORKFLOW_DEFINITION GetDocumentWorkFlow(int workFlowId);
        List<WORKFLOW_DEFINITION_STEPS> GetDocumentWorkFlowStep(int workFlowId);
        void SaveDocumentWorkFlowApproval(WF_DOCUMENT_WORKFLOW_APPROVAL documentWorkFlowApproval);
        List<WF_DOCUMENT_WORKFLOW_APPROVAL> GetDocumentWorkFlowApproval(int workFlowId);
        void SaveDocumentWorkFlowEscalation(WORKFLOW_DEFINITION_ESCALATION documentWorkFlowEscalation);
        WORKFLOW_DEFINITION_ESCALATION GetDocumentWorkFlowEscalationForUpdate(int workFlowId);
        WORKFLOW_DEFINITION_ESCALATION GetDocumentWorkFlowEscalation(int workFlowId);
        bool CheckWorkFlowStepFolderIdExist(int folderId, int workFlowId);
        List<int> GetAssocitePages(int docId, int workFlowId);
        COMMON_FIELDS GetWorkFlowCommonFields(int workFlowId);
        WF_VENDOR_CREDIT_DETAIL GetVenderCreditByCreditId(int vendorCreditId);
        bool IsDocumentApproved(int workFlowId, int entityId);
        WORKFLOW_DOCUMENT_ENTITIES GetDocumentEntity(int workFlowId, int entityId);
        void SaveDocumentEntity(WORKFLOW_DOCUMENT_ENTITIES objDocument);
        void SaveWorkflowTimeLine(WF_TIME_LINE timeLine);
        string GetLastWorkFlowNote(int workFlowId, int entityId, int approvalStatus);
        string GetWorkflowTitleById(int workFlowId);
        int GetWorkflowInitiator(int workFlowId, int entityId);
        WF_PAYROLL GetPayroll(int entityId);
        WF_DSR_REPORT GetDailySalesReport(int dsrId);
        WORKFLOW_CUSTOM GetCustomWorkFlow(int customWorkflowId);
        WF_PAYROLL_REPORT GetPayrollReport(int payrollId);

        WF_TOBEPROCESSED_WIDGET GetToBeProcessedDeletedDocumentByDocId(int originalId);

        IEnumerable<WORKFLOW_REVIEWER> GetSubItemsByListId(int listId);
    }
}
