using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabrEntity.Entity
{
    internal class UserSite
    {
        private int _Id { get; set; }
        private User _User { get; set; }
        private Role _Role { get; set; }
        private DateTime _RegistrationDate { get; set; }
        private int _QuantityPublication { get; set; }
        private bool _Isbanned { get; set; }
      

        public UserSite(User User, Role Role, int QuantityPublication, bool Isbanned = false)
        {
            _Id = User._Id;
            _User = User;
            _Role = Role;
            _QuantityPublication = QuantityPublication;
            _Isbanned = Isbanned;
         
        }
    }
}
