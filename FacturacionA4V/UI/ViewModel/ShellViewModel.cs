using System.Windows.Input;

namespace FacturacionA4V.UI.ViewModel;

public sealed class ShellViewModel : ObservableObject
{
    private object _currentViewModel;
    public object CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    public ICommand IrAInicioCommand { get; }
    public ICommand IrACargaCommand { get; }
    public ICommand IrAResultadosCommand { get; }

    private readonly Func<object> _inicioFactory;
    private readonly Func<object> _cargaFactory;
    private readonly Func<object> _resultadosFactory;

    public ShellViewModel(
        Func<object> inicioFactory,
        Func<object> cargaFactory,
        Func<object> resultadosFactory)
    {
        _inicioFactory = inicioFactory;
        _cargaFactory = cargaFactory;
        _resultadosFactory = resultadosFactory;

        IrAInicioCommand = new RelayCommand(() => CurrentViewModel = _inicioFactory());
        IrACargaCommand = new RelayCommand(() => CurrentViewModel = _cargaFactory());
        IrAResultadosCommand = new RelayCommand(() => CurrentViewModel = _resultadosFactory());

        // pantalla inicial
        CurrentViewModel = _inicioFactory();
    }
}