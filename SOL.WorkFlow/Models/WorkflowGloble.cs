using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class WorkflowGloble
    {
        public enum MoveType
        {
            SpecificFolder = 1,
            FixedFolder = 2,
            ApproverSelectionFolder = 3,
        }

        public enum Step
        {
            ToBeProcessed = 1,
            ToBeReviewed = 2,
            ProcessedFinancialDocuments = 3,
        }

        public enum EscalationType
        {
            SendBack = 1,
            ForwardTo = 2,
        }

        public enum PayType
        {
            Fixed = 1,
            Hourly = 2,
        }

        public enum WorkFlowType
        {
            VenderBill = 1,
            VenderCredit = 2,
            Payroll = 3,
            DSReport = 4,
            PayrollReport = 5
        }

        public enum PayrollPeriodType
        {
            Weekly = 1,
            Bi_Weekly = 2,
            Monthly = 3,
        }

        public enum WorkFlowLevel
        {
            NotAllowPageSelection = 1,
            AllowPageSelection = 2,
        }

        public static string Weekly = "Weekly";
        public static string Bi_Weekly = "Bi Weekly";
        public static string Monthly = "Monthly";

        public enum WorkFlowEmailEventType
        {
            Create = 1,
            ApprovalWaitting = 2,
            Approved = 3,
            Denied = 4,
            Reasign = 5,
            completed = 6,
        }
    }
}
