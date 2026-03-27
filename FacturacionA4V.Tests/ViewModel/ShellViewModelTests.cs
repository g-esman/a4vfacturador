using FacturacionA4V.UI.ViewModel;

namespace FacturacionA4V.Tests.ViewModel;

// TEST-080 a TEST-084
public class ShellViewModelTests
{
    private readonly object _inicio = new();
    private readonly object _carga = new();
    private readonly object _resultados = new();

    private ShellViewModel CreateVm() =>
        new ShellViewModel(() => _inicio, () => _carga, () => _resultados);

    // TEST-080
    [Fact]
    public void Constructor_CurrentViewModelEsInicio()
    {
        var vm = CreateVm();
        Assert.Same(_inicio, vm.CurrentViewModel);
    }

    // TEST-081
    [Fact]
    public void IrACarga_CambiaCurrentViewModel()
    {
        var vm = CreateVm();
        vm.IrACargaCommand.Execute(null);
        Assert.Same(_carga, vm.CurrentViewModel);
    }

    // TEST-082
    [Fact]
    public void IrAResultados_CambiaCurrentViewModel()
    {
        var vm = CreateVm();
        vm.IrAResultadosCommand.Execute(null);
        Assert.Same(_resultados, vm.CurrentViewModel);
    }

    // TEST-083
    [Fact]
    public void IrAInicio_DesdeCarga_VuelveAInicio()
    {
        var vm = CreateVm();
        vm.IrACargaCommand.Execute(null);
        vm.IrAInicioCommand.Execute(null);
        Assert.Same(_inicio, vm.CurrentViewModel);
    }

    // TEST-084
    [Fact]
    public void Navegacion_DisparaPropertyChangedDeCurrentViewModel()
    {
        var vm = CreateVm();
        string? propertyName = null;
        vm.PropertyChanged += (_, e) => propertyName = e.PropertyName;

        vm.IrACargaCommand.Execute(null);

        Assert.Equal(nameof(ShellViewModel.CurrentViewModel), propertyName);
    }
}
