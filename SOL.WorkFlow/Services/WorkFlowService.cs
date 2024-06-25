using SOL.Common.Models;
using SOL.Common.Business.Events.Workflow;
using SOL.Common.Business.Interfaces;
using SOL.PMS.Interfaces;
using SOL.ECM.Interfaces;
using SOL.ECM.Models;
using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOL.Addressbook.Interfaces;
using SOL.Admin.Interfaces;
using SOL.Admin.Models;

namespace SOL.WorkFlow.Services
{
    public class WorkFlowService<T> : IWorkFlowService<T>
    {
        IWorkFlowRepository<T> _repWorkFlow;
        IEventPublisher _eventPublisher;
        private INodeService<T> _srvNode;
        private IPmsService<T> _srvPms;
        IAddressbookService<int> _srvAddressbook = null;
        IAdminService<int> _srvAdmin = null;

        string attachmentTempFolder = ConfigurationManager.AppSettings["TempDataFolder"].ToString();
        string attachmentFolder = ConfigurationManager.AppSettings["Documents"].ToString();
        string ecmVersionsFolder = ConfigurationManager.AppSettings["ECMVersionsFolder"].ToString();
        string openOfficePath = ConfigurationManager.AppSettings["PortableOpenOfficeExecutable"].ToString();
        string openOfficeRequestTimeout = ConfigurationManager.AppSettings["OpenOfficeRequestTimeOutSeconds"].ToString();
        string imageConversionMethod = ConfigurationManager.AppSettings["imageConversionMethod"].ToString();
        string isopenOffice = ConfigurationManager.AppSettings["UseOpenOffice"].ToString();

        public WorkFlowService(IWorkFlowRepository<T> repWorkFlow, IEventPublisher eventPublisher, INodeService<T> srvNode,
            IPmsService<T> srvPms, IAddressbookService<int> srvAddressbook, IAdminService<int> srvAdmin)
        {
            this._repWorkFlow = repWorkFlow;
            this._eventPublisher = eventPublisher;
            this._srvNode = srvNode;
            this._srvPms = srvPms;
            this._srvAddressbook = srvAddressbook;
            this._srvAdmin = srvAdmin;
        }

        # region Workflow

        /// <summary>
        /// Saves the work flow.
        /// </summary>
        /// <param name="documentWorkflowModel">The document workflow model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public int SaveWorkFlow(WorkFlowDetailsViewModel documentWorkflowModel, int userId, ref string errorMessage, int userType, ref bool isAuthorized)
        {
            var workFlowId = documentWorkflowModel.WORKFLOW_ID;

            var documentWorkflow = new WORKFLOW_DEFINITION();
            var workFlowLevel = documentWorkflowModel.WORK_FLOW_LEVEL;
            if (workFlowId == 0)
            {
                documentWorkflow.USER_ID = userId;
                documentWorkflow.DATE_OF_ENTRY = DateTime.UtcNow;
                documentWorkflow.USER_MODIFIED = userId;
                documentWorkflow.DATE_MODIFIED = DateTime.UtcNow;
                documentWorkflow.DELETED_FLAG = false;
                documentWorkflow.ACTIVE_FLAG = documentWorkflowModel.ACTIVE_FLAG;
                documentWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC = false;
                documentWorkflow.WORK_FLOW_LEVEL = (int)WorkflowGloble.WorkFlowLevel.NotAllowPageSelection;
            }
            else
            {
                documentWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
                documentWorkflow.USER_MODIFIED = userId;
                documentWorkflow.DATE_MODIFIED = DateTime.UtcNow;
                documentWorkflow.ACTIVE_FLAG = documentWorkflowModel.ACTIVE_FLAG;
            }
            //Start
            //var showSystemACLAdministrator = _srvAdmin.IsUserSystemAdmin(userId);
            if (userType != (int)UserType.Admin && userType != (int)UserType.SystemAdmin)
            {
                int? aclId;
                if (workFlowId == 0)
                {
                     aclId = _srvAdmin.GetModuleAcl(SOL.Admin.Models.Module.DocumentProcessingWorkflow);
                }
                else
                {
                    aclId = documentWorkflow.ACL_ID;
                }
                
                if (!aclId.HasValue)
                {
                    isAuthorized = false;
                    return 0;
                }
                var permissions = _srvAdmin.GetPermissions(userId, aclId.Value).ToList();
                if (!permissions.Any(x => x == (byte)Permission.Owner || x == (byte)Permission.Write))
                {
                    isAuthorized = false;
                    return 0;
                }
            }
            isAuthorized = true;
            //End
            
            
            documentWorkflow.WORKFLOW_DEFINITION_TITLE = documentWorkflowModel.WORKFLOW_TITLE;
            documentWorkflow.SHORT_TITLE = documentWorkflowModel.SHORT_TITLE;
            documentWorkflow.CUSTOME_FIELD_ID = documentWorkflowModel.CUSTOME_FIELD_ID;
            documentWorkflow.ACL_ID = documentWorkflowModel.ACL_ID;
            documentWorkflow.DESCRIPTION = documentWorkflowModel.DESCRIPTION;
            documentWorkflow.ALLOW_MULTIPLE_ENTRIES = documentWorkflowModel.ALLOW_MULTIPLE_ENTRIES;
            documentWorkflow.IS_SHOW_IN_MENU = documentWorkflowModel.IS_SHOW_IN_MENU;
            documentWorkflow.SECTION_ID = documentWorkflowModel.SECTION_ID;
            workFlowId = _repWorkFlow.SaveWorkFlow(documentWorkflow);
            if(documentWorkflowModel.WORKFLOW_ID == 0) {
            
           
            var parentFolderId = 0;
            var folderName = "WorkFlow Document";
           parentFolderId = _srvNode.GetFolderIdByParentIdAndFolderName(8, folderName);
            if (parentFolderId == 0)
            {
                var objECMFolders = new ECMFolder()
                {
                    FOLDER_ID = 0,
                    PARENT_ID = 8,
                    FOLDER_NAME = folderName,
                    DESCRIPTION = null,
                    ACL_ID = documentWorkflowModel.ACL_ID,
                    ENTITY_ID = null,
                    ENTITY_TYPE = 23,

                };
                parentFolderId = _srvNode.SaveFolder(objECMFolders, userId, false, false);
            }
           

            var objECMFolder = new ECMFolder()
            {
                FOLDER_ID = 0,
                PARENT_ID = parentFolderId,
                FOLDER_NAME = documentWorkflowModel.WORKFLOW_TITLE,
                DESCRIPTION = null,
                ACL_ID = documentWorkflowModel.ACL_ID,
                ENTITY_ID = workFlowId,
                ENTITY_TYPE = 23,

            };
            var nodeId = _srvNode.SaveFolder(objECMFolder, userId, false, false);
            }
            return workFlowId;
        }

