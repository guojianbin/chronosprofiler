﻿using System.Windows.Input;
using Chronos.Core;
using Rhiannon.Threading;
using Rhiannon.Windows.Presentation;
using Rhiannon.Windows.Presentation.Commands;

namespace Chronos.Client.Win.Views.References.Class
{
    public class ViewModel : ViewModelBase, IViewModel
    {
        private Reference<ProcessInfo, ThreadInfo, FunctionInfo, IEvent> _reference;
        private readonly ClassInfo _classInfo;
        private readonly IProcessShadow _processShadow;
        private readonly IProcessShadowNavigator _processShadowNavigator;
        private readonly ITaskFactory _taskFactory;
        private object _selectedNode;

        public ViewModel(ClassInfo classInfo, IProcessShadow processShadow, IProcessShadowNavigator processShadowNavigator,
            IEventNameFormatter eventNameFormatter, ITaskFactory taskFactory)
        {
            _classInfo = classInfo;
            _processShadow = processShadow;
            _taskFactory = taskFactory;
            processShadow.ThreadTraces.Reloaded += OnThreadTracesReloaded;
            _processShadowNavigator = processShadowNavigator;
            EventNameFormatter = eventNameFormatter;
        }

        private void OnThreadTracesReloaded()
        {
            ReloadReferencesCommand.Execute(null);
        }

        public ICommand NavigateToEventCommand { get; private set; }

        public ICommand ReloadReferencesCommand { get; private set; }

        public IEventNameFormatter EventNameFormatter { get; private set; }

        public object SelectedNode
        {
            get { return _selectedNode; }
            set { SetPropertyAndNotifyChanged(() => SelectedNode, ref _selectedNode, value); }
        }

		public Reference<ProcessInfo, ThreadInfo, FunctionInfo, IEvent> Reference
		{
			get { return _reference; }
			set { SetPropertyAndNotifyChanged(() => Reference, ref _reference, value); }
		}

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            NavigateToEventCommand = new SyncCommand<object>(NavigateToEvent);
            ReloadReferencesCommand = new AsyncCommand(ReloadReferences, _taskFactory);
            ReloadReferencesCommand.Execute(null);
        }

        private void ReloadReferences()
        {
            Reference = _processShadow.ReferencesAnalyzer.GetClassReferences(_classInfo);
        }

        private void NavigateToEvent(object parameter)
        {
            Reference<IEvent> reference = parameter as Reference<IEvent>;
            if (reference == null)
            {
                return;
            }
            _processShadowNavigator.Navigate(reference.Item);
        }

        public override void Dispose()
        {
            base.Dispose();
            _processShadow.ThreadTraces.Reloaded -= OnThreadTracesReloaded;
        }
    }
}