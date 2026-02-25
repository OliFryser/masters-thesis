using System.Collections.Generic;

namespace WFC.Output
{
    public class Result
    {
        public Result(Map? map, Status status)
        {
            Map = map;
            Status = status;
        }

        public Map? Map { get; }
        public Status Status { get; }
    }
}