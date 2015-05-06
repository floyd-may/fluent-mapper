using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PDMS.Client.AppServices.Caching;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public class ListViewModel<TOperator, TEquipment> : ViewModelBase<ListViewModel<TOperator, TEquipment>>
        where TOperator : ICacheableDriver<TEquipment>
    {
        private readonly ICache<TOperator> _driversCache;
        private readonly IOperatorViewModelFactory<TOperator, TEquipment> _operatorViewModelFactory;

        private readonly AsyncSubject<IEnumerable<IOperatorViewModel>> _obsTaskOperators;
        private readonly BehaviorSubject<string> _obsFilterText;
        private readonly BehaviorSubject<string> _obsSortProp;
        private readonly BehaviorSubject<bool> _obsSortDescending;
        private readonly BehaviorSubject<bool> _obsHideInactive;
        private readonly BehaviorSubject<IEnumerable<IOperatorViewModel>> _obsOperators; 

        public ListViewModel(
            ICache<TOperator> driversCache,
            IOperatorViewModelFactory<TOperator, TEquipment> operatorViewModelFactory, 
            IScheduler scheduler
            )
            : base(scheduler)
        {
            _driversCache = driversCache;
            _operatorViewModelFactory = operatorViewModelFactory;

            _obsTaskOperators = new AsyncSubject<IEnumerable<IOperatorViewModel>>();

            _obsHideInactive = new BehaviorSubject<bool>(true);
            PropertyChangeFromObservable(x => x.HideInactive, x => { }, _obsHideInactive);

            _obsFilterText = new BehaviorSubject<string>("");
            PropertyChangeFromObservable(x => x.FilterText, x => { }, _obsFilterText);
            
            _obsSortDescending = new BehaviorSubject<bool>(false);
            PropertyChangeFromObservable(x => x.SortDescending, x => { }, _obsSortDescending);

            _obsSortProp = new BehaviorSubject<string>("LastName");
            PropertyChangeFromObservable(x => x.SortProperty, x => { }, _obsSortProp);

            _obsOperators = new BehaviorSubject<IEnumerable<IOperatorViewModel>>(Enumerable.Empty<IOperatorViewModel>());
            PropertyChangeFromObservable(x => x.Operators, x => { }, _obsOperators);

            var throttledFilterTextSubject = new BehaviorSubject<string>(_obsFilterText.Value);
            _obsFilterText
                .Throttle(TimeSpan.FromSeconds(0.5), Scheduler)
                .Subscribe(x => throttledFilterTextSubject.OnNext(x));

            _obsTaskOperators
                .CombineLatest(
                    _obsHideInactive,
                    (ops, hide) => ops.Where(op => !hide || op.IsActive)
                    )
                .CombineLatest(
                    throttledFilterTextSubject, 
                    (ops, txt) => ops.Where(op => op.PassesFilter(txt)))
                .CombineLatest(
                    _obsSortProp,
                    _obsSortDescending,
                    SortOperators)
                .Subscribe(_obsOperators);
        }

        public void Fetch()
        {
            Observable.FromAsync(_driversCache.Get)
                .Select(x => 
                    x.Select(op => _operatorViewModelFactory.Create(op))
                    )
                .Subscribe(_obsTaskOperators)
                ;
        }

        public IEnumerable<IOperatorViewModel> Operators
        {
            get { return _obsOperators.Value; }
            set { _obsOperators.OnNext(value); } 
        }

        public string FilterText 
        { 
            get { return _obsFilterText.Value; } 
            set { _obsFilterText.OnNext(value); } 
        }

        public string SortProperty
        {
            get { return _obsSortProp.Value; }
            set { _obsSortProp.OnNext(value); }
        }

        public bool SortDescending
        {
            get { return _obsSortDescending.Value; }
            set { _obsSortDescending.OnNext(value); }
        }

        public bool HideInactive
        {
            get { return _obsHideInactive.Value; }
            set { _obsHideInactive.OnNext(value); }
        }

        private IEnumerable<IOperatorViewModel> SortOperators(
            IEnumerable<IOperatorViewModel> ops,
            string sortProperty,
            bool descending)
        {
            var allProperties = typeof (IOperatorViewModel)
                .GetProperties();
            var propInfo = allProperties
                .FirstOrDefault(x => x.Name == sortProperty)
                ?? allProperties.First(x => x.Name == "LastName")
                ;

            var getter = propInfo.GetGetMethod();

            if (descending)
                return ops.OrderByDescending(x => getter.Invoke(x, new object[0]));
            else
                return ops.OrderBy(x => getter.Invoke(x, new object[0]));
        }
    }
}