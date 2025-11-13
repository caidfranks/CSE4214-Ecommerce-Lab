using System;
using System.Collections.Generic;
using System.Linq;

namespace GameVault.Client.Services
{
    public class BreadcrumbService
    {
        private List<BreadcrumbItem> _items = new();
        public IReadOnlyList<BreadcrumbItem> Items => _items.AsReadOnly();
        public event Action? OnChange;

        public void Set(params BreadcrumbItem[] items)
        {
            _items = items.ToList();
            OnChange?.Invoke();
        }

        public void Clear()
        {
            _items.Clear();
            OnChange?.Invoke();
        }
    }

    public class BreadcrumbItem
    {
        public string Text { get; set; } = "";
        public string? Href { get; set; }
        public bool IsActive => string.IsNullOrEmpty(Href);

        public BreadcrumbItem(string text, string? href = null)
        {
            Text = text;
            Href = href;
        }
    }
}