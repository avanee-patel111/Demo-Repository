using SOL.Addressbook.Interfaces;
using SOL.Common.Business.Events.ECM;
using SOL.Common.Business.Interfaces;
using SOL.Common.Business.Models;
using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mail;

namespace SOL.WorkFlow.Services
{
    public class WorkflowMessageService<T> : IWorkflowMessageService<T>
    {
        private IWorkflowMessageRepository<T> _repWorkflowMessage;
        IWorkFlowRepository<T> _repWorkFlow;
        IBaseRepository<T> _repBase;
        IAddressbookService<T> _srvAdrBk;
        IVendorCreditRepository<T> _repWorkflowVendorCredit;
        IBaseService _srvBase = null;


        public WorkflowMessageService(IWorkflowMessageRepository<T> repWorkflowMessage, IBaseRepository<T> baseRep, IVendorCreditRepository<T> repWorkflowVendorCredit,
              IWorkFlowRepository<T> repWorkFlow, IAddressbookService<T> srvAdrBk, IBaseService srvBase)
        {
            this._repWorkFlow = repWorkFlow;
            this._repWorkflowMessage = repWorkflowMessage;
            this._repBase = baseRep;
            this._srvAdrBk = srvAdrBk;
            this._repWorkflowVendorCredit = repWorkflowVendorCredit;
            this._srvBase = srvBase;
        }

        /// <summary>
        /// Saves the work flow message.
        /// </summary>
        /// <param name="workflowMessage">The workflow message.</param>
        public void SaveWorkFlowMessage(WF_WORKFLOW_MESSAGES workflowMessage, int userId, string companyLogo)
        {
          
            if (workflowMessage.MESSAGE_ID == default(int))
            {
                workflowMessage.DELETED_FLAG = false;
                workflowMessage.DATE_MODIFIED = DateTime.UtcNow;
                workflowMessage.DATE_OF_ENTRY = DateTime.UtcNow;
                workflowMessage.USER_ID = userId;
                workflowMessage.USER_MODIFIED = userId;
                _repWorkflowMessage.SaveWorkFlowMessage(workflowMessage);
            }
            else
            {
                var orignalWorkFlowMessage = _repWorkflowMessage.GetWorkflowMessage(workflowMessage.MESSAGE_ID);
                orignalWorkFlowMessage.USER_MODIFIED = userId;
                orignalWorkFlowMessage.DATE_MODIFIED = DateTime.UtcNow;
                _repWorkflowMessage.SaveWorkFlowMessage(orignalWorkFlowMessage);
            }

            if (workflowMessage.IsShareWithOthers == true && workflowMessage.CommunicationShare != null)
            {
                SaveWorkFlowMessageReciepients(workflowMessage.CommunicationShare, userId, workflowMessage.MESSAGE_ID);
            }
            SendMessageAddOnDocumentNotification(workflowMessage, userId, companyLogo);
        }
        /// <summary>
        /// Saves the work flow message comment.
        /// </summary>
        /// <param name="workflowMessageComment">The workflow message comment.</param>
        public void SaveWorkFlowMessageComment(workFlowCommentMsgInfo workflowMessageComment, int userId, string companyLogo)
        {
            var workflowMessageCommentInfo = workflowMessageComment.workflowMessageComment;
            if (workflowMessageCommentInfo.COMMENT_ID == default(int))
            {
                workflowMessageCommentInfo.DELETED_FLAG = false;
                workflowMessageCommentInfo.DATE_MODIFIED = DateTime.UtcNow;
                workflowMessageCommentInfo.DATE_OF_ENTRY = DateTime.UtcNow;
                workflowMessageCommentInfo.USER_ID = userId;
                workflowMessageCommentInfo.USER_MODIFIED = userId;
                _repWorkflowMessage.SaveWorkFlowMessageComment(workflowMessageComment.workflowMessageComment);
            }
            else
            {
                var orignalWorkFlowMessage = _repWorkflowMessage.GetWorkflowMessageComment(workflowMessageCommentInfo.COMMENT_ID);
                orignalWorkFlowMessage.USER_MODIFIED = userId;
                orignalWorkFlowMessage.DATE_MODIFIED = DateTime.UtcNow;
                _repWorkflowMessage.SaveWorkFlowMessageComment(workflowMessageComment.workflowMessageComment);
            }
            SendCommentAddOnDocumentMessageNotification(workflowMessageCommentInfo.COMMENT, workflowMessageCommentInfo.MESSAGE_ID, userId,
                workflowMessageComment.ProcessId, workflowMessageComment.WorkflowId, companyLogo);
            //SendCommentAddOnDocumentMessageNotification(workflowMessageCommentInfo.COMMENT, workflowMessageCommentInfo.MESSAGE_ID, userId);
        }
        /// <summary>
        /// Saves the work flow message comment.
        /// </summary>
        /// <param name="workflowMessageRecipients">The workflow message recipients.</param>
        public void SaveWorkFlowMessageRecipients(WF_WORKFLOW_MSGBOARD_RECIPIENTS workflowMessageRecipient, int userId)
        {
            var workflowMessageRecipients = new List<WF_WORKFLOW_MSGBOARD_RECIPIENTS>();
            if (workflowMessageRecipient.MESSAGE_SHARING_RECIPIENT_ID == default(int))
            {
                workflowMessageRecipient.DELETED_FLAG = false;
                workflowMessageRecipient.DATE_MODIFIED = DateTime.UtcNow;
                workflowMessageRecipient.DATE_OF_ENTRY = DateTime.UtcNow;
                workflowMessageRecipient.USER_ID = userId;
                workflowMessageRecipient.USER_MODIFIED = userId;
                workflowMessageRecipients.Add(workflowMessageRecipient);
            }
            else
            {
                var orignalWorkFlowRecipients = _repWorkflowMessage.GetWorkflowRecipient(workflowMessageRecipient.MESSAGE_SHARING_RECIPIENT_ID);
                workflowMessageRecipient.USER_MODIFIED = userId;
                workflowMessageRecipient.DATE_MODIFIED = DateTime.UtcNow;
                workflowMessageRecipients.Add(workflowMessageRecipient);
            }
            _repWorkflowMessage.SaveWorkFlowMessageRecipients(workflowMessageRecipients);


        }

