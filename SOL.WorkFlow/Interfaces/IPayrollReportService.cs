using SOL.Common.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IPayrollReportService<T>
    {
        List<PayrollPeriod> GetPayrollPeriods(PayrollPeriod payrollPeriod, bool isFuture);

        List<PayrollPeriod> SetPayrollPeriods(DateTime selectedDate, DateTime dueDate, int ratePeriodId, int yearStartDay,
          int yearStartMonth, int yearEndDay, int yearEndMonth);

        object SavePayrollReport(PayrollReportModel payrollReportModel, int userId, UserType userType, string addedBy,
                ref string errorMessage, string companyLogo, string companyUrl);

        WF_PAYROLL_REPORT GetPayrollReport(int payrollReportId);

        void ApprovePayrollReportStatus(UpdateStatusViewModel objUpdateStatus, int userId, UserType userType, string companyLogo, string companyUrl);
    }
}
