namespace Entities;

public class Company
{
    public string McNumber { get; private set; }
    public string CompanyName { get; private set; }
    public int Id { get; set; } = 0;

    private Company() { } // private constructor, only builder can create

    // Builder inner class
    public class Builder
    {
        private string _mcNumber = "DEFAULTVAL";
        private string _companyName = "Default Name";
        private int _id = 0;

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

        public Builder SetId(int id)
        {
            if (id < 0) 
                throw new ArgumentException("Id cannot be negative");
            _id = id;
            return this;
        }

        public Company Build()
        {
            return new Company
            {
                McNumber = _mcNumber,
                CompanyName = _companyName,
                Id = _id
            };
        }
    }
}