        /// <summary>
        /// Gets the work flow.
        /// </summary>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <returns></returns>
        public WorkFlowDetailsViewModel GetWorkFlow(int workFlowId)
        {
            var documentWorkflowModel = new WorkFlowDetailsViewModel();
            var documentWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
            if (documentWorkflow != null)
            {
                var documentWorkflowStepsModel = new List<DocumentWorkflowStepsModel>();
                var documentWorkflowApprovalModel = new List<DocumentWorkflowApprovalModel>();
                documentWorkflowModel.WORKFLOW_ID = documentWorkflow.WORKFLOW_DEFINITION_ID;
                documentWorkflowModel.WORKFLOW_TITLE = documentWorkflow.WORKFLOW_DEFINITION_TITLE;
                documentWorkflowModel.SHORT_TITLE = documentWorkflow.SHORT_TITLE;
                documentWorkflowModel.CUSTOME_FIELD_ID = documentWorkflow.CUSTOME_FIELD_ID;
                documentWorkflowModel.ACL_ID = documentWorkflow.ACL_ID;
                documentWorkflowModel.DESCRIPTION = documentWorkflow.DESCRIPTION;
                documentWorkflowModel.IS_BOOKKEEPING_CLIENT_SPECIFIC = documentWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC;
                documentWorkflowModel.APPROVAL_CHANGES_TYPE = documentWorkflow.APPROVAL_CHANGES_TYPE;
                documentWorkflowModel.WORK_FLOW_LEVEL = documentWorkflow.WORK_FLOW_LEVEL;
                documentWorkflowModel.ALLOW_MULTIPLE_ENTRIES = documentWorkflow.ALLOW_MULTIPLE_ENTRIES;
                documentWorkflowModel.IS_SHOW_IN_MENU = documentWorkflow.IS_SHOW_IN_MENU;
                documentWorkflowModel.SECTION_ID = documentWorkflow.SECTION_ID;
                documentWorkflowModel.ACTIVE_FLAG = documentWorkflow.ACTIVE_FLAG;

                var documentWorkflowSteps = _repWorkFlow.GetDocumentWorkFlowStep(workFlowId).ToList();
                foreach (var documentWorkflowStep in documentWorkflowSteps)
                {
                    var folderId = documentWorkflowStep.FOLDER_ID;
                    documentWorkflowStepsModel.Add(new DocumentWorkflowStepsModel()
                    {
                        ORDER_ID = documentWorkflowStep.ORDER_ID,
                        FOLDER_ID = documentWorkflowStep.FOLDER_ID,
                        FOLDER_PATH = folderId.HasValue ? _srvNode.GetFolderPath(folderId.Value) : "",
                        MOVE_ID = documentWorkflowStep.MOVE_ID,
                        STEP_ID = documentWorkflowStep.STEP_ID,
                    });
                }
                documentWorkflowModel.DocumentWorkflowSteps = documentWorkflowStepsModel;
                var documentWorkflowApprovals = _repWorkFlow.GetDocumentWorkFlowApproval(workFlowId).ToList();
                foreach (var documentWorkflowApproval in documentWorkflowApprovals)
                {
                    documentWorkflowApprovalModel.Add(new DocumentWorkflowApprovalModel()
                    {
                        APPROVAL_LEVEL = documentWorkflowApproval.APPROVAL_LEVEL,
                        USER_OR_ROLE = documentWorkflowApproval.USER_OR_ROLE,
                        IS_USER_ROLE = documentWorkflowApproval.IS_USER_ROLE,
                    });
                }
                documentWorkflowModel.DocumentWorkflowApproval = documentWorkflowApprovalModel;
                var documentWorkflowEscalation = _repWorkFlow.GetDocumentWorkFlowEscalation(workFlowId);
                if (documentWorkflowEscalation != null)
                {
                    var documentWorkflowEscalationModel = new DocumentWorkflowEscalationModel();
                    documentWorkflowEscalationModel.APPROVAL_EXPIRE_DAYS = documentWorkflowEscalation.APPROVAL_EXPIRE_DAYS;
                    documentWorkflowEscalationModel.APPROVAL_EXPIRE_HOURS = documentWorkflowEscalation.APPROVAL_EXPIRE_HOURS;
                    documentWorkflowEscalationModel.APPROVAL_EXPIRE_MINUTES = documentWorkflowEscalation.APPROVAL_EXPIRE_MINUTES;
                    documentWorkflowEscalationModel.IS_REMINDER = documentWorkflowEscalation.IS_REMINDER;
                    documentWorkflowEscalationModel.REMINDER_MINUTES = documentWorkflowEscalation.REMINDER_MINUTES;
                    documentWorkflowEscalationModel.ESCALATION_TYPE_ID = documentWorkflowEscalation.ESCALATION_TYPE_ID;
                    documentWorkflowEscalationModel.ESCALATION_USER_ID = documentWorkflowEscalation.ESCALATION_USER_ID;
                    documentWorkflowModel.DocumentWorkflowEscalation = documentWorkflowEscalationModel;
                }
            }
            return documentWorkflowModel;
        }

