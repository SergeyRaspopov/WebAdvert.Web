using AdvertApi.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients.Models;

namespace WebAdvert.Web
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<ApiAdvert, AdvertModel>().ReverseMap();
            CreateMap<ApiCreateAdvertResponse, CreateAdvertResponse>().ReverseMap();
            CreateMap<ApiConfirmAdvert, ConfirmAdvertModel>().ReverseMap();

            CreateMap<ApiAdvert, CreateAdvertViewModel>().ReverseMap();
        }
    }
}
