using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class PayrollPeriod
    {
        //********************   Bookkeeping Configuration Paramiters  ********************
        public Nullable<int> PAYROLL_CYCLE_ID { get; set; }
        public System.DateTime PAYROLL_DUE_DATE { get; set; }
        public System.DateTime PAYROLL_END_DATE { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }

        //********************   Payroll Period  ********************
        public int ID { get; set; }
        public bool IS_SELECTED { get; set; }
        public string PAYROLL_NAME { get; set; }
        public System.DateTime PAYROLL_PERIOD_END_DATE { get; set; }
        public System.DateTime PAYROLL_PERIOD_START_DATE { get; set; }

    }
}
