#region licence
// The MIT License (MIT)
// 
// Filename: CreateSetupService.cs
// Date Created: 2014/07/22
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
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.Services.Concrete
{

    public class CreateSetupService : ICreateSetupService
    {

        private readonly IGenericServicesDbContext _db;

        public CreateSetupService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <typeparam name="TDto">The type of the data to output. This must be EfGeneric Dto</typeparam>
        /// <returns>The dto with any secondary data filled in</returns>
        public TDto GetDto<TDto>() where TDto : class
        {
            var service = DecodeToService<CreateSetupService>.CreateCorrectService<TDto>(WhatItShouldBe.SyncSpecificDto, _db);
            return service.GetDto();
        }
    }

    public class CreateSetupService<TData, TDto> : ICreateSetupService<TData, TDto>
        where TData : class
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        private readonly IGenericServicesDbContext _db;

        public CreateSetupService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <returns>A TDto which has had the SetupSecondaryData method called on it</returns>
        public TDto GetDto()
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                dto.SetupSecondaryData(_db, dto);

            return dto;
        }
    }
}
