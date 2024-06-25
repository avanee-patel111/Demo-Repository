using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class BookkeepingConfigurationOperationHoursModel
    {
        public int OPERATION_HOURS_ID { get; set; }
        public int BKC_ID { get; set; }
        public byte WEEKDAY { get; set; }
        public Nullable<decimal> OPERATION_HOUR_LABLE_ONE { get; set; }
        public Nullable<decimal> OPERATION_HOUR_LABLE_TWO { get; set; }
    }
}
