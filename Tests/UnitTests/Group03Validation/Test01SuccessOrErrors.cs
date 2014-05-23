using System;
using System.Linq;
using GenericServices.Concrete;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group03Validation
{
    class Test01SuccessOrErrors
    {

        [Test]
        public void Check01DefaultsToNotValidOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT

            //VERIFY
            status.IsValid.ShouldEqual(false); 
            status.ToString().ShouldEqual("Not currently setup");
        }

        [Test]
        public void Check02DefaultsAccessErrorsFailsOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            var ex = Assert.Throws<NullReferenceException>(() => status.Errors.Any());

            //VERIFY
            ex.Message.ShouldEqual("The status must have an error set or the success message set before you can access errors.");
        }

        [Test]
        public void Check05SetSuccessOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.SetSuccessMessage("This was {0}.", "successful");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasWarnings.ShouldEqual(false);
            status.SuccessMessage.ShouldEqual("This was successful.");
            status.Errors.Count.ShouldEqual(0);
        }

        [Test]
        public void Check06SuccessWithWarningsOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddWarning("This is a warning");
            status.SetSuccessMessage("This was {0}.", "successful");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasWarnings.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("This was successful. (has 1 warnings)");
            status.Warnings.Count.ShouldEqual(1);
            status.Warnings[0].ShouldEqual("Warning: This is a warning");
        }

        [Test]
        public void Check10AddSingleErrorOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddSingleError("This was {0}.", "bad");

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.SuccessMessage.ShouldEqual("");
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("This was bad.");
            status.Errors[0].MemberNames.Count().ShouldEqual(0);
        }

        [Test]
        public void Check11AddNamedParameterErrorOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddNamedParameterError("MyParameterName", "This was {0}.", "bad");

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.SuccessMessage.ShouldEqual("");
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("This was bad.");
            status.Errors[0].MemberNames.Count().ShouldEqual(1);
            status.Errors[0].MemberNames.First().ShouldEqual("MyParameterName");
        }

        //Cannot test this as cannot new DbEntityEntry and won't accept null 
        //[Test]
        //public void Check20AddNamedParameterErrorOk()
        //{
        //    //SETUP  
        //    var status = new SuccessOrErrors();
        //    var dbErrors = new Collection<DbEntityValidationResult>
        //    {
        //        new DbEntityValidationResult( new DbEntityEntry( null), new Collection<DbValidationError>
        //            {
        //                new DbValidationError( "property1", "Error in property1"),
        //                new DbValidationError( "property2", "Error in property2")
        //            })
        //    }; 

        //    //ATTEMPT
        //    status.SetErrors(dbErrors);

        //    //VERIFY
        //    status.IsValid.ShouldEqual(false);
        //    status.Errors.Count.ShouldEqual(2);
        //    status.Errors[0].ErrorMessage.ShouldEqual("Error in property1");
        //    status.Errors[0].MemberNames.Count().ShouldEqual(1);
        //    status.Errors[0].MemberNames.First().ShouldEqual("property1");
        //    status.Errors[1].ErrorMessage.ShouldEqual("Error in property2");
        //    status.Errors[1].MemberNames.Count().ShouldEqual(1);
        //    status.Errors[1].MemberNames.First().ShouldEqual("property2");
        //}
    }
}
