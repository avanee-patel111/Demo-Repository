using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Interfaces
{
    public interface IWorkflowTimelineService<T>
    {

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
        void SaveWorflowTimeline(int entityId, int entityType, int? status, int? objectId, int? previousObjectId, int objectType, int eventId,
            string remarks, int userId, DateTime date, int? orderId, bool? isRole);
    }
}
