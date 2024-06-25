using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public enum WorkflowTimelineObject
    {
        Payroll = 1,
        Approver = 2,
        Document = 3,
        DSR = 4,
        VendorBill = 5,
        VendorCredit = 6,
        Approver_Actions = 7,
        PayrollReport = 8,
        CustomReport = 9,
    }
}
