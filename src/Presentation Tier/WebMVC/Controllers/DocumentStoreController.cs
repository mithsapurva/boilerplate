using Microsoft.AspNetCore.Mvc;


namespace Web.Controllers
{
    using Web.Models;

    public class DocumentStoreController : Controller
    {
        [HttpPost]
        public IActionResult UploadFile(FileViewModel inputModel)
        {

            return RedirectToAction("ListFiles");
        }
    }
}