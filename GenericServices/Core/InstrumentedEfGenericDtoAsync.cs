#region licence
// The MIT License (MIT)
// 
// Filename: InstrumentedEfGenericDtoAsync.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GenericServices.Core
{

    public abstract class InstrumentedEfGenericDtoAsync<TData, TDto> : EfGenericDtoAsync<TData, TDto>, ICheckIfWarnings
        where TData : class, new()
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        /// <summary>
        /// Used to surround calls with using to catch start/end time
        /// </summary>
        private class LogStartStop : IDisposable
        {
            private readonly InstrumentedEfGenericDtoAsync<TData, TDto> _callingClass;
            private readonly string _callingMethodName;

            public LogStartStop(InstrumentedEfGenericDtoAsync<TData, TDto> callingClass, [CallerMemberName] string callerName = "")
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

        public InstrumentedEfGenericDtoAsync()
        {
            _timer.Start();
        }

        internal InstrumentedEfGenericDtoAsync(InstrumentedOpFlags whereToFail)
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
        public IEnumerable<InstrumentedLog> LogOfCalls { get { return new ReadOnlyCollection<InstrumentedLog>(_logOfCalls); } }


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
        private void LogCaller( CallTypes callType = CallTypes.Point, [CallerMemberName] string callerName = "")
        {
            LogSpecificName(callerName, callType);
        }

        //---------------------------------------------------------------------
        //ICheckIfWarnings implementation

        /// <summary>
        /// This allows the user to control whether data should still be written even if warnings found
        /// </summary>
        public bool WriteEvenIfWarning { get { return _whereToFail == InstrumentedOpFlags.ForceActionWarnWithWrite; } }

        //---------------------------------------------------------------------
        //overridden methods

        protected internal override IQueryable<TDto> BuildListQueryUntracked(IGenericServicesDbContext context)
        {
            using (new LogStartStop( this))
                return base.BuildListQueryUntracked(context);
        }

        protected internal override async Task SetupSecondaryDataAsync(IGenericServicesDbContext db, TDto dto)
        {
            LogCaller();
        }


        /// <summary>
        /// This returns the TData item that fits the key(s) in the DTO.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal protected override async Task<TData> FindItemTrackedAsync(IGenericServicesDbContext context)
        {
            using (new LogStartStop(this))
                return await context.Set<TData>().FindAsync(GetKeyValues(context));
        }

        protected internal override async Task<ISuccessOrErrors> CopyDataToDtoAsync(IGenericServicesDbContext context, TData source, TDto destination)
        {
            using (new LogStartStop(this))
            {
                if (_whereToFail.HasFlag(InstrumentedOpFlags.FailOnCopyDataToDto))
                    return new SuccessOrErrors().AddSingleError("Flag was set to fail in CopyDataToDtoAsync.");

                return await base.CopyDataToDtoAsync(context, source, destination);
            }
        }

        protected internal override async Task<ISuccessOrErrors> CopyDtoToDataAsync(IGenericServicesDbContext context, TDto source, TData destination)
        {
            using (new LogStartStop(this))
            {
                if (_whereToFail.HasFlag(InstrumentedOpFlags.FailOnCopyDtoToData))
                    return new SuccessOrErrors().AddSingleError("Flag was set to fail in CopyDtoToDataAsync.");

                //Use the below code to instrument the inner parts of the mapping 
                //CreateDtoToDataMapping();
                //LogSpecificName("After CreateMap");
                //Mapper.Map(source, destination);
                //return SuccessOrErrors.Success("Successful copy of data");

                return await base.CopyDtoToDataAsync(context, source, destination);
            }
        }

        protected internal override async Task<ISuccessOrErrors<TDto>> CreateDtoAndCopyDataInAsync(
            IGenericServicesDbContext context, Expression<Func<TData, bool>> predicate)
        {
            LogCaller(CallTypes.Start);
            var status = await base.CreateDtoAndCopyDataInAsync(context, predicate);
            if (status.IsValid)
            {
                var instDto = status.Result as InstrumentedEfGenericDtoAsync<TData, TDto>;
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
