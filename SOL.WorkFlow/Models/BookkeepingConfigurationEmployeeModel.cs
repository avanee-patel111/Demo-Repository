using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class BookkeepingConfigurationEmployeeModel
    {
        public int EMPLOYEE_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MOBILE_NUMBE { get; set; }
        public Nullable<int> PRIMARY_DESIGNATION { get; set; }
        public bool ACTIVE_FLAG { get; set; }
        public List<BookkeepingConfigurationEmployeeAddressModel> EmployeeAddressModel { get; set; }
        public List<BookkeepingConfigurationEmployeeScheduleModel> EmployeeScheduleModel { get; set; }
        public List<BookkeepingConfigurationEmployeeDesignationModel> EmployeeDesignationModel { get; set; }
    }
}
