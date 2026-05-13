using System;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface IPostRepository
{
    /// <summary>
    /// Adds a new Post to the repository.
    /// </summary>
    /// <param name="post"></param>
    void Add(Post post);




}
