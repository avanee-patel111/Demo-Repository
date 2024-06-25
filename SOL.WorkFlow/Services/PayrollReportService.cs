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
    public class PayrollReportService<T> : BaseService<T>, IPayrollReportService<T>
    {
        IPayrollReportRepository<int> _repPayrollReport = null;
        IPmsService<T> _srvPms;
        IWorkflowDocumentService<T> _srvWfDocumentService = null;
        IApprovalService<T> _srvApproval;
        IWorkFlowRepository<T> _repWorkFlow;
        IWorkflowTimelineService<T> _srvTimeLine = null;

        public PayrollReportService(IPayrollReportRepository<int> repPayrollReport,
            IAddressbookRepository<T> repAdrbook, IPmsService<T> srvPms, IWorkFlowRepository<T> repWorkFlow,
             IApprovalService<T> srvApproval, IWorkflowDocumentService<T> srvWfDocumentService,
            IWorkflowTimelineService<T> srvTimeLine)
            : base(repAdrbook)
        {
            this._repPayrollReport = repPayrollReport;
            this._srvPms = srvPms;
            this._srvWfDocumentService = srvWfDocumentService;
            this._srvApproval = srvApproval;
            this._repWorkFlow = repWorkFlow;
            this._srvTimeLine = srvTimeLine;
        }


        public List<PayrollPeriod> GetPayrollPeriods(PayrollPeriod payrollPeriod, bool isFuture)
        {
            var endDay = GetCompanySetup<int>("YEAR_END_DAY");
            var endMonth = GetCompanySetup<int>("YEAR_END_MONTH");
            var startDay = GetCompanySetup<int>("YEAR_START_DAY");
            var startMonth = GetCompanySetup<int>("YEAR_START_MONTH");
            var payrollEndDate = payrollPeriod.PAYROLL_END_DATE;
            if (isFuture)
            {
                return SetFuturePayrollPeriod(payrollPeriod.START_DATE.Value, payrollPeriod.PAYROLL_DUE_DATE,
               payrollPeriod.PAYROLL_CYCLE_ID.Value, startDay, startMonth, endDay, endMonth);
            }
            return SetPayrollPeriod(payrollEndDate, payrollPeriod.PAYROLL_DUE_DATE,
                payrollPeriod.PAYROLL_CYCLE_ID.Value, startDay, startMonth, endDay, endMonth);
        }

        private List<PayrollPeriod> SetPayrollPeriod(DateTime selectedDate, DateTime dueDate, int ratePeriodId, int yearStartDay,
            int yearStartMonth, int yearEndDay, int yearEndMonth)
        {
            DateTime? todayDate = null;
            var payrollPeriods = new List<PayrollPeriod>();
            var payrollEndDate = selectedDate;
            var payrollStartDate = selectedDate;
            var currentYear = selectedDate.Year;
            DateTime firstYearDate = new DateTime(currentYear, yearStartMonth, yearStartDay);
            DateTime lastYearDate = new DateTime(currentYear, yearEndMonth, yearEndDay);
            int j = 1;
            if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Weekly)
            {
                if (!todayDate.HasValue)
                    todayDate = DateTime.UtcNow.AddDays(-7);
                todayDate = new DateTime(todayDate.Value.Year, todayDate.Value.Month, todayDate.Value.Day);
                for (int i = 0; payrollEndDate > firstYearDate; i++)
                {
                    var startDate = payrollEndDate.AddDays(-6);
                    if (firstYearDate > startDate)
                    {
                        payrollStartDate = payrollEndDate.AddDays(1);
                        payrollEndDate = payrollEndDate.AddDays(1);
                        break;
                    }
                    payrollEndDate = startDate;
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollStartDate,
                        PAYROLL_PERIOD_START_DATE = payrollEndDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    if (todayDate <= payrollEndDate && todayDate >= payrollStartDate)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollEndDate.ToString("MM/dd/yyyy") + " - " + payrollStartDate.ToString("MM/dd/yyyy");
                    payrollStartDate = payrollEndDate.AddDays(-1);
                    payrollEndDate = payrollEndDate.AddDays(-1);
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Bi_Weekly)
            {
                if (!todayDate.HasValue)
                    todayDate = DateTime.UtcNow.AddDays(-14);
                todayDate = new DateTime(todayDate.Value.Year, todayDate.Value.Month, todayDate.Value.Day);
                for (int i = 0; payrollEndDate > firstYearDate; i++)
                {
                    var startDate = payrollEndDate.AddDays(-13);
                    if (firstYearDate > startDate)
                    {
                        payrollStartDate = payrollEndDate.AddDays(1);
                        payrollEndDate = payrollEndDate.AddDays(1);
                        break;
                    }
                    payrollEndDate = startDate;
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollStartDate,
                        PAYROLL_PERIOD_START_DATE = payrollEndDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    if (todayDate <= payrollEndDate && todayDate >= payrollStartDate)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollEndDate.ToString("MM/dd/yyyy") + " - " + payrollStartDate.ToString("MM/dd/yyyy");
                    payrollStartDate = payrollEndDate;
                    payrollStartDate = payrollEndDate.AddDays(-1);
                    payrollEndDate = payrollEndDate.AddDays(-1);
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Monthly)
            {
                for (int i = 0; payrollEndDate > firstYearDate; i++)
                {
                    DateTime nextYear = new DateTime(payrollEndDate.AddMonths(-1).Year, payrollEndDate.AddMonths(-1).Month, 1);
                    var year = nextYear.Year;
                    var month = nextYear.Month;
                    int days = System.DateTime.DaysInMonth(currentYear, month);
                    payrollEndDate = payrollEndDate.AddDays(-days).AddDays(1);
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollStartDate,
                        PAYROLL_PERIOD_START_DATE = payrollEndDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    if (i == 0)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollEndDate.ToString("MM/dd/yyyy") + " - " + payrollStartDate.ToString("MM/dd/yyyy");
                    payrollEndDate = payrollEndDate.AddDays(-1);
                    payrollStartDate = payrollEndDate;
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            if (payrollPeriods.Count > 0)
            {
                payrollPeriods.Reverse();
                //payrollEndDate = selectedDate.AddDays(1);
                //payrollStartDate = selectedDate.AddDays(1);
            }
            payrollEndDate = selectedDate.AddDays(1);
            payrollStartDate = selectedDate.AddDays(1);
            if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Weekly)
            {
                if (!todayDate.HasValue)
                    todayDate = DateTime.UtcNow.AddDays(-7);
                todayDate = new DateTime(todayDate.Value.Year, todayDate.Value.Month, todayDate.Value.Day);
                for (int i = 0; payrollEndDate < lastYearDate; i++)
                {
                    var startDate = payrollStartDate.AddDays(6);
                    if (firstYearDate > startDate)
                    {
                        payrollStartDate = payrollEndDate.AddDays(1);
                        break;
                    }
                    payrollEndDate = startDate;
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollEndDate,
                        PAYROLL_PERIOD_START_DATE = payrollStartDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    if (todayDate <= payrollEndDate && todayDate >= payrollStartDate)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollStartDate.ToString("MM/dd/yyyy") + " - " + payrollEndDate.ToString("MM/dd/yyyy");
                    payrollStartDate = payrollEndDate.AddDays(1);
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Bi_Weekly)
            {
                if (!todayDate.HasValue)
                    todayDate = DateTime.UtcNow.AddDays(-14);
                todayDate = new DateTime(todayDate.Value.Year, todayDate.Value.Month, todayDate.Value.Day);
                for (int i = 0; payrollEndDate < lastYearDate; i++)
                {
                    var startDate = payrollStartDate.AddDays(13);
                    if (firstYearDate > startDate)
                    {
                        payrollStartDate = payrollEndDate.AddDays(1);
                        break;
                    }
                    payrollEndDate = startDate;
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollEndDate,
                        PAYROLL_PERIOD_START_DATE = payrollStartDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    if (todayDate <= payrollEndDate && todayDate >= payrollStartDate)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollStartDate.ToString("MM/dd/yyyy") + " - " + payrollEndDate.ToString("MM/dd/yyyy");
                    payrollStartDate = payrollEndDate.AddDays(1);
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Monthly)
            {
                for (int i = 0; payrollEndDate < lastYearDate; i++)
                {
                    var nextMonth = new DateTime(payrollEndDate.AddMonths(1).Year, payrollEndDate.AddMonths(1).Month, 1);
                    var year = payrollEndDate.Year;
                    var month = payrollEndDate.Month;
                    int days = System.DateTime.DaysInMonth(currentYear, month);
                    payrollEndDate = payrollEndDate.AddDays(days).AddDays(-1);
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollEndDate,
                        PAYROLL_PERIOD_START_DATE = payrollStartDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    DateTime previousMonth = DateTime.UtcNow;
                    previousMonth = previousMonth.AddMonths(-1);
                    if (previousMonth.Month == month)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollStartDate.ToString("MM/dd/yyyy") + " - " + payrollEndDate.ToString("MM/dd/yyyy");
                    payrollEndDate = payrollEndDate.AddDays(1);
                    payrollStartDate = payrollEndDate;
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            return payrollPeriods;
        }

        public object SavePayrollReport(PayrollReportModel payrollReportModel, int userId, UserType userType, string addedBy,
            ref string errorMessage, string companyLogo, string companyUrl)
        {
            var obj = new object();
            var payrollReport = new WF_PAYROLL_REPORT();
            bool genrateDocument = true;
            bool isCheckOriginalName = false;
            var documentOperation = new DocumentOperation();
            var payrollReportId = payrollReportModel.PAYROLL_REPORT_ID;
            var oldPayrollReportId = payrollReportId;
            var payrollReportTitle = payrollReportModel.TITLE;
            var approvalStatus = payrollReportModel.APPROVAL_STATUS;
            var payrollReportStartDate = payrollReportModel.PAYROLL_PERIOD_START_DATE;
            var payrollReportEndDate = payrollReportModel.PAYROLL_PERIOD_END_DATE;
            var clientId = payrollReportModel.CLIENT_ID;
            var isExistPayrollReportTitle = _repPayrollReport.isExistPayrollReportTitle(payrollReportTitle, payrollReportId);
            var isExistPayrollReport = _repPayrollReport.isExistPayrollReport(payrollReportStartDate, payrollReportEndDate, payrollReportId, clientId);
            if (isExistPayrollReport == true)
            {
                errorMessage = "This payroll periods overlaps in existing payroll period.";
            }
            if (isExistPayrollReportTitle == true)
            {
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = "This payroll periods overlaps in existing payroll period and ";
                }
                errorMessage = errorMessage + payrollReportTitle + " already exists in your records. To add this document to the existing Report, click on Add to Existing.";
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return obj;
            }
            if (payrollReportId == 0)
            {
                var endDay = GetCompanySetup<int>("YEAR_END_DAY");
                var endMonth = GetCompanySetup<int>("YEAR_END_MONTH");
                var startDay = GetCompanySetup<int>("YEAR_START_DAY");
                var startMonth = GetCompanySetup<int>("YEAR_START_MONTH");

                payrollReport.CLIENT_ID = clientId;
                payrollReport.DATE_OF_ENTRY = DateTime.UtcNow;
                payrollReport.USER_ID = userId;
                payrollReport.DELETED_FLAG = false;
                payrollReport.YEAR_START_DAY = startDay;
                payrollReport.YEAR_START_MONTH = startMonth;
                payrollReport.YEAR_END_DAY = endDay;
                payrollReport.YEAR_END_MONTH = endMonth;
            }
            else
            {
                payrollReport = _repPayrollReport.GetPayrollReport(payrollReportId);
                clientId = payrollReport.CLIENT_ID;
                genrateDocument = false;
            }
            int projectId = _srvPms.GetProjectIdByClientId(clientId);
            var workFlowId = payrollReportModel.WORKFLOW_ID;
            payrollReport.WORKFLOW_DEFINITION_ID = workFlowId;
            payrollReport.DATE_MODIFIED = DateTime.UtcNow;
            payrollReport.USER_MODIFIED = userId;
            payrollReport.APPROVAL_STATUS = approvalStatus;
            payrollReport.TITLE = payrollReportTitle;
            payrollReport.PAYROLL_REPORT_DATE = payrollReportModel.PAYROLL_REPORT_DATE;
            payrollReport.DESCRIPTION = payrollReportModel.DESCRIPTION;
            payrollReport.PAYROLL_PERIOD_START_DATE = payrollReportModel.PAYROLL_PERIOD_START_DATE;
            payrollReport.PAYROLL_PERIOD_END_DATE = payrollReportModel.PAYROLL_PERIOD_END_DATE;
            payrollReport.IS_RECORDED = payrollReportModel.IS_RECORDED;
            payrollReport.PAYROLL_CYCLE_ID = payrollReportModel.PAYROLL_CYCLE_ID;
            payrollReport.PAYROLL_END_DATE = payrollReportModel.PAYROLL_END_DATE;
            payrollReport.PAYROLL_DUE_DATE = payrollReportModel.PAYROLL_DUE_DATE;
            _repPayrollReport.SavePayrollReport(payrollReport);
            payrollReportId = payrollReport.PAYROLL_REPORT_ID;
            if (genrateDocument == true)
            {
                _srvTimeLine.SaveWorflowTimeline(payrollReportId, workFlowId, approvalStatus, null, null,
                (int)WorkflowTimelineObject.PayrollReport, (int)WorkflowTimelineEvents.Payroll_Report_Created, string.Empty, userId, DateTime.UtcNow, null, null);
            }
            else
            {
                _srvTimeLine.SaveWorflowTimeline(payrollReportId, workFlowId, approvalStatus, null, null,
               (int)WorkflowTimelineObject.PayrollReport, (int)WorkflowTimelineEvents.Payroll_Report_Updated, string.Empty, userId, DateTime.UtcNow, null, null);
            }
            var docId = payrollReportModel.DOC_ID;
            var originelDocId = docId;
            var newDocId = 0;
            var isClosePage = true;
            var IsApproveMode = payrollReportModel.IsApproveMode;
            var associtaedPages = payrollReportModel.AssocitaedPages;
            byte ByOverView = 0;
            if (oldPayrollReportId == 0)
            {
                _srvWfDocumentService.ManageBillDocuments(payrollReportId, workFlowId, addedBy, approvalStatus,
                       genrateDocument, isCheckOriginalName, associtaedPages,
                        userId, userType, ref errorMessage, projectId, docId, ref originelDocId,
                       ref newDocId, ref isClosePage, IsApproveMode, payrollReportModel.TITLE);
                _srvWfDocumentService.SaveDocumentEntity(workFlowId, payrollReportId, payrollReportModel.documentId, newDocId, userId);
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
                if (payrollReportModel.Approvers != null)
                {
                    _srvApproval.SaveApprovers(payrollReportModel.Approvers, payrollReportModel.NOTE_TO_PAYER, payrollReportId,
                        workFlowId, userId, approvalStatus, ByOverView,companyLogo);
                }
            }
            SendPreEmailNotification(payrollReportModel.Approvers, userId, workFlowId, payrollReportId, companyLogo, companyUrl);
            obj = new
            {
                originelDocId = originelDocId,
                isClosePage = isClosePage,
                payrollReportId = payrollReportId,
                newDocId = newDocId
            };
            return obj;
        }

        public WF_PAYROLL_REPORT GetPayrollReport(int payrollReportId)
        {
            return _repPayrollReport.GetPayrollReport(payrollReportId);
        }

        public void ApprovePayrollReportStatus(UpdateStatusViewModel updateStatusViewModel, int userId, UserType userType, string companyLogo, string companyUrl)
        {
            string errorMessage = string.Empty;
            var flowStatus = updateStatusViewModel.Status;
            var entityId = updateStatusViewModel.EntityId;
            var workFlowId = (int)CustomWorkflowTypes.PayrollReport;
            _srvApproval.   ManageApproversOnApprovals(updateStatusViewModel.Approvers, updateStatusViewModel.ApproverNote,null,
                flowStatus, entityId, workFlowId, userId, companyLogo);
            var dsReport = _repPayrollReport.GetPayrollReport(entityId);
            var isAnyPendingAprrovers = _srvApproval.IsAnyPendingAprrover(entityId, workFlowId);
            var docId = _repWorkFlow.GetNewDocIdByEntityId(entityId, updateStatusViewModel.WorkFlowId);
            if (flowStatus == (int)FlowStatus.Denied)
            {
                dsReport.APPROVAL_STATUS = flowStatus;
                _srvWfDocumentService.MoVeDocumentToDeniedFolder(userId, userType, ref errorMessage,
                       docId, docId, workFlowId);
            }
            else if (isAnyPendingAprrovers == false)
            {
                dsReport.APPROVAL_STATUS = flowStatus;
                _srvWfDocumentService.MoVeDocumentToCompletedFolder(userId, userType, ref errorMessage,
                            docId, docId, string.Empty, workFlowId);
            }
            dsReport.USER_MODIFIED = userId;
            dsReport.DATE_MODIFIED = DateTime.UtcNow;
            _repPayrollReport.SaveChanges();
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

        private List<PayrollPeriod> SetFuturePayrollPeriod(DateTime startDate, DateTime dueDate, int ratePeriodId, int yearStartDay,
            int yearStartMonth, int yearEndDay, int yearEndMonth)
        {
            var todayDate = DateTime.UtcNow;
            var payrollPeriods = new List<PayrollPeriod>();
            var payrollEndDate = startDate;
            var payrollStartDate = startDate;
            var currentYear = startDate.Year;//
            DateTime lastYearDate = new DateTime(currentYear, yearEndMonth, yearEndDay);
            int j = 1;
            if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Weekly)
            {
                todayDate = todayDate.AddDays(-7);
                todayDate = new DateTime(todayDate.Year, todayDate.Month, todayDate.Day);
                for (int i = 0; payrollEndDate < lastYearDate; i++)
                {
                    payrollEndDate = payrollStartDate.AddDays(6);
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollEndDate,
                        PAYROLL_PERIOD_START_DATE = payrollStartDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = startDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    if (todayDate <= payrollEndDate && todayDate >= payrollStartDate)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollStartDate.ToString("MM/dd/yyyy") + " - " + payrollEndDate.ToString("MM/dd/yyyy");
                    payrollStartDate = payrollEndDate.AddDays(1);
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Bi_Weekly)
            {
                todayDate = todayDate.AddDays(-14);
                todayDate = new DateTime(todayDate.Year, todayDate.Month, todayDate.Day);
                for (int i = 0; payrollEndDate < lastYearDate; i++)
                {
                    payrollEndDate = payrollStartDate.AddDays(13);
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollEndDate,
                        PAYROLL_PERIOD_START_DATE = payrollStartDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = startDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    if (todayDate <= payrollEndDate && todayDate >= payrollStartDate)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollStartDate.ToString("MM/dd/yyyy") + " - " + payrollEndDate.ToString("MM/dd/yyyy");
                    payrollStartDate = payrollEndDate.AddDays(1);
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Monthly)
            {
                for (int i = 0; payrollEndDate < lastYearDate; i++)
                {
                    var nextMonth = new DateTime(payrollEndDate.AddMonths(1).Year, payrollEndDate.AddMonths(1).Month, 1);
                    var year = payrollEndDate.Year;
                    var month = payrollEndDate.Month;
                    int days = System.DateTime.DaysInMonth(currentYear, month);
                    payrollEndDate = payrollEndDate.AddDays(days).AddDays(-1);
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollEndDate,
                        PAYROLL_PERIOD_START_DATE = payrollStartDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = startDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    if (DateTime.UtcNow.Month == month)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollStartDate.ToString("MM/dd/yyyy") + " - " + payrollEndDate.ToString("MM/dd/yyyy");
                    payrollEndDate = payrollEndDate.AddDays(1);
                    payrollStartDate = payrollEndDate;
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            return payrollPeriods;
        }

        public List<PayrollPeriod> SetPayrollPeriods(DateTime selectedDate, DateTime dueDate, int ratePeriodId, int yearStartDay,
          int yearStartMonth, int yearEndDay, int yearEndMonth)
        {
            var payrollPeriods = new List<PayrollPeriod>();
            var payrollEndDate = selectedDate;
            var payrollStartDate = selectedDate;
            var currentYear = selectedDate.Year;
            DateTime firstYearDate = new DateTime(currentYear, yearStartMonth, yearStartDay);
            DateTime lastYearDate = new DateTime(currentYear, yearEndMonth, yearEndDay);
            int j = 1;
            if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Weekly)
            {
                for (int i = 0; payrollEndDate > firstYearDate; i++)
                {
                    var startDate = payrollEndDate.AddDays(-6);
                    if (firstYearDate > startDate)
                    {
                        break;
                    }
                    payrollEndDate = startDate;
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollStartDate,
                        PAYROLL_PERIOD_START_DATE = payrollEndDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    payroll.PAYROLL_NAME = payrollEndDate.ToString("MM/dd/yyyy") + " - " + payrollStartDate.ToString("MM/dd/yyyy");
                    payrollStartDate = payrollEndDate.AddDays(-1);
                    payrollEndDate = payrollEndDate.AddDays(-1);
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Bi_Weekly)
            {
                for (int i = 0; payrollEndDate > firstYearDate; i++)
                {
                    var startDate = payrollEndDate.AddDays(-13);
                    if (firstYearDate > startDate)
                    {
                        payrollStartDate = payrollEndDate.AddDays(1);
                        break;
                    }
                    payrollEndDate = startDate;
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollStartDate,
                        PAYROLL_PERIOD_START_DATE = payrollEndDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    payroll.PAYROLL_NAME = payrollEndDate.ToString("MM/dd/yyyy") + " - " + payrollStartDate.ToString("MM/dd/yyyy");
                    payrollStartDate = payrollEndDate;
                    payrollStartDate = payrollEndDate.AddDays(-1);
                    payrollEndDate = payrollEndDate.AddDays(-1);
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Monthly)
            {
                for (int i = 0; payrollEndDate > firstYearDate; i++)
                {
                    DateTime nextYear = new DateTime(payrollEndDate.AddMonths(-1).Year, payrollEndDate.AddMonths(-1).Month, 1);
                    var year = nextYear.Year;
                    var month = nextYear.Month;
                    int days = System.DateTime.DaysInMonth(currentYear, month);
                    payrollEndDate = payrollEndDate.AddDays(-days).AddDays(1);
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollStartDate,
                        PAYROLL_PERIOD_START_DATE = payrollEndDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    if (i == 0)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollEndDate.ToString("MM/dd/yyyy") + " - " + payrollStartDate.ToString("MM/dd/yyyy");
                    payrollEndDate = payrollEndDate.AddDays(-1);
                    payrollStartDate = payrollEndDate;
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            if (payrollPeriods.Count > 0)
            {
                payrollPeriods.Reverse();
                payrollEndDate = selectedDate.AddDays(1);
                payrollStartDate = selectedDate.AddDays(1);
            }
            if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Weekly)
            {
                for (int i = 0; payrollEndDate < lastYearDate; i++)
                {
                    var startDate = payrollStartDate.AddDays(6);
                    if (firstYearDate > startDate)
                    {
                        payrollStartDate = payrollEndDate.AddDays(1);
                        break;
                    }
                    payrollEndDate = startDate;
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollEndDate,
                        PAYROLL_PERIOD_START_DATE = payrollStartDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    payroll.PAYROLL_NAME = payrollStartDate.ToString("MM/dd/yyyy") + " - " + payrollEndDate.ToString("MM/dd/yyyy");
                    payrollStartDate = payrollEndDate.AddDays(1);
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Bi_Weekly)
            {
                for (int i = 0; payrollEndDate < lastYearDate; i++)
                {
                    var startDate = payrollStartDate.AddDays(13);
                    if (firstYearDate > startDate)
                    {
                        payrollStartDate = payrollEndDate.AddDays(1);
                        break;
                    }
                    payrollEndDate = startDate;
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollEndDate,
                        PAYROLL_PERIOD_START_DATE = payrollStartDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    payroll.PAYROLL_NAME = payrollStartDate.ToString("MM/dd/yyyy") + " - " + payrollEndDate.ToString("MM/dd/yyyy");
                    payrollStartDate = payrollEndDate.AddDays(1);
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Monthly)
            {
                for (int i = 0; payrollEndDate < lastYearDate; i++)
                {
                    var nextMonth = new DateTime(payrollEndDate.AddMonths(1).Year, payrollEndDate.AddMonths(1).Month, 1);
                    var year = payrollEndDate.Year;
                    var month = payrollEndDate.Month;
                    int days = System.DateTime.DaysInMonth(currentYear, month);
                    payrollEndDate = payrollEndDate.AddDays(days).AddDays(-1);
                    var payroll = new PayrollPeriod()
                    {
                        PAYROLL_PERIOD_END_DATE = payrollEndDate,
                        PAYROLL_PERIOD_START_DATE = payrollStartDate,
                        PAYROLL_DUE_DATE = dueDate,
                        PAYROLL_END_DATE = selectedDate,
                        PAYROLL_CYCLE_ID = ratePeriodId,
                        ID = j,
                    };
                    DateTime previousMonth = DateTime.UtcNow;
                    previousMonth = previousMonth.AddMonths(-1);
                    if (previousMonth.Month == month)
                    {
                        payroll.IS_SELECTED = true;
                    }
                    payroll.PAYROLL_NAME = payrollStartDate.ToString("MM/dd/yyyy") + " - " + payrollEndDate.ToString("MM/dd/yyyy");
                    payrollEndDate = payrollEndDate.AddDays(1);
                    payrollStartDate = payrollEndDate;
                    payrollPeriods.Add(payroll);
                    j++;
                }
            }
            return payrollPeriods;
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
