using System;

namespace Tests.DataClasses.Concrete.Helpers
{
    public abstract class TrackUpdate
    {
        public DateTime LastUpdated { get; protected set; }

        internal void UpdateTrackingInfo()
        {
            LastUpdated = DateTime.UtcNow;
        }

        protected TrackUpdate()
        {
            UpdateTrackingInfo();           //on creation then set date
        }
    }
}
