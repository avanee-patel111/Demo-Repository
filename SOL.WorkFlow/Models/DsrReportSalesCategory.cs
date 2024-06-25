using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class DsrReportSalesCategory
    {
        public int DSR_CATEGORY_ID { get; set; }
        public int DSR_ID { get; set; }
        public int SALES_CATEGORY_ID { get; set; }
        public decimal VALUE { get; set; }
    }
}
