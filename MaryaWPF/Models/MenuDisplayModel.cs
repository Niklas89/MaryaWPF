using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Models;

public class MenuDisplayModel 
{
    public string? Title { get; set; }
    public PackIconKind SelectedIcon { get; set; }
    public PackIconKind UnselectedIcon { get; set; }
    private object? _notification = null;

    public object? Notification
    {
        get { return _notification; }
       // set { }
        // set { SetProperty(ref _notification, value); }
    }

}
