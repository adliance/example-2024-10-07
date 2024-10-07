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
        var registration = new Registration
        {
            FirstName = viewModel.FirstName,
            LastName = viewModel.LastName,
            CreatedUtc = DateTime.UtcNow
        };
        db.Registrations.Add(registration);
        await db.SaveChangesAsync();
        viewModel.ShowSuccessMessage = true;

        logger.LogInformation($"Registration of {viewModel.FirstName} {viewModel.LastName} stored in database with ID {registration.Id}.");
        return viewModel;
    }
}
