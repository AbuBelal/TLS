using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    public  class StdWithCenterId
    {
        public Entities.Student Student { get; set; }
        public long CenterId { get; set; }
    }
}
