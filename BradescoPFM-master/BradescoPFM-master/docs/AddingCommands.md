# Adding Comands

You can add new admin commands using the ICommand interface:

```cs
public interface ICommand
{
    void Run(IActivity activity);
}
```

Every time a new message comes to MessageController, the [`CommandHandler.cs`](../src/PFM/PFM.Bot/Commands/CommandHandler.cs) is in charge to apply a Regex expression and match the pattern
`/command=commandName` - for instance, the command named *ProActive* would be called by the client by `/command=proactive`.


## Steps to extend functionally through new commands:

1. Create your command implementing `ICommand` interface, adding any behaviour that uses `IActivity` as an argument. Example:

```cs
public void Run(IActivity activity)
{
    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
    Task task = new Task(async () =>
    {
        var reply = ((Activity)activity).CreateReply("Um momento, vou pesquisar!");
        await connector.Conversations.SendToConversationAsync(reply);

        var service = new CoreService();
        var alert = await service.CheckNegativeBalance(Settings.AccountId);

        reply = ((Activity)activity).CreateReply(alert.Message);
        await connector.Conversations.SendToConversationAsync(reply);
    });
    task.Start();
}
```

2. Add the command name and its instance to this switch statement on [`CommandFactory.cs`](../src/PFM/PFM.Bot/Commands/CommandFactory.cs)


```cs
public class CommandFactory
{
    public static ICommand GetCommand(string command)
    {
        switch (command.ToLower())
        {
            case "proactive":
                return new ProactiveAlertCommand();
            default:
                return new DefaultCommand();
        }
    }
}
```