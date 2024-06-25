using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Services
{
    public class WorkflowTimelineService<T> : IWorkflowTimelineService<T>
    {
        private IWorkFlowRepository<T> _repWorkFlow;
        public WorkflowTimelineService(IWorkFlowRepository<T> repWorkFlow) {
            this._repWorkFlow = repWorkFlow;
        }

        /// <summary>
        /// Saves the worflow timeline.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="status">The status.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <param name="previousObjectId">The previous object identifier.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="remarks">The remarks.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="isRole">The is role.</param>
        public void SaveWorflowTimeline(int entityId, int entityType,int? status, int? objectId, int? previousObjectId, int objectType, int eventId,
       string remarks, int userId, DateTime date,int? orderId,bool? isRole)
        {
            var timeLine = new WF_TIME_LINE();
            timeLine.EVENT_ID = eventId;
            timeLine.ENTITY_ID = entityId;
            timeLine.ORDER_ID = orderId;
            timeLine.IS_ROLE = isRole;
            timeLine.APPROVAL_STATUS = status;
            timeLine.ENTITY_TYPE = entityType;
            timeLine.OBJECT_ID = objectId;
            timeLine.PREVIOUS_OBJECT_ID = previousObjectId;
            timeLine.OBJECT_TYPE_ID = objectType;
            timeLine.REMARKS = remarks;
            timeLine.USER_ID = userId;
            timeLine.USER_MODIFIED = userId;
            timeLine.DATE_OF_ENTRY = date;
            timeLine.DATE_MODIFIED = date;
            timeLine.DELETED_FLAG = false;            
            _repWorkFlow.SaveWorkflowTimeLine(timeLine);
        }
    }
}
