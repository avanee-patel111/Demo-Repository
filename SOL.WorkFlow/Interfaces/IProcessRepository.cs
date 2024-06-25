using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
   public interface IProcessRepository<T>
    {

        /// <summary>
        /// Saves the process.
        /// </summary>
        /// <param name="process">The process.</param>
       void SaveProcess(WF_PROCESS process);
       /// <summary>
       /// Gets the process.
       /// </summary>
       /// <param name="processId">The process identifier.</param>
       WF_PROCESS GetProcess(int processId);
    }
}
