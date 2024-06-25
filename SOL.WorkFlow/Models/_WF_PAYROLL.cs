using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public partial class WF_PAYROLL
    {
        public List<WF_PAYROLL_LINE_ITEMS> PayrollLineItems { get; set; }
        public WORKFLOW_REVIEWER[] Approvers { get; set; }
        public byte Status { get; set; }
        public string NOTE_TO_PAYER { get; set; }
        public int[] AssocitaedPages { get; set; }
        public bool IsApproveMode { get; set; }
        public int DOCUMENT_ID { get; set; }
        public List<DocumentWorkFlow> documentId { get; set; }

    }
}
