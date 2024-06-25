using SOL.Addressbook.Interfaces;
using SOL.Common.Models;
using SOL.Common.Business.Services;
using SOL.ECM.Models;
using SOL.PMS.Interfaces;
using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using SOL.ECM.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;

using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace SOL.WorkFlow.Services
{
    public class CustomWorkflowService<T> : BaseService<T>, ICustomWorkflowService<T>
    {
        ICustomWorkflowRepository<int> _repCustomWorkflow = null;
        IAddressbookRepository<T> _repAdrbook;
        IPmsService<T> _srvPms;
        IWorkflowDocumentService<T> _srvWfDocumentService = null;
        INodeService<T> _srvNode = null;
        IApprovalService<T> _srvApproval;
        IWorkFlowRepository<T> _repWorkFlow;
        IBillService<int> _srvBill = null;
        IWorkflowTimelineService<T> _srvTimeLine = null;
        INodeRepository<T> _repNode;

        string attachmentTempFolder = ConfigurationManager.AppSettings["TempDataFolder"].ToString();

        public CustomWorkflowService(ICustomWorkflowRepository<int> repCustomWorkflow,
            IAddressbookRepository<T> repAdrbook, IPmsService<T> srvPms, IWorkFlowRepository<T> repWorkFlow, IBillService<int> srvBill,
             IApprovalService<T> srvApproval, IWorkflowDocumentService<T> srvWfDocumentService, IWorkflowTimelineService<T> srvTimeLine, INodeService<T> srvNode, INodeRepository<T> repNode)
            : base(repAdrbook)
        {
            this._repCustomWorkflow = repCustomWorkflow;
            this._repAdrbook = repAdrbook;
            this._srvPms = srvPms;
            this._srvWfDocumentService = srvWfDocumentService;
            this._srvNode = srvNode;
            this._srvBill = srvBill;
            this._srvApproval = srvApproval;
            this._repWorkFlow = repWorkFlow;
            this._srvTimeLine = srvTimeLine;
            this._repNode = repNode;
        }

        public object CustomworkflowFields(CustomWorkflowCustomField CustomworkflowCustomFields,
                 ref string errorMessage, int _userId)
        {
            var obj = new object();
            var workFlow = new WORKFLOW_CUSTOM();
            var customWorkflowId = CustomworkflowCustomFields.WORKFLOW_ID;
            if (customWorkflowId == default(int))
            {
                //workFlow.CLIENT_ID = clientId;
                //workFlow.DATE_OF_ENTRY = DateTime.UtcNow;
                //workFlow.USER_ID = userId;
                //workFlow.DELETED_FLAG = false;
            }
            else
            {
                workFlow = _repCustomWorkflow.GetCustomWorkFlow(customWorkflowId);
                //clientId = workFlow.CLIENT_ID;
                //genrateDocument = false;
            }
            workFlow.WORKFLOW_CUSTOM_FIELD =  SetCustomField(CustomworkflowCustomFields.
                CustomWorkFlowCustomMetadataFieldValueModel, workFlow.WORKFLOW_CUSTOM_FIELD,
                ref errorMessage, _userId, customWorkflowId);
            _repCustomWorkflow.SaveCustomWorkFlow(workFlow);
            return obj;
        }
        public object CustomMultiworkflowFields(CustomWorkflowCustomField CustomworkflowCustomFields,
                ref string errorMessage, int _userId)
        {
            var obj = new object();
            var workFlow = new WORKFLOW_CUSTOM();
            var customWorkflowId = CustomworkflowCustomFields.WORKFLOW_ID;
            if (customWorkflowId == default(int))
            {
                //workFlow.CLIENT_ID = clientId;
                //workFlow.DATE_OF_ENTRY = DateTime.UtcNow;
                //workFlow.USER_ID = userId;
                //workFlow.DELETED_FLAG = false;
            }
            else
            {
                workFlow = _repCustomWorkflow.GetCustomWorkFlow(customWorkflowId);
                //clientId = workFlow.CLIENT_ID;
                //genrateDocument = false;
            }
            workFlow.WORKFLOW_CUSTOM_FIELD = SetmultiCustomField(CustomworkflowCustomFields.
                CustomWorkFlowCustomMetadataFieldValueModel, workFlow.WORKFLOW_CUSTOM_FIELD,
                ref errorMessage, _userId, customWorkflowId);
            _repCustomWorkflow.SaveCustomWorkFlow(workFlow);
            return obj;
        }
        public object CustomWorkflowFieldMultiple(CustomWorkflowCustomField CustomworkflowCustomFields,
                 ref string errorMessage, int _userId)
        {
            var obj = new object();
            var workFlow = new WORKFLOW_CUSTOM();
            var customWorkflowId = CustomworkflowCustomFields.WORKFLOW_ID;
            if (customWorkflowId == default(int))
            {
                //workFlow.CLIENT_ID = clientId;
                //workFlow.DATE_OF_ENTRY = DateTime.UtcNow;
                //workFlow.USER_ID = userId;
                //workFlow.DELETED_FLAG = false;
            }
            else
            {
                workFlow = _repCustomWorkflow.GetCustomWorkFlow(customWorkflowId);
                //clientId = workFlow.CLIENT_ID;
                //genrateDocument = false;
            }
            workFlow.WORKFLOW_CUSTOM_FIELD_MULTIPLE = SetCustomFieldMultiple(CustomworkflowCustomFields.
              CustomWorkFlowCustomMetadataFieldMultiValueModel, workFlow.WORKFLOW_CUSTOM_FIELD_MULTIPLE,
              ref errorMessage, _userId, customWorkflowId);
            _repCustomWorkflow.SaveCustomWorkFlow(workFlow);
            return obj;
        }

        public object CustomMultiWorkflowFieldMultiple(CustomWorkflowCustomField CustomworkflowCustomFields,
                 ref string errorMessage, int _userId)
        {
            var obj = new object();
            var workFlow = new WORKFLOW_CUSTOM();
            var customWorkflowId = CustomworkflowCustomFields.WORKFLOW_ID;
            if (customWorkflowId == default(int))
            {
                //workFlow.CLIENT_ID = clientId;
                //workFlow.DATE_OF_ENTRY = DateTime.UtcNow;
                //workFlow.USER_ID = userId;
                //workFlow.DELETED_FLAG = false;
            }
            else
            {
                workFlow = _repCustomWorkflow.GetCustomWorkFlow(customWorkflowId);
                //clientId = workFlow.CLIENT_ID;
                //genrateDocument = false;
            }
            workFlow.WORKFLOW_CUSTOM_FIELD_MULTIPLE = SetMultiCustomFieldMultiple(CustomworkflowCustomFields.
              CustomWorkFlowCustomMetadataFieldMultiValueModel, workFlow.WORKFLOW_CUSTOM_FIELD_MULTIPLE,
              ref errorMessage, _userId, customWorkflowId);
            _repCustomWorkflow.SaveCustomWorkFlow(workFlow);
            return obj;
        }

        public object SaveCustomWorkFlowDraftToNotstarted(workflowDraftToNotstarted customWorkFlowModel, int userId, UserType userType, string addedBy,
            ref string errorMessage, string companyLogo, string companyUrl)
        {
            var obj = new object();
            var workFlow = new WORKFLOW_CUSTOM();
            var customWorkflowId = customWorkFlowModel.WORKFLOW_ID;
            workFlow = _repCustomWorkflow.GetCustomWorkFlow(customWorkflowId);
            workFlow.WORKFLOW_STATUS = customWorkFlowModel.WORKFLOW_STATUS;
            _repCustomWorkflow.SaveCustomWorkFlow(workFlow);
            return obj;
        }
        
        public object EditCustomWorkFlowTitle(WorkflowCustomTilte customWorkFlowModel, int userId, UserType userType, string addedBy,
            ref string errorMessage, string companyLogo, string companyUrl)
        {
            var obj = new object();
            var workFlow = new WORKFLOW_CUSTOM();
            var customWorkflowId = customWorkFlowModel.WORKFLOW_ID;
            var entityType = 23;
            workFlow = _repCustomWorkflow.GetCustomWorkFlow(customWorkflowId);
            workFlow.TITLE = customWorkFlowModel.TITLE;
            _repCustomWorkflow.SaveCustomWorkFlow(workFlow);
            var FolderDetails = _repNode.GetFolderDeatils(customWorkflowId, entityType);
            FolderDetails.FOLDER_NAME = customWorkFlowModel.TITLE;
            _repNode.SaveFolder(FolderDetails, ref errorMessage);
            return obj;
        }
        
        public object DeleteCustomFieldsEntry(int Entries,int WorkflowId, int _userId, UserType _userType)
        {
            var obj = new object();
            List<WORKFLOW_CUSTOM_FIELD> CustomFieldsValue = new List<WORKFLOW_CUSTOM_FIELD>();
            CustomFieldsValue = _repCustomWorkflow.GetCustomFiledsValue(Entries, WorkflowId);
            List<WORKFLOW_CUSTOM_FIELD_MULTIPLE> CustomFieldsMultiValue = new List<WORKFLOW_CUSTOM_FIELD_MULTIPLE>();
            CustomFieldsMultiValue = _repCustomWorkflow.GetCustomFiledsMultiValue(Entries, WorkflowId);
            foreach (var customField in CustomFieldsValue)
            { 
            
            }
                _repCustomWorkflow.DeleteCustomFieldsEntry(CustomFieldsValue ,Entries, WorkflowId, _userId);
            _repCustomWorkflow.DeleteCustomFieldsMultiEntry(CustomFieldsMultiValue, Entries, WorkflowId, _userId);
            return obj;
        }

        public object SaveEditCustomFieldValue(CustomWorkFlowModel customWorkFlowModel, int _userId)
        {
            var obj = new object();
            var workFlow = new WORKFLOW_CUSTOM();
            List<WORKFLOW_CUSTOM_FIELD> CustomFieldsValue = new List<WORKFLOW_CUSTOM_FIELD>();
            if (customWorkFlowModel.CustomWorkFlowCustomMetadataFieldValueModel != null)
            {
                var CustomWorkflowfieldsDetails = customWorkFlowModel.CustomWorkFlowCustomMetadataFieldValueModel;
                foreach (var cusotmfields in CustomWorkflowfieldsDetails)
                {
                    var workflowCustomField = new WORKFLOW_CUSTOM_FIELD
                    {
                        WORKFLOW_CUSTOM_FIELD_ID = cusotmfields.CUSTOM_WORKFLOW_CUSTOM_FIELD_ID,
                        WORKFLOW_ID = customWorkFlowModel.WORKFLOW_ID,
                        CUSTOM_FIELD_TYPE_ID = cusotmfields.CUSTOM_FIELD_TYPE_ID,
                        DATA_TYPE_ID = cusotmfields.DATA_TYPE_ID,
                        DDL_VALUE = cusotmfields.DDL_VALUE,
                        BOOLEAN_VALUE = cusotmfields.BOOLEAN_VALUE,
                        INTEGER_VALUE = cusotmfields.INTEGER_VALUE,
                        DECIMAL_VALUE = cusotmfields.DECIMAL_VALUE,
                        DATETIME_VALUE = cusotmfields.DATETIME_VALUE,
                        TEXT_VALUE = cusotmfields.TEXT_VALUE,
                        TEXT_AREA_VALUE = cusotmfields.TEXT_AREA_VALUE,
                        TIME_VALUE = cusotmfields.TIME_VALUE,
                        USER_ID = _userId,
                        USER_MODIFIED = _userId,
                        DATE_MODIFIED = DateTime.UtcNow,
                        DELETED_FLAG = false,
                        DATE_OF_ENTRY = DateTime.UtcNow,
                        WORKFLOW_ENTRIES = cusotmfields.WORKFLOW_ENTRIES,
                    };

                    CustomFieldsValue.Add(workflowCustomField);
                }
                _repCustomWorkflow.SaveEditCustomFieldValue(CustomFieldsValue);
            }
            List<WORKFLOW_CUSTOM_FIELD_MULTIPLE> CustomFieldsMultiValue = new List<WORKFLOW_CUSTOM_FIELD_MULTIPLE>();
            if (customWorkFlowModel.CustomWorkFlowCustomMetadataFieldMultiValueModel!=null) {            
            var CustomWorkflowMultifieldsDetails = customWorkFlowModel.CustomWorkFlowCustomMetadataFieldMultiValueModel;
            foreach (var cusotmfields in CustomWorkflowMultifieldsDetails)
            {
                var workflowCustomField = new WORKFLOW_CUSTOM_FIELD_MULTIPLE
                {
                    WORKFLOW_CUSTOM_FIELD_MULTIPLE_ID = cusotmfields.CUSTOM_WORKFLOW_CUSTOM_FIELD_MULTIPLE_ID,
                    WORKFLOW_ID = customWorkFlowModel.WORKFLOW_ID,
                    CUSTOM_FIELD_TYPE_ID = cusotmfields.CUSTOM_FIELD_TYPE_ID,
                    DATA_TYPE_ID = cusotmfields.DATA_TYPE_ID,
                    VALUE = cusotmfields.VALUE,
                    USER_ID = _userId,
                    USER_MODIFIED = _userId,
                    DATE_MODIFIED = DateTime.UtcNow,
                    DELETED_FLAG = false,
                    DATE_OF_ENTRY = DateTime.UtcNow,
                    WORKFLOW_ENTRIES = cusotmfields.WORKFLOW_ENTRIES,
                };

                CustomFieldsMultiValue.Add(workflowCustomField);
            }
                _repCustomWorkflow.SaveEditCustomMultiFieldValue(CustomFieldsMultiValue);
            }          
            return obj;
        }



        public object SaveCustomWorkFlow(CustomWorkFlowModel customWorkFlowModel, int userId, UserType userType, string addedBy,
            ref string errorMessage, string companyLogo, string companyUrl)
        {
            var obj = new object();
            var workFlow = new WORKFLOW_CUSTOM();
            bool genrateDocument = true;
            bool isCheckOriginalName = false;
            var documentOperation = new DocumentOperation();
            var clientId = customWorkFlowModel.CLIENT_ID;
            var customWorkflowId = customWorkFlowModel.WORKFLOW_ID;
            var oldCustomWorkflowId = customWorkflowId;
            var workflowfolderId = customWorkFlowModel.FOLDER_ID;
            var customWorkflowTitle = customWorkFlowModel.TITLE;
            var approvalStatus = customWorkFlowModel.APPROVAL_STATUS;
            var workFlowOwner = customWorkFlowModel.WORKFLOW_OWNER;
            var workFlowStatus = customWorkFlowModel.WORKFLOW_STATUS;
            var workFlowOwnerIsRole = customWorkFlowModel.WORKFLOW_OWNER_IS_ROLE;
           // var workflowAllowMultipleEntires = customWorkFlowModel.ALLOW_MULTIPLE_ENTRIES;



            var isExistWorkflowTitle = _repCustomWorkflow.isExistWorkFlowTitle(customWorkflowTitle, customWorkflowId);
            if (isExistWorkflowTitle == true)
            {
                errorMessage = customWorkflowTitle + " already exists in your records. To add this document to the existing Workflow, click on Add to Existing.";
                return obj;
            }

            if (customWorkflowId == default(int))
            {
                workFlow.CLIENT_ID = clientId;
                workFlow.DATE_OF_ENTRY = DateTime.UtcNow;
                workFlow.USER_ID = userId;
                workFlow.DELETED_FLAG = false;
            }
            else
            {
                workFlow = _repCustomWorkflow.GetCustomWorkFlow(customWorkflowId);
                clientId = workFlow.CLIENT_ID;
                genrateDocument = false;
            }

            var workFlowId = customWorkFlowModel.WORKFLOW_DEFINITION_ID;
            workFlow.WORKFLOW_DEFINITION_ID = workFlowId;
            workFlow.TITLE = customWorkflowTitle;
            workFlow.APPROVAL_STATUS = approvalStatus;
            workFlow.DATE = DateTime.UtcNow;
            workFlow.DESCRIPTION = customWorkFlowModel.DESCRIPTION;
            workFlow.SAVE_AS_DRAFT = customWorkFlowModel.SAVE_AS_DRAFT;
            workFlow.USER_MODIFIED = userId;
            workFlow.DATE_MODIFIED = DateTime.UtcNow;
            workFlow.WORKFLOW_OWNER = workFlowOwner;
            workFlow.WORKFLOW_STATUS = workFlowStatus;
            workFlow.IS_ROLE = workFlowOwnerIsRole;
            //workFlow.ALLOW_MULTIPLE_ENTRIES = workflowAllowMultipleEntires;


            workFlow.WORKFLOW_CUSTOM_FIELD = SetCustomField(customWorkFlowModel.
                CustomWorkFlowCustomMetadataFieldValueModel, workFlow.WORKFLOW_CUSTOM_FIELD,
                ref errorMessage, userId, customWorkflowId);         
            if (errorMessage.Length > 0)
            {
                return obj;
            }
            workFlow.WORKFLOW_CUSTOM_FIELD_MULTIPLE = SetCustomFieldMultiple(customWorkFlowModel.
              CustomWorkFlowCustomMetadataFieldMultiValueModel, workFlow.WORKFLOW_CUSTOM_FIELD_MULTIPLE,
              ref errorMessage, userId, customWorkflowId);
            if (errorMessage.Length > 0)
            {
                return obj;
            }
            _repCustomWorkflow.SaveCustomWorkFlow(workFlow);
            customWorkflowId = workFlow.WORKFLOW_ID;
            //if (workFlow.WORKFLOW_CUSTOM_FIELD != null)
            //{
            //    _srvTimeLine.SaveWorflowTimeline(customWorkflowId, workFlowId, approvalStatus, 0, null,
            //    (int)WorkflowTimelineObject.Workflow_Details, (int)WorkflowTimelineEvents.Detail_Added, string.Empty, userId, DateTime.UtcNow, null, null);
            //}
            var WorkflowDocumentFolderId = _srvNode.GetFolderIdByParentIdFolderName( 1, "WorkFlow Documents");
            if(WorkflowDocumentFolderId == 0)
            {
                var objECMFoldersWorkflowDocument = new ECMFolder()
                {
                    FOLDER_ID = 0,
                    PARENT_ID = 1,
                    FOLDER_NAME = "WorkFlow Documents",
                    DESCRIPTION = null,
                    ACL_ID = null,
                    ENTITY_ID = null,
                    ENTITY_TYPE = 23,

                };
                WorkflowDocumentFolderId = _srvNode.SaveFolder(objECMFoldersWorkflowDocument, userId, false, false);
            }
            var WorkflowDefinitionDetail = new WORKFLOW_DEFINITION();
            WorkflowDefinitionDetail = _repCustomWorkflow.GetWorkflowDefinition(workFlowId);
            var WorkflowDefinitionFolder = _srvNode.GetFolderIdByParentIdFolderName(WorkflowDocumentFolderId, WorkflowDefinitionDetail.WORKFLOW_DEFINITION_TITLE);
           // var WorkflowDefinitionFolder = _srvNode.GetFolderIdByEntityId(workFlowId, 23);
            if(WorkflowDefinitionFolder == 0)
            {        
                var objECMFoldersWorkflowDefinition = new ECMFolder()
                {
                    FOLDER_ID = 0,
                    PARENT_ID = WorkflowDocumentFolderId,
                    FOLDER_NAME = WorkflowDefinitionDetail.WORKFLOW_DEFINITION_TITLE,
                    DESCRIPTION = null,
                    ACL_ID = null,
                    ENTITY_ID = workFlowId,
                    ENTITY_TYPE = 23,

                };
                WorkflowDefinitionFolder = _srvNode.SaveFolder(objECMFoldersWorkflowDefinition, userId, false, false);
            }

            if (customWorkFlowModel.CLIENT_ID != null)
            {
              var clientName = _repAdrbook.GetCompanyNameByCompanyId(customWorkFlowModel.CLIENT_ID.Value);
                var OldClientFolderId = _srvNode.GetFolderIdByParentIdFolderName(WorkflowDefinitionFolder, clientName);
                if(OldClientFolderId == 0)
                {
                    var objECMFoldersWorkflowDefinition = new ECMFolder()
                    {
                        FOLDER_ID = 0,
                        PARENT_ID = WorkflowDefinitionFolder,
                        FOLDER_NAME = clientName,
                        DESCRIPTION = null,
                        ACL_ID = null,
                        ENTITY_ID = workFlowId,
                        ENTITY_TYPE = 23,

                    };
                    OldClientFolderId = _srvNode.SaveFolder(objECMFoldersWorkflowDefinition, userId, false, false);
                }
                var objECMFolders = new ECMFolder()
                {
                    FOLDER_ID = 0,
                    PARENT_ID = OldClientFolderId,
                    FOLDER_NAME = $"{customWorkflowId} - {customWorkflowTitle}",
                    DESCRIPTION = null,
                    ACL_ID = null,
                    ENTITY_ID = customWorkflowId,
                    ENTITY_TYPE = 23,

                };
                workflowfolderId = _srvNode.SaveFolder(objECMFolders, userId, false, false);

            }

            else
            {
                var FolderAclId  = _repNode.GetAclByFolderId(WorkflowDefinitionFolder);
                var objECMFolders = new ECMFolder()
                {
                    FOLDER_ID = 0,
                    PARENT_ID = WorkflowDefinitionFolder,
                    FOLDER_NAME = $"{customWorkflowId} - {customWorkflowTitle}",
                    DESCRIPTION = null,
                    ACL_ID = FolderAclId,
                    ENTITY_ID = customWorkflowId,
                    ENTITY_TYPE = 23,

                };
                workflowfolderId = _srvNode.SaveFolder(objECMFolders, userId, false, false);
            }


        

            //var parentfolderId = _srvNode.GetFolderIdByEntityId(101, 23);
            //var objECMFolders = new ECMFolder()
            //{
            //    FOLDER_ID = 0,
            //    PARENT_ID = parentfolderId,
            //    FOLDER_NAME = customWorkflowTitle,
            //    DESCRIPTION = null,
            //    ACL_ID = null,
            //    ENTITY_ID = customWorkflowId,
            //    ENTITY_TYPE = 23,
            //};
            //workflowfolderId = _srvNode.SaveFolder(objECMFolders, userId, false, false);

            if (genrateDocument == true)
            {
                _srvTimeLine.SaveWorflowTimeline(customWorkflowId, workFlowId, approvalStatus, null, null,
                (int)WorkflowTimelineObject.CustomReport, (int)WorkflowTimelineEvents.Custom_Workflow_Created, string.Empty, userId, DateTime.UtcNow, null, null);
            }
            else
            {
                _srvTimeLine.SaveWorflowTimeline(customWorkflowId, workFlowId, approvalStatus, null, null,
                (int)WorkflowTimelineObject.CustomReport, (int)WorkflowTimelineEvents.Custom_Workflow_Updated, string.Empty, userId, DateTime.UtcNow, null, null);
            }



            var UploadFiles = customWorkFlowModel.UploadedFiles;
            var folderName = _srvNode.GetFolderNameByFolderId(workflowfolderId);
            var folder = _srvNode.GetFolder(workflowfolderId);
            var monthOfEntry = ECMGlobal.GetMonthYear();
            if (UploadFiles != null)
            {
                List<string> tempFileFullPath = new List<string>();

                var serverPathTempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, attachmentTempFolder);

                List<UploadDocumentModel> UploadDocumentModels = new List<UploadDocumentModel>();
                foreach (var item in UploadFiles)
                {
                    string strTempFolder = Path.Combine(serverPathTempFolder, item.SystemFileName);
                    tempFileFullPath.Add(strTempFolder);
                    UploadDocumentModel UploadDocumentModelss = new UploadDocumentModel()
                    {
                        SystemFileName = item.SystemFileName,
                        FileName = item.OrignalFileName,
                        FileSize = item.FileSize.Value,
                        SystemFileFullName = strTempFolder,
                        SystemLastModified = null,
                    };
                    UploadDocumentModels.Add(UploadDocumentModelss);
                }
                UploadDocument Documents = new UploadDocument()
                {
                    TempFileFullPath = tempFileFullPath,
                    DocIds = null,
                    DocumentVersionId = 0,
                    FolderId = workflowfolderId,
                    DocId = 0,
                    UserId = userId,
                    UserType = userType,
                    UserName = addedBy,
                    AttachmentTempFolder = null,
                    AttachmentFolder = "data\\",
                    EcmVersionsFolder = "Version",
                    FolderName = null,
                    libraryId = null,
                    EntityType = null,
                    EntityId = null,
                    WorkflowCustom = 0,
                    DocumentUploadType = 1,
                    TaskId = 0,
                    TimeSheetLineItemId = 0,
                    AttachEntityId = null,
                    AttachEntityTypeId = 0,
                    CommentId = null,
                    UploadDocumentModels = UploadDocumentModels,
                    TicketTitle = null,
                    TicketCommentId = 0,
                    isViewGenerated = 0,
                    isCheckOriginalName = null,
                    DocumentTypeId = 0,
                    IsPolicyLapsed = null,
                    ExistingDocuments = null,
                    ToDoMainId = 0,
                    ToDoOwnerId = 0,
                    UploadedDocuments = null,
                    HasWorkflow = false,
                    EXT_SOURCE_ID = null,
                    EXT_DOC_ID = null,
                    WorkFlowID = null,
                    EventTitle = null,
                    PermissionedUserIds = null,
                    isRecursiveEvent = false,
                    UploadedDocumentsDetails = null,
                };
                //DocumentWorkFlow documentId = null;
                _srvNode.UploadDocument(Documents, ref errorMessage);
            }

            //if (UploadFiles != null)
            //{
            //    int[] docIds;
            //    docIds = _srvNode.GetDocumentIdsByFolderId(Convert.ToInt32(workflowfolderId));
            //    foreach (var item in docIds)
            //    {
            //     //   _srvTimeLine.SaveWorflowTimeline(entityId, workflowId, (int)approvalStatus, item.docIds, null,
            //     //(int)WorkflowTimelineObject.Approver, (int)WorkflowTimelineEvents.Approver_Added, approvalNote, userId, date,
            //     //approver.ORDER_ID, approver.IS_ROLE);
            //    }
            //}
            var documents = _repNode.GetDocumentsByFolderId(workflowfolderId);
            List<DocumentWorkFlow> documentId = new List<DocumentWorkFlow>();
            foreach (var item in documents)
            {
                DocumentWorkFlow documentList = new DocumentWorkFlow
                {
                    DOC_ID = item.DOC_ID  // Assign the DOC_ID value here
                };
                documentId.Add(documentList);
                //_srvTimeLine.SaveWorflowTimeline(customWorkflowId, workFlowId, approvalStatus, item.DOC_ID, null,
                // (int)WorkflowTimelineObject.Workflow_Document, (int)WorkflowTimelineEvents.Document_Added, string.Empty, userId, DateTime.UtcNow, null, null);

            }

            var linkDocumentsIds = customWorkFlowModel.LINK_DOC_ID;
            if(linkDocumentsIds != null)
            {              
                foreach (var item in linkDocumentsIds)
                {
                    DocumentWorkFlow linkDocument = new DocumentWorkFlow
                    {
                        DOC_ID = item.DOC_ID  // Assign the DOC_ID value here
                    };
                    documentId.Add(linkDocument);

                //    _srvTimeLine.SaveWorflowTimeline(customWorkflowId, workFlowId, approvalStatus, item.DOC_ID, null,
                //(int)WorkflowTimelineObject.Workflow_Document, (int)WorkflowTimelineEvents.Document_Added, string.Empty, userId, DateTime.UtcNow, null, null);
                }

            }
            var docId = customWorkFlowModel.DOC_ID;
            var originelDocId = docId;
            var newDocId = 0;
            var isClosePage = true;
            var IsApproveMode = customWorkFlowModel.IsApproveMode;
            var associtaedPages = customWorkFlowModel.AssocitaedPages;
            var documentProcessingWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
            var isBookkeepingClientSpecific = true;
            var workFlowLevel = (byte)WorkflowGloble.WorkFlowLevel.AllowPageSelection;
            byte ByOverView = 0;
            if (documentProcessingWorkflow != null)
            {
                if (documentProcessingWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC.HasValue)
                {
                    isBookkeepingClientSpecific = documentProcessingWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC.Value;
                }
                workFlowLevel = documentProcessingWorkflow.WORK_FLOW_LEVEL.Value;
            }
            if (oldCustomWorkflowId == 0)
            {
                if (isBookkeepingClientSpecific)
                {
                    int projectId = _srvPms.GetProjectIdByClientId(clientId.Value);
                    _srvWfDocumentService.ManageCustomWorkflowDocuments(customWorkflowId, workFlowId, addedBy, approvalStatus,
                           genrateDocument, isCheckOriginalName, associtaedPages,workFlowLevel,
                            userId, userType, ref errorMessage, projectId, documentId, ref originelDocId,
                           ref newDocId, ref isClosePage, IsApproveMode, customWorkflowTitle);
                }
                else
                {
                    int processFolderId = 0;
                    int toBeReviewdFolderId = 0;
                    var steps = documentProcessingWorkflow.WORKFLOW_DEFINITION_STEPS.Where(x =>  x.DELETED_FLAG == false);
                    if (steps != null)
                    {
                        var step = steps.Where(x => x.STEP_ID == (int)WorkflowGloble.Step.ToBeProcessed).FirstOrDefault();
                        if (step != null)
                        {
                            if (step.FOLDER_ID.HasValue)
                            {
                                processFolderId = step.FOLDER_ID.Value;
                            }
                        }
                        step = steps.Where(x => x.STEP_ID == (int)WorkflowGloble.Step.ToBeReviewed).FirstOrDefault();
                        if (step != null)
                        {
                            if (step.FOLDER_ID.HasValue)
                            {
                                toBeReviewdFolderId = step.FOLDER_ID.Value;
                            }
                        }
                    }
                    if (documentId != null)
                    {
                        _srvWfDocumentService.ManageCustomFlowDocuments(customWorkflowId, workFlowId, documentId, userId, processFolderId, toBeReviewdFolderId,
                           userType, (int)approvalStatus, workFlowLevel, ref newDocId, ref originelDocId, ref isClosePage, customWorkflowTitle,
                           associtaedPages, addedBy, ref errorMessage);
                    }
                }
                _srvWfDocumentService.SaveDocumentEntity(workFlowId, customWorkflowId, documentId, workflowfolderId, userId);
                if (documentId.Count != 0)
                {
                    if (documentId != null)
                    {
                        //int[] docIdss = { 0 };
                        int[] docIdss = new int[documentId.Count];
                        for (int i = 0; i < documentId.Count; i++)
                        {
                            docIdss[i] = documentId[i].DOC_ID;
                        }
                        int[] WorkflowCustoms = { customWorkflowId };
                        int entityTypeTask = (int)ECMGlobal.ECMLibraryId.ContentStructure;
                        _srvNode.SaveExistTaskDocumentAssociation(docIdss, WorkflowCustoms, userId, entityTypeTask);
                    }

                }
                //if (documentId != null)
                //{
                //    int entityType = (int)ECMGlobal.ECMLibraryId.ContentStructure;
                //    //int[] docIdss = { };
                //    int[] entityId = { customWorkflowId };
                //    foreach (var item in documentId)
                //    {
                //        if (item.DOC_ID != 0)
                //        {
                //            int[] docIdss = { item.DOC_ID };
                //            //int[] docIdss = { item.DOC_ID};
                //            _srvBill.CopyDocumentToFolder(item.DOC_ID, workflowfolderId, workFlowId, userId, userType, ref errorMessage);
                //            _srvNode.SaveExistTaskDocumentAssociation(docIdss, entityId, userId, entityType);
                //        }
                //    }
                //}
                //_srvBill.UploadDocumentbyFolderId(uploadDocument, folderId, ref errorMessage);
            }
            else
            {
                newDocId = docId;
                if (approvalStatus != (int)FlowStatus.Draft)
                {
                    if (isBookkeepingClientSpecific)
                    {
                        int projectId = _srvPms.GetProjectIdByClientId(clientId.Value);
                        _srvWfDocumentService.MoveDocumentInToBeReviewedFolder(userId, userType, ref errorMessage,
                            docId, originelDocId, projectId);
                    }
                    else
                    {
                        int folderId = 0;
                        var steps = documentProcessingWorkflow.WORKFLOW_DEFINITION_STEPS.Where(x =>  x.DELETED_FLAG == false);
                        if (steps != null)
                        {
                            var step = steps.Where(x => x.STEP_ID == (int)WorkflowGloble.Step.ToBeReviewed).FirstOrDefault();
                            if (step != null)
                            {
                                if (step.FOLDER_ID.HasValue)
                                {
                                    folderId = step.FOLDER_ID.Value;
                                }
                            }
                        }
                        if (folderId != 0)
                        {
                            _srvWfDocumentService.MoveDocumentInToBeReviewedFolderByStep(userId, userType, ref errorMessage,
                                docId, originelDocId, folderId);
                        }
                    }
                }
            }
//if (approvalStatus != (int)FlowStatus.Draft)
           // {
                if (customWorkFlowModel.Approvers != null)
                {
                    _srvApproval.SaveApprovers(customWorkFlowModel.Approvers, customWorkFlowModel.NOTE_TO_PAYER, customWorkflowId,
                        workFlowId, userId, approvalStatus, ByOverView,companyLogo);
                }
          //  }
            //SendPreEmailNotification(customWorkFlowModel.Approvers, userId, workFlowId, customWorkflowId, companyLogo, companyUrl);
            obj = new
            {
                originelDocId = originelDocId,
                isClosePage = isClosePage,
                customWorkflowId = customWorkflowId,
                newDocId = newDocId
            };
            return obj;
        }

        private List<WORKFLOW_CUSTOM_FIELD_MULTIPLE> SetCustomFieldMultiple(CustomWorkFlowCustomMetadataFieldMultiValueModel[] metaDatas,
            ICollection<WORKFLOW_CUSTOM_FIELD_MULTIPLE> oldCustomeFields,
            ref string errorMessage, int userId, int bkcId)
        {
            if (oldCustomeFields != null)
            {
                foreach (var customField in oldCustomeFields)
                {
                    customField.DATE_MODIFIED = DateTime.UtcNow;
                    customField.USER_MODIFIED = userId;
                    customField.DELETED_FLAG = true;
                }
            }
            if (metaDatas != null)
            {
                var isExist = false;
                var labelList = new List<string>();
                foreach (var metaData in metaDatas)
                {
                    var isNew = false;
                    var isUnique = metaData.UNIQUE_FIELD;
                    var customFieldTypeId = metaData.CUSTOM_FIELD_TYPE_ID;
                    var dataTypeId = Convert.ToByte(metaData.DATA_TYPE_ID);
                    var label = metaData.FIELD_LABEL;
                    var id = metaData.CUSTOM_WORKFLOW_CUSTOM_FIELD_MULTIPLE_ID;
                    var value = metaData.VALUE;
                    WORKFLOW_CUSTOM_FIELD_MULTIPLE documentMetadataEntity;
                   // if (oldCustomeFields != null && oldCustomeFields.Count() > 0)
                      //  if (oldCustomeFields != null)
                   // {
                        documentMetadataEntity = oldCustomeFields.Where(x => x.VALUE == value).FirstOrDefault();
                        if (documentMetadataEntity == null)
                        {
                            documentMetadataEntity = new WORKFLOW_CUSTOM_FIELD_MULTIPLE();
                            documentMetadataEntity.WORKFLOW_ENTRIES = metaData.WORKFLOW_ENTRIES;
                            documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                            documentMetadataEntity.USER_ID = userId;
                            isNew = true;
                        }
                    //else
                    //{
                    //    id = documentMetadataEntity.WORKFLOW_CUSTOM_FIELD_MULTIPLE_ID;
                    //}
                    // }
                    else
                    {
                        documentMetadataEntity = new WORKFLOW_CUSTOM_FIELD_MULTIPLE();
                        documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        documentMetadataEntity.USER_ID = userId;
                        isNew = true;
                    }
                    documentMetadataEntity.CUSTOM_FIELD_TYPE_ID = customFieldTypeId;
                    documentMetadataEntity.DATA_TYPE_ID = dataTypeId;
                    documentMetadataEntity.DATE_MODIFIED = DateTime.UtcNow;
                    documentMetadataEntity.USER_MODIFIED = userId;
                    documentMetadataEntity.DELETED_FLAG = false;
                    documentMetadataEntity.VALUE = value;
                    if (isUnique == true)
                    {
                        if (metaData.VALUE.HasValue)
                        {
                            var isExisting = _repCustomWorkflow.IsExistingMultipleDDLValue(metaData.VALUE.Value, id);
                            if (isExisting)
                            {
                                isExist = true;
                                labelList.Add(label);
                            }
                        }
                    }
                    if (isNew)
                    {
                        oldCustomeFields.Add(documentMetadataEntity);
                    }
                }
                //if (isExist == true)
                //{
                //    var totalCount = labelList.Count;
                //    var lable = string.Empty;
                //    for (int i = 0; i < 1; i++)
                //    {
                //        lable += '"' + labelList[i] + '"';
                //    }
                //    for (int i = 1; i < totalCount - 1; i++)
                //    {
                //        lable += ", " + '"' + labelList[i] + '"';
                //    }
                //    if (totalCount != 1)
                //    {
                //        for (int i = totalCount - 1; i < totalCount; i++)
                //        {
                //            lable += " and " + '"' + labelList[i] + '"';
                //        }
                //    }
                //    errorMessage = string.Format("The value specified for the field(s) {0} already exists in metadata of another document.", lable);
                //}
            }
            return oldCustomeFields.ToList();
        }
        private List<WORKFLOW_CUSTOM_FIELD_MULTIPLE> SetMultiCustomFieldMultiple(CustomWorkFlowCustomMetadataFieldMultiValueModel[] metaDatas,
            ICollection<WORKFLOW_CUSTOM_FIELD_MULTIPLE> oldCustomeFields,
            ref string errorMessage, int userId, int bkcId)
        {
            if (oldCustomeFields != null)
            {
                foreach (var customField in oldCustomeFields)
                {
                    //customField.DATE_MODIFIED = DateTime.UtcNow;
                    //customField.USER_MODIFIED = userId;
                    //customField.DELETED_FLAG = false;
                }
            }
            if (metaDatas != null)
            {
                var isExist = false;
                var labelList = new List<string>();
                foreach (var metaData in metaDatas)
                {
                    var isNew = false;
                    var isUnique = metaData.UNIQUE_FIELD;
                    var customFieldTypeId = metaData.CUSTOM_FIELD_TYPE_ID;
                    var workflowEntries = metaData.WORKFLOW_ENTRIES;
                    var dataTypeId = Convert.ToByte(metaData.DATA_TYPE_ID);
                    var label = metaData.FIELD_LABEL;
                    var id = metaData.CUSTOM_WORKFLOW_CUSTOM_FIELD_MULTIPLE_ID;
                    var value = metaData.VALUE;
                    WORKFLOW_CUSTOM_FIELD_MULTIPLE documentMetadataEntity;
                    // if (oldCustomeFields != null && oldCustomeFields.Count() > 0)
                    //  if (oldCustomeFields != null)
                    // {
                    documentMetadataEntity = oldCustomeFields.Where(x => x.VALUE == value).FirstOrDefault();
                    if (documentMetadataEntity == null)
                    {
                        documentMetadataEntity = new WORKFLOW_CUSTOM_FIELD_MULTIPLE();
                        documentMetadataEntity.WORKFLOW_ENTRIES = metaData.WORKFLOW_ENTRIES;
                        documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        documentMetadataEntity.USER_ID = userId;
                        isNew = true;
                    }
                    //else
                    //{
                    //    id = documentMetadataEntity.WORKFLOW_CUSTOM_FIELD_MULTIPLE_ID;
                    //}
                    // }
                    else
                    {
                        documentMetadataEntity = new WORKFLOW_CUSTOM_FIELD_MULTIPLE();
                        documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        documentMetadataEntity.USER_ID = userId;
                        isNew = true;
                    }
                    documentMetadataEntity.CUSTOM_FIELD_TYPE_ID = customFieldTypeId;
                    documentMetadataEntity.WORKFLOW_ENTRIES = workflowEntries;
                    documentMetadataEntity.DATA_TYPE_ID = dataTypeId;
                    documentMetadataEntity.DATE_MODIFIED = DateTime.UtcNow;
                    documentMetadataEntity.USER_MODIFIED = userId;
                    documentMetadataEntity.DELETED_FLAG = false;
                    documentMetadataEntity.VALUE = value;
                    if (isUnique == true)
                    {
                        if (metaData.VALUE.HasValue)
                        {
                            var isExisting = _repCustomWorkflow.IsExistingMultipleDDLValue(metaData.VALUE.Value, id);
                            if (isExisting)
                            {
                                isExist = true;
                                labelList.Add(label);
                            }
                        }
                    }
                    if (isNew)
                    {
                        oldCustomeFields.Add(documentMetadataEntity);
                    }
                }
                //if (isExist == true)
                //{
                //    var totalCount = labelList.Count;
                //    var lable = string.Empty;
                //    for (int i = 0; i < 1; i++)
                //    {
                //        lable += '"' + labelList[i] + '"';
                //    }
                //    for (int i = 1; i < totalCount - 1; i++)
                //    {
                //        lable += ", " + '"' + labelList[i] + '"';
                //    }
                //    if (totalCount != 1)
                //    {
                //        for (int i = totalCount - 1; i < totalCount; i++)
                //        {
                //            lable += " and " + '"' + labelList[i] + '"';
                //        }
                //    }
                //    errorMessage = string.Format("The value specified for the field(s) {0} already exists in metadata of another document.", lable);
                //}
            }
            return oldCustomeFields.ToList();
        }
        private List<WORKFLOW_CUSTOM_FIELD> SetCustomField(CustomWorkFlowCustomMetadataFieldValueModel[] metaDatas,
            ICollection<WORKFLOW_CUSTOM_FIELD> oldCustomeFields,
            ref string errorMessage, int userId, int bkcId)
        {
            if (oldCustomeFields != null)
            {
                foreach (var customField in oldCustomeFields)
                {
                    customField.DATE_MODIFIED = DateTime.UtcNow;
                    customField.USER_MODIFIED = userId;
                    customField.DELETED_FLAG = true;
                    customField.BOOLEAN_VALUE = null;
                    customField.DATETIME_VALUE = null;
                    customField.DDL_VALUE = null;
                    customField.DECIMAL_VALUE = null;
                    customField.INTEGER_VALUE = null;
                    customField.TEXT_AREA_VALUE = null;
                    customField.TEXT_VALUE = null;
                }
            }
            if (metaDatas != null)
            {
                var isExist = false;
                var labelList = new List<string>();
                foreach (var metaData in metaDatas)
                {
                    
                    var isNew = false;
                    var isUnique = metaData.UNIQUE_FIELD;
                    var id = metaData.CUSTOM_FIELD_TYPE_ID;
                    var dataTypeId = Convert.ToByte(metaData.DATA_TYPE_ID);
                    var label = metaData.FIELD_LABEL;

                    WORKFLOW_CUSTOM_FIELD documentMetadataEntity;
                    
                   // if (oldCustomeFields != null && oldCustomeFields.Count() > 0)
                   //if (oldCustomeFields != null)
                   // {
                        documentMetadataEntity = oldCustomeFields.Where(x => x.CUSTOM_FIELD_TYPE_ID == id).FirstOrDefault();
                        if (documentMetadataEntity == null)
                        {
                            documentMetadataEntity = new WORKFLOW_CUSTOM_FIELD();
                            documentMetadataEntity.WORKFLOW_ENTRIES = metaData.WORKFLOW_ENTRIES;
                            documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                            documentMetadataEntity.USER_ID = userId;
                            isNew = true;
                        }
                    // }
                    else
                    {
                        documentMetadataEntity = new WORKFLOW_CUSTOM_FIELD();
                        documentMetadataEntity.WORKFLOW_ENTRIES = metaData.WORKFLOW_ENTRIES;
                        documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        documentMetadataEntity.USER_ID = userId;
                        isNew = true;
                    }
                    documentMetadataEntity.CUSTOM_FIELD_TYPE_ID = id;
                    documentMetadataEntity.DATA_TYPE_ID = dataTypeId;
                    documentMetadataEntity.DATE_MODIFIED = DateTime.UtcNow;
                    documentMetadataEntity.USER_MODIFIED = userId;
                    documentMetadataEntity.DELETED_FLAG = false;

                    switch ((ECMGlobal.MetadataType)dataTypeId)
                    {
                        case ECMGlobal.MetadataType.Boolean:
                            var booleanValue = metaData.BOOLEAN_VALUE;
                            documentMetadataEntity.BOOLEAN_VALUE = booleanValue;
                            break;
                        case ECMGlobal.MetadataType.Integer:
                            var integerValue = metaData.INTEGER_VALUE;
                            documentMetadataEntity.INTEGER_VALUE = integerValue;
                            if (isUnique == true)
                            {
                                if (integerValue.HasValue)
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingIntegerValue(integerValue.Value, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }

                            }
                            break;
                        case ECMGlobal.MetadataType.Decimal:
                            var decimalValue = metaData.DECIMAL_VALUE;
                            documentMetadataEntity.DECIMAL_VALUE = decimalValue;
                            if (isUnique == true)
                            {
                                if (decimalValue.HasValue)
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingDecimalValue(decimalValue.Value, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.DateTime:
                            var dateTimeValue = metaData.DATETIME_VALUE;
                            documentMetadataEntity.DATETIME_VALUE = dateTimeValue;
                            if (isUnique == true)
                            {
                                if (dateTimeValue.HasValue)
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingDateTimeValue(dateTimeValue.Value, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.Time:
                            var TimeValue = metaData.TIME_VALUE;
                            documentMetadataEntity.TIME_VALUE = TimeValue;
                            if (isUnique == true)
                            {
                                if (TimeValue.HasValue)
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingTimeValue(TimeValue.Value, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.Text:
                            var textValue = metaData.TEXT_VALUE;
                            documentMetadataEntity.TEXT_VALUE = textValue;
                            if (isUnique == true)
                            {
                                if (textValue != null && textValue != "")
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingTextValue(textValue, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.TextArea:
                            var textAreaValue = metaData.TEXT_AREA_VALUE;
                            documentMetadataEntity.TEXT_AREA_VALUE = textAreaValue;
                            if (isUnique == true)
                            {
                                if (textAreaValue != null && textAreaValue != "")
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingTextAreaValue(textAreaValue, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.DropDown:
                            var ddlValue = metaData.DDL_VALUE;
                            documentMetadataEntity.DDL_VALUE = ddlValue;
                            if (isUnique == true)
                            {
                                if (ddlValue != null && ddlValue != 0)
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingDDLValue(ddlValue.Value, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;

                    }
                    if (isNew)
                    {
                        oldCustomeFields.Add(documentMetadataEntity);
                    }
                }
                //if (isExist == true)
                //{
                //    var totalCount = labelList.Count;
                //    var lable = string.Empty;
                //    for (int i = 0; i < 1; i++)
                //    {
                //        lable += '"' + labelList[i] + '"';
                //    }
                //    for (int i = 1; i < totalCount - 1; i++)
                //    {
                //        lable += ", " + '"' + labelList[i] + '"';
                //    }
                //    if (totalCount != 1)
                //    {
                //        for (int i = totalCount - 1; i < totalCount; i++)
                //        {
                //            lable += " and " + '"' + labelList[i] + '"';
                //        }
                //    }
                //    errorMessage = string.Format("The value specified for the field(s) {0} already exists in metadata of another document.", lable);
                //}
            }
            return oldCustomeFields.ToList();
        }
        private List<WORKFLOW_CUSTOM_FIELD> SetmultiCustomField(CustomWorkFlowCustomMetadataFieldValueModel[] metaDatas,
            ICollection<WORKFLOW_CUSTOM_FIELD> oldCustomeFields,
            ref string errorMessage, int userId, int bkcId)
        {
            if (oldCustomeFields != null)
            {
                foreach (var customField in oldCustomeFields)
                {
                    //customField.DATE_MODIFIED = DateTime.UtcNow;
                    //customField.USER_MODIFIED = userId;
                    //customField.DELETED_FLAG = false;
                    //customField.BOOLEAN_VALUE = null;
                    //customField.DATETIME_VALUE = null;
                    //customField.DDL_VALUE = null;
                    //customField.DECIMAL_VALUE = null;
                    //customField.INTEGER_VALUE = null;
                    //customField.TEXT_AREA_VALUE = null;
                    //customField.TEXT_VALUE = null;
                }
            }
            if (metaDatas != null)
            {
                var isExist = false;
                var labelList = new List<string>();
                foreach (var metaData in metaDatas)
                {

                    var isNew = false;
                    var isUnique = metaData.UNIQUE_FIELD;
                    var id = metaData.CUSTOM_FIELD_TYPE_ID;
                    var dataTypeId = Convert.ToByte(metaData.DATA_TYPE_ID);
                    var label = metaData.FIELD_LABEL;

                    WORKFLOW_CUSTOM_FIELD documentMetadataEntity;

                    // if (oldCustomeFields != null && oldCustomeFields.Count() > 0)
                    //if (oldCustomeFields != null)
                    // {
                    documentMetadataEntity = oldCustomeFields.Where(x => x.CUSTOM_FIELD_TYPE_ID == id).FirstOrDefault();
                    if (documentMetadataEntity == null)
                    {
                        documentMetadataEntity = new WORKFLOW_CUSTOM_FIELD();
                        documentMetadataEntity.WORKFLOW_ENTRIES = metaData.WORKFLOW_ENTRIES;
                        documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        documentMetadataEntity.USER_ID = userId;
                        isNew = true;
                    }
                    // }
                    else
                    {
                        documentMetadataEntity = new WORKFLOW_CUSTOM_FIELD();
                        documentMetadataEntity.WORKFLOW_ENTRIES = metaData.WORKFLOW_ENTRIES;
                        documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        documentMetadataEntity.USER_ID = userId;
                        isNew = true;
                    }
                    documentMetadataEntity.CUSTOM_FIELD_TYPE_ID = id;
                    documentMetadataEntity.DATA_TYPE_ID = dataTypeId;
                    documentMetadataEntity.DATE_MODIFIED = DateTime.UtcNow;
                    documentMetadataEntity.USER_MODIFIED = userId;
                    documentMetadataEntity.DELETED_FLAG = false;

                    switch ((ECMGlobal.MetadataType)dataTypeId)
                    {
                        case ECMGlobal.MetadataType.Boolean:
                            var booleanValue = metaData.BOOLEAN_VALUE;
                            documentMetadataEntity.BOOLEAN_VALUE = booleanValue;
                            break;
                        case ECMGlobal.MetadataType.Integer:
                            var integerValue = metaData.INTEGER_VALUE;
                            documentMetadataEntity.INTEGER_VALUE = integerValue;
                            if (isUnique == true)
                            {
                                if (integerValue.HasValue)
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingIntegerValue(integerValue.Value, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }

                            }
                            break;
                        case ECMGlobal.MetadataType.Decimal:
                            var decimalValue = metaData.DECIMAL_VALUE;
                            documentMetadataEntity.DECIMAL_VALUE = decimalValue;
                            if (isUnique == true)
                            {
                                if (decimalValue.HasValue)
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingDecimalValue(decimalValue.Value, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.DateTime:
                            var dateTimeValue = metaData.DATETIME_VALUE;
                            documentMetadataEntity.DATETIME_VALUE = dateTimeValue;
                            if (isUnique == true)
                            {
                                if (dateTimeValue.HasValue)
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingDateTimeValue(dateTimeValue.Value, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.Text:
                            var textValue = metaData.TEXT_VALUE;
                            documentMetadataEntity.TEXT_VALUE = textValue;
                            if (isUnique == true)
                            {
                                if (textValue != null && textValue != "")
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingTextValue(textValue, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.Time:
                            var TimeValue = metaData.TIME_VALUE;
                            documentMetadataEntity.TIME_VALUE = TimeValue;
                            if (isUnique == true)
                            {
                                if (TimeValue.HasValue)
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingTimeValue(TimeValue.Value, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.TextArea:
                            var textAreaValue = metaData.TEXT_AREA_VALUE;
                            documentMetadataEntity.TEXT_AREA_VALUE = textAreaValue;
                            if (isUnique == true)
                            {
                                if (textAreaValue != null && textAreaValue != "")
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingTextAreaValue(textAreaValue, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.DropDown:
                            var ddlValue = metaData.DDL_VALUE;
                            documentMetadataEntity.DDL_VALUE = ddlValue;
                            if (isUnique == true)
                            {
                                if (ddlValue != null && ddlValue != 0)
                                {
                                    var isExisting = _repCustomWorkflow.IsExistingDDLValue(ddlValue.Value, id);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;

                    }
                    if (isNew)
                    {
                        oldCustomeFields.Add(documentMetadataEntity);
                    }
                }
                //if (isExist == true)
                //{
                //    var totalCount = labelList.Count;
                //    var lable = string.Empty;
                //    for (int i = 0; i < 1; i++)
                //    {
                //        lable += '"' + labelList[i] + '"';
                //    }
                //    for (int i = 1; i < totalCount - 1; i++)
                //    {
                //        lable += ", " + '"' + labelList[i] + '"';
                //    }
                //    if (totalCount != 1)
                //    {
                //        for (int i = totalCount - 1; i < totalCount; i++)
                //        {
                //            lable += " and " + '"' + labelList[i] + '"';
                //        }
                //    }
                //    errorMessage = string.Format("The value specified for the field(s) {0} already exists in metadata of another document.", lable);
                //}
            }
            return oldCustomeFields.ToList();
        }
        public void ApproveCustomWorkflowStatus(UpdateStatusViewModel updateStatusViewModel, int userId, UserType userType, string companyLogo, string companyUrl)
        {
            var workFlowId = updateStatusViewModel.WorkFlowId;
            string errorMessage = string.Empty;
            var flowStatus = updateStatusViewModel.Status;
            var workflowDefinitionId = updateStatusViewModel.WorkflowDefinitionId;
            var workflowCustomId = updateStatusViewModel.WorkFlowId;
            _srvApproval.ManageApproversOnApprovals(updateStatusViewModel.Approvers, updateStatusViewModel.ApproverNote, updateStatusViewModel.WorkflowOwner,
                flowStatus, workflowDefinitionId, workFlowId, userId, companyLogo);
            var customWorkFlow = _repCustomWorkflow.GetCustomWorkFlow(workflowCustomId);
            var isAnyPendingAprrovers = _srvApproval.IsAnyPendingAprrover(workflowDefinitionId, workFlowId);
           // var docId = _repWorkFlow.GetNewDocIdByEntityId(workflowDefinitionId, workFlowId);
            //var documentProcessingWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
            //var isBookkeepingClientSpecific = false;
            //if (documentProcessingWorkflow != null)
            //{
            //    if (documentProcessingWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC.HasValue)
            //    {
            //        isBookkeepingClientSpecific = documentProcessingWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC.Value;
            //    }
            //}
            //if (flowStatus == (int)FlowStatus.Denied)
            //{
            //    if (isBookkeepingClientSpecific)
            //    {
            //        _srvWfDocumentService.MoVeDocumentToDeniedFolder(userId, userType, ref errorMessage,
            //           docId, docId, workFlowId);
            //    }
            //    else
            //    {
            //        int folderId = 0;
            //        var steps = documentProcessingWorkflow.WORKFLOW_DEFINITION_STEPS.Where(x =>  x.DELETED_FLAG == false);
            //        if (steps != null)
            //        {
            //            var step = steps.Where(x => x.STEP_ID == (int)WorkflowGloble.Step.ToBeReviewed).FirstOrDefault();
            //            if (step != null)
            //            {
            //                if (step.FOLDER_ID.HasValue)
            //                {
            //                    folderId = step.FOLDER_ID.Value;
            //                }
            //            }
            //        }
            //        if (folderId != 0)
            //        {
            //            _srvWfDocumentService.MoVeDocumentToDeniedFolderByStep(userId, userType,
            //           docId, folderId, ref errorMessage);
            //        }
            //    }
            //    customWorkFlow.APPROVAL_STATUS = flowStatus;
            //}
            //else if (isAnyPendingAprrovers == false)
            //{
            //    //if (isBookkeepingClientSpecific)
            //    //{
            //    //    _srvWfDocumentService.MoVeDocumentToCompletedFolder(userId, userType, ref errorMessage, docId, docId, string.Empty, workFlowId);
            //    //}
            //    //else
            //    //{
            //    //    int folderId = 0;
            //    //    var steps = documentProcessingWorkflow.WORKFLOW_DEFINITION_STEPS.Where(x =>  x.DELETED_FLAG == false);
            //    //    if (steps != null)
            //    //    {
            //    //        var step = steps.Where(x => x.STEP_ID == (int)WorkflowGloble.Step.ProcessedFinancialDocuments).FirstOrDefault();
            //    //        if (step != null)
            //    //        {
            //    //            if (step.FOLDER_ID.HasValue)
            //    //            {
            //    //                folderId = step.FOLDER_ID.Value;
            //    //            }
            //    //        }
            //    //    }
            //    //    if (folderId != 0)
            //    //    {
            //    //        _srvWfDocumentService.MoVeDocumentToCompletedFolderByStep(userId, userType,
            //    //                    docId, folderId, ref errorMessage);
            //    //    }
            //    //}
            //    customWorkFlow.APPROVAL_STATUS = flowStatus;
            //}
            customWorkFlow.APPROVAL_STATUS = flowStatus;
            customWorkFlow.USER_MODIFIED = userId;
            customWorkFlow.DATE_MODIFIED = DateTime.UtcNow;
            _repCustomWorkflow.SaveCustomWorkFlow(customWorkFlow);
            if (flowStatus == (int)FlowStatus.Denied)
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workflowDefinitionId, workFlowId, companyUrl,
                  companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Denied, false, null);
            }
            else if (isAnyPendingAprrovers == false || flowStatus == (int)FlowStatus.Denied)
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workflowDefinitionId, workFlowId, companyUrl,
                 companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Approved, false, null);

                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workflowDefinitionId, workFlowId, companyUrl,
                 companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.completed, false, null);
            }
            else
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workflowDefinitionId, workFlowId, companyUrl,
                 companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Approved, false, null);

                var approver = _srvApproval.GetNextApprover(workflowDefinitionId, workFlowId);
                if (approver != null)
                {
                    if (approver.IS_ROLE.Value == true)
                    {
                        var users = _srvApproval.GetRoleUsers(approver.CONTACT_ID);
                        if (users.Count() > 0)
                        {
                            foreach (var contactId in users)
                            {
                                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(contactId, workflowDefinitionId, workFlowId, companyUrl,
                                companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, false, null);
                            }
                        }
                    }
                    else
                    {
                        _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(approver.CONTACT_ID, workflowDefinitionId, workFlowId, companyUrl,
                            companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, false, null);
                    }
                }
            }
        }

        private void SendPreEmailNotification(WORKFLOW_REVIEWER[] approvers, int userId,
           int workFlowId, int entityId, string companyLogo, string companyUrl)
        {
            var isPreApproveUser = _srvApproval.IsCurrentUserPreApprover(userId, approvers);
            if (isPreApproveUser)
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                         companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, false, null);
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                        companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Approved, false, null);
            }
            var preApprovedUsers = _srvApproval.IsPreApproverEmailNotification(userId, approvers);
            if (preApprovedUsers.Count() > 0)
            {
                foreach (var contactId in preApprovedUsers)
                {
                    if (contactId != userId)
                    {
                        _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(contactId, workFlowId, entityId, companyUrl,
                       companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, true, userId);

                        _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(contactId, workFlowId, entityId, companyUrl,
                         companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.Approved, true, userId);
                    }
                }
            }
            var approver = _srvApproval.GetNextApprover(workFlowId, entityId);
            if (approver != null)
            {
                if (approver.IS_ROLE.Value == true)
                {
                    var users = _srvApproval.GetRoleUsers(approver.CONTACT_ID);
                    if (users.Count() > 0)
                    {
                        foreach (var contactId in users)
                        {
                            _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(contactId, workFlowId, entityId, companyUrl,
                            companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, false, null);
                        }
                    }
                }
                else
                {
                    _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(approver.CONTACT_ID, workFlowId, entityId, companyUrl,
                        companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.ApprovalWaitting, false, null);
                }
            }
            var isAnyPendingAprrovers = _srvApproval.IsAnyPendingAprrover(entityId, workFlowId);
            if (isAnyPendingAprrovers == false)
            {
                _srvWfDocumentService.SaveWorkFlowEmailEventEntryInOutBound(userId, workFlowId, entityId, companyUrl,
                           companyLogo, (int)WorkflowGloble.WorkFlowEmailEventType.completed, false, null);
            }
        }


        public WORKFLOW_CUSTOM GetCustomWorkFlow(int customWorkflowId)
        {
            return _repCustomWorkflow.GetCustomWorkFlow(customWorkflowId);
        }

        public int? GetWorkflowIdinWorkflow(int docId, int userId)
        {
            return _repCustomWorkflow.GetWorkflowIdinWorkflow(docId, userId);
        }

        public int? GetCustomEntityIdInWorkFlow(int docId, int userId)
        {
            return _repCustomWorkflow.GetCustomEntityIdInWorkFlow(docId, userId);
        }
      
        public bool IsDocViewableInWorkFlow(int contactId, int docId, int workflowId)
        {
            return _repCustomWorkflow.GetIsDocViewableInWorkFlow(contactId, docId, workflowId);
        }

        public int? GetFolderIdByWorkflowId(int customWorkflowId)
        {
            return _repCustomWorkflow.GetFolderIdByWorkflowId(customWorkflowId);
        }

    }
}
