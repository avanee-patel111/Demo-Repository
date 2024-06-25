using SOL.Common.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
   public interface IVendorCreditService<T>
    {
       object SaveVendorCredit(VendorCreditViewModel objVendorCreditModel, string addedBy, string companyLogo, string companyUrl, ref string errorMessage);
       int ApproveVenorCreditStatus(UpdateStatusViewModel objVenorCreditViewModel, int userId, UserType userType, string companyLogo, string companyUrl);
       int UpdateVendorCredit(List<WF_VENDOR_CREDIT_DETAIL> objVendorCredit);
       WF_VENDOR_CREDIT_DETAIL GetVendorCreditById(int vendorCredit);
       
    }
}
