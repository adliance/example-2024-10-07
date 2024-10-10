using System.Net;
using Adliance.Buddy.Crypto;
using Example.Web.Controllers;
using Example.Web.Factories.ViewModels.Home;
using Example.Web.Models.Database;
using Example.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Example.Web.Tests.Controllers;

public class HomeControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IServiceScope _scope;
    private readonly Db _db;
    private readonly IndexViewModelFactory _ivmf;
    private readonly HomeController _hc;

    public HomeControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Init();
        _scope = _factory.Services.CreateScope();
        _db = _scope.ServiceProvider.GetRequiredService<Db>(); // most tests need to check database, so provide it here already and easier to use in specific tests
        _ivmf = _scope.ServiceProvider.GetRequiredService<IndexViewModelFactory>();
        _hc = new HomeController();
    }

    public void Dispose()
    {
        _db.Dispose();
        _scope.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task Post_Will_Store_Registration_In_Database()
    {
        var firstname = "Some first name";
        var lastname = "Some last name";
        var email = "Some email";

        // send a raw POST here for true integration test
        using var httpClient = _factory.CreateClient();
        var response = await httpClient.PostAsync("/", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
            new("FirstName", firstname), new("LastName", lastname), new("Email", email)
        }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Single(await _db.Registrations.ToListAsync());

        var registration = await _db.Registrations.SingleAsync();
        Assert.Equal(firstname, registration.FirstName);
        Assert.Equal(lastname, registration.LastName);

        var emailHashed = Crypto.Hash(email, registration.EmailHashSalt);
        Assert.Equal(emailHashed, registration.EmailHash);
    }

    [Fact]
    public async Task Same_Names_Different_Emails_Insertion_In_Database()
    {
        var firstname = "Some first name";
        var lastname = "Some last name";
        List<string> emails = ["email1@example.com","email2@example.com", "email3@example.com"];

        // send a raw POST here for true integration test
        using var httpClient = _factory.CreateClient();

        foreach (var email in emails)
        {
            var response = await httpClient.PostAsync("/", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("FirstName", firstname), new("LastName", lastname), new("Email", email)
            }));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // 3 entries in database
        Assert.Equal((await _db.Registrations.ToListAsync()).Count, emails.Count);

        var registrations = await _db.Registrations.ToListAsync();

        for (var i = 0; i < registrations.Count; i++)
        {

            Assert.Equal(firstname, registrations[i].FirstName);
            Assert.Equal(lastname, registrations[i].LastName);

            var emailHashed = Crypto.Hash(emails[i], registrations[i].EmailHashSalt);
            Assert.Equal(emailHashed, registrations[i].EmailHash);
        }
    }

    [Fact]
    public async Task Different_Names_Different_Emails_Insertion_In_Database()
    {
        var firstnames = new[] { "John", "Emma", "Liam" };
        var lastnames = new[] { "Doe", "Smith", "Johnson" };
        var emails = new[] {"email1@example.com","email2@example.com", "email3@example.com"};

        // send a raw POST here for true integration test
        using var httpClient = _factory.CreateClient();

        for (var i = 0; i < firstnames.Length; i++)
        {
            var response = await httpClient.PostAsync("/", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("FirstName", firstnames[i]), new("LastName", lastnames[i]), new("Email", emails[i])
            }));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // 3 entries in database
        Assert.Equal((await _db.Registrations.ToListAsync()).Count, emails.Length);

        var registrations = await _db.Registrations.ToListAsync();

        for (var i = 0; i < registrations.Count; i++)
        {
            Assert.Equal(firstnames[i], registrations[i].FirstName);
            Assert.Equal(lastnames[i], registrations[i].LastName);

            var emailHashed = Crypto.Hash(emails[i], registrations[i].EmailHashSalt);
            Assert.Equal(emailHashed, registrations[i].EmailHash);
        }
    }

    [Fact]
    public async Task Duplicate_Emails_Insertion_In_Database()
    {
        var firstname = "Some first name";
        var lastname = "Some last name";
        var email = "Some email";

        // send a raw POST here for true integration test
        using var httpClient = _factory.CreateClient();
        var response = await httpClient.PostAsync("/", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
            new("FirstName", firstname), new("LastName", lastname), new("Email", email)
        }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // one entry in database
        Assert.Single(await _db.Registrations.ToListAsync());

        var registration = await _db.Registrations.SingleAsync();
        Assert.Equal(firstname, registration.FirstName);
        Assert.Equal(lastname, registration.LastName);

        var emailHashed = Crypto.Hash(email, registration.EmailHashSalt);
        Assert.Equal(emailHashed, registration.EmailHash);

        // try to insert same entry again.
        response = await httpClient.PostAsync("/", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
            new("FirstName", firstname), new("LastName", lastname), new("Email", email)
        }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // still only one entry - no duplicates
        Assert.Single(await _db.Registrations.ToListAsync());
    }

    [Fact]
    public async Task Missing_Required_Firstname_Field_Insertion_In_Database()
    {
        var lastname = "Some last name";
        var email = "Some email";

        using var httpClient = _factory.CreateClient();
        var response = await httpClient.PostAsync("/", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
            new("LastName", lastname), new("Email", email)
        }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // nothing was added, as firstname is required
        Assert.Empty(await _db.Registrations.ToListAsync());
    }

    [Fact]
    public async Task Missing_Required_Lastname_Field_Insertion_In_Database()
    {
        var firstname = "Some first name";
        var email = "Some email";

        using var httpClient = _factory.CreateClient();
        var response = await httpClient.PostAsync("/", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
            new("FirstName", firstname), new("Email", email)
        }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // nothing was added, as lastname is required
        Assert.Empty(await _db.Registrations.ToListAsync());
    }

    [Fact]
    public async Task Missing_Required_Email_Field_Insertion_In_Database()
    {
        var firstname = "Some first name";
        var lastname = "Some last name";

        using var httpClient = _factory.CreateClient();
        var response = await httpClient.PostAsync("/", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
            new("FirstName", firstname), new("LastName", lastname)
        }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // nothing was added, as email is required
        Assert.Empty(await _db.Registrations.ToListAsync());
    }

    [Fact]
    public async Task Valid_ViewModel_Check_Response_Messages()
    {

        var validViewModel = new IndexViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            EMail = "john.doe@example.com"
        };

        var result = await _hc.Index(_ivmf, validViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);

        var model = Assert.IsType<IndexViewModel>(viewResult.Model);

        // no duplicate email address - success message
        Assert.True(model.ShowSuccessMessage);
        Assert.False(model.ShowErrorMessage);
    }

    [Fact]
    public async Task Duplicate_ViewModel_Check_Response_Messages()
    {

        var validViewModel = new IndexViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            EMail = "john.doe@example.com"
        };

        var result = await _hc.Index(_ivmf, validViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);

        var model = Assert.IsType<IndexViewModel>(viewResult.Model);

        // no duplicate email address - success message
        Assert.True(model.ShowSuccessMessage);
        Assert.False(model.ShowErrorMessage);

        var duplicateViewModel = new IndexViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            EMail = "john.doe@example.com"
        };

        var resultDuplicate = await _hc.Index(_ivmf, duplicateViewModel);

        var viewResultDuplicate = Assert.IsType<ViewResult>(resultDuplicate);

        var modelDuplicate = Assert.IsType<IndexViewModel>(viewResultDuplicate.Model);

        // duplicate email address - error message
        Assert.False(modelDuplicate.ShowSuccessMessage);
        Assert.True(modelDuplicate.ShowErrorMessage);
    }
}
