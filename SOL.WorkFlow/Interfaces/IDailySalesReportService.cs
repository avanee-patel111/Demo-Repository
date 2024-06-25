using SOL.Common.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
   public interface IDailySalesReportService<T>
   {      
       object SaveDailySalesReport(DsrReportModel dsrReportModel, int userId, UserType userType, string addedBy,
           ref string errorMessage, string companyLogo, string companyUrl);

       void ApproveDailySalesReportStatus(UpdateStatusViewModel updateStatusViewModel, int userId, UserType userType, string companyLogo, string companyUrl);
       WF_DSR_REPORT GetDailySalesReport(int dsrId);
   }
}
