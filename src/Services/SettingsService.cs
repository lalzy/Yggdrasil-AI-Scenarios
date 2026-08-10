// SettingsService.cs

using Yggdrasil.Models;
using Yggdrasil.Extensions;
using Yggdrasil.Data;

namespace Yggdrasil.Services;

public class SettingsService{
    private readonly AppDbContext _db;

    public SettingsService(AppDbContext db){
        _db = db;
    }
    public List<Settings> getAll(){
        return _db.Set<Settings>().ToList();
    }
    /// <summary>Get the current Theme selected </summary>
    /// <returns>ServiceResult with Theme object</returns>
    public ServiceResult<Themes> getTheme(){
        var theme = _db.Set<Settings>().Select(s=>s.Theme).Distinct().FirstOrDefault();
        return new ServiceResult<Themes>(theme);
    }
}
