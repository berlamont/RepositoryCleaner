﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryPickerViewModel.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.ViewModels
{
    using System.IO;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Catel.Services;

    public class DirectoryPickerViewModel : ViewModelBase
    {
        #region Constructors
        public DirectoryPickerViewModel(ISelectDirectoryService selectDirectoryService, IProcessService processService)
        {
            Argument.IsNotNull(() => selectDirectoryService);
            Argument.IsNotNull(() => processService);

            _selectDirectoryService = selectDirectoryService;
            _processService = processService;

            OpenDirectory = new TaskCommand(OnOpenDirectoryExecuteAsync, OnOpenDirectoryCanExecute);
            SelectDirectory = new TaskCommand(OnSelectDirectoryExecuteAsync);
        }
        #endregion

        #region Fields
        private readonly IProcessService _processService;
        private readonly ISelectDirectoryService _selectDirectoryService;
        #endregion

        #region Properties
        public double LabelWidth { get; set; }

        public string LabelText { get; set; }

        public string SelectedDirectory { get; set; }
        #endregion

        #region Commands
        /// <summary>
        /// Gets the OpenDirectory command.
        /// </summary>
        public TaskCommand OpenDirectory { get; private set; }

        /// <summary>
        /// Method to check whether the OpenDirectory command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnOpenDirectoryCanExecute()
        {
            if (string.IsNullOrWhiteSpace(SelectedDirectory))
            {
                return false;
            }

            // Don't allow users to write text that they can "invoke" via our software
            if (!Directory.Exists(SelectedDirectory))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method to invoke when the OpenDirectory command is executed.
        /// </summary>
        private async Task OnOpenDirectoryExecuteAsync()
        {
            var fullPath = Path.GetFullPath(SelectedDirectory);
            _processService.StartProcess(fullPath);
        }

        /// <summary>
        /// Gets the SelectDirectory command.
        /// </summary>
        public TaskCommand SelectDirectory { get; private set; }

        /// <summary>
        /// Method to invoke when the SelectOutputDirectory command is executed.
        /// </summary>
        private async Task OnSelectDirectoryExecuteAsync()
        {
            if (!string.IsNullOrEmpty(SelectedDirectory))
            {
                _selectDirectoryService.InitialDirectory = Path.GetFullPath(SelectedDirectory);
            }

            if (await _selectDirectoryService.DetermineDirectoryAsync())
            {
                SelectedDirectory = _selectDirectoryService.DirectoryName;
            }
        }
        #endregion
    }
}