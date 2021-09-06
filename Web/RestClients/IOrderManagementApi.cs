using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using Web.ViewModels;

namespace Web.RestClients
{
    public interface IOrderManagementApi
    {
        [Get("/order")]
        Task<List<OrderViewModel>> GetOrders();

        [Get("/order/{orderId}")]
        Task<OrderViewModel> GetOrderById(Guid orderId);
    }
}