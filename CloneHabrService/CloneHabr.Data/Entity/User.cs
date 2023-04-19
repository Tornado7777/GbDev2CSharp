using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloneHabr.Data;
using CloneHabr.Data.Entity;

namespace CloneHabr.Data
{
    [Table("Users")]
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [StringLength(100)]
        public string Login { get; set; }

        [StringLength(100)]
        public string PasswordSalt { get; set; }

        [StringLength(100)]
        public string PasswordHash { get; set; }

        public bool Locked { get; set; }

        public DateTime? EndDateLocked { get; set; }

        
        [ForeignKey(nameof(Account))]
        public int? AccountId { get; set; }
        public Account Account { get; set; }
        public int RoleId { get; set; }

        //связь один ко многим
        [InverseProperty(nameof(UserSession.User))]
        public virtual ICollection<UserSession> Sessions { get; set; } = new HashSet<UserSession>();
        [InverseProperty(nameof(Comment.User))]
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        [InverseProperty(nameof(Violation.User))]
        public virtual ICollection<Violation> Violations { get; set; } = new HashSet<Violation>();

    }
}
