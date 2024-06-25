using SOL.Common.Models;
using SOL.Common.Business.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOL.EZBroker.DAL.DTO;

namespace SOL.WorkFlow.Interfaces
{
    public interface ICustomWorkflowRepository<T> : IBaseRepository<T>
   {   
       void SaveChanges();

        WORKFLOW_CUSTOM GetCustomWorkFlow(int customWorkflowId);
        WORKFLOW_DEFINITION GetWorkflowDefinition(int workflowDefinitionId);
       bool IsExistingIntegerValue(int integerValue, int id);
       bool IsExistingDecimalValue(decimal decimalValue, int id);
       bool IsExistingDateTimeValue(DateTime dateTime, int id);

        bool IsExistingTimeValue(TimeSpan dateTime, int id);
        bool IsExistingTextValue(string textValue, int id);
       bool IsExistingTextAreaValue(string textAreaValue, int id);
       bool IsExistingDDLValue(int ddlValue, int id);
       bool IsExistingMultipleDDLValue(int ddlValue, int id);

       void SaveCustomWorkFlow(WORKFLOW_CUSTOM workFlow); 
        
        void DeleteCustomFieldsEntry( List<WORKFLOW_CUSTOM_FIELD> CustomFieldsValue, int Entries,int WorkflowId, int _userId);
        
        void SaveEditCustomFieldValue( List<WORKFLOW_CUSTOM_FIELD> CustomFieldsValue);
        void SaveEditCustomMultiFieldValue( List<WORKFLOW_CUSTOM_FIELD_MULTIPLE> CustomFieldsValue);
        
        void DeleteCustomFieldsMultiEntry( List<WORKFLOW_CUSTOM_FIELD_MULTIPLE> CustomFieldsMultiValue, int Entries,int WorkflowId, int _userId);

        bool isExistWorkFlowTitle(string customWorkflowTitle, int customWorkflowId);

        List<WORKFLOW_CUSTOM_FIELD> GetCustomFiledsValue(int Entries, int WorkflowId);
        
        List<WORKFLOW_CUSTOM_FIELD_MULTIPLE> GetCustomFiledsMultiValue(int Entries, int WorkflowId);

       List<DTOGetCustomWorkflowLineItemById> GetCustomWorkflowLineItems(int customWorkflowId);

        int? GetWorkflowIdinWorkflow(int docId, int userId);
        bool GetIsDocViewableInWorkFlow(int contactId, int docId, int workflowId);
        int? GetCustomEntityIdInWorkFlow(int docId, int userId);
        int? GetFolderIdByWorkflowId(int customWorkflowId);

   }
}
