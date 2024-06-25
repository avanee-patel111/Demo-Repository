using SOL.Common.Business.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IProcessService<T>
    {
        void SaveProcess(WF_PROCESS process, CommonCustomField CustomFieldsValues,
            int userId, int userType, string addedBy, ref string errorMessage);
    }
}
