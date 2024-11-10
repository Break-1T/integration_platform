using integration_platform;

await IntegratorAppBuilder.CreateWebApplication(args, x => { }).RunAsync();

// needed for integration tests
public partial class Program { }