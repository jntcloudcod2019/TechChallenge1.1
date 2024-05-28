using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TechChallengeFIAP.Models;

public class ContactRepositoryTests
{
    [Fact]
    public async Task GetAllContactsAsync_Should_Return_All_Contacts_Using_Mock()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new Contact { Id = 1, Name = "Test Contact 1", Phone = "123456789", Email = "test1@example.com", Ddd = 11 },
            new Contact { Id = 2, Name = "Test Contact 2", Phone = "987654321", Email = "test2@example.com", Ddd = 12 }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Contact>>();
        mockSet.As<IAsyncEnumerable<Contact>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Contact>(contacts.GetEnumerator()));

        mockSet.As<IQueryable<Contact>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Contact>(contacts.Provider));
        mockSet.As<IQueryable<Contact>>().Setup(m => m.Expression).Returns(contacts.Expression);
        mockSet.As<IQueryable<Contact>>().Setup(m => m.ElementType).Returns(contacts.ElementType);
        mockSet.As<IQueryable<Contact>>().Setup(m => m.GetEnumerator()).Returns(contacts.GetEnumerator());

        var mockContext = new Mock<ContactDbContext>();
        mockContext.Setup(c => c.Contacts).Returns(mockSet.Object);

        var repository = new ContactRepository(mockContext.Object);

        // Act
        var result = await repository.GetAllContactsAsync();

        // Assert
        Xunit.Assert.Equal(2, result.Count());
    }
}

public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
    {
        throw new NotImplementedException();
    }
}

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        return new ValueTask(Task.CompletedTask);
    }
}
