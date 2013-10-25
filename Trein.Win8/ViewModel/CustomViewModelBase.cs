using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using System.Linq.Expressions;

namespace Trein.Win8.ViewModel
{
   
    public class CustomViewModelBase : ViewModelBase
    {
        public string ApplicationTitle
        {
            get
            {
                return "Treintijden";
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
