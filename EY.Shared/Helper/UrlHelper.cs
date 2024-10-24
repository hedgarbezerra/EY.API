using EY.Shared.Attributes;
using EY.Shared.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Shared.Helper
{
    [BindInterface(typeof(IUrlHelper), ServiceLifetime.Singleton)]
    public class UrlHelper : IUrlHelper
    {
        public string GetDisplayUrl(HttpRequest request) => request.GetDisplayUrl();
    }
}