        private void SaveWorkFlowMessageReciepients(CommunicationShare objShare, int userId, int messageId)
        {

            var entryDate = DateTime.UtcNow;
            var objSharingRecipients = new List<WF_WORKFLOW_MSGBOARD_RECIPIENTS>();
            if (objShare.ToEmails != null)
            {
                foreach (var toId in objShare.ToEmails)
                {
                    var startPos = toId.Email.IndexOf('<');
                    string filteredToEmailId = string.Empty;
                    string contactName = string.Empty;
                    if (startPos == -1)
                    {
                        filteredToEmailId = toId.Email;
                    }
                    else
                    {
                        contactName = toId.Email.Substring(0, startPos).ToString();
                        filteredToEmailId = toId.Email.Substring(startPos, toId.Email.Length - startPos).Replace("<", string.Empty);
                        filteredToEmailId = filteredToEmailId.Replace(">", string.Empty);
                    }

                    var sharingRecipient = new WF_WORKFLOW_MSGBOARD_RECIPIENTS()
                    {
                        MESSAGE_ID = messageId,
                        RECIPIENT_EMAIL = filteredToEmailId.ToLower(),
                        IS_TO = true,
                        IS_BCC = false,
                        IS_CC = false,
                        DATE_OF_ENTRY = entryDate,
                        DATE_MODIFIED = entryDate,
                        USER_ID = userId,
                        USER_MODIFIED = userId,
                        DELETED_FLAG = false,
                        CONTACT_NAME = contactName,
                        CONTACT_ID = toId.Id
                    };
                    objSharingRecipients.Add(sharingRecipient);

                }
            }
            if (objShare.CcEmails != null)
            {
                foreach (var emailCc in objShare.CcEmails)
                {
                    var startPos = emailCc.Email.IndexOf('<');
                    string filteredToEmailId = string.Empty;
                    string contactName = string.Empty;
                    if (startPos == -1)
                    {
                        filteredToEmailId = emailCc.Email;
                    }
                    else
                    {
                        contactName = emailCc.Email.Substring(0, startPos).ToString();
                        filteredToEmailId = emailCc.Email.Substring(startPos, emailCc.Email.Length - startPos).Replace("<", string.Empty);
                        filteredToEmailId = filteredToEmailId.Replace(">", string.Empty);
                    }
                    var sharingRecipient = objSharingRecipients.Where(x => x.RECIPIENT_EMAIL == filteredToEmailId).FirstOrDefault();
                    if (sharingRecipient == null)
                    {
                        sharingRecipient = new WF_WORKFLOW_MSGBOARD_RECIPIENTS()
                        {
                            MESSAGE_ID = messageId,
                            RECIPIENT_EMAIL = filteredToEmailId.ToLower(),
                            IS_TO = false,
                            IS_BCC = false,
                            IS_CC = true,
                            DATE_OF_ENTRY = entryDate,
                            DATE_MODIFIED = entryDate,
                            USER_ID = userId,
                            USER_MODIFIED = userId,
                            DELETED_FLAG = false,
                            CONTACT_NAME = contactName,
                            CONTACT_ID = emailCc.Id
                        };
                        objSharingRecipients.Add(sharingRecipient);

                    }
                    else
                    {
                        sharingRecipient.IS_CC = true;
                    }

                }
            }
            if (objShare.BccEmails != null)
            {
                foreach (var bccemail in objShare.BccEmails)
                {
                    var startPos = bccemail.Email.IndexOf('<');
                    string filteredToEmailId = string.Empty;
                    string contactName = string.Empty;
                    if (startPos == -1)
                    {
                        filteredToEmailId = bccemail.Email;
                    }
                    else
                    {
                        contactName = bccemail.Email.Substring(0, startPos).ToString();
                        filteredToEmailId = bccemail.Email.Substring(startPos, bccemail.Email.Length - startPos).Replace("<", string.Empty);
                        filteredToEmailId = filteredToEmailId.Replace(">", string.Empty);
                    }
                    var sharingRecipient = objSharingRecipients.Where(x => x.RECIPIENT_EMAIL == filteredToEmailId).FirstOrDefault();
                    if (sharingRecipient == null)
                    {
                        sharingRecipient = new WF_WORKFLOW_MSGBOARD_RECIPIENTS()
                        {
                            MESSAGE_ID = messageId,
                            RECIPIENT_EMAIL = filteredToEmailId.ToLower(),
                            IS_TO = false,
                            IS_BCC = true,
                            IS_CC = false,
                            DATE_OF_ENTRY = entryDate,
                            DATE_MODIFIED = entryDate,
                            USER_ID = userId,
                            USER_MODIFIED = userId,
                            DELETED_FLAG = false,
                            CONTACT_NAME = contactName,
                            CONTACT_ID = bccemail.Id
                        };

                        objSharingRecipients.Add(sharingRecipient);


                    }
                    else
                    {
                        sharingRecipient.IS_BCC = true;
                    }

                }
            }

            var existingReceipients = GetWorkflowMessageRecipients(messageId);
            var existingEmails = existingReceipients.Select(x => x.RECIPIENT_EMAIL).ToList();
            var newEmailIds = objSharingRecipients.Select(x => x.RECIPIENT_EMAIL).ToList();

            var emailsToRemove = existingEmails.Except(newEmailIds);

            var emailsToAdd = newEmailIds.Except(existingEmails);

            var emailsToUpdate = newEmailIds.Intersect(existingEmails);
            foreach (var email in emailsToUpdate)
            {
                var recipient = existingReceipients.FirstOrDefault(x => x.RECIPIENT_EMAIL == email);
                var addedRecipient = objSharingRecipients.FirstOrDefault(x => x.RECIPIENT_EMAIL == email);
                if (recipient != null && addedRecipient != null)
                {
                    recipient.DELETED_FLAG = false;
                    recipient.CONTACT_NAME = addedRecipient.CONTACT_NAME;
                    recipient.IS_BCC = addedRecipient.IS_BCC;
                    recipient.IS_CC = addedRecipient.IS_CC;
                    recipient.IS_TO = addedRecipient.IS_TO;
                    recipient.USER_MODIFIED = userId;
                    recipient.DATE_MODIFIED = entryDate;
                }
            }

            foreach (var emailToAdd in emailsToAdd)
            {
                var addRecipients = objSharingRecipients.FirstOrDefault(x => x.RECIPIENT_EMAIL == emailToAdd);
                if (addRecipients != null)
                {
                    existingReceipients.Add(addRecipients);
                }

            }
            foreach (var emailToRemove in emailsToRemove)
            {
                var removeRecipients = existingReceipients.FirstOrDefault(x => x.RECIPIENT_EMAIL == emailToRemove);
                if (removeRecipients != null)
                {
                    removeRecipients.DELETED_FLAG = true;
                    removeRecipients.DATE_MODIFIED = entryDate;
                    removeRecipients.USER_MODIFIED = userId;
                }
            }

            _repWorkflowMessage.SaveWorkFlowMessageRecipients(existingReceipients);

        }

