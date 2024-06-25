using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public  class salesAndRevenueModel
    {
        public int[] KpisIds { get; set; }
        public int[] SalesCategoryIds { get; set; }
        public int[] AccountIds { get; set; }
        public string IndustryNames { get; set; }
        public int IndustryId { get; set; }
        public int CompanyId { get; set; }
    }
}
