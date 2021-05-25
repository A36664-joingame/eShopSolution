using eShopeSolution.Utilities.Constants;
using eShopeSolution.Utilities.Exceptions;
using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public class ProductService : IProductService
    {
        // hiện tại là đang sử dụng theo DI đã được học ở khóa cơ bản rồi nha

        // đây là đang sử dụng cách 1 bên DI nhe mở khóa cơ bản ra mà xem
        private readonly EShopDbContext _context;// thằng này có tác dụng kết nối với database để ta thực hiện các chức năng bên dưới

        private readonly IStorageService _storageService;

        public ProductService(EShopDbContext context, IStorageService storageService) // phải refrnece từ tầng data để lấy EShopDBContext
        {
            _context = context; // thằng Di nó sẽ tự tiêm vào đây cho chúng ta và

            // tiêm thằng storService
            _storageService = storageService;
        }

        // Addviewcount theo id của Product
        public async Task AddViewCount(int ProductId)
        {
            var Product = await _context.Products.FindAsync(ProductId);
            Product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        // tạo ra sản phẩm theo từng request chuyền vào
        public async Task<int> Create(ProductCreateRequest request) // lấy của Manage nhe
        {
            // tạo một đối tượng
            var product = new Product()
            {
                // phần của Product
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,

                // phần của ProductTranslations
                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation()
                    {
                        Name=request.Name,
                        Description=request.Description,
                        Details=request.Details,
                        SeoAlias=request.SeoAlias,
                        SeoDescription=request.SeoDescription,
                        SeoTitle=request.SeoTitle,
                        LanguageId=request.LanguageId
                    }
                }
            };
            // thêm ảnh cho sản phẩm

            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption="Thumbnail image",
                        DateCreated=DateTime.Now,
                        FileSize=request.ThumbnailImage.Length,
                        ImagePath= await this.SaveFile(request.ThumbnailImage),
                        Isdefault=true,
                        SortOrder=1
                    }
                };
            }

            _context.Products.Add(product);

            // save bất đồng bộ nó sẽ được đa luồng nên có thể sử lý được nhiều Request cùng một lúc
            await _context.SaveChangesAsync(); // thay vì chờ Save song nó có thể nhả ra và thực hiện Request khác cùng một lúc
            return product.Id;
        }

        // xóa Product theo Id
        public async Task<int> Delete(int ProductId)
        {
            // xóa sản phẩm phải xóa cả ảnh của sản phẩm đấy vd: ta có Iphone7 mà xóa đi tất nhiên trong dữ liệu không được còn ảnh cảu iphone7

            // đây là xóa sản phẩm chứ không phải là số số lượng sản phẩm nhe

            var Product = await _context.Products.FindAsync(ProductId);
            if (Product == null) throw new EShopeException($"Cannot find a Product :{ProductId}");  // tạo một Project class Exception

            // tìm xem có thằng nào giống thứ muốn xóa không // nhơ phải xóa ảnh trước rồi mới xóa sản phẩm
            var images = _context.ProductImages.Where(x => x.ProductId == ProductId); // tất cả những ảnh có ProductId = Id của sản phẩm cần xóa thì ta sẽ xóa tấ vì một sản phẩm có nhiều ảnh

            // tát nhiên 1 sản phẩm có nhiều image lên phải xóa hết vì ta xóa sản phẩm
            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);  // xóa cả image tương ứng
            }

            // xóa ảnh song ta xóa sản phẩm
            _context.Products.Remove(Product);

            return await _context.SaveChangesAsync();
        }

        // phân trang
        public async Task<ApiResult<PagedResult<ProductVm>>> GetAllPaging(GetManageProductPagingRequest request)
        {
            // bước 1 :select join  ,, nhớ nhe giữ liệu có đủ mới cho query chỗ này
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId into ppt
                        from pt in ppt.DefaultIfEmpty() //left join
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        into ppic
                        from pic in ppic.DefaultIfEmpty() // leftjoin
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        into picc
                        from c in picc.DefaultIfEmpty() // leftjoin
                        where pt.LanguageId == request.LanguageId
                        select new { p, pt, pic, pi };
            // bước 3: filter theo điều kiện
            if (!string.IsNullOrEmpty(request.KeyWord))
                query = query.Where(x => x.pt.Name.Contains(request.KeyWord));
            if (request.CategoryId != null && request.CategoryId != 0)// có nghĩa là có tất cả các tìm kiếm nào
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);//một trong só nhữ thằng này thì mới được
            }
            // bước 3: paging
            int totalRow = await query.CountAsync(); // lấy ra tông số số dòng để phân trang
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)// nếu PageIndex=2 và PageSize=20 thì bỏ qua 10 chỉ lấy 10 bẩn ghi hiện lên ko lấy tất để phù hợp với PageSize
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.Description,
                    Details = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.SeoAlias,
                    SeoDescription = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.SeoDescription,
                    SeoTitle = x.pt == null ? SystemConstants.ProductConstants.NA : x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).ToListAsync(); // vì ta Async ở đây nên trên kia ta chỉ cần await để đẩy vào data là song  nhớ là ToListAsync nha vì bên PageRsult Item ta để là list
                                  // bước 4: selecet and Project
            var pagedResult = new PagedResult<ProductVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };

            if (pagedResult != null)
            {
                return new ApiSuccessResult<PagedResult<ProductVm>>(pagedResult);
            }

            return new ApiErrorResult<PagedResult<ProductVm>>();
        }

        public async Task<ProductVm> GetById(int productId, string languageId)
        {
            var product = await _context.Products.FindAsync(productId);
            // lấy ra thằng đầu tiên thảo mãn ,FirstOrDefaultAsync
            /*
               First hoặc FirstOrDefault nếu em nghĩ kết quả có thể là nhiều hơn 1 bản ghi
               SingleOrDefault khi không có phần tử nào nó sẽ trả về null còn nếu trả về 1 phần thử thì nó có giá trị, nếu 2 phần tử thì lỗi.
                Single thì nếu không có phần  tử nào nó sẽ lỗi luôn, chỉ có kết quả khi có 1 bản ghi thoả mãn
             */
            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == productId
            && x.LanguageId == languageId);

            var categories = await (from c in _context.Categories
                                    join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                                    join pic in _context.ProductInCategories
                                    on c.Id equals pic.CategoryId
                                    where pic.ProductId == productId && ct.LanguageId == languageId
                                    select ct.Name).ToListAsync();
            var productViewModel = new ProductVm()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Description = productTranslation != null ? productTranslation.Description : null,
                LanguageId = productTranslation.LanguageId,
                Details = productTranslation != null ? productTranslation.Details : null,
                Name = productTranslation != null ? productTranslation.Name : null,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                SeoAlias = productTranslation != null ? productTranslation.SeoAlias : null,
                SeoDescription = productTranslation != null ? productTranslation.SeoDescription : null,
                SeoTitle = productTranslation != null ? productTranslation.SeoTitle : null,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                Categories = categories,
                IsFeatured = product.IsFeatured
            };
            return productViewModel;
        }

        // bắt đầu Update vào  bảng ProductTransalation còn Product chỉ để query thôi
        public async Task<int> Update(ProductUpdateRequest request)
        {
            // đầu tiên tìm product theo id
            var product = await _context.Products.FindAsync(request.Id);

            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(
                x => x.ProductId == request.Id &&
                x.LanguageId == request.LanguageId);// lấy thông tin sản phẩm theo id và ngôn ngữ

            if (product == null || productTranslation == null)
            {
                throw new EShopeException($"Cannot find a Product with id :{request.Id}");
            }

            productTranslation.Name = request.Name;
            productTranslation.SeoAlias = request.SeoAlias;
            productTranslation.SeoDescription = request.SeoDescription;
            productTranslation.SeoTitle = request.SeoTitle;
            productTranslation.Description = request.Description;
            productTranslation.Details = request.Details;

            // update ảnh cho sản phẩm
            if (request.ThumbnailImage != null) // ThumbnailImage ở trong ProductUpdateRequest cũng được lấy theo request
            {
                // nhớ _context là đại diện cho database nên có thể where truy vấn như bình thường nhe
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.Isdefault == true && x.ProductId == request.Id); // tìm kiếm xem có thằng nào phù hợp không để update thay thế nó
                if (thumbnailImage != null)
                {
                    // nếu khác null ta mới update nó

                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    _context.ProductImages.Update(thumbnailImage);
                }
            }

            _context.ProductTranslations.Update(productTranslation);

            if (request.IsFeatured || !request.IsFeatured)
            {
                product.IsFeatured = request.IsFeatured;
                _context.Products.Update(product);
            }

            return await _context.SaveChangesAsync();
        }

        // bắt đầu update Price
        public async Task<bool> UpdatePrice(int ProductId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(ProductId);
            if (product == null) throw new EShopeException($"Cannot find a Product with id :{ProductId}");
            product.Price = newPrice;
            return await _context.SaveChangesAsync() > 0;// nếu >0 thì nó return true là thành công
        }

        public async Task<bool> UpdateStock(int ProductId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(ProductId);
            if (product == null) throw new EShopeException($"Cannot find a Product with id :{ProductId}");
            if (addedQuantity == 0)
            {
                product.Stock += 1;
                await _context.SaveChangesAsync();
                product.Stock -= 1;
                return await _context.SaveChangesAsync() > 0;
            }
            product.Stock += addedQuantity; // số lượng cộng với chất lượng
            return await _context.SaveChangesAsync() > 0;// nếu >0 thì nó return true là thành công
        }

        // các phương thức về Image

        public async Task<int> AddImage(ProductImageCreateRequest request)
        {
            // tạo ra một image để add
            var productImage = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                Isdefault = request.Isdefault,
                ProductId = request.ProductId,
                SortOrder = request.SortOrder
            };
            if (request.ImageFile != null)
            {
                // lấy địa chỉ file và kích thước file trong request
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync(); //
            return productImage.Id;
        }

        public async Task<int> UpDateImage(int imageId, ProductImageUpdateRequest request)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
                throw new EShopeException($"Cannot find an image with id:{imageId}");
            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }

            _context.ProductImages.Update(productImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<ProductImageVm> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
                throw new EShopeException($"Cannot find an image with id:{imageId}");
            var viewModel = new ProductImageVm()
            {
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                Id = image.Id,
                ImagePath = image.ImagePath,
                Isdefault = image.Isdefault,
                ProductId = image.ProductId,
                SortOrder = image.SortOrder
            };

            return viewModel;
        }

        public async Task<List<ProductImageVm>> GetListImages(int productId)
        {
            return await _context.ProductImages.Where(x => x.ProductId == productId)
                .Select(i => new ProductImageVm()
                {
                    Caption = i.Caption,
                    DateCreated = i.DateCreated,
                    FileSize = i.FileSize,
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    Isdefault = i.Isdefault,
                    ProductId = i.ProductId,
                    SortOrder = i.SortOrder
                }).ToListAsync();
        }

        public async Task<int> RemoveImage(int imageId)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
                throw new EShopeException($"Cannot find an image with id:{imageId}");
            _context.ProductImages.Remove(productImage);
            return await _context.SaveChangesAsync(); // chả về tất cả số bản ghi
        }

        // hàm để save Image
        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        public async Task<PagedResult<ProductVm>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request) // lấy của public nhe
        {
            // bước 1 :select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations
                        on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId

                        join c in _context.Categories on pic.CategoryId equals c.Id
                        where pt.LanguageId == languageId
                        select new { p, pt, pic };
            // bước 1: filter theo điều kiện

            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)// HasValue mặc đinh là true
            {
                //using System.linq
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);//một trong só nhữ thằng này thì mới được
            }
            // bước 3: paging
            int totalRow = await query.CountAsync(); // lấy ra tông số số dòng để phân trang   phải include using Microsoft.EntityFrameworkCore;
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)// nếu PageIndex=2 và PageSize=20 thì bỏ qua 10 chỉ lấy 10 bẩn ghi hiện lên ko lấy tất để phù hợp với PageSize
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                }).ToListAsync(); // vì ta Async ở đây nên trên kia ta chỉ cần await để đẩy vào data là song  nhớ là ToListAsync nha vì bên PageRsult Item ta để là list
                                  // bước 4: selecet and Project
            var pagedResult = new PagedResult<ProductVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            // lấy user
            var user = await _context.Products.FindAsync(id); // tìm thoe id mà gặp phát lấy ra luân rồi không tìm tiếp  // co nghĩa là chỉ 1
            if (user == null) // nếu user không tồn tại
            {
                return new ApiErrorResult<bool>($"sản phẩm với id {id} không tồn tại");
            }
            foreach (var category in request.Categories)
            {
                var productInCategory = await _context.ProductInCategories.FirstOrDefaultAsync(x =>
                x.CategoryId == int.Parse(category.Id)
                && x.ProductId == id);
                if (productInCategory != null && category.Selected == false)
                {
                    _context.ProductInCategories.Remove(productInCategory);
                }
                else if (productInCategory == null && category.Selected)
                {
                    await _context.ProductInCategories.AddAsync(new ProductInCategory()
                    {
                        CategoryId = int.Parse(category.Id),
                        ProductId = id
                    }); // ở đay là add từng role nên không có s nhe
                }
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }

        public async Task<List<ProductVm>> GetFeaturedProducts(string langugeId, int take)
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        into ppic
                        from pic in ppic.DefaultIfEmpty() // leftjoin
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where (pi.Isdefault)
                        join c in _context.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty() // leftjoin
                        where pt.LanguageId == langugeId && (pi == null || pi.Isdefault == true) && p.IsFeatured == true
                        select new { p, pt, pic, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).ToListAsync(); // vì ta Async ở đây nên trên kia ta chỉ cần await để đẩy vào data là song  nhớ là ToListAsync nha vì bên PageRsult Item ta để là list
                                  // bước 4: selecet and Project

            return data;
        }

        public async Task<List<ProductVm>> GetLatestProducts(string langugeId, int take)
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        into ppic
                        from pic in ppic.DefaultIfEmpty() // leftjoin
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where (pi.Isdefault)
                        join c in _context.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty() // leftjoin
                        where pt.LanguageId == langugeId && (pi == null || pi.Isdefault == true)
                        select new { p, pt, pic, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).ToListAsync(); // vì ta Async ở đây nên trên kia ta chỉ cần await để đẩy vào data là song  nhớ là ToListAsync nha vì bên PageRsult Item ta để là list
                                  // bước 4: selecet and Project

            return data;
        }

        public async Task<List<ProductVm>> GetRelatedProducts(int productId, string langugeId, int take)
        {
            var product = from p in _context.Products
                          join pc in _context.ProductInCategories on p.Id equals pc.ProductId
                          where (p.Id == productId)
                          select pc.CategoryId;
            int categoryId = product.FirstOrDefault();

            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        into ppic
                        from pic in ppic.DefaultIfEmpty() // leftjoin
                        where (pic.CategoryId == categoryId)
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where (pi.Isdefault)
                        join c in _context.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty() // leftjoin
                        where pt.LanguageId == langugeId && (pi == null || pi.Isdefault == true)
                        select new { p, pt, pic, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).ToListAsync(); // vì ta Async ở đây nên trên kia ta chỉ cần await để đẩy vào data là song  nhớ là ToListAsync nha vì bên PageRsult Item ta để là list
                                  // bước 4: selecet and Project

            return data;
        }

        public async Task<List<string>> GetImagePaths(int productId)
        {
            var imagePaths = await _context.ProductImages.Where(x => x.ProductId == productId).Select(x => x.ImagePath).ToListAsync();
            return imagePaths;
        }
    }
}