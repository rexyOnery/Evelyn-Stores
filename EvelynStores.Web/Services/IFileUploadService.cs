using System.IO;
using System.Threading.Tasks;

namespace EvelynStores.Web.Services;

public interface IFileUploadService
{
    Task<string?> UploadAsync(Stream stream, string fileName, string contentType);
}
