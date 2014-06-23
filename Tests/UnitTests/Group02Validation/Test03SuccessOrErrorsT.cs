using System;
using GenericServices.Services;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group02Validation
{
    class Test03SuccessOrErrorsT
    {

        [Test]
        public void Check01SetErrorsNoResultOk()
        {
            //SETUP  
            var status = new SuccessOrErrors<string>();

            //ATTEMPT
            status.AddSingleError("This is an error");

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Result.ShouldEqual(null);
        }

        [Test]
        public void Check02SetSuccessResultOk()
        {
            //SETUP  
            var status = new SuccessOrErrors<string>();

            //ATTEMPT
            status.SetSuccessWithResult("The result", "This is a message");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.Result.ShouldEqual("The result");
        }

        [Test]
        public void Check03SetSuccessViaStaticOk()
        {
            //SETUP             

            //ATTEMPT
            var status = SuccessOrErrors<string>.SuccessWithResult("The result", "This is a message");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.Result.ShouldEqual("The result");
        }

        [Test]
        public void Check04SetSuccessMessageFail()
        {
            //SETUP  
            var status = new SuccessOrErrors<string>();

            //ATTEMPT
            var ex =
                Assert.Throws<InvalidOperationException>(() => status.SetSuccessMessage("Bad way of setting success"));

            //VERIFY
            ex.Message.ShouldEqual("With SuccessOrErrors<T> you must call SetSuccessWithResult.");
        }

    }
}
