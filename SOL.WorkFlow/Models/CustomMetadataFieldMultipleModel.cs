using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
   public class CustomMetadataFieldMultipleModel
   {
       public Nullable<bool> UNIQUE_FIELD { get; set; }
       public int ID { get; set; }
       public int BKC_ID { get; set; }
       public int CUSTOM_FIELD_ID { get; set; }
       public string FIELD_LABEL { get; set; }
       public int CUSTOM_FIELD_TYPE_ID { get; set; }
       public byte DATA_TYPE_ID { get; set; }
       public Nullable<int> VALUE { get; set; }
    }
}
