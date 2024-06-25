using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class DsrReportLineItems
    {
        public int DSR_LINE_ID { get; set; }
        public int DSR_ID { get; set; }
        public Nullable<int> CUSTOM_FIELD_CONFIGURATION_ID { get; set; }
        public Nullable<int> CUSTOM_FIELD_TYPE_ID { get; set; }
        public byte DATA_TYPE_ID { get; set; }
        public Nullable<int> DDL_VALUE { get; set; }
        public Nullable<bool> BOOLEAN_VALUE { get; set; }
        public Nullable<int> INTEGER_VALUE { get; set; }
        public Nullable<decimal> DECIMAL_VALUE { get; set; }
        public Nullable<System.DateTime> DATETIME_VALUE { get; set; }
        public string TEXT_VALUE { get; set; }
        public string TEXT_AREA_VALUE { get; set; }
    }
}
