#region licence
// The MIT License (MIT)
// 
// Filename: Test03SuccessOrErrorsT.cs
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
using GenericLibsBase;
using GenericLibsBase.Core;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group00GenericLibsBase
{
    class Test03SuccessOrErrorsT
    {

        [Test]
        public void Test01DefaultsToNotValidOk()
        {
            //SETUP  
            var status = new SuccessOrErrors<string>();

            //ATTEMPT

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.ToString().ShouldEqual("Not currently setup");
        }

        [Test]
        public void Test02UnsetStatusHasNoErrorsOk()
        {
            //SETUP  
            var status = new SuccessOrErrors<string>();

            //ATTEMPT

            //VERIFY
            status.HasErrors.ShouldEqual(false);
        }

        [Test]
        public void Test03DefaultsAccessErrorsFailsOk()
        {
            //SETUP  
            var status = new SuccessOrErrors<string>();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => status.Errors.Any());

            //VERIFY
            ex.Message.ShouldEqual("The status must have an error set or the success message set before you can access errors.");
        }

        [Test]
        public void Test04AddSingleErrorOk()
        {
            //SETUP  
            var status = new SuccessOrErrors<string>();

            //ATTEMPT
            status.AddSingleError("This was {0}.", "bad");

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.HasErrors.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("");
            status.Result.ShouldEqual(null);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("This was bad.");
            status.Errors[0].MemberNames.Count().ShouldEqual(0);
        }

        [Test]
        public void Test05SetSuccessResultOk()
        {
            //SETUP  
            var status = new SuccessOrErrors<string>();

            //ATTEMPT
            status.SetSuccessWithResult("The result", "This is a message");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasErrors.ShouldEqual(false);
            status.Result.ShouldEqual("The result");
            status.SuccessMessage.ShouldEqual("This is a message");
        }

        [Test]
        public void Test06SetSuccessViaStaticOk()
        {
            //SETUP             

            //ATTEMPT
            var status = SuccessOrErrors<string>.SuccessWithResult("The result", "This is a message");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasErrors.ShouldEqual(false);
            status.Result.ShouldEqual("The result");
            status.SuccessMessage.ShouldEqual("This is a message");
        }


        [Test]
        public void Test07UpdateSuccessMessageOk()
        {
            //SETUP             
            var status = SuccessOrErrors<string>.SuccessWithResult("The result", "This is a message");

            //ATTEMPT
            status.UpdateSuccessMessage("New success message");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasErrors.ShouldEqual(false);
            status.SuccessMessage.ShouldEqual("New success message");
        }

        [Test]
        public void Test08CheckAssignNonResultOk()
        {
            //SETUP  
            ISuccessOrErrors status = new SuccessOrErrors<string>();

            //ATTEMPT
            ((SuccessOrErrors<string>)status).SetSuccessWithResult("The result", "This is a message");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasErrors.ShouldEqual(false);
            ((SuccessOrErrors<string>)status).Result.ShouldEqual("The result");
        }

        //--------------------------------------------------------------

        [Test]
        public void Test10CheckConvertResultToNormalStatusValidOk()
        {
            //SETUP  
            var statusWithResult = new SuccessOrErrors<string>();
            statusWithResult.SetSuccessWithResult("The result", "This is a message");

            //ATTEMPT
            var status = statusWithResult as ISuccessOrErrors;

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasErrors.ShouldEqual(false);
        }

        [Test]
        public void Test11CheckConvertResultToNormalStatusNotValidOk()
        {
            //SETUP  
            var statusWithResult = new SuccessOrErrors<string>();
            statusWithResult.AddSingleError("There was an error");

            //ATTEMPT
            var status = statusWithResult as ISuccessOrErrors;

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.HasErrors.ShouldEqual(true);
        }


        [Test]
        public void Test15CheckConvertNormalStatusToStatusValidOk()
        {
            //SETUP  
            var statusWithResult = new SuccessOrErrors();
            statusWithResult.SetSuccessMessage("This is a message");

            //ATTEMPT
            var status = SuccessOrErrors<string>.ConvertNonResultStatus(statusWithResult);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasErrors.ShouldEqual(false);
            status.Result.ShouldEqual(null);
        }

        [Test]
        public void Test16CheckConvertResultToNormalStatusNotValidOk()
        {
            //SETUP  
            var statusWithResult = new SuccessOrErrors();
            statusWithResult.AddSingleError("error");

            //ATTEMPT
            var status = SuccessOrErrors<string>.ConvertNonResultStatus(statusWithResult);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.HasErrors.ShouldEqual(true);
        }

        [Test]
        public void Test16CheckConvertResultStatusToAnotherResultStatusValidOk()
        {
            //SETUP  
            var statusWithResult = new SuccessOrErrors<int>();
            statusWithResult.SetSuccessWithResult(1,"This is a message");

            //ATTEMPT
            var status = SuccessOrErrors<string>.ConvertNonResultStatus(statusWithResult);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.HasErrors.ShouldEqual(false);
            status.Result.ShouldEqual(null);
        }

        [Test]
        public void Test17CheckConvertIntToAnotherResultStatusFail()
        {
            //SETUP  

            //ATTEMPT
            var ex = Assert.Throws<ArgumentNullException>(() => SuccessOrErrors<string>.ConvertNonResultStatus("string"));

            //VERIFY
            ex.Message.ShouldStartWith("The status parameter was not derived from a type thta supported ISuccessOrErrors.");
        }

        //---------------------------------------------

        [Test]
        public void Test20CombineDefaultsToNotValidOk()
        {
            //SETUP  
            var status1 = new SuccessOrErrors<string>();
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
            var status1 = new SuccessOrErrors<string>();
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
            var status1 = new SuccessOrErrors<string>();
            var status2 = new SuccessOrErrors();
            status1.SetSuccessWithResult("The result", "This was {0}.", "successful");

            //ATTEMPT
            status1.Combine(status2);

            //VERIFY
            status1.IsValid.ShouldEqual(true);
            status1.HasErrors.ShouldEqual(false);
            status1.HasWarnings.ShouldEqual(false);
            status1.Result.ShouldEqual("The result");
            status1.SuccessMessage.ShouldEqual("This was successful.");
            status1.Errors.Count.ShouldEqual(0);
        }

        [Test]
        public void Test26CombineOtherSetSuccessOk()
        {
            //SETUP 
            var status1 = new SuccessOrErrors<string>();
            var status2 = new SuccessOrErrors<DateTime>();
            status2.SetSuccessWithResult(DateTime.Now, "This was {0}.", "successful");

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
            var status1 = new SuccessOrErrors<string>();
            var status2 = new SuccessOrErrors();
            status2.AddWarning("This is a warning");
            status1.SetSuccessWithResult("The result", "This was {0}.", "successful");

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
            var status1 = new SuccessOrErrors<string>();
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
            var status1 = new SuccessOrErrors<string>();
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
            var status1 = new SuccessOrErrors<string>();
            var status2 = "hello";

            //ATTEMPT
            var ex = Assert.Throws<ArgumentNullException>(() => status1.Combine(status2));

            //VERIFY
            ex.Message.ShouldStartWith("The status parameter was not derived from a type that supported ISuccessOrErrors.");
        }

    }
}
