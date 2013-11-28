using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wygwam.Windows.Security
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited=true)]
    public class SecurityPropertyAttribute : Attribute
    {
    }
}
