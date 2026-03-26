namespace FacturacionA4V.UI.ViewModel
{
    public sealed class SelectableItem<T> : ObservableObject
    {
        public T Value { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public SelectableItem(T value)
        {
            Value = value;
        }
    }
}
