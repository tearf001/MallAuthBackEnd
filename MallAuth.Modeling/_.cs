using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection;
using log4net;
using System.Dynamic;
using Castle.DynamicProxy;
namespace MallAuth.Modeling
{

    public class MallCtxInterceptor : StandardInterceptor {

        protected override void PerformProceed(IInvocation invocation)
        {
            try
            {
                base.PerformProceed(invocation);
            }
            catch(Exception e) {
                //记录异常日志
                invocation.LogError(e);
            }
        }
        /// <summary>
        /// 执行之后的日志...
        /// </summary>
        /// <param name="invocation"></param>
        protected override void PostProceed(IInvocation invocation)
        {
            // todo
            // handler invocation.ReturnValue;
        }
    }
    public class _ {
        public static CDMAMallDbContext service
        {
            get
            {
                //动态代理
                ProxyGenerator g = new ProxyGenerator();
                return g.CreateClassProxy(typeof(CDMAMallDbContext), new MallCtxInterceptor()) as CDMAMallDbContext;                
            }
        }
    }

    #region 并发方式
    //public class _
    //{

    //    #region 全局
    //    private static StringBuilder Msg;
    //    private static AdapterQueue<CDMAMallDbContext> MallCtxQueue = new AdapterQueue<CDMAMallDbContext>();
    //    #endregion


    //    #region 商城管理

    //    public static ICDMAMallDbContext service
    //    {
    //        get
    //        {
    //            ProxyGenerator g = new ProxyGenerator();
    //            CDMAMallDbContext qt = MallCtxQueue.Dequeue();
    //            var service = g.CreateClassProxy(typeof(CDMAMallDbContext), new MallCtxInterceptor()) as ICDMAMallDbContext;
    //            return service as ICDMAMallDbContext;

    //        }
    //    }
    //    [Obsolete]
    //    public static List<getProductInfos_Result> getProductInfos_Result(int? productId)
    //    {            
    //        List<getProductInfos_Result> result = null;
    //        try
    //        {
    //            CDMAMallDbContext qt = MallCtxQueue.Dequeue();                
    //            result = qt.getProductInfos(productId).ToList();
    //            MallCtxQueue.Enqueue(qt);
    //        }
    //        catch (Exception ex)
    //        {
    //            MethodBase.GetCurrentMethod().LogError(ex, productId);
    //        }
    //        return result;
    //    }
    //    [Obsolete]
    //    public static List<getProductList_Result> getProductList_Result(int? subCategoryId)
    //    {
    //        return service.getProductList(subCategoryId).ToList();
    //        //List<getProductList_Result> result = null;
    //        //try
    //        //{
    //        //    CDMAMallDbContext qt = MallCtxQueue.Dequeue();
    //        //    result = qt.getProductList(subCategoryId).ToList();
    //        //    MallCtxQueue.Enqueue(qt);
    //        //}
    //        //catch (Exception ex)
    //        //{
    //        //    MethodBase.GetCurrentMethod().LogError(ex, subCategoryId);               
    //        //}
    //        //return result;
    //    }


    //} 
    /// <summary>
    /// 并发队列,先进先出
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class AdapterQueue<T> : ConcurrentQueue<T> where T : new()
    {
        public T Dequeue()
        { //TryDequeue 尝试移除并返回位于并发队列开头处的对象。
            T item;
            if (!this.TryDequeue(out item))
                item = new T();
            // item = System.Activator.CreateInstance<T>(); 
            return item;
        }
    }
    #endregion
    public static class _helper
    {

        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(_));
        /// <summary>
        /// 调用方式  MethodBase.GetCurrentMethod().LogError(ex, productId);
        /// </summary>
        /// <param name="method"></param>
        /// <param name="ex"></param>
        /// <param name="values"></param>
        public static void LogError(this MethodBase method, Exception ex, params object[] values)
        {
            wrap(ex, method, values);
        }
              
        public static void LogError(this IInvocation invoke, Exception ex)
        {
            var method = invoke.Method;
            var values = invoke.Arguments;
            wrap(ex, method, values);
        }

        private static void wrap(Exception ex, MemberInfo method, object[] values)
        {
            ParameterInfo[] parms = ((MethodBase)method).GetParameters();
            object[] namevalues = new object[2 * parms.Length];

            string msg = "--执行失败:[" + method.Name + "(";
            for (int i = 0, j = 0; i < parms.Length; i++, j += 2)
            {
                msg += "{" + j + "}={" + (j + 1) + "}, ";
                namevalues[j] = parms[i].Name;
                if (i < values.Length) namevalues[j + 1] = values[i];
            }
            msg += "exception=" + ex.Message + ")]--";
            //Console.WriteLine(string.Format(msg, namevalues));
            log.Warn(string.Format(msg, namevalues));
            throw ex;//继续抛出
        }

    }
   
}
