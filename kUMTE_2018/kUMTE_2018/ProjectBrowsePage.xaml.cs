﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using kUMTE_2018.Models;
using SQLite;
using Todoist.Net;
using Todoist.Net.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace kUMTE_2018
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectBrowsePage : ContentPage
    {
        public ObservableCollection<Project> Items { get; set; }

        public static string AuthKey { get; set; } = string.Empty;

        public ProjectBrowsePage()
        {
            InitializeComponent();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (var conn = new SQLiteConnection(App.AppDataDbString))
            {
                var item = conn.Get<AppSetting>(1);
                AuthKey = item.AuthKey;
            }

            if (string.IsNullOrEmpty(AuthKey))
            {
                DisplayAlert("Action required", "You need to set auth. key first", "Ok");
                Navigation.PushAsync(new SettingPage());
            }
            LoadProjects();
        }


        private async Task LoadProjects()
        {
            Items = new ObservableCollection<Project>();
            using (var client = new TodoistClient(AuthKey))
            {
                var project = await client.Projects.GetAsync();
                foreach (var item in project)
                {
                    Items.Add(item);
                }
            }
            MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            ((ListView)sender).SelectedItem = null;

            await Navigation.PushAsync(new TaskBrowsePage((Project)e.Item));
        }

        private void AddButton_OnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddProjectPage());
        }
    }
}
