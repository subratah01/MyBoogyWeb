using System;

namespace OA.Model
{
    public class APIResponse
    {
        public string Message { get; set; }
        public object DataResult { get; set; }
        public int StatusCode { get; set; }
    }
}
