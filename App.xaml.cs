public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
        
        // Usar Dispatcher para asegurar que la UI esté lista antes de verificar el estado
        Dispatcher.Dispatch(() =>
        {
            ((AppShell)MainPage).CheckLoginState();
        });
    }
}