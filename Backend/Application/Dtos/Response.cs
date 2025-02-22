using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Application.Dtos
{
    public class Response
    {
        public object Message { get; set; }
        public int StatusCode { get; set; }
    }
}