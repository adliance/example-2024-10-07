using Example.Web.Factories.ViewModels.Home;
using Example.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

namespace Example.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index([FromServices] IndexViewModelFactory factory)
    {
        return View(factory.Create());
    }

    [HttpPost]
    [IgnoreAntiforgeryToken] // yes, this should not be there, but for this simple example it makes our integration testing much easier
    public async Task<IActionResult> Index([FromServices] IndexViewModelFactory factory, IndexViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }
        return View(await factory.HandleRegistrationAsync(viewModel));
    }
}
