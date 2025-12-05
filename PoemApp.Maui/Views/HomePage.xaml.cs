using PoemApp.Client.ApiClients;
using PoemApp.Core.DTOs;

namespace PoemApp.Maui.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly PoemApiClient _poemApiClient;

        public HomePage(PoemApiClient poemApiClient)
        {
            InitializeComponent();
            _poemApiClient = poemApiClient;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var poems = await _poemApiClient.GetAllAsync();
            poemList.ItemsSource = poems;
        }
    }
}