using ApiContracts.Enums;
using System.Text.RegularExpressions;

namespace Entities;

public class Driver : User
{
    public Company McNumber { get; private set; }
    public DriverStatus Status { get; private set; } = DriverStatus.available;
    public TrailerType Trailer_type { get; private set; }
    public string Location_State { get; private set; }
    public int Location_Zip_Code { get; private set; }
    public new UserRole Role { get; private set; } = UserRole.DRIVER;

    public Driver() { }

    public class Builder : User.Builder
    {
        private Company _mcNumber;
        private DriverStatus _status = DriverStatus.available;
        private TrailerType _trailerType = TrailerType.dry_van;

        private string _state = "NA";
        private int _zip = 10;
        
        public Builder SetMcNumber(Company mc)
        {
            _mcNumber = mc;
            return this;
        }

        public Builder SetStatus(DriverStatus status)
        {
            _status = status;
            return this;
        }

        public Builder SetTrailerType(TrailerType type)
        {
            _trailerType = type;
            return this;
        }

        public Builder SetLocationState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("State cannot be empty.");

            // Must be 2 letters
            if (!Regex.IsMatch(state, "^[A-Za-z]{2}$"))
                throw new ArgumentException("State must be exactly 2 letters (A–Z).");

            _state = state.ToUpper();
            return this;
        }

        public Builder SetLocationZip(int zip)
        {
            if (zip <= 0 || zip >= 100000)
                throw new ArgumentException("ZIP code must be between 1 and 99999.");

            _zip = zip;
            return this;
        }

        
        public new Driver Build()
        {
            User baseUser = base.Build(); 

            return new Driver
            {
                // inherited
                Id = baseUser.Id,
                FirstName = baseUser.FirstName,
                LastName = baseUser.LastName,
                Email = baseUser.Email,
                PhoneNumber = baseUser.PhoneNumber,
                Password = baseUser.Password,

                // forced DRIVER role
                Role = UserRole.DRIVER,

                // driver-specific
                McNumber = _mcNumber,
                Status = _status,
                Trailer_type = _trailerType,
                Location_State = _state,
                Location_Zip_Code = _zip
            };
        }
    }
}
