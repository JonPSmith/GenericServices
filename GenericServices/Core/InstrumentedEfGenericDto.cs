#region licence
// The MIT License (MIT)
// 
// Filename: InstrumentedEfGenericDto.cs
// Date Created: 2014/06/24
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace GenericServices.Core
{
    [Flags]
    public enum InstrumentedOpFlags
    {
        NormalOperation = 0, FailOnCopyDataToDto = 1, FailOnCreateDataFromDto = 2, FailOnUpdateDataFromDto = 4, 
        //these flags are not used in InstrumentedEfGenericDto, but inside unit tests
        ForceActionFail = 16, ForceActionWarnWithWrite = 32, ForceActionkWarnNoWrite = 64 }

    public abstract class InstrumentedEfGenericDto<TEntity, TDto> : EfGenericDto<TEntity, TDto>
        where TEntity : class, new()
        where TDto : EfGenericDto<TEntity, TDto>, new()
    {
        /// <summary>
        /// Used to surround calls with using to catch start/end time
        /// </summary>
        private class LogStartStop : IDisposable
        {
            private readonly InstrumentedEfGenericDto<TEntity, TDto> _callingClass;
            private readonly string _callingMethodName;

            public LogStartStop(InstrumentedEfGenericDto<TEntity, TDto> callingClass, [CallerMemberName] string callerName = "")
            {
                _callingClass = callingClass;
                _callingMethodName = callerName;

                _callingClass.LogSpecificName(_callingMethodName, CallTypes.Start);
            }

            public void Dispose()
            {
                _callingClass.LogSpecificName(_callingMethodName, CallTypes.End);
            }
        }

        //private fields

        private  Stopwatch _timer = new Stopwatch();

        private Collection<InstrumentedLog> _logOfCalls = new Collection<InstrumentedLog>();

        private InstrumentedOpFlags _whereToFail;

        //--------------------------------------------------
        //ctors

        protected InstrumentedEfGenericDto()
        {
            _timer.Start();
        }

        internal InstrumentedEfGenericDto(InstrumentedOpFlags whereToFail)
            : this()
        {
            _whereToFail = whereToFail;
        }

        //-----------------------------------------------------
        //properties to get instrumented results

        /// <summary>
        /// Instrumentation property which returns a list of call points as a comma delimited string.
        /// For start/end call points it only returns one entry.
        /// </summary>
        public string FunctionsCalledCommaDelimited 
        { 
            get { return string.Join(",", _logOfCalls.Where(x => x.CallType != CallTypes.End).Select(x => x.CallPoint));} 
        }


        /// <summary>
        /// Instrumentation property which returns a list of instrumented call points with the time since the dto was created
        /// </summary>
        public ReadOnlyCollection<InstrumentedLog> LogOfCalls { get { return new ReadOnlyCollection<InstrumentedLog>(_logOfCalls); } }


        //---------------------------------------------------------------------
        //public methods for logging and getting results

        /// <summary>
        /// Instrumentation method which allows a specific point to be logged with a given name
        /// </summary>
        /// <param name="callPoint"></param>
        /// <param name="callType">defaults to Point</param>
        public void LogSpecificName(string callPoint, CallTypes callType = CallTypes.Point)
        {
            _logOfCalls.Add(new InstrumentedLog(callPoint, callType, _timer.ElapsedMilliseconds));
        }

        /// <summary>
        /// Thsi will log the name of the calling method
        /// </summary>
        /// <param name="callType">defaults to Point</param>
        /// <param name="callerName">Do not use. Filled in by system with the calling method name</param>
        public void LogCaller( CallTypes callType = CallTypes.Point, [CallerMemberName] string callerName = "")
        {
            LogSpecificName(callerName, callType);
        }

        //---------------------------------------------------------------------
        //overridden methods

        protected internal override IQueryable<TDto> ListQueryUntracked(IGenericServicesDbContext context)
        {
            using (new LogStartStop( this))
                return base.ListQueryUntracked(context);
        }

        protected internal override void SetupSecondaryData(IGenericServicesDbContext db, TDto dto)
        {
            LogCaller();
        }

        protected internal override TEntity FindItemTrackedForUpdate(IGenericServicesDbContext context)
        {
            using (new LogStartStop(this))
                return base.FindItemTrackedForUpdate(context);
        }

        protected internal override ISuccessOrErrors<TEntity> CreateDataFromDto(IGenericServicesDbContext context, TDto source)
        {
            using (new LogStartStop(this))
            {
                if (_whereToFail.HasFlag(InstrumentedOpFlags.FailOnCreateDataFromDto))
                    return SuccessOrErrors<TEntity>.ConvertNonResultStatus(new SuccessOrErrors().AddSingleError("Flag was set to fail in CreateDataFromDto."));

                return base.CreateDataFromDto(context, source);
            }
        }

        protected internal override ISuccessOrErrors UpdateDataFromDto(IGenericServicesDbContext context, TDto source, TEntity destination)
        {
            using (new LogStartStop(this))
            {
                if (_whereToFail.HasFlag(InstrumentedOpFlags.FailOnUpdateDataFromDto))
                    return new SuccessOrErrors().AddSingleError("Flag was set to fail in UpdateDataFromDto.");

                return base.UpdateDataFromDto(context, source, destination);
            }
        }

        /// <summary>
        /// This copies an existing TEntity into a new the dto using a Lambda expression to define the where clause
        /// It copies TEntity properties into all TDto properties that have accessable setters, i.e. not private
        /// </summary>
        /// <returns>status. If valid result is dto. Otherwise null if not found</returns>
        internal protected override ISuccessOrErrors<TDto> DetailDtoFromDataIn(IGenericServicesDbContext context,
            Expression<Func<TEntity, bool>> predicate)
        {
            LogCaller(CallTypes.Start);
            var status = base.DetailDtoFromDataIn(context, predicate);
            if (status.IsValid)
            {
                var instDto = status.Result as InstrumentedEfGenericDto<TEntity, TDto>;
                instDto._timer = _timer;
                instDto._logOfCalls = _logOfCalls;
                instDto._whereToFail = _whereToFail;
                instDto.LogCaller(CallTypes.End);            
                return new SuccessOrErrors<TDto>(instDto as TDto, status.SuccessMessage);
            }

            //else the dto is null so we can't turn it into a new instrumented dto
            return status;

        }

    }
}
