using System;
using MediatR;

namespace NexaWork.Application.Features.Client.Post.Commands.Delete;

public record DeletePostCommand(Guid PostId) : IRequest;
