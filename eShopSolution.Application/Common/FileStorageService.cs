using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Common
{
    public class FileStorageService : IStorageService
    {
        private readonly string _userContentFolder;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";  // nó lưu vào user-content nhe

        /*vào project application sửa thẻ Itemgroup thành thế này
         * <ItemGroup>
	             <FrameworkReference Include="Microsoft.AspNetCore.App"/>
           </ItemGroup>
         *
         */

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            //Combine(kết hợp)
            _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, USER_CONTENT_FOLDER_NAME);
        }

        public string GetFileUrl(string fileName)
        {
            return $"/{USER_CONTENT_FOLDER_NAME}/{fileName}"; // lấy đường dẫn
        }

        public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName); // Combine(kết hợp) giữa _userContentFolder và fileName để tạo địa chỉ file
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);  // đọc byte từ stream hiện tại và viết chúng vào byte khác
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
    }
}