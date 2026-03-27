using FacturacionA4V.UI.ViewModel;

namespace FacturacionA4V.Tests.ViewModel;

// Subclase concreta para testear ObservableObject
file class TestObservable : ObservableObject
{
    private string _value = "";
    public string Value
    {
        get => _value;
        set { _value = value; OnPropertyChanged(); }
    }

    public void DispararManual(string name) => OnPropertyChanged(name);
}

// TEST-035 a TEST-036
public class ObservableObjectTests
{
    [Fact]
    public void OnPropertyChanged_DisparaEventoConNombreCorrecto()
    {
        var obj = new TestObservable();
        string? received = null;
        obj.PropertyChanged += (_, e) => received = e.PropertyName;

        obj.Value = "test";

        Assert.Equal(nameof(TestObservable.Value), received);
    }

    [Fact]
    public void OnPropertyChanged_SinSuscriptores_NoLanzaExcepcion()
    {
        var obj = new TestObservable();
        var ex = Record.Exception(() => obj.DispararManual("Prop"));
        Assert.Null(ex);
    }
}

// TEST-037 a TEST-040
public class RelayCommandTests
{
    [Fact]
    public void Execute_LlamaAccion()
    {
        bool called = false;
        var cmd = new RelayCommand(() => called = true);

        cmd.Execute(null);

        Assert.True(called);
    }

    [Fact]
    public void CanExecute_SinFuncion_RetornaTrue()
    {
        var cmd = new RelayCommand(() => { });
        Assert.True(cmd.CanExecute(null));
    }

    [Fact]
    public void CanExecute_ConFuncionFalsa_RetornaFalse()
    {
        var cmd = new RelayCommand(() => { }, () => false);
        Assert.False(cmd.CanExecute(null));
    }

    [Fact]
    public void CanExecute_ConFuncionVerdadera_RetornaTrue()
    {
        var cmd = new RelayCommand(() => { }, () => true);
        Assert.True(cmd.CanExecute(null));
    }

    [Fact]
    public void RaiseCanExecuteChanged_DisparaEvento()
    {
        var cmd = new RelayCommand(() => { });
        bool fired = false;
        cmd.CanExecuteChanged += (_, _) => fired = true;

        cmd.RaiseCanExecuteChanged();

        Assert.True(fired);
    }
}

// TEST-041 a TEST-043
public class RelayCommandTTests
{
    [Fact]
    public void Execute_ConParametroCorrecto_LlamaAccion()
    {
        string? received = null;
        var cmd = new RelayCommand<string>(s => received = s);

        cmd.Execute("hola");

        Assert.Equal("hola", received);
    }

    [Fact]
    public void Execute_ConParametroTipoIncorrecto_NoLanzaExcepcion()
    {
        var cmd = new RelayCommand<string>(_ => { });
        var ex = Record.Exception(() => cmd.Execute(123));
        Assert.Null(ex);
    }

    [Fact]
    public void CanExecute_ConParametroTipoIncorrecto_RetornaFalse()
    {
        var cmd = new RelayCommand<string>(_ => { }, _ => true);
        Assert.False(cmd.CanExecute(123));
    }

    [Fact]
    public void RaiseCanExecuteChanged_DisparaEvento()
    {
        var cmd = new RelayCommand<string>(_ => { });
        bool fired = false;
        cmd.CanExecuteChanged += (_, _) => fired = true;

        cmd.RaiseCanExecuteChanged();

        Assert.True(fired);
    }
}

// TEST-044 a TEST-045
public class SelectableItemTests
{
    [Fact]
    public void IsSelected_CambioATrue_DisparaPropertyChanged()
    {
        var item = new SelectableItem<string>("valor");
        string? propertyName = null;
        item.PropertyChanged += (_, e) => propertyName = e.PropertyName;

        item.IsSelected = true;

        Assert.Equal(nameof(SelectableItem<string>.IsSelected), propertyName);
    }

    [Fact]
    public void IsSelected_MismoValor_NoDisparaEvento()
    {
        var item = new SelectableItem<string>("valor");
        int count = 0;
        item.PropertyChanged += (_, _) => count++;

        item.IsSelected = false; // ya es false por defecto

        Assert.Equal(0, count);
    }

    [Fact]
    public void Value_RetornaValorOriginal()
    {
        var item = new SelectableItem<int>(42);
        Assert.Equal(42, item.Value);
    }
}
