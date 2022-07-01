using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelListingAPI.HotelListing.Models
{

    public record RequestParams
    {
        const int maxPageSize = 50;
        public int Page { get; set; } = 1;
        private int _size = 10;

        public int Size 
        {
            get
            {
                return _size;
            }
            set 
            {
                _size = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}