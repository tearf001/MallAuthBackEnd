using MallAuth.AuthProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MallAuth.Controllers
{
    /// <summary>
    /// 本控制器为测试控制器,附注全部调用格式   
    /******************************************************************************
         
     * 示例
     * --In this example, “/api/v1/products” would be routed to a different controller than “/api/v2/products”.
     
     * /api/v1/products       
     * /api/v2/products
      
     * Overloaded URI segments           
     * --In this example, “1” is an order number, but “pending” maps to a collection.

     * /orders/1           
     * /orders/pending
    
     * Mulitple parameter types               
     * --In this example, “1” is an order number, but “2013/06/16” specifies a date.   
     
     * /orders/1            
     * /orders/2013/06/16
     
     * [Route("customers/{customerId}/orders")]         
     * public IEnumerable<Order> FindOrdersByCustomer(int customerId) { ... }
     
     * [Route("customers/{customerId}/orders/{orderId}")]
     * public Order GetOrderByCustomer(int customerId, int orderId) { ... }
            
         
     * {constraints 见 http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2}
         
     * You can apply multiple constraints to a parameter, separated by a colon.
         
     * 如 {x:max(10)} -> users/{id:int:min(1)}          
     * 如 {x:regex(^\d{3}-\d{3}-\d{4}$)} -> phone/{number:regex(^\d{3}-\d{3}-\d{4}$)}
     
     * 时间类型: [Route("{*date:datetime}")]  // wildcard
       public HttpResponseMessage Get(DateTime date) { ... }
     
     * 自定义:Custom Route Constraints
     * 参数缺省值
     * [Route("api/books/locale/{lcid:int?}")]    
     * public IEnumerable<Book> GetBooksByLocale(int lcid = 1033) { ... }
         ******************************************************************************/
    /// </summary>
    [RoutePrefix("api/TestResource")]
    public class TestResourceController : ApiController
    {
        //[Thinktecture.IdentityModel.Authorization.WebApi.ClaimsAuthorize] 第三方鉴权
        [MyClaimAuthorizationFilter]
        [Route("")]//同前缀
        //[Route("sub/{rscId}/..")] 可变匹配
        //[Route("~/api/authors/{authorId:int}/books")] 覆盖前缀路由
        public IHttpActionResult Get()
        {
            return Ok(Order.CreateOrders());
        }

    }


    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public Boolean IsDelivered { get; set; }

        public static List<Order> CreateOrders()
        {
            List<Order> OrderList = new List<Order> 
            {
                new Order {OrderID = 10248, CustomerName = "荆州用户", City = "荆州", IsDelivered = true },
                new Order {OrderID = 10249, CustomerName = "武汉用户", City = "武汉", IsDelivered = false},
                new Order {OrderID = 10250, CustomerName = "水果湖用户", City = "武汉", IsDelivered = false },
                new Order {OrderID = 10251, CustomerName = "某淘宝人", City = "广水", IsDelivered = false},
                new Order {OrderID = 10252, CustomerName = "李小姐", City = "黄石", IsDelivered = true}
            };

            return OrderList;
        }
    }
}