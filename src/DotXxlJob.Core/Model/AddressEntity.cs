using System;

namespace DotXxlJob.Core.Model
{
    public class AddressEntry
    {
        public string RequestUri { get; set; }

        private DateTime? LastFailedTime { get; set; }

        private int FailedTimes { get; set; }

        public bool CheckAccessible()
        {
            if (LastFailedTime == null)
                return true;

            if (DateTime.UtcNow.Subtract(LastFailedTime.Value) > Constants.AdminServerReconnectInterval)
                return true;

            if (FailedTimes < Constants.AdminServerCircuitFailedTimes)
                return true;

            return false;
        }

        public void Reset()
        {
            LastFailedTime = null;
            FailedTimes = 0;
        }

        public void SetFail()
        {
            LastFailedTime = DateTime.UtcNow;
            FailedTimes++;
        }
    }
}