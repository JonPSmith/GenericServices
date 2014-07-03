using GenericServices;
using GenericServices.Logger;
using NUnit.Framework;
using Tests.Helpers;
using Log4NetGenericLogger = Tests.Helpers.Log4NetGenericLogger;

namespace Tests.UnitTests.Group05GenericLogger
{


    class Test02SetupLogger
    {
        class FirstClass
        {
            public static readonly IGenericLogger Logger;

            static FirstClass()
            {
                Logger = GenericLoggerFactory.GetLogger("hello");
            }
        }

        class SecondClass
        {
            public static readonly IGenericLogger Logger;

            static SecondClass()
            {
                Logger = GenericLoggerFactory.GetLogger("test");
            }
        }

        [Test]
        public void Check01DefaultLoggerSetupOk()
        {

            //SETUP  
            GenericLoggerFactory.SetLoggerMethod = name => new NoLoggingGenericLogger();

            //ATTEMPT
            var classWithLogger = new FirstClass();

            //VERIFY
            FirstClass.Logger.IsA<NoLoggingGenericLogger>();

        }

        [Test]
        public void Check02ChangedLoggerSetupOk()
        {

            //SETUP  
            GenericLoggerFactory.SetLoggerMethod = name => new Log4NetGenericLogger(name);

            //ATTEMPT
            var classWithLogger = new SecondClass();

            //VERIFY
            SecondClass.Logger.IsA<Log4NetGenericLogger>();

        }

    }
}
