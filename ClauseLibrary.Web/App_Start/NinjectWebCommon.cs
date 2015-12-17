// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using ClauseLibrary.Common;
using ClauseLibrary.Common.Services;
using ClauseLibrary.Web.App_Start;
using ClauseLibrary.Web.Models.Database.Services;
using ClauseLibrary.Web.Models.DataModel;
using ClauseLibrary.Web.Repositories;
using ClauseLibrary.Web.Services;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.WebApi;
using WebActivatorEx;
using IDependencyResolver = System.Web.Mvc.IDependencyResolver;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof (NinjectWebCommon), "Start")]
[assembly: ApplicationShutdownMethod(typeof (NinjectWebCommon), "Stop")]

namespace ClauseLibrary.Web.App_Start
{
    /// <summary>
    /// The Ninject IOC container
    /// </summary>
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof (OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof (NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
                DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IListItemRequestService<Clause>>().To<ListItemRequestService<Clause>>();
            kernel.Bind<IListItemRequestService<Group>>().To<ListItemRequestService<Group>>();
            kernel.Bind<IListItemsRepository<Clause>>().To<ClauseRepository>();
            kernel.Bind<IListItemsRepository<Group>>().To<GroupRepository>();
            kernel.Bind<IListItemsRepository<Tag>>().To<TagRepository>();
            kernel.Bind<IListItemsRepository<Favourite>>().To<FavouritesRepository>();
            kernel.Bind<IListItemsRepository<ExternalLink>>().To<ExternalLinksRepository>();

            kernel.Bind<ISharePointService>().To<SharePointService>();
            kernel.Bind<IProvisioningService>().To<ProvisioningService>();

            if (SettingsHelper.UseSqlForLoginSettings)
            {
                kernel.Bind<ILoginSettingsService>().To<SqlLoginSettingsService>();
            }
            else
            {
                kernel.Bind<ILoginSettingsService>().To<JsonFileLoginSettingsService>();
            }

            if (SettingsHelper.UseCookieForTokenStorage)
            {
                kernel.Bind<IRefreshTokenManager>().To<ServerCookieRefreshTokenManager>();
            }
            else
            {
                kernel.Bind<IRefreshTokenManager>().To<QueryStringRefreshTokenManager>();
            }
            kernel.Bind<LoggingService>().To<LoggingService>();
        }
    }

    /// <summary>
    /// A dependency resolver for ninject.
    /// </summary>
    public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver, System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectDependencyResolver" /> class.
        /// </summary>
        public NinjectDependencyResolver(StandardKernel kernel) : base(kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        object IDependencyResolver.GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        IEnumerable<object> IDependencyResolver.GetServices(Type serviceType)
        {
            try
            {
                return _kernel.GetAll(serviceType);
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }

        /// <summary>
        /// Begins the scope.
        /// </summary>
        public IDependencyScope BeginScope()
        {
            return this;
        }
    }

}

#region License 
// ClauseLibrary, https://github.com/OfficeDev/clauselibrary 
//   
// Copyright 2015(c) Microsoft Corporation 
//   
// All rights reserved. 
//   
// MIT License: 
//   
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
// following conditions: 
//   
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software. 
//   
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
// LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT 
// SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN 
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE 
// USE OR OTHER DEALINGS IN THE SOFTWARE. 
#endregion