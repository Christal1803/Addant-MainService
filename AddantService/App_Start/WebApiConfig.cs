using AddantSDal.DAL;
using AddantSDAL.DAL;
using CertusWebAppService.App_Start;
using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Microsoft.Owin.Security.OAuth;

namespace AddantService
{
	public static class WebApiConfig
	{
        public static string GetAllowedOrgins()
        {
            var retval = string.Empty;
            try
            {
                retval = ConfigurationManager.AppSettings["orgins"].ToString();
            }
            catch { }
            return retval;
        }
        public static void Register(HttpConfiguration config)
		{


            //find and delete log file which is older than a month.
            config.EnableCors(new System.Web.Http.Cors.EnableCorsAttribute(GetAllowedOrgins(), "*", "*"));

   //         var cors = new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*");
			//config.EnableCors(cors);

			// Web API configuration and services

			config.SuppressDefaultHostAuthentication();
			config.Filters.Add(new HostAuthenticationFilter(Microsoft.Owin.Security.OAuth.OAuthDefaults.AuthenticationType));

			config.MapHttpAttributeRoutes();
			//config.Routes.MapHttpRoute(
			//    name: "DefaultApi",
			//    routeTemplate: "api/{controller}/{id}",
			//    defaults: new { id = RouteParameter.Optional }
			//);
			config.Routes.MapHttpRoute(
			  name: "DefaultApi",
			  routeTemplate: "api/{controller}/{id}",
			  defaults: new { id = RouteParameter.Optional }
		  );

			//to enable cors requesting
			config.EnableCors(new System.Web.Http.Cors.EnableCorsAttribute(ConfigurationManager.AppSettings["orgins"].ToString(), "*", "*"));
			var container = new UnityContainer();
			container.RegisterType<AddantSDal.DAL.ILoginRepository, AddantSDal.DAL.LoginRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<IEnquiryRepository, EnquiryRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<IBlogRepository, BlogRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<IPositionRepository, PositionRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<ICandidateRepository, CandidateRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<IAddantLifeRepository, AddantLifeRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<IUserRepository, UserRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<IEmbedCodeRepository, EmbedCodeRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<IEmailTemplateRepository, EmailTemplateRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<IEventCategoryRepository, EventCategoryRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<IUserRoleRepository, UserRoleRepository>(new HierarchicalLifetimeManager());
			container.RegisterType<IUserPrivilegeRepository, UserPrivilegeRepository>(new HierarchicalLifetimeManager());


			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));


			config.DependencyResolver = new UnityResolver(container);
			DeleteOldLog();
		}

		public static void DeleteOldLog()
		{
			Directory.GetFiles(HttpContext.Current.Server.MapPath($"~/" + "Logger"))
		   .Select(f => new FileInfo(f))
		   .Where(f => f.LastWriteTime < DateTime.Now.AddMonths(-3))
		   .ToList()
		   .ForEach(f => f.Delete());
		}

	}
}
