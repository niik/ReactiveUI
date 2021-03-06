﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ninject;
using ReactiveUI;
using ReactiveUI.Mobile;
using ReactiveUI.Routing;
using MobileSample_WinRT.Views;

namespace MobileSample_WinRT.ViewModels
{
#if FALSE
    public class AkavacheDriver : ISuspensionDriver, IEnableLogger
    {
        public AkavacheDriver(string applicationName)
        {
            BlobCache.ApplicationName = applicationName;
            BlobCache.SerializerSettings = new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
            };
        }

        public IObservable<T> LoadState<T>() where T : class, IApplicationRootState
        {
            return BlobCache.UserAccount.GetObjectAsync<T>("__AppState"); ; ; ;
        }

        public IObservable<Unit> SaveState<T>(T state) where T : class, IApplicationRootState
        {
            return BlobCache.UserAccount.InsertObject("__AppState", state)
                .SelectMany(BlobCache.UserAccount.Flush());
        }

        public IObservable<Unit> InvalidateState()
        {
            BlobCache.UserAccount.InvalidateObject<object>("__AppState");
            return Observable.Return(Unit.Default);
        }
    }
#endif

    [DataContract]
    public class AppBootstrapper : ReactiveObject, IApplicationRootState
    {
        [DataMember] RoutingState _Router;

        public IRoutingState Router {
            get { return _Router; }
            set { _Router = (RoutingState) value; } // XXX: This is dumb.
        }

        [IgnoreDataMember]
        public IKernel Kernel { get; protected set; }

        public AppBootstrapper()
        {
            Router = new RoutingState();

            Kernel = new StandardKernel();
            Kernel.Bind<TestPage1ViewModel>().ToSelf();
            Kernel.Bind<TestPage2ViewModel>().ToSelf();
            Kernel.Bind<TestPage3ViewModel>().ToSelf();

            Kernel.Bind<IViewFor<TestPage1ViewModel>>().To<TestPage1View>();
            Kernel.Bind<IViewFor<TestPage2ViewModel>>().To<TestPage2View>();
            Kernel.Bind<IViewFor<TestPage3ViewModel>>().To<TestPage3View>();

            Kernel.Bind<IApplicationRootState>().ToConstant(this);
            Kernel.Bind<IViewFor>().To<MainPage>().Named("InitialPage");

            Kernel.Bind<IScreen>().ToConstant(this);

            RxApp.ConfigureServiceLocator(
                (t, s) => Kernel.Get(t, s),
                (t, s) => Kernel.GetAll(t, s),
                (c, t, s) => { var r = Kernel.Bind(t).To(c); if (s != null) r.Named(s); });

            Router.Navigate.Execute(new TestPage1ViewModel(this));
        }
    }
}
