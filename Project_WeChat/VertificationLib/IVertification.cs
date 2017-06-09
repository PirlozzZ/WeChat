using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertificationLib
{
    interface IVertification
    {
        bool VertifyMethod(string loginno, string password);
    }
}
