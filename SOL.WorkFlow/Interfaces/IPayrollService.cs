using SOL.Common.Models;
using SOL.Common.Business.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IPayrollService<T>
    {
        void SaveBookkeepingConfiguration(BookkeepingConfigurationModel bookkeepingConfigurationModel, int userId, ref string errorMessage);

        BookkeepingConfigurationModel GetBookkeepingConfiguration(int companyId);

        void SaveBankDetails(BookkeepingConfigurationBankDetailsModel bankDetails, int userId, ref string errorMessage);
        void SaveCreaditCardDetails(BookkeepingConfigurationCreditCardDetailsModel creditCardDetails, int userId, ref string errorMessage);
        List<BookkeepingConfigurationCreditCardDetailsModel> GetCreaditCardDetails(int companyId);
        List<BookkeepingConfigurationBankDetailsModel> GetBankDetails(int companyId);
        BookkeepingConfigurationCreditCardDetailsModel GetCreaditCardDetailsById(int creaditCardDetailsId);
        BookkeepingConfigurationBankDetailsModel GetBankDetailsById(int bankDetailsId);

        bool DeleteCommon(T entityTypeid, T id, T UserId, out string strMessage);

        void SaveBookkeepingEmployeeDetails(BookkeepingConfigurationEmployeeModel bookkeepingConfigurationEmployeeModel,
            int userId, ref List<int> employeeAddressIds);
        int SaveEmployeeDesignation(BookkeepingConfigurationEmployeeDesignationModel employeeDesignationModel,
           int userId, ref string errorMessage);
        void SaveSalesAndRevinue(salesAndRevenueModel kpisAndSalesDetails, int userId);
        void SaveNewSalesCategory(string SalescategoryName, int userId, ref string errorMessage, int industryId);

        void SaveClientsAndVendor(CientsAndVendorModel CientsAndVendor, int userId, ref string errorMessage);

        IdValueModel GetBookKeepingIndustryName(int CompanyId);


        void SaveChartOfAccount(ChartOfAccount chartOfAccountModel, int userId);

        PayrollPeriod GetPayrollPeriod(int companyId);

        DateTime GetPayrollYearStartDate(int year, int companyId);
        void SaveChartofAccountNames(WF_MS_ACCOUNT objMsAccount, int userId, ref string errorMessage);
        void SaveIndustryWiseDesignations(WF_BK_MS_DESIGNATION objMSDesignation, int userId, ref string errorMessage);

    }
}
