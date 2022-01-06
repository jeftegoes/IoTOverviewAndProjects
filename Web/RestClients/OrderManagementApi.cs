using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Refit;
using Web.ViewModels;

namespace Web.RestClients
{
    public class OrderManagementApi : IOrderManagementApi
    {
        private IOrderManagementApi _orderManagementApi;
        private readonly IOptions<AppSettings> _settings;

        public OrderManagementApi(IOptions<AppSettings> settings,
                                  HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri($"{ settings.Value.OrdersApiUrl }/api");
            _orderManagementApi = RestService.For<IOrderManagementApi>(httpClient);
            _settings = settings;
        }

        public async Task<List<OrderViewModel>> GetOrders()
        {
            return await _orderManagementApi.GetOrders();
        }

        public async Task<OrderViewModel> GetOrderById(Guid orderId)
        {
            try
            {
                return await _orderManagementApi.GetOrderById(orderId);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}