using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
   public interface IChartsAccountService<T>
    {
        /// <summary>
        /// Manages the bill bank account.
        /// </summary>
        /// <param name="BillingAccounts">The billing accounts.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="workflowId">The workflow identifier.</param>
       void SaveBillBankAccount(WF_BILLING_ACCOUNT[] BillingAccounts, int entityId, int workflowId,int userId);
    }
}