        /// <summary>
        /// Gets the workflow message recipients.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public List<WF_WORKFLOW_MSGBOARD_RECIPIENTS> GetWorkflowMessageRecipients(int messageId)
        {
            return _repWorkflowMessage.GetWorkflowMessageRecipients(messageId);
        }

        public void SendMessageAddOnDocumentNotification(WF_WORKFLOW_MESSAGES workflowMessage, int userId, string companyLogo)
        {
            var tempEmails = new List<string>();
            var toEmailIds = string.Empty;
            var ccEmailIds = string.Empty;
            var bccEmailIds = string.Empty;
            var objShare = workflowMessage.CommunicationShare;
            if (objShare == null)
            {
                return;
            }
            if (objShare.ToEmails != null)
            {
                foreach (var toId in objShare.ToEmails)
                {
                    var startPos = toId.Email.IndexOf('<');
                    string filteredToEmailId = string.Empty;
                    string contactName = string.Empty;
                    if (startPos == -1)
                    {
                        filteredToEmailId = toId.Email;
                    }
                    else
                    {
                        contactName = toId.Email.Substring(0, startPos).ToString();
                        filteredToEmailId = toId.Email.Substring(startPos, toId.Email.Length - startPos)
                            .Replace("<", string.Empty);
                        filteredToEmailId = filteredToEmailId.Replace(">", string.Empty);
                    }
                    var isExistEmail = tempEmails.Any(x => x == filteredToEmailId);
                    if (!isExistEmail)
                    {
                        toEmailIds += filteredToEmailId + ";";
                        tempEmails.Add(filteredToEmailId);
                    }
                }
            }
            if (objShare.CcEmails != null)
            {
                foreach (var emailCc in objShare.CcEmails)
                {
                    var startPos = emailCc.Email.IndexOf('<');
                    string filteredToEmailId = string.Empty;
                    string contactName = string.Empty;
                    if (startPos == -1)
                    {
                        filteredToEmailId = emailCc.Email;
                    }
                    else
                    {
                        contactName = emailCc.Email.Substring(0, startPos).ToString();
                        filteredToEmailId = emailCc.Email.Substring(startPos, emailCc.Email.Length - startPos)
                            .Replace("<", string.Empty);
                        filteredToEmailId = filteredToEmailId.Replace(">", string.Empty);
                    }

                    var isExistEmail = tempEmails.Any(x => x == filteredToEmailId);
                    if (!isExistEmail)
                    {
                        ccEmailIds += filteredToEmailId + ";";
                        tempEmails.Add(filteredToEmailId);
                    }
                }
            }
            if (objShare.BccEmails != null)
            {
                foreach (var bccemail in objShare.BccEmails)
                {
                    var startPos = bccemail.Email.IndexOf('<');
                    string filteredToEmailId = string.Empty;
                    string contactName = string.Empty;
                    if (startPos == -1)
                    {
                        filteredToEmailId = bccemail.Email;
                    }
                    else
                    {
                        contactName = bccemail.Email.Substring(0, startPos).ToString();
                        filteredToEmailId = bccemail.Email.Substring(startPos, bccemail.Email.Length - startPos)
                            .Replace("<", string.Empty);
                        filteredToEmailId = filteredToEmailId.Replace(">", string.Empty);
                    }
                    var isExistEmail = tempEmails.Any(x => x == filteredToEmailId);
                    if (!isExistEmail)
                    {
                        bccEmailIds += filteredToEmailId + ";";
                        tempEmails.Add(filteredToEmailId);
                    }                   
                }
            }
            SaveWorkFlowMessageEntryInOutBound(workflowMessage.TITLE, workflowMessage.DESCRIPTION, ccEmailIds, toEmailIds,
                bccEmailIds, userId, workflowMessage.WORKFLOW_ID, workflowMessage.PROCESS_ID, companyLogo);
        }
        private void SaveWorkFlowMessageEntryInOutBound(string title, string notes, string ccEmailId, string toEmailId,
            string bccEmailId, int userId, int workflowId, int id , string companyLogo)
        {
            StringBuilder content = null;
            var userName = _srvAdrBk.GetContactNameByContactId(userId);
            var contactTechSupport = _repBase.GetCompanySetup<string>("CONTACT_TECHSUPPORT");
            var workflow = _repWorkflowMessage.GetWorkflowTitleById(workflowId);

            var formatedSubject = "WIP: A new message has been added on " + workflow.WORKFLOW_DEFINITION_TITLE;

            if (contactTechSupport != null)
            {

                var strEmailTemplatePath = GetEmailTemplatePath("WorkflowMessageEmailTemplate.html");

                using (var streamReader = new StreamReader(strEmailTemplatePath))
                {
                    content = new StringBuilder(streamReader.ReadToEnd());
                }
                DateTime date = new DateTime();
                var titleOrNumber = string.Empty;

                switch ((CustomWorkflowTypes)workflowId)
                {
                    case CustomWorkflowTypes.BILLING_DETAILS:
                        var billingDetails = _repWorkFlow.GetBillByBillId(id);
                        date = billingDetails.INVOICE_DATE;  
                        titleOrNumber = _repWorkflowMessage.GetBillingInvoiceNumberById(id);
                        break;

                    case CustomWorkflowTypes.VENDOR_CREDIT:
                       var vendorCreditDetails = _repWorkflowVendorCredit.GetVendorCreditById(id);
                       date = vendorCreditDetails.CREATED_DATE;
                        titleOrNumber = title;
                        break;
                }

                content.Replace("##Date##", date.ToString("MMMM d, yyyy"));
                content.Replace("##Ref##", titleOrNumber);
                content.Replace("##WorkflowTitle##", workflow.WORKFLOW_DEFINITION_TITLE);
                content.Replace("##MessageTitle##", title);
                content.Replace("##AddedBy##", userName);
                if (notes != null)
                {
                    content.Replace("##MessageDescription##", System.Web.HttpUtility.HtmlDecode(notes).ToString());
                }
                else
                {
                    content.Replace("##MessageDescription##", "-");
                }

                StringBuilder bodyMessage = null;
                var strEmailTemplate = _srvBase.GetEmailTemplatePath(companyLogo, EmailTemplateType.WithRequiredUserName);

                bodyMessage = new StringBuilder(strEmailTemplate);
                bodyMessage.Replace("##UserName##", userName);
                bodyMessage.Replace("##EmailContent##", content.ToString());

                var outboundCommunication = new OUTBOUND_COMMUNICATION()
                {
             
                    BODY_MESSAGE = bodyMessage.ToString(),
                    DATE_MODIFIED = DateTime.UtcNow,
                    DATE_OF_ENTRY = DateTime.UtcNow,
                    DELETED_FLAG = false,
                    SENT_FLAG = 0,
                    FROM_ID = contactTechSupport,
                    SUBJECT_MESSAGE = formatedSubject,
                    TO_ID = toEmailId,
                    CC_ID = ccEmailId,
                    BCC_ID = bccEmailId,
                    USER_ID = userId,
                    USER_MODIFIED = userId,
                    M_FORMAT = (byte)MailFormat.Html
                };

                _repBase.SendEmail(outboundCommunication);
            }
        }

