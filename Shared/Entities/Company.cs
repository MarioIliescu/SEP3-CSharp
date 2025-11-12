namespace Entities;

public class Company
{
    public string McNumber { get; private set; }
    public string CompanyName { get; private set; }

    private Company() { } // private constructor, only builder can create

    // Builder inner class
    public class Builder
    {
        private string _mcNumber = "DEFAULTVAL";
        private string _companyName = "Default Name";

        public Builder SetMcNumber(string mcNumber)
        {
            if (string.IsNullOrEmpty(mcNumber))
                throw new ArgumentException("McNumber cannot be null or empty");
            if (mcNumber.Length != 10)
                throw new ArgumentException("McNumber must be 10 characters long");
            _mcNumber = mcNumber;
            return this; // fluent
        }

        public Builder SetCompanyName(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
                throw new ArgumentException("CompanyName cannot be null or empty");
            _companyName = companyName;
            return this;
        }
        

        public Company Build()
        {
            return new Company
            {
                McNumber = _mcNumber,
                CompanyName = _companyName,
            };
        }
    }
}