using SOL.Addressbook.Interfaces;
using SOL.Common.Models;
using SOL.Common.Business.Services;
using SOL.ECM.Models;
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
    public class DailySalesReportService<T> : BaseService<T>, IDailySalesReportService<T>
    {
        IDailySalesReportRepository<int> _repDailySalesReport = null;
        IPmsService<T> _srvPms;
        IWorkflowDocumentService<T> _srvWfDocumentService = null;
        IApprovalService<T> _srvApproval;
        IWorkFlowRepository<T> _repWorkFlow;
        IWorkflowTimelineService<T> _srvTimeLine = null;

        public DailySalesReportService(IDailySalesReportRepository<int> repDailySalesReport,
            IAddressbookRepository<T> repAdrbook, IPmsService<T> srvPms, IWorkFlowRepository<T> repWorkFlow,
             IApprovalService<T> srvApproval, IWorkflowDocumentService<T> srvWfDocumentService, IWorkflowTimelineService<T> srvTimeLine)
            : base(repAdrbook)
        {
            this._repDailySalesReport = repDailySalesReport;
            this._srvPms = srvPms;
            this._srvWfDocumentService = srvWfDocumentService;
            this._srvApproval = srvApproval;
            this._repWorkFlow = repWorkFlow;
            this._srvTimeLine = srvTimeLine;
        }
        
        public object SaveDailySalesReport(DsrReportModel dsReportModel, int userId, UserType userType, string addedBy,
            ref string errorMessage, string companyLogo, string companyUrl)
        {
            var obj = new object();
            var dsReport = new WF_DSR_REPORT();
            var dsReportSalesCategory = dsReportModel.DsrReportSalesCategory;
            var dsReportLineItems = dsReportModel.DsrReportLineItems;
            bool genrateDocument = true;
            bool isCheckOriginalName = false;
            var documentOperation = new DocumentOperation();
            var dsrId = dsReportModel.DSR_ID;
            var oldDsrId = dsrId;
            var dsReportTitle = dsReportModel.TITLE;
            var approvalStatus = dsReportModel.APPROVAL_STATUS;
            var isExistSalesReportTitle = _repDailySalesReport.isExistSalesReportTitle(dsReportTitle, dsrId);
            if (isExistSalesReportTitle == true)
            {
                errorMessage = dsReportTitle + " already exists in your records. To add this document to the existing Report, click on Add to Existing.";
                return obj;
            }
            var clientId = dsReportModel.CLIENT_ID;
            if (dsrId == 0)
            {
                dsReport.CLIENT_ID = clientId;
                dsReport.DATE_OF_ENTRY = DateTime.UtcNow;
                dsReport.USER_ID = userId;
                dsReport.DELETED_FLAG = false;
            }
            else
            {
                dsReport = _repDailySalesReport.GetDailySalesReport(dsrId);
                clientId = dsReport.CLIENT_ID;
                genrateDocument = false;
            }
            int projectId = _srvPms.GetProjectIdByClientId(clientId);
            var workFlowId = dsReportModel.WORKFLOW_ID;
            dsReport.DATE_MODIFIED = DateTime.UtcNow;
            dsReport.USER_MODIFIED = userId;
            dsReport.REVIEWER_STATUS = approvalStatus;
            dsReport.TITLE = dsReportTitle;
            dsReport.DSR_DATE = dsReportModel.DSR_DATE;
            dsReport.DESCRIPTION = dsReportModel.DESCRIPTION;
            dsReport.OPERATING_HOURS = dsReportModel.OPERATING_HOURS;
            dsReport.NO_OF_CHECKS = dsReportModel.NO_OF_CHECKS;
            dsReport.WORKFLOW_DEFINITION_ID = dsReportModel.WORKFLOW_ID;
            dsReport.TOTAL_SALES = dsReportModel.TOTAL_SALES;
            dsReport.AVERAGE_TOTAL = dsReportModel.AVERAGE_TOTAL;
            dsReport.AMEX_CC_RECEIPTS = dsReportModel.AMEX_CC_RECEIPTS;
            dsReport.AMEX_CC_TIPS = dsReportModel.AMEX_CC_TIPS;
            dsReport.AMEX_CC_SALES_TEX = dsReportModel.AMEX_CC_SALES_TEX;
            dsReport.M_V_D_CC_RECEIPTS = dsReportModel.M_V_D_CC_RECEIPTS;
            dsReport.M_V_D_CC_TIPS = dsReportModel.M_V_D_CC_TIPS;
            dsReport.M_V_D_CC_SALES_TEX = dsReportModel.M_V_D_CC_SALES_TEX;
            dsReport.TOTAL_CC_RECEIPTS = dsReportModel.TOTAL_CC_RECEIPTS;
            dsReport.TOTAL_CC_TIPS = dsReportModel.TOTAL_CC_TIPS;
            dsReport.TOTAL_CC_SALES_TEX = dsReportModel.TOTAL_CC_SALES_TEX;
            dsReport.NET_CC_SALES = dsReportModel.NET_CC_SALES;
            dsReport.CC_TIPS_PERCENTAGE = dsReportModel.CC_TIPS_PERCENTAGE;
            dsReport.CASE_SALES = dsReportModel.CASE_SALES;
            dsReport.WF_DSR_REPORT_LINE_ITEM = SetCustomField(dsReportLineItems,
                dsReport.WF_DSR_REPORT_LINE_ITEM, userId);
            dsReport.WF_DSR_REPORT_CATEGORY = SetDsReportSalesCategory(dsReportSalesCategory,
               dsReport.WF_DSR_REPORT_CATEGORY, userId);
            _repDailySalesReport.SaveDailySalesReport(dsReport);
            dsrId = dsReport.DSR_ID;
            if (genrateDocument == true)
            {
                _srvTimeLine.SaveWorflowTimeline(dsrId, workFlowId, approvalStatus, null, null,
                (int)WorkflowTimelineObject.DSR, (int)WorkflowTimelineEvents.DSR_Created, string.Empty, userId, DateTime.UtcNow, null, null);
            }
            else
            {
                _srvTimeLine.SaveWorflowTimeline(dsrId, workFlowId, approvalStatus, null, null,
                (int)WorkflowTimelineObject.DSR, (int)WorkflowTimelineEvents.DSR_Updated, string.Empty, userId, DateTime.UtcNow, null, null);
            }
            var docId = dsReportModel.DOC_ID;
            var documentId = dsReportModel.documentId;
            var originelDocId = docId;
            var newDocId = 0;
            var isClosePage = true;
            var IsApproveMode = dsReportModel.IsApproveMode;
            var associtaedPages = dsReportModel.AssocitaedPages;
            byte ByOverView = 0;
            if (oldDsrId == 0)
            {
                _srvWfDocumentService.ManageBillDocuments(dsrId, workFlowId, addedBy, approvalStatus,
                       genrateDocument, isCheckOriginalName, associtaedPages,
                        userId, userType, ref errorMessage, projectId, docId, ref originelDocId,
                       ref newDocId, ref isClosePage, IsApproveMode, dsReportModel.TITLE);
                _srvWfDocumentService.SaveDocumentEntity(workFlowId, dsrId, documentId, newDocId, userId);
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
            if (approvalStatus != (int)FlowStatus.Draft)
            {
                if (dsReportModel.Approvers != null)
                {
                    _srvApproval.SaveApprovers(dsReportModel.Approvers, dsReportModel.NOTE_TO_PAYER, dsrId,
                        workFlowId, userId, approvalStatus, ByOverView, companyLogo);
                }               
            }
            SendPreEmailNotification(dsReportModel.Approvers, userId, workFlowId, dsrId, companyLogo, companyUrl);
            obj = new
            {
                originelDocId = originelDocId,
                isClosePage = isClosePage,
                dsrId = dsrId,
                newDocId = newDocId
            };
            return obj;
        }

        public void ApproveDailySalesReportStatus(UpdateStatusViewModel updateStatusViewModel, int userId, UserType userType, string companyLogo, string companyUrl)
        {
            string errorMessage = string.Empty;
            var workFlowId = (int)CustomWorkflowTypes.DSReport;
            var flowStatus = updateStatusViewModel.Status;
            var entityId = updateStatusViewModel.EntityId;
            _srvApproval.ManageApproversOnApprovals(updateStatusViewModel.Approvers, updateStatusViewModel.ApproverNote,null,
                flowStatus, entityId, workFlowId, userId, companyLogo);
            var dsReport = _repDailySalesReport.GetDailySalesReport(entityId);
            var isAnyPendingAprrovers = _srvApproval.IsAnyPendingAprrover(entityId, workFlowId);
            var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, updateStatusViewModel.WorkFlowId);
            if (flowStatus == (int)FlowStatus.Denied)
            {
                dsReport.REVIEWER_STATUS = flowStatus;
                _srvWfDocumentService.MoVeDocumentToDeniedFolder(userId, userType, ref errorMessage,
                       docId, docId, workFlowId);
            }
            else if (isAnyPendingAprrovers == false)
            {
                dsReport.REVIEWER_STATUS = flowStatus;
                _srvWfDocumentService.MoVeDocumentToCompletedFolder(userId, userType, ref errorMessage,
                            docId, docId, string.Empty, workFlowId);
            }            
            dsReport.USER_MODIFIED = userId;
            dsReport.DATE_MODIFIED = DateTime.UtcNow;
            _repDailySalesReport.SaveDailySalesReport(dsReport);
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

        private List<WF_DSR_REPORT_LINE_ITEM> SetCustomField(IEnumerable<DsrReportLineItems> reportLineItems,
          ICollection<WF_DSR_REPORT_LINE_ITEM> oldCustomeFields, int userId)
        {
            if (oldCustomeFields != null)
            {
                foreach (var customField in oldCustomeFields)
                {
                    customField.DATE_MODIFIED = DateTime.UtcNow;
                    customField.USER_MODIFIED = userId;
                    customField.DELETED_FLAG = true;
                    customField.BOOLEAN_VALUE = null;
                    customField.DATETIME_VALUE = null;
                    customField.DDL_VALUE = null;
                    customField.DECIMAL_VALUE = null;
                    customField.INTEGER_VALUE = null;
                    customField.TEXT_AREA_VALUE = null;
                    customField.TEXT_VALUE = null;
                }
            }
            if (reportLineItems != null)
            {
                var labelList = new List<string>();
                foreach (var reportLineItem in reportLineItems)
                {
                    var isNew = false;
                    var id = reportLineItem.CUSTOM_FIELD_CONFIGURATION_ID;
                    var dataTypeId = Convert.ToByte(reportLineItem.DATA_TYPE_ID);
                    var customFieldTypeId = reportLineItem.CUSTOM_FIELD_TYPE_ID;
                    WF_DSR_REPORT_LINE_ITEM documentMetadataEntity;
                    if (oldCustomeFields != null && oldCustomeFields.Count() > 0)
                    {
                        documentMetadataEntity = oldCustomeFields.Where(x => x.CUSTOM_FIELD_CONFIGURATION_ID == id).FirstOrDefault();
                        if (documentMetadataEntity == null)
                        {
                            documentMetadataEntity = new WF_DSR_REPORT_LINE_ITEM();
                            documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                            documentMetadataEntity.USER_ID = userId;
                            isNew = true;
                        }
                    }
                    else
                    {
                        documentMetadataEntity = new WF_DSR_REPORT_LINE_ITEM();
                        documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        documentMetadataEntity.USER_ID = userId;
                        isNew = true;
                    }
                    documentMetadataEntity.CUSTOM_FIELD_TYPE_ID = customFieldTypeId;
                    documentMetadataEntity.CUSTOM_FIELD_CONFIGURATION_ID = id;
                    documentMetadataEntity.DATA_TYPE_ID = dataTypeId;
                    documentMetadataEntity.DATE_MODIFIED = DateTime.UtcNow;
                    documentMetadataEntity.USER_MODIFIED = userId;
                    documentMetadataEntity.DELETED_FLAG = false;

                    switch ((ECMGlobal.MetadataType)dataTypeId)
                    {
                        case ECMGlobal.MetadataType.Boolean:
                            var booleanValue = reportLineItem.BOOLEAN_VALUE;
                            documentMetadataEntity.BOOLEAN_VALUE = booleanValue;
                            break;
                        case ECMGlobal.MetadataType.Integer:
                            var integerValue = reportLineItem.INTEGER_VALUE;
                            documentMetadataEntity.INTEGER_VALUE = integerValue;
                            break;
                        case ECMGlobal.MetadataType.Decimal:
                            var decimalValue = reportLineItem.DECIMAL_VALUE;
                            documentMetadataEntity.DECIMAL_VALUE = decimalValue;
                            break;
                        case ECMGlobal.MetadataType.DateTime:
                            var dateTimeValue = reportLineItem.DATETIME_VALUE;
                            documentMetadataEntity.DATETIME_VALUE = dateTimeValue;
                            break;
                        case ECMGlobal.MetadataType.Text:
                            var textValue = reportLineItem.TEXT_VALUE;
                            documentMetadataEntity.TEXT_VALUE = textValue;
                            break;
                        case ECMGlobal.MetadataType.TextArea:
                            var textAreaValue = reportLineItem.TEXT_AREA_VALUE;
                            documentMetadataEntity.TEXT_AREA_VALUE = textAreaValue;
                            break;
                        case ECMGlobal.MetadataType.DropDown:
                            var ddlValue = reportLineItem.DDL_VALUE;
                            documentMetadataEntity.DDL_VALUE = ddlValue;
                            break;
                    }
                    if (isNew)
                    {
                        oldCustomeFields.Add(documentMetadataEntity);
                    }
                }
            }
            return oldCustomeFields.ToList();
        }

        private List<WF_DSR_REPORT_CATEGORY> SetDsReportSalesCategory(IEnumerable<DsrReportSalesCategory> dsReportSalesCategory,
         ICollection<WF_DSR_REPORT_CATEGORY> oldDsReportSalesCategory, int userId)
        {
            if (oldDsReportSalesCategory != null)
            {
                foreach (var customField in oldDsReportSalesCategory)
                {
                    customField.DATE_MODIFIED = DateTime.UtcNow;
                    customField.USER_MODIFIED = userId;
                    customField.DELETED_FLAG = true;
                }
            }
            if (dsReportSalesCategory != null)
            {
                var labelList = new List<string>();
                foreach (var salesCategory in dsReportSalesCategory)
                {
                    var isNew = false;
                    var id = salesCategory.SALES_CATEGORY_ID;
                    var value = salesCategory.VALUE;

                    WF_DSR_REPORT_CATEGORY salesCategoryEntity;
                    if (oldDsReportSalesCategory != null && oldDsReportSalesCategory.Count() > 0)
                    {
                        salesCategoryEntity = oldDsReportSalesCategory.Where(x => x.SALES_CATEGORY_ID == id).FirstOrDefault();
                        if (salesCategoryEntity == null)
                        {
                            salesCategoryEntity = new WF_DSR_REPORT_CATEGORY();
                            salesCategoryEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                            salesCategoryEntity.USER_ID = userId;
                            isNew = true;
                        }
                    }
                    else
                    {
                        salesCategoryEntity = new WF_DSR_REPORT_CATEGORY();
                        salesCategoryEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        salesCategoryEntity.USER_ID = userId;
                        isNew = true;
                    }
                    salesCategoryEntity.SALES_CATEGORY_ID = id;
                    salesCategoryEntity.VALUE = value;
                    salesCategoryEntity.DATE_MODIFIED = DateTime.UtcNow;
                    salesCategoryEntity.USER_MODIFIED = userId;
                    salesCategoryEntity.DELETED_FLAG = false;
                    if (isNew)
                    {
                        oldDsReportSalesCategory.Add(salesCategoryEntity);
                    }
                }
            }
            return oldDsReportSalesCategory.ToList();
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


        public WF_DSR_REPORT GetDailySalesReport(int dsrId)
        {
            return _repDailySalesReport.GetDailySalesReport(dsrId);
        }
    }
}
