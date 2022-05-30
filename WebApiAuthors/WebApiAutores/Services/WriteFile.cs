namespace WebApiAuthors.Services
{
    public class WriteFile : IHostedService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string fileName = "file1.txt";
        private Timer _timer;

        public WriteFile(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            ToWrite("Proceso iniciado");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            ToWrite("Proceso finalizado");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            ToWrite("Proceso en ejecución: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        private void ToWrite(string message)
        {
            var route = $@"{_env.ContentRootPath}\wwwroot\{fileName}";
            using (StreamWriter writer = new StreamWriter(route, append: true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
