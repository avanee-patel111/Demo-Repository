using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Services
{
    public class ChartsAccountService<T> : IChartsAccountService<T>
    {
        IChartsAccountRepository<T> _repAccount;
        public ChartsAccountService(IChartsAccountRepository<T> repAccount)
        {
            this._repAccount = repAccount;
        }
        /// <summary>
        /// Manages the bill bank account.
        /// </summary>
        /// <param name="BillingAccounts">The billing accounts.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="workflowId">The workflow identifier.</param>
        public void SaveBillBankAccount(WF_BILLING_ACCOUNT[] BillingAccounts, int entityId, int workflowId, int userId)
        {
            if (BillingAccounts != null)
            {
                if (entityId != 0)
                {
                    _repAccount.DeleteBillingAccounts(entityId);
                }

                foreach (var item in BillingAccounts)
                {
                    var objBillingAccount = new WF_BILLING_ACCOUNT();
                    objBillingAccount.ACCOUNT_ENTITY_ID = entityId;
                    objBillingAccount.WORKFLOW_ID = workflowId;
                    objBillingAccount.ACCOUNT_ID = item.ACCOUNT_ID;
                    objBillingAccount.DESCRIPTION = item.DESCRIPTION;
                    objBillingAccount.AMOUNT = item.AMOUNT;
                    objBillingAccount.USER_ID = userId;
                    objBillingAccount.DATE_OF_ENTRY = DateTime.UtcNow;
                    objBillingAccount.USER_MODIFIED = userId;
                    objBillingAccount.DATE_MODIFIED = DateTime.UtcNow;
                    objBillingAccount.DELETED_FLAG = false;
                    _repAccount.SaveAccountDetail(objBillingAccount);
                }
            }
        }
    }
}
