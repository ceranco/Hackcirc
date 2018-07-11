using System;

namespace Node
{
    public static class Utility
    {
        public static Int64 GetTimeCount()
        {
            // Time since 1970, in order 
            Int64 timeCount =
                    (Int64)DateTime.Now.ToUniversalTime().Subtract(
                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            return timeCount;
        }

    }
}
