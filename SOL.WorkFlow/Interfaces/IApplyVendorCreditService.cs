using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
   public interface IApplyVendorCreditService<T>
    {
       WF_BILLING_DETAIL ApplyVendorCredit(ApplyCreditViewModel[] objVendorCreditViewModel, int userId, ref string errorMessage);
    }
}
