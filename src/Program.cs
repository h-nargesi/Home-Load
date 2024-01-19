using Photon.HomeLoad;
using Serilog;
using Serilog.Events;

try
{
    var personPath = (args.Length > 0 ? args[0] : null) ?? "person.json";
    var configPath = (args.Length > 1 ? args[1] : null) ?? "config.json";
    if (args.Length <= 2 || !int.TryParse(args[2], out var period)) period = 15;

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console(LogEventLevel.Information)
        .WriteTo.File("events-.log", LogEventLevel.Debug, rollingInterval: RollingInterval.Day)
        .CreateLogger();

    var config = LoadConfig(configPath);

    using var service = new HttpRequests(config);

    while (true)
    {
        try
        {
            var banks = await GetBanks(service);

            if (banks == null) continue;

            foreach (var bank in banks)
            {
                Log.Logger.Information("CHECK-BANK\t{0}\t{1}", bank.BankOrganizationUnitId, bank.Title);

                var branches = await GetBranches(service, bank);
            }
        }
        finally
        {
            Thread.Sleep(1000);
        }
    }
}
catch (Exception ex)
{
    Log.Logger.Error(ex, "Error");
}

static Config LoadConfig(string configPath)
{
    var config = JsonHandler.LoadFromFile<Config>(configPath);

    Log.Logger.Information("LOAD-CONFIG\tc={0}\tp={1}", config.CarName, config.ProductName);
    Log.Logger.Debug("LOAD-CONFIG={0}", config.SerializeJson());

    return config;
}

static async Task<Bank[]> GetBanks(HttpRequests service)
{
    var banks = await service.Banks();

    if (!banks.Any())
    {
        throw new Exception("bank not found!");
    }

    Log.Logger.Information("GET-BANKS\tx{0}", banks.Length);
    Log.Logger.Debug("GET-BANKS={0}", banks.SerializeJson());

    return banks;
}

static async Task<Branch[]> GetBranches(HttpRequests service, Bank bank)
{
    var branches = await service.Branches(bank.BankOrganizationUnitId);

    Log.Logger.Information("GET-BRANCHES\tx{0}", branches.Length);
    Log.Logger.Debug("GET-BRANCHES={0}", branches.SerializeJson());

    return branches;
}
