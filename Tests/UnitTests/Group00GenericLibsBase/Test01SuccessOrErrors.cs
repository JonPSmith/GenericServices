#region licence
// The MIT License (MIT)
// 
// Filename: Test01SuccessOrErrors.cs
// Date Created: 2014/10/28
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Linq;
using GenericLibsBase.Core;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group00GenericLibsBase
{
    class Test01SuccessOrErrors
    {

        [Test]
        public void Test01DefaultsToNotValidOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT

            //VERIFY
            status.IsValid.ShouldEqual(false); 
            status.ToString().ShouldEqual("Not currently setup");
        }

        [Test]
        public void Test02UnsetStatusHasNoErrorsOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT

            //VERIFY
            status.HasErrors.ShouldEqual(false);
        }

        [Test]
        public void Test03DefaultsAccessErrorsFailsOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => status.Errors.Any());

            //VERIFY
            ex.Message.ShouldEqual("The status must have an error set or the success message set before you can access errors.");
        }

        [Test]
        public void Test05SetSuccessOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.SetSuccessMessage("This was {0}.", "successful");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasErrors.ShouldEqual(false);
            status.HasWarnings.ShouldEqual(false);
            status.SuccessMessage.ShouldEqual("This was successful.");
            status.Errors.Count.ShouldEqual(0);
        }

        [Test]
        public void Test06SuccessWithWarningsOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddWarning("This is a warning");
            status.SetSuccessMessage("This was {0}.", "successful");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasErrors.ShouldEqual(false);
            status.HasWarnings.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("This was successful. (has 1 warnings)");
            status.Warnings.Count.ShouldEqual(1);
            status.Warnings[0].ShouldEqual("Warning: This is a warning");
        }

        [Test]
        public void Test10AddSingleErrorOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddSingleError("This was {0}.", "bad");

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.HasErrors.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("");
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("This was bad.");
            status.Errors[0].MemberNames.Count().ShouldEqual(0);
        }

        [Test]
        public void Test11AddNamedParameterErrorOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddNamedParameterError("MyParameterName", "This was {0}.", "bad");

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.HasErrors.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("");
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("This was bad.");
            status.Errors[0].MemberNames.Count().ShouldEqual(1);
            status.Errors[0].MemberNames.First().ShouldEqual("MyParameterName");
        }

        //-----------------------------------------------------
        //Check combine

        [Test]
        public void Test20CombineDefaultsToNotValidOk()
        {
            //SETUP  
            var status1 = new SuccessOrErrors();
            var status2 = new SuccessOrErrors();

            //ATTEMPT
            status1.Combine(status2);

            //VERIFY
            status1.IsValid.ShouldEqual(false);
            status1.ToString().ShouldEqual("Not currently setup");
        }

        [Test]
        public void Test21CombineUnsetStatusHasNoErrorsOk()
        {
            //SETUP  
            var status1 = new SuccessOrErrors();
            var status2 = new SuccessOrErrors();

            //ATTEMPT
            status1.Combine(status2);

            //VERIFY
            status1.HasErrors.ShouldEqual(false);
        }

        [Test]
        public void Test25CombineSetSuccessOk()
        {
            //SETUP 
            var status1 = new SuccessOrErrors();
            var status2 = new SuccessOrErrors();
            status1.SetSuccessMessage("This was {0}.", "successful");

            //ATTEMPT
            status1.Combine(status2);

            //VERIFY
            status1.IsValid.ShouldEqual(true);
            status1.HasErrors.ShouldEqual(false);
            status1.HasWarnings.ShouldEqual(false);
            status1.SuccessMessage.ShouldEqual("This was successful.");
            status1.Errors.Count.ShouldEqual(0);
        }

        [Test]
        public void Test26CombineOtherSetSuccessOk()
        {
            //SETUP 
            var status1 = new SuccessOrErrors();
            var status2 = new SuccessOrErrors();
            status2.SetSuccessMessage("This was {0}.", "successful");

            //ATTEMPT
            status1.Combine(status2);

            //VERIFY
            status1.IsValid.ShouldEqual(false);
            status1.ToString().ShouldEqual("Not currently setup");
        }

        [Test]
        public void Test27SuccessWithOtherWarningsOk()
        {
            //SETUP  
            var status1 = new SuccessOrErrors();
            var status2 = new SuccessOrErrors();
            status2.AddWarning("This is a warning");
            status1.SetSuccessMessage("This was {0}.", "successful");

            //ATTEMPT
            status1.Combine(status2);

            //VERIFY
            status1.IsValid.ShouldEqual(true);
            status1.HasErrors.ShouldEqual(false);
            status1.HasWarnings.ShouldEqual(true);
            status1.SuccessMessage.ShouldEqual("This was successful. (has 1 warnings)");
            status1.Warnings.Count.ShouldEqual(1);
            status1.Warnings[0].ShouldEqual("Warning: This is a warning");
        }

        [Test]
        public void Test30CombineAddSingleErrorOk()
        {
            //SETUP  
            var status1 = new SuccessOrErrors();
            var status2 = new SuccessOrErrors();
            status1.AddSingleError("This was {0}.", "bad");

            //ATTEMPT
            status1.Combine(status2);

            //VERIFY
            status1.IsValid.ShouldEqual(false);
            status1.HasErrors.ShouldEqual(true);
            status1.SuccessMessage.ShouldEqual("");
            status1.Errors.Count.ShouldEqual(1);
            status1.Errors[0].ErrorMessage.ShouldEqual("This was bad.");
            status1.Errors[0].MemberNames.Count().ShouldEqual(0);
        }

        [Test]
        public void Test31CombineAddOtherSingleErrorOk()
        {
            //SETUP  
            var status1 = new SuccessOrErrors();
            var status2 = new SuccessOrErrors();
            status2.AddSingleError("This was {0}.", "bad");

            //ATTEMPT
            status1.Combine(status2);

            //VERIFY
            status1.IsValid.ShouldEqual(false);
            status1.HasErrors.ShouldEqual(true);
            status1.SuccessMessage.ShouldEqual("");
            status1.Errors.Count.ShouldEqual(1);
            status1.Errors[0].ErrorMessage.ShouldEqual("This was bad.");
            status1.Errors[0].MemberNames.Count().ShouldEqual(0);
        }

        [Test]
        public void Test35CombineBadStatusFail()
        {
            //SETUP  
            var status1 = new SuccessOrErrors();
            var status2 = "hello";

            //ATTEMPT
            var ex = Assert.Throws<ArgumentNullException>(() => status1.Combine(status2));

            //VERIFY
            ex.Message.ShouldStartWith("The status parameter was not derived from a type that supported ISuccessOrErrors.");
        }

        //-----------------------------------------------------
        //get combined errors

        [Test]
        public void Test50SingleErrorGetAllErrorsOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddSingleError("This was {0}.", "bad");
            var str = status.GetAllErrors();

            //VERIFY
            str.ShouldEqual("This was bad.");
        }

        [Test]
        public void Test51SingleErrorAsHtmlOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddSingleError("This was {0}.", "bad");
            var html = status.ErrorsAsHtml();

            //VERIFY
            html.ShouldEqual("<p>This was bad.</p>");           
        }

        [Test]
        public void Test52SingleParamErrorGetAllErrorsOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddNamedParameterError("MyParameterName", "This was {0}.", "bad");
            var str = status.GetAllErrors();

            //VERIFY
            str.ShouldEqual("MyParameterName: This was bad.");
        }

        [Test]
        public void Test53SingleParamErrorAsHtmlOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddNamedParameterError("MyParameterName", "This was {0}.", "bad");
            var html = status.ErrorsAsHtml();

            //VERIFY
            html.ShouldEqual("<p>MyParameterName: This was bad.</p>");
        }

        [Test]
        public void Test54MultipleErrorsGetAllErrorsOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddSingleError("This was {0}.", "bad");
            status.AddNamedParameterError("MyParameterName", "This was {0}.", "bad");
            var str = status.GetAllErrors();

            //VERIFY
            str.ShouldEqual("This was bad.\nMyParameterName: This was bad.");
        }

        [Test]
        public void Test55MultipleErrorsAsHtmlOk()
        {
            //SETUP  
            var status = new SuccessOrErrors();

            //ATTEMPT
            status.AddSingleError("This was {0}.", "bad");
            status.AddNamedParameterError("MyParameterName", "This was {0}.", "bad");
            var html = status.ErrorsAsHtml();

            //VERIFY
            html.ShouldEqual("<ul><li>This was bad.</li><li>MyParameterName: This was bad.</li></ul>");
        }



        //Cannot test this as cannot new DbEntityEntry and won't accept null 
        //[Test]
        //public void Test50AddNamedParameterErrorOk()
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
