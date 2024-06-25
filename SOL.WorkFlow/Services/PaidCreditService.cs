using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Services
{
    public class PaidCreditService<T> : IPaidCreditService<T>
    {
        private IPaidCreditRepository<int> _repPaidCredit;

        public PaidCreditService(IPaidCreditRepository<int> repPaidCredit)
        {
            this._repPaidCredit = repPaidCredit;
        }

        public void SavePaidCredit(List<PaidCreditViewModel> objPaidCreditViewModel, int userId)
        {
            foreach(var item in objPaidCreditViewModel)
            {
                var objPaidCredit = new WF_PAID_CREDIT()
                {
                    VENDOR_CREDIT_REFERENCE = item.VENDOR_CREDIT_REFERENCE,
                    VENDOR_CREDIT_ID = item.VENDOR_CREDIT_ID,
                    AMOUNT = item.AMOUNT,
                    BILL_ID = item.BILL_ID,
                    DATE_OF_ENTRY = DateTime.UtcNow,
                    USER_ID = userId,
                    USER_MODIFIED = userId,
                    DATE_MODIFIED = DateTime.UtcNow,
                    DELETED_FLAG = false
                };
                _repPaidCredit.SavePaidCredit(objPaidCredit);
            }
            
        }
    }
}
