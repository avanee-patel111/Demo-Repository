using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
  public  interface IWorkFlowPayrollRepository<T>
    {
        void SavePayroll(WF_PAYROLL payroll);
        void SavePayrollLineItem(List<WF_PAYROLL_LINE_ITEMS> payrollLineItems);
        WF_PAYROLL GetPayroll(int payrollId);
        WF_PAYROLL_LINE_ITEMS GetPayrollLineItem(int payrollLineItemId);     
    }
}
