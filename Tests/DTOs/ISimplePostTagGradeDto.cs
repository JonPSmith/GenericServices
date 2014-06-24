using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using GenericServices.Core;
using GenericServices.Services;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs
{
    internal interface ISimplePostTagGradeDto
    {
        [Key]
        [Column(Order = 1)]
        int PostId { get; }

        [ForeignKey("PostId")]
        Post PostPart { get; }

        [Key]
        [Column(Order = 2)]
        int TagId { get; }

        [ForeignKey("TagId")]
        Tag TagPart { get; }

        int Grade { get; set; }
        string TagPartName { get; }
        string PostPartTitle { get; }
        string FunctionsCalledCommaDelimited { get; }
        ReadOnlyCollection<InstrumentedLog> LogOfCalls { get; }
        bool WriteEvenIfWarning { get; }
        void LogSpecificName(string callPoint, CallTypes callType = CallTypes.Point);
        void LogCaller( CallTypes callType = CallTypes.Point, [CallerMemberName] string callerName = "");

        /// <summary>
        /// Optional method that will setup any mapping etc. that are cached. This will will improve speed later.
        /// The GenericDto will still work without this method being called, but the first use that needs the map will be slower. 
        /// </summary>
        void CacheSetup();
    }
}