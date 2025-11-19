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

                // forced DISPATCHER role
                Role = UserRole.DISPATCHER,

                // dispatcher-specific
                Current_Rate =  _current_rate
            };
        }
    }
}
