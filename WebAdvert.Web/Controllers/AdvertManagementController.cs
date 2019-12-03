using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.Services;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private IFileUploader _fileUploader;

        public AdvertManagementController(IFileUploader fileUploader)
        {
            _fileUploader = fileUploader;
        }

        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var id = "11111";
                var fileName = "";
                if (imageFile != null)
                {
                    fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    var filePath = $"{id}/{fileName}";
                    try
                    {
                        using (var readStream = imageFile.OpenReadStream())
                        {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream).ConfigureAwait(false);
                            if (!result)
                                throw new Exception("Could not upload the image to file repository. Pleas e see the logs");
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                else
                    ModelState.AddModelError("NullImg", "Image cannot be null");
            }
            return View();
        }
    }
}
