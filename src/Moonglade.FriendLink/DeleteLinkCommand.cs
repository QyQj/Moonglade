﻿using MediatR;
using Microsoft.Extensions.Logging;
using Moonglade.Data;
using Moonglade.Data.Entities;

namespace Moonglade.FriendLink;

public record DeleteLinkCommand(Guid Id) : IRequest;

public class DeleteLinkCommandHandler(
    MoongladeRepository<FriendLinkEntity> repo,
    ILogger<DeleteLinkCommandHandler> logger) : IRequestHandler<DeleteLinkCommand>
{
    public async Task Handle(DeleteLinkCommand request, CancellationToken ct)
    {
        var link = await repo.GetByIdAsync(request.Id, ct);
        if (null != link)
        {
            await repo.DeleteAsync(link, ct);
        }

        logger.LogInformation("Deleted a friend link: {Title}", link?.Title);
    }
}