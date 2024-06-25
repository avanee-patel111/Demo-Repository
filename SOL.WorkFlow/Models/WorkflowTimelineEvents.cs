using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public enum WorkflowTimelineEvents
    {
        Payroll_Created = 1,
        Payroll_Updated = 2,
        Approver_Added=3,
        Approver_Deleted=4,
        Entity_Approed=5,
        Entity_Denied=6,
        Document_Associate = 7,
        Document_Unassociate = 8,
        DSR_Created = 9,
        DSR_Updated = 10,
        Vendor_Bill_Created = 11,
        Vendor_Bill_Updated = 12,
        Vendor_Credit_Created = 13,
        Vendor_Credit_Updated = 14,
        Payroll_Report_Created = 15,
        Payroll_Report_Updated = 16,
        Custom_Workflow_Created = 17,
        Custom_Workflow_Updated = 18,
        Entity_Assigned = 19,
        Entity_OnHOld = 20,

    }
}
