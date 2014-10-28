#region licence
// The MIT License (MIT)
// 
// Filename: Test01SuccessOrErrors.cs
// Date Created: 2014/05/21
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
using GenericServices.Core;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group02Validation
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
            var ex = Assert.Throws<InvalidOperationException>(() => status.Errors.Any());

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

        [Test]
        public void Check20SingleErrorAsHtmlOk()
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
        public void Check21SingleParamErrorAsHtmlOk()
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
        public void Check23MultipleErrorsAsHtmlOk()
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
