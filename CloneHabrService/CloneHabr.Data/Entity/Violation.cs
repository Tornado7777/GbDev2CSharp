using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Data.Entity
{
    [Table("Violations")]
    public class Violation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TypeOfViolation { get; set; }

        public string Punishment { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime TimeOfViolation { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Duration { get; set; }

        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public User User { get; set; }

    }
}