        public void SendCommentAddOnDocumentMessageNotification(string comment, int messageId, int userId, int id, int workflowId, string companyLogo)
        {
            var objRecipients = _repWorkflowMessage.GetWorkflowMessageRecipients(messageId);
            var userIdOfMessage = _repWorkflowMessage.GetUserIdOfWorkflowDocumentMessage(messageId);
            var emailOfMessageUser = _srvAdrBk.GetContactEmail(userIdOfMessage);
            string recipientsToEmailIds = string.Empty;
            string recipientsCCEmailIds = string.Empty;
            recipientsToEmailIds += emailOfMessageUser + ";";
            foreach (var recipient in objRecipients)
            {
                if (recipient.IS_TO == true)
                {
                    recipientsToEmailIds += recipient.RECIPIENT_EMAIL + ";";
                }
                else if (recipient.IS_CC == true)
                {
                    recipientsCCEmailIds += recipient.RECIPIENT_EMAIL + ";";
                }
            }

            StringBuilder content = null;
            var userName = _srvAdrBk.GetContactNameByContactId(userId);
            var messageTitle = _repWorkflowMessage.GetWorkflowMessageTitle(messageId);
            string formatedSubject = string.Empty;

            if (messageTitle.Length >= 150)
            {
                formatedSubject = "WIP: A new comment has been added on Message " + messageTitle.Substring(0, 150) + "..";
            }
            else
            {
                formatedSubject = "WIP: A new comment has been added on Message " + messageTitle;
            }

            var contactTechSupport = _repBase.GetCompanySetup<string>("CONTACT_TECHSUPPORT");
            if (recipientsToEmailIds != null || recipientsCCEmailIds != null)
            {

                var strEmailTemplatePath = GetEmailTemplatePath("WorkflowMessageCommentEmailTemplate.html");
                using (var streamReader = new StreamReader(strEmailTemplatePath))
                {
                    content = new StringBuilder(streamReader.ReadToEnd());
                }

                var workflow = _repWorkflowMessage.GetWorkflowTitleById(id);
                DateTime date = new DateTime();
                var titleOrNumber = string.Empty;
                switch ((CustomWorkflowTypes)workflowId)
                {
                    case CustomWorkflowTypes.BILLING_DETAILS:
                        var billingDetails = _repWorkFlow.GetBillByBillId(id);
                        date = billingDetails.INVOICE_DATE;
                        titleOrNumber = _repWorkflowMessage.GetBillingInvoiceNumberById(id);
                        break;

                    case CustomWorkflowTypes.VENDOR_CREDIT:
                        var vendorCreditDetails = _repWorkflowVendorCredit.GetVendorCreditById(id);
                        date = vendorCreditDetails.CREATED_DATE;
                        titleOrNumber = messageTitle;
                        break;
                }



                content.Replace("##Date##", date.ToString("MMMM d, yyyy"));
                content.Replace("##Ref##", titleOrNumber);
                content.Replace("##WorkflowTitle##", workflow.WORKFLOW_DEFINITION_TITLE);

                content.Replace("##MessageTitle##", messageTitle);
                content.Replace("##Comment##", comment);
                content.Replace("##AddedBy##", userName);

                StringBuilder bodyMessage = null;
                var strEmailTemplate = _srvBase.GetEmailTemplatePath(companyLogo, EmailTemplateType.WithRequiredUserName);
                bodyMessage = new StringBuilder(strEmailTemplate);
                bodyMessage.Replace("##UserName##", "User");
                bodyMessage.Replace("##EmailContent##", content.ToString());

                var outboundCommunication = new OUTBOUND_COMMUNICATION()
                {
              
                    BODY_MESSAGE = bodyMessage.ToString(),
                    DATE_MODIFIED = DateTime.UtcNow,
                    DATE_OF_ENTRY = DateTime.UtcNow,
                    DELETED_FLAG = false,
                    SENT_FLAG = 0,
                    FROM_ID = contactTechSupport,
                    SUBJECT_MESSAGE = formatedSubject,
                    TO_ID = recipientsToEmailIds,
                    CC_ID = recipientsCCEmailIds,
                    USER_ID = userId,
                    USER_MODIFIED = userId,
                    M_FORMAT = (byte)MailFormat.Html
                };
                _repBase.SendEmail(outboundCommunication);
            }
        }
        public string GetEmailTemplatePath(string bodyFileName)
        {
            var appDomain = System.AppDomain.CurrentDomain;
            var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
            var strEmailTemplatePath = Path.Combine(basePath, "Resources", bodyFileName);
            return strEmailTemplatePath;
        }
    }
}
