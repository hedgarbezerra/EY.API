using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Shared.Contracts
{
    /// <summary>
    /// Provides methods for handling URLs from HTTP requests.
    /// </summary>
    public interface IUrlHelper
    {
        /// <summary>
        /// Gets the display URL from the specified HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request to generate the URL from.</param>
        /// <returns>A string representation of the display URL.</returns>
        string GetDisplayUrl(HttpRequest request);
    }
}
