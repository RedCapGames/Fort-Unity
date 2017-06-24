using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort.Dispatcher
{
    public interface IDispatcher
    {
        void Dispach(Action action);
    }
}
