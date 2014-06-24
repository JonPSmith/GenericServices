using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using GenericServices.Services;

namespace GenericServices.Core
{
    [Flags]
    public enum InstrumentedOpFlags { NormalOperation = 0, FailOnCopyDataToDto = 1, FailOnCopyDtoToData = 2,
        //these flags are not used in InstrumentedEfGenericDto, but inside unit tests
        ForceActionFail = 4, ForceActionWarnWithWrite = 8, ForceActionkWarnNoWrite = 16 }

    public abstract class InstrumentedEfGenericDto<TData, TDto> : EfGenericDto<TData, TDto>, ICheckIfWarnings
        where TData : class
        where TDto : EfGenericDto<TData, TDto>
    {
        /// <summary>
        /// Used to surround calls with using to catch start/end time
        /// </summary>
        private class LogStartStop : IDisposable
        {
            private readonly InstrumentedEfGenericDto<TData, TDto> _callingClass;
            private readonly string _callingMethodName;

            public LogStartStop(InstrumentedEfGenericDto<TData, TDto> callingClass, [CallerMemberName] string callerName = "")
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

        public InstrumentedEfGenericDto()
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
        //ICheckIfWarnings implementation

        /// <summary>
        /// This allows the user to control whether data should still be written even if warnings found
        /// </summary>
        public bool WriteEvenIfWarning { get { return _whereToFail == InstrumentedOpFlags.ForceActionWarnWithWrite; } }

        //---------------------------------------------------------------------
        //overridden methods

        protected internal override IQueryable<TDto> BuildListQueryUntracked(IDbContextWithValidation context)
        {
            using (new LogStartStop( this))
                return base.BuildListQueryUntracked(context);
        }

        protected internal override void SetupSecondaryData(IDbContextWithValidation db, TDto dto)
        {
            LogCaller();
        }

        protected internal override TData FindItemTracked(IDbContextWithValidation context)
        {
            using (new LogStartStop(this))
                return base.FindItemTracked(context);
        }

        protected internal override ISuccessOrErrors CopyDataToDto(IDbContextWithValidation context, TData source, TDto destination)
        {
            using (new LogStartStop(this))
            {
                if (_whereToFail.HasFlag(InstrumentedOpFlags.FailOnCopyDataToDto))
                    return new SuccessOrErrors().AddSingleError("Flag was set to fail here.");

                return base.CopyDataToDto(context, source, destination);
            }
        }

        protected internal override ISuccessOrErrors CopyDtoToData(IDbContextWithValidation context, TDto source, TData destination)
        {
            using (new LogStartStop(this))
            {
                if (_whereToFail.HasFlag(InstrumentedOpFlags.FailOnCopyDtoToData))
                    return new SuccessOrErrors().AddSingleError("Flag was set to fail here.");

                //Use the below code to instrument the inner parts of the mapping 
                //CreateDtoToDataMapping();
                //LogSpecificName("After CreateMap");
                //Mapper.Map(source, destination);
                //return SuccessOrErrors.Success("Successfull copy of data");

                return base.CopyDtoToData(context, source, destination);
            }
        }

        protected internal override TDto CreateDtoAndCopyDataIn(IDbContextWithValidation context, Expression<Func<TData, bool>> predicate)
        {
            LogCaller(CallTypes.Start);
            var newDto = base.CreateDtoAndCopyDataIn(context, predicate);
            var instDto = newDto as InstrumentedEfGenericDto<TData, TDto>;
            instDto._timer = _timer;
            instDto._logOfCalls = _logOfCalls;
            instDto._whereToFail = _whereToFail;
            instDto.LogCaller(CallTypes.End);
            return instDto as TDto;
        }





    }
}
