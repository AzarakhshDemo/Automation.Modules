using meTesting.Automation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace meTesting.LetterSrv;

public class LetterService : ILetterService
{
    static Dictionary<int, Letter> _store = new();
    public Letter CreateLetter(Letter inp)
    {
        var id = _store.Count + 1;
        _store[id] = inp;
        inp.Id = id;
        Console.WriteLine($"{id}:{inp} created");
        return inp;
    }

    public IEnumerable<Letter> GetInbox(int userId) => _store
        .Where(a => a.Value.UserId == userId && !a.Value.IsArchived)
        .Select(a => a.Value);

    public Letter Approve(int id, int userId)
    {
        var t = Get(id);
        t.IsApproved = true;
        t.State = LetterState.Approved;
        t.UserId = t.SignsBy;
        Console.WriteLine($"{id} approved");
        return t;
    }

    public Letter Archive(int id, int userId)
    {
        var t = Get(id);
        if (!t.CanArchive)
            throw new InvalidOperationException();
        t.IsArchived = true;
        Console.WriteLine($"{id} archived");
        return t;
    }


    public Letter Get(int id)
    {
        return _store[id];
    }

    public Letter Sign(int id, int userId)
    {
        var t = Get(id);
        t.IsSigned = true;
        t.State = LetterState.Signed;
        Console.WriteLine($"{id} signed");
        return t;
    }

    public Letter Reject(int id, int userId)
    {
        var t = Get(id);
        t.IsRejected = true;
        t.State = LetterState.Rejected;
        Console.WriteLine($"{id} rejected");
        return t;
    }
}

public static class DiHelper
{
    public static IServiceCollection AddLetterService(this IServiceCollection services)
    {
        services.RemoveAll<ILetterService>();
        services.AddScoped<ILetterService, LetterService>();
        return services;
    }
}
