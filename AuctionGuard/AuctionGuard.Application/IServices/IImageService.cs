using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile imageFile);
    }
}
