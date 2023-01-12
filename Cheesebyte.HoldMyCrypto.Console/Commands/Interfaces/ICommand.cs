namespace Cheesebyte.HoldMyCrypto.Console.Commands.Interfaces;

/// <summary>
/// Client-side commands with short pieces of action.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Starts running the functionality of this command.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/>.</returns>
    Task Run();
}