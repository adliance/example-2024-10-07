using Adliance.Buddy.Crypto;
using Example.Web.Models.Database;
using Example.Web.ViewModels.Home;

namespace Example.Web.Factories.ViewModels.Home;

public class IndexViewModelFactory(ILogger<IndexViewModelFactory> logger, Db db)
{
    public IndexViewModel Create()
    {
        return new IndexViewModel();
    }

    public async Task<IndexViewModel> HandleRegistrationAsync(IndexViewModel viewModel)
    {

        if (EmailExists(viewModel.EMail))
        {
            viewModel.ShowErrorMessage = true;

            logger.LogInformation($"Registration of {viewModel.FirstName} {viewModel.LastName} ({viewModel.EMail}) failed.");
        }
        else
        {
            var hash = Crypto.Hash(viewModel.EMail, out var salt);

            var registration = new Registration
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                CreatedUtc = DateTime.UtcNow,
                EmailHash = hash,
                EmailHashSalt = salt
            };

            db.Registrations.Add(registration);
            await db.SaveChangesAsync();
            viewModel.ShowSuccessMessage = true;

            logger.LogInformation($"Registration of {viewModel.FirstName} {viewModel.LastName} ({viewModel.EMail}) stored in database with ID {registration.Id}.");
        }

        return viewModel;
    }

    private bool EmailExists(string email)
    {
        foreach (var reg in db.Registrations.ToList())
        {
            var hash = Crypto.Hash(email, reg.EmailHashSalt);

            if (hash.Equals(reg.EmailHash))
            {
                return true;
            }
        }
        return false;
    }
}
