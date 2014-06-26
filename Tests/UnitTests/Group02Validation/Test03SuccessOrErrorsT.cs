using System;
using GenericServices;
using GenericServices.Core;
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
            status.SuccessMessage.ShouldEqual("This is a message");
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
            status.SuccessMessage.ShouldEqual("This is a message");
        }


        [Test]
        public void Check04UpdateSuccessMessageOk()
        {
            //SETUP             
            var status = SuccessOrErrors<string>.SuccessWithResult("The result", "This is a message");

            //ATTEMPT
            status.UpdateSuccessMessage("New success message");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("New success message");
        }

        [Test]
        public void Check05SetSuccessMessageFail()
        {
            //SETUP  
            var status = new SuccessOrErrors<string>();

            //ATTEMPT
            var ex =
                Assert.Throws<InvalidOperationException>(() => status.SetSuccessMessage("Bad way of setting success"));

            //VERIFY
            ex.Message.ShouldEqual("With SuccessOrErrors<T> you must call SetSuccessWithResult.");
        }


        [Test]
        public void Check10CheckAssignNonResultOk()
        {
            //SETUP  
            ISuccessOrErrors status = new SuccessOrErrors<string>();

            //ATTEMPT
            ((SuccessOrErrors<string>)status).SetSuccessWithResult("The result", "This is a message");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            ((SuccessOrErrors<string>)status).Result.ShouldEqual("The result");
        }

    }
}
