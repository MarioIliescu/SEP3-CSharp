namespace Entities;

public class Company
{
    public string McNumber { get; private set; } = "DEFAULTVAL";

    public void SetMcNumber(string mcNumber)
    {
        if (string.IsNullOrEmpty(mcNumber))
        {
            throw new ArgumentException("McNumber cannot be null or empty");
        }
        else if (mcNumber.Length != 10)
        {
            throw new ArgumentException("McNumber must be 10 characters long");
        }
        McNumber = mcNumber;
    }
    public string CompanyName { get; private set; } = "Default Name";
    public void SetCompanyName(string companyName)
    {
        if (string.IsNullOrEmpty(companyName))
        {
            throw new ArgumentException("CompanyName cannot be null or empty");
        }
        CompanyName = companyName;
    }
    public int Id { get; set; }
}