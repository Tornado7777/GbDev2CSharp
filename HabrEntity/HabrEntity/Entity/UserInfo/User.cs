using System;

namespace HabrEntity.Entity
{
    public class User
    {
        internal int _Id { get; private set; }
        private string _Patronymic { get; set; }
        private string _LastName { get; set; }
        private string _FirstName { get; set; }

        private DateTime _BirthDay { get; set; }

        private string _Email { get; set; }
        private string _Telephone { get; set; }
        private Adress _Adress { get; set; }

        public User(int Id, string Firstname, string Lastname, string Patronymic, string Email, string Phone, Adress Adress, DateTime BirthDay )
        {
            _Id= Id;
            _FirstName= Firstname;
            _LastName= Lastname;
            _Patronymic= Patronymic;
            _Email= Email;
            _Telephone= Phone;
            _Adress= Adress;
            _BirthDay = BirthDay;


        }
    }
}