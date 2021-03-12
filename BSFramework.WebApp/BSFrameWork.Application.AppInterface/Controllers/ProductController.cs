using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    /// 产品接口
    /// </summary>
    [RoutePrefix("api/Product")]
    public class ProductController : BaseApiController
    {
        private static List<ProductModel> products = new List<ProductModel>
            {
                new ProductModel
                { Id = 1,
                    Name = "Civic",
                    Brand = "Honda",
                    Category = "Sedon"
                },
                new ProductModel
                {
                    Id = 3,
                    Name = "320",
                    Brand = "BMW",
                    Category = "Sedon"
                },
                new ProductModel
                {
                    Id = 2,
                    Name = "CRV",
                    Brand = "Honda",
                    Category = "SUV"
                }
            };

        /// <summary>
        /// 构建器
        /// </summary>
        public ProductController()
        {
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>详情</returns>
        /// <response code="404">无效的资源</response>
        /// <response code="500">未处理的服务器错误</response>
        [Route("{id}", Name = "Product")]
        [ResponseType(typeof(ProductModel))]
        public IHttpActionResult Get(int id)
        {
            var product = products.Find(x => x.Id == id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="category">类型</param>
        /// <param name="brand">品牌</param>
        /// <param name="pageSize">记录数</param>
        /// <param name="pageIndex">页</param>
        /// <returns>产品列表</returns>
        [Route("")]
        [ResponseType(typeof(List<ProductModel>))]
        public HttpResponseMessage Get(string category = null, string brand = null, int pageSize = 10, int pageIndex = 1)
        {
            var query = products.Where(x => true);
            if (!string.IsNullOrEmpty(category))
                query = query.Where(x => x.Category.Contains(category));
            if (!string.IsNullOrEmpty(brand))
                query = query.Where(x => x.Brand.Contains(brand));

            var total = query.Count();

            query = query.Skip(pageSize * (pageIndex - 1)).Take((pageSize));

            var data = query.ToList();
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, data);
            response.Headers.Add("X-Pagination-Total", total.ToString());

            return response;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">产品</param>
        /// <returns>产品</returns>
        /// <response code="201">成功</response>
        /// <response code="400">验证错误</response>
        /// <response code="500">未处理的服务器错误</response>
        [Route("")]
        [ResponseType(typeof(List<ProductModel>))]
        public IHttpActionResult Post(ProductModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (products.Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError(string.Empty, $"已存在 {model.Name}");
                return BadRequest(ModelState);
            }

            model.Id = products.Count + 1;

            products.Add(model);
            return CreatedAtRoute("Product", new { id = model.Id }, model);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>详情</returns>
        /// <response code="500">未处理的服务器错误</response>
        [Route("{id}")]
        [ResponseType(typeof(List<ProductModel>))]
        public IHttpActionResult Delete(int id)
        {
            var product = products.Find(x => x.Id == id);
            if (product == null)
                return Ok();

            products.RemoveAll(x => x.Id == id);

            return Ok(product);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="model">产品</param>
        /// <returns>no content</returns>
        /// <response code="404">无效的资源</response>
        /// <response code="400">验证错误</response>
        /// <response code="500">未处理的服务器错误</response>
        [Route("{id}")]
        public IHttpActionResult Put(int id, ProductModel model)
        {
            var product = products.Find(x => x.Id == id);
            if (product == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            product.Name = model.Name;
            product.Category = model.Category;
            product.Brand = model.Brand;

            return Ok();
        }

        /// <summary>
        /// 产品类型
        /// </summary>
        /// <returns>类型列表</returns>
        [HttpGet]
        [Route("Category")]
        public IHttpActionResult Categories()
        {
            return Ok(products.Select(x => x.Category).Distinct());
        }
    }

    /// <summary>
    /// 产品
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// 产品名
        /// </summary>
        [Required(ErrorMessage = "请输入产品名"),
            MinLength(2, ErrorMessage = "产品名至少需要2字符"),
            MaxLength(10, ErrorMessage = "产品名至多需要10字符")]
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Required(ErrorMessage = "请输入类型"),
            MinLength(2, ErrorMessage = "类型需要2字符"),
            MaxLength(10, ErrorMessage = "类型至多需要10字符")]
        public string Category { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        [Required(ErrorMessage = "请输入品牌"),
            MinLength(2, ErrorMessage = "品牌需要2字符"),
            MaxLength(10, ErrorMessage = "品牌至多需要10字符")]
        public string Brand { get; set; }
    }
}
