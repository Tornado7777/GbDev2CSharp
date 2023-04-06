using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public DateTime EndDateLocked { get; set; }
        public Account Account { get; set; } = null!;

        //связь один ко многим
        [InverseProperty(nameof(UserSession.User))]
        public virtual ICollection<UserSession> Sessions { get; set; } = new HashSet<UserSession>();

    }
}
