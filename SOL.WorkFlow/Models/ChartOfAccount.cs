using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class ChartOfAccount
    {
        public int CHART_ACCOUNT_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public List<int> ACCOUNT_IDS { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public bool IsExisting { get; set; }
    }
}
