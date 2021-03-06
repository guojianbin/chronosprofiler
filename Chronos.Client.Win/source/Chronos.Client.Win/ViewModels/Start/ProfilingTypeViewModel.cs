﻿using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Chronos.Extensibility;

namespace Chronos.Client.Win.ViewModels.Start
{
    public sealed class ProfilingTypeViewModel : PropertyChangedBase
    {
        private readonly ProfilingTypeSettingsCollection _profilingTypesSettings;
        private readonly List<ProfilingTypeViewModel> _profilingTypes;
        private readonly IProfilingType _profilingType;
        private readonly FrameworkViewModel _framework;
        //private object _settingsViewModel;
        private ProfilingTypeSettings _profilingTypeSettings;
        private int _references;
        private bool _isCheckedManually;

        public ProfilingTypeViewModel(IProfilingType profilingType, FrameworkViewModel framework,
            List<ProfilingTypeViewModel> profilingTypes, ProfilingTypeSettingsCollection profilingTypesSettings)
        {
            _references = 0;
            _profilingType = profilingType;
            _framework = framework;
            _profilingTypes = profilingTypes;
            _profilingTypesSettings = profilingTypesSettings;
        }

        public IProfilingType ProfilingType
        {
            get { return _profilingType; }
        }

        //public object SettingsViewModel
        //{
        //    get
        //    {
        //        if (_settingsViewModel == null && IsChecked)
        //        {
        //            _settingsViewModel = _profilingType.GetAdapter().CreateSettingsPresentation(_profilingTypeSettings);
        //        }
        //        return _settingsViewModel;
        //    }
        //}

        public bool IsVisible
        {
            get { return !_profilingType.Definition.IsHidden; }
        }

        public bool IsEnabled
        {
            get { return !_profilingType.IsTechnical; }
        }

        public bool IsChecked
        {
            get { return _isCheckedManually || _references > 0; }
            set
            {
                if (_references > 0)
                {
                    return;
                }
                _isCheckedManually = value;
                if (value)
                {
                    AddReference(false);
                }
                else
                {
                    RemoveReference(false);
                }
            }
        }

        private int References
        {
            get { return _references; }
            set
            {
                _references = value;
                NotifyOfPropertyChange(() => References);
                NotifyOfPropertyChange(() => IsChecked);
            }
        }

        internal void AddReference(bool automaticReference)
        {
            if (automaticReference)
            {
                References++;
            }
            _framework.AddReference();
            //If settings collection doesn't contain setings block then add it
            if (!_profilingTypesSettings.Contains(_profilingType.Definition.Uid))
            {
                if (_profilingTypeSettings == null)
                {
                    _profilingTypeSettings = _profilingTypesSettings.GetOrCreate(_profilingType.Definition.Uid);
                    _profilingTypeSettings.FrameworkUid = _profilingType.Definition.FrameworkUid;
                }
                else
                {
                    _profilingTypesSettings.Add(_profilingTypeSettings);
                }
            }
            //Look through profiling types and enable dependencies of current profiling type
            IEnumerable<DependencyDefinition> dependencies =_profilingType.Definition.Dependencies;
            foreach (ProfilingTypeViewModel profilingTypeViewModel in _profilingTypes)
            {
                //profilingTypeViewModel is current profiling type - do nothing
                if (profilingTypeViewModel == this)
                {
                    continue;
                }
                ProfilingTypeViewModel viewModel = profilingTypeViewModel;
                //If profilingTypeViewModel is dependency for current profiling type?
                //If yes - enable it
                if (dependencies.Any(x => viewModel.ProfilingType.Definition.Uid == x.Uid))
                {
                    viewModel.AddReference(true);
                }
            }
        }

        internal void RemoveReference(bool automaticReference)
        {
            if (automaticReference)
            {
                References--;
            }
            _framework.RemoveRefrence();
            //If is not enabled (last reference was removed) then remove settings block from the collection
            if (!IsChecked)
            {
                _profilingTypesSettings.Remove(_profilingType.Definition.Uid);
            }
            //Look through profiling types and enable dependencies of current profiling type
            IEnumerable<DependencyDefinition> dependencies = _profilingType.Definition.Dependencies;
            foreach (ProfilingTypeViewModel profilingTypeViewModel in _profilingTypes)
            {
                //profilingTypeViewModel is current profiling type - do nothing
                if (profilingTypeViewModel == this)
                {
                    continue;
                }
                ProfilingTypeViewModel viewModel = profilingTypeViewModel;
                //If profilingTypeViewModel is dependency for current profiling type?
                //If yes - disable it
                if (dependencies.Any(x => viewModel.ProfilingType.Definition.Uid == x.Uid))
                {
                    viewModel.RemoveReference(true);
                }
            }
        }

        //internal void SetSelection(SelectionType selectionType)
        //{
        //    _selectionType = selectionType;
        //    if (SelectionTypeToBoolConverter.Convert(_selectionType))
        //    {
        //        if (_profilingTypeSettings == null)
        //        {
        //            _profilingTypeSettings = _profilingTypeSettingsCollection.GetOrCreate(_profilingType.Uid);
        //        }
        //        else
        //        {
        //            _profilingTypeSettingsCollection.Add(_profilingTypeSettings);
        //        }
        //    }
        //    else
        //    {
        //        _profilingTypeSettingsCollection.Remove(_profilingType.Uid);
        //    }
        //    if (_isUpdating)
        //    {
        //        return;
        //    }
        //    _isUpdating = true;
        //    //Remove SelectionType.Auto if it is for all profiling types
        //    foreach (ProfilingTypeViewModel viewModel in _profilingTypes)
        //    {
        //        //if ((viewModel.SelectionType & SelectionType.Auto) == SelectionType.Auto)
        //        //{
        //        //    viewModel.SelectionType = viewModel.SelectionType ^ SelectionType.Auto;
        //        //}
        //    }
        //    //Update SelectionType.Auto for all profiling types
        //    foreach (ProfilingTypeViewModel viewModel in _profilingTypes)
        //    {
        //        IEnumerable<ExtensionDependency> dependencies = viewModel.ProfilingType.Extension.Dependencies;
        //        foreach (ProfilingTypeViewModel v in _profilingTypes)
        //        {
        //            if (v == viewModel)
        //            {
        //                continue;
        //            }
        //            //if (dependencies.Any(x => v.ProfilingType.Extension.Uid == x.Uid))
        //            //{
        //            //    v.SelectionType = v.SelectionType | SelectionType.Auto;
        //            //}
        //        }
        //    }
        //    _isUpdating = false;
        //}

        //public void SwitchTo(IHostApplication hostApplication)
        //{
        //    _profilingTypeSettingsViewModel.SwitchTo(hostApplication);
        //}
    }
}