        /// <summary>
        /// Gets the work flow short tile.
        /// </summary>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <returns></returns>
        public string GetWorkFlowShortTile(int workFlowId)
        {
            string shortTitle = "";
            var documentWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
            if (documentWorkflow != null)
            { shortTitle = documentWorkflow.SHORT_TITLE; }
            return shortTitle;
        }

        /// <summary>
        /// Saves the document work flow step.
        /// </summary>
        /// <param name="documentWorkflowModel">The document workflow model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        public void SaveDocumentWorkFlowStep(WorkFlowDetailsViewModel documentWorkflowModel, int userId,
            ref string errorMessage)
        {
            var workFlowId = documentWorkflowModel.WORKFLOW_ID;
            var documentWorkflowStepsOld = _repWorkFlow.GetDocumentWorkFlowStep(workFlowId);
            var documentWorkflowStepsModel = documentWorkflowModel.DocumentWorkflowSteps;
            if (documentWorkflowStepsOld != null)
            {
                foreach (var documentWorkflowStep in documentWorkflowStepsOld)
                {
                    var stepId = documentWorkflowStep.STEP_ID;
                    var moveId = documentWorkflowStep.MOVE_ID;
                    var folderId = documentWorkflowStep.FOLDER_ID;
                    documentWorkflowStep.USER_MODIFIED = userId;
                    documentWorkflowStep.DATE_MODIFIED = DateTime.UtcNow;
                    documentWorkflowStep.DELETED_FLAG = true;
                    _repWorkFlow.SaveDocumentWorkFlowStep(documentWorkflowStep);
                    if (moveId == (int)WorkflowGloble.MoveType.SpecificFolder)
                    {
                        if (stepId == (int)WorkflowGloble.Step.ToBeProcessed)
                        {
                            if (folderId.HasValue)
                            {
                                var isExistInLogin = _srvAdmin.IsExistDefaultTemplateId(userId, folderId.Value);
                                var isExistInFlowStep = _repWorkFlow.CheckWorkFlowStepFolderIdExist(folderId.Value, workFlowId);
                                if (!isExistInFlowStep && !isExistInLogin)
                                {
                                    var folder = _srvNode.GetFolder(folderId.Value);
                                    folder.HAS_WORKFLOW = null;
                                    folder.DATE_MODIFIED = DateTime.UtcNow;
                                    folder.USER_MODIFIED = userId;
                                    _srvNode.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            if (documentWorkflowStepsModel != null)
            {
                foreach (var documentWorkflowStepModel in documentWorkflowStepsModel)
                {
                    var stepId = documentWorkflowStepModel.STEP_ID;
                    var moveId = documentWorkflowStepModel.MOVE_ID;
                    var folderId = documentWorkflowStepModel.FOLDER_ID;
                    if (moveId == (int)WorkflowGloble.MoveType.SpecificFolder)
                    {
                        if (folderId == 0 || folderId == null)
                        {
                            continue;
                        }
                        else
                        {
                            var refFolderId = _srvNode.GetFolderRefIdByFolderOriginalId(folderId.Value);
                            if (refFolderId.HasValue)
                            {
                                folderId = refFolderId.Value;
                                if (stepId == (int)WorkflowGloble.Step.ToBeProcessed)
                                {
                                    var folder = _srvNode.GetFolder(folderId.Value);
                                    folder.HAS_WORKFLOW = true;
                                    folder.DATE_MODIFIED = DateTime.UtcNow;
                                    folder.USER_MODIFIED = userId;
                                    _srvNode.SaveChanges();
                                }
                            }
                        }
                    }
                    var documentWorkflowStep = new WORKFLOW_DEFINITION_STEPS();
                    documentWorkflowStep.WORKFLOW_DEFINITION_ID = documentWorkflowModel.WORKFLOW_ID;
                    documentWorkflowStep.STEP_ID = documentWorkflowStepModel.STEP_ID;
                    documentWorkflowStep.MOVE_ID = moveId;
                    documentWorkflowStep.FOLDER_ID = folderId;
                    documentWorkflowStep.ORDER_ID = documentWorkflowStepModel.ORDER_ID;
                    documentWorkflowStep.USER_ID = userId;
                    documentWorkflowStep.DATE_OF_ENTRY = DateTime.UtcNow;
                    documentWorkflowStep.USER_MODIFIED = userId;
                    documentWorkflowStep.DATE_MODIFIED = DateTime.UtcNow;
                    documentWorkflowStep.DELETED_FLAG = false;
                    _repWorkFlow.SaveDocumentWorkFlowStep(documentWorkflowStep);
                }
            }
            var documentWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
            if (documentWorkflow != null)
            {
                if (documentWorkflowModel.IS_BOOKKEEPING_CLIENT_SPECIFIC.HasValue)
                {
                    documentWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC = documentWorkflowModel.IS_BOOKKEEPING_CLIENT_SPECIFIC;
                }
                else
                {
                    documentWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC = false;
                }
                documentWorkflow.WORK_FLOW_LEVEL = documentWorkflowModel.WORK_FLOW_LEVEL;
                _repWorkFlow.SaveWorkFlow(documentWorkflow);
            }
            SaveFolderHaseWorkFlow(workFlowId, documentWorkflowStepsOld);
        }

        /// <summary>
        /// Saves the folder hase work flow.
        /// </summary>
        /// <param name="workFlowId">The work flow identifier.</param>
        /// <param name="oldDocumentWorkflowSteps">The old document workflow steps.</param>
        private void SaveFolderHaseWorkFlow(int workFlowId, List<WORKFLOW_DEFINITION_STEPS> oldDocumentWorkflowSteps)
        {
            if (oldDocumentWorkflowSteps != null)
            {
                var toBeProcessedFolder = oldDocumentWorkflowSteps.Where(x => x.MOVE_ID == (int)WorkflowGloble.MoveType.SpecificFolder
                    && x.STEP_ID == (int)WorkflowGloble.Step.ToBeProcessed).FirstOrDefault();
                if (toBeProcessedFolder != null)
                {
                    var folderId = toBeProcessedFolder.FOLDER_ID;
                    if (folderId != 0 && folderId != null)
                    {
                        var folder = _srvNode.GetFolder(folderId.Value);
                        if (folder != null)
                        {
                            folder.HAS_WORKFLOW = false;
                            _srvNode.SaveChanges();
                        }
                    }
                }
            }
            var documentWorkflowSteps = _repWorkFlow.GetDocumentWorkFlowStep(workFlowId);
            if (documentWorkflowSteps != null)
            {
                var toBeProcessedFolder = documentWorkflowSteps.Where(x => x.MOVE_ID == (int)WorkflowGloble.MoveType.SpecificFolder
                    && x.STEP_ID == (int)WorkflowGloble.Step.ToBeProcessed &&  x.DELETED_FLAG == false).FirstOrDefault();
                if (toBeProcessedFolder != null)
                {
                    var folderId = toBeProcessedFolder.FOLDER_ID;
                    if (folderId != 0 && folderId != null)
                    {
                        var folder = _srvNode.GetFolder(folderId.Value);
                        if (folder != null)
                        {
                            folder.HAS_WORKFLOW = true;
                            _srvNode.SaveChanges();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the document work flow approval.
        /// </summary>
        /// <param name="documentWorkflowModel">The document workflow model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        public void SaveDocumentWorkFlowApproval(WorkFlowDetailsViewModel documentWorkflowModel, int userId,
           ref string errorMessage)
        {
            var workFlowId = documentWorkflowModel.WORKFLOW_ID;
            var documentWorkflowApprovals = _repWorkFlow.GetDocumentWorkFlowApproval(workFlowId);
            if (documentWorkflowApprovals != null)
            {
                foreach (var documentWorkflowApproval in documentWorkflowApprovals)
                {
                    documentWorkflowApproval.USER_MODIFIED = userId;
                    documentWorkflowApproval.DATE_MODIFIED = DateTime.UtcNow;
                    documentWorkflowApproval.DELETED_FLAG = true;
                    _repWorkFlow.SaveDocumentWorkFlowApproval(documentWorkflowApproval);
                }
            }
            if (documentWorkflowModel.DocumentWorkflowApproval != null)
            {
                foreach (var documentWorkflowApprovalModel in documentWorkflowModel.DocumentWorkflowApproval)
                {
                    if (documentWorkflowApprovalModel.USER_OR_ROLE == null)
                    {
                        continue;
                    }
                    var documentWorkflowApproval = new WF_DOCUMENT_WORKFLOW_APPROVAL();
                    documentWorkflowApproval.WORKFLOW_ID = documentWorkflowModel.WORKFLOW_ID;
                    documentWorkflowApproval.APPROVAL_LEVEL = documentWorkflowApprovalModel.APPROVAL_LEVEL;
                    documentWorkflowApproval.USER_OR_ROLE = documentWorkflowApprovalModel.USER_OR_ROLE;
                    documentWorkflowApproval.IS_USER_ROLE = documentWorkflowApprovalModel.IS_USER_ROLE;
                    documentWorkflowApproval.USER_ID = userId;
                    documentWorkflowApproval.DATE_OF_ENTRY = DateTime.UtcNow;
                    documentWorkflowApproval.USER_MODIFIED = userId;
                    documentWorkflowApproval.DATE_MODIFIED = DateTime.UtcNow;
                    documentWorkflowApproval.DELETED_FLAG = false;
                    _repWorkFlow.SaveDocumentWorkFlowApproval(documentWorkflowApproval);
                }
            }
            var documentWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
            if (documentWorkflow != null)
            {
                documentWorkflow.APPROVAL_CHANGES_TYPE = documentWorkflowModel.APPROVAL_CHANGES_TYPE;
                _repWorkFlow.SaveWorkFlow(documentWorkflow);
            }
        }

        /// <summary>
        /// Get folder Path
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public string GetFolderPath(int folderId)
        {
            return _srvNode.GetFolderPath(folderId);
        }

        /// <summary>
        /// Saves the document work flow escalation.
        /// </summary>
        /// <param name="documentWorkflowModel">The document workflow model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        public void SaveDocumentWorkFlowEscalation(WorkFlowDetailsViewModel documentWorkflowModel, int userId,
        ref string errorMessage)
        {
            int? reminderMinutes = null;
            var documentWorkflowEscalationModel = documentWorkflowModel.DocumentWorkflowEscalation;
            var workFlowId = documentWorkflowModel.WORKFLOW_ID;
            var isReminder = documentWorkflowEscalationModel.IS_REMINDER;
            var isDeleted = documentWorkflowEscalationModel.IsDeleted;
            var escalationTypeId = documentWorkflowEscalationModel.ESCALATION_TYPE_ID;
            var forwardToUserId = documentWorkflowEscalationModel.ESCALATION_USER_ID;
            if (isReminder.HasValue)
            {
                reminderMinutes = documentWorkflowEscalationModel.REMINDER_MINUTES;
            }
            var documentWorkflowEscalation = _repWorkFlow.GetDocumentWorkFlowEscalationForUpdate(workFlowId);
            if (documentWorkflowEscalation != null)
            {
                if (isDeleted)
                {
                    documentWorkflowEscalation.APPROVAL_EXPIRE_DAYS = null;
                    documentWorkflowEscalation.APPROVAL_EXPIRE_HOURS = null;
                    documentWorkflowEscalation.APPROVAL_EXPIRE_MINUTES = null;
                    documentWorkflowEscalation.IS_REMINDER = null;
                    documentWorkflowEscalation.REMINDER_MINUTES = null;
                    documentWorkflowEscalation.ESCALATION_TYPE_ID = null;
                    documentWorkflowEscalation.ESCALATION_USER_ID = null;
                    documentWorkflowEscalation.USER_MODIFIED = userId;
                    documentWorkflowEscalation.DATE_MODIFIED = DateTime.UtcNow;
                    documentWorkflowEscalation.DELETED_FLAG = true;
                    _repWorkFlow.SaveDocumentWorkFlowEscalation(documentWorkflowEscalation);
                }
                else
                {
                    documentWorkflowEscalation.APPROVAL_EXPIRE_DAYS = documentWorkflowEscalationModel.APPROVAL_EXPIRE_DAYS;
                    documentWorkflowEscalation.APPROVAL_EXPIRE_HOURS = documentWorkflowEscalationModel.APPROVAL_EXPIRE_HOURS;
                    documentWorkflowEscalation.APPROVAL_EXPIRE_MINUTES = documentWorkflowEscalationModel.APPROVAL_EXPIRE_MINUTES;
                    documentWorkflowEscalation.IS_REMINDER = isReminder;
                    documentWorkflowEscalation.REMINDER_MINUTES = (byte?)reminderMinutes;
                    if (escalationTypeId == (byte)WorkflowGloble.EscalationType.ForwardTo)
                    {
                        documentWorkflowEscalation.ESCALATION_USER_ID = forwardToUserId;
                    }
                    else
                    {
                        documentWorkflowEscalation.ESCALATION_USER_ID = null;
                    }
                    documentWorkflowEscalation.ESCALATION_TYPE_ID = escalationTypeId;
                    documentWorkflowEscalation.USER_MODIFIED = userId;
                    documentWorkflowEscalation.DATE_MODIFIED = DateTime.UtcNow;
                    documentWorkflowEscalation.DELETED_FLAG = false;
                    _repWorkFlow.SaveDocumentWorkFlowEscalation(documentWorkflowEscalation);
                }
            }
            else
            {
                documentWorkflowEscalation = new WORKFLOW_DEFINITION_ESCALATION();
                documentWorkflowEscalation.WORKFLOW_DEFINITION_ID = documentWorkflowModel.WORKFLOW_ID;
                documentWorkflowEscalation.APPROVAL_EXPIRE_DAYS = documentWorkflowEscalationModel.APPROVAL_EXPIRE_DAYS;
                documentWorkflowEscalation.APPROVAL_EXPIRE_HOURS = documentWorkflowEscalationModel.APPROVAL_EXPIRE_HOURS;
                documentWorkflowEscalation.APPROVAL_EXPIRE_MINUTES = documentWorkflowEscalationModel.APPROVAL_EXPIRE_MINUTES;
                documentWorkflowEscalation.IS_REMINDER = documentWorkflowEscalationModel.IS_REMINDER;
                documentWorkflowEscalation.REMINDER_MINUTES = (byte?)reminderMinutes;
                documentWorkflowEscalation.ESCALATION_TYPE_ID = documentWorkflowEscalationModel.ESCALATION_TYPE_ID;
                if (escalationTypeId == (byte)WorkflowGloble.EscalationType.ForwardTo)
                {
                    documentWorkflowEscalation.ESCALATION_USER_ID = forwardToUserId;
                }
                else
                {
                    documentWorkflowEscalation.ESCALATION_USER_ID = null;
                }
                documentWorkflowEscalation.USER_ID = userId;
                documentWorkflowEscalation.DATE_OF_ENTRY = DateTime.UtcNow;
                documentWorkflowEscalation.USER_MODIFIED = userId;
                documentWorkflowEscalation.DATE_MODIFIED = DateTime.UtcNow;
                documentWorkflowEscalation.DELETED_FLAG = false;
                _repWorkFlow.SaveDocumentWorkFlowEscalation(documentWorkflowEscalation);
            }
        }

        public List<int> GetAssocitePages(DocumentAssociation documentAssociation)
        {
            int docId = documentAssociation.DocId;
            int workFlowId = documentAssociation.WorkFlowId;
            var associatedPages = _repWorkFlow.GetAssocitePages(docId, workFlowId);
            return associatedPages;
        }
        public WFCommonFields GetWorkFlowCommonFields(int workFlowId)
        {
            var commonFieldModel = new WFCommonFields();
            var commonField = _repWorkFlow.GetWorkFlowCommonFields(workFlowId);
            if (commonField != null)
            {
                commonFieldModel = new WFCommonFields()
                {
                    WORKFLOW_ID = commonField.WORKFLOW_ID,
                    WORKFLOW_DATE_NAME = commonField.WORKFLOW_DATE_NAME,
                    WORKFLOW_TITLE = commonField.WORKFLOW_TITLE,
                };
            }
            return commonFieldModel;
        }

        public bool IsDocumentApproved(int workFlowId, int pageNo, int docId)
        {
            var isApproved = false;
            var objDocument = _repWorkFlow.GetDocumentByDocIdAndPageNo(docId, pageNo, workFlowId);
            if (objDocument != null)
            {
                var entityId = objDocument.WORKFLOW_ID;
                isApproved = _repWorkFlow.IsDocumentApproved(workFlowId, entityId.Value);
            }
            return isApproved;
        }

        #endregion


        public int? GetWorkFlowCustomFieldTypeId(int workFlowId)
        {
            int? customeFieldId = null;
            var documentWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
            if (documentWorkflow != null)
            { customeFieldId = documentWorkflow.CUSTOME_FIELD_ID; }
            return customeFieldId;

        }

        public int GetWorkflowInitiator(int workFlowId, int entityId)
        {
            return _repWorkFlow.GetWorkflowInitiator(workFlowId, entityId);
        }

        public string GetWorkFlowFullTile(int workFlowId)
        {
            string fullTitle = "";
            var documentWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
            if (documentWorkflow != null)
            { fullTitle = documentWorkflow.WORKFLOW_DEFINITION_TITLE; }
            return fullTitle;
        }


        public int GetDocIdByEntityId(int entityId, int workFlowId)
        {
            return _repWorkFlow.GetDocIdByEntityId(entityId, workFlowId);
        }

        public int GetNewDocIdByEntityId(int entityId, int workFlowId)
        {
            return _repWorkFlow.GetNewDocIdByEntityId(entityId, workFlowId);
        }

        public void DeleteWorkflow(int entityId, int workFlowId, int userId, UserType userType, ref string errorMessage)
        {
            var toBeProcessedFolderId = 0;
            if (workFlowId == (int)WorkflowGloble.WorkFlowType.VenderBill)
            {
                var workflowVenderBill = _repWorkFlow.GetBillByBillId(entityId);
                if (workflowVenderBill == null)
                {
                    errorMessage = "UnAuthorized";
                    return;
                }
                var projectId = _srvPms.GetProjectIdByClientId(workflowVenderBill.CLIENT_ID.Value);
                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed,
                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                if (workflowVenderBill != null)
                {
                    workflowVenderBill.DELETED_FLAG = true;
                    workflowVenderBill.USER_MODIFIED = userId;
                    workflowVenderBill.DATE_MODIFIED = DateTime.UtcNow;
                }
            }

            else if (workFlowId == (int)WorkflowGloble.WorkFlowType.VenderCredit)
            {
                var workflowVenderCredit = _repWorkFlow.GetVenderCreditByCreditId(entityId);
                if (workflowVenderCredit == null)
                {
                    errorMessage = "UnAuthorized";
                    return;
                }
                var projectId = _srvPms.GetProjectIdByClientId(workflowVenderCredit.CLIENT_ID.Value);
                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed,
                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                if (workflowVenderCredit != null)
                {
                    workflowVenderCredit.DELETED_FLAG = true;
                    workflowVenderCredit.USER_MODIFIED = userId;
                    workflowVenderCredit.DATE_MODIFIED = DateTime.UtcNow;
                }
            }

            else if (workFlowId == (int)WorkflowGloble.WorkFlowType.DSReport)
            {
                var workflowDSReport = _repWorkFlow.GetDailySalesReport(entityId);
                if (workflowDSReport == null)
                {
                    errorMessage = "UnAuthorized";
                    return;
                }
                var projectId = _srvPms.GetProjectIdByClientId(workflowDSReport.CLIENT_ID);
                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed,
                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                if (workflowDSReport != null)
                {
                    workflowDSReport.DELETED_FLAG = true;
                    workflowDSReport.USER_MODIFIED = userId;
                    workflowDSReport.DATE_MODIFIED = DateTime.UtcNow;
                }

            }

            else if (workFlowId == (int)WorkflowGloble.WorkFlowType.Payroll)
            {
                var workflowPayroll = _repWorkFlow.GetPayroll(entityId);
                if (workflowPayroll == null)
                {
                    errorMessage = "UnAuthorized";
                    return;
                }
                var projectId = _srvPms.GetProjectIdByClientId(workflowPayroll.CLIENT_ID);
                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed,
                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                if (workflowPayroll != null)
                {
                    workflowPayroll.DELETED_FLAG = true;
                    workflowPayroll.USER_MODIFIED = userId;
                    workflowPayroll.DATE_MODIFIED = DateTime.UtcNow;
                }
            }
            else if (workFlowId == (int)WorkflowGloble.WorkFlowType.PayrollReport)
            {
                var workflowPayrollReport = _repWorkFlow.GetPayrollReport(entityId);
                if (workflowPayrollReport == null)
                {
                    errorMessage = "UnAuthorized";
                    return;
                }
                var projectId = _srvPms.GetProjectIdByClientId(workflowPayrollReport.CLIENT_ID);
                toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed,
                projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                if (workflowPayrollReport != null)
                {
                    workflowPayrollReport.DELETED_FLAG = true;
                    workflowPayrollReport.USER_MODIFIED = userId;
                    workflowPayrollReport.DATE_MODIFIED = DateTime.UtcNow;
                }
            }
            else
            {
                var customWorkFlow = _repWorkFlow.GetCustomWorkFlow(entityId);
                if (customWorkFlow != null)
                {
                    customWorkFlow.DELETED_FLAG = true;
                    customWorkFlow.USER_MODIFIED = userId;
                    customWorkFlow.DATE_MODIFIED = DateTime.UtcNow;
                }
                var clientId = customWorkFlow.CLIENT_ID;
                var projectId = 0;
                if (clientId.HasValue)
                {
                    projectId = _srvPms.GetProjectIdByClientId(clientId.Value);
                    toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed,
                    projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                }
                else
                {
                    var documentProcessingWorkflow = _repWorkFlow.GetDocumentWorkFlow(workFlowId);
                    var isBookkeepingClientSpecific = true;
                    if (documentProcessingWorkflow != null)
                    {
                        if (documentProcessingWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC.HasValue)
                        {
                            isBookkeepingClientSpecific = documentProcessingWorkflow.IS_BOOKKEEPING_CLIENT_SPECIFIC.Value;
                        }
                    }
                    if (isBookkeepingClientSpecific)
                    {
                        if (clientId.HasValue)
                        {
                            projectId = _srvPms.GetProjectIdByClientId(clientId.Value);
                            toBeProcessedFolderId = _srvNode.GetFolderIdByEntityIdAndFolderName(ECMGlobal.ToBeProcessed,
                            projectId, (int)ECMGlobal.ECMLibraryId.ContentStructure);
                        }
                    }
                    else
                    {
                        var steps = documentProcessingWorkflow.WORKFLOW_DEFINITION_STEPS.Where(x =>  x.DELETED_FLAG == false);
                        if (steps != null)
                        {
                            var step = steps.Where(x => x.STEP_ID == (int)WorkflowGloble.Step.ToBeProcessed).FirstOrDefault();
                            if (step != null)
                            {
                                if (step.FOLDER_ID.HasValue)
                                {
                                    toBeProcessedFolderId = step.FOLDER_ID.Value;
                                }
                            }
                        }
                    }
                }
            }

            var documents = _repWorkFlow.GetWorkFlowDocument(entityId, workFlowId);
            if (documents.Count() > 0)
            {
                var newDocIds = documents.Select(x => x.NEW_DOC_ID).Distinct();
                var originalIds = documents.Select(x => x.DOC_ID).Distinct();
                if (originalIds != null)
                {
                    foreach (var originalId in originalIds)
                    {
                        var originelDocNoOfPages = _srvNode.GetOriginelDocumentTotalPages(originalId);
                        var noOfPages = documents.Where(x => x.DOC_ID == originalId).Select(x => x.DOC_ID).Count();

                        var workFlowDocuments = documents.Where(x => x.DOC_ID == originalId).ToList();
                        foreach (var workFlowDocument in workFlowDocuments)
                        {
                            workFlowDocument.DELETED_FLAG = true;
                            workFlowDocument.USER_MODIFIED = userId;
                            workFlowDocument.DATE_MODIFIED = DateTime.UtcNow;
                        }

                        if (originelDocNoOfPages > noOfPages)
                        {
                            var newDocId = workFlowDocuments.Select(x => x.NEW_DOC_ID).FirstOrDefault();
                            _srvNode.DeletedDocument(newDocId, userId);
                        }
                        else if (originelDocNoOfPages != 0)
                        {
                            if (toBeProcessedFolderId != 0)
                            {
                                foreach (var newDocId in newDocIds)
                                {
                                    var documentOperation = new DocumentOperation();
                                    var oldDocument = _srvNode.GetDocument(newDocId);
                                    if (oldDocument != null)
                                    {
                                        documentOperation.DestinationFolderId = toBeProcessedFolderId;
                                        documentOperation.SourceFolderId = oldDocument.FOLDER_ID;
                                        List<int> docIds = new List<int>();
                                        docIds.Add(newDocId);
                                        documentOperation.DocIds = docIds.ToArray();
                                        documentOperation.isByPass = true;
                                        _srvNode.CutDocuments(documentOperation, userId, attachmentFolder, userType, true, ref errorMessage);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (toBeProcessedFolderId != 0)
                            {
                                foreach (var newDocId in newDocIds)
                                {
                                    var documentOperation = new DocumentOperation();
                                    var newDocument = _srvNode.GetDocument(newDocId);
                                    if (newDocument != null)
                                    {
                                        documentOperation.DestinationFolderId = toBeProcessedFolderId;
                                        documentOperation.SourceFolderId = newDocument.FOLDER_ID;
                                        List<int> docIds = new List<int>();
                                        docIds.Add(newDocId);
                                        documentOperation.DocIds = docIds.ToArray();
                                        documentOperation.isByPass = true;
                                        _srvNode.CutDocuments(documentOperation, userId, attachmentFolder, userType, true, ref errorMessage);

                                    }
                                }
                            }

                        }
                        if (originelDocNoOfPages != 0)
                        {
                            var oldbjTobeProcesses = _repWorkFlow.IsExistDocumentInProcessedWidget(originalId);
                            if (oldbjTobeProcesses == null)
                            {
                                oldbjTobeProcesses = _repWorkFlow.GetToBeProcessedDeletedDocumentByDocId(originalId);
                                if (oldbjTobeProcesses != null)
                                {
                                    oldbjTobeProcesses.USER_MODIFIED = userId;
                                    oldbjTobeProcesses.DATE_MODIFIED = DateTime.UtcNow;
                                    oldbjTobeProcesses.DELETED_FLAG = false;
                                }
                            }
                            else
                            {
                                oldbjTobeProcesses.USER_MODIFIED = userId;
                                oldbjTobeProcesses.DATE_MODIFIED = DateTime.UtcNow;
                                oldbjTobeProcesses.DELETED_FLAG = false;
                            }
                        }
                    }
                }
            }
            _repWorkFlow.SaveChanges();
        }

        public WORKFLOW_CUSTOM WorkflowOwnerDisableSection (int workflowId)
        {
            var customWorkFlow = _repWorkFlow.GetCustomWorkFlow(workflowId);
            return customWorkFlow;
        }


        public void ReOrderSubItems(int destinationOrderId, int sourceOrderId, int userId, int userType, int listId)
        {
            var subItems = _repWorkFlow.GetSubItemsByListId(listId);
            var sourceField = subItems.Where(x => x.REVIEWER_ID == sourceOrderId).FirstOrDefault();
            var sourceFieldOrderId = sourceField.ORDER_ID;
            var destinationField = subItems.Where(x => x.REVIEWER_ID == destinationOrderId).FirstOrDefault();
            var destinationFieldOrderId = destinationField.ORDER_ID;
            if (sourceFieldOrderId == destinationFieldOrderId)
            {
                return;
            }
            if (sourceFieldOrderId > destinationFieldOrderId)
            {
                var destinationFieldOrders = subItems.Where(x => x.ORDER_ID >= destinationFieldOrderId && x.ORDER_ID < sourceFieldOrderId);
                foreach (var destinationFieldOrder in destinationFieldOrders)
                {
                    destinationFieldOrder.ORDER_ID += 1;
                    destinationFieldOrder.USER_MODIFIED = userId;
                    destinationFieldOrder.DATE_MODIFIED = DateTime.UtcNow;
                }
                sourceField.ORDER_ID = destinationFieldOrderId;
                sourceField.USER_MODIFIED = userId;
                sourceField.DATE_OF_ENTRY = DateTime.UtcNow;
            }
            else if (sourceFieldOrderId < destinationFieldOrderId)
            {
                var sourceFieldOrders = subItems.Where(x => x.ORDER_ID > sourceFieldOrderId && x.ORDER_ID <= destinationFieldOrderId);
                foreach (var sourceFieldOrder in sourceFieldOrders)
                {
                    sourceFieldOrder.ORDER_ID -= 1;
                    sourceFieldOrder.USER_MODIFIED = userId;
                    sourceFieldOrder.DATE_MODIFIED = DateTime.UtcNow;
                }
                sourceField.ORDER_ID = destinationFieldOrderId;
                sourceField.USER_MODIFIED = userId;
                sourceField.DATE_OF_ENTRY = DateTime.UtcNow;
            }
            _repWorkFlow.SaveChanges();
        }
    }
}
