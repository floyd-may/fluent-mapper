using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;

namespace PMDS.Client.UI
{
    public abstract class ViewModelBase<TThis> : INotifyPropertyChanged
        where TThis : ViewModelBase<TThis>
    {
        public IScheduler Scheduler { get; private set; }

        protected ViewModelBase(IScheduler scheduler)
        {
            Scheduler = scheduler;
        }

        protected void PropertyChangeFromObservable<TProp>(
            Expression<Func<TThis, TProp>> expression,
            Action<TProp> setter,
            IObservable<TProp> observable)
        {
            if(expression.Body.NodeType != ExpressionType.MemberAccess)
                throw new InvalidOperationException("Expression must be a property access of the form 'x => x.PropName'.");

            var propName = (expression.Body as MemberExpression).Member.Name;

            observable.DistinctUntilChanged().ObserveOn(Scheduler).Subscribe(x => SetProperty(x, setter, propName));
        }

        public event PropertyChangedEventHandler PropertyChanged = (o, e) => { };

        private void SetProperty<TProp>(TProp newValue, Action<TProp> setter, string propertyName)
        {
            setter(newValue);
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}