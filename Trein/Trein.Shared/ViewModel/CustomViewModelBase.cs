using System;
using GalaSoft.MvvmLight;
using System.Linq.Expressions;
using Windows.ApplicationModel.Resources;
using GalaSoft.MvvmLight.Ioc;
using Trein.Services.Interfaces;

namespace Trein.ViewModel
{
    public class CustomViewModelBase : ViewModelBase
    {
      protected ResourceLoader _resourceLoader = new ResourceLoader("Resources");

      public INavigationService NavigationService { get; set; }

      public CustomViewModelBase()
      {
        NavigationService = SimpleIoc.Default.GetInstance<INavigationService>();
      }

        public string ApplicationTitle
        {
            get
            {
              return _resourceLoader.GetString("AppTitle");
            }
        }

        protected void RaisePropertyChanged(Expression<Func<object>> expression)
        {
            RaisePropertyChanged(GetPropertyName(expression));
        }

        /// <summary>
        /// Gets a property name, usage: GetPropertyName(() => Object.PropertyName)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
                memberExpression = (MemberExpression)((UnaryExpression)expression.Body).Operand;

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Gets a property name, usage: Utils.GetPropertyName<T>(x => x.PropertyName);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
                memberExpression = (MemberExpression)((UnaryExpression)expression.Body).Operand;

            return memberExpression.Member.Name;
        }

    }
}
