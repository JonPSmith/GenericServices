using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericServices
{
    /// <summary>
    /// Place this on a property to stop it being copied back to the TEntity
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CopyToDtoOnlyAttribute : Attribute
    {
    }
}
