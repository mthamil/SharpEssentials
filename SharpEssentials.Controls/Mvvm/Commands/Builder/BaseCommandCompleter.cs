using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharpEssentials.Controls.Mvvm.Commands.Builder
{
    /// <summary>
    /// Convenient base class for command completers.
    /// </summary>
    public abstract class BaseCommandCompleter : ICommandCompleter
    {
        /// <summary>
        /// Sets the operation that a command will execute.
        /// </summary>
        /// <param name="operation">The parameterless operation to be executed</param>
        /// <returns>A new command</returns>
        public ICommand Executes(Action operation) => Executes(_ => operation());

        /// <summary>
        /// Sets the operation that a command will execute.
        /// </summary>
        /// <param name="operation">The operation to be executed</param>
        /// <returns>A new command</returns>
        public ICommand Executes(Action<object> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            var command = new TriggeredRelayCommand(operation, CanExecute());
            return Configure(command);
        }

        /// <summary>
        /// Sets the asynchronous operation that a command will execute.
        /// </summary>
        /// <param name="operation">The parameterless, asynchronous operation to be executed</param>
        /// <returns>A new command</returns>
        public IAsyncCommand ExecutesAsync(Func<Task> operation) => ExecutesAsync(_ => operation());

        /// <summary>
        /// Sets the asynchronous operation that a command will execute.
        /// </summary>
        /// <param name="operation">The asynchronous operation to be executed</param>
        /// <returns>A new command</returns>
        public IAsyncCommand ExecutesAsync(Func<object, Task> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            var command = new TriggeredAsyncRelayCommand(operation, CanExecute());
            return Configure(command);
        }

        protected abstract Predicate<object> CanExecute(); 

        protected virtual TCommand Configure<TCommand>(TCommand command) where TCommand : ITriggerableCommand => command;
    }
}