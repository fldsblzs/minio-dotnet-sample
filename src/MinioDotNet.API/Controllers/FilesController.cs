using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Minio;

namespace MinioDotNet.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private const string BucketName = "test";

        private readonly ILogger<FilesController> _logger;
        private readonly MinioClient _minioClient;

        public FilesController(
            ILogger<FilesController> logger,
            MinioClient minioClient)
        {
            _logger = logger;
            _minioClient = minioClient;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var filePath = Path.GetTempFileName();

            _logger.LogInformation($"Temp file name: '{filePath}'.");

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            _logger.LogInformation("File copied to stream.");

            var fileId = Guid.NewGuid().ToString();

            try
            {
                if (!await _minioClient.BucketExistsAsync(BucketName))
                    await _minioClient.MakeBucketAsync(BucketName);

                _logger.LogInformation("Bucket exists/created, uploading file...");

                await _minioClient.PutObjectAsync(BucketName, fileId, stream, stream.Length, file.ContentType);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }

            return Ok(new {FileId = fileId});
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var filePath = Path.GetTempFileName();
            _logger.LogInformation($"Temp file name: '{filePath}'.");

            await using var stream = System.IO.File.Create(filePath);
            await _minioClient.GetObjectAsync(
                BucketName, 
                fileId,
                (callbackStream) => callbackStream.CopyToAsync(stream));

            return File(stream, "application/kekw");
        }
    }
}