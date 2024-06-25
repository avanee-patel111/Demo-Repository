using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
   public interface IVendorCreditRepository<T>
    {
       int SaveVendorCredit(WF_VENDOR_CREDIT_DETAIL objVendorCreditDetail);
       WF_VENDOR_CREDIT_DETAIL GetVendorCreditById(int vendorCreditId);
       int GetVenderIdByVendorCreditId(int vendorCreditId);
       int UpdateApproverStatsu(WF_VENDOR_CREDIT_DETAIL objVendorCreditDetail);
       int UpdateVendorCredit(WF_VENDOR_CREDIT_DETAIL objVendorCreditDetail);
    }
}
