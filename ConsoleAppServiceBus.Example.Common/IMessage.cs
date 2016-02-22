using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppServiceBus.Example.Common
{
    public interface IMessage
    {
       int Id { get; set; }
       DateTime Created { get; set; }
       string CreatedBy { get; set; }
       DateTime Modified { get; set; }
       string ModifiedBy { get; set; }
    }
}
