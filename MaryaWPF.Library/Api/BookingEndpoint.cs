﻿using MaryaWPF.Library.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Api
{
    public class BookingEndpoint : IBookingEndpoint
    {
        private IAPIHelper _apiHelper;

        public BookingEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<BookingModel>> GetAll()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("admin/bookings"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<BookingModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<List<BookingModel>> GetAllBookingsByService(int id)
        {
            string uri = "admin/bookings/service/" + id;
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<BookingModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task RemoveBooking(int bookingId)
        {
            BookingModel data = new BookingModel { CancelDate = DateTime.Now };
            string uri = "admin/booking/cancel/" + bookingId;
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync<BookingModel>(uri, data))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
