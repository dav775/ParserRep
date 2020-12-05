using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Parser
{
    //Класс модели со сведениями об обрабатываемом файле
    public class ProcFile : IProcFile, INotifyPropertyChanged
    {
        private string fileName;
        private int completion;
        private string message;

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public int Completion
        {
            get { return completion; }
            set
            {
                completion = value;
                OnPropertyChanged("Completion");
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }
        //Определяем член-события
        public event PropertyChangedEventHandler PropertyChanged;
        //Метод отвечает за уведомление о наступлении события зарегистрированных объектов
        protected virtual void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            //Реализуем метод потокобезопасным методом, копируем ссылку на член-события
            PropertyChangedEventHandler handler = Interlocked.CompareExchange(ref PropertyChanged, null, null);
            //Проверяем существуют ли подписчики на событие, если да - уведомляем их о наступлении события
            handler?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
