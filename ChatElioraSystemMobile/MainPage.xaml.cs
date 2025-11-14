using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystemMobile.ViewModels;
using System.Collections.ObjectModel;

namespace ChatElioraSystemMobile;

public partial class MainPage : ContentPage
{
    public MainPage(IChatViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        if (viewModel is ChatViewModel vm)
        {
            vm.ScrollToEndRequested += () =>
            {
                if (MessagesView.ItemsSource is ObservableCollection<IChatMessage> items && items.Count > 0)
                {
                    var last = items.Last();
                    MessagesView.ScrollTo(last, position: ScrollToPosition.End, animate: true);
                }
            };
        }
        //if (viewModel is ChatViewModel vm)
        //{
        //    vm.ScrollToEndRequested += () =>
        //    {
        //        // 🌀 Scroll na koniec
        //        if (MessagesView.ItemsSource is ICollection<object> items && items.Count > 0)
        //        {
        //            var last = items.Last();
        //            MessagesView.ScrollTo(last, position: ScrollToPosition.End, animate: true);
        //        }
        //    };
        //}
    }

    private async void OnTestQdrantClicked(object sender, EventArgs e)
    {
        try
        {
            using var client = new HttpClient();
            // TODO: Configure your Qdrant server address via dependency injection or configuration
            // Example: var qdrantService = Host.Services.GetRequiredService<IUriAdressService>();
            // var url = $"{qdrantService.GetDbVecAdressRest()}/collections";
            var url = "http://localhost:6333/collections"; // Default Qdrant port - configure for your environment

            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            await DisplayAlert("Qdrant Debug", $"Status: {response.StatusCode}\n\n{json}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Qdrant Debug ERROR", ex.ToString(), "OK");

        }
    }
}

