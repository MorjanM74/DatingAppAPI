using System.Net.Http.Headers;
using System.Text.Json;
using API.Helpers;

namespace API.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage
        , int totalItems, int totalPages)
    {

        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
            
        var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
        response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
}