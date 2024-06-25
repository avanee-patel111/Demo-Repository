using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
  public  class BookkeepingConfigurationEmployeeScheduleModel
  {
      public int SCHEDULE_ID { get; set; }
      public int EMPLOYEE_ID { get; set; }
      public int WEEK_DAY_ID { get; set; }
      public Nullable<System.TimeSpan> IN_TIME { get; set; }
      public Nullable<System.TimeSpan> OUT_TIME { get; set; }
      public bool IS_OFF_DAY { get; set; }
    }
}
