using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
   public interface IChartsAccountRepository<T>
    {
        /// <summary>
        /// Deletes the billing accounts.
        /// </summary>
        /// <param name="bill_Id">The bill identifier.</param>
       void DeleteBillingAccounts(int bill_Id);
       /// <summary>
       /// Saves the account detail.
       /// </summary>
       /// <param name="objAccountDetail">The object account detail.</param>
       /// <returns></returns>
       int SaveAccountDetail(WF_BILLING_ACCOUNT objAccountDetail);
       /// <summary>
       /// Gets the bill account by identifier.
       /// </summary>
       /// <param name="billAccountId">The bill account identifier.</param>
       /// <returns></returns>
       WF_BILLING_ACCOUNT GetBillAccountById(int billAccountId);
    }
}
