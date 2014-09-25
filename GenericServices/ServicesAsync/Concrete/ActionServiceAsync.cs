#region licence
// The MIT License (MIT)
// 
// Filename: ActionServiceAsync.cs
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
using System;
using System.Threading.Tasks;
using GenericServices.Actions.Internal;
using GenericServices.Core;

namespace GenericServices.ServicesAsync.Concrete
{
    public class ActionServiceAsync<TActionOut, TActionIn> : IActionServiceAsync<TActionOut, TActionIn> 
    {
        private readonly IGenericServicesDbContext _db;
        private readonly IActionAsync<TActionOut, TActionIn> _actionToRun;

        public ActionServiceAsync(IGenericServicesDbContext db, IActionAsync<TActionOut, TActionIn> actionToRun)
        {
            if (actionToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the action. Check you have added IActionSync<TActionOut, TActionIn> to the classe's interface.");
            _db = db;
            _actionToRun = actionToRun;
        }

        /// <summary>
        /// This runs a action that returns a result. 
        /// </summary>
        /// <param name="actionData">Data that the action takes in to undertake the action</param>
        /// <returns>A Task containing status, which has a result if Valid</returns>
        public async Task<ISuccessOrErrors<TActionOut>> DoActionAsync(TActionIn actionData)
        {

            try
            {
                var status = await _actionToRun.DoActionAsync(actionData);
                return status.AskedToSaveChanges(_actionToRun)
                    ? await status.SaveChangesAttemptAsync(actionData, _db)
                    : status;
            }
            finally
            {
                var disposable = _actionToRun as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }

    //---------------------------------------------------------------------------
    //DTO version

    public class ActionServiceAsync<TActionOut, TActionIn, TDto> : IActionServiceAsync<TActionOut, TActionIn, TDto>
        where TActionIn : class, new()
        where TDto : EfGenericDtoAsync<TActionIn, TDto>, new()
    {

        private readonly IGenericServicesDbContext _db;
        private readonly IActionAsync<TActionOut, TActionIn> _actionToRun;

        public ActionServiceAsync(IGenericServicesDbContext db, IActionAsync<TActionOut, TActionIn> actionToRun)
        {
            if (actionToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the action. Check you have added IActionSync<TActionOut, TActionIn> to the classe's interface.");
            _db = db;
            _actionToRun = actionToRun;
        }

        /// <summary>
        /// This runs an action that does not write to the database. 
        /// It first converts the dto to the TActionIn format and then runs the action
        /// </summary>
        /// <param name="dto">The dto to be converted to the TActionIn class</param>
        /// <returns>A Task containing status, which has a result if Valid</returns>
        public async Task<ISuccessOrErrors<TActionOut>> DoActionAsync(TDto dto)
        {
            ISuccessOrErrors<TActionOut> status = new SuccessOrErrors<TActionOut>();

            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoActionWithoutValidate))
                return status.AddSingleError("Running an action is not setup for this data.");

            var actionInData = new TActionIn();
            var nonResultStatus = await dto.CopyDtoToDataAsync(_db, dto, actionInData); //convert Tdto into TActionIn format
            if (!nonResultStatus.IsValid) 
                return SuccessOrErrors<TActionOut>.ConvertNonResultStatus( nonResultStatus);

            try
            {
                status = await _actionToRun.DoActionAsync(actionInData);
                return status.AskedToSaveChanges(_actionToRun)
                    ? await status.SaveChangesAttemptAsync(actionInData, _db)
                    : status;
            }
            finally
            {
                var disposable = _actionToRun as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TDto> ResetDtoAsync(TDto dto)
        {
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                await dto.SetupSecondaryDataAsync(_db, dto);

            return dto;
        }
    }

}