using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Services
{
    public class ApplyVendorCreditService<T> : IApplyVendorCreditService<int>
    {
        IVendorCreditService<T> _srvVendorCredit;
        IBillService<T> _srvBillService;
        IPaidCreditService<T> _srvPaidCredit;
        public ApplyVendorCreditService(IVendorCreditService<T> srvVendorCredit, IBillService<T> srvBillService, IPaidCreditService<T> srvPaidCredit)
        {
            this._srvVendorCredit = srvVendorCredit;
            this._srvBillService = srvBillService;
            this._srvPaidCredit = srvPaidCredit;
        }

        public WF_BILLING_DETAIL ApplyVendorCredit(ApplyCreditViewModel[] objVendorCreditViewModel, int userId, ref string errorMessage)
        {
            var objBillDetail = _srvBillService.GetBillDetailById(objVendorCreditViewModel[0].BILL_ID);

           

            decimal totalCredit = 0.0M;
            var vendorCreditds = new List<WF_VENDOR_CREDIT_DETAIL>();
            var paidCredits = new List<PaidCreditViewModel>();
            foreach(var item in objVendorCreditViewModel)
            {
                var objVendorCredit = _srvVendorCredit.GetVendorCreditById(item.VENDOR_CREDIT_ID);
                objVendorCredit.AVAILABLE_AMOUNT = objVendorCredit.AVAILABLE_AMOUNT - item.APPLY_AMOUNT;
                objVendorCredit.DATE_MODIFIED = DateTime.UtcNow;
                objVendorCredit.USER_MODIFIED = userId;

                vendorCreditds.Add(objVendorCredit);
                totalCredit = totalCredit + item.APPLY_AMOUNT;

               

                if (item.APPLY_AMOUNT != 0)
                {
                    var objPaidCredit = new PaidCreditViewModel();
                    objPaidCredit.VENDOR_CREDIT_REFERENCE = item.REFERENCE;
                    objPaidCredit.VENDOR_CREDIT_ID = item.VENDOR_CREDIT_ID;
                    objPaidCredit.BILL_ID = item.BILL_ID;
                    objPaidCredit.AMOUNT = item.APPLY_AMOUNT;
                    paidCredits.Add(objPaidCredit);
                }
                
               
            }

            if (objBillDetail.BALANCE < totalCredit)
            {
                errorMessage = "Total applied amount is greater than bill balance.";
                return new WF_BILLING_DETAIL();
            }
            
            int vendorCreditId = _srvVendorCredit.UpdateVendorCredit(vendorCreditds);
            objBillDetail.CREDIT_APPLIED = (objBillDetail.CREDIT_APPLIED != null ? objBillDetail.CREDIT_APPLIED : 0) + totalCredit;
            objBillDetail.BALANCE = objBillDetail.AMOUNT - objBillDetail.CREDIT_APPLIED.Value;

            if (objBillDetail.BALANCE == 0)
            {
                objBillDetail.PAYMENT_STATUS = (byte)PaymentStatus.Paid;
            }
            else if (objBillDetail.AMOUNT > objBillDetail.BALANCE)
            {
                objBillDetail.PAYMENT_STATUS = (byte)PaymentStatus.Partially_Paid;
            }
            _srvBillService.UpdateBillCredit(objBillDetail);

            _srvPaidCredit.SavePaidCredit(paidCredits, userId);
            

            return objBillDetail;
        }
    }
}
