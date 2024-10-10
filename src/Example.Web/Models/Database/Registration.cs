namespace Example.Web.Models.Database;

public class Registration
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime CreatedUtc { get; set; }
    public string EmailHash { get; set; } = "";
    public string EmailHashSalt { get; set; } = "";
}
