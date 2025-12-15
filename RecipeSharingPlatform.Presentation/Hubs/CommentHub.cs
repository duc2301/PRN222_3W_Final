using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class CommentHub : Hub
{
    public async Task JoinRecipeGroup(int recipeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"recipe-{recipeId}");
    }

    public async Task LeaveRecipeGroup(string recipeId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, recipeId);
    }

    public async Task SendComment(int recipeId, object comment)
    {
        await Clients.Group($"recipe-{recipeId}").SendAsync("ReceiveComment", comment);
    }

    public async Task SendLike(string recipeId, object likeInfo)
    {
        await Clients.Group(recipeId).SendAsync("ReceiveLike", likeInfo);
    }
}