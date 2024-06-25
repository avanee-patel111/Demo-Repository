using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class BookkeepingConfigurationEmployeeDesignationModel
    {
        public int EMPLOYEE_DESIGNATION_ID { get; set; }
        public int EMPLOYEE_ID { get; set; }
        public int DESIGNATION_ID { get; set; }
        public byte PAY_TYPE_ID { get; set; }
        public System.DateTime START_DATE { get; set; }
        public System.DateTime END_DATE { get; set; }
        public decimal RATE { get; set; }
        public Nullable<int> CYCLE_ID { get; set; }
    }
}
