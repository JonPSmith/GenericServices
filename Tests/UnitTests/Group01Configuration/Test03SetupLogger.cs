using GenericServices;
using GenericServices.Logger;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group01Configuration
{

    class Test03SetupLogger
    {
        class FirstClass
        {
            public static readonly IGenericLogger Logger;

            static FirstClass()
            {
                Logger = ServicesConfiguration.GetLogger("hello");
            }
        }

        class SecondClass
        {
            public static readonly IGenericLogger Logger;

            static SecondClass()
            {
                Logger = ServicesConfiguration.GetLogger("test");
            }
        }

        [Test]
        public void Check01DefaultLoggerSetupOk()
        {

            //SETUP  
            ServicesConfiguration.SetLoggerMethod = name => new NoLoggingGenericLogger();

            //ATTEMPT
            var classWithLogger = new FirstClass();

            //VERIFY
            FirstClass.Logger.IsA<NoLoggingGenericLogger>();

        }

        [Test]
        public void Check02ChangedLoggerSetupOk()
        {

            //SETUP  
            ServicesConfiguration.SetLoggerMethod = name => new Log4NetGenericLogger(name);

            //ATTEMPT
            var classWithLogger = new SecondClass();

            //VERIFY
            SecondClass.Logger.IsA<Log4NetGenericLogger>();

        }

    }
}
