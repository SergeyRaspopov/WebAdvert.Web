using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.ServiceClients.Models;
using WebAdvert.Web.Services;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private IFileUploader _fileUploader;
        private IAdvertApiClient _advertApiClient;
        private IMapper _mapper;

        public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertApiClient, IMapper mapper)
        {
            _fileUploader = fileUploader;
            _advertApiClient = advertApiClient;
            _mapper = mapper;
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
                var advert = await _advertApiClient.Create(_mapper.Map<ApiAdvert>(model));
                var fileName = "";
                if (imageFile != null)
                {
                    fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : advert.Id;
                    var filePath = $"{advert.Id}/{fileName}";
                    try
                    {
                        using (var readStream = imageFile.OpenReadStream())
                        {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream).ConfigureAwait(false);
                            if (!result)
                                throw new Exception("Could not upload the image to file repository. Please see the logs");
                        }

                        var confirmModel = new ApiConfirmAdvert()
                        {
                            Id = advert.Id,
                            FilePath = filePath,
                            Status = AdvertApi.Models.AdvertStatus.Active
                        };
                        var canConfirm = await _advertApiClient.Confirm(confirmModel);
                        if (!canConfirm)
                            throw new Exception($"Cannot confirm advert of id = {advert.Id}");
                        
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        var confirmModel = new ApiConfirmAdvert()
                        {
                            Id = advert.Id,
                            FilePath = filePath,
                            Status = AdvertApi.Models.AdvertStatus.Pending
                        };
                        var canConfirm = await _advertApiClient.Confirm(confirmModel);
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
