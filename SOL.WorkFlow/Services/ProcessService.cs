using SOL.Common.Business.Interfaces;
using SOL.Common.Business.Models;
using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Services
{
    public class ProcessService<T> : IProcessService<int>
    {
        ICustomTypeService<int> _srvCustomType = null;
        IProcessRepository<int> _repProcess = null;

        public ProcessService(ICustomTypeService<int> srvCustomType, IProcessRepository<int> repProcess)
        {
            _srvCustomType = srvCustomType;
            _repProcess = repProcess;
        }

        public void SaveProcess(WF_PROCESS process, CommonCustomField CustomFieldsValues,
            int userId,int userType,string addedBy, ref string errorMessage)
        {
            if (process.PROCESS_ID == default(int))
            {
                process.DATE_MODIFIED = DateTime.UtcNow;
                process.DATE_OF_ENTRY = DateTime.UtcNow;
                process.USER_ID = userId;
                process.USER_MODIFIED = userId;
                process.DELETED_FLAG = false;
            }
            else {
                var orignalProcess = _repProcess.GetProcess(process.PROCESS_ID);
                orignalProcess.TITLE = process.TITLE;
                orignalProcess.DESCIPTION = process.DESCIPTION;
                orignalProcess.DATE_MODIFIED = DateTime.UtcNow;
                orignalProcess.USER_MODIFIED = userId;
              
            }
            _repProcess.SaveProcess(process);
            CustomFieldsValues.EntityId = process.PROCESS_ID;
            _srvCustomType.SaveCustomTypeFieldValuesData(CustomFieldsValues, userId, ref errorMessage);
        }
    }
}
