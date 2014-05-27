using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GenericServices.Concrete;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs.Concrete
{
    class SimplePostTagGradeDto : InstrumentedEfGenericDto<PostTagGrade, SimplePostTagGradeDto>, ISimplePostTagGradeDto
    {

        [Key]
        [Column(Order = 1)]
        public int PostId { get; internal set; }
        [ForeignKey("PostId")]
        public Post PostPart { get; internal set; }

        [Key]
        [Column(Order = 2)]
        public int TagId { get; internal set; }
        [ForeignKey("TagId")]
        public Tag TagPart { get; internal set; }

        public int Grade { get; set; }

        //--------------------------------
        //now the extra bits

        public string TagPartName { get; internal set; }

        public string PostPartTitle { get; internal set; }

        protected internal override ServiceFunctions SupportedFunctions
        {
            get { return ServiceFunctions.AllCrudButCreate | ServiceFunctions.DoesNotNeedSetup; }
        }
    }
}
