using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Models
{
    public class DocumentWorkflowStepsModel
    {
        public Nullable<int> ORDER_ID { get; set; }
        public Nullable<int> FOLDER_ID { get; set; }
        public Nullable<int> MOVE_ID { get; set; }
        public int STEP_ID { get; set; }
        public string FOLDER_PATH { get; set; }
       
    }
}
