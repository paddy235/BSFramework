using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.BusinessExceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class TrainingExcuteException : Exception
    {
        public TrainingExcuteException() : base() { }
        public TrainingExcuteException(string message) : base(message) { }
    }
}
