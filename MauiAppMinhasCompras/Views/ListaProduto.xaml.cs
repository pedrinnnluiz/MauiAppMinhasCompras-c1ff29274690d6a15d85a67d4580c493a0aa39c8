using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
    {
        InitializeComponent();
        lst_nome.ItemsSource = lista;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        lista.Clear();

        List<Produto> tmp = await App.Db.GetAll();
        tmp.ForEach(x => lista.Add(x));
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is MenuItem menuItem && menuItem.BindingContext is Produto produto)
            {
                bool confirm = await DisplayAlert("Confirmar",
                                                  $"Deseja remover o produto \"{produto.Descricao}\"?",
                                                  "Sim", "Não");

                if (confirm)
                {
                    lista.Remove(produto);
                    await App.Db.Delete(produto.Id);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "Ok");
        }
    }



    private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = e.NewTextValue?.ToLower() ?? "";
        if (string.IsNullOrWhiteSpace(filtro))
        {
            lst_nome.ItemsSource = lista;
        }
        else
        {
            lst_nome.ItemsSource = lista.Where(p =>
                p.Descricao?.ToLower().Contains(filtro) == true
            );
        }
    }

    private void lst_nome_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;
            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}
