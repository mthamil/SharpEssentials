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
        public ICommand Executes(Action operation) => Executes<object>(_ => operation());

        /// <summary>
        /// Sets the operation that a command will execute.
        /// </summary>
        /// <param name="operation">The operation to be executed</param>
        /// <returns>A new command</returns>
        public ICommand Executes<T>(Action<T> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            var command = new TriggeredRelayCommand<T>(operation, CanExecute<T>());
            return Configure(command);
        }

        /// <summary>
        /// Sets the asynchronous operation that a command will execute.
        /// </summary>
        /// <param name="operation">The parameterless, asynchronous operation to be executed</param>
        /// <returns>A new command</returns>
        public IAsyncCommand ExecutesAsync(Func<Task> operation) => ExecutesAsync<object>(_ => operation());

        /// <summary>
        /// Sets the asynchronous operation that a command will execute.
        /// </summary>
        /// <param name="operation">The asynchronous operation to be executed</param>
        /// <returns>A new command</returns>
        public IAsyncCommand ExecutesAsync<T>(Func<T, Task> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            var command = new TriggeredAsyncRelayCommand<T>(operation, CanExecute<T>());
            return Configure(command);
        }

        protected abstract Predicate<T> CanExecute<T>(); 

        protected virtual TCommand Configure<TCommand>(TCommand command) where TCommand : ITriggerableCommand => command;
    }
}