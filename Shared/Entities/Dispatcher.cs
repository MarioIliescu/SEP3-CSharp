using ApiContracts.Enums;

namespace Entities;

public class Dispatcher : User
{
    public double Current_Rate { get; private set; }
    public new UserRole Role { get; private set; } = UserRole.DISPATCHER;

    public Dispatcher() { }

    public class Builder : User.Builder
    {
        private double _current_rate;
        
        public Builder SetCurrentRate(double cr)
        {
            _current_rate = cr;
            return this;
        }

        public new Builder SetRole(UserRole role)
        {
            base.SetRole(role);
            return this;
        }

        public new Builder SetFirstName(String firstName)
        {
            base.SetFirstName(firstName);
            return this;
        }

        public new Builder SetLastName(String lastName)
        {
            base.SetLastName(lastName);
            return this;
        }

        public new Builder SetPhoneNumber(String phoneNumber)
        {
            base.SetPhoneNumber(phoneNumber);
            return this;
        }

        public new Builder SetEmail(String email)
        {
            base.SetEmail(email);
            return this;
        }

        public new Builder SetId(int Id)
        {
            base.SetId(Id);
            return this;
        }

        public new Builder SetPassword(String password)
        {
            base.SetPassword(password);
            return this;
        }
        public new Dispatcher Build()
        {
            User baseUser = base.Build(); 

            return new Dispatcher()
            {
                // inherited
                Id = baseUser.Id,
                FirstName = baseUser.FirstName,
                LastName = baseUser.LastName,
                Email = baseUser.Email,
                PhoneNumber = baseUser.PhoneNumber,
                Password = baseUser.Password,
                Role = UserRole.DISPATCHER,
                // dispatcher-specific
                Current_Rate =  _current_rate
            };
        }
    }
}