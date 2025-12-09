using ApiContracts.Enums;
using System.Text.RegularExpressions;

namespace Entities;

public class Driver : User
{
    public String McNumber { get; private set; } = "Stuffstuff";
    public DriverStatus Status { get; private set; } = DriverStatus.Available;
    public TrailerType Trailer_type { get; private set; } = TrailerType.Dry_van;
    public string Location_State { get; private set; } = "AL";
    public int Location_Zip_Code { get; private set; } = 35010;
    public new DriverCompanyRole CompanyRole { get; private set; } = DriverCompanyRole.Driver;

    public Driver() { }

    public class Builder : User.Builder
    {
        private String _mcNumber;
        private DriverStatus _status = DriverStatus.Available;
        private TrailerType _trailerType = TrailerType.Dry_van;
        private DriverCompanyRole _companyRole = DriverCompanyRole.Driver;

        private string _state = "NA";
        private int _zip = 10;
        
        public new Builder SetMcNumber(String mc)
        {
            _mcNumber = mc;
            return this;
        }

        public new Builder SetStatus(DriverStatus status)
        {
            _status = status;
            return this;
        }

        public new Builder SetTrailerType(TrailerType type)
        {
            _trailerType = type;
            return this;
        }

        public new Builder SetLocationState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("State cannot be empty.");

            // Must be 2 letters
         //   if (!Regex.IsMatch(state, "^[A-Za-z]{2}$"))
          //      throw new ArgumentException("State must be exactly 2 letters (A–Z).");

            _state = state.ToUpper();
            return this;
        }

        public new Builder SetLocationZip(int zip)
        {
              if (zip <= 0 || zip >= 100000)
                throw new ArgumentException("ZIP code must be between 1 and 99999.");
              _zip = zip;
            return this;
        }
        public new Builder SetCompanyRole(DriverCompanyRole role)
        {
            this._companyRole = role;
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

        public new Builder SetPhotoUrl(string url)
        {
            base.SetPhotoUrl(url);
            return this;
        }

        public new Builder SetPassword(String password)
        {
            base.SetPassword(password);
            return this;
        }
        public new Driver Build()
        {
            User baseUser = base.Build(); 

            return new Driver
            {
                Id = baseUser.Id,
                FirstName = baseUser.FirstName,
                LastName = baseUser.LastName,
                Email = baseUser.Email,
                Role = baseUser.Role,
                PhoneNumber = baseUser.PhoneNumber,
                Password = baseUser.Password,
                PhotoUrl = baseUser.PhotoUrl,
                // driver-specific
                CompanyRole = _companyRole,
                McNumber = _mcNumber,
                Status = _status,
                Trailer_type = _trailerType,
                Location_State = _state,
                Location_Zip_Code = _zip
            };
        }
    }
}