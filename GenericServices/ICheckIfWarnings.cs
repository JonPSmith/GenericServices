using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericServices
{
    public interface ICheckIfWarnings
    {
        /// <summary>
        /// This allows the user to control whether data should still be written if warnings
        /// </summary>
        bool WriteEvenIfWarning { get; }
    }

}
