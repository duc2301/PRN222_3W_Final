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

    // Like Recipe
    public async Task SendLike(int recipeId, object likeInfo)
    {
        await Clients.Group($"recipe-{recipeId}").SendAsync("ReceiveLike", likeInfo);
    }

    // --- NEW: Like Comment ---
    public async Task SendCommentLike(int recipeId, int commentId, int newCount)
    {
        await Clients.Group($"recipe-{recipeId}").SendAsync("ReceiveCommentLike", commentId, newCount);
    }
    public async Task SendCommentDelete(int recipeId, int commentId)
    {
        await Clients.Group($"recipe-{recipeId}").SendAsync("ReceiveCommentDelete", commentId);
    }
}