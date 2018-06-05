using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageServiceGUI.Views;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using ImageServiceGUI.Model;
using ImageServiceGUI.Commands;
using Newtonsoft.Json.Linq;
using ImageServiceGUI.Communication;
using System.ComponentModel;
using System.Windows.Data;
using System.Diagnostics;
using ImageServiceGUI.Enums;

namespace ImageServiceGUI.ViewModel
{
    class SettingsViewModel : INotifyPropertyChanged, ISettingsViewModel
    {
        public ICommand RemoveCommand { get; private set; }
        // this eventhandlers sends messages to the the model
        public event EventHandler<CommandMessage> Request;
        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<int, IServiceCommands> commandDictionary;

        #region notifyable params
        public ObservableCollection<string> List { get; set; }
        private string outputDir;
        public string OutputDir
        {
            get { return this.outputDir; }
            set
            {
                if (this.outputDir != value)
                {
                    this.outputDir = value;
                    this.NotifypropertyChanged("Outputdir");
                }
            }
        }
        private string srcName;
        public string SrcName
        {
            get { return this.srcName; }
            set
            {
                if (this.srcName != value)
                {
                    this.srcName = value;
                    this.NotifypropertyChanged("SrcName");
                }
            }
        }
        private string logName;
        public string LogName
        {
            get { return this.logName; }
            set
            {
                if (this.logName != value)
                {
                    this.logName = value;
                    this.NotifypropertyChanged("LogName");
                }
            }
        }
        private string thumbSize;
        public string ThumbSize
        {
            get { return this.thumbSize; }
            set
            {
                if (this.thumbSize != value)
                {
                    this.thumbSize = value;
                    this.NotifypropertyChanged("ThumbSize");
                }
            }
        }
        private string selected;
        public string Selected
        {
            get { return this.selected; }
            set
            {
                if (this.selected != value)
                {
                    this.selected = value;
                    this.NotifypropertyChanged("Selected");
                }                
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            List = new ObservableCollection<string>();
            
            // in order to be able to make changes to the list from other threads
            BindingOperations.EnableCollectionSynchronization(List, List);

            RemoveCommand = new DelegateCommand(this.OnRemove, this.CanRemove);
            // to get updated wether the button should be enabled
            this.PropertyChanged += SelectionChanged;
            
            new SettingsModel(this).dataReceive += OnDataReceived;

            this.commandDictionary = new Dictionary<int, IServiceCommands>()
            {
                { (int)CommandEnum.ConfigCommand, new ConfigCommand(this)},
                { (int)CommandEnum.CloseCommand, new CloseHandlerCommand(this.List) }
            };

            Request?.Invoke(this, new CommandMessage((int)CommandEnum.ConfigCommand));
        }

        /// <summary>
        /// Called when [remove].
        /// </summary>
        public void OnRemove()
        {
            if (!String.IsNullOrEmpty(Selected))
            {
                Request?.Invoke(this, new CommandMessage((int)CommandEnum.CloseCommand, Selected));
            }                        
        }

        /// <summary>
        /// Determines whether this instance can remove.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can remove; otherwise, <c>false</c>.
        /// </returns>
        public bool CanRemove()
        {
            return !String.IsNullOrEmpty(Selected);
        }

        /// <summary>
        /// Selections the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void SelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            var command = this.RemoveCommand as DelegateCommand;
            command.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Notifyproperties the changed.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        public void NotifypropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /// <summary>
        /// Called when [data received].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        public void OnDataReceived(object sender, CommandMessage args)
        {
            if (commandDictionary.TryGetValue(args.commandID, out IServiceCommands command))
            {
                command.Execute(args.commandArgs);
            }
        }
    }
}
