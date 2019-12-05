using System.Threading.Tasks;
using WebAdvert.Web.ServiceClients.Models;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<ApiCreateAdvertResponse> Create(ApiAdvert model);
        Task<bool> Confirm(ApiConfirmAdvert model);
    }
}
