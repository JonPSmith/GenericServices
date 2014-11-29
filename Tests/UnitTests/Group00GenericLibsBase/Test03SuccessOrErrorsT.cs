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
using GenericLibsBase;
using GenericLibsBase.Core;
using NUnit.Framework;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group00GenericLibsBase
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
        public void Check05CheckAssignNonResultOk()
        {
            //SETUP  
            ISuccessOrErrors status = new SuccessOrErrors<string>();

            //ATTEMPT
            ((SuccessOrErrors<string>)status).SetSuccessWithResult("The result", "This is a message");

            //VERIFY
            status.IsValid.ShouldEqual(true);
            ((SuccessOrErrors<string>)status).Result.ShouldEqual("The result");
        }

        //--------------------------------------------------------------

        [Test]
        public void Check10CheckConvertResultToNormalStatusValidOk()
        {
            //SETUP  
            var statusWithResult = new SuccessOrErrors<string>();
            statusWithResult.SetSuccessWithResult("The result", "This is a message");

            //ATTEMPT
            var status = statusWithResult as ISuccessOrErrors;

            //VERIFY
            status.IsValid.ShouldEqual(true);
        }

        [Test]
        public void Check11CheckConvertResultToNormalStatusNotValidOk()
        {
            //SETUP  
            var statusWithResult = new SuccessOrErrors<string>();
            statusWithResult.AddSingleError("There was an error");

            //ATTEMPT
            var status = statusWithResult as ISuccessOrErrors;

            //VERIFY
            status.IsValid.ShouldEqual(false);
        }


        [Test]
        public void Check15CheckConvertNormalStatusToStatusValidOk()
        {
            //SETUP  
            var statusWithResult = new SuccessOrErrors();
            statusWithResult.SetSuccessMessage("This is a message");

            //ATTEMPT
            var status = SuccessOrErrors<string>.ConvertNonResultStatus(statusWithResult);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.Result.ShouldEqual(null);
        }

        [Test]
        public void Check16CheckConvertResultToNormalStatusNotValidOk()
        {
            //SETUP  
            var statusWithResult = new SuccessOrErrors();
            statusWithResult.AddSingleError("error");

            //ATTEMPT
            var status = SuccessOrErrors<string>.ConvertNonResultStatus(statusWithResult);

            //VERIFY
            status.IsValid.ShouldEqual(false);
        }

        [Test]
        public void Check16CheckConvertResultStatusToAnotherResultStatusValidOk()
        {
            //SETUP  
            var statusWithResult = new SuccessOrErrors<int>();
            statusWithResult.SetSuccessWithResult(1,"This is a message");

            //ATTEMPT
            var status = SuccessOrErrors<string>.ConvertNonResultStatus(statusWithResult);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.Result.ShouldEqual(null);
        }

        [Test]
        public void Check17CheckConvertIntToAnotherResultStatusFail()
        {
            //SETUP  

            //ATTEMPT
            var ex = Assert.Throws<ArgumentNullException>(() => SuccessOrErrors<string>.ConvertNonResultStatus("string"));

            //VERIFY
            ex.Message.ShouldStartWith("The status parameter was not derived from a type thta supported ISuccessOrErrors.");
        }
    }
}
