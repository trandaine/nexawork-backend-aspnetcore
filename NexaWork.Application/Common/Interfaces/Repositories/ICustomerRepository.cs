using System;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface ICustomerRepository
{
    /// <summary>
    /// Tạo mới một Customer. 
    /// </summary>
    /// <param name="customer"></param>
    void Create(Customer customer);

    /// <summary>
    /// Lấy Customer theo IdentityUserId. Trả về null nếu không tìm thấy.
    /// </summary>
    /// <param name="identityUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Customer?> GetByIdentityIdAsync(string identityUserId, CancellationToken cancellationToken);

    /// <summary>
    /// Lấy Customer theo CustomerId. Trả về null nếu không tìm thấy.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken);


    /// <summary>
    /// Lấy tất cả Customer. Trả về danh sách rỗng nếu không có Customer nào.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Customer>> GetAllCustomerAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Update customer information
    /// </summary>
    /// <param name="customer"></param>
    void Update(Customer customer);

}
