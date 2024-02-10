        using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Axelerate.Command
{
    public class MyCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;


        private readonly Action<object> _excute;

        private readonly Predicate<object> _canExcute;



        public MyCommand(Action<Object> excute, Predicate<Object> canExcute)
        {
            _excute = excute;

            _canExcute = canExcute;


        }
        public bool CanExecute(object parameter)
        {
            return _canExcute(parameter);
        }

        public void Execute(object parameter)
        {
            _excute(parameter);
        }
    }
}
