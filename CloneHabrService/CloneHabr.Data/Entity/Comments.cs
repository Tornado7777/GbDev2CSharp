using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Data.Entity
{
    [Table("Comments")]
    public class Comments
    {
        [ForeignKey("IdComment")]
        public int CommentsId { get; set; }
        public string TextComments { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreationTimeComments { get; set; }

        [InverseProperty(nameof(Account.Comment))]
        public virtual ICollection<Account> Sessions { get; set; } = new HashSet<Account>();

    }
}
