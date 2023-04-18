using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Data.Entity
{
    [Table("Violations")]
    public class Violations
    {
        [ForeignKey(nameof(Account))]
        public int ViolationsId { get; set; }
        public string TypeOfViolations { get; set; }

        public string Punishment { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime TimeOfViolation { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Duration { get; set; }

        [InverseProperty(nameof(Account.Violation))]
        public virtual ICollection<Account> Sessions { get; set; } = new HashSet<Account>();

    }
}
