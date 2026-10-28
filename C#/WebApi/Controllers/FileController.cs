using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class FileController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
