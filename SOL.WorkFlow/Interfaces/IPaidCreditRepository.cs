using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
   public interface IPaidCreditRepository<T>
    {
       void SavePaidCredit(WF_PAID_CREDIT objPaidCredit);
    }
}
