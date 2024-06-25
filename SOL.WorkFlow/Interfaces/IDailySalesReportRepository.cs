using SOL.Common.Business.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IDailySalesReportRepository<T> : IBaseRepository<T>
    {
        void SaveDailySalesReport(WF_DSR_REPORT dsrReport);
        void SaveChanges();

        WF_DSR_REPORT GetDailySalesReport(int dsrId);

        bool isExistSalesReportTitle(string dsrReportTitle, int dsrId);
    }
}
