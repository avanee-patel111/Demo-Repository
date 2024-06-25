using SOL.Common.Business.Models;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IWorkflowMessageRepository<T>
    {
        /// <summary>
        /// Saves the work flow message.
        /// </summary>
        /// <param name="workflowMessage">The workflow message.</param>
        void SaveWorkFlowMessage(WF_WORKFLOW_MESSAGES workflowMessage);
        /// <summary>
        /// Saves the work flow message comment.
        /// </summary>
        /// <param name="workflowMessageComment">The workflow message comment.</param>
        void SaveWorkFlowMessageComment(WF_WORKFLOW_MESSAGE_COMMENT workflowMessageComment);
        /// <summary>
        /// Saves the work flow message comment.
        /// </summary>
        /// <param name="workflowMessageRecipients">The workflow message recipients.</param>
        void SaveWorkFlowMessageRecipients(List<WF_WORKFLOW_MSGBOARD_RECIPIENTS> workflowMessageRecipients);
        /// <summary>
        /// Gets the workflow message.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        WF_WORKFLOW_MESSAGES GetWorkflowMessage(int messageId);
        /// <summary>
        /// Gets the workflow message comment.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <returns></returns>
        WF_WORKFLOW_MESSAGE_COMMENT GetWorkflowMessageComment(int commentId);
        /// <summary>
        /// Gets the workflow message recipients.
        /// </summary>
        /// <param name="receptionId">The reception identifier.</param>
        /// <returns></returns>
        WF_WORKFLOW_MSGBOARD_RECIPIENTS GetWorkflowRecipient(int receptionId);
        /// <summary>
        /// Gets the workflow message recipients.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        List<WF_WORKFLOW_MSGBOARD_RECIPIENTS> GetWorkflowMessageRecipients(int messageId);
        /// <summary>
        /// Gets the user identifier of workflow document message.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        int GetUserIdOfWorkflowDocumentMessage(int messageId);
        /// <summary>
        /// Gets the workflow message title.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        string GetWorkflowMessageTitle(int messageId);


        /// <summary>
        /// Gets the workflow title by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        //IdValueModel GetWorkflowTitleById(int id);
        WORKFLOW_DEFINITION GetWorkflowTitleById(int id);

        DateTime GetBillingDateById(int billId);

        string GetBillingInvoiceNumberById(int billId);

         DateTime GetVendorCreditDateById(int vendorId);

    }
}
