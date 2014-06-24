using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GenericServices;

namespace Tests.DataClasses.Concrete
{
    public class Tag : ICheckIfWarnings
    {
        public int TagId { get; set; }

        [MaxLength(64)]
        [Required]
        [RegularExpression(@"\w*", ErrorMessage = "The slug must not contain spaces or non-alphanumeric characters.")]
        public string Slug { get; set; }

        [MaxLength(128)]
        [Required]
        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }

        public override string ToString()
        {
            return string.Format("TagId: {0}, Name: {1}, Slug: {2}", TagId, Name, Slug);
        }

        [NotMapped]
        public bool WriteEvenIfWarning { get; private set; }

        public Tag()
        {
        }

        public Tag(bool writeEvenIfWarning)
        {
            WriteEvenIfWarning = writeEvenIfWarning;
        }
    }
}
