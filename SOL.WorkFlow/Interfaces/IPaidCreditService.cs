using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IPaidCreditService<T>
    {
        void SavePaidCredit(List<PaidCreditViewModel> objPaidCreditViewModel, int userId);
    }
}
