
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;

namespace FacturacionA4V.Infrastructure
{
    public interface IDriveFileService
    {
        Task<MemoryStream> DownloadFile();
        Task Upload(Stream stream);
    }

    public class GoogleDriveFileService : IDriveFileService
    {
        private readonly string _serviceAccountPath;
        private readonly string _fileId;

        public GoogleDriveFileService(string serviceAccountPath, string fileId)
        {
            _serviceAccountPath = serviceAccountPath;
            _fileId = fileId;
        }

        private DriveService CreateService()
        {
            var credential = GoogleCredential
                .FromFile(_serviceAccountPath)
                .CreateScoped(DriveService.Scope.Drive);

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "FacturacionA4V"
            });
        }

        public async Task<MemoryStream> DownloadFile()
        {
            var service = CreateService();
            var request = service.Files.Get(_fileId);

            var stream = new MemoryStream();
            await request.DownloadAsync(stream);
            stream.Position = 0;

            return stream;
        }

        public async Task Upload(Stream stream)
        {
            var service = CreateService();

            stream.Position = 0;

            var fileMetadata = new Google.Apis.Drive.v3.Data.File();

            var request = service.Files.Update(fileMetadata, _fileId, stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            await request.UploadAsync(); // BUG-003 fix: was missing await
        }
    }
}
