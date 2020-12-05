using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Parser
{
    //Класс модели конфигурации для менеджеров конфигурац XML и JSON реалиует интерфейсы IConfig и INotifyPropertyChanged
    public class Config : IConfig, INotifyPropertyChanged
    {

        private bool processingType;
        private string workDir;
        private TimeSpan interval;

        public bool ProcessingType
        {
            get { return processingType; }
            set
            {
                processingType = value;
                OnPropertyChanged("ProcessingType");
            }
        }

        public string WorkDir
        {
            get { return workDir; }
            set
            {
                workDir = value;
                OnPropertyChanged("WorkDir");
            }
        }

        public TimeSpan Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                OnPropertyChanged("Interval");
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
