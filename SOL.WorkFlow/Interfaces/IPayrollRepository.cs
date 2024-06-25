using SOL.Common.Business.Interfaces;
using SOL.Common.Business.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IPayrollRepository<T> : IBaseRepository<T>
    {
        void SaveBookkeepingConfiguration(WF_BK_BOOKKEEPING_CONFIGURATION bookkeepingConfiguration);
        IdValueModel GetBookKeepingIndustryName(int companyId);
        void SaveChanges();
        WF_BK_BOOKKEEPING_CONFIGURATION GetBookkeepingConfiguration(int companyId);
        List<WF_BK_CREDIT_CARD_DETAILS> GetCreditCardDetails(int companyId);
        List<WF_BK_BANK_DETAILS> GetBankDetails(int companyId);
        void SaveBankDetails(WF_BK_BANK_DETAILS bankDetails);
        void SaveCreaditCardDetails(WF_BK_CREDIT_CARD_DETAILS creditCardDetails);
        WF_BK_CREDIT_CARD_DETAILS GetCreaditCardDetailsById(int creaditCardDetailsId);
        WF_BK_BANK_DETAILS GetBankDetailsById(int bankDetailsId);
        bool IsExistingIntegerValue(int p, int id, int bkcId);
        bool IsExistingDecimalValue(decimal p, int id, int bkcId);
        bool IsExistingDateTimeValue(DateTime dateTime, int id, int bkcId);
        bool IsExistingTextValue(string textValue, int id, int bkcId);
        bool IsExistingTextAreaValue(string textAreaValue, int id, int bkcId);
        bool IsExistingDDLValue(int ddlValue, int id, int bkcId);
        bool IsExistingMultipleDDLValue(int ddlValue, int id, int bkcId);
        List<WF_BK_SALES_CATEGORY> GetSalesCategoryDetails(int companyId);
        List<WF_BK_KPIS> GetKpisDetails(int companyId);
        void SaveSalesAndRevinue(List<WF_BK_KPIS> kpisData, List<WF_BK_SALES_CATEGORY> salesCategories);
        void SaveNewSalesCategory(WF_BK_MS_SALES_CATEGORY newCategoryName, string salesCategoryName);
        bool IsNewSalesCategoryExist(string salesCategoryName);
        void SaveClientsAndVendor(WF_BK_CUSTOMER newCientsAndVendor);
        WF_BK_CUSTOMER GetExistingCustomerInfoByCustomerId(int customerId);

        WF_BK_EMPLOYEE GetBookkeepingEmployeeDetails(int employeeId);

        void SaveBookkeepingEmployeeDetails(WF_BK_EMPLOYEE employeeDetails);

        void SaveBookkeepingEmployeeAddress(WF_BK_EMPLOYEE_ADDRESS employeeAddress);

        void SaveBookkeepingEmployeeSchedule(WF_BK_EMPLOYEE_SCHEDULE employeeSchedule);

        void SaveBookkeepingEmployeeDesignation(WF_BK_EMPLOYEE_DESIGNATION employeeDesignation);

        WF_BK_EMPLOYEE_DESIGNATION GetEmployeeDesignationDetailsById(int employeeDesignationId);

        bool IsMultiplierExist(int employeeId, DateTime startDate, DateTime endDate, byte payTypeId,
            int employeeDesignationId);

        string GetBankAccountTypeNameByAccTypeId(int accounTypeId);

        string GetCreditCardTypeNameByTypeId(int CardTypeId);
        string GetTypeNameByTypeId(int typeId);

        bool IsClientOrVendorNameAndTypeExist(string name, int typeId,int companyId);
       
        bool CheckAccountNumberIsExist(int bankDetailId, string accountNumber, int accountType);

        bool CheckCreditCardNumberIsExist(int creditCardId, string cardNumber, int cardType);
        string GetVendorNameByVendorId(int vendorId);
        WF_BK_CHART_OF_ACCOUNT GetChartOfAccountDetails(int chartOfAccountId);
        
        void SaveChartOfAccount(List<WF_BK_CHART_OF_ACCOUNT> chartofAccounts);

        string GetAccountName(int accountId);

        PayrollPeriod GetPayrollPeriod(int companyId);

        DateTime GetPayrollYearStartDate(int year, int companyId);
        bool ValidateChartofAccountNames(int accountId, string accountName);
        void SaveChartofAccountNames(WF_MS_ACCOUNT objMsAccount);
        bool ValidateIndustryWiseDesignations(int designationId, string designationName);
        void SaveIndustryWiseDesignations(WF_BK_MS_DESIGNATION objMSDesignation);
    }
}
