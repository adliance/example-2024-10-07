namespace Example.Web.Models.Database;

public class Registration
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime CreatedUtc { get; set; }
}
