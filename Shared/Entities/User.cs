using System.Text.RegularExpressions;
using ApiContracts.Enums;

namespace Entities;

public class User
{
    public int Id { get; set; } = 0;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }

    public User()
    {
    }


    public class Builder
    {
        private int _id = 0;
        private string _firstName = "First";
        private string _lastName = "Last";
        private string _email = "first.last@gmail.com";
        private string _phoneNumber = "+23-456-7890";
        private string _password = "password123";
        private UserRole _role = UserRole.DRIVER;


        public Builder SetId(int id)
        {
            if (id < 0)
                throw new ArgumentException("Id cannot be negative");
            _id = id;
            return this;
        }

        public Builder SetFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException(
                    "First name cannot be null or empty.", nameof(firstName));

            if (firstName.Length < 2)
                throw new ArgumentException(
                    "First name must be at least 2 characters long.",
                    nameof(firstName));

            if (!Regex.IsMatch(firstName, "^[A-Za-z'-]+$"))
                throw new ArgumentException(
                    "First name contains invalid characters.",
                    nameof(firstName));

            _firstName = firstName;
            return this;
        }

        public Builder SetLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException(
                    "First name cannot be null or empty.", nameof(lastName));

            if (lastName.Length < 2)
                throw new ArgumentException(
                    "First name must be at least 2 characters long.",
                    nameof(lastName));

            if (!Regex.IsMatch(lastName, "^[A-Za-z'-]+$"))
                throw new ArgumentException(
                    "First name contains invalid characters.",
                    nameof(lastName));

            _lastName = lastName;
            return this;
        }
        
        public Builder SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Email format is invalid.", nameof(email));

            _email = email;
            return this;
        }
        
        public Builder SetPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));

            if (!Regex.IsMatch(phoneNumber, @"^\+?\d{1,3}([ -]?\d{2,4}){2,5}$"))
                throw new ArgumentException("Phone number format is invalid. Expected formats like +49 123 456 789 or +421-987-654-321.", nameof(phoneNumber));

            _phoneNumber = phoneNumber;
            return this;
        }
        
        public Builder SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.", nameof(password));
            if (password.Length > 24)
            {
                throw new ArgumentException("Password cannot exceed 50 characters.", nameof(password));
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
                throw new ArgumentException("Password must contain at least one uppercase letter.", nameof(password));

            if (!Regex.IsMatch(password, @"[a-z]"))
                throw new ArgumentException("Password must contain at least one lowercase letter.", nameof(password));

            if (!Regex.IsMatch(password, @"[0-9]"))
                throw new ArgumentException("Password must contain at least one digit.", nameof(password));

            _password = password;
            return this;
        }
        public Builder SetRole(UserRole role)
        {
            if (!Enum.IsDefined(typeof(UserRole), role))
                throw new ArgumentException("Invalid user role specified.", nameof(role));

            _role = role;
            return this;
        }
        
        public new User Build()
        {
            return new User
            {
                Id = _id,
                FirstName = _firstName,
                LastName = _lastName,
                Email = _email,
                PhoneNumber = _phoneNumber,
                Password = _password,
                Role = _role
            };
        }

    }
}