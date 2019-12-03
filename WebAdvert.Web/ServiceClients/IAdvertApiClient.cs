using AdvertApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<CreateAdvertResponse> Create(AdvertModel model);
    }
}
