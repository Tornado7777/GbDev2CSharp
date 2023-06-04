using CloneHabr.Dto.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Dto.Requests
{
    public class UsersResponse
    {
        public UserStatus Status { get; set; }
        public List<UserDto> ListUserDto { get; set; }
    }
}
