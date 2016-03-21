#region licence
// The MIT License (MIT)
// 
// Filename: Test03AGenericDtoSetup.cs
// Date Created: 2014/11/11
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
using GenericServices;
using NUnit.Framework;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group08CrudServices
{
    class Test03AGenericDtoSetup
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            GenericServicesConfig.ClearAutoMapperCache();
        }

        [SetUp]
        public void SetUp()
        {
            GenericServicesConfig.UseDelegateDecompilerWhereNeeded = true;
        }

        [Test]
        public void Test01SetupSimpleDto()
        {
            //SETUP

            //ATTEMPT
            var dto = new SimpleTagDto();

            //VERIFY
            dto.NeedsDecompile.ShouldEqual(false);
        }

        [Test]
        public void Test02SetupDelegateDecompileForced()
        {
            //SETUP

            //ATTEMPT
            var dto = new DelegateDecompileForced();

            //VERIFY
            dto.NeedsDecompile.ShouldEqual(true);
        }
  
        [Test]
        public void Test10SetupDelegateDecompilerPostDto()
        {
            //SETUP

            //ATTEMPT
            var dto = new DelegateDecompilePostDto();

            //VERIFY
            dto.NeedsDecompile.ShouldEqual(true);
        }

        [Test]
        public void Test11SetupDelegateDecompilerPostDto()
        {
            //SETUP
            GenericServicesConfig.UseDelegateDecompilerWhereNeeded = false;

            //ATTEMPT
            var dto = new DelegateDecompilePostDto();

            //VERIFY
            dto.NeedsDecompile.ShouldEqual(false);
        }

        [Test]
        public void Test20SetupDtoWithAssociation()
        {
            //SETUP

            //ATTEMPT
            var dto = new DelegateDecompileNeededPostDto();

            //VERIFY
            dto.NeedsDecompile.ShouldEqual(true);
        }


        [Test]
        public void Test21SetupDtoWithAssociationsArray()
        {
            //SETUP

            //ATTEMPT
            var dto = new TabNeededTagAndDelegatePostDto();

            //VERIFY
            dto.NeedsDecompile.ShouldEqual(true);
        }

        [Test]
        public void Test25SetupDtoWithAssociationThatNeedsDecompile()
        {
            //SETUP

            //ATTEMPT
            var dto = new SimpleTagDtoAssociatedToDecompileClass();

            //VERIFY
            dto.NeedsDecompile.ShouldEqual(true);
        }

        [Test]
        public void Test30CheckAssociatedMappingsBad()
        {

            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => new BadSpecialMappingDto());

            //VERIFY
            ex.Message.ShouldEqual("You have not supplied a class based on EfGenericDto to set up the mapping.");
        }

    }
}
