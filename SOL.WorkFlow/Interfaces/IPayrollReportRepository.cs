using SOL.Common.Business.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IPayrollReportRepository<T> : IBaseRepository<T>
    {       
        void SaveChanges();
        void SavePayrollReport(WF_PAYROLL_REPORT payrollReport);
        WF_PAYROLL_REPORT GetPayrollReport(int payrollId);
        bool isExistPayrollReportTitle(string payrollReportTitle, int payrollId);

        bool isExistPayrollReport(DateTime startDate, DateTime endDate, int payrollId, int clientId);

    }
}
