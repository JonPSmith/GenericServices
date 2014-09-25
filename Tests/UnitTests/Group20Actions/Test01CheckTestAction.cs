#region licence
// The MIT License (MIT)
// 
// Filename: Test01CheckTestAction.cs
// Date Created: 2014/06/23
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
using NUnit.Framework;
using Tests.Actions;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group20Actions
{
    class Test01CheckTestAction
    {

        [Test]
        public void Check01RunActionSuccessOk()
        {
            //SETUP  
            var testAction = new EmptyTestAction(false);

            //ATTEMPT
            var data = new Tag {TagId = -123};
            var status = testAction.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(data.TagId);
            testAction.DisposeWasCalled.ShouldEqual(false);
        }

        [Test]
        public void Check05CheckDisposeCalledOk()
        {
            //SETUP  
            var testAction = new EmptyTestAction(false);

            //ATTEMPT
            testAction.Dispose();

            //VERIFY
            testAction.DisposeWasCalled.ShouldEqual(true);
        }
        
    }
}
