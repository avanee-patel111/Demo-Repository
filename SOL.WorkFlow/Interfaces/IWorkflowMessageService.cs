using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IWorkflowMessageService<T>
    {
        /// <summary>
        /// Saves the work flow message.
        /// </summary>
        /// <param name="workflowMessage">The workflow message.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        void SaveWorkFlowMessage(WF_WORKFLOW_MESSAGES workflowMessage, int userId, string companyLogo);
        /// <summary>
        /// Saves the work flow message comment.
        /// </summary>
        /// <param name="workflowMessageComment">The workflow message comment.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        void SaveWorkFlowMessageComment(workFlowCommentMsgInfo workflowMessageComment, int userId, string companyLogo);
        /// <summary>
        /// Saves the work flow message recipients.
        /// </summary>
        /// <param name="workflowMessageRecipients">The workflow message recipients.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        void SaveWorkFlowMessageRecipients(WF_WORKFLOW_MSGBOARD_RECIPIENTS workflowMessageRecipients, int userId);
    }
}
