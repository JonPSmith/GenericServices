using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GenericServices.Logger;

namespace GenericServices
{

    public static class GenericLoggerFactory
    {
        /// <summary>
        /// This should be given a method that takes a string, to have the logger, and returns
        /// a IGenericLogger instance. 
        /// </summary>
        public static Func<string, IGenericLogger> SetLoggerMethod { get; set; }

        /// <summary>
        /// This returns the logger set in the Generic Service package
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IGenericLogger GetLogger( string name)
        {
            return SetLoggerMethod(name);
        }

        //This sets the default logger to the noLogger so that there is always a logger present
        private static readonly IGenericLogger NoLoggerInstance = new NoLoggingGenericLogger(); 
        static GenericLoggerFactory()
        {
            SetLoggerMethod = name => NoLoggerInstance;
        }
    }
}
