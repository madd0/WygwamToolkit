using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wygwam.Windows.Tools
{
    public delegate Task AsyncAction();
    public delegate Task<T> AsyncAction<T>();
    public delegate Task<T> AsyncAction<T, U>(U obj);
}
