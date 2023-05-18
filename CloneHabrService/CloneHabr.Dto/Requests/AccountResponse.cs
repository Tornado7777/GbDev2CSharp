using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Dto.Requests
{
    public class AccountResponse
    {
        public AccountStatus Status { get; set; }
        public AccountDto Account { get; set; }
    }
}
