using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Refit;
using Web.ViewModels;

namespace Web.RestClients
{
    public class OrderManagementApi : IOrderManagementApi
    {
        private IOrderManagementApi _orderManagementApi;

        public OrderManagementApi(IConfiguration configuration, HttpClient httpClient)
        {
            var apiHostAndPort = configuration.GetSection("ApiServiceLocation")
                .GetValue<string>("OrdersApiLocation");

            httpClient.BaseAddress = new Uri($"https://{ apiHostAndPort }/api");
            _orderManagementApi = RestService.For<IOrderManagementApi>(httpClient);
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